using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v106: Kalos Starter Set (KSS 1-39). Also the alternate-art audit: Unified Minds' Litwick (28) and Riolu (116) are prints of 27
    /// and 115.
    /// </summary>
    public class KssTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string SnorlaxGX = "SMP-5";
        const string Pansage = "KSS-2";
        const string Honedge = "KSS-21";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "KSS" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(31, own.Count);
            Assert.AreEqual("PHF-90", C("KSS-32").BaseKey, "Furfrou is Phantom Forces' card");
            Assert.AreEqual("PHF-90", C("KSS-33").BaseKey, "and so is its second art");
            Assert.AreEqual("BKP-112", C("KSS-39").BaseKey, "Tierno");
        }

        [Test]
        public void Unified_Minds_Alternate_Arts_Are_Linked()
        {
            Assert.AreEqual("UNM-27", C("UNM-28").BaseKey, "Litwick");
            Assert.AreEqual("UNM-115", C("UNM-116").BaseKey, "Riolu");
            Assert.AreEqual(0, db.Cards.Count(d => (d.Key == "UNM-28" || d.Key == "UNM-116") && !d.IsAlternatePrint));
        }

        [Test]
        public void Collect_Draws_A_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Collect"), new ScriptedAgent());
            var pansage = e.ScenarioActive(0, C(Pansage));
            e.ScenarioAttach(pansage, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }
    }
}
