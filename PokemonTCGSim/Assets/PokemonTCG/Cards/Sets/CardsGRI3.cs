using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Guardians Rising (GRI, v89), part 3: Fighting, Darkness and Metal (GRI 62-87). Sudowoodo, Lycanroc-GX, Beldum and Metang
    /// (Hidden Fates Shiny Vault) were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsGRI3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("GRI-62", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("GRI-64", new MachokeGRI());
            r.RegisterByKey("GRI-65", new ComboMon(new SettleTheScoreMon("Settle the Score", 80), new RecoilMon("Submission", 30)));
            r.RegisterByKey("GRI-67", new DoubleShotMon());
            r.RegisterByKey("GRI-68", new CardsNXD.BaredFangsMon("Finishing Stinger"));
            r.RegisterByKey("GRI-69", new FlipBonusMon("Rising Lunge", 30));
            r.RegisterByKey("GRI-71", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep), new LandslipMon()));
            r.RegisterByKey("GRI-72", new CardsUPR1.LessPerSelfCounterMon("Karate Chop", 10));
            r.RegisterByKey("GRI-73", new ComboMon(new NoRetreatMon("Corner"), new TailsNothingMon("Wild Kick")));
            r.RegisterByKey("GRI-75", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("GRI-76", new ComboMon(new CardsASC.ToolBonusMon("Enhanced Stomp", 60), new RecoilMon("High Horsepower", 40)));
            r.RegisterByKey("GRI-77", new ComboMon(new CardsCRZ1.PiercingMon("Swift"), new CosmicsplosionMon()));
            // Darkness
            r.RegisterByKey("GRI-78", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("GRI-79", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 30),
                new CardsASR1.PlusMon("Raven's Claw", 10, c => c.OpponentPokemon().Sum(m => m.DamageCounters))));
            r.RegisterByKey("GRI-80", new LimitationMon());
            r.RegisterByKey("GRI-81", new ComboMon(new CardsVIV1.FutureSightMon(), new DoomNewsMon()));
            r.RegisterByKey("GRI-82", new CardsFLF1.NoResistanceMon("Sky Uppercut"));
            // Metal
            r.RegisterByKey("GRI-85", new MetagrossGX());
            r.RegisterByKey("GRI-86", new CardsDAA2.EnergyLinkMon());
            r.RegisterByKey("GRI-87", new ComboMon(new CardsMEG.SnipeMon("Shining Arrow", 50), new LockAfterAttackMon("Fangs of the Sunne", true)));
        }

        // ================================================================== Fighting

        /// <summary>
        /// Daunting Pose: prevent all damage done to your Benched Pokémon by the opponent's attacks; the opponent's attacks and Abilities
        /// can't put damage counters on them. Cross Chop: heads, 30 more.
        /// </summary>
        private sealed class MachokeGRI : Mon
        {
            private static readonly Mon Chop = new FlipBonusMon("Cross Chop", 30);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target != null && info.Target.Owner == src.Player && !info.TargetIsActive && info.AgainstOpponent;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || !damageCounters || target == null || target.Owner != src.Player || src.State.IsActive(target)) return false;
                if (kind != EffectSourceKind.Attack && kind != EffectSourceKind.Ability) return false;
                int owner = src.Engine.CurrentSourceOwner;
                return owner >= 0 && owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Chop.HandleAttack(ctx, atk); }
        }

        /// <summary>Settle the Score: 80 more damage for each Prize card the opponent took during their last turn. Used for Settle the Score, Avenging Heart (the attack name and the numbers are passed in).</summary>
        internal sealed class SettleTheScoreMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public SettleTheScoreMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int taken = ctx.Opp.PrizesTakenTurn == ctx.OpponentsLastTurn ? ctx.Opp.PrizesTakenThatTurn : 0;
                ctx.DamageDefending(atk.Damage + bonus * taken);
                return true;
            }
        }

        /// <summary>Double Shot: 10 damage to 2 of the opponent's Benched Pokémon (no printed damage).</summary>
        private sealed class DoubleShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Shot") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return Nothing(ctx);
                var targets = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Double Shot: choose 2 of your opponent's Benched Pokémon");
                foreach (var t in targets) ctx.DamageTarget(t, 10);
                return true;
            }
        }

        /// <summary>Landslip: discard the top 3 cards of your deck; 100 damage for each Energy card discarded.</summary>
        private sealed class LandslipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Landslip") return false;
                var milled = ctx.Me.Deck.Count > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, 3) : new List<CardInstance>();
                int energy = milled.Count(c => c.Def.IsEnergy);
                if (energy == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * energy);
                return true;
            }
        }

        /// <summary>Cosmicsplosion: then this Pokémon is Knocked Out.</summary>
        private sealed class CosmicsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cosmicsplosion") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.Engine.KnockOut(me, "Cosmicsplosion");
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Limitation: the opponent can't play Supporter cards from their hand during their next turn.</summary>
        private sealed class LimitationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Limitation") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsTWM2.SupporterLockEffect("Your opponent can't play Supporter cards during their next turn (Limitation)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Doom News: put all Energy attached to this Pokémon into your hand; at the end of the opponent's next turn, the Defending Pokémon is Knocked Out.</summary>
        private sealed class DoomNewsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom News") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    foreach (var en in me.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(me, en);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of its next turn (Doom News)", ctx.Player, defending.Uid, ctx.Turn, "Doom News", 0));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>
        /// Geotech System: once during your turn, attach a Psychic or Metal Energy card from your discard pile to your Active Pokémon.
        /// Giga Hammer: can't be used next turn. Algorithm-GX: search your deck for up to 5 cards.
        /// </summary>
        private sealed class MetagrossGX : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new LockAfterAttackMon("Giga Hammer", true), new SearchAttackMon("Algorithm-GX", d => true, 5));

            private static List<CardInstance> Pile(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Psychic || d.EnergyProvides == EnergyType.Metal);
            }

            protected override string Activated { get { return "Geotech System"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && Pile(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                var pile = Pile(ctx);
                if (active == null || pile.Count == 0) return;
                ctx.AttachCardsAnyWay(pile, 1, (card, mon) => mon == active, "Geotech System: attach a Psychic or Metal Energy card to " + active.Name, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }
    }
}
