using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// The deck builder's "AI deck advice" dialog (v75), three ways:
    ///  - Improve this deck: up to 6 swaps for the deck in the editor, each with why the card goes in and why the other goes out;
    ///  - Build around a card: a whole deck around the card you pick (Load it into the editor, then save it);
    ///  - Counter a matchup: up to 5 swaps that help against a deck from your library.
    /// Apply changes one by one or all at once. "Test it" plays the deck before and after the changes against the same games
    /// (the classic bot on both sides) to see whether they help. See DeckAdvisor for what the advice is based on.
    /// </summary>
    public sealed partial class DeckBuilderScreen
    {
        /// <summary>The learning AI's brain, which the advisor reads (GameBootstrap sets it; the advice works without one too).</summary>
        public LearningBrain Brain;

        private const int ImproveMode = 0, AroundMode = 1, CounterMode = 2;
        private static readonly string[] AdviceModes = { "Improve this deck", "Build around a card", "Counter a matchup" };
        private static readonly string[] TrialGameNames = { "10 games", "20 games", "50 games" };
        private static readonly int[] TrialGameCounts = { 10, 20, 50 };
        private const int MaxImproveChanges = 6, MaxCounterChanges = 5;

        private bool adviceOpen;
        private int adviceMode;
        private DeckAdvice advice;
        /// <summary>The mode and opponent the shown advice was made with, and the deck it was made for.</summary>
        private int adviceRunMode;
        private AdvisorDeck adviceRunOpponent;
        private List<CardDef> adviceBase;
        private readonly HashSet<int> adviceApplied = new HashSet<int>();
        private string aroundSearch = "";
        private CardDef aroundCard;
        private int counterIndex = -1;
        private Vector2 adviceScroll;
        private List<AdvisorDeck> adviceLibrary;

        // The advice is worked out on another thread; the dialog picks it up at the start of a frame.
        private volatile bool adviceBusy;
        private bool busyShown;
        private DeckAdvice pendingAdvice;
        private readonly object adviceGate = new object();

        private DeckTrial trial;
        private DeckTrialStatus trialStatus;

        /// <summary>A deck trial is playing in the background (v120: the battery saver keeps the screen on for it).</summary>
        public bool TrialRunning
        {
            get
            {
                var t = trial;
                return t != null && t.Status().Running;
            }
        }
        private int trialGamesPick = 1;
        private string trialNote = "";

        private string aroundCacheKey;
        private List<CardDef> aroundCache = new List<CardDef>();
        private GUIStyle adviceWrap, adviceSmall, adviceBold;

        /// <summary>Stops a running test (leaving the deck builder).</summary>
        public void StopAdvisor()
        {
            if (trial != null) trial.Cancel();
        }

        private void OpenAdvice()
        {
            adviceOpen = true;
            adviceScroll = Vector2.zero;
        }

        private void CloseAdvice()
        {
            adviceOpen = false;
        }

        /// <summary>The decks the advisor learns from: every legal 60-card deck in the library (built in and yours).</summary>
        private List<AdvisorDeck> AdviceLibrary()
        {
            if (adviceLibrary != null) return adviceLibrary;
            var list = new List<AdvisorDeck>();
            foreach (var record in decks)
            {
                var problems = new List<string>();
                var cards = DeckLibrary.Build(db, record, problems);
                if (cards.Count == 60 && problems.Count == 0) list.Add(new AdvisorDeck(record.Name, cards));
            }
            adviceLibrary = list;
            return list;
        }

        /// <summary>Picks up finished advice and the test's progress. Only during Layout, so every pass of a frame draws the same controls.</summary>
        private void UpdateAdvice()
        {
            if (Event.current == null || Event.current.type != EventType.Layout) return;
            lock (adviceGate)
            {
                if (pendingAdvice != null)
                {
                    advice = pendingAdvice;
                    pendingAdvice = null;
                    adviceApplied.Clear();
                }
            }
            busyShown = adviceBusy;
            trialStatus = trial == null ? null : trial.Status();
        }

        private void RunAdvice()
        {
            if (adviceBusy) return;
            int mode = adviceMode;
            var cards = Expanded();
            string name = deckName;
            var fmt = format;
            var library = AdviceLibrary();
            var brain = Brain;
            var target = aroundCard;
            var opponent = counterIndex >= 0 && counterIndex < library.Count ? library[counterIndex] : null;
            if (mode == AroundMode && target == null) return;
            if (mode == CounterMode && opponent == null) return;

            if (trial != null) trial.Cancel();
            trial = null;
            trialStatus = null;
            trialNote = "";
            advice = null;
            adviceApplied.Clear();
            adviceRunMode = mode;
            adviceRunOpponent = opponent;
            adviceBase = cards;
            adviceBusy = true;
            busyShown = true;
            var worker = new System.Threading.Thread(() =>
            {
                DeckAdvice result;
                try
                {
                    var advisor = new DeckAdvisor(db, fmt, library, brain);
                    if (mode == AroundMode) result = advisor.BuildAround(target);
                    else if (mode == CounterMode) result = advisor.Counter(name, cards, opponent.Name, opponent.Cards, MaxCounterChanges);
                    else result = advisor.SuggestChanges(name, cards, MaxImproveChanges);
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    result = new DeckAdvice { Error = "The advisor ran into a problem: " + ex.Message };
                }
                lock (adviceGate) pendingAdvice = result;
                adviceBusy = false;
            });
            worker.IsBackground = true;
            worker.Name = "Deck advice";
            worker.Start();
        }

        // ------------------------------------------------------------------ Applying

        /// <summary>Removes one copy of a card: the same Pokémon, or any printing of a Trainer or Energy with that name.</summary>
        private void RemoveOneLike(CardDef card)
        {
            CardDef found = null;
            foreach (var d in order)
            {
                bool same = card.IsPokemon ? d.BaseKey == card.BaseKey : d.Name == card.Name;
                if (same && Count(d) > 0)
                {
                    found = d;
                    break;
                }
            }
            if (found != null) Remove(found);
        }

        private void ApplyAdviceChange(int index)
        {
            if (advice == null || index < 0 || index >= advice.Changes.Count || adviceApplied.Contains(index)) return;
            var change = advice.Changes[index];
            if (change.Cut != null)
                for (int i = 0; i < change.Count; i++) RemoveOneLike(change.Cut);
            if (change.Add != null)
            {
                var printing = art.ContainsKey(change.Add.BaseKey) ? ArtFor(change.Add) : change.Add;
                Add(printing, change.Count, true);
                if (!art.ContainsKey(change.Add.BaseKey)) art[change.Add.BaseKey] = printing;
            }
            adviceApplied.Add(index);
            SetStatus("Applied: " + change.Label + ".", false);
        }

        private void ApplyAllAdvice()
        {
            if (advice == null) return;
            // Cuts first, so the adds always have room.
            for (int i = 0; i < advice.Changes.Count; i++)
            {
                if (adviceApplied.Contains(i)) continue;
                var change = advice.Changes[i];
                if (change.Cut != null)
                    for (int k = 0; k < change.Count; k++) RemoveOneLike(change.Cut);
            }
            for (int i = 0; i < advice.Changes.Count; i++)
            {
                if (adviceApplied.Contains(i)) continue;
                var change = advice.Changes[i];
                if (change.Add != null)
                {
                    var printing = art.ContainsKey(change.Add.BaseKey) ? ArtFor(change.Add) : change.Add;
                    Add(printing, change.Count, true);
                    if (!art.ContainsKey(change.Add.BaseKey)) art[change.Add.BaseKey] = printing;
                }
                adviceApplied.Add(i);
            }
            SetStatus("Applied all " + advice.Changes.Count + " changes.", false);
        }

        private void LoadAdviceDeck()
        {
            if (advice == null || advice.Result == null || advice.Result.Count == 0) return;
            SetCards(Decklist.Group(advice.Result));
            editing = null;
            deckName = string.IsNullOrEmpty(advice.DeckName) ? "AI Deck" : advice.DeckName;
            SetFormat(advice.Format);
            adviceOpen = false;
            SetStatus("Loaded the AI's deck into the editor. Save it to keep it.", false);
        }

        // ------------------------------------------------------------------ Testing

        private List<AdvisorDeck> TrialOpponents()
        {
            var list = new List<AdvisorDeck>();
            if (adviceRunMode == CounterMode)
            {
                if (adviceRunOpponent != null) list.Add(adviceRunOpponent);
                return list;
            }
            var library = AdviceLibrary().Where(x => x.Name != deckName && x.Cards.All(c => Formats.Allows(format, c))).ToList();
            int n = Math.Min(4, library.Count);
            for (int i = 0; i < n; i++) list.Add(library[i * library.Count / n]);
            return list;
        }

        private void StartTrial()
        {
            if (advice == null || adviceBase == null || advice.Result == null) return;
            var opponents = TrialOpponents();
            if (opponents.Count == 0)
            {
                trialNote = "No legal decks in your library to test against.";
                return;
            }
            if (Decklist.Validate(adviceBase, format).Count > 0 || Decklist.Validate(advice.Result, format).Count > 0)
            {
                trialNote = "Both the deck and the changed deck need to be legal 60-card decks to test.";
                return;
            }
            if (trial != null) trial.Cancel();
            trial = new DeckTrial(StandardCards.CreateRegistry(), adviceBase, advice.Result, opponents, TrialGameCounts[trialGamesPick], Environment.TickCount);
            trialNote = "";
            trial.Start();
        }

        // ------------------------------------------------------------------ Drawing

        private static Rect AdviceRect()
        {
            float w = Mathf.Min(920f, UiScale.Width - 40f);
            float h = Mathf.Min(760f, UiScale.Height - 40f);
            return new Rect(UiScale.Width / 2f - w / 2f, UiScale.Height / 2f - h / 2f, w, h);
        }

        /// <summary>The mouse wheel over the dialog scrolls the dialog, not the panels behind it.</summary>
        private void HandleAdviceWheel(Event e)
        {
            if (e == null || e.type != EventType.ScrollWheel) return;
            if (!AdviceRect().Contains(e.mousePosition)) return;
            adviceScroll.y = Mathf.Max(0f, adviceScroll.y + e.delta.y * 20f);
            e.Use();
        }

        private GUIStyle AdviceWrap(UiStyles ui)
        {
            if (adviceWrap == null) adviceWrap = new GUIStyle(ui.Label) { wordWrap = true, richText = false };
            return adviceWrap;
        }

        private GUIStyle AdviceSmall(UiStyles ui)
        {
            if (adviceSmall == null)
            {
                adviceSmall = new GUIStyle(ui.Label) { wordWrap = true, fontSize = 12 };
                adviceSmall.normal.textColor = new Color(0.8f, 0.8f, 0.85f);
            }
            return adviceSmall;
        }

        private GUIStyle AdviceBold(UiStyles ui)
        {
            if (adviceBold == null) adviceBold = new GUIStyle(ui.Label) { fontStyle = FontStyle.Bold, wordWrap = true };
            return adviceBold;
        }

        private void DrawAdvice(UiStyles ui, List<System.Action> actions)
        {
            var r = AdviceRect();
            GUI.Box(r, GUIContent.none);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 12f, r.y + 10f, r.width - 24f, r.height - 20f));

            GUILayout.BeginHorizontal();
            GUILayout.Label("AI deck advice", ui.Title);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Close", GUILayout.Width(100))) actions.Add(() => CloseAdvice());
            GUILayout.EndHorizontal();

            GUI.enabled = !busyShown;
            int mode = GUILayout.Toolbar(adviceMode, AdviceModes);
            if (mode != adviceMode)
            {
                int picked = mode;
                actions.Add(() => adviceMode = picked);
            }
            GUI.enabled = true;
            GUILayout.Space(6);

            adviceScroll = GUILayout.BeginScrollView(adviceScroll);
            if (adviceMode == AroundMode) DrawAroundInputs(ui, actions);
            else if (adviceMode == CounterMode) DrawCounterInputs(ui, actions);
            else DrawImproveInputs(ui, actions);
            GUILayout.Space(10);
            DrawAdviceResult(ui, actions);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawImproveInputs(UiStyles ui, List<System.Action> actions)
        {
            GUILayout.Label("Looks at the deck in the editor (" + Total + " cards, " + Formats.Label(format) + ") and suggests up to " + MaxImproveChanges
                            + " swaps: what's missing (evolution lines, Energy for the attacks, draw, Pokémon search, gust, switching, recovery), "
                            + "what similar decks in your library agree on, and what the learning AI's games say about each card. "
                            + "A deck under 60 cards is filled; one over 60 is trimmed.", AdviceSmall(ui));
            GUI.enabled = !busyShown && Total > 0;
            if (GUILayout.Button(busyShown ? "Thinking…" : "Suggest changes", GUILayout.Height(30))) actions.Add(() => RunAdvice());
            GUI.enabled = true;
        }

        private void DrawAroundInputs(UiStyles ui, List<System.Action> actions)
        {
            GUILayout.Label("Pick a card. If decks in your library play it, the deck is their common core; if not, it's built from the card's "
                            + "evolution line, the Trainers your library plays most for each job and the Energy its attacks need. "
                            + "Then Load it into the editor and save it.", AdviceSmall(ui));
            string active = aroundSearch ?? "";
            GUILayout.BeginHorizontal();
            GUILayout.Label("Card name", GUILayout.Width(80));
            aroundSearch = GUILayout.TextField(aroundSearch ?? "");
            GUILayout.EndHorizontal();
            var matches = AroundMatches(active);
            if (active.Trim().Length < 2) GUILayout.Label("Type at least 2 letters of a card's name.", AdviceSmall(ui));
            else if (matches.Count == 0) GUILayout.Label("No " + Formats.Label(format) + " card has that in its name.", AdviceSmall(ui));
            foreach (var d in matches)
            {
                var card = d;
                bool selected = aroundCard != null && aroundCard.Key == card.Key;
                string label = card.Name + "   (" + card.SetCode + " " + card.Number + (card.IsPokemon ? ", " + StageLabel(card) : card.IsTrainer ? ", " + card.TrainerKind : ", Energy") + ")";
                if (GUILayout.Toggle(selected, label, ui.Button) && !selected) actions.Add(() => aroundCard = card);
            }
            GUI.enabled = !busyShown && aroundCard != null;
            if (GUILayout.Button(busyShown ? "Thinking…" : aroundCard == null ? "Pick a card above" : "Build a deck around " + aroundCard.Name, GUILayout.Height(30)))
                actions.Add(() => RunAdvice());
            GUI.enabled = true;
        }

        private static string StageLabel(CardDef d)
        {
            switch (d.Stage)
            {
                case Stage.Stage1: return "Stage 1";
                case Stage.Stage2: return "Stage 2";
                case Stage.Restored: return "Restored";
                default: return "Basic";
            }
        }

        /// <summary>Cards in the deck's format whose name contains the text (Pokémon first, up to 24).</summary>
        private List<CardDef> AroundMatches(string text)
        {
            string key = (int)format + "|" + (text ?? "").Trim().ToLowerInvariant();
            if (key == aroundCacheKey) return aroundCache;
            aroundCacheKey = key;
            aroundCache = new List<CardDef>();
            string needle = (text ?? "").Trim();
            if (needle.Length < 2) return aroundCache;
            aroundCache = pool
                .Where(d => d.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 && Formats.Allows(format, d))
                .OrderBy(d => d.IsPokemon ? 0 : d.IsTrainer ? 1 : 2)
                .ThenBy(d => d.Name.Length)
                .ThenBy(d => d.Name)
                .Take(24)
                .ToList();
            return aroundCache;
        }

        private void DrawCounterInputs(UiStyles ui, List<System.Action> actions)
        {
            GUILayout.Label("Pick the deck you want to beat. The advice looks at its main attacker (and its Weakness) and at what it relies on "
                            + "(Abilities, Tools, Special Energy, Items, Stadiums), then suggests up to " + MaxCounterChanges + " swaps for the deck in the editor.", AdviceSmall(ui));
            var library = AdviceLibrary();
            if (library.Count == 0) GUILayout.Label("No legal decks in your library yet.", AdviceSmall(ui));
            for (int i = 0; i < library.Count; i++)
            {
                int index = i;
                bool selected = counterIndex == index;
                if (GUILayout.Toggle(selected, library[i].Name, ui.Button) && !selected) actions.Add(() => counterIndex = index);
            }
            bool ready = counterIndex >= 0 && counterIndex < library.Count && Total > 0;
            GUI.enabled = !busyShown && ready;
            if (GUILayout.Button(busyShown ? "Thinking…" : ready ? "Suggest changes against " + library[counterIndex].Name : "Pick a deck above", GUILayout.Height(30)))
                actions.Add(() => RunAdvice());
            GUI.enabled = true;
        }

        private void DrawAdviceResult(UiStyles ui, List<System.Action> actions)
        {
            if (busyShown)
            {
                GUILayout.Label("Thinking…", ui.Title);
                return;
            }
            if (advice == null) return;
            if (!string.IsNullOrEmpty(advice.Error))
            {
                GUILayout.Label(advice.Error, ui.Error);
                return;
            }
            GUILayout.Label(advice.Title, ui.Title);
            foreach (var note in advice.Notes) GUILayout.Label(note, AdviceSmall(ui));
            GUILayout.Space(6);

            if (advice.IsWholeDeck)
            {
                if (advice.Changes.Count > 0)
                {
                    GUILayout.Label("Already made to the list:", AdviceBold(ui));
                    foreach (var change in advice.Changes) DrawChange(change, ui);
                }
                GUILayout.Label(advice.DeckName + " — " + advice.Result.Count + " cards", AdviceBold(ui));
                GUILayout.Label(Decklist.Format(null, Decklist.Group(advice.Result), advice.Format).Trim(), AdviceWrap(ui));
                if (GUILayout.Button("Load into the editor", GUILayout.Height(30))) actions.Add(() => LoadAdviceDeck());
                return;
            }

            if (advice.Changes.Count == 0) return;
            for (int i = 0; i < advice.Changes.Count; i++)
            {
                int index = i;
                var change = advice.Changes[i];
                GUILayout.BeginHorizontal();
                GUILayout.Label(change.Label, AdviceBold(ui));
                GUILayout.FlexibleSpace();
                bool applied = adviceApplied.Contains(index);
                GUI.enabled = !applied;
                if (GUILayout.Button(applied ? "Applied" : "Apply", GUILayout.Width(100))) actions.Add(() => ApplyAdviceChange(index));
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                DrawChangeReasons(change, ui);
                GUILayout.Space(4);
            }
            GUI.enabled = adviceApplied.Count < advice.Changes.Count;
            if (GUILayout.Button("Apply all", GUILayout.Height(28))) actions.Add(() => ApplyAllAdvice());
            GUI.enabled = true;
            GUILayout.Space(8);
            DrawTrial(ui, actions);
        }

        private void DrawChange(DeckChange change, UiStyles ui)
        {
            GUILayout.Label(change.Label, AdviceBold(ui));
            DrawChangeReasons(change, ui);
        }

        private void DrawChangeReasons(DeckChange change, UiStyles ui)
        {
            if (change.Add != null && !string.IsNullOrEmpty(change.Why)) GUILayout.Label("In: " + change.Why, AdviceSmall(ui));
            if (change.Cut != null && !string.IsNullOrEmpty(change.CutWhy)) GUILayout.Label("Out: " + change.Cut.Name + ": " + change.CutWhy + ".", AdviceSmall(ui));
        }

        private void DrawTrial(UiStyles ui, List<System.Action> actions)
        {
            GUILayout.Label("Test it", AdviceBold(ui));
            GUILayout.Label(adviceRunMode == CounterMode
                ? "Plays the deck before and after the changes against the same games with " + (adviceRunOpponent == null ? "that deck" : adviceRunOpponent.Name) + " (the classic bot plays both sides)."
                : "Plays the deck before and after the changes against the same games with up to 4 decks from your library (the classic bot plays both sides).",
                AdviceSmall(ui));
            var status = trialStatus;
            bool running = status != null && status.Running;
            GUILayout.BeginHorizontal();
            GUI.enabled = !running;
            GUILayout.Label("Games per opponent", GUILayout.Width(140));
            int pick = GUILayout.Toolbar(trialGamesPick, TrialGameNames, GUILayout.Width(300));
            if (pick != trialGamesPick)
            {
                int picked = pick;
                actions.Add(() => trialGamesPick = picked);
            }
            if (GUILayout.Button("Test before vs after", GUILayout.Width(180))) actions.Add(() => StartTrial());
            GUI.enabled = running;
            if (GUILayout.Button("Stop", GUILayout.Width(80))) actions.Add(() => StopAdvisor());
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(trialNote)) GUILayout.Label(trialNote, ui.Error);
            if (status == null) return;
            if (running) GUILayout.Label("Playing… " + status.Done + " / " + status.Total + " games", AdviceSmall(ui));
            if (status.GamesBefore > 0 || status.GamesAfter > 0)
            {
                GUILayout.Label("Before: " + Percent(status.RateBefore) + " wins (" + status.WinsBefore + "/" + status.GamesBefore + ")    After: "
                                + Percent(status.RateAfter) + " wins (" + status.WinsAfter + "/" + status.GamesAfter + ")",
                    status.RateAfter > status.RateBefore ? ui.Good : AdviceBold(ui));
                foreach (var line in status.Lines) GUILayout.Label(line, AdviceSmall(ui));
                if (status.Finished)
                    GUILayout.Label("A handful of games swings a lot: treat a few wins either way as noise, and a big gap over 50 games as a real signal.", AdviceSmall(ui));
            }
            if (status.Errors > 0) GUILayout.Label(status.Errors + " game(s) hit an error: " + status.LastError, ui.Error);
            if (status.Cancelled) GUILayout.Label("Stopped.", AdviceSmall(ui));
        }

        private static string Percent(double x)
        {
            return Mathf.RoundToInt((float)(x * 100)) + "%";
        }
    }
}
