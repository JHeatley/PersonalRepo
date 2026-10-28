using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>v78: training can play as the Learning, Neural or Hybrid AI, apart from the Opponent switch.</summary>
    public class TrainStyleTests : ScenarioTestBase
    {
        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }
        private List<CardDef> Zoroark() { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); }

        private static List<List<string>> Options()
        {
            return new List<List<string>>
            {
                new List<string> { "M:Attack:Dragapult ex/Phantom Dive", "M:Attack|ko1" },
                new List<string> { "M:End|atk1", "M:End|attach0" },
                new List<string> { "M:Trainer:Ultra Ball", "M:Trainer|Item|mid" }
            };
        }

        private static float[] SomeState()
        {
            var x = new float[NeuralNet.DenseSize];
            x[1] = 1f;
            x[3] = 0.5f;
            return x;
        }

        [Test]
        public void Each_Style_Can_Be_Asked_For_Whatever_The_Brains_Own()
        {
            var brain = new LearningBrain { Style = AiStyle.Hybrid };
            var options = Options();
            var state = SomeState();
            brain.SetWeight("M:End|atk1", 1.0);
            for (int k = 0; k < 30; k++) brain.ImitateChoice(options, state, new[] { 2 }, 1.0);
            foreach (var style in new[] { AiStyle.Learning, AiStyle.Neural, AiStyle.Hybrid })
            {
                var asked = brain.PlayValues(options, state, style);
                brain.Style = style;
                CollectionAssert.AreEqual(brain.PlayValues(options, state), asked, style.ToString());
                brain.Style = AiStyle.Hybrid;
            }
        }

        [Test]
        public void An_Agent_Plays_Its_Own_Style_And_Leaves_The_Brains_Alone()
        {
            var trained = new LearningBrain();
            new TrainingSession(StandardCards.CreateRegistry(), trained, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 6, 0, 7801).RunBlocking();
            string text = trained.Serialize();
            var neural = new LearningBrain();
            neural.LoadText(text);
            neural.Style = AiStyle.Neural;
            var learning = new LearningBrain();
            learning.LoadText(text);
            learning.Style = AiStyle.Learning;

            for (int seed = 7810; seed < 7813; seed++)
            {
                var a = Play(neural, null, seed);
                var b = Play(learning, AiStyle.Neural, seed);
                Assert.AreEqual(a.Winner, b.Winner, "seed " + seed);
                Assert.AreEqual(a.Turns, b.Turns, "seed " + seed);
                Assert.AreEqual(a.Reason, b.Reason, "seed " + seed);
            }
            Assert.AreEqual(AiStyle.Learning, learning.Style, "the agents' style doesn't change the brain's");
        }

        private GameResult Play(LearningBrain brain, AiStyle? style, int seed)
        {
            var a0 = new LearningAgent(brain, seed * 2 + 1, "Dragapult", "Zoroark") { Training = false, Record = false, Style = style };
            var a1 = new LearningAgent(brain, seed * 2 + 2, "Zoroark", "Dragapult") { Training = false, Record = false, Style = style };
            var engine = new GameEngine(StandardCards.CreateRegistry(), a0, a1);
            engine.CardsPerPlayer = 60;
            return engine.Run(Dragapult(), Zoroark(), seed);
        }

        [Test]
        public void A_Training_Session_Plays_The_Style_Picked_For_It()
        {
            var brain = new LearningBrain { Style = AiStyle.Hybrid };
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 4, 2, 7820)
            {
                Style = AiStyle.Neural
            };
            Assert.AreEqual("Neural AI", session.StyleLabel);
            session.RunBlocking();
            var st = session.Status();
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(AiStyle.Hybrid, brain.Style, "the Opponent switch's style is left alone");
            Assert.AreEqual("Neural AI", brain.History().Last().Style);
            Assert.Greater(brain.FeatureCount, 0, "the feature weights learned too");
            Assert.Greater(brain.Net.Updates, 0);
            Assert.AreEqual("Hybrid AI", new TrainingSession(StandardCards.CreateRegistry(), brain, "A", Dragapult(), "B", Zoroark(), 1, 0, 1).StyleLabel,
                "no style picked: the brain's");
        }

        [Test]
        public void The_Style_Of_Each_Session_Is_Saved()
        {
            var brain = new LearningBrain();
            brain.AddSession(new SessionRecord { When = "2026-10-15 09:00", DeckA = "A", DeckB = "B", Games = 100, Style = "Neural AI" });
            var back = new LearningBrain();
            back.LoadText(brain.Serialize());
            Assert.AreEqual("Neural AI", back.History()[0].Style);
            // A brain saved before v78 has no style on its sessions.
            var old = new LearningBrain();
            old.LoadText("s\t100\t50\t50\t-1\t-1\t0\t2026-01-01 10:00\tA\tB\n");
            Assert.AreEqual(1, old.History().Count);
            Assert.AreEqual("", old.History()[0].Style);
        }
    }
}
