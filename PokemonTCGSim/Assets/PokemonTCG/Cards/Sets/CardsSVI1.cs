using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Scarlet &amp; Violet (SVI) 1-99: the Pokémon that weren't already in the pool (all G-mark, Expanded only). Reprints already
    /// in the pool (Growlithe and Quaquaval from the promos, Miraidon...) are alternate prints. Pokémon whose attacks only do
    /// their printed damage need no script: Breloom, Skiddo, Sprigatito, Floragato, Smoliv, Houndour, Fuecoco, Crocalor,
    /// Buizel, Clauncher, Quaxly, Quaxwell, both Cetoddle, Mareep, Toxel, Wattrel (78), Flabébé.
    /// </summary>
    public static class CardsSVI1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SVI-1", new ShieldAttackMon("Guard Press", 30));
            r.RegisterByKey("SVI-2", new CardsOBF1.RetreatCostDamageMon("Superpowered Throw", 30, false));
            r.RegisterByKey("SVI-3", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("SVI-5", new CardsASC.CounterattackMon("Counterattack Quills", 3, null));
            r.RegisterByKey("SVI-6", new CardsASC.CounterattackMon("Counterattack Quills", 3, null));
            r.RegisterByKey("SVI-7", new CardsOBF1.HealBenchedMon("Fresh-Picked Fruit", 60));
            r.RegisterByKey("SVI-8", new AdaptiveEvolutionMon());
            r.RegisterByKey("SVI-9", new AdaptiveEvolutionMon());
            r.RegisterByKey("SVI-10", new MiraclePowderMon("Miracle Powder"));
            r.RegisterByKey("SVI-12", new FlipBonusMon("Rising Lunge", 30));
            r.RegisterByKey("SVI-15", new TrickCapeMon());
            r.RegisterByKey("SVI-16", new StringHaulMon());
            r.RegisterByKey("SVI-17", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("SVI-18", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("SVI-19", new SpidopsExSVI());
            r.RegisterByKey("SVI-25", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("SVI-26", new ComboMon(new EerieTentaclesMon(), new CoinsMon("Triple Smash", 3)));
            r.RegisterByKey("SVI-27", new CardsPAR2.FlipConditionMon("Slightly Spicy", SpecialCondition.Burned));
            r.RegisterByKey("SVI-29", new ComboMon(new ConditionAttackMon("Hot Bite", SpecialCondition.Burned), new Cards30C.TypeEnergyBonusMon("Super Spicy Breath", EnergyType.Fire, 90)));
            r.RegisterByKey("SVI-30", new CardsPFL.FlipUntilTailsTimesMon("Relentless Flames"));
            r.RegisterByKey("SVI-32", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Claws", 10), new CardsASC.SelfDiscardTypeMon("Bright Flame", EnergyType.Fire, 2)));
            r.RegisterByKey("SVI-34", new CardsTWM1.SelfDiscardMon("Fire Blast", 1));
            r.RegisterByKey("SVI-35", new CardsPOR.FlipPerTypedEnergyMon("Concentrated Fire", EnergyType.Fire));
            r.RegisterByKey("SVI-38", new ComboMon(new PassionateSingingMon(), new RecoilMon("Blazing Shout", 30)));
            r.RegisterByKey("SVI-39", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("SVI-44", new JumpMon());
            r.RegisterByKey("SVI-45", new CardsPFL.DamagedBonusMon("Tyrannical Tail", 180));
            r.RegisterByKey("SVI-47", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20));
            r.RegisterByKey("SVI-48", new LockAfterAttackMon("Aqua Slash", false));
            r.RegisterByKey("SVI-50", new LockAfterAttackMon("Aqua Cannon", false));
            r.RegisterByKey("SVI-51", new SearchAttackMon("Vivid Charge", d => d.IsBasicEnergy, 3));
            r.RegisterByKey("SVI-56", new DigALittleMon());
            r.RegisterByKey("SVI-60", new CardsBST1.GMaxRollingMon("Sweeping Tackle", 20));
            r.RegisterByKey("SVI-63", new SwitchSelfMon("Magnetic Switch", false));
            r.RegisterByKey("SVI-64", new RecoilMon("Explosion", 90));
            r.RegisterByKey("SVI-65", new ComboMon(new EnergyCrushMon(), new RecoilMon("Pulse Launcher", 30)));
            r.RegisterByKey("SVI-67", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("SVI-69", new CardsMEG.SnipeMon("Linear Attack", 20));
            r.RegisterByKey("SVI-70", new ComboMon(new JunkHuntMon(), new ParalyzeFlipMon("Thunder Shock")));
            r.RegisterByKey("SVI-72", new ComboMon(new YankAwayMon(), new RecoilMon("Thunder", 20)));
            r.RegisterByKey("SVI-73", new ParalyzeFlipMon("Jolt"));
            r.RegisterByKey("SVI-77", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SVI-79", new ComboMon(new CardsMEG.SnipeMon("Skill Dive", 50), new CardsASC.SelfDiscardTypeMon("Thunder Blast", EnergyType.Lightning, 1)));
            r.RegisterByKey("SVI-82", new ForcedSleepMon());
            r.RegisterByKey("SVI-83", new PendulumInfluenceMon());
            r.RegisterByKey("SVI-87", new EnvelopingShadowMon());
            r.RegisterByKey("SVI-88", new BanetteExSVI());
            r.RegisterByKey("SVI-92", new CardsFFI1.MagicalLeafThirtyMon());
            r.RegisterByKey("SVI-93", new FlorgesSVI());
            r.RegisterByKey("SVI-94", new CardsASC.FlipDiscardDefendingEnergyMon("Energy Munch"));
            r.RegisterByKey("SVI-95", new CardsMEW1.CounterBonusMon("Second Bite", 10));
            r.RegisterByKey("SVI-98", new ShieldAttackMon("Springy", 30));
        }

        // ================================================================== Shared helpers (also used by CardsSVI2)

        /// <summary>"Flip a coin. If tails, during your next turn, this Pokémon can't attack" (Leg Stomp, Heavy Stomp).</summary>
        public sealed class FlipTailsLockMon : Mon
        {
            private readonly string attack;

            public FlipTailsLockMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>"Move all Energy from this Pokémon to 1 of your Benched Pokémon" after the damage (Happy Cyclone). With no Bench the Energy stays.</summary>
        public sealed class MoveAllEnergyToBenchMon : Mon
        {
            private readonly string attack;

            public MoveAllEnergyToBenchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var bench = ctx.Me.Bench.Where(m => m != me).ToList();
                if (!ctx.InPlay(me) || bench.Count == 0 || me.Energy.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, attack + ": move all Energy from " + me.Name + " to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var card in me.Energy.ToList()) ctx.Engine.MoveEnergy(me, target, card);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Adaptive Evolution: this Pokémon can evolve during your first turn or the turn you play it.</summary>
        internal sealed class AdaptiveEvolutionMon : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }
        }

        /// <summary>Miracle Powder: flip a coin; on heads choose a Special Condition for the opponent's Active Pokémon. Used for Miracle Powder, Miraculous Paint (the attack name is passed in).</summary>
        internal sealed class MiraclePowderMon : Mon
        {
            private readonly string attack;

            public MiraclePowderMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                var all = new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };
                int pick = ctx.ChooseOption(attack + ": choose a Special Condition", all.Select(c => c.ToString()).ToArray());
                ctx.ApplyCondition(defending, all[pick]);
                return true;
            }
        }

        /// <summary>Trick Cape: you may put an Energy attached to the opponent's Active Pokémon into their hand.</summary>
        private sealed class TrickCapeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick Cape") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Trick Cape: put an Energy attached to " + defending.Name + " into your opponent's hand?")) return true;
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Trick Cape: put which Energy into your opponent's hand?")[0];
                ctx.Engine.ReturnEnergyToHand(defending, card);
                return true;
            }
        }

        /// <summary>String Haul: flip a coin; on heads switch in 1 of the opponent's Benched Pokémon to the Active Spot.</summary>
        private sealed class StringHaulMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "String Haul") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.GustTargets().Count > 0) ctx.GustOpponent("String Haul: switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>Trap Territory: the opponent's Active Pokémon's Retreat Cost is [C] more. Wire Hang: 30 more for each [C] in it.</summary>
        private sealed class SpidopsExSVI : Mon
        {
            private static readonly Mon WireHang = new CardsOBF1.RetreatCostDamageMon("Wire Hang", 30, false);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !src.State.IsActive(mon)) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WireHang.HandleAttack(ctx, atk); }
        }

        /// <summary>Eerie Tentacles: you may move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class EerieTentaclesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eerie Tentacles") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Eerie Tentacles: move an Energy from " + defending.Name + " to 1 of your opponent's Benched Pokémon?")) return true;
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Eerie Tentacles: move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Eerie Tentacles: move it to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Passionate Singing: attach up to 2 Basic Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class PassionateSingingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Passionate Singing") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0 && ctx.Me.HasPokemonInPlay)
                    ctx.AttachCardsAnyWay(energy, 2, (c, m) => true, "Passionate Singing: attach a Basic Energy from your discard pile");
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Jump: flip a coin; on heads 10 damage to 1 of the opponent's Pokémon (no Weakness or Resistance for Benched Pokémon).</summary>
        private sealed class JumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jump") return false;
                if (ctx.Flip()) ctx.DamageAnyOpponent(10);
                return true;
            }
        }

        /// <summary>Dig a Little: flip a coin; on heads discard the top card of the opponent's deck.</summary>
        private sealed class DigALittleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig a Little") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Energy Crush: 50 damage for each Energy attached to all of the opponent's Pokémon.</summary>
        private sealed class EnergyCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Crush") return false;
                int energy = ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
                if (energy > 0) ctx.DamageDefending(atk.Damage * energy);
                else ctx.Log(ctx.Pokemon.Name + "'s Energy Crush did no damage.");
                return true;
            }
        }

        /// <summary>Junk Hunt: put an Item card from your discard pile into your hand.</summary>
        private sealed class JunkHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Junk Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Junk Hunt: put which Item card into your hand?"));
                return true;
            }
        }

        /// <summary>Yank Away: choose 2 random cards from the opponent's hand; they reveal them and shuffle them into their deck.</summary>
        private sealed class YankAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Yank Away") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var picked = new List<CardInstance>();
                var pool = hand.ToList();
                for (int k = 0; k < 2 && pool.Count > 0; k++)
                {
                    var card = pool[ctx.State.Rng.Next(pool.Count)];
                    pool.Remove(card);
                    picked.Add(card);
                }
                ctx.Engine.Reveal(ctx.Opponent, picked, "revealed these cards from " + ctx.Engine.Their(ctx.Opponent) + " hand");
                foreach (var card in picked)
                    if (hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Forced Sleep: the opponent chooses 1 of their Benched Pokémon and switches it with their Active Pokémon; the new Active
        /// Pokémon is now Asleep. With no Bench nothing happens (your ruling, v41).
        /// </summary>
        private sealed class ForcedSleepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Forced Sleep") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.OpponentSwitchesActive()) return true;
                var fresh = ctx.Opp.Active;
                if (fresh != null) ctx.ApplyCondition(fresh, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Pendulum Influence: flip a coin; on heads choose an attack from 1 of the opponent's Pokémon in play and use it as this
        /// attack. Hypno is "this Pokémon" for the copied attack (Compendium).
        /// </summary>
        private sealed class PendulumInfluenceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pendulum Influence") return false;
                if (!ctx.Flip()) return true;
                var withAttacks = ctx.OpponentPokemon(m => m.Def.Attacks.Count > 0 || (m.Def.IsLevelUp && m.Stack.Count >= 2));
                if (withAttacks.Count == 0) return Nothing(ctx);
                var from = ctx.ChoosePokemon(withAttacks, "Pendulum Influence: use an attack of which of your opponent's Pokémon?");
                if (from == null) return Nothing(ctx);
                CardDef def;
                int index;
                if (!ChooseCopiedAttack(ctx, from, "Pendulum Influence: choose 1 of " + from.Name + "'s attacks to use", out def, out index)) return Nothing(ctx);
                UseCopiedAttack(ctx, def, index);
                return true;
            }
        }

        /// <summary>Enveloping Shadow: flip a coin; on heads the opponent can't play Item cards from their hand during their next turn.</summary>
        private sealed class EnvelopingShadowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enveloping Shadow") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new CardsPAR2.TrainerKindLockEffect("Enveloping Shadow: no Item cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1, false));
                return true;
            }
        }

        /// <summary>
        /// Everlasting Darkness: the opponent can't play Item cards from their hand during their next turn (Pokémon Tools aren't
        /// Items: Compendium, Vitality Band). Poltergeist: the opponent reveals their hand; 60 damage for each Trainer card there.
        /// </summary>
        private sealed class BanetteExSVI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Everlasting Darkness")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.AddEffect(new CardsPAR2.TrainerKindLockEffect("Everlasting Darkness: no Item cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1, false));
                    return true;
                }
                if (atk.Name == "Poltergeist")
                {
                    TefPokemon.RevealOpponentHand(ctx);
                    int trainers = ctx.Opp.Hand.Count(c => c.Def.IsTrainer);
                    if (trainers > 0) ctx.DamageDefending(atk.Damage * trainers);
                    else ctx.Log(ctx.Pokemon.Name + "'s Poltergeist did no damage.");
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Blooming Garden: your Pokémon in play have no Weakness. Moonblast: the Defending Pokémon's attacks do 30 less damage
        /// during the opponent's next turn, to every target (Compendium, Kilowattrel's Skill Dive).
        /// </summary>
        private sealed class FlorgesSVI : Mon
        {
            private static readonly Mon Moonblast = new CardsMEG.WeakenDefendingMon("Moonblast", 30);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Moonblast.HandleAttack(ctx, atk); }
        }
    }
}
