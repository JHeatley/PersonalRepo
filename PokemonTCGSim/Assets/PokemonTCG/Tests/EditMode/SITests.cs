using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v140: Southern Islands (SI 1-18): the card data and the attacks, with your Paradise Pollen ruling.</summary>
    public class SITests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Mew = "SI-1";
        const string Onix = "SI-3";
        const string Ivysaur = "SI-5";
        const string Jigglypuff = "SI-8";
        const string Butterfree = "SI-9";
        const string Tentacruel = "SI-10";
        const string Lapras = "SI-12";
        const string Exeggutor = "SI-13";
        const string Slowking = "SI-14";
        const string Lickitung = "SI-16";
        const string Vileplume = "SI-17";
        const string Primeape = "SI-18";
        const string EnergyArk = "N2-75";
        const string Hitmonlee = "HSP-25";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";

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

        [Test]
        public void Southern_Islands_Is_In_The_Pool_As_Unlimited_Cards()
        {
            var set = db.Cards.Where(d => d.SetCode == "SI").ToList();
            Assert.AreEqual(18, set.Count);
            Assert.IsTrue(set.All(d => d.UnlimitedOnly && d.IsPokemon));
            Assert.AreEqual(30, C(Mew).Hp);
            Assert.AreEqual("x", C(Onix).Attacks[0].DamageSuffix, "Slam is 20x");
            Assert.AreEqual(EnergyType.Psychic, C("SI-4").Resistance);
        }

        [Test]
        public void Rainbow_Wave_Hits_Every_Opponents_Pokemon_Of_The_Chosen_Type_Without_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rainbow Wave", "Fighting"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(Mew));
            Attach(e, mew, Psychic, 1);
            Attach(e, mew, Fighting, 1);
            var lee = e.ScenarioActive(1, C(Hitmonlee));
            var onix = e.ScenarioBench(1, C(Onix));
            var blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, lee.DamageCounters, "no Psychic Weakness");
            Assert.AreEqual(2, onix.DamageCounters, "Benched too");
            Assert.AreEqual(0, blissey.DamageCounters, "not a Fighting Pokémon");
        }

        [Test]
        public void Strange_Scent_Heals_3_Counters_For_Each_Player_With_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Strange Scent"), new ScriptedAgent());
            var ivysaur = e.ScenarioActive(0, C(Ivysaur));
            Attach(e, ivysaur, Grass, 1);
            ivysaur.DamageCounters = 1;
            var lee = e.ScenarioBench(0, C(Hitmonlee));
            lee.DamageCounters = 3;
            var blissey = e.ScenarioActive(1, C(Blissey));
            blissey.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.AreEqual(1, ivysaur.DamageCounters + lee.DamageCounters, "3 of your 4 counters");
            Assert.AreEqual(3, blissey.DamageCounters, "the opponent got tails");
        }

        [Test]
        public void Tempting_Eyes_Brings_Up_The_Benched_Pokemon_You_Choose()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tempting Eyes", "Chansey"), new ScriptedAgent());
            var jiggly = e.ScenarioActive(0, C(Jigglypuff));
            Attach(e, jiggly, Psychic, 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioBench(1, C(Hitmonlee));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(chansey, e.State.Players[1].Active);
        }

        [Test]
        public void Rainbow_Powder_Poisons_On_Tails_And_Horrid_Pollen_Confuses()
        {
            foreach (var attacker in new[] { Butterfree, Vileplume })
            {
                bool powder = attacker == Butterfree;
                var e = Scenario(new ScriptedAgent().Then(powder ? "Attack: Rainbow Powder" : "Attack: Horrid Pollen"), new ScriptedAgent());
                var mon = e.ScenarioActive(0, C(attacker));
                Attach(e, mon, Grass, 3);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(false));
                Step(e);
                Assert.IsTrue(blissey.Has(powder ? SpecialCondition.Poisoned : SpecialCondition.Confused), attacker);
                Assert.IsFalse(blissey.Has(SpecialCondition.Paralyzed));
            }
        }

        [Test]
        public void Tentacle_Grip_Draws_2_For_Each_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tentacle Grip"), new ScriptedAgent());
            var tentacruel = e.ScenarioActive(0, C(Tentacruel));
            Attach(e, tentacruel, Water, 2);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true));
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Gentle_Song_Heals_Both_Active_Pokemon_And_Puts_The_Defending_One_To_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gentle Song"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(Lapras));
            Attach(e, lapras, Water, 1);
            lapras.DamageCounters = 3;
            var blissey = e.ScenarioActive(1, C(Blissey));
            blissey.DamageCounters = 1;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, lapras.DamageCounters);
            Assert.AreEqual(0, blissey.DamageCounters, "1 if it only has 1");
            Assert.IsTrue(blissey.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Sharpshooter_Flips_For_Each_Grass_Energy_At_The_Chosen_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sharpshooter", "Chansey"), new ScriptedAgent());
            var exeggutor = e.ScenarioActive(0, C(Exeggutor));
            Attach(e, exeggutor, Grass, 3);
            var blissey = e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true, true, false));
            Step(e);
            Assert.AreEqual(2, chansey.DamageCounters);
            Assert.AreEqual(0, blissey.DamageCounters);
        }

        [Test]
        public void Revelation_Finds_A_Trainer_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Revelation"), new ScriptedAgent());
            var slowking = e.ScenarioActive(0, C(Slowking));
            Attach(e, slowking, Psychic, 1);
            e.ScenarioAddToDeck(0, C(EnergyArk), 1);
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual("Energy Ark", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Lick_Wounds_Can_Heal_The_Opponents_Pokemon_And_Tongue_Stretch_Snipes_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lick Wounds"), new ScriptedAgent());
            var lickitung = e.ScenarioActive(0, C(Lickitung));
            Attach(e, lickitung, Psychic, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            blissey.DamageCounters = 3;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters);

            var other = Scenario(new ScriptedAgent().Then("Attack: Tongue Stretch", "Hitmonlee"), new ScriptedAgent());
            var lick = other.ScenarioActive(0, C(Lickitung));
            Attach(other, lick, Psychic, 2);
            var target = other.ScenarioActive(1, C(Blissey));
            var lee = other.ScenarioBench(1, C(Hitmonlee));
            other.ScenarioSetTurn(0, 3);
            other.State.Rng = new Rng(SeedFor(true));
            Step(other);
            Assert.AreEqual(2, lee.DamageCounters);
            Assert.AreEqual(0, target.DamageCounters);
        }

        [Test]
        public void Paradise_Pollen_Heals_Itself_And_A_Benched_Pokemon_Only_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Paradise Pollen"), new ScriptedAgent());
                var vileplume = e.ScenarioActive(0, C(Vileplume));
                Attach(e, vileplume, Grass, 1);
                vileplume.DamageCounters = 3;
                var lee = e.ScenarioBench(0, C(Hitmonlee));
                lee.DamageCounters = 1;
                e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 1 : 3, vileplume.DamageCounters);
                Assert.AreEqual(heads ? 0 : 1, lee.DamageCounters, "your ruling: the Bench heal only on heads");
            }
        }

        [Test]
        public void Rampage_Adds_10_Per_Counter_And_Confuses_On_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rampage"), new ScriptedAgent());
            var primeape = e.ScenarioActive(0, C(Primeape));
            Attach(e, primeape, Fighting, 2);
            primeape.DamageCounters = 2;
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(8, blissey.DamageCounters, "(20 + 20) x2 for Blissey's Fighting Weakness");
            Assert.IsTrue(primeape.Has(SpecialCondition.Confused));
        }
    }
}
