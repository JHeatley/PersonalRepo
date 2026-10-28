using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sun &amp; Moon (SUM, v90), part 4: Darkness, Metal, Fairy and Dragon (SUM 76-96). Umbreon-GX (Hidden Fates Shiny Vault) and
    /// Solgaleo-GX were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsSUM4
    {
        private static readonly string[] DratiniLine = { "Dratini", "Dragonair", "Dragonite" };

        public static void Register(CardScriptRegistry r)
        {
            // Darkness
            r.RegisterByKey("SUM-77", new ComboMon(new EvilOrdersMon(), new CardsWHT.FlipMoreMon("Endeavor", 2, 30)));
            r.RegisterByKey("SUM-78", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("SUM-79", new ComboMon(new CardsDAA1.GustAttackMon("Taunt", false), new CardsPFL.DamagedBonusMon("Claw Rend", 30)));
            r.RegisterByKey("SUM-81", new DiscardDefendingToolsMon("Gnaw Through"));
            r.RegisterByKey("SUM-82", new CardsASC.CounterattackMon("Rough Skin", 3, new CardsOBF1.RetreatCostDamageMon("Aqua Impact", 20, false)));
            r.RegisterByKey("SUM-83", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("SUM-84", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("SUM-85", new ComboMon(new CardsASR1.PlusMon("False Accusation", 20, c => c.Opp.Hand.Count), new DiscardDefendingToolsMon("Obsidian Fang")));
            // Metal
            r.RegisterByKey("SUM-86", new CardsBST1.RadarMon("Spelunk", 3));
            r.RegisterByKey("SUM-87", new AlolanDugtrioSUM());
            r.RegisterByKey("SUM-88", new ComboMon(new MetallicSoundMon(), new FlipBonusMon("Aerial Ace", 30)));
            // Fairy
            r.RegisterByKey("SUM-90", new CardsFST1.FlipRecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SUM-91", new ComboMon(new NoRetreatMon("Dark Clamp"), new CardsFST2.ConditionBonusMon("Fight Back", 80, c => c.Pokemon.DamageCounters > 0)));
            r.RegisterByKey("SUM-92", new FlyAroundMon());
            r.RegisterByKey("SUM-93", new RibombeeSUM());
            // Dragon
            r.RegisterByKey("SUM-94", new SignsOfEvolutionLineMon("Signs of Evolution", DratiniLine));
            r.RegisterByKey("SUM-95", new DragonsWishMon());
            r.RegisterByKey("SUM-96", new ComboMon(new DragonWaveMon(), new TailsNothingMon("Giant Tail")));
        }

        // ================================================================== Darkness

        /// <summary>Evil Orders: search your deck for up to as many cards as you have Benched Pokémon and put them into your hand.</summary>
        private sealed class EvilOrdersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evil Orders") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Me.Bench.Count;
                if (n == 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => true, n, "Evil Orders: search your deck for up to " + n + " cards", false);
                return true;
            }
        }

        /// <summary>"Discard all Pokémon Tool cards from your opponent's Active Pokémon" (Gnaw Through; Obsidian Fang does it before the damage).</summary>
        internal sealed class DiscardDefendingToolsMon : Mon
        {
            private readonly string attack;

            public DiscardDefendingToolsMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending)) ctx.Engine.DiscardAllTools(defending);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>
        /// Tangling Hair: the opponent's Active Pokémon's Retreat Cost is [C] more (several stack). Dig Under: 50 damage to 1 of the
        /// opponent's Pokémon, not affected by Weakness or Resistance.
        /// </summary>
        private sealed class AlolanDugtrioSUM : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !src.State.IsActive(mon)) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig Under") return false;
                ctx.DamageAnyOpponent(50, true);
                return true;
            }
        }

        /// <summary>Metallic Sound: discard all Special Energy from each Pokémon (yours and your opponent's).</summary>
        private sealed class MetallicSoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metallic Sound") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                {
                    if (!ctx.InPlay(mon) || (mon.Owner != ctx.Player && !ctx.CanAffect(mon))) continue;
                    foreach (var en in mon.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(mon, en);
                }
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Fly Around: if any damage is done to this Pokémon by attacks during the opponent's next turn, flip a coin; heads, prevent that damage.</summary>
        private sealed class FlyAroundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fly Around") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new FlipToPreventEffect(me.Name + " may dodge attack damage (Fly Around)", ctx.Player, me.Uid, ctx.Turn + 1, "Fly Around"));
                return true;
            }
        }

        /// <summary>"If any damage is done to this Pokémon by attacks during your opponent's next turn, flip a coin. If heads, prevent that damage."</summary>
        internal sealed class FlipToPreventEffect : ContinuousEffect
        {
            private readonly string source;

            public FlipToPreventEffect(string name, int owner, int targetUid, int expiresAfterTurn, string source)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.source = source;
            }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return false;
                return engine.FlipCoin(Owner, source);
            }
        }

        /// <summary>Curative Pollen: once during your turn, heal 20 damage from 1 of your Pokémon.</summary>
        private sealed class RibombeeSUM : Mon
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)); }

            protected override string Activated { get { return "Curative Pollen"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = Hurt(ctx);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Curative Pollen: heal 20 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }
        }

        // ================================================================== Dragon

        /// <summary>"Search your deck for a [A], a [B], and a [C], reveal them, and put them into your hand" (1 of each name).</summary>
        internal sealed class SignsOfEvolutionLineMon : Mon
        {
            private readonly string attack;
            private readonly string[] names;

            public SignsOfEvolutionLineMon(string attack, string[] names)
            {
                this.attack = attack;
                this.names = names;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && names.Contains(d.Name), names.Length,
                    attack + ": search your deck for a " + string.Join(", a ", names) + " (1 of each)");
                var keep = new List<CardInstance>();
                foreach (var card in found)
                {
                    if (keep.Any(k => k.Def.Name == card.Def.Name)) ctx.Me.Deck.Add(card);
                    else keep.Add(card);
                }
                ctx.Me.Hand.AddRange(keep);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Dragon's Wish: during your next turn, you may attach any number of Energy cards from your hand to your Pokémon.</summary>
        private sealed class DragonsWishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon's Wish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fx = new DragonsWishEffect("You may attach any number of Energy cards from your hand during your next turn (Dragon's Wish)", ctx.Player, ctx.Turn + 2);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>The owner may attach any number of Energy cards from their hand while this lasts.</summary>
        public sealed class DragonsWishEffect : ContinuousEffect
        {
            public DragonsWishEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool AllowsAnyNumberOfEnergyAttachments(GameEngine engine, int player) { return player == Owner; }
        }

        /// <summary>Dragon Wave: discard a Grass Energy and a Lightning Energy from this Pokémon.</summary>
        private sealed class DragonWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Wave") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.DiscardEnergyFrom(me, 1, "Dragon Wave: discard a Grass Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, EnergyType.Grass));
                ctx.DiscardEnergyFrom(me, 1, "Dragon Wave: discard a Lightning Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, EnergyType.Lightning));
                return true;
            }
        }
    }
}
