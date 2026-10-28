using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v54: Brilliant Stars (BRS) and its Trainer Gallery: Spectral Breach, Blunder Policy, Café Master and Additional Order,
    /// the Style Mustards (your rulings), Emergency Surfacing from the discard pile, and the other cards with new effects.
    /// </summary>
    public class BrsTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";

        const string Tauros = "MEW-128";
        const string MawileV = "SIT-70";
        const string CorviknightVmax = "SIT-TG19";
        const string BossOrders = "MEG-114";
        const string DoubleTurboEnergy = "ASR-216";

        const string Breloom = "BRS-4";
        const string Tropius = "BRS-5";
        const string Torkoal = "BRS-23";
        const string Infernape = "BRS-26";
        const string Staryu = "BRS-30";
        const string Empoleon = "BRS-37";
        const string RaichuV = "BRS-45";
        const string Mewtwo = "BRS-56";
        const string Dusknoir = "BRS-62";
        const string WhimsicottVstar = "BRS-65";
        const string Dedenne = "BRS-67";
        const string MimikyuV = "BRS-68";
        const string MimikyuVmax = "BRS-69";
        const string Alcremie = "BRS-71";
        const string Muk = "BRS-85";
        const string HonchkrowV = "BRS-88";
        const string MorpekoV = "BRS-95";
        const string AggronVmax = "BRS-97";
        const string Escavalier = "BRS-101";
        const string Klinklang = "BRS-104";
        const string Gabite = "BRS-108";
        const string Garchomp = "BRS-109";
        const string Tornadus = "BRS-126";
        const string AcerolasPremonition = "BRS-129";
        const string BlunderPolicy = "BRS-131";
        const string CafeMaster = "BRS-133";
        const string CleansingGloves = "BRS-136";
        const string Gloria = "BRS-141";
        const string Kindler = "BRS-143";
        const string MagmaBasin = "BRS-144";
        const string PotHelmet = "BRS-146";
        const string RoseannesBackup = "BRS-148";
        const string TeamYellsCheer = "BRS-149";
        const string PikachuV = "BRS-157";
        const string DusknoirTG = "BRS-TG6";
        const string RapidStrikeUrshifuV = "BRS-TG20";
        const string RapidStrikeMustard = "BRS-TG27";
        const string SingleStrikeMustard = "BRS-TG28";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        // ================================================================== Data

        [Test]
        public void Every_Brilliant_Stars_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 186; n++) Assert.IsNotNull(db.Get("BRS-" + n), "BRS " + n + " is missing");
            for (int n = 1; n <= 30; n++) Assert.IsNotNull(db.Get("BRS-TG" + n), "BRS TG" + n + " is missing");
            Assert.AreEqual("CRZ-18", db.MainPrinting(C("BRS-17")).Key, "Charizard V was already in the pool");
            Assert.AreEqual("ASR-216", db.MainPrinting(C("BRS-151")).Key, "and Double Turbo Energy");
            Assert.AreEqual("BRS-13", db.MainPrinting(C("BRS-152")).Key, "the full art is a second print of Shaymin V");
            Assert.AreEqual("BRS-68", db.MainPrinting(C("BRS-TG16")).Key);
            Assert.AreEqual("BRS-TG19", db.MainPrinting(C("BRS-TG29")).Key);
            Assert.IsTrue(C(WhimsicottVstar).Attacks[1].VstarPower, "Fluffball Star");
            Assert.AreEqual(0, C(WhimsicottVstar).Attacks[1].Damage, "Fluffball Star has no printed damage");
            Assert.IsTrue(C("BRS-TG3").IsRapidStrike, "Octillery is a Rapid Strike Pokémon");
            Assert.IsTrue(C("BRS-TG10").IsSingleStrike, "Houndoom is a Single Strike Pokémon");
            Assert.IsTrue(C(RapidStrikeMustard).IsRapidStrike);
            Assert.IsFalse(Formats.IsStandardCard(C(MimikyuV)), "regulation mark F: Expanded");
        }

        // ================================================================== Spectral Breach (your ruling)

        [Test]
        public void Spectral_Breach_Makes_Special_Energy_Give_One_Colorless_For_Both_Players()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            e.ScenarioAttach(mawile, C(DoubleTurboEnergy));
            var foe = e.ScenarioActive(1, C(MawileV));
            e.ScenarioAttach(foe, C(DoubleTurboEnergy));
            Assert.AreEqual(2, e.EnergyCount(mawile));
            e.ScenarioBench(1, C(DusknoirTG));
            Assert.AreEqual(1, e.EnergyCount(mawile));
            Assert.AreEqual(1, e.EnergyCount(foe), "its owner's Special Energy too");
        }

        [Test]
        public void Spectral_Breach_Doesnt_Reach_A_Pokemon_Shielded_From_The_Opponents_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var corviknight = e.ScenarioActive(0, C(CorviknightVmax));
            e.ScenarioAttach(corviknight, C(DoubleTurboEnergy));
            var mawile = e.ScenarioBench(0, C(MawileV));
            e.ScenarioAttach(mawile, C(DoubleTurboEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(DusknoirTG));
            Assert.AreEqual(2, e.EnergyCount(corviknight), "your ruling: Lustrous Body keeps its Special Energy working");
            Assert.AreEqual(1, e.EnergyCount(mawile));
        }

        // ================================================================== Blunder Policy (your ruling)

        [Test]
        public void Blunder_Policy_Draws_3_At_The_End_Of_The_Turn_After_A_Tails()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Double Spin"), new ScriptedAgent());
            var staryu = e.ScenarioActive(0, C(Staryu));
            Attach(e, staryu, Psychic, 1);
            e.ScenarioAttachTool(staryu, C(BlunderPolicy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "not yet");
            e.EndTurn();
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Blunder_Policy_Needs_A_Coin_Flip_In_The_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Life Sucker"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(Mewtwo));
            Attach(e, mewtwo, Psychic, 1);
            e.ScenarioAttachTool(mewtwo, C(BlunderPolicy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            e.EndTurn();
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        // ================================================================== Café Master and Additional Order

        [Test]
        public void Cafe_Master_Attaches_Different_Types_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Café Master", "Tauros|Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var first = e.ScenarioBench(0, C(Tauros));
            var second = e.ScenarioBench(0, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Grass)));
            e.ScenarioAddToHand(0, C(CafeMaster));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "Café Master ends the turn");
            Assert.AreEqual(1, first.Energy.Count);
            Assert.AreEqual(1, second.Energy.Count);
            Assert.AreNotEqual(first.Energy[0].Def.EnergyProvides, second.Energy[0].Def.EnergyProvides, "a different type for each");
        }

        [Test]
        public void Additional_Order_Keeps_The_Turn_Going_After_Cafe_Master()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Café Master"), new ScriptedAgent());
            e.ScenarioActive(0, C(Alcremie));
            var bench = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAddToHand(0, C(CafeMaster));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep(), "Alcremie is in the Active Spot");
            Assert.AreEqual(1, bench.Energy.Count);
        }

        // ================================================================== Style Mustards (your ruling)

        [Test]
        public void Single_Strike_Style_Mustard_Draws_5_Even_When_The_Search_Finds_Nothing()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Single Strike Style Mustard"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(SingleStrikeMustard));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count, "your ruling: you searched, so you draw 5");
        }

        [Test]
        public void Single_Strike_Style_Mustard_Needs_An_Empty_Hand_And_Room_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(SingleStrikeMustard));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Single Strike Style Mustard"));
            for (int i = 0; i < 5; i++) e.ScenarioBench(0, C(Tauros));
            Assert.IsFalse(Offered(e, 0, "Play Single Strike Style Mustard"), "your ruling: a full Bench means no search");
            e.State.Players[0].Bench.RemoveAt(0);
            e.ScenarioAddToHand(0, C(Grass));
            Assert.IsFalse(Offered(e, 0, "Play Single Strike Style Mustard"), "only as the last card in your hand");
        }

        [Test]
        public void Rapid_Strike_Style_Mustard_Benches_A_Rapid_Strike_Pokemon_From_The_Discard_And_Draws_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rapid Strike Style Mustard"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(RapidStrikeUrshifuV), 1);
            e.ScenarioAddToHand(0, C(RapidStrikeMustard));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Rapid Strike Urshifu V", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        // ================================================================== Abilities

        [Test]
        public void Emergency_Surfacing_Benches_Empoleon_From_The_Discard_Pile_With_An_Empty_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Emergency Surfacing"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Empoleon), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Emergency Surfacing"));
            Step(e);
            Assert.AreEqual("Empoleon", e.State.Players[0].Bench.Single().Name);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
            Assert.IsFalse(Offered(e, 0, "Emergency Surfacing"), "the hand isn't empty any more");
        }

        [Test]
        public void Curative_Bower_Stops_Confusion_On_Pokemon_With_Grass_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Grass, 1);
            e.ScenarioBench(0, C(Tropius));
            var foe = e.ScenarioActive(1, C(Tauros));
            Attach(e, foe, Grass, 1);
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.ApplyCondition(tauros, SpecialCondition.Confused));
            Assert.IsTrue(e.ApplyCondition(foe, SpecialCondition.Confused), "only your own Pokémon");
            tauros.Conditions = SpecialCondition.Confused;
            e.EndTurn();
            Assert.IsFalse(tauros.Has(SpecialCondition.Confused), "it recovers");
        }

        [Test]
        public void Sludge_Street_Raises_The_Retreat_Cost_Of_The_Opponents_Poisoned_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Muk));
            var foe = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(2, e.RetreatCostOf(foe));
            foe.Conditions = SpecialCondition.Poisoned;
            mine.Conditions = SpecialCondition.Poisoned;
            Assert.AreEqual(3, e.RetreatCostOf(foe));
            Assert.AreEqual(2, e.RetreatCostOf(mine), "not your own");
        }

        [Test]
        public void Boss_Pockets_Lets_Honchkrow_V_Have_4_Tools()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var honchkrow = e.ScenarioActive(0, C(HonchkrowV));
            var tauros = e.ScenarioBench(0, C(Tauros));
            Assert.AreEqual(4, e.MaxTools(honchkrow));
            Assert.AreEqual(1, e.MaxTools(tauros));
        }

        [Test]
        public void Miraculous_Armor_Takes_100_Less_From_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var escavalier = e.ScenarioActive(0, C(Escavalier));
            var mawile = e.ScenarioActive(1, C(MawileV));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.DealAttackDamage(mawile, escavalier, 150);
            Assert.AreEqual(5, escavalier.DamageCounters);
            e.DealAttackDamage(tauros, escavalier, 30);
            Assert.AreEqual(8, escavalier.DamageCounters, "not from other Pokémon");
        }

        [Test]
        public void Gear_Wall_Protects_Your_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var muk = e.ScenarioBench(0, C(Muk));
            e.ScenarioBench(0, C(Klinklang));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.DealAttackDamage(foe, tauros, 30);
            Assert.AreEqual(1, tauros.DamageCounters);
            e.DealAttackDamage(foe, muk, 30, false);
            Assert.AreEqual(3, muk.DamageCounters, "Muk is a Stage 1");
        }

        [Test]
        public void Dummy_Doll_Protects_Mimikyu_V_Played_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Mimikyu V"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(MimikyuV));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var mimikyu = e.State.Players[0].Bench.Single();
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(0, e.DealAttackDamage(foe, mimikyu, 100));
            Assert.AreEqual(0, mimikyu.DamageCounters);
        }

        [Test]
        public void Sonic_Slip_Protects_Garchomp_When_It_Evolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var gabite = e.ScenarioBench(0, C(Gabite));
            e.ScenarioAddToHand(0, C(Garchomp));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Garchomp", gabite.Name);
            e.ScenarioSetTurn(1, 4);
            Assert.AreEqual(0, e.DealAttackDamage(foe, gabite, 100));
        }

        [Test]
        public void Sudden_Cyclone_Has_The_Opponent_Switch_Their_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Tornadus"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Tornadus));
            e.ScenarioActive(1, C(Tauros));
            var mawile = e.ScenarioBench(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(mawile, e.State.Players[1].Active);
        }

        [Test]
        public void Special_Transfer_Moves_A_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Special Transfer"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttach(tauros, C(DoubleTurboEnergy));
            var dusknoir = e.ScenarioBench(0, C(Dusknoir));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual(1, dusknoir.Energy.Count);
        }

        // ================================================================== Attacks

        [Test]
        public void Fast_Charge_Can_Be_Used_On_The_First_Players_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(RaichuV));
            Attach(e, raichu, Lightning, 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Fast Charge"));
            Assert.IsFalse(Offered(e, 0, "Attack: Dynamic Spark"));
        }

        [Test]
        public void Powdery_Uppercut_Needs_Spore_Ball_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var breloom = e.ScenarioActive(0, C(Breloom));
            Attach(e, breloom, Grass, 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Powdery Uppercut"));
            breloom.LastAttackName = "Spore Ball";
            breloom.LastAttackTurn = 1;
            Assert.IsTrue(Offered(e, 0, "Attack: Powdery Uppercut"));
        }

        [Test]
        public void Infernal_Vortex_Does_80_For_Each_Energy_Card_On_Top_And_Discards_Them()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Infernal Vortex"), new ScriptedAgent());
            var infernape = e.ScenarioActive(0, C(Infernape));
            Attach(e, infernape, Fire, 1);
            var mawile = e.ScenarioActive(1, C(MawileV));
            var deck = e.State.Players[0].Deck;
            deck.Clear();
            foreach (var key in new[] { Fire, Tauros, Fire, Tauros, Tauros, Tauros }) deck.Add(e.NewCard(0, C(key)));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, mawile.DamageCounters, "2 Energy cards: 160");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(4, deck.Count);
        }

        [Test]
        public void Dede_Flash_Does_60_More_And_Confuses_When_The_Opponent_Has_1_Prize_Left()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dede-Flash"), new ScriptedAgent());
            var dedenne = e.ScenarioActive(0, C(Dedenne));
            Attach(e, dedenne, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.State.Players[1].Prizes.RemoveRange(0, 5);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, tauros.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Confused));
        }

        [Test]
        public void Jealous_Eyes_Puts_3_Counters_For_Each_Prize_The_Opponent_Took()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jealous Eyes"), new ScriptedAgent());
            var mimikyu = e.ScenarioActive(0, C(MimikyuV));
            Attach(e, mimikyu, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.State.Players[1].Prizes.RemoveRange(0, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, tauros.DamageCounters);
        }

        [Test]
        public void Ominous_Numbers_Places_13_Counters_After_Acerolas_Premonition()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ominous Numbers"), new ScriptedAgent());
            var mimikyu = e.ScenarioActive(0, C(MimikyuVmax));
            Attach(e, mimikyu, Psychic, 2);
            var aggron = e.ScenarioActive(1, C(AggronVmax));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].SupporterNameThisTurn = "Acerola's Premonition";
            Step(e);
            Assert.AreEqual(13, aggron.DamageCounters);
        }

        [Test]
        public void Hangry_Spike_Does_120_More_After_Marnies_Pride()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Hangry Spike"), new ScriptedAgent());
            var morpeko = e.ScenarioActive(0, C(MorpekoV));
            Attach(e, morpeko, Darkness, 3);
            var aggron = e.ScenarioActive(1, C(AggronVmax));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].SupporterNameThisTurn = "Marnie's Pride";
            Step(e);
            Assert.AreEqual(24, aggron.DamageCounters);
        }

        [Test]
        public void Trick_Wind_Stops_Tools_And_Special_Energy_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trick Wind"), new ScriptedAgent());
            var whimsicott = e.ScenarioActive(0, C(WhimsicottVstar));
            Attach(e, whimsicott, Psychic, 3);
            var aggron = e.ScenarioActive(1, C(AggronVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, aggron.DamageCounters);
            var helmet = e.ScenarioAddToHand(1, C(PotHelmet));
            var turbo = e.ScenarioAddToHand(1, C(DoubleTurboEnergy));
            e.ScenarioSetTurn(1, 4);
            Assert.IsFalse(e.CanPlayTrainer(1, helmet));
            Assert.IsTrue(e.EnergyCardBlocked(1, turbo));
        }

        [Test]
        public void Lightning_Blast_Discards_The_Lightning_Energy_For_120_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lightning Blast"), new ScriptedAgent());
            var pikachu = e.ScenarioActive(0, C(PikachuV));
            Attach(e, pikachu, Lightning, 3);
            var aggron = e.ScenarioActive(1, C(AggronVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(22, aggron.DamageCounters);
            Assert.AreEqual(0, pikachu.Energy.Count);
        }

        // ================================================================== Trainers

        [Test]
        public void Brilliant_Stars_Tools_Are_Pokemon_Tools_Not_Items()
        {
            foreach (var key in new[] { BlunderPolicy, CleansingGloves, "BRS-142", PotHelmet })
            {
                Assert.IsTrue(C(key).IsTool, key);
                Assert.IsFalse(C(key).IsItem, key + " was printed as an Item, but Tools are their own Trainer type now");
            }
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var helmet = e.ScenarioAddToHand(0, C(PotHelmet));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].ItemLockTurn = 3;
            Assert.IsTrue(e.CanPlayTrainer(0, helmet), "an Item lock doesn't stop Pokémon Tools");
        }

        [Test]
        public void Pot_Helmet_Protects_Pokemon_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(PotHelmet));
            var mawile = e.ScenarioBench(0, C(MawileV));
            e.ScenarioAttachTool(mawile, C(PotHelmet));
            var foe = e.ScenarioActive(1, C(Tauros));
            e.DealAttackDamage(foe, tauros, 50);
            Assert.AreEqual(2, tauros.DamageCounters);
            e.DealAttackDamage(foe, mawile, 50, false);
            Assert.AreEqual(5, mawile.DamageCounters, "Mawile V has a Rule Box");
        }

        [Test]
        public void Cleansing_Gloves_Do_30_More_To_A_Psychic_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(CleansingGloves));
            var mewtwo = e.ScenarioActive(1, C(Mewtwo));
            e.DealAttackDamage(tauros, mewtwo, 30);
            Assert.AreEqual(6, mewtwo.DamageCounters);
            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros2 = e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAttachTool(tauros2, C(CleansingGloves));
            var foe = e2.ScenarioActive(1, C(Tauros));
            e2.DealAttackDamage(tauros2, foe, 30);
            Assert.AreEqual(3, foe.DamageCounters, "not a Psychic Pokémon");
        }

        [Test]
        public void Magma_Basin_Attaches_Fire_Energy_From_The_Discard_With_2_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Magma Basin"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var torkoal = e.ScenarioBench(0, C(Torkoal));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(MagmaBasin));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, torkoal.Energy.Count);
            Assert.AreEqual(2, torkoal.DamageCounters);
            Assert.IsFalse(Offered(e, 0, "Use Magma Basin"), "once during each player's turn");
        }

        [Test]
        public void Kindler_Discards_A_Fire_Energy_And_Takes_From_The_Top_7()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Kindler"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Kindler));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Kindler"), "it needs a Fire Energy card to discard");
            e.ScenarioAddToHand(0, C(Fire));
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(1, p.Hand.Count);
            Assert.AreEqual(2, p.Discard.Count, "the Fire Energy and Kindler");
        }

        [Test]
        public void Gloria_Benches_Basic_Pokemon_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Gloria", "Tauros|Tauros"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDeck(0, C(Tauros), 2);
            e.ScenarioAddToDeck(0, C(MawileV), 1);
            e.ScenarioAddToHand(0, C(Gloria));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(2, bench.Count);
            Assert.IsTrue(bench.All(m => m.Name == "Tauros"));
        }

        [Test]
        public void Roseannes_Backup_Shuffles_One_Card_Of_Each_Chosen_Kind()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Roseanne's Backup", "a Pokémon|an Energy card"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioAddToHand(0, C(RoseannesBackup));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(22, p.Deck.Count);
            Assert.AreEqual("Roseanne's Backup", p.Discard.Single().Def.Name);
        }

        [Test]
        public void Team_Yells_Cheer_Shuffles_Pokemon_And_Supporters_But_Not_Itself()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Yell's Cheer", "Tauros|Boss's Orders"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            e.ScenarioAddToDiscard(0, C(BossOrders), 1);
            e.ScenarioAddToDiscard(0, C(TeamYellsCheer), 1);
            e.ScenarioAddToHand(0, C(TeamYellsCheer));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(22, p.Deck.Count);
            Assert.AreEqual(2, p.Discard.Count(c => c.Def.Name == "Team Yell's Cheer"));
        }

        [Test]
        public void Acerolas_Premonition_Draws_A_Card_For_Each_Trainer_In_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Acerola's Premonition"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(AcerolasPremonition));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(PotHelmet));
            e.ScenarioAddToHand(1, C(Gloria));
            e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Hand.Count);
            Assert.AreEqual("Acerola's Premonition", e.State.Players[0].SupporterNameThisTurn);
        }
    }
}
