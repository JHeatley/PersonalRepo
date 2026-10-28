using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// Sets added in v11 (Twilight Masquerade, Shrouded Fable, Stellar Crown, Surging Sparks, Prismatic Evolutions,
    /// Journey Together): every card in real games, plus the engine features they brought in.
    /// </summary>
    public class NewSetTests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        static readonly string[] Energy = { "MEE-9", "MEE-10", "MEE-11", "MEE-12", "MEE-13", "MEE-14", "MEE-15", "MEE-16" };

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        private static int NumberOf(CardDef d)
        {
            int n;
            return int.TryParse(d.Number, out n) ? n : 9999;
        }

        /// <summary>Decks that between them hold one of every card of the set (about 40 per deck, filled with Basic Energy).</summary>
        private List<List<CardDef>> SetDecks(string set)
        {
            var cards = db.Cards.Where(d => d.SetCode == set).OrderBy(NumberOf).ToList();
            int count = System.Math.Max(2, (cards.Count + 39) / 40);
            int per = (cards.Count + count - 1) / count;
            var decks = new List<List<CardDef>>();
            for (int k = 0; k < count; k++)
            {
                var deck = cards.Skip(k * per).Take(per).ToList();
                if (!deck.Any(d => d.IsBasicPokemon))
                    for (int i = 0; i < 4; i++) deck.Add(C(Dreepy));
                for (int i = 0; deck.Count < 60; i++) deck.Add(C(Energy[(i + k) % Energy.Length]));
                decks.Add(deck);
            }
            return decks;
        }

        [TestCase("SCR")]
        [TestCase("JTG")]
        [TestCase("TWM")]
        [TestCase("SFA")]
        [TestCase("SSP")]
        [TestCase("PRE")]
        [TestCase("DRI")]
        [TestCase("WHT")]
        [TestCase("BLK")]
        [TestCase("MEG")]
        [TestCase("PFL")]
        [TestCase("MEP")]
        [TestCase("ASC")]
        [TestCase("POR")]
        [TestCase("CRI")]
        [TestCase("PBL")]
        [TestCase("30C")]
        [TestCase("SVP")]
        [TestCase("PAF")]
        [TestCase("PAR")]
        [TestCase("MEW")]
        [TestCase("OBF")]
        [TestCase("PAL")]
        [TestCase("SVI")]
        [TestCase("WP")]
        [TestCase("CRZ")]
        [TestCase("SIT")]
        [TestCase("LOR")]
        [TestCase("PGO")]
        [TestCase("ASR")]
        [TestCase("BRS")]
        [TestCase("FST")]
        [TestCase("CEL")]
        [TestCase("EVS")]
        [TestCase("CRE")]
        [TestCase("BST")]
        [TestCase("SHF")]
        [TestCase("VIV")]
        [TestCase("CPA")]
        [TestCase("DAA")]
        [TestCase("RCL")]
        [TestCase("SSH")]
        [TestCase("SP")]
        [TestCase("CEC")]
        [TestCase("HIF")]
        [TestCase("UNM")]
        [TestCase("UNB")]
        [TestCase("DET")]
        [TestCase("TEU")]
        [TestCase("LOT")]
        [TestCase("DRM")]
        [TestCase("CES")]
        [TestCase("FLI")]
        [TestCase("UPR")]
        [TestCase("CIN")]
        [TestCase("SLG")]
        [TestCase("BUS")]
        [TestCase("GRI")]
        [TestCase("SUM")]
        [TestCase("SMP")]
        [TestCase("EVO")]
        [TestCase("STS")]
        [TestCase("FCO")]
        [TestCase("GEN")]
        [TestCase("BKP")]
        [TestCase("BKT")]
        [TestCase("AOR")]
        [TestCase("ROS")]
        [TestCase("DCR")]
        [TestCase("PRC")]
        [TestCase("PHF")]
        [TestCase("FFI")]
        [TestCase("FLF")]
        [TestCase("XY")]
        [TestCase("KSS")]
        [TestCase("XYP")]
        [TestCase("LTR")]
        [TestCase("PLB")]
        [TestCase("PLF")]
        [TestCase("PLS")]
        [TestCase("BCR")]
        [TestCase("BS")]
        [TestCase("JU")]
        [TestCase("FO")]
        [TestCase("TR")]
        [TestCase("G1")]
        [TestCase("DRV")]
        [TestCase("DRX")]
        [TestCase("G2")]
        [TestCase("DEX")]
        [TestCase("NXD")]
        [TestCase("NVI")]
        [TestCase("EPO")]
        [TestCase("BLW")]
        [TestCase("BWP")]
        [TestCase("HSP")]
        [TestCase("DPP")]
        [TestCase("NP")]
        [TestCase("N1")]
        [TestCase("N2")]
        [TestCase("SI")]
        [TestCase("N3")]
        [TestCase("N4")]
        [TestCase("LC")]
        [TestCase("E1")]
        [TestCase("BG")]
        [TestCase("E2")]
        [TestCase("E3")]
        [TestCase("RM")]
        [TestCase("RS")]
        public void Every_Card_Of_The_Set_Survives_Random_Play(string set)
        {
            var decks = SetDecks(set);
            Assert.Greater(decks.Count, 1, "no cards for " + set);
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
                for (int j = 0; j < decks.Count; j++)
                {
                    if (i == j) continue;
                    var report = MatchRunner.Run(scripts, decks[i], decks[j], 3, 9100 + i * 100 + j * 10, (seat, s) => new RandomAgent(s), true);
                    foreach (var err in report.Errors) errors.Add(set + " deck " + i + " vs " + j + ": " + err);
                }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }

        [TestCase("SCR")]
        [TestCase("JTG")]
        [TestCase("TWM")]
        [TestCase("SFA")]
        [TestCase("SSP")]
        [TestCase("PRE")]
        [TestCase("DRI")]
        [TestCase("WHT")]
        [TestCase("BLK")]
        [TestCase("MEG")]
        [TestCase("PFL")]
        [TestCase("MEP")]
        [TestCase("ASC")]
        [TestCase("POR")]
        [TestCase("CRI")]
        [TestCase("PBL")]
        [TestCase("30C")]
        [TestCase("SVP")]
        [TestCase("PAF")]
        [TestCase("PAR")]
        [TestCase("MEW")]
        [TestCase("OBF")]
        [TestCase("PAL")]
        [TestCase("SVI")]
        [TestCase("WP")]
        [TestCase("CRZ")]
        [TestCase("SIT")]
        [TestCase("LOR")]
        [TestCase("PGO")]
        [TestCase("ASR")]
        [TestCase("BRS")]
        [TestCase("FST")]
        [TestCase("CEL")]
        [TestCase("EVS")]
        [TestCase("CRE")]
        [TestCase("BST")]
        [TestCase("SHF")]
        [TestCase("VIV")]
        [TestCase("CPA")]
        [TestCase("DAA")]
        [TestCase("RCL")]
        [TestCase("SSH")]
        [TestCase("SP")]
        [TestCase("CEC")]
        [TestCase("HIF")]
        [TestCase("UNM")]
        [TestCase("UNB")]
        [TestCase("DET")]
        [TestCase("TEU")]
        [TestCase("LOT")]
        [TestCase("DRM")]
        [TestCase("CES")]
        [TestCase("FLI")]
        [TestCase("UPR")]
        [TestCase("CIN")]
        [TestCase("SLG")]
        [TestCase("BUS")]
        [TestCase("GRI")]
        [TestCase("SUM")]
        [TestCase("SMP")]
        [TestCase("EVO")]
        [TestCase("STS")]
        [TestCase("FCO")]
        [TestCase("GEN")]
        [TestCase("BKP")]
        [TestCase("BKT")]
        [TestCase("AOR")]
        [TestCase("ROS")]
        [TestCase("DCR")]
        [TestCase("PRC")]
        [TestCase("PHF")]
        [TestCase("FFI")]
        [TestCase("FLF")]
        [TestCase("XY")]
        [TestCase("KSS")]
        [TestCase("XYP")]
        [TestCase("LTR")]
        [TestCase("PLB")]
        [TestCase("PLF")]
        [TestCase("PLS")]
        [TestCase("BCR")]
        [TestCase("BS")]
        [TestCase("JU")]
        [TestCase("FO")]
        [TestCase("TR")]
        [TestCase("G1")]
        [TestCase("DRV")]
        [TestCase("DRX")]
        [TestCase("G2")]
        [TestCase("DEX")]
        [TestCase("NXD")]
        [TestCase("NVI")]
        [TestCase("EPO")]
        [TestCase("BLW")]
        [TestCase("BWP")]
        [TestCase("HSP")]
        [TestCase("DPP")]
        [TestCase("NP")]
        [TestCase("N1")]
        [TestCase("N2")]
        [TestCase("SI")]
        [TestCase("N3")]
        [TestCase("N4")]
        [TestCase("LC")]
        [TestCase("E1")]
        [TestCase("BG")]
        [TestCase("E2")]
        [TestCase("E3")]
        [TestCase("RM")]
        [TestCase("RS")]
        public void Every_Card_Of_The_Set_Plays_Full_Games_With_Heuristic_Bots(string set)
        {
            var decks = SetDecks(set);
            var scripts = StandardCards.CreateRegistry();
            var errors = new List<string>();
            for (int i = 0; i < decks.Count; i++)
            {
                var report = MatchRunner.Run(scripts, decks[i], decks[(i + 1) % decks.Count], 3, 9500 + i * 10, (seat, s) => new HeuristicAgent(s), true);
                foreach (var err in report.Errors) errors.Add(set + " deck " + i + ": " + err);
            }
            CollectionAssert.IsEmpty(errors, string.Join("\n", errors.ToArray()));
        }

        // ------------------------------------------------------------------ Antique Fossils (your ruling)

        [Test]
        public void Antique_Fossil_Is_An_Item_Played_Onto_The_Bench_As_A_60_HP_Basic()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Antique Root Fossil"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            var card = e.ScenarioAddToHand(0, C("SCR-130"));
            Assert.IsTrue(card.Def.IsItem, "it's an Item card");
            Assert.IsFalse(card.Def.IsBasicPokemon, "not a Basic Pokémon card (no setup, no mulligan help)");
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var fossil = e.State.Players[0].Bench.Single();
            Assert.AreEqual(60, e.MaxHp(fossil));
            Assert.AreEqual(0, e.PrizesFor(fossil), "no Prize cards for it");
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.DiscardFossil), "can be discarded during your turn");
        }

        [Test]
        public void Item_Lock_Stops_Antique_Fossils()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("SCR-129"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Bench Antique Cover Fossil"));
            e.State.Players[0].ItemLockTurn = 3;
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Bench Antique Cover Fossil"));
        }

        [Test]
        public void Antique_Fossil_Cannot_Retreat_Or_Be_Poisoned_And_Lileep_Evolves_From_It()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var fossil = e.ScenarioActive(0, C("SCR-130"));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("SCR-5"));
            e.ScenarioSetTurn(0, 3);
            var actions = e.GetMainActions(0);
            Assert.IsFalse(actions.Any(o => o.Action == ActionType.Retreat), "can't retreat");
            Assert.IsTrue(actions.Any(o => o.Action == ActionType.Evolve && o.Label.Contains("Lileep")), "Lileep evolves from it");
            Assert.IsFalse(e.ApplyCondition(fossil, SpecialCondition.Poisoned), "no Special Conditions");
        }

        // ------------------------------------------------------------------ Other Stellar Crown engine features

        [Test]
        public void Wide_Wall_Stops_Boss_s_Orders()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boss's Orders"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var rhyperior = e.ScenarioActive(1, C("SCR-76"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEG-114"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(rhyperior, e.State.Players[1].Active, "the Supporter had no effect on the opponent's Pokémon");
        }

        [Test]
        public void Emergency_Rotation_Benches_Klinklang_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Emergency Rotation"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            var klinklang = e.ScenarioAddToHand(0, C("SCR-101"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.UseHandAbility), "needs an opposing Stage 2");
            e.ScenarioBench(1, C("SCR-76"));
            Step(e);
            Assert.AreEqual("Klinklang", e.State.Players[0].Bench.Single().Name);
            Assert.IsFalse(e.State.Players[0].Hand.Contains(klinklang));
        }

        [Test]
        public void Disorienting_Flash_Confusion_Puts_8_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Disorienting Flash"), new ScriptedAgent());
            var lanturn = e.ScenarioActive(0, C("SCR-49"));
            e.ScenarioAttach(lanturn, C(Psychic));
            e.ScenarioAttach(lanturn, C(Psychic));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsTrue(target.Has(SpecialCondition.Confused));
            Assert.AreEqual(8, target.ConfusionCounters);
        }

        [Test]
        public void All_Out_Attack_Does_More_After_Form_Ranks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Form Ranks", "Attack: All-Out Attack"), new ScriptedAgent());
            var falinks = e.ScenarioActive(0, C("SCR-88"));
            e.ScenarioAttach(falinks, C(Psychic));
            e.ScenarioAttach(falinks, C(Psychic));
            var target = e.ScenarioActive(1, C("SCR-76"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "30 + 90");
        }

        // ------------------------------------------------------------------ Journey Together

        [Test]
        public void Auto_Heal_Heals_The_Pokemon_You_Attach_Energy_To_While_Magearna_Is_Active()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Dreepy"), new ScriptedAgent());
            var magearna = e.ScenarioActive(0, C("JTG-107"));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            dreepy.DamageCounters = 5;
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, dreepy.Energy.Count);
            Assert.AreEqual(0, dreepy.DamageCounters, "healed 90 (all 50)");
            Assert.AreSame(magearna, e.State.Players[0].Active);
        }

        [Test]
        public void Auto_Heal_Does_Nothing_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Psychic Energy to Dreepy"), new ScriptedAgent());
            var dreepy = e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C("JTG-107"));
            dreepy.DamageCounters = 5;
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, dreepy.DamageCounters);
        }

        [Test]
        public void Slumbering_Smack_Puts_Both_To_Sleep_And_Adds_100_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slumbering Smack", "Attack: Slumbering Smack"), new ScriptedAgent());
            var komala = e.ScenarioActive(0, C("JTG-129"));
            e.ScenarioAttach(komala, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters, "30 the first time");
            Assert.IsTrue(target.Has(SpecialCondition.Asleep), "the Defending Pokémon falls asleep");
            Assert.IsTrue(komala.Has(SpecialCondition.Asleep), "Komala falls asleep too");
            NextTurn(e);
            Step(e);
            NextTurn(e);
            e.RemoveCondition(komala, SpecialCondition.Asleep);
            Step(e);
            Assert.AreEqual(3 + 13, target.DamageCounters, "30 + 100 during the next turn");
        }

        [Test]
        public void Daunting_Gaze_Stops_Items_Only_While_Tyranitar_Is_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            var tyranitar = e.ScenarioBench(1, C("JTG-95"));
            e.ScenarioAddToHand(0, C("JTG-156"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Redeemable Ticket")), "Tyranitar on the Bench");
            e.SwitchActive(1, tyranitar);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Redeemable Ticket")), "Tyranitar Active");
        }

        [Test]
        public void Secret_Forest_Path_Lowers_The_Active_Retreat_Cost_From_The_Bench()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C("JTG-79"));
            e.ScenarioActive(1, C(Dreepy));
            Assert.AreEqual(4, e.RetreatCostOf(mamoswine));
            e.ScenarioBench(0, C("JTG-89"));
            Assert.AreEqual(2, e.RetreatCostOf(mamoswine));
            e.ScenarioBench(0, C("JTG-89"));
            Assert.AreEqual(0, e.RetreatCostOf(mamoswine));
        }

        [Test]
        public void Lillies_Pearl_Gives_1_Fewer_Prize_When_Knocked_Out_By_An_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rumbling March"), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C("JTG-79"));
            e.ScenarioAttach(mamoswine, C("MEE-14"));
            e.ScenarioAttach(mamoswine, C("MEE-14"));
            var cutiefly = e.ScenarioActive(1, C("JTG-66"));
            e.ScenarioAttachTool(cutiefly, C("JTG-151"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreNotSame(cutiefly, e.State.Players[1].Active, "Knocked Out");
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "no Prize card for it");
        }

        [Test]
        public void Redeemable_Ticket_Replaces_The_Prizes_With_The_Top_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Redeemable Ticket"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("JTG-156"));
            e.ScenarioSetTurn(0, 3);
            int deck = e.State.Players[0].Deck.Count;
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(6, me.Prizes.Count);
            Assert.AreEqual(deck, me.Deck.Count);
            Assert.IsTrue(me.Prizes.All(c => c.Def.Name == "Psychic Energy"), "new Prizes came from the top of the deck");
            Assert.IsTrue(me.Deck.Skip(me.Deck.Count - 6).All(c => c.Def.Name == "Fire Energy"), "old Prizes went to the bottom");
        }
        // ------------------------------------------------------------------ Twilight Masquerade

        [Test]
        public void Slow_Crunch_Knocks_Out_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Slow Crunch"), new ScriptedAgent());
            var pinsir = e.ScenarioActive(0, C("TWM-3"));
            e.ScenarioAttach(pinsir, C("MEE-9"));
            e.ScenarioAttach(pinsir, C(Psychic));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, pinsir.Energy.Count, "Pinsir discards all its Energy");
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid), "still in play during the opponent's turn");
            Step(e);
            NextTurn(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid), "Knocked Out at the end of the opponent's turn");
        }

        [Test]
        public void Permeating_Chill_Puts_9_Counters_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Permeating Chill"), new ScriptedAgent());
            var glaceon = e.ScenarioActive(0, C("TWM-54"));
            e.ScenarioAttach(glaceon, C("MEE-11"));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(12, target.DamageCounters);
        }

        [Test]
        public void Quick_Sign_Is_The_Only_Attack_On_The_First_Players_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var volbeat = e.ScenarioActive(0, C("TWM-9"));
            e.ScenarioAttach(volbeat, C(Psychic));
            e.ScenarioAttach(volbeat, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 1);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            CollectionAssert.Contains(labels, "Attack: Quick Sign");
            CollectionAssert.DoesNotContain(labels, "Attack: Coordinated Strike");
            e.ScenarioSetTurn(0, 3);
            CollectionAssert.Contains(e.GetMainActions(0).Select(o => o.Label).ToList(), "Attack: Coordinated Strike");
        }

        [Test]
        public void Scream_Stops_Supporters_During_The_Opponents_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Scream"));
            e.ScenarioActive(0, C(Dreepy));
            var tail = e.ScenarioActive(1, C("TWM-94"));
            e.ScenarioAttach(tail, C(Psychic));
            e.ScenarioAddToHand(0, C("JTG-142"));
            e.ScenarioSetTurn(1, 2);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(0, e.State.CurrentPlayer);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Billy")), "no Supporters this turn");
        }

        [Test]
        public void Zero_To_Hero_Switches_In_Palafin_Ex_When_Palafin_Retreats()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat Palafin"), new ScriptedAgent());
            var palafin = e.ScenarioActive(0, C("TWM-60"));
            e.ScenarioAttach(palafin, C(Psychic));
            palafin.DamageCounters = 2;
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("TWM-61"), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual("Palafin ex", palafin.Name, "same Pokémon, new card");
            Assert.IsTrue(me.Bench.Contains(palafin));
            Assert.AreEqual(2, palafin.DamageCounters, "damage counters stay");
            Assert.IsTrue(me.Deck.Any(c => c.Def.Name == "Palafin"), "Palafin went into the deck");
        }

        [Test]
        public void Palafin_Ex_Cannot_Evolve_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-59"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("TWM-61"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve));
        }

        [Test]
        public void Survival_Brace_Saves_A_Full_HP_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Rumbling March"), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C("JTG-79"));
            e.ScenarioAttach(mamoswine, C("MEE-14"));
            e.ScenarioAttach(mamoswine, C("MEE-14"));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAttachTool(target, C("TWM-164"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(target, e.State.Players[1].Active, "not Knocked Out");
            Assert.AreEqual(10, e.RemainingHp(target));
            Assert.IsNull(target.Tool, "Survival Brace is discarded");
        }

        [Test]
        public void Mentally_Calm_Stops_Scoop_Up_Cyclone()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C("TWM-50"));
            e.ScenarioAddToHand(0, C("TWM-162"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Scoop Up Cyclone")), "Milotic in play");
            e.State.Players[1].Bench.Clear();
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("Scoop Up Cyclone")), "no Milotic");
        }

        [Test]
        public void Freezing_Shroud_Hits_Pokemon_With_Abilities_During_Checkup()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var froslass = e.ScenarioBench(0, C("TWM-53"));
            var magearna = e.ScenarioActive(1, C("JTG-107"));
            e.ScenarioSetTurn(0, 3);
            e.EndTurn();
            Assert.AreEqual(1, magearna.DamageCounters);
            Assert.AreEqual(0, froslass.DamageCounters, "not Froslass");
        }

        [Test]
        public void Gutsy_Swing_Is_Free_With_A_Special_Condition()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var conkeldurr = e.ScenarioActive(0, C("TWM-105"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Gutsy Swing"));
            e.ApplyCondition(conkeldurr, SpecialCondition.Poisoned);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Gutsy Swing"));
        }

        [Test]
        public void Drum_Beating_Raises_Attack_And_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Drum Beating"), new ScriptedAgent());
            var rillaboom = e.ScenarioActive(0, C("TWM-16"));
            e.ScenarioAttach(rillaboom, C("MEE-9"));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            int before = e.RetreatCostOf(target);
            Step(e);
            NextTurn(e);
            Assert.AreEqual(before + 1, e.RetreatCostOf(target));
            Assert.AreEqual(3, e.AttackCost(target, target.Def.Attacks[0]).Count, "Rumbling March costs 1 more");
        }
        // ------------------------------------------------------------------ Shrouded Fable

        [Test]
        public void Daydream_Ends_The_Turn_When_Energy_Is_Attached_To_The_Defending_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Daydream"), new ScriptedAgent().Then("Attach Psychic Energy to Mamoswine ex"));
            var hypno = e.ScenarioActive(0, C("SFA-17"));
            e.ScenarioAttach(hypno, C(Psychic));
            e.ScenarioAttach(hypno, C(Psychic));
            e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            bool over = e.MainPhaseStep();
            Assert.IsTrue(over, "attaching the Energy ended the turn");
        }

        [Test]
        public void Poison_Fang_Puts_2_Counters_In_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Poison Fang"), new ScriptedAgent());
            var crobat = e.ScenarioActive(0, C("SFA-29"));
            e.ScenarioAttach(crobat, C("MEE-15"));
            e.ScenarioAttach(crobat, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(14, target.DamageCounters);
        }

        [Test]
        public void Crescent_Purge_Turns_A_Prize_Face_Up_For_80_More()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Crescent Purge", "Prize card 1"), new ScriptedAgent());
            var cresselia = e.ScenarioActive(0, C("SFA-21"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(cresselia, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(16, target.DamageCounters, "80 + 80");
            Assert.AreEqual(1, e.State.Players[0].FaceUpPrizeIds.Count);
            var view = PlayerView.Create(e, 1);
            Assert.AreEqual(1, view.Sides[0].PrizeSlots.Count(c => c != null), "the opponent sees the face-up Prize card");
        }

        [Test]
        public void Unnerve_Stops_Boss_s_Orders()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boss's Orders"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var active = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C("SFA-45"));
            e.ScenarioAddToHand(0, C("MEG-114"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(active, e.State.Players[1].Active);
        }

        [Test]
        public void Massive_Body_Stops_Stadiums_While_Active()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C("SFA-42"));
            e.ScenarioAddToHand(0, C("JTG-150"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Levincia")));
        }

        [Test]
        public void Bring_Down_The_Axe_Knocks_Out_A_Pokemon_With_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Bring Down the Axe"), new ScriptedAgent());
            var haxorus = e.ScenarioActive(0, C("SFA-46"));
            e.ScenarioAttach(haxorus, C("MEE-14"));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioAttach(target, C("JTG-159"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
        }
        // ------------------------------------------------------------------ Surging Sparks

        [Test]
        public void Technical_Machine_Gives_Its_Attack_And_Is_Discarded_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dreepy = e.ScenarioActive(0, C(Dreepy));
            e.ScenarioAttach(dreepy, C("MEE-9"));
            e.ScenarioAttach(dreepy, C("MEE-11"));
            e.ScenarioAttach(dreepy, C(Psychic));
            e.ScenarioAttachTool(dreepy, C("SSP-188"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Attack: Fluorite")));
            e.EndTurn();
            Assert.IsNull(dreepy.Tool);
        }

        [Test]
        public void Boosted_Evolution_Lets_An_Active_Eevee_Evolve_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C("SSP-143"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("TWM-11"));
            e.ScenarioSetTurn(0, 1);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve), "Active Eevee");
            e.ScenarioBench(0, C(Dreepy));
            e.SwitchActive(0, e.State.Players[0].Bench[0]);
            Assert.IsTrue(e.State.Players[0].Bench.Contains(eevee));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve), "Benched Eevee");
        }

        [Test]
        public void Frigid_Fangs_Stops_Pokemon_With_2_Or_Fewer_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Frigid Fangs"), new ScriptedAgent());
            var walrein = e.ScenarioActive(0, C("SSP-45"));
            e.ScenarioAttach(walrein, C("MEE-11"));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioAttach(target, C("MEE-14"));
            e.ScenarioAttach(target, C("MEE-14"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsTrue(e.AttacksPrevented(target));
            e.ScenarioAttach(target, C("MEE-14"));
            Assert.IsFalse(e.AttacksPrevented(target), "3 Energy");
        }

        [Test]
        public void Corrosive_Shards_Stops_Energy_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Corrosive Shards"), new ScriptedAgent());
            var glimmora = e.ScenarioActive(0, C("SSP-115"));
            e.ScenarioAttach(glimmora, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioAddToHand(1, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsTrue(target.Has(SpecialCondition.Poisoned));
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Action == ActionType.AttachEnergy && o.PokemonUid == target.Uid));
        }

        [Test]
        public void Metal_Defender_Removes_Weakness_For_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Metal Defender"), new ScriptedAgent());
            var archaludon = e.ScenarioActive(0, C("SSP-130"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(archaludon, C("MEE-16"));
            e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsNotNull(e.WeaknessOf(archaludon));
            Step(e);
            NextTurn(e);
            Assert.IsNull(e.WeaknessOf(archaludon));
        }

        [Test]
        public void Sticky_Bind_Turns_Off_Benched_Stage_2_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mamoswine = e.ScenarioActive(0, C("JTG-79"));
            var benched = e.ScenarioBench(0, C("JTG-79"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C("SSP-107"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.HasAbility(mamoswine), "Active Stage 2 keeps it");
            Assert.IsFalse(e.HasAbility(benched), "Benched Stage 2 loses it");
        }

        [Test]
        public void Familial_March_Puts_Maushold_Onto_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Familial March", "Maushold|Maushold"), new ScriptedAgent());
            var maushold = e.ScenarioActive(0, C("SSP-158"));
            e.ScenarioAttach(maushold, C(Psychic));
            e.ScenarioAddToDeck(0, C("SSP-158"), 2);
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[0].Bench.Count(b => b.Name == "Maushold"));
        }
        // ------------------------------------------------------------------ Prismatic Evolutions

        [Test]
        public void Rainbow_DNA_Lets_Eevee_Ex_Evolve_Only_Into_Eeveelution_Ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("PRE-75"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("PRE-6"));
            e.ScenarioAddToHand(0, C("TWM-11"));
            e.ScenarioSetTurn(0, 3);
            var evolve = e.GetMainActions(0).Where(o => o.Action == ActionType.Evolve).Select(o => o.Card.Name).ToList();
            CollectionAssert.Contains(evolve, "Leafeon ex");
            CollectionAssert.DoesNotContain(evolve, "Leafeon");
        }

        [Test]
        public void Rainbow_DNA_Does_Not_Work_With_Grand_Tree()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var eevee = e.ScenarioActive(0, C("PRE-75"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("PRE-6"), 1);
            e.ScenarioSetStadium(0, C("SCR-136"));
            e.ScenarioSetTurn(0, 3);
            // Grand Tree evolves from the deck, so Rainbow DNA doesn't apply; with no other evolution path for Eevee ex,
            // Grand Tree can't be used at all (Compendium, Stellar Crown FAQ 2024-09-12).
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Grand Tree")));
            Assert.AreEqual("Eevee ex", eevee.Name);
            Assert.IsTrue(e.State.Players[0].Deck.Any(c => c.Def.Name == "Leafeon ex"));
        }

        [Test]
        public void Onyx_Takes_A_Prize_Card()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Onyx"), new ScriptedAgent());
            var umbreon = e.ScenarioActive(0, C("PRE-60"));
            e.ScenarioAttach(umbreon, C("MEE-12"));
            e.ScenarioAttach(umbreon, C(Psychic));
            e.ScenarioAttach(umbreon, C("MEE-15"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
            Assert.AreEqual(0, umbreon.Energy.Count);
        }

        [Test]
        public void Insomnia_Stops_Sleep_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var hoothoot = e.ScenarioActive(0, C("PRE-77"));
            e.ScenarioActive(1, C(Dreepy));
            Assert.IsFalse(e.ApplyCondition(hoothoot, SpecialCondition.Asleep));
            Assert.IsTrue(e.ApplyCondition(hoothoot, SpecialCondition.Poisoned));
        }

        [Test]
        public void Amazez_Shuffles_Evolution_Cards_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amazez"), new ScriptedAgent());
            var espeon = e.ScenarioActive(0, C("PRE-34"));
            e.ScenarioAttach(espeon, C("MEE-9"));
            e.ScenarioAttach(espeon, C(Psychic));
            e.ScenarioAttach(espeon, C("MEE-15"));
            var target = e.ScenarioActive(1, C("PRE-47"));
            e.ScenarioEvolve(target, C("PRE-48"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Larvitar", target.Name);
            Assert.IsTrue(e.State.Players[1].Deck.Any(c => c.Def.Name == "Pupitar"));
        }

        [Test]
        public void Amarys_Discards_A_Big_Hand_At_The_End_Of_The_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Amarys"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("PRE-93"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            e.EndTurn();
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Amp_You_Very_Much_Takes_An_Extra_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Amp You Very Much"), new ScriptedAgent());
            var hands = e.ScenarioActive(0, C("PRE-31"));
            e.ScenarioAttach(hands, C("MEE-12"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(hands, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count);
        }
        // ------------------------------------------------------------------ Destined Rivals

        [Test]
        public void Buzzing_Boost_Attaches_Grass_Energy_When_Yanmega_Ex_Moves_To_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes", "Grass Energy|Grass Energy|Grass Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var yanmega = e.ScenarioBench(0, C("DRI-2"));
            e.ScenarioEvolve(yanmega, C("DRI-3"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToDeck(0, C("MEE-9"), 3);
            e.ScenarioSetTurn(0, 3);
            e.SwitchActive(0, yanmega);
            Assert.AreEqual(3, yanmega.Energy.Count);
            Assert.IsTrue(yanmega.Energy.All(c => c.Def.Name == "Grass Energy"));
        }

        [Test]
        public void Potent_Glare_Stops_Pokemon_With_Abilities_From_The_Hand_Except_Team_Rocket_s()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var arbok = e.ScenarioActive(1, C("DRI-112"));
            e.ScenarioEvolve(arbok, C("DRI-113"));
            e.ScenarioAddToHand(0, C("DRI-45"));  // Misty's Psyduck (Ability)
            e.ScenarioAddToHand(0, C("DRI-125")); // Team Rocket's Koffing (Ability, Team Rocket's)
            e.ScenarioAddToHand(0, C("DRI-75"));  // Electrike (no Ability)
            e.ScenarioSetTurn(0, 3);
            var labels = e.GetMainActions(0).Select(o => o.Label).ToList();
            CollectionAssert.DoesNotContain(labels, "Bench Misty's Psyduck");
            CollectionAssert.Contains(labels, "Bench Team Rocket's Koffing");
            CollectionAssert.Contains(labels, "Bench Electrike");
        }

        [Test]
        public void Darkest_Impulse_Puts_4_Counters_On_A_Pokemon_Evolved_From_The_Hand_And_Does_Not_Stack()
        {
            var e = Scenario(new ScriptedAgent().Then("Evolve"), new ScriptedAgent());
            var electrike = e.ScenarioActive(0, C("DRI-75"));
            e.ScenarioAddToHand(0, C("DRI-76"));
            e.ScenarioActive(1, C(Dreepy));
            for (int i = 0; i < 2; i++)
            {
                var amp = e.ScenarioBench(1, C("DRI-72"));
                e.ScenarioEvolve(amp, C("DRI-73"));
                e.ScenarioEvolve(amp, C("DRI-74"));
            }
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Manectric", electrike.Name);
            Assert.AreEqual(4, electrike.DamageCounters);
        }

        [Test]
        public void Divers_Catch_Puts_Basic_Water_Energy_Into_The_Hand_On_A_Knock_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Yes"), new ScriptedAgent().Then("Attack: Electroslug"));
            var staryu = e.ScenarioActive(0, C("DRI-46"));
            e.ScenarioAttach(staryu, C("MEE-11"));
            e.ScenarioAttach(staryu, C("MEE-11"));
            var huntail = e.ScenarioBench(0, C("DRI-54"));
            e.ScenarioEvolve(huntail, C("DRI-55"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(staryu.Uid), "Misty's Staryu was Knocked Out");
            Assert.AreEqual(2, e.State.Players[0].Hand.Count(c => c.Def.Name == "Water Energy"));
        }

        [Test]
        public void Evil_Incineration_Discards_The_Defending_Pokemon_Without_A_Prize()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Evil Incineration"), new ScriptedAgent());
            var moltres = e.ScenarioActive(0, C("DRI-31"));
            e.ScenarioAttach(moltres, C("MEE-10"));
            e.ScenarioAttach(moltres, C("MEE-10"));
            e.ScenarioAttach(moltres, C("DRI-182"));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "not a Knock Out");
            Assert.IsFalse(moltres.Energy.Any(c => c.Def.Name == "Team Rocket's Energy"));
            Assert.IsNotNull(e.State.Players[1].Active, "the opponent promoted a new Active Pokémon");
        }

        [Test]
        public void Corrosive_Sludge_Discards_The_Defending_Pokemon_At_The_End_Of_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Corrosive Sludge"), new ScriptedAgent());
            var grimer = e.ScenarioActive(0, C("DRI-123"));
            e.ScenarioAttach(grimer, C("MEE-15"));
            e.ScenarioAttach(grimer, C("MEE-15"));
            var target = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e);
            Assert.IsNotNull(e.State.FindPokemon(target.Uid));
            Step(e);
            NextTurn(e);
            Assert.IsNull(e.State.FindPokemon(target.Uid));
            Assert.AreEqual(6, e.State.Players[0].Prizes.Count, "discarded, not Knocked Out");
        }

        [Test]
        public void Smog_Signals_Benches_Koffing_When_Team_Rocket_s_Koffing_Is_Damaged()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Electroslug"), new ScriptedAgent().Then("Team Rocket's Koffing|Team Rocket's Koffing"));
            var buzz = e.ScenarioActive(0, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioActive(1, C("DRI-125"));
            e.ScenarioAddToDeck(1, C("DRI-125"), 2);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, e.State.Players[1].Bench.Count(m => m.Name == "Team Rocket's Koffing"));
        }

        [Test]
        public void Tainted_Horn_Poison_Puts_8_Counters_In_Checkup()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tainted Horn"), new ScriptedAgent());
            var king = e.ScenarioActive(0, C("DRI-117"));
            e.ScenarioEvolve(king, C("DRI-118"));
            e.ScenarioEvolve(king, C("DRI-119"));
            e.ScenarioAttach(king, C("MEE-15"));
            e.ScenarioAttach(king, C("MEE-15"));
            e.ScenarioAttach(king, C(Psychic));
            var target = e.ScenarioActive(1, C("DRI-66"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, target.DamageCounters);
            e.EndTurn();
            Assert.AreEqual(18, target.DamageCounters);
        }

        [Test]
        public void Bother_Bot_Turns_A_Prize_Face_Up_And_Swaps_It_With_A_Card_From_The_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Team Rocket's Bother-Bot", "Face-down Prize card 1", "Yes"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("DRI-172"));
            var card = e.ScenarioAddToHand(1, C("DRI-75"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(6, opp.Prizes.Count);
            Assert.IsTrue(opp.Prizes.Contains(card), "the hand card went into the Prize cards");
            CollectionAssert.Contains(opp.FaceUpPrizeIds, card.Id, "and it stays face up");
            Assert.AreEqual(1, opp.Hand.Count(c => c.Def.Name == "Fire Energy"));
        }

        [Test]
        public void Stone_Palace_Reduces_Damage_To_Steven_s_Pokemon_Once()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var baltoy = e.ScenarioActive(0, C("DRI-83"));
            e.ScenarioBench(0, C("DRI-86"));
            e.ScenarioBench(0, C("DRI-86"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, baltoy.DamageCounters, "50 - 30, and 2 Stone Palace don't stack");
        }

        [Test]
        public void Granite_Cave_Reduces_Damage_To_Steven_s_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var baltoy = e.ScenarioActive(0, C("DRI-83"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetStadium(0, C("DRI-166"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, baltoy.DamageCounters);
        }

        [Test]
        public void Lose_Cool_Adds_120_With_2_Damage_Counters()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Impact Blow"), new ScriptedAgent());
            var ape = e.ScenarioActive(0, C("DRI-90"));
            e.ScenarioEvolve(ape, C("DRI-91"));
            e.ScenarioEvolve(ape, C("DRI-92"));
            e.ScenarioAttach(ape, C("MEE-14"));
            e.ScenarioAttach(ape, C("MEE-14"));
            ape.DamageCounters = 2;
            var target = e.ScenarioActive(1, C("DRI-64"));
            e.ScenarioEvolve(target, C("DRI-65"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(28, target.DamageCounters);
        }

        [Test]
        public void Flustered_Leap_Puts_Misty_s_Psyduck_On_Top_Of_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Flustered Leap"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var psyduck = e.ScenarioBench(0, C("DRI-45"));
            e.ScenarioAttach(psyduck, C("MEE-11"));
            e.ScenarioAddToDeck(0, C("MEE-9"), 1); // the bottom card
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(0, me.Bench.Count);
            Assert.AreEqual("Misty's Psyduck", me.Deck[0].Def.Name);
            Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Grass Energy"), "the bottom card was discarded");
            Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Water Energy"), "the attached Energy was discarded");
        }

        [Test]
        public void Melt_Away_Makes_The_Retreat_Cost_0_Without_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var magcargo = e.ScenarioActive(0, C("DRI-35"));
            e.ScenarioEvolve(magcargo, C("DRI-36"));
            e.ScenarioActive(1, C(Dreepy));
            Assert.AreEqual(0, e.RetreatCostOf(magcargo));
            e.ScenarioAttach(magcargo, C("MEE-10"));
            Assert.AreEqual(3, e.RetreatCostOf(magcargo));
        }

        [Test]
        public void Rocket_Brain_Can_Be_Used_More_Than_Once_A_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Rocket Brain", "Dreepy", "Use Rocket Brain", "Dreepy"), new ScriptedAgent());
            var orbeetle = e.ScenarioActive(0, C("DRI-15"));
            e.ScenarioEvolve(orbeetle, C("DRI-88"));
            e.ScenarioEvolve(orbeetle, C("DRI-89"));
            var drowzee = e.ScenarioBench(0, C("DRI-79"));
            drowzee.DamageCounters = 2;
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(0, drowzee.DamageCounters);
            Assert.AreEqual(2, dreepy.DamageCounters);
        }

        // ------------------------------------------------------------------ White Flare

        [Test]
        public void Stimulated_Evolution_Lets_Shelmet_Evolve_On_The_First_Turn_With_Karrablast_In_Play()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("WHT-8"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("WHT-9"));
            e.ScenarioSetTurn(0, 1);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve), "no Karrablast in play");
            e.ScenarioBench(0, C("BLK-9"));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve && o.Label.Contains("Accelgor")), "Karrablast in play");
        }

        [Test]
        public void Inferno_Fandango_Attaches_Fire_Energy_From_The_Hand_More_Than_Once()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Inferno Fandango", "Emboar", "Use Inferno Fandango", "Dreepy"), new ScriptedAgent());
            var emboar = e.ScenarioActive(0, C("WHT-11"));
            e.ScenarioEvolve(emboar, C("WHT-12"));
            e.ScenarioEvolve(emboar, C("WHT-13"));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(1, emboar.Energy.Count);
            Assert.AreEqual(1, dreepy.Energy.Count);
            var actions = e.GetMainActions(0);
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Use Inferno Fandango")), "no Fire Energy left in the hand");
            Assert.IsTrue(actions.Any(o => o.Action == ActionType.AttachEnergy), "the attachment for the turn is still there");
        }

        [Test]
        public void Torrential_Whirlpool_Switches_Both_Active_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Torrential Whirlpool"), new ScriptedAgent());
            var samurott = e.ScenarioActive(0, C("WHT-21"));
            e.ScenarioEvolve(samurott, C("WHT-22"));
            e.ScenarioEvolve(samurott, C("WHT-23"));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            var electabuzz = e.ScenarioBench(1, C("DRI-68"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(dreepy, e.State.Players[0].Active);
            Assert.AreSame(electabuzz, e.State.Players[1].Active);
        }

        [Test]
        public void Torrential_Whirlpool_Works_When_The_Opponent_Has_No_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Torrential Whirlpool"), new ScriptedAgent());
            var samurott = e.ScenarioActive(0, C("WHT-21"));
            e.ScenarioEvolve(samurott, C("WHT-22"));
            e.ScenarioEvolve(samurott, C("WHT-23"));
            var dreepy = e.ScenarioBench(0, C(Dreepy));
            var oppActive = e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(dreepy, e.State.Players[0].Active);
            Assert.AreSame(oppActive, e.State.Players[1].Active);
        }

        [Test]
        public void Gale_Thrust_Adds_90_Only_The_Turn_Keldeo_Ex_Moves_To_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gale Thrust", "Attack: Gale Thrust"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            var keldeo = e.ScenarioBench(0, C("WHT-30"));
            e.ScenarioAttach(keldeo, C("MEE-11"));
            e.ScenarioAttach(keldeo, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            e.ScenarioSetTurn(0, 3);
            e.SwitchActive(0, keldeo);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "30 + 90");
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(15, target.DamageCounters, "30 the next time");
        }

        [Test]
        public void Greedy_Eater_Takes_1_More_Prize_For_A_Basic()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Dark Bite"), new ScriptedAgent());
            var hydreigon = e.ScenarioActive(0, C("WHT-65"));
            e.ScenarioEvolve(hydreigon, C("WHT-66"));
            e.ScenarioEvolve(hydreigon, C("WHT-67"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(hydreigon, C("MEE-15"));
            e.ScenarioAttach(hydreigon, C(Psychic));
            e.ScenarioAttach(hydreigon, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Prizes.Count);
        }

        [Test]
        public void Ancient_Wing_Devolves_The_Opponents_Pokemon_Into_Their_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Ancient Wing"), new ScriptedAgent());
            var archeops = e.ScenarioActive(0, C("WHT-50"));
            e.ScenarioEvolve(archeops, C("WHT-51"));
            var pignite = e.ScenarioActive(1, C("WHT-11"));
            e.ScenarioEvolve(pignite, C("WHT-12"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Tepig", pignite.Name);
            Assert.IsTrue(e.State.Players[1].Hand.Any(c => c.Def.Name == "Pignite"));
        }

        [Test]
        public void Distorted_Future_Makes_The_Opponent_Shuffle_Their_Hand_And_Draw_3()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Distorted Future"), new ScriptedAgent());
            var gothitelle = e.ScenarioActive(0, C("WHT-41"));
            e.ScenarioEvolve(gothitelle, C("WHT-42"));
            e.ScenarioEvolve(gothitelle, C("WHT-43"));
            e.ScenarioActive(1, C(Dreepy));
            for (int i = 0; i < 6; i++) e.ScenarioAddToHand(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, e.State.Players[1].Hand.Count);
            Assert.AreEqual(23, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Look_For_Prey_Benches_A_Basic_With_70_HP_Or_Less_From_The_Opponents_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Look for Prey"), new ScriptedAgent());
            var mandibuzz = e.ScenarioActive(0, C("WHT-63"));
            e.ScenarioEvolve(mandibuzz, C("WHT-64"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(1, C("BLK-28"));
            e.ScenarioAddToHand(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.AreEqual(1, opp.Bench.Count);
            Assert.AreEqual("Dreepy", opp.Bench[0].Name);
            Assert.IsTrue(opp.Hand.Any(c => c.Def.Name == "Kyurem ex"), "230 HP is too much");
        }

        [Test]
        public void Plume_Protection_Stops_Damage_To_The_Fossil_On_The_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Electrobullet"), new ScriptedAgent());
            var zebstrika = e.ScenarioActive(0, C("WHT-31"));
            e.ScenarioEvolve(zebstrika, C("WHT-32"));
            e.ScenarioAttach(zebstrika, C("MEE-12"));
            e.ScenarioAttach(zebstrika, C("MEE-12"));
            e.ScenarioAttach(zebstrika, C(Psychic));
            var target = e.ScenarioActive(1, C("JTG-79"));
            var fossil = e.ScenarioBench(1, C("WHT-79"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(10, target.DamageCounters);
            Assert.AreEqual(0, fossil.DamageCounters);
        }

        // ------------------------------------------------------------------ Black Bolt

        [Test]
        public void Lively_Needles_Adds_100_After_A_Heal_This_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Healing Leaves", "Attack: Lively Needles"), new ScriptedAgent());
            var maractus = e.ScenarioActive(0, C("BLK-8"));
            e.ScenarioAttach(maractus, C("MEE-9"));
            maractus.DamageCounters = 2;
            var swadloon = e.ScenarioBench(0, C("WHT-1"));
            e.ScenarioEvolve(swadloon, C("WHT-2"));
            var target = e.ScenarioActive(1, C("BLK-28"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, maractus.DamageCounters, "Healing Leaves");
            Step(e);
            Assert.AreEqual(12, target.DamageCounters, "20 + 100");
        }

        [Test]
        public void Debut_Performance_Lets_Meloetta_Ex_Attack_On_The_First_Turn()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var meloetta = e.ScenarioActive(0, C("BLK-44"));
            e.ScenarioAttach(meloetta, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 1);
            CollectionAssert.Contains(e.GetMainActions(0).Select(o => o.Label).ToList(), "Attack: Echoed Voice");
        }

        [Test]
        public void Echoed_Voice_Does_80_More_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Echoed Voice", "Attack: Echoed Voice"), new ScriptedAgent());
            var meloetta = e.ScenarioActive(0, C("BLK-44"));
            e.ScenarioAttach(meloetta, C(Psychic));
            var target = e.ScenarioActive(1, C("BLK-28"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, target.DamageCounters);
            NextTurn(e);
            Step(e);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(3 + 11, target.DamageCounters, "30 + 80");
        }

        [Test]
        public void Sturdy_Leaves_Crustle_With_10_HP_From_Full_HP()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Heavy Impact"));
            var crustle = e.ScenarioActive(0, C("BLK-51"));
            e.ScenarioEvolve(crustle, C("BLK-52"));
            e.ScenarioBench(0, C(Dreepy));
            var gigalith = e.ScenarioActive(1, C("WHT-46"));
            e.ScenarioEvolve(gigalith, C("WHT-47"));
            e.ScenarioEvolve(gigalith, C("WHT-48"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(gigalith, C("MEE-14"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreSame(crustle, e.State.Players[0].Active, "not Knocked Out");
            Assert.AreEqual(10, e.RemainingHp(crustle));
        }

        [Test]
        public void Mighty_Shell_Stops_Attacks_From_Pokemon_With_Special_Energy()
        {
            foreach (bool special in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
                var carracosta = e.ScenarioActive(0, C("BLK-22"));
                e.ScenarioEvolve(carracosta, C("BLK-23"));
                var buzz = e.ScenarioActive(1, C("DRI-68"));
                e.ScenarioAttach(buzz, C("MEE-12"));
                e.ScenarioAttach(buzz, C(special ? "JTG-159" : "MEE-12"));
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(special ? 0 : 10, carracosta.DamageCounters, special ? "Special Energy: prevented" : "50 x 2 for Weakness");
            }
        }

        [Test]
        public void Gear_Coating_Reduces_Damage_To_Pokemon_With_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var dreepy = e.ScenarioActive(0, C(Dreepy));
            e.ScenarioAttach(dreepy, C("MEE-16"));
            var klinklang = e.ScenarioBench(0, C("BLK-61"));
            e.ScenarioEvolve(klinklang, C("BLK-62"));
            e.ScenarioEvolve(klinklang, C("BLK-63"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(3, dreepy.DamageCounters, "50 - 20");
        }

        [Test]
        public void Poison_Point_Poisons_The_Attacking_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var scolipede = e.ScenarioActive(0, C("BLK-54"));
            e.ScenarioEvolve(scolipede, C("BLK-55"));
            e.ScenarioEvolve(scolipede, C("BLK-56"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(5, scolipede.DamageCounters);
            Assert.IsTrue(buzz.Has(SpecialCondition.Poisoned));
        }

        // ------------------------------------------------------------------ Mega Evolution / Phantasmal Flames / promos

        [Test]
        public void Spiteful_Swirl_Puts_A_Damage_Counter_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var nickit = e.ScenarioActive(0, C("MEG-89"));
            e.ScenarioBench(0, C("MEG-87"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(5, nickit.DamageCounters);
            Assert.AreEqual(1, buzz.DamageCounters, "Spiteful Swirl from the Bench");
        }

        [Test]
        public void Fragile_Husk_Gives_No_Prize_Cards_To_A_Pokemon_Ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Claw of Darkness"));
            var shedinja = e.ScenarioActive(0, C("MEG-16"));
            e.ScenarioEvolve(shedinja, C("MEG-61"));
            e.ScenarioBench(0, C(Dreepy));
            var absol = e.ScenarioActive(1, C("MEG-86"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(absol, C("MEE-15"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreNotSame(shedinja, e.State.Players[0].Active, "Knocked Out");
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "no Prize cards for Shedinja");
        }

        [Test]
        public void Shadowy_Concealment_Gives_1_Fewer_Prize_For_A_Darkness_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Claw of Darkness"));
            e.ScenarioActive(0, C("MEG-89"));
            e.ScenarioBench(0, C("PFL-56"));
            var absol = e.ScenarioActive(1, C("MEG-86"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(absol, C("MEE-15"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(6, e.State.Players[1].Prizes.Count, "1 Prize card - 1");
        }

        [Test]
        public void Excited_Turbo_Attaches_Fire_Energy_To_The_Bench_More_Than_Once()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Excited Turbo", "Oricorio ex", "Use Excited Turbo", "Charmander"), new ScriptedAgent());
            e.ScenarioActive(0, C("PFL-13"));
            var oricorio = e.ScenarioBench(0, C("PFL-18"));
            var charmander = e.ScenarioBench(0, C("PFL-11"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(1, oricorio.Energy.Count);
            Assert.AreEqual(1, charmander.Energy.Count);
        }

        [Test]
        public void Excited_Turbo_Needs_A_Fire_Mega_Evolution_Pokemon_Ex()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioBench(0, C("PFL-18"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("Excited Turbo")));
        }

        [Test]
        public void Juggernaut_Horn_Adds_The_Attack_Damage_Taken_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Juggernaut Horn"), new ScriptedAgent().Then("Attack: Slash"));
            var heracross = e.ScenarioActive(0, C("PFL-4"));
            e.ScenarioAttach(heracross, C("MEE-9"));
            e.ScenarioAttach(heracross, C("MEE-9"));
            var kyurem = e.ScenarioActive(1, C("BLK-28"));
            e.ScenarioAttach(kyurem, C(Psychic));
            e.ScenarioAttach(kyurem, C(Psychic));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(5, heracross.DamageCounters);
            NextTurn(e);
            Step(e);
            Assert.AreEqual(15, kyurem.DamageCounters, "100 + 50");
        }

        [Test]
        public void Diamond_Coat_Takes_30_Less_Damage()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
            var diancie = e.ScenarioActive(0, C("PFL-41"));
            var buzz = e.ScenarioActive(1, C("DRI-68"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioAttach(buzz, C("MEE-12"));
            e.ScenarioSetTurn(1, 4);
            Step(e);
            Assert.AreEqual(2, diancie.DamageCounters, "50 - 30");
        }

        [Test]
        public void Infernal_Slash_Needs_4_Fire_Energy_From_The_Hand()
        {
            foreach (int fire in new[] { 3, 4 })
            {
                var e = Scenario(new ScriptedAgent().Then("Attack: Infernal Slash"), new ScriptedAgent());
                var ceruledge = e.ScenarioActive(0, C("PFL-20"));
                e.ScenarioAttach(ceruledge, C("MEE-10"));
                var target = e.ScenarioActive(1, C("BLK-28"));
                for (int i = 0; i < fire; i++) e.ScenarioAddToHand(0, C("MEE-10"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(fire == 4 ? 22 : 0, target.DamageCounters);
                Assert.AreEqual(fire == 4 ? 0 : 3, e.State.Players[0].Hand.Count);
            }
        }

        [Test]
        public void Ancient_Bulwark_Stops_Attackers_With_2_Or_Fewer_Energy()
        {
            foreach (int energy in new[] { 2, 3 })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent().Then("Attack: Electroslug"));
                var dreepy = e.ScenarioActive(0, C(Dreepy));
                e.ScenarioBench(0, C("MEP-85"));
                var buzz = e.ScenarioActive(1, C("DRI-68"));
                for (int i = 0; i < energy; i++) e.ScenarioAttach(buzz, C("MEE-12"));
                e.ScenarioSetTurn(1, 4);
                Step(e);
                Assert.AreEqual(energy == 2 ? 0 : 5, dreepy.DamageCounters, energy + " Energy");
            }
        }

        [Test]
        public void Luminous_Wing_Stops_Mortal_Shuriken()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mortal Shuriken", "Mega Clefable ex"), new ScriptedAgent());
            e.ScenarioActive(0, C("MEP-81"));
            e.ScenarioAddToHand(0, C("MEE-11"));
            var clefable = e.ScenarioActive(1, C("MEP-72"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, clefable.DamageCounters, "Luminous Wing");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count, "the Water Energy was still discarded");
        }

        [Test]
        public void Mortal_Shuriken_Puts_6_Damage_Counters_On_Any_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Mortal Shuriken", "Nickit"), new ScriptedAgent());
            e.ScenarioActive(0, C("MEP-81"));
            e.ScenarioAddToHand(0, C("MEE-11"));
            e.ScenarioActive(1, C("MEP-72"));
            var nickit = e.ScenarioBench(1, C("MEG-89"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, nickit.DamageCounters);
        }

        [Test]
        public void Tyrannically_Gutsy_Adds_150_HP_With_Special_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tyrantrum = e.ScenarioActive(0, C("MEP-66"));
            e.ScenarioActive(1, C(Dreepy));
            Assert.AreEqual(180, e.RemainingHp(tyrantrum));
            e.ScenarioAttach(tyrantrum, C("JTG-159"));
            Assert.AreEqual(330, e.RemainingHp(tyrantrum));
        }

        [Test]
        public void Lt_Surges_Bargain_Each_Player_Takes_A_Prize_When_The_Opponent_Agrees()
        {
            var e = Scenario(new ScriptedAgent().Then("Lt. Surge's Bargain"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C("MEG-120"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count);
            Assert.AreEqual(5, e.State.Players[1].Prizes.Count);
        }

        [Test]
        public void Dizzying_Valley_Keeps_A_Pokemon_Confused_When_It_Evolves()
        {
            foreach (bool valley in new[] { true, false })
            {
                var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
                var charmander = e.ScenarioActive(0, C("PFL-11"));
                e.ScenarioActive(1, C(Dreepy));
                if (valley) e.ScenarioSetStadium(0, C("PFL-88"));
                e.ScenarioSetTurn(0, 3);
                e.ApplyCondition(charmander, SpecialCondition.Confused);
                var card = e.ScenarioAddToHand(0, C("PFL-12"));
                e.State.Players[0].Hand.Remove(card);
                e.EvolvePokemon(charmander, card);
                Assert.AreEqual("Charmeleon", charmander.Name);
                Assert.AreEqual(valley, charmander.Has(SpecialCondition.Confused), valley ? "Dizzying Valley" : "evolving cures it");
            }
        }
    }
}
