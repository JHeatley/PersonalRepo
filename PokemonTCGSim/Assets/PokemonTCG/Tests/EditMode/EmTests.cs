using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v166: Emerald (EM): the card data and prints, Battle Frontier, Wise Aura, Emerald's Darkness Energy and Double Rainbow Energy
    /// (your rulings), Cursed Glare and Eruption.
    /// </summary>
    public class EmTests : ScenarioTestBase
    {
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Fire = "MEE-10";

        const string Groudon = "EM-14";
        const string Hariyama = "EM-31";
        const string Makuhita = "EM-54";
        const string DarknessEnergy = "EM-86";
        const string DoubleRainbow = "EM-87";
        const string BattleFrontier = "EM-75";
        const string CacturneEx = "EM-91";
        const string MedichamEx = "EM-95";
        const string Slakoth = "DX-73";
        const string Vigoroth = "DX-50";
        const string Slaking = "DX-15";
        const string SharpedoEx = "DX-104";
        const string Pidgeot = "RG-10";
        const string Kangaskhan = "RG-6";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Emerald_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(91, db.Cards.Count(d => d.SetCode == "EM"), "107 cards minus 16 prints");
            Assert.AreEqual(107, db.All.Count(d => d.SetCode == "EM"));
            Assert.AreEqual("MEG-125", db.MainPrinting(db.Get("EM-83")).Key);
            Assert.AreEqual("N1-19", db.MainPrinting(db.Get("EM-88")).Key);
            Assert.AreEqual("MEE-14", db.MainPrinting(db.Get("EM-106")).Key);
            Assert.AreEqual("EM-86", db.MainPrinting(C(DarknessEnergy)).Key, "your ruling: its own card");
            Assert.AreEqual("EM-87", db.MainPrinting(C(DoubleRainbow)).Key, "your ruling: its own card");
        }

        [Test]
        public void Battle_Frontier_Switches_Off_Colorless_Evolved_Pokemon_s_Bodies()
        {
            foreach (bool stadium in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Drag Off"), new ScriptedAgent());
                var sharpedo = e.ScenarioActive(0, C(SharpedoEx));
                Attach(e, sharpedo, Darkness, 2);
                var slaking = e.ScenarioActive(1, C(Slakoth));
                e.ScenarioEvolve(slaking, C(Vigoroth));
                e.ScenarioEvolve(slaking, C(Slaking));
                if (stadium) e.ScenarioSetStadium(0, C(BattleFrontier));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(stadium ? 2 : 0, slaking.DamageCounters, "Lazy Aura takes 30 off unless switched off");
            }
        }

        [Test]
        public void Wise_Aura_Stops_Other_Pokemon_s_Poke_Powers()
        {
            foreach (bool medicham in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(medicham ? MedichamEx : Chansey));
                e.ScenarioActive(1, C(Pidgeot));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(!medicham, Offered(e, 1, "Use Quick Search"));
            }
        }

        [Test]
        public void Emerald_Darkness_Energy_Adds_10_For_Darkness_Pokemon_Only()
        {
            foreach (bool dark in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then(dark ? "Attack: Drag Off" : "Attack: Headbutt"), new ScriptedAgent());
                var attacker = e.ScenarioActive(0, C(dark ? SharpedoEx : Kangaskhan));
                Attach(e, attacker, DarknessEnergy, 1);
                Attach(e, attacker, Darkness, 1);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(dark ? 3 : 2, chansey.DamageCounters);
            }
        }

        [Test]
        public void Emerald_Double_Rainbow_Energy_Takes_10_Off_Before_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cross-Cut"), new ScriptedAgent());
            var hariyama = e.ScenarioActive(0, C(Makuhita));
            e.ScenarioEvolve(hariyama, C(Hariyama));
            Attach(e, hariyama, DoubleRainbow, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters, "(20 - 10) x 2, not 20 x 2 - 10");
        }

        [Test]
        public void Cursed_Glare_Stops_Special_Energy_On_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(CacturneEx));
            var hariyama = e.ScenarioActive(1, C(Makuhita));
            e.ScenarioEvolve(hariyama, C(Hariyama));
            e.ScenarioAddToHand(1, C(DoubleRainbow));
            e.ScenarioAddToHand(1, C(Fighting));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Attach Double Rainbow Energy to Hariyama"));
            Assert.IsTrue(Offered(e, 1, "Attach Fighting Energy to Hariyama"));
        }

        [Test]
        public void Eruption_Counts_The_Energy_Cards_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eruption"), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(Groudon));
            Attach(e, groudon, Fighting, 4);
            var sharpedo = e.ScenarioActive(1, C(SharpedoEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, sharpedo.DamageCounters, "40 + 10 for each of the 2 Energy cards discarded");
        }
    }
}
