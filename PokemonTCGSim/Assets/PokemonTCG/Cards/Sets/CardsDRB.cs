using System;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Dragon Uncommon and Common Pokémon (DR 23-81, 98-100's starters are in CardsDR; v159). Unlimited only.</summary>
    public static class CardsDRB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DR-23", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("DR-24", new CardsPAR2.FlipConditionMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("DR-25", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("DR-26", new DragonDewMon());
            r.RegisterByKey("DR-27", new EnergyRecallUpToMon());
            r.RegisterByKey("DR-28", new ComboMon(new CountersOnDefendingMon("Double Metal Ball", 2), new BackspinMon()));
            r.RegisterByKey("DR-29", new ComboMon(new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon)), new BigExplosionMon()));
            r.RegisterByKey("DR-30", new CardsRSB.MagnitudeMon());
            r.RegisterByKey("DR-31", new FuryCutterFourMon());
            r.RegisterByKey("DR-32", new ComboMon(new DragonCrushMon(), new CardsFLF1.NoResistanceMon("Aqua Sonic")));
            r.RegisterByKey("DR-33", new ComboMon(new CardsRS.RetaliateFlipMon(), new CardsUPR3.SmokescreenMon("Smokescreen", 1)));
            r.RegisterByKey("DR-34", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 20), new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1)));
            r.RegisterByKey("DR-35", new CardsRSB.CoinsPlusMon("Triple Smash", 3, 20));
            r.RegisterByKey("DR-36", new CardsDR.AlsoOneBenchedMon("Mud Splash", 10));
            r.RegisterByKey("DR-37", new CardsASR1.PlusMon("Meditate", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters));
            r.RegisterByKey("DR-38", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new CardsFLF1.NoResistanceMon("Sonic Wing")));
            r.RegisterByKey("DR-39", new ComboMon(new CardsRS.ExtraEnergyMon("Energy Cannon", 2), new CardsSSB.AnyOpponentTextMon("Water Arrow", 30)));
            r.RegisterByKey("DR-40", new DodgeMon("Agility", false));
            r.RegisterByKey("DR-41", new EnergyGuardMon());
            r.RegisterByKey("DR-42", new GraniteHeadMon());
            r.RegisterByKey("DR-43", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new NightAttackMon()));
            r.RegisterByKey("DR-44", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("DR-45", new ComboMon(new NoRetreatMon("Clutch"), new QuickDiveMon()));
            r.RegisterByKey("DR-46", new LevitateMon());
            r.RegisterByKey("DR-47", new CoinsMon("Quick Turn", 2));
            r.RegisterByKey("DR-48", new SubmergeBenchMon(new CardsRSB.MagnitudeMon()));
            r.RegisterByKey("DR-49", new PlainMon());
            r.RegisterByKey("DR-50", new ComboMon(new TailsNothingMon("Risky Kick"), new ConditionAttackMon("Dragon Eye", SpecialCondition.Asleep)));
            r.RegisterByKey("DR-51", new PlainMon());
            r.RegisterByKey("DR-52", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned), new ParalyzeFlipMon("Bubble")));
            r.RegisterByKey("DR-53", new PlainMon());
            r.RegisterByKey("DR-54", new ConditionAttackMon("Toxic Grip", SpecialCondition.Poisoned));
            r.RegisterByKey("DR-55", new PebbleThrowMon());
            r.RegisterByKey("DR-56", new FlipBonusMon("Steady Punch", 10));
            r.RegisterByKey("DR-57", new PoisonSpurtMon());
            r.RegisterByKey("DR-58", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("DR-59", new CardsE3B.OpponentSwitchesMon("Roar"));
            r.RegisterByKey("DR-60", new CallMagikarpMon());
            r.RegisterByKey("DR-61", new PlainMon());
            r.RegisterByKey("DR-62", new CardsSSB.AnyOpponentTextMon("Random Spark", 10));
            r.RegisterByKey("DR-63", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("DR-64", new ParalyzeFlipMon("Jolt"));
            r.RegisterByKey("DR-65", new CardsBS1.ExtraWaterMon("Water Gun"));
            r.RegisterByKey("DR-66", new CardsPAR2.FlipConditionMon("Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("DR-67", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("DR-68", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("DR-69", new FlipBonusMon("Firebreathing", 10));
            r.RegisterByKey("DR-70", new KindleMon());
            r.RegisterByKey("DR-71", new CardsFST1.TakeLessMon(10, null));
            r.RegisterByKey("DR-72", new LuringFlameMon());
            r.RegisterByKey("DR-73", new SwitchSelfMon("Bounce", true));
            r.RegisterByKey("DR-74", new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("DR-75", new ComboMon(new ConditionAttackMon("Lullaby", SpecialCondition.Asleep), new ParalyzeFlipMon("Stifling Fluff")));
            r.RegisterByKey("DR-76", new CardsE3B.OpponentSwitchesMon("Whirlwind"));
            r.RegisterByKey("DR-77", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("DR-78", new PlainMon());
            r.RegisterByKey("DR-79", new ComboMon(new ParalyzeFlipMon("Bind"), new FlipBonusMon("Rock Smash", 20)));
            r.RegisterByKey("DR-80", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("DR-81", new ComboMon(new CallForFriendsMon(), new ParalyzeFlipMon("String Shot")));
        }

        /// <summary>"Put N damage counters on each Defending Pokémon" (Double Metal Ball).</summary>
        private sealed class CountersOnDefendingMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersOnDefendingMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, counters);
                return true;
            }
        }

        /// <summary>Dragon Dew: remove 2 damage counters from 1 of your Pokémon (1 if it has only 1).</summary>
        private sealed class DragonDewMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Dew") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Dragon Dew: remove 2 damage counters from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        /// <summary>Energy Recall: up to 2 basic Energy cards from your discard pile onto Flaaffy.</summary>
        private sealed class EnergyRecallUpToMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Recall") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0 || !ctx.InPlay(me)) return true;
                int n = Math.Min(2, basics.Count);
                foreach (var card in ctx.ChooseCards(basics, 1, n, "Energy Recall: attach up to " + n + " basic Energy cards to " + me.Name))
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Backspin: after the damage, you may discard an Energy card from Forretress; if you do, switch it with 1 of your Benched Pokémon.</summary>
        private sealed class BackspinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Backspin") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Backspin: discard an Energy card from " + me.Name + " to switch it with 1 of your Benched Pokémon?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Backspin: discard which Energy card?") > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Big Explosion: 80 damage to each Active Pokémon, the Defending Pokémon and Graveler itself.</summary>
        private sealed class BigExplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Explosion") return false;
                ctx.DamageDefending(80);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(80);
                return true;
            }
        }

        /// <summary>Fury Cutter: 4 coins; all heads, 60 more; otherwise 10 more for each heads.</summary>
        private sealed class FuryCutterFourMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage + (heads == 4 ? 60 : 10 * heads));
                return true;
            }
        }

        /// <summary>Dragon Crush: heads, 10 damage; either way an Energy card is discarded from the Defending Pokémon (your ruling).</summary>
        private sealed class DragonCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Crush") return false;
                var defending = ctx.Defending;
                if (ctx.Flip()) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Dragon Crush: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Energy Guard (Poké-Body): with any basic Energy cards attached, the opponent's attacks do 10 less damage to Shelgon (after Weakness and Resistance).</summary>
        private sealed class EnergyGuardMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return me.Energy.Any(e => e.Def.IsBasicEnergy) ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Granite Head: during the opponent's next turn, their attacks do 10 less damage to Shelgon (after Weakness and Resistance).</summary>
        private sealed class GraniteHeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Granite Head") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(10);
                return true;
            }
        }

        /// <summary>Night Attack: 1 damage counter on 1 of the opponent's Pokémon.</summary>
        internal sealed class NightAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Attack") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Night Attack: put a damage counter on which Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
                return true;
            }
        }

        /// <summary>Quick Dive: flip a coin; heads, 50 damage to 1 of the opponent's Pokémon (no Weakness or Resistance).</summary>
        private sealed class QuickDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Dive") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 50), true, false);
                return true;
            }
        }

        /// <summary>Levitate (Poké-Body): with any basic Energy cards attached, Vibrava's Retreat Cost is 0. Razor Wing.</summary>
        private sealed class LevitateMon : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Any(e => e.Def.IsBasicEnergy) ? 0 : cost;
            }
        }

        /// <summary>Submerge (Poké-Body): while Whiscash is on your Bench, prevent all damage done to it by the opponent's attacks.</summary>
        private sealed class SubmergeBenchMon : Mon
        {
            private readonly Mon attacks;

            public SubmergeBenchMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Pebble Throw: 10 damage to 1 of the opponent's Benched Pokémon (no Weakness or Resistance).</summary>
        private sealed class PebbleThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pebble Throw") return false;
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(Math.Max(atk.Damage, 10));
                return true;
            }
        }

        /// <summary>Poison Spurt: discard a Grass Energy card attached to Grimer; the Defending Pokémon is now Poisoned.</summary>
        internal sealed class PoisonSpurtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Spurt") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!CardsBS1.CopyingWithMetronome && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Poison Spurt: discard a Grass Energy card", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Grass));
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Call for Family: any number of Magikarp from your deck onto your Bench.</summary>
        private sealed class CallMagikarpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.Name == "Magikarp", ctx.BenchSpace, "Call for Family: put any number of Magikarp onto your Bench");
                return true;
            }
        }

        /// <summary>Kindle: discard a Fire Energy card attached to Numel, then an Energy card attached to the Defending Pokémon.</summary>
        private sealed class KindleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kindle") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "Kindle: discard a Fire Energy card", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Kindle: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Luring Flame: switch 1 of the opponent's Benched Pokémon in; the new Defending Pokémon is Burned. With no Benched Pokémon, nothing (your ruling).</summary>
        private sealed class LuringFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Luring Flame") return false;
                if (ctx.GustTargets().Count == 0) return Nothing(ctx);
                var old = ctx.Defending;
                ctx.GustOpponent("Luring Flame: choose the new Defending Pokémon");
                var now = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (now != null && now != old && TefPokemon.StillDefending(ctx, now)) ctx.ApplyCondition(now, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Call for Friends: any number of Grass Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class CallForFriendsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Friends") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => d.PokemonType == EnergyType.Grass, ctx.BenchSpace, "Call for Friends: put any number of Grass Basic Pokémon onto your Bench");
                return true;
            }
        }
    }
}
