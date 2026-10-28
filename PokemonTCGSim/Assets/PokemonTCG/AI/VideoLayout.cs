using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>How a box of the screen is read.</summary>
    public enum VideoBoxKind
    {
        /// <summary>Text read by OCR (a card name, a player's name, the Stadium, a turn banner).</summary>
        Text,
        /// <summary>A number read by OCR (a turn counter, Prize cards left shown as a digit, HP, a clock).</summary>
        Number,
        /// <summary>A row of Prize card icons: the filled ones are counted (Slots says how many places there are).</summary>
        Icons,
        /// <summary>A card on the table or on screen, recognised by its artwork.</summary>
        CardArt
    }

    /// <summary>A box drawn on the video, as fractions of the frame (0 to 1 from the top left), and what it shows.</summary>
    public sealed class VideoBox
    {
        public string Role = "";
        public VideoBoxKind Kind;
        public double X, Y, W, H;
        /// <summary>Icons: the number of icon places in the row.</summary>
        public int Slots = 6;

        public VideoBox Clone()
        {
            return new VideoBox { Role = Role, Kind = Kind, X = X, Y = Y, W = W, H = H, Slots = Slots };
        }
    }

    /// <summary>
    /// The roles a box can have (v77). Player 0 is the player on the left of an overlay (or at the bottom of TCG Live:
    /// you); player 1 is the other one.
    /// </summary>
    public static class VideoRoles
    {
        public const int MaxBench = 8;
        public const string Stadium = "stadium";
        /// <summary>A turn banner ("YOUR TURN" / "OPPONENT'S TURN"): the layout's TurnText says which player each phrase means.</summary>
        public const string Banner = "banner";

        public static string PlayerName(int p) { return "player." + p; }
        public static string Turns(int p) { return "turns." + p; }
        public static string Clock(int p) { return "clock." + p; }
        public static string Prizes(int p) { return "prizes." + p; }
        public static string Active(int p) { return "active." + p; }
        public static string ActiveHp(int p) { return "active." + p + ".hp"; }
        public static string ActiveArt(int p) { return "active." + p + ".art"; }
        public static string Bench(int p, int slot) { return "bench." + p + "." + slot; }
        public static string BenchHp(int p, int slot) { return "bench." + p + "." + slot + ".hp"; }
        public static string BenchArt(int p, int slot) { return "bench." + p + "." + slot + ".art"; }

        /// <summary>Every role, in the order the layout editor lists them, with the kind of box each one is.</summary>
        public static List<KeyValuePair<string, VideoBoxKind>> All()
        {
            var list = new List<KeyValuePair<string, VideoBoxKind>>();
            for (int p = 0; p < 2; p++)
            {
                list.Add(new KeyValuePair<string, VideoBoxKind>(PlayerName(p), VideoBoxKind.Text));
                list.Add(new KeyValuePair<string, VideoBoxKind>(Turns(p), VideoBoxKind.Number));
                list.Add(new KeyValuePair<string, VideoBoxKind>(Clock(p), VideoBoxKind.Number));
                list.Add(new KeyValuePair<string, VideoBoxKind>(Prizes(p), VideoBoxKind.Icons));
                list.Add(new KeyValuePair<string, VideoBoxKind>(Active(p), VideoBoxKind.Text));
                list.Add(new KeyValuePair<string, VideoBoxKind>(ActiveHp(p), VideoBoxKind.Number));
                list.Add(new KeyValuePair<string, VideoBoxKind>(ActiveArt(p), VideoBoxKind.CardArt));
                for (int s = 1; s <= MaxBench; s++)
                {
                    list.Add(new KeyValuePair<string, VideoBoxKind>(Bench(p, s), VideoBoxKind.Text));
                    list.Add(new KeyValuePair<string, VideoBoxKind>(BenchHp(p, s), VideoBoxKind.Number));
                    list.Add(new KeyValuePair<string, VideoBoxKind>(BenchArt(p, s), VideoBoxKind.CardArt));
                }
            }
            list.Add(new KeyValuePair<string, VideoBoxKind>(Stadium, VideoBoxKind.Text));
            list.Add(new KeyValuePair<string, VideoBoxKind>(Banner, VideoBoxKind.Text));
            return list;
        }

        /// <summary>What a role means, for the layout editor ("Left player's Bench 2 HP").</summary>
        public static string Describe(string role, string[] playerLabels)
        {
            var parts = (role ?? "").Split('.');
            if (parts.Length == 1) return role == Stadium ? "Stadium" : role == Banner ? "Turn banner" : role;
            int p;
            if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out p) || p < 0 || p > 1) return role;
            string who = playerLabels != null && playerLabels.Length > p ? playerLabels[p] : "Player " + (p + 1);
            switch (parts[0])
            {
                case "player": return who + ": name";
                case "turns": return who + ": turn counter";
                case "clock": return who + ": clock";
                case "prizes": return who + ": Prize cards left";
                case "active":
                    if (parts.Length == 2) return who + ": Active Pokémon's name";
                    return who + ": Active Pokémon's " + (parts[2] == "hp" ? "HP" : "card");
                case "bench":
                    if (parts.Length < 3) return role;
                    string slot = "Bench " + parts[2];
                    if (parts.Length == 3) return who + ": " + slot + " name";
                    return who + ": " + slot + (parts[3] == "hp" ? " HP" : " card");
            }
            return role;
        }
    }

    /// <summary>
    /// A layout: the boxes drawn once for one kind of video (a stream's overlay, TCG Live, a camera over the table), so
    /// every video of that kind is read the same way (v77). Saved as text in the Videos folder.
    /// </summary>
    public sealed class VideoLayout
    {
        public const string Header = "# Pokemon TCG video layout v1";

        public string Name = "New layout";
        /// <summary>The OCR language: "ja" or "en".</summary>
        public string Language = "en";
        /// <summary>The turn banner's text for each player ("YOUR TURN", "OPPONENT'S TURN"), when the layout has a banner box.</summary>
        public string[] TurnText = { "", "" };
        /// <summary>A built-in layout (it can be copied and changed, not overwritten).</summary>
        public bool BuiltIn;
        public readonly List<VideoBox> Boxes = new List<VideoBox>();

        public VideoBox Find(string role)
        {
            foreach (var b in Boxes) if (b.Role == role) return b;
            return null;
        }

        public void Set(VideoBox box)
        {
            Boxes.RemoveAll(b => b.Role == box.Role);
            Boxes.Add(box);
        }

        public void Remove(string role)
        {
            Boxes.RemoveAll(b => b.Role == role);
        }

        /// <summary>The highest Bench slot the layout has for a player.</summary>
        public int BenchSlots(int p)
        {
            int n = 0;
            for (int s = 1; s <= VideoRoles.MaxBench; s++)
                if (Find(VideoRoles.Bench(p, s)) != null || Find(VideoRoles.BenchArt(p, s)) != null) n = s;
            return n;
        }

        public VideoLayout Clone(string name)
        {
            var copy = new VideoLayout { Name = name ?? Name, Language = Language, TurnText = new[] { TurnText[0], TurnText[1] } };
            foreach (var b in Boxes) copy.Boxes.Add(b.Clone());
            return copy;
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append(Header).Append('\n');
            sb.Append("name\t").Append(Clean(Name)).Append('\n');
            sb.Append("language\t").Append(Clean(Language)).Append('\n');
            sb.Append("turntext\t").Append(Clean(TurnText[0])).Append('\t').Append(Clean(TurnText[1])).Append('\n');
            foreach (var b in Boxes)
            {
                sb.Append("box\t").Append(b.Role).Append('\t').Append(b.Kind.ToString()).Append('\t')
                  .Append(F(b.X)).Append('\t').Append(F(b.Y)).Append('\t').Append(F(b.W)).Append('\t').Append(F(b.H)).Append('\t')
                  .Append(b.Slots.ToString(CultureInfo.InvariantCulture)).Append('\n');
            }
            return sb.ToString();
        }

        private static string F(double v)
        {
            return v.ToString("0.#####", CultureInfo.InvariantCulture);
        }

        private static string Clean(string s)
        {
            return (s ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }

        public static VideoLayout Parse(string text)
        {
            var layout = new VideoLayout();
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                var f = raw.Split('\t');
                switch (f[0])
                {
                    case "name": if (f.Length > 1) layout.Name = f[1]; break;
                    case "language": if (f.Length > 1) layout.Language = f[1]; break;
                    case "turntext":
                        layout.TurnText = new[] { f.Length > 1 ? f[1] : "", f.Length > 2 ? f[2] : "" };
                        break;
                    case "box":
                    {
                        if (f.Length < 7) break;
                        VideoBoxKind kind;
                        if (!Enum.TryParse(f[2], out kind)) break;
                        double x, y, w, h;
                        if (!D(f[3], out x) || !D(f[4], out y) || !D(f[5], out w) || !D(f[6], out h)) break;
                        int slots = 6, parsed;
                        if (f.Length > 7 && int.TryParse(f[7], NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed)) slots = parsed;
                        layout.Set(new VideoBox { Role = f[1], Kind = kind, X = x, Y = y, W = w, H = h, Slots = Math.Max(1, slots) });
                        break;
                    }
                }
            }
            return layout;
        }

        private static bool D(string s, out double v)
        {
            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v);
        }

        // ------------------------------------------------------------------ Built-in layouts

        /// <summary>The built-in layouts, measured on 1920 x 1080 footage.</summary>
        public static List<VideoLayout> BuiltIns()
        {
            return new List<VideoLayout> { ChampionsLeague(), TcgLive() };
        }

        private static VideoBox Px(string role, VideoBoxKind kind, double x, double y, double w, double h)
        {
            return new VideoBox { Role = role, Kind = kind, X = x / 1920.0, Y = y / 1080.0, W = w / 1920.0, H = h / 1080.0 };
        }

        /// <summary>
        /// The Japanese Champions League stream overlay: each player's turn counter and Prize card icons along the top, the
        /// Active Pokémon and 5 Bench rows (name and HP) down each side, the Stadium at the bottom. Left is the player who
        /// went first in the footage it was measured on.
        /// </summary>
        public static VideoLayout ChampionsLeague()
        {
            var l = new VideoLayout { Name = "Champions League overlay (Japanese)", Language = "ja", BuiltIn = true };
            l.Set(Px(VideoRoles.Turns(0), VideoBoxKind.Number, 55, 10, 70, 60));
            l.Set(Px(VideoRoles.Turns(1), VideoBoxKind.Number, 1805, 10, 70, 60));
            l.Set(Px(VideoRoles.PlayerName(0), VideoBoxKind.Text, 170, 15, 240, 45));
            l.Set(Px(VideoRoles.PlayerName(1), VideoBoxKind.Text, 1400, 15, 240, 45));
            var prizes0 = Px(VideoRoles.Prizes(0), VideoBoxKind.Icons, 595, 8, 290, 54);
            prizes0.Slots = 6;
            l.Set(prizes0);
            var prizes1 = Px(VideoRoles.Prizes(1), VideoBoxKind.Icons, 1038, 8, 290, 54);
            prizes1.Slots = 6;
            l.Set(prizes1);
            // Rows: Active at y 627, Bench rows from y 728, 70 pixels apart.
            l.Set(Px(VideoRoles.Active(0), VideoBoxKind.Text, 18, 630, 300, 50));
            l.Set(Px(VideoRoles.ActiveHp(0), VideoBoxKind.Number, 326, 625, 66, 58));
            l.Set(Px(VideoRoles.Active(1), VideoBoxKind.Text, 1535, 630, 300, 50));
            l.Set(Px(VideoRoles.ActiveHp(1), VideoBoxKind.Number, 1838, 625, 66, 58));
            for (int s = 1; s <= 5; s++)
            {
                double y = 728 + (s - 1) * 70;
                l.Set(Px(VideoRoles.Bench(0, s), VideoBoxKind.Text, 18, y, 300, 55));
                l.Set(Px(VideoRoles.BenchHp(0, s), VideoBoxKind.Number, 326, y, 66, 55));
                l.Set(Px(VideoRoles.Bench(1, s), VideoBoxKind.Text, 1535, y, 300, 55));
                l.Set(Px(VideoRoles.BenchHp(1, s), VideoBoxKind.Number, 1838, y, 66, 55));
            }
            l.Set(Px(VideoRoles.Stadium, VideoBoxKind.Text, 805, 1022, 310, 42));
            return l;
        }

        /// <summary>
        /// Pokémon TCG Live (16:9 window or full screen). You (player 0) are at the bottom. Prize cards left and both clocks are
        /// on the right; the turn banner in the middle. Card names on the board are too small to read, so the Active Pokémon
        /// and Bench cards are recognised by their artwork (needs card images) and their HP read from the badge.
        /// </summary>
        public static VideoLayout TcgLive()
        {
            var l = new VideoLayout { Name = "Pokémon TCG Live", Language = "en", BuiltIn = true };
            l.TurnText = new[] { "YOUR TURN", "OPPONENT'S TURN" };
            l.Set(Px(VideoRoles.Banner, VideoBoxKind.Text, 860, 505, 480, 70));
            l.Set(Px(VideoRoles.Prizes(1), VideoBoxKind.Number, 1858, 408, 58, 64));
            l.Set(Px(VideoRoles.Prizes(0), VideoBoxKind.Number, 1858, 488, 58, 68));
            l.Set(Px(VideoRoles.Clock(1), VideoBoxKind.Number, 1852, 372, 68, 30));
            l.Set(Px(VideoRoles.Clock(0), VideoBoxKind.Number, 1852, 552, 68, 30));
            l.Set(Px(VideoRoles.ActiveArt(1), VideoBoxKind.CardArt, 875, 218, 173, 242));
            l.Set(Px(VideoRoles.ActiveHp(1), VideoBoxKind.Number, 985, 218, 50, 32));
            l.Set(Px(VideoRoles.ActiveArt(0), VideoBoxKind.CardArt, 875, 493, 173, 245));
            l.Set(Px(VideoRoles.ActiveHp(0), VideoBoxKind.Number, 985, 493, 50, 32));
            for (int s = 1; s <= 5; s++)
            {
                double x = 655 + (s - 1) * 125;
                l.Set(Px(VideoRoles.BenchArt(1, s), VideoBoxKind.CardArt, x, 52, 107, 153));
                l.Set(Px(VideoRoles.BenchHp(1, s), VideoBoxKind.Number, x + 57, 52, 34, 24));
                l.Set(Px(VideoRoles.BenchArt(0, s), VideoBoxKind.CardArt, x, 752, 107, 153));
                l.Set(Px(VideoRoles.BenchHp(0, s), VideoBoxKind.Number, x + 57, 752, 34, 24));
            }
            return l;
        }
    }
}
