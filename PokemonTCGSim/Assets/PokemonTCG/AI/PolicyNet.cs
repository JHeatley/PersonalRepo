using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// One decision as the policy network sees it (filled in by PolicyEncoder). Card fields hold rows of the network's card
    /// table (0 = no card). The layout matches the Python bot's encoder (PokemonTCGBot/bot/encode.py), so a network trained
    /// there plays here unchanged.
    /// </summary>
    public sealed class PolicyInput
    {
        public const int MaxBench = 8;
        public const int SideSlots = 1 + MaxBench;
        public const int NumSlots = 2 * SideSlots;
        public const int NoTarget = NumSlots;
        /// <summary>Numbers per Pokémon in play (26 before the Energy-needed and evolution ones: PolicyNet.Widen).</summary>
        public const int SlotFeatures = 47;
        public const int GlobalFeatures = 20 + PolicyKinds.DecisionCount;
        /// <summary>Numbers per option (6 before the Energy attachment and evolution ones: PolicyNet.Widen).</summary>
        public const int OptionFeatures = 21;
        /// <summary>
        /// This turn's numbers (actions, Abilities used), then the Item numbers, the free-retreat pivots on each Bench and the
        /// Bench and play-from-hand numbers (2 + SideSlots before those: PolicyNet.Widen).
        /// </summary>
        public const int TurnFeatures = 2 + SideSlots + 16;
        /// <summary>Bags of cards: hand, your discard, their discard, your unseen cards, their known hand, cards they've shown, your plays this turn, their last turn.</summary>
        public const int BagCount = 8;
        public const int BagHand = 0, BagMyDiscard = 1, BagOppDiscard = 2, BagUnseen = 3, BagOppKnownHand = 4, BagOppSeen = 5, BagTurnPlayed = 6, BagOppLastTurn = 7;
        /// <summary>Most cards kept in each bag (the Python encoder's limits; the count feature is n / 20).</summary>
        public static readonly int[] BagLimit = { 40, 60, 60, 60, 20, 60, 20, 20 };

        public readonly int[] SlotCard = new int[NumSlots];
        public readonly int[] SlotTool = new int[NumSlots];
        public readonly float[] SlotFeat = new float[NumSlots * SlotFeatures];
        public readonly int[][] Bags = new int[BagCount][];
        public int Stadium;
        public int Source;
        public readonly float[] Globals = new float[GlobalFeatures];
        public readonly float[] TurnFeats = new float[TurnFeatures];

        public int Count;
        public int[] OptKind = new int[0];
        public int[] OptCard = new int[0];
        public int[] OptTarget = new int[0];
        public float[] OptFeat = new float[0];

        public PolicyInput()
        {
            for (int b = 0; b < BagCount; b++) Bags[b] = new int[0];
        }

        public void SetOptions(int count)
        {
            Count = count;
            OptKind = new int[count];
            OptCard = new int[count];
            OptTarget = new int[count];
            OptFeat = new float[count * OptionFeatures];
            for (int i = 0; i < count; i++) OptTarget[i] = NoTarget;
        }

        /// <summary>A copy with only these options (for picking several cards one at a time).</summary>
        public PolicyInput WithOptions(IList<int> keep)
        {
            var x = (PolicyInput)MemberwiseClone();
            x.SetOptions(keep.Count);
            for (int k = 0; k < keep.Count; k++)
            {
                int i = keep[k];
                x.OptKind[k] = OptKind[i];
                x.OptCard[k] = OptCard[i];
                x.OptTarget[k] = OptTarget[i];
                Array.Copy(OptFeat, i * OptionFeatures, x.OptFeat, k * OptionFeatures, OptionFeatures);
            }
            return x;
        }
    }

    /// <summary>The Python bot's decision and option kinds, in its order (they index the network's inputs).</summary>
    public static class PolicyKinds
    {
        public static readonly string[] Decisions = { "main", "go_first", "setup_active", "setup_bench", "promote", "pick_card",
            "pick_pokemon", "pick_energy", "yes_no", "pick_option", "discard", "search", "counters" };
        public static readonly string[] Options = { "end", "attack", "ability", "retreat", "bench", "evolve", "attach", "tool", "play",
            "stadium", "card", "pokemon", "energy", "done", "yes", "no", "first", "second", "choice" };
        public const int DecisionCount = 13;
        public const int OptionCount = 19;

        public static int Decision(string name)
        {
            return Array.IndexOf(Decisions, name);
        }

        public static int Option(string name)
        {
            return Array.IndexOf(Options, name);
        }
    }

    /// <summary>
    /// The policy/value network ported from the Python bot (PokemonTCGBot/bot/model.py). The game state (board slots, hand,
    /// discard piles, what you remember about the opponent, this turn's plays) becomes a vector h; each option is encoded
    /// from its kind, its card and the Pokémon it targets and scored against h; a softmax over the scores is the policy. The
    /// value head predicts the result (+1 win, -1 loss) for the player deciding.
    ///
    /// It learns by PPO (PolicyTrainer) and by imitation (your plays). Forward and backward passes are written out by hand
    /// (no libraries), so it runs anywhere Unity does. Thread-safe: games read it while the trainer updates it.
    /// </summary>
    public sealed class PolicyNet
    {
        public const int CardDim = 48, SlotDim = 96, Width = 384, OptionDim = 128, StaticSize = 176;
        /// <summary>
        /// Input sizes of networks saved before the per-attack costs (PolicyCards), the Energy-needed and evolution numbers
        /// per Pokémon and the attachment numbers per option. They still load: Widen gives their first layers zero weights for
        /// the new inputs, so they play exactly as before until training teaches them to use them.
        /// </summary>
        private const int OldSlotFeatures = 26, OldOptionFeatures = 6, OldStaticSize = 64, OldTurnFeatures = 2 + PolicyInput.SideSlots;
        /// <summary>
        /// Residual blocks in the trunk: a new network has DefaultBlocks, Grow makes it deeper (BigBlocks: the Make the network bigger
        /// button), and loading takes whatever the file has, up to MaxBlocks.
        /// </summary>
        public const int DefaultBlocks = 3, BigBlocks = 6, MaxBlocks = 12;
        public const int SlotIn = 2 * CardDim + PolicyInput.SlotFeatures;
        public const int BagSize = CardDim + 1;
        public const int StateSize = PolicyInput.NumSlots * SlotDim + 3 * BagSize + 2 * CardDim + PolicyInput.GlobalFeatures
                                     + 3 * BagSize + 2 * BagSize + PolicyInput.TurnFeatures;
        public const int OptionIn = 2 * CardDim + SlotDim + PolicyInput.OptionFeatures;
        public const int ValueHidden = Width / 2;
        private const float LnEps = 1e-5f;
        private const int FormatVersion = 1;

        // ------------------------------------------------------------------ Parameters

        internal sealed class Param
        {
            public readonly string Name;
            public readonly int[] Shape;
            public int Index;
            public float[] Data;
            public float[] M, V;

            public Param(string name, params int[] shape)
            {
                Name = name;
                Shape = shape;
                int n = 1;
                foreach (int d in shape) n *= d;
                Data = new float[n];
            }
        }

        private readonly List<Param> parameters = new List<Param>();
        private readonly Dictionary<string, Param> byName = new Dictionary<string, Param>(StringComparer.Ordinal);

        private Param P(string name, params int[] shape)
        {
            var p = new Param(name, shape) { Index = parameters.Count };
            parameters.Add(p);
            byName[name] = p;
            return p;
        }

        // Card table: a learned row per card (row 0 = no card) plus a projection of the card's printed stats.
        private readonly object cardLock = new object();
        private readonly List<float[]> cardRows = new List<float[]>();
        private readonly List<float[]> cardStatic = new List<float[]>();
        /// <summary>Rows whose printed stats from OldStaticSize on aren't filled in yet (loaded from an older network), and how many.</summary>
        private readonly List<bool> cardStale = new List<bool>();
        private volatile int staleRows;
        private readonly List<string> cardKeys = new List<string>();
        private readonly List<float[]> cardM = new List<float[]>(), cardV = new List<float[]>();
        private readonly Dictionary<string, int> rowOfKey = new Dictionary<string, int>(StringComparer.Ordinal);
        /// <summary>A copy of rowOfKey that games read without locking (replaced whenever a card is added).</summary>
        private volatile Dictionary<string, int> rowIndex = new Dictionary<string, int>(StringComparer.Ordinal);
        /// <summary>Card vectors for the current weights, by row (filled as they're needed; cleared when the weights change).</summary>
        private float[][] cardCache = new float[0][];

        private Param projW, projB, slot0W, slot0B, slot2W, slot2B, inpW, inpB, finalLnG, finalLnB, kind,
            opt0W, opt0B, opt2W, opt2B, queryW, queryB, score1W, score1B, score3W, score3B, value0W, value0B, value2W, value2B, noneSlot;
        private Param[] lnG, lnB, b1W, b1B, b3W, b3B;
        private int blocks;

        /// <summary>Residual blocks in the trunk (3 at first; more after Grow, or when a bigger network is loaded).</summary>
        public int Blocks { get { return Volatile.Read(ref blocks); } }

        private readonly ReaderWriterLockSlim rw = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        private long steps;
        private int adamT;

        /// <summary>Learning rate and the other optimizer settings (the Python bot's defaults).</summary>
        public double LearningRate = 2e-4;
        public double MaxGradNorm = 0.5;

        /// <summary>Games this network has trained on (including the Python bot's games it started from).</summary>
        public long GamesTrained;
        /// <summary>Optimizer steps taken in this app (0 = still exactly as loaded or fresh).</summary>
        public long Steps { get { return Interlocked.Read(ref steps); } }
        private long skippedSteps;

        /// <summary>Learning steps skipped because their gradients weren't finite numbers.</summary>
        public long SkippedSteps { get { return Interlocked.Read(ref skippedSteps); } }

        /// <summary>True when every weight is a finite number (a network with NaN in it is never saved or loaded).</summary>
        public bool AllFinite()
        {
            rw.EnterReadLock();
            try
            {
                foreach (var p in parameters)
                    foreach (float v in p.Data)
                        if (float.IsNaN(v) || float.IsInfinity(v)) return false;
                lock (cardLock)
                {
                    foreach (var r in cardRows)
                        foreach (float v in r)
                            if (float.IsNaN(v) || float.IsInfinity(v)) return false;
                }
                return true;
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        /// <summary>Running average of the training loss shown on the Train screen.</summary>
        public double AverageLoss;
        /// <summary>Where the starting weights came from ("Python bot, 126,225 games"), or "" for a fresh network.</summary>
        public string Origin = "";

        /// <summary>
        /// It knows something: loaded from the Python bot or trained here. A fresh network (random weights) is never asked; the AI
        /// plays as it did before the network existed.
        /// </summary>
        public bool Trained { get { return Steps > 0 || GamesTrained > 0 || !string.IsNullOrEmpty(Origin); } }

        public PolicyNet() : this(DefaultBlocks)
        {
        }

        /// <summary>A fresh network with this many residual blocks.</summary>
        public PolicyNet(int blocks)
        {
            if (blocks < 1 || blocks > MaxBlocks) throw new ArgumentOutOfRangeException("blocks");
            Build(blocks);
            InitRandom(1);
        }

        /// <summary>Makes the parameters (zeros) for a network with this many blocks, in the order they're saved.</summary>
        private void Build(int nb)
        {
            parameters.Clear();
            byName.Clear();
            lnG = new Param[nb];
            lnB = new Param[nb];
            b1W = new Param[nb];
            b1B = new Param[nb];
            b3W = new Param[nb];
            b3B = new Param[nb];
            projW = P("cards.proj.weight", CardDim, StaticSize);
            projB = P("cards.proj.bias", CardDim);
            slot0W = P("slot.0.weight", SlotDim, SlotIn);
            slot0B = P("slot.0.bias", SlotDim);
            slot2W = P("slot.2.weight", SlotDim, SlotDim);
            slot2B = P("slot.2.bias", SlotDim);
            inpW = P("inp.0.weight", Width, StateSize);
            inpB = P("inp.0.bias", Width);
            for (int b = 0; b < nb; b++)
            {
                lnG[b] = P("body." + b + ".net.0.weight", Width);
                lnB[b] = P("body." + b + ".net.0.bias", Width);
                b1W[b] = P("body." + b + ".net.1.weight", Width, Width);
                b1B[b] = P("body." + b + ".net.1.bias", Width);
                b3W[b] = P("body." + b + ".net.3.weight", Width, Width);
                b3B[b] = P("body." + b + ".net.3.bias", Width);
            }
            finalLnG = P("body." + nb + ".weight", Width);
            finalLnB = P("body." + nb + ".bias", Width);
            kind = P("kind.weight", PolicyKinds.OptionCount, CardDim);
            opt0W = P("option.0.weight", OptionDim, OptionIn);
            opt0B = P("option.0.bias", OptionDim);
            opt2W = P("option.2.weight", OptionDim, OptionDim);
            opt2B = P("option.2.bias", OptionDim);
            queryW = P("query.weight", OptionDim, Width);
            queryB = P("query.bias", OptionDim);
            score1W = P("score.1.weight", OptionDim, OptionDim);
            score1B = P("score.1.bias", OptionDim);
            score3W = P("score.3.weight", 1, OptionDim);
            score3B = P("score.3.bias", 1);
            value0W = P("value.0.weight", ValueHidden, Width);
            value0B = P("value.0.bias", ValueHidden);
            value2W = P("value.2.weight", 1, ValueHidden);
            value2B = P("value.2.bias", 1);
            noneSlot = P("none_slot", SlotDim);
            Volatile.Write(ref blocks, nb);
        }

        /// <summary>
        /// Makes the network deeper (more residual blocks) without changing what it knows: the new blocks start out adding
        /// nothing (their last layer is zeros), so it plays exactly as before, and training then puts them to use. The optimizer
        /// keeps its state for the weights it had. Not while it's training (the trainer's buffers are sized for the network).
        /// </summary>
        public void Grow(int nb)
        {
            if (nb > MaxBlocks) throw new ArgumentOutOfRangeException("nb", "at most " + MaxBlocks + " blocks");
            rw.EnterWriteLock();
            try
            {
                int old = blocks;
                if (nb <= old) return;
                var kept = new Dictionary<string, Param>(byName, StringComparer.Ordinal);
                // The last normalization layer is named after the block count: it moves to the end.
                kept["body." + nb + ".weight"] = kept["body." + old + ".weight"];
                kept["body." + nb + ".bias"] = kept["body." + old + ".bias"];
                kept.Remove("body." + old + ".weight");
                kept.Remove("body." + old + ".bias");
                Build(nb);
                var rng = new Rng(1000 + old);
                foreach (var p in parameters)
                {
                    Param was;
                    if (kept.TryGetValue(p.Name, out was) && was.Data.Length == p.Data.Length)
                    {
                        p.Data = was.Data;
                        p.M = was.M;
                        p.V = was.V;
                    }
                }
                for (int b = old; b < nb; b++)
                {
                    // LayerNorm as fresh (gain 1, bias 0), the first layer random as PyTorch would make it, the last one zeros.
                    Fill(lnG[b].Data, 1f);
                    double bound = 1.0 / Math.Sqrt(Width);
                    var w = b1W[b].Data;
                    for (int i = 0; i < w.Length; i++) w[i] = (float)((Uniform01(rng) * 2 - 1) * bound);
                }
                cardCache = new float[0][];
            }
            finally
            {
                weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                rw.ExitWriteLock();
            }
        }

        /// <summary>
        /// Saved weights from before the inputs grew (OldSlotFeatures, OldOptionFeatures, OldStaticSize, OldTurnFeatures) get
        /// zero weights for the new inputs, which come last in each layer's input. Returns the printed-stat count per card the file was made with.
        /// </summary>
        private static int Widen(Dictionary<string, float[]> data)
        {
            float[] proj;
            int statics = data.TryGetValue("cards.proj.weight", out proj) && proj.Length == CardDim * OldStaticSize ? OldStaticSize : StaticSize;
            WidenRows(data, "slot.0.weight", SlotDim, SlotIn - PolicyInput.SlotFeatures + OldSlotFeatures, SlotIn);
            WidenRows(data, "option.0.weight", OptionDim, OptionIn - PolicyInput.OptionFeatures + OldOptionFeatures, OptionIn);
            WidenRows(data, "cards.proj.weight", CardDim, OldStaticSize, StaticSize);
            WidenRows(data, "inp.0.weight", Width, StateSize - PolicyInput.TurnFeatures + OldTurnFeatures, StateSize); // they come last
            return statics;
        }

        private static void WidenRows(Dictionary<string, float[]> data, string name, int rows, int oldColumns, int columns)
        {
            float[] a;
            if (oldColumns == columns || !data.TryGetValue(name, out a) || a.Length != rows * oldColumns) return;
            var b = new float[rows * columns];
            for (int r = 0; r < rows; r++) Array.Copy(a, r * oldColumns, b, r * columns, oldColumns);
            data[name] = b;
        }

        /// <summary>The block count of saved weights (body.0, body.1, ... each with its .net.1.weight), or 0 if there are none.</summary>
        private static int BlocksIn(Dictionary<string, float[]> data)
        {
            int nb = 0;
            while (nb <= MaxBlocks && data.ContainsKey("body." + nb + ".net.1.weight")) nb++;
            return nb;
        }

        /// <summary>
        /// True when saved weights fit this network's layers, with nb blocks (Build(nb) makes the trunk; the rest doesn't depend
        /// on it).
        /// </summary>
        private bool Fits(Dictionary<string, float[]> data, int nb)
        {
            if (nb < 1 || nb > MaxBlocks) return false;
            foreach (var p in parameters)
                if (!p.Name.StartsWith("body.", StringComparison.Ordinal) && !Has(data, p.Name, p.Data.Length)) return false;
            for (int b = 0; b < nb; b++)
            {
                string pre = "body." + b + ".net.";
                if (!Has(data, pre + "0.weight", Width) || !Has(data, pre + "0.bias", Width) || !Has(data, pre + "1.weight", Width * Width)
                    || !Has(data, pre + "1.bias", Width) || !Has(data, pre + "3.weight", Width * Width) || !Has(data, pre + "3.bias", Width))
                    return false;
            }
            return Has(data, "body." + nb + ".weight", Width) && Has(data, "body." + nb + ".bias", Width);
        }

        private static bool Has(Dictionary<string, float[]> data, string name, int length)
        {
            float[] a;
            return data.TryGetValue(name, out a) && a.Length == length;
        }

        /// <summary>Takes saved weights that Fits(data, nb) (under the write lock), rebuilding the trunk if its size differs.</summary>
        private void TakeWeights(Dictionary<string, float[]> data, int nb)
        {
            if (nb != blocks) Build(nb);
            foreach (var p in parameters)
            {
                p.Data = data[p.Name];
                p.M = null;
                p.V = null;
            }
        }

        /// <summary>Fresh weights (PyTorch's default initialisation), no cards.</summary>
        private void InitRandom(int seed)
        {
            var rng = new Rng(seed);
            foreach (var p in parameters)
            {
                if (p.Name.EndsWith(".bias", StringComparison.Ordinal) || p == noneSlot) Fill(p.Data, 0f);
                else if (p.Shape.Length == 1) Fill(p.Data, 1f); // LayerNorm gains
                else if (p == kind) for (int i = 0; i < p.Data.Length; i++) p.Data[i] = (float)Gaussian(rng);
                else
                {
                    double bound = 1.0 / Math.Sqrt(p.Shape[1]);
                    for (int i = 0; i < p.Data.Length; i++) p.Data[i] = (float)((Uniform01(rng) * 2 - 1) * bound);
                }
                p.M = null;
                p.V = null;
            }
            lock (cardLock)
            {
                cardRows.Clear();
                cardStatic.Clear();
                cardStale.Clear();
                staleRows = 0;
                cardKeys.Clear();
                cardM.Clear();
                cardV.Clear();
                rowOfKey.Clear();
                rowIndex = new Dictionary<string, int>(StringComparer.Ordinal);
                cardRows.Add(new float[CardDim]);
                cardStatic.Add(new float[StaticSize]);
                cardStale.Add(false);
                cardKeys.Add("");
                cardM.Add(null);
                cardV.Add(null);
                cardCache = new float[0][];
            }
            steps = 0;
            adamT = 0;
            GamesTrained = 0;
            AverageLoss = 0;
            Origin = "";
        }

        private static double Gaussian(Rng rng)
        {
            double u1 = Math.Max(1e-12, Uniform01(rng)), u2 = Uniform01(rng);
            return Math.Sqrt(-2 * Math.Log(u1)) * Math.Cos(2 * Math.PI * u2);
        }

        private static double Uniform01(Rng rng)
        {
            return (rng.NextULong() >> 11) * (1.0 / 9007199254740992.0);
        }

        private static void Fill(float[] a, float v)
        {
            for (int i = 0; i < a.Length; i++) a[i] = v;
        }

        public int CardRows { get { lock (cardLock) return cardRows.Count; } }

        private int weightsRevision;

        /// <summary>
        /// Goes up whenever the weights change (a learning step, the learner's weights taken in, loading, a reset), so a copy kept
        /// elsewhere (the GPU learner choosing moves, GpuMoves) knows when it's out of date. New card rows don't count: see CardRows.
        /// </summary>
        public int WeightsRevision { get { return System.Threading.Volatile.Read(ref weightsRevision); } }

        // ------------------------------------------------------------------ Cards

        /// <summary>Row of the card table for this card (added on first sight, starting from its printed stats only).</summary>
        public int RowOf(CardDef def)
        {
            if (def == null) return 0;
            string key = def.BaseKey ?? def.Name;
            int known;
            if (rowIndex.TryGetValue(key, out known))
            {
                if (staleRows > 0) Refresh(known, def);
                return known;
            }
            lock (cardLock)
            {
                int row;
                if (rowOfKey.TryGetValue(key, out row)
                    || (def.Key != null && def.Key != key && rowOfKey.TryGetValue(def.Key, out row)))
                {
                    if (staleRows > 0) Refresh(row, def);
                    return row;
                }
                if (rowOfKey.TryGetValue("name:" + NameKey(def), out row))
                {
                    rowOfKey[key] = row;
                    rowIndex = new Dictionary<string, int>(rowOfKey, StringComparer.Ordinal);
                    if (staleRows > 0) Refresh(row, def);
                    return row;
                }
                row = cardRows.Count;
                cardRows.Add(new float[CardDim]);
                cardStatic.Add(PolicyCards.Static(def));
                cardStale.Add(false);
                cardKeys.Add(key);
                cardM.Add(null);
                cardV.Add(null);
                rowOfKey[key] = row;
                rowIndex = new Dictionary<string, int>(rowOfKey, StringComparer.Ordinal);
                return row;
            }
        }

        /// <summary>
        /// Fills in the printed stats a row loaded from an older network lacks (from OldStaticSize on: the per-attack costs),
        /// from the card itself. The learned row and the older stats stay as they were.
        /// </summary>
        private void Refresh(int row, CardDef def)
        {
            lock (cardLock)
            {
                if (row <= 0 || row >= cardStale.Count || !cardStale[row]) return;
                var fresh = PolicyCards.Static(def);
                var s = (float[])cardStatic[row].Clone();
                Array.Copy(fresh, OldStaticSize, s, OldStaticSize, StaticSize - OldStaticSize);
                cardStatic[row] = s;
                cardStale[row] = false;
                staleRows--;
                var cache = cardCache;
                if (row < cache.Length) cache[row] = null;
            }
            Interlocked.Increment(ref weightsRevision); // the GPU learner's copy of the stats is out of date now (GpuMoves)
        }

        /// <summary>True when a card's printed stats are all zero from index from on.</summary>
        private static bool Blank(float[] s, int from)
        {
            for (int i = from; i < s.Length; i++) if (s[i] != 0) return false;
            return true;
        }

        /// <summary>Name used to match a card printed in a set the Python bot didn't know.</summary>
        internal static string NameKey(CardDef def)
        {
            if (def.Kind == CardKind.Energy && def.IsBasicEnergy) return "Basic " + PolicyCards.TypeName(def.EnergyProvides) + " Energy";
            return def.Name;
        }

        /// <summary>The card's vector: its learned row plus the projection of its printed stats (zero for row 0).</summary>
        private float[] CardVector(int row)
        {
            if (row <= 0) return null;
            var cache = cardCache;
            if (row < cache.Length)
            {
                var v = cache[row];
                if (v != null) return v;
            }
            float[] r, s;
            lock (cardLock)
            {
                r = cardRows[row];
                s = cardStatic[row];
            }
            var outv = new float[CardDim];
            var w = projW.Data;
            for (int o = 0; o < CardDim; o++)
            {
                float sum = projB.Data[o] + r[o];
                int off = o * StaticSize;
                for (int i = 0; i < StaticSize; i++) sum += w[off + i] * s[i];
                outv[o] = sum;
            }
            if (row < cache.Length) cache[row] = outv; // two threads may both compute it: same numbers either way
            else
            {
                lock (cardLock)
                {
                    var now = cardCache;
                    if (row >= now.Length)
                    {
                        var bigger = new float[Math.Max(row + 1, cardRows.Count + 64)][];
                        Array.Copy(now, bigger, now.Length);
                        now = bigger;
                        cardCache = now;
                    }
                    now[row] = outv;
                }
            }
            return outv;
        }

        // ------------------------------------------------------------------ Math

        /// <summary>Error function (Abramowitz and Stegun 7.1.26, error below 1.5e-7).</summary>
        internal static float Erf(float xf)
        {
            double x = Math.Abs(xf);
            double t = 1.0 / (1.0 + 0.3275911 * x);
            double y = 1.0 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592) * t * Math.Exp(-x * x);
            return (float)(xf < 0 ? -y : y);
        }

        private const float InvSqrt2 = 0.70710678118654752f;
        private const float InvSqrt2Pi = 0.39894228040143267f;

        internal static float Gelu(float x)
        {
            return 0.5f * x * (1f + Erf(x * InvSqrt2));
        }

        internal static float GeluGrad(float x)
        {
            return 0.5f * (1f + Erf(x * InvSqrt2)) + x * InvSqrt2Pi * (float)Math.Exp(-0.5 * x * x);
        }

        /// <summary>
        /// Faster versions of the inner loops, when installed (Unity's Burst compiler, from Assets/PokemonTCG/Burst); null runs the
        /// plain C# ones. Set once at startup.
        /// </summary>
        public static IPolicyMath FastMath;

        private static IPolicyMath Kernels { get { return FastMath ?? ManagedPolicyMath.Instance; } }

        /// <summary>Which version of the inner loops the network is using ("Burst" or "C#").</summary>
        public static string MathName { get { return FastMath == null ? "C#" : FastMath.GetType().Name.Replace("PolicyMath", ""); } }

        /// <summary>y[o] = b[o] + sum_i W[o, i] * x[xOff + i].</summary>
        private static void Linear(float[] w, float[] b, int outN, int inN, float[] x, int xOff, float[] y, int yOff)
        {
            Array.Copy(b, 0, y, yOff, outN);
            Kernels.MatVecAcc(w, 0, inN, outN, inN, x, xOff, y, yOff);
        }


        /// <summary>dx[xOff + i] += sum_o W[o, i] * dy[o]; gW[o, i] += dy[o] * x[xOff + i]; gB[o] += dy[o].</summary>
        private static void LinearBack(float[] w, int outN, int inN, float[] x, int xOff, float[] dy, int dyOff, float[] dx, int dxOff, float[] gw, float[] gb)
        {
            if (gb != null) for (int o = 0; o < outN; o++) gb[o] += dy[dyOff + o];
            if (gw != null) Kernels.OuterAcc(gw, 0, inN, outN, inN, dy, dyOff, x, xOff);
            if (dx != null) Kernels.MatTVecAcc(w, 0, inN, outN, inN, dy, dyOff, dx, dxOff);
        }

        private static readonly int Lanes = Vector<float>.Count;
        /// <summary>Use the CPU's vector instructions (where the runtime supports them well).</summary>
        public static bool Simd = Vector.IsHardwareAccelerated && Environment.GetEnvironmentVariable("POLICY_NO_SIMD") == null;

        /// <summary>sum_i a[ao + i] * b[bo + i], with SIMD where the platform has it.</summary>
        internal static float Dot(float[] a, int ao, float[] b, int bo, int n)
        {
            int i = 0;
            float sum = 0f;
            if (Simd && n >= Lanes)
            {
                var acc = Vector<float>.Zero;
                int last = n - Lanes;
                for (; i <= last; i += Lanes) acc += new Vector<float>(a, ao + i) * new Vector<float>(b, bo + i);
                sum = Vector.Dot(acc, Vector<float>.One);
            }
            else
            {
                float s0 = 0f, s1 = 0f, s2 = 0f, s3 = 0f;
                int last4 = n - 4;
                for (; i <= last4; i += 4)
                {
                    s0 += a[ao + i] * b[bo + i];
                    s1 += a[ao + i + 1] * b[bo + i + 1];
                    s2 += a[ao + i + 2] * b[bo + i + 2];
                    s3 += a[ao + i + 3] * b[bo + i + 3];
                }
                sum = (s0 + s1) + (s2 + s3);
            }
            for (; i < n; i++) sum += a[ao + i] * b[bo + i];
            return sum;
        }

        /// <summary>y[yo + i] += alpha * x[xo + i].</summary>
        internal static void Axpy(float alpha, float[] x, int xo, float[] y, int yo, int n)
        {
            int i = 0;
            if (Simd && n >= Lanes)
            {
                var va = new Vector<float>(alpha);
                int last = n - Lanes;
                for (; i <= last; i += Lanes) (new Vector<float>(y, yo + i) + va * new Vector<float>(x, xo + i)).CopyTo(y, yo + i);
            }
            else
            {
                int last4 = n - 4;
                for (; i <= last4; i += 4)
                {
                    y[yo + i] += alpha * x[xo + i];
                    y[yo + i + 1] += alpha * x[xo + i + 1];
                    y[yo + i + 2] += alpha * x[xo + i + 2];
                    y[yo + i + 3] += alpha * x[xo + i + 3];
                }
            }
            for (; i < n; i++) y[yo + i] += alpha * x[xo + i];
        }

        private static void LayerNorm(float[] x, float[] g, float[] b, float[] xhat, float[] y, out float invStd)
        {
            int n = x.Length;
            double mean = 0;
            for (int i = 0; i < n; i++) mean += x[i];
            mean /= n;
            double var = 0;
            for (int i = 0; i < n; i++)
            {
                double d = x[i] - mean;
                var += d * d;
            }
            var /= n;
            invStd = (float)(1.0 / Math.Sqrt(var + LnEps));
            for (int i = 0; i < n; i++)
            {
                xhat[i] = (float)((x[i] - mean) * invStd);
                y[i] = xhat[i] * g[i] + b[i];
            }
        }

        private static void LayerNormBack(float[] dy, float[] xhat, float invStd, float[] g, float[] dx, float[] gg, float[] gb)
        {
            int n = dy.Length;
            double sum = 0, sumX = 0;
            var dxhat = new float[n];
            for (int i = 0; i < n; i++)
            {
                dxhat[i] = dy[i] * g[i];
                sum += dxhat[i];
                sumX += dxhat[i] * xhat[i];
                gg[i] += dy[i] * xhat[i];
                gb[i] += dy[i];
            }
            for (int i = 0; i < n; i++)
                dx[i] += (float)(invStd * (dxhat[i] - sum / n - xhat[i] * sumX / n));
        }

        // ------------------------------------------------------------------ Forward

        /// <summary>GELU and its derivative, computed exactly (they share the exponential). Used to fill the tables below.</summary>
        private static void GeluExact(double x, out double y, out double dy)
        {
            double ax = Math.Abs(x) * InvSqrt2;
            double t = 1.0 / (1.0 + 0.3275911 * ax);
            double e = Math.Exp(-ax * ax); // = exp(-x^2 / 2)
            double erf = 1.0 - (((((1.061405429 * t - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592) * t * e;
            if (x < 0) erf = -erf;
            y = 0.5 * x * (1 + erf);
            dy = 0.5 * (1 + erf) + x * InvSqrt2Pi * e;
        }

        // GELU and its derivative tabulated on [-GeluRange, GeluRange] (linear interpolation, error below 2e-6): an exponential
        // per number was the slowest part of the network. Outside the range GELU is x (or 0) to within 1e-14.
        private const float GeluRange = 8f;
        private const int GeluSteps = 256; // per unit
        private static readonly float[] GeluTable = BuildGeluTable(false), GeluGradTable = BuildGeluTable(true);

        private static float[] BuildGeluTable(bool grad)
        {
            int n = (int)(2 * GeluRange * GeluSteps) + 2;
            var t = new float[n];
            for (int i = 0; i < n; i++)
            {
                double x = -GeluRange + (double)i / GeluSteps;
                GeluExact(x, out double y, out double dy);
                t[i] = (float)(grad ? dy : y);
            }
            return t;
        }

        private static void GeluBoth(float x, out float y, out float dy)
        {
            if (x >= GeluRange)
            {
                y = x;
                dy = 1f;
                return;
            }
            if (!(x > -GeluRange)) // also NaN, so a bad number can never read outside the table
            {
                y = 0f;
                dy = 0f;
                return;
            }
            float f = (x + GeluRange) * GeluSteps;
            int i = (int)f;
            float w = f - i;
            y = GeluTable[i] + w * (GeluTable[i + 1] - GeluTable[i]);
            dy = GeluGradTable[i] + w * (GeluGradTable[i + 1] - GeluGradTable[i]);
        }

        private static void GeluAll(float[] pre, float[] act, float[] grad)
        {
            for (int j = 0; j < pre.Length; j++) GeluBoth(pre[j], out act[j], out grad[j]);
        }

        /// <summary>Everything one forward pass computed (the backward pass needs it).</summary>
        public sealed class Pass
        {
            internal readonly float[][] SlotInput = new float[PolicyInput.NumSlots][];
            internal readonly float[][] SlotAct = new float[PolicyInput.NumSlots][];
            internal readonly float[][] SlotGrad = new float[PolicyInput.NumSlots][];
            internal readonly float[] Slots = new float[PolicyInput.NumSlots * SlotDim];
            internal readonly float[] State = new float[StateSize];
            /// <summary>Parts of State that aren't all zeros (start, length pairs; empty slots and bags are skipped).</summary>
            internal int[] Segments;
            /// <summary>Parts of State whose gradient is needed (card vectors and slots; not the plain numbers).</summary>
            internal int[] GradSegments;
            internal readonly float[] InpGrad = new float[Width];
            // Per block (EnsureBlocks: as many as the network that last used this Pass, which threads share between networks).
            internal float[][] X = new float[0][];
            internal float[][] LnHat = new float[0][];
            internal float[][] LnOut = new float[0][];
            internal float[] LnInv = new float[0];
            internal float[][] HAct = new float[0][];
            internal float[][] HGrad = new float[0][];
            /// <summary>Blocks of the network whose forward pass this holds (the backward pass checks it).</summary>
            internal int Blocks;
            internal readonly float[] FinalHat = new float[Width];
            internal float FinalInv;
            internal readonly float[] H = new float[Width];
            internal readonly float[] Q = new float[OptionDim];
            internal float[][] OptIn = new float[0][], OptAct = new float[0][], OptGrad = new float[0][], ZAct = new float[0][],
                ZGrad = new float[0][], S1Act = new float[0][], S1Grad = new float[0][];
            internal readonly float[] SlotPre = new float[SlotDim];
            internal readonly float[] OptTmp = new float[OptionDim], OptOut = new float[OptionDim];
            internal readonly List<int> SegList = new List<int>(), GradSegList = new List<int>();
            internal readonly float[] VAct = new float[ValueHidden];
            internal readonly float[] VGrad = new float[ValueHidden];
            public float[] Logits;
            public float Value;

            /// <summary>Room for k options (the arrays are kept and reused when this Pass is used again).</summary>
            internal void EnsureOptions(int k)
            {
                if (OptIn.Length >= k) return;
                int size = Math.Max(k, OptIn.Length * 2);
                OptIn = Grow(OptIn, size, OptionIn);
                OptAct = Grow(OptAct, size, OptionDim);
                OptGrad = Grow(OptGrad, size, OptionDim);
                ZAct = Grow(ZAct, size, OptionDim);
                ZGrad = Grow(ZGrad, size, OptionDim);
                S1Act = Grow(S1Act, size, OptionDim);
                S1Grad = Grow(S1Grad, size, OptionDim);
            }

            private static float[][] Grow(float[][] a, int size, int width)
            {
                var b = new float[size][];
                Array.Copy(a, b, a.Length);
                for (int i = a.Length; i < size; i++) b[i] = new float[width];
                return b;
            }

            /// <summary>Room for a network with nb blocks.</summary>
            internal void EnsureBlocks(int nb)
            {
                Blocks = nb;
                if (LnInv.Length >= nb) return;
                X = Grow(X, nb + 1, Width);
                LnHat = Grow(LnHat, nb, Width);
                LnOut = Grow(LnOut, nb, Width);
                HAct = Grow(HAct, nb, Width);
                HGrad = Grow(HGrad, nb, Width);
                LnInv = new float[nb];
            }
        }

        /// <summary>Logits for the options and the value. Safe to call from many threads at once.</summary>
        public Pass Forward(PolicyInput x)
        {
            return ForwardMany(new[] { x })[0];
        }

        /// <summary>
        /// Just the option scores (a new array) and the value, working in this thread's reused memory: what playing needs, with
        /// almost nothing left for the garbage collector (Unity's is slow and stops everything while it works).
        /// </summary>
        public float[] Evaluate(PolicyInput x, out float value)
        {
            rw.EnterReadLock();
            try
            {
                var p = ForwardLocked(new[] { x }, true)[0];
                value = p.Value;
                return p.Logits;
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        /// <summary>
        /// ForwardMany in this thread's reused memory, for training: the passes are only good until this thread's next forward
        /// pass, so run the backward pass first.
        /// </summary>
        internal Pass[] ForwardPooled(IList<PolicyInput> xs)
        {
            rw.EnterReadLock();
            try
            {
                return ForwardLocked(xs, true);
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        [ThreadStatic] private static Pass[] passPool;
        [ThreadStatic] private static Dictionary<string, float[][]> scratchPool;

        private static Pass[] PassPool(int n)
        {
            var a = passPool;
            if (a == null || a.Length < n)
            {
                var b = new Pass[Math.Max(n, 32)];
                if (a != null) Array.Copy(a, b, a.Length);
                for (int i = 0; i < b.Length; i++) if (b[i] == null) b[i] = new Pass();
                passPool = a = b;
            }
            return a;
        }

        /// <summary>This thread's working arrays called name: at least n rows of length size (cleared when asked).</summary>
        private static float[][] Scratch(string name, int n, int size, bool clear)
        {
            var pool = scratchPool ?? (scratchPool = new Dictionary<string, float[][]>());
            float[][] a;
            if (!pool.TryGetValue(name, out a) || a.Length < n)
            {
                var b = new float[Math.Max(n, 32)][];
                if (a != null) Array.Copy(a, b, a.Length);
                for (int i = 0; i < b.Length; i++) if (b[i] == null) b[i] = new float[size];
                pool[name] = a = b;
            }
            if (clear) for (int i = 0; i < n; i++) Array.Clear(a[i], 0, size);
            if (a.Length != n)
            {
                var exact = new float[n][];
                Array.Copy(a, exact, n);
                return exact;
            }
            return a;
        }

        /// <summary>
        /// Forward passes for several decisions at once. The big layers run over the whole batch row by row, so each weight is
        /// read from memory once per batch instead of once per decision (several times faster when training).
        /// </summary>
        public Pass[] ForwardMany(IList<PolicyInput> xs)
        {
            rw.EnterReadLock();
            try
            {
                return ForwardLocked(xs, false);
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        private int bagOffset(int which)
        {
            // State layout: slots, hand, my discard, their discard, stadium, source, globals, memory bags, history bags, turn features.
            int o = PolicyInput.NumSlots * SlotDim;
            switch (which)
            {
                case PolicyInput.BagHand: return o;
                case PolicyInput.BagMyDiscard: return o + BagSize;
                case PolicyInput.BagOppDiscard: return o + 2 * BagSize;
            }
            int memory = o + 3 * BagSize + 2 * CardDim + PolicyInput.GlobalFeatures;
            if (which <= PolicyInput.BagOppSeen) return memory + (which - PolicyInput.BagUnseen) * BagSize;
            return memory + 3 * BagSize + (which - PolicyInput.BagTurnPlayed) * BagSize;
        }

        private const int StadiumOffset = PolicyInput.NumSlots * SlotDim + 3 * BagSize;
        private const int SourceOffset = StadiumOffset + CardDim;
        private const int GlobalsOffset = SourceOffset + CardDim;
        private const int TurnOffset = StateSize - PolicyInput.TurnFeatures;

        /// <summary>
        /// ys[k] = b + W xs[k] for each decision of a batch. Plain C# goes one weight row at a time for the whole batch (each row
        /// is read once); fast kernels take one decision at a time.
        /// </summary>
        private static void LinearBatch(float[] w, float[] b, int outN, int inN, float[][] xs, float[][] ys)
        {
            int n = xs.Length;
            if (FastMath != null)
            {
                for (int k = 0; k < n; k++) Linear(w, b, outN, inN, xs[k], 0, ys[k], 0);
                return;
            }
            for (int o = 0; o < outN; o++)
            {
                int row = o * inN;
                float bias = b[o];
                for (int k = 0; k < n; k++) ys[k][o] = bias + Dot(w, row, xs[k], 0, inN);
            }
        }

        /// <summary>Backward of LinearBatch: gW, gB and (if dxs isn't null) dxs[k] += W^T dys[k].</summary>
        private static void LinearBackBatch(float[] w, int outN, int inN, float[][] xs, float[][] dys, float[][] dxs, float[] gw, float[] gb)
        {
            int n = xs.Length;
            if (FastMath != null)
            {
                for (int k = 0; k < n; k++) LinearBack(w, outN, inN, xs[k], 0, dys[k], 0, dxs == null ? null : dxs[k], 0, gw, gb);
                return;
            }
            for (int o = 0; o < outN; o++)
            {
                int row = o * inN;
                for (int k = 0; k < n; k++)
                {
                    float d = dys[k][o];
                    if (d == 0f) continue;
                    gb[o] += d;
                    Axpy(d, xs[k], 0, gw, row, inN);
                    if (dxs != null) Axpy(d, w, row, dxs[k], 0, inN);
                }
            }
        }

        private Pass[] ForwardLocked(IList<PolicyInput> xs, bool pooled)
        {
            int n = xs.Count, nb = blocks;
            var ps = new Pass[n];
            var pool = pooled ? PassPool(n) : null;
            for (int i = 0; i < n; i++)
            {
                ps[i] = pooled ? pool[i] : new Pass();
                ps[i].EnsureBlocks(nb);
                Inputs(xs[i], ps[i]);
            }

            // Trunk, batched
            var pre = Scratch("pre", n, Width, false);
            // The input layer only over the parts of the state that aren't zeros (empty slots and bags are skipped).
            if (FastMath != null)
            {
                for (int i = 0; i < n; i++)
                {
                    var p = ps[i];
                    Array.Copy(inpB.Data, pre[i], Width);
                    var seg = p.Segments;
                    for (int k = 0; k < seg.Length; k += 2) FastMath.MatVecAcc(inpW.Data, seg[k], StateSize, Width, seg[k + 1], p.State, seg[k], pre[i], 0);
                }
            }
            else
            {
                for (int o = 0; o < Width; o++)
                {
                    int row = o * StateSize;
                    for (int i = 0; i < n; i++)
                    {
                        var p = ps[i];
                        float sum = inpB.Data[o];
                        var seg = p.Segments;
                        for (int k = 0; k < seg.Length; k += 2) sum += Dot(inpW.Data, row + seg[k], p.State, seg[k], seg[k + 1]);
                        pre[i][o] = sum;
                    }
                }
            }
            for (int i = 0; i < n; i++) GeluAll(pre[i], ps[i].X[0], ps[i].InpGrad);
            var tmp = Scratch("tmp", n, Width, false);
            for (int b = 0; b < nb; b++)
            {
                for (int i = 0; i < n; i++) LayerNorm(ps[i].X[b], lnG[b].Data, lnB[b].Data, ps[i].LnHat[b], ps[i].LnOut[b], out ps[i].LnInv[b]);
                LinearBatch(b1W[b].Data, b1B[b].Data, Width, Width, Each(ps, p => p.LnOut[b]), pre);
                for (int i = 0; i < n; i++) GeluAll(pre[i], ps[i].HAct[b], ps[i].HGrad[b]);
                LinearBatch(b3W[b].Data, b3B[b].Data, Width, Width, Each(ps, p => p.HAct[b]), tmp);
                for (int i = 0; i < n; i++)
                {
                    var xb = ps[i].X[b];
                    var xn = ps[i].X[b + 1];
                    var t = tmp[i];
                    for (int j = 0; j < Width; j++) xn[j] = xb[j] + t[j];
                }
            }
            for (int i = 0; i < n; i++) LayerNorm(ps[i].X[nb], finalLnG.Data, finalLnB.Data, ps[i].FinalHat, ps[i].H, out ps[i].FinalInv);
            var hs = Each(ps, p => p.H);
            LinearBatch(queryW.Data, queryB.Data, OptionDim, Width, hs, Each(ps, p => p.Q));
            var vpre = Scratch("vpre", n, ValueHidden, false);
            LinearBatch(value0W.Data, value0B.Data, ValueHidden, Width, hs, vpre);
            for (int i = 0; i < n; i++)
            {
                var p = ps[i];
                GeluAll(vpre[i], p.VAct, p.VGrad);
                p.Value = (float)Math.Tanh(value2B.Data[0] + Dot(value2W.Data, 0, p.VAct, 0, ValueHidden));
                Options(xs[i], p);
            }
            return ps;
        }

        private static float[][] Each(Pass[] ps, Func<Pass, float[]> pick)
        {
            var a = new float[ps.Length][];
            for (int i = 0; i < ps.Length; i++) a[i] = pick(ps[i]);
            return a;
        }

        /// <summary>The board slots and the state vector of one decision.</summary>
        private void Inputs(PolicyInput x, Pass p)
        {
            Array.Clear(p.Slots, 0, p.Slots.Length);
            Array.Clear(p.State, 0, p.State.Length);
            for (int s = 0; s < PolicyInput.NumSlots; s++)
            {
                if (x.SlotCard[s] <= 0) continue;
                var inp = p.SlotInput[s] ?? (p.SlotInput[s] = new float[SlotIn]);
                var act = p.SlotAct[s] ?? (p.SlotAct[s] = new float[SlotDim]);
                var grad = p.SlotGrad[s] ?? (p.SlotGrad[s] = new float[SlotDim]);
                Array.Clear(inp, 0, SlotIn);
                var cv = CardVector(x.SlotCard[s]);
                Array.Copy(cv, 0, inp, 0, CardDim);
                var tv = CardVector(x.SlotTool[s]);
                if (tv != null) Array.Copy(tv, 0, inp, CardDim, CardDim);
                Array.Copy(x.SlotFeat, s * PolicyInput.SlotFeatures, inp, 2 * CardDim, PolicyInput.SlotFeatures);
                Linear(slot0W.Data, slot0B.Data, SlotDim, SlotIn, inp, 0, p.SlotPre, 0);
                GeluAll(p.SlotPre, act, grad);
                Linear(slot2W.Data, slot2B.Data, SlotDim, SlotDim, act, 0, p.Slots, s * SlotDim);
            }
            var st = p.State;
            Array.Copy(p.Slots, 0, st, 0, p.Slots.Length);
            for (int b = 0; b < PolicyInput.BagCount; b++)
            {
                int off = bagOffset(b);
                int n = 0;
                foreach (int row in x.Bags[b])
                {
                    var cv = CardVector(row);
                    if (cv == null) continue;
                    for (int j = 0; j < CardDim; j++) st[off + j] += cv[j];
                    n++;
                }
                if (n > 0) for (int j = 0; j < CardDim; j++) st[off + j] /= n;
                st[off + CardDim] = n / 20f;
            }
            var sv = CardVector(x.Stadium);
            if (sv != null) Array.Copy(sv, 0, st, StadiumOffset, CardDim);
            var srcv = CardVector(x.Source);
            if (srcv != null) Array.Copy(srcv, 0, st, SourceOffset, CardDim);
            Array.Copy(x.Globals, 0, st, GlobalsOffset, PolicyInput.GlobalFeatures);
            Array.Copy(x.TurnFeats, 0, st, TurnOffset, PolicyInput.TurnFeatures);

            var segs = p.SegList;
            var grads = p.GradSegList;
            segs.Clear();
            grads.Clear();
            for (int s = 0; s < PolicyInput.NumSlots; s++)
            {
                if (x.SlotCard[s] <= 0) continue;
                Segment(segs, s * SlotDim, SlotDim);
                Segment(grads, s * SlotDim, SlotDim);
            }
            for (int b = 0; b < PolicyInput.BagCount; b++)
            {
                int off = bagOffset(b);
                if (st[off + CardDim] == 0f) continue; // empty bag: all zeros
                Segment(segs, off, BagSize);
                Segment(grads, off, CardDim);
            }
            if (sv != null)
            {
                Segment(segs, StadiumOffset, CardDim);
                Segment(grads, StadiumOffset, CardDim);
            }
            if (srcv != null)
            {
                Segment(segs, SourceOffset, CardDim);
                Segment(grads, SourceOffset, CardDim);
            }
            Segment(segs, GlobalsOffset, PolicyInput.GlobalFeatures);
            Segment(segs, TurnOffset, PolicyInput.TurnFeatures);
            p.Segments = segs.ToArray();
            p.GradSegments = grads.ToArray();
        }

        /// <summary>Adds [start, start + length) to a list of (start, length) pairs, joining it to the last one when they touch.</summary>
        private static void Segment(List<int> segs, int start, int length)
        {
            int n = segs.Count;
            if (n >= 2 && segs[n - 2] + segs[n - 1] == start) segs[n - 1] += length;
            else
            {
                segs.Add(start);
                segs.Add(length);
            }
        }

        /// <summary>Scores each option of one decision against its state.</summary>
        private void Options(PolicyInput x, Pass p)
        {
            int k = x.Count;
            p.EnsureOptions(k);
            p.Logits = new float[k];
            var tmp = p.OptTmp;
            var o2 = p.OptOut;
            for (int i = 0; i < k; i++)
            {
                var oin = p.OptIn[i];
                Array.Clear(oin, 0, OptionIn);
                Array.Copy(kind.Data, x.OptKind[i] * CardDim, oin, 0, CardDim);
                var cv = CardVector(x.OptCard[i]);
                if (cv != null) Array.Copy(cv, 0, oin, CardDim, CardDim);
                int t = x.OptTarget[i];
                if (t >= 0 && t < PolicyInput.NumSlots) Array.Copy(p.Slots, t * SlotDim, oin, 2 * CardDim, SlotDim);
                else Array.Copy(noneSlot.Data, 0, oin, 2 * CardDim, SlotDim);
                Array.Copy(x.OptFeat, i * PolicyInput.OptionFeatures, oin, 2 * CardDim + SlotDim, PolicyInput.OptionFeatures);
                Linear(opt0W.Data, opt0B.Data, OptionDim, OptionIn, oin, 0, tmp, 0);
                var act = p.OptAct[i];
                var grad = p.OptGrad[i];
                GeluAll(tmp, act, grad);
                Linear(opt2W.Data, opt2B.Data, OptionDim, OptionDim, act, 0, o2, 0);
                for (int j = 0; j < OptionDim; j++) tmp[j] = o2[j] + p.Q[j];
                var zAct = p.ZAct[i];
                var zGrad = p.ZGrad[i];
                GeluAll(tmp, zAct, zGrad);
                Linear(score1W.Data, score1B.Data, OptionDim, OptionDim, zAct, 0, tmp, 0);
                var s1Act = p.S1Act[i];
                var s1Grad = p.S1Grad[i];
                GeluAll(tmp, s1Act, s1Grad);
                p.Logits[i] = score3B.Data[0] + Dot(score3W.Data, 0, s1Act, 0, OptionDim);
            }
        }

        // ------------------------------------------------------------------ Backward

        /// <summary>Gradients of every parameter (one per training thread; Add them up before Apply).</summary>
        public sealed class Grad
        {
            internal readonly float[][] G;
            internal readonly Dictionary<int, float[]> Cards = new Dictionary<int, float[]>();
            public int Samples;
            public double Loss, PolicyLoss, ValueLoss, Entropy, ClipFraction;

            /// <summary>Back to zero, to be used again (a Grad is 8 MB: reusing it spares the garbage collector).</summary>
            public void Clear()
            {
                foreach (var a in G) Array.Clear(a, 0, a.Length);
                Cards.Clear();
                Samples = 0;
                Loss = PolicyLoss = ValueLoss = Entropy = ClipFraction = 0;
            }

            /// <summary>Sized for this network (false once it has grown).</summary>
            public bool Fits(PolicyNet net)
            {
                return G.Length == net.parameters.Count;
            }

            internal Grad(PolicyNet net)
            {
                G = new float[net.parameters.Count][];
                for (int i = 0; i < G.Length; i++) G[i] = new float[net.parameters[i].Data.Length];
            }

            public void Add(Grad other)
            {
                for (int i = 0; i < G.Length; i++)
                {
                    var a = G[i];
                    var b = other.G[i];
                    for (int j = 0; j < a.Length; j++) a[j] += b[j];
                }
                foreach (var kv in other.Cards)
                {
                    float[] row;
                    if (!Cards.TryGetValue(kv.Key, out row)) Cards[kv.Key] = row = new float[CardDim];
                    for (int j = 0; j < CardDim; j++) row[j] += kv.Value[j];
                }
                Samples += other.Samples;
                Loss += other.Loss;
                PolicyLoss += other.PolicyLoss;
                ValueLoss += other.ValueLoss;
                Entropy += other.Entropy;
                ClipFraction += other.ClipFraction;
            }
        }

        public Grad NewGrad()
        {
            return new Grad(this);
        }

        private float[] G(Grad g, Param p)
        {
            return g.G[p.Index];
        }

        private void CardGrad(Grad g, int row, float[] d, int off, float scale)
        {
            if (row <= 0) return;
            float[] acc;
            if (!g.Cards.TryGetValue(row, out acc)) g.Cards[row] = acc = new float[CardDim];
            for (int j = 0; j < CardDim; j++) acc[j] += scale * d[off + j];
        }

        /// <summary>
        /// Adds the gradients of a loss to g, given dLoss/dLogit for each option and dLoss/dValue (after the tanh).
        /// Call with the Pass from Forward on the same weights (the trainer holds them still while it collects gradients).
        /// </summary>
        public void Backward(PolicyInput x, Pass p, float[] dLogits, float dValue, Grad g)
        {
            BackwardMany(new[] { x }, new[] { p }, new[] { dLogits }, new[] { dValue }, g);
        }

        /// <summary>Backward passes for a batch made by ForwardMany (same weights).</summary>
        public void BackwardMany(IList<PolicyInput> xs, Pass[] ps, float[][] dLogits, float[] dValues, Grad g)
        {
            rw.EnterReadLock();
            try
            {
                BackwardLocked(xs, ps, dLogits, dValues, g);
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        private void BackwardLocked(IList<PolicyInput> xs, Pass[] ps, float[][] dLogits, float[] dValues, Grad g)
        {
            int n = ps.Length;
            if (!g.Fits(this)) throw new InvalidOperationException("the gradient buffer is for a different size of network");
            foreach (var p in ps)
                if (p.Blocks != blocks) throw new InvalidOperationException("the network changed size between its forward and backward passes");
            var dh = Scratch("dh", n, Width, true);
            var dq = Scratch("dq", n, OptionDim, true);
            var dvPre = Scratch("dvPre", n, ValueHidden, true);
            var dSlots = Scratch("dSlots", n, PolicyInput.NumSlots * SlotDim, true);
            float[] gS3W = G(g, score3W), gS3B = G(g, score3B), gS1W = G(g, score1W), gS1B = G(g, score1B),
                gO2W = G(g, opt2W), gO2B = G(g, opt2B), gO0W = G(g, opt0W), gO0B = G(g, opt0B), gKind = G(g, kind), gNone = G(g, noneSlot),
                gV2W = G(g, value2W), gV2B = G(g, value2B);
            var ds1 = new float[OptionDim];
            var dzAct = new float[OptionDim];
            var dz = new float[OptionDim];
            var dAct = new float[OptionDim];
            var dPre = new float[OptionDim];
            var dIn = new float[OptionIn];

            // Heads, one decision at a time
            for (int k = 0; k < n; k++)
            {
                var x = xs[k];
                var p = ps[k];
                float dValue = dValues[k];
                if (dValue != 0f)
                {
                    float dv = dValue * (1f - p.Value * p.Value);
                    gV2B[0] += dv;
                    for (int j = 0; j < ValueHidden; j++)
                    {
                        gV2W[j] += dv * p.VAct[j];
                        dvPre[k][j] = dv * value2W.Data[j] * p.VGrad[j];
                    }
                }
                var dl = dLogits[k];
                for (int i = 0; i < x.Count; i++)
                {
                    float d = dl[i];
                    if (d == 0f) continue;
                    gS3B[0] += d;
                    var s1Act = p.S1Act[i];
                    var s1Grad = p.S1Grad[i];
                    for (int j = 0; j < OptionDim; j++)
                    {
                        gS3W[j] += d * s1Act[j];
                        ds1[j] = d * score3W.Data[j] * s1Grad[j];
                    }
                    Array.Clear(dzAct, 0, OptionDim);
                    LinearBack(score1W.Data, OptionDim, OptionDim, p.ZAct[i], 0, ds1, 0, dzAct, 0, gS1W, gS1B);
                    var zGrad = p.ZGrad[i];
                    var dqk = dq[k];
                    for (int j = 0; j < OptionDim; j++)
                    {
                        dz[j] = dzAct[j] * zGrad[j];
                        dqk[j] += dz[j];
                    }
                    Array.Clear(dAct, 0, OptionDim);
                    LinearBack(opt2W.Data, OptionDim, OptionDim, p.OptAct[i], 0, dz, 0, dAct, 0, gO2W, gO2B);
                    var optGrad = p.OptGrad[i];
                    for (int j = 0; j < OptionDim; j++) dPre[j] = dAct[j] * optGrad[j];
                    Array.Clear(dIn, 0, OptionIn);
                    LinearBack(opt0W.Data, OptionDim, OptionIn, p.OptIn[i], 0, dPre, 0, dIn, 0, gO0W, gO0B);
                    int krow = x.OptKind[i] * CardDim;
                    for (int j = 0; j < CardDim; j++) gKind[krow + j] += dIn[j];
                    CardGrad(g, x.OptCard[i], dIn, CardDim, 1f);
                    int t = x.OptTarget[i];
                    if (t >= 0 && t < PolicyInput.NumSlots)
                        for (int j = 0; j < SlotDim; j++) dSlots[k][t * SlotDim + j] += dIn[2 * CardDim + j];
                    else
                        for (int j = 0; j < SlotDim; j++) gNone[j] += dIn[2 * CardDim + j];
                }
            }

            // Trunk, batched
            var hs = Each(ps, p => p.H);
            LinearBackBatch(queryW.Data, OptionDim, Width, hs, dq, dh, G(g, queryW), G(g, queryB));
            LinearBackBatch(value0W.Data, ValueHidden, Width, hs, dvPre, dh, G(g, value0W), G(g, value0B));
            var dx = Scratch("dx", n, Width, true);
            for (int k = 0; k < n; k++) LayerNormBack(dh[k], ps[k].FinalHat, ps[k].FinalInv, finalLnG.Data, dx[k], G(g, finalLnG), G(g, finalLnB));
            var dHAct = Scratch("dHAct", n, Width, true);
            var dLn = Scratch("dLn", n, Width, true);
            for (int b = blocks - 1; b >= 0; b--)
            {
                // x[b+1] = x[b] + W3 gelu(W1 LN(x[b])): dx passes straight through, plus the branch
                for (int k = 0; k < n; k++)
                {
                    Array.Clear(dHAct[k], 0, Width);
                    Array.Clear(dLn[k], 0, Width);
                }
                LinearBackBatch(b3W[b].Data, Width, Width, Each(ps, p => p.HAct[b]), dx, dHAct, G(g, b3W[b]), G(g, b3B[b]));
                for (int k = 0; k < n; k++)
                {
                    var hg = ps[k].HGrad[b];
                    var d = dHAct[k];
                    for (int j = 0; j < Width; j++) d[j] *= hg[j];
                }
                LinearBackBatch(b1W[b].Data, Width, Width, Each(ps, p => p.LnOut[b]), dHAct, dLn, G(g, b1W[b]), G(g, b1B[b]));
                for (int k = 0; k < n; k++) LayerNormBack(dLn[k], ps[k].LnHat[b], ps[k].LnInv[b], lnG[b].Data, dx[k], G(g, lnG[b]), G(g, lnB[b]));
            }
            for (int k = 0; k < n; k++)
            {
                var ig = ps[k].InpGrad;
                var d = dx[k];
                for (int j = 0; j < Width; j++) d[j] *= ig[j];
            }
            var dState = Scratch("dState", n, StateSize, true);
            float[] gInpW = G(g, inpW), gInpB = G(g, inpB);
            if (FastMath != null)
            {
                for (int i = 0; i < n; i++)
                {
                    var p = ps[i];
                    var d = dx[i];
                    for (int o = 0; o < Width; o++) gInpB[o] += d[o];
                    var seg = p.Segments;
                    for (int k = 0; k < seg.Length; k += 2) FastMath.OuterAcc(gInpW, seg[k], StateSize, Width, seg[k + 1], d, 0, p.State, seg[k]);
                    seg = p.GradSegments;
                    for (int k = 0; k < seg.Length; k += 2) FastMath.MatTVecAcc(inpW.Data, seg[k], StateSize, Width, seg[k + 1], d, 0, dState[i], seg[k]);
                }
            }
            else
            {
                for (int o = 0; o < Width; o++)
                {
                    int row = o * StateSize;
                    for (int i = 0; i < n; i++)
                    {
                        float d = dx[i][o];
                        if (d == 0f) continue;
                        gInpB[o] += d;
                        var p = ps[i];
                        var seg = p.Segments;
                        for (int k = 0; k < seg.Length; k += 2) Axpy(d, p.State, seg[k], gInpW, row + seg[k], seg[k + 1]);
                        seg = p.GradSegments;
                        for (int k = 0; k < seg.Length; k += 2) Axpy(d, inpW.Data, row + seg[k], dState[i], seg[k], seg[k + 1]);
                    }
                }
            }

            // State pieces and slots, one decision at a time
            var dSIn = new float[SlotIn];
            var dSPre = new float[SlotDim];
            float[] gS2W = G(g, slot2W), gS2B = G(g, slot2B), gS0W = G(g, slot0W), gS0B = G(g, slot0B);
            for (int k = 0; k < n; k++)
            {
                var x = xs[k];
                var p = ps[k];
                var ds = dState[k];
                var dsl = dSlots[k];
                for (int j = 0; j < dsl.Length; j++) dsl[j] += ds[j];
                for (int b = 0; b < PolicyInput.BagCount; b++)
                {
                    int off = bagOffset(b);
                    int cnt = 0;
                    foreach (int row in x.Bags[b]) if (row > 0) cnt++;
                    if (cnt == 0) continue;
                    foreach (int row in x.Bags[b]) CardGrad(g, row, ds, off, 1f / cnt);
                }
                CardGrad(g, x.Stadium, ds, StadiumOffset, 1f);
                CardGrad(g, x.Source, ds, SourceOffset, 1f);
                for (int s = 0; s < PolicyInput.NumSlots; s++)
                {
                    if (x.SlotCard[s] <= 0) continue;
                    Array.Clear(dSPre, 0, SlotDim);
                    LinearBack(slot2W.Data, SlotDim, SlotDim, p.SlotAct[s], 0, dsl, s * SlotDim, dSPre, 0, gS2W, gS2B);
                    var sg = p.SlotGrad[s];
                    for (int j = 0; j < SlotDim; j++) dSPre[j] *= sg[j];
                    Array.Clear(dSIn, 0, SlotIn);
                    LinearBack(slot0W.Data, SlotDim, SlotIn, p.SlotInput[s], 0, dSPre, 0, dSIn, 0, gS0W, gS0B);
                    CardGrad(g, x.SlotCard[s], dSIn, 0, 1f);
                    CardGrad(g, x.SlotTool[s], dSIn, CardDim, 1f);
                }
                g.Samples++;
            }
        }

        /// <summary>Compares analytic gradients with finite differences on a few weights (headless tests).</summary>
        public void GradCheck(Grad g, Func<double> loss, PolicyInput x)
        {
            var rng = new Rng(7);
            foreach (var p in parameters)
            {
                double worst = 0;
                var gp = g.G[p.Index];
                var picks = new List<int>();
                for (int t = 0; t < Math.Min(3, p.Data.Length); t++)
                {
                    int best = -1;
                    for (int tries = 0; tries < 3000 && picks.Count < p.Data.Length; tries++)
                    {
                        int c = rng.Next(p.Data.Length);
                        if (!picks.Contains(c) && (best < 0 || Math.Abs(gp[c]) > Math.Abs(gp[best]))) best = c;
                    }
                    picks.Add(best);
                }
                foreach (int i in picks)
                {
                    float old = p.Data[i];
                    const float h = 1e-2f;
                    p.Data[i] = old + h; cardCache = new float[0][];
                    double up = loss();
                    p.Data[i] = old - h; cardCache = new float[0][];
                    double down = loss();
                    p.Data[i] = old; cardCache = new float[0][];
                    double num = (up - down) / (2 * h);
                    double ana = g.G[p.Index][i];
                    double err = Math.Abs(num - ana) / Math.Max(1e-3, Math.Abs(num) + Math.Abs(ana));
                    if (err > worst) worst = err;
                }
                Console.WriteLine(p.Name.PadRight(28) + " worst relative error " + worst.ToString("g3"));
            }
            foreach (var kv in g.Cards)
            {
                var row = cardRows[kv.Key];
                float old = row[0];
                const float h = 1e-2f;
                row[0] = old + h; cardCache = new float[0][]; double up = loss();
                row[0] = old - h; cardCache = new float[0][]; double down = loss();
                row[0] = old; cardCache = new float[0][];
                double num = (up - down) / (2 * h);
                Console.WriteLine("card row " + kv.Key + ": numeric " + num.ToString("g4") + " analytic " + kv.Value[0].ToString("g4"));
                break;
            }
        }

        // ------------------------------------------------------------------ Optimizer

        /// <summary>
        /// One Adam step with the summed gradients of a minibatch (divided by its size), after clipping their overall size to
        /// MaxGradNorm. Card rows only move when they were used.
        /// </summary>
        public void Apply(Grad g, int batch)
        {
            if (batch <= 0) return;
            if (!g.Fits(this)) throw new InvalidOperationException("the gradient buffer is for a different size of network");
            float scale = 1f / batch;
            // Card-vector gradients become gradients of the card rows and the stats projection.
            var gProjW = G(g, projW);
            var gProjB = G(g, projB);
            var rowGrads = new Dictionary<int, float[]>();
            foreach (var kv in g.Cards)
            {
                float[] s;
                lock (cardLock) s = cardStatic[kv.Key];
                var d = kv.Value;
                for (int o = 0; o < CardDim; o++)
                {
                    gProjB[o] += d[o];
                    int off = o * StaticSize;
                    for (int i = 0; i < StaticSize; i++) gProjW[off + i] += d[o] * s[i];
                }
                rowGrads[kv.Key] = d;
            }
            double norm = 0;
            foreach (var a in g.G) foreach (float v in a) norm += (double)v * v * scale * scale;
            foreach (var d in rowGrads.Values) foreach (float v in d) norm += (double)v * v * scale * scale;
            norm = Math.Sqrt(norm);
            if (double.IsNaN(norm) || double.IsInfinity(norm))
            {
                // A step this broken would ruin every weight: skip it.
                Interlocked.Increment(ref skippedSteps);
                return;
            }
            if (norm > MaxGradNorm) scale *= (float)(MaxGradNorm / (norm + 1e-6));

            rw.EnterWriteLock();
            try
            {
                adamT++;
                double b1 = 0.9, b2 = 0.999, eps = 1e-8;
                double c1 = 1 - Math.Pow(b1, adamT), c2 = 1 - Math.Pow(b2, adamT);
                double lr = LearningRate * Math.Sqrt(c2) / c1;
                for (int k = 0; k < parameters.Count; k++)
                {
                    var p = parameters[k];
                    if (p.M == null)
                    {
                        p.M = new float[p.Data.Length];
                        p.V = new float[p.Data.Length];
                    }
                    AdamUpdate(p.Data, p.M, p.V, g.G[k], scale, b1, b2, eps, lr);
                }
                lock (cardLock)
                {
                    foreach (var kv in rowGrads)
                    {
                        int row = kv.Key;
                        if (cardM[row] == null)
                        {
                            cardM[row] = new float[CardDim];
                            cardV[row] = new float[CardDim];
                        }
                        AdamUpdate(cardRows[row], cardM[row], cardV[row], kv.Value, scale, b1, b2, eps, lr);
                    }
                }
                cardCache = new float[0][];
                Interlocked.Increment(ref steps);
            }
            finally
            {
                weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                rw.ExitWriteLock();
            }
        }

        private static void AdamUpdate(float[] w, float[] m, float[] v, float[] grad, float scale, double b1, double b2, double eps, double lr)
        {
            for (int i = 0; i < w.Length; i++)
            {
                double gr = grad[i] * scale;
                m[i] = (float)(b1 * m[i] + (1 - b1) * gr);
                v[i] = (float)(b2 * v[i] + (1 - b2) * gr * gr);
                w[i] -= (float)(lr * m[i] / (Math.Sqrt(v[i]) + eps));
            }
        }

        /// <summary>
        /// One learning step toward these choices (cross-entropy on the policy; the value head is left alone). strength scales
        /// the step.
        /// </summary>
        public void Imitate(List<KeyValuePair<PolicyInput, int>> picks, float strength)
        {
            var g = NewGrad();
            var batch = new List<KeyValuePair<PolicyInput, int>>();
            foreach (var kv in picks)
                if (kv.Key != null && kv.Value >= 0 && kv.Value < kv.Key.Count && kv.Key.Count >= 2) batch.Add(kv);
            for (int b = 0; b < batch.Count; b += 32)
            {
                var part = batch.GetRange(b, Math.Min(32, batch.Count - b));
                var xs = new PolicyInput[part.Count];
                for (int k = 0; k < part.Count; k++) xs[k] = part[k].Key;
                var passes = ForwardPooled(xs);
                var dl = new float[part.Count][];
                for (int k = 0; k < part.Count; k++)
                {
                    var p = Softmax(passes[k].Logits);
                    var d = new float[xs[k].Count];
                    for (int i = 0; i < d.Length; i++) d[i] = strength * (float)(p[i] - (i == part[k].Value ? 1 : 0));
                    dl[k] = d;
                }
                BackwardMany(xs, passes, dl, new float[part.Count], g);
            }
            if (batch.Count > 0) Apply(g, batch.Count);
        }

        // ------------------------------------------------------------------ Policy helpers

        /// <summary>Softmax of the logits.</summary>
        public static double[] Softmax(float[] logits)
        {
            var p = new double[logits.Length];
            if (logits.Length == 0) return p;
            double max = double.MinValue;
            foreach (float l in logits) if (l > max) max = l;
            double z = 0;
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = Math.Exp(logits[i] - max);
                z += p[i];
            }
            for (int i = 0; i < p.Length; i++) p[i] /= z;
            return p;
        }

        // ------------------------------------------------------------------ Saving and loading

        /// <summary>The whole network (weights, card table, counters) as bytes.</summary>
        public byte[] ToBytes()
        {
            rw.EnterReadLock();
            try
            {
                using (var ms = new MemoryStream())
                using (var w = new BinaryWriter(ms))
                {
                    w.Write(Encoding.ASCII.GetBytes("PTCGNET1"));
                    w.Write(FormatVersion);
                    w.Write(steps);
                    w.Write(GamesTrained);
                    w.Write(AverageLoss);
                    w.Write(Origin ?? "");
                    w.Write(parameters.Count);
                    foreach (var p in parameters)
                    {
                        w.Write(p.Name);
                        w.Write(p.Data.Length);
                        foreach (float v in p.Data) w.Write(v);
                    }
                    lock (cardLock)
                    {
                        w.Write(cardRows.Count);
                        for (int r = 1; r < cardRows.Count; r++)
                        {
                            w.Write(cardKeys[r]);
                            foreach (float v in cardRows[r]) w.Write(v);
                            foreach (float v in cardStatic[r]) w.Write(v);
                        }
                        // Other names a row answers to (Python printings, matched names)
                        var aliases = new List<KeyValuePair<string, int>>();
                        foreach (var kv in rowOfKey) if (kv.Value < cardKeys.Count && cardKeys[kv.Value] != kv.Key) aliases.Add(kv);
                        w.Write(aliases.Count);
                        foreach (var kv in aliases)
                        {
                            w.Write(kv.Key);
                            w.Write(kv.Value);
                        }
                    }
                    w.Flush();
                    return ms.ToArray();
                }
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        // ------------------------------------------------------------------ The GPU learner (GpuLearner.cs, GpuLearner/learner.py)

        /// <summary>
        /// Writes the weights for the GPU learner: each tensor (name, shape, numbers), then the card table's learned rows and,
        /// with stats, their printed stats. Strings are an int32 byte length and UTF-8.
        /// </summary>
        public int WriteLearnerWeights(BinaryWriter w, bool withStats)
        {
            int rows;
            rw.EnterReadLock();
            try
            {
                w.Write(parameters.Count);
                foreach (var p in parameters)
                {
                    GpuLearner.WriteString(w, p.Name);
                    w.Write(p.Shape.Length);
                    foreach (int d in p.Shape) w.Write(d);
                    foreach (float v in p.Data) w.Write(v);
                }
                lock (cardLock)
                {
                    rows = cardRows.Count;
                    w.Write(rows);
                    foreach (var row in cardRows) foreach (float v in row) w.Write(v);
                    if (withStats) foreach (var st in cardStatic) foreach (float v in st) w.Write(v);
                }
            }
            finally
            {
                rw.ExitReadLock();
            }
            return rows;
        }

        /// <summary>
        /// The card rows from row from on (cards met since the GPU learner last had the table): their count, learned rows and
        /// printed stats. Returns the card table's size it wrote up to.
        /// </summary>
        public int WriteNewCardRows(BinaryWriter w, int from)
        {
            rw.EnterReadLock();
            try
            {
                lock (cardLock)
                {
                    int rows = cardRows.Count;
                    w.Write(from);
                    w.Write(rows - from);
                    for (int i = from; i < rows; i++) foreach (float v in cardRows[i]) w.Write(v);
                    for (int i = from; i < rows; i++) foreach (float v in cardStatic[i]) w.Write(v);
                    return rows;
                }
            }
            finally
            {
                rw.ExitReadLock();
            }
        }

        /// <summary>
        /// Takes the weights the GPU learner sends back after learning (WriteLearnerWeights' layout, without stats): every tensor
        /// by name, and the card rows it had (a card added here meanwhile keeps its row). Counts the learner's steps as this
        /// network's. Refuses numbers that aren't finite, leaving the weights as they were.
        /// </summary>
        public void ReadLearnerWeights(BinaryReader r, int stepsTaken)
        {
            int count = r.ReadInt32();
            var data = new Dictionary<string, float[]>(StringComparer.Ordinal);
            for (int k = 0; k < count; k++)
            {
                string name = GpuLearner.ReadString(r);
                int nd = r.ReadInt32();
                int n = 1;
                for (int d = 0; d < nd; d++) n *= r.ReadInt32();
                var a = new float[n];
                for (int i = 0; i < n; i++)
                {
                    a[i] = r.ReadSingle();
                    if (float.IsNaN(a[i]) || float.IsInfinity(a[i])) throw new InvalidDataException("the learner sent a weight that isn't a number");
                }
                data[name] = a;
            }
            foreach (var p in parameters)
            {
                float[] a;
                if (!data.TryGetValue(p.Name, out a) || a.Length != p.Data.Length) throw new InvalidDataException("the learner's weights don't fit this network (" + p.Name + ")");
            }
            int rows = r.ReadInt32();
            var learned = new float[rows][];
            for (int row = 0; row < rows; row++)
            {
                var e = new float[CardDim];
                for (int j = 0; j < CardDim; j++)
                {
                    e[j] = r.ReadSingle();
                    if (float.IsNaN(e[j]) || float.IsInfinity(e[j])) throw new InvalidDataException("the learner sent a card row that isn't a number");
                }
                learned[row] = e;
            }
            rw.EnterWriteLock();
            try
            {
                foreach (var p in parameters)
                {
                    p.Data = data[p.Name];
                    p.M = null; // the learner keeps the optimizer's state
                    p.V = null;
                }
                lock (cardLock)
                {
                    for (int row = 1; row < rows && row < cardRows.Count; row++)
                    {
                        cardRows[row] = learned[row];
                        cardM[row] = null;
                        cardV[row] = null;
                    }
                }
                cardCache = new float[0][];
                steps += stepsTaken;
            }
            finally
            {
                weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                rw.ExitWriteLock();
            }
        }

        /// <summary>Loads ToBytes() output, or a file exported from the Python bot (tools/export_to_sim.py). False if it isn't one.</summary>
        public bool FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length < 12) return false;
            string magic = Encoding.ASCII.GetString(bytes, 0, 8);
            try
            {
                if (magic == "PTCGNET1") return LoadOwn(bytes);
                if (magic == "PTCGPOL1") return LoadPython(bytes);
            }
            catch (EndOfStreamException)
            {
            }
            catch (IOException)
            {
            }
            catch (ArgumentException)
            {
            }
            catch (InvalidDataException)
            {
            }
            return false;
        }

        private bool LoadOwn(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var r = new BinaryReader(ms))
            {
                r.ReadBytes(8);
                if (r.ReadInt32() != FormatVersion) return false;
                long st = r.ReadInt64();
                long games = r.ReadInt64();
                double loss = r.ReadDouble();
                string origin = r.ReadString();
                int count = r.ReadInt32();
                var data = new Dictionary<string, float[]>(StringComparer.Ordinal);
                for (int k = 0; k < count; k++)
                {
                    string name = r.ReadString();
                    int n = r.ReadInt32();
                    var a = new float[n];
                    for (int i = 0; i < n; i++) a[i] = Clean(r.ReadSingle());
                    data[name] = a;
                }
                int fileStatics = Widen(data);
                int nb = BlocksIn(data);
                if (!Fits(data, nb)) return false;
                int rows = r.ReadInt32();
                var keys = new List<string> { "" };
                var learned = new List<float[]> { new float[CardDim] };
                var statics = new List<float[]> { new float[StaticSize] };
                for (int row = 1; row < rows; row++)
                {
                    keys.Add(r.ReadString());
                    var e = new float[CardDim];
                    for (int j = 0; j < CardDim; j++) e[j] = Clean(r.ReadSingle());
                    var s = new float[StaticSize];
                    for (int j = 0; j < fileStatics; j++) s[j] = Clean(r.ReadSingle());
                    learned.Add(e);
                    statics.Add(s);
                }
                var aliases = new List<KeyValuePair<string, int>>();
                int na = r.ReadInt32();
                for (int k = 0; k < na; k++) aliases.Add(new KeyValuePair<string, int>(r.ReadString(), r.ReadInt32()));

                rw.EnterWriteLock();
                try
                {
                    TakeWeights(data, nb);
                    SetCards(keys, learned, statics, aliases);
                    steps = st;
                    adamT = 0;
                    GamesTrained = games;
                    AverageLoss = loss;
                    Origin = origin;
                }
                finally
                {
                    weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                    rw.ExitWriteLock();
                }
                return true;
            }
        }

        private bool LoadPython(byte[] bytes)
        {
            using (var ms = new MemoryStream(bytes))
            using (var r = new BinaryReader(ms))
            {
                r.ReadBytes(8);
                if (r.ReadInt32() != 1) return false;
                int count = r.ReadInt32();
                var data = new Dictionary<string, float[]>(StringComparer.Ordinal);
                for (int k = 0; k < count; k++)
                {
                    string name = ReadPyString(r);
                    int nd = r.ReadInt32();
                    int n = 1;
                    for (int d = 0; d < nd; d++) n *= r.ReadInt32();
                    var a = new float[n];
                    for (int i = 0; i < n; i++) a[i] = Clean(r.ReadSingle());
                    data[name] = a;
                }
                int fileStatics = Widen(data);
                int nb = BlocksIn(data);
                if (!Fits(data, nb)) return false;
                float[] ids;
                if (!data.TryGetValue("cards.ids.weight", out ids)) return false;
                int cards = r.ReadInt32();
                if (ids.Length != (cards + 1) * CardDim) return false;
                var keys = new List<string> { "" };
                var learned = new List<float[]> { new float[CardDim] };
                var statics = new List<float[]> { new float[StaticSize] };
                var aliases = new List<KeyValuePair<string, int>>();
                for (int c = 1; c <= cards; c++)
                {
                    string name = ReadPyString(r);
                    string prints = ReadPyString(r);
                    var s = new float[StaticSize];
                    for (int j = 0; j < fileStatics; j++) s[j] = Clean(r.ReadSingle());
                    var e = new float[CardDim];
                    Array.Copy(ids, c * CardDim, e, 0, CardDim);
                    var printList = prints.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    keys.Add(printList.Length > 0 ? printList[0] : "name:" + name);
                    learned.Add(e);
                    statics.Add(s);
                    for (int k = 1; k < printList.Length; k++) aliases.Add(new KeyValuePair<string, int>(printList[k], c));
                    aliases.Add(new KeyValuePair<string, int>("name:" + name, c));
                }
                string config = ReadPyString(r);
                long games = 0;
                int gi = config.IndexOf("\"games\":", StringComparison.Ordinal);
                if (gi >= 0)
                {
                    int start = gi + 8, end = start;
                    while (end < config.Length && (char.IsDigit(config[end]) || config[end] == ' ')) end++;
                    long.TryParse(config.Substring(start, end - start).Trim(), out games);
                }

                rw.EnterWriteLock();
                try
                {
                    TakeWeights(data, nb);
                    SetCards(keys, learned, statics, aliases);
                    steps = 0;
                    adamT = 0;
                    GamesTrained = games;
                    AverageLoss = 0;
                    Origin = "Python bot, " + games.ToString("N0", System.Globalization.CultureInfo.InvariantCulture) + " games";
                }
                finally
                {
                    weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                    rw.ExitWriteLock();
                }
                return true;
            }
        }

        private void SetCards(List<string> keys, List<float[]> learned, List<float[]> statics, List<KeyValuePair<string, int>> aliases)
        {
            lock (cardLock)
            {
                cardKeys.Clear();
                cardRows.Clear();
                cardStatic.Clear();
                cardStale.Clear();
                staleRows = 0;
                cardM.Clear();
                cardV.Clear();
                rowOfKey.Clear();
                for (int i = 0; i < keys.Count; i++)
                {
                    cardKeys.Add(keys[i]);
                    cardRows.Add(learned[i]);
                    cardStatic.Add(statics[i]);
                    // Saved before the per-attack stats (or never refreshed since): filled in from the card when it's next met.
                    bool stale = i > 0 && Blank(statics[i], OldStaticSize);
                    cardStale.Add(stale);
                    if (stale) staleRows++;
                    cardM.Add(null);
                    cardV.Add(null);
                    if (i > 0 && !rowOfKey.ContainsKey(keys[i])) rowOfKey[keys[i]] = i;
                }
                foreach (var kv in aliases)
                    if (kv.Value > 0 && kv.Value < keys.Count && !rowOfKey.ContainsKey(kv.Key)) rowOfKey[kv.Key] = kv.Value;
                rowIndex = new Dictionary<string, int>(rowOfKey, StringComparer.Ordinal);
                cardCache = new float[0][];
            }
        }

        private static string ReadPyString(BinaryReader r)
        {
            int n = r.ReadInt32();
            return Encoding.UTF8.GetString(r.ReadBytes(n));
        }

        /// <summary>A weight read from a file; a NaN or infinity means the file is damaged and isn't loaded.</summary>
        private static float Clean(float v)
        {
            if (float.IsNaN(v) || float.IsInfinity(v)) throw new InvalidDataException("the network holds a value that isn't a number");
            return v;
        }

        /// <summary>A copy of this network (for the opponent pool and "undo").</summary>
        public PolicyNet Clone()
        {
            var copy = new PolicyNet(Blocks);
            copy.FromBytes(ToBytes());
            return copy;
        }

        /// <summary>Back to fresh random weights.</summary>
        public void Reset()
        {
            rw.EnterWriteLock();
            try
            {
                InitRandom(1);
            }
            finally
            {
                weightsRevision++; // the GPU learner's copy is out of date now (GpuMoves)
                rw.ExitWriteLock();
            }
        }
    }
}
