using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PokemonTCG.Core
{
    /// <summary>
    /// A format you make yourself (v123): its name, deck size, Prize cards, legal sets, banned cards, deck rules (singleton,
    /// Rule Box Pokémon, ACE SPECs) and game rules (GameRules). Saved as a small text file (ToText / Parse); decks built for it
    /// carry a "# Format: name" line.
    /// </summary>
    public sealed class CustomFormat
    {
        public const string Header = "# Pokemon TCG custom format v1";
        public const int MinDeckSize = 20, MaxDeckSize = 100, MinPrizes = 1, MaxPrizes = 6;

        public string Name = "My Format";
        public int DeckSize = 60;
        public int PrizeCount = 6;
        /// <summary>Every set is legal, including sets added to the game later. When false, only Sets are.</summary>
        public bool AllSets = true;
        /// <summary>Legal set codes ("BS", "JU", "SVI"...). A card is legal when any of its printings is from one of them.</summary>
        public readonly HashSet<string> Sets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>Banned cards by main printing key (every printing of a banned card is banned).</summary>
        public readonly HashSet<string> Banned = new HashSet<string>(StringComparer.Ordinal);
        /// <summary>Every card except basic Energy is limited to 1 copy (instead of 4).</summary>
        public bool Singleton;
        /// <summary>Rule Box Pokémon (ex, EX, GX, V, VSTAR, VMAX, Radiant, BREAK, LV.X, Prism Star...) may be played.</summary>
        public bool AllowRuleBox = true;
        /// <summary>ACE SPEC cards may be played (still 1 per deck).</summary>
        public bool AllowAceSpec = true;
        public GameRules Rules = new GameRules();

        public int MaxCopies { get { return Singleton ? 1 : 4; } }

        public CustomFormat Clone()
        {
            var c = new CustomFormat
            {
                Name = Name, DeckSize = DeckSize, PrizeCount = PrizeCount, AllSets = AllSets, Singleton = Singleton,
                AllowRuleBox = AllowRuleBox, AllowAceSpec = AllowAceSpec, Rules = (Rules ?? new GameRules()).Clone()
            };
            c.Sets.UnionWith(Sets);
            c.Banned.UnionWith(Banned);
            return c;
        }

        // ------------------------------------------------------------------ Card legality

        private CardDatabase legalDb;
        private HashSet<string> legalKeys;

        /// <summary>Call after changing Sets or AllSets.</summary>
        public void Invalidate()
        {
            legalDb = null;
            legalKeys = null;
        }

        /// <summary>True when the card has a printing from a legal set (basic Energy always does).</summary>
        public bool InLegalSet(CardDatabase db, CardDef d)
        {
            if (d == null) return false;
            if (AllSets || (d.IsEnergy && d.IsBasicEnergy)) return true;
            if (db == null) return Sets.Contains(d.SetCode ?? "");
            lock (Sets)
            {
                if (legalKeys == null || legalDb != db)
                {
                    var keys = new HashSet<string>(StringComparer.Ordinal);
                    foreach (var p in db.All)
                        if (!string.IsNullOrEmpty(p.SetCode) && Sets.Contains(p.SetCode)) keys.Add(p.BaseKey);
                    legalKeys = keys;
                    legalDb = db;
                }
                return legalKeys.Contains(d.BaseKey);
            }
        }

        public bool IsBanned(CardDef d) { return d != null && (Banned.Contains(d.BaseKey) || Banned.Contains(d.Key)); }

        /// <summary>A Pokémon with a Rule Box (or a Prism Star card), which "Rule Box Pokémon" off keeps out.</summary>
        public static bool IsRuleBoxPokemon(CardDef d) { return d != null && d.IsPokemon && (d.HasRuleBox || d.IsPrismStar); }

        /// <summary>Why the card can't go in a deck of this format, or null when it can.</summary>
        public string WhyNotAllowed(CardDatabase db, CardDef d)
        {
            if (d == null) return "unknown card";
            if (IsBanned(d)) return "banned in " + Name;
            if (!InLegalSet(db, d)) return "not from a legal set of " + Name;
            if (!AllowRuleBox && IsRuleBoxPokemon(d)) return "no Rule Box Pokémon in " + Name;
            if (!AllowAceSpec && d.IsAceSpec) return "no ACE SPEC cards in " + Name;
            return null;
        }

        public bool Allows(CardDatabase db, CardDef d) { return WhyNotAllowed(db, d) == null; }

        /// <summary>Deck-building problems for this format (empty when the deck is legal).</summary>
        public List<string> Validate(CardDatabase db, IList<CardDef> deck)
        {
            var problems = Decklist.Validate(deck, null, DeckSize, MaxCopies);
            foreach (var d in deck.Distinct())
            {
                var why = WhyNotAllowed(db, d);
                if (why != null) problems.Add(d.Name + " (" + d.Key + ") isn't legal: " + why + ".");
            }
            foreach (var g in deck.Where(x => x.IsPokemon && x.Name.StartsWith("Shining ")).GroupBy(x => x.Name))
                if (g.Count() > 1) problems.Add("Only 1 " + g.Key + " is allowed in a deck.");
            if (PrizeCount >= DeckSize - 7) problems.Add(Name + " needs more cards in a deck than " + PrizeCount + " Prize cards and a hand of 7.");
            return problems;
        }

        // ------------------------------------------------------------------ Names

        public static readonly string[] Reserved = { "Standard", "Expanded", "Unlimited" };

        /// <summary>Why this name can't be used for a custom format, or null when it's fine.</summary>
        public static string NameProblem(string name, IEnumerable<CustomFormat> others, CustomFormat self)
        {
            var n = (name ?? "").Trim();
            if (n.Length == 0) return "Give the format a name.";
            if (n.Length > 40) return "Keep the name under 40 characters.";
            if (n.IndexOfAny(new[] { '\n', '\r', '\t', ':', '#' }) >= 0) return "The name can't contain ':' or '#'.";
            if (Reserved.Any(r => r.Equals(n, StringComparison.OrdinalIgnoreCase))) return "\"" + n + "\" is a built-in format.";
            if (others != null && others.Any(o => o != self && o.Name.Trim().Equals(n, StringComparison.OrdinalIgnoreCase)))
                return "There's already a format called \"" + n + "\".";
            return null;
        }

        // ------------------------------------------------------------------ Text file

        public string ToText()
        {
            var sb = new StringBuilder();
            sb.Append(Header).Append('\n');
            sb.Append("name: ").Append(Name.Trim()).Append('\n');
            sb.Append("deck-size: ").Append(DeckSize.ToString(CultureInfo.InvariantCulture)).Append('\n');
            sb.Append("prizes: ").Append(PrizeCount.ToString(CultureInfo.InvariantCulture)).Append('\n');
            sb.Append("sets: ").Append(AllSets ? "all" : string.Join(", ", Sets.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray())).Append('\n');
            sb.Append("banned: ").Append(string.Join(", ", Banned.OrderBy(s => s, StringComparer.Ordinal).ToArray())).Append('\n');
            sb.Append("singleton: ").Append(YesNo(Singleton)).Append('\n');
            sb.Append("rule-box-pokemon: ").Append(YesNo(AllowRuleBox)).Append('\n');
            sb.Append("ace-spec: ").Append(YesNo(AllowAceSpec)).Append('\n');
            var r = Rules ?? new GameRules();
            sb.Append("first-turn-attack: ").Append(YesNo(r.FirstTurnAttack)).Append('\n');
            sb.Append("first-turn-supporter: ").Append(YesNo(r.FirstTurnSupporter)).Append('\n');
            sb.Append("draw-before-coin-flip: ").Append(YesNo(r.DrawBeforeCoinFlip)).Append('\n');
            sb.Append("tools-are-items: ").Append(YesNo(r.ToolsAreItems)).Append('\n');
            sb.Append("old-trainers-are-plain-trainers: ").Append(YesNo(r.OldTrainersArePlain)).Append('\n');
            return sb.ToString();
        }

        private static string YesNo(bool b) { return b ? "yes" : "no"; }

        private static bool Yes(string v) { v = v.Trim().ToLowerInvariant(); return v == "yes" || v == "true" || v == "1" || v == "on"; }

        /// <summary>Reads ToText's file. Unknown lines are ignored; missing ones keep the defaults. Null if there's no name.</summary>
        public static CustomFormat Parse(string text)
        {
            var f = new CustomFormat { Name = "" };
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                var line = raw.Trim();
                if (line.Length == 0 || line.StartsWith("#")) continue;
                int colon = line.IndexOf(':');
                if (colon <= 0) continue;
                var key = line.Substring(0, colon).Trim().ToLowerInvariant();
                var value = line.Substring(colon + 1).Trim();
                int n;
                switch (key)
                {
                    case "name": f.Name = value; break;
                    case "deck-size":
                        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) f.DeckSize = Math.Max(MinDeckSize, Math.Min(MaxDeckSize, n));
                        break;
                    case "prizes":
                        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out n)) f.PrizeCount = Math.Max(MinPrizes, Math.Min(MaxPrizes, n));
                        break;
                    case "sets":
                        f.Sets.Clear();
                        f.AllSets = value.Equals("all", StringComparison.OrdinalIgnoreCase);
                        if (!f.AllSets)
                            foreach (var s in value.Split(',')) if (s.Trim().Length > 0) f.Sets.Add(s.Trim());
                        break;
                    case "banned":
                        f.Banned.Clear();
                        foreach (var s in value.Split(',')) if (s.Trim().Length > 0) f.Banned.Add(s.Trim());
                        break;
                    case "singleton": f.Singleton = Yes(value); break;
                    case "rule-box-pokemon": f.AllowRuleBox = Yes(value); break;
                    case "ace-spec": f.AllowAceSpec = Yes(value); break;
                    case "first-turn-attack": f.Rules.FirstTurnAttack = Yes(value); break;
                    case "first-turn-supporter": f.Rules.FirstTurnSupporter = Yes(value); break;
                    case "draw-before-coin-flip": f.Rules.DrawBeforeCoinFlip = Yes(value); break;
                    case "tools-are-items": f.Rules.ToolsAreItems = Yes(value); break;
                    case "old-trainers-are-plain-trainers": f.Rules.OldTrainersArePlain = Yes(value); break;
                }
            }
            return f.Name.Length == 0 ? null : f;
        }
    }

    /// <summary>
    /// A deck's format: one of the built-in ones (Standard, Expanded, Unlimited) or a custom format (v123). Games use its Prize
    /// cards and rules; the deck builder its card pool and deck rules.
    /// </summary>
    public sealed class GameFormat
    {
        public readonly string Name;
        /// <summary>The built-in format (meaningless for a custom one).</summary>
        public readonly DeckFormat BuiltIn;
        public readonly CustomFormat Custom;

        private GameFormat(string name, DeckFormat builtIn, CustomFormat custom)
        {
            Name = name;
            BuiltIn = builtIn;
            Custom = custom;
        }

        public bool IsCustom { get { return Custom != null; } }

        public static readonly GameFormat Standard = new GameFormat("Standard", DeckFormat.Standard, null);
        public static readonly GameFormat Expanded = new GameFormat("Expanded", DeckFormat.Expanded, null);
        public static readonly GameFormat Unlimited = new GameFormat("Unlimited", DeckFormat.Unlimited, null);
        public static readonly GameFormat[] BuiltIns = { Standard, Expanded, Unlimited };

        public static GameFormat Of(DeckFormat f) { return BuiltIns[(int)f]; }

        public static GameFormat Of(CustomFormat custom) { return custom == null ? Standard : new GameFormat(custom.Name, DeckFormat.Unlimited, custom); }

        /// <summary>A built-in format by name, or a custom format in CustomFormats; null if there's none by that name.</summary>
        public static GameFormat Named(string name)
        {
            var n = (name ?? "").Trim();
            if (n.Length == 0) return Standard;
            foreach (var b in BuiltIns)
                if (b.Name.Equals(n, StringComparison.OrdinalIgnoreCase)) return b;
            var custom = CustomFormats.Find(n);
            return custom == null ? null : Of(custom);
        }

        public int DeckSize { get { return Custom != null ? Custom.DeckSize : 60; } }
        public int PrizeCount { get { return Custom != null ? Custom.PrizeCount : 6; } }
        public GameRules Rules { get { return Custom != null ? Custom.Rules ?? GameRules.Default : GameRules.Default; } }

        public bool Allows(CardDef d)
        {
            return Custom != null ? Custom.Allows(CustomFormats.Database, d) : Formats.Allows(BuiltIn, d);
        }

        /// <summary>Why the card can't go in a deck of this format, or null.</summary>
        public string WhyNotAllowed(CardDef d)
        {
            if (Custom != null) return Custom.WhyNotAllowed(CustomFormats.Database, d);
            if (Formats.Allows(BuiltIn, d)) return null;
            return Formats.IsExpandedBanned(d) ? "Banned in Expanded" : Formats.IsUnlimitedOnly(d) ? "Unlimited only" : "Expanded only";
        }

        public List<string> Validate(IList<CardDef> deck)
        {
            return Custom != null ? Custom.Validate(CustomFormats.Database, deck) : Decklist.Validate(deck, BuiltIn);
        }

        public override string ToString() { return Name; }
    }

    /// <summary>The custom formats the game knows (the Unity side loads them from the Formats folder), and the card pool they check against.</summary>
    public static class CustomFormats
    {
        private static List<CustomFormat> all = new List<CustomFormat>();
        public static CardDatabase Database;

        public static IList<CustomFormat> All { get { return all; } }

        /// <summary>Replaces the list (a new list each time, so readers on other threads keep a consistent one).</summary>
        public static void Set(IEnumerable<CustomFormat> formats)
        {
            all = (formats ?? Enumerable.Empty<CustomFormat>()).Where(f => f != null).OrderBy(f => f.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static CustomFormat Find(string name)
        {
            var n = (name ?? "").Trim();
            foreach (var f in all)
                if (f.Name.Trim().Equals(n, StringComparison.OrdinalIgnoreCase)) return f;
            return null;
        }
    }
}
