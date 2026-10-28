using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokemonTCG.AI;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// The "Learn from a video" screen (v77): the video on the left with the layout's boxes over it, and four tabs on the
    /// right: 1 Layout (draw the boxes), 2 Read (read the video, fix names), 3 Events (check, fix and tag), 4 Study.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private static readonly string[] VideoTabs = { "1. Layout", "2. Read", "3. Events", "4. Study" };
        private static readonly string[] BoxKindNames = { "Text", "Number", "Icons", "Card" };
        private static readonly string[] IntervalLabels = { "Every 0.5 s", "Every 1 s", "Every 2 s" };
        private static readonly double[] Intervals = { 0.5, 1.0, 2.0 };
        private static readonly VideoEventKind[] TagKinds =
        {
            VideoEventKind.Attack, VideoEventKind.Ability, VideoEventKind.Trainer, VideoEventKind.Energy, VideoEventKind.Bench,
            VideoEventKind.Evolve, VideoEventKind.Retreat, VideoEventKind.Active, VideoEventKind.KnockOut, VideoEventKind.Prizes,
            VideoEventKind.EndTurn, VideoEventKind.Turn, VideoEventKind.Win, VideoEventKind.Line
        };
        private static readonly string[] TagKindNames =
        {
            "Attack", "Ability", "Trainer", "Energy", "To Bench", "Evolve", "Retreat", "New Active", "Knock Out", "Prizes",
            "End turn", "Turn starts", "Wins", "Log line"
        };

        private int videoTab;
        private int videoRolePlayer;
        private string videoRole = "";
        private bool videoDrawing;
        private Vector2 videoDragFrom, videoDragTo;
        private Rect videoRect;
        private Vector2 videoTabScroll, videoEventScroll, videoLogScroll;
        private string videoLayoutNameInput = "";
        private bool videoConfirmDelete;
        private int tagKind, tagPlayer, tagSpot;
        private string tagCard = "", tagCard2 = "", tagMove = "", tagAmount = "", tagText = "";
        private string tagError;
        private int videoAttackPick = -1;

        private void DrawVideo(UiStyles ui)
        {
            var actions = new List<Action>();
            var r = new Rect(10f, 10f, UiScale.Width - 20f, UiScale.Height - 20f);
            GUI.Box(r, GUIContent.none);

            // Top bar: the video file.
            GUILayout.BeginArea(new Rect(r.x + 10, r.y + 6, r.width - 20, 70));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Learn from a video", ui.Header, GUILayout.Width(250));
            GUI.enabled = videoJob == null && !YouTubeBusy;
            GUILayout.Label("Video file or YouTube link:", ui.Label, GUILayout.Width(175));
            videoPathInput = GUILayout.TextField(videoPathInput ?? "", GUILayout.MinWidth(200));
            if (GUILayout.Button("Open", GUILayout.Width(70))) actions.Add(() => OpenVideoInput(videoPathInput));
            if (FilePicker.CanBrowse && GUILayout.Button("Browse...", GUILayout.Width(90))) actions.Add(() => pendingVideoBrowse = true);
            GUI.enabled = true;
            if (youTube != null && GUILayout.Button("Cancel download", GUILayout.Width(130))) actions.Add(() => youTube.Cancel());
            if (youTubeNeedsTool && youTube == null)
            {
                GUI.enabled = youTubeToolDownload == null;
                if (GUILayout.Button("Get yt-dlp", GUILayout.Width(100))) actions.Add(StartYouTubeToolDownload);
                GUI.enabled = true;
            }
            if (GUILayout.Button("Back", GUILayout.Width(80))) actions.Add(() => CloseVideoScreen());
            GUILayout.EndHorizontal();
            if (!string.IsNullOrEmpty(videoMessage)) GUILayout.Label(videoMessage, Hint(ui));
            GUILayout.EndArea();

            float panelW = Mathf.Clamp(r.width * 0.36f, 380f, 560f);
            var videoArea = new Rect(r.x + 10, r.y + 80, r.width - panelW - 30, r.height - 150);
            DrawVideoView(ui, videoArea, actions);
            DrawVideoControls(ui, new Rect(videoArea.x, videoArea.yMax + 6, videoArea.width, 56), actions);

            var panel = new Rect(videoArea.xMax + 10, r.y + 80, panelW, r.height - 90);
            GUILayout.BeginArea(panel);
            int tab = GUILayout.Toolbar(videoTab, VideoTabs);
            if (tab != videoTab) actions.Add(() => videoTab = tab);
            videoTabScroll = GUILayout.BeginScrollView(videoTabScroll);
            switch (videoTab)
            {
                case 0: DrawVideoLayoutTab(ui, actions); break;
                case 1: DrawVideoReadTab(ui, actions); break;
                case 2: DrawVideoEventsTab(ui, actions); break;
                default: DrawVideoStudyTab(ui, actions); break;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            foreach (var a in actions) a();
        }

        // ------------------------------------------------------------------ The video and the boxes

        private void DrawVideoView(UiStyles ui, Rect area, List<Action> actions)
        {
            GUI.Box(area, GUIContent.none);
            if (videoFrame == null || !videoReady)
            {
                GUI.Label(new Rect(area.x + 20, area.y + 20, area.width - 40, 80),
                    "Open a video of a game: a stream with an overlay, a recording of Pokémon TCG Live, or a camera over the table. "
                    + "A YouTube link works too: it's downloaded into the Videos folder first (only download videos you're allowed to).", ui.Label);
                return;
            }
            float aspect = videoFrame.width / (float)videoFrame.height;
            float w = area.width, h = w / aspect;
            if (h > area.height)
            {
                h = area.height;
                w = h * aspect;
            }
            videoRect = new Rect(area.x + (area.width - w) / 2f, area.y + (area.height - h) / 2f, w, h);
            GUI.DrawTexture(videoRect, videoFrame, ScaleMode.StretchToFill, false);

            var layout = VideoLayoutNow;
            foreach (var box in layout.Boxes)
            {
                var rect = BoxRect(box);
                bool selected = box.Role == videoRole;
                Color color = box.Kind == VideoBoxKind.CardArt ? new Color(0.2f, 0.9f, 1f) : box.Kind == VideoBoxKind.Icons ? new Color(1f, 0.85f, 0.2f) : new Color(1f, 0.3f, 0.3f);
                Outline(rect, selected ? Color.white : color, selected ? 3f : 1.5f);
                if (selected) GUI.Label(new Rect(rect.x, rect.yMax + 1, 260, 18), VideoRoles.Describe(box.Role, VideoPlayerLabels()), Hint(ui));
            }

            // Drawing a box: drag on the video while a role is picked on the Layout tab.
            var e = Event.current;
            if (videoTab == 0 && !string.IsNullOrEmpty(videoRole) && videoJob == null)
            {
                if (e.type == EventType.MouseDown && e.button == 0 && videoRect.Contains(e.mousePosition))
                {
                    videoDrawing = true;
                    videoDragFrom = e.mousePosition;
                    videoDragTo = e.mousePosition;
                    e.Use();
                }
                else if (e.type == EventType.MouseDrag && videoDrawing)
                {
                    videoDragTo = e.mousePosition;
                    e.Use();
                }
                else if (e.type == EventType.MouseUp && videoDrawing)
                {
                    videoDrawing = false;
                    var dragStart = videoDragFrom;
                    var dragEnd = e.mousePosition;
                    string role = videoRole;
                    actions.Add(() => PlaceBox(role, dragStart, dragEnd));
                    e.Use();
                }
            }
            if (videoDrawing)
            {
                var drag = Rect.MinMaxRect(Mathf.Min(videoDragFrom.x, videoDragTo.x), Mathf.Min(videoDragFrom.y, videoDragTo.y),
                    Mathf.Max(videoDragFrom.x, videoDragTo.x), Mathf.Max(videoDragFrom.y, videoDragTo.y));
                Outline(drag, Color.white, 2f);
            }
        }

        private Rect BoxRect(VideoBox box)
        {
            return new Rect(videoRect.x + (float)box.X * videoRect.width, videoRect.y + (float)box.Y * videoRect.height,
                (float)box.W * videoRect.width, (float)box.H * videoRect.height);
        }

        private static void Outline(Rect r, Color color, float thickness)
        {
            var old = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(new Rect(r.x, r.y, r.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(r.x, r.yMax - thickness, r.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(r.x, r.y, thickness, r.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(r.xMax - thickness, r.y, thickness, r.height), Texture2D.whiteTexture);
            GUI.color = old;
        }

        private void PlaceBox(string role, Vector2 start, Vector2 end)
        {
            if (videoRect.width <= 0 || videoRect.height <= 0) return;
            float x0 = Mathf.Clamp01((Mathf.Min(start.x, end.x) - videoRect.x) / videoRect.width);
            float y0 = Mathf.Clamp01((Mathf.Min(start.y, end.y) - videoRect.y) / videoRect.height);
            float x1 = Mathf.Clamp01((Mathf.Max(start.x, end.x) - videoRect.x) / videoRect.width);
            float y1 = Mathf.Clamp01((Mathf.Max(start.y, end.y) - videoRect.y) / videoRect.height);
            if (x1 - x0 < 0.004f || y1 - y0 < 0.004f) return;
            var layout = VideoLayoutNow;
            var old = layout.Find(role);
            var kind = old != null ? old.Kind : DefaultKind(role);
            layout.Set(new VideoBox { Role = role, Kind = kind, X = x0, Y = y0, W = x1 - x0, H = y1 - y0, Slots = old != null ? old.Slots : 6 });
            videoLayoutDirty = true;
        }

        private static VideoBoxKind DefaultKind(string role)
        {
            foreach (var kv in VideoRoles.All()) if (kv.Key == role) return kv.Value;
            return VideoBoxKind.Text;
        }

        private string[] VideoPlayerLabels()
        {
            var names = videoSession != null ? videoSession.PlayerNames() : null;
            return new[]
            {
                "Left player" + (names != null && !string.IsNullOrEmpty(videoSession.Names[0]) ? " (" + names[0] + ")" : ""),
                "Right player" + (names != null && !string.IsNullOrEmpty(videoSession.Names[1]) ? " (" + names[1] + ")" : "")
            };
        }

        private void DrawVideoControls(UiStyles ui, Rect area, List<Action> actions)
        {
            GUILayout.BeginArea(area);
            bool can = videoReady && videoJob == null;
            GUI.enabled = can;
            double length = VideoLength;
            float now = (float)VideoTime;
            float picked = GUILayout.HorizontalSlider(now, 0f, Mathf.Max(0.1f, (float)length));
            if (can && Mathf.Abs(picked - now) > 0.05f) actions.Add(() => SeekVideo(picked));
            GUILayout.BeginHorizontal();
            bool playing = videoPlayer != null && videoPlayer.isPlaying;
            if (GUILayout.Button(playing ? "Pause" : "Play", GUILayout.Width(70)))
                actions.Add(() =>
                {
                    if (videoPlayer == null) return;
                    if (videoPlayer.isPlaying) videoPlayer.Pause();
                    else videoPlayer.Play();
                });
            if (GUILayout.Button("-10 s", GUILayout.Width(55))) actions.Add(() => SeekVideo(VideoTime - 10));
            if (GUILayout.Button("-1 s", GUILayout.Width(50))) actions.Add(() => SeekVideo(VideoTime - 1));
            if (GUILayout.Button("+1 s", GUILayout.Width(50))) actions.Add(() => SeekVideo(VideoTime + 1));
            if (GUILayout.Button("+10 s", GUILayout.Width(55))) actions.Add(() => SeekVideo(VideoTime + 10));
            GUI.enabled = true;
            GUILayout.Label(VideoEvent.Clock(VideoTime) + " / " + VideoEvent.Clock(length), ui.Label, GUILayout.Width(140));
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        // ------------------------------------------------------------------ 1. Layout

        private void DrawVideoLayoutTab(UiStyles ui, List<Action> actions)
        {
            GUILayout.Label("Draw a box over each thing the video shows: pick it below, then drag on the video. A layout is drawn once "
                            + "and used for every video of that kind. Left is player 1 (on TCG Live: you, at the bottom).", Hint(ui));
            var layout = VideoLayoutNow;
            GUILayout.BeginHorizontal();
            GUI.enabled = videoJob == null;
            if (GUILayout.Button("<", GUILayout.Width(30))) actions.Add(() => { videoLayoutIndex = (videoLayoutIndex + videoLayouts.Count - 1) % videoLayouts.Count; videoRole = ""; });
            GUILayout.Label(layout.Name + (layout.BuiltIn ? " (built-in)" : ""), ui.Title);
            if (GUILayout.Button(">", GUILayout.Width(30))) actions.Add(() => { videoLayoutIndex = (videoLayoutIndex + 1) % videoLayouts.Count; videoRole = ""; });
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Text on screen:", ui.Label, GUILayout.Width(110));
            int lang = GUILayout.Toolbar(layout.Language == "ja" ? 1 : 0, new[] { "English", "Japanese" });
            if ((lang == 1) != (layout.Language == "ja"))
                actions.Add(() => { layout.Language = lang == 1 ? "ja" : "en"; videoLayoutDirty = true; });
            GUILayout.EndHorizontal();
            if (layout.Find(VideoRoles.Banner) != null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Turn banner, left:", ui.Label, GUILayout.Width(120));
                string t0 = GUILayout.TextField(layout.TurnText[0] ?? "");
                GUILayout.Label("right:", ui.Label, GUILayout.Width(40));
                string t1 = GUILayout.TextField(layout.TurnText[1] ?? "");
                if (t0 != layout.TurnText[0] || t1 != layout.TurnText[1]) actions.Add(() => { layout.TurnText = new[] { t0, t1 }; videoLayoutDirty = true; });
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(4);
            int who = GUILayout.Toolbar(videoRolePlayer, new[] { "Left player", "Right player", "Both" });
            if (who != videoRolePlayer) actions.Add(() => videoRolePlayer = who);
            var labels = VideoPlayerLabels();
            foreach (var kv in VideoRoles.All())
            {
                string role = kv.Key;
                bool both = role == VideoRoles.Stadium || role == VideoRoles.Banner;
                if (videoRolePlayer == 2 ? !both : (both || !role.Contains("." + videoRolePlayer))) continue;
                if (videoRolePlayer < 2 && !RoleBelongsTo(role, videoRolePlayer)) continue;
                var box = layout.Find(role);
                GUILayout.BeginHorizontal();
                bool picked = videoRole == role;
                string label = (box != null ? "■ " : "□ ") + VideoRoles.Describe(role, labels);
                if (GUILayout.Toggle(picked, label, ui.Button, GUILayout.MinWidth(220)) != picked)
                    actions.Add(() => videoRole = picked ? "" : role);
                if (box != null)
                {
                    GUILayout.Label(BoxKindNames[(int)box.Kind], Hint(ui), GUILayout.Width(52));
                    if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        actions.Add(() => { layout.Remove(role); videoLayoutDirty = true; });
                }
                GUILayout.EndHorizontal();
            }

            var chosen = layout.Find(videoRole);
            if (chosen != null)
            {
                GUILayout.Space(4);
                GUILayout.Label("The picked box is read as:", ui.Label);
                int kind = GUILayout.Toolbar((int)chosen.Kind, BoxKindNames);
                if (kind != (int)chosen.Kind) actions.Add(() => { chosen.Kind = (VideoBoxKind)kind; videoLayoutDirty = true; });
                if (chosen.Kind == VideoBoxKind.Icons)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Icon places in the row: " + chosen.Slots, ui.Label, GUILayout.Width(190));
                    if (GUILayout.Button("-", GUILayout.Width(30))) actions.Add(() => { chosen.Slots = Mathf.Max(1, chosen.Slots - 1); videoLayoutDirty = true; });
                    if (GUILayout.Button("+", GUILayout.Width(30))) actions.Add(() => { chosen.Slots = Mathf.Min(12, chosen.Slots + 1); videoLayoutDirty = true; });
                    GUILayout.EndHorizontal();
                }
                GUILayout.Label(KindHelp(chosen.Kind), Hint(ui));
            }
            else if (!string.IsNullOrEmpty(videoRole)) GUILayout.Label("Drag on the video to draw this box.", ui.Good);

            GUILayout.Space(6);
            if (layout.BuiltIn) GUILayout.Label("Built-in layouts can't be changed: changes are kept while the app is open; save a copy to keep them.", Hint(ui));
            else if (videoLayoutDirty) GUILayout.Label("Changed (saved when you leave the screen, or now).", Hint(ui));
            GUILayout.BeginHorizontal();
            if (!layout.BuiltIn && GUILayout.Button("Save", GUILayout.Width(70))) actions.Add(() => SaveVideoLayout(layout));
            videoLayoutNameInput = GUILayout.TextField(videoLayoutNameInput ?? "", GUILayout.MinWidth(120));
            if (GUILayout.Button("Save as a copy", GUILayout.Width(120))) actions.Add(() => SaveVideoLayoutAs(videoLayoutNameInput));
            GUILayout.EndHorizontal();
            if (!layout.BuiltIn)
            {
                if (!videoConfirmDelete)
                {
                    if (GUILayout.Button("Delete this layout", GUILayout.Width(150))) actions.Add(() => videoConfirmDelete = true);
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Yes, delete it", GUILayout.Width(120))) actions.Add(() => { DeleteVideoLayout(); videoConfirmDelete = false; });
                    if (GUILayout.Button("Keep it", GUILayout.Width(90))) actions.Add(() => videoConfirmDelete = false);
                    GUILayout.EndHorizontal();
                }
            }
            GUI.enabled = true;
        }

        private static bool RoleBelongsTo(string role, int player)
        {
            var parts = role.Split('.');
            return parts.Length >= 2 && parts[1] == player.ToString(CultureInfo.InvariantCulture);
        }

        private static string KindHelp(VideoBoxKind kind)
        {
            switch (kind)
            {
                case VideoBoxKind.Text: return "Text: a name read by the OCR (Japanese names are translated).";
                case VideoBoxKind.Number: return "Number: HP (\"90/110\" or \"80\"), a turn counter, Prize cards left as a digit, or a clock.";
                case VideoBoxKind.Icons: return "Icons: a row of Prize card icons; the colourful ones are counted.";
                default: return "Card: a card on the table or the screen, recognised by its artwork (needs card images; draw the box around the whole card).";
            }
        }

        // ------------------------------------------------------------------ 2. Read

        private void DrawVideoReadTab(UiStyles ui, List<Action> actions)
        {
            var session = videoSession;
            if (session == null || !videoReady)
            {
                GUILayout.Label("Open a video first.", ui.Label);
                return;
            }
            GUILayout.Label("The video is read a moment at a time with the layout \"" + VideoLayoutNow.Name + "\". A long video takes a "
                            + "while; you can read a stretch of it (boxes that didn't change aren't read again).", Hint(ui));
            GUI.enabled = videoJob == null;
            int interval = Array.IndexOf(Intervals, session.Interval);
            if (interval < 0) interval = 1;
            int newInterval = GUILayout.Toolbar(interval, IntervalLabels);
            if (newInterval != interval) actions.Add(() => session.Interval = Intervals[newInterval]);
            GUILayout.BeginHorizontal();
            GUILayout.Label("From " + VideoEvent.Clock(session.Start) + " to " + (session.Stop > 0 ? VideoEvent.Clock(session.Stop) : "the end"), ui.Label, GUILayout.Width(170));
            if (GUILayout.Button("From here", GUILayout.Width(85))) actions.Add(() => session.Start = Math.Round(VideoTime, 1));
            if (GUILayout.Button("To here", GUILayout.Width(70))) actions.Add(() => session.Stop = Math.Round(VideoTime, 1));
            if (GUILayout.Button("Whole video", GUILayout.Width(95))) actions.Add(() => { session.Start = 0; session.Stop = 0; });
            GUILayout.EndHorizontal();

            // The decks, so card recognition only looks at their cards.
            if (VideoLayoutNow.Boxes.Any(b => b.Kind == VideoBoxKind.CardArt))
            {
                GUILayout.Label("Card recognition: " + ArtIndexStatus() + ". Picking the decks (if you know them) makes it surer:", Hint(ui));
                for (int p = 0; p < 2; p++)
                {
                    int seat = p;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(p == 0 ? "Left deck:" : "Right deck:", ui.Label, GUILayout.Width(80));
                    if (GUILayout.Button("<", GUILayout.Width(28))) actions.Add(() => session.Decks[seat] = CycleDeck(session.Decks[seat], -1));
                    GUILayout.Label(string.IsNullOrEmpty(session.Decks[p]) ? "(any card)" : session.Decks[p], ui.Label);
                    if (GUILayout.Button(">", GUILayout.Width(28))) actions.Add(() => session.Decks[seat] = CycleDeck(session.Decks[seat], 1));
                    GUILayout.EndHorizontal();
                }
            }

            if (VideoLayoutNow.Language == "ja")
            {
                GUILayout.Label(videoNames.SpeciesCount > 0
                    ? "Japanese Pokémon names: " + videoNames.SpeciesCount + " known."
                    : "Japanese names need PokeAPI's list of Pokémon names (a text file, downloaded once).", Hint(ui));
                if (videoNames.SpeciesCount == 0 && GUILayout.Button(videoNamesDownload != null ? "Downloading..." : "Get the Japanese Pokémon names", GUILayout.Width(260)))
                    actions.Add(() => StartNamesDownload());
            }
            if (!OcrAvailable) GUILayout.Label("Text is read with Windows' built-in OCR, so names, HP and numbers are only read on Windows.", ui.Error);
            GUI.enabled = true;

            GUILayout.Space(4);
            if (videoJob == null)
            {
                GUI.enabled = videoReady;
                if (GUILayout.Button(session.Readings.Count > 0 ? "Read it again" : "Start reading", GUILayout.Height(30))) actions.Add(() => StartVideoReading());
                GUI.enabled = true;
                if (session.Readings.Count > 0)
                    GUILayout.Label(session.Readings.Count + " moments read so far; reading again replaces them (for the stretch read) and finds the events again. "
                                    + "Your tagged events are kept.", Hint(ui));
            }
            else
            {
                var job = videoJob;
                float done = job.Total > 0 ? Mathf.Clamp01(job.Read / (float)job.Total) : 0f;
                GUILayout.Label("Reading: " + job.Read + " of " + job.Total + " moments" + (job.OcrRunning ? " (OCR working...)" : "")
                                + (job.State == 2 ? ", finishing the text." : "."), ui.Label);
                var bar = GUILayoutUtility.GetRect(100, 14, GUILayout.ExpandWidth(true));
                GUI.Box(bar, GUIContent.none);
                GUI.DrawTexture(new Rect(bar.x + 1, bar.y + 1, (bar.width - 2) * done, bar.height - 2), ui.Selected);
                if (GUILayout.Button("Stop", GUILayout.Width(80))) actions.Add(() => job.Cancelled = true);
            }

            if (videoUnknown.Count > 0)
            {
                GUILayout.Space(6);
                GUILayout.Label("Names that weren't recognised", ui.Title);
                GUILayout.Label("Type the card's English name; it's remembered for every video after this.", Hint(ui));
                foreach (var kv in videoUnknown.Take(25))
                {
                    string shown = kv.Key;
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(shown + "  (x" + kv.Value + ")", ui.Label, GUILayout.Width(170));
                    string typed;
                    videoNameFixes.TryGetValue(shown, out typed);
                    string now = GUILayout.TextField(typed ?? "", GUILayout.MinWidth(120));
                    if (now != (typed ?? "")) videoNameFixes[shown] = now;
                    if (GUILayout.Button("Save", GUILayout.Width(55))) actions.Add(() => SaveVideoNameFix(shown, videoNameFixes.ContainsKey(shown) ? videoNameFixes[shown] : ""));
                    GUILayout.EndHorizontal();
                }
            }
        }

        private string CycleDeck(string current, int step)
        {
            var names = new List<string> { "" };
            names.AddRange(decks.Select(d => d.Name));
            int at = Mathf.Max(0, names.IndexOf(current ?? ""));
            return names[(at + step + names.Count) % names.Count];
        }

        // ------------------------------------------------------------------ 3. Events

        private void DrawVideoEventsTab(UiStyles ui, List<Action> actions)
        {
            var session = videoSession;
            if (session == null)
            {
                GUILayout.Label("Open a video first.", ui.Label);
                return;
            }
            var names = session.PlayerNames();
            int unsure = session.Events.Count(x => x.Enabled && (x.IsUnidentifiedAttack || !x.Sure));
            GUILayout.Label(session.Events.Count + " events" + (unsure > 0 ? ", " + unsure + " to check (marked ?)" : "") + ". Click a time to see that moment. "
                            + "Untick an event to leave it out.", Hint(ui));
            GUILayout.BeginHorizontal();
            GUI.enabled = session.Readings.Count > 0 && videoJob == null;
            if (GUILayout.Button("Find the events again", GUILayout.Width(170))) actions.Add(() => RebuildVideoEvents());
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            float listH = Mathf.Max(200f, UiScale.Height * 0.42f);
            videoEventScroll = GUILayout.BeginScrollView(videoEventScroll, GUI.skin.box, GUILayout.Height(listH));
            for (int i = 0; i < session.Events.Count; i++)
            {
                var ev = session.Events[i];
                int index = i;
                GUILayout.BeginHorizontal();
                bool on = GUILayout.Toggle(ev.Enabled, "", GUILayout.Width(18));
                if (on != ev.Enabled) actions.Add(() => ev.Enabled = on);
                if (GUILayout.Button(ev.Setup ? "setup" : VideoEvent.Clock(ev.Time), GUILayout.Width(52))) actions.Add(() => SeekVideo(ev.Time));
                bool check = ev.IsUnidentifiedAttack || !ev.Sure;
                string text = (check ? "? " : "") + (ev.Manual ? "[tag] " : "") + ev.Describe(names).Substring(ev.Describe(names).IndexOf("  ", StringComparison.Ordinal) + 2);
                var style = ev.Kind == VideoEventKind.Turn ? ui.Title : check ? ui.Error : ui.Label;
                GUILayout.Label(text, style);
                if (ev.IsUnidentifiedAttack && GUILayout.Button("Which?", GUILayout.Width(60))) actions.Add(() => videoAttackPick = videoAttackPick == index ? -1 : index);
                if (GUILayout.Button("x", GUILayout.Width(22))) actions.Add(() => { session.Events.Remove(ev); videoAttackPick = -1; });
                GUILayout.EndHorizontal();
                if (videoAttackPick == index && ev.IsUnidentifiedAttack)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(70);
                    foreach (var choice in ev.Choices.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string attack = choice;
                        if (GUILayout.Button(attack)) actions.Add(() => { ev.Attack = attack; ev.Sure = true; videoAttackPick = -1; SaveVideoSession(); });
                    }
                    if (GUILayout.Button("Not an attack")) actions.Add(() => { ev.Kind = VideoEventKind.Counters; ev.Amount = ev.Amount / 10; ev.Sure = true; videoAttackPick = -1; });
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            DrawTagForm(ui, actions, session, names);
        }

        private void DrawTagForm(UiStyles ui, List<Action> actions, VideoSession session, string[] names)
        {
            GUILayout.Space(6);
            GUILayout.Label("Tag a moment (at " + VideoEvent.Clock(VideoTime) + ")", ui.Title);
            GUILayout.Label("Add what the reader can't see: an attack it couldn't tell, an Ability, a Trainer card, a retreat, the end of a turn. "
                            + "Names are the cards' English names.", Hint(ui));
            int kind = GUILayout.SelectionGrid(tagKind, TagKindNames, 4);
            if (kind != tagKind) actions.Add(() => { tagKind = kind; tagError = null; });
            int player = GUILayout.Toolbar(tagPlayer, new[] { names[0], names[1] });
            if (player != tagPlayer) actions.Add(() => tagPlayer = player);
            var k = TagKinds[Mathf.Clamp(tagKind, 0, TagKinds.Length - 1)];
            switch (k)
            {
                case VideoEventKind.Attack:
                    TagField(ui, "Attacking Pokémon:", ref tagCard);
                    TagField(ui, "Attack:", ref tagMove);
                    TagField(ui, "Hit (opponent's Pokémon):", ref tagCard2);
                    TagField(ui, "Damage:", ref tagAmount);
                    break;
                case VideoEventKind.Ability:
                    TagField(ui, "Pokémon:", ref tagCard);
                    TagField(ui, "Ability:", ref tagMove);
                    break;
                case VideoEventKind.Trainer:
                case VideoEventKind.Bench:
                case VideoEventKind.Active:
                case VideoEventKind.KnockOut:
                    TagField(ui, k == VideoEventKind.Trainer ? "Trainer card:" : "Pokémon:", ref tagCard);
                    break;
                case VideoEventKind.Energy:
                    TagField(ui, "Energy card:", ref tagCard);
                    TagField(ui, "To Pokémon:", ref tagCard2);
                    tagSpot = GUILayout.Toolbar(tagSpot, new[] { "Active Spot", "Bench" });
                    break;
                case VideoEventKind.Evolve:
                    TagField(ui, "Pokémon:", ref tagCard);
                    TagField(ui, "Evolved into:", ref tagCard2);
                    tagSpot = GUILayout.Toolbar(tagSpot, new[] { "Active Spot", "Bench" });
                    break;
                case VideoEventKind.Retreat:
                    TagField(ui, "Retreated Pokémon:", ref tagCard);
                    TagField(ui, "New Active Pokémon:", ref tagCard2);
                    break;
                case VideoEventKind.Prizes:
                    TagField(ui, "Prize cards taken:", ref tagAmount);
                    break;
                case VideoEventKind.Turn:
                    TagField(ui, "Turn number (blank: next):", ref tagAmount);
                    break;
                case VideoEventKind.Line:
                    TagField(ui, "Battle log line:", ref tagText);
                    break;
            }
            if (!string.IsNullOrEmpty(tagError)) GUILayout.Label(tagError, ui.Error);
            if (GUILayout.Button("Add at " + VideoEvent.Clock(VideoTime), GUILayout.Width(160))) actions.Add(() => AddTag(session, k));
        }

        private void TagField(UiStyles ui, string label, ref string value)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, ui.Label, GUILayout.Width(170));
            value = GUILayout.TextField(value ?? "");
            GUILayout.EndHorizontal();
        }

        /// <summary>Adds a tagged event at the video's current time (names checked against the pool, attacks against the card).</summary>
        private void AddTag(VideoSession session, VideoEventKind kind)
        {
            tagError = null;
            var e = new VideoEvent { Time = Math.Round(VideoTime, 2), Kind = kind, Player = tagPlayer, Manual = true };
            string card = null, card2 = null;
            bool needsCard = kind == VideoEventKind.Attack || kind == VideoEventKind.Ability || kind == VideoEventKind.Trainer || kind == VideoEventKind.Bench
                             || kind == VideoEventKind.Active || kind == VideoEventKind.KnockOut || kind == VideoEventKind.Energy || kind == VideoEventKind.Evolve
                             || kind == VideoEventKind.Retreat;
            bool needsCard2 = kind == VideoEventKind.Energy || kind == VideoEventKind.Evolve || kind == VideoEventKind.Retreat
                              || (kind == VideoEventKind.Attack && (tagAmount ?? "").Trim().Length > 0);
            double score;
            if (needsCard && (card = videoNames.MatchPool(tagCard ?? "", out score)) == null)
            {
                tagError = "\"" + tagCard + "\" isn't a card in the pool.";
                return;
            }
            if (needsCard2 && (card2 = videoNames.MatchPool(tagCard2 ?? "", out score)) == null)
            {
                tagError = "\"" + tagCard2 + "\" isn't a card in the pool.";
                return;
            }
            e.Card = card ?? "";
            e.Card2 = card2 ?? "";
            e.Spot = tagSpot == 0 ? "Active Spot" : "Bench";
            int amount;
            int.TryParse((tagAmount ?? "").Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out amount);
            switch (kind)
            {
                case VideoEventKind.Attack:
                case VideoEventKind.Ability:
                {
                    string move = MoveName(card, tagMove, kind == VideoEventKind.Attack);
                    if (move == null)
                    {
                        tagError = card + " has no " + (kind == VideoEventKind.Attack ? "attack" : "Ability") + " called \"" + tagMove + "\".";
                        return;
                    }
                    e.Attack = move;
                    e.Amount = Math.Max(0, amount);
                    e.Target = 1 - tagPlayer;
                    break;
                }
                case VideoEventKind.Prizes:
                    if (amount < 1 || amount > 6)
                    {
                        tagError = "Type how many Prize cards (1 to 6).";
                        return;
                    }
                    e.Amount = amount;
                    break;
                case VideoEventKind.Turn:
                    e.Amount = Math.Max(0, amount);
                    break;
                case VideoEventKind.Line:
                    if ((tagText ?? "").Trim().Length == 0)
                    {
                        tagError = "Type the line.";
                        return;
                    }
                    e.Text = tagText.Trim();
                    break;
            }
            session.Events.Add(e);
            session.SortEvents();
            SaveVideoSession();
            tagCard = tagCard2 = tagMove = tagAmount = tagText = "";
        }

        /// <summary>The card's attack (or Ability) of that name, spelled as on the card; null if it has none.</summary>
        private string MoveName(string cardName, string typed, bool attack)
        {
            string want = CardDatabase.LooseName(typed ?? "");
            if (want.Length == 0) return null;
            foreach (var d in db.Cards)
            {
                if (d.Name != cardName) continue;
                if (attack)
                {
                    foreach (var a in d.Attacks) if (CardDatabase.LooseName(a.Name) == want) return a.Name;
                }
                else
                {
                    foreach (var ab in d.Abilities) if (CardDatabase.LooseName(ab.Name) == want) return ab.Name;
                }
            }
            return null;
        }

        // ------------------------------------------------------------------ 4. Study

        private void DrawVideoStudyTab(UiStyles ui, List<Action> actions)
        {
            var session = videoSession;
            if (session == null)
            {
                GUILayout.Label("Open a video first.", ui.Label);
                return;
            }
            GUILayout.Label("The video is studied as a battle log: the AI learns from the plays it shows (attacks and where they "
                            + "went for both players; plays from the hand only when you tagged them, and those don't count as decisions "
                            + "since the hand isn't known). The winner's plays count fully, the loser's half.", Hint(ui));
            for (int p = 0; p < 2; p++)
            {
                int seat = p;
                GUILayout.BeginHorizontal();
                GUILayout.Label(p == 0 ? "Left player:" : "Right player:", ui.Label, GUILayout.Width(95));
                string typed = GUILayout.TextField(session.Names[p] ?? "");
                if (typed != (session.Names[p] ?? "")) actions.Add(() => session.Names[seat] = typed);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("Went first:", ui.Label, GUILayout.Width(95));
            int first = GUILayout.Toolbar(session.FirstPlayer + 1, new[] { "Not known", "Left", "Right" });
            if (first != session.FirstPlayer + 1) actions.Add(() => { session.FirstPlayer = first - 1; RebuildVideoEvents(); });
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Won:", ui.Label, GUILayout.Width(95));
            int winner = GUILayout.Toolbar(session.Winner + 1, new[] { "Not known", "Left", "Right" });
            if (winner != session.Winner + 1) actions.Add(() => session.Winner = winner - 1);
            GUILayout.EndHorizontal();

            GUILayout.Label("The battle log it makes:", ui.Label);
            string log = session.BuildLog();
            videoLogScroll = GUILayout.BeginScrollView(videoLogScroll, GUI.skin.box, GUILayout.Height(Mathf.Max(160f, UiScale.Height * 0.3f)));
            GUILayout.Label(log, Hint(ui));
            GUILayout.EndScrollView();

            bool busy = videoStudyRunning || brain == null || (trainView != null && trainView.Running) || studyRunningView;
            GUI.enabled = !busy && session.Events.Count > 0;
            if (GUILayout.Button(session.Studied ? "Study it again (it counts again)" : "Study this video", GUILayout.Height(30)))
                actions.Add(() => { SaveVideoSession(); StudyVideo(); });
            GUI.enabled = true;
            if (trainView != null && trainView.Running) GUILayout.Label("Videos can be studied once training has finished.", Hint(ui));
            if (!string.IsNullOrEmpty(videoStudyResult)) GUILayout.Label(videoStudyResult, videoStudyRunning ? ui.Label : ui.Good);
        }
    }
}
