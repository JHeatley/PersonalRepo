using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v95: Generations (GEN 1-83 and the Radiant Collection RC1-RC32): Wobbuffet's Bide Barricade, Machamp's Fighting Fury,
    /// Mr. Mime's Bench Barrier, Evosoda and Wally (your rulings on BREAK and on Pokémon that already evolved), Floral Crown and
    /// the rest.
    /// </summary>
    public class GenTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Grass = "MEE-9";
        const string Fairy = "SUM-Y";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string Omanyte = "FCO-17";
        const string Omastar = "FCO-18";
        const string OmastarBreak = "FCO-19";
        const string FairyGarden = "FCO-100";

        const string VenusaurEX = "GEN-1";
        const string MVenusaurEX = "GEN-2";
        const string Caterpie = "GEN-3";
        const string Metapod = "GEN-4";
        const string Butterfree = "GEN-5";
        const string Parasect = "GEN-7";
        const string Pinsir = "GEN-9";
        const string LeafeonEX = "GEN-10";
        const string Gyarados = "GEN-23";
        const string Zubat = "GEN-30";
        const string Golbat = "GEN-31";
        const string Gastly = "GEN-33";
        const string Haunter = "GEN-34";
        const string Gengar = "GEN-35";
        const string Jynx = "GEN-36";
        const string MeowsticEX = "GEN-37";
        const string Machamp = "GEN-42";
        const string Hitmonchan = "GEN-48";
        const string MrMime = "GEN-52";
        const string Dodrio = "GEN-56";
        const string Snorlax = "GEN-58";
        const string Evosoda = "GEN-62";
        const string MaxRevive = "GEN-65";
        const string Olympia = "GEN-66";
        const string Revitalizer = "GEN-70";
        const string RedCard = "GEN-71";
        const string TeamFlareGrunt = "GEN-73";
        const string Charmander = "GEN-RC3";
        const string Charmeleon = "GEN-RC4";
        const string Charizard = "GEN-RC5";
        const string FlareonEX = "GEN-RC6";
        const string Wobbuffet = "GEN-RC11";
        const string SylveonEX = "GEN-RC21";
        const string Diancie = "GEN-RC22";
        const string FloralCrown = "GEN-RC26";
        const string Wally = "GEN-RC27";

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Radiant_Collection_And_Reprints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "GEN" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(92, own.Count);
            Assert.IsTrue(own.Where(d => d.Name != "Red Card").All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(RedCard)), "Red Card is on the Expanded ban list");
            Assert.AreEqual("GEN-RC6", C("GEN-RC28").BaseKey, "full art Flareon-EX");
            Assert.AreEqual("GEN-28", C("GEN-28a").BaseKey, "the second Jolteon-EX print");
            Assert.AreEqual("GEN-27", C("GEN-RC9").BaseKey, "Radiant Collection Raichu");
            Assert.AreEqual("POR-71", C("GEN-60").BaseKey, "Crushing Hammer keeps its script");
            Assert.AreEqual("STS-65", C("GEN-RC16").BaseKey, "Yveltal");
            Assert.IsTrue(C(MVenusaurEX).IsXYMegaEvolution);
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Bide_Barricade_Turns_Off_Every_Non_Psychic_Ability_While_Wobbuffet_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            var dodrio = e.ScenarioBench(0, C(Dodrio));
            var jynx = e.ScenarioBench(0, C(Jynx));
            e.ScenarioActive(1, C(Wobbuffet));
            var ownDodrio = e.ScenarioBench(1, C(Dodrio));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.AbilityBlocked(dodrio, "Retreat Aid"));
            Assert.IsTrue(e.AbilityBlocked(ownDodrio, "Retreat Aid"), "its owner's Pokémon too");
            Assert.AreEqual(4, e.RetreatCostOf(snorlax));
            Assert.IsFalse(e.AbilityBlocked(jynx, "Victory Kiss"), "Psychic Pokémon keep theirs");
            Assert.IsTrue(e.OutOfPlayAbilitiesBlocked(0, C(Dodrio)), "in the hand and discard pile");
            Assert.IsTrue(e.OutOfPlayAbilitiesBlocked(1, C(Dodrio)));
            Assert.IsFalse(e.OutOfPlayAbilitiesBlocked(0, C(Jynx)));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax2 = e2.ScenarioActive(0, C(SnorlaxGX));
            var dodrio2 = e2.ScenarioBench(0, C(Dodrio));
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioBench(1, C(Wobbuffet));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.AbilityBlocked(dodrio2, "Retreat Aid"), "only while Wobbuffet is Active");
            Assert.AreEqual(2, e2.RetreatCostOf(snorlax2));
        }

        [Test]
        public void Retreat_Aid_Stacks_From_The_Bench_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioBench(0, C(Dodrio));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(snorlax));
            e.ScenarioBench(0, C(Dodrio));
            Assert.AreEqual(0, e.RetreatCostOf(snorlax));

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dodrio = e2.ScenarioActive(0, C(Dodrio));
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e2.RetreatCostOf(dodrio), "not while Dodrio is the Active Pokémon");
        }

        [Test]
        public void Fighting_Fury_Adds_20_For_Each_Machamp()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Mach Cross"), new ScriptedAgent());
            var chan = e.ScenarioActive(0, C(Hitmonchan));
            e.ScenarioAttach(chan, C(Fighting));
            e.ScenarioAttach(chan, C(Psychic));
            e.ScenarioAttach(chan, C(Psychic));
            e.ScenarioBench(0, C(Machamp));
            e.ScenarioBench(0, C(Machamp));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, foe.DamageCounters, "60 + 20 + 20");
        }

        [Test]
        public void Bench_Barrier_Stops_Damage_To_Its_Bench_From_Any_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Berserker Splash"), new ScriptedAgent());
            var gyarados = e.ScenarioActive(0, C(Gyarados));
            e.ScenarioAttach(gyarados, C(Water));
            e.ScenarioAttach(gyarados, C(Psychic));
            e.ScenarioAttach(gyarados, C(Psychic));
            var mine = e.ScenarioBench(0, C(Bounsweet));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            var mime = e.ScenarioBench(1, C(MrMime));
            var theirs = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, foe.DamageCounters, "the Active Pokémon still takes the damage");
            Assert.AreEqual(0, mime.DamageCounters);
            Assert.AreEqual(0, theirs.DamageCounters);
            Assert.AreEqual(1, mine.DamageCounters, "the attacker's own Bench has no Mr. Mime");
        }

        [Test]
        public void Shadow_Ear_Moves_A_Damage_Counter_To_The_Opponent()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shadow Ear"), new ScriptedAgent());
            var meowstic = e.ScenarioActive(0, C(MeowsticEX));
            meowstic.DamageCounters = 2;
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, meowstic.DamageCounters);
            Assert.AreEqual(1, foe.DamageCounters);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Shadow Ear")), "once a turn");
        }

        [Test]
        public void Victory_Kiss_Heals_The_Active_Pokemon_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Victory Kiss"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            snorlax.DamageCounters = 3;
            e.ScenarioBench(0, C(Jynx));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, snorlax.DamageCounters);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var jynx = e2.ScenarioActive(0, C(Jynx));
            jynx.DamageCounters = 3;
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.StartsWith("Use Victory Kiss")), "only from the Bench");
        }

        [Test]
        public void Sneaky_Bite_And_Gothic_Fear_Trigger_When_Evolving_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("into Golbat", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Zubat));
            e.ScenarioAddToHand(0, C(Golbat));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, foe.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("into Haunter", "Yes"), new ScriptedAgent());
            var haunter = e2.ScenarioActive(0, C(Gastly));
            e2.ScenarioAddToHand(0, C(Haunter));
            var foe2 = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(foe2.Has(SpecialCondition.Confused));
            Assert.IsTrue(haunter.Has(SpecialCondition.Confused), "both Active Pokémon");
        }

        [Test]
        public void Flash_Fire_Moves_A_Fire_Energy_To_Flareon_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Flash Fire"), new ScriptedAgent());
            var flareon = e.ScenarioActive(0, C(FlareonEX));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioAttach(bench, C(Fire));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, flareon.Energy.Count);
            Assert.AreEqual(0, bench.Energy.Count);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Creep_Show_Knocks_Out_With_3_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Creep Show"), new ScriptedAgent());
            var gengar = e.ScenarioActive(0, C(Gengar));
            e.ScenarioAttach(gengar, C(Psychic));
            e.ScenarioAttach(gengar, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            foe.DamageCounters = 3;
            e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Lapras-GX"));

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Creep Show"), new ScriptedAgent());
            var gengar2 = e2.ScenarioActive(0, C(Gengar));
            e2.ScenarioAttach(gengar2, C(Psychic));
            e2.ScenarioAttach(gengar2, C(Psychic));
            var foe2 = e2.ScenarioActive(1, C(LaprasGX));
            foe2.DamageCounters = 2;
            e2.ScenarioBench(1, C(Bounsweet));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreSame(foe2, e2.State.Players[1].Active);
            Assert.AreEqual(2, foe2.DamageCounters);
        }

        [Test]
        public void Heavy_Suplex_Counts_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Suplex"), new ScriptedAgent());
            var pinsir = e.ScenarioActive(0, C(Pinsir));
            e.ScenarioAttach(pinsir, C(Grass));
            e.ScenarioAttach(pinsir, C(Psychic));
            e.ScenarioAttach(pinsir, C(Psychic));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, foe.DamageCounters, "40 + 20 for each of 4 Colorless");
        }

        [Test]
        public void Natures_Breath_Needs_A_Stadium_For_30_More_And_The_Heal()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nature's Breath"), new ScriptedAgent());
            var leafeon = e.ScenarioActive(0, C(LeafeonEX));
            leafeon.DamageCounters = 3;
            e.ScenarioAttach(leafeon, C(Grass));
            e.ScenarioAttach(leafeon, C(Grass));
            e.ScenarioAttach(leafeon, C(Psychic));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetStadium(1, C(FairyGarden));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, foe.DamageCounters);
            Assert.AreEqual(0, leafeon.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Nature's Breath"), new ScriptedAgent());
            var leafeon2 = e2.ScenarioActive(0, C(LeafeonEX));
            leafeon2.DamageCounters = 3;
            e2.ScenarioAttach(leafeon2, C(Grass));
            e2.ScenarioAttach(leafeon2, C(Grass));
            e2.ScenarioAttach(leafeon2, C(Psychic));
            var foe2 = e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(9, foe2.DamageCounters);
            Assert.AreEqual(3, leafeon2.DamageCounters);
        }

        [Test]
        public void Colorful_Spores_Attaches_A_Different_Type_To_Each_Of_3_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Colorful Spores"), new ScriptedAgent());
            var parasect = e.ScenarioActive(0, C(Parasect));
            e.ScenarioAttach(parasect, C(Psychic));
            var a = e.ScenarioBench(0, C(Bounsweet));
            var b = e.ScenarioBench(0, C(SnorlaxGX));
            e.ScenarioAddToDeck(0, C(Grass), 1);
            e.ScenarioAddToDeck(0, C(Water), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, parasect.Energy.Count);
            Assert.AreEqual(1, a.Energy.Count);
            Assert.AreEqual(1, b.Energy.Count);
            var added = new[] { parasect.Energy[1].Def.EnergyProvides, a.Energy[0].Def.EnergyProvides, b.Energy[0].Def.EnergyProvides };
            Assert.AreEqual(3, added.Distinct().Count(), "a different type for each");
            Assert.AreEqual(19, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Sleepy_Press_Heals_20_And_Sleeps_With_Two_Coins()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sleepy Press"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            snorlax.DamageCounters = 5;
            for (int i = 0; i < 4; i++) e.ScenarioAttach(snorlax, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, foe.DamageCounters);
            Assert.AreEqual(3, snorlax.DamageCounters);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Asleep));
            var script = e.Scripts.For(C(Snorlax));
            Assert.AreEqual(2, script.ModifySleepCoins(new EffectContext(e, 0, snorlax.Top, snorlax), snorlax, 1));
        }

        [Test]
        public void Recall_Uses_An_Attack_Of_A_Previous_Evolution()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Recall", "Slash"), new ScriptedAgent());
            var zard = e.ScenarioActive(0, C(Charmander));
            e.ScenarioEvolve(zard, C(Charmeleon));
            e.ScenarioEvolve(zard, C(Charizard));
            e.ScenarioAttach(zard, C(Fire));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, foe.DamageCounters);
        }

        [Test]
        public void Precious_Ribbon_Moves_A_Fairy_Energy_And_Heals_50()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Precious Ribbon"), new ScriptedAgent());
            var sylveon = e.ScenarioActive(0, C(SylveonEX));
            e.ScenarioAttach(sylveon, C(Fairy));
            e.ScenarioAttach(sylveon, C(Fairy));
            e.ScenarioAttach(sylveon, C(Psychic));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            bench.DamageCounters = 5;
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, foe.DamageCounters);
            Assert.AreEqual(1, bench.Energy.Count);
            Assert.AreEqual(0, bench.DamageCounters);
            Assert.AreEqual(2, sylveon.Energy.Count);
        }

        [Test]
        public void Diamond_Storm_Heals_Each_Fairy_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Diamond Storm"), new ScriptedAgent());
            var diancie = e.ScenarioActive(0, C(Diancie));
            diancie.DamageCounters = 2;
            e.ScenarioAttach(diancie, C(Fairy));
            e.ScenarioAttach(diancie, C(Fairy));
            e.ScenarioAttach(diancie, C(Psychic));
            var sylveon = e.ScenarioBench(0, C(SylveonEX));
            sylveon.DamageCounters = 5;
            var grass = e.ScenarioBench(0, C(Bounsweet));
            grass.DamageCounters = 5;
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, foe.DamageCounters);
            Assert.AreEqual(0, diancie.DamageCounters);
            Assert.AreEqual(2, sylveon.DamageCounters);
            Assert.AreEqual(5, grass.DamageCounters, "not a Fairy Pokémon");
        }

        // ------------------------------------------------------------------ Evosoda and Wally

        [Test]
        public void Evosoda_Evolves_From_The_Deck_But_Not_On_The_First_Turn_Or_A_New_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Evosoda", "Metapod"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var caterpie = e.ScenarioBench(0, C(Caterpie));
            e.ScenarioAddToHand(0, C(Evosoda));
            e.ScenarioAddToDeck(0, C(Metapod), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Metapod", caterpie.Name);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Caterpie));
            e2.ScenarioAddToHand(0, C(Evosoda));
            e2.ScenarioAddToDeck(0, C(Metapod), 1);
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 1);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label == "Play Evosoda"), "not during your first turn, Adaptive Evolution or not");

            var e3 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var fresh = e3.ScenarioActive(0, C(Caterpie));
            e3.ScenarioAddToHand(0, C(Evosoda));
            e3.ScenarioAddToDeck(0, C(Metapod), 1);
            e3.ScenarioActive(1, C(LaprasGX));
            e3.ScenarioSetTurn(0, 3);
            fresh.TurnEnteredPlay = 3;
            Assert.IsFalse(e3.GetMainActions(0).Any(o => o.Label == "Play Evosoda"), "not on a Pokémon put into play this turn");
            fresh.TurnEnteredPlay = 1;
            fresh.TurnEvolved = 3;
            Assert.IsFalse(e3.GetMainActions(0).Any(o => o.Label == "Play Evosoda"), "nor one that evolved this turn");
        }

        [Test]
        public void Evosoda_Can_Find_A_Break_Evolution()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Evosoda", "Omastar BREAK"), new ScriptedAgent());
            var omastar = e.ScenarioActive(0, C(Omanyte));
            e.ScenarioEvolve(omastar, C(Omastar));
            e.ScenarioAddToHand(0, C(Evosoda));
            e.ScenarioAddToDeck(0, C(OmastarBreak), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Omastar BREAK", omastar.Name);
        }

        [Test]
        public void Wally_Works_On_The_First_Turn_And_On_A_Pokemon_That_Already_Evolved()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Wally", "Metapod"), new ScriptedAgent());
            var caterpie = e.ScenarioActive(0, C(Caterpie));
            caterpie.TurnEnteredPlay = 1;
            e.ScenarioAddToHand(0, C(Wally));
            e.ScenarioAddToDeck(0, C(Metapod), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 1);
            Step(e);
            Assert.AreEqual("Metapod", caterpie.Name);

            var e2 = Scenario(new ScriptedAgent().Then("Play Wally", "Butterfree"), new ScriptedAgent());
            var metapod = e2.ScenarioActive(0, C(Caterpie));
            e2.ScenarioEvolve(metapod, C(Metapod));
            metapod.TurnEvolved = 3;
            e2.ScenarioAddToHand(0, C(Wally));
            e2.ScenarioAddToDeck(0, C(Butterfree), 1);
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual("Butterfree", metapod.Name, "your ruling: Wally allows it");
        }

        [Test]
        public void Wally_Cant_Evolve_A_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(VenusaurEX));
            e.ScenarioAddToHand(0, C(Wally));
            e.ScenarioAddToDeck(0, C(MVenusaurEX), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Wally"));
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Olympia_Switches_And_Heals_30_From_The_Pokemon_Moved_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Olympia"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            snorlax.DamageCounters = 5;
            e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(Olympia));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Bounsweet", e.State.Players[0].Active.Name);
            Assert.AreEqual(2, snorlax.DamageCounters);
        }

        [Test]
        public void Red_Card_Leaves_The_Opponent_With_4_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Red Card"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(RedCard));
            e.ScenarioActive(1, C(LaprasGX));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
            Assert.AreEqual(23, e.State.Players[1].Deck.Count, "20 + 7 - 4");
        }

        [Test]
        public void Team_Flare_Grunt_Discards_An_Energy_From_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Flare Grunt"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(TeamFlareGrunt));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttach(foe, C(Water));
            e.ScenarioAttach(foe, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, foe.Energy.Count);
        }

        [Test]
        public void Revitalizer_And_Max_Revive_Recycle_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Revitalizer"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Revitalizer));
            e.ScenarioAddToDiscard(0, C(Bounsweet), 2);
            e.ScenarioAddToDiscard(0, C(Caterpie), 1);
            e.ScenarioAddToDiscard(0, C(LaprasGX), 1);
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Grass)));

            var e2 = Scenario(new ScriptedAgent().Then("Play Max Revive"), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            e2.ScenarioAddToHand(0, C(MaxRevive));
            e2.ScenarioAddToDiscard(0, C(Bounsweet), 1);
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual("Bounsweet", e2.State.Players[0].Deck[0].Def.Name);
            Assert.IsFalse(e2.State.Players[0].Discard.Any(c => c.Def.Name == "Bounsweet"));
        }

        [Test]
        public void Floral_Crown_Heals_A_Basic_Pokemon_At_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var basic = e.ScenarioActive(0, C(Bounsweet));
            basic.DamageCounters = 3;
            e.ScenarioAttachTool(basic, C(FloralCrown));
            var evolved = e.ScenarioBench(0, C(Caterpie));
            e.ScenarioEvolve(evolved, C(Metapod));
            evolved.DamageCounters = 3;
            e.ScenarioAttachTool(evolved, C(FloralCrown));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(3, basic.DamageCounters, "not at the end of its owner's turn");
            e.ScenarioSetTurn(1, 4);
            e.EndTurn();
            Assert.AreEqual(1, basic.DamageCounters);
            Assert.AreEqual(3, evolved.DamageCounters, "only a Basic Pokémon");
        }
    }
}
