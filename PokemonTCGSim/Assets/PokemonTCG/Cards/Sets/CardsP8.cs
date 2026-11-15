using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>POP Series 8 (P8, 2008; v172): the cards not already in the pool (the others are prints). Unlimited only.</summary>
    public static class CardsP8
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P8-1", new ComboMon(new ParalyzeFlipMon("Body Slam"), new FireSpinBasicsMon()));
            r.RegisterByKey("P8-2", new ComboMon(new CardsASC.SelfGuardMon("Blocking Punch", 20), new StrikingKickMon()));
            r.RegisterByKey("P8-3", new CardsDR.IntimidatingFangMon(new CardsFST1.FlipRecoilMon("Thunder", 40)));
            r.RegisterByKey("P8-4", new ComboMon(new CardsFST1.TwoBenchedMon("Rock Slide", 10), new CardsRSB.CoinsPlusMon("Triple Nose", 3, 20)));
            r.RegisterByKey("P8-5", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsDR.FlipDiscardSelfMon("Air Slash", null)));
            r.RegisterByKey("P8-8", new ComboMon(new FlipBonusMon("Fasten Claw", 30), new ParalyzeFlipMon("Thunder Fang")));
            r.RegisterByKey("P8-16", new CardsDX.ConditionImmuneMon(SpecialCondition.Paralyzed, new FlipBonusMon("Quick Attack", 10)));
        }

        /// <summary>Fire Spin: discard 2 basic Energy cards from this Pokémon (can't: nothing).</summary>
        private sealed class FireSpinBasicsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Spin") return false;
                var me = ctx.Pokemon;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (basics.Count < 2) return Nothing(ctx);
                foreach (var card in ctx.ChooseCards(basics, 2, 2, "Fire Spin: discard 2 basic Energy cards")) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Striking Kick: the damage ignores Resistance and effects on the Defending Pokémon.</summary>
        private sealed class StrikingKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Striking Kick") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }
    }
}
