using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Roaring Skies (ROS, v99), part 3: the Trainers and Double Dragon Energy (83-97, VS Seeker 110). Trainers' Mail, Professor
    /// Sycamore-era staples and Switch were already in the pool; the full arts (98-108) are reprints.
    /// </summary>
    public static class CardsROS3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Gallade Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Healing Scarf", new HealingScarf());
            r.RegisterByName("Latios Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Mega Turbo", new MegaTurbo());
            r.RegisterByName("Rayquaza Spirit Link", new CardsEVO3.SpiritLink());
            r.RegisterByName("Sky Field", new SkyField());
            r.RegisterByName("Wide Lens", new WideLens());
            r.RegisterByName("Winona", new Winona());
            r.RegisterByName("Double Dragon Energy", new DoubleDragonEnergy());
            r.RegisterByName("VS Seeker", new VSSeeker());
        }

        /// <summary>Healing Scarf: whenever you attach an Energy card from your hand to this Pokémon, heal 20 damage from it.</summary>
        private sealed class HealingScarf : CardScript
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || target.DamageCounters == 0) return;
                src.Heal(target, 20);
            }
        }

        /// <summary>Mega Turbo: attach a basic Energy card from your discard pile to 1 of your Mega Evolution Pokémon (Primal Pokémon count: 2015).</summary>
        private sealed class MegaTurbo : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard().Count > 0 && ctx.MyPokemon().Any(m => m.Def.IsXYMegaEvolution);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 1, (card, mon) => mon.Def.IsXYMegaEvolution,
                    "Mega Turbo: attach a basic Energy card from your discard pile to 1 of your Mega Evolution Pokémon", false);
            }
        }

        /// <summary>
        /// Sky Field: each player can have 8 Pokémon on their Bench. When it leaves play, each player discards Benched Pokémon down to
        /// 5, its owner first (at once, before promoting: 2015-04-30). Replaced by a Stadium that allows the bigger Bench, nothing is
        /// discarded (the engine runs "leaves play" after the new Stadium is in: 2024-09-26).
        /// </summary>
        private sealed class SkyField : CardScript
        {
            public override int ModifyMaxBench(EffectContext src, int player, int max) { return Math.Max(max, 8); }

            public override void OnLeavePlay(EffectContext ctx)
            {
                ctx.Engine.EnforceBenchLimits(ctx.Player);
            }
        }

        /// <summary>Wide Lens: damage this Pokémon's attacks do to the opponent's Benched Pokémon is affected by Weakness and Resistance (2015-05-07).</summary>
        private sealed class WideLens : CardScript
        {
            public override bool AppliesWeaknessAndResistanceOnBench(EffectContext src, DamageInfo info)
            {
                return src.Pokemon != null && info.Attacker == src.Pokemon;
            }
        }

        /// <summary>Winona: search your deck for up to 3 Colorless Pokémon, show them, put them into your hand.</summary>
        private sealed class Winona : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && (d.PokemonType == EnergyType.Colorless || d.SecondType == EnergyType.Colorless), 3,
                    "Winona: choose up to 3 Colorless Pokémon");
            }
        }

        /// <summary>
        /// Double Dragon Energy: only on a Dragon Pokémon (discarded otherwise); provides every type of Energy, 2 at a time. Tag
        /// Switch can move it by 1 unit (2019-09-12): it's moved as a card.
        /// </summary>
        private sealed class DoubleDragonEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Dragon)) return new List<int>();
                return RecifeEnergy.Units(EnergyMask.AnyType, 2);
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && ctx.Engine.IsType(target, EnergyType.Dragon); }

            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Engine.IsType(ctx.Pokemon, EnergyType.Dragon); }
        }

        /// <summary>VS Seeker: put a Supporter card from your discard pile into your hand (one played this turn too: 2011-04-28).</summary>
        private sealed class VSSeeker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.IsSupporter); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return;
                var pick = supporters.Count == 1 ? supporters[0] : ctx.ChooseCards(supporters, 1, 1, "VS Seeker: put a Supporter card from your discard pile into your hand")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
            }
        }
    }
}
