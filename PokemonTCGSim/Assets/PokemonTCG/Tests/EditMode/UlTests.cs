using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: HS—Unleashed (UL): the card data and prints, and the new effects (Time Hollow, Thunder Fall, Berserk, Psychic Float,
    /// Wash Out, Energy Signal, Pokémon Circulator).
    /// </summary>
    public class UlTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Unleashed_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(88, db.Cards.Count(d => d.SetCode == "UL"));
            Assert.AreEqual("HSP-17", db.MainPrinting(C("UL-34")).Key, "Minun");
            Assert.AreEqual("DP-109", db.MainPrinting(C("UL-80")).Key, "PlusPower");
        }

        [Test]
        public void Every_Unleashed_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "UL" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Time_Hollow_Devolves_Into_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Time Hollow", "Gyarados"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C("UL-1"));
            Attach(e, jirachi, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            var gyarados = e.ScenarioBench(1, C("MT-89"));
            e.ScenarioEvolve(gyarados, C("MT-26"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Magikarp", gyarados.Name);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "Gyarados"));
        }

        [Test]
        public void Thunder_Fall_Hits_Every_Pokemon_With_A_Poke_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Thunder Fall"), new ScriptedAgent());
            var legend = e.ScenarioActive(0, C("UL-90"));
            Attach(e, legend, "MEE-12", 1);
            Attach(e, legend, Water, 1);
            var banette = e.ScenarioBench(0, C("PL-19"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            var hypno = e.ScenarioBench(1, C("HS-23"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, legend.Energy.Count);
            Assert.AreEqual(0, chansey.DamageCounters);
            Assert.AreEqual(8, hypno.DamageCounters);
            Assert.AreEqual(8, banette.DamageCounters, "both players' Pokémon");
        }

        [Test]
        public void Berserk_Adds_60_Damage_When_Damaged()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hammer Arm"), new ScriptedAgent());
            var ursaring = e.ScenarioActive(0, C("UL-89"));
            Attach(e, ursaring, Psychic, 3);
            ursaring.DamageCounters = 1;
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, chansey.DamageCounters, "30 + 60");
        }

        [Test]
        public void Psychic_Float_Frees_The_Active_Pokemons_Retreat()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("UL-4"));
            e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(chansey);
            Attach(e, chansey, Psychic, 1);
            Assert.Greater(before, 0);
            Assert.AreEqual(0, e.RetreatCostOf(chansey));
        }

        [Test]
        public void Wash_Out_Moves_Water_Energy_To_The_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Wash Out").Then("Use Wash Out"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            var blastoise = e.ScenarioBench(0, C("UL-13"));
            Attach(e, blastoise, Water, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, active.Energy.Count, "a bot moves 1 per use; the power can be used again");
        }

        [Test]
        public void Energy_Signal_Poisons_On_A_Psychic_Attachment()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C("UL-23"));
            e.ScenarioAddToHand(0, C(Psychic));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(chansey.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Pokemon_Circulator_Needs_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("UL-81"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Pokémon Circulator"));
            e.ScenarioBench(1, C(Chansey));
            Assert.IsTrue(Offered(e, 0, "Play Pokémon Circulator"));
        }
    }
}
