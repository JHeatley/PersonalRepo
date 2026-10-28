using System;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>POP Series 1 (P1 1-17; v163). Unlimited only. Armaldo ex and Tyranitar ex have two Weaknesses.</summary>
    public static class CardsP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P1-1", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("P1-2", new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"));
            r.RegisterByKey("P1-3", new DodgeMon("Fly", true));
            r.RegisterByKey("P1-4", new ComboMon(new CardsDAA1.DrainMon("Cling"), new FlipBonusMon("Leaf Blade", 30)));
            r.RegisterByKey("P1-5", new ComboMon(new ParalyzeFlipMon("Bubble"), new CardsG1.ChooseBenchedMon("Mud Splash", 1, 20, true)));
            r.RegisterByKey("P1-6", new ComboMon(new CardsTWM1.HealSelfMon("Blot", 10), new CardsE3B.OpponentSwitchesMon("Whirlwind")));
            r.RegisterByKey("P1-7", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("P1-8", new InsomniaMon(new CardsCES1.FeintAttackMon("Feint Attack", 20)));
            r.RegisterByKey("P1-9", new FlipBonusMon("Rock Smash", 10));
            r.RegisterByKey("P1-10", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("P1-11", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("P1-12", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new CardsFST1.TwoBenchedMon("Spark", 10)));
            r.RegisterByKey("P1-13", new ComboMon(new FlipBonusMon("Quick Attack", 10), new DodgeMon("Agility", false)));
            r.RegisterByKey("P1-14", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("P1-15", new SwellowFocusEnergy());
            r.RegisterByKey("P1-16", new ComboMon(new CardsRS.TextDamageMon("Twin-blade", 30), new SupersonicClawsMon()));
            r.RegisterByKey("P1-17", new CardsRS.DiscardBasicOrNothingMon("Critical Crush", 2, false));
        }

        /// <summary>Insomnia (Poké-Body): this Pokémon can't be Asleep.</summary>
        internal sealed class InsomniaMon : Mon
        {
            private readonly Mon attacks;

            public InsomniaMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon && condition == SpecialCondition.Asleep;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Focus Energy: during your next turn, Agility's base damage is 70. Agility: heads, prevent all effects of attacks, damage included, next turn.</summary>
        private sealed class SwellowFocusEnergy : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Agility") return false;
                bool focused = me.LastAttackName == "Focus Energy" && me.LastAttackTurn == ctx.MyLastTurn;
                bool heads = ctx.Flip();
                ctx.DamageDefending(focused ? 70 : atk.Damage);
                if (heads && ctx.InPlay(me)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Supersonic Claws: the damage isn't affected by Resistance.</summary>
        private sealed class SupersonicClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supersonic Claws") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, false, false);
                return true;
            }
        }
    }
}
