using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v130: Noble Victories (NVI 1-102): the prints (your ruling: Eelektrik's Dynamotor card is its own card), Solid Rock, Cursed
    /// Body, Durable Body, Craftsmanship, Dynamotor, Mysterious Evolution (your ruling: either side's partner counts), V-create,
    /// Damagriiigus, Core Cannon, Devour, Synchronoise, Frost Vanish and Xtransceiver.
    /// </summary>
    public class NviTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Karrablast = "NVI-8";
        const string Shelmet = "NVI-11";
        const string VictiniV = "NVI-15";
        const string Carracosta = "NVI-26";
        const string Jellicent = "NVI-31";
        const string Cryogonal = "NVI-33";
        const string Eelektrik = "NVI-40";
        const string Cofagrigus = "NVI-46";
        const string DurableCofagrigus = "NVI-47";
        const string Beheeyem = "NVI-56";
        const string Gigalith = "NVI-61";
        const string Conkeldurr = "NVI-64";
        const string Escavalier = "NVI-80";
        const string Durant = "NVI-83";
        const string Xtransceiver = "NVI-96";
        const string Pawniard = "DEX-78";
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

        /// <summary>Pawniard (Metal) attacks with Scratch for 20.</summary>
        private GameEngine Scratching(out PokemonInPlay pawniard)
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Scratch"), new ScriptedAgent());
            pawniard = e.ScenarioActive(0, C(Pawniard));
            Attach(e, pawniard, Metal, 2);
            e.ScenarioSetTurn(0, 3);
            return e;
        }

        // ------------------------------------------------------------------ The set

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            Assert.AreEqual(71, db.Cards.Count(d => d.SetCode == "NVI"));
            Assert.AreEqual("LTR-9", C("NVI-1").BaseKey, "Sewaddle");
            Assert.AreEqual("NXD-101", C("NVI-60").BaseKey, "Chandelure");
            Assert.AreEqual("DEX-110", C("NVI-67").BaseKey, "Archeops");
            Assert.AreEqual("KSS-19", C("NVI-81").BaseKey, "Pawniard");
            Assert.AreEqual("PAL-188", C("NVI-95").BaseKey, "Super Rod");
            Assert.AreEqual("30C-CC18", C("NVI-101").BaseKey, "the full art N");
            Assert.AreEqual(Eelektrik, C(Eelektrik).BaseKey, "its own card, not Black Bolt's (your ruling)");
            Assert.AreEqual(Stage.Restored, C("NVI-25").Stage);
            Assert.AreEqual("Cover Fossil", C("NVI-25").EvolvesFrom);
            Assert.AreEqual("Plume Fossil", C("NVI-66").EvolvesFrom);
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Solid_Rock_Takes_50_Less_On_Heads()
        {
            foreach (bool heads in new[] { true, false })
            {
                PokemonInPlay pawniard;
                var e = Scratching(out pawniard);
                var carracosta = e.ScenarioActive(1, C(Carracosta));
                e.State.Rng = new Rng(SeedFor(heads));
                Step(e);
                Assert.AreEqual(heads ? 0 : 2, carracosta.DamageCounters);
            }
        }

        [Test]
        public void Cursed_Body_Confuses_The_Attacker()
        {
            PokemonInPlay pawniard;
            var e = Scratching(out pawniard);
            e.ScenarioActive(1, C(Jellicent));
            Step(e);
            Assert.IsTrue(pawniard.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Durable_Body_Leaves_10_HP_On_Heads()
        {
            PokemonInPlay pawniard;
            var e = Scratching(out pawniard);
            var cofagrigus = e.ScenarioActive(1, C(DurableCofagrigus));
            cofagrigus.DamageCounters = 8;
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[1].InPlay().Contains(cofagrigus));
            Assert.AreEqual(10, e.RemainingHp(cofagrigus));
        }

        [Test]
        public void Craftsmanship_Adds_20_HP_For_Each_Fighting_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var conkeldurr = e.ScenarioActive(0, C(Conkeldurr));
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(140, e.MaxHp(conkeldurr));
            Attach(e, conkeldurr, Fighting, 2);
            Attach(e, conkeldurr, Psychic, 1);
            Assert.AreEqual(180, e.MaxHp(conkeldurr));
        }

        [Test]
        public void Dynamotor_Attaches_A_Lightning_Energy_From_The_Discard_Pile_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dynamotor"), new ScriptedAgent());
            e.ScenarioActive(0, C(Eelektrik));
            var eevee = e.ScenarioBench(0, C(Eevee));
            e.ScenarioAddToDiscard(0, C(Lightning), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, eevee.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Mysterious_Evolution_Needs_A_Shelmet_In_Play_On_Either_Side()
        {
            foreach (bool shelmet in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Mysterious Evolution"), new ScriptedAgent());
                var karrablast = e.ScenarioActive(0, C(Karrablast));
                Attach(e, karrablast, Psychic, 1);
                e.ScenarioAddToDeck(0, C(Escavalier), 1);
                e.ScenarioActive(1, C(shelmet ? Shelmet : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(shelmet ? "Escavalier" : "Karrablast", karrablast.Name, shelmet ? "the opponent's Shelmet counts (your ruling)" : "no Shelmet");
            }
        }

        [Test]
        public void V_Create_Needs_5_Benched_Pokemon()
        {
            foreach (int bench in new[] { 4, 5 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: V-create"), new ScriptedAgent());
                var victini = e.ScenarioActive(0, C(VictiniV));
                Attach(e, victini, "MEE-10", 1);
                Attach(e, victini, Psychic, 1);
                for (int i = 0; i < bench; i++) e.ScenarioBench(0, C(Eevee));
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(bench == 5 ? 10 : 0, blissey.DamageCounters);
            }
        }

        [Test]
        public void Damagriiigus_Moves_All_Counters_From_A_Benched_Pokemon_To_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Damagriiigus"), new ScriptedAgent());
            var cofagrigus = e.ScenarioActive(0, C(Cofagrigus));
            Attach(e, cofagrigus, Psychic, 2);
            var eevee = e.ScenarioBench(0, C(Eevee));
            eevee.DamageCounters = 3;
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, eevee.DamageCounters);
            Assert.AreEqual(3, chansey.DamageCounters);
        }

        [Test]
        public void Core_Cannon_Does_20_For_Each_Fighting_Energy_To_Any_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Core Cannon", "Blissey"), new ScriptedAgent());
            var gigalith = e.ScenarioActive(0, C(Gigalith));
            Attach(e, gigalith, Fighting, 3);
            e.ScenarioActive(1, C(Chansey));
            var blissey = e.ScenarioBench(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, blissey.DamageCounters);
        }

        [Test]
        public void Devour_Discards_A_Card_For_Each_Of_Your_Durant()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Devour"), new ScriptedAgent());
            var durant = e.ScenarioActive(0, C(Durant));
            Attach(e, durant, Metal, 1);
            e.ScenarioBench(0, C(Durant));
            e.ScenarioBench(0, C(Durant));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(17, e.State.Players[1].Deck.Count);
            Assert.AreEqual(3, e.State.Players[1].Discard.Count);
        }

        [Test]
        public void Synchronoise_Hits_Benched_Pokemon_That_Share_A_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Synchronoise"), new ScriptedAgent());
            var beheeyem = e.ScenarioActive(0, C(Beheeyem));
            Attach(e, beheeyem, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            var eevee = e.ScenarioBench(1, C(Eevee));
            var pawniard = e.ScenarioBench(1, C(Pawniard));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, eevee.DamageCounters, "Colorless, like Chansey");
            Assert.AreEqual(0, pawniard.DamageCounters, "Metal");
        }

        [Test]
        public void Frost_Vanish_May_Return_Cryogonal_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frost Vanish", "Yes"), new ScriptedAgent());
            var cryogonal = e.ScenarioActive(0, C(Cryogonal));
            Attach(e, cryogonal, Water, 2);
            e.ScenarioBench(0, C(Eevee));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, chansey.DamageCounters);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Cryogonal));
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Key == Water), "with its Energy");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Xtransceiver_Finds_A_Supporter_On_Heads()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Xtransceiver"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Xtransceiver));
            e.ScenarioAddToDeck(0, C(Hooligans), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Hooligans));
        }
    }
}
