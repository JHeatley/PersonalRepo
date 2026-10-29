using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// The learning AI in the app:
    ///  - its brain lives in brain.txt in the AI folder (SavePaths: persistentDataPath/AI unless you chose another
    ///    folder on the Save Folders screen) and is loaded at start;
    ///  - the Train screen plays any two decks (built-in or saved) against each other for 100 to 25,000 games while it
    ///    learns, and can measure itself against the classic bot before and after;
    ///  - in normal games the opponent is the learning AI (or the classic bot, from the menu); after each game it
    ///    learns from its own play and from yours, then the Areas of Improvement review runs (GameBootstrapReview.cs).
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string PrefOpponentAi = "PokemonTCG.OpponentAI";
        /// <summary>Learning AI 0, Neural AI 1, Hybrid AI 2 (v74; AiStyle).</summary>
        private const string PrefOpponentStyle = "PokemonTCG.OpponentStyle";
        /// <summary>The Opponent switch (v74): the three ways the learning AI can play, then the classic bot.</summary>
        private static readonly string[] OpponentChoices = { "Learning AI", "Neural AI", "Hybrid AI", "Classic bot" };
        /// <summary>How the AI plays in training (v78): Learning AI 0, Neural AI 1, Hybrid AI 2 (AiStyle), apart from the Opponent switch.</summary>
        private const string PrefTrainStyle = "PokemonTCG.TrainStyle";
        private const string PrefTrainA = "PokemonTCG.TrainDeckA";
        private const string PrefTrainB = "PokemonTCG.TrainDeckB";
        private const string PrefTrainGames = "PokemonTCG.TrainGames";
        private const string PrefTrainAll = "PokemonTCG.TrainAllDecks";
        private const string PrefTrainRandomA = "PokemonTCG.TrainRandomA";
        /// <summary>Speed for Neural AI training: Gentle (half the threads, rests) or Fast (every usable thread but two, no rests).</summary>
        private const string PrefTrainFast = "PokemonTCG.TrainFast";
        /// <summary>Neural AI training learns on the GPU learner (GpuLearner/learner.py), and where it listens.</summary>
        private const string PrefTrainGpu = "PokemonTCG.TrainGpu";
        private const string PrefTrainLookAhead = "PokemonTCG.TrainLookAhead";
        private const string PrefTrainGpuMoves = "PokemonTCG.TrainGpuMoves";
        private const string PrefTrainExploiters = "PokemonTCG.TrainExploiters";
        /// <summary>Train exploiters for the league (TrainingSession.Exploiters).</summary>
        private bool trainExploiters = true;
        /// <summary>Choose the Neural AI's moves on the GPU learner too (many games at once), with "Learn on the GPU".</summary>
        private bool trainGpuMoves;
        private static readonly string[] TrainLookAheadNames = { "Off", "10% of games", "25% of games" };
        private static readonly double[] TrainLookAheadShares = { 0, 0.1, 0.25 };
        /// <summary>Learn from the look-ahead: 0 off, 1 in 10% of the Neural AI's training games, 2 in 25%.</summary>
        private int trainLookAhead;
        private const string PrefTrainGpuAddress = "PokemonTCG.TrainGpuAddress";
        private const string PrefTrainRandomB = "PokemonTCG.TrainRandomB";
        /// <summary>The most games one session can be set to play (typed in).</summary>
        private const int MaxTrainGames = 100000000;
        /// <summary>Keep the app off some CPU threads (a faulty core), and which ones (CpuThreads).</summary>
        private const string PrefSkipCpu = "PokemonTCG.SkipCpuThreads";
        private const string PrefLookAhead = "PokemonTCG.LookAhead";
        private static readonly string[] LookAheadNames = { "Off", "Quick", "Deep" };
        /// <summary>The Neural AI's look-ahead in games against you: 0 off, 1 quick, 2 deep (PolicySearch).</summary>
        private int lookAhead;
        private const string PrefSkipCpuList = "PokemonTCG.SkipCpuThreadList";
        private static readonly int[] TrainCounts = { 100, 500, 1000, 5000, 10000, 15000, 20000, 25000 };
        private static readonly string[] TrainCountLabels = { "100", "500", "1,000", "5,000", "10,000", "15,000", "20,000", "25,000" };
        /// <summary>The game counts sit in rows of this many buttons (8 in one row don't fit the column; v56).</summary>
        private const int TrainCountsPerRow = 4;
        /// <summary>Games per measurement against the classic bot (half with each deck), before and after training.</summary>
        private const int MeasureGames = 200;

        private LearningBrain brain;
        private string brainError;
        private bool opponentLearning = true;
        private AiStyle opponentStyle = AiStyle.Hybrid;

        // Train screen
        private TrainingSession training;
        private TrainingStatus trainView;
        private int trainA = -1, trainB = -1;
        private int trainGames = 1000;
        private string trainGamesText = "1,000";
        /// <summary>
        /// A random deck every game on side A or B instead of the deck picked: one side random is focused practice for the other
        /// side's deck; both random is all decks against all decks.
        /// </summary>
        private bool trainRandomA, trainRandomB;
        private bool trainFast;
        private bool trainGpu;
        /// <summary>Games a second at most, as typed ("" or 0: as fast as it can).</summary>
        private string trainRateText = "";
        private const string PrefTrainRate = "PokemonTCG.TrainGamesPerSecond";
        private string trainGpuAddress = GpuLearner.DefaultAddress;
        private bool TrainAll { get { return trainRandomA && trainRandomB; } }
        private bool TrainFocus { get { return trainRandomA != trainRandomB; } }
        /// <summary>The deck that plays every game in focus mode (index into decks), or -1.</summary>
        private int TrainFocusDeck { get { return !TrainFocus ? -1 : trainRandomA ? trainB : trainA; } }
        private bool trainMeasure = true;
        private AiStyle trainStyle = AiStyle.Hybrid;
        private bool confirmReset;
        private bool sessionUndone;
        private Vector2 trainScrollA, trainScrollB, trainScrollInfo, trainScrollMatchups, trainScrollLessons, trainScrollDecks, trainScrollSettings;
        private GUIStyle listRowStyle;
        private float listRowHeight = 20f;
        private float nextBrainRefresh;
        private BrainSnapshot brainView;
        private Texture2D chartBar, chartLine, chartBack;
        private GUIStyle chartLabel;
        private string trainMessage;
        private bool skipCpu;
        private string skipCpuText = "2, 3";
        private string skipCpuMessage;

        // The game being played
        private LearningAgent learningBot;
        private DecisionLog humanLog;
        private EventCollector eventLog;
        private bool gameEndHandled;
        private volatile string learnStatus;

        /// <summary>What the Train screen shows about the brain (copied once a second, not every frame).</summary>
        private sealed class BrainSnapshot
        {
            public int Weights;
            public int GamesTrained;
            public int GamesVsYou;
            public int DecisionsStudied;
            public int Feedback;
            public int LogsStudied;
            public int LogDecisions;
            public long NetSteps;
            public double NetLoss;
            public string NetOrigin;
            public int NetBlocks;
            public AiStyle Style;
            public List<MatchupStats> Matchups = new List<MatchupStats>();
            public List<DeckSkill> Decks = new List<DeckSkill>();
            public List<KeyValuePair<string, double>> Lessons = new List<KeyValuePair<string, double>>();
            public List<SessionRecord> History = new List<SessionRecord>();
            public int ChampionVersion;
            /// <summary>The checks against the champion, newest first.</summary>
            public List<ChampionCheck> ChampionChecks = new List<ChampionCheck>();
            public List<LeagueMember> League = new List<LeagueMember>();
            public double[] LeagueShares = new double[0];
            /// <summary>What happened in the league, newest first.</summary>
            public List<string> LeagueEvents = new List<string>();
            private string[] lessonText;

            /// <summary>The league's members, then its history.</summary>
            public string LeagueRow(int k)
            {
                if (k < League.Count)
                {
                    var m = League[k];
                    return "   " + m.Name + " · joined at " + m.AddedAt.ToString("N0") + " games · "
                           + (m.Games < 1 ? "not played yet" : "it scores " + Math.Round(m.WinRate * 100) + "% against it lately")
                           + " · picked for " + Math.Round(LeagueShares[k] * 100) + "% of the league's games";
                }
                return "   " + LeagueEvents[k - League.Count];
            }

            public string ChampionRow(int k)
            {
                return "   " + ChampionChecks[k].When + "  " + ChampionGate.Describe(ChampionChecks[k]);
            }

            public string DeckRow(int k)
            {
                var d = Decks[k];
                return "   " + d.Deck + ":  " + d.Level + " (uncertainty " + d.Uncertainty.ToString("0.00") + ") · " + d.Games.ToString("N0") + " games, won " + Pct(d.Wins, Math.Max(1, d.Games));
            }

            public string MatchupRow(int k)
            {
                var m = Matchups[k];
                return "   " + m.Deck + " vs " + m.Opponent + ":  " + m.Games.ToString("N0") + " games, " + Pct(m.Wins, Math.Max(1, m.Games));
            }

            /// <summary>Lesson k as a line of text, worked out the first time it scrolls into view.</summary>
            public string LessonRow(int k)
            {
                if (lessonText == null || lessonText.Length != Lessons.Count) lessonText = new string[Lessons.Count];
                if (lessonText[k] == null)
                {
                    var kv = Lessons[k];
                    lessonText[k] = "   " + (k + 1).ToString("N0") + ".   " + (kv.Value >= 0 ? "+" : "−") + Math.Abs(kv.Value).ToString("0.00") + "   " + Features.Describe(kv.Key);
                }
                return lessonText[k];
            }
        }

        // Read on the main thread only (SavePaths uses PlayerPrefs); background work takes a copy first.
        private static string AiFolder { get { return SavePaths.BrainFolder; } }
        private static string BrainPath { get { return SavePaths.BrainPath; } }
        private static string FeedbackPath { get { return SavePaths.FeedbackPath; } }

        private void LoadBrain()
        {
            opponentLearning = PlayerPrefs.GetInt(PrefOpponentAi, 1) == 1;
            opponentStyle = (AiStyle)Mathf.Clamp(PlayerPrefs.GetInt(PrefOpponentStyle, (int)AiStyle.Hybrid), 0, 2);
            brainError = null;
            try
            {
                UnityCardData.LoadPolicyStart();
                brain = LearningBrain.Load(BrainPath);
                brainError = brain.PolicyLoadError; // v171: its network file couldn't be read (it isn't saved over)
            }
            catch (Exception e)
            {
                brainError = "Couldn't read the AI's file (" + e.Message + "); starting fresh.";
                brain = new LearningBrain { FilePath = BrainPath };
            }
            brain.Style = opponentStyle;
            brainView = null;
            nextBrainRefresh = 0f;
            // A Regional Sim event in progress plays with (and teaches) the brain from the new folder from now on.
            if (regional != null) regional.Brain = brain;
        }

        private void SaveBrainInBackground()
        {
            var b = brain;
            if (b == null) return;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try { b.Save(); }
                catch (Exception e) { learnStatus = "Couldn't save the AI: " + e.Message; }
            });
        }

        /// <summary>At start: keeps the app off the CPU threads chosen on the Train screen, if that's on.</summary>
        private void LoadSkipCpu()
        {
            skipCpu = PlayerPrefs.GetInt(PrefSkipCpu, 0) == 1;
            skipCpuText = PlayerPrefs.GetString(PrefSkipCpuList, "2, 3") ?? "";
            if (skipCpu && CpuThreads.Supported) ApplySkipCpu();
        }

        private void SetSkipCpu(bool on, string text)
        {
            bool was = skipCpu;
            skipCpu = on;
            skipCpuText = text ?? "";
            PlayerPrefs.SetInt(PrefSkipCpu, on ? 1 : 0);
            PlayerPrefs.SetString(PrefSkipCpuList, skipCpuText);
            if (on || was) ApplySkipCpu();
        }

        private void ApplySkipCpu()
        {
            skipCpuMessage = CpuThreads.Apply(skipCpu ? CpuThreads.Parse(skipCpuText) : new List<int>());
        }

        /// <summary>
        /// The "Don't use CPU threads" switch as one row (the menu and the Regional Sim; the Train screen has its own layout).
        /// Every place shows the same setting.
        /// </summary>
        private void DrawSkipCpuRow(UiStyles ui, List<Action> actions, float labelWidth)
        {
            if (!CpuThreads.Supported) return;
            GUILayout.BeginHorizontal();
            GUILayout.Label("CPU threads", GUILayout.Width(labelWidth));
            bool newSkip = GUILayout.Toggle(skipCpu, " Don't use", GUILayout.Width(100));
            string newSkipText = GUILayout.TextField(skipCpuText, GUILayout.Width(90));
            GUILayout.Label("  " + (skipCpu && skipCpuMessage != null ? skipCpuMessage : "Off: every thread is used.") + " For a faulty CPU core.", Hint(ui));
            GUILayout.EndHorizontal();
            if (newSkip != skipCpu || newSkipText != skipCpuText) actions.Add(() => SetSkipCpu(newSkip, newSkipText));
        }

        private void SetOpponentLearning(bool on)
        {
            opponentLearning = on;
            PlayerPrefs.SetInt(PrefOpponentAi, on ? 1 : 0);
        }

        /// <summary>Position of the Opponent switch: the learning AI's style (0-2), or 3 for the classic bot.</summary>
        private int OpponentPick { get { return opponentLearning ? (int)opponentStyle : 3; } }

        /// <summary>
        /// The Opponent switch (v74). The style is the brain's: training, the Regional Sim and the measuring games play the same
        /// way as your opponent. Every style learns from every game (the feature weights and the network both).
        /// </summary>
        private void SetOpponentPick(int pick)
        {
            if (pick >= 3)
            {
                SetOpponentLearning(false);
                return;
            }
            SetOpponentLearning(true);
            opponentStyle = (AiStyle)Mathf.Clamp(pick, 0, 2);
            PlayerPrefs.SetInt(PrefOpponentStyle, (int)opponentStyle);
            if (brain != null) brain.Style = opponentStyle;
        }

        private static string StyleName(AiStyle style)
        {
            return OpponentChoices[(int)style];
        }

        /// <summary>What each training style means (v78).</summary>
        private static string TrainStyleHint(AiStyle style)
        {
            string how = style == AiStyle.Learning
                ? "The training games are played with the feature weights only."
                : style == AiStyle.Neural
                    ? "The training games are played with the neural network only, and the network learns from them by PPO self-play (it starts from the Python bot's training)."
                    : "The training games are played with the weights and the network added together.";
            return how + (style == AiStyle.Neural ? "" : " The feature weights learn from every game.") + " The measuring games play the same way. "
                   + "Your opponent's style is the Opponent switch on the menu.";
        }

        /// <summary>
        /// The bot for a game: the learning AI (no exploring) or the classic bot. forceLearning: always the learning AI
        /// (your Regional Sim opponents, v57), whatever the menu says.
        /// </summary>
        private IPlayerAgent MakeBot(int seed, string myDeckName, string oppDeckName, bool learnFromGame, bool forceLearning = false, AiStyle? style = null)
        {
            if ((!opponentLearning && !forceLearning) || brain == null) return new HeuristicAgent(seed);
            // style: the Regional Sim's own choice (v117); null plays the Opponent switch's style.
            var agent = new LearningAgent(brain, seed, myDeckName, oppDeckName) { Training = false, Record = learnFromGame, Style = style };
            if (learnFromGame) learningBot = agent;
            if (lookAhead > 0 && agent.Neural != null) agent.Neural.Search = MakeSearch();
            return agent;
        }

        /// <summary>The look-ahead the menu asks for, guessing your hidden cards from the decks in this app (legal ones).</summary>
        private PolicySearch MakeSearch()
        {
            int threads = Math.Max(1, Environment.ProcessorCount - 2 - (skipCpu ? CpuThreads.Parse(skipCpuText).Count : 0));
            var search = new PolicySearch(brain.Policy, lookAhead >= 2 ? SearchSettings.Deep(threads) : SearchSettings.Quick(threads));
            for (int i = 0; i < decks.Count; i++)
            {
                if (deckProblems[i].Count > 0) continue;
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, decks[i], problems);
                if (problems.Count == 0 && cards.Count > 0) search.KnownDecks.Add(cards);
            }
            return search;
        }

        /// <summary>The menu's Look-ahead row (Neural AI only).</summary>
        private void DrawLookAheadRow(UiStyles ui)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Look-ahead", GUILayout.Width(80));
            int pick = GUILayout.Toolbar(lookAhead, LookAheadNames, GUILayout.Width(240));
            GUILayout.Label("  Neural AI only: it plays its best few moves out a turn ahead (Deep: two) on guesses at your hidden cards, "
                            + "never your real ones. Quick: under a second a move; Deep: a few seconds.", Hint(ui));
            GUILayout.EndHorizontal();
            if (pick != lookAhead)
            {
                lookAhead = pick;
                PlayerPrefs.SetInt(PrefLookAhead, pick);
                PlayerPrefs.Save();
            }
        }

        /// <summary>One line for the menu: what the opponent is and how much it has learned.</summary>
        private string BrainLine()
        {
            if (!opponentLearning) return "The classic bot plays the same way every game.";
            if (brain == null) return "";
            string line = StyleName(opponentStyle) + ": trained on " + brain.GamesTrained.ToString("N0") + " games, learned from " + brain.GamesVsYou + " of yours";
            if (opponentStyle != AiStyle.Learning)
            {
                long steps = brain.Policy.Steps;
                string origin = brain.Policy.Origin;
                line += ", network " + (string.IsNullOrEmpty(origin) ? "" : "from the " + origin + ", ") + steps.ToString("N0") + " learning steps here";
            }
            line += ".";
            var st = trainView;
            if (st != null && st.Running) line += "  Training now: " + st.Done + " / " + st.Games + ".";
            return line;
        }

        // ------------------------------------------------------------------ After a game

        /// <summary>The game is over: the AI learns from it, then the review runs. All on a background thread.</summary>
        private void OnGameFinished()
        {
            if (host == null || host.Engine == null || host.Engine.State == null || !host.Engine.State.IsOver) return;
            if (viewerSeat < 0 || humanLog == null) return; // two bots: nothing to learn from you or to review
            var engine = host.Engine;
            var state = engine.State;
            int seat = viewerSeat;
            var bot = learningBot;
            var decisions = humanLog.Decisions();
            var events = eventLog != null ? eventLog.Entries() : new List<EventCollector.Entry>();
            string yourDeck = deckNames[seat] ?? "";
            string theirDeck = deckNames[1 - seat] ?? "";
            var b = brain;
            var a = new GameAnalyzer(StandardCards.CreateRegistry(), b, decisions, events, seat, yourDeck, theirDeck, state, engine.PrizeCount, engine.Rules)
            {
                YourDeckList = engine.StartingDeck(seat) != null ? engine.StartingDeck(seat).ToList() : null,
                TheirDeckList = engine.StartingDeck(1 - seat) != null ? engine.StartingDeck(1 - seat).ToList() : null
            };
            StartReview(a, yourDeck, theirDeck);
            BeginRatedGame(state, seat, yourDeck, theirDeck); // v157: after StartReview (it rates any earlier game left waiting)
            learnStatus = b != null && bot != null ? "The AI is learning from this game..." : null;
            learningFromGame = b != null && bot != null;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    if (b != null && bot != null)
                    {
                        bot.LearnFromGame(engine);
                        double weight = state.Winner == seat ? 1.0 : state.Winner < 0 ? 0.75 : 0.5;
                        int studied = LearningAgent.LearnFromPlayer(b, decisions, yourDeck, theirDeck, weight);
                        b.IncrementCounters(0, 1, studied);
                        b.Save();
                        learnStatus = "The AI learned from this game (" + studied + " of your decisions studied).";
                    }
                }
                catch (Exception e)
                {
                    learnStatus = "The AI couldn't learn from this game: " + e.Message;
                }
                finally
                {
                    learningFromGame = false;
                }
                a.Run();
            });
        }

        // ------------------------------------------------------------------ Train screen

        private void OpenTrainScreen()
        {
            RefreshDecks();
            trainA = IndexOfDeck(PlayerPrefs.GetString(PrefTrainA, ""), myDeck);
            trainB = IndexOfDeck(PlayerPrefs.GetString(PrefTrainB, ""), oppDeck);
            // The style training plays (v78): its own choice, starting from the Opponent switch's until you pick one.
            trainStyle = (AiStyle)Mathf.Clamp(PlayerPrefs.GetInt(PrefTrainStyle, (int)opponentStyle), 0, 2);
            trainGames = Mathf.Clamp(PlayerPrefs.GetInt(PrefTrainGames, 1000), 1, MaxTrainGames);
            trainGamesText = trainGames.ToString("N0");
            bool all = PlayerPrefs.GetInt(PrefTrainAll, 0) == 1; // the earlier All decks switch
            trainRandomA = PlayerPrefs.GetInt(PrefTrainRandomA, all ? 1 : 0) == 1;
            trainFast = PlayerPrefs.GetInt(PrefTrainFast, 0) == 1;
            trainGpu = PlayerPrefs.GetInt(PrefTrainGpu, 0) == 1;
            trainLookAhead = Mathf.Clamp(PlayerPrefs.GetInt(PrefTrainLookAhead, 0), 0, TrainLookAheadNames.Length - 1);
            trainGpuMoves = PlayerPrefs.GetInt(PrefTrainGpuMoves, 0) == 1;
            trainExploiters = PlayerPrefs.GetInt(PrefTrainExploiters, 1) == 1;
            trainRateText = PlayerPrefs.GetString(PrefTrainRate, "") ?? "";
            trainGpuAddress = PlayerPrefs.GetString(PrefTrainGpuAddress, GpuLearner.DefaultAddress) ?? GpuLearner.DefaultAddress;
            trainRandomB = PlayerPrefs.GetInt(PrefTrainRandomB, all ? 1 : 0) == 1;
            confirmReset = false;
            nextBrainRefresh = 0f;
            mode = Mode.Train;
        }

        /// <summary>Called from Update: copies everything the Train screen and menu show, once per frame (IMGUI draws in several passes).</summary>
        private void UpdateTraining()
        {
            if (training != null) trainView = training.Status();
            UpdateBackgroundTraining();
            if (mode == Mode.Train && brain != null && Time.unscaledTime >= nextBrainRefresh)
            {
                nextBrainRefresh = Time.unscaledTime + 1f;
                brainView = new BrainSnapshot
                {
                    Weights = brain.FeatureCount,
                    GamesTrained = brain.GamesTrained,
                    GamesVsYou = brain.GamesVsYou,
                    DecisionsStudied = brain.YourDecisionsStudied,
                    Feedback = brain.FeedbackGiven,
                    LogsStudied = brain.LogsStudied,
                    LogDecisions = brain.LogDecisionsStudied,
                    NetSteps = brain.Policy.Steps,
                    NetLoss = brain.Policy.AverageLoss,
                    NetOrigin = brain.Policy.Origin,
                    NetBlocks = brain.Policy.Blocks,
                    Style = brain.Style,
                    Matchups = brain.Matchups(),
                    Decks = brain.DeckSkills(),
                    Lessons = brain.StrongestLessons(0, 5), // all of them; the Train screen shows them in a scrolling list
                    History = brain.History(),
                    ChampionVersion = brain.ChampionVersion,
                    ChampionChecks = Enumerable.Reverse(brain.ChampionChecks).ToList(),
                    League = brain.League.Members(),
                    LeagueEvents = Enumerable.Reverse(brain.League.Events()).ToList()
                };
                brainView.LeagueShares = League.PickShares(brainView.League);
            }
        }

        /// <summary>The decks the Neural AI's checks against its champion play: every legal deck (All decks' list), the same whatever is trained.</summary>
        private List<KeyValuePair<string, List<CardDef>>> ChampionDeckPool()
        {
            var pool = new List<KeyValuePair<string, List<CardDef>>>();
            foreach (int i in AllTrainDecks())
            {
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, decks[i], problems);
                if (problems.Count == 0 && cards.Count > 0) pool.Add(new KeyValuePair<string, List<CardDef>>(decks[i].Name, cards));
            }
            return pool;
        }

        /// <summary>The decks All decks mode trains with: every legal deck, but not decks for a custom format (other rules and Prize cards).</summary>
        private List<int> AllTrainDecks()
        {
            var list = new List<int>();
            for (int i = 0; i < decks.Count; i++)
                if (deckProblems[i].Count == 0 && !decks[i].IsCustomFormat) list.Add(i);
            return list;
        }

        /// <summary>
        /// The random side's decks in focus mode: like All decks (every legal deck, no custom formats), but for a deck of a custom
        /// format, the legal decks of that format (they share its rules and Prize cards).
        /// </summary>
        private List<int> TrainPoolFor(int focus)
        {
            if (focus < 0 || focus >= decks.Count || !decks[focus].IsCustomFormat) return AllTrainDecks();
            var list = new List<int>();
            for (int i = 0; i < decks.Count; i++)
                if (deckProblems[i].Count == 0 && decks[i].FormatName.Equals(decks[focus].FormatName, StringComparison.OrdinalIgnoreCase)) list.Add(i);
            return list;
        }

        /// <summary>Fast speed's games at once: the CPU threads the app may use (without the ones it's told to skip), less two for the screen.</summary>
        private int FastThreads
        {
            get
            {
                int usable = Environment.ProcessorCount;
                if (skipCpu && CpuThreads.Supported) usable -= CpuThreads.Parse(skipCpuText).Count(n => n < Environment.ProcessorCount);
                return Math.Max(1, usable - 2);
            }
        }

        private void ApplyTrainSpeed(TrainingSession session)
        {
            session.NeuralThreads = trainFast ? FastThreads : 0;
            session.NeuralRest = trainFast ? 0 : 0.25;
            session.GpuAddress = trainGpu ? trainGpuAddress : null;
            session.GamesPerSecond = TrainRate;
            session.LearnFromLookAhead = TrainLookAheadShares[trainLookAhead];
            session.GpuMoves = trainGpu && trainGpuMoves;
            session.Exploiters = trainExploiters;
        }

        /// <summary>The typed games-a-second limit (0: none).</summary>
        private double TrainRate
        {
            get
            {
                double r;
                return double.TryParse((trainRateText ?? "").Trim().Replace(',', '.'), System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out r) && r > 0 ? Math.Min(r, 1000) : 0;
            }
        }

        private void SetTrainRandom(bool a, bool b)
        {
            trainRandomA = a;
            trainRandomB = b;
            PlayerPrefs.SetInt(PrefTrainRandomA, a ? 1 : 0);
            PlayerPrefs.SetInt(PrefTrainRandomB, b ? 1 : 0);
        }

        private void SetTrainGames(int games)
        {
            trainGames = Mathf.Clamp(games, 1, MaxTrainGames);
            trainGamesText = trainGames.ToString("N0");
            PlayerPrefs.SetInt(PrefTrainGames, trainGames);
        }

        /// <summary>The number typed in the games box (commas and spaces allowed); up to MaxTrainGames.</summary>
        private void TypeTrainGames(string text)
        {
            trainGamesText = text ?? "";
            var digits = new string(trainGamesText.Where(char.IsDigit).ToArray());
            if (digits.Length == 0) return;
            long n;
            if (!long.TryParse(digits.Length > 9 ? "9999999999" : digits, out n) || n < 1) return;
            trainGames = (int)Math.Min(MaxTrainGames, n);
            PlayerPrefs.SetInt(PrefTrainGames, trainGames);
        }

        private bool TrainDecksReady
        {
            get
            {
                if (TrainAll) return AllTrainDecks().Count > 0;
                if (TrainFocus)
                {
                    int f = TrainFocusDeck;
                    return f >= 0 && f < decks.Count && deckProblems[f].Count == 0 && TrainPoolFor(f).Count > 0;
                }
                return trainA >= 0 && trainB >= 0 && trainA < decks.Count && trainB < decks.Count
                       && deckProblems[trainA].Count == 0 && deckProblems[trainB].Count == 0;
            }
        }

        private void StartTraining()
        {
            if (!TrainDecksReady || brain == null) return;
            if (training != null && training.Running) return;
            int games = Mathf.Clamp(trainGames, 1, MaxTrainGames);
            int seed = (Environment.TickCount & 0x0FFFFFFF) | 1;
            if (trainRandomA || trainRandomB)
            {
                int focus = TrainFocusDeck;
                var pool = new List<KeyValuePair<string, List<CardDef>>>();
                foreach (int i in focus >= 0 ? TrainPoolFor(focus) : AllTrainDecks())
                {
                    var problems = new List<string>();
                    var cards = DeckLibrary.Build(db, decks[i], problems);
                    if (problems.Count == 0) pool.Add(new KeyValuePair<string, List<CardDef>>(decks[i].Name, cards));
                }
                if (pool.Count == 0)
                {
                    trainMessage = "There are no legal decks to train with.";
                    return;
                }
                List<CardDef> focusCards = null;
                if (focus >= 0)
                {
                    var problems = new List<string>();
                    focusCards = DeckLibrary.Build(db, decks[focus], problems);
                    if (problems.Count > 0)
                    {
                        trainMessage = "The deck isn't legal: " + string.Join(" ", problems.ToArray());
                        return;
                    }
                    PlayerPrefs.SetString(trainRandomA ? PrefTrainB : PrefTrainA, decks[focus].Name);
                }
                string nameA = trainRandomA ? "Random deck" : decks[focus].Name, nameB = trainRandomB ? "Random deck" : decks[focus].Name;
                training = new TrainingSession(StandardCards.CreateRegistry(), brain, nameA, trainRandomA ? pool[0].Value : focusCards,
                    nameB, trainRandomB ? pool[0].Value : focusCards, games, trainMeasure ? MeasureGames : 0, seed)
                {
                    DeckPool = pool,
                    RandomA = trainRandomA,
                    RandomB = trainRandomB
                };
                training.Style = trainStyle;
                var poolFormat = (focus >= 0 ? decks[focus].ResolvedFormat : null) ?? GameFormat.Standard;
                training.Rules = poolFormat.Rules;
                training.PrizeCount = poolFormat.PrizeCount;
                sessionUndone = false;
                trainMessage = null;
                ApplyTrainSpeed(training);
                training.ChampionDecks = ChampionDeckPool();
                training.Start();
                trainView = training.Status();
                return;
            }
            var pa = new List<string>();
            var pb = new List<string>();
            var cardsA = DeckLibrary.Build(db, decks[trainA], pa);
            var cardsB = DeckLibrary.Build(db, decks[trainB], pb);
            if (pa.Count > 0 || pb.Count > 0)
            {
                trainMessage = "A deck isn't legal: " + string.Join(" ", pa.Concat(pb).ToArray());
                return;
            }
            PlayerPrefs.SetString(PrefTrainA, decks[trainA].Name);
            PlayerPrefs.SetString(PrefTrainB, decks[trainB].Name);
            training = new TrainingSession(StandardCards.CreateRegistry(), brain, decks[trainA].Name, cardsA, decks[trainB].Name, cardsB,
                games, trainMeasure ? MeasureGames : 0, seed);
            training.Style = trainStyle;
            // A custom format's deck trains by that format's rules and Prize cards (v123).
            var trainFormat = decks[trainA].ResolvedFormat ?? GameFormat.Standard;
            training.Rules = trainFormat.Rules;
            training.PrizeCount = trainFormat.PrizeCount;
            sessionUndone = false;
            trainMessage = null;
            ApplyTrainSpeed(training);
            training.ChampionDecks = ChampionDeckPool();
            training.Start();
            trainView = training.Status();
        }

        private void UndoSession()
        {
            if (training == null || training.Running || string.IsNullOrEmpty(training.BrainBefore)) return;
            brain.LoadText(training.BrainBefore);
            SaveBrainInBackground();
            sessionUndone = true;
            nextBrainRefresh = 0f;
            trainMessage = "The AI is back to how it was before this session.";
        }

        /// <summary>
        /// Makes the Neural AI's network deeper (PolicyNet.Grow): it keeps what it knows and plays the same until training puts the
        /// new blocks to use. Not while training runs here or in the background (that would save over it with the old size).
        /// </summary>
        private void GrowNetwork()
        {
            if (brain == null || (training != null && training.Running) || BackgroundRunning) return;
            brain.Policy.Grow(PolicyNet.BigBlocks);
            SaveBrainInBackground();
            nextBrainRefresh = 0f;
            trainMessage = "The neural network is bigger now (" + brain.Policy.Blocks + " blocks). It plays exactly as before; training puts the new blocks to use.";
        }

        private void ResetBrain()
        {
            if (training != null && training.Running) return;
            brain.Reset();
            brainError = null;
            SaveBrainInBackground();
            confirmReset = false;
            nextBrainRefresh = 0f;
            trainMessage = "The AI was reset: it plays like the classic bot again and will learn from scratch.";
        }

        private Texture2D ChartTexture(ref Texture2D tex, Color color)
        {
            if (tex == null) tex = CardVisuals.Solid(color);
            return tex;
        }

        private void DrawTrain(UiStyles ui)
        {
            var actions = new List<Action>();
            var st = trainView;
            bool running = st != null && st.Running;
            // The whole window's width (on a PC the screen has room for it), less a margin.
            float w = UiScale.Width - 30f;
            var r = new Rect(UiScale.Width / 2f - w / 2f, 15f, w, UiScale.Height - 30f);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 14, r.y + 10, r.width - 28, r.height - 20));
            GUILayout.Label("Train the AI", ui.Header);
            GUILayout.Label("Pick two decks: built-in or your own. The AI plays them against each other, learns from every game, and keeps what it learns for your games against it. "
                            + "It also learns from every game you play against it, and from your answers on the Areas of Improvement screen.", ui.Label);
            if (brainError != null) GUILayout.Label(brainError, ui.Error);

            // The settings column gets at least the room its widest row needs; the deck lists and the info panel share the rest.
            float setW = Mathf.Clamp((r.width - 28f) * 0.36f, 460f, 640f);
            float leftW = r.width - 28f - setW - 14f;
            float colW = (leftW - 8f) / 2f;
            float listH = Mathf.Clamp(r.height * 0.32f, 140f, 300f);
            // Two sides: the deck lists with the results and what the AI knows under them (left, two columns wide), and the
            // settings and progress in a column of their own that scrolls (right).
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(leftW));
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label("Deck A", ui.Title);
            GUI.enabled = !running;
            bool newRandomA = GUILayout.Toggle(trainRandomA, " A random deck every game");
            if (newRandomA != trainRandomA) actions.Add(() => SetTrainRandom(newRandomA, trainRandomB));
            GUI.enabled = !running && !trainRandomA;
            trainScrollA = GUILayout.BeginScrollView(trainScrollA, GUILayout.Height(listH));
            for (int i = 0; i < decks.Count; i++)
            {
                int k = i;
                if (GUILayout.Toggle(trainA == i, DeckLabel(i), ui.Button) && trainA != i) actions.Add(() => trainA = k);
            }
            GUILayout.EndScrollView();
            GUI.enabled = true;
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label("Deck B", ui.Title);
            GUI.enabled = !running;
            bool newRandomB = GUILayout.Toggle(trainRandomB, " A random deck every game");
            if (newRandomB != trainRandomB) actions.Add(() => SetTrainRandom(trainRandomA, newRandomB));
            GUI.enabled = !running && !trainRandomB;
            trainScrollB = GUILayout.BeginScrollView(trainScrollB, GUILayout.Height(listH));
            for (int i = 0; i < decks.Count; i++)
            {
                int k = i;
                if (GUILayout.Toggle(trainB == i, DeckLabel(i), ui.Button) && trainB != i) actions.Add(() => trainB = k);
            }
            GUILayout.EndScrollView();
            GUI.enabled = true;
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            // The session's results, the growth chart and what the AI knows, in the space under the deck lists.
            GUILayout.Space(6);
            trainScrollInfo = GUILayout.BeginScrollView(trainScrollInfo, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
            // Held to the panel's width (less its scroll bar), so long lines wrap and the lists' own scroll bars stay in view.
            GUILayout.BeginVertical(GUILayout.Width(leftW - 24f));
            DrawTrainResults(ui, st, actions);
            DrawGrowthChart(ui, leftW - 38f);
            DrawLogStudyPanel(ui, actions, running);
            DrawBrainPanel(ui, actions, running);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(setW));
            trainScrollSettings = GUILayout.BeginScrollView(trainScrollSettings, false, false, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.ExpandHeight(true));
            // Held to the column's width (less its scroll bar), so long lines wrap instead of running off the right edge.
            GUILayout.BeginVertical(GUILayout.Width(setW - 20f));
            GUILayout.Label("Games to play", ui.Title);
            GUI.enabled = !running;
            int countPick = Array.IndexOf(TrainCounts, trainGames);
            int newCount = GUILayout.SelectionGrid(countPick, TrainCountLabels, TrainCountsPerRow);
            if (newCount != countPick && newCount >= 0) actions.Add(() => SetTrainGames(TrainCounts[newCount]));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Or type any number:", GUILayout.Width(140));
            string newGamesText = GUILayout.TextField(trainGamesText, GUILayout.Width(130));
            GUILayout.EndHorizontal();
            GUILayout.Label(trainGames.ToString("N0") + " games (up to " + MaxTrainGames.ToString("N0") + ")", Hint(ui));
            if (newGamesText != trainGamesText) actions.Add(() => TypeTrainGames(newGamesText));
            if (TrainAll)
                GUILayout.Label("All decks against all decks: every game picks two of your " + AllTrainDecks().Count + " legal decks at random (mirrors too), "
                                + "the way the Python bot trained. Decks for a custom format aren't included.", Hint(ui));
            else if (TrainFocus)
            {
                int f = TrainFocusDeck;
                GUILayout.Label("Focused practice: " + (f >= 0 && f < decks.Count ? decks[f].Name : "the deck you pick on the other side")
                                + " plays every game, against a random deck from your " + TrainPoolFor(f).Count + " legal decks (mirrors too).", Hint(ui));
            }
            bool newMeasure = GUILayout.Toggle(trainMeasure, " Measure it against the classic bot before and after (+" + (MeasureGames * 2) + " games)");
            if (newMeasure != trainMeasure) actions.Add(() => trainMeasure = newMeasure);
            if (trainMeasure)
                GUILayout.Label("A test it takes twice: before training it plays " + MeasureGames + " games against the classic bot (the sim's original AI, which never "
                                + "learns); after training it replays the same " + MeasureGames + " games (same decks, seats and shuffles), so the difference "
                                + "comes from what it learned, not luck. It doesn't learn from these games. 50% = as good as the classic bot"
                                + (trainStyle == AiStyle.Neural ? "; the Neural AI is also checked (100 games) every fifth of the session." : "."), Hint(ui));
            GUILayout.Label("Train as", ui.Label);
            int newStyle = GUILayout.Toolbar((int)trainStyle, new[] { OpponentChoices[0], OpponentChoices[1], OpponentChoices[2] });
            if (newStyle != (int)trainStyle)
                actions.Add(() =>
                {
                    trainStyle = (AiStyle)Mathf.Clamp(newStyle, 0, 2);
                    PlayerPrefs.SetInt(PrefTrainStyle, (int)trainStyle);
                });
            GUILayout.Label(TrainStyleHint(trainStyle), Hint(ui));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed", GUILayout.Width(60));
            int speed = GUILayout.Toolbar(trainFast ? 1 : 0, new[] { "Gentle", "Fast" });
            GUILayout.EndHorizontal();
            if ((speed == 1) != trainFast)
                actions.Add(() =>
                {
                    trainFast = speed == 1;
                    PlayerPrefs.SetInt(PrefTrainFast, trainFast ? 1 : 0);
                });
            GUILayout.Label(trainFast
                ? "Fast (Neural AI training): " + FastThreads + " games at once and no rests, about 1.5 to 2 times Gentle's speed. It works the CPU hard: keep \"Don't use CPU threads\" on while your CPU has a faulty core."
                : "Gentle (Neural AI training): half the CPU's threads, with a short rest after each learning step: cooler and quieter.", Hint(ui));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Games per second", GUILayout.Width(120));
            string newRate = GUILayout.TextField(trainRateText, GUILayout.Width(60));
            GUILayout.Label(TrainRate > 0 ? "  at most " + TrainRate.ToString("0.#") + " a second" : "  blank: as fast as it can", Hint(ui));
            GUILayout.EndHorizontal();
            if (newRate != trainRateText)
                actions.Add(() =>
                {
                    trainRateText = newRate;
                    PlayerPrefs.SetString(PrefTrainRate, trainRateText ?? "");
                });
            GUILayout.BeginHorizontal();
            bool newGpu = GUILayout.Toggle(trainGpu, " Learn on the GPU at");
            string newGpuAddress = GUILayout.TextField(trainGpuAddress, GUILayout.Width(130));
            GUILayout.EndHorizontal();
            if (newGpu != trainGpu || newGpuAddress != trainGpuAddress)
                actions.Add(() =>
                {
                    trainGpu = newGpu;
                    trainGpuAddress = newGpuAddress;
                    PlayerPrefs.SetInt(PrefTrainGpu, trainGpu ? 1 : 0);
                    PlayerPrefs.SetString(PrefTrainGpuAddress, trainGpuAddress ?? "");
                });
            // Expert roadmap step 2: learning from the look-ahead.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Learn from look-ahead", GUILayout.Width(150));
            int newLookAhead = GUILayout.Toolbar(trainLookAhead, TrainLookAheadNames);
            GUILayout.EndHorizontal();
            if (newLookAhead != trainLookAhead)
                actions.Add(() =>
                {
                    trainLookAhead = newLookAhead;
                    PlayerPrefs.SetInt(PrefTrainLookAhead, trainLookAhead);
                });
            GUILayout.Label("Neural AI only. In these games the network looks ahead before some of the moves it isn't sure of, plays the move the "
                            + "look-ahead found, and learns to judge moves the way the look-ahead did. Slower: 10% of games trains about 40% "
                            + "slower, 25% about twice as slow. The champion checks show whether it's paying off."
                            + (trainGpu ? " Learning on the GPU needs the updated learner.py for this." : ""), Hint(ui));
            // Expert roadmap step 6: the league's exploiters.
            bool newExploiters = GUILayout.Toggle(trainExploiters, " Train exploiters for the league");
            if (newExploiters != trainExploiters)
                actions.Add(() =>
                {
                    trainExploiters = newExploiters;
                    PlayerPrefs.SetInt(PrefTrainExploiters, trainExploiters ? 1 : 0);
                });
            GUILayout.Label("Neural AI only. Every " + ExploiterEveryText + " training games, a copy of the network trains "
                            + TrainingSession.DefaultExploiterGames.ToString("N0") + " games learning only to beat it; if it then wins " + Math.Round(ChampionGate.PromoteAt * 100)
                            + "% or more, it found a weakness and joins the league (the past versions it trains against, under What the AI knows), so "
                            + "the network learns to cover it. About 5% more training time (around 10 to 15% when choosing moves on the GPU: an "
                            + "exploiter's moves are chosen on the CPU).", Hint(ui));
            if (trainGpu)
            {
                bool newGpuMoves = GUILayout.Toggle(trainGpuMoves, " Choose moves on the GPU too (many games at once)");
                if (newGpuMoves != trainGpuMoves)
                    actions.Add(() =>
                    {
                        trainGpuMoves = newGpuMoves;
                        PlayerPrefs.SetInt(PrefTrainGpuMoves, trainGpuMoves ? 1 : 0);
                    });
                GUILayout.Label("Running the network for each move is about half of a training game's work on the CPU. With this on, about 4 games "
                                + "per thread play at once and the GPU learner chooses all their moves together, so the CPU only runs the games. "
                                + "Needs the updated learner.py.", Hint(ui));
            }
            if (trainGpu)
                GUILayout.Label("Neural AI training plays its games here and learns on the graphics card: start PokemonTCGSim/GpuLearner/learner.py first "
                                + "(see its README). Learning is most of the training time, so this is about 2 to 3 times as fast and keeps that work off the CPU. "
                                + "If the learner isn't running, it learns on the CPU and says so.", Hint(ui));
            GUI.enabled = true;
            if (CpuThreads.Supported)
            {
                GUILayout.BeginHorizontal();
                bool newSkip = GUILayout.Toggle(skipCpu, " Don't use CPU threads");
                string newSkipText = GUILayout.TextField(skipCpuText, GUILayout.Width(90));
                GUILayout.EndHorizontal();
                if (newSkip != skipCpu || newSkipText != skipCpuText) actions.Add(() => SetSkipCpu(newSkip, newSkipText));
                GUILayout.Label((skipCpu && skipCpuMessage != null ? skipCpuMessage + " " : "")
                                + "For a faulty CPU core: the whole app stays off these threads, numbered as in Task Manager's Set affinity "
                                + "(CPU 0, 1, 2...) and OCCT's logical cores.", Hint(ui));
            }
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            bool background = BackgroundRunning;
            GUI.enabled = !running && !background && TrainDecksReady && brain != null;
            if (GUILayout.Button("Start training", GUILayout.Height(32))) actions.Add(StartTraining);
            GUI.enabled = running;
            if (GUILayout.Button("Stop", GUILayout.Height(32), GUILayout.Width(100))) actions.Add(() => training.Cancel());
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            // v120: a black screen while it trains (it keeps going; tap to wake).
            if (GUILayout.Button("Dark screen", GUILayout.Height(32))) actions.Add(PowerSaver.GoDark);
            GUI.enabled = true;
            if (GUILayout.Button("Back to menu", GUILayout.Height(32))) actions.Add(() => { RefreshDecks(); mode = Mode.Menu; });
            GUILayout.EndHorizontal();
            DrawBackgroundTraining(ui, actions, running);
            if (running && PowerSaver.Active) GUILayout.Label("Battery saver on: training plays half the time and rests the other half.", Hint(ui));
            if (!TrainDecksReady && TrainAll)
                GUILayout.Label("There are no legal decks to train with.", ui.Error);
            else if (!TrainDecksReady && TrainFocus)
                GUILayout.Label("Pick a legal deck on the side that isn't random.", ui.Error);
            else if (!TrainDecksReady && trainA >= 0 && trainB >= 0)
                GUILayout.Label("Both decks must be legal to train with them.", ui.Error);
            if (!trainRandomA && !trainRandomB && trainA >= 0 && trainA == trainB) GUILayout.Label("Same deck on both sides: a mirror match. The AI learns that deck's own play.", Hint(ui));
            if (trainMessage != null) GUILayout.Label(trainMessage, ui.Good);
            DrawTrainProgress(ui, st, setW - 34f);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            foreach (var a in actions) a();
        }

        private void DrawTrainProgress(UiStyles ui, TrainingStatus st, float width)
        {
            if (st == null) return;
            GUILayout.Space(6);
            GUILayout.Label(st.Phase, ui.Label);
            int total = st.Games + st.EvalTotal;
            int done = st.Done + st.EvalDone;
            float frac = total == 0 ? 0f : Mathf.Clamp01((float)done / total);
            var bar = GUILayoutUtility.GetRect(width, 18f);
            GUI.Box(bar, GUIContent.none);
            GUI.DrawTexture(new Rect(bar.x + 2, bar.y + 2, (bar.width - 4) * frac, bar.height - 4), ui.Selected);
            string speed = st.GamesPerSecond > 0 ? "  ·  " + st.GamesPerSecond.ToString("0.0") + " games/s" : "";
            string left = "";
            if (st.Running && st.GamesPerSecond > 0)
            {
                double seconds = (total - done) / st.GamesPerSecond;
                left = "  ·  about " + TimeLeft(seconds) + " left";
            }
            GUILayout.Label("Training games " + st.Done + " / " + st.Games + (st.EvalTotal > 0 ? "   ·   measuring games " + st.EvalDone + " / " + st.EvalTotal : "") + speed + left, Hint(ui));
            if (st.Errors > 0) GUILayout.Label(st.Errors + " game(s) hit an engine error and were skipped. Last: " + st.LastError, ui.Error);
            else if (!string.IsNullOrEmpty(st.LastError)) GUILayout.Label(st.LastError, ui.Error);
        }

        private void DrawTrainResults(UiStyles ui, TrainingStatus st, List<Action> actions)
        {
            if (st == null || training == null) return;
            bool all = training.AllDecksMode, focus = training.FocusMode;
            // In focus mode the focused deck's wins are on its own side (A or B).
            bool focusIsA = focus && !training.RandomA;
            GUILayout.Label("This session: " + (all || focus ? training.MatchLabel : training.DeckA + " (A) vs " + training.DeckB + " (B)") + ", as the " + training.StyleLabel, ui.Title);
            int played = st.WinsA + st.WinsB + st.Draws;
            if (played > 0 && all)
                GUILayout.Label(played.ToString("N0") + " games played" + (st.Draws > 0 ? ", " + st.Draws + " unfinished" : "") + ".", ui.Label);
            else if (played > 0 && focus)
            {
                int focusWins = focusIsA ? st.WinsA : st.WinsB;
                GUILayout.Label(training.FocusDeck + " won " + Pct(focusWins, played) + " (" + focusWins + " of " + played.ToString("N0") + ") against random decks"
                                + (st.Draws > 0 ? ", " + st.Draws + " unfinished" : "") + ".", ui.Label);
            }
            else if (played > 0)
                GUILayout.Label("Deck A won " + Pct(st.WinsA, played) + " (" + st.WinsA + "), deck B won " + Pct(st.WinsB, played) + " (" + st.WinsB + ")"
                                + (st.Draws > 0 ? ", " + st.Draws + " unfinished" : "") + ". Seats alternate, so each deck goes first half the time.", ui.Label);
            foreach (var b in st.Blocks)
            {
                int n = b.Played;
                if (n == 0) continue;
                string tried = b.Decisions > 0 ? "  ·  tried something new in " + (100.0 * b.Explored / b.Decisions).ToString("0.0") + "% of decisions" : "";
                string wins = all ? "" : focus ? training.FocusDeck + " " + Pct(focusIsA ? b.WinsA : b.WinsB, n) + "  ·  " : "A " + Pct(b.WinsA, n) + "  ·  B " + Pct(b.WinsB, n) + "  ·  ";
                GUILayout.Label("Games " + b.From + "–" + b.To + ":  " + wins
                                + b.AverageTurns.ToString("0.0") + " turns on average" + tried, Hint(ui));
            }
            if (st.ScoreBefore >= 0 || st.ScoreAfter >= 0)
            {
                string before = st.ScoreBefore >= 0 ? (st.ScoreBefore * 100).ToString("0") + "%" : "-";
                string after = st.ScoreAfter >= 0 ? (st.ScoreAfter * 100).ToString("0") + "%" : (st.Running ? "measuring..." : "-");
                GUILayout.Label("Test against the classic bot: won " + before + " before training, " + after + " after", ui.Title);
                GUILayout.Label("The same " + MeasureGames + " games both times (" + (all ? "random decks" : focus ? training.FocusDeck + " against random decks" : "half with each deck")
                                + ", same seats and shuffles), so the change comes from what it learned. 50% = as good as the classic bot; unfinished games count as half a win.", Hint(ui));
                if (st.ScoreBefore >= 0 && st.ScoreAfter >= 0)
                {
                    double delta = (st.ScoreAfter - st.ScoreBefore) * 100;
                    string verdict = delta >= 5 ? "It got better."
                        : delta <= -5 ? "It got worse this time: you can undo the session below."
                        : "No clear change: with " + MeasureGames + " games, differences under about 5 points are mostly luck.";
                    GUILayout.Label((delta >= 0 ? "+" : "") + delta.ToString("0") + " points. " + verdict, delta <= -5 ? ui.Error : ui.Good);
                }
            }
            if (st.Finished && !sessionUndone && !string.IsNullOrEmpty(training.BrainBefore))
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Undo this session", GUILayout.Width(170))) actions.Add(UndoSession);
                GUILayout.Label("Puts the AI back to how it was before these games.", Hint(ui));
                GUILayout.EndHorizontal();
            }
            GUILayout.Space(8);
        }

        /// <summary>Bars: how the AI did against the classic bot after each measured session (50% = as good as the classic bot).</summary>
        private void DrawGrowthChart(UiStyles ui, float width)
        {
            var b = brainView;
            if (b == null) return;
            var measured = b.History.Where(rec => rec.ScoreAfter >= 0).ToList();
            if (measured.Count == 0)
            {
                GUILayout.Label("Growth: train with \"Measure it against the classic bot\" on to see how the AI improves over time.", Hint(ui));
                return;
            }
            GUILayout.Label("Growth: the AI against the classic bot after each measured session (the line is 50%, as good as the classic bot)", ui.Title);
            if (chartLabel == null)
            {
                chartLabel = new GUIStyle(ui.Small);
                chartLabel.normal.textColor = Color.white;
            }
            var area = GUILayoutUtility.GetRect(width, 130f);
            GUI.DrawTexture(area, ChartTexture(ref chartBack, new Color(0f, 0f, 0f, 0.25f)));
            var shown = measured.Skip(Math.Max(0, measured.Count - 30)).ToList();
            float slot = (area.width - 20f) / Math.Max(1, shown.Count);
            float barW = Mathf.Min(40f, slot * 0.7f);
            float baseY = area.yMax - 18f;
            float h = area.height - 30f;
            for (int i = 0; i < shown.Count; i++)
            {
                var s = shown[i];
                float x = area.x + 10f + i * slot + (slot - barW) / 2f;
                float bh = (float)s.ScoreAfter * h;
                GUI.DrawTexture(new Rect(x, baseY - bh, barW, bh), ChartTexture(ref chartBar, new Color(0.35f, 0.75f, 1f, 0.9f)));
                GUI.Label(new Rect(x - 6f, baseY - bh - 16f, barW + 12f, 16f), (s.ScoreAfter * 100).ToString("0"), chartLabel);
                GUI.Label(new Rect(x - 6f, baseY + 1f, barW + 12f, 16f), "#" + (measured.Count - shown.Count + i + 1), chartLabel);
            }
            GUI.DrawTexture(new Rect(area.x + 4f, baseY - 0.5f * h, area.width - 8f, 1f), ChartTexture(ref chartLine, new Color(1f, 0.85f, 0.2f, 0.9f)));
            var last = shown[shown.Count - 1];
            GUILayout.Label("Last session: " + last.DeckA + " vs " + last.DeckB + (string.IsNullOrEmpty(last.Style) ? "" : " as the " + last.Style) + ", " + last.Games + " games on " + last.When
                            + (last.ScoreBefore >= 0 ? ", " + (last.ScoreBefore * 100).ToString("0") + "% → " + (last.ScoreAfter * 100).ToString("0") + "%" : "") + ".", Hint(ui));
            GUILayout.Space(8);
        }

        private Vector2 trainScrollChampion;
        private static string ChampionEveryText { get { return TrainingSession.DefaultChampionEvery.ToString("N0"); } }
        private static string ChampionMinText { get { return TrainingSession.DefaultChampionMinGames.ToString("N0"); } }

        private const string GrowCostText = "On the CPU, training is about 25% slower with it (learning takes about 40% longer); learning on the GPU, with Choose moves on the GPU too, it hardly matters. Against you, each move still takes a moment.";

        private Vector2 trainScrollLeague;
        private static string ExploiterEveryText { get { return TrainingSession.DefaultExploiterEvery.ToString("N0"); } }

        private void DrawBrainPanel(UiStyles ui, List<Action> actions, bool running)
        {
            var b = brainView;
            if (b == null) return;
            GUILayout.Label("What the AI knows", ui.Title);
            GUILayout.Label("Trained on " + b.GamesTrained.ToString("N0") + " games · learned from " + b.GamesVsYou + " of your games (" + b.DecisionsStudied.ToString("N0")
                            + " of your decisions studied) · " + b.Feedback + " answers on Areas of Improvement · " + b.LogsStudied.ToString("N0") + " game logs studied ("
                            + b.LogDecisions.ToString("N0") + " decisions) · " + b.Weights.ToString("N0") + " things learned.", ui.Label);
            GUILayout.Label("Neural network: " + (string.IsNullOrEmpty(b.NetOrigin) ? "" : "started from the " + b.NetOrigin + " · ")
                            + (b.NetSteps > 0
                                ? b.NetSteps.ToString("N0") + " learning steps here · loss " + b.NetLoss.ToString("0.00")
                                  + " (how far off its guesses of who wins were, lately: not a score, since its opponent is itself and gets better too; "
                                  + "a sudden jump to several times its usual size means something went wrong)"
                                : "no learning steps here yet (Train as Neural AI to train it)")
                            + " · your opponent plays as " + StyleName(b.Style) + " (the Opponent switch on the menu); training plays as " + StyleName(trainStyle)
                            + " (Train as, above).", ui.Label);
            GUILayout.BeginHorizontal();
            if (b.NetBlocks < PolicyNet.BigBlocks)
            {
                GUI.enabled = !running && !BackgroundRunning;
                if (GUILayout.Button("Make the network bigger (" + PolicyNet.BigBlocks + " blocks)", GUILayout.Width(280))) actions.Add(GrowNetwork);
                GUI.enabled = true;
                GUILayout.Label("Network size: " + b.NetBlocks + " blocks. Bigger (" + PolicyNet.BigBlocks + ") can learn more: it keeps everything it knows and plays "
                                + "exactly the same at first, and training puts the new blocks to use. " + GrowCostText + " It can't be made smaller "
                                + "again (Reset the AI goes back to the starting network)." + (running || BackgroundRunning ? " (Not while training runs.)" : ""), Hint(ui));
            }
            else
                GUILayout.Label("Network size: " + b.NetBlocks + " blocks (made bigger). " + GrowCostText, Hint(ui));
            GUILayout.EndHorizontal();
            // Each list scrolls on its own (scroll with the mouse over it, or drag its scroll bar) and holds everything.
            float listMax = Mathf.Clamp(UiScale.Height * 0.3f, 150f, 360f);
            // The yardstick (ChampionGate): the network's champion version and its checks.
            if (b.ChampionVersion > 0)
            {
                GUILayout.Label("Champion: version " + b.ChampionVersion + ". The yardstick: every " + ChampionEveryText + " Neural AI training games, and at the "
                                + "end of a session of " + ChampionMinText + " or more, the network plays " + ChampionGate.DefaultGames + " games against the champion (the "
                                + "same games each time, all your legal decks). It takes over at " + Math.Round(ChampionGate.PromoteAt * 100) + "% or more, so a new "
                                + "version means it really got better · " + b.ChampionChecks.Count + (b.ChampionChecks.Count == 1 ? " check" : " checks"), Hint(ui));
                trainScrollChampion = DrawScrollList(ui, trainScrollChampion, b.ChampionChecks.Count, Mathf.Min(listMax, 150f), b.ChampionRow);
            }
            else
                GUILayout.Label("Champion: none yet. The next Neural AI training session keeps the network as it is as the first champion, the yardstick it "
                                + "has to beat later.", Hint(ui));
            // The league (League): past versions it trains against, and what happened there.
            if (b.League.Count > 0 || b.LeagueEvents.Count > 0)
            {
                GUILayout.Label("League: past versions it trains against besides itself (in 15 to 30% of its training games, the ones it does worst against "
                                + "most often). A champion it replaces joins, a snapshot every " + TrainingSession.LeagueSnapshotEvery.ToString("N0")
                                + " games, and exploiters that find a weakness; at most " + League.Capacity + ", the easiest leaves first · "
                                + b.League.Count + (b.League.Count == 1 ? " past version" : " past versions"), Hint(ui));
                trainScrollLeague = DrawScrollList(ui, trainScrollLeague, b.League.Count + b.LeagueEvents.Count, Mathf.Min(listMax, 180f), b.LeagueRow);
            }
            else
                GUILayout.Label("League: none yet. The next Neural AI training session starts it with the champion, a past version it keeps "
                                + "training against as it moves on.", Hint(ui));
            if (b.Decks.Count > 0)
            {
                GUILayout.Label("How well the Neural AI knows each deck it has trained with, most games first (about 0.6 = plays it confidently, 0.9 = still "
                                + "working it out; the deck advisor counts the decks it wins more with more) · " + b.Decks.Count.ToString("N0")
                                + (b.Decks.Count == 1 ? " deck" : " decks"), Hint(ui));
                trainScrollDecks = DrawScrollList(ui, trainScrollDecks, b.Decks.Count, listMax, b.DeckRow);
            }
            if (b.Matchups.Count > 0)
            {
                GUILayout.Label("Matchups it has played, most games first (that deck's win rate) · " + b.Matchups.Count.ToString("N0")
                                + (b.Matchups.Count == 1 ? " matchup" : " matchups"), Hint(ui));
                trainScrollMatchups = DrawScrollList(ui, trainScrollMatchups, b.Matchups.Count, listMax, b.MatchupRow);
            }
            if (b.Lessons.Count > 0)
            {
                GUILayout.Label("Its lessons, strongest first (+ plays it likes more than the classic bot, − plays it likes less) · "
                                + b.Lessons.Count.ToString("N0") + (b.Lessons.Count == 1 ? " lesson" : " lessons"), Hint(ui));
                trainScrollLessons = DrawScrollList(ui, trainScrollLessons, b.Lessons.Count, listMax, b.LessonRow);
            }
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUI.enabled = !running;
            if (!confirmReset)
            {
                if (GUILayout.Button("Reset the AI", GUILayout.Width(140))) actions.Add(() => confirmReset = true);
                GUILayout.Label("Erases everything it learned (training, your games, your feedback), the neural network included.", Hint(ui));
            }
            else
            {
                GUILayout.Label("Erase everything the AI learned?", ui.Error);
                if (GUILayout.Button("Yes, reset it", GUILayout.Width(120))) actions.Add(ResetBrain);
                if (GUILayout.Button("Cancel", GUILayout.Width(80))) actions.Add(() => confirmReset = false);
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.Label("The AI's file: " + BrainPath, Hint(ui));
        }

        /// <summary>
        /// A list in a box with its own scroll bar, holding every row. Only the rows in view are drawn, so a list of thousands
        /// costs no more than a short one. A short list shrinks to fit. Returns the new scroll position.
        /// </summary>
        private Vector2 DrawScrollList(UiStyles ui, Vector2 scroll, int count, float maxHeight, Func<int, string> rowText)
        {
            var style = ListRowStyle(ui);
            float rowH = listRowHeight;
            float contentH = Mathf.Max(rowH, count * rowH);
            float viewH = Mathf.Min(maxHeight, contentH + 4f);
            GUILayout.BeginVertical(GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(viewH));
            var area = GUILayoutUtility.GetRect(10f, contentH, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint && count > 0)
            {
                int first = Mathf.Clamp(Mathf.FloorToInt((scroll.y - area.y) / rowH), 0, count - 1);
                int last = Mathf.Clamp(Mathf.CeilToInt((scroll.y - area.y + viewH) / rowH), 0, count - 1);
                for (int k = first; k <= last; k++)
                    GUI.Label(new Rect(area.x, area.y + k * rowH, area.width, rowH), rowText(k), style);
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            return scroll;
        }

        /// <summary>One line per row: no wrapping, cut off at the edge.</summary>
        private GUIStyle ListRowStyle(UiStyles ui)
        {
            if (listRowStyle == null)
            {
                listRowStyle = new GUIStyle(Hint(ui)) { wordWrap = false, clipping = TextClipping.Clip, alignment = TextAnchor.MiddleLeft };
                listRowStyle.margin = new RectOffset(0, 0, 0, 0);
                listRowHeight = Mathf.Max(18f, Mathf.Ceil(listRowStyle.CalcHeight(new GUIContent("Ag"), 1000f)) + 2f);
            }
            return listRowStyle;
        }

        /// <summary>"40 s", "25 min", "2 h 10 min" (the 10,000 to 25,000 game sessions can run for hours).</summary>
        private static string TimeLeft(double seconds)
        {
            if (seconds < 90) return Math.Ceiling(seconds) + " s";
            double minutes = Math.Ceiling(seconds / 60);
            if (minutes < 90) return minutes + " min";
            int hours = (int)(minutes / 60);
            int rest = (int)(minutes - hours * 60);
            return hours + " h" + (rest > 0 ? " " + rest + " min" : "");
        }

        private static string Pct(int part, int whole)
        {
            return whole <= 0 ? "-" : (100.0 * part / whole).ToString("0") + "%";
        }
    }
}
