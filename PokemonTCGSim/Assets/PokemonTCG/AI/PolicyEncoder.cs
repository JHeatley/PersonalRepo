using System;
using System.Collections.Generic;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// What one player remembers during a game, like a real player keeps track: every card the opponent has shown, the cards
    /// known to be in the opponent's hand (revealed by searches until that hand is shuffled away), what each side did this
    /// turn and last turn. It only ever reads public events and the player's own PlayerView, so it can't learn hidden cards.
    /// </summary>
    public sealed class PolicyMemory : IGameObserver
    {
        /// <summary>Supporters and other cards that shuffle the player's hand away (a revealed hand is forgotten after them).</summary>
        public static readonly HashSet<string> HandShufflers = new HashSet<string>(StringComparer.Ordinal)
        {
            "Lillie's Determination", "Judge", "Unfair Stamp", "Team Rocket's Archer", "Iono", "Professor's Research",
            "Roxanne", "Marnie", "Cynthia", "N", "Reset Stamp"
        };

        public int Seat = -1;
        /// <summary>This player's 60 cards, if known (for the "cards still in my deck or Prize cards" input).</summary>
        public List<CardDef> DeckList;

        public int Turn = -1;
        public int ActionsThisTurn;
        public int AbilitiesThisTurn;
        public bool EnergyAttached, SupporterPlayed, Retreated, StadiumPlayed;
        public readonly HashSet<int> AbilityUidsThisTurn = new HashSet<int>();
        public readonly List<CardDef> PlayedThisTurn = new List<CardDef>();
        public readonly List<CardDef> OppLastTurn = new List<CardDef>();
        private readonly List<CardDef> oppThisTurn = new List<CardDef>();
        public readonly Dictionary<int, CardDef> OppKnownHand = new Dictionary<int, CardDef>();
        public readonly Dictionary<int, CardDef> OppSeen = new Dictionary<int, CardDef>();
        /// <summary>Turn each Pokémon (by Uid) came into play or last evolved, and the card on top then.</summary>
        private readonly Dictionary<int, int> enteredTurn = new Dictionary<int, int>();
        private readonly Dictionary<int, int> topCard = new Dictionary<int, int>();
        private readonly Dictionary<int, int> movedTurn = new Dictionary<int, int>();
        private readonly int[] activeUid = { -1, -1 };
        public readonly int[] KoTurn = { -10, -10 };
        /// <summary>Your Prize cards as you last saw them when you searched your deck (by card id).</summary>
        public readonly Dictionary<int, CardDef> KnownPrizes = new Dictionary<int, CardDef>();

        /// <summary>Your cards you know are still in your Prizes: the known ones not seen anywhere else since (a taken Prize card is in your hand).</summary>
        public List<CardDef> StillPrized(SideView me)
        {
            var list = new List<CardDef>();
            if (KnownPrizes.Count == 0 || me == null) return list;
            var visible = new HashSet<int>();
            foreach (var c in me.Hand) if (c != null) visible.Add(c.Id);
            foreach (var c in me.Discard) if (c != null) visible.Add(c.Id);
            foreach (var c in me.LostZone) if (c != null) visible.Add(c.Id);
            foreach (var mon in InPlay(me))
            {
                foreach (var c in mon.Stack) if (c != null) visible.Add(c.Id);
                foreach (var c in mon.Energy) if (c != null) visible.Add(c.Id);
                foreach (var c in mon.Tools) if (c != null) visible.Add(c.Id);
                foreach (var c in mon.Attached) if (c != null) visible.Add(c.Id);
            }
            foreach (var kv in KnownPrizes) if (!visible.Contains(kv.Key)) list.Add(kv.Value);
            if (list.Count > me.PrizeCount) list.RemoveRange(me.PrizeCount, list.Count - me.PrizeCount);
            return list;
        }

        public PolicyMemory(int seat)
        {
            Seat = seat;
        }

        private GameEngine engine;

        /// <summary>This player's decklist: the one given, or their own starting deck from the game they're in.</summary>
        public List<CardDef> Deck()
        {
            if (DeckList == null && engine != null && Seat >= 0)
            {
                var start = engine.StartingDeck(Seat);
                if (start != null) DeckList = new List<CardDef>(start);
            }
            return DeckList;
        }

        private List<CardDef> factsFor;
        private bool deckTeamEnergy;
        private float deckItemShare;

        /// <summary>
        /// Facts about the whole deck, worked out once per decklist (not on every decision): does a card in it use Energy on your
        /// other Pokémon (PolicyPlan.TeamUsesEnergy), and what share of it is Items.
        /// </summary>
        public bool DeckUsesTeamEnergy { get { DeckFacts(); return deckTeamEnergy; } }
        public float DeckItemShare { get { DeckFacts(); return deckItemShare; } }

        private void DeckFacts()
        {
            var deck = Deck();
            if (deck == null || ReferenceEquals(deck, factsFor)) return;
            deckTeamEnergy = PolicyPlan.TeamUsesEnergy(deck);
            int items = 0;
            foreach (var d in deck) if (d != null && d.IsItem) items++;
            deckItemShare = deck.Count > 0 ? items / (float)deck.Count : 0;
            factsFor = deck;
        }

        public void OnEvent(GameEngine engine, GameEvent ev)
        {
            if (engine != null) this.engine = engine;
            if (Seat < 0 || ev == null) return;
            int opp = 1 - Seat;
            int turn = engine != null && engine.State != null ? engine.State.TurnNumber : Turn;
            switch (ev.Type)
            {
                case GameEventType.TurnStart:
                    if (ev.Player == opp) oppThisTurn.Clear();
                    break;
                case GameEventType.TurnEnd:
                    if (ev.Player == opp)
                    {
                        OppLastTurn.Clear();
                        OppLastTurn.AddRange(oppThisTurn);
                    }
                    break;
                case GameEventType.Reveal:
                    if (ev.Player == opp && ev.Cards != null)
                        foreach (var c in ev.Cards)
                        {
                            if (c == null || c.Def == null) continue;
                            OppKnownHand[c.Id] = c.Def;
                            OppSeen[c.Id] = c.Def;
                        }
                    break;
                case GameEventType.MulliganReveal:
                    if (ev.Player == opp && ev.Cards != null)
                        foreach (var c in ev.Cards)
                            if (c != null && c.Def != null) OppSeen[c.Id] = c.Def;
                    break;
                case GameEventType.Shuffle:
                    // "... shuffled N cards from their hand into their deck": whatever was known about that hand is gone.
                    if (ev.Player == opp && ev.Text != null && ev.Text.IndexOf(" hand into ", StringComparison.Ordinal) >= 0) OppKnownHand.Clear();
                    break;
                case GameEventType.KnockOut:
                    if (ev.Player == 0 || ev.Player == 1) KoTurn[ev.Player] = turn;
                    break;
                case GameEventType.Search:
                    // Searching your deck shows all of it, so you know which of your cards are in the Prizes (a real player
                    // works it out the same way).
                    if (ev.Player == Seat && engine != null && engine.State != null)
                    {
                        KnownPrizes.Clear();
                        foreach (var c in engine.State.Players[Seat].Prizes) if (c != null && c.Def != null) KnownPrizes[c.Id] = c.Def;
                    }
                    break;
            }
            if (ev.Player == opp && ev.Card != null && ev.CardId >= 0)
            {
                OppSeen[ev.CardId] = ev.Card;
                OppKnownHand.Remove(ev.CardId);
                if (ev.Type == GameEventType.PutOnBench || ev.Type == GameEventType.Evolve || ev.Type == GameEventType.AttachEnergy
                    || ev.Type == GameEventType.AttachTool || ev.Type == GameEventType.PlayTrainer)
                {
                    oppThisTurn.Add(ev.Card);
                    if (ev.Type == GameEventType.PlayTrainer && HandShufflers.Contains(ev.Card.Name)) OppKnownHand.Clear();
                }
            }
        }

        /// <summary>Brings the memory up to date with what this view shows (call at every decision).</summary>
        public void See(PlayerView v)
        {
            if (v == null) return;
            if (v.TurnNumber != Turn)
            {
                Turn = v.TurnNumber;
                ActionsThisTurn = 0;
                AbilitiesThisTurn = 0;
                EnergyAttached = SupporterPlayed = Retreated = StadiumPlayed = false;
                AbilityUidsThisTurn.Clear();
                PlayedThisTurn.Clear();
            }
            var opp = v.Opponent;
            if (opp == null) return;
            var visible = new HashSet<int>();
            foreach (var c in opp.Discard) Note(c, visible);
            foreach (var c in opp.LostZone) Note(c, visible);
            foreach (var c in opp.PrizeSlots) if (c != null) Note(c, visible);
            foreach (var hand in opp.MulliganHands) foreach (var c in hand) if (c != null && c.Def != null) OppSeen[c.Id] = c.Def;
            if (opp.HandFaceUp) foreach (var c in opp.Hand) if (c != null && c.Def != null) OppKnownHand[c.Id] = c.Def;
            for (int side = 0; side < 2; side++)
            {
                var s = v.Sides[side];
                if (s == null) continue;
                int idx = side;
                if (s.Active != null && s.Active.Uid != activeUid[idx])
                {
                    if (activeUid[idx] >= 0) movedTurn[s.Active.Uid] = v.TurnNumber;
                    activeUid[idx] = s.Active.Uid;
                }
                foreach (var mon in InPlay(s))
                {
                    int top = mon.Top != null ? mon.Top.Id : -1;
                    int known;
                    if (!topCard.TryGetValue(mon.Uid, out known) || known != top)
                    {
                        // New in play (or evolved) since the last look: on its owner's turn it may have come in this turn;
                        // otherwise it came in during the turn before.
                        enteredTurn[mon.Uid] = v.CurrentPlayer == s.Player ? v.TurnNumber : v.TurnNumber - 1;
                        topCard[mon.Uid] = top;
                    }
                    if (side == opp.Player)
                    {
                        foreach (var c in mon.Stack) Note(c, visible);
                        foreach (var c in mon.Energy) Note(c, visible);
                        foreach (var c in mon.Tools) Note(c, visible);
                        foreach (var c in mon.Attached) Note(c, visible);
                    }
                }
            }
            if (v.Stadium != null && v.StadiumOwner == opp.Player) Note(v.Stadium, visible);
            // A known card that is now somewhere else has left the hand; an empty hand holds nothing.
            if (opp.HandCount == 0) OppKnownHand.Clear();
            else
            {
                var gone = new List<int>();
                foreach (var id in OppKnownHand.Keys) if (visible.Contains(id)) gone.Add(id);
                foreach (var id in gone) OppKnownHand.Remove(id);
                // More known than the hand holds (e.g. a shuffle we couldn't see): keep the most recent ones.
                while (OppKnownHand.Count > opp.HandCount)
                {
                    int first = -1;
                    foreach (var id in OppKnownHand.Keys)
                    {
                        first = id;
                        break;
                    }
                    OppKnownHand.Remove(first);
                }
            }
        }

        private void Note(CardRef c, HashSet<int> visible)
        {
            if (c == null || c.Def == null) return;
            OppSeen[c.Id] = c.Def;
            visible.Add(c.Id);
        }

        public static IEnumerable<PokemonView> InPlay(SideView s)
        {
            if (s.Active != null) yield return s.Active;
            foreach (var b in s.Bench) yield return b;
        }

        /// <summary>The player chose this main action (call after choosing, before the engine carries it out).</summary>
        public void NoteMainAction(DecisionOption o)
        {
            if (o == null) return;
            switch (o.Action)
            {
                case ActionType.EndTurn:
                case ActionType.Attack:
                    return;
            }
            ActionsThisTurn++;
            switch (o.Action)
            {
                case ActionType.AttachEnergy:
                    EnergyAttached = true;
                    break;
                case ActionType.Retreat:
                    Retreated = true;
                    break;
                case ActionType.UseAbility:
                case ActionType.UseHandAbility:
                    AbilitiesThisTurn++;
                    if (o.PokemonUid >= 0) AbilityUidsThisTurn.Add(o.PokemonUid);
                    break;
                case ActionType.PlayTrainer:
                    if (o.Card != null && o.Card.IsSupporter) SupporterPlayed = true;
                    if (o.Card != null && o.Card.IsStadium) StadiumPlayed = true;
                    break;
            }
            if (o.Card != null && (o.Action == ActionType.BenchBasic || o.Action == ActionType.Evolve || o.Action == ActionType.AttachEnergy
                                   || o.Action == ActionType.PlayTrainer))
                PlayedThisTurn.Add(o.Card);
        }

        /// <summary>Makes this memory the same as another (a look-ahead's copy of the player's memory, PolicySearch).</summary>
        public void CopyFrom(PolicyMemory o)
        {
            Seat = o.Seat;
            Copy(o.KnownPrizes, KnownPrizes);
            DeckList = o.DeckList;
            Turn = o.Turn;
            ActionsThisTurn = o.ActionsThisTurn;
            AbilitiesThisTurn = o.AbilitiesThisTurn;
            EnergyAttached = o.EnergyAttached;
            SupporterPlayed = o.SupporterPlayed;
            Retreated = o.Retreated;
            StadiumPlayed = o.StadiumPlayed;
            Copy(o.AbilityUidsThisTurn, AbilityUidsThisTurn);
            Copy(o.PlayedThisTurn, PlayedThisTurn);
            Copy(o.OppLastTurn, OppLastTurn);
            Copy(o.oppThisTurn, oppThisTurn);
            Copy(o.OppKnownHand, OppKnownHand);
            Copy(o.OppSeen, OppSeen);
            Copy(o.enteredTurn, enteredTurn);
            Copy(o.topCard, topCard);
            Copy(o.movedTurn, movedTurn);
            activeUid[0] = o.activeUid[0];
            activeUid[1] = o.activeUid[1];
            KoTurn[0] = o.KoTurn[0];
            KoTurn[1] = o.KoTurn[1];
        }

        private static void Copy<T>(ICollection<T> from, ICollection<T> to)
        {
            to.Clear();
            foreach (var x in from) to.Add(x);
        }

        public bool EnteredBefore(int uid, int turn)
        {
            int t;
            return !enteredTurn.TryGetValue(uid, out t) || t < turn;
        }

        public bool MovedThisTurn(int uid, int turn)
        {
            int t;
            return movedTurn.TryGetValue(uid, out t) && t == turn;
        }
    }

    /// <summary>
    /// Turns a DecisionRequest into the policy network's input (the Python bot's layout). Decisions that pick several cards
    /// are asked one card at a time: Options lists what's left, plus "done" once enough have been picked.
    /// </summary>
    public static class PolicyEncoder
    {
        /// <summary>One option offered to the network: an index into the request's options, or -1 for "done".</summary>
        public struct Choice
        {
            public int Option;
            public bool Done { get { return Option < 0; } }
        }

        public static string DecisionName(DecisionRequest r)
        {
            switch (r.Kind)
            {
                case DecisionKind.MainAction: return "main";
                case DecisionKind.GoFirstOrSecond: return "go_first";
                case DecisionKind.SetupActive: return "setup_active";
                case DecisionKind.SetupBench: return "setup_bench";
                case DecisionKind.PromoteActive: return "promote";
                case DecisionKind.YesNo: return "yes_no";
                case DecisionKind.ChoosePokemon:
                    return Has(r.Prompt, "damage counter") ? "counters" : "pick_pokemon";
                case DecisionKind.ChooseCards:
                    if (r.SeenTitle == "Your deck" || Has(r.Prompt, "search")) return "search";
                    if (Has(r.Prompt, "discard")) return "discard";
                    if (AllEnergyInPlay(r)) return "pick_energy";
                    return "pick_card";
                case DecisionKind.ChoosePrize: return "pick_card";
                default: return "pick_option"; // ChooseOption, ChooseNumber, MulliganDraws, CallCoinFlip
            }
        }

        private static bool Has(string s, string what)
        {
            return s != null && s.IndexOf(what, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool AllEnergyInPlay(DecisionRequest r)
        {
            if (r.Options.Count == 0 || r.View == null) return false;
            foreach (var o in r.Options) if (o.Card == null || !o.Card.IsEnergy) return false;
            return Has(r.Prompt, "energy");
        }

        public static string OptionName(DecisionRequest r, DecisionOption o, int index)
        {
            if (r.Kind == DecisionKind.MainAction)
            {
                switch (o.Action)
                {
                    case ActionType.EndTurn: return "end";
                    case ActionType.Attack: return "attack";
                    case ActionType.UseAbility:
                    case ActionType.UseHandAbility: return "ability";
                    case ActionType.Retreat: return "retreat";
                    case ActionType.BenchBasic:
                    case ActionType.CombineVUnion: return "bench";
                    case ActionType.Evolve: return "evolve";
                    case ActionType.AttachEnergy: return "attach";
                    case ActionType.UseStadium: return "stadium";
                    case ActionType.PlayTrainer: return o.Card != null && o.Card.TrainerKind == TrainerKind.Tool ? "tool" : "play";
                    default: return "play";
                }
            }
            switch (r.Kind)
            {
                case DecisionKind.YesNo:
                    return o.Label != null && o.Label.StartsWith("Y", StringComparison.OrdinalIgnoreCase) ? "yes" : "no";
                case DecisionKind.GoFirstOrSecond:
                    return Has(o.Label, "second") ? "second" : "first";
                case DecisionKind.ChoosePokemon:
                case DecisionKind.PromoteActive:
                    return "pokemon";
                case DecisionKind.SetupActive:
                case DecisionKind.SetupBench:
                case DecisionKind.ChooseCards:
                case DecisionKind.ChoosePrize:
                    return o.PokemonUid >= 0 && o.CardId < 0 ? "pokemon" : "card";
                default:
                    return "choice";
            }
        }

        /// <summary>Main actions in the Python bot's order (End turn, Retreat, Abilities, Stadium, cards from the hand, attacks).</summary>
        private static int MainRank(DecisionOption o)
        {
            switch (o.Action)
            {
                case ActionType.EndTurn: return 0;
                case ActionType.Retreat: return 1;
                case ActionType.UseAbility: return 2;
                case ActionType.UseStadium: return 3;
                case ActionType.Attack: return 5;
                default: return 4;
            }
        }

        /// <summary>
        /// The choices the network is offered at this point of a request: every option for a single pick, or for multi-picks the
        /// options not taken yet (identical cards once) plus "done" once at least MinChoices are picked.
        /// </summary>
        public static List<Choice> Choices(DecisionRequest r, IList<int> picked)
        {
            var list = new List<Choice>();
            bool multi = r.MaxChoices > 1 || r.MinChoices == 0;
            if (!multi)
            {
                var order = new List<int>();
                for (int i = 0; i < r.Options.Count; i++) order.Add(i);
                if (r.Kind == DecisionKind.MainAction)
                    order.Sort((a, b) => MainRank(r.Options[a]) != MainRank(r.Options[b]) ? MainRank(r.Options[a]).CompareTo(MainRank(r.Options[b])) : a.CompareTo(b));
                foreach (int i in order) list.Add(new Choice { Option = i });
                return list;
            }
            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int i = 0; i < r.Options.Count; i++)
            {
                if (picked.Contains(i)) continue;
                var o = r.Options[i];
                string key = o.Card != null ? o.Card.BaseKey + "|" + o.PokemonUid : "#" + i;
                if (o.Card != null && o.PokemonUid < 0 && !seen.Add(key)) continue;
                list.Add(new Choice { Option = i });
            }
            if (picked.Count >= r.MinChoices) list.Add(new Choice { Option = -1 });
            return list;
        }

        /// <summary>The choice standing for option i when identical cards are listed once, or -1.</summary>
        public static int SameAs(DecisionRequest r, List<Choice> choices, int i)
        {
            var o = r.Options[i];
            if (o.Card == null) return -1;
            for (int k = 0; k < choices.Count; k++)
            {
                if (choices[k].Done) continue;
                var c = r.Options[choices[k].Option];
                if (c.Card != null && c.Card.BaseKey == o.Card.BaseKey && c.PokemonUid == o.PokemonUid) return k;
            }
            return -1;
        }

        /// <summary>
        /// Encodes the request with these choices as the options. attacks: what each attack on offer really does, by option
        /// index (PolicySearch.PreviewAttacks), or null to use the printed damage.
        /// </summary>
        public static PolicyInput Encode(DecisionRequest r, PolicyMemory mem, PolicyNet net, List<Choice> choices,
                                         Dictionary<int, PolicySearch.AttackResult> attacks = null)
        {
            var x = new PolicyInput();
            var v = r.View;
            int viewer = v != null ? (v.Viewer < 0 ? r.Player : v.Viewer) : r.Player;
            var me = v != null ? v.Sides[viewer] : null;
            var opp = v != null ? v.Sides[1 - viewer] : null;
            int turn = v != null ? v.TurnNumber : 0;
            var slotOf = new Dictionary<int, int>();

            // What's still to come for you (your hand and the cards left in your deck and Prizes, less the Prize cards you know
            // are there): evolutions and their attacks.
            List<CardDef> toCome = null, unseen = null, deckLeft = null, prized = null;
            bool teamEnergy = false;
            if (me != null)
            {
                unseen = UnseenDefs(v, me, mem);
                prized = mem != null ? mem.StillPrized(me) : new List<CardDef>();
                deckLeft = Less(unseen, prized);
                toCome = ToCome(me, deckLeft);
                teamEnergy = mem != null && mem.DeckUsesTeamEnergy;
            }

            // Board slots: yours, then the opponent's (hidden while setting up).
            if (me != null)
            {
                var gustTarget = v.InSetup ? null : BestGustTarget(me, opp);
                for (int side = 0; side < 2; side++)
                {
                    var s = side == 0 ? me : opp;
                    var other = side == 0 ? opp : me;
                    if (side == 1 && v.InSetup) break;
                    int k = 0;
                    foreach (var mon in PolicyMemory.InPlay(s))
                    {
                        if (k >= PolicyInput.SideSlots) break;
                        int slot = side * PolicyInput.SideSlots + k;
                        slotOf[mon.Uid] = slot;
                        x.SlotCard[slot] = net.RowOf(mon.Top != null ? mon.Top.Def : null);
                        x.SlotTool[slot] = mon.Tool != null ? net.RowOf(mon.Tool.Def) : 0;
                        SlotFeatures(mon, k == 0, turn, mem, x.SlotFeat, slot * PolicyInput.SlotFeatures);
                        RetreatFeatures(mon, x.SlotFeat, slot * PolicyInput.SlotFeatures);
                        GuardFeatures(mon, v.InSetup ? null : other, x.SlotFeat, slot * PolicyInput.SlotFeatures);
                        PlanFeatures(mon, side == 0 ? toCome : null, side == 0 ? me : null, v.InSetup ? null : other,
                                     x.SlotFeat, slot * PolicyInput.SlotFeatures);
                        if (side == 1 && ReferenceEquals(mon, gustTarget))
                            x.SlotFeat[slot * PolicyInput.SlotFeatures + 47] = 1;
                        k++;
                    }
                }
            }

            if (me != null)
            {
                x.Bags[PolicyInput.BagHand] = Rows(net, me.Hand, PolicyInput.BagLimit[PolicyInput.BagHand], false);
                x.Bags[PolicyInput.BagMyDiscard] = Rows(net, me.Discard, PolicyInput.BagLimit[PolicyInput.BagMyDiscard], true);
                x.Bags[PolicyInput.BagOppDiscard] = Rows(net, opp.Discard, PolicyInput.BagLimit[PolicyInput.BagOppDiscard], true);
                x.Bags[PolicyInput.BagUnseen] = Sorted(Unseen(unseen, net), PolicyInput.BagLimit[PolicyInput.BagUnseen]);
                if (mem != null)
                {
                    x.Bags[PolicyInput.BagOppKnownHand] = Sorted(DefRows(net, mem.OppKnownHand.Values), PolicyInput.BagLimit[PolicyInput.BagOppKnownHand]);
                    x.Bags[PolicyInput.BagOppSeen] = Sorted(DefRows(net, mem.OppSeen.Values), PolicyInput.BagLimit[PolicyInput.BagOppSeen]);
                    if (v.CurrentPlayer == viewer)
                        x.Bags[PolicyInput.BagTurnPlayed] = Sorted(DefRows(net, mem.PlayedThisTurn), PolicyInput.BagLimit[PolicyInput.BagTurnPlayed]);
                    x.Bags[PolicyInput.BagOppLastTurn] = Sorted(DefRows(net, mem.OppLastTurn), PolicyInput.BagLimit[PolicyInput.BagOppLastTurn]);
                }
                x.Stadium = v.Stadium != null ? net.RowOf(v.Stadium.Def) : 0;

                var gl = x.Globals;
                gl[0] = me.DeckCount / 60f;
                gl[1] = opp.DeckCount / 60f;
                gl[2] = me.PrizeCount / 6f;
                gl[3] = opp.PrizeCount / 6f;
                gl[4] = me.HandCount / 10f;
                gl[5] = opp.HandCount / 10f;
                gl[6] = me.Discard.Count / 30f;
                gl[7] = opp.Discard.Count / 30f;
                if (mem != null && v.CurrentPlayer == viewer)
                {
                    gl[8] = mem.EnergyAttached ? 1 : 0;
                    gl[9] = mem.SupporterPlayed ? 1 : 0;
                    gl[10] = mem.Retreated ? 1 : 0;
                    gl[11] = mem.StadiumPlayed ? 1 : 0;
                }
                gl[12] = v.CurrentPlayer == viewer ? 1 : 0;
                gl[13] = turn == 1 ? 1 : 0;
                gl[14] = Math.Min(turn, 60) / 60f;
                gl[15] = v.InSetup ? 1 : 0;
                gl[16] = v.Stadium != null && v.StadiumOwner == viewer ? 1 : 0;
                if (mem != null)
                {
                    gl[17] = mem.KoTurn[viewer] == turn - 1 ? 1 : 0;
                    gl[18] = mem.KoTurn[1 - viewer] == turn - 1 ? 1 : 0;
                }
                gl[19] = me.Bench.Count / 8f;

                ItemFeatures(r, v, me, opp, mem, unseen, x.TurnFeats);
                x.TurnFeats[2 + PolicyInput.SideSlots + 7] = Math.Min(FreePivots(me), 3) / 3f;
                if (!v.InSetup) x.TurnFeats[2 + PolicyInput.SideSlots + 8] = Math.Min(FreePivots(opp), 3) / 3f;
                BenchFeatures(v, me, opp, toCome, x.TurnFeats);
                if (!v.InSetup) x.TurnFeats[2 + PolicyInput.SideSlots + 15] = OpponentSpreads(opp, mem) ? 1 : 0;
                if (!v.InSetup) RaceFeatures(me, opp, toCome, x.TurnFeats);
                OutsFeatures(me, opp, mem, deckLeft, prized, x.TurnFeats);
                if (mem != null && v.CurrentPlayer == viewer && !v.InSetup)
                {
                    var tf = x.TurnFeats;
                    tf[0] = Math.Min(mem.ActionsThisTurn, 40) / 20f;
                    tf[1] = mem.AbilitiesThisTurn / 5f;
                    int k = 0;
                    foreach (var mon in PolicyMemory.InPlay(me))
                    {
                        if (k >= PolicyInput.SideSlots) break;
                        tf[2 + k] = mem.AbilityUidsThisTurn.Contains(mon.Uid) ? 1 : 0;
                        k++;
                    }
                }
            }
            int dk = PolicyKinds.Decision(DecisionName(r));
            x.Globals[20 + (dk < 0 ? PolicyKinds.Decision("pick_option") : dk)] = 1;
            x.Source = r.Source != null ? net.RowOf(r.Source) : 0;

            // Options
            x.SetOptions(choices.Count);
            for (int i = 0; i < choices.Count; i++)
            {
                var c = choices[i];
                int f = i * PolicyInput.OptionFeatures;
                x.OptFeat[f + 5] = i / 20f;
                if (c.Done)
                {
                    x.OptKind[i] = PolicyKinds.Option("done");
                    continue;
                }
                var o = r.Options[c.Option];
                string kind = OptionName(r, o, c.Option);
                x.OptKind[i] = PolicyKinds.Option(kind);
                x.OptCard[i] = o.Card != null ? net.RowOf(o.Card) : 0;
                int slot;
                if (o.PokemonUid >= 0 && slotOf.TryGetValue(o.PokemonUid, out slot)) x.OptTarget[i] = slot;
                if (kind == "attack")
                {
                    int ai = Math.Max(0, o.Index);
                    x.OptFeat[f + Math.Min(ai, 1)] = 1;
                    if (o.Card != null && ai < o.Card.Attacks.Count)
                    {
                        var attack = o.Card.Attacks[ai];
                        x.OptFeat[f + 2] = attack.Damage / 300f;
                        // What this attack does to their Active: lock their Items, Knock it Out, how much of its HP it takes.
                        x.OptFeat[f + 13] = PolicyCards.AttackLocksItems(attack) ? 1 : 0;
                        // With a preview, what it really did when played out (every "+", Tool and Stadium counted) and the
                        // Prize cards it took; without, the printed damage and the Active's Prize cards if that Knocks it Out.
                        PolicySearch.AttackResult real;
                        if (attacks != null && attacks.TryGetValue(c.Option, out real) && opp != null && opp.Active != null)
                        {
                            x.OptFeat[f + 14] = real.KnocksOut ? 1 : 0;
                            x.OptFeat[f + 15] = Ratio(real.Damage, opp.Active.RemainingHp);
                            x.OptFeat[f + 21] = Math.Min(real.Prizes, 3) / 3f;
                        }
                        else if (me != null && opp != null && me.Active != null && opp.Active != null && !v.InSetup)
                        {
                            int damage = PolicyPlan.DamageTo(o.Card, me.Active.AbilitiesActive, attack, opp.Active);
                            bool ko = damage > 0 && damage >= opp.Active.RemainingHp;
                            x.OptFeat[f + 14] = ko ? 1 : 0;
                            x.OptFeat[f + 15] = Ratio(damage, opp.Active.RemainingHp);
                            x.OptFeat[f + 21] = ko ? Math.Min(PrizesFor(opp.Active), 3) / 3f : 0;
                        }
                    }
                }
                else if (kind == "choice")
                {
                    int data = r.Kind == DecisionKind.ChooseNumber && o.Index >= 0 ? o.Index : c.Option;
                    x.OptFeat[f + 3] = data / 5f;
                }
                if (o.Card != null && me != null)
                {
                    int copies = 0;
                    foreach (var h in me.Hand) if (h.Def != null && h.Def.BaseKey == o.Card.BaseKey) copies++;
                    x.OptFeat[f + 4] = copies / 4f;
                }
                if (me != null && o.Card != null && o.PokemonUid >= 0 && r.Kind == DecisionKind.MainAction
                    && (o.Action == ActionType.AttachEnergy || o.Action == ActionType.Evolve))
                    OptionPlan(o, me, v.InSetup ? null : opp, toCome, teamEnergy, x.OptFeat, f);
                if (me != null && o.Card != null && o.Card.IsPokemon && o.Action == ActionType.BenchBasic)
                    BenchOption(o.Card, me, toCome, x.OptFeat, f);
            }
            return x;
        }

        /// <summary>True when this Energy attachment (a main action) would be wasted (PolicyPlan.Wasted).</summary>
        public static bool WastedAttachment(DecisionRequest r, PolicyMemory mem, DecisionOption o)
        {
            var v = r.View;
            if (v == null || o.Card == null || !o.Card.IsEnergy) return false;
            int viewer = v.Viewer < 0 ? r.Player : v.Viewer;
            var me = v.Sides[viewer];
            bool active;
            var target = Mine(me, o.PokemonUid, out active);
            if (target == null || target.Top == null || target.Top.Def == null) return false;
            var prized = mem != null ? mem.StillPrized(me) : new List<CardDef>();
            var toCome = ToCome(me, Less(UnseenDefs(v, me, mem), prized));
            var line = PolicyPlan.Line(target.Top.Def, toCome);
            bool team = mem != null && mem.DeckUsesTeamEnergy;
            return PolicyPlan.Wasted(target.EnergyUnits, o.Card, line, target.RetreatCost, active, team);
        }

        /// <summary>Your Pokémon with this Uid in play, or null.</summary>
        private static PokemonView Mine(SideView me, int uid, out bool active)
        {
            active = me.Active != null && me.Active.Uid == uid;
            foreach (var mon in PolicyMemory.InPlay(me)) if (mon.Uid == uid) return mon;
            return null;
        }

        /// <summary>
        /// An Energy attachment's or an evolution's numbers (option features 6 to 12): does the Energy help an attack (now or
        /// after evolving) or the Active Pokémon's retreat, does it make an attack usable, is it wasted (PolicyPlan.Wasted),
        /// does its bonus need this Pokémon's type and get it (Growing Grass Energy on a Grass Pokémon), is it the Pokémon's
        /// own type, does it make an attack usable that Knocks Out their Active; for an evolution, the Energy its strongest
        /// attack would still need.
        /// </summary>
        private static void OptionPlan(DecisionOption o, SideView me, SideView opp, List<CardDef> toCome, bool teamEnergy, float[] feat, int f)
        {
            bool active;
            var target = Mine(me, o.PokemonUid, out active);
            if (target == null || target.Top == null || target.Top.Def == null) return;
            var top = target.Top.Def;
            if (o.Action == ActionType.AttachEnergy && o.Card.IsEnergy)
            {
                int unit = PolicyPlan.UnitOf(o.Card);
                var line = PolicyPlan.Line(top, toCome);
                feat[f + 6] = PolicyPlan.Helps(target.EnergyUnits, unit, line, target.RetreatCost, active) ? 1 : 0;
                feat[f + 7] = PolicyPlan.Completes(target.EnergyUnits, unit, top) ? 1 : 0;
                feat[f + 8] = PolicyPlan.Wasted(target.EnergyUnits, o.Card, line, target.RetreatCost, active, teamEnergy) ? 1 : 0;
                int bonus = PolicyCards.BonusType(o.Card);
                bool ownType = PolicyCards.TypeIndex(top.PokemonType) == bonus
                               || (top.SecondType.HasValue && PolicyCards.TypeIndex(top.SecondType.Value) == bonus);
                feat[f + 9] = bonus >= 0 && ownType ? 1 : 0;
                feat[f + 10] = o.Card.EnergyProvides == top.PokemonType
                               || (top.SecondType.HasValue && o.Card.EnergyProvides == top.SecondType.Value) ? 1 : 0;
                if (opp != null && opp.Active != null && PolicyPlan.Completes(target.EnergyUnits, unit, top))
                {
                    feat[f + 11] = PolicyPlan.CanKnockOut(top, new List<int>(target.EnergyUnits) { unit }, 0, opp.Active, target.AbilitiesActive) ? 1 : 0;
                }
                feat[f + 20] = PolicyPlan.Blocks(o.Card, top) && !PolicyPlan.EffectsBlocked(target) ? 1 : 0;
            }
            else if (o.Action == ActionType.Evolve && o.Card.IsPokemon)
            {
                int missing = PolicyPlan.StrongestMissing(target.EnergyUnits, new[] { o.Card });
                feat[f + 12] = missing < 0 ? 0 : Math.Min(missing, 4) / 4f;
            }
        }

        private static void SlotFeatures(PokemonView mon, bool active, int turn, PolicyMemory mem, float[] f, int o)
        {
            f[o + 0] = 1;
            f[o + 1] = active ? 1 : 0;
            int damage = mon.DamageCounters * 10;
            f[o + 2] = (mon.MaxHp - damage) / 300f;
            f[o + 3] = damage / 300f;
            f[o + 4] = mon.MaxHp / 300f;
            foreach (int unit in mon.EnergyUnits)
            {
                int type = SingleType(unit);
                f[o + 5 + (type < 0 ? 9 : type)] += 0.25f;
            }
            f[o + 15] = mon.Energy.Count / 4f;
            var cond = mon.Conditions;
            f[o + 16] = (cond & SpecialCondition.Poisoned) != 0 ? 1 : 0;
            f[o + 17] = (cond & SpecialCondition.Burned) != 0 ? 1 : 0;
            f[o + 18] = (cond & SpecialCondition.Asleep) != 0 ? 1 : 0;
            f[o + 19] = (cond & SpecialCondition.Paralyzed) != 0 ? 1 : 0;
            f[o + 20] = (cond & SpecialCondition.Confused) != 0 ? 1 : 0;
            f[o + 21] = mem == null || mem.EnteredBefore(mon.Uid, turn) ? 1 : 0;
            f[o + 22] = mem != null && mem.MovedThisTurn(mon.Uid, turn) ? 1 : 0;
            foreach (var note in mon.Notes)
            {
                if (note == null) continue;
                if (Has(note, "can't attack") || Has(note, "cannot attack")) f[o + 23] = 1;
                if (Has(note, "prevent") || Has(note, "less damage") || Has(note, "damage taken")) f[o + 24] = 1;
            }
            f[o + 25] = (mon.Stack.Count - 1) / 2f;
        }

        /// <summary>
        /// A Pokémon's plan numbers (slot features 26 to 39). What it needs: can it attack now, the Energy missing for its
        /// cheapest and its strongest attack, Energy beyond what any attack or its retreat needs, for yours (toCome: your hand and
        /// deck) an evolution in hand, how many are left in the deck and the Energy its line's strongest attack needs, and
        /// whether its cards do more with Energy than pay for attacks. What it faces (other: the other side): can their Active
        /// Knock it Out next turn (with at most one more Energy) and how much of its HP that takes, can any of theirs, and can it
        /// Knock Out their Active now or with one more Energy, and how much of that Pokémon's HP its best attack takes.
        /// </summary>
        private static void PlanFeatures(PokemonView mon, List<CardDef> toCome, SideView mine, SideView other, float[] f, int o)
        {
            var top = mon.Top != null ? mon.Top.Def : null;
            if (top == null) return;
            var units = mon.EnergyUnits;
            int cheapest = PolicyPlan.Cheapest(units, top);
            int strongest = PolicyPlan.StrongestMissing(units, new[] { top });
            f[o + 26] = cheapest == 0 ? 1 : 0;
            f[o + 27] = Math.Max(0, Math.Min(cheapest, 4)) / 4f;
            f[o + 28] = Math.Max(0, Math.Min(strongest, 4)) / 4f;
            f[o + 29] = Math.Min(Math.Max(0, units.Count - Math.Max(PolicyPlan.MostCost(top), mon.RetreatCost)), 4) / 4f;
            var line = PolicyPlan.Line(top, toCome);
            if (mine != null)
            {
                int inHand = 0;
                foreach (var c in mine.Hand) if (c != null && c.Def != null && c.Def.IsPokemon && c.Def.EvolvesFrom == top.Name) inHand++;
                f[o + 30] = inHand > 0 ? 1 : 0;
                f[o + 31] = Math.Min(PolicyPlan.EvolutionsOf(top.Name, toCome) - inHand, 4) / 4f;
                int lineMissing = PolicyPlan.StrongestMissing(units, line);
                f[o + 32] = Math.Max(0, Math.Min(lineMissing, 4)) / 4f;
            }
            else f[o + 32] = f[o + 28];
            foreach (var c in line)
                if (PolicyCards.MentionsEnergy(c)) f[o + 33] = 1;
            if (other == null) return;
            if (other.Active != null)
            {
                int theirs = PolicyPlan.BestDamage(other.Active, 1, mon);
                f[o + 34] = theirs > 0 && theirs >= mon.RemainingHp ? 1 : 0;
                f[o + 35] = Ratio(theirs, mon.RemainingHp);
                f[o + 37] = PolicyPlan.CanKnockOut(mon, 0, other.Active) ? 1 : 0;
                f[o + 38] = PolicyPlan.CanKnockOut(mon, 1, other.Active) ? 1 : 0;
                f[o + 39] = Ratio(PolicyPlan.BestDamage(mon, 1, other.Active), other.Active.RemainingHp);
            }
            foreach (var them in PolicyMemory.InPlay(other))
                if (PolicyPlan.CanKnockOut(them, 1, mon)) f[o + 36] = 1;
        }

        /// <summary>
        /// A Pokémon's protection numbers (slot features 43 to 46): its Abilities have an effect now (not turned off, as by
        /// Legendary Pyroduct), the other side's Active can't damage it (its protection stops every damaging attack of theirs),
        /// it can't damage their Active, and an attached card stops the effects of attacks on it (Mist Energy).
        /// </summary>
        private static void GuardFeatures(PokemonView mon, SideView other, float[] f, int o)
        {
            var top = mon.Top != null ? mon.Top.Def : null;
            if (top == null) return;
            f[o + 43] = mon.AbilitiesActive && PolicyPlan.HasAbility(top) ? 1 : 0;
            if (other != null && other.Active != null && other.Active.Top != null)
            {
                f[o + 44] = PolicyPlan.AllShielded(other.Active.Top.Def, other.Active.AbilitiesActive, mon) ? 1 : 0;
                f[o + 45] = PolicyPlan.AllShielded(top, mon.AbilitiesActive, other.Active) ? 1 : 0;
            }
            f[o + 46] = PolicyPlan.EffectsBlocked(mon) ? 1 : 0;
        }

        /// <summary>A Pokémon's retreat numbers (slot features 40 to 42): its Retreat Cost now, free retreat, Energy enough to pay it.</summary>
        private static void RetreatFeatures(PokemonView mon, float[] f, int o)
        {
            f[o + 40] = Math.Min(mon.RetreatCost, 4) / 4f;
            f[o + 41] = mon.RetreatCost == 0 ? 1 : 0;
            f[o + 42] = mon.EnergyUnits.Count >= mon.RetreatCost ? 1 : 0;
        }

        /// <summary>
        /// The Bench and play-from-hand numbers (turn features from 2 + SideSlots + 9 on): your Bench limit and free spaces, your
        /// Pokémon with an Ability that works when played from your hand (Meowth ex) in your hand and still in your deck (they
        /// need a free space to be any use), your Supporters in hand, and the opponent's free Bench spaces. A full Bench can't
        /// take the Pokémon you'll want later.
        /// </summary>
        private static void BenchFeatures(PlayerView v, SideView me, SideView opp, List<CardDef> toCome, float[] tf)
        {
            int o = 2 + PolicyInput.SideSlots + 9;
            tf[o] = Math.Min(me.BenchLimit, 8) / 8f;
            tf[o + 1] = Math.Max(0, Math.Min(me.BenchLimit - me.Bench.Count, 8)) / 8f;
            int inHand = 0, supporters = 0;
            foreach (var c in me.Hand)
            {
                if (c == null || c.Def == null) continue;
                if (PolicyCards.PlaysFromHand(c.Def)) inHand++;
                if (c.Def.IsTrainer && c.Def.TrainerKind == TrainerKind.Supporter) supporters++;
            }
            int all = 0;
            if (toCome != null) foreach (var d in toCome) if (PolicyCards.PlaysFromHand(d)) all++;
            tf[o + 2] = Math.Min(inHand, 2) / 2f;
            tf[o + 3] = Math.Min(Math.Max(0, all - inHand), 4) / 4f;
            tf[o + 4] = Math.Min(supporters, 4) / 4f;
            if (!v.InSetup) tf[o + 5] = Math.Max(0, Math.Min(opp.BenchLimit - opp.Bench.Count, 8)) / 8f;
        }

        /// <summary>True when the opponent has shown a Pokémon with an attack that puts damage counters on Pokémon (spread).</summary>
        private static bool OpponentSpreads(SideView opp, PolicyMemory mem)
        {
            Func<CardDef, bool> spreads = PolicyCards.Spreads;
            foreach (var mon in PolicyMemory.InPlay(opp)) if (mon.Top != null && spreads(mon.Top.Def)) return true;
            foreach (var c in opp.Discard) if (c != null && spreads(c.Def)) return true;
            if (mem != null) foreach (var d in mem.OppSeen.Values) if (spreads(d)) return true;
            return false;
        }

        /// <summary>
        /// Benching a Basic (option features 16 to 19): does its Ability work as it's played (Meowth ex), the Bench spaces left
        /// after it, do evolutions for it remain (in hand or deck), and how many of it are in play already.
        /// </summary>
        private static void BenchOption(CardDef card, SideView me, List<CardDef> toCome, float[] feat, int f)
        {
            feat[f + 16] = PolicyCards.PlaysFromHand(card) ? 1 : 0;
            feat[f + 17] = Math.Max(0, Math.Min(me.BenchLimit - me.Bench.Count - 1, 8)) / 8f;
            feat[f + 18] = PolicyPlan.EvolutionsOf(card.Name, toCome) > 0 ? 1 : 0;
            int copies = 0;
            foreach (var mon in PolicyMemory.InPlay(me)) if (mon.Top != null && mon.Top.Def != null && mon.Top.Def.Name == card.Name) copies++;
            feat[f + 19] = Math.Min(copies, 3) / 3f;
        }

        /// <summary>Free-retreat Pokémon on a Bench (pivots: one can come in and go out again for nothing).</summary>
        private static int FreePivots(SideView s)
        {
            int n = 0;
            foreach (var b in s.Bench) if (b != null && b.RetreatCost == 0) n++;
            return n;
        }

        /// <summary>
        /// Items (turn features from 2 + SideSlots on): are you locked out of Items now (an Item lock this turn, or Items in hand
        /// that none of the options can play), is the opponent locked on their next turn, how many Items you hold, what share of
        /// the opponent's cards seen so far are Items and how many, and how many Items are left in your deck and what share of
        /// your deck they make. With these and the attacks' Item-lock flag the network can weigh locking their Items against
        /// the other things an attack could do.
        /// </summary>
        private static void ItemFeatures(DecisionRequest r, PlayerView v, SideView me, SideView opp, PolicyMemory mem, List<CardDef> unseen, float[] tf)
        {
            int o = 2 + PolicyInput.SideSlots;
            int turn = v.TurnNumber;
            int itemsInHand = 0;
            foreach (var c in me.Hand) if (c != null && c.Def != null && c.Def.IsItem) itemsInHand++;
            bool locked = me.ItemLockTurn == turn && v.CurrentPlayer == me.Player;
            if (!locked && itemsInHand > 0 && r.Kind == DecisionKind.MainAction && v.CurrentPlayer == me.Player)
            {
                bool playable = false;
                foreach (var op in r.Options)
                    if (op.Action == ActionType.PlayTrainer && op.Card != null && op.Card.IsItem) playable = true;
                locked = !playable;
            }
            tf[o] = locked ? 1 : 0;
            int theirTurn = v.CurrentPlayer == me.Player ? turn + 1 : turn;
            tf[o + 1] = opp.ItemLockTurn == theirTurn ? 1 : 0;
            tf[o + 2] = Math.Min(itemsInHand, 5) / 5f;
            int seen = 0, seenItems = 0;
            var counted = new HashSet<int>();
            Action<int, CardDef> count = (id, d) =>
            {
                if (d == null || !counted.Add(id)) return;
                seen++;
                if (d.IsItem) seenItems++;
            };
            foreach (var c in opp.Discard) if (c != null) count(c.Id, c.Def);
            if (mem != null) foreach (var kv in mem.OppSeen) count(kv.Key, kv.Value);
            tf[o + 3] = seen > 0 ? seenItems / (float)seen : 0;
            tf[o + 4] = Math.Min(seenItems, 10) / 10f;
            int leftItems = 0;
            foreach (var d in unseen) if (d.IsItem) leftItems++;
            tf[o + 5] = Math.Min(leftItems, 10) / 10f;
            if (mem != null) tf[o + 6] = mem.DeckItemShare;
        }

        /// <summary>Your hand and these cards still to come from your deck.</summary>
        private static List<CardDef> ToCome(SideView me, List<CardDef> deckLeft)
        {
            var list = new List<CardDef>();
            foreach (var c in me.Hand) if (c != null && c.Def != null) list.Add(c.Def);
            list.AddRange(deckLeft);
            return list;
        }

        /// <summary>These cards less those (one for one, matched by card).</summary>
        private static List<CardDef> Less(List<CardDef> cards, List<CardDef> less)
        {
            if (less == null || less.Count == 0) return cards;
            var take = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var d in less)
            {
                int n;
                string key = d.BaseKey ?? d.Name;
                take.TryGetValue(key, out n);
                take[key] = n + 1;
            }
            var list = new List<CardDef>();
            foreach (var d in cards)
            {
                int n;
                string key = d.BaseKey ?? d.Name;
                if (take.TryGetValue(key, out n) && n > 0) take[key] = n - 1;
                else list.Add(d);
            }
            return list;
        }

        /// <summary>Prize cards the other player takes for Knocking Out this Pokémon.</summary>
        private static int PrizesFor(PokemonView mon)
        {
            return mon.Top != null && mon.Top.Def != null ? mon.Top.Def.PrizeValue : 1;
        }

        /// <summary>
        /// The opponent's Benched Pokémon most worth bringing up to Knock Out (slot feature 47): of those your Active could
        /// Knock Out with at most one more Energy, the one giving the most Prize cards (the first on a tie). Null if none.
        /// </summary>
        private static PokemonView BestGustTarget(SideView me, SideView opp)
        {
            if (me.Active == null || opp == null) return null;
            PokemonView best = null;
            foreach (var b in opp.Bench)
                if (b != null && PolicyPlan.CanKnockOut(me.Active, 1, b) && (best == null || PrizesFor(b) > PrizesFor(best))) best = b;
            return best;
        }

        /// <summary>
        /// The Prize race (turn features from 2 + SideSlots + 16 on, and 33 and 34): the Knock Outs each side needs to take its
        /// last Prize card, the Pokémon worth 2 or more Prize cards each side has in play, the turns each side would need to
        /// win and who is ahead in that race, and the Prize cards each side could take in its next 2 turns (TurnsToWin). A
        /// board set up to win in 1 or 2 turns looks different here from one that needs 3 or 4.
        /// </summary>
        private static void RaceFeatures(SideView me, SideView opp, List<CardDef> toCome, float[] tf)
        {
            int o = 2 + PolicyInput.SideSlots + 16;
            int myKos, theirKos, mySoon, theirSoon;
            int mine = TurnsToWin(me, opp, toCome, out myKos, out mySoon), theirs = TurnsToWin(opp, me, null, out theirKos, out theirSoon);
            tf[o] = Math.Min(myKos, 6) / 6f;
            tf[o + 1] = Math.Min(theirKos, 6) / 6f;
            tf[o + 2] = Math.Min(MultiPrize(me), 3) / 3f;
            tf[o + 3] = Math.Min(MultiPrize(opp), 3) / 3f;
            tf[o + 4] = Math.Min(mine, 8) / 8f;
            tf[o + 5] = Math.Min(theirs, 8) / 8f;
            tf[o + 6] = (Math.Max(-4, Math.Min(4, theirs - mine)) + 4) / 8f;
            tf[o + 17] = Math.Min(mySoon, 6) / 6f;
            tf[o + 18] = Math.Min(theirSoon, 6) / 6f;
        }

        private static int MultiPrize(SideView s)
        {
            int n = 0;
            foreach (var mon in PolicyMemory.InPlay(s)) if (PrizesFor(mon) >= 2) n++;
            return n;
        }

        /// <summary>Most turns counted for one Knock Out, and the turns ahead in which the board keeps powering up.</summary>
        private const int MaxTurnsPerKo = 3, PowerUpTurns = 3;

        /// <summary>
        /// Turns the attacker's side needs to take its remaining Prize cards from the defender's Pokémon in play, played out a
        /// turn at a time: each turn its best attacker hits one target, with one more Energy attached each turn (one
        /// attachment a turn, up to PowerUpTurns more) and, for yours (toCome), as the evolutions still to come in its line;
        /// damage stays on the target from turn to turn, so a Pokémon needing two hits takes two turns. The next target is
        /// the one giving the most Prize cards per turn spent on it (a 2-Prize Pokémon ex in one hit before a 1-Prize one;
        /// no more Prize cards counted than are left). A target nothing can damage counts MaxTurnsPerKo turns; Prize cards
        /// left after the Pokémon in play count a turn and a Knock Out each. kos: the Knock Outs that takes; soon: the Prize
        /// cards taken within its next 2 turns.
        /// </summary>
        private static int TurnsToWin(SideView attacker, SideView defender, List<CardDef> toCome, out int kos, out int soon)
        {
            var targets = new List<PokemonView>(PolicyMemory.InPlay(defender));
            // damage[t, e]: the most any attacker does to target t with e + 1 more Energy.
            var damage = new int[targets.Count, PowerUpTurns];
            var hp = new int[targets.Count];
            foreach (var mon in PolicyMemory.InPlay(attacker))
            {
                var top = mon.Top != null ? mon.Top.Def : null;
                if (top == null) continue;
                var cards = toCome != null ? PolicyPlan.Line(top, toCome) : new List<CardDef> { top };
                for (int t = 0; t < targets.Count; t++)
                    foreach (var c in cards)
                        for (int e = 0; e < PowerUpTurns; e++)
                            damage[t, e] = Math.Max(damage[t, e], PolicyPlan.BestDamage(c, mon.EnergyUnits, e + 1, targets[t], mon.AbilitiesActive));
            }
            for (int t = 0; t < targets.Count; t++) hp[t] = targets[t].RemainingHp;
            var taken = new bool[targets.Count];
            int left = attacker.PrizeCount, turns = 0;
            kos = soon = 0;
            while (left > 0)
            {
                int pick = -1, pickNeed = 0, pickPrizes = 0;
                for (int t = 0; t < targets.Count; t++)
                {
                    if (taken[t]) continue;
                    int need = TurnsFor(damage, t, hp[t], turns);
                    int prizes = Math.Min(PrizesFor(targets[t]), left);
                    // More Prize cards per turn first (prizes / need, compared without dividing), then the quicker one.
                    if (pick < 0 || prizes * pickNeed > pickPrizes * need || (prizes * pickNeed == pickPrizes * need && need < pickNeed))
                    {
                        pick = t;
                        pickNeed = need;
                        pickPrizes = prizes;
                    }
                }
                if (pick < 0) break;
                taken[pick] = true;
                turns += pickNeed;
                left -= pickPrizes;
                kos++;
                if (turns <= 2) soon += pickPrizes;
            }
            if (left > 0)
            {
                if (turns < 2) soon += Math.Min(left, 2 - turns);
                turns += left;
                kos += left;
            }
            return turns;
        }

        /// <summary>Turns to Knock Out target t (hp left) starting turns from now, its damage adding up (at most MaxTurnsPerKo).</summary>
        private static int TurnsFor(int[,] damage, int t, int hp, int from)
        {
            for (int n = 1; n <= MaxTurnsPerKo; n++)
            {
                hp -= Math.Max(0, damage[t, Math.Min(from + n - 1, PowerUpTurns - 1)]);
                if (hp <= 0) return n;
            }
            return MaxTurnsPerKo;
        }

        /// <summary>
        /// Outs and resources (turn features from 2 + SideSlots + 23 on): your Prize cards you know of, the Supporters and
        /// Energy left in your deck (less those known to be Prizes) and the chance your next draw is a Supporter, the cards
        /// that bring up an opponent's Benched Pokémon (Boss's Orders) in your hand and left in your deck, the cards that
        /// switch your Active in your hand, and of the opponent's: such gust and switch cards seen so far, and whether their
        /// hand is known to hold a gust card.
        /// </summary>
        private static void OutsFeatures(SideView me, SideView opp, PolicyMemory mem, List<CardDef> deckLeft, List<CardDef> prized, float[] tf)
        {
            int o = 2 + PolicyInput.SideSlots + 23;
            tf[o] = Math.Min(prized.Count, 6) / 6f;
            int supporters = 0, energy = 0, gusts = 0;
            foreach (var d in deckLeft)
            {
                if (d.IsTrainer && d.TrainerKind == TrainerKind.Supporter) supporters++;
                if (d.IsEnergy) energy++;
                if (PolicyCards.Gusts(d)) gusts++;
            }
            tf[o + 1] = Math.Min(supporters, 10) / 10f;
            tf[o + 2] = Math.Min(1f, supporters / (float)Math.Max(1, me.DeckCount));
            tf[o + 3] = Math.Min(energy, 15) / 15f;
            int gustsInHand = 0, switchesInHand = 0;
            foreach (var c in me.Hand)
            {
                if (c == null || c.Def == null) continue;
                if (PolicyCards.Gusts(c.Def)) gustsInHand++;
                if (PolicyCards.SwitchesOwn(c.Def)) switchesInHand++;
            }
            tf[o + 4] = Math.Min(gustsInHand, 2) / 2f;
            tf[o + 5] = Math.Min(gusts, 4) / 4f;
            tf[o + 6] = Math.Min(switchesInHand, 2) / 2f;
            int theirGusts = 0, theirSwitches = 0;
            var counted = new HashSet<int>();
            Action<int, CardDef> count = (id, d) =>
            {
                if (d == null || !counted.Add(id)) return;
                if (PolicyCards.Gusts(d)) theirGusts++;
                if (PolicyCards.SwitchesOwn(d)) theirSwitches++;
            };
            foreach (var c in opp.Discard) if (c != null) count(c.Id, c.Def);
            if (mem != null) foreach (var kv in mem.OppSeen) count(kv.Key, kv.Value);
            tf[o + 7] = Math.Min(theirGusts, 4) / 4f;
            tf[o + 8] = Math.Min(theirSwitches, 4) / 4f;
            if (mem != null) foreach (var d in mem.OppKnownHand.Values) if (PolicyCards.Gusts(d)) tf[o + 9] = 1;
        }

        /// <summary>Damage as a share of the HP left, 0 to 1 (1 = twice what's needed or more).</summary>
        private static float Ratio(int damage, int hp)
        {
            if (damage <= 0 || hp <= 0) return 0;
            return Math.Min(2f, damage / (float)hp) / 2f;
        }

        /// <summary>The Python type index of a unit that provides exactly one type, else -1 (any-type and multi-type units).</summary>
        private static int SingleType(int unit)
        {
            if (unit == 0 || (unit & (unit - 1)) != 0) return -1;
            foreach (EnergyType t in Enum.GetValues(typeof(EnergyType)))
                if (EnergyMask.Of(t) == unit) return PolicyCards.TypeIndex(t);
            return -1;
        }

        private static int[] Rows(PolicyNet net, List<CardRef> cards, int limit, bool last)
        {
            int n = Math.Min(limit, cards.Count);
            var rows = new int[n];
            int start = last ? cards.Count - n : 0;
            for (int i = 0; i < n; i++) rows[i] = net.RowOf(cards[start + i].Def);
            return rows;
        }

        private static List<int> DefRows(PolicyNet net, IEnumerable<CardDef> defs)
        {
            var list = new List<int>();
            foreach (var d in defs) if (d != null) list.Add(net.RowOf(d));
            return list;
        }

        private static int[] Sorted(List<int> rows, int limit)
        {
            rows.Sort();
            if (rows.Count > limit) rows.RemoveRange(limit, rows.Count - limit);
            return rows.ToArray();
        }

        /// <summary>Your decklist minus every card of yours you can see: what's still in your deck and Prize cards.</summary>
        private static List<int> Unseen(List<CardDef> unseen, PolicyNet net)
        {
            var rows = new List<int>();
            foreach (var d in unseen) rows.Add(net.RowOf(d));
            return rows;
        }

        /// <summary>Your cards you haven't seen yet: your deck list less the cards in your hand, discard pile, Lost Zone, Prizes you've seen and in play.</summary>
        private static List<CardDef> UnseenDefs(PlayerView v, SideView me, PolicyMemory mem)
        {
            var left = new Dictionary<string, int>(StringComparer.Ordinal);
            var defs = new Dictionary<string, CardDef>(StringComparer.Ordinal);
            var deck = mem == null ? null : mem.Deck();
            if (deck == null) return new List<CardDef>();
            foreach (var d in deck)
            {
                if (d == null) continue;
                string key = d.BaseKey ?? d.Name;
                int n;
                left.TryGetValue(key, out n);
                left[key] = n + 1;
                defs[key] = d;
            }
            Action<CardRef> take = c =>
            {
                if (c == null || c.Def == null) return;
                string key = c.Def.BaseKey ?? c.Def.Name;
                int n;
                if (left.TryGetValue(key, out n) && n > 0) left[key] = n - 1;
            };
            foreach (var c in me.Hand) take(c);
            foreach (var c in me.Discard) take(c);
            foreach (var c in me.LostZone) take(c);
            foreach (var c in me.PrizeSlots) take(c);
            foreach (var mon in PolicyMemory.InPlay(me))
            {
                foreach (var c in mon.Stack) take(c);
                foreach (var c in mon.Energy) take(c);
                foreach (var c in mon.Tools) take(c);
                foreach (var c in mon.Attached) take(c);
            }
            if (v.Stadium != null && v.StadiumOwner == me.Player) take(v.Stadium);
            var cards = new List<CardDef>();
            foreach (var kv in left) for (int i = 0; i < kv.Value; i++) cards.Add(defs[kv.Key]);
            return cards;
        }
    }
}
