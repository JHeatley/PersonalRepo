using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v143: Legendary Collection (LC): 109 reprints filed as prints of the cards already in the pool (your ruling: the old Trainers
    /// are the Base Set cards, Mysterious Fossil is Fossil's 10-HP card) and the one new card, Gastly 76 (Lick costs [C]).
    /// </summary>
    public class LcTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string GastlyLC = "LC-76";
        const string GastlyFO = "FO-33";
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

        [Test]
        public void Legendary_Collection_Is_Gastly_Plus_109_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "LC").ToList();
            Assert.AreEqual(1, set.Count);
            Assert.AreEqual("Gastly", set[0].Name);
            Assert.IsTrue(set[0].UnlimitedOnly);
            for (int i = 1; i <= 110; i++) Assert.IsNotNull(db.Get("LC-" + i), "LC " + i);
            Assert.AreEqual(110, db.All.Count(d => d.SetCode == "LC"));
        }

        [TestCase("LC-102", "BS-76")]
        [TestCase("LC-107", "BS-81")]
        [TestCase("LC-108", "BS-91")]
        [TestCase("LC-110", "BS-94")]
        [TestCase("LC-109", "FO-62")]
        [TestCase("LC-29", "WP-12")]
        [TestCase("LC-3", "30C-CC2")]
        [TestCase("LC-82", "JU-57")]
        public void Reprints_Are_Prints_Of_The_Pool_Cards(string print, string main)
        {
            Assert.AreEqual(main, db.MainPrinting(db.Get(print)).Key);
        }

        [Test]
        public void Gastly_Licks_With_Any_Energy_Unlike_Fossils()
        {
            foreach (bool lc in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Lick"), new ScriptedAgent());
                var gastly = e.ScenarioActive(0, C(lc ? GastlyLC : GastlyFO));
                e.ScenarioAttach(gastly, C(Fire));
                var blissey = e.ScenarioActive(1, C(Blissey));
                e.ScenarioSetTurn(0, 3);
                bool offered = e.GetMainActions(0).Any(o => o.Label.Contains("Attack: Lick"));
                Assert.AreEqual(lc, offered, "Fossil's Lick costs [P]");
                if (!lc) continue;
                e.State.Rng = new Rng(SeedFor(true));
                Step(e);
                Assert.AreEqual(1, blissey.DamageCounters);
                Assert.IsTrue(blissey.Has(SpecialCondition.Paralyzed));
            }
        }
    }
}
