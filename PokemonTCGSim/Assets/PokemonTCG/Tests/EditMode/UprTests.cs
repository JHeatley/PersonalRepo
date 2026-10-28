using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v85: Ultra Prism (UPR 1-173).</summary>
    public class UprTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Mewtwo = "DET-12";
        const string Snubbull = "DET-15";
        const string Pupitar = "CES-75";
        const string Regice = "JTG-42";
        const string Charmander = "TEU-12";
        const string SilvallyGX = "HIF-SV79";
        const string PathToThePeak = "ASR-213";
        const string Turtwig = "UPR-7";
        const string Grotle = "UPR-8";
        const string Torterra = "UPR-9";
        const string Cherrim = "UPR-11";
        const string Infernape = "UPR-23";
        const string AlolanSandslash = "UPR-29";
        const string Empoleon = "UPR-34";
        const string Luxray = "UPR-48";
        const string Drifblim = "UPR-52";
        const string Giratina = "UPR-58";
        const string Cosmog = "UPR-60";
        const string Lunala = "UPR-62";
        const string DawnWings = "UPR-63";
        const string Rampardos = "UPR-65";
        const string Passimian = "UPR-70";
        const string Weavile = "UPR-74";
        const string Darkrai = "UPR-77";
        const string AlolanDugtrio = "UPR-79";
        const string Magnemite = "UPR-81";
        const string Bastiodon = "UPR-85";
        const string Shiinotic = "UPR-93";
        const string Cyrus = "UPR-120";
        const string ElectricMemory = "UPR-121";
        const string EscapeBoard = "UPR-122";
        const string Lillie = "UPR-125";
        const string Looker = "UPR-126";
        const string MissingClover = "UPR-129";
        const string SuperBoost = "UPR-136";
        const string UnitLPM = "UPR-138";
        const string Celesteela = "UPR-144";
        const string LunalaGX = "UPR-172";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        [Test]
        public void The_Whole_Set_Is_In_The_Pool_As_Expanded()
        {
            var own = db.Cards.Where(d => d.SetCode == "UPR" && !d.IsAlternatePrint).ToList();
            Assert.AreEqual(128, own.Count);
            Assert.IsTrue(own.All(d => Formats.CategoryOf(d) == CardCategory.Expanded), "no regulation mark: Expanded");
            Assert.AreEqual("Ultra Prism", C(Giratina).PrintSetName);
            Assert.AreEqual("HIF-SV46", C("UPR-13").BaseKey, "Leafeon-GX was already in the pool (Hidden Fates Shiny Vault)");
            Assert.AreEqual("HIF-SV40", C("UPR-99").BaseKey, "so was Garchomp");
            Assert.AreEqual("HIF-SV58", C("UPR-142").BaseKey, "Xurkitree-GX has no regular print in Ultra Prism");
            Assert.AreEqual("UPR-140", C("UPR-158").BaseKey, "Pheromosa-GX's rainbow rare");
            Assert.AreEqual("POR-71", C("UPR-166").BaseKey, "Crushing Hammer was already in the pool");
            Assert.AreEqual("30C-CC23", C("UPR-173").BaseKey, "Solgaleo-GX was already in the pool");
            Assert.AreEqual("HIF-SV82", C("UPR-119a").BaseKey);
            Assert.AreEqual("UPR-122", C("UPR-167").BaseKey, "gold Escape Board");
            Assert.AreEqual("Giratina ◇", C(Giratina).Name);
            Assert.IsTrue(C(Giratina).IsPrismStar);
            Assert.IsTrue(C(Cyrus).IsPrismStar);
            Assert.IsTrue(C(SuperBoost).IsPrismStar);
            Assert.AreEqual(0, C(AlolanSandslash).Attacks[0].Cost.Length, "Spike Armor costs nothing");
            Assert.AreEqual(0, C("UPR-31").Attacks[0].Damage, "Splatter's 20 is only in the text");
            Assert.AreEqual(0, C(Luxray).Attacks[0].Damage, "so is Volt Bolt's 150");
            Assert.AreEqual("Cosmoem", C(LunalaGX).EvolvesFrom);
            Assert.IsTrue(C(Celesteela).IsUltraBeast);
        }

        [Test]
        public void Super_Boost_Energy_Only_Works_On_A_Stage_2_And_Gives_4_With_3_Stage_2_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var grotle = e.ScenarioActive(0, C(Grotle));
            e.ScenarioAttach(grotle, C(SuperBoost));
            Assert.AreEqual(1, e.EnergyCount(grotle));
            Assert.IsFalse(e.HasEnergyOfType(grotle, EnergyType.Grass), "only Colorless on a Stage 1 (Compendium)");
            var torterra = e.ScenarioBench(0, C(Torterra));
            e.ScenarioAttach(torterra, C(SuperBoost));
            Assert.AreEqual(1, e.EnergyCount(torterra));
            Assert.IsTrue(e.HasEnergyOfType(torterra, EnergyType.Grass), "every type on a Stage 2");
            e.ScenarioBench(0, C(Infernape));
            Assert.AreEqual(1, e.EnergyCount(torterra), "2 Stage 2 Pokémon in play");
            e.ScenarioBench(0, C(Empoleon));
            Assert.AreEqual(4, e.EnergyCount(torterra), "3 Stage 2 Pokémon in play: 4 Energy");
            Assert.AreEqual(1, e.EnergyCount(grotle), "still 1 on the Stage 1");
        }

        [Test]
        public void Escape_Board_Lowers_The_Retreat_Cost_And_Lets_An_Asleep_Pokemon_Retreat()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var regice = e.ScenarioActive(0, C(Regice));
            e.ScenarioBench(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(regice, C(Water));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(regice, SpecialCondition.Asleep);
            Assert.IsFalse(e.CanRetreat(e.State.Players[0]), "Asleep");
            e.ScenarioAttachTool(regice, C(EscapeBoard));
            Assert.AreEqual(2, e.RetreatCostOf(regice), "3 - 1");
            Assert.IsTrue(e.CanRetreat(e.State.Players[0]), "Escape Board lets it retreat while Asleep");
        }

        [Test]
        public void Electric_Memory_Makes_Silvally_GX_A_Lightning_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var silvally = e.ScenarioActive(0, C(SilvallyGX));
            Assert.IsTrue(e.IsType(silvally, EnergyType.Colorless));
            e.ScenarioAttachTool(silvally, C(ElectricMemory));
            Assert.IsTrue(e.IsType(silvally, EnergyType.Lightning));
            Assert.IsFalse(e.IsType(silvally, EnergyType.Colorless), "only Lightning now");
            var regice = e.ScenarioBench(0, C(Regice));
            e.ScenarioAttachTool(regice, C(ElectricMemory));
            Assert.IsFalse(e.IsType(regice, EnergyType.Lightning), "only for Silvally-GX");
        }

        [Test]
        public void Cyrus_Needs_A_Water_Or_Metal_Active_And_The_Opponent_Keeps_2_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cyrus"), new ScriptedAgent().Then("Snubbull|Mewtwo"));
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Regice));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioBench(1, C(Pupitar));
            e.ScenarioBench(1, C(Cosmog));
            e.ScenarioAddToHand(0, C(Cyrus));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Cyrus ◇"), "Snubbull is a Fairy Pokémon");
            e.State.Players[0].Active = null;
            e.ScenarioActive(0, C(Regice));
            int deck = e.State.Players[1].Deck.Count;
            Step(e);
            var bench = e.State.Players[1].Bench.Select(m => m.Name).OrderBy(n => n).ToList();
            CollectionAssert.AreEqual(new[] { "Mewtwo", "Snubbull" }, bench);
            Assert.AreEqual(deck + 2, e.State.Players[1].Deck.Count, "Pupitar and Cosmog shuffled into the deck");
            Assert.IsTrue(e.State.Players[0].LostZone.Any(c => c.Def.Name == "Cyrus ◇"), "a Prism Star card goes to the Lost Zone");
        }

        [Test]
        public void Four_Missing_Clover_At_Once_Take_A_Prize_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Missing Clover"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(0, C(MissingClover));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(5, me.Prizes.Count, "took a Prize card");
            Assert.AreEqual(4, me.Discard.Count(c => c.Def.Name == "Missing Clover"));
            Assert.AreEqual(1, me.Hand.Count, "only the Prize card");
        }

        [Test]
        public void Chaotic_Star_Attaches_The_Only_Psychic_Energy_In_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Giratina"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Giratina));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var giratina = e.State.Players[0].Bench.Single();
            Assert.AreEqual(1, giratina.Energy.Count, "1 Energy is enough (your ruling, v85)");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "the Ability doesn't use the attachment for the turn");
        }

        [Test]
        public void Full_Moon_Star_Attaches_A_Psychic_Energy_For_Each_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Full Moon Star"), new ScriptedAgent());
            var lunala = e.ScenarioActive(0, C(Lunala));
            e.ScenarioAttach(lunala, C(Psychic));
            e.ScenarioBench(0, C(Cosmog));
            e.ScenarioAddToDiscard(0, C(Psychic), 5);
            e.ScenarioActive(1, C(Regice));
            e.ScenarioBench(1, C(Snubbull));
            e.ScenarioBench(1, C(Mewtwo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            int attached = e.State.Players[0].InPlay().Sum(m => m.Energy.Count);
            Assert.AreEqual(4, attached, "1 + 3 for the 3 opposing Pokémon");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == C(Psychic).Name));
        }

        [Test]
        public void Invasion_Switches_Dawn_Wings_Necrozma_GX_In_And_Moons_Eclipse_Needs_More_Prizes_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Invasion"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            var necrozma = e.ScenarioBench(0, C(DawnWings));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(necrozma, C(Psychic));
            e.ScenarioActive(1, C(Regice));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(necrozma, e.State.Players[0].Active);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Moon's Eclipse-GX")), "same number of Prize cards left");
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Moon's Eclipse-GX")), "6 left against 5");
        }

        [Test]
        public void Abyssal_Sleep_Makes_The_Opponent_Flip_2_Coins_To_Wake_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Abyssal Sleep"), new ScriptedAgent());
            var darkrai = e.ScenarioActive(0, C(Darkrai));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(darkrai, C(Darkness));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Asleep));
            Assert.AreEqual(2, target.SleepCoins);
            Assert.AreEqual(12, target.DamageCounters);
        }

        [Test]
        public void Evil_Admonition_Only_Counts_Abilities_That_Work()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Evil Admonition"), new ScriptedAgent());
                var weavile = e.ScenarioActive(0, C(Weavile));
                e.ScenarioAttach(weavile, C(Darkness));
                var target = e.ScenarioActive(1, C(Regice));
                e.ScenarioBench(1, C(SilvallyGX));
                e.ScenarioBench(1, C(Shiinotic));
                if (pass == 1) e.ScenarioSetStadium(1, C(PathToThePeak));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(pass == 0 ? 10 : 5, target.DamageCounters,
                    pass == 0 ? "Silvally-GX and Shiinotic" : "Path to the Peak shuts off Silvally-GX's Ability, so it doesn't count (your ruling, v85)");
            }
        }

        [Test]
        public void Earthen_Shield_Stops_Damage_From_Pokemon_With_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Mewtwo));
            var bastiodon = e.ScenarioActive(1, C(Bastiodon));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, bastiodon, 60);
            Assert.AreEqual(4, bastiodon.DamageCounters, "60 - 20 Resistance with basic Energy only");
            e.ScenarioAttach(attacker, C(UnitLPM));
            e.DealAttackDamage(attacker, bastiodon, 60);
            Assert.AreEqual(4, bastiodon.DamageCounters, "no damage from a Pokémon with Special Energy");
        }

        [Test]
        public void Solid_Unit_Protects_Magnemite_Only_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Snubbull));
            var active = e.ScenarioActive(1, C(Magnemite));
            var benched = e.ScenarioBench(1, C(Magnemite));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, benched, 30);
            Assert.AreEqual(0, benched.DamageCounters);
            e.DealAttackDamage(attacker, active, 30);
            Assert.AreEqual(3, active.DamageCounters);
        }

        [Test]
        public void Intimidating_Fang_Takes_30_Off_The_Opposing_Active_Pokemons_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Snubbull));
            var luxray = e.ScenarioActive(1, C(Luxray));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, luxray, 50);
            Assert.AreEqual(2, luxray.DamageCounters);
        }

        [Test]
        public void Moongeist_Beam_Stops_Healing_During_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Moongeist Beam"), new ScriptedAgent());
            var lunala = e.ScenarioActive(0, C(LunalaGX));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(lunala, C(Psychic));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
            NextTurn(e);
            Assert.AreEqual(0, e.Heal(target, 30), "can't be healed");
            NextTurn(e);
            NextTurn(e);
            Assert.AreEqual(30, e.Heal(target, 30), "the effect is over");
        }

        [Test]
        public void Blaster_GX_Turns_All_Your_Prize_Cards_Face_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blaster-GX"), new ScriptedAgent());
            var celesteela = e.ScenarioActive(0, C(Celesteela));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(celesteela, C(Metal));
            e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].FaceUpPrizeIds.Count);
            Assert.AreEqual(0, e.State.Players[1].FaceUpPrizeIds.Count);
            Assert.IsTrue(e.State.Players[0].GxUsed);
        }

        [Test]
        public void Lillie_Draws_Up_To_8_On_Your_First_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lillie"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToHand(0, C(Lillie));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 2);
            Step(e);
            Assert.AreEqual(8, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Looker_Draws_From_The_Bottom_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Looker"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snubbull));
            e.ScenarioActive(1, C(Snubbull));
            e.ScenarioAddToDeck(0, C(Fire), 3);
            e.ScenarioAddToHand(0, C(Looker));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(3, hand.Count);
            Assert.IsTrue(hand.All(c => c.Def.Name == C(Fire).Name), "the 3 Fire Energy at the bottom");
        }

        [Test]
        public void Wild_Crash_Knocks_Out_A_Basic_Pokemon_But_Not_An_Evolution()
        {
            for (int pass = 0; pass < 2; pass++)
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Wild Crash"), new ScriptedAgent());
                var rampardos = e.ScenarioActive(0, C(Rampardos));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(rampardos, C(Fighting));
                var target = e.ScenarioActive(1, C(pass == 0 ? Regice : Pupitar));
                e.ScenarioBench(1, C(Snubbull));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                if (pass == 0) Assert.IsNull(e.State.FindPokemon(target.Uid), "Regice is a Basic Pokémon");
                else
                {
                    Assert.AreEqual(target, e.State.Players[1].Active, "Pupitar is a Stage 1");
                    Assert.AreEqual(0, target.DamageCounters, "Wild Crash has no printed damage");
                }
            }
        }

        [Test]
        public void Power_Huddle_Adds_30_Against_An_Active_Evolution_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Passimian));
            e.ScenarioBench(0, C(Passimian));
            var basic = e.ScenarioActive(1, C(Regice));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, basic, 40);
            Assert.AreEqual(4, basic.DamageCounters, "Regice is a Basic Pokémon");
            e.State.Players[1].Active = null;
            var evolved = e.ScenarioActive(1, C(Pupitar));
            e.DealAttackDamage(attacker, evolved, 40);
            Assert.AreEqual(7, evolved.DamageCounters, "40 + 30 from the Benched Passimian");
        }

        [Test]
        public void Spike_Armor_Puts_6_Damage_Counters_On_The_Next_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spike Armor"), new ScriptedAgent());
            var sandslash = e.ScenarioActive(0, C(AlolanSandslash));
            var opponent = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, opponent.DamageCounters);
            NextTurn(e);
            e.DealAttackDamage(opponent, sandslash, 20);
            Assert.AreEqual(9, opponent.DamageCounters, "3 from Spike Armor's damage, 6 more when it damaged Sandslash");
        }

        [Test]
        public void Weather_Guard_Takes_Away_The_Weakness_Of_Your_Grass_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var attacker = e.ScenarioActive(0, C(Charmander));
            var turtwig = e.ScenarioActive(1, C(Turtwig));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(attacker, turtwig, 30);
            Assert.AreEqual(6, turtwig.DamageCounters, "Weakness to Fire");
            e.ScenarioBench(1, C(Cherrim));
            e.DealAttackDamage(attacker, turtwig, 10);
            Assert.AreEqual(7, turtwig.DamageCounters, "no Weakness with Cherrim in play");
        }

        [Test]
        public void Damage_Transport_Moves_Up_To_4_Counters_From_Each_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Damage Transport"), new ScriptedAgent());
            var drifblim = e.ScenarioActive(0, C(Drifblim));
            e.ScenarioAttach(drifblim, C(Psychic));
            e.ScenarioAttach(drifblim, C(Psychic));
            drifblim.DamageCounters = 5;
            var benched = e.ScenarioBench(0, C(Snubbull));
            benched.DamageCounters = 2;
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, drifblim.DamageCounters);
            Assert.AreEqual(0, benched.DamageCounters);
            Assert.AreEqual(6, target.DamageCounters);
        }

        [Test]
        public void Gold_Rush_Does_30_For_Each_Metal_Energy_Card_Discarded_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gold Rush"), new ScriptedAgent());
            e.ScenarioActive(0, C(AlolanDugtrio));
            for (int i = 0; i < 3; i++) e.ScenarioAddToHand(0, C(Metal));
            var target = e.ScenarioActive(1, C(Celesteela));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "the bot discarded 1");
            Assert.AreEqual(3, target.DamageCounters);
        }
    }
}
