using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// POP Series 3 (P3, 2006; v171): the cards not already in the pool (High Pressure System 10, Low Pressure System 11 and
    /// Pikachu are prints). Unlimited only.
    /// </summary>
    public static partial class CardsP3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("P3-1", new ComboMon(new SwitchSelfMon("Smash Turn", true), new RocketTackleMon()));
            r.RegisterByKey("P3-2", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CardsRS.DiscardBasicOrNothingMon("Fire Spin", 2, false)));
            r.RegisterByKey("P3-3", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("P3-4", new ComboMon(new CheerUpMon("Plusle"), new CardsMEG.WeakenDefendingMon("Negative Ion", 10), new PowerOrBodyBoltMon("Power Bolt", AbilityDef.PokePower)));
            r.RegisterByKey("P3-5", new ComboMon(new CheerUpMon("Minun"), new FlipBonusMon("Positive Ion", 10), new PowerOrBodyBoltMon("Body Bolt", AbilityDef.PokeBody)));
            r.RegisterByKey("P3-6", new ExtraWater20Mon("Water Gun"));
            r.RegisterByKey("P3-7", new CardsASC.SelfDiscardTypeMon("Flamethrower", EnergyType.Fire, 1));
            r.RegisterByKey("P3-8", new ComboMon(new CardsCG.DiscardCardToHandMon("Sniff Out"), new CoinsMon("Fury Attack", 3)));
            r.RegisterByKey("P3-9", new CardsPK.PoisonCountersMon("Toxic", 2));
            r.RegisterByKey("P3-12", new CardsDX.FormChangeMon("Ditto", new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused), "Duplicate"));
            r.RegisterByKey("P3-13", new RecoilMon("Take Down", 10));
            r.RegisterByKey("P3-14", new ComboMon(new FlipBonusMon("Gouge", 10), new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("P3-15", new RecoilMon("Double-edge", 10));
            r.RegisterByKey("P3-16", new BustleMon());
        }

        /// <summary>Rocket Tackle: 10 damage to itself; flip a coin, heads, no damage to it from attacks during the opponent's next turn.</summary>
        private sealed class RocketTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rocket Tackle") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(10);
                if (ctx.Flip() && ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks during the opponent's next turn (Rocket Tackle)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Cheer Up: draw a card, or 2 if you have [partner] in play.</summary>
        private sealed class CheerUpMon : Mon
        {
            private readonly string partner;

            public CheerUpMon(string partner) { this.partner = partner; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cheer Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(ctx.Me.InPlay().Any(m => m.Name == partner) ? 2 : 1);
                return true;
            }
        }

        /// <summary>Power Bolt / Body Bolt: 30 to 1 of the opponent's Pokémon that has a Poké-Power (Poké-Body); no W/R on the Bench.</summary>
        private sealed class PowerOrBodyBoltMon : Mon
        {
            private readonly string attack;
            private readonly string kind;

            public PowerOrBodyBoltMon(string attack, string kind)
            {
                this.attack = attack;
                this.kind = kind;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var targets = ctx.OpponentPokemon(m => m.Def.Abilities.Any(a => a.Kind == kind));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, attack + ": 30 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, 30);
                return true;
            }
        }

        /// <summary>"20 more damage for each Water Energy not used to pay for this attack (no more than 40 more)" (Water Gun, Hydro Pump).</summary>
        internal sealed class ExtraWater20Mon : Mon
        {
            private readonly string attack;

            public ExtraWater20Mon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + 20 * CardsBS1.ExtraWaterMon.Extra(ctx, atk));
                return true;
            }
        }

        /// <summary>Bustle: 2 coins, 20 for each heads; with any heads, the Defending Pokémon is now Confused.</summary>
        private sealed class BustleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bustle") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
