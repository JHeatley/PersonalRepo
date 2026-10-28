using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>A card name the video reader recognised.</summary>
    public sealed class NameMatch
    {
        /// <summary>The card name in the pool, or null when nothing matched well enough.</summary>
        public string Name;
        /// <summary>1 for an exact match, less for a near one.</summary>
        public double Score;
        /// <summary>What the OCR read.</summary>
        public string Raw = "";
        /// <summary>The English name it was translated to (before matching the pool).</summary>
        public string English = "";
    }

    /// <summary>
    /// Turns a name the OCR read into a card name of the pool (v77). English names are matched to the pool directly (a
    /// letter or two off is fine). Japanese names are translated first: the Pokémon's species from a Japanese / English
    /// table (PokeAPI's species names, downloaded once), with the TCG's prefixes and suffixes ("かがやく" Radiant, "メガ"
    /// Mega, "ガラル" Galarian, "ロケット団の" Team Rocket's, "ex", "V", "VSTAR"...); Trainer names come from a small built-in
    /// table and from your own corrections (the video screen saves every name you fix).
    /// </summary>
    public sealed class CardNameReader
    {
        private readonly Dictionary<string, string> poolByLoose = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly List<KeyValuePair<string, string>> poolLoose = new List<KeyValuePair<string, string>>();
        /// <summary>KatakanaKey(species) to the English species name.</summary>
        private readonly Dictionary<string, string> species = new Dictionary<string, string>(StringComparer.Ordinal);
        /// <summary>KatakanaKey(whole name) to an English card name: Trainers, special Pokémon names and your corrections.</summary>
        private readonly Dictionary<string, string> names = new Dictionary<string, string>(StringComparer.Ordinal);

        public CardNameReader(IEnumerable<string> poolNames)
        {
            foreach (var n in poolNames ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrEmpty(n)) continue;
                string loose = CardDatabase.LooseName(n);
                if (loose.Length == 0 || poolByLoose.ContainsKey(loose)) continue;
                poolByLoose[loose] = n;
                poolLoose.Add(new KeyValuePair<string, string>(loose, n));
            }
            LoadTable(BuiltInNames);
        }

        public static CardNameReader ForDatabase(CardDatabase db)
        {
            return new CardNameReader(db.Cards.Select(d => d.Name).Distinct());
        }

        public int SpeciesCount { get { return species.Count; } }
        public int NameCount { get { return names.Count; } }

        public void AddSpecies(string japanese, string english)
        {
            string key = VideoText.KatakanaKey(japanese);
            if (key.Length == 0 || string.IsNullOrEmpty(english)) return;
            species[key] = english.Trim();
        }

        /// <summary>A whole name: what the screen shows, and the card name it means (your corrections win over everything).</summary>
        public void AddName(string shown, string cardName)
        {
            string key = VideoText.KatakanaKey(shown);
            if (key.Length == 0 || string.IsNullOrEmpty(cardName)) return;
            names[key] = cardName.Trim();
        }

        /// <summary>Reads "shown TAB card name" lines (lines starting with # are comments). Returns how many were read.</summary>
        public int LoadTable(string text)
        {
            int n = 0;
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                int tab = raw.IndexOf('\t');
                if (tab <= 0) continue;
                AddName(raw.Substring(0, tab), raw.Substring(tab + 1));
                n++;
            }
            return n;
        }

        /// <summary>
        /// Reads PokeAPI's pokemon_species_names.csv (species id, language id, name, genus). The English and Japanese language
        /// ids are found from Bulbasaur's rows (English "Bulbasaur", Japanese in katakana), so the file's numbering doesn't matter.
        /// Returns how many Japanese names were added.
        /// </summary>
        public int LoadPokeApiSpecies(string csv)
        {
            var rows = new List<string[]>();
            foreach (var line in (csv ?? "").Replace("\r", "").Split('\n'))
            {
                if (line.Length == 0) continue;
                var f = SplitCsv(line);
                if (f.Count < 3) continue;
                rows.Add(f.ToArray());
            }
            int englishLang = -1;
            var japaneseLangs = new HashSet<int>();
            foreach (var r in rows)
            {
                int id, lang;
                if (!int.TryParse(r[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out id) || id != 1) continue;
                if (!int.TryParse(r[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out lang)) continue;
                if (r[2] == "Bulbasaur" && englishLang < 0) englishLang = lang;
                if (r[2].Length > 0 && r[2].All(c => VideoText.IsKatakana(c))) japaneseLangs.Add(lang);
            }
            if (englishLang < 0) englishLang = 9;
            if (japaneseLangs.Count == 0)
            {
                japaneseLangs.Add(1);
                japaneseLangs.Add(11);
            }
            var english = new Dictionary<int, string>();
            foreach (var r in rows)
            {
                int id, lang;
                if (!int.TryParse(r[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) continue;
                if (!int.TryParse(r[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out lang)) continue;
                if (lang == englishLang) english[id] = r[2];
            }
            int added = 0;
            foreach (var r in rows)
            {
                int id, lang;
                if (!int.TryParse(r[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out id)) continue;
                if (!int.TryParse(r[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out lang)) continue;
                string en;
                if (!japaneseLangs.Contains(lang) || !english.TryGetValue(id, out en)) continue;
                AddSpecies(r[2], CardSpelling(en));
                added++;
            }
            return added;
        }

        /// <summary>PokeAPI spells a few names differently from the cards.</summary>
        private static string CardSpelling(string en)
        {
            return (en ?? "").Replace('’', '\'');
        }

        private static List<string> SplitCsv(string line)
        {
            var fields = new List<string>();
            var sb = new StringBuilder();
            bool quoted = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (quoted)
                {
                    if (c == '"')
                    {
                        if (i + 1 < line.Length && line[i + 1] == '"')
                        {
                            sb.Append('"');
                            i++;
                        }
                        else quoted = false;
                    }
                    else sb.Append(c);
                }
                else if (c == '"') quoted = true;
                else if (c == ',')
                {
                    fields.Add(sb.ToString());
                    sb.Length = 0;
                }
                else sb.Append(c);
            }
            fields.Add(sb.ToString());
            return fields;
        }

        // ------------------------------------------------------------------ Reading a name

        /// <summary>The card a name read from the screen means (Name null when nothing in the pool matches well enough).</summary>
        public NameMatch Read(string shown)
        {
            var result = new NameMatch { Raw = shown ?? "" };
            string text = VideoText.Normalize(shown);
            if (VideoText.IsEmptySlot(text)) return result;
            // A whole-name entry (your corrections, Trainers) first.
            string key = VideoText.KatakanaKey(text);
            string exact;
            if (names.TryGetValue(key, out exact))
            {
                result.English = exact;
                double s0;
                result.Name = MatchPool(exact, out s0);
                result.Score = result.Name != null ? s0 : 0;
                return result;
            }
            string english = VideoText.HasJapanese(text) ? Translate(text) : StripNumbers(text);
            if (english == null)
            {
                // Not a Pokémon: a Trainer name close to one in the table.
                string near = Nearest(names, key, 1);
                if (near == null) return result;
                english = near;
            }
            result.English = english;
            double score;
            result.Name = MatchPool(english, out score);
            result.Score = result.Name != null ? score : 0;
            return result;
        }

        /// <summary>Drops HP numbers and other stray digits the box may hold ("Makuhita 80").</summary>
        private static string StripNumbers(string s)
        {
            var parts = s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Where(p => !p.All(c => char.IsDigit(c) || c == '/')).ToArray();
            return string.Join(" ", parts);
        }

        private static readonly string[] Suffixes = { "VSTAR", "VMAX", "V-UNION", "VUNION", "BREAK", "LV.X", "LVX", "GX", "EX", "ex", "V" };
        private static readonly KeyValuePair<string, string>[] Prefixes =
        {
            new KeyValuePair<string, string>("カガヤク", "Radiant "),
            new KeyValuePair<string, string>("メガ", "Mega "),
            new KeyValuePair<string, string>("ガラル", "Galarian "),
            new KeyValuePair<string, string>("アローラ", "Alolan "),
            new KeyValuePair<string, string>("ヒスイ", "Hisuian "),
            new KeyValuePair<string, string>("パルデア", "Paldean "),
            new KeyValuePair<string, string>("イチゲキ", "Single Strike "),
            new KeyValuePair<string, string>("レンゲキ", "Rapid Strike "),
            new KeyValuePair<string, string>("コクバ", "Shadow Rider "),
            new KeyValuePair<string, string>("ハクバ", "Ice Rider "),
            new KeyValuePair<string, string>("アカツキ", "Bloodmoon ")
        };
        private static readonly KeyValuePair<string, string>[] Masks =
        {
            new KeyValuePair<string, string>("ミドリノメン", "Teal Mask "),
            new KeyValuePair<string, string>("イドノメン", "Wellspring Mask "),
            new KeyValuePair<string, string>("カマドノメン", "Hearthflame Mask "),
            new KeyValuePair<string, string>("イシズエノメン", "Cornerstone Mask ")
        };
        private static readonly KeyValuePair<string, string>[] Owners =
        {
            new KeyValuePair<string, string>("ロケット団", "Team Rocket's "),
            new KeyValuePair<string, string>("マグマ団", "Team Magma's "),
            new KeyValuePair<string, string>("アクア団", "Team Aqua's "),
            new KeyValuePair<string, string>("ナンジャモ", "Iono's "),
            new KeyValuePair<string, string>("ホップ", "Hop's "),
            new KeyValuePair<string, string>("N", "N's "),
            new KeyValuePair<string, string>("リーリエ", "Lillie's "),
            new KeyValuePair<string, string>("マリィ", "Marnie's "),
            new KeyValuePair<string, string>("ペパー", "Arven's "),
            new KeyValuePair<string, string>("シロナ", "Cynthia's "),
            new KeyValuePair<string, string>("ヒビキ", "Ethan's "),
            new KeyValuePair<string, string>("カスミ", "Misty's "),
            new KeyValuePair<string, string>("タケシ", "Brock's "),
            new KeyValuePair<string, string>("エリカ", "Erika's "),
            new KeyValuePair<string, string>("ダイゴ", "Steven's ")
        };

        /// <summary>A Japanese Pokémon name in English, or null when its species isn't known.</summary>
        public string Translate(string japanese)
        {
            string text = VideoText.Normalize(japanese).Replace(" ", "");
            // The suffix: the Latin letters at the end ("ex", "VSTAR"; "Xex" is Mega Charizard X ex).
            int end = text.Length;
            while (end > 0 && (text[end - 1] < 128) && text[end - 1] != 'ー') end--;
            string latin = text.Substring(end);
            string core = text.Substring(0, end);
            string suffix = "", form = "";
            foreach (var s in Suffixes)
            {
                if (!latin.EndsWith(s, StringComparison.OrdinalIgnoreCase)) continue;
                // "ex" and "EX" are different eras; the pool match doesn't care about case.
                suffix = " " + s;
                latin = latin.Substring(0, latin.Length - s.Length);
                break;
            }
            latin = latin.Trim('-', '.', ' ');
            if (latin == "X" || latin == "Y") form = " " + latin;
            else if (latin.Length > 0 && suffix.Length == 0) suffix = " " + latin;

            string owner = "";
            int of = core.IndexOf('の');
            if (of > 0)
            {
                string who = VideoText.KatakanaKey(core.Substring(0, of));
                foreach (var o in Owners)
                {
                    if (VideoText.KatakanaKey(o.Key) != who) continue;
                    owner = o.Value;
                    core = core.Substring(of + 1);
                    break;
                }
            }
            string coreKey = VideoText.KatakanaKey(core);
            string mask = "";
            foreach (var m in Masks)
            {
                // "オーガポン みどりのめん" (the mask after the name) or before it.
                if (coreKey.EndsWith(m.Key, StringComparison.Ordinal) && coreKey.Length > m.Key.Length)
                {
                    mask = m.Value;
                    coreKey = coreKey.Substring(0, coreKey.Length - m.Key.Length);
                    break;
                }
                if (coreKey.StartsWith(m.Key, StringComparison.Ordinal) && coreKey.Length > m.Key.Length)
                {
                    mask = m.Value;
                    coreKey = coreKey.Substring(m.Key.Length);
                    break;
                }
            }
            string speciesName = Species(coreKey);
            string prefix = "";
            if (speciesName == null)
            {
                // "メガルカリオ": the species after a prefix (tried after the whole name, since Meganium starts with メガ too).
                foreach (var p in Prefixes)
                {
                    if (!coreKey.StartsWith(p.Key, StringComparison.Ordinal) || coreKey.Length <= p.Key.Length) continue;
                    string rest = Species(coreKey.Substring(p.Key.Length));
                    if (rest == null) continue;
                    prefix = p.Value;
                    speciesName = rest;
                    break;
                }
            }
            if (speciesName == null) return null;
            return (prefix + owner + mask + speciesName + form + suffix).Trim();
        }

        private string Species(string key)
        {
            if (key.Length == 0) return null;
            string en;
            if (species.TryGetValue(key, out en)) return en;
            return Nearest(species, key, 0);
        }

        /// <summary>The value whose key is nearest to this one (an OCR slip or two, by length), or null if none is close or two tie.</summary>
        private static string Nearest(Dictionary<string, string> table, string key, int extraSlack)
        {
            if (key.Length < 3) return null;
            int allowed = (key.Length >= 8 ? 2 : 1) + extraSlack;
            if (key.Length < 4) allowed = Math.Min(allowed, 1);
            int best = int.MaxValue;
            string bestValue = null;
            bool tie = false;
            foreach (var kv in table)
            {
                if (Math.Abs(kv.Key.Length - key.Length) > allowed) continue;
                int d = VideoText.Distance(kv.Key, key);
                if (d > allowed) continue;
                if (d < best)
                {
                    best = d;
                    bestValue = kv.Value;
                    tie = false;
                }
                else if (d == best && kv.Value != bestValue) tie = true;
            }
            return tie ? null : bestValue;
        }

        /// <summary>The pool's name for an English name (exact, or a letter or two off). score: 1 exact, less for near.</summary>
        public string MatchPool(string english, out double score)
        {
            score = 0;
            string loose = CardDatabase.LooseName(english);
            if (loose.Length == 0) return null;
            string name;
            if (poolByLoose.TryGetValue(loose, out name))
            {
                score = 1;
                return name;
            }
            int allowed = loose.Length >= 12 ? 3 : loose.Length >= 7 ? 2 : loose.Length >= 4 ? 1 : 0;
            if (allowed == 0) return null;
            int best = int.MaxValue;
            string bestName = null;
            bool tie = false;
            foreach (var kv in poolLoose)
            {
                if (Math.Abs(kv.Key.Length - loose.Length) > allowed) continue;
                int d = VideoText.Distance(kv.Key, loose);
                if (d > allowed) continue;
                if (d < best)
                {
                    best = d;
                    bestName = kv.Value;
                    tie = false;
                }
                else if (d == best) tie = true;
            }
            if (bestName == null || tie) return null;
            score = 1.0 - best / (double)Math.Max(1, loose.Length);
            return bestName;
        }

        /// <summary>
        /// Trainer cards, Stadiums and special Pokémon names as Japanese overlays show them (the species table covers the
        /// rest of the Pokémon). Your corrections are added on top.
        /// </summary>
        public const string BuiltInNames =
            "# Stadiums\n" +
            "活力の森\tForest of Vitality\n" +
            "頂への雪道\tPath to the Peak\n" +
            "ジャミングタワー\tJamming Tower\n" +
            "ボウルタウン\tArtazon\n" +
            "テーブルシティ\tMesagoza\n" +
            "ゼロの大空洞\tArea Zero Underdepths\n" +
            "グラビティマウンテン\tGravity Mountain\n" +
            "崩れたスタジアム\tCollapsed Stadium\n" +
            "ポケストップ\tPokéStop\n" +
            "シンオウの神殿\tTemple of Sinnoh\n" +
            "ロストシティ\tLost City\n" +
            "トレーニングコート\tTraining Court\n" +
            "ビーチコート\tBeach Court\n" +
            "スパイクタウン\tSpikemuth\n" +
            "# Trainers\n" +
            "ボスの指令\tBoss's Orders\n" +
            "ナンジャモ\tIono\n" +
            "ペパー\tArven\n" +
            "博士の研究\tProfessor's Research\n" +
            "ジャッジマン\tJudge\n" +
            "ふしぎなアメ\tRare Candy\n" +
            "ネストボール\tNest Ball\n" +
            "ハイパーボール\tUltra Ball\n" +
            "なかよしポフィン\tBuddy-Buddy Poffin\n" +
            "夜のタンカ\tNight Stretcher\n" +
            "大地の器\tEarthen Vessel\n" +
            "ポケモンいれかえ\tSwitch\n" +
            "スーパーエネルギー回収\tSuperior Energy Retrieval\n" +
            "エネルギー回収\tEnergy Retrieval\n" +
            "森の封印石\tForest Seal Stone\n" +
            "ポケギア3.0\tPokégear 3.0\n" +
            "アカマツ\tCrispin\n" +
            "ジニア\tJacq\n" +
            "ネモ\tNemona\n" +
            "クラベル\tClavell\n" +
            "リーリエの決心\tLillie's Determination\n" +
            "ミモザ\tMiriam\n" +
            "カイ\tIrida\n" +
            "セレナ\tSerena\n" +
            "マリィ\tMarnie\n" +
            "クララ\tKlara\n" +
            "セイボリー\tAvery\n" +
            "シロナの覇気\tCynthia's Ambition\n";
    }
}
