using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Regional Sim (v57): a Swiss event of learning AIs (RegionalSim), with you in it or AI only.
    ///  - Setup: players, rounds, best of 1 or 3, Standard or Expanded, your deck (the AI seats pick theirs at random from the
    ///    legal decks of that format), and whether the AI games explore while learning.
    ///  - The event screen: your match (or, AI only, the table picked to watch this round), the round's progress, standings
    ///    with tiebreakers, every round's pairings and results, and how each deck is doing.
    ///  - Your games and the watched tables are played on the board; every other table plays in the background and the
    ///    next round is paired only when the whole round is over.
    ///  - The event is saved in the AI folder (regional.txt) and picks up where it was when you come back.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string RegionalFileName = "regional.txt";
        private const string PrefRegionalAutoWatch = "PokemonTCG.RegionalAutoWatch";
        // The setup screen remembers your last event's settings (v57.2).
        private const string PrefRegPlayers = "PokemonTCG.RegionalPlayers";
        private const string PrefRegRounds = "PokemonTCG.RegionalRounds";
        private const string PrefRegBestOf3 = "PokemonTCG.RegionalBestOf3";
        private const string PrefRegExpanded = "PokemonTCG.RegionalExpanded";
        private const string PrefRegAiOnly = "PokemonTCG.RegionalAiOnly";
        private const string PrefRegExplore = "PokemonTCG.RegionalExplore";
        private const string PrefRegWatch = "PokemonTCG.RegionalWatch";
        private const string PrefRegField = "PokemonTCG.RegionalField";
        private const string PrefRegFillRandom = "PokemonTCG.RegionalFillRandom";
        private const string PrefRegFieldPct = "PokemonTCG.RegionalFieldPercent";
        private const string PrefRegFieldByPct = "PokemonTCG.RegionalFieldByPercent";
        private const string PrefRegTopCut = "PokemonTCG.RegionalTopCut";
        private const string PrefRegCutBestOf3 = "PokemonTCG.RegionalCutBestOf3";
        private const string PrefRegTables = "PokemonTCG.RegionalTablesAtOnce";
        private const string PrefRegDay2 = "PokemonTCG.RegionalDay2";
        private const string PrefRegDay2Min = "PokemonTCG.RegionalDay2MinPlayers";
        private const string PrefRegDay2Rounds = "PokemonTCG.RegionalDay2Rounds";
        private const string PrefRegDay2Points = "PokemonTCG.RegionalDay2Points";
        private const int MaxDay2Rounds = 9;
        private const string PrefRegAsymCut = "PokemonTCG.RegionalAsymmetricCut";
        private static readonly string[] RegionalTabs = { "Standings", "Pairings & results", "Decks" };
        private static readonly string[] RegionalTabsWithCut = { "Standings", "Pairings & results", "Decks", "Top cut" };
        /// <summary>Top cut sizes on the setup screen (v70): none, top 2, top 4, top 8.</summary>
        private static readonly int[] RegionalCutSizes = { 0, 2, 4, 8 };
        private static readonly int[] RegionalPresets = { 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4000, 10000 };
        /// <summary>Players in all, you included (v69: 6 to 10,000).</summary>
        private const int MaxRegionalPlayers = RegionalSim.MaxPlayers;
        private const int MinRegionalPlayers = RegionalSim.MinPlayers;
        private const int MaxRegionalRounds = 18;

        /// <summary>How a Regional Sim game is set up (null for an ordinary game).</summary>
        private sealed class RegionalGameStart
        {
            public string Label = "";
            /// <summary>The seat that lost the last game and chooses to go first or second, or -1 for the coin flip.</summary>
            public int ChooserSeat = -1;
            /// <summary>An AI table you watch (both seats are the learning AI) instead of your own game.</summary>
            public bool Watch;
            /// <summary>ChooserSeat is the higher seed choosing for game 1 of a top cut match (v70), not the loser of the last game.</summary>
            public bool HigherSeedChooses;
            public string NameA = "", NameB = "";
            public bool Explore;
            /// <summary>A Tie-Breaker game after a drawn top cut game (v76): it ends as soon as one player leads on Prize cards.</summary>
            public bool TieBreaker;
        }

        private RegionalSim regional;
        private RegionalView regionalView;
        private string regionalError;
        private bool regionalSetupOpen;
        private bool regionalLoadTried;
        private float nextRegionalRefresh;
        private float nextRegionalSave;
        private int regionalSavedVersion = -1;
        private int regionalSeenRound = -1;
        private int regionalSeenAiGames;
        private bool regionalSavedAtEnd;
        private bool regionalPaused;
        private bool regionalConfirmEnd;
        private bool pendingRegional, pendingRegionalGame, pendingRegionalHub;
        private bool regionalShutDown;
        private int regionalShownRound = -1;

        // Setup
        /// <summary>Players in all, you included (the last valid number typed or picked).</summary>
        private int regTotal = 32;
        /// <summary>What's in the "Players in all" box (it can be anything while you type; v57.2).</summary>
        private string regPlayersText = "32";
        private bool regSettingsLoaded;
        private int regRounds = 5;
        private bool regBestOf3 = true;
        private bool regExpanded;
        private bool regAiOnly;
        private bool regExplore = true;
        private bool regWatch = true;
        private int regDeck = -1;
        private Vector2 regDeckScroll;
        /// <summary>The field you chose (v69): how many AI players play each deck, by deck name, and what's typed in each box.</summary>
        private readonly Dictionary<string, int> regFieldCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> regFieldTexts = new Dictionary<string, string>(StringComparer.Ordinal);
        /// <summary>The AI players without a chosen deck pick one at random (on), or every AI player needs a chosen deck (off).</summary>
        private bool regFillRandom = true;
        private Vector2 regFieldScroll, regSetupScroll, regCutScroll;
        /// <summary>The field as meta shares instead (v70): percent of the AI players for each deck, and what's typed in each box.</summary>
        private bool regFieldByPercent;
        private readonly Dictionary<string, double> regFieldPercents = new Dictionary<string, double>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> regFieldPctTexts = new Dictionary<string, string>(StringComparer.Ordinal);
        /// <summary>Top cut (v70): 0, 2, 4 or 8 players after Swiss, single elimination, best of 1 or 3.</summary>
        private int regTopCut;
        private bool regCutBestOf3 = true;
        // v127: Day 1 and Day 2 (big events only), and dropping.
        private bool regDay2;
        private int regDay2Min = 256;
        private int regDay2Rounds = 5;
        private int regDay2Points = 19;
        private bool regionalConfirmDrop;
        // v127: asymmetrical top cut (a play-in for the seats on the bubble's match points).
        private bool regAsymCut;
        /// <summary>AI tables played at once in the background (v71): your setting, kept for every event, 1 to 64.</summary>
        private int regTables = RegionalSim.SuggestedTablesAtOnce(Environment.ProcessorCount);

        // Event screen
        private int regTab;
        private int regViewRound;
        private Vector2 regStandingsScroll, regPairingsScroll, regDecksScroll;
        /// <summary>
        /// v134.1: where the tabs end on the event screen, measured each repaint. The list gets the room left below it, so
        /// error lines or a long status above can't push the buttons at the bottom off the screen.
        /// </summary>
        private float regListTop = 320f;
        /// <summary>v134.4: the earliest time (Time.unscaledTime) the brain is saved again for "200 more AI games".</summary>
        private float nextRegionalBrainSave;
        /// <summary>The event screen's button bar and the line under it, kept at the bottom of the panel.</summary>
        private const float RegionalFooterHeight = 86f;
        private GUIStyle regRowStyle, regHeadStyle, regNumberStyle;
        private bool regAutoWatch = true;
        private int regSkipRound = -1, regSkipTable = -1;

        // The game being played for the event
        private bool regionalGame;
        private bool regionalWatch;
        private int regionalGameRound, regionalGameTable, regionalGameIndex;
        private bool regionalGameReported;
        private string regionalGameResultText;
        private int regionalNextGame;
        private LearningAgent watchAgentA, watchAgentB;

        private static string RegionalPath { get { return Path.Combine(SavePaths.BrainFolder, RegionalFileName); } }

        // ------------------------------------------------------------------ Opening, saving, ending

        private void OpenRegional()
        {
            RefreshDecks();
            regAutoWatch = PlayerPrefs.GetInt(PrefRegionalAutoWatch, 1) == 1;
            LoadRegionalSettings();
            if (regional == null && !regionalLoadTried) TryLoadRegional();
            regionalSetupOpen = regional == null;
            regionalConfirmEnd = false;
            if (regional != null)
            {
                regionalView = regional.View();
                regViewRound = regionalView.Round;
            }
            mode = Mode.Regional;
        }

        private void TryLoadRegional()
        {
            regionalLoadTried = true;
            string path = RegionalPath;
            SafeFile.Recover(path);
            if (!File.Exists(path)) return;
            try
            {
                AttachRegional(RegionalSim.Parse(File.ReadAllText(path, Encoding.UTF8)));
            }
            catch (Exception e)
            {
                regionalError = "Your saved event couldn't be read (" + e.Message + "). Starting a new one replaces it.";
                regional = null;
            }
        }

        /// <summary>Hooks an event up to the brain and the cards and starts its AI tables.</summary>
        private void AttachRegional(RegionalSim sim)
        {
            sim.Brain = brain;
            sim.ScriptsFactory = StandardCards.CreateRegistry;
            var problems = new List<string>();
            foreach (var kv in sim.DeckTexts())
            {
                var found = new List<string>();
                var cards = DeckLibrary.Build(db, new DeckRecord { Name = kv.Key, Text = kv.Value }, found);
                if (cards.Count > 0) sim.SetDeckCards(kv.Key, cards);
                if (found.Count > 0) problems.Add(kv.Key + ": " + string.Join(" ", found.ToArray()));
            }
            if (problems.Count > 0) regionalError = "Some decks of this event have problems now: " + string.Join("  ", problems.ToArray());
            regional = sim;
            regionalView = sim.View();
            regViewRound = regionalView.Round;
            regionalSeenRound = regionalView.Round;
            regionalSeenAiGames = regionalView.AiGames;
            regionalSavedAtEnd = regionalView.Finished;
            regionalSavedVersion = -1;
            regionalPaused = false;
            if (!regionalView.Finished) sim.StartWorkers(RegionalTablesNow());
        }

        /// <summary>AI tables played at once in the background: your setting (v71; 15 in v69), less with the battery saver.</summary>
        private int RegionalTablesAtOnce()
        {
            // v120: the battery saver plays at most 2 at once.
            return WorkThrottle.Threads(Mathf.Clamp(regTables, 1, RegionalSim.MaxTablesAtOnce));
        }

        /// <summary>
        /// The tables playing right now: RegionalTablesAtOnce, but while you're in a game (playing or watching), at most half
        /// the processor threads less one, so your game and the screen keep the rest. Back to the full number when you leave
        /// the game (v134.3).
        /// </summary>
        private int RegionalTablesNow()
        {
            int n = RegionalTablesAtOnce();
            if (mode == Mode.Game) n = Math.Min(n, InGameTablesCap());
            return n;
        }

        private static int InGameTablesCap()
        {
            return Math.Max(1, Environment.ProcessorCount / 2 - 1);
        }

        /// <summary>Sets the tables played at once (saved at once). A running event changes over straight away.</summary>
        private void SetRegionalTables(int n)
        {
            regTables = Mathf.Clamp(n, 1, RegionalSim.MaxTablesAtOnce);
            PlayerPrefs.SetInt(PrefRegTables, regTables);
            PlayerPrefs.Save();
            var sim = regional;
            if (sim != null && !regionalPaused && !sim.Finished) sim.StartWorkers(RegionalTablesNow());
        }

        /// <summary>
        /// Every frame. v120: when the battery saver turns on or off (plugging in, unplugging), a running event changes its tables
        /// at once; v134.3: so does going into a game or coming back from one.
        /// </summary>
        private void SyncRegionalTables()
        {
            var sim = regional;
            if (sim == null || regionalPaused || sim.Finished || !sim.WorkersRunning) return;
            int want = RegionalTablesNow();
            if (sim.TablesAtOnce != want) sim.StartWorkers(want);
        }

        private void SaveRegional()
        {
            var sim = regional;
            // A big event's file is large (about 1 MB for 4,000 players over 9 rounds, 2.5 MB for 10,000), so it's written less often.
            int players = regionalView != null ? regionalView.Players : 0;
            nextRegionalSave = Time.unscaledTime + (players > 5000 ? 20f : players > 2000 ? 10f : players > 1000 ? 5f : 2f);
            if (sim == null) return;
            try
            {
                int ver = sim.Version;
                string text = sim.Serialize();
                SafeFile.WriteText(RegionalPath, text);
                regionalSavedVersion = ver;
            }
            catch (Exception e)
            {
                regionalError = "Couldn't save the event: " + e.Message;
            }
        }

        /// <summary>
        /// Ends the event (it's deleted) and goes back to the setup. v117: what the AI learned is kept even when the event ends
        /// early: the tables finish the games they're playing (so those count too) and the brain is saved before the event goes.
        /// </summary>
        private void EndRegional()
        {
            StopRegionalAndSaveLearning(10000);
            regional = null;
            regionalView = null;
            regionalConfirmEnd = false;
            try
            {
                if (File.Exists(RegionalPath)) File.Delete(RegionalPath);
            }
            catch (Exception e)
            {
                regionalError = "Couldn't delete the saved event: " + e.Message;
            }
            regionalSetupOpen = true;
        }

        /// <summary>
        /// Stops the event's AI tables, letting each finish the game it's playing (up to waitMs) so that game teaches the brain
        /// too, then saves the brain straight away (not in the background, so quitting right after can't lose it).
        /// </summary>
        private void StopRegionalAndSaveLearning(int waitMs)
        {
            var sim = regional;
            if (sim != null) sim.StopWorkers(waitMs);
            var b = brain;
            if (b == null) return;
            try
            {
                b.Save();
                learnStatus = "What the AI learned in the event is saved.";
            }
            catch (Exception e)
            {
                regionalError = "Couldn't save what the AI learned: " + e.Message;
            }
        }

        /// <summary>On quitting: the event is saved and its AI tables stop (unfinished tables are played next time).</summary>
        private void ShutDownRegional()
        {
            if (regional == null || regionalShutDown) return;
            regionalShutDown = true;
            SaveRegional();
            // v117: the brain is saved too, so nothing the event taught since the last save is lost.
            StopRegionalAndSaveLearning(3000);
        }

        private void SetRegionalPaused(bool paused)
        {
            var sim = regional;
            if (sim == null) return;
            regionalPaused = paused;
            if (paused) sim.StopWorkers(0);
            else if (!sim.Finished) sim.StartWorkers(RegionalTablesNow());
        }

        /// <summary>Every frame: refreshes what the screen shows, saves the event and the brain now and then, and opens the table to watch.</summary>
        private void UpdateRegional()
        {
            if (pendingRegional)
            {
                pendingRegional = false;
                OpenRegional();
            }
            if (pendingRegionalGame)
            {
                pendingRegionalGame = false;
                StartNextRegionalGame();
            }
            if (pendingRegionalHub)
            {
                pendingRegionalHub = false;
                BackToRegionalHub();
            }

            var sim = regional;
            if (sim == null) return;
            float now = Time.unscaledTime;
            if (now < nextRegionalRefresh) return;
            nextRegionalRefresh = now + 0.25f;
            int ver = sim.Version;
            if (regionalView == null || regionalView.Version != ver) regionalView = sim.View();
            var v = regionalView;
            if (ver != regionalSavedVersion && now >= nextRegionalSave) SaveRegional();
            // The brain is saved after every round and every 200 AI games; v134.4: the 200 games at most every 3 minutes (with the
            // tables at full speed that was every minute or so, and a big brain takes a while to write).
            bool gamesDue = v.AiGames - regionalSeenAiGames >= 200 && now >= nextRegionalBrainSave;
            if (v.Round != regionalSeenRound || gamesDue || (v.Finished && !regionalSavedAtEnd))
            {
                nextRegionalBrainSave = now + 180f;
                regionalSeenRound = v.Round;
                regionalSeenAiGames = v.AiGames;
                if (v.Finished) regionalSavedAtEnd = true;
                SaveBrainInBackground();
            }
            // The Pairings tab follows the event to each new round.
            if (v.Round != regionalShownRound)
            {
                regionalShownRound = v.Round;
                regViewRound = v.Round;
            }
            // AI only, watching: the table picked for this round opens on the board (once; "Back to the event" doesn't reopen it).
            if (mode == Mode.Regional && !regionalSetupOpen && regAutoWatch && v.Watching && v.Featured != null && !v.Featured.Finished && !v.Finished
                && !(v.Featured.Round == regSkipRound && v.Featured.Table == regSkipTable))
            {
                regSkipRound = v.Featured.Round;
                regSkipTable = v.Featured.Table;
                StartRegionalWatch();
            }
        }

        private void BackToRegionalHub()
        {
            if (regionalWatch)
            {
                regSkipRound = regionalGameRound;
                regSkipTable = regionalGameTable;
            }
            StopGame();
            if (board != null) board.Clear();
            regionalGame = false;
            regionalWatch = false;
            watchAgentA = null;
            watchAgentB = null;
            if (regional != null) regionalView = regional.View();
            regionalSetupOpen = regional == null;
            mode = Mode.Regional;
        }

        // ------------------------------------------------------------------ Starting an event

        /// <summary>Legal decks of the format, as indices into the deck list.</summary>
        private List<int> RegionalDeckPool(DeckFormat format)
        {
            var list = new List<int>();
            for (int i = 0; i < decks.Count; i++)
                if (decks[i].Format == format && !decks[i].IsCustomFormat && i < deckProblems.Count && deckProblems[i].Count == 0) list.Add(i);
            return list;
        }

        /// <summary>v127: Day 2 is on and the event is big enough for it.</summary>
        private bool Day2Applies(int players)
        {
            return regDay2 && players >= regDay2Min;
        }

        /// <summary>v127: you drop from the event (after you confirmed).</summary>
        private void DropFromRegional()
        {
            regionalConfirmDrop = false;
            var sim = regional;
            if (sim == null) return;
            if (sim.DropYou())
            {
                regionalView = sim.View();
                SaveRegional();
            }
        }

        private void StartRegionalEvent()
        {
            regionalError = null;
            var format = regExpanded ? DeckFormat.Expanded : DeckFormat.Standard;
            var pool = RegionalDeckPool(format);
            if (pool.Count == 0)
            {
                regionalError = "There are no legal " + Formats.Label(format) + " decks for the AI players to pick from. Build or import one in the deck builder.";
                return;
            }
            if (!regAiOnly && !pool.Contains(regDeck))
            {
                regionalError = "Pick your deck.";
                return;
            }
            if (!RegionalPlayersValid())
            {
                regionalError = "Type a number of players from " + MinRegionalPlayers + " to " + MaxRegionalPlayers.ToString("N0") + ".";
                return;
            }
            var texts = new Dictionary<string, string>(StringComparer.Ordinal);
            var names = new List<string>();
            foreach (int i in pool)
            {
                if (texts.ContainsKey(decks[i].Name)) continue;
                texts[decks[i].Name] = decks[i].Text;
                names.Add(decks[i].Name);
            }
            string yourDeck = regAiOnly ? null : decks[regDeck].Name;
            int total = Mathf.Clamp(regTotal, MinRegionalPlayers, MaxRegionalPlayers);
            int aiSeats = regAiOnly ? total : total - 1;
            // The decks you chose for the AI players (v69; meta shares v70), as numbers or meta shares; the rest pick at random when that's on.
            var chosen = RegionalChosenSeats(names, aiSeats);
            string fieldProblem = RegionalFieldProblem(names, aiSeats);
            if (fieldProblem != null)
            {
                regionalError = fieldProblem;
                return;
            }
            if (regTopCut > total)
            {
                regionalError = "A top " + regTopCut + " needs at least " + regTopCut + " players.";
                return;
            }
            int seed = (Environment.TickCount & 0x0FFFFFFF) | 1;
            try
            {
                if (regional != null) regional.StopWorkers(1500);
                var sim = RegionalSim.Create(aiSeats, Mathf.Clamp(regRounds, 1, MaxRegionalRounds), regBestOf3, Formats.Label(format), yourDeck, names,
                                             chosen, regFillRandom, texts, regExplore, seed, regAiOnly && regWatch);
                sim.SetTopCut(regTopCut, regCutBestOf3);
                sim.SetAsymmetricCut(regTopCut > 0 && regAsymCut);
                // v127: a big event is split into 2 days.
                if (Day2Applies(total)) sim.SetDayTwo(Mathf.Clamp(regRounds, 1, MaxRegionalRounds), Mathf.Clamp(regDay2Rounds, 1, MaxDay2Rounds), Mathf.Max(0, regDay2Points));
                if (!regAiOnly && regWithFriend && !AddOnlineFriend(sim)) return;
                sim.Begin();
                AttachRegional(sim);
                SaveRegional();
                regionalSetupOpen = false;
                regionalConfirmDrop = false;
                regTab = 0;
                regViewRound = 1;
                regSkipRound = -1;
                regSkipTable = -1;
                if (!regAiOnly) PlayerPrefs.SetString(PrefMyDeck, decks[regDeck].Name);
                SaveRegionalSettings();
            }
            catch (Exception e)
            {
                regionalError = "The event couldn't start: " + e.Message;
            }
        }

        // ------------------------------------------------------------------ Your games and the watched tables

        private void StartNextRegionalGame()
        {
            var sim = regional;
            if (sim == null) return;
            if (sim.YouPlaying) StartRegionalGame();
            else StartRegionalWatch();
        }

        private void StartRegionalGame()
        {
            var sim = regional;
            if (sim == null || !sim.YourGamePending) return;
            var m = sim.YourMatch();
            if (m == null) return;
            var you = sim.Entrant(m.A);
            var opp = sim.Entrant(m.B);
            if (you == null || opp == null) return;
            regionalGameRound = m.Round;
            regionalGameTable = m.Table;
            regionalGameIndex = m.Games.Count;
            regionalGameReported = false;
            regionalGameResultText = null;
            regionalNextGame = 0;
            int seat = Mathf.Clamp(humanSeat, 0, 1);
            int chooser = sim.YourOrderChooser();
            var start = new RegionalGameStart
            {
                Label = RegionalSim.RoundName(m.Round, sim.Rounds, sim.TopCut, sim.PlayInRounds) + ", table " + m.Table + ", game " + (m.Games.Count + 1) + " against " + opp.Name
                        + (RegionalSim.TieBreakerNext(m) ? " (Tie-Breaker game: the first to lead on Prize cards wins)" : ""),
                ChooserSeat = chooser < 0 ? -1 : chooser == 0 ? seat : 1 - seat,
                HigherSeedChooses = RegionalSim.HigherSeedChoosesFor(m) && chooser >= 0,
                TieBreaker = RegionalSim.TieBreakerNext(m)
            };
            // Your online friend: your match against them is played over the network (OnlineMatch).
            var friend = opp.Remote ? RegionalFriendMatch(opp) : null;
            if (opp.Remote && friend == null)
            {
                regionalError = opp.Name + " isn't connected: they're dropped from the event when their connection is lost.";
                return;
            }
            StartGame(new DeckRecord { Name = you.Deck, Text = sim.DeckText(you.Deck) ?? "" },
                      new DeckRecord { Name = opp.Deck, Text = sim.DeckText(opp.Deck) ?? "" }, start, friend);
        }

        private void StartRegionalWatch()
        {
            var sim = regional;
            if (sim == null) return;
            var m = sim.FeaturedMatch();
            if (m == null) return;
            var a = sim.Entrant(m.A);
            var b = sim.Entrant(m.B);
            if (a == null || b == null) return;
            regionalGameRound = m.Round;
            regionalGameTable = m.Table;
            regionalGameIndex = m.Games.Count;
            regionalGameReported = false;
            regionalGameResultText = null;
            regionalNextGame = 0;
            int watchChooser = sim.FeaturedOrderChooser();
            var start = new RegionalGameStart
            {
                Watch = true,
                NameA = a.Name,
                NameB = b.Name,
                Explore = sim.AiExplore,
                ChooserSeat = watchChooser,
                HigherSeedChooses = RegionalSim.HigherSeedChoosesFor(m) && watchChooser >= 0,
                TieBreaker = RegionalSim.TieBreakerNext(m),
                Label = RegionalSim.RoundName(m.Round, sim.Rounds, sim.TopCut, sim.PlayInRounds) + ", table " + m.Table + ", game " + (m.Games.Count + 1)
                        + (RegionalSim.TieBreakerNext(m) ? " (Tie-Breaker game)" : "")
            };
            StartGame(new DeckRecord { Name = a.Deck, Text = sim.DeckText(a.Deck) ?? "" },
                      new DeckRecord { Name = b.Deck, Text = sim.DeckText(b.Deck) ?? "" }, start);
        }

        /// <summary>A Regional Sim game is over and fully shown: it counts for the event (and a watched table's AIs learn from it).</summary>
        private void ReportRegionalGame(PlayerView over)
        {
            var sim = regional;
            if (sim == null || over == null || regionalGameReported) return;
            regionalGameReported = true;
            bool recorded;
            if (regionalWatch)
            {
                int resultA = over.Winner < 0 ? -1 : over.Winner == 0 ? 0 : 1;
                var engine = host != null ? host.Engine : null;
                var agentA = watchAgentA;
                var agentB = watchAgentB;
                var b = brain;
                if (engine != null && b != null && agentA != null && agentB != null)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        try
                        {
                            agentA.LearnFromGame(engine);
                            agentB.LearnFromGame(engine);
                            b.IncrementCounters(1, 0, 0);
                        }
                        catch (Exception e)
                        {
                            learnStatus = "The AI couldn't learn from this game: " + e.Message;
                        }
                    });
                }
                recorded = sim.RecordFeaturedGame(regionalGameRound, regionalGameTable, regionalGameIndex, resultA);
            }
            else
            {
                int result = over.Winner < 0 ? 0 : over.Winner == viewerSeat ? 1 : -1;
                recorded = sim.RecordYourGame(regionalGameRound, regionalGameIndex, result);
            }
            var v = sim.View();
            regionalView = v;
            RegionalMatch match = null;
            foreach (var x in v.Matches)
                if (x.Round == regionalGameRound && x.Table == regionalGameTable) match = x;
            regionalGameResultText = !recorded ? "This game wasn't counted: the match had moved on."
                : match == null ? null : MatchLine(v, match, true);
            regionalNextGame = recorded && match != null && !match.Finished ? match.Games.Count + 1 : 0;
            SaveRegional();
        }

        /// <summary>One line about a match: who leads or who won, and the score.</summary>
        private static string MatchLine(RegionalView v, RegionalMatch m, bool afterGame)
        {
            if (m.IsBye) return NameOf(v, m.A) + " has a bye (a win).";
            string a = NameOf(v, m.A), b = NameOf(v, m.B);
            bool you = a == "You";
            int wa = m.WinsA, wb = m.WinsB;
            string score = Math.Max(wa, wb) + "-" + Math.Min(wa, wb);
            if (m.Finished)
            {
                if (m.Result < 0) return (you ? "Your match against " + b : a + " vs " + b) + " is a tie (" + m.Score + ").";
                if (you) return (m.Result == 0 ? "You won the match " : "You lost the match ") + score + " against " + b + ".";
                return (m.Result == 0 ? a : b) + " won the match " + score + " (" + a + " vs " + b + ").";
            }
            string next = afterGame ? " Game " + (m.Games.Count + 1) + " is next." : "";
            if (wa == wb) return (you ? "You and " + b + " are" : a + " and " + b + " are") + " level at " + wa + "-" + wb + "." + next;
            if (you) return (wa > wb ? "You lead " : "You trail ") + wa + "-" + wb + " against " + b + "." + next;
            return (wa > wb ? a : b) + " leads " + score + "." + next;
        }

        private static string NameOf(RegionalView v, int id)
        {
            return id >= 0 && id < v.Names.Length ? v.Names[id] : "-";
        }

        private static string DeckOf(RegionalView v, int id)
        {
            if (id >= 0 && id == HiddenDeckPlayer(v)) return HiddenDeckText;
            return id >= 0 && id < v.DeckNames.Length ? v.DeckNames[id] : "";
        }

        /// <summary>What the event screen shows instead of the deck of the player you're paired with (v134.7).</summary>
        private const string HiddenDeckText = "deck unknown";

        /// <summary>
        /// v134.7: the player you're paired with now, whose deck you don't see until your match against them is over (like at a
        /// real event, you find out by playing). -1 when there's none (AI only, a bye, or your match is over).
        /// </summary>
        private static int HiddenDeckPlayer(RegionalView v)
        {
            if (v == null || !v.YouPlaying || v.You == null) return -1;
            var m = v.YourMatch;
            if (m == null || m.Finished || m.IsBye) return -1;
            return m.A == v.You.Id ? m.B : m.B == v.You.Id ? m.A : -1;
        }

        /// <summary>A standings row's deck, hidden for the player you're paired with (v134.7).</summary>
        private static string StandingDeck(RegionalView v, RegionalStanding s)
        {
            return s != null && s.Id == HiddenDeckPlayer(v) ? HiddenDeckText : s != null ? s.Deck : "";
        }

        private static string RecordOf(RegionalView v, int id)
        {
            var s = v.StandingOf(id);
            return s == null ? "" : s.Record;
        }

        private static string Pct1(double x)
        {
            return (x * 100).ToString("0.0", CultureInfo.InvariantCulture) + "%";
        }

        private static string Ordinal(int n)
        {
            int t = n % 100;
            if (t >= 11 && t <= 13) return n + "th";
            switch (n % 10)
            {
                case 1: return n + "st";
                case 2: return n + "nd";
                case 3: return n + "rd";
                default: return n + "th";
            }
        }

        // ------------------------------------------------------------------ Drawing

        private void DrawRegional(UiStyles ui)
        {
            if (regionalSetupOpen || regional == null)
            {
                DrawRegionalSetup(ui);
                return;
            }
            var v = regionalView ?? regional.View();
            regionalView = v;
            var actions = new List<Action>();
            float w = Mathf.Min(1180f, UiScale.Width - 30f);
            var r = new Rect(UiScale.Width / 2f - w / 2f, 15f, w, UiScale.Height - 30f);
            GUI.Box(r, GUIContent.none);
            float innerH = r.height - 20f;
            float mainH = innerH - RegionalFooterHeight;
            GUILayout.BeginArea(new Rect(r.x + 14, r.y + 10, r.width - 28, mainH));

            string title = "Regional Sim  ·  " + v.Format + "  ·  " + (v.BestOfThree ? "best of 3" : "best of 1") + "  ·  " + v.Players.ToString("N0") + " players"
                           + (v.Day1Rounds > 0 ? "  ·  2 days, Day 2 at " + v.Day2Points + " points" : "")
                           + (v.TopCut > 0 ? "  ·  top " + v.TopCut + " cut, " + (v.CutBestOfThree ? "best of 3" : "best of 1") + (v.AsymmetricCut ? ", asymmetrical" : "") : "");
            GUILayout.Label(title, ui.Header);
            string stage;
            if (v.Finished)
                stage = v.TopCut > 0 && v.Champion >= 0
                    ? "The event is over: " + NameOf(v, v.Champion) + " won the top " + v.TopCut + " with " + DeckOf(v, v.Champion) + "."
                    : "The event is over after " + v.Rounds + " rounds.";
            else if (v.InPlayIn)
                stage = "Top cut play-in: " + RegionalSim.RoundName(v.Round, v.Rounds, v.TopCut, v.PlayInRounds) + "  ·  " + v.PlayInPool.Count + " players on " + v.PlayInPoints
                        + " points play for " + v.PlayInSeats + " seat" + (v.PlayInSeats == 1 ? "" : "s") + " of the top " + v.TopCut + " (single elimination, " + (v.CutBestOfThree ? "best of 3" : "best of 1") + ")";
            else if (v.InTopCut) stage = "Top cut: " + RegionalSim.RoundName(v.Round, v.Rounds, v.TopCut, v.PlayInRounds) + " (single elimination, " + (v.CutBestOfThree ? "best of 3" : "best of 1") + ")";
            else if (v.Day1Rounds > 0 && v.Round <= v.Day1Rounds)
                stage = "Day 1: round " + v.Round + " of " + v.Day1Rounds + "  ·  " + v.Day2Points + " points or more make Day 2 (" + (v.Rounds - v.Day1Rounds) + " more rounds"
                        + (v.TopCut > 0 ? ", then a top " + v.TopCut + " cut)" : ")");
            else if (v.Day1Rounds > 0)
                stage = "Day 2: round " + v.Round + " of " + v.Rounds + "  ·  " + v.Day2Players.ToString("N0") + " players made Day 2" + (v.TopCut > 0 ? ", then a top " + v.TopCut + " cut" : "");
            else stage = "Round " + v.Round + " of " + v.Rounds + (v.TopCut > 0 ? ", then a top " + v.TopCut + " cut" : "");
            GUILayout.Label(stage, ui.Title);

            DrawRegionalStatus(ui, v, actions, r.width - 28);
            if (regionalError != null) GUILayout.Label(regionalError, ui.Error);
            if (!string.IsNullOrEmpty(v.LastError)) GUILayout.Label("AI tables: " + v.AiErrors + " game(s) hit an engine error and counted as draws. Last: " + v.LastError, ui.Error);
            if (!string.IsNullOrEmpty(learnStatus)) GUILayout.Label(learnStatus, Hint(ui));

            GUILayout.Space(4);
            var tabs = v.TopCut > 0 ? RegionalTabsWithCut : RegionalTabs;
            int tab = Mathf.Clamp(regTab, 0, tabs.Length - 1);
            int newTab = GUILayout.Toolbar(tab, tabs, GUILayout.Width(Mathf.Min(140f * tabs.Length, r.width - 28)));
            if (newTab != regTab) actions.Add(() => regTab = newTab);
            if (Event.current.type == EventType.Repaint) regListTop = GUILayoutUtility.GetLastRect().yMax;
            // The table's header row, its frame and the line of notes under it take about 90 more.
            float listH = Mathf.Max(80f, mainH - regListTop - 90f);
            if (tab == 0) DrawRegionalStandings(ui, v, listH, r.width - 28);
            else if (tab == 1) DrawRegionalPairings(ui, v, listH, r.width - 28, actions);
            else if (tab == 2) DrawRegionalDecks(ui, v, listH, r.width - 28);
            else DrawRegionalTopCut(ui, v, listH, r.width - 28);
            GUILayout.EndArea();

            // The buttons always sit at the bottom of the panel.
            GUILayout.BeginArea(new Rect(r.x + 14, r.y + 10 + mainH, r.width - 28, RegionalFooterHeight));
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Back to menu", GUILayout.Height(30), GUILayout.Width(150)))
                actions.Add(() => { RefreshDecks(); mode = Mode.Menu; });
            if (!v.Finished)
            {
                if (GUILayout.Button(regionalPaused ? "Resume the AI tables" : "Pause the AI tables", GUILayout.Height(30), GUILayout.Width(190)))
                {
                    bool pause = !regionalPaused;
                    actions.Add(() => SetRegionalPaused(pause));
                }
                // v120: a black screen while the AI tables play (they keep going; tap to wake).
                GUI.enabled = !regionalPaused;
                if (GUILayout.Button("Dark screen", GUILayout.Height(30), GUILayout.Width(110))) actions.Add(PowerSaver.GoDark);
                GUI.enabled = true;
            }
            // v127: drop from the event (the rest plays out without you).
            if (v.YouPlaying && v.YouStillIn && !regionalConfirmEnd && !regionalConfirmDrop)
            {
                if (GUILayout.Button("Drop from the event", GUILayout.Height(30), GUILayout.Width(170)))
                    actions.Add(() => regionalConfirmDrop = true);
            }
            if (regionalConfirmDrop)
            {
                var yours = v.YourMatch;
                bool midMatch = yours != null && !yours.Finished && !yours.IsBye;
                GUILayout.Label("Drop? " + (midMatch ? "Your match this round counts as a loss, and you" : "You") + " won't be paired again. The event plays out without you, and you keep your place by your record.", ui.Error, GUILayout.Width(470));
                if (GUILayout.Button("Drop", GUILayout.Height(30), GUILayout.Width(70))) actions.Add(DropFromRegional);
                if (GUILayout.Button("Cancel", GUILayout.Height(30), GUILayout.Width(80))) actions.Add(() => regionalConfirmDrop = false);
            }
            else if (!regionalConfirmEnd)
            {
                if (GUILayout.Button(v.Finished ? "New event" : "End this event", GUILayout.Height(30), GUILayout.Width(150)))
                    actions.Add(() => regionalConfirmEnd = true);
            }
            else
            {
                GUILayout.Label(v.Finished ? "Start over? This event's results are deleted." : "End it? The event and its results are deleted. What the AI learned is saved first.", ui.Error, GUILayout.Width(420));
                if (GUILayout.Button("Yes", GUILayout.Height(30), GUILayout.Width(70))) actions.Add(EndRegional);
                if (GUILayout.Button("No", GUILayout.Height(30), GUILayout.Width(70))) actions.Add(() => regionalConfirmEnd = false);
            }
            GUILayout.EndHorizontal();
            GUILayout.Label("The event is saved as you go (" + RegionalPath + "), and the AI tables keep playing while you're elsewhere in the app.", Hint(ui));
            GUILayout.EndArea();

            foreach (var a in actions) a();
        }

        /// <summary>Your match (or the table to watch), the round's progress and the AI tables.</summary>
        private void DrawRegionalStatus(UiStyles ui, RegionalView v, List<Action> actions, float width)
        {
            if (v.YouPlaying && v.You != null)
            {
                GUILayout.Label("You: " + v.You.Record + " (" + v.You.Points + " points), " + Ordinal(v.You.Rank) + " of " + v.Players.ToString("N0")
                                + "  ·  " + v.You.Deck, ui.Label);
                var m = v.YourMatch;
                int yourPlace = v.You.Id < v.Placement.Length ? v.Placement[v.You.Id] : v.You.Rank;
                bool youMadeCut = v.CutSeeds.Contains(v.You.Id);
                if (v.YouDropped && !v.Finished)
                {
                    GUILayout.Label("You dropped after " + RegionalSim.RoundName(Math.Max(1, v.YouDroppedAfterRound), v.Rounds, v.TopCut, v.PlayInRounds).ToLowerInvariant()
                                    + ". The event plays out without you: you're " + Ordinal(yourPlace) + " of " + v.Players.ToString("N0") + " so far.", ui.Label);
                }
                else if (v.Day2Decided && v.You.MissedDay2 && !v.Finished && !v.InTopCut)
                {
                    GUILayout.Label("You didn't make Day 2 (" + v.You.Points + " points; " + v.Day2Points + " needed). The event plays out without you: you're "
                                    + Ordinal(yourPlace) + " of " + v.Players.ToString("N0") + " so far.", ui.Label);
                }
                else if (v.Finished)
                {
                    string how = v.TopCut > 0 && youMadeCut
                        ? (yourPlace == 1 ? "You won the top " + v.TopCut + "! " : "You made the top " + v.TopCut + ". ")
                        : "";
                    GUILayout.Label(how + "You finished " + Ordinal(yourPlace) + " of " + v.Players.ToString("N0") + " (" + v.You.Record + " in Swiss"
                                    + (v.YouDropped ? ", dropped" : v.You.MissedDay2 ? ", out after Day 1" : "") + ").", ui.Good);
                }
                else if (m == null && v.InPlayIn)
                {
                    bool above = v.PlayInAbove.Contains(v.You.Id);
                    bool inPool = v.PlayInPool.Contains(v.You.Id);
                    GUILayout.Label(above ? "You're in the top " + v.TopCut + " (above the " + v.PlayInPoints + "-point bubble). Waiting for the play-in: see the Top cut tab."
                                    : inPool && v.YouStillIn ? "You have a bye this play-in round (a better seed). Waiting for the other tables."
                                    : inPool ? "You lost in the play-in and didn't make the top " + v.TopCut + ". It plays out in the background: see the Top cut tab."
                                    : "You finished " + Ordinal(v.You.Rank) + " in Swiss and didn't make the top " + v.TopCut + ". It plays out in the background: see the Top cut tab.", ui.Label);
                }
                else if (m == null && v.InTopCut)
                {
                    GUILayout.Label(youMadeCut
                        ? "You're out of the top cut (" + Ordinal(yourPlace) + " place so far). It plays out in the background: see the Top cut tab."
                        : v.PlayInPool.Contains(v.You.Id) ? "You lost in the play-in and didn't make the top " + v.TopCut + ". It plays out in the background: see the Top cut tab."
                        : "You finished " + Ordinal(v.You.Rank) + " in Swiss and didn't make the top " + v.TopCut + ". It plays out in the background: see the Top cut tab.", ui.Label);
                }
                else if (m == null)
                {
                    GUILayout.Label("Waiting for the next round to be paired.", Hint(ui));
                }
                else if (m.IsBye)
                {
                    GUILayout.Label("Round " + m.Round + ": you have a bye (it counts as a win). Waiting for the other tables.", ui.Label);
                }
                else
                {
                    string roundName = RegionalSim.RoundName(m.Round, v.Rounds, v.TopCut, v.PlayInRounds);
                    string opp = (m.IsCut ? "(" + m.SeedB + ") " : "") + NameOf(v, m.B) + " (" + RecordOf(v, m.B) + ", " + DeckOf(v, m.B) + ")";
                    string you = m.IsCut ? "you (" + m.SeedA + ")" : "you";
                    if (!m.Finished)
                    {
                        GUILayout.Label(roundName + ", table " + m.Table + ": " + you + " vs " + opp + (m.Games.Count > 0 ? "  ·  " + MatchLine(v, m, false) : ""), ui.Label);
                        if (RegionalSim.HigherSeedChoosesFor(m))
                            GUILayout.Label(m.SeedA < m.SeedB ? "You're the higher seed: you choose who goes first" + (m.Games.Count == 0 ? " in game 1." : " (a Tie-Breaker game after the drawn one).") : "Your opponent is the higher seed and chooses who goes first" + (m.Games.Count == 0 ? " in game 1." : " (a Tie-Breaker game after the drawn one)."), Hint(ui));
                        string label = m.Games.Count == 0 ? "Play your match" : "Play game " + (m.Games.Count + 1);
                        if (GUILayout.Button(label, GUILayout.Height(34), GUILayout.Width(220))) actions.Add(() => pendingRegionalGame = true);
                    }
                    else
                    {
                        GUILayout.Label(roundName + ": " + MatchLine(v, m, false) + "  Waiting for the other tables.", ui.Label);
                    }
                }
            }
            else
            {
                GUILayout.Label("AI only: every seat is the Neural AI, and it learns from every game of the event.", ui.Label);
                GUILayout.BeginHorizontal();
                bool watch = GUILayout.Toggle(v.Watching, " Watch one match each round (picked at random)");
                if (watch != v.Watching)
                {
                    bool on = watch;
                    actions.Add(() =>
                    {
                        if (regional != null) regional.SetWatching(on);
                        regionalView = regional != null ? regional.View() : null;
                    });
                }
                if (v.Watching)
                {
                    bool auto = GUILayout.Toggle(regAutoWatch, " Open it automatically");
                    if (auto != regAutoWatch)
                    {
                        bool on = auto;
                        actions.Add(() =>
                        {
                            regAutoWatch = on;
                            PlayerPrefs.SetInt(PrefRegionalAutoWatch, on ? 1 : 0);
                        });
                    }
                }
                GUILayout.EndHorizontal();
                var f = v.Featured;
                if (v.Watching && f != null && !v.Finished)
                {
                    string line = "Table " + f.Table + ": " + NameOf(v, f.A) + " (" + DeckOf(v, f.A) + ") vs " + NameOf(v, f.B) + " (" + DeckOf(v, f.B) + ")";
                    if (f.Finished) GUILayout.Label("This round's watched table is over: " + MatchLine(v, f, false), ui.Label);
                    else
                    {
                        GUILayout.Label(line + (f.Games.Count > 0 ? "  ·  " + MatchLine(v, f, false) : ""), ui.Label);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button(f.Games.Count == 0 ? "Watch it" : "Watch game " + (f.Games.Count + 1), GUILayout.Height(32), GUILayout.Width(200)))
                            actions.Add(() => pendingRegionalGame = true);
                        if (GUILayout.Button("Let it play in the background", GUILayout.Height(32), GUILayout.Width(240)))
                            actions.Add(() =>
                            {
                                if (regional != null) regional.ReleaseFeatured();
                                regionalView = regional != null ? regional.View() : null;
                            });
                        GUILayout.EndHorizontal();
                        GUILayout.Label("The round waits for this table, so the next one starts once it's been watched (or let go).", Hint(ui));
                    }
                }
                if (v.Finished && v.Standings.Count > 0)
                {
                    int champId = v.Champion >= 0 ? v.Champion : v.Standings[0].Id;
                    var champ = v.StandingOf(champId);
                    GUILayout.Label("Winner: " + NameOf(v, champId) + " with " + DeckOf(v, champId) + (champ != null ? " (" + champ.Record + " in Swiss)" : "")
                                    + (v.TopCut > 0 ? ", champion of the top " + v.TopCut + "." : "."), ui.Good);
                }
            }

            if (!v.Finished)
            {
                float frac = v.RoundMatches == 0 ? 0f : Mathf.Clamp01((float)v.RoundFinished / v.RoundMatches);
                var bar = GUILayoutUtility.GetRect(Mathf.Min(width, 700f), 16f, GUILayout.Width(Mathf.Min(width, 700f)));
                GUI.Box(bar, GUIContent.none);
                GUI.DrawTexture(new Rect(bar.x + 2, bar.y + 2, (bar.width - 4) * frac, bar.height - 4), ui.Selected);
                GUILayout.Label(RegionalSim.RoundName(v.Round, v.Rounds, v.TopCut, v.PlayInRounds) + ": " + v.RoundFinished + " of " + v.RoundMatches + " matches finished  ·  " + v.RoundPlaying
                                + " AI tables playing now" + (regionalPaused ? " (paused)" : "") + "  ·  " + v.AiGames.ToString("N0") + " AI games so far", Hint(ui));
                DrawRegionalTablesAtOnce(ui, actions, false);
            }
        }

        private GUIStyle RegionalRowStyle(UiStyles ui)
        {
            if (regRowStyle == null)
            {
                regRowStyle = new GUIStyle(ListRowStyle(ui)) { richText = true };
                regHeadStyle = new GUIStyle(regRowStyle) { fontStyle = FontStyle.Bold };
            }
            return regRowStyle;
        }

        /// <summary>
        /// A table with a header row and its own scroll bar. Only the rows in view are drawn, so a thousand players cost no
        /// more than ten. widths: each column's width (the last one takes what's left).
        /// </summary>
        private Vector2 DrawRegionalTable(UiStyles ui, Vector2 scroll, int count, float maxHeight, float width, float[] widths, string[] headers, Func<int, int, string> cell)
        {
            var style = RegionalRowStyle(ui);
            float rowH = listRowHeight;
            var head = GUILayoutUtility.GetRect(width, rowH, GUILayout.Width(width));
            float hx = head.x + 6f;
            for (int c = 0; c < headers.Length; c++)
            {
                float cw = c == headers.Length - 1 ? Mathf.Max(40f, head.xMax - hx) : widths[c];
                GUI.Label(new Rect(hx, head.y, cw - 6f, rowH), headers[c], regHeadStyle);
                hx += widths[c];
            }
            float contentH = Mathf.Max(rowH, count * rowH);
            float viewH = Mathf.Min(maxHeight, contentH + 4f);
            GUILayout.BeginVertical(GUI.skin.box, GUILayout.Width(width));
            scroll = GUILayout.BeginScrollView(scroll, GUILayout.Height(viewH));
            var area = GUILayoutUtility.GetRect(10f, contentH, GUILayout.ExpandWidth(true));
            if (Event.current.type == EventType.Repaint && count > 0)
            {
                int first = Mathf.Clamp(Mathf.FloorToInt((scroll.y - area.y) / rowH), 0, count - 1);
                int last = Mathf.Clamp(Mathf.CeilToInt((scroll.y - area.y + viewH) / rowH), 0, count - 1);
                for (int k = first; k <= last; k++)
                {
                    float x = area.x;
                    for (int c = 0; c < widths.Length; c++)
                    {
                        float cw = c == widths.Length - 1 ? Mathf.Max(40f, area.xMax - x) : widths[c];
                        GUI.Label(new Rect(x, area.y + k * rowH, cw - 6f, rowH), cell(k, c), style);
                        x += widths[c];
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            return scroll;
        }

        private static string Mark(bool you, string text)
        {
            return you ? "<color=" + MineColor + "><b>" + text + "</b></color>" : text;
        }

        private void DrawRegionalStandings(UiStyles ui, RegionalView v, float height, float width)
        {
            var rows = v.Standings;
            var widths = new[] { 56f, 130f, 80f, 60f, 70f, 70f, 260f };
            var headers = new[] { "Rank", "Player", "Record", "Points", "Opp %", "Opp opp %", "Deck" };
            regStandingsScroll = DrawRegionalTable(ui, regStandingsScroll, rows.Count, height, width, widths, headers, (k, c) =>
            {
                var s = rows[k];
                switch (c)
                {
                    case 0: return Mark(s.Human, s.Rank.ToString());
                    case 1: return Mark(s.Human, s.Name) + (s.Dropped ? " <color=#A9ADB8>(dropped)</color>" : s.MissedDay2 ? " <color=#A9ADB8>(Day 1)</color>" : "");
                    case 2: return Mark(s.Human, s.Record);
                    case 3: return Mark(s.Human, s.Points.ToString());
                    case 4: return Mark(s.Human, s.OppWinPct > 0 ? Pct1(s.OppWinPct) : "-");
                    case 5: return Mark(s.Human, s.OppOppWinPct > 0 ? Pct1(s.OppOppWinPct) : "-");
                    default: return Mark(s.Human, StandingDeck(v, s));
                }
            });
            GUILayout.Label("Win 3 points, tie 1. Ties are broken by opponents' win percentage, then opponents' opponents' (each player's win percentage is at least 25%; byes count as wins but not as opponents)."
                            + (v.Day1Rounds > 0 ? " Once Day 1 is over, everyone who made Day 2 ranks above everyone who didn't." : "")
                            + " Players who dropped keep their place by their record.", Hint(ui));
        }

        private void DrawRegionalPairings(UiStyles ui, RegionalView v, float height, float width, List<Action> actions)
        {
            int shown = Mathf.Clamp(regViewRound <= 0 ? v.Round : regViewRound, 1, Math.Max(1, v.Round));
            GUILayout.BeginHorizontal();
            GUI.enabled = shown > 1;
            if (GUILayout.Button("<", GUILayout.Width(32))) actions.Add(() => regViewRound = shown - 1);
            GUI.enabled = true;
            GUILayout.Label(RegionalSim.RoundName(shown, v.Rounds, v.TopCut, v.PlayInRounds) + (shown == v.Round && !v.Finished ? " (now)" : ""), ui.Title, GUILayout.Width(150));
            GUI.enabled = shown < v.Round;
            if (GUILayout.Button(">", GUILayout.Width(32))) actions.Add(() => regViewRound = shown + 1);
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            var rows = v.Matches.Where(x => x.Round == shown).OrderBy(x => x.Table).ToList();
            var widths = new[] { 56f, 330f, 120f, 330f };
            var headers = new[] { "Table", "Player", "Result", "Opponent" };
            regPairingsScroll = DrawRegionalTable(ui, regPairingsScroll, rows.Count, height - 30f, width, widths, headers, (k, c) =>
            {
                var m = rows[k];
                bool you = NameOf(v, m.A) == "You";
                switch (c)
                {
                    case 0: return Mark(you, m.Table.ToString());
                    case 1: return Mark(you, (m.IsCut ? "(" + m.SeedA + ") " : "") + NameOf(v, m.A) + "  <color=#A9ADB8>" + DeckOf(v, m.A) + "</color>");
                    case 2:
                        if (m.IsBye) return "bye";
                        if (!m.Finished) return m.InProgress ? "playing " + m.WinsA + "-" + m.WinsB : m.Games.Count > 0 ? m.WinsA + "-" + m.WinsB + " so far" : "waiting";
                        return Mark(you, m.Result == 0 ? "won " + m.WinsA + "-" + m.WinsB : m.Result == 1 ? "lost " + m.WinsA + "-" + m.WinsB : "tie " + m.WinsA + "-" + m.WinsB);
                    default:
                        return m.IsBye ? "" : (m.IsCut ? "(" + m.SeedB + ") " : "") + NameOf(v, m.B) + "  <color=#A9ADB8>" + DeckOf(v, m.B) + "</color>";
                }
            });
        }

        /// <summary>
        /// The top cut (v70): who's in it by seed (or who would be, while Swiss is on), each round's matches and results, and the
        /// final places.
        /// </summary>
        private void DrawRegionalTopCut(UiStyles ui, RegionalView v, float height, float width)
        {
            regCutScroll = GUILayout.BeginScrollView(regCutScroll, GUILayout.Height(height), GUILayout.Width(width));
            if (v.PlayInPool.Count > 0)
            {
                // v127: the asymmetrical top cut's play-in.
                GUILayout.Label("Play-in: " + v.PlayInPool.Count + " players on " + v.PlayInPoints + " points for " + v.PlayInSeats + " seat" + (v.PlayInSeats == 1 ? "" : "s")
                                + (v.PlayInAbove.Count > 0 ? " (" + v.PlayInAbove.Count + " above them are in)" : ""), ui.Title);
                for (int k = 0; k < v.PlayInPool.Count; k++)
                {
                    int pid = v.PlayInPool[k];
                    var st = v.StandingOf(pid);
                    GUILayout.Label(Mark(NameOf(v, pid) == "You", "(" + (v.PlayInAbove.Count + k + 1) + ") " + NameOf(v, pid) + "  ·  " + (st != null ? st.Record : "") + "  ·  " + DeckOf(v, pid)), RegionalRowStyle(ui));
                }
                GUILayout.Space(4);
                for (int r = v.Rounds + 1; r <= Math.Min(v.Round, v.CutStart); r++) DrawRegionalCutRound(ui, v, r);
            }
            if (v.CutSeeds.Count == 0)
            {
                if (!v.InPlayIn)
                {
                    GUILayout.Label("The top " + v.TopCut + " is seeded from the standings after round " + v.Rounds + ". If Swiss ended now:", ui.Label);
                    foreach (var s in v.Standings.Take(v.TopCut))
                        GUILayout.Label(Mark(s.Human, "(" + s.Rank + ") " + s.Name + "  ·  " + s.Record + "  ·  " + StandingDeck(v, s)), RegionalRowStyle(ui));
                    if (v.AsymmetricCut)
                        GUILayout.Label("Asymmetrical: everyone on the same match points as the last seat plays for the seats those points hold.", Hint(ui));
                }
            }
            else
            {
                for (int r = v.CutStart + 1; r <= Math.Min(v.Round, v.CutStart + v.CutRounds); r++) DrawRegionalCutRound(ui, v, r);
                GUILayout.Label(v.Finished ? "Final places" : "Places so far", ui.Title);
                var places = v.CutSeeds.Select(id => new KeyValuePair<int, int>(id, id < v.Placement.Length ? v.Placement[id] : 0)).OrderBy(kv => kv.Value).ToList();
                foreach (var place in places)
                {
                    bool you = NameOf(v, place.Key) == "You";
                    GUILayout.Label(Mark(you, Ordinal(place.Value) + "  " + NameOf(v, place.Key) + "  ·  " + DeckOf(v, place.Key) + "  ·  seed " + (v.CutSeeds.IndexOf(place.Key) + 1)), RegionalRowStyle(ui));
                }
            }
            GUILayout.EndScrollView();
            GUILayout.Label((v.AsymmetricCut ? "Asymmetrical: everyone on the last seat's match points plays single elimination for those seats first (byes to the better seeds; the winners keep their Swiss order). " : "")
                            + "Single elimination, seeded by the Swiss standings. The higher seed chooses who goes first in game 1; a drawn game is followed by a Tie-Breaker game, 6 Prize cards that end at the first Prize lead (after "
                            + RegionalSim.MaxCutGames + " games, the higher seed goes through).", Hint(ui));
        }

        /// <summary>One round of the top cut or its play-in (v127: shared by both), its matches and results.</summary>
        private void DrawRegionalCutRound(UiStyles ui, RegionalView v, int roundNumber)
        {
            GUILayout.Label(RegionalSim.RoundName(roundNumber, v.Rounds, v.TopCut, v.PlayInRounds), ui.Title);
            foreach (var m in v.Matches.Where(x => x.Round == roundNumber).OrderBy(x => x.Table))
            {
                bool you = NameOf(v, m.A) == "You" || NameOf(v, m.B) == "You";
                string a = "(" + m.SeedA + ") " + NameOf(v, m.A) + " <color=#A9ADB8>" + DeckOf(v, m.A) + "</color>";
                string b = "(" + m.SeedB + ") " + NameOf(v, m.B) + " <color=#A9ADB8>" + DeckOf(v, m.B) + "</color>";
                string result;
                if (m.Finished)
                {
                    int winner = m.Result == 0 ? m.A : m.B;
                    result = NameOf(v, winner) + " won " + Math.Max(m.WinsA, m.WinsB) + "-" + Math.Min(m.WinsA, m.WinsB)
                             + (m.ConcededBy >= 0 ? " (" + NameOf(v, m.ConcededBy) + " dropped)" : m.WinsA == m.WinsB ? " (higher seed, after " + m.Games.Count + " games)" : "");
                }
                else result = m.InProgress ? "playing " + m.WinsA + "-" + m.WinsB : m.Games.Count > 0 ? m.WinsA + "-" + m.WinsB + " so far" : "waiting";
                GUILayout.Label(Mark(you, a + "   vs   " + b + "   ·   " + result), RegionalRowStyle(ui));
            }
            GUILayout.Space(4);
        }

        private void DrawRegionalDecks(UiStyles ui, RegionalView v, float height, float width)
        {
            var rows = v.Decks;
            var widths = new[] { 300f, 70f, 70f, 110f, 80f, 90f };
            var headers = new[] { "Deck", "Players", "Share", "Matches", "Win %", "Best finish" };
            int players = Math.Max(1, v.Players);
            regDecksScroll = DrawRegionalTable(ui, regDecksScroll, rows.Count, height, width, widths, headers, (k, c) =>
            {
                var d = rows[k];
                switch (c)
                {
                    case 0: return d.Deck;
                    case 1: return d.Players.ToString();
                    case 2: return Pct1((double)d.Players / players);
                    case 3: return d.Wins + "-" + d.Losses + "-" + d.Ties;
                    case 4: return d.Wins + d.Losses + d.Ties > 0 ? Pct1(d.WinRate) : "-";
                    default: return d.BestRank == int.MaxValue ? "-" : Ordinal(d.BestRank);
                }
            });
            GUILayout.Label("Matches and win % leave out byes; a tie counts as half a win.", Hint(ui));
        }

        // ------------------------------------------------------------------ Setup screen

        private void DrawRegionalSetup(UiStyles ui)
        {
            var actions = new List<Action>();
            float w = Mathf.Min(1000f, UiScale.Width - 30f);
            var r = new Rect(UiScale.Width / 2f - w / 2f, 15f, w, UiScale.Height - 30f);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 14, r.y + 10, r.width - 28, r.height - 20));
            // The whole screen scrolls (v69: the AI players' decks made it longer).
            regSetupScroll = GUILayout.BeginScrollView(regSetupScroll);
            GUILayout.Label("Regional Sim", ui.Header);
            GUILayout.Label("A Swiss event where every other seat is the Neural AI. It learns from every game as the event goes on. The AI "
                            + "players play the decks you choose for them, and the rest pick theirs at random from the legal decks of the format. Your "
                            + "games are played on the board; the other tables play in the background, and a round is paired only when every match "
                            + "of the previous one is over.", ui.Label);
            GUILayout.Space(6);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Who plays", GUILayout.Width(150));
            int who = GUILayout.Toolbar(regAiOnly ? 1 : 0, new[] { "You and the AI", "AI only (you watch)" }, GUILayout.Width(360));
            if (who != (regAiOnly ? 1 : 0)) actions.Add(() => SetRegionalAiOnly(who == 1));
            GUILayout.EndHorizontal();

            // Players in all, you included: type any number from 6 to 10,000, or use the buttons (v57.2, v69).
            int total = regTotal;
            bool playersValid = RegionalPlayersValid();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Players in all", GUILayout.Width(150));
            string typed = GUILayout.TextField(regPlayersText ?? "", 5, RegionalNumberStyle(ui), GUILayout.Width(80), GUILayout.Height(26));
            if (typed != regPlayersText)
            {
                string t = new string((typed ?? "").Where(char.IsDigit).ToArray());
                actions.Add(() => SetRegionalPlayersText(t));
            }
            foreach (int step in new[] { -10, -1, 1, 10 })
            {
                int d = step;
                if (GUILayout.Button((d > 0 ? "+" : "") + d, GUILayout.Width(42), GUILayout.Height(26))) actions.Add(() => SetRegionalPlayers(regTotal + d));
            }
            string who2 = regAiOnly ? "all of them AI players" : "you and " + (total - 1).ToString("N0") + " AI players";
            GUILayout.Label(playersValid ? "  " + total.ToString("N0") + " players: " + who2 : "", Hint(ui));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(150));
            if (playersValid) GUILayout.Label("Type any number from " + MinRegionalPlayers + " to " + MaxRegionalPlayers.ToString("N0") + ", or pick one:", Hint(ui));
            else GUILayout.Label("Type a number of players from " + MinRegionalPlayers + " to " + MaxRegionalPlayers.ToString("N0") + ".", ui.Error);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("", GUILayout.Width(150));
            foreach (int preset in RegionalPresets)
            {
                int p = preset;
                if (GUILayout.Button(p.ToString("N0"), GUILayout.Width(p >= 10000 ? 62 : 54))) actions.Add(() => SetRegionalPlayers(p));
            }
            GUILayout.EndHorizontal();

            int suggested = RegionalSim.SuggestedRounds(total);
            bool twoDays = playersValid && Day2Applies(total);
            GUILayout.BeginHorizontal();
            GUILayout.Label(twoDays ? "Day 1 rounds" : "Rounds", GUILayout.Width(150));
            if (GUILayout.Button("<", GUILayout.Width(30))) actions.Add(() => regRounds = Mathf.Clamp(regRounds - 1, 1, MaxRegionalRounds));
            GUILayout.Label(regRounds.ToString(), ui.Title, GUILayout.Width(36));
            if (GUILayout.Button(">", GUILayout.Width(30))) actions.Add(() => regRounds = Mathf.Clamp(regRounds + 1, 1, MaxRegionalRounds));
            if (GUILayout.Button("Use " + suggested + " (the usual for " + total.ToString("N0") + " players)", GUILayout.Width(300))) actions.Add(() => regRounds = suggested);
            GUILayout.EndHorizontal();

            // v127: Day 1 and Day 2.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Day 2", GUILayout.Width(150));
            bool day2 = GUILayout.Toggle(regDay2, " Split big events into 2 days");
            if (day2 != regDay2) actions.Add(() => regDay2 = day2);
            GUILayout.EndHorizontal();
            if (regDay2)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("2 days from", GUILayout.Width(150));
                foreach (int step in new[] { -100, -10 })
                {
                    int d = step;
                    if (GUILayout.Button(d.ToString(), GUILayout.Width(46), GUILayout.Height(24))) actions.Add(() => regDay2Min = Mathf.Clamp(regDay2Min + d, MinRegionalPlayers, MaxRegionalPlayers));
                }
                GUILayout.Label(regDay2Min.ToString("N0"), ui.Title, GUILayout.Width(64));
                foreach (int step in new[] { 10, 100 })
                {
                    int d = step;
                    if (GUILayout.Button("+" + d, GUILayout.Width(46), GUILayout.Height(24))) actions.Add(() => regDay2Min = Mathf.Clamp(regDay2Min + d, MinRegionalPlayers, MaxRegionalPlayers));
                }
                GUILayout.Label("  players (smaller events play 1 day)", Hint(ui));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Day 2 rounds", GUILayout.Width(150));
                if (GUILayout.Button("<", GUILayout.Width(30))) actions.Add(() => regDay2Rounds = Mathf.Clamp(regDay2Rounds - 1, 1, MaxDay2Rounds));
                GUILayout.Label(regDay2Rounds.ToString(), ui.Title, GUILayout.Width(36));
                if (GUILayout.Button(">", GUILayout.Width(30))) actions.Add(() => regDay2Rounds = Mathf.Clamp(regDay2Rounds + 1, 1, MaxDay2Rounds));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Day 2 needs", GUILayout.Width(150));
                if (GUILayout.Button("<", GUILayout.Width(30))) actions.Add(() => regDay2Points = Mathf.Max(0, regDay2Points - 1));
                GUILayout.Label(regDay2Points.ToString(), ui.Title, GUILayout.Width(36));
                if (GUILayout.Button(">", GUILayout.Width(30))) actions.Add(() => regDay2Points = Mathf.Min(3 * MaxRegionalRounds, regDay2Points + 1));
                GUILayout.Label("  match points after Day 1 (19 is a 6-2-1 record; win 3, tie 1)", Hint(ui));
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(150));
                if (!playersValid) GUILayout.Label("", Hint(ui));
                else if (twoDays)
                {
                    string dayLine = "This event: Day 1 is " + regRounds + " rounds with all " + total.ToString("N0") + " players; everyone with " + regDay2Points
                                     + "+ points plays " + regDay2Rounds + " more on Day 2 (" + (regRounds + regDay2Rounds) + " rounds in all, records carried over)"
                                     + (regTopCut > 0 ? ", then the top " + regTopCut + " cut." : ".") + " Players who miss Day 2 place below everyone who made it.";
                    GUILayout.Label(regRounds * 3 < regDay2Points ? dayLine + " Nobody can reach " + regDay2Points + " points in " + regRounds + " rounds!" : dayLine,
                                    regRounds * 3 < regDay2Points ? ui.Error : Hint(ui));
                }
                else GUILayout.Label("This event has " + total.ToString("N0") + " players, fewer than " + regDay2Min.ToString("N0") + ", so it's 1 day of " + regRounds + " rounds.", Hint(ui));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Matches", GUILayout.Width(150));
            int bo = GUILayout.Toolbar(regBestOf3 ? 1 : 0, new[] { "Best of 1", "Best of 3" }, GUILayout.Width(240));
            if (bo != (regBestOf3 ? 1 : 0)) actions.Add(() => regBestOf3 = bo == 1);
            GUILayout.Label("  In a best of 3 the loser of each game chooses to go first or second in the next one.", Hint(ui));
            GUILayout.EndHorizontal();

            // Top cut (v70): single elimination after the Swiss rounds.
            GUILayout.BeginHorizontal();
            GUILayout.Label("Top cut", GUILayout.Width(150));
            int cutIndex = Math.Max(0, Array.IndexOf(RegionalCutSizes, regTopCut));
            int newCut = GUILayout.Toolbar(cutIndex, new[] { "None", "Top 2", "Top 4", "Top 8" }, GUILayout.Width(320));
            if (newCut != cutIndex)
            {
                int size = RegionalCutSizes[newCut];
                actions.Add(() => regTopCut = size);
            }
            GUILayout.EndHorizontal();
            if (regTopCut > 0)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Top cut matches", GUILayout.Width(150));
                int cbo = GUILayout.Toolbar(regCutBestOf3 ? 1 : 0, new[] { "Best of 1", "Best of 3" }, GUILayout.Width(240));
                if (cbo != (regCutBestOf3 ? 1 : 0)) actions.Add(() => regCutBestOf3 = cbo == 1);
                GUILayout.EndHorizontal();
                // v127: asymmetrical top cut.
                GUILayout.BeginHorizontal();
                GUILayout.Label("Asymmetrical", GUILayout.Width(150));
                bool asym = GUILayout.Toggle(regAsymCut, " Ties on the bubble play it out");
                if (asym != regAsymCut) actions.Add(() => regAsymCut = asym);
                GUILayout.EndHorizontal();
                if (regAsymCut)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("", GUILayout.Width(150));
                    GUILayout.Label("When Swiss is over, everyone on the same match points as the last top " + regTopCut + " seat (inside the cut by tiebreakers or not) plays single elimination for the seats those points hold, "
                                    + (regCutBestOf3 ? "best of 3" : "best of 1") + " like the top cut. The better Swiss seeds get the byes, and the winners take the seats in their Swiss order.", Hint(ui));
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(150));
                if (regTopCut > total)
                    GUILayout.Label("A top " + regTopCut + " needs at least " + regTopCut + " players.", ui.Error);
                else
                    GUILayout.Label("After the Swiss rounds the top " + regTopCut + " play single elimination, seeded by the standings"
                                    + (regTopCut == 8 ? " (1 v 8, 4 v 5, 2 v 7, 3 v 6)" : regTopCut == 4 ? " (1 v 4, 2 v 3)" : "")
                                    + ". The higher seed chooses who goes first in game 1, and a drawn game is followed by a Tie-Breaker game (it ends at the first Prize lead).", Hint(ui));
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label("Format", GUILayout.Width(150));
            int fmt = GUILayout.Toolbar(regExpanded ? 1 : 0, new[] { "Standard", "Expanded" }, GUILayout.Width(240));
            if (fmt != (regExpanded ? 1 : 0)) actions.Add(() => { regExpanded = fmt == 1; regDeck = -1; });
            GUILayout.EndHorizontal();


            DrawRegionalFriendRow(actions);
            bool explore = GUILayout.Toggle(regExplore, " The AI games try other plays now and then while learning (it learns faster, like on the Train screen; your own opponents always play their best)");
            if (explore != regExplore) actions.Add(() => regExplore = explore);
            if (regAiOnly)
            {
                bool watch = GUILayout.Toggle(regWatch, " Watch one match each round on the board (picked at random; the others play in the background)");
                if (watch != regWatch) actions.Add(() => regWatch = watch);
            }

            var format = regExpanded ? DeckFormat.Expanded : DeckFormat.Standard;
            var pool = RegionalDeckPool(format);
            if (!regAiOnly)
            {
                if (pool.Count > 0 && !pool.Contains(regDeck))
                {
                    int remembered = IndexOfDeck(PlayerPrefs.GetString(PrefMyDeck, ""), -1);
                    int first = pool.Contains(remembered) ? remembered : pool[0];
                    actions.Add(() => regDeck = first);
                }
                GUILayout.Label("Your deck (legal " + Formats.Label(format) + " decks)", ui.Title);
                regDeckScroll = GUILayout.BeginScrollView(regDeckScroll, GUILayout.Height(Mathf.Clamp(r.height - 520f, 90f, 260f)));
                foreach (int i in pool)
                {
                    int k = i;
                    if (GUILayout.Toggle(regDeck == i, decks[i].Label, ui.Button) && regDeck != i) actions.Add(() => regDeck = k);
                }
                GUILayout.EndScrollView();
            }
            var fieldNames = pool.Select(ix => decks[ix].Name).Distinct().ToList();
            int aiSeatsNow = regAiOnly ? total : total - 1;
            if (pool.Count == 0)
                GUILayout.Label("There are no legal " + Formats.Label(format) + " decks yet. Decks are Standard unless their list has a \"# Format: Expanded\" line.", ui.Error);
            else
                DrawRegionalField(ui, fieldNames, aiSeatsNow, actions);
            GUILayout.Space(4);
            DrawRegionalTablesAtOnce(ui, actions, true);
            int matchesPerRound = total / 2;
            GUILayout.Label("Each round has " + matchesPerRound.ToString("N0") + " matches (about " + (regBestOf3 ? matchesPerRound * 5 / 2 : matchesPerRound).ToString("N0")
                            + " games), played " + RegionalTablesAtOnce() + " at a time in the background.", Hint(ui));

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUI.enabled = pool.Count > 0 && (regAiOnly || pool.Contains(regDeck)) && brain != null && playersValid
                          && RegionalFieldProblem(fieldNames, aiSeatsNow) == null && regTopCut <= total;
            if (GUILayout.Button("Start the event", GUILayout.Height(34), GUILayout.Width(200))) actions.Add(StartRegionalEvent);
            GUI.enabled = true;
            if (regional != null && GUILayout.Button("Back to your event", GUILayout.Height(34), GUILayout.Width(200))) actions.Add(() => regionalSetupOpen = false);
            if (GUILayout.Button("Back to menu", GUILayout.Height(34), GUILayout.Width(150))) actions.Add(() => { RefreshDecks(); mode = Mode.Menu; });
            GUILayout.EndHorizontal();
            if (regionalError != null) GUILayout.Label(regionalError, ui.Error);
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            foreach (var a in actions) a();
        }

        /// <summary>
        /// Tables at once (v71): the number with -5, -1, +1 and +5, and a button for the number that suits this computer (its
        /// processor threads minus 2). withHint: the line explaining it, on the setup screen. It changes a running event at once.
        /// </summary>
        private void DrawRegionalTablesAtOnce(UiStyles ui, List<Action> actions, bool withHint)
        {
            int threads = Environment.ProcessorCount;
            int suggestedTables = RegionalSim.SuggestedTablesAtOnce(threads);
            int tables = Mathf.Clamp(regTables, 1, RegionalSim.MaxTablesAtOnce); // your setting (the battery saver may play fewer)
            int playing = RegionalTablesAtOnce();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Tables at once", GUILayout.Width(150));
            foreach (int step in new[] { -5, -1 })
            {
                int d = step;
                GUI.enabled = tables > 1;
                if (GUILayout.Button(d.ToString(), GUILayout.Width(38), GUILayout.Height(24))) actions.Add(() => SetRegionalTables(tables + d));
            }
            GUI.enabled = true;
            GUILayout.Label(tables.ToString(), ui.Title, GUILayout.Width(36));
            foreach (int step in new[] { 1, 5 })
            {
                int d = step;
                GUI.enabled = tables < RegionalSim.MaxTablesAtOnce;
                if (GUILayout.Button("+" + d, GUILayout.Width(38), GUILayout.Height(24))) actions.Add(() => SetRegionalTables(tables + d));
            }
            GUI.enabled = tables != suggestedTables;
            if (GUILayout.Button("Use " + suggestedTables + " (best for this computer)", GUILayout.Width(250), GUILayout.Height(24)))
                actions.Add(() => SetRegionalTables(suggestedTables));
            GUI.enabled = true;
            if (playing < tables) GUILayout.Label("  Battery saver: " + playing + " at once for now.", Hint(ui));
            else if (!withHint && tables > threads) GUILayout.Label("  More than this computer's " + threads + " processor threads: rounds won't finish sooner.", ui.Error);
            GUILayout.EndHorizontal();
            if (withHint)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("", GUILayout.Width(150));
                if (tables > threads)
                    GUILayout.Label("This computer has " + threads + " processor threads. More tables than that won't finish a round sooner: each game "
                                    + "just gets slower. " + suggestedTables + " keeps it busy and leaves room for the screen and your own game.", ui.Error);
                else
                    GUILayout.Label("AI tables played at the same time in the background (1 to " + RegionalSim.MaxTablesAtOnce + "). This computer has " + threads
                                    + " processor threads, so " + suggestedTables + " is the most that helps. You can change it during the event too.", Hint(ui));
                GUILayout.EndHorizontal();
            }
            DrawSkipCpuRow(ui, actions, 150);
        }

        // ------------------------------------------------------------------ The AI players' decks (v69)

        /// <summary>How many AI players you chose for a deck (0 if none).</summary>
        private int RegionalFieldCount(string deckName)
        {
            int n;
            return deckName != null && regFieldCounts.TryGetValue(deckName, out n) ? n : 0;
        }

        private void SetRegionalFieldCount(string deckName, int n)
        {
            n = Mathf.Clamp(n, 0, MaxRegionalPlayers);
            if (n == 0) regFieldCounts.Remove(deckName);
            else regFieldCounts[deckName] = n;
            regFieldTexts.Remove(deckName);
            GUI.FocusControl(null);
        }

        /// <summary>From a deck's box: a number is used at once; an empty box is 0.</summary>
        private void SetRegionalFieldText(string deckName, string text)
        {
            regFieldTexts[deckName] = text;
            int n;
            if (string.IsNullOrEmpty(text)) regFieldCounts.Remove(deckName);
            else if (int.TryParse(text, out n))
            {
                n = Mathf.Clamp(n, 0, MaxRegionalPlayers);
                if (n == 0) regFieldCounts.Remove(deckName);
                else regFieldCounts[deckName] = n;
            }
        }

        private string RegionalFieldText(string deckName)
        {
            string t;
            if (regFieldTexts.TryGetValue(deckName, out t)) return t;
            int n = RegionalFieldCount(deckName);
            return n > 0 ? n.ToString() : "";
        }

        /// <summary>A deck's meta share in percent (0 if none).</summary>
        private double RegionalFieldPercent(string deckName)
        {
            double x;
            return deckName != null && regFieldPercents.TryGetValue(deckName, out x) ? x : 0;
        }

        private void SetRegionalFieldPercent(string deckName, double pct)
        {
            pct = Math.Round(Math.Max(0, Math.Min(100, pct)), 2);
            if (pct <= 0) regFieldPercents.Remove(deckName);
            else regFieldPercents[deckName] = pct;
            regFieldPctTexts.Remove(deckName);
            GUI.FocusControl(null);
        }

        /// <summary>From a deck's share box: "12.5" is used at once; an empty box is 0.</summary>
        private void SetRegionalFieldPercentText(string deckName, string text)
        {
            regFieldPctTexts[deckName] = text;
            double x;
            if (string.IsNullOrEmpty(text)) regFieldPercents.Remove(deckName);
            else if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out x))
            {
                x = Math.Max(0, Math.Min(100, x));
                if (x <= 0) regFieldPercents.Remove(deckName);
                else regFieldPercents[deckName] = x;
            }
        }

        private string RegionalFieldPercentText(string deckName)
        {
            string t;
            if (regFieldPctTexts.TryGetValue(deckName, out t)) return t;
            double x = RegionalFieldPercent(deckName);
            return x > 0 ? x.ToString("0.##", CultureInfo.InvariantCulture) : "";
        }

        /// <summary>The decks you chose as AI players per deck: the numbers you typed, or the meta shares turned into players (v70).</summary>
        private List<KeyValuePair<string, int>> RegionalChosenSeats(IList<string> deckNames, int aiSeats)
        {
            if (!regFieldByPercent)
                return deckNames.Where(n => RegionalFieldCount(n) > 0).Select(n => new KeyValuePair<string, int>(n, RegionalFieldCount(n))).ToList();
            var shares = deckNames.Where(n => RegionalFieldPercent(n) > 0).Select(n => new KeyValuePair<string, double>(n, RegionalFieldPercent(n))).ToList();
            return RegionalSim.SeatsFromShares(shares, aiSeats);
        }

        /// <summary>Why the chosen field can't start (too many players, or seats left with random picking off), or null.</summary>
        private string RegionalFieldProblem(IList<string> deckNames, int aiSeats)
        {
            if (regFieldByPercent)
            {
                double share = deckNames.Sum(n => RegionalFieldPercent(n));
                string shown = share.ToString("0.##", CultureInfo.InvariantCulture) + "%";
                if (share > 100.0001) return "The shares add up to " + shown + ", more than 100%.";
                if (!regFillRandom && Math.Abs(share - 100) > 0.0001)
                    return "The shares add up to " + shown + ": make them 100%, or let the rest of the AI players pick at random.";
                return null;
            }
            int assigned = deckNames.Sum(n => RegionalFieldCount(n));
            if (assigned > aiSeats)
                return "The decks you chose are for " + assigned.ToString("N0") + " AI players, but there are only " + aiSeats.ToString("N0") + ".";
            if (assigned < aiSeats && !regFillRandom)
                return assigned.ToString("N0") + " of the " + aiSeats.ToString("N0") + " AI players have a deck: choose decks for the other "
                       + (aiSeats - assigned).ToString("N0") + ", or let them pick at random.";
            return null;
        }

        /// <summary>
        /// The field: for each legal deck, how many AI players play it, or (v70) its share of the AI players in percent; and
        /// whether the AI players left over pick theirs at random from every legal deck.
        /// </summary>
        private void DrawRegionalField(UiStyles ui, List<string> deckNames, int aiSeats, List<Action> actions)
        {
            GUILayout.Space(4);
            GUILayout.Label("The AI players' decks", ui.Title);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Set them by", GUILayout.Width(150));
            int byPct = GUILayout.Toolbar(regFieldByPercent ? 1 : 0, new[] { "Number of AI players", "Meta share (% of the AI players)" }, GUILayout.Width(460));
            if (byPct != (regFieldByPercent ? 1 : 0)) actions.Add(() => { regFieldByPercent = byPct == 1; GUI.FocusControl(null); });
            GUILayout.EndHorizontal();
            GUILayout.Label(regFieldByPercent
                ? "Type each deck's share of the AI players in percent (12.5 is fine). Leave a deck empty for none."
                : "Type how many AI players play each deck. Leave a deck empty for none.", Hint(ui));
            bool fill = GUILayout.Toggle(regFillRandom, " The AI players left over pick their decks at random from all " + deckNames.Count + " legal decks");
            if (fill != regFillRandom) actions.Add(() => regFillRandom = fill);

            var seats = RegionalChosenSeats(deckNames, aiSeats);
            var seatsByDeck = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var seat in seats) seatsByDeck[seat.Key] = seat.Value;
            int[] steps = regFieldByPercent ? new[] { -5, -1, 1, 5 } : new[] { -10, -1, 1, 10 };
            regFieldScroll = GUILayout.BeginScrollView(regFieldScroll, GUILayout.Height(Mathf.Clamp(26f * deckNames.Count + 10f, 60f, 260f)));
            foreach (var name in deckNames)
            {
                string deckName = name;
                GUILayout.BeginHorizontal();
                GUILayout.Label(deckName, GUILayout.Width(300));
                string shown = regFieldByPercent ? RegionalFieldPercentText(deckName) : RegionalFieldText(deckName);
                string typedValue = GUILayout.TextField(shown, regFieldByPercent ? 6 : 5, GUILayout.Width(64), GUILayout.Height(22));
                if (typedValue != shown)
                {
                    if (regFieldByPercent)
                    {
                        string cleaned = new string((typedValue ?? "").Where(ch => char.IsDigit(ch) || ch == '.').ToArray());
                        actions.Add(() => SetRegionalFieldPercentText(deckName, cleaned));
                    }
                    else
                    {
                        string digitsOnly = new string((typedValue ?? "").Where(char.IsDigit).ToArray());
                        actions.Add(() => SetRegionalFieldText(deckName, digitsOnly));
                    }
                }
                if (regFieldByPercent) GUILayout.Label("%", GUILayout.Width(16));
                foreach (int step in steps)
                {
                    int d = step;
                    if (GUILayout.Button((d > 0 ? "+" : "") + d, GUILayout.Width(38), GUILayout.Height(22)))
                    {
                        if (regFieldByPercent) actions.Add(() => SetRegionalFieldPercent(deckName, RegionalFieldPercent(deckName) + d));
                        else actions.Add(() => SetRegionalFieldCount(deckName, RegionalFieldCount(deckName) + d));
                    }
                }
                int players;
                seatsByDeck.TryGetValue(deckName, out players);
                string note = regFieldByPercent
                    ? (RegionalFieldPercent(deckName) > 0 ? "  " + players.ToString("N0") + (players == 1 ? " AI player" : " AI players") : "")
                    : (players > 0 ? "  " + Pct1((double)players / Math.Max(1, aiSeats)) + " of the AI players" : "");
                GUILayout.Label(note, Hint(ui));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();

            int assigned = seats.Sum(kv => kv.Value);
            string problem = RegionalFieldProblem(deckNames, aiSeats);
            string shareNote = regFieldByPercent ? " (" + deckNames.Sum(n => RegionalFieldPercent(n)).ToString("0.##", CultureInfo.InvariantCulture) + "% in all)" : "";
            string summary = problem ?? (assigned == 0 ? "No decks chosen: every AI player picks one at random."
                : assigned == aiSeats ? "Every AI player plays a deck you chose (" + assigned.ToString("N0") + ")" + shareNote + "."
                : assigned.ToString("N0") + " AI players play the decks you chose" + shareNote + "; the other " + (aiSeats - assigned).ToString("N0") + " pick at random.");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear", GUILayout.Width(80)))
                actions.Add(() =>
                {
                    if (regFieldByPercent)
                    {
                        regFieldPercents.Clear();
                        regFieldPctTexts.Clear();
                    }
                    else
                    {
                        regFieldCounts.Clear();
                        regFieldTexts.Clear();
                    }
                    GUI.FocusControl(null);
                });
            GUILayout.Label(summary, problem != null ? ui.Error : Hint(ui));
            GUILayout.EndHorizontal();
        }

        private void SetRegionalAiOnly(bool aiOnly)
        {
            // The number of players in all stays the same: with you in it, one seat is yours.
            regAiOnly = aiOnly;
        }

        /// <summary>From a button: sets the number of players in all and shows it in the box.</summary>
        private void SetRegionalPlayers(int n)
        {
            regTotal = Mathf.Clamp(n, MinRegionalPlayers, MaxRegionalPlayers);
            regPlayersText = regTotal.ToString();
            // The box shows the new number even if you were typing in it.
            GUI.FocusControl(null);
        }

        /// <summary>From the box: a number in range is used at once; anything else waits (the Start button stays off).</summary>
        private void SetRegionalPlayersText(string text)
        {
            regPlayersText = text;
            int n;
            if (int.TryParse(text, out n) && n >= MinRegionalPlayers && n <= MaxRegionalPlayers) regTotal = n;
        }

        private bool RegionalPlayersValid()
        {
            int n;
            return int.TryParse(regPlayersText, out n) && n >= MinRegionalPlayers && n <= MaxRegionalPlayers;
        }

        private GUIStyle RegionalNumberStyle(UiStyles ui)
        {
            if (regNumberStyle == null)
            {
                regNumberStyle = new GUIStyle(GUI.skin.textField) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
            }
            return regNumberStyle;
        }

        /// <summary>The setup screen starts from your last event's settings.</summary>
        private void LoadRegionalSettings()
        {
            if (regSettingsLoaded) return;
            regSettingsLoaded = true;
            regTotal = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegPlayers, 32), MinRegionalPlayers, MaxRegionalPlayers);
            regPlayersText = regTotal.ToString();
            regRounds = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegRounds, 5), 1, MaxRegionalRounds);
            regBestOf3 = PlayerPrefs.GetInt(PrefRegBestOf3, 1) == 1;
            regExpanded = PlayerPrefs.GetInt(PrefRegExpanded, 0) == 1;
            regAiOnly = PlayerPrefs.GetInt(PrefRegAiOnly, 0) == 1;
            regExplore = PlayerPrefs.GetInt(PrefRegExplore, 1) == 1;
            regWatch = PlayerPrefs.GetInt(PrefRegWatch, 1) == 1;
            regFillRandom = PlayerPrefs.GetInt(PrefRegFillRandom, 1) == 1;
            regFieldByPercent = PlayerPrefs.GetInt(PrefRegFieldByPct, 0) == 1;
            regTopCut = PlayerPrefs.GetInt(PrefRegTopCut, 0);
            if (Array.IndexOf(RegionalCutSizes, regTopCut) < 0) regTopCut = 0;
            regCutBestOf3 = PlayerPrefs.GetInt(PrefRegCutBestOf3, 1) == 1;
            regDay2 = PlayerPrefs.GetInt(PrefRegDay2, 0) == 1;
            regAsymCut = PlayerPrefs.GetInt(PrefRegAsymCut, 0) == 1;
            regDay2Min = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegDay2Min, 256), MinRegionalPlayers, MaxRegionalPlayers);
            regDay2Rounds = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegDay2Rounds, 5), 1, MaxDay2Rounds);
            regDay2Points = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegDay2Points, 19), 0, 3 * MaxRegionalRounds);
            regTables = Mathf.Clamp(PlayerPrefs.GetInt(PrefRegTables, RegionalSim.SuggestedTablesAtOnce(Environment.ProcessorCount)), 1, RegionalSim.MaxTablesAtOnce);
            regFieldPercents.Clear();
            regFieldPctTexts.Clear();
            foreach (var entry in PlayerPrefs.GetString(PrefRegFieldPct, "").Split('\u001E'))
            {
                var parts = entry.Split('\u001F');
                double x;
                if (parts.Length == 2 && parts[0].Length > 0 && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out x) && x > 0)
                    regFieldPercents[parts[0]] = Math.Min(100, x);
            }
            regFieldCounts.Clear();
            regFieldTexts.Clear();
            foreach (var entry in PlayerPrefs.GetString(PrefRegField, "").Split('\u001E'))
            {
                var parts = entry.Split('\u001F');
                int n;
                if (parts.Length == 2 && parts[0].Length > 0 && int.TryParse(parts[1], out n) && n > 0) regFieldCounts[parts[0]] = Mathf.Min(n, MaxRegionalPlayers);
            }
        }

        private void SaveRegionalSettings()
        {
            PlayerPrefs.SetInt(PrefRegPlayers, regTotal);
            PlayerPrefs.SetInt(PrefRegRounds, regRounds);
            PlayerPrefs.SetInt(PrefRegBestOf3, regBestOf3 ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegExpanded, regExpanded ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegAiOnly, regAiOnly ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegExplore, regExplore ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegWatch, regWatch ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegFillRandom, regFillRandom ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegFieldByPct, regFieldByPercent ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegTopCut, regTopCut);
            PlayerPrefs.SetInt(PrefRegCutBestOf3, regCutBestOf3 ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegDay2, regDay2 ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegAsymCut, regAsymCut ? 1 : 0);
            PlayerPrefs.SetInt(PrefRegDay2Min, regDay2Min);
            PlayerPrefs.SetInt(PrefRegDay2Rounds, regDay2Rounds);
            PlayerPrefs.SetInt(PrefRegDay2Points, regDay2Points);
            PlayerPrefs.SetInt(PrefRegTables, Mathf.Clamp(regTables, 1, RegionalSim.MaxTablesAtOnce)); // your setting, not the battery saver's
            PlayerPrefs.SetString(PrefRegFieldPct, string.Join("\u001E", regFieldPercents.Select(kv => kv.Key + "\u001F" + kv.Value.ToString("R", CultureInfo.InvariantCulture)).ToArray()));
            PlayerPrefs.SetString(PrefRegField, string.Join("\u001E", regFieldCounts.Select(kv => kv.Key + "\u001F" + kv.Value).ToArray()));
            PlayerPrefs.Save();
        }
    }
}
