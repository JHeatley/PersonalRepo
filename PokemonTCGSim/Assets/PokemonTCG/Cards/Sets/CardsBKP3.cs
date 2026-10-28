using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BREAKpoint (BKP, v96), part 3: the Trainers and Splash Energy (96-113, Skyla 122). Great Ball, Max Potion, Misty's
    /// Determination, Pokémon Catcher, Potion, Professor Sycamore and Skyla were already in the pool.
    /// </summary>
    public static class CardsBKP3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("All-Night Party", new AllNightParty());
            r.RegisterByName("Bursting Balloon", new BurstingBalloon());
            r.RegisterByName("Delinquent", new Delinquent());
            r.RegisterByName("Fighting Fury Belt", new FightingFuryBelt());
            r.RegisterByName("Gyarados Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Scizor Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Max Elixir", new MaxElixir());
            r.RegisterByName("Psychic's Third Eye", new PsychicsThirdEye());
            r.RegisterByName("Puzzle of Time", new PuzzleOfTime());
            r.RegisterByName("Reverse Valley", new ReverseValley());
            r.RegisterByName("Tierno", new Tierno());
            r.RegisterByName("Splash Energy", new SplashEnergy());
        }

        /// <summary>All-Night Party: once during each player's turn, if their Active Pokémon is Asleep, they may wake it up and heal 30 damage from it.</summary>
        private sealed class AllNightParty : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.Has(SpecialCondition.Asleep); }

            protected override bool Use(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null || !active.Has(SpecialCondition.Asleep)) return false;
                ctx.Engine.RemoveCondition(active, SpecialCondition.Asleep);
                if (active.DamageCounters > 0) ctx.Heal(active, 30);
                return true;
            }
        }

        /// <summary>
        /// Bursting Balloon: discarded at the end of the opponent's turn, even while Tools have no effect (Yveltal's Fright Night: 2016;
        /// v97). If its Pokémon is your Active Pokémon and is damaged by an opponent's attack (even if Knocked Out), 6 damage counters on
        /// the Attacking Pokémon, after the attack's own effects (healing included: Compendium, 2016-06-09).
        /// </summary>
        private sealed class BurstingBalloon : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                ctx.Engine.PlaceDamageCounters(attacker, 6, "Bursting Balloon");
            }

            public override bool DiscardedAtEndOfOpponentsTurn { get { return true; } }
        }

        /// <summary>Delinquent: discard any Stadium card in play; if you do, your opponent discards 3 cards from their hand (they choose).</summary>
        private sealed class Delinquent : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.State.Stadium != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.State.Stadium == null) return;
                ctx.Engine.DiscardStadium();
                if (ctx.State.Stadium != null || ctx.Opp.Hand.Count == 0) return;
                int n = Math.Min(3, ctx.Opp.Hand.Count);
                var picks = n == ctx.Opp.Hand.Count ? ctx.Opp.Hand.ToList()
                    : ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), n, n, "Delinquent: discard " + n + " cards from your hand", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }
        }

        /// <summary>Fighting Fury Belt: its Basic Pokémon gets +40 HP and its attacks do 10 more to the opponent's Active Pokémon (before W/R).</summary>
        private sealed class FightingFuryBelt : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.IsBasicPokemon ? hp + 40 : hp;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !me.Def.IsBasicPokemon || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 10;
            }
        }

        /// <summary>Max Elixir: look at the top 6 cards of your deck and attach a basic Energy card there to a Basic Pokémon on your Bench; shuffle the rest back.</summary>
        private sealed class MaxElixir : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyBench().Where(m => m.Def.IsBasicPokemon).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                var targets = Targets(ctx);
                if (energy.Count > 0 && targets.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, 1, "Max Elixir: choose a basic Energy card to attach to a Basic Pokémon on your Bench");
                    if (picks.Count > 0)
                    {
                        var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Max Elixir: attach " + picks[0].Def.Name + " to which Benched Basic Pokémon?");
                        if (target != null && ctx.Me.Deck.Remove(picks[0])) ctx.Engine.AttachEnergyCard(picks[0], target);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Psychic's Third Eye: your opponent reveals their hand; discard as many cards as you like from your hand, then draw that many.</summary>
        private sealed class PsychicsThirdEye : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var picks = ctx.ChooseCards(hand, 0, hand.Count, "Psychic's Third Eye: discard as many cards as you like from your hand, then draw that many");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (picks.Count > 0) ctx.Draw(picks.Count);
            }
        }

        /// <summary>
        /// Puzzle of Time: you may play 2 at once. One card: look at the top 3 cards of your deck and put them back in any order. Two
        /// cards: put 2 cards from your discard pile into your hand (not the Puzzle of Time cards being played).
        /// </summary>
        private sealed class PuzzleOfTime : CardScript
        {
            private static CardInstance OtherPuzzle(EffectContext ctx)
            {
                return ctx.Me.Hand.FirstOrDefault(c => c.Def.Name == "Puzzle of Time" && c != ctx.Card);
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 || (OtherPuzzle(ctx) != null && ctx.Me.Discard.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var other = OtherPuzzle(ctx);
                if (other != null && ctx.Me.Discard.Count > 0 && ctx.AskYesNo("Puzzle of Time: play 2 Puzzle of Time cards at once (2 cards from your discard pile into your hand)?"))
                {
                    ctx.Me.Hand.Remove(other);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " played 2 Puzzle of Time cards at once.");
                    var pile = ctx.Me.Discard.ToList();
                    int n = Math.Min(2, pile.Count);
                    var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Puzzle of Time: put 2 cards from your discard pile into your hand");
                    ctx.DiscardToHand(picks);
                    ctx.Me.Discard.Add(other);
                    return;
                }
                if (ctx.Me.Deck.Count > 0) CardsOBF2.ReorderOwnTop(ctx, 3, "Puzzle of Time");
            }
        }

        /// <summary>The way Reverse Valley faces: Owner is the player whose Metal Pokémon take 10 less damage.</summary>
        internal sealed class ReverseValleyFacing : StadiumFacingEffect
        {
            public ReverseValleyFacing(int metalSide, int stadiumId) : base("Reverse Valley", metalSide, stadiumId, metalSide) { }
        }

        /// <summary>
        /// Reverse Valley: the player who plays it chooses which way it faces. The Metal Pokémon of the player one half points at take 10
        /// less damage from the opponent's attacks (after W/R); the other player's Darkness Pokémon do 10 more to the opponent's Active
        /// Pokémon (before W/R).
        /// </summary>
        private sealed class ReverseValley : CardScript
        {
            public override void OnStadiumPlayed(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ReverseValleyFacing);
                int pick = ctx.ChooseOption("Reverse Valley: which way does it face?",
                    "Your Metal Pokémon take 10 less damage; your opponent's Darkness Pokémon do 10 more damage",
                    "Your Darkness Pokémon do 10 more damage; your opponent's Metal Pokémon take 10 less damage");
                int side = pick == 0 ? ctx.Player : ctx.Opponent;
                ctx.State.Effects.Add(new ReverseValleyFacing(side, ctx.Card.Id));
                ctx.Log("Reverse Valley: " + ctx.Engine.PlayerName(side) + "'s Metal Pokémon take 10 less damage, and "
                    + ctx.Engine.PlayerName(1 - side) + "'s Darkness Pokémon do 10 more damage.");
            }

            public override void OnLeavePlay(EffectContext ctx)
            {
                ctx.State.Effects.RemoveAll(e => e is ReverseValleyFacing);
            }

            /// <summary>The player facing the Metal half (the one who played it, if it was put into play without choosing).</summary>
            private static int MetalSide(EffectContext src)
            {
                foreach (var e in src.State.Effects)
                {
                    var facing = e as ReverseValleyFacing;
                    if (facing != null && src.Card != null && facing.StadiumId == src.Card.Id) return facing.Owner;
                }
                return src.Player;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (t == null || info.Attacker == null || info.Attacker.Owner == t.Owner || damage <= 0) return damage;
                return t.Owner == MetalSide(src) && src.Engine.IsType(t, EnergyType.Metal) ? damage - 10 : damage;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || !info.ToOpponentsActive || damage <= 0) return damage;
                return a.Owner != MetalSide(src) && src.Engine.IsType(a, EnergyType.Darkness) ? damage + 10 : damage;
            }
        }

        /// <summary>Tierno: draw 3 cards.</summary>
        private sealed class Tierno : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(3); }
        }

        /// <summary>
        /// Splash Energy: only on a Water Pokémon (discarded from anything else); provides Water there. If that Pokémon is Knocked Out by
        /// damage from the opponent's attack, the whole Pokémon (every card in it) goes into your hand and its attached cards are
        /// discarded (Compendium, 2016-02-11).
        /// </summary>
        private sealed class SplashEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Water)) return new List<int>();
                return new List<int> { EnergyMask.Of(EnergyType.Water) };
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Water); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Water); }

            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return byOpponentsAttack && mon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Water);
            }
        }
    }
}
