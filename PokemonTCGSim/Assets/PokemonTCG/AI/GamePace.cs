using System;
using System.Threading;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Keeps training to a number of games a second (the Train screen's "Games per second"), so the CPU has room to spare:
    /// each game waits for its turn to start, the turns PerSecond apart, on any number of threads. Time spent learning
    /// isn't made up afterwards with a burst of games: a turn that has passed is simply gone.
    /// </summary>
    public sealed class GamePace
    {
        /// <summary>Games a second at most (0 or less: no limit).</summary>
        public readonly double PerSecond;
        private readonly object gate = new object();
        private long nextTicks;

        public GamePace(double perSecond)
        {
            PerSecond = perSecond;
        }

        /// <summary>Waits until this game may start (returns at once without a limit, or when stop says so).</summary>
        public void Wait(Func<bool> stop)
        {
            if (PerSecond <= 0) return;
            long slot;
            lock (gate)
            {
                long now = DateTime.UtcNow.Ticks;
                if (nextTicks < now) nextTicks = now;
                slot = nextTicks;
                nextTicks += (long)(TimeSpan.TicksPerSecond / PerSecond);
            }
            while (true)
            {
                long wait = (slot - DateTime.UtcNow.Ticks) / TimeSpan.TicksPerMillisecond;
                if (wait <= 0 || (stop != null && stop())) return;
                Thread.Sleep((int)Math.Min(50, wait));
            }
        }
    }
}
