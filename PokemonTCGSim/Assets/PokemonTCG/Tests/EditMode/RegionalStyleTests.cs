using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// The Regional Sim's AI players play as the Neural AI. Events saved when the Learning AI and Hybrid AI were still choices
    /// (v117) load as the Neural AI too.
    /// </summary>
    public class RegionalStyleTests
    {
        private static RegionalSim MakeEvent()
        {
            var texts = new Dictionary<string, string> { { "Test Deck", "" } };
            var sim = RegionalSim.Create(8, 3, false, "Standard", null, new[] { "Test Deck" }, texts, false, 7);
            sim.Begin();
            return sim;
        }

        [Test]
        public void A_New_Event_Plays_The_Neural_AI()
        {
            var sim = MakeEvent();
            Assert.AreEqual(AiStyle.Neural, sim.PlayStyle);
            Assert.AreEqual(AiStyle.Neural, sim.View().PlayStyle);
            Assert.AreEqual(AiStyle.Neural, RegionalSim.Parse(sim.Serialize()).PlayStyle);
        }

        [Test]
        public void Events_Saved_With_The_Learning_Or_Hybrid_AI_Play_The_Neural_AI()
        {
            foreach (var old in new[] { AiStyle.Learning, AiStyle.Hybrid })
            {
                var sim = MakeEvent();
                sim.PlayStyle = old; // as an event saved before they were removed
                Assert.IsTrue(sim.Serialize().Split('\n').Any(l => l.StartsWith("Style\t")));
                Assert.AreEqual(AiStyle.Neural, RegionalSim.Parse(sim.Serialize()).PlayStyle, old.ToString());
            }
        }

        [Test]
        public void An_Event_Saved_Before_v117_Plays_The_Neural_AI()
        {
            var sim = MakeEvent();
            var lines = sim.Serialize().Split('\n').Where(l => !l.StartsWith("Style\t")).ToArray();
            Assert.AreEqual(AiStyle.Neural, RegionalSim.Parse(string.Join("\n", lines)).PlayStyle);
        }
    }
}
