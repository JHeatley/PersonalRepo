using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v171: POP Series 3, 4 and 5 (P3, P4, P5): the card data and prints, and the new effects.</summary>
    public class PopExTests : ScenarioTestBase
    {
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Water = "MEE-11";
        const string Metal = "MEE-16";
        const string Fire = "MEE-10";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void The_POP_Series_Are_In_The_Pool_With_Their_Prints()
        {
            Assert.AreEqual(14, db.Cards.Count(d => d.SetCode == "P3"));
            Assert.AreEqual(9, db.Cards.Count(d => d.SetCode == "P4"));
            Assert.AreEqual(9, db.Cards.Count(d => d.SetCode == "P5"));
            for (int n = 1; n <= 17; n++)
                foreach (var set in new[] { "P3", "P4", "P5" }) Assert.IsNotNull(db.Get(set, n.ToString()), set + " " + n);
            Assert.AreEqual("DR-85", db.MainPrinting(C("P3-10")).Key, "High Pressure System");
            Assert.AreEqual("CEL-CC15", db.MainPrinting(C("P5-17")).Key, "Umbreon ★");
            Assert.AreEqual("P4-13", db.MainPrinting(C("P4-13")).Key, "Pikachu's Spark costs LC: its own card");
        }

        [Test]
        public void Every_POP_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => (d.SetCode == "P3" || d.SetCode == "P4" || d.SetCode == "P5")
                                            && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Cheer_Up_Draws_2_With_The_Partner_In_Play()
        {
            foreach (bool partner in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Cheer Up"), new ScriptedAgent());
                var minun = e.ScenarioActive(0, C("P3-4"));
                Attach(e, minun, Lightning, 1);
                if (partner) e.ScenarioBench(0, C("P3-5"));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(partner ? 2 : 1, e.State.Players[0].Hand.Count);
            }
        }

        [Test]
        public void Power_Bolt_Only_Hits_Pokemon_With_A_Poke_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Power Bolt"), new ScriptedAgent());
            var minun = e.ScenarioActive(0, C("P3-4"));
            Attach(e, minun, Lightning, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            var mew = e.ScenarioBench(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.DamageCounters);
            Assert.AreEqual(3, mew.DamageCounters, "Mew ex has Psychic Vision (Poké-Power)");
        }

        [Test]
        public void Water_Gun_Does_20_More_For_Each_Extra_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Water Gun"), new ScriptedAgent());
            var vaporeon = e.ScenarioActive(0, C("P3-6"));
            Attach(e, vaporeon, Water, 4);
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, target.DamageCounters, "30 + 2 x 20 (no more than 40 more)");
        }

        [Test]
        public void Sonicboom_Ignores_Weakness_And_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sonicboom"), new ScriptedAgent());
            var chimecho = e.ScenarioActive(0, C("P4-1"));
            Attach(e, chimecho, Metal, 2);
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
        }

        [Test]
        public void Heavy_Metal_Flips_For_Each_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Metal"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C("P5-13"));
            Attach(e, pikachu, Metal, 2);
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.That(target.DamageCounters, Is.EqualTo(1).Or.EqualTo(3).Or.EqualTo(5));
        }

        [Test]
        public void Purple_Ray_Confuses_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Espeon", "Yes"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("P5-16"));
            var theirs = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(mine.Has(SpecialCondition.Confused) && theirs.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Extra_Draw_Attaches_Energy_Only_Against_A_Pokemon_ex()
        {
            foreach (bool ex in new[] { false, true })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Extra Draw", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
                var mew = e.ScenarioActive(0, C("P5-3"));
                Attach(e, mew, Fire, 1);
                e.ScenarioActive(1, C(ex ? MewEx : Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(ex ? 3 : 1, mew.Energy.Count);
            }
        }

        [Test]
        public void Mist_Cuts_Damage_From_Stage_2_Evolved_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Rock Hurl"));
            var pelipper = e.ScenarioActive(0, C("P5-14"));
            var swampert = e.ScenarioActive(1, C("CG-58"));
            e.ScenarioEvolve(swampert, C("CG-38"));
            e.ScenarioEvolve(swampert, C("CG-27"));
            Attach(e, swampert, "MEE-14", 3);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(3, pelipper.DamageCounters, "60 - 30");
        }
    }
}
