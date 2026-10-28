using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// The learning AI. It starts from the classic bot's scores (HeuristicAgent) and adds what its LearningBrain has
    /// learned about each option, so with an empty brain it plays exactly like the classic bot and every game of
    /// training or against you moves it from there.
    ///
    /// Main actions keep the classic bot's turn structure: learned values can reorder setup plays, skip one or
    /// pick one up that the classic bot skips, choose between attacks or decide not to attack, but an attack never
    /// jumps ahead of the setup plays it would otherwise make first.
    /// </summary>
    public sealed class LearningAgent : IPlayerAgent
    {
        private readonly HeuristicAgent heuristic;
        private readonly LearningBrain brain;
        private readonly Rng exploreRng;

        /// <summary>Name of the deck this agent plays and the one it plays against (for deck and matchup features).</summary>
        public string MyDeck;
        public string OppDeck;
        /// <summary>Training: sometimes try the second or third best option to find out whether it's better.</summary>
        public bool Training;
        /// <summary>Chance of trying something else when Training.</summary>
        public double ExploreRate = 0.08;
        /// <summary>Keep the decisions of this game so LearnFromGame can learn from them.</summary>
        public bool Record = true;
        public readonly List<LearnedDecision> Trajectory = new List<LearnedDecision>();
        /// <summary>Seat this agent played (from the requests it saw), or -1.</summary>
        public int Seat = -1;
        /// <summary>
        /// The style this agent plays (v78: the Train screen's own choice); null plays the brain's Style (the Opponent switch).
        /// Whatever the style, the games it records teach both the feature weights and the network.
        /// </summary>
        public AiStyle? Style;

        private int usesTurn = -1;
        private readonly Dictionary<string, int> usesThisTurn = new Dictionary<string, int>(StringComparer.Ordinal);

        public LearningAgent(LearningBrain brain, int seed, string myDeck, string oppDeck)
        {
            this.brain = brain;
            heuristic = new HeuristicAgent(seed);
            exploreRng = new Rng(seed ^ 0x2F6B3D1);
            MyDeck = myDeck;
            OppDeck = oppDeck;
        }

        public LearningBrain Brain { get { return brain; } }

        public List<int> Decide(DecisionRequest r)
        {
            Seat = r.Player;
            var c = heuristic.Score(r);
            if (brain == null || r.Options.Count < 2) return heuristic.Pick(r, c);

            var feats = new List<List<string>>(r.Options.Count);
            for (int i = 0; i < r.Options.Count; i++) feats.Add(Features.For(r, i, MyDeck, OppDeck));
            if (r.View != null && r.View.TurnNumber != usesTurn)
            {
                usesTurn = r.View.TurnNumber;
                usesThisTurn.Clear();
            }
            var repeated = RepeatedOptions(r);
            var state = NeuralNet.StateOf(r);
            var adjusted = Adjust(r, c, Style.HasValue ? brain.PlayValues(feats, state, Style.Value) : brain.PlayValues(feats, state), repeated);
            var greedy = heuristic.Pick(r, adjusted, c.Count, c.Jitter);
            var chosen = greedy;
            bool explored = false;
            if (Training && exploreRng.Next(10000) < (int)(ExploreRate * 10000))
            {
                var alt = Explore(r, c, adjusted, greedy, repeated);
                if (alt != null)
                {
                    chosen = alt;
                    explored = true;
                }
            }
            NoteUses(r, chosen);
            if (Record)
            {
                Trajectory.Add(new LearnedDecision
                {
                    Turn = r.View != null ? r.View.TurnNumber : 0,
                    OptionFeatures = feats,
                    Chosen = new List<int>(chosen),
                    Greedy = new List<int>(greedy),
                    Explored = explored,
                    MyPrizes = r.View != null ? r.View.Me.PrizeCount : 0,
                    OppPrizes = r.View != null ? r.View.Opponent.PrizeCount : 0,
                    State = state
                });
            }
            return chosen;
        }

        /// <summary>Learns from this game's decisions (call once the game is over), then forgets them.</summary>
        public void LearnFromGame(GameEngine engine)
        {
            if (brain == null || Seat < 0 || engine == null || engine.State == null) return;
            var s = engine.State;
            double outcome = s.Winner < 0 ? 0 : (s.Winner == Seat ? 1 : -1);
            brain.LearnFromGame(Trajectory, outcome, s.TurnNumber, s.Players[Seat].Prizes.Count, s.Players[1 - Seat].Prizes.Count, MyDeck, OppDeck);
            Trajectory.Clear();
        }

        // ------------------------------------------------------------------ Learned adjustments

        /// <summary>The classic scores plus the learned values, scaled to each kind of decision.</summary>
        public static double[] Adjust(DecisionRequest r, ScoredChoice c, double[] values, bool[] repeated)
        {
            var s = (double[])c.Scores.Clone();
            if (r.Kind == DecisionKind.MainAction)
            {
                for (int i = 0; i < s.Length; i++)
                    s[i] += MainBonus(r.Options[i], c.Scores[i], values[i], repeated != null && repeated[i]);
                return s;
            }
            double min = double.MaxValue, max = double.MinValue;
            foreach (var x in c.Scores)
            {
                if (x < min) min = x;
                if (x > max) max = x;
            }
            double scale = Clamp(max - min, 1, 40) * 0.5;
            for (int i = 0; i < s.Length; i++) s[i] += values[i] * scale;
            return s;
        }

        /// <summary>
        /// Learned value (-4..4) in main-action score units. Setup plays (5-95) move a lot; attacks (about 3.5-4) and End
        /// turn (1) only within their band, so attacking still comes after the setup plays that stay worth making.
        /// </summary>
        public static double MainBonus(DecisionOption o, double baseScore, double value, bool repeated)
        {
            if (o.Action == ActionType.Attack) return Clamp(0.5 * value, -3, 1.4);
            if (o.Action == ActionType.EndTurn) return Clamp(0.5 * value, -0.9, 2.4);
            if (baseScore >= 5)
            {
                double bonus = value * Math.Max(8, baseScore * 0.3);
                bonus = Clamp(bonus, -(baseScore + 5), 40);
                // The classic bot may already have used this several times this turn: never push it up again.
                return repeated ? Math.Min(0, bonus) : bonus;
            }
            if (baseScore >= 1) return Clamp(0.5 * value, -3, 1.4); // plays that end the turn (Boxed Order, Lumiose City)
            double b = Clamp(10 * value, -60, 60);
            // Plays the classic bot skips (no-gain loops among them): learning may pick one up, but not over and over.
            return repeated ? Math.Min(0, b) : b;
        }

        private static double Clamp(double x, double lo, double hi)
        {
            return x < lo ? lo : x > hi ? hi : x;
        }

        // ------------------------------------------------------------------ Exploring

        private List<int> Explore(DecisionRequest r, ScoredChoice c, double[] adjusted, List<int> greedy, bool[] repeated)
        {
            int n = adjusted.Length;
            int count = Math.Max(r.MinChoices, Math.Min(c.Count, r.MaxChoices));
            var order = HeuristicAgent.TopIndices(adjusted, n);
            if (count == 1 && greedy.Count == 1)
            {
                var candidates = new List<int>();
                foreach (int i in order)
                {
                    if (i == greedy[0] || (repeated != null && repeated[i])) continue;
                    if (candidates.Count < 2) candidates.Add(i);
                }
                if (r.Kind == DecisionKind.MainAction)
                {
                    // Always consider the best attack too: when to attack and with what matters most.
                    foreach (int i in order)
                    {
                        if (i == greedy[0] || r.Options[i].Action != ActionType.Attack) continue;
                        if (!candidates.Contains(i)) candidates.Add(i);
                        break;
                    }
                }
                if (candidates.Count == 0) return null;
                return new List<int> { candidates[exploreRng.Next(candidates.Count)] };
            }
            if (count <= 0 || count >= n || greedy.Count != count) return null;
            // Swap the weakest pick for the best option left out.
            var picks = new List<int>(greedy);
            int outside = -1;
            foreach (int i in order)
            {
                if (!picks.Contains(i))
                {
                    outside = i;
                    break;
                }
            }
            if (outside < 0) return null;
            int weakest = 0;
            for (int k = 1; k < picks.Count; k++)
                if (adjusted[picks[k]] < adjusted[picks[weakest]]) weakest = k;
            picks[weakest] = outside;
            return picks;
        }

        // ------------------------------------------------------------------ Repeat guard

        private static string UseKey(DecisionOption o)
        {
            return (int)o.Action + "|" + (o.Card == null ? "" : o.Card.Name) + "|" + o.Index;
        }

        private bool[] RepeatedOptions(DecisionRequest r)
        {
            if (r.Kind != DecisionKind.MainAction) return null;
            var result = new bool[r.Options.Count];
            for (int i = 0; i < r.Options.Count; i++)
            {
                int n;
                result[i] = usesThisTurn.TryGetValue(UseKey(r.Options[i]), out n) && n >= 2;
            }
            return result;
        }

        private void NoteUses(DecisionRequest r, List<int> chosen)
        {
            if (r.Kind != DecisionKind.MainAction) return;
            foreach (int i in chosen)
            {
                string key = UseKey(r.Options[i]);
                int n;
                usesThisTurn.TryGetValue(key, out n);
                usesThisTurn[key] = n + 1;
            }
        }

        // ------------------------------------------------------------------ What it would do (for reviews and imitation)

        /// <summary>
        /// The option(s) a learning agent would pick for this request (no exploring, no randomness), and the features
        /// of every option. Used to compare your plays with the AI's.
        /// </summary>
        public static List<int> BestFor(LearningBrain brain, DecisionRequest r, string myDeck, string oppDeck, out List<List<string>> feats, out double[] adjusted)
        {
            var h = new HeuristicAgent(1);
            var c = h.Score(r);
            feats = new List<List<string>>(r.Options.Count);
            for (int i = 0; i < r.Options.Count; i++) feats.Add(Features.For(r, i, myDeck, oppDeck));
            var values = brain != null ? brain.PlayValues(feats, NeuralNet.StateOf(r)) : new double[r.Options.Count];
            adjusted = Adjust(r, c, values, null);
            int count = Math.Max(r.MinChoices, Math.Min(c.Count, r.MaxChoices));
            return HeuristicAgent.TopIndices(adjusted, count);
        }

        /// <summary>
        /// Learns from your game: the brain moves toward your plays where they differ from what it would have done.
        /// weight: how much to trust this game (more when you won). Returns how many of your decisions it studied.
        /// </summary>
        public static int LearnFromPlayer(LearningBrain brain, IList<RecordedDecision> decisions, string yourDeck, string theirDeck, double weight)
        {
            if (brain == null || decisions == null) return 0;
            int studied = 0;
            foreach (var d in decisions)
            {
                var r = d.Request;
                if (r == null || d.Answer == null || r.Options.Count < 2 || r.View == null) continue;
                if (r.Kind == DecisionKind.MulliganDraws || r.Kind == DecisionKind.ChooseNumber || r.Kind == DecisionKind.CallCoinFlip || r.Kind == DecisionKind.ChoosePrize) continue;
                List<List<string>> feats = null;
                double[] adjusted = null;
                List<int> best = null;
                try
                {
                    best = BestFor(brain, r, yourDeck, theirDeck, out feats, out adjusted);
                }
                catch (Exception)
                {
                    continue; // a request the scorer can't read (shouldn't happen); skip it
                }
                studied++;
                var yours = d.Answer.Where(i => i >= 0 && i < r.Options.Count).Distinct().ToList();
                if (yours.Count == 0) continue;
                // The network studies every one of your choices (also the ones it already agrees with).
                brain.ImitateChoice(feats, NeuralNet.StateOf(r), yours, weight);
                if (yours.Count == best.Count && !yours.Except(best).Any()) continue; // it already plays like you here
                var preferred = MeanFeatures(feats, yours);
                var other = MeanFeatures(feats, best);
                brain.Imitate(preferred, other, weight);
            }
            return studied;
        }

        /// <summary>Features of the picked options, each listed once per pick (so multi-picks count proportionally).</summary>
        private static List<string> MeanFeatures(List<List<string>> feats, List<int> picks)
        {
            var list = new List<string>();
            foreach (int i in picks)
                if (i >= 0 && i < feats.Count && feats[i] != null) list.AddRange(feats[i]);
            return list;
        }
    }
}
