using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>How hard the Neural AI looks ahead (PolicySearch).</summary>
    public sealed class SearchSettings
    {
        /// <summary>Play-outs for one decision, shared among the moves it tries.</summary>
        public int Playouts = 48;
        /// <summary>The network's top moves it tries, at most.</summary>
        public int MaxCandidates = 4;
        /// <summary>It tries moves until their chances under the network add up to this (the rest are too unlikely to matter).</summary>
        public double PriorCover = 0.97;
        /// <summary>No look-ahead when the network is this sure of its top move.</summary>
        public double SkipAbove = 0.9;
        /// <summary>
        /// Whole turns each play-out looks ahead: 1 plays out the rest of this turn and the opponent's next turn, then asks the
        /// network how good the position is when its own next turn begins; 2 goes one round further.
        /// </summary>
        public int Turns = 1;
        /// <summary>Threads the play-outs share.</summary>
        public int Threads = 1;
        /// <summary>Time limit for one decision in seconds (0: none): no new play-outs start after it.</summary>
        public double MaxSeconds = 4;
        /// <summary>
        /// Another move has to beat the network's own top move by this much (on the value scale, -1 lost to 1 won) to replace
        /// it, so the play-outs' noise doesn't make it change its mind for nothing.
        /// </summary>
        public double Margin = 0.03;
        /// <summary>
        /// How strongly what the play-outs found reshapes the network's choice in the target it learns from (training from the
        /// look-ahead): the target is the network's probabilities times e^(TargetScale x the move's play-out value).
        /// </summary>
        public double TargetScale = 10;
        /// <summary>
        /// v172: plans the rest of the turn too. After the move being tried, the turn's next main actions are chosen by a search
        /// tree that all the play-outs of that move share (each play-out tries the most promising plan so far, or a new one), not
        /// only by the network's single top move. So it finds whole turns (this Ball, then that evolution, then attach, then
        /// attack) the network alone wouldn't play.
        /// </summary>
        public bool PlanTurn;
        /// <summary>Main actions after the first that the plan of the turn decides; past that the network plays.</summary>
        public int PlanDepth = 8;
        /// <summary>
        /// How much the plan tries moves the network likes but it hasn't tried much, against the plans that did best so far (the
        /// c of the PUCT rule, on the value scale).
        /// </summary>
        public double Explore = 1.0;
        /// <summary>
        /// v172: how firmly it guesses the opponent's deck. Each guess draws a deck from the known decks, the ones most like the
        /// cards seen far more often: a deck's weight is e^(DeckBelief x how well it matches). 0: always the closest one (the
        /// default: 1.5 measured no better, 46% in 200 games against 0, both looking ahead quick).
        /// </summary>
        public double DeckBelief = 0;
        /// <summary>
        /// v172: training looks at moves picked with random (Gumbel) noise on the network's chances, not always its top few, so
        /// it also learns about moves the network underrates. For training searches only; off until a training run shows it helps.
        /// </summary>
        public bool Gumbel;

        public static SearchSettings Quick(int threads)
        {
            return new SearchSettings { Playouts = 32, MaxCandidates = 3, Turns = 1, Threads = Math.Max(1, threads), MaxSeconds = 2, PlanTurn = PlanByDefault };
        }

        public static SearchSettings Deep(int threads)
        {
            return new SearchSettings { Playouts = 96, MaxCandidates = 5, Turns = 2, Threads = Math.Max(1, threads), MaxSeconds = 6, PlanTurn = PlanByDefault };
        }

        /// <summary>A light look-ahead for training games (learning from the look-ahead): many decisions, a little each.</summary>
        public static SearchSettings Training()
        {
            return new SearchSettings { Playouts = 16, MaxCandidates = 3, SkipAbove = 0.85, Turns = 1, Threads = 1, MaxSeconds = 1, PlanTurn = PlanByDefault };
        }

        /// <summary>Whether the presets plan the whole turn (v172; set from the head-to-head measurement against the earlier look-ahead).</summary>
        public static bool PlanByDefault = false;
    }

    /// <summary>
    /// Look-ahead for the Neural AI (step 2 towards an expert player). At a main action the network isn't sure about, it takes
    /// its few most likely moves and plays each one out many times, a turn or two ahead, with the network playing both sides;
    /// then it asks the network how good the position is and keeps the move that did best.
    ///
    /// It doesn't cheat: every play-out starts from a guess at what it can't see. The opponent's hand, deck and face-down Prize
    /// cards are rebuilt from the cards the opponent has shown (in play, discarded, revealed) and the known deck most like
    /// them (KnownDecks), never from the opponent's real hidden cards; its own deck and face-down Prize cards are shuffled.
    /// Each move is tried on the same guesses and the same coin flips, so the comparison is fair, and the moves that do badly
    /// stop being tried early (sequential halving), so most of the play-outs go to the close calls.
    /// </summary>
    public sealed class PolicySearch
    {
        public readonly PolicyNet Net;
        public readonly SearchSettings Settings;
        /// <summary>Decklists the opponent might be playing (the sim's decks): the guess at their hidden cards comes from these.</summary>
        public List<List<CardDef>> KnownDecks = new List<List<CardDef>>();
        /// <summary>Prize-card reward of the network's training (the value it learned includes it), see PolicyTrainer.PrizeReward.</summary>
        public double PrizeReward = 0.05;
        /// <summary>The play-outs choose their moves on the GPU (GpuMoves) when set.</summary>
        public GpuMoves Moves;

        /// <summary>Counts: decisions looked at, moves changed from the network's own, play-outs run and failed.</summary>
        public int Searches, Changed, Playouts, Failed;
        /// <summary>What the last look-ahead found, for tests and the log ("" when it didn't look).</summary>
        public string LastReport = "";

        private readonly object counts = new object();

        public PolicySearch(PolicyNet net, SearchSettings settings)
        {
            Net = net;
            Settings = settings ?? new SearchSettings();
        }

        private sealed class Candidate
        {
            public PolicyEncoder.Choice Choice;
            /// <summary>Its index in the choices.</summary>
            public int Index;
            public DecisionOption Option;
            public double Prior;
            public double Sum;
            public int N;
            public double Mean { get { return N > 0 ? Sum / N : double.NegativeInfinity; } }
            /// <summary>The plan of the rest of the turn after this move (Settings.PlanTurn), shared by its play-outs.</summary>
            public PlanNode Plan;
        }

        /// <summary>
        /// The move to make (an index into choices) at a main action: the network's top move, or a better one the play-outs
        /// found. engine is the game being played (only what the player could know is read from it).
        /// </summary>
        public int Pick(GameEngine engine, DecisionRequest r, PolicyMemory memory, List<PolicyEncoder.Choice> choices, double[] probs, int top)
        {
            double[] target;
            return Pick(engine, r, memory, choices, probs, top, out target);
        }

        /// <summary>
        /// Pick, and the improved choice the network learns from (training from the look-ahead): its probabilities over the
        /// choices, reweighted by what the play-outs found (null when it didn't look ahead).
        /// </summary>
        public int Pick(GameEngine engine, DecisionRequest r, PolicyMemory memory, List<PolicyEncoder.Choice> choices, double[] probs, int top,
            out double[] target)
        {
            target = null;
            LastReport = "";
            if (engine == null || engine.State == null || r == null || r.Kind != DecisionKind.MainAction || choices.Count < 2) return top;
            if (probs[top] >= Settings.SkipAbove || choices[top].Done) return top;
            int seat = r.Player;
            var order = new List<int>();
            for (int i = 0; i < choices.Count; i++) if (!choices[i].Done) order.Add(i);
            var rank = new double[choices.Count];
            for (int i = 0; i < rank.Length; i++) rank[i] = probs[i];
            if (Settings.Gumbel)
            {
                // Gumbel-top-k: the moves to look at are drawn from the network's chances (log p + Gumbel noise), so a move it
                // underrates still gets looked at sometimes. The noise comes from the position, so a position draws the same moves.
                var noise = new Rng(((int)(engine.State.Rng.Clone().NextULong() & 0x3FFFFFFF)) ^ 0x6A11BE);
                for (int i = 0; i < rank.Length; i++)
                {
                    double u = Math.Max(1e-12, (noise.NextULong() >> 11) * (1.0 / 9007199254740992.0));
                    rank[i] = Math.Log(Math.Max(1e-12, probs[i])) - Math.Log(-Math.Log(u));
                }
            }
            order.Sort((a, b) => rank[b].CompareTo(rank[a]));
            var cands = new List<Candidate>();
            double cover = 0;
            foreach (int i in order)
            {
                if (cands.Count >= Math.Max(1, Settings.MaxCandidates) || (cands.Count > 0 && cover >= Settings.PriorCover)) break;
                cands.Add(new Candidate
                {
                    Choice = choices[i], Index = i, Option = r.Options[choices[i].Option], Prior = probs[i],
                    Plan = Settings.PlanTurn ? new PlanNode() : null
                });
                cover += probs[i];
            }
            if (cands.Count < 2) return top;

            var root = engine.State.Clone();
            var myDeck = memory.Deck() ?? ToList(engine.StartingDeck(seat));
            var setup = new Setup
            {
                Root = root, Seat = seat, Memory = memory, MyDeck = myDeck, Scripts = engine.Scripts, PrizeCount = engine.PrizeCount,
                Rules = engine.Rules, MaxTurns = engine.MaxTurns, StopTurn = root.TurnNumber + 2 * Math.Max(1, Settings.Turns),
                RootLead = Lead(root, seat), Seen = SeenCards(root, seat, memory)
            };
            var clock = Stopwatch.StartNew();
            int baseSeed = (int)(root.Rng.Clone().NextULong() & 0x3FFFFFFF) ^ (root.TurnNumber * 7919);
            int world = 0;
            var alive = new List<Candidate>(cands);
            int rounds = (int)Math.Ceiling(Math.Log(alive.Count, 2));
            for (int round = 0; round < rounds && alive.Count > 1; round++)
            {
                int n = Math.Max(2, Settings.Playouts / (rounds * alive.Count));
                var scores = new double[alive.Count, n];
                int first = world;
                var opts = new ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Settings.Threads) };
                Parallel.For(0, alive.Count * n, opts, job =>
                {
                    int c = job / n, k = job % n;
                    scores[c, k] = double.NaN;
                    if (Settings.MaxSeconds > 0 && clock.Elapsed.TotalSeconds > Settings.MaxSeconds) return;
                    int seed = baseSeed + (first + k) * 104729;
                    scores[c, k] = Playout(setup, alive[c].Option, alive[c].Plan, seed);
                });
                world += n;
                bool any = false;
                // Only the guesses every move got through count, so each comparison is on the same guesses.
                for (int k = 0; k < n; k++)
                {
                    bool all = true;
                    for (int c = 0; c < alive.Count; c++) if (double.IsNaN(scores[c, k])) all = false;
                    lock (counts)
                    {
                        Playouts += alive.Count;
                        if (!all) Failed++;
                    }
                    if (!all) continue;
                    any = true;
                    for (int c = 0; c < alive.Count; c++)
                    {
                        alive[c].Sum += scores[c, k];
                        alive[c].N++;
                    }
                }
                if (!any) break; // out of time (or every play-out failed): keep what it has
                alive.Sort((a, b) => b.Mean.CompareTo(a.Mean));
                if (round < rounds - 1) alive.RemoveRange((alive.Count + 1) / 2, alive.Count - (alive.Count + 1) / 2);
            }

            lock (counts) Searches++;
            // The best of the moves still in the running; the network's own top move stays unless another clearly beats it.
            var topCand = cands.Find(c => c.Index == top) ?? cands[0];
            var best = alive[0].N > 0 ? alive[0] : topCand;
            if (best != topCand && alive.Contains(topCand) && best.Mean - topCand.Mean < Settings.Margin) best = topCand;
            LastReport = Report(cands, best) + (best.Plan != null && best.Plan.N > 1 ? " | its plan for the turn: " + best.Plan.Describe(6) : "");
            target = Target(cands, probs);
            if (best != topCand) lock (counts) Changed++;
            return choices.IndexOf(best.Choice);
        }

        /// <summary>
        /// The network's probabilities reweighted by the play-outs (Gumbel MuZero's "completed" values): a move played out gets its
        /// average result, one that wasn't the prior-weighted average of those that were; each gets e^(TargetScale x value).
        /// </summary>
        private double[] Target(List<Candidate> cands, double[] probs)
        {
            double weight = 0, mixed = 0;
            foreach (var c in cands)
            {
                if (c.N == 0) continue;
                weight += c.Prior;
                mixed += c.Prior * c.Mean;
            }
            if (weight <= 0) return null;
            mixed /= weight;
            var value = new double[probs.Length];
            for (int i = 0; i < value.Length; i++) value[i] = mixed;
            foreach (var c in cands) if (c.N > 0) value[c.Index] = c.Mean;
            var logits = new double[probs.Length];
            double max = double.NegativeInfinity;
            for (int i = 0; i < logits.Length; i++)
            {
                logits[i] = Math.Log(Math.Max(1e-9, probs[i])) + Settings.TargetScale * value[i];
                if (logits[i] > max) max = logits[i];
            }
            double sum = 0;
            for (int i = 0; i < logits.Length; i++)
            {
                logits[i] = Math.Exp(logits[i] - max);
                sum += logits[i];
            }
            for (int i = 0; i < logits.Length; i++) logits[i] /= sum;
            return logits;
        }

        private static string Report(List<Candidate> cands, Candidate best)
        {
            var parts = new List<string>();
            foreach (var c in cands)
                parts.Add((c == best ? "* " : "") + c.Option.Label + " (network " + Math.Round(c.Prior * 100) + "%, "
                          + (c.N > 0 ? "played out " + c.N + "x, " + c.Mean.ToString("0.00") : "not played out") + ")");
            return string.Join("; ", parts);
        }

        private static List<CardDef> ToList(IList<CardDef> cards)
        {
            return cards == null ? null : new List<CardDef>(cards);
        }

        /// <summary>Prize cards the opponent has left minus the player's (as the training's reward counts them).</summary>
        private static int Lead(GameState s, int seat)
        {
            return s.Players[1 - seat].Prizes.Count - s.Players[seat].Prizes.Count;
        }

        // ------------------------------------------------------------------ Play-outs

        private sealed class Setup
        {
            public GameState Root;
            public int Seat;
            public PolicyMemory Memory;
            public List<CardDef> MyDeck;
            public CardScriptRegistry Scripts;
            public int PrizeCount, MaxTurns, StopTurn, RootLead;
            public GameRules Rules;
            /// <summary>The opponent's cards the player has seen (by card id) and where they are now if that's public.</summary>
            public Dictionary<int, CardDef> Seen;
        }

        /// <summary>The look-ahead reached its end: the network's value of the position there.</summary>
        private sealed class CutOff : Exception
        {
            public readonly double Value;

            public CutOff(double value) : base("look-ahead end")
            {
                Value = value;
            }
        }

        /// <summary>
        /// The player's side in a play-out: the first main action is the move being tried; with a plan (Settings.PlanTurn) the
        /// turn's next main actions follow the plan tree (PlanNode.Select), until it reaches a part of the tree no play-out has
        /// been to; then the network plays, until StopTurn.
        /// </summary>
        private sealed class SearchSeat : IPlayerAgent, IGameObserver
        {
            private readonly PolicyAgent inner;
            private readonly DecisionOption first;
            private readonly int stopTurn;
            private bool firstDone;
            public bool Made;
            public GameEngine Engine;
            /// <summary>Where this play-out is in the plan tree (null: not planning, or past the part the tree knows).</summary>
            private PlanNode node;
            private readonly PolicySearch search;
            private int turn = -1, depth;
            /// <summary>The plan's steps this play-out took, for PlanNode.Backup.</summary>
            public readonly List<PlanNode.Step> Path = new List<PlanNode.Step>();

            public SearchSeat(PolicyAgent inner, DecisionOption first, int stopTurn, PolicySearch search = null, PlanNode plan = null)
            {
                this.inner = inner;
                this.first = first;
                this.stopTurn = stopTurn;
                this.search = search;
                node = plan;
            }

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                Engine = engine;
                inner.OnEvent(engine, ev);
            }

            public List<int> Decide(DecisionRequest r)
            {
                if (!firstDone && r.Kind == DecisionKind.MainAction)
                {
                    firstDone = true;
                    int i = ForcedPlayAgent.Match(r.Options, first);
                    if (i < 0) throw new InvalidOperationException("the move isn't on offer in this guess");
                    Made = true;
                    inner.Memory.See(r.View);
                    inner.Memory.NoteMainAction(r.Options[i]);
                    if (r.View != null) turn = r.View.TurnNumber;
                    return new List<int> { i };
                }
                if (firstDone && r.Kind == DecisionKind.MainAction && r.View != null && r.View.TurnNumber >= stopTurn)
                    throw new CutOff(inner.ValueOf(r));
                if (node != null && r.Kind == DecisionKind.MainAction && r.View != null)
                {
                    if (r.View.TurnNumber != turn || depth >= search.Settings.PlanDepth) node = null; // the turn is over: no more plan
                    else
                    {
                        inner.Memory.See(r.View);
                        var choices = PolicyEncoder.Choices(r, new List<int>());
                        if (choices.Count >= 2)
                        {
                            var probs = inner.Probabilities(r, choices);
                            int pick = PlanNode.Select(ref node, r, choices, probs, search.Settings.Explore, Path);
                            depth++;
                            var c = choices[pick];
                            if (c.Done) return new List<int>();
                            inner.Memory.NoteMainAction(r.Options[c.Option]);
                            return new List<int> { c.Option };
                        }
                    }
                }
                return inner.Decide(r);
            }
        }

        // ------------------------------------------------------------------ The plan of the turn

        /// <summary>
        /// A point in the plan of the turn (Settings.PlanTurn): the main actions tried from here, with how often each was tried
        /// and how the play-outs that tried it ended. The guesses at the hidden cards differ between play-outs, so a move is
        /// counted only in the play-outs it was on offer in (information-set Monte Carlo tree search); moves are told apart by
        /// what they do (PlanKey), not by card ids, which differ between guesses for cards drawn during the turn.
        /// </summary>
        public sealed class PlanNode
        {
            public sealed class Edge
            {
                public double Prior;
                public int PriorCount, N, Available;
                public double Sum;
                public PlanNode Child;
            }

            /// <summary>One step a play-out took: the point and the move.</summary>
            public struct Step
            {
                public PlanNode Node;
                public Edge Edge;
            }

            private readonly Dictionary<string, Edge> edges = new Dictionary<string, Edge>(StringComparer.Ordinal);
            public int N;
            public double Sum;
            /// <summary>The tree is shared by the play-outs running at once; one lock for all of it (the work is in the play-outs).</summary>
            private static readonly object treeLock = new object();
            /// <summary>Taken from a move while a play-out is on its way through it, so others running at once try something else.</summary>
            private const double VirtualLoss = 1.0;

            /// <summary>
            /// The move to make at this point of the plan (an index into choices), and node moved on to the point after it. A
            /// point no play-out has been to yet is where the plan ends: it takes the network's top move and node becomes null
            /// (the network plays from there). Otherwise the PUCT rule: the move's average result so far, plus explore x its
            /// prior x sqrt(tries here) / (1 + its tries).
            /// </summary>
            public static int Select(ref PlanNode node, DecisionRequest r, List<PolicyEncoder.Choice> choices, double[] probs, double explore,
                List<Step> path)
            {
                lock (treeLock)
                {
                    var here = node;
                    int top = 0;
                    for (int i = 1; i < probs.Length; i++) if (probs[i] > probs[top]) top = i;
                    var offered = new Edge[choices.Count];
                    for (int i = 0; i < choices.Count; i++)
                    {
                        string key = PlanKey(r, choices[i]);
                        Edge e;
                        if (!here.edges.TryGetValue(key, out e)) here.edges[key] = e = new Edge();
                        // The network's chances for it, averaged over the guesses it was offered in.
                        e.Prior += (probs[i] - e.Prior) / (++e.PriorCount);
                        e.Available++;
                        offered[i] = e;
                    }
                    if (here.N == 0)
                    {
                        // The end of the known plan: this play-out finds out how good the network's way on from here is.
                        path.Add(new Step { Node = here, Edge = null });
                        node = null;
                        return top;
                    }
                    double parent = here.Sum / here.N;
                    int best = top;
                    double bestScore = double.NegativeInfinity;
                    for (int i = 0; i < choices.Count; i++)
                    {
                        var e = offered[i];
                        // Not tried yet: as good as this point's average so far, a little less (first-play urgency).
                        double q = e.N > 0 ? e.Sum / e.N : parent - 0.05;
                        double score = q + explore * e.Prior * Math.Sqrt(Math.Max(1, e.Available)) / (1 + e.N);
                        if (score > bestScore)
                        {
                            bestScore = score;
                            best = i;
                        }
                    }
                    var chosen = offered[best];
                    chosen.N++;
                    chosen.Sum -= VirtualLoss;
                    if (chosen.Child == null) chosen.Child = new PlanNode();
                    path.Add(new Step { Node = here, Edge = chosen });
                    node = chosen.Child;
                    return best;
                }
            }

            /// <summary>Counts a finished play-out's result (value) along the plan it followed; NaN: it failed (only undoes the virtual loss).</summary>
            public static void Backup(PlanNode root, List<Step> path, double value)
            {
                lock (treeLock)
                {
                    bool ok = !double.IsNaN(value);
                    foreach (var step in path)
                    {
                        if (step.Edge != null)
                        {
                            step.Edge.Sum += VirtualLoss;
                            if (ok) step.Edge.Sum += value;
                            else step.Edge.N--;
                        }
                    }
                    if (!ok) return;
                    // Every point the play-out went through, the plan's start included (the move tried was made before it).
                    root.N++;
                    root.Sum += value;
                    foreach (var step in path)
                    {
                        if (step.Node == root) continue;
                        step.Node.N++;
                        step.Node.Sum += value;
                    }
                }
            }

            /// <summary>The best plan found from here: its moves (by label) and their average results, for the report.</summary>
            public string Describe(int maxSteps)
            {
                lock (treeLock)
                {
                    var parts = new List<string>();
                    var n = this;
                    while (n != null && parts.Count < maxSteps)
                    {
                        Edge best = null;
                        string label = null;
                        foreach (var kv in n.edges)
                            if (kv.Value.N > 0 && (best == null || kv.Value.N > best.N))
                            {
                                best = kv.Value;
                                label = kv.Key;
                            }
                        if (best == null) break;
                        int bar = label.LastIndexOf('|');
                        parts.Add((bar >= 0 ? label.Substring(bar + 1) : label) + " (" + best.N + "x, " + (best.Sum / best.N).ToString("0.00") + ")");
                        n = best.Child;
                    }
                    return string.Join(" → ", parts);
                }
            }

            /// <summary>What a move does, the same in every guess: its kind, card name, target and which attack or Ability.</summary>
            public static string PlanKey(DecisionRequest r, PolicyEncoder.Choice c)
            {
                if (c.Done) return "done|end the turn";
                var o = r.Options[c.Option];
                return (int)o.Action + "|" + (o.Card != null ? o.Card.Name : "") + "|" + o.PokemonUid + "|" + o.Index + "|" + o.Label;
            }
        }

        /// <summary>One play-out of a move on one guess (the seed picks the guess and the coin flips). NaN when it failed.</summary>
        private double Playout(Setup setup, DecisionOption move, PlanNode plan, int seed)
        {
            SearchSeat me = null;
            double value = double.NaN;
            try
            {
                value = PlayoutWith(setup, move, plan, seed, out me);
                return value;
            }
            finally
            {
                if (plan != null && me != null) PlanNode.Backup(plan, me.Path, value);
            }
        }

        private double PlayoutWith(Setup setup, DecisionOption move, PlanNode plan, int seed, out SearchSeat me)
        {
            me = null;
            int seat = setup.Seat;
            List<CardDef> theirDeck;
            var s = Guess(setup, seed, out theirDeck);
            s.Rng = new Rng(seed ^ 0x51ED);
            var mine = new PolicyAgent(Net, seed * 2 + 1, seat, setup.MyDeck) { Greedy = true, Moves = Moves };
            mine.Memory.CopyFrom(setup.Memory);
            var theirs = new PolicyAgent(Net, seed * 2 + 2, 1 - seat, theirDeck) { Greedy = true, Moves = Moves };
            theirs.Memory.Turn = setup.Root.TurnNumber;
            me = new SearchSeat(mine, move, setup.StopTurn, this, plan);
            var engine = new GameEngine(setup.Scripts, seat == 0 ? (IPlayerAgent)me : theirs, seat == 0 ? (IPlayerAgent)theirs : me);
            engine.CheckInvariants = false;
            engine.CardsPerPlayer = 0;
            engine.PrizeCount = setup.PrizeCount;
            engine.Rules = setup.Rules;
            engine.MaxTurns = setup.MaxTurns;
            try
            {
                engine.LoadState(s);
                var result = engine.ResumeGame();
                if (!me.Made) return double.NaN;
                double outcome = result.Winner == seat ? 1 : result.Winner >= 0 ? -1 : 0;
                return outcome + PrizeReward * (Lead(engine.State, seat) - setup.RootLead);
            }
            catch (CutOff cut)
            {
                return cut.Value + PrizeReward * (Lead(engine.State, seat) - setup.RootLead);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return double.NaN;
            }
        }

        // ------------------------------------------------------------------ Guessing the hidden cards

        /// <summary>
        /// The opponent's cards the player has seen this game: the memory's (revealed, played, discarded...) and everything of
        /// theirs that is face up now. By card id, so two copies count twice.
        /// </summary>
        private static Dictionary<int, CardDef> SeenCards(GameState s, int seat, PolicyMemory memory)
        {
            var seen = new Dictionary<int, CardDef>();
            if (memory != null) foreach (var kv in memory.OppSeen) seen[kv.Key] = kv.Value;
            foreach (var c in PublicCards(s, 1 - seat)) seen[c.Id] = c.Def;
            return seen;
        }

        /// <summary>A player's cards that are face up now: in play, discarded, in the Lost Zone, face-up Prize cards, their Stadium.</summary>
        private static List<CardInstance> PublicCards(GameState s, int player)
        {
            var p = s.Players[player];
            var list = new List<CardInstance>();
            list.AddRange(p.Discard);
            list.AddRange(p.LostZone);
            foreach (var mon in p.InPlay())
                foreach (var c in mon.AllCards())
                    if (!s.FaceDownCards.ContainsKey(c.Id)) list.Add(c);
            foreach (var c in p.Prizes) if (p.FaceUpPrizeIds.Contains(c.Id)) list.Add(c);
            if (s.StadiumOwner == player)
            {
                if (s.Stadium != null) list.Add(s.Stadium);
                if (s.StadiumPartner != null) list.Add(s.StadiumPartner);
            }
            list.RemoveAll(c => c == null || c.Def == null);
            return list;
        }

        /// <summary>
        /// A copy of the position with the hidden cards guessed: the player's own deck and face-down Prize cards shuffled, and the
        /// opponent's hand, deck and face-down Prize cards (and face-down Benched Pokémon) made up from the guess at their deck.
        /// Only counts are read from the opponent's hidden cards.
        /// </summary>
        public GameState Guess(GameState root, int seat, PolicyMemory memory, int seed, out List<CardDef> theirDeck)
        {
            var setup = new Setup { Root = root, Seat = seat, Memory = memory, Seen = SeenCards(root, seat, memory) };
            return Guess(setup, seed, out theirDeck);
        }

        private GameState Guess(Setup setup, int seed, out List<CardDef> theirDeck)
        {
            var s = setup.Root.Clone();
            var rng = new Rng(seed);
            int seat = setup.Seat;
            var me = s.Players[seat];
            var opp = s.Players[1 - seat];

            // Mine: the deck and face-down Prize cards are the right cards (my own list) in an unknown order.
            var pool = new List<CardInstance>(me.Deck);
            var mySlots = HiddenPrizeSlots(me);
            foreach (int i in mySlots) pool.Add(me.Prizes[i]);
            rng.Shuffle(pool);
            int k = 0;
            foreach (int i in mySlots) me.Prizes[i] = pool[k++];
            me.Deck = pool.GetRange(k, pool.Count - k);

            // Theirs: a deck like the cards they've shown, less what's face up now and what I know is in their hand.
            theirDeck = GuessDeck(setup.Seen, rng);
            var known = new List<CardDef>();
            if (setup.Memory != null)
                foreach (var def in setup.Memory.OppKnownHand.Values)
                    if (known.Count < opp.Hand.Count) known.Add(def);
            var counts = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var c in PublicCards(s, 1 - seat)) Add(counts, c.Def.Name, 1);
            foreach (var def in known) Add(counts, def.Name, 1);
            var rest = new List<CardDef>();
            var source = theirDeck ?? new List<CardDef>(setup.Seen.Values);
            foreach (var def in source)
            {
                int n;
                if (counts.TryGetValue(def.Name, out n) && n > 0) counts[def.Name] = n - 1;
                else rest.Add(def);
            }
            var oppSlots = HiddenPrizeSlots(opp);
            int needed = (opp.Hand.Count - known.Count) + opp.Deck.Count + oppSlots.Count;
            rng.Shuffle(rest);
            if (rest.Count > needed) rest.RemoveRange(needed, rest.Count - needed);
            var filler = source.Count > 0 ? source : setup.MyDeck ?? new List<CardDef>();
            while (rest.Count < needed && filler.Count > 0) rest.Add(filler[rng.Next(filler.Count)]);
            rng.Shuffle(rest);

            int nextId = 1;
            foreach (var c in AllCards(s)) nextId = Math.Max(nextId, c.Id + 1);
            int owner = 1 - seat;
            int at = 0;
            var hand = new List<CardInstance>();
            foreach (var def in known) hand.Add(new CardInstance(nextId++, def, owner));
            while (hand.Count < opp.Hand.Count && at < rest.Count) hand.Add(new CardInstance(nextId++, rest[at++], owner));
            opp.Hand = hand;
            foreach (int i in oppSlots)
            {
                if (at >= rest.Count) break;
                var old = opp.Prizes[i];
                var made = new CardInstance(nextId++, rest[at++], owner);
                int slot;
                if (opp.PrizeSlotByCard.TryGetValue(old.Id, out slot))
                {
                    opp.PrizeSlotByCard.Remove(old.Id);
                    opp.PrizeSlotByCard[made.Id] = slot;
                }
                opp.Prizes[i] = made;
            }
            var deck = new List<CardInstance>();
            while (at < rest.Count) deck.Add(new CardInstance(nextId++, rest[at++], owner));
            opp.Deck = deck;

            // Their face-down Benched Pokémon (Lt. Surge's Secret Plan): a Basic Pokémon from the guess.
            var basics = source.FindAll(d => d.IsBasicPokemon);
            foreach (var id in new List<int>(s.FaceDownCards.Keys))
            {
                var real = s.FaceDownCards[id];
                if (real.Owner != owner) continue;
                if (basics.Count > 0) s.FaceDownCards[id] = new CardInstance(id, basics[rng.Next(basics.Count)], owner);
            }
            return s;
        }

        /// <summary>
        /// A known deck for the guess, or null when there are none to pick from. v172: drawn at random, a deck's chance growing
        /// with how well it matches the cards seen (e^(DeckBelief x its score), so a much better match is nearly certain but
        /// near-ties share the guesses); DeckBelief 0 takes the closest one (ties at random), as before.
        /// </summary>
        private List<CardDef> GuessDeck(Dictionary<int, CardDef> seen, Rng rng)
        {
            if (KnownDecks == null || KnownDecks.Count == 0) return null;
            var shown = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var def in seen.Values) Add(shown, def.Name, 1);
            var best = new List<List<CardDef>>();
            int bestScore = int.MinValue;
            var all = new List<KeyValuePair<List<CardDef>, int>>();
            foreach (var deck in KnownDecks)
            {
                if (deck == null || deck.Count == 0) continue;
                var have = new Dictionary<string, int>(StringComparer.Ordinal);
                foreach (var def in deck) Add(have, def.Name, 1);
                int score = 0;
                foreach (var kv in shown)
                {
                    int n;
                    have.TryGetValue(kv.Key, out n);
                    // A card it holds counts for it; a card it can't hold (or more copies than it has) counts twice against it.
                    score += Math.Min(n, kv.Value) - 2 * Math.Max(0, kv.Value - n);
                }
                all.Add(new KeyValuePair<List<CardDef>, int>(deck, score));
                if (score > bestScore)
                {
                    bestScore = score;
                    best.Clear();
                }
                if (score == bestScore) best.Add(deck);
            }
            if (best.Count == 0) return null;
            double beta = Settings.DeckBelief;
            if (beta <= 0) return best[rng.Next(best.Count)];
            double total = 0;
            foreach (var kv in all) total += Math.Exp(beta * (kv.Value - bestScore));
            double u = (rng.NextULong() >> 11) * (1.0 / 9007199254740992.0) * total;
            foreach (var kv in all)
            {
                u -= Math.Exp(beta * (kv.Value - bestScore));
                if (u <= 0) return kv.Key;
            }
            return best[0];
        }

        private static void Add(Dictionary<string, int> counts, string key, int n)
        {
            int had;
            counts.TryGetValue(key, out had);
            counts[key] = had + n;
        }

        private static List<int> HiddenPrizeSlots(PlayerState p)
        {
            var slots = new List<int>();
            for (int i = 0; i < p.Prizes.Count; i++)
                if (!p.FaceUpPrizeIds.Contains(p.Prizes[i].Id)) slots.Add(i);
            return slots;
        }

        private static IEnumerable<CardInstance> AllCards(GameState s)
        {
            foreach (var p in s.Players)
            {
                foreach (var c in p.Deck) yield return c;
                foreach (var c in p.Hand) yield return c;
                foreach (var c in p.Discard) yield return c;
                foreach (var c in p.Prizes) yield return c;
                foreach (var c in p.LostZone) yield return c;
                foreach (var c in p.SetAsideHand) yield return c;
                foreach (var mon in p.InPlay())
                    foreach (var c in mon.AllCards()) yield return c;
            }
            if (s.Stadium != null) yield return s.Stadium;
            if (s.StadiumPartner != null) yield return s.StadiumPartner;
        }
    }
}
