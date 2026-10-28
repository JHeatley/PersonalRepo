using System;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// A card's printed stats as the policy network reads them (64 numbers, the Python bot's _card_static layout): kind,
    /// stage, HP, types, Weakness, Resistance, Retreat Cost, rule box, attacks, Abilities, Trainer kind and Energy type.
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

        public static float[] Static(CardDef c)
        {
            var f = new float[PolicyNet.StaticSize];
            if (c == null) return f;
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
            }
            else
            {
                f[53] = c.IsBasicEnergy ? 1 : 0;
                if (c.IsBasicEnergy) f[54 + TypeIndex(c.EnergyProvides)] = 1;
            }
            return f;
        }
    }
}
