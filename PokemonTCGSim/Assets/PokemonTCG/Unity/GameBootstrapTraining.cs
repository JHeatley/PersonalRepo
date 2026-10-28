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
        /// <summary>Keep the app off some CPU threads (a faulty core), and which ones (CpuThreads).</summary>
        private const string PrefSkipCpu = "PokemonTCG.SkipCpuThreads";
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
        private int trainCountIndex = 2;
        private bool trainMeasure = true;
        private AiStyle trainStyle = AiStyle.Hybrid;
        private bool confirmReset;
        private bool sessionUndone;
        private Vector2 trainScrollA, trainScrollB, trainScrollInfo, trainScrollMatchups, trainScrollLessons;
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
            public AiStyle Style;
            public List<MatchupStats> Matchups = new List<MatchupStats>();
            public List<KeyValuePair<string, double>> Lessons = new List<KeyValuePair<string, double>>();
            public List<SessionRecord> History = new List<SessionRecord>();
            private string[] lessonText;

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
            try
            {
                UnityCardData.LoadPolicyStart();
                brain = LearningBrain.Load(BrainPath);
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
            return agent;
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
            var a = new GameAnalyzer(StandardCards.CreateRegistry(), b, decisions, events, seat, yourDeck, theirDeck, state, engine.PrizeCount, engine.Rules);
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
            confirmReset = false;
            nextBrainRefresh = 0f;
            mode = Mode.Train;
        }

        /// <summary>Called from Update: copies everything the Train screen and menu show, once per frame (IMGUI draws in several passes).</summary>
        private void UpdateTraining()
        {
            if (training != null) trainView = training.Status();
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
                    Style = brain.Style,
                    Matchups = brain.Matchups(),
                    Lessons = brain.StrongestLessons(0, 5), // all of them; the Train screen shows them in a scrolling list
                    History = brain.History()
                };
            }
        }

        private bool TrainDecksReady
        {
            get
            {
                return trainA >= 0 && trainB >= 0 && trainA < decks.Count && trainB < decks.Count
                       && deckProblems[trainA].Count == 0 && deckProblems[trainB].Count == 0;
            }
        }

        private void StartTraining()
        {
            if (!TrainDecksReady || brain == null) return;
            if (training != null && training.Running) return;
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
            int games = TrainCounts[Mathf.Clamp(trainCountIndex, 0, TrainCounts.Length - 1)];
            int seed = (Environment.TickCount & 0x0FFFFFFF) | 1;
            training = new TrainingSession(StandardCards.CreateRegistry(), brain, decks[trainA].Name, cardsA, decks[trainB].Name, cardsB,
                games, trainMeasure ? MeasureGames : 0, seed);
            training.Style = trainStyle;
            // A custom format's deck trains by that format's rules and Prize cards (v123).
            var trainFormat = decks[trainA].ResolvedFormat ?? GameFormat.Standard;
            training.Rules = trainFormat.Rules;
            training.PrizeCount = trainFormat.PrizeCount;
            sessionUndone = false;
            trainMessage = null;
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

        private void ResetBrain()
        {
            if (training != null && training.Running) return;
            brain.Reset();
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
            float w = Mathf.Min(1180f, UiScale.Width - 30f);
            var r = new Rect(UiScale.Width / 2f - w / 2f, 15f, w, UiScale.Height - 30f);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 14, r.y + 10, r.width - 28, r.height - 20));
            GUILayout.Label("Train the AI", ui.Header);
            GUILayout.Label("Pick two decks: built-in or your own. The AI plays them against each other, learns from every game, and keeps what it learns for your games against it. "
                            + "It also learns from every game you play against it, and from your answers on the Areas of Improvement screen.", ui.Label);
            if (brainError != null) GUILayout.Label(brainError, ui.Error);

            float colW = (r.width - 60f) / 3f;
            float listH = Mathf.Clamp(r.height * 0.32f, 140f, 300f);
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label("Deck A", ui.Title);
            GUI.enabled = !running;
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
            trainScrollB = GUILayout.BeginScrollView(trainScrollB, GUILayout.Height(listH));
            for (int i = 0; i < decks.Count; i++)
            {
                int k = i;
                if (GUILayout.Toggle(trainB == i, DeckLabel(i), ui.Button) && trainB != i) actions.Add(() => trainB = k);
            }
            GUILayout.EndScrollView();
            GUI.enabled = true;
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label("Games to play", ui.Title);
            GUI.enabled = !running;
            int newCount = GUILayout.SelectionGrid(trainCountIndex, TrainCountLabels, TrainCountsPerRow);
            if (newCount != trainCountIndex) actions.Add(() => trainCountIndex = newCount);
            bool newMeasure = GUILayout.Toggle(trainMeasure, " Measure it against the classic bot before and after (+" + (MeasureGames * 2) + " games)");
            if (newMeasure != trainMeasure) actions.Add(() => trainMeasure = newMeasure);
            GUILayout.Label("Train as", ui.Label);
            int newStyle = GUILayout.Toolbar((int)trainStyle, new[] { OpponentChoices[0], OpponentChoices[1], OpponentChoices[2] });
            if (newStyle != (int)trainStyle)
                actions.Add(() =>
                {
                    trainStyle = (AiStyle)Mathf.Clamp(newStyle, 0, 2);
                    PlayerPrefs.SetInt(PrefTrainStyle, (int)trainStyle);
                });
            GUILayout.Label(TrainStyleHint(trainStyle), Hint(ui));
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
            GUI.enabled = !running && TrainDecksReady && brain != null;
            if (GUILayout.Button("Start training", GUILayout.Height(32))) actions.Add(StartTraining);
            GUI.enabled = running;
            if (GUILayout.Button("Stop", GUILayout.Height(32), GUILayout.Width(80))) actions.Add(() => training.Cancel());
            // v120: a black screen while it trains (it keeps going; tap to wake).
            if (GUILayout.Button("Dark screen", GUILayout.Height(32), GUILayout.Width(110))) actions.Add(PowerSaver.GoDark);
            GUI.enabled = true;
            if (GUILayout.Button("Back to menu", GUILayout.Height(32), GUILayout.Width(120))) actions.Add(() => { RefreshDecks(); mode = Mode.Menu; });
            GUILayout.EndHorizontal();
            if (running && PowerSaver.Active) GUILayout.Label("Battery saver on: training plays half the time and rests the other half.", Hint(ui));
            if (!TrainDecksReady && trainA >= 0 && trainB >= 0)
                GUILayout.Label("Both decks must be legal to train with them.", ui.Error);
            if (trainA >= 0 && trainA == trainB) GUILayout.Label("Same deck on both sides: a mirror match. The AI learns that deck's own play.", Hint(ui));
            if (trainMessage != null) GUILayout.Label(trainMessage, ui.Good);
            DrawTrainProgress(ui, st, colW);
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            GUILayout.Space(6);

            trainScrollInfo = GUILayout.BeginScrollView(trainScrollInfo);
            DrawTrainResults(ui, st, actions);
            DrawGrowthChart(ui, r.width - 60f);
            DrawLogStudyPanel(ui, actions, running);
            DrawBrainPanel(ui, actions, running);
            GUILayout.EndScrollView();
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
            GUILayout.Label("This session: " + training.DeckA + " (A) vs " + training.DeckB + " (B), as the " + training.StyleLabel, ui.Title);
            int played = st.WinsA + st.WinsB + st.Draws;
            if (played > 0)
                GUILayout.Label("Deck A won " + Pct(st.WinsA, played) + " (" + st.WinsA + "), deck B won " + Pct(st.WinsB, played) + " (" + st.WinsB + ")"
                                + (st.Draws > 0 ? ", " + st.Draws + " unfinished" : "") + ". Seats alternate, so each deck goes first half the time.", ui.Label);
            foreach (var b in st.Blocks)
            {
                int n = b.Played;
                if (n == 0) continue;
                string tried = b.Decisions > 0 ? "  ·  tried something new in " + (100.0 * b.Explored / b.Decisions).ToString("0.0") + "% of decisions" : "";
                GUILayout.Label("Games " + b.From + "–" + b.To + ":  A " + Pct(b.WinsA, n) + "  ·  B " + Pct(b.WinsB, n) + "  ·  " + b.AverageTurns.ToString("0.0") + " turns on average" + tried, Hint(ui));
            }
            if (st.ScoreBefore >= 0 || st.ScoreAfter >= 0)
            {
                string before = st.ScoreBefore >= 0 ? (st.ScoreBefore * 100).ToString("0") + "%" : "-";
                string after = st.ScoreAfter >= 0 ? (st.ScoreAfter * 100).ToString("0") + "%" : (st.Running ? "measuring..." : "-");
                GUILayout.Label("Against the classic bot (same " + MeasureGames + " games each time, both decks): " + before + " before, " + after + " after.", ui.Title);
                if (st.ScoreBefore >= 0 && st.ScoreAfter >= 0)
                {
                    double delta = (st.ScoreAfter - st.ScoreBefore) * 100;
                    string verdict = delta >= 5 ? "It got better." : delta <= -5 ? "It got worse this time: you can undo the session below." : "No clear change (differences under about 5 points are noise).";
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
                                ? b.NetSteps.ToString("N0") + " learning steps here · loss " + b.NetLoss.ToString("0.00") + " (lower is better)"
                                : "no learning steps here yet (Train as Neural AI to train it)")
                            + " · your opponent plays as " + StyleName(b.Style) + " (the Opponent switch on the menu); training plays as " + StyleName(trainStyle)
                            + " (Train as, above).", ui.Label);
            // Each list scrolls on its own (scroll with the mouse over it, or drag its scroll bar) and holds everything.
            float listMax = Mathf.Clamp(UiScale.Height * 0.3f, 150f, 360f);
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
