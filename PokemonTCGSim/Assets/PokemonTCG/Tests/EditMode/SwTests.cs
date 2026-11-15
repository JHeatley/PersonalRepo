using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v172: Secret Wonders (SW): the card data and prints, and the new effects (Jamming, Waterlog, Burning Coat, Sonic Blade,
    /// Telepass, Psychic Lock, Energy Shift, Plant Cloak, Camouflage, Dark Engage, Roseanne's Research, Team Galactic's Mars).
    /// </summary>
    public class SwTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
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
        public void Secret_Wonders_Is_In_The_Pool_With_Its_Prints()
        {
            Assert.AreEqual(123, db.Cards.Count(d => d.SetCode == "SW"));
            for (int n = 1; n <= 132; n++) Assert.IsNotNull(db.Get("SW", n.ToString()), "SW " + n);
            Assert.AreEqual("MT-109", db.MainPrinting(C("SW-119")).Key, "Bebe's Search");
            Assert.AreEqual("DP-109", db.MainPrinting(C("SW-121")).Key, "PlusPower");
            Assert.AreEqual("SW-83", db.MainPrinting(C("SW-83")).Key, "this Clefairy is its own card");
        }

        [Test]
        public void Every_Secret_Wonders_Card_With_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => d.SetCode == "SW" && (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer || d.IsSpecialEnergy)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Key).ToList();
            CollectionAssert.IsEmpty(missing);
        }

        [Test]
        public void Jamming_Hurts_Every_Pokemon_Of_A_Player_Who_Plays_A_Supporter()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Play Professor Oak's Visit"));
            e.ScenarioActive(0, C("SW-1"));
            var active = e.ScenarioActive(1, C(Chansey));
            var bench = e.ScenarioBench(1, C(Chansey));
            e.ScenarioAddToHand(1, C("SW-122"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(1, active.DamageCounters);
            Assert.AreEqual(1, bench.DamageCounters);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count, "Professor Oak's Visit: 3 drawn, 1 to the bottom");
        }

        [Test]
        public void Waterlog_Attaches_Water_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Waterlog"), new ScriptedAgent());
            var blastoise = e.ScenarioActive(0, C("SW-2"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, blastoise.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Burning_Coat_Mills_3_When_Entei_Knocks_Out_A_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blaze Roar"), new ScriptedAgent());
            var entei = e.ScenarioActive(0, C("SW-4"));
            Attach(e, entei, Fire, 3);
            e.ScenarioActive(1, C("MT-99"));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Discard.Count(c => c.Def.IsEnergy), "the top 3 cards (Psychic Energy) were discarded");
        }

        [Test]
        public void Sonic_Blade_Leaves_50_Hp_And_Forces_A_Switch()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sonic Blade"), new ScriptedAgent());
            var gallade = e.ScenarioActive(0, C("SW-6"));
            Attach(e, gallade, "MEE-14", 1);
            Attach(e, gallade, Psychic, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(50, e.RemainingHp(chansey));
            Assert.IsTrue(e.State.Players[1].Bench.Contains(chansey));
        }

        [Test]
        public void Telepass_Uses_A_Supporter_From_The_Opponents_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Telepass"), new ScriptedAgent());
            e.ScenarioActive(0, C("SW-7"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDiscard(1, C("SW-122"), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "3 drawn, 1 to the bottom");
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Professor Oak's Visit"), "it stays in the opponent's discard pile");
        }

        [Test]
        public void Psychic_Lock_Stops_The_Opponents_Poke_Powers_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psychic Lock"), new ScriptedAgent());
            var gardevoir = e.ScenarioActive(0, C("SW-7"));
            Attach(e, gardevoir, Psychic, 3);
            var bronzong = e.ScenarioActive(1, C("MT-74"));
            e.ScenarioEvolve(bronzong, C("MT-6"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Assert.AreEqual(1, e.State.CurrentPlayer);
            Assert.IsFalse(Offered(e, 1, "Miracle Oracle"));
        }

        [Test]
        public void Energy_Shift_Turns_A_Knocked_Out_Electrode_Into_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Super Psy Bolt"));
            var electrode = e.ScenarioActive(0, C("SW-117"));
            e.ScenarioEvolve(electrode, C("SW-26"));
            electrode.DamageCounters = 7;
            var chansey = e.ScenarioBench(0, C(Chansey));
            var mew = e.ScenarioActive(1, C(MewEx));
            Attach(e, mew, Psychic, 2);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsTrue(chansey.Energy.Any(c => c.Def.Name == "Electrode"), "Electrode is attached as an Energy card");
            Assert.AreEqual(1, e.EnergyCountOfType(chansey, EnergyType.Fire), "it provides every type");
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "it still counts as Knocked Out");
        }

        [Test]
        public void Plant_Cloak_Gives_40_Hp_With_2_Grass_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wormadam = e.ScenarioActive(0, C("SW-41"));
            e.ScenarioActive(1, C(Chansey));
            Attach(e, wormadam, Grass, 1);
            Assert.AreEqual(80, e.MaxHp(wormadam));
            Attach(e, wormadam, Grass, 1);
            Assert.AreEqual(120, e.MaxHp(wormadam));
        }

        [Test]
        public void Camouflage_Cuts_Damage_From_A_Matching_Type()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electromagnetic Jam"));
            var kecleon = e.ScenarioActive(0, C("SW-52"));
            Attach(e, kecleon, Lightning, 1);
            var electrike = e.ScenarioActive(1, C("MT-81"));
            Attach(e, electrike, Lightning, 1);
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, kecleon.DamageCounters);
        }

        [Test]
        public void Dark_Engage_Makes_The_Active_Pokemon_Darkness()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Dark Engage"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C("SW-40"));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEqual(new[] { EnergyType.Darkness }, e.TypesOf(chansey));
        }

        [Test]
        public void Roseannes_Research_Finds_Basic_Pokemon_And_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Roseanne's Research", "Chansey|Grass"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToDeck(0, C(Chansey), 1);
            e.ScenarioAddToDeck(0, C(Grass), 1);
            e.ScenarioAddToHand(0, C("SW-125"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Chansey"));
            Assert.AreEqual(1, hand.Count(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Team_Galactics_Mars_Draws_2_And_Bottoms_An_Opponents_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Galactic's Mars"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C("SW-126"));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual(0, e.State.Players[1].Hand.Count);
            Assert.AreEqual("Grass Energy", e.State.Players[1].Deck.Last().Def.Name);
        }
    }
}
