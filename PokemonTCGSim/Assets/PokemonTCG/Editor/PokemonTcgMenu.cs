using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace PokemonTCG.EditorTools
{
    /// <summary>Menu bar: Pokemon TCG/...</summary>
    public static class PokemonTcgMenu
    {
        [MenuItem("Pokemon TCG/Create Board Scene")]
        public static void CreateBoardScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            new GameObject("Pokemon TCG Game").AddComponent<GameBootstrap>();
            const string folder = "Assets/PokemonTCG/Scenes";
            if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets/PokemonTCG", "Scenes");
            EditorSceneManager.SaveScene(scene, folder + "/Board.unity");
            Debug.Log("[PokemonTCG] Created " + folder + "/Board.unity. Press Play to open the menu.");
        }

        /// <summary>v118.2: puts the UI size back to Auto (works in and out of Play mode).</summary>
        [MenuItem("Pokemon TCG/Reset UI Size")]
        public static void ResetUiSize()
        {
            UiScale.Setting = 0;
            Debug.Log("[PokemonTCG] UI size reset to Auto.");
        }

        [MenuItem("Pokemon TCG/Simulate 1,000 Games - Dragapult vs N's Zoroark (Heuristic Bots)")]
        public static void SimulateCrossMatch()
        {
            var db = UnityCardData.LoadAllCards();
            var a = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var b = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            Simulate("Dragapult (P1) vs N's Zoroark (P2), heuristic bots", a, b, 1000, (seat, s) => new HeuristicAgent(s));
        }

        /// <summary>
        /// Trains the Neural AI (the same brain file the game uses) on the built-in Dragapult and N's Zoroark decks,
        /// measuring it against the classic bot before and after. The Train screen in the game does the same for any decks.
        /// </summary>
        [MenuItem("Pokemon TCG/Train Neural AI - 1,000 Games (Dragapult vs N's Zoroark)")]
        public static void TrainNeuralAi()
        {
            var decks = LoadBuiltInDecks();
            if (decks.Count < 2) return;
            var a = decks.FirstOrDefault(d => d.Key.Contains("Dragapult"));
            var b = decks.FirstOrDefault(d => d.Key.Contains("Zoroark"));
            if (a.Value == null) a = decks[0];
            if (b.Value == null || b.Key == a.Key) b = decks.First(d => d.Key != a.Key);
            PokemonTCG.Presentation.UnityCardData.LoadPolicyStart();
            var brain = LearningBrain.Load(PokemonTCG.Presentation.SavePaths.BrainPath); // the folder chosen on the Save Folders screen
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, a.Key, a.Value, b.Key, b.Value, 1000, 200, Environment.TickCount & 0x0FFFFFFF);
            session.Start();
            try
            {
                while (!session.Finished)
                {
                    var st = session.Status();
                    int total = st.Games + st.EvalTotal;
                    int done = st.Done + st.EvalDone;
                    if (EditorUtility.DisplayCancelableProgressBar("Pokemon TCG", st.Phase + " (" + done + " / " + total + ")", total == 0 ? 0f : (float)done / total))
                        session.Cancel();
                    System.Threading.Thread.Sleep(250);
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            var end = session.Status();
            Debug.Log("[PokemonTCG] Neural AI trained: " + a.Key + " vs " + b.Key + ", " + end.Done + " games (A " + end.WinsA + ", B " + end.WinsB + ", " + end.Errors + " errors) in "
                      + end.Seconds.ToString("0") + "s. Against the classic bot: " + (end.ScoreBefore * 100).ToString("0") + "% before, " + (end.ScoreAfter * 100).ToString("0")
                      + "% after. Brain: " + brain.FeatureCount + " weights, " + brain.GamesTrained + " games trained.");
        }

        private static List<KeyValuePair<string, List<CardDef>>> LoadBuiltInDecks()
        {
            var db = UnityCardData.LoadAllCards();
            var decks = new List<KeyValuePair<string, List<CardDef>>>();
            foreach (var d in DeckLibrary.LoadAll().Where(x => x.BuiltIn))
            {
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, d, problems);
                if (problems.Count == 0) decks.Add(new KeyValuePair<string, List<CardDef>>(d.Name, cards));
                else Debug.LogWarning("[PokemonTCG] Skipping " + d.Name + ": " + string.Join(" ", problems.ToArray()));
            }
            return decks;
        }

        /// <summary>Every built-in deck plays every other one (both seats) with heuristic bots; prints a win-rate table.</summary>
        [MenuItem("Pokemon TCG/Simulate Round Robin - All Built-in Decks (Heuristic Bots)")]
        public static void SimulateRoundRobin()
        {
            var decks = LoadBuiltInDecks();
            if (decks.Count < 2) return;
            var scripts = StandardCards.CreateRegistry();
            const int gamesPerSeat = 2;
            var wins = new int[decks.Count];
            var played = new int[decks.Count];
            var errors = new List<string>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            int total = decks.Count * (decks.Count - 1), done = 0;
            try
            {
                for (int i = 0; i < decks.Count; i++)
                {
                    for (int j = 0; j < decks.Count; j++)
                    {
                        if (i == j) continue;
                        done++;
                        EditorUtility.DisplayProgressBar("Pokemon TCG", decks[i].Key + " vs " + decks[j].Key, (float)done / total);
                        var report = MatchRunner.Run(scripts, decks[i].Value, decks[j].Value, gamesPerSeat, 7000 + (i * 100 + j) * gamesPerSeat, (seat, s) => new HeuristicAgent(s), true);
                        wins[i] += report.Wins[0];
                        wins[j] += report.Wins[1];
                        played[i] += report.Completed;
                        played[j] += report.Completed;
                        foreach (var e in report.Errors) errors.Add(decks[i].Key + " vs " + decks[j].Key + ": " + e);
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("[PokemonTCG] Round robin, heuristic bots (" + watch.Elapsed.TotalSeconds.ToString("0") + "s). Win rate by deck:");
            foreach (int i in Enumerable.Range(0, decks.Count).OrderByDescending(k => played[k] == 0 ? 0 : (double)wins[k] / played[k]))
                sb.AppendLine("  " + (played[i] == 0 ? 0 : 100.0 * wins[i] / played[i]).ToString("0.0") + "%  " + decks[i].Key + "  (" + wins[i] + "/" + played[i] + ")");
            if (errors.Count > 0)
            {
                sb.AppendLine("Errors (" + errors.Count + "):");
                foreach (var e in errors.Take(20)) sb.AppendLine("  " + e);
                Debug.LogWarning(sb.ToString());
            }
            else Debug.Log(sb.ToString());
        }

        [MenuItem("Pokemon TCG/Simulate 1,000 Games - All Built-in Matchups (Random Bots, Fuzz Test)")]
        public static void SimulateAllRandom()
        {
            var decks = LoadBuiltInDecks();
            int pairs = decks.Count * decks.Count;
            if (pairs == 0) return;
            foreach (var x in decks)
                foreach (var y in decks)
                    Simulate(x.Key + " vs " + y.Key + ", random bots", x.Value, y.Value, Math.Max(1, 1000 / pairs), (seat, s) => new RandomAgent(s));
        }

        private static void Simulate(string label, List<CardDef> deck0, List<CardDef> deck1, int games, Func<int, int, IPlayerAgent> factory)
        {
            if (deck0.Count != 60 || deck1.Count != 60)
            {
                Debug.LogError("[PokemonTCG] A deck didn't load as 60 cards; see warnings above.");
                return;
            }
            var scripts = StandardCards.CreateRegistry();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            SimulationReport report;
            try
            {
                report = MatchRunner.Run(scripts, deck0, deck1, games, 1000, factory, true, (done, total) =>
                {
                    if (done % 20 == 0)
                        EditorUtility.DisplayProgressBar("Pokemon TCG", label + ": " + done + " / " + total, (float)done / total);
                });
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
            string text = "[PokemonTCG] " + label + " (" + watch.Elapsed.TotalSeconds.ToString("0.0") + "s)\n" + report.Summary();
            if (report.Errors.Count > 0) Debug.LogWarning(text);
            else Debug.Log(text);
        }
    }
}
