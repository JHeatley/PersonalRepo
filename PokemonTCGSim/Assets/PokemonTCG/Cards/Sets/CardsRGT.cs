using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// FireRed &amp; LeafGreen Trainer cards (RG 88, 94, 96, 98; v162). The others are prints: Bill's Maintenance, Energy Removal 2,
    /// Energy Switch, EXP.ALL, Great Ball, Life Herb, Poké Ball, Pokémon Reversal, Super Scoop Up, VS Seeker, Potion, Switch and
    /// Multi Energy.
    /// </summary>
    public static class CardsRGT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RG-88", new CeliosNetwork());
            r.RegisterByKey("RG-94", new MtMoon());
            r.RegisterByKey("RG-96", new PokeDexHandy909());
            r.RegisterByKey("RG-98", new ProfOaksResearch());
        }

        /// <summary>Celio's Network (Supporter): a Basic Pokémon or Evolution card (not a Pokémon-ex) from your deck into your hand.</summary>
        private sealed class CeliosNetwork : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && (d.IsBasicPokemon || d.IsEvolution) && !d.IsOldEx, 1, "Celio's Network: choose a Basic Pokémon or Evolution card (not a Pokémon-ex)");
            }
        }

        /// <summary>Mt. Moon (Stadium): Pokémon (both players') with maximum HP less than 70 can't use Poké-Powers.</summary>
        private sealed class MtMoon : CardScript
        {
            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return mon != null && mon.PrintedHp < 70; }
        }

        /// <summary>PokéDex HANDY909: shuffle your deck, then look at the top 6 cards and put them back in any order.</summary>
        private sealed class PokeDexHandy909 : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.ShuffleMyDeck();
                CardsOBF2.ReorderOwnTop(ctx, 6, "PokéDex HANDY909");
            }
        }

        /// <summary>Prof. Oak's Research (Supporter): shuffle your hand into your deck, then draw 5 cards.</summary>
        private sealed class ProfOaksResearch : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
            }
        }
    }
}
