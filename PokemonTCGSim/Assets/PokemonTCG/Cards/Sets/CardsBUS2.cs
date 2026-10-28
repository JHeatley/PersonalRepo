using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Burning Shadows (BUS, v88), part 2: Lightning and Psychic (BUS 40-63). Seviper (Hidden Fates Shiny Vault) was already in the
    /// pool and keeps its script.
    /// </summary>
    public static class CardsBUS2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Lightning
            r.RegisterByKey("BUS-40", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("BUS-41", new RaichuBUS());
            r.RegisterByKey("BUS-42", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("BUS-43", new ComboMon(new ThunderPunchMon(), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("BUS-44", new CardsFST2.ConditionBonusMon("Aqua Shock", 30,
                c => c.Defending != null && c.Defending.Energy.Any(en => CardsDRI1.ProvidesType(c, c.Defending, en, EnergyType.Water))));
            r.RegisterByKey("BUS-45", new SmallAppetiteMon());
            r.RegisterByKey("BUS-46", new ComboMon(new NoRetreatMon("Dark Clamp"), new VacuumBoltMon()));
            r.RegisterByKey("BUS-47", new FlipBonusMon("Electrosmash", 20));
            // Psychic
            r.RegisterByKey("BUS-48", new SlowkingBUS());
            r.RegisterByKey("BUS-49", new CardsASR1.TimesMon("Shadowy Knot", c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending)));
            r.RegisterByKey("BUS-51", new CardsRCL1.DiscardBasicToBenchMon("Dark Guidance"));
            r.RegisterByKey("BUS-52", new ComboMon(new NightRoamMon(), new FlipBonusMon("Ambush", 30)));
            r.RegisterByKey("BUS-53", new DusknoirBUS());
            r.RegisterByKey("BUS-54", new FlipBonusMon("Frog Hop", 20));
            r.RegisterByKey("BUS-55", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned), new PoisonBoostMon()));
            r.RegisterByKey("BUS-56", new CardsBLK.VenoshockMon(40));
            r.RegisterByKey("BUS-57", new SwitchSelfMon("Spin Turn", false));
            r.RegisterByKey("BUS-58", new ComboMon(new ConditionAttackMon("Poison Horn", SpecialCondition.Poisoned), new CardsFLF1.NoResistanceMon("Steamroller")));
            r.RegisterByKey("BUS-59", new CardsUNB2.PerplexingEyesMon());
            r.RegisterByKey("BUS-60", new ComboMon(new DrawAttackMon("Allure", 3), new CardsASR1.TimesMon("Hand Kinesis", c => c.Me.Hand.Count)));
            r.RegisterByKey("BUS-61", new CardsTWM1.HealSelfMon("Absorb Life", 10));
            r.RegisterByKey("BUS-62", new ComboMon(new NoRetreatMon("Sand Tomb"), new DustStormMon()));
            r.RegisterByKey("BUS-63", new NecrozmaGX());
        }

        // ================================================================== Lightning

        /// <summary>Evoshock: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may make the opponent's Active Pokémon Paralyzed. Volt Tackle: 30 to itself.</summary>
        private sealed class RaichuBUS : Mon
        {
            private static readonly Mon Tackle = new RecoilMon("Volt Tackle", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Evoshock", target != null, "Use Evoshock? Your opponent's Active Pokémon is now Paralyzed.")) return;
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Paralyzed);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>Thunder Punch: flip a coin; heads, 60 more damage; tails, this Pokémon does 20 damage to itself.</summary>
        private sealed class ThunderPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Punch") return false;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage + 60);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(20);
                return true;
            }
        }

        /// <summary>Small Appetite: nothing if the opponent's Active Pokémon's maximum HP is 100 or more (Tools count: Compendium, 2017-08-03).</summary>
        private sealed class SmallAppetiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Small Appetite") return false;
                var defending = ctx.Defending;
                if (defending == null || ctx.Engine.MaxHp(defending) >= 100) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Vacuum Bolt: you may do 80 more damage; if you do, it also does 80 damage to 1 of your Pokémon.</summary>
        private sealed class VacuumBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vacuum Bolt") return false;
                bool more = ctx.AskYesNo("Vacuum Bolt: do 80 more damage? If you do, this attack also does 80 damage to 1 of your Pokémon.");
                ctx.DamageDefending(atk.Damage + (more ? 80 : 0));
                if (!more) return true;
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Vacuum Bolt: do 80 damage to which of your Pokémon?");
                if (target != null) ctx.DamageTarget(target, 80);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Drift Ashore: search your deck for a card. Unarmed: with no cards in your hand, it costs no Energy.</summary>
        private sealed class SlowkingBUS : Mon
        {
            private static readonly Mon Drift = new SearchAttackMon("Drift Ashore", d => true, 1);

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name == "Unarmed" && ctx.Me.Hand.Count == 0) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drift.HandleAttack(ctx, atk); }
        }

        /// <summary>Night Roam: put 1 damage counter on each Pokémon in play (both yours and your opponent's).</summary>
        private sealed class NightRoamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Roam") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                    if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>
        /// Dark Invitation: once during your turn, look at the opponent's hand; you may put a Basic Pokémon from it onto their Bench,
        /// with 3 damage counters on it. Not usable with a full opposing Bench (Compendium, 2017-08-03). Mind Jack: 30 more for each of
        /// the opponent's Benched Pokémon.
        /// </summary>
        private sealed class DusknoirBUS : Mon
        {
            private static readonly Mon Jack = new CardsASR1.PlusMon("Mind Jack", 30, c => c.Opp.Bench.Count);

            protected override string Activated { get { return "Dark Invitation"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Hand.Count > 0 && ctx.Engine.BenchHasRoom(ctx.Opponent);
            }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.NoteLookedAt(hand, "Your opponent's hand", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.Their(ctx.Opponent) + " hand (Dark Invitation).");
                var basics = hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0) return;
                var pick = ctx.ChooseCards(basics, 0, 1, "Dark Invitation: you may put a Basic Pokémon from your opponent's hand onto their Bench (it gets 3 damage counters)");
                if (pick.Count == 0 || !ctx.Engine.BenchHasRoom(ctx.Opponent) || !ctx.Opp.Hand.Remove(pick[0])) return;
                var benched = ctx.Engine.PutOntoBench(ctx.Opponent, pick[0], false);
                if (benched != null) ctx.PlaceCounters(benched, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Jack.HandleAttack(ctx, atk); }
        }

        /// <summary>Poison Boost: 80 more if this Pokémon is Poisoned; then it's no longer Poisoned.</summary>
        private sealed class PoisonBoostMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Boost") return false;
                var me = ctx.Pokemon;
                bool poisoned = me.Has(SpecialCondition.Poisoned);
                ctx.DamageDefending(atk.Damage + (poisoned ? 80 : 0));
                if (poisoned && ctx.InPlay(me)) ctx.Engine.RemoveCondition(me, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Dust Storm: discard all Tools and Energy attached to both Active Pokémon; the Pokémon stay in play (your ruling, v88).</summary>
        private sealed class DustStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dust Storm") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                {
                    ctx.Engine.DiscardAllEnergy(defending);
                    ctx.Engine.DiscardAllTools(defending);
                }
                if (ctx.InPlay(me))
                {
                    ctx.Engine.DiscardAllEnergy(me);
                    ctx.Engine.DiscardAllTools(me);
                }
                return true;
            }
        }

        /// <summary>
        /// Light's End: prevent all damage done to this Pokémon by attacks from the opponent's Colorless Pokémon. Prismatic Burst:
        /// discard all Psychic Energy from it, 60 more for each card discarded. Black Ray-GX: 100 to each of the opponent's
        /// Pokémon-GX and Pokémon-EX, not affected by Weakness or Resistance.
        /// </summary>
        private sealed class NecrozmaGX : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.AgainstOpponent && src.Engine.IsType(info.Attacker, EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Prismatic Burst")
                {
                    int n = 0;
                    foreach (var en in me.Energy.Where(x => CardsDRI1.ProvidesType(ctx, me, x, EnergyType.Psychic)).ToList())
                        if (ctx.Engine.DiscardEnergy(me, en)) n++;
                    ctx.DamageDefending(atk.Damage + 60 * n);
                    return true;
                }
                if (atk.Name != "Black Ray-GX") return false;
                foreach (var mon in ctx.OpponentPokemon(m => m.Def.IsGXorEX))
                    ctx.DamageWith(mon, 100, false, false, false);
                return true;
            }
        }
    }
}
