using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v155: FLO (Sigilyph ex, Riolu, Mega Lucario Z ex) and the MEGA x MEGA Parade (M7A) Mega Attack Rare prints.</summary>
    public class FloTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Metal = "MEE-16";
        const string Chansey = "E2-69";
        const string MegaDragonite = "MEP-91";
        const string Sigilyph = "FLO-32";
        const string Lucario = "FLO-58";
        const string Grisham = "DLR-93";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void FLO_Cards_And_The_MEGA_x_MEGA_Parade_Prints()
        {
            Assert.AreEqual(3, db.Cards.Count(d => d.SetCode == "FLO"));
            Assert.AreEqual("Flowing Fists", C(Sigilyph).PrintSetName, "v157.1: the set's name is out");
            var lucario = C(Lucario);
            Assert.IsTrue(lucario.IsMegaEx && lucario.IsEx);
            Assert.AreEqual("Riolu", lucario.EvolvesFrom);
            Assert.AreEqual(3, lucario.PrizeValue);
            Assert.AreEqual(2, C(Sigilyph).PrizeValue);
            Assert.AreEqual("MEP-81", db.MainPrinting(C("M7A-208")).Key, "Mega Greninja ex");
            Assert.AreEqual("MEP-71", db.MainPrinting(C("M7A-214")).Key, "Mega Zygarde ex");
            Assert.AreEqual("MEP-90", db.MainPrinting(C("M7A-217")).Key, "Mega Darkrai ex");
            Assert.AreEqual("DLR-84", db.MainPrinting(C("M7A-218")).Key, "Mega Rayquaza ex");
        }

        [Test]
        public void Mega_Protection_Stops_Damage_From_Mega_Evolution_Pokemon_ex_And_Dancing_Fist_Hits_ex_Harder()
        {
            foreach (bool sigilyph in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Dancing Fist"), new ScriptedAgent());
                var lucario = e.ScenarioActive(0, C(Lucario));
                Attach(e, lucario, Metal, 2);
                var target = e.ScenarioActive(1, C(sigilyph ? Sigilyph : MegaDragonite));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(sigilyph ? 0 : 24, target.DamageCounters, sigilyph ? "Mega Protection" : "120 + 120 against a Pokémon ex");
            }
        }

        [Test]
        public void Aura_Seeker_Takes_A_Supporter_While_Active()
        {
            foreach (bool active in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Use Aura Seeker"), new ScriptedAgent());
                if (active) e.ScenarioActive(0, C(Lucario));
                else
                {
                    e.ScenarioActive(0, C(Chansey));
                    e.ScenarioBench(0, C(Lucario));
                }
                e.ScenarioAddToDeck(0, C(Grisham), 1);
                e.ScenarioAddToDeck(0, C(Psychic), 3);
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(active, Offered(e, 0, "Use Aura Seeker"));
                if (!active) continue;
                Step(e);
                Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Grisham"));
                Assert.IsFalse(Offered(e, 0, "Use Aura Seeker"), "once a turn");
            }
        }

        [Test]
        public void Power_Cyclone_Moves_An_Energy_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Power Cyclone"), new ScriptedAgent());
            var sigilyph = e.ScenarioActive(0, C(Sigilyph));
            Attach(e, sigilyph, Psychic, 3);
            var bench = e.ScenarioBench(0, C(Chansey));
            var dragonite = e.ScenarioActive(1, C(MegaDragonite));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(14, dragonite.DamageCounters);
            Assert.AreEqual(2, sigilyph.Energy.Count);
            Assert.AreEqual(1, bench.Energy.Count);
        }
    }
}
