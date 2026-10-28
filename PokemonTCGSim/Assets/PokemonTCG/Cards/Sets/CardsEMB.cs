using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Emerald Uncommon and Common Pokémon (EM 22-74; v166). Unlimited only.</summary>
    public static class CardsEMB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EM-22", new ComboMon(new CardsASR1.PlusMon("Power Blow", 10, c => c.EnergyCount(c.Pokemon)), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("EM-23", new ComboMon(CardsSS.ConditionBonus("Extra Flame", 30, d => d.Def.IsOldEx), new ConditionAttackMon("Super Singe", SpecialCondition.Burned)));
            r.RegisterByKey("EM-24", new CardsRG.EnergyRetreatMon(true, new FlipBonusMon("Rock Smash", 10)));
            r.RegisterByKey("EM-25", new ComboMon(new CardsDR.AlsoOneBenchedMon("Lava Burn", 10), new FlipBonusMon("Sharp Claws", 10)));
            r.RegisterByKey("EM-26", new SwitchSelfMon("Smash Turn", true));
            r.RegisterByKey("EM-27", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("EM-28", new ConditionAttackMon("Tail Shake", SpecialCondition.Asleep));
            r.RegisterByKey("EM-29", new ComboMon(new ParalyzeFlipMon("Psyshock"), new SwitchSelfMon("Teleport Blast", true)));
            r.RegisterByKey("EM-30", new CardsSSB.AnyOpponentTextMon("Snap Tail", 10));
            r.RegisterByKey("EM-31", new ComboMon(CardsSS.ConditionBonus("Cross-Cut", 20, CardsE2.IsEvolved), new ParalyzeFlipMon("Shove")));
            r.RegisterByKey("EM-32", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("EM-33", new CardsHL.NoWeaknessResistanceMon("Mind Shock"));
            r.RegisterByKey("EM-34", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("EM-35", new ComboMon(new ParalyzeFlipMon("Body Slam"), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("EM-36", new ConditionAttackMon("Super Hypno Wave", SpecialCondition.Asleep));
            r.RegisterByKey("EM-37", new MinunElectroGuard());
            r.RegisterByKey("EM-38", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            r.RegisterByKey("EM-39", new ComboMon(new DrawAttackMon("Collect", 1), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("EM-40", new ComboMon(new CardsTWM1.HealSelfMon("Blot", 10), new ConditionAttackMon("Poison Breath", SpecialCondition.Poisoned)));
            r.RegisterByKey("EM-41", new CardsSSB.AnyOpponentTextMon("Skill Dive", 40));
            r.RegisterByKey("EM-42", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("EM-43", new PlainMon());
            r.RegisterByKey("EM-44", new CoinsMon("Spike Cannon", 3));
            r.RegisterByKey("EM-45", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("EM-46", new PlainMon());
            r.RegisterByKey("EM-47", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("EM-48", new PlainMon());
            r.RegisterByKey("EM-49", new BenchShieldMon(false, new TailsNothingMon("Lunge")));
            r.RegisterByKey("EM-50", new CardsSSB.AnyOpponentTextMon("Water Arrow", 10));
            r.RegisterByKey("EM-51", new PlainMon());
            r.RegisterByKey("EM-52", new DigUnderMon(20));
            r.RegisterByKey("EM-53", new ComboMon(new BenchSearchAttackMon("Call for Friend", 1), new CardsEM.EvolvedConditionMon("Strange Scale", SpecialCondition.Confused)));
            r.RegisterByKey("EM-54", new FlipBonusMon("Cross Chop", 10));
            r.RegisterByKey("EM-55", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("EM-56", new FlipBonusMon("Tail Strike", 10));
            r.RegisterByKey("EM-57", new ComboMon(new DrawAttackMon("Collect", 1), new TailsNothingMon("Lunge")));
            r.RegisterByKey("EM-58", new PlainMon());
            r.RegisterByKey("EM-59", new CardsSS.BabyEvolutionMon("Pikachu", new DrawAttackMon("Collect", 1)));
            r.RegisterByKey("EM-60", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("EM-61", new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep));
            r.RegisterByKey("EM-62", new PlainMon());
            r.RegisterByKey("EM-63", new PlainMon());
            r.RegisterByKey("EM-64", new PlainMon());
            r.RegisterByKey("EM-65", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("EM-66", new TailsNothingMon("Lunge"));
            r.RegisterByKey("EM-67", new BenchShieldMon(true, new TailsNothingMon("Razor Wind")));
            r.RegisterByKey("EM-68", new TailsNothingMon("Razor Wind"));
            r.RegisterByKey("EM-69", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("EM-70", new TailsNothingMon("Tail Smash"));
            r.RegisterByKey("EM-71", new PlainMon());
            r.RegisterByKey("EM-72", new FlipBonusMon("Firebreathing", 10));
            r.RegisterByKey("EM-73", new PlainMon());
            r.RegisterByKey("EM-74", new ParalyzeFlipMon("Body Slam"));
        }

        /// <summary>Electro-guard (Poké-Body): with any Lightning Energy attached, Minun has no Weakness. Quick Attack.</summary>
        private sealed class MinunElectroGuard : Mon
        {
            private static readonly Mon Quick = new FlipBonusMon("Quick Attack", 20);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && mon == me && me.Energy.Any(e => CardsHIF1.Provides(src, me, e, EnergyType.Lightning));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Quick.HandleAttack(ctx, atk); }
        }

        /// <summary>Submerge / Feathery (Poké-Body): while this Pokémon is on your Bench, no attack damage to it (Submerge: anyone's attacks; Feathery: only the opponent's).</summary>
        private sealed class BenchShieldMon : Mon
        {
            private readonly bool opponentOnly;
            private readonly Mon attacks;

            public BenchShieldMon(bool opponentOnly, Mon attacks)
            {
                this.opponentOnly = opponentOnly;
                this.attacks = attacks;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || !src.Me.Bench.Contains(me) || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return opponentOnly && info.Attacker.Owner == src.Player ? damage : 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dig Under: N damage to 1 of the opponent's Pokémon, not affected by Weakness or Resistance.</summary>
        private sealed class DigUnderMon : Mon
        {
            private readonly int amount;

            public DigUnderMon(int amount) { this.amount = amount; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Under") return false;
                ctx.DamageAnyOpponent(amount, true);
                return true;
            }
        }
    }
}
