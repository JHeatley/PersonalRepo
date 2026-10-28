using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v116: Fossil (FO 1-62): Toxic Gas, Prehistoric Power, Transform, Clairvoyance, Transparency, Kabuto Armor, Step In, Curse,
    /// Strange Behavior, Cowardice, Mysterious Fossil, Gigashock, Wildfire, Energy Conversion and Mr. Fuji.
    /// </summary>
    public class FoTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";

        const string Dragonair = "BS-18";
        const string Dratini = "BS-26";
        const string Charmeleon = "BS-24";
        const string Charmander = "BS-46";
        const string Gastly = "BS-50";
        const string Pikachu = "BS-58";
        const string Rattata = "BS-61";
        const string Alakazam = "BS-1";
        const string Kadabra = "BS-32";
        const string Abra = "BS-43";
        const string MrMime = "JU-6";
        const string Pinsir = "JU-9";
        const string Snorlax = "JU-11";

        const string Aerodactyl = "FO-1";
        const string Ditto = "FO-3";
        const string Dragonite = "FO-4";
        const string Gengar = "FO-5";
        const string Haunter = "FO-6";
        const string Moltres = "FO-12";
        const string Muk = "FO-13";
        const string Raichu = "FO-14";
        const string GastlyFO = "FO-33";
        const string Slowbro = "FO-43";
        const string Grimer = "FO-48";
        const string Kabuto = "FO-50";
        const string Omanyte = "FO-52";
        const string Slowpoke = "FO-55";
        const string Tentacool = "FO-56";
        const string MrFuji = "FO-58";
        const string MysteriousFossil = "FO-62";

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

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(44, db.Cards.Count(d => d.SetCode == "FO"));
            Assert.AreEqual(Aerodactyl, C("FO-16").BaseKey, "the non-holo Aerodactyl");
            Assert.AreEqual("WP-20", C("FO-53").BaseKey, "Psyduck");
            Assert.AreEqual("GEN-32", C(Slowpoke).BaseKey, "Slowpoke");
            Assert.AreEqual("POR-72", C("FO-59").BaseKey, "Energy Search");
            Assert.IsTrue(C(MysteriousFossil).IsFossil);
            Assert.AreEqual(10, C(MysteriousFossil).Hp);
            Assert.AreEqual("Mysterious Fossil", C(Omanyte).EvolvesFrom);
        }

        [Test]
        public void Toxic_Gas_Switches_Off_Other_Pokemon_Powers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C(MrMime));
            var rattata = e.ScenarioActive(1, C(Rattata));
            var muk = e.ScenarioBench(1, C(Grimer));
            e.ScenarioEvolve(muk, C(Muk));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, mime, 30);
            Assert.AreEqual(3, mime.DamageCounters, "Invisible Wall is ignored");
            Assert.IsTrue(e.AbilityBlocked(mime, "Invisible Wall"));
            Assert.IsFalse(e.AbilityBlocked(muk, "Toxic Gas"));
        }

        [Test]
        public void Prehistoric_Power_Stops_Evolving()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Charmander));
            e.ScenarioAddToHand(0, C(Charmeleon));
            e.ScenarioActive(1, C(Rattata));
            var aerodactyl = e.ScenarioBench(1, C(MysteriousFossil));
            e.ScenarioEvolve(aerodactyl, C(Aerodactyl));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Evolve")));
            aerodactyl.Conditions = SpecialCondition.Asleep;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Evolve")), "the power stops while Aerodactyl is Asleep");
        }

        [Test]
        public void Mysterious_Fossil_Evolves_Into_Omanyte()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var fossil = e.ScenarioBench(0, C(MysteriousFossil));
            e.ScenarioAddToHand(0, C(Omanyte));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(10, e.MaxHp(fossil));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Evolve") && o.Label.Contains("Omanyte")));
        }

        [Test]
        public void Transform_Copies_The_Opponents_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            e.ScenarioAttach(ditto, C(Psychic));
            e.ScenarioAttach(ditto, C(Psychic));
            e.ScenarioActive(1, C(Pinsir));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(60, e.MaxHp(ditto));
            Assert.IsTrue(e.IsType(ditto, EnergyType.Grass));
            Assert.AreEqual(EnergyType.Fire, e.WeaknessOf(ditto));
            Assert.IsTrue(e.UsableAttacks(ditto).Any(a => a.Attack.Name == "Irongrip"), "Psychic Energy pays [G][G]");
            ditto.Conditions = SpecialCondition.Confused;
            Assert.AreEqual(50, e.MaxHp(ditto), "not a copy while Confused");
        }

        [Test]
        public void Transform_Also_Copies_Pokemon_Powers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(1, 4);
            e.ApplyCondition(ditto, SpecialCondition.Poisoned);
            Assert.IsFalse(ditto.Has(SpecialCondition.Poisoned), "Thick Skinned, your ruling (v116)");
        }

        [Test]
        public void Clairvoyance_Shows_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var omanyte = e.ScenarioBench(0, C(MysteriousFossil));
            e.ScenarioEvolve(omanyte, C(Omanyte));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.HandFaceUp(1));
            Assert.IsFalse(e.HandFaceUp(0));
            Assert.AreEqual(1, PlayerView.Create(e, 0).Sides[1].Hand.Count);
        }

        [Test]
        public void Transparency_Heads_Stops_The_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bite"), new ScriptedAgent());
            var rattata = e.ScenarioActive(0, C(Rattata));
            e.ScenarioAttach(rattata, C(Psychic));
            var haunter = e.ScenarioActive(1, C(Gastly));
            e.ScenarioEvolve(haunter, C(Haunter));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.AreEqual(0, haunter.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Bite"), new ScriptedAgent());
            var rattata2 = e2.ScenarioActive(0, C(Rattata));
            e2.ScenarioAttach(rattata2, C(Psychic));
            var haunter2 = e2.ScenarioActive(1, C(Gastly));
            e2.ScenarioEvolve(haunter2, C(Haunter));
            e2.ScenarioSetTurn(0, 3);
            e2.State.Rng = new Rng(SeedFor(false));
            Step(e2);
            Assert.AreEqual(2, haunter2.DamageCounters);
        }

        [Test]
        public void Kabuto_Armor_Halves_Attack_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kabuto = e.ScenarioActive(0, C(MysteriousFossil));
            e.ScenarioEvolve(kabuto, C(Kabuto));
            var rattata = e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(rattata, kabuto, 30);
            Assert.AreEqual(1, kabuto.DamageCounters, "15 rounded down to 10");
        }

        [Test]
        public void Step_In_Moves_Dragonite_To_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Step In"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var dragonite = e.ScenarioBench(0, C(Dratini));
            e.ScenarioEvolve(dragonite, C(Dragonair));
            e.ScenarioEvolve(dragonite, C(Dragonite));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(dragonite, e.State.Players[0].Active);
        }

        [Test]
        public void Curse_Moves_A_Counter_Between_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Curse"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C(Gastly));
            e.ScenarioEvolve(gengar, C(Haunter));
            e.ScenarioEvolve(gengar, C(Gengar));
            var rattata = e.ScenarioActive(1, C(Rattata));
            rattata.DamageCounters = 1;
            var pinsir = e.ScenarioBench(1, C(Pinsir));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, rattata.DamageCounters);
            Assert.AreEqual(1, pinsir.DamageCounters);
        }

        [Test]
        public void Strange_Behavior_Moves_Counters_Onto_Slowbro()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Strange Behavior"), new ScriptedAgent());
            var slowbro = e.ScenarioActive(0, C(Slowpoke));
            e.ScenarioEvolve(slowbro, C(Slowbro));
            var rattata = e.ScenarioBench(0, C(Rattata));
            rattata.DamageCounters = 2;
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, slowbro.DamageCounters);
            Assert.AreEqual(1, rattata.DamageCounters);
        }

        [Test]
        public void Cowardice_Returns_Tentacool_And_Discards_Its_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Cowardice"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var tentacool = e.ScenarioBench(0, C(Tentacool));
            e.ScenarioAttach(tentacool, C(Psychic));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(0, p.Bench.Count);
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Tentacool"));
            Assert.IsTrue(p.Discard.Any(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Gigashock_Hits_The_Opponents_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gigashock"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(Pikachu));
            e.ScenarioEvolve(raichu, C(Raichu));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(raichu, C(Lightning));
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            var a = e.ScenarioBench(1, C(Rattata));
            var b = e.ScenarioBench(1, C(Pinsir));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, snorlax.DamageCounters);
            Assert.AreEqual(1, a.DamageCounters);
            Assert.AreEqual(1, b.DamageCounters);
        }

        [Test]
        public void Wildfire_Mills_One_Card_For_Each_Fire_Energy_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wildfire"), new ScriptedAgent());
            var moltres = e.ScenarioActive(0, C(Moltres));
            e.ScenarioAttach(moltres, C(Fire));
            e.ScenarioAttach(moltres, C(Fire));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, moltres.Energy.Count, "the bot discards 1");
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Energy_Conversion_Takes_Energy_And_Hurts_Gastly()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Energy Conversion"), new ScriptedAgent());
            var gastly = e.ScenarioActive(0, C(GastlyFO));
            e.ScenarioAttach(gastly, C(Psychic));
            e.ScenarioAttach(gastly, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(1, gastly.DamageCounters);
        }

        [Test]
        public void Mr_Fuji_Shuffles_A_Benched_Pokemon_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mr. Fuji"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rattata));
            var benched = e.ScenarioBench(0, C(Rattata));
            e.ScenarioAttach(benched, C(Psychic));
            e.ScenarioAddToHand(0, C(MrFuji));
            e.ScenarioActive(1, C(Rattata));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(22, e.State.Players[0].Deck.Count);
        }
    }
}
