using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>What a card does for a deck, read from its text (v75, for the deck advisor). A card can have several roles.</summary>
    [Flags]
    public enum CardRole
    {
        None = 0,
        /// <summary>Draws cards (Lillie's Determination, a drawing Ability).</summary>
        Draw = 1 << 0,
        /// <summary>Finds Pokémon in the deck (Ultra Ball, Nest Ball, Buddy-Buddy Poffin).</summary>
        PokemonSearch = 1 << 1,
        /// <summary>Finds Energy in the deck (Earthen Vessel).</summary>
        EnergySearch = 1 << 2,
        /// <summary>Finds Trainer cards in the deck (Arven, Pokégear 3.0).</summary>
        TrainerSearch = 1 << 3,
        /// <summary>Finds any card in the deck (Pidgeot ex's Quick Search).</summary>
        Tutor = 1 << 4,
        /// <summary>Brings up one of the opponent's Benched Pokémon (Boss's Orders).</summary>
        Gust = 1 << 5,
        /// <summary>Switches or makes retreating free (Switch, Air Balloon).</summary>
        Switch = 1 << 6,
        /// <summary>Gets cards back from the discard pile (Night Stretcher, Super Rod).</summary>
        Recovery = 1 << 7,
        /// <summary>Attaches extra Energy.</summary>
        EnergyAccel = 1 << 8,
        /// <summary>Disrupts the opponent's hand or board (Iono, Judge, Crushing Hammer).</summary>
        Disruption = 1 << 9,
        Heal = 1 << 10,
        /// <summary>Discards Energy from the opponent's Pokémon.</summary>
        EnergyRemoval = 1 << 11,
        /// <summary>Discards Pokémon Tools.</summary>
        ToolRemoval = 1 << 12,
        /// <summary>Turns off Abilities (Path to the Peak).</summary>
        AbilityLock = 1 << 13,
        /// <summary>Stops the opponent playing Items (Budew's Itchy Pollen).</summary>
        ItemLock = 1 << 14,
        /// <summary>Evolves a Basic straight into a Stage 2 (Rare Candy).</summary>
        RareCandy = 1 << 15
    }

    /// <summary>
    /// Reads a card's roles from its text: a Trainer's or Energy's text, a Pokémon's Abilities (OfAttacks reads its attacks).
    /// The texts are matched one sentence at a time with a few patterns ("search your deck for ... Pokémon ... put",
    /// "draw 3 cards", "Switch in 1 of your opponent's Benched Pokémon"...). Results are cached by printing.
    /// </summary>
    public static class CardRoles
    {
        public static readonly CardRole[] All =
        {
            CardRole.Draw, CardRole.PokemonSearch, CardRole.EnergySearch, CardRole.TrainerSearch, CardRole.Tutor, CardRole.Gust,
            CardRole.Switch, CardRole.Recovery, CardRole.EnergyAccel, CardRole.Disruption, CardRole.Heal, CardRole.EnergyRemoval,
            CardRole.ToolRemoval, CardRole.AbilityLock, CardRole.ItemLock, CardRole.RareCandy
        };

        private static readonly object gate = new object();
        private static readonly Dictionary<string, CardRole> cache = new Dictionary<string, CardRole>(StringComparer.Ordinal);
        private static readonly Dictionary<string, CardRole> attackCache = new Dictionary<string, CardRole>(StringComparer.Ordinal);

        private const RegexOptions I = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
        private static readonly Regex Sentences = new Regex(@"(?<=[.!?])\s+");
        private static readonly Regex Searchy = new Regex(@"\b(search|searches|look through|find)\b|from your deck|from among them", I);
        private static readonly Regex Puts = new Regex(@"\b(put|puts|attach)\b|into your hand|onto your Bench", I);
        private static readonly Regex FromAmong = new Regex(@"among them", I);
        private static readonly Regex DrawWords = new Regex(@"\bdraws? (a card|cards|that many|until|up to|\d+)", I);
        private static readonly Regex OpponentDraws = new Regex(@"\b(they|your opponent|that player) draws? [^,;.]*", I);
        private static readonly Regex OpponentSubject = new Regex(@"^(only if [^:]*:\s*)?(they|your opponent)\b", I);
        private static readonly Regex PokemonWord = new Regex(@"Pokémon(?! Tool)");
        private static readonly Regex EnergyWord = new Regex(@"\bEnergy\b");
        private static readonly Regex EnergyAttached = new Regex(@"Energy attached", I);
        private static readonly Regex TrainerWord = new Regex(@"\b(Item|Supporter|Trainer|Pokémon Tool|Stadium)\b");
        private static readonly Regex AnyCard = new Regex(@"\b(a|an|1|2|3|up to \d+|\d+) cards?\b", I);
        private static readonly Regex Gust = new Regex(@"opponent's Benched Pokémon[^.]*\bActive\b|switch(es)? in 1 of your opponent's Benched|your opponent switches their Active Pokémon with 1 of their Benched", I);
        private static readonly Regex SwitchWords = new Regex(@"switch your Active Pokémon with 1 of your Benched|switches their Active Pokémon with 1 of their Benched|has no Retreat Cost|Retreat Cost is[^.]*less", I);
        private static readonly Regex Recover = new Regex(@"from your discard pile[^.]*\binto your (hand|deck)|discard pile (back )?into your (hand|deck)|from your discard pile[^.]*onto your Bench", I);
        private static readonly Regex Attach = new Regex(@"\battach\b", I);
        private static readonly Regex NotAccel = new Regex(@"opponent's|can't attach|\battach (it|this card|a Pokémon Tool)", I);
        private static readonly Regex Disrupt = new Regex(@"(your opponent|each player|both players) shuffles? (their|his or her) hands?|your opponent discards|they put their hand|your opponent reveals (their|his or her) hand|from your opponent's hand", I);
        private static readonly Regex HealWords = new Regex(@"\bheal (\d+|all)", I);
        private static readonly Regex EnergyRemove = new Regex(@"discard[^.]*\bEnergy\b[^.]*(from|attached to) (1 of )?(your opponent's|the Defending)", I);
        private static readonly Regex ToolRemove = new Regex(@"discard[^.]*Pokémon Tools? (card )?(attached|from (1 of )?your opponent)", I);
        private static readonly Regex AbilityOff = new Regex(@"have no Abilities|can't use (any )?Abilities", I);
        private static readonly Regex ItemsOff = new Regex(@"can't play (any )?Item cards", I);
        private static readonly Regex Candy = new Regex(@"Stage 2 card from your hand onto", I);

        /// <summary>Roles of a card: a Trainer's or Energy's text, or a Pokémon's Abilities.</summary>
        public static CardRole Of(CardDef d)
        {
            if (d == null) return CardRole.None;
            CardRole r;
            lock (gate)
            {
                if (cache.TryGetValue(d.Key, out r)) return r;
            }
            r = CardRole.None;
            if (d.IsPokemon)
            {
                foreach (var ab in d.Abilities) r |= FromText(ab.Text);
            }
            else
            {
                r = FromText(d.Text);
                if (d.Name == "Rare Candy") r |= CardRole.RareCandy;
            }
            lock (gate) cache[d.Key] = r;
            return r;
        }

        /// <summary>Roles of a Pokémon's attacks (Budew's Itchy Pollen locks Items).</summary>
        public static CardRole OfAttacks(CardDef d)
        {
            if (d == null || !d.IsPokemon) return CardRole.None;
            CardRole r;
            lock (gate)
            {
                if (attackCache.TryGetValue(d.Key, out r)) return r;
            }
            r = CardRole.None;
            foreach (var a in d.Attacks) r |= FromText(a.Text);
            lock (gate) attackCache[d.Key] = r;
            return r;
        }

        public static bool Has(CardDef d, CardRole role)
        {
            return (Of(d) & role) != 0;
        }

        /// <summary>The roles one text gives, sentence by sentence.</summary>
        public static CardRole FromText(string text)
        {
            var r = CardRole.None;
            if (string.IsNullOrEmpty(text)) return r;
            string prev = "";
            foreach (var raw in Sentences.Split(text))
            {
                string s = raw.Trim();
                if (s.Length == 0) continue;
                string mine = OpponentDraws.Replace(s, "");
                if (DrawWords.IsMatch(mine) && !OpponentSubject.IsMatch(s)) r |= CardRole.Draw;
                bool searchy = (Searchy.IsMatch(s) && Puts.IsMatch(s)) || (Searchy.IsMatch(prev) && FromAmong.IsMatch(s));
                if (searchy && s.IndexOf("discard pile", StringComparison.OrdinalIgnoreCase) < 0)
                {
                    bool typed = false;
                    if (PokemonWord.IsMatch(s))
                    {
                        r |= CardRole.PokemonSearch;
                        typed = true;
                    }
                    if (EnergyWord.IsMatch(s) && !EnergyAttached.IsMatch(s))
                    {
                        r |= CardRole.EnergySearch;
                        typed = true;
                    }
                    if (TrainerWord.IsMatch(s))
                    {
                        r |= CardRole.TrainerSearch;
                        typed = true;
                    }
                    if (!typed && AnyCard.IsMatch(s)) r |= CardRole.Tutor;
                }
                if (Gust.IsMatch(s)) r |= CardRole.Gust;
                if (SwitchWords.IsMatch(s)) r |= CardRole.Switch;
                if (Recover.IsMatch(s)) r |= CardRole.Recovery;
                if (Attach.IsMatch(s) && EnergyWord.IsMatch(s) && !NotAccel.IsMatch(s)) r |= CardRole.EnergyAccel;
                if (Disrupt.IsMatch(s)) r |= CardRole.Disruption;
                if (HealWords.IsMatch(s)) r |= CardRole.Heal;
                if (EnergyRemove.IsMatch(s)) r |= CardRole.EnergyRemoval | CardRole.Disruption;
                if (ToolRemove.IsMatch(s)) r |= CardRole.ToolRemoval;
                if (AbilityOff.IsMatch(s)) r |= CardRole.AbilityLock;
                if (ItemsOff.IsMatch(s)) r |= CardRole.ItemLock | CardRole.Disruption;
                if (Candy.IsMatch(s)) r |= CardRole.RareCandy;
                prev = s;
            }
            return r;
        }

        /// <summary>What a role does, for sentences like "3 cards that draw cards".</summary>
        public static string Describe(CardRole role)
        {
            switch (role)
            {
                case CardRole.Draw: return "draw cards";
                case CardRole.PokemonSearch: return "find Pokémon";
                case CardRole.EnergySearch: return "find Energy";
                case CardRole.TrainerSearch: return "find Trainers";
                case CardRole.Tutor: return "find any card";
                case CardRole.Gust: return "bring up one of your opponent's Benched Pokémon";
                case CardRole.Switch: return "switch your Active Pokémon or make retreating cheaper";
                case CardRole.Recovery: return "get cards back from the discard pile";
                case CardRole.EnergyAccel: return "attach extra Energy";
                case CardRole.Disruption: return "disrupt your opponent";
                case CardRole.Heal: return "heal";
                case CardRole.EnergyRemoval: return "discard your opponent's Energy";
                case CardRole.ToolRemoval: return "discard Pokémon Tools";
                case CardRole.AbilityLock: return "turn off Abilities";
                case CardRole.ItemLock: return "stop your opponent playing Items";
                case CardRole.RareCandy: return "evolve straight to Stage 2";
                default: return role.ToString();
            }
        }

        /// <summary>Short name of a role, for summary lines.</summary>
        public static string ShortName(CardRole role)
        {
            switch (role)
            {
                case CardRole.Draw: return "Draw";
                case CardRole.PokemonSearch: return "Pokémon search";
                case CardRole.EnergySearch: return "Energy search";
                case CardRole.TrainerSearch: return "Trainer search";
                case CardRole.Tutor: return "Any-card search";
                case CardRole.Gust: return "Gust";
                case CardRole.Switch: return "Switch";
                case CardRole.Recovery: return "Recovery";
                case CardRole.EnergyAccel: return "Energy acceleration";
                case CardRole.Disruption: return "Disruption";
                case CardRole.Heal: return "Healing";
                case CardRole.EnergyRemoval: return "Energy removal";
                case CardRole.ToolRemoval: return "Tool removal";
                case CardRole.AbilityLock: return "Ability lock";
                case CardRole.ItemLock: return "Item lock";
                case CardRole.RareCandy: return "Rare Candy";
                default: return role.ToString();
            }
        }
    }
}
