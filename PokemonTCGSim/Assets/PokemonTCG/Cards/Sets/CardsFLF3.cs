using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Flashfire (FLF, v104), part 3: the Trainers (88-99). Lysandre, Pal Pad, Pokémon Center Lady, Pokémon Fan Club, Sacred Ash and
    /// Ultra Ball were already in the pool.
    /// </summary>
    public static class CardsFLF3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Blacksmith", new Blacksmith());
            r.RegisterByName("Fiery Torch", new FieryTorch());
            r.RegisterByName("Magnetic Storm", new MagneticStorm());
            r.RegisterByName("Protection Cube", new ProtectionCube());
            r.RegisterByName("Startling Megaphone", new StartlingMegaphone());
            r.RegisterByName("Trick Shovel", new TrickShovel());
        }

        private static bool IsFireEnergyCard(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Fire); }

        /// <summary>Blacksmith: attach 2 Fire Energy cards from your discard pile to 1 of your Fire Pokémon.</summary>
        private sealed class Blacksmith : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire)); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Targets(ctx).Count > 0 && ctx.Me.Discard.Any(c => IsFireEnergyCard(c.Def));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var pile = ctx.Me.Discard.Where(c => IsFireEnergyCard(c.Def)).ToList();
                if (targets.Count == 0 || pile.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Blacksmith: attach 2 Fire Energy cards from your discard pile to which Fire Pokémon?");
                if (target == null) return;
                int n = Math.Min(2, pile.Count);
                var picks = n == pile.Count ? pile : ctx.ChooseCards(pile, n, n, "Blacksmith: choose 2 Fire Energy cards to attach to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Fiery Torch: discard a Fire Energy card from your hand (needed to play it) and draw 2 cards. Not playable with an empty deck (2014-11).</summary>
        private sealed class FieryTorch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(c => IsFireEnergyCard(c.Def));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => IsFireEnergyCard(c.Def)).ToList();
                if (fire.Count == 0) return;
                var card = ctx.ChooseCards(fire, 1, 1, "Fiery Torch: discard which Fire Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
            }
        }

        /// <summary>Magnetic Storm: Pokémon in play (both players') have no Resistance.</summary>
        private sealed class MagneticStorm : CardScript
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info) { return info.Target != null; }
        }

        /// <summary>Protection Cube: prevent all damage done to its Pokémon by that Pokémon's own attacks.</summary>
        private sealed class ProtectionCube : CardScript
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                return holder != null && info.Target == holder && info.Attacker == holder;
            }
        }

        /// <summary>Startling Megaphone: discard all Pokémon Tools attached to each of the opponent's Pokémon.</summary>
        private sealed class StartlingMegaphone : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Tools.Count > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0))
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var tool in mon.Tools.ToList()) ctx.Engine.DiscardTool(mon, tool);
                }
            }
        }

        /// <summary>Trick Shovel: look at the top card of either player's deck; you may discard it or put it back on top.</summary>
        private sealed class TrickShovel : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int whose;
                if (ctx.Me.Deck.Count == 0) whose = ctx.Opponent;
                else if (ctx.Opp.Deck.Count == 0) whose = ctx.Player;
                else whose = ctx.ChooseOption("Trick Shovel: look at the top card of whose deck?", "Your deck", "Your opponent's deck") == 0 ? ctx.Player : ctx.Opponent;
                var deck = ctx.State.Players[whose].Deck;
                if (deck.Count == 0) return;
                var top = deck[0];
                string owner = whose == ctx.Player ? "your" : "your opponent's";
                ctx.Engine.NoteLookedAt(new List<CardInstance> { top }, "Top card of " + owner + " deck", ctx.Player);
                var pick = ctx.ChooseCards(new List<CardInstance> { top }, 0, 1, "Trick Shovel: choose " + top.Def.Name + " to discard it, or choose nothing to put it back on top of " + owner + " deck");
                if (pick.Count == 0)
                {
                    ctx.Log("Trick Shovel: the card was put back on top of " + owner + " deck.");
                    return;
                }
                ctx.Engine.DiscardTopOfDeck(whose, 1);
            }
        }
    }
}
