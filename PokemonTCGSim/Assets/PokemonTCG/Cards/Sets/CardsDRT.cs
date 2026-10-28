using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Trainer cards (DR 83, 85-87; v159). Balloon Berry 82 is Neo Revelation's, Energy Recycle System 84 and TV Reporter 88
    /// are Celestial Storm's (prints).
    /// </summary>
    public static class CardsDRT
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DR-83", new BufferPiece());
            r.RegisterByKey("DR-85", new HighPressureSystem());
            r.RegisterByKey("DR-86", new LowPressureSystem());
            r.RegisterByKey("DR-87", new MrBrineysCompassion());
        }

        /// <summary>
        /// Buffer Piece (Pokémon Tool): the opponent's attacks do 20 less damage to the holder (after Weakness and Resistance);
        /// discarded at the end of the opponent's turn after you played it.
        /// </summary>
        private sealed class BufferPiece : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? Math.Max(0, damage - 20) : damage;
            }

            public override bool DiscardedAtEndOfOpponentsTurn { get { return true; } }
        }

        /// <summary>High Pressure System (Stadium): each player's Fire and Water Pokémon have a Retreat Cost 1 less.</summary>
        private sealed class HighPressureSystem : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || cost <= 0) return cost;
                return src.Engine.IsType(mon, EnergyType.Fire) || src.Engine.IsType(mon, EnergyType.Water) ? cost - 1 : cost;
            }
        }

        /// <summary>Low Pressure System (Stadium): each Grass and Lightning Pokémon in play gets +10 HP.</summary>
        private sealed class LowPressureSystem : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null) return hp;
                return src.Engine.IsType(mon, EnergyType.Grass) || src.Engine.IsType(mon, EnergyType.Lightning) ? hp + 10 : hp;
            }
        }

        /// <summary>
        /// Mr. Briney's Compassion (Supporter): 1 of your Pokémon in play other than a Pokémon-ex, and everything attached, back into
        /// your hand. Your Active Pokémon only with a Benched Pokémon to take its place.
        /// </summary>
        private sealed class MrBrineysCompassion : CardScript
        {
            private static System.Collections.Generic.List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => !m.Def.IsOldEx && (ctx.Me.Bench.Count > 0 || m != ctx.Me.Active));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Mr. Briney's Compassion: return which Pokémon and everything attached to your hand?");
                if (mon != null) ctx.Engine.ReturnPokemonToHand(mon);
            }
        }
    }
}
