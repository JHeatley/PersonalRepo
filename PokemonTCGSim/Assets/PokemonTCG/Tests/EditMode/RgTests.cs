using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v162: FireRed &amp; LeafGreen (RG): the card data and prints, Energy Flame, Magic Odds / Evens, Mt. Moon, Family Bonds,
    /// Retreat Aid, Acid Sampler, Thick Skin, Legendary Ascent and Prof. Oak's Research.
    /// </summary>
    public class RgTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Fire = "MEE-10";
        const string Grass = "MEE-9";

        const string Kangaskhan = "RG-6";
        const string Nidoqueen = "RG-9";
        const string Slowbro = "RG-14";
        const string Snorlax = "RG-15";
        const string Victreebel = "RG-17";
        const string Dodrio = "RG-21";
        const string Ivysaur = "RG-35";
        const string Nidorino = "RG-41";
        const string Persian = "RG-44";
        const string MtMoon = "RG-94";
        const string ProfOaksResearch = "RG-98";
        const string CharizardEx = "RG-105";
        const string MrMimeOdds = "RG-110";
        const string MrMimeEvens = "RG-111";
        const string GyaradosEx = "RG-109";
        const string ZapdosEx = "RG-116";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void FireRed_And_LeafGreen_Is_In_The_Pool_With_Its_Prints()
        {
            var set = db.Cards.Where(d => d.SetCode == "RG").ToList();
            Assert.AreEqual(102, set.Count, "116 cards minus 14 prints");
            Assert.AreEqual(116, db.All.Count(d => d.SetCode == "RG"));
            Assert.AreEqual("DR-98", db.MainPrinting(db.Get("RG-113")).Key);
            Assert.AreEqual("N4-93", db.MainPrinting(db.Get("RG-91")).Key, "EXP.ALL is a print, like Life Herb");
            Assert.AreEqual("SS-93", db.MainPrinting(db.Get("RG-103")).Key);
            Assert.IsTrue(C(CharizardEx).IsOldEx);
        }

        [Test]
        public void Energy_Flame_Makes_Every_Energy_Fire()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C(CharizardEx));
            Attach(e, charizard, Water, 5);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Burn Down"));
        }

        [Test]
        public void Magic_Evens_Stops_20_And_Magic_Odds_Does_Not()
        {
            foreach (bool evens in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Headbutt"), new ScriptedAgent());
                var kangaskhan = e.ScenarioActive(0, C(Kangaskhan));
                Attach(e, kangaskhan, Fire, 2);
                var mime = e.ScenarioActive(1, C(evens ? MrMimeEvens : MrMimeOdds));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(evens ? 0 : 2, mime.DamageCounters);
            }
        }

        [Test]
        public void Mt_Moon_Stops_Poke_Powers_Of_Pokemon_Under_70_HP()
        {
            foreach (bool moon in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(Slowbro));
                var hurt = e.ScenarioBench(0, C(Chansey));
                hurt.DamageCounters = 2;
                e.ScenarioActive(1, C(Chansey));
                if (moon) e.ScenarioSetStadium(1, C(MtMoon));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(!moon, Offered(e, 0, "Use Strange Behavior"));
            }
        }

        [Test]
        public void Family_Bonds_Removes_The_Nido_Retreat_Cost()
        {
            foreach (bool queen in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var nidorino = e.ScenarioActive(0, C(Nidorino));
                if (queen) e.ScenarioBench(0, C(Nidoqueen));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(queen ? 0 : 1, e.RetreatCostOf(nidorino));
            }
        }

        [Test]
        public void Retreat_Aid_Helps_Your_Active_But_Not_Pokemon_ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            e.ScenarioBench(0, C(Dodrio));
            var gyarados = e.ScenarioActive(1, C(GyaradosEx));
            e.ScenarioBench(1, C(Dodrio));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.RetreatCostOf(snorlax));
            Assert.AreEqual(3, e.RetreatCostOf(gyarados));
        }

        [Test]
        public void Acid_Sampler_Puts_A_Counter_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Victreebel));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, chansey.DamageCounters);
        }

        [Test]
        public void Thick_Skin_Stops_Poison_Seed()
        {
            foreach (bool persian in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Poison Seed"), new ScriptedAgent());
                var ivysaur = e.ScenarioActive(0, C(Ivysaur));
                Attach(e, ivysaur, Grass, 1);
                var target = e.ScenarioActive(1, C(persian ? Persian : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(!persian, target.Has(SpecialCondition.Poisoned));
            }
        }

        [Test]
        public void Legendary_Ascent_Switches_Zapdos_ex_In_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Zapdos ex"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ZapdosEx));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Zapdos ex", e.State.Players[0].Active.Name);
        }

        [Test]
        public void Prof_Oaks_Research_Shuffles_The_Hand_And_Draws_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Prof. Oak's Research"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ProfOaksResearch));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(18, e.State.Players[0].Deck.Count, "20 + 3 shuffled in - 5 drawn");
        }
    }
}
