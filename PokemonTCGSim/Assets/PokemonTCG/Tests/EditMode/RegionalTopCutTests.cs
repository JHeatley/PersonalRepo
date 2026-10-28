using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v70: Regional Sim meta shares (a percent of the AI players for each deck) and a top cut after Swiss: the top 2, 4 or 8
    /// play single elimination, best of 1 or 3, the higher seed chooses who goes first in game 1, and drawn games are played
    /// again (after 9 games the higher seed goes through).
    /// </summary>
    public class RegionalTopCutTests
    {
        private static readonly string[] OneDeck = { "Test Deck" };

        private static Dictionary<string, string> Texts(params string[] names)
        {
            var d = new Dictionary<string, string>();
            foreach (var n in names) d[n] = "";
            return d;
        }

        /// <summary>A best of 1 Swiss event with a top cut, whose AI tables are decided by hand (no cards: nothing plays in the background).</summary>
        private static RegionalSim MakeEvent(int aiSeats, int rounds, bool withYou, int cut, bool cutBestOf3, int seed)
        {
            var sim = RegionalSim.Create(aiSeats, rounds, false, "Standard", withYou ? "Test Deck" : null, OneDeck, Texts(OneDeck), false, seed);
            sim.SetTopCut(cut, cutBestOf3);
            sim.Begin();
            return sim;
        }

        private static List<RegionalMatch> RoundMatches(RegionalSim sim, int round)
        {
            return sim.View().Matches.Where(x => x.Round == round).OrderBy(x => x.Table).ToList();
        }

        /// <summary>Decides every AI table of the current round: A wins games until it has won `wins` of them.</summary>
        private static void AWinsEveryTable(RegionalSim sim, int wins)
        {
            int round = sim.Round;
            var names = sim.View().Names;
            foreach (var m in RoundMatches(sim, round))
            {
                if (m.Finished || m.IsBye || names[m.A] == "You") continue;
                for (int g = m.Games.Count; g < wins; g++) sim.RecordTableGame(round, m.Table, g, 0);
            }
        }

        private static string Seeds(RegionalMatch m)
        {
            return m.SeedA + "v" + m.SeedB;
        }

        // ================================================================== Meta shares

        [Test]
        public void Meta_Shares_Become_AI_Players_That_Add_Up()
        {
            var seats = RegionalSim.SeatsFromShares(new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("A", 25),
                new KeyValuePair<string, double>("B", 12.5),
                new KeyValuePair<string, double>("C", 62.5)
            }, 20);
            CollectionAssert.AreEqual(new[] { "A:5", "B:3", "C:12" }, seats.Select(s => s.Key + ":" + s.Value).ToArray(),
                "2.5 and 12.5 players: the seat left over goes to the deck listed first");

            var thirds = RegionalSim.SeatsFromShares(new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("A", 33.33),
                new KeyValuePair<string, double>("B", 33.33),
                new KeyValuePair<string, double>("C", 33.34)
            }, 100);
            CollectionAssert.AreEqual(new[] { "A:33", "B:33", "C:34" }, thirds.Select(s => s.Key + ":" + s.Value).ToArray());

            var part = RegionalSim.SeatsFromShares(new List<KeyValuePair<string, double>>
            {
                new KeyValuePair<string, double>("A", 50),
                new KeyValuePair<string, double>("B", 25),
                new KeyValuePair<string, double>("C", 0)
            }, 31);
            CollectionAssert.AreEqual(new[] { "A:15", "B:8" }, part.Select(s => s.Key + ":" + s.Value).ToArray(),
                "75% of 31 AI players is 23 of them; a deck at 0% is left out");

            var sim = RegionalSim.Create(20, 3, false, "Standard", null, new[] { "A", "B", "C" }, seats, false, Texts("A", "B", "C"), false, 7);
            var decks = sim.View().DeckNames;
            Assert.AreEqual(5, decks.Count(deck => deck == "A"));
            Assert.AreEqual(3, decks.Count(deck => deck == "B"));
            Assert.AreEqual(12, decks.Count(deck => deck == "C"));
        }

        // ================================================================== The top cut

        [Test]
        public void A_Top_Cut_Is_2_4_Or_8_Players_Set_Before_The_Event_Starts()
        {
            var sim = RegionalSim.Create(6, 3, false, "Standard", null, OneDeck, Texts(OneDeck), false, 41);
            Assert.Throws<ArgumentException>(() => sim.SetTopCut(8, false), "6 players can't have a top 8");
            Assert.Throws<ArgumentException>(() => sim.SetTopCut(3, false));
            sim.SetTopCut(4, true);
            Assert.AreEqual(4, sim.TopCut);
            Assert.IsTrue(sim.CutBestOfThree);
            sim.Begin();
            Assert.Throws<InvalidOperationException>(() => sim.SetTopCut(2, false));
            Assert.AreEqual(3, RegionalSim.CutRoundsFor(8));
            Assert.AreEqual(2, RegionalSim.CutRoundsFor(4));
            Assert.AreEqual(1, RegionalSim.CutRoundsFor(2));
            Assert.AreEqual(0, RegionalSim.CutRoundsFor(0));
            Assert.AreEqual("Round 3", RegionalSim.RoundName(3, 3, 8));
            Assert.AreEqual("Top 8", RegionalSim.RoundName(4, 3, 8));
            Assert.AreEqual("Top 4", RegionalSim.RoundName(5, 3, 8));
            Assert.AreEqual("Final", RegionalSim.RoundName(6, 3, 8));
            Assert.AreEqual("Final", RegionalSim.RoundName(6, 5, 2));
        }

        [Test]
        public void A_Top_8_Is_Seeded_From_The_Swiss_Standings_And_Played_Out_After_Swiss()
        {
            var sim = MakeEvent(16, 3, false, 8, false, 21);
            for (int r = 1; r <= 3; r++)
            {
                Assert.AreEqual(r, sim.Round);
                AWinsEveryTable(sim, 1);
            }
            var v = sim.View();
            Assert.AreEqual(4, v.Round, "the top 8 comes after round 3");
            Assert.IsTrue(v.InTopCut);
            Assert.IsFalse(v.Finished);
            var swiss = sim.Standings();
            CollectionAssert.AreEqual(swiss.Take(8).Select(s => s.Id).ToArray(), v.CutSeeds.ToArray(), "seeded by the standings after Swiss");

            var top8 = RoundMatches(sim, 4);
            CollectionAssert.AreEqual(new[] { "1v8", "4v5", "2v7", "3v6" }, top8.Select(Seeds).ToArray());
            foreach (var m in top8)
            {
                Assert.IsTrue(m.IsCut);
                Assert.AreEqual(v.CutSeeds[m.SeedA - 1], m.A, "the higher seed is A");
                Assert.AreEqual(v.CutSeeds[m.SeedB - 1], m.B);
            }

            AWinsEveryTable(sim, 1);
            CollectionAssert.AreEqual(new[] { "1v4", "2v3" }, RoundMatches(sim, 5).Select(Seeds).ToArray(), "the winners of tables 1 and 2 meet, then 3 and 4");
            AWinsEveryTable(sim, 1);
            CollectionAssert.AreEqual(new[] { "1v2" }, RoundMatches(sim, 6).Select(Seeds).ToArray(), "the final");
            Assert.IsFalse(sim.Finished);
            AWinsEveryTable(sim, 1);
            Assert.IsTrue(sim.Finished);

            var end = sim.View();
            Assert.AreEqual(end.CutSeeds[0], end.Champion, "seed 1 won every match");
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7, 8 }, end.CutSeeds.Select(id => end.Placement[id]).ToArray(),
                "the final's loser is 2nd; the top 4's losers are 3rd and 4th by seed, the top 8's 5th to 8th");
            foreach (var other in swiss.Skip(8)) Assert.AreEqual(other.Rank, end.Placement[other.Id], "everyone else by their Swiss standing");
            CollectionAssert.AreEqual(swiss.Select(s => s.Id + " " + s.Record).ToArray(), sim.Standings().Select(s => s.Id + " " + s.Record).ToArray(),
                "the top cut doesn't change the Swiss standings");
            Assert.AreEqual(1, end.Decks.Single().BestRank);
        }

        [Test]
        public void A_Drawn_Top_Cut_Game_Is_Played_Again_With_The_Higher_Seed_Choosing()
        {
            var sim = MakeEvent(8, 1, false, 2, false, 31);
            AWinsEveryTable(sim, 1);
            Assert.AreEqual(2, sim.Round);
            var match = RoundMatches(sim, 2).Single();
            Assert.AreEqual("1v2", Seeds(match));
            Assert.AreEqual(0, RegionalSim.OrderChooserFor(match), "the higher seed (A) chooses in game 1");
            Assert.IsTrue(RegionalSim.HigherSeedChoosesFor(match));

            Assert.IsTrue(sim.RecordTableGame(2, 1, 0, -1), "a drawn game");
            match = RoundMatches(sim, 2).Single();
            Assert.IsFalse(match.Finished, "a best of 1 top cut match needs a winner");
            Assert.IsFalse(sim.Finished);
            Assert.AreEqual(0, RegionalSim.OrderChooserFor(match), "the replay: the higher seed chooses again");
            Assert.IsTrue(RegionalSim.HigherSeedChoosesFor(match));

            Assert.IsTrue(sim.RecordTableGame(2, 1, 1, 1), "the lower seed wins the replay");
            Assert.IsTrue(sim.Finished);
            var v = sim.View();
            Assert.AreEqual(match.B, v.Champion);
            Assert.AreEqual(1, v.Placement[match.B]);
            Assert.AreEqual(2, v.Placement[match.A]);
        }

        [Test]
        public void A_Top_Cut_Match_Still_Level_After_9_Games_Goes_To_The_Higher_Seed()
        {
            var m = new RegionalMatch { A = 0, B = 1, SeedA = 2, SeedB = 1 };
            for (int g = 0; g < RegionalSim.MaxCutGames - 1; g++) m.Games.Add(-1);
            Assert.IsFalse(RegionalSim.Decided(m, false), "8 drawn games");
            Assert.AreEqual(-1, m.Result);
            Assert.AreEqual(1, RegionalSim.OrderChooserFor(m), "B is the higher seed and keeps choosing");
            m.Games.Add(-1);
            Assert.IsTrue(RegionalSim.Decided(m, false));
            m.Finished = true;
            Assert.AreEqual(1, m.Result, "B is seed 1");

            var bo3 = new RegionalMatch { A = 0, B = 1, SeedA = 1, SeedB = 4 };
            bo3.Games.AddRange(new[] { 0, 1, -1 });
            Assert.IsFalse(RegionalSim.Decided(bo3, true), "1-1 and a drawn game: a top cut match plays on (in Swiss it's a tie)");
            Assert.AreEqual(0, RegionalSim.OrderChooserFor(bo3), "A lost the last game that had a winner, so A chooses");
            Assert.IsFalse(RegionalSim.HigherSeedChoosesFor(bo3));
            bo3.Games.Add(1);
            Assert.IsTrue(RegionalSim.Decided(bo3, true));
            bo3.Finished = true;
            Assert.AreEqual(1, bo3.Result, "B won 2-1");

            Assert.AreEqual(-1, RegionalSim.OrderChooserFor(new RegionalMatch { A = 0, B = 1 }), "Swiss game 1 is still the coin flip");
        }

        [Test]
        public void You_Are_Player_A_In_The_Top_Cut_And_The_Higher_Seed_Chooses_In_Game_1()
        {
            var sim = MakeEvent(7, 1, true, 8, false, 61);
            AWinsEveryTable(sim, 1);
            Assert.IsTrue(sim.RecordYourGame(1, 0, -1), "you lose round 1");
            Assert.AreEqual(2, sim.Round, "the top 8");
            var yours = sim.YourMatch();
            Assert.IsNotNull(yours);
            Assert.IsTrue(yours.IsCut);
            Assert.AreEqual(0, yours.A, "you are A");
            Assert.Greater(yours.SeedA, yours.SeedB, "0-1 after Swiss: you're the lower seed");
            Assert.AreEqual(1, sim.YourOrderChooser(), "your opponent is the higher seed and chooses");
            Assert.IsTrue(RegionalSim.HigherSeedChoosesFor(yours));

            AWinsEveryTable(sim, 1);
            Assert.AreEqual(2, sim.Round, "the top 8 waits for your match");
            Assert.IsTrue(sim.RecordYourGame(2, 0, 1), "you win");
            Assert.AreEqual(3, sim.Round, "the top 4");
            Assert.AreEqual(0, sim.YourMatch().A);
        }

        [Test]
        public void A_Top_Cut_Is_Saved_And_Read_Back()
        {
            var sim = MakeEvent(8, 1, false, 4, true, 51);
            AWinsEveryTable(sim, 1);
            Assert.AreEqual(2, sim.Round, "the top 4");
            Assert.IsTrue(sim.RecordTableGame(2, 1, 0, 0));
            string text = sim.Serialize();
            var back = RegionalSim.Parse(text);
            Assert.AreEqual(text, back.Serialize());
            Assert.AreEqual(4, back.TopCut);
            Assert.IsTrue(back.CutBestOfThree);
            var seeds = back.View().CutSeeds;
            CollectionAssert.AreEqual(sim.View().CutSeeds, seeds);
            CollectionAssert.AreEqual(RoundMatches(sim, 2).Select(x => Seeds(x) + " " + x.Score).ToArray(),
                                      RoundMatches(back, 2).Select(x => Seeds(x) + " " + x.Score).ToArray());

            AWinsEveryTable(back, 2);
            Assert.AreEqual(3, back.Round, "the final");
            AWinsEveryTable(back, 2);
            Assert.IsTrue(back.Finished);
            Assert.AreEqual(seeds[0], back.View().Champion);
        }
    }
}
