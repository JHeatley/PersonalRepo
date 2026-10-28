using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ultra Prism (UPR, v85), part 2: Lightning and Psychic (UPR 43-63). Rotom has the same Ability and attack as its Forbidden
    /// Light print and reuses that script.
    /// </summary>
    public static class CardsUPR2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("UPR-43", new RecoilMon("Thunder", 30));
            r.RegisterByKey("UPR-44", new SteelShortMon());
            r.RegisterByKey("UPR-45", new CardsTEU3.EvolutionaryAdvantageMon());
            r.RegisterByKey("UPR-46", new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1));
            r.RegisterByKey("UPR-47", new DisconnectMon());
            r.RegisterByKey("UPR-48", new LuxrayUPR());
            r.RegisterByKey("UPR-49", new ComboMon(new SnugglyGeneratorMon(), new ParalyzeFlipMon("Nuzzle")));
            CardsUPR1.Same(r, "UPR-50", "FLI-40");
            // Psychic
            r.RegisterByKey("UPR-51", new ConditionAttackMon("Creepy Wind", SpecialCondition.Confused));
            r.RegisterByKey("UPR-52", new ComboMon(new DamageTransportMon(), new CardsDRI1.WhirlwindMon("Wind Wheel")));
            r.RegisterByKey("UPR-53", new ComboMon(new LightlessWorldMon(), new TerrifyMon()));
            r.RegisterByKey("UPR-54", new CardsRCL1.SetupBoostMon("Hone Claws", "Pierce", 60));
            r.RegisterByKey("UPR-55", new CardsCES1.ConditionsMon("Dangerous Stinger", SpecialCondition.Paralyzed, SpecialCondition.Poisoned));
            r.RegisterByKey("UPR-56", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("UPR-57", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Exact Revenge", 70, c => c.WasKnockedOutLastOpponentTurn(d => d.PokemonType == EnergyType.Fighting || d.SecondType == EnergyType.Fighting, true))));
            r.RegisterByKey("UPR-58", new StarAttachMon("Chaotic Star", EnergyType.Psychic, new CardsTWM1.SelfDiscardMon("Crisis Dive", 2)));
            r.RegisterByKey("UPR-59", new ComboMon(new LunarPaybackMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 20)));
            r.RegisterByKey("UPR-60", new SwitchSelfMon("Teleport", false));
            r.RegisterByKey("UPR-61", new SwitchSelfMon("Teleport", false));
            r.RegisterByKey("UPR-62", new ComboMon(new StarEnergyMon("Full Moon Star", EnergyType.Psychic), new CardsASR1.TimesMon("Psystorm", EnergyOnAllPokemon)));
            r.RegisterByKey("UPR-63", new DawnWingsNecrozmaGX());
        }

        /// <summary>Energy attached to all Pokémon in play, both players' (Psystorm).</summary>
        internal static int EnergyOnAllPokemon(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)) + ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
        }

        /// <summary>"You can use this attack only if you have more Prize cards remaining than your opponent" (Sun's / Moon's Eclipse-GX).</summary>
        internal static bool BehindOnPrizes(EffectContext ctx) { return ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count; }

        // ================================================================== Shared helpers

        /// <summary>
        /// Chaotic Star / Nightmare Star: when you play this Pokémon from your hand onto your Bench during your turn, you may attach
        /// 2 [T] Energy cards from your hand to it. With only 1 in hand you may attach that 1 (your ruling, v85). The attacks are
        /// handled by the given parts.
        /// </summary>
        internal sealed class StarAttachMon : Mon
        {
            private readonly string ability;
            private readonly EnergyType type;
            private readonly Mon[] parts;

            public StarAttachMon(string ability, EnergyType type, params Mon[] parts)
            {
                this.ability = ability;
                this.type = type;
                this.parts = parts;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var energy = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, type)).ToList();
                if (!TriggerOnce(ctx, ability, energy.Count > 0 && ctx.Engine.CanAttachFromHand(me),
                    "Use " + ability + "? Attach up to 2 " + type + " Energy cards from your hand to " + me.Name + ".")) return;
                ctx.AttachCardsAnyWay(energy, Math.Min(2, energy.Count), (card, mon) => mon == me, ability + ": attach " + type + " Energy from your hand to " + me.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                foreach (var part in parts)
                    if (part.HandleAttack(ctx, atk)) return true;
                return false;
            }
        }

        /// <summary>Full Moon Star / Radiant Star: for each of the opponent's Pokémon in play, attach a [T] Energy card from your discard pile to your Pokémon in any way you like.</summary>
        internal sealed class StarEnergyMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public StarEnergyMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int count = ctx.OpponentPokemon().Count;
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (count == 0 || energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, Math.Min(count, energy.Count), (card, mon) => true, attack + ": attach " + type + " Energy from your discard pile", false);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Steel Short: if the opponent's Active Pokémon is a Metal Pokémon, it is now Paralyzed.</summary>
        private sealed class SteelShortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steel Short") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.Engine.IsType(defending, EnergyType.Metal)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Disconnect: the opponent can't play Item cards from their hand during their next turn.</summary>
        internal sealed class DisconnectMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Disconnect") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn (Disconnect).");
                return true;
            }
        }

        /// <summary>
        /// Intimidating Fang: while this Pokémon is your Active Pokémon, the opponent's Active Pokémon's attacks do 30 less damage
        /// (before Weakness and Resistance), to every target (Compendium for Sandile, 2019-05-02). Volt Bolt: discard all Lightning
        /// Energy from this Pokémon; 150 damage to 1 of the opponent's Pokémon (no printed damage).
        /// </summary>
        private sealed class LuxrayUPR : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || !src.PokemonIsActive || attacker == null || attacker.Owner == src.Player || attacker != src.Opp.Active || damage <= 0) return damage;
                return Math.Max(0, damage - 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volt Bolt") return false;
                var me = ctx.Pokemon;
                foreach (var e in me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, e);
                ctx.DamageAnyOpponent(150);
                return true;
            }
        }

        /// <summary>Snuggly Generator: for each of your Benched Pokémon with the Nuzzle attack, search your deck for a Lightning Energy card and attach it to that Pokémon.</summary>
        private sealed class SnugglyGeneratorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snuggly Generator") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var nuzzlers = ctx.MyBench().Where(m => m.Def.Attacks.Any(a => a.Name == "Nuzzle")).ToList();
                if (nuzzlers.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                foreach (var mon in nuzzlers)
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 1, "Snuggly Generator: search your deck for a Lightning Energy card for " + mon.Name);
                    if (found.Count == 0) break;
                    if (ctx.InPlay(mon) && ctx.Engine.AttachEnergyCard(found[0], mon)) continue;
                    ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Damage Transport: move 4 damage counters from each of your Pokémon to the opponent's Active Pokémon. A Pokémon with 4 or
        /// fewer has them all moved; none can be skipped (Compendium, 2018-02-01).
        /// </summary>
        private sealed class DamageTransportMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Transport") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0))
                {
                    var defending = ctx.Defending;
                    if (defending == null) break;
                    ctx.Engine.MoveDamageCounters(mon, defending, Math.Min(4, mon.DamageCounters), "Damage Transport");
                }
                return true;
            }
        }

        /// <summary>Lightless World: put 2 Supporter cards from your discard pile into your hand (as many as there are, up to 2).</summary>
        private sealed class LightlessWorldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightless World") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                int n = Math.Min(2, supporters.Count);
                var picks = supporters.Count <= n ? supporters : ctx.ChooseCards(supporters, n, n, "Lightless World: put which 2 Supporter cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Terrify: if the Defending Pokémon is a Basic Pokémon, it can't attack during the opponent's next turn.</summary>
        private sealed class TerrifyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Terrify") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !defending.Def.IsBasicPokemon || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Terrify)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Lunar Payback: discard an Energy from this Pokémon; if you do, swap the damage counters on it with those on the opponent's Active Pokémon.</summary>
        private sealed class LunarPaybackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lunar Payback") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Lunar Payback: discard an Energy from " + me.Name) < 1) return true;
                var defending = ctx.Defending;
                if (defending != null) ctx.Engine.SwapDamageCounters(me, defending, "Lunar Payback");
                return true;
            }
        }

        /// <summary>
        /// Invasion: once during your turn, if this Pokémon is on your Bench, you may switch it with your Active Pokémon. Dark Flash:
        /// not affected by Resistance. Moon's Eclipse-GX: only with more Prize cards left than the opponent; prevent all effects of
        /// attacks, including damage, done to this Pokémon during the opponent's next turn.
        /// </summary>
        private sealed class DawnWingsNecrozmaGX : Mon
        {
            private static readonly Mon Flash = new CardsFLF1.NoResistanceMon("Dark Flash");

            protected override string Activated { get { return "Invasion"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx) { ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon); }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Moon's Eclipse-GX" || BehindOnPrizes(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Moon's Eclipse-GX")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.ProtectSelfNextTurn();
                    return true;
                }
                return Flash.HandleAttack(ctx, atk);
            }
        }
    }
}
