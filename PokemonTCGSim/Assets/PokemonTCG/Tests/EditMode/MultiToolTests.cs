using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v38: Pokémon can hold more than one Pokémon Tool (Rotom ex's Multi Adapter).</summary>
    public class MultiToolTests : ScenarioTestBase
    {
        const string RotomEx = "PFL-29";
        const string WashRotom = "DRI-61";
        const string Remoraid = "PAR-33";
        const string DefianceVest = "PAR-162";
        const string LuxuriousCape = "PAR-166";
        const string ToolScrapper = "ASC-212";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Multi_Adapter_Lets_Your_Rotom_Pokemon_Hold_2_Tools()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Luxurious Cape"), new ScriptedAgent());
            var rotomEx = e.ScenarioActive(0, C(RotomEx));
            var wash = e.ScenarioBench(0, C(WashRotom));
            var remoraid = e.ScenarioBench(0, C(Remoraid));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAttachTool(rotomEx, C(DefianceVest));
            e.ScenarioAttachTool(wash, C(DefianceVest));
            e.ScenarioAttachTool(rotomEx, C(DefianceVest));
            e.ScenarioAttachTool(remoraid, C(DefianceVest));
            e.ScenarioAddToHand(0, C(LuxuriousCape));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(2, e.MaxTools(rotomEx));
            Assert.AreEqual(2, e.MaxTools(wash));
            Assert.AreEqual(1, e.MaxTools(remoraid), "only Pokémon with Rotom in the name");
            Assert.IsTrue(Offered(e, 0, "Play Luxurious Cape"));

            Step(e); // Wash Rotom is the only Pokémon with room, so it gets the Cape
            Assert.AreEqual(2, wash.Tools.Count);
            Assert.IsTrue(wash.HasToolNamed("Luxurious Cape"));
            Assert.AreEqual(180, e.MaxHp(wash), "both Tools work: the Cape's +100 HP");
        }

        [Test]
        public void Without_Multi_Adapter_A_Pokemon_Still_Holds_1_Tool()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wash = e.ScenarioActive(0, C(WashRotom));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAttachTool(wash, C(DefianceVest));
            e.ScenarioAddToHand(0, C(LuxuriousCape));
            e.ScenarioSetTurn(0, 3);

            Assert.AreEqual(1, e.MaxTools(wash));
            Assert.IsFalse(Offered(e, 0, "Play Luxurious Cape"));
        }

        [Test]
        public void Two_Rotom_ex_Still_Allow_Only_2_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var rotomEx = e.ScenarioActive(0, C(RotomEx));
            e.ScenarioBench(0, C(RotomEx));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.MaxTools(rotomEx));
        }

        [Test]
        public void Opponents_Multi_Adapter_Does_Not_Help_Your_Rotom()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wash = e.ScenarioActive(0, C(WashRotom));
            e.ScenarioActive(1, C(RotomEx));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.MaxTools(wash), "\"each of your Pokémon\"");
        }

        [Test]
        public void When_Multi_Adapter_Stops_Working_The_Owner_Discards_Down_To_1_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Luxurious Cape"), new ScriptedAgent());
            var wash = e.ScenarioActive(0, C(WashRotom));
            var rotomEx = e.ScenarioBench(0, C(RotomEx));
            e.ScenarioActive(1, C(Remoraid));
            e.ScenarioAttachTool(wash, C(DefianceVest));
            e.ScenarioAttachTool(wash, C(LuxuriousCape));
            e.ScenarioSetTurn(0, 3);

            e.DiscardPokemonFromPlay(rotomEx);
            e.StateChecks();

            Assert.AreEqual(1, wash.Tools.Count);
            Assert.IsTrue(wash.HasToolNamed("Defiance Vest"), "the owner chose to keep Defiance Vest");
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Luxurious Cape"));
        }

        [Test]
        public void Tool_Scrapper_Can_Discard_Both_Tools_From_One_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Tool Scrapper", "Defiance Vest|Luxurious Cape"), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            var wash = e.ScenarioActive(1, C(WashRotom));
            e.ScenarioBench(1, C(RotomEx));
            e.ScenarioAttachTool(wash, C(DefianceVest));
            e.ScenarioAttachTool(wash, C(LuxuriousCape));
            e.ScenarioAddToHand(0, C(ToolScrapper));
            e.ScenarioSetTurn(0, 3);

            Step(e);
            Assert.AreEqual(0, wash.Tools.Count);
            Assert.AreEqual(2, e.State.Players[1].Discard.Count(c => c.Def.IsTool));
        }

        [Test]
        public void Every_Attached_Tool_Belongs_To_The_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Remoraid));
            var wash = e.ScenarioActive(1, C(WashRotom));
            e.ScenarioBench(1, C(RotomEx));
            e.ScenarioAttachTool(wash, C(DefianceVest));
            e.ScenarioAttachTool(wash, C(LuxuriousCape));
            e.ScenarioSetTurn(0, 3);

            var cards = wash.AllCards();
            Assert.AreEqual(3, cards.Count, "Wash Rotom and both Tools go wherever it goes");
            Assert.AreEqual(2, cards.Count(c => c.Def.IsTool));
            e.DiscardPokemonFromPlay(wash);
            Assert.AreEqual(2, e.State.Players[1].Discard.Count(c => c.Def.IsTool));
        }
    }
}
