using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Drop this on an empty GameObject in an empty scene and press Play.
    /// Menu: pick your deck and the bot's deck (or watch two bots). Deck builder: build, import and save decks.
    /// Game: plays engine events back at a readable pace and lets you play by dragging cards, clicking Pokémon
    /// and choosing cards in pop-ups (see GameBootstrapScreens.cs).
    ///
    /// The UI is IMGUI: it works with any render pipeline and input setting. Nothing in here decides rules;
    /// it only displays PlayerView snapshots and submits the options the engine offers.
    /// </summary>
    public sealed partial class GameBootstrap : MonoBehaviour
    {
        private enum Mode { Menu, Builder, Game, Train, Folders, Regional, Video, Formats, Rating }

        [Header("Players")]
        [Tooltip("0 = you are Player 1, 1 = you are Player 2.")]
        public int humanSeat;

        [Header("Presentation")]
        [Tooltip("Seconds per ordinary game event (set by the Pace buttons).")]
        public float secondsPerEvent = 0.45f;
        [Tooltip("Seconds an opponent's play is shown in the middle of the screen (set by the Pace buttons).")]
        public float revealSeconds = 1.8f;

        private const string PrefMyDeck = "PokemonTCG.MyDeck";
        private const string PrefOppDeck = "PokemonTCG.OpponentDeck";
        private const string PrefPace = "PokemonTCG.Pace";
        /// <summary>Whether the full battle log is open (v55.2; otherwise only its last 2 lines show along the bottom edge).</summary>
        private const string PrefLogOpen = "PokemonTCG.LogOpen";
        /// <summary>Your birthday as month * 100 + day (0 = not set), for _____'s Pikachu's Birthday Surprise (v48).</summary>
        private const string PrefBirthday = "PokemonTCG.Birthday";
        private static readonly string[] MonthNames = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private static readonly int[] DaysInMonth = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly string[] PaceNames = { "Slow", "Normal", "Fast" };
        private static readonly float[] PaceEvent = { 0.75f, 0.45f, 0.2f };
        private static readonly float[] PaceReveal = { 2.8f, 1.8f, 0.9f };
        private const string MineColor = "#86C8FF";
        private const string TheirColor = "#FF9E8C";

        private Mode mode = Mode.Menu;
        private Camera cam;
        private BoardView board;
        private CardDatabase db;
        private DeckBuilderScreen builder;
        /// <summary>Your custom formats (v123), opened from the menu or the deck builder (formatsFromBuilder: go back there).</summary>
        private FormatEditorScreen formatEditor;
        private bool formatsFromBuilder, pendingFormats;
        /// <summary>The menu's format: "" shows every deck; a format's name shows its decks and plays by its rules (v123).</summary>
        private string menuFormat = "";
        private const string PrefMenuFormat = "PokemonTCG.MenuFormat";
        private int pace = 1;

        // Menu
        private List<DeckRecord> decks = new List<DeckRecord>();
        private List<List<string>> deckProblems = new List<List<string>>();
        private int myDeck, oppDeck;
        private bool botVsBot;
        private string seedText = "";
        private int birthday;
        private string menuError;
        private Vector2 myScroll, oppScroll, menuScroll;

        // Game
        private EngineHost host;
        /// <summary>
        /// v134.4: the card scripts for the games on the board, built once instead of at the start of every game (about 9,500
        /// cards, a noticeable pause on the main thread). Like the AI tables, which keep one for all their games, the games
        /// take turns using it: a new one is built only if the last game's engine is still running.
        /// </summary>
        private CardScriptRegistry boardScripts;
        private EngineHost boardScriptsUser;
        private HumanAgent human;
        private PlayerView current;
        private float nextEventTime;
        private int currentSeed;
        /// <summary>The seat at the bottom of the screen whose hand is shown, or -1 when watching two bots.</summary>
        private int viewerSeat = -1;
        private readonly string[] deckNames = new string[2];
        private readonly List<string> log = new List<string>();
        private string errorText;
        private DeckRecord lastMine, lastTheirs;

        private DecisionRequest shownRequest;
        private readonly HashSet<int> multiSelection = new HashSet<int>();
        private int filterCardId = int.MinValue;
        private Vector2 optionScroll;
        private Vector2 logScroll;
        private CardView hovered;
        private Vector2 mouseGui;
        private Rect hudRect, logRect, panelRect, endRect;
        private GUIStyle logStyle, hintStyle, hudStyle, hpStyle, logLineStyle;
        /// <summary>The full battle log is open (v55.2). Closed, its last 2 lines show in a strip along the bottom edge.</summary>
        private bool logOpen;
        /// <summary>The decision panel is folded down to its title bar so the cards behind it can be seen and clicked (v55.2).</summary>
        private bool panelHidden;

        /// <summary>State changes made while drawing, applied after the GUI pass (IMGUI draws each frame in several passes).</summary>
        private readonly List<Action> guiActions = new List<Action>();

        private bool pendingStart, pendingRestart, pendingMenu, pendingBuilder, pendingTrain;

        private void Start()
        {
            // v118: phones and tablets run at 60 frames a second (Unity's mobile default is 30). v120: PowerSaver.Tick sets the
            // frame rate and when the screen may turn off from then on.
            if (Application.isMobilePlatform) Application.targetFrameRate = 60;
            SetupCamera();
            board = new GameObject("Board").AddComponent<BoardView>();
            board.Build();
            db = UnityCardData.LoadAllCards();
            CardVisuals.WarmIndexInBackground(); // v134.6: list the card images now, off the main thread
            SavePaths.Load();
            LoadSkipCpu();
            lookAhead = Mathf.Clamp(PlayerPrefs.GetInt(PrefLookAhead, 0), 0, 2);
            ApplyPace(PlayerPrefs.GetInt(PrefPace, 1));
            logOpen = PlayerPrefs.GetInt(PrefLogOpen, 0) == 1;
            birthday = Mathf.Max(0, PlayerPrefs.GetInt(PrefBirthday, 0));
            menuFormat = PlayerPrefs.GetString(PrefMenuFormat, "") ?? "";
            RefreshDecks();
            LoadBrain();
        }

        private void SetupCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                go.tag = "MainCamera";
                cam = go.AddComponent<Camera>();
            }
            cam.transform.position = BoardView.ViewPoint;
            cam.transform.LookAt(new Vector3(0f, 0f, -1.0f));
            cam.fieldOfView = 45f;
            // The nearest thing on the table is about 12 units away, so a near plane of 2 (not 0.1) keeps the depth buffer
            // precise (v41). Cards on the far side vanishing in builds turned out to be the shader, not depth precision (v54.1).
            cam.nearClipPlane = 2f;
            cam.farClipPlane = 60f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.06f, 0.07f, 0.09f);
        }

        private void ApplyPace(int p)
        {
            pace = Mathf.Clamp(p, 0, PaceNames.Length - 1);
            secondsPerEvent = PaceEvent[pace];
            revealSeconds = PaceReveal[pace];
            PlayerPrefs.SetInt(PrefPace, pace);
        }

        private void RefreshDecks()
        {
            FormatLibrary.LoadAll();
            if (menuFormat.Length > 0 && GameFormat.Named(menuFormat) == null) menuFormat = "";
            decks = DeckLibrary.LoadAll();
            deckProblems = decks.Select(d =>
            {
                var problems = new List<string>();
                DeckLibrary.Build(db, d, problems);
                return problems;
            }).ToList();
            myDeck = IndexOfDeck(PlayerPrefs.GetString(PrefMyDeck, ""), 0);
            oppDeck = IndexOfDeck(PlayerPrefs.GetString(PrefOppDeck, ""), decks.Count > 1 ? 1 : 0);
            KeepPicksInFormat();
        }

        /// <summary>Opens the format editor (v123); fromBuilder: Back returns to the deck builder.</summary>
        private void OpenFormats(bool fromBuilder)
        {
            formatsFromBuilder = fromBuilder;
            if (formatEditor == null) formatEditor = new FormatEditorScreen(db);
            else formatEditor.Reload();
            formatEditor.WantsClose = false;
            mode = Mode.Formats;
        }

        /// <summary>A deck the menu shows under its format choice (v123).</summary>
        private bool InMenuFormat(int i)
        {
            if (i < 0 || i >= decks.Count) return false;
            return menuFormat.Length == 0 || decks[i].FormatName.Equals(menuFormat, StringComparison.OrdinalIgnoreCase)
                   || (decks[i].ResolvedFormat != null && decks[i].ResolvedFormat.Name.Equals(menuFormat, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>After the format choice changes, the picked decks move to decks of that format.</summary>
        private void KeepPicksInFormat()
        {
            var shown = Enumerable.Range(0, decks.Count).Where(InMenuFormat).ToList();
            if (shown.Count == 0) return;
            if (!InMenuFormat(myDeck)) myDeck = shown[0];
            if (!InMenuFormat(oppDeck)) oppDeck = shown.Count > 1 && shown[0] == myDeck ? shown[1] : shown[0];
        }

        /// <summary>
        /// The format a game from the menu plays by (v123): the menu's format choice, or else your deck's format. Its Prize cards
        /// and rules (first turn, opening order, Tools, old Trainers) are used.
        /// </summary>
        private GameFormat MenuGameFormat(DeckRecord mine)
        {
            var chosen = menuFormat.Length > 0 ? GameFormat.Named(menuFormat) : null;
            return chosen ?? (mine != null ? mine.ResolvedFormat : null) ?? GameFormat.Standard;
        }

        private int IndexOfDeck(string name, int fallback)
        {
            int i = decks.FindIndex(d => d.Name == name);
            if (i >= 0) return i;
            return decks.Count == 0 ? -1 : Mathf.Clamp(fallback, 0, decks.Count - 1);
        }

        // ------------------------------------------------------------------ Starting and stopping games

        /// <summary>
        /// Starts a game. reg: a Regional Sim game (v57) - your game against an AI seat, or an AI table you watch - or null for
        /// an ordinary game from the menu.
        /// </summary>
        private void StartGame(DeckRecord mine, DeckRecord theirs, RegionalGameStart reg = null)
        {
            StopGame();
            menuError = null;
            var p1 = new List<string>();
            var p2 = new List<string>();
            var deck1 = DeckLibrary.Build(db, mine, p1);
            var deck2 = DeckLibrary.Build(db, theirs, p2);
            if (p1.Count > 0 || p2.Count > 0)
            {
                string problem = (p1.Count > 0 ? mine.Name + ": " + string.Join(" ", p1.ToArray()) + "\n" : "") +
                                 (p2.Count > 0 ? theirs.Name + ": " + string.Join(" ", p2.ToArray()) : "");
                if (reg != null)
                {
                    regionalError = "The game couldn't start: " + problem;
                    regionalGame = false;
                    regionalWatch = false;
                    mode = Mode.Regional;
                    return;
                }
                menuError = problem;
                mode = Mode.Menu;
                return;
            }
            lastMine = mine;
            lastTheirs = theirs;
            regionalGame = reg != null;
            regionalWatch = reg != null && reg.Watch;
            // Two bots: "Watch two bots play each other" from the menu, or a Regional Sim table you watch.
            bool watching = reg != null ? reg.Watch : botVsBot;

            errorText = null;
            log.Clear();
            current = null;
            shownRequest = null;
            multiSelection.Clear();
            filterCardId = int.MinValue;
            ResetScreens();

            int parsedSeed;
            currentSeed = reg == null && int.TryParse(seedText, out parsedSeed) && parsedSeed != 0 ? parsedSeed : (Environment.TickCount & 0x7fffffff) | 1;
            int seat = regionalWatch ? 0 : Mathf.Clamp(humanSeat, 0, 1);
            human = watching ? null : new HumanAgent();
            learningBot = null;
            watchAgentA = null;
            watchAgentB = null;
            humanLog = watching ? null : new DecisionLog();
            IPlayerAgent you, bot;
            if (regionalWatch)
            {
                // A Regional Sim table: both seats are the learning AI, and both learn from the game.
                watchAgentA = new LearningAgent(brain, currentSeed + 11, mine.Name, theirs.Name) { Training = reg.Explore, Record = brain != null, Style = reg.Style };
                watchAgentB = new LearningAgent(brain, currentSeed + 23, theirs.Name, mine.Name) { Training = reg.Explore, Record = brain != null, Style = reg.Style };
                you = watchAgentA;
                bot = watchAgentB;
            }
            else
            {
                // Your seat records every decision (the AI studies them; Areas of Improvement replays them). Your Regional Sim
                // opponents are always the learning AI.
                you = watching ? MakeBot(currentSeed + 11, mine.Name, theirs.Name, false) : new InteractiveRecordingAgent(human, humanLog);
                bot = MakeBot(currentSeed + 23, theirs.Name, mine.Name, !watching, reg != null, reg != null ? reg.Style : (AiStyle?)null);
            }
            IPlayerAgent a0 = seat == 0 ? you : bot;
            IPlayerAgent a1 = seat == 0 ? bot : you;
            var d0 = seat == 0 ? deck1 : deck2;
            var d1 = seat == 0 ? deck2 : deck1;
            // The game is saved to the Game logs folder when it ends (v76): both seats' answers go into the record.
            BeginGameRecord(ref a0, ref a1, d0, d1, regionalWatch || learningBot != null);

            if (boardScripts == null || (boardScriptsUser != null && !boardScriptsUser.Finished)) boardScripts = StandardCards.CreateRegistry();
            var engine = new GameEngine(boardScripts, a0, a1);
            engine.CheckInvariants = true;
            engine.CardsPerPlayer = d0.Count == d1.Count ? d0.Count : 0;
            // Custom formats (v123): the Prize cards and rules of the format (Regional Sim games use the normal ones).
            if (reg == null)
            {
                var gameFormat = MenuGameFormat(mine);
                engine.PrizeCount = gameFormat.PrizeCount;
                engine.Rules = gameFormat.Rules;
            }
            if (humanLog != null) humanLog.Engine = engine;
            eventLog = new EventCollector();
            engine.AddObserver(eventLog);
            gameEndHandled = false;
            finalView = null;
            learnStatus = null;
            // Short names keep the battle log readable ("Opponent's Dreepy", "Your Dragapult ex").
            string youName = regionalWatch ? reg.NameA : watching ? (seat == 0 ? "Bot 1" : "Bot 2") : "You";
            string themName = regionalWatch ? reg.NameB : watching ? (seat == 0 ? "Bot 2" : "Bot 1") : "Opponent";
            engine.PlayerNames = seat == 0 ? new[] { youName, themName } : new[] { themName, youName };
            // Your birthday from the menu (_____'s Pikachu); a bot never has a birthday.
            engine.Birthdays = new int[2];
            if (!watching) engine.Birthdays[seat] = birthday;
            // Best of 3: the loser of the previous game chooses to go first or second.
            engine.OrderChooser = reg != null ? reg.ChooserSeat : -1;
            // Game 1 of a top cut match: the higher seed chooses (v70).
            engine.OrderChooserIsHigherSeed = reg != null && reg.HigherSeedChooses;
            // A Tie-Breaker game after a drawn top cut game (v76).
            engine.TieBreaker = reg != null && reg.TieBreaker;
            FillGameRecord(engine, currentSeed, seat == 0 ? mine.Name : theirs.Name, seat == 0 ? theirs.Name : mine.Name);
            deckNames[seat] = mine.Name;
            deckNames[1 - seat] = theirs.Name;

            viewerSeat = watching ? -1 : seat;
            board.BottomSeat = viewerSeat < 0 ? seat : viewerSeat;
            board.Clear();
            host = new EngineHost(engine, human, viewerSeat);
            boardScriptsUser = host;
            host.Start(d0, d1, currentSeed);
            // v134.7: in your own Regional Sim match you find out your opponent's deck by playing it.
            string theirLabel = reg != null && !reg.Watch ? "your opponent's deck" : theirs.Name;
            log.Add((reg != null ? "Regional Sim, " + reg.Label + ": " : "New game: ") + mine.Name + " vs " + theirLabel + " (seed " + currentSeed + ").");
            mode = Mode.Game;
        }

        /// <summary>
        /// The deck name the game screen shows for a seat. v134.7: in your own Regional Sim match the opponent's stays hidden
        /// (deckNames keeps it for the AI's learning and the game log file).
        /// </summary>
        private string ShownDeckName(int seat)
        {
            if (regionalGame && !regionalWatch && viewerSeat >= 0 && seat != viewerSeat) return null;
            return deckNames[seat];
        }

        private void StopGame()
        {
            gameRecord = null;
            CancelReview();
            finalView = null;
            if (host != null) host.Stop();
            host = null;
            human = null;
            current = null;
            shownRequest = null;
            ResetScreens();
        }

        private void OnDestroy()
        {
            StopGame();
            ShutDownRegional();
        }

        private void OnApplicationQuit()
        {
            FinishRatedGame(null); // v157: a game whose review hadn't finished is still rated (by its result)
            StopGame();
            ShutDownRegional();
            if (training != null && training.Running)
            {
                training.Cancel();
                training.Join(2000);
            }
        }

        // ------------------------------------------------------------------ Update

        private void Update()
        {
            CardVisuals.PumpLoads(); // v134.6: card images decoded in the background are finished a couple per frame
            if (pendingStart)
            {
                pendingStart = false;
                if (myDeck >= 0 && oppDeck >= 0)
                {
                    PlayerPrefs.SetString(PrefMyDeck, decks[myDeck].Name);
                    PlayerPrefs.SetString(PrefOppDeck, decks[oppDeck].Name);
                    StartGame(decks[myDeck], decks[oppDeck]);
                }
            }
            if (pendingRestart)
            {
                pendingRestart = false;
                if (lastMine != null && lastTheirs != null) StartGame(lastMine, lastTheirs);
            }
            if (pendingMenu)
            {
                pendingMenu = false;
                if (regionalGame)
                {
                    // A Regional Sim game you leave isn't counted: it's played again from the event screen.
                    BackToRegionalHub();
                }
                else
                {
                    StopGame();
                    board.Clear();
                    RefreshDecks();
                    mode = Mode.Menu;
                }
            }
            if (pendingBuilder)
            {
                pendingBuilder = false;
                if (builder == null) builder = new DeckBuilderScreen(db);
                else builder.Reload();
                builder.Brain = brain;
                builder.WantsClose = false;
                mode = Mode.Builder;
            }
            if (pendingTrain)
            {
                pendingTrain = false;
                OpenTrainScreen();
            }
            if (pendingFormats)
            {
                pendingFormats = false;
                OpenFormats(false);
            }
            if (mode == Mode.Builder && builder != null && builder.WantsFormats)
            {
                builder.WantsFormats = false;
                OpenFormats(true);
            }
            if (mode == Mode.Formats && formatEditor != null && formatEditor.WantsClose)
            {
                formatEditor.WantsClose = false;
                if (formatsFromBuilder && builder != null)
                {
                    builder.Reload();
                    mode = Mode.Builder;
                }
                else
                {
                    RefreshDecks();
                    mode = Mode.Menu;
                }
            }
            UpdateTraining();
            UpdateLogStudy();
            UpdateFolders();
            UpdateRating();
            UpdateRegional();
            UpdateVideo();
            if (mode == Mode.Builder && builder != null && builder.WantsClose)
            {
                builder.WantsClose = false;
                builder.StopAdvisor();
                RefreshDecks();
                mode = Mode.Menu;
            }

            if (mode == Mode.Game) UpdateGame();
            PowerSaver.Tick(AiWorking(), mode == Mode.Game || mode == Mode.Video); // v120: the battery saver
            SyncRegionalTables();
        }

        /// <summary>Training, the Regional Sim's AI tables or a deck trial are playing in the background.</summary>
        private bool AiWorking()
        {
            if (training != null && training.Running) return true;
            var sim = regional;
            if (sim != null && !regionalPaused && !sim.Finished && sim.WorkersRunning) return true;
            return builder != null && builder.TrialRunning;
        }

        /// <summary>The black screen's line: what the AI is doing.</summary>
        private string AiWorkLine()
        {
            if (training != null && training.Running)
            {
                var st = training.Status();
                return "Training the AI: " + st.Done + " / " + st.Games + " games";
            }
            var view = regionalView;
            if (regional != null && view != null && !view.Finished)
                return "Regional Sim: round " + view.Round + ", " + view.AiGames + " AI games played";
            if (builder != null && builder.TrialRunning) return "Trying the deck's changes";
            return "The AI is working";
        }

        /// <summary>
        /// v120: a phone or tablet put the app in the background (or turned the screen off): the AI work waits (battery saver
        /// Auto or On) and what it learned is saved now, as Android may close a background app without warning.
        /// </summary>
        private void OnApplicationPause(bool paused)
        {
            PowerSaver.AppPaused(paused);
            if (!paused || !Application.isMobilePlatform) return;
            SaveBrainInBackground();
            if (regional != null) SaveRegional();
        }

        private void UpdateGame()
        {
            if (host == null) return;

            HostMessage msg;
            // A mulligan screen holds the game until you press Done.
            while (mulliganSeat < 0 && Time.time >= nextEventTime && host.TryDequeue(out msg))
            {
                if (msg.Error != null)
                {
                    errorText = msg.Error;
                    Debug.LogError("[PokemonTCG] " + msg.Error);
                    break;
                }
                var ev = msg.Event;
                AddLog(ev);
                current = msg.View;
                board.Apply(current);
                if (ev.Type == GameEventType.MulliganReveal && ev.Player >= 0)
                {
                    OpenMulliganScreen(ev.Player, msg.View);
                    nextEventTime = Time.time;
                    break;
                }
                float delay = DelayFor(ev);
                float show = RevealTimeFor(ev);
                if (show > 0f)
                {
                    StartReveal(ev, show);
                    delay = Mathf.Max(delay, show);
                }
                // Every coin flip is shown as a coin tossed in the middle of the screen (GameBootstrapCoin.cs).
                if (ev.Type == GameEventType.CoinFlip) delay = Mathf.Max(delay, StartCoin(ev));
                nextEventTime = Time.time + delay;
                if (delay > 0f) break;
            }

            if (human != null)
            {
                var req = human.Pending;
                if (req != null && host.QueuedCount == 0 && Time.time >= nextEventTime && mulliganSeat < 0)
                {
                    if (req != shownRequest && req != lastSubmitted)
                    {
                        shownRequest = req;
                        multiSelection.Clear();
                        filterCardId = int.MinValue;
                        optionScroll = Vector2.zero;
                        OnNewRequest(req);
                        current = req.View;
                        board.Apply(current);
                    }
                }
                else if (req == null)
                {
                    shownRequest = null;
                }
            }

            // The game is over and fully shown: the AI learns from it and Areas of Improvement starts.
            if (!gameEndHandled && host.Finished && host.QueuedCount == 0 && errorText == null && current != null && current.IsOver)
            {
                gameEndHandled = true;
                finalView = current;
                SaveGameRecord();
                OnGameFinished();
                if (regionalGame) ReportRegionalGame(finalView);
            }
            UpdateReview();

            board.SetHovered(dragging ? null : hovered, dragging ? pressId : int.MinValue);
        }

        private float DelayFor(GameEvent e)
        {
            float d;
            switch (e.Type)
            {
                case GameEventType.Info:
                case GameEventType.Shuffle:
                case GameEventType.Search:
                case GameEventType.TurnEnd:
                    d = secondsPerEvent * 0.4f;
                    break;
                case GameEventType.Attack:
                case GameEventType.KnockOut:
                case GameEventType.TurnStart:
                case GameEventType.GameOver:
                    d = secondsPerEvent * 2f;
                    break;
                default:
                    d = secondsPerEvent;
                    break;
            }
            // The opponent's turn plays a little slower than your own so you can follow it.
            if (viewerSeat >= 0 && e.Player >= 0 && e.Player != viewerSeat) d *= 1.3f;
            return d;
        }

        // ------------------------------------------------------------------ Battle log

        private void AddLog(GameEvent ev)
        {
            if (ev == null || string.IsNullOrEmpty(ev.Text)) return;
            string line = Colorize(ev.Text);
            switch (ev.Type)
            {
                case GameEventType.TurnStart:
                    line = "<b>——— " + line + " ———</b>";
                    break;
                case GameEventType.Attack:
                case GameEventType.KnockOut:
                case GameEventType.GameOver:
                    line = "<b>" + line + "</b>";
                    break;
                case GameEventType.Draw:
                case GameEventType.Shuffle:
                case GameEventType.Search:
                    line = "<color=#A9ADB8>" + line + "</color>";
                    break;
            }
            log.Add(line);
            if (log.Count > 400) log.RemoveAt(0);
            logScroll.y = float.MaxValue;
        }

        /// <summary>Colours each player's name (and "Your") so it's easy to see who did what.</summary>
        private string Colorize(string text)
        {
            if (host == null) return text;
            int mineSeat = viewerSeat >= 0 ? viewerSeat : (board != null ? board.BottomSeat : 0);
            for (int seat = 0; seat < 2; seat++)
            {
                string name = host.Engine.PlayerName(seat);
                bool mine = seat == mineSeat;
                string col = mine ? MineColor : TheirColor;
                text = Regex.Replace(text, @"\b" + Regex.Escape(name) + @"('s)?\b", m => "<color=" + col + ">" + m.Value + "</color>");
                if (name == "You") text = Regex.Replace(text, @"\bYour\b", "<color=" + col + ">Your</color>");
            }
            return text;
        }

        // ------------------------------------------------------------------ GUI

        private void OnGUI()
        {
            if (PowerSaver.NoteInput(Event.current)) return; // v120: a tap on the black screen only wakes it
            UiScale.HandleShortcuts(Event.current); // v118.2: Ctrl+0 / Ctrl+minus / Ctrl+plus on every screen
            UiScale.Apply(); // v118: bigger UI on phones and tablets (or the menu's UI size)
            if (PowerSaver.Dark)
            {
                PowerSaver.DrawDark(AiWorkLine());
                return;
            }
            var ui = UiStyles.Get();
            switch (mode)
            {
                case Mode.Menu:
                    DrawMenu(ui);
                    break;
                case Mode.Builder:
                    if (builder != null) builder.Draw();
                    break;
                case Mode.Formats:
                    if (formatEditor != null) formatEditor.Draw();
                    break;
                case Mode.Train:
                    DrawTrain(ui);
                    break;
                case Mode.Folders:
                    DrawFolders(ui);
                    break;
                case Mode.Rating:
                    DrawRating(ui);
                    break;
                case Mode.Regional:
                    DrawRegional(ui);
                    break;
                case Mode.Video:
                    DrawVideo(ui);
                    break;
                default:
                    DrawGame(ui);
                    break;
            }
        }

        private void DrawMenu(UiStyles ui)
        {
            float w = Mathf.Min(900f, UiScale.Width - 40f);
            var r = new Rect(UiScale.Width / 2f - w / 2f, 30f, w, UiScale.Height - 60f);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 16, r.y + 16, r.width - 32, r.height - 32));
            // v118.2: the whole menu scrolls, so every row (UI size included) can be reached on a short screen.
            menuScroll = GUILayout.BeginScrollView(menuScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            GUILayout.Label("Pokémon TCG (offline)", ui.Header);
            GUILayout.Label("Pick your deck and the deck the bot plays. Build or import your own in the deck builder.", ui.Label);
            GUILayout.Space(8);

            // v123: the format to play. Its decks are listed, and the game uses its Prize cards and rules.
            var formatNames = new List<string> { "All decks" };
            formatNames.AddRange(GameFormat.BuiltIns.Select(f => f.Name));
            formatNames.AddRange(CustomFormats.All.Select(f => f.Name));
            int formatPick = menuFormat.Length == 0 ? 0 : Mathf.Max(0, formatNames.FindIndex(n => n.Equals(menuFormat, StringComparison.OrdinalIgnoreCase)));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Format", GUILayout.Width(80));
            int newFormatPick = GUILayout.SelectionGrid(formatPick, formatNames.ToArray(), Mathf.Min(5, formatNames.Count));
            if (GUILayout.Button("Formats...", GUILayout.Width(100))) pendingFormats = true;
            GUILayout.EndHorizontal();
            var playFormat = menuFormat.Length > 0 ? GameFormat.Named(menuFormat) : null;
            if (playFormat != null && playFormat.IsCustom) GUILayout.Label(DeckBuilderScreen.FormatSummary(playFormat.Custom), Hint(ui));
            else if (playFormat == null) GUILayout.Label("Every deck. A game plays by your deck's format (a custom format's Prize cards and rules too).", Hint(ui));

            int pickMine = myDeck, pickTheirs = oppDeck;
            float colW = (r.width - 64) / 2f;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label(botVsBot ? "Bot 1's deck" : "Your deck", ui.Title);
            myScroll = GUILayout.BeginScrollView(myScroll, GUILayout.Height(Mathf.Max(160f, r.height - 382f)));
            for (int i = 0; i < decks.Count; i++)
                if (InMenuFormat(i) && GUILayout.Toggle(myDeck == i, DeckLabel(i), ui.Button) && myDeck != i) pickMine = i;
            if (!Enumerable.Range(0, decks.Count).Any(InMenuFormat)) GUILayout.Label("No decks for this format yet. Build one in the deck builder.", Hint(ui));
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(colW));
            GUILayout.Label(botVsBot ? "Bot 2's deck" : "Opponent's deck (bot)", ui.Title);
            oppScroll = GUILayout.BeginScrollView(oppScroll, GUILayout.Height(Mathf.Max(160f, r.height - 382f)));
            for (int i = 0; i < decks.Count; i++)
                if (InMenuFormat(i) && GUILayout.Toggle(oppDeck == i, DeckLabel(i), ui.Button) && oppDeck != i) pickTheirs = i;
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Space(6);
            botVsBot = GUILayout.Toggle(botVsBot, " Watch two bots play each other");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Opponent", GUILayout.Width(80));
            int aiPick = GUILayout.Toolbar(OpponentPick, OpponentChoices, GUILayout.Width(440));
            GUILayout.Label("  " + BrainLine(), Hint(ui));
            GUILayout.EndHorizontal();
            DrawLookAheadRow(ui);
            // v157: your Elo rating.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Your Elo", GUILayout.Width(80));
            GUILayout.Label(RatingMenuLine(), ui.Label);
            if (GUILayout.Button("Your rating...", GUILayout.Width(130))) pendingRating = true;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Seed (blank = random)", GUILayout.Width(170));
            seedText = GUILayout.TextField(seedText ?? "", GUILayout.Width(120));
            GUILayout.Label("   Pace", GUILayout.Width(60));
            int newPace = GUILayout.Toolbar(pace, PaceNames, GUILayout.Width(240));
            GUILayout.EndHorizontal();
            int newBirthday = DrawBirthdayRow(ui);
            // v118: UI size (Auto makes the interface bigger on phones and tablets).
            GUILayout.BeginHorizontal();
            GUILayout.Label("UI size", GUILayout.Width(170));
            int uiPick = Mathf.Max(0, Array.IndexOf(UiScale.Choices, UiScale.Setting));
            int newUiPick = GUILayout.Toolbar(uiPick, UiScale.Choices.Select(UiScale.Label).ToArray(), GUILayout.Width(420));
            GUILayout.Label(UiScale.Capped ? "  This window fits " + UiScale.EffectivePercent + "%.  Ctrl+0: Auto" : "  Ctrl+0: Auto", Hint(ui));
            GUILayout.EndHorizontal();
            // v120: battery saver (Auto: on while unplugged).
            GUILayout.BeginHorizontal();
            GUILayout.Label("Battery saver", GUILayout.Width(170));
            int saverPick = GUILayout.Toolbar(PowerSaver.Mode, PowerSaver.ModeNames, GUILayout.Width(240));
            GUILayout.Label("  " + PowerSaver.StatusLine(), Hint(ui));
            GUILayout.EndHorizontal();
            var cpuActions = new List<Action>();
            DrawSkipCpuRow(ui, cpuActions, 170);
            GUILayout.Space(6);

            bool ready = myDeck >= 0 && oppDeck >= 0 && deckProblems[myDeck].Count == 0 && deckProblems[oppDeck].Count == 0
                         && InMenuFormat(myDeck) && InMenuFormat(oppDeck);
            GUILayout.BeginHorizontal();
            GUI.enabled = ready;
            if (GUILayout.Button("Start game", GUILayout.Height(34))) pendingStart = true;
            GUI.enabled = true;
            if (GUILayout.Button("Deck builder", GUILayout.Height(34))) pendingBuilder = true;
            if (GUILayout.Button("Train the AI", GUILayout.Height(34))) pendingTrain = true;
            if (GUILayout.Button("Regional Sim", GUILayout.Height(34))) pendingRegional = true;
            if (GUILayout.Button("Save folders", GUILayout.Height(34))) pendingFolders = true;
            GUILayout.EndHorizontal();

            if (!ready && myDeck >= 0 && oppDeck >= 0)
            {
                foreach (var i in new[] { myDeck, oppDeck }.Distinct())
                    foreach (var p in deckProblems[i]) GUILayout.Label(decks[i].Name + ": " + p, ui.Error);
            }
            if (menuError != null) GUILayout.Label(menuError, ui.Error);
            GUILayout.Label("Your saved decks live in " + SavePaths.DeckFolder + "; the AI's brain in " + SavePaths.BrainFolder + ". Change them with Save folders.", Hint(ui));
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            myDeck = pickMine;
            oppDeck = pickTheirs;
            if (newFormatPick != formatPick && newFormatPick >= 0 && newFormatPick < formatNames.Count)
            {
                menuFormat = newFormatPick == 0 ? "" : formatNames[newFormatPick];
                PlayerPrefs.SetString(PrefMenuFormat, menuFormat);
                KeepPicksInFormat();
            }
            if (newUiPick != uiPick) UiScale.Setting = UiScale.Choices[newUiPick];
            if (saverPick != PowerSaver.Mode) PowerSaver.Mode = saverPick;
            if (newPace != pace) ApplyPace(newPace);
            foreach (var a in cpuActions) a();
            if (aiPick != OpponentPick) SetOpponentPick(aiPick);
            if (newBirthday != birthday)
            {
                birthday = newBirthday;
                PlayerPrefs.SetInt(PrefBirthday, birthday);
            }
        }

        /// <summary>
        /// "Your birthday" (month and day) for _____'s Pikachu's Birthday Surprise. Returns the new value (month * 100 + day,
        /// 0 = not set); the caller stores it after the layout, so the controls drawn don't change during one GUI event.
        /// </summary>
        private int DrawBirthdayRow(UiStyles ui)
        {
            int month = birthday / 100, day = birthday % 100;
            bool showDay = month >= 1 && month <= 12;
            GUILayout.BeginHorizontal();
            GUILayout.Label("Your birthday", GUILayout.Width(170));
            if (GUILayout.Button("<", GUILayout.Width(26))) month = month <= 0 ? 12 : month - 1;
            GUILayout.Label(showDay ? MonthNames[month - 1] : "Not set", GUILayout.Width(80));
            if (GUILayout.Button(">", GUILayout.Width(26))) month = month >= 12 ? 0 : month + 1;
            if (showDay)
            {
                int maxDay = DaysInMonth[birthday / 100 - 1];
                if (day < 1 || day > maxDay) day = 1;
                if (GUILayout.Button("<", GUILayout.Width(26))) day = day <= 1 ? maxDay : day - 1;
                GUILayout.Label(day.ToString(), GUILayout.Width(28));
                if (GUILayout.Button(">", GUILayout.Width(26))) day = day >= maxDay ? 1 : day + 1;
            }
            string note = GameEngine.IsBirthday(birthday, DateTime.Today) ? "Happy birthday! _____'s Pikachu can flip for 50 more damage today."
                : "For _____'s Pikachu's Birthday Surprise (the bot never has a birthday).";
            GUILayout.Label("  " + note, Hint(ui));
            GUILayout.EndHorizontal();
            if (month < 1 || month > 12) return 0;
            int max = DaysInMonth[month - 1];
            if (day < 1) day = 1;
            if (day > max) day = max;
            return month * 100 + day;
        }

        private string DeckLabel(int i)
        {
            var problems = deckProblems[i];
            return decks[i].Label + (problems.Count == 0 ? "" : "   [not legal: " + problems.Count + " problem" + (problems.Count == 1 ? "" : "s") + "]");
        }

        private void DrawGame(UiStyles ui)
        {
            var e = Event.current;
            mouseGui = e.mousePosition;
            guiActions.Clear();
            bool modal = ModalOpen;
            if (e.type == EventType.Repaint)
                hovered = board != null && !modal && !dragging && !OverPanel(mouseGui) ? board.Pick(cam, mouseGui) : null;

            DrawCardNames(ui);
            DrawPokemonBadges(ui);
            DrawDeckCounts(ui);
            if (!modal) DrawHighlights(ui);

            GUI.enabled = !modal;
            DrawHud(ui);
            DrawLog(ui);
            panelRect = new Rect();
            if (!PickerVisible && !CoinCallVisible) DrawDecisionPanel(ui);
            endRect = new Rect();
            DrawEndPanels(ui);
            DrawReview(ui);
            GUI.enabled = true;

            if (!modal && !dragging && hovered != null && hovered.Def != null && Vector3.Dot(hovered.transform.up, Vector3.up) >= 0f)
                ui.DrawTooltip(CardText.Describe(hovered.Def), mouseGui);

            DrawDragGhost(ui);
            DrawReveal(ui);
            DrawCoin(ui);
            DrawPickerReturn(ui);

            // Pop-ups go on top of everything else.
            if (mulliganSeat >= 0) DrawMulliganScreen(ui);
            else if (CoinCallVisible) DrawCoinCall(ui);
            else if (PickerVisible) DrawCardPicker(ui);
            else if (pileSeat >= 0) DrawPileViewer(ui);
            else if (popupUid >= 0) DrawPokemonPopup(ui);

            if (!modal) HandleBoardInput(e, ui);
            else
            {
                pressing = false;
                dragging = false;
            }

            foreach (var a in guiActions) a();
            guiActions.Clear();
        }

        /// <summary>Over one of the IMGUI panels (clicks there shouldn't reach the board).</summary>
        private bool OverPanel(Vector2 p)
        {
            return hudRect.Contains(p) || logRect.Contains(p) || panelRect.Contains(p) || endRect.Contains(p) || reviewRect.Contains(p) || previewBarRect.Contains(p);
        }

        private void DrawHud(UiStyles ui)
        {
            // Compact (v55.2): it used to reach down over the opponent's deck and Bench on 1366 x 768 screens. The hint line
            // moved into the decision panel.
            hudRect = new Rect(10, 10, 470, 104);
            GUI.Box(hudRect, GUIContent.none);
            if (current != null)
            {
                var sb = new StringBuilder();
                if (current.InSetup) sb.Append("Setup");
                else sb.Append("Turn " + current.TurnNumber + " - " + (host != null ? host.Engine.Possessive(current.CurrentPlayer) : SeatName(current.CurrentPlayer)) + " turn");
                sb.Append("\n");
                for (int i = 0; i < 2; i++)
                {
                    var s = current.Sides[i];
                    sb.Append("<b>" + SeatName(i) + "</b>  hand " + s.HandCount + " · deck " + s.DeckCount + " · prizes " + s.PrizeCount + " · discard " + s.Discard.Count + (s.LostZone.Count > 0 ? " · Lost Zone " + s.LostZone.Count : "")
                              + (string.IsNullOrEmpty(ShownDeckName(i)) ? "" : "   <color=#A9ADB8>" + ShownDeckName(i) + "</color>") + "\n");
                }
                if (current.Stadium != null) sb.Append("Stadium: " + current.Stadium.Def.Name);
                if (hudStyle == null) hudStyle = new GUIStyle(ui.Label) { fontSize = 13, wordWrap = false, richText = true, clipping = TextClipping.Clip };
                GUI.Label(new Rect(18, 12, 455, 68), sb.ToString(), hudStyle);
            }
            if (GUI.Button(new Rect(18, 82, 120, 24), regionalGame ? "Back to event" : "Quit to menu")) pendingMenu = true;
            GUI.Label(new Rect(150, 83, 50, 22), "Pace", ui.Label);
            int newPace = GUI.Toolbar(new Rect(195, 82, 270, 24), pace, PaceNames);
            if (newPace != pace) guiActions.Add(() => ApplyPace(newPace));
        }

        private GUIStyle Hint(UiStyles ui)
        {
            if (hintStyle == null)
            {
                hintStyle = new GUIStyle(ui.Label) { fontSize = 12, wordWrap = true };
                hintStyle.normal.textColor = new Color(0.78f, 0.8f, 0.86f);
            }
            return hintStyle;
        }

        private string SeatName(int seat)
        {
            if (host == null || seat < 0) return "Nobody";
            return host.Engine.PlayerName(seat);
        }

        /// <summary>
        /// The battle log (v55.2). Closed (the default), its last 2 lines show in a thin strip along the bottom edge of the
        /// screen, under your hand, where it covers no cards. "Open log" shows the whole log, scrollable, in the bottom-left
        /// corner until you close it again; the choice is remembered.
        /// </summary>
        private void DrawLog(UiStyles ui)
        {
            if (logStyle == null)
            {
                logStyle = new GUIStyle(ui.Label) { richText = true, fontSize = 13, wordWrap = true };
                logStyle.margin = new RectOffset(2, 2, 1, 1);
            }
            if (logLineStyle == null) logLineStyle = new GUIStyle(logStyle) { wordWrap = false, clipping = TextClipping.Clip };

            if (!logOpen)
            {
                const float stripHeight = 44f;
                logRect = new Rect(10, UiScale.Height - stripHeight - 6, UiScale.Width - 20, stripHeight);
                GUI.Box(logRect, GUIContent.none);
                if (GUI.Button(new Rect(logRect.x + 6, logRect.y + 8, 84, 28), "Open log")) guiActions.Add(() => SetLogOpen(true));
                float textX = logRect.x + 98;
                float textWidth = logRect.width - 104;
                int n = log.Count;
                if (n >= 2) GUI.Label(new Rect(textX, logRect.y + 3, textWidth, 19), log[n - 2], logLineStyle);
                if (n >= 1) GUI.Label(new Rect(textX, logRect.y + 22, textWidth, 19), log[n - 1], logLineStyle);
                return;
            }

            float height = Mathf.Min(300f, UiScale.Height - 130f);
            logRect = new Rect(10, UiScale.Height - height - 6, Mathf.Min(470f, UiScale.Width - 20f), height);
            GUI.Box(logRect, GUIContent.none);
            if (GUI.Button(new Rect(logRect.xMax - 92, logRect.y + 4, 86, 22), "Close log")) guiActions.Add(() => SetLogOpen(false));
            GUILayout.BeginArea(new Rect(logRect.x + 6, logRect.y + 4, logRect.width - 12, logRect.height - 8));
            GUILayout.Label("<b>Battle log</b>", logStyle);
            logScroll = GUILayout.BeginScrollView(logScroll);
            foreach (var line in log) GUILayout.Label(line, logStyle);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void SetLogOpen(bool open)
        {
            logOpen = open;
            if (open) logScroll.y = float.MaxValue;
            PlayerPrefs.SetInt(PrefLogOpen, open ? 1 : 0);
        }

        private void DrawCardNames(UiStyles ui)
        {
            if (board == null || cam == null) return;
            foreach (var cv in board.Cards)
            {
                if (cv == null || cv.Def == null || board.IsTucked(cv.Id) || CardVisuals.HasBoardImage(cv.Def)) continue;
                if (Vector3.Dot(cv.transform.up, Vector3.up) < 0f) continue; // face down
                var r = cv.ScreenRect(cam);
                if (r.width < 24f) continue;
                GUI.Label(new Rect(r.x + 2, r.y + 2, r.width - 4, Mathf.Min(r.height - 4, 48f)), cv.Def.Name, ui.Small);
            }
        }

        /// <summary>
        /// Badges on each Pokémon in play: its HP in a box at the top-right corner (red when damaged), a row of Energy
        /// symbols along the bottom-left edge (hover one for its name), and a line under the card for Special Conditions,
        /// Tools and timed effects. The opponent's cards face them (upside down on screen), so their badges are placed the
        /// same way on the card as it faces its owner: HP at the screen's bottom-left, Energy along the top-right, the
        /// line above the card.
        /// </summary>
        private void DrawPokemonBadges(UiStyles ui)
        {
            if (current == null || board == null || cam == null) return;
            if (hpStyle == null)
            {
                hpStyle = new GUIStyle(ui.Badge) { richText = true, alignment = TextAnchor.MiddleCenter };
                hpStyle.normal.textColor = Color.black;
            }
            string hoverText = null;
            foreach (var side in current.Sides)
            {
                var all = new List<PokemonView>();
                if (side.Active != null) all.Add(side.Active);
                all.AddRange(side.Bench);
                bool facingAway = side.Player != board.BottomSeat;
                foreach (var p in all)
                {
                    var cv = board.Get(p.Top.Id);
                    if (cv == null) continue;
                    var r = cv.ScreenRect(cam);
                    if (r.width < 8f) continue;

                    // HP, top-right.
                    bool hurt = p.DamageCounters > 0;
                    int fontSize = Mathf.Clamp(Mathf.RoundToInt(r.height * 0.11f), 11, 22);
                    hpStyle.fontSize = fontSize;
                    string hp = hurt
                        ? "<color=#C81E1E>" + Mathf.Max(0, p.RemainingHp) + "</color><size=" + Mathf.Max(8, fontSize - 4) + ">/" + p.MaxHp + "</size>"
                        : p.MaxHp.ToString();
                    var hpSize = hpStyle.CalcSize(new GUIContent(hp));
                    var hpBox = facingAway
                        ? new Rect(r.x - hpSize.x * 0.25f - 4f, r.yMax - hpSize.y * 0.65f - 2f, hpSize.x + 10f, hpSize.y + 2f)
                        : new Rect(r.xMax - hpSize.x * 0.75f - 6f, r.y - hpSize.y * 0.35f, hpSize.x + 10f, hpSize.y + 2f);
                    CardDraw.Fill(new Rect(hpBox.x - 1f, hpBox.y - 1f, hpBox.width + 2f, hpBox.height + 2f), new Color(0f, 0f, 0f, 0.8f));
                    CardDraw.Fill(hpBox, Color.white);
                    GUI.Label(hpBox, hp, hpStyle);

                    // Energy symbols, bottom-left.
                    float d = Mathf.Clamp(r.width * 0.2f, 14f, 30f);
                    if (p.Energy.Count > 0)
                    {
                        float step = d * 0.9f;
                        float maxRow = r.width * 1.25f;
                        if (step * (p.Energy.Count - 1) + d > maxRow) step = Mathf.Max(d * 0.35f, (maxRow - d) / (p.Energy.Count - 1));
                        for (int k = 0; k < p.Energy.Count; k++)
                        {
                            var icon = facingAway
                                ? new Rect(r.xMax + d * 0.25f - d - step * k, r.y - d * 0.4f, d, d)
                                : new Rect(r.x - d * 0.25f + step * k, r.yMax - d * 0.6f, d, d);
                            CardDraw.EnergyIcon(icon, p.Energy[k].Def);
                            if (icon.Contains(mouseGui)) hoverText = p.Energy[k].Def.Name + (p.Energy[k].Def.IsSpecialEnergy ? "\n" + p.Energy[k].Def.Text : "");
                        }
                    }

                    // Conditions, Tools and effects, under the card.
                    var parts = new List<string>();
                    if (p.Conditions != SpecialCondition.None) parts.Add(p.Conditions.ToString());
                    foreach (var tool in p.Tools) parts.Add("Tool: " + tool.Def.Name);
                    if (p.Notes.Count > 0) parts.Add("(" + string.Join("; ", p.Notes.ToArray()) + ")");
                    if (parts.Count > 0)
                    {
                        string text = string.Join("  ", parts.ToArray());
                        var size = ui.Badge.CalcSize(new GUIContent(text));
                        var badge = facingAway
                            ? new Rect(r.center.x - size.x / 2f - 4, r.y - d * 0.45f - 20f, size.x + 8, 20)
                            : new Rect(r.center.x - size.x / 2f - 4, r.yMax + d * 0.45f, size.x + 8, 20);
                        GUI.DrawTexture(badge, p.Conditions != SpecialCondition.None ? ui.BadgeBackground : CardVisuals.Solid(new Color(0.05f, 0.08f, 0.1f, 0.85f)));
                        GUI.Label(badge, text, ui.Badge);
                    }
                }
            }
            if (hoverText != null && !ModalOpen) ui.DrawTooltip(hoverText, mouseGui);
        }

        /// <summary>
        /// How many cards are left in each deck (shown for both players, even at 0), on the side of the deck away from the
        /// discard pile: under your deck, and above your opponent's (under it as they see it). If the score box covers that
        /// spot, the opponent's count goes beside their deck instead. Each Lost Zone gets a "Lost Zone" label too, with its
        /// card count once it has cards.
        /// </summary>
        private void DrawDeckCounts(UiStyles ui)
        {
            if (current == null || board == null || cam == null) return;
            string markerTip = null;
            foreach (var side in current.Sides)
            {
                if (side.Player < 0 || side.Player > 1) continue;
                var r = board.DeckScreenRect(cam, side.Player);
                string text = side.DeckCount == 1 ? "1 card" : side.DeckCount + " cards";
                var size = ui.Badge.CalcSize(new GUIContent(text));
                Rect badge;
                if (side.Player == board.BottomSeat) badge = new Rect(r.center.x - size.x / 2f - 4, r.yMax + 2, size.x + 8, 20);
                else
                {
                    badge = new Rect(r.center.x - size.x / 2f - 4, r.y - 22, size.x + 8, 20);
                    if (badge.Overlaps(hudRect)) badge = new Rect(r.xMax + 4, r.center.y - 10, size.x + 8, 20);
                }
                GUI.DrawTexture(badge, CardVisuals.Solid(new Color(0.05f, 0.08f, 0.1f, 0.85f)));
                GUI.Label(badge, text, ui.Badge);

                // The Lost Zone's name and size (v49), beside its sideways pile on the side towards the middle of the table (v69),
                // clear of the Prize cards and the discard piles.
                var lz = board.LostZoneScreenRect(cam, side.Player);
                string lzText = "Lost Zone" + (side.LostZone.Count > 0 ? " · " + side.LostZone.Count : "");
                var lzSize = ui.Badge.CalcSize(new GUIContent(lzText));
                var lzBadge = side.Player == board.BottomSeat
                    ? new Rect(lz.xMax + 4, lz.center.y - 10, lzSize.x + 8, 20)
                    : new Rect(lz.x - lzSize.x - 12, lz.center.y - 10, lzSize.x + 8, 20);
                GUI.DrawTexture(lzBadge, CardVisuals.Solid(new Color(0.08f, 0.1f, 0.18f, 0.85f)));
                GUI.Label(lzBadge, lzText, ui.Badge);

                // The VSTAR and GX markers: what they mean, on hover.
                if (!ModalOpen)
                {
                    if (board.MarkerScreenRect(cam, side.Player, true).Contains(mouseGui))
                        markerTip = SeatName(side.Player) + "'s VSTAR marker: " + (side.VstarUsed ? "VSTAR Power used (only 1 per game)." : "VSTAR Power not used yet.");
                    else if (board.MarkerScreenRect(cam, side.Player, false).Contains(mouseGui))
                        markerTip = SeatName(side.Player) + "'s GX marker: " + (side.GxUsed ? "GX attack used (only 1 per game)." : "GX attack not used yet.");
                }
            }
            if (markerTip != null) ui.DrawTooltip(markerTip, mouseGui);
        }

        /// <summary>A face-down Prize card on the board that this Prize option stands for (Index = its slot).</summary>
        private static bool PrizeOptionTouches(DecisionRequest req, DecisionOption o, int cardId)
        {
            return req.Kind == DecisionKind.ChoosePrize && o.CardId < 0 && o.Index >= 0 && cardId == BoardView.PrizeCardId(req.Player, o.Index);
        }

        private void DrawHighlights(UiStyles ui)
        {
            if (shownRequest == null || board == null || cam == null || dragging) return;
            var ids = new HashSet<int>();
            foreach (var o in shownRequest.Options)
            {
                if (o.CardId >= 0) ids.Add(o.CardId);
                if (shownRequest.Kind == DecisionKind.ChoosePrize && o.CardId < 0 && o.Index >= 0) ids.Add(BoardView.PrizeCardId(shownRequest.Player, o.Index));
                if (o.SameCardIds != null) foreach (var same in o.SameCardIds) ids.Add(same);
                if (o.PokemonUid >= 0 && current != null)
                {
                    var p = current.FindPokemon(o.PokemonUid);
                    if (p != null) ids.Add(p.Top.Id);
                }
            }
            foreach (var id in ids)
            {
                var cv = board.Get(id);
                if (cv == null) continue;
                bool selected = id == filterCardId || shownRequest.Options.Where((o, i) => multiSelection.Contains(i)).Any(o => o.CardId == id);
                CardDraw.Frame(cv.ScreenRect(cam), selected ? ui.Selected : ui.Highlight, 3f);
            }
        }

        private void DrawDecisionPanel(UiStyles ui)
        {
            var req = shownRequest;
            if (req == null || human == null) return;
            // Folded down to its title bar (v55.2), so the cards behind it (your deck and discard pile, the opponent's Prize
            // cards on smaller screens) can be seen and clicked. A new choice that isn't your main action unfolds it.
            if (panelHidden)
            {
                panelRect = new Rect(UiScale.Width - 370, 10, 360, 40);
                GUI.Box(panelRect, GUIContent.none);
                string title = req.Prompt ?? "";
                if (title.Length > 34) title = title.Substring(0, 33) + "...";
                GUI.Label(new Rect(panelRect.x + 10, panelRect.y + 9, panelRect.width - 110, 22), title, ui.Label);
                if (GUI.Button(new Rect(panelRect.xMax - 92, panelRect.y + 8, 84, 24), "Show")) guiActions.Add(() => panelHidden = false);
                return;
            }
            panelRect = new Rect(UiScale.Width - 370, 10, 360, Mathf.Min(UiScale.Height - 70, 620));
            var area = panelRect;
            GUI.Box(area, GUIContent.none);
            // The Hide button sits on its own row at the top, so a long prompt never runs under it.
            if (GUI.Button(new Rect(area.xMax - 64, area.y + 4, 56, 20), "Hide")) guiActions.Add(() => panelHidden = true);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 26, area.width - 16, area.height - 34));
            GUILayout.Label(req.Prompt, ui.Title);
            if (req.Source != null) GUILayout.Label("From: " + req.Source.Name, ui.Label);
            if (req.Kind == DecisionKind.MainAction && filterCardId == int.MinValue)
                GUILayout.Label("Drag a card from your hand onto the board or a Pokémon to play it. Click a Pokémon to see its attacks and Abilities, or a discard pile to see it.", Hint(ui));
            else if (req.Kind == DecisionKind.SetupActive || req.Kind == DecisionKind.SetupBench)
                GUILayout.Label("Drag a Basic Pokémon from your hand onto the board. Clicking a card shows its option below.", Hint(ui));
            else if (req.Kind == DecisionKind.ChoosePrize)
                GUILayout.Label("Click one of your Prize cards on the table, or pick one below.", Hint(ui));
            bool multi = !req.IsSingleChoice;
            if (multi) GUILayout.Label("Pick " + (req.MinChoices == req.MaxChoices ? req.MinChoices.ToString() : req.MinChoices + " to " + req.MaxChoices) + "  (selected " + multiSelection.Count + ")", ui.Label);
            if (IsCardChoice(req) && pickerHidden && GUILayout.Button("Show the cards")) guiActions.Add(() => pickerHidden = false);

            // IMGUI lays controls out twice per event, so state changes are applied after drawing.
            bool showAll = false;
            bool confirm = false;
            int clicked = -1;
            int filter = filterCardId;
            if (filter != int.MinValue)
            {
                GUILayout.Label("Options for the selected card:", ui.Label);
                if (GUILayout.Button("Show all options")) showAll = true;
            }

            optionScroll = GUILayout.BeginScrollView(optionScroll);
            for (int i = 0; i < req.Options.Count; i++)
            {
                var o = req.Options[i];
                if (filter != int.MinValue && !OptionTouchesCard(o, filter)) continue;
                if (multi)
                {
                    bool on = multiSelection.Contains(i);
                    bool now = GUILayout.Toggle(on, " " + o.Label);
                    if (now != on)
                    {
                        int idx = i;
                        if (now) guiActions.Add(() => { if (multiSelection.Count < req.MaxChoices) multiSelection.Add(idx); });
                        else guiActions.Add(() => multiSelection.Remove(idx));
                    }
                }
                else if (GUILayout.Button(o.Label, ui.Button))
                {
                    clicked = i;
                }
            }
            GUILayout.EndScrollView();

            if (multi)
            {
                bool ok = multiSelection.Count >= req.MinChoices && multiSelection.Count <= req.MaxChoices;
                GUI.enabled = ok && !ModalOpen;
                if (GUILayout.Button("Confirm")) confirm = true;
                GUI.enabled = !ModalOpen;
            }
            GUILayout.EndArea();

            if (showAll) guiActions.Add(() => filterCardId = int.MinValue);
            if (clicked >= 0)
            {
                int pick = clicked;
                guiActions.Add(() =>
                {
                    // Options listed for a card you clicked in your hand: play that very copy.
                    if (filter != int.MinValue) req.Options[pick].UseCopy(filter);
                    Submit(new List<int> { pick });
                });
            }
            else if (confirm) guiActions.Add(() => Submit(multiSelection.OrderBy(x => x).ToList()));
        }

        private bool OptionTouchesCard(DecisionOption o, int cardId)
        {
            if (o.UsesCard(cardId)) return true;
            if (o.PokemonUid >= 0 && current != null)
            {
                var p = current.FindPokemon(o.PokemonUid);
                if (p != null && p.Top.Id == cardId) return true;
            }
            return false;
        }

        /// <summary>A click on a card on the board while a decision is waiting (not used for main actions' Pokémon, which open the pop-up).</summary>
        private void OnCardClicked(CardView cv)
        {
            var req = shownRequest;
            if (req == null || human == null) return;
            var matching = new List<int>();
            for (int i = 0; i < req.Options.Count; i++)
                if (OptionTouchesCard(req.Options[i], cv.Id) || PrizeOptionTouches(req, req.Options[i], cv.Id)) matching.Add(i);
            if (matching.Count == 0) return;

            if (!req.IsSingleChoice)
            {
                int i = matching[0];
                if (multiSelection.Contains(i)) multiSelection.Remove(i);
                else if (multiSelection.Count < req.MaxChoices) multiSelection.Add(i);
                return;
            }
            // A single click never plays a card from your hand: it lists what that card can do (drag it to play it).
            // That includes setting up your Active Pokémon and Bench. Other choices (a Pokémon to target...) take a click.
            if (!IsDragDecision(req) && matching.Count == 1)
            {
                Submit(matching);
                return;
            }
            filterCardId = cv.Id;
            optionScroll = Vector2.zero;
        }

        private void Submit(List<int> choice)
        {
            var h = human;
            lastSubmitted = shownRequest;
            shownRequest = null;
            multiSelection.Clear();
            filterCardId = int.MinValue;
            popupUid = -1;
            if (h != null) h.Submit(choice);
        }

        private void DrawEndPanels(UiStyles ui)
        {
            if (errorText != null)
            {
                var r = new Rect(UiScale.Width / 2f - 300, UiScale.Height / 2f - 160, 600, 320);
                endRect = r;
                GUI.Box(r, GUIContent.none);
                GUILayout.BeginArea(new Rect(r.x + 10, r.y + 10, r.width - 20, r.height - 20));
                GUILayout.Label("Something went wrong in the engine", ui.Title);
                GUILayout.Label(errorText.Length > 900 ? errorText.Substring(0, 900) + "..." : errorText, ui.Label);
                GUILayout.BeginHorizontal();
                if (regionalGame)
                {
                    // Not counted for the event: play it again or go back.
                    if (GUILayout.Button("Play this game again")) pendingRegionalGame = true;
                    if (GUILayout.Button("Back to the event")) pendingRegionalHub = true;
                }
                else
                {
                    if (GUILayout.Button("Play again")) pendingRestart = true;
                    if (GUILayout.Button("Back to menu")) pendingMenu = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
                return;
            }
            var over = finalView ?? current;
            if (over != null && over.IsOver && (host == null || host.QueuedCount == 0) && !(reviewOpen && analyzer != null))
            {
                float h = (analyzer != null ? 184 : 150) + (regionalGame ? 46 : 0) + 24;
                var r = new Rect(UiScale.Width / 2f - 210, UiScale.Height / 2f - 70, 420, h);
                endRect = r;
                GUI.Box(r, GUIContent.none);
                GUILayout.BeginArea(new Rect(r.x + 10, r.y + 10, r.width - 20, r.height - 20));
                GUILayout.Label(over.Winner < 0 ? "Draw" : SeatName(over.Winner) + (SeatName(over.Winner) == "You" ? " won!" : " won."), ui.Title);
                GUILayout.Label(over.EndReason, ui.Label);
                if (regionalGame && !string.IsNullOrEmpty(regionalGameResultText)) GUILayout.Label(regionalGameResultText, ui.Good);
                GUILayout.Label(RatingEndLine(), ui.Small); // v157
                if (analyzer != null && GUILayout.Button("Areas of Improvement")) guiActions.Add(() => reviewOpen = true);
                GUILayout.BeginHorizontal();
                if (regionalGame)
                {
                    // Regional Sim: the next game of the match, or back to the event (a game can't be replayed once it counted).
                    if (regionalNextGame > 0 && GUILayout.Button((regionalWatch ? "Watch game " : "Play game ") + regionalNextGame)) pendingRegionalGame = true;
                    if (GUILayout.Button("Back to the event")) pendingRegionalHub = true;
                }
                else
                {
                    if (GUILayout.Button("Play again")) pendingRestart = true;
                    if (GUILayout.Button("Back to menu")) pendingMenu = true;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }
}
