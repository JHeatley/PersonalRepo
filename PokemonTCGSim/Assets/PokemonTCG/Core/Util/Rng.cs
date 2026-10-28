using System;
using System.Collections.Generic;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Small deterministic RNG (xorshift64*). Same seed gives the same game on every platform,
    /// which is what makes replays, bug reports ("seed 1234 crashes") and AI search possible.
    /// </summary>
    public sealed class Rng
    {
        private ulong state;

        public Rng(int seed)
        {
            state = ((ulong)(uint)seed + 1UL) * 0x9E3779B97F4A7C15UL;
            if (state == 0) state = 0x2545F4914F6CDD1DUL;
            for (int i = 0; i < 4; i++) NextULong();
        }

        private Rng(ulong rawState, bool raw)
        {
            state = rawState;
        }

        public ulong NextULong()
        {
            state ^= state >> 12;
            state ^= state << 25;
            state ^= state >> 27;
            return state * 2685821657736338717UL;
        }

        /// <summary>Uniform integer in [0, maxExclusive).</summary>
        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0) throw new ArgumentOutOfRangeException("maxExclusive");
            return (int)((NextULong() >> 1) % (ulong)maxExclusive);
        }

        public bool CoinFlip()
        {
            return (NextULong() >> 63) == 1UL;
        }

        public void Shuffle<T>(IList<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Next(i + 1);
                T tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }

        public Rng Clone()
        {
            return new Rng(state, true);
        }
    }
}
