using System;
using System.Collections.Generic;
using System.IO;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// How the learning AI turns what it has learned into choices (v74). The app only offers the Neural AI now; Learning and
    /// Hybrid remain for the fallback when no network loads and for the tests that compare them.
    /// </summary>
    public enum AiStyle
    {
        /// <summary>The learning AI's feature weights only (as before v74).</summary>
        Learning = 0,
        /// <summary>The neural network only.</summary>
        Neural = 1,
        /// <summary>Both: the feature weights and the network's values added together.</summary>
        Hybrid = 2
    }

    /// <summary>
    /// The neural network half of the learning AI (v74), trained inside the app on the CPU from the same games as the feature
    /// weights (training, Regional Sim, your games and your feedback).
    ///
    /// Each option of a decision goes in as its feature names (the same ones the feature weights use, see Features.For),
    /// hashed into an embedding table, plus a short description of the game state (StateOf: Prize cards, hands, decks,
    /// Benches, the Active Pokémon's HP and Energy, the turn). Two hidden layers of 32 ReLU units give one score per option;
    /// a softmax over the options is the network's policy. It learns by policy gradient: a decision is made more (or less)
    /// likely in proportion to the credit the game gives it, and by imitation (your plays, and later game logs).
    ///
    /// Untrained, it has no opinion (all values 0), so the AI plays exactly as before until it has learned something.
    /// Thread-safe: the training thread and a live game can share it.
    /// </summary>
    public sealed class NeuralNet
    {
        /// <summary>Rows of the embedding table (feature names are hashed into it, twice each, with a random sign).</summary>
        public const int Buckets = 8192;
        public const int Hidden = 32;
        /// <summary>Numbers describing the game state (StateOf).</summary>
        public const int DenseSize = 20;
        /// <summary>Largest value (either way) handed to the agent, as for the feature weights.</summary>
        public const double MaxValue = 4.0;
        private const int FormatVersion = 1;

        private readonly object sync = new object();
        private float[] emb, wd, b0, w1, b1, w2;
        private float b2;

        /// <summary>Step size of each learning step.</summary>
        public double LearningRate = 0.02;
        /// <summary>Largest strength (credit) of one learning step, either way.</summary>
        public double MaxStrength = 1.5;

        private long updates;
        private double averageLoss;

        public NeuralNet()
        {
            Init();
        }

        /// <summary>Learning steps taken so far (0 = untrained).</summary>
        public long Updates { get { lock (sync) return updates; } }

        /// <summary>Running average of how surprised the network was by the plays it learned from (lower = it already expected them).</summary>
        public double AverageLoss { get { lock (sync) return averageLoss; } }

        public bool Trained { get { lock (sync) return updates > 0; } }

        private void Init()
        {
            emb = new float[Buckets * Hidden];
            wd = new float[DenseSize * Hidden];
            b0 = new float[Hidden];
            w1 = new float[Hidden * Hidden];
            b1 = new float[Hidden];
            w2 = new float[Hidden];
            b2 = 0f;
            // A fixed seed, so a fresh network is always the same one.
            var rng = new Rng(74);
            // Wide enough that different options look different to the network from the start.
            for (int i = 0; i < emb.Length; i++) emb[i] = Uniform(rng, 0.5);
            for (int i = 0; i < wd.Length; i++) wd[i] = Uniform(rng, 0.3);
            double limit = Math.Sqrt(6.0 / (Hidden + Hidden));
            for (int i = 0; i < w1.Length; i++) w1[i] = Uniform(rng, limit);
            for (int j = 0; j < Hidden; j++) b0[j] = 0.05f;
            for (int j = 0; j < Hidden; j++) b1[j] = 0.05f;
            // The output layer starts at zero: an untrained network scores every option the same.
            updates = 0;
            averageLoss = 0;
        }

        private static float Uniform(Rng rng, double limit)
        {
            return (float)((rng.Next(1000001) / 1000000.0 * 2.0 - 1.0) * limit);
        }

        /// <summary>Forgets everything (back to the fresh network).</summary>
        public void Reset()
        {
            lock (sync) Init();
        }

        // ------------------------------------------------------------------ Game state

        /// <summary>The numbers describing the game state at a decision (all roughly 0..1).</summary>
        public static float[] StateOf(DecisionRequest r)
        {
            var x = new float[DenseSize];
            var v = r == null ? null : r.View;
            if (v == null) return x;
            var me = v.Me;
            var opp = v.Opponent;
            if (me == null || opp == null) return x;
            string phase = Features.Phase(v);
            x[0] = phase == "early" ? 1f : 0f;
            x[1] = phase == "mid" ? 1f : 0f;
            x[2] = phase == "late" ? 1f : 0f;
            x[3] = me.PrizeCount / 6f;
            x[4] = opp.PrizeCount / 6f;
            x[5] = (opp.PrizeCount - me.PrizeCount) / 6f;
            x[6] = Math.Min(me.HandCount, 15) / 10f;
            x[7] = Math.Min(opp.HandCount, 15) / 10f;
            x[8] = me.DeckCount / 60f;
            x[9] = opp.DeckCount / 60f;
            x[10] = me.Bench.Count / 5f;
            x[11] = opp.Bench.Count / 5f;
            x[12] = HpShare(me.Active);
            x[13] = HpShare(opp.Active);
            x[14] = me.Active == null ? 0f : Math.Min(me.Active.Energy.Count, 6) / 5f;
            x[15] = opp.Active == null ? 0f : Math.Min(opp.Active.Energy.Count, 6) / 5f;
            x[16] = Math.Min(v.TurnNumber, 30) / 20f;
            x[17] = r.Kind == DecisionKind.MainAction ? 1f : 0f;
            x[18] = v.FirstPlayer == v.Viewer ? 1f : 0f;
            x[19] = me.Active != null && HeuristicAgent.CanAttackNow(me.Active) ? 1f : 0f;
            return x;
        }

        private static float HpShare(PokemonView p)
        {
            if (p == null || p.MaxHp <= 0) return 0f;
            return Math.Max(0, p.RemainingHp) / (float)p.MaxHp;
        }

        // ------------------------------------------------------------------ Forward pass

        /// <summary>One option run through the network, with what the backward pass needs.</summary>
        private sealed class Pass
        {
            public int[] Index;
            public float[] Sign;
            public float Scale;
            public readonly float[] H0 = new float[Hidden];
            public readonly float[] A0 = new float[Hidden];
            public readonly float[] H1 = new float[Hidden];
            public readonly float[] A1 = new float[Hidden];
            public float Y;
        }

        /// <summary>FNV-1a with a final mix: the same feature name always lands in the same rows.</summary>
        private static uint Hash(string s, uint seed)
        {
            uint h = 2166136261u ^ seed;
            for (int i = 0; i < s.Length; i++)
            {
                h ^= s[i];
                h *= 16777619u;
            }
            h ^= h >> 15;
            h *= 0x2C1B3C6Du;
            h ^= h >> 12;
            h *= 0x297A2D39u;
            h ^= h >> 15;
            return h;
        }

        /// <summary>Call inside the lock.</summary>
        private Pass Forward(List<string> feats, float[] dense)
        {
            var p = new Pass();
            int n = feats == null ? 0 : feats.Count;
            p.Index = new int[n * 2];
            p.Sign = new float[n * 2];
            for (int k = 0; k < n; k++)
            {
                string f = feats[k] ?? "";
                uint h1 = Hash(f, 0u);
                uint h2 = Hash(f, 0x9E3779B9u);
                p.Index[2 * k] = (int)(h1 % Buckets);
                p.Sign[2 * k] = (h1 & 0x80000000u) != 0 ? -1f : 1f;
                p.Index[2 * k + 1] = (int)(h2 % Buckets);
                p.Sign[2 * k + 1] = (h2 & 0x80000000u) != 0 ? -1f : 1f;
            }
            int m = p.Index.Length;
            p.Scale = m > 0 ? (float)(1.0 / Math.Sqrt(m)) : 0f;
            for (int j = 0; j < Hidden; j++) p.H0[j] = b0[j];
            for (int k = 0; k < m; k++)
            {
                int row = p.Index[k] * Hidden;
                float s = p.Sign[k] * p.Scale;
                for (int j = 0; j < Hidden; j++) p.H0[j] += s * emb[row + j];
            }
            if (dense != null)
            {
                for (int d = 0; d < DenseSize && d < dense.Length; d++)
                {
                    float value = dense[d];
                    if (value == 0f) continue;
                    int row = d * Hidden;
                    for (int j = 0; j < Hidden; j++) p.H0[j] += value * wd[row + j];
                }
            }
            for (int j = 0; j < Hidden; j++) p.A0[j] = p.H0[j] > 0f ? p.H0[j] : 0f;
            for (int i = 0; i < Hidden; i++)
            {
                float sum = b1[i];
                int row = i * Hidden;
                for (int j = 0; j < Hidden; j++) sum += w1[row + j] * p.A0[j];
                p.H1[i] = sum;
                p.A1[i] = sum > 0f ? sum : 0f;
            }
            float y = b2;
            for (int i = 0; i < Hidden; i++) y += w2[i] * p.A1[i];
            p.Y = y;
            return p;
        }

        /// <summary>
        /// Backward pass for one option with g = d(loss)/d(score), applying the step right away (plain SGD). Call inside the lock.
        /// </summary>
        private void Backward(Pass p, float[] dense, float g, float lr)
        {
            var dA1 = new float[Hidden];
            for (int i = 0; i < Hidden; i++)
            {
                dA1[i] = g * w2[i];
                w2[i] -= lr * g * p.A1[i];
            }
            b2 -= lr * g;
            var dA0 = new float[Hidden];
            for (int i = 0; i < Hidden; i++)
            {
                if (p.H1[i] <= 0f) continue;
                float dh = dA1[i];
                if (dh == 0f) continue;
                int row = i * Hidden;
                for (int j = 0; j < Hidden; j++)
                {
                    dA0[j] += dh * w1[row + j];
                    w1[row + j] -= lr * dh * p.A0[j];
                }
                b1[i] -= lr * dh;
            }
            for (int j = 0; j < Hidden; j++)
            {
                if (p.H0[j] <= 0f) dA0[j] = 0f;
                else b0[j] -= lr * dA0[j];
            }
            int m = p.Index.Length;
            for (int k = 0; k < m; k++)
            {
                int row = p.Index[k] * Hidden;
                float s = p.Sign[k] * p.Scale * lr;
                for (int j = 0; j < Hidden; j++) emb[row + j] -= s * dA0[j];
            }
            if (dense != null)
            {
                for (int d = 0; d < DenseSize && d < dense.Length; d++)
                {
                    float value = dense[d];
                    if (value == 0f) continue;
                    int row = d * Hidden;
                    for (int j = 0; j < Hidden; j++) wd[row + j] -= lr * value * dA0[j];
                }
            }
        }

        // ------------------------------------------------------------------ Using it

        /// <summary>
        /// The network's value of each option (-4..4): its scores, centred on their average, so only the differences between the
        /// options count. All 0 while it's untrained.
        /// </summary>
        public double[] Values(List<List<string>> options, float[] dense)
        {
            int n = options == null ? 0 : options.Count;
            var result = new double[n];
            if (n < 2) return result;
            lock (sync)
            {
                if (updates == 0) return result;
                double mean = 0;
                for (int i = 0; i < n; i++)
                {
                    result[i] = Forward(options[i], dense).Y;
                    mean += result[i];
                }
                mean /= n;
                for (int i = 0; i < n; i++) result[i] = Clamp(result[i] - mean, -MaxValue, MaxValue);
            }
            return result;
        }

        /// <summary>The network's policy: how likely it thinks each option is the one to pick (sums to 1).</summary>
        public double[] Policy(List<List<string>> options, float[] dense)
        {
            int n = options == null ? 0 : options.Count;
            var p = new double[n];
            if (n == 0) return p;
            lock (sync)
            {
                double max = double.MinValue;
                for (int i = 0; i < n; i++)
                {
                    p[i] = Forward(options[i], dense).Y;
                    if (p[i] > max) max = p[i];
                }
                double z = 0;
                for (int i = 0; i < n; i++)
                {
                    p[i] = Math.Exp(p[i] - max);
                    z += p[i];
                }
                for (int i = 0; i < n; i++) p[i] /= z;
            }
            return p;
        }

        /// <summary>
        /// One learning step for a decision: the chosen option(s) become more likely in proportion to strength (less likely when
        /// strength is negative). A softmax policy-gradient step (with strength 1 it's plain imitation of the choice). Returns
        /// how surprised the network was by the choice (-log of the chance it gave it), or 0 if nothing was learned.
        /// </summary>
        public double Learn(List<List<string>> options, float[] dense, IList<int> chosen, double strength)
        {
            if (options == null || options.Count < 2 || chosen == null || chosen.Count == 0) return 0;
            if (double.IsNaN(strength) || Math.Abs(strength) < 1e-6) return 0;
            double adv = Clamp(strength, -MaxStrength, MaxStrength);
            int n = options.Count;
            int valid = 0;
            foreach (int c in chosen) if (c >= 0 && c < n) valid++;
            if (valid == 0) return 0;
            lock (sync)
            {
                var passes = new Pass[n];
                double max = double.MinValue;
                for (int i = 0; i < n; i++)
                {
                    passes[i] = Forward(options[i], dense);
                    if (passes[i].Y > max) max = passes[i].Y;
                }
                var p = new double[n];
                double z = 0;
                for (int i = 0; i < n; i++)
                {
                    p[i] = Math.Exp(passes[i].Y - max);
                    z += p[i];
                }
                for (int i = 0; i < n; i++) p[i] /= z;
                var target = new double[n];
                foreach (int c in chosen) if (c >= 0 && c < n) target[c] += 1.0 / valid;
                double loss = 0;
                for (int i = 0; i < n; i++)
                    if (target[i] > 0) loss -= target[i] * Math.Log(Math.Max(1e-9, p[i]));
                // Positive credit: the usual step toward the choice (it fades as the choice becomes certain). Negative credit:
                // "unlikelihood", a step away that fades as the choice becomes unlikely, so a bad play is never pushed down forever.
                double scale;
                if (adv >= 0) scale = adv;
                else
                {
                    double chosenMass = 0;
                    for (int i = 0; i < n; i++) chosenMass += target[i] * p[i];
                    scale = adv * Math.Min(5.0, chosenMass / Math.Max(1e-6, 1 - chosenMass));
                }
                float lr = (float)LearningRate;
                for (int i = 0; i < n; i++)
                {
                    float g = (float)(-scale * (target[i] - p[i]));
                    if (Math.Abs(g) < 1e-7f) continue;
                    Backward(passes[i], dense, g, lr);
                }
                updates++;
                averageLoss = updates == 1 ? loss : averageLoss + 0.002 * (loss - averageLoss);
                return loss;
            }
        }

        // ------------------------------------------------------------------ Saving

        /// <summary>Everything the network knows as one line of text (base64 of the weights), for the AI's file.</summary>
        public string ToBase64()
        {
            lock (sync)
            {
                using (var ms = new MemoryStream())
                {
                    using (var w = new BinaryWriter(ms))
                    {
                        w.Write(FormatVersion);
                        w.Write(Buckets);
                        w.Write(Hidden);
                        w.Write(DenseSize);
                        w.Write(updates);
                        w.Write(averageLoss);
                        WriteArray(w, emb);
                        WriteArray(w, wd);
                        WriteArray(w, b0);
                        WriteArray(w, w1);
                        WriteArray(w, b1);
                        WriteArray(w, w2);
                        w.Write(b2);
                        w.Flush();
                        return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }

        /// <summary>Loads ToBase64() text. A network of another shape, or damaged text, is ignored (returns false; the network is unchanged).</summary>
        public bool FromBase64(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(text.Trim());
            }
            catch (FormatException)
            {
                return false;
            }
            try
            {
                using (var ms = new MemoryStream(bytes))
                {
                    using (var r = new BinaryReader(ms))
                    {
                        if (r.ReadInt32() != FormatVersion || r.ReadInt32() != Buckets || r.ReadInt32() != Hidden || r.ReadInt32() != DenseSize) return false;
                        long u = r.ReadInt64();
                        double loss = r.ReadDouble();
                        var e = ReadArray(r, Buckets * Hidden);
                        var d = ReadArray(r, DenseSize * Hidden);
                        var c0 = ReadArray(r, Hidden);
                        var m1 = ReadArray(r, Hidden * Hidden);
                        var c1 = ReadArray(r, Hidden);
                        var m2 = ReadArray(r, Hidden);
                        float c2 = r.ReadSingle();
                        lock (sync)
                        {
                            emb = e;
                            wd = d;
                            b0 = c0;
                            w1 = m1;
                            b1 = c1;
                            w2 = m2;
                            b2 = c2;
                            updates = u;
                            averageLoss = loss;
                        }
                        return true;
                    }
                }
            }
            catch (EndOfStreamException)
            {
                return false;
            }
            catch (IOException)
            {
                return false;
            }
        }

        private static void WriteArray(BinaryWriter w, float[] a)
        {
            for (int i = 0; i < a.Length; i++) w.Write(a[i]);
        }

        private static float[] ReadArray(BinaryReader r, int count)
        {
            var a = new float[count];
            for (int i = 0; i < count; i++)
            {
                float value = r.ReadSingle();
                a[i] = float.IsNaN(value) || float.IsInfinity(value) ? 0f : value;
            }
            return a;
        }

        private static double Clamp(double x, double lo, double hi)
        {
            return x < lo ? lo : x > hi ? hi : x;
        }
    }
}
