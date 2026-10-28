using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Choosing moves on the GPU (expert roadmap step 3). Running the network for one decision costs the CPU most of a millisecond,
    /// about half of a training game's time; the graphics card does a few hundred decisions in about the time the CPU does one.
    /// So many games run at once (PolicyTrainer.GameThreads), each asks here instead of running the network itself, and one
    /// dispatcher thread sends what they asked for to the GPU learner together (GpuLearner.Infer) and hands each game its answer.
    /// If the learner can't be reached, the games choose on the CPU (Net.Evaluate) and the GPU is tried again a little later.
    /// </summary>
    public sealed class GpuMoves : IDisposable
    {
        public readonly PolicyNet Net;
        private readonly GpuLearner gpu;
        /// <summary>Decisions sent together at most, and how long the first one waits for others to join it.</summary>
        public int MaxBatch = 256;
        public double MaxWaitMs = 2;

        /// <summary>Decisions answered by the GPU, batches sent, and the last problem (null when fine).</summary>
        public long Decisions, Batches;
        public string LastError;

        private sealed class Request
        {
            public PolicyInput Input;
            public float[] Logits;
            public float Value;
            public bool Failed;
            public readonly ManualResetEventSlim Done = new ManualResetEventSlim(false);
        }

        private readonly object gate = new object();
        private readonly List<Request> waiting = new List<Request>();
        private readonly Thread dispatcher;
        private bool stopping;
        /// <summary>After a failure, the CPU chooses until this time; then the GPU is tried again.</summary>
        private long retryAt;

        public GpuMoves(PolicyNet net, GpuLearner gpu)
        {
            Net = net;
            this.gpu = gpu;
            dispatcher = new Thread(Dispatch) { IsBackground = true, Name = "Neural AI GPU moves" };
            dispatcher.Start();
        }

        /// <summary>True while it's choosing on the CPU after a failure (the GPU is tried again soon).</summary>
        public bool OnCpu { get { return Stopwatch.GetTimestamp() < Interlocked.Read(ref retryAt); } }

        /// <summary>The network's option scores and value for one decision, like PolicyNet.Evaluate (waits for its batch).</summary>
        public float[] Evaluate(PolicyInput x, out float value)
        {
            if (OnCpu) return Net.Evaluate(x, out value);
            var req = new Request { Input = x };
            lock (gate)
            {
                if (stopping) return Net.Evaluate(x, out value);
                waiting.Add(req);
                Monitor.PulseAll(gate);
            }
            req.Done.Wait();
            req.Done.Dispose();
            if (req.Failed) return Net.Evaluate(x, out value);
            value = req.Value;
            return req.Logits;
        }

        private void Dispatch()
        {
            var batch = new List<Request>();
            var inputs = new List<PolicyInput>();
            while (true)
            {
                batch.Clear();
                lock (gate)
                {
                    while (waiting.Count == 0 && !stopping) Monitor.Wait(gate);
                    if (stopping && waiting.Count == 0) return;
                    // The first decision waits a moment for others to join it (they come from the other games' threads).
                    var first = Stopwatch.GetTimestamp();
                    long wait = (long)(MaxWaitMs * Stopwatch.Frequency / 1000);
                    while (waiting.Count < MaxBatch && !stopping)
                    {
                        long left = wait - (Stopwatch.GetTimestamp() - first);
                        if (left <= 0) break;
                        Monitor.Wait(gate, TimeSpan.FromTicks(Math.Max(1, left * TimeSpan.TicksPerSecond / Stopwatch.Frequency)));
                    }
                    int take = Math.Min(MaxBatch, waiting.Count);
                    batch.AddRange(waiting.GetRange(0, take));
                    waiting.RemoveRange(0, take);
                }
                inputs.Clear();
                foreach (var req in batch) inputs.Add(req.Input);
                try
                {
                    float[] values;
                    var logits = gpu.Infer(Net, inputs, out values);
                    for (int i = 0; i < batch.Count; i++)
                    {
                        batch[i].Logits = logits[i];
                        batch[i].Value = values[i];
                    }
                    Interlocked.Add(ref Decisions, batch.Count);
                    Interlocked.Increment(ref Batches);
                    LastError = null;
                }
                catch (Exception e)
                {
                    // The games choose on the CPU for a while (these decisions too), then the GPU is tried again.
                    foreach (var req in batch) req.Failed = true;
                    LastError = "GPU learner: couldn't choose moves there: " + e.Message + " (choosing on the CPU; trying the GPU again in 30 seconds)";
                    Interlocked.Exchange(ref retryAt, Stopwatch.GetTimestamp() + 30 * Stopwatch.Frequency);
                }
                foreach (var req in batch) req.Done.Set();
            }
        }

        public void Dispose()
        {
            lock (gate)
            {
                stopping = true;
                Monitor.PulseAll(gate);
            }
            dispatcher.Join(5000);
            lock (gate)
            {
                foreach (var req in waiting)
                {
                    req.Failed = true;
                    req.Done.Set();
                }
                waiting.Clear();
            }
        }
    }
}
