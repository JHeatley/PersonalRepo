using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v103: Furious Fists (FFI 1-113): Restored Pokémon (Amaura, Tyrunt), Shining Spirit, King's Song, Quaking Punch, Hand Control,
    /// Sinister Suggestion, Secret Key, Bust In, Focus Sash, Training Center, Mountain Ring, Fossil Researcher and the rest.
    /// </summary>
    public class FfiTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string MarshadowMachampGX = "UNB-82";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string MalamarEX = "PHF-58";

        const string Poliwag = "FFI-15";
        const string Poliwhirl = "FFI-16";
        const string Poliwrath = "FFI-17";
        const string Politoed = "FFI-18";
        const string SeismitoadEX = "FFI-20";
        const string Amaura = "FFI-25";
        const string Drowzee = "FFI-35";
        const string Hypno = "FFI-36";
        const string LucarioEX = "FFI-54";
        const string Tyrunt = "FFI-61";
        const string Hawlucha = "FFI-63";
        const string Clefairy = "FFI-70";
        const string Klefki = "FFI-73";
        const string DragoniteEX = "FFI-74";
        const string FocusSash = "FFI-91";
        const string FossilResearcher = "FFI-92";
        const string MountainRing = "FFI-97";
        const string TrainingCenter = "FFI-102";
        const string Tierno = "BKP-112";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "FFI" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(90, own.Count);
            Assert.AreEqual(Stage.Restored, C(Amaura).Stage);
            Assert.AreEqual(Stage.Restored, C(Tyrunt).Stage);
            Assert.AreEqual("FFI-4", C("FFI-105").BaseKey, "full art Heracross-EX");
            Assert.AreEqual("GEN-42", C("FFI-46").BaseKey, "Machamp is the Generations card");
        }

        [Test]
        public void Shining_Spirit_Ignores_Weakness_And_Flying_Press_Needs_A_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flying Press"), new ScriptedAgent());
            var hawlucha = e.ScenarioActive(0, C(Hawlucha));
            e.ScenarioAttach(hawlucha, C(Fighting));
            var malamar = e.ScenarioActive(1, C(MalamarEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, malamar.DamageCounters, "60, not doubled by its Fighting Weakness");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Flying Press"), new ScriptedAgent());
            var other = e2.ScenarioActive(0, C(Hawlucha));
            e2.ScenarioAttach(other, C(Fighting));
            var snorlax = e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, snorlax.DamageCounters, "not a Pokémon-EX");
        }

        [Test]
        public void Kings_Song_Drops_The_Colorless_Cost_Of_The_Poliwag_Line()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var poliwrath = e.ScenarioActive(0, C(Poliwag));
            e.ScenarioEvolve(poliwrath, C(Poliwhirl));
            e.ScenarioEvolve(poliwrath, C(Poliwrath));
            e.ScenarioAttach(poliwrath, C(Water));
            e.ScenarioAttach(poliwrath, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.UsableAttacks(poliwrath).Any(a => a.Attack.Name == "Submission"));
            var politoed = e.ScenarioBench(0, C(Poliwag));
            e.ScenarioEvolve(politoed, C(Poliwhirl));
            e.ScenarioEvolve(politoed, C(Politoed));
            Assert.IsTrue(e.UsableAttacks(poliwrath).Any(a => a.Attack.Name == "Submission"), "[W][W][C][C] - [C][C]");
            Assert.IsTrue(e.UsableAttacks(poliwrath).Any(a => a.Attack.Name == "Steamroll"), "[C][C][C]: nothing to pay");
        }

        [Test]
        public void Quaking_Punch_Stops_Items_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Quaking Punch"), new ScriptedAgent());
            var toad = e.ScenarioActive(0, C(SeismitoadEX));
            e.ScenarioAttach(toad, C(Water));
            e.ScenarioAttach(toad, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].ItemLockTurn);
        }

        [Test]
        public void Hand_Control_Uses_The_Opponents_Supporter_On_Them()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hand Control"), new ScriptedAgent());
            var hypno = e.ScenarioActive(0, C(Drowzee));
            e.ScenarioEvolve(hypno, C(Hypno));
            e.ScenarioAttach(hypno, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(1, C(Tierno));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count, "the opponent drew 3");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Tierno"));
        }

        [Test]
        public void Sinister_Suggestion_Makes_Every_Opponent_Flip_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sinister Suggestion"), new ScriptedAgent());
            var drowzee = e.ScenarioActive(0, C(Drowzee));
            e.ScenarioAttach(drowzee, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            for (int i = 0; i < 8; i++) Assert.IsFalse(e.FlipCoin(1, "test"));
        }

        [Test]
        public void Secret_Key_Makes_Fairy_Resistance_40()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var clefairy = e.ScenarioActive(0, C(Clefairy));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(20, e.ResistanceAmountOf(clefairy));
            e.ScenarioBench(0, C(Klefki));
            Assert.AreEqual(40, e.ResistanceAmountOf(clefairy));
        }

        [Test]
        public void Bust_In_Moves_Energy_And_Switches_Dragonite_EX_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Dragonite-EX", "Yes"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttach(lapras, C(Water));
            e.ScenarioAddToHand(0, C(DragoniteEX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var active = e.State.Players[0].Active;
            Assert.AreEqual("Dragonite-EX", active.Name);
            Assert.AreEqual(1, active.Energy.Count);
            Assert.AreEqual(0, lapras.Energy.Count);
        }

        [Test]
        public void Focus_Sash_Saves_A_Fighting_Pokemon_At_Full_HP_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var lucario = e.ScenarioActive(1, C(LucarioEX));
            e.ScenarioAttachTool(lucario, C(FocusSash));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, lucario, 300);
            Assert.AreEqual(17, lucario.DamageCounters, "10 HP left");
            Assert.AreEqual(0, lucario.Tools.Count, "discarded");
        }

        [Test]
        public void Focus_Sash_And_Acme_Of_Heroism_GX_Let_The_Owner_Choose_Which_Saves_It()
        {
            var owner = new ScriptedAgent().Then("Attack: Acme of Heroism-GX");
            var e = Scenario(new ScriptedAgent(), owner);
            e.ScenarioActive(0, C(SnorlaxGX));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            var mm = e.ScenarioActive(1, C(MarshadowMachampGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(mm, C(Fighting));
            e.ScenarioAttachTool(mm, C(FocusSash));
            e.ScenarioSetTurn(1, 2);
            Step(e);
            Assert.AreEqual(0, mm.DamageCounters);
            owner.Then("Acme of Heroism-GX");
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(lapras, mm, 400);
            Assert.AreEqual(26, mm.DamageCounters, "10 HP left");
            Assert.AreEqual(1, mm.Tools.Count, "Acme of Heroism-GX applied, so Focus Sash stays");
        }

        [Test]
        public void Training_Center_And_Mountain_Ring()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var poliwhirl = e.ScenarioActive(1, C(Poliwag));
            e.ScenarioEvolve(poliwhirl, C(Poliwhirl));
            var benched = e.ScenarioBench(1, C(Poliwag));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioSetStadium(1, C(TrainingCenter));
            Assert.AreEqual(110, e.MaxHp(poliwhirl));
            Assert.AreEqual(60, e.MaxHp(benched), "Basic Pokémon don't get it");
            e.ScenarioSetStadium(1, C(MountainRing));
            e.DealAttackDamage(snorlax, benched, 30);
            Assert.AreEqual(0, benched.DamageCounters);
        }

        [Test]
        public void Fossil_Researcher_Benches_Amaura_And_Tyrunt()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Fossil Researcher", "Amaura|Tyrunt"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(FossilResearcher));
            e.ScenarioAddToDeck(0, C(Amaura), 1);
            e.ScenarioAddToDeck(0, C(Tyrunt), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var names = e.State.Players[0].Bench.Select(m => m.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "Amaura", "Tyrunt" }, names);
        }
    }
}
