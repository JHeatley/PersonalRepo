using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>v117: the Regional Sim's AI players play as the Learning AI, the Neural AI or the Hybrid AI, saved with the event.</summary>
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
        public void A_New_Event_Plays_Hybrid_AI()
        {
            var sim = MakeEvent();
            Assert.AreEqual(AiStyle.Hybrid, sim.PlayStyle);
            Assert.AreEqual(AiStyle.Hybrid, sim.View().PlayStyle);
        }

        [Test]
        public void The_Style_Changes_And_Is_Saved_With_The_Event()
        {
            var sim = MakeEvent();
            int before = sim.Version;
            sim.PlayStyle = AiStyle.Neural;
            Assert.Greater(sim.Version, before, "the screen sees the change");
            Assert.AreEqual(AiStyle.Neural, sim.View().PlayStyle);
            var back = RegionalSim.Parse(sim.Serialize());
            Assert.AreEqual(AiStyle.Neural, back.PlayStyle);
            back.PlayStyle = AiStyle.Learning;
            Assert.AreEqual(AiStyle.Learning, RegionalSim.Parse(back.Serialize()).PlayStyle);
        }

        [Test]
        public void An_Event_Saved_Before_v117_Plays_Hybrid_AI()
        {
            var sim = MakeEvent();
            sim.PlayStyle = AiStyle.Learning;
            var lines = sim.Serialize().Split('\n').Where(l => !l.StartsWith("Style\t")).ToArray();
            var old = RegionalSim.Parse(string.Join("\n", lines));
            Assert.AreEqual(AiStyle.Hybrid, old.PlayStyle);
        }
    }
}
