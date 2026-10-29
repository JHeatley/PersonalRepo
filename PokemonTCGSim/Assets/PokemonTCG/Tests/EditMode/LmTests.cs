using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v170: Legend Maker (LM): the card data and prints, React Energy and the cards that count it, Poké-Power locks (Stench),
    /// Pattern Distraction, Deadlock, Shadow Curse, and the Stadiums with their Compendium rulings.
    /// </summary>
    public class LmTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";

        const string Gengar = "LM-5";
        const string Muk = "LM-11";
        const string Gorebyss = "LM-17";
        const string Huntail = "LM-18";
        const string Spinda = "LM-26";
        const string Dunsparce = "LM-31";
        const string Furret = "LM-33";
        const string Kecleon = "LM-37";
        const string Omanyte = "LM-60";
        const string Sentret = "LM-62";
        const string Shuppet = "LM-63";
        const string Rapidash = "RG-13";
        const string FullFlame = "LM-74";
        const string GiantStump = "LM-75";
        const string StrangeCave = "LM-77";
        const string MysteriousFossil = "LM-79";
        const string React = "LM-82";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private sealed class Recorder : IGameObserver
        {
            public readonly List<string> Text = new List<string>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Text.Add(ev.Text); }
        }

        [Test]
        public void Legend_Maker_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(87, db.Cards.Count(d => d.SetCode == "LM"), "93 cards minus 6 prints");
            Assert.AreEqual(93, db.All.Count(d => d.SetCode == "LM"));
            Assert.AreEqual("SS-90", db.MainPrinting(db.Get("LM-78")).Key, "Claw Fossil: Sandstorm's");
            Assert.AreEqual("SS-92", db.MainPrinting(db.Get("LM-80")).Key, "Root Fossil: Sandstorm's");
            Assert.AreEqual("HL-57", db.MainPrinting(db.Get("LM-50")).Key);
            Assert.AreEqual("CES-151", db.MainPrinting(db.Get("LM-81")).Key);
            Assert.AreEqual("CEL-CC13", db.MainPrinting(db.Get("LM-88")).Key);
            Assert.AreEqual("NP-35", db.MainPrinting(db.Get("LM-93")).Key);
            var fossil = C(MysteriousFossil);
            Assert.IsTrue(fossil.IsFossil);
            Assert.AreEqual(50, fossil.Hp);
            StringAssert.Contains("your opponent's Dunsparce can't attack", C(Dunsparce).Abilities[0].Text);
            Assert.IsTrue(C(React).IsSpecialEnergy);
        }

        [Test]
        public void Reactive_Booster_Makes_React_Energy_On_Huntail_Provide_2_Of_Every_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var huntail = e.ScenarioActive(0, C(Huntail));
            var shuppet = e.ScenarioBench(0, C(Shuppet));
            e.ScenarioBench(0, C(Gorebyss));
            e.ScenarioAttach(huntail, C(React));
            e.ScenarioAttach(shuppet, C(React));
            e.ScenarioActive(1, C(Sentret));
            CollectionAssert.AreEqual(new[] { EnergyMask.AnyType, EnergyMask.AnyType }, e.EnergyUnitsOf(huntail, huntail.Energy[0]));
            CollectionAssert.AreEqual(new[] { EnergyMask.Colorless }, e.EnergyUnitsOf(shuppet, shuppet.Energy[0]), "only Huntail and Gorebyss");
        }

        [Test]
        public void Stench_Stops_Poke_Powers_While_Muk_Is_Active()
        {
            foreach (bool muk in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Huntail));
                e.ScenarioActive(1, C(muk ? Muk : Sentret));
                e.ScenarioAddToDeck(0, C(React), 1);
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!muk, Offered(e, 0, "Use Reactive Generator"));
            }
        }

        [Test]
        public void Reactive_Generator_Attaches_A_React_Energy_Only_Without_One()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Reactive Generator", "React Energy"), new ScriptedAgent());
            var huntail = e.ScenarioActive(0, C(Huntail));
            e.ScenarioActive(1, C(Sentret));
            e.ScenarioAddToDeck(0, C(React), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("React Energy", huntail.Energy.Single().Def.Name);
            Assert.IsFalse(Offered(e, 0, "Use Reactive Generator"));
        }

        [Test]
        public void Pattern_Distraction_Makes_Basic_Pokemon_Flip_To_Attack()
        {
            foreach (bool evolved in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then(evolved ? "Attack: Body Slam" : "Attack: Scratch"));
                var log = new Recorder();
                e.AddObserver(log);
                e.ScenarioActive(0, C(Spinda));
                var attacker = e.ScenarioActive(1, C(Sentret));
                if (evolved) e.ScenarioEvolve(attacker, C(Furret));
                e.ScenarioAttach(attacker, C(Psychic));
                e.ScenarioAttach(attacker, C(Psychic));
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(!evolved, log.Text.Any(t => t.Contains("tries to attack")), evolved ? "Furret is an Evolved Pokémon" : "Sentret is a Basic Pokémon");
            }
        }

        [Test]
        public void Deadlock_Stops_Only_The_Opponents_Dunsparce()
        {
            foreach (string mine in new[] { Dunsparce, Sentret })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(mine));
                var theirs = e.ScenarioActive(1, C(Dunsparce));
                e.ScenarioAttach(theirs, C(Psychic));
                e.ScenarioSetTurn(1, 4);
                Assert.AreEqual(mine != Dunsparce, Offered(e, 1, "Attack: Down Draw"));
            }
        }

        [Test]
        public void Shadow_Curse_Puts_3_Counters_When_Gengar_Is_Knocked_Out_By_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Scratch"), new ScriptedAgent().Then("Yes"));
            var sentret = e.ScenarioActive(0, C(Sentret));
            e.ScenarioAttach(sentret, C(Psychic));
            var gengar = e.ScenarioActive(1, C(Gengar));
            gengar.DamageCounters = 9;
            e.ScenarioBench(1, C(Shuppet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(gengar));
            Assert.AreEqual(3, sentret.DamageCounters);
        }

        [Test]
        public void Reactive_Colors_Makes_Kecleon_Six_Types_With_React_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C(Kecleon));
            e.ScenarioActive(1, C(Sentret));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Colorless));
            e.ScenarioAttach(kecleon, C(React));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Colorless));
            foreach (var t in new[] { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting })
                Assert.IsTrue(e.IsType(kecleon, t), t.ToString());
        }

        [Test]
        public void Giant_Stump_Cuts_Each_Bench_To_3_Without_Prizes()
        {
            var me = new ScriptedAgent().Then("Play Giant Stump", "Sentret", "Sentret");
            var e = Scenario(me, new ScriptedAgent().Then("Sentret", "Sentret"));
            e.ScenarioActive(0, C(Sentret));
            e.ScenarioActive(1, C(Sentret));
            for (int i = 0; i < 5; i++)
            {
                e.ScenarioBench(0, C(Sentret));
                e.ScenarioBench(1, C(Sentret));
            }
            e.ScenarioAddToHand(0, C(GiantStump));
            e.ScenarioAddToHand(0, C(Shuppet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            for (int p = 0; p < 2; p++)
            {
                Assert.AreEqual(3, e.State.Players[p].Bench.Count);
                Assert.AreEqual(6, e.State.Players[p].Prizes.Count, "they aren't Knocked Out");
                Assert.AreEqual(2, e.State.Players[p].Discard.Count(c => c.Def.Name == "Sentret"));
            }
            Assert.IsFalse(Offered(e, 0, "Bench Shuppet"), "3 is a full Bench now");
        }

        [Test]
        public void Full_Flame_Burns_For_4_Counters_Even_With_Fiery_Aura()
        {
            foreach (bool aura in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(aura ? Rapidash : Sentret));
                var burned = e.ScenarioActive(1, C(Kecleon));
                Assert.AreEqual(aura ? 4 : 2, e.BurnCounters(burned));
                e.ScenarioSetStadium(0, C(FullFlame));
                Assert.AreEqual(4, e.BurnCounters(burned), "4 either way (Compendium: they don't add up)");
            }
        }

        [Test]
        public void Strange_Cave_Puts_Omanyte_From_The_Hand_Onto_The_Bench_As_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Strange Cave"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sentret));
            e.ScenarioActive(1, C(Sentret));
            e.ScenarioSetStadium(0, C(StrangeCave));
            e.ScenarioAddToHand(0, C(Omanyte));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Bench Omanyte"), "a Stage 1 Pokémon can't be benched by itself");
            Step(e);
            var omanyte = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Omanyte", omanyte.Name);
            Assert.IsTrue(omanyte.IsBasic);
            Assert.IsFalse(Offered(e, 0, "Use Strange Cave"), "once a turn");
        }
    }
}
