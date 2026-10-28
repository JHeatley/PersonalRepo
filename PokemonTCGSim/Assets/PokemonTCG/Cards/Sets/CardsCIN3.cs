using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crimson Invasion (CIN, v86), part 3: Fighting, Darkness, Metal, Fairy and Dragon (CIN 50-77). Buzzwole-GX (30C CC24),
    /// Hydreigon (DRM 33), Guzzlord-GX and Kartana-GX (Hidden Fates Shiny Vault) were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsCIN3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("CIN-50", new CoinsMon("Three-Step Strike", 3));
            r.RegisterByKey("CIN-51", new LuchaFightMon());
            r.RegisterByKey("CIN-52", new ParalyzeFlipMon("Leer"));
            r.RegisterByKey("CIN-53", new RegirockCIN());
            r.RegisterByKey("CIN-54", new ComboMon(new ConditionAttackMon("Eerie Fluid", SpecialCondition.Confused), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("CIN-56", new BewearCIN());
            // Darkness
            r.RegisterByKey("CIN-59", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new CardsFLF1.NoResistanceMon("Puncturing Fangs")));
            r.RegisterByKey("CIN-60", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("CIN-61", new CoinsMon("Double Hit", 2));
            // Metal
            r.RegisterByKey("CIN-64", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new CardsASC.FlipDiscardDefendingEnergyMon("Crunch")));
            r.RegisterByKey("CIN-67", new ComboMon(new CardsASR1.PlusMon("Revenge Cannon", 10, c => c.Me.Bench.Sum(m => m.DamageCounters)), new CardsFLF1.NoResistanceMon("Buster Swing")));
            r.RegisterByKey("CIN-68", new RegisteelCIN());
            r.RegisterByKey("CIN-69", new ComboMon(new CoinsMon("Fury Attack", 3), new RecoilMon("Iron Tackle", 30)));
            // Fairy
            r.RegisterByKey("CIN-71", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("CIN-72", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep),
                new CardsFST2.ConditionBonusMon("Punishing Slap", 60, c => c.OpponentPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(c, m, e, EnergyType.Darkness))).Count > 0)));
            r.RegisterByKey("CIN-73", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 1), new LockAfterAttackMon("Bright Horns", true)));
            // Dragon
            r.RegisterByKey("CIN-74", new AlolanExeggutorGX());
            r.RegisterByKey("CIN-76", new CardsDRI1.WhirlwindMon("Noble Roar"));
            r.RegisterByKey("CIN-77", new ComboMon(new CardsFST2.ConditionBonusMon("War Cry", 90, c => c.MyPokemon().Count < c.OpponentPokemon().Count), new ClangingScalesMon()));
        }

        // ================================================================== Fighting

        /// <summary>Lucha Fight: during the opponent's next turn, the Defending Pokémon's attacks do 30 more damage (before Weakness and Resistance).</summary>
        private sealed class LuchaFightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lucha Fight") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 30 more damage during its next turn (Lucha Fight)", ctx.Player, defending.Uid, ctx.Turn + 1, -30));
                return true;
            }
        }

        /// <summary>Rock Peak Growl: your Registeel's attacks do 10 more damage to the opponent's Active Pokémon (before Weakness and Resistance). Tough Swing: not affected by Resistance.</summary>
        private sealed class RegirockCIN : Mon
        {
            private static readonly Mon Swing = new CardsFLF1.NoResistanceMon("Tough Swing");

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || attacker.Def.Name != "Registeel") return damage;
                return info.ToOpponentsActive && damage > 0 ? damage + 10 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swing.HandleAttack(ctx, atk); }
        }

        /// <summary>Fluffy: this Pokémon takes 30 less damage from attacks from the opponent's non-Fire Pokémon (after Weakness and Resistance). Cross-Cut: 60 more against an Evolution Pokémon.</summary>
        private sealed class BewearCIN : Mon
        {
            private static readonly Mon Cut = new CardsPAL1.EvolutionBonusMon("Cross-Cut", 60);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || info.Target != me || attacker == null || attacker.Owner == me.Owner) return damage;
                return src.Engine.IsType(attacker, EnergyType.Fire) ? damage : damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cut.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Turbo Arm: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon. Iron Hand: heal 30 if Regice is on your Bench.</summary>
        private sealed class RegisteelCIN : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Turbo Arm")
                {
                    ctx.DamageDefending(atk.Damage);
                    var energy = ctx.BasicEnergyInDiscard();
                    if (energy.Count > 0 && ctx.Me.Bench.Count > 0)
                        ctx.AttachCardsAnyWay(energy, 1, (card, mon) => ctx.Me.Bench.Contains(mon), "Turbo Arm: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon", false);
                    return true;
                }
                if (atk.Name != "Iron Hand") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Any(m => m.Def.Name == "Regice") && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Tropical Head: 20 damage times the Energy attached to this Pokémon to 1 of the opponent's Pokémon (no printed damage). Dragon
        /// Hammer: Confused. Tower-Go-Round-GX: move any amount of Energy between your Pokémon in any way you like.
        /// </summary>
        private sealed class AlolanExeggutorGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tropical Head")
                {
                    int amount = 20 * ctx.EnergyCount(ctx.Pokemon);
                    if (amount > 0) ctx.DamageAnyOpponent(amount);
                    else ctx.Log(ctx.Pokemon.Name + "'s Tropical Head did no damage.");
                    return true;
                }
                if (atk.Name == "Dragon Hammer")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                if (atk.Name != "Tower-Go-Round-GX") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.MyPokemon().Count < 2) return true;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy) holders[e] = mon;
                if (holders.Count == 0) return true;
                foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Tower-Go-Round-GX: choose any amount of Energy to move"))
                {
                    var origin = holders[e];
                    if (!ctx.InPlay(origin) || !origin.Energy.Contains(e)) continue;
                    var dest = ctx.ChoosePokemon(ctx.MyPokemon(m => m != origin), "Tower-Go-Round-GX: move " + e.Def.Name + " from " + origin.Name + " to which Pokémon?");
                    if (dest != null) ctx.Engine.MoveEnergy(origin, dest, e);
                }
                return true;
            }
        }

        /// <summary>Clanging Scales: during the opponent's next turn, this Pokémon takes 30 more damage from attacks (after Weakness and Resistance).</summary>
        private sealed class ClangingScalesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clanging Scales") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new DamageTakenEffect(me.Name + " takes 30 more damage during the opponent's next turn (Clanging Scales)", ctx.Player, me.Uid, ctx.Turn + 1, 30));
                return true;
            }
        }
    }
}
