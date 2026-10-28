using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v51: Lost Origin (LOR) and its Trainer Gallery: Lost Block, Lost City, Collapsed Stadium, Windup Arm, Eternal Zone,
    /// Ancient Star (your ruling: whichever came first wins), Tricky Slap's Rock-Paper-Scissors (your ruling) and the other
    /// cards that needed new engine work.
    /// </summary>
    public class LorTests : ScenarioTestBase
    {
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";

        const string Tauros = "MEW-128";
        const string Pikachu = "MEW-25";
        const string MawileV = "SIT-70";
        const string CorviknightVmax = "SIT-TG19";
        const string BosssOrders = "MEG-114";

        const string Dustox = "LOR-10";
        const string Trevenant = "LOR-17";
        const string Kingdra = "LOR-37";
        const string KyuremVmax = "LOR-49";
        const string Cramorant = "LOR-50";
        const string Clefable = "LOR-63";
        const string Gengar = "LOR-66";
        const string MrMime = "LOR-67";
        const string Sableye = "LOR-70";
        const string Banette = "LOR-73";
        const string EnamorusV = "LOR-82";
        const string Machamp = "LOR-88";
        const string AerodactylVstar = "LOR-93";
        const string Barbaracle = "LOR-107";
        const string Drapion = "LOR-119";
        const string Sneasler = "LOR-123";
        const string GalarianStunfisk = "LOR-127";
        const string HisuianGoodraV = "LOR-135";
        const string PidgeotV = "LOR-137";
        const string Porygon = "LOR-140";
        const string Porygon2 = "LOR-141";
        const string PorygonZ = "LOR-142";
        const string Snorlax = "LOR-143";
        const string Komala = "LOR-149";
        const string ArcPhone = "LOR-152";
        const string BoxOfDisaster = "LOR-154";
        const string Fantina = "LOR-157";
        const string LakeAcuity = "LOR-160";
        const string LostCity = "LOR-161";
        const string PanicMask = "LOR-165";
        const string Riley = "LOR-166";
        const string Thorton = "LOR-167";
        const string WindupArm = "LOR-170";
        const string GiftEnergy = "LOR-171";
        const string CollapsedStadium = "LOR-215";
        const string CrobatV = "LOR-TG20";
        const string EternatusVmax = "LOR-TG22";

        /// <summary>Plays like the person on screen (IInteractiveAgent), answering like a ScriptedAgent.</summary>
        private sealed class PersonAgent : IInteractiveAgent
        {
            private readonly ScriptedAgent inner = new ScriptedAgent();

            public PersonAgent Then(params string[] labels)
            {
                inner.Then(labels);
                return this;
            }

            public List<int> Decide(DecisionRequest r) { return inner.Decide(r); }
        }

        private GameEngine PersonScenario(PersonAgent p1, PersonAgent p2)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), p1, p2);
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C(Psychic), 20);
            e.ScenarioAddToDeck(1, C(Psychic), 20);
            return e;
        }

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        private void FillLostZone(GameEngine e, int player, int count)
        {
            for (int i = 0; i < count; i++) e.State.Players[player].LostZone.Add(e.NewCard(player, C(Psychic)));
        }

        /// <summary>Mawile V with 3 Energy: Chomp Down does 100.</summary>
        private PokemonInPlay ChompDowner(GameEngine e, int player)
        {
            var mawile = e.ScenarioActive(player, C(MawileV));
            Attach(e, mawile, Psychic, 3);
            return mawile;
        }

        // ================================================================== Data

        [Test]
        public void Every_Lost_Origin_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 217; n++) Assert.IsNotNull(db.Get("LOR-" + n), "LOR " + n + " is missing");
            for (int n = 1; n <= 30; n++) Assert.IsNotNull(db.Get("LOR-TG" + n), "LOR TG" + n + " is missing");
            Assert.AreEqual("LOR-27", db.MainPrinting(C("LOR-173")).Key, "the full art is a second print of Delphox V");
            Assert.AreEqual("CRZ-29", db.MainPrinting(C("LOR-33")).Key, "Seel was already in the pool");
            Assert.AreEqual("LOR-TG17", db.MainPrinting(C("LOR-TG29")).Key, "TG29 is a second print of Pikachu VMAX");
            Assert.AreEqual("LOR-TG3", db.Get("LOR", "TG03").Key, "TG03 and TG3 are the same card number");
            Assert.IsTrue(C("LOR-TG7").IsSingleStrike, "Banette");
            Assert.IsTrue(C(AerodactylVstar).Attacks[1].VstarPower, "Ancient Star");
            Assert.IsTrue(C(Drapion).Abilities[0].VstarPower, "Hazard Star");
            Assert.AreEqual("Adventurer's Discovery", C("LOR-TG23").Name, "straight apostrophe, like every other card name");
            Assert.IsFalse(Formats.IsStandardCard(C(AerodactylVstar)), "regulation mark F: Expanded");
        }

        // ================================================================== Lost Zone cards

        [Test]
        public void Lost_Block_Sends_The_Opponents_Prize_Cards_To_Their_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var tauros = e.ScenarioActive(1, C(Tauros));
            tauros.DamageCounters = 3;
            e.ScenarioBench(1, C(Barbaracle));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(5, p.Prizes.Count, "the Prize card was taken");
            Assert.AreEqual(0, p.Hand.Count, "not into the hand");
            Assert.AreEqual(1, p.LostZone.Count, "into the Lost Zone");
        }

        [Test]
        public void Lost_Block_Is_Gone_Once_Barbaracle_Is_Knocked_Out()
        {
            // Compendium (2022-10-06): the Prize card for Barbaracle itself goes to the hand.
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var barbaracle = e.ScenarioActive(1, C(Barbaracle));
            barbaracle.DamageCounters = 3;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(5, p.Prizes.Count);
            Assert.AreEqual(1, p.Hand.Count);
            Assert.AreEqual(0, p.LostZone.Count);
        }

        [Test]
        public void Lost_City_Puts_The_Whole_Knocked_Out_Pokemon_In_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var porygon = e.ScenarioActive(1, C(Porygon));
            e.ScenarioEvolve(porygon, C(Porygon2));
            Attach(e, porygon, Psychic, 1);
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetStadium(0, C(LostCity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            CollectionAssert.AreEquivalent(new[] { "Porygon", "Porygon2" }, opp.LostZone.Select(c => c.Def.Name).ToArray(), "the evolution stack too (Compendium)");
            Assert.AreEqual("Psychic Energy", opp.Discard.Single().Def.Name, "attached cards are discarded");
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "Prize cards are taken as usual");
        }

        [Test]
        public void Puppet_Offering_Takes_A_Supporter_Back_And_Banette_Goes_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Puppet Offering"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var banette = e.ScenarioBench(0, C(Banette));
            Attach(e, banette, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(BosssOrders), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            Assert.AreEqual("Boss's Orders", p.Hand.Single().Def.Name);
            Assert.AreEqual(0, p.Bench.Count);
            Assert.AreEqual("Banette", p.LostZone.Single().Def.Name);
            Assert.AreEqual("Psychic Energy", p.Discard.Single().Def.Name, "its Energy is discarded");
        }

        [Test]
        public void Lost_Mine_Needs_10_Cards_In_Your_Own_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sableye = e.ScenarioActive(0, C(Sableye));
            Attach(e, sableye, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            FillLostZone(e, 1, 10);
            FillLostZone(e, 0, 9);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Lost Mine"), "the opponent's Lost Zone doesn't count");
            FillLostZone(e, 0, 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Lost Mine"));
        }

        [Test]
        public void Lost_Provisions_Makes_Cramorants_Attack_Free()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Cramorant));
            e.ScenarioActive(1, C(Tauros));
            FillLostZone(e, 0, 3);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Spit Innocently"));
            FillLostZone(e, 0, 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Spit Innocently"), "4 cards in the Lost Zone: no Energy needed");
        }

        // ================================================================== Bench size

        [Test]
        public void Collapsed_Stadium_Cuts_Both_Benches_To_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Collapsed Stadium"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Pikachu));
            e.ScenarioAddToHand(0, C(CollapsedStadium));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.AreEqual(4, e.MaxBench(0));
            Assert.AreEqual(4, e.State.Players[0].Bench.Count);
            Assert.AreEqual(4, e.State.Players[1].Bench.Count);
            Assert.AreEqual("Tauros", e.State.Players[0].Discard.Single().Def.Name);
            Assert.AreEqual("Pikachu", e.State.Players[1].Discard.Single().Def.Name);
        }

        [Test]
        public void Eternal_Zone_Allows_8_Benched_Pokemon_But_Only_Darkness_Ones()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(EternatusVmax));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(CrobatV));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(8, e.MaxBench(0));
            Assert.AreEqual(5, e.MaxBench(1), "only its owner's Bench");
            Assert.IsTrue(Offered(e, 0, "Bench Crobat V"));
            Assert.IsFalse(Offered(e, 0, "Bench Tauros"), "no non-Darkness Pokémon into play");
            Assert.IsTrue(e.PutIntoPlayBlocked(0, C(Tauros)), "by any means (Compendium)");

            e.ScenarioSetStadium(1, C(CollapsedStadium));
            Assert.AreEqual(4, e.MaxBench(0), "Collapsed Stadium: the smaller number wins");
        }

        // ================================================================== Ancient Star (your ruling)

        [Test]
        public void Ancient_Star_Takes_Away_The_Abilities_Of_The_Opponents_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ancient Star"), new ScriptedAgent());
            var aerodactyl = e.ScenarioActive(0, C(AerodactylVstar));
            Attach(e, aerodactyl, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            var pidgeot = e.ScenarioBench(1, C(PidgeotV));
            var kyurem = e.ScenarioBench(1, C(KyuremVmax));
            var komala = e.ScenarioBench(1, C(Komala));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(pidgeot, "Vanishing Wings"));
            Step(e);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
            Assert.IsTrue(e.AbilityBlocked(pidgeot, "Vanishing Wings"));
            Assert.IsTrue(e.AbilityBlocked(kyurem, "Glaciated World"));
            Assert.IsFalse(e.AbilityBlocked(komala, "All Just a Dream"), "only Pokémon V");
            Assert.IsTrue(e.HasAbility(aerodactyl), "it counts as a Pokémon with an Ability (Compendium)");

            e.DiscardPokemonFromPlay(aerodactyl);
            Assert.IsFalse(e.AbilityBlocked(pidgeot, "Vanishing Wings"), "until Aerodactyl VSTAR leaves play");
        }

        [Test]
        public void Ancient_Star_Doesnt_Beat_Shields_That_Were_In_Play_First()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ancient Star"), new ScriptedAgent());
            var aerodactyl = e.ScenarioActive(0, C(AerodactylVstar));
            Attach(e, aerodactyl, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            var enamorus = e.ScenarioBench(1, C(EnamorusV));
            var pidgeot = e.ScenarioBench(1, C(PidgeotV));
            Attach(e, pidgeot, Psychic, 1);
            var kyurem = e.ScenarioBench(1, C(KyuremVmax));
            var corviknight = e.ScenarioBench(1, C(CorviknightVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.AbilityBlocked(enamorus, "Guardian of Love"), "your ruling: Enamorus V was in play first, so Guardian of Love keeps working");
            Assert.IsFalse(e.AbilityBlocked(pidgeot, "Vanishing Wings"), "protected by Guardian of Love (Psychic Energy attached)");
            Assert.IsTrue(e.AbilityBlocked(kyurem, "Glaciated World"), "no Psychic Energy: not protected");
            Assert.IsFalse(e.AbilityBlocked(corviknight, "Lustrous Body"), "Lustrous Body was in play first");

            e.ScenarioAttach(kyurem, C(Psychic));
            Assert.IsFalse(e.AbilityBlocked(kyurem, "Glaciated World"), "the Pokémon Guardian of Love protects keep their Abilities");
        }

        [Test]
        public void Ancient_Star_Beats_Shields_That_Come_Into_Play_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ancient Star"), new ScriptedAgent());
            var aerodactyl = e.ScenarioActive(0, C(AerodactylVstar));
            Attach(e, aerodactyl, Psychic, 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var enamorus = e.ScenarioBench(1, C(EnamorusV));
            var pidgeot = e.ScenarioBench(1, C(PidgeotV));
            Attach(e, pidgeot, Psychic, 1);
            var corviknight = e.ScenarioBench(1, C(CorviknightVmax));
            Assert.IsTrue(e.AbilityBlocked(enamorus, "Guardian of Love"), "your ruling: Ancient Star was there first");
            Assert.IsTrue(e.AbilityBlocked(pidgeot, "Vanishing Wings"));
            Assert.IsTrue(e.AbilityBlocked(corviknight, "Lustrous Body"));
        }

        // ================================================================== Rock-Paper-Scissors (your ruling)

        [Test]
        public void Tricky_Slap_Protects_Mr_Mime_When_You_Win_Rock_Paper_Scissors()
        {
            var e = PersonScenario(new PersonAgent().Then("Attack: Tricky Slap", "Paper"), new PersonAgent().Then("Rock"));
            var mime = e.ScenarioActive(0, C(MrMime));
            Attach(e, mime, Psychic, 3);
            var target = e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, target.DamageCounters);
            Assert.IsTrue(e.State.Effects.Any(fx => fx is ProtectEffect && fx.TargetUid == mime.Uid), "Paper beats Rock");
        }

        [Test]
        public void Tricky_Slap_Ties_Are_Thrown_Again()
        {
            var e = PersonScenario(new PersonAgent().Then("Attack: Tricky Slap", "Rock", "Scissors"), new PersonAgent().Then("Rock", "Rock"));
            var mime = e.ScenarioActive(0, C(MrMime));
            Attach(e, mime, Psychic, 3);
            e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsFalse(e.State.Effects.Any(fx => fx is ProtectEffect && fx.TargetUid == mime.Uid), "Rock beats Scissors: the opponent won");
        }

        // ================================================================== Poison, Sleep and conditions

        [Test]
        public void Nadir_Powder_And_Poison_Peak_Add_Up_To_10_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nadir Powder"), new ScriptedAgent());
            var dustox = e.ScenarioActive(0, C(Dustox));
            Attach(e, dustox, Psychic, 2);
            e.ScenarioBench(0, C(Sneasler));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Confused) && tauros.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(10, e.PoisonCounters(tauros), "8 instead of 1, plus 2 more");
        }

        [Test]
        public void Hazard_Star_And_Poison_Peak_Make_5_Counters()
        {
            // Compendium (2022-09-08): 3 + 2 = 5.
            var e = Scenario(new ScriptedAgent().Then("Use Hazard Star"), new ScriptedAgent());
            e.ScenarioActive(0, C(Drapion));
            e.ScenarioBench(0, C(Sneasler));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            Assert.IsTrue(tauros.Has(SpecialCondition.Paralyzed) && tauros.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(5, e.PoisonCounters(tauros));
            Assert.IsTrue(e.State.Players[0].VstarUsed);
        }

        [Test]
        public void Unfazed_Fat_Stops_The_Effects_Of_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nadir Powder"), new ScriptedAgent());
            var dustox = e.ScenarioActive(0, C(Dustox));
            Attach(e, dustox, Psychic, 2);
            var snorlax = e.ScenarioActive(1, C(Snorlax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(SpecialCondition.None, snorlax.Conditions);
        }

        [Test]
        public void Windup_Arm_Lets_A_Paralyzed_Pokemon_Attack_And_Thumping_Snore_Flips_2_Coins()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Thumping Snore"), new ScriptedAgent());
            var snorlax = e.ScenarioActive(0, C(Snorlax));
            Attach(e, snorlax, Psychic, 3);
            snorlax.Conditions = SpecialCondition.Paralyzed;
            var target = e.ScenarioActive(1, C(MawileV));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Thumping Snore"), "Paralyzed");
            e.ScenarioAttachTool(snorlax, C(WindupArm));
            Assert.IsTrue(Offered(e, 0, "Attack: Thumping Snore"), "Windup Arm");
            Step(e);
            Assert.AreEqual(18, target.DamageCounters);
            Assert.IsTrue(snorlax.Has(SpecialCondition.Asleep));
            Assert.IsFalse(snorlax.Has(SpecialCondition.Paralyzed), "Asleep replaces Paralyzed");
            Assert.AreEqual(2, snorlax.SleepCoins);
        }

        // ================================================================== Prize cards

        [Test]
        public void Elder_Tree_Barrier_Gives_No_Prize_Cards_To_A_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var trevenant = e.ScenarioActive(1, C(Trevenant));
            trevenant.DamageCounters = 3;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(trevenant, e.State.Players[1].Active);
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void All_Just_A_Dream_Gives_No_Prize_Cards_While_Komala_Is_Asleep()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var komala = e.ScenarioActive(1, C(Komala));
            komala.Conditions = SpecialCondition.Asleep;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(komala, e.State.Players[1].Active);
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Box_Of_Disaster_Puts_8_Counters_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Lost Dive"), new ScriptedAgent());
            var aerodactyl = e.ScenarioActive(0, C(AerodactylVstar));
            Attach(e, aerodactyl, Fighting, 1);
            Attach(e, aerodactyl, Psychic, 2);
            var enamorus = e.ScenarioActive(1, C(EnamorusV));
            e.ScenarioAttachTool(enamorus, C(BoxOfDisaster));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(enamorus, e.State.Players[1].Active, "Knocked Out from full HP");
            Assert.AreEqual(8, aerodactyl.DamageCounters);
            Assert.AreEqual(3, e.State.Players[0].LostZone.Count, "Lost Dive put the top 3 cards in the Lost Zone");
        }

        [Test]
        public void Gift_Energy_Draws_Until_7_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var tauros = e.ScenarioActive(1, C(Tauros));
            tauros.DamageCounters = 3;
            e.ScenarioAttach(tauros, C(GiftEnergy));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[1].Hand.Count);
        }

        // ================================================================== Damage changes

        [Test]
        public void Panic_Mask_Stops_Damage_From_A_Pokemon_With_40_HP_Or_Less()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = ChompDowner(e, 0);
            mawile.DamageCounters = 17;
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttachTool(tauros, C(PanicMask));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
        }

        [Test]
        public void Lake_Acuity_Protects_Pokemon_With_Water_Or_Fighting_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Chomp Down"), new ScriptedAgent());
            ChompDowner(e, 0);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioAttach(tauros, C(Water));
            e.ScenarioSetStadium(0, C(LakeAcuity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, tauros.DamageCounters, "100 - 20");
        }

        [Test]
        public void Spirit_Charm_Doesnt_Stack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rolling Shell"), new ScriptedAgent());
            var goodra = e.ScenarioActive(0, C(HisuianGoodraV));
            Attach(e, goodra, Water, 1);
            Attach(e, goodra, Metal, 1);
            Attach(e, goodra, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Clefable));
            e.ScenarioBench(1, C(Clefable));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, tauros.DamageCounters, "140 - 30 from a Dragon Pokémon, once");
        }

        [Test]
        public void Fantina_Shields_Your_Pokemon_From_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Fantina"), new ScriptedAgent().Then("Attack: Chomp Down"));
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Fantina));
            ChompDowner(e, 1);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Fantina"), "needs 10 cards in the Lost Zone");
            FillLostZone(e, 0, 10);
            Assert.IsFalse(e.MainPhaseStep());
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters, "100 - 120");
        }

        [Test]
        public void Crisis_Muscles_Adds_150_HP_Late_In_The_Game()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var machamp = e.ScenarioActive(0, C(Machamp));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(150, e.MaxHp(machamp));
            e.State.Players[1].Prizes.RemoveRange(0, 3);
            Assert.AreEqual(300, e.MaxHp(machamp), "the opponent has 3 Prize cards left");
        }

        // ================================================================== Other cards

        [Test]
        public void Netherworld_Gate_Benches_Gengar_From_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Gengar's Netherworld Gate from your discard pile"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToDiscard(0, C(Gengar), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var gengar = e.State.Players[0].Bench.Single();
            Assert.AreEqual("Gengar", gengar.Name);
            Assert.AreEqual(3, gengar.DamageCounters);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count);
        }

        [Test]
        public void Seething_Currents_Refreshes_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Seething Currents", "your opponent"), new ScriptedAgent());
            e.ScenarioActive(0, C(Kingdra));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 6; i++) e.ScenarioAddToHand(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var opp = e.State.Players[1];
            Assert.AreEqual(4, opp.Hand.Count);
            Assert.IsTrue(opp.Hand.All(c => c.Def.Name == "Psychic Energy"), "drawn from the top");
            Assert.AreEqual(22, opp.Deck.Count);
            Assert.IsTrue(opp.Deck.Skip(16).All(c => c.Def.Name == "Tauros"), "the old hand is at the bottom");
        }

        [Test]
        public void Downgrading_Beam_Shuffles_The_Evolution_Cards_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Downgrading Beam", "2"), new ScriptedAgent());
            var z = e.ScenarioActive(0, C(PorygonZ));
            Attach(e, z, Psychic, 2);
            var target = e.ScenarioActive(1, C(Porygon));
            e.ScenarioEvolve(target, C(Porygon2));
            e.ScenarioEvolve(target, C(PorygonZ));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual("Porygon", opp.Active.Name);
            Assert.AreEqual(0, opp.Hand.Count, "not into the hand");
            Assert.AreEqual(22, opp.Deck.Count);
        }

        [Test]
        public void Thorton_Switches_A_Basic_Pokemon_And_Keeps_Everything_On_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Thorton"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 5;
            Attach(e, tauros, Psychic, 1);
            e.ScenarioAddToDiscard(0, C(MawileV), 1);
            e.ScenarioAddToHand(0, C(Thorton));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var active = e.State.Players[0].Active;
            Assert.AreSame(tauros, active, "the same Pokémon in play");
            Assert.AreEqual("Mawile V", active.Name);
            Assert.AreEqual(5, active.DamageCounters);
            Assert.AreEqual(1, active.Energy.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Tauros"));
        }

        [Test]
        public void Arc_Phone_Switches_The_Top_Card_With_A_Prize_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Arc Phone", "Prize card 1"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Pikachu)));
            e.ScenarioAddToHand(0, C(ArcPhone));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            Assert.AreEqual(6, p.Prizes.Count);
            Assert.IsTrue(p.Prizes.Any(c => c.Def.Name == "Pikachu"));
            Assert.AreEqual("Fire Energy", p.Deck[0].Def.Name);
        }

        [Test]
        public void Riley_Lets_The_Opponent_Choose_2_Cards_To_Discard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Riley"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Riley));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.MainPhaseStep());
            var p = e.State.Players[0];
            Assert.AreEqual(3, p.Hand.Count);
            Assert.AreEqual(3, p.Discard.Count, "2 cards and Riley");
            Assert.AreEqual(15, p.Deck.Count);
        }

        [Test]
        public void Field_Trap_Discards_The_Opponents_Stadium_And_2_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Field Trap"), new ScriptedAgent());
            var stunfisk = e.ScenarioActive(0, C(GalarianStunfisk));
            Attach(e, stunfisk, Metal, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetStadium(1, C(LakeAcuity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.AreEqual(0, tauros.Energy.Count);
            Assert.AreEqual(2, tauros.DamageCounters);
        }

        [Test]
        public void Field_Trap_Leaves_Your_Own_Stadium()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Field Trap"), new ScriptedAgent());
            var stunfisk = e.ScenarioActive(0, C(GalarianStunfisk));
            Attach(e, stunfisk, Metal, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            e.ScenarioSetStadium(0, C(LakeAcuity));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNotNull(e.State.Stadium);
            Assert.AreEqual(2, tauros.Energy.Count);
        }
    }
}
