using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Burning Shadows (BUS, v88), part 1: Grass, Fire and Water (BUS 1-39). No regulation mark: Expanded. Wimpod, Golisopod-GX,
    /// Charizard-GX, Ho-Oh-GX and Tapu Fini-GX (Hidden Fates Shiny Vault) were already in the pool and keep their scripts. "A [T]
    /// Energy card" outside play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsBUS1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("BUS-1", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("BUS-2", new CardsTWM1.HealSelfMon("Shed Skin", 40));
            r.RegisterByKey("BUS-3", new ComboMon(new ByeByeHealMon(), new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("BUS-4", new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned));
            r.RegisterByKey("BUS-5", new ConditionAttackMon("Stinky Scent", SpecialCondition.Confused));
            r.RegisterByKey("BUS-6", new VileplumeBUS());
            r.RegisterByKey("BUS-7", new NoRetreatMon("Bind Down"));
            r.RegisterByKey("BUS-8", new ComboMon(new CardsDAA1.DrainMon("Giga Drain"), new CoinsMon("Crosswise Whip", 4)));
            r.RegisterByKey("BUS-9", new DodgeMon("Agility", false));
            r.RegisterByKey("BUS-10", new ComboMon(new CardsCRZ1.PiercingMon("Swift"), new CoinsMon("Comet Punch", 4)));
            r.RegisterByKey("BUS-11", new HeracrossBUS());
            r.RegisterByKey("BUS-13", new AttachTypeFromHandMon("Leaf Supply", EnergyType.Grass));
            r.RegisterByKey("BUS-14", new FlipBonusMon("Ambush", 10));
            r.RegisterByKey("BUS-15", new BubbleNetMon());
            // Fire
            r.RegisterByKey("BUS-23", new CardsSLG1.DiscardEnergyToBenchMon("Flare Recharge", EnergyType.Fire));
            r.RegisterByKey("BUS-24", new ComboMon(new CardsBLW.OdorSleuthMon(2), new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            r.RegisterByKey("BUS-25", new ComboMon(new CardsASR1.TimesMon("Diabolical Claws", c => c.MyPrizesTaken), new QueensHazeMon()));
            r.RegisterByKey("BUS-26", new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire));
            // Water
            r.RegisterByKey("BUS-27", new CardsPAR2.FlipConditionMon("Powder Snow", SpecialCondition.Asleep));
            CardsUPR1.Same(r, "BUS-28", "TEU-111"); // Luminous Barrier: Team Up's Fairy print has the same Ability
            r.RegisterByKey("BUS-29", new CardsMEG.SnipeMon("Water Arrow", 10));
            r.RegisterByKey("BUS-30", new CardsMEG.SnipeMon("Water Arrow", 30));
            r.RegisterByKey("BUS-31", new KingdraBUS());
            r.RegisterByKey("BUS-32", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("BUS-33", new ComboMon(new CardsASR1.TimesMon("Venting Anger", c => c.Me.Discard.Count(d => d.Def.IsPokemon && d.Def.Name == "Magikarp")), new OwnBenchQuakeMon("Splash Burn", 30)));
            r.RegisterByKey("BUS-34", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("BUS-35", new AzumarillBUS());
            r.RegisterByKey("BUS-37", new AquaReflectMon());
            r.RegisterByKey("BUS-38", new ComboMon(new ConditionAttackMon("Gnash Teeth", SpecialCondition.Confused), new SynchronoiseMon()));
        }

        // ================================================================== Shared

        /// <summary>"You may attach a [T] Energy card from your hand to 1 of your Pokémon" (Leaf Supply).</summary>
        internal sealed class AttachTypeFromHandMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public AttachTypeFromHandMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => ctx.Engine.CanAttachFromHand(mon), attack + ": you may attach a " + type + " Energy card from your hand to 1 of your Pokémon");
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Bye-Bye Heal: heal all damage from all of your Pokémon, then shuffle this Pokémon and all attached cards into your deck.</summary>
        private sealed class ByeByeHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bye-Bye Heal") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                bool wasActive = ctx.IsActive(me);
                ctx.Engine.ShufflePokemonIntoDeck(me);
                if (wasActive && ctx.Me.Active == null && ctx.Me.Bench.Count > 0) ctx.Engine.PromoteActive(ctx.Player);
                return true;
            }
        }

        /// <summary>Disgusting Pollen: while this is your Active Pokémon, the opponent's Basic Pokémon can't attack. Downer Shock: heads Asleep, tails Confused.</summary>
        private sealed class VileplumeBUS : Mon
        {
            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && mon != null && mon.Owner != src.Player && mon.IsBasic;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Downer Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, heads ? SpecialCondition.Asleep : SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Guts: if this Pokémon would be Knocked Out by damage from an attack, flip a coin; heads, it isn't and its remaining HP becomes
        /// 10. Pitch: the opponent switches their Active Pokémon.
        /// </summary>
        private sealed class HeracrossBUS : Mon
        {
            private static readonly Mon Pitch = new CardsASC.PushDownMon("Pitch");

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return false;
                Announce(src, "Guts");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pitch.HandleAttack(ctx, atk); }
        }

        /// <summary>Bubble Net: during the opponent's next turn, Energy can't be attached from their hand to the Defending Pokémon (it ends if that Pokémon goes to the Bench).</summary>
        private sealed class BubbleNetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Net") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsCRI.NoEnergyFromHandEffect("Energy can't be attached from the hand to " + defending.Name + " (Bubble Net)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Queen's Haze-GX: discard all Energy from the opponent's Active Pokémon.</summary>
        private sealed class QueensHazeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Queen's Haze-GX") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.DiscardAllEnergy(defending);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Brine: 90 damage to 1 of the opponent's Pokémon that has any damage counters (no printed damage). Tornado Shot: discard a
        /// Water Energy from this Pokémon; 30 to 1 of the opponent's Benched Pokémon too.
        /// </summary>
        private sealed class KingdraBUS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Brine")
                {
                    var hurt = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count == 0) return Nothing(ctx);
                    var target = ctx.ChoosePokemon(hurt, "Brine: do 90 damage to which of your opponent's damaged Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 90);
                    return true;
                }
                if (atk.Name != "Tornado Shot") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Tornado Shot: discard a Water Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water));
                ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>Splash Burn: 30 damage to each of your own Benched Pokémon too. Used for Splash Burn, Mini Earthquake, Earthquake (the attack name and the numbers are passed in).</summary>
        internal sealed class OwnBenchQuakeMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public OwnBenchQuakeMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Thick Fat: this Pokémon takes 30 less damage from attacks from the opponent's Fire and Water Pokémon (after Weakness and Resistance).</summary>
        private sealed class AzumarillBUS : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || info.Target != me || attacker == null || attacker.Owner == me.Owner) return damage;
                return src.Engine.IsType(attacker, EnergyType.Fire) || src.Engine.IsType(attacker, EnergyType.Water) ? damage - 30 : damage;
            }
        }

        /// <summary>Aqua Reflect: move a Water Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class AquaReflectMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Reflect") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var water = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                var pick = ctx.ChooseCards(water, 1, 1, "Aqua Reflect: move which Water Energy?")[0];
                var dest = ctx.ChoosePokemon(ctx.MyBench(), "Aqua Reflect: move " + pick.Def.Name + " to which Benched Pokémon?");
                if (dest != null) ctx.Engine.MoveEnergy(me, dest, pick);
                return true;
            }
        }

        /// <summary>Synchronoise: 20 damage to each of the opponent's Benched Pokémon that shares a type with their Active Pokémon (once each, however many types match).</summary>
        internal sealed class SynchronoiseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchronoise") return false;
                var defending = ctx.Defending;
                var types = defending == null ? new List<EnergyType>() : ctx.Engine.TypesOf(defending);
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.Engine.TypesOf(mon).Any(t => types.Contains(t))) ctx.DamageTarget(mon, 20);
                return true;
            }
        }
    }
}
