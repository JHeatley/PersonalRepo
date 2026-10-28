using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Vivid Voltage (VIV, 2020): Pokémon 83-145 (Fighting, Darkness, Metal, Colorless) and the full-art Galarian Sirfetch'd V
    /// (174), the Trainers and the Special Energy. Allister, Bea, Leon, Nessa, Opal, Rocky Helmet, Pokémon Center Lady and Cara
    /// Liss were already in the pool. Your ruling: under Shiftry's Shifty Substitution any Supporter can be played to draw 3.
    /// Pokémon whose attacks only do their printed damage need no script (Wooper, Rockruff, Lycanroc, Mudbray, Clobbopus,
    /// Poochyena, Metang, Eevee, Pikipek).
    /// </summary>
    public static class CardsVIV2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("VIV-84", new FickleImpactMon());
            r.RegisterByKey("VIV-85", new DeckDistillerMon());
            r.RegisterByKey("VIV-86", new CardsASR1.TimesMon("Strike Back", OwnDamageCounters));
            r.RegisterByKey("VIV-87", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20));
            r.RegisterByKey("VIV-88", new ComboMon(new CardsCRZ1.CycleDrawMon("Cycle Draw", 3), new CardsBRS1.SupporterBonusMon("Tornado Kick", "Bea", 80)));
            r.RegisterByKey("VIV-89", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new RecoilMon("Megaton Fall", 30)));
            r.RegisterByKey("VIV-90", new TailsNothingMon("Best Punch"));
            r.RegisterByKey("VIV-91", new TailsNothingMon("Focus Fist"));
            r.RegisterByKey("VIV-92", new ComboMon(new CardsASC.SelfGuardMon("Guard Press", 30), new CardsPAL1.StadiumBonusMon("Earthen Power", 80)));
            r.RegisterByKey("VIV-93", new CardsPAL1.PrizeBehindBonusMon("Core Avenger", 80));
            r.RegisterByKey("VIV-97", new HeavySlamMon());
            r.RegisterByKey("VIV-98", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("VIV-99", new EruptionShotMon());
            r.RegisterByKey("VIV-101", new ComboMon(new NoRetreatMon("Full Nelson"), new TentacleBusterMon()));
            r.RegisterByKey("VIV-102", new ComboMon(new CardsVIV1.MetalArmamentMon(), new CardsBST2.GigatonBashMon("Amazing Shield")));
            r.RegisterByKey("VIV-174", new SirfetchdV());
            // Darkness
            r.RegisterByKey("VIV-104", new CardsMEG.WeakenDefendingMon("Ferocious Bellow", 50));
            r.RegisterByKey("VIV-105", new ComboMon(new DrawAttackMon("Filch", 1), new CardsPOR.MemoryLockMon("Torment")));
            r.RegisterByKey("VIV-106", new HazardousClawsMon());
            r.RegisterByKey("VIV-107", new CardsSSP2.StompOffMon("Dredge Up", 3));
            r.RegisterByKey("VIV-109", new ComboMon(new CardsSSP2.StompOffMon("Dredge Up", 3), new TantrumMon()));
            r.RegisterByKey("VIV-110", new SearchAttackMon("Lucky Find", IsItemCard, 1));
            r.RegisterByKey("VIV-111", new ComboMon(new TrashCycloneMon(), new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned)));
            // Metal
            r.RegisterByKey("VIV-112", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("VIV-113", new StealyClawsMon());
            r.RegisterByKey("VIV-114", new ComboMon(new RecoilMon("Take Down", 30), new DoubleKoMon()));
            r.RegisterByKey("VIV-115", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Hammer", 10), new RecoilMon("Iron Tackle", 30)));
            r.RegisterByKey("VIV-116", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("VIV-118", new MetagrossVIV());
            r.RegisterByKey("VIV-119", new JirachiVIV());
            r.RegisterByKey("VIV-120", new ComboMon(new DrawAttackMon("Spike Draw", 1), new LockAfterAttackMon("Knuckle Impact", false)));
            r.RegisterByKey("VIV-121", new ComboMon(new CardsPAR1.BringerMon("Rewind Time", EnergyType.Metal, 2), new CardsTWM1.SelfDiscardMon("Flash of Destruction", 2)));
            r.RegisterByKey("VIV-122", new CardsDRI2.CrunchMon("Drill Run"));
            r.RegisterByKey("VIV-123", new SwitchSelfMon("Reaction", false));
            r.RegisterByKey("VIV-124", new SwiftSwingMon());
            r.RegisterByKey("VIV-125", new CardsASC.CounterattackMon("Counterattack", 3, new NoRetreatMon("Grip and Squeeze")));
            r.RegisterByKey("VIV-126", new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge"));
            r.RegisterByKey("VIV-127", new CardsASR1.PlusMon("Max Hack", 30, MyPrizesTaken));
            r.RegisterByKey("VIV-128", new ComboMon(new CardsBKT1.DeepSeaSwirlMon("Overhaul", 6), new CardsASR1.PlusMon("Windup Cannon", 20, OpponentBenchCount)));
            r.RegisterByKey("VIV-129", new ComboMon(new Cards30C.SelfCounterBonusMon("Raging Claws", 10), new CardsTWM1.SelfDiscardMon("Power Blast", 1)));
            // Colorless
            r.RegisterByKey("VIV-132", new CardsASR1.JurassicHammerMon("Wind Pressure", 5));
            r.RegisterByKey("VIV-133", new CoinsMon("Double Peck", 2));
            r.RegisterByKey("VIV-134", new ComboMon(new FlipBonusMon("Quick Attack", 40), new EnergyAssistMon()));
            r.RegisterByKey("VIV-135", new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"));
            r.RegisterByKey("VIV-136", new CardsBLK.RoundMon());
            r.RegisterByKey("VIV-137", new CardsBLK.RoundMon());
            r.RegisterByKey("VIV-138", new AmazingBurstMon());
            r.RegisterByKey("VIV-139", new SearchAttackMon("Minor Errand-Running", IsBasicEnergyCard, 2));
            r.RegisterByKey("VIV-140", new CardsPAL1.EvolutionBonusMon("White Wind", 70));
            r.RegisterByKey("VIV-141", new SearchAttackMon("Max Glide", AnyCard, 2));
            r.RegisterByKey("VIV-142", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Jet Draft", false), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("VIV-144", new CardsPAF2.BloomingEnergyMon("Charging Trumpet", 3));
            r.RegisterByKey("VIV-145", new ComboMon(new CardsDRI2.CrunchMon("Energy Cutoff"), new CardsVIV1.AnyEnergyToHandMon("Loop Cannon", 2)));

            // Trainers
            r.RegisterByName("Beauty", new Beauty());
            r.RegisterByName("Cape of Toughness", new CapeOfToughness());
            r.RegisterByName("Circhester Bath", new CirchesterBath());
            r.RegisterByName("Drone Rotom", new DroneRotom());
            r.RegisterByName("Hero's Medal", new HerosMedal());
            r.RegisterByName("League Staff", new LeagueStaff());
            r.RegisterByName("Memory Capsule", new MemoryCapsule());
            r.RegisterByName("Moomoo Cheese", new MoomooCheese());
            r.RegisterByName("Telescopic Sight", new TelescopicSight());
            r.RegisterByName("Wyndon Stadium", new WyndonStadium());
            // Special Energy
            r.RegisterByName("Aromatic Grass Energy", new AromaticGrassEnergy());
            r.RegisterByName("Coating Metal Energy", new CoatingMetalEnergy());
            r.RegisterByName("Stone Fighting Energy", new StoneFightingEnergy());
            r.RegisterByName("Wash Water Energy", new WashWaterEnergy());
        }

        // ================================================================== Filters and counters

        private static bool IsItemCard(CardDef d) { return d.IsItem; }
        private static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }
        private static bool AnyCard(CardDef d) { return true; }
        private static int OwnDamageCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters; }
        private static int MyPrizesTaken(EffectContext ctx) { return ctx.MyPrizesTaken; }
        private static int OpponentBenchCount(EffectContext ctx) { return ctx.Opp.Bench.Count; }

        // ================================================================== Fighting

        /// <summary>Fickle Impact: does nothing if you have exactly 2, 4 or 6 Prize cards remaining.</summary>
        private sealed class FickleImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fickle Impact") return false;
                int left = ctx.Me.Prizes.Count;
                if (left == 2 || left == 4 || left == 6) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Deck Distiller: flip until tails; discard the top card of the opponent's deck for each heads.</summary>
        private sealed class DeckDistillerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deck Distiller") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        /// <summary>Heavy Slam: 30 less damage for each [C] in the opponent's Active Pokémon's Retreat Cost.</summary>
        internal sealed class HeavySlamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Slam") return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                int damage = Math.Max(0, atk.Damage - 30 * cost);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log("Heavy Slam did no damage.");
                return true;
            }
        }

        /// <summary>Eruption Shot: discard the top card of your deck; if it's an Energy card, 90 more damage and attach it to this Pokémon.</summary>
        private sealed class EruptionShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption Shot") return false;
                var me = ctx.Pokemon;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                var card = discarded.Count > 0 ? discarded[0] : null;
                bool energy = card != null && card.Def.IsEnergy;
                ctx.DamageDefending(atk.Damage + (energy ? 90 : 0));
                if (energy && ctx.InPlay(me) && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Tentacle Buster: 120 more if this Pokémon used Full Nelson during your last turn.</summary>
        private sealed class TentacleBusterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tentacle Buster") return false;
                var me = ctx.Pokemon;
                bool nelson = me.LastAttackName == "Full Nelson" && me.LastAttackTurn == ctx.MyLastTurn;
                ctx.DamageDefending(atk.Damage + (nelson ? 120 : 0));
                return true;
            }
        }

        private sealed class VmaxShieldEffect : ContinuousEffect
        {
            public VmaxShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Owner != Owner && info.Attacker.Def.IsVmax;
            }
        }

        /// <summary>Resolute Spear: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may move any amount of Fighting Energy from your other Pokémon to it. Meteor Smash: can't attack next turn.</summary>
        private sealed class SirfetchdV : Mon
        {
            private static readonly Mon MeteorSmash = new LockAfterAttackMon("Meteor Smash", false);

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null) return;
                var engine = ctx.Engine;
                var fighting = new List<CardInstance>();
                var holderOf = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                {
                    if (mon == me) continue;
                    foreach (var e in mon.Energy)
                        if (engine.EnergyUnitsOf(mon, e).Any(u => (u & EnergyMask.Of(EnergyType.Fighting)) != 0))
                        {
                            fighting.Add(e);
                            holderOf[e.Id] = mon;
                        }
                }
                if (!TriggerOnce(ctx, "Resolute Spear", fighting.Count > 0, "Use Resolute Spear? Move Fighting Energy from your other Pokémon to " + me.Name + ".")) return;
                var picks = ctx.ChooseCards(fighting, 0, fighting.Count, "Resolute Spear: move which Fighting Energy to " + me.Name + "?");
                foreach (var e in picks) engine.MoveEnergy(holderOf[e.Id], me, e);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MeteorSmash.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Hazardous Claws: discard 2 Energy from this Pokémon; the opponent's Active Pokémon is now Paralyzed and Poisoned.</summary>
        private sealed class HazardousClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hazardous Claws") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Hazardous Claws: discard 2 Energy from " + me.Name);
                if (TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Tantrum: this Pokémon is now Confused.</summary>
        private sealed class TantrumMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantrum") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Trash Cyclone: 30 damage for each Pokémon Tool card in your discard pile, then shuffle them into your deck.</summary>
        private sealed class TrashCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trash Cyclone") return false;
                var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
                if (tools.Count > 0) ctx.DamageDefending(atk.Damage * tools.Count);
                else ctx.Log("Trash Cyclone: no Pokémon Tool cards in the discard pile, so no damage.");
                if (tools.Count > 0)
                {
                    ctx.DiscardToDeck(tools);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Stealy Claws: flip 3 coins; if any are heads, the opponent reveals their hand and you discard a Trainer card from it for each heads.</summary>
        private sealed class StealyClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stealy Claws") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var trainers = ctx.Opp.Hand.Where(c => c.Def.IsTrainer).ToList();
                int n = Math.Min(heads, trainers.Count);
                if (n == 0) return true;
                var picks = ctx.ChooseCards(trainers, n, n, "Stealy Claws: discard " + n + (n == 1 ? " Trainer card" : " Trainer cards") + " from your opponent's hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Double KO: both Active Pokémon are Knocked Out.</summary>
        private sealed class DoubleKoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double KO") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.Engine.KnockOut(defending, "Double KO");
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.KnockOut(ctx.Pokemon, "Double KO");
                return true;
            }
        }

        /// <summary>Levitation Field: your Pokémon in play have no Retreat Cost. Leg Quake: an Evolution Defending Pokémon can't attack next turn.</summary>
        private sealed class MetagrossVIV : Mon
        {
            private static bool IsEvolutionCard(CardDef d) { return d.IsEvolution; }
            private static readonly Mon LegQuake = new CardsEVS1.DefenderCantAttackMon("Leg Quake", IsEvolutionCard);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player ? cost - 100 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LegQuake.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Dreamy Revelation: once during your turn, if this Pokémon is Active, look at the top 2 cards of your deck and put 1 into
        /// your hand (the other stays on top). Amazing Star: search your deck for up to 7 basic Energy cards and attach them to
        /// your Pokémon in any way you like.
        /// </summary>
        private sealed class JirachiVIV : Mon
        {
            protected override string Activated { get { return "Dreamy Revelation"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var pick = top.Count == 1 ? top[0] : ctx.ChooseCards(top, 1, 1, "Dreamy Revelation: put which card into your hand? (the other stays on top of your deck)")[0];
                ctx.Me.Deck.Remove(pick);
                ctx.Me.Hand.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 of the top 2 cards of " + ctx.Engine.Their(ctx.Player) + " deck into " + ctx.Engine.Their(ctx.Player) + " hand (Dreamy Revelation).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amazing Star") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsBasicEnergyCard, 7, "Amazing Star: choose up to 7 basic Energy cards");
                ctx.AttachCardsAnyWay(found, found.Count, (card, mon) => true, "Amazing Star", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Swift Swing: 30 damage for each Metal Energy attached to this Pokémon, then switch it with 1 of your Benched Pokémon.</summary>
        private sealed class SwiftSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift Swing") return false;
                int metal = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Metal);
                if (metal > 0) ctx.DamageDefending(atk.Damage * metal);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Energy Assist: attach up to 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EnergyAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                ctx.DamageDefending(atk.Damage);
                var basics = ctx.BasicEnergyInDiscard();
                if (ctx.Me.Bench.Count == 0 || basics.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Energy Assist: attach basic Energy to which Benched Pokémon?");
                if (target != null) ctx.AttachCardsAnyWay(basics, 2, (card, mon) => mon == target, "Energy Assist", true);
                return true;
            }
        }

        /// <summary>Amazing Burst: discard all basic Energy from this Pokémon; 80 damage for each type of basic Energy discarded.</summary>
        private sealed class AmazingBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amazing Burst") return false;
                var me = ctx.Pokemon;
                var types = new HashSet<EnergyType>();
                foreach (var e in me.Energy.Where(c => c.Def.IsBasicEnergy).ToList())
                    if (ctx.Engine.DiscardEnergy(me, e)) types.Add(e.Def.EnergyProvides);
                if (types.Count > 0) ctx.DamageDefending(atk.Damage * types.Count);
                else ctx.Log("Amazing Burst: no basic Energy was discarded, so no damage.");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Beauty: draw 2 cards; the player who goes first may play it during their first turn.</summary>
        private sealed class Beauty : CardScript
        {
            public override bool PlayableOnFirstTurn(EffectContext ctx) { return true; }
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(2); }
        }

        /// <summary>League Staff: draw 2 cards, 2 more with Wyndon Stadium in play.</summary>
        private sealed class LeagueStaff : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool wyndon = ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == "Wyndon Stadium";
                ctx.Draw(wyndon ? 4 : 2);
            }
        }

        /// <summary>Drone Rotom: the opponent reveals their hand; then look at the top card of their deck.</summary>
        private sealed class DroneRotom : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                if (ctx.Opp.Deck.Count == 0) return;
                var top = new List<CardInstance> { ctx.Opp.Deck[0] };
                ctx.Engine.NoteLookedAt(top, "Top card of your opponent's deck", ctx.Player);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at the top card of " + ctx.Engine.Their(ctx.Opponent) + " deck (Drone Rotom).");
            }
        }

        /// <summary>Moomoo Cheese: heal 30 damage from up to 2 of your Pokémon that have Energy attached.</summary>
        private sealed class MoomooCheese : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Count > 0 && m.DamageCounters > 0); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var picks = targets.Count <= 2 ? targets : ctx.ChoosePokemonMany(targets, 1, 2, "Moomoo Cheese: heal 30 damage from up to 2 of your Pokémon with Energy attached");
                foreach (var m in picks) ctx.Heal(m, 30);
            }
        }

        /// <summary>Hero's Medal: the Pokémon VMAX it's attached to gets -100 HP and gives up 1 fewer Prize card when Knocked Out by the opponent's attack damage; not onto a Pokémon VMAX with 100 HP or less left.</summary>
        private sealed class HerosMedal : CardScript
        {
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target)
            {
                return target == null || !target.Def.IsVmax || ctx.Engine.RemainingHp(target) > 100;
            }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.IsVmax ? hp - 100 : hp;
            }

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || !knockedOut.Def.IsVmax || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                src.Log("Hero's Medal: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
                return prizes - 1;
            }
        }

        /// <summary>Memory Capsule: the Pokémon it's attached to can use the attacks of its previous Evolutions.</summary>
        private sealed class MemoryCapsule : CardScript
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || mon.Stack.Count < 2) yield break;
                for (int s = 0; s < mon.Stack.Count - 1; s++)
                {
                    var d = mon.Stack[s].Def;
                    if (!d.IsPokemon) continue;
                    for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
                }
            }
        }

        /// <summary>Telescopic Sight: the holder's attacks do 30 more damage to the opponent's Benched Pokémon V and Pokémon-GX.</summary>
        private sealed class TelescopicSight : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.AgainstOpponent || info.TargetIsActive || damage <= 0) return damage;
                return CardsEVS1.IsVOrGx(info.Target.Def) ? damage + 30 : damage;
            }
        }

        /// <summary>Cape of Toughness: the Basic Pokémon it's attached to gets +50 HP, except Pokémon-GX.</summary>
        private sealed class CapeOfToughness : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.IsBasicPokemon && !mon.Def.IsGX ? hp + 50 : hp;
            }
        }

        /// <summary>Circhester Bath: all Basic Pokémon (both players') take 20 less damage from attacks from the opponent's Pokémon.</summary>
        private sealed class CirchesterBath : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                var a = info.Attacker;
                if (t == null || a == null || a.Owner == t.Owner || !t.Def.IsBasicPokemon) return damage;
                return damage - 20;
            }
        }

        /// <summary>Wyndon Stadium: whenever either player plays a Pokémon VMAX from their hand to evolve their Pokémon V, heal 100 damage from it.</summary>
        private sealed class WyndonStadium : CardScript
        {
            public override void OnPokemonEvolvedFromHandUnderStadium(EffectContext src, PokemonInPlay evolved, CardDef before)
            {
                if (evolved == null || !evolved.Def.IsVmax || before == null || !before.IsPokemonV || evolved.DamageCounters == 0) return;
                src.Engine.Heal(evolved, 100);
            }
        }

        // ================================================================== Special Energy

        /// <summary>Aromatic Grass Energy: provides Grass; its Grass Pokémon recovers from and can't be affected by Special Conditions.</summary>
        private sealed class AromaticGrassEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Grass), 1); }

            // The engine removes Special Conditions a Pokémon can't have right after each action ("recovers").
            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && target == src.Pokemon && src.Engine.IsType(target, EnergyType.Grass);
            }
        }

        /// <summary>Coating Metal Energy: provides Metal; its Metal Pokémon has no Weakness.</summary>
        private sealed class CoatingMetalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Metal), 1); }

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Metal);
            }
        }

        /// <summary>Stone Fighting Energy: provides Fighting; its Fighting Pokémon takes 20 less damage from the opponent's attacks.</summary>
        private sealed class StoneFightingEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Fighting), 1); }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || !src.Engine.IsType(me, EnergyType.Fighting)) return damage;
                return damage - 20;
            }
        }

        /// <summary>Wash Water Energy: provides Water; prevents all effects of the opponent's attacks on its Water Pokémon (not damage).</summary>
        private sealed class WashWaterEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Water), 1); }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && src.Engine.IsType(target, EnergyType.Water);
            }
        }
    }
}
