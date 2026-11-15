using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v157: the rest of Delta Reign (DLR) and the newest Mega Promos (MEP): alternate arts and reprints as prints, and the new
    /// effects (Slippery Oil, Make Waves, Brutal Whirlpool, Deep Sea Draw, Generator's Touch, Raid, High Density Armor, Tormenting
    /// Poison, Lure Out, Bouncy-Bounce Charge, Cottony Carrier, Punk Out, Punishing Needles, Custom Vest, Tate &amp; Liza's Training).
    /// </summary>
    public class DlrRestTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Grass = "MEE-9";
        const string Fighting = "MEE-14";
        const string Chansey = "E2-69";
        const string MegaLatias = "MEG-100";
        const string MegaRayquaza = "DLR-84";
        const string Pyroduct = "DLR-94";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        /// <summary>Puts a new card on top of a player's deck.</summary>
        private void AddToTop(GameEngine e, int player, string key)
        {
            var deck = e.State.Players[player].Deck;
            e.ScenarioAddToDeck(player, C(key), 1);
            var card = deck[deck.Count - 1];
            deck.RemoveAt(deck.Count - 1);
            deck.Insert(0, card);
        }

        [Test]
        public void Alternate_Arts_And_Reprints_Are_Prints()
        {
            var prints = new[]
            {
                new[] { "DLR-113", "DLR-68" }, new[] { "DLR-115", "DLR-80" }, new[] { "DLR-116", "DLR-82" }, new[] { "DLR-124", "DLR-88" },
                new[] { "DLR-127", "DLR-91" }, new[] { "DLR-130", "DLR-102" }, new[] { "MEP-117", "DLR-26" }, new[] { "DLR-21", "MEP-112" },
                new[] { "DLR-48", "MEP-113" }, new[] { "DLR-66", "DLR-112" }, new[] { "DLR-118", "DLR-11" }, new[] { "DLR-135", "DLR-11" },
                new[] { "DLR-129", "POR-82" }, new[] { "DLR-133", "POR-86" }, new[] { "DLR-141", "DLR-84" }, new[] { "MEP-97", "30C-18" },
                new[] { "MEP-115", "POR-35" }, new[] { "MEP-120", "MEP-28" }, new[] { "MEP-92", "SVP-45" },
            };
            foreach (var pair in prints) Assert.AreEqual(pair[1], db.MainPrinting(C(pair[0])).Key, pair[0]);
            for (int n = 1; n <= 141; n++) Assert.IsNotNull(db.Get("DLR", n.ToString()), "DLR " + n);
            Assert.AreEqual("Dolliv", C("DLR-17").EvolvesFrom, "a Stage 2 evolves from the Stage 1");
            Assert.AreEqual("Nidorina", C("DLR-57").EvolvesFrom);
            Assert.AreEqual(0, C("DLR-80").Attacks[0].Cost.Count(), "Bouncy-Bounce Charge costs nothing");
            Assert.IsTrue(C("DLR-91").IsTool && C("DLR-102").IsSupporter);
        }

        [Test]
        public void Slippery_Oil_Switches_Out_The_Opponents_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Slippery Oil"), new ScriptedAgent().Then("Kyogre"));
            e.ScenarioActive(0, C("DLR-17"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C("DLR-26"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Kyogre", e.State.Players[1].Active.Name);
            Assert.IsTrue(e.State.Players[1].Bench.Contains(chansey));
            Assert.IsFalse(Offered(e, 0, "Use Slippery Oil"), "once during your turn");
        }

        [Test]
        public void Make_Waves_Shuffles_Both_Hands_In_And_Each_Player_Draws_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Make Waves"), new ScriptedAgent());
            var mantine = e.ScenarioActive(0, C("DLR-25"));
            Attach(e, mantine, Water, 1);
            for (int i = 0; i < 2; i++) e.ScenarioAddToHand(0, C(Chansey));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Brutal_Whirlpool_Hits_The_Bench_Only_With_A_Legendary_Stadium()
        {
            foreach (bool legendary in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Brutal Whirlpool"), new ScriptedAgent());
                var kyogre = e.ScenarioActive(0, C("DLR-26"));
                Attach(e, kyogre, Water, 3);
                if (legendary) e.ScenarioSetStadium(0, C(Pyroduct));
                var active = e.ScenarioActive(1, C("DLR-52"));
                var benched = e.ScenarioBench(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                // Legendary Pyroduct turns off the Abilities of Evolution Pokémon, so Steelix's High Density Armor stops working.
                Assert.AreEqual(legendary ? 10 : 4, active.DamageCounters, legendary ? "no Ability under Legendary Pyroduct" : "100 - 60 (High Density Armor at full HP)");
                Assert.AreEqual(legendary ? 5 : 0, benched.DamageCounters, legendary ? "50 to each Benched Pokémon" : "no Legendary Stadium");
            }
        }

        [Test]
        public void Deep_Sea_Draw_Draws_And_May_Put_A_Card_On_The_Bottom()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Deep Sea Draw", "Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-28"));
            e.ScenarioAddToHand(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, me.Hand.Count, "drew a Psychic Energy, put Chansey back");
            Assert.AreEqual(deck, me.Deck.Count);
            Assert.AreEqual("Chansey", me.Deck[me.Deck.Count - 1].Def.Name, "on the bottom");
        }

        [Test]
        public void Generators_Touch_Attaches_A_Lightning_Energy_From_The_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Generator's Touch", "Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-39"));
            var benched = e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Use Generator's Touch"), "no Lightning Energy in the discard pile");
            e.ScenarioAddToDiscard(0, C(Lightning), 1);
            Assert.IsTrue(Offered(e, 0, "Use Generator's Touch"));
            Step(e);
            Assert.AreEqual(1, benched.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Raid_Does_90_More_The_Turn_It_Evolved_From_Wattrel()
        {
            foreach (bool fresh in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Raid"), new ScriptedAgent());
                var mon = e.ScenarioActive(0, C("DLR-40"));
                e.ScenarioEvolve(mon, C("DLR-41"));
                mon.TurnEvolved = fresh ? 3 : 1;
                Attach(e, mon, Lightning, 1);
                var target = e.ScenarioActive(1, C(MegaLatias));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(fresh ? 12 : 3, target.DamageCounters, fresh ? "30 + 90" : "evolved during an earlier turn");
            }
        }

        [Test]
        public void High_Density_Armor_Only_Works_At_Full_HP()
        {
            foreach (bool damaged in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Strafe"));
                var steelix = e.ScenarioActive(0, C("DLR-52"));
                if (damaged) steelix.DamageCounters = 1;
                var latias = e.ScenarioActive(1, C(MegaLatias));
                Attach(e, latias, Psychic, 1);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(damaged ? 5 : 0, steelix.DamageCounters, damaged ? "1 + 40" : "40 - 60");
            }
        }

        [Test]
        public void Tormenting_Poison_Puts_4_Counters_At_Each_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tormenting Poison"), new ScriptedAgent());
            var ariados = e.ScenarioActive(0, C("DLR-58"));
            e.ScenarioEvolve(ariados, C("DLR-59"));
            Attach(e, ariados, Darkness, 1);
            var target = e.ScenarioActive(1, C("DLR-26"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(4, target.PoisonCounterBase);
        }

        [Test]
        public void Lure_Out_Puts_Basic_Pokemon_From_The_Opponents_Top_5_On_Their_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lure Out", "Chansey|Kyogre"), new ScriptedAgent());
            var sableye = e.ScenarioActive(0, C("DLR-60"));
            Attach(e, sableye, Darkness, 1);
            e.ScenarioActive(1, C(Chansey));
            AddToTop(e, 1, "DLR-26");
            AddToTop(e, 1, "DLR-41"); // a Stage 1 stays in the deck
            AddToTop(e, 1, Chansey);
            int deck = e.State.Players[1].Deck.Count;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(2, opp.Bench.Count);
            Assert.AreEqual(deck - 2, opp.Deck.Count);
            Assert.IsTrue(opp.Deck.Any(c => c.Def.Name == "Kilowattrel"));
        }

        [Test]
        public void Bouncy_Bounce_Charge_Is_Free_And_Attaches_To_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bouncy-Bounce Charge", "Psychic Energy", "Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C("DLR-80"));
            var benched = e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, benched.Energy.Count);
        }

        [Test]
        public void Cottony_Carrier_Removes_The_Retreat_Cost_Of_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var altaria = e.ScenarioBench(0, C("DLR-81"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(chansey));
            e.ScenarioEvolve(altaria, C("DLR-82"));
            Assert.AreEqual(0, e.RetreatCostOf(chansey));
        }

        [Test]
        public void Punk_Out_Removes_Wimpods_Retreat_Cost_Against_A_Pokemon_ex()
        {
            foreach (bool ex in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var wimpod = e.ScenarioActive(0, C("DLR-10"));
                e.ScenarioActive(1, C(ex ? MegaLatias : Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(ex ? 0 : 3, e.RetreatCostOf(wimpod));
            }
        }

        [Test]
        public void Punishing_Needles_Does_50_More_For_Each_Opponent_Pokemon_With_An_Ability()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Punishing Needles"), new ScriptedAgent());
            var cacturne = e.ScenarioActive(0, C("DLR-4"));
            e.ScenarioEvolve(cacturne, C("DLR-5"));
            Attach(e, cacturne, Grass, 1);
            var target = e.ScenarioActive(1, C(MegaLatias));
            e.ScenarioBench(1, C("DLR-28"));
            e.ScenarioBench(1, C("DLR-39"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, target.DamageCounters, "10 + 2 x 50");
        }

        [Test]
        public void Custom_Vest_Takes_60_Less_From_A_Mega_Evolution_ex_Unless_The_Holder_Is_One()
        {
            foreach (bool megaHolder in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Strafe"));
                var holder = e.ScenarioActive(0, C(megaHolder ? MegaRayquaza : "DLR-26"));
                e.ScenarioAttachTool(holder, C("DLR-91"));
                var latias = e.ScenarioActive(1, C(MegaLatias));
                Attach(e, latias, Psychic, 1);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(megaHolder ? 4 : 0, holder.DamageCounters);
            }
        }

        [Test]
        public void Tate_And_Lizas_Training_Goes_Back_To_The_Hand_With_A_Legendary_Stadium()
        {
            foreach (bool legendary in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Tate & Liza's Training"), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioActive(1, C(Chansey));
                if (legendary) e.ScenarioSetStadium(1, C(Pyroduct));
                e.ScenarioAddToHand(0, C("DLR-102"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                var me = e.State.Players[0];
                Assert.AreEqual(legendary ? 3 : 2, me.Hand.Count);
                Assert.AreEqual(legendary, me.Hand.Any(c => c.Def.Name == "Tate & Liza's Training"));
                Assert.AreEqual(legendary ? 0 : 1, me.Discard.Count(c => c.Def.Name == "Tate & Liza's Training"));
            }
        }
    }
}
