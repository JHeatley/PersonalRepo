using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: HS—Undaunted (UD): the card data and prints, and the new effects (Allergy Flower, Retreat Aid, Defense Sign, Red
    /// Armor, Space Virus, Defender, Ruins of Alph, Legend Box, Evolution Memories).
    /// </summary>
    public class UdTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Chansey = "E2-69";
        const string MewEx = "HP-100";

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        [Test]
        public void Undaunted_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(86, db.Cards.Count(d => d.SetCode == "UD"));
            Assert.AreEqual("HSP-24", db.MainPrinting(C("UD-51")).Key, "Hitmonchan");
            Assert.AreEqual("N1-104", db.MainPrinting(C("UD-79")).Key, "Darkness Energy");
        }

        [Test]
        public void Every_Undaunted_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "UD" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Allergy_Flower_Stops_Items()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("UD-24"));
            e.ScenarioAddToHand(0, C("PL-115"));
            e.ScenarioAddToDiscard(0, C(Chansey), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Pokémon Rescue"));
        }

        [Test]
        public void Retreat_Aid_Lowers_The_Active_Pokemons_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            int before = e.RetreatCostOf(chansey);
            e.ScenarioBench(0, C("UD-11"));
            Assert.AreEqual(System.Math.Max(0, before - 2), e.RetreatCostOf(chansey));
        }

        [Test]
        public void Defense_Sign_Protects_Benched_Grass_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C(Chansey));
            var turtwig = e.ScenarioBench(0, C("PL-101"));
            e.ScenarioBench(0, C("UD-23"));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, "MEE-11", 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, turtwig.DamageCounters);
        }

        [Test]
        public void Red_Armor_Stops_Attackers_With_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var scizor = e.ScenarioActive(0, C("UD-84"));
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 1);
            Attach(e, mew, "RR-102", 1);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, scizor.DamageCounters);
        }

        [Test]
        public void Defender_Reduces_Damage_And_Leaves_After_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Defender"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C(Chansey));
            e.ScenarioAddToHand(0, C("UD-72"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, chansey.Tools.Count);
            e.EndTurn();
            Assert.AreEqual(1, chansey.Tools.Count, "stays through its owner's turn");
            e.StartTurn();
            Step(e);
            Assert.AreEqual(1, chansey.DamageCounters, "30 - 20");
            e.EndTurn();
            Assert.AreEqual(0, chansey.Tools.Count);
        }

        [Test]
        public void Ruins_Of_Alph_Removes_Resistance()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var m = e.ScenarioActive(0, C("PL-12"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(0, C("UD-76"));
            Assert.AreEqual(0, e.Scripts.For(C("UD-76")).ModifyResistanceAmount(null, m, 20));
        }

        [Test]
        public void Legend_Box_Benches_A_Revealed_Legend_With_Its_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Legend Box"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C("HS-113"), 1);
            e.ScenarioAddToDeck(0, C(Psychic), 3);
            e.ScenarioAddToDeck(0, C("HS-114"), 1);
            e.ScenarioAddToHand(0, C("UD-75"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var lugia = e.State.Players[0].Bench.FirstOrDefault(x => x.Name == "Lugia LEGEND");
            Assert.IsNotNull(lugia);
            Assert.AreEqual(2, lugia.Stack.Count);
            Assert.AreEqual(3, lugia.Energy.Count);
        }

        [Test]
        public void Evolution_Memories_Offers_The_Attacks_Of_Eevee_Evolutions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var espeon = e.ScenarioActive(0, C("UD-47"));
            e.ScenarioEvolve(espeon, C("UD-81"));
            Attach(e, espeon, "MEE-10", 3);
            var flareon = e.ScenarioBench(0, C("UD-47"));
            e.ScenarioEvolve(flareon, C("UD-26"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Flamethrower"));
        }
    }
}
