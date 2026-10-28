using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v91: SM Black Star Promos (SMP SM01-SM248).</summary>
    public class SmpTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";
        const string DoubleColorless = "SLG-69";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string LaprasGX = "SUM-35";
        const string Bounsweet = "SUM-18";
        const string Steenee = "SUM-19";
        const string Tirtouga = "UNM-44";
        const string ChoiceBand = "BUS-162";
        const string PokeBall = "POR-80";
        const string SnorlaxGX = "SMP-5";
        const string Lurantis = "SMP-25";
        const string TapuKoko = "SMP-30";
        const string BewearGX = "SMP-34";
        const string TsareenaGX = "SMP-56";
        const string TapuBulu = "SMP-61";
        const string Silvally = "SMP-64";
        const string ShiningHoOh = "SMP-70";
        const string PressureMewtwo = "SMP-77";
        const string ChampionsFestival = "SMP-78";
        const string ShiningCelebi = "SMP-79";
        const string Latias = "SMP-87";
        const string TapuFini = "SMP-92";
        const string Marshadow = "SMP-93";
        const string SolgaleoGX = "SMP-104";
        const string DawnWings = "SMP-106";
        const string DuskMane = "SMP-107";
        const string RaikouGX = "SMP-121";
        const string ZygardeGX = "SMP-122";
        const string Latios = "SMP-136";
        const string ZekromGX = "SMP-138";
        const string Pikachu = "SMP-157";
        const string Deoxys = "SMP-164";
        const string UltraNecrozma = "SMP-165";
        const string FlareonGX = "SMP-171";
        const string VaporeonGX = "SMP-172";
        const string EeveeGX = "SMP-174";
        const string MelmetalGX = "SMP-178";
        const string MarowakGX = "SMP-187";
        const string DetectivePikachu = "SMP-194";
        const string CharizardGX = "SMP-195";
        const string MewtwoGX = "SMP-196";
        const string GreninjaGX = "SMP-197";
        const string Sudowoodo = "SMP-207";
        const string PorygonZGX = "SMP-216";
        const string TrevenantDusknoir = "SMP-217";
        const string Celebi = "SMP-224";
        const string SandslashGX = "SMP-236";
        const string Glaceon = "SMP-238";
        const string CarracostaGX = "SMP-239";
        const string Aipom = "SMP-244";
        const string Mismagius = "SMP-245";
        const string SabrinaBrycen = "SMP-246";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Promos_Are_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "SMP" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(103, own.Count, "the promos that aren't prints of cards already in the pool");
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("SM Black Star Promos", C(SnorlaxGX).PrintSetName);
            Assert.AreEqual("SUM-53", C("SMP-9").BaseKey, "Togedemaru is Sun & Moon's");
            Assert.AreEqual("HIF-SV58", C("SMP-68").BaseKey, "Xurkitree-GX, printed with the typo Lighting-GX");
            Assert.AreEqual("SP-296", C("SMP-78").BaseKey, "Champions Festival");
            Assert.AreEqual("SMP-4", C("SMP-81").BaseKey, "the same Pikachu promo again");
            Assert.AreEqual("SMP-30", C("SMP-30a").BaseKey);
            Assert.AreEqual("SMP-174", C("SMP-242").BaseKey, "Eevee-GX");
            Assert.AreEqual(0, C(DuskMane).Attacks[0].Damage, "Dusk Shot's 60 is only in the text");
            Assert.AreEqual("TAG TEAM", C(TrevenantDusknoir).Mechanic);
        }

        // ------------------------------------------------------------------ SM01-SM79

        [Test]
        public void Snorlax_GX_Falls_Asleep_And_Can_Only_Use_Thunderous_Snore_While_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Collapse"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(SnorlaxGX));
            for (int i = 0; i < 5; i++) e.ScenarioAttach(snorlax, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, target.DamageCounters);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Asleep));
            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sleeper = e2.ScenarioActive(0, C(SnorlaxGX));
            for (int i = 0; i < 5; i++) e2.ScenarioAttach(sleeper, C(Psychic));
            e2.ScenarioActive(1, C(Celesteela));
            e2.ScenarioSetTurn(0, 3);
            e2.ApplyCondition(sleeper, SpecialCondition.Asleep);
            var labels = e2.GetMainActions(0).Select(o => o.Label).ToList();
            Assert.IsTrue(labels.Any(l => l.Contains("Thunderous Snore")));
            Assert.IsFalse(labels.Any(l => l.Contains("Collapse")));
        }

        [Test]
        public void Sunny_Day_Adds_20_To_Grass_And_Fire_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var turtwig = e.ScenarioActive(0, C(Turtwig));
            e.ScenarioBench(0, C(Lurantis));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(turtwig, target, 30);
            Assert.AreEqual(5, target.DamageCounters);
        }

        [Test]
        public void Flying_Flip_Hits_Each_Opposing_Pokemon_For_20()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Flying Flip"), new ScriptedAgent());
            var koko = e.ScenarioActive(0, C(TapuKoko));
            e.ScenarioAttach(koko, C(Psychic));
            e.ScenarioAttach(koko, C(Psychic));
            var active = e.ScenarioActive(1, C(Mewtwo));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, active.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
        }

        [Test]
        public void Big_Throw_GX_Discards_The_Defending_Pokemon_Without_A_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Big Throw-GX"), new ScriptedAgent());
            var bewear = e.ScenarioActive(0, C(BewearGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(bewear, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttach(target, C(Psychic));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Mewtwo"));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Queens_Command_GX_Makes_The_Opponent_Discard_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Queen's Command-GX"), new ScriptedAgent());
            var tsareena = e.ScenarioActive(0, C(TsareenaGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tsareena, C(Grass));
            e.ScenarioActive(1, C(Mewtwo));
            for (int i = 0; i < 5; i++) e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Calm_Strike_Does_60_More_After_Your_GX_Attack()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Calm Strike"), new ScriptedAgent());
                var bulu = e.ScenarioActive(0, C(TapuBulu));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(bulu, C(Grass));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1) e.State.Players[0].GxUsed = true;
                Step(e);
                Assert.AreEqual(pass == 0 ? 6 : 12, target.DamageCounters);
            }
        }

        [Test]
        public void Gear_Scan_Finds_An_Item_Not_A_Tool()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gear Scan"), new ScriptedAgent());
            var silvally = e.ScenarioActive(0, C(Silvally));
            e.ScenarioAttach(silvally, C(Psychic));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C(ChoiceBand), 1);
            e.ScenarioAddToDeck(0, C(PokeBall), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Poké Ball", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Golden_Wing_Moves_Basic_Energy_To_The_Bench_When_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Fire Energy|Fire Energy"), new ScriptedAgent().Then("Attack: Blizzard Burn"));
            var hooh = e.ScenarioActive(0, C(ShiningHoOh));
            e.ScenarioAttach(hooh, C(Fire));
            e.ScenarioAttach(hooh, C(Fire));
            var benched = e.ScenarioBench(0, C(Turtwig));
            var lapras = e.ScenarioActive(1, C(LaprasGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(lapras, C(Water));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(hooh.Uid));
            Assert.AreEqual(2, benched.Energy.Count);
        }

        [Test]
        public void Pressure_Takes_20_Off_The_Opponents_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(PressureMewtwo));
            var attacker = e.ScenarioActive(1, C(Turtwig));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, mewtwo, 50);
            Assert.AreEqual(3, mewtwo.DamageCounters);
        }

        [Test]
        public void Time_Recall_Lets_Evolved_Pokemon_Use_Earlier_Attacks()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var steenee = e.ScenarioActive(0, C(Bounsweet));
                e.ScenarioEvolve(steenee, C(Steenee));
                e.ScenarioAttach(steenee, C(Grass));
                if (pass == 1) e.ScenarioBench(0, C(ShiningCelebi));
                e.ScenarioActive(1, C(Mewtwo));
                e.ScenarioSetTurn(0, 3);
                Assert.AreEqual(pass == 1, e.GetMainActions(0).Any(o => o.Label.Contains("Splash")), pass == 1 ? "with Shining Celebi" : "without it");
            }
        }

        // ------------------------------------------------------------------ SM82-SM145

        [Test]
        public void Flight_Support_Gives_Latios_No_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var latios = e.ScenarioActive(0, C(Latios));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(latios));
            e.ScenarioBench(0, C(Latias));
            Assert.AreEqual(0, e.RetreatCostOf(latios));
        }

        [Test]
        public void Shining_Currents_Does_60_More_After_Healing_A_Water_Pokemon()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Shining Currents"), new ScriptedAgent());
                var fini = e.ScenarioActive(0, C(TapuFini));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(fini, C(Water));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1) fini.HealedOnTurn = 3;
                Step(e);
                Assert.AreEqual(pass == 0 ? 6 : 12, target.DamageCounters);
            }
        }

        [Test]
        public void Shadowy_Echoes_Benches_A_Basic_From_Each_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shadowy Echoes"), new ScriptedAgent());
            var marshadow = e.ScenarioActive(0, C(Marshadow));
            e.ScenarioAttach(marshadow, C(Psychic));
            e.ScenarioAddToDiscard(0, C(Turtwig), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToDiscard(1, C(Snubbull), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Turtwig", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual("Snubbull", e.State.Players[1].Bench.Single().Name);
        }

        [Test]
        public void Shining_Mane_Removes_Weakness_From_Your_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var turtwig = e.ScenarioActive(0, C(Turtwig));
            e.ScenarioBench(0, C(SolgaleoGX));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNull(e.WeaknessOf(turtwig));
            Assert.AreEqual(EnergyType.Psychic, e.WeaknessOf(theirs));
        }

        [Test]
        public void Turbo_Strike_Attaches_2_Basic_Energy_To_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Turbo Strike"), new ScriptedAgent());
            var solgaleo = e.ScenarioActive(0, C(SolgaleoGX));
            e.ScenarioAttach(solgaleo, C(Psychic));
            e.ScenarioAttach(solgaleo, C(Psychic));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
            Assert.AreEqual(2, benched.Energy.Count);
        }

        [Test]
        public void Dusk_Shot_Only_Hits_A_Pokemon_GX_Or_EX_Without_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dusk Shot"), new ScriptedAgent());
            var necrozma = e.ScenarioActive(0, C(DuskMane));
            e.ScenarioAttach(necrozma, C(Metal));
            var active = e.ScenarioActive(1, C(Mewtwo));
            var gx = e.ScenarioBench(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, active.DamageCounters);
            Assert.AreEqual(6, gx.DamageCounters);
        }

        [Test]
        public void Gulf_Stream_Counts_Its_Damage_With_6_Prize_Cards_Left()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Gulf Stream"), new ScriptedAgent());
                var necrozma = e.ScenarioActive(0, C(DawnWings));
                e.ScenarioAttach(necrozma, C(Psychic));
                necrozma.DamageCounters = 3;
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1) e.State.Players[0].Prizes.RemoveAt(0);
                Step(e);
                Assert.AreEqual(pass == 0 ? 8 : 2, target.DamageCounters);
            }
        }

        [Test]
        public void Thunderous_Rain_GX_Hits_Each_Opposing_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Thunderous Rain-GX"), new ScriptedAgent());
            var raikou = e.ScenarioActive(0, C(RaikouGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(raikou, C(Lightning));
            var active = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttach(active, C(Psychic));
            var bare = e.ScenarioBench(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, active.DamageCounters);
            Assert.AreEqual(0, bare.DamageCounters);
        }

        [Test]
        public void Liberation_GX_Does_120_For_Each_Energy_Card_In_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Liberation-GX"), new ScriptedAgent());
            var zygarde = e.ScenarioActive(0, C(ZygardeGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(zygarde, C(Fighting));
            var target = e.ScenarioActive(1, C(LaprasGX));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioAddToHand(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
        }

        [Test]
        public void Bullet_Uppercut_Does_60_More_To_A_GX_Without_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bullet Uppercut"), new ScriptedAgent());
            var zekrom = e.ScenarioActive(0, C(ZekromGX));
            e.ScenarioAttach(zekrom, C(Lightning));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, target.DamageCounters, "Celesteela-GX is weak to Lightning, but it's 10 + 60");
        }

        // ------------------------------------------------------------------ SM153-SM200

        [Test]
        public void Pika_Shield_Stops_Paralysis()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(Pikachu));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(1, 4);
            e.ApplyCondition(pikachu, SpecialCondition.Paralyzed);
            Assert.IsFalse(pikachu.Has(SpecialCondition.Paralyzed));
        }

        [Test]
        public void Psycho_Boost_Does_50_Right_After_Using_It()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Psycho Boost"), new ScriptedAgent());
                var deoxys = e.ScenarioActive(0, C(Deoxys));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(deoxys, C(Psychic));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1)
                {
                    deoxys.LastAttackName = "Psycho Boost";
                    deoxys.LastAttackTurn = 1;
                }
                Step(e);
                Assert.AreEqual(pass == 0 ? 10 : 5, target.DamageCounters);
            }
        }

        [Test]
        public void Power_Suction_Moves_An_Energy_To_Deoxys()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Power Suction"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioAttach(active, C(Psychic));
            var deoxys = e.ScenarioBench(0, C(Deoxys));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, active.Energy.Count);
            Assert.AreEqual(1, deoxys.Energy.Count);
        }

        [Test]
        public void Shining_Burst_Needs_6_Or_Fewer_Prize_Cards_Between_Both_Players()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Shining Burst"), new ScriptedAgent());
                var necrozma = e.ScenarioActive(0, C(UltraNecrozma));
                e.ScenarioAttach(necrozma, C(Psychic));
                e.ScenarioAttach(necrozma, C(Psychic));
                e.ScenarioAttach(necrozma, C(Metal));
                e.ScenarioAttach(necrozma, C(Metal));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioBench(1, C(Snubbull));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1)
                {
                    e.State.Players[0].Prizes.RemoveRange(0, 3);
                    e.State.Players[1].Prizes.RemoveRange(0, 3);
                }
                Step(e);
                if (pass == 0)
                {
                    Assert.AreEqual(10, target.DamageCounters);
                    Assert.AreEqual(4, necrozma.Energy.Count);
                }
                else
                {
                    Assert.IsNull(e.State.FindPokemon(target.Uid), "200 Knocks Out Celesteela-GX");
                    Assert.AreEqual(0, necrozma.Energy.Count);
                }
            }
        }

        [Test]
        public void Heat_Stage_Attaches_Up_To_3_Fire_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heat Stage"), new ScriptedAgent());
            var flareon = e.ScenarioActive(0, C(FlareonGX));
            e.ScenarioAttach(flareon, C(Fire));
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, flareon.Energy.Count);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Hydrating_Drops_Heals_The_Active_Water_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Hydrating Drops"), new ScriptedAgent());
            var lapras = e.ScenarioActive(0, C(LaprasGX));
            lapras.DamageCounters = 5;
            e.ScenarioBench(0, C(VaporeonGX));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, lapras.DamageCounters);
        }

        [Test]
        public void Ascension_DNA_Heals_And_Evolves_Eevee_GX()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ascension DNA"), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C(EeveeGX));
            eevee.DamageCounters = 4;
            e.ScenarioAddToHand(0, C(VaporeonGX));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Vaporeon-GX", eevee.Name);
            Assert.AreEqual(0, eevee.DamageCounters);
        }

        [Test]
        public void Iron_Force_GX_Attaches_Metal_Energy_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Iron Force-GX"), new ScriptedAgent());
            var melmetal = e.ScenarioActive(0, C(MelmetalGX));
            e.ScenarioAttach(melmetal, C(Metal));
            e.ScenarioAddToDiscard(0, C(Metal), 3);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, melmetal.Energy.Count);
        }

        [Test]
        public void Lost_Boomerang_GX_Sends_A_Knocked_Out_Pokemon_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lost Boomerang-GX"), new ScriptedAgent());
            e.ScenarioActive(0, C(MarowakGX));
            var target = e.ScenarioActive(1, C(Snubbull));
            target.DamageCounters = 2;
            e.ScenarioAttach(target, C(Psychic));
            var other = e.ScenarioBench(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
            var lost = e.State.Players[1].LostZone.Select(c => c.Def.Name).ToList();
            CollectionAssert.AreEquivalent(new[] { "Snubbull", "Psychic Energy" }, lost);
            Assert.AreEqual(5, other.DamageCounters);
        }

        [Test]
        public void Cursed_Body_Confuses_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var marowak = e.ScenarioActive(0, C(MarowakGX));
            var attacker = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, marowak, 30);
            Assert.IsTrue(attacker.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Brilliant_Deduction_Takes_1_Of_The_Top_4_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Brilliant Deduction"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(DetectivePikachu));
            e.ScenarioAttach(pikachu, C(Lightning));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
            Assert.AreEqual(19, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Raging_Destruction_Attaches_The_Fire_Energy_It_Discards()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Raging Destruction"), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C(CharizardGX));
            e.ScenarioAttach(charizard, C(Fire));
            e.State.Players[0].Deck.Clear();
            e.ScenarioAddToDeck(0, C(Fire), 4);
            e.ScenarioAddToDeck(0, C(Grass), 6);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, charizard.Energy.Count);
            Assert.AreEqual(4, e.State.Players[0].Discard.Count(c => c.Def.Name == "Grass Energy"));
            Assert.AreEqual(2, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Telekinesis_Ignores_Weakness()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Telekinesis"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoGX));
            e.ScenarioAttach(mewtwo, C(Psychic));
            e.ScenarioAttach(mewtwo, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, target.DamageCounters, "Mewtwo is weak to Psychic, but it's 50");
        }

        [Test]
        public void Elusive_Master_Benches_Greninja_GX_From_An_Otherwise_Empty_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Elusive Master"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioAddToHand(0, C(GreninjaGX));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Greninja-GX", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Dark_Mist_GX_Puts_A_Benched_Pokemon_Into_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dark Mist-GX"), new ScriptedAgent());
            var greninja = e.ScenarioActive(0, C(GreninjaGX));
            e.ScenarioAttach(greninja, C(Water));
            e.ScenarioActive(1, C(Mewtwo));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(benched.Uid));
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Snubbull"));
        }

        // ------------------------------------------------------------------ SM206-SM246

        [Test]
        public void Territorial_Strike_Needs_Your_Stadium()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Territorial Strike"), new ScriptedAgent());
                var sudowoodo = e.ScenarioActive(0, C(Sudowoodo));
                e.ScenarioAttach(sudowoodo, C(Fighting));
                e.ScenarioAttach(sudowoodo, C(Fighting));
                var target = e.ScenarioActive(1, C(Mewtwo));
                if (pass == 1) e.ScenarioSetStadium(0, C(ChampionsFestival));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 0 : 8, target.DamageCounters);
            }
        }

        [Test]
        public void Troubleshooting_Discards_A_Special_Energy_To_Heal_80()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Troubleshooting"), new ScriptedAgent());
            var porygon = e.ScenarioActive(0, C(PorygonZGX));
            e.ScenarioAttach(porygon, C(DoubleColorless));
            porygon.DamageCounters = 9;
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, porygon.DamageCounters);
            Assert.AreEqual(0, porygon.Energy.Count);
        }

        [Test]
        public void Pale_Moon_GX_Knocks_Out_Next_Turn_And_Strips_Energy_With_An_Extra_Psychic()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pale Moon-GX"), new ScriptedAgent());
            var tag = e.ScenarioActive(0, C(TrevenantDusknoir));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(tag, C(Psychic));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttach(target, C(Psychic));
            e.ScenarioAttach(target, C(Psychic));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, target.Energy.Count);
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid));
            e.EndTurn();
            e.ResolveKnockouts();
            Assert.IsNull(e.State.FindPokemon(target.Uid));
        }

        [Test]
        public void Call_For_Greatness_Finds_Pokemon_GX_With_Different_Names()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Call for Greatness", "Celesteela-GX|Celesteela-GX|Lapras-GX"), new ScriptedAgent());
            var celebi = e.ScenarioActive(0, C(Celebi));
            e.ScenarioAttach(celebi, C(Psychic));
            e.ScenarioAddToDeck(0, C(Celesteela), 2);
            e.ScenarioAddToDeck(0, C(LaprasGX), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEquivalent(new[] { "Celesteela-GX", "Lapras-GX" }, e.State.Players[0].Hand.Select(c => c.Def.Name).ToList());
        }

        [Test]
        public void Spiky_Storm_GX_Hits_Only_Damaged_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spiky Storm-GX"), new ScriptedAgent());
            var sandslash = e.ScenarioActive(0, C(SandslashGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(sandslash, C(Water));
            var active = e.ScenarioActive(1, C(Mewtwo));
            active.DamageCounters = 1;
            var hurt = e.ScenarioBench(1, C(Celesteela));
            hurt.DamageCounters = 1;
            var fresh = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, active.DamageCounters);
            Assert.AreEqual(11, hurt.DamageCounters);
            Assert.AreEqual(0, fresh.DamageCounters);
        }

        [Test]
        public void High_Density_Armor_Takes_90_Off_Only_At_Full_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var carracosta = e.ScenarioActive(0, C(CarracostaGX));
            var attacker = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, carracosta, 100);
            Assert.AreEqual(1, carracosta.DamageCounters);
            e.DealAttackDamage(attacker, carracosta, 100);
            Assert.AreEqual(11, carracosta.DamageCounters);
        }

        [Test]
        public void Stone_Age_GX_Benches_Fossil_Pokemon_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stone Age-GX"), new ScriptedAgent());
            var carracosta = e.ScenarioActive(0, C(CarracostaGX));
            e.ScenarioAttach(carracosta, C(Fighting));
            e.ScenarioAddToDiscard(0, C(Tirtouga), 1);
            e.ScenarioAddToDiscard(0, C(Turtwig), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Tirtouga", e.State.Players[0].Bench.Single().Name);
        }

        [Test]
        public void Yank_Out_Discards_Down_To_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Yank Out"), new ScriptedAgent());
            var aipom = e.ScenarioActive(0, C(Aipom));
            e.ScenarioAttach(aipom, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            for (int i = 0; i < 8; i++) e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Nohahex_Knocks_Out_With_Exactly_9_Damage_Counters()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Nohahex"), new ScriptedAgent());
                var mismagius = e.ScenarioActive(0, C(Mismagius));
                e.ScenarioAttach(mismagius, C(Psychic));
                e.ScenarioAttach(mismagius, C(Psychic));
                var target = e.ScenarioActive(1, C(Mewtwo));
                target.DamageCounters = pass == 0 ? 8 : 9;
                e.ScenarioBench(1, C(Snubbull));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 1, e.State.FindPokemon(target.Uid) == null);
            }
        }

        [Test]
        public void Hypnotic_Blizzard_Puts_The_Active_To_Sleep_And_Hits_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hypnotic Blizzard"), new ScriptedAgent());
            var glaceon = e.ScenarioActive(0, C(Glaceon));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(glaceon, C(Water));
            var active = e.ScenarioActive(1, C(Mewtwo));
            var benched = e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(active.Has(SpecialCondition.Asleep));
            Assert.AreEqual(9, active.DamageCounters);
            Assert.AreEqual(2, benched.DamageCounters);
        }

        [Test]
        public void Sabrina_And_Brycen_Finds_2_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sabrina & Brycen", "Grass Energy|Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAddToHand(0, C(SabrinaBrycen));
            e.ScenarioAddToDeck(0, C(Grass), 1);
            e.ScenarioAddToDeck(0, C(Fire), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            CollectionAssert.AreEquivalent(new[] { "Grass Energy", "Fire Energy" }, e.State.Players[0].Hand.Select(c => c.Def.Name).ToList());
        }
    }
}
