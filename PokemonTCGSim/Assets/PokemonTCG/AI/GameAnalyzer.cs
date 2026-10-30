using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One point in the Areas of Improvement review.</summary>
    public sealed class ReviewItem
    {
        public const string BetterPlays = "Plays that would have given you a better chance";
        public const string Habits = "What you could improve";

        public string Section = Habits;
        public int Turn;
        public string Title = "";
        public string Detail = "";

        /// <summary>Estimated chance (0..1) to win after your play and after the suggested play; -1 when not simulated.</summary>
        public double YourChance = -1;
        public double SuggestedChance = -1;
        /// <summary>Simulated games behind each estimate.</summary>
        public int Simulations;

        /// <summary>The decision this item is about (null for items about the whole game).</summary>
        public RecordedDecision Decision;
        public int YourOption = -1;
        public int SuggestedOption = -1;
        public string YourPlay = "";
        public string SuggestedPlay = "";

        /// <summary>Board previews from your side (null when not available).</summary>
        public PlayerView Before;
        public PlayerView AfterYours;
        public PlayerView AfterSuggested;
        public PlayerView SuggestedTurnEnd;
        /// <summary>What you did that turn, and what the AI would have done from this point.</summary>
        public List<string> YourTurn = new List<string>();
        public List<string> SuggestedTurn = new List<string>();

        /// <summary>0 = no answer yet, 1 = "Makes sense", -1 = "I disagree".</summary>
        public int Feedback;
        public string Note = "";

        public bool CanPreview { get { return Before != null; } }
        public bool CanGiveFeedback { get { return Decision != null && YourOption >= 0 && SuggestedOption >= 0; } }
        public double Gain { get { return YourChance < 0 || SuggestedChance < 0 ? 0 : SuggestedChance - YourChance; } }
    }

    /// <summary>The finished review of one game.</summary>
    public sealed class GameReview
    {
        public string Headline = "";
        /// <summary>What went wrong (or right): the Prize race, Knock Outs, the turns that decided the game.</summary>
        public List<string> Summary = new List<string>();
        public List<ReviewItem> Items = new List<ReviewItem>();
        /// <summary>Decisions compared with the AI and how many differed (for the "how this was worked out" note).</summary>
        public int DecisionsCompared;
        public int Differences;
        public int Simulated;
        /// <summary>The AI that judged your plays and played the positions out: "Neural AI" once it has trained, else "Learning AI".</summary>
        public string Judge = "Learning AI";
        /// <summary>Your games the review has gone through so far, this one included (for the habits' history).</summary>
        public int HabitReviews;
    }

    /// <summary>
    /// Areas of Improvement: goes through your decisions after a game, finds the ones where the AI would have played
    /// differently, and plays both versions out many times from that point (with the cards you couldn't see shuffled
    /// each time) to estimate which gave the better chance to win. Adds a checklist of habits (Energy attachments,
    /// Supporters, Bench size) and a summary of how the game was won or lost.
    ///
    /// Run() is slow (seconds): call it on a background thread and poll Progress().
    /// </summary>
    public sealed class GameAnalyzer
    {
        /// <summary>Simulated games per play compared (each one is paired: same shuffle and coin flips for both plays).</summary>
        public int RolloutsPerOption = 20;
        /// <summary>Decisions to simulate at most.</summary>
        public int MaxCandidates = 6;
        /// <summary>Simulations stop after this many turns and score the position by Prize cards.</summary>
        public int RolloutTurnCap = 20;
        /// <summary>Report a better play only when it wins this much more often (0.08 = 8 percentage points).</summary>
        public double ReportThreshold = 0.08;
        /// <summary>
        /// The Neural AI counts your play as a different choice only when it likes its own at least this much more (in probability)
        /// and gives yours less than NeuralKeepsYours: plays it rates about as well aren't mistakes.
        /// </summary>
        public double NeuralGap = 0.25, NeuralKeepsYours = 0.25;
        /// <summary>Threads the simulations run on.</summary>
        public int Threads = Math.Max(1, Environment.ProcessorCount / 2);
        /// <summary>Your and your opponent's decklists (the engine's starting decks), for the Neural AI; read from the game when not set.</summary>
        public List<CardDef> YourDeckList, TheirDeckList;
        /// <summary>Keep this game's habits in the brain's profile of your play (off for a review that's only being checked).</summary>
        public bool RecordHabits = true;

        private readonly CardScriptRegistry scripts;
        private readonly LearningBrain brain;
        private readonly List<RecordedDecision> decisions;
        private readonly List<EventCollector.Entry> events;
        private readonly int seat;
        private readonly string yourDeck, theirDeck;
        private readonly int winner;
        private readonly string endReason;
        private readonly int finalTurn;
        private readonly int yourPrizesLeft, theirPrizesLeft;
        private readonly int prizeCount;
        /// <summary>The game's format rules (custom formats, v123).</summary>
        private readonly GameRules rules;

        private readonly object gate = new object();
        private string phase = "Waiting";
        private int done, total;
        private bool finished;
        private string error;
        private volatile bool cancel;
        private GameReview review;
        /// <summary>The Neural AI judges and plays the positions out (it has trained), rather than the Learning AI.</summary>
        private bool neural;
        /// <summary>Habits found in this game, before they're added up.</summary>
        private readonly List<HabitHit> hits = new List<HabitHit>();

        public GameAnalyzer(CardScriptRegistry scripts, LearningBrain brain, List<RecordedDecision> decisions, List<EventCollector.Entry> events,
            int seat, string yourDeck, string theirDeck, GameState finalState, int prizeCount, GameRules rules = null)
        {
            this.rules = rules ?? GameRules.Default;
            this.scripts = scripts;
            this.brain = brain;
            this.decisions = decisions ?? new List<RecordedDecision>();
            this.events = events ?? new List<EventCollector.Entry>();
            this.seat = seat;
            this.yourDeck = yourDeck ?? "";
            this.theirDeck = theirDeck ?? "";
            this.prizeCount = prizeCount <= 0 ? 6 : prizeCount;
            if (finalState != null)
            {
                winner = finalState.Winner;
                endReason = finalState.EndReason ?? "";
                finalTurn = finalState.TurnNumber;
                yourPrizesLeft = finalState.Players[seat].Prizes.Count;
                theirPrizesLeft = finalState.Players[1 - seat].Prizes.Count;
            }
            else
            {
                winner = -1;
                endReason = "";
            }
        }

        public void Cancel()
        {
            cancel = true;
        }

        public void Progress(out string phaseText, out int doneCount, out int totalCount, out bool isFinished, out string errorText)
        {
            lock (gate)
            {
                phaseText = phase;
                doneCount = done;
                totalCount = total;
                isFinished = finished;
                errorText = error;
            }
        }

        /// <summary>The review once Run() has finished (null before).</summary>
        public GameReview Review
        {
            get { lock (gate) return finished ? review : null; }
        }

        private void SetPhase(string text, int count)
        {
            lock (gate)
            {
                phase = text;
                done = 0;
                total = count;
            }
        }

        private void Tick()
        {
            lock (gate) done++;
        }

        public GameReview Run()
        {
            var result = new GameReview();
            try
            {
                SetPhase("Reading the game", 1);
                Summarize(result);
                var mains = decisions.Where(d => d.Request != null && d.Request.Kind == DecisionKind.MainAction && d.Snapshot != null && d.Answer != null && d.Answer.Count == 1).ToList();

                neural = brain != null && brain.Policy != null && brain.Policy.Trained;
                result.Judge = neural ? "Neural AI" : "AI";
                if (YourDeckList == null && mains.Count > 0) YourDeckList = CardsOf(mains[0].Snapshot, seat);
                if (TheirDeckList == null && mains.Count > 0) TheirDeckList = CardsOf(mains[0].Snapshot, 1 - seat);
                // What you knew at each decision, for the Neural AI: the game's public events up to that turn.
                var memory = new PolicyMemory(seat) { DeckList = YourDeckList };
                int nextEvent = 0;

                SetPhase("Comparing your plays with the " + result.Judge + "'s", mains.Count);
                var candidates = new List<Candidate>();
                var aiChoice = new Dictionary<RecordedDecision, Candidate>();
                foreach (var d in mains)
                {
                    if (cancel) break;
                    double[] probs = null;
                    if (neural)
                    {
                        while (nextEvent < events.Count && events[nextEvent].Turn < d.Turn)
                        {
                            memory.Turn = events[nextEvent].Turn;
                            memory.OnEvent(null, events[nextEvent].Event);
                            nextEvent++;
                        }
                        probs = NeuralChoices(d, memory);
                    }
                    var c = Compare(d, probs);
                    Tick();
                    if (c == null) continue;
                    result.DecisionsCompared++;
                    aiChoice[d] = c;
                    if (c.Suggested != c.Yours) result.Differences++;
                    if (c.Priority > 0) candidates.Add(c);
                }

                // The most important differences, at most two per turn.
                var picked = new List<Candidate>();
                foreach (var c in candidates.OrderByDescending(x => x.Priority).ThenByDescending(x => x.Decision.Turn))
                {
                    if (picked.Count >= MaxCandidates) break;
                    if (picked.Count(p => p.Decision.Turn == c.Decision.Turn) >= 2) continue;
                    picked.Add(c);
                }

                SetPhase("Simulating the key moments", picked.Count * RolloutsPerOption);
                var simulated = new HashSet<RecordedDecision>();
                foreach (var c in picked)
                {
                    if (cancel) break;
                    Simulate(c);
                    simulated.Add(c.Decision);
                    result.Simulated++;
                    if (c.SuggestedChance >= 0 && c.SuggestedChance - c.YourChance >= ReportThreshold)
                        result.Items.Add(BetterPlayItem(c));
                }

                SetPhase("Checking habits", 1);
                AddHabits(result, aiChoice, simulated);
                if (!cancel) AddHabitItems(result, aiChoice);

                SetPhase("Preparing previews", result.Items.Count);
                foreach (var item in result.Items)
                {
                    if (cancel) break;
                    BuildPreviews(item);
                    Tick();
                }
                result.Items = result.Items
                    .OrderBy(i => i.Section == ReviewItem.BetterPlays ? 0 : 1)
                    .ThenByDescending(i => i.Gain)
                    .ThenBy(i => i.Turn)
                    .ToList();
            }
            catch (Exception e)
            {
                lock (gate) error = e.GetType().Name + ": " + e.Message;
            }
            lock (gate)
            {
                review = result;
                finished = true;
                phase = cancel ? "Stopped" : "Done";
            }
            return result;
        }

        // ------------------------------------------------------------------ Summary: what went wrong

        private string Who(int player)
        {
            return player == seat ? "you" : "your opponent";
        }

        private void Summarize(GameReview r)
        {
            if (winner == seat) r.Headline = "You won on turn " + finalTurn + ".";
            else if (winner < 0) r.Headline = "The game ended without a winner on turn " + finalTurn + ".";
            else r.Headline = "You lost on turn " + finalTurn + ".";
            if (!string.IsNullOrEmpty(endReason)) r.Headline += " " + endReason.Replace("Opponent", "Your opponent");

            // Each player takes their own Prize cards, so what you took is what's missing from yours.
            int yourTaken = prizeCount - yourPrizesLeft;
            int theirTaken = prizeCount - theirPrizesLeft;
            r.Summary.Add("Prize cards: you took " + yourTaken + ", your opponent took " + theirTaken + ".");

            // Prize race from the events: who was ahead, and the turns that swung it.
            int mine = 0, theirs = 0;
            int lastAheadTurn = -1;
            bool wasAhead = false, fellBehind = false;
            var takenByTurn = new Dictionary<int, int>();
            var kos = new List<string>();
            foreach (var e in events)
            {
                var ev = e.Event;
                if (ev == null) continue;
                if (ev.Type == GameEventType.TakePrize && ev.Player >= 0)
                {
                    if (ev.Player == seat) mine++;
                    else
                    {
                        theirs++;
                        int n;
                        takenByTurn.TryGetValue(e.Turn, out n);
                        takenByTurn[e.Turn] = n + 1;
                    }
                    if (mine > theirs)
                    {
                        wasAhead = true;
                        lastAheadTurn = e.Turn;
                    }
                    else if (wasAhead && theirs > mine && !fellBehind)
                    {
                        fellBehind = true;
                    }
                }
                else if (ev.Type == GameEventType.KnockOut && ev.Player == seat)
                {
                    string name = ev.Card != null ? ev.Card.Name : "Pokémon";
                    kos.Add(name + " (turn " + e.Turn + ")");
                }
            }
            if (kos.Count > 0)
                r.Summary.Add("Your Pokémon Knocked Out: " + string.Join(", ", kos.ToArray()) + ".");
            if (takenByTurn.Count > 0)
            {
                var worst = takenByTurn.OrderByDescending(kv => kv.Value).ThenBy(kv => kv.Key).First();
                if (worst.Value >= 2)
                    r.Summary.Add("The turn that hurt most: turn " + worst.Key + ", when your opponent took " + worst.Value + " Prize cards in one turn.");
            }
            if (fellBehind && winner != seat)
                r.Summary.Add("You were ahead in the Prize race until turn " + lastAheadTurn + ", then your opponent pulled ahead.");
            if (endReason.Contains("couldn't draw"))
                r.Summary.Add(winner == seat ? "Your opponent ran out of cards." : "You ran out of cards in your deck: watch how many cards your draw Supporters and search cards use up late in the game.");
            if (endReason.Contains("no Pokémon") || endReason.Contains("no Benched"))
                r.Summary.Add(winner == seat ? "Your opponent ran out of Pokémon in play." : "You ran out of Pokémon in play: keep a Benched Pokémon ready to replace your Active.");
        }

        // ------------------------------------------------------------------ Comparing a decision with the AI

        private sealed class Candidate
        {
            public RecordedDecision Decision;
            public int Yours;
            public int Suggested;
            public double[] Adjusted;
            public int Priority;
            public string Title = "";
            public string Why = "";
            public double YourChance = -1;
            public double SuggestedChance = -1;
            public int Simulations;
            /// <summary>The Neural AI's probability of your play and of its own (-1 when the Learning AI judged).</summary>
            public double YourProb = -1, AiProb = -1;
            /// <summary>The Neural AI's probability of each option (null when the Learning AI judged).</summary>
            public double[] Probs;
        }

        /// <summary>
        /// How likely the Neural AI is to make each option at this decision (0..1, by option), or null when it can't say. An
        /// option the encoder merged with an identical one gets that one's probability.
        /// </summary>
        private double[] NeuralChoices(RecordedDecision d, PolicyMemory memory)
        {
            try
            {
                var r = d.Request;
                memory.See(r.View);
                var choices = PolicyEncoder.Choices(r, new List<int>());
                if (choices.Count < 2) return null;
                var x = PolicyEncoder.Encode(r, memory, brain.Policy, choices);
                float value;
                var logits = brain.Policy.Evaluate(x, out value);
                var p = PolicyNet.Softmax(logits);
                var probs = new double[r.Options.Count];
                var listed = new bool[r.Options.Count];
                for (int i = 0; i < choices.Count; i++)
                {
                    if (choices[i].Done || choices[i].Option < 0 || choices[i].Option >= probs.Length) continue;
                    probs[choices[i].Option] += p[i];
                    listed[choices[i].Option] = true;
                }
                for (int i = 0; i < probs.Length; i++)
                {
                    if (listed[i]) continue;
                    foreach (var c in choices)
                        if (!c.Done && listed[c.Option] && SamePlay(r.Options[i], r.Options[c.Option], r.View)) probs[i] = probs[c.Option];
                }
                return probs;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>Every card a player has anywhere in the game (their decklist, as far as the review is concerned).</summary>
        private static List<CardDef> CardsOf(GameState s, int player)
        {
            if (s == null) return null;
            var p = s.Players[player];
            var list = new List<CardDef>();
            foreach (var c in p.Deck) list.Add(c.Def);
            foreach (var c in p.Hand) list.Add(c.Def);
            foreach (var c in p.Prizes) list.Add(c.Def);
            foreach (var c in p.Discard) list.Add(c.Def);
            if (p.Active != null) foreach (var c in p.Active.AllCards()) list.Add(c.Def);
            foreach (var b in p.Bench) foreach (var c in b.AllCards()) list.Add(c.Def);
            return list;
        }

        private Candidate Compare(RecordedDecision d, double[] probs)
        {
            var r = d.Request;
            if (r.Options.Count < 2 || r.View == null) return null;
            int yours = d.Answer[0];
            if (yours < 0 || yours >= r.Options.Count) return null;
            List<List<string>> feats;
            double[] adjusted;
            List<int> best;
            try
            {
                best = LearningAgent.BestFor(brain, r, yourDeck, theirDeck, out feats, out adjusted);
            }
            catch (Exception)
            {
                return null;
            }
            if (best.Count == 0) return null;
            var c = new Candidate { Decision = d, Yours = yours, Suggested = best[0], Adjusted = adjusted };
            if (probs != null)
            {
                // The Neural AI's choice; your play stands when it rates it about as well.
                int top = 0;
                for (int i = 1; i < probs.Length; i++) if (probs[i] > probs[top]) top = i;
                c.YourProb = probs[yours];
                c.AiProb = probs[top];
                c.Probs = probs;
                c.Suggested = top;
                if (top == yours || probs[top] - probs[yours] < NeuralGap || probs[yours] >= NeuralKeepsYours) c.Suggested = yours;
            }
            if (c.Suggested == yours) return c;
            var y = r.Options[yours];
            var a = r.Options[c.Suggested];
            if (SamePlay(y, a, r.View)) return c;
            // Pure ordering: you made the AI's play later in the same turn anyway.
            bool setupBoth = adjusted[yours] >= 5 && adjusted[c.Suggested] >= 5 && y.Action != ActionType.Attack && a.Action != ActionType.Attack;
            if (setupBoth && DidLaterThisTurn(d, a)) return c;

            bool yAttack = y.Action == ActionType.Attack, aAttack = a.Action == ActionType.Attack;
            bool yEnd = y.Action == ActionType.EndTurn;
            bool aSetup = adjusted[c.Suggested] >= 5 && !aAttack;
            if (yEnd && aAttack)
            {
                c.Priority = 100;
                c.Title = "You ended your turn without attacking";
                c.Why = "You could have used " + a.Label.Replace("Attack: ", "") + ".";
            }
            else if (yAttack && aAttack)
            {
                c.Priority = 90;
                c.Title = "A different attack";
                c.Why = "You used " + y.Label.Replace("Attack: ", "") + "; the AI would have used " + a.Label.Replace("Attack: ", "") + ".";
            }
            else if (yAttack && aSetup)
            {
                c.Priority = 80;
                c.Title = "You attacked before finishing your turn";
                c.Why = "Attacking ends your turn. Before it, the AI would still have done: " + a.Label + ".";
            }
            else if (yEnd && aSetup)
            {
                c.Priority = 70;
                c.Title = "You ended your turn with a useful play left";
                c.Why = "The AI would still have done: " + a.Label + ".";
            }
            else if (y.Action == ActionType.Retreat || a.Action == ActionType.Retreat)
            {
                c.Priority = 60;
                c.Title = y.Action == ActionType.Retreat ? "Retreating" : "Not retreating";
                c.Why = "You chose: " + y.Label + ". The AI would have chosen: " + a.Label + ".";
            }
            else if (IsSupporter(y) || IsSupporter(a))
            {
                c.Priority = 55;
                c.Title = "Your Supporter for the turn";
                c.Why = "You chose: " + y.Label + ". The AI would have chosen: " + a.Label + ".";
            }
            else if (adjusted[c.Suggested] >= 5 && adjusted[yours] < 1)
            {
                c.Priority = 50;
                c.Title = "A play the AI would have held back";
                c.Why = "You chose: " + y.Label + ". The AI would rather: " + a.Label + ".";
            }
            else
            {
                c.Priority = 20;
                c.Title = "A different play";
                c.Why = "You chose: " + y.Label + ". The AI would have chosen: " + a.Label + ".";
            }
            if (c.AiProb >= 0)
            {
                // The surer the Neural AI is, the more the moment matters.
                c.Priority += (int)Math.Round(40 * (c.AiProb - c.YourProb));
                c.Why += " The Neural AI would make your play " + Percent(c.YourProb) + " of the time, its own " + Percent(c.AiProb) + ".";
            }
            return c;
        }

        private static bool IsSupporter(DecisionOption o)
        {
            return o.Action == ActionType.PlayTrainer && o.Card != null && o.Card.IsSupporter;
        }

        /// <summary>Two options that do the same thing (another copy of the same card on the same kind of target).</summary>
        private static bool SamePlay(DecisionOption x, DecisionOption y, PlayerView v)
        {
            if (x.Action != y.Action || x.Index != y.Index) return false;
            string cx = x.Card == null ? "" : x.Card.Name, cy = y.Card == null ? "" : y.Card.Name;
            if (cx != cy) return false;
            if (x.PokemonUid == y.PokemonUid) return true;
            var px = v.FindPokemon(x.PokemonUid);
            var py = v.FindPokemon(y.PokemonUid);
            if (px == null || py == null) return false;
            return px.Top.Def.Name == py.Top.Def.Name && px.DamageCounters == py.DamageCounters && px.Energy.Count == py.Energy.Count
                   && (v.Me.Active == null || (v.Me.Active.Uid != px.Uid && v.Me.Active.Uid != py.Uid));
        }

        private bool DidLaterThisTurn(RecordedDecision d, DecisionOption play)
        {
            foreach (var later in decisions)
            {
                if (later.Index <= d.Index || later.Turn != d.Turn) continue;
                var r = later.Request;
                if (r == null || r.Kind != DecisionKind.MainAction || later.Answer == null || later.Answer.Count != 1) continue;
                int i = later.Answer[0];
                if (i < 0 || i >= r.Options.Count) continue;
                if (SamePlay(r.Options[i], play, r.View) || r.Options[i].Label == play.Label) return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ Simulations

        private void Simulate(Candidate c)
        {
            var d = c.Decision;
            var yours = d.Request.Options[c.Yours];
            var suggested = d.Request.Options[c.Suggested];
            double sumYours = 0, sumSuggested = 0;
            int n = 0;
            int baseSeed = 7919 * (d.Index + 1) + finalTurn;
            var sums = new object();
            // Each play-out is independent (its own shuffle of the hidden cards), so they run on several threads.
            System.Threading.Tasks.Parallel.For(0, RolloutsPerOption, new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = Math.Max(1, Threads) }, k =>
            {
                if (cancel) return;
                int seed = baseSeed + k * 104729;
                var world = Determinize(d.Snapshot, seat, seed);
                double y = Rollout(world, yours, seed);
                double s = y < 0 ? -1 : Rollout(world, suggested, seed);
                Tick();
                if (y < 0 || s < 0) return;
                lock (sums)
                {
                    sumYours += y;
                    sumSuggested += s;
                    n++;
                }
            });
            if (n < Math.Max(4, RolloutsPerOption / 3)) return; // too many failed: no estimate
            c.YourChance = sumYours / n;
            c.SuggestedChance = sumSuggested / n;
            c.Simulations = n;
        }

        /// <summary>
        /// A copy of the position with everything you couldn't see shuffled: your deck and face-down Prize cards, and
        /// your opponent's hand, deck and face-down Prize cards (hand size and counts stay the same).
        /// </summary>
        public static GameState Determinize(GameState snapshot, int seat, int seed)
        {
            var s = snapshot.Clone();
            var rng = new Rng(seed);
            var me = s.Players[seat];
            var opp = s.Players[1 - seat];

            var pool = new List<CardInstance>(me.Deck);
            var mySlots = HiddenPrizeSlots(me);
            foreach (int i in mySlots) pool.Add(me.Prizes[i]);
            rng.Shuffle(pool);
            int k = 0;
            foreach (int i in mySlots) me.Prizes[i] = pool[k++];
            me.Deck = pool.GetRange(k, pool.Count - k);

            var other = new List<CardInstance>(opp.Hand);
            other.AddRange(opp.Deck);
            var oppSlots = HiddenPrizeSlots(opp);
            foreach (int i in oppSlots) other.Add(opp.Prizes[i]);
            rng.Shuffle(other);
            int hand = opp.Hand.Count;
            opp.Hand = other.GetRange(0, hand);
            k = hand;
            foreach (int i in oppSlots) opp.Prizes[i] = other[k++];
            opp.Deck = other.GetRange(k, other.Count - k);
            return s;
        }

        private static List<int> HiddenPrizeSlots(PlayerState p)
        {
            var slots = new List<int>();
            for (int i = 0; i < p.Prizes.Count; i++)
                if (!p.FaceUpPrizeIds.Contains(p.Prizes[i].Id)) slots.Add(i);
            return slots;
        }

        /// <summary>
        /// The AI for both sides of a play-out: the Neural AI (its best play every time; each side knows only its own decklist)
        /// once it has trained, else the Learning AI.
        /// </summary>
        private void Agents(int seed, out IPlayerAgent mine, out IPlayerAgent theirs)
        {
            if (neural)
            {
                mine = new PolicyAgent(brain.Policy, seed * 2 + 1, seat, YourDeckList) { Greedy = true, PreviewAttacks = false };
                theirs = new PolicyAgent(brain.Policy, seed * 2 + 2, 1 - seat, TheirDeckList) { Greedy = true, PreviewAttacks = false };
                return;
            }
            mine = new LearningAgent(brain, seed * 2 + 1, yourDeck, theirDeck) { Record = false };
            theirs = new LearningAgent(brain, seed * 2 + 2, theirDeck, yourDeck) { Record = false };
        }

        /// <summary>Plays the position out with the AI on both sides after the given first play. 1 = you win, 0 = you lose, -1 = failed.</summary>
        private double Rollout(GameState world, DecisionOption first, int seed)
        {
            var s = world.Clone();
            s.Rng = new Rng(seed); // the same coin flips and shuffles for both plays
            IPlayerAgent mine, theirs;
            Agents(seed, out mine, out theirs);
            var forced = new ForcedPlayAgent(mine, first, null);
            try
            {
                var engine = new GameEngine(scripts, seat == 0 ? (IPlayerAgent)forced : theirs, seat == 0 ? (IPlayerAgent)theirs : forced);
                // The wrapped agent watches the game too (the Neural AI remembers what it has seen).
                var watcher = mine as IGameObserver;
                if (watcher != null) engine.AddObserver(watcher);
                engine.CheckInvariants = false;
                engine.CardsPerPlayer = 0;
                engine.PrizeCount = prizeCount;
                engine.Rules = rules;
                engine.LoadState(s);
                engine.MaxTurns = s.TurnNumber + RolloutTurnCap;
                var result = engine.ResumeGame();
                if (!forced.ForcedPlayMade) return -1;
                if (result.Winner == seat) return 1;
                if (result.Winner >= 0) return 0;
                // Unfinished: judge by the Prize cards still to take.
                int diff = engine.State.Players[1 - seat].Prizes.Count - engine.State.Players[seat].Prizes.Count;
                return Math.Max(0.1, Math.Min(0.9, 0.5 + 0.09 * diff));
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private ReviewItem BetterPlayItem(Candidate c)
        {
            var d = c.Decision;
            var item = new ReviewItem
            {
                Section = ReviewItem.BetterPlays,
                Turn = d.Turn,
                Title = "Turn " + d.Turn + ": " + c.Title,
                Decision = d,
                YourOption = c.Yours,
                SuggestedOption = c.Suggested,
                YourPlay = d.Request.Options[c.Yours].Label,
                SuggestedPlay = d.Request.Options[c.Suggested].Label,
                YourChance = c.YourChance,
                SuggestedChance = c.SuggestedChance,
                Simulations = c.Simulations
            };
            item.Detail = c.Why + " Playing it out " + c.Simulations + " times from here (with the cards you couldn't see shuffled each time): "
                          + "your play won about " + Percent(c.YourChance) + ", the AI's play about " + Percent(c.SuggestedChance) + ".";
            item.YourTurn = TurnPlays(d.Turn);
            return item;
        }

        private static string Percent(double x)
        {
            return Math.Round(x * 100) + "%";
        }

        /// <summary>Labels of your main actions on that turn, in order.</summary>
        private List<string> TurnPlays(int turn)
        {
            var list = new List<string>();
            foreach (var d in decisions)
            {
                if (d.Turn != turn || d.Request == null || d.Request.Kind != DecisionKind.MainAction || d.Answer == null || d.Answer.Count != 1) continue;
                int i = d.Answer[0];
                if (i >= 0 && i < d.Request.Options.Count) list.Add(d.Request.Options[i].Label);
            }
            return list;
        }

        // ------------------------------------------------------------------ Habits

        /// <summary>One time a habit came up in this game.</summary>
        private sealed class HabitHit
        {
            public string Key = "";
            public string Title = "";
            public string Advice = "";
            public int Turn;
            public string Example = "";
            /// <summary>0..1: the chance to win it cost when played out (Measured), else how sure the AI was.</summary>
            public double Strength;
            public bool Measured;
            /// <summary>The Neural AI's probability of your play and of its own (-1 when not known).</summary>
            public double YourProb = -1, AiProb = -1;

            /// <summary>How much it counts in this game: a play-out's measured loss fully, the AI's preference alone half.</summary>
            public double Weight { get { return Measured ? Strength : 0.5 * Strength; } }
        }

        private void Hit(string key, string title, string advice, int turn, string example, double strength, bool measured, double yourProb = -1, double aiProb = -1)
        {
            if (hits.Any(h => h.Key == key && h.Turn == turn)) return; // once a turn
            hits.Add(new HabitHit
            {
                Key = key, Title = title, Advice = advice, Turn = turn, Example = example ?? "", Strength = strength, Measured = measured, YourProb = yourProb,
                AiProb = aiProb
            });
        }

        /// <summary>
        /// Finds this game's habits: every play the AI would have made differently (sorted into kinds, like "attacking before
        /// attaching an Energy" or "choosing Ultra Ball over Nest Ball"), plus Energy attachments and Supporters left unplayed
        /// and a thin Bench. AddHabitItems turns them into the review's notes, with your history of each.
        /// </summary>
        private void AddHabits(GameReview r, Dictionary<RecordedDecision, Candidate> ai, HashSet<RecordedDecision> simulated)
        {
            // Plays the AI would have made differently.
            foreach (var kv in ai.OrderBy(x => x.Key.Index))
            {
                var c = kv.Value;
                if (c.Priority <= 0 || c.Suggested == c.Yours) continue;
                var req = c.Decision.Request;
                string key, title, advice;
                Classify(req.Options[c.Yours], req.Options[c.Suggested], out key, out title, out advice);
                bool measured = c.SuggestedChance >= 0 && c.YourChance >= 0;
                double strength = measured ? Math.Max(0, c.SuggestedChance - c.YourChance) : c.AiProb >= 0 ? c.AiProb - c.YourProb : 0.4;
                Hit(key, title, advice, c.Decision.Turn, "you: " + req.Options[c.Yours].Label + "; the AI: " + req.Options[c.Suggested].Label, strength, measured,
                    c.YourProb, c.AiProb);
            }

            // The last main-action decision of each of your turns (the moment you ended the turn or attacked).
            var lastOfTurn = new Dictionary<int, RecordedDecision>();
            foreach (var d in decisions)
            {
                if (d.Request == null || d.Request.Kind != DecisionKind.MainAction || d.Answer == null || d.Answer.Count != 1) continue;
                lastOfTurn[d.Turn] = d;
            }
            foreach (var kv in lastOfTurn.OrderBy(x => x.Key))
            {
                var d = kv.Value;
                var req = d.Request;
                int pick = d.Answer[0];
                if (pick < 0 || pick >= req.Options.Count) continue;
                var chosen = req.Options[pick];
                if (chosen.Action != ActionType.EndTurn && chosen.Action != ActionType.Attack) continue;
                Candidate c;
                ai.TryGetValue(d, out c);
                double[] adjusted = c != null ? c.Adjusted : null;
                double[] probs = c != null ? c.Probs : null;
                string before = chosen.Action == ActionType.Attack ? "attacked" : "ended the turn";

                // Energy: an attachment was still available when the turn ended (the option only exists until you attach).
                int attach = BestIndex(req, adjusted, o => o.Action == ActionType.AttachEnergy);
                if (attach >= 0 && (probs != null ? probs[attach] >= 0.2 && probs[attach] > probs[pick] : adjusted == null || adjusted[attach] >= 5))
                    Hit(chosen.Action == ActionType.Attack ? "attack>energy" : "end>energy", chosen.Action == ActionType.Attack ? "Attacking before you attach an Energy" : "Ending your turn without attaching an Energy",
                        EnergyAdvice, kv.Key, "you " + before + " with " + req.Options[attach].Label + " still possible", probs != null ? probs[attach] : 0.3, false,
                        probs != null ? probs[pick] : -1, probs != null ? probs[attach] : -1);

                // Supporter: one was playable and the AI would have played it.
                int supporter = BestIndex(req, adjusted, IsSupporter);
                if (supporter >= 0 && (probs != null ? probs[supporter] >= 0.2 && probs[supporter] > probs[pick] : adjusted != null && adjusted[supporter] >= 5))
                    Hit(chosen.Action == ActionType.Attack ? "attack>supporter" : "end>supporter", chosen.Action == ActionType.Attack ? "Attacking before you play your Supporter" : "Ending your turn without playing a Supporter",
                        SupporterAdvice, kv.Key, "you " + before + " holding " + req.Options[supporter].Card.Name, probs != null ? probs[supporter] : 0.3, false,
                        probs != null ? probs[pick] : -1, probs != null ? probs[supporter] : -1);

                // Bench: one or no Benched Pokémon while a Basic could have gone down.
                var v = req.View;
                if (v != null && v.Me.Bench.Count <= 1 && req.Options.Any(o => o.Action == ActionType.BenchBasic && o.Card != null && o.Card.IsPokemon))
                    Hit("thin-bench", "A thin Bench", BenchAdvice, kv.Key, (v.Me.Bench.Count == 0 ? "no" : "one") + " Benched Pokémon with a Basic you could bench", 0.2, false);
            }
        }

        private const string EnergyAdvice = "One Energy attachment a turn is free tempo: even one on a Benched Pokémon gets your next attacker ready sooner.";
        private const string SupporterAdvice = "A Supporter every turn keeps your hand and board moving, unless you're holding it for a reason (a Boss's Orders for the right moment, not wanting to discard your hand).";
        private const string BenchAdvice = "With one or no Benched Pokémon, a Knock Out on your Active can end the game on the spot, and you have nothing ready to attack next.";

        /// <summary>The kind of play an option is, for sorting habits: "attack", "end", "energy", "supporter", "item"...</summary>
        private static string KindOf(DecisionOption o)
        {
            switch (o.Action)
            {
                case ActionType.Attack: return "attack";
                case ActionType.EndTurn: return "end";
                case ActionType.Retreat: return "retreat";
                case ActionType.AttachEnergy: return "energy";
                case ActionType.BenchBasic: return "bench";
                case ActionType.Evolve: return "evolve";
                case ActionType.UseAbility: return "ability";
                case ActionType.PlayTrainer:
                    if (o.Card == null) return "trainer";
                    if (o.Card.IsSupporter) return "supporter";
                    if (o.Card.IsItem) return o.Card.IsTool ? "tool" : "item";
                    if (o.Card.IsStadium) return "stadium";
                    return "trainer";
                default: return o.Action.ToString().ToLowerInvariant();
            }
        }

        /// <summary>"attach an Energy" (after "would").</summary>
        private static string Do(string kind)
        {
            switch (kind)
            {
                case "attack": return "attack";
                case "end": return "end the turn";
                case "retreat": return "retreat";
                case "energy": return "attach an Energy";
                case "bench": return "bench a Basic";
                case "evolve": return "evolve";
                case "ability": return "use an Ability";
                case "supporter": return "play your Supporter";
                case "item": return "play an Item";
                case "tool": return "attach a Tool";
                case "stadium": return "play a Stadium";
                default: return "make another play";
            }
        }

        /// <summary>"Attaching an Energy" (to start a title).</summary>
        private static string Doing(string kind)
        {
            switch (kind)
            {
                case "attack": return "Attacking";
                case "end": return "Ending your turn";
                case "retreat": return "Retreating";
                case "energy": return "Attaching an Energy";
                case "bench": return "Benching a Basic";
                case "evolve": return "Evolving";
                case "ability": return "Using an Ability";
                case "supporter": return "Playing your Supporter";
                case "item": return "Playing an Item";
                case "tool": return "Attaching a Tool";
                case "stadium": return "Playing a Stadium";
                default: return "That play";
            }
        }

        /// <summary>Sorts a play the AI would have made differently into a habit: its key, title and a line of advice.</summary>
        private static void Classify(DecisionOption yours, DecisionOption ai, out string key, out string title, out string advice)
        {
            string ky = KindOf(yours), ka = KindOf(ai);
            if (ky == ka)
            {
                string cy = yours.Card != null ? yours.Card.Name : "", ca = ai.Card != null ? ai.Card.Name : "";
                if (cy.Length > 0 && ca.Length > 0 && cy != ca)
                {
                    key = "card:" + ky + ":" + cy + ">" + ca;
                    title = "Choosing " + cy + " over " + ca;
                    advice = "In spots like this the AI would rather use " + ca + ".";
                    return;
                }
                key = "target:" + ky;
                switch (ky)
                {
                    case "energy": title = "Where you attach your Energy"; break;
                    case "attack": title = "Which attack you use"; break;
                    case "retreat": title = "Which Pokémon you switch in"; break;
                    case "evolve": title = "Which Pokémon you evolve"; break;
                    case "ability": title = "Which Ability you use"; break;
                    case "tool": title = "Where you attach Tools"; break;
                    case "bench": title = "Which Pokémon you bench"; break;
                    default: title = "Which target you pick for " + Doing(ky).ToLowerInvariant(); break;
                }
                advice = "The same kind of play, but the AI would pick differently: think about which Pokémon needs it most over the next two turns.";
                return;
            }
            key = ky + ">" + ka;
            // Any more setting up when the AI would attack straight away is one habit, whatever the play was.
            if (ka == "attack" && ky != "end" && ky != "retreat")
            {
                key = "more>attack";
                title = "Playing on when the AI would attack right away";
                advice = "The AI sees nothing more worth doing first: every extra play uses up cards or shows your hand for nothing, so attack now.";
                return;
            }
            if (ky == "end") title = ka == "attack" ? "Ending your turn without attacking" : "Ending your turn when the AI would " + Do(ka);
            else if (ky == "attack") title = "Attacking before you " + Do(ka);
            else if (ka == "attack") title = Doing(ky) + " when the AI would attack right away";
            else if (ka == "end") title = Doing(ky) + " when the AI would end the turn";
            else if (ka == "retreat") title = Doing(ky) + " when the AI would retreat first";
            else title = Doing(ky) + " before you " + Do(ka);

            if (key == "end>attack") advice = "Unless attacking would hurt you, its damage and effects move the Prize race forward.";
            else if (ky == "attack") advice = "Attacking ends your turn, so do everything else that helps first.";
            else if (key == "end>energy") advice = EnergyAdvice;
            else if (key == "end>supporter") advice = SupporterAdvice;
            else if (ky == "end") advice = "Ending the turn early gives up a play that would have helped.";
            else if (key == "supporter>item") advice = "Items first is usually better: they can find what the Supporter needs, and a Supporter that shuffles or discards your hand would throw them away.";
            else if (key == "item>supporter") advice = "Here the AI would play the Supporter first, so its draw can find better targets for your Items.";
            else if (ka == "retreat") advice = "The AI would switch your Active Pokémon first: to save it from a Knock Out, or to attack with a better one.";
            else if (ky == "retreat") advice = "Retreating costs Energy and tempo; the AI would keep your Active Pokémon in and " + Do(ka) + ".";
            else if (ka == "bench") advice = BenchAdvice;
            else if (ka == "attack") advice = "The AI sees nothing more worth doing first: attack now.";
            else if (ka == "end") advice = "The AI would stop here: that play costs you more than it gains (a card you'll want later, or a risk).";
            else advice = "In spots like this the AI would " + Do(ka) + " first.";
        }

        /// <summary>
        /// Adds this game's habits to your profile (in the brain) and makes the review's notes about them: when they came up
        /// this game, what to do instead, and your history (how many of your reviewed games, more or less often lately).
        /// </summary>
        private void AddHabitItems(GameReview r, Dictionary<RecordedDecision, Candidate> ai)
        {
            var byKey = hits.GroupBy(h => h.Key).ToList();
            var found = byKey.Select(g => new HabitInGame { Key = g.Key, Title = g.First().Title, Times = g.Count(), Strength = g.Average(h => h.Strength) }).ToList();
            List<PlayHabit> history;
            if (RecordHabits && brain != null)
            {
                history = brain.RecordHabits(found);
                r.HabitReviews = brain.HabitReviews;
            }
            else
            {
                history = found.Select(f => new PlayHabit { Key = f.Key, Title = f.Title, Games = 1, Times = f.Times, Strength = f.Strength * f.Times, Recent = LearningBrain.HabitRecency }).ToList();
                r.HabitReviews = brain != null ? brain.HabitReviews + 1 : 1;
            }
            int reviews = Math.Max(1, r.HabitReviews);
            // The ones that matter most this game first: how much each cost, times how often it came up. A habit whose plays were
            // played out and made no real difference (under 2 points of chance to win) isn't worth a note: the measurement beats
            // the AI's preference.
            var ranked = byKey.Where(g => !g.Any(h => h.Measured) || g.Where(h => h.Measured).Average(h => h.Strength) >= 0.02)
                              .OrderByDescending(g => g.Sum(h => h.Weight)).ThenBy(g => g.Min(h => h.Turn)).Take(5).ToList();
            foreach (var g in ranked)
            {
                var list = g.OrderBy(h => h.Turn).ToList();
                var first = list[0];
                var h = history.FirstOrDefault(x => x.Key == g.Key) ?? new PlayHabit { Key = g.Key, Games = 1 };
                var sb = new StringBuilder();
                sb.Append("This game: turn ").Append(first.Turn).Append(" (").Append(first.Example).Append(")");
                var more = list.Skip(1).Select(x => x.Turn.ToString(CultureInfo.InvariantCulture)).Distinct().ToList();
                if (more.Count > 0) sb.Append(", and turn").Append(more.Count == 1 ? " " : "s ").Append(Join(more));
                sb.Append(". ").Append(first.Advice).Append(' ');
                if (h.Games <= 1) sb.Append("The first time the review has seen this from you.");
                else
                {
                    sb.Append("It has come up in ").Append(h.Games).Append(" of your ").Append(reviews).Append(" reviewed games");
                    double usual = (h.Games - 1) / (double)Math.Max(1, reviews - 1);
                    if (reviews >= 5 && h.RecentBefore < usual * 0.7) sb.Append(", less often lately than it used to: you're improving here");
                    else if (reviews >= 5 && h.RecentBefore > usual * 1.3) sb.Append(", more often lately");
                    sb.Append('.');
                }
                var measured = list.Where(x => x.Measured).ToList();
                var rated = list.FirstOrDefault(x => x.AiProb >= 0 && x.YourProb >= 0);
                if (measured.Count > 0)
                    sb.Append(" Played out from the position, it cost about ").Append(Percent(measured.Average(x => x.Strength))).Append(" chance to win.");
                else if (rated != null)
                    sb.Append(" On turn ").Append(rated.Turn).Append(" the Neural AI would have made your play ").Append(Percent(rated.YourProb))
                      .Append(" of the time and its own ").Append(Percent(rated.AiProb)).Append('.');
                r.Items.Add(new ReviewItem { Section = ReviewItem.Habits, Turn = first.Turn, Title = first.Title, Detail = sb.ToString() });
            }
        }

        private static int BestIndex(DecisionRequest r, double[] adjusted, Func<DecisionOption, bool> match)
        {
            int best = -1;
            for (int i = 0; i < r.Options.Count; i++)
            {
                if (!match(r.Options[i])) continue;
                if (best < 0 || (adjusted != null && adjusted[i] > adjusted[best])) best = i;
            }
            return best;
        }

        private static int FirstTurn(List<string> entries)
        {
            int turn;
            if (entries.Count == 0) return 0;
            var first = entries[0];
            int space = first.IndexOf(' ');
            return int.TryParse(space > 0 ? first.Substring(0, space) : first, out turn) ? turn : 0;
        }

        private static string Join(List<string> items)
        {
            if (items.Count == 1) return items[0];
            if (items.Count == 2) return items[0] + " and " + items[1];
            return string.Join(", ", items.Take(items.Count - 1).ToArray()) + " and " + items[items.Count - 1];
        }

        // ------------------------------------------------------------------ Previews

        private void BuildPreviews(ReviewItem item)
        {
            var d = item.Decision;
            if (d == null || d.Snapshot == null) return;
            try
            {
                item.Before = ViewOf(d.Snapshot);
                // Your play: the next time you chose an action that turn shows exactly what happened. Otherwise (you
                // attacked or ended the turn) replay it with your recorded follow-up choices.
                var next = decisions.FirstOrDefault(x => x.Index > d.Index && x.Request != null && x.Request.Kind == DecisionKind.MainAction && x.Snapshot != null);
                if (next != null && next.Turn == d.Turn && d.Request.Options[item.YourOption].Action != ActionType.Attack)
                {
                    item.AfterYours = ViewOf(next.Snapshot);
                }
                else
                {
                    List<string> ignore = new List<string>();
                    PlayerView unused;
                    item.AfterYours = Play(d.Snapshot, d.Request.Options[item.YourOption], ReplayAfter(d), false, ignore, out unused);
                }
                PlayerView afterFirst;
                item.SuggestedTurnEnd = Play(d.Snapshot, d.Request.Options[item.SuggestedOption], null, true, item.SuggestedTurn, out afterFirst);
                item.AfterSuggested = afterFirst;
            }
            catch (Exception)
            {
                // Previews are a bonus; the item still stands without them.
            }
        }

        private PlayerView ViewOf(GameState snapshot)
        {
            var engine = NewEngine(new HeuristicAgent(1), new HeuristicAgent(2));
            engine.LoadState(snapshot.Clone());
            return PlayerView.Create(engine, seat);
        }

        private GameEngine NewEngine(IPlayerAgent mine, IPlayerAgent theirs)
        {
            var engine = new GameEngine(scripts, seat == 0 ? mine : theirs, seat == 0 ? theirs : mine);
            engine.CheckInvariants = false;
            engine.CardsPerPlayer = 0;
            engine.PrizeCount = prizeCount;
            engine.Rules = rules;
            engine.PlayerNames = seat == 0 ? new[] { "You", "Opponent" } : new[] { "Opponent", "You" };
            return engine;
        }

        /// <summary>Your recorded answers right after a main action (cards picked for a search, the Pokémon you switched in...).</summary>
        private Queue<RecordedDecision> ReplayAfter(RecordedDecision d)
        {
            var q = new Queue<RecordedDecision>();
            foreach (var later in decisions)
            {
                if (later.Index <= d.Index) continue;
                if (later.Request == null || later.Request.Kind == DecisionKind.MainAction) break;
                // Which face-down Prize card you took: replays use bot seats, which aren't asked (they take the first one).
                if (later.Request.Kind == DecisionKind.ChoosePrize) continue;
                q.Enqueue(later);
            }
            return q;
        }

        /// <summary>
        /// Makes one play from the real position (the real order of the deck: this shows what would actually have
        /// happened), optionally finishing the turn with the AI. Returns the board at the end; afterFirst is the board
        /// right after the first play.
        /// </summary>
        private PlayerView Play(GameState snapshot, DecisionOption first, Queue<RecordedDecision> replay, bool finishTurn, List<string> log, out PlayerView afterFirst)
        {
            IPlayerAgent mine, theirs;
            Agents(5, out mine, out theirs);
            var forced = new ForcedPlayAgent(mine, first, replay);
            var engine = NewEngine(forced, theirs);
            var watcher = mine as IGameObserver;
            if (watcher != null) engine.AddObserver(watcher);
            var collector = new TextLog(log);
            engine.AddObserver(collector);
            engine.LoadState(snapshot.Clone());
            afterFirst = null;
            try
            {
                bool over = engine.MainPhaseStep();
                afterFirst = PlayerView.Create(engine, seat);
                int guard = 0;
                while (finishTurn && !over && guard++ < 60) over = engine.MainPhaseStep();
            }
            catch (GameOverException)
            {
            }
            if (afterFirst == null) afterFirst = PlayerView.Create(engine, seat);
            return PlayerView.Create(engine, seat);
        }

        private sealed class TextLog : IGameObserver
        {
            private readonly List<string> lines;

            public TextLog(List<string> lines)
            {
                this.lines = lines;
            }

            public void OnEvent(GameEngine engine, GameEvent ev)
            {
                if (lines == null || ev == null || string.IsNullOrEmpty(ev.Text) || lines.Count >= 30) return;
                switch (ev.Type)
                {
                    case GameEventType.Shuffle:
                    case GameEventType.Search:
                    case GameEventType.TurnEnd:
                        return;
                }
                lines.Add(ev.Text);
            }
        }

        // ------------------------------------------------------------------ Feedback

        /// <summary>
        /// "I disagree": teach the AI to prefer your play in spots like this one (it keeps adjusting, within limits,
        /// until it would make your play there). "Makes sense": a small nudge toward the suggestion.
        /// </summary>
        public static void ApplyFeedback(LearningBrain brain, ReviewItem item, string yourDeck, string theirDeck, bool disagree)
        {
            if (brain == null || item == null || !item.CanGiveFeedback) return;
            brain.NoteFeedback();
            var r = item.Decision.Request;
            int preferred = disagree ? item.YourOption : item.SuggestedOption;
            int other = disagree ? item.SuggestedOption : item.YourOption;
            List<List<string>> feats;
            double[] adjusted;
            var best = LearningAgent.BestFor(brain, r, yourDeck, theirDeck, out feats, out adjusted);
            if (!disagree)
            {
                brain.ApplyPreference(feats[preferred], feats[other], 0.5);
                return;
            }
            for (int step = 0; step < 6; step++)
            {
                if (best.Count > 0 && best[0] == preferred) break;
                brain.ApplyPreference(feats[preferred], feats[other], 1.0);
                best = LearningAgent.BestFor(brain, r, yourDeck, theirDeck, out feats, out adjusted);
            }
        }
    }

    /// <summary>
    /// Plays a forced first main action (matched among the options the engine offers), then your recorded follow-up
    /// answers while they still fit, then whatever the inner agent decides.
    /// </summary>
    public sealed class ForcedPlayAgent : IPlayerAgent
    {
        private readonly IPlayerAgent inner;
        private readonly DecisionOption forced;
        private readonly Queue<RecordedDecision> replay;
        private bool forcedDone;

        /// <summary>The forced play was found among the options and made.</summary>
        public bool ForcedPlayMade { get; private set; }

        public ForcedPlayAgent(IPlayerAgent inner, DecisionOption forced, Queue<RecordedDecision> replay)
        {
            this.inner = inner;
            this.forced = forced;
            this.replay = replay;
        }

        public List<int> Decide(DecisionRequest r)
        {
            if (!forcedDone && r.Kind == DecisionKind.MainAction)
            {
                forcedDone = true;
                int i = Match(r.Options, forced);
                if (i >= 0)
                {
                    ForcedPlayMade = true;
                    return new List<int> { i };
                }
            }
            else if (forcedDone && replay != null)
            {
                // A person is asked even when there's only one possible answer; a bot seat isn't. Skip those.
                while (replay.Count > 0 && !Fits(replay.Peek(), r) && OnlyOneAnswer(replay.Peek().Request)) replay.Dequeue();
                if (replay.Count > 0)
                {
                    var next = replay.Dequeue();
                    if (Fits(next, r))
                    {
                        var answer = next.Answer.Where(x => x >= 0 && x < r.Options.Count).Distinct().ToList();
                        if (answer.Count >= r.MinChoices && answer.Count <= r.MaxChoices) return answer;
                    }
                    else replay.Clear(); // the game went differently; stop replaying
                }
            }
            return inner.Decide(r);
        }

        private static bool Fits(RecordedDecision d, DecisionRequest r)
        {
            return d.Request != null && d.Answer != null && d.Request.Kind == r.Kind && d.Request.Options.Count == r.Options.Count;
        }

        private static bool OnlyOneAnswer(DecisionRequest r)
        {
            return r == null || r.Options.Count <= 1 || r.MinChoices >= r.Options.Count;
        }

        /// <summary>The option that makes the same play (same action, card and target), or -1.</summary>
        public static int Match(List<DecisionOption> options, DecisionOption target)
        {
            if (target == null) return -1;
            for (int i = 0; i < options.Count; i++)
            {
                var o = options[i];
                if (o.Action != target.Action || o.PokemonUid != target.PokemonUid || o.Index != target.Index) continue;
                if (o.CardId == target.CardId || o.UsesCard(target.CardId) || target.UsesCard(o.CardId)) return i;
            }
            for (int i = 0; i < options.Count; i++)
                if (options[i].Action == target.Action && options[i].Label == target.Label) return i;
            return -1;
        }
    }
}
