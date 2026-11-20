using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Diamond &amp; Pearl (DP): the card data and prints, and the new effects (Unown powers, Intense Voltage, Gleam Eyes,
    /// Magnetize, Harden, Bide, See Beyond, Professor Rowan, Rival, Speed Stadium, DP PlusPower).
    /// </summary>
    public class DpSetTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Lightning = "MEE-12";
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
        public void Diamond_And_Pearl_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(97, db.Cards.Count(d => d.SetCode == "DP"));
            for (int n = 1; n <= 130; n++) Assert.IsNotNull(db.Get("DP", n.ToString()), "DP " + n);
            Assert.AreEqual("DPP-11", db.MainPrinting(C("DP-120")).Key, "Empoleon LV.X");
            Assert.AreEqual("DPP-22", db.MainPrinting(C("DP-90")).Key, "Mime Jr.");
            Assert.AreEqual("DP-109", db.MainPrinting(C("DP-109")).Key, "DP PlusPower is its own card");
        }

        [Test]
        public void Every_Diamond_And_Pearl_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "DP" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Unown_Draw_Only_Works_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C("DP-68"));
            e.ScenarioBench(0, C("DP-68"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            var uses = e.GetMainActions(0).Count(o => o.Label.Contains("Use DRAW"));
            Assert.AreEqual(1, uses, "only the Benched Unown D");
        }

        [Test]
        public void Intense_Voltage_Moves_Lightning_Energy_Repeatedly()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Intense Voltage", "Use Intense Voltage"), new ScriptedAgent());
            var electivire = e.ScenarioActive(0, C("DP-48"));
            e.ScenarioEvolve(electivire, C("DP-81"));
            e.ScenarioEvolve(electivire, C("DP-3"));
            var donor = e.ScenarioBench(0, C(Chansey));
            Attach(e, donor, Lightning, 2);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, electivire.Energy.Count, "a bot moves 1 per use, and it can use the power again");
            Assert.AreEqual(0, donor.Energy.Count);
        }

        [Test]
        public void Magnetize_Frees_An_Active_Pokemon_With_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("DP-8"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(active));
            Attach(e, active, Metal, 1);
            Assert.AreEqual(0, e.RetreatCostOf(active));
        }

        [Test]
        public void Harden_Prevents_Small_Attack_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Harden"), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var cascoon = e.ScenarioActive(0, C("DP-44"));
            Attach(e, cascoon, Grass, 1);
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Step(e);
            Assert.AreEqual(0, cascoon.DamageCounters, "30 damage is prevented");
        }

        [Test]
        public void Professor_Rowan_Keeps_1_Card_And_Draws_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Rowan", "Chansey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("DP-112"));
            e.ScenarioAddToHand(0, C(Chansey));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(5, hand.Count);
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Chansey"));
        }

        [Test]
        public void Rival_Gives_You_3_Cards_The_Opponent_Picks()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rival"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("DP-113"));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(deck - 3, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Diamond_And_Pearl_Trainer_Searches_Skip_Supporters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Errand-Running"), new ScriptedAgent());
            e.ScenarioActive(0, C("DP-43"));
            e.ScenarioAddToDeck(0, C("DP-112"), 1);
            e.ScenarioAddToDeck(0, C("DP-108"), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Night Pokémon Center", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Diamond_And_Pearl_PlusPower_Adds_10_For_The_Chosen_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play PlusPower", "Attack: Rollout"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            Attach(e, chansey, Psychic, 1);
            e.ScenarioAddToHand(0, C("DP-109"));
            var target = e.ScenarioActive(1, C(MewEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, target.DamageCounters, "10 + 10");
        }

        [Test]
        public void Hard_Feelings_Adds_A_Counter_For_Each_Prize_Taken()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hard Feelings"), new ScriptedAgent());
            var dusknoir = e.ScenarioActive(0, C("DP-2"));
            Attach(e, dusknoir, Psychic, 3);
            var target = e.ScenarioActive(1, C(MewEx));
            e.State.Players[1].Prizes.RemoveRange(0, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5 + e.PrizesTaken(1), target.DamageCounters);
        }
    }
}
