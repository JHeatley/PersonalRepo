using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v173: POP Series 9 (P9): the card data and prints, and a few effects (High Volt, Dragon Rage, Growl).</summary>
    public class P9Tests : ScenarioTestBase
    {
        const string Lightning = "MEE-12";
        const string Chansey = "E2-69";

        [Test]
        public void Pop_Series_9_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(16, db.Cards.Count(d => d.SetCode == "P9"));
            Assert.AreEqual("MT-85", db.MainPrinting(C("P9-14")).Key, "Gible");
        }

        [Test]
        public void Every_Pop_Series_9_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "P9" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void High_Volt_Does_100_The_Turn_Raichu_Evolves_From_Pikachu()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: High Volt"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C("P9-15"));
            e.ScenarioEvolve(raichu, C("P9-3"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(raichu, C(Lightning));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            raichu.TurnEvolved = e.State.TurnNumber;
            Step(e);
            Assert.AreEqual(10, chansey.DamageCounters);
        }

        [Test]
        public void Growl_Weakens_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Growl"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var pikachu = e.ScenarioActive(0, C("P9-15"));
            e.ScenarioAttach(pikachu, C(Lightning));
            var mew = e.ScenarioActive(1, C("HP-100"));
            for (int i = 0; i < 2; i++) e.ScenarioAttach(mew, C("MEE-13"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Step(e);
            int full = mew.Def.Attacks.First(a => a.Name == "Super Psy Bolt").Damage;
            Assert.AreEqual((full - 20) / 10, pikachu.DamageCounters);
        }
    }
}
