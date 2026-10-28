using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    public sealed class SimulationReport
    {
        public int Games;
        public int[] Wins = new int[2];
        public int Draws;
        public int TotalTurns;
        public int LongestGame;
        public readonly Dictionary<string, int> EndReasons = new Dictionary<string, int>();
        public readonly List<string> Errors = new List<string>();

        public int Completed { get { return Games - Errors.Count; } }
        public double AverageTurns { get { return Completed == 0 ? 0 : (double)TotalTurns / Completed; } }

        public string Summary()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Games: " + Games + "  completed: " + Completed + "  errors: " + Errors.Count);
            sb.AppendLine("Player 1 wins: " + Wins[0] + "  Player 2 wins: " + Wins[1] + "  draws/unfinished: " + Draws);
            sb.AppendLine("Average turns: " + AverageTurns.ToString("0.0") + "  longest: " + LongestGame);
            sb.AppendLine("How games ended:");
            foreach (var kv in EndReasons.OrderByDescending(k => k.Value))
                sb.AppendLine("  " + kv.Value + " x " + kv.Key);
            if (Errors.Count > 0)
            {
                sb.AppendLine("First errors (re-run the seed to reproduce):");
                foreach (var e in Errors.Take(10)) sb.AppendLine("  " + e);
            }
            return sb.ToString();
        }
    }

    public static class MatchRunner
    {
        /// <summary>
        /// Plays many full games headlessly. Each game gets its own seed, so any failure can be replayed exactly.
        /// </summary>
        public static SimulationReport Run(
            CardScriptRegistry scripts,
            IList<CardDef> deck0,
            IList<CardDef> deck1,
            int games,
            int baseSeed,
            Func<int, int, IPlayerAgent> agentFactory,
            bool checkInvariants,
            Action<int, int> progress = null)
        {
            var report = new SimulationReport();
            for (int g = 0; g < games; g++)
            {
                int seed = baseSeed + g;
                report.Games++;
                try
                {
                    var engine = new GameEngine(scripts, agentFactory(0, seed * 2 + 1), agentFactory(1, seed * 2 + 2));
                    engine.CheckInvariants = checkInvariants;
                    engine.CardsPerPlayer = deck0.Count == deck1.Count ? deck0.Count : 0;
                    var result = engine.Run(deck0, deck1, seed);
                    if (result.Winner >= 0) report.Wins[result.Winner]++;
                    else report.Draws++;
                    report.TotalTurns += result.Turns;
                    report.LongestGame = Math.Max(report.LongestGame, result.Turns);
                    string reason = Generalize(result.Reason);
                    int n;
                    report.EndReasons.TryGetValue(reason, out n);
                    report.EndReasons[reason] = n + 1;
                }
                catch (Exception e)
                {
                    string where = "";
                    if (e.StackTrace != null)
                    {
                        var firstLine = e.StackTrace.Split('\n').FirstOrDefault();
                        if (firstLine != null) where = " @ " + firstLine.Trim();
                    }
                    report.Errors.Add("seed " + seed + ": " + e.GetType().Name + ": " + e.Message + where);
                }
                if (progress != null) progress(g + 1, games);
            }
            return report;
        }

        private static string Generalize(string reason)
        {
            if (string.IsNullOrEmpty(reason)) return "(no reason)";
            return reason.Replace("Player 1", "A player").Replace("Player 2", "A player");
        }
    }
}
