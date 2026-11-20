using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v174: Aura Seeker (ASK), the Japanese set Flowing Fists comes from: the card data and prints, and the new effects (Torrential
    /// Pump, Deception, Hustle Screw, Toxic Gem, Heavy Bomber, Skill Control, Bloody Fear, Korrina's Seriousness, Fight Arena,
    /// Overflowing Cheer, Vibrating Wings, Synchro Body, Crasher Wake's Fervor).
    /// </summary>
    public class AskTests : ScenarioTestBase
    {
        const string Chansey = "E2-69";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Garchomp = "ASK-67";
        const string Fervor = "ASK-76";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Aura_Seeker_Is_In_The_Pool_With_Its_Flowing_Fists_Prints()
        {
            Assert.AreEqual(75, db.Cards.Count(d => d.SetCode == "ASK"));
            Assert.AreEqual("FLO-32", db.MainPrinting(C("ASK-31")).Key, "Sigilyph ex");
            Assert.AreEqual("FLO-45", db.MainPrinting(C("ASK-44")).Key, "Riolu");
            Assert.AreEqual("FLO-58", db.MainPrinting(C("ASK-57")).Key, "Mega Lucario Z ex");
        }

        [Test]
        public void Every_Aura_Seeker_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "ASK" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Torrential_Pump_Adds_30_To_The_Bench_And_Does_Not_Stack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hail"), new ScriptedAgent());
            var cryogonal = e.ScenarioActive(0, C("ASK-21"));
            Attach(e, cryogonal, Water, 2);
            e.ScenarioBench(0, C("ASK-16"));
            e.ScenarioBench(0, C("ASK-16"));
            var active = e.ScenarioActive(1, C(Chansey));
            var benched = e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, active.DamageCounters, "the Active Pokémon takes the printed 20");
            Assert.AreEqual(5, benched.DamageCounters, "20 + 30 once, though there are 2 Blastoise");
        }

        [Test]
        public void Deception_Gives_No_Prizes_With_Exactly_5_Cards_In_Hand()
        {
            foreach (int hand in new[] { 5, 4 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
                var ninetales = e.ScenarioActive(0, C("ASK-13"));
                ninetales.DamageCounters = e.MaxHp(ninetales) / 10 - 1;
                e.ScenarioBench(0, C(Chansey));
                for (int i = 0; i < hand; i++) e.ScenarioAddToHand(0, C(Psychic));
                var mew = e.ScenarioActive(1, C("HP-100"));
                Attach(e, mew, Psychic, 2);
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(hand == 5 ? 6 : 5, e.State.Players[1].Prizes.Count, hand + " cards in hand");
            }
        }

        [Test]
        public void Hustle_Screw_Counts_Revealed_Fervors_And_Shuffles_Them_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hustle Screw", "Crasher Wake's Fervor|Crasher Wake's Fervor"), new ScriptedAgent());
            var floatzel = e.ScenarioActive(0, C("ASK-20"));
            Attach(e, floatzel, Water, 2);
            e.ScenarioAddToHand(0, C(Fervor));
            e.ScenarioAddToHand(0, C(Fervor));
            var garchomp = e.ScenarioActive(1, C(Garchomp));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(24, garchomp.DamageCounters, "40 + 2 x 100");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count(c => c.Def.Name == "Crasher Wake's Fervor"));
            Assert.AreEqual(deck + 2, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Toxic_Gem_Leaves_2_Pokemon_At_60_Hp()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Toxic Gem"), new ScriptedAgent());
            var glimmora = e.ScenarioActive(0, C("ASK-54"));
            Attach(e, glimmora, Darkness, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var counters = e.State.Players[1].InPlay().Select(m => m.DamageCounters).OrderBy(x => x).ToList();
            CollectionAssert.AreEqual(new[] { 0, 4, 4 }, counters);
        }

        [Test]
        public void Heavy_Bomber_Is_50_Less_For_Each_Retreat_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Bomber"), new ScriptedAgent());
            var tingLu = e.ScenarioActive(0, C("ASK-55"));
            Attach(e, tingLu, Darkness, 4);
            var garchomp = e.ScenarioActive(1, C(Garchomp));
            e.ScenarioSetTurn(0, 3);
            int retreat = e.RetreatCostOf(garchomp);
            Step(e);
            Assert.AreEqual(25 - 5 * retreat, garchomp.DamageCounters);
        }

        [Test]
        public void Skill_Control_Uses_The_Opponents_Mega_Ex_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Skill Control", "Great Spin"), new ScriptedAgent());
            var malamar = e.ScenarioActive(0, C("ASK-52"));
            Attach(e, malamar, Psychic, 1);
            var glimmora = e.ScenarioActive(1, C("ASK-54"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(25, glimmora.DamageCounters, "Great Spin's 250");
        }

        [Test]
        public void Korrinas_Seriousness_Adds_20_Per_Prize_The_Opponent_Took_And_Bloody_Fear_Stops_Stadiums()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Korrina's Seriousness", "Attack: Bloody Fear"), new ScriptedAgent());
            var absol = e.ScenarioActive(0, C("ASK-30"));
            Attach(e, absol, Psychic, 3);
            e.ScenarioAddToHand(0, C("ASK-75"));
            var garchomp = e.ScenarioActive(1, C(Garchomp));
            e.State.Players[1].Prizes.RemoveAt(0);
            e.State.Players[1].Prizes.RemoveAt(0);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(20, garchomp.DamageCounters, "160 + 2 x 20");
            e.ScenarioAddToHand(1, C("ASK-78"));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(Offered(e, 1, "Play Fight Arena"), "Bloody Fear: no Stadium cards during the next turn");
        }

        [Test]
        public void Fight_Arena_Heals_20_From_Pokemon_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent().Then("Fight Arena"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = 3;
            var absol = e.ScenarioBench(0, C("ASK-30"));
            absol.DamageCounters = 3;
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C("ASK-78"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, chansey.DamageCounters);
            Assert.AreEqual(3, absol.DamageCounters, "Pokémon ex have a Rule Box");
            Assert.IsFalse(Offered(e, 0, "Fight Arena"), "once during each player's turn");
        }

        [Test]
        public void Overflowing_Cheer_Makes_Guts_Fight_Cost_Colorless_Less()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hitmontop = e.ScenarioActive(0, C("ASK-40"));
            Attach(e, hitmontop, Fighting, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            var guts = hitmontop.Def.Attacks.First(a => a.Name == "Guts Fight");
            Assert.IsFalse(e.CanPayAttack(hitmontop, guts));
            e.ScenarioBench(0, C("ASK-46"));
            Assert.IsTrue(e.CanPayAttack(hitmontop, guts));
        }

        [Test]
        public void Vibrating_Wings_Is_Used_From_The_Hand_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Vibrating Wings"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("ASK-63"));
            e.ScenarioAddToHand(0, C("ASK-63"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Vibrating Wings"));
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Vibrava"));
            Assert.IsFalse(Offered(e, 0, "Vibrating Wings"));
        }

        [Test]
        public void Synchro_Body_Gives_Kecleon_The_Types_Of_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C("ASK-71"));
            e.ScenarioActive(1, C(Chansey));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Water));
            e.ScenarioBench(0, C("ASK-14"));
            e.ScenarioBench(0, C("ASK-71"));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Water));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Colorless));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Fire));
        }

        [Test]
        public void Crasher_Wakes_Fervor_Shuffles_A_Water_Pokemon_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Crasher Wake's Fervor"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var squirtle = e.ScenarioBench(0, C("ASK-14"));
            Attach(e, squirtle, Water, 1);
            e.ScenarioAddToHand(0, C(Fervor));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(deck + 2, e.State.Players[0].Deck.Count);
        }
    }
}
