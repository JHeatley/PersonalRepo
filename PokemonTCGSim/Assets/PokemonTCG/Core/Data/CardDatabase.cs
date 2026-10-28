using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PokemonTCG.Core
{
    // Plain serializable shapes for card data files. Field names match the JSON.
    // Unity's JsonUtility fills these in the Unity layer; any JSON library works for a headless build.

    [Serializable]
    public class CardDataFile
    {
        public string format;
        public CardDataDto[] cards;
    }

    [Serializable]
    public class CardDataDto
    {
        public string set;
        public string number;
        public string name;
        /// <summary>"Pokemon", "Trainer" or "Energy".</summary>
        public string kind;
        public string regulationMark;
        public string text;

        // Pokémon
        /// <summary>"Basic", "Stage1" or "Stage2".</summary>
        public string stage;
        public string evolvesFrom;
        public int hp;
        public string type;
        public string weakness;
        public string resistance;
        /// <summary>EX-era cards with 2 Weaknesses or 2 Resistances (v158): the second type.</summary>
        public string weakness2;
        public string resistance2;
        public int retreat;
        public bool ex;
        public bool megaEx;
        public bool tera;
        /// <summary>"Ancient", "Future" (Pokémon and Trainers), "Baby" (Neo-era Baby Pokémon with the Baby Rule), "Single Strike", "Rapid Strike", "Fusion Strike".</summary>
        public string[] tags;
        /// <summary>True for an older printing that is legal because of a reprint.</summary>
        public bool reprintLegal;
        /// <summary>Older rule-box mechanic: "Pokémon-ex", "EX", "M EX", "GX", "TAG TEAM", "V", "VMAX", "VSTAR", "Radiant", "LEGEND", "LV.X", "BREAK".</summary>
        public string mechanic;
        /// <summary>A second type (Delta Species, LEGEND cards).</summary>
        public string secondType;
        /// <summary>Resistance amount when it isn't 30 (older cards: 20). 0 = 30.</summary>
        public int resistanceAmount;
        /// <summary>Weakness "+N" instead of x2 (Uxie: 20). 0 = x2.</summary>
        public int weaknessAmount;
        /// <summary>The original card came out before Black & White: Unlimited format only.</summary>
        public bool unlimitedOnly;
        /// <summary>v136: "You may have as many of this card in your deck as you like" (the Arceus of the DP era).</summary>
        public bool anyNumber;
        /// <summary>v139: "You may have up to 4 Basic Pokémon cards in your deck with Unown in their names" (Neo Discovery's Unown).</summary>
        public bool unownRule;
        public bool onePerDeck;
        /// <summary>With unlimitedOnly: a pre-Black &amp; White card made legal in Expanded by a reprint (v132: PlusPower). It stays an old Trainer.</summary>
        public bool expandedByReprint;
        /// <summary>LEGEND halves: "top" or "bottom".</summary>
        public string legendHalf;
        /// <summary>XY-era Ancient Trait (v98): "θ Stop", "θ Double", "θ Max" (the ones the engine knows; others are scripted).</summary>
        public string ancientTrait;
        public AbilityDto[] abilities;
        public AttackDto[] attacks;

        // Trainer
        /// <summary>"Item", "Supporter", "Stadium" or "Tool".</summary>
        public string trainerType;
        public bool aceSpec;
        /// <summary>Antique Fossil Items played as Basic Pokémon (hp and abilities are read too).</summary>
        public bool fossil;
        /// <summary>Snorlax Doll: like a fossil, but put into play only during setup (hp is read too).</summary>
        public bool doll;
        /// <summary>Unidentified Fossil (SIT, v50): played as a Basic Pokémon like an Antique Fossil, but Special Conditions affect it and it gives Prize cards.</summary>
        public bool fossilPlain;
        /// <summary>Rare Fossil (DAA, v64): like an Antique Fossil (no Special Conditions), but its Knock Out gives a Prize card.</summary>
        public bool fossilPrize;
        /// <summary>Lillie's Poké Doll (CEC, v68): like Unidentified Fossil, but no Prize cards and it goes to the bottom of the deck instead of the discard pile.</summary>
        public bool pokeDoll;
        /// <summary>Robo Substitute (v102): played as a Pokémon like an Antique Fossil, Special Conditions affect it, no Prize for it.</summary>
        public bool robo;

        // Energy
        public bool basicEnergy;
        public string provides;
    }

    /// <summary>Alternate printings (other arts and reprints) of cards in the pool. Resources/CardPrintings/printings.json.</summary>
    [Serializable]
    public class PrintingsFile
    {
        public PrintingCardDto[] cards;
    }

    [Serializable]
    public class PrintingCardDto
    {
        /// <summary>The card in the pool, e.g. "TWM-130".</summary>
        public string key;
        /// <summary>Every international printing, including the card itself.</summary>
        public PrintDto[] prints;
    }

    [Serializable]
    public class PrintDto
    {
        public string set;
        public string number;
        public string setName;
    }

    [Serializable]
    public class AttackDto
    {
        public string name;
        public string cost;
        public int damage;
        public string damageSuffix;
        public string text;
        /// <summary>A VSTAR Power attack (v49).</summary>
        public bool vstar;
    }

    [Serializable]
    public class AbilityDto
    {
        public string name;
        public string text;
        /// <summary>"Pokémon Power", "Poké-Power" or "Poké-Body" for older cards; empty = Ability.</summary>
        public string kind;
        /// <summary>A VSTAR Power Ability (v49), also on a Pokémon Tool (Sky Seal Stone).</summary>
        public bool vstar;
    }

    public sealed class CardDatabase
    {
        private readonly Dictionary<string, CardDef> byKey = new Dictionary<string, CardDef>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, List<CardDef>> byName = new Dictionary<string, List<CardDef>>(StringComparer.OrdinalIgnoreCase);
        /// <summary>Main printing key -> every printing of that card (main printing first).</summary>
        private readonly Dictionary<string, List<CardDef>> printings = new Dictionary<string, List<CardDef>>(StringComparer.OrdinalIgnoreCase);

        /// <summary>Every printing, alternate arts included.</summary>
        public IEnumerable<CardDef> All { get { return byKey.Values; } }
        /// <summary>One entry per card (main printings only), e.g. for the deck builder's card pool.</summary>
        public IEnumerable<CardDef> Cards { get { return byKey.Values.Where(d => !d.IsAlternatePrint); } }

        public static string MakeKey(string set, string number)
        {
            return (set ?? "").Trim().ToUpperInvariant() + "-" + NormalizeNumber(number);
        }

        private static string NormalizeNumber(string number)
        {
            number = (number ?? "").Trim();
            int n;
            if (int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) return n.ToString(CultureInfo.InvariantCulture);
            // A letter prefix and a number (v49): "GG01" and "GG1" (Crown Zenith's Galarian Gallery), "TG05" and "TG5" are
            // the same card. A letter after the number ("68a") stays as it is.
            int digits = 0;
            while (digits < number.Length && char.IsLetter(number[digits])) digits++;
            if (digits > 0 && digits < number.Length && digits <= 3
                && int.TryParse(number.Substring(digits), NumberStyles.None, CultureInfo.InvariantCulture, out n))
                return number.Substring(0, digits).ToUpperInvariant() + n.ToString(CultureInfo.InvariantCulture);
            return number.ToUpperInvariant();
        }

        public void Add(CardDef def)
        {
            byKey[def.Key] = def;
            looseNames = null;
            List<CardDef> list;
            if (!byName.TryGetValue(def.Name, out list))
            {
                list = new List<CardDef>();
                byName[def.Name] = list;
            }
            list.Add(def);
        }

        public void AddAll(CardDataFile file)
        {
            if (file == null || file.cards == null) return;
            var added = new List<CardDef>();
            foreach (var dto in file.cards)
            {
                var d = FromDto(dto);
                Add(d);
                added.Add(d);
            }
            EvolutionLines.Learn(added);
        }

        /// <summary>Adds alternate printings for cards already loaded. Printings already in the pool as their own card are skipped.</summary>
        public void AddPrintings(PrintingsFile file)
        {
            if (file == null || file.cards == null) return;
            foreach (var c in file.cards)
            {
                if (c == null || c.prints == null) continue;
                var main = Get(c.key);
                if (main == null) continue;
                foreach (var p in c.prints)
                {
                    if (p == null || string.IsNullOrEmpty(p.set) || string.IsNullOrEmpty(p.number)) continue;
                    string key = MakeKey(p.set, p.number);
                    if (string.Equals(key, main.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(main.PrintSetName)) main.PrintSetName = p.setName ?? "";
                        continue;
                    }
                    if (byKey.ContainsKey(key)) continue;
                    var alt = main.CopyAsPrinting(key, (p.set ?? "").Trim().ToUpperInvariant(), NormalizeNumber(p.number), p.setName);
                    byKey[key] = alt;
                    byName[alt.Name].Add(alt);
                    PrintingList(main).Add(alt);
                }
            }
        }

        /// <summary>Every printing of a card, main printing first (just the card itself if it has no others).</summary>
        public List<CardDef> PrintingsOf(CardDef card)
        {
            var main = Get(card.BaseKey) ?? card;
            return new List<CardDef>(PrintingList(main));
        }

        /// <summary>The main printing of any printing.</summary>
        public CardDef MainPrinting(CardDef card)
        {
            return Get(card.BaseKey) ?? card;
        }

        private List<CardDef> PrintingList(CardDef main)
        {
            List<CardDef> list;
            if (!printings.TryGetValue(main.Key, out list))
            {
                list = new List<CardDef> { main };
                printings[main.Key] = list;
            }
            return list;
        }

        public CardDef Get(string key)
        {
            CardDef d;
            return byKey.TryGetValue(key, out d) ? d : null;
        }

        public CardDef Get(string set, string number)
        {
            return Get(MakeKey(set, number));
        }

        public CardDef GetByName(string name)
        {
            List<CardDef> list;
            return byName.TryGetValue(name, out list) && list.Count > 0 ? FirstOfName(list) : null;
        }

        /// <summary>
        /// The card a bare name means. Neo Genesis's Special Darkness Energy and Metal Energy share their names with the basic
        /// Energy (v138): a deck list line with just the name is the basic Energy.
        /// </summary>
        private static CardDef FirstOfName(List<CardDef> list)
        {
            foreach (var d in list)
                if (d.IsEnergy && d.IsBasicEnergy) return d;
            return list[0];
        }

        /// <summary>Card names with accents, punctuation and spaces dropped -> the card (built on first use; v69).</summary>
        private Dictionary<string, CardDef> looseNames;

        /// <summary>
        /// A card by its name written loosely, the way people type a deck list: no accent ("Poke Pad" is Poké Pad), no
        /// apostrophe or a curly one ("Professors Research"), any case and spacing. Null if no card has that name.
        /// </summary>
        public CardDef GetByLooseName(string name)
        {
            string loose = LooseName(name);
            if (loose.Length == 0) return null;
            if (looseNames == null)
            {
                var map = new Dictionary<string, CardDef>();
                foreach (var kv in byName)
                {
                    if (kv.Value.Count == 0) continue;
                    string k = LooseName(kv.Key);
                    if (k.Length > 0 && !map.ContainsKey(k)) map[k] = FirstOfName(kv.Value);
                }
                looseNames = map;
            }
            CardDef d;
            return looseNames.TryGetValue(loose, out d) ? d : null;
        }

        /// <summary>Lower-case letters and digits only, accents dropped: "Poké Pad" -> "pokepad".</summary>
        public static string LooseName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            var sb = new StringBuilder();
            foreach (char ch in name.Normalize(NormalizationForm.FormD))
            {
                if (CharUnicodeInfo.GetUnicodeCategory(ch) == UnicodeCategory.NonSpacingMark) continue;
                // "Tapu Koko ◇" is written "Tapu Koko Prism Star" in deck lists (v80).
                if (ch == '\u25C7') sb.Append("prismstar");
                else if (char.IsLetterOrDigit(ch)) sb.Append(char.ToLowerInvariant(ch));
            }
            return sb.ToString();
        }

        public static CardDef FromDto(CardDataDto dto)
        {
            var d = new CardDef();
            d.SetCode = (dto.set ?? "").Trim().ToUpperInvariant();
            d.Number = NormalizeNumber(dto.number);
            d.Key = MakeKey(dto.set, dto.number);
            d.CanonicalKey = d.Key;
            d.Name = dto.name ?? d.Key;
            d.RegulationMark = dto.regulationMark ?? "";
            d.Text = dto.text ?? "";
            d.LegalByReprint = dto.reprintLegal;
            d.UnlimitedOnly = dto.unlimitedOnly && !dto.expandedByReprint;
            d.PreBlackWhite = dto.unlimitedOnly;
            d.AnyNumberInDeck = dto.anyNumber;
            d.UnownRule = dto.unownRule;
            d.OnePerDeck = dto.onePerDeck || d.Text.Contains("You can't have more than 1 " + d.Name + " in your deck");
            d.Mechanic = dto.mechanic ?? "";
            d.LegendHalf = dto.legendHalf ?? "";
            d.AncientTrait = dto.ancientTrait ?? "";
            d.Kind = ParseEnum<CardKind>(dto.kind, "kind", d.Key);
            // Ancient / Future: Paradox Pokémon and some Trainers (Explorer's Guidance, Reboot Pod...).
            if (dto.tags != null)
            {
                foreach (var tag in dto.tags)
                {
                    if (string.Equals(tag, "Ancient", StringComparison.OrdinalIgnoreCase)) d.IsAncient = true;
                    if (string.Equals(tag, "Future", StringComparison.OrdinalIgnoreCase)) d.IsFuture = true;
                    if (string.Equals(tag, "Baby", StringComparison.OrdinalIgnoreCase)) d.IsBaby = true; // Neo-era Baby Rule (v48)
                    // Sword & Shield battle styles (v49)
                    if (string.Equals(tag, "Single Strike", StringComparison.OrdinalIgnoreCase)) d.AddStrike("Single Strike");
                    if (string.Equals(tag, "Rapid Strike", StringComparison.OrdinalIgnoreCase)) d.AddStrike("Rapid Strike");
                    if (string.Equals(tag, "Fusion Strike", StringComparison.OrdinalIgnoreCase)) d.AddStrike("Fusion Strike");
                    // Sun & Moon labels (v68)
                    if (string.Equals(tag, "Ultra Beast", StringComparison.OrdinalIgnoreCase)) d.IsUltraBeast = true;
                    if (string.Equals(tag, "TAG TEAM", StringComparison.OrdinalIgnoreCase)) d.HasTagTeamTag = true;
                    // Black & White labels (v108)
                    if (string.Equals(tag, "Team Plasma", StringComparison.OrdinalIgnoreCase)) d.IsTeamPlasma = true;
                }
            }

            if (d.Kind == CardKind.Pokemon)
            {
                d.Stage = ParseEnum<Stage>(dto.stage, "stage", d.Key);
                d.EvolvesFrom = dto.evolvesFrom ?? "";
                d.Hp = dto.hp;
                d.PokemonType = EnergyTypes.ParseType(dto.type) ?? EnergyType.Colorless;
                d.Weakness = EnergyTypes.ParseType(dto.weakness);
                d.Resistance = EnergyTypes.ParseType(dto.resistance);
                d.SecondWeakness = EnergyTypes.ParseType(dto.weakness2);
                d.SecondResistance = EnergyTypes.ParseType(dto.resistance2);
                if (dto.resistanceAmount > 0) d.ResistanceAmount = dto.resistanceAmount;
                d.SecondType = EnergyTypes.ParseType(dto.secondType);
                d.RetreatCost = dto.retreat;
                d.IsEx = dto.ex;
                d.IsMegaEx = dto.megaEx;
                d.IsTera = dto.tera;
                if (dto.abilities != null)
                    foreach (var a in dto.abilities)
                        if (a != null && !string.IsNullOrEmpty(a.name))
                            d.Abilities.Add(new AbilityDef { Name = a.name, Text = a.text ?? "", Kind = string.IsNullOrEmpty(a.kind) ? AbilityDef.AbilityKind : a.kind, VstarPower = a.vstar });
                if (dto.weaknessAmount > 0) d.WeaknessAmount = dto.weaknessAmount;
                AddAttacks(d, dto);
            }
            else if (d.Kind == CardKind.Trainer)
            {
                d.TrainerKind = ParseEnum<TrainerKind>(dto.trainerType, "trainerType", d.Key);
                d.IsAceSpec = dto.aceSpec;
                AddAttacks(d, dto); // Technical Machines carry the attack they give
                // Pokémon Tools that give their Pokémon an Ability (Sky Seal Stone's VSTAR Power, v49).
                if (d.TrainerKind == TrainerKind.Tool && dto.abilities != null)
                    foreach (var a in dto.abilities)
                        if (a != null && !string.IsNullOrEmpty(a.name))
                            d.Abilities.Add(new AbilityDef { Name = a.name, Text = a.text ?? "", VstarPower = a.vstar });
                if (dto.fossil || dto.doll || dto.fossilPlain || dto.pokeDoll || dto.robo)
                {
                    d.IsFossil = true;
                    d.IsDoll = dto.doll;
                    d.IsPokeDoll = dto.pokeDoll;
                    d.FossilPlain = dto.fossilPlain || dto.pokeDoll || dto.robo;
                    d.FossilGivesPrize = dto.fossilPlain || dto.fossilPrize;
                    d.Stage = Stage.Basic;
                    d.Hp = dto.hp > 0 ? dto.hp : 60;
                    d.PokemonType = EnergyType.Colorless;
                    if (dto.abilities != null)
                        foreach (var a in dto.abilities)
                            if (a != null && !string.IsNullOrEmpty(a.name))
                                d.Abilities.Add(new AbilityDef { Name = a.name, Text = a.text ?? "" });
                }
            }
            else
            {
                d.IsBasicEnergy = dto.basicEnergy;
                d.IsAceSpec = dto.aceSpec; // Legacy, Enriching and Neo Upper Energy are ACE SPECs
                d.EnergyProvides = EnergyTypes.ParseType(dto.provides) ?? EnergyType.Colorless;
            }
            return d;
        }

        private static void AddAttacks(CardDef d, CardDataDto dto)
        {
            if (dto.attacks == null) return;
            foreach (var a in dto.attacks)
                if (a != null && !string.IsNullOrEmpty(a.name))
                    d.Attacks.Add(new AttackDef
                    {
                        Name = a.name,
                        Cost = EnergyTypes.ParseCost(a.cost),
                        Damage = a.damage,
                        DamageSuffix = a.damageSuffix ?? "",
                        Text = a.text ?? "",
                        VstarPower = a.vstar
                    });
        }

        private static T ParseEnum<T>(string text, string field, string key) where T : struct
        {
            T value;
            if (!string.IsNullOrEmpty(text) && Enum.TryParse(text.Replace(" ", ""), true, out value)) return value;
            throw new FormatException("Card " + key + ": bad or missing '" + field + "' value '" + text + "'.");
        }
    }

    public sealed class DeckEntry
    {
        public int Count;
        public CardDef Card;
    }

    /// <summary>
    /// Reads deck lists in the Pokémon TCG Live / Limitless export format:
    ///   4 Dreepy TWM 128
    /// Section headers ("Pokémon: 20", "Trainer: 32", "Energy: 8") and blank lines are ignored.
    /// </summary>
    public static class Decklist
    {
        public static List<DeckEntry> Parse(string text, CardDatabase db, List<string> errors)
        {
            var entries = new List<DeckEntry>();
            var lines = (text ?? "").Replace("\r", "").Split('\n');
            foreach (var raw in lines)
            {
                // "{*}" and "Prism Star" are how deck lists write the Prism Star symbol ("Tapu Koko Prism Star", v80).
                var line = PrismStarWords.Replace(raw.Trim().Replace("{*}", "\u25C7"), " \u25C7");
                if (line.Length == 0 || line.StartsWith("#") || line.StartsWith("//")) continue;
                var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                int count;
                if (parts.Length < 2 || !int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out count)) continue;

                CardDef def = null;
                string name = string.Join(" ", parts.Skip(1).ToArray());
                // "2 Charmander (PAF-7)", "2 Charmander [PAF 7]" and "1 Budew PRE-4" (v69): the set and number in brackets, or
                // joined by a hyphen at the end.
                string strippedName = null;
                var m = BracketedPrint.Match(name);
                if (!m.Success) m = HyphenPrint.Match(name);
                if (m.Success)
                {
                    strippedName = m.Groups["name"].Value.Trim();
                    def = db.Get(m.Groups["set"].Value, m.Groups["num"].Value);
                }
                if (def == null && parts.Length >= 4)
                {
                    def = db.Get(parts[parts.Length - 2], parts[parts.Length - 1]);
                    if (def != null) name = string.Join(" ", parts.Skip(1).Take(parts.Length - 3).ToArray());
                }
                if (def == null)
                {
                    if (strippedName == null)
                        strippedName = parts.Length >= 4 ? string.Join(" ", parts.Skip(1).Take(parts.Length - 3).ToArray()) : name;
                    def = db.GetByName(strippedName) ?? db.GetByName(name) ?? db.GetByName(BasicEnergyAlias(strippedName))
                          // "Poke Pad" for Poké Pad, "Professors Research", curly apostrophes (v69)
                          ?? db.GetByLooseName(strippedName) ?? db.GetByLooseName(name) ?? db.GetByLooseName(BasicEnergyAlias(strippedName));
                }
                if (def == null)
                {
                    errors.Add("Unknown card: '" + line + "'");
                    continue;
                }
                entries.Add(new DeckEntry { Count = count, Card = def });
            }
            return entries;
        }

        private static readonly Regex PrismStarWords = new Regex(@"\s+Prism\s+Star\b", RegexOptions.IgnoreCase);

        /// <summary>"Charmander (PAF-7)", "Charmander (PAF 7)", "Charmander [PAF-007]".</summary>
        private static readonly Regex BracketedPrint = new Regex(@"^(?<name>.+?)\s*[\(\[]\s*(?<set>[A-Za-z0-9]{2,6})[\s\-_]+(?<num>[A-Za-z]{0,4}\d+[A-Za-z]?)\s*[\)\]]\s*$");
        /// <summary>"Budew PRE-4", "Budew PRE-004".</summary>
        private static readonly Regex HyphenPrint = new Regex(@"^(?<name>.+?)\s+(?<set>[A-Za-z0-9]{2,6})-(?<num>[A-Za-z]{0,4}\d+[A-Za-z]?)$");

        /// <summary>"Basic {P} Energy" and "Basic Psychic Energy" both mean "Psychic Energy".</summary>
        private static string BasicEnergyAlias(string name)
        {
            string n = name.Replace("Basic ", "");
            if (n.StartsWith("{") && n.Length > 3 && n[2] == '}')
            {
                var t = EnergyTypes.FromSymbol(n[1]);
                return t + n.Substring(3);
            }
            return n;
        }

        /// <summary>Groups a flat 60-card list into entries, keeping first-appearance order.</summary>
        public static List<DeckEntry> Group(IEnumerable<CardDef> deck)
        {
            var map = new Dictionary<string, DeckEntry>();
            var list = new List<DeckEntry>();
            foreach (var d in deck)
            {
                DeckEntry e;
                if (map.TryGetValue(d.Key, out e))
                {
                    e.Count++;
                    continue;
                }
                e = new DeckEntry { Count = 1, Card = d };
                map[d.Key] = e;
                list.Add(e);
            }
            return list;
        }

        /// <summary>Writes a deck in the TCG Live / Limitless export format, with an optional "# Name:" line.</summary>
        public static string Format(string deckName, IList<DeckEntry> entries)
        {
            return Format(deckName, entries, DeckFormat.Standard);
        }

        /// <summary>Same, with a "# Format: Expanded" / "# Format: Unlimited" line (Standard is the default and isn't written).</summary>
        public static string Format(string deckName, IList<DeckEntry> entries, DeckFormat format)
        {
            return Format(deckName, entries, Formats.Label(format));
        }

        /// <summary>Same, with any format's name, a custom format's included (v123).</summary>
        public static string Format(string deckName, IList<DeckEntry> entries, string formatName)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(deckName)) sb.Append("# Name: ").Append(deckName.Trim()).Append('\n');
            if (!string.IsNullOrEmpty(formatName) && !formatName.Trim().Equals("Standard", StringComparison.OrdinalIgnoreCase))
                sb.Append("# Format: ").Append(formatName.Trim()).Append('\n');
            AppendSection(sb, "Pokémon", entries.Where(e => e.Card.IsPokemon));
            AppendSection(sb, "Trainer", entries.Where(e => e.Card.IsTrainer));
            AppendSection(sb, "Energy", entries.Where(e => e.Card.IsEnergy));
            return sb.ToString();
        }

        private static void AppendSection(StringBuilder sb, string title, IEnumerable<DeckEntry> items)
        {
            var list = items.Where(e => e.Count > 0).ToList();
            if (list.Count == 0) return;
            sb.Append(title).Append(": ").Append(list.Sum(e => e.Count)).Append('\n');
            foreach (var e in list)
                sb.Append(e.Count).Append(' ').Append(e.Card.Name).Append(' ').Append(e.Card.SetCode).Append(' ').Append(e.Card.Number).Append('\n');
            sb.Append('\n');
        }

        /// <summary>Reads the "# Name: ..." line from a deck file, or returns null.</summary>
        public static string ReadName(string text)
        {
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                var line = raw.Trim();
                if (line.StartsWith("# Name:")) return line.Substring(7).Trim();
            }
            return null;
        }

        /// <summary>The name on the "# Format: ..." line (a built-in or custom format), or "Standard" with no line (v123).</summary>
        public static string ReadFormatName(string text)
        {
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                var line = raw.Trim();
                if (line.StartsWith("# Format:")) return line.Substring(9).Trim();
            }
            return "Standard";
        }

        /// <summary>Reads the "# Format: ..." line from a deck file. No line means Standard.</summary>
        public static DeckFormat ReadFormat(string text)
        {
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                var line = raw.Trim();
                if (line.StartsWith("# Format:")) return Formats.Parse(line.Substring(9));
            }
            return DeckFormat.Standard;
        }

        public static List<CardDef> Expand(List<DeckEntry> entries)
        {
            var list = new List<CardDef>();
            foreach (var e in entries)
                for (int i = 0; i < e.Count; i++) list.Add(e.Card);
            return list;
        }

        /// <summary>
        /// Deck-building rules for a format: 60 cards, 4 copies, 1 ACE SPEC, a Basic Pokémon, the format's card pool
        /// (Standard: H and later; Expanded: everything but the Unlimited-only classics; Unlimited: everything), and the
        /// older "1 Shining Pokémon of each name" and "1 Radiant Pokémon" rules. Returns an empty list when the deck is legal.
        /// </summary>
        public static List<string> Validate(IList<CardDef> deck, DeckFormat format)
        {
            var problems = Validate(deck, Formats.LegalMarks(format));
            if (format == DeckFormat.Expanded)
                foreach (var d in deck.Distinct())
                {
                    if (Formats.IsExpandedBanned(d))
                        problems.Add(d.Name + " (" + d.Key + ") is banned in Expanded: it's only legal in Unlimited.");
                    else if (d.UnlimitedOnly)
                        problems.Add(d.Name + " (" + d.Key + ") is Unlimited only (older than Black & White, or not legal for tournaments).");
                }
            foreach (var g in deck.Where(d => d.IsPokemon && d.Name.StartsWith("Shining ") && !d.OnePerDeck).GroupBy(d => d.Name))
                if (g.Count() > 1) problems.Add("Only 1 " + g.Key + " is allowed in a deck.");
            return problems;
        }

        /// <summary>Basic deck-building rules plus a regulation-mark check (null marks = any). Returns an empty list when the deck is legal.</summary>
        public static List<string> Validate(IList<CardDef> deck, ICollection<string> legalRegulationMarks)
        {
            return Validate(deck, legalRegulationMarks, 60, 4);
        }

        /// <summary>The same with another deck size and copy limit (custom formats, v123: 1 copy in a singleton format).</summary>
        public static List<string> Validate(IList<CardDef> deck, ICollection<string> legalRegulationMarks, int deckSize, int maxCopies)
        {
            var problems = new List<string>();
            if (deck.Count != deckSize) problems.Add("Deck has " + deck.Count + " cards (needs exactly " + deckSize + ").");
            // v136: a card that says you may have as many of it as you like (Arceus) doesn't count, in any format (your ruling).
            foreach (var g in deck.Where(d => !(d.IsEnergy && d.IsBasicEnergy) && !d.AnyNumberInDeck).GroupBy(d => d.Name))
                if (g.Count() > maxCopies) problems.Add(maxCopies == 1 ? "More than 1 copy of " + g.Key + " (singleton)." : "More than " + maxCopies + " copies of " + g.Key + ".");
            // v139 (Neo Discovery's Unown, your ruling): with any card with that rule in the deck, all Basic Pokémon with Unown in
            // their names count together toward 4, whatever letter they are.
            if (deck.Any(d => d.UnownRule))
            {
                int unown = deck.Count(d => d.IsUnownBasic);
                if (unown > 4) problems.Add("More than 4 Basic Pokémon with Unown in their names (" + unown + ").");
            }
            // v142: "You can't have more than 1 [this card] in your deck" (Miracle Energy, the Neo Shining Pokémon).
            foreach (var g in deck.Where(d => d.OnePerDeck).GroupBy(d => d.Name))
                if (deck.Count(d => d.Name == g.Key) > 1) problems.Add("Only 1 " + g.Key + " is allowed in a deck.");
            int aceSpecs = deck.Count(d => d.IsAceSpec);
            if (aceSpecs > 1) problems.Add("More than 1 ACE SPEC card (" + aceSpecs + ").");
            // Expanded (v109): Professor's Research, Professor Sycamore and Professor Juniper can't share a deck.
            var professors = deck.Where(d => d.IsSupporter && (d.Name == "Professor's Research" || d.Name == "Professor Sycamore" || d.Name == "Professor Juniper"))
                                 .Select(d => d.Name).Distinct().ToList();
            if (professors.Count > 1) problems.Add(string.Join(", ", professors.ToArray()) + " can't be in the same deck.");
            // Lysandre and Boss's Orders can't share a deck either (Compendium, 2020-04-30; v134).
            if (deck.Any(d => d.IsSupporter && d.Name == "Lysandre") && deck.Any(d => d.IsSupporter && d.Name == "Boss's Orders"))
                problems.Add("Lysandre, Boss's Orders can't be in the same deck.");
            int radiant = deck.Count(d => d.IsPokemon && d.IsRadiant);
            if (radiant > 1) problems.Add("More than 1 Radiant Pokémon (" + radiant + ").");
            // Pokémon ★ (Umbreon ★, v56): "You can't have more than 1 Pokémon ★ in your deck."
            int stars = deck.Count(d => d.IsPokemon && d.IsPokemonStar);
            if (stars > 1) problems.Add("More than 1 Pokémon ★ (" + stars + ").");
            // Prism Star (v80): "You can't have more than 1 ◇ card with the same name in your deck."
            foreach (var g in deck.Where(d => d.IsPrismStar).GroupBy(d => d.Name))
                if (g.Count() > 1) problems.Add("Only 1 " + g.Key + " is allowed in a deck (Prism Star).");
            if (!deck.Any(d => d.IsBasicPokemon)) problems.Add("Deck has no Basic Pokémon.");
            // Shedinja's Shell Survival (VIV, v62; Compendium 2020-11-12): it can't be the deck's only Basic Pokémon.
            else if (!deck.Any(d => d.IsBasicPokemon && !OnlyPutIntoPlayByEffect(d)))
                problems.Add("Deck has no Basic Pokémon you can play (Shedinja's Shell Survival).");
            if (legalRegulationMarks != null)
                foreach (var d in deck.Distinct())
                    if (!(d.IsEnergy && d.IsBasicEnergy) && !d.LegalByReprint && !legalRegulationMarks.Contains(d.RegulationMark))
                        problems.Add(d.Name + " (" + d.Key + ", mark '" + d.RegulationMark + "') isn't legal in this format.");
            return problems;
        }

        /// <summary>
        /// v139: one more copy of this card would break the Unown rule ("up to 4 Basic Pokémon cards with Unown in their names" once
        /// any card with that rule is in the deck).
        /// </summary>
        public static bool UnownRuleBlocks(IEnumerable<CardDef> deck, CardDef adding)
        {
            if (adding == null || !adding.IsUnownBasic) return false;
            var list = deck.Where(d => d != null).ToList();
            if (!adding.UnownRule && !list.Any(d => d.UnownRule)) return false;
            return list.Count(d => d.IsUnownBasic) >= 4;
        }

        /// <summary>A Basic Pokémon that only an effect can put into play (Shedinja's Shell Survival), so it can't start the game.</summary>
        public static bool OnlyPutIntoPlayByEffect(CardDef card)
        {
            return card.IsBasicPokemon && card.Name == "Shedinja" && card.Abilities.Any(ab => ab.Name == "Shell Survival");
        }
    }
}
