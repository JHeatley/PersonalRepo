using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Majestic Dawn (MD): the card data and prints, and the new effects (Primal Shell, Chilly Breath, Primal Claw, the Eevee
    /// veils, Darkness Shade, Primal Swirl, Hard Amber, Restore, Call Energy, Health Energy, Dawn Stadium).
    /// </summary>
    public class MdTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
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
        public void Majestic_Dawn_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(90, db.Cards.Count(d => d.SetCode == "MD"));
            for (int n = 1; n <= 100; n++) Assert.IsNotNull(db.Get("MD", n.ToString()), "MD " + n);
            Assert.AreEqual("CES-146", db.MainPrinting(C("MD-87")).Key, "Super Scoop Up");
            Assert.IsTrue(C("MD-84").FossilGivesPrize, "Old Amber gives a Prize card");
            Assert.AreEqual(1, C("MD-92").Abilities.Count, "Call Energy's effect is usable");
        }

        [Test]
        public void Every_Majestic_Dawn_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "MD" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Primal_Shell_Stops_The_Opponents_Trainers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("MD-6"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C("MT-114"));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Quick Ball"));
        }

        [Test]
        public void Chilly_Breath_Stops_The_Opponents_Poke_Powers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var glaceon = e.ScenarioActive(0, C("MD-5"));
            e.ScenarioEvolve(glaceon, C("MD-98"));
            var bronzong = e.ScenarioActive(1, C("MT-74"));
            e.ScenarioEvolve(bronzong, C("MT-6"));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Miracle Oracle"));
        }

        [Test]
        public void Primal_Claw_Hurts_A_Pokemon_That_Uses_A_Poke_Power()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Use Miracle Oracle"));
            e.ScenarioActive(0, C("MD-15"));
            var bronzong = e.ScenarioActive(1, C("MT-74"));
            e.ScenarioEvolve(bronzong, C("MT-6"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, bronzong.DamageCounters);
        }

        [Test]
        public void Sunlight_And_Moonlight_Veil_Help_Eevee_Evolutions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var vaporeon = e.ScenarioActive(0, C("MD-62"));
            e.ScenarioEvolve(vaporeon, C("MD-34"));
            e.ScenarioActive(1, C(Chansey));
            int hp = e.MaxHp(vaporeon);
            int retreat = e.RetreatCostOf(vaporeon);
            Assert.Greater(retreat, 0);
            e.ScenarioBench(0, C("MD-18"));
            e.ScenarioBench(0, C("MD-18"));
            Assert.AreEqual(hp + 20, e.MaxHp(vaporeon), "only 1 Sunlight Veil counts");
            e.ScenarioBench(0, C("MD-32"));
            Assert.AreEqual(0, e.RetreatCostOf(vaporeon));
            Assert.IsNull(e.WeaknessOf(vaporeon));
        }

        [Test]
        public void Darkness_Shade_Puts_The_Defending_Pokemon_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Darkrai"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("MD-3"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotEqual(SpecialCondition.None, chansey.Conditions & SpecialCondition.Asleep);
        }

        [Test]
        public void Primal_Swirl_Devolves_The_Opponents_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var omanyte = e.ScenarioActive(0, C("MD-69"));
            e.ScenarioAddToHand(0, C("MD-26"));
            e.ScenarioActive(1, C(Chansey));
            var benched = e.ScenarioBench(1, C("MD-62"));
            e.ScenarioEvolve(benched, C("MD-34"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Omastar", omanyte.Name);
            Assert.AreEqual("Eevee", benched.Name);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "Vaporeon"));
        }

        [Test]
        public void Hard_Amber_Prevents_Damage_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot", "Old Amber"));
            e.ScenarioActive(0, C(Chansey));
            var amber = e.ScenarioBench(0, C("MD-84"));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, Water, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, amber.DamageCounters);
        }

        [Test]
        public void Restore_Brings_Back_An_Evolution_As_A_Basic()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Restore"), new ScriptedAgent());
            var garchomp = e.ScenarioActive(0, C("MT-9"));
            e.ScenarioEvolve(garchomp, C("MD-97"));
            e.ScenarioAddToDiscard(0, C("MD-34"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Vaporeon"));
        }

        [Test]
        public void Call_Energy_Benches_2_Basics_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Call Energy", "Chansey|Chansey"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAttach(active, C("MD-92"));
            e.ScenarioAddToDeck(0, C(Chansey), 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Health_Energy_And_Dawn_Stadium_Heal_On_Attachment()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Health Energy"), new ScriptedAgent());
            var vaporeon = e.ScenarioActive(0, C("MD-34"));
            vaporeon.DamageCounters = 3;
            vaporeon.Conditions = SpecialCondition.Confused;
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("MD-94"));
            e.ScenarioSetStadium(1, C("MD-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, vaporeon.DamageCounters, "1 from Health Energy, 1 from Dawn Stadium");
            Assert.AreEqual(SpecialCondition.None, vaporeon.Conditions);
        }
    }
}
