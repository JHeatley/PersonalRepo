using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokemonTCG.Core
{
    /// <summary>
    /// v176: turns English text the game shows into another language (Japanese) at the moment it's drawn. Screens use templates
    /// taken from the UI code ("Saved as \"{0}\" in your decks." → "「{0}」としてデッキに保存しました。"); the placeholders are
    /// translated too. Text the rules engine writes (prompts, the battle log) stays English, but the Pokémon card names, attack
    /// names and Ability names in it are swapped for the official Japanese ones the name tables have.
    /// </summary>
    public sealed class Translator
    {
        private sealed class Template
        {
            public Regex Pattern;
            public string Target;
            public int[] Order;
            public string Longest;
            public int LiteralLength;
        }

        private readonly Dictionary<string, string> exact = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly List<Template> templates = new List<Template>();
        private readonly Dictionary<string, string> cache = new Dictionary<string, string>(StringComparer.Ordinal);
        private const int CacheLimit = 20000;

        private readonly Dictionary<string, string> cardNames = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> attacks = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, string> abilities = new Dictionary<string, string>(StringComparer.Ordinal);
        private Regex nameRegex, moveRegex;

        /// <summary>English to Japanese words in Pokémon card names (prefixes, owners, forms), for names the tables don't have.</summary>
        private readonly List<KeyValuePair<string, string>> nameWords = new List<KeyValuePair<string, string>>();
        private readonly Dictionary<string, string> species = new Dictionary<string, string>(StringComparer.Ordinal);
        private Regex speciesRegex;

        public int TemplateCount { get { return templates.Count + exact.Count; } }

        // ------------------------------------------------------------------ Loading

        /// <summary>
        /// UI templates, one per line: English, a tab, the translation. {0}, {1}... are the parts the code fills in (the translation
        /// may reorder them); \n and \t are written as escapes. Lines starting with # are comments.
        /// </summary>
        public void LoadTemplates(string text)
        {
            if (string.IsNullOrEmpty(text)) return;
            foreach (var raw in text.Split('\n'))
            {
                var line = raw.TrimEnd('\r');
                if (line.Length == 0 || line[0] == '#') continue;
                int tab = line.IndexOf('\t');
                if (tab <= 0) continue;
                string en = Unescape(line.Substring(0, tab)), ja = Unescape(line.Substring(tab + 1));
                if (ja.Length == 0) continue;
                AddTemplate(en, ja);
            }
            templates.Sort((a, b) => b.LiteralLength.CompareTo(a.LiteralLength));
            cache.Clear();
        }

        public void AddTemplate(string en, string ja)
        {
            var parts = Regex.Split(en, @"(\{\d+\})");
            if (parts.Length == 1)
            {
                exact[en.Replace("{{", "{").Replace("}}", "}")] = ja.Replace("{{", "{").Replace("}}", "}");
                return;
            }
            var sb = new StringBuilder("^");
            var order = new List<int>();
            string longest = "";
            int literal = 0;
            foreach (var p in parts)
            {
                var m = Regex.Match(p, @"^\{(\d+)\}$");
                if (m.Success)
                {
                    order.Add(int.Parse(m.Groups[1].Value));
                    sb.Append("(.*?)");
                }
                else
                {
                    string lit = p.Replace("{{", "{").Replace("}}", "}");
                    sb.Append(Regex.Escape(lit));
                    literal += lit.Trim().Length;
                    if (lit.Length > longest.Length) longest = lit;
                }
            }
            if (literal == 0) return; // "{0}" or "{0}{1}" would match anything
            sb.Append("$");
            templates.Add(new Template
            {
                Pattern = new Regex(sb.ToString(), RegexOptions.Singleline | RegexOptions.CultureInvariant),
                Target = ja.Replace("{{", "\u0001").Replace("}}", "\u0002"),
                Order = order.ToArray(),
                Longest = longest,
                LiteralLength = literal
            });
        }

        /// <summary>The name tables: Pokémon species, whole card names, attack names, Ability names and card-name words.</summary>
        public void LoadNames(IDictionary<string, string> speciesNames, IDictionary<string, string> cards,
                              IDictionary<string, string> attackNames, IDictionary<string, string> abilityNames,
                              IEnumerable<KeyValuePair<string, string>> words)
        {
            Copy(speciesNames, species);
            Copy(cards, cardNames);
            Copy(attackNames, attacks);
            Copy(abilityNames, abilities);
            if (words != null) nameWords.AddRange(words.OrderByDescending(w => w.Key.Length));
            speciesRegex = WordRegex(species.Keys);
            nameRegex = null;
            moveRegex = WordRegex(attacks.Keys.Concat(abilities.Keys).Distinct());
            cache.Clear();
        }

        /// <summary>names_ja.txt: "species|card|attack|ability|word", a tab, the English, a tab, the Japanese (# starts a comment).</summary>
        public void LoadNameTable(string text)
        {
            var sp = new Dictionary<string, string>();
            var cards = new Dictionary<string, string>();
            var atk = new Dictionary<string, string>();
            var ab = new Dictionary<string, string>();
            var words = new List<KeyValuePair<string, string>>();
            foreach (var raw in (text ?? "").Split('\n'))
            {
                var line = raw.TrimEnd('\r');
                if (line.Length == 0 || line[0] == '#') continue;
                var f = line.Split('\t');
                if (f.Length < 3 || f[1].Length == 0 || f[2].Length == 0) continue;
                switch (f[0])
                {
                    case "species": sp[f[1]] = f[2]; break;
                    case "card": cards[f[1]] = f[2]; break;
                    case "attack": atk[f[1]] = f[2]; break;
                    case "ability": ab[f[1]] = f[2]; break;
                    case "word": words.Add(new KeyValuePair<string, string>(f[1], f[2])); break;
                }
            }
            LoadNames(sp, cards, atk, ab, words);
        }

        /// <summary>The Pokémon card names to look for in text (the pool's names); built once, longest first.</summary>
        public void SetKnownCardNames(IEnumerable<string> pokemonCardNames)
        {
            var all = new HashSet<string>(pokemonCardNames.Where(n => !string.IsNullOrEmpty(n)), StringComparer.Ordinal);
            foreach (var n in species.Keys) all.Add(n);
            foreach (var n in all.ToList())
                if (!cardNames.ContainsKey(n))
                {
                    var built = BuildCardName(n);
                    if (built != n) cardNames[n] = built;
                }
            nameRegex = WordRegex(cardNames.Keys);
            cache.Clear();
        }

        private static void Copy(IDictionary<string, string> from, Dictionary<string, string> to)
        {
            if (from == null) return;
            foreach (var kv in from) if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value)) to[kv.Key] = kv.Value;
        }

        private static Regex WordRegex(IEnumerable<string> words)
        {
            var list = words.Where(w => w.Length >= 3).OrderByDescending(w => w.Length).Select(Regex.Escape).ToList();
            if (list.Count == 0) return null;
            return new Regex(@"(?<![\p{L}\p{N}])(" + string.Join("|", list.ToArray()) + @")(?![\p{L}\p{N}])", RegexOptions.CultureInvariant);
        }

        private static string Unescape(string s)
        {
            if (s.IndexOf('\\') < 0) return s;
            var sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '\\' && i + 1 < s.Length)
                {
                    char c = s[++i];
                    sb.Append(c == 'n' ? '\n' : c == 't' ? '\t' : c);
                }
                else sb.Append(s[i]);
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------ Names

        /// <summary>The Japanese name of a Pokémon card (official when known, otherwise built from the species name), or the English name.</summary>
        public string CardName(string english)
        {
            if (string.IsNullOrEmpty(english)) return english;
            string ja;
            if (cardNames.TryGetValue(english, out ja)) return ja;
            return BuildCardName(english);
        }

        public string AttackName(string english)
        {
            string ja;
            return english != null && attacks.TryGetValue(english, out ja) ? ja : english;
        }

        public string AbilityName(string english)
        {
            string ja;
            return english != null && abilities.TryGetValue(english, out ja) ? ja : english;
        }

        /// <summary>"Erika's Vileplume ex" → "エリカのラフレシアex": the species and the known words swapped, the spaces between Japanese parts dropped.</summary>
        private string BuildCardName(string english)
        {
            if (speciesRegex == null || !speciesRegex.IsMatch(english)) return english;
            string s = speciesRegex.Replace(english, m => species[m.Value]);
            foreach (var w in nameWords)
                if (s.IndexOf(w.Key, StringComparison.Ordinal) >= 0) s = Regex.Replace(s, @"(?<![\p{L}])" + Regex.Escape(w.Key) + @"(?![\p{Ll}])", w.Value.Replace("$", "$$"));
            // No spaces in a Japanese card name (リザードン ex → リザードンex, メガリザードン X ex → メガリザードンXex), and no hyphen
            // before GX or EX (ゼクロム-GX → ゼクロムGX), the way Japanese cards print names.
            if (Regex.IsMatch(s, @"[\p{IsKatakana}\p{IsHiragana}\p{IsCJKUnifiedIdeographs}]"))
            {
                s = s.Replace(" ", "");
                s = Regex.Replace(s, @"-(?=(GX|EX)\b)", "");
            }
            return s;
        }

        /// <summary>Swaps the Pokémon card names, attack names and Ability names in a piece of English text.</summary>
        public string Names(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string s = text;
            if (nameRegex != null) s = nameRegex.Replace(s, m => cardNames[m.Value]);
            if (moveRegex != null) s = moveRegex.Replace(s, m => MoveIn(s, m));
            return s;
        }

        /// <summary>An attack or Ability name, swapped only where text names an attack or Ability ("Attack: X", "used X", "'s X", the whole text).</summary>
        private string MoveIn(string text, Match m)
        {
            string ja;
            if (!attacks.TryGetValue(m.Value, out ja) && !abilities.TryGetValue(m.Value, out ja)) return m.Value;
            if (m.Index == 0 && m.Length == text.Length) return ja;
            string before = text.Substring(0, m.Index);
            if (Regex.IsMatch(before, @"(Attack: |Ability: |Ability |used |uses |Use |use |'s |’s |\(|\[|""|「|の)$")) return ja;
            if (Regex.IsMatch(before, @"[•·] ?$")) return ja; // a list line: "• Tackle 10"
            return m.Value;
        }

        // ------------------------------------------------------------------ Text

        /// <summary>The text in the other language: a screen template if one fits, otherwise the English with the names swapped.</summary>
        public string Translate(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string done;
            if (cache.TryGetValue(text, out done)) return done;
            done = TranslateUncached(text, 0);
            if (cache.Count >= CacheLimit) cache.Clear();
            cache[text] = done;
            return done;
        }

        private string TranslateUncached(string text, int depth)
        {
            string ja;
            if (exact.TryGetValue(text, out ja)) return ja;
            // Spaces around a translated piece are kept (" The AI builds its own deck" in a toggle).
            string trimmed = text.Trim();
            if (trimmed.Length != text.Length && trimmed.Length > 0 && exact.TryGetValue(trimmed, out ja))
                return text.Substring(0, text.IndexOf(trimmed, StringComparison.Ordinal)) + ja + text.Substring(text.IndexOf(trimmed, StringComparison.Ordinal) + trimmed.Length);
            if (depth < 3)
            {
                foreach (var t in templates)
                {
                    if (t.Longest.Length > 0 && text.IndexOf(t.Longest, StringComparison.Ordinal) < 0) continue;
                    var m = t.Pattern.Match(text);
                    if (!m.Success) continue;
                    var args = new string[t.Order.Length == 0 ? 0 : t.Order.Max() + 1];
                    for (int i = 0; i < t.Order.Length; i++) args[t.Order[i]] = TranslateUncached(m.Groups[i + 1].Value, depth + 1);
                    return Fill(t.Target, args);
                }
                // Several lines: each one on its own.
                if (text.IndexOf('\n') >= 0)
                {
                    var lines = text.Split('\n');
                    if (lines.Length > 1)
                        return string.Join("\n", lines.Select(l => l.Length == 0 ? l : TranslateUncached(l, depth + 1)).ToArray());
                }
            }
            return Names(text);
        }

        private static string Fill(string target, string[] args)
        {
            var s = Regex.Replace(target, @"\{(\d+)\}", m =>
            {
                int i = int.Parse(m.Groups[1].Value);
                return i < args.Length ? args[i] ?? "" : m.Value;
            });
            return s.Replace("\u0001", "{").Replace("\u0002", "}");
        }
    }
}
