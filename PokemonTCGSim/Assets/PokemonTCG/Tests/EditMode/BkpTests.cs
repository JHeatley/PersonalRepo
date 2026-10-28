using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v96: BREAKpoint (BKP 1-123): Garbotoxin, Shadow Stitching against Wind Charm, Zap Zone, Nervous Seed, the BREAK Pokémon (their
    /// Weakness, Resistance and Retreat Cost come from the card under them), Splash Energy, Puzzle of Time and the rest.
    /// </summary>
    public class BkpTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string Grass = "MEE-9";
        const string Fairy = "SUM-Y";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string FairyGarden = "FCO-100";

        const string Meganium = "BKP-3";
        const string Lilligant = "BKP-8";
        const string Psyduck = "BKP-16";
        const string Golduck = "BKP-17";
        const string GolduckBreak = "BKP-18";
        const string Slowbro = "BKP-20";
        const string Suicune = "BKP-30";
        const string ManaphyEX = "BKP-32";
        const string Frogadier = "BKP-39";
        const string Greninja = "BKP-40";
        const string GreninjaBreak = "BKP-41";
        const string Electivire = "BKP-43";
        const string Luxray = "BKP-46";
        const string LuxrayBreak = "BKP-47";
        const string Zebstrika = "BKP-49";
        const string Garbodor = "BKP-57";
        const string Aegislash = "BKP-62";
        const string Phantump = "BKP-64";
        const string Trevenant = "BKP-65";
        const string TrevenantBreak = "BKP-66";
        const string Sudowoodo = "BKP-67";
        const string Garchomp = "BKP-70";
        const string DarkraiEX = "BKP-74";
        const string ScizorEX = "BKP-76";
        const string MScizorEX = "BKP-77";
        const string Clefairy = "BKP-81";
        const string Clefable = "BKP-82";
        const string Dragalge = "BKP-86";
        const string Rattata = "BKP-87";
        const string Raticate = "BKP-88";
        const string RaticateBreak = "BKP-89";
        const string AllNightParty = "BKP-96";
        const string BurstingBalloon = "BKP-97";
        const string Delinquent = "BKP-98";
        const string FightingFuryBelt = "BKP-99";
        const string MaxElixir = "BKP-102";
        const string ThirdEye = "BKP-108";
        const string PuzzleOfTime = "BKP-109";
        const string ReverseValley = "BKP-110";
        const string SplashEnergy = "BKP-113";

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "BKP" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(106, own.Count);
            Assert.IsTrue(own.Where(d => d.Name != "Delinquent" && d.Name != "Puzzle of Time").All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(C(Delinquent)), "on the Expanded ban list");
            Assert.AreEqual("30C-CC22", C(GreninjaBreak).BaseKey, "Greninja BREAK is the card 30th Celebration reprinted");
            Assert.AreEqual("BKP-26", C("BKP-123").BaseKey, "secret rare Gyarados-EX");
            Assert.AreEqual("BKP-98", C("BKP-98b").BaseKey);
            Assert.AreEqual("PAL-183", C("BKP-100").BaseKey, "Great Ball keeps its script");
            Assert.IsTrue(C(LuxrayBreak).IsBreak);
            Assert.IsTrue(C("BKP-27").IsXYMegaEvolution);
            Assert.IsTrue(Formats.IsExpandedBanned(C(PuzzleOfTime)), "on the Expanded ban list");
        }

        [Test]
        public void A_Break_Pokemon_Has_The_Weakness_Resistance_And_Retreat_Cost_Of_The_Card_Under_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(Greninja));
            e.ScenarioEvolve(greninja, C(GreninjaBreak));
            var luxray = e.ScenarioBench(0, C(Luxray));
            e.ScenarioEvolve(luxray, C(LuxrayBreak));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(greninja), "Greninja's");
            Assert.AreEqual(EnergyType.Grass, e.WeaknessOf(greninja));
            Assert.AreEqual(EnergyType.Metal, e.ResistanceOf(luxray));
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(luxray));
            Assert.AreEqual(1, e.RetreatCostOf(luxray));
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Garbotoxin_Needs_A_Tool_And_Spares_Only_Garbotoxin()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAttach(lapras, C(Water));
            var manaphy = e.ScenarioBench(0, C(ManaphyEX));
            var garbodor = e.ScenarioActive(1, C(Garbodor));
            e.ScenarioAttachTool(garbodor, C(FightingFuryBelt));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.AbilityBlocked(manaphy, "Aqua Tube"));
            Assert.AreEqual(2, e.RetreatCostOf(lapras));
            Assert.IsFalse(e.AbilityBlocked(garbodor, "Garbotoxin"));
            Assert.IsTrue(e.OutOfPlayAbilitiesBlocked(0, C(ManaphyEX)), "in the hand and discard pile too");
            garbodor.Tools.Clear();
            Assert.IsFalse(e.AbilityBlocked(manaphy, "Aqua Tube"));
            Assert.AreEqual(0, e.RetreatCostOf(lapras), "Aqua Tube: Water Energy, no Retreat Cost");
        }

        [Test]
        public void Shadow_Stitching_Takes_Away_The_Opponents_Abilities_Until_The_End_Of_Their_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shadow Stitching"), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(Greninja));
            e.ScenarioAttach(greninja, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            var manaphy = e.ScenarioBench(1, C(ManaphyEX));
            var mine = e.ScenarioBench(0, C(ManaphyEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, foe.DamageCounters);
            Assert.IsTrue(e.AbilityBlocked(manaphy, "Aqua Tube"));
            Assert.IsFalse(e.AbilityBlocked(mine, "Aqua Tube"), "only the opponent's");
            Assert.IsTrue(e.OutOfPlayAbilitiesBlocked(1, C(ManaphyEX)));
            Assert.IsFalse(e.OutOfPlayAbilitiesBlocked(0, C(ManaphyEX)));
        }

        [Test]
        public void Wind_Charm_Keeps_Shadow_Stitching_Off_The_Pokemon_In_Play_While_Suicune_Is_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shadow Stitching"), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(Greninja));
            e.ScenarioAttach(greninja, C(Psychic));
            var suicune = e.ScenarioActive(1, C(Suicune));
            var manaphy = e.ScenarioBench(1, C(ManaphyEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, suicune.DamageCounters, "damage isn't an effect");
            Assert.IsFalse(e.AbilityBlocked(manaphy, "Aqua Tube"));
            Assert.IsFalse(e.AbilityBlocked(suicune, "Wind Charm"));
            var later = e.ScenarioBench(1, C(ManaphyEX));
            Assert.IsTrue(e.AbilityBlocked(later, "Aqua Tube"), "a Pokémon put into play later isn't protected");
        }

        [Test]
        public void Wind_Charm_Prevents_Attack_Effects_Only_While_Suicune_Is_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sing"), new ScriptedAgent());
            var clefairy = e.ScenarioActive(0, C(Clefairy));
            e.ScenarioAttach(clefairy, C(Fairy));
            var suicune = e.ScenarioActive(1, C(Suicune));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(suicune.Has(SpecialCondition.Asleep));

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Sing"), new ScriptedAgent());
            var clefairy2 = e2.ScenarioActive(0, C(Clefairy));
            e2.ScenarioAttach(clefairy2, C(Fairy));
            var lapras = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioBench(1, C(Suicune));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsTrue(lapras.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Zap_Zone_Ignores_Effects_On_The_Defending_Pokemon()
        {
            foreach (bool zebstrika in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Knuckle Punch"), new ScriptedAgent());
                var electivire = e.ScenarioActive(0, C(Electivire));
                e.ScenarioAttach(electivire, C(Psychic));
                e.ScenarioAttach(electivire, C(Psychic));
                if (zebstrika) e.ScenarioBench(0, C(Zebstrika));
                var scizor = e.ScenarioActive(1, C(ScizorEX));
                e.ScenarioSetTurn(0, 3);
                e.AddEffect(new DamageTakenEffect("Steel Wing", 1, scizor.Uid, 3, -20));
                Step(e);
                Assert.AreEqual(zebstrika ? 3 : 1, scizor.DamageCounters);
            }
        }

        [Test]
        public void Nervous_Seed_Makes_The_Opponents_Basic_Pokemon_Pay_More()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            var meganium = e.ScenarioBench(0, C(Meganium));
            e.ScenarioActive(1, C(Trevenant));
            e.ScenarioSetTurn(0, 3);
            var collect = C(LaprasGX).Attacks[0];
            Assert.AreEqual(collect.Cost.Length + 1, e.AttackCost(lapras, collect).Count);
            var force = C(Meganium).Attacks[0];
            Assert.AreEqual(force.Cost.Length, e.AttackCost(meganium, force).Count, "not an Evolution Pokémon");
        }

        [Test]
        public void Overgrow_Adds_70_With_50_HP_Or_Less_And_Green_Force_Heals_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Green Force"), new ScriptedAgent());
            var meganium = e.ScenarioActive(0, C(Meganium));
            meganium.DamageCounters = 10;
            e.ScenarioAttach(meganium, C(Grass));
            e.ScenarioAttach(meganium, C(Grass));
            e.ScenarioAttach(meganium, C(Psychic));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, foe.DamageCounters);
            Assert.AreEqual(0, meganium.DamageCounters);
        }

        [Test]
        public void Hyper_Transfer_Can_Be_Used_Again_And_Again()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Hyper Transfer", "Water Energy", "Use Hyper Transfer", "Golduck BREAK"), new ScriptedAgent());
            var golduck = e.ScenarioActive(0, C(Psyduck));
            e.ScenarioEvolve(golduck, C(Golduck));
            e.ScenarioEvolve(golduck, C(GolduckBreak));
            e.ScenarioAttach(golduck, C(Water));
            e.ScenarioAttach(golduck, C(Water));
            var lapras = e.ScenarioBench(0, C(LaprasGX));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(2, lapras.Energy.Count);
            Assert.AreEqual(0, golduck.Energy.Count);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Boo_Hoo_Puts_The_Defending_Pokemon_To_Sleep_When_Energy_Is_Attached_To_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Boo-Hoo"), new ScriptedAgent());
            var lilligant = e.ScenarioActive(0, C(Lilligant));
            e.ScenarioAttach(lilligant, C(Grass));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(foe.Has(SpecialCondition.Asleep));
            e.ScenarioSetTurn(1, 4);
            var card = e.ScenarioAddToHand(1, C(Water));
            e.State.Players[1].Hand.Remove(card);
            e.AttachEnergyCard(card, foe, true);
            Assert.IsTrue(foe.Has(SpecialCondition.Asleep));
        }

        [Test]
        public void Walk_Off_Homer_Wins_With_1_Prize_Card_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Walk-Off Homer"), new ScriptedAgent());
            var slowbro = e.ScenarioActive(0, C(Slowbro));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(slowbro, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.State.Players[0].Prizes.RemoveRange(0, 5);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.IsOver);
            Assert.AreEqual(0, e.State.Winner);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Walk-Off Homer"), new ScriptedAgent());
            var slowbro2 = e2.ScenarioActive(0, C(Slowbro));
            for (int i = 0; i < 3; i++) e2.ScenarioAttach(slowbro2, C(Psychic));
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsFalse(e2.State.IsOver, "6 Prize cards left");
        }

        [Test]
        public void Watch_And_Learn_Copies_The_Attack_The_Opponent_Used_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Watch and Learn"), new ScriptedAgent());
            var sudowoodo = e.ScenarioActive(0, C(Sudowoodo));
            e.ScenarioAttach(sudowoodo, C(Fighting));
            e.ScenarioAttach(sudowoodo, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            var opp = e.State.Players[1];
            opp.LastAttackTurn = 2;
            opp.LastAttackSourceDef = C(Electivire);
            opp.LastAttackIndex = 1;
            Step(e);
            Assert.AreEqual(9, foe.DamageCounters, "Electroslug's 90");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Watch and Learn"), new ScriptedAgent());
            var sudowoodo2 = e2.ScenarioActive(0, C(Sudowoodo));
            e2.ScenarioAttach(sudowoodo2, C(Fighting));
            e2.ScenarioAttach(sudowoodo2, C(Psychic));
            var foe2 = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, foe2.DamageCounters, "nothing to copy");
        }

        [Test]
        public void Painful_Sword_Doubles_The_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Painful Sword"), new ScriptedAgent());
            var aegislash = e.ScenarioActive(0, C(Aegislash));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(aegislash, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            foe.DamageCounters = 3;
            var bench = e.ScenarioBench(1, C(Bounsweet));
            bench.DamageCounters = 2;
            var clean = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, foe.DamageCounters);
            Assert.AreEqual(4, bench.DamageCounters);
            Assert.AreEqual(0, clean.DamageCounters);
        }

        [Test]
        public void Break_Attacks_Silent_Fear_And_Super_Fang()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Silent Fear"), new ScriptedAgent());
            var trevenant = e.ScenarioActive(0, C(Phantump));
            e.ScenarioEvolve(trevenant, C(Trevenant));
            e.ScenarioEvolve(trevenant, C(TrevenantBreak));
            e.ScenarioAttach(trevenant, C(Psychic));
            e.ScenarioAttach(trevenant, C(Psychic));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            var bench = e.ScenarioBench(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, foe.DamageCounters);
            Assert.AreEqual(3, bench.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Super Fang"), new ScriptedAgent());
            var raticate = e2.ScenarioActive(0, C(Rattata));
            e2.ScenarioEvolve(raticate, C(Raticate));
            e2.ScenarioEvolve(raticate, C(RaticateBreak));
            e2.ScenarioAttach(raticate, C(Psychic));
            e2.ScenarioAttach(raticate, C(Psychic));
            var foe2 = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(10, e2.RemainingHp(foe2));
        }

        [Test]
        public void Antibodies_Keeps_Raticate_And_Raticate_Break_Free_Of_Special_Conditions()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var raticate = e.ScenarioActive(0, C(Rattata));
            e.ScenarioEvolve(raticate, C(Raticate));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.ApplyCondition(raticate, SpecialCondition.Poisoned));
            e.ScenarioEvolve(raticate, C(RaticateBreak));
            Assert.IsFalse(e.ApplyCondition(raticate, SpecialCondition.Asleep), "the BREAK keeps the Ability");
        }

        [Test]
        public void Bite_Off_And_Gale_Thrust_And_Iron_Crusher()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bite Off"), new ScriptedAgent());
            var garchomp = e.ScenarioActive(0, C(Garchomp));
            e.ScenarioAttach(garchomp, C(Fighting));
            e.ScenarioAttach(garchomp, C(Fighting));
            var scizor = e.ScenarioActive(1, C(ScizorEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, scizor.DamageCounters, "80 more against a Pokémon-EX");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Gale Thrust"), new ScriptedAgent());
            var scizor2 = e2.ScenarioActive(0, C(ScizorEX));
            e2.ScenarioAttach(scizor2, C(Metal));
            e2.ScenarioAttach(scizor2, C(Metal));
            scizor2.MovedToActiveOnTurn = 3;
            var foe2 = e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(11, foe2.DamageCounters);

            var e3 = Scenario(new ScriptedAgent().Then("Attack: Iron Crusher", "Discard Fairy Garden"), new ScriptedAgent());
            var mega = e3.ScenarioActive(0, C(ScizorEX));
            e3.ScenarioEvolve(mega, C(MScizorEX));
            e3.ScenarioAttach(mega, C(Metal));
            e3.ScenarioAttach(mega, C(Metal));
            var foe3 = e3.ScenarioActive(1, C(SnorlaxGX));
            e3.ScenarioSetStadium(1, C(FairyGarden));
            e3.ScenarioSetTurn(0, 3);
            Step(e3);
            Assert.AreEqual(12, foe3.DamageCounters);
            Assert.IsNull(e3.State.Stadium);
        }

        [Test]
        public void Moon_Barrier_Stops_Dragon_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Moon Barrier"), new ScriptedAgent());
            var clefable = e.ScenarioActive(0, C(Clefable));
            e.ScenarioAttach(clefable, C(Fairy));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            var dragalge = e.ScenarioBench(1, C(Dragalge));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, foe.DamageCounters);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(dragalge, clefable, 60);
            Assert.AreEqual(0, clefable.DamageCounters);
            e.DealAttackDamage(foe, clefable, 30);
            Assert.AreEqual(3, clefable.DamageCounters);
        }

        [Test]
        public void Water_Duplicates_Benches_Frogadier_From_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Water Duplicates", "Frogadier|Frogadier|Frogadier"), new ScriptedAgent());
            var frogadier = e.ScenarioActive(0, C(Frogadier));
            e.ScenarioAttach(frogadier, C(Water));
            e.ScenarioAddToDeck(0, C(Frogadier), 3);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Bench.Count(m => m.Name == "Frogadier"));
        }

        // ------------------------------------------------------------------ Trainers and Energy

        [Test]
        public void Splash_Energy_Returns_The_Knocked_Out_Water_Pokemon_To_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var golduck = e.ScenarioActive(0, C(Psyduck));
            e.ScenarioEvolve(golduck, C(Golduck));
            e.ScenarioAttach(golduck, C(SplashEnergy));
            e.ScenarioAttach(golduck, C(Water));
            e.ScenarioBench(0, C(Bounsweet));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, golduck, 300);
            e.ResolveKnockouts();
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count(c => c.Def.Name == "Psyduck" || c.Def.Name == "Golduck"), "the whole Pokémon");
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Splash Energy"));
            Assert.AreEqual(1, me.Discard.Count(c => c.Def.Name == "Water Energy"));
            var holder = e.ScenarioBench(0, C(Bounsweet));
            Assert.IsFalse(e.Scripts.For(C(SplashEnergy)).CanAttachTo(new EffectContext(e, 0, null, holder), holder), "only on a Water Pokémon");
        }

        [Test]
        public void Fighting_Fury_Belt_Gives_A_Basic_Pokemon_40_HP_And_10_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var basic = e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAttachTool(basic, C(FightingFuryBelt));
            var evolved = e.ScenarioBench(0, C(Psyduck));
            e.ScenarioEvolve(evolved, C(Golduck));
            e.ScenarioAttachTool(evolved, C(FightingFuryBelt));
            var foe = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(100, e.MaxHp(basic));
            Assert.AreEqual(100, e.MaxHp(evolved), "only a Basic Pokémon");
            e.DealAttackDamage(basic, foe, 10);
            Assert.AreEqual(2, foe.DamageCounters);
        }

        [Test]
        public void Bursting_Balloon_Hits_Back_Then_Goes_At_The_End_Of_The_Opponents_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var holder = e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAttachTool(holder, C(BurstingBalloon));
            var foe = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, holder, 10);
            Assert.AreEqual(6, foe.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(0, holder.Tools.Count);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Bursting Balloon"));
        }

        [Test]
        public void Delinquent_Discards_The_Stadium_And_3_Cards_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Delinquent"), new ScriptedAgent());
            e.ScenarioActive(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(Delinquent));
            e.ScenarioActive(1, C(LaprasGX));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetStadium(1, C(FairyGarden));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(2, e.State.Players[1].Hand.Count);

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Bounsweet));
            e2.ScenarioAddToHand(0, C(Delinquent));
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label == "Play Delinquent"), "needs a Stadium in play");
        }

        [Test]
        public void Max_Elixir_Attaches_A_Basic_Energy_From_The_Top_6_To_A_Benched_Basic()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Max Elixir"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioAddToHand(0, C(MaxElixir));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, bench.Energy.Count);
            Assert.AreEqual(19, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Two_Puzzle_Of_Time_Cards_Put_2_Cards_From_The_Discard_Pile_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Puzzle of Time", "Yes", "Bounsweet|Lapras-GX"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(PuzzleOfTime));
            e.ScenarioAddToHand(0, C(PuzzleOfTime));
            e.ScenarioAddToDiscard(0, C(Bounsweet), 1);
            e.ScenarioAddToDiscard(0, C(LaprasGX), 1);
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(2, me.Hand.Count);
            Assert.IsTrue(me.Hand.Any(c => c.Def.Name == "Bounsweet") && me.Hand.Any(c => c.Def.Name == "Lapras-GX"));
            Assert.AreEqual(2, me.Discard.Count(c => c.Def.Name == "Puzzle of Time"));
        }

        [Test]
        public void Reverse_Valley_Faces_One_Way()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Reverse Valley", "Your Metal Pokémon take 10 less"), new ScriptedAgent());
            var scizor = e.ScenarioActive(0, C(ScizorEX));
            e.ScenarioAddToHand(0, C(ReverseValley));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var darkrai = e.ScenarioBench(1, C(DarkraiEX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Reverse Valley", e.State.Stadium.Def.Name);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(lapras, scizor, 30);
            Assert.AreEqual(2, scizor.DamageCounters, "Metal: 10 less");
            e.DealAttackDamage(darkrai, scizor, 30);
            Assert.AreEqual(5, scizor.DamageCounters, "the opponent's Darkness: 10 more, then 10 less");
        }

        [Test]
        public void Psychics_Third_Eye_Trades_Cards_And_All_Night_Party_Wakes_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Psychic's Third Eye", "Water Energy|Water Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAddToHand(0, C(ThirdEye));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Water Energy"));

            var e2 = Scenario(new ScriptedAgent().Then("Use All-Night Party"), new ScriptedAgent());
            var snorlax = e2.ScenarioActive(0, C(SnorlaxGX));
            snorlax.DamageCounters = 4;
            e2.ApplyCondition(snorlax, SpecialCondition.Asleep);
            e2.ScenarioSetStadium(1, C(AllNightParty));
            e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.IsFalse(snorlax.Has(SpecialCondition.Asleep));
            Assert.AreEqual(1, snorlax.DamageCounters);
        }
    }
}
