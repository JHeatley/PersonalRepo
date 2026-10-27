using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PokemonTCG
{
    /// <summary>
    /// The whole front end: a menu to pick decks and the bot, and a board that shows
    /// the game the Python server is running. Everything is built from code at startup,
    /// so the project needs no scene setup (see TcgBootstrap).
    /// </summary>
    public class TcgApp : MonoBehaviour
    {
        public string serverUrl = "http://127.0.0.1:8765";
        public float botDelay = 0.7f;

        TcgClient client;
        ImageCache images;
        RectTransform root;

        // Menu
        RectTransform menu;
        InputField urlField;
        Text menuStatus;
        Button deckButton, botDeckButton, botButton;
        string[] decks = new string[0];
        string[] bots = new string[0];
        int deckIndex = -1, botDeckIndex = -1, botIndex;

        // Board
        RectTransform board;
        Text oppInfo, youInfo, banner, promptText;
        RectTransform oppBench, oppActive, youActive, youBench, hand, stadiumSlot, optionList;
        Button showAllButton, slowerButton, fasterButton;
        Text logText;
        ScrollRect logScroll;
        RectTransform preview;
        RawImage previewArt;
        Text previewText;
        RectTransform resultPanel;
        Text resultText;

        StateData state;
        bool busy;
        float nextAdvance;
        readonly List<CardView> views = new List<CardView>();
        List<OptionData> filter;
        int lastLogCount;
        string lastGame;

        void Start()
        {
            client = new TcgClient(serverUrl);
            images = new ImageCache(this);
            EnsureCameraAndEvents();
            BuildCanvas();
            BuildMenu();
            BuildBoard();
            ShowMenu();
        }

        void Update()
        {
            if (state == null || busy || state.done || !board.gameObject.activeSelf)
                return;
            if (state.waiting_on == "bot" && Time.time >= nextAdvance)
                Send("/advance", new GameRequest { game = state.game });
        }

        // ================================================================ setup
        void EnsureCameraAndEvents()
        {
            if (Camera.main == null)
            {
                var cam = new GameObject("Main Camera").AddComponent<Camera>();
                cam.tag = "MainCamera";
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = Ui.Background;
            }
            else
            {
                Camera.main.clearFlags = CameraClearFlags.SolidColor;
                Camera.main.backgroundColor = Ui.Background;
            }
            if (FindFirstObjectByType<EventSystem>() != null)
                return;
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
            var module = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (module != null)
                es.AddComponent(module);
            else
                Debug.LogError("No UI input module found. Set Project Settings > Player > Active Input Handling to 'Both'.");
#else
            es.AddComponent<StandaloneInputModule>();
#endif
        }

        void BuildCanvas()
        {
            var go = new GameObject("Canvas", typeof(RectTransform));
            go.transform.SetParent(transform, false);
            var canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            go.AddComponent<GraphicRaycaster>();
            root = (RectTransform)go.transform;
        }

        // ================================================================= menu
        void BuildMenu()
        {
            menu = Ui.Fill(Ui.Rect("Menu", root));
            var bg = menu.gameObject.AddComponent<Image>();
            bg.color = Ui.Background;
            var panel = Ui.Panelled("Panel", menu, Ui.Panel).rectTransform;
            Ui.Place(panel, 0.3f, 0.12f, 0.7f, 0.88f);
            var col = Ui.Column(panel, 16, TextAnchor.UpperCenter);
            col.padding = new RectOffset(40, 40, 36, 36);

            Ui.Size(Ui.Label(panel, "Pokémon TCG vs Bot", 54, TextAnchor.MiddleCenter, Ui.Accent, FontStyle.Bold), -1, 80);
            Ui.Size(Ui.Label(panel, "Game server", 20, TextAnchor.MiddleLeft, Ui.Muted), -1, 26);
            var urlBg = Ui.Panelled("Url", panel, new Color(0.92f, 0.93f, 0.95f));
            Ui.Size(urlBg, -1, 48);
            urlField = urlBg.gameObject.AddComponent<InputField>();
            var urlText = Ui.Label(urlBg.transform, "", 22, TextAnchor.MiddleLeft, Color.black);
            urlText.supportRichText = false;
            Ui.Fill(urlText.rectTransform, 10);
            urlField.textComponent = urlText;
            urlField.text = serverUrl;

            deckButton = Cycler(panel, "Your deck", () => deckIndex = Step(deckIndex, decks.Length, -1, true),
                                () => deckIndex = Step(deckIndex, decks.Length, 1, true));
            botDeckButton = Cycler(panel, "Bot's deck", () => botDeckIndex = Step(botDeckIndex, decks.Length, -1, true),
                                   () => botDeckIndex = Step(botDeckIndex, decks.Length, 1, true));
            botButton = Cycler(panel, "Bot", () => botIndex = Step(botIndex, bots.Length, -1, false),
                               () => botIndex = Step(botIndex, bots.Length, 1, false));

            Ui.Size(Ui.Rect("Gap", panel), -1, 10);
            Ui.Size(Ui.Button(panel, "Play", () => StartGame(false), 30), -1, 64);
            Ui.Size(Ui.Button(panel, "Watch bot vs bot", () => StartGame(true), 24), -1, 52);
            Ui.Size(Ui.Button(panel, "Reconnect to server", LoadInfo, 20), -1, 44);
            menuStatus = Ui.Label(panel, "", 20, TextAnchor.UpperCenter, Ui.Muted);
            Ui.Size(menuStatus, -1, 90);
        }

        Button Cycler(RectTransform parent, string title, Action prev, Action next)
        {
            Ui.Size(Ui.Label(parent, title, 20, TextAnchor.MiddleLeft, Ui.Muted), -1, 26);
            var row = Ui.Rect("Row", parent);
            Ui.Size(row, -1, 50);
            Ui.Row(row, 8, TextAnchor.MiddleCenter);
            Ui.Size(Ui.Button(row, "<", () => { prev(); RefreshMenu(); }, 28), 60, 50);
            var label = Ui.Button(row, "", () => { next(); RefreshMenu(); }, 24);
            var le = Ui.Size(label, 0, 50);
            le.flexibleWidth = 1;
            Ui.Size(Ui.Button(row, ">", () => { next(); RefreshMenu(); }, 28), 60, 50);
            return label;
        }

        static int Step(int index, int count, int dir, bool allowRandom)
        {
            if (count == 0) return allowRandom ? -1 : 0;
            int lo = allowRandom ? -1 : 0;
            int span = count - lo;
            return ((index - lo + dir) % span + span) % span + lo;
        }

        void RefreshMenu()
        {
            Ui.SetButtonText(deckButton, deckIndex < 0 || decks.Length == 0 ? "Random" : decks[deckIndex]);
            Ui.SetButtonText(botDeckButton, botDeckIndex < 0 || decks.Length == 0 ? "Random" : decks[botDeckIndex]);
            Ui.SetButtonText(botButton, bots.Length == 0 ? "(connect first)" : bots[botIndex]);
        }

        void ShowMenu()
        {
            menu.gameObject.SetActive(true);
            board.gameObject.SetActive(false);
            state = null;
            RefreshMenu();
            LoadInfo();
        }

        void LoadInfo()
        {
            client.BaseUrl = urlField.text.Trim().TrimEnd('/');
            menuStatus.text = "Connecting to " + client.BaseUrl + " ...";
            StartCoroutine(client.GetInfo(info =>
            {
                decks = info.decks ?? new string[0];
                bots = info.bots ?? new string[0];
                botIndex = Mathf.Clamp(botIndex, 0, Math.Max(0, bots.Length - 1));
                menuStatus.text = "Connected. " + decks.Length + " decks available.";
                RefreshMenu();
            }, err => menuStatus.text = err));
        }

        void StartGame(bool watch)
        {
            if (busy) return;
            client.BaseUrl = urlField.text.Trim().TrimEnd('/');
            var req = new NewGameRequest
            {
                deck = deckIndex >= 0 && deckIndex < decks.Length ? decks[deckIndex] : "",
                bot_deck = botDeckIndex >= 0 && botDeckIndex < decks.Length ? decks[botDeckIndex] : "",
                bot = botIndex < bots.Length ? bots[botIndex] : "",
                watch = watch,
            };
            menuStatus.text = "Starting...";
            busy = true;
            StartCoroutine(client.Post("/new", req, s =>
            {
                busy = false;
                menu.gameObject.SetActive(false);
                board.gameObject.SetActive(true);
                Show(s);
            }, err =>
            {
                busy = false;
                menuStatus.text = err;
            }));
        }

        // ================================================================ board
        void BuildBoard()
        {
            board = Ui.Fill(Ui.Rect("Board", root));
            var bg = board.gameObject.AddComponent<Image>();
            bg.color = Ui.Background;

            var field = Ui.Place(Ui.Rect("Field", board), 0, 0, 0.76f, 1);
            oppInfo = Ui.Label(field, "", 24, TextAnchor.MiddleCenter, Ui.Muted);
            Ui.Place(oppInfo.rectTransform, 0.01f, 0.96f, 0.99f, 1f);
            oppBench = Ui.Place(Ui.Rect("OppBench", field), 0, 0.80f, 1, 0.96f);
            Ui.Row(oppBench, 10, TextAnchor.MiddleCenter);
            oppActive = Ui.Place(Ui.Rect("OppActive", field), 0.2f, 0.60f, 0.8f, 0.80f);
            Ui.Row(oppActive, 10, TextAnchor.MiddleCenter);

            var mid = Ui.Panelled("Banner", field, new Color(1, 1, 1, 0.05f));
            Ui.Place(mid.rectTransform, 0.15f, 0.56f, 0.85f, 0.60f);
            banner = Ui.Label(mid.transform, "", 26, TextAnchor.MiddleCenter, Color.white, FontStyle.Bold);
            Ui.Fill(banner.rectTransform);

            stadiumSlot = Ui.Place(Ui.Rect("Stadium", field), 0.01f, 0.42f, 0.14f, 0.74f);
            Ui.Column(stadiumSlot, 4, TextAnchor.MiddleCenter).childForceExpandWidth = false;

            youActive = Ui.Place(Ui.Rect("YouActive", field), 0.2f, 0.355f, 0.8f, 0.555f);
            Ui.Row(youActive, 10, TextAnchor.MiddleCenter);
            youBench = Ui.Place(Ui.Rect("YouBench", field), 0, 0.195f, 1, 0.355f);
            Ui.Row(youBench, 10, TextAnchor.MiddleCenter);
            youInfo = Ui.Label(field, "", 22, TextAnchor.MiddleCenter, Ui.Muted);
            Ui.Place(youInfo.rectTransform, 0.01f, 0.165f, 0.99f, 0.195f);
            var handArea = Ui.Place(Ui.Rect("Hand", field), 0.005f, 0.005f, 0.995f, 0.165f);
            hand = Ui.Scroll(handArea, false, new Color(0, 0, 0, 0.25f));

            // Right side: decision, options and log
            var side = Ui.Panelled("Side", board, Ui.Panel).rectTransform;
            Ui.Place(side, 0.76f, 0, 1, 1);
            var top = Ui.Place(Ui.Rect("Top", side), 0.03f, 0.95f, 0.97f, 0.995f);
            Ui.Row(top, 8, TextAnchor.MiddleCenter);
            Ui.Size(Ui.Button(top, "Menu", ShowMenu, 20), 120, 42);
            slowerButton = Ui.Button(top, "Slower", () => botDelay = Mathf.Min(3f, botDelay * 1.5f), 20);
            Ui.Size(slowerButton, 120, 42);
            fasterButton = Ui.Button(top, "Faster", () => botDelay = Mathf.Max(0.05f, botDelay / 1.5f), 20);
            Ui.Size(fasterButton, 120, 42);

            promptText = Ui.Label(side, "", 24, TextAnchor.UpperLeft, Ui.Accent, FontStyle.Bold);
            Ui.Place(promptText.rectTransform, 0.04f, 0.83f, 0.96f, 0.94f);
            optionList = Ui.Scroll(Ui.Place(Ui.Rect("Options", side), 0.03f, 0.42f, 0.97f, 0.83f), true,
                                   new Color(0, 0, 0, 0.2f));
            showAllButton = Ui.Button(side, "Show all options", () => { filter = null; Render(); }, 18);
            Ui.Place((RectTransform)showAllButton.transform, 0.03f, 0.375f, 0.97f, 0.415f);

            var logArea = Ui.Place(Ui.Rect("Log", side), 0.03f, 0.01f, 0.97f, 0.365f);
            var logContent = Ui.Scroll(logArea, true, new Color(0, 0, 0, 0.3f));
            logScroll = logArea.GetComponent<ScrollRect>();
            logText = Ui.Label(logContent, "", 17, TextAnchor.UpperLeft, Ui.TextColor);
            logText.verticalOverflow = VerticalWrapMode.Overflow;

            // Hover preview over the left of the field
            preview = Ui.Panelled("Preview", board, new Color(0.05f, 0.06f, 0.09f, 0.96f)).rectTransform;
            Ui.Place(preview, 0.005f, 0.17f, 0.24f, 0.99f);
            preview.GetComponent<Image>().raycastTarget = false;
            var artRt = Ui.Place(Ui.Rect("Art", preview), 0.05f, 0.40f, 0.95f, 0.98f);
            previewArt = artRt.gameObject.AddComponent<RawImage>();
            previewArt.raycastTarget = false;
            previewText = Ui.Label(preview, "", 17, TextAnchor.UpperLeft, Ui.TextColor);
            Ui.Place(previewText.rectTransform, 0.05f, 0.01f, 0.95f, 0.39f);
            preview.gameObject.SetActive(false);

            // Game over
            resultPanel = Ui.Panelled("Result", board, new Color(0, 0, 0, 0.85f)).rectTransform;
            Ui.Place(resultPanel, 0.2f, 0.38f, 0.56f, 0.62f);
            var rcol = Ui.Column(resultPanel, 14, TextAnchor.MiddleCenter);
            rcol.padding = new RectOffset(30, 30, 24, 24);
            resultText = Ui.Label(resultPanel, "", 36, TextAnchor.MiddleCenter, Ui.Accent, FontStyle.Bold);
            Ui.Size(resultText, -1, 110);
            Ui.Size(Ui.Button(resultPanel, "Play again", () => StartGame(state != null && state.watch), 24), -1, 52);
            Ui.Size(Ui.Button(resultPanel, "Menu", ShowMenu, 22), -1, 46);
            resultPanel.gameObject.SetActive(false);
        }

        // ============================================================== network
        void Send(string path, GameRequest req)
        {
            busy = true;
            StartCoroutine(client.Post(path, req, s =>
            {
                busy = false;
                Show(s);
            }, err =>
            {
                busy = false;
                promptText.text = err;
                promptText.color = new Color(1f, 0.45f, 0.4f);
                nextAdvance = Time.time + 2f;
            }));
        }

        void Choose(int option)
        {
            if (busy || state == null || state.waiting_on != "you") return;
            filter = null;
            Send("/choose", new GameRequest { game = state.game, option = option });
        }

        void Show(StateData s)
        {
            if (s.game != lastGame)
            {
                lastGame = s.game;
                lastLogCount = 0;
                logText.text = "";
                filter = null;
            }
            state = s;
            nextAdvance = Time.time + (s.watch ? botDelay : botDelay * 0.8f);
            AppendLog();
            Render();
        }

        // =============================================================== render
        void Render()
        {
            foreach (var area in new[] { oppBench, oppActive, youActive, youBench, hand, stadiumSlot, optionList })
                Ui.Clear(area);
            views.Clear();
            preview.gameObject.SetActive(false);

            var s = state;
            oppInfo.text = SideInfo("Bot (" + s.bot_deck + ")", s.opp, true);
            youInfo.text = SideInfo(s.watch ? "Bot (" + s.your_deck + ")" : "You (" + s.your_deck + ")", s.you, false);

            foreach (var m in s.opp.active) AddMon(oppActive, m, 150, "opp:0");
            for (int i = 0; i < s.opp.bench.Length; i++) AddMon(oppBench, s.opp.bench[i], 112, "opp:" + (i + 1));
            foreach (var m in s.you.active) AddMon(youActive, m, 150, "you:0");
            for (int i = 0; i < s.you.bench.Length; i++) AddMon(youBench, s.you.bench[i], 112, "you:" + (i + 1));
            foreach (var c in s.you.hand) AddCard(hand, c, 108, "hand");
            if (s.stadium.Length > 0)
            {
                AddCard(stadiumSlot, s.stadium[0], 110, "stadium");
                var who = Ui.Label(stadiumSlot, s.stadium_owner == "you" ? "your Stadium" : "their Stadium", 16,
                                   TextAnchor.MiddleCenter, Ui.Muted);
                Ui.Size(who, 120, 22);
            }

            bool mine = s.waiting_on == "you" && s.decision.Length > 0;
            if (s.done)
                banner.text = "Game over";
            else if (s.in_setup)
                banner.text = "Setup";
            else
                banner.text = "Turn " + s.turn + " · " + (s.your_turn ? (s.watch ? "Bottom player" : "Your turn") :
                                                             (s.watch ? "Top player" : "Bot's turn"));

            promptText.color = Ui.Accent;
            if (mine)
                promptText.text = Prompt(s.decision[0]);
            else if (s.done)
                promptText.text = s.result;
            else
                promptText.text = s.watch ? "Watching the bot play itself..." : "Bot is thinking...";

            if (mine)
            {
                var opts = filter ?? new List<OptionData>(s.decision[0].options);
                foreach (var o in opts)
                    AddOption(o);
                foreach (var v in views)
                    v.SetHighlight(Matching(v).Count > 0);
            }
            showAllButton.gameObject.SetActive(mine && filter != null);
            slowerButton.gameObject.SetActive(s.watch);
            fasterButton.gameObject.SetActive(s.watch);

            resultPanel.gameObject.SetActive(s.done);
            resultText.text = s.result;
        }

        static string SideInfo(string who, SideData d, bool showHandCount)
        {
            var sb = new StringBuilder(who);
            sb.Append("   ·   Prizes ").Append(d.prizes);
            sb.Append("   ·   Deck ").Append(d.deck);
            sb.Append("   ·   Discard ").Append(d.discard);
            if (showHandCount) sb.Append("   ·   Hand ").Append(d.hand_count);
            return sb.ToString();
        }

        static string Prompt(DecisionData d)
        {
            string text;
            switch (d.kind)
            {
                case "main": text = "Your turn: choose an action"; break;
                case "go_first": text = "You won the coin flip"; break;
                case "setup_active": text = "Choose your Active Pokémon"; break;
                case "setup_bench": text = "Put Basic Pokémon on your Bench (Done when finished)"; break;
                case "promote": text = "Choose a new Active Pokémon"; break;
                case "search": text = "Search your deck"; break;
                case "discard": text = "Choose a card to discard"; break;
                case "counters": text = "Place a damage counter"; break;
                case "pick_pokemon": text = "Choose a Pokémon"; break;
                case "pick_energy": text = "Choose an Energy"; break;
                case "yes_no": text = "Do you want to?"; break;
                default: text = "Choose"; break;
            }
            if (d.source.Length > 0) text += "\n" + d.source[0].name;
            if (!string.IsNullOrEmpty(d.prompt)) text += ": " + d.prompt;
            return text;
        }

        CardView AddCard(RectTransform parent, CardData card, float width, string reference)
        {
            var v = CardView.Create(parent, width, images);
            v.SetCard(card);
            Hook(v, reference);
            return v;
        }

        void AddMon(RectTransform parent, MonData mon, float width, string reference)
        {
            var v = CardView.Create(parent, width, images);
            v.SetMon(mon);
            Hook(v, reference);
        }

        void Hook(CardView v, string reference)
        {
            v.Ref = reference;
            v.Clicked = OnCardClicked;
            v.HoverStart = cv => ShowPreview(cv.Card, cv.Mon);
            v.HoverEnd = cv => preview.gameObject.SetActive(false);
            views.Add(v);
        }

        void AddOption(OptionData o)
        {
            var row = Ui.Rect("Option", optionList);
            Ui.Size(row, -1, 50);
            Ui.Row(row, 6, TextAnchor.MiddleLeft);
            // Cards that aren't on the board (search results, discard choices...) get a thumbnail
            if (o.card.Length > 0 && o.card_ref != "hand" && string.IsNullOrEmpty(o.target))
            {
                var thumb = CardView.Create(row, 34, images);
                thumb.SetCard(o.card[0]);
                thumb.HoverStart = cv => ShowPreview(cv.Card, null);
                thumb.HoverEnd = cv => preview.gameObject.SetActive(false);
                int index = o.index;
                thumb.Clicked = cv => Choose(index);
            }
            int idx = o.index;
            var b = Ui.Button(row, o.label, () => Choose(idx), 19);
            var le = Ui.Size(b, 0, 48);
            le.flexibleWidth = 1;
            if (o.kind == "end" || o.kind == "done" || o.kind == "no")
                b.GetComponent<Image>().color = new Color(0.35f, 0.38f, 0.45f);
            else if (o.kind == "attack")
                b.GetComponent<Image>().color = new Color(0.72f, 0.28f, 0.22f);
        }

        List<OptionData> Matching(CardView v)
        {
            var list = new List<OptionData>();
            if (state == null || state.waiting_on != "you" || state.decision.Length == 0 || v.Card == null)
                return list;
            foreach (var o in state.decision[0].options)
            {
                if (v.Ref == "hand")
                {
                    if (o.card_ref == "hand" && o.card.Length > 0 && o.card[0].id == v.Card.id)
                        list.Add(o);
                }
                else if (!string.IsNullOrEmpty(v.Ref) && o.target == v.Ref)
                {
                    list.Add(o);
                }
            }
            return list;
        }

        void OnCardClicked(CardView v)
        {
            var m = Matching(v);
            if (m.Count == 1)
                Choose(m[0].index);
            else if (m.Count > 1)
            {
                filter = m;
                Render();
            }
        }

        void ShowPreview(CardData card, MonData mon)
        {
            if (card == null) return;
            preview.gameObject.SetActive(true);
            preview.SetAsLastSibling();
            var text = new StringBuilder();
            text.Append("<b>").Append(card.name).Append("</b>\n");
            if (mon != null)
            {
                text.Append("HP ").Append(Mathf.Max(0, mon.hp - mon.damage)).Append("/").Append(mon.hp);
                if (mon.energy.Length > 0) text.Append("   Energy ").Append(string.Join(" ", mon.energy));
                text.Append("\n");
                if (mon.tool.Length > 0) text.Append("Tool: ").Append(mon.tool[0].name).Append("\n");
                if (mon.conditions.Length > 0) text.Append(string.Join(", ", mon.conditions)).Append("\n");
            }
            text.Append(card.text);
            previewText.text = text.ToString();
            previewArt.enabled = false;
            string url = !string.IsNullOrEmpty(card.image_large) ? card.image_large : card.image;
            images.Get(url, tex =>
            {
                if (tex == null && url != card.image)
                {
                    images.Get(card.image, small => SetPreviewArt(small));
                    return;
                }
                SetPreviewArt(tex);
            });
        }

        void SetPreviewArt(Texture2D tex)
        {
            if (tex == null || previewArt == null) return;
            previewArt.texture = tex;
            previewArt.enabled = true;
            var fitter = previewArt.GetComponent<AspectRatioFitter>();
            if (fitter == null)
            {
                fitter = previewArt.gameObject.AddComponent<AspectRatioFitter>();
                fitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            }
            fitter.aspectRatio = (float)tex.width / tex.height;
        }

        void AppendLog()
        {
            if (state.log == null) return;
            var sb = new StringBuilder();
            for (int i = lastLogCount; i < state.log.Length; i++)
            {
                string line = state.log[i];
                if (line.StartsWith("---"))
                    sb.Append("<color=#FFCC33>").Append(line).Append("</color>\n");
                else
                    sb.Append(line).Append('\n');
            }
            lastLogCount = state.log.Length;
            if (sb.Length == 0) return;
            string all = logText.text + sb;
            // Keep the log short enough for a single Text component
            if (all.Length > 12000)
                all = all.Substring(all.Length - 12000);
            logText.text = all;
            Canvas.ForceUpdateCanvases();
            logScroll.verticalNormalizedPosition = 0;
        }
    }
}
