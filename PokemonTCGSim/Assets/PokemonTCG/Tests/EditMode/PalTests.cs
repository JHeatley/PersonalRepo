using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.Cards;
using PokemonTCG.Core;

namespace PokemonTCG.Tests
{
    /// <summary>v40: Paldea Evolved (PAL), with your Superior Energy Retrieval and exact-name rulings.</summary>
    public class PalTests : ScenarioTestBase
    {
        const string Grass = "MEE-9";
        const string Fire = "MEE-10";
        const string Lightning = "MEE-12";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Metal = "MEE-16";

        const string Tropius = "PAL-7";
        const string Spidops = "PAL-18";
        const string Nymble = "PAL-19";
        const string Lokix = "PAL-21";
        const string Pincurchin = "PAL-73";
        const string Shinx = "PAL-68";
        const string Hariyama = "PAL-113";
        const string Glimmora = "PAL-126";
        const string Copperajah = "PAL-150";
        const string Orthworm = "PAL-151";
        const string Slaking = "PAL-162";
        const string Maushold = "PAL-168";
        const string BraveryCharm = "PAL-173";
        const string SnowyMountain = "PAL-174";
        const string Wasteland = "PAL-175";
        const string ChoiceBelt = "PAL-176";
        const string Dendra = "PAL-179";
        const string Falkner = "PAL-180";
        const string FightingAuLait = "PAL-181";
        const string Giacomo = "PAL-182";
        const string Grusha = "PAL-184";
        const string PracticeStudio = "PAL-186";
        const string SuperRod = "PAL-188";
        const string SuperiorEnergyRetrieval = "PAL-189";
        const string JetEnergy = "PAL-190";
        const string LuminousEnergy = "PAL-191";
        const string TherapeuticEnergy = "PAL-193";

        const string Tauros = "MEW-128";
        const string Pidgey = "OBF-162";
        const string Pidgeotto = "OBF-163";
        const string MausholdEx = "PAR-155";
        const string TinkatonPAR = "PAR-85";
        const string ZacianV = "30C-CC26";
        const string TingLu = "TWM-110";
        const string Cleffa = "SVP-37";
        const string JammingTower = "TWM-153";
        const string LuxuriousCape = "PAR-166";

        private static bool Offered(GameEngine e, int player, string text)
        {
            return e.GetMainActions(player).Any(o => o.Label.Contains(text));
        }

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
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
        public void Every_Paldea_Evolved_Card_Is_In_The_Pool()
        {
            for (int n = 1; n <= 279; n++)
                Assert.IsNotNull(db.Get("PAL-" + n), "PAL " + n + " is missing");
        }

        [Test]
        public void Reprints_Are_Alternate_Prints_Of_The_Pool_Card()
        {
            Assert.AreEqual("SVP-76", db.MainPrinting(C("PAL-12")).Key, "Sprigatito");
            Assert.AreEqual("SVP-33", db.MainPrinting(C("PAL-15")).Key, "Meowscarada ex");
            Assert.AreEqual("SVP-31", db.MainPrinting(C("PAL-262")).Key, "Tinkaton ex's gold print");
            Assert.AreEqual("30C-CC30", db.MainPrinting(C("PAL-42")).Key, "Magikarp");
            Assert.IsTrue(C("PAL-86").IsTera && C("PAL-93").IsTera, "Slowking ex and Dedenne ex are Tera");
        }

        // ================================================================== Your rulings

        [Test]
        public void Superior_Energy_Retrieval_Takes_Up_To_4_And_Needs_Only_2_Other_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Superior Energy Retrieval", "Pidgey|Pidgey", "Fire Energy|Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(SuperiorEnergyRetrieval));
            e.ScenarioAddToHand(0, C(Pidgey));
            e.ScenarioAddToDiscard(0, C(Fire), 3);
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Superior Energy Retrieval"), "needs 2 other cards to discard");
            e.ScenarioAddToHand(0, C(Pidgey));
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(2, p.Hand.Count(c => c.Def.Name == "Fire Energy"), "your ruling: up to 4, and you needn't take all");
            Assert.AreEqual(1, p.Discard.Count(c => c.Def.Name == "Fire Energy"));
            Assert.AreEqual(2, p.Discard.Count(c => c.Def.Name == "Pidgey"));
        }

        [Test]
        public void Gnaw_Relentlessly_Counts_Only_Cards_Named_Maushold()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Gnaw Relentlessly"), new ScriptedAgent());
            var maushold = e.ScenarioActive(0, C(Maushold));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(maushold, C(Psychic));
            e.ScenarioBench(0, C(Maushold));
            e.ScenarioBench(0, C(MausholdEx));
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(2, active.DamageCounters, "your ruling: Maushold ex doesn't count");
            Assert.AreEqual(2, benched.DamageCounters);
        }

        // ================================================================== Abilities

        [Test]
        public void Stir_And_Snooze_Makes_Slaking_Flip_2_Coins_To_Wake_Up()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var slaking = e.ScenarioActive(0, C(Slaking));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(slaking, SpecialCondition.Asleep);
            var log = new FlipLog();
            e.AddObserver(log);
            e.EndTurn();
            Assert.AreEqual(2, log.Flips.Count(t => t.Contains("is Asleep (")));
        }

        [Test]
        public void Shattering_Crystal_Flips_For_No_Prize_Cards()
        {
            var seen = new HashSet<int>();
            for (int seed = 0; seed < 30; seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent(), new ScriptedAgent());
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                var glimmora = e.ScenarioActive(0, C(Glimmora));
                e.ScenarioActive(1, C(Tauros));
                e.ScenarioSetTurn(1, 4);
                seen.Add(e.PrizesFor(glimmora));
            }
            CollectionAssert.AreEquivalent(new[] { 0, 1 }, seen.ToArray(), "heads: no Prize cards; tails: the usual 1");
        }

        [Test]
        public void Arm_Thrust_Practice_Stacks_And_Covers_Only_Your_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var mine = e.ScenarioActive(0, C(Tauros));
            e.ScenarioBench(0, C(Hariyama));
            e.ScenarioBench(0, C(Hariyama));
            var theirs = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(1, 4);
            e.DealAttackDamage(theirs, mine, 50);
            Assert.AreEqual(3, mine.DamageCounters, "2 Hariyama: 20 less");
            e.DealAttackDamage(mine, theirs, 50);
            Assert.AreEqual(5, theirs.DamageCounters);
        }

        [Test]
        public void Nutritional_Iron_Gives_100_HP_With_3_Metal_Energy()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var orthworm = e.ScenarioActive(0, C(Orthworm));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAttach(orthworm, C(Metal));
            e.ScenarioAttach(orthworm, C(Metal));
            e.ScenarioAttach(orthworm, C(Psychic));
            Assert.AreEqual(130, e.MaxHp(orthworm));
            e.ScenarioAttach(orthworm, C(Metal));
            Assert.AreEqual(230, e.MaxHp(orthworm));
        }

        [Test]
        public void Big_Roar_Works_Only_From_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Big Roar"), new ScriptedAgent());
            e.ScenarioActive(0, C(Shinx));
            var benchShinx = e.ScenarioBench(0, C(Shinx));
            var theirActive = e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Pidgey));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(1, e.GetMainActions(0).Count(o => o.Label.Contains("Use Big Roar")), "not the Benched Shinx");
            Step(e);
            Assert.AreEqual("Pidgey", e.State.Players[1].Active.Name);
            Assert.IsTrue(e.State.Players[1].Bench.Contains(theirActive));
            Assert.IsNotNull(benchShinx);
        }

        // ================================================================== Attacks

        [Test]
        public void Tropic_Breeze_Keeps_The_Energy_With_No_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tropic Breeze"), new ScriptedAgent());
            var tropius = e.ScenarioActive(0, C(Tropius));
            e.ScenarioAttach(tropius, C(Grass));
            e.ScenarioAttach(tropius, C(Grass));
            e.ScenarioAttach(tropius, C(Psychic));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, tropius.Energy.Count, "Compendium (2023-06-08)");

            var bench = Scenario(new ScriptedAgent().Then("Attack: Tropic Breeze"), new ScriptedAgent());
            var t2 = bench.ScenarioActive(0, C(Tropius));
            bench.ScenarioAttach(t2, C(Grass));
            bench.ScenarioAttach(t2, C(Grass));
            bench.ScenarioAttach(t2, C(Psychic));
            var pidgey = bench.ScenarioBench(0, C(Pidgey));
            bench.ScenarioActive(1, C(Tauros));
            bench.ScenarioSetTurn(0, 3);
            Step(bench);
            Assert.AreEqual(0, t2.Energy.Count);
            Assert.AreEqual(3, pidgey.Energy.Count);
        }

        [Test]
        public void Entangling_Trap_Shuffles_Both_Active_Pokemon_Into_The_Decks()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Entangling Trap"), new ScriptedAgent());
            var spidops = e.ScenarioActive(0, C(Spidops));
            e.ScenarioAttach(spidops, C(Grass));
            e.ScenarioAttach(spidops, C(Grass));
            e.ScenarioBench(0, C(Pidgey));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioBench(1, C(Pidgeotto));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual("Pidgey", e.State.Players[0].Active.Name);
            Assert.AreEqual("Pidgeotto", e.State.Players[1].Active.Name);
            Assert.AreEqual(23, e.State.Players[0].Deck.Count, "Spidops and its 2 Energy");
            Assert.AreEqual(21, e.State.Players[1].Deck.Count);
        }

        [Test]
        public void Assaulting_Kick_Does_100_More_Right_After_Evolving_From_Nymble()
        {
            var e = Scenario(new ScriptedAgent().Then("into Lokix", "Attack: Assaulting Kick"), new ScriptedAgent());
            var nymble = e.ScenarioActive(0, C(Nymble));
            e.ScenarioAttach(nymble, C(Grass));
            var dragonite = e.ScenarioActive(1, C("OBF-159"));
            e.ScenarioAddToHand(0, C(Lokix));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            Assert.AreEqual(13, dragonite.DamageCounters, "30 + 100");
        }

        [Test]
        public void Follow_Up_Kerzap_Needs_Stun_Needle_Last_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Stun Needle"), new ScriptedAgent());
            var pincurchin = e.ScenarioActive(0, C(Pincurchin));
            e.ScenarioAttach(pincurchin, C(Lightning));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Attack: Follow-Up Kerzap"));
            Step(e);
            NextTurn(e);
            NextTurn(e);
            Assert.IsTrue(Offered(e, 0, "Attack: Follow-Up Kerzap"));
        }

        [Test]
        public void Nosequake_Also_Hits_Your_Own_Bench()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Nosequake"), new ScriptedAgent());
            var copperajah = e.ScenarioActive(0, C(Copperajah));
            e.ScenarioAttach(copperajah, C(Metal));
            e.ScenarioAttach(copperajah, C(Metal));
            e.ScenarioAttach(copperajah, C(Psychic));
            var mine = e.ScenarioBench(0, C(Tauros));
            e.ScenarioActive(1, C(ZacianV));
            e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, mine.DamageCounters);
        }

        // ================================================================== Tools and Stadiums

        [Test]
        public void Bravery_Charm_Works_Again_Before_Knock_Outs_When_Ground_Crasher_Discards_Jamming_Tower()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Ground Crasher"), new ScriptedAgent());
            var tingLu = e.ScenarioActive(0, C(TingLu));
            e.ScenarioAttach(tingLu, C(Fighting));
            e.ScenarioActive(1, C(Tauros));
            var cleffa = e.ScenarioBench(1, C(Cleffa));
            e.ScenarioAttachTool(cleffa, C(BraveryCharm));
            e.ScenarioSetStadium(1, C(JammingTower));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(30, e.MaxHp(cleffa), "Jamming Tower turns Bravery Charm off");
            Step(e);
            Assert.IsNull(e.State.Stadium);
            Assert.IsTrue(e.State.Players[1].Bench.Contains(cleffa), "Compendium (2024-09-05): Cleffa survives with 80 HP");
            Assert.AreEqual(3, cleffa.DamageCounters);
        }

        [Test]
        public void Choice_Belt_Adds_30_Against_Pokemon_V_Only()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioAttachTool(tauros, C(ChoiceBelt));
            var zacian = e.ScenarioActive(1, C(ZacianV));
            var benchTauros = e.ScenarioBench(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(tauros, zacian, 30);
            Assert.AreEqual(6, zacian.DamageCounters);
            e.SwitchActive(1, benchTauros);
            e.DealAttackDamage(tauros, benchTauros, 30);
            Assert.AreEqual(3, benchTauros.DamageCounters);
        }

        [Test]
        public void Practice_Studio_Adds_10_For_Stage_1_Attackers()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var pidgeotto = e.ScenarioActive(0, C(Pidgeotto));
            var pidgey = e.ScenarioBench(0, C(Pidgey));
            var tauros = e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(PracticeStudio));
            e.ScenarioSetTurn(0, 3);
            e.DealAttackDamage(pidgeotto, tauros, 40);
            Assert.AreEqual(5, tauros.DamageCounters);
            e.DealAttackDamage(pidgey, tauros, 20);
            Assert.AreEqual(7, tauros.DamageCounters, "a Basic gets nothing");
        }

        [Test]
        public void Calamitous_Wasteland_Raises_Basic_Non_Fighting_Retreat_Costs()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            var tingLu = e.ScenarioBench(0, C(TingLu));
            var pidgeotto = e.ScenarioBench(0, C(Pidgeotto));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetStadium(1, C(Wasteland));
            e.ScenarioSetTurn(0, 3);
            Assert.AreEqual(3, e.RetreatCostOf(tauros));
            Assert.AreEqual(C(TingLu).RetreatCost, e.RetreatCostOf(tingLu), "Fighting");
            Assert.AreEqual(1, e.RetreatCostOf(pidgeotto), "Stage 1");
        }

        [Test]
        public void Calamitous_Snowy_Mountain_Hurts_Basic_Non_Water_Pokemon_Given_Energy_From_Hand()
        {
            var e = Scenario(new ScriptedAgent().Then("Psychic Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetStadium(1, C(SnowyMountain));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, tauros.Energy.Count);
            Assert.AreEqual(2, tauros.DamageCounters);
        }

        // ================================================================== Special Energy

        [Test]
        public void Jet_Energy_Switches_The_Benched_Pokemon_It_Is_Attached_To_Into_The_Active_Spot()
        {
            var e = Scenario(new ScriptedAgent().Then("Attach Jet Energy to Pidgey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var pidgey = e.ScenarioBench(0, C(Pidgey));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(JetEnergy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(pidgey, e.State.Players[0].Active);
            Assert.AreEqual(1, pidgey.Energy.Count);
        }

        [Test]
        public void Luminous_Energy_Is_Every_Type_Unless_Another_Special_Energy_Is_Attached()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tinkaton = e.ScenarioActive(0, C(TinkatonPAR));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAttach(tinkaton, C(LuminousEnergy));
            Assert.IsTrue(e.HasEnergyOfType(tinkaton, EnergyType.Metal), "Compendium (Alloyed Hammer, 2023-11-09)");
            Assert.IsTrue(e.HasEnergyOfType(tinkaton, EnergyType.Psychic));
            Assert.AreEqual(1, e.EnergyCount(tinkaton));
            e.ScenarioAttach(tinkaton, C(TherapeuticEnergy));
            Assert.IsFalse(e.HasEnergyOfType(tinkaton, EnergyType.Metal), "another Special Energy: Colorless only");
            Assert.IsFalse(C(LuminousEnergy).IsBasicEnergy, "Compendium (Alcremie, 2024-09-12)");
        }

        [Test]
        public void Therapeutic_Energy_Cures_And_Blocks_Sleep_Confusion_And_Paralysis()
        {
            var e = Scenario(new ScriptedAgent().Then("Therapeutic Energy"), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(TherapeuticEnergy));
            e.ScenarioSetTurn(0, 3);
            e.ApplyCondition(tauros, SpecialCondition.Confused);
            e.ApplyCondition(tauros, SpecialCondition.Poisoned);
            Step(e);
            Assert.IsFalse(tauros.Has(SpecialCondition.Confused));
            Assert.IsTrue(tauros.Has(SpecialCondition.Poisoned), "Poison isn't covered");
            Assert.IsFalse(e.ApplyCondition(tauros, SpecialCondition.Asleep));
        }

        // ================================================================== Supporters and Items

        [Test]
        public void Dendra_Needs_Another_Card_And_Draws_Up_To_5()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Dendra", "Pidgey"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Dendra));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Dendra"), "no other cards in your hand");
            e.ScenarioAddToHand(0, C(Pidgey));
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Hand.Count);
            Assert.AreEqual("Pidgey", e.State.Players[0].Deck.Last().Def.Name);
        }

        [Test]
        public void Falkner_Draws_2_More_With_Your_Stadium_In_Play()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Falkner"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Falkner));
            e.ScenarioSetStadium(0, C(PracticeStudio));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);

            var theirs = Scenario(new ScriptedAgent().Then("Play Falkner"), new ScriptedAgent());
            theirs.ScenarioActive(0, C(Tauros));
            theirs.ScenarioActive(1, C(Tauros));
            theirs.ScenarioAddToHand(0, C(Falkner));
            theirs.ScenarioSetStadium(1, C(PracticeStudio));
            theirs.ScenarioSetTurn(0, 3);
            Step(theirs);
            Assert.AreEqual(2, theirs.State.Players[0].Hand.Count, "the opponent's Stadium isn't yours");
        }

        [Test]
        public void Grusha_Draws_To_7_When_None_Of_Your_Pokemon_Have_Energy()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Grusha"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(Grusha));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(7, e.State.Players[0].Hand.Count);
        }

        [Test]
        public void Giacomo_Discards_A_Special_Energy_From_Each_Opposing_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Giacomo"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            var active = e.ScenarioActive(1, C(Tauros));
            var benched = e.ScenarioBench(1, C(Pidgey));
            e.ScenarioAddToHand(0, C(Giacomo));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Giacomo"));
            e.ScenarioAttach(active, C(JetEnergy));
            e.ScenarioAttach(active, C(Psychic));
            e.ScenarioAttach(benched, C(TherapeuticEnergy));
            Step(e);
            Assert.AreEqual(1, active.Energy.Count);
            Assert.AreEqual("Psychic Energy", active.Energy[0].Def.Name);
            Assert.AreEqual(0, benched.Energy.Count);
        }

        [Test]
        public void Fighting_Au_Lait_Needs_You_Behind_On_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var tauros = e.ScenarioActive(0, C(Tauros));
            tauros.DamageCounters = 7;
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(FightingAuLait));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(Offered(e, 0, "Play Fighting Au Lait"), "same number of Prize cards");
            e.State.Players[1].Prizes.RemoveAt(0);
            Assert.IsTrue(Offered(e, 0, "Play Fighting Au Lait"));
        }

        [Test]
        public void Super_Rod_Shuffles_Pokemon_And_Basic_Energy_Back()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Super Rod", "Pidgey|Fire Energy|Pidgeotto"), new ScriptedAgent());
            e.ScenarioActive(0, C(Tauros));
            e.ScenarioActive(1, C(Tauros));
            e.ScenarioAddToHand(0, C(SuperRod));
            e.ScenarioAddToDiscard(0, C(Pidgey), 1);
            e.ScenarioAddToDiscard(0, C(Pidgeotto), 1);
            e.ScenarioAddToDiscard(0, C(Fire), 2);
            e.ScenarioAddToDiscard(0, C(Grusha), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var p = e.State.Players[0];
            Assert.AreEqual(23, p.Deck.Count);
            Assert.AreEqual(3, p.Discard.Count, "1 Fire Energy, Grusha and Super Rod itself");
        }
    }
}
