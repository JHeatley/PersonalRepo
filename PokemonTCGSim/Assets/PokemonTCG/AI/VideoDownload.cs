using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace PokemonTCG.AI
{
    /// <summary>
    /// A YouTube link on the video screen: yt-dlp (a free command-line downloader, kept in the Videos folder) saves the video
    /// into the Videos folder, and it then opens like any video file. It takes the H.264 MP4 picture only, up to 1080p: what
    /// Unity's VideoPlayer plays, what the layouts are measured on, and no sound (the reading doesn't use it), so no other
    /// tool is needed to join sound and picture. A video already downloaded is opened again without downloading.
    /// </summary>
    public sealed class VideoDownload
    {
        public const string ToolFileName = "yt-dlp.exe";
        /// <summary>The latest yt-dlp for Windows, from its GitHub releases.</summary>
        public const string ToolUrl = "https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp.exe";

        private static readonly Regex IdPattern = new Regex(
            @"(?:youtube\.com/(?:watch\?(?:[^#\s]*&)?v=|live/|shorts/|embed/|v/)|youtu\.be/)([A-Za-z0-9_-]{11})(?![A-Za-z0-9_-])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        private static readonly Regex ProgressPattern = new Regex(@"^\[download\]\s+(\d+(?:\.\d+)?)%", RegexOptions.CultureInvariant);

        /// <summary>The video's 11-character id in a YouTube link (watch, youtu.be, live, shorts, embed), or null.</summary>
        public static string YouTubeId(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            var m = IdPattern.Match(text.Trim());
            return m.Success ? m.Groups[1].Value : null;
        }

        public static bool IsYouTubeLink(string text)
        {
            return YouTubeId(text) != null;
        }

        /// <summary>yt-dlp's command line for the video with this id, saved in folder as "title [id].mp4".</summary>
        public static string Arguments(string id, string folder)
        {
            string output = Path.Combine(folder, "%(title).80B [%(id)s].%(ext)s");
            return "--no-playlist --match-filter \"!is_live\" --newline --progress --encoding utf-8 --windows-filenames"
                   + " -f \"bv*[vcodec^=avc1][height<=1080][ext=mp4]/b[vcodec^=avc1][height<=1080][ext=mp4]\""
                   + " --print after_move:filepath -o \"" + output + "\" \"https://www.youtube.com/watch?v=" + id + "\"";
        }

        /// <summary>The percentage on one of yt-dlp's progress lines ("[download]  42.0% of ..."), or -1.</summary>
        public static double ParseProgress(string line)
        {
            if (string.IsNullOrEmpty(line)) return -1;
            var m = ProgressPattern.Match(line.Trim());
            double p;
            return m.Success && double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out p) ? p : -1;
        }

        /// <summary>The video with this id already in folder ("... [id].mp4"), or null.</summary>
        public static string FindDownloaded(string folder, string id)
        {
            if (string.IsNullOrEmpty(id) || !Directory.Exists(folder)) return null;
            return Directory.GetFiles(folder, "*[" + id + "].mp4").OrderByDescending(File.GetLastWriteTimeUtc).FirstOrDefault();
        }

        /// <summary>yt-dlp.exe in the tools folder, or null.</summary>
        public static string FindTool(string toolsFolder)
        {
            string path = Path.Combine(toolsFolder, ToolFileName);
            return File.Exists(path) ? path : null;
        }

        /// <summary>yt-dlp's error, in words for the screen.</summary>
        public static string Explain(string error)
        {
            if (string.IsNullOrEmpty(error)) return "yt-dlp stopped without saying why.";
            if (error.Contains("Requested format is not available"))
                return "YouTube has no H.264 MP4 version of this video up to 1080p, which is what Unity can play.";
            if (error.Contains("Sign in to confirm")) return "YouTube wants a signed-in account for this video, so it can't be downloaded here.";
            if (error.Contains("Private video") || error.Contains("Video unavailable")) return "The video is private or unavailable.";
            if (error.Contains("is_live")) return "The video is a live stream that's still going: try again once it has ended.";
            return error.Trim();
        }

        // ------------------------------------------------------------------ One download

        public readonly string Id;
        public readonly string Folder;
        private readonly object gate = new object();
        private Process process;
        private volatile bool cancelled;
        private string status = "Starting the download...";
        private string filePath, error, lastError;
        private double percent = -1;
        private bool done;

        public VideoDownload(string id, string folder)
        {
            Id = id;
            Folder = folder;
        }

        public string Status { get { lock (gate) return status; } }
        public double Percent { get { lock (gate) return percent; } }
        public bool Done { get { lock (gate) return done; } }
        /// <summary>The saved video once Done, or null (then Error says why, or it was cancelled).</summary>
        public string FilePath { get { lock (gate) return filePath; } }
        public string Error { get { lock (gate) return error; } }
        public bool Cancelled { get { return cancelled; } }

        /// <summary>Runs yt-dlp on its own thread.</summary>
        public void Start(string tool)
        {
            var thread = new Thread(() => Run(tool)) { IsBackground = true, Name = "YouTube download" };
            thread.Start();
        }

        public void Cancel()
        {
            cancelled = true;
            try
            {
                var p = process;
                if (p != null && !p.HasExited) p.Kill();
            }
            catch (Exception) { }
        }

        private void Run(string tool)
        {
            try
            {
                Directory.CreateDirectory(Folder);
                var info = new ProcessStartInfo
                {
                    FileName = tool,
                    Arguments = Arguments(Id, Folder),
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8
                };
                using (var p = Process.Start(info))
                {
                    if (p == null) throw new InvalidOperationException("yt-dlp couldn't be started.");
                    process = p;
                    p.ErrorDataReceived += (sender, args) => OnLine(args.Data, true);
                    p.BeginErrorReadLine();
                    string line;
                    while ((line = p.StandardOutput.ReadLine()) != null) OnLine(line, false);
                    p.WaitForExit();
                    process = null;
                    lock (gate)
                    {
                        if (filePath == null || !File.Exists(filePath)) filePath = FindDownloaded(Folder, Id);
                        if (cancelled) filePath = null;
                        else if (filePath == null) error = Explain(lastError);
                        done = true;
                    }
                }
            }
            catch (Exception e)
            {
                lock (gate)
                {
                    error = cancelled ? null : "Couldn't run yt-dlp: " + e.Message;
                    done = true;
                }
            }
        }

        private void OnLine(string line, bool stderr)
        {
            if (string.IsNullOrEmpty(line)) return;
            lock (gate)
            {
                double p = ParseProgress(line);
                if (p >= 0)
                {
                    percent = p;
                    status = "Downloading from YouTube: " + p.ToString("0", CultureInfo.InvariantCulture) + "%";
                    return;
                }
                string trimmed = line.Trim();
                if (stderr && trimmed.StartsWith("ERROR", StringComparison.Ordinal)) lastError = trimmed.Substring(Math.Min(trimmed.Length, 6)).Trim();
                else if (!stderr && trimmed.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase)) filePath = trimmed;
            }
        }
    }
}
