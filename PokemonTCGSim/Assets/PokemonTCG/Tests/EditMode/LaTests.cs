using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Legends Awakened (LA): the card data and prints, and the new effects (Form Change, Slow Start, Fossil Armor, White
    /// Smoke, Doom Desire, Destiny Bond, Ditto DNA, Snowpoint Temple, Cynthia's Feelings, the Technical Machines).
    /// </summary>
    public class LaTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Metal = "MEE-16";
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
        public void Legends_Awakened_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(143, db.Cards.Count(d => d.SetCode == "LA"));
            for (int n = 1; n <= 146; n++) Assert.IsNotNull(db.Get("LA", n.ToString()), "LA " + n);
            Assert.AreEqual("DPP-28", db.MainPrinting(C("LA-144")).Key, "Mewtwo LV.X");
            Assert.IsTrue(C("LA-138").FossilGivesPrize, "Claw Fossil gives a Prize card");
        }

        [Test]
        public void Every_Legends_Awakened_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "LA" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Form_Change_Swaps_In_Another_Deoxys()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Form Change"), new ScriptedAgent());
            var deoxys = e.ScenarioActive(0, C("LA-1"));
            deoxys.DamageCounters = 2;
            e.ScenarioAddToDeck(0, C("LA-26"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Deoxys Speed Forme", deoxys.Name);
            Assert.AreEqual(2, deoxys.DamageCounters, "damage counters stay");
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Name == "Deoxys Normal Forme"));
        }

        [Test]
        public void Slow_Start_Waits_For_3_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var gigas = e.ScenarioActive(0, C("LA-15"));
            foreach (var t in new[] { Water, "MEE-14", Metal, Psychic }) Attach(e, gigas, t, 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Crush Grip"));
            while (e.State.Players[1].Prizes.Count > 3) e.State.Players[1].Prizes.RemoveAt(0);
            Assert.IsTrue(Offered(e, 0, "Crush Grip"));
        }

        [Test]
        public void Fossil_Armor_Stops_60_Damage_Or_Less()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var armaldo = e.ScenarioActive(0, C("LA-18"));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, armaldo.DamageCounters);
        }

        [Test]
        public void White_Smoke_Protects_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Ice Shot"));
            e.ScenarioActive(0, C("LA-41"));
            var bench = e.ScenarioBench(0, C(Chansey));
            var glaceon = e.ScenarioActive(1, C("MD-20"));
            Attach(e, glaceon, Water, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, bench.DamageCounters);
        }

        [Test]
        public void Doom_Desire_Knocks_Out_The_Defending_Pokemon_A_Turn_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Doom Desire"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C("LA-31"));
            Attach(e, jirachi, Metal, 2);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, jirachi.Energy.Count);
            e.EndTurn();
            e.StartTurn();
            Assert.IsTrue(e.State.Players[1].InPlay().Contains(chansey), "still there during its own turn");
            e.EndTurn();
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(chansey));
        }

        [Test]
        public void Destiny_Bond_Takes_The_Attacker_Down_Too()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Destiny Bond"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var froslass = e.ScenarioActive(0, C("LA-3"));
            Attach(e, froslass, Psychic, 1);
            froslass.DamageCounters = 7;
            e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(mew), "Mew ex is Knocked Out by Destiny Bond");
        }

        [Test]
        public void Ditto_Dna_Copies_The_Opponents_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C("LA-27"));
            Attach(e, ditto, Psychic, 2);
            var mew = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(e.MaxHp(mew), e.MaxHp(ditto));
            Assert.IsTrue(Offered(e, 0, "Super Psy Bolt"));
        }

        [Test]
        public void Snowpoint_Temple_Gives_Unevolved_Pokemon_20_Hp()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            var gyarados = e.ScenarioActive(1, C("MT-89"));
            e.ScenarioEvolve(gyarados, C("MT-26"));
            int a = e.MaxHp(chansey), b = e.MaxHp(gyarados);
            e.ScenarioSetStadium(0, C("LA-134"));
            Assert.AreEqual(a + 20, e.MaxHp(chansey));
            Assert.AreEqual(b, e.MaxHp(gyarados));
        }

        [Test]
        public void Cynthias_Feelings_Draws_8_After_A_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cynthia's Feelings"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("LA-131"));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].LastKnockedOutTurn = 2;
            Step(e);
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Technical_Machine_Ts2_Attaches_And_Devolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Technical Machine TS-2", "Attack: Devoluter"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C("LA-137"));
            var gyarados = e.ScenarioActive(1, C("MT-89"));
            e.ScenarioEvolve(gyarados, C("MT-26"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Active.Tools.Any(t => t.Def.Name == "Technical Machine TS-2"));
            Step(e);
            Assert.AreEqual("Magikarp", gyarados.Name);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "Gyarados"));
        }
    }
}
