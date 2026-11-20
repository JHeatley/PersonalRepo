using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// POP Series 4 (P4, 2006; v171): the cards not already in the pool (the others are prints; Pikachu 13 is its own card, its
    /// Spark costs LC). Unlimited only.
    /// </summary>
    public static partial class CardsP4
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P4-1", new ComboMon(new ParalyzeFlipMon("Wrap"), new NoWeaknessResistanceMon("Sonicboom")));
            r.RegisterByKey("P4-3", new CardsRG.EnergyRetreatMon(true, new NoRetreatMon("Sand Pit")));
            r.RegisterByKey("P4-4", new MewReactiveBarrier());
            r.RegisterByKey("P4-6", new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"), new CardsFLF1.NoResistanceMon("Sky Uppercut")));
            r.RegisterByKey("P4-7", new ComboMon(new FlipBonusMon("Quick Attack", 10), new CardsE3B.HealSelfMon("Blot", 20)));
            r.RegisterByKey("P4-11", new ConditionAttackMon("Super Hypno Wave", SpecialCondition.Asleep));
            r.RegisterByKey("P4-12", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("P4-13", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("P4-16", new ComboMon(new CardsE3B.HealSelfMon("Nap", 20), new CardsASC.SelfGuardMon("Expand", 20)));
        }

        /// <summary>"This attack's damage isn't affected by Weakness or Resistance" (Sonicboom).</summary>
        internal sealed class NoWeaknessResistanceMon : Mon
        {
            private readonly string attack;

            public NoWeaknessResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>Reactive Barrier (Poké-Body): with a React Energy card attached, no effects other than damage from the opponent's attacks on Mew. Psyshock.</summary>
        private sealed class MewReactiveBarrier : Mon
        {
            private static readonly Mon Psyshock = new ParalyzeFlipMon("Psyshock");

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                if (me == null || target != me || kind != EffectSourceKind.Attack || !me.Energy.Any(e => e.Def.Name == "React Energy")) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psyshock.HandleAttack(ctx, atk); }
        }
    }
}
