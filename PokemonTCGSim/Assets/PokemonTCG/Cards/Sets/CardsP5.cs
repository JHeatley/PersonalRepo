using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// POP Series 5 (P5, 2007; v171): the cards not already in the pool (Bill's Maintenance, Boost Energy, Umbreon ★ and the others
    /// are prints). Unlimited only.
    /// </summary>
    public static partial class CardsP5
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P5-1", new CardsASC.SelfDiscardTypeMon("Fire Blast", EnergyType.Fire, 1));
            r.RegisterByKey("P5-2", new AerowingMon());
            r.RegisterByKey("P5-3", new ComboMon(new CardsE2.CopyMon(), new ExtraDrawMon()));
            r.RegisterByKey("P5-11", new CardsCES1.FeintAttackMon("Feint Attack", 10));
            r.RegisterByKey("P5-12", new CardsTWM1.SelfDiscardMon("Thunderbolt", 99));
            r.RegisterByKey("P5-13", new HeavyMetalMon());
            r.RegisterByKey("P5-14", new PelipperMist());
            r.RegisterByKey("P5-15", new DodgeMon("Detect", false));
            r.RegisterByKey("P5-16", new EspeonStarPurpleRay());
        }

        /// <summary>Aerowing: before the damage you may flip a coin; tails, the attack does nothing; heads, it does 60 instead.</summary>
        private sealed class AerowingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aerowing") return false;
                if (ctx.AskYesNo("Aerowing: flip a coin? Heads, 60 damage instead of " + atk.Damage + "; tails, the attack does nothing."))
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(60);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Extra Draw: if your opponent has any Pokémon-ex in play, attach up to 2 basic Energy cards from your deck to this Pokémon.</summary>
        private sealed class ExtraDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra Draw") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.OpponentPokemon().Any(m => m.Def.IsOldEx) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 2, "Extra Draw: choose up to 2 basic Energy cards to attach to " + me.Name);
                foreach (var card in found)
                    if (!ctx.InPlay(me) || !ctx.Engine.AttachEnergyCard(card, me))
                        if (!ctx.Me.Deck.Contains(card) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Heavy Metal: a coin for each Metal Energy attached; 20 more for each heads.</summary>
        private sealed class HeavyMetalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Metal") return false;
                int metal = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal);
                int heads = metal > 0 ? ctx.FlipCoins(metal) : 0;
                ctx.DamageDefending(atk.Damage + 20 * heads);
                return true;
            }
        }

        /// <summary>
        /// Mist (Poké-Body): attack damage from Stage 2 Evolved Pokémon (both players') is 30 less (after W/R). Lightning Wing: 10 to
        /// 1 of your own Benched Pokémon (no W/R).
        /// </summary>
        private sealed class PelipperMist : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return a.Def.Stage == Stage.Stage2 && CardsE2.IsEvolved(a) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Wing") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Lightning Wing: 10 damage to which of your Benched Pokémon?");
                if (target != null) ctx.DamageWith(target, 10, false, false, false);
                return true;
            }
        }

        /// <summary>Purple Ray (Poké-Power): when you put Espeon ★ onto your Bench from your hand, each Active Pokémon (both players') is now Confused. Psychic Boom.</summary>
        private sealed class EspeonStarPurpleRay : Mon
        {
            private const string Power = "Purple Ray";
            private static readonly Mon Boom = new CardsASR1.PlusMon("Psychic Boom", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending));

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, ctx.Me.Active != null || ctx.Opp.Active != null, "Use " + Power + "? Each Active Pokémon (both yours and your opponent's) is now Confused.")) return;
                foreach (var active in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (active != null && ctx.InPlay(active)) ctx.ApplyCondition(active, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Boom.HandleAttack(ctx, atk); }
        }
    }
}
