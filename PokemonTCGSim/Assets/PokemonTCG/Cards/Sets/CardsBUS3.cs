using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Burning Shadows (BUS, v88), part 3: Fighting and Darkness (BUS 64-90). Darkrai-GX (Hidden Fates Shiny Vault) was already in
    /// the pool and keeps its script.
    /// </summary>
    public static class CardsBUS3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("BUS-64", new ComboMon(new CardsPAL1.EvolutionBonusMon("Cross-Cut", 60), new CardsPAF1.DiscardStadiumMon("Bedrock Breaker"),
                new CardsFLF1.NoResistanceMon("Muscle Punch-GX")));
            r.RegisterByKey("BUS-66", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new TailsNothingMon("Megahorn")));
            r.RegisterByKey("BUS-67", new RhyperiorBUS());
            r.RegisterByKey("BUS-68", new LunatoneBUS());
            r.RegisterByKey("BUS-69", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsFST2.ConditionBonusMon("Solar Heat", 20, c => c.State.Stadium != null)));
            r.RegisterByKey("BUS-71", new LucarioBUS());
            r.RegisterByKey("BUS-72", new SawkBUS());
            r.RegisterByKey("BUS-74", new ComboMon(new GutsyHammerMon(), new CardsWHT.FlipMoreMon("Double Stomp", 2, 40)));
            r.RegisterByKey("BUS-75", new ComboMon(new CardsCRZ1.BasicTargetBonusMon("Dangerous Claws", 30), new NoRetreatMon("Corner")));
            r.RegisterByKey("BUS-77", new FlipBonusMon("Stomp", 20));
            r.RegisterByKey("BUS-78", new ComboMon(new CardsASC.PushDownMon("Kick Away"), new CardsBKT1.ToughHammerMon("Vigorous Dash")));
            r.RegisterByKey("BUS-79", new IntentionalGroundingMon());
            r.RegisterByKey("BUS-80", new MarshadowGX());
            // Darkness
            r.RegisterByKey("BUS-81", new CardsRCL1.SetupBoostMon("Focus Energy", "Bite", 50));
            r.RegisterByKey("BUS-82", new ComboMon(new CardsASC.ToolBonusMon("Enhanced Fang", 50), new TailsNothingMon("Hyper Fang")));
            r.RegisterByKey("BUS-83", new ComboMon(new CardsUNM1.NamedBenchSearchMon("Division", "Alolan Grimer", 1), new ConditionAttackMon("Slippery Sludge", SpecialCondition.Confused)));
            r.RegisterByKey("BUS-84", new ComboMon(new CardsASR1.PlusMon("Chemical Breath", 70, CardsASR1.DefendingConditions), new CardsDRI2.CrunchMon("Crunch"), new TriHazardMon()));
            r.RegisterByKey("BUS-85", new CardsRCL1.SetupBoostMon("Hone Claws", "Slash", 60));
            r.RegisterByKey("BUS-86", new RuleOfEvilMon());
            r.RegisterByKey("BUS-87", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new CardsFST2.ConditionBonusMon("Dark Raid", 80, c => c.Opp.GxUsed)));
            r.RegisterByKey("BUS-89", new ParalyzeFlipMon("Constrict"));
            r.RegisterByKey("BUS-90", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new EnergySloshMon()));
        }

        // ================================================================== Fighting

        /// <summary>
        /// Toppling Wind: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may discard the top 3 cards of the
        /// opponent's deck. Rock Wrecker: not affected by Weakness or Resistance; it can't attack during your next turn.
        /// </summary>
        private sealed class RhyperiorBUS : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Toppling Wind", ctx.Opp.Deck.Count > 0, "Use Toppling Wind? Discard the top 3 cards of your opponent's deck.")) return;
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Wrecker") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Heal Block: while you have Solrock in play, no Pokémon (yours or the opponent's) can be healed.</summary>
        private sealed class LunatoneBUS : Mon
        {
            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.Me.InPlay().Any(m => m.Def.Name == "Solrock");
            }
        }

        /// <summary>
        /// Stance: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may prevent all effects of attacks,
        /// including damage, done to it during the opponent's next turn. It's an effect on this Pokémon, so it stays when it moves to
        /// the Bench (Compendium, 2017-08-03).
        /// </summary>
        private sealed class LucarioBUS : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Stance", true, "Use Stance? Prevent all effects of attacks, including damage, done to " + ctx.Pokemon.Name + " during your opponent's next turn.")) return;
                ctx.AddEffect(new StanceEffect(ctx.Pokemon.Name + " is protected from attacks during the opponent's next turn (Stance)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
            }
        }

        /// <summary>Like ProtectEffect, but it doesn't end when the Pokémon moves between the Active Spot and the Bench.</summary>
        public sealed class StanceEffect : ContinuousEffect
        {
            public StanceEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                return target.Uid == TargetUid && kind == EffectSourceKind.Attack;
            }
        }

        /// <summary>
        /// Quick Guard: during the opponent's next turn, prevent all damage done to it by attacks from Basic Pokémon; it can't use Quick
        /// Guard during your next turn. Brick Break: not affected by Resistance or effects on the opponent's Active Pokémon.
        /// </summary>
        private sealed class SawkBUS : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Quick Guard")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    CardsMEG.ShieldFromBasics(ctx, atk.Name);
                    ctx.SelfCantUseAttackNextTurn("Quick Guard");
                    return true;
                }
                if (atk.Name != "Brick Break") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, false, true);
                return true;
            }
        }

        /// <summary>Gutsy Hammer: then this Pokémon does 10 damage to itself for each damage counter on it.</summary>
        private sealed class GutsyHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gutsy Hammer") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.DamageSelf(10 * me.DamageCounters);
                return true;
            }
        }

        /// <summary>Intentional Grounding: discard a Pokémon Tool card from your hand; if you can't, this attack does nothing.</summary>
        private sealed class IntentionalGroundingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intentional Grounding") return false;
                var tools = ctx.Me.Hand.Where(c => c.Def.IsTool).ToList();
                if (tools.Count == 0) return Nothing(ctx);
                var pick = ctx.ChooseCards(tools, 1, 1, "Intentional Grounding: discard a Pokémon Tool card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Shadow Hunt: it can use the attacks of any Basic Pokémon in your discard pile (it still needs the Energy for them).
        /// Peerless Hundred Blows-GX: 50 damage for each basic Energy card attached to it.
        /// </summary>
        private sealed class MarshadowGX : Mon
        {
            private static readonly Mon Blows = new CardsASR1.TimesMon("Peerless Hundred Blows-GX", c => c.Pokemon.Energy.Count(en => en.Def.IsBasicEnergy));

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var seen = new HashSet<string>();
                foreach (var card in src.Me.Discard)
                {
                    var d = card.Def;
                    if (!d.IsBasicPokemon || !seen.Add(d.BaseKey)) continue;
                    for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blows.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Tri Hazard-GX: switch in 1 of the opponent's Benched Pokémon; it's Burned, Paralyzed and Poisoned. With no Bench it does nothing (Compendium, 2017-08-03).</summary>
        private sealed class TriHazardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tri Hazard-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var target = ctx.GustOpponent("Tri Hazard-GX: switch which of your opponent's Benched Pokémon into the Active Spot?");
                if (target == null || !ctx.InPlay(target) || !ctx.IsActive(target)) return true;
                ctx.ApplyCondition(target, SpecialCondition.Burned);
                ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>
        /// Rule of Evil: 60 damage to each Pokémon that has an Ability (both players'). Only Abilities that work count (Compendium,
        /// Garbotoxin, 2017-09-14).
        /// </summary>
        private sealed class RuleOfEvilMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rule of Evil") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon().Concat(ctx.MyPokemon()).Where(m => ctx.Engine.HasAbility(m)).ToList();
                foreach (var mon in targets)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 60);
                return true;
            }
        }

        /// <summary>Energy Slosh: you may move all Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class EnergySloshMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Slosh") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Energy Slosh: move all Energy from " + me.Name + " to 1 of your Benched Pokémon?")) return true;
                var dest = ctx.ChoosePokemon(ctx.MyBench(), "Energy Slosh: move the Energy to which Benched Pokémon?");
                if (dest == null) return true;
                foreach (var en in me.Energy.ToList()) ctx.Engine.MoveEnergy(me, dest, en);
                return true;
            }
        }
    }
}
