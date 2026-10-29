using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Legend Maker Trainer and Energy cards (LM 72-82; v170). Claw Fossil 78 and Root Fossil 80 are Sandstorm's (prints); Mysterious
    /// Fossil 79 is its own 50-HP card (the engine plays it like the other fossils). Compendium rulings: Full Flame and Fiery Aura
    /// don't add up (4 counters either way); Giant Stump's discards aren't Knock Outs, and a full Bench is 3 while it's in play;
    /// Power Tree's search is optional; Pokémon put down by Strange Cave are Basic Pokémon.
    /// </summary>
    public static class CardsLMT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LM-72", new CursedStone());
            r.RegisterByKey("LM-73", new Fieldworker());
            r.RegisterByKey("LM-74", new FullFlame());
            r.RegisterByKey("LM-75", new GiantStump());
            r.RegisterByKey("LM-76", new PowerTree());
            r.RegisterByKey("LM-77", new StrangeCave());
            r.RegisterByKey("LM-79", new CardScript()); // Mysterious Fossil: played like the other fossils (engine)
            r.RegisterByKey("LM-82", new CardScript()); // React Energy: [C]; the cards that count it do the rest
        }

        /// <summary>Cursed Stone: between turns, each player puts 1 damage counter on each of their Pokémon that has a Poké-Power.</summary>
        private sealed class CursedStone : CardScript
        {
            private static bool HasPokePower(PokemonInPlay mon) { return mon.Def.Abilities.Any(a => a.Kind == AbilityDef.PokePower); }

            public override int CheckupAffects(EffectContext src) { return -1; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                for (int k = 0; k < 2; k++)
                {
                    int player = k == 0 ? src.Engine.State.CurrentPlayer : 1 - src.Engine.State.CurrentPlayer;
                    foreach (var mon in src.State.Players[player].InPlay().Where(HasPokePower).ToList())
                        src.Engine.PlaceDamageCounters(mon, 1, "Cursed Stone");
                }
            }
        }

        /// <summary>Fieldworker (Supporter): draw 3 cards; your opponent may draw a card.</summary>
        private sealed class Fieldworker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(3);
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Fieldworker: draw a card?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 1);
            }
        }

        /// <summary>Full Flame: Burned Pokémon take 4 damage counters between turns instead of 2, and evolving or devolving doesn't remove Burned.</summary>
        private sealed class FullFlame : CardScript
        {
            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters) { return Math.Max(counters, 4); }

            public override bool KeepsBurnOnEvolve(EffectContext src, PokemonInPlay mon) { return true; }
        }

        /// <summary>Giant Stump: no player can have more than 3 Benched Pokémon; when it comes into play, each player discards Benched Pokémon down to 3 (you first).</summary>
        private sealed class GiantStump : CardScript
        {
            public override int CapMaxBench(EffectContext src, int player) { return 3; }

            public override void OnStadiumPlayed(EffectContext ctx) { ctx.Engine.EnforceBenchLimits(ctx.Player); }
        }

        /// <summary>Power Tree: once during each player's turn, with no Special Energy cards in their discard pile, they may put a basic Energy card from it into their hand.</summary>
        private sealed class PowerTree : CardScript
        {
            private const string Limit = "Power Tree";

            public override bool CanUseStadium(EffectContext ctx)
            {
                return !ctx.NamedLimitUsedThisTurn(Limit) && !ctx.Me.Discard.Any(c => c.Def.IsSpecialEnergy) && ctx.BasicEnergyInDiscard().Count > 0;
            }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var cards = ctx.BasicEnergyInDiscard();
                if (cards.Count == 0) return false;
                var card = ctx.ChooseCards(cards, 1, 1, "Power Tree: put which basic Energy card into your hand?");
                ctx.Engine.ShowCards(ctx.Opponent, card, Limit, "Power Tree: the card your opponent took", ctx.Source);
                ctx.DiscardToHand(card);
                return false;
            }
        }

        /// <summary>Strange Cave: once during each player's turn, they may put an Omanyte, Kabuto, Aerodactyl, Aerodactyl ex, Lileep or Anorith from their hand onto their Bench, as a Basic Pokémon.</summary>
        private sealed class StrangeCave : CardScript
        {
            private const string Limit = "Strange Cave";
            private static readonly string[] Names = CardsLMB.Ancients.Concat(new[] { "Aerodactyl ex" }).ToArray();

            private static List<CardInstance> Cards(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && Names.Contains(c.Def.Name) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.BenchSpace > 0 && Cards(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var cards = Cards(ctx);
                if (cards.Count == 0 || ctx.BenchSpace <= 0) return false;
                var card = ctx.ChooseCards(cards, 1, 1, "Strange Cave: put which Pokémon onto your Bench?")[0];
                if (!ctx.Me.Hand.Remove(card)) return false;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false, true);
                if (mon != null) mon.Markers[PokemonInPlay.ConsideredBasicMarker] = 1;
                return false;
            }
        }
    }
}
