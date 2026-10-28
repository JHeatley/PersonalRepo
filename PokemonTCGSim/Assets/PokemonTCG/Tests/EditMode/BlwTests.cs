using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v132: Black &amp; White (BLW 1-115): the prints, PlusPower (your ruling: Base Set's card, now legal in Expanded too), Pike and
    /// Flame Burst (your rulings: the Defending Pokémon is hit as well), Constant Rattle, Nature Power, Dream Eater, Electrispark,
    /// Cell Culture, Blindside, Punishment, Shift Gear, Odor Sleuth, Spit Acid, Petal Dance and Magical Leaf.
    /// </summary>
    public class BlwTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string Serperior = "BLW-5";
        const string Petilil = "BLW-9";
        const string Lilligant = "BLW-10";
        const string Maractus = "BLW-12";
        const string Sawsbuck = "BLW-14";
        const string Simisear = "BLW-22";
        const string Samurott = "BLW-31";
        const string Zebstrika = "BLW-43";
        const string Munna = "BLW-48";
        const string Solosis = "BLW-55";
        const string Scrafty = "BLW-69";
        const string Mandibuzz = "BLW-73";
        const string Klinklang = "BLW-76";
        const string Stoutland = "BLW-83";
        const string PlusPower = "BLW-96";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string Hooligans = "DEX-95";

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

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        /// <summary>The attacker (player 0) against Chansey, with Blissey on the opponent's Bench.</summary>
        private GameEngine Attacking(string attacker, string[] labels, out PokemonInPlay mon, out PokemonInPlay chansey, out PokemonInPlay blissey)
        {
            var e = Scenario(new ScriptedAgent().Then(labels), new ScriptedAgent());
            mon = e.ScenarioActive(0, C(attacker));
            chansey = e.ScenarioActive(1, C(Chansey));
            blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            return e;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(76, db.Cards.Count(d => d.SetCode == "BLW"));
            Assert.AreEqual("LTR-8", C("BLW-6").BaseKey, "Serperior (Royal Heal)");
            Assert.AreEqual("LTR-27", C("BLW-20").BaseKey, "Emboar (Inferno Fandango)");
            Assert.AreEqual("CEL-CC20", C("BLW-26").BaseKey, "Reshiram");
            Assert.AreEqual("CEL-CC21", C("BLW-114").BaseKey, "the secret rare Zekrom");
            Assert.AreEqual("NXD-102", C("BLW-71").BaseKey, "Zoroark");
            Assert.AreEqual("EPO-78", C("BLW-78").BaseKey, "Patrat");
            Assert.AreEqual("PLB-84", C("BLW-101").BaseKey, "Professor Juniper");
            Assert.AreEqual("MEE-13", C("BLW-109").BaseKey, "Psychic Energy");
        }

        [Test]
        public void PlusPower_Is_Base_Sets_Card_Legal_In_Expanded_And_Unlimited()
        {
            Assert.AreEqual("BS-84", C(PlusPower).BaseKey);
            foreach (var key in new[] { PlusPower, "BS-84" })
            {
                Assert.IsTrue(Formats.Allows(DeckFormat.Expanded, C(key)), key + " is legal in Expanded (your ruling)");
                Assert.IsTrue(Formats.Allows(DeckFormat.Unlimited, C(key)), key);
                Assert.IsTrue(C(key).IsOldTrainer, key + " is still an old Trainer");
            }
            Assert.IsTrue(C("BS-81").UnlimitedOnly, "Base Set's other Trainers stay Unlimited only");
        }

        // ------------------------------------------------------------------ Grass and Fire

        [Test]
        public void Magical_Leaf_Heals_10_On_Heads()
        {
            PokemonInPlay petilil, chansey, blissey;
            var e = Attacking(Petilil, new[] { "Attack: Magical Leaf" }, out petilil, out chansey, out blissey);
            Attach(e, petilil, Grass, 1);
            petilil.DamageCounters = 2;
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
            Assert.AreEqual(1, petilil.DamageCounters);
        }

        [Test]
        public void Petal_Dance_Confuses_Lilligant()
        {
            PokemonInPlay lilligant, chansey, blissey;
            var e = Attacking(Lilligant, new[] { "Attack: Petal Dance" }, out lilligant, out chansey, out blissey);
            Attach(e, lilligant, Grass, 1);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(6, chansey.DamageCounters);
            Assert.IsTrue(lilligant.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Constant_Rattle_Does_10_30_Or_60()
        {
            foreach (var flips in new[] { new[] { true, false, false }, new[] { true, true, false }, new[] { true, true, true }, new[] { false, false, false } })
            {
                PokemonInPlay maractus, chansey, blissey;
                var e = Attacking(Maractus, new[] { "Attack: Constant Rattle" }, out maractus, out chansey, out blissey);
                Attach(e, maractus, Grass, 1);
                e.State.Rng = new Rng(SeedFor(flips));
                Step(e);
                int heads = flips.Count(f => f);
                Assert.AreEqual(heads == 1 ? 1 : heads == 2 ? 3 : heads == 3 ? 6 : 0, chansey.DamageCounters, heads + " heads");
            }
        }

        [Test]
        public void Nature_Power_Counts_Grass_Energy_On_Both_Sides()
        {
            PokemonInPlay sawsbuck, chansey, blissey;
            var e = Attacking(Sawsbuck, new[] { "Attack: Nature Power" }, out sawsbuck, out chansey, out blissey);
            Attach(e, sawsbuck, Grass, 2);
            Attach(e, blissey, Grass, 1);
            Step(e);
            Assert.AreEqual(5, chansey.DamageCounters, "20 + 3 x 10");
        }

        [Test]
        public void Flame_Burst_Hits_The_Defending_Pokemon_And_2_Benched()
        {
            PokemonInPlay simisear, chansey, blissey;
            var e = Attacking(Simisear, new[] { "Attack: Flame Burst" }, out simisear, out chansey, out blissey);
            Attach(e, simisear, Fire, 1);
            var eevee = e.ScenarioBench(1, C(Eevee));
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
            Assert.AreEqual(2, blissey.DamageCounters);
            Assert.AreEqual(2, eevee.DamageCounters);
        }

        // ------------------------------------------------------------------ Water, Lightning and Psychic

        [Test]
        public void Pike_Hits_The_Defending_Pokemon_And_1_Benched()
        {
            PokemonInPlay samurott, chansey, blissey;
            var e = Attacking(Samurott, new[] { "Attack: Pike", "Blissey" }, out samurott, out chansey, out blissey);
            Attach(e, samurott, Psychic, 2);
            Step(e);
            Assert.AreEqual(3, chansey.DamageCounters, "your ruling: the printed 30 hits the Defending Pokémon");
            Assert.AreEqual(3, blissey.DamageCounters);
        }

        [Test]
        public void Electrispark_Also_Does_10_To_Each_Benched_Pokemon()
        {
            PokemonInPlay zebstrika, chansey, blissey;
            var e = Attacking(Zebstrika, new[] { "Attack: Electrispark" }, out zebstrika, out chansey, out blissey);
            Attach(e, zebstrika, Lightning, 3);
            Step(e);
            Assert.AreEqual(7, chansey.DamageCounters);
            Assert.AreEqual(1, blissey.DamageCounters);
        }

        [Test]
        public void Dream_Eater_Needs_An_Asleep_Defending_Pokemon()
        {
            foreach (bool asleep in new[] { true, false })
            {
                PokemonInPlay munna, chansey, blissey;
                var e = Attacking(Munna, new[] { "Attack: Dream Eater" }, out munna, out chansey, out blissey);
                Attach(e, munna, Psychic, 2);
                if (asleep) e.ApplyCondition(chansey, SpecialCondition.Asleep);
                Step(e);
                Assert.AreEqual(asleep ? 6 : 0, chansey.DamageCounters);
            }
        }

        [Test]
        public void Cell_Culture_Benches_A_Solosis()
        {
            PokemonInPlay solosis, chansey, blissey;
            var e = Attacking(Solosis, new[] { "Attack: Cell Culture" }, out solosis, out chansey, out blissey);
            Attach(e, solosis, Psychic, 1);
            e.ScenarioAddToDeck(0, C(Solosis), 1);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Solosis"));
        }

        // ------------------------------------------------------------------ Darkness and Metal

        [Test]
        public void Spit_Acid_Burns_And_Paralyzes_On_Heads()
        {
            PokemonInPlay scrafty, chansey, blissey;
            var e = Attacking(Scrafty, new[] { "Attack: Spit Acid" }, out scrafty, out chansey, out blissey);
            Attach(e, scrafty, Darkness, 1);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(chansey.Has(SpecialCondition.Burned));
            Assert.IsTrue(chansey.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Blindside_Hits_Only_A_Damaged_Pokemon()
        {
            PokemonInPlay mandibuzz, chansey, blissey;
            var e = Attacking(Mandibuzz, new[] { "Attack: Blindside" }, out mandibuzz, out chansey, out blissey);
            Attach(e, mandibuzz, Darkness, 1);
            blissey.DamageCounters = 1;
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        [Test]
        public void Punishment_Does_60_More_To_A_Stage_2_Pokemon()
        {
            foreach (bool stage2 in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Punishment"), new ScriptedAgent());
                var mandibuzz = e.ScenarioActive(0, C(Mandibuzz));
                Attach(e, mandibuzz, Darkness, 3);
                var defender = e.ScenarioActive(1, C(stage2 ? Serperior : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(stage2 ? 10 : 4, defender.DamageCounters);
            }
        }

        [Test]
        public void Shift_Gear_Moves_A_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shift Gear"), new ScriptedAgent());
            var klinklang = e.ScenarioActive(0, C(Klinklang));
            Attach(e, klinklang, Metal, 1);
            var eevee = e.ScenarioBench(0, C(Eevee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, klinklang.Energy.Count);
            Assert.AreEqual(1, eevee.Energy.Count);
        }

        // ------------------------------------------------------------------ Colorless

        [Test]
        public void Odor_Sleuth_Takes_A_Card_From_The_Discard_Pile_For_Each_Heads()
        {
            PokemonInPlay stoutland, chansey, blissey;
            var e = Attacking(Stoutland, new[] { "Attack: Odor Sleuth" }, out stoutland, out chansey, out blissey);
            Attach(e, stoutland, Psychic, 2);
            e.ScenarioAddToDiscard(0, C(Hooligans), 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Key == Hooligans));
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Key == Hooligans));
        }
    }
}
