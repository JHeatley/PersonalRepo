using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Holon Phantoms Trainer and Energy cards (HP 85-98; v170). Professor Cozmo's Discovery, Rare Candy, the fossils, Darkness, Metal,
    /// Multi and Dark Metal Energy are prints. Mr. Stone's Project 88 finds only basic Energy cards (Emerald's finds Basic Pokémon too),
    /// so it's its own card. Compendium: δ Rainbow Energy and Rainbow Energy are different cards (4 of each in a deck).
    /// </summary>
    public static class CardsHPT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("HP-85", new HolonAdventurer());
            r.RegisterByKey("HP-86", new HolonFossil());
            r.RegisterByKey("HP-87", new HolonLake());
            r.RegisterByKey("HP-88", new MrStonesProject());
            r.RegisterByKey("HP-98", new DeltaRainbowEnergy());
        }

        /// <summary>A δ Rainbow Energy card.</summary>
        internal static bool IsDeltaRainbow(CardDef d) { return d != null && d.IsSpecialEnergy && d.Name == "δ Rainbow Energy"; }

        /// <summary>Holon Adventurer (Supporter): discard a card from your hand (you must), then draw 3 cards, or 4 if it was a δ Pokémon.</summary>
        private sealed class HolonAdventurer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var pick = ctx.ChooseCards(others, 1, 1, "Holon Adventurer: discard a card from your hand (a δ Pokémon draws 4)")[0];
                bool delta = pick.Def.IsPokemon && pick.Def.IsDeltaSpecies;
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(delta ? 4 : 3);
            }
        }

        /// <summary>
        /// Holon Fossil: flip a coin; heads, an Omanyte, Kabuto, Aerodactyl, Aerodactyl ex, Lileep or Anorith from your deck onto your
        /// Bench; tails, one from your hand. Either way it's a Basic Pokémon.
        /// </summary>
        private sealed class HolonFossil : CardScript
        {
            private static readonly string[] Names = CardsLMB.Ancients.Concat(new[] { "Aerodactyl ex" }).ToArray();

            private static bool Fits(EffectContext ctx, CardDef d) { return d.IsPokemon && Names.Contains(d.Name) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                CardInstance card = null;
                if (ctx.Flip())
                {
                    if (ctx.Me.Deck.Count == 0) return;
                    var found = ctx.SearchMyDeck(d => Fits(ctx, d), 1, "Holon Fossil: choose an Omanyte, Kabuto, Aerodactyl, Aerodactyl ex, Lileep or Anorith");
                    if (found.Count > 0) card = found[0];
                    ctx.ShuffleMyDeck();
                }
                else
                {
                    var cards = ctx.Me.Hand.Where(c => c != ctx.Card && Fits(ctx, c.Def)).ToList();
                    if (cards.Count == 0) return;
                    card = ctx.ChooseCards(cards, 1, 1, "Holon Fossil: put which Pokémon from your hand onto your Bench?")[0];
                    if (!ctx.Me.Hand.Remove(card)) return;
                }
                if (card == null) return;
                if (ctx.BenchSpace <= 0)
                {
                    ctx.Me.Hand.Add(card);
                    return;
                }
                var mon = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                if (mon != null) mon.Markers[PokemonInPlay.ConsideredBasicMarker] = 1;
            }
        }

        /// <summary>Holon Lake: each player's δ Pokémon can use this card's attack, Delta Call (a δ Pokémon from your deck into your hand).</summary>
        private sealed class HolonLake : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                var lake = src.Card;
                if (lake == null || mon == null || lake.Def.Attacks.Count == 0 || !CardsDS.Delta(mon)) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(lake.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsPokemon && d.IsDeltaSpecies, 1, "Delta Call: choose a δ Pokémon");
            }
        }

        /// <summary>Mr. Stone's Project (Supporter): up to 2 basic Energy cards from your deck, or from your discard pile, into your hand (choose 1 before looking: Compendium).</summary>
        private sealed class MrStonesProject : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var discard = ctx.BasicEnergyInDiscard();
                bool fromDiscard = discard.Count > 0 && (ctx.Me.Deck.Count == 0 || ctx.ChooseOption("Mr. Stone's Project: search where?", "Your deck", "Your discard pile") == 1);
                if (!fromDiscard)
                {
                    ctx.SearchToHand(d => d.IsBasicEnergy, 2, "Mr. Stone's Project: choose up to 2 basic Energy cards");
                    return;
                }
                var picks = ctx.ChooseCards(discard, 0, Math.Min(2, discard.Count), "Mr. Stone's Project: choose up to 2 basic Energy cards from your discard pile");
                ctx.Engine.ShowCards(ctx.Opponent, picks, "Mr. Stone's Project", "Mr. Stone's Project: the cards your opponent took", ctx.Source);
                ctx.DiscardToHand(picks);
            }
        }

        /// <summary>δ Rainbow Energy: [C]; on a δ Pokémon, every type but 1 Energy at a time.</summary>
        private sealed class DeltaRainbowEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return new List<int> { CardsDS.Delta(ctx.Pokemon) ? EnergyMask.AnyType : EnergyMask.Colorless };
            }
        }
    }
}
