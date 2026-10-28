using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v57: Regional Sim. Swiss pairing (random round 1, equal records after that, no rematches, one bye for an odd number of
    /// players), best of 1 and best of 3, rounds that wait for every match, standings with tiebreakers, saving, AI-only events
    /// with a table to watch, and the loser of a game choosing to go first or second in the next.
    /// </summary>
    public class RegionalTests : ScenarioTestBase
    {
        private static readonly string[] OneDeck = { "Test Deck" };

        private static Dictionary<string, string> Texts(params string[] names)
        {
            var d = new Dictionary<string, string>();
            foreach (var n in names) d[n] = "";
            return d;
        }

        /// <summary>An event whose AI tables are decided by hand (no cards: nothing is played in the background).</summary>
        private static RegionalSim MakeEvent(int aiSeats, int rounds, bool bestOf3, bool withYou, int seed, bool watch = false)
        {
            var sim = RegionalSim.Create(aiSeats, rounds, bestOf3, "Standard", withYou ? "Test Deck" : null, OneDeck, Texts(OneDeck), false, seed, watch);
            sim.Begin();
            return sim;
        }

        private static List<RegionalMatch> RoundMatches(RegionalSim sim, int round)
        {
            return sim.View().Matches.Where(x => x.Round == round).OrderBy(x => x.Table).ToList();
        }

        /// <summary>Decides every AI table of the current round: A wins each game.</summary>
        private static void AWinsEveryTable(RegionalSim sim, bool bestOf3)
        {
            int round = sim.Round;
            var names = sim.View().Names;
            foreach (var m in RoundMatches(sim, round))
            {
                if (m.Finished || m.IsBye || names[m.A] == "You") continue;
                sim.RecordTableGame(round, m.Table, 0, 0);
                if (bestOf3) sim.RecordTableGame(round, m.Table, 1, 0);
            }
        }

        private List<DeckRecord> BuiltInStandardDecks()
        {
            var list = new List<DeckRecord>();
            foreach (var d in DeckLibrary.LoadAll())
            {
                if (!d.BuiltIn || d.Format != DeckFormat.Standard) continue;
                var problems = new List<string>();
                DeckLibrary.Build(db, d, problems);
                if (problems.Count == 0) list.Add(d);
            }
            return list;
        }

        // ================================================================== Pairing

        [Test]
        public void Round_1_Pairs_Everyone_Once_And_An_Odd_Number_Gets_One_Bye()
        {
            var sim = MakeEvent(7, 3, false, false, 11);
            var round1 = RoundMatches(sim, 1);
            Assert.AreEqual(4, round1.Count, "3 tables and a bye");
            Assert.AreEqual(1, round1.Count(x => x.IsBye));
            var seen = new List<int>();
            foreach (var m in round1)
            {
                seen.Add(m.A);
                if (!m.IsBye) seen.Add(m.B);
            }
            CollectionAssert.AreEquivalent(Enumerable.Range(0, 7), seen, "everyone plays exactly once");
            Assert.IsTrue(round1.Single(x => x.IsBye).Finished, "a bye is a finished win");
        }

        [Test]
        public void Later_Rounds_Pair_Equal_Records_Without_Rematches()
        {
            var sim = MakeEvent(8, 3, false, false, 21);
            var played = new HashSet<long>();
            foreach (var m in RoundMatches(sim, 1)) played.Add(RegionalSim.PairKey(m.A, m.B));
            AWinsEveryTable(sim, false);
            Assert.AreEqual(2, sim.Round, "round 1 is over, so round 2 is paired");
            var standings = sim.Standings();
            foreach (var m in RoundMatches(sim, 2))
            {
                var a = standings.Single(x => x.Id == m.A);
                var b = standings.Single(x => x.Id == m.B);
                Assert.AreEqual(a.Points, b.Points, "1-0 plays 1-0 and 0-1 plays 0-1");
                Assert.IsFalse(played.Contains(RegionalSim.PairKey(m.A, m.B)), "no rematch");
            }
        }

        [Test]
        public void Swiss_Pairs_Skip_A_Rematch_When_There_Is_Another_Way()
        {
            var played = new HashSet<long> { RegionalSim.PairKey(0, 1) };
            var pairs = RegionalSim.SwissPairs(new List<int> { 0, 1, 2, 3, 4, 5 }, played);
            Assert.AreEqual(3, pairs.Count);
            Assert.IsFalse(pairs.Any(p => RegionalSim.PairKey(p.Key, p.Value) == RegionalSim.PairKey(0, 1)));
            CollectionAssert.AreEquivalent(new[] { 0, 1, 2, 3, 4, 5 }, pairs.SelectMany(p => new[] { p.Key, p.Value }));

            // 2 players who already played: the rematch can't be avoided.
            var forced = RegionalSim.SwissPairs(new List<int> { 0, 1 }, played);
            Assert.AreEqual(1, forced.Count);
        }

        [Test]
        public void The_Bye_Goes_To_The_Lowest_Ranked_Player_Without_One()
        {
            var sim = MakeEvent(7, 3, false, false, 31);
            int bye1 = RoundMatches(sim, 1).Single(x => x.IsBye).A;
            AWinsEveryTable(sim, false);
            var standings = sim.Standings();
            var round2 = RoundMatches(sim, 2);
            int bye2 = round2.Single(x => x.IsBye).A;
            Assert.AreNotEqual(bye1, bye2, "not the same player twice");
            // The round 2 bye counts as a win as soon as it's given: its 3 points are the player's only ones.
            Assert.AreEqual(3, standings.Single(x => x.Id == bye2).Points, "a player without a win gets it");
        }

        [Test]
        public void Standings_Break_Ties_By_Opponents_Win_Percentage()
        {
            var sim = MakeEvent(8, 3, false, false, 41);
            AWinsEveryTable(sim, false);
            AWinsEveryTable(sim, false);
            var st = sim.Standings();
            for (int k = 1; k < st.Count; k++)
            {
                Assert.GreaterOrEqual(st[k - 1].Points, st[k].Points);
                if (st[k - 1].Points == st[k].Points) Assert.GreaterOrEqual(st[k - 1].OppWinPct, st[k].OppWinPct);
            }
            // Each player's win percentage is at least 25%, and the first tiebreaker averages the opponents'.
            var view = sim.View();
            foreach (var s in st)
            {
                Assert.GreaterOrEqual(s.WinPct, 0.25);
                var opps = view.Matches.Where(x => x.Finished && !x.IsBye && (x.A == s.Id || x.B == s.Id)).Select(x => x.A == s.Id ? x.B : x.A).ToList();
                Assert.AreEqual(opps.Average(o => st.Single(x => x.Id == o).WinPct), s.OppWinPct, 1e-9);
            }
            Assert.AreEqual(1, st[0].Rank);
            Assert.AreEqual(2, st[0].Wins, "a 2-0 player on top after 2 rounds");
        }

        // ================================================================== Matches and rounds

        [Test]
        public void Best_Of_3_Ends_At_2_Wins_And_Best_Of_1_After_1_Game()
        {
            var m = new RegionalMatch { A = 0, B = 1 };
            m.Games.Add(0);
            Assert.IsTrue(RegionalSim.Decided(m, false));
            Assert.IsFalse(RegionalSim.Decided(m, true));
            m.Games.Add(1);
            Assert.IsFalse(RegionalSim.Decided(m, true), "1-1");
            m.Games.Add(0);
            Assert.IsTrue(RegionalSim.Decided(m, true));
            Assert.AreEqual(0, m.Result, "2-1 for A");

            var tie = new RegionalMatch { A = 0, B = 1 };
            tie.Games.AddRange(new[] { 0, 1, -1 });
            Assert.IsTrue(RegionalSim.Decided(tie, true), "3 games played");
            Assert.AreEqual(-1, tie.Result, "1-1 with a drawn game is a tie");
        }

        [Test]
        public void A_Round_Waits_For_Your_Match_And_You_Are_Always_Player_A()
        {
            var sim = MakeEvent(5, 2, true, true, 51);
            var yours = sim.YourMatch();
            Assert.IsNotNull(yours);
            Assert.AreEqual(0, yours.A, "you are A");
            Assert.AreEqual("You", sim.View().Names[yours.A]);
            AWinsEveryTable(sim, true);
            Assert.AreEqual(1, sim.Round, "the AI tables are over, but yours isn't");
            Assert.IsTrue(sim.YourGamePending);
            Assert.AreEqual(-1, sim.YourOrderChooser(), "game 1: the coin flip");

            Assert.IsTrue(sim.RecordYourGame(1, 0, 1));
            Assert.IsFalse(sim.RecordYourGame(1, 0, 1), "the same game isn't counted twice");
            Assert.AreEqual(1, sim.YourOrderChooser(), "you won game 1: your opponent chooses");
            Assert.IsTrue(sim.RecordYourGame(1, 1, -1));
            Assert.AreEqual(0, sim.YourOrderChooser(), "you lost game 2: you choose");
            Assert.AreEqual(1, sim.Round);
            Assert.IsTrue(sim.RecordYourGame(1, 2, 1));
            Assert.AreEqual(2, sim.Round, "your match was the last one: round 2 is paired");
            var you = sim.View().You;
            Assert.AreEqual("1-0-0", you.Record);
            Assert.AreEqual(2, you.GameWins);
            Assert.AreEqual(1, you.GameLosses);
        }

        [Test]
        public void The_Event_Ends_After_The_Last_Round()
        {
            var sim = MakeEvent(6, 2, false, false, 61);
            AWinsEveryTable(sim, false);
            Assert.IsFalse(sim.Finished);
            AWinsEveryTable(sim, false);
            Assert.IsTrue(sim.Finished);
            var st = sim.Standings();
            Assert.AreEqual(st.Sum(x => x.Wins), st.Sum(x => x.Losses), "every win is someone's loss");
            Assert.AreEqual(2, st[0].Wins);
            Assert.IsFalse(sim.RecordTableGame(2, 1, 1, 0), "nothing more to record");
        }

        [Test]
        public void A_4000_Player_Event_Pairs_Every_Round_Without_Rematches()
        {
            var sim = MakeEvent(4000, 3, false, false, 121);
            for (int r = 1; r <= 3; r++)
            {
                var round = RoundMatches(sim, r);
                Assert.AreEqual(2000, round.Count, "round " + r);
                var seen = new HashSet<int>();
                foreach (var m in round)
                {
                    Assert.IsTrue(seen.Add(m.A));
                    Assert.IsTrue(seen.Add(m.B));
                }
                AWinsEveryTable(sim, false);
            }
            Assert.IsTrue(sim.Finished);
            var played = new HashSet<long>();
            foreach (var m in sim.View().Matches) Assert.IsTrue(played.Add(RegionalSim.PairKey(m.A, m.B)), "no rematches");
            Assert.AreEqual(500, sim.Standings().Count(x => x.Wins == 3), "3-0: one in eight");
        }

        [Test]
        public void An_Event_Needs_6_To_10000_Players()
        {
            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(5, 3, false, "Standard", null, OneDeck, Texts(OneDeck), false, 1));
            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(4, 3, false, "Standard", "Test Deck", OneDeck, Texts(OneDeck), false, 1));
            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(10001, 3, false, "Standard", null, OneDeck, Texts(OneDeck), false, 1));
            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(10000, 3, false, "Standard", "Test Deck", OneDeck, Texts(OneDeck), false, 1), "10,001 with you");
            Assert.AreEqual(6, RegionalSim.Create(5, 3, false, "Standard", "Test Deck", OneDeck, Texts(OneDeck), false, 1).PlayerCount, "you and 5 AI players");
            Assert.AreEqual(4000, RegionalSim.Create(4000, 3, false, "Standard", null, OneDeck, Texts(OneDeck), false, 1).PlayerCount);
            Assert.AreEqual(10000, RegionalSim.Create(10000, 3, false, "Standard", null, OneDeck, Texts(OneDeck), false, 1).PlayerCount, "v69: up to 10,000");
        }

        [Test]
        public void A_10000_Player_Event_Pairs_Its_First_Rounds_Without_Rematches()
        {
            var sim = MakeEvent(10000, 2, false, false, 131);
            for (int r = 1; r <= 2; r++)
            {
                var round = RoundMatches(sim, r);
                Assert.AreEqual(5000, round.Count, "round " + r);
                var seen = new HashSet<int>();
                foreach (var m in round)
                {
                    Assert.IsTrue(seen.Add(m.A));
                    Assert.IsTrue(seen.Add(m.B));
                }
                AWinsEveryTable(sim, false);
            }
            Assert.IsTrue(sim.Finished);
            var played = new HashSet<long>();
            foreach (var m in sim.View().Matches) Assert.IsTrue(played.Add(RegionalSim.PairKey(m.A, m.B)), "no rematches");
        }

        [Test]
        public void You_Can_Choose_The_AI_Players_Decks_And_Leave_The_Rest_To_Chance()
        {
            var names = new List<string> { "Deck A", "Deck B", "Deck C" };
            var texts = names.ToDictionary(n => n, n => "");
            var chosen = new List<KeyValuePair<string, int>> { new KeyValuePair<string, int>("Deck A", 6), new KeyValuePair<string, int>("Deck B", 2) };
            var sim = RegionalSim.Create(20, 3, false, "Standard", null, names, chosen, true, texts, false, 7);
            var decksPlayed = Enumerable.Range(0, sim.PlayerCount).Select(i => sim.Entrant(i).Deck).ToList();
            Assert.GreaterOrEqual(decksPlayed.Count(x => x == "Deck A"), 6, "at least the 6 you chose");
            Assert.GreaterOrEqual(decksPlayed.Count(x => x == "Deck B"), 2);
            Assert.AreEqual(20, decksPlayed.Count);

            // Every seat chosen, with nothing left to chance.
            var all = new List<KeyValuePair<string, int>> { new KeyValuePair<string, int>("Deck A", 15), new KeyValuePair<string, int>("Deck C", 5) };
            var exact = RegionalSim.Create(20, 3, false, "Standard", null, names, all, false, texts, false, 8);
            var exactDecks = Enumerable.Range(0, exact.PlayerCount).Select(i => exact.Entrant(i).Deck).ToList();
            Assert.AreEqual(15, exactDecks.Count(x => x == "Deck A"));
            Assert.AreEqual(5, exactDecks.Count(x => x == "Deck C"));
            Assert.AreEqual(0, exactDecks.Count(x => x == "Deck B"));

            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(20, 3, false, "Standard", null, names, chosen, false, texts, false, 9),
                "8 of 20 chosen, with random picking off");
            var tooMany = new List<KeyValuePair<string, int>> { new KeyValuePair<string, int>("Deck A", 21) };
            Assert.Throws<System.ArgumentException>(() => RegionalSim.Create(20, 3, false, "Standard", null, names, tooMany, true, texts, false, 9));
        }

        [Test]
        public void Suggested_Rounds_Grow_With_The_Event()
        {
            Assert.AreEqual(3, RegionalSim.SuggestedRounds(8));
            Assert.AreEqual(5, RegionalSim.SuggestedRounds(32));
            Assert.AreEqual(7, RegionalSim.SuggestedRounds(100));
            Assert.AreEqual(9, RegionalSim.SuggestedRounds(600));
        }

        // ================================================================== Watching a table (AI only)

        [Test]
        public void Watching_Keeps_One_Table_Each_Round_For_The_Board()
        {
            var sim = MakeEvent(6, 2, false, false, 71, true);
            var featured = sim.FeaturedMatch();
            Assert.IsNotNull(featured, "one table of round 1 is picked to watch");
            Assert.IsTrue(sim.Watching);
            // The background plays the other tables (without cards each game counts as a draw, instantly).
            sim.PlayAiTablesNow();
            var round1 = RoundMatches(sim, 1);
            Assert.IsFalse(round1.Single(x => x.Table == featured.Table).Finished, "the watched table waits for the board");
            Assert.AreEqual(2, round1.Count(x => x.Finished));
            Assert.AreEqual(1, sim.Round, "the round waits for it");
            Assert.IsTrue(sim.RecordFeaturedGame(1, featured.Table, 0, 1));
            Assert.AreEqual(2, sim.Round);
            Assert.IsNotNull(sim.FeaturedMatch(), "round 2 has its own table to watch");

            // Let it play in the background instead.
            sim.ReleaseFeatured();
            Assert.IsNull(sim.FeaturedMatch());
            sim.PlayAiTablesNow();
            Assert.IsTrue(sim.Finished);
        }

        [Test]
        public void Watching_Is_Only_For_AI_Only_Events()
        {
            var sim = MakeEvent(5, 2, false, true, 81, true);
            Assert.IsFalse(sim.Watching);
            Assert.IsNull(sim.FeaturedMatch());
        }

        // ================================================================== Saving

        [Test]
        public void An_Event_Is_Saved_And_Read_Back_As_It_Was()
        {
            var sim = RegionalSim.Create(6, 3, true, "Expanded", "Test Deck", new[] { "Deck A", "Deck B" },
                new Dictionary<string, string> { { "Test Deck", "4 Dreepy TWM 128\n# Format: Expanded" }, { "Deck A", "x\ty" }, { "Deck B", "back\\slash" } },
                true, 91);
            sim.Begin();
            AWinsEveryTable(sim, true);
            bool recorded = sim.RecordYourGame(1, 0, 1); // false if you have the bye in round 1
            string text = sim.Serialize();
            var back = RegionalSim.Parse(text);
            Assert.AreEqual(text, back.Serialize());
            Assert.AreEqual(3, back.Rounds);
            Assert.IsTrue(back.BestOfThree);
            Assert.AreEqual("Expanded", back.Format);
            Assert.IsTrue(back.YouPlaying);
            Assert.AreEqual("4 Dreepy TWM 128\n# Format: Expanded", back.DeckText("Test Deck"));
            Assert.AreEqual("x\ty", back.DeckText("Deck A"));
            Assert.AreEqual("back\\slash", back.DeckText("Deck B"));
            Assert.AreEqual(recorded ? 1 : 0, back.YourMatch().Games.Count, "your game 1 is kept");
            Assert.AreEqual(sim.Round, back.Round);
            Assert.Throws<System.FormatException>(() => RegionalSim.Parse("not an event"));
        }

        // ================================================================== Real games

        [Test]
        public void An_AI_Only_Event_Plays_Real_Games_And_Teaches_The_Brain()
        {
            var decks = BuiltInStandardDecks();
            Assert.GreaterOrEqual(decks.Count, 2);
            var names = decks.Take(2).Select(d => d.Name).ToList();
            var texts = decks.Take(2).ToDictionary(d => d.Name, d => d.Text);
            var sim = RegionalSim.Create(6, 2, false, "Standard", null, names, texts, true, 101);
            sim.Brain = new LearningBrain();
            sim.ScriptsFactory = StandardCards.CreateRegistry;
            foreach (var deck in decks.Take(2)) sim.SetDeckCards(deck.Name, DeckLibrary.Build(db, deck, new List<string>()));
            sim.Begin();
            sim.PlayAiTablesNow();
            var view = sim.View();
            Assert.IsTrue(view.Finished, view.LastError ?? "");
            Assert.AreEqual(0, view.AiErrors, view.LastError ?? "");
            Assert.AreEqual(6, view.AiGames, "3 tables in each of 2 rounds");
            Assert.AreEqual(6, sim.Brain.GamesTrained, "every AI game teaches the brain");
            Assert.IsTrue(view.Matches.All(x => x.Finished));
        }

        [Test]
        public void Your_Event_Plays_The_AI_Tables_In_The_Background_Best_Of_3()
        {
            var decks = BuiltInStandardDecks();
            var names = decks.Take(2).Select(d => d.Name).ToList();
            var texts = decks.Take(2).ToDictionary(d => d.Name, d => d.Text);
            var sim = RegionalSim.Create(5, 2, true, "Standard", names[0], names, texts, false, 111);
            sim.ScriptsFactory = StandardCards.CreateRegistry;
            foreach (var deck in decks.Take(2)) sim.SetDeckCards(deck.Name, DeckLibrary.Build(db, deck, new List<string>()));
            sim.Begin();
            sim.StartWorkers(2);
            try
            {
                for (int wait = 0; wait < 900 && sim.View().RoundFinished < 2; wait++) System.Threading.Thread.Sleep(100);
                var view = sim.View();
                Assert.AreEqual(2, view.RoundFinished, "both AI tables finished in the background: " + (view.LastError ?? ""));
                Assert.AreEqual(1, view.Round, "round 2 waits for your match");
                foreach (var aiTable in view.Matches.Where(x => x.Round == 1 && x.A != 0))
                    Assert.IsTrue(RegionalSim.Decided(aiTable, true), "best of 3: someone won 2 games, or 3 were played");
                sim.RecordYourGame(1, 0, -1);
                sim.RecordYourGame(1, 1, -1);
                Assert.AreEqual(2, sim.Round);
            }
            finally
            {
                sim.StopWorkers(5000);
            }
        }

        [Test]
        public void The_Loser_Of_The_Last_Game_Chooses_To_Go_First_Or_Second()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var flips = new FlipCounter();
            var engine = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent().Then("Go second"));
            engine.AddObserver(flips);
            engine.OrderChooser = 1;
            engine.Setup(deck, deck, 7);
            Assert.AreEqual(0, engine.State.FirstPlayer, "seat 1 chose to go second");
            Assert.AreEqual(0, flips.CoinFlips, "no opening coin flip");
        }

        private sealed class FlipCounter : IGameObserver
        {
            public int CoinFlips;

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                if (ev.Type == GameEventType.CoinFlip) CoinFlips++;
            }
        }
    }
}
