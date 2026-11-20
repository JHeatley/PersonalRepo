using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: HeartGold &amp; SoulSilver (HS): the card data and prints, the LEGEND cards, and the new effects (Sacred Rainbow, Ocean
    /// Grow, Sweet Sleeping Face, Conductivity, Playground, Pokémon Collector, Shell Barricade).
    /// </summary>
    public class HsTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void HeartGold_SoulSilver_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(95, db.Cards.Count(d => d.SetCode == "HS"));
            Assert.AreEqual("HSP-12", db.MainPrinting(C("HS-17")).Key, "Cleffa");
            Assert.AreEqual("CES-151", db.MainPrinting(C("HS-104")).Key, "Rainbow Energy");
            Assert.IsTrue(C("HS-111").IsLegend);
            Assert.AreEqual("top", C("HS-111").LegendHalf);
            Assert.AreEqual("bottom", C("HS-112").LegendHalf);
        }

        [Test]
        public void Every_HeartGold_SoulSilver_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "HS" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Sacred_Rainbow_Makes_All_Energy_Fire()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hoOh = e.ScenarioActive(0, C("HS-111"));
            Attach(e, hoOh, Psychic, 4);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(4, e.EnergyCountOfType(hoOh, EnergyType.Fire));
            Assert.IsTrue(e.CanPayAttack(hoOh, hoOh.Def.Attacks.First(a => a.Name == "Bright Wing")));
        }

        [Test]
        public void Lugia_Legend_Benches_Both_Halves_And_Ocean_Grow_Attaches_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Lugia LEGEND", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("HS-113"));
            e.ScenarioAddToHand(0, C("HS-114"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var lugia = e.State.Players[0].Bench.FirstOrDefault(m => m.Name == "Lugia LEGEND");
            Assert.IsNotNull(lugia);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "both halves");
            Assert.AreEqual(5, lugia.Energy.Count, "the top 5 cards were all Energy");
        }

        [Test]
        public void Sweet_Sleeping_Face_Stops_Damage_While_Asleep()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var pichu = e.ScenarioActive(0, C("HS-28"));
            pichu.Conditions = SpecialCondition.Asleep;
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, pichu.DamageCounters);
        }

        [Test]
        public void Conductivity_Hurts_Pokemon_The_Opponent_Attaches_To()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attach Psychic Energy"));
            e.ScenarioActive(0, C("HS-105"));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual(1, chansey.DamageCounters);
        }

        [Test]
        public void Playground_Benches_Basic_Pokemon_For_Both_Players()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Playground", "Chansey|Chansey"), new ScriptedAgent().Then("Chansey"));
            var pichu = e.ScenarioActive(0, C("HS-28"));
            e.ScenarioAddToDeck(0, C(Chansey), 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(1, C(Chansey), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
            Assert.AreEqual(1, e.State.Players[1].Bench.Count);
            Assert.IsTrue(pichu.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Pokemon_Collector_Finds_3_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Pokémon Collector", "Chansey|Chansey|Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToDeck(0, C(Chansey), 3);
            e.ScenarioAddToHand(0, C("HS-97"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count(c => c.Def.Name == "Chansey"));
        }

        [Test]
        public void Shell_Barricade_Protects_Shuckle_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C(Chansey));
            var shuckle = e.ScenarioBench(0, C("HS-11"));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, "MEE-11", 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, shuckle.DamageCounters);
        }
    }
}
