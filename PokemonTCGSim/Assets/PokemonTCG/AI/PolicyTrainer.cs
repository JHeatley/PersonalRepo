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
        public double PolicyLoss, ValueLoss, Entropy, ClipFraction;
        public string LastError;
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
            int next = 0, finished = 0;
            object gate = new object();
            int baseSeed = Interlocked.Add(ref seedCounter, GamesPerRound * 3);
            PolicyNet[] poolNow;
            lock (pool) poolNow = pool.ToArray();
            var workers = new List<Thread>();
            int threads = Math.Max(1, Math.Min(WorkThrottle.Threads(Threads), GamesPerRound));
            for (int t = 0; t < threads; t++)
            {
                var th = new Thread(() =>
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
                        lock (gate)
                        {
                            Merge(round, result.Item2);
                            samples.AddRange(result.Item1);
                            finished++;
                        }
                        if (GameFinished != null) GameFinished(result.Item3);
                        WorkThrottle.Rest(DateTime.UtcNow - started, () => ShouldStop != null && ShouldStop()); // the battery saver
                    }
                }) { IsBackground = true, Name = "Neural AI training" };
                workers.Add(th);
                th.Start();
            }
            foreach (var th in workers) th.Join();
            if (samples.Count > 0) Learn(samples, round);
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
            var learner = new PolicyAgent(Net, seed + 1, seat, mine.Value) { Greedy = false, Record = true };
            IPlayerAgent other;
            PolicyAgent otherLearner = null;
            if (mode == 0) other = otherLearner = new PolicyAgent(Net, seed + 2, 1 - seat, theirs.Value) { Greedy = false, Record = true };
            else if (mode == 1) other = new PolicyAgent(poolNow[rng.Next(poolNow.Length)], seed + 2, 1 - seat, theirs.Value) { Greedy = false };
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
                    foreach (var step in a.Trajectory) r.Mistakes += step.Mistakes;
                }
            }
            catch (Exception e)
            {
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
                    int count = Math.Min(Minibatch, order.Length - start);
                    var g = Gradients(samples, adv, order, start, count);
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
            int threads = Math.Max(1, Math.Min(Threads, count / 16 + 1));
            var grads = new PolicyNet.Grad[threads];
            var workers = new Thread[threads];
            for (int t = 0; t < threads; t++)
            {
                int tt = t;
                workers[t] = new Thread(() =>
                {
                    var g = Net.NewGrad();
                    for (int k = tt; k < count; k += threads)
                    {
                        int i = order[start + k];
                        Accumulate(samples[i], adv[i], g);
                    }
                    grads[tt] = g;
                }) { IsBackground = true, Name = "Neural AI learning" };
                workers[t].Start();
            }
            foreach (var w in workers) w.Join();
            for (int t = 1; t < threads; t++) grads[0].Add(grads[t]);
            return grads[0];
        }

        private void Accumulate(Sample s, float a, PolicyNet.Grad g)
        {
            var pass = Net.Forward(s.Input);
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
            bool flat = (a > 0 && ratio > 1 + Clip) || (a < 0 && ratio < 1 - Clip);
            double dLogpA = flat ? 0 : -a * ratio;
            var dLogits = new float[n];
            for (int i = 0; i < n; i++)
            {
                double d = dLogpA * ((i == s.Action ? 1 : 0) - p[i]);
                d += EntropyCoef * p[i] * (logp[i] + entropy); // - coef * entropy
                dLogits[i] = (float)d;
            }
            double ret = Math.Max(-1.5, Math.Min(1.5, s.Ret));
            double vErr = pass.Value - ret;
            float dValue = (float)(2 * ValueCoef * vErr);
            Net.Backward(s.Input, pass, dLogits, dValue, g);
            g.PolicyLoss += pg;
            g.ValueLoss += vErr * vErr;
            g.Entropy += entropy;
            g.ClipFraction += Math.Abs(ratio - 1) > Clip ? 1 : 0;
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
            var workers = new List<Thread>();
            for (int t = 0; t < Math.Max(1, Math.Min(Threads, games)); t++)
            {
                var th = new Thread(() =>
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
                        var mine = FocusDeck != null ? decks.Find(x => x.Key == FocusDeck) : decks[rng.Next(decks.Count)];
                        if (mine.Value == null) mine = decks[rng.Next(decks.Count)];
                        var theirs = decks[rng.Next(decks.Count)];
                        int seat = k % 2;
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
                }) { IsBackground = true, Name = "Neural AI measuring" };
                workers.Add(th);
                th.Start();
            }
            foreach (var th in workers) th.Join();
            wins = w;
            draws = d;
            losses = l;
            int played = w + d + l;
            return played == 0 ? -1 : (w + 0.5 * d) / played;
        }
    }
}
