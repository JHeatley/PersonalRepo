using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v50: Silver Tempest (SIT) and its Trainer Gallery: Unidentified Fossil, the Seal Stones, Star Cipher, Surprise Spores,
    /// Emergency Entry, attack chains that check the player's last attack, and the new engine hooks they needed.
    /// </summary>
    public class SitTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Water = "MEE-11";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Tauros = "MEW-128";
        const string Pikachu = "MEW-25";
        const string BosssOrders = "MEG-114";

        const string Ariados = "SIT-4";
        const string Amoonguss = "SIT-12";
        const string Torracat = "SIT-31";
        const string Jynx = "SIT-62";
        const string Mismagius = "SIT-64";
        const string UnownVstar = "SIT-66";
        const string MawileV = "SIT-70";
        const string Medicham = "SIT-73";
        const string Reuniclus = "SIT-78";
        const string Dedenne = "SIT-85";
        const string HisuianArcanineV = "SIT-90";
        const string Anorith = "SIT-95";
        const string SkuntankV = "SIT-108";
        const string Metagross = "SIT-119";
        const string RadiantJirachi = "SIT-120";
        const string Togedemaru = "SIT-127";
        const string RegidragoV = "SIT-135";
        const string RegidragoVstar = "SIT-136";
        const string LugiaV = "SIT-138";
        const string LugiaVstar = "SIT-139";
        const string HoOhV = "SIT-140";
        const string Archeops = "SIT-147";
        const string Brandon = "SIT-151";
        const string EarthenSealStone = "SIT-154";
        const string EmergencyJelly = "SIT-155";
        const string ForestSealStone = "SIT-156";
        const string LeafyCamoPoncho = "SIT-160";
        const string QuadStone = "SIT-163";
        const string UnidentifiedFossil = "SIT-165";
        const string RegenerativeEnergy = "SIT-168";
        const string VGuardEnergy = "SIT-169";
        const string GapejawBog = "SIT-213";

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
        public void Every_Silver_Tempest_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 215; n++) Assert.IsNotNull(db.Get("SIT-" + n), "SIT " + n + " is missing");
            for (int n = 1; n <= 30; n++) Assert.IsNotNull(db.Get("SIT-TG" + n), "SIT TG" + n + " is missing");
            Assert.AreEqual("SIT-7", db.MainPrinting(C("SIT-170")).Key, "the full art is a second print of Serperior V");
            Assert.AreEqual(LugiaVstar, db.MainPrinting(C("SIT-211")).Key);
            Assert.AreEqual("SIT-26", db.MainPrinting(C("SIT-TG1")).Key, "TG1 Braixen is a second print of SIT 26");
            Assert.AreEqual("CRZ-101", db.MainPrinting(C("SIT-TG20")).Key, "Rayquaza VMAX was already in the pool");
            Assert.AreEqual("CRZ-7", db.MainPrinting(C("SIT-5")).Key, "Sunkern was already in the pool");
            Assert.AreEqual("SIT-TG5", db.Get("SIT", "TG05").Key, "TG05 and TG5 are the same card number");
            Assert.IsTrue(C("SIT-TG15").IsRapidStrike, "Blaziken VMAX");
            Assert.IsTrue(C("SIT-TG7").IsSingleStrike, "Rockruff");
            Assert.IsFalse(Formats.IsStandardCard(C(LugiaVstar)), "regulation mark F: Expanded");
        }

        [Test]
        public void Seal_Stones_And_Unidentified_Fossil_Load_From_The_Card_Data()
        {
            var earthen = C(EarthenSealStone);
            Assert.IsTrue(earthen.IsTool);
            Assert.AreEqual("Star Gravity", earthen.Attacks[0].Name);
            Assert.IsTrue(earthen.Attacks[0].VstarPower);
            var forest = C(ForestSealStone);
            Assert.AreEqual("Star Alchemy", forest.Abilities[0].Name);
            Assert.IsTrue(forest.Abilities[0].VstarPower);
            var fossil = C(UnidentifiedFossil);
            Assert.IsTrue(fossil.IsItem && fossil.IsFossil && fossil.FossilPlain);
            Assert.AreEqual(60, fossil.Hp);
            Assert.AreEqual("Unidentified Fossil", C(Anorith).EvolvesFrom);
        }

        // ================================================================== Unidentified Fossil

        [Test]
        public void Unidentified_Fossil_Gives_A_Prize_Card_And_Can_Be_Poisoned()
        {
            // Your ruling: as its text says nothing else, it's like any Basic Pokémon for Prize cards and Special Conditions.
            var e = Scenario(new ScriptedAgent().Then("Bench Unidentified Fossil"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(UnidentifiedFossil));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var fossil = e.State.Players[0].Bench.Single();
            Assert.IsTrue(fossil.IsBasic);
            Assert.AreEqual(60, e.MaxHp(fossil));
            Assert.AreEqual(1, e.PrizesFor(fossil));

            e.SwitchActive(0, fossil);
            Assert.IsTrue(e.ApplyCondition(fossil, SpecialCondition.Poisoned));

            e.ScenarioAddToHand(0, C(Anorith));
            e.ScenarioSetTurn(0, 5);
            Assert.IsTrue(Offered(e, 0, "into Anorith"));
            Assert.IsFalse(Offered(e, 0, "Retreat"), "it can't retreat");
        }

        // ================================================================== VSTAR Powers

        [Test]
        public void Star_Cipher_Makes_Every_Opposing_Weakness_Psychic()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Star Cipher"), new ScriptedAgent());
            var unown = e.ScenarioActive(0, C(UnownVstar));
            Attach(e, unown, Psychic, 3);
            var dragon = e.ScenarioActive(1, C(RegidragoV));
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNull(e.WeaknessOf(dragon));
            Step(e);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Assert.AreEqual(EnergyType.Psychic, e.WeaknessOf(dragon), "your ruling: a Pokémon with no Weakness gets Psychic Weakness");
            Assert.AreEqual(EnergyType.Psychic, e.WeaknessOf(tauros));
            Assert.AreEqual(EnergyType.Darkness, e.WeaknessOf(unown), "only the opponent's Pokémon");

            e.DiscardPokemonFromPlay(unown);
            Assert.IsNull(e.WeaknessOf(dragon), "until the Unown leaves play");
            Assert.AreEqual(EnergyType.Fighting, e.WeaknessOf(tauros));
        }

        [Test]
        public void Earthen_Seal_Stone_Gives_A_Pokemon_V_Star_Gravity()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Star Gravity"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaV));
            Attach(e, lugia, Psychic, 3);
            e.ScenarioAttachTool(lugia, C(EarthenSealStone));
            var skuntank = e.ScenarioActive(1, C(SkuntankV));
            var hurt = e.ScenarioBench(1, C(MawileV));
            hurt.DamageCounters = 12;
            var plain = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Star Gravity"));
            Step(e);
            Assert.AreEqual(11, skuntank.DamageCounters, "210 HP down to 100");
            Assert.AreEqual(12, hurt.DamageCounters, "already under 100 HP");
            Assert.AreEqual(0, plain.DamageCounters, "not a Pokémon V");
            Assert.IsTrue(e.State.Players[0].VstarUsed);
        }

        [Test]
        public void Earthen_Seal_Stone_Does_Nothing_For_Other_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 3);
            e.ScenarioAttachTool(tauros, C(EarthenSealStone));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Star Gravity"));
        }

        [Test]
        public void Forest_Seal_Stone_Searches_For_Any_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Star Alchemy", "Lugia VSTAR"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaV));
            e.ScenarioAttachTool(lugia, C(ForestSealStone));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDeck(0, C(LugiaVstar), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual("Lugia VSTAR", e.State.Players[0].Hand.Single().Def.Name);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Assert.IsFalse(Offered(e, 0, "Use Star Alchemy"));
        }

        [Test]
        public void Apex_Dragon_Uses_An_Attack_Of_A_Dragon_Pokemon_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Apex Dragon", "Dragon Laser"), new ScriptedAgent());
            var regidrago = e.ScenarioActive(0, C(RegidragoVstar));
            Attach(e, regidrago, Grass, 2);
            Attach(e, regidrago, Fire, 1);
            e.ScenarioAddToDiscard(0, C(RegidragoV), 1);
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            var target = e.ScenarioActive(1, C(LugiaV));
            var benched = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(13, target.DamageCounters);
            Assert.AreEqual(3, benched.DamageCounters, "Dragon Laser's 30 to a Benched Pokémon");
        }

        [Test]
        public void Legacy_Star_Discards_7_And_Takes_2_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Legacy Star", "Lugia VSTAR|Psychic"), new ScriptedAgent());
            e.ScenarioActive(0, C(RegidragoVstar));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(0, C(LugiaVstar), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            Assert.AreEqual(13, p.Deck.Count);
            Assert.AreEqual(2, p.Hand.Count);
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Lugia VSTAR"));
            Assert.IsTrue(p.VstarUsed);
        }

        [Test]
        public void Summoning_Star_Benches_Colorless_Pokemon_Without_A_Rule_Box()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Summoning Star", "Tauros|Archeops"), new ScriptedAgent());
            e.ScenarioActive(0, C(LugiaVstar));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Tauros), 1);
            e.ScenarioAddToDiscard(0, C(Archeops), 1);
            e.ScenarioAddToDiscard(0, C(LugiaV), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            CollectionAssert.AreEquivalent(new[] { "Tauros", "Archeops" }, p.Bench.Select(m => m.Name).ToArray());
            Assert.AreEqual("Lugia V", p.Discard.Single().Def.Name, "Pokémon V have a Rule Box");
        }

        // ================================================================== Abilities

        [Test]
        public void Surprise_Spores_Discards_The_Attackers_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Amoonguss));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Amoonguss", e.State.Players[1].Discard.Single().Def.Name, "Chomp Down discarded it");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "Surprise Spores discarded the attacker's hand");
            Assert.AreEqual(2, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Surprise_Spores_Ignores_Your_Own_Discards()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Refinement", "Amoonguss"), new ScriptedAgent());
            e.ScenarioActive(0, C("SIT-68"));
            e.ScenarioAddToHand(0, C(Amoonguss));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(2, e.State.Players[0].Hand.Count, "Refinement drew 2");
            Assert.AreEqual(1, e.State.Players[1].Hand.Count, "nothing happens to the opponent's hand");
        }

        [Test]
        public void Emergency_Entry_Benches_Metagross_Drawn_For_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(1, 2);
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Metagross)));
            e.StartTurn();
            var p = e.State.Players[0];
            Assert.AreEqual(0, e.State.CurrentPlayer);
            Assert.AreEqual("Metagross", p.Bench.Single().Name);
            Assert.AreEqual(3, p.Hand.Count, "3 cards drawn; Metagross isn't in the hand");
        }

        [Test]
        public void Selfish_Lips_Gives_No_Prize_Cards_To_A_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            var jynx = e.ScenarioActive(1, C(Jynx));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(jynx, e.State.Players[1].Active, "Jynx was Knocked Out");
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "no Prize card for it");
        }

        [Test]
        public void Spiteful_Magic_Puts_8_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            e.ScenarioActive(1, C(Mismagius));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, mawile.DamageCounters);
        }

        [Test]
        public void Spiteful_Magic_Needs_Full_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            var mismagius = e.ScenarioActive(1, C(Mismagius));
            mismagius.DamageCounters = 1;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, mawile.DamageCounters);
        }

        [Test]
        public void Persistent_Cells_Puts_Reuniclus_Into_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tempest Dive"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaVstar));
            Attach(e, lugia, Psychic, 4);
            var reuniclus = e.ScenarioActive(1, C(Reuniclus));
            e.ScenarioAttach(reuniclus, C(Psychic));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual("Reuniclus", opp.Hand.Single().Def.Name);
            Assert.AreEqual(1, opp.Discard.Count, "its Energy is discarded");
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "the Prize card is still taken");
        }

        [Test]
        public void Entrusted_Wishes_Searches_When_Knocked_Out_In_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent().Then("Psychic|Psychic|Psychic"));
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            e.ScenarioActive(1, C(RadiantJirachi));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);
            Assert.AreEqual(17, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Chakra_Awakening_Needs_Exactly_4_Cards_In_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Medicham));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attack: Vigorous Kick"), "it costs nothing with 4 cards in hand");
            e.ScenarioAddToHand(0, C(Psychic));
            Assert.IsFalse(Offered(e, 0, "Attack: Vigorous Kick"));
        }

        [Test]
        public void Hidden_Threads_Raises_The_Cost_Of_Opposing_VSTAR_Attacks_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaVstar));
            Attach(e, lugia, Psychic, 4);
            e.ScenarioActive(1, C(Ariados));
            e.ScenarioBench(1, C(Ariados));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Tempest Dive"));
            e.ScenarioAttach(lugia, C(Psychic));
            Assert.IsTrue(Offered(e, 0, "Attack: Tempest Dive"), "two Ariados still add only [C]");
        }

        [Test]
        public void Irresistible_Force_Can_Be_Used_Again_And_Again()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Irresistible Force", "Fighting", "Use Irresistible Force"), new ScriptedAgent());
            var arcanine = e.ScenarioActive(0, C(HisuianArcanineV));
            var tauros = e.ScenarioBench(0, C(Tauros));
            Attach(e, tauros, Fighting, 2);
            e.ScenarioAttach(tauros, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(2, arcanine.Energy.Count);
            Assert.AreEqual(1, tauros.Energy.Count, "only Fighting Energy moves");
            Assert.IsFalse(Offered(e, 0, "Use Irresistible Force"));
        }

        // ================================================================== Attacks

        [Test]
        public void Dede_Short_Needs_Toge_Dash_During_Your_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Toge Dash"), new ScriptedAgent());
            var toge = e.ScenarioActive(0, C(Togedemaru));
            e.ScenarioAttach(toge, C(Lightning));
            var dedenne = e.ScenarioBench(0, C(Dedenne));
            e.ScenarioAttach(dedenne, C(Lightning));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual("Toge Dash", p.LastAttackName);
            Assert.AreEqual(3, p.LastAttackTurn);
            e.SwitchActive(0, dedenne);
            e.ScenarioSetTurn(0, 5);
            Assert.IsTrue(Offered(e, 0, "Attack: Dede-Short"));
            e.ScenarioSetTurn(0, 7);
            Assert.IsFalse(Offered(e, 0, "Attack: Dede-Short"), "only the turn right after Toge Dash");
        }

        [Test]
        public void Pursuit_Blast_Does_120_To_A_Pokemon_That_Retreated()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pursuit Blast"), new ScriptedAgent().Then("Retreat"));
            var skuntank = e.ScenarioActive(0, C(SkuntankV));
            Attach(e, skuntank, Psychic, 2);
            var pikachu = e.ScenarioActive(1, C(Pikachu));
            e.ScenarioAttach(pikachu, C(Psychic));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(1, 2);
            e.MainPhaseStep();
            Assert.AreEqual(2, pikachu.RetreatedOnTurn);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, pikachu.DamageCounters);
        }

        [Test]
        public void Gritty_Claws_Keeps_Torracat_At_10_HP_From_Full_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gritty Claws"), new ScriptedAgent());
            var torracat = e.ScenarioActive(0, C(Torracat));
            Attach(e, torracat, Fire, 2);
            var attacker = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(attacker, torracat, 200);
            Assert.AreEqual(7, torracat.DamageCounters, "80 HP down to 10");
            e.DealAttackDamage(attacker, torracat, 20);
            Assert.IsTrue(e.IsKnockedOut(torracat), "no longer at full HP");
        }

        [Test]
        public void Toge_Dash_On_Heads_Gives_No_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var toge = e.ScenarioActive(0, C(Togedemaru));
            e.State.Effects.Add(new CardsSIT1.NoPrizeEffect("Toge Dash test", 0, toge.Uid, 4));
            Assert.AreEqual(0, e.PrizesFor(toge));
        }

        // ================================================================== Trainers and Energy

        [Test]
        public void Leafy_Camo_Poncho_Stops_Bosss_Orders_On_A_Pokemon_VSTAR()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boss's Orders"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(BosssOrders));
            e.ScenarioActive(1, C(Tauros));
            var vstar = e.ScenarioBench(1, C(LugiaVstar));
            e.ScenarioAttachTool(vstar, C(LeafyCamoPoncho));
            var other = e.ScenarioBench(1, C(Pikachu));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreSame(other, e.State.Players[1].Active);
        }

        [Test]
        public void Quad_Stone_Heals_Everything_With_4_At_Once()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Quad Stone", "Yes"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Tauros));
            active.DamageCounters = 5;
            var benched = e.ScenarioBench(0, C(Tauros));
            benched.DamageCounters = 3;
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(0, C(QuadStone));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual(0, active.DamageCounters);
            Assert.AreEqual(0, benched.DamageCounters);
            Assert.AreEqual(4, e.State.Players[0].Discard.Count(c => c.Def.Name == "Quad Stone"));
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void One_Quad_Stone_Heals_10_From_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Quad Stone"), new ScriptedAgent());
            var active = e.ScenarioActive(0, C(Tauros));
            active.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(QuadStone));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual(4, active.DamageCounters);
        }

        [Test]
        public void Brandon_Must_Be_The_Last_Card_In_Your_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Brandon"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioBench(1, C(Pikachu));
            var brandon = e.ScenarioAddToHand(0, C(Brandon));
            var extra = e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Brandon"));
            e.State.Players[0].Hand.Remove(extra);
            Assert.IsTrue(Offered(e, 0, "Play Brandon"));
            e.MainPhaseStep();
            Assert.AreEqual(3, e.State.Players[0].Hand.Count, "a card for each Benched Pokémon (both players')");
        }

        [Test]
        public void V_Guard_Energy_Takes_30_Less_From_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttach(tauros, C(VGuardEnergy));
            e.ScenarioAttach(tauros, C(VGuardEnergy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, tauros.DamageCounters, "100 minus 30 once (it doesn't stack)");
        }

        [Test]
        public void Regenerative_Energy_Heals_100_When_The_Pokemon_V_Evolves()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var lugia = e.ScenarioActive(0, C(LugiaV));
            lugia.DamageCounters = 12;
            e.ScenarioAttach(lugia, C(RegenerativeEnergy));
            e.ScenarioAddToHand(0, C(LugiaVstar));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual("Lugia VSTAR", lugia.Name);
            Assert.AreEqual(2, lugia.DamageCounters);
        }

        [Test]
        public void Emergency_Jelly_Heals_At_The_End_Of_Either_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var mon = e.ScenarioActive(1, C(Tauros));
            mon.DamageCounters = 9;
            e.ScenarioAttachTool(mon, C(EmergencyJelly));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(0, mon.DamageCounters, "healed at the end of the opponent's turn");
            Assert.AreEqual(0, mon.Tools.Count, "then discarded");
            Assert.AreEqual("Emergency Jelly", e.State.Players[1].Discard.Single().Def.Name);
        }

        [Test]
        public void Gapejaw_Bog_Only_Hurts_Pokemon_Benched_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Pikachu"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Pikachu));
            e.ScenarioSetStadium(1, C(GapejawBog));
            e.ScenarioSetTurn(0, 3);
            e.MainPhaseStep();
            Assert.AreEqual(2, e.State.Players[0].Bench.Single().DamageCounters);
            var fromDeck = e.PutOntoBench(0, e.NewCard(0, C(Pikachu)), false);
            Assert.AreEqual(0, fromDeck.DamageCounters);
        }

        [Test]
        public void Ho_Oh_V_Revives_From_The_Discard_Pile_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ho-Oh V's Reviving Flame from your discard pile", "Fire|Water|Grass"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(0, C(HoOhV), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            e.ScenarioAddToDiscard(0, C(Water), 1);
            e.ScenarioAddToDiscard(0, C(Grass), 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "Reviving Flame ends the turn");
            var hooh = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Ho-Oh V", hooh.Name);
            Assert.AreEqual(3, hooh.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }
    }
}
