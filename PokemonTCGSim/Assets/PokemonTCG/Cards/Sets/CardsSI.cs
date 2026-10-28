using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Southern Islands (SI 1-18, 2001; v140): the Southern Islands promo cards. Unlimited only. Your ruling: Paradise Pollen heals
    /// nothing on tails (its Benched heal comes only with heads, like Shockwave and Hatch). Tentacruel's Poison Sting paralyzes, as
    /// printed. Pokémon whose attacks only do their printed damage: Slicing Wind, Razor Leaf, Gnaw, Rolling Tackle, Low Kick.
    /// </summary>
    public static class CardsSI
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SI-1", new RainbowWaveMon());
            r.RegisterByKey("SI-2", new TailsNothingMon("Supersonic Flight"));
            r.RegisterByKey("SI-3", new ComboMon(new CoinsMon("Slam", 2), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("SI-4", new CardsPAR2.FlipConditionMon("Bawl", SpecialCondition.Confused));
            r.RegisterByKey("SI-5", new StrangeScentMon());
            r.RegisterByKey("SI-6", new FlipBonusMon("Sharp Teeth", 30));
            r.RegisterByKey("SI-7", new DodgeMon("Agility", false));
            r.RegisterByKey("SI-8", new ComboMon(new CardsBS1.LureMon("Tempting Eyes"), new CardsPAR2.FlipConditionMon("Sing", SpecialCondition.Asleep)));
            r.RegisterByKey("SI-9", new ComboMon(new DodgeMon("Fly", true), new HeadsTailsConditionMon("Rainbow Powder", SpecialCondition.Paralyzed, SpecialCondition.Poisoned)));
            r.RegisterByKey("SI-10", new ComboMon(new TentacleGripMon(), new ParalyzeFlipMon("Poison Sting")));
            r.RegisterByKey("SI-11", new SnipeMon("Squirt", 10, false));
            r.RegisterByKey("SI-12", new ComboMon(new GentleSongMon(), new ParalyzeFlipMon("Ice Beam")));
            r.RegisterByKey("SI-13", new SharpshooterMon());
            r.RegisterByKey("SI-14", new ComboMon(new RevelationMon(), new CardsN2.PsychicPlusMon()));
            r.RegisterByKey("SI-15", new CardsBS1.ExtraWaterMon("Water Gun"));
            r.RegisterByKey("SI-16", new ComboMon(new LickWoundsMon(), new SnipeMon("Tongue Stretch", 20, true)));
            r.RegisterByKey("SI-17", new ComboMon(new ParadisePollenMon(), new HeadsTailsConditionMon("Horrid Pollen", SpecialCondition.Paralyzed, SpecialCondition.Confused)));
            r.RegisterByKey("SI-18", new CardsJU.RampageMon());
        }

        /// <summary>Remove 2 damage counters (1 if it only has 1).</summary>
        private static void HealTwo(EffectContext ctx, PokemonInPlay mon)
        {
            if (mon != null && ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, 20);
        }

        /// <summary>
        /// Rainbow Wave: choose a type of Energy other than Colorless attached to this Pokémon; 20 damage to each of the opponent's
        /// Pokémon of that type, Benched ones too, no Weakness or Resistance.
        /// </summary>
        private sealed class RainbowWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Wave") return false;
                var me = ctx.Pokemon;
                if (me == null) return true;
                var types = new List<EnergyType>();
                foreach (EnergyType t in Enum.GetValues(typeof(EnergyType)))
                    if (t != EnergyType.Colorless && me.Energy.Any(e => CardsDRI1.ProvidesType(ctx, me, e, t))) types.Add(t);
                if (types.Count == 0) return Nothing(ctx);
                var type = types[0];
                if (types.Count > 1)
                {
                    int pick = ctx.ChooseOption("Rainbow Wave: choose a type of Energy attached to " + me.Name, types.Select(x => x.ToString()).ToArray());
                    if (pick >= 0 && pick < types.Count) type = types[pick];
                }
                ctx.Log("Rainbow Wave: " + type + ".");
                foreach (var mon in ctx.OpponentPokemon(m => ctx.Engine.IsType(m, type)))
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>
        /// Strange Scent: each player flips a coin; each player with heads chooses 3 damage counters in all among their own Pokémon
        /// and removes them (all of them if they have fewer).
        /// </summary>
        private sealed class StrangeScentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strange Scent") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var heads = new[] { ctx.Player, ctx.Opponent }.Where(p => ctx.Engine.FlipCoin(p, "Strange Scent")).ToList();
                foreach (int player in heads)
                    for (int i = 0; i < 3; i++)
                    {
                        var hurt = ctx.State.Players[player].InPlay().Where(m => m.DamageCounters > 0).ToList();
                        if (hurt.Count == 0) break;
                        var target = hurt.Count == 1 ? hurt[0] : ctx.Engine.ChoosePokemon(player, hurt, "Strange Scent: remove a damage counter (" + (i + 1) + " of 3) from which of your Pokémon?", ctx.Source);
                        if (target == null || ctx.Heal(target, 10) <= 0) break;
                    }
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, the Defending Pokémon is now [A]. If tails, [B]." (Rainbow Powder, Horrid Pollen.)</summary>
        internal sealed class HeadsTailsConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition onHeads;
            private readonly SpecialCondition onTails;

            public HeadsTailsConditionMon(string attack, SpecialCondition onHeads, SpecialCondition onTails)
            {
                this.attack = attack;
                this.onHeads = onHeads;
                this.onTails = onTails;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? onHeads : onTails);
                return true;
            }
        }

        /// <summary>Tentacle Grip: a coin for each Water Energy attached to this Pokémon; draw 2 cards for each heads.</summary>
        private sealed class TentacleGripMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacle Grip") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads > 0) ctx.Draw(2 * heads);
                return true;
            }
        }

        /// <summary>
        /// Squirt / Tongue Stretch: N damage to 1 of the opponent's Pokémon you choose, no Weakness or Resistance (Tongue Stretch:
        /// only on heads).
        /// </summary>
        internal sealed class SnipeMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly bool flip;

            public SnipeMon(string attack, int amount, bool flip)
            {
                this.attack = attack;
                this.amount = amount;
                this.flip = flip;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (flip && !ctx.Flip()) return true;
                ctx.DamageAnyOpponent(amount, true);
                return true;
            }
        }

        /// <summary>Gentle Song: remove 2 damage counters from this Pokémon and 2 from the Defending Pokémon (1 if only 1); the Defending Pokémon is now Asleep.</summary>
        private sealed class GentleSongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gentle Song") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                HealTwo(ctx, ctx.Pokemon);
                if (defending == null || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (ctx.CanAffect(defending)) HealTwo(ctx, defending);
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Sharpshooter: choose 1 of the opponent's Pokémon; a coin for each Grass Energy attached to this Pokémon, 10 per heads to it (no W/R).</summary>
        private sealed class SharpshooterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sharpshooter") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = all.Count == 1 ? all[0] : ctx.ChoosePokemon(all, "Sharpshooter: choose 1 of your opponent's Pokémon");
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                if (heads > 0 && target != null && ctx.InPlay(target)) ctx.DamageWith(target, 10 * heads, false, false, false);
                return true;
            }
        }

        /// <summary>Revelation: heads, put a Trainer card from your deck into your hand (shown to the opponent).</summary>
        private sealed class RevelationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Revelation") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => d.IsTrainer, 1, "Revelation: choose a Trainer card to put into your hand");
                return true;
            }
        }

        /// <summary>Lick Wounds: heads, remove 2 damage counters (1 if only 1) from 1 Pokémon in play with damage counters, yours or your opponent's.</summary>
        private sealed class LickWoundsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lick Wounds") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                hurt.AddRange(ctx.OpponentPokemon(m => m.DamageCounters > 0 && ctx.CanAffect(m)));
                if (hurt.Count == 0) return true;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Lick Wounds: remove 2 damage counters from which Pokémon?");
                HealTwo(ctx, target);
                return true;
            }
        }

        /// <summary>
        /// Paradise Pollen: heads, remove 2 damage counters from this Pokémon, then from 1 of your Benched Pokémon with damage counters
        /// (1 if only 1). Tails, nothing (your ruling).
        /// </summary>
        private sealed class ParadisePollenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paradise Pollen") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                HealTwo(ctx, ctx.Pokemon);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Paradise Pollen: remove 2 damage counters from which of your Benched Pokémon?");
                HealTwo(ctx, target);
                return true;
            }
        }
    }
}
