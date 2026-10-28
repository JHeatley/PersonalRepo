using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Keeps the whole program (training, games, Burst, everything) off some of the CPU's threads: for a PC with a faulty core,
    /// whose threads give wrong answers or crash Windows under load. The threads are numbered as in Task Manager's
    /// "Set affinity" and OCCT's "logical core" (from 0); "2, 3" is physical core 1's two threads. Windows only; it lasts
    /// until the program closes.
    /// </summary>
    public static class CpuThreads
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetProcessAffinityMask(IntPtr process, out UIntPtr processMask, out UIntPtr systemMask);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetProcessAffinityMask(IntPtr process, UIntPtr mask);

        public static bool Supported { get { return Environment.OSVersion.Platform == PlatformID.Win32NT; } }

        /// <summary>The thread numbers in text like "2, 3" or "2 3" (anything else is ignored), each once, in order.</summary>
        public static List<int> Parse(string text)
        {
            var threads = new List<int>();
            if (string.IsNullOrEmpty(text)) return threads;
            foreach (var part in text.Split(new[] { ',', ' ', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int n;
                if (int.TryParse(part.Trim(), out n) && n >= 0 && n < 64 && !threads.Contains(n)) threads.Add(n);
            }
            threads.Sort();
            return threads;
        }

        /// <summary>The CPU threads to use: every thread in all, without the skipped ones (0 when that would leave none).</summary>
        public static ulong Mask(ulong all, List<int> skip)
        {
            ulong mask = all;
            foreach (int n in skip) mask &= ~(1UL << n);
            return mask;
        }

        /// <summary>
        /// Runs this program on every CPU thread except skip (none: all of them again). Returns what happened, for the screen.
        /// </summary>
        public static string Apply(List<int> skip)
        {
            if (!Supported) return "Skipping CPU threads only works on Windows.";
            try
            {
                var self = GetCurrentProcess();
                UIntPtr current, system;
                if (!GetProcessAffinityMask(self, out current, out system)) return "Windows didn't say which CPU threads there are.";
                ulong all = system.ToUInt64();
                ulong mask = Mask(all, skip);
                if (mask == 0) return "That would skip every CPU thread; nothing was changed.";
                if (!SetProcessAffinityMask(self, new UIntPtr(mask)))
                    return "Windows refused to change the CPU threads (error " + Marshal.GetLastWin32Error() + ").";
                var used = new List<int>();
                foreach (int n in skip)
                    if ((all & (1UL << n)) != 0) used.Add(n);
                return used.Count == 0
                    ? "Using every CPU thread."
                    : "Not using CPU thread" + (used.Count == 1 ? " " : "s ") + string.Join(", ", used.ConvertAll(n => n.ToString()).ToArray()) + ".";
            }
            catch (Exception e)
            {
                return "Couldn't change the CPU threads: " + e.Message;
            }
        }
    }
}
