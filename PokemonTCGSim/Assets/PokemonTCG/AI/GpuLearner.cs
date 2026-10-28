using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>What the GPU learner did with one round's decisions.</summary>
    public sealed class GpuLearnResult
    {
        public double PolicyLoss, ValueLoss, Entropy, ClipFraction, Seconds;
        /// <summary>Cross-entropy to the look-ahead's choices (training from the look-ahead; 0 without).</summary>
        public double DistillLoss;
        public int Batches, Skipped;
        /// <summary>Where it learned, e.g. "cuda:0 (NVIDIA GeForce RTX 2060)".</summary>
        public string Device = "";
    }

    /// <summary>
    /// Learning on the graphics card: the Neural AI's training plays its games here (on the CPU) and hands each round's
    /// decisions to GpuLearner/learner.py, a small Python program that runs the same PPO learning with PyTorch on the GPU and
    /// sends the new weights back. They talk over a local connection (127.0.0.1:5577 unless you choose another); the
    /// message layout is described at the top of learner.py. When the learner isn't running or fails, PolicyTrainer learns
    /// that round on the CPU as before.
    /// </summary>
    public sealed class GpuLearner : IDisposable
    {
        /// <summary>
        /// Message version: 1, or 2 when some decisions carry the look-ahead's improved choice (training from the look-ahead): then
        /// TRN1 adds the distillation weight after max grad norm, and the samples end with f32 has-target[N] and f32
        /// targets[sum of option counts]. learner.py reads both; RES1 ends with f64 distillation loss (after the weights) for version 2.
        /// </summary>
        public const int Version = 1, VersionWithTargets = 2;
        /// <summary>
        /// A TRN1 for another network than the AI's (an exploiter, League): i32 3, i32 slot, then the message as above. The learner
        /// keeps a model and optimizer for each slot, so the AI's own (slot 0, every other message) is left as it was.
        /// </summary>
        public const int VersionWithSlot = 3;
        public const string DefaultAddress = "127.0.0.1:5577";

        public readonly string Host;
        public readonly int Port;
        private TcpClient client;
        private Stream stream;

        /// <summary>Where the learner last learned ("" until it has).</summary>
        public string Device { get; private set; }

        /// <summary>
        /// The network the learner holds, as far as this side knows (for choosing moves there, GpuMoves): the weights' revision
        /// and the card rows it has. -1: unknown, so the weights are sent before the next moves.
        /// </summary>
        private int learnerRevision = -1, learnerRows;
        private readonly object exchangeGate = new object();

        public bool Connected { get { return client != null && client.Connected; } }

        public GpuLearner(string host, int port)
        {
            Host = string.IsNullOrEmpty(host) ? "127.0.0.1" : host;
            Port = port;
            Device = "";
        }

        /// <summary>"host:port" (or just a port); false when it isn't one.</summary>
        public static bool TryParse(string address, out string host, out int port)
        {
            host = "127.0.0.1";
            port = 5577;
            if (string.IsNullOrEmpty(address)) return false;
            string a = address.Trim();
            int colon = a.LastIndexOf(':');
            string portText = colon >= 0 ? a.Substring(colon + 1) : a;
            if (colon > 0) host = a.Substring(0, colon).Trim();
            return int.TryParse(portText.Trim(), out port) && port > 0 && port < 65536 && host.Length > 0;
        }

        public void Connect(int timeoutMs)
        {
            Disconnect();
            var c = new TcpClient { NoDelay = true, ReceiveTimeout = 10 * 60 * 1000, SendTimeout = 60 * 1000 };
            var wait = c.BeginConnect(Host, Port, null, null);
            if (!wait.AsyncWaitHandle.WaitOne(timeoutMs))
            {
                c.Close();
                throw new IOException("no GPU learner answered at " + Host + ":" + Port + " (start GpuLearner/learner.py)");
            }
            try
            {
                c.EndConnect(wait);
            }
            catch (SocketException e)
            {
                c.Close();
                throw new IOException("no GPU learner at " + Host + ":" + Port + " (start GpuLearner/learner.py): " + e.Message);
            }
            client = c;
            stream = new BufferedStream(c.GetStream(), 1 << 20);
        }

        public void Disconnect()
        {
            try
            {
                if (stream != null) stream.Dispose();
                if (client != null) client.Close();
            }
            catch (Exception)
            {
            }
            stream = null;
            client = null;
        }

        public void Dispose()
        {
            Disconnect();
        }

        /// <summary>
        /// Learns from one round's decisions on the GPU (the PPO settings given), then puts the learned weights into net.
        /// Throws IOException when the learner can't be reached or answers with an error.
        /// </summary>
        public GpuLearnResult Train(PolicyNet net, IList<PolicyInput> inputs, int[] actions, float[] logp, float[] adv, float[] ret,
            int epochs, int minibatch, int seed, double clip, double dualClip, double valueCoef, double entropyCoef,
            float[][] targets = null, double distillCoef = 0, int slot = 0)
        {
            if (!Connected) Connect(3000);
            bool withTargets = false;
            if (targets != null) foreach (var t in targets) if (t != null) withTargets = true;
            int sentRows;
            var body = new MemoryStream();
            using (var w = new BinaryWriter(body, Encoding.UTF8, true))
            {
                if (slot != 0)
                {
                    w.Write(VersionWithSlot);
                    w.Write(slot);
                }
                w.Write(withTargets ? VersionWithTargets : Version);
                w.Write(epochs);
                w.Write(minibatch);
                w.Write(seed);
                w.Write(clip);
                w.Write(dualClip);
                w.Write(valueCoef);
                w.Write(entropyCoef);
                w.Write(net.LearningRate);
                w.Write(net.MaxGradNorm);
                if (withTargets) w.Write(distillCoef);
                sentRows = net.WriteLearnerWeights(w, true);
                WriteSamples(w, inputs, actions, logp, adv, ret);
                if (withTargets)
                {
                    for (int i = 0; i < inputs.Count; i++) w.Write(targets[i] != null && targets[i].Length == inputs[i].Count ? 1f : 0f);
                    for (int i = 0; i < inputs.Count; i++)
                    {
                        var t = targets[i];
                        bool ok = t != null && t.Length == inputs[i].Count;
                        for (int k = 0; k < inputs[i].Count; k++) w.Write(ok ? t[k] : 0f);
                    }
                }
            }
            var reply = Exchange("TRN1", body, "RES1");
            using (var r = new BinaryReader(new MemoryStream(reply)))
            {
                var result = new GpuLearnResult
                {
                    PolicyLoss = r.ReadDouble(), ValueLoss = r.ReadDouble(), Entropy = r.ReadDouble(), ClipFraction = r.ReadDouble(),
                    Batches = r.ReadInt32(), Skipped = r.ReadInt32(), Seconds = r.ReadDouble(), Device = ReadString(r)
                };
                net.ReadLearnerWeights(r, result.Batches);
                if (withTargets) result.DistillLoss = r.ReadDouble(); // after the weights
                Device = result.Device;
                // The learner keeps the weights it just sent back, so it can choose moves with them straight away (the AI's own
                // network: it chooses moves only for that one).
                if (slot == 0)
                {
                    learnerRevision = net.WeightsRevision;
                    learnerRows = sentRows;
                }
                return result;
            }
        }

        /// <summary>The learner's network output for these inputs, with the network's current weights (to check it matches this one).</summary>
        public float[][] Forward(PolicyNet net, IList<PolicyInput> inputs, out float[] values)
        {
            if (!Connected) Connect(3000);
            var body = new MemoryStream();
            using (var w = new BinaryWriter(body, Encoding.UTF8, true))
            {
                w.Write(Version);
                net.WriteLearnerWeights(w, true);
                var none = new int[inputs.Count];
                var zeros = new float[inputs.Count];
                WriteSamples(w, inputs, none, zeros, zeros, zeros);
            }
            var reply = Exchange("FWD1", body, "OUT1");
            var logits = new float[inputs.Count][];
            values = new float[inputs.Count];
            using (var r = new BinaryReader(new MemoryStream(reply)))
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    logits[i] = new float[inputs[i].Count];
                    for (int k = 0; k < logits[i].Length; k++) logits[i][k] = r.ReadSingle();
                    values[i] = r.ReadSingle();
                }
            }
            return logits;
        }

        private byte[] Exchange(string kind, MemoryStream body, string expect)
        {
            lock (exchangeGate) return ExchangeLocked(kind, body, expect);
        }

        private byte[] ExchangeLocked(string kind, MemoryStream body, string expect)
        {
            try
            {
                var head = new byte[12];
                Encoding.ASCII.GetBytes(kind, 0, 4, head, 0);
                BitConverter.GetBytes(body.Length).CopyTo(head, 4);
                stream.Write(head, 0, 12);
                body.Position = 0;
                body.CopyTo(stream);
                stream.Flush();
                ReadExactly(head, 12);
                string got = Encoding.ASCII.GetString(head, 0, 4);
                long size = BitConverter.ToInt64(head, 4);
                if (size < 0 || size > int.MaxValue) throw new IOException("the GPU learner sent a broken message");
                var payload = new byte[size];
                ReadExactly(payload, (int)size);
                if (got == "ERR1")
                {
                    using (var r = new BinaryReader(new MemoryStream(payload))) throw new IOException("the GPU learner said: " + ReadString(r));
                }
                if (got != expect) throw new IOException("the GPU learner answered " + got + " instead of " + expect);
                return payload;
            }
            catch (IOException)
            {
                Disconnect();
                learnerRevision = -1; // a new connection may be a restarted learner
                throw;
            }
            catch (SocketException e)
            {
                Disconnect();
                throw new IOException("lost the GPU learner: " + e.Message);
            }
            catch (ObjectDisposedException e)
            {
                Disconnect();
                throw new IOException("lost the GPU learner: " + e.Message);
            }
        }

        private void ReadExactly(byte[] buffer, int count)
        {
            int got = 0;
            while (got < count)
            {
                int n = stream.Read(buffer, got, count - got);
                if (n <= 0) throw new IOException("the GPU learner closed the connection");
                got += n;
            }
        }

        /// <summary>
        /// Choosing moves on the GPU (GpuMoves): the network's option scores and value for a batch of decisions, worked out by the
        /// learner with the network it holds. Its copy is brought up to date first: all the weights when they changed here (SETW),
        /// or just the cards met since (CARD). Throws IOException when the learner can't be reached or answers with an error.
        /// </summary>
        public float[][] Infer(PolicyNet net, IList<PolicyInput> inputs, out float[] values)
        {
            lock (exchangeGate)
            {
                if (!Connected) Connect(3000);
                int revision = net.WeightsRevision;
                if (revision != learnerRevision || net.CardRows < learnerRows)
                {
                    var weights = new MemoryStream();
                    int rows;
                    using (var w = new BinaryWriter(weights, Encoding.UTF8, true))
                    {
                        w.Write(Version);
                        rows = net.WriteLearnerWeights(w, true);
                    }
                    ExchangeLocked("SETW", weights, "OKW1");
                    learnerRevision = revision;
                    learnerRows = rows;
                }
                else if (net.CardRows > learnerRows)
                {
                    var cards = new MemoryStream();
                    int rows;
                    using (var w = new BinaryWriter(cards, Encoding.UTF8, true))
                    {
                        w.Write(Version);
                        rows = net.WriteNewCardRows(w, learnerRows);
                    }
                    ExchangeLocked("CARD", cards, "OKW1");
                    learnerRows = rows;
                }
                var body = new MemoryStream();
                using (var w = new BinaryWriter(body, Encoding.UTF8, true))
                {
                    w.Write(Version);
                    var none = new int[inputs.Count];
                    var zeros = new float[inputs.Count];
                    WriteSamples(w, inputs, none, zeros, zeros, zeros);
                }
                var reply = ExchangeLocked("INF1", body, "OUT1");
                return ReadOutputs(reply, inputs, out values);
            }
        }

        private static float[][] ReadOutputs(byte[] reply, IList<PolicyInput> inputs, out float[] values)
        {
            var logits = new float[inputs.Count][];
            values = new float[inputs.Count];
            using (var r = new BinaryReader(new MemoryStream(reply)))
            {
                for (int i = 0; i < inputs.Count; i++)
                {
                    logits[i] = new float[inputs[i].Count];
                    for (int k = 0; k < logits[i].Length; k++) logits[i][k] = r.ReadSingle();
                    values[i] = r.ReadSingle();
                }
            }
            return logits;
        }

        /// <summary>The decisions, column by column (see learner.py's "samples").</summary>
        private static void WriteSamples(BinaryWriter w, IList<PolicyInput> xs, int[] actions, float[] logp, float[] adv, float[] ret)
        {
            int n = xs.Count;
            w.Write(n);
            w.Write(PolicyInput.NumSlots);
            w.Write(PolicyInput.SlotFeatures);
            w.Write(PolicyInput.GlobalFeatures);
            w.Write(PolicyInput.TurnFeatures);
            w.Write(PolicyInput.OptionFeatures);
            w.Write(PolicyInput.BagCount);
            foreach (var x in xs) foreach (int v in x.SlotCard) w.Write(v);
            foreach (var x in xs) foreach (int v in x.SlotTool) w.Write(v);
            foreach (var x in xs) foreach (float v in x.SlotFeat) w.Write(v);
            for (int b = 0; b < PolicyInput.BagCount; b++)
            {
                foreach (var x in xs) w.Write(x.Bags[b].Length);
                foreach (var x in xs) foreach (int v in x.Bags[b]) w.Write(v);
            }
            foreach (var x in xs) w.Write(x.Stadium);
            foreach (var x in xs) w.Write(x.Source);
            foreach (var x in xs) foreach (float v in x.Globals) w.Write(v);
            foreach (var x in xs) foreach (float v in x.TurnFeats) w.Write(v);
            foreach (var x in xs) w.Write(x.Count);
            foreach (var x in xs) for (int i = 0; i < x.Count; i++) w.Write(x.OptKind[i]);
            foreach (var x in xs) for (int i = 0; i < x.Count; i++) w.Write(x.OptCard[i]);
            foreach (var x in xs) for (int i = 0; i < x.Count; i++) w.Write(x.OptTarget[i]);
            foreach (var x in xs) for (int i = 0; i < x.Count * PolicyInput.OptionFeatures; i++) w.Write(x.OptFeat[i]);
            for (int i = 0; i < n; i++) w.Write(actions[i]);
            for (int i = 0; i < n; i++) w.Write(logp[i]);
            for (int i = 0; i < n; i++) w.Write(adv[i]);
            for (int i = 0; i < n; i++) w.Write(ret[i]);
        }

        public static void WriteString(BinaryWriter w, string s)
        {
            var b = Encoding.UTF8.GetBytes(s ?? "");
            w.Write(b.Length);
            w.Write(b);
        }

        public static string ReadString(BinaryReader r)
        {
            int n = r.ReadInt32();
            if (n < 0 || n > 1 << 20) throw new InvalidDataException("a broken string from the GPU learner");
            return Encoding.UTF8.GetString(r.ReadBytes(n));
        }
    }
}
