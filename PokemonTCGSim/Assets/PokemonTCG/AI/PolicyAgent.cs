using System;
using System.Collections.Generic;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One decision the policy network made, kept for training (PPO needs what it saw, did and expected).</summary>
    public sealed class PolicyStep
    {
        public PolicyInput Input;
        public int Action;
        public float LogP;
        public float Value;
        /// <summary>Prize cards the opponent has left minus yours, at the time (the prize-swing reward compares these).</summary>
        public int PrizeLead;
        /// <summary>Sequencing mistakes this choice made (see PolicyAgent.SequencingMistakes).</summary>
        public int Mistakes;
        /// <summary>Sampled a move other than its top-rated one (it tried something new, for the Train screen).</summary>
        public bool Explored;
        /// <summary>
        /// Training from the look-ahead: the improved choice the play-outs found (over the same options), which the network learns
        /// to match; null for an ordinary decision (learned by PPO).
        /// </summary>
        public float[] Target;
    }

    /// <summary>
    /// Plays a seat with the policy network alone (the Neural AI). Every choice comes from the network: it samples from the
    /// policy while training and takes the top-rated option otherwise. Picks of several cards are made one card at a time.
    /// It watches the game's public events to remember what a real player would (see PolicyMemory).
    /// </summary>
    public sealed class PolicyAgent : IPlayerAgent, IGameObserver
    {
        private static readonly HashSet<string> DrawAbilities = new HashSet<string>(StringComparer.Ordinal)
        {
            "Recon Directive", "Run Errand", "Flip the Script", "Teal Dance", "Trade", "Lunar Cycle", "Boom Boom Groove",
            "Attract Customers", "Run Away Draw", "Last-Ditch Catch"
        };
        private static readonly HashSet<string> SequencingShufflers = new HashSet<string>(StringComparer.Ordinal)
        {
            "Lillie's Determination", "Judge", "Unfair Stamp", "Team Rocket's Archer"
        };

        public readonly PolicyNet Net;
        public readonly PolicyMemory Memory;
        /// <summary>Top-rated option every time (playing for real); otherwise sample from the policy (training).</summary>
        public bool Greedy = true;
        public bool Record;
        public readonly List<PolicyStep> Trajectory = new List<PolicyStep>();
        /// <summary>While recording (training): the cards it had in hand at some point, and the cards it played (for deck building).</summary>
        public readonly HashSet<string> Drawn = new HashSet<string>(StringComparer.Ordinal);
        public readonly HashSet<string> Played = new HashSet<string>(StringComparer.Ordinal);
        public int Seat { get { return Memory.Seat; } }

        /// <summary>When this returns true, the game is abandoned at the agent's next decision (training was stopped).</summary>
        public Func<bool> Abort;
        /// <summary>Look-ahead at main actions (playing for real, not while training or recording); null plays on the network alone.</summary>
        public PolicySearch Search;
        /// <summary>
        /// Training from the look-ahead: while training (Record), this share of its main actions the network isn't sure of are
        /// played by the look-ahead (Search), and the decision keeps the look-ahead's improved choice as its target.
        /// </summary>
        public double LearnFromSearch;
        /// <summary>Decisions the look-ahead made while training (they carry a target).</summary>
        public int Searched;
        /// <summary>Choosing moves on the GPU (many games at once): the network runs there instead of here; null: here.</summary>
        public GpuMoves Moves;

        private float[] Evaluate(PolicyInput x, out float value)
        {
            var moves = Moves;
            return moves != null && moves.Net == Net ? moves.Evaluate(x, out value) : Net.Evaluate(x, out value);
        }
        private GameEngine engine;

        /// <summary>Counts for the sequencing report.</summary>
        public int Attacks, AttacksWithResourcesLeft, Shuffles, ShufflesWithPlaysLeft, DrewThenShuffled;
        private readonly Rng rng;
        private int drewTurn = -1;

        public PolicyAgent(PolicyNet net, int seed, int seat = -1, List<CardDef> deckList = null)
        {
            Net = net;
            rng = new Rng(seed ^ 0x5EED17);
            Memory = new PolicyMemory(seat) { DeckList = deckList };
        }

        public void OnEvent(GameEngine engine, GameEvent ev)
        {
            if (engine != null) this.engine = engine;
            Memory.OnEvent(engine, ev);
        }

        /// <summary>The network's value of the position at this decision (how likely it thinks it is to win, -1 to 1).</summary>
        public float ValueOf(DecisionRequest r)
        {
            if (Memory.Seat < 0) Memory.Seat = r.Player;
            Memory.See(r.View);
            var choices = PolicyEncoder.Choices(r, new List<int>());
            if (choices.Count == 0) return 0;
            float value;
            Evaluate(PolicyEncoder.Encode(r, Memory, Net, choices), out value);
            return value;
        }

        /// <summary>
        /// The network's probability for each of these choices at this decision (the look-ahead's plan of the turn uses them as
        /// its priors). Call Memory.See(r.View) first.
        /// </summary>
        public double[] Probabilities(DecisionRequest r, List<PolicyEncoder.Choice> choices)
        {
            if (Memory.Seat < 0) Memory.Seat = r.Player;
            float value;
            return PolicyNet.Softmax(Evaluate(PolicyEncoder.Encode(r, Memory, Net, choices), out value));
        }

        public List<int> Decide(DecisionRequest r)
        {
            if (Abort != null && Abort()) throw new OperationCanceledException("training stopped");
            var picked = new List<int>();
            if (r == null || r.Options.Count == 0 || r.MaxChoices <= 0) return picked;
            if (Memory.Seat < 0) Memory.Seat = r.Player;
            Memory.See(r.View);
            if (Record && r.View != null && r.View.Me != null)
                foreach (var c in r.View.Me.Hand)
                    if (c != null && c.Def != null) Drawn.Add(c.Def.Name);
            bool single = !(r.MaxChoices > 1 || r.MinChoices == 0);
            while (picked.Count < r.MaxChoices)
            {
                var choices = PolicyEncoder.Choices(r, picked);
                if (choices.Count == 0) break;
                int pick;
                if (choices.Count == 1) pick = 0; // nothing to decide
                else pick = Choose(r, choices);
                var c = choices[pick];
                if (c.Done) break;
                picked.Add(c.Option);
                if (single) break;
            }
            while (picked.Count < r.MinChoices)
            {
                // Shouldn't happen (done is only offered once enough are picked); keep the engine's rules anyway.
                for (int i = 0; i < r.Options.Count && picked.Count < r.MinChoices; i++) if (!picked.Contains(i)) picked.Add(i);
            }
            if (r.Kind == DecisionKind.MainAction && picked.Count > 0) Memory.NoteMainAction(r.Options[picked[0]]);
            return picked;
        }

        private int Choose(DecisionRequest r, List<PolicyEncoder.Choice> choices)
        {
            var x = PolicyEncoder.Encode(r, Memory, Net, choices);
            float value;
            var logits = Evaluate(x, out value);
            var p = PolicyNet.Softmax(logits);
            int a = 0, top = 0;
            for (int i = 1; i < p.Length; i++) if (logits[i] > logits[top]) top = i;
            float[] target = null;
            if (Greedy && Search != null && !Record && r.Kind == DecisionKind.MainAction && engine != null)
                a = Search.Pick(engine, r, Memory, choices, p, top);
            else if (Record && LearnFromSearch > 0 && Search != null && r.Kind == DecisionKind.MainAction && engine != null
                     && (rng.NextULong() >> 11) * (1.0 / 9007199254740992.0) < LearnFromSearch
                     && (a = SearchedPick(r, choices, p, top, out target)) >= 0)
            {
                // the look-ahead's move, with its improved choice as the target (SearchedPick)
            }
            else if (Greedy) a = top;
            else
            {
                double u = (rng.NextULong() >> 11) * (1.0 / 9007199254740992.0), acc = 0;
                a = p.Length - 1;
                for (int i = 0; i < p.Length; i++)
                {
                    acc += p[i];
                    if (u < acc)
                    {
                        a = i;
                        break;
                    }
                }
            }
            int mistakes = r.Kind == DecisionKind.MainAction ? SequencingMistakes(r, choices, a) : 0;
            if (Record)
            {
                if (r.Kind == DecisionKind.MainAction && !choices[a].Done)
                {
                    var played = r.Options[choices[a].Option].Card;
                    if (played != null) Played.Add(played.Name);
                }
                var v = r.View;
                int lead = v != null && v.Me != null && v.Opponent != null ? v.Opponent.PrizeCount - v.Me.PrizeCount : 0;
                Trajectory.Add(new PolicyStep
                {
                    Input = x,
                    Action = a,
                    LogP = (float)Math.Log(Math.Max(1e-12, p[a])),
                    Value = value,
                    PrizeLead = lead,
                    Mistakes = mistakes,
                    Explored = target == null && a != top,
                    Target = target
                });
            }
            return a;
        }

        /// <summary>The look-ahead's move and improved choice for a training decision; -1 when it didn't look ahead (sure enough).</summary>
        private int SearchedPick(DecisionRequest r, List<PolicyEncoder.Choice> choices, double[] p, int top, out float[] target)
        {
            target = null;
            double[] improved;
            int pick = Search.Pick(engine, r, Memory, choices, p, top, out improved);
            if (improved == null) return -1;
            target = new float[improved.Length];
            for (int i = 0; i < improved.Length; i++) target[i] = (float)improved[i];
            Searched++;
            return pick;
        }

        /// <summary>
        /// The Python bot's common sequencing mistakes: shuffling your hand away (Lillie's Determination, Judge...) while
        /// Items, Basics to bench or an Energy attachment were still available; attacking (which ends the turn) with an Energy
        /// attachment, an Ability or a Basic to bench left; using a draw Ability and then shuffling the hand away that turn.
        /// </summary>
        private int SequencingMistakes(DecisionRequest r, List<PolicyEncoder.Choice> choices, int a)
        {
            if (choices[a].Done) return 0;
            var o = r.Options[choices[a].Option];
            int turn = r.View != null ? r.View.TurnNumber : 0;
            int mistakes = 0;
            if (o.Action == ActionType.PlayTrainer && o.Card != null && SequencingShufflers.Contains(o.Card.Name))
            {
                Shuffles++;
                bool playsLeft = false;
                foreach (var c in choices)
                {
                    if (c.Done || c.Option == choices[a].Option) continue;
                    var other = r.Options[c.Option];
                    if (other.Card != null && other.Card.BaseKey == o.Card.BaseKey) continue;
                    if (other.Action == ActionType.PlayTrainer || other.Action == ActionType.BenchBasic || other.Action == ActionType.AttachEnergy
                        || other.Action == ActionType.Evolve) playsLeft = true;
                }
                if (playsLeft)
                {
                    ShufflesWithPlaysLeft++;
                    mistakes++;
                }
                if (drewTurn == turn)
                {
                    DrewThenShuffled++;
                    mistakes++;
                }
            }
            else if (o.Action == ActionType.Attack)
            {
                Attacks++;
                foreach (var c in choices)
                {
                    if (c.Done || c.Option == choices[a].Option) continue;
                    var other = r.Options[c.Option];
                    if (other.Action == ActionType.AttachEnergy || other.Action == ActionType.UseAbility || other.Action == ActionType.BenchBasic)
                    {
                        AttacksWithResourcesLeft++;
                        mistakes++;
                        break;
                    }
                }
            }
            else if (o.Action == ActionType.UseAbility && o.Card != null && o.Index >= 0 && o.Index < o.Card.Abilities.Count
                     && DrawAbilities.Contains(o.Card.Abilities[o.Index].Name))
                drewTurn = turn;
            return mistakes;
        }
    }
}
