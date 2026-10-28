using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One decision a learning agent made, kept until the game ends so the result can be credited to it.</summary>
    public sealed class LearnedDecision
    {
        public int Turn;
        /// <summary>Feature names of each option (see Features.For).</summary>
        public List<List<string>> OptionFeatures;
        public List<int> Chosen;
        /// <summary>What the agent would have picked without exploring.</summary>
        public List<int> Greedy;
        /// <summary>The agent tried something other than its best guess (training only).</summary>
        public bool Explored;
        /// <summary>Prize cards each side had left at the time.</summary>
        public int MyPrizes, OppPrizes;
        /// <summary>The game state at the time, for the neural network (NeuralNet.StateOf); null in older records.</summary>
        public float[] State;
    }

    /// <summary>How a deck has done against another deck in training and in your games.</summary>
    public sealed class MatchupStats
    {
        public string Deck;
        public string Opponent;
        public int Games;
        public int Wins;
        public int Losses;
        /// <summary>Running average of the game result (-1..1), the "expected" result the brain compares each game with.</summary>
        public double Baseline;
        /// <summary>Running average of the Prize swing over two turns.</summary>
        public double ShortBaseline;
    }

    /// <summary>
    /// What the Neural AI's training games say about one card in one deck, for deck building: the games the deck was played
    /// with it, the games the card was in hand at some point ("drawn") and the games it was played, each with the wins.
    /// Lift compares the win rate when it was drawn with the win rate when it wasn't: a card that helps the deck wins more
    /// often when it shows up (the measure deck builders use; cards played only when behind can still look worse than they are).
    /// </summary>
    public sealed class CardResult
    {
        public string Deck = "";
        public string Card = "";
        public int Games, Wins;
        public int Drawn, DrawnWins;
        public int Played, PlayedWins;

        public int NotDrawn { get { return Games - Drawn; } }
        public double DrawnWinRate { get { return Drawn == 0 ? 0 : DrawnWins / (double)Drawn; } }
        public double NotDrawnWinRate { get { return NotDrawn <= 0 ? 0 : (Wins - DrawnWins) / (double)NotDrawn; } }
        /// <summary>Win rate when drawn minus when not drawn (-1..1).</summary>
        public double Lift { get { return DrawnWinRate - NotDrawnWinRate; } }
        /// <summary>Enough games both ways for Lift to mean something.</summary>
        public bool HasLift(int atLeast) { return Drawn >= atLeast && NotDrawn >= atLeast; }
    }

    /// <summary>
    /// One of your play habits the Areas of Improvement review keeps track of across your games: a kind of play where the AI
    /// would do something else (attacking before attaching Energy, picking one card over another...).
    /// </summary>
    public sealed class PlayHabit
    {
        public string Key = "";
        public string Title = "";
        /// <summary>Reviewed games it came up in, and how many times in all.</summary>
        public int Games, Times;
        /// <summary>Sum over the times of how much it seemed to matter (0..1 each: a measured loss of chance to win, or how sure the AI was).</summary>
        public double Strength;
        /// <summary>How often it comes up lately: an average over about the last 6 reviewed games (0..1), before and after this game.</summary>
        public double Recent, RecentBefore;
        /// <summary>Review number it last came up in.</summary>
        public int LastReview;

        public double AverageStrength { get { return Times == 0 ? 0 : Strength / Times; } }
    }

    /// <summary>A habit as it came up in one game (what GameAnalyzer hands to LearningBrain.RecordHabits).</summary>
    public sealed class HabitInGame
    {
        public string Key = "";
        public string Title = "";
        public int Times;
        public double Strength;
    }

    /// <summary>How well the Neural AI knows a deck from its training games: games and results, and how unsure it is lately.</summary>
    public sealed class DeckSkill
    {
        public string Deck = "";
        public int Games, Wins, Losses;
        public long Decisions;
        /// <summary>
        /// Recent average of how unlikely its own picks were to it (-log p per decision, over about the last 50 games): about
        /// 0.6 is a deck it plays confidently, 0.9 and up one it's still working out.
        /// </summary>
        public double Uncertainty;

        public double WinRate { get { return Games == 0 ? 0 : Wins / (double)Games; } }

        /// <summary>"well learned", "learning" or "still new to it", from the uncertainty.</summary>
        public string Level
        {
            get { return Decisions == 0 ? "not played yet" : Uncertainty <= 0.68 ? "well learned" : Uncertainty <= 0.82 ? "learning" : "still new to it"; }
        }
    }

    /// <summary>
    /// What the learning AI thinks of playing one card (v75, for the deck advisor): the average weight of the features about
    /// playing it (benching, evolving into, attacking or using an Ability with it, playing it as a Trainer), weighted by how
    /// often each was updated. Positive: playing it tended to work out better than the alternatives.
    /// </summary>
    public sealed class CardOpinion
    {
        public string Card = "";
        public double Value;
        /// <summary>How many updates the opinion rests on.</summary>
        public int Updates;
    }

    /// <summary>One training session, for the history chart.</summary>
    public sealed class SessionRecord
    {
        public string When = "";
        public string DeckA = "";
        public string DeckB = "";
        public int Games;
        public int WinsA;
        public int WinsB;
        /// <summary>Win rate (0..1) of the learning AI against the classic bot before and after the session, or -1 if not measured.</summary>
        public double ScoreBefore = -1;
        public double ScoreAfter = -1;
        public int EvalGames;
        /// <summary>How the AI played in the session ("Learning AI", "Neural AI", "Hybrid AI"; v78), "" for older sessions.</summary>
        public string Style = "";
    }

    /// <summary>
    /// What the learning AI knows: a weight for each feature (see Features). An option's learned value is the sum of
    /// its features' weights; LearningAgent adds it to the classic bot's score for that option.
    ///
    /// Weights move after every game: decisions the agent made (above all the ones where it tried something
    /// different) are credited with how the game went from there, compared with how that matchup usually goes.
    /// Your games add a second signal: the brain moves toward the plays you made where they differ from its own,
    /// and "I disagree" / "Makes sense" on the Areas of Improvement screen moves it directly.
    ///
    /// Thread-safe: training, a live game and the UI can all use one brain.
    ///
    /// v74: it also holds a neural network (Net) that learns from the same games, your plays and your feedback. Style says how
    /// the agents use them: the feature weights (Learning AI), the network (Neural AI) or both added together (Hybrid AI).
    /// </summary>
    public sealed class LearningBrain
    {
        private sealed class Weight
        {
            public double W;
            public int N;
        }

        private readonly object sync = new object();
        private readonly Dictionary<string, Weight> weights = new Dictionary<string, Weight>(StringComparer.Ordinal);
        private readonly Dictionary<string, MatchupStats> matchups = new Dictionary<string, MatchupStats>(StringComparer.Ordinal);
        private readonly List<SessionRecord> history = new List<SessionRecord>();
        private readonly Dictionary<string, CardResult> cardResults = new Dictionary<string, CardResult>(StringComparer.Ordinal);
        private readonly Dictionary<string, DeckSkill> deckSkills = new Dictionary<string, DeckSkill>(StringComparer.Ordinal);
        private readonly Dictionary<string, PlayHabit> habits = new Dictionary<string, PlayHabit>(StringComparer.Ordinal);
        /// <summary>Your games the Areas of Improvement review has gone through (the habits' denominator).</summary>
        public int HabitReviews { get; private set; }

        /// <summary>Largest weight of one feature.</summary>
        public const double MaxWeight = 3.0;
        /// <summary>Largest learned value of one option (sum of its weights).</summary>
        public const double MaxValue = 4.0;
        /// <summary>Step size for game results (shrinks for features that have been updated many times).</summary>
        public double LearningRate = 0.12;
        /// <summary>Step size for copying your plays.</summary>
        public double ImitationRate = 0.06;
        /// <summary>Credit per own turn between a decision and the end of the game.</summary>
        public double Discount = 0.93;
        /// <summary>Decisions where the agent simply took its best guess count this much (exploring ones count fully).</summary>
        public double GreedyFactor = 0.1;
        /// <summary>Share of each decision's credit that comes from the Prize swing over the next two turns instead of the game result.</summary>
        public double PrizeShare = 0.4;

        /// <summary>The small network that learns alongside the feature weights (v74).</summary>
        public readonly NeuralNet Net = new NeuralNet();
        /// <summary>
        /// The Neural AI: the policy/value network ported from the Python bot (PolicyNet). It starts from PolicyStart (the Python
        /// bot's trained weights) and trains by PPO in Neural AI training sessions. Saved next to the brain's file (FilePath + ".neural").
        /// </summary>
        public readonly PolicyNet Policy = new PolicyNet();
        /// <summary>
        /// The network the Neural AI starts from and goes back to when the AI is reset: Resources/AI/policy_start.bytes, exported
        /// from the Python bot by PokemonTCGBot/tools/export_to_sim.py. Set once at startup; nothing ever writes to it.
        /// </summary>
        public static byte[] PolicyStart;

        public LearningBrain()
        {
            ResetPolicy();
        }

        /// <summary>The Neural AI back to where it started (the Python bot's weights, or a fresh network without them).</summary>
        public void ResetPolicy()
        {
            var start = PolicyStart;
            if (start == null || !Policy.FromBytes(start)) Policy.Reset();
        }

        /// <summary>
        /// The Neural AI copies your choices (one learning step for the whole game). picks: each decision as the network saw it
        /// and the option you took; strength: how much to trust them (more when you won).
        /// </summary>
        public void ImitatePolicy(List<KeyValuePair<PolicyInput, int>> picks, double strength)
        {
            if (picks == null || picks.Count == 0 || strength <= 0) return;
            Policy.Imitate(picks, (float)(NeuralImitationRate * strength));
        }
        /// <summary>How agents playing with this brain choose (the Opponent switch). Not saved with the brain; the menu sets it.</summary>
        public volatile AiStyle Style = AiStyle.Hybrid;
        /// <summary>The network learns from decisions where the agent simply took its best guess at this share of their credit.</summary>
        public double NeuralGreedyFactor = 0.3;
        /// <summary>Strength of the network's step toward a play it studies (yours, or one in a game log).</summary>
        public double NeuralImitationRate = 0.5;

        public int GamesTrained;
        public int GamesVsYou;
        public int YourDecisionsStudied;
        public int FeedbackGiven;
        /// <summary>Game logs studied (TCG Live battle logs and games saved by this app, v76) and the decisions they held.</summary>
        public int LogsStudied;
        public int LogDecisionsStudied;

        /// <summary>File Save() writes to (set by whoever loads the brain).</summary>
        public string FilePath;

        public int FeatureCount
        {
            get { lock (sync) return weights.Count; }
        }

        // ------------------------------------------------------------------ Values

        /// <summary>Learned value of each option (sum of its features' weights, clamped).</summary>
        public double[] Values(List<List<string>> optionFeatures)
        {
            var result = new double[optionFeatures.Count];
            lock (sync)
            {
                for (int i = 0; i < optionFeatures.Count; i++)
                {
                    double sum = 0;
                    var feats = optionFeatures[i];
                    if (feats != null)
                    {
                        foreach (var f in feats)
                        {
                            Weight w;
                            if (weights.TryGetValue(f, out w)) sum += w.W;
                        }
                    }
                    result[i] = Clamp(sum, -MaxValue, MaxValue);
                }
            }
            return result;
        }

        /// <summary>
        /// The values an agent plays with, by Style: the feature weights, the network, or both added together (clamped). state is
        /// NeuralNet.StateOf for the decision.
        /// </summary>
        public double[] PlayValues(List<List<string>> optionFeatures, float[] state)
        {
            return PlayValues(optionFeatures, state, Style);
        }

        /// <summary>The values for a given style (v78: training can play a style other than your opponent's).</summary>
        public double[] PlayValues(List<List<string>> optionFeatures, float[] state, AiStyle style)
        {
            if (style == AiStyle.Learning) return Values(optionFeatures);
            var neural = Net.Values(optionFeatures, state);
            if (style == AiStyle.Neural) return neural;
            var linear = Values(optionFeatures);
            for (int i = 0; i < linear.Length && i < neural.Length; i++) linear[i] = Clamp(linear[i] + neural[i], -MaxValue, MaxValue);
            return linear;
        }

        public double WeightOf(string feature)
        {
            lock (sync)
            {
                Weight w;
                return weights.TryGetValue(feature, out w) ? w.W : 0;
            }
        }

        public void SetWeight(string feature, double value)
        {
            lock (sync) Get(feature).W = Clamp(value, -MaxWeight, MaxWeight);
        }

        // ------------------------------------------------------------------ Learning

        /// <summary>
        /// Credits one side's decisions with how the game went. outcome: 1 win, -1 loss, 0 draw.
        /// Prize counts are the ones left at the end of the game.
        /// </summary>
        public void LearnFromGame(IList<LearnedDecision> decisions, double outcome, int finalTurn, int myPrizesLeft, int oppPrizesLeft, string deck, string opponent)
        {
            if (decisions == null) return;
            lock (sync)
            {
                var stats = Matchup(deck, opponent);
                double baseline = stats.Baseline;
                double shortBaseline = stats.ShortBaseline;
                double shortSum = 0;
                int shortCount = 0;
                for (int i = 0; i < decisions.Count; i++)
                {
                    var d = decisions[i];
                    if (d == null || d.OptionFeatures == null || d.OptionFeatures.Count < 2) continue;

                    // Prize swing over the next two turns of each player (or to the end of the game).
                    int myLater = myPrizesLeft, oppLater = oppPrizesLeft;
                    for (int j = i + 1; j < decisions.Count; j++)
                    {
                        if (decisions[j].Turn >= d.Turn + 4)
                        {
                            myLater = decisions[j].MyPrizes;
                            oppLater = decisions[j].OppPrizes;
                            break;
                        }
                    }
                    double swing = Clamp(((d.MyPrizes - myLater) - (d.OppPrizes - oppLater)) / 3.0, -1, 1);
                    shortSum += swing;
                    shortCount++;

                    int ownTurnsLeft = Math.Max(0, (finalTurn - d.Turn) / 2);
                    double credit = (1 - PrizeShare) * (outcome - baseline) * Math.Pow(Discount, ownTurnsLeft)
                                    + PrizeShare * (swing - shortBaseline);
                    if (Math.Abs(credit) < 1e-6) continue;

                    var grad = new Dictionary<string, double>(StringComparer.Ordinal);
                    double factor;
                    if (d.Explored)
                    {
                        AddMean(grad, d.OptionFeatures, d.Chosen, 1);
                        AddMean(grad, d.OptionFeatures, d.Greedy, -1);
                        factor = 1;
                    }
                    else
                    {
                        AddMean(grad, d.OptionFeatures, d.Chosen, 1);
                        AddMeanAll(grad, d.OptionFeatures, -1);
                        factor = GreedyFactor;
                    }
                    Step(grad, LearningRate * factor * credit);
                    // The network learns from the same credit (a policy-gradient step on the choice).
                    if (d.State != null && d.Chosen != null)
                        Net.Learn(d.OptionFeatures, d.State, d.Chosen, credit * (d.Explored ? 1.0 : NeuralGreedyFactor));
                }

                stats.Games++;
                if (outcome > 0) stats.Wins++;
                else if (outcome < 0) stats.Losses++;
                double rate = Math.Max(0.02, 1.0 / stats.Games);
                stats.Baseline += rate * (outcome - stats.Baseline);
                if (shortCount > 0) stats.ShortBaseline += rate * (shortSum / shortCount - stats.ShortBaseline);
            }
        }

        /// <summary>Moves the brain toward a play someone else made (yours) and away from the one it would have made.</summary>
        public void Imitate(List<string> preferredFeatures, List<string> otherFeatures, double strength)
        {
            var grad = new Dictionary<string, double>(StringComparer.Ordinal);
            if (preferredFeatures != null) foreach (var f in preferredFeatures) Accumulate(grad, f, 1);
            if (otherFeatures != null) foreach (var f in otherFeatures) Accumulate(grad, f, -1);
            lock (sync) Step(grad, ImitationRate * strength);
        }

        /// <summary>
        /// The network studies one choice someone made (you, or a player in a game log): options are the decision's option
        /// features, state its NeuralNet.StateOf, chosen the picks. strength: how much to trust it (more for a winner's play).
        /// </summary>
        public void ImitateChoice(List<List<string>> options, float[] state, IList<int> chosen, double strength)
        {
            Net.Learn(options, state, chosen, NeuralImitationRate * strength);
        }

        /// <summary>
        /// Feedback from the Areas of Improvement screen: shift the learned values so the preferred play scores at least
        /// margin above the other one (or move by one strong step if it's already there).
        /// </summary>
        public void ApplyPreference(List<string> preferredFeatures, List<string> otherFeatures, double margin)
        {
            var grad = new Dictionary<string, double>(StringComparer.Ordinal);
            if (preferredFeatures != null) foreach (var f in preferredFeatures) Accumulate(grad, f, 1);
            if (otherFeatures != null) foreach (var f in otherFeatures) Accumulate(grad, f, -1);
            // The network: the preferred play over the other one, as a 2-option choice.
            Net.Learn(new List<List<string>> { preferredFeatures ?? new List<string>(), otherFeatures ?? new List<string>() }, null, new[] { 0 }, 1.0);
            int touched = grad.Count(kv => Math.Abs(kv.Value) > 1e-9);
            if (touched == 0) return;
            lock (sync)
            {
                double gap = 0;
                foreach (var kv in grad)
                {
                    Weight w;
                    if (weights.TryGetValue(kv.Key, out w)) gap += w.W * kv.Value;
                }
                double needed = Math.Max(0.5, margin - gap);
                double perFeature = Math.Min(1.0, needed / touched);
                foreach (var kv in grad)
                {
                    if (Math.Abs(kv.Value) < 1e-9) continue;
                    var w = Get(kv.Key);
                    w.W = Clamp(w.W + perFeature * Math.Sign(kv.Value), -MaxWeight, MaxWeight);
                    w.N++;
                }
            }
        }

        /// <summary>Counts one studied game log and its decisions (v76).</summary>
        public void NoteLogStudied(int decisions)
        {
            lock (sync)
            {
                LogsStudied++;
                LogDecisionsStudied += Math.Max(0, decisions);
            }
        }

        /// <summary>Counts one "I disagree" / "Makes sense" answer.</summary>
        public void NoteFeedback()
        {
            lock (sync) FeedbackGiven++;
        }

        private static void AddMean(Dictionary<string, double> grad, List<List<string>> options, List<int> picks, double sign)
        {
            if (picks == null || picks.Count == 0) return;
            double share = sign / picks.Count;
            foreach (int i in picks)
            {
                if (i < 0 || i >= options.Count || options[i] == null) continue;
                foreach (var f in options[i]) Accumulate(grad, f, share);
            }
        }

        private static void AddMeanAll(Dictionary<string, double> grad, List<List<string>> options, double sign)
        {
            double share = sign / options.Count;
            foreach (var feats in options)
            {
                if (feats == null) continue;
                foreach (var f in feats) Accumulate(grad, f, share);
            }
        }

        private static void Accumulate(Dictionary<string, double> grad, string f, double amount)
        {
            double old;
            grad.TryGetValue(f, out old);
            grad[f] = old + amount;
        }

        /// <summary>Gradient step; features seen often move more slowly. Call inside the lock.</summary>
        private void Step(Dictionary<string, double> grad, double scale)
        {
            if (Math.Abs(scale) < 1e-12) return;
            foreach (var kv in grad)
            {
                if (Math.Abs(kv.Value) < 1e-9) continue;
                var w = Get(kv.Key);
                double rate = scale / Math.Sqrt(1 + w.N / 20.0);
                w.W = Clamp(w.W + rate * kv.Value, -MaxWeight, MaxWeight);
                w.N++;
            }
        }

        private Weight Get(string feature)
        {
            Weight w;
            if (!weights.TryGetValue(feature, out w))
            {
                w = new Weight();
                weights[feature] = w;
            }
            return w;
        }

        private static double Clamp(double x, double lo, double hi)
        {
            return x < lo ? lo : x > hi ? hi : x;
        }

        // ------------------------------------------------------------------ Matchups and history

        private MatchupStats Matchup(string deck, string opponent)
        {
            deck = deck ?? "";
            opponent = opponent ?? "";
            string key = deck + "\u0001" + opponent;
            MatchupStats m;
            if (!matchups.TryGetValue(key, out m))
            {
                m = new MatchupStats { Deck = deck, Opponent = opponent };
                matchups[key] = m;
            }
            return m;
        }

        /// <summary>
        /// Adds a game to a matchup's games, wins and losses only (outcome: 1 win, -1 loss, 0 draw): the Neural AI's training
        /// games, which the feature weights don't learn from, so their expected result is left alone.
        /// </summary>
        public void RecordMatchup(string deck, string opponent, int outcome)
        {
            lock (sync)
            {
                var stats = Matchup(deck, opponent);
                stats.Games++;
                if (outcome > 0) stats.Wins++;
                else if (outcome < 0) stats.Losses++;
            }
        }

        // ------------------------------------------------------------------ Deck building: what training says about decks and cards

        private CardResult CardResultFor(string deck, string card)
        {
            string key = deck + "\u0001" + card;
            CardResult r;
            if (!cardResults.TryGetValue(key, out r))
            {
                r = new CardResult { Deck = deck, Card = card };
                cardResults[key] = r;
            }
            return r;
        }

        private DeckSkill SkillFor(string deck)
        {
            DeckSkill s;
            if (!deckSkills.TryGetValue(deck, out s))
            {
                s = new DeckSkill { Deck = deck };
                deckSkills[deck] = s;
            }
            return s;
        }

        /// <summary>
        /// One side of a Neural AI training game: the deck, its cards (Basic Energy left out), the cards it drew and played, how
        /// it ended (1 won, -1 lost, 0 draw) and its decisions with their total surprise (-log p).
        /// </summary>
        public void RecordDeckGame(string deck, IEnumerable<CardDef> cards, ICollection<string> drawn, ICollection<string> played, int outcome,
            int decisions, double surprise)
        {
            if (string.IsNullOrEmpty(deck) || cards == null) return;
            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var c in cards)
                if (c != null && !c.IsBasicEnergy) names.Add(c.Name);
            bool won = outcome > 0;
            lock (sync)
            {
                foreach (var name in names)
                {
                    var r = CardResultFor(deck, name);
                    r.Games++;
                    if (won) r.Wins++;
                    if (drawn != null && drawn.Contains(name))
                    {
                        r.Drawn++;
                        if (won) r.DrawnWins++;
                    }
                    if (played != null && played.Contains(name))
                    {
                        r.Played++;
                        if (won) r.PlayedWins++;
                    }
                }
                var s = SkillFor(deck);
                s.Games++;
                if (won) s.Wins++;
                else if (outcome < 0) s.Losses++;
                if (decisions > 0)
                {
                    double avg = surprise / decisions;
                    s.Uncertainty = s.Decisions == 0 ? avg : s.Uncertainty + Math.Max(0.02, 1.0 / s.Games) * (avg - s.Uncertainty);
                    s.Decisions += decisions;
                }
            }
        }

        /// <summary>The card results for a deck, or for every deck added together (deck null), most games first.</summary>
        public List<CardResult> CardResultsOf(string deck)
        {
            lock (sync)
            {
                if (deck != null)
                    return cardResults.Values.Where(r => r.Deck == deck).Select(Copy).OrderByDescending(r => r.Games).ToList();
                var all = new Dictionary<string, CardResult>(StringComparer.Ordinal);
                foreach (var r in cardResults.Values)
                {
                    CardResult t;
                    if (!all.TryGetValue(r.Card, out t))
                    {
                        t = new CardResult { Deck = "", Card = r.Card };
                        all[r.Card] = t;
                    }
                    t.Games += r.Games;
                    t.Wins += r.Wins;
                    t.Drawn += r.Drawn;
                    t.DrawnWins += r.DrawnWins;
                    t.Played += r.Played;
                    t.PlayedWins += r.PlayedWins;
                }
                return all.Values.OrderByDescending(r => r.Games).ToList();
            }
        }

        private static CardResult Copy(CardResult r)
        {
            return new CardResult
            {
                Deck = r.Deck, Card = r.Card, Games = r.Games, Wins = r.Wins, Drawn = r.Drawn, DrawnWins = r.DrawnWins, Played = r.Played,
                PlayedWins = r.PlayedWins
            };
        }

        /// <summary>How well the Neural AI knows a deck (an empty record if it hasn't trained with it).</summary>
        public DeckSkill SkillOf(string deck)
        {
            lock (sync)
            {
                DeckSkill s;
                if (deck == null || !deckSkills.TryGetValue(deck, out s)) return new DeckSkill { Deck = deck ?? "" };
                return new DeckSkill { Deck = s.Deck, Games = s.Games, Wins = s.Wins, Losses = s.Losses, Decisions = s.Decisions, Uncertainty = s.Uncertainty };
            }
        }

        /// <summary>Every deck the Neural AI has trained with, most games first.</summary>
        public List<DeckSkill> DeckSkills()
        {
            lock (sync)
                return deckSkills.Values.Select(s => new DeckSkill { Deck = s.Deck, Games = s.Games, Wins = s.Wins, Losses = s.Losses, Decisions = s.Decisions, Uncertainty = s.Uncertainty })
                    .OrderByDescending(s => s.Games).ToList();
        }

        // ------------------------------------------------------------------ Your habits (the Areas of Improvement review)

        /// <summary>How fast "lately" forgets: each review counts this much of the recent rate.</summary>
        public const double HabitRecency = 0.15;

        /// <summary>
        /// Adds one reviewed game's habits to your profile and returns them with their history (games, times, how often lately,
        /// before and after this game). Habits that didn't come up this game become a little rarer "lately".
        /// </summary>
        public List<PlayHabit> RecordHabits(IList<HabitInGame> found)
        {
            lock (sync)
            {
                HabitReviews++;
                var now = new HashSet<string>(StringComparer.Ordinal);
                foreach (var h in found ?? new List<HabitInGame>())
                    if (h != null && !string.IsNullOrEmpty(h.Key) && h.Times > 0) now.Add(h.Key);
                foreach (var h in habits.Values)
                {
                    h.RecentBefore = h.Recent;
                    if (!now.Contains(h.Key)) h.Recent *= 1 - HabitRecency;
                }
                var result = new List<PlayHabit>();
                foreach (var f in found ?? new List<HabitInGame>())
                {
                    if (f == null || !now.Contains(f.Key)) continue;
                    PlayHabit h;
                    if (!habits.TryGetValue(f.Key, out h))
                    {
                        h = new PlayHabit { Key = f.Key };
                        habits[f.Key] = h;
                    }
                    if (!string.IsNullOrEmpty(f.Title)) h.Title = f.Title;
                    if (h.LastReview != HabitReviews)
                    {
                        h.Games++;
                        h.Recent = h.Recent * (1 - HabitRecency) + HabitRecency;
                        h.LastReview = HabitReviews;
                    }
                    h.Times += f.Times;
                    h.Strength += Math.Max(0, Math.Min(1, f.Strength)) * f.Times;
                    result.Add(CopyHabit(h));
                }
                return result;
            }
        }

        /// <summary>Your habits, most common lately first.</summary>
        public List<PlayHabit> Habits()
        {
            lock (sync) return habits.Values.Select(CopyHabit).OrderByDescending(h => h.Recent).ThenByDescending(h => h.Games).ToList();
        }

        private static PlayHabit CopyHabit(PlayHabit h)
        {
            return new PlayHabit
            {
                Key = h.Key, Title = h.Title, Games = h.Games, Times = h.Times, Strength = h.Strength, Recent = h.Recent, RecentBefore = h.RecentBefore,
                LastReview = h.LastReview
            };
        }

        /// <summary>Games a card's lift needs both ways (drawn and not drawn) before the advisor uses it.</summary>
        public const int LiftGames = 25;

        public List<MatchupStats> Matchups()
        {
            lock (sync)
            {
                return matchups.Values.Select(m => new MatchupStats
                {
                    Deck = m.Deck, Opponent = m.Opponent, Games = m.Games, Wins = m.Wins, Losses = m.Losses,
                    Baseline = m.Baseline, ShortBaseline = m.ShortBaseline
                }).OrderByDescending(m => m.Games).ToList();
            }
        }

        public void AddSession(SessionRecord s)
        {
            lock (sync) history.Add(s);
        }

        public List<SessionRecord> History()
        {
            lock (sync) return history.ToList();
        }

        /// <summary>
        /// The strongest things the brain has learned (largest weights), most confident first; count 0 or less returns all of them.
        /// Only weights updated at least minUpdates times and not close to zero count as lessons.
        /// </summary>
        public List<KeyValuePair<string, double>> StrongestLessons(int count, int minUpdates)
        {
            List<KeyValuePair<string, double>> found;
            lock (sync)
            {
                found = weights.Where(kv => kv.Value.N >= minUpdates && Math.Abs(kv.Value.W) >= 0.05)
                    .Select(kv => new KeyValuePair<string, double>(kv.Key, kv.Value.W))
                    .ToList();
            }
            // Sorted outside the lock, so a long list doesn't hold up training while the Train screen refreshes.
            found.Sort((x, y) =>
            {
                int byWeight = Math.Abs(y.Value).CompareTo(Math.Abs(x.Value));
                return byWeight != 0 ? byWeight : string.CompareOrdinal(x.Key, y.Key);
            });
            return count > 0 && found.Count > count ? found.GetRange(0, count) : found;
        }

        private static readonly string[] OpinionPrefixes = { "M:Trainer:", "M:Bench:", "M:Evolve:", "M:HandAbility:", "M:Stadium:", "M:Attack:", "M:Ability:" };

        /// <summary>
        /// The card a feature is about, for CardOpinions: only the plain "playing this card" features ("M:Trainer:Ultra Ball",
        /// "M:Attack:Dragapult ex/Phantom Dive"), not the ones for a situation ("...|early") or the setup choices. Null otherwise.
        /// </summary>
        public static string CardOfFeature(string feature)
        {
            if (string.IsNullOrEmpty(feature) || feature.IndexOf('|') >= 0) return null;
            foreach (var prefix in OpinionPrefixes)
            {
                if (!feature.StartsWith(prefix, StringComparison.Ordinal)) continue;
                string rest = feature.Substring(prefix.Length);
                if (prefix == "M:Attack:" || prefix == "M:Ability:")
                {
                    int slash = rest.IndexOf('/');
                    if (slash > 0) rest = rest.Substring(0, slash);
                }
                return rest.Length == 0 || rest == "?" ? null : rest;
            }
            return null;
        }

        /// <summary>
        /// What the brain has learned about playing each card, by card name (v75, the deck advisor reads it). The general
        /// features count, plus the ones for this deck ("D:deck|...", a little more) and for this matchup ("X:deck>opponent|...",
        /// more again) when the names are given.
        /// </summary>
        public Dictionary<string, CardOpinion> CardOpinions(string deck, string opponent)
        {
            string deckPrefix = string.IsNullOrEmpty(deck) ? null : "D:" + deck + "|";
            string matchPrefix = string.IsNullOrEmpty(deck) || string.IsNullOrEmpty(opponent) ? null : "X:" + deck + ">" + opponent + "|";
            var sums = new Dictionary<string, double[]>(StringComparer.Ordinal);
            lock (sync)
            {
                foreach (var kv in weights)
                {
                    string f = kv.Key;
                    double scale = 1.0;
                    if (f.StartsWith("D:", StringComparison.Ordinal))
                    {
                        if (deckPrefix == null || !f.StartsWith(deckPrefix, StringComparison.Ordinal)) continue;
                        f = f.Substring(deckPrefix.Length);
                        scale = 1.5;
                    }
                    else if (f.StartsWith("X:", StringComparison.Ordinal))
                    {
                        if (matchPrefix == null || !f.StartsWith(matchPrefix, StringComparison.Ordinal)) continue;
                        f = f.Substring(matchPrefix.Length);
                        scale = 2.0;
                    }
                    string card = CardOfFeature(f);
                    if (card == null || kv.Value.N <= 0) continue;
                    double share = Math.Min(kv.Value.N, 200) * scale;
                    double[] acc;
                    if (!sums.TryGetValue(card, out acc))
                    {
                        acc = new double[3];
                        sums[card] = acc;
                    }
                    acc[0] += kv.Value.W * share;
                    acc[1] += share;
                    acc[2] += kv.Value.N;
                }
            }
            var result = new Dictionary<string, CardOpinion>(StringComparer.Ordinal);
            foreach (var kv in sums)
            {
                if (kv.Value[1] <= 0) continue;
                result[kv.Key] = new CardOpinion { Card = kv.Key, Value = kv.Value[0] / kv.Value[1], Updates = (int)Math.Min(int.MaxValue, kv.Value[2]) };
            }
            // The Neural AI's training games: how much more often the deck wins when the card shows up (its lift). This deck's
            // games count fully; every deck's together, at half weight, for cards this deck hasn't played enough with.
            var mine = string.IsNullOrEmpty(deck) ? new List<CardResult>() : CardResultsOf(deck).Where(r => r.HasLift(LiftGames)).ToList();
            var seen = new HashSet<string>(mine.Select(r => r.Card), StringComparer.Ordinal);
            foreach (var r in mine) MergeLift(result, r, 1.0);
            foreach (var r in CardResultsOf(null))
                if (!seen.Contains(r.Card) && r.HasLift(LiftGames)) MergeLift(result, r, string.IsNullOrEmpty(deck) ? 1.0 : 0.5);
            return result;
        }

        /// <summary>Adds a card's lift to its opinion (scaled to the feature weights' range, weighted by its games).</summary>
        private static void MergeLift(Dictionary<string, CardOpinion> opinions, CardResult r, double weight)
        {
            double value = Clamp(3.0 * r.Lift, -1, 1);
            int updates = (int)(Math.Min(r.Drawn, r.NotDrawn) * weight);
            if (updates <= 0) return;
            CardOpinion op;
            if (!opinions.TryGetValue(r.Card, out op))
            {
                opinions[r.Card] = new CardOpinion { Card = r.Card, Value = value, Updates = updates };
                return;
            }
            long total = (long)op.Updates + updates;
            op.Value = (op.Value * op.Updates + value * updates) / total;
            op.Updates = (int)Math.Min(int.MaxValue, total);
        }

        /// <summary>How a deck has done in the brain's games, all opponents together (v75): games, wins, losses.</summary>
        public MatchupStats RecordOf(string deck)
        {
            var total = new MatchupStats { Deck = deck ?? "", Opponent = "" };
            lock (sync)
            {
                foreach (var m in matchups.Values)
                {
                    if (m.Deck != deck) continue;
                    total.Games += m.Games;
                    total.Wins += m.Wins;
                    total.Losses += m.Losses;
                }
            }
            return total;
        }

        /// <summary>How deck has done against opponent in the brain's games (v75), or null if they never met.</summary>
        public MatchupStats MatchupOf(string deck, string opponent)
        {
            lock (sync)
            {
                MatchupStats m;
                if (!matchups.TryGetValue((deck ?? "") + "\u0001" + (opponent ?? ""), out m) || m.Games == 0) return null;
                return new MatchupStats { Deck = m.Deck, Opponent = m.Opponent, Games = m.Games, Wins = m.Wins, Losses = m.Losses, Baseline = m.Baseline, ShortBaseline = m.ShortBaseline };
            }
        }

        public void IncrementCounters(int gamesTrained, int gamesVsYou, int decisionsStudied)
        {
            lock (sync)
            {
                GamesTrained += gamesTrained;
                GamesVsYou += gamesVsYou;
                YourDecisionsStudied += decisionsStudied;
            }
        }

        public void Reset()
        {
            lock (sync)
            {
                weights.Clear();
                matchups.Clear();
                history.Clear();
                cardResults.Clear();
                deckSkills.Clear();
                habits.Clear();
                HabitReviews = 0;
                GamesTrained = 0;
                GamesVsYou = 0;
                YourDecisionsStudied = 0;
                FeedbackGiven = 0;
                LogsStudied = 0;
                LogDecisionsStudied = 0;
            }
            Net.Reset();
            ResetPolicy();
        }

        // ------------------------------------------------------------------ Saving

        private const string Header = "# Pokemon TCG learning brain v1";

        /// <summary>
        /// Text format, one record per line, fields separated by tabs:
        ///   c  gamesTrained  gamesVsYou  decisionsStudied  feedback  [logsStudied  logDecisionsStudied (v76)]
        ///   w  weight  updates  feature
        ///   m  games  wins  losses  baseline  shortBaseline  deck  opponent
        ///   s  games  winsA  winsB  scoreBefore  scoreAfter  evalGames  when  deckA  deckB
        ///   n  base64 of the neural network's weights (v74; only once it has learned something)
        /// </summary>
        public string Serialize()
        {
            using (var w = new StringWriter(CultureInfo.InvariantCulture))
            {
                WriteTo(w);
                // The Neural AI's network too (for "Undo this session"); the brain's file keeps it in its own file instead.
                w.Write("p\t");
                w.Write(Convert.ToBase64String(Policy.ToBytes()));
                w.Write('\n');
                return w.ToString();
            }
        }

        /// <summary>
        /// Writes Serialize()'s text. v134.4: the lock is held only to copy the numbers out, and the sorting and formatting happen
        /// after, so the AIs playing (which read the weights under the same lock for every decision) never wait for a save. A
        /// big brain was holding them up for seconds.
        /// </summary>
        public void WriteTo(TextWriter w)
        {
            var sb = new StringBuilder();
            string[] keys;
            double[] values;
            int[] updates;
            int count = 0;
            lock (sync)
            {
                sb.Append(Header).Append('\n');
                sb.Append("c\t").Append(GamesTrained).Append('\t').Append(GamesVsYou).Append('\t').Append(YourDecisionsStudied).Append('\t').Append(FeedbackGiven)
                  .Append('\t').Append(LogsStudied).Append('\t').Append(LogDecisionsStudied).Append('\n');
                foreach (var m in matchups.Values)
                {
                    sb.Append("m\t").Append(m.Games).Append('\t').Append(m.Wins).Append('\t').Append(m.Losses).Append('\t')
                      .Append(D(m.Baseline)).Append('\t').Append(D(m.ShortBaseline)).Append('\t').Append(Clean(m.Deck)).Append('\t').Append(Clean(m.Opponent)).Append('\n');
                }
                foreach (var r in cardResults.Values.OrderBy(x => x.Deck, StringComparer.Ordinal).ThenBy(x => x.Card, StringComparer.Ordinal))
                {
                    sb.Append("k\t").Append(r.Games).Append('\t').Append(r.Wins).Append('\t').Append(r.Drawn).Append('\t').Append(r.DrawnWins).Append('\t')
                      .Append(r.Played).Append('\t').Append(r.PlayedWins).Append('\t').Append(Clean(r.Deck)).Append('\t').Append(Clean(r.Card)).Append('\n');
                }
                foreach (var k in deckSkills.Values.OrderBy(x => x.Deck, StringComparer.Ordinal))
                {
                    sb.Append("q\t").Append(k.Games).Append('\t').Append(k.Wins).Append('\t').Append(k.Losses).Append('\t').Append(k.Decisions).Append('\t')
                      .Append(D(k.Uncertainty)).Append('\t').Append(Clean(k.Deck)).Append('\n');
                }
                if (HabitReviews > 0) sb.Append("hr\t").Append(HabitReviews).Append('\n');
                foreach (var h in habits.Values.OrderBy(x => x.Key, StringComparer.Ordinal))
                {
                    sb.Append("h\t").Append(h.Games).Append('\t').Append(h.Times).Append('\t').Append(D(h.Strength)).Append('\t').Append(D(h.Recent)).Append('\t')
                      .Append(h.LastReview).Append('\t').Append(Clean(h.Key)).Append('\t').Append(Clean(h.Title)).Append('\n');
                }
                foreach (var s in history)
                {
                    sb.Append("s\t").Append(s.Games).Append('\t').Append(s.WinsA).Append('\t').Append(s.WinsB).Append('\t')
                      .Append(D(s.ScoreBefore)).Append('\t').Append(D(s.ScoreAfter)).Append('\t').Append(s.EvalGames).Append('\t')
                      .Append(Clean(s.When)).Append('\t').Append(Clean(s.DeckA)).Append('\t').Append(Clean(s.DeckB)).Append('\t').Append(Clean(s.Style)).Append('\n');
                }
                keys = new string[weights.Count];
                values = new double[weights.Count];
                updates = new int[weights.Count];
                foreach (var kv in weights)
                {
                    if (Math.Abs(kv.Value.W) < 1e-6) continue;
                    keys[count] = kv.Key;
                    values[count] = kv.Value.W;
                    updates[count] = kv.Value.N;
                    count++;
                }
            }
            w.Write(sb.ToString());
            // Same order as before (by feature, ordinal), so the file doesn't change from one save to the next for no reason.
            var order = new int[count];
            for (int i = 0; i < count; i++) order[i] = i;
            Array.Sort(order, (x, y) => string.CompareOrdinal(keys[x], keys[y]));
            foreach (int i in order)
            {
                w.Write("w\t");
                w.Write(D(values[i]));
                w.Write('\t');
                w.Write(updates[i].ToString(CultureInfo.InvariantCulture));
                w.Write('\t');
                w.Write(Clean(keys[i]));
                w.Write('\n');
            }
            if (Net.Trained)
            {
                w.Write("n\t");
                w.Write(Net.ToBase64());
                w.Write('\n');
            }
        }

        /// <summary>Replaces everything with the contents of Serialize() text. Unreadable lines are skipped.</summary>
        public void LoadText(string text)
        {
            lock (sync)
            {
                weights.Clear();
                matchups.Clear();
                history.Clear();
                cardResults.Clear();
                deckSkills.Clear();
                habits.Clear();
                HabitReviews = 0;
                GamesTrained = GamesVsYou = YourDecisionsStudied = FeedbackGiven = 0;
                LogsStudied = LogDecisionsStudied = 0;
                Net.Reset();
                if (string.IsNullOrEmpty(text)) return;
                foreach (var raw in text.Split('\n'))
                {
                    var line = raw.TrimEnd('\r');
                    if (line.Length < 2 || line[0] == '#') continue;
                    var p = line.Split('\t');
                    try
                    {
                        switch (p[0])
                        {
                            case "c":
                                if (p.Length >= 5)
                                {
                                    GamesTrained = I(p[1]);
                                    GamesVsYou = I(p[2]);
                                    YourDecisionsStudied = I(p[3]);
                                    FeedbackGiven = I(p[4]);
                                    if (p.Length >= 7)
                                    {
                                        LogsStudied = I(p[5]);
                                        LogDecisionsStudied = I(p[6]);
                                    }
                                }
                                break;
                            case "w":
                                if (p.Length >= 4)
                                {
                                    var w = Get(p[3]);
                                    w.W = Clamp(Dp(p[1]), -MaxWeight, MaxWeight);
                                    w.N = I(p[2]);
                                }
                                break;
                            case "m":
                                if (p.Length >= 8)
                                {
                                    var m = Matchup(p[6], p[7]);
                                    m.Games = I(p[1]);
                                    m.Wins = I(p[2]);
                                    m.Losses = I(p[3]);
                                    m.Baseline = Dp(p[4]);
                                    m.ShortBaseline = Dp(p[5]);
                                }
                                break;
                            case "n":
                                if (p.Length >= 2) Net.FromBase64(p[1]);
                                break;
                            case "hr":
                                if (p.Length >= 2) HabitReviews = I(p[1]);
                                break;
                            case "h":
                                if (p.Length >= 8)
                                {
                                    habits[p[6]] = new PlayHabit
                                    {
                                        Games = I(p[1]), Times = I(p[2]), Strength = Dp(p[3]), Recent = Dp(p[4]), RecentBefore = Dp(p[4]),
                                        LastReview = I(p[5]), Key = p[6], Title = p[7]
                                    };
                                }
                                break;
                            case "k":
                                if (p.Length >= 9)
                                {
                                    var r = CardResultFor(p[7], p[8]);
                                    r.Games = I(p[1]);
                                    r.Wins = I(p[2]);
                                    r.Drawn = I(p[3]);
                                    r.DrawnWins = I(p[4]);
                                    r.Played = I(p[5]);
                                    r.PlayedWins = I(p[6]);
                                }
                                break;
                            case "q":
                                if (p.Length >= 7)
                                {
                                    var k = SkillFor(p[6]);
                                    k.Games = I(p[1]);
                                    k.Wins = I(p[2]);
                                    k.Losses = I(p[3]);
                                    k.Decisions = long.Parse(p[4], CultureInfo.InvariantCulture);
                                    k.Uncertainty = Dp(p[5]);
                                }
                                break;
                            case "p":
                                if (p.Length >= 2) Policy.FromBytes(Convert.FromBase64String(p[1].Trim()));
                                break;
                            case "s":
                                if (p.Length >= 10)
                                {
                                    history.Add(new SessionRecord
                                    {
                                        Games = I(p[1]), WinsA = I(p[2]), WinsB = I(p[3]),
                                        ScoreBefore = Dp(p[4]), ScoreAfter = Dp(p[5]), EvalGames = I(p[6]),
                                        When = p[7], DeckA = p[8], DeckB = p[9], Style = p.Length >= 11 ? p[10] : ""
                                    });
                                }
                                break;
                        }
                    }
                    catch (FormatException)
                    {
                        // Skip a damaged line rather than losing the whole brain.
                    }
                    catch (OverflowException)
                    {
                    }
                }
            }
        }

        public static LearningBrain Load(string path)
        {
            var brain = new LearningBrain();
            brain.FilePath = path;
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) brain.LoadText(File.ReadAllText(path, Encoding.UTF8));
            string neural = NeuralPath(path);
            if (neural != null && File.Exists(neural)) brain.Policy.FromBytes(File.ReadAllBytes(neural));
            return brain;
        }

        /// <summary>Writes the brain to FilePath (through a temporary file, so a crash can't leave half a brain).</summary>
        /// <summary>
        /// Saving is paused: another program is training this brain's file (the app's "Train in the background"), and the app
        /// reloads the file when it's done rather than writing over it.
        /// </summary>
        public bool SavesPaused;

        public void Save()
        {
            if (string.IsNullOrEmpty(FilePath) || SavesPaused) return;
            // v134.4: streamed to the file, not built as one big string first (that string and its copies were large enough
            // to set off long garbage collections, which freeze the whole app). Saves take turns.
            lock (fileLock)
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                string temp = FilePath + ".tmp";
                using (var writer = new StreamWriter(temp, false, new UTF8Encoding(false), 1 << 16)) WriteTo(writer);
                if (File.Exists(FilePath)) File.Delete(FilePath);
                File.Move(temp, FilePath);
                // The Neural AI's network, in its own file next to the brain's (never a damaged one: the last good file stays).
                string neural = NeuralPath(FilePath);
                if (Policy.AllFinite())
                {
                    File.WriteAllBytes(neural + ".tmp", Policy.ToBytes());
                    if (File.Exists(neural)) File.Delete(neural);
                    File.Move(neural + ".tmp", neural);
                }
            }
        }

        /// <summary>The file holding the Neural AI's network for the brain saved at path.</summary>
        public static string NeuralPath(string path)
        {
            return string.IsNullOrEmpty(path) ? null : path + ".neural";
        }

        private readonly object fileLock = new object();

        private static string D(double x)
        {
            return x.ToString("0.######", CultureInfo.InvariantCulture);
        }

        private static double Dp(string s)
        {
            return double.Parse(s, NumberStyles.Float, CultureInfo.InvariantCulture);
        }

        private static int I(string s)
        {
            return int.Parse(s, NumberStyles.Integer, CultureInfo.InvariantCulture);
        }

        private static string Clean(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }
    }
}
