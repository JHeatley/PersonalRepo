using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v93: Steam Siege (STS 1-116): dual-type Pokémon, Restored Pokémon and their Fossils, Wicked Wind, Pokémon Ranger, Ninja Boy,
    /// Greedy Dice, Klefki's Wonder Lock, Volcanion-EX's Steam Up and the rest.
    /// </summary>
    public class StsTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Fairy = "SUM-Y";
        const string Grass = "MEE-9";
        const string DoubleColorless = "SLG-69";
        const string ChoiceBand = "BUS-162";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string Steenee = "SUM-19";
        const string MCamerupt = "MEG-22";
        const string Numel = "MEG-21";
        const string PidgeotEX = "EVO-64";
        const string MPidgeotEX = "EVO-65";
        const string Gastly = "EVO-47";
        const string Haunter = "EVO-48";

        const string Yanma = "STS-6";
        const string Yanmega = "STS-7";
        const string YanmegaBreak = "STS-8";
        const string Seedot = "STS-9";
        const string Nuzleaf = "STS-10";
        const string Shiftry = "STS-11";
        const string Volcanion = "STS-25";
        const string VolcanionEX = "STS-26";
        const string Oshawott = "STS-30";
        const string Dewott = "STS-31";
        const string Samurott = "STS-32";
        const string Clauncher = "STS-33";
        const string Clawitzer = "STS-34";
        const string ClawitzerBreak = "STS-35";
        const string Bergmite = "STS-36";
        const string Avalugg = "STS-37";
        const string Mareep = "STS-38";
        const string Flaaffy = "STS-39";
        const string Ampharos = "STS-40";
        const string Joltik = "STS-41";
        const string Galvantula = "STS-42";
        const string Anorith = "STS-56";
        const string Croagunk = "STS-58";
        const string Toxicroak = "STS-59";
        const string Sneasel = "STS-60";
        const string Weavile = "STS-61";
        const string Pawniard = "STS-63";
        const string Bisharp = "STS-64";
        const string Shieldon = "STS-69";
        const string Bastiodon = "STS-70";
        const string Klink = "STS-71";
        const string Klang = "STS-72";
        const string Klinklang = "STS-73";
        const string MagearnaEX = "STS-75";
        const string GardevoirEX = "STS-78";
        const string MGardevoirEX = "STS-79";
        const string Klefki = "STS-80";
        const string Meowth = "STS-88";
        const string Talonflame = "STS-96";
        const string Hawlucha = "STS-97";
        const string ArmorFossil = "STS-98";
        const string PokePuff = "STS-99";
        const string GreedyDice = "STS-102";
        const string NinjaBoy = "STS-103";
        const string Ranger = "STS-104";
        const string SpecialCharge = "STS-105";
        const string Sycamore = "STS-114";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Dual_Types_And_Restored_Pokemon()
        {
            var own = db.Cards.Where(d => d.SetCode == "STS" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(107, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("STS-26", C("STS-115").BaseKey, "secret rare Volcanion-EX");
            Assert.AreEqual("STS-104", C("STS-113").BaseKey, "full art Pokémon Ranger");
            Assert.IsTrue(C(VolcanionEX).HasType(EnergyType.Fire) && C(VolcanionEX).HasType(EnergyType.Water), "Fire and Water");
            Assert.IsTrue(C(Bisharp).HasType(EnergyType.Darkness) && C(Bisharp).HasType(EnergyType.Metal));
            Assert.AreEqual(Stage.Restored, C(Shieldon).Stage);
            Assert.IsFalse(C(Shieldon).IsBasicPokemon, "a Restored Pokémon isn't a Basic Pokémon");
            Assert.IsFalse(C(Shieldon).IsEvolution);
            Assert.IsTrue(C(YanmegaBreak).IsBreak);
        }

        // ------------------------------------------------------------------ Restored Pokémon

        [Test]
        public void Armor_Fossil_Shieldon_Benches_Shieldon_From_The_Bottom_7_And_Shieldon_Cant_Be_Played_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Armor Fossil Shieldon"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(ArmorFossil));
            e.ScenarioAddToHand(0, C(Shieldon));
            e.ScenarioAddToDeck(0, C(Shieldon), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Bench Shieldon"), "not a Basic Pokémon");
            Step(e);
            Assert.AreEqual("Shieldon", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Shieldon"), "the one in the hand stays there");
        }

        [Test]
        public void Restored_Barrier_Removes_Weakness_From_Restored_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var shieldon = e.ScenarioActive(0, C(Shieldon));
            e.ScenarioBench(0, C(Anorith));
            var bastiodon = e.ScenarioBench(0, C(Shieldon));
            e.ScenarioEvolve(bastiodon, C(Bastiodon));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNull(e.WeaknessOf(shieldon));
            Assert.AreEqual(EnergyType.Fire, e.WeaknessOf(bastiodon), "Bastiodon is a Stage 1 Pokémon");
        }

        // ------------------------------------------------------------------ Grass

        [Test]
        public void Sonic_Vision_Ignores_Energy_With_Exactly_4_Cards_In_Hand_Even_Under_Yanmega_Break()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var yanmega = e.ScenarioActive(0, C(Yanma));
            e.ScenarioEvolve(yanmega, C(Yanmega));
            e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Assault Boom")), "3 cards");
            e.ScenarioAddToHand(0, C(Psychic));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Assault Boom")), "exactly 4 cards");
            e.ScenarioEvolve(yanmega, C(YanmegaBreak));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Barrier Break")), "the BREAK keeps Sonic Vision");
        }

        [Test]
        public void Barrier_Break_Ignores_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Barrier Break"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Yanma));
            e.ScenarioEvolve(mon, C(Yanmega));
            e.ScenarioEvolve(mon, C(YanmegaBreak));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mon, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, lapras.DamageCounters, "Lapras-GX is weak to Grass, but not for Barrier Break");
        }

        [Test]
        public void Wicked_Wind_Switches_Off_Tools_And_Pokemon_Ranger_Removes_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Wicked Wind"), new ScriptedAgent().Then("Play Pokémon Ranger"));
            var shiftry = e.ScenarioActive(0, C(Seedot));
            e.ScenarioEvolve(shiftry, C(Nuzleaf));
            e.ScenarioEvolve(shiftry, C(Shiftry));
            e.ScenarioAttach(shiftry, C(Grass));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(1, C(Ranger));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.ToolsDisabled());
            Assert.IsTrue(e.StadiumAndToolsSwitchedOff());
            NextTurn(e);
            Assert.IsTrue(e.ToolsDisabled(), "still during the opponent's next turn");
            Step(e);
            Assert.IsFalse(e.StadiumAndToolsSwitchedOff(), "Pokémon Ranger removed the attack's effect");
        }

        // ------------------------------------------------------------------ Fire

        [Test]
        public void Steam_Up_Adds_30_To_Basic_Fire_Pokemon_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Steam Up", "Attack: Power Heater"), new ScriptedAgent());
            var volcanion = e.ScenarioActive(0, C(Volcanion));
            e.ScenarioAttach(volcanion, C(Fire));
            var ex = e.ScenarioBench(0, C(VolcanionEX));
            e.ScenarioAddToHand(0, C(Fire));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(5, snorlax.DamageCounters, "20 + 30");
            Assert.AreEqual(1, ex.Energy.Count, "Power Heater attached the discarded Fire Energy");
        }

        // ------------------------------------------------------------------ Water

        [Test]
        public void Ultimate_Blade_Knocks_Out_A_Pokemon_Left_With_60_HP_Or_Less()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ultimate Blade"), new ScriptedAgent());
            var samurott = e.ScenarioActive(0, C(Oshawott));
            e.ScenarioEvolve(samurott, C(Dewott));
            e.ScenarioEvolve(samurott, C(Samurott));
            e.ScenarioAttach(samurott, C(Water));
            e.ScenarioAttach(samurott, C(Water));
            var steenee = e.ScenarioActive(1, C(Bounsweet));
            e.ScenarioEvolve(steenee, C(Steenee));
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Players[1].InPlay().Contains(steenee), "90 - 40 = 50 HP left");
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Lock_On_Stops_Retreat_And_Sets_Up_120_More_Damage_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lock-On"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Clauncher));
            e.ScenarioEvolve(mon, C(Clawitzer));
            e.ScenarioEvolve(mon, C(ClawitzerBreak));
            e.ScenarioAttach(mon, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Effects.OfType<CantRetreatEffect>().Any(fx => fx.TargetUid == snorlax.Uid));
            var more = e.State.Effects.OfType<DamageTakenEffect>().Single(fx => fx.TargetUid == snorlax.Uid);
            Assert.AreEqual(5, more.ActiveFromTurn, "during your next turn");
        }

        [Test]
        public void Melting_Floe_Mills_3_Of_The_Opponents_Cards_For_Each_Water_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Melting Floe"), new ScriptedAgent());
            var avalugg = e.ScenarioActive(0, C(Bergmite));
            e.ScenarioEvolve(avalugg, C(Avalugg));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(avalugg, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Water)));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Water)));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(19, e.State.Players[0].Deck.Count, "22 - 3");
            Assert.AreEqual(14, e.State.Players[1].Deck.Count, "2 Water Energy: 6 cards");
        }

        [Test]
        public void Mega_Boost_Only_Feeds_Xy_Era_Megas()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mega Boost"), new ScriptedAgent());
            var mega = e.ScenarioActive(0, C(GardevoirEX));
            e.ScenarioEvolve(mega, C(MGardevoirEX));
            var claw = e.ScenarioBench(0, C(Clauncher));
            e.ScenarioEvolve(claw, C(Clawitzer));
            e.ScenarioAddToHand(0, C(DoubleColorless));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mega.Energy.Count(c => c.Def.Name == "Double Colorless Energy"));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var camerupt = e2.ScenarioActive(0, C(Numel));
            e2.ScenarioEvolve(camerupt, C(MCamerupt));
            var claw2 = e2.ScenarioBench(0, C(Clauncher));
            e2.ScenarioEvolve(claw2, C(Clawitzer));
            e2.ScenarioAddToHand(0, C(DoubleColorless));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.Contains("Mega Boost")), "a Mega Evolution Pokémon ex isn't an XY-era Mega");
        }

        // ------------------------------------------------------------------ Lightning

        [Test]
        public void Shocking_Light_Hits_Only_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shocking Light"), new ScriptedAgent());
            var amph = e.ScenarioActive(0, C(Mareep));
            e.ScenarioEvolve(amph, C(Flaaffy));
            e.ScenarioEvolve(amph, C(Ampharos));
            var pidgeot = e.ScenarioActive(1, C(PidgeotEX));
            e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, pidgeot.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var amph2 = e2.ScenarioActive(0, C(Mareep));
            e2.ScenarioEvolve(amph2, C(Flaaffy));
            e2.ScenarioEvolve(amph2, C(Ampharos));
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.Contains("Shocking Light")), "Pokémon-GX aren't Pokémon-EX");
        }

        [Test]
        public void Double_Thread_Hits_Benched_Pokemon_With_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Double Thread"), new ScriptedAgent());
            var galv = e.ScenarioActive(0, C(Joltik));
            e.ScenarioEvolve(galv, C(Galvantula));
            e.ScenarioAttach(galv, C(Lightning));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, snorlax.DamageCounters, "the erratum: Benched Pokémon only (your ruling, v134)");
            Assert.AreEqual(6, lapras.DamageCounters, "Galvantula is also a Grass Pokémon: Lapras-GX's Weakness applies on the Bench");
        }

        // ------------------------------------------------------------------ Fighting and Darkness

        [Test]
        public void Poison_Enzyme_Stops_Damage_From_Poisoned_Attackers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Collapse"));
            var toxicroak = e.ScenarioActive(0, C(Croagunk));
            e.ScenarioEvolve(toxicroak, C(Toxicroak));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(snorlax, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            e.ApplyCondition(snorlax, SpecialCondition.Poisoned);
            Step(e);
            Assert.AreEqual(0, toxicroak.DamageCounters);
        }

        [Test]
        public void Poison_Up_Adds_3_Poison_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Poison Up"), new ScriptedAgent());
            var croagunk = e.ScenarioActive(0, C(Croagunk));
            e.ScenarioAttach(croagunk, C(Fighting));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(snorlax, SpecialCondition.Poisoned);
            Step(e);
            Assert.AreEqual(4, snorlax.PoisonCounterBase);
        }

        [Test]
        public void Tear_Away_Returns_A_Tool_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Tear Away"), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C(Sneasel));
            e.ScenarioEvolve(weavile, C(Weavile));
            e.ScenarioAttachTool(weavile, C(ChoiceBand));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, weavile.Tools.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Choice Band"));
        }

        [Test]
        public void Mach_Claw_Ignores_Resistance()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mach Claw"), new ScriptedAgent());
            var bisharp = e.ScenarioActive(0, C(Pawniard));
            e.ScenarioEvolve(bisharp, C(Bisharp));
            e.ScenarioAttach(bisharp, C(Darkness));
            e.ScenarioAttach(bisharp, C(Darkness));
            var gardevoir = e.ScenarioActive(1, C(GardevoirEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, gardevoir.DamageCounters, "60, x2 for the Metal Weakness, no -20 for the Darkness Resistance");
        }

        // ------------------------------------------------------------------ Metal

        [Test]
        public void Mystic_Heart_Stops_Effects_On_Pokemon_With_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Hypnosis"));
            var active = e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAttach(active, C(Metal));
            e.ScenarioBench(0, C(MagearnaEX));
            var haunter = e.ScenarioActive(1, C(Gastly));
            e.ScenarioEvolve(haunter, C(Haunter));
            e.ScenarioAttach(haunter, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsFalse(active.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Mystic_Heart_Prevents_Shell_Trap_Counters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Shell Trap"));
            var magearna = e.ScenarioActive(0, C(MagearnaEX));
            e.ScenarioAttach(magearna, C(Metal));
            var turtonator = e.ScenarioActive(1, C("FST-198"));
            e.ScenarioAttach(turtonator, C(Fire));
            e.ScenarioAttach(turtonator, C(Fighting));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            int before = magearna.DamageCounters;
            e.ScenarioSetTurn(0, 5);
            e.DealAttackDamage(magearna, turtonator, 30);
            Assert.AreEqual(before, magearna.DamageCounters, "Shell Trap's 8 counters are an effect of the opponent's attack");
        }

        [Test]
        public void Heavy_Bumper_Takes_10_Less_For_Each_Retreat_Cost_Colorless()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Collapse"));
            var klinklang = e.ScenarioActive(0, C(Klink));
            e.ScenarioEvolve(klinklang, C(Klang));
            e.ScenarioEvolve(klinklang, C(Klinklang));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(snorlax, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(4, klinklang.DamageCounters, "80 - 10 for each of Snorlax-GX's 4 Retreat Cost");
        }

        // ------------------------------------------------------------------ Fairy

        [Test]
        public void Despair_Ray_Discards_Benched_Pokemon_For_10_More_Each()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Despair Ray", "Yes", "Bounsweet|Bounsweet"), new ScriptedAgent());
            var mega = e.ScenarioActive(0, C(GardevoirEX));
            e.ScenarioEvolve(mega, C(MGardevoirEX));
            e.ScenarioAttach(mega, C(Fairy));
            e.ScenarioAttach(mega, C(Psychic));
            e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioBench(0, C(Bounsweet));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, snorlax.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Wonder_Lock_Makes_Klefki_A_Tool_That_Blocks_Xy_Megas_Until_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Wonder Lock"), new ScriptedAgent().Then("Attack: Mach Cyclone"));
            var active = e.ScenarioActive(0, C(SnorlaxGX));
            var klefki = e.ScenarioBench(0, C(Klefki));
            e.ScenarioAttach(klefki, C(Psychic));
            var mega = e.ScenarioActive(1, C(PidgeotEX));
            e.ScenarioEvolve(mega, C(MPidgeotEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mega, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(1, active.Tools.Count(c => c.Def.Name == "Klefki"));
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Psychic Energy"), "Klefki's own Energy is discarded");
            NextTurn(e);
            Step(e);
            Assert.AreEqual(0, active.DamageCounters, "M Pidgeot-EX is an XY-era Mega");
            e.EndTurn();
            Assert.AreEqual(0, active.Tools.Count, "discarded at the end of the opponent's turn");
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Klefki"));
        }

        // ------------------------------------------------------------------ Colorless

        [Test]
        public void Stall_Only_Works_On_Your_First_Turn_Going_Second()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meowth = e.ScenarioActive(1, C(Meowth));
            e.ScenarioAttach(meowth, C(Psychic));
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioSetTurn(1, 2);
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Label.Contains("Stall")));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label.Contains("Stall")));
        }

        [Test]
        public void Gale_Wings_Lets_Talonflame_Start_Active_And_Keep_A_Hand_Without_Basics()
        {
            var scripts = StandardCards.CreateRegistry();
            Assert.IsTrue(scripts.For(C(Talonflame)).CanStartAsActive);
            Assert.IsTrue(scripts.For(C(Talonflame)).MayKeepOpeningHandWithIt);
        }

        [Test]
        public void Sudden_Cyclone_Makes_The_Opponent_Switch()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Hawlucha", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(Hawlucha));
            e.ScenarioActive(1, C(SnorlaxGX));
            var benched = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(benched, e.State.Players[1].Active);
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Ninja_Boy_Swaps_A_Basic_In_Play_Keeping_Everything()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ninja Boy"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Bounsweet));
            active.DamageCounters = 2;
            e.ScenarioAttach(active, C(Grass));
            e.ScenarioAddToHand(0, C(NinjaBoy));
            e.ScenarioAddToDeck(0, C(LaprasGX), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(active, e.State.Players[0].Active);
            Assert.AreEqual("Lapras-GX", active.Name);
            Assert.AreEqual(2, active.DamageCounters);
            Assert.AreEqual(1, active.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Deck.Count(c => c.Def.Name == "Bounsweet"), "the first Pokémon went into the deck");
        }

        [Test]
        public void Greedy_Dice_Is_Played_When_Taken_As_A_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Power Heater"), new ScriptedAgent());
            var volcanion = e.ScenarioActive(0, C(Volcanion));
            e.ScenarioAttach(volcanion, C(Fire));
            e.State.Players[0].Prizes.Clear();
            e.ScenarioAddPrizes(0, C(GreedyDice), 6);
            var target = e.ScenarioActive(1, C(Bounsweet));
            target.DamageCounters = 5;
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            int taken = 6 - me.Prizes.Count;
            Assert.GreaterOrEqual(taken, 1);
            Assert.AreEqual(0, me.Hand.Count(c => c.Def.Name == "Greedy Dice"), "each one was played as it was taken");
            Assert.AreEqual(taken, me.Discard.Count(c => c.Def.Name == "Greedy Dice"));
        }

        [Test]
        public void Captivating_Poke_Puff_Benches_The_Opponents_Basics()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Captivating Poké Puff", "Bounsweet"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(PokePuff));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(1, C(Bounsweet));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Bounsweet", e.State.Players[1].Bench.Single().Name);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Poke_Puff_Needs_Bench_Room_And_Ranger_Needs_An_Attack_Effect()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(PokePuff));
            e.ScenarioAddToHand(0, C(Ranger));
            e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioAddToHand(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            var actions = e.GetMainActions(0);
            Assert.IsFalse(actions.Any(o => o.Action == ActionType.PlayTrainer && o.Card.Name == "Captivating Poké Puff"), "the opponent's Bench is full");
            Assert.IsFalse(actions.Any(o => o.Action == ActionType.PlayTrainer && o.Card.Name == "Pokémon Ranger"), "no effect of an attack to remove");
        }

        [Test]
        public void Special_Charge_And_Professor_Sycamore()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Special Charge"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(SpecialCharge));
            e.ScenarioAddToDiscard(0, C(DoubleColorless), 2);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(22, e.State.Players[0].Deck.Count);

            var e2 = Scenario(new ScriptedAgent().Then("Play Professor Sycamore"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Bounsweet));
            e2.ScenarioAddToHand(0, C(Sycamore));
            for (int i = 0; i < 3; i++) e2.ScenarioAddToHand(0, C(Fire));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(7, e2.State.Players[0].Hand.Count);
            Assert.AreEqual(3, e2.State.Players[0].Discard.Count(c => c.Def.Name == "Fire Energy"));
        }
    }
}
