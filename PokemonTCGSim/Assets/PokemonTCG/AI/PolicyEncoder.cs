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

        /// <summary>Encodes the request with these choices as the options.</summary>
        public static PolicyInput Encode(DecisionRequest r, PolicyMemory mem, PolicyNet net, List<Choice> choices)
        {
            var x = new PolicyInput();
            var v = r.View;
            int viewer = v != null ? (v.Viewer < 0 ? r.Player : v.Viewer) : r.Player;
            var me = v != null ? v.Sides[viewer] : null;
            var opp = v != null ? v.Sides[1 - viewer] : null;
            int turn = v != null ? v.TurnNumber : 0;
            var slotOf = new Dictionary<int, int>();

            // Board slots: yours, then the opponent's (hidden while setting up).
            if (me != null)
            {
                for (int side = 0; side < 2; side++)
                {
                    var s = side == 0 ? me : opp;
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
                        k++;
                    }
                }
            }

            if (me != null)
            {
                x.Bags[PolicyInput.BagHand] = Rows(net, me.Hand, PolicyInput.BagLimit[PolicyInput.BagHand], false);
                x.Bags[PolicyInput.BagMyDiscard] = Rows(net, me.Discard, PolicyInput.BagLimit[PolicyInput.BagMyDiscard], true);
                x.Bags[PolicyInput.BagOppDiscard] = Rows(net, opp.Discard, PolicyInput.BagLimit[PolicyInput.BagOppDiscard], true);
                x.Bags[PolicyInput.BagUnseen] = Sorted(Unseen(v, me, mem, net), PolicyInput.BagLimit[PolicyInput.BagUnseen]);
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
                    if (o.Card != null && ai < o.Card.Attacks.Count) x.OptFeat[f + 2] = o.Card.Attacks[ai].Damage / 300f;
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
            }
            return x;
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
        private static List<int> Unseen(PlayerView v, SideView me, PolicyMemory mem, PolicyNet net)
        {
            var left = new Dictionary<int, int>();
            var deck = mem == null ? null : mem.Deck();
            if (deck == null) return new List<int>();
            foreach (var d in deck)
            {
                int row = net.RowOf(d);
                int n;
                left.TryGetValue(row, out n);
                left[row] = n + 1;
            }
            Action<CardRef> take = c =>
            {
                if (c == null || c.Def == null) return;
                int row = net.RowOf(c.Def);
                int n;
                if (left.TryGetValue(row, out n) && n > 0) left[row] = n - 1;
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
            var rows = new List<int>();
            foreach (var kv in left) for (int i = 0; i < kv.Value; i++) rows.Add(kv.Key);
            return rows;
        }
    }
}
