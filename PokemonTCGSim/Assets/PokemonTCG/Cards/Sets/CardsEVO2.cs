using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Evolutions (EVO, v92), part 2: the Lightning, Psychic and Fighting Pokémon (35-62).</summary>
    public static class CardsEVO2
    {
        public static void Register(CardScriptRegistry r)
        {
            CardsUPR1.Same(r, "EVO-35", "CEL-5"); // Thunder Jolt: flip, 10 to itself on tails (Celebrations' Pikachu has no Resistance)
            r.RegisterByKey("EVO-36", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning), new SparkBoltMon()));
            r.RegisterByKey("EVO-37", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new RecoilMon("Self-Destruct", 50)));
            r.RegisterByKey("EVO-38", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new RecoilMon("Self-Destruct", 80)));
            r.RegisterByKey("EVO-39", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("EVO-40", new ElectrodeEVO());
            r.RegisterByKey("EVO-41", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new CardsGRI2.FlipBonusOrRecoilMon("Thunder Punch", 10, 10)));
            r.RegisterByKey("EVO-42", new ComboMon(new RecoilMon("Thunder", 30), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("EVO-43", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("EVO-44", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("EVO-45", new ComboMon(new NoRetreatMon("Rumble"), new TailSwingMon()));
            r.RegisterByKey("EVO-46", new CardsCEC1.ToxicCECMon("Toxic Drill"));
            r.RegisterByKey("EVO-47", new ComboMon(new LittleGrudgeMon(), new CardsPAR2.FlipConditionMon("Nightmare", SpecialCondition.Asleep)));
            r.RegisterByKey("EVO-48", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new CardsBLW.DreamEaterMon()));
            r.RegisterByKey("EVO-49", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("EVO-50", new CardsBS1.FoulGasMon("Foul Gas"));
            r.RegisterByKey("EVO-51", new MewtwoEVO());
            r.RegisterByKey("EVO-52", new ComboMon(new CardsGRI2.AttachDiscardEnergyToSelfMon("Energy Absorption"), new CardsTWM1.HealSelfMon("Regeneration", 60)));
            r.RegisterByKey("EVO-53", new MewEVO());
            r.RegisterByKey("EVO-54", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("EVO-55", new DiglettEVO());
            r.RegisterByKey("EVO-56", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20));
            r.RegisterByKey("EVO-57", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("EVO-58", new ComboMon(new CardsUPR1.LessPerSelfCounterMon("Karate Chop", 10), new RecoilMon("Submission", 20)));
            r.RegisterByKey("EVO-59", new CardsASC.CounterattackMon("Counterattack", 3, null));
            r.RegisterByKey("EVO-60", new CardsFST1.NextTurnBonusMon("Boomerang Lariat", 100));
            r.RegisterByKey("EVO-61", new CardsLOR1.HardenMon("Harden", 60));
        }

        // ================================================================== Lightning

        /// <summary>Spark Bolt: you may discard all Energy from this Pokémon; if you do, 70 more damage.</summary>
        private sealed class SparkBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spark Bolt") return false;
                var me = ctx.Pokemon;
                int bonus = 0;
                if (me.Energy.Count > 0 && ctx.AskYesNo("Spark Bolt: discard all Energy from " + me.Name + " to do 70 more damage?")
                    && ctx.Engine.DiscardAllEnergy(me) > 0) bonus = 70;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>
        /// Buzzap Thunder (errata, 2017-01-19): once during your turn (before your attack), you may Knock Out this Pokémon (your
        /// opponent takes a Prize card for it) and attach it to 1 of your other [L] Pokémon as a Special Energy card that
        /// provides [L][L] while it's attached to any Pokémon. The target is chosen first; the card is attached once the Knock Out
        /// has put it in the discard pile (the Voltorb under it and everything attached stays there). In the hand or discard pile
        /// it's a Pokémon card again.
        /// </summary>
        internal sealed class ElectrodeEVO : Mon
        {
            internal const string Buzzap = "Buzzap Thunder";
            private const string TargetMarker = "Buzzap Thunder target";

            protected override string Activated { get { return Buzzap; } }
            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == Buzzap; }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && ctx.Engine.IsType(m, EnergyType.Lightning));
            }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Buzzap Thunder: attach " + ctx.Pokemon.Name + " as Energy to which of your Lightning Pokémon?");
                if (target == null) return;
                ctx.Pokemon.Markers[TargetMarker] = target.Uid;
                if (!ctx.Engine.KnockOut(ctx.Pokemon, Buzzap)) ctx.Pokemon.Markers.Remove(TargetMarker);
            }

            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                int uid;
                if (me == null || !me.Markers.TryGetValue(TargetMarker, out uid)) return;
                me.Markers.Remove(TargetMarker);
                var target = src.State.FindPokemon(uid);
                var card = src.Card;
                if (target == null || target.Owner != src.Player || card == null || !src.Me.Discard.Remove(card)) return;
                src.Engine.AttachEnergyCard(card, target);
                src.Log(card.Def.Name + " was attached to " + target.Name + " as a Special Energy card that provides Lightning Lightning (Buzzap Thunder).");
            }

            /// <summary>Attached as Energy: [L][L].</summary>
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return new List<int> { EnergyMask.Of(EnergyType.Lightning), EnergyMask.Of(EnergyType.Lightning) };
            }
        }

        // ================================================================== Psychic

        /// <summary>Tail Swing: 20 damage to each of the opponent's Benched Basic Pokémon (no Weakness or Resistance).</summary>
        private sealed class TailSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Swing") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var target in ctx.OpponentBench().Where(m => m.Def.IsBasicPokemon).ToList())
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>
        /// Little Grudge: during your opponent's next turn, if this Pokémon is Knocked Out by damage from an attack, discard an
        /// Energy from the Attacking Pokémon (even if that attack moved it to the Bench: Compendium, 2016-11-03).
        /// </summary>
        private sealed class LittleGrudgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Little Grudge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new LittleGrudgeEffect(ctx.Pokemon.Name + ": if it's Knocked Out by an attack during the opponent's next turn, the Attacking Pokémon discards an Energy (Little Grudge)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class LittleGrudgeEffect : ContinuousEffect
        {
            public LittleGrudgeEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return;
                if (!engine.IsKnockedOut(info.Target)) return;
                var attacker = info.Attacker;
                if (engine.State.FindPokemon(attacker.Uid) != attacker || attacker.Energy.Count == 0) return;
                var pick = attacker.Energy.Count == 1 ? attacker.Energy[0]
                    : engine.ChooseCards(Owner, attacker.Energy.ToList(), 1, 1, "Little Grudge: discard an Energy from " + attacker.Name, null)[0];
                engine.DiscardEnergy(attacker, pick);
            }
        }

        /// <summary>
        /// Psychic: 20 more for each Energy on the opponent's Active Pokémon. Barrier: prevent all effects of attacks, including
        /// damage, done to this Pokémon during the opponent's next turn; it can't be used if 1 of your Pokémon used Barrier
        /// during your last turn (a game-state check: switching or Pokémon Ranger doesn't change it, Compendium 2016).
        /// </summary>
        private sealed class MewtwoEVO : Mon
        {
            private const string Barrier = "Barrier";
            private static readonly Mon Psy = new CardsMEG.DefendingEnergyBonusMon("Psychic", 20);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != Barrier) return true;
                return !(ctx.Me.LastAttackName == Barrier && ctx.Me.LastAttackTurn == ctx.MyLastTurn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Barrier) return Psy.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Neutral Shield: prevent all effects of attacks, including damage, done to this Pokémon by the opponent's Evolution Pokémon. Psy Bolt: flip, Paralyzed.</summary>
        private sealed class MewEVO : Mon
        {
            private static readonly Mon Bolt = new ParalyzeFlipMon("Psy Bolt");

            private static bool OpposingEvolution(EffectContext src, PokemonInPlay attacker)
            {
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsEvolution;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target != null && target == src.Pokemon && kind == EffectSourceKind.Attack && OpposingEvolution(src, src.Engine.CurrentSourcePokemon);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && OpposingEvolution(src, info.Attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bolt.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Submerge: while this Pokémon is on your Bench, prevent all damage done to it by attacks (both players'). Dig Through: 30 to 1 Benched Pokémon only.</summary>
        private sealed class DiglettEVO : Mon
        {
            private static readonly Mon Dig = new CardsSUM3.BenchOnlyMon("Dig Through", 30);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dig.HandleAttack(ctx, atk); }
        }
    }
}
