using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// SM Black Star Promos (SMP, v91), part 1: SM01-SM79. Most of the promos are prints of set cards already in the pool and keep
    /// their scripts (see build_smp.py); this Tapu Lele is a Psychic print of Ultra Prism's Fairy one and shares its script.
    /// </summary>
    public static class CardsSMP1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SMP-1", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("SMP-3", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("SMP-4", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("SMP-5", new SnorlaxGX());
            r.RegisterByKey("SMP-7", new RecoilMon("Nosedive", 20));
            r.RegisterByKey("SMP-8", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SMP-25", new LurantisSMP());
            r.RegisterByKey("SMP-29", new ComboMon(new CardsBKP2.CountersOnActiveMon("Haunt", 1), new CardsEVS1.AstonishMon("Astonish")));
            r.RegisterByKey("SMP-30", new CardsCEC3.SpreadAmountMon("Flying Flip", 20));
            r.RegisterByKey("SMP-34", new ComboMon(new NoRetreatMon("Bear Hug"), new CoinsMon("Double Impact", 2), new BigThrowMon()));
            r.RegisterByKey("SMP-40", new CardsSP1.FlipNoDamageMon("Iron Defense"));
            r.RegisterByKey("SMP-43", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("SMP-44", new ParalyzeFlipMon("Nuzzle"));
            CardsUPR1.Same(r, "SMP-45", "UPR-94"); // Psywave, Magical Swap
            r.RegisterByKey("SMP-51", new FlipSearchMon("Nasty Plot", d => true, "a card"));
            r.RegisterByKey("SMP-56", new ComboMon(new CardsDAA1.GustAttackMon("Side Eye", true),
                new CardsFST2.ConditionBonusMon("Jumping Side Kick", 90, c => c.Defending != null && c.Defending.Has(SpecialCondition.Confused)), new QueensCommandMon()));
            r.RegisterByKey("SMP-61", new ComboMon(new CardsTWM1.HealSelfMon("Horn Leech", 30), new CardsFST2.ConditionBonusMon("Calm Strike", 60, c => c.Me.GxUsed)));
            r.RegisterByKey("SMP-64", new ComboMon(new SearchAttackMon("Gear Scan", CardsLOT4.IsItemCard, 1), new FlipBonusMon("X-Scissor", 30)));
            r.RegisterByKey("SMP-65", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("SMP-70", new ShiningHoOh());
            r.RegisterByKey("SMP-77", new MewtwoSMP());
            r.RegisterByKey("SMP-79", new RelicanthTEF()); // Time Recall works like Relicanth's Memory Dive
        }

        // ================================================================== Shared helpers

        /// <summary>"Flip a coin. If heads, search your deck for [a card] and put it into your hand" (Nasty Plot, Find a Friend).</summary>
        internal sealed class FlipSearchMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public FlipSearchMon(string attack, Func<CardDef, bool> filter, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(filter, 1, attack + ": search your deck for " + what);
                return true;
            }
        }

        /// <summary>"The opponent discards N cards from their hand" (they choose which).</summary>
        internal static void OpponentDiscardsFromHand(EffectContext ctx, int count, string source)
        {
            var hand = ctx.Opp.Hand.ToList();
            if (hand.Count == 0) return;
            int n = Math.Min(count, hand.Count);
            var picks = n == hand.Count ? hand : ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, source + ": discard " + n + " cards from your hand", ctx.Source);
            foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
        }

        // ================================================================== Pokémon

        /// <summary>
        /// Collapse and Pulverizing Pancake-GX put this Pokémon to sleep. Thunderous Snore can be used while it's Asleep and does
        /// nothing unless it is; the other attacks still can't be used while it's Asleep.
        /// </summary>
        private sealed class SnorlaxGX : Mon
        {
            // Thunderous Snore can be used while Asleep (not while Paralyzed): the engine's hook for that (Dream Waltz).
            public override bool UsableWhileAsleep(AttackDef attack) { return attack != null && attack.Name == "Thunderous Snore"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Thunderous Snore")
                {
                    if (!me.Has(SpecialCondition.Asleep)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (atk.Name != "Collapse" && atk.Name != "Pulverizing Pancake-GX") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Sunny Day: your Grass and Fire Pokémon's attacks do 20 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class LurantisSMP : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return src.Engine.IsType(attacker, EnergyType.Grass) || src.Engine.IsType(attacker, EnergyType.Fire) ? damage + 20 : damage;
            }
        }

        /// <summary>Big Throw-GX: discard the opponent's Active Pokémon and all cards attached to it (not a Knock Out: no Prize cards).</summary>
        private sealed class BigThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Throw-GX") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.DiscardPokemonFromPlay(defending);
                return true;
            }
        }

        /// <summary>Queen's Command-GX: the opponent discards 4 cards from their hand.</summary>
        private sealed class QueensCommandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Queen's Command-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                OpponentDiscardsFromHand(ctx, 4, "Queen's Command-GX");
                return true;
            }
        }

        /// <summary>
        /// Golden Wing: if this Pokémon is your Active Pokémon and is Knocked Out by damage from an opponent's attack, move up to 2
        /// basic Energy cards from it to your Benched Pokémon in any way you like. Fire Blast: discard an Energy from this Pokémon.
        /// </summary>
        private sealed class ShiningHoOh : Mon
        {
            private static readonly Mon Blast = new CardsTWM1.SelfDiscardMon("Fire Blast", 1);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var mon = src.Pokemon;
                if (mon == null || !byOpponentsAttack || !wasActive) return;
                var bench = src.Me.Bench.Where(m => m != mon && !src.Engine.IsKnockedOut(m)).ToList();
                var energy = mon.Energy.Where(x => x.Def.IsBasicEnergy).ToList();
                if (bench.Count == 0 || energy.Count == 0) return;
                var picks = src.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Golden Wing: move up to 2 basic Energy cards from " + mon.Name + " to your Benched Pokémon");
                foreach (var en in picks)
                {
                    var dest = bench.Count == 1 ? bench[0] : src.ChoosePokemon(bench, "Golden Wing: move " + en.Def.Name + " to which Benched Pokémon?");
                    if (dest != null) src.Engine.MoveEnergy(mon, dest, en);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Pressure: while this Pokémon is your Active Pokémon, the opponent's Active Pokémon's attacks do 20 less damage (before Weakness and Resistance).</summary>
        private sealed class MewtwoSMP : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (info.Target == null || info.Target.Owner != src.Player || !src.State.IsActive(info.Attacker)) return damage;
                return damage - 20;
            }
        }
    }
}
