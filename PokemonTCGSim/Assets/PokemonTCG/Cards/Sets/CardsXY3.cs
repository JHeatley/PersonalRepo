using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// XY (XY, v105), part 3: the Trainers new to the pool (Cassius, Hard Charm, Muscle Band, Shadow Circle). Evosoda, Fairy Garden,
    /// Great Ball, Max Revive, Professor Sycamore, Professor's Letter, Red Card, Roller Skates, Shauna, Super Potion, Team Flare
    /// Grunt, Double Colorless Energy, Rainbow Energy and the basic Energy were already in it.
    /// </summary>
    public static class CardsXY3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByName("Cassius", new Cassius());
            r.RegisterByName("Hard Charm", new HardCharm());
            r.RegisterByName("Muscle Band", new MuscleBand());
            r.RegisterByName("Shadow Circle", new ShadowCircle());
        }

        /// <summary>Cassius: shuffle 1 of your Pokémon and all attached cards into your deck (even your only one: you then lose; your ruling).</summary>
        private sealed class Cassius : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mon = ctx.ChoosePokemon(ctx.MyPokemon(), "Cassius: shuffle which of your Pokémon and all attached cards into your deck?");
                if (mon == null) return;
                ctx.Engine.ShufflePokemonIntoDeck(mon);
            }
        }

        /// <summary>Hard Charm: 20 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class HardCharm : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == holder.Owner) return damage;
                return damage - 20;
            }
        }

        /// <summary>Muscle Band: the holder's attacks do 20 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class MuscleBand : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Attacker != holder || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 20;
            }
        }

        /// <summary>Shadow Circle: each Pokémon with any Darkness Energy attached (both players') has no Weakness.</summary>
        private sealed class ShadowCircle : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon.Energy.Any(en => CardsDRI1.ProvidesType(src, mon, en, EnergyType.Darkness));
            }
        }
    }
}
