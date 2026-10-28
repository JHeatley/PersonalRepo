using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v74: the neural network half of the learning AI, and the Learning / Neural / Hybrid styles.</summary>
    public class NeuralTests : ScenarioTestBase
    {
        private static List<List<string>> Options()
        {
            return new List<List<string>>
            {
                new List<string> { "M:Attack:Dragapult ex/Phantom Dive", "M:Attack:Dragapult ex/Phantom Dive|ko1", "M:Attack|ko1" },
                new List<string> { "M:End|atk1", "M:End|attach0", "M:End|atk1|mid" },
                new List<string> { "M:Trainer:Ultra Ball", "M:Trainer:Ultra Ball|mid", "M:Trainer|Item|mid" }
            };
        }

        private static float[] SomeState()
        {
            var x = new float[NeuralNet.DenseSize];
            x[1] = 1f;
            x[3] = 0.5f;
            x[4] = 0.66f;
            x[6] = 0.6f;
            return x;
        }

        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }
        private List<CardDef> Zoroark() { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); }

        [Test]
        public void An_Untrained_Network_Has_No_Opinion()
        {
            var net = new NeuralNet();
            Assert.IsFalse(net.Trained);
            var values = net.Values(Options(), SomeState());
            Assert.IsTrue(values.All(v => v == 0), "the AI plays exactly as before until the network has learned something");
            var brain = new LearningBrain { Style = AiStyle.Hybrid };
            brain.SetWeight("M:End|atk1", 0.75);
            var played = brain.PlayValues(Options(), SomeState());
            CollectionAssert.AreEqual(brain.Values(Options()), played, "Hybrid with an untrained network is the feature weights");
        }

        [Test]
        public void Learning_Makes_The_Chosen_Option_Score_Higher()
        {
            var net = new NeuralNet();
            var options = Options();
            var state = SomeState();
            for (int k = 0; k < 60; k++) net.Learn(options, state, new[] { 2 }, 1.0);
            Assert.Greater(net.Updates, 0);
            var values = net.Values(options, state);
            Assert.Greater(values[2], values[0]);
            Assert.Greater(values[2], values[1]);
            var policy = net.Policy(options, state);
            Assert.AreEqual(1.0, policy.Sum(), 1e-6);
            Assert.Greater(policy[2], 0.4, "it now expects that play more than the others (1 in 3 untrained)");
        }

        [Test]
        public void Negative_Credit_Makes_The_Choice_Less_Likely()
        {
            var net = new NeuralNet();
            var options = Options();
            var state = SomeState();
            double before = net.Policy(options, state)[0];
            for (int k = 0; k < 40; k++) net.Learn(options, state, new[] { 0 }, -1.0);
            Assert.Less(net.Policy(options, state)[0], before);
        }

        [Test]
        public void The_Network_Saves_And_Loads_With_The_Brain()
        {
            var brain = new LearningBrain();
            var options = Options();
            var state = SomeState();
            for (int k = 0; k < 25; k++) brain.ImitateChoice(options, state, new[] { 1 }, 1.0);
            var expected = brain.Net.Values(options, state);
            string text = brain.Serialize();
            StringAssert.Contains("\nn\t", text);

            var path = Path.Combine(Path.GetTempPath(), "pokemontcg_neural_test.txt");
            brain.FilePath = path;
            brain.Save();
            var loaded = LearningBrain.Load(path);
            File.Delete(path);
            Assert.AreEqual(brain.Net.Updates, loaded.Net.Updates);
            var got = loaded.Net.Values(options, state);
            for (int i = 0; i < expected.Length; i++) Assert.AreEqual(expected[i], got[i], 1e-5);

            loaded.Reset();
            Assert.IsFalse(loaded.Net.Trained, "Reset the AI erases the network too");
            Assert.IsFalse(loaded.Serialize().Contains("\nn\t"));
        }

        [Test]
        public void Undo_Puts_The_Network_Back_Too()
        {
            var brain = new LearningBrain();
            var options = Options();
            var state = SomeState();
            for (int k = 0; k < 10; k++) brain.ImitateChoice(options, state, new[] { 0 }, 1.0);
            string before = brain.Serialize();
            var expected = brain.Net.Values(options, state);
            for (int k = 0; k < 30; k++) brain.ImitateChoice(options, state, new[] { 2 }, 1.0);
            brain.LoadText(before);
            var got = brain.Net.Values(options, state);
            for (int i = 0; i < expected.Length; i++) Assert.AreEqual(expected[i], got[i], 1e-5);
        }

        [Test]
        public void Each_Style_Uses_What_It_Says()
        {
            var brain = new LearningBrain();
            var options = Options();
            var state = SomeState();
            brain.SetWeight("M:End|atk1", 1.0);
            for (int k = 0; k < 40; k++) brain.ImitateChoice(options, state, new[] { 2 }, 1.0);
            var linear = brain.Values(options);
            var neural = brain.Net.Values(options, state);

            brain.Style = AiStyle.Learning;
            CollectionAssert.AreEqual(linear, brain.PlayValues(options, state));
            brain.Style = AiStyle.Neural;
            CollectionAssert.AreEqual(neural, brain.PlayValues(options, state));
            brain.Style = AiStyle.Hybrid;
            var hybrid = brain.PlayValues(options, state);
            for (int i = 0; i < hybrid.Length; i++)
                Assert.AreEqual(System.Math.Max(-4.0, System.Math.Min(4.0, linear[i] + neural[i])), hybrid[i], 1e-9);
        }

        [Test]
        public void Games_Train_The_Network_Alongside_The_Feature_Weights()
        {
            var brain = new LearningBrain();
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 6, 0, 4242);
            session.RunBlocking();
            Assert.AreEqual(0, session.Status().Errors, session.Status().LastError);
            Assert.Greater(brain.FeatureCount, 0);
            Assert.Greater(brain.Net.Updates, 0, "the network learned from the same games");
        }

        [Test]
        public void Neural_And_Hybrid_Agents_Play_Full_Games()
        {
            foreach (var style in new[] { AiStyle.Neural, AiStyle.Hybrid })
            {
                var brain = new LearningBrain { Style = style };
                var options = Options();
                for (int k = 0; k < 20; k++) brain.ImitateChoice(options, SomeState(), new[] { 0 }, 1.0);
                var report = MatchRunner.Run(StandardCards.CreateRegistry(), Dragapult(), Zoroark(), 4, 6100,
                    (seat, s) => new LearningAgent(brain, s, seat == 0 ? "Dragapult" : "Zoroark", seat == 0 ? "Zoroark" : "Dragapult") { Training = true }, true);
                Assert.AreEqual(0, report.Errors.Count, style + ": " + report.Summary());
            }
        }

        [Test]
        public void The_Game_State_Is_Described_With_Small_Numbers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            var request = new DecisionRequest { Player = 0, Kind = DecisionKind.MainAction, Options = e.GetMainActions(0), View = PlayerView.Create(e, 0) };
            var x = NeuralNet.StateOf(request);
            Assert.AreEqual(NeuralNet.DenseSize, x.Length);
            Assert.IsTrue(x.All(v => v >= -1.01f && v <= 1.6f), string.Join(", ", x.Select(v => v.ToString()).ToArray()));
            Assert.AreEqual(1f, x[3], 1e-6, "6 Prize cards left");
            Assert.AreEqual(1f, x[17], "a main action");
        }
    }
}
