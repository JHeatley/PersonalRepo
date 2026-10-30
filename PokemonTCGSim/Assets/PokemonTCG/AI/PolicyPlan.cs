using System;
using System.Collections.Generic;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// What a Pokémon in play needs and threatens, worked out from the cards for the network's inputs (PolicyEncoder) and the
    /// training's wasted-Energy lesson (PolicyAgent): Energy still missing for its attacks and for its evolutions' attacks (the
    /// ones still to come, in your hand or deck), whether an Energy attachment helps any of them, and whether it can Knock Out
    /// the other side's Active Pokémon or be Knocked Out by it. Damage is the printed amount with Weakness and Resistance and
    /// the defender's damage-preventing Ability (Crustle against Pokémon ex, Cornerstone Mask Ogerpon ex against Pokémon with an
    /// Ability; not while its Abilities are off, and not against an attack that ignores effects on it): a guide rather than
    /// the exact result of every effect.
    /// </summary>
    public static class PolicyPlan
    {
        private static readonly EnergyType[] NoCost = new EnergyType[0];

        /// <summary>
        /// The Pokémon's card, then the evolutions that can still come onto it from these cards: the Stage 1s that evolve from
        /// it and the Stage 2s that evolve from those (each card once).
        /// </summary>
        public static List<CardDef> Line(CardDef top, IEnumerable<CardDef> available)
        {
            var line = new List<CardDef>();
            if (top == null) return line;
            line.Add(top);
            if (available == null) return line;
            var keys = new HashSet<string>(StringComparer.Ordinal) { top.BaseKey ?? top.Name };
            var names = new HashSet<string>(StringComparer.Ordinal) { top.Name };
            for (int depth = 0; depth < 2; depth++)
            {
                var next = new List<string>();
                foreach (var c in available)
                {
                    if (c == null || !c.IsPokemon || string.IsNullOrEmpty(c.EvolvesFrom) || !names.Contains(c.EvolvesFrom)) continue;
                    if (!keys.Add(c.BaseKey ?? c.Name)) continue;
                    line.Add(c);
                    next.Add(c.Name);
                }
                foreach (var n in next) names.Add(n);
            }
            return line;
        }

        /// <summary>Evolution cards among these that go straight onto a Pokémon called name.</summary>
        public static int EvolutionsOf(string name, IEnumerable<CardDef> cards)
        {
            int n = 0;
            if (cards == null || name == null) return 0;
            foreach (var c in cards) if (c != null && c.IsPokemon && c.EvolvesFrom == name) n++;
            return n;
        }

        public static int Missing(IList<int> units, AttackDef a)
        {
            return EnergyMask.Missing(units, a.Cost ?? NoCost);
        }

        /// <summary>Energy still missing for the card's cheapest attack (0 when one can be used now), or -1 with no attacks.</summary>
        public static int Cheapest(IList<int> units, CardDef c)
        {
            if (c == null || c.Attacks.Count == 0) return -1;
            int best = int.MaxValue;
            foreach (var a in c.Attacks) best = Math.Min(best, Missing(units, a));
            return best;
        }

        /// <summary>The card's strongest attack: the most printed damage (the later one on a tie), or null with no attacks.</summary>
        public static AttackDef Strongest(CardDef c)
        {
            AttackDef best = null;
            if (c == null) return null;
            foreach (var a in c.Attacks) if (best == null || a.Damage >= best.Damage) best = a;
            return best;
        }

        /// <summary>Energy still missing for the strongest attack among these cards (its line), or -1 with no attacks.</summary>
        public static int StrongestMissing(IList<int> units, IEnumerable<CardDef> cards)
        {
            AttackDef best = null;
            foreach (var c in cards)
            {
                var a = Strongest(c);
                if (a != null && (best == null || a.Damage >= best.Damage)) best = a;
            }
            return best == null ? -1 : Missing(units, best);
        }

        /// <summary>The most Energy any of the card's attacks costs.</summary>
        public static int MostCost(CardDef c)
        {
            int most = 0;
            if (c != null) foreach (var a in c.Attacks) most = Math.Max(most, a.Cost == null ? 0 : a.Cost.Length);
            return most;
        }

        /// <summary>The Energy unit an Energy card provides when attached (as PokemonView.EnergyUnits counts it).</summary>
        public static int UnitOf(CardDef energy)
        {
            return EnergyMask.Of(energy.EnergyProvides);
        }

        /// <summary>
        /// True when attaching this unit brings an attack of the line closer (now or after evolving), or the Retreat Cost of the
        /// Active Pokémon.
        /// </summary>
        public static bool Helps(IList<int> units, int unit, IList<CardDef> line, int retreatCost, bool active)
        {
            var more = new List<int>(units) { unit };
            foreach (var c in line)
                foreach (var a in c.Attacks)
                    if (Missing(more, a) < Missing(units, a)) return true;
            return active && units.Count < retreatCost;
        }

        /// <summary>True when attaching this unit makes one of the card's attacks usable that wasn't before.</summary>
        public static bool Completes(IList<int> units, int unit, CardDef c)
        {
            if (c == null) return false;
            var more = new List<int>(units) { unit };
            foreach (var a in c.Attacks)
                if (Missing(units, a) > 0 && Missing(more, a) == 0) return true;
            return false;
        }

        /// <summary>
        /// True when an Energy attachment does nothing: a basic Energy that helps no attack of the Pokémon or its evolutions to come
        /// (nor the Active Pokémon's retreat), on a line whose cards don't do anything else with Energy (more damage for each,
        /// discarding it, moving it), in a deck with no attack or Ability that uses Energy on your other Pokémon (Hail Blade).
        /// </summary>
        public static bool Wasted(IList<int> units, CardDef energy, IList<CardDef> line, int retreatCost, bool active, bool teamUsesEnergy)
        {
            if (energy == null || !energy.IsBasicEnergy || teamUsesEnergy) return false;
            foreach (var c in line) if (PolicyCards.MentionsEnergy(c)) return false;
            return !Helps(units, UnitOf(energy), line, retreatCost, active);
        }

        /// <summary>True when any card in the deck has an attack or Ability that uses Energy on your other Pokémon.</summary>
        public static bool TeamUsesEnergy(IEnumerable<CardDef> deck)
        {
            if (deck == null) return false;
            foreach (var c in deck) if (c != null && c.IsPokemon && PolicyCards.UsesTeamEnergy(c)) return true;
            return false;
        }

        /// <summary>
        /// The most damage the attacker's attacks could do to the defender with at most extra more Energy attached: printed
        /// damage, doubled for Weakness, less 30 for Resistance. -1 when none of its attacks can be used by then.
        /// </summary>
        public static int BestDamage(PokemonView attacker, int extra, PokemonView defender)
        {
            var c = attacker != null && attacker.Top != null ? attacker.Top.Def : null;
            return c == null ? -1 : BestDamage(c, attacker.EnergyUnits, extra, defender, attacker.AbilitiesActive);
        }

        /// <summary>BestDamage for an attacker given by its card and attached Energy units (abilitiesOn: its Abilities work).</summary>
        public static int BestDamage(CardDef c, IList<int> units, int extra, PokemonView defender, bool abilitiesOn = true)
        {
            if (c == null || defender == null) return -1;
            int best = -1;
            foreach (var a in c.Attacks)
            {
                if (Missing(units, a) > extra) continue;
                best = Math.Max(best, DamageTo(c, abilitiesOn, a, defender));
            }
            return best;
        }

        /// <summary>
        /// True when the defender's Ability prevents all this attack's damage: its Abilities are working, its condition fits the
        /// attacker (a Pokémon ex, a Pokémon with a working Ability, a Basic Pokémon, a Pokémon V), and the attack doesn't ignore
        /// effects on the defender (Superb Scissors, Demolish).
        /// </summary>
        public static bool Shielded(CardDef attacker, bool abilitiesOn, AttackDef a, PokemonView defender)
        {
            if (attacker == null || a == null || defender == null) return false;
            var d = defender.Top != null ? defender.Top.Def : null;
            if (d == null || !defender.AbilitiesActive) return false;
            int shield = PolicyCards.Shield(d);
            if (shield == 0 || PolicyCards.AttackIgnoresEffects(a)) return false;
            return ((shield & 1) != 0 && (attacker.IsEx || attacker.IsMegaEx))
                   || ((shield & 2) != 0 && abilitiesOn && HasAbility(attacker))
                   || ((shield & 4) != 0 && attacker.Stage == Stage.Basic)
                   || ((shield & 8) != 0 && attacker.IsPokemonV);
        }

        /// <summary>True when every damaging attack of the attacker is stopped by the defender's protection (and it has some).</summary>
        public static bool AllShielded(CardDef attacker, bool abilitiesOn, PokemonView defender)
        {
            bool any = false;
            if (attacker == null) return false;
            foreach (var a in attacker.Attacks)
            {
                if (a.Damage <= 0) continue;
                any = true;
                if (!Shielded(attacker, abilitiesOn, a, defender)) return false;
            }
            return any;
        }

        public static bool HasAbility(CardDef c)
        {
            return PolicyCards.HasAbility(c);
        }

        /// <summary>
        /// True when a card attached to this Pokémon stops the effects of the opponent's attacks on it (Mist Energy; Rocky
        /// Fighting Energy only on a Fighting Pokémon): the damage counters an attack would put on it, for one.
        /// </summary>
        public static bool EffectsBlocked(PokemonView mon)
        {
            var top = mon.Top != null ? mon.Top.Def : null;
            foreach (var c in mon.Energy) if (c != null && Blocks(c.Def, top)) return true;
            foreach (var c in mon.Tools) if (c != null && Blocks(c.Def, top)) return true;
            return false;
        }

        /// <summary>True when this card, attached to a Pokémon of this card, stops the effects of attacks on it (its type condition met).</summary>
        public static bool Blocks(CardDef card, CardDef pokemon)
        {
            if (card == null || !PolicyCards.CardPreventsEffects(card)) return false;
            int type = PolicyCards.BonusType(card);
            return type < 0 || (pokemon != null && (PolicyCards.TypeIndex(pokemon.PokemonType) == type
                                                    || (pokemon.SecondType.HasValue && PolicyCards.TypeIndex(pokemon.SecondType.Value) == type)));
        }

        /// <summary>
        /// An attack's printed damage against this defender: 0 when its protection stops it (Shielded), else doubled for
        /// Weakness and less 30 for Resistance unless the attack ignores them (Demolish).
        /// </summary>
        public static int DamageTo(CardDef attacker, bool abilitiesOn, AttackDef a, PokemonView defender)
        {
            if (Shielded(attacker, abilitiesOn, a, defender)) return 0;
            if (PolicyCards.AttackIgnoresWeakness(a)) return a.Damage;
            return DamageTo(attacker, a.Damage, defender);
        }

        /// <summary>Printed damage against this defender: doubled for Weakness, less 30 for Resistance.</summary>
        public static int DamageTo(CardDef attacker, int damage, PokemonView defender)
        {
            if (damage <= 0) return damage;
            if (Is(attacker, defender.Weakness) || Is(attacker, defender.Weakness2)) damage *= 2;
            var d = defender.Top != null ? defender.Top.Def : null;
            if (d != null && (Is(attacker, d.Resistance) || Is(attacker, d.SecondResistance))) damage = Math.Max(0, damage - 30);
            return damage;
        }

        private static bool Is(CardDef attacker, EnergyType? type)
        {
            return type.HasValue && (attacker.PokemonType == type.Value || (attacker.SecondType.HasValue && attacker.SecondType.Value == type.Value));
        }

        /// <summary>True when the attacker could Knock Out the defender with at most extra more Energy attached.</summary>
        public static bool CanKnockOut(PokemonView attacker, int extra, PokemonView defender)
        {
            int d = BestDamage(attacker, extra, defender);
            return d > 0 && d >= defender.RemainingHp;
        }

        /// <summary>CanKnockOut for an attacker given by its card and attached Energy units.</summary>
        public static bool CanKnockOut(CardDef c, IList<int> units, int extra, PokemonView defender, bool abilitiesOn = true)
        {
            int d = BestDamage(c, units, extra, defender, abilitiesOn);
            return d > 0 && d >= defender.RemainingHp;
        }
    }
}
