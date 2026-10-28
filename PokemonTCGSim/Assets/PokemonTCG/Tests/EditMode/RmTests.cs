using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v148: Pokémon Rumble (RM): the 16 cards, Giga Drain, Heat Boiler, the recoil attacks and Rage.</summary>
    public class RmTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Darkness = "MEE-15";

        const string Venusaur = "RM-1";
        const string Heatran = "RM-4";
        const string Pikachu = "RM-7";
        const string Skuntank = "RM-13";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void Pokemon_Rumble_Is_16_New_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "RM").ToList();
            Assert.AreEqual(16, set.Count);
            Assert.AreEqual(16, db.All.Count(d => d.SetCode == "RM"));
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual(20, C("RM-2").ResistanceAmount, "DP-era Resistance -20");
            Assert.AreEqual("Ivysaur", C(Venusaur).EvolvesFrom);
        }

        [Test]
        public void Giga_Drain_Heals_The_Damage_It_Did()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Giga Drain"), new ScriptedAgent());
            var venusaur = e.ScenarioActive(0, C(Venusaur));
            Attach(e, venusaur, Grass, 4);
            venusaur.DamageCounters = 8;
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, chansey.DamageCounters);
            Assert.AreEqual(3, venusaur.DamageCounters);
        }

        [Test]
        public void Heat_Boiler_Does_40_More_To_A_Water_Pokemon()
        {
            foreach (bool water in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Heat Boiler"), new ScriptedAgent());
                var heatran = e.ScenarioActive(0, C(Heatran));
                Attach(e, heatran, Fire, 4);
                var target = e.ScenarioActive(1, C(water ? "E2-19" : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(water ? 10 : 6, target.DamageCounters);
            }
        }

        [Test]
        public void Volt_Tackle_Hurts_Pikachu_And_Rage_Counts_Its_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Volt Tackle"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            Attach(e, pikachu, Lightning, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, chansey.DamageCounters);
            Assert.AreEqual(1, pikachu.DamageCounters);

            var f = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var skuntank = f.ScenarioActive(0, C(Skuntank));
            Attach(f, skuntank, Darkness, 3);
            skuntank.DamageCounters = 3;
            var target = f.ScenarioActive(1, C(Chansey));
            f.ScenarioSetTurn(0, 3);
            Step(f);
            Assert.AreEqual(5, target.DamageCounters, "20 + 3 x 10");
        }
    }
}
