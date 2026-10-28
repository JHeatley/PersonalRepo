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

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Game logs (v76): every game played here is saved to the Game logs folder (a GameRecord: both decks, the seed and every
    /// answer, plus the battle log), and the Train screen's "Study game logs" panel has the AI learn from logs: a TCG Live
    /// battle log you paste, or every log in the Game logs folder it hasn't studied yet (games from this app and TCG Live
    /// battle logs saved there as .txt files). See LogStudy.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string StudiedListName = SavePaths.StudiedLogsFileName;
        private const int MaxStudyRows = 200;

        private GameRecord gameRecord;
        /// <summary>The AI already learned from the game being recorded (your game against it, a Regional Sim table), so it's saved as studied.</summary>
        private bool gameRecordLearned;

        private volatile bool studyRunning;
        private readonly object studyGate = new object();
        private readonly List<string> studyResults = new List<string>();
        private string studyProgress = "";
        // What the panel draws, copied once a frame in Update (IMGUI draws each frame in several passes).
        private List<string> studyRowsView = new List<string>();
        private string studyProgressView = "";
        private bool studyRunningView;
        private int newLogsView = -1;
        private float nextLogScan;
        private string pasteLog = "";
        private Vector2 pasteScroll, studyScroll;

        private static string GameLogFolder { get { return SavePaths.GameLogFolder; } }
        private static string StudiedListPath { get { return Path.Combine(GameLogFolder, StudiedListName); } }

        // ------------------------------------------------------------------ Recording

        /// <summary>Starts a record for a new game and wraps both seats so their answers go into it.</summary>
        private void BeginGameRecord(ref IPlayerAgent a0, ref IPlayerAgent a1, IList<CardDef> d0, IList<CardDef> d1, bool learnedLive)
        {
            gameRecord = new GameRecord { When = DateTime.Now.ToString("yyyy-MM-dd HH:mm") };
            gameRecord.Decks[0].AddRange(d0);
            gameRecord.Decks[1].AddRange(d1);
            gameRecordLearned = learnedLive;
            a0 = GameRecord.Wrap(a0, gameRecord, 0);
            a1 = GameRecord.Wrap(a1, gameRecord, 1);
        }

        /// <summary>Copies the engine settings that change how the game plays into the record (call once the engine is set up).</summary>
        private void FillGameRecord(GameEngine engine, int seed, string deck0, string deck1)
        {
            var r = gameRecord;
            if (r == null) return;
            r.Seed = seed;
            r.Today = engine.Today;
            r.PrizeCount = engine.PrizeCount;
            r.Rules = engine.Rules;
            r.Names = new[] { engine.PlayerNames[0], engine.PlayerNames[1] };
            r.DeckNames = new[] { deck0 ?? "", deck1 ?? "" };
            r.Birthdays = new[] { engine.Birthdays[0], engine.Birthdays[1] };
            r.OrderChooser = engine.OrderChooser;
            r.OrderChooserIsHigherSeed = engine.OrderChooserIsHigherSeed;
            r.Interactive = new[] { engine.IsInteractive(0), engine.IsInteractive(1) };
            r.TieBreaker = engine.TieBreaker;
        }

        /// <summary>Saves the finished game to the Game logs folder.</summary>
        private void SaveGameRecord()
        {
            var r = gameRecord;
            gameRecord = null;
            if (r == null || host == null || host.Engine == null || host.Engine.State == null || !host.Engine.State.IsOver) return;
            var state = host.Engine.State;
            r.Winner = state.Winner;
            r.Turns = state.TurnNumber;
            r.Reason = state.EndReason ?? "";
            r.LogLines.AddRange(log);
            string text = r.Serialize();
            string name = DeckLibrary.SafeFileName(DateTime.Now.ToString("yyyy-MM-dd HHmmss") + " " + r.DeckNames[0] + " vs " + r.DeckNames[1]) + ".txt";
            bool learned = gameRecordLearned;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    Directory.CreateDirectory(GameLogFolder);
                    File.WriteAllText(Path.Combine(GameLogFolder, name), text, new UTF8Encoding(false));
                    // The AI learned from this game as it ended; don't study it twice.
                    if (learned) MarkStudied(new[] { name });
                }
                catch (Exception e)
                {
                    Debug.LogWarning("[PokemonTCG] Couldn't save the game log: " + e.Message);
                }
            });
        }

        // ------------------------------------------------------------------ Studying

        private static readonly object studiedFileLock = new object();

        private static HashSet<string> StudiedNames()
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            lock (studiedFileLock)
            {
                try
                {
                    if (File.Exists(StudiedListPath))
                        foreach (var line in File.ReadAllLines(StudiedListPath))
                            if (line.Trim().Length > 0) set.Add(line.Trim());
                }
                catch (IOException)
                {
                }
            }
            return set;
        }

        private static void MarkStudied(IEnumerable<string> names)
        {
            lock (studiedFileLock)
            {
                Directory.CreateDirectory(GameLogFolder);
                File.AppendAllText(StudiedListPath, string.Join("\n", names.ToArray()) + "\n", new UTF8Encoding(false));
            }
        }

        /// <summary>Log files in the Game logs folder the AI hasn't studied yet.</summary>
        private static List<string> NewLogFiles()
        {
            var list = new List<string>();
            if (!Directory.Exists(GameLogFolder)) return list;
            var studied = StudiedNames();
            foreach (var path in Directory.GetFiles(GameLogFolder, "*.txt").OrderBy(p => p, StringComparer.OrdinalIgnoreCase))
            {
                string file = Path.GetFileName(path);
                if (string.Equals(file, StudiedListName, StringComparison.OrdinalIgnoreCase) || studied.Contains(file)) continue;
                list.Add(path);
            }
            return list;
        }

        /// <summary>Called from Update: copies what the panel shows, and now and then counts the logs not studied yet.</summary>
        private void UpdateLogStudy()
        {
            lock (studyGate)
            {
                studyRowsView = new List<string>(studyResults);
                studyProgressView = studyProgress;
            }
            studyRunningView = studyRunning;
            if (mode == Mode.Train && Time.unscaledTime >= nextLogScan)
            {
                nextLogScan = Time.unscaledTime + 2f;
                try { newLogsView = NewLogFiles().Count; }
                catch (Exception) { newLogsView = -1; }
            }
        }

        private void StudyPastedLog()
        {
            string text = pasteLog ?? "";
            if (text.Trim().Length == 0 || studyRunning || brain == null) return;
            // Kept in the Game logs folder too, so it's there for later (and marked as studied).
            string name = DeckLibrary.SafeFileName("TCG Live " + DateTime.Now.ToString("yyyy-MM-dd HHmmss")) + ".txt";
            StartStudy(new List<KeyValuePair<string, string>> { new KeyValuePair<string, string>(name, text) }, true);
            pasteLog = "";
        }

        private void StudyNewLogs()
        {
            if (studyRunning || brain == null) return;
            var items = new List<KeyValuePair<string, string>>();
            foreach (var path in NewLogFiles())
            {
                try { items.Add(new KeyValuePair<string, string>(Path.GetFileName(path), File.ReadAllText(path))); }
                catch (IOException e) { AddStudyRow(Path.GetFileName(path) + ": couldn't read it (" + e.Message + ")."); }
            }
            if (items.Count == 0)
            {
                AddStudyRow("No new logs in " + GameLogFolder + ".");
                return;
            }
            StartStudy(items, false);
        }

        private void AddStudyRow(string row)
        {
            lock (studyGate)
            {
                studyResults.Insert(0, row);
                if (studyResults.Count > MaxStudyRows) studyResults.RemoveAt(studyResults.Count - 1);
            }
        }

        /// <summary>Studies the logs on another thread; each studied file is added to studied.txt (a pasted log is saved first).</summary>
        private void StartStudy(List<KeyValuePair<string, string>> items, bool saveFirst)
        {
            var b = brain;
            var cards = db;
            if (b == null || cards == null) return;
            studyRunning = true;
            lock (studyGate) studyProgress = "Studying " + items.Count + (items.Count == 1 ? " log..." : " logs...");
            ThreadPool.QueueUserWorkItem(_ =>
            {
                int decisions = 0, done = 0;
                try
                {
                    var scripts = StandardCards.CreateRegistry();
                    foreach (var item in items)
                    {
                        done++;
                        lock (studyGate) studyProgress = "Studying " + done + " of " + items.Count + ": " + item.Key;
                        LogStudyReport report;
                        try
                        {
                            report = LogStudy.Study(item.Value, item.Key, cards, scripts, b);
                        }
                        catch (Exception studyError)
                        {
                            report = new LogStudyReport { Source = item.Key, Error = "couldn't study it (" + studyError.GetType().Name + ": " + studyError.Message + ")." };
                        }
                        decisions += report.TotalStudied;
                        AddStudyRow(item.Key + ": " + report.Summary());
                        try
                        {
                            if (saveFirst)
                            {
                                Directory.CreateDirectory(GameLogFolder);
                                File.WriteAllText(Path.Combine(GameLogFolder, item.Key), item.Value, new UTF8Encoding(false));
                            }
                            MarkStudied(new[] { item.Key });
                        }
                        catch (Exception saveError)
                        {
                            AddStudyRow(item.Key + ": studied, but couldn't be noted as studied (" + saveError.Message + ").");
                        }
                    }
                    b.Save();
                    lock (studyGate) studyProgress = "Done: " + items.Count + (items.Count == 1 ? " log, " : " logs, ") + decisions + " decisions studied.";
                }
                catch (Exception e)
                {
                    lock (studyGate) studyProgress = "Studying stopped: " + e.Message;
                }
                finally
                {
                    studyRunning = false;
                    nextLogScan = 0f;
                }
            });
        }

        // ------------------------------------------------------------------ Train screen panel

        private void DrawLogStudyPanel(UiStyles ui, List<Action> actions, bool training)
        {
            GUILayout.Label("Study game logs", ui.Title);
            GUILayout.Label("Show the AI games to learn from: a Pokémon TCG Live battle log (paste it below, or save exported logs as .txt files in the "
                            + "Game logs folder), and the games played in this app (each one is saved there when it ends). It rebuilds every decision "
                            + "in the game with the options the player had and learns from it, the winner's plays fully and the loser's half. "
                            + "In a TCG Live log the log owner's hand is known, so all their plays count; the opponent's hidden hand means only "
                            + "their attacks, Abilities, retreats and turn ends do.", Hint(ui));
            bool busy = studyRunningView || training || brain == null;

            GUILayout.Label("Paste a TCG Live battle log:", ui.Label);
            pasteScroll = GUILayout.BeginScrollView(pasteScroll, GUILayout.Height(110));
            pasteLog = GUILayout.TextArea(pasteLog ?? "", GUILayout.ExpandHeight(true));
            GUILayout.EndScrollView();
            GUILayout.BeginHorizontal();
            GUI.enabled = !busy && !string.IsNullOrEmpty(pasteLog) && pasteLog.Trim().Length > 0;
            if (GUILayout.Button("Study this log", GUILayout.Width(160))) actions.Add(() => StudyPastedLog());
            GUI.enabled = !busy;
            if (GUILayout.Button("Paste from clipboard", GUILayout.Width(170))) actions.Add(() => pasteLog = GUIUtility.systemCopyBuffer ?? "");
            GUILayout.EndHorizontal();
            // Two rows of two: four in a row are wider than the Train screen's panel.
            GUILayout.BeginHorizontal();
            string newLabel = newLogsView > 0 ? "Study new logs in the folder (" + newLogsView + ")" : "Study new logs in the folder";
            GUI.enabled = !busy && newLogsView != 0;
            if (GUILayout.Button(newLabel, GUILayout.Width(250))) actions.Add(() => StudyNewLogs());
            GUI.enabled = true;
            if (GUILayout.Button("Open the Game logs folder", GUILayout.Width(200))) actions.Add(() => SavePaths.OpenInFileBrowser(GameLogFolder));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUI.enabled = !training;
            if (GUILayout.Button("Learn from a video...", GUILayout.Width(200))) actions.Add(() => OpenVideoScreen());
            GUI.enabled = true;
            GUILayout.Label("Stream footage with an overlay, TCG Live recordings or a camera over the table (v77).", Hint(ui));
            GUILayout.EndHorizontal();
            if (training) GUILayout.Label("Logs can be studied once training has finished.", Hint(ui));
            if (!string.IsNullOrEmpty(studyProgressView)) GUILayout.Label(studyProgressView, studyRunningView ? ui.Label : ui.Good);
            if (studyRowsView.Count > 0)
            {
                float listMax = Mathf.Clamp(UiScale.Height * 0.25f, 120f, 300f);
                var rows = studyRowsView;
                studyScroll = DrawScrollList(ui, studyScroll, rows.Count, listMax, k => "   " + rows[k]);
            }
            GUILayout.Label("Game logs folder: " + GameLogFolder, Hint(ui));
            GUILayout.Space(8);
        }
    }
}
