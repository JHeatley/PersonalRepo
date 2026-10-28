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
    /// Trains the learning AI: it plays deck A against deck B, or all decks in random pairs (seats alternate, both sides share the brain and both
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

        /// <summary>
        /// Neural AI training: games played at once (0: half the CPU's threads) and the rest after each learning step, as a share
        /// of the time it took (the Train screen's Speed: Gentle 0.25, Fast none).
        /// </summary>
        public int NeuralThreads;
        public double NeuralRest = 0.25;
        /// <summary>Training plays at most this many games a second (0: as fast as it can).</summary>
        public double GamesPerSecond;
        /// <summary>Neural AI training learns on the GPU learner at this address ("127.0.0.1:5577"; null or "": on the CPU).</summary>
        public string GpuAddress;

        /// <summary>
        /// The decks a random side picks from, a new one every game (set before Start). With RandomA and RandomB both on (the
        /// default) it's all decks against all decks, as the Python bot trains, and DeckA and DeckB are only labels; with one of
        /// them off, that side keeps its deck every game against a random one (focused practice for that deck).
        /// </summary>
        public List<KeyValuePair<string, List<CardDef>>> DeckPool;
        public bool RandomA = true, RandomB = true;

        /// <summary>
        /// The decks the Neural AI's checks against its champion play (ChampionGate): all your legal decks, so the yardstick is
        /// the same whatever this session trains on. Null: the pool, or this session's decks.
        /// </summary>
        public List<KeyValuePair<string, List<CardDef>>> ChampionDecks;
        /// <summary>A check against the champion every this many training games (0: none), and at the end of a long enough session.</summary>
        public int ChampionEvery = DefaultChampionEvery;
        /// <summary>A session's last check needs at least this many games trained since the one before.</summary>
        public int ChampionMinGames = DefaultChampionMinGames;
        public const int DefaultChampionEvery = 10000, DefaultChampionMinGames = 2000;
        public int ChampionGames = ChampionGate.DefaultGames;
        /// <summary>What the last check against the champion found ("" before one).</summary>
        public string ChampionNote { get; private set; }

        private bool HasPool { get { return DeckPool != null && DeckPool.Count > 0; } }
        /// <summary>Both sides random: all decks against all decks.</summary>
        public bool AllDecksMode { get { return HasPool && RandomA && RandomB; } }
        /// <summary>One side keeps its deck, the other gets a random one each game.</summary>
        public bool FocusMode { get { return HasPool && RandomA != RandomB; } }
        /// <summary>In focus mode: the deck that plays every game (deck A or deck B).</summary>
        public string FocusDeck { get { return FocusMode ? (RandomB ? DeckA : DeckB) : null; } }

        /// <summary>What the games are: "Deck A vs Deck B", one deck against random decks, or all decks.</summary>
        public string MatchLabel
        {
            get
            {
                if (AllDecksMode) return "all " + DeckPool.Count + " decks, a random pair every game";
                if (FocusMode) return FocusDeck + " vs a random deck every game (of " + DeckPool.Count + ")";
                return DeckA + " vs " + DeckB;
            }
        }

        /// <summary>
        /// The two decks for the game with this seed: deck A and deck B, with a random deck from the pool on each random side
        /// (the same for the same seed).
        /// </summary>
        private void Pair(int seed, out string nameA, out List<CardDef> deckA, out string nameB, out List<CardDef> deckB)
        {
            nameA = DeckA;
            deckA = cardsA;
            nameB = DeckB;
            deckB = cardsB;
            if (!HasPool) return;
            var rng = new Rng(seed ^ 0x2C1B3C6D);
            var a = DeckPool[rng.Next(DeckPool.Count)];
            var b = DeckPool[rng.Next(DeckPool.Count)];
            if (RandomA)
            {
                nameA = a.Key;
                deckA = a.Value;
            }
            if (RandomB)
            {
                nameB = b.Key;
                deckB = b.Value;
            }
        }

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

                SetPhase("Training the " + StyleLabel + ": " + MatchLabel);
                if ((Style.HasValue ? Style.Value : brain.Style) == AiStyle.Neural && brain.Policy.Trained) TrainNeural();
                else
                {
                    var pace = new GamePace(GamesPerSecond);
                    for (int g = 0; g < Games && !cancel; g++)
                    {
                        pace.Wait(() => cancel);
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
            string nameA, nameB;
            List<CardDef> cardsA, cardsB;
            Pair(seed, out nameA, out cardsA, out nameB, out cardsB);
            var agentA = new LearningAgent(brain, seed * 2 + 1, nameA, nameB) { Training = true, Style = Style };
            var agentB = new LearningAgent(brain, seed * 2 + 2, nameB, nameA) { Training = true, Style = Style };
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
            List<KeyValuePair<string, List<CardDef>>> decks;
            if (AllDecksMode) decks = DeckPool;
            else if (FocusMode)
            {
                // The focused deck plays every game (it's the learner's), against a random deck from the pool.
                decks = DeckPool.ToList();
                if (!decks.Any(d => d.Key == FocusDeck))
                    decks.Add(new KeyValuePair<string, List<CardDef>>(FocusDeck, RandomB ? cardsA : cardsB));
            }
            else
                decks = new List<KeyValuePair<string, List<CardDef>>>
                {
                    new KeyValuePair<string, List<CardDef>>(DeckA, cardsA),
                    new KeyValuePair<string, List<CardDef>>(DeckB == DeckA ? DeckB + " " : DeckB, cardsB)
                };
            var trainer = new PolicyTrainer(brain.Policy, scripts, decks, seedBase)
            {
                // Deck A against deck B; one deck against a random one (focus); or all decks, a random deck for each side.
                Matchup = !HasPool || !(RandomA || RandomB),
                FocusDeck = FocusDeck,
                Rules = Rules ?? GameRules.Default,
                PrizeCount = PrizeCount,
                GamesPerRound = Math.Max(8, Math.Min(64, Games)),
                // Leave two cores for the app itself (the screen and the garbage collector), so it stays responsive.
                // Half the cores, a short rest after each learning step and the memory tidied between phases: a long session in the
                // app shouldn't push the PC as hard as a stress test does. (The command-line trainer uses every core.)
                Threads = NeuralThreads > 0 ? NeuralThreads : Math.Max(1, Environment.ProcessorCount / 2),
                RestShare = Math.Max(0, NeuralRest),
                CollectGarbage = true,
                GameBudget = Games
            };
            string gpuHost;
            int gpuPort;
            if (!string.IsNullOrEmpty(GpuAddress) && GpuLearner.TryParse(GpuAddress, out gpuHost, out gpuPort)) trainer.Gpu = new GpuLearner(gpuHost, gpuPort);
            if (GamesPerSecond > 0) trainer.Pace = new GamePace(GamesPerSecond);
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
                    else if (AllDecksMode ? game.Outcome > 0 : FocusMode ? (RandomB ? game.Outcome > 0 : game.Outcome < 0) : game.WinnerDeck == DeckA)
                    {
                        status.WinsA++;
                        block.WinsA++;
                    }
                    else if (HasPool && (RandomA || RandomB) ? game.Outcome == 0 : game.WinnerDeck == "")
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
                    block.Decisions += game.Decisions;
                    block.Explored += game.Explored;
                    if (g + 1 == block.To)
                    {
                        status.Blocks.Add(block);
                        block = null;
                    }
                }
                if (game.Error == null)
                {
                    // The Train screen's matchup list: the network's deck, and the other deck too when the network played both
                    // seats (self-play). The names are trimmed: a mirror's second deck has a space added to tell them apart.
                    brain.RecordMatchup(game.LearnerDeck.Trim(), game.OpponentDeck.Trim(), game.Outcome);
                    if (game.SelfPlay) brain.RecordMatchup(game.OpponentDeck.Trim(), game.LearnerDeck.Trim(), -game.Outcome);
                    // For the deck advisor: which cards showed up in wins and losses, and how sure it was with each deck.
                    foreach (var seat in game.Seats)
                        brain.RecordDeckGame(seat.Deck, seat.Cards, seat.Drawn, seat.Played, seat.Outcome, seat.Decisions, seat.Surprise);
                }
            };
            try
            {
                TrainNeuralRounds(trainer, () => Volatile.Read(ref played), decks);
            }
            finally
            {
                trainer.Dispose();
            }
        }

        private void TrainNeuralRounds(PolicyTrainer trainer, Func<int> playedSoFar, List<KeyValuePair<string, List<CardDef>>> sessionDecks)
        {
            int nextChampion = ChampionEvery, lastChampion = 0;
            var championDecks = ChampionDecks != null && ChampionDecks.Count > 0 ? ChampionDecks : HasPool ? DeckPool : sessionDecks;
            // The first session with the yardstick: the network as it is now becomes the first champion (nothing to play).
            if (ChampionEvery > 0 && brain.Champion == null)
            {
                var first = ChampionGate.Run(brain, scripts, championDecks, ChampionGames, trainer.Threads, () => cancel);
                if (first != null) ChampionNote = "champion: " + ChampionGate.Describe(first);
            }
            long skippedBefore = brain.Policy.SkippedSteps;
            // Progress, shown on the status line: sequencing mistakes per game (they should fall), and, when measuring is on,
            // a check against the classic bot every fifth of the session on the same games each time (it should rise).
            string basePhase = "Training the " + StyleLabel + ": " + MatchLabel;
            double firstMistakes = -1, recentMistakes = -1;
            var checks = new List<string>();
            int checkEvery = Math.Max(250, Games / 5), nextCheck = checkEvery;
            int checkSeed = seedBase + 700000;
            while (!cancel && playedSoFar() < Games)
            {
                var round = trainer.Round();
                brain.IncrementCounters(round.Games, 0, 0);
                if (round.LearnerSeats > 0)
                {
                    double m = round.Mistakes / (double)round.LearnerSeats;
                    if (firstMistakes < 0) firstMistakes = m;
                    recentMistakes = recentMistakes < 0 ? m : 0.7 * recentMistakes + 0.3 * m;
                }
                int done = playedSoFar();
                if (EvalGames > 0 && done >= nextCheck && done < Games && !cancel)
                {
                    while (nextCheck <= done) nextCheck += checkEvery;
                    SetPhase(basePhase + " · checking against the classic bot (100 games)...");
                    int w, d, l;
                    double wr = trainer.Evaluate(seed => new HeuristicAgent(seed), 100, checkSeed, out w, out d, out l);
                    if (wr >= 0) checks.Add((wr * 100).ToString("0", CultureInfo.InvariantCulture) + "% at " + done.ToString("N0", CultureInfo.InvariantCulture));
                }
                string progress = basePhase;
                if (recentMistakes >= 0)
                    progress += " · sequencing mistakes per game " + recentMistakes.ToString("0.0", CultureInfo.InvariantCulture)
                                + " (first round " + firstMistakes.ToString("0.0", CultureInfo.InvariantCulture) + "; lower is better)";
                if (round.LearnedOn.Length > 0) progress += " · learning on the GPU (" + round.LearnedOn + ")";
                if (checks.Count > 0) progress += " · vs the classic bot: " + string.Join(" → ", checks.ToArray()) + " games";
                else if (EvalGames == 0) progress += " · tick \"Measure it against the classic bot\" to see it checked as it trains";
                if (!string.IsNullOrEmpty(ChampionNote)) progress += " · " + ChampionNote;
                SetPhase(progress);
                if (ChampionEvery > 0 && done >= nextChampion && done < Games && !cancel)
                {
                    while (nextChampion <= done) nextChampion += ChampionEvery;
                    CheckChampion(basePhase, trainer.Threads, championDecks);
                    lastChampion = done;
                }
                long skipped = brain.Policy.SkippedSteps - skippedBefore;
                if (skipped > 0)
                    lock (gate) status.LastError = skipped + " learning step" + (skipped == 1 ? " was" : "s were") + " skipped (numbers out of range); the network is unharmed.";
                if (!string.IsNullOrEmpty(round.LastError) && round.LastError.StartsWith("GPU learner", StringComparison.Ordinal))
                    lock (gate) status.LastError = round.LastError;
                SafeSave();
                if (round.Games == 0) break;
            }
            // The end of a long enough session: the network against the champion, so the screen shows where it stands.
            if (!cancel && ChampionEvery > 0 && playedSoFar() - lastChampion >= ChampionMinGames)
                CheckChampion(basePhase, trainer.Threads, championDecks);
        }

        /// <summary>The network against its champion (the yardstick, ChampionGate), promoted if it won clearly.</summary>
        private void CheckChampion(string basePhase, int threads, List<KeyValuePair<string, List<CardDef>>> decks)
        {
            SetPhase(basePhase + " · checking against the champion (" + ChampionGames + " games)...");
            var check = ChampionGate.Run(brain, scripts, decks, ChampionGames, threads, () => cancel);
            if (check == null) return;
            ChampionNote = "champion: " + ChampionGate.Describe(check);
            SetPhase(basePhase + " · " + ChampionNote);
            SafeSave();
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
                // Focus mode measures the focused deck (against random decks); otherwise half the games with each side's deck.
                bool learningPlaysA = FocusMode ? RandomB : k % 2 == 0;
                int learningSeat = FocusMode ? k % 2 : (k / 2) % 2;
                string nameA, nameB;
                List<CardDef> cardsA, cardsB;
                Pair(seed, out nameA, out cardsA, out nameB, out cardsB);
                var learning = new LearningAgent(brain, seed * 2 + 1, learningPlaysA ? nameA : nameB, learningPlaysA ? nameB : nameA) { Training = false, Record = false, Style = Style };
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
