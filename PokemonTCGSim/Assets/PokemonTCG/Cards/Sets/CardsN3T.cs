using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Neo Revelation Trainers (N3 60-64; v141). Your rulings: Balloon Berry is discarded on every retreat, even with a Retreat
    /// Cost of 0 (GameEngine.RetreatCostTool); Pokémon Breeder Fields finds a card that evolves from the chosen Pokémon; Rocket's
    /// Hideout counts only Dark Pokémon (names starting with "Dark ").
    /// </summary>
    public static class CardsN3T
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("N3-60", new BalloonBerry());
            r.RegisterByKey("N3-61", new HealingField());
            r.RegisterByKey("N3-62", new PokemonBreederFields());
            r.RegisterByKey("N3-63", new RocketsHideout());
            r.RegisterByKey("N3-64", new OldRod());
        }

        /// <summary>
        /// Balloon Berry (Pokémon Tool): when its holder retreats, it's discarded instead of the Energy (the engine pays with it; your
        /// ruling: even with a Retreat Cost of 0). Compendium: play it as if the holder's Retreat Cost is 0 while it's attached.
        /// </summary>
        private sealed class BalloonBerry : CardScript
        {
            public override bool ReplacesRetreatCost(EffectContext src) { return true; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon != null && mon == src.Pokemon ? 0 : cost; }
        }

        /// <summary>Healing Field (Stadium): once during each player's turn, they may flip a coin; heads, 2 damage counters off their Active Pokémon.</summary>
        private sealed class HealingField : CardScript
        {
            private const string Limit = "Healing Field";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Active != null; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var active = ctx.Me.Active;
                if (!ctx.Flip() || active == null) return false;
                if (active.DamageCounters > 0) ctx.Heal(active, 20);
                return false;
            }
        }

        /// <summary>
        /// Pokémon Breeder Fields: choose 1 or 2 of your non-Baby Pokémon that can evolve and flip a coin for each; for each heads, put a
        /// card from your deck that evolves from that Pokémon into your hand (your ruling).
        /// </summary>
        private sealed class PokemonBreederFields : CardScript
        {
            private static bool CanEvolve(PokemonInPlay mon)
            {
                var d = mon.Def;
                return !d.IsBaby && !d.IsFossil && d.Stage != Stage.Stage2 && string.IsNullOrEmpty(d.Mechanic);
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(CanEvolve).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var candidates = ctx.MyPokemon(CanEvolve);
                if (candidates.Count == 0) return;
                var chosen = ctx.ChoosePokemonMany(candidates, 1, 2, "Pokémon Breeder Fields: choose 1 or 2 of your Pokémon");
                bool searched = false;
                foreach (var mon in chosen)
                {
                    if (!ctx.Flip() || ctx.Me.Deck.Count == 0) continue;
                    var target = mon;
                    var found = ctx.SearchMyDeck(d => CardsEVS1.EvolvesOnto(ctx, target, d), 1, "Pokémon Breeder Fields: choose a card that evolves from " + mon.Name);
                    ctx.Me.Hand.AddRange(found);
                    searched = true;
                }
                if (searched) ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Rocket's Hideout (Stadium): each Dark Pokémon in play (both players') gets +20 HP (your ruling: names starting with "Dark ").</summary>
        private sealed class RocketsHideout : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon != null && mon.Name.StartsWith("Dark ", StringComparison.Ordinal) ? hp + 20 : hp;
            }
        }

        /// <summary>Old Rod: flip 2 coins; both heads, a Pokémon card from your discard pile into your hand; both tails, a Trainer card.</summary>
        private sealed class OldRod : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int heads = ctx.FlipCoins(2);
                if (heads == 1) return;
                bool pokemon = heads == 2;
                var cards = ctx.Me.Discard.Where(c => c != ctx.Card && (pokemon ? c.Def.IsPokemon : c.Def.IsTrainer)).ToList();
                if (cards.Count == 0) return;
                var pick = ctx.ChooseCards(cards, 1, 1, "Old Rod: put which " + (pokemon ? "Pokémon" : "Trainer") + " card into your hand?");
                ctx.DiscardToHand(pick);
            }
        }
    }
}
