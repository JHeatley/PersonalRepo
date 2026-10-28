using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Forbidden Light (FLI, v84), part 2: Lightning and Psychic (FLI 34-56). Xurkitree, Inkay, Malamar, Poipole and Naganadel-GX
    /// were already in the pool (Hidden Fates Shiny Vault) and keep their scripts.
    /// </summary>
    public static class CardsFLI2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("FLI-34", new SearchAttackMon("Searching Magnet", d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 3));
            r.RegisterByKey("FLI-35", new LockAfterAttackMon("Zap Cannon", true));
            r.RegisterByKey("FLI-36", new MagnezoneFLI());
            r.RegisterByKey("FLI-38", new ParalyzeFlipMon("Volt Wave"));
            // Psychic
            r.RegisterByKey("FLI-40", new RotomFLI());
            r.RegisterByKey("FLI-41", new CardsPOR.MemoryLockMon("Memory Skip"));
            r.RegisterByKey("FLI-42", new MespritFLI());
            r.RegisterByKey("FLI-43", new ComboMon(new PsychicAbductionMon(), new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("FLI-44", new EnergyTeaserMon());
            r.RegisterByKey("FLI-45", new ComboMon(new SwitchSelfMon("Teleportation Burst", false), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            r.RegisterByKey("FLI-46", new HonedgeFLI());
            r.RegisterByKey("FLI-47", new ConditionAttackMon("Metal Sound", SpecialCondition.Confused));
            r.RegisterByKey("FLI-48", new CurseOfTheBladeMon());
            r.RegisterByKey("FLI-49", new ComboMon(new TickingKnockOutMon(), new CardsTWM1.HealSelfMon("Draining Blade", 30)));
            r.RegisterByKey("FLI-53", new CardsCRI.CounterConditionMon("Poison Point", SpecialCondition.Poisoned, new CardsUNM3.TwisterMon()));
            r.RegisterByKey("FLI-54", new ComboMon(new SearchAttackMon("Hyperspace Ring", CardsLOT4.IsItemCard, 2), new ParalyzeFlipMon("Psy Bolt")));
        }

        // ================================================================== Lightning

        /// <summary>Magnetic Circuit: as often as you like during your turn, attach a Lightning Energy card from your hand to 1 of your Pokémon. Zap Cannon: not usable next turn.</summary>
        private sealed class MagnezoneFLI : Mon
        {
            private const string Circuit = "Magnetic Circuit";
            private static readonly Mon ZapCannon = new LockAfterAttackMon("Zap Cannon", true);

            private static bool IsLightning(CardDef d) { return CardsSSP1.IsBasicOf(d, EnergyType.Lightning); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Circuit || !ctx.IsMyTurn) return false;
                return ctx.Me.Hand.Any(x => IsLightning(x.Def)) && ctx.MyPokemon().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, IsLightning, () => ctx.MyPokemon(), Circuit, "a Lightning Energy card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ZapCannon.HandleAttack(ctx, atk); }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Roto Motor: with 9 or more Pokémon Tool cards in your discard pile, ignore all Energy in this Pokémon's attack costs (a set
        /// cost, so Sticky Membrane can't add to it: Compendium, 2018-05-03). Plasma Slice: can't attack next turn.
        /// </summary>
        private sealed class RotomFLI : Mon
        {
            private static readonly Mon Slice = new LockAfterAttackMon("Plasma Slice", false);

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Me.Discard.Count(c => c.Def.IsTool) < 9) return;
                if (ctx.Engine.AbilityBlocked(me, "Roto Motor")) return;
                cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slice.HandleAttack(ctx, atk); }
        }

        /// <summary>Silent Waves: with Azelf in play, the opponent's Pokémon in play have no Resistance. Mind Splash: 50 more with Uxie on your Bench.</summary>
        private sealed class MespritFLI : Mon
        {
            private static readonly Mon Splash = new CardsFST2.ConditionBonusMon("Mind Splash", 50, c => c.Me.Bench.Any(m => m.Def.Name == "Uxie"));

            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount)
            {
                if (mon == null || mon.Owner == src.Player) return amount;
                return src.Me.InPlay().Any(m => m.Def.Name == "Azelf") ? 0 : amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Abduction: only if you went second, on your first turn: shuffle 1 of the opponent's Benched Pokémon and all attached cards into their deck.</summary>
        private sealed class PsychicAbductionMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Psychic Abduction" || CardsUNB1.SecondPlayersFirstTurn(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Abduction") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench().Where(m => ctx.CanAffect(m)).ToList();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Psychic Abduction: shuffle which of your opponent's Benched Pokémon into their deck?");
                if (target != null && ctx.InPlay(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Energy Teaser: move an Energy from 1 of the opponent's Benched Pokémon to another of their Pokémon.</summary>
        private sealed class EnergyTeaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Teaser") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var sources = ctx.OpponentBench().Where(m => m.Energy.Count > 0 && ctx.CanAffect(m)).ToList();
                if (sources.Count == 0 || ctx.OpponentPokemon().Count < 2) return true;
                var origin = ctx.ChoosePokemon(sources, "Energy Teaser: move an Energy from which of your opponent's Benched Pokémon?");
                if (origin == null) return true;
                var energy = origin.Energy.Count == 1 ? origin.Energy.ToList() : ctx.ChooseCards(origin.Energy.ToList(), 1, 1, "Energy Teaser: move which Energy?");
                var targets = ctx.OpponentPokemon(m => m != origin && ctx.CanAffect(m));
                if (energy.Count == 0 || targets.Count == 0) return true;
                var dest = ctx.ChoosePokemon(targets, "Energy Teaser: move it to which of your opponent's Pokémon?");
                if (dest != null) ctx.Engine.MoveEnergy(origin, dest, energy[0]);
                return true;
            }
        }

        /// <summary>Final Hour: if this Pokémon is Active and Knocked Out by damage from an opponent's attack, put 3 damage counters on 1 of the opponent's Pokémon.</summary>
        private sealed class HonedgeFLI : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive) return;
                var targets = src.OpponentPokemon();
                if (targets.Count == 0) return;
                Announce(src, "Final Hour");
                var target = src.ChoosePokemon(targets, "Final Hour: put 3 damage counters on which of your opponent's Pokémon?");
                if (target != null) src.PlaceCounters(target, 3);
            }
        }

        /// <summary>Curse of the Blade: 3 damage counters on each of the opponent's Pokémon that has a Pokémon Tool attached (Lysandre Labs doesn't change "has a Tool").</summary>
        private sealed class CurseOfTheBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curse of the Blade") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Tools.Count > 0)) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        /// <summary>
        /// Ticking Knock Out: during your next turn, if the Defending Pokémon is damaged by an attack, it is Knocked Out. The effect is
        /// on the Defending Pokémon and ends when it moves to the Bench (Compendium, 2018-05-03).
        /// </summary>
        private sealed class TickingKnockOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ticking Knock Out") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var fx = new TickingEffect(defending.Name + " will be Knocked Out if an attack damages it during your next turn (Ticking Knock Out)", ctx.Player, defending.Uid, ctx.Turn + 2);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }

        private sealed class TickingEffect : ContinuousEffect
        {
            public TickingEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || damage <= 0) return;
                engine.KnockOut(info.Target, "Ticking Knock Out");
            }
        }
    }
}
