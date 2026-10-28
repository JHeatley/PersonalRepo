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
                AllDecks = decks
            };
            StringAssert.Contains("all 3 decks", session.MatchLabel);
            session.RunBlocking();
            var st = session.Status();
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(12, st.Done);
            Assert.AreEqual(12, st.WinsA + st.WinsB + st.Draws);
            Assert.GreaterOrEqual(st.ScoreBefore, 0);
            Assert.GreaterOrEqual(st.ScoreAfter, 0);
            // The Learning AI's games go into the matchup table: more than one pair was played. (The Neural AI's games pick their
            // decks in PolicyTrainer, the same way the command-line trainer does.)
            if (style == AiStyle.Learning) Assert.Greater(brain.Matchups().Count, 1);
            else Assert.Greater(brain.Policy.Steps, 0);
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
