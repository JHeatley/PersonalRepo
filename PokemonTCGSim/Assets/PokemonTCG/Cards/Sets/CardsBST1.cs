using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Battle Styles (BST, 2021), Pokémon 1-65: Grass, Fire, Water, Lightning and Psychic. Fighting through Colorless, the
    /// Trainers and the Special Energy are in CardsBST2. Regulation mark E: Expanded only.
    /// Already in the pool through Silver Tempest, Crown Zenith, Astral Radiance and Brilliant Stars: Kricketune V, Cherubi,
    /// Kingdra, Galarian Mr. Mime, Octillery, Shinx, Luxio, Luxray and Mimikyu V.
    /// Pokémon whose attacks only do their printed damage need no script (Cacnea, Blipbug, Tepig, Pignite, Seadra, Remoraid,
    /// Bruxish, Spoink).
    /// </summary>
    public static class CardsBST1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("BST-1", new CardsBLK.VenoshockMon(40));
            r.RegisterByKey("BST-2", new WeepinbellBST());
            r.RegisterByKey("BST-3", new PanicVineMon("Panic Vine"));
            r.RegisterByKey("BST-5", new ComboMon(new PullMon(), new Cards30C.TypeEnergyBonusMon("Spiny Punch", EnergyType.Darkness, 70)));
            r.RegisterByKey("BST-8", new CherrimBST());
            r.RegisterByKey("BST-9", new ComboMon(new NoRetreatMon("Big Bite"), new CoinsMon("Triple Whip", 3)));
            r.RegisterByKey("BST-10", new DevourMon());
            r.RegisterByKey("BST-11", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("BST-12", new CardsTWM1.HealSelfMon("Grass Cocooning", 40));
            r.RegisterByKey("BST-13", new VitalPowderMon());
            r.RegisterByKey("BST-14", new CardsPFL.FlipUntilTailsTimesMon("Continuous Slash"));
            r.RegisterByKey("BST-15", new CardsTWM1.HealSelfMon("Leaf Drain", 30));
            r.RegisterByKey("BST-16", new ComboMon(new CardsASC.PushDownMon("Push Down"), new NaturesJudgmentMon()));
            r.RegisterByKey("BST-18", new SourSpitMon("Sour Spit"));
            r.RegisterByKey("BST-19", new GMaxRollingMon("G-Max Rolling", 10));
            // Fire
            r.RegisterByKey("BST-20", new ComboMon(new HeatDashMon(), new ConditionAttackMon("Fire Fang", SpecialCondition.Burned)));
            r.RegisterByKey("BST-21", new ComboMon(new CardsCRZ1.VBonusMon("V Bullet", 50), new CardsTWM1.SelfDiscardMon("Flare Shot", 99)));
            r.RegisterByKey("BST-22", new ComboMon(new SpreadingFlamesMon(), new CardsCRZ1.VBonusMon("Max Victory", 120)));
            r.RegisterByKey("BST-25", new EmboarBST());
            r.RegisterByKey("BST-26", new BurningLicksMon());
            r.RegisterByKey("BST-27", new SearchAttackMon("Call Sign", IsPokemonCard, 1));
            r.RegisterByKey("BST-28", new ComboMon(new ConditionAttackMon("Perplex", SpecialCondition.Confused), new CardsASR1.TimesMon("Derisive Roasting", DefendingConditions)));
            r.RegisterByKey("BST-29", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("BST-30", new CentiskorchBST());
            // Water
            r.RegisterByKey("BST-31", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("BST-35", new BallJugglingMon());
            r.RegisterByKey("BST-38", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("BST-39", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("BST-40", new EmpoleonV());
            r.RegisterByKey("BST-41", new RecoverMon());
            r.RegisterByKey("BST-42", new CardsASR1.TimesMon("Sediment Sink", WaterEnergyInDiscard));
            // Lightning
            r.RegisterByKey("BST-44", new LockAfterAttackMon("Haymaker", true));
            r.RegisterByKey("BST-45", new ComboMon(new FlipBonusMon("Tumbling Attack", 30), new LockAfterAttackMon("Lightning Slam", true)));
            r.RegisterByKey("BST-49", new SearchAttackMon("Find a Friend", IsPokemonCard, 1));
            r.RegisterByKey("BST-50", new CardsASR1.PlusMon("Spiral Thunder", 40, OpponentsEnergyInPlay));
            r.RegisterByKey("BST-51", new MaxShockMon());
            r.RegisterByKey("BST-52", new YamperBST());
            r.RegisterByKey("BST-53", new BoltundBST());
            // Psychic
            r.RegisterByKey("BST-54", new TantailizingMon());
            r.RegisterByKey("BST-56", new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused));
            r.RegisterByKey("BST-57", new RecoilMon("Self-Destruct", 60));
            r.RegisterByKey("BST-58", new PowerSplitMon());
            r.RegisterByKey("BST-59", new ComboMon(new SearchAttackMon("Minor Errand-Running", IsBasicEnergyCard, 2), new ConditionAttackMon("Pleasant Tone", SpecialCondition.Asleep)));
            r.RegisterByKey("BST-60", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("BST-61", new MeowsticBST());
            r.RegisterByKey("BST-63", new ComboMon(new CardsFST1.TwoBenchedMon("Prismatic Ray", 20), new CardsSIT1.SpecialLaserMon("Special Laser", 120)));
            r.RegisterByKey("BST-64", new RadarMon("Radar", 4));
            r.RegisterByKey("BST-65", new EvomancyMon());
        }

        // ================================================================== Filters and counters

        internal static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }

        private static int WaterEnergyInDiscard(EffectContext ctx)
        {
            return ctx.Me.Discard.Count(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water));
        }

        private static int OpponentsEnergyInPlay(EffectContext ctx)
        {
            return ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
        }

        /// <summary>The number of Special Conditions on the opponent's Active Pokémon.</summary>
        private static int DefendingConditions(EffectContext ctx)
        {
            var mon = ctx.Defending;
            if (mon == null) return 0;
            int n = 0;
            foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                if (mon.Has(c)) n++;
            return n;
        }

        // ================================================================== Grass

        /// <summary>Dangerous Mucus: when you play this Pokémon from your hand to evolve 1 of your Pokémon, you may make the opponent's Active Pokémon Burned and Poisoned.</summary>
        private sealed class WeepinbellBST : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var theirs = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Dangerous Mucus", theirs != null && ctx.CanAffect(theirs), "Use Dangerous Mucus? The opponent's Active Pokémon becomes Burned and Poisoned.")) return;
                ctx.ApplyCondition(theirs, SpecialCondition.Burned);
                ctx.ApplyCondition(theirs, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Panic Vine: the opponent's Active Pokémon is now Confused and can't retreat during their next turn. Used for Panic Vine, Spiral Current (the attack name is passed in).</summary>
        internal sealed class PanicVineMon : Mon
        {
            private readonly string attack;

            public PanicVineMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Pull: switch 1 of the opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class PullMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.GustOpponent("Pull: switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>
        /// Spring Bloom: as often as you like during your turn, attach a Grass Energy card from your hand to 1 of your Pokémon
        /// that doesn't have a Rule Box.
        /// </summary>
        private sealed class CherrimBST : Mon
        {
            private const string Bloom = "Spring Bloom";

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => !m.Def.HasRuleBox); }

            private static bool CanBloom(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Grass)) && Targets(ctx).Count > 0;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == Bloom && ctx.IsMyTurn && CanBloom(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanBloom(ctx), offerDone =>
                {
                    var target = RepeatableAbility.ChoosePokemonOrDone(ctx, Targets(ctx),
                        Bloom + ": attach a Grass Energy card from your hand to which Pokémon (no Rule Box)?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (target == null) return false;
                    var card = ctx.Me.Hand.FirstOrDefault(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Grass));
                    if (card == null || !ctx.Me.Hand.Remove(card)) return false;
                    ctx.Engine.AttachEnergyCard(card, target, true);
                    return true;
                });
            }
        }

        /// <summary>Devour: discard the top card of the opponent's deck for each of your Durant in play.</summary>
        private sealed class DevourMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devour") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.MyPokemon(m => m.Name == "Durant").Count;
                if (n > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Vital Powder: heal all damage from 2 of your Benched Pokémon.</summary>
        private sealed class VitalPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vital Powder") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                int n = Math.Min(2, hurt.Count);
                var pick = hurt.Count <= 2 ? hurt : ctx.ChoosePokemonMany(hurt, n, n, "Vital Powder: heal all damage from which 2 Benched Pokémon?");
                foreach (var mon in pick) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Nature's Judgment: you may discard all Energy from this Pokémon; if you do, 80 more damage.</summary>
        private sealed class NaturesJudgmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nature's Judgment") return false;
                var me = ctx.Pokemon;
                int bonus = 0;
                if (me.Energy.Count > 0 && ctx.AskYesNo("Nature's Judgment: discard all Energy from " + me.Name + " for 80 more damage?"))
                    if (ctx.Engine.DiscardAllEnergy(me) > 0) bonus = 80;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Sour Spit: during the opponent's next turn, the Defending Pokémon's attacks cost 2 Colorless more. Used for Sour Spit, Dead Wood Detention (the attack name is passed in).</summary>
        internal sealed class SourSpitMon : Mon
        {
            private readonly string attack;

            public SourSpitMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost [C][C] more during its next turn (" + attack + ")",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 2));
                return true;
            }
        }

        /// <summary>G-Max Rolling: 10 less damage for each damage counter on this Pokémon. Used for G-Max Rolling, Sweeping Tackle (the attack name and the numbers are passed in).</summary>
        internal sealed class GMaxRollingMon : Mon
        {
            private readonly string attack;
            private readonly int each;

            public GMaxRollingMon(string attack, int each)
            {
                this.attack = attack;
                this.each = each;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int damage = Math.Max(0, atk.Damage - each * ctx.Pokemon.DamageCounters);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log(ctx.Pokemon.Name + "'s " + attack + " did no damage.");
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Heat Dash: you may attach a Fire Energy card from your hand to this Pokémon.</summary>
        private sealed class HeatDashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heat Dash") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var pile = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (pile.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(pile, 1, SelfOnly(me), "Heat Dash: attach a Fire Energy card to " + me.Name);
                return true;
            }

            private static Func<CardInstance, PokemonInPlay, bool> SelfOnly(PokemonInPlay me)
            {
                return (card, mon) => mon == me;
            }
        }

        /// <summary>Spreading Flames: attach up to 3 Fire Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        internal sealed class SpreadingFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spreading Flames") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 3, AnyOfMine, "Spreading Flames: attach a Fire Energy card");
                return true;
            }

            private static bool AnyOfMine(CardInstance card, PokemonInPlay mon) { return true; }
        }

        /// <summary>Fighting Fury Stance: your Single Strike Pokémon's attacks do 30 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class EmboarBST : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !attacker.Def.IsSingleStrike) return damage;
                return info.ToOpponentsActive && damage > 0 ? damage + 30 : damage;
            }
        }

        /// <summary>Burning Licks: flip 2 coins; for each heads, discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class BurningLicksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Licks") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, Math.Min(heads, defending.Energy.Count), "Burning Licks: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Overheater: the opponent's Burned Pokémon doesn't recover when their Pokémon Checkup coin is heads. Bursting
        /// Inferno: the opponent's Active Pokémon is now Burned.
        /// </summary>
        private sealed class CentiskorchBST : Mon
        {
            private static readonly Mon BurstingInferno = new ConditionAttackMon("Bursting Inferno", SpecialCondition.Burned);

            public override bool KeepsOpponentBurned(EffectContext src, int player) { return src.Pokemon != null && player != src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return BurstingInferno.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Ball Juggling: discard any number of Item cards with "Ball" in their name from your hand; 40 more damage for each.</summary>
        private sealed class BallJugglingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ball Juggling") return false;
                var balls = ctx.Me.Hand.Where(c => c.Def.IsItem && c.Def.Name.Contains("Ball")).ToList();
                int n = 0;
                if (balls.Count > 0)
                    foreach (var card in ctx.ChooseCards(balls, 0, balls.Count, "Ball Juggling: discard any number of Ball Item cards (40 more damage each)"))
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                ctx.DamageDefending(atk.Damage + 40 * n);
                return true;
            }
        }

        /// <summary>
        /// Emperor's Eyes: while this Pokémon is Active, the opponent's Basic Pokémon in play without a Rule Box have no
        /// Abilities. Swirling Slice: move an Energy from this Pokémon to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class EmpoleonV : Mon
        {
            private static readonly Mon SwirlingSlice = new CardsFST1.MoveEnergyToBenchMon("Swirling Slice");

            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner == src.Player || !src.State.IsActive(me)) return false;
                return mon.Def.IsBasicPokemon && !mon.Def.HasRuleBox;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SwirlingSlice.HandleAttack(ctx, atk); }
        }

        /// <summary>Recover: discard an Energy from this Pokémon and heal all damage from it.</summary>
        private sealed class RecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Recover: discard an Energy from " + me.Name) > 0)
                    ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Max Shock: if you have more Prize cards remaining than your opponent, their Active Pokémon is now Paralyzed.</summary>
        private sealed class MaxShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count && TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Ball Search: when you play this Pokémon from your hand onto your Bench, you may put a Poké Ball card and/or a Great Ball card from your discard pile into your hand.</summary>
        private sealed class YamperBST : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var poke = ctx.Me.Discard.FirstOrDefault(c => c.Def.Name == "Poké Ball");
                var great = ctx.Me.Discard.FirstOrDefault(c => c.Def.Name == "Great Ball");
                if (!TriggerOnce(ctx, "Ball Search", poke != null || great != null, "Use Ball Search? Put a Poké Ball and a Great Ball from your discard pile into your hand.")) return;
                var take = new List<CardInstance>();
                if (poke != null) take.Add(poke);
                if (great != null) take.Add(great);
                ctx.DiscardToHand(take);
            }
        }

        /// <summary>Corner: the Defending Pokémon can't retreat. Defiant Spark: costs only [L] if this Pokémon has any damage counters on it.</summary>
        private sealed class BoltundBST : Mon
        {
            private static readonly Mon Corner = new NoRetreatMon("Corner");

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Defiant Spark" || ctx.Pokemon == null || ctx.Pokemon.DamageCounters == 0) return;
                cost.Clear();
                cost.Add(EnergyType.Lightning);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Corner.HandleAttack(ctx, atk); }
        }

        // ================================================================== Psychic

        /// <summary>Tantailizing: flip a coin; on heads, switch 1 of the opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class TantailizingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantailizing") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.GustTargets().Count > 0 && ctx.Flip()) ctx.GustOpponent("Tantailizing: switch in which of your opponent's Benched Pokémon?");
                return true;
            }
        }

        /// <summary>
        /// Power Split: attach Psychic Energy cards from your discard pile to your Pokémon in any way you like until your
        /// Pokémon and the opponent's Pokémon have the same total amount of Energy attached.
        /// </summary>
        private sealed class PowerSplitMon : Mon
        {
            private static int Total(EffectContext ctx, bool mine)
            {
                return (mine ? ctx.MyPokemon() : ctx.OpponentPokemon()).Sum(m => ctx.EnergyCount(m));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Split") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                for (int guard = 0; guard < 30; guard++)
                {
                    if (Total(ctx, true) >= Total(ctx, false)) break;
                    var pile = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Psychic)).ToList();
                    if (pile.Count == 0) break;
                    if (ctx.AttachCardsAnyWay(pile, 1, AnyOfMine, "Power Split: attach a Psychic Energy card", false) == 0) break;
                }
                return true;
            }

            private static bool AnyOfMine(CardInstance card, PokemonInPlay mon) { return true; }
        }

        /// <summary>Ear Moves: once during your turn, move 1 damage counter from 1 of your Pokémon to 1 of your opponent's Pokémon.</summary>
        private sealed class MeowsticBST : Mon
        {
            protected override string Activated { get { return "Ear Moves"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0 && ctx.OpponentPokemon().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var giver = ctx.ChoosePokemon(ctx.MyPokemon(m => m.DamageCounters > 0), "Ear Moves: move a damage counter from which of your Pokémon?");
                var taker = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Ear Moves: move it to which of your opponent's Pokémon?");
                if (giver != null && taker != null) ctx.Engine.MoveDamageCounters(giver, taker, 1, "Ear Moves");
            }
        }

        /// <summary>Radar: look at the top 4 cards of your deck and put them back in any order. Used for Radar, Calculate, Spelunk, Clairvoyant Eye (the attack name and the numbers are passed in).</summary>
        internal sealed class RadarMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public RadarMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsOBF2.ReorderOwnTop(ctx, amount, attack);
                return true;
            }
        }

        /// <summary>Evomancy: for each Energy attached to this Pokémon, put a Stage 2 Pokémon (not Orbeetle) from your deck onto your Bench.</summary>
        private sealed class EvomancyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evomancy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(ctx.EnergyCount(ctx.Pokemon), ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Stage == Stage.Stage2 && d.Name != "Orbeetle" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d),
                    n, "Evomancy: up to " + n + " Stage 2 Pokémon for your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }
    }
}
