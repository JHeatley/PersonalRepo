using System;
using System.Collections.Generic;
using System.Globalization;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One check of the Neural AI's network against the champion (ChampionGate).</summary>
    public sealed class ChampionCheck
    {
        /// <summary>The brain's training games when it was checked.</summary>
        public int GamesTrained;
        public int Games, Wins, Draws, Losses;
        /// <summary>The network's share of the points against the champion (a draw is half), -1 for the first champion.</summary>
        public double WinRate = -1;
        /// <summary>It became the champion (always for the first one).</summary>
        public bool Promoted;
        /// <summary>The champion's version after this check.</summary>
        public int Version;
        public string When = "";
    }

    /// <summary>
    /// The fixed yardstick (step 1 towards an expert player). A copy of the network is kept as the champion; every so often the
    /// network plays it on the same games (the same decks, seats and shuffles each time), and when it wins clearly (55% or more)
    /// it becomes the new champion, the next version. Self-play win rates can't show progress (the opponent improves too) and
    /// the classic bot is soon too easy; the champion only moves when the network really got better. The versions and checks are
    /// kept in the brain and shown on the Train screen.
    /// </summary>
    public static class ChampionGate
    {
        public const double PromoteAt = 0.55;
        public const int DefaultGames = 300;
        /// <summary>The same games every check, so the numbers compare.</summary>
        public const int Seed = 4242001;

        /// <summary>
        /// Checks the brain's network against its champion and promotes it if it won clearly. With no champion yet, the network
        /// becomes the first one (nothing is played). Returns null when it was stopped before it finished.
        /// </summary>
        public static ChampionCheck Run(LearningBrain brain, CardScriptRegistry scripts, List<KeyValuePair<string, List<CardDef>>> decks, int games,
            int threads, Func<bool> stop)
        {
            string when = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
            if (brain.Champion == null)
            {
                brain.SetChampion(brain.Policy.Clone());
                var first = new ChampionCheck { GamesTrained = brain.GamesTrained, Promoted = true, Version = brain.ChampionVersion, When = when };
                brain.RecordChampionCheck(first);
                return first;
            }
            if (decks == null || decks.Count == 0) return null;
            var champion = brain.Champion;
            var trainer = new PolicyTrainer(brain.Policy, scripts, decks, Seed) { Matchup = false, Threads = Math.Max(1, threads), ShouldStop = stop };
            try
            {
                int w, d, l;
                double wr = trainer.Evaluate(seed => new PolicyAgent(champion, seed) { Greedy = true }, games, Seed, out w, out d, out l);
                int played = w + d + l;
                if (wr < 0 || played < games * 0.9) return null; // stopped (or most games failed): no verdict
                bool promote = wr >= PromoteAt;
                if (promote) brain.SetChampion(brain.Policy.Clone());
                var check = new ChampionCheck
                {
                    GamesTrained = brain.GamesTrained, Games = played, Wins = w, Draws = d, Losses = l, WinRate = wr, Promoted = promote,
                    Version = brain.ChampionVersion, When = when
                };
                brain.RecordChampionCheck(check);
                return check;
            }
            finally
            {
                trainer.Dispose();
            }
        }

        /// <summary>One line for a check ("At 40,000 games: won 58% of 300 games against version 3: it's version 4 now").</summary>
        public static string Describe(ChampionCheck c)
        {
            string at = "At " + c.GamesTrained.ToString("N0", CultureInfo.InvariantCulture) + " games";
            if (c.WinRate < 0) return at + ": the network became the first champion (version " + c.Version + ").";
            string won = "won " + Math.Round(c.WinRate * 100) + "% of " + c.Games + " games against version " + (c.Promoted ? c.Version - 1 : c.Version);
            return at + ": " + won + (c.Promoted ? ", so it's the new champion (version " + c.Version + ")." : " (it needs " + Math.Round(PromoteAt * 100) + "% to take over).");
        }
    }
}
