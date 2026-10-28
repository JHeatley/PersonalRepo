using System;
using System.Collections.Generic;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>Results of one round of training (a batch of games and the learning that followed).</summary>
    public sealed class PolicyRound
    {
        public int Games, LearnerWins, Draws, Errors, Decisions, Turns;
        public int VsPool, PoolWins;
        public int Mistakes;
        /// <summary>Seats the network played and learned from (2 in a game against itself).</summary>
        public int LearnerSeats;
        public double PolicyLoss, ValueLoss, Entropy, ClipFraction;
        public string LastError;
        /// <summary>Time spent playing the games and learning from them.</summary>
        public double CollectSeconds, LearnSeconds;
    }

    /// <summary>How one training game went (for the Train screen's deck A / deck B counts).</summary>
    public sealed class PolicyGame
    {
        public string LearnerDeck, OpponentDeck;
        /// <summary>Deck that won ("" for a draw), or null when the game failed.</summary>
        public string WinnerDeck;
        public int Turns;
        public string Error;
    }

    /// <summary>
    /// Trains the policy network by self-play with PPO, as the Python bot does (PokemonTCGBot/train.py): each round plays a
    /// batch of games, mostly against itself, sometimes against earlier versions of itself and the classic bot, then takes a
    /// few passes over the decisions it made. The reward is +1 for a win and -1 for a loss, plus a little for each Prize card
    /// swing, minus a little for each sequencing mistake.
    /// </summary>
    public sealed class PolicyTrainer
    {
        public readonly PolicyNet Net;
        private readonly CardScriptRegistry scripts;
        private readonly List<KeyValuePair<string, List<CardDef>>> decks;

        public int GamesPerRound = 64;
        public int Epochs = 2;
        public int Minibatch = 1024;
        public double Clip = 0.2, ValueCoef = 0.5, EntropyCoef = 0.01;
        public double Gamma = 0.999, Lambda = 0.95;
        public double PrizeReward = 0.05;
        public double SequencePenalty = 0.02;
        public double SelfShare = 0.6, PoolShare = 0.3, ClassicShare = 0.1;
        public int PoolSize = 8;
        public int SnapshotEvery = 10;
        /// <summary>Games played at once (one thread each), and threads for the learning step.</summary>
        public int Threads = Math.Max(1, Environment.ProcessorCount - 1);
        /// <summary>When set, the learner plays this deck in every game (the other seat gets a random deck).</summary>
        public string FocusDeck;
        public GameRules Rules = GameRules.Default;
        public int PrizeCount = 6;
        /// <summary>Called between games; return true to stop early (the round still learns from the games it finished).</summary>
        public Func<bool> ShouldStop;
        /// <summary>
        /// Called often; return true to stop right away (the Stop button): games in progress are abandoned and the round
        /// doesn't learn from its unfinished batch.
        /// </summary>
        public Func<bool> Abort;

        private bool Aborted { get { return Abort != null && Abort(); } }
        /// <summary>Called after each game (on the thread that played it).</summary>
        public Action<PolicyGame> GameFinished;
        /// <summary>
        /// Deck A against deck B (the Train screen): the learner plays one of the first two decks and its opponent the other,
        /// instead of random decks from the list.
        /// </summary>
        public bool Matchup;
        /// <summary>Games left to play (all rounds together); a round stops starting games when it runs out.</summary>
        public int GameBudget = int.MaxValue;

        private readonly List<PolicyNet> pool = new List<PolicyNet>();
        private int rounds;
        private int seedCounter;

        public PolicyTrainer(PolicyNet net, CardScriptRegistry scripts, List<KeyValuePair<string, List<CardDef>>> decks, int seed)
        {
            Net = net;
            this.scripts = scripts;
            this.decks = decks;
            seedCounter = seed & 0x3FFFFFFF;
        }

        private sealed class Sample
        {
            public PolicyInput Input;
            public int Action;
            public float LogP, Adv, Ret;
        }

        // ------------------------------------------------------------------ One round

        public PolicyRound Round()
        {
            var round = new PolicyRound();
            var samples = new List<Sample>();
            var clock = System.Diagnostics.Stopwatch.StartNew();
            int next = 0, finished = 0;
            object gate = new object();
            int baseSeed = Interlocked.Add(ref seedCounter, GamesPerRound * 3);
            PolicyNet[] poolNow;
            lock (pool) poolNow = pool.ToArray();
            int threads = Math.Max(1, Math.Min(Math.Min(WorkThrottle.Threads(Threads), Pool.Count), GamesPerRound));
            Log("[PokemonTCG] Neural AI round " + (rounds + 1) + ": playing " + GamesPerRound + " games on " + threads + " threads...");
            Pool.Run(t =>
            {
                if (t >= threads) return;
                {
                    while (true)
                    {
                        int g;
                        lock (gate)
                        {
                            if (next >= GamesPerRound || GameBudget <= 0 || (ShouldStop != null && ShouldStop())) return;
                            g = next++;
                            GameBudget--;
                        }
                        var started = DateTime.UtcNow;
                        var result = PlayOne(baseSeed + g * 3, poolNow);
                        if (result == null) return; // stopped mid-game
                        lock (gate)
                        {
                            Merge(round, result.Item2);
                            samples.AddRange(result.Item1);
                            finished++;
                        }
                        if (GameFinished != null) GameFinished(result.Item3);
                        WorkThrottle.Rest(DateTime.UtcNow - started, () => ShouldStop != null && ShouldStop()); // the battery saver
                    }
                }
            });
            round.CollectSeconds = clock.Elapsed.TotalSeconds;
            if (Aborted) return round;
            if (CollectGarbage) GC.Collect();
            clock.Restart();
            Log("[PokemonTCG] Neural AI round " + (rounds + 1) + ": played " + round.Games + " games in " + round.CollectSeconds.ToString("0.0") + "s; learning from "
                + samples.Count + " decisions (" + PolicyNet.MathName + ", memory " + (GC.GetTotalMemory(false) / 1000000) + " MB)...");
            if (samples.Count > 0) Learn(samples, round);
            round.LearnSeconds = clock.Elapsed.TotalSeconds;
            if (CollectGarbage) GC.Collect();
            Log("[PokemonTCG] Neural AI round " + (rounds + 1) + ": learned in " + round.LearnSeconds.ToString("0.0") + "s (" + Net.Steps + " learning steps so far"
                + (Net.SkippedSteps > 0 ? ", " + Net.SkippedSteps + " skipped" : "") + ").");
            Net.GamesTrained += round.Games;
            rounds++;
            if (SnapshotEvery > 0 && rounds % SnapshotEvery == 0)
            {
                var snap = Net.Clone();
                lock (pool)
                {
                    pool.Add(snap);
                    while (pool.Count > PoolSize) pool.RemoveAt(0);
                }
            }
            return round;
        }

        private static void Merge(PolicyRound into, PolicyRound one)
        {
            into.Games += one.Games;
            into.LearnerWins += one.LearnerWins;
            into.Draws += one.Draws;
            into.Errors += one.Errors;
            into.Decisions += one.Decisions;
            into.Turns += one.Turns;
            into.VsPool += one.VsPool;
            into.PoolWins += one.PoolWins;
            into.Mistakes += one.Mistakes;
            into.LearnerSeats += one.LearnerSeats;
            if (one.LastError != null) into.LastError = one.LastError;
        }

        private Tuple<List<Sample>, PolicyRound, PolicyGame> PlayOne(int seed, PolicyNet[] poolNow)
        {
            var rng = new Rng(seed);
            var r = new PolicyRound();
            var samples = new List<Sample>();
            double u = rng.Next(1000000) / 1000000.0;
            double total = SelfShare + (poolNow.Length > 0 ? PoolShare : 0) + ClassicShare;
            u *= total;
            int mode = u < SelfShare ? 0 : u < SelfShare + (poolNow.Length > 0 ? PoolShare : 0) ? 1 : 2;
            int seat = rng.Next(2);
            KeyValuePair<string, List<CardDef>> mine, theirs;
            if (Matchup && decks.Count >= 2)
            {
                int a = rng.Next(2);
                mine = decks[a];
                theirs = decks[1 - a];
            }
            else
            {
                mine = FocusDeck != null ? decks.Find(d => d.Key == FocusDeck) : decks[rng.Next(decks.Count)];
                if (mine.Value == null) mine = decks[rng.Next(decks.Count)];
                theirs = decks[rng.Next(decks.Count)];
            }
            var game = new PolicyGame { LearnerDeck = mine.Key, OpponentDeck = theirs.Key };
            var learner = new PolicyAgent(Net, seed + 1, seat, mine.Value) { Greedy = false, Record = true, Abort = Abort };
            IPlayerAgent other;
            PolicyAgent otherLearner = null;
            if (mode == 0) other = otherLearner = new PolicyAgent(Net, seed + 2, 1 - seat, theirs.Value) { Greedy = false, Record = true, Abort = Abort };
            else if (mode == 1) other = new PolicyAgent(poolNow[rng.Next(poolNow.Length)], seed + 2, 1 - seat, theirs.Value) { Greedy = false, Abort = Abort };
            else other = new HeuristicAgent(seed + 2);
            try
            {
                var engine = new GameEngine(scripts, seat == 0 ? (IPlayerAgent)learner : other, seat == 0 ? other : learner);
                engine.CheckInvariants = false;
                engine.CardsPerPlayer = mine.Value.Count == theirs.Value.Count ? mine.Value.Count : 0;
                engine.Rules = Rules ?? GameRules.Default;
                engine.PrizeCount = PrizeCount;
                var result = engine.Run(seat == 0 ? mine.Value : theirs.Value, seat == 0 ? theirs.Value : mine.Value, seed);
                var s = engine.State;
                r.Games = 1;
                r.Turns = result.Turns;
                game.Turns = result.Turns;
                game.WinnerDeck = result.Winner < 0 ? "" : result.Winner == seat ? mine.Key : theirs.Key;
                if (result.Winner < 0) r.Draws = 1;
                else if (mode == 0 || result.Winner == seat) r.LearnerWins = 1;
                if (mode != 0)
                {
                    r.VsPool = 1;
                    r.PoolWins = result.Winner == seat ? 1 : 0;
                }
                foreach (var a in new[] { learner, otherLearner })
                {
                    if (a == null || a.Trajectory.Count == 0) continue;
                    int me = a.Seat;
                    double outcome = result.Winner < 0 ? 0 : result.Winner == me ? 1 : -1;
                    int finalLead = s.Players[1 - me].Prizes.Count - s.Players[me].Prizes.Count;
                    samples.AddRange(Finish(a.Trajectory, outcome, finalLead));
                    r.Decisions += a.Trajectory.Count;
                    r.LearnerSeats++;
                    foreach (var step in a.Trajectory) r.Mistakes += step.Mistakes;
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                if (Aborted) return null;
                r.Errors = 1;
                r.LastError = "seed " + seed + ": " + e.GetType().Name + ": " + e.Message;
                game.Error = r.LastError;
                samples.Clear();
            }
            return Tuple.Create(samples, r, game);
        }

        /// <summary>Generalized advantage estimation over one player's decisions in one game.</summary>
        private List<Sample> Finish(List<PolicyStep> traj, double outcome, int finalLead)
        {
            int n = traj.Count;
            var rewards = new double[n];
            for (int t = 0; t < n; t++)
            {
                int nextLead = t + 1 < n ? traj[t + 1].PrizeLead : finalLead;
                rewards[t] = PrizeReward * (nextLead - traj[t].PrizeLead) - SequencePenalty * traj[t].Mistakes;
            }
            rewards[n - 1] += outcome;
            var list = new List<Sample>(n);
            var adv = new double[n];
            double last = 0;
            for (int t = n - 1; t >= 0; t--)
            {
                double nextV = t + 1 < n ? traj[t + 1].Value : 0;
                double delta = rewards[t] + Gamma * nextV - traj[t].Value;
                last = delta + Gamma * Lambda * last;
                adv[t] = last;
            }
            for (int t = 0; t < n; t++)
                list.Add(new Sample { Input = traj[t].Input, Action = traj[t].Action, LogP = traj[t].LogP, Adv = (float)adv[t], Ret = (float)(adv[t] + traj[t].Value) });
            return list;
        }

        // ------------------------------------------------------------------ PPO

        private void Learn(List<Sample> samples, PolicyRound round)
        {
            double mean = 0, sq = 0;
            foreach (var s in samples) mean += s.Adv;
            mean /= samples.Count;
            foreach (var s in samples) sq += (s.Adv - mean) * (s.Adv - mean);
            double std = Math.Sqrt(sq / samples.Count) + 1e-8;
            var adv = new float[samples.Count];
            for (int i = 0; i < samples.Count; i++) adv[i] = (float)((samples[i].Adv - mean) / std);

            var order = new int[samples.Count];
            for (int i = 0; i < order.Length; i++) order[i] = i;
            var rng = new Rng(rounds * 31 + 7);
            int batches = 0;
            for (int epoch = 0; epoch < Epochs; epoch++)
            {
                for (int i = order.Length - 1; i > 0; i--)
                {
                    int j = rng.Next(i + 1);
                    int tmp = order[i];
                    order[i] = order[j];
                    order[j] = tmp;
                }
                for (int start = 0; start < order.Length; start += Minibatch)
                {
                    if (Aborted) break;
                    int count = Math.Min(Minibatch, order.Length - start);
                    var stepStart = DateTime.UtcNow;
                    var g = Gradients(samples, adv, order, start, count);
                    // The battery saver and the rest setting apply to learning too (it's the heaviest work: vector maths on
                    // every thread), not only to playing games.
                    WorkThrottle.Rest(DateTime.UtcNow - stepStart, () => Aborted);
                    if (RestShare > 0) Thread.Sleep((int)Math.Min(2000, (DateTime.UtcNow - stepStart).TotalMilliseconds * RestShare));
                    if (Aborted) break;
                    Net.Apply(g, count);
                    round.PolicyLoss += g.PolicyLoss / count;
                    round.ValueLoss += g.ValueLoss / count;
                    round.Entropy += g.Entropy / count;
                    round.ClipFraction += g.ClipFraction / count;
                    batches++;
                }
            }
            if (batches > 0)
            {
                round.PolicyLoss /= batches;
                round.ValueLoss /= batches;
                round.Entropy /= batches;
                round.ClipFraction /= batches;
                Net.AverageLoss = Net.Steps <= batches ? round.PolicyLoss + ValueCoef * round.ValueLoss
                    : Net.AverageLoss + 0.1 * (round.PolicyLoss + ValueCoef * round.ValueLoss - Net.AverageLoss);
            }
        }

        /// <summary>Summed PPO gradients of a minibatch, computed on several threads.</summary>
        private PolicyNet.Grad Gradients(List<Sample> samples, float[] adv, int[] order, int start, int count)
        {
            int threads = Math.Max(1, Math.Min(Math.Min(WorkThrottle.Threads(Threads), Pool.Count), count / 16 + 1));
            var grads = new PolicyNet.Grad[threads];
            var failures = new Exception[threads];
            if (gradPool.Length < threads) Array.Resize(ref gradPool, threads);
            Pool.Run(tt =>
            {
                if (tt >= threads) return;
                {
                    try
                    {
                        // Each thread keeps its gradient buffer (8 MB) from one learning step to the next.
                        var g = gradPool[tt] ?? (gradPool[tt] = Net.NewGrad());
                        g.Clear();
                        // This thread's share of the minibatch, a few decisions at a time (the network is faster in batches).
                        var mine = new List<int>();
                        for (int k = tt; k < count; k += threads) mine.Add(order[start + k]);
                        for (int b = 0; b < mine.Count && !Aborted; b += BatchSize)
                            Accumulate(samples, adv, mine.GetRange(b, Math.Min(BatchSize, mine.Count - b)), g);
                        grads[tt] = g;
                    }
                    catch (Exception e)
                    {
                        failures[tt] = e;
                    }
                }
            });
            // A learning thread that failed is reported on this thread (training stops with the error) instead of dying alone.
            foreach (var e in failures) if (e != null) throw new InvalidOperationException("Neural AI learning failed: " + e.Message, e);
            for (int t = 1; t < threads; t++) grads[0].Add(grads[t]);
            return grads[0];
        }

        private PolicyNet.Grad[] gradPool = new PolicyNet.Grad[0];

        /// <summary>Where progress messages go (the Unity console in the app); null: nowhere.</summary>
        public static Action<string> LogTo;

        private static void Log(string message)
        {
            var log = LogTo;
            if (log != null) log(message);
        }

        private Workers pool2;

        /// <summary>The trainer's long-lived threads (they keep their reused working memory between rounds).</summary>
        private Workers Pool
        {
            get
            {
                if (pool2 == null) pool2 = new Workers(Math.Max(1, Threads));
                return pool2;
            }
        }

        /// <summary>Ends the trainer's threads (call when done with it).</summary>
        public void Dispose()
        {
            if (pool2 != null) pool2.Dispose();
            pool2 = null;
        }

        /// <summary>
        /// A few threads that stay alive and run the trainer's work: Run(job) calls job(0) ... job(Count - 1), one on each, and
        /// waits for all of them. They run below normal priority so the app's own thread stays responsive.
        /// </summary>
        private sealed class Workers
        {
            private readonly Thread[] threads;
            private readonly object sync = new object();
            private Action<int> job;
            private int generation, remaining;
            private Exception failure;
            private bool stopping;

            public int Count { get { return threads.Length; } }

            public Workers(int count)
            {
                threads = new Thread[count];
                for (int i = 0; i < count; i++)
                {
                    int index = i;
                    threads[i] = new Thread(() => Loop(index)) { IsBackground = true, Name = "Neural AI worker " + (i + 1), Priority = ThreadPriority.BelowNormal };
                    threads[i].Start();
                }
            }

            private void Loop(int index)
            {
                int seen = 0;
                while (true)
                {
                    Action<int> work;
                    lock (sync)
                    {
                        while (generation == seen && !stopping) Monitor.Wait(sync);
                        if (stopping) return;
                        seen = generation;
                        work = job;
                    }
                    try
                    {
                        work(index);
                    }
                    catch (Exception e)
                    {
                        lock (sync) if (failure == null) failure = e;
                    }
                    lock (sync)
                    {
                        if (--remaining == 0) Monitor.PulseAll(sync);
                    }
                }
            }

            public void Run(Action<int> work)
            {
                Exception failed;
                lock (sync)
                {
                    job = work;
                    failure = null;
                    remaining = threads.Length;
                    generation++;
                    Monitor.PulseAll(sync);
                    while (remaining > 0) Monitor.Wait(sync);
                    failed = failure;
                    job = null;
                }
                if (failed != null) throw new InvalidOperationException("Neural AI training failed: " + failed.Message, failed);
            }

            public void Dispose()
            {
                lock (sync)
                {
                    stopping = true;
                    Monitor.PulseAll(sync);
                }
            }
        }

        /// <summary>
        /// Rest after each learning step for this share of the time it took (0.25: a step of 4 seconds is followed by 1 second's
        /// rest), so a long session doesn't keep every core at full load. 0 in the command-line trainer.
        /// </summary>
        public double RestShare;

        /// <summary>Collect garbage between playing and learning (keeps the memory a round needs from piling up in Unity).</summary>
        public bool CollectGarbage;

        /// <summary>With a negative advantage, ratios above this stop pushing (dual-clip PPO).</summary>
        public double DualClip = 3.0;

        /// <summary>Decisions run through the network together in the learning step.</summary>
        public int BatchSize = 32;

        private void Accumulate(List<Sample> samples, float[] adv, List<int> batch, PolicyNet.Grad g)
        {
            var inputs = new PolicyInput[batch.Count];
            for (int k = 0; k < batch.Count; k++) inputs[k] = samples[batch[k]].Input;
            var passes = Net.ForwardPooled(inputs);
            var dLogits = new float[batch.Count][];
            var dValues = new float[batch.Count];
            for (int k = 0; k < batch.Count; k++)
            {
                var s = samples[batch[k]];
                float a = adv[batch[k]];
                var pass = passes[k];
                var logits = pass.Logits;
                int n = logits.Length;
                var p = PolicyNet.Softmax(logits);
                var logp = new double[n];
                double entropy = 0;
                for (int i = 0; i < n; i++)
                {
                    logp[i] = Math.Log(Math.Max(1e-12, p[i]));
                    entropy -= p[i] * logp[i];
                }
                double ratio = Math.Exp(logp[s.Action] - s.LogP);
                double clipped = Math.Max(1 - Clip, Math.Min(1 + Clip, ratio));
                double pg = -Math.Min(ratio * a, clipped * a);
                // d(pg)/d(logp[action]): the unclipped term's, unless the clipped term is the smaller one and is clipped.
                // Dual clip: with a negative advantage the ratio's pull is capped too (at DualClip), so one decision whose
                // probability grew a lot can't produce a huge step.
                bool flat = (a > 0 && ratio > 1 + Clip) || (a < 0 && ratio < 1 - Clip) || (a < 0 && ratio > DualClip);
                if (a < 0 && ratio > DualClip) pg = -DualClip * a;
                double dLogpA = flat ? 0 : -a * ratio;
                var d = new float[n];
                for (int i = 0; i < n; i++)
                {
                    double v = dLogpA * ((i == s.Action ? 1 : 0) - p[i]);
                    v += EntropyCoef * p[i] * (logp[i] + entropy); // - coef * entropy
                    d[i] = (float)v;
                }
                dLogits[k] = d;
                double ret = Math.Max(-1.5, Math.Min(1.5, s.Ret));
                double vErr = pass.Value - ret;
                dValues[k] = (float)(2 * ValueCoef * vErr);
                g.PolicyLoss += pg;
                g.ValueLoss += vErr * vErr;
                g.Entropy += entropy;
                g.ClipFraction += Math.Abs(ratio - 1) > Clip ? 1 : 0;
            }
            Net.BackwardMany(inputs, passes, dLogits, dValues, g);
        }

        // ------------------------------------------------------------------ Measuring

        /// <summary>
        /// Win rate (0..1) of the network (top-rated moves) against another player, half the games on each seat, with random
        /// decks from the pool (or the focus deck for the network).
        /// </summary>
        public double Evaluate(Func<int, IPlayerAgent> opponent, int games, int seed0, out int wins, out int draws, out int losses)
        {
            int w = 0, d = 0, l = 0, next = 0;
            object gate = new object();
            int threads = Math.Max(1, Math.Min(Pool.Count, games));
            Pool.Run(t =>
            {
                if (t >= threads) return;
                {
                    while (true)
                    {
                        int k;
                        lock (gate)
                        {
                            if (next >= games || (ShouldStop != null && ShouldStop())) return;
                            k = next++;
                        }
                        int seed = seed0 + k;
                        var rng = new Rng(seed);
                        KeyValuePair<string, List<CardDef>> mine, theirs;
                        int seat = k % 2;
                        if (Matchup && decks.Count >= 2)
                        {
                            // Deck A against deck B: half the games with each deck, seats alternating.
                            mine = decks[k % 2];
                            theirs = decks[1 - k % 2];
                            seat = (k / 2) % 2;
                        }
                        else
                        {
                            mine = FocusDeck != null ? decks.Find(x => x.Key == FocusDeck) : decks[rng.Next(decks.Count)];
                            if (mine.Value == null) mine = decks[rng.Next(decks.Count)];
                            theirs = decks[rng.Next(decks.Count)];
                        }
                        var bot = new PolicyAgent(Net, seed + 1, seat, mine.Value) { Greedy = true };
                        var opp = opponent(seed + 2);
                        int outcome = 0;
                        try
                        {
                            var engine = new GameEngine(scripts, seat == 0 ? (IPlayerAgent)bot : opp, seat == 0 ? opp : bot);
                            engine.CheckInvariants = false;
                            engine.CardsPerPlayer = mine.Value.Count == theirs.Value.Count ? mine.Value.Count : 0;
                            engine.Rules = Rules ?? GameRules.Default;
                            engine.PrizeCount = PrizeCount;
                            var res = engine.Run(seat == 0 ? mine.Value : theirs.Value, seat == 0 ? theirs.Value : mine.Value, seed);
                            outcome = res.Winner < 0 ? 0 : res.Winner == seat ? 1 : -1;
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        lock (gate)
                        {
                            if (outcome > 0) w++;
                            else if (outcome < 0) l++;
                            else d++;
                        }
                    }
                }
            });
            wins = w;
            draws = d;
            losses = l;
            int played = w + d + l;
            return played == 0 ? -1 : (w + 0.5 * d) / played;
        }
    }
}
