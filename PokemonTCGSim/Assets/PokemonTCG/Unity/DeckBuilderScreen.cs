using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Deck builder: saved decks on the left, the card pool in the middle, the deck being edited on the right.
    ///
    /// With card images on: drag a card from the pool onto the deck to add a copy, drag a card from the deck back
    /// onto the pool to remove a copy, and click any card to open its card view (large card, previous/next,
    /// art picker, In deck count with - and +). Right-click does nothing.
    ///
    /// Each card can use any of its printings (other arts and reprints); the chosen printing is what the deck list
    /// saves and what shows in game. Decks are saved as TCG Live / Limitless style text. Only cards in the card pool
    /// (i.e. cards the engine implements) can be added.
    ///
    /// Each card is in one category: Standard (regulation mark H and later), Expanded (Black & White to G) or Unlimited
    /// (first printed before Black & White). Each deck has a format: a Standard deck takes Standard cards only, an
    /// Expanded deck takes Standard and Expanded cards, an Unlimited deck takes any card. The pool's tabs work the same
    /// way (Expanded lists Standard cards too, Unlimited lists everything) and follow the deck's format when it changes.
    /// </summary>
    public sealed partial class DeckBuilderScreen
    {
        private readonly CardDatabase db;
        /// <summary>One entry per card (main printings).</summary>
        private readonly List<CardDef> pool;

        private List<DeckRecord> decks = new List<DeckRecord>();
        private DeckRecord editing;
        private string deckName = "New Deck";
        private DeckFormat format = DeckFormat.Standard;
        /// <summary>The deck's custom format (v123), or null for a built-in one (format). While it's set, format is Unlimited.</summary>
        private CustomFormat custom;
        /// <summary>The deck's format: format, or the custom one.</summary>
        private GameFormat deckFmt = GameFormat.Standard;
        /// <summary>Set when the player presses "Formats..." (the menu opens the format editor and comes back here).</summary>
        public bool WantsFormats;
        /// <summary>A deck in the list waiting for "Delete?" to be confirmed (v123).</summary>
        private DeckRecord confirmDelete;
        private int hiddenBuiltIns;
        /// <summary>Pool tab shown, as a DeckFormat: 0 = Standard cards (H and later), 1 = every Expanded-legal card (Standard
        /// included), 2 = every card (Unlimited); 3 = the cards the deck's custom format allows (v123).</summary>
        private int category;
        private int standardCount, expandedCount, unlimitedCount;
        /// <summary>Printings in the deck, in the order they were added.</summary>
        private readonly List<CardDef> order = new List<CardDef>();
        /// <summary>Printing key -> copies.</summary>
        private readonly Dictionary<string, int> counts = new Dictionary<string, int>();
        /// <summary>Main printing key -> the printing (art) this deck uses for that card.</summary>
        private readonly Dictionary<string, CardDef> art = new Dictionary<string, CardDef>();

        private string search = "";
        private int filter;
        private static readonly string[] Filters = { "All", "Pokémon", "Trainer", "Energy" };
        private Vector2 deckListScroll, poolScroll, deckScroll;
        private bool importOpen;
        private bool modal;
        private string importText = "";
        private string status = "";
        private bool statusIsError;
        private CardDef hoverCard;
        private Vector2 mouse;

        // Card images
        private bool showImages = true;
        /// <summary>v168.1: the pool shows only the cards that have no image file of their own yet.</summary>
        private bool missingImagesOnly;
        private int imagesFound, altImagesFound;
        /// <summary>Images that may still be loaded this frame (loading is spread over frames to keep the UI smooth).</summary>
        private int loadBudget;
        private const int ImagesPerFrame = 3;
        private const float Gap = 6f;
        private const float Scrollbar = 18f;
        private const float PoolTileW = 100f, PoolTileH = 140f;
        private const float DeckTileW = 78f, DeckTileH = 109f;
        private const float TileButtonsH = 20f;

        // Drag and drop
        private enum Zone { None, Pool, Deck }
        private const float DragThreshold = 6f;
        /// <summary>Card under the mouse button: a main printing from the pool, or a printing from the deck.</summary>
        private CardDef pressCard;
        private Zone pressZone;
        private Vector2 pressPos;
        private bool dragging;
        private Rect poolPanel, deckPanel;

        // Card view
        private CardDef detailCard;
        private List<CardDef> detailList = new List<CardDef>();
        private bool artMenuOpen;
        private Vector2 artScroll;

        private GUIStyle placeholderName, hint, arrowStyle, bigButton, bigLabel, richLabel, artItem, bigHint;

        /// <summary>Set when the player presses "Back to menu".</summary>
        public bool WantsClose;

        public DeckBuilderScreen(CardDatabase db)
        {
            this.db = db;
            pool = db.Cards
                .OrderBy(d => d.IsPokemon ? 0 : d.IsTrainer ? 1 : 2)
                .ThenBy(d => d.IsTrainer ? (int)d.TrainerKind : 0)
                .ThenBy(d => d.Name)
                .ToList();
            standardCount = pool.Count(d => Formats.CategoryOf(d) == CardCategory.Standard);
            unlimitedCount = pool.Count(d => Formats.CategoryOf(d) == CardCategory.Unlimited);
            expandedCount = pool.Count - standardCount - unlimitedCount;
            Reload();
            NewDeck();
            ReportImages();
        }

        public void Reload()
        {
            decks = DeckLibrary.LoadAll();
            hiddenBuiltIns = DeckLibrary.HiddenBuiltIns().Count;
            adviceLibrary = null;
            confirmDelete = null;
            // A custom format edited or deleted meanwhile (v123): pick up its new rules, or fall back to Unlimited.
            if (custom != null)
            {
                var now = CustomFormats.Find(custom.Name);
                if (now != null) SetCustomFormat(now);
                else SetFormat(DeckFormat.Unlimited);
            }
        }

        // ------------------------------------------------------------------ Images

        /// <summary>Counts the cards that have an image and logs files that don't match any card.</summary>
        private void ReportImages()
        {
            var files = CardVisuals.ImageFiles();
            imagesFound = pool.Count(d => files.ContainsKey(d.Key));
            altImagesFound = db.All.Count(d => d.IsAlternatePrint && files.ContainsKey(d.Key));
            var known = new HashSet<string>(db.All.Select(d => d.Key));
            var unmatched = files.Where(kv => !known.Contains(kv.Key)).Select(kv => Path.GetFileName(kv.Value)).OrderBy(n => n).ToList();
            var sb = new StringBuilder();
            sb.Append("[PokemonTCG] Card images: " + imagesFound + " of " + pool.Count + " cards have an image, plus " + altImagesFound
                      + " alternate arts, in " + CardVisuals.ImageFolder + ".");
            if (unmatched.Count > 0)
                sb.Append("\n" + unmatched.Count + " image file(s) don't match any printing of a card in the pool: "
                          + string.Join(", ", unmatched.Take(30).ToArray()) + (unmatched.Count > 30 ? ", ..." : ""));
            if (CardVisuals.SkippedFiles.Count > 0)
                sb.Append("\n" + CardVisuals.SkippedFiles.Count + " file(s) skipped: " + string.Join("; ", CardVisuals.SkippedFiles.Take(30).ToArray()));
            Debug.Log(sb.ToString());
        }

        private void ReloadImages()
        {
            CardVisuals.ReloadImages();
            ReportImages();
            SetStatus("Card images reloaded: " + imagesFound + " of " + pool.Count + " cards have an image, plus " + altImagesFound + " alternate arts.", false);
        }

        // ------------------------------------------------------------------ Deck model

        private void NewDeck()
        {
            editing = null;
            deckName = "New Deck";
            SetFormat(DeckFormat.Standard);
            order.Clear();
            counts.Clear();
            art.Clear();
        }

        private void Load(DeckRecord deck)
        {
            var problems = new List<string>();
            var entries = Decklist.Parse(deck.Text, db, problems);
            SetCards(entries);
            editing = deck;
            deckName = deck.Name;
            var named = deck.ResolvedFormat;
            if (named != null && named.IsCustom) SetCustomFormat(named.Custom);
            // A list with G-mark (Expanded) or Unlimited-only cards but no "# Format:" line is in the format those cards need.
            else SetFormat(Wider(deck.Format, Formats.Needed(entries.Select(e => e.Card))));
            if (named == null) problems.Insert(0, "Its format \"" + deck.FormatName + "\" doesn't exist any more: pick a format and save it again.");
            SetStatus(problems.Count == 0 ? "Loaded " + deck.Name + "." : string.Join("\n", problems.ToArray()), problems.Count > 0);
        }

        /// <summary>Sets the deck's format and turns the card pool to that format's tab, so it shows every card the deck
        /// can take. The pool tabs can still be switched by hand afterwards.</summary>
        private void SetFormat(DeckFormat newFormat)
        {
            format = newFormat;
            custom = null;
            deckFmt = GameFormat.Of(newFormat);
            category = (int)newFormat;
        }

        /// <summary>A custom format (v123): the pool shows the cards it allows.</summary>
        private void SetCustomFormat(CustomFormat f)
        {
            if (f == null)
            {
                SetFormat(DeckFormat.Standard);
                return;
            }
            format = DeckFormat.Unlimited;
            custom = f;
            deckFmt = GameFormat.Of(f);
            category = 3;
        }

        /// <summary>Every format a deck can be built for: the built-in ones, then yours.</summary>
        private List<GameFormat> AllFormats()
        {
            var list = new List<GameFormat>(GameFormat.BuiltIns);
            foreach (var f in CustomFormats.All) list.Add(GameFormat.Of(f));
            return list;
        }

        /// <summary>Replaces the deck's contents. The printings in the list become the deck's art choices.</summary>
        private void SetCards(List<DeckEntry> entries)
        {
            order.Clear();
            counts.Clear();
            art.Clear();
            foreach (var e in entries)
            {
                Add(e.Card, e.Count, false);
                if (!art.ContainsKey(e.Card.BaseKey)) art[e.Card.BaseKey] = e.Card;
            }
        }

        /// <summary>Copies of this exact printing.</summary>
        private int Count(CardDef printing)
        {
            int n;
            return counts.TryGetValue(printing.Key, out n) ? n : 0;
        }

        /// <summary>Copies of the card in any printing.</summary>
        private int CountCard(CardDef card)
        {
            string key = card.BaseKey;
            return order.Where(d => d.BaseKey == key).Sum(d => Count(d));
        }

        private int Total { get { return counts.Values.Sum(); } }

        private int CountByName(string name)
        {
            return order.Where(d => d.Name == name).Sum(d => Count(d));
        }

        /// <summary>
        /// Copies that count toward the copy limit for this name: basic Energy never does, so Neo Genesis's Special Darkness
        /// Energy and Metal Energy are limited on their own, apart from the basic Energy of the same name (v138).
        /// </summary>
        private int LimitedCountByName(string name)
        {
            return order.Where(d => d.Name == name && !(d.IsEnergy && d.IsBasicEnergy)).Sum(d => Count(d));
        }

        private bool CanAdd(CardDef d)
        {
            return AddBlockReason(d) == null;
        }

        /// <summary>Why another copy can't go in the deck, or null if it can.</summary>
        private string AddBlockReason(CardDef d)
        {
            var why = deckFmt.WhyNotAllowed(d);
            if (why != null) return why;
            if (Total >= deckFmt.DeckSize) return "Deck is full";
            if (d.IsAceSpec && order.Where(x => x.IsAceSpec).Sum(x => Count(x)) >= 1) return "1 ACE SPEC only";
            if (d.IsPokemon && d.IsRadiant && order.Where(x => x.IsPokemon && x.IsRadiant).Sum(x => Count(x)) >= 1) return "1 Radiant Pokémon only";
            if (d.IsPrismStar && CountByName(d.Name) >= 1) return "1 Prism Star card of each name only";
            if (d.OnePerDeck && CountByName(d.Name) >= 1) return "1 copy only";
            if (Decklist.UnownRuleBlocks(order.SelectMany(x => Enumerable.Repeat(x, Count(x))), d)) return "4 Unown in all only";
            int maxCopies = custom != null ? custom.MaxCopies : 4;
            if (!(d.IsEnergy && d.IsBasicEnergy) && !d.AnyNumberInDeck && LimitedCountByName(d.Name) >= maxCopies) return maxCopies == 1 ? "Singleton: 1 copy only" : maxCopies + " copies already";
            return null;
        }

        private void Add(CardDef d, int n, bool enforceLimits)
        {
            for (int i = 0; i < n; i++)
            {
                if (enforceLimits && !CanAdd(d)) return;
                if (!counts.ContainsKey(d.Key))
                {
                    counts[d.Key] = 0;
                    order.Add(d);
                }
                counts[d.Key]++;
            }
        }

        /// <summary>Removes one copy of this exact printing.</summary>
        private void Remove(CardDef d)
        {
            int n = Count(d);
            if (n <= 0) return;
            if (n == 1)
            {
                counts.Remove(d.Key);
                order.Remove(d);
            }
            else counts[d.Key] = n - 1;
        }

        /// <summary>The printing (art) this deck uses for a card.</summary>
        private CardDef ArtFor(CardDef card)
        {
            CardDef p;
            if (art.TryGetValue(card.BaseKey, out p)) return p;
            // v115: copies already in the deck keep their printing; otherwise the set the pool is limited to picks it.
            string key = card.BaseKey;
            var inDeck = order.FirstOrDefault(d => d.BaseKey == key);
            if (inDeck != null) return inDeck;
            return SetPrintingOf(card) ?? db.MainPrinting(card);
        }

        /// <summary>Adds one copy of a card in the deck's chosen art.</summary>
        private void AddCard(CardDef card)
        {
            Add(ArtFor(card), 1, true);
        }

        /// <summary>Removes one copy of a card, from the chosen art first.</summary>
        private void RemoveCard(CardDef card)
        {
            string key = card.BaseKey;
            var inDeck = order.Where(d => d.BaseKey == key).ToList();
            if (inDeck.Count == 0) return;
            var chosen = ArtFor(card);
            Remove(inDeck.FirstOrDefault(d => d.Key == chosen.Key) ?? inDeck[inDeck.Count - 1]);
        }

        /// <summary>Uses this printing for the card: every copy already in the deck switches to it.</summary>
        private void SetArt(CardDef card, CardDef printing)
        {
            string key = card.BaseKey;
            art[key] = printing;
            int copies = CountCard(card);
            if (copies == 0) return;
            int at = order.FindIndex(d => d.BaseKey == key);
            foreach (var d in order.Where(x => x.BaseKey == key).ToList())
            {
                counts.Remove(d.Key);
                order.Remove(d);
            }
            order.Insert(Mathf.Clamp(at, 0, order.Count), printing);
            counts[printing.Key] = copies;
        }

        private List<DeckEntry> Entries()
        {
            return order.Select(d => new DeckEntry { Card = d, Count = Count(d) }).Where(e => e.Count > 0).ToList();
        }

        private List<CardDef> Expanded()
        {
            return Decklist.Expand(Entries());
        }

        private void SetStatus(string text, bool error)
        {
            status = text;
            statusIsError = error;
        }

        private static string PrintLabel(CardDef d)
        {
            return d.SetCode + " " + d.Number + (string.IsNullOrEmpty(d.PrintSetName) ? "" : "  ·  " + d.PrintSetName);
        }

        // ------------------------------------------------------------------ Drawing

        public void Draw()
        {
            var ui = UiStyles.Get();
            var ev = Event.current;
            mouse = ev.mousePosition;
            // Right and middle clicks do nothing anywhere in the deck builder (IMGUI buttons would otherwise react to them).
            if (ev.button != 0 && (ev.type == EventType.MouseDown || ev.type == EventType.MouseUp || ev.type == EventType.MouseDrag || ev.type == EventType.ContextClick))
                ev.Use();
            if (ev.type == EventType.Repaint)
            {
                hoverCard = null;
                loadBudget = ImagesPerFrame;
            }
            bool images = showImages;

            // IMGUI draws each frame in several passes; collect clicks and apply them after drawing.
            var actions = new List<System.Action>();

            float pad = 10f;
            float leftW = 240f;
            float rest = UiScale.Width - leftW - pad * 4;
            float poolW = Mathf.Max(320f, rest * 0.5f);
            float deckW = Mathf.Max(320f, rest - poolW);
            float h = UiScale.Height - pad * 2;
            poolPanel = new Rect(pad * 2 + leftW, pad, poolW, h);
            deckPanel = new Rect(pad * 3 + leftW + poolW, pad, deckW, h);

            HandleDrag(ev, actions);
            if (importOpen) HandleImportWheel(ev);
            if (adviceOpen) HandleAdviceWheel(ev);
            UpdateAdvice();

            modal = importOpen || adviceOpen || detailCard != null || setMenuOpen;
            GUI.enabled = !modal;
            DrawDeckList(new Rect(pad, pad, leftW, h), ui, actions);
            DrawPool(poolPanel, ui, actions, search, filter, images);
            DrawDeck(deckPanel, ui, actions, images);
            GUI.enabled = true;

            if (importOpen) DrawImport(ui, actions);
            else if (adviceOpen) DrawAdvice(ui, actions);
            else if (detailCard != null) DrawDetail(ui, actions);
            else if (setMenuOpen) DrawSetMenu(ui, actions);
            else if (dragging && pressCard != null) DrawDragGhost(ui);
            else if (hoverCard != null) DrawHover(ui, hoverCard);

            foreach (var a in actions) a();
        }

        /// <summary>
        /// A press on a card becomes a drag once the mouse moves a few pixels. Releasing a drag over the deck adds a
        /// copy (from the pool) and over the pool removes one (from the deck); releasing without dragging opens the card view.
        /// </summary>
        private void HandleDrag(Event ev, List<System.Action> actions)
        {
            if (pressCard == null) return;
            if (ev.type == EventType.MouseDown)
            {
                // The button was released somewhere we didn't hear about (outside the window): forget the old press.
                pressCard = null;
                dragging = false;
                return;
            }
            if (ev.type == EventType.MouseDrag)
            {
                if (!dragging && (ev.mousePosition - pressPos).sqrMagnitude > DragThreshold * DragThreshold) dragging = true;
                ev.Use();
                return;
            }
            if (ev.rawType != EventType.MouseUp || ev.button != 0) return;

            var card = pressCard;
            var zone = pressZone;
            bool wasDrag = dragging;
            var pos = ev.mousePosition;
            pressCard = null;
            dragging = false;
            if (wasDrag)
            {
                if (zone == Zone.Pool && deckPanel.Contains(pos)) actions.Add(() => AddCard(card));
                else if (zone == Zone.Deck && poolPanel.Contains(pos)) actions.Add(() => Remove(card));
            }
            else
            {
                actions.Add(() => OpenDetail(card, zone));
            }
            ev.Use();
        }

        private void DrawDeckList(Rect area, UiStyles ui, List<System.Action> actions)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            GUILayout.Label("Decks", ui.Title);
            deckListScroll = GUILayout.BeginScrollView(deckListScroll);
            foreach (var d in decks)
            {
                var deck = d;
                bool selected = editing != null && editing.FilePath == deck.FilePath && editing.Name == deck.Name && editing.BuiltIn == deck.BuiltIn;
                if (confirmDelete == deck)
                {
                    // v123: delete from the list, after a second press.
                    GUILayout.Label((deck.BuiltIn ? "Remove the built-in deck \"" : "Delete \"") + deck.Name + "\"?", ui.Error);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button(deck.BuiltIn ? "Remove" : "Delete")) actions.Add(() => DeleteFromList(deck));
                    if (GUILayout.Button("Cancel")) actions.Add(() => confirmDelete = null);
                    GUILayout.EndHorizontal();
                    continue;
                }
                GUILayout.BeginHorizontal();
                if (GUILayout.Toggle(selected, deck.Label, ui.Button) && !selected) actions.Add(() => Load(deck));
                if (GUILayout.Button("X", GUILayout.Width(26))) actions.Add(() => confirmDelete = deck);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            if (GUILayout.Button("New deck")) actions.Add(NewDeck);
            int hiddenCount = hiddenBuiltIns;
            if (hiddenCount > 0 && GUILayout.Button("Restore built-in decks (" + hiddenCount + ")")) actions.Add(RestoreBuiltIns);
            if (GUILayout.Button("Formats...")) actions.Add(() => WantsFormats = true);
            GUILayout.Space(8);
            if (GUILayout.Button("Back to menu")) actions.Add(() => WantsClose = true);
            GUILayout.EndArea();
        }

        private void DrawPool(Rect area, UiStyles ui, List<System.Action> actions, string activeSearch, int activeFilter, bool images)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            GUILayout.Label("Card pool (" + pool.Count + " cards implemented)", ui.Title);
            // The tabs are cumulative, like the formats: Expanded shows Standard cards too, Unlimited shows every card.
            var tabs = new List<string>
            {
                "Standard (" + standardCount + ")",
                "Expanded (" + (standardCount + expandedCount) + ")",
                "Unlimited (" + pool.Count + ")"
            };
            // A custom format's deck (v123): a fourth tab with exactly the cards it allows.
            if (custom != null) tabs.Add(custom.Name + " (" + CustomPoolCount() + ")");
            int newCategory = GUILayout.Toolbar(Mathf.Min(category, tabs.Count - 1), tabs.ToArray());
            if (newCategory != category) actions.Add(() => category = newCategory);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search", GUILayout.Width(50));
            search = GUILayout.TextField(search ?? "");
            GUILayout.EndHorizontal();
            filter = GUILayout.Toolbar(filter, Filters);
            DrawSetRow(ui, actions);

            GUILayout.BeginHorizontal();
            bool toggled = GUILayout.Toggle(images, " Card images", GUILayout.Width(120));
            if (toggled != images) actions.Add(() => showImages = toggled);
            GUILayout.Label(imagesFound + " of " + pool.Count + " cards have an image" + (altImagesFound > 0 ? " (+" + altImagesFound + " alternate arts)" : ""), Hint(ui));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Reload images", GUILayout.Width(120))) actions.Add(ReloadImages);
            GUILayout.EndHorizontal();
            bool missingToggled = GUILayout.Toggle(missingImagesOnly, " Only cards missing an image (" + (pool.Count - imagesFound) + ")");
            if (missingToggled != missingImagesOnly) actions.Add(() => { missingImagesOnly = missingToggled; poolScroll = Vector2.zero; });
            if (imagesFound == 0)
                GUILayout.Label("No card images found. Put PNG or JPG files named like TWM_130.png in Assets/StreamingAssets/CardImages.", Hint(ui));
            if (custom != null && category < 3)
                GUILayout.Label("Showing a built-in format's cards. This deck is for " + custom.Name + ": only the cards on its tab can go in.", Hint(ui));
            if (custom == null && category == 1 && format == DeckFormat.Standard)
                GUILayout.Label("Showing every Expanded card. Only the Standard ones (H and later) can go in a Standard deck; set this deck's format to Expanded (right) to use the rest.", Hint(ui));
            if (custom == null && category == 2 && format != DeckFormat.Unlimited)
                GUILayout.Label("Showing every card. Cards first printed before Black & White are Unlimited only; set this deck's format to Unlimited (right) to use them.", Hint(ui));
            GUILayout.Label(images
                ? "Drag a card onto your deck to add it, or from your deck back here to remove it. Click a card to see it and pick its art."
                : "Press + to add a card. Click a card's name to see it and pick its art.", Hint(ui));

            var cards = pool.Where(c => MatchesFilter(c, activeSearch, activeFilter, category) && InSetFilter(c) && InImageFilter(c)).ToList();
            if (missingImagesOnly)
                GUILayout.Label(cards.Count == 0 ? "Every card shown here has an image." : cards.Count + " cards shown here have no image yet.", Hint(ui));
            if (images)
            {
                DrawPoolGrid(TakeRemainingRect(), cards, ui);
                GUILayout.EndArea();
                return;
            }

            poolScroll = GUILayout.BeginScrollView(poolScroll);
            foreach (var c in cards)
            {
                var card = c;
                GUILayout.BeginHorizontal();
                GUI.enabled = !modal && CanAdd(ArtFor(card));
                if (GUILayout.Button("+", GUILayout.Width(28))) actions.Add(() => AddCard(card));
                GUI.enabled = !modal;
                int n = CountCard(card);
                var shownArt = ArtFor(card);
                if (GUILayout.Button((n > 0 ? n + "x  " : "") + card.Name + "   <size=11>" + shownArt.SetCode + " " + shownArt.Number + " - " + CardText.Short(card) + "</size>",
                    RichLabel(ui)))
                    actions.Add(() => OpenDetail(card, Zone.Pool));
                GUILayout.EndHorizontal();
                TrackHover(ArtFor(card));
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private int customPoolCount = -1;
        private CustomFormat customPoolFor;

        private int CustomPoolCount()
        {
            if (customPoolFor != custom || customPoolCount < 0)
            {
                customPoolFor = custom;
                customPoolCount = custom == null ? 0 : pool.Count(d => deckFmt.Allows(d));
            }
            return customPoolCount;
        }

        private bool MatchesFilter(CardDef d, string text, int kind, int cardCategory)
        {
            // Cumulative tabs: the tab shows every card its format allows (Standard; Standard + Expanded; everything); the
            // fourth, a custom format's cards (v123).
            if (cardCategory >= 3 ? custom == null || !deckFmt.Allows(d) : !Formats.Allows((DeckFormat)cardCategory, d)) return false;
            if (kind == 1 && !d.IsPokemon) return false;
            if (kind == 2 && !d.IsTrainer) return false;
            if (kind == 3 && !d.IsEnergy) return false;
            if (!string.IsNullOrEmpty(text) && d.Name.IndexOf(text, System.StringComparison.OrdinalIgnoreCase) < 0) return false;
            return true;
        }

        /// <summary>v168.1: with "Only cards missing an image" on, a card passes only if it has no image file of its own.</summary>
        private bool InImageFilter(CardDef card)
        {
            return !missingImagesOnly || !CardVisuals.HasOwnImageFile(card);
        }

        private void TrackHover(CardDef card)
        {
            if (Event.current.type == EventType.Repaint && !modal && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                hoverCard = card;
        }

        private void DrawDeck(Rect area, UiStyles ui, List<System.Action> actions, bool images)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Deck name", GUILayout.Width(80));
            deckName = GUILayout.TextField(deckName ?? "");
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Format", GUILayout.Width(80));
            // The built-in formats and your custom ones (v123).
            var formats = AllFormats();
            int current = custom != null ? formats.FindIndex(f => f.Custom == custom) : (int)format;
            if (current < 0) current = (int)format;
            int newFormat = GUILayout.SelectionGrid(current, formats.Select(f => f.Name).ToArray(), Mathf.Min(4, formats.Count));
            if (newFormat != current && newFormat >= 0 && newFormat < formats.Count)
            {
                var picked = formats[newFormat];
                actions.Add(() => { if (picked.IsCustom) SetCustomFormat(picked.Custom); else SetFormat(picked.BuiltIn); });
            }
            if (GUILayout.Button("Formats...", GUILayout.Width(90))) actions.Add(() => WantsFormats = true);
            GUILayout.EndHorizontal();
            GUILayout.Label(custom != null ? FormatSummary(custom)
                : format == DeckFormat.Standard ? "H and later only" : format == DeckFormat.Expanded ? "Black & White and later" : "any card in the pool", Hint(ui));

            var entries = Entries();
            int pokemon = entries.Where(e => e.Card.IsPokemon).Sum(e => e.Count);
            int trainers = entries.Where(e => e.Card.IsTrainer).Sum(e => e.Count);
            int energy = entries.Where(e => e.Card.IsEnergy).Sum(e => e.Count);
            GUILayout.Label("Pokémon " + pokemon + "   Trainer " + trainers + "   Energy " + energy + "   Total " + (pokemon + trainers + energy) + " / " + deckFmt.DeckSize, ui.Title);

            var problems = deckFmt.Validate(Expanded());
            if (problems.Count == 0) GUILayout.Label("Legal for " + deckFmt.Name + " and ready to play.", ui.Good);
            else foreach (var p in problems) GUILayout.Label(p, ui.Error);

            if (images)
            {
                DrawDeckGrid(TakeRemainingRect(), entries, ui, actions);
            }
            else
            {
                deckScroll = GUILayout.BeginScrollView(deckScroll);
                DrawSection("Pokémon", entries.Where(e => e.Card.IsPokemon), ui, actions);
                DrawSection("Trainer", entries.Where(e => e.Card.IsTrainer), ui, actions);
                DrawSection("Energy", entries.Where(e => e.Card.IsEnergy), ui, actions);
                GUILayout.EndScrollView();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save")) actions.Add(() => Save(false));
            if (GUILayout.Button("Save as copy")) actions.Add(() => Save(true));
            GUI.enabled = !modal && editing != null && !editing.BuiltIn;
            if (GUILayout.Button("Delete")) actions.Add(DeleteCurrent);
            GUI.enabled = !modal;
            if (GUILayout.Button("Clear")) actions.Add(() => { order.Clear(); counts.Clear(); art.Clear(); });
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Import deck list")) actions.Add(() => { importOpen = true; importText = ""; importScroll = Vector2.zero; });
            if (GUILayout.Button("Copy deck list")) actions.Add(CopyToClipboard);
            GUILayout.EndHorizontal();
            // The AI's advice knows the built-in formats' 60-card, 4-copy decks, not custom formats (v123).
            GUI.enabled = !modal && custom == null;
            if (GUILayout.Button(custom == null ? "AI deck advice" : "AI deck advice (built-in formats only)")) actions.Add(() => OpenAdvice());
            GUI.enabled = !modal;

            if (!string.IsNullOrEmpty(status)) GUILayout.Label(status, statusIsError ? ui.Error : ui.Label);
            GUILayout.EndArea();
        }

        private void DrawSection(string title, IEnumerable<DeckEntry> items, UiStyles ui, List<System.Action> actions)
        {
            var list = items.ToList();
            if (list.Count == 0) return;
            GUILayout.Label(title + " (" + list.Sum(e => e.Count) + ")", ui.Title);
            foreach (var e in list)
            {
                var card = e.Card;
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-", GUILayout.Width(28))) actions.Add(() => Remove(card));
                GUI.enabled = !modal && CanAdd(card);
                if (GUILayout.Button("+", GUILayout.Width(28))) actions.Add(() => Add(card, 1, true));
                GUI.enabled = !modal;
                if (GUILayout.Button(e.Count + "  " + card.Name + "   <size=11>" + card.SetCode + " " + card.Number + "</size>", RichLabel(ui)))
                    actions.Add(() => OpenDetail(card, Zone.Deck));
                GUILayout.EndHorizontal();
                TrackHover(card);
            }
        }

        // ------------------------------------------------------------------ Card image grids

        /// <summary>The rest of the current GUILayout area, for a hand-laid-out scroll view.</summary>
        private static Rect TakeRemainingRect()
        {
            return GUILayoutUtility.GetRect(10f, 10000f, 10f, 10000f, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
        }

        private void DrawPoolGrid(Rect view, List<CardDef> cards, UiStyles ui)
        {
            int perRow = Mathf.Max(1, Mathf.FloorToInt((view.width - Scrollbar + Gap) / (PoolTileW + Gap)));
            int rows = (cards.Count + perRow - 1) / perRow;
            var content = new Rect(0f, 0f, Mathf.Max(1f, view.width - Scrollbar), rows * (PoolTileH + Gap));
            bool mouseInView = view.Contains(Event.current.mousePosition);
            poolScroll = GUI.BeginScrollView(view, poolScroll, content);
            for (int k = 0; k < cards.Count; k++)
            {
                var r = new Rect((k % perRow) * (PoolTileW + Gap), (k / perRow) * (PoolTileH + Gap), PoolTileW, PoolTileH);
                if (r.yMax < poolScroll.y || r.y > poolScroll.y + view.height) continue;
                var card = cards[k];
                Tile(r, card, ArtFor(card), CountCard(card), mouseInView, Zone.Pool, ui);
            }
            GUI.EndScrollView();
        }

        private void DrawDeckGrid(Rect view, List<DeckEntry> entries, UiStyles ui, List<System.Action> actions)
        {
            const float headerH = 26f;
            int perRow = Mathf.Max(1, Mathf.FloorToInt((view.width - Scrollbar + Gap) / (DeckTileW + Gap)));
            float rowH = DeckTileH + TileButtonsH + Gap;
            var sections = new List<KeyValuePair<string, List<DeckEntry>>>
            {
                new KeyValuePair<string, List<DeckEntry>>("Pokémon", entries.Where(e => e.Card.IsPokemon).ToList()),
                new KeyValuePair<string, List<DeckEntry>>("Trainer", entries.Where(e => e.Card.IsTrainer).ToList()),
                new KeyValuePair<string, List<DeckEntry>>("Energy", entries.Where(e => e.Card.IsEnergy).ToList())
            };
            float total = 0f;
            foreach (var sec in sections)
                if (sec.Value.Count > 0) total += headerH + ((sec.Value.Count + perRow - 1) / perRow) * rowH;

            var content = new Rect(0f, 0f, Mathf.Max(1f, view.width - Scrollbar), Mathf.Max(1f, total));
            bool mouseInView = view.Contains(Event.current.mousePosition);
            deckScroll = GUI.BeginScrollView(view, deckScroll, content);
            if (entries.Count == 0)
                GUI.Label(new Rect(0f, 0f, content.width, 60f), "Drag cards here from the card pool.", Hint(ui));
            float y = 0f;
            foreach (var sec in sections)
            {
                var list = sec.Value;
                if (list.Count == 0) continue;
                GUI.Label(new Rect(0f, y, content.width, headerH), sec.Key + " (" + list.Sum(e => e.Count) + ")", ui.Title);
                y += headerH;
                for (int k = 0; k < list.Count; k++)
                {
                    var card = list[k].Card;
                    var r = new Rect((k % perRow) * (DeckTileW + Gap), y + (k / perRow) * rowH, DeckTileW, DeckTileH);
                    if (r.yMax + TileButtonsH < deckScroll.y || r.y > deckScroll.y + view.height) continue;
                    Tile(r, card, card, list[k].Count, mouseInView, Zone.Deck, ui);
                    var minus = new Rect(r.x, r.yMax + 1f, DeckTileW / 2f - 1f, TileButtonsH - 2f);
                    var plus = new Rect(r.x + DeckTileW / 2f + 1f, r.yMax + 1f, DeckTileW / 2f - 1f, TileButtonsH - 2f);
                    if (GUI.Button(minus, "-")) actions.Add(() => Remove(card));
                    GUI.enabled = !modal && CanAdd(card);
                    if (GUI.Button(plus, "+")) actions.Add(() => Add(card, 1, true));
                    GUI.enabled = !modal;
                }
                y += ((list.Count + perRow - 1) / perRow) * rowH;
            }
            GUI.EndScrollView();
        }

        /// <summary>
        /// One card picture. card is what gets dragged or opened (a main printing in the pool, a printing in the
        /// deck); shown is the printing whose art is drawn. Left button: press to drag or click to open. Right button: nothing.
        /// </summary>
        private void Tile(Rect r, CardDef card, CardDef shown, int count, bool mouseInView, Zone zone, UiStyles ui)
        {
            var ev = Event.current;
            bool hover = !modal && mouseInView && r.Contains(ev.mousePosition);

            if (ev.type == EventType.Repaint)
            {
                bool beingDragged = dragging && pressCard == card && pressZone == zone;
                bool unavailable = zone == Zone.Pool && count == 0 && !CanAdd(shown);
                var prev = GUI.color;
                if (beingDragged || unavailable) GUI.color = new Color(1f, 1f, 1f, 0.4f);
                DrawCardFace(r, shown, ui);
                GUI.color = prev;
                if (count > 0)
                {
                    var badge = new Rect(r.xMax - 32f, r.y + 4f, 28f, 20f);
                    GUI.DrawTexture(badge, ui.BadgeBackground);
                    GUI.Label(badge, "×" + count, ui.Badge);
                }
                if (hover && !dragging)
                {
                    hoverCard = shown;
                    DrawFrame(r, ui.Highlight, 2f);
                }
            }
            else if (ev.type == EventType.MouseDown && ev.button == 0 && hover)
            {
                pressCard = card;
                pressZone = zone;
                pressPos = mouse;
                dragging = false;
                ev.Use();
            }
        }

        /// <summary>The card's image, or a placeholder while it loads or when there's no image.</summary>
        private void DrawCardFace(Rect r, CardDef shown, UiStyles ui)
        {
            var tex = CardVisuals.TryImage(shown, ref loadBudget);
            if (tex != null) GUI.DrawTexture(r, tex, ScaleMode.ScaleToFit);
            else DrawPlaceholder(r, shown, ui);
        }

        /// <summary>Stand-in for a card without an image (or whose image is still loading).</summary>
        private void DrawPlaceholder(Rect r, CardDef card, UiStyles ui)
        {
            var color = CardVisuals.TypeColor(card);
            GUI.DrawTexture(r, CardVisuals.Solid(color));
            var inner = new Rect(r.x + 4f, r.y + 4f, r.width - 8f, r.height - 8f);
            GUI.DrawTexture(inner, CardVisuals.Solid(Color.Lerp(color, Color.white, 0.55f)));
            GUI.Label(new Rect(inner.x + 2f, inner.y + 4f, inner.width - 4f, inner.height * 0.6f), card.Name, PlaceholderName(ui));
            GUI.Label(new Rect(inner.x + 2f, inner.yMax - 36f, inner.width - 4f, 34f), card.SetCode + " " + card.Number + "\n" + CardText.Short(card), ui.Small);
        }

        private static void DrawFrame(Rect r, Texture2D tex, float t)
        {
            GUI.DrawTexture(new Rect(r.x - t, r.y - t, r.width + 2 * t, t), tex);
            GUI.DrawTexture(new Rect(r.x - t, r.yMax, r.width + 2 * t, t), tex);
            GUI.DrawTexture(new Rect(r.x - t, r.y, t, r.height), tex);
            GUI.DrawTexture(new Rect(r.xMax, r.y, t, r.height), tex);
        }

        /// <summary>The card being dragged follows the mouse; the panel it would drop into is outlined.</summary>
        private void DrawDragGhost(UiStyles ui)
        {
            var shown = pressZone == Zone.Pool ? ArtFor(pressCard) : pressCard;
            bool overDeck = deckPanel.Contains(mouse);
            bool overPool = poolPanel.Contains(mouse);
            if (pressZone == Zone.Pool && overDeck) DrawFrame(deckPanel, CanAdd(shown) ? ui.Selected : ui.BadgeBackground, 3f);
            if (pressZone == Zone.Deck && overPool) DrawFrame(poolPanel, ui.Selected, 3f);
            var r = new Rect(mouse.x - PoolTileW / 2f, mouse.y - PoolTileH / 2f, PoolTileW, PoolTileH);
            var prev = GUI.color;
            GUI.color = new Color(1f, 1f, 1f, 0.85f);
            DrawCardFace(r, shown, ui);
            GUI.color = prev;
            string tip = null;
            if (pressZone == Zone.Pool && overDeck) tip = AddBlockReason(shown) ?? "Add 1";
            if (pressZone == Zone.Deck && overPool) tip = "Remove 1";
            if (tip != null)
            {
                var b = new Rect(r.x, r.yMax + 2f, r.width, 22f);
                GUI.DrawTexture(b, ui.BadgeBackground);
                GUI.Label(b, tip, ui.Badge);
            }
        }

        /// <summary>The hovered card, large, with its full text beside it.</summary>
        private void DrawHover(UiStyles ui, CardDef card)
        {
            var tex = ImageFor(card);
            string text = CardText.Describe(card);
            const float textW = 300f;
            float textH = ui.Tooltip.CalcHeight(new GUIContent(text), textW);
            float imgH = tex != null ? Mathf.Min(380f, UiScale.Height - 20f) : 0f;
            float imgW = tex != null ? imgH * tex.width / tex.height : 0f;
            float gap = tex != null ? 8f : 0f;
            float totalW = imgW + gap + textW;
            float x = mouse.x + 24f;
            if (x + totalW > UiScale.Width - 10f) x = mouse.x - 24f - totalW;
            x = Mathf.Max(10f, x);
            float y = Mathf.Clamp(mouse.y - 60f, 10f, Mathf.Max(10f, UiScale.Height - Mathf.Max(imgH, textH) - 10f));
            if (tex != null) GUI.DrawTexture(new Rect(x, y, imgW, imgH), tex, ScaleMode.ScaleToFit);
            GUI.Box(new Rect(x + imgW + gap, y, textW, textH), text, ui.Tooltip);
        }

        /// <summary>The printing's image (loading it now if needed), or null when it has none.</summary>
        private static Texture2D ImageFor(CardDef card)
        {
            if (!CardVisuals.HasImageFile(card)) return null;
            var face = CardVisuals.Face(card);
            return CardVisuals.HasImage(card) ? face : null;
        }

        // ------------------------------------------------------------------ Card view

        private void OpenDetail(CardDef card, Zone zone)
        {
            var main = db.MainPrinting(card);
            if (zone == Zone.Deck) detailList = order.Select(d => db.MainPrinting(d)).Distinct().ToList();
            else detailList = pool.Where(c => MatchesFilter(c, search, filter, category) && InSetFilter(c) && InImageFilter(c)).ToList();
            if (!detailList.Contains(main)) detailList.Insert(0, main);
            detailCard = main;
            artMenuOpen = false;
            artScroll = Vector2.zero;
            GUIUtility.keyboardControl = 0; // so the arrow keys and Esc reach the card view, not the search box
        }

        private void CloseDetail()
        {
            detailCard = null;
            artMenuOpen = false;
        }

        private void Navigate(int delta)
        {
            int i = detailList.IndexOf(detailCard);
            int j = Mathf.Clamp(i + delta, 0, detailList.Count - 1);
            if (i < 0 || j == i) return;
            detailCard = detailList[j];
            artMenuOpen = false;
            artScroll = Vector2.zero;
        }

        /// <summary>
        /// Large card in the middle with its text beside it, arrows for the previous/next card, the art picker,
        /// the In deck count and - / + buttons. Esc, the Close button or a click outside closes it.
        /// </summary>
        private void DrawDetail(UiStyles ui, List<System.Action> actions)
        {
            var ev = Event.current;
            var card = detailCard;
            var shown = ArtFor(card);

            var prevColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.75f);
            GUI.DrawTexture(new Rect(0f, 0f, UiScale.Width, UiScale.Height), CardVisuals.Solid(Color.white));
            GUI.color = prevColor;

            float cardH = Mathf.Min(UiScale.Height * 0.6f, 600f);
            float cardW = cardH * 63f / 88f;
            float textW = Mathf.Min(320f, UiScale.Width * 0.24f);
            const float arrowW = 56f;
            float totalW = arrowW + 12f + cardW + 16f + textW + 12f + arrowW;
            float x0 = (UiScale.Width - totalW) / 2f;
            float y0 = Mathf.Max(16f, (UiScale.Height - cardH - 170f) / 2f);
            var cardRect = new Rect(x0 + arrowW + 12f, y0, cardW, cardH);
            var textRect = new Rect(cardRect.xMax + 16f, y0, textW, cardH);
            var leftArrow = new Rect(x0, cardRect.center.y - 36f, arrowW, 72f);
            var rightArrow = new Rect(textRect.xMax + 12f, cardRect.center.y - 36f, arrowW, 72f);

            var tex = ImageFor(shown);
            if (tex != null) GUI.DrawTexture(cardRect, tex, ScaleMode.ScaleToFit);
            else DrawPlaceholder(cardRect, shown, ui);
            GUI.Box(textRect, CardText.Describe(shown), ui.Tooltip);

            int index = detailList.IndexOf(card);
            GUI.enabled = index > 0;
            if (GUI.Button(leftArrow, "‹", ArrowStyle(ui))) actions.Add(() => Navigate(-1));
            GUI.enabled = index >= 0 && index < detailList.Count - 1;
            if (GUI.Button(rightArrow, "›", ArrowStyle(ui))) actions.Add(() => Navigate(1));
            GUI.enabled = true;

            // Art picker, In deck count, - and +.
            float cy = cardRect.yMax + 12f;
            var artButton = new Rect(cardRect.center.x - 170f, cy, 340f, 34f);
            var printings = db.PrintingsOf(card);
            string artText = "Art:  " + PrintLabel(shown) + (printings.Count > 1 ? (artMenuOpen ? "   ▲" : "   ▼  (" + printings.Count + " versions)") : "");
            GUI.enabled = printings.Count > 1;
            if (GUI.Button(artButton, artText)) actions.Add(() => artMenuOpen = !artMenuOpen);
            GUI.enabled = true;

            int inDeck = CountCard(card);
            var inDeckRect = new Rect(cardRect.center.x - 100f, cy + 46f, 200f, 44f);
            GUI.Label(inDeckRect, "IN DECK   " + inDeck + (card.IsBasicEnergy || card.AnyNumberInDeck ? "" : " / " + (custom != null ? custom.MaxCopies : 4)), BigLabel(ui));
            var minus = new Rect(inDeckRect.x - 140f, cy + 46f, 124f, 44f);
            var plus = new Rect(inDeckRect.xMax + 16f, cy + 46f, 124f, 44f);
            GUI.enabled = !artMenuOpen && inDeck > 0;
            if (GUI.Button(minus, "−", BigButton(ui))) actions.Add(() => RemoveCard(card));
            GUI.enabled = !artMenuOpen && CanAdd(shown);
            if (GUI.Button(plus, "+", BigButton(ui))) actions.Add(() => AddCard(card));
            GUI.enabled = true;
            string blocked = AddBlockReason(shown);
            var cardCategory = Formats.CategoryOf(card);
            string categoryNote = cardCategory == CardCategory.Standard ? "Standard card (mark " + card.RegulationMark + ")"
                : cardCategory == CardCategory.Unlimited ? "Unlimited card (first printed before Black & White)"
                : "Expanded card" + (string.IsNullOrEmpty(card.RegulationMark) ? "" : " (mark " + card.RegulationMark + ")");
            if (card.IsEnergy && card.IsBasicEnergy) categoryNote = "Basic Energy";
            string fix = blocked == "Expanded only" ? ": set this deck's format to Expanded to add it."
                : blocked == "Unlimited only" ? ": set this deck's format to Unlimited to add it." : "";
            GUI.Label(new Rect(minus.x, minus.yMax + 6f, plus.xMax - minus.x, 22f), categoryNote + fix, BigHint(ui));

            var close = new Rect(UiScale.Width - 140f, 16f, 120f, 34f);
            if (GUI.Button(close, "Close")) actions.Add(CloseDetail);

            // The art list opens upwards over the card picture, so it never covers the buttons.
            Rect menu = new Rect();
            if (artMenuOpen && printings.Count > 1) menu = DrawArtMenu(card, shown, printings, artButton, cardRect, ui, actions);

            if (ev.type == EventType.KeyDown)
            {
                if (ev.keyCode == KeyCode.Escape)
                {
                    if (artMenuOpen) actions.Add(() => artMenuOpen = false);
                    else actions.Add(CloseDetail);
                    ev.Use();
                }
                else if (ev.keyCode == KeyCode.LeftArrow)
                {
                    actions.Add(() => Navigate(-1));
                    ev.Use();
                }
                else if (ev.keyCode == KeyCode.RightArrow)
                {
                    actions.Add(() => Navigate(1));
                    ev.Use();
                }
            }

            // A click on the dark background closes the card view (or just the art list).
            if (ev.type == EventType.MouseDown)
            {
                var content = Rect.MinMaxRect(leftArrow.x, y0, rightArrow.xMax, minus.yMax + 32f);
                if (artMenuOpen && !menu.Contains(ev.mousePosition) && !artButton.Contains(ev.mousePosition))
                {
                    actions.Add(() => artMenuOpen = false);
                    ev.Use();
                }
                else if (!content.Contains(ev.mousePosition) && !close.Contains(ev.mousePosition))
                {
                    actions.Add(CloseDetail);
                    ev.Use();
                }
            }
        }

        /// <summary>Every printing of the card, with a thumbnail when there's an image for it. Returns the list's rectangle.</summary>
        private Rect DrawArtMenu(CardDef card, CardDef shown, List<CardDef> printings, Rect artButton, Rect cardRect, UiStyles ui, List<System.Action> actions)
        {
            const float itemH = 46f;
            float h = Mathf.Min(printings.Count * itemH + 8f, cardRect.height - 10f);
            var menu = new Rect(artButton.x, artButton.y - h - 4f, artButton.width, h);
            GUI.DrawTexture(menu, CardVisuals.Solid(new Color(0.12f, 0.12f, 0.15f, 1f)));
            DrawFrame(menu, CardVisuals.Solid(new Color(0.5f, 0.5f, 0.55f, 1f)), 1f);
            var view = new Rect(menu.x + 4f, menu.y + 4f, menu.width - 8f, menu.height - 8f);
            var content = new Rect(0f, 0f, view.width - Scrollbar, printings.Count * itemH);
            artScroll = GUI.BeginScrollView(view, artScroll, content);
            for (int i = 0; i < printings.Count; i++)
            {
                var p = printings[i];
                var row = new Rect(0f, i * itemH, content.width, itemH - 4f);
                if (row.yMax < artScroll.y || row.y > artScroll.y + view.height) continue;
                bool own = CardVisuals.HasOwnImageFile(p);
                bool current = p.Key == shown.Key;
                if (GUI.Button(row, GUIContent.none)) actions.Add(() => { SetArt(card, p); artMenuOpen = false; });
                if (Event.current.type == EventType.Repaint)
                {
                    var thumb = new Rect(row.x + 4f, row.y + 3f, (row.height - 6f) * 63f / 88f, row.height - 6f);
                    var tex = own ? CardVisuals.TryImage(p, ref loadBudget) : null;
                    if (tex != null) GUI.DrawTexture(thumb, tex, ScaleMode.ScaleToFit);
                    else GUI.DrawTexture(thumb, CardVisuals.Solid(CardVisuals.TypeColor(p)));
                    string label = (current ? "<b>" : "") + PrintLabel(p) + (current ? "</b>   <color=#7CE08A>(in use)</color>" : "") + (own ? "" : "   <color=#999999>(no image)</color>");
                    GUI.Label(new Rect(thumb.xMax + 8f, row.y, row.width - thumb.width - 16f, row.height), label, ArtItem(ui));
                }
            }
            GUI.EndScrollView();
            return menu;
        }

        // ------------------------------------------------------------------ Styles

        private GUIStyle RichLabel(UiStyles ui)
        {
            if (richLabel == null) richLabel = new GUIStyle(ui.Label) { richText = true, wordWrap = false };
            return richLabel;
        }

        private GUIStyle PlaceholderName(UiStyles ui)
        {
            if (placeholderName == null)
            {
                placeholderName = new GUIStyle(ui.Small) { fontSize = 12, fontStyle = FontStyle.Bold, alignment = TextAnchor.UpperCenter, wordWrap = true };
                placeholderName.normal.textColor = Color.black;
            }
            return placeholderName;
        }

        private GUIStyle Hint(UiStyles ui)
        {
            if (hint == null)
            {
                hint = new GUIStyle(ui.Label) { fontSize = 12, wordWrap = true };
                hint.normal.textColor = new Color(0.8f, 0.8f, 0.85f);
            }
            return hint;
        }

        private GUIStyle ArrowStyle(UiStyles ui)
        {
            if (arrowStyle == null) arrowStyle = new GUIStyle(GUI.skin.button) { fontSize = 40, alignment = TextAnchor.MiddleCenter };
            return arrowStyle;
        }

        private GUIStyle BigButton(UiStyles ui)
        {
            if (bigButton == null) bigButton = new GUIStyle(GUI.skin.button) { fontSize = 28, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
            return bigButton;
        }

        private GUIStyle BigLabel(UiStyles ui)
        {
            if (bigLabel == null)
            {
                bigLabel = new GUIStyle(ui.Title) { fontSize = 20, alignment = TextAnchor.MiddleCenter, wordWrap = false };
                bigLabel.normal.textColor = Color.white;
            }
            return bigLabel;
        }

        private GUIStyle BigHint(UiStyles ui)
        {
            if (bigHint == null)
            {
                bigHint = new GUIStyle(ui.Label) { fontSize = 13, alignment = TextAnchor.MiddleCenter, wordWrap = false };
                bigHint.normal.textColor = new Color(0.85f, 0.85f, 0.9f);
            }
            return bigHint;
        }

        private GUIStyle ArtItem(UiStyles ui)
        {
            if (artItem == null)
            {
                artItem = new GUIStyle(ui.Label) { richText = true, wordWrap = false, alignment = TextAnchor.MiddleLeft, fontSize = 13 };
                artItem.normal.textColor = Color.white;
            }
            return artItem;
        }

        // ------------------------------------------------------------------ Import dialog

        // The dialog has a fixed size that fits the screen. The list scrolls inside its box, so a long or wrapped
        // list never pushes the buttons out of the dialog (v57.2).
        private const string ImportControlName = "DeckImportText";
        private const float ImportTitleH = 30f, ImportButtonsH = 34f;
        private Vector2 importScroll;
        private float importContentHeight;
        private bool importFollowCaret;
        private GUIStyle importStyle;

        private static Rect ImportRect()
        {
            float w = Mathf.Min(640f, UiScale.Width - 40f);
            float h = Mathf.Min(580f, UiScale.Height - 40f);
            return new Rect(UiScale.Width / 2f - w / 2f, UiScale.Height / 2f - h / 2f, w, h);
        }

        /// <summary>The box the list is typed or pasted into.</summary>
        private static Rect ImportViewRect()
        {
            var r = ImportRect();
            float top = r.y + 12f + ImportTitleH + 6f;
            float bottom = r.yMax - 12f - ImportButtonsH - 10f;
            return new Rect(r.x + 12f, top, r.width - 24f, Mathf.Max(60f, bottom - top));
        }

        private GUIStyle ImportStyle()
        {
            if (importStyle == null) importStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
            return importStyle;
        }

        private float ClampImportScroll(float y, float viewHeight)
        {
            return Mathf.Clamp(y, 0f, Mathf.Max(0f, importContentHeight - viewHeight));
        }

        /// <summary>The mouse wheel over the list scrolls the list, not the panels behind the dialog.</summary>
        private void HandleImportWheel(Event e)
        {
            if (e == null || e.type != EventType.ScrollWheel) return;
            var view = ImportViewRect();
            if (!view.Contains(e.mousePosition)) return;
            importScroll.y = ClampImportScroll(importScroll.y + e.delta.y * 20f, view.height);
            e.Use();
        }

        private void DrawImport(UiStyles ui, List<System.Action> actions)
        {
            var r = ImportRect();
            GUI.Box(r, GUIContent.none);
            GUI.Box(r, GUIContent.none);
            GUI.Label(new Rect(r.x + 12f, r.y + 12f, r.width - 24f, ImportTitleH), "Paste a deck list (TCG Live or Limitless export format)", ui.Title);

            var view = ImportViewRect();
            var style = ImportStyle();
            string text = importText ?? "";
            float line = style.lineHeight > 0f ? style.lineHeight : 18f;
            float innerW = Mathf.Max(40f, view.width - Scrollbar - 2f);
            // A spare line under the text, so a new line fits before the box catches up.
            float textH = Mathf.Max(view.height - 2f, style.CalcHeight(new GUIContent(text), innerW) + line + 4f);
            importContentHeight = textH;
            var textRect = new Rect(0f, 0f, innerW, textH);

            var ev = Event.current;
            bool focused = GUI.GetNameOfFocusedControl() == ImportControlName;
            if (focused && (ev.type == EventType.KeyDown || ev.type == EventType.ExecuteCommand)) importFollowCaret = true;

            importScroll.y = ClampImportScroll(importScroll.y, view.height);
            importScroll = GUI.BeginScrollView(view, importScroll, textRect, false, true);
            GUI.SetNextControlName(ImportControlName);
            importText = GUI.TextArea(textRect, text, style);
            GUI.EndScrollView();

            // Keep the caret in sight while typing or pasting into the box.
            if (importFollowCaret && ev.type == EventType.Repaint)
            {
                importFollowCaret = false;
                if (focused) FollowImportCaret(style, textRect, view.height, line);
            }

            float bw = (r.width - 24f - 2f * Gap) / 3f;
            float by = r.yMax - 12f - ImportButtonsH;
            if (GUI.Button(new Rect(r.x + 12f, by, bw, ImportButtonsH), "Paste from clipboard"))
                actions.Add(() => { importText = GUIUtility.systemCopyBuffer ?? ""; importScroll = Vector2.zero; GUI.FocusControl(null); });
            if (GUI.Button(new Rect(r.x + 12f + bw + Gap, by, bw, ImportButtonsH), "Load into editor")) actions.Add(ImportNow);
            if (GUI.Button(new Rect(r.x + 12f + 2f * (bw + Gap), by, bw, ImportButtonsH), "Cancel")) actions.Add(() => importOpen = false);
        }

        private void FollowImportCaret(GUIStyle style, Rect textRect, float viewHeight, float line)
        {
            var editor = GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl) as TextEditor;
            string text = importText ?? "";
            if (editor == null || editor.text != text) return;
            int index = Mathf.Clamp(editor.cursorIndex, 0, text.Length);
            float y = style.GetCursorPixelPosition(textRect, new GUIContent(text), index).y;
            if (y - 2f < importScroll.y) importScroll.y = y - 2f;
            else if (y + line + 4f > importScroll.y + viewHeight) importScroll.y = y + line + 4f - viewHeight;
            importScroll.y = ClampImportScroll(importScroll.y, viewHeight);
        }

        // ------------------------------------------------------------------ Actions

        private void ImportNow()
        {
            var problems = new List<string>();
            var entries = Decklist.Parse(importText, db, problems);
            if (entries.Count == 0)
            {
                SetStatus("Nothing recognised in that text. Lines should look like: 4 Dreepy TWM 128", true);
                return;
            }
            SetCards(entries);
            editing = null;
            deckName = Decklist.ReadName(importText) ?? "Imported Deck";
            var named = GameFormat.Named(Decklist.ReadFormatName(importText));
            if (named != null && named.IsCustom) SetCustomFormat(named.Custom);
            else SetFormat(Wider(Decklist.ReadFormat(importText), Formats.Needed(entries.Select(e => e.Card))));
            importOpen = false;
            if (problems.Count > 0)
                SetStatus("Imported, but these lines aren't in the card pool yet (not implemented):\n" + string.Join("\n", problems.ToArray()), true);
            else
                SetStatus("Imported " + Total + " cards.", false);
        }

        private void Save(bool asCopy)
        {
            string name = string.IsNullOrEmpty(deckName) ? "My Deck" : deckName.Trim();
            if (asCopy || (editing != null && editing.BuiltIn && editing.Name == name)) name = UniqueName(name);
            if (editing != null && !editing.BuiltIn && !asCopy && editing.Name != name)
                DeckLibrary.Delete(editing); // renamed: replace the old file
            var text = Decklist.Format(name, Entries(), deckFmt.Name);
            editing = DeckLibrary.Save(name, text);
            deckName = name;
            Reload();
            var problems = deckFmt.Validate(Expanded());
            SetStatus("Saved \"" + name + "\" to " + DeckLibrary.UserFolder + (problems.Count > 0 ? " (not legal yet, so it can't be used in a game)" : "."), false);
        }

        /// <summary>The wider of two formats (Standard &lt; Expanded &lt; Unlimited).</summary>
        private static DeckFormat Wider(DeckFormat a, DeckFormat b)
        {
            return (int)a >= (int)b ? a : b;
        }

        private string UniqueName(string name)
        {
            var taken = new HashSet<string>(decks.Select(d => d.Name));
            if (!taken.Contains(name)) return name;
            for (int i = 2; ; i++)
            {
                var candidate = name + " (" + i + ")";
                if (!taken.Contains(candidate)) return candidate;
            }
        }

        private void DeleteCurrent()
        {
            if (editing == null || editing.BuiltIn) return;
            DeckLibrary.Delete(editing);
            SetStatus("Deleted \"" + editing.Name + "\".", false);
            Reload();
            NewDeck();
        }

        /// <summary>
        /// The deck list's Delete (v123): deletes a saved deck's file, or hides a built-in deck (Restore built-in decks shows
        /// them again). The deck being edited stays in the editor, so it can still be saved again.
        /// </summary>
        private void DeleteFromList(DeckRecord deck)
        {
            confirmDelete = null;
            if (deck == null) return;
            DeckLibrary.Remove(deck);
            bool wasEditing = editing != null && editing.Name == deck.Name && editing.BuiltIn == deck.BuiltIn && editing.FilePath == deck.FilePath;
            if (wasEditing) editing = null;
            Reload();
            SetStatus(deck.BuiltIn ? "Removed the built-in deck \"" + deck.Name + "\" from the list (Restore built-in decks brings it back)."
                : "Deleted \"" + deck.Name + "\"." + (wasEditing ? " It's still in the editor: Save keeps it after all." : ""), false);
        }

        private void RestoreBuiltIns()
        {
            int n = DeckLibrary.RestoreBuiltIns();
            Reload();
            SetStatus(n == 0 ? "No built-in decks were hidden." : "Restored " + n + " built-in deck" + (n == 1 ? "" : "s") + ".", false);
        }

        /// <summary>One line about a custom format: deck size, Prize cards, the rules that differ.</summary>
        public static string FormatSummary(CustomFormat f)
        {
            var parts = new List<string> { f.DeckSize + " cards", f.PrizeCount + " Prize card" + (f.PrizeCount == 1 ? "" : "s"), f.AllSets ? "every set" : f.Sets.Count + " set" + (f.Sets.Count == 1 ? "" : "s") };
            if (f.Banned.Count > 0) parts.Add(f.Banned.Count + " banned");
            if (f.Singleton) parts.Add("singleton");
            if (!f.AllowRuleBox) parts.Add("no Rule Box Pokémon");
            if (!f.AllowAceSpec) parts.Add("no ACE SPEC");
            var r = f.Rules ?? GameRules.Default;
            if (r.FirstTurnAttack) parts.Add("attack on turn 1");
            if (r.FirstTurnSupporter) parts.Add("Supporter on turn 1");
            if (r.DrawBeforeCoinFlip) parts.Add("hands before the coin flip");
            if (r.ToolsAreItems) parts.Add("Tools are Items");
            if (r.OldTrainersArePlain) parts.Add("old Trainers are plain Trainers");
            return string.Join(" · ", parts.ToArray());
        }

        private void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = Decklist.Format(deckName, Entries(), deckFmt.Name);
            SetStatus("Deck list copied to the clipboard.", false);
        }
    }
}
