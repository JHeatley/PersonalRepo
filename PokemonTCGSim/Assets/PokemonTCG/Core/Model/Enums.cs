using System;

namespace PokemonTCG.Core
{
    public enum EnergyType
    {
        Colorless,
        Grass,
        Fire,
        Water,
        Lightning,
        Psychic,
        Fighting,
        Darkness,
        Metal,
        Dragon,
        /// <summary>XY-era Fairy type (M Gardevoir-EX). No Basic Fairy Energy in the pool; any-type Energy pays for it.</summary>
        Fairy
    }

    public enum CardKind
    {
        Pokemon,
        Trainer,
        Energy
    }

    public enum Stage
    {
        None,
        Basic,
        Stage1,
        Stage2,
        /// <summary>
        /// XY-era Restored Pokémon (Steam Siege's Shieldon and Anorith, v92): not Basic Pokémon, so they can't be played from the hand
        /// or put down at setup; only their Fossil Item (or another effect) puts them onto the Bench. They evolve normally.
        /// </summary>
        Restored
    }

    public enum TrainerKind
    {
        None,
        Item,
        Supporter,
        Stadium,
        Tool
    }

    [Flags]
    public enum SpecialCondition
    {
        None = 0,
        Asleep = 1,
        Confused = 2,
        Paralyzed = 4,
        Poisoned = 8,
        Burned = 16
    }

    public static class EnergyTypes
    {
        /// <summary>Parses one energy symbol letter as used on card data: G R W L P F D M N Y C (Y = Fairy).</summary>
        public static EnergyType FromSymbol(char c)
        {
            switch (char.ToUpperInvariant(c))
            {
                case 'G': return EnergyType.Grass;
                case 'R': return EnergyType.Fire;
                case 'W': return EnergyType.Water;
                case 'L': return EnergyType.Lightning;
                case 'P': return EnergyType.Psychic;
                case 'F': return EnergyType.Fighting;
                case 'D': return EnergyType.Darkness;
                case 'M': return EnergyType.Metal;
                case 'N': return EnergyType.Dragon;
                case 'Y': return EnergyType.Fairy;
                case 'C': return EnergyType.Colorless;
                default: throw new ArgumentException("Unknown energy symbol '" + c + "'");
            }
        }

        public static char ToSymbol(EnergyType t)
        {
            switch (t)
            {
                case EnergyType.Grass: return 'G';
                case EnergyType.Fire: return 'R';
                case EnergyType.Water: return 'W';
                case EnergyType.Lightning: return 'L';
                case EnergyType.Psychic: return 'P';
                case EnergyType.Fighting: return 'F';
                case EnergyType.Darkness: return 'D';
                case EnergyType.Metal: return 'M';
                case EnergyType.Dragon: return 'N';
                case EnergyType.Fairy: return 'Y';
                default: return 'C';
            }
        }

        /// <summary>Parses a cost string such as "RP" or "CCC". An empty or null string means a free attack.</summary>
        public static EnergyType[] ParseCost(string cost)
        {
            if (string.IsNullOrEmpty(cost)) return new EnergyType[0];
            var result = new EnergyType[cost.Length];
            for (int i = 0; i < cost.Length; i++) result[i] = FromSymbol(cost[i]);
            return result;
        }

        /// <summary>Parses a type written either as a symbol ("P") or a word ("Psychic"). Returns null for empty input.</summary>
        public static EnergyType? ParseType(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            if (text.Length == 1) return FromSymbol(text[0]);
            EnergyType parsed;
            if (Enum.TryParse(text, true, out parsed)) return parsed;
            throw new ArgumentException("Unknown type '" + text + "'");
        }
    }
}
