using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>What a card picture in the video was matched to.</summary>
    public sealed class ArtMatch
    {
        public string Key = "";
        public string Name;
        /// <summary>How alike the pictures are (-1 to 1; above about 0.55 is a likely match).</summary>
        public double Score;
        /// <summary>How far ahead of the best other card name it is.</summary>
        public double Margin;
        public bool Sure { get { return Name != null && Score >= CardArtIndex.MatchAt && Margin >= CardArtIndex.MarginAt; } }
    }

    /// <summary>
    /// Recognising physical (or TCG Live) cards in a video by their artwork (v77). Each card image is boiled down to a
    /// small "fingerprint" of its illustration: a 12 x 9 grid of colours and a 16 x 12 grid of brightness, each with its
    /// average taken out, so lighting and camera colour matter less. A box drawn around a card in the video is compared with
    /// every candidate (the decks' cards when known), trying the box a little shifted and scaled, since a card on a table
    /// is never exactly where the box is.
    /// </summary>
    public sealed class CardArtIndex
    {
        public const int ColorW = 12, ColorH = 9, GrayW = 16, GrayH = 12;
        public const int Length = ColorW * ColorH * 3 + GrayW * GrayH;
        /// <summary>Where the illustration is on a modern card, as fractions of the card's width and height.</summary>
        public const double ArtX0 = 0.08, ArtX1 = 0.92, ArtY0 = 0.10, ArtY1 = 0.48;
        public const double MatchAt = 0.55, MarginAt = 0.05;

        private readonly List<string> keys = new List<string>();
        private readonly List<string> names = new List<string>();
        private readonly List<float[]> prints = new List<float[]>();
        private readonly Dictionary<string, int> byKey = new Dictionary<string, int>(StringComparer.Ordinal);

        public int Count { get { return keys.Count; } }

        public bool Contains(string key)
        {
            return key != null && byKey.ContainsKey(key);
        }

        public void Add(string key, string name, float[] print)
        {
            if (string.IsNullOrEmpty(key) || print == null || print.Length != Length) return;
            int at;
            if (byKey.TryGetValue(key, out at))
            {
                names[at] = name;
                prints[at] = print;
                return;
            }
            byKey[key] = keys.Count;
            keys.Add(key);
            names.Add(name);
            prints.Add(print);
        }

        /// <summary>The fingerprint of a whole card picture (upright, the card filling the picture).</summary>
        public static float[] Fingerprint(VideoImage card)
        {
            if (card == null) return null;
            var art = card.CropNormalized(ArtX0, ArtY0, ArtX1 - ArtX0, ArtY1 - ArtY0);
            return FingerprintOfArt(art);
        }

        private static float[] FingerprintOfArt(VideoImage art)
        {
            var print = new float[Length];
            var color = art.Resize(ColorW, ColorH);
            int n = ColorW * ColorH;
            for (int channel = 0; channel < 3; channel++)
            {
                double mean = 0;
                for (int y = 0; y < ColorH; y++)
                    for (int x = 0; x < ColorW; x++)
                        mean += Channel(color, x, y, channel);
                mean /= n;
                for (int y = 0; y < ColorH; y++)
                    for (int x = 0; x < ColorW; x++)
                        print[channel * n + y * ColorW + x] = (float)(Channel(color, x, y, channel) - mean);
            }
            var gray = art.Resize(GrayW, GrayH);
            int offset = n * 3;
            double gm = 0;
            for (int y = 0; y < GrayH; y++)
                for (int x = 0; x < GrayW; x++)
                    gm += gray.Luma(x, y);
            gm /= GrayW * GrayH;
            for (int y = 0; y < GrayH; y++)
                for (int x = 0; x < GrayW; x++)
                    print[offset + y * GrayW + x] = (float)(gray.Luma(x, y) - gm);
            double norm = 0;
            foreach (var v in print) norm += v * v;
            norm = Math.Sqrt(norm);
            if (norm < 1e-6) norm = 1;
            for (int i = 0; i < print.Length; i++) print[i] = (float)(print[i] / norm);
            return print;
        }

        private static int Channel(VideoImage img, int x, int y, int channel)
        {
            return channel == 0 ? img.R(x, y) : channel == 1 ? img.G(x, y) : img.B(x, y);
        }

        public static double Similarity(float[] a, float[] b)
        {
            if (a == null || b == null || a.Length != b.Length) return -1;
            double s = 0;
            for (int i = 0; i < a.Length; i++) s += a[i] * b[i];
            return s;
        }

        /// <summary>
        /// The card in a box of the frame (x, y, w, h as fractions of the frame). allowed: card names to consider (null: all).
        /// The box is also tried shifted by 3% each way and scaled by 6%.
        /// </summary>
        public ArtMatch Match(VideoImage frame, double x, double y, double w, double h, ICollection<string> allowed)
        {
            var result = new ArtMatch();
            if (frame == null || keys.Count == 0) return result;
            var variants = new List<float[]>();
            foreach (double scale in new[] { 1.0, 0.94, 1.06 })
                foreach (double dx in new[] { 0.0, -0.03, 0.03 })
                    foreach (double dy in new[] { 0.0, -0.03, 0.03 })
                    {
                        double vw = w * scale, vh = h * scale;
                        double vx = x + (w - vw) / 2 + dx * w, vy = y + (h - vh) / 2 + dy * h;
                        var card = frame.CropNormalized(vx, vy, vw, vh);
                        if (card.Width < 8 || card.Height < 8) continue;
                        variants.Add(Fingerprint(card));
                    }
            if (variants.Count == 0) return result;
            var bestByName = new Dictionary<string, double>(StringComparer.Ordinal);
            var keyByName = new Dictionary<string, string>(StringComparer.Ordinal);
            for (int i = 0; i < keys.Count; i++)
            {
                if (allowed != null && !allowed.Contains(names[i])) continue;
                double best = -1;
                foreach (var v in variants)
                {
                    double s = Similarity(v, prints[i]);
                    if (s > best) best = s;
                }
                double old;
                if (!bestByName.TryGetValue(names[i], out old) || best > old)
                {
                    bestByName[names[i]] = best;
                    keyByName[names[i]] = keys[i];
                }
            }
            string top = null;
            double topScore = -2, second = -2;
            foreach (var kv in bestByName)
            {
                if (kv.Value > topScore)
                {
                    second = topScore;
                    topScore = kv.Value;
                    top = kv.Key;
                }
                else if (kv.Value > second) second = kv.Value;
            }
            if (top == null) return result;
            result.Name = top;
            result.Key = keyByName[top];
            result.Score = topScore;
            result.Margin = second > -2 ? topScore - second : 1;
            return result;
        }

        // ------------------------------------------------------------------ Saving (so the card images are read only once)

        /// <summary>One line per card: key TAB name TAB stamp TAB the fingerprint (bytes, base64). stamp says which image file it came from.</summary>
        public string Serialize(Func<string, string> stampOf)
        {
            var sb = new StringBuilder();
            sb.Append("# Card art fingerprints v1\n");
            var bytes = new byte[Length];
            for (int i = 0; i < keys.Count; i++)
            {
                var p = prints[i];
                for (int j = 0; j < Length; j++)
                {
                    int q = (int)Math.Round(p[j] * 4 * 127);
                    bytes[j] = (byte)(sbyte)Math.Max(-127, Math.Min(127, q));
                }
                sb.Append(keys[i]).Append('\t').Append(names[i]).Append('\t').Append(stampOf != null ? stampOf(keys[i]) : "")
                  .Append('\t').Append(Convert.ToBase64String(bytes)).Append('\n');
            }
            return sb.ToString();
        }

        /// <summary>Reads saved fingerprints; a line whose stamp no longer matches its image file (stampOf) is skipped.</summary>
        public int Load(string text, Func<string, string> stampOf)
        {
            int n = 0;
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                var f = raw.Split('\t');
                if (f.Length < 4) continue;
                if (stampOf != null && stampOf(f[0]) != f[2]) continue;
                byte[] bytes;
                try { bytes = Convert.FromBase64String(f[3]); }
                catch (FormatException) { continue; }
                if (bytes.Length != Length) continue;
                var p = new float[Length];
                double norm = 0;
                for (int j = 0; j < Length; j++)
                {
                    p[j] = (sbyte)bytes[j];
                    norm += p[j] * (double)p[j];
                }
                norm = Math.Sqrt(norm);
                if (norm < 1e-6) norm = 1;
                for (int j = 0; j < Length; j++) p[j] = (float)(p[j] / norm);
                Add(f[0], f[1], p);
                n++;
            }
            return n;
        }

        public static string Stamp(long size, DateTime written)
        {
            return size.ToString(CultureInfo.InvariantCulture) + "@" + written.ToUniversalTime().Ticks.ToString(CultureInfo.InvariantCulture);
        }
    }
}
