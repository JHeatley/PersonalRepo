using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// v20: the learning AI (brain, agent, training), resuming saved positions, the Areas of Improvement analyzer, and
    /// 30th Celebration's Classic Collection (Unlimited format, LEGEND, LV.X, GX, Lost Zone, Pokémon Powers).
    /// </summary>
    public class V20Tests : ScenarioTestBase
    {
        const string Dreepy = "TWM-128";
        const string Psychic = "MEE-13";
        const string Fighting = "MEE-14";
        const string Fire = "MEE-10";

        private List<CardDef> Dragapult() { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); }
        private List<CardDef> Zoroark() { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); }

        private sealed class TextLog : IGameObserver
        {
            public readonly List<string> Lines = new List<string>();
            public void OnEvent(GameEngine engine, GameEvent ev) { Lines.Add(ev.Text); }
        }

        // ================================================================== Learning AI

        [Test]
        public void Brain_Saves_And_Loads_Everything()
        {
            var brain = new LearningBrain();
            brain.SetWeight("M:Trainer:Ultra Ball|early", 1.25);
            brain.SetWeight("D:Deck A|M:End|atk1", -0.5);
            var decision = new LearnedDecision
            {
                Turn = 3,
                OptionFeatures = new List<List<string>> { new List<string> { "M:Attack:X/Y" }, new List<string> { "M:End|atk1" } },
                Chosen = new List<int> { 0 },
                Greedy = new List<int> { 1 },
                Explored = true,
                MyPrizes = 6,
                OppPrizes = 6
            };
            brain.LearnFromGame(new List<LearnedDecision> { decision }, 1, 10, 0, 3, "Deck A", "Deck B");
            brain.AddSession(new SessionRecord { When = "2026-10-08 12:00", DeckA = "Deck A", DeckB = "Deck B", Games = 100, WinsA = 55, WinsB = 45, ScoreBefore = 0.5, ScoreAfter = 0.56, EvalGames = 200 });
            brain.IncrementCounters(100, 2, 37);

            var path = Path.Combine(Path.GetTempPath(), "pokemontcg_brain_test.txt");
            brain.FilePath = path;
            brain.Save();
            var loaded = LearningBrain.Load(path);
            File.Delete(path);

            Assert.AreEqual(1.25, loaded.WeightOf("M:Trainer:Ultra Ball|early"), 1e-6);
            Assert.AreEqual(-0.5, loaded.WeightOf("D:Deck A|M:End|atk1"), 1e-6);
            Assert.AreEqual(brain.WeightOf("M:Attack:X/Y"), loaded.WeightOf("M:Attack:X/Y"), 1e-6);
            Assert.Greater(loaded.WeightOf("M:Attack:X/Y"), 0, "the explored play that won was credited");
            Assert.AreEqual(brain.FeatureCount, loaded.FeatureCount);
            Assert.AreEqual(100, loaded.GamesTrained);
            Assert.AreEqual(2, loaded.GamesVsYou);
            Assert.AreEqual(37, loaded.YourDecisionsStudied);
            Assert.AreEqual(1, loaded.Matchups().Count);
            Assert.AreEqual("Deck A", loaded.Matchups()[0].Deck);
            Assert.AreEqual(1, loaded.History().Count);
            Assert.AreEqual(0.56, loaded.History()[0].ScoreAfter, 1e-6);
        }

        [Test]
        public void Strongest_Lessons_Can_Return_Every_Lesson_Strongest_First()
        {
            var brain = new LearningBrain();
            var sb = new System.Text.StringBuilder();
            for (int k = 0; k < 40; k++)
            {
                double w = (k % 2 == 0 ? 1 : -1) * (0.1 + k * 0.01);
                sb.Append("w\t").Append(w.ToString(System.Globalization.CultureInfo.InvariantCulture)).Append("\t6\tG:lesson").Append(k).Append('\n');
            }
            sb.Append("w\t0.9\t2\tG:too few updates\n");
            sb.Append("w\t0.01\t50\tG:too small\n");
            brain.LoadText(sb.ToString());

            var all = brain.StrongestLessons(0, 5);
            Assert.AreEqual(40, all.Count, "every lesson, not just the top 10 (the Train screen scrolls through them)");
            for (int k = 1; k < all.Count; k++)
                Assert.GreaterOrEqual(System.Math.Abs(all[k - 1].Value), System.Math.Abs(all[k].Value), "strongest first");
            Assert.AreEqual("G:lesson39", all[0].Key);
            Assert.IsFalse(all.Any(kv => kv.Key == "G:too few updates" || kv.Key == "G:too small"));
            Assert.AreEqual(10, brain.StrongestLessons(10, 5).Count);
        }

        [Test]
        public void With_An_Empty_Brain_The_Learning_AI_Plays_Exactly_Like_The_Classic_Bot()
        {
            var deck = Dragapult();
            var classic = new TextLog();
            var e1 = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(7), new HeuristicAgent(8));
            e1.AddObserver(classic);
            var r1 = e1.Run(deck, Zoroark(), 321);

            var learning = new TextLog();
            var brain = new LearningBrain();
            var e2 = new GameEngine(StandardCards.CreateRegistry(), new LearningAgent(brain, 7, "A", "B"), new LearningAgent(brain, 8, "B", "A"));
            e2.AddObserver(learning);
            var r2 = e2.Run(deck, Zoroark(), 321);

            Assert.AreEqual(r1.Winner, r2.Winner);
            Assert.AreEqual(r1.Turns, r2.Turns);
            CollectionAssert.AreEqual(classic.Lines, learning.Lines);
        }

        [Test]
        public void Learning_Agents_Explore_And_Play_Full_Games_Without_Errors()
        {
            var brain = new LearningBrain();
            var report = MatchRunner.Run(StandardCards.CreateRegistry(), Dragapult(), Zoroark(), 20, 4100,
                (seat, s) => new LearningAgent(brain, s, seat == 0 ? "Dragapult" : "Zoroark", seat == 0 ? "Zoroark" : "Dragapult") { Training = true, ExploreRate = 0.2 }, true);
            Assert.AreEqual(0, report.Errors.Count, report.Summary());
        }

        [Test]
        public void Training_Session_Learns_From_Every_Game_And_Measures_Itself()
        {
            var brain = new LearningBrain();
            var session = new TrainingSession(StandardCards.CreateRegistry(), brain, "Dragapult", Dragapult(), "Zoroark", Zoroark(), 20, 10, 777);
            session.RunBlocking();
            var st = session.Status();
            Assert.IsTrue(st.Finished);
            Assert.AreEqual(20, st.Done);
            Assert.AreEqual(0, st.Errors, st.LastError);
            Assert.AreEqual(20, st.WinsA + st.WinsB + st.Draws);
            Assert.AreEqual(20, brain.GamesTrained);
            Assert.Greater(brain.FeatureCount, 0, "it learned something");
            Assert.That(st.ScoreBefore, Is.InRange(0.0, 1.0));
            Assert.That(st.ScoreAfter, Is.InRange(0.0, 1.0));
            Assert.AreEqual(1, brain.History().Count);
            Assert.IsNotEmpty(session.BrainBefore);
            Assert.IsTrue(brain.Matchups().Any(m => m.Deck == "Dragapult" && m.Opponent == "Zoroark"));
        }

        [Test]
        public void The_AI_Studies_Your_Decisions()
        {
            var log = new DecisionLog();
            var you = new RecordingAgent(new RandomAgent(3), log);
            var engine = new GameEngine(StandardCards.CreateRegistry(), you, new HeuristicAgent(4));
            log.Engine = engine;
            engine.Run(Dragapult(), Zoroark(), 55);
            Assert.Greater(log.Count, 10);
            Assert.IsTrue(log.Decisions().Any(d => d.Snapshot != null), "main actions keep a snapshot of the game");

            var brain = new LearningBrain();
            int studied = LearningAgent.LearnFromPlayer(brain, log.Decisions(), "Dragapult", "Zoroark", 1.0);
            Assert.Greater(studied, 0);
            Assert.Greater(brain.FeatureCount, 0, "a random player differs from the AI somewhere");
        }

        // ================================================================== Resuming a saved position

        private static List<RecordedDecision> RecordGame(GameEngine engine, DecisionLog log, List<CardDef> a, List<CardDef> b, int seed)
        {
            log.Engine = engine;
            engine.Run(a, b, seed);
            return log.Decisions();
        }

        [Test]
        public void A_Saved_Position_Plays_On_To_The_End_With_Every_Card_Accounted_For()
        {
            var log = new DecisionLog();
            var engine = new GameEngine(StandardCards.CreateRegistry(), new RecordingAgent(new HeuristicAgent(1), log), new HeuristicAgent(2));
            var decisions = RecordGame(engine, log, Dragapult(), Zoroark(), 99);
            var mid = decisions.Where(d => d.Snapshot != null && d.Turn >= 5).ToList();
            Assume.That(mid.Count, Is.GreaterThan(0), "the game was long enough");

            foreach (var d in mid.Take(3))
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(11), new HeuristicAgent(12));
                e.CheckInvariants = true;
                e.CardsPerPlayer = 60;
                e.LoadState(d.Snapshot.Clone());
                e.AssertInvariants();
                var result = e.ResumeGame();
                Assert.IsTrue(e.State.IsOver);
                Assert.That(result.Winner, Is.InRange(-1, 1));
            }
        }

        [Test]
        public void Shuffling_What_You_Couldnt_See_Keeps_Every_Card_And_Your_Hand()
        {
            var log = new DecisionLog();
            var engine = new GameEngine(StandardCards.CreateRegistry(), new RecordingAgent(new HeuristicAgent(1), log), new HeuristicAgent(2));
            var decisions = RecordGame(engine, log, Dragapult(), Zoroark(), 101);
            var snap = decisions.First(d => d.Snapshot != null && d.Turn >= 3).Snapshot;
            var world = GameAnalyzer.Determinize(snap, 0, 5);

            CollectionAssert.AreEqual(snap.Players[0].Hand.Select(c => c.Id), world.Players[0].Hand.Select(c => c.Id), "your hand is known");
            CollectionAssert.AreEquivalent(snap.Players[0].Deck.Concat(snap.Players[0].Prizes).Select(c => c.Id), world.Players[0].Deck.Concat(world.Players[0].Prizes).Select(c => c.Id));
            CollectionAssert.AreEquivalent(snap.Players[1].Hand.Concat(snap.Players[1].Deck).Concat(snap.Players[1].Prizes).Select(c => c.Id),
                world.Players[1].Hand.Concat(world.Players[1].Deck).Concat(world.Players[1].Prizes).Select(c => c.Id));
            Assert.AreEqual(snap.Players[1].Hand.Count, world.Players[1].Hand.Count);

            var e = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(1), new HeuristicAgent(2));
            e.CardsPerPlayer = 60;
            e.LoadState(world);
            e.AssertInvariants();
        }

        // ================================================================== Areas of Improvement

        [Test]
        public void Areas_Of_Improvement_Reviews_A_Game()
        {
            var log = new DecisionLog();
            var events = new EventCollector();
            var engine = new GameEngine(StandardCards.CreateRegistry(), new RecordingAgent(new RandomAgent(9), log), new HeuristicAgent(10));
            engine.AddObserver(events);
            RecordGame(engine, log, Dragapult(), Zoroark(), 202);

            var analyzer = new GameAnalyzer(StandardCards.CreateRegistry(), new LearningBrain(), log.Decisions(), events.Entries(), 0, "Dragapult", "Zoroark", engine.State, engine.PrizeCount)
            {
                RolloutsPerOption = 4,
                MaxCandidates = 2,
                RolloutTurnCap = 10
            };
            var review = analyzer.Run();
            string phase, error;
            int done, total;
            bool finished;
            analyzer.Progress(out phase, out done, out total, out finished, out error);
            Assert.IsTrue(finished);
            Assert.IsNull(error, error);
            Assert.IsNotNull(review);
            Assert.IsNotEmpty(review.Headline);
            Assert.IsNotEmpty(review.Summary);
            Assert.Greater(review.DecisionsCompared, 0);
            Assert.Greater(review.Differences, 0, "a random player plays differently from the AI");
            foreach (var item in review.Items.Where(i => i.Section == ReviewItem.BetterPlays))
            {
                Assert.Greater(item.SuggestedChance, item.YourChance);
                Assert.IsNotNull(item.Before, "previews were built");
            }
        }

        [Test]
        public void Areas_Of_Improvement_Uses_The_Neural_AI_And_Keeps_Your_Habits_Across_Games()
        {
            var brain = new LearningBrain();
            brain.Policy.Reset();
            // A round of self-play, so the network has opinions (a fresh one rates every option the same) and counts as trained.
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>("Dragapult", Dragapult()),
                new KeyValuePair<string, List<CardDef>>("Zoroark", Zoroark())
            };
            var trainer = new PolicyTrainer(brain.Policy, StandardCards.CreateRegistry(), decks, 8) { GamesPerRound = 6, Threads = 2, Minibatch = 256 };
            trainer.Round();
            trainer.Dispose();
            brain.Policy.GamesTrained = Math.Max(1, brain.Policy.GamesTrained);
            GameReview last = null;
            for (int game = 0; game < 2; game++)
            {
                var log = new DecisionLog();
                var events = new EventCollector();
                var engine = new GameEngine(StandardCards.CreateRegistry(), new RecordingAgent(new RandomAgent(30 + game), log), new HeuristicAgent(40 + game));
                engine.AddObserver(events);
                RecordGame(engine, log, Dragapult(), Zoroark(), 303 + game);
                var analyzer = new GameAnalyzer(StandardCards.CreateRegistry(), brain, log.Decisions(), events.Entries(), 0, "Dragapult", "Zoroark", engine.State, engine.PrizeCount)
                {
                    RolloutsPerOption = 4,
                    MaxCandidates = 2,
                    RolloutTurnCap = 10,
                    // A barely trained network has no strong opinions yet: count any play it doesn't rank first, to exercise the habits.
                    NeuralGap = 0,
                    NeuralKeepsYours = 0.95,
                    YourDeckList = engine.StartingDeck(0).ToList(),
                    TheirDeckList = engine.StartingDeck(1).ToList()
                };
                last = analyzer.Run();
                string phase, error;
                int done, total;
                bool finished;
                analyzer.Progress(out phase, out done, out total, out finished, out error);
                Assert.IsNull(error, error);
                Assert.AreEqual("Neural AI", last.Judge);
                Assert.Greater(last.DecisionsCompared, 0);
                Assert.Greater(last.Differences, 0, "a random player plays differently from the network");
                Assert.AreEqual(game + 1, last.HabitReviews);
                foreach (var item in last.Items.Where(i => i.Section == ReviewItem.BetterPlays)) Assert.Greater(item.SuggestedChance, item.YourChance);
            }
            var habits = last.Items.Where(i => i.Section == ReviewItem.Habits).ToList();
            Assert.IsNotEmpty(habits);
            Assert.LessOrEqual(habits.Count, 5);
            foreach (var h in habits)
            {
                StringAssert.StartsWith("This game: turn ", h.Detail);
                Assert.IsTrue(h.Detail.Contains("reviewed games") || h.Detail.Contains("first time"), h.Detail);
            }
            Assert.AreEqual(2, brain.HabitReviews);
            var profile = brain.Habits();
            Assert.IsNotEmpty(profile);
            Assert.IsTrue(profile.All(x => x.Games >= 1 && x.Games <= 2 && x.Times >= x.Games && x.Recent > 0 && !string.IsNullOrEmpty(x.Title)));

            // Kept in the brain's file.
            var copy = new LearningBrain();
            copy.LoadText(brain.Serialize());
            Assert.AreEqual(2, copy.HabitReviews);
            Assert.AreEqual(profile.Count, copy.Habits().Count);
            Assert.AreEqual(profile.Sum(x => x.Times), copy.Habits().Sum(x => x.Times));
        }

        [Test]
        public void Habits_Get_Rarer_Lately_When_They_Stop_Coming_Up()
        {
            var brain = new LearningBrain();
            var habit = new HabitInGame { Key = "end>energy", Title = "Ending your turn without attaching an Energy", Times = 2, Strength = 0.4 };
            for (int i = 0; i < 5; i++) brain.RecordHabits(new List<HabitInGame> { habit });
            double often = brain.Habits().Single().Recent;
            for (int i = 0; i < 5; i++) brain.RecordHabits(new List<HabitInGame>());
            var h = brain.Habits().Single();
            Assert.Less(h.Recent, often / 2);
            Assert.AreEqual(5, h.Games);
            Assert.AreEqual(10, h.Times);
            Assert.AreEqual(0.4, h.AverageStrength, 1e-9);
            Assert.AreEqual(10, brain.HabitReviews);
        }

        [Test]
        public void Disagreeing_Moves_The_AI_Toward_Your_Play()
        {
            var log = new DecisionLog();
            var engine = new GameEngine(StandardCards.CreateRegistry(), new RecordingAgent(new RandomAgent(21), log), new HeuristicAgent(22));
            var decisions = RecordGame(engine, log, Dragapult(), Zoroark(), 303);
            var brain = new LearningBrain();

            ReviewItem item = null;
            double before = 0;
            foreach (var d in decisions.Where(x => x.Request.Kind == DecisionKind.MainAction && x.Request.Options.Count >= 3))
            {
                List<List<string>> feats;
                double[] adjusted;
                var best = LearningAgent.BestFor(brain, d.Request, "Dragapult", "Zoroark", out feats, out adjusted);
                for (int i = 0; i < adjusted.Length; i++)
                {
                    if (i == best[0] || adjusted[i] < 5 || adjusted[best[0]] < 5) continue; // two setup plays
                    item = new ReviewItem { Decision = d, YourOption = i, SuggestedOption = best[0] };
                    before = adjusted[i] - adjusted[best[0]];
                    break;
                }
                if (item != null) break;
            }
            Assume.That(item, Is.Not.Null, "found a choice between two setup plays");

            GameAnalyzer.ApplyFeedback(brain, item, "Dragapult", "Zoroark", true);
            List<List<string>> feats2;
            double[] after;
            LearningAgent.BestFor(brain, item.Decision.Request, "Dragapult", "Zoroark", out feats2, out after);
            Assert.Greater(after[item.YourOption] - after[item.SuggestedOption], before);
            Assert.AreEqual(1, brain.FeedbackGiven);
        }

        // ================================================================== Classic Collection: formats

        [Test]
        public void Classic_Collection_Before_Black_And_White_Is_Unlimited_And_The_Rest_Expanded()
        {
            for (int n = 1; n <= 30; n++)
            {
                var d = C("30C-CC" + n);
                if (n <= 17)
                {
                    Assert.IsTrue(d.UnlimitedOnly, d.Name + " (CC" + n + ") is Unlimited only");
                    Assert.AreEqual(CardCategory.Unlimited, Formats.CategoryOf(d), d.Name);
                    Assert.IsFalse(Formats.Allows(DeckFormat.Expanded, d), d.Name);
                }
                else
                {
                    Assert.IsFalse(d.UnlimitedOnly, d.Name + " (CC" + n + ") is Expanded");
                    Assert.AreEqual(CardCategory.Expanded, Formats.CategoryOf(d), d.Name);
                    Assert.IsTrue(Formats.Allows(DeckFormat.Expanded, d), d.Name);
                }
                Assert.IsFalse(Formats.Allows(DeckFormat.Standard, d), d.Name + " isn't Standard");
                Assert.IsTrue(Formats.Allows(DeckFormat.Unlimited, d), d.Name);
            }
        }

        [Test]
        public void Unlimited_Decks_Take_Classic_Cards_And_Expanded_Decks_Dont()
        {
            var deck = new List<CardDef>();
            for (int i = 0; i < 4; i++) deck.Add(C("30C-CC1"));
            for (int i = 0; i < 56; i++) deck.Add(C(Fire));
            Assert.IsTrue(Decklist.Validate(deck, DeckFormat.Expanded).Any(p => p.Contains("Unlimited")));
            CollectionAssert.IsEmpty(Decklist.Validate(deck, DeckFormat.Unlimited));
            Assert.AreEqual(DeckFormat.Unlimited, Formats.Needed(deck));

            var text = Decklist.Format("Classic", Decklist.Group(deck), DeckFormat.Unlimited);
            StringAssert.Contains("# Format: Unlimited", text);
            Assert.AreEqual(DeckFormat.Unlimited, Decklist.ReadFormat(text));

            deck[0] = C("30C-CC6");
            deck[1] = C("30C-CC6");
            Assert.IsTrue(Decklist.Validate(deck, DeckFormat.Unlimited).Any(p => p.Contains("Shining Celebi")), "only 1 Shining Pokémon of a name");
        }

        // ================================================================== Classic Collection: mechanics

        [Test]
        public void Legend_Needs_Both_Halves_And_Is_Worth_2_Prizes()
        {
            var e = Scenario(new ScriptedAgent().Then("Bench Darkrai & Cresselia LEGEND"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAddToHand(0, C("30C-CC16"));
            Assert.IsFalse(e.GetMainActions(0).Any(o => o.Label.Contains("LEGEND")), "the top half alone can't be played");
            e.ScenarioAddToHand(0, C("30C-CC17"));
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.Contains("LEGEND")));
            Step(e);
            var legend = e.State.Players[0].Bench.Single();
            Assert.AreEqual(2, legend.Stack.Count, "both halves make one Pokémon");
            Assert.AreEqual(0, e.State.Players[0].Hand.Count);
            Assert.AreEqual(2, e.PrizesFor(legend));
            Assert.IsTrue(e.IsType(legend, EnergyType.Psychic) && e.IsType(legend, EnergyType.Darkness), "Darkness and Psychic");
            Assert.IsFalse(C("30C-CC16").IsBasicPokemon, "not a Basic Pokémon for setup");
        }

        [Test]
        public void Level_Up_Goes_Only_On_The_Active_Pokemon_And_Keeps_Its_Attacks()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var active = e.ScenarioActive(0, C("30C-20"));
            var benched = e.ScenarioBench(0, C("30C-20"));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAddToHand(0, C("30C-CC12"));
            var evolves = e.GetMainActions(0).Where(o => o.Action == ActionType.Evolve).ToList();
            Assert.IsTrue(evolves.Any(o => o.PokemonUid == active.Uid));
            Assert.IsFalse(evolves.Any(o => o.PokemonUid == benched.Uid), "LV.X only onto the Active Pokémon");
            e.ScenarioEvolve(active, C("30C-CC12"));
            var attacks = e.AttacksOf(active).Select(a => a.Attack.Name).ToList();
            CollectionAssert.Contains(attacks, "Hydro Reflect");
            CollectionAssert.Contains(attacks, "Wormhole", "Palkia's own attack is still usable");
            Assert.AreEqual(1, e.PrizesFor(active), "LV.X gives 1 Prize card");
        }

        [Test]
        public void Only_One_Gx_Attack_Per_Game()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var buzzwole = e.ScenarioActive(0, C("30C-CC24"));
            for (int i = 0; i < 3; i++) e.ScenarioAttach(buzzwole, C(Fighting));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.UsableAttacks(buzzwole).Any(a => a.Attack.Name == "Absorption-GX"));
            e.State.Players[0].GxUsed = true;
            Assert.IsFalse(e.UsableAttacks(buzzwole).Any(a => a.Attack.Name == "Absorption-GX"));
            Assert.IsTrue(e.UsableAttacks(buzzwole).Any(a => a.Attack.Name == "Knuckle Impact"));
            Assert.AreEqual(2, e.PrizesFor(buzzwole));
        }

        [Test]
        public void Gengar_Catastrophe_Sends_Knocked_Out_Pokemon_To_The_Lost_Zone()
        {
            var e = Scenario(new ScriptedAgent().Then("Attack: Cursed Drop"), new ScriptedAgent());
            e.CheckInvariants = true;
            e.CardsPerPlayer = 0;
            var gengar = e.ScenarioActive(0, C("30C-CC15"));
            e.ScenarioAttach(gengar, C(Psychic));
            e.ScenarioAttach(gengar, C(Psychic));
            var magikarp = e.ScenarioActive(1, C("30C-CC30"));
            e.ScenarioBench(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var opp = e.State.Players[1];
            Assert.IsTrue(opp.LostZone.Any(c => c.Def.Name == "Magikarp"), "Magikarp went to the Lost Zone");
            Assert.IsFalse(opp.Discard.Any(c => c.Def.Name == "Magikarp"));
            Assert.AreNotEqual(magikarp, opp.Active);
            Assert.Less(e.State.Players[0].Prizes.Count, 6, "a Prize card was still taken");
        }

        [Test]
        public void Energy_Burn_Turns_Every_Energy_Into_Fire()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Energy Burn"), new ScriptedAgent());
            var charizard = e.ScenarioActive(0, C("30C-CC2"));
            for (int i = 0; i < 4; i++) e.ScenarioAttach(charizard, C(Psychic));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e.UsableAttacks(charizard).Any(a => a.Attack.Name == "Fire Spin"));
            Step(e);
            Assert.IsTrue(e.UsableAttacks(charizard).Any(a => a.Attack.Name == "Fire Spin"));
        }

        // ================================================================== Your rulings on the v20 list

        [Test]
        public void Cinnabar_Lure_Can_Bench_Evolution_Pokemon()
        {
            var agent = new ScriptedAgent().Then("Attack: Cinnabar Lure");
            var e = Scenario(agent, new ScriptedAgent());
            var tatsugiri = e.ScenarioActive(0, C("SSP-142"));
            e.ScenarioAttach(tatsugiri, C(Fire));
            e.ScenarioAttach(tatsugiri, C("MEE-11"));
            e.ScenarioAttach(tatsugiri, C("MEE-15"));
            e.ScenarioActive(1, C(Dreepy));
            e.State.Players[0].Deck.Insert(0, e.NewCard(0, C("TEF-71"))); // Duosion
            e.State.Players[0].Deck.Insert(1, e.NewCard(0, C("TEF-70"))); // Solosis
            e.ScenarioSetTurn(0, 3);
            Step(e);
            var choice = agent.Seen.FirstOrDefault(r => r.Kind == DecisionKind.ChooseCards);
            Assert.IsNotNull(choice);
            Assert.IsTrue(choice.Options.Any(o => o.Card.Name == "Duosion"), "your ruling: Evolution Pokémon can be put onto the Bench too");
            Assert.IsTrue(choice.Options.Any(o => o.Card.Name == "Solosis"));
            var duosion = e.State.Players[0].Bench.FirstOrDefault(m => m.Name == "Duosion");
            Assert.IsNotNull(duosion);
            Assert.AreEqual(1, duosion.Stack.Count, "put into play on its own, not evolved");
        }

        [Test]
        public void A_Gx_Attack_Stopped_By_Confusion_Is_Not_Used_Up()
        {
            int failed = 0, worked = 0;
            for (int seed = 1; seed <= 40 && (failed == 0 || worked == 0); seed++)
            {
                var e = new GameEngine(StandardCards.CreateRegistry(), new ScriptedAgent().Then("Attack: Absorption-GX"), new ScriptedAgent());
                e.BeginScenario(seed);
                e.ScenarioAddPrizes(0, C(Fire), 6);
                e.ScenarioAddPrizes(1, C(Fire), 6);
                e.ScenarioAddToDeck(0, C(Psychic), 10);
                e.ScenarioAddToDeck(1, C(Psychic), 10);
                var buzzwole = e.ScenarioActive(0, C("30C-CC24"));
                for (int i = 0; i < 3; i++) e.ScenarioAttach(buzzwole, C(Fighting));
                buzzwole.Conditions = SpecialCondition.Confused;
                var target = e.ScenarioActive(1, C("TWM-130"));
                e.ScenarioSetTurn(0, 3);
                Step(e);
                if (target.DamageCounters == 0)
                {
                    failed++;
                    Assert.IsFalse(e.State.Players[0].GxUsed, "the attack didn't happen, so the GX attack isn't used up (your ruling)");
                }
                else
                {
                    worked++;
                    Assert.IsTrue(e.State.Players[0].GxUsed);
                }
            }
            Assert.Greater(failed, 0, "a Confusion coin came up tails at least once");
            Assert.Greater(worked, 0);
        }

        [Test]
        public void Uxie_Has_Weakness_Plus_20()
        {
            Assert.AreEqual(20, C("30C-CC13").WeaknessAmount);
            var e = Scenario(new ScriptedAgent().Then("Attack: Psychic"), new ScriptedAgent());
            var mew = e.ScenarioActive(0, C("30C-65"));
            e.ScenarioAttach(mew, C(Psychic));
            e.ScenarioAttach(mew, C(Psychic));
            var uxie = e.ScenarioActive(1, C("30C-CC13"));
            e.ScenarioSetTurn(0, 3);
            Step(e);
            Assert.AreEqual(3, uxie.DamageCounters, "10 damage +20 Weakness, not x2");
        }

        [Test]
        public void Pokemon_Powers_And_Poke_Bodies_Are_Not_Abilities()
        {
            var e = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var delcatty = e.ScenarioActive(0, C("30C-CC8")); // Colorless, Poké-Power Energy Draw
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioSetStadium(1, db.GetByName("Team Rocket's Watchtower")); // Colorless Pokémon have no Abilities
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Energy Draw")), "Ability lock doesn't stop a Poké-Power");
            Assert.IsFalse(e.HasAbility(delcatty), "it doesn't count as a Pokémon with an Ability");
            Assert.IsTrue(e.HasWorkingAbilityOrPower(delcatty));
            Assert.IsFalse(C("30C-CC8").HasAbility);
            Assert.IsTrue(C("30C-CC26").HasAbility, "Zacian V's Intrepid Sword is a real Ability");
        }

        [Test]
        public void Lv_X_Keeps_Poke_Powers_But_Not_Pokemon_Powers()
        {
            var e = Scenario(new ScriptedAgent().Then("Use Delta Control"), new ScriptedAgent());
            var mon = e.ScenarioActive(0, C("30C-CC11")); // Metagross: Poké-Power Delta Control
            e.ScenarioEvolve(mon, C("30C-CC12"));        // a LV.X put on top (test setup only)
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            Assert.IsTrue(e.GetMainActions(0).Any(o => o.Label.StartsWith("Use Delta Control")), "a LV.X keeps the Poké-Powers under it");
            int hand = e.State.Players[0].Hand.Count;
            Step(e);
            Assert.AreEqual(hand + 1, e.State.Players[0].Hand.Count, "Delta Control worked from under the LV.X");

            var e2 = Scenario(new ScriptedAgent(), new ScriptedAgent());
            var zard = e2.ScenarioActive(0, C("30C-CC2")); // Charizard: Pokémon Power Energy Burn
            e2.ScenarioAttach(zard, C(Psychic));
            e2.ScenarioEvolve(zard, C("30C-CC12"));
            e2.ScenarioActive(1, C(Dreepy));
            e2.ScenarioSetTurn(0, 3);
            Assert.IsFalse(e2.GetMainActions(0).Any(o => o.Label.StartsWith("Use Energy Burn")), "Pokémon Powers aren't kept (your ruling)");
            Assert.IsNull(e2.InheritedPowersCard(zard));
        }

        [Test]
        public void N_Shuffles_Both_Hands_And_Draws_By_Prize_Cards()
        {
            var e = Scenario(new ScriptedAgent().Then("Play N"), new ScriptedAgent());
            e.ScenarioActive(0, C(Dreepy));
            e.ScenarioActive(1, C(Dreepy));
            e.ScenarioSetTurn(0, 3);
            e.ScenarioAddToHand(0, C("30C-CC18"));
            e.ScenarioAddToHand(0, C(Psychic));
            e.ScenarioAddToHand(1, C(Psychic));
            e.State.Players[1].Prizes.RemoveAt(0);
            Step(e);
            Assert.AreEqual(6, e.State.Players[0].Hand.Count);
            Assert.AreEqual(5, e.State.Players[1].Hand.Count);
        }
    }
}
