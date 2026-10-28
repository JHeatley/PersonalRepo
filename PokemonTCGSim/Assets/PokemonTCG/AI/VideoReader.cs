using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>What was read from one moment of a video: each box's text, icon count or recognised card (v77).</summary>
    public sealed class FrameReading
    {
        public double Time;
        /// <summary>OCR text by box role.</summary>
        public readonly Dictionary<string, string> Text = new Dictionary<string, string>(StringComparer.Ordinal);
        /// <summary>Filled icons by box role (Prize card icons).</summary>
        public readonly Dictionary<string, int> Counts = new Dictionary<string, int>(StringComparer.Ordinal);
        /// <summary>The card recognised in a card box, by role ("" when the box held no card it could recognise).</summary>
        public readonly Dictionary<string, string> Art = new Dictionary<string, string>(StringComparer.Ordinal);

        public string TextOf(string role)
        {
            string s;
            return Text.TryGetValue(role, out s) ? s : null;
        }

        /// <summary>One line per reading: "r TAB time", then "t/c/a TAB role TAB value" lines (saved with a video study).</summary>
        public void AppendTo(StringBuilder sb)
        {
            sb.Append("r\t").Append(Time.ToString("0.###", CultureInfo.InvariantCulture)).Append('\n');
            foreach (var kv in Text) sb.Append("t\t").Append(kv.Key).Append('\t').Append(Clean(kv.Value)).Append('\n');
            foreach (var kv in Counts) sb.Append("c\t").Append(kv.Key).Append('\t').Append(kv.Value.ToString(CultureInfo.InvariantCulture)).Append('\n');
            foreach (var kv in Art) sb.Append("a\t").Append(kv.Key).Append('\t').Append(Clean(kv.Value)).Append('\n');
        }

        private static string Clean(string s)
        {
            return (s ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }
    }

    /// <summary>A Pokémon (or an empty place) as read at one moment.</summary>
    public sealed class MonReading
    {
        /// <summary>The card name, or null if it couldn't be read.</summary>
        public string Name;
        public string Raw = "";
        public int Hp = -1;
        public int MaxHp = -1;
        /// <summary>The place is empty (the overlay says so, or there's no card and no HP).</summary>
        public bool Empty;
        public bool Known { get { return Empty || Name != null; } }
    }

    /// <summary>One player's side as read at one moment.</summary>
    public sealed class SideReading
    {
        public string Name = "";
        public int Turns = -1;
        public int Clock = -1;
        public int Prizes = -1;
        public MonReading Active = new MonReading();
        public MonReading[] Bench = new MonReading[0];
    }

    /// <summary>The board as read at one moment of the video.</summary>
    public sealed class VideoSnapshot
    {
        public double Time;
        public readonly SideReading[] Sides = { new SideReading(), new SideReading() };
        /// <summary>The Stadium's card name; "" when there's none; null when it couldn't be read (or the layout has no box for it).</summary>
        public string Stadium;
        /// <summary>The player whose turn banner shows, or -1.</summary>
        public int Banner = -1;
        /// <summary>Names read that matched no card (for the "fix a name" list).</summary>
        public readonly List<string> UnknownNames = new List<string>();
    }

    /// <summary>Turns what the boxes read into a board snapshot (v77).</summary>
    public static class SnapshotReader
    {
        public static VideoSnapshot Read(FrameReading r, VideoLayout layout, CardNameReader names)
        {
            var snap = new VideoSnapshot { Time = r.Time };
            for (int p = 0; p < 2; p++)
            {
                var side = snap.Sides[p];
                string text;
                if ((text = r.TextOf(VideoRoles.PlayerName(p))) != null) side.Name = VideoText.Normalize(text);
                if ((text = r.TextOf(VideoRoles.Turns(p))) != null) side.Turns = Range(VideoText.ParseInt(text), 1, 99);
                if ((text = r.TextOf(VideoRoles.Clock(p))) != null) side.Clock = VideoText.ParseClock(text);
                int count;
                if (r.Counts.TryGetValue(VideoRoles.Prizes(p), out count)) side.Prizes = Range(count, 0, 6);
                else if ((text = r.TextOf(VideoRoles.Prizes(p))) != null) side.Prizes = Range(VideoText.ParseInt(text), 0, 6);
                side.Active = Mon(r, layout, names, VideoRoles.Active(p), VideoRoles.ActiveHp(p), VideoRoles.ActiveArt(p), snap);
                int slots = layout.BenchSlots(p);
                side.Bench = new MonReading[slots];
                for (int s = 1; s <= slots; s++)
                    side.Bench[s - 1] = Mon(r, layout, names, VideoRoles.Bench(p, s), VideoRoles.BenchHp(p, s), VideoRoles.BenchArt(p, s), snap);
            }
            string stadium = r.TextOf(VideoRoles.Stadium);
            if (stadium != null)
            {
                string shown = VideoText.AfterLabel(stadium);
                if (VideoText.IsEmptySlot(shown)) snap.Stadium = "";
                else
                {
                    var m = names.Read(shown);
                    snap.Stadium = m.Name;
                    if (m.Name == null) Unknown(snap, shown);
                }
            }
            string banner = r.TextOf(VideoRoles.Banner);
            if (!string.IsNullOrEmpty(banner)) snap.Banner = BannerPlayer(banner, layout);
            return snap;
        }

        private static int Range(int v, int min, int max)
        {
            return v < min || v > max ? -1 : v;
        }

        private static void Unknown(VideoSnapshot snap, string raw)
        {
            string t = (raw ?? "").Trim();
            if (t.Length > 0 && !snap.UnknownNames.Contains(t)) snap.UnknownNames.Add(t);
        }

        private static MonReading Mon(FrameReading r, VideoLayout layout, CardNameReader names, string nameRole, string hpRole, string artRole, VideoSnapshot snap)
        {
            var mon = new MonReading();
            bool hasNameBox = layout.Find(nameRole) != null;
            string text = r.TextOf(nameRole);
            string hpText = r.TextOf(hpRole);
            int hp, max;
            bool hpRead = hpText != null && VideoText.ParseHp(hpText, out hp, out max);
            if (hpRead)
            {
                VideoText.ParseHp(hpText, out hp, out max);
                mon.Hp = hp;
                mon.MaxHp = max;
            }
            if (hasNameBox && text != null)
            {
                mon.Raw = text;
                // "EMPTY" is an empty place; nothing read at all is too, unless an HP shows (then the name was just missed).
                bool blank = VideoText.Normalize(text).Length == 0;
                if (VideoText.IsEmptySlot(text) && (!blank || !hpRead))
                {
                    mon.Empty = true;
                    mon.Hp = -1;
                    mon.MaxHp = -1;
                    return mon;
                }
                if (!blank)
                {
                    var m = names.Read(text);
                    mon.Name = m.Name;
                    if (m.Name == null) Unknown(snap, text);
                }
            }
            // A card recognised by its artwork. One that isn't recognised stays unknown rather than empty: on TCG Live a card
            // shown in the middle of the screen often covers the board for a moment.
            string art;
            if (mon.Name == null && r.Art.TryGetValue(artRole, out art) && !string.IsNullOrEmpty(art)) mon.Name = art;
            if (mon.Name == null && !mon.Empty)
            {
                mon.Hp = -1;
                mon.MaxHp = -1;
            }
            return mon;
        }

        private static int BannerPlayer(string banner, VideoLayout layout)
        {
            string b = VideoText.Normalize(banner).ToUpperInvariant().Replace(" ", "").Replace("’", "'");
            int best = -1, bestDistance = int.MaxValue;
            for (int p = 0; p < 2; p++)
            {
                string want = VideoText.Normalize(layout.TurnText[p] ?? "").ToUpperInvariant().Replace(" ", "").Replace("’", "'");
                if (want.Length == 0) continue;
                int d = b.Contains(want) ? 0 : VideoText.Distance(b, want);
                if (d < bestDistance)
                {
                    bestDistance = d;
                    best = p;
                }
            }
            if (best < 0) return -1;
            string target = VideoText.Normalize(layout.TurnText[best]).Replace(" ", "");
            return bestDistance <= Math.Max(1, target.Length / 4) ? best : -1;
        }
    }
}
