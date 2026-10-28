using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v97: BREAKthrough (BKT 1-164): Dignified Fighter, Shining Body, Thick Fat, Plump Body, Calming Aroma, Fright Night, Dragon Dance,
    /// Damage Change, Parallel City, Burning Energy, the Tools and the rest.
    /// </summary>
    public class BktTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Fairy = "SUM-Y";
        const string LaprasGX = "SUM-35";
        const string SnorlaxGX = "SMP-5";
        const string Bounsweet = "SUM-18";
        const string Psyduck = "BKP-16";
        const string Golduck = "BKP-17";
        const string SplashEnergy = "BKP-113";
        const string FairyGarden = "FCO-100";

        const string ChesnaughtBreak = "BKT-12";
        const string HoundoomEX = "BKT-21";
        const string MHoundoomEX = "BKT-22";
        const string Octillery = "BKT-33";
        const string Empoleon = "BKT-38";
        const string MagnemiteInduction = "BKT-51";
        const string Magnemite = "BKT-52";
        const string Raikou = "BKT-55";
        const string MewtwoEXPhoton = "BKT-61";
        const string MewtwoEX = "BKT-62";
        const string MMewtwoEXInfinity = "BKT-64";
        const string Misdreavus = "BKT-65";
        const string Mismagius = "BKT-66";
        const string Cresselia = "BKT-70";
        const string Swinub = "BKT-80";
        const string Piloswine = "BKT-81";
        const string Mamoswine = "BKT-82";
        const string Gallade = "BKT-84";
        const string Yveltal = "BKT-94";
        const string Florges = "BKT-103";
        const string FlorgesBreak = "BKT-104";
        const string Spritzee = "BKT-105";
        const string Xerneas = "BKT-107";
        const string Haxorus = "BKT-111";
        const string Snorlax = "BKT-118";
        const string Hoothoot = "BKT-119";
        const string Noctowl = "BKT-120";
        const string Staraptor = "BKT-127";
        const string AssaultVest = "BKT-133";
        const string Brigette = "BKT-134";
        const string BuddyBuddyRescue = "BKT-135";
        const string FloatStone = "BKT-137";
        const string GiovannisScheme = "BKT-138";
        const string HeavyBall = "BKT-140";
        const string HeavyBoots = "BKT-141";
        const string ParallelCity = "BKT-145";
        const string ProfessorsLetter = "BKT-146";
        const string TownMap = "BKT-150";
        const string BurningEnergy = "BKT-151";

        // ------------------------------------------------------------------ Data

        [Test]
        public void The_Set_Is_In_The_Pool_With_Its_Prints_Linked()
        {
            var own = db.Cards.Where(d => d.SetCode == "BKT" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(136, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("GEN-6", C("BKT-1").BaseKey, "Paras is the Generations card");
            Assert.AreEqual("BKT-21", C("BKT-153").BaseKey, "full art Houndoom-EX");
            Assert.AreEqual("BKT-134", C("BKT-161").BaseKey, "full art Brigette");
            Assert.AreEqual("BKT-146", C("BKT-146a").BaseKey, "the second Professor's Letter print");
            Assert.AreEqual("PAL-188", C("BKT-149").BaseKey, "Super Rod keeps its script");
            Assert.AreEqual("CES-151", C("BKT-152").BaseKey, "Rainbow Energy keeps its script");
            Assert.IsTrue(C(ChesnaughtBreak).IsBreak);
            Assert.IsTrue(C(MHoundoomEX).IsXYMegaEvolution);
        }

        // ------------------------------------------------------------------ Abilities

        [Test]
        public void Dignified_Fighter_Adds_20_To_Basic_Pokemon_Attacking_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            var golduck = e.ScenarioBench(0, C(Psyduck));
            e.ScenarioEvolve(golduck, C(Golduck));
            e.ScenarioBench(0, C(Empoleon));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            var benched = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(lapras, snorlax, 30);
            Assert.AreEqual(5, snorlax.DamageCounters, "Basic: 20 more");
            e.DealAttackDamage(golduck, snorlax, 30);
            Assert.AreEqual(8, snorlax.DamageCounters, "an Evolution Pokémon gets nothing");
            e.DealAttackDamage(lapras, benched, 30);
            Assert.AreEqual(3, benched.DamageCounters, "only to the Active Pokémon");
        }

        [Test]
        public void Shining_Body_Needs_Lightning_Energy_And_Plump_Body_And_Thick_Fat_Take_30_Off()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var raikou = e.ScenarioActive(0, C(Raikou));
            var snorlax = e.ScenarioBench(0, C(Snorlax));
            var mamoswine = e.ScenarioBench(0, C(Mamoswine));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var bounsweet = e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(lapras, raikou, 60);
            Assert.AreEqual(6, raikou.DamageCounters, "no Lightning Energy");
            e.ScenarioAttach(raikou, C(Lightning));
            e.DealAttackDamage(lapras, raikou, 60);
            Assert.AreEqual(10, raikou.DamageCounters, "20 less with Lightning Energy");
            e.DealAttackDamage(lapras, snorlax, 60);
            Assert.AreEqual(3, snorlax.DamageCounters, "Plump Body");
            e.DealAttackDamage(lapras, mamoswine, 60);
            Assert.AreEqual(3, mamoswine.DamageCounters, "Thick Fat against a Water Pokémon");
            e.DealAttackDamage(bounsweet, mamoswine, 60);
            Assert.AreEqual(9, mamoswine.DamageCounters, "a Grass Pokémon gets no reduction");
        }

        [Test]
        public void Sparkling_Induction_My_Way_And_Float_Stone_Cut_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var magnemite = e.ScenarioActive(0, C(MagnemiteInduction));
            e.ScenarioActive(1, C(LaprasGX));
            var cresselia = e.ScenarioBench(1, C(Cresselia));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.RetreatCostOf(magnemite));
            e.ScenarioBench(0, C(Magnemite));
            Assert.AreEqual(0, e.RetreatCostOf(magnemite), "1 less for each Benched Magnemite");
            Assert.AreEqual(2, e.RetreatCostOf(cresselia));
            e.ScenarioSetStadium(0, C(FairyGarden));
            Assert.AreEqual(0, e.RetreatCostOf(cresselia), "My Way: any Stadium in play");
            var snorlax = e.ScenarioBench(0, C(Snorlax));
            e.ScenarioAttachTool(snorlax, C(FloatStone));
            Assert.AreEqual(0, e.RetreatCostOf(snorlax));
        }

        [Test]
        public void Calming_Aroma_Takes_A_Fairy_Off_Each_Of_Your_Attacks_For_Each_Florges()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var florges = e.ScenarioActive(0, C(Florges));
            var spritzee = e.ScenarioBench(0, C(Spritzee));
            var foeFlorges = e.ScenarioActive(1, C(Florges));
            e.ScenarioSetTurn(0, 3);
            var shine = C(Florges).Attacks[0];
            Assert.AreEqual(shine.Cost.Length - 1, e.AttackCost(florges, shine).Count);
            Assert.AreEqual(C(Spritzee).Attacks[0].Cost.Length, e.AttackCost(spritzee, C(Spritzee).Attacks[0]).Count, "no Fairy in the cost");
            e.ScenarioBench(0, C(Florges));
            Assert.AreEqual(shine.Cost.Length - 2, e.AttackCost(florges, shine).Count, "each Florges takes off one");
            Assert.AreEqual(shine.Cost.Length - 1, e.AttackCost(foeFlorges, shine).Count, "only its owner's Pokémon");
        }

        [Test]
        public void Floral_Breeze_Heals_30_And_Removes_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Floral Breeze"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            lapras.DamageCounters = 5;
            e.ApplyCondition(lapras, SpecialCondition.Poisoned);
            var florges = e.ScenarioBench(0, C(Florges));
            e.ScenarioEvolve(florges, C(FlorgesBreak));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, lapras.DamageCounters);
            Assert.IsFalse(lapras.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Abyssal_Hand_Draws_Up_To_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Abyssal Hand"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioBench(0, C(Octillery));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual(16, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Twisted_Incantation_Resets_The_Opponents_Hand_To_Their_Prize_Count()
        {
            var e = Scenario(new ScriptedAgent().Then("into Mismagius", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioBench(0, C(Misdreavus));
            e.ScenarioAddToHand(0, C(Mismagius));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioAddToHand(1, C(Water));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(6, opp.Hand.Count, "a card for each of their 6 Prize cards");
            Assert.AreEqual(16, opp.Deck.Count);
        }

        [Test]
        public void Fright_Night_Switches_Off_Every_Tool_While_Yveltal_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var yveltal = e.ScenarioActive(0, C(Yveltal));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C(FloatStone));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(lapras), "Float Stone has no effect");
            e.SwitchActive(0, bench);
            Assert.AreEqual(0, e.RetreatCostOf(lapras), "Yveltal on the Bench");
            Assert.IsTrue(e.State.Players[0].Bench.Contains(yveltal));
        }

        [Test]
        public void Fright_Night_Does_Not_Stop_The_End_Of_Turn_Tool_Discards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Yveltal));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            e.ScenarioAttachTool(bench, C("PAR-179"));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttachTool(lapras, C("BKP-97"));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(0, bench.Tools.Count, "a Technical Machine goes at the end of your turn (your ruling)");
            Assert.AreEqual(0, lapras.Tools.Count, "Bursting Balloon goes at the end of its owner's opponent's turn (2016)");
        }

        [Test]
        public void Gallade_Sensitive_Blade_Needs_A_Supporter_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sensitive Blade"), new ScriptedAgent());
            var gallade = e.ScenarioActive(0, C(Gallade));
            e.ScenarioAttach(gallade, C(Psychic));
            e.ScenarioAttach(gallade, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].SupporterPlayedThisTurn = true;
            Step(e);
            Assert.AreEqual(13, lapras.DamageCounters);
        }

        // ------------------------------------------------------------------ Attacks

        [Test]
        public void Damage_Change_Switches_The_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Damage Change"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoEX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mewtwo, C(Psychic));
            mewtwo.DamageCounters = 5;
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            lapras.DamageCounters = 2;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, mewtwo.DamageCounters);
            Assert.AreEqual(5, lapras.DamageCounters);
        }

        [Test]
        public void Psychic_Infinity_Counts_The_Energy_On_Both_Active_Pokemon_Without_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psychic Infinity"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoEX));
            e.ScenarioEvolve(mewtwo, C(MMewtwoEXInfinity));
            e.ScenarioAttach(mewtwo, C(Psychic));
            e.ScenarioAttach(mewtwo, C(Psychic));
            var foe = e.ScenarioActive(1, C(MewtwoEXPhoton));
            e.ScenarioAttach(foe, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, foe.DamageCounters, "10 + 30 x 3, not doubled by its Psychic Weakness");
        }

        [Test]
        public void Gathering_Footsteps_Counts_The_Current_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gathering Footsteps"), new ScriptedAgent());
            var piloswine = e.ScenarioActive(0, C(Swinub));
            e.ScenarioEvolve(piloswine, C(Piloswine));
            e.ScenarioAttach(piloswine, C(Fighting));
            e.ScenarioAttach(piloswine, C(Psychic));
            e.ScenarioAttach(piloswine, C(Psychic));
            e.ScenarioBench(0, C(Swinub));
            var mamoswine = e.ScenarioBench(0, C(Mamoswine));
            e.ScenarioAttachTool(mamoswine, C(FloatStone));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, lapras.DamageCounters, "30 + 10 x (2 + 1 + 0): Float Stone counts (2015-11-19)");
        }

        [Test]
        public void Primordial_Boom_Hits_Harder_With_Your_Stadium_And_Heals_With_Theirs()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Primordial Boom"), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C(Mamoswine));
            e.ScenarioAttach(mamoswine, C(Fighting));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(mamoswine, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetStadium(0, C(FairyGarden));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, lapras.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Primordial Boom"), new ScriptedAgent());
            var mamoswine2 = e2.ScenarioActive(0, C(Mamoswine));
            e2.ScenarioAttach(mamoswine2, C(Fighting));
            for (int i = 0; i < 3; i++) e2.ScenarioAttach(mamoswine2, C(Psychic));
            mamoswine2.DamageCounters = 6;
            var lapras2 = e2.ScenarioActive(1, C(LaprasGX));
            e2.ScenarioSetStadium(1, C(FairyGarden));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(8, lapras2.DamageCounters);
            Assert.AreEqual(2, mamoswine2.DamageCounters, "healed 40");
        }

        [Test]
        public void Dragon_Dance_Adds_100_Until_Haxorus_Leaves_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dragon Dance"), new ScriptedAgent());
            var haxorus = e.ScenarioActive(0, C(Haxorus));
            e.ScenarioAttach(haxorus, C(Psychic));
            var bench = e.ScenarioBench(0, C(Bounsweet));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.ActiveEffects().Any(fx => fx.Name.Contains("Dragon Dance")));
            e.ScenarioSetTurn(0, 5);
            e.DealAttackDamage(haxorus, lapras, 60);
            Assert.AreEqual(16, lapras.DamageCounters, "60 + 100");
            e.SwitchActive(0, bench);
            Assert.IsFalse(e.ActiveEffects().Any(fx => fx.Name.Contains("Dragon Dance")), "it ends when Haxorus moves to the Bench");
        }

        [Test]
        public void Proclaim_The_Night_And_High_Flight()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Proclaim the Night"), new ScriptedAgent());
            var hoothoot = e.ScenarioActive(0, C(Hoothoot));
            e.ScenarioAttach(hoothoot, C(Psychic));
            e.ScenarioAttach(hoothoot, C(Psychic));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[1].ItemLockTurn);

            var e2 = Scenario(new ScriptedAgent().Then("Attack: High Flight"), new ScriptedAgent());
            var noctowl = e2.ScenarioActive(0, C(Noctowl));
            e2.ScenarioAttach(noctowl, C(Psychic));
            e2.ScenarioAttach(noctowl, C(Psychic));
            e2.ScenarioAddToHand(0, C(TownMap));
            e2.ScenarioAddToHand(0, C(HeavyBall));
            var snorlax = e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioAddToHand(1, C(ProfessorsLetter));
            e2.ScenarioAddToHand(1, C(FloatStone));
            e2.ScenarioAddToHand(1, C(Water));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(6, snorlax.DamageCounters, "3 Item cards; a Pokémon Tool isn't an Item card");
        }

        [Test]
        public void Rainbow_Force_Counts_The_Types_On_Your_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rainbow Force"), new ScriptedAgent());
            var xerneas = e.ScenarioActive(0, C(Xerneas));
            e.ScenarioAttach(xerneas, C(Fairy));
            e.ScenarioAttach(xerneas, C(Psychic));
            e.ScenarioAttach(xerneas, C(Psychic));
            e.ScenarioBench(0, C(MewtwoEX));
            e.ScenarioBench(0, C(LaprasGX));
            e.ScenarioBench(0, C(LaprasGX));
            e.ScenarioBench(0, C(Snorlax));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, snorlax.DamageCounters, "Psychic, Water and Colorless: 10 + 90");
        }

        [Test]
        public void Cyclone_Slash_Switches_Before_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cyclone Slash"), new ScriptedAgent());
            var staraptor = e.ScenarioActive(0, C(Staraptor));
            e.ScenarioAttach(staraptor, C(Psychic));
            e.ScenarioAttach(staraptor, C(Psychic));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            var snorlax = e.ScenarioBench(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(snorlax, e.State.Players[1].Active);
            Assert.AreEqual(7, snorlax.DamageCounters);
            Assert.AreEqual(0, lapras.DamageCounters);
        }

        [Test]
        public void Burning_Energy_Comes_Back_After_Inferno_Fang_Discards_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Inferno Fang", "Yes"), new ScriptedAgent());
            var houndoom = e.ScenarioActive(0, C(HoundoomEX));
            e.ScenarioEvolve(houndoom, C(MHoundoomEX));
            e.ScenarioAttach(houndoom, C(BurningEnergy));
            e.ScenarioAttach(houndoom, C(Fire));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, snorlax.DamageCounters, "80 + 80");
            Assert.AreEqual(1, houndoom.Energy.Count);
            Assert.AreEqual("Burning Energy", houndoom.Energy[0].Def.Name);
            Assert.AreEqual(1, e.State.Players[0].Discard.Count(c => c.Def.Name == "Fire Energy"));
            var bounsweet = e.ScenarioBench(0, C(Bounsweet));
            Assert.IsFalse(e.Scripts.For(C(BurningEnergy)).CanAttachTo(new EffectContext(e, 0, null, bounsweet), bounsweet), "only on a Fire Pokémon");
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Assault_Vest_Stops_40_From_Pokemon_With_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var holder = e.ScenarioActive(0, C(SnorlaxGX));
            e.ScenarioAttachTool(holder, C(AssaultVest));
            var special = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAttach(special, C(SplashEnergy));
            var plain = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(special, holder, 60);
            Assert.AreEqual(2, holder.DamageCounters);
            e.DealAttackDamage(plain, holder, 60);
            Assert.AreEqual(8, holder.DamageCounters, "no Special Energy, no reduction");
        }

        [Test]
        public void Heavy_Boots_Needs_A_Retreat_Cost_Of_3_Or_More()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            e.ScenarioAttachTool(snorlax, C(HeavyBoots));
            var magnemite = e.ScenarioBench(0, C(Magnemite));
            e.ScenarioAttachTool(magnemite, C(HeavyBoots));
            e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(140, e.MaxHp(snorlax));
            Assert.AreEqual(60, e.MaxHp(magnemite));
            e.ApplyCondition(snorlax, SpecialCondition.Confused);
            Assert.IsFalse(snorlax.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Brigette_Benches_Up_To_3_Basic_Pokemon_Or_1_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Brigette", "Up to 3", "Bounsweet|Bounsweet|Bounsweet"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(Brigette));
            e.ScenarioAddToDeck(0, C(Bounsweet), 3);
            e.ScenarioAddToDeck(0, C(HoundoomEX), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Bench.Count(m => m.Name == "Bounsweet"));

            var e2 = Scenario(new ScriptedAgent().Then("Play Brigette", "1 Basic Pokémon-EX", "Houndoom-EX"), new ScriptedAgent());
            e2.ScenarioActive(0, C(LaprasGX));
            e2.ScenarioAddToHand(0, C(Brigette));
            e2.ScenarioAddToDeck(0, C(Bounsweet), 3);
            e2.ScenarioAddToDeck(0, C(HoundoomEX), 1);
            e2.ScenarioActive(1, C(SnorlaxGX));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(1, e2.State.Players[0].Bench.Count);
            Assert.AreEqual("Houndoom-EX", e2.State.Players[0].Bench[0].Name);
        }

        [Test]
        public void Buddy_Buddy_Rescue_Gives_Each_Player_A_Pokemon_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Buddy-Buddy Rescue"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(BuddyBuddyRescue));
            e.ScenarioAddToDiscard(0, C(Bounsweet), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioAddToDiscard(1, C(LaprasGX), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Bounsweet"));
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Lapras-GX"));
        }

        [Test]
        public void Giovannis_Scheme_Adds_20_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Giovanni's Scheme", "20 more damage"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(GiovannisScheme));
            var snorlax = e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.DealAttackDamage(lapras, snorlax, 30);
            Assert.AreEqual(5, snorlax.DamageCounters);
        }

        [Test]
        public void Heavy_Ball_Finds_A_Heavy_Pokemon_But_Not_A_Break_Card()
        {
            var agent = new ScriptedAgent().Then("Play Heavy Ball", "Snorlax-GX");
            var e = Scenario(agent, new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(HeavyBall));
            e.ScenarioAddToDeck(0, C(SnorlaxGX), 1);
            e.ScenarioAddToDeck(0, C(ChesnaughtBreak), 1);
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Snorlax-GX"));
            Assert.IsFalse(agent.Seen.Any(r => r.Options.Any(o => o.Label.Contains("Chesnaught BREAK"))), "a BREAK card has no Retreat Cost");
        }

        [Test]
        public void Parallel_City_Limits_One_Bench_And_Weakens_The_Other_Sides_Grass_Fire_And_Water()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Parallel City", "Your opponent can't have more than 3"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(ParallelCity));
            var theirs = e.ScenarioActive(1, C(LaprasGX));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Bounsweet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Parallel City", e.State.Stadium.Def.Name);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
            Assert.AreEqual(2, e.State.Players[1].Discard.Count(c => c.Def.Name == "Bounsweet"));
            Assert.AreEqual(3, e.MaxBench(1));
            Assert.AreEqual(5, e.MaxBench(0));
            Assert.AreEqual(1, PlayerView.Create(e, 0).StadiumFacing, "the board turns it towards the opponent");
            e.DealAttackDamage(mine, theirs, 60);
            Assert.AreEqual(4, theirs.DamageCounters, "my Water Pokémon do 20 less");
            e.DealAttackDamage(theirs, mine, 60);
            Assert.AreEqual(6, mine.DamageCounters, "the limited side's Pokémon do full damage");
        }

        [Test]
        public void Two_Way_Stadiums_Face_The_Way_They_Were_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Chaos Tower", "Your Pokémon can't be Confused or Poisoned"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C("FCO-94"));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, PlayerView.Create(e, 0).StadiumFacing, "Chaos Tower's upright half (Asleep or Paralyzed) points at the opponent");
        }

        [Test]
        public void Town_Map_Turns_The_Prize_Cards_Face_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Town Map"), new ScriptedAgent());
            e.ScenarioActive(0, C(LaprasGX));
            e.ScenarioAddToHand(0, C(TownMap));
            e.ScenarioActive(1, C(SnorlaxGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].FaceUpPrizeIds.Count);
            Assert.AreEqual(0, e.State.Players[1].FaceUpPrizeIds.Count);
        }
    }
}
