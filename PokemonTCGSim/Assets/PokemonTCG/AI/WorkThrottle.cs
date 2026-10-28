using System;
using System.Threading;

namespace PokemonTCG.AI
{
    /// <summary>
    /// v120: how hard the background AI work (training, the Regional Sim's AI tables, deck trials) may run. The battery saver
    /// sets it: the workers rest between games (BusyShare: the share of the time they play), at most MaxThreads of them play
    /// at once, and while Paused (the app is in the background on a phone) they wait between games. Games themselves are
    /// never cut short, so nothing changes in how a game is played or learned from.
    /// </summary>
    public static class WorkThrottle
    {
        private static volatile bool paused;
        private static double busyShare = 1.0;
        private static volatile int maxThreads;
        private static readonly object gate = new object();

        /// <summary>The workers wait between games until this is turned off again.</summary>
        public static bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }

        /// <summary>The share of the time a worker plays (0.1 to 1). 0.5: after a game that took 2 seconds, it rests 2 seconds.</summary>
        public static double BusyShare
        {
            get { lock (gate) return busyShare; }
            set { lock (gate) busyShare = Math.Max(0.1, Math.Min(1.0, value)); }
        }

        /// <summary>At most this many workers play at once; 0 means no limit.</summary>
        public static int MaxThreads
        {
            get { return maxThreads; }
            set { maxThreads = Math.Max(0, value); }
        }

        /// <summary>How many workers to run when wanted would be (at least 1).</summary>
        public static int Threads(int wanted)
        {
            int cap = maxThreads;
            wanted = Math.Max(1, wanted);
            return cap > 0 ? Math.Min(wanted, cap) : wanted;
        }

        /// <summary>
        /// Call after each background game with how long it took: waits while paused, then rests so the worker plays only
        /// BusyShare of the time (at most 10 seconds' rest). Returns at once when stop says so. Never call it holding a lock.
        /// </summary>
        public static void Rest(TimeSpan busy, Func<bool> stop)
        {
            while (paused)
            {
                if (stop != null && stop()) return;
                Thread.Sleep(250);
            }
            double share = BusyShare;
            if (share >= 0.999) return;
            double ms = Math.Min(10000.0, Math.Max(0.0, busy.TotalMilliseconds) * (1.0 / share - 1.0));
            var until = DateTime.UtcNow.AddMilliseconds(ms);
            while (true)
            {
                if (stop != null && stop()) return;
                double left = (until - DateTime.UtcNow).TotalMilliseconds;
                if (left <= 0) break;
                Thread.Sleep((int)Math.Min(100.0, Math.Max(1.0, left)));
            }
            // Paused while resting: wait for the app to come back.
            while (paused)
            {
                if (stop != null && stop()) return;
                Thread.Sleep(250);
            }
        }
    }
}
