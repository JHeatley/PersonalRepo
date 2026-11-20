using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v171: Power Keepers (PK): the card data and prints, and the new effects (the Elite Four Stadiums and Synergy Effects,
    /// Steven's Advice, Primal Stare, Rock Blast, Bench Manipulation, Pull Away, Cursed Eyes, Psychic Protector, Magnetic Redraw,
    /// Skill Hack, Metal Gravity, Chilling Breath, the Ray Poké-Powers, Thick Skin, Influence).
    /// </summary>
    public class PkTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Water = "MEE-11";
        const string Fire = "MEE-10";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Power_Keepers_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(76, db.Cards.Count(d => d.SetCode == "PK"));
            for (int n = 1; n <= 108; n++) Assert.IsNotNull(db.Get("PK", n.ToString()), "PK " + n);
            Assert.AreEqual("RS-3", db.MainPrinting(C("PK-5")).Key, "Blaziken");
            Assert.AreEqual("SS-90", db.MainPrinting(C("PK-84")).Key, "Claw Fossil");
            Assert.AreEqual("PK-83", db.MainPrinting(C("PK-83")).Key, "Steven's Advice is its own card");
            Assert.AreEqual("PK-11", db.MainPrinting(C("PK-11")).Key, "Machamp is its own card");
        }

        [Test]
        public void Every_Power_Keepers_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "PK" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Steven_Advice_Cant_Be_Played_With_7_Cards_In_Hand()
        {
            foreach (int extra in new[] { 5, 6 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Chansey));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioAddToHand(0, C("PK-83"));
                for (int i = 0; i < extra; i++) e.ScenarioAddToHand(0, C(Psychic));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(extra + 1 < 7, Offered(e, 0, "Play Steven's Advice"), (extra + 1) + " cards in hand");
            }
        }

        [Test]
        public void Phoebes_Stadium_Lowers_Psychic_Retreat_And_Turns_On_Sableye()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var sableye = e.ScenarioActive(0, C("PK-22"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetStadium(0, C("PK-79"));
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(0, e.RetreatCostOf(mew), "Mew ex retreats for 2 less");
            Step(e);
            Assert.AreEqual(0, sableye.DamageCounters, "Synergy Effect: no damage from a Pokémon-ex");
        }

        [Test]
        public void Sidneys_Stadium_Keeps_Darkness_Pokemon_From_Sleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hypnosis"), new ScriptedAgent());
            var kirlia = e.ScenarioActive(0, C("PK-31"));
            Attach(e, kirlia, Psychic, 1);
            var target = e.ScenarioActive(1, C("PK-18"));
            e.ScenarioSetStadium(0, C("PK-82"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(target.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Primal_Stare_Stops_Evolving_The_Opponents_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("PK-60"));
            e.ScenarioAddToHand(0, C("PK-36"));
            e.ScenarioActive(1, C("PK-10"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Evolve"));
        }

        [Test]
        public void Rock_Blast_Does_20_For_Each_Fighting_Energy_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Blast", "Fighting Energy|Fighting Energy|Fighting Energy"), new ScriptedAgent());
            var armaldo = e.ScenarioActive(0, C("PK-3"));
            Attach(e, armaldo, Fighting, 3);
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, target.DamageCounters);
            Assert.AreEqual(0, armaldo.Energy.Count);
        }

        [Test]
        public void Pull_Away_Makes_The_Opponent_Discard_Down_To_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pull Away"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C("PK-17"));
            Attach(e, mawile, Metal, 2);
            e.ScenarioActive(1, C(MewEx));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Cursed_Eyes_Moves_3_Damage_Counters_Between_Opponent_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Absol ex", "Yes", "Chansey", "Mew ex"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("PK-92"));
            var mew = e.ScenarioActive(1, C(MewEx));
            var chansey = e.ScenarioBench(1, C(Chansey));
            chansey.DamageCounters = 4;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, chansey.DamageCounters);
            Assert.AreEqual(3, mew.DamageCounters);
        }

        [Test]
        public void Psychic_Protector_Discards_Cards_To_Cut_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Psychic Energy|Psychic Energy"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var flygon = e.ScenarioActive(0, C("PK-94"));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Psychic));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, flygon.DamageCounters, "30 - 2 x 10");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Magnetic_Redraw_Has_Each_Player_Draw_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Magnetic Redraw"), new ScriptedAgent());
            e.ScenarioActive(0, C("PK-95"));
            e.ScenarioActive(1, C(Chansey));
            for (int i = 0; i < 2; i++) e.ScenarioAddToHand(0, C(Psychic));
            for (int i = 0; i < 8; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Skill_Hack_Copies_An_Attack_From_The_Opponents_Hand_Without_Its_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Skill Hack", "Devo Crush"), new ScriptedAgent());
            var shiftry = e.ScenarioActive(0, C("PK-97"));
            Attach(e, shiftry, Darkness, 1);
            var target = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters, "Devo Crush: 50");
        }

        [Test]
        public void Metal_Gravity_Hits_A_Healthy_Pokemon_That_Retreats()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat", "Chansey"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            Attach(e, active, Psychic, 2);
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C("PK-98"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, active.DamageCounters);
        }

        [Test]
        public void Chilling_Breath_Stops_The_Opponent_Playing_Trainers()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var sealeo = e.ScenarioActive(0, C("PK-37"));
            e.ScenarioAddToHand(0, C("PK-99"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C("CG-85"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Walrein ex", sealeo.Name);
            e.EndTurn();
            e.StartTurn();
            Assert.IsFalse(Offered(e, 1, "Play Windstorm"));
        }

        [Test]
        public void Crimson_Ray_Burns_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Flareon", "Yes"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("PK-100"));
            var theirs = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(mine.Has(SpecialCondition.Burned) && theirs.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Thick_Skin_Keeps_Zangoose_Free_Of_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hypnosis"), new ScriptedAgent());
            var kirlia = e.ScenarioActive(0, C("PK-31"));
            Attach(e, kirlia, Psychic, 1);
            var zangoose = e.ScenarioActive(1, C("PK-25"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(zangoose.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Influence_Benches_Fossil_Pokemon_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Influence", "Kabuto|Anorith"), new ScriptedAgent());
            var omanyte = e.ScenarioActive(0, C("PK-56"));
            Attach(e, omanyte, Water, 1);
            e.ScenarioAddToDeck(0, C("PK-51"), 1);
            e.ScenarioAddToDeck(0, C("PK-26"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
        }
    }
}
