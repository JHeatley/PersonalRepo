using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// A card's printed stats as the policy network reads them (PolicyNet.StaticSize numbers). The first 64 are the Python
    /// bot's _card_static layout: kind, stage, HP, types, Weakness, Resistance, Retreat Cost, rule box, attacks (counted
    /// together), Abilities, Trainer kind and Energy type. From 64 on, each of a Pokémon's first two attacks on its own: its
    /// cost by type, its damage, and what its text does with Energy and HP (AttackStats); then what its Abilities do
    /// (OtherStart: attaching or moving Energy, more HP, healing, Energy on your other Pokémon, locking the opponent's
    /// Items); then, for Energy and Tool cards, what their text does: more HP, a bonus only on one type of Pokémon, any type
    /// of Energy, less damage (ExtraStart).
    /// </summary>
    public static class PolicyCards
    {
        /// <summary>The Python bot's type order: Grass, Fire, Water, Lightning, Psychic, Fighting, Darkness, Metal, Dragon, Colorless.</summary>
        public static int TypeIndex(EnergyType t)
        {
            switch (t)
            {
                case EnergyType.Grass: return 0;
                case EnergyType.Fire: return 1;
                case EnergyType.Water: return 2;
                case EnergyType.Lightning: return 3;
                case EnergyType.Psychic: return 4;
                case EnergyType.Fairy: return 4; // no Fairy type in the Python bot's pool; closest is Psychic
                case EnergyType.Fighting: return 5;
                case EnergyType.Darkness: return 6;
                case EnergyType.Metal: return 7;
                case EnergyType.Dragon: return 8;
                default: return 9;
            }
        }

        public static string TypeName(EnergyType t)
        {
            return t.ToString();
        }

        public static bool HasRuleBox(CardDef c)
        {
            return c.IsEx || c.IsMegaEx || c.IsPokemonEX || c.IsGX || c.IsPokemonV || c.IsBreak || c.IsLegend || c.IsLevelUp
                   || c.IsOldEx || c.IsRadiant;
        }

        /// <summary>
        /// Where each attack's numbers start and how many there are, where the Abilities' begin (OtherStart, 8 numbers), where
        /// an Energy or Tool card's begin (ExtraStart, 24), where a Pokémon's play and Prize numbers begin (PlayStart, 16) and
        /// where any card's protection and Ability-stopping numbers begin (GuardStart, 8).
        /// </summary>
        public const int AttackStart = 64, AttackStride = 28, OtherStart = AttackStart + 2 * AttackStride, ExtraStart = OtherStart + 8,
            PlayStart = ExtraStart + 24, GuardStart = PlayStart + 16;

        /// <summary>The Python bot's type names in TypeIndex order, as card text writes them ("Grass Pokémon", "Water Energy").</summary>
        private static readonly string[] TypeWords = { "Grass", "Fire", "Water", "Lightning", "Psychic", "Fighting", "Darkness", "Metal", "Dragon" };

        /// <summary>The type a text names (as "Grass Pokémon" or "Water Energy"; Fairy counts as Psychic), or -1.</summary>
        public static int NamedType(string text, string after)
        {
            for (int i = 0; i < TypeWords.Length; i++)
                if (Has(text, TypeWords[i] + " " + after)) return i;
            if (Has(text, "Fairy " + after)) return TypeIndex(EnergyType.Fairy);
            return -1;
        }

        /// <summary>
        /// An Energy or Tool card's numbers: special Energy, raises HP, a bonus only on one type of Pokémon (and which type),
        /// provides any type, less damage taken, discards itself.
        /// </summary>
        private static void ExtraStats(CardDef c, float[] f)
        {
            string text = c.Text ?? "";
            int o = ExtraStart;
            f[o] = c.IsEnergy && !c.IsBasicEnergy ? 1 : 0;
            f[o + 1] = RaisesHp(text) ? 1 : 0;
            int type = NamedType(text, "Pok");
            if (type >= 0)
            {
                f[o + 2] = 1;
                f[o + 3 + type] = 1;
            }
            f[o + 13] = Has(text, "every type") || Has(text, "any type") ? 1 : 0;
            f[o + 14] = Has(text, "damage") && (Has(text, "less damage") || Has(text, "prevent") || Has(text, "reduce")) ? 1 : 0;
            f[o + 15] = Has(text, "discard this card") ? 1 : 0;
        }

        /// <summary>
        /// A Pokémon's play numbers: an Ability that works when you play it from your hand (Meowth ex's Last-Ditch Catch) and
        /// whether it finds a Supporter, finds or draws other cards, or works when it evolves; an Ability used once a turn, only
        /// in the Active Spot or only on the Bench; and the Prize cards it gives up when Knocked Out.
        /// </summary>
        private static void PlayStats(CardDef c, float[] f)
        {
            int o = PlayStart;
            foreach (var a in c.Abilities)
            {
                string text = a.Text ?? "";
                if (OnPlay(text))
                {
                    f[o] = 1;
                    if (Has(text, "Supporter")) f[o + 1] = 1;
                    if (Has(text, "search") || Has(text, "draw") || Has(text, "look at")) f[o + 2] = 1;
                    if (Has(text, "evolve")) f[o + 6] = 1;
                }
                if (Has(text, "once during your turn")) f[o + 3] = 1;
                if (Has(text, "on your Bench") || Has(text, "is Benched")) f[o + 4] = 1;
                if (Has(text, "Active Spot")) f[o + 5] = 1;
            }
            f[o + 7] = Math.Min(c.PrizeValue, 3) / 3f;
        }

        /// <summary>True when an Ability works as the Pokémon is played from your hand ("When you play this Pokémon from your hand onto your Bench").</summary>
        public static bool OnPlay(string text)
        {
            text = text ?? "";
            return Has(text, "when you play") && Has(text, "from your hand");
        }

        /// <summary>What a card's text says, worked out once per card (reading text on every decision would slow training down).</summary>
        private sealed class Facts
        {
            public bool PlaysFromHand, TeamEnergy, MentionsEnergy, LocksItems, PreventsEffects, Spreads, HasAbility;
            public int BonusType;
            /// <summary>Its Abilities prevent all damage from attacks by: 1 Pokémon ex, 2 Pokémon with an Ability, 4 Basic Pokémon, 8 Pokémon V.</summary>
            public int Shield;
        }

        /// <summary>The same for an attack's text.</summary>
        private sealed class AttackFacts
        {
            public bool LocksItems, IgnoresEffects, IgnoresWeakness, PlacesCounters;
        }

        private static readonly ConditionalWeakTable<CardDef, Facts> facts = new ConditionalWeakTable<CardDef, Facts>();
        private static readonly ConditionalWeakTable<AttackDef, AttackFacts> attackFacts = new ConditionalWeakTable<AttackDef, AttackFacts>();

        private static Facts Of(CardDef c)
        {
            return facts.GetValue(c, card =>
            {
                var f = new Facts { BonusType = NamedType(card.Text ?? "", "Pok"), PreventsEffects = PreventsEffects(card.Text) };
                foreach (var a in card.Attacks)
                {
                    string t = a.Text ?? "";
                    if (TeamEnergy(t)) f.TeamEnergy = true;
                    if (Has(t, "energy")) f.MentionsEnergy = true;
                    if (LocksItems(t)) f.LocksItems = true;
                    if (PlacesCounters(t)) f.Spreads = true;
                }
                foreach (var a in card.Abilities)
                {
                    string t = a.Text ?? "";
                    if (a.IsAbility) f.HasAbility = true;
                    if (card.IsPokemon && OnPlay(t)) f.PlaysFromHand = true;
                    if (TeamEnergy(t)) f.TeamEnergy = true;
                    if (Has(t, "energy")) f.MentionsEnergy = true;
                    if (LocksItems(t)) f.LocksItems = true;
                    if (PreventsDamage(t))
                    {
                        if (Has(t, "Pokémon ex")) f.Shield |= 1;
                        else if (Has(t, "have an Ability")) f.Shield |= 2;
                        else if (Has(t, "Basic Pokémon")) f.Shield |= 4;
                        else if (Has(t, "Pokémon V")) f.Shield |= 8;
                    }
                }
                return f;
            });
        }

        private static AttackFacts Of(AttackDef a)
        {
            return attackFacts.GetValue(a, attack =>
            {
                string t = attack.Text ?? "";
                return new AttackFacts
                {
                    LocksItems = LocksItems(t),
                    IgnoresEffects = IgnoresEffects(t),
                    IgnoresWeakness = Has(t, "Weakness") && Has(t, "Resistance") && (Has(t, "don't") || Has(t, "isn't") || Has(t, "not ")),
                    PlacesCounters = PlacesCounters(t)
                };
            });
        }

        public static bool AttackLocksItems(AttackDef a) { return a != null && Of(a).LocksItems; }
        public static bool AttackIgnoresEffects(AttackDef a) { return a != null && Of(a).IgnoresEffects; }
        public static bool AttackIgnoresWeakness(AttackDef a) { return a != null && Of(a).IgnoresWeakness; }
        /// <summary>True when the card has an attack that puts damage counters on Pokémon (spread).</summary>
        public static bool Spreads(CardDef c) { return c != null && c.IsPokemon && Of(c).Spreads; }
        /// <summary>The kinds of attacker its Abilities prevent all damage from (Facts.Shield bits).</summary>
        public static int Shield(CardDef c) { return c == null ? 0 : Of(c).Shield; }
        public static bool HasAbility(CardDef c) { return c != null && Of(c).HasAbility; }
        /// <summary>True when the card's own text stops the effects of attacks on its Pokémon (Mist Energy).</summary>
        public static bool CardPreventsEffects(CardDef c) { return c != null && Of(c).PreventsEffects; }

        /// <summary>True when the card has an Ability that works as it's played from your hand.</summary>
        public static bool PlaysFromHand(CardDef c)
        {
            return c != null && c.IsPokemon && Of(c).PlaysFromHand;
        }

        /// <summary>The type an Energy card's bonus needs its Pokémon to be ("attached to a Grass Pokémon"), or -1 for none.</summary>
        public static int BonusType(CardDef energy)
        {
            return energy == null ? -1 : Of(energy).BonusType;
        }

        /// <summary>
        /// True when an attack or Ability uses Energy on your other Pokémon too ("discard any amount of Water Energy from your
        /// Pokémon", "move an Energy from 1 of your Benched Pokémon"): then Energy on any of your Pokémon can count.
        /// </summary>
        public static bool UsesTeamEnergy(CardDef c)
        {
            return c != null && Of(c).TeamEnergy;
        }

        /// <summary>
        /// Energy already on your other Pokémon put to use: discarded (Hail Blade), moved, or counted (Storm Emerald: "for each
        /// Fire Energy ... attached to all of your Pokémon"). Attaching Energy to them (Infernal Reign) doesn't count: that
        /// doesn't make Energy already there worth more.
        /// </summary>
        private static bool TeamEnergy(string text)
        {
            text = text ?? "";
            return Has(text, "energy") && (Has(text, "your Pok") || Has(text, "your Benched") || Has(text, "each of your") || Has(text, "all of your"))
                   && (Has(text, "discard") || Has(text, "move") || Has(text, "for each") || Has(text, "amount of"));
        }

        /// <summary>
        /// One attack's numbers: its cost by type (Colorless last; 0.25 each), damage, a "+" or "×" damage, and what its text
        /// does: more damage for each Energy, discards Energy, attaches Energy, more damage tied to Energy, heals, has an effect,
        /// is a VSTAR Power, uses Energy on your other Pokémon, names a type of Energy, counts something "for each", locks the
        /// opponent's Items.
        /// </summary>
        private static void AttackStats(AttackDef a, float[] f, int o)
        {
            if (a.Cost != null) foreach (var t in a.Cost) f[o + TypeIndex(t)] += 0.25f;
            f[o + 10] = a.Damage / 300f;
            string suffix = a.DamageSuffix ?? "";
            f[o + 11] = suffix.Contains("+") ? 1 : 0;
            f[o + 12] = suffix.Contains("×") || suffix.Contains("x") ? 1 : 0;
            string text = a.Text ?? "";
            f[o + 13] = Has(text, "for each") && Has(text, "energy") ? 1 : 0;
            f[o + 14] = Has(text, "discard") && Has(text, "energy") ? 1 : 0;
            f[o + 15] = Has(text, "attach") && Has(text, "energy") ? 1 : 0;
            f[o + 16] = Has(text, "more damage") && Has(text, "energy") ? 1 : 0;
            f[o + 17] = Has(text, "heal") ? 1 : 0;
            f[o + 18] = text.Trim().Length > 0 ? 1 : 0;
            f[o + 19] = a.VstarPower ? 1 : 0;
            f[o + 20] = TeamEnergy(text) ? 1 : 0;
            f[o + 21] = NamedType(text, "Energy") >= 0 ? 1 : 0;
            f[o + 22] = Has(text, "for each") ? 1 : 0;
            f[o + 23] = LocksItems(text) ? 1 : 0;
            f[o + 24] = IgnoresEffects(text) ? 1 : 0;
            f[o + 25] = Has(text, "Weakness") && Has(text, "Resistance") && (Has(text, "don't") || Has(text, "isn't") || Has(text, "not")) ? 1 : 0;
            f[o + 26] = Has(text, "Benched Pok") && Has(text, "damage") ? 1 : 0;
            f[o + 27] = PlacesCounters(text) ? 1 : 0;
        }

        /// <summary>True when an attack puts damage counters on Pokémon (Phantom Dive): an effect, which Mist Energy stops.</summary>
        public static bool PlacesCounters(string text)
        {
            return Has(text ?? "", "damage counter");
        }

        /// <summary>
        /// True when a card stops the effects of the opponent's attacks on its Pokémon (Mist Energy, Rocky Fighting Energy on a
        /// Fighting Pokémon, Wash Water Energy): damage counters an attack puts on it are such an effect; its damage isn't.
        /// </summary>
        public static bool PreventsEffects(string text)
        {
            text = text ?? "";
            return Has(text, "Prevent all effects of") && Has(text, "attacks");
        }

        /// <summary>
        /// True when an attack's damage gets past effects on the defender ("not affected by any effects on the opponent's Active
        /// Pokémon", "any effects on your opponent's Active Pokémon don't change this attack's damage"): Crustle's Superb
        /// Scissors, Cornerstone Mask Ogerpon ex's Demolish.
        /// </summary>
        public static bool IgnoresEffects(string text)
        {
            text = text ?? "";
            return Has(text, "effects on") && Has(text, "Active") && (Has(text, "not affected") || Has(text, "isn't affected") || Has(text, "don't change") || Has(text, "doesn't change"));
        }

        /// <summary>
        /// A card's protection numbers (from GuardStart): an Ability that prevents all damage from attacks, and from whom (Pokémon
        /// ex, Pokémon with an Ability, Basic Pokémon, Pokémon V), less damage taken, no effects of attacks; and text that turns
        /// Abilities off ("Evolution Pokémon in play have no Abilities": Legendary Pyroduct).
        /// </summary>
        private static void GuardStats(CardDef c, float[] f)
        {
            int o = GuardStart;
            var texts = new List<string> { c.Text ?? "" };
            foreach (var a in c.Abilities) texts.Add(a.Text ?? "");
            foreach (var a in c.Attacks) if (Has(a.Text ?? "", "no Abilities")) f[o + 5] = 1;
            for (int i = 0; i < texts.Count; i++)
            {
                string text = texts[i];
                if (i > 0 && PreventsDamage(text))
                {
                    f[o] = 1;
                    if (Has(text, "Pokémon ex")) f[o + 1] = 1;
                    if (Has(text, "have an Ability")) f[o + 2] = 1;
                    if (Has(text, "Basic Pokémon")) f[o + 3] = 1;
                    if (Has(text, "Pokémon V")) f[o + 4] = 1;
                }
                if (Has(text, "no Abilities") || (Has(text, "Abilities") && Has(text, "can't be used"))) f[o + 5] = 1;
                if (Has(text, "less damage") || Has(text, "damage done to this Pokémon") && Has(text, "-")) f[o + 6] = 1;
                if (PreventsEffects(text)) f[o + 7] = 1;
            }
        }

        /// <summary>True when an Ability prevents all damage done to its Pokémon by attacks (perhaps only from some attackers).</summary>
        public static bool PreventsDamage(string text)
        {
            return Has(text ?? "", "Prevent all damage done to this Pok") && Has(text ?? "", "by attacks");
        }

        /// <summary>True when the text stops the opponent playing Items ("they can't play any Item cards from their hand").</summary>
        public static bool LocksItems(string text)
        {
            text = text ?? "";
            return Has(text, "Item") && (Has(text, "can't play") || Has(text, "cannot play"));
        }

        /// <summary>True when one of the card's attacks or Abilities locks the opponent's Items.</summary>
        public static bool LocksItems(CardDef c)
        {
            return c != null && Of(c).LocksItems;
        }

        private static bool Has(string text, string word)
        {
            return text.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool RaisesHp(string text)
        {
            return Has(text, "HP") && (text.Contains("+") || Has(text, "more HP") || Has(text, "maximum HP"));
        }

        /// <summary>
        /// True when the card's attack or Ability text does something with Energy beyond paying a cost: more damage for each
        /// Energy, discarding it, attaching or moving it. Extra Energy on such a Pokémon isn't wasted (PolicyPlan.Wasted).
        /// </summary>
        public static bool MentionsEnergy(CardDef c)
        {
            return c != null && Of(c).MentionsEnergy;
        }

        public static float[] Static(CardDef c)
        {
            var f = new float[PolicyNet.StaticSize];
            if (c == null) return f;
            GuardStats(c, f);
            f[0] = c.IsPokemon ? 1 : 0;
            f[1] = c.IsTrainer ? 1 : 0;
            f[2] = c.IsEnergy ? 1 : 0;
            if (c.IsPokemon)
            {
                int stage = c.Stage == Stage.Stage2 ? 2 : c.Stage == Stage.Stage1 ? 1 : 0;
                f[3 + stage] = 1;
                f[6] = c.Hp / 300f;
                f[7 + TypeIndex(c.PokemonType)] = 1;
                if (c.SecondType.HasValue) f[7 + TypeIndex(c.SecondType.Value)] = 1;
                if (c.Weakness.HasValue) f[17 + TypeIndex(c.Weakness.Value)] = 1;
                if (c.SecondWeakness.HasValue) f[17 + TypeIndex(c.SecondWeakness.Value)] = 1;
                if (c.Resistance.HasValue) f[27 + TypeIndex(c.Resistance.Value)] = 1;
                if (c.SecondResistance.HasValue) f[27 + TypeIndex(c.SecondResistance.Value)] = 1;
                f[37] = c.RetreatCost / 4f;
                f[38] = c.IsEx || c.IsPokemonEX ? 1 : 0;
                f[39] = c.IsMegaEx || c.IsOldMega ? 1 : 0;
                f[40] = c.IsTera ? 1 : 0;
                f[41] = HasRuleBox(c) ? 1 : 0;
                var attacks = c.Attacks;
                f[42] = attacks.Count / 2f;
                if (attacks.Count > 0)
                {
                    int maxBase = 0, minCost = int.MaxValue, maxCost = 0;
                    bool mod = false;
                    foreach (var a in attacks)
                    {
                        maxBase = Math.Max(maxBase, a.Damage);
                        int n = a.Cost == null ? 0 : a.Cost.Length;
                        minCost = Math.Min(minCost, n);
                        maxCost = Math.Max(maxCost, n);
                        if (!string.IsNullOrEmpty(a.DamageSuffix)) mod = true;
                    }
                    f[43] = maxBase / 300f;
                    f[44] = minCost / 4f;
                    f[45] = maxCost / 4f;
                    f[46] = mod ? 1 : 0;
                }
                int abilities = 0;
                foreach (var a in c.Abilities) if (a.IsAbility) abilities++;
                f[47] = abilities;
                for (int k = 0; k < Math.Min(2, attacks.Count); k++) AttackStats(attacks[k], f, AttackStart + k * AttackStride);
                f[OtherStart] = attacks.Count > 2 ? 1 : 0;
                foreach (var a in c.Abilities)
                {
                    string text = a.Text ?? "";
                    if (Has(text, "attach") && Has(text, "energy")) f[OtherStart + 1] = 1;
                    if (Has(text, "move") && Has(text, "energy")) f[OtherStart + 2] = 1;
                    if (RaisesHp(text)) f[OtherStart + 3] = 1;
                    if (Has(text, "heal")) f[OtherStart + 4] = 1;
                }
                f[OtherStart + 5] = MentionsEnergy(c) ? 1 : 0;
                f[OtherStart + 6] = UsesTeamEnergy(c) ? 1 : 0;
                foreach (var a in c.Abilities) if (LocksItems(a.Text)) f[OtherStart + 7] = 1;
                PlayStats(c, f);
            }
            else if (c.IsTrainer)
            {
                switch (c.TrainerKind)
                {
                    case TrainerKind.Item: f[48] = 1; break;
                    case TrainerKind.Supporter: f[49] = 1; break;
                    case TrainerKind.Stadium: f[50] = 1; break;
                    case TrainerKind.Tool: f[51] = 1; break;
                }
                f[52] = c.IsAceSpec ? 1 : 0;
                if (c.TrainerKind == TrainerKind.Tool) ExtraStats(c, f);
            }
            else
            {
                f[53] = c.IsBasicEnergy ? 1 : 0;
                if (c.IsBasicEnergy) f[54 + TypeIndex(c.EnergyProvides)] = 1;
                ExtraStats(c, f);
            }
            return f;
        }
    }
}
