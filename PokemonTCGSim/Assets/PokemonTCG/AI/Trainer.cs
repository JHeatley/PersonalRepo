using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>Results of one stretch of training games (the Train screen shows one row per block).</summary>
    public sealed class TrainingBlock
    {
        public int From;
        public int To;
        public int WinsA;
        public int WinsB;
        public int Draws;
        public int Errors;
        public int TotalTurns;
        public int Explored;
        public int Decisions;

        public int Played { get { return WinsA + WinsB + Draws; } }
        public double AverageTurns { get { return Played == 0 ? 0 : (double)TotalTurns / Played; } }

        public TrainingBlock Copy()
        {
            return (TrainingBlock)MemberwiseClone();
        }
    }

    /// <summary>A snapshot of a training session for the UI (one consistent copy per frame).</summary>
    public sealed class TrainingStatus
    {
        public string Phase = "";
        public int Done;
        public int Games;
        public int WinsA;
        public int WinsB;
        public int Draws;
        public int Errors;
        public int EvalDone;
        public int EvalTotal;
        /// <summary>Win rate (0..1) of the learning AI against the classic bot, or -1 when not measured (yet).</summary>
        public double ScoreBefore = -1;
        public double ScoreAfter = -1;
        public string LastError;
        public List<TrainingBlock> Blocks = new List<TrainingBlock>();
        public bool Running;
        public bool Finished;
        public bool Cancelled;
        public double Seconds;
        public double GamesPerSecond;
    }

    /// <summary>
    /// Trains the learning AI: it plays deck A against deck B (seats alternate, both sides share the brain and both
    /// learn), then optionally measures itself against the classic bot before and after with the same games, so you
    /// can see whether it actually got better. Runs on its own thread; Cancel() stops it after the current game.
    /// </summary>
    public sealed class TrainingSession
    {
        public readonly string DeckA;
        public readonly string DeckB;
        public readonly int Games;
        /// <summary>Games per measurement against the classic bot (0 = don't measure). Half with each deck.</summary>
        public readonly int EvalGames;

        private readonly List<CardDef> cardsA, cardsB;
        private readonly CardScriptRegistry scripts;
        private readonly LearningBrain brain;
        private readonly int seedBase;
        private readonly int blockSize;

        private readonly object gate = new object();
        private readonly TrainingStatus status = new TrainingStatus();
        private TrainingBlock block;
        private volatile bool cancel;
        private Thread thread;
        private DateTime started;
        private int trainingDone;

        /// <summary>The brain as it was before this session (for "Undo this session").</summary>
        public string BrainBefore { get; private set; }

        /// <summary>Saves the brain every this many games (and at the end).</summary>
        public int SaveEvery = 100;

        /// <summary>
        /// How the AI plays in this session's games and its measuring games (v78: the Train screen's choice); null plays the
        /// brain's Style. Both the feature weights and the network learn from every game whatever the style.
        /// </summary>
        public AiStyle? Style;
        /// <summary>The decks' format rules and Prize cards (custom formats, v123). Set before Start.</summary>
        public GameRules Rules = GameRules.Default;
        public int PrizeCount = 6;

        /// <summary>"Learning AI", "Neural AI" or "Hybrid AI" for the style this session plays.</summary>
        public string StyleLabel
        {
            get
            {
                var style = Style.HasValue ? Style.Value : brain.Style;
                return style == AiStyle.Learning ? "Learning AI" : style == AiStyle.Neural ? "Neural AI" : "Hybrid AI";
            }
        }

        public TrainingSession(CardScriptRegistry scripts, LearningBrain brain, string deckA, List<CardDef> cardsA, string deckB, List<CardDef> cardsB, int games, int evalGames, int seed)
        {
            this.scripts = scripts;
            this.brain = brain;
            DeckA = deckA;
            DeckB = deckB;
            this.cardsA = cardsA;
            this.cardsB = cardsB;
            Games = Math.Max(1, games);
            EvalGames = Math.Max(0, evalGames);
            seedBase = seed & 0x0FFFFFFF;
            blockSize = Math.Max(10, Games / 10);
            status.Games = Games;
            status.EvalTotal = EvalGames > 0 ? EvalGames * 2 : 0;
            status.Phase = "Waiting to start";
        }

        public TrainingStatus Status()
        {
            lock (gate)
            {
                var s = new TrainingStatus
                {
                    Phase = status.Phase, Done = status.Done, Games = status.Games, WinsA = status.WinsA, WinsB = status.WinsB,
                    Draws = status.Draws, Errors = status.Errors, EvalDone = status.EvalDone, EvalTotal = status.EvalTotal,
                    ScoreBefore = status.ScoreBefore, ScoreAfter = status.ScoreAfter, LastError = status.LastError,
                    Running = status.Running, Finished = status.Finished, Cancelled = status.Cancelled
                };
                foreach (var b in status.Blocks) s.Blocks.Add(b.Copy());
                if (block != null && block.Played + block.Errors > 0) s.Blocks.Add(block.Copy());
                double seconds = status.Running ? (DateTime.UtcNow - started).TotalSeconds : status.Seconds;
                s.Seconds = seconds;
                s.GamesPerSecond = seconds > 0.5 ? (trainingDone + status.EvalDone) / seconds : 0;
                return s;
            }
        }

        public bool Running { get { lock (gate) return status.Running; } }
        public bool Finished { get { lock (gate) return status.Finished; } }

        public void Start()
        {
            lock (gate)
            {
                if (status.Running || status.Finished) return;
                status.Running = true;
                started = DateTime.UtcNow;
            }
            thread = new Thread(Run);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;
            thread.Name = "Pokemon TCG training";
            thread.Start();
        }

        /// <summary>Runs the whole session on the calling thread (tests, editor menu).</summary>
        public void RunBlocking()
        {
            lock (gate)
            {
                if (status.Running || status.Finished) return;
                status.Running = true;
                started = DateTime.UtcNow;
            }
            Run();
        }

        public void Cancel()
        {
            cancel = true;
        }

        public void Join(int milliseconds)
        {
            var t = thread;
            if (t != null && t.IsAlive) t.Join(milliseconds);
        }

        private void SetPhase(string phase)
        {
            lock (gate) status.Phase = phase;
        }

        private void Run()
        {
            BrainBefore = brain.Serialize();
            var record = new SessionRecord
            {
                When = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                DeckA = DeckA,
                DeckB = DeckB,
                EvalGames = EvalGames,
                Style = StyleLabel
            };
            try
            {
                int evalSeed = seedBase + 500000;
                if (EvalGames > 0 && !cancel)
                {
                    SetPhase("Measuring the " + StyleLabel + " against the classic bot (before)");
                    double before = Evaluate(EvalGames, evalSeed);
                    lock (gate) status.ScoreBefore = before;
                    record.ScoreBefore = before;
                }

                SetPhase("Training the " + StyleLabel + ": " + DeckA + " vs " + DeckB);
                if ((Style.HasValue ? Style.Value : brain.Style) == AiStyle.Neural && brain.Policy.Trained) TrainNeural();
                else
                {
                    for (int g = 0; g < Games && !cancel; g++)
                    {
                        var gameStart = DateTime.UtcNow;
                        PlayTrainingGame(g);
                        if (SaveEvery > 0 && (g + 1) % SaveEvery == 0) SafeSave();
                        WorkThrottle.Rest(DateTime.UtcNow - gameStart, () => cancel); // v120: the battery saver
                    }
                }
                CloseBlock();

                if (EvalGames > 0 && !cancel)
                {
                    SetPhase("Measuring the " + StyleLabel + " against the classic bot (after)");
                    double after = Evaluate(EvalGames, evalSeed);
                    lock (gate) status.ScoreAfter = after;
                    record.ScoreAfter = after;
                }
            }
            catch (Exception e)
            {
                lock (gate) status.LastError = e.GetType().Name + ": " + e.Message;
            }
            finally
            {
                CloseBlock();
                lock (gate)
                {
                    record.Games = status.Done;
                    record.WinsA = status.WinsA;
                    record.WinsB = status.WinsB;
                }
                if (record.Games > 0) brain.AddSession(record);
                SafeSave();
                lock (gate)
                {
                    status.Running = false;
                    status.Finished = true;
                    status.Cancelled = cancel;
                    status.Seconds = (DateTime.UtcNow - started).TotalSeconds;
                    status.Phase = cancel ? "Stopped" : "Done";
                }
            }
        }

        private void CloseBlock()
        {
            lock (gate)
            {
                if (block != null && block.Played + block.Errors > 0) status.Blocks.Add(block);
                block = null;
            }
        }

        private void SafeSave()
        {
            try
            {
                brain.Save();
            }
            catch (Exception e)
            {
                lock (gate) status.LastError = "Couldn't save the AI: " + e.Message;
            }
        }

        private void PlayTrainingGame(int g)
        {
            int seed = seedBase + g;
            int seatA = g % 2;
            var agentA = new LearningAgent(brain, seed * 2 + 1, DeckA, DeckB) { Training = true, Style = Style };
            var agentB = new LearningAgent(brain, seed * 2 + 2, DeckB, DeckA) { Training = true, Style = Style };
            int winner = -2;
            int turns = 0;
            string error = null;
            try
            {
                var engine = new GameEngine(scripts, seatA == 0 ? (IPlayerAgent)agentA : agentB, seatA == 0 ? (IPlayerAgent)agentB : agentA);
                engine.CheckInvariants = false;
                engine.CardsPerPlayer = cardsA.Count == cardsB.Count ? cardsA.Count : 0;
                engine.Rules = Rules ?? GameRules.Default;
                engine.PrizeCount = PrizeCount;
                var result = engine.Run(seatA == 0 ? cardsA : cardsB, seatA == 0 ? cardsB : cardsA, seed);
                winner = result.Winner < 0 ? -1 : (result.Winner == seatA ? 0 : 1);
                turns = result.Turns;
                int explored = agentA.Trajectory.Count(d => d.Explored) + agentB.Trajectory.Count(d => d.Explored);
                int decisions = agentA.Trajectory.Count + agentB.Trajectory.Count;
                agentA.LearnFromGame(engine);
                agentB.LearnFromGame(engine);
                brain.IncrementCounters(1, 0, 0);
                lock (gate)
                {
                    EnsureBlock(g);
                    block.Explored += explored;
                    block.Decisions += decisions;
                }
            }
            catch (Exception e)
            {
                error = "game " + (g + 1) + " (seed " + seed + "): " + e.GetType().Name + ": " + e.Message;
            }

            lock (gate)
            {
                EnsureBlock(g);
                status.Done = g + 1;
                trainingDone = g + 1;
                if (error != null)
                {
                    status.Errors++;
                    status.LastError = error;
                    block.Errors++;
                }
                else if (winner == 0)
                {
                    status.WinsA++;
                    block.WinsA++;
                }
                else if (winner == 1)
                {
                    status.WinsB++;
                    block.WinsB++;
                }
                else
                {
                    status.Draws++;
                    block.Draws++;
                }
                if (error == null) block.TotalTurns += turns;
                if (g + 1 == block.To)
                {
                    status.Blocks.Add(block);
                    block = null;
                }
            }
        }

        /// <summary>
        /// Neural AI training: the policy network plays deck A against deck B by self-play (and now and then against its earlier
        /// versions and the classic bot) and learns by PPO after every round of games, like the Python bot.
        /// </summary>
        private void TrainNeural()
        {
            var decks = new List<KeyValuePair<string, List<CardDef>>>
            {
                new KeyValuePair<string, List<CardDef>>(DeckA, cardsA),
                new KeyValuePair<string, List<CardDef>>(DeckB == DeckA ? DeckB + " " : DeckB, cardsB)
            };
            var trainer = new PolicyTrainer(brain.Policy, scripts, decks, seedBase)
            {
                Matchup = true,
                Rules = Rules ?? GameRules.Default,
                PrizeCount = PrizeCount,
                GamesPerRound = Math.Max(8, Math.Min(64, Games)),
                GameBudget = Games
            };
            int played = 0;
            trainer.ShouldStop = () => cancel || Volatile.Read(ref played) >= Games;
            trainer.Abort = () => cancel; // Stop: leave the games in progress and don't learn from the unfinished batch
            trainer.GameFinished = game =>
            {
                int g = Interlocked.Increment(ref played) - 1;
                lock (gate)
                {
                    EnsureBlock(g);
                    status.Done = g + 1;
                    trainingDone = g + 1;
                    if (game.Error != null)
                    {
                        status.Errors++;
                        status.LastError = game.Error;
                        block.Errors++;
                    }
                    else if (game.WinnerDeck == DeckA)
                    {
                        status.WinsA++;
                        block.WinsA++;
                    }
                    else if (game.WinnerDeck == "")
                    {
                        status.Draws++;
                        block.Draws++;
                    }
                    else
                    {
                        status.WinsB++;
                        block.WinsB++;
                    }
                    if (game.Error == null) block.TotalTurns += game.Turns;
                    if (g + 1 == block.To)
                    {
                        status.Blocks.Add(block);
                        block = null;
                    }
                }
            };
            while (!cancel && Volatile.Read(ref played) < Games)
            {
                var round = trainer.Round();
                brain.IncrementCounters(round.Games, 0, 0);
                lock (gate)
                {
                    if (block != null) block.Decisions += round.Decisions;
                }
                SafeSave();
                if (round.Games == 0) break;
            }
        }

        /// <summary>Call inside the lock.</summary>
        private void EnsureBlock(int g)
        {
            if (block != null) return;
            int from = g / blockSize * blockSize;
            block = new TrainingBlock { From = from + 1, To = Math.Min(Games, from + blockSize) };
        }

        /// <summary>
        /// Win rate of the learning AI (no exploring) against the classic bot: half the games with each deck, seats
        /// alternating. The same seeds before and after training, so the difference comes from what it learned.
        /// </summary>
        private double Evaluate(int games, int seed0)
        {
            double points = 0;
            int played = 0;
            for (int k = 0; k < games && !cancel; k++)
            {
                int seed = seed0 + k;
                bool learningPlaysA = k % 2 == 0;
                int learningSeat = (k / 2) % 2;
                var learning = new LearningAgent(brain, seed * 2 + 1, learningPlaysA ? DeckA : DeckB, learningPlaysA ? DeckB : DeckA) { Training = false, Record = false, Style = Style };
                var classic = new HeuristicAgent(seed * 2 + 2);
                var learningCards = learningPlaysA ? cardsA : cardsB;
                var classicCards = learningPlaysA ? cardsB : cardsA;
                var evalStart = DateTime.UtcNow;
                try
                {
                    var engine = new GameEngine(scripts, learningSeat == 0 ? (IPlayerAgent)learning : classic, learningSeat == 0 ? (IPlayerAgent)classic : learning);
                    engine.CheckInvariants = false;
                    engine.CardsPerPlayer = cardsA.Count == cardsB.Count ? cardsA.Count : 0;
                    engine.Rules = Rules ?? GameRules.Default;
                    engine.PrizeCount = PrizeCount;
                    var result = engine.Run(learningSeat == 0 ? learningCards : classicCards, learningSeat == 0 ? classicCards : learningCards, seed);
                    if (result.Winner == learningSeat) points += 1;
                    else if (result.Winner < 0) points += 0.5;
                    played++;
                }
                catch (Exception e)
                {
                    lock (gate)
                    {
                        status.Errors++;
                        status.LastError = "measuring game " + (k + 1) + " (seed " + seed + "): " + e.GetType().Name + ": " + e.Message;
                    }
                }
                lock (gate) status.EvalDone++;
                WorkThrottle.Rest(DateTime.UtcNow - evalStart, () => cancel); // v120: the battery saver
            }
            return played == 0 ? -1 : points / played;
        }
    }
}
