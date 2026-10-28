using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// One video being studied (v77): which video and layout, what was read at each moment, the events found from that and
    /// the ones you tagged, the players' names, who went first and who won. Saved as text next to the video's name in the
    /// Videos folder, so you can come back to it. BuildLog writes it as a TCG Live battle log, which LogStudy studies.
    /// </summary>
    public sealed class VideoSession
    {
        public const string Header = "# Pokemon TCG video study v1";
        /// <summary>The first line of a log written from a video (TcgLiveLog.FromVideo).</summary>
        public const string LogHeader = "# Pokemon TCG game from a video";

        public string VideoPath = "";
        public string LayoutName = "";
        /// <summary>Seconds between the moments read.</summary>
        public double Interval = 1.0;
        /// <summary>Where reading starts and stops (seconds; Stop 0 = the end).</summary>
        public double Start, Stop;
        public string[] Names = { "", "" };
        /// <summary>-1 unknown (the reader may work it out).</summary>
        public int FirstPlayer = -1;
        /// <summary>-1 unknown or not shown.</summary>
        public int Winner = -1;
        /// <summary>The decks (saved deck names) each player used, when you know them: card recognition only looks at their cards.</summary>
        public string[] Decks = { "", "" };
        /// <summary>The video has been studied (studying again would count it twice).</summary>
        public bool Studied;
        public readonly List<FrameReading> Readings = new List<FrameReading>();
        public readonly List<VideoEvent> Events = new List<VideoEvent>();

        public string Title
        {
            get { return string.IsNullOrEmpty(VideoPath) ? "Video" : Path.GetFileNameWithoutExtension(VideoPath); }
        }

        /// <summary>The players' names for the log (the ones read from the screen if you didn't type any; never the same twice).</summary>
        public string[] PlayerNames()
        {
            var names = new string[2];
            for (int p = 0; p < 2; p++)
            {
                string n = (Names[p] ?? "").Trim();
                names[p] = n.Length > 0 ? n : (p == 0 ? "Left player" : "Right player");
            }
            if (names[0] == names[1]) names[1] += " 2";
            return names;
        }

        /// <summary>The name each player's box showed most often (for filling in the names).</summary>
        public string[] NamesOnScreen()
        {
            var result = new[] { "", "" };
            for (int p = 0; p < 2; p++)
            {
                var counts = new Dictionary<string, int>(StringComparer.Ordinal);
                foreach (var r in Readings)
                {
                    string t = r.TextOf(VideoRoles.PlayerName(p));
                    if (string.IsNullOrEmpty(t)) continue;
                    t = VideoText.Normalize(t).Replace("'s", "").Trim();
                    if (t.Length < 2) continue;
                    int c;
                    counts.TryGetValue(t, out c);
                    counts[t] = c + 1;
                }
                if (counts.Count > 0) result[p] = counts.OrderByDescending(kv => kv.Value).First().Key;
            }
            return result;
        }

        /// <summary>Reads the snapshots again from the saved readings and finds the events (the ones you tagged are kept).</summary>
        public List<VideoSnapshot> Rebuild(VideoLayout layout, CardNameReader names, CardDatabase db, VideoTimelineOptions options)
        {
            var snapshots = Readings.OrderBy(r => r.Time).Select(r => SnapshotReader.Read(r, layout, names)).ToList();
            var opts = options ?? new VideoTimelineOptions();
            opts.FirstPlayer = FirstPlayer;
            int first;
            var found = VideoTimeline.Build(snapshots, db, opts, out first);
            if (FirstPlayer < 0 && first >= 0) FirstPlayer = first;
            var manual = Events.Where(x => x.Manual).ToList();
            Events.Clear();
            Events.AddRange(found);
            Events.AddRange(manual);
            SortEvents();
            if (Winner < 0)
                foreach (var e in Events)
                    if (e.Kind == VideoEventKind.Win && e.Enabled) Winner = e.Player;
            return snapshots;
        }

        /// <summary>Setup events first, then by time (events at the same moment keep their order).</summary>
        public void SortEvents()
        {
            var setup = Events.Where(e => e.Setup).ToList();
            var rest = Events.Where(e => !e.Setup).Select((e, i) => new KeyValuePair<int, VideoEvent>(i, e))
                .OrderBy(kv => kv.Value.Time).ThenBy(kv => kv.Key).Select(kv => kv.Value).ToList();
            Events.Clear();
            Events.AddRange(setup);
            Events.AddRange(rest);
        }

        /// <summary>Card names the reader couldn't match, with how often each was read (most common first).</summary>
        public List<KeyValuePair<string, int>> UnknownNames(VideoLayout layout, CardNameReader names)
        {
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var r in Readings)
            {
                var snap = SnapshotReader.Read(r, layout, names);
                foreach (var n in snap.UnknownNames)
                {
                    int c;
                    counts.TryGetValue(n, out c);
                    counts[n] = c + 1;
                }
            }
            return counts.OrderByDescending(kv => kv.Value).ToList();
        }

        // ------------------------------------------------------------------ The battle log

        /// <summary>The video as a TCG Live battle log (what LogStudy reads).</summary>
        public string BuildLog()
        {
            var names = PlayerNames();
            var sb = new StringBuilder();
            sb.Append(LogHeader).Append(": ").Append(Title).Append('\n');
            sb.Append("Setup\n");
            sb.Append(names[0]).Append(" drew 7 cards for the opening hand.\n");
            sb.Append(names[1]).Append(" drew 7 cards for the opening hand.\n");
            if (FirstPlayer >= 0 && FirstPlayer < 2) sb.Append(names[FirstPlayer]).Append(" decided to go first.\n");
            bool won = false;
            foreach (var e in Events)
            {
                if (!e.Enabled) continue;
                if (e.Kind == VideoEventKind.Turn) sb.Append('\n');
                if (e.Kind == VideoEventKind.Win) won = true;
                foreach (var line in e.Lines(names)) sb.Append(line).Append('\n');
            }
            if (!won && Winner >= 0 && Winner < 2) sb.Append(names[Winner]).Append(" wins.\n");
            return sb.ToString();
        }

        // ------------------------------------------------------------------ Saving

        public string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(Header).Append('\n');
            Line(sb, "video", VideoPath);
            Line(sb, "layout", LayoutName);
            Line(sb, "interval", Interval.ToString("0.###", CultureInfo.InvariantCulture));
            Line(sb, "range", Start.ToString("0.###", CultureInfo.InvariantCulture), Stop.ToString("0.###", CultureInfo.InvariantCulture));
            Line(sb, "names", Names[0], Names[1]);
            Line(sb, "decks", Decks[0], Decks[1]);
            Line(sb, "first", FirstPlayer.ToString(CultureInfo.InvariantCulture));
            Line(sb, "winner", Winner.ToString(CultureInfo.InvariantCulture));
            Line(sb, "studied", Studied ? "1" : "0");
            foreach (var r in Readings) r.AppendTo(sb);
            foreach (var e in Events) e.AppendTo(sb);
            return sb.ToString();
        }

        private static void Line(StringBuilder sb, string tag, params string[] fields)
        {
            sb.Append(tag);
            foreach (var f in fields) sb.Append('\t').Append((f ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' '));
            sb.Append('\n');
        }

        public static VideoSession Parse(string text)
        {
            var s = new VideoSession();
            FrameReading reading = null;
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                var f = raw.Split('\t');
                string a = f.Length > 1 ? f[1] : "", b = f.Length > 2 ? f[2] : "";
                switch (f[0])
                {
                    case "video": s.VideoPath = a; break;
                    case "layout": s.LayoutName = a; break;
                    case "interval": s.Interval = Math.Max(0.1, Dbl(a, 1)); break;
                    case "range":
                        s.Start = Math.Max(0, Dbl(a, 0));
                        s.Stop = Math.Max(0, Dbl(b, 0));
                        break;
                    case "names": s.Names = new[] { a, b }; break;
                    case "decks": s.Decks = new[] { a, b }; break;
                    case "first": s.FirstPlayer = (int)Dbl(a, -1); break;
                    case "winner": s.Winner = (int)Dbl(a, -1); break;
                    case "studied": s.Studied = a == "1"; break;
                    case "r":
                        reading = new FrameReading { Time = Dbl(a, 0) };
                        s.Readings.Add(reading);
                        break;
                    case "t": if (reading != null && a.Length > 0) reading.Text[a] = b; break;
                    case "c": if (reading != null && a.Length > 0) reading.Counts[a] = (int)Dbl(b, 0); break;
                    case "a": if (reading != null && a.Length > 0) reading.Art[a] = b; break;
                    case "e":
                    {
                        var e = VideoEvent.Parse(f);
                        if (e != null) s.Events.Add(e);
                        break;
                    }
                }
            }
            return s;
        }

        private static double Dbl(string s, double fallback)
        {
            double v;
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v) ? v : fallback;
        }
    }
}
