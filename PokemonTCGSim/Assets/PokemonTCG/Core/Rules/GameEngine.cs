using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>Thrown internally to stop the game the moment a win condition is met.</summary>
    public sealed class GameOverException : Exception
    {
        public GameOverException() : base("Game over")
        {
        }
    }

    /// <summary>
    /// The rules engine. Pure C#, no Unity. It owns the GameState, asks agents for every decision,
    /// and reports everything that happens through IGameObserver events.
    ///
    /// The class is split over several files:
    ///   GameEngine.cs            setup, turn loop, decisions, events, scenario helpers (this file)
    ///   GameEngine.Actions.cs    legal main actions and how each one is carried out
    ///   GameEngine.Queries.cs    computed values: HP, costs, Energy, blocks and preventions from every source
    ///   GameEngine.Combat.cs     damage, damage counters, Knock Outs, Prizes, winning
    ///   GameEngine.Primitives.cs the building blocks card scripts use (draw, search, switch, attach...)
    /// </summary>
    public sealed partial class GameEngine
    {
        public GameState State { get; private set; }
        public readonly CardScriptRegistry Scripts;
        private readonly IPlayerAgent[] agents = new IPlayerAgent[2];
        private readonly List<IGameObserver> observers = new List<IGameObserver>();
        private int nextCardId = 1;

        public int MaxTurns = 400;
        public int MaxActionsPerTurn = 300;
        public int PrizeCount = 6;
        public int OpeningHandSize = 7;
        /// <summary>The format's rules (custom formats, v123): first-turn attacks and Supporters, the opening order, Tools and old Trainers.</summary>
        public GameRules Rules = GameRules.Default;
        /// <summary>Checks card conservation and board sanity after every action. Use in tests and simulations.</summary>
        public bool CheckInvariants;
        public int CardsPerPlayer = 60;
        public string[] PlayerNames = { "Player 1", "Player 2" };
        /// <summary>
        /// Each seat's birthday as month * 100 + day (1225 = December 25), or 0 for none (bots). When the game starts, a player
        /// whose birthday is Today has PlayerState.BirthdayToday set (_____'s Pikachu's Birthday Surprise, v48).
        /// </summary>
        public int[] Birthdays = { 0, 0 };
        /// <summary>The date the game is played on (tests can set another one).</summary>
        public DateTime Today = DateTime.Today;
        /// <summary>
        /// Best of 3 (Regional Sim, v57): the seat that lost the previous game chooses to go first or second, with no opening
        /// coin flip. -1 (the default) flips a coin as usual.
        /// </summary>
        public int OrderChooser = -1;
        /// <summary>
        /// OrderChooser is the higher seed of a top cut match choosing for game 1 (Regional Sim, v70), not the loser of the
        /// last game: only the wording changes.
        /// </summary>
        public bool OrderChooserIsHigherSeed;

        /// <summary>
        /// A Tie-Breaker game (v76, your ruling): what a single-elimination match plays after a drawn game, instead of the old
        /// Sudden Death. It's set up as usual with 6 Prize cards, and it ends as soon as one player has fewer Prize cards left
        /// than the other (a Prize lead), as well as by the usual win conditions.
        /// </summary>
        public bool TieBreaker;

        /// <summary>True when month * 100 + day is that date's month and day.</summary>
        public static bool IsBirthday(int monthDay, DateTime date)
        {
            return monthDay > 0 && monthDay == date.Month * 100 + date.Day;
        }

        /// <summary>
        /// Stage 1 name -> the Basic it evolves from. Rare Candy needs this to know which Basic a Stage 2 card
        /// belongs to. Filled from a built-in table and from every Stage 1 card the engine sees.
        /// </summary>
        public readonly Dictionary<string, string> PreEvolutionOf = new Dictionary<string, string>(EvolutionLines.Known);

        public GameEngine(CardScriptRegistry scripts, IPlayerAgent agent0, IPlayerAgent agent1)
        {
            Scripts = scripts;
            agents[0] = agent0;
            agents[1] = agent1;
            // Agents that keep track of the game (the Neural AI's memory) watch its public events.
            if (agent0 is IGameObserver watcher0) AddObserver(watcher0);
            if (agent1 is IGameObserver watcher1 && !ReferenceEquals(agent1, agent0)) AddObserver(watcher1);
        }

        public void AddObserver(IGameObserver observer)
        {
            observers.Add(observer);
        }

        public IPlayerAgent GetAgent(int player)
        {
            return agents[player];
        }

        public void SetAgent(int player, IPlayerAgent agent)
        {
            agents[player] = agent;
        }

        public string PlayerName(int player)
        {
            return player >= 0 && player < 2 ? PlayerNames[player] : "Nobody";
        }

        /// <summary>"Your" for a player named "You", otherwise "Name's" (for the battle log).</summary>
        public string Possessive(int player)
        {
            string name = PlayerName(player);
            return name == "You" ? "Your" : name + "'s";
        }

        /// <summary>"your" for a player named "You", otherwise "their".</summary>
        public string Their(int player)
        {
            return PlayerName(player) == "You" ? "your" : "their";
        }

        /// <summary>"Your Dreepy" / "Opponent's Dreepy".</summary>
        public string OwnedName(PokemonInPlay mon)
        {
            return mon == null ? "" : Possessive(mon.Owner) + " " + mon.Name;
        }

        /// <summary>Records which Basic each Stage 1 card evolves from (used by Rare Candy).</summary>
        public void LearnEvolutionLines(IEnumerable<CardDef> cards)
        {
            foreach (var d in cards) LearnEvolutionLine(d);
        }

        private void LearnEvolutionLine(CardDef d)
        {
            if (d != null && d.IsPokemon && d.Stage == Stage.Stage1 && !string.IsNullOrEmpty(d.EvolvesFrom))
                PreEvolutionOf[d.Name] = d.EvolvesFrom;
        }

        // ------------------------------------------------------------------ Running a game

        /// <summary>The 60 cards each player started with (a player knows their own decklist).</summary>
        public IList<CardDef> StartingDeck(int player)
        {
            return player >= 0 && player < 2 ? startingDecks[player] : null;
        }

        private readonly IList<CardDef>[] startingDecks = new IList<CardDef>[2];

        public GameResult Run(IList<CardDef> deck0, IList<CardDef> deck1, int seed)
        {
            startingDecks[0] = deck0;
            startingDecks[1] = deck1;
            try
            {
                Setup(deck0, deck1, seed);
                while (!State.IsOver)
                {
                    if (State.TurnNumber >= MaxTurns) EndGame(-1, "Turn limit reached.");
                    PlayTurn();
                }
            }
            catch (GameOverException)
            {
            }
            return new GameResult(State.Winner, State.EndReason, State.TurnNumber);
        }

        public void Setup(IList<CardDef> deck0, IList<CardDef> deck1, int seed)
        {
            State = new GameState(seed);
            State.InSetup = true;
            for (int i = 0; i < 2; i++)
                State.Players[i].BirthdayToday = Birthdays != null && i < Birthdays.Length && IsBirthday(Birthdays[i], Today);
            nextCardId = 1;
            sourceStack.Clear();
            endTurnRequested = false;
            ForgetLookedAt();
            LearnEvolutionLines(deck0);
            LearnEvolutionLines(deck1);
            // A format's rules can change the cards themselves (Tools as Items, old Trainers as plain Trainers, v123).
            if (Rules == null) Rules = GameRules.Default;
            if (Rules.ChangesCards)
            {
                deck0 = Rules.Adapt(deck0);
                deck1 = Rules.Adapt(deck1);
            }
            var decks = new[] { deck0, deck1 };
            defsById.Clear();
            for (int i = 0; i < 2; i++)
            {
                foreach (var def in decks[i])
                {
                    defsById[nextCardId] = def;
                    State.Players[i].Deck.Add(new CardInstance(nextCardId++, def, i));
                }
                State.Rng.Shuffle(State.Players[i].Deck);
            }
            Emit(GameEventType.GameStart, -1, "Game start");
            // A format can have both players draw their hands (and take mulligans) before the coin flip (v123).
            if (Rules.DrawBeforeCoinFlip)
                for (int i = 0; i < 2; i++) DrawOpeningHand(i);

            // First Ticket (v124): with the hands drawn before the coin flip, a player who plays it goes first without the flip.
            int ticket = Rules.DrawBeforeCoinFlip && OrderChooser != 0 && OrderChooser != 1 ? PlayFirstTickets() : -1;
            if (ticket >= 0) State.FirstPlayer = ticket;
            else
            {
                int flipWinner;
                string orderPrompt = "You won the coin flip. Go first or second?";
                if (OrderChooser == 0 || OrderChooser == 1)
                {
                    // Best of 3: the loser of the previous game chooses (Regional Sim, v57).
                    flipWinner = OrderChooser;
                    orderPrompt = OrderChooserIsHigherSeed ? "You're the higher seed. Go first or second?" : "You lost the last game. Go first or second?";
                    string why = OrderChooserIsHigherSeed ? " the higher seed" : null;
                    Emit(GameEventType.Info, flipWinner, PlayerName(flipWinner) == "You"
                        ? (why != null ? "You're" + why + ", so you choose who goes first." : "You lost the last game, so you choose who goes first.")
                        : PlayerName(flipWinner) + (why != null ? " is" + why + ", so they choose who goes first." : " lost the last game, so they choose who goes first."));
                }
                else
                {
                    // The person on screen calls the opening flip. With two bots nobody is asked and seat 0 has heads (as before,
                    // so seeded bot games don't change).
                    int caller = IsInteractive(0) ? 0 : IsInteractive(1) ? 1 : -1;
                    bool callHeads = true;
                    if (caller >= 0)
                    {
                        var calls = new List<DecisionOption>
                        {
                            new DecisionOption { Label = "Heads", Index = 1 },
                            new DecisionOption { Label = "Tails", Index = 0 }
                        };
                        int call = AskOne(caller, DecisionKind.CallCoinFlip, "Call the coin flip: heads or tails? The winner chooses to go first or second.", calls, null, false);
                        callHeads = calls[call].Index == 1;
                    }
                    bool openingHeads = State.Rng.CoinFlip();
                    flipWinner = caller >= 0 ? (openingHeads == callHeads ? caller : 1 - caller) : (openingHeads ? 0 : 1);
                    string flipText = caller >= 0
                        ? PlayerName(caller) + " called " + (callHeads ? "heads" : "tails") + ". It's " + (openingHeads ? "heads" : "tails") + ": " + PlayerName(flipWinner) + " won the coin flip."
                        : PlayerName(flipWinner) + " won the coin flip.";
                    Emit(GameEventType.CoinFlip, flipWinner, flipText, -1, -1, openingHeads ? 1 : 0);
                }
                var order = new List<DecisionOption>
                {
                    new DecisionOption { Label = "Go first", Index = 0 },
                    new DecisionOption { Label = "Go second", Index = 1 }
                };
                int pick = AskOne(flipWinner, DecisionKind.GoFirstOrSecond, orderPrompt, order, null, false);
                State.FirstPlayer = pick == 0 ? flipWinner : 1 - flipWinner;
            }

            if (!Rules.DrawBeforeCoinFlip)
                for (int i = 0; i < 2; i++) DrawOpeningHand(i);

            for (int i = 0; i < 2; i++) SetupPokemon(i);

            for (int i = 0; i < 2; i++)
            {
                var p = State.Players[i];
                for (int k = 0; k < PrizeCount && p.Deck.Count > 0; k++)
                {
                    p.Prizes.Add(p.Deck[0]);
                    p.Deck.RemoveAt(0);
                }
            }

            // For each mulligan the opponent took beyond your own, you may draw 1 card: when both players mulligan, the same number
            // cancels out (5 against 6: only the player with 5 may draw, 1 card; your ruling, v73.1). This happens after Active,
            // Bench and Prizes are set and before the first turn's draw; Basic Pokémon in the hand can then still go onto the
            // Bench (v69).
            for (int i = 0; i < 2; i++)
            {
                int extra = MulliganDrawsFor(i);
                if (extra <= 0) continue;
                var opts = new List<DecisionOption>();
                for (int n = extra; n >= 0; n--) opts.Add(new DecisionOption { Label = "Draw " + n, Index = n });
                string why = State.Players[i].Mulligans > 0
                    ? "Your opponent mulliganed " + extra + (extra == 1 ? " time" : " times") + " more than you. Draw how many extra cards?"
                    : "Your opponent mulliganed. Draw how many extra cards?";
                int choice = AskOne(i, DecisionKind.MulliganDraws, why, opts, null, false);
                int drawn = opts[choice].Index;
                DrawCards(i, drawn);
                // Then Basic Pokémon in the hand may still go onto the Bench before the game starts, as in TCG Live (v69).
                if (drawn > 0) SetupBench(i, true);
            }

            State.InSetup = false;
            State.CurrentPlayer = 1 - State.FirstPlayer;
            State.TurnNumber = 0;
            Emit(GameEventType.SetupDone, State.FirstPlayer, "Pokémon revealed. " + PlayerName(State.FirstPlayer) + " goes first.");
            if (CheckInvariants) AssertInvariants();
        }

        /// <summary>
        /// First Ticket (Dragon Vault, v124; your ruling: only when the hands come before the coin flip): each player whose hand has
        /// one may play it. If exactly one player does, they go first (no coin flip); if both do, the coin flip decides as usual.
        /// Returns the player going first, or -1.
        /// </summary>
        private int PlayFirstTickets()
        {
            var played = new List<int>();
            for (int i = 0; i < 2; i++)
            {
                var p = State.Players[i];
                var ticket = p.Hand.FirstOrDefault(c => c.Def.IsTrainer && Scripts.For(c.Def).PlayableBeforeCoinFlip);
                if (ticket == null) continue;
                if (!AskYesNo(i, "Play " + ticket.Def.Name + " to go first without the coin flip?", ticket.Def)) continue;
                p.Hand.Remove(ticket);
                p.Discard.Add(ticket);
                Emit(GameEventType.PlayTrainer, i, PlayerName(i) + " played " + ticket.Def.Name + ".", ticket.Id);
                played.Add(i);
            }
            if (played.Count == 1)
            {
                Emit(GameEventType.Info, played[0], PlayerName(played[0]) + " goes first (First Ticket), without the coin flip.");
                return played[0];
            }
            if (played.Count == 2) Emit(GameEventType.Info, -1, "Both players played First Ticket, so the coin flip decides who goes first.");
            return -1;
        }

        /// <summary>Extra cards this player may draw at setup: the opponent's mulligans minus their own (never below 0).</summary>
        public int MulliganDrawsFor(int player)
        {
            return Math.Max(0, State.Players[1 - player].Mulligans - State.Players[player].Mulligans);
        }

        private void DrawOpeningHand(int player)
        {
            var p = State.Players[player];
            while (true)
            {
                DrawCards(player, OpeningHandSize);
                // Manectric's Electric Start counts when going second (your ruling, v83).
                bool keep = p.Hand.Any(c => SetupPlaceable(player, c.Def));
                // Snorlax Doll (your ruling): a hand whose only "Basic" is Snorlax Doll may be kept, putting the Doll down for
                // setup, or mulliganed. The player chooses.
                if (!keep && p.Hand.Any(c => c.Def.IsDoll))
                    keep = AskYesNo(player, "Snorlax Doll: your hand has no Basic Pokémon. Keep it and put Snorlax Doll down for setup? (No takes a mulligan.)");
                // Talonflame's Gale Wings (v93): the player chooses to keep the hand and start with it, or take a mulligan.
                var starter = keep ? null : p.Hand.FirstOrDefault(c => c.Def.IsPokemon && Scripts.For(c.Def).MayKeepOpeningHandWithIt);
                if (starter != null)
                    keep = AskYesNo(player, starter.Def.Name + ": your hand has no Basic Pokémon. Keep it and put " + starter.Def.Name + " down as your Active Pokémon? (No takes a mulligan.)");
                if (keep)
                {
                    if (p.Mulligans > 0)
                        Emit(GameEventType.MulliganReveal, player, PlayerName(player) + " took " + p.Mulligans + (p.Mulligans == 1 ? " mulligan" : " mulligans") + " before drawing a Basic Pokémon.", -1, -1, p.Mulligans);
                    return;
                }
                p.Mulligans++;
                p.MulliganHands.Add(p.Hand.ToList());
                EmitCards(GameEventType.Mulligan, player, PlayerName(player) + " had no Basic Pokémon and took a mulligan (" + p.Mulligans + "), revealing " + Their(player) + " hand.", p.Hand);
                p.Deck.AddRange(p.Hand);
                p.Hand.Clear();
                State.Rng.Shuffle(p.Deck);
                if (p.Mulligans > 100) throw new InvalidOperationException("Deck for " + PlayerName(player) + " has no Basic Pokémon.");
            }
        }

        private void SetupPokemon(int player)
        {
            var p = State.Players[player];
            // Cinderace's Explosiveness: it may start face down in the Active Spot although it isn't a Basic Pokémon.
            // Snorlax Doll: "you may put it face down in the Active Spot or on your Bench" when setting up.
            // Manectric's Electric Start (v83): face down in the Active Spot or on the Bench when this player goes second.
            var basics = p.Hand.Where(c => SetupPlaceable(player, c.Def) || c.Def.IsDoll || (c.Def.IsPokemon && Scripts.For(c.Def).CanStartAsActive)).ToList();
            var activeOpts = DistinctCardOptions(basics);
            // The person on screen always places their Active Pokémon themselves (by dragging it), even with only one Basic;
            // bots take a single choice automatically.
            bool person = IsInteractive(player);
            int pick = AskOne(player, DecisionKind.SetupActive, person ? "Drag a Basic Pokémon onto the board as your Active Pokémon" : "Choose your Active Pokémon",
                activeOpts, null, !person);
            var activeCard = basics.First(c => c.Id == activeOpts[pick].CardId);
            p.Hand.Remove(activeCard);
            p.Active = new PokemonInPlay(State.NextUid++, player, activeCard, 0);
            Emit(GameEventType.Info, player, PlayerName(player) + " placed an Active Pokémon face down.", -1, p.Active.Uid);

            SetupBench(player, false);
        }

        /// <summary>
        /// Basic Pokémon from the hand onto the Bench during setup, face down: after the Active Pokémon, and again after the
        /// extra cards drawn for the opponent's mulligans (v69).
        /// </summary>
        private void SetupBench(int player, bool afterMulliganDraws)
        {
            var p = State.Players[player];
            if (IsInteractive(player))
            {
                // A person drags Basic Pokémon onto the Bench one at a time (or picks them from the list), then presses Done.
                string prompt = afterMulliganDraws
                    ? "You can still put Basic Pokémon onto your Bench (up to " + PlayerState.DefaultMaxBench + "): drag them there, then press Done"
                    : "Drag Basic Pokémon onto your Bench (up to " + PlayerState.DefaultMaxBench + "), then press Done";
                while (p.Bench.Count < PlayerState.DefaultMaxBench)
                {
                    var left = p.Hand.Where(c => SetupPlaceable(player, c.Def) || c.Def.IsDoll).ToList();
                    if (left.Count == 0) break;
                    var opts = left.Select(c => new DecisionOption { Label = "Bench " + c.Def.Name, Action = ActionType.BenchBasic, CardId = c.Id, Card = c.Def }).ToList();
                    opts.Add(new DecisionOption { Label = "Done (" + p.Bench.Count + " on the Bench)", Action = ActionType.EndTurn });
                    int k = AskOne(player, DecisionKind.SetupBench, prompt, opts, null, false);
                    if (k >= left.Count) break;
                    p.Hand.Remove(left[k]);
                    PutOntoBench(player, left[k], false);
                }
                return;
            }

            var rest = p.Hand.Where(c => SetupPlaceable(player, c.Def) || c.Def.IsDoll).ToList();
            int room = PlayerState.DefaultMaxBench - p.Bench.Count;
            if (rest.Count > 0 && room > 0)
            {
                var benchOpts = rest.Select(c => new DecisionOption { Label = c.Def.Name, CardId = c.Id, Card = c.Def }).ToList();
                var picks = AskMany(player, DecisionKind.SetupBench, "Choose up to " + room + " Basic Pokémon for your Bench", benchOpts, 0, room);
                foreach (int i in picks)
                {
                    p.Hand.Remove(rest[i]);
                    PutOntoBench(player, rest[i], false);
                }
            }
        }

        public void PlayTurn()
        {
            StartTurn();
            FinishTurn();
        }

        /// <summary>Main-phase actions until the turn is over, then the end of the turn (no draw: the turn has already started).</summary>
        public void FinishTurn()
        {
            int actions = 0;
            while (true)
            {
                actions++;
                if (actions > MaxActionsPerTurn)
                {
                    Emit(GameEventType.Info, State.CurrentPlayer, "Action limit reached; the turn ends.");
                    break;
                }
                if (MainPhaseStep()) break;
            }
            EndTurn();
        }

        // ------------------------------------------------------------------ Continuing from a saved position

        /// <summary>
        /// Loads a position saved with GameState.Clone() during another game (taken while a player was choosing a main
        /// action), so play can continue from it with this engine's agents: ResumeGame() plays it out, MainPhaseStep()
        /// takes one action. Used by the post-game review to try other plays. Clone the state again before loading it
        /// if you want to keep the saved copy untouched.
        /// </summary>
        public void LoadState(GameState state)
        {
            State = state;
            sourceStack.Clear();
            endTurnRequested = false;
            ForgetLookedAt();
            seenForNextAsk = null;
            seenTitleForNextAsk = null;
            returnAfterAttack.Clear();
            pendingDamageTriggers.Clear();
            attacksResolving = 0;
            ResolvingTrainer = null;
            defsById.Clear();
            int maxId = 0;
            foreach (var c in AllCardsIn(state))
            {
                defsById[c.Id] = c.Def;
                if (c.Id > maxId) maxId = c.Id;
                LearnEvolutionLine(c.Def);
            }
            nextCardId = maxId + 1;
        }

        private static IEnumerable<CardInstance> AllCardsIn(GameState s)
        {
            foreach (var p in s.Players)
            {
                foreach (var c in p.Deck) yield return c;
                foreach (var c in p.Hand) yield return c;
                foreach (var c in p.Discard) yield return c;
                foreach (var c in p.Prizes) yield return c;
                foreach (var mon in p.InPlay())
                    foreach (var c in mon.AllCards()) yield return c;
            }
            if (s.Stadium != null) yield return s.Stadium;
        }

        /// <summary>
        /// Plays a loaded position to the end: finishes the current player's turn from the main phase, then whole turns
        /// until someone wins or MaxTurns is reached (a draw).
        /// </summary>
        public GameResult ResumeGame()
        {
            try
            {
                if (!State.IsOver)
                {
                    FinishTurn();
                    while (!State.IsOver)
                    {
                        if (State.TurnNumber >= MaxTurns) EndGame(-1, "Turn limit reached.");
                        PlayTurn();
                    }
                }
            }
            catch (GameOverException)
            {
            }
            return new GameResult(State.Winner, State.EndReason, State.TurnNumber);
        }

        public void StartTurn()
        {
            if (State.ExtraTurnFor >= 0 && State.ExtraTurnFor == State.CurrentPlayer && State.TurnNumber > 0)
            {
                // "Take another turn after this one" (Star Chronos, v49). The opponent's turn that would come next is skipped
                // as a turn number, so this player's turns stay odd (or even): "during your next turn" effects reach the
                // extra turn, and effects meant for the opponent's next turn move on to the turn they really play.
                int skipped = State.TurnNumber + 1;
                State.ExtraTurnFor = -1;
                State.TurnNumber += 2;
                for (int i = 0; i < State.Effects.Count; i++)
                {
                    var fx = State.Effects[i];
                    if (fx.ExpiresAfterTurn == skipped || fx.ActiveFromTurn == skipped) State.Effects[i] = fx.PostponedPast(skipped);
                }
                Emit(GameEventType.Info, State.CurrentPlayer, PlayerName(State.CurrentPlayer) + " takes another turn.");
            }
            else
            {
                State.ExtraTurnFor = -1;
                State.TurnNumber++;
                State.CurrentPlayer = 1 - State.CurrentPlayer;
            }
            State.Effects.RemoveAll(e => e.ExpiresAfterTurn < State.TurnNumber);
            endTurnRequested = false;
            turnEndsBeforeActions = false;
            ForgetLookedAt();
            var p = State.Current;
            p.PreviousTurnNumber = p.LastTurnNumber;
            p.LastTurnNumber = State.TurnNumber;
            p.ResetTurnFlags();
            Emit(GameEventType.TurnStart, p.Index, "Turn " + State.TurnNumber + " - " + PlayerName(p.Index));
            ToolsBetweenTurns(true);
            if (p.Deck.Count == 0)
                EndGame(1 - p.Index, PlayerName(p.Index) + " couldn't draw a card at the start of " + Their(p.Index) + " turn.");
            bool skipDraw = false;
            foreach (var fx in ActiveEffects())
                if (fx.SkipsTurnDraw(this, p.Index) && !AttackEffectOnPlayerIgnored(fx, p.Index)) { skipDraw = true; break; }
            if (skipDraw && !State.IsOver)
                Emit(GameEventType.Info, p.Index, PlayerName(p.Index) + " can't draw a card at the beginning of " + Their(p.Index) + " turn.");
            if (!skipDraw && DrawCards(p.Index, 1) == 1 && !State.IsOver)
            {
                // "If you drew this Pokémon from your deck at the beginning of your turn" (Metagross's Emergency Entry, v50).
                var drawn = p.Hand[p.Hand.Count - 1];
                Scripts.For(drawn.Def).OnDrawnForTurn(new EffectContext(this, p.Index, drawn, null));
                // Dialga LV.X's Time Skip (v136): "after your opponent draws a card at the beginning of their next turn, their
                // turn ends". The turn goes straight to its end, before any action.
                foreach (var fx in ActiveEffects())
                {
                    if (State.IsOver || !fx.EndsTurnAfterDraw(this, p.Index) || AttackEffectOnPlayerIgnored(fx, p.Index)) continue;
                    Emit(GameEventType.Info, p.Index, Possessive(p.Index) + " turn ends (" + fx.Name + ").");
                    turnEndsBeforeActions = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Neo Genesis Berries (v138): Pokémon Tools that act "at any time between turns" (end of Pokémon Checkup, optional) and
        /// "at the start of each turn" (before the draw, mandatory). A Tool discarded by an earlier one is skipped.
        /// </summary>
        private void ToolsBetweenTurns(bool startOfTurn)
        {
            foreach (var src in Sources())
            {
                if (src.Kind != EffectSourceKind.Trainer || !Overrides(src.Script, "OnBetweenTurns") || !StillInPlay(src)) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnBetweenTurns(src.Ctx, startOfTurn); }
                finally { PopSource(); }
            }
        }

        /// <summary>v136: the turn that just started ends before its first action (Time Skip).</summary>
        private bool turnEndsBeforeActions;

        /// <summary>Asks the current player for one main-phase action and performs it. Returns true when the turn is over.</summary>
        public bool MainPhaseStep()
        {
            if (turnEndsBeforeActions)
            {
                turnEndsBeforeActions = false;
                return true;
            }
            int me = State.CurrentPlayer;
            ForgetLookedAt();
            var options = GetMainActions(me);
            int pick = AskOne(me, DecisionKind.MainAction, "Choose an action", options, null, false);
            bool turnOver = ExecuteMainAction(me, options[pick]);
            if (endTurnRequested)
            {
                endTurnRequested = false;
                turnOver = true;
            }
            ResolveKnockouts(); // also cuts a Bench over its limit (Sudowoodo's Roadblock, v69) and fills an empty Active Spot
            if (CheckInvariants) AssertInvariants();
            return turnOver;
        }

        public void EndTurn()
        {
            Emit(GameEventType.TurnEnd, State.CurrentPlayer, Possessive(State.CurrentPlayer) + " turn ends.");

            // "At the end of your turn" effects (Powerglass, Ignition Energy), before Pokémon Checkup.
            int current = State.CurrentPlayer;
            foreach (var src in Sources())
            {
                if (src.Ctx.Player != current) continue;
                if (!StillInPlay(src)) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnEndOfTurn(src.Ctx); }
                finally { PopSource(); }
            }
            // "At the end of each turn" (Emergency Jelly, v50): every source, either player's turn.
            foreach (var src in Sources())
            {
                if (!StillInPlay(src)) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnEndOfEachTurn(src.Ctx); }
                finally { PopSource(); }
            }
            // Bursting Balloon (v97): discarded at the end of the opponent's turn whether or not Tools work right now (2016); the
            // Technical Machines at the end of their owner's turn the same way (your ruling, v97).
            for (int side = 0; side < 2; side++)
            {
                foreach (var mon in State.Players[side].InPlay().ToList())
                    foreach (var tool in mon.Tools.ToList())
                    {
                        var toolScript = Scripts.For(tool.Def);
                        if (side == current ? toolScript.DiscardedAtEndOfOwnersTurn : toolScript.DiscardedAtEndOfOpponentsTurn) DiscardTool(mon, tool);
                    }
            }
            // Timed effects of attacks that happen "at the end of your opponent's next turn" (Slow Crunch, Permeating Chill).
            foreach (var e in ActiveEffects().ToList())
            {
                if (!State.Effects.Contains(e)) continue;
                PushSource(EffectSourceKind.Attack, e.Owner, null);
                try { e.OnEndOfTurn(this); }
                finally { PopSource(); }
            }
            ResolveKnockouts();
            // Wishiwashi's Scatter (v68): after the Knock Outs, before Pokémon Checkup. A Pokémon that left play is replaced.
            bool afterKnockOuts = false;
            foreach (var src in Sources())
            {
                if (!StillInPlay(src)) continue;
                int before = State.Players[0].InPlay().Count() + State.Players[1].InPlay().Count();
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnEndOfTurnAfterKnockOuts(src.Ctx); }
                finally { PopSource(); }
                if (State.Players[0].InPlay().Count() + State.Players[1].InPlay().Count() != before) afterKnockOuts = true;
            }
            if (afterKnockOuts) ResolveKnockouts();

            // Star Chronos: "Take another turn after this one. (Skip Pokémon Checkup.)" (v49)
            if (State.SkipCheckup)
            {
                State.SkipCheckup = false;
                Emit(GameEventType.Info, State.CurrentPlayer, "Pokémon Checkup is skipped.");
                if (CheckInvariants) AssertInvariants();
                return;
            }

            State.InCheckup = true;
            PushSource(EffectSourceKind.Rule, -1, null);
            try
            {
                int[] order = { State.CurrentPlayer, 1 - State.CurrentPlayer };
                foreach (int i in order)
                {
                    var mon = State.Players[i].Active;
                    if (mon == null) continue;
                    if (mon.Has(SpecialCondition.Poisoned)) PlaceDamageCounters(mon, PoisonCounters(mon), "Poison");
                    if (mon.Has(SpecialCondition.Burned))
                    {
                        PlaceDamageCounters(mon, BurnCounters(mon), "Burn");
                        if (FlipCoin(i, mon.Name + " is Burned"))
                        {
                            if (BurnRecoveryBlocked(mon)) Emit(GameEventType.Info, i, mon.Name + " stays Burned.", mon.Top.Id, mon.Uid);
                            else RemoveCondition(mon, SpecialCondition.Burned);
                        }
                    }
                    if (mon.Has(SpecialCondition.Asleep))
                    {
                        // Altaria's Soothing Lullaby: 2 coins for as long as that Asleep lasts; it wakes up only if both are heads.
                        int coins = Math.Max(1, mon.SleepCoins);
                        foreach (var src in SourcesFor(mon)) coins = src.Script.ModifySleepCoins(src.Ctx, mon, coins);
                        coins = Math.Max(1, coins);
                        int sleepHeads = coins == 1 ? (FlipCoin(i, mon.Name + " is Asleep") ? 1 : 0) : FlipCoins(i, coins, mon.Name + " is Asleep");
                        if (sleepHeads == coins) RemoveCondition(mon, SpecialCondition.Asleep);
                        // Darkrai LV.X's Endless Darkness (v136): both coins tails, the Pokémon is Knocked Out.
                        else if (sleepHeads == 0 && mon.SleepKnocksOut) KnockOut(mon, "Endless Darkness");
                    }
                    if (mon.Has(SpecialCondition.Paralyzed) && i == State.CurrentPlayer && State.TurnNumber > mon.ParalyzedOnTurn)
                        RemoveCondition(mon, SpecialCondition.Paralyzed);
                }
                // Char counters (Neo Genesis Quilava, v138): the Pokémon's owner flips after every turn; tails, 2 damage counters.
                foreach (int i in order)
                    foreach (var mon in State.Players[i].InPlay().ToList())
                        if (mon.Markers.ContainsKey(PokemonInPlay.CharMarker) && State.FindPokemon(mon.Uid) == mon && !FlipCoin(i, "Char counter on " + mon.Name))
                            PlaceDamageCounters(mon, 2, "Char");
                // Abilities that act during Pokémon Checkup (Froslass's Freezing Shroud), and the Stadium in play (Shrine of
                // Punishment, v69).
                var checkups = new List<PassiveSource>();
                foreach (var src in Sources())
                {
                    bool stadium = src.Kind == EffectSourceKind.Stadium;
                    if (src.Kind != EffectSourceKind.Ability && src.Kind != EffectSourceKind.PokemonPower && !stadium) continue;
                    if (Overrides(src.Script, "OnPokemonCheckup")) checkups.Add(src);
                }
                OrderCheckups(checkups);
                foreach (var src in checkups)
                {
                    if (src.Kind != EffectSourceKind.Stadium && !StillInPlay(src)) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnPokemonCheckup(src.Ctx); }
                    finally { PopSource(); }
                }
                ToolsBetweenTurns(false);
                ResolveKnockouts();
            }
            finally
            {
                PopSource();
                State.InCheckup = false;
            }
            if (CheckInvariants) AssertInvariants();
        }

        /// <summary>
        /// The player whose Pokémon 2 or more different Pokémon Checkup effects act on chooses their order (Compendium, Flygon's
        /// Sand Slammer vs Serperior's Royal Heal, 2012-11-01) when both players' cards act on them. Their effects take the same
        /// places in the list, so everything else keeps its order; copies of one effect aren't asked about.
        /// </summary>
        private void OrderCheckups(List<PassiveSource> checkups)
        {
            // Nearly every Checkup has fewer than 2 such effects: nothing to order and nothing to allocate.
            if (checkups.Count < 2) return;
            for (int turn = 0; turn < 2; turn++)
            {
                int player = turn == 0 ? State.CurrentPlayer : 1 - State.CurrentPlayer;
                // Only when both players' cards act on them: the order of a player's own effects (heals) doesn't matter.
                bool fromMe = false, fromThem = false;
                for (int i = 0; i < checkups.Count; i++)
                {
                    if (checkups[i].Script.CheckupAffects(checkups[i].Ctx) != player) continue;
                    if (checkups[i].Ctx.Player == player) fromMe = true; else fromThem = true;
                }
                if (!fromMe || !fromThem) continue;
                var slots = new List<int>();
                for (int i = 0; i < checkups.Count; i++)
                    if (checkups[i].Script.CheckupAffects(checkups[i].Ctx) == player) slots.Add(i);
                var left = slots.Select(slot => checkups[slot]).ToList();
                var chosen = new List<PassiveSource>();
                while (left.Count > 1 && left.Select(CheckupName).Distinct().Count() > 1)
                {
                    var opts = left.Select(s => new DecisionOption
                    {
                        Label = s.Ctx.Pokemon == null ? CheckupName(s) : CheckupName(s) + " (" + OwnedName(s.Ctx.Pokemon) + ")"
                    }).ToList();
                    int pick = AskOne(player, DecisionKind.ChooseOption, "Pokémon Checkup: which effect on your Pokémon happens next?", opts, null, false);
                    chosen.Add(left[pick]);
                    left.RemoveAt(pick);
                }
                chosen.AddRange(left);
                for (int k = 0; k < slots.Count; k++) checkups[slots[k]] = chosen[k];
            }
        }

        private static string CheckupName(PassiveSource src)
        {
            var mon = src.Ctx.Pokemon;
            if (src.Kind == EffectSourceKind.Stadium || mon == null) return src.Ctx.Card != null ? src.Ctx.Card.Def.Name : "Effect";
            var ability = mon.Def.Abilities.FirstOrDefault();
            return ability != null ? ability.Name : mon.Name;
        }

        /// <summary>A source can leave play while end-of-turn effects resolve; skip it if so.</summary>
        private bool StillInPlay(PassiveSource src)
        {
            if (src.Kind == EffectSourceKind.Stadium) return State.Stadium == src.Ctx.Card;
            var mon = src.Ctx.Pokemon;
            if (mon == null || State.FindPokemon(mon.Uid) != mon) return false;
            if (src.Kind == EffectSourceKind.Trainer) return mon.HasTool(src.Ctx.Card);
            if (src.Kind == EffectSourceKind.Energy) return mon.Energy.Contains(src.Ctx.Card);
            return true;
        }

        // ------------------------------------------------------------------ "Your turn ends"

        private bool endTurnRequested;

        /// <summary>A card effect ends the turn once the current action is done (Boxed Order: "Your turn ends.").</summary>
        public void RequestEndTurn()
        {
            if (State.IsOver || State.InSetup) return;
            endTurnRequested = true;
            Emit(GameEventType.Info, State.CurrentPlayer, Possessive(State.CurrentPlayer) + " turn will end.");
        }

        // ------------------------------------------------------------------ What is resolving right now

        private struct SourceFrame
        {
            public EffectSourceKind Kind;
            public int Owner;
            public PokemonInPlay Pokemon;
        }

        private readonly List<SourceFrame> sourceStack = new List<SourceFrame>();

        /// <summary>Marks the start of an effect (attack, Ability, Trainer...). Always pair with PopSource in a finally block.</summary>
        public void PushSource(EffectSourceKind kind, int owner, PokemonInPlay pokemon)
        {
            if (sourceStack.Count == 0) WatchHandDiscards(kind, owner);
            sourceStack.Add(new SourceFrame { Kind = kind, Owner = owner, Pokemon = pokemon });
        }

        public void PopSource()
        {
            if (sourceStack.Count > 0) sourceStack.RemoveAt(sourceStack.Count - 1);
            if (sourceStack.Count == 0)
            {
                ShowLookedAtIfUnseen();
                ForgetLookedAt(); // the effect that looked at the cards is over
                CheckHandDiscards();
            }
        }

        /// <summary>
        /// Cards in the other player's hand that react to being discarded from it by this effect (Amoonguss's Surprise Spores,
        /// v50): "during your opponent's turn, if this Pokémon is discarded from your hand by an effect of an attack or Ability
        /// from your opponent's Pokémon, or by an effect of your opponent's Item or Supporter cards". Watched from the start of
        /// the current player's attack, Ability or Trainer card to its end.
        /// </summary>
        private List<CardInstance> watchedHandCards;
        private int watchedHandOwner = -1;

        private void WatchHandDiscards(EffectSourceKind kind, int owner)
        {
            watchedHandCards = null;
            watchedHandOwner = -1;
            if (owner < 0 || owner != State.CurrentPlayer) return;
            if (kind != EffectSourceKind.Attack && kind != EffectSourceKind.Ability && kind != EffectSourceKind.PokemonPower && kind != EffectSourceKind.Trainer) return;
            var other = State.Players[1 - owner];
            foreach (var c in other.Hand)
            {
                if (!Scripts.For(c.Def).WatchesHandDiscardByOpponent) continue;
                if (watchedHandCards == null) watchedHandCards = new List<CardInstance>();
                watchedHandCards.Add(c);
            }
            if (watchedHandCards != null) watchedHandOwner = 1 - owner;
        }

        private void CheckHandDiscards()
        {
            if (watchedHandCards == null) return;
            var cards = watchedHandCards;
            int player = watchedHandOwner;
            watchedHandCards = null;
            watchedHandOwner = -1;
            if (State.IsOver) return;
            var discard = State.Players[player].Discard;
            foreach (var c in cards)
                if (discard.Contains(c)) Scripts.For(c.Def).OnDiscardedFromHandByOpponent(new EffectContext(this, player, c, null));
        }

        /// <summary>Kind of effect currently resolving (None outside of any effect).</summary>
        public EffectSourceKind CurrentSourceKind { get { return sourceStack.Count == 0 ? EffectSourceKind.None : sourceStack[sourceStack.Count - 1].Kind; } }
        /// <summary>Player whose card is causing the current effect, or -1.</summary>
        public int CurrentSourceOwner { get { return sourceStack.Count == 0 ? -1 : sourceStack[sourceStack.Count - 1].Owner; } }
        /// <summary>Pokémon whose attack or Ability is resolving (or that holds the Tool/Energy), or null.</summary>
        public PokemonInPlay CurrentSourcePokemon { get { return sourceStack.Count == 0 ? null : sourceStack[sourceStack.Count - 1].Pokemon; } }

        /// <summary>True when the current effect is an attack or Ability of the other player's Pokémon (so "prevent effects" can apply).</summary>
        public bool IsOpponentsAttackOrAbility(PokemonInPlay target)
        {
            var kind = CurrentSourceKind;
            return (kind == EffectSourceKind.Attack || kind == EffectSourceKind.Ability) && CurrentSourceOwner >= 0 && CurrentSourceOwner != target.Owner;
        }

        // ------------------------------------------------------------------ Decisions

        public int AskOne(int player, DecisionKind kind, string prompt, List<DecisionOption> options, CardDef source = null, bool autoPickSingle = true)
        {
            if (options.Count == 0) throw new InvalidOperationException("No options for decision: " + prompt);
            if (autoPickSingle && options.Count == 1) return 0;
            return Ask(player, kind, prompt, options, 1, 1, source)[0];
        }

        /// <summary>The seat is played by a person on screen (see IInteractiveAgent).</summary>
        public bool IsInteractive(int player)
        {
            return player >= 0 && player < agents.Length && agents[player] is IInteractiveAgent;
        }

        /// <summary>
        /// Asks for min..max of the options. When only one answer is possible (all of them), it's taken at once,
        /// unless alwaysAsk is set (cards from a person's discard pile: they still see them and pick them).
        /// </summary>
        public List<int> AskMany(int player, DecisionKind kind, string prompt, List<DecisionOption> options, int min, int max, CardDef source = null, bool alwaysAsk = false)
        {
            max = Math.Min(max, options.Count);
            min = Math.Max(0, Math.Min(min, max));
            if (max == 0) return new List<int>();
            if (min == options.Count && !alwaysAsk)
            {
                var all = new List<int>();
                for (int i = 0; i < options.Count; i++) all.Add(i);
                return all;
            }
            return Ask(player, kind, prompt, options, min, max, source);
        }

        public bool AskYesNo(int player, string prompt, CardDef source = null)
        {
            var opts = new List<DecisionOption>
            {
                new DecisionOption { Label = "Yes", Index = 1 },
                new DecisionOption { Label = "No", Index = 0 }
            };
            return AskOne(player, DecisionKind.YesNo, prompt, opts, source, false) == 0;
        }

        /// <summary>Asks for a number in [min, max]. Options are listed from max down to min.</summary>
        public int AskNumber(int player, int min, int max, string prompt, CardDef source = null)
        {
            var opts = new List<DecisionOption>();
            for (int n = max; n >= min; n--) opts.Add(new DecisionOption { Label = n.ToString(), Index = n });
            return opts[AskOne(player, DecisionKind.ChooseNumber, prompt, opts, source)].Index;
        }

        private int redirectDecisionsOf = -1;
        private int redirectDecisionsTo = -1;

        /// <summary>
        /// Hypno's Hand Control (v103): until EndDecisionRedirect, the choices that would be the first player's are made by the
        /// second (the opponent uses a Supporter's effect while you make all its decisions).
        /// </summary>
        public void RedirectDecisions(int ofPlayer, int toPlayer)
        {
            redirectDecisionsOf = ofPlayer;
            redirectDecisionsTo = toPlayer;
        }

        public void EndDecisionRedirect()
        {
            redirectDecisionsOf = -1;
            redirectDecisionsTo = -1;
        }

        private List<int> Ask(int player, DecisionKind kind, string prompt, List<DecisionOption> options, int min, int max, CardDef source)
        {
            if (redirectDecisionsOf >= 0 && player == redirectDecisionsOf) player = redirectDecisionsTo;
            if (lookedAt != null && player == lookedAtPlayer) lookedAtAsked = true;
            // Bodyguard (v94): a choice about the cards of a shielded hand is made without asking, so they're never shown
            // (whatever the attack does to that hand is undone afterwards).
            if (guardedHandPlayer >= 0 && TouchesGuardedHand(options, seenForNextAsk))
            {
                guardedHandTouched = true;
                seenForNextAsk = null;
                seenTitleForNextAsk = null;
                var auto = new List<int>();
                for (int i = 0; i < min && i < options.Count; i++) auto.Add(i);
                return auto;
            }
            var req = new DecisionRequest
            {
                Player = player,
                Kind = kind,
                Prompt = prompt,
                Options = options,
                MinChoices = min,
                MaxChoices = max,
                Source = source,
                View = PlayerView.Create(this, player)
            };
            if (seenForNextAsk != null)
            {
                req.SeenCards = seenForNextAsk.Select(c => new CardRef(c)).ToList();
                req.SeenTitle = seenTitleForNextAsk;
            }
            seenForNextAsk = null;
            seenTitleForNextAsk = null;
            var agent = agents[player];
            if (agent == null) throw new InvalidOperationException("No agent for " + PlayerName(player));
            var result = agent.Decide(req);
            Validate(req, result);
            return result;
        }

        private static void Validate(DecisionRequest req, List<int> result)
        {
            if (result == null) throw new InvalidOperationException("Agent returned no answer for: " + req.Prompt);
            if (result.Count < req.MinChoices || result.Count > req.MaxChoices)
                throw new InvalidOperationException("Agent picked " + result.Count + " options for '" + req.Prompt + "' (allowed " + req.MinChoices + "-" + req.MaxChoices + ").");
            var seen = new HashSet<int>();
            foreach (int i in result)
            {
                if (i < 0 || i >= req.Options.Count) throw new InvalidOperationException("Agent picked invalid option " + i + " for: " + req.Prompt);
                if (!seen.Add(i)) throw new InvalidOperationException("Agent picked option " + i + " twice for: " + req.Prompt);
            }
        }

        private static List<DecisionOption> DistinctCardOptions(IEnumerable<CardInstance> cards)
        {
            var list = new List<DecisionOption>();
            var byKey = new Dictionary<string, DecisionOption>();
            foreach (var c in cards)
            {
                DecisionOption first;
                if (byKey.TryGetValue(c.Def.BaseKey, out first))
                {
                    // Identical cards are listed once; the UI can still drag any copy (see DecisionOption.SameCardIds).
                    if (first.SameCardIds == null) first.SameCardIds = new List<int>();
                    first.SameCardIds.Add(c.Id);
                    continue;
                }
                var o = new DecisionOption { Label = c.Def.Name, CardId = c.Id, Card = c.Def };
                byKey[c.Def.BaseKey] = o;
                list.Add(o);
            }
            return list;
        }

        // ------------------------------------------------------------------ Events and checks

        public void Emit(GameEventType type, int player, string text, int cardId = -1, int pokemonUid = -1, int amount = 0)
        {
            // Bodyguard (v94): nothing about the cards of a shielded hand is shown while the opponent's attack resolves.
            if (guardedHandPlayer >= 0 && HidesGuardedHand(type, text, cardId, null)) { guardedHandTouched = true; return; }
            if (observers.Count == 0) return;
            var ev = new GameEvent { Type = type, Player = player, Text = text, CardId = cardId, PokemonUid = pokemonUid, Amount = amount };
            CardDef def;
            if (cardId >= 0 && IsPublicCardEvent(type) && defsById.TryGetValue(cardId, out def)) ev.Card = def;
            // Lt. Surge's Secret Plan (v126): a face-down card stays hidden.
            if (cardId >= 0 && State != null && State.FaceDownCards.ContainsKey(cardId)) ev.Card = FaceDownDef;
            for (int i = 0; i < observers.Count; i++) observers[i].OnEvent(this, ev);
        }

        /// <summary>Card id -> printed card, so events can say which card was played.</summary>
        private readonly Dictionary<int, CardDef> defsById = new Dictionary<int, CardDef>();

        /// <summary>Events whose card both players see (so GameEvent.Card may be filled in). Not prizes, draws or searches.</summary>
        private static bool IsPublicCardEvent(GameEventType type)
        {
            switch (type)
            {
                case GameEventType.PutOnBench:
                case GameEventType.Evolve:
                case GameEventType.AttachEnergy:
                case GameEventType.AttachTool:
                case GameEventType.PlayTrainer:
                case GameEventType.UseAbility:
                case GameEventType.Retreat:
                case GameEventType.Switch:
                case GameEventType.Attack:
                case GameEventType.KnockOut:
                case GameEventType.Promote:
                case GameEventType.Discard:
                case GameEventType.StadiumPlayed:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>An event that shows cards to both players (they're listed in GameEvent.Cards).</summary>
        public void EmitCards(GameEventType type, int player, string text, IEnumerable<CardInstance> cards)
        {
            if (guardedHandPlayer >= 0 && HidesGuardedHand(type, text, -1, cards)) { guardedHandTouched = true; return; }
            if (observers.Count == 0) return;
            var list = cards == null ? new List<CardRef>() : cards.Select(c => new CardRef(c)).ToList();
            var ev = new GameEvent { Type = type, Player = player, Text = text, Cards = list, Amount = list.Count };
            for (int i = 0; i < observers.Count; i++) observers[i].OnEvent(this, ev);
        }

        /// <summary>Shows cards to both players: "Opponent revealed Dreepy and put it into their hand."</summary>
        public void Reveal(int player, IList<CardInstance> cards, string what)
        {
            if (cards == null || cards.Count == 0) return;
            EmitCards(GameEventType.Reveal, player, PlayerName(player) + " " + what + ": " + string.Join(", ", cards.Select(c => c.Def.Name).ToArray()) + ".", cards);
        }

        // ------------------------------------------------------------------ What the player can see while choosing

        /// <summary>Cards an effect is looking at (LookAtTop), for the "All" tab of the next card choice.</summary>
        private List<CardInstance> lookedAt;
        private string lookedAtTitle;
        private int lookedAtPlayer = -1;
        /// <summary>v128: the player who looked has been asked something since (so they saw the cards, or chose from them).</summary>
        private bool lookedAtAsked;
        private List<CardInstance> seenForNextAsk;
        private string seenTitleForNextAsk;

        /// <summary>Remembers the cards an effect is looking at, so a choice from them can show them all.</summary>
        public void NoteLookedAt(List<CardInstance> cards, string title, int player)
        {
            lookedAt = cards;
            lookedAtTitle = title;
            lookedAtPlayer = player;
            lookedAtAsked = false;
        }

        /// <summary>
        /// v128 (your request): an effect looked at cards and the person never got a choice about them (Trainers' Mail or Max Elixir
        /// finding nothing they can take, Drone Rotom's look at the top card): before the effect ends or the deck is shuffled, they
        /// see the cards and press Done. Bots aren't asked; a choice from the cards (with its All tab) already showed them.
        /// </summary>
        private void ShowLookedAtIfUnseen()
        {
            if (lookedAt == null || lookedAtAsked || lookedAt.Count == 0) return;
            lookedAtAsked = true;
            int player = lookedAtPlayer;
            if (!IsInteractive(player) || State.IsOver) return;
            seenForNextAsk = lookedAt.ToList();
            seenTitleForNextAsk = lookedAtTitle;
            try { Ask(player, DecisionKind.ChooseCards, (lookedAtTitle ?? "The cards you looked at") + ": none of these can be taken. Press Done to go on.", new List<DecisionOption>(), 0, 0, null); }
            finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
        }

        public void ForgetLookedAt()
        {
            lookedAt = null;
            lookedAtTitle = null;
            lookedAtPlayer = -1;
        }

        /// <summary>The cards a choice from these options lets the player see ("All" tab), or null.</summary>
        private List<CardInstance> SeenFor(int player, IList<CardInstance> options, out string title)
        {
            title = null;
            if (options == null || options.Count == 0) return null;
            var p = State.Players[player];
            var o = State.Players[1 - player];
            if (lookedAt != null && lookedAtPlayer == player && options.All(c => lookedAt.Contains(c)))
            {
                title = lookedAtTitle;
                return lookedAt.ToList();
            }
            if (options.All(c => p.Discard.Contains(c)))
            {
                title = "Your discard pile";
                return p.Discard.ToList();
            }
            if (options.All(c => p.Hand.Contains(c)))
            {
                title = "Your hand";
                return p.Hand.ToList();
            }
            if (options.All(c => o.Hand.Contains(c)))
            {
                title = "Your opponent's hand";
                return o.Hand.ToList();
            }
            if (options.All(c => o.Discard.Contains(c)))
            {
                title = "Your opponent's discard pile";
                return o.Discard.ToList();
            }
            return null;
        }

        public void Log(int player, string text)
        {
            Emit(GameEventType.Info, player, text);
        }

        public void AssertInvariants()
        {
            string error = Invariants.Check(this, CardsPerPlayer);
            if (error != null) throw new InvalidOperationException("Invariant broken on turn " + State.TurnNumber + ": " + error);
        }

        // ------------------------------------------------------------------ Scenario building (tests, puzzles)

        /// <summary>Starts an empty board for hand-built scenarios. Nothing is dealt; place cards with the helpers below.</summary>
        public void BeginScenario(int seed)
        {
            State = new GameState(seed);
            nextCardId = 1;
            defsById.Clear();
            sourceStack.Clear();
            endTurnRequested = false;
            ForgetLookedAt();
            State.FirstPlayer = 0;
            State.CurrentPlayer = 0;
            State.TurnNumber = 0;
        }

        public CardInstance NewCard(int owner, CardDef def)
        {
            if (Rules != null && Rules.ChangesCards) def = Rules.Adapt(def); // the format's view of the card (v123)
            LearnEvolutionLine(def);
            defsById[nextCardId] = def;
            return new CardInstance(nextCardId++, def, owner);
        }

        public PokemonInPlay ScenarioActive(int player, CardDef def)
        {
            var mon = new PokemonInPlay(State.NextUid++, player, NewCard(player, def), 0);
            State.Players[player].Active = mon;
            return mon;
        }

        public PokemonInPlay ScenarioBench(int player, CardDef def)
        {
            var mon = new PokemonInPlay(State.NextUid++, player, NewCard(player, def), 0);
            State.Players[player].Bench.Add(mon);
            return mon;
        }

        /// <summary>Puts an evolution card on top of a scenario Pokémon (no rules checks, no triggers).</summary>
        public void ScenarioEvolve(PokemonInPlay mon, CardDef evolution)
        {
            mon.Stack.Add(NewCard(mon.Owner, evolution));
        }

        public void ScenarioAttach(PokemonInPlay mon, CardDef energy)
        {
            mon.Energy.Add(NewCard(mon.Owner, energy));
        }

        public void ScenarioAttachTool(PokemonInPlay mon, CardDef tool)
        {
            mon.Tools.Add(NewCard(mon.Owner, tool));
        }

        public void ScenarioSetStadium(int owner, CardDef stadium)
        {
            State.Stadium = NewCard(owner, stadium);
            State.StadiumOwner = owner;
        }

        public CardInstance ScenarioAddToHand(int player, CardDef def)
        {
            var c = NewCard(player, def);
            State.Players[player].Hand.Add(c);
            return c;
        }

        public void ScenarioAddToDeck(int player, CardDef def, int copies)
        {
            for (int i = 0; i < copies; i++) State.Players[player].Deck.Add(NewCard(player, def));
        }

        public void ScenarioAddToDiscard(int player, CardDef def, int copies)
        {
            for (int i = 0; i < copies; i++) State.Players[player].Discard.Add(NewCard(player, def));
        }

        public void ScenarioAddPrizes(int player, CardDef def, int copies)
        {
            for (int i = 0; i < copies; i++) State.Players[player].Prizes.Add(NewCard(player, def));
        }

        /// <summary>Makes it the given player's turn without drawing. Turn 1 = first player's first turn.</summary>
        public void ScenarioSetTurn(int player, int turnNumber)
        {
            State.TurnNumber = turnNumber;
            State.CurrentPlayer = player;
            State.FirstPlayer = (turnNumber % 2 == 1) ? player : 1 - player;
            State.Players[player].LastTurnNumber = turnNumber;
            State.Players[player].PreviousTurnNumber = turnNumber > 2 ? turnNumber - 2 : PlayerState.NoTurn;
            State.Players[1 - player].LastTurnNumber = turnNumber > 1 ? turnNumber - 1 : PlayerState.NoTurn;
            State.Players[1 - player].PreviousTurnNumber = turnNumber > 3 ? turnNumber - 3 : PlayerState.NoTurn;
            State.Players[player].ResetTurnFlags();
        }
    }

    /// <summary>Stage 1 Pokémon in the Standard pool and the Basic each evolves from (for Rare Candy).</summary>
    public static class EvolutionLines
    {
        /// <summary>Adds every Stage 1 card's line (called when card data loads, so new sets need no table edits).</summary>
        public static void Learn(IEnumerable<CardDef> cards)
        {
            lock (Known)
            {
                foreach (var d in cards)
                {
                    if (d != null && d.IsPokemon && d.Stage == Stage.Stage1 && !string.IsNullOrEmpty(d.EvolvesFrom) && !Known.ContainsKey(d.Name))
                        Known[d.Name] = d.EvolvesFrom;
                    if (d != null && d.IsPokemon && !string.IsNullOrEmpty(d.EvolvesFrom) && !d.IsBreak && !d.IsLevelUp && !d.IsLegend)
                        Evolvable.Add(d.EvolvesFrom);
                    if (d != null && d.IsPokemon && d.IsBreak && !string.IsNullOrEmpty(d.EvolvesFrom))
                        BreakEvolvable.Add(d.EvolvesFrom);
                }
            }
        }

        /// <summary>
        /// Names of Pokémon that some card in the pool evolves from (v95). Evosoda and Wally need a Pokémon that could evolve (not
        /// a Snorlax with no Evolution: Compendium, 2018-07-26), without looking at the deck.
        /// </summary>
        public static readonly HashSet<string> Evolvable = new HashSet<string>();

        /// <summary>Names of Pokémon that a BREAK Evolution card in the pool evolves from (Evosoda and Wally can find those: your ruling, v95).</summary>
        public static readonly HashSet<string> BreakEvolvable = new HashSet<string>();

        public static bool CanEvolveAtAll(string name)
        {
            return CanEvolveAtAll(name, false);
        }

        public static bool CanEvolveAtAll(string name, bool includeBreak)
        {
            lock (Known) { return name != null && (Evolvable.Contains(name) || (includeBreak && BreakEvolvable.Contains(name))); }
        }

        public static readonly Dictionary<string, string> Known = new Dictionary<string, string>
        {
            { "Drakloak", "Dreepy" },
            { "Kadabra", "Abra" },
            { "Dusclops", "Duskull" },
            { "Metang", "Beldum" },
            { "Steven's Metang", "Steven's Beldum" },
            { "Bayleef", "Chikorita" },
            { "Dipplin", "Applin" },
            { "Thwackey", "Grookey" },
            { "Combusken", "Torchic" },
            { "Cynthia's Gabite", "Cynthia's Gible" },
            { "Dolliv", "Smoliv" },
            { "Trumbeak", "Pikipek" },
            { "Prinplup", "Piplup" },
            { "Primeape", "Mankey" },
            { "Palpitoad", "Tympole" },
            { "Pawmo", "Pawmi" },
            { "Carkol", "Rolycoly" },
            { "Fraxure", "Axew" },
            // Temporal Forces
            { "Nuzleaf", "Seedot" },
            { "Grotle", "Turtwig" },
            { "Torracat", "Litten" },
            { "Croconaw", "Totodile" },
            { "Charjabug", "Grubbin" },
            { "Duosion", "Solosis" },
            { "Haunter", "Gastly" },
            { "Tranquill", "Pidove" }
        };
    }
}
