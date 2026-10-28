using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v92: Evolutions (EVO 1-113), the first XY-era set: Pokémon-EX, Mega Evolution (with Spirit Link), BREAK Evolution (its
    /// Abilities now carry over too), Buzzap Thunder, and Imakuni?'s Doduo running away into the Lost Zone (your ruling). XY-era
    /// Mega Evolution rules only cover XY-era Megas (your rules update).
    /// </summary>
    public class EvoTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string DoubleColorless = "SLG-69";
        const string ChoiceBand = "BUS-162";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string Steenee = "SUM-19";
        const string Snubbull = "DET-15";

        const string Beedrill = "EVO-7";
        const string Charizard = "EVO-11";
        const string CharizardEX = "EVO-12";
        const string MCharizardEX = "EVO-13";
        const string Ninetales = "EVO-15";
        const string NinetalesBreak = "EVO-16";
        const string Growlithe = "EVO-17";
        const string Arcanine = "EVO-18";
        const string Poliwrath = "EVO-25";
        const string MSlowbroEX = "EVO-27";
        const string Staryu = "EVO-30";
        const string Starmie = "EVO-31";
        const string StarmieBreak = "EVO-32";
        const string Pikachu = "EVO-35";
        const string Raichu = "EVO-36";
        const string Voltorb = "EVO-39";
        const string Electrode = "EVO-40";
        const string Zapdos = "EVO-42";
        const string Nidoking = "EVO-45";
        const string NidokingBreak = "EVO-46";
        const string Gastly = "EVO-47";
        const string Haunter = "EVO-48";
        const string Mewtwo = "EVO-51";
        const string Mew = "EVO-53";
        const string Diglett = "EVO-55";
        const string Machop = "EVO-57";
        const string Machamp = "EVO-59";
        const string Clefairy = "EVO-63";
        const string PidgeotEX = "EVO-64";
        const string MPidgeotEX = "EVO-65";
        const string Rattata = "EVO-66";
        const string Raticate = "EVO-67";
        const string Porygon = "EVO-71";
        const string DragoniteEX = "EVO-72";
        const string CharizardSpiritLink = "EVO-75";
        const string DevolutionSpray = "EVO-76";
        const string Maintenance = "EVO-79";
        const string ProfessorOaksHint = "EVO-84";
        const string Revive = "EVO-85";
        const string ImakunisDoduo = "EVO-112";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Reprints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "EVO" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(84, own.Count, "the cards that aren't already in the pool");
            Assert.IsTrue(own.Where(d => d.Name != "Imakuni?'s Doduo").All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(ImakunisDoduo)), "it can't be used at official tournaments");
            Assert.AreEqual("Evolutions", C(Beedrill).PrintSetName);
            Assert.AreEqual("EVO-2", C("EVO-100").BaseKey, "full art M Venusaur-EX");
            Assert.AreEqual("HIF-53", C("EVO-107").BaseKey, "full art Brock's Grit");
            Assert.AreEqual("CEL-CC6", C("EVO-113").BaseKey, "Here Comes Team Rocket!");
            Assert.AreEqual("WP-25", C("EVO-110").BaseKey, "Flying Pikachu");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C("WP-25")), "Evolutions reprinted it");
            Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(C("WP-28")), "Surfing Pikachu too");
            Assert.AreEqual("EVO-35", C(Pikachu).BaseKey, "not Celebrations' Pikachu: that one has no Resistance");
        }

        [Test]
        public void Mega_And_Break_Cards_Have_Their_Rules()
        {
            var mega = C(MCharizardEX);
            Assert.IsTrue(mega.IsOldMega);
            Assert.IsTrue(mega.IsPokemonEX);
            Assert.AreEqual(2, mega.PrizeValue);
            Assert.AreEqual("Charizard-EX", mega.EvolvesFrom);
            Assert.IsTrue(C(NinetalesBreak).IsBreak);
            Assert.AreEqual("Ninetales", C(NinetalesBreak).EvolvesFrom);
            Assert.AreEqual(1, C(NinetalesBreak).PrizeValue);
            var link = C(CharizardSpiritLink);
            Assert.IsTrue(link.IsTool);
            Assert.IsFalse(link.IsItem, "a Pokémon Tool is never an Item card");
        }

        // ------------------------------------------------------------------ Mega Evolution and Spirit Link

        [Test]
        public void Mega_Evolving_Ends_The_Turn_Unless_Its_Spirit_Link_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(CharizardEX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(MCharizardEX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "becoming a M Pokémon-EX ends the turn");

            var e2 = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var linked = e2.ScenarioActive(0, C(CharizardEX));
            e2.ScenarioAttachTool(linked, C(CharizardSpiritLink));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioAddToHand(0, C(MCharizardEX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.MainPhaseStep(), "Charizard Spirit Link keeps the turn going");
            Assert.AreEqual("M Charizard-EX", linked.Name);
        }

        [Test]
        public void Xy_Mega_Rules_Only_Cover_Xy_Megas_Not_Mega_Evolution_Pokemon_Ex()
        {
            Assert.IsTrue(C(MCharizardEX).IsXYMegaEvolution);
            Assert.IsFalse(C("MEG-22").IsXYMegaEvolution, "Mega Camerupt ex is a Mega Evolution Pokémon ex, not an XY-era Mega");
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C("MEG-21"));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C("MEG-22"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep(), "only the XY-era Mega Evolution rule ends the turn");
            Assert.AreEqual("Mega Camerupt ex", e.State.Players[0].Active.Name);
        }

        // ------------------------------------------------------------------ BREAK Evolution

        [Test]
        public void Ninetales_Break_Keeps_Fire_Blast_And_Explosive_Fireball_Counts_The_Discarded_Fire_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Explosive Fireball"), new ScriptedAgent());
            var nine = e.ScenarioActive(0, C(Ninetales));
            e.ScenarioEvolve(nine, C(NinetalesBreak));
            e.ScenarioAttach(nine, C(Fire));
            e.ScenarioAttach(nine, C(Fire));
            e.ScenarioAttach(nine, C(DoubleColorless));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Fire Blast")), "the attacks of the Pokémon under it");
            Step(e);
            Assert.AreEqual(13, snorlax.DamageCounters, "10 + 60 for each of the 2 Fire Energy");
            Assert.AreEqual(1, nine.Energy.Count, "the Double Colorless Energy stays");
        }

        [Test]
        public void Starmie_Break_Keeps_Space_Beacon_But_Not_A_Second_Use_In_The_Same_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var starmie = e.ScenarioActive(0, C(Staryu));
            e.ScenarioEvolve(starmie, C(Starmie));
            e.ScenarioEvolve(starmie, C(StarmieBreak));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Water), 2);
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Space Beacon")), "a BREAK Pokémon has the Abilities of the Pokémon under it");

            var e2 = Scenario(new ScriptedAgent().Then("Use Space Beacon", "Psychic Energy", "Evolve"), new ScriptedAgent());
            var second = e2.ScenarioActive(0, C(Staryu));
            e2.ScenarioEvolve(second, C(Starmie));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioAddToHand(0, C(StarmieBreak));
            e2.ScenarioAddToDiscard(0, C(Water), 2);
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(2, e2.State.Players[0].Hand.Count(c => c.Def.Name == "Water Energy"));
            e2.ScenarioAddToHand(0, C(Psychic));
            Step(e2);
            Assert.AreEqual("Starmie BREAK", second.Name);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.Contains("Space Beacon")), "already used this turn (Greninja BREAK ruling)");
        }

        [Test]
        public void Space_Beacon_Cant_Take_Back_The_Card_It_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Space Beacon", "Water Energy"), new ScriptedAgent());
            var starmie = e.ScenarioActive(0, C(Staryu));
            e.ScenarioEvolve(starmie, C(Starmie));
            e.ScenarioActive(1, C(SnorlaxGX));
            var tossed = e.ScenarioAddToHand(0, C(Water));
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(1, me.Hand.Count);
            Assert.AreNotEqual(tossed.Id, me.Hand[0].Id, "the other Water Energy");
            CollectionAssert.Contains(me.Discard, tossed);
        }

        [Test]
        public void Break_Star_Hits_Only_Pokemon_Break()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Break Star"), new ScriptedAgent());
            var starmie = e.ScenarioActive(0, C(Staryu));
            e.ScenarioEvolve(starmie, C(Starmie));
            e.ScenarioEvolve(starmie, C(StarmieBreak));
            e.ScenarioAttach(starmie, C(Water));
            var active = e.ScenarioActive(1, C(SnorlaxGX));
            var broken = e.ScenarioBench(1, C(Ninetales));
            e.ScenarioEvolve(broken, C(NinetalesBreak));
            var plain = e.ScenarioBench(1, C(Ninetales));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, active.DamageCounters);
            Assert.AreEqual(10, broken.DamageCounters);
            Assert.AreEqual(0, plain.DamageCounters);
        }

        [Test]
        public void Nidoking_Break_Uses_Tail_Swing_On_Benched_Basics_And_Toxic_Drill_Poisons_For_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tail Swing"), new ScriptedAgent());
            var king = e.ScenarioActive(0, C(Nidoking));
            e.ScenarioEvolve(king, C(NidokingBreak));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(king, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var basic = e.ScenarioBench(1, C(Bounsweet));
            var evolved = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioEvolve(evolved, C(Steenee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, lapras.DamageCounters);
            Assert.AreEqual(2, basic.DamageCounters);
            Assert.AreEqual(0, evolved.DamageCounters, "only Basic Pokémon");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Toxic Drill"), new ScriptedAgent());
            var drill = e2.ScenarioActive(0, C(Nidoking));
            e2.ScenarioEvolve(drill, C(NidokingBreak));
            for (int i = 0; i < 3; i++) e2.ScenarioAttach(drill, C(Psychic));
            var target = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(2, target.PoisonCounterBase);
        }

        // ------------------------------------------------------------------ Grass and Fire

        [Test]
        public void Swarming_Sting_Does_40_For_Each_Beedrill()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Swarming Sting"), new ScriptedAgent());
            var bee = e.ScenarioActive(0, C(Beedrill));
            e.ScenarioAttach(bee, C(Grass));
            e.ScenarioAttach(bee, C(Psychic));
            e.ScenarioBench(0, C(Beedrill));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, snorlax.DamageCounters);
        }

        [Test]
        public void Energy_Burn_Makes_Every_Energy_On_Charizard_Fire()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zard = e.ScenarioActive(0, C(Charizard));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(zard, C(Psychic));
            e.ScenarioAttach(zard, C(DoubleColorless));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(5, e.EnergyUnits(zard).Count);
            Assert.IsTrue(e.EnergyUnits(zard).All(u => u == EnergyMask.Of(EnergyType.Fire)));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Fire Spin")));
        }

        [Test]
        public void Burning_Road_Moves_Fire_Energy_When_Arcanine_Comes_In()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat", "Arcanine", "Yes", "Fire Energy|Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Beedrill));
            var arcanine = e.ScenarioBench(0, C(Growlithe));
            e.ScenarioEvolve(arcanine, C(Arcanine));
            var donor = e.ScenarioBench(0, C(Growlithe));
            e.ScenarioAttach(donor, C(Fire));
            e.ScenarioAttach(donor, C(Fire));
            e.ScenarioAttach(donor, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(arcanine, e.State.Players[0].Active);
            Assert.AreEqual(2, arcanine.Energy.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(1, donor.Energy.Count, "only Fire Energy moves");
        }

        // ------------------------------------------------------------------ Water

        [Test]
        public void Dashing_Punch_Does_50_More_After_Coming_In_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dashing Punch"), new ScriptedAgent());
            var poli = e.ScenarioActive(0, C(Poliwrath));
            e.ScenarioAttach(poli, C(Water));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            poli.MovedToActiveOnTurn = 3;
            Step(e);
            Assert.AreEqual(10, snorlax.DamageCounters);
        }

        [Test]
        public void Loll_Roll_Spin_Confuses_Itself_And_Is_Stronger_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Loll Roll Spin"), new ScriptedAgent());
            var slowbro = e.ScenarioActive(0, C(MSlowbroEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(slowbro, C(Water));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, snorlax.DamageCounters);
            Assert.IsTrue(slowbro.Has(SpecialCondition.Confused));
            Assert.AreEqual(1, e.State.Effects.OfType<AttackBoostEffect>().Count(fx => fx.TargetUid == slowbro.Uid && fx.Bonus == 100));
        }

        // ------------------------------------------------------------------ Lightning

        [Test]
        public void Spark_Bolt_Can_Discard_All_Energy_For_70_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spark Bolt", "Yes"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(Pikachu));
            e.ScenarioEvolve(raichu, C(Raichu));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(raichu, C(Lightning));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(14, snorlax.DamageCounters);
            Assert.AreEqual(0, raichu.Energy.Count);
        }

        [Test]
        public void Buzzap_Thunder_Knocks_Electrode_Out_And_Attaches_It_As_2_Lightning_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buzzap Thunder"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            var electrode = e.ScenarioBench(0, C(Voltorb));
            e.ScenarioEvolve(electrode, C(Electrode));
            e.ScenarioAttach(electrode, C(Lightning));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.IsFalse(me.Bench.Contains(electrode), "Electrode was Knocked Out");
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the opponent took a Prize card for it");
            Assert.AreEqual(1, pikachu.Energy.Count(c => c.Def.Name == "Electrode"));
            Assert.AreEqual(2, e.EnergyUnits(pikachu).Count(u => u == EnergyMask.Of(EnergyType.Lightning)));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Voltorb"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Lightning Energy"), "its own Energy is discarded");
        }

        [Test]
        public void Enhanced_Hammer_Can_Discard_A_Buzzapped_Electrode()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buzzap Thunder"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            var electrode = e.ScenarioBench(0, C(Voltorb));
            e.ScenarioEvolve(electrode, C(Electrode));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, pikachu.Energy.Count(c => c.Def.Name == "Electrode"));
            var hammerer = new ScriptedAgent().Then("Play Enhanced Hammer");
            var e2 = Scenario(hammerer, new ScriptedAgent());
            e2.ScenarioActive(0, C(SnorlaxGX));
            e2.ScenarioAddToHand(0, C("TWM-148"));
            var target = e2.ScenarioActive(1, C(Pikachu));
            e2.ScenarioAttach(target, C(Electrode));
            e2.ScenarioAttach(target, C(Lightning));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e2.GetMainActions(0).Any(o => o.Label == "Play Enhanced Hammer"), "the attached Electrode is a Special Energy card (your ruling)");
            Step(e2);
            Assert.AreEqual(0, target.Energy.Count(c => c.Def.Name == "Electrode"));
            Assert.AreEqual(1, target.Energy.Count(c => c.Def.Name == "Lightning Energy"), "basic Energy stays");
            Assert.AreEqual(1, e2.State.Players[1].Discard.Count(c => c.Def.Name == "Electrode"), "back in the discard pile as a Pokémon card");
        }

        [Test]
        public void Buzzap_Thunder_Needs_Another_Lightning_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var electrode = e.ScenarioActive(0, C(Voltorb));
            e.ScenarioEvolve(electrode, C(Electrode));
            e.ScenarioBench(0, C(Machop));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Buzzap Thunder")));
        }

        // ------------------------------------------------------------------ Psychic

        [Test]
        public void Little_Grudge_Discards_An_Energy_From_The_Pokemon_That_Knocks_Gastly_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Little Grudge"), new ScriptedAgent().Then("Attack: Collapse"));
            var gastly = e.ScenarioActive(0, C(Gastly));
            e.ScenarioAttach(gastly, C(Psychic));
            e.ScenarioBench(0, C(Machop));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(snorlax, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.IsFalse(e.State.Players[0].InPlay().Contains(gastly), "Gastly was Knocked Out");
            Assert.AreEqual(3, snorlax.Energy.Count, "Little Grudge discarded 1 of Snorlax-GX's Energy");
        }

        [Test]
        public void Dream_Eater_Only_Works_On_A_Sleeping_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dream Eater"), new ScriptedAgent());
            var haunter = e.ScenarioActive(0, C(Gastly));
            e.ScenarioEvolve(haunter, C(Haunter));
            e.ScenarioAttach(haunter, C(Psychic));
            e.ScenarioAttach(haunter, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, snorlax.DamageCounters);
        }

        [Test]
        public void Barrier_Cant_Be_Used_Two_Turns_In_A_Row()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioAttach(mewtwo, C(Psychic));
            e.ScenarioAttach(mewtwo, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Barrier")));
            e.State.Players[0].LastAttackName = "Barrier";
            e.State.Players[0].LastAttackTurn = 1;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Barrier")), "1 of your Pokémon used Barrier during your last turn");
        }

        [Test]
        public void Neutral_Shield_Stops_Evolution_Pokemon_But_Not_Basics()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Stomp"));
            var mew = e.ScenarioActive(0, C(Mew));
            var steenee = e.ScenarioActive(1, C(Bounsweet));
            e.ScenarioEvolve(steenee, C(Steenee));
            e.ScenarioAttach(steenee, C(Psychic));
            e.ScenarioAttach(steenee, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, mew.DamageCounters, "Steenee is an Evolution Pokémon");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Splash"));
            var mew2 = e2.ScenarioActive(0, C(Mew));
            var bounsweet = e2.ScenarioActive(1, C(Bounsweet));
            e2.ScenarioAttach(bounsweet, C(Psychic));
            e2.ScenarioSetTurn(1, 4);
            Step(e2);
            Assert.AreEqual(1, mew2.DamageCounters, "a Basic Pokémon's attack still hits");
        }

        // ------------------------------------------------------------------ Fighting

        [Test]
        public void Submerge_Protects_Benched_Diglett_And_Dig_Through_Only_Hits_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Feather Lance"), new ScriptedAgent());
            var pidgeot = e.ScenarioActive(0, C(PidgeotEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(pidgeot, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            var diglett = e.ScenarioBench(1, C(Diglett));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, snorlax.DamageCounters);
            Assert.AreEqual(0, diglett.DamageCounters, "Submerge");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Dig Through"), new ScriptedAgent());
            var digger = e2.ScenarioActive(0, C(Diglett));
            e2.ScenarioAttach(digger, C(Fighting));
            e2.ScenarioAttach(digger, C(Fighting));
            var active = e2.ScenarioActive(1, C(SnorlaxGX));
            var benched = e2.ScenarioBench(1, C(Snubbull));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, active.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters);
        }

        [Test]
        public void Counterattack_Puts_3_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Collapse"));
            var machamp = e.ScenarioActive(0, C(Machop));
            e.ScenarioEvolve(machamp, C("EVO-58"));
            e.ScenarioEvolve(machamp, C(Machamp));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(snorlax, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(8, machamp.DamageCounters);
            Assert.AreEqual(3, snorlax.DamageCounters);
        }

        // ------------------------------------------------------------------ Colorless and Fairy

        [Test]
        public void Metronome_Uses_The_Defending_Pokemons_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Metronome", "Collapse"), new ScriptedAgent());
            var clefairy = e.ScenarioActive(0, C(Clefairy));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(clefairy, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, snorlax.DamageCounters);
            Assert.IsTrue(clefairy.Has(SpecialCondition.Asleep), "Collapse puts the Pokémon using it to sleep");
        }

        [Test]
        public void Mirror_Move_Returns_Last_Turns_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mirror Move"), new ScriptedAgent());
            var pidgeot = e.ScenarioActive(0, C(PidgeotEX));
            e.ScenarioAttach(pidgeot, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            pidgeot.DamageCounters = 6;
            pidgeot.AttackDamageTurn = 2;
            pidgeot.AttackDamageThatTurn = 60;
            Step(e);
            Assert.AreEqual(6, snorlax.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Mirror Move"), new ScriptedAgent());
            var fresh = e2.ScenarioActive(0, C(PidgeotEX));
            e2.ScenarioAttach(fresh, C(Psychic));
            var other = e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, other.DamageCounters, "not damaged last turn: nothing");
        }

        [Test]
        public void Mach_Cyclone_Can_Make_The_Opponent_Switch()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mach Cyclone", "Yes"), new ScriptedAgent());
            var mega = e.ScenarioActive(0, C(PidgeotEX));
            e.ScenarioEvolve(mega, C(MPidgeotEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mega, C(Psychic));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            var benched = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, snorlax.DamageCounters);
            Assert.AreSame(benched, e.State.Players[1].Active);
        }

        [Test]
        public void Mischievous_Fang_Discards_The_Opponents_Active_Tools()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Rattata", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioAddToHand(0, C(Rattata));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAttachTool(snorlax, C(ChoiceBand));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, snorlax.Tools.Count);
            Assert.AreEqual(1, e.State.Players[1].Discard.Count(c => c.Def.Name == "Choice Band"));
        }

        [Test]
        public void Shadowy_Bite_Counts_Special_Energy_In_The_Opponents_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shadowy Bite"), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(Rattata));
            e.ScenarioEvolve(raticate, C(Raticate));
            e.ScenarioAttach(raticate, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAddToDiscard(1, C(DoubleColorless), 2);
            e.ScenarioAddToDiscard(1, C(Water), 3);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, lapras.DamageCounters);
        }

        [Test]
        public void Conversion_3_Changes_The_Defending_Pokemons_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Conversion 3", "Fighting"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(Porygon));
            e.ScenarioAttach(porygon, C(Psychic));
            var machop = e.ScenarioBench(0, C(Machop));
            var zapdos = e.ScenarioActive(1, C(Zapdos));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(zapdos), "even a Pokémon with no printed Weakness");
            e.DealAttackDamage(machop, zapdos, 30);
            Assert.AreEqual(4, zapdos.DamageCounters, "30 x2 for the new Weakness, then -20 for Zapdos's Fighting Resistance");
        }

        [Test]
        public void Pull_Up_Takes_2_Basics_But_Not_Dragonite_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Dragonite-EX", "Yes", "Pikachu|Machop"), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioAddToHand(0, C(DragoniteEX));
            e.ScenarioAddToDiscard(0, C(DragoniteEX), 1);
            e.ScenarioAddToDiscard(0, C(Pikachu), 1);
            e.ScenarioAddToDiscard(0, C(Machop), 1);
            e.ScenarioAddToDiscard(0, C(Gastly), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count);
            Assert.IsTrue(me.Hand.Any(c => c.Def.Name == "Pikachu") && me.Hand.Any(c => c.Def.Name == "Machop"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Dragonite-EX"));
        }

        // ------------------------------------------------------------------ Imakuni?'s Doduo (your ruling)

        [Test]
        public void Frenzied_Escape_Sends_Doduo_To_The_Lost_Zone_When_It_Retreats()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var doduo = e.ScenarioActive(0, C(ImakunisDoduo));
            e.ScenarioAttach(doduo, C(Psychic));
            e.ScenarioAttachTool(doduo, C(ChoiceBand));
            var machop = e.ScenarioBench(0, C(Machop));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreSame(machop, me.Active);
            Assert.AreEqual(0, me.Bench.Count, "Doduo ran away");
            Assert.AreEqual(1, me.LostZone.Count(c => c.Def.Name == "Imakuni?'s Doduo"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Psychic Energy"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Choice Band"));
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "not a Knock Out");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Devolution_Spray_Returns_The_Top_Evolution_And_It_Cant_Evolve_Again_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Devolution Spray"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(Pikachu));
            e.ScenarioEvolve(raichu, C(Raichu));
            e.ScenarioAddToHand(0, C(DevolutionSpray));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Pikachu", raichu.Name);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.Name == "Raichu"));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Evolve")), "it can't evolve this turn");
        }

        [Test]
        public void Maintenance_Needs_2_Other_Cards_And_Draws_1()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioAddToHand(0, C(Maintenance));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Maintenance"), "only 1 other card");

            var e2 = Scenario(new ScriptedAgent().Then("Play Maintenance"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Machop));
            e2.ScenarioAddToHand(0, C(Maintenance));
            e2.ScenarioAddToHand(0, C(Fire));
            e2.ScenarioAddToHand(0, C(Fire));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            var me = e2.State.Players[0];
            Assert.AreEqual(1, me.Hand.Count);
            Assert.AreEqual(21, me.Deck.Count, "20 + 2 shuffled in - 1 drawn");
        }

        [Test]
        public void Professor_Oaks_Hint_Draws_Up_To_7_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Professor Oak's Hint"), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioAddToHand(0, C(ProfessorOaksHint));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "your turn ends");
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Revive_Puts_A_Basic_From_The_Discard_Pile_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Revive"), new ScriptedAgent());
            e.ScenarioActive(0, C(Machop));
            e.ScenarioAddToHand(0, C(Revive));
            e.ScenarioAddToDiscard(0, C(Gastly), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Gastly", e.State.Players[0].Bench.Single().Name);
        }
    }
}
