using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// The game screen's pop-ups and board input:
    ///  - dragging a card from your hand onto the board or onto a Pokémon to play it;
    ///  - clicking a discard pile to see every card in it (yours or your opponent's);
    ///  - clicking a Pokémon for a large view with its Abilities and attacks (the ones you can use now are highlighted and clickable);
    ///  - a card picker with Valid / All tabs whenever an effect asks you to choose cards;
    ///  - a mulligan screen for each player who mulliganed;
    ///  - the opponent's plays shown large in the middle of the screen.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const float DragThreshold = 8f;
        private const float Scrollbar = 18f;

        // Drag and click on the board
        private bool pressing;
        private int pressId;
        private Vector2 pressPos;
        private bool dragging;
        /// <summary>A Trainer was dropped on this Pokémon: pick it automatically if the card then asks for a Pokémon.</summary>
        private int autoPickUid = -1;
        private DecisionRequest lastSubmitted;

        // Pop-ups
        private int mulliganSeat = -1;
        private List<List<CardRef>> mulliganHands = new List<List<CardRef>>();
        private Vector2 mulliganScroll;
        private int pileSeat = -1;
        /// <summary>The open pile is that player's Lost Zone, not their discard pile (v49).</summary>
        private bool pileIsLostZone;
        private Vector2 pileScroll;
        private int popupUid = -1;
        /// <summary>Pop-up: which of the Pokémon's cards is in front (0 = the Pokémon, then its Tools and Energy), and for which Pokémon.</summary>
        private int popupFront;
        private int popupFrontUid = -1;
        private Vector2 popupScroll;
        private bool pickerAllTab, pickerHidden;
        private Vector2 pickerScroll;

        // The opponent's plays, shown large
        private readonly List<CardDef> revealCards = new List<CardDef>();
        private string revealCaption;
        private float revealUntil;

        // Short messages ("That card can't go there")
        private string toast;
        private float toastUntil;

        private GUIStyle tabStyle, doneStyle, darkCentered, whiteSmall;

        private bool ModalOpen { get { return mulliganSeat >= 0 || PickerVisible || CoinCallVisible || pileSeat >= 0 || popupUid >= 0; } }

        private bool PickerVisible { get { return shownRequest != null && human != null && !pickerHidden && IsCardChoice(shownRequest); } }

        /// <summary>
        /// An effect asking you to choose cards (shown in the card picker rather than as a list), or a search that found
        /// nothing you can take (no options, but the whole deck to look through).
        /// </summary>
        private static bool IsCardChoice(DecisionRequest req)
        {
            if (req == null || req.Kind != DecisionKind.ChooseCards) return false;
            if (req.Options.Count == 0) return req.SeenCards != null && req.SeenCards.Count > 0;
            return req.Options.All(o => o.Card != null && o.CardId >= 0);
        }

        private void ResetScreens()
        {
            pressing = false;
            dragging = false;
            autoPickUid = -1;
            lastSubmitted = null;
            mulliganSeat = -1;
            pileSeat = -1;
            popupUid = -1;
            pickerAllTab = false;
            pickerHidden = false;
            revealCards.Clear();
            revealCaption = null;
            revealUntil = 0f;
            toast = null;
            coinUntil = -1f;
        }

        /// <summary>A new decision is shown. Returns true if it was answered automatically.</summary>
        private bool OnNewRequest(DecisionRequest req)
        {
            pickerAllTab = false;
            pickerHidden = false;
            pickerScroll = Vector2.zero;
            if (req.Kind != DecisionKind.MainAction)
            {
                popupUid = -1;
                pileSeat = -1;
                panelHidden = false; // a new kind of choice always shows the decision panel (v55.2)
            }

            int uid = autoPickUid;
            autoPickUid = -1;
            if (uid >= 0 && req.Kind == DecisionKind.ChoosePokemon)
            {
                int idx = req.Options.FindIndex(o => o.PokemonUid == uid);
                if (idx >= 0)
                {
                    Submit(new List<int> { idx });
                    return true;
                }
            }
            return false;
        }

        private void ShowToast(string text)
        {
            toast = text;
            toastUntil = Time.time + 2.5f;
        }

        // ================================================================== Board input: drag to play, click to inspect

        private void HandleBoardInput(Event e, UiStyles ui)
        {
            if (board == null || cam == null) return;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape && pressing)
            {
                pressing = false;
                dragging = false;
                e.Use();
                return;
            }
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                pressing = false; // always start clean (a release may have happened outside the window)
                dragging = false;
                if (OverPanel(e.mousePosition)) return;
                var cv = board.Pick(cam, e.mousePosition);
                if (cv == null) return;
                pressing = true;
                pressId = cv.Id;
                pressPos = e.mousePosition;
                dragging = false;
                e.Use();
            }
            else if (e.type == EventType.MouseDrag && pressing)
            {
                if (!dragging && (e.mousePosition - pressPos).sqrMagnitude > DragThreshold * DragThreshold && CanDragToPlay(pressId)) dragging = true;
                if (dragging) e.Use();
            }
            else if (e.rawType == EventType.MouseUp && e.button == 0 && pressing)
            {
                pressing = false;
                int id = pressId;
                bool wasDrag = dragging;
                dragging = false;
                var pos = e.mousePosition;
                if (wasDrag) guiActions.Add(() => DropCard(id, pos));
                else
                {
                    var cv = board.Pick(cam, pos);
                    if (cv != null && cv.Id == id) guiActions.Add(() => ClickCard(cv));
                }
                e.Use();
            }
        }

        /// <summary>Decisions answered by dragging a card out of your hand: main actions and placing Pokémon at setup.</summary>
        private static bool IsDragDecision(DecisionRequest req)
        {
            return req != null && (req.Kind == DecisionKind.MainAction || req.Kind == DecisionKind.SetupActive || req.Kind == DecisionKind.SetupBench);
        }

        /// <summary>A card in your hand that can be played now.</summary>
        private bool CanDragToPlay(int id)
        {
            var req = shownRequest;
            if (req == null || human == null || !IsDragDecision(req) || !board.IsInViewerHand(id)) return false;
            return req.Options.Any(o => o.UsesCard(id) && o.Action != ActionType.UseAbility);
        }

        private void ClickCard(CardView cv)
        {
            if (cv == null) return;
            var req = shownRequest;
            int seat = board.DiscardSeatOf(cv.Id);
            // The discard pile opens unless that top card is one of the choices right now.
            if (seat >= 0 && (req == null || req.Kind == DecisionKind.MainAction || !req.Options.Any(o => o.CardId == cv.Id)))
            {
                OpenPile(seat, false);
                return;
            }
            // So does the Lost Zone (cards never leave it, so its top card is never a choice).
            int lostSeat = board.LostZoneSeatOf(cv.Id);
            if (lostSeat >= 0)
            {
                OpenPile(lostSeat, true);
                return;
            }
            var pv = PokemonOfCard(cv.Id);
            if (pv != null && (req == null || req.Kind == DecisionKind.MainAction))
            {
                popupUid = pv.Uid;
                popupScroll = Vector2.zero;
                return;
            }
            OnCardClicked(cv);
        }

        /// <summary>A hand card released on the board: on a Pokémon (Energy, Evolution, Tool...) or anywhere above the hand (Basic, Item, Supporter, Stadium).</summary>
        private void DropCard(int id, Vector2 pos)
        {
            var req = shownRequest;
            if (req == null || human == null || !IsDragDecision(req)) return;
            var opts = new List<int>();
            for (int i = 0; i < req.Options.Count; i++)
                if (req.Options[i].UsesCard(id) && req.Options[i].Action != ActionType.UseAbility) opts.Add(i);
            if (opts.Count == 0 || OverPanel(pos)) return;
            if (pos.y >= board.HandTopY(cam) - 4f) return; // dropped back on the hand

            var hit = board.Pick(cam, pos);
            var target = hit != null ? PokemonOfCard(hit.Id) : null;
            var matches = new List<int>();
            if (target != null)
                foreach (var i in opts)
                    if (req.Options[i].PokemonUid == target.Uid) matches.Add(i);
            if (matches.Count == 0)
            {
                foreach (var i in opts)
                    if (req.Options[i].PokemonUid < 0) matches.Add(i);
                // A Basic Pokémon dropped on the board goes to the Bench (its Ability from the hand, if any, stays in the list).
                int bench = matches.FindIndex(i => req.Options[i].Action == ActionType.BenchBasic);
                if (bench >= 0 && matches.Count > 1) matches = new List<int> { matches[bench] };
                // A Tool, Switch, Rare Candy, Boss's Orders... dropped on a Pokémon: play it, then choose that Pokémon when asked.
                if (target != null && matches.Count == 1 && req.Options[matches[0]].Action == ActionType.PlayTrainer) autoPickUid = target.Uid;
            }

            if (matches.Count == 1)
            {
                req.Options[matches[0]].UseCopy(id); // play the copy you dragged (identical cards share one option)
                Submit(matches);
            }
            else if (matches.Count > 1)
            {
                filterCardId = id;
                optionScroll = Vector2.zero;
            }
            else if (target != null) ShowToast("That card can't go on " + target.Top.Def.Name + ". Drop it on a highlighted Pokémon.");
            else ShowToast("Drop it on a highlighted Pokémon.");
        }

        private void DrawDragGhost(UiStyles ui)
        {
            if (!dragging || current == null || shownRequest == null) return;
            CardDef def = null;
            foreach (var c in current.Me.Hand)
                if (c.Id == pressId) def = c.Def;
            if (def == null) return;

            float handTop = board.HandTopY(cam);
            bool loose = false;
            foreach (var o in shownRequest.Options)
            {
                if (!o.UsesCard(pressId) || o.Action == ActionType.UseAbility) continue;
                if (o.PokemonUid < 0)
                {
                    loose = true;
                    continue;
                }
                var p = current.FindPokemon(o.PokemonUid);
                var cv = p != null ? board.Get(p.Top.Id) : null;
                if (cv != null) CardDraw.Frame(cv.ScreenRect(cam), ui.Selected, 4f);
            }

            var size = new Vector2(120f, 168f);
            var r = new Rect(mouseGui.x - size.x / 2f, mouseGui.y - size.y * 0.7f, size.x, size.y);
            CardDraw.Card(r, def, 0.92f);
            string tip = mouseGui.y >= handTop - 4f ? "Release here to keep it in your hand"
                : shownRequest.Kind == DecisionKind.SetupActive ? "Release to make " + def.Name + " your Active Pokémon"
                : shownRequest.Kind == DecisionKind.SetupBench || (loose && def.IsBasicPokemon) ? "Release to put " + def.Name + " on your Bench"
                : loose ? "Release to play " + def.Name
                : "Drop it on a highlighted Pokémon";
            var tipRect = new Rect(r.x - 60f, r.yMax + 4f, r.width + 120f, 24f);
            GUI.DrawTexture(tipRect, ui.BadgeBackground);
            GUI.Label(tipRect, tip, ui.Badge);
        }

        private PokemonView PokemonOfCard(int id)
        {
            if (current == null) return null;
            foreach (var s in current.Sides)
            {
                if (s.Active != null && Holds(s.Active, id)) return s.Active;
                foreach (var b in s.Bench)
                    if (Holds(b, id)) return b;
            }
            return null;
        }

        private static bool Holds(PokemonView p, int id)
        {
            if (p.Top.Id == id) return true;
            foreach (var c in p.Stack) if (c.Id == id) return true;
            foreach (var c in p.Energy) if (c.Id == id) return true;
            foreach (var c in p.Tools) if (c.Id == id) return true;
            foreach (var c in p.Attached) if (c.Id == id) return true;
            return false;
        }

        private void DrawToast()
        {
            if (string.IsNullOrEmpty(toast) || Time.time >= toastUntil) return;
            var r = new Rect(UiScale.Width / 2f - 300f, 14f, 600f, 34f);
            CardDraw.Fill(r, new Color(0.55f, 0.1f, 0.1f, 0.92f));
            GUI.Label(r, toast, WhiteSmall());
        }

        // ================================================================== The opponent's plays, shown large

        /// <summary>How long to show this event's card in the middle of the screen (0 = don't).</summary>
        private float RevealTimeFor(GameEvent ev)
        {
            if (ev.Player < 0) return 0f;
            bool theirs = viewerSeat < 0 || ev.Player != viewerSeat;
            bool hasCards = ev.Cards != null && ev.Cards.Count > 0;
            switch (ev.Type)
            {
                case GameEventType.KnockOut:
                    return ev.Card != null ? revealSeconds * 0.7f : 0f;
                case GameEventType.PlayTrainer:
                case GameEventType.Attack:
                case GameEventType.UseAbility:
                case GameEventType.Evolve:
                case GameEventType.StadiumPlayed:
                    return theirs && ev.Card != null ? revealSeconds : 0f;
                case GameEventType.PutOnBench:
                case GameEventType.AttachEnergy:
                case GameEventType.AttachTool:
                case GameEventType.Retreat:
                    return theirs && ev.Card != null ? revealSeconds * 0.6f : 0f;
                case GameEventType.Reveal:
                    return theirs && hasCards ? revealSeconds * 0.8f : 0f;
                case GameEventType.Discard:
                    // Cards discarded from the hand or deck. Energy or Tools coming off a Pokémon show on the board.
                    return theirs && ev.PokemonUid < 0 && (hasCards || ev.Card != null) ? revealSeconds * 0.6f : 0f;
                default:
                    return 0f;
            }
        }

        private void StartReveal(GameEvent ev, float seconds)
        {
            revealCards.Clear();
            if (ev.Cards != null && ev.Cards.Count > 0)
                foreach (var c in ev.Cards) revealCards.Add(c.Def);
            else if (ev.Card != null) revealCards.Add(ev.Card);
            revealCaption = Colorize(ev.Text ?? "");
            revealUntil = Time.time + seconds;
        }

        private void DrawReveal(UiStyles ui)
        {
            DrawToast();
            if (revealCards.Count == 0 || Time.time >= revealUntil) return;
            float bandH = Mathf.Min(UiScale.Height * 0.62f, 540f);
            var band = new Rect(0f, (UiScale.Height - bandH) / 2f, UiScale.Width, bandH);
            CardDraw.Fill(band, new Color(0f, 0f, 0f, 0.62f));
            GUI.Label(new Rect(band.x + 30f, band.y + 8f, band.width - 60f, 62f), revealCaption ?? "", CardDraw.BigCaption());

            int n = Mathf.Min(revealCards.Count, 8);
            float cardH = band.height - 120f;
            float cardW = cardH * 63f / 88f;
            float gap = 16f;
            float total = n * cardW + (n - 1) * gap;
            if (total > UiScale.Width - 60f)
            {
                float k = (UiScale.Width - 60f) / total;
                cardW *= k;
                cardH *= k;
                gap *= k;
                total = UiScale.Width - 60f;
            }
            float x = (UiScale.Width - total) / 2f;
            float y = band.y + 76f;
            for (int i = 0; i < n; i++) CardDraw.Card(new Rect(x + i * (cardW + gap), y, cardW, cardH), revealCards[i]);
            string more = revealCards.Count > n ? "+" + (revealCards.Count - n) + " more  ·  " : "";
            GUI.Label(new Rect(band.x, band.yMax - 32f, band.width, 26f), more + "Click to continue", WhiteSmall());

            var e = Event.current;
            if (e.type == EventType.MouseDown && !ModalOpen)
            {
                guiActions.Add(() =>
                {
                    revealUntil = 0f;
                    revealCards.Clear();
                    nextEventTime = Time.time;
                    pressing = false;
                    dragging = false;
                });
                e.Use();
            }
        }

        // ================================================================== Mulligans

        private void OpenMulliganScreen(int seat, PlayerView view)
        {
            mulliganSeat = seat;
            mulliganHands = view != null && view.Sides[seat] != null ? view.Sides[seat].MulliganHands : new List<List<CardRef>>();
            mulliganScroll = Vector2.zero;
            pressing = false;
            dragging = false;
        }

        /// <summary>Like TCG Live: who mulliganed, how many times, and each hand they had to show.</summary>
        private void DrawMulliganScreen(UiStyles ui)
        {
            CardDraw.Dim(0.72f);
            float w = Mathf.Min(820f, UiScale.Width - 40f);
            float h = UiScale.Height - 60f;
            var r = new Rect((UiScale.Width - w) / 2f, 30f, w, h);
            CardDraw.Fill(r, new Color(0.92f, 0.93f, 0.95f, 1f));
            var header = new Rect(r.x, r.y, r.width, 78f);
            CardDraw.Fill(header, new Color(0.1f, 0.36f, 0.7f, 1f));
            string who = SeatName(mulliganSeat);
            GUI.Label(new Rect(header.x, header.y + 6f, header.width, 28f), who, CardDraw.Banner());
            GUI.Label(new Rect(header.x, header.y + 34f, header.width, 40f), "<size=28><i>MULLIGANS</i></size>", CardDraw.Banner());

            int n = mulliganHands.Count;
            var msgRect = new Rect(r.x + 20f, header.yMax + 14f, r.width - 40f, 58f);
            CardDraw.Fill(msgRect, Color.white);
            GUI.Label(msgRect, who + " had to take " + n + (n == 1 ? " mulligan" : " mulligans") + " before drawing a Basic Pokémon.", DarkCentered());

            var list = new Rect(r.x + 20f, msgRect.yMax + 14f, r.width - 40f, r.yMax - msgRect.yMax - 100f);
            const int perRow = 4;
            const float gap = 12f;
            float cardW = Mathf.Min(150f, (list.width - Scrollbar - 20f - gap * (perRow - 1)) / perRow);
            float cardH = cardW * 88f / 63f;
            float rowsH = 0f;
            foreach (var hand in mulliganHands) rowsH += 52f + ((hand.Count + perRow - 1) / perRow) * (cardH + gap) + 8f + 16f;
            var content = new Rect(0f, 0f, list.width - Scrollbar, Mathf.Max(rowsH, 10f));
            mulliganScroll = GUI.BeginScrollView(list, mulliganScroll, content);
            float y = 0f;
            for (int k = 0; k < mulliganHands.Count; k++)
            {
                var hand = mulliganHands[k];
                var box = new Rect(0f, y, content.width, 52f + ((hand.Count + perRow - 1) / perRow) * (cardH + gap) + 8f);
                CardDraw.Fill(box, Color.white);
                GUI.Label(new Rect(0f, y + 8f, content.width, 36f), "<size=22><b>MULLIGAN " + (k + 1) + "</b></size>", DarkCentered());
                float rowW = Mathf.Min(perRow, hand.Count) * (cardW + gap) - gap;
                float x0 = (content.width - rowW) / 2f;
                for (int i = 0; i < hand.Count; i++)
                {
                    var cr = new Rect(x0 + (i % perRow) * (cardW + gap), y + 52f + (i / perRow) * (cardH + gap), cardW, cardH);
                    CardDraw.Card(cr, hand[i].Def);
                }
                y += box.height + 16f;
            }
            GUI.EndScrollView();

            var done = new Rect(r.x + 20f, r.yMax - 72f, r.width - 40f, 56f);
            var e = Event.current;
            bool key = e.type == EventType.KeyDown && (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter || e.keyCode == KeyCode.Escape);
            if (GUI.Button(done, "DONE", DoneStyle()) || key)
            {
                if (key) e.Use();
                guiActions.Add(() => mulliganSeat = -1);
            }
        }

        // ================================================================== Discard piles and Lost Zones

        private void OpenPile(int seat, bool lostZone)
        {
            pileSeat = seat;
            pileIsLostZone = lostZone;
            pileScroll = Vector2.zero;
        }

        private void DrawPileViewer(UiStyles ui)
        {
            if (current == null || pileSeat < 0 || pileSeat > 1)
            {
                guiActions.Add(() => pileSeat = -1);
                return;
            }
            var pile = pileIsLostZone ? current.Sides[pileSeat].LostZone : current.Sides[pileSeat].Discard;
            CardDraw.Dim(0.62f);
            float w = Mathf.Min(1120f, UiScale.Width - 60f);
            float h = UiScale.Height - 80f;
            var r = new Rect((UiScale.Width - w) / 2f, 40f, w, h);
            CardDraw.Fill(r, new Color(0.11f, 0.12f, 0.15f, 0.98f));
            string owner = host != null ? host.Possessive(pileSeat) : SeatName(pileSeat) + "'s";
            GUI.Label(new Rect(r.x + 20f, r.y + 10f, r.width - 180f, 34f),
                "<size=22><b>" + owner + (pileIsLostZone ? " Lost Zone" : " discard pile") + "</b></size>   " + pile.Count + (pile.Count == 1 ? " card" : " cards") + "  (newest first)", CardDraw.PanelText());
            var close = new Rect(r.xMax - 140f, r.y + 10f, 120f, 34f);
            bool closeNow = GUI.Button(close, "Close");

            // A count of each card, then the cards themselves.
            var counts = pile.GroupBy(c => c.Def.Name).Select(g => g.Key + " ×" + g.Count()).ToArray();
            GUI.Label(new Rect(r.x + 20f, r.y + 48f, r.width - 40f, 44f), string.Join("   ", counts), CardDraw.PanelText());

            var grid = new Rect(r.x + 20f, r.y + 96f, r.width - 40f, r.height - 110f);
            const float cardW = 112f, cardH = 156f, gap = 10f;
            int perRow = Mathf.Max(1, Mathf.FloorToInt((grid.width - Scrollbar + gap) / (cardW + gap)));
            int rows = (pile.Count + perRow - 1) / perRow;
            var content = new Rect(0f, 0f, grid.width - Scrollbar, Mathf.Max(10f, rows * (cardH + gap)));
            CardDef hover = null;
            pileScroll = GUI.BeginScrollView(grid, pileScroll, content);
            for (int k = 0; k < pile.Count; k++)
            {
                var c = pile[pile.Count - 1 - k];
                var cr = new Rect((k % perRow) * (cardW + gap), (k / perRow) * (cardH + gap), cardW, cardH);
                if (cr.yMax < pileScroll.y || cr.y > pileScroll.y + grid.height) continue;
                CardDraw.Card(cr, c.Def);
                if (cr.Contains(Event.current.mousePosition)) hover = c.Def;
            }
            GUI.EndScrollView();
            if (pile.Count == 0) GUI.Label(grid, "Empty.", CardDraw.PanelText());

            if (hover != null && grid.Contains(mouseGui)) DrawCardPreview(ui, hover);

            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                closeNow = true;
                e.Use();
            }
            if (e.type == EventType.MouseDown && !r.Contains(e.mousePosition))
            {
                closeNow = true;
                e.Use();
            }
            if (closeNow) guiActions.Add(() => pileSeat = -1);
        }

        /// <summary>A large picture of a card with its text, beside the mouse.</summary>
        private void DrawCardPreview(UiStyles ui, CardDef def)
        {
            float h = Mathf.Min(420f, UiScale.Height - 40f);
            float w = h * 63f / 88f;
            string text = CardText.Describe(def);
            const float textW = 300f;
            float textH = ui.Tooltip.CalcHeight(new GUIContent(text), textW);
            float total = w + 8f + textW;
            float x = mouseGui.x + 24f;
            if (x + total > UiScale.Width - 10f) x = mouseGui.x - 24f - total;
            x = Mathf.Max(10f, x);
            float y = Mathf.Clamp(mouseGui.y - 80f, 10f, Mathf.Max(10f, UiScale.Height - Mathf.Max(h, textH) - 10f));
            CardDraw.Card(new Rect(x, y, w, h), def);
            GUI.Box(new Rect(x + w + 8f, y, textW, textH), text, ui.Tooltip);
        }

        // ================================================================== Pokémon pop-up

        /// <summary>
        /// A Pokémon in play, large, with its Abilities and attacks. During your turn the ones you can use now are
        /// highlighted green and can be clicked; Retreat is offered when you can retreat. The opponent's Pokémon are read-only.
        /// </summary>
        private void DrawPokemonPopup(UiStyles ui)
        {
            var pv = current != null ? current.FindPokemon(popupUid) : null;
            if (pv == null)
            {
                guiActions.Add(() => popupUid = -1);
                return;
            }
            var def = pv.Top.Def;
            var req = shownRequest;
            bool mine = viewerSeat >= 0 && pv.Owner == viewerSeat;
            bool canAct = mine && human != null && req != null && req.Kind == DecisionKind.MainAction;

            CardDraw.Dim(0.62f);
            // The Pokémon with the cards under it (what it evolved from, newest first; v69) and its attached cards (Tools, then
            // Energy) fanned out behind it. Click one, or the arrow, to bring it to the front.
            var fan = new List<CardDef> { def };
            var fanUnder = new List<bool> { false };
            for (int k = pv.Stack.Count - 2; k >= 0; k--)
            {
                fan.Add(pv.Stack[k].Def);
                fanUnder.Add(true);
            }
            foreach (var t in pv.Tools)
            {
                fan.Add(t.Def);
                fanUnder.Add(false);
            }
            foreach (var en in pv.Energy)
            {
                fan.Add(en.Def);
                fanUnder.Add(false);
            }
            foreach (var at in pv.Attached)
            {
                fan.Add(at.Def);
                fanUnder.Add(false);
            }
            if (popupFrontUid != pv.Uid)
            {
                popupFrontUid = pv.Uid;
                popupFront = 0;
            }
            if (popupFront >= fan.Count) popupFront = 0;
            float cardH = Mathf.Min(UiScale.Height - 140f, 640f);
            float cardW = cardH * 63f / 88f;
            float panelW = Mathf.Clamp(UiScale.Width - cardW - 120f, 300f, 500f);
            float room = UiScale.Width - 40f - cardW - 24f - panelW;
            float step = fan.Count > 1 ? Mathf.Clamp(room / (fan.Count - 1), cardW * 0.06f, cardW * 0.22f) : 0f;
            float fanW = cardW + step * (fan.Count - 1);
            float x0 = Mathf.Max(10f, (UiScale.Width - (fanW + 24f + panelW)) / 2f);
            float y0 = (UiScale.Height - cardH) / 2f;
            var cardRect = new Rect(x0, y0, cardW, cardH);
            var fanRect = new Rect(x0, y0, fanW, cardH);
            var panel = new Rect(Mathf.Min(x0 + fanW + 24f, UiScale.Width - panelW - 10f), y0, panelW, cardH);
            for (int rank = fan.Count - 1; rank >= 1; rank--)
            {
                float inset = cardH * 0.025f;
                var behind = new Rect(x0 + step * rank + inset * 0.7f, y0 + inset, cardW - inset * 1.4f, cardH - inset * 2f);
                CardDraw.Card(behind, fan[(popupFront + rank) % fan.Count]);
            }
            CardDraw.Card(cardRect, fan[popupFront]);
            if (fan.Count > 1)
            {
                var arrow = new Rect(cardRect.xMax - 22f, cardRect.center.y - 22f, 44f, 44f);
                if (GUI.Button(arrow, "\u203A")) guiActions.Add(() => popupFront = (popupFront + 1) % Mathf.Max(1, fan.Count));
                if (popupFront != 0)
                    GUI.Label(new Rect(cardRect.x, cardRect.yMax + 4f, cardRect.width, 22f),
                        (fanUnder[popupFront] ? "Under " + def.Name + " (it evolved from this)" : "Attached to " + def.Name) + "  (" + popupFront + " of " + (fan.Count - 1) + ")",
                        CardDraw.PanelText());
            }
            CardDraw.Fill(panel, new Color(0.11f, 0.12f, 0.15f, 0.97f));

            var text = CardDraw.PanelText();
            int clicked = -1;
            bool close = false;
            GUILayout.BeginArea(new Rect(panel.x + 12f, panel.y + 10f, panel.width - 24f, panel.height - 20f));
            GUILayout.Label("<size=22><b>" + def.Name + "</b></size>     <size=18>HP " + Mathf.Max(0, pv.RemainingHp) + " / " + pv.MaxHp + "</size>", text);
            bool active = current.Sides[pv.Owner].Active == pv;
            string whose = host != null ? host.Possessive(pv.Owner) : "";
            GUILayout.Label(def.Stage + " " + def.PokemonType + (def.IsEx ? " ex" : "") + "   ·   " + whose + (active ? " Active Pokémon" : " Benched Pokémon"), text);
            if (pv.Tools.Count > 0 || pv.Energy.Count > 0 || pv.Attached.Count > 0)
            {
                var energyNames = pv.Energy.GroupBy(c => c.Def.Name).Select(g => g.Key + (g.Count() > 1 ? " ×" + g.Count() : "")).ToArray();
                string attached = "<b>Energy (" + pv.Energy.Count + "):</b> " + (pv.Energy.Count == 0 ? "none" : string.Join(", ", energyNames));
                foreach (var tool in pv.Tools) attached += "\n<b>Tool:</b> " + tool.Def.Name + " – " + tool.Def.Text;
                foreach (var extra in pv.Attached) attached += "\n<b>Attached:</b> " + extra.Def.Name;
                GUILayout.Box(attached, CardDraw.Panel());
            }
            else GUILayout.Label("No Energy or Tools attached.", text);
            if (canAct) GUILayout.Label("<color=#9BE39F>Green = you can use it now. Click it to use it.</color>", text);

            popupScroll = GUILayout.BeginScrollView(popupScroll);
            for (int i = 0; i < def.Abilities.Count; i++)
            {
                var a = def.Abilities[i];
                int index = i;
                int opt = canAct ? req.Options.FindIndex(o => o.Action == ActionType.UseAbility && o.PokemonUid == pv.Uid && o.Card == def && o.Index == index) : -1;
                string kindLabel = (a.VstarPower ? "VSTAR Power " : "") + (a.IsAbility ? "Ability" : a.Kind);
                string body = "<color=#FF7A7A><b>" + kindLabel + "</b></color>   <b>" + a.Name + "</b>" + (pv.AbilitiesActive || !a.IsAbility ? "" : "   <i>(blocked right now)</i>") + "\n" + a.Text;
                if (opt >= 0)
                {
                    if (GUILayout.Button(body, CardDraw.PanelHot())) clicked = opt;
                }
                else GUILayout.Box(body, CardDraw.Panel());
            }
            // Poké-Powers a LV.X keeps from the Pokémon under it.
            if (canAct)
            {
                for (int i = 0; i < req.Options.Count; i++)
                {
                    var o = req.Options[i];
                    if (o.Action != ActionType.UseAbility || o.PokemonUid != pv.Uid || o.Card == null || o.Card == def) continue;
                    if (o.Index < 0 || o.Index >= o.Card.Abilities.Count) continue;
                    var inherited = o.Card.Abilities[o.Index];
                    string label = "<color=#FF7A7A><b>" + (inherited.VstarPower ? "VSTAR Power " : "") + inherited.Kind + "</b></color>   <b>" + inherited.Name + "</b>   <i>(" + o.Card.Name + ")</i>\n" + inherited.Text;
                    if (GUILayout.Button(label, CardDraw.PanelHot())) clicked = i;
                }
            }
            for (int i = 0; i < def.Attacks.Count; i++)
            {
                var atk = def.Attacks[i];
                int index = i;
                int opt = canAct ? req.Options.FindIndex(o => o.Action == ActionType.Attack && o.PokemonUid == pv.Uid && o.Card == def && o.Index == index) : -1;
                if (opt >= 0)
                {
                    if (GUILayout.Button(AttackText(atk, null), CardDraw.PanelHot())) clicked = opt;
                }
                else GUILayout.Box(AttackText(atk, null), CardDraw.Panel());
            }
            if (canAct)
            {
                // Attacks this Pokémon can use from other cards (Memory Dive, attack-copying Abilities...).
                for (int i = 0; i < req.Options.Count; i++)
                {
                    var o = req.Options[i];
                    if (o.Action != ActionType.Attack || o.PokemonUid != pv.Uid || o.Card == null || o.Card == def) continue;
                    if (o.Index < 0 || o.Index >= o.Card.Attacks.Count) continue;
                    if (GUILayout.Button(AttackText(o.Card.Attacks[o.Index], o.Card.Name), CardDraw.PanelHot())) clicked = i;
                }
            }
            if (def.Abilities.Count == 0 && def.Attacks.Count == 0) GUILayout.Label("No Abilities or attacks.", text);

            GUILayout.Space(6f);
            string weak = pv.Weakness.HasValue ? pv.Weakness.Value + (pv.Weakness2.HasValue ? "/" + pv.Weakness2.Value : "") + (def.WeaknessAmount > 0 ? " +" + def.WeaknessAmount : " ×2") : "none";
            string res = def.Resistance.HasValue ? def.Resistance.Value + (def.SecondResistance.HasValue ? "/" + def.SecondResistance.Value : "") + " −" + def.ResistanceAmount : "none";
            GUILayout.Label("Weakness " + weak + "     Resistance " + res + "     Retreat " + pv.RetreatCost, text);
            if (pv.Conditions != SpecialCondition.None) GUILayout.Label("Special Conditions: " + pv.Conditions, text);
            if (pv.Notes.Count > 0) GUILayout.Label("Effects: " + string.Join("; ", pv.Notes.ToArray()), text);
            if (pv.Stack.Count > 1) GUILayout.Label("Evolved from: " + string.Join(" → ", pv.Stack.Take(pv.Stack.Count - 1).Select(c => c.Def.Name).ToArray()), text);
            if (mine && !canAct) GUILayout.Label("<i>You can use these during your turn.</i>", text);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            int retreat = canAct ? req.Options.FindIndex(o => o.Action == ActionType.Retreat && o.PokemonUid == pv.Uid) : -1;
            if (retreat >= 0 && GUILayout.Button("Retreat (cost " + pv.RetreatCost + ")", GUILayout.Height(38f))) clicked = retreat;
            int discardFossil = canAct ? req.Options.FindIndex(o => o.Action == ActionType.DiscardFossil && o.PokemonUid == pv.Uid) : -1;
            if (discardFossil >= 0 && GUILayout.Button("Discard from play", GUILayout.Height(38f))) clicked = discardFossil;
            int flipOver = canAct ? req.Options.FindIndex(o => o.Action == ActionType.FlipFaceDown && o.PokemonUid == pv.Uid) : -1;
            if (flipOver >= 0 && GUILayout.Button("Flip it over", GUILayout.Height(38f))) clicked = flipOver;
            if (GUILayout.Button("Close", GUILayout.Height(38f))) close = true;
            GUILayout.EndHorizontal();
            GUILayout.EndArea();

            var e = Event.current;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
            {
                close = true;
                e.Use();
            }
            if (e.type == EventType.KeyDown && fan.Count > 1 && (e.keyCode == KeyCode.RightArrow || e.keyCode == KeyCode.LeftArrow))
            {
                int dir = e.keyCode == KeyCode.RightArrow ? 1 : fan.Count - 1;
                int count = fan.Count;
                guiActions.Add(() => popupFront = (popupFront + dir) % count);
                e.Use();
            }
            if (e.type == EventType.MouseDown && fan.Count > 1 && fanRect.Contains(e.mousePosition) && !cardRect.Contains(e.mousePosition) && step > 0f)
            {
                // A card peeking out behind: the strip right of the front card belongs to rank 1, the next strip to rank 2...
                int rank = Mathf.Clamp(Mathf.CeilToInt((e.mousePosition.x - cardRect.xMax) / step), 1, fan.Count - 1);
                int count = fan.Count;
                guiActions.Add(() => popupFront = (popupFront + rank) % count);
                e.Use();
            }
            else if (e.type == EventType.MouseDown && !fanRect.Contains(e.mousePosition) && !panel.Contains(e.mousePosition))
            {
                close = true;
                e.Use();
            }
            if (clicked >= 0)
            {
                int pick = clicked;
                guiActions.Add(() => Submit(new List<int> { pick }));
            }
            else if (close) guiActions.Add(() => popupUid = -1);
        }

        private static string AttackText(AttackDef atk, string fromCard)
        {
            string dmg = atk.Damage > 0 ? atk.Damage + atk.DamageSuffix : "";
            string cost = atk.Cost.Length == 0 ? "free" : atk.CostString;
            return "<b>[" + cost + "]   " + (atk.VstarPower ? "VSTAR Power: " : "") + atk.Name + "</b>" + (dmg.Length > 0 ? "     <size=18><b>" + dmg + "</b></size>" : "")
                   + (fromCard != null ? "   <i>(from " + fromCard + ")</i>" : "")
                   + (string.IsNullOrEmpty(atk.Text) ? "" : "\n" + atk.Text);
        }

        // ================================================================== Card picker (Valid / All)

        /// <summary>
        /// Shown whenever an effect asks you to choose cards: Valid lists the cards you can pick; All lists every card the
        /// effect lets you see (the cards you looked at, your whole deck for a search, your discard pile...), with the
        /// ones you can't pick dimmed. Pick cards, then press Done. "Inspect board" hides the picker for a moment.
        /// </summary>
        private void DrawCardPicker(UiStyles ui)
        {
            var req = shownRequest;
            CardDraw.Dim(0.58f);
            bool hasAll = req.SeenCards != null && req.SeenCards.Count > req.Options.Count;
            bool nothingValid = req.Options.Count == 0;
            bool allTab = hasAll && (pickerAllTab || nothingValid);

            var optionOf = new Dictionary<int, int>();
            for (int i = 0; i < req.Options.Count; i++) optionOf[req.Options[i].CardId] = i;
            var shown = new List<KeyValuePair<CardDef, int>>();
            if (allTab)
            {
                foreach (var c in req.SeenCards)
                {
                    int idx;
                    shown.Add(new KeyValuePair<CardDef, int>(c.Def, optionOf.TryGetValue(c.Id, out idx) ? idx : -1));
                }
            }
            else
            {
                for (int i = 0; i < req.Options.Count; i++) shown.Add(new KeyValuePair<CardDef, int>(req.Options[i].Card, i));
            }

            // Tabs
            const float top = 72f;
            var validRect = new Rect(40f, top - 50f, 180f, 40f);
            var allRect = new Rect(226f, top - 50f, 180f, 40f);
            GUI.enabled = !nothingValid;
            if (GUI.Toggle(validRect, !allTab, "VALID   " + req.Options.Count, TabStyle()) && allTab) guiActions.Add(() => pickerAllTab = false);
            GUI.enabled = true;
            if (hasAll)
            {
                if (GUI.Toggle(allRect, allTab, "ALL   " + req.SeenCards.Count, TabStyle()) && !allTab) guiActions.Add(() => pickerAllTab = true);
                if (allTab && !string.IsNullOrEmpty(req.SeenTitle))
                    GUI.Label(new Rect(allRect.xMax + 16f, top - 46f, 500f, 34f), req.SeenTitle + " (dimmed cards can't be chosen)", WhiteSmall());
            }

            // The card being played, on the right.
            float sourceW = req.Source != null ? Mathf.Min(240f, UiScale.Width * 0.18f) : 0f;
            if (req.Source != null) CardDraw.Card(new Rect(UiScale.Width - 40f - sourceW, top, sourceW, sourceW * 88f / 63f), req.Source);

            var grid = new Rect(40f, top, UiScale.Width - 80f - (sourceW > 0f ? sourceW + 30f : 0f), UiScale.Height - top - 120f);
            float cardW = Mathf.Clamp((grid.width - Scrollbar) / 6f - 12f, 100f, 170f);
            float cardH = cardW * 88f / 63f;
            const float gap = 12f;
            int perRow = Mathf.Max(1, Mathf.FloorToInt((grid.width - Scrollbar + gap) / (cardW + gap)));
            int rows = (shown.Count + perRow - 1) / perRow;
            var content = new Rect(0f, 0f, grid.width - Scrollbar, Mathf.Max(10f, rows * (cardH + gap) + 8f));
            CardDef hover = null;
            pickerScroll = GUI.BeginScrollView(grid, pickerScroll, content);
            for (int k = 0; k < shown.Count; k++)
            {
                var cr = new Rect((k % perRow) * (cardW + gap) + 4f, (k / perRow) * (cardH + gap) + 4f, cardW, cardH);
                var kv = shown[k];
                bool valid = kv.Value >= 0;
                bool selected = valid && multiSelection.Contains(kv.Value);
                CardDraw.Card(cr, kv.Key, valid ? 1f : 0.32f);
                if (selected)
                {
                    CardDraw.Frame(cr, ui.Selected, 5f);
                    var tag = new Rect(cr.x, cr.yMax - 26f, cr.width, 26f);
                    GUI.DrawTexture(tag, CardVisuals.Solid(new Color(0.1f, 0.55f, 0.25f, 0.92f)));
                    GUI.Label(tag, "SELECTED", ui.Badge);
                }
                else if (valid) CardDraw.Frame(cr, ui.Highlight, 2f);
                if (cr.Contains(Event.current.mousePosition)) hover = kv.Key;
                if (valid && GUI.Button(cr, GUIContent.none, GUIStyle.none))
                {
                    int idx = kv.Value;
                    guiActions.Add(() => TogglePick(req, idx));
                }
            }
            GUI.EndScrollView();

            // Banner: what to choose, how many are selected, Done.
            var banner = new Rect(0f, UiScale.Height - 104f, UiScale.Width, 96f);
            CardDraw.Fill(banner, new Color(0.08f, 0.33f, 0.66f, 0.96f));
            GUI.Label(new Rect(30f, banner.y + 6f, UiScale.Width - 440f, 52f), req.Prompt, CardDraw.Banner());
            string count = nothingValid
                ? "None of these cards can be chosen. Press Done when you've finished looking."
                : "Selected " + multiSelection.Count
                  + (req.MaxChoices == 1 ? "" : " (up to " + req.MaxChoices + ")")
                  + (req.MinChoices > 0 && req.MinChoices != req.MaxChoices ? ", at least " + req.MinChoices : "")
                  + (req.MinChoices == 0 ? "  ·  You may choose none" : "");
            GUI.Label(new Rect(30f, banner.y + 58f, UiScale.Width - 440f, 28f), count, WhiteSmall());
            bool ok = multiSelection.Count >= req.MinChoices && multiSelection.Count <= req.MaxChoices;
            GUI.enabled = ok;
            if (GUI.Button(new Rect(UiScale.Width - 390f, banner.y + 18f, 170f, 58f), "DONE", DoneStyle()))
                guiActions.Add(() => Submit(multiSelection.OrderBy(x => x).ToList()));
            GUI.enabled = true;
            if (GUI.Button(new Rect(UiScale.Width - 205f, banner.y + 18f, 175f, 58f), "Inspect board"))
                guiActions.Add(() => pickerHidden = true);

            if (hover != null && grid.Contains(mouseGui)) ui.DrawTooltip(CardText.Describe(hover), mouseGui);
        }

        private void TogglePick(DecisionRequest req, int index)
        {
            if (req != shownRequest) return;
            if (multiSelection.Contains(index))
            {
                multiSelection.Remove(index);
                return;
            }
            if (req.MaxChoices == 1) multiSelection.Clear();
            if (multiSelection.Count < req.MaxChoices) multiSelection.Add(index);
        }

        /// <summary>While the picker is hidden to look at the board, a button brings it back.</summary>
        private void DrawPickerReturn(UiStyles ui)
        {
            if (!pickerHidden || shownRequest == null || human == null || !IsCardChoice(shownRequest) || ModalOpen) return;
            var r = new Rect(UiScale.Width / 2f - 150f, 12f, 300f, 44f);
            if (GUI.Button(r, "Back to choosing cards", DoneStyle())) guiActions.Add(() => pickerHidden = false);
        }

        // ================================================================== Styles

        private GUIStyle TabStyle()
        {
            if (tabStyle == null)
            {
                tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 18, fontStyle = FontStyle.Bold };
                tabStyle.onNormal.background = CardVisuals.Solid(new Color(0.95f, 0.8f, 0.2f, 1f));
                tabStyle.onNormal.textColor = Color.black;
                tabStyle.onHover.background = tabStyle.onNormal.background;
                tabStyle.onHover.textColor = Color.black;
            }
            return tabStyle;
        }

        private GUIStyle DoneStyle()
        {
            if (doneStyle == null) doneStyle = new GUIStyle(GUI.skin.button) { fontSize = 22, fontStyle = FontStyle.Bold };
            return doneStyle;
        }

        private GUIStyle DarkCentered()
        {
            if (darkCentered == null)
            {
                darkCentered = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 18, alignment = TextAnchor.MiddleCenter };
                darkCentered.normal.textColor = new Color(0.12f, 0.12f, 0.14f);
            }
            return darkCentered;
        }

        private GUIStyle WhiteSmall()
        {
            if (whiteSmall == null)
            {
                whiteSmall = new GUIStyle(GUI.skin.label) { richText = true, wordWrap = true, fontSize = 14, alignment = TextAnchor.MiddleCenter };
                whiteSmall.normal.textColor = Color.white;
            }
            return whiteSmall;
        }
    }
}
