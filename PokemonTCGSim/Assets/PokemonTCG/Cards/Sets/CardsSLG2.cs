using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Shining Legends (SLG, v87), Trainers and Energy (SLG 58-70). Energy Retrieval, Great Ball, Hau, Lillie, Pokémon Catcher,
    /// Super Scoop Up, Switch, Ultra Ball and Warp Energy are prints of cards already in the pool. Pokémon Breeder is registered by
    /// its card number: other sets have a different card with that name.
    /// </summary>
    public static class CardsSLG2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Damage Mover", new DamageMover());
            r.RegisterByKey("SLG-63", new PokemonBreederSLG());
            r.RegisterByName("Sophocles", new Sophocles());
            r.RegisterByName("Double Colorless Energy", new DoubleColorlessEnergy());
        }

        /// <summary>
        /// Damage Mover: move 3 damage counters from 1 of your Pokémon to another of your Pokémon. A Pokémon with only 1 or 2 can be
        /// chosen; then all of them move (your ruling, v87).
        /// </summary>
        private sealed class DamageMover : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon().Count >= 2 && ctx.MyPokemon().Any(m => m.DamageCounters > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var damaged = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (damaged.Count == 0) return;
                var origin = ctx.ChoosePokemon(damaged, "Damage Mover: move damage counters from which of your Pokémon?");
                if (origin == null) return;
                var others = ctx.MyPokemon(m => m != origin);
                if (others.Count == 0) return;
                var dest = ctx.ChoosePokemon(others, "Damage Mover: move " + Math.Min(3, origin.DamageCounters) + " damage counters from " + origin.Name + " to which Pokémon?");
                if (dest != null) ctx.Engine.MoveDamageCounters(origin, dest, Math.Min(3, origin.DamageCounters), "Damage Mover");
            }
        }

        /// <summary>Pokémon Breeder (SLG): draw 2 cards and heal 20 damage from your Active Pokémon. Not playable with an empty deck.</summary>
        private sealed class PokemonBreederSLG : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 20);
            }
        }

        /// <summary>Sophocles: discard 2 cards from your hand; if you do, draw 4. The discard is a cost (Compendium, 2019-03-21).</summary>
        private sealed class Sophocles : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (hand.Count < 2) return;
                foreach (var card in ctx.ChooseCards(hand, 2, 2, "Sophocles: discard 2 cards from your hand (then draw 4 cards)"))
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(4);
            }
        }

        /// <summary>Double Colorless Energy: provides [C][C]. It's 1 Energy card (Compendium: Blaze Out-GX, Dark Call-GX).</summary>
        private sealed class DoubleColorlessEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 2); }
        }
    }
}
