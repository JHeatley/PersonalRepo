using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v101: Primal Clash (PRC 1-164): the Ancient Traits Ω Barrier, Ω Barrage, α Growth and α Recovery, Primal Reversion's Spirit
    /// Links, Allergic Shock, Rock Wall, Barrier Shrine, Archie's Ace in the Hole, Silent Lab, Shrine of Memories and the rest.
    /// </summary>
    public class PrcTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Lysandre = "AOR-78";
        const string HexManiac = "AOR-75";

        const string Weedle = "PRC-1";
        const string Kakuna = "PRC-2";
        const string Beedrill = "PRC-3";
        const string Vulpix = "PRC-20";
        const string Ninetales = "PRC-21";
        const string Horsea = "PRC-30";
        const string Seadra = "PRC-31";
        const string Mudkip = "PRC-33";
        const string Marshtomp = "PRC-34";
        const string Swampert = "PRC-36";
        const string WailordEX = "PRC-38";
        const string KyogreEX = "PRC-54";
        const string PrimalKyogreEX = "PRC-55";
        const string OmegaElectrike = "PRC-60";
        const string Tentacool = "PRC-71";
        const string Rhyhorn = "PRC-74";
        const string Rhydon = "PRC-75";
        const string RhyperiorWall = "PRC-77";
        const string Meditite = "PRC-79";
        const string MedichamBarrage = "PRC-81";
        const string GroudonEX = "PRC-85";
        const string PrimalGroudonEX = "PRC-86";
        const string Aegislash = "PRC-100";
        const string Honedge = "PRC-98";
        const string Doublade = "PRC-99";
        const string ArchiesAce = "PRC-124";
        const string GroudonSpiritLink = "PRC-131";
        const string ShrineOfMemories = "PRC-139";
        const string SilentLab = "PRC-140";
        const string WonderEnergy = "PRC-144";
        const string MarillFairy = "PRC-102";

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "PRC" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(131, own.Count);
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Swampert)));
            Assert.AreEqual("PRC-19", C("PRC-145").BaseKey, "full art Trevenant-EX");
            Assert.AreEqual("GEN-8", C("PRC-4").BaseKey, "Tangela is the Generations card");
            Assert.AreEqual("Ω Barrier", C(PrimalGroudonEX).AncientTrait);
            Assert.AreEqual("Ω Barrage", C(MedichamBarrage).AncientTrait);
            Assert.AreEqual("α Growth", C(PrimalKyogreEX).AncientTrait);
            Assert.AreEqual("α Recovery", C(Tentacool).AncientTrait);
            Assert.IsTrue(C(PrimalKyogreEX).IsXYMegaEvolution, "Primal Pokémon are Mega Evolution Pokémon");
        }

        // ------------------------------------------------------------------ Ancient Traits

        [Test]
        public void Omega_Barrier_Stops_The_Opponents_Lysandre_Only_On_That_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lysandre"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(Lysandre));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioBench(1, C(OmegaElectrike));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(lapras, e.State.Players[1].Active, "the Ω Barrier Pokémon can't be dragged up");

            var e2 = Scenario(new ScriptedAgent().Then("Play Lysandre"), new ScriptedAgent());
            e2.ScenarioActive(0, C(SnorlaxGX));
            e2.ScenarioAddToHand(0, C(Lysandre));
            e2.ScenarioActive(1, C(OmegaElectrike));
            var benched = e2.ScenarioBench(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(benched, e2.State.Players[1].Active, "an Ω Barrier Active Pokémon doesn't stop the switch (2015)");
        }

        [Test]
        public void Omega_Barrage_Attacks_Twice()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Yoga Kick", "Attack again: Yoga Kick"), new ScriptedAgent());
            var medicham = e.ScenarioActive(0, C(Meditite));
            e.ScenarioEvolve(medicham, C(MedichamBarrage));
            e.ScenarioAttach(medicham, C(Fighting));
            e.ScenarioAttach(medicham, C(Fighting));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, snorlax.DamageCounters, "30 twice, no Weakness");
        }

        [Test]
        public void Alpha_Growth_Attaches_A_Second_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy", "Attach Water Energy"), new ScriptedAgent());
            var swampert = e.ScenarioActive(0, C(Mudkip));
            e.ScenarioEvolve(swampert, C(Marshtomp));
            e.ScenarioEvolve(swampert, C(Swampert));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, swampert.Energy.Count, "one more, not two more (2015-02-05)");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Alpha_Recovery_Doubles_Healing()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tentacool = e.ScenarioActive(0, C(Tentacool));
            tentacool.DamageCounters = 5;
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(40, e.Heal(tentacool, 20));
            Assert.AreEqual(1, tentacool.DamageCounters);
        }

        [Test]
        public void Groudon_Spirit_Link_Works_For_Primal_Groudon_EX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var groudon = e.ScenarioActive(0, C(GroudonEX));
            e.ScenarioAttachTool(groudon, C(GroudonSpiritLink));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNotNull(e.SpiritLinkFor(groudon, C(PrimalGroudonEX)));
        }

        // ------------------------------------------------------------------ Pokémon

        [Test]
        public void Allergic_Shock_Knocks_Out_The_Defending_Pokemon_If_It_Is_Damaged_During_Your_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Allergic Shock"), new ScriptedAgent());
            var beedrill = e.ScenarioActive(0, C(Weedle));
            e.ScenarioEvolve(beedrill, C(Kakuna));
            e.ScenarioEvolve(beedrill, C(Beedrill));
            e.ScenarioAttach(beedrill, C(Grass));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            e.DealAttackDamage(beedrill, snorlax, 10);
            Assert.IsTrue(snorlax.ForcedKnockOut);
        }

        [Test]
        public void Rock_Wall_Takes_10_Off_Attack_Damage_To_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var rhyperior = e.ScenarioActive(1, C(Rhyhorn));
            e.ScenarioEvolve(rhyperior, C(Rhydon));
            e.ScenarioEvolve(rhyperior, C(RhyperiorWall));
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, lapras, 50);
            Assert.AreEqual(4, lapras.DamageCounters);
        }

        [Test]
        public void Barrier_Shrine_Stops_Stadiums_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var ninetales = e.ScenarioActive(1, C(Vulpix));
            e.ScenarioEvolve(ninetales, C(Ninetales));
            var stadium = e.ScenarioAddToHand(0, C(SilentLab));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, stadium));
        }

        [Test]
        public void Silent_Lab_Turns_Off_Basic_Pokemons_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var wailord = e.ScenarioActive(0, C(WailordEX));
            var aegislash = e.ScenarioBench(0, C(Honedge));
            e.ScenarioEvolve(aegislash, C(Doublade));
            e.ScenarioEvolve(aegislash, C(Aegislash));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetStadium(1, C(SilentLab));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.AbilityBlocked(wailord, "Water Veil"));
            Assert.IsFalse(e.AbilityBlocked(aegislash, "Miracle Guard"), "a Stage 2");
            Assert.IsNull(e.WeaknessOf(wailord), "Miracle Guard still works");
        }

        [Test]
        public void Shrine_Of_Memories_Gives_The_Previous_Evolutions_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var seadra = e.ScenarioActive(0, C(Horsea));
            e.ScenarioEvolve(seadra, C(Seadra));
            e.ScenarioAttach(seadra, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.UsableAttacks(seadra).Any(a => a.Attack.Name == "Bubble"));
            e.ScenarioSetStadium(1, C(ShrineOfMemories));
            Assert.IsTrue(e.UsableAttacks(seadra).Any(a => a.Attack.Name == "Bubble"));
        }

        // ------------------------------------------------------------------ Trainers and Energy

        [Test]
        public void Archies_Ace_In_The_Hole_Needs_To_Be_The_Last_Card_And_Benches_A_Stage_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Archie's Ace in the Hole"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            var archie = e.ScenarioAddToHand(0, C(ArchiesAce));
            var extra = e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToDiscard(0, C(Swampert), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, archie), "another card in hand");
            e.State.Players[0].Hand.Remove(extra);
            Step(e);
            Assert.AreEqual("Swampert", e.State.Players[0].Bench[0].Name);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Wonder_Energy_Stops_Attack_Effects_On_Its_Fairy_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var marill = e.ScenarioActive(1, C(MarillFairy));
            e.ScenarioAttach(marill, C(WonderEnergy));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.EnergyCountOfType(marill, EnergyType.Fairy));
            Assert.IsTrue(e.EffectsPrevented(marill, EffectSourceKind.Attack, 0, false));
        }
    }
}
