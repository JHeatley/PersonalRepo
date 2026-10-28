using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v145: Best of Game (BG): 5 prints of pool cards and 4 new cards (Rocket's Scizor, Rocket's Sneasel, Dark Ivysaur, Dark
    /// Venusaur), with your ruling that Vine Pull is a Poké-Body.
    /// </summary>
    public class BgTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Grass = "MEE-9";
        const string Metal = "MEE-16";
        const string Scizor = "BG-4";
        const string DarkIvysaur = "BG-6";
        const string DarkVenusaur = "BG-7";
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
        public void Best_Of_Game_Is_4_New_Cards_And_5_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "BG").ToList();
            Assert.AreEqual(4, set.Count);
            Assert.IsTrue(set.All(d => d.UnlimitedOnly));
            Assert.AreEqual(9, db.All.Count(d => d.SetCode == "BG"));
            Assert.AreEqual("BS-20", db.MainPrinting(db.Get("BG-1")).Key);
            Assert.AreEqual("BS-7", db.MainPrinting(db.Get("BG-2")).Key, "Hitmonchan: 70 HP on Limitless (the data file's 60 is wrong)");
            Assert.AreEqual("N1-96", db.MainPrinting(db.Get("BG-3")).Key);
            Assert.AreEqual("G2-14", db.MainPrinting(db.Get("BG-8")).Key);
            Assert.AreEqual("G1-11", db.MainPrinting(db.Get("BG-9")).Key);
            Assert.AreEqual(AbilityDef.PokeBody, C(DarkIvysaur).Abilities[0].Kind, "your ruling");
            Assert.AreEqual("Dark Ivysaur", C(DarkVenusaur).EvolvesFrom);
        }

        [Test]
        public void Vine_Pull_Switches_In_An_Opposing_Benched_Pokemon_When_It_Retreats()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var ivysaur = e.ScenarioActive(0, C(DarkIvysaur));
            Attach(e, ivysaur, Grass, 2);
            e.ScenarioBench(0, C(Hitmonlee));
            e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(ivysaur));
            Assert.AreSame(chansey, e.State.Players[1].Active);
        }

        [Test]
        public void Fury_Strikes_Hits_Where_The_Opponent_Puts_The_Markers()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fury Strikes"), new ScriptedAgent().Then("Chansey", "Chansey", "Blissey"));
            var ivysaur = e.ScenarioActive(0, C(DarkIvysaur));
            Attach(e, ivysaur, Grass, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            var chansey = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters);
            Assert.AreEqual(2, chansey.DamageCounters);
        }

        [Test]
        public void Focus_Doubles_Agilitys_Base_Damage_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Focus", "Attack: Agility"), new ScriptedAgent());
            var scizor = e.ScenarioActive(0, C(Scizor));
            Attach(e, scizor, Metal, 3);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            e.State.Rng = new Rng(SeedFor(false));
            Step(e);
            Assert.AreEqual(4, blissey.DamageCounters);
        }

        [Test]
        public void Horrid_Pollen_Always_Poisons()
        {
            foreach (int heads in new[] { 0, 1, 2 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Horrid Pollen"), new ScriptedAgent());
                var venusaur = e.ScenarioActive(0, C(DarkVenusaur));
                Attach(e, venusaur, Grass, 3);
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                e.State.Rng = new Rng(SeedFor(heads >= 1, heads == 2));
                Step(e);
                Assert.IsTrue(blissey.Has(SpecialCondition.Poisoned));
                var expected = heads == 1 ? SpecialCondition.Asleep : heads == 2 ? SpecialCondition.Confused : SpecialCondition.Paralyzed;
                Assert.IsTrue(blissey.Has(expected), heads + " heads");
            }
        }
    }
}
