using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v172: POP Series 6, 7 and 8: the new cards, their prints, and Riolu's Inner Focus.</summary>
    public class PopDpTests : ScenarioTestBase
    {
        [Test]
        public void Pop_6_And_8_Are_In_The_Pool_And_Pop_7_Is_All_Prints()
        {
            Assert.AreEqual(3, db.Cards.Count(d => d.SetCode == "P6"));
            Assert.AreEqual(7, db.Cards.Count(d => d.SetCode == "P8"));
            Assert.AreEqual(0, db.Cards.Count(d => d.SetCode == "P7"));
            Assert.AreEqual("DP-46", db.MainPrinting(C("P6-6")).Key);
            Assert.AreEqual("GE-22", db.MainPrinting(db.Get("P7", "3")).Key);
        }

        [Test]
        public void Every_Pop_6_And_8_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => (d.SetCode == "P6" || d.SetCode == "P8") && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Inner_Focus_Stops_Paralysis()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var riolu = e.ScenarioActive(0, C("P8-16"));
            e.ScenarioActive(1, C("E2-69"));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.ApplyCondition(riolu, SpecialCondition.Paralyzed));
        }
    }
}
