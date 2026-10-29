using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>The Neural AI's policy network (ported from the Python bot), its PPO training and how the AI styles use it.</summary>
    public class PolicyTests : ScenarioTestBase
    {
        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }
        private List<CardDef> Zoroark() { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); }

        /// <summary>Plays a game between two policy agents until the first decision with a few options, and returns it encoded.</summary>
        private PolicyInput SomeDecision(PolicyNet net)
        {
            PolicyInput found = null;
            var probe = new ProbeAgent(net, x => { if (found == null && x.Count >= 3) found = x; });
            var engine = new GameEngine(StandardCards.CreateRegistry(), probe, new HeuristicAgent(2));
            engine.Run(Dragapult(), Zoroark(), 11);
            Assert.NotNull(found, "a decision with 3 or more options");
            return found;
        }

        private sealed class ProbeAgent : IPlayerAgent, IGameObserver
        {
            private readonly PolicyAgent inner;
            private readonly Action<PolicyInput> seen;

            public ProbeAgent(PolicyNet net, Action<PolicyInput> seen)
            {
                inner = new PolicyAgent(net, 1) { Record = true };
                this.seen = seen;
            }

            public List<int> Decide(DecisionRequest r)
            {
                var picks = inner.Decide(r);
                foreach (var s in inner.Trajectory) seen(s.Input);
                inner.Trajectory.Clear();
                return picks;
            }

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                inner.OnEvent(engine, ev);
            }
        }

        [Test]
        public void Saving_And_Loading_The_Network_Keeps_Every_Weight()
        {
            var net = new PolicyNet { GamesTrained = 12 };
            var x = SomeDecision(net);
            var before = net.Forward(x);
            var copy = new PolicyNet();
            Assert.IsTrue(copy.FromBytes(net.ToBytes()));
            var after = copy.Forward(x);
            Assert.AreEqual(before.Value, after.Value, 1e-6);
            for (int i = 0; i < before.Logits.Length; i++) Assert.AreEqual(before.Logits[i], after.Logits[i], 1e-6);
            Assert.AreEqual(12, copy.GamesTrained);
            Assert.IsFalse(new PolicyNet().FromBytes(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }), "not a network");
        }

        [Test]
        public void Making_The_Network_Bigger_Keeps_What_It_Knows()
        {
            var net = new PolicyNet { GamesTrained = 40 };
            var x = SomeDecision(net);
            // Some learning first, so every layer holds something of its own.
            for (int i = 0; i < 5; i++) net.Imitate(new List<KeyValuePair<PolicyInput, int>> { new KeyValuePair<PolicyInput, int>(x, 0) }, 1f);
            var before = net.Forward(x);
            int revision = net.WeightsRevision;
            Assert.AreEqual(PolicyNet.DefaultBlocks, net.Blocks);
            net.Grow(PolicyNet.BigBlocks);
            Assert.AreEqual(PolicyNet.BigBlocks, net.Blocks);
            Assert.AreNotEqual(revision, net.WeightsRevision, "the GPU learner's copy is out of date");
            var after = net.Forward(x);
            Assert.AreEqual(before.Value, after.Value, 1e-6, "it plays exactly as before");
            for (int i = 0; i < before.Logits.Length; i++) Assert.AreEqual(before.Logits[i], after.Logits[i], 1e-6);
            Assert.AreEqual(5, net.Steps);
            Assert.AreEqual(40, net.GamesTrained);
            net.Grow(PolicyNet.DefaultBlocks);
            Assert.AreEqual(PolicyNet.BigBlocks, net.Blocks, "it never gets smaller");

            // Saved and loaded (into a network that starts at the usual size), it's the bigger one.
            var copy = new PolicyNet();
            Assert.IsTrue(copy.FromBytes(net.ToBytes()));
            Assert.AreEqual(PolicyNet.BigBlocks, copy.Blocks);
            Assert.AreEqual(after.Value, copy.Forward(x).Value, 1e-6);
            Assert.AreEqual(PolicyNet.BigBlocks, net.Clone().Blocks);
            // And a network of the usual size still loads into a bigger one.
            var small = new PolicyNet();
            var y = SomeDecision(small);
            var big = new PolicyNet(PolicyNet.BigBlocks);
            Assert.IsTrue(big.FromBytes(small.ToBytes()));
            Assert.AreEqual(PolicyNet.DefaultBlocks, big.Blocks);
            Assert.AreEqual(small.Forward(y).Value, big.Forward(y).Value, 1e-6);

            // It goes on learning, the new blocks too.
            int target = x.Count - 1;
            double p0 = PolicyNet.Softmax(net.Forward(x).Logits)[target];
            for (int i = 0; i < 10; i++) net.Imitate(new List<KeyValuePair<PolicyInput, int>> { new KeyValuePair<PolicyInput, int>(x, target) }, 1f);
            Assert.Greater(PolicyNet.Softmax(net.Forward(x).Logits)[target], p0);
            Assert.AreNotEqual(Dense(copy), Dense(net));
        }

        [Test]
        public void A_Trainer_Goes_On_Training_A_Network_Made_Bigger_Between_Rounds()
        {
            var net = new PolicyNet();
            var trainer = SmallTrainer(net);
            trainer.Round();
            long steps = net.Steps;
            Assert.Greater(steps, 0);
            net.Grow(PolicyNet.BigBlocks);
            var round = trainer.Round();
            trainer.Dispose();
            Assert.IsNull(round.LastError, round.LastError);
            Assert.Greater(net.Steps, steps, "it learned with the bigger network");
            Assert.IsTrue(net.AllFinite());
        }

        [Test]
        public void A_Learning_Step_Makes_The_Copied_Choice_More_Likely()
        {
            var net = new PolicyNet();
            var x = SomeDecision(net);
            int target = x.Count - 1;
            double before = PolicyNet.Softmax(net.Forward(x).Logits)[target];
            for (int i = 0; i < 20; i++) net.Imitate(new List<KeyValuePair<PolicyInput, int>> { new KeyValuePair<PolicyInput, int>(x, target) }, 1f);
            double after = PolicyNet.Softmax(net.Forward(x).Logits)[target];
            Assert.Greater(after, before);
            Assert.AreEqual(20, net.Steps);
        }

        [Test]
        public void Policy_Agents_Play_Full_Games_Without_Errors()
        {
            var net = new PolicyNet();
            var report = MatchRunner.Run(StandardCards.CreateRegistry(), Dragapult(), Zoroark(), 12, 6100,
                (seat, s) => seat == 0 ? (IPlayerAgent)new PolicyAgent(net, s) { Greedy = false } : new HeuristicAgent(s), true);
            Assert.AreEqual(0, report.Errors.Count, report.Summary());
        }

        [Test]
        public void The_Opponents_Hidden_Cards_Never_Reach_The_Network()
        {
            // Two games with the same seed and the same choices for player 1, but player 2's deck in another order: whatever
            // player 1 is shown can't depend on the order of cards it can't see... until they come into view. Compare the very
            // first decision (before anything of player 2's is revealed).
            PolicyInput first = null, second = null;
            var net = new PolicyNet();
            var deck = Zoroark();
            new GameEngine(StandardCards.CreateRegistry(), new ProbeAgent(net, x => { if (first == null) first = x; }), new HeuristicAgent(3)).Run(Dragapult(), deck, 21);
            var reversed = Enumerable.Reverse(deck).ToList();
            new GameEngine(StandardCards.CreateRegistry(), new ProbeAgent(net, x => { if (second == null) second = x; }), new HeuristicAgent(3)).Run(Dragapult(), reversed, 21);
            Assert.NotNull(first);
            Assert.NotNull(second);
            CollectionAssert.AreEqual(first.Bags[PolicyInput.BagOppKnownHand], second.Bags[PolicyInput.BagOppKnownHand]);
            CollectionAssert.AreEqual(first.Bags[PolicyInput.BagOppSeen], second.Bags[PolicyInput.BagOppSeen]);
            CollectionAssert.AreEqual(first.SlotCard.Skip(PolicyInput.SideSlots).ToArray(), second.SlotCard.Skip(PolicyInput.SideSlots).ToArray());
        }

        [Test]
        public void A_Fresh_Network_Leaves_Every_Style_Playing_As_Before()
        {
            var brain = new LearningBrain();
            if (LearningBrain.PolicyStart == null) Assert.IsFalse(brain.Policy.Trained, "no starting network loaded: a fresh one");
            brain.Policy.Reset();
            Assert.IsFalse(brain.Policy.Trained);
            var classic = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(7), new HeuristicAgent(8)).Run(Dragapult(), Zoroark(), 99);
            var neural = new GameEngine(StandardCards.CreateRegistry(), new LearningAgent(brain, 7, "A", "B") { Style = AiStyle.Neural },
                new LearningAgent(brain, 8, "B", "A") { Style = AiStyle.Neural }).Run(Dragapult(), Zoroark(), 99);
            Assert.AreEqual(classic.Winner, neural.Winner);
            Assert.AreEqual(classic.Turns, neural.Turns);
        }

        [Test]
        public void A_Neural_Training_Session_Trains_The_Network_By_Self_Play()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1; // counts as trained, so the session uses it
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 10, 2, 4242)
            {
                Style = AiStyle.Neural
            };
            session.RunBlocking();
            var st = session.Status();
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(10, st.Done);
            Assert.AreEqual(10, st.WinsA + st.WinsB + st.Draws);
            Assert.Greater(brain.Policy.Steps, 0, "the network learned");
            int explored = st.Blocks.Sum(b => b.Explored), decisions = st.Blocks.Sum(b => b.Decisions);
            Assert.Greater(explored, 0, "it tries moves other than its favourite while training (the Train screen's \"tried something new\")");
            Assert.Less(explored, decisions);
            // Its games are on the matchup list (both sides of a self-play game, and the deck names without the mirror's space).
            var matchups = brain.Matchups();
            Assert.GreaterOrEqual(matchups.Sum(m => m.Games), 10);
            CollectionAssert.IsSubsetOf(matchups.Select(m => m.Deck).Distinct(), new[] { "Dragapult", "Zoroark" });
            Assert.AreEqual(11, brain.Policy.GamesTrained);
            Assert.AreEqual(10, brain.GamesTrained);

            // Undo this session: the network goes back too.
            brain.LoadText(session.BrainBefore);
            Assert.AreEqual(0, brain.Policy.Steps);
            Assert.AreEqual(1, brain.Policy.GamesTrained);
        }

        [Test]
        public void The_Champion_Is_Kept_Checked_And_Replaced_Only_By_A_Clearly_Better_Network()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()), new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark())
            };
            var scripts = StandardCards.CreateRegistry();

            // The first check: the network as it is becomes version 1 (nothing played).
            var first = ChampionGate.Run(brain, scripts, decks, 20, 2, null);
            Assert.AreEqual(1, brain.ChampionVersion);
            Assert.IsTrue(first.Promoted);
            Assert.AreEqual(0, first.Games);
            CollectionAssert.AreEqual(brain.Policy.ToBytes(), brain.Champion.ToBytes(), "a copy of the network");
            StringAssert.Contains("first champion (version 1)", ChampionGate.Describe(first));

            // Later checks play the same games each time; it takes over only at 55% or more.
            var check = ChampionGate.Run(brain, scripts, decks, 20, 2, null);
            Assert.AreEqual(20, check.Games);
            Assert.AreEqual(check.WinRate >= ChampionGate.PromoteAt, check.Promoted);
            Assert.AreEqual(check.Promoted ? 2 : 1, brain.ChampionVersion);
            var again = ChampionGate.Run(brain, scripts, decks, 20, 2, null);
            if (!check.Promoted) Assert.AreEqual(check.WinRate, again.WinRate, 1e-9, "the same games, the same result");
            Assert.AreEqual(3, brain.ChampionChecks.Count);

            // Saved and read back, champion network included; Undo brings it back; a reset forgets it.
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "champion_test_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                brain.FilePath = path;
                brain.Save();
                var back = LearningBrain.Load(path);
                Assert.AreEqual(brain.ChampionVersion, back.ChampionVersion);
                Assert.AreEqual(3, back.ChampionChecks.Count);
                Assert.AreEqual(check.WinRate, back.ChampionChecks[1].WinRate, 1e-6);
                CollectionAssert.AreEqual(brain.Champion.ToBytes(), back.Champion.ToBytes());

                string before = brain.Serialize();
                var champ = brain.Champion.ToBytes();
                brain.Reset();
                Assert.IsNull(brain.Champion);
                Assert.AreEqual(0, brain.ChampionVersion);
                brain.Save();
                Assert.IsFalse(System.IO.File.Exists(LearningBrain.ChampionPath(path)), "no champion file after a reset");
                brain.LoadText(before);
                CollectionAssert.AreEqual(champ, brain.Champion.ToBytes(), "Undo keeps the champion as it was");
            }
            finally
            {
                foreach (var f in new[] { path, LearningBrain.NeuralPath(path), LearningBrain.ChampionPath(path) })
                    if (System.IO.File.Exists(f)) System.IO.File.Delete(f);
            }
        }

        [Test]
        public void A_Neural_Training_Session_Checks_Its_Champion()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 24, 0, 777)
            {
                Style = AiStyle.Neural,
                ChampionEvery = 12,
                ChampionMinGames = 8,
                ChampionGames = 10
            };
            session.RunBlocking();
            var checks = brain.ChampionChecks;
            Assert.GreaterOrEqual(checks.Count, 2, "the first champion, then a check during the session");
            Assert.AreEqual(-1, checks[0].WinRate, "the first is the network as it was");
            Assert.IsTrue(checks.Skip(1).All(c => c.Games == 10));
            StringAssert.Contains("champion", session.ChampionNote);
        }

        [Test]
        public void The_League_Picks_The_Past_Versions_It_Does_Worst_Against_Most_And_Keeps_The_Hardest()
        {
            var league = new League();
            var easy = league.Add(new PolicyNet(), "snapshot", "easy", 100, "");
            var hard = league.Add(new PolicyNet(), "champion", "hard", 200, "");
            for (int i = 0; i < 100; i++)
            {
                league.Record(easy.Id, 1); // the network always beats it
                league.Record(hard.Id, i % 4 == 0 ? 1 : 0); // and mostly loses to this one
            }
            var members = league.Members();
            Assert.Greater(members[0].WinRate, 0.9);
            Assert.Less(members[1].WinRate, 0.35);
            var rng = new Rng(3);
            int hardPicks = 0;
            for (int i = 0; i < 1000; i++) if (league.Pick(rng).Id == hard.Id) hardPicks++;
            Assert.Greater(hardPicks, 850, "the hard one is picked most");
            Assert.Less(hardPicks, 1000, "the easy one still now and then");

            // Full: the easiest leaves (once it has played enough), never the one joining.
            for (int i = 0; i < League.Capacity - 1; i++) league.Add(new PolicyNet(), "snapshot", "new " + i, 300 + i, "");
            Assert.AreEqual(League.Capacity, league.Count);
            Assert.IsFalse(league.Members().Any(m => m.Name == "easy"), "the one it beats most easily left");
            Assert.IsTrue(league.Members().Any(m => m.Name == "hard"));
            Assert.IsTrue(league.Members().Any(m => m.Name == "new " + (League.Capacity - 2)));
            Assert.IsTrue(league.Events().Any(e => e.Contains("easy left")));
            Assert.AreEqual(300 + League.Capacity - 2, league.NewestAddedAt);
        }

        [Test]
        public void The_League_Is_Saved_With_The_AI_Undone_With_A_Session_And_Forgotten_On_Reset()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            var a = new PolicyNet { GamesTrained = 11 };
            var b = new PolicyNet(PolicyNet.BigBlocks) { GamesTrained = 22 };
            var ma = brain.League.Add(a, "champion", "champion v1", 1000, "2026-01-01 10:00");
            brain.League.Record(ma.Id, 1);
            brain.League.LastExploiterAt = 1000;
            brain.League.ExploitersTried = 2;
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "league_test_" + Guid.NewGuid().ToString("N") + ".txt");
            try
            {
                brain.FilePath = path;
                string before = brain.Serialize();
                brain.League.Add(b, "exploiter", "exploiter 2", 2000, "");
                brain.Save();
                Assert.AreEqual(2, System.IO.Directory.GetFiles(LearningBrain.LeaguePath(path)).Length, "a file per member");

                var back = LearningBrain.Load(path);
                var members = back.League.Members();
                Assert.AreEqual(2, members.Count);
                Assert.AreEqual("champion v1", members[0].Name);
                Assert.AreEqual(1000, members[0].AddedAt);
                Assert.AreEqual(1, members[0].Games, 1e-9);
                Assert.AreEqual(11, members[0].Net.GamesTrained);
                Assert.AreEqual(PolicyNet.BigBlocks, members[1].Net.Blocks);
                Assert.AreEqual(2, back.League.ExploitersTried);
                Assert.AreEqual(1000, back.League.LastExploiterAt);
                Assert.IsTrue(back.League.Events().Any(e => e.Contains("champion v1 joined")));

                // Undo this session: the member it added leaves, and its file goes with the next save.
                brain.LoadText(before);
                Assert.AreEqual(1, brain.League.Count);
                Assert.AreSame(a, brain.League.Members()[0].Net, "the network it remembers");
                brain.Save();
                Assert.AreEqual(1, System.IO.Directory.GetFiles(LearningBrain.LeaguePath(path)).Length);

                brain.Reset();
                Assert.AreEqual(0, brain.League.Count);
                brain.Save();
                Assert.IsFalse(System.IO.Directory.Exists(LearningBrain.LeaguePath(path)), "no league after a reset");
                // A member added after the reset never takes an old one's file.
                Assert.Greater(brain.League.Add(new PolicyNet(), "snapshot", "after", 1, "").Id, ma.Id + 1);
            }
            finally
            {
                foreach (var f in new[] { path, LearningBrain.NeuralPath(path), LearningBrain.ChampionPath(path) })
                    if (System.IO.File.Exists(f)) System.IO.File.Delete(f);
                if (System.IO.Directory.Exists(LearningBrain.LeaguePath(path))) System.IO.Directory.Delete(LearningBrain.LeaguePath(path), true);
            }
        }

        [Test]
        public void Training_Plays_The_League_And_Keeps_Its_Results()
        {
            var net = new PolicyNet();
            var league = new League();
            var past = league.Add(new PolicyNet(), "snapshot", "past", 0, "");
            var trainer = SmallTrainer(net);
            trainer.League = league;
            trainer.SelfShare = 0;
            trainer.PoolShare = 1;
            trainer.ClassicShare = 0;
            var round = trainer.Round();
            trainer.Dispose();
            Assert.AreEqual(round.Games, round.VsLeague, "every game against the league (no snapshots yet)");
            Assert.Greater(round.VsLeague, 0);
            var m = league.Members()[0];
            Assert.AreEqual(round.VsLeague, m.Games, 0.1);
            Assert.AreEqual(round.LeaguePoints, m.Points, 0.1);
        }

        [Test]
        public void Exploiters_Train_To_Beat_The_Network_And_Join_The_League_Only_When_They_Do()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 72, 0, 778)
            {
                // Two rounds (64 games, then 8): the exploiter comes between them.
                Style = AiStyle.Neural,
                ChampionEvery = 8,
                ChampionMinGames = 8,
                ChampionGames = 10,
                ExploiterEvery = 8,
                ExploiterGames = 8
            };
            session.RunBlocking();
            Assert.GreaterOrEqual(brain.League.ExploitersTried, 1);
            Assert.IsTrue(brain.League.Events().Any(e => e.Contains("exploiter 1 won")), string.Join(" | ", brain.League.Events()));
            bool kept = brain.League.Members().Any(x => x.Kind == "exploiter");
            Assert.AreEqual(kept, brain.League.Events().Any(e => e.Contains("found a weakness")));
            Assert.IsTrue(brain.League.Members().Any(x => x.Kind == "champion"), "the league starts with the champion");
            Assert.AreEqual(72, brain.GamesTrained, "the exploiter's games aren't the network's training");
            StringAssert.Contains("exploiter", session.LeagueNote);
        }

        [Test]
        public void A_Network_With_Broken_Numbers_Is_Never_Loaded_Or_Learned_Into()
        {
            var net = new PolicyNet { GamesTrained = 5 };
            var x = SomeDecision(net);
            var good = net.ToBytes();

            // A learning step whose gradients aren't finite is skipped: the weights stay as they were.
            var before = net.Forward(x).Logits[0];
            var g = net.NewGrad();
            var pass = net.Forward(x);
            var bad = new float[x.Count];
            bad[0] = float.NaN;
            net.Backward(x, pass, bad, 0f, g);
            net.Apply(g, 1);
            Assert.AreEqual(1, net.SkippedSteps);
            Assert.AreEqual(before, net.Forward(x).Logits[0]);
            Assert.IsTrue(net.AllFinite());

            // A file holding a NaN (saved by an older version after a bad step) isn't loaded: the network is left alone.
            // The first weight comes after the header: "PTCGNET1", version, steps, games, loss, origin (""), count, the first
            // name ("cards.proj.weight") and its length.
            int first = 8 + 4 + 8 + 8 + 8 + 1 + 4 + 1 + "cards.proj.weight".Length + 4;
            var damaged = (byte[])good.Clone();
            Array.Copy(BitConverter.GetBytes(float.NaN), 0, damaged, first, 4);
            var other = new PolicyNet();
            Assert.IsTrue(other.FromBytes(good));
            Assert.IsFalse(other.FromBytes(damaged), "a damaged network isn't loaded");
            Assert.IsTrue(other.AllFinite());
            Assert.AreEqual(net.Forward(x).Logits[0], other.Forward(x).Logits[0], 1e-6, "still the good one");
        }

        [Test]
        public void Stopping_A_Neural_Training_Session_Stops_It_Quickly()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 100000, 0, 99)
            {
                Style = AiStyle.Neural
            };
            session.Start();
            var clock = System.Diagnostics.Stopwatch.StartNew();
            while (session.Status().Done < 2 && clock.Elapsed.TotalSeconds < 60) System.Threading.Thread.Sleep(50);
            long steps = brain.Policy.Steps;
            clock.Restart();
            session.Cancel();
            session.Join(30000);
            var st = session.Status();
            Assert.IsTrue(st.Finished, "stopped");
            Assert.IsTrue(st.Cancelled);
            Assert.Less(clock.Elapsed.TotalSeconds, 15, "games in progress are abandoned and the unfinished batch isn't learned from");
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(steps, brain.Policy.Steps, "no learning step after Stop");
        }

        [Test]
        public void The_Trainer_Learns_From_Games_Against_Itself_Its_Past_And_The_Classic_Bot()
        {
            var net = new PolicyNet();
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()),
                new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark())
            };
            var trainer = new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, 5) { GamesPerRound = 6, SnapshotEvery = 1, Threads = 2, Minibatch = 256 };
            var first = trainer.Round();
            var second = trainer.Round();
            Assert.AreEqual(0, first.Errors + second.Errors, first.LastError ?? second.LastError);
            Assert.AreEqual(12, first.Games + second.Games);
            Assert.Greater(first.Decisions, 0);
            Assert.Greater(net.Steps, 0);
            Assert.IsFalse(double.IsNaN(second.PolicyLoss) || double.IsNaN(second.ValueLoss));
        }

        [Test]
        public void Training_Can_Learn_From_The_Look_Ahead()
        {
            var net = new PolicyNet();
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()),
                new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark())
            };
            var trainer = new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, 9)
            {
                GamesPerRound = 4, Threads = 2, Minibatch = 256, SearchShare = 1, SearchDecisions = 1,
                TrainingSearch = new SearchSettings { Playouts = 4, MaxCandidates = 2, SkipAbove = 1.01, Threads = 1, MaxSeconds = 1 }
            };
            var before = net.ToBytes();
            var round = trainer.Round();
            trainer.Dispose();
            Assert.AreEqual(0, round.Errors, round.LastError);
            Assert.Greater(round.Searched, 0, "some decisions were made by the look-ahead");
            Assert.Less(round.Searched, round.Decisions, "the others are the network's own (only main actions look ahead)");
            Assert.Greater(round.DistillLoss, 0, "how far the network was from the look-ahead's choices");
            Assert.IsFalse(double.IsNaN(round.PolicyLoss) || double.IsNaN(round.ValueLoss) || double.IsNaN(round.DistillLoss));
            CollectionAssert.AreNotEqual(before, net.ToBytes(), "it learned");
            Assert.IsTrue(net.AllFinite());
        }

        [Test]
        public void The_Look_Ahead_Gives_An_Improved_Choice_To_Learn_From()
        {
            var net = new PolicyNet();
            var search = new PolicySearch(net, new SearchSettings { Playouts = 8, MaxCandidates = 3, SkipAbove = 1.01, Threads = 1, MaxSeconds = 2 });
            search.KnownDecks.Add(Dragapult());
            search.KnownDecks.Add(Zoroark());
            var probe = new TargetProbe(net, search);
            new GameEngine(StandardCards.CreateRegistry(), probe, new HeuristicAgent(4)) { MaxTurns = 8 }.Run(Dragapult(), Zoroark(), 51);
            Assert.Greater(probe.Targets, 0);
            Assert.AreEqual(0, probe.Bad, "every target is a probability over the same options");
        }

        /// <summary>Plays with the network and, at each main action, asks the look-ahead for its improved choice.</summary>
        private sealed class TargetProbe : IPlayerAgent, IGameObserver
        {
            private readonly PolicyAgent inner;
            private readonly PolicySearch search;
            private GameEngine engine;
            public int Targets, Bad;

            public TargetProbe(PolicyNet net, PolicySearch search)
            {
                inner = new PolicyAgent(net, 3, 0);
                this.search = search;
            }

            public List<int> Decide(DecisionRequest r)
            {
                if (r.Kind == DecisionKind.MainAction && engine != null)
                {
                    inner.Memory.See(r.View);
                    var choices = PolicyEncoder.Choices(r, new List<int>());
                    if (choices.Count >= 2)
                    {
                        float v;
                        var p = PolicyNet.Softmax(inner.Net.Evaluate(PolicyEncoder.Encode(r, inner.Memory, inner.Net, choices), out v));
                        int top = 0;
                        for (int i = 1; i < p.Length; i++) if (p[i] > p[top]) top = i;
                        double[] target;
                        search.Pick(engine, r, inner.Memory, choices, p, top, out target);
                        if (target != null)
                        {
                            Targets++;
                            if (target.Length != choices.Count || Math.Abs(target.Sum() - 1) > 1e-6 || target.Any(t => t < 0 || double.IsNaN(t))) Bad++;
                        }
                    }
                }
                return inner.Decide(r);
            }

            public void OnEvent(GameEngine e, GameEvent ev)
            {
                engine = e;
                inner.OnEvent(e, ev);
            }
        }

        private List<KeyValuePair<string, List<CardDef>>> ThreeDecks()
        {
            return new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()),
                new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark()),
                new KeyValuePair<string, List<CardDef>>("Alakazam", UnityCardData.LoadDeck(db, "Decklists/alakazam_dusknoir"))
            };
        }

        [Test]
        public void All_Decks_Training_Plays_A_Random_Pair_Every_Game([Values(AiStyle.Learning, AiStyle.Neural)] AiStyle style)
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1; // counts as trained, so the Neural AI session uses it
            var decks = ThreeDecks();
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "All decks", decks[0].Value, "All decks", decks[0].Value, 12, 4, 5150)
            {
                Style = style,
                DeckPool = decks
            };
            StringAssert.Contains("all 3 decks", session.MatchLabel);
            session.RunBlocking();
            var st = session.Status();
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(12, st.Done);
            Assert.AreEqual(12, st.WinsA + st.WinsB + st.Draws);
            Assert.GreaterOrEqual(st.ScoreBefore, 0);
            Assert.GreaterOrEqual(st.ScoreAfter, 0);
            // Every style's games go into the matchup table: more than one pair was played, every game counted at least once.
            var matchups = brain.Matchups();
            Assert.Greater(matchups.Count, 1);
            Assert.GreaterOrEqual(matchups.Sum(m => m.Games), 12);
            Assert.IsTrue(matchups.All(m => m.Wins + m.Losses <= m.Games && decks.Any(d => d.Key == m.Deck) && decks.Any(d => d.Key == m.Opponent)));
            if (style == AiStyle.Neural) Assert.Greater(brain.Policy.Steps, 0);
        }

        [Test]
        public void Focused_Practice_Plays_One_Deck_Every_Game_Against_Random_Decks(
            [Values(AiStyle.Learning, AiStyle.Neural)] AiStyle style, [Values(true, false)] bool focusOnA)
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var decks = ThreeDecks();
            var focus = Zoroark();
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain,
                focusOnA ? "Zoroark" : "Random deck", focusOnA ? focus : decks[0].Value,
                focusOnA ? "Random deck" : "Zoroark", focusOnA ? decks[0].Value : focus, 12, 4, 6060)
            {
                Style = style,
                DeckPool = decks,
                RandomA = !focusOnA,
                RandomB = focusOnA
            };
            Assert.IsTrue(session.FocusMode);
            Assert.IsFalse(session.AllDecksMode);
            Assert.AreEqual("Zoroark", session.FocusDeck);
            StringAssert.StartsWith("Zoroark vs a random deck", session.MatchLabel);
            session.RunBlocking();
            var st = session.Status();
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(12, st.WinsA + st.WinsB + st.Draws);
            Assert.GreaterOrEqual(st.ScoreBefore, 0);
            // Zoroark played every game (it's on every matchup row as the deck), against more than one deck.
            var asDeck = brain.Matchups().Where(m => m.Deck == "Zoroark").ToList();
            Assert.GreaterOrEqual(asDeck.Sum(m => m.Games), 12);
            Assert.Greater(asDeck.Select(m => m.Opponent).Distinct().Count(), 1);
            Assert.IsTrue(brain.Matchups().All(m => m.Deck == "Zoroark" || m.Opponent == "Zoroark"), "every game had Zoroark in it");
        }

        [Test]
        public void Neural_Training_Games_Teach_The_Deck_Advisor()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            brain.Policy.GamesTrained = 1;
            var decks = ThreeDecks();
            new TrainingSession(StandardCards.CreateRegistry(), brain, "All decks", decks[0].Value, "All decks", decks[0].Value, 12, 0, 7171)
            {
                Style = AiStyle.Neural,
                DeckPool = decks
            }.RunBlocking();

            var skills = brain.DeckSkills();
            Assert.Greater(skills.Count, 1);
            Assert.GreaterOrEqual(skills.Sum(k => k.Games), 12, "every game, both sides in self-play");
            foreach (var k in skills)
            {
                Assert.IsTrue(decks.Any(d => d.Key == k.Deck), k.Deck);
                Assert.Greater(k.Decisions, 0);
                Assert.That(k.Uncertainty, Is.GreaterThan(0).And.LessThan(5));
                Assert.AreNotEqual("not played yet", k.Level);
            }
            var cards = brain.CardResultsOf(skills[0].Deck);
            Assert.Greater(cards.Count, 5);
            Assert.IsTrue(cards.All(r => r.Games == skills[0].Games && r.Drawn <= r.Games && r.Wins <= r.Games && r.DrawnWins <= r.Drawn));
            Assert.IsTrue(cards.Any(r => r.Drawn > 0) && cards.Any(r => r.Played > 0));
            Assert.IsFalse(cards.Any(r => r.Card.EndsWith(" Energy") && decks.First(d => d.Key == skills[0].Deck).Value.Any(c => c.Name == r.Card && c.IsBasicEnergy)),
                "Basic Energy isn't rated");

            // Kept in the brain's file, and gone after a reset.
            var copy = new LearningBrain();
            copy.LoadText(brain.Serialize());
            Assert.AreEqual(skills.Sum(k => k.Games), copy.DeckSkills().Sum(k => k.Games));
            Assert.AreEqual(skills[0].Uncertainty, copy.SkillOf(skills[0].Deck).Uncertainty, 1e-6);
            Assert.AreEqual(cards.Sum(r => r.Drawn), copy.CardResultsOf(skills[0].Deck).Sum(r => r.Drawn));
            brain.Reset();
            Assert.AreEqual(0, brain.DeckSkills().Count);
            Assert.AreEqual(0, brain.CardResultsOf(null).Count);
        }

        [Test]
        public void The_Deck_Advisor_Rates_Cards_By_How_Often_The_Deck_Wins_When_They_Show_Up()
        {
            var brain = new LearningBrain();
            var dragapult = Dragapult();
            var trainers = dragapult.Where(c => c.IsTrainer).Select(c => c.Name).Distinct().ToList();
            string good = trainers[0], bad = trainers[1];
            // A variant of the Dragapult list (similar, not the same), trained 300 games: it wins much more often when it draws
            // the first card, and less often when it draws the second.
            var variant = dragapult.Take(58).Concat(dragapult.Where(c => c.IsBasicEnergy).Take(2)).ToList();
            for (int i = 0; i < 300; i++)
            {
                bool drewGood = i % 2 == 0, drewBad = i % 3 == 0;
                bool won = drewBad ? i % 4 == 0 : drewGood ? i % 5 != 0 : i % 10 < 4;
                var drawn = new HashSet<string>();
                if (drewGood) drawn.Add(good);
                if (drewBad) drawn.Add(bad);
                brain.RecordDeckGame("My Dragapult", variant, drawn, drawn, won ? 1 : -1, 80, 80 * 0.6);
                brain.RecordDeckGame("Dragapult", dragapult, new HashSet<string>(), new HashSet<string>(), i % 5 < 3 ? 1 : -1, 80, 80 * 0.7);
            }
            var results = brain.CardResultsOf("My Dragapult");
            Assert.Greater(results.Single(r => r.Card == good).Lift, 0.2);
            Assert.Less(results.Single(r => r.Card == bad).Lift, -0.1);
            var opinions = brain.CardOpinions("My Dragapult", null);
            Assert.Greater(opinions[good].Value, 0.3);
            Assert.Less(opinions[bad].Value, -0.2);
            Assert.AreEqual("well learned", brain.SkillOf("My Dragapult").Level);

            var library = new List<AdvisorDeck> { new AdvisorDeck("Dragapult", dragapult), new AdvisorDeck("Zoroark", Zoroark()) };
            var advice = new DeckAdvisor(db, DeckFormat.Standard, library, brain).SuggestChanges("My Dragapult", variant, 4);
            string notes = string.Join("\n", advice.Notes.ToArray());
            StringAssert.Contains("Neural AI training with this deck: 300 games", notes);
            StringAssert.Contains("wins more often when it draws " + good, notes);
            StringAssert.Contains("less often when it draws " + bad, notes);
            StringAssert.Contains("the AI wins 60% with it", notes, "the similar library deck it has trained with enough");
        }

        /// <summary>
        /// A stand-in for learner.py choosing moves: answers SETW and CARD with OKW1 and INF1 with option scores that prefer the last
        /// option, and refuses TRN1 (so the round learns on the CPU). Records the kinds of message it got.
        /// </summary>
        private sealed class FakeMoveLearner
        {
            public readonly List<string> Kinds = new List<string>();
            public int Port;
            private readonly System.Net.Sockets.TcpListener listener;
            private readonly bool fail;

            public FakeMoveLearner(bool fail)
            {
                this.fail = fail;
                listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
                listener.Start();
                Port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
                new System.Threading.Thread(Serve) { IsBackground = true }.Start();
            }

            private void Serve()
            {
                try
                {
                    using (var client = listener.AcceptTcpClient())
                    using (var stream = client.GetStream())
                    {
                        var r = new System.IO.BinaryReader(stream);
                        var w = new System.IO.BinaryWriter(stream);
                        while (true)
                        {
                            string kind = System.Text.Encoding.ASCII.GetString(r.ReadBytes(4));
                            if (kind.Length < 4) return;
                            var body = new System.IO.BinaryReader(new System.IO.MemoryStream(r.ReadBytes((int)r.ReadInt64())));
                            lock (Kinds) Kinds.Add(kind);
                            var reply = new System.IO.MemoryStream();
                            string answer;
                            using (var o = new System.IO.BinaryWriter(reply, System.Text.Encoding.UTF8, true))
                            {
                                if (fail || kind == "TRN1")
                                {
                                    answer = "ERR1";
                                    GpuLearner.WriteString(o, "not this time");
                                }
                                else if (kind == "INF1")
                                {
                                    answer = "OUT1";
                                    body.ReadInt32(); // version
                                    int n = body.ReadInt32(), slots = body.ReadInt32(), slotFeat = body.ReadInt32(), glob = body.ReadInt32(),
                                        turn = body.ReadInt32(), optFeat = body.ReadInt32(), bags = body.ReadInt32();
                                    body.ReadBytes(4 * n * slots * 2 + 4 * n * slots * slotFeat);
                                    for (int b = 0; b < bags; b++)
                                    {
                                        long sum = 0;
                                        for (int i = 0; i < n; i++) sum += body.ReadInt32();
                                        body.ReadBytes((int)(4 * sum));
                                    }
                                    body.ReadBytes(4 * n * 2 + 4 * n * glob + 4 * n * turn);
                                    var counts = new int[n];
                                    for (int i = 0; i < n; i++) counts[i] = body.ReadInt32();
                                    for (int i = 0; i < n; i++)
                                    {
                                        for (int k = 0; k < counts[i]; k++) o.Write(k == counts[i] - 1 ? 5f : 0f);
                                        o.Write(0.25f);
                                    }
                                }
                                else answer = "OKW1";
                            }
                            var head = new byte[12];
                            System.Text.Encoding.ASCII.GetBytes(answer, 0, 4, head, 0);
                            BitConverter.GetBytes(reply.Length).CopyTo(head, 4);
                            w.Write(head);
                            w.Write(reply.ToArray());
                            w.Flush();
                        }
                    }
                }
                catch (Exception)
                {
                    // the trainer went away
                }
                finally
                {
                    listener.Stop();
                }
            }
        }

        [Test]
        public void Moves_Can_Be_Chosen_On_The_GPU_Learner_With_Many_Games_At_Once()
        {
            var fake = new FakeMoveLearner(false);
            var net = new PolicyNet();
            var trainer = SmallTrainer(net);
            trainer.GamesPerRound = 8;
            trainer.Gpu = new GpuLearner("127.0.0.1", fake.Port);
            trainer.GpuMoves = true;
            var round = trainer.Round();
            Assert.AreEqual(0, round.Errors, round.LastError);
            Assert.AreEqual(8, round.Games);
            Assert.AreEqual(8, round.GamesAtOnce, "4 games per thread at once (all 8 of this round)");
            Assert.Greater(round.GpuDecisions, 0, "the learner chose moves");
            Assert.Greater(trainer.Moves.Batches, 0);
            Assert.Less(trainer.Moves.Batches, round.GpuDecisions, "several decisions in a batch");
            List<string> kinds;
            lock (fake.Kinds) kinds = new List<string>(fake.Kinds);
            Assert.AreEqual("SETW", kinds[0], "the network goes to the learner first");
            Assert.Contains("INF1", kinds);
            Assert.AreEqual(1, kinds.Count(k => k == "SETW"), "then only new cards (CARD), until the weights change");
            Assert.AreEqual("TRN1", kinds.Last(), "and the round's learning (refused here, so it learned on the CPU)");
            Assert.Greater(net.Steps, 0);
            trainer.Dispose();
        }

        [Test]
        public void When_The_GPU_Learner_Cant_Choose_Moves_The_Games_Go_On_On_The_CPU()
        {
            var fake = new FakeMoveLearner(true);
            var net = new PolicyNet();
            var trainer = SmallTrainer(net);
            trainer.Gpu = new GpuLearner("127.0.0.1", fake.Port);
            trainer.GpuMoves = true;
            var round = trainer.Round();
            Assert.AreEqual(0, round.Errors, round.LastError);
            Assert.AreEqual(4, round.Games, "the games finished anyway");
            StringAssert.Contains("GPU learner", round.LastError);
            Assert.Greater(net.Steps, 0, "and it learned on the CPU");
            trainer.Dispose();
        }

        [Test]
        public void Stopping_While_The_GPU_Learner_Is_Used_Leaves_No_Error()
        {
            // v171: Stop closes the learner's connection on purpose; what was in flight then failed and showed as the session's error.
            var fake = new FakeMoveLearner(false);
            var net = new PolicyNet();
            var trainer = SmallTrainer(net);
            var gpu = new GpuLearner("127.0.0.1", fake.Port);
            trainer.Gpu = gpu;
            trainer.GpuMoves = true;
            gpu.Abort(); // as Stop does
            var round = trainer.Round();
            trainer.Dispose();
            Assert.AreEqual(0, round.Errors);
            Assert.IsNull(round.LastError, "nothing failed: it was stopped");
            Assert.AreEqual(0, net.Steps, "and it doesn't hold up the stop by learning on the CPU instead");
        }

        [Test]
        public void A_Network_File_That_Cant_Be_Read_Is_Never_Saved_Over()
        {
            // v171: an out-of-date trainer couldn't read a bigger network, started from the 3-block start and saved that over it.
            string path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "unreadable_test_" + Guid.NewGuid().ToString("N") + ".txt");
            string neural = LearningBrain.NeuralPath(path);
            try
            {
                var brain = new LearningBrain { FilePath = path };
                brain.Policy.Grow(PolicyNet.BigBlocks);
                brain.Save();
                var back = LearningBrain.Load(path);
                Assert.IsNull(back.PolicyLoadError);
                Assert.AreEqual(PolicyNet.BigBlocks, back.Policy.Blocks, "a bigger network comes back as big");

                // A file this code can't read (a later format version stands in for a network an older program doesn't know).
                var bytes = System.IO.File.ReadAllBytes(neural);
                BitConverter.GetBytes(99).CopyTo(bytes, 8);
                System.IO.File.WriteAllBytes(neural, bytes);
                var stuck = LearningBrain.Load(path);
                Assert.IsNotNull(stuck.PolicyLoadError);
                Assert.IsTrue(System.IO.File.Exists(neural + ".unreadable"), "a copy is kept");
                stuck.Save();
                CollectionAssert.AreEqual(bytes, System.IO.File.ReadAllBytes(neural), "saving leaves the network's file alone");

                stuck.Reset(); // starting again on purpose may replace it
                Assert.IsNull(stuck.PolicyLoadError);
                stuck.Save();
                Assert.IsNull(LearningBrain.Load(path).PolicyLoadError);
            }
            finally
            {
                foreach (var f in new[] { path, neural, neural + ".unreadable", LearningBrain.ChampionPath(path) })
                    if (f != null && System.IO.File.Exists(f)) System.IO.File.Delete(f);
            }
        }

        private PolicyTrainer SmallTrainer(PolicyNet net)
        {
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()),
                new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark())
            };
            return new PolicyTrainer(net, StandardCards.CreateRegistry(), decks, 12) { GamesPerRound = 4, Threads = 2, Minibatch = 256 };
        }

        [Test]
        public void Without_A_GPU_Learner_A_Round_Learns_On_The_CPU()
        {
            var net = new PolicyNet();
            var trainer = SmallTrainer(net);
            // Nothing listens on this port.
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            trainer.Gpu = new GpuLearner("127.0.0.1", port);
            var round = trainer.Round();
            trainer.Dispose();
            Assert.AreEqual(0, round.Errors);
            StringAssert.Contains("GPU learner", round.LastError);
            StringAssert.Contains("learned on the CPU", round.LastError);
            Assert.AreEqual("", round.LearnedOn);
            Assert.Greater(net.Steps, 0, "it learned anyway");
        }

        [Test]
        public void A_Round_Can_Learn_On_The_GPU_Learner()
        {
            var net = new PolicyNet();
            // Stands in for the learner's new weights: a network that took a round of learning (different numbers, same shapes).
            var learned = new PolicyNet();
            var other = SmallTrainer(learned);
            other.Round();
            other.Dispose();
            Assert.AreNotEqual(Dense(net), Dense(learned));
            var listener = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
            listener.Start();
            int port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
            int decisions = -1;
            string kind = null;
            // A stand-in for learner.py: reads one TRN1 message and answers with stats and the "learned" weights.
            var fake = new System.Threading.Thread(() =>
            {
                using (var client = listener.AcceptTcpClient())
                using (var stream = client.GetStream())
                {
                    var r = new System.IO.BinaryReader(stream);
                    kind = System.Text.Encoding.ASCII.GetString(r.ReadBytes(4));
                    long size = r.ReadInt64();
                    var body = new System.IO.BinaryReader(new System.IO.MemoryStream(r.ReadBytes((int)size)));
                    Assert.AreEqual(GpuLearner.Version, body.ReadInt32());
                    body.ReadBytes(3 * 4 + 6 * 8); // epochs, minibatch, seed and the PPO settings
                    int count = body.ReadInt32();
                    for (int k = 0; k < count; k++)
                    {
                        GpuLearner.ReadString(body);
                        int nd = body.ReadInt32();
                        int n = 1;
                        for (int d = 0; d < nd; d++) n *= body.ReadInt32();
                        body.ReadBytes(4 * n);
                    }
                    int rows = body.ReadInt32();
                    body.ReadBytes(rows * (PolicyNet.CardDim + PolicyNet.StaticSize) * 4);
                    decisions = body.ReadInt32();
                    var reply = new System.IO.MemoryStream();
                    using (var w = new System.IO.BinaryWriter(reply, System.Text.Encoding.UTF8, true))
                    {
                        w.Write(-0.01);
                        w.Write(0.2);
                        w.Write(0.9);
                        w.Write(0.1);
                        w.Write(3);
                        w.Write(0);
                        w.Write(0.05);
                        GpuLearner.WriteString(w, "fake GPU");
                        learned.WriteLearnerWeights(w, false);
                    }
                    var head = new System.IO.BinaryWriter(stream);
                    head.Write(System.Text.Encoding.ASCII.GetBytes("RES1"));
                    head.Write(reply.Length);
                    head.Write(reply.ToArray());
                    head.Flush();
                }
            });
            fake.Start();
            var trainer = SmallTrainer(net);
            trainer.Gpu = new GpuLearner("127.0.0.1", port);
            var round = trainer.Round();
            fake.Join(30000);
            listener.Stop();
            trainer.Dispose();
            Assert.AreEqual("TRN1", kind);
            Assert.Greater(decisions, 0, "the round's decisions were sent");
            Assert.AreEqual("fake GPU", round.LearnedOn, round.LastError);
            Assert.AreEqual(3, net.Steps, "the learner's steps count as the network's");
            Assert.AreEqual(0.2, round.ValueLoss, 1e-9);
            Assert.AreEqual(Dense(learned), Dense(net), "the weights are the learner's now");
        }

        /// <summary>The network's tensors (not the card table) as the GPU learner sees them, as text to compare.</summary>
        private static string Dense(PolicyNet net)
        {
            var ms = new System.IO.MemoryStream();
            using (var w = new System.IO.BinaryWriter(ms, System.Text.Encoding.UTF8, true)) net.WriteLearnerWeights(w, false);
            ms.Position = 0;
            var r = new System.IO.BinaryReader(ms);
            int count = r.ReadInt32();
            for (int k = 0; k < count; k++)
            {
                GpuLearner.ReadString(r);
                int nd = r.ReadInt32();
                int n = 1;
                for (int d = 0; d < nd; d++) n *= r.ReadInt32();
                r.ReadBytes(4 * n);
            }
            return Convert.ToBase64String(ms.ToArray(), 0, (int)ms.Position);
        }

        [Test]
        public void Games_Per_Second_Keeps_Training_To_The_Rate_Chosen()
        {
            var pace = new GamePace(40);
            int started = 0;
            var clock = System.Diagnostics.Stopwatch.StartNew();
            var threads = Enumerable.Range(0, 4).Select(_ => new System.Threading.Thread(() =>
            {
                for (int i = 0; i < 10; i++)
                {
                    pace.Wait(null);
                    System.Threading.Interlocked.Increment(ref started);
                }
            })).ToList();
            threads.ForEach(t => t.Start());
            threads.ForEach(t => t.Join());
            // 40 games at 40 a second: the first starts at once, the last a little under a second later, whatever the threads.
            Assert.AreEqual(40, started);
            Assert.That(clock.Elapsed.TotalSeconds, Is.GreaterThan(0.9).And.LessThan(1.6));

            // A pause (learning) isn't made up afterwards with a burst.
            System.Threading.Thread.Sleep(500);
            clock.Restart();
            for (int i = 0; i < 10; i++) pace.Wait(null);
            Assert.Greater(clock.Elapsed.TotalSeconds, 0.2);

            clock.Restart();
            var none = new GamePace(0);
            for (int i = 0; i < 1000; i++) none.Wait(null);
            Assert.Less(clock.Elapsed.TotalSeconds, 0.5, "no limit: no waiting");
        }

        [Test]
        public void GPU_Learner_Addresses_Are_Read()
        {
            string host;
            int port;
            Assert.IsTrue(GpuLearner.TryParse("127.0.0.1:5577", out host, out port));
            Assert.AreEqual("127.0.0.1", host);
            Assert.AreEqual(5577, port);
            Assert.IsTrue(GpuLearner.TryParse("6000", out host, out port));
            Assert.AreEqual(6000, port);
            Assert.IsTrue(GpuLearner.TryParse(" pc-in-the-den:5578 ", out host, out port));
            Assert.AreEqual("pc-in-the-den", host);
            Assert.IsFalse(GpuLearner.TryParse("127.0.0.1:port", out host, out port));
            Assert.IsFalse(GpuLearner.TryParse("", out host, out port));
        }

        /// <summary>Plays the Neural AI (seat 0) against the classic bot and keeps the position and its memory at its first main action from that turn on.</summary>
        private sealed class PositionGrabber : IPlayerAgent, IGameObserver
        {
            public readonly PolicyAgent Inner;
            private readonly int fromTurn;
            private GameEngine engine;
            public GameState Position;
            public PolicyMemory Memory;

            public PositionGrabber(PolicyNet net, int fromTurn)
            {
                Inner = new PolicyAgent(net, 1, 0);
                this.fromTurn = fromTurn;
            }

            public List<int> Decide(DecisionRequest r)
            {
                var picks = Inner.Decide(r);
                if (Position == null && r.Kind == DecisionKind.MainAction && r.View.TurnNumber >= fromTurn)
                {
                    Position = engine.State.Clone();
                    Memory = new PolicyMemory(0);
                    Memory.CopyFrom(Inner.Memory);
                }
                return picks;
            }

            public void OnEvent(GameEngine e, GameEvent ev)
            {
                engine = e;
                Inner.OnEvent(e, ev);
            }
        }

        private static List<string> Names(IEnumerable<CardInstance> cards)
        {
            return cards.Select(c => c.Def.Name).ToList();
        }

        [Test]
        public void Look_Ahead_Never_Uses_The_Opponents_Hidden_Cards()
        {
            var grab = new PositionGrabber(new PolicyNet(), 5);
            var engine = new GameEngine(StandardCards.CreateRegistry(), grab, new HeuristicAgent(3));
            engine.Run(Dragapult(), Zoroark(), 31);
            Assert.NotNull(grab.Position, "a main action on turn 5 or later");
            var real = grab.Position;
            var opp = real.Players[1];

            // The same position with the opponent's hand, deck and face-down Prize cards swapped for other cards (same counts).
            var other = real.Clone();
            var o = other.Players[1];
            var swap = Dragapult();
            int id = 100000, k = 0;
            Func<CardInstance> next = () => new CardInstance(id++, swap[k++ % swap.Count], 1);
            o.Hand = o.Hand.Select(c => next()).ToList();
            o.Deck = o.Deck.Select(c => next()).ToList();
            for (int i = 0; i < o.Prizes.Count; i++) if (!o.FaceUpPrizeIds.Contains(o.Prizes[i].Id)) o.Prizes[i] = next();

            var search = new PolicySearch(new PolicyNet(), new SearchSettings());
            search.KnownDecks.Add(Dragapult());
            search.KnownDecks.Add(Zoroark());
            for (int seed = 1; seed <= 5; seed++)
            {
                List<CardDef> guessA, guessB;
                var a = search.Guess(real, 0, grab.Memory, seed, out guessA).Players[1];
                var b = search.Guess(other, 0, grab.Memory, seed, out guessB).Players[1];
                Assert.AreSame(guessA, guessB, "the same deck guessed");
                CollectionAssert.AreEqual(Names(a.Hand), Names(b.Hand), "the guessed hand doesn't depend on the real one");
                CollectionAssert.AreEqual(Names(a.Deck), Names(b.Deck));
                CollectionAssert.AreEqual(Names(a.Prizes), Names(b.Prizes));
                Assert.AreEqual(opp.Hand.Count, a.Hand.Count);
                Assert.AreEqual(opp.Deck.Count, a.Deck.Count);
                Assert.AreEqual(opp.Prizes.Count, a.Prizes.Count);
                // The player's own side keeps its hand and holds the same cards, only the hidden order changes.
                CollectionAssert.AreEqual(Names(real.Players[0].Hand), Names(search.Guess(real, 0, grab.Memory, seed, out guessA).Players[0].Hand));
            }
            List<CardDef> guess;
            search.Guess(real, 0, grab.Memory, 1, out guess);
            Assert.AreEqual(Zoroark().Count, guess.Count);
            CollectionAssert.AreEquivalent(Zoroark().Select(c => c.Name).ToList(), guess.Select(c => c.Name).ToList(),
                "after a few turns the cards Zoroark has shown point to the Zoroark deck");
        }

        [Test]
        public void The_Plan_Of_The_Turn_Learns_Which_Moves_Do_Best()
        {
            var r = new DecisionRequest { Kind = DecisionKind.MainAction };
            r.Options.Add(new DecisionOption { Label = "Play A", Action = ActionType.PlayTrainer });
            r.Options.Add(new DecisionOption { Label = "Play B", Action = ActionType.PlayTrainer });
            var choices = PolicyEncoder.Choices(r, new List<int>());
            int a = choices.FindIndex(c => r.Options[c.Option].Label == "Play A");
            var probs = new double[choices.Count];
            for (int i = 0; i < probs.Length; i++) probs[i] = i == a ? 0.7 : 0.3; // the network prefers A
            var root = new PolicySearch.PlanNode();

            // The first play-out reaches a point nobody has been to: the plan ends there and the network's top move is played.
            var node = root;
            var path = new List<PolicySearch.PlanNode.Step>();
            Assert.AreEqual(a, PolicySearch.PlanNode.Select(ref node, r, choices, probs, 1.0, path));
            Assert.IsNull(node, "past the known plan, the network plays");
            PolicySearch.PlanNode.Backup(root, path, 0.0);

            // B turns out better whenever it's tried: the plan comes to prefer it, though the network doesn't.
            int laterB = 0;
            for (int k = 0; k < 60; k++)
            {
                node = root;
                path = new List<PolicySearch.PlanNode.Step>();
                int pick = PolicySearch.PlanNode.Select(ref node, r, choices, probs, 1.0, path);
                bool b = r.Options[choices[pick].Option].Label == "Play B";
                if (k >= 30 && b) laterB++;
                PolicySearch.PlanNode.Backup(root, path, b ? 0.8 : -0.2);
            }
            Assert.Greater(laterB, 25, "most of the later play-outs try B");
            StringAssert.StartsWith("Play B", root.Describe(1));

            // A play-out that failed counts for nothing (its try is taken back).
            string before = root.Describe(1);
            node = root;
            path = new List<PolicySearch.PlanNode.Step>();
            PolicySearch.PlanNode.Select(ref node, r, choices, probs, 1.0, path);
            PolicySearch.PlanNode.Backup(root, path, double.NaN);
            Assert.AreEqual(before, root.Describe(1));
        }

        [Test]
        public void Deck_Guesses_Favour_The_Decks_That_Match_What_The_Opponent_Showed()
        {
            var grab = new PositionGrabber(new PolicyNet(), 5);
            new GameEngine(StandardCards.CreateRegistry(), grab, new HeuristicAgent(3)).Run(Dragapult(), Zoroark(), 31);
            int zoroark = Zoroark().Count;
            Func<double, int> zoroarkGuesses = belief =>
            {
                var search = new PolicySearch(new PolicyNet(), new SearchSettings { DeckBelief = belief });
                search.KnownDecks.Add(Dragapult());
                search.KnownDecks.Add(Zoroark());
                int n = 0;
                for (int seed = 1; seed <= 40; seed++)
                {
                    List<CardDef> guess;
                    search.Guess(grab.Position, 0, grab.Memory, seed, out guess);
                    if (ReferenceEquals(guess, search.KnownDecks[1])) n++;
                }
                return n;
            };
            Assert.AreEqual(40, zoroarkGuesses(0), "0: always the closest deck");
            Assert.GreaterOrEqual(zoroarkGuesses(1.5), 36, "a firm belief: nearly always the deck the shown cards point to");
            int loose = zoroarkGuesses(0.001);
            Assert.That(loose, Is.InRange(5, 35), "a very loose belief guesses both decks");
        }

        /// <summary>Plays a seat with a PolicyAgent and keeps every look-ahead report.</summary>
        private sealed class ReportProbe : IPlayerAgent, IGameObserver
        {
            private readonly PolicyAgent inner;
            public readonly List<string> Reports = new List<string>();

            public ReportProbe(PolicyAgent inner)
            {
                this.inner = inner;
            }

            public List<int> Decide(DecisionRequest r)
            {
                var picks = inner.Decide(r);
                if (r.Kind == DecisionKind.MainAction && !string.IsNullOrEmpty(inner.Search.LastReport)) Reports.Add(inner.Search.LastReport);
                inner.Search.LastReport = "";
                return picks;
            }

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                inner.OnEvent(engine, ev);
            }
        }

        [Test]
        public void The_Look_Ahead_Can_Plan_Whole_Turns()
        {
            var net = new PolicyNet();
            var settings = new SearchSettings { Playouts = 12, MaxCandidates = 3, SkipAbove = 1.01, Threads = 2, MaxSeconds = 3, PlanTurn = true };
            var search = new PolicySearch(net, settings);
            search.KnownDecks.Add(Dragapult());
            search.KnownDecks.Add(Zoroark());
            var probe = new ReportProbe(new PolicyAgent(net, 1, 0, Dragapult()) { Greedy = true, Search = search });
            var engine = new GameEngine(StandardCards.CreateRegistry(), probe, new HeuristicAgent(4)) { MaxTurns = 12 };
            engine.Run(Dragapult(), Zoroark(), 41);
            Assert.Greater(search.Searches, 3, "it looked ahead at its main actions");
            Assert.Less(search.Failed, Math.Max(2, search.Playouts / 20), "almost every guess plays out");
            Assert.IsTrue(probe.Reports.Any(x => x.Contains("its plan for the turn")), "some looks planned the rest of the turn: "
                          + string.Join(" / ", probe.Reports.Take(3)));
        }

        [Test]
        public void The_Neural_AI_Plays_Whole_Games_Looking_Ahead()
        {
            var net = new PolicyNet();
            var settings = new SearchSettings { Playouts = 8, MaxCandidates = 3, SkipAbove = 1.01, Threads = 2, MaxSeconds = 2 };
            var search = new PolicySearch(net, settings);
            search.KnownDecks.Add(Dragapult());
            search.KnownDecks.Add(Zoroark());
            var bot = new PolicyAgent(net, 1, 0, Dragapult()) { Greedy = true, Search = search };
            var engine = new GameEngine(StandardCards.CreateRegistry(), bot, new HeuristicAgent(4)) { MaxTurns = 12 };
            engine.Run(Dragapult(), Zoroark(), 41);
            Assert.Greater(search.Searches, 3, "it looked ahead at its main actions");
            Assert.Greater(search.Playouts, search.Searches * 4);
            Assert.Less(search.Failed, Math.Max(2, search.Playouts / 20), "almost every guess plays out");

            // Training and recording games never look ahead (they learn from the network's own choices).
            var learner = new PolicyAgent(net, 2, 0, Dragapult()) { Greedy = false, Record = true, Search = new PolicySearch(net, settings) };
            new GameEngine(StandardCards.CreateRegistry(), learner, new HeuristicAgent(5)) { MaxTurns = 8 }.Run(Dragapult(), Zoroark(), 42);
            Assert.AreEqual(0, learner.Search.Searches);
        }

        [Test]
        public void Skipping_CPU_Threads_Leaves_Every_Other_Thread()
        {
            CollectionAssert.AreEqual(new[] { 2, 3 }, CpuThreads.Parse("2, 3"));
            CollectionAssert.AreEqual(new[] { 2, 3, 7 }, CpuThreads.Parse(" 7 3,2 ;3 x -1 64"));
            Assert.AreEqual(0, CpuThreads.Parse("").Count);
            Assert.AreEqual(0xFFF3UL, CpuThreads.Mask(0xFFFFUL, CpuThreads.Parse("2,3")), "16 threads without 2 and 3");
            Assert.AreEqual(0xFFFFUL, CpuThreads.Mask(0xFFFFUL, CpuThreads.Parse("")));
            Assert.AreEqual(0xFFUL, CpuThreads.Mask(0xFFUL, CpuThreads.Parse("20")), "a thread the PC doesn't have changes nothing");
            Assert.AreEqual(0UL, CpuThreads.Mask(0x3UL, CpuThreads.Parse("0 1")), "skipping every thread leaves none (Apply refuses it)");
            if (!CpuThreads.Supported) StringAssert.Contains("Windows", CpuThreads.Apply(CpuThreads.Parse("2,3")));
        }
    }
}
