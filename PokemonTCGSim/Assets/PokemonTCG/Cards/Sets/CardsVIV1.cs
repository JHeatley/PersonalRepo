using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Vivid Voltage (VIV, 2020), Pokémon 1-82 and the full-art Alakazam V (172): Grass, Fire, Water, Lightning and Psychic.
    /// Fighting through Colorless, the Trainers and the Special Energy are in CardsVIV2. Regulation mark D: Expanded only.
    /// Already in the pool: Orbeetle V / VMAX, Charizard, Pikachu V / VMAX, Zekrom, Dusknoir (Lost Origin's and Brilliant
    /// Stars' Trainer Galleries), Flareon, Vaporeon, Jolteon (Brilliant Stars), Electrode and Snorlax (Chilling Reign) and
    /// Raikou (30th Celebration Classic Collection).
    /// Pokémon whose attacks only do their printed damage need no script (Exeggcute, Seedot, Nuzleaf, Oshawott, Voltorb,
    /// Blitzle, Zebstrika, Joltik, Cottonee).
    /// </summary>
    public static class CardsVIV1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("VIV-1", new SearchAttackMon("Bug Hunch", IsGrassPokemon, 2));
            r.RegisterByKey("VIV-2", new CardsTWM1.HealSelfMon("Shed Skin", 30));
            r.RegisterByKey("VIV-3", new BeedrillVIV());
            r.RegisterByKey("VIV-5", new CardsPOR.MemoryLockMon("Head Crack"));
            r.RegisterByKey("VIV-6", new SwitchSelfMon("U-turn", true));
            r.RegisterByKey("VIV-7", new SwitchSelfMon("U-turn", true));
            r.RegisterByKey("VIV-8", new CardsMEW1.WithdrawMon("Iron Defense"));
            r.RegisterByKey("VIV-9", new ComboMon(new CardsASR1.TimesMon("Energy Press", DefendingEnergy), new AmazingBloomMon()));
            r.RegisterByKey("VIV-12", new ShiftryVIV());
            r.RegisterByKey("VIV-13", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("VIV-14", new NinjaskVIV());
            r.RegisterByKey("VIV-15", new ComboMon(new CardsTWM1.HealSelfMon("Leech Seed", 20), new FlowerBearingMon()));
            r.RegisterByKey("VIV-16", new ComboMon(new CardsMEG.SnipeMon("Linear Attack", 50), new LockAfterAttackMon("Techno Blast", false)));
            r.RegisterByKey("VIV-17", new SynthesisMon());
            r.RegisterByKey("VIV-18", new RecoilMon("Take Down", 30));
            r.RegisterByKey("VIV-19", new CardsSIT1.SpecialLaserMon("Special Anchor", 60));
            r.RegisterByKey("VIV-22", new ComboMon(new NoRetreatMon("Bind Down"), new JungleRisingMon()));
            // Fire
            r.RegisterByKey("VIV-23", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("VIV-24", new CardsBRS1.MillSelfMon("Raging Flames", 3));
            r.RegisterByKey("VIV-27", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("VIV-28", new CardsTWM1.SelfDiscardMon("Bright Flame", 2));
            r.RegisterByKey("VIV-29", new TalonflameV());
            // Water
            r.RegisterByKey("VIV-31", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20));
            r.RegisterByKey("VIV-32", new WailordVIV());
            r.RegisterByKey("VIV-34", new AquaWashMon(1));
            r.RegisterByKey("VIV-35", new CardsFST1.TakeLessMon(30, new AquaWashMon(2)));
            r.RegisterByKey("VIV-36", new ComboMon(new ParalyzeFlipMon("Freezing Headbutt"), new RecoilMon("Frozen Slice", 30)));
            r.RegisterByKey("VIV-37", new CardsMEW1.SpreadBenchMon("Max Whiteout", 30));
            r.RegisterByKey("VIV-38", new NoRetreatMon("Jaw Lock"));
            r.RegisterByKey("VIV-39", new ViseWaveMon());
            r.RegisterByKey("VIV-40", new GulpMissileMon());
            r.RegisterByKey("VIV-41", new FlockMon());
            r.RegisterByKey("VIV-42", new TargetedSkewerMon());
            // Lightning
            r.RegisterByKey("VIV-48", new ThunderSnipeMon());
            r.RegisterByKey("VIV-49", new ComboMon(new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused), new DamagingSparkMon()));
            r.RegisterByKey("VIV-51", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("VIV-52", new ManectricVIV());
            r.RegisterByKey("VIV-56", new ComboMon(new ParalyzeFlipMon("Stun Needle"), new CardsASC.CounterMultiplierMon("Shocking Pursuit")));
            r.RegisterByKey("VIV-57", new TynamoVIV());
            r.RegisterByKey("VIV-58", new ShockingSmashMon());
            r.RegisterByKey("VIV-59", new ComboMon(new BiteMarkMon(), new ElectroSprinklerMon()));
            r.RegisterByKey("VIV-61", new CardsFST2.ConditionBonusMon("Fighting Lightning", 80, DefendingIsVOrGx));
            r.RegisterByKey("VIV-62", new CardsCEC3.SpreadAmountMon("Spinning Fan", 10));
            // Psychic
            r.RegisterByKey("VIV-63", new MiniMetronomeMon());
            r.RegisterByKey("VIV-64", new ClefableVIV());
            r.RegisterByKey("VIV-65", new ComboMon(new CardsLOR1.CountersAnyWayMon("Psypower", 2), new CommandingTailMon()));
            r.RegisterByKey("VIV-66", new ShedinjaVIV());
            r.RegisterByKey("VIV-67", new CardsBKP2.CountersOnActiveMon("Haunt", 1));
            r.RegisterByKey("VIV-68", new BanetteVIV());
            r.RegisterByKey("VIV-69", new FutureSightMon());
            r.RegisterByKey("VIV-70", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("VIV-72", new ComboMon(new AuspiciousToneMon(), new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("VIV-73", new UnamplifiedSoundwaveMon());
            r.RegisterByKey("VIV-74", new CardsBRS1.LivelyFruitMon());
            r.RegisterByKey("VIV-76", new ComboMon(new DrawAttackMon("Triple Draw", 3), new FlyingFuryMon()));
            r.RegisterByKey("VIV-77", new ConditionAttackMon("Tail Trickery", SpecialCondition.Confused));
            r.RegisterByKey("VIV-78", new ComboMon(new GeoHuntMon(), new CardsTWM1.HealSelfMon("Aurora Gain", 30)));
            r.RegisterByKey("VIV-79", new DiancieVIV());
            r.RegisterByKey("VIV-80", new CardsPOR.SweetScentMon("Sweet Scent", 20));
            r.RegisterByKey("VIV-81", new AlcremieVIV());
            r.RegisterByKey("VIV-82", new ComboMon(new MetalArmamentMon(), new CardsFST2.ConditionBonusMon("Amazing Sword", 150, OpponentHasVmax)));
            r.RegisterByKey("VIV-172", new ComboMon(new CardsLOR1.CountersAnyWayMon("Zen Spoon", 3), new CardsASR1.TimesMon("Mind Ruler", OpponentHandSize)));
        }

        // ================================================================== Filters, counters and shared scripts

        private static bool IsGrassPokemon(CardDef d) { return d.IsPokemon && d.PokemonType == EnergyType.Grass; }
        private static int OpponentHandSize(EffectContext ctx) { return ctx.Opp.Hand.Count; }

        private static int DefendingEnergy(EffectContext ctx)
        {
            var d = ctx.Defending;
            return d == null ? 0 : ctx.EnergyCount(d);
        }

        internal static bool DefendingIsVOrGx(EffectContext ctx)
        {
            var d = ctx.Defending;
            return d != null && CardsEVS1.IsVOrGx(d.Def);
        }

        private static bool OpponentHasVmax(EffectContext ctx) { return ctx.OpponentPokemon(m => m.Def.IsVmax).Count > 0; }

        /// <summary>Metal Armament (Zacian, Zamazenta): attach a basic Energy card from your discard pile to this Pokémon.</summary>
        internal sealed class MetalArmamentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Armament") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (ctx.InPlay(me) && basics.Count > 0) ctx.AttachCardsAnyWay(basics, 1, (card, mon) => mon == me, "Metal Armament", false);
                return true;
            }
        }

        /// <summary>"Put N Energy attached to this Pokémon into your hand" after the damage, any type (Loop Cannon).</summary>
        internal sealed class AnyEnergyToHandMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public AnyEnergyToHandMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                var picks = me.Energy.Count <= count ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), count, count, attack + ": choose " + count + " Energy to put into your hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Elusive Master: once during your turn, if this Pokémon is the last card in your hand, you may play it onto your Bench and draw 3 cards.</summary>
        private sealed class BeedrillVIV : Mon
        {
            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Hand.Count == 1 && ctx.Me.Hand[0] == ctx.Card && ctx.BenchSpace > 0
                    && !ctx.Engine.PokemonPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, true);
                ctx.Draw(3);
            }
        }

        /// <summary>Amazing Bloom: for each of your Benched Pokémon, search your deck for a card that evolves from it and evolve it.</summary>
        private sealed class AmazingBloomMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amazing Bloom") return false;
                if (ctx.Me.Deck.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var engine = ctx.Engine;
                foreach (var mon in ctx.MyBench())
                {
                    if (!ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) continue;
                    string name = mon.Name;
                    if (!ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == name)) continue;
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name && !engine.Scripts.For(d).PutIntoPlayOnlyByEffect, 1,
                        "Amazing Bloom: choose a card that evolves from " + name, false);
                    if (found.Count > 0) engine.EvolvePokemon(mon, found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Shifty Substitution: while this Pokémon is Active, each Supporter card in the opponent's hand has the effect "Draw 3
        /// cards" instead (the engine plays it that way; any Supporter can be played: your ruling). Fan Tornado: you may have the
        /// opponent switch their Active Pokémon.
        /// </summary>
        private sealed class ShiftryVIV : Mon
        {
            private static readonly Mon FanTornado = new CardsDRI1.WhirlwindMon("Fan Tornado", true);

            public override bool TurnsOpponentsSupportersIntoDrawThree(EffectContext src) { return src.Pokemon != null && src.PokemonIsActive; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FanTornado.HandleAttack(ctx, atk); }
        }

        /// <summary>Cast-Off Shell: when you play this Pokémon from your hand to evolve, you may put a Shedinja from your deck onto your Bench. Absorb: heal 30.</summary>
        private sealed class NinjaskVIV : Mon
        {
            private static readonly Mon Absorb = new CardsTWM1.HealSelfMon("Absorb", 30);

            private static bool IsShedinja(CardDef d) { return d.IsPokemon && d.Name == "Shedinja"; }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Cast-Off Shell", ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0, "Use Cast-Off Shell? Put a Shedinja from your deck onto your Bench.")) return;
                var found = ctx.SearchMyDeck(IsShedinja, 1, "Cast-Off Shell: put a Shedinja onto your Bench");
                foreach (var c in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Absorb.HandleAttack(ctx, atk); }
        }

        /// <summary>Flower Bearing: heads, the opponent shuffles their Active Pokémon and all attached cards and puts them on the bottom of their deck.</summary>
        private sealed class FlowerBearingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flower Bearing") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var cards = defending.AllCards().ToList();
                ctx.Engine.RemoveFromPlay(defending);
                ctx.State.Rng.Shuffle(cards);
                ctx.Opp.Deck.AddRange(cards);
                ctx.Log(defending.Name + " and all attached cards were put on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Flower Bearing).");
                return true;
            }
        }

        /// <summary>Synthesis: search your deck for a Grass Energy card and attach it to 1 of your Pokémon.</summary>
        internal sealed class SynthesisMon : Mon
        {
            private static bool IsGrassEnergy(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Grass); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synthesis") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsGrassEnergy, 1, "Synthesis: choose a Grass Energy card");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Synthesis: attach " + found[0].Def.Name + " to which of your Pokémon?");
                    ctx.Engine.AttachEnergyCard(found[0], target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Jungle Rising: you may attach up to 2 basic Energy cards from your hand to your Benched Pokémon; heal all damage from each Pokémon that got Energy.</summary>
        private sealed class JungleRisingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jungle Rising") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Me;
                var basics = me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0 || me.Bench.Count == 0) return true;
                var before = new Dictionary<int, int>();
                foreach (var b in me.Bench) before[b.Uid] = b.Energy.Count;
                ctx.AttachCardsAnyWay(basics, 2, (card, mon) => me.Bench.Contains(mon), "Jungle Rising", true);
                foreach (var b in me.Bench.ToList())
                {
                    int had;
                    if (before.TryGetValue(b.Uid, out had) && b.Energy.Count > had && b.DamageCounters > 0) ctx.Heal(b, b.DamageCounters * 10);
                }
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Fast Flight: if you go first, usable on your first turn; discard your hand and draw 6. Bright Wing: discard an Energy.</summary>
        private sealed class TalonflameV : Mon
        {
            private static readonly Mon BrightWing = new CardsTWM1.SelfDiscardMon("Bright Wing", 1);

            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == "Fast Flight"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fast Flight") return BrightWing.HandleAttack(ctx, atk);
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(6);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Water Veil: whenever you attach an Energy card from your hand to this Pokémon, it recovers from all Special Conditions. Hydro Pump.</summary>
        private sealed class WailordVIV : Mon
        {
            private static readonly Mon HydroPump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 40);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target != src.Pokemon || target.Conditions == SpecialCondition.None) return;
                Announce(src, "Water Veil");
                src.Engine.RemoveAllConditions(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HydroPump.HandleAttack(ctx, atk); }
        }

        /// <summary>Aqua Wash: you may put N Energy attached to the opponent's Active Pokémon into their hand.</summary>
        internal sealed class AquaWashMon : Mon
        {
            private readonly int count;

            public AquaWashMon(int count) { this.count = count; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Wash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                string what = count == 1 ? "an Energy" : "up to " + count + " Energy";
                var picks = ctx.ChooseCards(defending.Energy.ToList(), 0, Math.Min(count, defending.Energy.Count), "Aqua Wash: you may put " + what + " attached to " + defending.Name + " into your opponent's hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(defending, e);
                return true;
            }
        }

        /// <summary>Vise Wave: if you played Nessa from your hand this turn, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class ViseWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vise Wave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.SupporterNameThisTurn == "Nessa" && TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Continuous Gulp Missile: discard any number of Arrokuda from your Bench; 60 damage for each.</summary>
        private sealed class GulpMissileMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Continuous Gulp Missile") return false;
                var arrokuda = ctx.MyBench().Where(m => m.Name == "Arrokuda").ToList();
                var picks = arrokuda.Count == 0 ? new List<PokemonInPlay>()
                    : ctx.ChoosePokemonMany(arrokuda, 0, arrokuda.Count, "Continuous Gulp Missile: discard any number of Arrokuda from your Bench (60 damage each)");
                foreach (var fish in picks) ctx.Engine.DiscardPokemonFromPlay(fish);
                if (picks.Count > 0) ctx.DamageDefending(atk.Damage * picks.Count);
                else ctx.Log("Continuous Gulp Missile: no Arrokuda were discarded, so no damage.");
                return true;
            }
        }

        /// <summary>Flock: search your deck for up to 2 Arrokuda and put them onto your Bench.</summary>
        private sealed class FlockMon : Mon
        {
            private static bool IsArrokuda(CardDef d) { return d.Name == "Arrokuda"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flock") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(IsArrokuda, 2, "Flock: put up to 2 Arrokuda onto your Bench");
                return true;
            }
        }

        /// <summary>Targeted Skewer: 20 damage to 1 of the opponent's Benched Pokémon for each damage counter on it.</summary>
        private sealed class TargetedSkewerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Targeted Skewer") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Targeted Skewer: choose 1 of your opponent's Benched Pokémon (20 damage for each damage counter on it)");
                if (target != null && target.DamageCounters > 0) ctx.DamageTarget(target, 20 * target.DamageCounters);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Thunder Snipe: discard all Energy from this Pokémon; 160 damage to 1 of the opponent's Pokémon V or Pokémon-GX.</summary>
        private sealed class ThunderSnipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Snipe") return false;
                ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                var targets = ctx.OpponentPokemon(m => CardsEVS1.IsVOrGx(m.Def));
                if (targets.Count == 0)
                {
                    ctx.Log("Thunder Snipe: your opponent has no Pokémon V or Pokémon-GX.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Thunder Snipe: 160 damage to which Pokémon V or Pokémon-GX?");
                if (target != null) ctx.DamageTarget(target, 160);
                return true;
            }
        }

        /// <summary>Damaging Spark: also 30 damage to each of the opponent's Benched Pokémon that has damage counters on it.</summary>
        private sealed class DamagingSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damaging Spark") return false;
                var hurt = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var hit in hurt) if (ctx.InPlay(hit) && !ctx.IsActive(hit)) ctx.DamageTarget(hit, 30);
                return true;
            }
        }

        /// <summary>High Speed: when you play this Pokémon from your hand to evolve, you may draw 3 cards.</summary>
        private sealed class ManectricVIV : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "High Speed", ctx.Me.Deck.Count > 0, "Use High Speed? Draw 3 cards.")) return;
                ctx.Draw(3);
            }
        }

        /// <summary>Levitate: if this Pokémon has any Energy attached, it has no Retreat Cost.</summary>
        private sealed class TynamoVIV : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count > 0 ? cost - 100 : cost;
            }
        }

        /// <summary>Shocking Smash: heads, discard an Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class ShockingSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shocking Smash") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0 || !ctx.Flip()) return true;
                var target = ctx.ChoosePokemon(targets, "Shocking Smash: discard an Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Shocking Smash: discard an Energy from " + target.Name);
                return true;
            }
        }

        /// <summary>Electrified Bite Mark: if the opponent attaches an Energy card from their hand to the Defending Pokémon during their next turn, 6 damage counters on it.</summary>
        private sealed class BiteMarkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electrified Bite Mark") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsMEG.EnergyTrapEffect(defending.Name + " gets 6 damage counters if Energy is attached to it from the hand during its next turn (Electrified Bite Mark)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 6));
                return true;
            }
        }

        /// <summary>Electro Sprinkler: also 30 damage to 1 of your Benched Pokémon and 30 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class ElectroSprinklerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Sprinkler") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0)
                {
                    var mine = ctx.ChoosePokemon(ctx.MyBench(), "Electro Sprinkler: 30 damage to which of your Benched Pokémon?");
                    if (mine != null) ctx.DamageTarget(mine, 30);
                }
                if (ctx.Opp.Bench.Count > 0)
                {
                    var theirs = ctx.ChoosePokemon(ctx.OpponentBench(), "Electro Sprinkler: 30 damage to which of your opponent's Benched Pokémon?");
                    if (theirs != null) ctx.DamageTarget(theirs, 30);
                }
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Mini-Metronome: heads, use 1 of the opponent's Active Pokémon's attacks as this attack.</summary>
        private sealed class MiniMetronomeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mini-Metronome") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, "Mini-Metronome: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                    UseCopiedAttack(ctx, copied, index);
                else ctx.Log("Mini-Metronome: there's no attack to use.");
                return true;
            }
        }

        /// <summary>Lunar Blessing: once during your turn, if your Active Pokémon has Psychic Energy, heal 20 damage from it and it recovers from a Special Condition.</summary>
        private sealed class ClefableVIV : Mon
        {
            private static readonly SpecialCondition[] All = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override string Activated { get { return "Lunar Blessing"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && ctx.Engine.EnergyCountOfType(a, EnergyType.Psychic) > 0 && (a.DamageCounters > 0 || a.Conditions != SpecialCondition.None);
            }

            protected override void Ability(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                if (a == null) return;
                if (a.DamageCounters > 0) ctx.Heal(a, 20);
                var has = All.Where(c => a.Has(c)).ToList();
                if (has.Count == 0) return;
                int pick = has.Count == 1 ? 0 : ctx.ChooseOption("Lunar Blessing: " + a.Name + " recovers from which Special Condition?", has.Select(c => c.ToString()).ToArray());
                ctx.Engine.RemoveCondition(a, has[pick]);
            }
        }

        /// <summary>Commanding Tail: you may have the opponent shuffle their hand into their deck; if you do, they draw 4.</summary>
        private sealed class CommandingTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Commanding Tail") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.AskYesNo("Commanding Tail: have your opponent shuffle their hand into their deck and draw 4 cards?")) return true;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
                return true;
            }
        }

        /// <summary>Shell Survival: only Ninjask's Cast-Off Shell puts this Pokémon into play. Life Squeeze: damage counters until the opponent's Active Pokémon has 10 HP left.</summary>
        private sealed class ShedinjaVIV : Mon
        {
            public override bool PutIntoPlayOnlyByEffect { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Life Squeeze") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int counters = (ctx.RemainingHp(defending) - 10) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                else ctx.Log("Life Squeeze: " + defending.Name + " already has 10 HP or less left.");
                return true;
            }
        }

        /// <summary>Curse of Devolution: when you play this Pokémon from your hand to evolve, you may devolve 1 of the opponent's evolved Benched Pokémon (the top card goes to their hand).</summary>
        private sealed class BanetteVIV : Mon
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.OpponentBench().Where(m => m.Stack.Count >= 2 && !m.Def.IsLegend).ToList(); }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Curse of Devolution", Targets(ctx).Count > 0, "Use Curse of Devolution? Devolve 1 of your opponent's evolved Benched Pokémon.")) return;
                var targets = Targets(ctx);
                var target = ctx.ChoosePokemon(targets, "Curse of Devolution: devolve which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.Devolve(target, 1);
            }
        }

        /// <summary>Future Sight: look at the top 4 cards of either player's deck and put them back in any order.</summary>
        internal sealed class FutureSightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Future Sight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool mine = ctx.Me.Deck.Count > 0;
                bool theirs = ctx.Opp.Deck.Count > 0;
                if (!mine && !theirs) return true;
                bool useMine = mine && (!theirs || ctx.ChooseOption("Future Sight: look at the top 4 cards of which deck?", "Your deck", "Your opponent's deck") == 0);
                if (useMine) CardsOBF2.ReorderOwnTop(ctx, 4, "Future Sight");
                else CardsDRI2.ReorderOpponentTop(ctx, 4, "Future Sight");
                return true;
            }
        }

        /// <summary>Auspicious Tone: search your deck for a Pokémon and a Supporter card.</summary>
        private sealed class AuspiciousToneMon : Mon
        {
            private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
            private static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Auspicious Tone") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsPokemonCard, 1, "Auspicious Tone: choose a Pokémon");
                found.AddRange(ctx.SearchMyDeck(IsSupporterCard, 1, "Auspicious Tone: choose a Supporter card"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Unamplified Soundwave: with any cards in your hand it does nothing; otherwise 30 damage to each of the opponent's Pokémon.</summary>
        private sealed class UnamplifiedSoundwaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Unamplified Soundwave") return false;
                if (ctx.Me.Hand.Count > 0) return Nothing(ctx);
                foreach (var m in ctx.OpponentPokemon())
                {
                    if (!ctx.InPlay(m)) continue;
                    if (ctx.IsActive(m)) ctx.DamageDefending(30);
                    else ctx.DamageTarget(m, 30);
                }
                return true;
            }
        }

        /// <summary>Flying Fury: before doing damage, you may discard any number of Pokémon Tools from your Pokémon; 40 more for each.</summary>
        private sealed class FlyingFuryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flying Fury") return false;
                var tools = new List<CardInstance>();
                var holderOf = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var t in mon.Tools)
                    {
                        tools.Add(t);
                        holderOf[t.Id] = mon;
                    }
                int discarded = 0;
                if (tools.Count > 0)
                {
                    var picks = ctx.ChooseCards(tools, 0, tools.Count, "Flying Fury: discard any number of Pokémon Tools from your Pokémon (40 more damage each)");
                    foreach (var t in picks) if (ctx.Engine.DiscardTool(holderOf[t.Id], t)) discarded++;
                }
                ctx.DamageDefending(atk.Damage + 40 * discarded);
                return true;
            }
        }

        /// <summary>Geo Hunt: put a card from your discard pile into your hand.</summary>
        private sealed class GeoHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Geo Hunt") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Discard.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Geo Hunt: put which card from your discard pile into your hand?");
                ctx.DiscardToHand(pick);
                return true;
            }
        }

        /// <summary>Sparkle Veil: while this Pokémon is Active, your Pokémon take 30 less damage from the opponent's attacks. Sensitive Ray: 70 more after playing a Supporter.</summary>
        private sealed class DiancieVIV : Mon
        {
            private static bool PlayedSupporter(EffectContext ctx) { return ctx.Me.SupporterThisTurn != null; }
            private static readonly Mon SensitiveRay = new CardsFST2.ConditionBonusMon("Sensitive Ray", 70, PlayedSupporter);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                var a = info.Attacker;
                if (src.Pokemon == null || !src.PokemonIsActive || t == null || t.Owner != src.Player || a == null || a.Owner == src.Player) return damage;
                return damage - 30;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SensitiveRay.HandleAttack(ctx, atk); }
        }

        /// <summary>Sharing Sweets: when you play this Pokémon from your hand to evolve, you may have each player draw a card. Wonder Shine: Confused.</summary>
        private sealed class AlcremieVIV : Mon
        {
            private static readonly Mon WonderShine = new ConditionAttackMon("Wonder Shine", SpecialCondition.Confused);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Sharing Sweets", true, "Use Sharing Sweets? Each player draws a card.")) return;
                ctx.Draw(1);
                ctx.Engine.DrawCards(ctx.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WonderShine.HandleAttack(ctx, atk); }
        }
    }
}
