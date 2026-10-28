using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v164: Team Rocket Returns (TRR): the card data and prints, the Pokémon ★ deck rule, Rocket's Hideout, R Energy, Rocket's
    /// Tricky Gym, Darkest Impulse, Dragon Veil, Saturation, Holy Shield and Detour.
    /// </summary>
    public class TrrTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";

        const string DarkAmpharos = "TRR-2";
        const string Kingdra = "TRR-12";
        const string Togetic = "TRR-14";
        const string DarkRaticate = "TRR-17";
        const string Magby = "TRR-24";
        const string DarkFlaaffy = "TRR-33";
        const string Mareep = "TRR-67";
        const string Onix = "TRR-69";
        const string Wooper = "TRR-81";
        const string RocketsHideout = "TRR-87";
        const string RocketsMission = "TRR-88";
        const string TrickyGym = "TRR-90";
        const string REnergy = "TRR-95";
        const string RocketsMewtwoEx = "TRR-99";
        const string RocketsSneaselEx = "TRR-103";
        const string MudkipStar = "TRR-107";
        const string TorchicStar = "TRR-108";
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
        public void Team_Rocket_Returns_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(107, db.Cards.Count(d => d.SetCode == "TRR"), "111 cards minus 4 prints");
            Assert.AreEqual(111, db.All.Count(d => d.SetCode == "TRR"));
            Assert.AreEqual("EVS-143", db.MainPrinting(db.Get("TRR-83")).Key);
            Assert.AreEqual("CEL-CC12", db.MainPrinting(db.Get("TRR-86")).Key);
            Assert.AreEqual("DR-99", db.MainPrinting(db.Get("TRR-110")).Key);
            Assert.AreEqual("CEL-CC6", db.MainPrinting(db.Get("TRR-111")).Key);
            Assert.AreEqual("TRR-87", db.MainPrinting(C(RocketsHideout)).Key, "your ruling: its own card");
            Assert.AreEqual(EnergyType.Darkness, C(DarkAmpharos).SecondType);
            Assert.IsTrue(C(MudkipStar).IsPokemonStar);
        }

        [Test]
        public void Only_One_Pokemon_Star_In_A_Deck()
        {
            var deck = new List<CardDef> { C(MudkipStar), C(TorchicStar) };
            while (deck.Count < 60) deck.Add(C(Water));
            var problems = Decklist.Validate(deck, DeckFormat.Unlimited);
            Assert.IsTrue(problems.Any(p => p.Contains("Pokémon ★")), string.Join("; ", problems.ToArray()));
        }

        [Test]
        public void Rocket_s_Hideout_Gives_Rocket_s_Pokemon_20_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(RocketsMewtwoEx));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(0, C(RocketsHideout));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(120, e.MaxHp(mewtwo));
            Assert.AreEqual(100, e.MaxHp(chansey));
        }

        [Test]
        public void R_Energy_Goes_Only_On_Dark_Or_Rocket_s_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(RocketsSneaselEx));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToHand(0, C(REnergy));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attach R Energy to Rocket's Sneasel ex"));
            Assert.IsFalse(Offered(e, 0, "Attach R Energy to Chansey"));
        }

        [Test]
        public void R_Energy_Adds_10_And_Is_Discarded_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drag Off"), new ScriptedAgent());
            var sneasel = e.ScenarioActive(0, C(RocketsSneaselEx));
            Attach(e, sneasel, REnergy, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters, "10 + 10");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sneasel2 = e2.ScenarioActive(0, C(RocketsSneaselEx));
            Attach(e2, sneasel2, REnergy, 1);
            e2.ScenarioActive(1, C(Chansey));
            e2.ScenarioSetTurn(0, 3);
            e2.EndTurn();
            Assert.AreEqual(0, sneasel2.Energy.Count);
        }

        [Test]
        public void Rocket_s_Tricky_Gym_Lends_Feint_Attack_To_Rocket_s_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sneasel = e.ScenarioActive(0, C(RocketsSneaselEx));
            Attach(e, sneasel, Darkness, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            Attach(e, chansey, Fire, 1);
            e.ScenarioSetStadium(0, C(TrickyGym));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Feint Attack"));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Attack: Feint Attack"));
        }

        [Test]
        public void Darkest_Impulse_Puts_2_Counters_On_Each_Evolved_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Evolve Mareep (Active) into Dark Flaaffy"));
            e.ScenarioActive(0, C(DarkAmpharos));
            e.ScenarioBench(0, C(DarkAmpharos));
            var mareep = e.ScenarioActive(1, C(Mareep));
            e.ScenarioAddToHand(1, C(DarkFlaaffy));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual("Dark Flaaffy", mareep.Name);
            Assert.AreEqual(2, mareep.DamageCounters, "2 Dark Ampharos don't stack");
        }

        [Test]
        public void Dragon_Veil_Removes_Your_Active_Pokemon_s_Weakness()
        {
            foreach (bool kingdra in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Granite Head"));
                var chansey = e.ScenarioActive(0, C(Chansey));
                if (kingdra) e.ScenarioBench(0, C(Kingdra));
                var onix = e.ScenarioActive(1, C(Onix));
                Attach(e, onix, Fighting, 2);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(kingdra ? 2 : 4, chansey.DamageCounters);
            }
        }

        [Test]
        public void Saturation_Cures_Wooper_When_Water_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy to Wooper"), new ScriptedAgent());
            var wooper = e.ScenarioActive(0, C(Wooper));
            wooper.DamageCounters = 2;
            e.ApplyCondition(wooper, SpecialCondition.Poisoned);
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(wooper.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(1, wooper.DamageCounters);
        }

        [Test]
        public void Holy_Shield_Stops_Dark_Pokemon_s_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spread Poison"), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(DarkRaticate));
            Attach(e, raticate, Darkness, 2);
            var togetic = e.ScenarioActive(1, C(Togetic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, togetic.DamageCounters);
            Assert.IsFalse(togetic.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Detour_Uses_The_Supporter_Played_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rocket's Mission", "Attack: Detour"), new ScriptedAgent());
            var magby = e.ScenarioActive(0, C(Magby));
            Attach(e, magby, Fire, 1);
            e.ScenarioAddToHand(0, C(RocketsMission));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "discarded 1, drew 3");
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count, "Detour: discard 1, draw 3 again");
        }
    }
}
