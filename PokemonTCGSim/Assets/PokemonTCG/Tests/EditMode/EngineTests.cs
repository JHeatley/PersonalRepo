using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// Agent for tests: each queued entry picks the option whose label contains that text.
    /// Use "A|B" to pick several options in one multi-select decision.
    /// With nothing queued it ends the turn on main actions and takes the first option(s) otherwise.
    /// </summary>
    public sealed class ScriptedAgent : IPlayerAgent
    {
        private readonly Queue<string> picks = new Queue<string>();
        public readonly List<DecisionRequest> Seen = new List<DecisionRequest>();

        public ScriptedAgent Then(params string[] labels)
        {
            foreach (var l in labels) picks.Enqueue(l);
            return this;
        }

        public List<int> Decide(DecisionRequest r)
        {
            Seen.Add(r);
            // Cards shown with nothing to take (Done) don't use up a scripted answer.
            if (r.MaxChoices == 0 && r.Options.Count == 0) return new List<int>();
            if (picks.Count > 0)
            {
                var result = new List<int>();
                foreach (var want in picks.Dequeue().Split('|'))
                {
                    int idx = -1;
                    for (int i = 0; i < r.Options.Count; i++)
                        if (r.Options[i].Label.Contains(want) && !result.Contains(i)) { idx = i; break; }
                    if (idx < 0)
                        Assert.Fail("No option containing '" + want + "' for '" + r.Prompt + "'. Options: " +
                                    string.Join(" | ", r.Options.Select(o => o.Label).ToArray()));
                    result.Add(idx);
                }
                return result;
            }
            if (r.Kind == DecisionKind.MainAction)
                return new List<int> { r.Options.FindIndex(o => o.Action == ActionType.EndTurn) };
            int n = System.Math.Min(r.MaxChoices, System.Math.Max(r.MinChoices, 1));
            return Enumerable.Range(0, n).ToList();
        }
    }

    /// <summary>Card pool loading and hand-built board helpers shared by the test classes.</summary>
    public abstract class ScenarioTestBase
    {
        protected CardDatabase db;

        [OneTimeSetUp]
        public void LoadCards()
        {
            db = UnityCardData.LoadAllCards();
        }

        protected CardDef C(string key)
        {
            var d = db.Get(key);
            Assert.IsNotNull(d, "Missing card " + key);
            return d;
        }

        /// <summary>Empty board with 6 Prizes and 20 Psychic Energy in each deck. Player 1 is seat 0.</summary>
        protected GameEngine Scenario(ScriptedAgent p1, ScriptedAgent p2)
        {
            var e = new GameEngine(StandardCards.CreateRegistry(), p1, p2);
            e.BeginScenario(42);
            e.ScenarioAddPrizes(0, C("MEE-10"), 6);
            e.ScenarioAddPrizes(1, C("MEE-10"), 6);
            e.ScenarioAddToDeck(0, C("MEE-13"), 20);
            e.ScenarioAddToDeck(1, C("MEE-13"), 20);
            return e;
        }

        protected static void Step(GameEngine e)
        {
            try { e.MainPhaseStep(); }
            catch (GameOverException) { }
        }
    }

    public class EngineTests : ScenarioTestBase
    {

        // ------------------------------------------------------------------ Data

        [Test]
        public void Decklist_Loads_As_Legal_60()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            Assert.AreEqual(60, deck.Count);
            CollectionAssert.IsEmpty(Decklist.Validate(deck, Formats.Standard2026));
            Assert.AreEqual(1, deck.Count(d => d.IsAceSpec));
        }

        [Test]
        public void Every_Card_With_Card_Text_Has_A_Script()
        {
            var scripts = StandardCards.CreateRegistry();
            var missing = db.All.Where(d => (d.Abilities.Count > 0 || d.Attacks.Any(a => !string.IsNullOrEmpty(a.Text)) || d.IsTrainer)
                                            && !scripts.HasScriptFor(d)).Select(d => d.Name).ToList();
            CollectionAssert.IsEmpty(missing, "Cards with effects but no script: " + string.Join(", ", missing.ToArray()));
        }

        // ------------------------------------------------------------------ Turn rules

        [Test]
        public void First_Player_Cannot_Attack_Or_Use_Supporter_On_Turn_1()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var dreepy = e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioAttach(dreepy, C("MEE-13"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C("MEG-114"));

            e.ScenarioSetTurn(0, 1);
            var turn1 = e.GetMainActions(0);
            Assert.IsFalse(turn1.Any(o => o.Action == ActionType.Attack), "attack offered on turn 1");
            Assert.IsFalse(turn1.Any(o => o.Label == "Play Boss's Orders"), "Supporter offered on turn 1");

            e.ScenarioSetTurn(0, 3);
            var turn3 = e.GetMainActions(0);
            Assert.IsTrue(turn3.Any(o => o.Label == "Attack: Petty Grudge"));
            Assert.IsTrue(turn3.Any(o => o.Label == "Play Boss's Orders"));
        }

        [Test]
        public void Cannot_Evolve_On_First_Turns_Or_The_Turn_It_Was_Played()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Dreepy"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C("TWM-128"));
            e.ScenarioAddToHand(0, C("TWM-129"));

            e.ScenarioSetTurn(0, 2);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.Evolve), "evolved on a first turn");

            e.ScenarioSetTurn(0, 3);
            Step(e); // bench the new Dreepy
            var evolveTargets = e.GetMainActions(0).Where(o => o.Action == ActionType.Evolve).Select(o => o.Label).ToList();
            Assert.AreEqual(1, evolveTargets.Count, string.Join(", ", evolveTargets.ToArray()));
            StringAssert.Contains("(Active)", evolveTargets[0]);
        }

        // ------------------------------------------------------------------ Damage

        [Test]
        public void Phantom_Dive_Deals_200_And_Places_6_Counters_On_The_Bench()
        {
            var p1 = new ScriptedAgent().Then("Attack: Phantom Dive");
            for (int i = 0; i < 6; i++) p1.Then("Opponent's Dreepy (Bench 1)");
            var e = Scenario(p1, new ScriptedAgent());
            var drag = e.ScenarioActive(0, C("TWM-130"));
            e.ScenarioAttach(drag, C("MEE-10"));
            e.ScenarioAttach(drag, C("MEE-13"));
            e.ScenarioActive(1, C("TWM-95"));
            var b1 = e.ScenarioBench(1, C("TWM-128"));
            var b2 = e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);

            Step(e);

            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "Munkidori (110 HP) should be Knocked Out for 1 Prize");
            Assert.AreEqual(6, b1.DamageCounters);
            Assert.AreEqual(0, b2.DamageCounters);
            Assert.AreSame(b1, e.State.Players[1].Active, "opponent promotes a new Active");
        }

        [Test]
        public void Tera_Prevents_Attack_Damage_On_The_Bench_But_Not_Damage_Counters()
        {
            var p1 = new ScriptedAgent().Then("Attack: Cruel Arrow", "Opponent's Dragapult ex (Bench 1)");
            var e = Scenario(p1, new ScriptedAgent());
            var fez = e.ScenarioActive(0, C("ASC-142"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(fez, C("MEE-13"));
            e.ScenarioActive(1, C("TWM-128"));
            var benchedDragapult = e.ScenarioBench(1, C("TWM-130"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(0, benchedDragapult.DamageCounters, "Tera should prevent Cruel Arrow on the Bench");

            var p1b = new ScriptedAgent().Then("Attack: Phantom Dive");
            for (int i = 0; i < 6; i++) p1b.Then("Opponent's Dragapult ex (Bench 1)");
            var e2 = Scenario(p1b, new ScriptedAgent());
            var drag = e2.ScenarioActive(0, C("TWM-130"));
            e2.ScenarioAttach(drag, C("MEE-10"));
            e2.ScenarioAttach(drag, C("MEE-13"));
            e2.ScenarioActive(1, C("TWM-128"));
            var target = e2.ScenarioBench(1, C("TWM-130"));
            e2.ScenarioSetTurn(0, 3);
            Step(e2);
            Assert.AreEqual(6, target.DamageCounters, "damage counters are not damage, so Tera doesn't stop them");
        }

        [Test]
        public void Weakness_Doubles_Damage_To_The_Active()
        {
            var p1 = new ScriptedAgent().Then("Attack: Cruel Arrow", "Opponent's Munkidori (Active)");
            var e = Scenario(p1, new ScriptedAgent());
            var fez = e.ScenarioActive(0, C("ASC-142"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(fez, C("MEE-13"));
            var munki = e.ScenarioActive(1, C("TWM-95"));
            e.ScenarioBench(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(5, e.State.Players[0].Prizes.Count, "100 x2 (Darkness Weakness) should KO a 110 HP Munkidori");
            Assert.AreNotSame(munki, e.State.Players[1].Active);
        }

        [Test]
        public void Risky_Ruins_Hits_Benched_Basic_Non_Darkness_Pokemon_Only()
        {
            var p1 = new ScriptedAgent().Then("Bench Dreepy", "Bench Fezandipiti ex");
            var e = Scenario(p1, new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.State.Stadium = e.NewCard(1, C("MEG-127"));
            e.State.StadiumOwner = 1;
            e.ScenarioAddToHand(0, C("TWM-128"));
            e.ScenarioAddToHand(0, C("ASC-142"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Step(e);
            var bench = e.State.Players[0].Bench;
            Assert.AreEqual(2, bench[0].DamageCounters, "Dreepy should take 2 counters");
            Assert.AreEqual(0, bench[1].DamageCounters, "Darkness-type Fezandipiti ex is unaffected");
        }

        // ------------------------------------------------------------------ Card effects

        [Test]
        public void Lillies_Determination_Draws_8_With_6_Prizes_And_6_Otherwise()
        {
            foreach (int prizes in new[] { 6, 5 })
            {
                var e = Scenario(new ScriptedAgent().Then("Play Lillie's Determination"), new ScriptedAgent());
                if (prizes == 5) e.State.Players[0].Prizes.RemoveAt(0);
                e.ScenarioActive(0, C("TWM-128"));
                e.ScenarioActive(1, C("TWM-128"));
                e.ScenarioAddToHand(0, C("MEG-119"));
                e.ScenarioAddToHand(0, C("MEE-10"));
                e.ScenarioAddToHand(0, C("MEE-10"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                Assert.AreEqual(prizes == 6 ? 8 : 6, e.State.Players[0].Hand.Count, "with " + prizes + " prizes");
            }
        }

        [Test]
        public void Itchy_Pollen_Stops_Items_Only_During_The_Next_Turn()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Itchy Pollen"), new ScriptedAgent());
            e.ScenarioActive(0, C("ASC-16"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(1, C("POR-81"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            e.EndTurn();
            e.StartTurn();
            Assert.AreEqual(1, e.State.CurrentPlayer);
            Assert.IsFalse(e.GetMainActions(1).Any(o => o.Label == "Play Poké Pad"), "Item allowed under Itchy Pollen");
            e.EndTurn();
            e.StartTurn();
            e.EndTurn();
            e.StartTurn();
            Assert.IsTrue(e.GetMainActions(1).Any(o => o.Label == "Play Poké Pad"), "lock should have worn off");
        }

        [Test]
        public void Run_Away_Draw_Draws_3_And_Shuffles_Dudunsparce_Into_The_Deck()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Run Away Draw"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            var dud = e.ScenarioBench(0, C("TEF-129"));
            e.ScenarioAttach(dud, C("MEE-10"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(3, me.Hand.Count);
            Assert.AreEqual(0, me.Bench.Count);
            Assert.AreEqual(20 - 3 + 2, me.Deck.Count, "Dudunsparce and its Energy go back into the deck");
        }

        [Test]
        public void Tuck_Tail_Returns_Meowth_And_Attachments_To_Hand_Then_Promotes()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Tuck Tail"), new ScriptedAgent());
            var meowth = e.ScenarioActive(0, C("POR-62"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(meowth, C("MEE-15"));
            var dreepy = e.ScenarioBench(0, C("TWM-128"));
            var target = e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var me = e.State.Players[0];
            Assert.AreEqual(4, me.Hand.Count, "Meowth ex + 3 Energy");
            Assert.AreSame(dreepy, me.Active);
            Assert.AreEqual(6, target.DamageCounters);
        }

        [Test]
        public void Bosss_Orders_Drags_Up_A_Benched_Pokemon()
        {
            var e = Scenario(new ScriptedAgent().Then("Play Boss's Orders", "Opponent's Munkidori (Bench 1)"), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            var oldActive = e.ScenarioActive(1, C("TWM-128"));
            var munki = e.ScenarioBench(1, C("TWM-95"));
            e.ScenarioAddToHand(0, C("MEG-114"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreSame(munki, e.State.Players[1].Active);
            Assert.AreSame(oldActive, e.State.Players[1].Bench[0]);
        }

        [Test]
        public void Ultra_Ball_Needs_Two_Other_Cards()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            e.ScenarioActive(0, C("TWM-128"));
            e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioAddToHand(0, C("MEG-131"));
            e.ScenarioAddToHand(0, C("MEE-10"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label == "Play Ultra Ball"));
            e.ScenarioAddToHand(0, C("MEE-10"));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label == "Play Ultra Ball"));
        }

        [Test]
        public void Adrena_Brain_Moves_Counters_Only_With_Darkness_Energy()
        {
            // Only one damaged Pokémon and one target exist, so the engine picks those automatically; we choose the amount.
            var p1 = new ScriptedAgent().Then("Use Adrena-Brain", "3");
            var e = Scenario(p1, new ScriptedAgent());
            var munki = e.ScenarioActive(0, C("TWM-95"));
            var hurt = e.ScenarioBench(0, C("TWM-128"));
            hurt.DamageCounters = 4;
            var target = e.ScenarioActive(1, C("TWM-128"));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.UseAbility), "no Darkness Energy yet");
            e.ScenarioAttach(munki, C("MEE-15"));
            Step(e);
            Assert.AreEqual(1, hurt.DamageCounters);
            Assert.AreEqual(3, target.DamageCounters);
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Action == ActionType.UseAbility), "once per turn");
        }

        // ------------------------------------------------------------------ Whole games

        [Test]
        public void Same_Seed_Gives_The_Same_Game()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var a = PlayLogged(deck, 99);
            var b = PlayLogged(deck, 99);
            Assert.AreEqual(a, b);
        }

        private string PlayLogged(List<CardDef> deck, int seed)
        {
            var engine = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(1), new HeuristicAgent(2));
            var recorder = new Recorder();
            engine.AddObserver(recorder);
            var result = engine.Run(deck, deck, seed);
            return result.Winner + "|" + result.Turns + "|" + recorder.Text.Count + "|" + string.Join("\n", recorder.Text.ToArray());
        }

        private sealed class Recorder : IGameObserver
        {
            public readonly List<string> Text = new List<string>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Text.Add(ev.Text); }
        }

        [Test]
        public void Heuristic_Mirror_200_Games_Without_Errors()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var report = MatchRunner.Run(StandardCards.CreateRegistry(), deck, deck, 200, 500, (seat, s) => new HeuristicAgent(s), true);
            TestContext.WriteLine(report.Summary());
            Assert.AreEqual(0, report.Errors.Count, report.Summary());
            Assert.Greater(report.Wins[0] + report.Wins[1], 0, report.Summary());
        }

        [Test]
        public void Random_Mirror_300_Games_Without_Errors()
        {
            var deck = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var report = MatchRunner.Run(StandardCards.CreateRegistry(), deck, deck, 300, 900, (seat, s) => new RandomAgent(s), true);
            TestContext.WriteLine(report.Summary());
            Assert.AreEqual(0, report.Errors.Count, report.Summary());
        }
    }
}
