using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v161: Hidden Legends (HL): the card data and prints, Temperamental Weather, Primal Pull, Mark of Antiquity, Silver Wind,
    /// Energy Grounding, Desert Ruins and Steven's Advice.
    /// </summary>
    public class HlTests : ScenarioTestBase
    {
        const string Fighting = "MEE-14";
        const string Lightning = "MEE-12";
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Fire = "MEE-10";

        const string Claydol = "HL-2";
        const string Jirachi = "HL-8";
        const string Medicham = "HL-10";
        const string Masquerain = "HL-20";
        const string SunnyCastform = "HL-26";
        const string Castform = "HL-30";
        const string Lanturn = "HL-38";
        const string Meditite = "HL-66";
        const string DesertRuins = "HL-88";
        const string MagneticStorm = "HL-91";
        const string StevensAdvice = "HL-92";
        const string GroudonEx = "HL-93";
        const string KyogreEx = "HL-94";
        const string LatiasEx = "DR-93";
        const string Graveler = "DR-30";
        const string Pikachu = "SS-72";
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
        public void Hidden_Legends_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "HL").ToList();
            Assert.AreEqual(99, set.Count, "102 cards minus 3 prints");
            Assert.AreEqual(102, db.All.Count(d => d.SetCode == "HL"));
            Assert.AreEqual("NP-23", db.MainPrinting(db.Get("HL-21")).Key);
            Assert.AreEqual("NP-22", db.MainPrinting(db.Get("HL-29")).Key);
            Assert.AreEqual("CES-136", db.MainPrinting(db.Get("HL-90")).Key, "your ruling: Life Herb is a print");
            Assert.AreEqual("HL-91", db.MainPrinting(C(MagneticStorm)).Key, "its own card");
            Assert.AreEqual(EnergyType.Metal, C(Jirachi).SecondType);
        }

        [Test]
        public void Temperamental_Weather_Swaps_Castform_With_Everything_On_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Temperamental Weather"), new ScriptedAgent());
            var castform = e.ScenarioActive(0, C(Castform));
            castform.DamageCounters = 2;
            Attach(e, castform, Fire, 1);
            e.ScenarioAddToDeck(0, C(SunnyCastform), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Sunny Castform", castform.Name);
            Assert.AreEqual(2, castform.DamageCounters);
            Assert.AreEqual(1, castform.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Name == "Castform"));
            Assert.IsFalse(Offered(e, 0, "Use Temperamental Weather"), "only 1 each turn");
        }

        [Test]
        public void Primal_Pull_Makes_Evolved_Pokemon_Pay_1_More()
        {
            foreach (bool claydol in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var medicham = e.ScenarioActive(0, C(Meditite));
                e.ScenarioEvolve(medicham, C(Medicham));
                Attach(e, medicham, Fighting, 2);
                e.ScenarioActive(1, C(claydol ? Claydol : Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!claydol, Offered(e, 0, "Attack: Meditate"));
            }
        }

        [Test]
        public void Mark_Of_Antiquity_Stops_The_Other_Legends()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C(KyogreEx));
            Attach(e, kyogre, Water, 3);
            e.ScenarioActive(1, C(GroudonEx));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Super Tidal Wave"));
        }

        [Test]
        public void Silver_Wind_Adds_30_To_Attacks_On_It_During_Your_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Silver Wind", "Attack: Silver Wind"), new ScriptedAgent());
            var masquerain = e.ScenarioActive(0, C(Masquerain));
            Attach(e, masquerain, Grass, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, chansey.DamageCounters);
            e.ScenarioSetTurn(0, 5);
            Step(e);
            Assert.AreEqual(9, chansey.DamageCounters, "30 + 30 more");
        }

        [Test]
        public void Energy_Grounding_Catches_A_Basic_Energy_From_A_Knocked_Out_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rollout"));
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            Attach(e, pikachu, Lightning, 1);
            var lanturn = e.ScenarioBench(0, C(Lanturn));
            var graveler = e.ScenarioActive(1, C(Graveler));
            Attach(e, graveler, Fighting, 3);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsFalse(e.State.Players[0].InPlay().Contains(pikachu));
            Assert.AreEqual(1, lanturn.Energy.Count, "the Lightning Energy came over to Lanturn");
        }

        [Test]
        public void Desert_Ruins_Hurts_Pokemon_ex_With_100_HP_Or_More()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var latias = e.ScenarioActive(0, C(LatiasEx));
            var groudon = e.ScenarioActive(1, C(GroudonEx));
            e.ScenarioSetStadium(0, C(DesertRuins));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, groudon.DamageCounters, "100 HP");
            Assert.AreEqual(0, latias.DamageCounters, "90 HP");
        }

        [Test]
        public void Stevens_Advice_Needs_7_Cards_Or_Fewer_In_Hand()
        {
            foreach (int others in new[] { 6, 7 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioAddToHand(0, C(StevensAdvice));
                for (int i = 0; i < others; i++) e.ScenarioAddToHand(0, C(Fire));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(others == 6, Offered(e, 0, "Play Steven's Advice"), (others + 1) + " cards in hand");
            }
        }
    }
}
