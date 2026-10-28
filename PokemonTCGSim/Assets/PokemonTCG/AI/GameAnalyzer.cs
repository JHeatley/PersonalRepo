using System;
using System.Collections.Generic;
using System.Linq;
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

                SetPhase("Comparing your plays with the AI's", mains.Count);
                var candidates = new List<Candidate>();
                var aiChoice = new Dictionary<RecordedDecision, Candidate>();
                foreach (var d in mains)
                {
                    if (cancel) break;
                    var c = Compare(d);
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
        }

        private Candidate Compare(RecordedDecision d)
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
            for (int k = 0; k < RolloutsPerOption && !cancel; k++)
            {
                int seed = baseSeed + k * 104729;
                var world = Determinize(d.Snapshot, seat, seed);
                double y = Rollout(world, yours, seed);
                double s = y < 0 ? -1 : Rollout(world, suggested, seed);
                Tick();
                if (y < 0 || s < 0) continue;
                sumYours += y;
                sumSuggested += s;
                n++;
            }
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

        /// <summary>Plays the position out with the AI on both sides after the given first play. 1 = you win, 0 = you lose, -1 = failed.</summary>
        private double Rollout(GameState world, DecisionOption first, int seed)
        {
            var s = world.Clone();
            s.Rng = new Rng(seed); // the same coin flips and shuffles for both plays
            var mine = new LearningAgent(brain, seed * 2 + 1, yourDeck, theirDeck) { Record = false };
            var theirs = new LearningAgent(brain, seed * 2 + 2, theirDeck, yourDeck) { Record = false };
            var forced = new ForcedPlayAgent(mine, first, null);
            try
            {
                var engine = new GameEngine(scripts, seat == 0 ? (IPlayerAgent)forced : theirs, seat == 0 ? (IPlayerAgent)theirs : forced);
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

        private void AddHabits(GameReview r, Dictionary<RecordedDecision, Candidate> ai, HashSet<RecordedDecision> simulated)
        {
            // The last main-action decision of each of your turns (the moment you ended the turn or attacked).
            var lastOfTurn = new Dictionary<int, RecordedDecision>();
            foreach (var d in decisions)
            {
                if (d.Request == null || d.Request.Kind != DecisionKind.MainAction || d.Answer == null || d.Answer.Count != 1) continue;
                lastOfTurn[d.Turn] = d;
            }

            var noEnergy = new List<string>();
            var noSupporter = new List<string>();
            var thinBench = new List<int>();
            var noAttack = new List<string>();
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

                // Energy: an attachment was still available when the turn ended (the option only exists until you attach).
                int attach = BestIndex(req, adjusted, o => o.Action == ActionType.AttachEnergy);
                if (attach >= 0 && (adjusted == null || adjusted[attach] >= 5))
                    noEnergy.Add(kv.Key + " (" + req.Options[attach].Label + ")");

                // Supporter: one was playable and the AI would have played it.
                int supporter = BestIndex(req, adjusted, IsSupporter);
                if (supporter >= 0 && adjusted != null && adjusted[supporter] >= 5)
                    noSupporter.Add(kv.Key + " (" + req.Options[supporter].Card.Name + ")");

                // Bench: one or no Benched Pokémon while a Basic could have gone down.
                var v = req.View;
                if (v != null && v.Me.Bench.Count <= 1 && req.Options.Any(o => o.Action == ActionType.BenchBasic && o.Card != null && o.Card.IsPokemon))
                    thinBench.Add(kv.Key);

                // Ending without attacking, when the AI would have attacked (and it wasn't simulated above).
                if (chosen.Action == ActionType.EndTurn && !simulated.Contains(d) && c != null && c.Suggested >= 0
                    && req.Options[c.Suggested].Action == ActionType.Attack)
                    noAttack.Add(kv.Key + " (" + req.Options[c.Suggested].Label.Replace("Attack: ", "") + ")");
            }

            if (noEnergy.Count > 0)
                r.Items.Add(new ReviewItem
                {
                    Title = "Missed Energy attachments",
                    Turn = FirstTurn(noEnergy),
                    Detail = "You ended the turn without attaching an Energy on turn " + Join(noEnergy) + ". One attachment a turn is free tempo: even an Energy on a Benched Pokémon gets your next attacker ready sooner."
                });
            if (noSupporter.Count > 0)
                r.Items.Add(new ReviewItem
                {
                    Title = "Turns without a Supporter",
                    Turn = FirstTurn(noSupporter),
                    Detail = "You had a Supporter you could play but didn't on turn " + Join(noSupporter) + ". Unless you're holding it for a reason (saving a Boss's Orders for the right moment, not wanting to discard your hand), playing your Supporter every turn keeps your hand and board moving."
                });
            if (thinBench.Count > 0)
                r.Items.Add(new ReviewItem
                {
                    Title = "A thin Bench",
                    Turn = thinBench[0],
                    Detail = "On turn " + Join(thinBench.Select(t => t.ToString()).ToList()) + " you ended with one or no Benched Pokémon while you had a Basic Pokémon you could bench. If your Active Pokémon is Knocked Out with nothing to promote, you lose on the spot, and a thin Bench also limits your next attacker."
                });
            if (noAttack.Count > 0)
                r.Items.Add(new ReviewItem
                {
                    Title = "Turns without an attack",
                    Turn = FirstTurn(noAttack),
                    Detail = "You ended your turn without attacking on turn " + Join(noAttack) + ", although the AI would have attacked. Chip damage adds up, and many attacks also have useful effects."
                });
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
            var mine = new LearningAgent(brain, 11, yourDeck, theirDeck) { Record = false };
            var theirs = new LearningAgent(brain, 12, theirDeck, yourDeck) { Record = false };
            var forced = new ForcedPlayAgent(mine, first, replay);
            var engine = NewEngine(forced, theirs);
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
