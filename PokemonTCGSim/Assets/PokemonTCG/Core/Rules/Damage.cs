using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>What kind of card effect is currently resolving. Used for "prevent all effects of attacks" and similar.</summary>
    public enum EffectSourceKind
    {
        None,
        Attack,
        Ability,
        Trainer,
        /// <summary>A Special Energy card's effect.</summary>
        Energy,
        Stadium,
        /// <summary>Game rules: Pokémon Checkup, end-of-turn upkeep.</summary>
        Rule,
        /// <summary>A Pokémon Power, Poké-Power or Poké-Body (older cards). Not an Ability: effects about Abilities don't touch it.</summary>
        PokemonPower
    }

    /// <summary>One instance of attack damage moving through the pipeline.</summary>
    public sealed class DamageInfo
    {
        public PokemonInPlay Attacker;
        public PokemonInPlay Target;
        public bool TargetIsActive;
        public bool ApplyWeakness = true;
        public bool ApplyResistance = true;
        /// <summary>"This attack's damage isn't affected by any effects on your opponent's Active Pokémon."</summary>
        public bool IgnoreDefenderEffects;
        /// <summary>Weakness and Resistance apply even though the target is on the Bench (Electabuzz's Lightning Bolt: "Apply Weakness and Resistance").</summary>
        public bool WeaknessAndResistanceOnBench;
        public AttackDef Attack;
        private HashSet<string> applied;

        public bool AgainstOpponent { get { return Attacker != null && Attacker.Owner != Target.Owner; } }

        /// <summary>Attack damage to the opponent's Active Pokémon (what "does X more damage to your opponent's Active Pokémon" boosts).</summary>
        public bool ToOpponentsActive { get { return AgainstOpponent && TargetIsActive; } }

        /// <summary>For "The effect of X doesn't stack": returns true the first time it's called with this name for this damage.</summary>
        public bool ApplyOnce(string effectName)
        {
            if (applied == null) applied = new HashSet<string>();
            return applied.Add(effectName);
        }
    }

    /// <summary>
    /// Energy is tracked as "units": each unit is a bit mask of the types it can pay for.
    /// A basic Psychic Energy is one unit {Psychic}; Prism Energy on a Basic is one unit {every type};
    /// Neo Upper Energy on a Stage 2 is two units {every type}; a Colorless unit is {Colorless}.
    /// </summary>
    public static class EnergyMask
    {
        public const int Colorless = 1;
        /// <summary>Every type (Grass through Fairy). Any unit can also pay a Colorless cost.</summary>
        public const int AnyType = 0x7FE;

        public static int Of(EnergyType t)
        {
            return 1 << (int)t;
        }

        public static bool Provides(int unit, EnergyType t)
        {
            if (t == EnergyType.Colorless) return unit != 0;
            return (unit & Of(t)) != 0;
        }

        /// <summary>True when the units can pay the cost (a small exact matching search).</summary>
        public static bool CanPay(IList<int> units, IList<EnergyType> cost)
        {
            if (cost.Count == 0) return true;
            if (units.Count < cost.Count) return false;
            var specific = new List<EnergyType>();
            int colorless = 0;
            foreach (var t in cost)
            {
                if (t == EnergyType.Colorless) colorless++;
                else specific.Add(t);
            }
            var used = new bool[units.Count];
            return Assign(units, specific, 0, used, colorless);
        }

        private static bool Assign(IList<int> units, List<EnergyType> specific, int i, bool[] used, int colorless)
        {
            if (i == specific.Count)
            {
                int free = 0;
                for (int k = 0; k < used.Length; k++) if (!used[k]) free++;
                return free >= colorless;
            }
            int bit = Of(specific[i]);
            for (int k = 0; k < units.Count; k++)
            {
                if (used[k] || (units[k] & bit) == 0) continue;
                used[k] = true;
                if (Assign(units, specific, i + 1, used, colorless)) return true;
                used[k] = false;
            }
            return false;
        }

        /// <summary>How many more Energy the cost needs (0 when payable). Approximate for planning (bots).</summary>
        public static int Missing(IList<int> units, IList<EnergyType> cost)
        {
            if (CanPay(units, cost)) return 0;
            var list = new List<int>(units);
            int added = 0;
            // Add the cheapest helpful unit until payable: a unit of each still-unpaid specific type, then Colorless.
            for (int guard = 0; guard < 12 && !CanPay(list, cost); guard++)
            {
                bool progressed = false;
                foreach (var t in cost)
                {
                    if (t == EnergyType.Colorless) continue;
                    var trial = new List<int>(list) { Of(t) };
                    var needWithout = CountUnpaidSpecific(list, cost);
                    if (CountUnpaidSpecific(trial, cost) < needWithout)
                    {
                        list = trial;
                        progressed = true;
                        break;
                    }
                }
                if (!progressed) list.Add(Colorless);
                added++;
            }
            return added;
        }

        private static int CountUnpaidSpecific(IList<int> units, IList<EnergyType> cost)
        {
            var used = new bool[units.Count];
            int unpaid = 0;
            foreach (var t in cost)
            {
                if (t == EnergyType.Colorless) continue;
                int bit = Of(t);
                int found = -1;
                for (int k = 0; k < units.Count; k++) if (!used[k] && (units[k] & bit) != 0) { found = k; break; }
                if (found < 0) unpaid++;
                else used[found] = true;
            }
            return unpaid;
        }
    }
}
