using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v55: Fusion Strike (FST): Fusion Strike Energy, Goodra's Slimy Room (your ruling: every Energy card attached from the
    /// hand), Skaters' Park, the cards played 2 at once (your ruling: 2 Item cards), and the other cards with new effects.
    /// </summary>
    public class FstTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fire = "MEE-10";
        const string Fighting = "MEE-14";

        const string Tauros = "MEW-128";
        const string MawileV = "SIT-70";
        const string Deoxys = "CRZ-GG12";
        const string GreedentV = "CRZ-120";
        const string Latias = "CRZ-GG20";

        const string Accelgor = "FST-14";
        const string Ninetales = "FST-31";
        const string ChandelureVmax = "FST-40";
        const string CinderaceV = "FST-44";
        const string Sizzlipede = "FST-47";
        const string Starmie = "FST-53";
        const string Croconaw = "FST-56";
        const string Feraligatr = "FST-57";
        const string Swampert = "FST-64";
        const string Huntail = "FST-66";
        const string Gorebyss = "FST-67";
        const string GalarianDarmanitan = "FST-72";
        const string Pyukumuku = "FST-77";
        const string InteleonVmax = "FST-79";
        const string GalarianCursola = "FST-118";
        const string Meloetta = "FST-124";
        const string Dreepy = "FST-128";
        const string Golem = "FST-137";
        const string Hariyama = "FST-143";
        const string GenesectV = "FST-185";
        const string Togedemaru = "FST-187";
        const string Goodra = "FST-197";
        const string Turtonator = "FST-198";
        const string Blissey = "FST-203";
        const string GreedentVmax = "FST-218";
        const string Wooloo = "FST-221";
        const string BattleVipPass = "FST-225";
        const string CrossSwitcher = "FST-230";
        const string Crossceiver = "FST-231";
        const string Dancer = "FST-232";
        const string FarewellBell = "FST-234";
        const string PowerTablet = "FST-236";
        const string QuickBall = "FST-237";
        const string Schoolboy = "FST-238";
        const string Schoolgirl = "FST-239";
        const string SkatersPark = "FST-242";
        const string SpongyGloves = "FST-243";
        const string FusionStrikeEnergy = "FST-244";
        const string EspeonVmax = "FST-270";
        const string TrainingCourt = "FST-282";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        /// <summary>A seed whose first coin flips come up as given (true = heads), to set the game's random numbers right before a flip.</summary>
        private static int SeedFor(params bool[] flips)
        {
            for (int seed = 1; seed < 100000; seed++)
            {
                var rng = new Rng(seed);
                bool match = true;
                foreach (var heads in flips)
                    if (rng.CoinFlip() != heads) { match = false; break; }
                if (match) return seed;
            }
            throw new InvalidOperationException("no seed for those flips");
        }

        // ================================================================== Data

        [Test]
        public void Every_Fusion_Strike_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 284; n++) Assert.IsNotNull(db.Get("FST-" + n), "FST " + n + " is missing");
            Assert.AreEqual("CRZ-GG12", db.MainPrinting(C("FST-120")).Key, "Deoxys was already in the pool");
            Assert.AreEqual("30C-CC28", db.MainPrinting(C("FST-114")).Key, "and Mew VMAX");
            Assert.AreEqual("FST-21", db.MainPrinting(C("FST-246")).Key, "the full art is a second print of Tsareena V");
            Assert.AreEqual("FST-185", db.MainPrinting(C("FST-255")).Key);
            Assert.AreEqual("MEE-9", db.MainPrinting(C("FST-283")).Key, "the gold Grass Energy");
            Assert.IsTrue(C(Accelgor).IsFusionStrike);
            Assert.IsTrue(C(CinderaceV).IsSingleStrike);
            Assert.IsTrue(C(InteleonVmax).IsRapidStrike);
            Assert.IsTrue(C(FusionStrikeEnergy).IsSpecialEnergy && C(FusionStrikeEnergy).IsFusionStrike);
            Assert.IsTrue(C(SpongyGloves).IsTool, "a Pokémon Tool");
            Assert.IsFalse(C(SpongyGloves).IsItem, "not an Item");
            Assert.IsFalse(Formats.IsStandardCard(C(GenesectV)), "regulation mark E: Expanded");
        }

        [Test]
        public void Deoxys_Is_A_Single_Rapid_And_Fusion_Strike_Pokemon()
        {
            var deoxys = C(Deoxys);
            Assert.IsTrue(deoxys.IsSingleStrike);
            Assert.IsTrue(deoxys.IsRapidStrike);
            Assert.IsTrue(deoxys.IsFusionStrike);
            Assert.IsFalse(C(Tauros).IsFusionStrike);
        }

        // ================================================================== Fusion Strike Energy

        [Test]
        public void Fusion_Strike_Energy_Attaches_Only_To_Fusion_Strike_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meloetta = e.ScenarioActive(0, C(Meloetta));
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(FusionStrikeEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            var attach = e.GetMainActions(0).Where(o => o.Action == ActionType.AttachEnergy).ToList();
            Assert.AreEqual(1, attach.Count);
            Assert.AreEqual(meloetta.Uid, attach[0].PokemonUid);
        }

        [Test]
        public void Fusion_Strike_Energy_Provides_Every_Type_One_At_A_Time()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var latias = e.ScenarioActive(0, C(Latias));
            e.ScenarioActive(1, C(Tauros));
            var dynaBarrier = C(Latias).Attacks[0];
            Attach(e, latias, FusionStrikeEnergy, 2);
            Assert.IsFalse(e.CanPayAttack(latias, dynaBarrier), "[R][P][C] needs 3 Energy");
            Attach(e, latias, FusionStrikeEnergy, 1);
            Assert.IsTrue(e.CanPayAttack(latias, dynaBarrier));
        }

        [Test]
        public void Fusion_Strike_Energy_On_Another_Pokemon_Is_Discarded()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(tauros, C(FusionStrikeEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.StateChecks();
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual("Fusion Strike Energy", e.State.Players[0].Discard.Single().Def.Name);
        }

        [Test]
        public void Fusion_Strike_Energy_Prevents_The_Effects_Of_The_Opponents_Abilities()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Double Gunner"), new ScriptedAgent());
            e.ScenarioActive(0, C(InteleonVmax));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            var meloetta = e.ScenarioBench(1, C(Meloetta));
            e.ScenarioAttach(meloetta, C(FusionStrikeEnergy));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, meloetta.DamageCounters, "Double Gunner's damage counters are an effect of an Ability");
            Assert.AreEqual(2, tauros.DamageCounters);
            Assert.AreEqual("Water Energy", e.State.Players[0].Discard.Single().Def.Name, "the Water Energy card was discarded to use it");
        }

        [Test]
        public void Fusion_Strike_Energy_Keeps_Abilities_Against_The_Opponents_Ability_Blockers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var genesect = e.ScenarioActive(0, C(GenesectV));
            e.ScenarioAttach(genesect, C(FusionStrikeEnergy));
            var bare = e.ScenarioBench(0, C(GenesectV));
            var klefki = e.ScenarioActive(1, C("PAF-159"));
            e.ScenarioBench(1, C("PAF-158"));
            Assert.IsTrue(e.AbilityBlocked(bare, "Fusion Strike System"), "Fettered in Misfortune / Mischievous Lock");
            Assert.IsFalse(e.AbilityBlocked(genesect, "Fusion Strike System"), "Compendium (Fettered in Misfortune) and your ruling (every other one)");
            e.State.Players[1].Bench.Clear();
            Assert.IsTrue(e.AbilityBlocked(bare, "Fusion Strike System"), "Mischievous Lock alone");
            Assert.IsFalse(e.AbilityBlocked(genesect, "Fusion Strike System"));
            Assert.AreEqual("Klefki", klefki.Name);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(0, C("PAF-158"));
            Assert.IsTrue(e.AbilityBlocked(genesect, "Fusion Strike System"), "your own Spiritomb: the Energy only stops the opponent's Abilities");
        }

        // ================================================================== Slimy Room (your ruling)

        [Test]
        public void Slimy_Room_Discards_The_Energy_On_Tails_Without_Using_Up_The_Attachment()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Goodra));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual("Water Energy", e.State.Players[0].Discard.Single().Def.Name);
            Assert.IsFalse(e.State.Players[0].EnergyAttachedThisTurn, "it doesn't use up the Energy attachment for the turn");
        }

        [Test]
        public void Slimy_Room_Also_Flips_For_Energy_Attached_From_The_Hand_By_An_Ability()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Muddy Maker"), new ScriptedAgent());
            var swampert = e.ScenarioActive(0, C(Swampert));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Goodra));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual(0, swampert.Energy.Count, "your ruling: every Energy card attached from the hand flips");
            Assert.AreEqual("Water Energy", e.State.Players[0].Discard.Single().Def.Name);
        }

        [Test]
        public void Slimy_Room_Works_Only_From_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Water Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Water));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Goodra));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
        }

        // ================================================================== Skaters' Park

        [Test]
        public void Skaters_Park_Puts_Basic_Energy_Paid_To_Retreat_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            var bench = e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(SkatersPark));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(bench, e.State.Players[0].Active);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Name == "Psychic Energy"));
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        // ================================================================== Played 2 at once (your ruling)

        [Test]
        public void Cross_Switcher_Needs_A_Second_Copy_And_Switches_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cross Switcher"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var myBench = e.ScenarioBench(0, C(Meloetta));
            e.ScenarioAddToHand(0, C(CrossSwitcher));
            e.ScenarioActive(1, C(Tauros));
            var theirBench = e.ScenarioBench(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Cross Switcher"), "1 copy can't be played");
            e.ScenarioAddToHand(0, C(CrossSwitcher));
            Assert.IsTrue(Offered(e, 0, "Play Cross Switcher"));
            Step(e);
            Assert.AreSame(theirBench, e.State.Players[1].Active);
            Assert.AreSame(myBench, e.State.Players[0].Active);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Cross Switcher"));
        }

        [Test]
        public void A_Pair_Flips_For_Each_Card_When_Trainers_Need_A_Flip()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cross Switcher"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Meloetta));
            e.ScenarioAddToHand(0, C(CrossSwitcher));
            e.ScenarioAddToHand(0, C(CrossSwitcher));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new TrainerFlipEffect("Quaking Fist", 1, 0, 4));
            e.State.Rng = new Rng(SeedFor(true, false));
            Step(e);
            Assert.AreSame(theirs, e.State.Players[1].Active, "your ruling: the second card is an Item too; tails, so the pair does nothing");
            Assert.AreSame(mine, e.State.Players[0].Active);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Cross Switcher"));
        }

        [Test]
        public void Crossceiver_Pair_Puts_A_Pokemon_From_The_Discard_Pile_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Crossceiver"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Crossceiver));
            e.ScenarioAddToHand(0, C(Crossceiver));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Crossceiver"), "nothing to take back");
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            Step(e);
            Assert.AreEqual("Tauros", e.State.Players[0].Hand.Single().Def.Name);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Crossceiver"));
        }

        // ================================================================== Trainers

        [Test]
        public void Power_Tablet_Boosts_Only_Fusion_Strike_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Power Tablet"), new ScriptedAgent());
            var meloetta = e.ScenarioActive(0, C(Meloetta));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(PowerTablet));
            var mawile = e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.DealAttackDamage(meloetta, mawile, 70);
            Assert.AreEqual(10, mawile.DamageCounters);
            e.DealAttackDamage(tauros, mawile, 30);
            Assert.AreEqual(13, mawile.DamageCounters, "Tauros isn't a Fusion Strike Pokémon");
        }

        [Test]
        public void Battle_VIP_Pass_Works_Only_On_Your_First_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Battle VIP Pass", "Tauros|Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(BattleVipPass));
            e.ScenarioAddToDeck(0, C(Tauros), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Battle VIP Pass"));
            e.ScenarioSetTurn(0, 1);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count);
        }

        [Test]
        public void Dancer_Draws_5_On_The_First_Turn_Of_The_Player_Going_Second()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Dancer"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Dancer));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 2);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Dancer_Draws_2_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Dancer"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Dancer));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 4);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Schoolgirl_And_Schoolboy_Look_At_The_Opponents_Prize_Count()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Schoolgirl"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Schoolgirl));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count, "6 Prize cards left: even");

            var f = Scenario(new ScriptedAgent().Then("Play Schoolboy"), new ScriptedAgent());
            f.ScenarioActive(0, C(Tauros));
            f.ScenarioAddToHand(0, C(Schoolboy));
            f.ScenarioActive(1, C(Tauros));
            f.ScenarioSetTurn(0, 3);
            Step(f);
            Assert.AreEqual(2, f.State.Players[0].Hand.Count);
        }

        [Test]
        public void Quick_Ball_Discards_A_Card_And_Finds_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Quick Ball"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(QuickBall));
            e.ScenarioAddToDeck(0, C(Meloetta), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Quick Ball"), "it needs another card to discard");
            e.ScenarioAddToHand(0, C(Psychic));
            Step(e);
            Assert.AreEqual("Meloetta", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Psychic Energy"));
        }

        [Test]
        public void Training_Court_Returns_A_Basic_Energy_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Training Court"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Psychic), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(TrainingCourt));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Psychic Energy", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsFalse(Offered(e, 0, "Use Training Court"), "once during each player's turn");
        }

        [Test]
        public void Farewell_Bell_Searches_When_Its_Pokemon_VMAX_Is_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var greedent = e.ScenarioActive(1, C(GreedentVmax));
            e.ScenarioAttachTool(greedent, C(FarewellBell));
            e.ScenarioBench(1, C(Tauros));
            greedent.DamageCounters = 31;
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, greedent, 30);
            e.ResolveKnockouts();
            Assert.AreEqual(1, e.State.Players[1].Hand.Count);
            Assert.AreEqual(19, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Cursed_Shimmer_Stops_The_Opponent_Playing_Tools_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var gloves = e.ScenarioAddToHand(0, C(SpongyGloves));
            var chandelure = e.ScenarioActive(1, C(ChandelureVmax));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.CanPlayTrainer(0, gloves));
            e.State.Players[1].Active = tauros;
            e.State.Players[1].Bench.Remove(tauros);
            e.State.Players[1].Bench.Add(chandelure);
            Assert.IsTrue(e.CanPlayTrainer(0, gloves), "only from the Active Spot");
        }

        // ================================================================== Abilities

        [Test]
        public void Single_Strike_Jammer_Raises_The_Opponents_Single_Strike_Attack_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Huntail));
            var mine = e.ScenarioBench(0, C(CinderaceV));
            var cinderace = e.ScenarioActive(1, C(CinderaceV));
            var deoxys = e.ScenarioBench(1, C(Deoxys));
            var flare = C(CinderaceV).Attacks[0];
            var photonBoost = C(Deoxys).Attacks[0];
            Assert.AreEqual(3, e.AttackCost(cinderace, flare).Count);
            Assert.AreEqual(2, e.AttackCost(mine, flare).Count, "not your own");
            Assert.AreEqual(4, e.AttackCost(deoxys, photonBoost).Count, "Deoxys is a Single Strike Pokémon too");
            e.ScenarioAttach(deoxys, C(FusionStrikeEnergy));
            Assert.AreEqual(3, e.AttackCost(deoxys, photonBoost).Count, "Fusion Strike Energy keeps the opponent's Abilities off it");
        }

        [Test]
        public void Rapid_Strike_Canceler_Takes_Away_The_Opponents_Rapid_Strike_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var gorebyss = e.ScenarioBench(0, C(Gorebyss));
            var mine = e.ScenarioBench(0, C(InteleonVmax));
            var inteleon = e.ScenarioActive(1, C(InteleonVmax));
            Assert.IsTrue(e.AbilityBlocked(inteleon, "Double Gunner"));
            Assert.IsFalse(e.AbilityBlocked(mine, "Double Gunner"), "only the opponent's");
            e.State.Players[0].Bench.Remove(gorebyss);
            Assert.IsFalse(e.AbilityBlocked(inteleon, "Double Gunner"));
        }

        [Test]
        public void Guts_Leaves_Hariyama_At_10_HP_On_Heads()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var hariyama = e.ScenarioActive(1, C(Hariyama));
            hariyama.DamageCounters = 10;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(true));
            e.DealAttackDamage(tauros, hariyama, 50);
            Assert.IsFalse(e.IsKnockedOut(hariyama));
            Assert.AreEqual(10, e.RemainingHp(hariyama));
        }

        [Test]
        public void Guts_Doesnt_Save_Hariyama_On_Tails()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var hariyama = e.ScenarioActive(1, C(Hariyama));
            hariyama.DamageCounters = 10;
            e.ScenarioSetTurn(0, 3);
            e.State.Rng = new Rng(SeedFor(false));
            e.DealAttackDamage(tauros, hariyama, 50);
            Assert.IsTrue(e.IsKnockedOut(hariyama));
        }

        [Test]
        public void Desperate_Blast_Puts_10_Damage_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var golem = e.ScenarioActive(1, C(Golem));
            e.ScenarioBench(1, C(Tauros));
            golem.DamageCounters = 17;
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, golem, 30);
            e.ResolveKnockouts();
            Assert.AreEqual(10, tauros.DamageCounters);
        }

        [Test]
        public void Expert_In_Roundness_Stops_Damage_From_Pokemon_VMAX_To_Rollout_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var togedemaru = e.ScenarioActive(0, C(Togedemaru));
            e.ScenarioBench(0, C(Blissey));
            var tauros = e.ScenarioBench(0, C(Tauros));
            var greedent = e.ScenarioActive(1, C(GreedentVmax));
            var foeTauros = e.ScenarioBench(1, C(Tauros));
            Assert.AreEqual(0, e.DealAttackDamage(greedent, togedemaru, 100));
            Assert.AreEqual(30, e.DealAttackDamage(greedent, tauros, 30, false), "Tauros doesn't have Let's All Rollout");
            Assert.AreEqual(30, e.DealAttackDamage(foeTauros, togedemaru, 30, false), "only Pokémon VMAX");
        }

        [Test]
        public void Lets_All_Rollout_Counts_Your_Benched_Pokemon_With_The_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Let's All Rollout"), new ScriptedAgent());
            var togedemaru = e.ScenarioActive(0, C(Togedemaru));
            Attach(e, togedemaru, Psychic, 2);
            e.ScenarioBench(0, C(Wooloo));
            e.ScenarioBench(0, C(Wooloo));
            e.ScenarioBench(0, C(Tauros));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, foe.DamageCounters, "2 Wooloo: 40");
        }

        [Test]
        public void Byway_Of_The_Nine_Tailed_Fox_Lowers_Retreat_With_Fire_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Ninetales));
            var foe = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(2, e.RetreatCostOf(tauros));
            e.ScenarioAttach(tauros, C(Fire));
            e.ScenarioAttach(foe, C(Fire));
            Assert.AreEqual(0, e.RetreatCostOf(tauros));
            Assert.AreEqual(2, e.RetreatCostOf(foe), "only your own Pokémon");
        }

        [Test]
        public void Solar_Revelation_Stops_Attack_Effects_On_Pokemon_With_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Singe", "Attack: Singe"));
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioBench(0, C(EspeonVmax));
            var sizzlipede = e.ScenarioActive(1, C(Sizzlipede));
            e.ScenarioAttach(sizzlipede, C(Fire));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsFalse(tauros.Has(SpecialCondition.Burned));
            tauros.Energy.Clear();
            e.ScenarioSetTurn(1, 6);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Burned), "no Energy attached");
        }

        [Test]
        public void Fusion_Strike_System_Draws_Up_To_The_Fusion_Strike_Pokemon_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Fusion Strike System"), new ScriptedAgent());
            e.ScenarioActive(0, C(GenesectV));
            e.ScenarioBench(0, C(Meloetta));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "3 Fusion Strike Pokémon");
        }

        [Test]
        public void Pitch_A_Pyukumuku_Puts_It_On_The_Bottom_And_Draws_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Pitch a Pyukumuku"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Pyukumuku));
            e.ScenarioAddToHand(0, C(Pyukumuku));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Pyukumuku", e.State.Players[0].Deck.Last().Def.Name);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.IsFalse(Offered(e, 0, "Pitch a Pyukumuku"), "only 1 each turn");
        }

        [Test]
        public void Rowdy_Discards_The_Top_5_Of_Your_Deck_On_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var croconaw = e.ScenarioBench(0, C(Croconaw));
            e.ScenarioAddToHand(0, C(Feraligatr));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual("Feraligatr", croconaw.Name);
            Assert.AreEqual(15, e.State.Players[0].Deck.Count);
            Assert.AreEqual(20, e.State.Players[1].Deck.Count);
        }

        // ================================================================== Attacks

        [Test]
        public void Ninja_Tornado_Costs_Grass_After_Accelgor_Moves_Up()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var accelgor = e.ScenarioActive(0, C(Accelgor));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            var tornado = C(Accelgor).Attacks[0];
            Assert.AreEqual(3, e.AttackCost(accelgor, tornado).Count);
            accelgor.MovedToActiveOnTurn = 3;
            var cost = e.AttackCost(accelgor, tornado);
            Assert.AreEqual(1, cost.Count);
            Assert.AreEqual(EnergyType.Grass, cost[0]);
        }

        [Test]
        public void Daruma_Headbutt_Costs_Water_When_Darmanitan_Is_Damaged()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var darmanitan = e.ScenarioActive(0, C(GalarianDarmanitan));
            e.ScenarioActive(1, C(Tauros));
            var headbutt = C(GalarianDarmanitan).Attacks[1];
            Assert.AreEqual(3, e.AttackCost(darmanitan, headbutt).Count);
            darmanitan.DamageCounters = 1;
            Assert.AreEqual(1, e.AttackCost(darmanitan, headbutt).Count);
        }

        [Test]
        public void Turn_A_Profit_Takes_2_More_Prizes_For_A_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Turn a Profit"), new ScriptedAgent());
            var greedent = e.ScenarioActive(0, C(GreedentVmax));
            Attach(e, greedent, Psychic, 2);
            var foe = e.ScenarioActive(1, C(Tauros));
            foe.DamageCounters = 10;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Prizes.Count, "1 Prize card and 2 more");
        }

        [Test]
        public void Force_Regeneration_Benches_The_Opponents_Pokemon_V_At_30_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Force Regeneration"), new ScriptedAgent());
            var cursola = e.ScenarioActive(0, C(GalarianCursola));
            e.ScenarioAttach(cursola, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(1, C(GreedentV), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var greedent = e.State.Players[1].Bench.Single();
            Assert.AreEqual("Greedent V", greedent.Name);
            Assert.AreEqual(30, e.RemainingHp(greedent));
        }

        [Test]
        public void Multishot_Star_Does_30_For_Each_Water_Energy_Discarded()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Multishot Star", "Water Energy|Water Energy"), new ScriptedAgent());
            var starmie = e.ScenarioActive(0, C(Starmie));
            Attach(e, starmie, Water, 2);
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, starmie.Energy.Count);
            Assert.AreEqual(6, foe.DamageCounters);
        }

        [Test]
        public void Shell_Trap_Puts_8_Damage_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shell Trap"), new ScriptedAgent());
            var turtonator = e.ScenarioActive(0, C(Turtonator));
            e.ScenarioAttach(turtonator, C(Fire));
            e.ScenarioAttach(turtonator, C(Fighting));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, foe.DamageCounters);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(foe, turtonator, 30);
            Assert.AreEqual(11, foe.DamageCounters);
        }
    }
}
