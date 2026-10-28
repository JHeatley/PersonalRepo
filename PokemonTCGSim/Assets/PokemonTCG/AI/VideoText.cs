using System;
using System.Globalization;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Cleaning up what the OCR read (v77): full-width letters and digits, half-width katakana, letters read as digits and
    /// the other way round, katakana read as look-alike kanji, and the numbers the overlays show (HP "90/110", turn
    /// counters, clocks "19:21").
    /// </summary>
    public static class VideoText
    {
        /// <summary>Full-width ASCII to ASCII, half-width katakana to full-width, the many dashes to "ー" between kana, spaces tidied.</summary>
        public static string Normalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string t = s;
            try
            {
                t = s.Normalize(NormalizationForm.FormKC);
            }
            catch (Exception)
            {
                // Some runtimes lack the normalization tables: do the part that matters by hand.
                var sb0 = new StringBuilder(s.Length);
                foreach (char c in s) sb0.Append(c >= '！' && c <= '～' ? (char)(c - 0xFEE0) : c == '　' ? ' ' : c);
                t = sb0.ToString();
            }
            var sb = new StringBuilder(t.Length);
            for (int i = 0; i < t.Length; i++)
            {
                char c = t[i];
                if (IsDash(c))
                {
                    // A dash after a kana is the long vowel mark.
                    char before = sb.Length > 0 ? sb[sb.Length - 1] : ' ';
                    sb.Append(IsKana(before) ? 'ー' : '-');
                    continue;
                }
                if (char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0 && sb[sb.Length - 1] != ' ') sb.Append(' ');
                    continue;
                }
                sb.Append(c);
            }
            return sb.ToString().Trim();
        }

        private static bool IsDash(char c)
        {
            return c == '-' || c == '‐' || c == '‑' || c == '–' || c == '—' || c == '―' || c == '−' || c == 'ｰ';
        }

        public static bool IsKana(char c)
        {
            return (c >= 'ぁ' && c <= 'ゖ') || (c >= 'ァ' && c <= 'ヺ') || c == 'ー';
        }

        public static bool IsKatakana(char c)
        {
            return (c >= 'ァ' && c <= 'ヺ') || c == 'ー';
        }

        public static bool HasJapanese(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            foreach (char c in s)
                if (IsKana(c) || (c >= '一' && c <= '鿿')) return true;
            return false;
        }

        /// <summary>
        /// For comparing katakana names: hiragana to katakana, kanji that look like katakana to the katakana (一 ー, 二 ニ, 口 ロ,
        /// 力 カ, 工 エ, 卜 ト, 八 ハ, 夕 タ, 才 オ, 匕 ヒ), and no spaces or dots.
        /// </summary>
        public static string KatakanaKey(string s)
        {
            s = Normalize(s);
            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c >= 'ぁ' && c <= 'ゖ') c = (char)(c + 0x60);
                switch (c)
                {
                    case '一': c = 'ー'; break;
                    case '二': c = 'ニ'; break;
                    case '口': c = 'ロ'; break;
                    case '力': c = 'カ'; break;
                    case '工': c = 'エ'; break;
                    case '卜': c = 'ト'; break;
                    case '八': c = 'ハ'; break;
                    case '夕': c = 'タ'; break;
                    case '才': c = 'オ'; break;
                    case '匕': c = 'ヒ'; break;
                    case '-': c = 'ー'; break;
                }
                if (c == ' ' || c == '・' || c == '･' || c == '.' || c == '·') continue;
                sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>An empty overlay slot: nothing read, "EMPTY", or just dashes.</summary>
        public static bool IsEmptySlot(string s)
        {
            string t = Normalize(s).Trim().Trim('-', 'ー', '_', '.', ' ');
            if (t.Length == 0) return true;
            string upper = t.ToUpperInvariant().Replace(" ", "");
            return upper == "EMPTY" || upper == "EMPTV" || upper == "EMPIY" || upper == "NONE";
        }

        /// <summary>Digits as the OCR often misreads them in a number (O 0, l I | 1, S 5, B 8, Z 2, g 9).</summary>
        private static char DigitLike(char c)
        {
            switch (c)
            {
                case 'O': case 'o': return '0';
                case 'l': case 'I': case '|': return '1';
                case 'S': return '5';
                case 'B': return '8';
                case 'Z': return '2';
                default: return c;
            }
        }

        /// <summary>
        /// The digits of a text that should be a number, look-alike letters fixed inside tokens that have a digit (a word like
        /// "TURN" is left out, so its letters aren't read as digits). The separator ('/' or ':') is kept once between digits.
        /// </summary>
        private static string Digits(string s, char separator)
        {
            s = Normalize(s);
            var sb = new StringBuilder();
            foreach (var token in s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                bool hasDigit = false;
                foreach (char c in token) if (c >= '0' && c <= '9') hasDigit = true;
                bool word = !hasDigit && token.Length > 2;
                foreach (char raw in token)
                {
                    char c = word ? raw : DigitLike(raw);
                    if (c >= '0' && c <= '9')
                    {
                        if (!word) sb.Append(c);
                    }
                    else if (IsSeparator(c, separator) && sb.Length > 0 && sb[sb.Length - 1] != separator) sb.Append(separator);
                }
            }
            return sb.ToString().Trim(separator);
        }

        private static bool IsSeparator(char c, char separator)
        {
            if (separator == '/') return c == '/' || c == '\\';
            if (separator == ':') return c == ':' || c == '.' || c == ';';
            return false;
        }

        /// <summary>The first whole number in the text (look-alike letters fixed inside it), or -1. Words around it are ignored.</summary>
        public static int ParseInt(string s)
        {
            foreach (var token in Normalize(s).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
            {
                bool hasDigit = false;
                foreach (char c in token) if (c >= '0' && c <= '9') hasDigit = true;
                if (!hasDigit && token.Length > 2) continue;
                var sb = new StringBuilder();
                foreach (char raw in token)
                {
                    char c = DigitLike(raw);
                    if (c >= '0' && c <= '9') sb.Append(c);
                    else if (sb.Length > 0) break;
                }
                int n;
                if (sb.Length > 0 && sb.Length <= 6 && int.TryParse(sb.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return n;
            }
            return -1;
        }

        /// <summary>
        /// HP as shown: "310/310", "90 /110" (current and max), or "80" (current only: max -1). The slash is sometimes lost
        /// ("90110"): then it's split where both halves are HP-like (multiples of 10, current not above max). False if it isn't HP.
        /// </summary>
        public static bool ParseHp(string s, out int hp, out int max)
        {
            hp = -1;
            max = -1;
            string d = Digits(s, '/');
            if (d.Length == 0) return false;
            int slash = d.IndexOf('/');
            if (slash > 0)
            {
                int a, b;
                if (int.TryParse(d.Substring(0, slash), NumberStyles.Integer, CultureInfo.InvariantCulture, out a)
                    && int.TryParse(d.Substring(slash + 1).Replace("/", ""), NumberStyles.Integer, CultureInfo.InvariantCulture, out b)
                    && HpLike(a) && HpLike(b) && b > 0 && a <= b)
                {
                    hp = a;
                    max = b;
                    return true;
                }
                d = d.Replace("/", "");
            }
            if (d.Length >= 4)
            {
                for (int cut = 1; cut < d.Length; cut++)
                {
                    int a, b;
                    if (!int.TryParse(d.Substring(0, cut), NumberStyles.Integer, CultureInfo.InvariantCulture, out a)) continue;
                    if (!int.TryParse(d.Substring(cut), NumberStyles.Integer, CultureInfo.InvariantCulture, out b)) continue;
                    if (d[cut] == '0' || !HpLike(a) || !HpLike(b) || b == 0 || a > b) continue;
                    hp = a;
                    max = b;
                    return true;
                }
            }
            int one;
            if (d.Length <= 3 && int.TryParse(d, NumberStyles.Integer, CultureInfo.InvariantCulture, out one) && HpLike(one))
            {
                hp = one;
                return true;
            }
            return false;
        }

        private static bool HpLike(int n)
        {
            return n >= 0 && n <= 500 && n % 10 == 0;
        }

        /// <summary>A clock "19:21" (or "1921") as seconds, or -1.</summary>
        public static int ParseClock(string s)
        {
            string d = Digits(s, ':');
            int colon = d.IndexOf(':');
            int minutes, seconds;
            if (colon > 0)
            {
                if (!int.TryParse(d.Substring(0, colon), NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes)) return -1;
                string rest = d.Substring(colon + 1).Replace(":", "");
                if (rest.Length > 2) rest = rest.Substring(0, 2);
                if (!int.TryParse(rest, NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds)) return -1;
            }
            else
            {
                if (d.Length < 3 || d.Length > 4) return -1;
                if (!int.TryParse(d.Substring(0, d.Length - 2), NumberStyles.Integer, CultureInfo.InvariantCulture, out minutes)) return -1;
                if (!int.TryParse(d.Substring(d.Length - 2), NumberStyles.Integer, CultureInfo.InvariantCulture, out seconds)) return -1;
            }
            if (seconds > 59 || minutes > 120) return -1;
            return minutes * 60 + seconds;
        }

        /// <summary>The part after a label ("スタジアム:活力の森", "Stadium: Artazon"), or the whole text when there's no label.</summary>
        public static string AfterLabel(string s)
        {
            s = Normalize(s);
            int colon = s.IndexOfAny(new[] { ':', '：' });
            if (colon >= 0 && colon < s.Length - 1) return s.Substring(colon + 1).Trim();
            return s.Trim();
        }

        /// <summary>Edit distance between two strings (for matching names the OCR got a little wrong).</summary>
        public static int Distance(string a, string b)
        {
            a = a ?? "";
            b = b ?? "";
            if (a.Length == 0) return b.Length;
            if (b.Length == 0) return a.Length;
            var prev = new int[b.Length + 1];
            var cur = new int[b.Length + 1];
            for (int j = 0; j <= b.Length; j++) prev[j] = j;
            for (int i = 1; i <= a.Length; i++)
            {
                cur[0] = i;
                for (int j = 1; j <= b.Length; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    cur[j] = Math.Min(Math.Min(cur[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
                }
                var swap = prev;
                prev = cur;
                cur = swap;
            }
            return prev[b.Length];
        }
    }
}
