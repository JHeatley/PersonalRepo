using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Evolutions (EVO, v92), part 1: the Grass, Fire and Water Pokémon (1-34) and the secret rare Exeggutor (109). The XY-era
    /// Pokémon-EX, Mega Evolution and BREAK rules are in the engine (CardDef.IsOldMega / IsBreak); the full arts are prints.
    /// </summary>
    public static class CardsEVO1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EVO-1", new ComboMon(new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned), new CardsTWM1.HealSelfMon("Jungle Hammer", 30)));
            r.RegisterByKey("EVO-2", new CrisisVineMon());
            r.RegisterByKey("EVO-3", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("EVO-4", new ComboMon(new ShieldAttackMon("Stiffen", 40), new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("EVO-5", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("EVO-6", new ComboMon(new ShieldAttackMon("Stiffen", 40), new CardsPAR2.FlipConditionMon("Poison Powder", SpecialCondition.Poisoned)));
            r.RegisterByKey("EVO-7", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new SwarmingStingMon()));
            r.RegisterByKey("EVO-8", new ComboMon(new ParalyzeFlipMon("Bind"), new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned)));
            r.RegisterByKey("EVO-9", new CardsSUM1.DiscardTypeFromSelfMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("EVO-10", new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1));
            r.RegisterByKey("EVO-11", new CharizardEVO());
            r.RegisterByKey("EVO-12", new LockAfterAttackMon("Combustion Blast", true));
            r.RegisterByKey("EVO-13", new RecoilMon("Crimson Dive", 50));
            r.RegisterByKey("EVO-14", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("EVO-15", new ComboMon(new LureMon(), new CardsSUM1.DiscardTypeFromSelfMon("Fire Blast", EnergyType.Fire, 1)));
            r.RegisterByKey("EVO-16", new ExplosiveFireballMon());
            r.RegisterByKey("EVO-17", new SwitchSelfMon("Hind Kick", false));
            r.RegisterByKey("EVO-18", new ArcanineEVO());
            r.RegisterByKey("EVO-20", new CardsSUM1.DiscardTypeFromSelfMon("Flamethrower", EnergyType.Fire, 1));
            r.RegisterByKey("EVO-21", new ComboMon(new CardsLOT4.RapidSpinMon(), new CardsFST1.FlipRecoilMon("Splash Bomb", 30)));
            r.RegisterByKey("EVO-22", new CardsFST1.TwoBenchedMon("Hydro Bombard", 30));
            r.RegisterByKey("EVO-24", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("EVO-25", new ComboMon(new DashingPunchMon(), new CardsDRI2.CrunchMon("Whirlpool")));
            r.RegisterByKey("EVO-26", new SlackOffMon());
            r.RegisterByKey("EVO-27", new LollRollSpinMon());
            r.RegisterByKey("EVO-28", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("EVO-29", new ComboMon(new SuperDeepDiveMon(), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("EVO-30", new FlipBonusMon("Quick Blow", 10));
            r.RegisterByKey("EVO-31", new StarmieEVO());
            r.RegisterByKey("EVO-32", new BreakStarMon());
            r.RegisterByKey("EVO-33", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("EVO-34", new ComboMon(new ParalyzeFlipMon("Bubble Beam"), new CardsBWP.BothHeadsOrNothingMon("Dragon Rage")));
            r.RegisterByKey("EVO-109", new FlipBonusMon("Stomp", 10));
        }

        // ================================================================== Grass

        /// <summary>Crisis Vine: the opponent's Active Pokémon is now Paralyzed and Poisoned.</summary>
        private sealed class CrisisVineMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crisis Vine") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Swarming Sting: 40 damage to 1 of your opponent's Pokémon for each Beedrill you have in play (Weakness and Resistance only for the Active).</summary>
        private sealed class SwarmingStingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swarming Sting") return false;
                int n = ctx.MyPokemon(m => m.Name == "Beedrill").Count;
                if (n > 0) ctx.DamageAnyOpponent(40 * n);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Energy Burn: every Energy attached to this Pokémon provides [R] instead of its usual type (a Double Colorless Energy
        /// gives [R][R]). Fire Spin: discard 3 Energy from this Pokémon.
        /// </summary>
        private sealed class CharizardEVO : Mon
        {
            private static readonly Mon Spin = new CardsTWM1.SelfDiscardMon("Fire Spin", 3);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spin.HandleAttack(ctx, atk); }
        }

        /// <summary>Lure: switch 1 of the opponent's Benched Pokémon with their Active Pokémon; the new Active Pokémon can't retreat during their next turn.</summary>
        private sealed class LureMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lure") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var lured = ctx.GustOpponent("Lure: choose 1 of your opponent's Benched Pokémon to switch in");
                if (lured != null && ctx.CanAffect(lured)) ctx.DefendingCantRetreat(lured);
                return true;
            }
        }

        /// <summary>Explosive Fireball: discard all [R] Energy from this Pokémon, then 10 damage plus 60 more for each Energy card discarded.</summary>
        private sealed class ExplosiveFireballMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Explosive Fireball") return false;
                var me = ctx.Pokemon;
                int fire = me.Energy.Count(en => CardsHIF1.Provides(ctx, me, en, EnergyType.Fire));
                int discarded = fire == 0 ? 0 : ctx.DiscardEnergyFrom(me, fire, "Explosive Fireball: discard all Fire Energy from " + me.Name,
                    en => CardsHIF1.Provides(ctx, me, en, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage + 60 * discarded);
                return true;
            }
        }

        /// <summary>
        /// Burning Road: once during your turn (before your attack), when this Pokémon moves from the Bench to the Active Spot, you
        /// may move any number of [R] Energy from your other Pokémon to it. It's decided right then; if you don't use it, it can be
        /// used on a later move that turn (Compendium, 2016-11-03). Scorching Breath: it can't attack during your next turn.
        /// </summary>
        private sealed class ArcanineEVO : Mon
        {
            private static readonly Mon Breath = new LockAfterAttackMon("Scorching Breath", false);
            internal const string Road = "Burning Road";

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.ResolvingAttack != null || ctx.AbilityUsedThisTurn(Road)) return;
                var fire = new List<CardInstance>();
                var holders = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    foreach (var card in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, card, EnergyType.Fire)) { fire.Add(card); holders[card.Id] = mon; }
                if (fire.Count == 0) return;
                if (!ctx.AskYesNo("Burning Road: move Fire Energy from your other Pokémon to " + me.Name + "?")) return;
                ctx.MarkAbilityUsed(Road);
                Announce(ctx, Road);
                var picks = ctx.ChooseCards(fire, 1, fire.Count, "Burning Road: choose the Fire Energy to move to " + me.Name);
                foreach (var card in picks)
                {
                    var giver = holders[card.Id];
                    if (ctx.InPlay(giver) && ctx.InPlay(me) && giver.Energy.Contains(card)) ctx.Engine.MoveEnergy(giver, me, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Breath.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Dashing Punch: 50 more damage if this Pokémon moved from the Bench to the Active Spot this turn.</summary>
        private sealed class DashingPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dashing Punch") return false;
                ctx.DamageDefending(atk.Damage + (CardsUNM1.CameInThisTurn(ctx) ? 50 : 0));
                return true;
            }
        }

        /// <summary>Slack Off: heal 60 damage from this Pokémon; it can't attack during your next turn. Flash Splash: printed damage.</summary>
        private sealed class SlackOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slack Off") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.Heal(ctx.Pokemon, 60);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Loll Roll Spin: this Pokémon is now Confused, and during your next turn its Loll Roll Spin does 100 more damage (before
        /// Weakness and Resistance). The bonus is on the Pokémon, so it ends if it leaves the Active Spot or evolves.
        /// </summary>
        private sealed class LollRollSpinMon : Mon
        {
            private const string Spin = "Loll Roll Spin";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Spin) return false;
                var me = ctx.Pokemon;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(fx => fx.TargetUid == uid && fx.AttackName == Spin).Sum(fx => fx.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (!ctx.InPlay(me)) return true;
                ctx.ApplyCondition(me, SpecialCondition.Confused);
                var boost = new AttackBoostEffect(me.Name + "'s Loll Roll Spin does 100 more damage during its next turn", ctx.Player, uid, ctx.Turn + 2, Spin, 100);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Super Deep Dive: heal 40 damage from this Pokémon, then switch it with 1 of your Benched Pokémon.</summary>
        private sealed class SuperDeepDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Deep Dive") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.Heal(ctx.Pokemon, 40);
                ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>
        /// Space Beacon: once during your turn (before your attack), you may discard a card from your hand; if you do, put 2 basic
        /// Energy cards from your discard pile into your hand (not the card you just discarded). Star Freeze: flip, Paralyzed.
        /// </summary>
        private sealed class StarmieEVO : Mon
        {
            private static readonly Mon Freeze = new ParalyzeFlipMon("Star Freeze");

            protected override string Activated { get { return "Space Beacon"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsBasicEnergy);
            }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                var toss = ctx.ChooseCards(hand, 1, 1, "Space Beacon: discard a card from your hand");
                if (toss.Count == 0) return;
                var tossed = toss[0];
                ctx.Engine.DiscardFromHand(ctx.Player, tossed);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy && c != tossed).ToList();
                if (energy.Count == 0) return;
                int n = Math.Min(2, energy.Count);
                var picks = n == energy.Count ? energy : ctx.ChooseCards(energy, n, n, "Space Beacon: put 2 basic Energy cards from your discard pile into your hand");
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Freeze.HandleAttack(ctx, atk); }
        }

        /// <summary>Break Star: 100 damage to each of the opponent's Pokémon BREAK (Weakness and Resistance only for the Active).</summary>
        private sealed class BreakStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Break Star") return false;
                foreach (var target in ctx.OpponentPokemon(m => m.Def.IsBreak))
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, 100);
                return true;
            }
        }

    }
}
