using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Deoxys Trainer and Energy cards (DX 86, 89-91, 94, 95; v165). Balloon Berry (your ruling), Crystal Shard, Lady Outing, Master
    /// Ball, Strength Charm and Boost Energy are prints. Energy Charge 86 is its own card (your ruling).
    /// </summary>
    public static class CardsDXT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DX-86", new EnergyChargeDX());
            r.RegisterByKey("DX-89", new MeteorFalls());
            r.RegisterByKey("DX-90", new ProfessorCozmosDiscovery());
            r.RegisterByKey("DX-91", new SpaceCenter());
            r.RegisterByKey("DX-94", new HealEnergy());
            r.RegisterByKey("DX-95", new ScrambleEnergy());
        }

        /// <summary>Energy Charge (Deoxys): heads, shuffle 2 Energy cards (1 if there's only 1) from your discard pile into your deck.</summary>
        private sealed class EnergyChargeDX : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsEnergy); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (cards.Count == 0) return;
                int n = Math.Min(2, cards.Count);
                var picks = cards.Count == n ? cards : ctx.ChooseCards(cards, n, n, "Energy Charge: choose " + n + " Energy cards to shuffle into your deck");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.Engine.Reveal(ctx.Player, picks, "shuffled these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " deck (Energy Charge)");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Meteor Falls (Stadium): each player's Active Evolved Pokémon (not a Pokémon-ex) can use the attacks of its Basic Pokémon and Stage 1 cards (paying their cost).</summary>
        private sealed class MeteorFalls : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                var list = new List<KeyValuePair<CardDef, int>>();
                if (mon == null || mon.Stack.Count < 2 || mon.Def.IsOldEx || !src.Engine.State.IsActive(mon)) return list;
                for (int i = 0; i < mon.Stack.Count - 1; i++)
                {
                    var under = mon.Stack[i].Def;
                    if (!under.IsPokemon) continue;
                    for (int j = 0; j < under.Attacks.Count; j++) list.Add(new KeyValuePair<CardDef, int>(under, j));
                }
                return list;
            }
        }

        /// <summary>Professor Cozmo's Discovery (Supporter): heads, draw the bottom 3 cards of your deck; tails, draw the top 2.</summary>
        private sealed class ProfessorCozmosDiscovery : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip())
                {
                    ctx.Draw(2);
                    return;
                }
                int n = Math.Min(3, ctx.Me.Deck.Count);
                var bottom = ctx.Me.Deck.Skip(ctx.Me.Deck.Count - n).ToList();
                foreach (var card in bottom)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Hand.Add(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " drew the bottom " + n + " card" + (n == 1 ? "" : "s") + " of " + ctx.Engine.Their(ctx.Player) + " deck (Professor Cozmo's Discovery).");
            }
        }

        /// <summary>Space Center (Stadium): Poké-Bodies of Basic Pokémon in play (both players') are ignored, except Pokémon-ex and Pokémon with an owner in their name.</summary>
        private sealed class SpaceCenter : CardScript
        {
            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName)
            {
                return mon != null && mon.Stack.Count == 1 && mon.Def.IsBasicPokemon && !mon.Def.IsOldEx && !CardsHL.HasOwner(mon.Name);
            }
        }

        /// <summary>Heal Energy: Colorless; attached from your hand, 1 damage counter and all Special Conditions come off that Pokémon (not a Pokémon-ex).</summary>
        private sealed class HealEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Colorless, 1); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null || mon.Def.IsOldEx) return;
                if (mon.DamageCounters > 0) ctx.Heal(mon, 10);
                ctx.Engine.RemoveAllConditions(mon);
            }
        }

        /// <summary>
        /// Scramble Energy: only on an Evolved Pokémon (not a Pokémon-ex); Colorless, or 3 Energy of any types while you have more Prize
        /// cards left than the opponent; discarded if its Pokémon isn't an Evolved Pokémon (or is a Pokémon-ex).
        /// </summary>
        private sealed class ScrambleEnergy : CardScript
        {
            private static bool Fits(PokemonInPlay mon) { return mon != null && CardsE2.IsEvolved(mon) && !mon.Def.IsOldEx; }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon != null && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count) return RecifeEnergy.Units(EnergyMask.AnyType, 3);
                return RecifeEnergy.Units(EnergyMask.Colorless, 1);
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return Fits(target); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !Fits(ctx.Pokemon); }
        }
    }
}
