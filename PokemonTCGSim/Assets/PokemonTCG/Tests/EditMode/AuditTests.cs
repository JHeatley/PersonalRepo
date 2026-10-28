using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v134: the rulings audit. Compendium rulings for cards already in the pool that hadn't been applied (RULES_NOTES 266), and your
    /// two audit rulings (Galvantula's Double Thread erratum is in StsTests; Channeler removes the effects of both players' attacks).
    /// </summary>
    public class AuditTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";

        const string Electivire = "FFI-30";
        const string Magmortar = "DRX-21";
        const string DeoxysEx = "PLF-53";
        const string PlasmaEnergy = "PLB-91";
        const string Dce = "SLG-69";
        const string AlolanRaichu = "UNM-57";
        const string TelescopicSight = "VIV-160";
        const string LaprasGX = "SUM-35";
        const string RotomVstar = "CRZ-46";
        const string Cinccino = "SHF-SV94";
        const string Houndoom = "BRS-TG10";
        const string SingleStrikeEnergy = "BST-141";
        const string Gothitelle = "GRI-54";
        const string Beedrill = "PRC-3";
        const string MrMime = "PRC-101";
        const string HeadRinger = "PHF-97";
        const string Lickilicky = "BST-114";
        const string Chandelure = "UNM-30";
        const string Lampent = "UNM-29";
        const string Skiploom = "EVS-3";
        const string Jumpluff = "EVS-4";
        const string BoostShake = "EVS-142";
        const string Raihan = "CRZ-140";
        const string Steelix = "UNM-104";
        const string Channeler = "UNM-190";
        const string LumineonV = "CRZ-GG39";
        const string TingLuEx = "PAF-244";
        const string GapejawBog = "SIT-213";
        const string Chansey = "DEX-80";
        const string Blissey = "DEX-82";
        const string Eevee = "DEX-84";
        const string Hooligans = "DEX-95";
        const string DustIsland = "UNB-168";
        const string Switch = "MEG-130";
        const string WeavileGX = "UNM-132";
        const string GreensExploration = "UNB-175";
        const string PowerPlant = "CEC-269";
        const string Lysandre = "AOR-78";
        const string BossesOrders = "MEG-114";
        const string LysandresTrumpCard = "PHF-99";
        const string ErikasInvitation = "MEW-160";
        const string RainbowEnergy = "CES-151";
        const string Hoppip = "EVS-2";
        const string Rockruff = "HIF-SV23";
        const string LycanrocGX = "HIF-SV66";
        const string StealthyHood = "UNB-186";
        const string Darkness = "MEE-15";
        const string Water = "MEE-11";
        const string SharpedoEx = "PRC-91";
        const string Nidoking = "TEU-59";
        const string Milotic = "EVS-38";
        const string SordwardAndShielbert = "SIT-TG28";
        const string Riley = "LOR-166";
        const string Carracosta = "UNM-45";
        const string CorviknightVmax = "SIT-TG19";
        const string GalarianWeezing = "SHF-42";
        const string Ludicolo = "CES-38";
        const string Slaking = "CES-115";
        const string Garbodor = "BKP-57";
        const string Klefki = "PAF-159";
        const string EmpoleonV = "BST-40";
        const string LatiosGX = "UNM-78";
        const string KeldeoGX = "UNM-47";
        const string Sneasel = "UNM-131";
        const string Flygon = "BCR-99";
        const string Serperior = "LTR-8";
        const string ArceusDialgaPalkiaGX = "CEC-156";
        const string Metal = "MEE-16";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private void Attach(GameEngine e, PokemonInPlay mon, string energy, int count)
        {
            for (int i = 0; i < count; i++) e.ScenarioAttach(mon, C(energy));
        }

        [Test]
        public void Tag_Team_Spark_Counts_The_Energy_On_All_Your_Magmortar()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tag Team Spark"), new ScriptedAgent());
            var electivire = e.ScenarioActive(0, C(Electivire));
            Attach(e, electivire, Lightning, 1);
            Attach(e, e.ScenarioBench(0, C(Magmortar)), Fire, 1);
            Attach(e, e.ScenarioBench(0, C(Magmortar)), Fire, 2);
            var blissey = e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(8, blissey.DamageCounters, "20 + 20 x (1 + 2)");
        }

        [Test]
        public void Helix_Force_Counts_Double_Colorless_Energy_As_2()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Helix Force"), new ScriptedAgent());
            var deoxys = e.ScenarioActive(0, C(DeoxysEx));
            Attach(e, deoxys, Psychic, 1);
            Attach(e, deoxys, PlasmaEnergy, 1);
            var blissey = e.ScenarioActive(1, C(Blissey));
            Attach(e, blissey, Dce, 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, blissey.DamageCounters, "30 + 30 x 2");
        }

        [Test]
        public void Electro_Rain_Hits_A_Pokemon_Chosen_Twice_Once_For_Telescopic_Sight()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Electro Rain", "Lightning|Lightning", "Lapras-GX", "Lapras-GX"), new ScriptedAgent());
            var raichu = e.ScenarioActive(0, C(AlolanRaichu));
            Attach(e, raichu, Lightning, 2);
            e.ScenarioAttachTool(raichu, C(TelescopicSight));
            e.ScenarioActive(1, C(Chansey));
            var lapras = e.ScenarioBench(1, C(LaprasGX));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(9, lapras.DamageCounters, "30 + 30 as one hit, +30 Telescopic Sight once");
        }

        [Test]
        public void Conversion_Star_And_Make_Do_Need_A_Card_In_The_Deck()
        {
            foreach (var key in new[] { RotomVstar, Cinccino })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                e.ScenarioActive(0, C(key));
                e.ScenarioAddToHand(0, C(Hooligans));
                e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                string ability = key == RotomVstar ? "Conversion Star" : "Make Do";
                Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains(ability)), ability);
                e.State.Players[0].Deck.Clear();
                Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains(ability)), ability + " with an empty deck");
            }
        }

        [Test]
        public void Single_Strike_Roar_Stops_When_All_4_Single_Strike_Energy_Are_Known()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Houndoom));
            e.ScenarioAddToDeck(0, C(SingleStrikeEnergy), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Single Strike Roar")));
            e.ScenarioAddToDiscard(0, C(SingleStrikeEnergy), 4);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Single Strike Roar")));
        }

        [Test]
        public void Tractorbeam_Does_Nothing_Without_A_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tractorbeam"), new ScriptedAgent());
            var gothitelle = e.ScenarioActive(0, C(Gothitelle));
            Attach(e, gothitelle, Psychic, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, chansey.DamageCounters);
        }

        [Test]
        public void Allergic_Shock_Lets_A_Lethal_Hit_Count_As_A_Knock_Out_By_Damage()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Allergic Shock"), new ScriptedAgent());
            var beedrill = e.ScenarioActive(0, C(Beedrill));
            Attach(e, beedrill, Grass, 1);
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            e.DealAttackDamage(beedrill, chansey, 100);
            Assert.IsFalse(chansey.ForcedKnockOut, "the damage Knocked it Out, not Allergic Shock");
        }

        [Test]
        public void Trick_Discards_A_Team_Flare_Tool_Moved_To_A_Pokemon_That_Isnt_A_Pokemon_EX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Trick"), new ScriptedAgent());
            var mime = e.ScenarioActive(0, C(MrMime));
            Attach(e, mime, Psychic, 1);
            var deoxys = e.ScenarioActive(1, C(DeoxysEx));
            e.ScenarioAttachTool(deoxys, C(HeadRinger));
            var eevee = e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, deoxys.Tools.Count);
            Assert.AreEqual(0, eevee.Tools.Count);
            Assert.IsTrue(e.State.Players.Any(p => p.Discard.Any(c => c.Def.Key == HeadRinger)));
        }

        [Test]
        public void Selickt_Discards_From_The_Deck_When_The_Hand_Is_Empty()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Selickt"), new ScriptedAgent());
            var lickilicky = e.ScenarioActive(0, C(Lickilicky));
            Attach(e, lickilicky, Psychic, 3);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Discard.Count);
        }

        [Test]
        public void Spirit_Burner_Can_Bench_A_Stage_1_Fire_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Spirit Burner", "Lampent"), new ScriptedAgent());
            var chandelure = e.ScenarioActive(0, C(Chandelure));
            Attach(e, chandelure, Fire, 1);
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C(Lampent)));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, e.State.Players[0].Bench.Count(m => m.Name == "Lampent"));
        }

        [Test]
        public void Boost_Shake_Can_Evolve_A_Stage_1_That_Evolved_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boost Shake"), new ScriptedAgent());
            var skiploom = e.ScenarioActive(0, C("EVS-2"));
            e.ScenarioEvolve(skiploom, C(Skiploom));
            skiploom.TurnEvolved = 3;
            e.ScenarioAddToHand(0, C(BoostShake));
            e.ScenarioAddToDeck(0, C(Jumpluff), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Jumpluff", skiploom.Name);
        }

        [Test]
        public void Raihan_Needs_A_Basic_Energy_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Raihan));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            e.State.Players[0].LastKnockedOutTurn = 2;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Raihan")));
            e.ScenarioAddToDiscard(0, C(Fire), 1);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Raihan")));
        }

        [Test]
        public void Channeler_Removes_Your_Own_Gigaton_Shake()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gigaton Shake", "Play Channeler"), new ScriptedAgent());
            var steelix = e.ScenarioActive(0, C(Steelix));
            Attach(e, steelix, Fighting, 5);
            e.ScenarioAddToHand(0, C(Channeler));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack:")), "Gigaton Shake: no attacks this turn");
            Step(e);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack:")), "your ruling: Channeler removes it");
        }

        [Test]
        public void Luminous_Sign_Can_Go_Before_Gapejaw_Bog()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Lumineon V", "Lumineon V's effect first", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(LumineonV));
            e.ScenarioAddToDeck(0, C(Hooligans), 1);
            e.ScenarioActive(1, C(TingLuEx));
            e.ScenarioSetStadium(1, C(GapejawBog));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var lumineon = e.State.Players[0].Bench.Single();
            Assert.AreEqual(2, lumineon.DamageCounters, "Gapejaw Bog");
            Assert.IsTrue(e.State.Players[0].Hand.Any(c => c.Def.Key == Hooligans), "Luminous Sign went first, before Cursed Land shut it off");
        }
        [Test]
        public void Dust_Island_Passes_A_Stronger_Poison_On()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Switch"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            chansey.Conditions = SpecialCondition.Poisoned;
            chansey.PoisonCounterBase = 4;
            var eevee = e.ScenarioBench(0, C(Eevee));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetStadium(1, C(DustIsland));
            e.ScenarioAddToHand(0, C(Switch));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(eevee, e.State.Players[0].Active);
            Assert.IsTrue(eevee.Has(SpecialCondition.Poisoned));
            Assert.AreEqual(4, eevee.PoisonCounterBase, "Severe Poison's 4 counters follow");
        }

        [Test]
        public void Greens_Exploration_Counts_Only_Working_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(WeavileGX));
            e.ScenarioAddToHand(0, C(GreensExploration));
            e.ScenarioAddToDeck(0, C(Switch), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Green's Exploration")), "Weavile-GX has an Ability");
            e.ScenarioSetStadium(1, C(PowerPlant));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Green's Exploration")), "Power Plant: Pokémon-GX have no Abilities");
        }

        [Test]
        public void Lysandre_And_Bosss_Orders_Cant_Share_A_Deck()
        {
            var deck = new[] { C(Lysandre), C(BossesOrders) }.ToList();
            for (int i = 0; i < 4; i++) deck.Add(C(Chansey));
            while (deck.Count < 60) deck.Add(C(Psychic));
            Assert.IsTrue(Decklist.Validate(deck, DeckFormat.Expanded).Any(p => p.Contains("Lysandre, Boss's Orders")));
            deck[1] = C(Lysandre);
            Assert.IsFalse(Decklist.Validate(deck, DeckFormat.Expanded).Any(p => p.Contains("Boss's Orders")));
        }

        [Test]
        public void Lysandres_Trump_Card_Leaves_Every_Lysandres_Trump_Card_In_The_Discard_Pile()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Lysandre's Trump Card"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(0, C(LysandresTrumpCard));
            e.ScenarioAddToDiscard(0, C(LysandresTrumpCard), 1);
            e.ScenarioAddToDiscard(0, C(Psychic), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Discard.Count(c => c.Def.Key == LysandresTrumpCard));
            Assert.AreEqual(0, e.State.Players[0].Discard.Count(c => c.Def.Key == Psychic));
        }

        [Test]
        public void Gapejaw_Bog_Hits_The_Pokemon_Erikas_Invitation_Benches()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Erika's Invitation"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(ErikasInvitation));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioAddToHand(1, C(Eevee));
            e.ScenarioSetStadium(0, C(GapejawBog));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var eevee = e.State.Players[1].Active;
            Assert.AreEqual("Eevee", eevee.Name);
            Assert.AreEqual(2, eevee.DamageCounters);
        }

        [Test]
        public void Solar_Evolution_Goes_Before_Rainbow_Energys_Damage_Counter()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Rainbow Energy", "Yes"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C(Hoppip));
            e.ScenarioEvolve(mon, C(Skiploom));
            e.ScenarioAddToHand(0, C(RainbowEnergy));
            e.ScenarioAddToDeck(0, C(Jumpluff), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Jumpluff", mon.Name);
            Assert.AreEqual(0, mon.DamageCounters, "the counter was for the Skiploom it was attached to");
        }

        [Test]
        public void Bloodthirsty_Eyes_Gets_Past_Stealthy_Hood_On_The_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve", "Yes", "Eevee"), new ScriptedAgent());
            e.ScenarioActive(0, C(Rockruff));
            e.ScenarioAddToHand(0, C(LycanrocGX));
            var chansey = e.ScenarioActive(1, C(Chansey));
            e.ScenarioAttachTool(chansey, C(StealthyHood));
            var eevee = e.ScenarioBench(1, C(Eevee));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(eevee, e.State.Players[1].Active, "only the Benched Pokémon is targeted");
        }

        [Test]
        public void Attaching_Energy_From_The_Hand_With_Reacting_Cards_In_Play_Works()
        {
            // v134.1: the reaction loop read one entry past the end of its list whenever a card in play could react to the
            // attachment, which threw in most games (the regional simulation counted them as draws).
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Chansey (Active)"), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioBench(0, C(Ludicolo));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(CorviknightVmax));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, chansey.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        // ------------------------------------------------------------------ v134, second pass

        [Test]
        public void Hunt_And_Drag_Off_Do_No_Damage_Without_A_New_Active_Pokemon()
        {
            foreach (var key in new[] { SharpedoEx, Nidoking })
            {
                string attack = key == SharpedoEx ? "Hunt" : "Drag Off";
                var e = Scenario(new ScriptedAgent().Then("Attack: " + attack), new ScriptedAgent());
                var attacker = e.ScenarioActive(0, C(key));
                Attach(e, attacker, Darkness, 2);
                var chansey = e.ScenarioActive(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(0, chansey.DamageCounters, attack + ": no Benched Pokémon, so no new Active Pokémon to damage");
            }
        }

        [Test]
        public void Sordward_And_Shielbert_Does_Nothing_Against_Dew_Guard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Sordward & Shielbert"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(SordwardAndShielbert));
            e.ScenarioAddToDiscard(0, C(Switch), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Milotic));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "no Trainer card back and no draw");
            Assert.AreEqual(deck, e.State.Players[0].Deck.Count);
        }

        [Test]
        public void Riley_Keeps_All_5_Cards_Against_Dew_Guard()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Riley"), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Riley));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioBench(1, C(Milotic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count, "the opponent can't choose any to discard");
            Assert.AreEqual(0, e.State.Players[0].Discard.Count(c => c.Def.Key == Psychic));
        }

        [Test]
        public void Ancient_Custom_Leaves_Stealthy_Hood_Working()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var chansey = e.ScenarioActive(0, C(Chansey));
            e.ScenarioAttachTool(chansey, C(StealthyHood));
            var blissey = e.ScenarioBench(0, C(Blissey));
            e.ScenarioAttachTool(blissey, C(TelescopicSight));
            e.ScenarioActive(1, C(Carracosta));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.ToolWorks(chansey, chansey.Tools[0]), "Stealthy Hood protects its holder (2020-03-12)");
            Assert.IsFalse(e.ToolWorks(blissey, blissey.Tools[0]), "Ancient Custom");
        }

        [Test]
        public void Lustrous_Body_Keeps_Working_Under_Neutralizing_Gas()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var corviknight = e.ScenarioBench(0, C(CorviknightVmax));
            var ludicolo = e.ScenarioBench(0, C(Ludicolo));
            e.ScenarioActive(1, C(GalarianWeezing));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.AbilityBlocked(corviknight, "Lustrous Body"), "Lustrous Body wins whichever came first (2021-04-08)");
            Assert.IsTrue(e.AbilityBlocked(ludicolo, "Swing Dance"));
        }

        [Test]
        public void The_Ability_Lock_That_Was_On_First_Wins()
        {
            // Garbotoxin was on first: Slaking moving into the Active Spot later can't turn Lazy on.
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            var slaking = e.ScenarioBench(0, C(Slaking));
            e.ScenarioActive(1, C(Chansey));
            var garbodor = e.ScenarioBench(1, C(Garbodor));
            e.ScenarioAttachTool(garbodor, C(TelescopicSight));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.SwitchActive(0, slaking));
            Assert.IsTrue(e.AbilityBlocked(slaking, "Lazy"), "Garbotoxin was on first");
            Assert.IsFalse(e.AbilityBlocked(garbodor, "Garbotoxin"));

            // Lazy was on first: Garbodor getting its Tool later can't turn Garbotoxin on.
            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking2 = e2.ScenarioActive(0, C(Slaking));
            e2.ScenarioActive(1, C(Chansey));
            var garbodor2 = e2.ScenarioBench(1, C(Garbodor));
            e2.ScenarioSetTurn(0, 3);
            e2.AttachTool(e2.NewCard(1, C(TelescopicSight)), garbodor2);
            Assert.IsTrue(e2.AbilityBlocked(garbodor2, "Garbotoxin"), "Lazy was on first");
            Assert.IsFalse(e2.AbilityBlocked(slaking2, "Lazy"));
        }

        [Test]
        public void Ability_Locks_Turned_On_Together_Go_To_The_Turn_Player()
        {
            foreach (int turnPlayer in new[] { 0, 1 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var klefki = e.ScenarioActive(0, C(Klefki));
                var empoleon = e.ScenarioActive(1, C(EmpoleonV));
                e.ScenarioSetTurn(turnPlayer, 3 + turnPlayer);
                Assert.AreEqual(turnPlayer == 0, e.AbilityBlocked(empoleon, "Emperor's Eyes"), "Mischievous Lock");
                Assert.AreEqual(turnPlayer == 1, e.AbilityBlocked(klefki, "Mischievous Lock"), "Emperor's Eyes");
            }
        }

        [Test]
        public void Clear_Vision_GX_Cant_Reach_A_Keldeo_GX_That_Comes_Into_Play_Later()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clear Vision-GX"), new ScriptedAgent());
            var latios = e.ScenarioActive(0, C(LatiosGX));
            e.ScenarioAttach(latios, C(Psychic));
            for (int i = 0; i < 4; i++) e.ScenarioBench(0, C(Sneasel));
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var keldeo = e.ScenarioBench(1, C(KeldeoGX));
            var weavile = e.ScenarioBench(1, C(WeavileGX));
            Assert.IsFalse(e.AttackNamedPrevented(keldeo, "Resolute Blade-GX"), "Pure Heart keeps Clear Vision-GX off it (2019)");
            Assert.IsTrue(e.AttackNamedPrevented(weavile, "Nocturnal Maneuvers-GX"));
        }

        [Test]
        public void Serperiors_Owner_Orders_Sand_Slammer_And_Royal_Heal()
        {
            foreach (string first in new[] { "Sand Slammer", "Royal Heal" })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then(first));
                e.ScenarioActive(0, C(Flygon));
                var serperior = e.ScenarioActive(1, C(Serperior));
                var chansey = e.ScenarioBench(1, C(Chansey));
                e.ScenarioSetTurn(0, 3);
                e.EndTurn();
                int expected = first == "Sand Slammer" ? 0 : 1;
                Assert.AreEqual(expected, chansey.DamageCounters, first + " first");
                Assert.AreEqual(expected, serperior.DamageCounters, first + " first");
            }
        }

        [Test]
        public void Raihan_After_An_Extra_Turn_Looks_At_The_Opponents_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Chansey));
            e.ScenarioAddToHand(0, C(Raihan));
            e.ScenarioAddToDiscard(0, C(Psychic), 1);
            e.ScenarioActive(1, C(Chansey));
            e.ScenarioSetTurn(0, 5);
            e.State.Players[0].LastKnockedOutTurn = 4;
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Raihan")));
            e.EndTurn();
            e.State.ExtraTurnFor = 0;
            e.StartTurn();
            Assert.AreEqual(0, e.State.CurrentPlayer);
            Assert.AreEqual(7, e.State.TurnNumber);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Raihan")), "the opponent's last turn is still turn 4 (2022-07-07)");
            NextTurn(e);
            NextTurn(e);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Raihan")), "not after the opponent's next turn");
        }

        [Test]
        public void Only_The_GX_Users_Channeler_Removes_Altered_Creation_GX()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Altered Creation-GX", "Play Channeler"), new ScriptedAgent().Then("Play Channeler"));
            var arceus = e.ScenarioActive(0, C(ArceusDialgaPalkiaGX));
            Attach(e, arceus, Metal, 1);
            e.ScenarioAddToHand(0, C(Channeler));
            e.ScenarioActive(1, C(Blissey));
            e.ScenarioAddToHand(1, C(Channeler));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Func<bool> on = () => e.State.Effects.Any(x => x.Name.StartsWith("Altered Creation-GX", StringComparison.Ordinal));
            Assert.IsTrue(on());
            NextTurn(e);
            Step(e);
            Assert.IsTrue(on(), "the opponent's Channeler leaves it (2020-01-09)");
            NextTurn(e);
            Step(e);
            Assert.IsFalse(on(), "the GX user's own Channeler removes it");
        }
    }
}
