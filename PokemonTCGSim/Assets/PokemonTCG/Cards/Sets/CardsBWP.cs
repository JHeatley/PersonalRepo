using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BW Black Star Promos (BWP BW01-BW101, 2011-2013; Expanded). Most promos are prints of set cards (Legendary Treasures,
    /// Celebrations' and 30th Celebration's Classic Collections, Next Destinies, Dark Explorers, Dragons Exalted, Boundaries Crossed,
    /// Plasma Storm, Plasma Freeze, Plasma Blast, Black &amp; White, Emerging Powers, the SWSH promo Champions Festival and N);
    /// Victory Cup 30-31, Tropical Beach 50, Axew 16, Pansage 14 and Genesect 101 are prints of the promos they repeat.
    /// Random Spark (Luxio, Ampharos) follows your Telekinesis ruling: one hit, to the Pokémon you choose.
    /// Pokémon whose attacks only do their printed damage need no script: Minccino 13.
    /// </summary>
    public static class CardsBWP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BWP-1", new CoinsMon("Slam", 2));
            r.RegisterByKey("BWP-2", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("BWP-3", new CardsPAR2.FlipConditionMon("Water Pulse", SpecialCondition.Asleep));
            r.RegisterByKey("BWP-6", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("BWP-7", new ComboMon(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned), new FlipBonusMon("Firebreathing", 20)));
            r.RegisterByKey("BWP-8", new ComboMon(new CardsPAR2.FlipConditionMon("Sleep Pulse", SpecialCondition.Asleep), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("BWP-10", new BothHeadsOrNothingMon("Dragon Rage"));
            r.RegisterByKey("BWP-11", new CoinsMon("Bullet Seed", 4));
            r.RegisterByKey("BWP-12", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("BWP-18", new CardsPAR2.FlipConditionMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("BWP-19", new ComboMon(
                new CardsFST2.ConditionBonusMon("Punishment", 20, c => c.Defending != null && c.Defending.Def.Stage == Stage.Stage2),
                new CardsMEG.WeakenDefendingMon("Snarl", 20)));
            r.RegisterByKey("BWP-25", new ParalyzeFlipMon("Paralyzing Gaze"));
            r.RegisterByKey("BWP-26", new NoRetreatMon("Big Bite"));
            r.RegisterByKey("BWP-27", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("BWP-28", new TropicalBeach());
            r.RegisterByKey("BWP-29", new VictoryCup());
            r.RegisterByKey("BWP-33", new SwitchSelfMon("Run Around", false));
            r.RegisterByKey("BWP-34", new ComboMon(new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused), new RandomSparkMon(40)));
            r.RegisterByKey("BWP-35", new ComboMon(new CardsTWM1.HealSelfMon("Nap", 20), new FlipBonusMon("Jump On", 10)));
            r.RegisterByKey("BWP-39", new BattleCity());
            r.RegisterByKey("BWP-49", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 1), new CardsPHF1.DreamDanceMon()));
            r.RegisterByKey("BWP-52", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("BWP-54", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("BWP-55", new ElgyemWeakBarrier());
            r.RegisterByKey("BWP-56", new ComboMon(new CoinsMon("Fury Attack", 3), new ColdCrushMon()));
            r.RegisterByKey("BWP-57", new ComboMon(new CardsASC.SelfGuardMon("Armor Press", 20), new CoinsMon("Dual Chop", 2)));
            r.RegisterByKey("BWP-60", new SwordsDanceMon());
            r.RegisterByKey("BWP-65", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("BWP-67", new ComboMon(new RandomSparkMon(30), new ElectricannonMon()));
            r.RegisterByKey("BWP-68", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("BWP-69", new CardsWHT.FlipMoreMon("Acrobatics", 2, 20));
            r.RegisterByKey("BWP-70", new JustifiedMon(new FlipBonusMon("Leaf Blade", 30)));
            r.RegisterByKey("BWP-71", new JustifiedMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow")));
            r.RegisterByKey("BWP-72", new JustifiedMon(new CardsMEG.FlipUntilTailsBonusMon("Iron Head", 20)));
            r.RegisterByKey("BWP-73", new ComboMon(new SwitchSelfMon("Hide in Shadows", true), new CardsPAR2.FlipConditionMon("Dark Hole", SpecialCondition.Asleep)));
            r.RegisterByKey("BWP-76", new ComboMon(new ParalyzeFlipMon("Electribeam"), new RecoilMon("Self Destruct", 100)));
            r.RegisterByKey("BWP-77", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("BWP-78", new ComboMon(new FlipBonusMon("Quick Attack", 20), new CardsTWM1.SelfDiscardMon("Thunder Blast", 1)));
            r.RegisterByKey("BWP-79", new ComboMon(
                new CardsFST2.ConditionBonusMon("Extrasensory", 30, c => c.Me.Hand.Count == c.Opp.Hand.Count),
                new NoRetreatMon("Rock Tomb")));
            r.RegisterByKey("BWP-80", new ComboMon(new CardsDAA1.GustAttackMon("Taunt", false), new CardsEPO.PowerUpMon("Boost Claw", 30)));
            r.RegisterByKey("BWP-85", new LucarioIntuition());
            r.RegisterByKey("BWP-86", new ComboMon(new CardsPAR2.FlipConditionMon("Signal Beam", SpecialCondition.Confused), new CardsASC.NextTurnBoostMon("Overdrive Smash", 60)));
            r.RegisterByKey("BWP-98", new MewPsyscan());
            r.RegisterByKey("BWP-99", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam"), new CardsSUM1.DiscardTypeFromSelfMon("Breaker Bazooka", EnergyType.Grass, 99)));
        }

        /// <summary>Dragon Rage: flip 2 coins; if either of them is tails, this attack does nothing. Used for Dragon Rage, Shot in the Dark, Drill Tackle, Epic Splash (the attack name is passed in).</summary>
        internal sealed class BothHeadsOrNothingMon : Mon
        {
            private readonly string attack;

            public BothHeadsOrNothingMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.FlipCoins(2) < 2) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Random Spark: N damage to 1 of the opponent's Pokémon (one hit, to the Pokémon you choose).</summary>
        private sealed class RandomSparkMon : Mon
        {
            private readonly int amount;

            public RandomSparkMon(int amount) { this.amount = amount; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Random Spark") return false;
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>Weak Barrier: with any Psychic Energy attached, this Pokémon has no Weakness. Quick Blow: heads, 20 more.</summary>
        private sealed class ElgyemWeakBarrier : Mon
        {
            private static readonly Mon QuickBlow = new FlipBonusMon("Quick Blow", 20);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && mon == src.Pokemon && src.Engine.HasEnergyOfType(mon, EnergyType.Psychic);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return QuickBlow.HandleAttack(ctx, atk); }
        }

        /// <summary>Cold Crush: you may discard an Energy from this Pokémon; if you do, discard an Energy from the Defending Pokémon.</summary>
        private sealed class ColdCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Crush") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Cold Crush: discard an Energy from " + me.Name + " to discard an Energy from " + defending.Name + "?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Cold Crush: discard an Energy from " + me.Name) > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Cold Crush: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Swords Dance: during your next turn, this Pokémon's Aqua Blade base damage is 120 (60 more). Aqua Blade.</summary>
        private sealed class SwordsDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Swords Dance")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s Aqua Blade has a base damage of 120 during its next turn (Swords Dance)", ctx.Player, me.Uid, ctx.Turn + 2, "Aqua Blade", 60);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name != "Aqua Blade") return false;
                int uid = me.Uid;
                // Base damage 120 however many times Swords Dance was used.
                bool boosted = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Any(e => e.TargetUid == uid && e.AttackName == "Aqua Blade");
                ctx.DamageDefending(boosted ? 120 : atk.Damage);
                return true;
            }
        }

        /// <summary>Electricannon: you may discard all Lightning Energy from this Pokémon; if you do, 60 more damage.</summary>
        private sealed class ElectricannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electricannon") return false;
                var me = ctx.Pokemon;
                bool discard = ctx.Engine.HasEnergyOfType(me, EnergyType.Lightning)
                    && ctx.AskYesNo("Electricannon: discard all Lightning Energy from " + me.Name + " for 60 more damage?");
                int discarded = discard ? CardsTEU2.DiscardAllOfType(ctx, me, EnergyType.Lightning) : 0;
                ctx.DamageDefending(atk.Damage + (discarded > 0 ? 60 : 0));
                return true;
            }
        }

        /// <summary>Justified: this Pokémon's attacks do 50 more damage to the opponent's Darkness Pokémon (before Weakness and Resistance).</summary>
        private sealed class JustifiedMon : Mon
        {
            private readonly Mon attacks;

            public JustifiedMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == me.Owner || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Darkness) ? damage + 50 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Intuition: when you play this Pokémon from your hand to evolve, you may draw 2 cards. Fast Punch: heads, 30 more.</summary>
        private sealed class LucarioIntuition : Mon
        {
            private static readonly Mon FastPunch = new FlipBonusMon("Fast Punch", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Intuition", ctx.Me.Deck.Count > 0, "Intuition: draw 2 cards?")) return;
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FastPunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Psyscan: once during your turn, if this Pokémon is Active, the opponent reveals their hand. Psychic Exchange: shuffle your hand in, draw 6.</summary>
        private sealed class MewPsyscan : Mon
        {
            protected override string Activated { get { return "Psyscan"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsActive(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx) { TefPokemon.RevealOpponentHand(ctx); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Exchange") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(6);
                return true;
            }
        }

        /// <summary>Tropical Beach: once during each player's turn, they may draw until they have 7 cards; if they do, their turn ends.</summary>
        private sealed class TropicalBeach : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Hand.Count < 7 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                if (ctx.DrawUntil(7) <= 0) return false;
                ctx.Log("Tropical Beach: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
                return false;
            }
        }

        /// <summary>Battle City: once during each player's turn, they may flip a coin; heads, draw a card.</summary>
        private sealed class BattleCity : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.Draw(1);
                return false;
            }
        }

        /// <summary>Victory Cup: flip a coin; heads, search your deck for a Pokémon and put it into your hand.</summary>
        private sealed class VictoryCup : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.SearchToHand(d => d.IsPokemon, 1, "Victory Cup: choose a Pokémon");
            }
        }
    }
}
