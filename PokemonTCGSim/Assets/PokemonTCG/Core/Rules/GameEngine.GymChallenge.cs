using System;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>
    /// Gym Challenge (v126) rules support.
    /// - Lt. Surge's Secret Plan (your ruling: the printed text): a card from the hand goes face down onto the Bench as a Basic
    ///   Pokémon. In play it's a stand-in card with the same id (FaceDownDef: a Basic Pokémon with no attacks, no Weakness and
    ///   no Retreat Cost), and the real card waits in GameState.FaceDownCards. It's flipped over when its owner chooses to during
    ///   their turn, when the opponent attacks while it's the Defending Pokémon, when it would be damaged, get damage counters or a
    ///   Special Condition, when an attack affects it (an effect on it, a switch, its Energy), when it retreats, and when it
    ///   leaves play. Flipped over, a card that isn't a Basic Pokémon is discarded with everything attached to it.
    /// - Brock's Ninetales's Shapeshift: Evolution cards attached to a Pokémon (PokemonInPlay.Attached). While it has one, the
    ///   newest is the Pokémon it's treated as (your ruling): that card's name, type, HP, Weakness, Resistance, Retreat Cost,
    ///   attacks and Pokémon Powers (through InheritedPowersCard, like Ditto's Transform). It keeps Shapeshift. It can't evolve
    ///   or devolve.
    /// </summary>
    public sealed partial class GameEngine
    {
        /// <summary>The stand-in for a face-down card in play (Lt. Surge's Secret Plan).</summary>
        public static readonly CardDef FaceDownDef = new CardDef
        {
            Key = "FACEDOWN",
            Name = "Face-down card",
            SetCode = "",
            Number = "",
            Kind = CardKind.Pokemon,
            Stage = Stage.Basic,
            Hp = 10,
            PokemonType = EnergyType.Colorless,
            RetreatCost = 0,
            UnlimitedOnly = true,
            IsFaceDown = true,
            Text = "A card put face down by Lt. Surge's Secret Plan. It's a Basic Pokémon while it's face down; its owner may flip it over during their turn."
        };

        /// <summary>The Pokémon is a face-down card (Lt. Surge's Secret Plan).</summary>
        public static bool IsFaceDown(PokemonInPlay mon)
        {
            return mon != null && mon.Stack.Count > 0 && mon.Stack[0].Def.IsFaceDown;
        }

        /// <summary>The real card behind a face-down Pokémon, or null.</summary>
        public CardInstance FaceDownCardOf(PokemonInPlay mon)
        {
            if (!IsFaceDown(mon)) return null;
            CardInstance real;
            return State.FaceDownCards.TryGetValue(mon.Stack[0].Id, out real) ? real : null;
        }

        /// <summary>Lt. Surge's Secret Plan: puts a card (already out of the hand) face down onto the Bench. Null with a full Bench.</summary>
        public PokemonInPlay PutFaceDownOntoBench(int player, CardInstance card)
        {
            if (card == null || !BenchHasRoom(player)) return null;
            var standIn = new CardInstance(card.Id, FaceDownDef, card.Owner);
            State.FaceDownCards[card.Id] = card;
            return PutOntoBench(player, standIn, false);
        }

        /// <summary>Puts the real card back in the stand-in's place (no other effect). Returns the real card, or null.</summary>
        private CardInstance RestoreFaceDown(PokemonInPlay mon)
        {
            if (!IsFaceDown(mon)) return null;
            var standIn = mon.Stack[0];
            CardInstance real;
            if (!State.FaceDownCards.TryGetValue(standIn.Id, out real)) return null;
            State.FaceDownCards.Remove(standIn.Id);
            mon.Stack[0] = real;
            return real;
        }

        /// <summary>
        /// Flips a face-down Pokémon over: a Basic Pokémon card stays in play as that Pokémon; any other card is discarded with
        /// everything attached to it. Returns true when the Pokémon is still in play.
        /// </summary>
        public bool FlipFaceDown(PokemonInPlay mon)
        {
            if (!IsFaceDown(mon)) return mon != null && State.FindPokemon(mon.Uid) == mon;
            var real = RestoreFaceDown(mon);
            if (real == null) return State.FindPokemon(mon.Uid) == mon;
            Emit(GameEventType.Info, mon.Owner, PlayerName(mon.Owner) + " flipped over the face-down card: it's " + real.Def.Name + ".", real.Id, mon.Uid);
            if (real.Def.IsBasicPokemon) return true;
            Emit(GameEventType.Info, mon.Owner, real.Def.Name + " isn't a Basic Pokémon, so it's discarded with all cards attached to it (Lt. Surge's Secret Plan).", real.Id, mon.Uid);
            DiscardPokemonFromPlay(mon);
            return false;
        }

        /// <summary>
        /// "Flip the card if it is affected by an attack": a face-down Pokémon an attack is about to affect is flipped over first.
        /// Returns false when it's no longer in play.
        /// </summary>
        private bool FlipIfAttackAffects(PokemonInPlay mon)
        {
            if (!IsFaceDown(mon) || CurrentSourceKind != EffectSourceKind.Attack) return true;
            return FlipFaceDown(mon);
        }

        /// <summary>Brock's Ninetales's Shapeshift: the Evolution card the Pokémon is treated as right now (the newest attached), or null.</summary>
        public CardDef ShapeshiftCard(PokemonInPlay mon)
        {
            if (mon == null || mon.Attached.Count == 0) return null;
            for (int i = mon.Attached.Count - 1; i >= 0; i--)
                if (mon.Attached[i].Def.IsEvolution) return mon.Attached[i].Def;
            return null;
        }

        /// <summary>The attached Evolution card the Pokémon is treated as (Shapeshift), or null.</summary>
        public CardInstance ShapeshiftCardInstance(PokemonInPlay mon)
        {
            if (mon == null || mon.Attached.Count == 0) return null;
            for (int i = mon.Attached.Count - 1; i >= 0; i--)
                if (mon.Attached[i].Def.IsEvolution) return mon.Attached[i];
            return null;
        }

        /// <summary>Discards the cards attached to a Pokémon under Shapeshift (PokemonInPlay.Attached).</summary>
        public void DiscardAttachedCards(PokemonInPlay mon, IList<CardInstance> cards, string reason)
        {
            if (mon == null || cards == null) return;
            foreach (var card in cards.ToList())
            {
                if (!mon.Attached.Remove(card)) continue;
                Emit(GameEventType.Discard, mon.Owner, card.Def.Name + " was discarded from " + mon.Name + " (" + reason + ").", card.Id, mon.Uid);
                DiscardFromPlay(card);
            }
        }

        /// <summary>
        /// Plays a Trainer card's effect for a player who didn't play it from their hand (Chaos Gym's tails for the opponent,
        /// Sabrina's Psychic Control from the opponent's discard pile). Nothing about where the card goes afterwards: the caller
        /// places it. Returns false if the player can't meet the card's requirements.
        /// </summary>
        public bool UseTrainerFor(int player, CardInstance card)
        {
            if (card == null || !card.Def.IsTrainer) return false;
            var script = Scripts.For(card.Def);
            var ctx = new EffectContext(this, player, card, null);
            if (!script.CanPlayTrainer(ctx)) return false;
            Emit(GameEventType.Info, player, PlayerName(player) + " used " + card.Def.Name + ".", card.Id);
            PushSource(EffectSourceKind.Trainer, player, null);
            var outerTrainer = ResolvingTrainer;
            ResolvingTrainer = card.Def;
            try { script.ResolveTrainer(ctx); }
            finally
            {
                ResolvingTrainer = outerTrainer;
                PopSource();
            }
            return true;
        }

        /// <summary>
        /// Chaos Gym: a player who plays a Trainer card (not a Stadium) flips a coin. Tails: they can't play it; their opponent may
        /// use it instead (a Pokémon Tool can't be: it would be put into play), and the card goes to its owner's discard pile.
        /// Returns true when the card was dealt with here (tails).
        /// </summary>
        private bool ChaosGymStops(int player, CardInstance card)
        {
            if (card.Def.IsStadium || State.Stadium == null || StadiumSwitchedOff()) return false;
            var stadium = Scripts.For(State.Stadium.Def);
            if (!stadium.FlipsForTrainers(StadiumContext(), player, card)) return false;
            if (FlipCoin(player, State.Stadium.Def.Name + " (" + card.Def.Name + ")")) return false;
            Emit(GameEventType.Info, player, PlayerName(player) + " can't play " + card.Def.Name + " (" + State.Stadium.Def.Name + ").", card.Id);
            int opponent = 1 - player;
            if (!card.Def.IsTool && Scripts.For(card.Def).CanPlayTrainer(new EffectContext(this, opponent, card, null))
                && AskYesNo(opponent, State.Stadium.Def.Name + ": use " + card.Def.Name + " as if you had played it?", card.Def))
                UseTrainerFor(opponent, card);
            var owner = State.Players[card.Owner];
            if (!owner.Discard.Contains(card)) owner.Discard.Add(card);
            return true;
        }

        /// <summary>
        /// An attack is about to happen: the opponent's face-down Active Pokémon is flipped over (the attacking player needs to know
        /// what it is). If it's discarded, its owner promotes a new Active Pokémon before the attack goes on.
        /// </summary>
        private void FlipDefendingFaceDown(int player, PokemonInPlay attacker)
        {
            var side = State.Players[1 - player];
            if (!IsFaceDown(side.Active)) return;
            PushSource(EffectSourceKind.Attack, player, attacker);
            try { FlipFaceDown(side.Active); }
            finally { PopSource(); }
            if (side.Active != null) return;
            if (side.Bench.Count > 0) PromoteActive(1 - player);
            else CheckWinConditions();
        }

        /// <summary>The player's Retreat with a face-down Active Pokémon flips it first. Returns false when the Retreat can't go on.</summary>
        private bool FlipBeforeRetreat(int player)
        {
            var p = State.Players[player];
            if (!IsFaceDown(p.Active)) return true;
            if (!FlipFaceDown(p.Active))
            {
                // It was discarded: the player chooses a new Active Pokémon, and that was their Retreat for the turn.
                p.RetreatedThisTurn = true;
                if (p.Active == null && p.Bench.Count > 0) PromoteActive(player);
                else if (!p.HasPokemonInPlay) CheckWinConditions();
                return false;
            }
            return CanRetreat(p);
        }

        /// <summary>Card scripts' "always on" checks (Brock's Primeape's Scram, Shapeshift's discards), run with the other state checks.</summary>
        private void PokemonStateChecks()
        {
            int before = State.Players[0].InPlay().Count() + State.Players[1].InPlay().Count();
            for (int i = 0; i < 2; i++)
                foreach (var mon in State.Players[i].InPlay().ToList())
                {
                    if (State.IsOver || State.FindPokemon(mon.Uid) != mon) continue;
                    var script = Scripts.For(mon.Def);
                    PushSource(mon.Def.Abilities.Count > 0 ? mon.Def.AbilitySourceKind : EffectSourceKind.Rule, mon.Owner, mon);
                    try { script.OnStateCheck(new EffectContext(this, mon.Owner, mon.Top, mon)); }
                    finally { PopSource(); }
                }
            if (State.Players[0].InPlay().Count() + State.Players[1].InPlay().Count() == before) return;
            for (int i = 0; i < 2 && !State.IsOver; i++)
            {
                var p = State.Players[i];
                if (p.Active == null && p.Bench.Count > 0) PromoteActive(i);
                else if (!p.HasPokemonInPlay) CheckWinConditions();
            }
        }
    }
}
