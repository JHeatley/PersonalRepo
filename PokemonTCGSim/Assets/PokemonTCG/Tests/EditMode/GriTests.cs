using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v89: Guardians Rising (GRI 1-169).</summary>
    public class GriTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string FairyEnergy = "SUM-Y";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Turtwig = "UPR-7";
        const string Celesteela = "UPR-144";
        const string Charmander = "BUS-18";
        const string ChoiceBand = "BUS-162";
        const string MountLanakila = "BUS-118";
        const string Golisopod = "GRI-9";
        const string Victini = "GRI-10";
        const string Chandelure = "GRI-13";
        const string Salandit = "GRI-15";
        const string Salazzle = "GRI-16";
        const string AlolanSandslash = "GRI-20";
        const string Tentacool = "GRI-23";
        const string Vanillite = "GRI-33";
        const string Wishiwashi = "GRI-37";
        const string WishiwashiGX = "GRI-38";
        const string AlolanGeodude = "GRI-40";
        const string Garbodor = "GRI-51";
        const string Oricorio = "GRI-56";
        const string ToxapexGX = "GRI-57";
        const string Dhelmise = "GRI-59";
        const string Machoke = "GRI-64";
        const string Machamp = "GRI-65";
        const string Absol = "GRI-81";
        const string MetagrossGX = "GRI-85";
        const string Whimsicott = "GRI-91";
        const string Comfey = "GRI-93";
        const string Jangmoo = "GRI-98";
        const string Swellow = "GRI-104";
        const string Komala = "GRI-114";
        const string AltarOfTheMoone = "GRI-117";
        const string AltarOfTheSunne = "GRI-118";
        const string FieldBlower = "GRI-125";
        const string Hala = "GRI-126";
        const string MaxPotion = "GRI-128";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "GRI" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(113, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Guardians Rising", C(Golisopod).PrintSetName);
            Assert.AreEqual("HIF-SV52", C("GRI-131").BaseKey, "Turtonator-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("BUS-162", C("GRI-121").BaseKey, "Choice Band came with Burning Shadows' secret rare");
            Assert.AreEqual("CES-132", C("GRI-144").BaseKey, "full art Hau");
            Assert.AreEqual("SLG-69", C("GRI-166").BaseKey, "Double Colorless Energy");
            Assert.AreEqual("GRI-85", C("GRI-157a").BaseKey);
            Assert.AreEqual(0, C("GRI-54").Attacks[0].Damage, "Tractorbeam's 30 is only in the text");
            Assert.AreEqual(0, C("GRI-19").Attacks[0].Cost.Length, "Defense Curl costs nothing");
            Assert.AreEqual(Stage.Stage2, C("GRI-147").Stage, "Incineroar-GX, printed here only as a rainbow rare");
        }

        [Test]
        public void Victory_Star_Offers_One_Reflip_Of_Any_Of_Your_Attacks_Each_Turn()
        {
            var me = new ScriptedAgent().Then("Attack: Triple Spin");
            var e = Scenario(me, new ScriptedAgent());
            var vanillite = e.ScenarioActive(0, C(Vanillite));
            e.ScenarioAttach(vanillite, C(Water));
            e.ScenarioBench(0, C(Victini));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int asked = me.Seen.Count(r => (r.Prompt ?? "").Contains("Victory Star") && (r.Prompt ?? "").Contains("flip again"));
            Assert.AreEqual(1, asked, "once, from the Benched Victini");
        }

        [Test]
        public void Armor_Takes_30_Off_Every_Attack()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            var golisopod = e.ScenarioActive(1, C(Golisopod));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, golisopod, 50);
            Assert.AreEqual(2, golisopod.DamageCounters);
        }

        [Test]
        public void Shady_Move_Moves_A_Counter_Between_Any_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shady Move", "Mewtwo"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chandelure));
            var mine = e.ScenarioBench(0, C(Turtwig));
            mine.DamageCounters = 2;
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, mine.DamageCounters);
            Assert.AreEqual(1, theirs.DamageCounters);
        }

        [Test]
        public void Hot_Poison_Burns_And_Poisons_When_Salazzle_Evolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Salandit));
            e.ScenarioAddToHand(0, C(Salazzle));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Burned));
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Slush_Rush_Draws_A_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Slush Rush"), new ScriptedAgent());
            e.ScenarioActive(0, C(AlolanSandslash));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Sludge_Shock_Stops_Healing_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Sludge Shock"), new ScriptedAgent());
            var tentacool = e.ScenarioActive(0, C(Tentacool));
            e.ScenarioAttach(tentacool, C(Water));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            e.Heal(target, 10);
            Assert.AreEqual(1, target.DamageCounters);
        }

        [Test]
        public void Schooling_Swaps_In_Wishiwashi_GX_Keeping_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Schooling"), new ScriptedAgent());
            var fish = e.ScenarioActive(0, C(Wishiwashi));
            e.ScenarioAttach(fish, C(Water));
            fish.DamageCounters = 2;
            e.ScenarioAddToHand(0, C(WishiwashiGX));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Wishiwashi-GX", fish.Name);
            Assert.AreEqual(2, fish.DamageCounters);
            Assert.AreEqual(1, fish.Energy.Count);
            Assert.AreEqual("Wishiwashi", e.State.Players[0].Hand.Single().Def.Name);
        }

        [Test]
        public void Blue_Surge_GX_Moves_All_The_Energy_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blue Surge-GX"), new ScriptedAgent());
            var gx = e.ScenarioActive(0, C(WishiwashiGX));
            for (int i = 0; i < 5; i++) e.ScenarioAttach(gx, C(Water));
            var benched = e.ScenarioBench(0, C(Turtwig));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, gx.Energy.Count);
            Assert.AreEqual(5, benched.Energy.Count);
        }

        [Test]
        public void Rock_Polish_Gives_No_Retreat_Cost_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rock Polish"), new ScriptedAgent());
            var geodude = e.ScenarioActive(0, C(AlolanGeodude));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(2, e.RetreatCostOf(geodude));
            Step(e);
            NextTurn(e);
            Assert.AreEqual(2, e.RetreatCostOf(geodude), "not during the opponent's turn");
            NextTurn(e);
            Assert.AreEqual(0, e.RetreatCostOf(geodude));
        }

        [Test]
        public void Trashalanche_Counts_Items_But_Not_Tools_In_The_Opponents_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trashalanche"), new ScriptedAgent());
            var garbodor = e.ScenarioActive(0, C(Garbodor));
            e.ScenarioAttach(garbodor, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioAddToDiscard(1, C(FieldBlower), 2);
            e.ScenarioAddToDiscard(1, C(ChoiceBand), 1);
            e.ScenarioAddToDiscard(1, C(Hala), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, target.DamageCounters, "2 x 20: the 2 Items only; a Pokémon Tool isn't an Item card (your ruling, v90), nor is the Supporter");
        }

        [Test]
        public void Super_Intense_Poison_Puts_10_Counters_Between_Turns()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Super Intense Poison"), new ScriptedAgent());
            var toxapex = e.ScenarioActive(0, C(ToxapexGX));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(toxapex, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(10, target.PoisonCounterBase);
        }

        [Test]
        public void Steelworker_Stacks_For_Metal_Attackers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var metal = e.ScenarioActive(0, C(Celesteela));
            var other = e.ScenarioBench(0, C(Snubbull));
            e.ScenarioBench(0, C(Dhelmise));
            e.ScenarioBench(0, C(Dhelmise));
            var target = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(metal, target, 30);
            Assert.AreEqual(5, target.DamageCounters, "30 + 10 + 10");
            e.DealAttackDamage(other, target, 30);
            Assert.AreEqual(8, target.DamageCounters, "Snubbull isn't a Metal Pokémon");
        }

        [Test]
        public void Daunting_Pose_Protects_The_Bench_From_Damage_And_Counters()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioActive(1, C(Machoke));
            var benched = e.ScenarioBench(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, benched, 30);
            Assert.AreEqual(0, benched.DamageCounters);
        }

        [Test]
        public void Settle_The_Score_Counts_Prize_Cards_Taken_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Settle the Score"), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C(Machamp));
            e.ScenarioAttach(machamp, C(Fighting));
            e.ScenarioAttach(machamp, C(Fighting));
            var target = e.ScenarioActive(1, C(WishiwashiGX));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[1].PrizesTakenTurn = 2;
            e.State.Players[1].PrizesTakenThatTurn = 2;
            Step(e);
            Assert.AreEqual(18, target.DamageCounters, "20 + 2 x 80");
        }

        [Test]
        public void Doom_News_Knocks_Out_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Doom News"), new ScriptedAgent());
            var absol = e.ScenarioActive(0, C(Absol));
            e.ScenarioAttach(absol, C(Darkness));
            e.ScenarioAttach(absol, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, absol.Energy.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.IsEnergy));
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid));
            e.EndTurn();
            e.ResolveKnockouts();
            Assert.IsNull(e.State.FindPokemon(target.Uid));
        }

        [Test]
        public void Geotech_System_Attaches_Psychic_Or_Metal_Energy_To_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Geotech System"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Snubbull));
            e.ScenarioBench(0, C(MetagrossGX));
            e.ScenarioAddToDiscard(0, C(Psychic), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Psychic Energy", active.Energy.Single().Def.Name);
        }

        [Test]
        public void The_Wages_Of_Fluff_Gives_2_More_Prize_Cards_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: The Wages of Fluff"), new ScriptedAgent());
            var whimsicott = e.ScenarioActive(0, C(Whimsicott));
            e.ScenarioAttach(whimsicott, C(Psychic));
            var target = e.ScenarioActive(1, C(Snubbull));
            e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            e.DealAttackDamage(whimsicott, target, 100);
            e.ResolveKnockouts();
            Assert.AreEqual(3, e.State.Players[0].Prizes.Count, "1 + 2 Prize cards");
        }

        [Test]
        public void Flower_Shield_Protects_Pokemon_With_Fairy_Energy()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var active = e.ScenarioActive(0, C(Snubbull));
                if (pass == 0) e.ScenarioAttach(active, C(FairyEnergy));
                e.ScenarioBench(0, C(Comfey));
                e.ScenarioActive(1, C(Mewtwo));
                e.ScenarioSetTurn(0, 3);
                e.ApplyCondition(active, SpecialCondition.Poisoned);
                Assert.AreEqual(pass == 1, active.Has(SpecialCondition.Poisoned), pass == 0 ? "Fairy Energy attached" : "no Fairy Energy");
            }
        }

        [Test]
        public void Comatose_Puts_Komala_To_Sleep_And_Hypno_Roll_Works_While_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Komala"), new ScriptedAgent());
            var komala = e.ScenarioActive(0, C(Komala));
            e.ScenarioAttach(komala, C(Psychic));
            e.ScenarioAttach(komala, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(komala.Has(SpecialCondition.Asleep));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Hypno Roll")));
        }

        [Test]
        public void Bulletproof_Takes_10_Off()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            var jangmo = e.ScenarioActive(1, C(Jangmoo));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, jangmo, 30);
            Assert.AreEqual(2, jangmo.DamageCounters);
        }

        [Test]
        public void Swallow_Dive_Does_80_More_After_Agility()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Swallow Dive"), new ScriptedAgent());
                var swellow = e.ScenarioActive(0, C(Swellow));
                e.ScenarioAttach(swellow, C(Psychic));
                var target = e.ScenarioActive(1, C(Celesteela));
                e.ScenarioSetTurn(0, 3);
                if (pass == 1)
                {
                    swellow.LastAttackName = "Agility";
                    swellow.LastAttackTurn = 1;
                }
                Step(e);
                Assert.AreEqual(pass == 0 ? 4 : 12, target.DamageCounters);
            }
        }

        [Test]
        public void Revelation_Dance_Needs_A_Stadium()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Revelation Dance"), new ScriptedAgent());
                var oricorio = e.ScenarioActive(0, C(Oricorio));
                e.ScenarioAttach(oricorio, C(Psychic));
                var target = e.ScenarioActive(1, C(Celesteela));
                if (pass == 1) e.ScenarioSetStadium(1, C(MountLanakila));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 0 : 3, target.DamageCounters);
            }
        }

        [Test]
        public void Altars_Change_Retreat_Costs_And_Weakness()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            e.ScenarioAttach(mewtwo, C(Psychic));
            var charmander = e.ScenarioActive(1, C(Charmander));
            e.ScenarioSetTurn(0, 3);
            int cost = e.RetreatCostOf(mewtwo);
            e.ScenarioSetStadium(1, C(AltarOfTheMoone));
            Assert.AreEqual(System.Math.Max(0, cost - 2), e.RetreatCostOf(mewtwo));
            e.ScenarioSetStadium(1, C(AltarOfTheSunne));
            Assert.IsNull(e.WeaknessOf(charmander), "Fire Pokémon have no Weakness");
        }

        [Test]
        public void Field_Blower_Discards_Tools_And_The_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Field Blower", "Choice Band|Mount Lanakila"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var theirs = e.ScenarioActive(1, C(Mewtwo));
            e.ScenarioAttachTool(theirs, C(ChoiceBand));
            e.ScenarioSetStadium(1, C(MountLanakila));
            e.ScenarioAddToHand(0, C(FieldBlower));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, theirs.Tools.Count);
            Assert.IsNull(e.State.Stadium);
        }

        [Test]
        public void Hala_Draws_7_After_Your_GX_Attack()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Play Hala"), new ScriptedAgent());
                e.ScenarioActive(0, C(Snubbull));
                e.ScenarioActive(1, C(Mewtwo));
                e.ScenarioAddToHand(0, C(Hala));
                e.ScenarioAddToHand(0, C(Grass));
                if (pass == 1) e.State.Players[0].GxUsed = true;
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 4 : 7, e.State.Players[0].Hand.Count);
            }
        }

        [Test]
        public void Max_Potion_Heals_Everything_And_Discards_The_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Max Potion"), new ScriptedAgent());
            var hurt = e.ScenarioActive(0, C(Mewtwo));
            hurt.DamageCounters = 9;
            e.ScenarioAttach(hurt, C(Psychic));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(MaxPotion));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, hurt.DamageCounters);
            Assert.AreEqual(0, hurt.Energy.Count);
        }
    }
}
