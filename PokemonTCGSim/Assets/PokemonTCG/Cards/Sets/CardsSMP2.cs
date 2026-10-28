using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>SM Black Star Promos (SMP, v91), part 2: SM82-SM145 (the promos new to the pool).</summary>
    public static class CardsSMP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SMP-82", new ComboMon(new CardsFST2.ConditionBonusMon("Argent Wing", 60, c => c.Defending != null && c.Engine.HasAbility(c.Defending)),
                new CardsTWM1.SelfDiscardMon("Aero Force", 1)));
            r.RegisterByKey("SMP-86", new Cards30CC.BenchHitMon("Spark", 10));
            r.RegisterByKey("SMP-87", new LatiasSMP());
            r.RegisterByKey("SMP-89", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Rip Claw"), new CardsSUM1.DiscardTypeFromSelfMon("Doom Crush", EnergyType.Darkness, 1)));
            r.RegisterByKey("SMP-92", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep),
                new CardsFST2.ConditionBonusMon("Shining Currents", 60, c => c.MyPokemon().Any(m => m.HealedOnTurn == c.Turn && c.Engine.IsType(m, EnergyType.Water)))));
            r.RegisterByKey("SMP-93", new ComboMon(new ShadowyEchoesMon(), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("SMP-103", new CardsSUM1.DiscardTypeFromSelfMon("Lunar Javelin", EnergyType.Psychic, 2));
            r.RegisterByKey("SMP-104", new SolgaleoGXSMP());
            r.RegisterByKey("SMP-105", new FlipBonusMon("Stone Edge", 30));
            r.RegisterByKey("SMP-106", new ComboMon(new CardsASR1.PlusMon("Gulf Stream", 20, c => c.Me.Prizes.Count == 6 ? c.Pokemon.DamageCounters : 0),
                new CardsTWM1.SelfDiscardMon("Sword of Dawn", 2)));
            r.RegisterByKey("SMP-107", new ComboMon(new DuskShotMon(), new CardsFST2.ConditionBonusMon("Rusty Claws", 100, c => c.Opp.Prizes.Count == 1)));
            // Ash's Pikachu
            r.RegisterByKey("SMP-108", new ComboMon(new SearchAttackMon("I Choose You!", d => d.IsPokemon, 1), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("SMP-109", new ComboMon(new DodgeMon("Agility", false), new CardsFST1.FlipRecoilMon("Thunder", 20)));
            r.RegisterByKey("SMP-110", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"), new CardsFST1.FlipRecoilMon("Thunder", 20)));
            r.RegisterByKey("SMP-111", new ComboMon(new FlipBonusMon("Quick Attack", 10), new RecoilMon("Volt Tackle", 10)));
            r.RegisterByKey("SMP-112", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("SMP-113", new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"));
            r.RegisterByKey("SMP-114", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Iron Tail"), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("SMP-119", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("SMP-121", new ComboMon(new CardsFST1.FlipRecoilMon("Thunder", 50), new ThunderousRainMon()));
            r.RegisterByKey("SMP-122", new ComboMon(new CardsTWM1.HealSelfMon("Cell Storm", 30), new CardsFST1.RevealCountMon("Liberation-GX", d => d.IsEnergy, "Energy")));
            r.RegisterByKey("SMP-127", new FlipBonusMon("Tumbling Attack", 40));
            r.RegisterByKey("SMP-128", new CardsTWM1.HealSelfMon("Spiral Drain", 30));
            r.RegisterByKey("SMP-133", new ComboMon(new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1), new CoinsMon("Thundering Hurricane-GX", 4)));
            r.RegisterByKey("SMP-134", new ComboMon(new CardsMEG.FlipUntilTailsBonusMon("Wild Fury", 30), new DiscardDefendingEnergyAllMon("Destructive Cyclone-GX")));
            r.RegisterByKey("SMP-135", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new MistBallMon()));
            r.RegisterByKey("SMP-136", new ComboMon(new EnergyExtractMon(), new CardsG2.ElectroBeamMon("Luster Purge")));
            r.RegisterByKey("SMP-138", new ComboMon(new BulletUppercutMon(), new CardsWHT.FlipMoreMon("Swift Bolt Strike", 2, 60), new CardsPAF2.IgnoreDefenderEffectsMon("Rampage Bolt-GX")));
            r.RegisterByKey("SMP-143", new TailsNothingMon("Sky Attack"));
            r.RegisterByKey("SMP-144", new CardsDAA1.FlipCantAttackMon("Sheer Cold"));
            r.RegisterByKey("SMP-145", new ParalyzeFlipMon("Thunder Shock"));
        }

        // ================================================================== Shared helpers

        /// <summary>"Discard all Energy from your opponent's Active Pokémon" (Destructive Cyclone-GX).</summary>
        internal sealed class DiscardDefendingEnergyAllMon : Mon
        {
            private readonly string attack;

            public DiscardDefendingEnergyAllMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    foreach (var en in defending.Energy.ToList()) ctx.Engine.DiscardEnergy(defending, en);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Flight Support: your Latios in play have no Retreat Cost. Misty Gale: heal 30 damage from each of your Benched Pokémon.</summary>
        private sealed class LatiasSMP : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && mon.Def.Name == "Latios" ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Misty Gale") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 30);
                return true;
            }
        }

        /// <summary>Shadowy Echoes: put a Basic Pokémon from each player's discard pile onto its owner's Bench (you choose both).</summary>
        private sealed class ShadowyEchoesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadowy Echoes") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    if (ctx.Engine.BenchSpace(player) <= 0) continue;
                    var basics = p.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(player, c.Def)).ToList();
                    if (basics.Count == 0) continue;
                    var card = basics.Count == 1 ? basics[0]
                        : ctx.ChooseCards(basics, 1, 1, "Shadowy Echoes: put which Basic Pokémon onto " + (player == ctx.Player ? "your" : "your opponent's") + " Bench?")[0];
                    if (p.Discard.Remove(card)) ctx.Engine.PutOntoBench(player, card, false);
                }
                return true;
            }
        }

        /// <summary>
        /// Shining Mane: your Pokémon in play have no Weakness. Turbo Strike: attach 2 basic Energy cards from your discard pile to 1
        /// of your Benched Pokémon. Prominence-GX: heal all damage from all of your Pokémon.
        /// </summary>
        private sealed class SolgaleoGXSMP : Mon
        {
            private static readonly Mon Prominence = new CardsGRI4.HealAllMineMon("Prominence-GX");

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Strike") return Prominence.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                var pile = ctx.BasicEnergyInDiscard();
                if (bench.Count == 0 || pile.Count == 0) return true;
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Turbo Strike: attach 2 basic Energy cards to which of your Benched Pokémon?");
                if (target != null) ctx.AttachCardsAnyWay(pile, 2, (card, mon) => mon == target, "Turbo Strike: attach basic Energy cards to " + target.Name, false);
                return true;
            }
        }

        /// <summary>Dusk Shot: 60 damage to 1 of the opponent's Pokémon-GX or Pokémon-EX, not affected by Weakness or Resistance.</summary>
        private sealed class DuskShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dusk Shot") return false;
                var targets = ctx.OpponentPokemon(m => m.Def.IsGXorEX);
                if (targets.Count == 0) return Nothing(ctx);
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Dusk Shot: choose 1 of your opponent's Pokémon-GX or Pokémon-EX");
                if (target != null) ctx.DamageWith(target, 60, false, false, false);
                return true;
            }
        }

        /// <summary>Thunderous Rain-GX: 100 damage to each of the opponent's Pokémon with any Energy attached.</summary>
        private sealed class ThunderousRainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderous Rain-GX") return false;
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                if (targets.Count == 0) return Nothing(ctx);
                foreach (var mon in targets) ctx.DamageTarget(mon, 100);
                return true;
            }
        }

        /// <summary>Mist Ball: flip a coin; tails, discard a Fire Energy and a Psychic Energy from this Pokémon.</summary>
        private sealed class MistBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mist Ball") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads || !ctx.InPlay(me)) return true;
                ctx.DiscardEnergyFrom(me, 1, "Mist Ball: discard a Fire Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, EnergyType.Fire));
                ctx.DiscardEnergyFrom(me, 1, "Mist Ball: discard a Psychic Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, EnergyType.Psychic));
                return true;
            }
        }

        /// <summary>Energy Extract: search your deck for a basic Energy card and attach it to this Pokémon.</summary>
        internal sealed class EnergyExtractMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Extract") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Extract: choose a basic Energy card to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Bullet Uppercut: 60 more against a Pokémon-GX or Pokémon-EX; the damage isn't affected by Weakness.</summary>
        private sealed class BulletUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bullet Uppercut") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                ctx.DamageWith(defending, atk.Damage + (defending.Def.IsGXorEX ? 60 : 0), false, true, false);
                return true;
            }
        }
    }
}
