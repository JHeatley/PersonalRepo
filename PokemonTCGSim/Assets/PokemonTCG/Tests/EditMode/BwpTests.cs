using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v133: BW Black Star Promos (BWP BW01-BW101): the prints, Tropical Beach, Battle City, Victory Cup, Justified, Weak Barrier,
    /// Swords Dance, Random Spark (one hit, by your Telekinesis ruling), Dragon Rage, Cold Crush, Electricannon, Intuition and Psyscan.
    /// </summary>
    public class BwpTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";

        const string Axew = "BWP-10";
        const string TropicalBeach = "BWP-28";
        const string VictoryCup = "BWP-29";
        const string Luxio = "BWP-34";
        const string BattleCity = "BWP-39";
        const string Elgyem = "BWP-55";
        const string Keldeo = "BWP-60";
        const string Ampharos = "BWP-67";
        const string Virizion = "BWP-70";
        const string Riolu = "BWP-33";
        const string Lucario = "BWP-85";
        const string Mew = "BWP-98";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string Woobat = "EPO-36";
        const string DarkraiEx = "LTR-88";

        private static int SeedFor(params bool[] flips)
        {
            for (int seed = 1; seed < 100000; seed++)
            {
                var rng = new Rng(seed);
                bool match = true;
                foreach (var heads in flips)
                    if (rng.CoinFlip() != heads) { match = false; break; }
                if (match) return seed;
            }
            throw new InvalidOperationException("no seed for those flips");
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Promos_Are_In_The_Pool_With_Their_Prints_Linked()
        {
            Assert.AreEqual(45, db.Cards.Count(d => d.SetCode == "BWP"));
            Assert.AreEqual(VictoryCup, C("BWP-30").BaseKey, "the 2nd place Victory Cup");
            Assert.AreEqual(VictoryCup, C("BWP-31").BaseKey, "the 3rd place Victory Cup");
            Assert.AreEqual(TropicalBeach, C("BWP-50").BaseKey);
            Assert.AreEqual(Axew, C("BWP-16").BaseKey);
            Assert.AreEqual("BWP-99", C("BWP-101").BaseKey, "Genesect");
            Assert.AreEqual("SP-296", C("BWP-95").BaseKey, "Champions Festival");
            Assert.AreEqual("30C-CC18", C("BWP-100").BaseKey, "N");
            Assert.AreEqual("CEL-CC22", C("BWP-45").BaseKey, "Mewtwo-EX");
            Assert.AreEqual("LTR-88", C("BWP-46").BaseKey, "Darkrai-EX");
            Assert.AreEqual(0, C(Ampharos).Attacks[0].Damage, "Random Spark: one hit to the chosen Pokémon");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Tropical_Beach_Draws_Up_To_7_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Tropical Beach"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetStadium(0, C(TropicalBeach));
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "the turn ends");
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Battle_City_Draws_A_Card_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Battle City"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetStadium(0, C(BattleCity));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 0, e.State.Players[0].Hand.Count);
                Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Battle City")), "once a turn");
            }
        }

        [Test]
        public void Every_Victory_Cup_Finds_A_Pokemon_On_Heads()
        {
            foreach (var key in new[] { VictoryCup, "BWP-30", "BWP-31" })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Victory Cup"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioAddToHand(0, C(key));
                e.ScenarioAddToDeck(0, C(Eevee), 1);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Eevee), key);
            }
        }

        // ------------------------------------------------------------------ Pokémon

        [Test]
        public void Random_Spark_Hits_Only_The_Chosen_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Random Spark", "Blissey"), new ScriptedAgent());
            var luxio = e.ScenarioActive(0, C(Luxio));
            Attach(e, luxio, Lightning, 3);
            var chansey = e.ScenarioActive(1, C(Chansey));
            var blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, blissey.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        [Test]
        public void Dragon_Rage_Needs_Two_Heads()
        {
            foreach (bool both in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Rage"), new ScriptedAgent());
                var axew = e.ScenarioActive(0, C(Axew));
                Attach(e, axew, Psychic, 2);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(both ? SeedFor(true, true) : SeedFor(true, false));
                Step(e);
                Assert.AreEqual(both ? 5 : 0, chansey.DamageCounters);
            }
        }

        [Test]
        public void Justified_Adds_50_Against_Darkness_Pokemon()
        {
            foreach (bool dark in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Leaf Blade"), new ScriptedAgent());
                var virizion = e.ScenarioActive(0, C(Virizion));
                Attach(e, virizion, Grass, 3);
                var defender = e.ScenarioActive(1, C(dark ? DarkraiEx : Chansey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(false));
                Step(e);
                Assert.AreEqual(dark ? 10 : 5, defender.DamageCounters, dark ? "50 + 50 against a Darkness Pokémon" : "50");
            }
        }

        [Test]
        public void Weak_Barrier_Removes_Weakness_With_Psychic_Energy()
        {
            foreach (bool psychic in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Psy Bolt"), new ScriptedAgent());
                var woobat = e.ScenarioActive(0, C(Woobat));
                Attach(e, woobat, Psychic, 2);
                var elgyem = e.ScenarioActive(1, C(Elgyem));
                if (psychic) Attach(e, elgyem, Psychic, 1);
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(false));
                Step(e);
                Assert.AreEqual(psychic ? 2 : 4, elgyem.DamageCounters, psychic ? "no Weakness" : "20 x2 Psychic Weakness");
            }
        }

        [Test]
        public void Swords_Dance_Makes_Aqua_Blade_Do_120_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Swords Dance", "Attack: Aqua Blade"), new ScriptedAgent());
            var keldeo = e.ScenarioActive(0, C(Keldeo));
            Attach(e, keldeo, Water, 3);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters);
        }

        [Test]
        public void Electricannon_May_Discard_Lightning_Energy_For_60_More()
        {
            foreach (string answer in new[] { "Yes", "No" })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Electricannon", answer), new ScriptedAgent());
                var ampharos = e.ScenarioActive(0, C(Ampharos));
                Attach(e, ampharos, Lightning, 3);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(answer == "Yes" ? 12 : 6, blissey.DamageCounters);
                Assert.AreEqual(answer == "Yes" ? 0 : 3, ampharos.Energy.Count);
            }
        }

        [Test]
        public void Intuition_Draws_2_When_Evolving_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes"), new ScriptedAgent());
            var riolu = e.ScenarioActive(0, C(Riolu));
            e.ScenarioAddToHand(0, C(Lucario));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            riolu.TurnEnteredPlay = 1;
            Step(e);
            Assert.AreEqual("Lucario", riolu.Name);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Psyscan_Works_Only_From_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Mew));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Psyscan")));
            var bench = Scenario(new ScriptedAgent(), new ScriptedAgent());
            bench.ScenarioActive(0, C(Chansey));
            bench.ScenarioBench(0, C(Mew));
            bench.ScenarioActive(1, C(Chansey));
            bench.ScenarioAddToHand(1, C(Psychic));
            bench.ScenarioSetTurn(0, 3);
            Assert.IsFalse(bench.GetMainActions(0).Any(o => o.Label.Contains("Psyscan")));
        }
    }
}
