using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sun &amp; Moon (SUM, v90), part 3: Psychic and Fighting (SUM 54-75). Espeon-GX (Hidden Fates Shiny Vault), Cosmoem and
    /// Lunala-GX (Ultra Prism) were already in the pool and keep their scripts.
    /// </summary>
    public static class CardsSUM3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Psychic
            r.RegisterByKey("SUM-54", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("SUM-55", new ComboMon(new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned), new CardsWHT.FlipMoreMon("Acrobatics", 2, 20)));
            r.RegisterByKey("SUM-56", new ComboMon(new CardsDAA2.TriplePoisonMon("Triple Poison", 3), new CardsFST2.ConditionBonusMon("Surprise Strike", 60, CardsUNM1.CameInThisTurn)));
            r.RegisterByKey("SUM-57", new CardsPAR2.FlipConditionMon("Super Poison Breath", SpecialCondition.Poisoned));
            r.RegisterByKey("SUM-58", new AlolanMukSUM());
            r.RegisterByKey("SUM-59", new CardsASR1.TimesMon("Psychic Boom", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending)));
            r.RegisterByKey("SUM-60", new ComboMon(new CardsASR1.PlusMon("Meditate", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters),
                new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("SUM-62", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("SUM-63", new ToxapexSUM());
            r.RegisterByKey("SUM-64", new DrawAttackMon("Dust Gathering", 1));
            // Fighting
            r.RegisterByKey("SUM-67", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("SUM-68", new CardsDRI1.WhirlwindMon("Push Out"));
            r.RegisterByKey("SUM-69", new CardsFST2.ConditionBonusMon("Smack Down", 50, c => c.Defending != null && c.Defending.Def.Resistance == EnergyType.Fighting));
            r.RegisterByKey("SUM-70", new CardsDAA2.RockHurlMon());
            r.RegisterByKey("SUM-71", new ComboMon(new RockArtilleryMon(), new CardsFLF1.NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("SUM-73", new ComboMon(new BenchOnlyMon("Fling", 30), new CardsASR1.PlusMon("Team Play", 30, c => c.Me.Bench.Count(m => m.Def.Name == "Passimian"))));
            r.RegisterByKey("SUM-74", new ComboMon(new ShoreUpMon(), new NoRetreatMon("Sand Tomb")));
            r.RegisterByKey("SUM-75", new CardsFST1.TakeLessMon(20, new CardsDAA1.DrainMon("Absorb Vitality")));
        }

        // ================================================================== Psychic

        /// <summary>
        /// Power of Alchemy: each Basic Pokémon in play, in each player's hand and in each player's discard pile has no Abilities
        /// (Alolan Muk itself is a Stage 1). Crunch: flip, heads discard an Energy from the opponent's Active Pokémon.
        /// </summary>
        private sealed class AlolanMukSUM : Mon
        {
            private static readonly Mon Crunch = new CardsASC.FlipDiscardDefendingEnergyMon("Crunch");

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && mon != null && mon.Def.IsBasicPokemon;
            }

            public override bool BlocksOpponentsAbilitiesOutOfPlay(EffectContext src, CardDef card) { return src.Pokemon != null && card != null && card.IsBasicPokemon; }

            public override bool BlocksOwnAbilitiesOutOfPlay(EffectContext src, CardDef card) { return src.Pokemon != null && card != null && card.IsBasicPokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Toxic Spikes: whenever the opponent's Active Pokémon retreats, their new Active Pokémon is now Poisoned. Venoshock: 50 more if Poisoned.</summary>
        private sealed class ToxapexSUM : Mon
        {
            private static readonly Mon Venoshock = new CardsBLK.VenoshockMon(50);

            public override void OnOpponentPokemonMovedToBench(EffectContext src, int player, PokemonInPlay movedToBench)
            {
                if (src.Pokemon == null || player == src.Player || movedToBench == null || movedToBench.RetreatedOnTurn != src.Turn) return;
                var active = src.Opp.Active;
                if (active == null) return;
                if (src.ApplyCondition(active, SpecialCondition.Poisoned))
                    src.Log("Toxic Spikes (" + src.Pokemon.Name + ") poisoned " + active.Name + ".");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Venoshock.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Rock Artillery: discard any amount of Fighting Energy from your Pokémon; 50 damage for each card discarded.</summary>
        private sealed class RockArtilleryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Artillery") return false;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var en in mon.Energy)
                        if (CardsHIF1.Provides(ctx, mon, en, EnergyType.Fighting)) holders[en] = mon;
                var cards = holders.Keys.ToList();
                var picks = cards.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(cards, 0, cards.Count, "Rock Artillery: discard any amount of Fighting Energy from your Pokémon (50 damage each)");
                int n = 0;
                foreach (var en in picks)
                    if (ctx.Engine.DiscardEnergy(holders[en], en)) n++;
                if (n == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 1 of your opponent's Benched Pokémon" and nothing to the Active (Fling).</summary>
        internal sealed class BenchOnlyMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchOnlyMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.OpponentBench().Count == 0) return Nothing(ctx);
                ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>Shore Up: attach a Fighting Energy card from your discard pile to this Pokémon.</summary>
        private sealed class ShoreUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shore Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                if (pile.Count == 0 || !ctx.InPlay(me)) return true;
                ctx.AttachCardsAnyWay(pile, 1, (card, mon) => mon == me, "Shore Up: attach a Fighting Energy card to " + me.Name, false);
                return true;
            }
        }

    }
}
