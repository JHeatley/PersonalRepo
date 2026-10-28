using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Shining Fates (SHF, 2021): the main-set and Shiny Vault Pokémon from Grass to Psychic. Fighting through Colorless and
    /// the Trainers are in CardsSHF2. Regulation mark D: Expanded only.
    /// Already in the pool: Frosmoth (Astral Radiance), Crobat V (Lost Origin), Boss's Orders, Professor's Research, Piers,
    /// Gym Trainer (Silver Tempest), and the Shiny Vault Blipbug, Flapple, Galarian Obstagoon, Oranguru, Centiskorch V / VMAX
    /// and Eternatus V / VMAX.
    /// Pokémon whose attacks only do their printed damage need no script (Yanma, Cacnea, Dartrix, Gossifleur, Horsea, Buizel,
    /// Floatzel, Chewtle, Sizzlipede, Sobble, Arrokuda, Toxel).
    /// Dracovish, Arctovish, Dracozolt and Arctozolt evolve from Rare Fossil (Rebel Clash), which isn't in the pool yet.
    /// </summary>
    public static class CardsSHF1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("SHF-2", new RecoilMon("Nosedive", 30));
            r.RegisterByKey("SHF-3", new ActiveLookMon("Woodland Stroll", IsEnergyCard, "an Energy card", null));
            r.RegisterByKey("SHF-5", new AttachLeavesMon());
            r.RegisterByKey("SHF-6", new SkyCircusMon(new CardsMEG.SnipeBenchMon("Wind Shard", 60)));
            r.RegisterByKey("SHF-8", new DecidueyeSHF());
            r.RegisterByKey("SHF-9", new ComboMon(new AnchorAngerMon(), new LockAfterAttackMon("Giga Hammer", true)));
            r.RegisterByKey("SHF-10", new ComboMon(new SwingingChainMon(), new LockAfterAttackMon("Max Anchor", true)));
            r.RegisterByKey("SHF-11", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("SHF-12", new ThwackeySHF());
            r.RegisterByKey("SHF-13", new RillaboomSHF());
            r.RegisterByKey("SHF-15", new ComboMon(new CardsOBF1.HealBenchedMon("Enriching Seeds", 9990), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("SHF-16", new CardsTWM1.HealSelfMon("Suctioning Vines", 30));
            r.RegisterByKey("SHF-SV8", new CardsASC.SelfGuardMon("Reflect", 40));
            r.RegisterByKey("SHF-SV9", new OrbeetleSHF());
            r.RegisterByKey("SHF-SV10", new BenchSearchAttackMon("Call for Family", 3));
            r.RegisterByKey("SHF-SV11", new BlessingOfFluffMon());
            r.RegisterByKey("SHF-SV12", new CardsMEW1.WithdrawMon("Withdraw"));
            r.RegisterByKey("SHF-SV14", new AppletunSHF());
            r.RegisterByKey("SHF-SV105", new ComboMon(new ForestFeastMon(), new RecoilMon("Wood Hammer", 30)));
            r.RegisterByKey("SHF-SV106", new MaxBeatingMon());
            // Fire
            r.RegisterByKey("SHF-17", new RecoilMon("Amazing Blaze", 60));
            r.RegisterByKey("SHF-18", new FieldRunnerMon());
            r.RegisterByKey("SHF-19", new ComboMon(new CardsFST1.CounterMon("Counter"), new ConditionAttackMon("Max Pyro Ball", SpecialCondition.Burned)));
            r.RegisterByKey("SHF-SV15", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("SHF-SV16", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("SHF-SV17", new CinderaceSHF());
            r.RegisterByKey("SHF-SV19", new ComboMon(new HundredFootFlamesMon(), new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            r.RegisterByKey("SHF-SV107", new CardsTWM1.SelfDiscardMon("G-Max Wildfire", 2));
            // Water
            r.RegisterByKey("SHF-21", new CardsCRZ1.SpreadAllMon("Amazing Surge"));
            r.RegisterByKey("SHF-24", new ActiveLookMon("Ocean Search", IsPokemonCard, "a Pokémon", null));
            r.RegisterByKey("SHF-25", new CoinsMon("Dynamite Steam", 2));
            r.RegisterByKey("SHF-27", new CardsFST1.TakeLessMon(30, null));
            r.RegisterByKey("SHF-28", new CardsMEG.SnipeBenchMon("Spit-Out Shot", 50));
            r.RegisterByKey("SHF-29", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("SHF-SV20", new CardsASC.SelfGuardMon("Reflect", 30));
            r.RegisterByKey("SHF-SV21", new MrRimeSHF());
            r.RegisterByKey("SHF-SV22", new EnergyToHandMon("Aurora Loop", EnergyType.Water, 2));
            r.RegisterByKey("SHF-SV23", new ParalyzeFlipMon("Ice Punch"));
            r.RegisterByKey("SHF-SV24", new ComboMon(new CardsMEW1.SpreadBenchMon("Blizzard", 10), new LockAfterAttackMon("Crushing Headbutt", true)));
            r.RegisterByKey("SHF-SV26", new ShadyDealingsMon(1, null));
            r.RegisterByKey("SHF-SV27", new ShadyDealingsMon(2, new Cards30CC.BenchHitMon("Aqua Bullet", 20)));
            r.RegisterByKey("SHF-SV29", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Vise Bite", 30, false), new NoRetreatMon("Jaw Lock")));
            r.RegisterByKey("SHF-SV30", new ComboMon(new DodgeMon("Dive", false), new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20)));
            r.RegisterByKey("SHF-SV32", new SpiralJetMon());
            r.RegisterByKey("SHF-SV35", new EiscueSHF());
            r.RegisterByKey("SHF-SV36", new DracovishSHF());
            r.RegisterByKey("SHF-SV37", new ComboMon(new CardsASC.SelfGuardMon("Hard Face", 60), new ConditionAttackMon("Cold Breath", SpecialCondition.Asleep)));
            r.RegisterByKey("SHF-SV110", new ComboMon(new BodySurfMon(), new EnergyToHandMon("Ocean Loop", EnergyType.Water, 2)));
            r.RegisterByKey("SHF-SV111", new CardsMEG.EnergyBonusMon("G-Max Pump", EnergyType.Water, 30));
            // Lightning
            r.RegisterByKey("SHF-31", new CardsFST2.ConditionBonusMon("Under Pressure", 50, OpponentHasThreePrizesOrFewer));
            r.RegisterByKey("SHF-32", new LuxioSHF());
            r.RegisterByKey("SHF-33", new RaidMon());
            r.RegisterByKey("SHF-34", new RotomSHF());
            r.RegisterByKey("SHF-35", new ComboMon(new DrawAttackMon("Famished", 1), new ParalyzeFlipMon("Thunder Shock")));
            r.RegisterByKey("SHF-37", new ComboMon(new Cards30CC.BenchHitMon("Spark", 20), new ElectroWheelMon()));
            r.RegisterByKey("SHF-38", new CardsMEW1.SpreadBenchMon("Max Discharge", 20));
            r.RegisterByKey("SHF-SV39", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("SHF-SV40", new ComboMon(new NoRetreatMon("Big Bite"), new CardsFST2.ConditionBonusMon("Fighting Fangs", 90, DefendingIsVOrGx)));
            r.RegisterByKey("SHF-SV42", new ComboMon(new RiskTakerMon(), new RecoilMon("Thunder Jolt", 30)));
            r.RegisterByKey("SHF-SV43", new ShockingNeedlesMon());
            r.RegisterByKey("SHF-SV44", new ComboMon(new CardsPOR.MemoryLockMon("Torment"), new Cards30CC.BenchHitMon("Spark", 20)));
            r.RegisterByKey("SHF-SV45", new ComboMon(new CardsASC.NextTurnBoostMon("Amping Up", 90), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("SHF-SV46", new ArctozoltSHF());
            r.RegisterByKey("SHF-SV112", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned), new CardsFST2.ConditionBonusMon("Electric Riot", 90, DefendingPoisoned)));
            r.RegisterByKey("SHF-SV113", new CardsFST2.ConditionBonusMon("G-Max Riot", 80, DefendingPoisoned));
            // Psychic
            r.RegisterByKey("SHF-39", new IndeedeeV());
            r.RegisterByKey("SHF-64", new ComboMon(new HealEachMon("Sugary Sprinkles", 30, true), new CardsEVS1.DefenderCantAttackMon("Sweet Splash", IsBasicPokemonCard)));
            r.RegisterByKey("SHF-73", new ComboMon(new AdornmentMon(), new GMaxWhiskMon()));
            r.RegisterByKey("SHF-SV47", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("SHF-SV48", new RapidashSHF());
            r.RegisterByKey("SHF-SV49", new CardsLOR1.CountersAnyWayMon("Cursed Drop", 3));
            r.RegisterByKey("SHF-SV50", new CursolaSHF());
            r.RegisterByKey("SHF-SV51", MadParty());
            r.RegisterByKey("SHF-SV52", new CardsLOR1.FurtiveDropMon(2));
            r.RegisterByKey("SHF-SV53", new PolteageistSHF());
            r.RegisterByKey("SHF-SV54", new SearchAttackMon("Find a Friend", IsPokemonCard, 1));
            r.RegisterByKey("SHF-SV55", new ComboMon(new CardsTWM1.HealSelfMon("Calm Mind", 30), new FlipBonusMon("Brutal Swing", 30)));
            r.RegisterByKey("SHF-SV56", new HattereneSHF());
            r.RegisterByKey("SHF-SV57", new HealEachMon("Aromatherapy", 10, false));
            r.RegisterByKey("SHF-SV58", new ComboMon(new DecorateMon(), new CardsTWM1.HealSelfMon("Draining Kiss", 30)));
            r.RegisterByKey("SHF-SV59", new ComboMon(new HealEachMon("Replenish Time", 30, false), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("SHF-SV60", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("SHF-SV61", new SummonMon());
            r.RegisterByKey("SHF-SV62", new DragapultSHF());
        }

        // ================================================================== Filters, counters and shared scripts

        internal static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        internal static bool IsEnergyCard(CardDef d) { return d.IsEnergy; }
        private static bool IsBasicPokemonCard(CardDef d) { return d.IsBasicPokemon; }
        private static bool OpponentHasThreePrizesOrFewer(EffectContext ctx) { return ctx.Opp.Prizes.Count <= 3; }

        private static bool DefendingIsVOrGx(EffectContext ctx)
        {
            var d = ctx.Defending;
            return d != null && CardsEVS1.IsVOrGx(d.Def);
        }

        private static bool DefendingPoisoned(EffectContext ctx)
        {
            var d = ctx.Defending;
            return d != null && d.Has(SpecialCondition.Poisoned);
        }

        /// <summary>The Pokémon in your discard pile that have the Mad Party attack.</summary>
        internal static int MadPartyInDiscard(EffectContext ctx)
        {
            return ctx.Me.Discard.Count(c => HasMadParty(c.Def));
        }

        internal static bool HasMadParty(CardDef d) { return d.IsPokemon && d.Attacks.Any(a => a.Name == "Mad Party"); }

        /// <summary>Mad Party: 20 damage for each Pokémon in your discard pile that has the Mad Party attack.</summary>
        internal static Mon MadParty() { return new CardsASR1.TimesMon("Mad Party", MadPartyInDiscard); }

        /// <summary>
        /// Sky Circus (Rowlet, Swanna): if you played Bird Keeper from your hand during this turn, ignore all Energy in this
        /// Pokémon's attack costs. An Ability, so it's off while Abilities are blocked.
        /// </summary>
        internal sealed class SkyCircusMon : Mon
        {
            private readonly Mon attacks;

            public SkyCircusMon(Mon attacks) { this.attacks = attacks; }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !src.IsMyTurn) return;
                if (src.Me.SupporterNameThisTurn == "Bird Keeper") cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Woodland Stroll / Ocean Search: once during your turn, if this Pokémon is Active, look at the top 6 cards of your deck,
        /// reveal a card of the kind there and put it into your hand, then shuffle the other cards back.
        /// </summary>
        private sealed class ActiveLookMon : Mon
        {
            private readonly string ability;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;
            private readonly Mon attacks;

            public ActiveLookMon(string ability, Func<CardDef, bool> filter, string what, Mon attacks)
            {
                this.ability = ability;
                this.filter = filter;
                this.what = what;
                this.attacks = attacks;
            }

            protected override string Activated { get { return ability; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(6);
                var fits = top.Where(c => filter(c.Def)).ToList();
                if (fits.Count > 0)
                {
                    var picks = ctx.ChooseCards(fits, 0, 1, ability + ": reveal " + what + " and put it into your hand");
                    RecifeTrainers.TakeIntoHand(ctx, picks);
                }
                if (top.Count > 0) ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Put N [T] Energy attached to this Pokémon into your hand" after the damage (Aurora Loop, Ocean Loop, Spiky Knuckle).</summary>
        internal sealed class EnergyToHandMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public EnergyToHandMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = me.Energy.Where(c => c.Def.EnergyProvides == type).ToList();
                if (energy.Count == 0) return true;
                var picks = energy.Count <= count ? energy : ctx.ChooseCards(energy, count, count, attack + ": choose " + count + " " + type + " Energy to put into your hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                return true;
            }
        }

        /// <summary>"Heal N damage from each of your (Benched) Pokémon" (Sugary Sprinkles, Aromatherapy, Replenish Time).</summary>
        internal sealed class HealEachMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly bool benchOnly;

            public HealEachMon(string attack, int amount, bool benchOnly)
            {
                this.attack = attack;
                this.amount = amount;
                this.benchOnly = benchOnly;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in benchOnly ? ctx.MyBench() : ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, amount);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Attach Leaves: attach up to 2 Grass Energy cards from your discard pile to your Benched Pokémon in any way you like.</summary>
        private sealed class AttachLeavesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Attach Leaves") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var grass = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Grass)).ToList();
                var me = ctx.Me;
                ctx.AttachCardsAnyWay(grass, 2, (card, mon) => me.Bench.Contains(mon), "Attach Leaves", true);
                return true;
            }
        }

        /// <summary>Deep Forest Camo: prevent all damage from attacks by the opponent's Pokémon V and Pokémon-GX. Splitting Arrow: 20 to 2 Benched.</summary>
        private sealed class DecidueyeSHF : Mon
        {
            private static readonly Mon SplittingArrow = new CardsFST1.TwoBenchedMon("Splitting Arrow", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || info.Target != src.Pokemon || a == null || a.Owner == src.Player) return damage;
                return CardsEVS1.IsVOrGx(a.Def) ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SplittingArrow.HandleAttack(ctx, atk); }
        }

        /// <summary>Anchor Anger: 90 more if any of your Grass Pokémon were Knocked Out by damage from an opponent's attack during their last turn.</summary>
        private sealed class AnchorAngerMon : Mon
        {
            private static bool IsGrass(CardDef d) { return d.PokemonType == EnergyType.Grass; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Anchor Anger") return false;
                ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutLastOpponentTurn(IsGrass, true) ? 90 : 0));
                return true;
            }
        }

        /// <summary>Swinging Chain: 30 damage to 1 of the opponent's Pokémon for each Grass Energy attached to this Pokémon.</summary>
        private sealed class SwingingChainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swinging Chain") return false;
                int grass = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                if (grass > 0) ctx.DamageAnyOpponent(30 * grass);
                else ctx.Log("Swinging Chain: no Grass Energy, so no damage.");
                return true;
            }
        }

        /// <summary>Lay of the Land: if you have a Stadium in play, this Pokémon has no Retreat Cost.</summary>
        private sealed class ThwackeySHF : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && CardsPAL1.HaveStadium(src) ? cost - 100 : cost;
            }
        }

        /// <summary>Voltage Beat: once during your turn, search your deck for up to 2 Grass Energy cards and attach them to 1 of your Pokémon.</summary>
        private sealed class RillaboomSHF : Mon
        {
            protected override string Activated { get { return "Voltage Beat"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Grass), 2, "Voltage Beat: choose up to 2 Grass Energy cards");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Voltage Beat: attach them to which of your Pokémon?");
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Bug's Radar: once during your turn, look at the top 3 cards of the opponent's deck and put them back in any order. Brainwave: 30 more per Psychic Energy.</summary>
        private sealed class OrbeetleSHF : Mon
        {
            private static readonly Mon Brainwave = new CardsMEG.EnergyBonusMon("Brainwave", EnergyType.Psychic, 30);

            protected override string Activated { get { return "Bug's Radar"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { CardsDRI2.ReorderOpponentTop(ctx, 3, "Bug's Radar"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Brainwave.HandleAttack(ctx, atk); }
        }

        /// <summary>Blessing of Fluff: search your deck for up to 3 Grass Energy cards and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class BlessingOfFluffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blessing of Fluff") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Grass), 3, "Blessing of Fluff: choose up to 3 Grass Energy cards");
                var me = ctx.Me;
                ctx.AttachCardsAnyWay(found, found.Count, (card, mon) => me.Bench.Contains(mon), "Blessing of Fluff", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Delicious Aroma: once during your turn, flip a coin; on heads, switch 1 of the opponent's Benched Basic Pokémon with their Active Pokémon.</summary>
        private sealed class AppletunSHF : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.GustTargets().Where(m => m.Def.IsBasicPokemon).ToList(); }

            protected override string Activated { get { return "Delicious Aroma"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Delicious Aroma: switch in which of your opponent's Benched Basic Pokémon?");
                ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Forest Feast: put up to 2 Basic Grass Pokémon from your deck onto your Bench.</summary>
        private sealed class ForestFeastMon : Mon
        {
            private static bool IsGrass(CardDef d) { return d.PokemonType == EnergyType.Grass; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Forest Feast") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(IsGrass, 2, "Forest Feast: put up to 2 Basic Grass Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>Max Beating: you may discard up to 3 Grass Energy from this Pokémon for 50 more damage each.</summary>
        private sealed class MaxBeatingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Beating") return false;
                var me = ctx.Pokemon;
                var grass = me.Energy.Where(c => c.Def.EnergyProvides == EnergyType.Grass).ToList();
                int discarded = 0;
                if (grass.Count > 0)
                {
                    var picks = ctx.ChooseCards(grass, 0, Math.Min(3, grass.Count), "Max Beating: discard up to 3 Grass Energy (50 more damage each)");
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(me, e)) discarded++;
                }
                ctx.DamageDefending(atk.Damage + 50 * discarded);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Field Runner: if a Stadium is in play, this Pokémon has no Retreat Cost.</summary>
        private sealed class FieldRunnerMon : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.State.Stadium != null ? cost - 100 : cost;
            }
        }

        /// <summary>Libero: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may attach up to 2 Fire Energy cards from your discard pile to it. Flare Striker: discard 2 Energy.</summary>
        private sealed class CinderaceSHF : Mon
        {
            private static readonly Mon FlareStriker = new CardsTWM1.SelfDiscardMon("Flare Striker", 2);

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null) return;
                var fire = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (!TriggerOnce(ctx, "Libero", fire.Count > 0, "Use Libero? Attach up to 2 Fire Energy cards from your discard pile to " + me.Name + ".")) return;
                ctx.AttachCardsAnyWay(fire, 2, (card, mon) => mon == me, "Libero", true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FlareStriker.HandleAttack(ctx, atk); }
        }

        /// <summary>Hundred Foot Flames: discard the top card of the opponent's deck for each Fire Energy attached to this Pokémon.</summary>
        private sealed class HundredFootFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hundred Foot Flames") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int fire = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fire);
                if (fire > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, fire);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Shuffle Dance: once during your turn, switch 1 of the opponent's face-down Prize cards with the top card of their
        /// deck (both stay face down). Mad Party: 20 for each Mad Party Pokémon in your discard pile.
        /// </summary>
        private sealed class MrRimeSHF : Mon
        {
            private static readonly Mon Party = MadParty();

            protected override string Activated { get { return "Shuffle Dance"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var opp = ctx.Opp;
                return opp.Deck.Count > 0 && opp.Prizes.Any(c => !opp.FaceUpPrizeIds.Contains(c.Id));
            }

            protected override void Ability(EffectContext ctx)
            {
                var opp = ctx.Opp;
                if (opp.Deck.Count == 0) return;
                var slots = opp.PrizeSlotList();
                var opts = new List<DecisionOption>();
                for (int k = 0; k < opp.Prizes.Count; k++)
                {
                    if (opp.FaceUpPrizeIds.Contains(opp.Prizes[k].Id)) continue;
                    opts.Add(new DecisionOption { Label = "Prize card " + (slots[k] + 1), Index = k });
                }
                if (opts.Count == 0) return;
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Shuffle Dance: choose 1 of your opponent's face-down Prize cards", opts, ctx.Source);
                int idx = opts[pick].Index;
                var prize = opp.Prizes[idx];
                var top = opp.Deck[0];
                int slot = slots[idx];
                opp.Prizes[idx] = top;
                opp.Deck[0] = prize;
                opp.PrizeSlotByCard.Remove(prize.Id);
                opp.PrizeSlotByCard[top.Id] = slot;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched 1 of " + ctx.Engine.Their(ctx.Opponent) + " Prize cards with the top card of " + ctx.Engine.Their(ctx.Opponent) + " deck (Shuffle Dance).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Party.HandleAttack(ctx, atk); }
        }

        /// <summary>Shady Dealings: when you play this Pokémon from your hand to evolve, you may search your deck for N Trainer cards.</summary>
        private sealed class ShadyDealingsMon : Mon
        {
            private readonly int count;
            private readonly Mon attacks;

            public ShadyDealingsMon(int count, Mon attacks)
            {
                this.count = count;
                this.attacks = attacks;
            }

            private static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                string what = count == 1 ? "a Trainer card" : "up to " + count + " Trainer cards";
                if (!TriggerOnce(ctx, "Shady Dealings", ctx.Me.Deck.Count > 0, "Use Shady Dealings? Search your deck for " + what + ".")) return;
                ctx.SearchToHand(IsTrainerCard, count, "Shady Dealings: choose " + what);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral Jet: discard 2 Water Energy cards from your hand; if you don't, this attack does nothing.</summary>
        private sealed class SpiralJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Jet") return false;
                var water = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count < 2) return Nothing(ctx);
                var picks = water.Count == 2 ? water : ctx.ChooseCards(water, 2, 2, "Spiral Jet: discard 2 Water Energy cards from your hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Ice Face: at full HP this Pokémon takes 60 less damage from the opponent's attacks. Blizzard: 10 to each Benched.</summary>
        private sealed class EiscueSHF : Mon
        {
            private static readonly Mon Blizzard = new CardsMEW1.SpreadBenchMon("Blizzard", 10);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return me.DamageCounters == 0 ? damage - 60 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blizzard.HandleAttack(ctx, atk); }
        }

        /// <summary>Primal Law: while this Pokémon is Active, the opponent can't play Pokémon from their hand to evolve their Pokémon.</summary>
        private sealed class DracovishSHF : Mon
        {
            public override bool BlocksPlayingPokemon(EffectContext src, int player, CardDef card)
            {
                return player != src.Player && src.Pokemon != null && src.PokemonIsActive && card != null && !card.IsBasicPokemon;
            }
        }

        /// <summary>Body Surf: attach a Water Energy card from your hand to this Pokémon; if you do, switch it with 1 of your Benched Pokémon.</summary>
        private sealed class BodySurfMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Body Surf") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var water = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (!ctx.InPlay(me) || water.Count == 0) return true;
                var card = water.Count == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Body Surf: attach which Water Energy card?")[0];
                ctx.Me.Hand.Remove(card);
                if (ctx.Engine.AttachEnergyCard(card, me, true) && ctx.InPlay(me)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Top Entry: if you drew this Pokémon at the start of your turn and your Bench isn't full, you may put it onto your Bench.</summary>
        private sealed class LuxioSHF : Mon
        {
            public override void OnDrawnForTurn(EffectContext ctx) { TopEntry(ctx); }
        }

        /// <summary>Top Entry (also Lombre and Lampent, Rebel Clash): offered right after the card is drawn for the turn, first turn included.</summary>
        internal static void TopEntry(EffectContext ctx)
        {
            if (ctx.BenchSpace <= 0 || ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def)) return;
            if (!ctx.AskYesNo("Top Entry: you drew " + ctx.Card.Def.Name + ". Put it onto your Bench?")) return;
            if (!ctx.Me.Hand.Remove(ctx.Card)) return;
            ctx.Engine.PushSource(EffectSourceKind.Ability, ctx.Player, null);
            try
            {
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                if (mon != null)
                    ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s " + ctx.Card.Def.Name + " used its Ability Top Entry.", ctx.Card.Id, mon.Uid);
            }
            finally { ctx.Engine.PopSource(); }
        }

        /// <summary>Raid: 100 more if this Pokémon evolved from Luxio during this turn.</summary>
        private sealed class RaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raid") return false;
                var me = ctx.Pokemon;
                bool fromLuxio = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Luxio";
                ctx.DamageDefending(atk.Damage + (fromLuxio ? 100 : 0));
                return true;
            }
        }

        /// <summary>Roto Choice: when you play this Pokémon from your hand onto your Bench, you may search your deck for up to 2 Items with "Rotom" in their names. Thunder Shock: heads, Paralyzed.</summary>
        private sealed class RotomSHF : Mon
        {
            private static readonly Mon ThunderShock = new ParalyzeFlipMon("Thunder Shock");

            private static bool IsRotomItem(CardDef d) { return d.IsItem && d.Name.Contains("Rotom"); }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Roto Choice", ctx.Me.Deck.Count > 0, "Use Roto Choice? Search your deck for up to 2 Item cards with \"Rotom\" in their names.")) return;
                ctx.SearchToHand(IsRotomItem, 2, "Roto Choice: choose up to 2 Item cards with \"Rotom\" in their names");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ThunderShock.HandleAttack(ctx, atk); }
        }

        /// <summary>Electro Wheel: discard an Energy from this Pokémon; if you do, switch it with 1 of your Benched Pokémon.</summary>
        private sealed class ElectroWheelMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Wheel") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Electro Wheel: discard an Energy from " + me.Name) > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Risk Taker: heads, discard the top 5 cards of the opponent's deck; tails, the top 5 of your own.</summary>
        private sealed class RiskTakerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Risk Taker") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Flip() ? ctx.Opponent : ctx.Player, 5);
                return true;
            }
        }

        /// <summary>Shocking Needles: flip 4 coins, 30 damage for each heads; with at least 2 heads, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class ShockingNeedlesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shocking Needles") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Biting Whirlpool: whenever the opponent attaches an Energy card from their hand to 1 of their Pokémon, 2 damage counters on it (each Arctozolt: your ruling).</summary>
        private sealed class ArctozoltSHF : Mon
        {
            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target == null || target.Owner == src.Player || !src.InPlay(target)) return;
                src.Engine.Emit(GameEventType.UseAbility, src.Player, src.Engine.PlayerName(src.Player) + "'s " + src.Pokemon.Name + ": Biting Whirlpool.", src.Card.Id, src.Pokemon.Uid);
                src.PlaceCounters(target, 2);
            }
        }

        // ================================================================== Psychic

        /// <summary>Watch Over: once during your turn, heal 20 damage from your Active Pokémon. Psychic: 60 more for each Energy on the opponent's Active Pokémon.</summary>
        private sealed class IndeedeeV : Mon
        {
            private static readonly Mon Psychic = new CardsMEG.DefendingEnergyBonusMon("Psychic", 60);

            protected override string Activated { get { return "Watch Over"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0; }
            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Me.Active, 20); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>Adornment: for each of your Benched Pokémon, search your deck for a Psychic Energy card and attach it to that Pokémon.</summary>
        private sealed class AdornmentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Adornment") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int benched = ctx.Me.Bench.Count;
                if (benched == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Psychic), benched, "Adornment: choose up to " + benched + " Psychic Energy cards (1 for each of your Benched Pokémon)");
                var open = ctx.MyBench();
                foreach (var card in found)
                {
                    var target = open.Count == 1 ? open[0] : ctx.ChoosePokemon(open, "Adornment: attach " + card.Def.Name + " to which Benched Pokémon?");
                    open.Remove(target);
                    ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>G-Max Whisk: discard any amount of Energy from your Pokémon; 60 damage for each card discarded.</summary>
        private sealed class GMaxWhiskMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Whisk") return false;
                var all = new List<CardInstance>();
                var holderOf = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy)
                    {
                        all.Add(e);
                        holderOf[e.Id] = mon;
                    }
                int discarded = 0;
                if (all.Count > 0)
                {
                    var picks = ctx.ChooseCards(all, 0, all.Count, "G-Max Whisk: discard any amount of Energy from your Pokémon (60 damage each)");
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(holderOf[e.Id], e)) discarded++;
                }
                if (discarded > 0) ctx.DamageDefending(atk.Damage * discarded);
                else ctx.Log("G-Max Whisk: no Energy was discarded, so no damage.");
                return true;
            }
        }

        /// <summary>Pastel Veil: your Pokémon recover from all Special Conditions and can't be affected by any. Psychic: 30 more per Energy on the opponent's Active.</summary>
        private sealed class RapidashSHF : Mon
        {
            private static readonly Mon Psychic = new CardsMEG.DefendingEnergyBonusMon("Psychic", 30);

            // The engine removes Special Conditions a Pokémon can't have right after each action ("recover").
            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>Perish Body: if this Pokémon is Knocked Out in the Active Spot by damage from an opponent's attack, flip a coin; heads, the Attacking Pokémon is Knocked Out. Corner: can't retreat.</summary>
        private sealed class CursolaSHF : Mon
        {
            private static readonly Mon Corner = new NoRetreatMon("Corner");

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Perish Body");
                if (src.Flip()) src.Engine.KnockOut(attacker, "Perish Body");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Corner.HandleAttack(ctx, atk); }
        }

        /// <summary>Tea Break: discard a Pokémon with the Mad Party attack from your hand to draw 2 cards, once during your turn. Mad Party.</summary>
        private sealed class PolteageistSHF : Mon
        {
            private static readonly Mon Party = MadParty();

            private static List<CardInstance> Fuel(EffectContext ctx) { return ctx.Me.Hand.Where(c => HasMadParty(c.Def)).ToList(); }

            protected override string Activated { get { return "Tea Break"; } }
            protected override bool AbilityReady(EffectContext ctx) { return Fuel(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fuel = Fuel(ctx);
                if (fuel.Count == 0) return;
                var pick = fuel.Count == 1 ? fuel[0] : ctx.ChooseCards(fuel, 1, 1, "Tea Break: discard a Pokémon with the Mad Party attack")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Party.HandleAttack(ctx, atk); }
        }

        /// <summary>Mind Hat: once during your turn, each player discards a card from their hand, the opponent first. Dripping Grudge: 1 damage counter for each Pokémon in your discard pile.</summary>
        private sealed class HattereneSHF : Mon
        {
            protected override string Activated { get { return "Mind Hat"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 || ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count > 0)
                {
                    var theirs = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Mind Hat: discard a card from your hand", ctx.Source);
                    foreach (var c in theirs) ctx.Engine.DiscardFromHand(ctx.Opponent, c);
                }
                if (ctx.Me.Hand.Count > 0)
                {
                    var mine = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Mind Hat: discard a card from your hand");
                    foreach (var c in mine) ctx.Engine.DiscardFromHand(ctx.Player, c);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dripping Grudge") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon);
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Decorate: attach any number of basic Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class DecorateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Decorate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0) ctx.AttachCardsAnyWay(basics, basics.Count, (card, mon) => true, "Decorate", true);
                return true;
            }
        }

        /// <summary>Summon: search your deck for a Dreepy and put it onto your Bench.</summary>
        private sealed class SummonMon : Mon
        {
            private static bool IsDreepy(CardDef d) { return d.Name == "Dreepy"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Summon") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(IsDreepy, 1, "Summon: put a Dreepy onto your Bench");
                return true;
            }
        }

        /// <summary>Infiltrator: if any damage is done to this Pokémon by attacks, flip a coin; heads, prevent it. Phantom Force: 3 damage counters on the opponent's Bench in any way.</summary>
        private sealed class DragapultSHF : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && damage > 0 && src.Engine.FlipCoin(src.Player, "Infiltrator (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Phantom Force") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(3, () => ctx.OpponentBench());
                return true;
            }
        }
    }
}
