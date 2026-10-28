using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Paldea Evolved (PAL) 1-99: the Pokémon that weren't already in the pool (all G-mark, Expanded only). Reprints already in
    /// the pool (the Sprigatito, Fuecoco and Quaxly lines from the promos, Meowscarada ex, Skeledirge ex, Quaquaval ex, the
    /// Classic Collection Magikarp...) are alternate prints. Pokémon whose attacks only do their printed damage need no script:
    /// Sprigatito (13), Tarountula (17), Rellor, Charcadet, Frigibax, Voltorb, Electrode, Pawmi, Tadbulb (78), Sandygast.
    /// </summary>
    public static class CardsPAL1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PAL-6", new CardsFLF1.NoResistanceMon("Smashing Horn"));
            r.RegisterByKey("PAL-7", new ComboMon(new CardsTWM1.HealSelfMon("Leaf Drain", 20), new TropicBreezeMon()));
            r.RegisterByKey("PAL-8", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("PAL-9", new ComboMon(new CoinsMon("Double Stab", 2), new OrderARaidMon()));
            r.RegisterByKey("PAL-16", new NoRetreatMon("Bind Down"));
            r.RegisterByKey("PAL-18", new EntanglingTrapMon());
            r.RegisterByKey("PAL-19", new DodgeMon("Slight Splash", false));
            r.RegisterByKey("PAL-20", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("PAL-21", new LokixPAL());
            r.RegisterByKey("PAL-22", new RideTheWindMon());
            r.RegisterByKey("PAL-23", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("PAL-24", new ComboMon(new CardsTWM1.HealSelfMon("Absorb Life", 30), new CardsBST1.SourSpitMon("Dead Wood Detention")));
            r.RegisterByKey("PAL-29", new RecoilMon("Heat Dive", 10));
            r.RegisterByKey("PAL-30", new ComboMon(new NoRetreatMon("Clutch"), new CardsASC.SelfDamagedBonusMon("Fiery Breeze", 90)));
            r.RegisterByKey("PAL-31", new CardsPAF1.DiscardStadiumMon("Blazing Destruction"));
            r.RegisterByKey("PAL-32", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new Cards30CC.BenchHitMon("Overrun", 20)));
            r.RegisterByKey("PAL-35", new FlipHealSelfMon("Spacing Out", 30));
            r.RegisterByKey("PAL-43", new ComboMon(new PrizeBehindBonusMon("Revengeful Storm", 100), new RecoilMon("Berserker Tackle", 50)));
            r.RegisterByKey("PAL-44", new CardsTWM1.HealSelfMon("Bubble Drain", 20));
            r.RegisterByKey("PAL-45", new ComboMon(new CardsTWM1.HealSelfMon("Bubble Drain", 30), new CoinsMon("Slam", 2)));
            r.RegisterByKey("PAL-46", new DrawAttackMon("Double Draw", 2));
            r.RegisterByKey("PAL-47", new ComboMon(new SearchAttackMon("Matching", d => d.IsSupporter, 2), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("PAL-48", new FrigidBlockMon());
            r.RegisterByKey("PAL-49", new CardsMEG.SandAttackMon("Apply Gel"));
            r.RegisterByKey("PAL-53", new CardsMEW1.SpreadBenchMon("Avalanche", 10));
            r.RegisterByKey("PAL-54", new ComboMon(new RestMon(), new RecoilMon("Double-Edge", 10)));
            r.RegisterByKey("PAL-55", new SpecialHornMon());
            r.RegisterByKey("PAL-63", new CardsG2.ElectroBeamMon("Dynamic Bolt"));
            r.RegisterByKey("PAL-64", new ComboMon(new CardsMEG.ChargeSelfMon("Electrocharge", EnergyType.Lightning, 2), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("PAL-65", new MagneticChargeMon());
            r.RegisterByKey("PAL-68", new ShinxPAL());
            r.RegisterByKey("PAL-72", new CardsDRI2.CrunchMon("Needle Crush"));
            r.RegisterByKey("PAL-73", new PincurchinPAL());
            r.RegisterByKey("PAL-75", new Cards30CC.BenchHitMon("Electrobullet", 30));
            r.RegisterByKey("PAL-76", new Cards30CC.BenchHitMon("Electric Fist", 60));
            r.RegisterByKey("PAL-77", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("PAL-79", new ComboMon(new CardsMEG.SnipeMon("Jumping Press", 50), new ParalyzingBallMon()));
            r.RegisterByKey("PAL-81", new DevastatingWindMon());
            r.RegisterByKey("PAL-82", new LockAfterAttackMon("Jet Wing", false));
            r.RegisterByKey("PAL-83", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("PAL-85", new TailFishingMon());
            r.RegisterByKey("PAL-86", new ComboMon(new ConditionAttackMon("Profound Knowledge", SpecialCondition.Confused), new WiseHeadbuttMon("Wise Headbutt")));
            r.RegisterByKey("PAL-87", new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep));
            r.RegisterByKey("PAL-88", new MismagiusPAL());
            r.RegisterByKey("PAL-90", new ConditionAttackMon("Eerie Wave", SpecialCondition.Confused));
            r.RegisterByKey("PAL-91", new ConditionAttackMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("PAL-92", new GothitellePAL());
            r.RegisterByKey("PAL-93", new ComboMon(new TailSwapMon(), new CardsTWM1.SelfDiscardMon("Wondrous Shot", 1)));
            r.RegisterByKey("PAL-94", new CardsPOR.MemoryLockMon("Plotter's Command"));
            r.RegisterByKey("PAL-96", new StadiumBonusMon("Earthen Power", 80));
        }

        // ================================================================== Shared helpers (also used by CardsPAL2)

        /// <summary>"Flip a coin. If heads, heal N damage from this Pokémon" (Spacing Out).</summary>
        public sealed class FlipHealSelfMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FlipHealSelfMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, amount);
                return true;
            }
        }

        /// <summary>"If you have more Prize cards remaining than your opponent, this attack does N more damage" (Revengeful Storm).</summary>
        public sealed class PrizeBehindBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public PrizeBehindBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If you have a Stadium in play, this attack does N more damage" (the Stadium is yours: you played it).</summary>
        public sealed class StadiumBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public StadiumBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (HaveStadium(ctx) ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If you have a Stadium in play": a Stadium you played is in play.</summary>
        public static bool HaveStadium(EffectContext ctx)
        {
            return ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
        }

        /// <summary>"If your opponent's Active Pokémon is an Evolution Pokémon, this attack does N more damage" (Cross-Cut).</summary>
        public sealed class EvolutionBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public EvolutionBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Def.IsEvolution ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Move all Energy from this Pokémon to 1 of your Benched Pokémon" (Tropic Breeze). With no Bench the Energy stays (Compendium, 2023-06-08).</summary>
        private sealed class TropicBreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tropic Breeze") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var bench = ctx.Me.Bench.Where(m => m != me).ToList();
                if (!ctx.InPlay(me) || bench.Count == 0 || me.Energy.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Tropic Breeze: move all Energy from " + me.Name + " to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var card in me.Energy.ToList()) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Order a Raid: shuffle 1 of your Benched Combee and its cards into your deck; if you can't, the attack does nothing.</summary>
        private sealed class OrderARaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Order a Raid") return false;
                var combee = ctx.Me.Bench.Where(m => m.Def.Name == "Combee").ToList();
                if (combee.Count == 0) return Nothing(ctx);
                var pick = ctx.ChoosePokemon(combee, "Order a Raid: shuffle which Benched Combee into your deck?");
                if (pick == null) return Nothing(ctx);
                ctx.Engine.ShufflePokemonIntoDeck(pick);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Entangling Trap: each player's Active Pokémon and all attached cards are shuffled into their deck. You promote your new
        /// Active Pokémon first (the engine does that right after the attack), then the opponent.
        /// </summary>
        private sealed class EntanglingTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entangling Trap") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (ctx.PokemonIsActive) ctx.Engine.ShufflePokemonIntoDeck(me);
                if (defending != null && ctx.InPlay(defending))
                {
                    if (ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                    else ctx.Log("The effect of Entangling Trap on " + defending.Name + " was prevented.");
                }
                return true;
            }
        }

        /// <summary>Assaulting Kick: 100 more damage if this Pokémon evolved from Nymble during this turn.</summary>
        private sealed class LokixPAL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assaulting Kick") return false;
                var me = ctx.Pokemon;
                bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Nymble";
                ctx.DamageDefending(atk.Damage + (fresh ? 100 : 0));
                return true;
            }
        }

        /// <summary>Ride the Wind: flip a coin; on heads switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class RideTheWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ride the Wind") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Frigid Block: discard all Energy from this Pokémon; during the opponent's next turn it takes 100 less damage from attacks.</summary>
        private sealed class FrigidBlockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frigid Block") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                ctx.SelfTakesLessNextTurn(100);
                return true;
            }
        }

        /// <summary>Rest: this Pokémon is now Asleep; heal 60 damage from it.</summary>
        internal sealed class RestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                if (ctx.PokemonIsActive) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                if (ctx.InPlay(me)) ctx.Heal(me, 60);
                return true;
            }
        }

        /// <summary>Special Horn: 140 more damage if this Pokémon has any Special Energy attached.</summary>
        private sealed class SpecialHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Horn") return false;
                bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? 140 : 0));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Magnetic Charge: attach up to 2 Basic [L] Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class MagneticChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Charge") return false;
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Magnetic Charge: choose up to 2 Basic Lightning Energy to attach");
                if (picks.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Magnetic Charge: attach them to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Big Roar: once during your turn, if Shinx is Active, you may switch out the opponent's Active Pokémon (they choose the new one).</summary>
        private sealed class ShinxPAL : Mon
        {
            protected override string Activated { get { return "Big Roar"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        /// <summary>Stun Needle: flip, heads Paralyzed. Follow-Up Kerzap: only if this Pokémon used Stun Needle during your last turn.</summary>
        private sealed class PincurchinPAL : Mon
        {
            private static readonly Mon StunNeedle = new ParalyzeFlipMon("Stun Needle");

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Follow-Up Kerzap") return true;
                var me = ctx.Pokemon;
                return me.LastAttackName == "Stun Needle" && me.LastAttackTurn == ctx.MyLastTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return StunNeedle.HandleAttack(ctx, atk); }
        }

        /// <summary>Paralyzing Ball: you may discard 2 [L] Energy from this Pokémon to make the opponent's Active Pokémon Paralyzed.</summary>
        private sealed class ParalyzingBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paralyzing Ball") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || !TefPokemon.StillDefending(ctx, defending)) return true;
                var lightning = me.Energy.Where(e => CardsOBF1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                if (lightning.Count < 2 || !ctx.AskYesNo("Paralyzing Ball: discard 2 Lightning Energy to Paralyze " + defending.Name + "?")) return true;
                ctx.DiscardEnergyFrom(me, 2, "Paralyzing Ball: discard 2 Lightning Energy", e => lightning.Contains(e));
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Devastating Wind: the opponent shuffles their hand into their deck and draws 4 cards.</summary>
        internal sealed class DevastatingWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devastating Wind") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Tail-Fishing: flip a coin; heads, search your deck for a card; tails, discard a card from your hand.</summary>
        private sealed class TailFishingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail-Fishing") return false;
                if (ctx.Flip())
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Tail-Fishing: put a card from your deck into your hand", false, 1);
                    return true;
                }
                if (ctx.Me.Hand.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Tail-Fishing: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                return true;
            }
        }

        /// <summary>Magical Flick: when you evolve into it from your hand, you may move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class MismagiusPAL : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                bool ready = defending != null && defending.Energy.Count > 0 && ctx.Opp.Bench.Count > 0;
                if (!TriggerOnce(ctx, "Magical Flick", ready, "Magical Flick: move an Energy from your opponent's Active Pokémon to 1 of their Benched Pokémon?")) return;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Magical Flick on " + defending.Name + " was prevented.");
                    return;
                }
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Magical Flick: move which Energy?")[0];
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Magical Flick: move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(defending, target, card);
            }
        }

        /// <summary>Read the Stars: once during your turn, look at the top 2 cards of the opponent's deck; put 1 back on top and the other on the bottom. Psych Out: random discard.</summary>
        private sealed class GothitellePAL : Mon
        {
            private static readonly Mon PsychOut = new CardsASC.RandomDiscardMon("Psych Out");

            protected override string Activated { get { return "Read the Stars"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var deck = ctx.Opp.Deck;
                var top = deck.Take(Math.Min(2, deck.Count)).ToList();
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + (top.Count == 1 ? " card" : " cards") + " of your opponent's deck", ctx.Player);
                if (top.Count < 2)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of the other player's deck and put it back.");
                    return;
                }
                var keep = ctx.ChooseCards(top, 1, 1, "Read the Stars: choose the card to put back on top (the other goes on the bottom)")[0];
                var other = top[0] == keep ? top[1] : top[0];
                deck.Remove(other);
                deck.Add(other);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 of the top 2 cards of the other player's deck back and the other on the bottom.");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PsychOut.HandleAttack(ctx, atk); }
        }

        /// <summary>Tail Swap (Tera): move all damage counters from 1 of your Benched Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class TailSwapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Swap") return false;
                var defending = ctx.Defending;
                var hurt = ctx.Me.Bench.Where(m => m.DamageCounters > 0).ToList();
                if (defending == null || hurt.Count == 0) return true;
                var from = ctx.ChoosePokemon(hurt, "Tail Swap: move all damage counters from which Benched Pokémon?");
                if (from != null) ctx.Engine.MoveDamageCounters(from, defending, from.DamageCounters, "Tail Swap");
                return true;
            }
        }
    }
}
