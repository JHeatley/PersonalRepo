using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>Legends Awakened: Uncommons, Commons and Trainers (Rares and LV.X are in CardsLA.cs).</summary>
    public static partial class CardsLA
    {
        private static bool UnownBenched(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

        private static void RegisterUncommonsAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("LA-46", new ComboMon(new CardsASC.SelfGuardMon("Guard Claw", 20), new FlipBonusMon("X-Scissor", 20)));
            r.RegisterByKey("LA-47", new ComboMon(new EruptionMon(), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("LA-48", new FormChangeMon("Castform", "Temperament", new WeatherBallMon()));
            r.RegisterByKey("LA-49", new FormChangeMon("Castform", "Temperament", new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("LA-50", new FormChangeMon("Castform", "Temperament", new CardsSS.EachOpponentMon("Hail", 10)));
            r.RegisterByKey("LA-51", new FormChangeMon("Castform", "Temperament", new CardsASC.SelfDiscardTypeMon("Ember", EnergyType.Fire, 1)));
            r.RegisterByKey("LA-52", new ComboMon(new ParalyzeFlipMon("Wrap"), new FlipBonusMon("Rising Lunge", 20)));
            r.RegisterByKey("LA-53", new ComboMon(new CardsASC.SelfGuardMon("Expand", 10), new CardsDRI1.WhirlwindMon("Whirlwind")));
            r.RegisterByKey("LA-54", new ComboMon(new PsychicStrategyMon(), new SuperEggsplosionMon()));
            r.RegisterByKey("LA-55", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned), new FlipBonusMon("Sharp Tail", 40)));
            r.RegisterByKey("LA-56", new ComboMon(new MagicalStepMon(), new CardsASR1.TimesMon("Grind", c => c.EnergyCount(c.Pokemon))));
            r.RegisterByKey("LA-57", new HoundoomRevengeFang());
            r.RegisterByKey("LA-58", new ComboMon(new RushingWaterMon(), new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("LA-59", new ComboMon(new EnergySplitMon(), new CardsASR1.PlusMon("Aqua Bolt", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water))));
            r.RegisterByKey("LA-60", new ComboMon(new CoinsMon("Comet Punch", 4), new CardsSW.BatonPassMon("Baton Pass")));
            r.RegisterByKey("LA-61", new ComboMon(new CardsSSB.AnyOpponentTextMon("Focus Blast", 30), new CardsFST2.ConditionBonusMon("Spike Lariat", 20, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("LA-62", new ComboMon(new Cards30CC.BenchHitMon("Spark", 10), new FlipTailsDiscardAllMon("Thunderous Claw", EnergyType.Lightning)));
            r.RegisterByKey("LA-63", new ComboMon(new HeavyBoneMon(), new CardsPFL.FlipUntilTailsTimesMon("Bone Rush")));
            r.RegisterByKey("LA-64", new DodgeMon("Agility", false));
            r.RegisterByKey("LA-65", new ComboMon(new ParalyzeFlipMon("Psyshock"), new CardsRSB.CoinsPlusMon("Bullet Punch", 2, 20)));
            r.RegisterByKey("LA-66", new ComboMon(new BiteOnMon(), new CardsASR1.TimesMon("Harass", c => c.MyPokemon().Count)));
            r.RegisterByKey("LA-67", new CardsDS.EvolutionaryMon("Cast-off Shell", CastOffShell, c => c.BenchSpace > 0 && (c.Me.Hand.Concat(c.Me.Discard).Any(x => x.Def.IsPokemon && x.Def.Name == "Shedinja")), new ChipOffLa()));
            r.RegisterByKey("LA-68", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 30), new FlipBonusMon("Fasten Claws", 10)));
            r.RegisterByKey("LA-69", new ComboMon(new CardsSUM5.OptionalBoostRecoilMon("Charge Dash", 30, 30), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("LA-70", new AquaPumpMon());
            r.RegisterByKey("LA-71", new ComboMon(new StarBoomerangMon(), new CoreFlashMon()));
            r.RegisterByKey("LA-72", new ComboMon(new SwallowUpLaMon(), new ConditionAttackMon("Gunk Shot", SpecialCondition.Poisoned)));
            r.RegisterByKey("LA-73", new SwellowBigWing());
            r.RegisterByKey("LA-74", new ComboMon(new CardsASR1.TimesMon("Herd Charge", c => c.FlipCoins(c.MyPokemon(m => m.Name == "Tauros").Count)), new CardsGRI2.FlipBonusOrRecoilMon("Thrash", 20, 10)));
            r.RegisterByKey("LA-75", new ComboMon(new TentacleStrikeMon(), new DangerousPoisonMon()));
            r.RegisterByKey("LA-76", new UnownJunk());
            r.RegisterByKey("LA-77", new UnownRetire());
            r.RegisterByKey("LA-78", new UnownUnseen());
            r.RegisterByKey("LA-79", new UnownVacation());
            r.RegisterByKey("LA-80", new UnownWall());
            r.RegisterByKey("LA-81", new UnownYawn());
            r.RegisterByKey("LA-82", new UnownQuestion());
            r.RegisterByKey("LA-83", new BeldumMetalChain());
            r.RegisterByKey("LA-84", new DrawAttackMon("Allure", 1));
            r.RegisterByKey("LA-85", new ComboMon(new VineBindMon(), new RecoilMon("Careless Tackle", 10)));
            r.RegisterByKey("LA-86", new ComboMon(new CardsDP.RestMon("Rest", 40), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("LA-87", new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("LA-88", new CardsFST2.ConditionBonusMon("Aqua Spark", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water) > 0));
            r.RegisterByKey("LA-89", new FlipGateMon("Knock Off", new CardsHPB.KnockOffMon("Knock Off", false)));
            r.RegisterByKey("LA-90", new CoinsMon("Bonemerang", 2));
            r.RegisterByKey("LA-91", new CoinsMon("Slam", 2));
            r.RegisterByKey("LA-92", new CardsPAR2.FlipConditionMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("LA-93", new ComboMon(new BenchTypedMon("Call for Family", 2, d => d.HasType(EnergyType.Grass) || d.HasType(EnergyType.Psychic)), new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep)));
            r.RegisterByKey("LA-94", new ComboMon(new LightPoisonMon(), new FlipBonusMon("Jump On", 10)));
            r.RegisterByKey("LA-95", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("LA-96", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new ChooseOpponentMon("Frustration", 30, m => m.DamageCounters == 0)));
            r.RegisterByKey("LA-97", new ComboMon(new SpaceOutMon(), new CardsCES1.FlipConditionsMon("Stinky Nectar", SpecialCondition.Confused, SpecialCondition.Poisoned)));
            r.RegisterByKey("LA-98", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new ParalyzeFlipMon("Gastro Acid")));
            r.RegisterByKey("LA-99", new ComboMon(new CardsMT.SpikeArmorMon("Counter Punch", 4), new GutBaseMon("Gut Strike", 60)));
            r.RegisterByKey("LA-100", new GutKickMon());
            r.RegisterByKey("LA-101", new ComboMon(new CoinsMon("Triple Kick", 3), new GutSpinMon()));
            r.RegisterByKey("LA-102", new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"), new SwitchSelfMon("Reverse Thrust", false)));
            r.RegisterByKey("LA-103", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("LA-104", new ComboMon(new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused), new RecoilMon("Double-edge", 10)));
            r.RegisterByKey("LA-105", new ComboMon(new AstonishMon("Astonish"), new CardsE3B.HealSelfMon("Absorb", 20)));
            r.RegisterByKey("LA-106", new ComboMon(new PayDayMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("LA-107", new ComboMon(new SearchAttackMon("Show Off", d => d.IsBasicEnergy, 1), new CardsFST2.ConditionBonusMon("Payback", 20, c => c.Opp.Prizes.Count == 1)));
            r.RegisterByKey("LA-108", new NoRetreatMon("Big Bite"));
            r.RegisterByKey("LA-109", new CardsFST2.ConditionBonusMon("Nose Poke", 20, c => c.Me.Bench.Any(m => m.Name == "Probopass")));
            r.RegisterByKey("LA-110", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("LA-111", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new CardsE3B.HealSelfMon("Blot", 10)));
            r.RegisterByKey("LA-112", new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("LA-113", new CardsBS1.SelfdestructMon("Selfdestruct", 10, 50));
            r.RegisterByKey("LA-114", new CardsSI.HeadsTailsConditionMon("Twiddle", SpecialCondition.Confused, SpecialCondition.Asleep));
            r.RegisterByKey("LA-115", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("LA-116", new ComboMon(new NoRetreatMon("Corner"), new FlipBonusMon("Ambush", 20)));
            r.RegisterByKey("LA-117", new PlainMon());
            r.RegisterByKey("LA-118", new ComboMon(new CardsMT.SearchAttachSelfMon("Recharge", d => CardsRS.BasicOf(d, EnergyType.Lightning)), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("LA-119", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("LA-120", new RobMon());
            r.RegisterByKey("LA-121", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new SwitchSelfMon("Bounce", true)));
            r.RegisterByKey("LA-122", new StaryuRecoverMon());
            r.RegisterByKey("LA-123", new SniffOutMon());
            r.RegisterByKey("LA-124", new PeckMon());
            r.RegisterByKey("LA-125", new ComboMon(new DodgeMon("Crystal Barrier", false), new CardsASC.FlipDiscardDefendingEnergyMon("Mysterious Beam")));
            r.RegisterByKey("LA-126", new CardsUF.BabyEvolutionAnyMon(new[] { "Hitmonlee", "Hitmonchan", "Hitmontop" }, new GutBlowMon()));
            r.RegisterByKey("LA-127", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("LA-128", new ComboMon(new SwitchSelfMon("U-turn", false), new DodgeMon("Agility", false)));

            r.RegisterByKey("LA-129", new BubbleCoat());
            r.RegisterByKey("LA-130", new BucksTraining());
            r.RegisterByKey("LA-131", new CynthiasFeelings());
            r.RegisterByKey("LA-132", new EnergyPickup());
            r.RegisterByKey("LA-133", new PokeRadar());
            r.RegisterByKey("LA-134", new SnowpointTemple());
            r.RegisterByKey("LA-135", new StarkMountain());
            r.RegisterByKey("LA-136", new TechnicalMachineTs(Evoluter));
            r.RegisterByKey("LA-137", new TechnicalMachineTs(Devoluter));
            r.RegisterByKey("LA-138", new CardsASC.CounterattackMon("Jagged Stone", 1, null));
            r.RegisterByKey("LA-139", new CardsSS.RainDishMon("Spongy Stone", null));
        }

        // ------------------------------------------------------------------ Uncommons

        /// <summary>Eruption: each player discards the top card of their deck; 30 more for each Energy card discarded.</summary>
        private sealed class EruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption") return false;
                int energy = 0;
                foreach (var p in new[] { ctx.Player, ctx.Opponent }) energy += ctx.Engine.DiscardTopOfDeck(p, 1).Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 30 * energy);
                return true;
            }
        }

        /// <summary>Weather Ball: with your Stadium in play, remove 3 damage counters from this Pokémon; with the opponent's, 30 more.</summary>
        private sealed class WeatherBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Weather Ball") return false;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                if (mine && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
                ctx.DamageDefending(atk.Damage + (theirs ? 30 : 0));
                return true;
            }
        }

        /// <summary>Psychic Strategy: each player shuffles their hand into their deck, then draws up to as many cards as the other player had (you first).</summary>
        private sealed class PsychicStrategyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Strategy") return false;
                int mine = ctx.Me.Hand.Count, theirs = ctx.Opp.Hand.Count;
                foreach (var c in ctx.Me.Hand.ToList()) { ctx.Me.Hand.Remove(c); ctx.Me.Deck.Add(c); }
                ctx.ShuffleMyDeck();
                foreach (var c in ctx.Opp.Hand.ToList()) { ctx.Opp.Hand.Remove(c); ctx.Opp.Deck.Add(c); }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                if (theirs > 0) ctx.Draw(theirs);
                if (mine > 0) ctx.Engine.DrawCards(ctx.Opponent, mine);
                return true;
            }
        }

        /// <summary>Super Eggsplosion: discard any Energy cards from your Pokémon; a coin for each; 50 times the heads.</summary>
        private sealed class SuperEggsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Eggsplosion") return false;
                int n = 0;
                foreach (var m in ctx.MyPokemon(p => p.Energy.Count > 0))
                {
                    var picks = ctx.ChooseCards(m.Energy.ToList(), 0, m.Energy.Count, "Super Eggsplosion: discard which Energy cards from " + m.Name + "?");
                    foreach (var e in picks) ctx.Engine.DiscardEnergy(m, e);
                    n += picks.Count;
                }
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(n));
                return true;
            }
        }

        /// <summary>Magical Step: the Defending Pokémon is Confused, with 6 damage counters instead of 3 for Confusion.</summary>
        private sealed class MagicalStepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Step") return false;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending) && ctx.ApplyCondition(defending, SpecialCondition.Confused)) defending.ConfusionCounters = 6;
                return true;
            }
        }

        /// <summary>Revenge Fang (Poké-Body): with fewer Benched Pokémon than the opponent, Houndoom's attacks do 40 more to the Active Pokémon (before W/R). Burning Sensation, Bright Flame.</summary>
        private sealed class HoundoomRevengeFang : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsCG.IfDefendingConditionMon("Burning Sensation", SpecialCondition.Burned, m => m.DamageCounters > 0), new FlipTailsDiscardTypeTwo("Bright Flame", EnergyType.Fire));

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Me.Bench.Count < src.Opp.Bench.Count ? damage + 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Flip a coin. If tails, discard 2 [type] Energy attached to this Pokémon" (Bright Flame).</summary>
        private sealed class FlipTailsDiscardTypeTwo : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public FlipTailsDiscardTypeTwo(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, attack + ": discard 2 " + type + " Energy", e => CardsDRI1.ProvidesType(ctx, me, e, type));
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, discard all [type] Energy attached to this Pokémon" (Thunderous Claw).</summary>
        private sealed class FlipTailsDiscardAllMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public FlipTailsDiscardAllMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(me))
                    foreach (var e in me.Energy.Where(x => CardsDRI1.ProvidesType(ctx, me, x, type)).ToList()) ctx.Engine.DiscardEnergy(me, e);
                return true;
            }
        }

        /// <summary>Rushing Water: move an Energy card from the Defending Pokémon to another of the opponent's Pokémon.</summary>
        private sealed class RushingWaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rushing Water") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Rushing Water: move which Energy card?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Rushing Water: move it to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>Energy Split: 30 to each of the opponent's Pokémon with Energy attached (no W/R on the Bench).</summary>
        private sealed class EnergySplitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Split") return false;
                foreach (var m in ctx.OpponentPokemon(p => p.Energy.Count > 0)) ctx.DamageTarget(m, 30);
                return true;
            }
        }

        /// <summary>Heavy Bone: tails, it can't be used during your next turn.</summary>
        private sealed class HeavyBoneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Bone") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn("Heavy Bone");
                return true;
            }
        }

        /// <summary>Bite On: a non-Evolved Defending Pokémon can't attack, retreat or use Poké-Powers next turn.</summary>
        private sealed class BiteOnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bite On") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || CardsE2.IsEvolved(defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new HeldEffect(defending.Name + " can't attack, retreat or use Poké-Powers next turn (Bite On)", ctx.Player, defending.Uid, ctx.Turn + 1, true, true));
                return true;
            }
        }

        /// <summary>On a Pokémon until the end of the next turn: no attacks / retreat (optional) and no Poké-Powers.</summary>
        private sealed class HeldEffect : ContinuousEffect
        {
            private readonly bool attacks;

            public HeldEffect(string name, int owner, int targetUid, int expiresAfterTurn, bool attacks, bool endsWhenMoved) : base(name, owner, targetUid, expiresAfterTurn, endsWhenMoved) { this.attacks = attacks; }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return attacks && mon != null && mon.Uid == TargetUid; }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return attacks && mon != null && mon.Uid == TargetUid; }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Cast-off Shell: when Ninjask evolves from your hand, put Shedinja from your hand or discard pile onto your Bench as a Basic Pokémon.</summary>
        private static void CastOffShell(EffectContext ctx)
        {
            var cards = ctx.Me.Hand.Concat(ctx.Me.Discard).Where(c => c.Def.IsPokemon && c.Def.Name == "Shedinja").ToList();
            if (cards.Count == 0 || ctx.BenchSpace == 0) return;
            var card = ctx.ChooseCards(cards, 1, 1, "Cast-off Shell: put which Shedinja onto your Bench?")[0];
            if (!ctx.Me.Hand.Remove(card)) ctx.Me.Discard.Remove(card);
            ctx.Engine.PutOntoBench(ctx.Player, card, false);
        }

        /// <summary>Chip Off: with 6 or more cards in the opponent's hand, random cards are discarded until they have 5.</summary>
        private sealed class ChipOffLa : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chip Off") return false;
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < 60 && ctx.Opp.Hand.Count >= 6; i++)
                {
                    var card = CardsHP.RandomOpponentCard(ctx);
                    if (card == null) break;
                    ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                }
                return true;
            }
        }

        /// <summary>Aqua Pump: you may discard up to 2 Water Energy cards from your hand; 10 more for each.</summary>
        private sealed class AquaPumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Pump") return false;
                var water = ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList();
                var picks = water.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(water, 0, Math.Min(2, water.Count), "Aqua Pump: discard up to 2 Water Energy cards from your hand (10 more each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DamageDefending(atk.Damage + 10 * picks.Count);
                return true;
            }
        }

        /// <summary>Star Boomerang: you may return this Pokémon and its cards to your hand.</summary>
        private sealed class StarBoomerangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Boomerang") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Star Boomerang: return " + ctx.Pokemon.Name + " and its cards to your hand?")) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Core Flash: 50 to 1 of the opponent's Pokémon with a Poké-Power or Poké-Body (no W/R on the Bench).</summary>
        private sealed class CoreFlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Flash") return false;
                var targets = ctx.OpponentPokemon(m => m.Def.Abilities.Any(a => a.IsPokePowerOrBody));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Core Flash: do 50 damage to which Pokémon?");
                if (target != null) ctx.DamageTarget(target, 50);
                return true;
            }
        }

        /// <summary>Swallow Up: 80 instead if the Defending Pokémon has fewer HP left than this Pokémon.</summary>
        private sealed class SwallowUpLaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swallow Up") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.RemainingHp(ctx.Defending) < ctx.RemainingHp(ctx.Pokemon) ? 80 : atk.Damage);
                return true;
            }
        }

        /// <summary>Big Wing (Poké-Body): with no Energy attached, Swellow's Retreat Cost is 0. Mach Descent: base 60 if it was on your Bench this turn.</summary>
        private sealed class SwellowBigWing : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return mon != null && mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Descent") return false;
                ctx.DamageDefending(ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? 60 : atk.Damage);
                return true;
            }
        }

        /// <summary>Tentacle Strike: attach up to 2 Energy cards from the opponent's discard pile to their Pokémon in any way; 20 to a Pokémon for each attached to it.</summary>
        private sealed class TentacleStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacle Strike") return false;
                var pile = ctx.Opp.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (pile.Count == 0) return true;
                var hits = new Dictionary<PokemonInPlay, int>();
                foreach (var card in ctx.ChooseCards(pile, 1, Math.Min(2, pile.Count), "Tentacle Strike: attach which Energy cards from your opponent's discard pile?"))
                {
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Tentacle Strike: attach " + card.Def.Name + " to which of your opponent's Pokémon?");
                    if (target == null || !ctx.CanAffect(target)) continue;
                    if (ctx.Opp.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target))
                    {
                        if (!ctx.Opp.Discard.Contains(card)) ctx.Opp.Discard.Add(card);
                        continue;
                    }
                    int n;
                    hits[target] = (hits.TryGetValue(target, out n) ? n : 0) + 20;
                }
                foreach (var kv in hits) if (ctx.InPlay(kv.Key)) ctx.DamageTarget(kv.Key, kv.Value);
                return true;
            }
        }

        /// <summary>Dangerous Poison: if this Pokémon has less Energy than the Defending Pokémon, it's Paralyzed and Poisoned.</summary>
        private sealed class DangerousPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Poison") return false;
                var defending = ctx.Defending;
                bool less = defending != null && ctx.EnergyCount(ctx.Pokemon) < ctx.EnergyCount(defending);
                ctx.DamageDefending(atk.Damage);
                if (!less || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        // ------------------------------------------------------------------ Unown

        /// <summary>JUNK (Poké-Power): when you put Unown J from your hand onto your Bench, heads puts a Trainer card from your discard pile into your hand. Hidden Power: the Defending Pokémon is no longer Asleep.</summary>
        private sealed class UnownJunk : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var pile = ctx.Me.Discard.Where(c => DpTrainer(c.Def)).ToList();
                if (ctx.Engine.DiscardToHandBlocked(ctx.Player) || !TriggerOnce(ctx, "JUNK", pile.Count > 0, "JUNK: flip a coin for a Trainer card from your discard pile?") || !ctx.Flip()) return;
                ctx.DiscardToHand(ctx.ChooseCards(pile, 1, 1, "JUNK: put which Trainer card into your hand?"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) defending.Conditions &= ~SpecialCondition.Asleep;
                return true;
            }
        }

        /// <summary>RETIRE (Poké-Power): once during your turn, if Unown R is on your Bench, discard it and its cards (not a Knock Out) and draw a card. Hidden Power: move basic Energy cards between your Pokémon in any way.</summary>
        private sealed class UnownRetire : Mon
        {
            protected override string Activated { get { return "RETIRE"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownBenched(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
                ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                for (int i = 0; i < 20; i++)
                {
                    var donors = ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy));
                    if (donors.Count == 0 || ctx.Me.InPlay().Count() < 2) break;
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, donors, "Hidden Power: move a basic Energy card from which Pokémon? (Done to stop)", true);
                    if (from == null) break;
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Hidden Power: move it to which Pokémon?");
                    if (to == null) break;
                    var cards = from.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    ctx.Engine.MoveEnergy(from, to, cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Hidden Power: move which Energy card?")[0]);
                    if (!RepeatableAbility.ForPerson(ctx)) break;
                }
                return true;
            }
        }

        /// <summary>UNSEEN (Poké-Body): while Unown U is on your Bench, prevent all effects of the opponent's attacks, damage included, on your Benched Unown. Hidden Power: discard any 1 card from your deck.</summary>
        private sealed class UnownUnseen : Mon
        {
            private static bool Covered(EffectContext src, PokemonInPlay t) { return src.Pokemon != null && UnownBenched(src) && t != null && t.Owner == src.Player && src.Me.Bench.Contains(t) && t.Name.StartsWith("Unown", StringComparison.Ordinal); }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return Covered(src, info.Target) && info.Attacker != null && info.Attacker.Owner != src.Player && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters) { return kind == EffectSourceKind.Attack && Covered(src, target); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, 1, "Hidden Power: discard which card from your deck?")[0];
                ctx.Me.Deck.Remove(card);
                ctx.Me.Discard.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>VACATION (Poké-Power): once during your turn, remove 2 damage counters from each of your Pokémon; your turn ends. Not with a Special Condition. Hidden Power: 10 to each of your Benched Pokémon.</summary>
        private sealed class UnownVacation : Mon
        {
            private static readonly Mon Hidden = new CardsMD_OwnBench("Hidden Power", 10);

            protected override string Activated { get { return "VACATION"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var m in ctx.MyPokemon(p => p.DamageCounters > 0)) ctx.Heal(m, 20);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (VACATION).");
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>"Does N damage to each of your Benched Pokémon" after the printed damage.</summary>
        private sealed class CardsMD_OwnBench : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public CardsMD_OwnBench(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.MyBench().ToList()) ctx.DamageTarget(m, amount);
                return true;
            }
        }

        /// <summary>WALL (Poké-Body): while Unown W is Active, attack damage to your Pokémon is 10 less (after W/R). Hidden Power: draw a card.</summary>
        private sealed class UnownWall : Mon
        {
            private static readonly Mon Hidden = new DrawAttackMon("Hidden Power", 1);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>YAWN (Poké-Power): once during your turn, if Unown Y is on your Bench, remove 1 damage counter from your Active Unown; it's now Asleep. Hidden Power: up to 2 Trainer cards from your deck into your hand.</summary>
        private sealed class UnownYawn : Mon
        {
            private static readonly Mon Hidden = new SearchAttackMon("Hidden Power", DpTrainer, 2);

            protected override string Activated { get { return "YAWN"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownBenched(ctx) && ctx.Me.Active != null && ctx.Me.Active.Name.StartsWith("Unown", StringComparison.Ordinal); }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active.DamageCounters > 0) ctx.Heal(active, 10);
                ctx.ApplyCondition(active, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hidden.HandleAttack(ctx, atk); }
        }

        /// <summary>? (Poké-Power): once during your turn, if Unown ? is on your Bench, put a Pokémon from your hand face down; a wrong guess of its type by the opponent draws you a card. Hidden Power: discard up to 2 cards, draw as many.</summary>
        private sealed class UnownQuestion : Mon
        {
            private static readonly EnergyType[] Types = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting, EnergyType.Darkness, EnergyType.Metal, EnergyType.Colorless, EnergyType.Dragon };

            protected override string Activated { get { return "?"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownBenched(ctx) && ctx.Me.Hand.Any(c => c.Def.IsPokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList(), 1, 1, "?: put which Pokémon face down?")[0];
                var opts = Types.Select((t, i) => new DecisionOption { Label = t.ToString(), Index = i }).ToList();
                int guess = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "?: guess the type of the face-down Pokémon", opts, ctx.Source, false);
                var type = Types[Math.Max(0, Math.Min(Types.Length - 1, guess))];
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, "?", "?: the face-down Pokémon", ctx.Source);
                if (!card.Def.HasType(type)) ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Me.Hand.Count == 0) return true;
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 0, Math.Min(2, ctx.Me.Hand.Count), "Hidden Power: discard up to 2 cards (draw that many)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                if (picks.Count > 0) ctx.Draw(picks.Count);
                return true;
            }
        }

        // ------------------------------------------------------------------ Commons

        /// <summary>Metal Chain (Poké-Power): once during your turn, when you attach a Metal Energy card from your hand to Beldum, you may put a Beldum from your deck onto your Bench. Not with a Special Condition. Take Down.</summary>
        private sealed class BeldumMetalChain : Mon
        {
            private static readonly Mon Down = new RecoilMon("Take Down", 10);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || energy.Def.EnergyProvides != EnergyType.Metal || !CardsDS.PowerReady(src)) return;
                if (!TriggerOnce(src, "Metal Chain", src.BenchSpace > 0 && src.Me.Deck.Count > 0, "Metal Chain: put a Beldum from your deck onto your Bench?")) return;
                src.SearchToBench(d => d.Name == "Beldum", 1, "Metal Chain: choose a Beldum");
                src.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Down.HandleAttack(ctx, atk); }
        }

        /// <summary>Vine Bind: heads, the Defending Pokémon can't use Poké-Powers next turn.</summary>
        private sealed class VineBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vine Bind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new HeldEffect(defending.Name + " can't use Poké-Powers next turn (Vine Bind)", ctx.Player, defending.Uid, ctx.Turn + 1, false, false));
                return true;
            }
        }

        /// <summary>"Search your deck for up to N [kind of] Basic Pokémon and put them onto your Bench" (Call for Family).</summary>
        private sealed class BenchTypedMon : Mon
        {
            private readonly string attack;
            private readonly int max;
            private readonly Func<CardDef, bool> filter;

            public BenchTypedMon(string attack, int max, Func<CardDef, bool> filter)
            {
                this.attack = attack;
                this.max = max;
                this.filter = filter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace == 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToBench(d => !d.IsLevelUp && filter(d), max, attack + ": choose Basic Pokémon for your Bench");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Light Poison: tails, nothing; heads, Poisoned.</summary>
        private sealed class LightPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light Poison") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Space Out: remove 2 damage counters from this Pokémon; it's now Asleep.</summary>
        private sealed class SpaceOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Space Out") return false;
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 20);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Gut Strike: base [N] with Tyrogue under this Pokémon.</summary>
        private sealed class GutBaseMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public GutBaseMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(EvolvedFrom(ctx, "Tyrogue") ? amount : atk.Damage);
                return true;
            }
        }

        /// <summary>Gut Kick: with Tyrogue under it, you may do 30 to any 1 Benched Pokémon instead.</summary>
        private sealed class GutKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gut Kick") return false;
                var bench = ctx.MyBench().Concat(ctx.OpponentBench()).ToList();
                if (EvolvedFrom(ctx, "Tyrogue") && bench.Count > 0 && ctx.AskYesNo("Gut Kick: do 30 damage to a Benched Pokémon instead?"))
                {
                    var target = ctx.ChoosePokemon(bench, "Gut Kick: do 30 damage to which Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 30);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Gut Spin: with Tyrogue under it, you may switch this Pokémon with a Benched one.</summary>
        private sealed class GutSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gut Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (EvolvedFrom(ctx, "Tyrogue") && ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Gut Spin: switch with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Pay Day: heads, draw a card.</summary>
        private sealed class PayDayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pay Day") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.Draw(1);
                return true;
            }
        }

        /// <summary>Rob: look at the opponent's hand; any Pokémon Tool or Technical Machine cards there go into their deck, shuffled.</summary>
        private sealed class RobMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rob") return false;
                if (ctx.Opp.Hand.Count == 0) return true;
                ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Rob", "Rob: your opponent's hand", ctx.Source);
                var taken = ctx.Opp.Hand.Where(c => c.Def.IsTool || c.Def.Name.StartsWith("Technical Machine", StringComparison.Ordinal)).ToList();
                foreach (var c in taken) { ctx.Opp.Hand.Remove(c); ctx.Opp.Deck.Insert(0, c); }
                if (taken.Count > 0) ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Recover: discard an Energy from this Pokémon and remove 3 damage counters from it.</summary>
        private sealed class StaryuRecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Recover: discard an Energy") > 0 && me.DamageCounters > 0) ctx.Heal(me, 30);
                return true;
            }
        }

        /// <summary>Sniff Out: heads, any 1 card from your discard pile into your hand.</summary>
        private sealed class SniffOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sniff Out") return false;
                if (!ctx.Flip() || ctx.Me.Discard.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Sniff Out: put which card into your hand?"));
                return true;
            }
        }

        /// <summary>Focus Energy: during your next turn Peck's base damage is 30. Peck.</summary>
        private sealed class PeckMon : Mon
        {
            private const string Marker = "Focus Energy";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == Marker)
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Peck") return false;
                int turn;
                ctx.DamageDefending(me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn ? 30 : atk.Damage);
                return true;
            }
        }

        /// <summary>Gut Blow: against an Evolved Pokémon, 20 more and 10 to itself.</summary>
        private sealed class GutBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gut Blow") return false;
                bool evolved = ctx.Defending != null && CardsE2.IsEvolved(ctx.Defending);
                ctx.DamageDefending(atk.Damage + (evolved ? 20 : 0));
                if (evolved) ctx.DamageSelf(10);
                return true;
            }
        }

        // ------------------------------------------------------------------ Trainers

        /// <summary>Bubble Coat (Pokémon Tool): the holder has no Weakness; if it's damaged by the opponent's attack, discard this at the end of the turn.</summary>
        private sealed class BubbleCoat : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null && mon == src.Pokemon; }

            public override void OnEndOfEachTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || src.IsMyTurn || holder.AttackDamageTurn != src.Turn || !holder.HasTool(src.Card)) return;
                src.Log("Bubble Coat is discarded.");
                src.Engine.DiscardTool(holder, src.Card);
            }
        }

        /// <summary>Buck's Training (Supporter): draw 2 cards; this turn your Active Pokémon's attacks do 10 more to the Active Pokémon (before W/R).</summary>
        private sealed class BucksTraining : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                ctx.AddEffect(new BuckEffect("Your Active Pokémon's attacks do 10 more damage this turn (Buck's Training)", ctx.Player, ctx.Turn));
            }
        }

        private sealed class BuckEffect : ContinuousEffect
        {
            public BuckEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Owner != Owner || target == null || target.Owner == Owner || damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (engine.State.Players[Owner].Active != attacker || engine.State.Players[target.Owner].Active != target) return damage;
                return damage + 10;
            }
        }

        /// <summary>Cynthia's Feelings (Supporter): shuffle your hand into your deck and draw 4 (8 if any of your Pokémon was Knocked Out during the opponent's last turn).</summary>
        private sealed class CynthiasFeelings : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count + ctx.Me.Hand.Count(c => c != ctx.Card) > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var c in ctx.Me.Hand.Where(c => c != ctx.Card).ToList()) { ctx.Me.Hand.Remove(c); ctx.Me.Deck.Add(c); }
                ctx.ShuffleMyDeck();
                ctx.Draw(ctx.WasKnockedOutLastOpponentTurn() ? 8 : 4);
            }
        }

        /// <summary>Energy Pickup: heads, attach a basic Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class EnergyPickup : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return;
                var card = ctx.ChooseCards(pile, 1, 1, "Energy Pickup: attach which basic Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Energy Pickup: attach it to which Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
            }
        }

        /// <summary>Poké Radar: look at the top 5 cards; any Pokémon there into your hand; the rest back, shuffled.</summary>
        private sealed class PokeRadar : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(5);
                var pokemon = top.Where(c => c.Def.IsPokemon).ToList();
                if (pokemon.Count > 0) ctx.DeckToHand(ctx.ChooseCards(pokemon, 0, pokemon.Count, "Poké Radar: put which Pokémon into your hand?"));
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Snowpoint Temple (Stadium): each Pokémon in play that isn't Evolved (both players') gets +20 HP.</summary>
        private sealed class SnowpointTemple : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp) { return mon != null && !CardsE2.IsEvolved(mon) ? hp + 20 : hp; }
        }

        /// <summary>Stark Mountain (Stadium): once during each player's turn, they may move a Fire or Fighting Energy from 1 of their Pokémon to another.</summary>
        private sealed class StarkMountain : CardScript
        {
            private const string Limit = "Stark Mountain";

            private static bool Fits(EffectContext ctx, PokemonInPlay m, CardInstance e) { return CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fire) || CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fighting); }

            public override bool CanUseStadium(EffectContext ctx)
            {
                return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.InPlay().Count() > 1 && ctx.MyPokemon(m => m.Energy.Any(e => Fits(ctx, m, e))).Count > 0;
            }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                var from = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Energy.Any(e => Fits(ctx, m, e))), "Stark Mountain: move a Fire or Fighting Energy from which Pokémon?");
                if (from == null) return true;
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Stark Mountain: move it to which Pokémon?");
                if (to == null) return true;
                var cards = from.Energy.Where(e => Fits(ctx, from, e)).ToList();
                ctx.Engine.MoveEnergy(from, to, cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Stark Mountain: move which Energy card?")[0]);
                return true;
            }
        }

        /// <summary>Technical Machine TS-1 / TS-2: attached to 1 of your Pokémon, which may use its attack instead of its own.</summary>
        private sealed class TechnicalMachineTs : CardScript
        {
            private readonly Action<EffectContext> attack;

            public TechnicalMachineTs(Action<EffectContext> attack) { this.attack = attack; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override bool IsAttachedNonTool { get { return true; } }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.InPlay().Any(); }

            /// <summary>Played from the hand, it attaches to 1 of your Pokémon (it isn't a Pokémon Tool) and stays there.</summary>
            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), ctx.Card.Def.Name + ": attach it to which of your Pokémon?");
                if (target == null) return;
                ctx.CardPlacedElsewhere = true;
                ctx.Engine.AttachTool(ctx.Card, target);
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex) { attack(ctx); }
        }

        /// <summary>Evoluter: search your deck for a card that evolves from 1 of your Pokémon and evolve it.</summary>
        private static void Evoluter(EffectContext ctx)
        {
            if (ctx.Me.Deck.Count == 0) return;
            var names = ctx.MyPokemon().Select(m => m.Name).ToList();
            var found = ctx.SearchMyDeck(d => d.IsPokemon && !d.IsLevelUp && d.IsEvolution && names.Contains(d.EvolvesFrom), 1, "Evoluter: choose a card that evolves from 1 of your Pokémon");
            if (found.Count > 0)
            {
                var targets = ctx.MyPokemon(m => m.Name == found[0].Def.EvolvesFrom);
                var target = targets.Count == 0 ? null : ctx.ChoosePokemon(targets, "Evoluter: evolve which Pokémon?");
                if (target != null) ctx.Engine.EvolvePokemon(target, found[0]);
                else ctx.Me.Deck.Add(found[0]);
            }
            ctx.ShuffleMyDeck();
        }

        /// <summary>Devoluter: remove the highest Stage card from 1 of the opponent's Evolved Pokémon (not LV.X) into their hand.</summary>
        private static void Devoluter(EffectContext ctx)
        {
            var targets = ctx.OpponentPokemon(m => CardsE2.IsEvolved(m) && !m.Def.IsLevelUp && ctx.CanAffect(m));
            if (targets.Count == 0) return;
            var target = ctx.ChoosePokemon(targets, "Devoluter: devolve which of your opponent's Pokémon?");
            if (target != null) ctx.Engine.Devolve(target, 1);
        }
    }
}
