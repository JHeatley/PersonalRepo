using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Great Encounters (GE): the card data and prints, and the new effects (Wild Growth, Wash Out, Toxic Virus, GUARD, LINK,
    /// Conversion, Moon Skip, Amulet Coin, Moonlight Stadium, Felicity's Drawing, Premier Ball).
    /// </summary>
    public class GeTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void Great_Encounters_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(98, db.Cards.Count(d => d.SetCode == "GE"));
            for (int n = 1; n <= 106; n++) Assert.IsNotNull(db.Get("GE", n.ToString()), "GE " + n);
            Assert.AreEqual("DPP-17", db.MainPrinting(C("GE-105")).Key, "Dialga LV.X");
            Assert.AreEqual("CEL-CC16", db.MainPrinting(C("GE-15")).Key, "Claydol");
        }

        [Test]
        public void Every_Great_Encounters_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "GE" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Wild_Growth_Doubles_Basic_Grass_Energy_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var treecko = e.ScenarioActive(0, C("GE-90"));
            Attach(e, treecko, Grass, 1);
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(1, e.EnergyUnits(treecko).Count);
            e.ScenarioBench(0, C("GE-8"));
            Assert.AreEqual(2, e.EnergyUnits(treecko).Count);
            e.ScenarioBench(0, C("GE-8"));
            Assert.AreEqual(2, e.EnergyUnits(treecko).Count, "only 1 Wild Growth counts");
        }

        [Test]
        public void Wash_Out_Moves_Water_Energy_To_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Wash Out"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("GE-9"));
            var donor = e.ScenarioBench(0, C(Chansey));
            Attach(e, donor, Water, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, active.Energy.Count);
            Assert.AreEqual(0, donor.Energy.Count);
        }

        [Test]
        public void Toxic_Virus_Adds_A_Poison_Counter()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C("GE-31"));
            e.ScenarioSetTurn(0, 3);
            chansey.Conditions = SpecialCondition.Poisoned;
            e.EndTurn();
            Assert.AreEqual(2, chansey.DamageCounters);
        }

        [Test]
        public void Guard_Attaches_Unown_G_And_Stops_Attack_Effects()
        {
            var e = Scenario(new ScriptedAgent().Then("Use GUARD"), new ScriptedAgent().Then("Attack: Hypnosis"));
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("GE-57"));
            var drowzee = e.ScenarioActive(1, C("GE-65"));
            Attach(e, drowzee, Psychic, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(chansey.HasToolNamed("Unown [G]"));
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.AreEqual(SpecialCondition.None, chansey.Conditions & SpecialCondition.Asleep);
        }

        [Test]
        public void Link_Lets_Unown_L_Use_Other_Unowns_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var link = e.ScenarioActive(0, C("GE-91"));
            Attach(e, link, Psychic, 2);
            e.ScenarioBench(0, C("GE-29"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.GetMainActions(0).Count(o => o.Label.Contains("Hidden Power")), "its own and Unown H's");
        }

        [Test]
        public void Conversion_Changes_Porygon_Zs_Type()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Conversion"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C("GE-6"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEqual(new[] { EnergyType.Water }, e.TypesOf(porygon));
        }

        [Test]
        public void Moon_Skip_Takes_1_More_Prize_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Moon Skip"), new ScriptedAgent());
            var cresselia = e.ScenarioActive(0, C("GE-2"));
            e.ScenarioEvolve(cresselia, C("GE-103"));
            Attach(e, cresselia, Psychic, 3);
            var slakoth = e.ScenarioActive(1, C("MT-99"));
            slakoth.DamageCounters = 2;
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Amulet_Coin_Draws_At_The_End_Of_Your_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAttachTool(chansey, C("GE-97"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Moonlight_Stadium_Frees_Psychic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var drowzee = e.ScenarioActive(0, C("GE-65"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(chansey);
            e.ScenarioSetStadium(0, C("GE-100"));
            Assert.AreEqual(0, e.RetreatCostOf(drowzee));
            Assert.AreEqual(before, e.RetreatCostOf(chansey));
        }

        [Test]
        public void Felicitys_Drawing_Discards_2_And_Draws_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Felicity's Drawing", "Grass|Grass"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("GE-98"));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Premier_Ball_Takes_A_Level_X_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Premier Ball", "Discard"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDiscard(0, C("GE-103"), 1);
            e.ScenarioAddToHand(0, C("GE-101"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Cresselia LV.X"));
        }
    }
}
