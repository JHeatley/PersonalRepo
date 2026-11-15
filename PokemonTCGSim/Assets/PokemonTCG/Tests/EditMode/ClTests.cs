using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: Call of Legends (CL): the card data and prints (most of it reprints earlier HGSS cards), and the new effects (Lost
    /// World, Lost Remover, Dimension Sphere, Research Record, Extreme Speed, Destructive Tsunami).
    /// </summary>
    public class ClTests : ScenarioTestBase
    {
        const string Chansey = "E2-69";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Call_Of_Legends_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(41, db.Cards.Count(d => d.SetCode == "CL"));
            Assert.AreEqual("HS-4", db.MainPrinting(C("CL-7")).Key, "Gyarados");
            Assert.AreEqual("HS-12", db.MainPrinting(C("CL-32")).Key, "Slowking");
            Assert.AreEqual("HS-101", db.MainPrinting(C("CL-83")).Key, "Professor Oak's New Theory");
        }

        [Test]
        public void Every_Call_Of_Legends_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "CL" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Lost_World_Wins_With_6_Pokemon_In_The_Opponents_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(0, C("CL-81"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Lost World"));
            for (int i = 0; i < 6; i++) { e.State.Players[1].LostZone.Add(e.ScenarioAddToHand(1, C(Chansey))); }
            e.State.Players[1].Hand.Clear();
            Assert.IsTrue(Offered(e, 0, "Lost World"));
        }

        [Test]
        public void Lost_Remover_Sends_Special_Energy_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lost Remover"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("CL-80"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttach(chansey, C("RR-102"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.Energy.Count);
            Assert.AreEqual(1, e.State.Players[1].LostZone.Count);
        }

        [Test]
        public void Dimension_Sphere_Counts_Your_Lost_Zone_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dimension Sphere"), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C("CL-14"));
            e.ScenarioAttach(lucario, C("MEE-14"));
            e.ScenarioAttach(lucario, C("MEE-14"));
            e.State.Players[0].LostZone.Add(e.ScenarioAddToHand(0, C(Chansey)));
            e.State.Players[0].Hand.Clear();
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int expected = 5 * (e.WeaknessOf(chansey) == EnergyType.Fighting ? 2 : 1);
            Assert.AreEqual(expected, chansey.DamageCounters, "30 + 20 for the Pokémon in the Lost Zone");
        }

        [Test]
        public void Extreme_Speed_Lowers_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var raikou = e.ScenarioActive(0, C("CL-SL9"));
            e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(raikou);
            e.ScenarioAttach(raikou, C("MEE-12"));
            Assert.AreEqual(System.Math.Max(0, before - 1), e.RetreatCostOf(raikou));
        }

        [Test]
        public void Research_Record_Keeps_Some_Cards_On_Top()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Research Record", "Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C("MEE-13"), 3);
            e.State.Players[0].Deck.Insert(2, e.ScenarioAddToHand(0, C(Chansey)));
            e.State.Players[0].Hand.Clear();
            e.ScenarioAddToDeck(0, C("MEE-10"), 2);
            e.ScenarioAddToHand(0, C("CL-84"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var deck = e.State.Players[0].Deck;
            Assert.AreEqual("Chansey", deck[0].Def.Name);
            Assert.AreEqual(6, deck.Count);
        }

        [Test]
        public void Destructive_Tsunami_Hits_One_Side()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Destructive Tsunami"), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C("CL-12"));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(kyogre, C("MEE-11"));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int mine = e.State.Players[0].InPlay().Sum(m => m.DamageCounters);
            int theirs = e.State.Players[1].InPlay().Sum(m => m.DamageCounters);
            Assert.IsTrue((mine == 0) != (theirs == 0), "only one player's Pokémon are hit");
        }
    }
}
