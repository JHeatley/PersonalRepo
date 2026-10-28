using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v72: Unified Minds. The engine features it brought in and a few cards that are easy to get wrong.</summary>
    public class UnmTests : ScenarioTestBase
    {
        const string Psychic = "MEE-13";
        const string Lightning = "MEE-12";
        const string Darkness = "MEE-15";
        const string Pikachu = "UNM-56";
        const string Foongus = "UNM-13";
        const string Amoonguss = "UNM-14";
        const string Carracosta = "UNM-45";
        const string KeldeoGX = "UNM-47";
        const string MewtwoMewGX = "UNM-71";
        const string LatiosGX = "UNM-78";
        const string WeavileGX = "UNM-132";
        const string Sneasel = "UNM-131";
        const string Purrloin = "UNM-135";
        const string Archen = "UNM-120";
        const string Yanmega = "UNM-3";
        const string Eelektross = "UNM-66";
        const string UTurnBoard = "UNM-211";
        const string RecycleEnergy = "UNM-212";
        const string WeaknessGuardEnergy = "UNM-213";
        const string GiantBomb = "UNM-196";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ================================================================== Engine features

        [Test]
        public void Ancient_Custom_Turns_Off_The_Opponents_Tools_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Sneasel));
            e.ScenarioAttachTool(mine, C(UTurnBoard));
            var theirs = e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAttachTool(theirs, C(UTurnBoard));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(theirs), "U-Turn Board works");
            e.ScenarioBench(0, C(Carracosta));
            Assert.AreEqual(1, e.RetreatCostOf(theirs), "the opponent's Tool has no effect");
            Assert.AreEqual(0, e.RetreatCostOf(mine), "Carracosta's own side keeps its Tools");
        }

        [Test]
        public void U_Turn_Board_Goes_To_The_Hand_When_It_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cleaning Up"), new ScriptedAgent());
            var purrloin = e.ScenarioActive(0, C(Purrloin));
            e.ScenarioAttach(purrloin, C(Darkness));
            var theirs = e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAttachTool(theirs, C(UTurnBoard));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(theirs.Tool);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count(c => c.Def.Name == "U-Turn Board"), "into the opponent's hand");
            Assert.IsFalse(e.State.Players[1].Discard.Any(c => c.Def.Name == "U-Turn Board"));
        }

        [Test]
        public void Recycle_Energy_Goes_To_The_Hand_When_It_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Air Slash"), new ScriptedAgent());
            var yanmega = e.ScenarioActive(0, C(Yanmega));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(yanmega, C(RecycleEnergy));
            e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, yanmega.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Recycle Energy"));
        }

        [Test]
        public void Bursting_Spores_Reacts_To_A_Pokemon_With_Spore_Played_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Foongus", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioBench(0, C(Amoonguss));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioAddToHand(0, C(Foongus));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Asleep));
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
        }

        // ================================================================== Pokémon

        [Test]
        public void Pure_Heart_Stops_Damage_From_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Claw Slash"));
            var keldeo = e.ScenarioActive(0, C(KeldeoGX));
            var weavile = e.ScenarioActive(1, C(WeavileGX));
            e.ScenarioAttach(weavile, C(Darkness));
            e.ScenarioAttach(weavile, C(Darkness));
            e.ScenarioAttach(weavile, C(Darkness));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, keldeo.DamageCounters);
        }

        [Test]
        public void Clear_Vision_GX_Stops_The_Opponents_GX_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clear Vision-GX"), new ScriptedAgent());
            var latios = e.ScenarioActive(0, C(LatiosGX));
            e.ScenarioAttach(latios, C(Psychic));
            var weavile = e.ScenarioActive(1, C(WeavileGX));
            var keldeo = e.ScenarioBench(1, C(KeldeoGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Clear Vision-GX")), "Power Bind: 4 or fewer Pokémon in play");
            for (int i = 0; i < 4; i++) e.ScenarioBench(0, C(Sneasel));
            Step(e);
            Assert.IsTrue(e.State.Players[0].GxUsed);
            Assert.IsFalse(e.State.Players[1].GxUsed, "an effect on their Pokémon, not their GX attack used up (Hala)");
            Assert.IsTrue(e.AttackNamedPrevented(weavile, "Nocturnal Maneuvers-GX"), "the opponent can't use a GX attack for the rest of the game");
            Assert.IsFalse(e.AttackNamedPrevented(keldeo, "Resolute Blade-GX"), "Pure Heart: Clear Vision-GX can't affect Keldeo-GX");
            Assert.IsFalse(e.AttackNamedPrevented(weavile, "Claw Slash"));
        }

        [Test]
        public void Perfection_Uses_The_Attacks_Of_Benched_Pokemon_GX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoMewGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mewtwo, C(Darkness));
            e.ScenarioBench(0, C(WeavileGX));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Claw Slash")), "Weavile-GX's attack");
        }

        [Test]
        public void Electric_Swamp_Benches_Eelektross_From_The_Hand_With_4_Lightning_Energy_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Electric Swamp", "No"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(pikachu, C(Lightning));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C(Eelektross));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Electric Swamp")), "only 3 Lightning Energy");
            e.ScenarioAttach(pikachu, C(Lightning));
            Step(e);
            Assert.AreEqual("Eelektross", e.State.Players[0].Bench.Single().Name);
        }

        // ================================================================== Trainers and Energy

        [Test]
        public void Weakness_Guard_Energy_Removes_Weakness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sneasel = e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            Assert.IsNotNull(e.WeaknessOf(sneasel));
            e.ScenarioAttach(sneasel, C(WeaknessGuardEnergy));
            Assert.IsNull(e.WeaknessOf(sneasel));
        }

        [Test]
        public void Hapu_Takes_2_Of_The_Top_6_And_Discards_The_Rest()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hapu"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C("UNM-200"));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count);
            Assert.AreEqual(5, me.Discard.Count, "4 cards and Hapu");
            Assert.AreEqual(deck - 6, me.Deck.Count);
        }

        [Test]
        public void Reset_Stamp_Leaves_The_Opponent_A_Card_For_Each_Prize_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Reset Stamp"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            for (int i = 0; i < 9; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(0, C("UNM-206"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Pokemon_Research_Lab_Benches_Fossil_Pokemon_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Pokémon Research Lab", "Archen|Archen"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToDeck(0, C(Archen), 2);
            e.ScenarioSetStadium(0, C("UNM-205"));
            e.ScenarioSetTurn(0, 3);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "the turn ends");
            Assert.AreEqual(2, e.State.Players[0].Bench.Count(m => m.Name == "Archen"));
        }

        [Test]
        public void Blues_Tactics_Draws_Up_To_8_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Blue's Tactics"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C("UNM-188"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count, "not yet");
            e.EndTurn();
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Dark_City_Frees_The_Retreat_Of_Basic_Darkness_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sneasel = e.ScenarioActive(0, C(Sneasel));
            var keldeo = e.ScenarioActive(1, C(KeldeoGX));
            Assert.AreEqual(1, e.RetreatCostOf(sneasel));
            e.ScenarioSetStadium(0, C("UNM-193"));
            Assert.AreEqual(0, e.RetreatCostOf(sneasel));
            Assert.AreEqual(2, e.RetreatCostOf(keldeo), "not a Darkness Pokémon");
        }

        [Test]
        public void Giant_Bomb_Is_Discarded_At_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sneasel = e.ScenarioActive(0, C(Sneasel));
            e.ScenarioAttachTool(sneasel, C(GiantBomb));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioSetTurn(0, 3);
            NextTurn(e);
            Assert.IsNotNull(sneasel.Tool, "still there during the opponent's turn");
            e.EndTurn();
            Assert.IsNull(sneasel.Tool);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Giant Bomb"));
        }

        [Test]
        public void Channeler_Removes_Item_Lock()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Channeler"), new ScriptedAgent());
            e.ScenarioActive(0, C(Sneasel));
            e.ScenarioActive(1, C(Sneasel));
            e.ScenarioAddToHand(0, C("UNM-190"));
            e.ScenarioAddToHand(0, C("UNM-206"));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].ItemLockTurn = 3;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Reset Stamp")));
            Step(e);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Reset Stamp")));
        }
    }
}
