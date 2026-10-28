using System;
using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>The deck formats. A deck list says which one it's built for ("# Format: Expanded"); Standard is the default.</summary>
    public enum DeckFormat
    {
        Standard,
        Expanded,
        /// <summary>Every card, including the Classic Collection reprints of cards older than Black & White.</summary>
        Unlimited
    }

    /// <summary>The three pools a card can be in (the deck builder's tabs).</summary>
    public enum CardCategory
    {
        /// <summary>Regulation mark H and later (or a legal reprint, or a Basic Energy).</summary>
        Standard,
        /// <summary>Black & White through regulation mark G.</summary>
        Expanded,
        /// <summary>Cards that first came out before Black & White (30th Celebration's Classic Collection): Unlimited only.</summary>
        Unlimited
    }

    /// <summary>
    /// Format legality as data. Rotation is a one-line change here (or, later, a format file in an update pack).
    ///
    /// Card categories: a card is in the Standard category when its regulation mark is H or later (or it's legal
    /// through a reprint, or it's a Basic Energy). Cards whose original printing is older than Black & White are in the
    /// Unlimited category. Everything else (Black & White through G) is in the Expanded category.
    /// Standard decks may only use Standard cards; Expanded decks Standard and Expanded cards; Unlimited decks anything.
    /// </summary>
    public static class Formats
    {
        /// <summary>Standard from April 10, 2026 (Pokémon TCG Live from March 26, 2026): G rotated out; H, I, J and later legal.</summary>
        public static readonly string[] Standard2026 = { "H", "I", "J", "K", "L" };

        /// <summary>
        /// The Expanded ban list (your list, v67; checked again on 2026-10-15 against the official list: 27 cards, Medicham V from
        /// April 10, 2026 included. Blaine's Quiz Show, banned only in Pokémon TCG Live, stays legal here: your ruling, v96):
        /// these printings can't be played in Expanded, only in Unlimited. Keys are
        /// set code and number; every printing of a banned card shares its main key, so reprints are banned too. Most
        /// of these sets aren't in the pool yet; the ban applies as soon as they're added under these keys.
        /// </summary>
        public static readonly HashSet<string> ExpandedBanned = new HashSet<string>(StringComparer.Ordinal)
        {
            "NVI-67", "DEX-110",                       // Archeops
            "UNB-165",                                 // Chip-Chip Ice Axe
            "BKP-98", "BKP-98a", "BKP-98b",            // Delinquent
            "CEC-83",                                  // Duskull
            "FLI-83",                                  // Flabébé
            "RCL-22", "SHF-SV13", "ASR-TG2", "SP-22",  // Flapple (in the pool)
            "AOR-74",                                  // Forest of Giant Plants
            "PLF-101", "PLF-115",                      // Ghetsis
            "AOR-75", "AOR-75a",                       // Hex Maniac
            "CEC-194", "CEC-265",                      // Island Challenge Amulet
            "HIF-58", "HIF-68",                        // Jessie & James
            "UNB-178", "HIF-60",                       // Lt. Surge's Strategy
            "PHF-99", "PHF-118",                       // Lysandre's Trump Card
            "SLG-45", "SMP-85", "SMP-SM85",            // Marshadow
            "PRC-133", "PRC-158",                      // Maxie's Hidden Ball Trick
            "EVS-83", "EVS-185", "EVS-186",            // Medicham V (in the pool)
            "FLF-23",                                  // Milotic
            "UNB-78",                                  // Mismagius
            "UPR-114",                                 // Oranguru
            "BKP-109",                                 // Puzzle of Time
            "XY-124", "GEN-71",                        // Red Card
            "UNM-206", "UNM-206a", "UNM-253",          // Reset Stamp
            "DEX-62",                                  // Sableye
            "RCL-165", "RCL-207",                      // Scoop Up Net (in the pool)
            "ROS-77", "ROS-77a", "ROS-106",            // Shaymin-EX
            "LOT-90", "LOT-91",                        // Unown
        };

        /// <summary>True when this printing (or the card it reprints) is on the Expanded ban list.</summary>
        public static bool IsExpandedBanned(CardDef d)
        {
            return d != null && (ExpandedBanned.Contains(d.Key) || ExpandedBanned.Contains(d.BaseKey));
        }

        /// <summary>Unlimited only: printed before Black & White, marked not legal for tournaments, or banned in Expanded.</summary>
        public static bool IsUnlimitedOnly(CardDef d)
        {
            return d != null && (d.UnlimitedOnly || IsExpandedBanned(d));
        }

        /// <summary>True when the card is in the current Standard category (H and later).</summary>
        public static bool IsStandardCard(CardDef d)
        {
            if (d == null) return false;
            if (d.IsEnergy && d.IsBasicEnergy) return true;
            if (IsUnlimitedOnly(d)) return false;
            return d.LegalByReprint || Array.IndexOf(Standard2026, d.RegulationMark) >= 0;
        }

        public static CardCategory CategoryOf(CardDef d)
        {
            if (IsStandardCard(d)) return CardCategory.Standard;
            return IsUnlimitedOnly(d) ? CardCategory.Unlimited : CardCategory.Expanded;
        }

        /// <summary>True when the card may go in a deck of this format.</summary>
        public static bool Allows(DeckFormat format, CardDef d)
        {
            switch (format)
            {
                case DeckFormat.Unlimited: return true;
                case DeckFormat.Expanded: return d != null && !IsUnlimitedOnly(d);
                default: return IsStandardCard(d);
            }
        }

        /// <summary>The narrowest format these cards fit in (Standard, then Expanded, then Unlimited).</summary>
        public static DeckFormat Needed(IEnumerable<CardDef> cards)
        {
            var needed = DeckFormat.Standard;
            foreach (var d in cards)
            {
                if (d == null) continue;
                if (IsUnlimitedOnly(d)) return DeckFormat.Unlimited;
                if (!IsStandardCard(d)) needed = DeckFormat.Expanded;
            }
            return needed;
        }

        /// <summary>Regulation marks a format allows, for Decklist.Validate. Null means any mark (Expanded, Unlimited).</summary>
        public static ICollection<string> LegalMarks(DeckFormat format)
        {
            return format == DeckFormat.Standard ? Standard2026 : null;
        }

        public static string Label(DeckFormat format)
        {
            switch (format)
            {
                case DeckFormat.Expanded: return "Expanded";
                case DeckFormat.Unlimited: return "Unlimited";
                default: return "Standard";
            }
        }

        /// <summary>"Expanded" / "Unlimited" (any case); anything else is Standard.</summary>
        public static DeckFormat Parse(string text)
        {
            var t = text == null ? "" : text.Trim();
            if (t.Equals("Expanded", StringComparison.OrdinalIgnoreCase)) return DeckFormat.Expanded;
            if (t.Equals("Unlimited", StringComparison.OrdinalIgnoreCase)) return DeckFormat.Unlimited;
            return DeckFormat.Standard;
        }
    }
}
