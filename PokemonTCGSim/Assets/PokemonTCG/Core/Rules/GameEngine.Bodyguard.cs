using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Marowak's Bodyguard (Fates Collide, v94): "Prevent all effects of attacks done to you or your hand by your opponent's
    /// Pokémon. Remove any existing effects." Your ruling: it covers the opponent's attack effects on the player (Item,
    /// Supporter, Stadium and Pokémon locks, Trainer flips, the Item and evolution locks) and on the hand, revealing included
    /// (discarding, shuffling, moving or showing cards from the hand). Not the deck, the Prize cards or the player's Pokémon.
    /// </summary>
    public sealed partial class GameEngine
    {
        /// <summary>Something on this player's side (Bodyguard) shields them and their hand from the opponent's attacks' effects.</summary>
        public bool PlayerShieldedFromAttacks(int player)
        {
            if (player < 0 || player > 1) return false;
            foreach (var src in Sources())
                if (src.Ctx.Player == player && src.Script.ShieldsPlayerFromAttackEffects(src.Ctx)) return true;
            return false;
        }

        /// <summary>
        /// An effect of the opponent's attack that would work on this player, which Bodyguard keeps from doing anything while it
        /// shields them ("Remove any existing effects": one already in place stops working too).
        /// </summary>
        public bool AttackEffectOnPlayerIgnored(ContinuousEffect effect, int player)
        {
            return effect != null && effect.FromAttack && player >= 0 && effect.Owner != player && PlayerShieldedFromAttacks(player);
        }

        /// <summary>The player can't play Item cards this turn (an attack's Item lock), unless Bodyguard shields them.</summary>
        public bool ItemLocked(int player)
        {
            return State.Players[player].ItemLockTurn == State.TurnNumber && !PlayerShieldedFromAttacks(player);
        }

        /// <summary>The player can't evolve Pokémon from their hand this turn (Evolution Jammer), unless Bodyguard shields them.</summary>
        public bool EvolveLocked(int player)
        {
            // Aerodactyl's Prehistoric Power (v116) stops every player's evolving.
            if (PrehistoricPowerActive()) return true;
            return State.Players[player].EvolveLockTurn == State.TurnNumber && !PlayerShieldedFromAttacks(player);
        }

        // ------------------------------------------------------------------ The shielded hand during an attack

        /// <summary>The player whose hand the attack resolving now can't touch, or -1.</summary>
        private int guardedHandPlayer = -1;
        /// <summary>Ids and names of the cards in that hand when the attack started (what may not be shown).</summary>
        private HashSet<int> guardedHandIds;
        private HashSet<string> guardedHandNames;
        /// <summary>Something tried to show or choose from that hand.</summary>
        private bool guardedHandTouched;

        /// <summary>True while an attack is resolving that can't do anything to this player's hand (Bodyguard).</summary>
        public bool HandEffectsPrevented(int player)
        {
            return player >= 0 && player == guardedHandPlayer;
        }

        private sealed class HandGuard
        {
            public int Player;
            public List<CardInstance> Original;
            public List<CardInstance> DeckBefore;
            public int ItemLock;
            public int EvolveLock;
            public int PreviousPlayer;
            public HashSet<int> PreviousIds;
            public HashSet<string> PreviousNames;
            public bool PreviousTouched;
        }

        /// <summary>
        /// The attack runs against a copy of the shielded player's hand. EndHandGuard puts the real hand back, so whatever the
        /// attack did to the copy (discards, shuffles, draws, cards put on the deck or into play) is undone.
        /// </summary>
        private HandGuard BeginHandGuard(int player)
        {
            var p = State.Players[player];
            var guard = new HandGuard
            {
                Player = player,
                Original = p.Hand,
                DeckBefore = p.Deck.ToList(),
                ItemLock = p.ItemLockTurn,
                EvolveLock = p.EvolveLockTurn,
                PreviousPlayer = guardedHandPlayer,
                PreviousIds = guardedHandIds,
                PreviousNames = guardedHandNames,
                PreviousTouched = guardedHandTouched
            };
            p.Hand = new List<CardInstance>(guard.Original);
            guardedHandPlayer = player;
            guardedHandIds = new HashSet<int>(guard.Original.Select(c => c.Id));
            guardedHandNames = new HashSet<string>(guard.Original.Select(c => c.Def.Name));
            guardedHandTouched = false;
            return guard;
        }

        private void EndHandGuard(HandGuard guard)
        {
            var p = State.Players[guard.Player];
            var copy = p.Hand;
            p.Hand = guard.Original;
            bool touched = guardedHandTouched;
            guardedHandPlayer = guard.PreviousPlayer;
            guardedHandIds = guard.PreviousIds;
            guardedHandNames = guard.PreviousNames;
            guardedHandTouched = guard.PreviousTouched;

            bool undone = false;
            // Cards the attack took out of the hand come back from wherever they went.
            foreach (var card in guard.Original)
            {
                if (copy.Contains(card)) continue;
                TakeBackForHand(card, guard.Player);
                undone = true;
            }
            // Cards it put into the hand: the ones drawn from the deck go back on top; any other (a Pokémon returned from play
            // is an effect on that Pokémon) stays.
            var drawn = new List<CardInstance>();
            foreach (var card in copy)
            {
                if (guard.Original.Contains(card)) continue;
                if (guard.DeckBefore.Contains(card)) drawn.Add(card);
                else guard.Original.Add(card);
            }
            if (drawn.Count > 0)
            {
                undone = true;
                p.Deck.InsertRange(0, drawn);
            }
            // The deck gets its old order back when the attack did nothing else to it (the hand shuffled into it didn't happen).
            if (undone && p.Deck.Count == guard.DeckBefore.Count && new HashSet<CardInstance>(p.Deck).SetEquals(guard.DeckBefore))
            {
                p.Deck.Clear();
                p.Deck.AddRange(guard.DeckBefore);
            }
            if (p.ItemLockTurn != guard.ItemLock) { p.ItemLockTurn = guard.ItemLock; undone = true; }
            if (p.EvolveLockTurn != guard.EvolveLock) { p.EvolveLockTurn = guard.EvolveLock; undone = true; }
            if (undone || touched)
                Emit(GameEventType.Info, guard.Player, "Bodyguard: the attack's effects on " + PlayerName(guard.Player) + " and " + Their(guard.Player) + " hand were prevented.");
        }

        /// <summary>Takes a card that left the shielded hand back out of the zone it was put in.</summary>
        private void TakeBackForHand(CardInstance card, int owner)
        {
            foreach (var pl in State.Players)
            {
                if (pl.Deck.Remove(card) || pl.Discard.Remove(card) || pl.LostZone.Remove(card) || pl.Prizes.Remove(card)) return;
                if (pl.Index != owner && pl.Hand.Remove(card)) return;
                foreach (var mon in pl.InPlay().ToList())
                {
                    if (mon.Energy.Remove(card) || mon.Tools.Remove(card)) return;
                    if (!mon.Stack.Contains(card)) continue;
                    // A Basic Pokémon put onto the Bench from the hand goes back; anything more can't be taken apart.
                    if (mon.Stack.Count == 1 && mon.Energy.Count == 0 && mon.Tools.Count == 0 && pl.Bench.Remove(mon)) return;
                    State.Players[owner].Hand.Remove(card);
                    return;
                }
            }
        }

        /// <summary>A decision about cards of the shielded hand: it's answered without asking anyone, so nothing is shown.</summary>
        private bool TouchesGuardedHand(List<DecisionOption> options, List<CardInstance> seen)
        {
            if (guardedHandPlayer < 0 || guardedHandIds == null) return false;
            if (options != null && options.Any(o => o.CardId >= 0 && guardedHandIds.Contains(o.CardId))) return true;
            return seen != null && seen.Any(c => guardedHandIds.Contains(c.Id));
        }

        /// <summary>A log line or event about cards of the shielded hand, which is held back while the attack resolves.</summary>
        private bool HidesGuardedHand(GameEventType type, string text, int cardId, IEnumerable<CardInstance> cards)
        {
            if (guardedHandPlayer < 0 || guardedHandIds == null) return false;
            if (cardId >= 0 && guardedHandIds.Contains(cardId)) return true;
            if (cards != null && cards.Any(c => guardedHandIds.Contains(c.Id))) return true;
            if (string.IsNullOrEmpty(text)) return false;
            switch (type)
            {
                case GameEventType.Info:
                case GameEventType.Reveal:
                case GameEventType.Discard:
                case GameEventType.Draw:
                case GameEventType.Search:
                case GameEventType.Shuffle:
                    return guardedHandNames.Any(n => text.IndexOf(n, StringComparison.Ordinal) >= 0);
                default:
                    return false;
            }
        }
    }
}
