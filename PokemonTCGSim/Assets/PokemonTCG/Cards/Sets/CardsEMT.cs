using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Emerald Trainer and Energy cards (EM 75, 79, 84, 86, 87; v166). The others are prints. Darkness Energy 86 and Double Rainbow
    /// Energy 87 are their own cards (your rulings).
    /// </summary>
    public static class CardsEMT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EM-75", new BattleFrontier());
            r.RegisterByKey("EM-79", new MrStonesProject());
            r.RegisterByKey("EM-84", new Scott());
            r.RegisterByKey("EM-86", new DarknessEnergyEM());
            r.RegisterByKey("EM-87", new DoubleRainbowEnergyEM());
        }

        /// <summary>
        /// Battle Frontier (Stadium): each player's Colorless, Darkness and Metal Evolved Pokémon can't use Poké-Powers or Poké-Bodies.
        /// The printed types are used, so a Poké-Body that changes types can't switch itself off in a loop.
        /// </summary>
        private sealed class BattleFrontier : CardScript
        {
            private static bool Covers(PokemonInPlay mon)
            {
                if (mon == null || !CardsE2.IsEvolved(mon)) return false;
                var d = mon.Def;
                return d.HasType(EnergyType.Colorless) || d.HasType(EnergyType.Darkness) || d.HasType(EnergyType.Metal);
            }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return Covers(mon); }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return Covers(mon); }
        }

        /// <summary>Mr. Stone's Project (Supporter): up to 2 Basic cards (Basic Pokémon or basic Energy) from your deck, or from your discard pile, into your hand.</summary>
        private sealed class MrStonesProject : CardScript
        {
            private static bool Basic(CardDef d) { return d.IsBasicPokemon || d.IsBasicEnergy; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || ctx.Me.Discard.Any(c => Basic(c.Def)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool fromDiscard = ctx.Me.Discard.Any(c => Basic(c.Def))
                    && (ctx.Me.Deck.Count == 0 || ctx.ChooseOption("Mr. Stone's Project: search where?", "Your deck", "Your discard pile") == 1);
                if (!fromDiscard)
                {
                    ctx.SearchToHand(Basic, 2, "Mr. Stone's Project: choose up to 2 Basic Pokémon or basic Energy cards");
                    return;
                }
                var cards = ctx.Me.Discard.Where(c => Basic(c.Def)).ToList();
                var picks = ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Mr. Stone's Project: put up to 2 Basic cards from your discard pile into your hand");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Me.Hand.Add(card);
                if (picks.Count > 0) ctx.Engine.Reveal(ctx.Player, picks, "put these cards from " + ctx.Engine.Their(ctx.Player) + " discard pile into " + ctx.Engine.Their(ctx.Player) + " hand (Mr. Stone's Project)");
            }
        }

        /// <summary>Scott (Supporter): up to 3 Supporter and Stadium cards in any combination from your deck into your hand.</summary>
        private sealed class Scott : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter || d.IsStadium, 3, "Scott: choose up to 3 Supporter and Stadium cards");
            }
        }

        /// <summary>Darkness Energy (Emerald): Darkness; its Pokémon's attacks do 10 more to the Active Pokémon (before W/R) if it's Darkness or has Dark in its name.</summary>
        private sealed class DarknessEnergyEM : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Darkness), 1); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(holder, EnergyType.Darkness) || CardsTRR.Dark(holder.Name) ? damage + 10 : damage;
            }
        }

        /// <summary>
        /// Double Rainbow Energy (Emerald): only on an Evolved Pokémon that isn't a Pokémon-ex; every type, 2 Energy at a time; its
        /// Pokémon's damage to the opponent's Pokémon is 10 less (before Weakness and Resistance); discarded when it's no longer Evolved.
        /// </summary>
        private sealed class DoubleRainbowEnergyEM : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 2); }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return CardsE2.IsEvolved(target) && !target.Def.IsOldEx; }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !CardsE2.IsEvolved(ctx.Pokemon); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || info.Target == null || info.Target.Owner == src.Player || damage <= 0) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? Math.Max(0, damage - 10) : damage;
            }
        }
    }
}
