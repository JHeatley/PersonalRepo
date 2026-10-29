using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v160: Team Magma vs Team Aqua (MA): the card data and prints, 2-type Pokémon, Power Saver, Aqua Energy, Double Rainbow Energy,
    /// delayed Special Conditions, Archie, Team Magma Hideout and MA's own Strength Charm.
    /// </summary>
    public class MaTests : ScenarioTestBase
    {
        const string Fighting = "MEE-14";
        const string Water = "MEE-11";

        const string Kyogre = "MA-3";
        const string Walrein = "MA-6";
        const string MagmaLairon = "MA-20";
        const string AquaCarvanha = "MA-48";
        const string AquaSpheal = "MA-57";
        const string MagmaAron = "MA-58";
        const string Archie = "MA-71";
        const string StrengthCharm = "MA-74";
        const string MagmaHideout = "MA-83";
        const string AquaEnergy = "MA-86";
        const string DoubleRainbow = "MA-88";
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
        public void Team_Magma_vs_Team_Aqua_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "MA").ToList();
            Assert.AreEqual(93, set.Count, "97 cards minus 4 prints");
            Assert.AreEqual(97, db.All.Count(d => d.SetCode == "MA"));
            Assert.AreEqual("CEL-CC11", db.MainPrinting(db.Get("MA-9")).Key);
            Assert.AreEqual("NP-13", db.MainPrinting(db.Get("MA-42")).Key);
            Assert.AreEqual("MA-72", db.MainPrinting(db.Get("MA-72")).Key, "your ruling: Dual Ball is its own card (it can find Baby Pokémon)");
            Assert.AreEqual("G2-126", db.MainPrinting(db.Get("MA-85")).Key);
            Assert.AreEqual("MA-74", db.MainPrinting(C(StrengthCharm)).Key, "your ruling: its own card");
            Assert.AreEqual(EnergyType.Darkness, C(Kyogre).SecondType);
        }

        [Test]
        public void Power_Saver_Needs_4_Team_Aqua_Pokemon_In_Play()
        {
            foreach (int extra in new[] { 2, 3 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var kyogre = e.ScenarioActive(0, C(Kyogre));
                Attach(e, kyogre, Water, 2);
                for (int i = 0; i < extra; i++) e.ScenarioBench(0, C(AquaCarvanha));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.IsTrue(e.IsType(kyogre, EnergyType.Darkness), "both types");
                Assert.AreEqual(extra == 3, Offered(e, 0, "Attack: Aqua Trip"), extra + 1 + " Team Aqua Pokémon");
            }
        }

        [Test]
        public void Aqua_Energy_Gives_2_Water_Or_Darkness_And_Goes_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var carvanha = e.ScenarioActive(0, C(AquaCarvanha));
            var chansey = e.ScenarioBench(0, C(Chansey));
            Attach(e, carvanha, AquaEnergy, 1);
            e.ScenarioAddToHand(0, C(AquaEnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(EnergyMask.CanPay(e.EnergyUnits(carvanha), new List<EnergyType> { EnergyType.Water, EnergyType.Water }));
            Assert.IsTrue(EnergyMask.CanPay(e.EnergyUnits(carvanha), new List<EnergyType> { EnergyType.Darkness, EnergyType.Darkness }), "your ruling: any mix");
            Assert.IsFalse(Offered(e, 0, "Attach Aqua Energy to " + chansey.Name), "only Team Aqua Pokémon");
            e.EndTurn();
            Assert.AreEqual(0, carvanha.Energy.Count);
        }

        [Test]
        public void Double_Rainbow_Energy_Goes_On_Evolved_Pokemon_And_Takes_10_Off_Their_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var lairon = e.ScenarioActive(0, C(MagmaAron));
            var aron = e.ScenarioBench(0, C(MagmaAron));
            e.ScenarioEvolve(lairon, C(MagmaLairon));
            Attach(e, lairon, DoubleRainbow, 1);
            Attach(e, lairon, Fighting, 1);
            e.ScenarioAddToHand(0, C(DoubleRainbow));
            var spheal = e.ScenarioActive(1, C(AquaSpheal));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attach Double Rainbow Energy to " + aron.Name), "not on a Basic Pokémon");
            Step(e);
            Assert.AreEqual(4, spheal.DamageCounters, "50 - 10");
        }

        [Test]
        public void Aqua_Trance_Puts_The_Defending_Pokemon_To_Sleep_At_The_End_Of_Its_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Aqua Trance"), new ScriptedAgent());
            var spheal = e.ScenarioActive(0, C(AquaSpheal));
            Attach(e, spheal, Water, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            Assert.IsFalse(chansey.Has(SpecialCondition.Asleep), "not yet");
            e.ScenarioSetTurn(1, 4);
            e.EndTurn();
            Assert.IsTrue(chansey.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Archie_Benches_A_Team_Aqua_Stage_2_With_2_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Archie"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Archie));
            e.ScenarioAddToDeck(0, C(Walrein), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var walrein = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Team Aqua's Walrein", walrein.Name);
            Assert.AreEqual(2, walrein.DamageCounters);
        }

        [Test]
        public void Team_Magma_Hideout_Puts_A_Counter_On_Other_Basic_Pokemon_Played_From_The_Hand()
        {
            foreach (bool magma in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then(magma ? "Bench Team Magma's Aron" : "Bench Team Aqua's Carvanha"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioAddToHand(0, C(magma ? MagmaAron : AquaCarvanha));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetStadium(1, C(MagmaHideout));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(magma ? 0 : 1, e.State.Players[0].Bench.Single().DamageCounters);
            }
        }

        [Test]
        public void MA_Strength_Charm_Adds_10_After_Weakness_And_Is_Then_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var lairon = e.ScenarioActive(0, C(MagmaAron));
            e.ScenarioEvolve(lairon, C(MagmaLairon));
            Attach(e, lairon, Fighting, 3);
            e.ScenarioAttachTool(lairon, C(StrengthCharm));
            var walrein = e.ScenarioActive(1, C(Walrein));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, walrein.DamageCounters, "50 x 2 (Fighting Weakness) + 10");
            e.EndTurn();
            Assert.AreEqual(0, lairon.Tools.Count);
        }
    }
}
