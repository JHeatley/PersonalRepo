using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v59: Chilling Reign (CRE 1-233). Your rulings: Urn of Vitality only takes cards named Single Strike Energy; Greedent's
    /// Brazen Tail doesn't stop the opponent's Pokémon Tools (Rugged Helmet); Aggron's Seismic Rift also hits Aggron.
    /// </summary>
    public class CreTests : ScenarioTestBase
    {
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Darkness = "MEE-15";
        const string Metal = "MEE-16";
        const string Grass = "MEE-9";
        const string Tauros = "MEW-128";

        const string Cinderace = "CRE-28";
        const string Weavile = "CRE-31";
        const string CastformSnowy = "CRE-34";
        const string RapidUrshifu = "CRE-44";
        const string Zebstrika = "CRE-51";
        const string Thundurus = "CRE-52";
        const string GalarianSlowpoke = "CRE-54";
        const string GalarianSirfetchd = "CRE-79";
        const string Runerigus = "CRE-83";
        const string Grapploct = "CRE-92";
        const string Koffing = "CRE-94";
        const string GalarianWeezing = "CRE-96";
        const string GalarianSlowking = "CRE-98";
        const string SlowkingV = "CRE-99";
        const string SlowkingVmax = "CRE-100";
        const string Aggron = "CRE-111";
        const string BlisseyV = "CRE-119";
        const string Kecleon = "CRE-122";
        const string TornadusV = "CRE-124";
        const string TornadusVmax = "CRE-125";
        const string Greedent = "CRE-128";
        const string Agatha = "CRE-129";
        const string Avery = "CRE-130";
        const string DynaTreeHill = "CRE-135";
        const string Flannery = "CRE-139";
        const string Chestplate = "CRE-141";
        const string OldCemetery = "CRE-147";
        const string Peonia = "CRE-149";
        const string SkiesScroll = "CRE-151";
        const string RuggedHelmet = "CRE-152";
        const string ImpactEnergy = "CRE-157";
        const string LuckyEnergy = "CRE-158";
        const string SpiralEnergy = "CRE-159";
        const string RapidashV = "CRE-167";
        const string Electrode = "CRE-222";
        const string Snorlax = "CRE-224";
        const string UrnOfVitality = "CRE-229";

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
        public void Every_Chilling_Reign_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 233; n++) Assert.IsNotNull(db.Get("CRE-" + n), "CRE " + n + " is missing");
            Assert.AreEqual("FST-245", db.MainPrinting(C("CRE-7")).Key, "Celebi V was already in the pool");
            Assert.AreEqual("FST-245", db.MainPrinting(C("CRE-160")).Key, "and its full art");
            Assert.AreEqual("ASR-TG11", db.MainPrinting(C("CRE-223")).Key, "the secret-rare Bronzong");
            Assert.AreEqual("CRZ-GG64", db.MainPrinting(C("CRE-218")).Key, "the rainbow Melony");
            Assert.AreEqual("CRE-8", db.MainPrinting(C("CRE-199")).Key, "the rainbow Celebi VMAX");
            Assert.AreEqual("CRE-167", db.MainPrinting(C("CRE-168")).Key, "the second Galarian Rapidash V");
            Assert.AreEqual("CRE-136", db.MainPrinting(C("CRE-225")).Key, "the gold Echoing Horn");
            Assert.IsFalse(Formats.IsStandardCard(C(Cinderace)), "regulation mark E: Expanded");
            Assert.IsTrue(C(Cinderace).IsSingleStrike);
            Assert.IsTrue(C(Weavile).IsRapidStrike);
            Assert.IsTrue(C(ImpactEnergy).IsSingleStrike && C(ImpactEnergy).IsSpecialEnergy);
            Assert.IsTrue(C("CRE-136").IsRapidStrike, "Echoing Horn is a Rapid Strike card");
            Assert.AreEqual("Galarian Slowpoke", C(GalarianSlowking).EvolvesFrom);
            Assert.AreEqual(0, C("CRE-33").Attacks[0].Damage, "Rainfall has no printed damage");
            Assert.AreEqual(1, C(SkiesScroll).Attacks.Count);
            Assert.IsTrue(C(SkiesScroll).IsTool);
        }

        // ================================================================== Your rulings

        [Test]
        public void Urn_Of_Vitality_Doesnt_Take_Impact_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(UrnOfVitality));
            e.ScenarioAddToDiscard(0, C(ImpactEnergy), 2);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Urn of Vitality"), "your ruling: only cards named Single Strike Energy");
        }

        [Test]
        public void Brazen_Tail_Keeps_Special_Energy_From_Flannery()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Flannery"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Flannery));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioAttach(blissey, C(LuckyEnergy));
            e.ScenarioBench(1, C(Greedent));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.Energy.Count, "Brazen Tail");

            var e2 = Scenario(new ScriptedAgent().Then("Play Flannery"), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(Flannery));
            var b2 = e2.ScenarioActive(1, C(BlisseyV));
            e2.ScenarioAttach(b2, C(LuckyEnergy));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, b2.Energy.Count, "without Greedent, Flannery discards it");
        }

        [Test]
        public void Brazen_Tail_Doesnt_Stop_Rugged_Helmet()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(Cinderace));
            Attach(e, cinderace, Fire, 1);
            Attach(e, cinderace, Psychic, 1);
            e.ScenarioBench(0, C(Greedent));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioAttachTool(blissey, C(RuggedHelmet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, blissey.DamageCounters);
            Assert.AreEqual(1, cinderace.Energy.Count, "your ruling: Tools aren't stopped by Brazen Tail");
            Assert.AreEqual(1, e.State.Players[0].Hand.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Seismic_Rift_Hits_Aggron_And_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Seismic Rift"), new ScriptedAgent());
            var aggron = e.ScenarioActive(0, C(Aggron));
            Attach(e, aggron, Metal, 4);
            var tauros = e.ScenarioBench(0, C(Tauros));
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(24, blissey.DamageCounters);
            Assert.AreEqual(3, aggron.DamageCounters, "your ruling: Aggron too");
            Assert.AreEqual(3, tauros.DamageCounters);
        }

        // ================================================================== Pokémon

        [Test]
        public void Two_Hit_KO_Knocks_Out_The_Defending_Pokemon_When_A_Rapid_Strike_Attack_Damages_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Two-Hit KO", "Attack: Slashing Claw"), new ScriptedAgent());
            var weavile = e.ScenarioActive(0, C(Weavile));
            Attach(e, weavile, Psychic, 1);
            var urshifu = e.ScenarioBench(0, C(RapidUrshifu));
            Attach(e, urshifu, Psychic, 1);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            e.SwitchActive(0, urshifu);
            Step(e);
            Assert.AreNotEqual(tauros, e.State.Players[1].Active, "40 damage, but Two-Hit KO Knocks it Out");
            Assert.IsTrue(e.State.Players[1].Discard.Any(c => c.Def.Name == "Tauros"));
        }

        [Test]
        public void Word_Of_Ruin_Knocks_Out_At_The_End_Of_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Word of Ruin"), new ScriptedAgent());
            var slowking = e.ScenarioActive(0, C(SlowkingV));
            Attach(e, slowking, Darkness, 2);
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(tauros, e.State.Players[1].Active);
            e.EndTurn();
            Assert.AreNotEqual(tauros, e.State.Players[1].Active);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Max_Toxify_Poison_Puts_12_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Max Toxify"), new ScriptedAgent());
            var slowking = e.ScenarioActive(0, C(SlowkingV));
            e.ScenarioEvolve(slowking, C(SlowkingVmax));
            Attach(e, slowking, Darkness, 2);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, blissey.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(13, blissey.DamageCounters);
        }

        [Test]
        public void Energy_Factory_Doubles_Basic_Darkness_Energy_On_Weezing_Only_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var weezing = e.ScenarioActive(0, C(GalarianWeezing));
            Attach(e, weezing, Darkness, 1);
            var koffing = e.ScenarioBench(0, C(Koffing));
            Attach(e, koffing, Darkness, 1);
            e.ScenarioBench(0, C(GalarianWeezing));
            e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(2, e.EnergyCountOfType(weezing, EnergyType.Darkness), "2 Energy Factories don't make 3");
            Assert.AreEqual(1, e.EnergyCountOfType(koffing, EnergyType.Darkness), "Koffing doesn't have Weezing in its name");
        }

        [Test]
        public void Chromashift_Makes_Kecleon_The_Type_Of_Its_Basic_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var kecleon = e.ScenarioActive(0, C(Kecleon));
            e.ScenarioActive(1, C(Tauros));
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Colorless));
            Attach(e, kecleon, Fire, 1);
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Fire));
            Assert.IsFalse(e.IsType(kecleon, EnergyType.Colorless));
            Attach(e, kecleon, Grass, 1);
            Assert.IsTrue(e.IsType(kecleon, EnergyType.Fire) && e.IsType(kecleon, EnergyType.Grass));
        }

        [Test]
        public void Weather_Reading_Needs_8_Stadiums_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(CastformSnowy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToDiscard(0, C(DynaTreeHill), 7);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Frosty Typhoon"));
            e.ScenarioAddToDiscard(0, C(DynaTreeHill), 1);
            Assert.IsTrue(Offered(e, 0, "Attack: Frosty Typhoon"), "no Energy needed");
        }

        [Test]
        public void Crisis_Power_Adds_30_For_Each_Prize_The_Opponent_Took()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(Cinderace));
            Attach(e, cinderace, Fire, 2);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.State.Players[1].Prizes.RemoveRange(0, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(21, blissey.DamageCounters, "150 + 2 x 30");
        }

        [Test]
        public void Coordinated_Bolt_Needs_Another_Rapid_Strike_Attacker_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Coordinated Bolt"), new ScriptedAgent());
            var zebstrika = e.ScenarioActive(0, C(Zebstrika));
            Attach(e, zebstrika, Lightning, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            var me = e.State.Players[0];
            me.LastAttackTurn = 1;
            me.LastAttackerDef = C(RapidUrshifu);
            me.LastAttackerUid = 9999;
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, blissey.DamageCounters, "30 + 90");

            var e2 = Scenario(new ScriptedAgent().Then("Attack: Coordinated Bolt"), new ScriptedAgent());
            var z2 = e2.ScenarioActive(0, C(Zebstrika));
            Attach(e2, z2, Lightning, 1);
            var b2 = e2.ScenarioActive(1, C(BlisseyV));
            var me2 = e2.State.Players[0];
            me2.LastAttackTurn = 1;
            me2.LastAttackerDef = C(Zebstrika);
            me2.LastAttackerUid = z2.Uid;
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(3, b2.DamageCounters, "Zebstrika itself doesn't count");
        }

        [Test]
        public void Libra_Horn_Leaves_100_HP()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Libra Horn"), new ScriptedAgent());
            var rapidash = e.ScenarioActive(0, C(RapidashV));
            Attach(e, rapidash, Psychic, 2);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(15, blissey.DamageCounters);
        }

        [Test]
        public void Stranglehold_Master_Adds_2_To_The_Opponents_Retreat_Cost_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var grapploct = e.ScenarioActive(0, C(Grapploct));
            var tauros = e.ScenarioActive(1, C(Tauros));
            Assert.AreEqual(4, e.RetreatCostOf(tauros));
            Assert.AreEqual(3, e.RetreatCostOf(grapploct), "not on its own side");
            var bench = e.ScenarioBench(0, C(Tauros));
            e.SwitchActive(0, bench);
            Assert.AreEqual(2, e.RetreatCostOf(tauros), "not from the Bench");
        }

        [Test]
        public void Spiteful_Slate_Returns_The_Damage_To_A_Pokemon_VMAX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Blasting Wind"), new ScriptedAgent());
            var tornadus = e.ScenarioActive(0, C(TornadusV));
            e.ScenarioEvolve(tornadus, C(TornadusVmax));
            Attach(e, tornadus, Psychic, 2);
            var runerigus = e.ScenarioActive(1, C(Runerigus));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, runerigus.DamageCounters);
            Assert.AreEqual(6, tornadus.DamageCounters);
        }

        [Test]
        public void Gormandize_Draws_To_7_And_Ends_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Gormandize"), new ScriptedAgent());
            e.ScenarioActive(0, C(Snorlax));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.MainPhaseStep(), "Gormandize ends the turn");
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Buzzap_Generator_Attaches_2_Lightning_Energy_And_Knocks_Out_Electrode()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Buzzap Generator", "Lightning Energy|Lightning Energy", "Lightning Energy", "Thundurus", "Thundurus"), new ScriptedAgent());
            var thundurus = e.ScenarioActive(0, C(Thundurus));
            e.ScenarioBench(0, C(Electrode));
            e.ScenarioAddToDeck(0, C(Lightning), 2);
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, thundurus.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Bench.Count);
            Assert.IsTrue(e.State.Players[0].Discard.Any(c => c.Def.Name == "Electrode"));
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count, "the opponent takes a Prize card");
        }

        // ================================================================== Trainers

        [Test]
        public void Old_Cemetery_Puts_2_Counters_On_Non_Psychic_Pokemon_Getting_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Fire Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(OldCemetery));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(2, tauros.DamageCounters);

            var e2 = Scenario(new ScriptedAgent().Then("Attach Psychic Energy"), new ScriptedAgent());
            var rapidash = e2.ScenarioActive(0, C(RapidashV));
            e2.ScenarioAddToHand(0, C(Psychic));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetStadium(1, C(OldCemetery));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(0, rapidash.DamageCounters, "Psychic Pokémon are spared");
        }

        [Test]
        public void Dyna_Tree_Hill_Stops_Healing()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            tauros.DamageCounters = 3;
            e.ScenarioSetStadium(0, C(DynaTreeHill));
            Assert.AreEqual(0, e.Heal(tauros, 30));
            Assert.AreEqual(3, tauros.DamageCounters);
        }

        [Test]
        public void Galarian_Chestplate_Takes_30_Off_For_A_Galarian_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(Cinderace));
            Attach(e, cinderace, Fire, 2);
            var sirfetchd = e.ScenarioActive(1, C(GalarianSirfetchd));
            e.ScenarioAttachTool(sirfetchd, C(Chestplate));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, sirfetchd.DamageCounters, "150 - 30");
        }

        [Test]
        public void Rapid_Strike_Scroll_Of_The_Skies_Gives_Gravdrop()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gravdrop"), new ScriptedAgent());
            var urshifu = e.ScenarioActive(0, C(RapidUrshifu));
            e.ScenarioAttachTool(urshifu, C(SkiesScroll));
            Attach(e, urshifu, Lightning, 1);
            Attach(e, urshifu, Psychic, 1);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            Attach(e, blissey, Psychic, 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(11, blissey.DamageCounters, "10 + 2 x 50");
        }

        [Test]
        public void Avery_Leaves_The_Opponent_3_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Avery"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Avery));
            e.ScenarioActive(1, C(Tauros));
            for (int i = 0; i < 5; i++) e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Bench.Count);
            Assert.AreEqual(3, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Agatha_Moves_Up_To_3_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Agatha", "3"), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            mine.DamageCounters = 5;
            e.ScenarioAddToHand(0, C(Agatha));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, mine.DamageCounters);
            Assert.AreEqual(3, theirs.DamageCounters);
        }

        [Test]
        public void Peonia_Swaps_Prize_Cards_With_Cards_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Peonia", "3"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioAddToHand(0, C(Peonia));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioAddToHand(0, C(Grass));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(6, p.Prizes.Count);
            Assert.AreEqual(2, p.Hand.Count);
        }

        // ================================================================== Energy

        [Test]
        public void Impact_Energy_Only_Goes_On_Single_Strike_Pokemon_And_Stops_Poison()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var sirfetchd = e.ScenarioActive(0, C(GalarianSirfetchd));
            e.ScenarioAddToHand(0, C(ImpactEnergy));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(Offered(e, 0, "Attach Impact Energy"));
            e.ScenarioAttach(sirfetchd, C(ImpactEnergy));
            Assert.IsFalse(e.ApplyCondition(sirfetchd, SpecialCondition.Poisoned));
            Assert.IsTrue(e.ApplyCondition(sirfetchd, SpecialCondition.Confused), "only Poison");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e2.ScenarioActive(0, C(Tauros));
            e2.ScenarioAddToHand(0, C(ImpactEnergy));
            e2.ScenarioActive(1, C(Tauros));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e2, 0, "Attach Impact Energy"), "Tauros isn't a Single Strike Pokémon");
        }

        [Test]
        public void Spiral_Energy_Stops_Paralysis_On_A_Rapid_Strike_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var urshifu = e.ScenarioActive(0, C(RapidUrshifu));
            e.ScenarioAttach(urshifu, C(SpiralEnergy));
            e.ScenarioActive(1, C(Tauros));
            Assert.IsFalse(e.ApplyCondition(urshifu, SpecialCondition.Paralyzed));
            Assert.IsTrue(e.ApplyCondition(urshifu, SpecialCondition.Poisoned));
        }

        [Test]
        public void Lucky_Energy_Draws_A_Card_When_Its_Active_Pokemon_Is_Damaged()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Fireball Shot"), new ScriptedAgent());
            var cinderace = e.ScenarioActive(0, C(Cinderace));
            Attach(e, cinderace, Fire, 2);
            var blissey = e.ScenarioActive(1, C(BlisseyV));
            e.ScenarioAttach(blissey, C(LuckyEnergy));
            e.ScenarioSetTurn(0, 3);
            int before = e.State.Players[1].Hand.Count;
            Step(e);
            Assert.AreEqual(before + 1, e.State.Players[1].Hand.Count);
        }
    }
}
