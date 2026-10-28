using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v168: Unseen Forces (UF): the card data and prints, Intimidating Fang after Weakness, the restricted Tools (Energy Root, Curse
    /// Powder, Fluffy Berry, Sitrus Berry, Solid Rage), Mary's Request, Snappy Move, Energy Evolution and the Unown.
    /// </summary>
    public class UfTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Fire = "MEE-10";
        const string Fighting = "MEE-14";

        const string Flareon = "UF-5";
        const string Aipom = "UF-34";
        const string Granbull = "UF-39";
        const string Eevee = "UF-55";
        const string Larvitar = "UF-61";
        const string Snubbull = "UF-74";
        const string CursePowder = "UF-80";
        const string EnergyRoot = "UF-83";
        const string FluffyBerry = "UF-85";
        const string MarysRequest = "UF-86";
        const string SitrusBerry = "UF-91";
        const string SolidRage = "UF-92";
        const string UnownA = "UF-A";
        const string UnownB = "UF-B";
        const string UnownN = "UF-N";
        const string UnownQuestion = "UF-?";
        const string Pidgeot = "RG-10";
        const string SharpedoEx = "DX-104";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private PokemonInPlay GranbullIn(GameEngine e, int player)
        {
            var mon = e.ScenarioActive(player, C(Snubbull));
            e.ScenarioEvolve(mon, C(Granbull));
            return mon;
        }

        [Test]
        public void Unseen_Forces_Is_In_The_Pool_With_Its_Prints_And_Unown()
        {
            Assert.AreEqual(129, db.Cards.Count(d => d.SetCode == "UF"), "145 cards minus 16 prints");
            Assert.AreEqual(145, db.All.Count(d => d.SetCode == "UF"), "117 numbered cards and 28 Unown");
            Assert.AreEqual("EM-86", db.MainPrinting(db.Get("UF-96")).Key);
            Assert.AreEqual("NP-34", db.MainPrinting(db.Get("UF-17")).Key);
            Assert.AreEqual("UF-91", db.MainPrinting(C(SitrusBerry)).Key, "your ruling: its own card");
            Assert.AreEqual("Unown", C(UnownQuestion).Name);
        }

        [Test]
        public void Intimidating_Fang_Takes_10_Off_After_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var larvitar = e.ScenarioActive(0, C(Larvitar));
            Attach(e, larvitar, Fighting, 2);
            var granbull = GranbullIn(e, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, granbull.DamageCounters, "20 x 2 - 10, not (20 - 10) x 2");
        }

        [Test]
        public void Energy_Root_Gives_20_HP_And_Stops_Poke_Powers_But_Not_On_Pokemon_ex()
        {
            foreach (bool root in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                var pidgeot = e.ScenarioActive(1, C(Pidgeot));
                int hp = e.MaxHp(pidgeot);
                if (root) e.ScenarioAttachTool(pidgeot, C(EnergyRoot));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(!root, Offered(e, 1, "Use Quick Search"));
                if (root) Assert.AreEqual(hp + 20, e.MaxHp(pidgeot));
            }
            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sharpedo = e2.ScenarioActive(0, C(SharpedoEx));
            Assert.IsFalse(e2.CanAttachToolTo(e2.NewCard(0, C(EnergyRoot)), sharpedo));
            Assert.IsFalse(e2.CanAttachToolTo(e2.NewCard(0, C(SitrusBerry)), sharpedo));
            Assert.IsFalse(e2.CanAttachToolTo(e2.NewCard(0, C(CursePowder)), e2.ScenarioBench(0, C(Snubbull))), "Curse Powder: Evolved Pokémon only");
        }

        [Test]
        public void Curse_Powder_Puts_3_Counters_On_The_Attacker_That_Knocks_Out_Its_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var larvitar = e.ScenarioActive(0, C(Larvitar));
            Attach(e, larvitar, Fighting, 2);
            var granbull = GranbullIn(e, 1);
            granbull.DamageCounters = 4;
            e.ScenarioAttachTool(granbull, C(CursePowder));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, larvitar.DamageCounters);
        }

        [Test]
        public void Fluffy_Berry_Makes_The_Retreat_Cost_0()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var granbull = GranbullIn(e, 0);
            Assert.AreEqual(2, e.RetreatCostOf(granbull));
            e.ScenarioAttachTool(granbull, C(FluffyBerry));
            Assert.AreEqual(0, e.RetreatCostOf(granbull));
        }

        [Test]
        public void Sitrus_Berry_Removes_3_Counters_Between_Turns_With_3_Or_More()
        {
            foreach (int counters in new[] { 2, 4 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var chansey = e.ScenarioActive(0, C(Chansey));
                chansey.DamageCounters = counters;
                e.ScenarioAttachTool(chansey, C(SitrusBerry));
                e.ScenarioActive(1, C(Pidgeot));
                e.ScenarioSetTurn(1, 4);
                e.StartTurn();
                Assert.AreEqual(counters == 4 ? 1 : 2, chansey.DamageCounters);
                Assert.AreEqual(counters == 4 ? 0 : 1, chansey.Tools.Count);
            }
        }

        [Test]
        public void Solid_Rage_Adds_20_With_More_Prizes_Left()
        {
            foreach (bool behind in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Crushing Blow"), new ScriptedAgent());
                var granbull = GranbullIn(e, 0);
                Attach(e, granbull, Psychic, 2);
                e.ScenarioAttachTool(granbull, C(SolidRage));
                var chansey = e.ScenarioActive(1, C(Chansey));
                if (behind) e.State.Players[1].Prizes.RemoveAt(0);
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(behind ? 5 : 3, chansey.DamageCounters);
            }
        }

        [Test]
        public void Marys_Request_Draws_3_Without_A_Stage_2()
        {
            foreach (bool stage2 in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Mary's Request"), new ScriptedAgent());
                e.ScenarioActive(0, C(stage2 ? Pidgeot : Chansey));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioAddToHand(0, C(MarysRequest));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(stage2 ? 1 : 3, e.State.Players[0].Hand.Count);
            }
        }

        [Test]
        public void Snappy_Move_Draws_And_Puts_Aipom_On_The_Bottom_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Snappy Move"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var aipom = e.ScenarioBench(0, C(Aipom));
            Attach(e, aipom, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(0, me.Bench.Count);
            Assert.AreEqual(1, me.Hand.Count);
            Assert.AreEqual("Aipom", me.Deck[me.Deck.Count - 1].Def.Name);
            Assert.AreEqual(1, me.Discard.Count, "the Energy is discarded");
        }

        [Test]
        public void Energy_Evolution_Evolves_Eevee_Into_The_Energy_s_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Fire Energy", "Yes"), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C(Eevee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C(Flareon), 1);
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Flareon", eevee.Name);
            Assert.AreEqual(1, eevee.Energy.Count);
        }

        [Test]
        public void Shuffle_Switches_Unown_With_Another_Unown_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shuffle (Unown (Active))"), new ScriptedAgent());
            var unown = e.ScenarioActive(0, C(UnownA));
            unown.DamageCounters = 2;
            e.ScenarioBench(0, C(UnownN));
            e.ScenarioAddToDeck(0, C(UnownB), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(UnownB, unown.Def.Key);
            Assert.AreEqual(2, unown.DamageCounters, "damage counters stay");
            Assert.AreEqual(21, e.State.Players[0].Deck.Count, "Unown A went into the deck");
            Assert.IsFalse(Offered(e, 0, "Use Shuffle"), "only 1 Shuffle each turn");
        }

        [Test]
        public void Unown_N_Removes_5_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hidden Power"), new ScriptedAgent());
            var unown = e.ScenarioActive(0, C(UnownN));
            unown.DamageCounters = 5;
            Attach(e, unown, Psychic, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, unown.DamageCounters);
        }

        [Test]
        public void Unown_Question_Draws_2_When_The_Opponent_Guesses_Wrong()
        {
            foreach (bool pokemon in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Hidden Power"), new ScriptedAgent());
                var unown = e.ScenarioActive(0, C(UnownQuestion));
                Attach(e, unown, Psychic, 1);
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioAddToHand(0, C(pokemon ? Chansey : Fire));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pokemon ? 1 : 3, e.State.Players[0].Hand.Count, "the opponent guesses Pokémon (the first option)");
            }
        }
    }
}
