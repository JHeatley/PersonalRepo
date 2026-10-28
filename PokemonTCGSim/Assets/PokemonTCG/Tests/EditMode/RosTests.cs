using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v99: Roaring Skies (ROS 1-110): the Ancient Traits Δ Plus, Δ Evolution and Δ Wild, Tool Concealment, Set Up, Doom Desire,
    /// Close Encounter, Sky Field, Wide Lens, Double Dragon Energy, VS Seeker and the rest.
    /// </summary>
    public class RosTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Metal = "MEE-16";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string FloatStone = "BKT-137";
        const string FairyGarden = "FCO-100";
        const string Flareon = "AOR-13";
        const string Steven = "AOR-95";
        const string RayquazaEX = "30C-CC19";

        const string Wurmple = "ROS-3";
        const string Dustox = "ROS-8";
        const string Nincada = "ROS-9";
        const string Ninjask = "ROS-10";
        const string Shuppet = "ROS-30";
        const string BanetteConceal = "ROS-31";
        const string BanetteJammer = "ROS-32";
        const string Deoxys = "ROS-33";
        const string Absol = "ROS-40";
        const string Jirachi = "ROS-42";
        const string Carbink = "ROS-47";
        const string MRayquazaEX = "ROS-61";
        const string HydreigonEX = "ROS-62";
        const string Swablu = "ROS-73";
        const string Altaria = "ROS-74";
        const string ShayminEX = "ROS-77";
        const string HealingScarf = "ROS-84";
        const string MegaTurbo = "ROS-86";
        const string SkyField = "ROS-89";
        const string Winona = "ROS-96";
        const string DoubleDragonEnergy = "ROS-97";
        const string VSSeeker = "ROS-110";

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "ROS" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(88, own.Count);
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C(Dustox)), "no regulation mark: Expanded");
            Assert.AreEqual("ROS-26", C("ROS-98").BaseKey, "full art Thundurus-EX");
            Assert.AreEqual("ROS-77", C("ROS-77a").BaseKey);
            Assert.AreEqual("GEN-25", C("ROS-16").BaseKey, "Articuno is the Generations card");
            Assert.AreEqual("CEL-CC24", C("ROS-76").BaseKey, "M Rayquaza-EX is the Celebrations Classic Collection card");
            Assert.AreEqual("Δ Plus", C(Dustox).AncientTrait);
            Assert.AreEqual("Δ Evolution", C(BanetteJammer).AncientTrait);
            Assert.AreEqual("Δ Wild", C(MRayquazaEX).AncientTrait);
            Assert.IsTrue(C(MRayquazaEX).IsXYMegaEvolution);
        }

        // ------------------------------------------------------------------ Ancient Traits

        [Test]
        public void Delta_Evolution_Evolves_The_Turn_The_Pokemon_Was_Played()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            var shuppet = e.ScenarioBench(0, C(Shuppet));
            shuppet.TurnEnteredPlay = e.State.TurnNumber;
            var swablu = e.ScenarioActive(0, C(Swablu));
            swablu.TurnEnteredPlay = e.State.TurnNumber;
            Assert.IsTrue(e.CanEvolveInto(shuppet, C(BanetteJammer), true), "Δ Evolution");
            Assert.IsFalse(e.CanEvolveInto(shuppet, C(BanetteConceal), true), "no Δ Evolution");
            Assert.IsTrue(e.CanEvolveInto(swablu, C(Altaria), true));
        }

        [Test]
        public void Delta_Wild_Takes_20_Less_From_Grass_Fire_Water_And_Lightning_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var flareon = e.ScenarioActive(0, C(Flareon));
            var deoxys = e.ScenarioBench(0, C(Deoxys));
            var mega = e.ScenarioActive(1, C(RayquazaEX));
            e.ScenarioEvolve(mega, C(MRayquazaEX));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(flareon, mega, 100);
            Assert.AreEqual(8, mega.DamageCounters, "Fire: 100 - 20");
            e.DealAttackDamage(deoxys, mega, 100);
            Assert.AreEqual(18, mega.DamageCounters, "Psychic: no change");
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Tool_Concealment_Turns_Off_Every_Tool_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C(FloatStone));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAttachTool(snorlax, C(FloatStone));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(lapras), "Float Stone works");
            var banette = e.ScenarioBench(1, C(Shuppet));
            e.ScenarioEvolve(banette, C(BanetteConceal));
            Assert.IsFalse(e.ToolWorks(lapras, lapras.Tools[0]), "the opponent's Tools");
            Assert.IsFalse(e.ToolWorks(snorlax, snorlax.Tools[0]), "its own side's Tools too");
            Assert.AreEqual(C(LaprasGX).RetreatCost, e.RetreatCostOf(lapras));
        }

        [Test]
        public void Set_Up_Draws_Until_6_When_Shaymin_EX_Is_Benched()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Shaymin-EX", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(ShayminEX));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Cursed_Eyes_Moves_3_Damage_Counters_Between_The_Opponents_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Absol", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Absol));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            snorlax.DamageCounters = 5;
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, snorlax.DamageCounters);
            Assert.AreEqual(3, lapras.DamageCounters);
        }

        [Test]
        public void Dragon_Road_And_Clear_Humming()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hydreigon = e.ScenarioActive(0, C(HydreigonEX));
            var altaria = e.ScenarioBench(0, C(Swablu));
            e.ScenarioEvolve(altaria, C(Altaria));
            var shaymin = e.ScenarioBench(0, C(ShayminEX));
            var theirs = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.RetreatCostOf(hydreigon), "no Stadium in play");
            e.ScenarioSetStadium(1, C(FairyGarden));
            Assert.AreEqual(1, e.RetreatCostOf(hydreigon), "any Stadium, the opponent's too");
            Assert.IsNull(e.WeaknessOf(shaymin), "your Colorless Pokémon");
            Assert.IsNull(e.WeaknessOf(altaria));
            Assert.IsNotNull(e.WeaknessOf(theirs), "not the opponent's");
        }

        [Test]
        public void Jewel_Armor_Prevents_Attack_Damage_Only_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var benched = e.ScenarioBench(1, C(Carbink));
            var active = e.ScenarioActive(1, C(Carbink));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(snorlax, benched, 30);
            Assert.AreEqual(0, benched.DamageCounters);
            e.DealAttackDamage(snorlax, active, 30);
            Assert.AreEqual(3, active.DamageCounters);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Close_Encounter_Can_Be_Used_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Close Encounter"), new ScriptedAgent());
            var deoxys = e.ScenarioActive(0, C(Deoxys));
            e.ScenarioAttach(deoxys, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 1);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "2 cards drawn (2015-05-07)");
        }

        [Test]
        public void Doom_Desire_Knocks_Out_The_Defending_Pokemon_At_The_End_Of_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Doom Desire"), new ScriptedAgent());
            var jirachi = e.ScenarioActive(0, C(Jirachi));
            e.ScenarioAttach(jirachi, C(Metal));
            e.ScenarioAttach(jirachi, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, jirachi.Energy.Count, "all Energy discarded");
            e.EndTurn();
            e.StartTurn();
            Assert.IsNotNull(e.State.FindPokemon(snorlax.Uid), "not yet");
            e.EndTurn();
            Assert.IsNull(e.State.FindPokemon(snorlax.Uid));
        }

        [Test]
        public void Evolution_Jammer_Stops_Evolving_From_The_Hand_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Evolution Jammer"), new ScriptedAgent());
            var banette = e.ScenarioActive(0, C(Shuppet));
            e.ScenarioEvolve(banette, C(BanetteJammer));
            e.ScenarioAttach(banette, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].EvolveLockTurn);
        }

        [Test]
        public void Afterimage_Assault_Benches_Ninjask_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Afterimage Assault", "Ninjask|Ninjask"), new ScriptedAgent());
            var ninjask = e.ScenarioActive(0, C(Nincada));
            e.ScenarioEvolve(ninjask, C(Ninjask));
            e.ScenarioAttach(ninjask, C(Grass));
            e.ScenarioAttach(ninjask, C(Psychic));
            e.ScenarioAddToDeck(0, C(Ninjask), 2);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count(m => m.Name == "Ninjask"), "a Stage 1 straight onto the Bench");
        }

        // ------------------------------------------------------------------ Trainers and Energy

        [Test]
        public void Sky_Field_Allows_8_Benched_And_Cuts_Back_To_5_When_It_Leaves()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetStadium(0, C(SkyField));
            for (int i = 0; i < 7; i++) e.ScenarioBench(0, C(Wurmple));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(8, e.MaxBench(0));
            Assert.AreEqual(8, e.MaxBench(1));
            e.PutStadiumIntoPlay(1, e.ScenarioAddToHand(1, C(FairyGarden)));
            Assert.AreEqual(5, e.State.Players[0].Bench.Count, "Sky Field left play");
        }

        [Test]
        public void Healing_Scarf_Heals_20_When_Energy_Is_Attached_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C(HealingScarf));
            lapras.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, lapras.DamageCounters);
        }

        [Test]
        public void Mega_Turbo_Attaches_To_A_Mega_Evolution_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Mega Turbo"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(MegaTurbo));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, e.State.Players[0].Hand[0]), "no Mega Evolution Pokémon");
            var mega = e.ScenarioBench(0, C(RayquazaEX));
            e.ScenarioEvolve(mega, C(MRayquazaEX));
            Step(e);
            Assert.AreEqual(1, mega.Energy.Count);
            Assert.AreEqual(0, lapras.Energy.Count);
        }

        [Test]
        public void Double_Dragon_Energy_Provides_2_Of_Any_Type_On_A_Dragon_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hydreigon = e.ScenarioActive(0, C(HydreigonEX));
            e.ScenarioAttach(hydreigon, C(DoubleDragonEnergy));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.EnergyCount(hydreigon));
            var script = StandardCards.CreateRegistry().For(C(DoubleDragonEnergy));
            Assert.IsFalse(script.CanAttachTo(new EffectContext(e, 0, hydreigon.Energy[0], hydreigon), lapras));
        }

        [Test]
        public void VS_Seeker_And_Winona()
        {
            var e = Scenario(new ScriptedAgent().Then("Play VS Seeker"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(VSSeeker));
            e.ScenarioAddToDiscard(0, C(Steven), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Steven"));

            var e2 = Scenario(new ScriptedAgent().Then("Play Winona", "Shaymin-EX|Altaria"), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            e2.ScenarioAddToHand(0, C(Winona));
            e2.ScenarioAddToDeck(0, C(ShayminEX), 1);
            e2.ScenarioAddToDeck(0, C(Altaria), 1);
            e2.ScenarioAddToDeck(0, C(Jirachi), 1);
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            var hand = e2.State.Players[0].Hand;
            Assert.AreEqual(2, hand.Count(c => c.Def.IsPokemon));
            Assert.IsFalse(hand.Any(c => c.Def.Name == "Jirachi"), "Colorless Pokémon only");
        }
    }
}
