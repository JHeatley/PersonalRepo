using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// Sets added in v19 (Ascended Heroes, Perfect Order, Chaos Rising, Pitch Black, 30th Celebration, Black Star
    /// promos): the engine features their cards brought in. Every card also plays full games in NewSetTests.
    /// </summary>
    public class V19SetTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Darkness = "MEE-15";
        const string Buzz = "DRI-68";       // Electroslug: LL, 50
        const string Zeraora = "MEP-89";    // Mega Zeraora ex, Thunderous Fist: 60 for each Lightning Energy
        const string Wailord = "PBL-16";    // Wailord ex, 380 HP, weak to Lightning

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private PokemonInPlay BuzzReady(GameEngine e)
        {
            var buzz = e.ScenarioActive(1, C(Buzz));
            e.ScenarioAttach(buzz, C(Lightning));
            e.ScenarioAttach(buzz, C(Lightning));
            return buzz;
        }

        // ------------------------------------------------------------------ Pitch Black

        [Test]
        public void Backtrack_Badge_Offers_One_Reflip_For_A_Colorless_Attacker()
        {
            foreach (string tool in new[] { "PBL-74", null })
            {
                var me = new ScriptedAgent().Then("Attack: Challenging Delivery");
                var e = Scenario(me, new ScriptedAgent());
                var bombirdier = e.ScenarioActive(0, C("PBL-71"));
                e.ScenarioAttach(bombirdier, C(Psychic));
                if (tool != null) e.ScenarioAttachTool(bombirdier, C(tool));
                e.ScenarioActive(1, C(Dreepy));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                int asked = me.Seen.Count(r => (r.Prompt ?? "").Contains("Ignore the results and flip again"));
                Assert.AreEqual(tool != null ? 1 : 0, asked, tool != null ? "once per turn with the Badge" : "no Badge, no reflip");
            }
        }

        [Test]
        public void Voltaic_Lightning_Energy_Adds_20_Damage_To_The_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tackle"), new ScriptedAgent());
            var electrike = e.ScenarioActive(0, C("PBL-23"));
            e.ScenarioAttach(electrike, C("PBL-84"));
            e.ScenarioAttach(electrike, C(Lightning));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters, "30 + 20");
        }

        [Test]
        public void Shadowy_Darkness_Energy_Protects_Its_Benched_Darkness_Pokemon()
        {
            foreach (bool shadowy in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Quick Dive", "Nickit"));
                e.ScenarioActive(0, C(Dreepy));
                var nickit = e.ScenarioBench(0, C("PBL-53"));
                if (shadowy) e.ScenarioAttach(nickit, C("PBL-83"));
                var vikavolt = e.ScenarioActive(1, C("PBL-26"));
                e.ScenarioAttach(vikavolt, C(Lightning));
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(shadowy ? 0 : 5, nickit.DamageCounters, shadowy ? "protected on the Bench" : "no Shadowy Energy");
            }
        }

        [Test]
        public void Intense_Kiss_Discards_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Intense Kiss"), new ScriptedAgent());
            var jynx = e.ScenarioActive(0, C("PBL-32"));
            e.ScenarioAttach(jynx, C(Psychic));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid), "still there during the opponent's turn");
            e.EndTurn();
            Assert.IsNull(e.State.FindPokemon(target.Uid), "discarded at the end of that turn");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Dreepy"));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "not a Knock Out: no Prize cards");
        }

        [Test]
        public void Binding_Flame_Raises_Only_The_Opponents_Active_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C("PBL-38"));
            var theirs = e.ScenarioActive(1, C(Dreepy));
            int printed = C(Dreepy).RetreatCost;
            Assert.AreEqual(printed + 1, e.RetreatCostOf(theirs));
            Assert.AreEqual(printed, e.RetreatCostOf(mine));
        }

        [Test]
        public void Fossil_Quarry_Benches_Antique_Fossils_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Fossil Quarry", "Antique Armor Fossil|Antique Skull Fossil"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetStadium(0, C("PBL-76"));
            e.ScenarioAddToDeck(0, C("PBL-72"), 1);
            e.ScenarioAddToDeck(0, C("PBL-73"), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(2, bench.Count);
            Assert.IsTrue(bench.All(m => m.Def.IsFossil));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Use Fossil Quarry"), "once during each player's turn");
        }

        [Test]
        public void Protective_Armor_Reduces_Attack_Damage_By_10()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var fossil = e.ScenarioActive(0, C("PBL-72"));
            BuzzReady(e);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(4, fossil.DamageCounters, "50 - 10");
        }

        [Test]
        public void Tremendous_Bomb_Hits_Back_At_A_Mega_Evolution_Pokemon_Ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Thunderous Fist"));
            var holder = e.ScenarioActive(0, C(Dreepy));
            e.ScenarioAttachTool(holder, C("PBL-82"));
            e.ScenarioBench(0, C(Dreepy));
            var zeraora = e.ScenarioActive(1, C(Zeraora));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(zeraora, C(Lightning));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(12, zeraora.DamageCounters, "240 damage from a Mega Evolution Pokémon ex");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Tremendous Bomb"));
        }

        [Test]
        public void Lively_Cutter_Does_200_More_After_A_Heal_This_Turn()
        {
            foreach (bool healed in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Lively Cutter"), new ScriptedAgent());
                var lurantis = e.ScenarioActive(0, C("PBL-4"));
                e.ScenarioAttach(lurantis, C("MEE-9"));
                lurantis.DamageCounters = 5;
                var target = e.ScenarioActive(1, C(Wailord));
                e.ScenarioSetTurn(0, 3);
                if (healed) e.Heal(lurantis, 30);
                Step(e);
                Assert.AreEqual(healed ? 26 : 6, target.DamageCounters);
            }
        }

        [Test]
        public void Bug_Out_Counts_Bug_Out_Pokemon_At_The_Bottom_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bug Out"), new ScriptedAgent());
            var sizzlipede = e.ScenarioActive(0, C("PBL-9"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(sizzlipede, C(Psychic));
            e.ScenarioAddToDeck(0, C("PBL-9"), 2); // the bottom 7: 5 Energy and 2 Sizzlipede
            var target = e.ScenarioActive(1, C(Wailord));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, target.DamageCounters, "2 x 50");
            Assert.AreEqual(17, e.State.Players[0].Deck.Count, "the 5 other cards were discarded");
            Assert.AreEqual(5, e.State.Players[0].Discard.Count);
        }

        // ------------------------------------------------------------------ Perfect Order

        [Test]
        public void Infinite_Shadow_Puts_Gengar_Into_The_Hand_When_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Thunderous Fist"));
            var gengar = e.ScenarioActive(0, C("POR-48"));
            e.ScenarioEvolve(gengar, C("POR-49"));
            e.ScenarioEvolve(gengar, C("POR-50"));
            e.ScenarioBench(0, C(Dreepy));
            var zeraora = e.ScenarioActive(1, C(Zeraora));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(zeraora, C(Lightning));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(gengar.Uid), "Knocked Out");
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Gengar"), "Gengar went to the hand");
            Assert.IsFalse(e.State.Players[0].Discard.Any(c => c.Def.Name == "Gengar"));
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the opponent still takes a Prize card");
        }

        [Test]
        public void Tundra_Wall_Protects_Pokemon_With_Water_Energy()
        {
            foreach (bool water in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
                var dreepy = e.ScenarioActive(0, C(Dreepy));
                if (water) e.ScenarioAttach(dreepy, C(Water));
                e.ScenarioBench(0, C("POR-24"));
                BuzzReady(e);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(water ? 0 : 5, dreepy.DamageCounters, water ? "50 - 50" : "no Water Energy");
            }
        }

        [Test]
        public void Fighting_Roar_Lets_Luxio_Evolve_The_Turn_It_Evolved_Against_A_Pokemon_Ex()
        {
            foreach (bool ex in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Dreepy));
                var luxio = e.ScenarioBench(0, C("POR-26"));
                e.ScenarioEvolve(luxio, C("POR-27"));
                e.ScenarioActive(1, C(ex ? Zeraora : Dreepy));
                e.ScenarioSetTurn(0, 3);
                luxio.TurnEvolved = 3;
                Assert.AreEqual(ex, e.CanEvolveInto(luxio, C("POR-28")), ex ? "Fighting Roar" : "normal rule");
            }
        }

        // ------------------------------------------------------------------ Chaos Rising

        [Test]
        public void Pernicious_Poison_Puts_16_Damage_Counters_In_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pernicious Poison"), new ScriptedAgent());
            var dragalge = e.ScenarioActive(0, C("CRI-65"));
            e.ScenarioAttach(dragalge, C(Water));
            e.ScenarioAttach(dragalge, C(Darkness));
            var target = e.ScenarioActive(1, C(Wailord));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            e.EndTurn();
            Assert.AreEqual(16, target.DamageCounters);
        }

        [Test]
        public void Startling_Drop_Mills_8_When_The_Opponent_Discards_Ferrothorn_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Controlled Burn"));
            e.ScenarioActive(0, C(Dreepy));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C("CRI-63")));
            var sizzlipede = e.ScenarioActive(1, C("PBL-9"));
            e.ScenarioAttach(sizzlipede, C("MEE-10"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Ferrothorn"));
            Assert.AreEqual(12, e.State.Players[1].Deck.Count, "the top 8 cards of the attacker's deck");
        }

        // ------------------------------------------------------------------ 30th Celebration

        [Test]
        public void Supereffective_Pheromones_Makes_Weakness_x3_With_Volbeat_In_Play()
        {
            foreach (bool volbeat in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
                var wailord = e.ScenarioActive(0, C(Wailord));
                e.ScenarioBench(0, C("30C-4"));
                if (volbeat) e.ScenarioBench(0, C("30C-3"));
                BuzzReady(e);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(volbeat ? 15 : 10, wailord.DamageCounters, volbeat ? "50 x 3" : "50 x 2");
            }
        }

        [Test]
        public void Keep_Hidden_Protects_Pikachu_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Quick Dive", "Pikachu"));
            e.ScenarioActive(0, C(Dreepy));
            var pikachu = e.ScenarioBench(0, C("30C-33"));
            var vikavolt = e.ScenarioActive(1, C("PBL-26"));
            e.ScenarioAttach(vikavolt, C(Lightning));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, pikachu.DamageCounters);
        }

        [Test]
        public void Counterattack_Grouping_Does_Not_Stack()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            e.ScenarioActive(0, C("30C-22"));
            e.ScenarioBench(0, C("30C-22"));
            var buzz = BuzzReady(e);
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(3, buzz.DamageCounters, "your ruling: once per hit, however many Wishiwashi are in play");
        }

        [Test]
        public void Celebration_Takes_2_Prizes_With_Exactly_30_Cards_In_Hand()
        {
            foreach (int cards in new[] { 30, 29 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Celebration"), new ScriptedAgent());
                var gholdengo = e.ScenarioActive(0, C("30C-108"));
                e.ScenarioAttach(gholdengo, C("MEE-16"));
                e.ScenarioActive(1, C(Dreepy));
                for (int i = 0; i < cards; i++) e.ScenarioAddToHand(0, C(Psychic));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(cards == 30 ? 4 : 6, e.State.Players[0].Prizes.Count);
                Assert.AreEqual(cards == 30 ? 0 : 29, e.State.Players[0].Hand.Count, "the hand is shuffled into the deck");
            }
        }

        [Test]
        public void Good_Sleep_Heals_Snorlax_When_It_Stays_Asleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C("30C-119"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            snorlax.DamageCounters = 5;
            e.ApplyCondition(snorlax, SpecialCondition.Asleep);
            e.EndTurn();
            Assert.AreEqual(snorlax.Has(SpecialCondition.Asleep) ? 0 : 5, snorlax.DamageCounters, "healed only if it stayed Asleep");
        }

        // ------------------------------------------------------------------ Scarlet & Violet promos

        [Test]
        public void Iono_Puts_Both_Hands_On_The_Bottom_And_Draws_By_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Iono"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("SVP-124"));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Water));
            for (int i = 0; i < 2; i++) e.ScenarioAddToHand(1, C(Water));
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count, "6 Prize cards left");
            Assert.AreEqual(5, e.State.Players[1].Hand.Count, "5 Prize cards left");
            Assert.AreEqual(C(Water).Name, e.State.Players[0].Deck.Last().Def.Name, "the old hand went to the bottom");
        }

        [Test]
        public void Mysterious_Shield_Stops_Pokemon_Ex_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Thunderous Fist"));
            var aegislash = e.ScenarioActive(0, C("SVP-60"));
            var zeraora = e.ScenarioActive(1, C(Zeraora));
            e.ScenarioAttach(zeraora, C(Lightning));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, aegislash.DamageCounters);
        }

        [Test]
        public void Squawk_And_Seize_Only_Works_On_Your_First_Turn()
        {
            foreach (int turn in new[] { 2, 4 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C("SVP-160"));
                e.ScenarioActive(1, C(Dreepy));
                e.ScenarioSetTurn(0, turn);
                bool offered = e.GetMainActions(0).Any(o => o.Label.Contains("Squawk and Seize"));
                Assert.AreEqual(turn == 2, offered, "turn " + turn);
            }
        }
    }
}
