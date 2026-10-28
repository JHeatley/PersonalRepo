using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v167: POP Series 2 (P2): the card data and prints, and Celebi ex's Time Reversal.</summary>
    public class P2Tests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string CelebiEx = "P2-17";
        const string Chansey = "E2-69";

        [Test]
        public void Pop_Series_2_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(10, db.Cards.Count(d => d.SetCode == "P2"), "17 cards minus 7 prints");
            Assert.AreEqual(17, db.All.Count(d => d.SetCode == "P2"));
            Assert.AreEqual("E1-23", db.MainPrinting(db.Get("P2-2")).Key);
            Assert.AreEqual("RG-35", db.MainPrinting(db.Get("P2-7")).Key);
            Assert.AreEqual("30C-CC1", db.MainPrinting(db.Get("P2-16")).Key);
            Assert.IsTrue(C(CelebiEx).IsOldEx);
        }

        [Test]
        public void Time_Reversal_Puts_A_Discarded_Card_On_Top_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Celebi ex"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(CelebiEx));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("MEE-10", e.State.Players[0].Deck[0].Def.Key);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }
    }
}
