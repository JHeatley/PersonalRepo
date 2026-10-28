using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v127: Regional Sim Day 1 / Day 2 (players with enough match points after Day 1 play Day 2, records carried over; the
    /// others place below them), dropping (your unfinished match is a loss, you aren't paired again, you keep your place), and
    /// the asymmetrical top cut (everyone on the last seat's match points plays single elimination for those seats).
    /// </summary>
    public class RegionalDay2Tests
    {
        private static readonly string[] OneDeck = { "Test Deck" };

        private static Dictionary<string, string> Texts()
        {
            return new Dictionary<string, string> { { "Test Deck", "" } };
        }

        private static RegionalSim Make(int aiSeats, int rounds, bool withYou, int seed)
        {
            return RegionalSim.Create(aiSeats, rounds, false, "Standard", withYou ? "Test Deck" : null, OneDeck, Texts(), false, seed);
        }

        private static List<RegionalMatch> RoundMatches(RegionalSim sim, int round)
        {
            return sim.View().Matches.Where(x => x.Round == round).OrderBy(x => x.Table).ToList();
        }

        /// <summary>Decides every AI table of the current round: A wins.</summary>
        private static void AWinsEveryTable(RegionalSim sim)
        {
            int round = sim.Round;
            var names = sim.View().Names;
            foreach (var m in RoundMatches(sim, round))
            {
                if (m.Finished || m.IsBye || names[m.A] == "You" || names[m.B] == "You") continue;
                sim.RecordTableGame(round, m.Table, m.Games.Count, 0);
            }
        }

        private static void PlayOut(RegionalSim sim)
        {
            for (int guard = 0; guard < 40 && !sim.Finished; guard++) AWinsEveryTable(sim);
            Assert.IsTrue(sim.Finished, "the event plays out");
        }

        private static void AssertPlacesAreUnique(RegionalView v)
        {
            CollectionAssert.AreEquivalent(Enumerable.Range(1, v.Players).ToArray(), v.Placement, "every player has a place, each place once");
        }

        // ================================================================== Day 2

        [Test]
        public void Day_2_Is_Played_By_The_Players_With_Enough_Match_Points()
        {
            var sim = Make(16, 9, false, 501);
            sim.SetDayTwo(3, 2, 6);
            sim.Begin();
            Assert.AreEqual(5, sim.Rounds, "3 rounds on Day 1 and 2 on Day 2");
            for (int r = 1; r <= 3; r++) AWinsEveryTable(sim);
            var v = sim.View();
            Assert.AreEqual(4, v.Round);
            Assert.IsTrue(v.Day2Decided);
            Assert.IsTrue(v.IsDay2Round);
            var made = v.Standings.Where(x => x.Day2).Select(x => x.Id).ToList();
            Assert.AreEqual(v.Standings.Count(x => x.Points >= 6), made.Count, "6 points or more after round 3 make Day 2");
            Assert.AreEqual(made.Count, v.Day2Players);
            foreach (var m in RoundMatches(sim, 4))
            {
                Assert.IsTrue(made.Contains(m.A), "only Day 2 players are paired");
                if (!m.IsBye) Assert.IsTrue(made.Contains(m.B), "only Day 2 players are paired");
            }
            Assert.AreEqual((made.Count + 1) / 2, RoundMatches(sim, 4).Count);

            PlayOut(sim);
            v = sim.View();
            int worstDay2 = v.Standings.Where(x => x.Day2).Max(x => x.Rank);
            int bestMissed = v.Standings.Where(x => x.MissedDay2).Min(x => x.Rank);
            Assert.Less(worstDay2, bestMissed, "everyone who made Day 2 places above everyone who didn't");
            foreach (var s in v.Standings.Where(x => x.MissedDay2))
                Assert.AreEqual(3, s.Wins + s.Losses + s.Ties, "players out after Day 1 keep their Day 1 record");
            foreach (var s in v.Standings.Where(x => x.Day2))
                Assert.AreEqual(5, s.Wins + s.Losses + s.Ties, "Day 2 records carry over from Day 1");
            AssertPlacesAreUnique(v);
        }

        [Test]
        public void Nobody_Making_Day_2_Ends_The_Event_After_Day_1()
        {
            var sim = Make(8, 9, false, 502);
            sim.SetDayTwo(2, 3, 99);
            sim.Begin();
            AWinsEveryTable(sim);
            AWinsEveryTable(sim);
            Assert.IsTrue(sim.Finished);
            Assert.AreEqual(0, sim.View().Day2Players);
        }

        [Test]
        public void Day_2_Is_Set_Before_The_Event_Starts()
        {
            var sim = Make(8, 3, false, 503);
            Assert.Throws<System.ArgumentException>(() => sim.SetDayTwo(0, 2, 6));
            sim.Begin();
            Assert.Throws<System.InvalidOperationException>(() => sim.SetDayTwo(3, 2, 6));
        }

        // ================================================================== Dropping

        [Test]
        public void Dropping_Concedes_Your_Match_And_The_Event_Plays_Out_Without_You()
        {
            var sim = Make(7, 3, true, 504);
            sim.Begin();
            Assert.IsTrue(sim.YourGamePending);
            var mine = sim.YourMatch();
            Assert.IsTrue(sim.View().YouStillIn);

            Assert.IsTrue(sim.DropYou());
            Assert.IsFalse(sim.DropYou(), "you can drop only once");
            Assert.IsTrue(sim.YouDropped);
            Assert.IsFalse(sim.YourGamePending);
            var conceded = RoundMatches(sim, 1).First(x => x.Table == mine.Table);
            Assert.IsTrue(conceded.Finished);
            Assert.AreEqual(0, conceded.ConcededBy);
            Assert.AreEqual(1, conceded.Result, "your opponent wins the match");

            AWinsEveryTable(sim);
            Assert.AreEqual(2, sim.Round);
            foreach (var m in RoundMatches(sim, 2))
                Assert.IsTrue(m.A != 0 && m.B != 0, "you aren't paired again");
            Assert.AreEqual(4, RoundMatches(sim, 2).Count, "7 players left: 3 matches and a bye");

            PlayOut(sim);
            var v = sim.View();
            Assert.IsTrue(v.YouDropped);
            Assert.IsFalse(v.YouStillIn);
            Assert.AreEqual("0-1-0", v.You.Record);
            Assert.IsTrue(v.You.Dropped);
            Assert.AreEqual(v.You.Rank, v.Placement[0], "you place where your record puts you");
            AssertPlacesAreUnique(v);
        }

        [Test]
        public void A_Player_Who_Dropped_Is_Left_Out_Of_The_Top_Cut()
        {
            var sim = Make(7, 3, true, 505);
            sim.SetTopCut(8, false);
            sim.Begin();
            sim.DropYou();
            PlayOut(sim);
            var v = sim.View();
            Assert.AreEqual(4, v.TopCut, "7 players left: the top 8 becomes a top 4");
            Assert.IsFalse(v.CutSeeds.Contains(0));
            AssertPlacesAreUnique(v);
        }

        // ================================================================== Asymmetrical top cut

        [Test]
        public void Asymmetrical_Top_Cut_Has_The_Bubble_Play_For_Its_Seats()
        {
            var sim = Make(16, 3, false, 506);
            sim.SetTopCut(4, false);
            sim.SetAsymmetricCut(true);
            sim.Begin();
            for (int r = 1; r <= 3; r++) AWinsEveryTable(sim);
            var v = sim.View();
            var swiss = v.Standings;
            int bubble = swiss[3].Points;
            int above = swiss.Count(s => s.Points > bubble);
            int pool = swiss.Count(s => s.Points == bubble);
            Assert.AreEqual(2, above, "16 players, 3 rounds: two at 3-0");
            Assert.AreEqual(6, pool, "and six at 2-1 for the other 2 seats");

            Assert.IsTrue(v.InPlayIn);
            Assert.IsTrue(v.InTopCut);
            Assert.AreEqual(2, v.PlayInSeats);
            CollectionAssert.AreEqual(swiss.Take(2).Select(s => s.Id).ToArray(), v.PlayInAbove.ToArray());
            CollectionAssert.AreEqual(swiss.Skip(2).Take(6).Select(s => s.Id).ToArray(), v.PlayInPool.ToArray(), "inside the cut by tiebreakers or not, all six play");
            Assert.AreEqual(0, v.CutSeeds.Count);
            Assert.AreEqual("Play-in 1", RegionalSim.RoundName(4, 3, 4, v.PlayInRounds));

            // 6 players for 2 seats: 3 matches (all six play), seeded 3 v 8, 4 v 7, 5 v 6.
            var first = RoundMatches(sim, 4);
            CollectionAssert.AreEqual(new[] { "3v8", "4v7", "5v6" }, first.Select(m => m.SeedA + "v" + m.SeedB).ToArray());
            Assert.IsTrue(first.All(m => m.PlayIn && m.IsCut));
            AWinsEveryTable(sim);

            // 3 left for 2 seats: the best seed has a bye, the other two play.
            v = sim.View();
            Assert.IsTrue(v.InPlayIn);
            Assert.AreEqual(2, v.PlayInRounds);
            var second = RoundMatches(sim, 5);
            CollectionAssert.AreEqual(new[] { "4v5" }, second.Select(m => m.SeedA + "v" + m.SeedB).ToArray());
            AWinsEveryTable(sim);

            // The top 4: the two above the bubble, then the winners by Swiss seed.
            v = sim.View();
            Assert.IsFalse(v.InPlayIn);
            CollectionAssert.AreEqual(new[] { swiss[0].Id, swiss[1].Id, swiss[2].Id, swiss[3].Id }, v.CutSeeds.ToArray());
            Assert.AreEqual(6, v.CutStart + 1);
            Assert.AreEqual("Top 4", RegionalSim.RoundName(6, 3, 4, v.PlayInRounds));
            Assert.AreEqual("Final", RegionalSim.RoundName(7, 3, 4, v.PlayInRounds));
            var cut = RoundMatches(sim, 6);
            Assert.IsTrue(cut.All(m => m.IsCut && !m.PlayIn));
            CollectionAssert.AreEqual(new[] { "1v4", "2v3" }, cut.Select(m => m.SeedA + "v" + m.SeedB).ToArray());

            PlayOut(sim);
            v = sim.View();
            Assert.AreEqual(7, v.Round);
            Assert.AreEqual(5, v.Placement[swiss[4].Id], "the play-in's other players place after the top 4, in Swiss order");
            AssertPlacesAreUnique(v);
            Assert.AreEqual(3, v.Standings.First(s => s.Id == swiss[2].Id).Wins + v.Standings.First(s => s.Id == swiss[2].Id).Losses,
                "the play-in doesn't change the Swiss standings");
        }

        [Test]
        public void Asymmetrical_Top_Cut_Without_A_Tie_On_The_Bubble_Has_No_Play_In()
        {
            var sim = Make(16, 3, false, 507);
            sim.SetTopCut(2, false);
            sim.SetAsymmetricCut(true);
            sim.Begin();
            for (int r = 1; r <= 3; r++) AWinsEveryTable(sim);
            var v = sim.View();
            Assert.IsFalse(v.InPlayIn, "the two at 3-0 hold both seats");
            Assert.AreEqual(0, v.PlayInPool.Count);
            Assert.AreEqual(2, v.CutSeeds.Count);
            Assert.AreEqual("Final", RegionalSim.RoundName(4, 3, 2, v.PlayInRounds));
        }

        // ================================================================== Saving

        [Test]
        public void Day_2_Drops_And_The_Play_In_Are_Saved()
        {
            var sim = Make(15, 9, true, 508);
            sim.SetDayTwo(2, 2, 3);
            sim.SetTopCut(4, true);
            sim.SetAsymmetricCut(true);
            sim.Begin();
            sim.DropYou();
            AWinsEveryTable(sim);
            AWinsEveryTable(sim);
            AWinsEveryTable(sim);
            string text = sim.Serialize();
            var back = RegionalSim.Parse(text);
            Assert.AreEqual(text, back.Serialize());
            var v = back.View();
            Assert.AreEqual(2, v.Day1Rounds);
            Assert.AreEqual(3, v.Day2Points);
            Assert.IsTrue(v.Day2Decided);
            Assert.IsTrue(v.YouDropped);
            Assert.IsTrue(v.AsymmetricCut);
            Assert.AreEqual(sim.View().Day2Players, v.Day2Players);
            Assert.AreEqual(0, v.Matches.First(m => m.Round == 1 && (m.A == 0 || m.B == 0)).ConcededBy);

            PlayOut(sim);
            text = sim.Serialize();
            back = RegionalSim.Parse(text);
            Assert.AreEqual(text, back.Serialize());
            var before = sim.View();
            var after = back.View();
            CollectionAssert.AreEqual(before.PlayInPool, after.PlayInPool);
            CollectionAssert.AreEqual(before.Placement, after.Placement);
            Assert.AreEqual(before.Matches.Count(m => m.PlayIn), after.Matches.Count(m => m.PlayIn));
        }
    }
}
