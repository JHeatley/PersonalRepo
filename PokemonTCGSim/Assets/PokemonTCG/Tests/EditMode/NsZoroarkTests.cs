using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>Cards and mechanics added with the N's Zoroark ex list, plus deck lists and cross-deck games.</summary>
    public class NsZoroarkTests : ScenarioTestBase
    {
        // Card keys used below
        const string Zorua = "JTG-97", Zoroark = "JTG-98", Zekrom = "ASC-155", Reshiram = "JTG-116";
        const string Pecharunt = "SFA-39", Yveltal = "MEG-88", BlackBelt = "JTG-143", Cyrano = "SSP-170", Judge = "POR-76";
        const string PPUp = "JTG-153", PunkHelmet = "PFL-92", Castle = "JTG-152", Watchtower = "DRI-180";
        const string Fire = "MEE-10", Psychic = "MEE-13", Dark = "MEE-15";

        private static void NextTurn(GameEngine e)
        {
            e.EndTurn();
            e.StartTurn();
        }

        // ------------------------------------------------------------------ Deck lists

        [Test]
        public void Zoroark_Decklist_Loads_As_Legal_60()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            Assert.AreEqual(60, deck.Count);
            CollectionAssert.IsEmpty(Decklist.Validate(deck, Formats.Standard2026));
        }

        [Test]
        public void Deck_List_Text_Round_Trips()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            var text = Decklist.Format("My Zoroark", Decklist.Group(deck));
            Assert.AreEqual("My Zoroark", Decklist.ReadName(text));
            var errors = new List<string>();
            var again = Decklist.Expand(Decklist.Parse(text, db, errors));
            CollectionAssert.IsEmpty(errors);
            CollectionAssert.AreEquivalent(deck.Select(d => d.Key).ToList(), again.Select(d => d.Key).ToList());
        }

        // ------------------------------------------------------------------ N's Zoroark ex

        [Test]
        public void Night_Joker_Uses_A_Benched_N_Pokemon_Attack()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night Joker", "N's Zekrom: Shred"), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(Zoroark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioBench(0, C(Zekrom));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "Shred (70) should KO a 70 HP Dreepy");
        }

        [Test]
        public void Night_Joker_Needs_A_Benched_N_Pokemon()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(Zoroark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioBench(0, C("TWM-95")); // Munkidori isn't an N's Pokémon
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Attack: Night Joker"));
        }

        [Test]
        public void Rampaging_Thunder_Copy_Stops_Zoroark_Attacking_Next_Turn_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Night Joker", "N's Zekrom: Rampaging Thunder"), new ScriptedAgent());
            var zoroark = e.ScenarioActive(0, C(Zoroark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioAttach(zoroark, C(Dark));
            e.ScenarioBench(0, C(Zekrom));
            var target = e.ScenarioActive(1, C("TWM-130"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(25, target.DamageCounters);

            NextTurn(e); // opponent's turn 4
            NextTurn(e); // our turn 5
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Attack), "Zoroark should be locked this turn");
            NextTurn(e); // opponent's turn 6
            NextTurn(e); // our turn 7
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Attack: Night Joker"), "the lock should have worn off");
        }

        [Test]
        public void Trade_Discards_One_Card_And_Draws_Two()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Trade", "Fire Energy"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioBench(0, C(Zoroark));
            e.ScenarioAddToHand(0, C(Fire));
            e.ScenarioAddToHand(0, C(Dark));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(3, me.Hand.Count);
            Assert.IsTrue(me.Discard.Any(c => c.Def.Name == "Fire Energy"));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Trade")), "once per turn");
        }

        // ------------------------------------------------------------------ Other Pokémon

        [Test]
        public void Subjugating_Chains_Switches_In_A_Darkness_Pokemon_And_Poisons_It()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Subjugating Chains"), new ScriptedAgent());
            var dreepy = e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioBench(0, C(Pecharunt));
            var yveltal = e.ScenarioBench(0, C(Yveltal));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreSame(yveltal, me.Active);
            Assert.IsTrue(yveltal.Has(SpecialCondition.Poisoned));
            Assert.IsTrue(me.Bench.Contains(dreepy));
        }

        [Test]
        public void Irritated_Outburst_Scales_With_Prizes_The_Opponent_Took()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Irritated Outburst"), new ScriptedAgent());
            var pech = e.ScenarioActive(0, C(Pecharunt));
            e.ScenarioAttach(pech, C(Dark));
            e.ScenarioAttach(pech, C(Dark));
            e.State.Players[1].Prizes.RemoveRange(0, 2); // opponent has taken 2
            var target = e.ScenarioActive(1, C(Reshiram));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(12, target.DamageCounters);
        }

        [Test]
        public void Clutch_Stops_The_Defending_Pokemon_Retreating_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Clutch"), new ScriptedAgent());
            var yv = e.ScenarioActive(0, C(Yveltal));
            e.ScenarioAttach(yv, C(Dark));
            var defender = e.ScenarioActive(1, C(Pecharunt));
            e.ScenarioAttach(defender, C(Dark));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            NextTurn(e); // opponent's turn
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Action == ActionType.Retreat), "Clutch should stop retreating");
            NextTurn(e);
            NextTurn(e); // opponent's following turn
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Action == ActionType.Retreat), "Clutch should have worn off");
        }

        [Test]
        public void Powerful_Rage_Counts_Damage_On_The_Attacker()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Powerful Rage"), new ScriptedAgent());
            var resh = e.ScenarioActive(0, C(Reshiram));
            e.ScenarioAttach(resh, C(Fire));
            e.ScenarioAttach(resh, Lightning());
            resh.DamageCounters = 3;
            var target = e.ScenarioActive(1, C(Pecharunt));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(6, target.DamageCounters, "3 counters x 20 = 60");
        }

        /// <summary>No Lightning Energy is in the pool yet, so tests make one.</summary>
        private static CardDef Lightning()
        {
            return new CardDef { Key = "TEST-L", Name = "Lightning Energy", Kind = CardKind.Energy, IsBasicEnergy = true, EnergyProvides = EnergyType.Lightning };
        }

        // ------------------------------------------------------------------ Trainers

        [Test]
        public void Black_Belts_Training_Adds_40_Only_Against_An_Active_Pokemon_Ex()
        {
            foreach (var targetKey in new[] { "POR-62", Reshiram })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Black Belt's Training", "Attack: Dark Feather"), new ScriptedAgent());
                var yv = e.ScenarioActive(0, C(Yveltal));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(yv, C(Dark));
                e.ScenarioAddToHand(0, C(BlackBelt));
                var target = e.ScenarioActive(1, C(targetKey));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Step(e);
                Assert.AreEqual(targetKey == "POR-62" ? 15 : 11, target.DamageCounters, "target " + targetKey);
            }
        }

        [Test]
        public void Judge_Leaves_Both_Players_With_4_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Judge"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C(Judge));
            e.ScenarioAddToHand(0, C(Fire));
            for (int i = 0; i < 7; i++) e.ScenarioAddToHand(1, C(Fire));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, e.State.Players[0].Hand.Count);
            Assert.AreEqual(4, e.State.Players[1].Hand.Count);
        }

        [Test]
        public void Cyrano_Finds_Up_To_3_Pokemon_Ex()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Cyrano", "Meowth ex|Fezandipiti ex|Pecharunt ex"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C(Cyrano));
            e.ScenarioAddToDeck(0, C("POR-62"), 1);
            e.ScenarioAddToDeck(0, C("ASC-142"), 1);
            e.ScenarioAddToDeck(0, C(Pecharunt), 1);
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var hand = e.State.Players[0].Hand;
            Assert.AreEqual(3, hand.Count);
            Assert.IsTrue(hand.All(c => c.Def.IsEx));
        }

        [Test]
        public void Ns_PP_Up_Attaches_Energy_From_Discard_To_A_Benched_Ns_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play N's PP Up"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            var zekrom = e.ScenarioBench(0, C(Zekrom));
            e.State.Players[0].Discard.Add(e.NewCard(0, C(Dark)));
            e.ScenarioAddToHand(0, C(PPUp));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(1, zekrom.Energy.Count);
            Assert.AreEqual(0, e.State.Players[0].Discard.Count(c => c.Def.IsEnergy));
        }

        [Test]
        public void Tools_Attach_From_Hand_One_Per_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Punk Helmet"), new ScriptedAgent());
            var zorua = e.ScenarioActive(0, C(Zorua));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C(PunkHelmet));
            e.ScenarioAddToHand(0, C(PunkHelmet));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.IsNotNull(zorua.Tool);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Punk Helmet"), "no Pokémon left without a Tool");
        }

        [Test]
        public void Punk_Helmet_Puts_4_Counters_On_The_Attacker_Even_If_Knocked_Out()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Jet Headbutt"), new ScriptedAgent());
            var drag = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(drag, C(Psychic));
            var zorua = e.ScenarioActive(1, C(Zorua));
            zorua.Tools.Add(e.NewCard(1, C(PunkHelmet)));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(4, drag.DamageCounters);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "Zorua was still Knocked Out");
        }

        [Test]
        public void Ns_Castle_Removes_Retreat_Cost_For_Ns_Pokemon_Only()
        {
            var e = Scenario(new ScriptedAgent().Then("Retreat N's Zekrom"), new ScriptedAgent());
            var zekrom = e.ScenarioActive(0, C(Zekrom));
            var dreepy = e.ScenarioBench(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Retreat), "Zekrom has no Energy for its Retreat Cost of 2");
            e.State.Stadium = e.NewCard(0, C(Castle));
            e.State.StadiumOwner = 0;
            Step(e);
            Assert.AreSame(dreepy, e.State.Players[0].Active);
            Assert.AreEqual(0, zekrom.Energy.Count);
        }

        [Test]
        public void Team_Rockets_Watchtower_Turns_Off_Colorless_Abilities()
        {
            var p1 = new ScriptedAgent().Then("Bench Meowth ex");
            var e = Scenario(p1, new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-129"));          // Drakloak (Dragon)
            e.ScenarioBench(0, C("TEF-129"));           // Dudunsparce (Colorless)
            e.ScenarioAddToHand(0, C("POR-62"));        // Meowth ex (Colorless)
            e.ScenarioAddToDeck(0, C("MEG-119"), 1);
            e.ScenarioActive(1, C("TWM-128"));
            e.State.Stadium = e.NewCard(1, C(Watchtower));
            e.State.StadiumOwner = 1;
            e.ScenarioSetTurn(0, 3);

            var actions = e.GetMainActions(0);
            Assert.IsTrue(actions.Any(o => o.Label.StartsWith("Use Recon Directive")), "Dragon-type Abilities still work");
            Assert.IsFalse(actions.Any(o => o.Label.StartsWith("Use Run Away Draw")), "Colorless Abilities are off");
            Step(e);
            Assert.IsFalse(p1.Seen.Any(r => r.Kind == DecisionKind.YesNo), "Last-Ditch Catch shouldn't trigger");
        }

        // ------------------------------------------------------------------ Whole games

        // Whole games are slow in the Unity Editor (several times this computer's .NET speed), so fewer of them, with a longer limit
        // than the Test Runner's default.
        [Test, Timeout(600000)]
        public void Dragapult_vs_Zoroark_80_Heuristic_Games_Without_Errors()
        {
            var a = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var b = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            var report = MatchRunner.Run(StandardCards.CreateRegistry(), a, b, 80, 2000, (seat, s) => new HeuristicAgent(s), true);
            TestContext.WriteLine(report.Summary());
            Assert.AreEqual(0, report.Errors.Count, report.Summary());
            Assert.Greater(report.Wins[0] + report.Wins[1], 0, report.Summary());
        }

        // Whole games are slow in the Unity Editor (several times this computer's .NET speed), so fewer of them, with a longer limit
        // than the Test Runner's default.
        [Test, Timeout(600000)]
        public void Zoroark_Mirror_And_Cross_100_Random_Games_Without_Errors()
        {
            var a = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var b = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            var scripts = StandardCards.CreateRegistry();
            var r1 = MatchRunner.Run(scripts, b, b, 50, 3000, (seat, s) => new RandomAgent(s), true);
            var r2 = MatchRunner.Run(scripts, b, a, 50, 4000, (seat, s) => new RandomAgent(s), true);
            TestContext.WriteLine(r1.Summary());
            TestContext.WriteLine(r2.Summary());
            Assert.AreEqual(0, r1.Errors.Count, r1.Summary());
            Assert.AreEqual(0, r2.Errors.Count, r2.Summary());
        }
    }
}
