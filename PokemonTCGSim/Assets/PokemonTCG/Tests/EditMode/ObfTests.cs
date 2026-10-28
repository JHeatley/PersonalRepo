using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v39: Obsidian Flames (OBF), with your New Moon and Soothing Lullaby rulings and the engine support the set needed.</summary>
    public class ObfTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string Trevenant = "OBF-12";
        const string DecidueyeEx = "OBF-15";
        const string Tsareena = "OBF-18";
        const string Camerupt = "OBF-32";
        const string Seismitoad = "OBF-52";
        const string Tadbulb = "OBF-75";
        const string Bellibolt = "OBF-78";
        const string ClefableEx = "OBF-82";
        const string Lunatone = "OBF-92";
        const string Solrock = "OBF-93";
        const string Claydol = "OBF-95";
        const string Bonsly = "OBF-110";
        const string Stunfisk = "OBF-112";
        const string KlawfEx = "OBF-120";
        const string Clodsire = "OBF-129";
        const string Houndour = "OBF-131";
        const string RevavroomEx = "OBF-156";
        const string Dratini = "OBF-157";
        const string Dragonair = "OBF-158";
        const string DragoniteEx = "OBF-159";
        const string Altaria = "OBF-160";
        const string Pidgey = "OBF-162";
        const string Kangaskhan = "OBF-165";
        const string Skwovet = "OBF-178";
        const string GreedentEx = "OBF-179";
        const string Brassius = "OBF-187";
        const string Geeta = "OBF-188";
        const string LetterOfEncouragement = "OBF-189";
        const string Ortega = "OBF-190";
        const string PatrolCap = "OBF-191";
        const string LeagueHeadquarters = "OBF-192";
        const string TeamStarGrunt = "OBF-195";
        const string TownStore = "OBF-196";
        const string VengefulPunch = "OBF-197";

        const string Tauros = "MEW-128";
        const string RotomEx = "PFL-29";
        const string LuxuriousCape = "PAR-166";
        const string LivelyStadium = "SSP-180";
        const string JammingTower = "TWM-153";
        const string RiskyRuins = "MEG-127";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private static AttackDef AttackNamed(PokemonInPlay mon, string name)
        {
            return mon.Def.Attacks.First(a => a.Name == name);
        }

        private sealed class FlipLog : IGameObserver
        {
            public readonly List<string> Flips = new List<string>();

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                if (ev.Type == GameEventType.CoinFlip) Flips.Add(ev.Text);
            }
        }

        // ================================================================== Data

        [Test]
        public void Every_Obsidian_Flames_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 230; n++)
                Assert.IsNotNull(db.Get("OBF-" + n), "OBF " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("SVP-103", db.MainPrinting(C("OBF-134")).Key, "Houndoom ex");
            Assert.AreEqual("SVP-104", db.MainPrinting(C("OBF-153")).Key, "Melmetal ex");
            Assert.AreEqual("SVP-39", db.MainPrinting(C("OBF-143")).Key, "Mawile");
            Assert.AreEqual("OBF-131", db.MainPrinting(C("OBF-204")).Key, "Houndour's illustration rare");
            Assert.AreEqual("OBF-156", db.MainPrinting(C("OBF-224")).Key, "Revavroom ex's gold print");
            Assert.IsTrue(C(DragoniteEx).IsTera && C(GreedentEx).IsTera, "Tera ex");
        }

        // ================================================================== Lunatone's New Moon (your ruling: every Stadium effect)

        [Test]
        public void New_Moon_Moves_League_Headquarters_Extra_Cost_To_The_Opponents_Basics()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lunatone = e.ScenarioActive(0, C(Lunatone));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(LeagueHeadquarters));
            e.ScenarioSetTurn(0, 3);
            var moonPress = AttackNamed(lunatone, "Moon Press");
            Assert.AreEqual(4, e.AttackCost(lunatone, moonPress).Count, "Basic Pokémon pay [C] more");

            e.ScenarioBench(0, C(Solrock));
            Assert.AreEqual(3, e.AttackCost(lunatone, moonPress).Count, "New Moon is on with Solrock in play");
            Assert.AreEqual(3, e.AttackCost(tauros, AttackNamed(tauros, "Rage")).Count, "the opponent's Basics still pay more");
        }

        [Test]
        public void New_Moon_Turns_Off_Good_Stadium_Effects_Too_And_Keeps_Tools_Working_Under_Jamming_Tower()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lunatone = e.ScenarioActive(0, C(Lunatone));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(LivelyStadium));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(120, e.MaxHp(lunatone));
            var solrock = e.ScenarioBench(0, C(Solrock));
            Assert.AreEqual(90, e.MaxHp(lunatone), "your ruling: good effects are prevented too");
            Assert.AreEqual(90, e.MaxHp(solrock));
            Assert.AreEqual(150, e.MaxHp(tauros), "the opponent's Basics still get +30");

            var tower = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var luna = tower.ScenarioActive(0, C(Lunatone));
            tower.ScenarioAttachTool(luna, C(LuxuriousCape));
            var t2 = tower.ScenarioActive(1, C(Tauros));
            tower.ScenarioAttachTool(t2, C(LuxuriousCape));
            tower.ScenarioSetStadium(1, C(JammingTower));
            tower.ScenarioSetTurn(0, 3);
            Assert.AreEqual(90, tower.MaxHp(luna), "Jamming Tower turns the Cape off");
            tower.ScenarioBench(0, C(Solrock));
            Assert.AreEqual(190, tower.MaxHp(luna), "with New Moon on, its Tool works again");
            Assert.AreEqual(120, tower.MaxHp(t2), "the opponent's Tool is still off");
        }

        [Test]
        public void New_Moon_Turns_On_At_Once_So_Risky_Ruins_Misses_The_Solrock_Just_Benched()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Solrock"), new ScriptedAgent());
            e.ScenarioActive(0, C(Lunatone));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(RiskyRuins));
            e.ScenarioAddToHand(0, C(Solrock));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench[0].DamageCounters);

            var alone = Scenario(new ScriptedAgent().Then("Bench Solrock"), new ScriptedAgent());
            alone.ScenarioActive(0, C(Tauros));
            alone.ScenarioActive(1, C(Tauros));
            alone.ScenarioSetStadium(1, C(RiskyRuins));
            alone.ScenarioAddToHand(0, C(Solrock));
            alone.ScenarioSetTurn(0, 3);
            Step(alone);
            Assert.AreEqual(2, alone.State.Players[0].Bench[0].DamageCounters, "no Lunatone, no New Moon");
        }

        // ================================================================== Other Abilities

        [Test]
        public void Forest_Miasma_Puts_A_Counter_On_The_Opponents_Active_In_Every_Checkup()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Trevenant));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            NextTurn(e);
            Assert.AreEqual(1, tauros.DamageCounters);
            NextTurn(e);
            Assert.AreEqual(2, tauros.DamageCounters, "the opponent's Checkup too");
        }

        [Test]
        public void Total_Freedom_Switches_Decidueye_In_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Total Freedom"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var decidueye = e.ScenarioBench(0, C(DecidueyeEx));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(decidueye, e.State.Players[0].Active);
            Assert.IsFalse(Offered(e, 0, "Use Total Freedom"), "once during your turn");
        }

        [Test]
        public void Quaking_Zone_Raises_Only_The_Opponents_Active_Attack_Costs_While_Seismitoad_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var toad = e.ScenarioActive(0, C(Seismitoad));
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(3, e.AttackCost(active, AttackNamed(active, "Rage")).Count);
            Assert.AreEqual(2, e.AttackCost(benched, AttackNamed(benched, "Rage")).Count);
            var mine = e.ScenarioBench(0, C(Tauros));
            Assert.AreEqual(2, e.AttackCost(mine, AttackNamed(mine, "Rage")).Count, "only the opponent's");
            e.SwitchActive(0, mine);
            Assert.AreEqual(2, e.AttackCost(active, AttackNamed(active, "Rage")).Count, "Seismitoad is on the Bench");
            Assert.IsNotNull(toad);
        }

        [Test]
        public void Insulator_Stops_Damage_From_Lightning_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var bellibolt = e.ScenarioActive(0, C(Bellibolt));
            var tadbulb = e.ScenarioActive(1, C(Tadbulb));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(tadbulb, bellibolt, 40);
            Assert.AreEqual(0, bellibolt.DamageCounters);
            e.DealAttackDamage(tauros, bellibolt, 30);
            Assert.AreEqual(3, bellibolt.DamageCounters);
        }

        [Test]
        public void Lunar_Zone_Gives_No_Retreat_Cost_To_Your_Pokemon_With_Psychic_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(active, C(Psychic));
            e.ScenarioBench(0, C(ClefableEx));
            var dry = e.ScenarioBench(0, C(Tauros));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttach(theirs, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(0, e.RetreatCostOf(active));
            Assert.AreEqual(2, e.RetreatCostOf(dry), "no Psychic Energy");
            Assert.AreEqual(2, e.RetreatCostOf(theirs), "only your Pokémon");
        }

        [Test]
        public void Counterattacking_Pincer_Discards_An_Energy_From_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioAttach(tauros, C(Fire));
            var klawf = e.ScenarioActive(1, C(KlawfEx));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, klawf, 30);
            Assert.AreEqual(1, tauros.Energy.Count);
        }

        [Test]
        public void Custom_Trap_Needs_A_Tool_On_Stunfisk()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var stunfisk = e.ScenarioActive(1, C(Stunfisk));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, stunfisk, 30);
            Assert.AreEqual(0, tauros.DamageCounters, "no Tool attached");
            e.ScenarioAttachTool(stunfisk, C(LuxuriousCape));
            e.DealAttackDamage(tauros, stunfisk, 30);
            Assert.AreEqual(5, tauros.DamageCounters);
        }

        [Test]
        public void Tune_Up_Lets_Revavroom_ex_Hold_4_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var revavroom = e.ScenarioActive(0, C(RevavroomEx));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioBench(0, C(RotomEx));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(4, e.MaxTools(revavroom), "Multi Adapter's 2 doesn't lower it");
            Assert.AreEqual(1, e.MaxTools(tauros), "only Revavroom ex itself");
        }

        // ================================================================== Attacks

        [Test]
        public void Soothing_Lullaby_Sleep_Flips_2_Coins_Until_It_Wakes_Up()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Soothing Lullaby"), new ScriptedAgent());
            var altaria = e.ScenarioActive(0, C(Altaria));
            e.ScenarioAttach(altaria, C(Water));
            e.ScenarioAttach(altaria, C(Metal));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Asleep));
            Assert.AreEqual(2, tauros.SleepCoins);
            var log = new FlipLog();
            e.AddObserver(log);
            e.EndTurn();
            Assert.AreEqual(2, log.Flips.Count(t => t.Contains("is Asleep (")), "2 coins in Pokémon Checkup");

            var fresh = Scenario(new ScriptedAgent(), new ScriptedAgent());
            fresh.ScenarioActive(0, C(Tauros));
            var t2 = fresh.ScenarioActive(1, C(Tauros));
            fresh.ScenarioSetTurn(0, 3);
            t2.SleepCoins = 2;
            fresh.ApplyCondition(t2, SpecialCondition.Asleep);
            Assert.AreEqual(1, t2.SleepCoins, "a new Asleep from another effect is the normal kind");
        }

        [Test]
        public void Queenly_Heel_Stops_Evolving_From_The_Hand_Until_The_Pokemon_Moves_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Queenly Heel"), new ScriptedAgent());
            var tsareena = e.ScenarioActive(0, C(Tsareena));
            e.ScenarioAttach(tsareena, C("MEE-9"));
            var dratini = e.ScenarioActive(1, C(Dratini));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, dratini.DamageCounters);
            NextTurn(e);
            Assert.AreEqual(1, e.State.CurrentPlayer);
            Assert.IsFalse(e.CanEvolveInto(dratini, C(Dragonair), true), "not from the hand");
            Assert.IsTrue(e.CanEvolveInto(dratini, C(Dragonair), false), "an effect that evolves from the deck still can");
            e.SwitchActive(1, tauros);
            Assert.IsTrue(e.CanEvolveInto(dratini, C(Dragonair), true), "Compendium: moving to the Bench ends it");
        }

        [Test]
        public void Eruption_Does_100_More_For_Each_Energy_Card_Discarded_From_Both_Decks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Eruption"), new ScriptedAgent());
            var camerupt = e.ScenarioActive(0, C(Camerupt));
            e.ScenarioAttach(camerupt, C(Fire));
            e.ScenarioAttach(camerupt, C(Psychic));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(25, dragonite.DamageCounters, "50 + 2 Psychic Energy x100");
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Kaboom_Doll_Leaves_The_Defending_Pokemon_At_10_HP_And_Hits_Claydol_For_120()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Kaboom Doll"), new ScriptedAgent());
            var claydol = e.ScenarioActive(0, C(Claydol));
            e.ScenarioAttach(claydol, C(Psychic));
            e.ScenarioBench(0, C(Tauros));
            var dragonite = e.ScenarioActive(1, C(DragoniteEx));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, e.RemainingHp(dragonite));
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Claydol"), "120 damage Knocks Out the 120-HP Claydol");
        }

        [Test]
        public void Coordinated_Pack_Gives_Each_Benched_Houndour_A_Darkness_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Coordinated Pack", "Darkness Energy|Darkness Energy"), new ScriptedAgent());
            var leader = e.ScenarioActive(0, C(Houndour));
            e.ScenarioAttach(leader, C(Darkness));
            var a = e.ScenarioBench(0, C(Houndour));
            var b = e.ScenarioBench(0, C("OBF-132"));
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToDeck(0, C(Darkness), 3);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, a.Energy.Count);
            Assert.AreEqual(1, b.Energy.Count);
            Assert.AreEqual(1, leader.Energy.Count, "only Benched Houndour");
        }

        [Test]
        public void Splattering_Poison_Poisons_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Splattering Poison"), new ScriptedAgent());
            var clodsire = e.ScenarioActive(0, C(Clodsire));
            e.ScenarioAttach(clodsire, C(Darkness));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(clodsire.Has(SpecialCondition.Poisoned));
        }

        [Test]
        public void Never_Ever_Enough_Takes_All_3_Or_Discards_Them_And_Draws_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Never Ever Enough", "Yes"), new ScriptedAgent());
            var greedent = e.ScenarioActive(0, C(GreedentEx));
            e.ScenarioAttach(greedent, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);

            var no = Scenario(new ScriptedAgent().Then("Attack: Never Ever Enough", "No"), new ScriptedAgent());
            var g2 = no.ScenarioActive(0, C(GreedentEx));
            no.ScenarioAttach(g2, C(Psychic));
            no.ScenarioActive(1, C(Tauros));
            no.ScenarioSetTurn(0, 3);
            Step(no);
            Assert.AreEqual(3, no.State.Players[0].Hand.Count);
            Assert.AreEqual(3, no.State.Players[0].Discard.Count);
            Assert.AreEqual(14, no.State.Players[0].Deck.Count);
        }

        [Test]
        public void League_Headquarters_Makes_A_Free_Attack_Cost_1_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var bonsly = e.ScenarioActive(0, C(Bonsly));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            var blubbering = AttackNamed(bonsly, "Blubbering");
            Assert.AreEqual(0, e.AttackCost(bonsly, blubbering).Count);
            e.ScenarioSetStadium(1, C(LeagueHeadquarters));
            Assert.AreEqual(1, e.AttackCost(bonsly, blubbering).Count, "Compendium: a free attack then needs 1 Energy");
            Assert.IsFalse(Offered(e, 0, "Attack: Blubbering"));
        }

        [Test]
        public void League_Headquarters_Doesnt_Raise_A_Cost_Plasma_Bane_Sets()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kyurem = e.ScenarioActive(0, C("SFA-47"));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(LeagueHeadquarters));
            e.ScenarioSetTurn(0, 3);
            var trifrost = AttackNamed(kyurem, "Trifrost");
            Assert.AreEqual(trifrost.Cost.Length + 1, e.AttackCost(kyurem, trifrost).Count);
            e.ScenarioAddToDiscard(1, C("SFA-57"), 1);
            Assert.AreEqual(1, e.AttackCost(kyurem, trifrost).Count, "Compendium (2024-09-12): a set cost isn't increased");
        }

        // ================================================================== Trainers

        [Test]
        public void Patrol_Cap_Stops_The_Opponent_Discarding_Your_Deck_While_Its_Holder_Is_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nicked Nibble"), new ScriptedAgent());
            var skwovet = e.ScenarioActive(0, C(Skwovet));
            e.ScenarioAttach(skwovet, C(Psychic));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(PatrolCap));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(20, e.State.Players[1].Deck.Count);

            var bench = Scenario(new ScriptedAgent().Then("Attack: Nicked Nibble"), new ScriptedAgent());
            var s2 = bench.ScenarioActive(0, C(Skwovet));
            bench.ScenarioAttach(s2, C(Psychic));
            bench.ScenarioActive(1, C(Tauros));
            var benched = bench.ScenarioBench(1, C(Tauros));
            bench.ScenarioAttachTool(benched, C(PatrolCap));
            bench.ScenarioSetTurn(0, 3);
            Step(bench);
            Assert.AreEqual(19, bench.State.Players[1].Deck.Count, "only while the holder is Active");
        }

        [Test]
        public void Vengeful_Punch_Puts_4_Counters_On_The_Attacker_That_Knocks_Out_Its_Holder()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Headbutt"), new ScriptedAgent());
            var kangaskhan = e.ScenarioActive(0, C(Kangaskhan));
            e.ScenarioAttach(kangaskhan, C(Psychic));
            var pidgey = e.ScenarioActive(1, C(Pidgey));
            pidgey.DamageCounters = 3;
            e.ScenarioAttachTool(pidgey, C(VengefulPunch));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Pidgey"));
            Assert.AreEqual(4, kangaskhan.DamageCounters);
        }

        [Test]
        public void Vengeful_Punch_Does_Nothing_When_The_Holder_Was_Devolved_After_The_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Devolution Ray"), new ScriptedAgent());
            var aerodactyl = e.ScenarioActive(0, C("MEW-142"));
            e.ScenarioAttach(aerodactyl, C(Psychic));
            e.ScenarioAttach(aerodactyl, C(Psychic));
            var dragonair = e.ScenarioActive(1, C(Dratini));
            e.ScenarioEvolve(dragonair, C(Dragonair));
            e.ScenarioAttachTool(dragonair, C(VengefulPunch));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Dragonair"), "devolved before Knock Outs are checked");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Dratini"));
            Assert.AreEqual(0, aerodactyl.DamageCounters, "Compendium: the Dratini left wasn't the Pokémon that took the damage");
        }

        [Test]
        public void Geeta_Attaches_2_Basic_Energy_And_Then_Your_Pokemon_Cant_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Geeta", "Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Geeta));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, tauros.Energy.Count);
            Assert.IsFalse(Offered(e, 0, "Attack:"), "not this turn");
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(Offered(e, 0, "Attack: Rage"), "only during that turn");
        }

        [Test]
        public void Brassius_Draws_The_Rest_Of_Your_Hand_Plus_1()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Brassius"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Brassius));
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count, "4 cards (not Brassius) + 1");
            Assert.AreEqual(19, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Letter_Of_Encouragement_Needs_A_Knock_Out_During_The_Opponents_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(LetterOfEncouragement));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Letter of Encouragement"));
            e.State.Players[0].LastKnockedOutTurn = 2;
            Assert.IsTrue(Offered(e, 0, "Play Letter of Encouragement"));
        }

        [Test]
        public void Ortega_Puts_A_Card_On_The_Bottom_And_The_Opponent_May_Draw()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Ortega", "Pidgey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Ortega));
            e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual("Pidgey", opp.Deck.Last().Def.Name);
            Assert.AreEqual(2, opp.Hand.Count, "Tauros + the card they chose to draw");
        }

        [Test]
        public void Team_Star_Grunt_Puts_An_Energy_On_Top_Of_The_Opponents_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Star Grunt"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(TeamStarGrunt));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Team Star Grunt"), "a Trainer can't be played for no effect");
            e.ScenarioAttach(tauros, C(Fire));
            Step(e);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual("Fire Energy", e.State.Players[1].Deck[0].Def.Name);
        }

        [Test]
        public void Town_Store_Finds_A_Pokemon_Tool_Once_Per_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Town Store"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(PatrolCap), 2);
            e.ScenarioSetStadium(1, C(TownStore));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Patrol Cap"));
            Assert.IsFalse(Offered(e, 0, "Use Town Store"));
        }
    }
}
