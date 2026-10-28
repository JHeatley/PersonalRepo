using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Neo Discovery Trainers (N2 72-75; v139), played as Items.</summary>
    public static class CardsN2T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N2-72", new FossilEgg());
            r.RegisterByKey("N2-73", new HyperDevolutionSpray());
            r.RegisterByKey("N2-74", new RuinWall());
            r.RegisterByKey("N2-75", new EnergyArk());
        }

        /// <summary>
        /// Fossil Egg: flip a coin; heads, put a card that evolves from Mysterious Fossil onto your Bench from your deck (then shuffle) or
        /// from your hand, as a Basic Pokémon. Not with a full Bench.
        /// </summary>
        private sealed class FossilEgg : CardScript
        {
            private static bool Fits(EffectContext ctx, CardDef d)
            {
                return CardsN2.EvolvesFromMysteriousFossil(d) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip() || ctx.BenchSpace <= 0) return;
                var inHand = ctx.Me.Hand.Where(c => c != ctx.Card && Fits(ctx, c.Def)).ToList();
                bool fromHand = false;
                if (inHand.Count > 0)
                    fromHand = ctx.Me.Deck.Count == 0 || ctx.ChooseOption("Fossil Egg: put a card that evolves from Mysterious Fossil onto your Bench from...", "Your deck", "Your hand") == 1;
                if (fromHand)
                {
                    var pick = ctx.ChooseCards(inHand, 1, 1, "Fossil Egg: put which card onto your Bench?")[0];
                    if (ctx.Me.Hand.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                    return;
                }
                if (ctx.Me.Deck.Count == 0) return;
                var found = ctx.SearchMyDeck(d => Fits(ctx, d), 1, "Fossil Egg: choose a card that evolves from Mysterious Fossil");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Hyper Devolution Spray: choose 1 of your evolved Pokémon; its highest Stage Evolution card goes into your hand. That Pokémon
        /// can't evolve this turn.
        /// </summary>
        private sealed class HyperDevolutionSpray : CardScript
        {
            private static List<PokemonInPlay> Evolved(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Stack.Count >= 2 && !m.Def.IsMultiCard);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Evolved(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var evolved = Evolved(ctx);
                if (evolved.Count == 0) return;
                var target = ctx.ChoosePokemon(evolved, "Hyper Devolution Spray: devolve which of your Pokémon?");
                if (target == null) return;
                var removed = ctx.Engine.Devolve(target, 1);
                if (removed.Count > 0) target.CantEvolveOnTurn = ctx.Turn;
            }
        }

        /// <summary>Ruin Wall: put a Basic Pokémon with Unown in its name from your deck onto your Bench. Not with a full Bench.</summary>
        private sealed class RuinWall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0) return;
                ctx.SearchToBench(d => d.Name.IndexOf("Unown", StringComparison.Ordinal) >= 0, 1, "Ruin Wall: choose a card with Unown in its name");
            }
        }

        /// <summary>Energy Ark: flip 2 coins; for each heads, put a basic Energy card from your deck into your hand (shown to the opponent).</summary>
        private sealed class EnergyArk : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || ctx.Me.Deck.Count == 0) return;
                ctx.SearchToHand(d => d.IsEnergy && d.IsBasicEnergy, heads, "Energy Ark: choose up to " + heads + " basic Energy card" + (heads == 1 ? "" : "s"));
            }
        }
    }
}
