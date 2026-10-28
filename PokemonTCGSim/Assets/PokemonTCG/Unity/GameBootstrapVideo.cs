using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;
using Debug = UnityEngine.Debug;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Learning from video (v77). A video file is opened in Unity's VideoPlayer and read a moment at a time (every second by
    /// default) through a layout: boxes drawn once per kind of video over the names, HP, turn counters, Prize card icons,
    /// clocks and cards. Text is read by Windows' built-in OCR (a PowerShell script, Japanese or English), Prize card icons
    /// are counted, cards are recognised by their artwork. The readings become events (VideoTimeline) that you can check,
    /// fix and add to by hand, and the result is studied as a TCG Live battle log (LogStudy). See the screen in
    /// GameBootstrapVideoScreen.cs.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string PokeApiSpeciesUrl = "https://raw.githubusercontent.com/PokeAPI/pokeapi/master/data/v2/csv/pokemon_species_names.csv";
        private const string SpeciesFileName = "pokemon_species_names.csv";
        private const string VideoNamesFileName = "names.txt";
        private const string ArtCacheFileName = "card art.txt";

        // Read on the main thread (SavePaths uses PlayerPrefs).
        private static string VideoFolder { get { return Path.Combine(SavePaths.BrainFolder, "Videos"); } }
        private static string LayoutFolder { get { return Path.Combine(VideoFolder, "Layouts"); } }
        /// <summary>Videos downloaded from YouTube links, and the downloader (yt-dlp).</summary>
        private static string DownloadFolder { get { return Path.Combine(VideoFolder, "Downloads"); } }
        private static string ToolsFolder { get { return Path.Combine(VideoFolder, "Tools"); } }

        private GameObject videoObject;
        private VideoPlayer videoPlayer;
        private RenderTexture videoFrame;
        private Texture2D videoReadback;
        private bool videoReady, videoSeekDone, videoFrameArrived, videoNeedsCopy;
        private float videoSeekDoneAt;
        private string videoMessage = "";
        private string videoPathInput = "";
        private bool pendingVideoBrowse;
        private VideoSession videoSession;
        private string videoSessionPath;
        private readonly List<VideoLayout> videoLayouts = new List<VideoLayout>();
        private int videoLayoutIndex;
        private bool videoLayoutDirty;
        private CardNameReader videoNames;
        private UnityWebRequest videoNamesDownload;
        private List<KeyValuePair<string, int>> videoUnknown = new List<KeyValuePair<string, int>>();
        private readonly Dictionary<string, string> videoNameFixes = new Dictionary<string, string>();
        private volatile bool videoStudyRunning;
        // A YouTube link: the download in progress, the downloader being fetched, and the link waiting for it.
        private VideoDownload youTube;
        private UnityWebRequest youTubeToolDownload;
        private string youTubeWaiting;
        private bool youTubeNeedsTool;
        private string videoStudyResult = "";

        // Card recognition: fingerprints of the card images, built a few at a time and saved.
        private CardArtIndex videoArt;
        private List<KeyValuePair<string, string>> videoArtQueue;
        private int videoArtPos;
        private Func<string, string> videoArtStamp;

        private VideoLayout VideoLayoutNow
        {
            get
            {
                if (videoLayouts.Count == 0) LoadVideoLayouts();
                videoLayoutIndex = Mathf.Clamp(videoLayoutIndex, 0, videoLayouts.Count - 1);
                return videoLayouts[videoLayoutIndex];
            }
        }

        private static bool OcrAvailable
        {
            get { return Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor; }
        }

        // ------------------------------------------------------------------ Opening and leaving the screen

        private void OpenVideoScreen()
        {
            RefreshDecks();
            if (videoLayouts.Count == 0) LoadVideoLayouts();
            if (videoNames == null) LoadVideoNames();
            mode = Mode.Video;
        }

        private void CloseVideoScreen()
        {
            if (videoJob != null) videoJob.Cancelled = true;
            videoJob = null;
            if (youTube != null) youTube.Cancel();
            SaveVideoSession();
            if (videoLayoutDirty && !VideoLayoutNow.BuiltIn) SaveVideoLayout(VideoLayoutNow);
            StopVideo();
            OpenTrainScreen();
        }

        /// <summary>Called from Update.</summary>
        private void UpdateVideo()
        {
            if (pendingVideoBrowse)
            {
                pendingVideoBrowse = false;
                string start = string.IsNullOrEmpty(videoPathInput) ? "" : Path.GetDirectoryName(videoPathInput);
                string picked = FilePicker.PickVideo("Choose a video of a game", start);
                if (!string.IsNullOrEmpty(picked))
                {
                    videoPathInput = picked;
                    OpenVideo(picked);
                }
            }
            UpdateNamesDownload();
            UpdateYouTube();
            if (mode != Mode.Video) return;
            UpdateArtIndex();
            if (videoPlayer != null && videoReady && videoPlayer.texture != null && (videoPlayer.isPlaying || videoNeedsCopy))
            {
                Graphics.Blit(videoPlayer.texture, videoFrame);
                videoNeedsCopy = false;
            }
            if (videoJob != null) StepVideoJob();
        }

        // ------------------------------------------------------------------ A file or a YouTube link

        /// <summary>The Open button: a video file, or a YouTube link (downloaded first, then opened).</summary>
        private void OpenVideoInput(string text)
        {
            if (VideoDownload.IsYouTubeLink(text)) OpenYouTube(text);
            else OpenVideo(text);
        }

        private bool YouTubeBusy { get { return youTube != null || youTubeToolDownload != null; } }

        private void OpenYouTube(string link)
        {
            if (YouTubeBusy) return;
            string id = VideoDownload.YouTubeId(link);
            string already = VideoDownload.FindDownloaded(DownloadFolder, id);
            if (already != null)
            {
                OpenVideo(already);
                return;
            }
            if (!OcrAvailable)
            {
                videoMessage = "YouTube links work on Windows (the video reading does too). Download the video yourself and open the file.";
                return;
            }
            string tool = VideoDownload.FindTool(ToolsFolder);
            if (tool == null)
            {
                youTubeWaiting = link;
                youTubeNeedsTool = true;
                videoMessage = "YouTube links need yt-dlp, a free downloader (about 18 MB, from its GitHub page). Click \"Get yt-dlp\" once and the video downloads after it.";
                return;
            }
            youTubeNeedsTool = false;
            youTube = new VideoDownload(id, DownloadFolder);
            youTube.Start(tool);
            videoMessage = youTube.Status;
        }

        /// <summary>"Get yt-dlp": downloads it into the Videos folder's Tools folder.</summary>
        private void StartYouTubeToolDownload()
        {
            if (youTubeToolDownload != null) return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                videoMessage = "This computer isn't connected to the internet. You can also save " + VideoDownload.ToolFileName + " from yt-dlp's GitHub page into " + ToolsFolder + " yourself.";
                return;
            }
            Directory.CreateDirectory(ToolsFolder);
            string target = Path.Combine(ToolsFolder, VideoDownload.ToolFileName + ".download");
            youTubeToolDownload = new UnityWebRequest(VideoDownload.ToolUrl, UnityWebRequest.kHttpVerbGET)
            {
                downloadHandler = new DownloadHandlerFile(target) { removeFileOnAbort = true }
            };
            youTubeToolDownload.SendWebRequest();
            videoMessage = "Getting yt-dlp...";
        }

        /// <summary>Called from Update: the downloader's download, then the video's.</summary>
        private void UpdateYouTube()
        {
            var request = youTubeToolDownload;
            if (request != null && request.isDone)
            {
                youTubeToolDownload = null;
                string target = Path.Combine(ToolsFolder, VideoDownload.ToolFileName);
                string part = target + ".download";
                try
                {
                    if (request.result != UnityWebRequest.Result.Success)
                        videoMessage = "Couldn't get yt-dlp: " + request.error;
                    else if (!File.Exists(part) || new FileInfo(part).Length < 1000000)
                        videoMessage = "The yt-dlp download didn't look right, so it wasn't kept.";
                    else
                    {
                        if (File.Exists(target)) File.Delete(target);
                        File.Move(part, target);
                        youTubeNeedsTool = false;
                        string link = youTubeWaiting;
                        youTubeWaiting = null;
                        if (link != null) OpenYouTube(link);
                        else videoMessage = "yt-dlp is ready.";
                    }
                }
                catch (Exception e)
                {
                    videoMessage = "Couldn't keep yt-dlp: " + e.Message;
                }
                finally
                {
                    request.Dispose();
                    try { if (File.Exists(part)) File.Delete(part); }
                    catch (Exception) { }
                }
            }
            var download = youTube;
            if (download == null) return;
            if (!download.Done)
            {
                videoMessage = download.Status;
                return;
            }
            youTube = null;
            if (download.FilePath != null) OpenVideo(download.FilePath);
            else videoMessage = download.Cancelled ? "The download was cancelled." : "Couldn't download the video: " + download.Error;
        }

        // ------------------------------------------------------------------ The video player

        private void OpenVideo(string path)
        {
            StopVideo();
            path = (path ?? "").Trim().Trim('"');
            if (path.Length == 0) return;
            if (!File.Exists(path))
            {
                videoMessage = "There's no file at " + path + ".";
                return;
            }
            videoObject = new GameObject("Video study player");
            videoPlayer = videoObject.AddComponent<VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = path;
            videoPlayer.renderMode = VideoRenderMode.APIOnly;
            videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
            videoPlayer.isLooping = false;
            videoPlayer.skipOnDrop = true;
            videoPlayer.waitForFirstFrame = true;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.prepareCompleted += source => OnVideoPrepared();
            videoPlayer.errorReceived += (source, message) => videoMessage = "The video couldn't be played: " + message
                + " (Unity plays H.264 MP4 best; re-save the video as MP4 if it's another format.)";
            videoPlayer.seekCompleted += source =>
            {
                videoSeekDone = true;
                videoSeekDoneAt = Time.unscaledTime;
                videoNeedsCopy = true;
            };
            videoPlayer.frameReady += (source, frameIndex) =>
            {
                videoFrameArrived = true;
                videoNeedsCopy = true;
            };
            videoReady = false;
            videoMessage = "Opening " + Path.GetFileName(path) + "...";
            videoPlayer.Prepare();
            LoadVideoSessionFor(path);
        }

        private void OnVideoPrepared()
        {
            if (videoPlayer == null) return;
            int w = (int)videoPlayer.width, h = (int)videoPlayer.height;
            if (w <= 0 || h <= 0)
            {
                w = 1920;
                h = 1080;
            }
            // Read at no more than 1920 x 1080 (what the layouts are measured on; more only costs time).
            float scale = Mathf.Min(1f, Mathf.Min(1920f / w, 1080f / h));
            int rw = Mathf.Max(16, Mathf.RoundToInt(w * scale)), rh = Mathf.Max(16, Mathf.RoundToInt(h * scale));
            if (videoFrame != null) videoFrame.Release();
            videoFrame = new RenderTexture(rw, rh, 0, RenderTextureFormat.ARGB32);
            videoFrame.Create();
            videoReady = true;
            videoMessage = Path.GetFileName(videoPlayer.url) + ": " + w + " x " + h + ", " + VideoEvent.Clock(videoPlayer.length) + ".";
            SeekVideo(0.0);
        }

        private void StopVideo()
        {
            if (videoPlayer != null) videoPlayer.Stop();
            if (videoObject != null) Destroy(videoObject);
            videoObject = null;
            videoPlayer = null;
            videoReady = false;
            if (videoFrame != null)
            {
                videoFrame.Release();
                Destroy(videoFrame);
            }
            videoFrame = null;
        }

        private void SeekVideo(double seconds)
        {
            if (videoPlayer == null || !videoReady) return;
            double length = videoPlayer.length;
            if (length > 0) seconds = Math.Max(0, Math.Min(length - 0.05, seconds));
            videoSeekDone = false;
            videoFrameArrived = false;
            videoPlayer.time = seconds;
        }

        private double VideoTime { get { return videoPlayer != null && videoReady ? videoPlayer.time : 0; } }
        private double VideoLength { get { return videoPlayer != null && videoReady ? videoPlayer.length : 0; } }

        /// <summary>The frame being shown, as pixels (rows from the top).</summary>
        private VideoImage ReadVideoFrame()
        {
            int w = videoFrame.width, h = videoFrame.height;
            if (videoReadback == null || videoReadback.width != w || videoReadback.height != h)
            {
                if (videoReadback != null) Destroy(videoReadback);
                videoReadback = new Texture2D(w, h, TextureFormat.RGBA32, false);
            }
            var previous = RenderTexture.active;
            RenderTexture.active = videoFrame;
            videoReadback.ReadPixels(new Rect(0, 0, w, h), 0, 0, false);
            videoReadback.Apply(false);
            RenderTexture.active = previous;
            return ToVideoImage(videoReadback.GetPixels32(), w, h);
        }

        /// <summary>Unity's pixels (rows from the bottom) as a VideoImage (rows from the top).</summary>
        private static VideoImage ToVideoImage(Color32[] pixels, int w, int h)
        {
            var bytes = new byte[w * h * 4];
            for (int y = 0; y < h; y++)
            {
                int source = (h - 1 - y) * w;
                for (int x = 0; x < w; x++)
                {
                    var c = pixels[source + x];
                    int i = (y * w + x) * 4;
                    bytes[i] = c.r;
                    bytes[i + 1] = c.g;
                    bytes[i + 2] = c.b;
                    bytes[i + 3] = 255;
                }
            }
            return new VideoImage(w, h, bytes);
        }

        // ------------------------------------------------------------------ Reading the video

        /// <summary>A reading of the video in progress: seek, wait for the frame, read the boxes; text goes to the OCR in batches.</summary>
        private sealed class VideoJob
        {
            public double Next, End, Step;
            /// <summary>0 seek the next moment, 1 waiting for its frame, 2 waiting for the last OCR results.</summary>
            public int State;
            public float WaitingSince;
            public int Read, Total;
            public bool Cancelled;
            public string Folder;
            public string Language;
            public bool Ocr;
            public int FileNumber;
            public VideoLayout Layout;
            public HashSet<string> AllowedCards;
            public readonly Dictionary<string, byte[]> Signature = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            public readonly Dictionary<string, string> LastFile = new Dictionary<string, string>(StringComparer.Ordinal);
            public readonly Dictionary<string, byte[]> ArtSignature = new Dictionary<string, byte[]>(StringComparer.Ordinal);
            public readonly Dictionary<string, string> LastArt = new Dictionary<string, string>(StringComparer.Ordinal);
            public readonly List<KeyValuePair<FrameReading, Dictionary<string, string>>> Waiting = new List<KeyValuePair<FrameReading, Dictionary<string, string>>>();
            public readonly List<string> NewFiles = new List<string>();
            public readonly Dictionary<string, string> Results = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public readonly object Gate = new object();
            public volatile bool OcrRunning;
            public volatile string OcrError;
            public readonly List<FrameReading> Done = new List<FrameReading>();
        }

        private VideoJob videoJob;

        private void StartVideoReading()
        {
            if (videoPlayer == null || !videoReady || videoSession == null || videoJob != null) return;
            var layout = VideoLayoutNow;
            if (layout.Boxes.Count == 0)
            {
                videoMessage = "The layout has no boxes yet: draw them on the Layout tab first.";
                return;
            }
            bool needsText = layout.Boxes.Any(b => b.Kind == VideoBoxKind.Text || b.Kind == VideoBoxKind.Number);
            bool needsArt = layout.Boxes.Any(b => b.Kind == VideoBoxKind.CardArt);
            if (needsArt) StartArtIndex();
            videoPlayer.Pause();
            double end = videoSession.Stop > 0 ? Math.Min(videoSession.Stop, VideoLength) : VideoLength;
            var job = new VideoJob
            {
                Next = Math.Max(0, videoSession.Start),
                End = Math.Max(0, end - 0.05),
                Step = Math.Max(0.25, videoSession.Interval),
                Layout = layout.Clone(layout.Name),
                Language = layout.Language == "ja" ? "ja" : "en",
                Ocr = needsText && OcrAvailable,
                Folder = Path.Combine(Application.temporaryCachePath, "Video OCR " + DateTime.Now.ToString("HHmmss")),
                AllowedCards = AllowedArtCards()
            };
            job.Total = Math.Max(1, (int)Math.Floor((job.End - job.Next) / job.Step) + 1);
            try
            {
                Directory.CreateDirectory(job.Folder);
                if (job.Ocr) File.WriteAllText(Path.Combine(job.Folder, "ocr.ps1"), VideoOcr.Script, new UTF8Encoding(true));
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't make a working folder for the reading: " + e.Message;
                return;
            }
            videoSession.LayoutName = layout.Name;
            videoMessage = needsText && !OcrAvailable
                ? "Reading without text: Windows' OCR is only on Windows, so only Prize card icons and cards are read here."
                : "Reading the video...";
            videoJob = job;
        }

        /// <summary>The cards of the decks you picked for the players (card recognition then only looks at those), or null for every card.</summary>
        private HashSet<string> AllowedArtCards()
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var deckName in videoSession.Decks)
            {
                if (string.IsNullOrEmpty(deckName)) continue;
                var deck = decks.FirstOrDefault(d => d.Name == deckName);
                if (deck == null) continue;
                foreach (var entry in Decklist.Parse(deck.Text, db, new List<string>()))
                    if (entry.Card != null) names.Add(entry.Card.Name);
            }
            return names.Count > 0 ? names : null;
        }

        private void StepVideoJob()
        {
            var job = videoJob;
            if (job.Cancelled)
            {
                videoJob = null;
                videoMessage = "Reading stopped; the events from before are kept.";
                CleanVideoFolder(job.Folder);
                return;
            }
            if (job.OcrError != null)
            {
                videoJob = null;
                videoMessage = job.OcrError + (job.Language == "ja" ? " " + VideoOcr.JapaneseHelp : "");
                CleanVideoFolder(job.Folder);
                return;
            }
            switch (job.State)
            {
                case 0:
                    if (job.Next > job.End + 1e-6)
                    {
                        job.State = 2;
                        SendToOcr(job);
                        break;
                    }
                    SeekVideo(job.Next);
                    job.WaitingSince = Time.unscaledTime;
                    job.State = 1;
                    break;
                case 1:
                {
                    float now = Time.unscaledTime;
                    bool ready = videoSeekDone && (videoFrameArrived || now - videoSeekDoneAt > 0.3f);
                    if (!ready && now - job.WaitingSince < 3f) break;
                    if (videoPlayer.texture != null) Graphics.Blit(videoPlayer.texture, videoFrame);
                    ReadMoment(job, job.Next);
                    job.Read++;
                    job.Next += job.Step;
                    job.State = 0;
                    if (job.NewFiles.Count >= 150) SendToOcr(job);
                    ResolveReadings(job);
                    break;
                }
                case 2:
                    if (!job.OcrRunning && job.NewFiles.Count > 0) SendToOcr(job);
                    ResolveReadings(job);
                    if (!job.OcrRunning && job.NewFiles.Count == 0 && job.Waiting.Count == 0) FinishVideoJob(job);
                    break;
            }
        }

        /// <summary>Reads every box of the frame now shown. Text boxes that look the same as last time reuse that OCR result.</summary>
        private void ReadMoment(VideoJob job, double time)
        {
            var frame = ReadVideoFrame();
            var reading = new FrameReading { Time = Math.Round(time, 3) };
            var files = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (var box in job.Layout.Boxes)
            {
                var crop = frame.CropNormalized(box.X, box.Y, box.W, box.H);
                switch (box.Kind)
                {
                    case VideoBoxKind.Icons:
                        reading.Counts[box.Role] = crop.CountFilledIcons(box.Slots);
                        break;
                    case VideoBoxKind.CardArt:
                    {
                        if (videoArt == null || videoArt.Count == 0) break;
                        var sig = crop.Signature();
                        byte[] last;
                        string known;
                        if (job.ArtSignature.TryGetValue(box.Role, out last) && VideoImage.SignatureDifference(sig, last) < 6 && job.LastArt.TryGetValue(box.Role, out known))
                        {
                            reading.Art[box.Role] = known;
                            break;
                        }
                        var match = videoArt.Match(frame, box.X, box.Y, box.W, box.H, job.AllowedCards);
                        string name = match.Sure ? match.Name : "";
                        job.ArtSignature[box.Role] = sig;
                        job.LastArt[box.Role] = name;
                        reading.Art[box.Role] = name;
                        break;
                    }
                    default:
                    {
                        if (!job.Ocr) break;
                        var sig = crop.Signature();
                        byte[] last;
                        string file;
                        if (job.Signature.TryGetValue(box.Role, out last) && VideoImage.SignatureDifference(sig, last) < 4 && job.LastFile.TryGetValue(box.Role, out file))
                        {
                            files[box.Role] = file;
                            break;
                        }
                        file = Path.Combine(job.Folder, (job.FileNumber++).ToString("000000") + ".bmp");
                        try
                        {
                            File.WriteAllBytes(file, VideoOcr.Prepare(crop).ToBmp());
                        }
                        catch (IOException)
                        {
                            break;
                        }
                        job.NewFiles.Add(file);
                        job.Signature[box.Role] = sig;
                        job.LastFile[box.Role] = file;
                        files[box.Role] = file;
                        break;
                    }
                }
            }
            job.Waiting.Add(new KeyValuePair<FrameReading, Dictionary<string, string>>(reading, files));
        }

        /// <summary>Sends the pictures waiting for OCR to Windows' OCR on another thread.</summary>
        private void SendToOcr(VideoJob job)
        {
            if (job.OcrRunning || job.NewFiles.Count == 0) return;
            var batch = new List<string>(job.NewFiles);
            job.NewFiles.Clear();
            job.OcrRunning = true;
            string folder = job.Folder, language = job.Language;
            int batchNumber = job.FileNumber;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    string error;
                    var results = RunWindowsOcr(batch, folder, language, batchNumber, out error);
                    lock (job.Gate)
                    {
                        foreach (var f in batch)
                        {
                            string text;
                            job.Results[f] = results != null && results.TryGetValue(f, out text) ? text : null;
                        }
                    }
                    if (error != null) job.OcrError = error;
                }
                catch (Exception e)
                {
                    job.OcrError = "Windows' OCR couldn't be run: " + e.Message;
                }
                finally
                {
                    job.OcrRunning = false;
                }
            });
        }

        /// <summary>Runs the OCR script over the pictures (Windows PowerShell 5.1), and reads its results.</summary>
        private static Dictionary<string, string> RunWindowsOcr(List<string> pictures, string folder, string language, int batchNumber, out string error)
        {
            error = null;
            string list = Path.Combine(folder, "list " + batchNumber + ".txt");
            string output = Path.Combine(folder, "results " + batchNumber + ".txt");
            File.WriteAllText(list, VideoOcr.ListFile(pictures), new UTF8Encoding(false));
            string powershell = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell", "v1.0", "powershell.exe");
            if (!File.Exists(powershell)) powershell = "powershell.exe";
            var info = new System.Diagnostics.ProcessStartInfo
            {
                FileName = powershell,
                Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -File \"" + Path.Combine(folder, "ocr.ps1") + "\" -List \"" + list
                            + "\" -Out \"" + output + "\" -Lang " + language,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };
            string stderr;
            using (var process = System.Diagnostics.Process.Start(info))
            {
                if (process == null)
                {
                    error = "Windows PowerShell couldn't be started.";
                    return null;
                }
                process.OutputDataReceived += (sender, args) => { };
                process.BeginOutputReadLine();
                stderr = process.StandardError.ReadToEnd();
                if (!process.WaitForExit(10 * 60 * 1000))
                {
                    try { process.Kill(); }
                    catch (Exception) { }
                    error = "Windows' OCR took too long.";
                    return null;
                }
            }
            if (!File.Exists(output))
            {
                error = "Windows' OCR didn't answer" + (string.IsNullOrEmpty(stderr) ? "." : ": " + stderr.Trim());
                return null;
            }
            string parseError;
            var results = VideoOcr.ParseResults(File.ReadAllText(output, Encoding.UTF8), out parseError);
            if (parseError != null) error = parseError;
            return results;
        }

        /// <summary>Moves readings whose OCR results are all back (in order) to the finished list.</summary>
        private void ResolveReadings(VideoJob job)
        {
            lock (job.Gate)
            {
                while (job.Waiting.Count > 0)
                {
                    var item = job.Waiting[0];
                    bool ready = true;
                    foreach (var kv in item.Value)
                        if (!job.Results.ContainsKey(kv.Value)) ready = false;
                    if (!ready) break;
                    foreach (var kv in item.Value)
                    {
                        string text = job.Results[kv.Value];
                        // A picture the OCR couldn't read isn't a reading at all (an empty one would mean "nothing there").
                        if (text != null) item.Key.Text[kv.Key] = text;
                    }
                    job.Done.Add(item.Key);
                    job.Waiting.RemoveAt(0);
                }
            }
        }

        private void FinishVideoJob(VideoJob job)
        {
            videoJob = null;
            CleanVideoFolder(job.Folder);
            if (videoSession == null) return;
            // Readings from this stretch of the video replace the old ones there; the rest are kept.
            double from = job.Done.Count > 0 ? job.Done.Min(r => r.Time) : 0, to = job.Done.Count > 0 ? job.Done.Max(r => r.Time) : 0;
            videoSession.Readings.RemoveAll(r => r.Time >= from - 1e-6 && r.Time <= to + 1e-6);
            videoSession.Readings.AddRange(job.Done);
            videoSession.Readings.Sort((x, y) => x.Time.CompareTo(y.Time));
            RebuildVideoEvents();
            videoMessage = "Read " + job.Done.Count + " moments: " + videoSession.Events.Count(e => !e.Manual) + " events found."
                           + (videoUnknown.Count > 0 ? " Some names weren't recognised: fix them on the Read tab." : "");
            videoTab = 2;
        }

        private static void CleanVideoFolder(string folder)
        {
            try
            {
                if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder)) Directory.Delete(folder, true);
            }
            catch (Exception)
            {
                // Windows may still hold a file for a moment; the temporary folder is cleaned up by the system anyway.
            }
        }

        /// <summary>Finds the events again from the saved readings (after a name fix, a new first player...). Tagged events are kept.</summary>
        private void RebuildVideoEvents()
        {
            if (videoSession == null) return;
            var layout = FindVideoLayout(videoSession.LayoutName) ?? VideoLayoutNow;
            videoSession.Rebuild(layout, videoNames, db, new VideoTimelineOptions());
            videoUnknown = videoSession.UnknownNames(layout, videoNames);
            var shown = videoSession.NamesOnScreen();
            for (int p = 0; p < 2; p++)
                if (string.IsNullOrEmpty(videoSession.Names[p]) && shown[p].Length > 0) videoSession.Names[p] = shown[p];
            SaveVideoSession();
        }

        // ------------------------------------------------------------------ Names

        private void LoadVideoNames()
        {
            videoNames = CardNameReader.ForDatabase(db);
            try
            {
                string species = Path.Combine(VideoFolder, SpeciesFileName);
                if (File.Exists(species)) videoNames.LoadPokeApiSpecies(File.ReadAllText(species, Encoding.UTF8));
                string fixes = Path.Combine(VideoFolder, VideoNamesFileName);
                if (File.Exists(fixes)) videoNames.LoadTable(File.ReadAllText(fixes, Encoding.UTF8));
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't read the name tables: " + e.Message;
            }
        }

        /// <summary>Downloads PokeAPI's list of Pokémon names (a small text file) for reading Japanese names.</summary>
        private void StartNamesDownload()
        {
            if (videoNamesDownload != null) return;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                videoMessage = "This computer isn't connected to the internet. You can also save " + SpeciesFileName + " from PokeAPI's GitHub into " + VideoFolder + " yourself.";
                return;
            }
            videoNamesDownload = UnityWebRequest.Get(PokeApiSpeciesUrl);
            videoNamesDownload.SendWebRequest();
            videoMessage = "Getting the Japanese Pokémon names...";
        }

        private void UpdateNamesDownload()
        {
            var request = videoNamesDownload;
            if (request == null || !request.isDone) return;
            videoNamesDownload = null;
            try
            {
                if (request.result != UnityWebRequest.Result.Success)
                {
                    videoMessage = "Couldn't get the names: " + request.error;
                    return;
                }
                string text = request.downloadHandler.text ?? "";
                var check = new CardNameReader(new string[0]);
                int added = check.LoadPokeApiSpecies(text);
                if (added < 500)
                {
                    videoMessage = "The names file didn't look right, so it wasn't kept.";
                    return;
                }
                Directory.CreateDirectory(VideoFolder);
                File.WriteAllText(Path.Combine(VideoFolder, SpeciesFileName), text, new UTF8Encoding(false));
                LoadVideoNames();
                videoMessage = "Got " + videoNames.SpeciesCount + " Japanese Pokémon names.";
                RebuildVideoEvents();
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't save the names: " + e.Message;
            }
            finally
            {
                request.Dispose();
            }
        }

        /// <summary>A name you fixed: what the screen showed and the card it is. Saved, so every video after this reads it right.</summary>
        private void SaveVideoNameFix(string shown, string typed)
        {
            double score;
            string card = videoNames.MatchPool(typed ?? "", out score);
            if (card == null)
            {
                videoMessage = "\"" + typed + "\" isn't a card in the pool (type its English name).";
                return;
            }
            videoNames.AddName(shown, card);
            try
            {
                Directory.CreateDirectory(VideoFolder);
                File.AppendAllText(Path.Combine(VideoFolder, VideoNamesFileName), shown.Replace('\t', ' ') + "\t" + card + "\n", new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't save the name: " + e.Message;
            }
            videoNameFixes.Remove(shown);
            RebuildVideoEvents();
            videoMessage = "\"" + shown + "\" is " + card + " from now on.";
        }

        // ------------------------------------------------------------------ Card recognition

        private void StartArtIndex()
        {
            if (videoArt != null) return;
            videoArt = new CardArtIndex();
            var files = CardVisuals.ImageFiles();
            videoArtStamp = key =>
            {
                string file;
                if (!files.TryGetValue(key, out file)) return "";
                try
                {
                    var fi = new FileInfo(file);
                    return CardArtIndex.Stamp(fi.Length, fi.LastWriteTimeUtc);
                }
                catch (Exception)
                {
                    return "";
                }
            };
            try
            {
                string cache = Path.Combine(VideoFolder, ArtCacheFileName);
                if (File.Exists(cache)) videoArt.Load(File.ReadAllText(cache), videoArtStamp);
            }
            catch (Exception)
            {
                // A broken cache is rebuilt.
            }
            videoArtQueue = files.Where(kv => !videoArt.Contains(kv.Key) && db.Get(kv.Key) != null).ToList();
            videoArtPos = 0;
            if (videoArtQueue.Count == 0) videoArtQueue = null;
        }

        /// <summary>Fingerprints a few card images each frame (about 20 ms of work), then saves them all.</summary>
        private void UpdateArtIndex()
        {
            if (videoArtQueue == null) return;
            var watch = System.Diagnostics.Stopwatch.StartNew();
            while (videoArtPos < videoArtQueue.Count && watch.ElapsedMilliseconds < 20)
            {
                var item = videoArtQueue[videoArtPos++];
                var def = db.Get(item.Key);
                if (def == null) continue;
                Texture2D tex = null;
                try
                {
                    tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                    if (tex.LoadImage(File.ReadAllBytes(item.Value)))
                    {
                        var image = ToVideoImage(tex.GetPixels32(), tex.width, tex.height);
                        if (image.Height > 420) image = image.Resize(Mathf.Max(8, image.Width * 420 / image.Height), 420);
                        videoArt.Add(item.Key, def.Name, CardArtIndex.Fingerprint(image));
                    }
                }
                catch (Exception)
                {
                    // An unreadable image is skipped (the deck builder reports those).
                }
                finally
                {
                    if (tex != null) Destroy(tex);
                }
            }
            if (videoArtPos < videoArtQueue.Count) return;
            videoArtQueue = null;
            try
            {
                Directory.CreateDirectory(VideoFolder);
                File.WriteAllText(Path.Combine(VideoFolder, ArtCacheFileName), videoArt.Serialize(videoArtStamp), new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                Debug.LogWarning("[PokemonTCG] Couldn't save the card art fingerprints: " + e.Message);
            }
        }

        private string ArtIndexStatus()
        {
            if (videoArt == null) return CardVisuals.ImageFiles().Count == 0 ? "no card images (see Card images in the README)" : "not loaded yet";
            if (videoArtQueue != null) return "learning the card images: " + videoArtPos + " of " + videoArtQueue.Count;
            return videoArt.Count + " card images ready";
        }

        // ------------------------------------------------------------------ Layouts

        private void LoadVideoLayouts()
        {
            videoLayouts.Clear();
            videoLayouts.AddRange(VideoLayout.BuiltIns());
            try
            {
                if (Directory.Exists(LayoutFolder))
                    foreach (var file in Directory.GetFiles(LayoutFolder, "*.txt").OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                    {
                        var layout = VideoLayout.Parse(File.ReadAllText(file, Encoding.UTF8));
                        if (layout.Boxes.Count == 0 && string.IsNullOrEmpty(layout.Name)) continue;
                        videoLayouts.RemoveAll(l => !l.BuiltIn && l.Name == layout.Name);
                        videoLayouts.Add(layout);
                    }
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't read the saved layouts: " + e.Message;
            }
            videoLayoutIndex = Mathf.Clamp(videoLayoutIndex, 0, videoLayouts.Count - 1);
        }

        private VideoLayout FindVideoLayout(string name)
        {
            if (videoLayouts.Count == 0) LoadVideoLayouts();
            return videoLayouts.FirstOrDefault(l => l.Name == name);
        }

        private void SaveVideoLayout(VideoLayout layout)
        {
            if (layout == null || layout.BuiltIn) return;
            try
            {
                Directory.CreateDirectory(LayoutFolder);
                File.WriteAllText(Path.Combine(LayoutFolder, DeckLibrary.SafeFileName(layout.Name) + ".txt"), layout.Serialize(), new UTF8Encoding(false));
                videoLayoutDirty = false;
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't save the layout: " + e.Message;
            }
        }

        /// <summary>Saves a copy of the layout under a new name (built-in layouts are changed this way).</summary>
        private void SaveVideoLayoutAs(string name)
        {
            name = (name ?? "").Trim();
            if (name.Length == 0)
            {
                videoMessage = "Type a name for the layout first.";
                return;
            }
            if (videoLayouts.Any(l => l.Name == name && l.BuiltIn))
            {
                videoMessage = "A built-in layout already has that name.";
                return;
            }
            var copy = VideoLayoutNow.Clone(name);
            copy.BuiltIn = false;
            videoLayouts.RemoveAll(l => !l.BuiltIn && l.Name == name);
            videoLayouts.Add(copy);
            videoLayoutIndex = videoLayouts.IndexOf(copy);
            SaveVideoLayout(copy);
            videoMessage = "Saved the layout \"" + name + "\".";
        }

        private void DeleteVideoLayout()
        {
            var layout = VideoLayoutNow;
            if (layout.BuiltIn) return;
            try
            {
                string file = Path.Combine(LayoutFolder, DeckLibrary.SafeFileName(layout.Name) + ".txt");
                if (File.Exists(file)) File.Delete(file);
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't delete the layout: " + e.Message;
                return;
            }
            videoLayouts.Remove(layout);
            videoLayoutIndex = 0;
        }

        // ------------------------------------------------------------------ Sessions

        private void LoadVideoSessionFor(string path)
        {
            SaveVideoSession();
            videoSessionPath = Path.Combine(VideoFolder, DeckLibrary.SafeFileName(Path.GetFileNameWithoutExtension(path)) + ".video.txt");
            videoSession = null;
            try
            {
                if (File.Exists(videoSessionPath))
                {
                    var saved = VideoSession.Parse(File.ReadAllText(videoSessionPath, Encoding.UTF8));
                    if (string.Equals(Path.GetFileName(saved.VideoPath), Path.GetFileName(path), StringComparison.OrdinalIgnoreCase)) videoSession = saved;
                }
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't read what was saved for this video (" + e.Message + "); starting over.";
            }
            if (videoSession == null) videoSession = new VideoSession { LayoutName = VideoLayoutNow.Name };
            videoSession.VideoPath = path;
            var layout = FindVideoLayout(videoSession.LayoutName);
            if (layout != null) videoLayoutIndex = videoLayouts.IndexOf(layout);
            videoUnknown = videoSession.Readings.Count > 0 ? videoSession.UnknownNames(VideoLayoutNow, videoNames) : new List<KeyValuePair<string, int>>();
            videoStudyResult = "";
        }

        private void SaveVideoSession()
        {
            if (videoSession == null || string.IsNullOrEmpty(videoSessionPath)) return;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(videoSessionPath));
                File.WriteAllText(videoSessionPath, videoSession.Serialize(), new UTF8Encoding(false));
            }
            catch (Exception e)
            {
                videoMessage = "Couldn't save the video's events: " + e.Message;
            }
        }

        // ------------------------------------------------------------------ Studying

        private void StudyVideo()
        {
            var session = videoSession;
            var b = brain;
            var cards = db;
            if (session == null || b == null || cards == null || videoStudyRunning) return;
            string log = session.BuildLog();
            string title = session.Title;
            string logName = DeckLibrary.SafeFileName("Video " + title) + ".txt";
            string folder = GameLogFolder;
            videoStudyRunning = true;
            videoStudyResult = "Studying...";
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    var scripts = StandardCards.CreateRegistry();
                    var report = LogStudy.Study(log, title, cards, scripts, b);
                    b.Save();
                    try
                    {
                        // Kept with the game logs (marked studied, so "Study new logs" doesn't count it again).
                        Directory.CreateDirectory(folder);
                        File.WriteAllText(Path.Combine(folder, logName), log, new UTF8Encoding(false));
                        MarkStudied(new[] { logName });
                    }
                    catch (Exception)
                    {
                        // The study counts even if the copy couldn't be saved.
                    }
                    videoStudyResult = report.Summary();
                    AddStudyRow(logName + ": " + report.Summary());
                    if (string.IsNullOrEmpty(report.Error)) session.Studied = true;
                }
                catch (Exception e)
                {
                    videoStudyResult = "Couldn't study it: " + e.Message;
                }
                finally
                {
                    videoStudyRunning = false;
                }
            });
        }
    }
}
