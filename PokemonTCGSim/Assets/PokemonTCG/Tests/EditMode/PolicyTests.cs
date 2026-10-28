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
