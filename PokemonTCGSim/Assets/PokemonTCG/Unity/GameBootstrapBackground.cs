using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PokemonTCG.AI;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// "Train in the background": the Train screen's session runs in the command-line trainer (Tools/sim-train/sim-train.exe,
    /// built from Headless) instead of inside the app. It's several times faster (the .NET runtime uses the CPU's vector
    /// instructions; Unity's doesn't) and keeps the app light; with "Learn on the GPU" its learning goes to the graphics card too.
    /// The app writes a job folder (the settings, the decklists and the AI's brain file), shows the trainer's progress, asks it
    /// to stop when you press Stop, and reloads the brain when it's done: the network, the matchups, how well it knows each
    /// deck, the card results and the session's record (Growth chart). Meanwhile the app doesn't save the brain (it would
    /// write over the trainer's), so what the AI learns from games you play then isn't kept.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private System.Diagnostics.Process bgProcess;
        private int bgPid = -1;
        private Dictionary<string, string> bgProgress = new Dictionary<string, string>();
        private float nextBgRead;
        private string bgMessage;
        private bool bgChecked;
        /// <summary>v170: when Stop was asked (UTC), or null; after ForceStopAfter the Train screen offers Force stop.</summary>
        private DateTime? bgStopAsked;
        /// <summary>v170: the trainer was ended with Force stop (it saved nothing).</summary>
        private bool bgForced;
        private static readonly TimeSpan ForceStopAfter = TimeSpan.FromSeconds(60);

        private static string BackgroundTrainerPath
        {
            get { return Path.GetFullPath(Path.Combine(Application.dataPath, "..", "Tools", "sim-train", "sim-train.exe")); }
        }

        private static bool BackgroundTrainerAvailable
        {
            get
            {
                return (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                       && File.Exists(BackgroundTrainerPath);
            }
        }

        // Read on the main thread (SavePaths uses PlayerPrefs).
        private static string BackgroundJobFolder { get { return Path.Combine(SavePaths.BrainFolder, "Background training"); } }

        private bool BackgroundRunning
        {
            get
            {
                if (bgPid < 0) return false;
                try
                {
                    if (bgProcess == null) bgProcess = System.Diagnostics.Process.GetProcessById(bgPid);
                    return !bgProcess.HasExited;
                }
                catch (Exception)
                {
                    return false; // no such process any more
                }
            }
        }

        private string BgValue(string key, string fallback = "")
        {
            string v;
            return bgProgress.TryGetValue(key, out v) ? v : fallback;
        }

        private int BgInt(string key)
        {
            int v;
            return int.TryParse(BgValue(key), NumberStyles.Integer, CultureInfo.InvariantCulture, out v) ? v : 0;
        }

        private double BgDouble(string key, double fallback)
        {
            double v;
            return double.TryParse(BgValue(key), NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : fallback;
        }

        private void ReadBackgroundProgress()
        {
            var path = Path.Combine(BackgroundJobFolder, "progress.txt");
            if (!File.Exists(path)) return;
            try
            {
                var map = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var line in File.ReadAllLines(path))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0) map[line.Substring(0, eq)] = line.Substring(eq + 1);
                }
                bgProgress = map;
            }
            catch (IOException)
            {
                // The trainer was writing it: next time.
            }
        }

        /// <summary>
        /// Called from Update: follows the background trainer, and reloads the brain when it's done. The first time, it also picks
        /// up a trainer still running from before the app was last closed.
        /// </summary>
        private void UpdateBackgroundTraining()
        {
            if (!bgChecked && brain != null)
            {
                bgChecked = true;
                ReadBackgroundProgress();
                int pid = BgInt("pid");
                if (pid > 0 && BgValue("finished") != "1")
                {
                    bgPid = pid;
                    if (BackgroundRunning)
                    {
                        brain.SavesPaused = true;
                        bgMessage = "Training in the background (started before the app was last closed).";
                        var stopFile = Path.Combine(BackgroundJobFolder, "stop");
                        if (File.Exists(stopFile))
                        {
                            bgStopAsked = File.GetLastWriteTimeUtc(stopFile);
                            bgMessage = "Stopping (asked before the app was last closed): the trainer finishes its games and saves...";
                        }
                    }
                    else bgPid = -1;
                }
            }
            if (bgPid < 0 || Time.unscaledTime < nextBgRead) return;
            nextBgRead = Time.unscaledTime + 1f;
            ReadBackgroundProgress();
            if (BackgroundRunning) return;
            // Done (or stopped, or failed): take the brain the trainer saved.
            bgPid = -1;
            bgProcess = null;
            bgStopAsked = null;
            string error = BgValue("lastError");
            int done = BgInt("done");
            int bgBlocksBefore = JobBlocks();
            LoadBrain();
            if (brain != null) brain.SavesPaused = false;
            // v171: a background run must never come back with a smaller network (an out-of-date trainer did that once).
            string shrunk = brain != null && bgBlocksBefore > 0 && brain.Policy.Blocks < bgBlocksBefore
                ? " Warning: the network came back smaller (" + brain.Policy.Blocks + " blocks, it had " + bgBlocksBefore + "). The background "
                  + "trainer is probably out of date: copy Tools/sim-train from the project as well as the scripts."
                : "";
            if (bgForced)
                bgMessage = "Background training was force-stopped, so it saved nothing from this session: the AI is as it was before it started.";
            else
                bgMessage = (BgValue("cancelled") == "1" ? "Background training stopped" : "Background training finished") + " after " + done.ToString("N0") + " games"
                            + (string.IsNullOrEmpty(error) ? "" : " (" + error + ")") + ". What it learned is loaded: see below." + shrunk;
            bgForced = false;
            nextBrainRefresh = 0f;
        }

        /// <summary>The network's size when the background run started, from its job.txt (0 if it doesn't say).</summary>
        private static int JobBlocks()
        {
            try
            {
                foreach (var line in File.ReadAllLines(Path.Combine(BackgroundJobFolder, "job.txt")))
                {
                    int v;
                    if (line.StartsWith("blocks=", StringComparison.Ordinal)
                        && int.TryParse(line.Substring(7).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v)) return v;
                }
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
            return 0;
        }

        /// <summary>
        /// Asks the background trainer its version ("sim-train version"); null if it's at least TrainerVersion.Number, otherwise
        /// what's wrong. Trainers from before v171 don't know the command at all, which means out of date too.
        /// </summary>
        private static string CheckTrainerVersion()
        {
            string update = " Copy the Tools/sim-train folder from the project too (not only the scripts), so the background trainer matches the app.";
            try
            {
                var info = new System.Diagnostics.ProcessStartInfo(BackgroundTrainerPath, "version")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    WorkingDirectory = Path.GetDirectoryName(BackgroundTrainerPath)
                };
                using (var p = System.Diagnostics.Process.Start(info))
                {
                    if (p == null) return "Couldn't start the background trainer to check its version.";
                    var read = p.StandardOutput.ReadToEndAsync();
                    if (!p.WaitForExit(20000))
                    {
                        try { p.Kill(); } catch (InvalidOperationException) { }
                        return "The background trainer didn't answer when asked its version (20 seconds).";
                    }
                    string text = read.Wait(2000) ? read.Result ?? "" : "";
                    int found = -1;
                    foreach (var line in text.Split('\n'))
                    {
                        var t = line.Trim();
                        int v;
                        if (t.StartsWith("sim-train ", StringComparison.Ordinal) && int.TryParse(t.Substring(10).Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out v))
                            found = v;
                    }
                    if (found < 0) return "The background trainer (" + BackgroundTrainerPath + ") is older than the app, and an old one can lose what the AI learned (it can't read a bigger network)." + update;
                    if (found < TrainerVersion.Number)
                        return "The background trainer is version " + found + " but the app needs " + TrainerVersion.Number + " or later." + update;
                    return null;
                }
            }
            catch (Exception e)
            {
                return "Couldn't check the background trainer's version: " + e.Message;
            }
        }

        /// <summary>Starts the Train screen's session in the background trainer.</summary>
        private void StartBackgroundTraining()
        {
            if (BackgroundRunning || (training != null && training.Running) || brain == null || !TrainDecksReady) return;
            if (!BackgroundTrainerAvailable)
            {
                bgMessage = "The background trainer isn't there: " + BackgroundTrainerPath + " (Windows only).";
                return;
            }
            // v171: an older trainer may not read the AI's file as it is now and would save its own idea of it over the file.
            string versionProblem = CheckTrainerVersion();
            if (versionProblem != null)
            {
                bgMessage = versionProblem;
                return;
            }
            if (brain.PolicyLoadError != null)
            {
                bgMessage = "Not training in the background: the network's file couldn't be read (see above).";
                return;
            }
            try
            {
                string dir = BackgroundJobFolder;
                Directory.CreateDirectory(Path.Combine(dir, "decks"));
                foreach (var f in Directory.GetFiles(Path.Combine(dir, "decks"))) File.Delete(f);
                foreach (var name in new[] { "progress.txt", "stop" })
                    if (File.Exists(Path.Combine(dir, name))) File.Delete(Path.Combine(dir, name));

                // The decks: the pool for a random side, and the decks picked on the other sides.
                int focus = TrainFocusDeck;
                var used = new List<int>();
                var pool = trainRandomA || trainRandomB ? (focus >= 0 ? TrainPoolFor(focus) : AllTrainDecks()) : new List<int>();
                used.AddRange(pool);
                // The Neural AI's checks against its champion play every legal deck, whatever is trained.
                var champion = AllTrainDecks();
                used.AddRange(champion);
                if (!trainRandomA) used.Add(trainA);
                if (!trainRandomB) used.Add(trainB);
                var sb = new StringBuilder();
                foreach (int i in used.Distinct())
                {
                    File.WriteAllText(Path.Combine(dir, "decks", i + ".txt"), decks[i].Text ?? "", new UTF8Encoding(false));
                    sb.Append("deck.").Append(i).Append('=').Append(decks[i].Name.Replace('\n', ' ')).Append('\n');
                }
                int fixedDeck = !trainRandomA ? trainA : !trainRandomB ? trainB : -1;
                var format = (fixedDeck >= 0 ? decks[fixedDeck].ResolvedFormat : null) ?? GameFormat.Standard;
                sb.Append("brain=").Append(BrainPath).Append('\n');
                sb.Append("games=").Append(Mathf.Clamp(trainGames, 1, MaxTrainGames)).Append('\n');
                sb.Append("measure=").Append(trainMeasure ? MeasureGames : 0).Append('\n');
                sb.Append("style=").Append(trainStyle == AiStyle.Learning ? "learning" : trainStyle == AiStyle.Hybrid ? "hybrid" : "neural").Append('\n');
                sb.Append("pool=").Append(string.Join(",", pool.Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray())).Append('\n');
                sb.Append("champion=").Append(string.Join(",", champion.Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray())).Append('\n');
                sb.Append("a=").Append(trainRandomA ? -1 : trainA).Append('\n');
                sb.Append("b=").Append(trainRandomB ? -1 : trainB).Append('\n');
                sb.Append("randomA=").Append(trainRandomA ? 1 : 0).Append('\n');
                sb.Append("randomB=").Append(trainRandomB ? 1 : 0).Append('\n');
                sb.Append("threads=").Append(trainFast ? FastThreads : Math.Max(1, Environment.ProcessorCount / 2)).Append('\n');
                sb.Append("rest=").Append(trainFast ? "0" : "0.25").Append('\n');
                sb.Append("rate=").Append(TrainRate.ToString(CultureInfo.InvariantCulture)).Append('\n');
                if (trainGpu) sb.Append("gpu=").Append(trainGpuAddress).Append('\n');
                if (trainGpu && trainGpuMoves) sb.Append("gpumoves=1\n");
                sb.Append("lookahead=").Append(TrainLookAheadShares[trainLookAhead].ToString(CultureInfo.InvariantCulture)).Append('\n');
                sb.Append("exploiters=").Append(trainExploiters ? 1 : 0).Append('\n');
                sb.Append("prizes=").Append(format.PrizeCount).Append('\n');
                sb.Append("seed=").Append((Environment.TickCount & 0x0FFFFFFF) | 1).Append('\n');
                sb.Append("blocks=").Append(brain.Policy.Blocks).Append('\n'); // the network's size now (checked when it comes back)
                File.WriteAllText(Path.Combine(dir, "job.txt"), sb.ToString(), new UTF8Encoding(false));

                // The trainer starts from the brain as it is now; the app stops saving it until the trainer is done.
                brain.SavesPaused = false;
                brain.Save();
                brain.SavesPaused = true;

                string args = "background --job \"" + dir + "\" --root \"" + Path.GetFullPath(Path.Combine(Application.dataPath, "..")) + "\"";
                if (skipCpu && CpuThreads.Parse(skipCpuText).Count > 0) args += " --skip-cpu " + string.Join(",", CpuThreads.Parse(skipCpuText).Select(n => n.ToString(CultureInfo.InvariantCulture)).ToArray());
                var info = new System.Diagnostics.ProcessStartInfo(BackgroundTrainerPath, args)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(BackgroundTrainerPath)
                };
                bgProcess = System.Diagnostics.Process.Start(info);
                if (bgProcess == null) throw new IOException("Windows didn't start it");
                bgPid = bgProcess.Id;
                bgProgress = new Dictionary<string, string>();
                nextBgRead = 0f;
                bgMessage = null;
                bgStopAsked = null;
                bgForced = false;
                trainMessage = null;
            }
            catch (Exception e)
            {
                if (brain != null) brain.SavesPaused = false;
                bgPid = -1;
                bgProcess = null;
                bgMessage = "Couldn't start the background trainer: " + e.Message;
            }
        }

        /// <summary>Asks the background trainer to stop (it finishes its games, saves, and the app reloads the brain).</summary>
        private void StopBackgroundTraining()
        {
            try
            {
                File.WriteAllText(Path.Combine(BackgroundJobFolder, "stop"), "stop");
                if (bgStopAsked == null) bgStopAsked = DateTime.UtcNow;
                bgMessage = "Stopping: the trainer finishes its games and saves...";
            }
            catch (Exception e)
            {
                bgMessage = "Couldn't ask the trainer to stop: " + e.Message;
            }
        }

        /// <summary>
        /// v170: Force stop. Ends the trainer program at once, for one that doesn't stop (an older sim-train.exe waiting on a GPU learner
        /// that never answers). It saves nothing from the session, so the app reloads the AI as it was when the training started.
        /// </summary>
        private void ForceStopBackgroundTraining()
        {
            try
            {
                if (bgProcess == null && bgPid > 0) bgProcess = System.Diagnostics.Process.GetProcessById(bgPid);
                if (bgProcess != null && !bgProcess.HasExited) bgProcess.Kill();
                bgForced = true;
                File.Delete(Path.Combine(BackgroundJobFolder, "stop"));
                bgMessage = "Force-stopping the trainer...";
            }
            catch (Exception e)
            {
                bgMessage = "Couldn't end the trainer (end sim-train.exe in Task Manager): " + e.Message;
            }
            nextBgRead = 0f;
        }

        /// <summary>The Train screen's background-training row: the button, or the progress and Stop while it runs.</summary>
        private void DrawBackgroundTraining(UiStyles ui, List<Action> actions, bool runningHere)
        {
            bool bg = BackgroundRunning;
            GUILayout.BeginHorizontal();
            GUI.enabled = !runningHere && !bg && TrainDecksReady && brain != null && BackgroundTrainerAvailable;
            if (GUILayout.Button("Train in the background (fastest)", GUILayout.Height(28))) actions.Add(StartBackgroundTraining);
            GUI.enabled = bg;
            if (GUILayout.Button("Stop it", GUILayout.Height(28), GUILayout.Width(80))) actions.Add(StopBackgroundTraining);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (bg && bgStopAsked != null && DateTime.UtcNow - bgStopAsked.Value > ForceStopAfter)
            {
                GUILayout.Label("It hasn't stopped after " + FormatDuration((DateTime.UtcNow - bgStopAsked.Value).TotalSeconds) + ". Force stop ends the trainer now; "
                                + "it saves nothing from this session (the AI stays as it was before it started).", ui.Error);
                if (GUILayout.Button("Force stop", GUILayout.Height(28))) actions.Add(ForceStopBackgroundTraining);
            }
            if (bg)
            {
                int done = BgInt("done"), games = BgInt("games");
                double rate = BgDouble("gamesPerSecond", 0), seconds = BgDouble("seconds", 0);
                string left = rate > 0 && games > done ? "  ·  about " + FormatDuration((games - done) / rate) + " left" : "";
                GUILayout.Label("In the background: " + BgValue("phase", "starting..."), ui.Label);
                GUILayout.Label("Games " + done.ToString("N0") + " / " + games.ToString("N0") + (rate > 0 ? "  ·  " + rate.ToString("0.0") + " games/s" : "")
                                + left + (BgInt("evalTotal") > 0 ? "  ·  measuring games " + BgInt("evalDone") + " / " + BgInt("evalTotal") : "")
                                + "  ·  running " + FormatDuration(seconds), Hint(ui));
                double before = BgDouble("scoreBefore", -1);
                if (before >= 0) GUILayout.Label("Before training it won " + Math.Round(before * 100) + "% against the classic bot.", Hint(ui));
                if (!string.IsNullOrEmpty(BgValue("lastError"))) GUILayout.Label(BgValue("lastError"), ui.Error);
                GUILayout.Label("The app doesn't save the AI meanwhile: when the trainer finishes (or you stop it), it loads what it learned. "
                                + "Games you play against the AI until then don't teach it.", Hint(ui));
            }
            else if (!BackgroundTrainerAvailable)
                GUILayout.Label("Background training needs Windows and Tools/sim-train/sim-train.exe next to the Assets folder.", Hint(ui));
            else
                GUILayout.Label("Runs this session (the decks, games, style and settings above) in a separate trainer program: several times faster "
                                + "than training in the app, and the app stays light. When it's done, the app loads what it learned.", Hint(ui));
            if (!string.IsNullOrEmpty(bgMessage)) GUILayout.Label(bgMessage, ui.Good);
        }

        private static string FormatDuration(double seconds)
        {
            if (seconds < 60) return Math.Max(0, Math.Round(seconds)) + " s";
            if (seconds < 3600) return Math.Round(seconds / 60) + " min";
            if (seconds < 48 * 3600) return (seconds / 3600).ToString("0.0", CultureInfo.InvariantCulture) + " h";
            return (seconds / 86400).ToString("0.0", CultureInfo.InvariantCulture) + " days";
        }
    }
}
