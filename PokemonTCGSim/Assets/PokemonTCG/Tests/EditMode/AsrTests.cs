using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v53: Astral Radiance (ASR) and its Trainer Gallery: Temple of Sinnoh and Double Turbo Energy, Path to the Peak, Canceling
    /// Cologne, Hisuian Heavy Ball, Stand Sentry, Wish Connector and Resonant Evolution (your rulings), and the other cards with
    /// new effects. Also the rulebook's attack order: effects that cancel an attack come before the Confusion check (your ruling).
    /// </summary>
    public class AsrTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Water = "MEE-11";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Tauros = "MEW-128";
        const string MawileV = "SIT-70";
        const string RustSyndicateGrunt = "PBL-81";

        const string Leafeon = "ASR-13";
        const string Golduck = "ASR-29";
        const string Glaceon = "ASR-38";
        const string HisuianAvalugg = "ASR-48";
        const string HisuianTyphlosionVstar = "ASR-54";
        const string Togetic = "ASR-56";
        const string Togekiss = "ASR-57";
        const string Uxie = "ASR-65";
        const string Mesprit = "ASR-66";
        const string Azelf = "ASR-67";
        const string Wyrdeer = "ASR-69";
        const string HisuianGrowlithe = "ASR-70";
        const string Sudowoodo = "ASR-74";
        const string LucarioV = "ASR-78";
        const string RadiantHawlucha = "ASR-81";
        const string HisuianOverqwil = "ASR-90";
        const string HisuianSneasler = "ASR-93";
        const string Mightyena = "ASR-96";
        const string Registeel = "ASR-108";
        const string Bastiodon = "ASR-110";
        const string Regidrago = "ASR-118";
        const string Eevee = "ASR-119";
        const string Hoothoot = "ASR-120";
        const string OranguruV = "ASR-133";
        const string WyrdeerV = "ASR-134";
        const string CancelingCologne = "ASR-136";
        const string GutsyPickaxe = "ASR-145";
        const string HisuianHeavyBall = "ASR-146";
        const string JubilifeVillage = "ASR-148";
        const string Kamado = "ASR-149";
        const string SpicySeasonedCurry = "ASR-151";
        const string SupereffectiveGlasses = "ASR-152";
        const string SwitchCart = "ASR-154";
        const string TempleOfSinnoh = "ASR-155";
        const string Zisu = "ASR-159";
        const string VirizionV = "ASR-164";
        const string JirachiV = "ASR-170";
        const string PathToThePeak = "ASR-213";
        const string DoubleTurboEnergy = "ASR-216";
        const string Kingdra = "ASR-TG3";
        const string CalyrexVmax = "ASR-TG15";
        const string GalarianZapdosV = "ASR-TG19";

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
        public void Every_Astral_Radiance_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 216; n++) Assert.IsNotNull(db.Get("ASR-" + n), "ASR " + n + " is missing");
            for (int n = 1; n <= 30; n++) Assert.IsNotNull(db.Get("ASR-TG" + n), "ASR TG" + n + " is missing");
            Assert.AreEqual("CRZ-GG1", db.MainPrinting(C("ASR-2")).Key, "Hisuian Voltorb was already in the pool");
            Assert.AreEqual("SIT-165", db.MainPrinting(C("ASR-157")).Key, "Unidentified Fossil too");
            Assert.AreEqual("30C-CC26", db.MainPrinting(C("ASR-TG21")).Key, "and Zacian V");
            Assert.AreEqual("ASR-1", db.MainPrinting(C("ASR-160")).Key, "the full art is a second print of Beedrill V");
            Assert.AreEqual("ASR-TG18", db.MainPrinting(C("ASR-TG30")).Key);
            Assert.AreEqual(0, C("ASR-1").Attacks[1].Damage, "Swarming Sting has no printed damage");
            Assert.AreEqual(0, C("ASR-3").Attacks[0].Cost.Length, "Triple Draw is free");
            Assert.IsTrue(C(HisuianTyphlosionVstar).Attacks[1].VstarPower, "Shimmering Star");
            Assert.IsTrue(C(DoubleTurboEnergy).Text.Contains("2 Colorless"));
            Assert.IsFalse(Formats.IsStandardCard(C(JirachiV)), "regulation mark F: Expanded");
        }

        // ================================================================== Attack order (your ruling)

        [Test]
        public void Effects_That_Cancel_An_Attack_Are_Checked_Before_Confusion()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rage"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Psychic, 2);
            var foe = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            tauros.Conditions = SpecialCondition.Confused;
            // Sand Attack on the attacker, and every coin player 1 flips comes up tails.
            e.AddEffect(new CardsMEG.SandAttackEffect("Sand Attack", 1, tauros.Uid, 4));
            e.AddEffect(new CardsMEW1.OverthinkEffect("Overthink", 1, 4));
            Step(e);
            Assert.AreEqual(0, foe.DamageCounters, "the attack didn't happen");
            Assert.AreEqual(0, tauros.DamageCounters, "the Sand Attack flip came first, so there was no Confusion flip (rulebook order)");
        }

        // ================================================================== Temple of Sinnoh and Double Turbo Energy

        [Test]
        public void Double_Turbo_Energy_Gives_2_And_Temple_Of_Sinnoh_Makes_It_1()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            e.ScenarioAttach(mawile, C(DoubleTurboEnergy));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(2, e.EnergyCount(mawile));
            e.ScenarioSetStadium(1, C(TempleOfSinnoh));
            Assert.AreEqual(1, e.EnergyCount(mawile), "Compendium: 1 [C] under Temple of Sinnoh");
        }

        [Test]
        public void Double_Turbo_Energy_Takes_20_Off_Unless_Temple_Of_Sinnoh_Is_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            e.ScenarioAttach(mawile, C(DoubleTurboEnergy));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.DamageCounters, "30 - 20");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile2 = e2.ScenarioActive(0, C(MawileV));
            Attach(e2, mawile2, Psychic, 1);
            e2.ScenarioAttach(mawile2, C(DoubleTurboEnergy));
            var tauros2 = e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetStadium(1, C(TempleOfSinnoh));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, tauros2.DamageCounters, "no reduction: the Energy has no other effect");
        }

        [Test]
        public void Special_Energy_Under_Temple_Of_Sinnoh_Is_Still_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Crushing Punch"), new ScriptedAgent());
            var lucario = e.ScenarioActive(0, C(LucarioV));
            Attach(e, lucario, Psychic, 2);
            var mawile = e.ScenarioActive(1, C(MawileV));
            e.ScenarioAttach(mawile, C(DoubleTurboEnergy));
            e.ScenarioSetStadium(1, C(TempleOfSinnoh));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, mawile.Energy.Count, "Crushing Punch discards it");
        }

        // ================================================================== Abilities turned off

        [Test]
        public void Path_To_The_Peak_Turns_Off_Rule_Box_Abilities_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(OranguruV));
            e.ScenarioBench(0, C(Wyrdeer));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Use Back Order"));
            e.ScenarioSetStadium(1, C(PathToThePeak));
            Assert.IsFalse(Offered(e, 0, "Use Back Order"), "Oranguru V has a Rule Box");
            Assert.IsTrue(Offered(e, 0, "Use Hurried Gait"), "Wyrdeer doesn't");
        }

        [Test]
        public void Canceling_Cologne_Affects_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Canceling Cologne"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(CancelingCologne));
            var avalugg = e.ScenarioActive(1, C(HisuianAvalugg));
            var sudowoodo = e.ScenarioBench(1, C(Sudowoodo));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(e.AbilityBlocked(avalugg, "Massive Ice"));
            Assert.IsFalse(e.AbilityBlocked(sudowoodo, "Vamoose"));
            e.SwitchActive(1, sudowoodo);
            Assert.IsTrue(e.AbilityBlocked(sudowoodo, "Vamoose"), "the new Active Pokémon has no Abilities either");
            Assert.IsFalse(e.AbilityBlocked(avalugg, "Massive Ice"), "the old one gets them back on the Bench");
        }

        // ================================================================== Trainers

        [Test]
        public void Hisuian_Heavy_Ball_Swaps_Itself_For_A_Basic_Pokemon_In_The_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Hisuian Heavy Ball"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddPrizes(0, C(Tauros), 1);
            e.ScenarioAddToHand(0, C(HisuianHeavyBall));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(7, p.Prizes.Count);
            Assert.IsTrue(p.Hand.Any(c => c.Def.Name == "Tauros"), "the Basic Pokémon went to the hand");
            Assert.IsTrue(p.Prizes.Any(c => c.Def.Name == "Hisuian Heavy Ball"), "Hisuian Heavy Ball took its place");
            Assert.IsFalse(p.Discard.Any(c => c.Def.Name == "Hisuian Heavy Ball"));
        }

        [Test]
        public void Stand_Sentry_Keeps_Basic_Energy_On_The_Bench_From_The_Opponents_Supporter()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Rust Syndicate Grunt"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(RustSyndicateGrunt));
            e.ScenarioActive(1, C(Tauros));
            var hoothoot = e.ScenarioBench(1, C(Hoothoot));
            Attach(e, hoothoot, Psychic, 1);
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].LastKnockedOutTurn = 2;
            Step(e);
            Assert.AreEqual("Rust Syndicate Grunt", e.State.Players[0].Discard.Last().Def.Name, "the Supporter was played");
            Assert.AreEqual(1, hoothoot.Energy.Count, "Stand Sentry");
        }

        [Test]
        public void Gutsy_Pickaxe_Attaches_A_Fighting_Energy_To_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Gutsy Pickaxe"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var bench = e.ScenarioBench(0, C(Tauros));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Fighting)));
            e.ScenarioAddToHand(0, C(GutsyPickaxe));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, bench.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Kamado_Keeps_One_Card_And_Draws_4()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Kamado"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Kamado));
            e.ScenarioAddToHand(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Play Kamado"));
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(5, p.Hand.Count, "1 kept + 4 drawn");
            Assert.AreEqual(3, p.Discard.Count, "2 discarded and Kamado");
        }

        [Test]
        public void Kamado_Needs_Another_Card_In_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Kamado));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Kamado"));
        }

        [Test]
        public void Zisu_Draws_To_One_More_Than_The_Opponent()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Zisu"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Zisu));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 4; i++) e.ScenarioAddToHand(1, C(Grass));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Spicy_Seasoned_Curry_Burns_And_Heals_40()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Spicy Seasoned Curry"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 6;
            e.ScenarioAddToHand(0, C(SpicySeasonedCurry));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, tauros.DamageCounters);
            Assert.IsTrue(tauros.Has(SpecialCondition.Burned));
        }

        [Test]
        public void Switch_Cart_Heals_The_Pokemon_It_Benched_And_Frontier_Road_Moves_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Switch Cart"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 5;
            Attach(e, tauros, Psychic, 2);
            var wyrdeer = e.ScenarioBench(0, C(WyrdeerV));
            e.ScenarioAddToHand(0, C(SwitchCart));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(wyrdeer, e.State.Players[0].Active);
            Assert.AreEqual(2, tauros.DamageCounters, "healed 30 on the Bench");
            Assert.AreEqual(2, wyrdeer.Energy.Count, "Frontier Road moved the Energy over");
        }

        [Test]
        public void Supereffective_Glasses_Make_Weakness_x3()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bite"), new ScriptedAgent());
            var growlithe = e.ScenarioActive(0, C(HisuianGrowlithe));
            Attach(e, growlithe, Fighting, 1);
            Attach(e, growlithe, Psychic, 1);
            e.ScenarioAttachTool(growlithe, C(SupereffectiveGlasses));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, tauros.DamageCounters, "30 x3");
        }

        [Test]
        public void Jubilife_Village_Refreshes_The_Hand_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Jubilife Village"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(JubilifeVillage));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "the turn ends");
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
        }

        // ================================================================== Knock Out triggers (your ruling)

        [Test]
        public void Wish_Connector_Works_When_Jirachi_V_Itself_Is_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            var jirachi = e.ScenarioActive(1, C(JirachiV));
            Attach(e, jirachi, Psychic, 1);
            jirachi.DamageCounters = 15;
            var tauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(tauros, e.State.Players[1].Active, "Jirachi V was Knocked Out");
            Assert.AreEqual(1, tauros.Energy.Count, "your ruling: Wish Connector moved its basic Energy");
        }

        [Test]
        public void Wish_Connector_Saves_Energy_From_Another_Basic_Pokemon_V()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            var foe = e.ScenarioActive(1, C(MawileV));
            Attach(e, foe, Psychic, 1);
            foe.DamageCounters = 17;
            var jirachi = e.ScenarioBench(1, C(JirachiV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, jirachi.Energy.Count);
        }

        [Test]
        public void Deep_Sea_King_Takes_Water_Energy_From_The_Knocked_Out_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent().Then("Water|Water"));
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            Attach(e, tauros, Water, 2);
            tauros.DamageCounters = 11;
            var kingdra = e.ScenarioBench(1, C(Kingdra));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, kingdra.Energy.Count);
        }

        // ================================================================== Evolving (your ruling)

        [Test]
        public void Resonant_Evolution_Evolves_An_Eevee_Benched_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var older = e.ScenarioBench(0, C(Eevee));
            var fresh = e.ScenarioBench(0, C(Eevee));
            e.ScenarioAddToHand(0, C(Leafeon));
            e.ScenarioAddToDeck(0, C(Glaceon), 1);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            fresh.TurnEnteredPlay = 3;
            Step(e);
            Assert.AreEqual("Leafeon", older.Name);
            Assert.AreEqual("Glaceon", fresh.Name, "your ruling: it works on an Eevee that came into play this turn");
        }

        [Test]
        public void Shine_Of_Happiness_Heals_90_From_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 10;
            e.ScenarioBench(0, C(Togetic));
            e.ScenarioAddToHand(0, C(Togekiss));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.DamageCounters);
        }

        // ================================================================== Pokémon

        [Test]
        public void Mental_Shroud_Needs_Uxie_And_Azelf()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mesprit = e.ScenarioActive(0, C(Mesprit));
            e.ScenarioBench(0, C(Uxie));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(EnergyType.Darkness, e.WeaknessOf(mesprit));
            e.ScenarioBench(0, C(Azelf));
            Assert.IsNull(e.WeaknessOf(mesprit));
        }

        [Test]
        public void Shimmering_Star_Knocks_Out_With_Exactly_4_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Shimmering Star"), new ScriptedAgent());
            var typhlosion = e.ScenarioActive(0, C(HisuianTyphlosionVstar));
            Attach(e, typhlosion, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            tauros.DamageCounters = 4;
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(tauros, e.State.Players[1].Active);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
            Assert.IsTrue(e.State.Players[0].VstarUsed);
        }

        [Test]
        public void Heavy_Slam_Does_50_Less_For_Each_Colorless_In_The_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Heavy Slam"), new ScriptedAgent());
            var registeel = e.ScenarioActive(0, C(Registeel));
            Attach(e, registeel, Metal, 3);
            var jirachi = e.ScenarioActive(1, C(JirachiV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(17, jirachi.DamageCounters, "220 - 50 for a Retreat Cost of 1");
        }

        [Test]
        public void Primal_Fortress_Takes_30_Off_Pokemon_V_Attacks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Bastiodon));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, tauros.DamageCounters);
        }

        [Test]
        public void Big_Match_Adds_30_Against_An_Active_Pokemon_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Pouty Slap"), new ScriptedAgent());
            var mawile = e.ScenarioActive(0, C(MawileV));
            Attach(e, mawile, Psychic, 1);
            e.ScenarioBench(0, C(RadiantHawlucha));
            var calyrex = e.ScenarioActive(1, C(CalyrexVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, calyrex.DamageCounters);
        }

        [Test]
        public void Carry_And_Climb_Lowers_The_Active_Retreat_Cost()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(2, e.RetreatCostOf(tauros));
            e.ScenarioBench(0, C(HisuianSneasler));
            Assert.AreEqual(0, e.RetreatCostOf(tauros));
        }

        [Test]
        public void Hustle_Bark_And_Fighting_Instinct_Lower_Attack_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mightyena = e.ScenarioActive(0, C(Mightyena));
            var zapdos = e.ScenarioBench(0, C(GalarianZapdosV));
            e.ScenarioActive(1, C(MawileV));
            Assert.AreEqual(3, e.AttackCost(mightyena, mightyena.Def.Attacks[0]).Count);
            Assert.AreEqual(3, e.AttackCost(zapdos, zapdos.Def.Attacks[0]).Count, "1 of the opponent's Pokémon V");
            e.ScenarioBench(1, C(CalyrexVmax));
            Assert.AreEqual(0, e.AttackCost(mightyena, mightyena.Def.Attacks[0]).Count, "the opponent has a Pokémon VMAX");
            Assert.AreEqual(2, e.AttackCost(zapdos, zapdos.Def.Attacks[0]).Count, "2 of them");
        }

        [Test]
        public void Dragons_Hoard_Works_Only_In_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Regidrago));
            e.ScenarioBench(0, C(Regidrago));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.GetMainActions(0).Count(o => o.Label.StartsWith("Use Dragon's Hoard")));
        }

        [Test]
        public void Tormenting_Poison_Puts_5_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tormenting Poison"), new ScriptedAgent());
            e.ScenarioActive(0, C(HisuianOverqwil));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(5, e.PoisonCounters(tauros));
        }

        [Test]
        public void Verdant_Wind_Protects_Pokemon_With_Grass_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            Attach(e, tauros, Grass, 1);
            tauros.Conditions = SpecialCondition.Asleep;
            e.ScenarioBench(0, C(VirizionV));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.StateChecks();
            Assert.AreEqual(SpecialCondition.None, tauros.Conditions, "recovers");
            Assert.IsFalse(e.ApplyCondition(tauros, SpecialCondition.Poisoned), "and can't be affected");
        }

        [Test]
        public void Entangled_Dive_Discards_Both_Active_Pokemon_Without_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Entangled Dive"), new ScriptedAgent());
            var golduck = e.ScenarioActive(0, C(Golduck));
            Attach(e, golduck, Water, 3);
            e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Tauros", e.State.Players[0].Active.Name);
            Assert.AreEqual(0, e.State.Players[1].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Golduck"));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "not a Knock Out");
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count);
        }
    }
}
