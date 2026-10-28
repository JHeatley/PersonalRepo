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
        public int Seat { get { return Memory.Seat; } }

        /// <summary>When this returns true, the game is abandoned at the agent's next decision (training was stopped).</summary>
        public Func<bool> Abort;

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
            Memory.OnEvent(engine, ev);
        }

        public List<int> Decide(DecisionRequest r)
        {
            if (Abort != null && Abort()) throw new OperationCanceledException("training stopped");
            var picked = new List<int>();
            if (r == null || r.Options.Count == 0 || r.MaxChoices <= 0) return picked;
            if (Memory.Seat < 0) Memory.Seat = r.Player;
            Memory.See(r.View);
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
            var pass = Net.Forward(x);
            var p = PolicyNet.Softmax(pass.Logits);
            int a = 0;
            if (Greedy)
            {
                for (int i = 1; i < p.Length; i++) if (pass.Logits[i] > pass.Logits[a]) a = i;
            }
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
                var v = r.View;
                int lead = v != null && v.Me != null && v.Opponent != null ? v.Opponent.PrizeCount - v.Me.PrizeCount : 0;
                Trajectory.Add(new PolicyStep
                {
                    Input = x,
                    Action = a,
                    LogP = (float)Math.Log(Math.Max(1e-12, p[a])),
                    Value = pass.Value,
                    PrizeLead = lead,
                    Mistakes = mistakes
                });
            }
            return a;
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
