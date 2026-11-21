using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>A deck the AI built for itself to play (v175): the list, its name and how it was built.</summary>
    public sealed class BuiltDeck
    {
        /// <summary>The card the deck is built around.</summary>
        public CardDef Around;
        public string Name = "";
        public DeckFormat Format;
        public List<CardDef> Cards = new List<CardDef>();
        /// <summary>The deck list text (with its "# Format:" line), ready for the deck library.</summary>
        public string Text = "";
        /// <summary>The advisor's notes on how the deck was put together.</summary>
        public readonly List<string> Notes = new List<string>();
        /// <summary>Set when no deck could be built, with the reason.</summary>
        public string Error;
        public bool Ok { get { return Error == null && Cards.Count == 60; } }
    }

    /// <summary>
    /// v175: the AI builds its own deck for a game, in a format, around a card you name or one it picks at random. The deck advisor
    /// does the building (the library's decks and what the AI has learned about cards); a random pick that comes out illegal is
    /// thrown away and another card is tried.
    /// </summary>
    public static class RandomDeckBuilder
    {
        /// <summary>How many random cards are tried before giving up.</summary>
        public const int RandomTries = 8;

        /// <summary>
        /// Pokémon worth building a deck around in a format: one printing each, legal there, playable from the hand, and hitting
        /// hard enough to win games (a rough 100 damage, or a Pokémon ex).
        /// </summary>
        public static List<CardDef> Candidates(CardDatabase db, DeckFormat format)
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            var list = new List<CardDef>();
            foreach (var d in db.Cards)
            {
                if (d == null || !d.IsPokemon || d.IsAlternatePrint || !Formats.Allows(format, d)) continue;
                if (Decklist.OnlyPutIntoPlayByEffect(d) || d.Stage == Stage.Restored || d.Attacks.Count == 0) continue;
                if (!(d.IsEx || d.IsMegaEx) && DeckAdvisor.BestDamage(d) < 100) continue;
                if (seen.Add(DeckAdvisor.Id(d))) list.Add(d);
            }
            return list;
        }

        /// <summary>Pokémon in the format whose name contains the text, best matches first (for the "build around" search box).</summary>
        public static List<CardDef> Search(CardDatabase db, DeckFormat format, string text, int max)
        {
            var list = new List<CardDef>();
            if (string.IsNullOrWhiteSpace(text)) return list;
            string want = text.Trim();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var d in db.Cards)
            {
                if (d == null || d.IsAlternatePrint || !Formats.Allows(format, d)) continue;
                if (d.Name.IndexOf(want, StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (seen.Add(DeckAdvisor.Id(d))) list.Add(d);
            }
            return list
                .OrderBy(d => d.Name.Equals(want, StringComparison.OrdinalIgnoreCase) ? 0 : d.Name.StartsWith(want, StringComparison.OrdinalIgnoreCase) ? 1 : 2)
                .ThenBy(d => d.IsPokemon ? 0 : 1)
                .ThenBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
                .ThenBy(d => d.Key, StringComparer.Ordinal)
                .Take(max)
                .ToList();
        }

        /// <summary>
        /// Builds a deck. With a card, around that card (one try; its error comes back if it can't be done). Without one, around a
        /// random candidate, trying others until a legal 60-card deck comes out.
        /// </summary>
        public static BuiltDeck Build(CardDatabase db, DeckFormat format, IEnumerable<AdvisorDeck> library, LearningBrain brain, CardDef around, Random rng)
        {
            var advisor = new DeckAdvisor(db, format, library, brain);
            if (around != null) return FromAdvice(advisor.BuildAround(around), around, format);

            var pool = Candidates(db, format);
            if (pool.Count == 0) return new BuiltDeck { Format = format, Error = "There are no Pokémon to build around in " + Formats.Label(format) + "." };
            BuiltDeck last = null;
            for (int i = 0; i < RandomTries && pool.Count > 0; i++)
            {
                int pick = rng.Next(pool.Count);
                var card = pool[pick];
                pool.RemoveAt(pick);
                last = FromAdvice(advisor.BuildAround(card), card, format);
                if (last.Ok) return last;
            }
            if (last == null) last = new BuiltDeck { Format = format };
            last.Error = "Couldn't build a legal deck after " + RandomTries + " tries" + (last.Error != null ? " (last: " + last.Error + ")" : "") + ". Try again or name a card.";
            return last;
        }

        private static BuiltDeck FromAdvice(DeckAdvice advice, CardDef around, DeckFormat format)
        {
            var built = new BuiltDeck { Around = around, Format = format };
            if (advice == null)
            {
                built.Error = "The advisor didn't return a deck.";
                return built;
            }
            built.Notes.AddRange(advice.Notes);
            if (advice.Error != null)
            {
                built.Error = advice.Error;
                return built;
            }
            built.Cards = advice.Result ?? new List<CardDef>();
            built.Name = string.IsNullOrEmpty(advice.DeckName) ? around.Name + " (AI)" : advice.DeckName;
            var problems = Decklist.Validate(built.Cards, format);
            if (built.Cards.Count != 60 || problems.Count > 0)
            {
                built.Error = problems.Count > 0 ? problems[0] : "The deck has " + built.Cards.Count + " cards, not 60.";
                return built;
            }
            built.Text = Decklist.Format(built.Name, Decklist.Group(built.Cards), format);
            return built;
        }
    }
}
