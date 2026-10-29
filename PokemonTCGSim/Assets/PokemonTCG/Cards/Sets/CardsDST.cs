using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Delta Species Trainer and Energy cards (DS 91-98, 104-106; v169). The Holon Supporters cost discarding another card from the
    /// hand: without one they can't be played. Dual Ball 89 (Team Magma vs Team Aqua's), Great Ball, Master Ball, Super Scoop Up,
    /// Potion, Switch, Darkness Energy and Metal Energy are prints. Holon Energy WP only stops effects of attacks (Compendium, 2006-07-25).
    /// </summary>
    public static class CardsDST
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DS-91", new HolonFarmer());
            r.RegisterByKey("DS-92", new HolonLass());
            r.RegisterByKey("DS-93", new HolonMentor());
            r.RegisterByKey("DS-94", new HolonResearchTower());
            r.RegisterByKey("DS-95", new HolonResearcher());
            r.RegisterByKey("DS-96", new HolonRuins());
            r.RegisterByKey("DS-97", new HolonScientist());
            r.RegisterByKey("DS-98", new HolonTransceiver());
            r.RegisterByKey("DS-104", new HolonEnergyFF());
            r.RegisterByKey("DS-105", new HolonEnergyGL());
            r.RegisterByKey("DS-106", new HolonEnergyWP());
        }

        // ================================================================== Holon Supporters

        /// <summary>
        /// "Discard a card from your hand. If you can't discard a card from your hand, you can't play this card." The discard is a
        /// cost, paid before the effect (so Holon Farmer can take that card back: Compendium, 2005-12-08).
        /// </summary>
        internal abstract class HolonSupporter : CardScript
        {
            protected virtual bool Ready(EffectContext ctx) { return true; }

            protected abstract void Effect(EffectContext ctx);

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card) && Ready(ctx); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var pick = ctx.ChooseCards(others, 1, 1, ctx.SourceName + ": discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                Effect(ctx);
            }
        }

        /// <summary>
        /// Holon Farmer: 3 basic Energy cards and 3 Basic Pokémon or Evolution cards from your discard pile on top of your deck, then
        /// shuffle. As many as you can up to 3 of each, and 3 if there are 3 or more (Compendium, 2005-12-08).
        /// </summary>
        private sealed class HolonFarmer : HolonSupporter
        {
            protected override void Effect(EffectContext ctx)
            {
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                var pokemon = ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList();
                int ne = Math.Min(3, energy.Count), np = Math.Min(3, pokemon.Count);
                var picks = new List<CardInstance>();
                if (ne > 0) picks.AddRange(ctx.ChooseCards(energy, ne, ne, "Holon Farmer: choose " + ne + " basic Energy cards"));
                if (np > 0) picks.AddRange(ctx.ChooseCards(pokemon, np, np, "Holon Farmer: choose " + np + " Basic Pokémon or Evolution cards"));
                if (picks.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, picks, "put these cards from their discard pile into their deck (Holon Farmer)");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Holon Lass: look at as many cards from the top of your deck as there are Prize cards left (both players'); any Energy cards among them into your hand; shuffle.</summary>
        private sealed class HolonLass : HolonSupporter
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Effect(EffectContext ctx)
            {
                int n = ctx.Me.Prizes.Count + ctx.Opp.Prizes.Count;
                var top = ctx.LookAtTop(n);
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, energy.Count, "Holon Lass: put which Energy cards into your hand?");
                    if (picks.Count > 0)
                    {
                        ctx.Engine.Reveal(ctx.Player, picks, "put these Energy cards into their hand (Holon Lass)");
                        ctx.DeckToHand(picks);
                    }
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Holon Mentor: up to 3 Basic Pokémon with 100 HP or less from your deck into your hand.</summary>
        private sealed class HolonMentor : HolonSupporter
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Effect(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicPokemon && !d.IsLegend && d.Hp <= 100, 3, "Holon Mentor: choose up to 3 Basic Pokémon with 100 HP or less");
            }
        }

        /// <summary>
        /// Holon Researcher: a Metal Energy card (only one named Metal Energy: Compendium, 2005-12-01) or a δ Basic Pokémon or Evolution
        /// card from your deck into your hand.
        /// </summary>
        private sealed class HolonResearcher : HolonSupporter
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Effect(EffectContext ctx)
            {
                ctx.SearchToHand(d => (d.IsEnergy && d.Name == "Metal Energy") || (d.IsPokemon && d.IsDeltaSpecies), 1,
                    "Holon Researcher: choose a Metal Energy card or a δ Pokémon");
            }
        }

        /// <summary>Holon Scientist: with fewer cards in your hand than the opponent, draw until you have as many. You must be able to draw after paying the discard (Compendium, 2007-01-04).</summary>
        private sealed class HolonScientist : HolonSupporter
        {
            protected override bool Ready(EffectContext ctx)
            {
                int after = ctx.Me.Hand.Count(c => c != ctx.Card) - 1;
                return ctx.Me.Deck.Count > 0 && after < ctx.Opp.Hand.Count;
            }

            protected override void Effect(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count < ctx.Opp.Hand.Count) ctx.DrawUntil(ctx.Opp.Hand.Count);
            }
        }

        /// <summary>
        /// Holon Transceiver: a Supporter card with Holon in its name, from your deck or from your discard pile into your hand. The
        /// place is chosen first; a failed deck search doesn't go on to the discard pile (Compendium, 2006-01-05).
        /// </summary>
        private sealed class HolonTransceiver : CardScript
        {
            private static bool HolonSupporterCard(CardDef d) { return d.IsSupporter && d.Name.Contains("Holon"); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Discard.Any(c => HolonSupporterCard(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool fromDeck = ctx.Me.Deck.Count > 0;
                if (fromDeck && ctx.Me.Discard.Any(c => HolonSupporterCard(c.Def)))
                    fromDeck = ctx.ChooseOption("Holon Transceiver: search where?", "Your deck", "Your discard pile") == 0;
                if (fromDeck)
                {
                    ctx.SearchToHand(HolonSupporterCard, 1, "Holon Transceiver: choose a Supporter card with Holon in its name");
                    return;
                }
                var cards = ctx.Me.Discard.Where(c => HolonSupporterCard(c.Def)).ToList();
                if (cards.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(cards, 1, 1, "Holon Transceiver: put which Supporter card into your hand?"));
            }
        }

        // ================================================================== Stadiums

        /// <summary>
        /// Holon Research Tower: basic Energy cards attached to δ Pokémon (both players') provide Metal Energy as well as their own type,
        /// 1 Energy at a time. They're still basic Energy cards of their type (Compendium, 2005-11-03).
        /// </summary>
        private sealed class HolonResearchTower : CardScript
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (energy == null || holder == null || !energy.Def.IsBasicEnergy || !holder.Def.IsDeltaSpecies) return;
                for (int i = 0; i < units.Count; i++)
                    if (units[i] != 0) units[i] |= EnergyMask.Of(EnergyType.Metal);
            }
        }

        /// <summary>Holon Ruins: once during their turn, a player with a δ Pokémon in play may draw a card; if they do, they discard a card from their hand.</summary>
        private sealed class HolonRuins : CardScript
        {
            private const string Limit = "Holon Ruins";

            public override bool CanUseStadium(EffectContext ctx)
            {
                return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Deck.Count > 0 && ctx.Me.InPlay().Any(m => m.Def.IsDeltaSpecies);
            }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (ctx.Draw(1) == 0 || ctx.Me.Hand.Count == 0) return false;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Holon Ruins: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                return false;
            }
        }

        // ================================================================== Holon Energy

        /// <summary>The Pokémon a Holon Energy card is attached to, when its effects apply (not on Pokémon-ex), or null.</summary>
        private static PokemonInPlay Holder(EffectContext src)
        {
            var mon = src.Pokemon;
            return mon != null && mon.Energy.Contains(src.Card) && !mon.Def.IsOldEx ? mon : null;
        }

        /// <summary>The holder also has a basic Energy card of that type attached (one is enough for any number of Holon Energy: Compendium).</summary>
        private static bool AlsoHas(PokemonInPlay holder, EnergyType type)
        {
            return holder != null && holder.Energy.Any(e => CardsDS.BasicOf(e.Def, type));
        }

        /// <summary>Holon Energy FF: with a basic Fire Energy card too, no Weakness; with a basic Fighting Energy card too, its attacks' damage isn't affected by Resistance.</summary>
        private sealed class HolonEnergyFF : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                var holder = Holder(src);
                return holder != null && mon == holder && AlsoHas(holder, EnergyType.Fire);
            }

            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                var holder = Holder(src);
                return holder != null && info.Attacker == holder && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && AlsoHas(holder, EnergyType.Fighting);
            }
        }

        /// <summary>
        /// Holon Energy GL: with a basic Grass Energy card too, its Pokémon can't be affected by Special Conditions (attaching it cures
        /// them: Compendium); with a basic Lightning Energy card too, damage done to it by the opponent's Pokémon-ex is 10 less after W/R
        /// (each Holon Energy GL counts, only for the Pokémon it's attached to).
        /// </summary>
        private sealed class HolonEnergyGL : CardScript
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                var holder = Holder(src);
                return holder != null && target == holder && AlsoHas(holder, EnergyType.Grass);
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = Holder(src);
                var a = info.Attacker;
                if (holder == null || info.Target != holder || a == null || a.Owner == holder.Owner || !a.Def.IsOldEx || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return AlsoHas(holder, EnergyType.Lightning) ? damage - 10 : damage;
            }
        }

        /// <summary>
        /// Holon Energy WP: with a basic Water Energy card too, prevent all effects (not damage) of the opponent's attacks on its Pokémon,
        /// damage counters included (only attacks: Compendium, 2006-07-25); with a basic Psychic Energy card too, its Retreat Cost is 0.
        /// </summary>
        private sealed class HolonEnergyWP : CardScript
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var holder = Holder(src);
                if (holder == null || target != holder || kind != EffectSourceKind.Attack || !AlsoHas(holder, EnergyType.Water)) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != holder.Owner;
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var holder = Holder(src);
                return holder != null && mon == holder && AlsoHas(holder, EnergyType.Psychic) ? 0 : cost;
            }
        }
    }
}
