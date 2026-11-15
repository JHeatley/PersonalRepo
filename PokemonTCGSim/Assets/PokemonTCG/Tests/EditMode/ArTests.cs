using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v173: Arceus (AR): the card data and prints (any number of Arceus in a deck), and the new effects (Keystone Seal, Expert
    /// Belt, Lucky Egg, Bench Shield, Cloak Evolution, Level-Down, Fire Formation, Wild Guard, Multitype).
    /// </summary>
    public class ArTests : ScenarioTestBase
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
        public void Arceus_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(100, db.Cards.Count(d => d.SetCode == "AR"));
            Assert.AreEqual("DPP-53", db.MainPrinting(C("AR-94")).Key, "Arceus LV.X (Omniscient)");
            Assert.AreEqual("DR-83", db.MainPrinting(C("AR-84")).Key, "Buffer Piece");
            Assert.IsTrue(C("AR-AR1").AnyNumberInDeck);
            Assert.IsTrue(C("AR-96").AnyNumberInDeck, "Arceus LV.X");
        }

        [Test]
        public void Every_Arceus_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "AR" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Keystone_Seal_Stops_Items_For_Both_Players()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("PL-115"));
            e.ScenarioAddToDiscard(0, C(Chansey), 1);
            e.ScenarioActive(1, C("AR-32"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Pokémon Rescue"));
        }

        [Test]
        public void Expert_Belt_Adds_Hp_Damage_And_A_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Super Psy Bolt"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C(MewEx));
            Attach(e, mew, Psychic, 2);
            int hp = e.MaxHp(mew);
            e.ScenarioAttachTool(mew, C("AR-87"));
            Assert.AreEqual(hp + 20, e.MaxHp(mew));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, chansey.DamageCounters, "Super Psy Bolt 30 + 20");
        }

        [Test]
        public void Lucky_Egg_Draws_Up_To_7_After_A_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.DamageCounters = e.MaxHp(chansey) / 10 - 1;
            e.ScenarioAttachTool(chansey, C("AR-88"));
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Bench_Shield_Protects_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C(Chansey));
            var bench = e.ScenarioBench(0, C(Chansey));
            e.ScenarioAttachTool(bench, C("AR-83"));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, "MEE-11", 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, bench.DamageCounters);
        }

        [Test]
        public void Burmy_Evolves_The_Turn_It_Is_Played()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var burmy = e.ScenarioBench(0, C("AR-56"));
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("AR-49"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            burmy.TurnEnteredPlay = e.State.TurnNumber;
            Assert.IsTrue(Offered(e, 0, "Wormadam Plant Cloak"));
        }

        [Test]
        public void Level_Down_Shuffles_A_Level_Up_Card_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Level-Down"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C("AR-17"));
            e.ScenarioEvolve(gengar, C("AR-97"));
            var dialga = e.ScenarioActive(1, C("PL-7"));
            e.ScenarioEvolve(dialga, C("PL-122"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Dialga G", dialga.Name);
            Assert.AreEqual(1, e.State.Players[1].Deck.Count(c => c.Def.Name == "Dialga G LV.X"));
        }

        [Test]
        public void Wild_Guard_Stops_Pokemon_SP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Team Attack"));
            var rapidash = e.ScenarioActive(0, C("AR-28"));
            e.ScenarioBench(0, C(Chansey));
            var weavile = e.ScenarioActive(1, C("PL-17"));
            Attach(e, weavile, "MEE-15", 3);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, rapidash.DamageCounters);
        }

        [Test]
        public void Multitype_Makes_Arceus_Lv_X_The_Type_Under_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var arceus = e.ScenarioActive(0, C("AR-AR4"));
            e.ScenarioEvolve(arceus, C("AR-96"));
            e.ScenarioActive(1, C(Chansey));
            Assert.AreEqual(C("AR-AR4").PokemonType, e.TypesOf(arceus).Single());
        }
    }
}
