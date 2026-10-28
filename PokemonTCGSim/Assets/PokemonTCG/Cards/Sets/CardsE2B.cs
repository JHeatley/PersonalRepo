using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Aquapolis Uncommon and Common Pokémon (E2 45-117; v146). Unlimited only. Elekid (9), Smoochum (61) and Tyrogue (63) are Baby
    /// Pokémon. Your rulings: Furret's Scavenger Hunt needs at least 2 cards in your hand. Pokémon whose attacks only do their
    /// printed damage: Psyduck 104.
    /// </summary>
    public static class CardsE2B
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E2-9", new EnergyKickMon());
            r.RegisterByKey("E2-45", new BlotMon());
            r.RegisterByKey("E2-46", new ComboMon(new CoinsMon("Tri Attack", 3), new PeckAttackMon()));
            r.RegisterByKey("E2-47", new ComboMon(new ParalyzeFlipMon("Thundershock"), new SparkMon()));
            r.RegisterByKey("E2-48", new FurretE2());
            r.RegisterByKey("E2-49", new GloomE2());
            r.RegisterByKey("E2-50", new CardsSI.HeadsTailsConditionMon("Distortion Beam", SpecialCondition.Asleep, SpecialCondition.Confused));
            r.RegisterByKey("E2-51", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("E2-52", new AttractMon());
            r.RegisterByKey("E2-53", new ConditionAttackMon("Sleepy Ball", SpecialCondition.Asleep));
            r.RegisterByKey("E2-54", new ComboMon(new TripleBoneMon(), new CardsE2.FlipUntilTailsMon("Bone Rush")));
            r.RegisterByKey("E2-55", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Horn", SpecialCondition.Poisoned), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("E2-56", new CardsBS1.ThrashMon("Thrash"));
            r.RegisterByKey("E2-57", new ComboMon(new CoinsMon("Quick Turn", 2), new CrossCutMon()));
            r.RegisterByKey("E2-58", new WaterBulletMon());
            r.RegisterByKey("E2-59", new ComboMon(new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters), new DodgeMon("Undulate", false)));
            r.RegisterByKey("E2-60", new LightweightMon(new BreakPowderMon()));
            r.RegisterByKey("E2-61", new EnergyKissMon());
            r.RegisterByKey("E2-62", new NoRetreatMon("Gooey Thread"));
            r.RegisterByKey("E2-63", new EnergyPunchMon());
            r.RegisterByKey("E2-64", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("E2-65", new ComboMon(new GrowthMon(), new CoinsMon("Double Razor Leaf", 2)));
            r.RegisterByKey("E2-66", new DeepDiveMon());
            r.RegisterByKey("E2-67", new ComboMon(new StretchTailMon(), new SkedaddleMon()));
            r.RegisterByKey("E2-68", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Vine", SpecialCondition.Poisoned), new FlipBonusMon("Sharp Leaf", 10)));
            r.RegisterByKey("E2-69", new RecoilMon("Double-edge", 40));
            r.RegisterByKey("E2-70", new DodgeMon("Float", false));
            r.RegisterByKey("E2-71", new ComboMon(new ParalyzeFlipMon("Jolt"), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("E2-72", new TearyEyesMon());
            r.RegisterByKey("E2-73", new DodgeMon("Super Speed", false));
            r.RegisterByKey("E2-74", new CardsE2.SleepInducerMon(true));
            r.RegisterByKey("E2-75", new ComboMon(new ChargeUpMon(), new TailsNothingMon("Lunge")));
            r.RegisterByKey("E2-76", new ComboMon(new GrowthSpurtMon(), new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep)));
            r.RegisterByKey("E2-77", new ComboMon(new CardsE2.FlipUntilTailsMon("Continuous Eggsplosion"), new EnergySupportMon()));
            r.RegisterByKey("E2-78", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("E2-79", new CardsPAR2.FlipConditionMon("Poison Sludge", SpecialCondition.Poisoned));
            r.RegisterByKey("E2-80", new ComboMon(new RoarMon(), new FlipBonusMon("Firebreathing", 10)));
            r.RegisterByKey("E2-81", new ComboMon(new RushMon(), new TailsNothingMon("Smash Punch")));
            r.RegisterByKey("E2-82", new ComboMon(new CoinsMon("Double Kick", 2), new TripleSpinMon()));
            r.RegisterByKey("E2-83", new LightweightMon(new RecoilMon("Careless Tackle", 10)));
            r.RegisterByKey("E2-84", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("E2-85", new SwitchSelfMon("Reverse Thrust", false));
            r.RegisterByKey("E2-86", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("E2-87", new CardsCES1.FeintAttackMon("Feint Attack", 20));
            r.RegisterByKey("E2-88", new BabyOutingMon());
            r.RegisterByKey("E2-89", new ComboMon(new MountainEaterMon(), new FlipBonusMon("Rising Lunge", 10)));
            r.RegisterByKey("E2-90", new ComboMon(new CardsE2.AnyOpponentMon("Long Tongue", 10), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("E2-91", new MagnemiteE2());
            r.RegisterByKey("E2-92", new ComboMon(new MugMon(), new RageMon()));
            r.RegisterByKey("E2-93", new ChargeMon());
            r.RegisterByKey("E2-94", new CardsE2.FlipUntilTailsMon("Continuous Tumble"));
            r.RegisterByKey("E2-95", new MrMimeE2());
            r.RegisterByKey("E2-96", new CardsPAR2.FlipConditionMon("Poison Horn", SpecialCondition.Poisoned));
            r.RegisterByKey("E2-97", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("E2-98", new CardsASC.FlipPerEnergyMon("Crush"));
            r.RegisterByKey("E2-99", new ComboMon(new ConditionAttackMon("Spore", SpecialCondition.Asleep), new CardsEVS1.EvolveFromDeckAttackMon("Spore Evolution")));
            r.RegisterByKey("E2-100", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("E2-101", new ComboMon(new HornGrabMon(), new SuperSliceMon()));
            r.RegisterByKey("E2-102", new CardsBS1.DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("E2-103", new DataSortMon());
            r.RegisterByKey("E2-104", new PlainMon());
            r.RegisterByKey("E2-105", new CardsE2.AnyOpponentMon("Splatter", 10));
            r.RegisterByKey("E2-106", new CardsN4.SwiftMon());
            r.RegisterByKey("E2-107", new ComboMon(new SpyMon(), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("E2-108", new FishingTailMon());
            r.RegisterByKey("E2-109", new ComboMon(new PaintTrickMon(), new CoinsMon("Doublslap", 2)));
            r.RegisterByKey("E2-110", new ComboMon(new RobMon(), new TailsNothingMon("Claw")));
            r.RegisterByKey("E2-111", new CardsSI.HeadsTailsConditionMon("Sharp Stinger", SpecialCondition.Poisoned, SpecialCondition.Paralyzed));
            r.RegisterByKey("E2-112", new ComboMon(new TicklingVinesMon(), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("E2-113", new CardsBS1.DiscardCostMon("Recover", EnergyType.Water, 1, RecoverAll, new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("E2-114", new MinorErrandRunningMon());
            r.RegisterByKey("E2-115", new CardsN4.SwiftMon());
            r.RegisterByKey("E2-116", new CollectEnergyMon("Collect Fire", EnergyType.Fire));
            r.RegisterByKey("E2-117", new ParalyzeFlipMon("Bubble"));
        }

        /// <summary>Recover: remove all damage counters from this Pokémon (after discarding a Water Energy card for it).</summary>
        private static void RecoverAll(EffectContext ctx, AttackDef atk)
        {
            var me = ctx.Pokemon;
            if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
        }

        // ================================================================== Baby Pokémon

        /// <summary>Energy Kick: heads, choose an Energy card on the Defending Pokémon; the opponent moves it to 1 of their other Pokémon (nothing without a Bench).</summary>
        private sealed class EnergyKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Kick") return false;
                var defending = ctx.Defending;
                if (defending == null || ctx.Opp.Bench.Count == 0) return Nothing(ctx);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Energy Kick: choose an Energy card attached to " + defending.Name)[0];
                var bench = ctx.OpponentBench();
                var target = bench.Count == 1 ? bench[0] : ctx.Engine.ChoosePokemon(ctx.Opponent, bench, "Energy Kick: move " + card.Def.Name + " to which of your Benched Pokémon?", ctx.Source);
                if (target != null) ctx.Engine.MoveEnergy(defending, target, card);
                return true;
            }
        }

        /// <summary>Energy Kiss: a coin for each Energy card attached to the Defending Pokémon, 10 per heads.</summary>
        private sealed class EnergyKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Kiss") return false;
                var defending = ctx.Defending;
                int coins = defending == null ? 0 : defending.Energy.Count;
                ctx.DamageDefending(atk.Damage * (coins > 0 ? ctx.FlipCoins(coins) : 0));
                return true;
            }
        }

        /// <summary>Energy Punch: heads, discard a Special Energy card attached to the Defending Pokémon; tails, the attack does nothing.</summary>
        private sealed class EnergyPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Punch") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                var special = defending.Energy.Where(e => e.Def.IsSpecialEnergy).ToList();
                if (special.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Energy Punch: discard which Special Energy card from " + defending.Name + "?", e => e.Def.IsSpecialEnergy);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Blot: remove 1 damage counter from this Pokémon.</summary>
        internal sealed class BlotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blot") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Enervating Pollen (Poké-Body): Resistance on each Active Pokémon only reduces damage by 10. Sleep Sap: both the Defending Pokémon and Gloom are Asleep.</summary>
        private sealed class GloomE2 : Mon
        {
            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount)
            {
                return mon != null && src.IsActive(mon) && amount > 10 ? 10 : amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Sap") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Cross-Cut: 20 more against an Evolved Pokémon.</summary>
        private sealed class CrossCutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cross-Cut") return false;
                ctx.DamageDefending(atk.Damage + (CardsE2.IsEvolved(ctx.Defending) ? 20 : 0));
                return true;
            }
        }

        /// <summary>Lightweight (Poké-Body): [C] less Retreat Cost for each Grass Energy attached.</summary>
        private sealed class LightweightMon : Mon
        {
            private readonly Mon attacks;

            public LightweightMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon ? cost - src.Engine.EnergyCountOfType(mon, EnergyType.Grass) : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Break Powder: heads, 20 more; tails, the Defending Pokémon is Asleep.</summary>
        private sealed class BreakPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Break Powder") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Growth: attach up to 2 Grass Energy cards from your hand to this Pokémon.</summary>
        private sealed class GrowthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Growth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var grass = ctx.Me.Hand.Where(c => CardsE2.IsBasicEnergyOf(c.Def, EnergyType.Grass)).ToList();
                if (grass.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(grass, 2, (c, m) => m == me, "Growth");
                return true;
            }
        }

        /// <summary>Growth Spurt: attach an Energy card from your hand to this Pokémon.</summary>
        private sealed class GrowthSpurtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Growth Spurt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(energy, 1, (c, m) => m == me, "Growth Spurt", false);
                return true;
            }
        }

        /// <summary>Horn Grab: heads, Paralyzed; tails, the attack does nothing.</summary>
        private sealed class HornGrabMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horn Grab") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Super Slice: 2 coins; if either is tails, the attack does nothing.</summary>
        private sealed class SuperSliceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Slice") return false;
                if (ctx.FlipCoins(2) < 2) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Tickling Vines: heads, the Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class TicklingVinesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tickling Vines") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Tickling Vines)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Water Bullet: a coin for each Water Energy attached, 30 + 10 per heads.</summary>
        private sealed class WaterBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Bullet") return false;
                int coins = ctx.Pokemon == null ? 0 : ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Deep Dive: 3 coins; a damage counter off this Pokémon for each heads.</summary>
        private sealed class DeepDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Dive") return false;
                int heads = ctx.FlipCoins(3);
                var me = ctx.Pokemon;
                if (heads > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10 * heads);
                return true;
            }
        }

        /// <summary>Fishing Tail: a Pokémon card or basic Energy card from your discard pile into your hand.</summary>
        private sealed class FishingTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fishing Tail") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Discard.Where(c => c.Def.IsPokemon || c.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0) return true;
                ctx.DiscardToHand(cards.Count == 1 ? cards : ctx.ChooseCards(cards, 1, 1, "Fishing Tail: put which card into your hand?"));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Spark: 10 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spark") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>Conductive Body (Poké-Body): [C] less Retreat Cost for each Magnemite on your Bench. Magnetic Bomb: heads, 10 more; tails, 10 to itself.</summary>
        private sealed class MagnemiteE2 : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon != src.Pokemon) return cost;
                return cost - src.State.Players[mon.Owner].Bench.Count(m => m.Name == "Magnemite");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Bomb") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Charge: attach a Lightning Energy card from your discard pile to this Pokémon.</summary>
        private sealed class ChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => CardsE2.IsBasicEnergyOf(c.Def, EnergyType.Lightning)).ToList();
                if (cards.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(cards, 1, (c, m) => m == me, "Charge", false);
                return true;
            }
        }

        /// <summary>Collect Fire (and the like): with [type] Energy cards in your discard pile, flip; heads, attach 1 of them to this Pokémon.</summary>
        internal sealed class CollectEnergyMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public CollectEnergyMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.Me.Discard.Where(c => CardsE2.IsBasicEnergyOf(c.Def, type)).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me) || !ctx.Flip()) return true;
                ctx.AttachCardsAnyWay(cards, 1, (c, m) => m == me, attack, false);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Energy Barrier (Poké-Body): attack damage to it is 10 less for each basic Energy card attached (20 less at most). Mind Shock: no Weakness or Resistance.</summary>
        private sealed class MrMimeE2 : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                int cut = Math.Min(20, 10 * me.Energy.Count(e => e.Def.IsBasicEnergy));
                return Math.Max(0, damage - cut);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Shock") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>Energy Support: heads, attach a Psychic Energy card from your deck to 1 of your Benched Pokémon.</summary>
        private sealed class EnergySupportMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Support") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsE2.IsBasicEnergyOf(d, EnergyType.Psychic), 1, "Energy Support: choose a Psychic Energy card");
                foreach (var card in found)
                {
                    var bench = ctx.MyBench();
                    var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Energy Support: attach it to which Benched Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Triple Bone: 3 coins; 10 damage per heads to 1 of the opponent's Benched Pokémon (no Weakness or Resistance).</summary>
        private sealed class TripleBoneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Bone") return false;
                if (ctx.Opp.Bench.Count == 0) return true;
                var bench = ctx.OpponentBench();
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Triple Bone: choose 1 of your opponent's Benched Pokémon");
                int heads = ctx.FlipCoins(3);
                if (target != null && heads > 0 && ctx.InPlay(target)) ctx.DamageTarget(target, 10 * heads);
                return true;
            }
        }

        /// <summary>Rush: 5 coins, 10 per heads; this Pokémon can't attack during your next turn.</summary>
        private sealed class RushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rush") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(5));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Triple Spin: 3 coins, 20 per heads; with 2 or more heads, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class TripleSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Spin") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Mountain Eater: the opponent discards the top card of their deck; then a damage counter off this Pokémon.</summary>
        private sealed class MountainEaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Eater") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Mug: before the damage, discard all Trainer cards attached to the Defending Pokémon.</summary>
        private sealed class MugMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mug") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending))
                    foreach (var tool in defending.Tools.ToList()) ctx.Engine.DiscardTool(defending, tool);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Rage: 10 more for each damage counter on this Pokémon.</summary>
        internal sealed class RageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rage") return false;
                ctx.DamageDefending(atk.Damage + 10 * (ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Rob: look at the opponent's hand; every Technical Machine and Pokémon Tool card there goes into their deck, which they shuffle.</summary>
        private sealed class RobMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rob") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Rob: your opponent's hand", ctx.Source);
                var picks = hand.Where(c => CardsE2T.IsTechnicalMachineOrTool(c.Def)).ToList();
                if (picks.Count == 0) return true;
                foreach (var card in picks) if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Rob: " + string.Join(", ", picks.Select(c => c.Def.Name).ToArray()) + " went into " + ctx.Engine.Possessive(ctx.Opponent) + " deck, which was shuffled.");
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Peck Attack: 2 coins; 1 heads, 20; 2 heads, 50; 2 tails, the Defending Pokémon is Confused.</summary>
        private sealed class PeckAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peck Attack") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                if (heads == 0)
                {
                    if (defending != null && ctx.CanAffect(defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                ctx.DamageDefending(heads == 2 ? 50 : 20);
                return true;
            }
        }

        /// <summary>
        /// Scavenger Hunt (Poké-Power): put 2 cards from your hand into your deck (your ruling: only with at least 2 cards in your hand),
        /// then search your deck for an Energy card and put it into your hand.
        /// </summary>
        private sealed class FurretE2 : Mon
        {
            private const string Power = "Scavenger Hunt";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count >= 2; }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count < 2) return;
                var picks = ctx.ChooseCards(hand, 2, 2, Power + ": put 2 cards from your hand into your deck");
                foreach (var card in picks) if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                ctx.SearchToHand(d => d.IsEnergy, 1, Power + ": choose an Energy card");
            }
        }

        /// <summary>Attract: switch 1 of the opponent's Benched Pokémon with the Defending Pokémon.</summary>
        private sealed class AttractMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attract") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.GustTargets().Count > 0) ctx.GustOpponent("Attract: choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>Stretch Tail: heads, 10 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class StretchTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stretch Tail") return false;
                if (ctx.Opp.Bench.Count > 0 && ctx.Flip()) ctx.DamageOneBenched(10);
                return true;
            }
        }

        /// <summary>
        /// Skedaddle: with a Bench, after the damage, this Pokémon and its basic Energy cards go back to your hand (its other cards are
        /// discarded); without one, the attack does nothing.
        /// </summary>
        private sealed class SkedaddleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Skedaddle") return false;
                if (ctx.Me.Bench.Count == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Engine.IsKnockedOut(me) || !ctx.Engine.CanPutIntoHand(me)) return true;
                foreach (var card in me.Energy.Where(x => !x.Def.IsBasicEnergy).ToList()) ctx.Engine.DiscardEnergy(me, card);
                foreach (var t in me.Tools.ToList()) ctx.Engine.DiscardTool(me, t);
                ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Teary Eyes: during the opponent's next turn, attack damage to this Pokémon is 20 less.</summary>
        internal sealed class TearyEyesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teary Eyes") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(20);
                return true;
            }
        }

        /// <summary>Charge Up: heads, attach an Energy card from your deck to this Pokémon.</summary>
        private sealed class ChargeUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Charge Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Charge Up: choose an Energy card to attach to " + me.Name);
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Roar: the opponent switches the Defending Pokémon with 1 of their Benched Pokémon (their choice).</summary>
        private sealed class RoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Baby Outing: look at the top 3 cards of your deck, put 1 of them into your hand and shuffle the rest into your deck.</summary>
        private sealed class BabyOutingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Baby Outing") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return true;
                var pick = ctx.ChooseCards(top, 1, 1, "Baby Outing: put which card into your hand?");
                ctx.DeckToHand(pick);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Data Sort: heads, shuffle a Trainer card from your discard pile into your deck.</summary>
        private sealed class DataSortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Data Sort") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0 || !ctx.Flip()) return true;
                var pick = trainers.Count == 1 ? trainers : ctx.ChooseCards(trainers, 1, 1, "Data Sort: shuffle which Trainer card into your deck?");
                ctx.Engine.ShowCards(ctx.Opponent, pick, "Data Sort", "Your opponent shuffles this card into their deck.", ctx.Source);
                ctx.DiscardToDeck(pick);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Spy: look at the top 3 cards of the opponent's deck (they stay in the same order).</summary>
        private sealed class SpyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.Opp.Deck.Take(3).ToList();
                if (top.Count > 0) ctx.Engine.ShowCards(ctx.Player, top, "Top of your opponent's deck", "Spy: the top " + top.Count + " cards of your opponent's deck", ctx.Source);
                return true;
            }
        }

        /// <summary>Paint Trick: heads, the Defending Pokémon is Confused; then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class PaintTrickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paint Trick") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Minor Errand-Running: 2 coins; a basic Energy card from your deck into your hand for each heads.</summary>
        private sealed class MinorErrandRunningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Minor Errand-Running") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsBasicEnergy, heads, "Minor Errand-Running: choose up to " + heads + " basic Energy card" + (heads == 1 ? "" : "s"));
                return true;
            }
        }
    }
}
