using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v49: Crown Zenith (CRZ) and the Sword &amp; Shield mechanics it brought in: VSTAR Powers, Pokémon VMAX, Radiant
    /// Pokémon, battle styles, the Lost Zone, Star Chronos's extra turn, Sky Seal Stone and Grant's discard-pile effect.
    /// </summary>
    public class CrzTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Metal = "MEE-16";

        const string Tauros = "MEW-128";
        const string Pikachu = "MEW-25";
        const string BosssOrders = "MEG-114";

        const string Oddish = "CRZ-1";
        const string Tangela = "CRZ-4";
        const string LeafeonVstar = "CRZ-14";
        const string CharizardV = "CRZ-18";
        const string CharizardVstar = "CRZ-19";
        const string RadiantCharizard = "CRZ-20";
        const string GlaceonV = "CRZ-38";
        const string RotomV = "CRZ-45";
        const string RadiantCharjabug = "CRZ-51";
        const string ZeraoraVmax = "CRZ-54";
        const string ZeraoraVstar = "CRZ-55";
        const string HattereneVmax = "CRZ-66";
        const string ZacianV = "CRZ-95";
        const string ZamazentaVstar = "CRZ-99";
        const string RayquazaVmax = "CRZ-101";
        const string RadiantEternatus = "CRZ-105";
        const string Ditto = "CRZ-107";
        const string Snorlax = "CRZ-109";
        const string EeveeV = "CRZ-108";
        const string Bidoof = "CRZ-111";
        const string RegigigasVstar = "CRZ-114";
        const string Oranguru = "CRZ-119";
        const string FriendsInHisui = "CRZ-130";
        const string SkySealStone = "CRZ-143";
        const string Kricketune = "CRZ-GG2";
        const string Toxtricity = "CRZ-GG9";
        const string Diancie = "CRZ-GG13";
        const string LumineonV = "CRZ-GG39";
        const string MewtwoVstar = "CRZ-GG44";
        const string DeoxysVmax = "CRZ-GG45";
        const string ColresssExperiment = "CRZ-GG59";
        const string Grant = "CRZ-GG62";
        const string DialgaVstar = "CRZ-GG68";
        const string GiratinaVstar = "CRZ-GG69";
        const string ZeraoraV = "CRZ-53";
        const string KyogreV = "CRZ-37";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        // ================================================================== Data

        [Test]
        public void Every_Crown_Zenith_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 160; n++) Assert.IsNotNull(db.Get("CRZ-" + n), "CRZ " + n + " is missing");
            for (int n = 1; n <= 70; n++) Assert.IsNotNull(db.Get("CRZ-GG" + n), "CRZ GG" + n + " is missing");
            Assert.AreEqual("CRZ-62", db.MainPrinting(C("CRZ-GG11")).Key, "GG11 is a second print of Lunatone");
            Assert.AreEqual(RayquazaVmax, db.MainPrinting(C("CRZ-102")).Key);
            Assert.AreEqual("POR-71", db.MainPrinting(C("CRZ-125")).Key, "Crushing Hammer was already in the pool");
            Assert.AreEqual("CRZ-GG13", db.Get("CRZ", "GG013").Key, "GG013 and GG13 are the same card number");
            Assert.AreEqual("CRZ-GG13", CardDatabase.MakeKey("crz", "GG013"));
            Assert.IsFalse(Formats.IsStandardCard(C(RadiantCharizard)), "regulation mark F: Expanded");
        }

        [Test]
        public void Sword_And_Shield_Mechanics_Load_From_The_Card_Data()
        {
            var radiant = C(RadiantCharizard);
            Assert.IsTrue(radiant.IsRadiant && radiant.HasRuleBox);
            Assert.AreEqual(1, radiant.PrizeValue, "a Radiant Pokémon gives 1 Prize card");
            var vmax = C(ZeraoraVmax);
            Assert.IsTrue(vmax.IsVmax && vmax.IsPokemonV);
            Assert.AreEqual(3, vmax.PrizeValue);
            Assert.AreEqual("Zeraora V", vmax.EvolvesFrom);
            var vstar = C(CharizardVstar);
            Assert.IsTrue(vstar.IsVstar);
            Assert.AreEqual(2, vstar.PrizeValue);
            Assert.AreEqual("Star Blaze", vstar.Attacks[1].Name);
            Assert.IsTrue(vstar.Attacks[1].VstarPower);
            Assert.IsFalse(vstar.Attacks[0].VstarPower);
            Assert.IsTrue(C("CRZ-GG70").Abilities[0].VstarPower, "Starbirth is a VSTAR Power");
            var stone = C(SkySealStone);
            Assert.IsTrue(stone.IsTool);
            Assert.AreEqual("Star Order", stone.Abilities[0].Name);
            Assert.IsTrue(stone.Abilities[0].VstarPower);
            Assert.IsTrue(C("CRZ-39").IsRapidStrike, "Shinx (Battle Styles reprint)");
            Assert.AreEqual("", C("CRZ-40").Strike, "the other Shinx has no battle style");
            Assert.IsTrue(C("CRZ-104").IsSingleStrike, "Duraludon VMAX");
            Assert.IsTrue(C("CRZ-GG20").IsFusionStrike, "Latias");
            Assert.IsTrue(C("CRZ-60").IsFusionStrike, "Mew V");
        }

        [Test]
        public void Only_One_Radiant_Pokemon_In_A_Deck()
        {
            var deck = new List<CardDef> { C(RadiantCharizard), C(RadiantCharjabug) };
            for (int i = 0; i < 58; i++) deck.Add(C(Psychic));
            Assert.IsTrue(Decklist.Validate(deck, DeckFormat.Expanded).Any(p => p.Contains("Radiant")));
            deck[1] = C(Oddish);
            Assert.IsFalse(Decklist.Validate(deck, DeckFormat.Expanded).Any(p => p.Contains("Radiant")));
        }

        // ================================================================== VSTAR Powers

        [Test]
        public void A_VSTAR_Power_Ability_Uses_Up_The_VSTAR_Power_Attacks_Too()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ivy Star"), new ScriptedAgent());
            var zard = e.ScenarioActive(0, C(CharizardVstar));
            Attach(e, zard, Fire, 4);
            e.ScenarioBench(0, C(LeafeonVstar));
            e.ScenarioActive(1, C(Pikachu));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Star Blaze"));
            Assert.IsTrue(Offered(e, 0, "Use Ivy Star"));

            Assert.IsFalse(e.MainPhaseStep(), "using an Ability doesn't end the turn");
            Assert.AreEqual("Tauros", e.State.Players[1].Active.Name, "Ivy Star switched in the Benched Pokémon");
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Assert.IsFalse(Offered(e, 0, "Attack: Star Blaze"), "only 1 VSTAR Power in a game");
            Assert.IsTrue(Offered(e, 0, "Attack: Explosive Fire"));
            Assert.IsFalse(Offered(e, 0, "Use Ivy Star"));
            Assert.IsTrue(PlayerView.Create(e, 1).Sides[0].VstarUsed, "the VSTAR marker is turned over for both players");
        }

        [Test]
        public void A_VSTAR_Power_Attack_Uses_Up_The_VSTAR_Power()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Star Blaze"), new ScriptedAgent());
            var zard = e.ScenarioActive(0, C(CharizardVstar));
            Attach(e, zard, Fire, 4);
            e.ScenarioBench(0, C(LeafeonVstar));
            e.ScenarioActive(1, C(RegigigasVstar));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.State.Players[0].VstarUsed);
            Step(e);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Assert.AreEqual(2, zard.Energy.Count, "Star Blaze discards 2 Energy");
            e.ScenarioSetTurn(0, 5);
            Assert.IsFalse(Offered(e, 0, "Use Ivy Star"), "the VSTAR Power is gone for the rest of the game");
        }

        [Test]
        public void Lightning_Storm_Star_Hits_Each_Chosen_Pokemon_Once_With_The_Total()
        {
            // Your ruling: a Pokémon chosen 3 times takes one 180-damage hit, so a damage reduction applies only once.
            var e = Scenario(new ScriptedAgent().Then("Attack: Lightning Storm Star", "Deoxys VMAX", "Deoxys VMAX", "Deoxys VMAX", "Tauros"), new ScriptedAgent());
            var zeraora = e.ScenarioActive(0, C(ZeraoraVstar));
            Attach(e, zeraora, Lightning, 4);
            var deoxys = e.ScenarioActive(1, C(DeoxysVmax));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, deoxys.DamageCounters, "180 in one hit, minus 30 once for Protective DNA (no Weakness)");
            Assert.AreEqual(3, tauros.DamageCounters, "60 minus 30 on the Bench");
            Assert.IsTrue(e.State.Players[0].VstarUsed);
        }

        [Test]
        public void Star_Chronos_Takes_Another_Turn_And_Skips_Pokemon_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Star Chronos"), new ScriptedAgent());
            var dialga = e.ScenarioActive(0, C(DialgaVstar));
            Attach(e, dialga, Metal, 5);
            var target = e.ScenarioActive(1, C(RegigigasVstar));
            e.ScenarioBench(1, C(Tauros));
            target.Conditions |= SpecialCondition.Poisoned;
            e.ScenarioSetTurn(0, 3);
            // An effect meant for the opponent's next turn (turn 4, which Star Chronos skips).
            e.State.Effects.Add(new DamageTakenEffect("test shield", 0, dialga.Uid, 4, -30));

            Assert.IsTrue(e.MainPhaseStep());
            Assert.AreEqual(22, target.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(22, target.DamageCounters, "Pokémon Checkup is skipped: no Poison damage");
            e.StartTurn();
            Assert.AreEqual(0, e.State.CurrentPlayer, "the same player takes another turn");
            Assert.AreEqual(5, e.State.TurnNumber, "turn numbers stay odd for this player");
            var shield = e.State.Effects.FirstOrDefault(x => x.Name == "test shield");
            Assert.IsNotNull(shield, "an effect for the opponent's next turn waits for the turn they really play");
            Assert.AreEqual(6, shield.ExpiresAfterTurn);

            e.EndTurn();
            Assert.AreEqual(23, target.DamageCounters, "Pokémon Checkup is back after the extra turn");
            e.StartTurn();
            Assert.AreEqual(1, e.State.CurrentPlayer);
            Assert.AreEqual(6, e.State.TurnNumber);
            Assert.IsTrue(e.State.Effects.Any(x => x.Name == "test shield"));
        }

        [Test]
        public void Sky_Seal_Stone_Gives_A_Pokemon_V_Star_Order()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Star Order", "Attack: Heat Blast"), new ScriptedAgent());
            var zard = e.ScenarioActive(0, C(CharizardV));
            Attach(e, zard, Fire, 4);
            e.ScenarioAttachTool(zard, C(SkySealStone));
            var tauros = e.ScenarioBench(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(SkySealStone));
            var vmax = e.ScenarioActive(1, C(ZeraoraVmax));
            vmax.DamageCounters = 20;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.GetMainActions(0).Count(o => o.Label.Contains("Use Star Order")), "only the Pokémon V can use it");

            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Prizes.Count, "3 Prize cards for the VMAX and 1 more for Star Order");
        }

        // ================================================================== Lost Zone

        [Test]
        public void Colresss_Experiment_Puts_Cards_In_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Colress's Experiment"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(ColresssExperiment));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            var p = e.State.Players[0];
            Assert.AreEqual(3, p.Hand.Count);
            Assert.AreEqual(2, p.LostZone.Count);
            Assert.AreEqual(15, p.Deck.Count);
            Assert.AreEqual(2, PlayerView.Create(e, 1).Sides[0].LostZone.Count, "the Lost Zone can be looked at like a discard pile");
        }

        [Test]
        public void Star_Requiem_Needs_10_Cards_In_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Star Requiem"), new ScriptedAgent());
            var giratina = e.ScenarioActive(0, C(GiratinaVstar));
            Attach(e, giratina, Grass, 1);
            Attach(e, giratina, Psychic, 1);
            var target = e.ScenarioActive(1, C(RegigigasVstar));
            e.ScenarioBench(1, C(Tauros));
            for (int i = 0; i < 9; i++) e.State.Players[0].LostZone.Add(e.NewCard(0, C(Psychic)));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Star Requiem"), "9 cards aren't enough");
            e.State.Players[0].LostZone.Add(e.NewCard(0, C(Psychic)));
            Assert.IsTrue(Offered(e, 0, "Attack: Star Requiem"));
            Step(e);
            Assert.AreNotSame(target, e.State.Players[1].Active, "the Defending Pokémon is Knocked Out");
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count, "a VSTAR gives 2 Prize cards");
        }

        // ================================================================== Trainers

        [Test]
        public void Grant_Comes_Back_From_The_Discard_Pile_For_2_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Grant from your discard pile"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Grant), 1);
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Grant from your discard pile"));
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            Assert.AreEqual(1, p.Hand.Count);
            Assert.AreEqual("Grant", p.Hand[0].Def.Name);
            Assert.AreEqual(2, p.Discard.Count, "the 2 cards it cost");
            Assert.IsFalse(p.SupporterPlayedThisTurn, "it doesn't use up the Supporter for the turn");
            Assert.IsTrue(Offered(e, 0, "Play Grant"));
            Assert.IsFalse(Offered(e, 0, "from your discard pile"));
        }

        [Test]
        public void Princesss_Curtain_Stops_Boss_Orders_On_Benched_Basic_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boss's Orders"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(BosssOrders));
            e.ScenarioActive(1, C(Diancie));
            var basic = e.ScenarioBench(1, C(Tauros));
            var vmax = e.ScenarioBench(1, C(RayquazaVmax));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreSame(vmax, e.State.Players[1].Active, "only the Pokémon that isn't a Basic could be switched in");
            Assert.IsTrue(e.State.Players[1].Bench.Contains(basic));
        }

        // ================================================================== Pokémon

        [Test]
        public void Swelling_Tune_Gives_40_HP_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var oddish = e.ScenarioActive(0, C(Oddish));
            var k1 = e.ScenarioBench(0, C(Kricketune));
            e.ScenarioBench(0, C(Kricketune));
            var tangela = e.ScenarioActive(1, C(Tangela));
            Assert.AreEqual(90, e.MaxHp(oddish), "+40 from one Swelling Tune only");
            Assert.AreEqual(90, e.MaxHp(k1), "not Kricketune itself");
            Assert.AreEqual(80, e.MaxHp(tangela), "only your own Grass Pokémon");
        }

        [Test]
        public void Maximum_Downer_Needs_Only_Fusion_Strike_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Toxtricity));
            var vmax = e.ScenarioActive(1, C(ZeraoraVmax));
            Assert.AreEqual(290, e.MaxHp(vmax));
            e.ScenarioBench(0, C(Tauros));
            Assert.AreEqual(320, e.MaxHp(vmax), "Tauros isn't a Fusion Strike Pokémon");
        }

        [Test]
        public void Ditto_Uses_Attacks_Of_Basic_Pokemon_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Impact"), new ScriptedAgent());
            var ditto = e.ScenarioActive(0, C(Ditto));
            Attach(e, ditto, Psychic, 3);
            e.ScenarioAddToDiscard(0, C(Snorlax), 1);
            e.ScenarioAddToDiscard(0, C(EeveeV), 1);
            var target = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Heavy Impact"));
            Assert.IsFalse(Offered(e, 0, "Attack: Vee Brave"), "Pokémon with a Rule Box (Eevee V) don't count");
            Step(e);
            Assert.AreEqual(8, target.DamageCounters);
        }

        [Test]
        public void Climactic_Gate_Puts_Pokemon_VMAX_Onto_The_Bench_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Radiant Eternatus", "Yes", "Rayquaza VMAX|Rayquaza VMAX"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(RadiantEternatus));
            e.ScenarioAddToDeck(0, C(RayquazaVmax), 2);
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "Climactic Gate ends the turn");
            Assert.AreEqual(2, e.State.Players[0].Bench.Count(m => m.Def.IsVmax));
        }

        [Test]
        public void Witchs_Domain_Moves_Damage_Counters_To_The_Opponents_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Witch's Domain"), new ScriptedAgent());
            e.ScenarioActive(0, C(HattereneVmax));
            var hurt = e.ScenarioBench(0, C(Tauros));
            hurt.DamageCounters = 3;
            var target = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual(1, hurt.DamageCounters);
            Assert.AreEqual(2, target.DamageCounters);
        }

        [Test]
        public void Instant_Charge_Draws_3_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Instant Charge"), new ScriptedAgent());
            e.ScenarioActive(0, C(RotomV));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep());
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Psy_Purge_Discards_Psychic_Energy_From_Any_Of_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Psy Purge", "Psychic Energy|Psychic Energy|Psychic Energy"), new ScriptedAgent());
            var mewtwo = e.ScenarioActive(0, C(MewtwoVstar));
            Attach(e, mewtwo, Psychic, 2);
            var bench = e.ScenarioBench(0, C(Tauros));
            Attach(e, bench, Psychic, 1);
            var target = e.ScenarioActive(1, C(RegigigasVstar));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(27, target.DamageCounters, "90 for each of the 3 Energy");
            Assert.AreEqual(0, mewtwo.Energy.Count + bench.Energy.Count);
        }

        [Test]
        public void Piercing_Strike_Ignores_Weakness_And_Effects_On_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Piercing Strike"), new ScriptedAgent());
            var zacian = e.ScenarioActive(0, C(ZacianV));
            Attach(e, zacian, Metal, 1);
            var glaceon = e.ScenarioActive(1, C(GlaceonV));
            e.State.Effects.Add(new DamageTakenEffect("test shield", 1, glaceon.Uid, 4, -30));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, glaceon.DamageCounters, "40: no Metal Weakness, no damage reduction");
        }

        [Test]
        public void Shield_Star_Protects_All_Your_Pokemon_During_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Shield Star"), new ScriptedAgent().Then("Attack: Thunderous Bolt"));
            var zamazenta = e.ScenarioActive(0, C(ZamazentaVstar));
            var zeraora = e.ScenarioActive(1, C(ZeraoraV));
            Attach(e, zeraora, Lightning, 3);
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            NextTurn(e);
            Step(e);
            Assert.AreEqual(9, zamazenta.DamageCounters, "190 minus 100");
        }

        [Test]
        public void Carefree_Countenance_Protects_Bidoof_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dual Splash", "Tauros|Bidoof"), new ScriptedAgent());
            var kyogre = e.ScenarioActive(0, C(KyogreV));
            Attach(e, kyogre, Water, 1);
            Attach(e, kyogre, Psychic, 2);
            var tauros = e.ScenarioActive(1, C(Tauros));
            var bidoof = e.ScenarioBench(1, C(Bidoof));
            e.ScenarioBench(1, C(Pikachu));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, tauros.DamageCounters);
            Assert.AreEqual(0, bidoof.DamageCounters);
        }

        [Test]
        public void Primate_Acting_Uses_A_Supporter_From_The_Opponents_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Primate Acting", "Friends in Hisui"), new ScriptedAgent());
            var oranguru = e.ScenarioActive(0, C(Oranguru));
            Attach(e, oranguru, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(1, C(FriendsInHisui), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "Friends in Hisui: draw 3");
        }

        [Test]
        public void Luminous_Sign_Searches_For_A_Supporter_When_Benched_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Lumineon V", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(LumineonV));
            e.ScenarioAddToDeck(0, C(FriendsInHisui), 1);
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Name == "Friends in Hisui"));
        }
    }
}
