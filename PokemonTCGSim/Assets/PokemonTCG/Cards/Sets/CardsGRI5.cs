using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Guardians Rising (GRI, v89), Trainers (GRI 116-130). Aether Paradise Conservation Area and Brooklet Hill (Hidden Fates), Choice
    /// Band, Multi Switch and Rescue Stretcher (Burning Shadows' secret rares), Energy Loto, Energy Recycler and Enhanced Hammer were
    /// already in the pool.
    /// </summary>
    public static class CardsGRI5
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Altar of the Moone", new AltarOfTheMoone());
            r.RegisterByName("Altar of the Sunne", new AltarOfTheSunne());
            r.RegisterByName("Aqua Patch", new AquaPatch());
            r.RegisterByName("Field Blower", new FieldBlower());
            r.RegisterByName("Hala", new Hala());
            r.RegisterByName("Mallow", new Mallow());
            r.RegisterByName("Max Potion", new MaxPotion());
        }

        /// <summary>Altar of the Moone: each Pokémon in play with any Psychic or Darkness Energy attached has a Retreat Cost [C][C] less.</summary>
        private sealed class AltarOfTheMoone : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null) return cost;
                var engine = src.Engine;
                bool has = mon.Energy.Any(en => engine.EnergyUnitsOf(mon, en).Any(u => EnergyMask.Provides(u, EnergyType.Psychic) || EnergyMask.Provides(u, EnergyType.Darkness)));
                return has ? cost - 2 : cost;
            }
        }

        /// <summary>Altar of the Sunne: each Fire and Metal Pokémon in play has no Weakness.</summary>
        private sealed class AltarOfTheSunne : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && (src.Engine.IsType(mon, EnergyType.Fire) || src.Engine.IsType(mon, EnergyType.Metal));
            }
        }

        /// <summary>Aqua Patch: attach a Water Energy card from your discard pile to 1 of your Benched Water Pokémon.</summary>
        private sealed class AquaPatch : CardScript
        {
            private static bool Target(EffectContext ctx, PokemonInPlay mon) { return ctx.Me.Bench.Contains(mon) && ctx.Engine.IsType(mon, EnergyType.Water); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water).Count > 0 && ctx.Me.Bench.Any(m => Target(ctx, m));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var water = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (water.Count == 0) return;
                ctx.AttachCardsAnyWay(water, 1, (card, mon) => Target(ctx, mon), "Aqua Patch: attach a Water Energy card to 1 of your Benched Water Pokémon", false);
            }
        }

        /// <summary>Field Blower: choose up to 2 Pokémon Tool and/or Stadium cards in play (either player's) and discard them.</summary>
        private sealed class FieldBlower : CardScript
        {
            private static List<CardInstance> Targets(EffectContext ctx)
            {
                var list = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon())) list.AddRange(mon.Tools);
                if (ctx.State.Stadium != null) list.Add(ctx.State.Stadium);
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                foreach (var card in ctx.ChooseCards(targets, 1, Math.Min(2, targets.Count), "Field Blower: discard up to 2 Pokémon Tool and Stadium cards in play"))
                {
                    if (card == ctx.State.Stadium)
                    {
                        ctx.Engine.DiscardStadium();
                        continue;
                    }
                    var holder = ctx.Engine.ToolHolder(card);
                    if (holder != null) ctx.Engine.DiscardTool(holder, card);
                }
            }
        }

        /// <summary>Hala: shuffle your hand into your deck, then draw 4 cards, or 7 if you've used your GX attack (being hit by Clear Vision-GX doesn't count).</summary>
        private sealed class Hala : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Me.GxUsed ? 7 : 4);
            }
        }

        /// <summary>Mallow: search your deck for 2 cards, shuffle your deck, then put those cards on top of it in any order.</summary>
        private sealed class Mallow : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(2, ctx.Me.Deck.Count);
                if (n == 0) return;
                var found = ctx.SearchMyDeck(d => true, n, "Mallow: search your deck for 2 cards to put on top of it", false, n);
                ctx.ShuffleMyDeck();
                CardsCRZ1.PutOnTopInOrder(ctx, found, "Mallow");
            }
        }

        /// <summary>Max Potion: heal all damage from 1 of your Pokémon (it needs damage), then discard all Energy from it (it may have none).</summary>
        private sealed class MaxPotion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.MyPokemon().Any(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m));
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Max Potion: heal all damage from which of your Pokémon (its Energy is discarded)?");
                if (target == null) return;
                ctx.Heal(target, target.DamageCounters * 10);
                ctx.Engine.DiscardAllEnergy(target);
            }
        }
    }
}
