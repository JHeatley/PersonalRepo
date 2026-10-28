using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Origin (LOR 1-151, 172, 181; 2022): the Pokémon that weren't already in the pool (LOR 4, 33, 52, 58, 60, 79, 118,
    /// 131, 134 and 147 came in with Crown Zenith's reprints). Regulation mark F: Expanded only. The full arts and secret rares
    /// (but not Gallade V, 181) and most of the Trainer Gallery are second prints of the main cards. Pokémon whose attacks only do their
    /// printed damage need no script: Oddish, Phantump, Blipbug, Shellos, Snover, Ducklett, Shuppet, Machop, Machoke, Mienfoo,
    /// Binacle, Murkrow, Bronzor, Goomy, Lickitung, Lickilicky and Skwovet. The Trainer Gallery Pokémon, Trainers and Gift
    /// Energy are in CardsLOR2.
    /// </summary>
    public static class CardsLOR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LOR-2", new CardsTWM1.HealSelfMon("Absorb", 30));
            r.RegisterByKey("LOR-3", new ComboMon(new CardsTWM1.HealSelfMon("Mega Drain", 30), new AllergyStormMon()));
            r.RegisterByKey("LOR-5", new ParasectLOR());
            r.RegisterByKey("LOR-6", new SearchAttackMon("Creepy-Crawly Congregation", IsWurmpleFamily, 60));
            r.RegisterByKey("LOR-7", new EntanglingStringMon());
            r.RegisterByKey("LOR-8", new BeautiflyLOR());
            r.RegisterByKey("LOR-9", new HardenMon("Harden", 60));
            r.RegisterByKey("LOR-10", new NadirPowderMon());
            r.RegisterByKey("LOR-11", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("LOR-12", new CardsDRI1.WhirlwindMon("Push Down", true));
            r.RegisterByKey("LOR-13", new ComboMon(new CardsDRI1.WhirlwindMon("Fan Tornado", true), new TearingGustMon()));
            r.RegisterByKey("LOR-14", new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep));
            r.RegisterByKey("LOR-15", new ComboMon(new ConditionAttackMon("Poisonous Whip", SpecialCondition.Poisoned), new AssassinsRoseMon()));
            r.RegisterByKey("LOR-17", new TrevenantLOR());
            r.RegisterByKey("LOR-19", new CardsASC.SelfGuardMon("Barrier Attack", 30));
            r.RegisterByKey("LOR-20", new OrbeetleLOR());
            r.RegisterByKey("LOR-21", new CardsOBF1.AttachFromDiscardToSelfMon("Draw In", EnergyType.Fire));
            r.RegisterByKey("LOR-22", new LostVolcanoMon());
            r.RegisterByKey("LOR-23", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("LOR-24", new CardsSSP2.StompOffMon("Kindling Panic", 1));
            r.RegisterByKey("LOR-25", new CardsASC.FlipDiscardDefendingEnergyMon("Flickering Glow"));
            r.RegisterByKey("LOR-26", new ChandelureLOR());
            r.RegisterByKey("LOR-27", new ComboMon(new Cards30C.BurnAndConfuseMon("Eerie Glow"), new MagicalFireMon()));
            r.RegisterByKey("LOR-28", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("LOR-29", new PyroarLOR());
            r.RegisterByKey("LOR-30", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("LOR-31", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("LOR-32", new LordlySongleaderMon());
            r.RegisterByKey("LOR-34", new ComboMon(new DodgeMon("Swim Freely", false), new FloeReturnMon()));
            r.RegisterByKey("LOR-35", new SwitchSelfMon("Reverse Thrust", false));
            r.RegisterByKey("LOR-36", new ComboMon(new DodgeMon("Swim Freely", false), new HydroJetMon()));
            r.RegisterByKey("LOR-37", new KingdraLOR());
            r.RegisterByKey("LOR-38", new MatchingLookMon());
            r.RegisterByKey("LOR-40", new FinneonLOR());
            r.RegisterByKey("LOR-41", new DodgeMon("Swim Freely", false));
            r.RegisterByKey("LOR-43", new RecoilMon("Double-Edge", 30));
            r.RegisterByKey("LOR-44", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("LOR-45", new ComboMon(new UpstreamSpiritsMon(), new CardsTWM1.SelfDiscardMon("Water Shot", 1)));
            r.RegisterByKey("LOR-47", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("LOR-48", new RapidFreezeMon());
            r.RegisterByKey("LOR-49", new KyuremVmax());
            r.RegisterByKey("LOR-50", new CramorantLOR());
            r.RegisterByKey("LOR-51", new ComboMon(new FreezeDownMon(), new RecoilMon("Wild Tackle", 30)));
            r.RegisterByKey("LOR-53", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new AceSparkMon()));
            r.RegisterByKey("LOR-54", new ParalyzeFlipMon("Thunder Fang"));
            r.RegisterByKey("LOR-55", new AssaultLaserMon());
            r.RegisterByKey("LOR-56", new ComboMon(new CardsWHT.DragOffMon("Magnetic Tension", 40), new SplittingBeamMon()));
            r.RegisterByKey("LOR-57", new ComboMon(new SearchAttackMon("Magnetic Grip", IsItemCard, 2), new ElectroStarMon()));
            r.RegisterByKey("LOR-59", new SearchAttackMon("Call Sign", IsLightningPokemon, 1));
            r.RegisterByKey("LOR-61", new ComboMon(new CoilMon(), new CardsTWM1.SelfDiscardMon("Extreme Current", 2)));
            r.RegisterByKey("LOR-62", new ClefairyLOR());
            r.RegisterByKey("LOR-63", new ClefableLOR());
            r.RegisterByKey("LOR-64", new FurtiveDropMon(1));
            r.RegisterByKey("LOR-65", new CountersAnyWayMon("Cursed Drop", 3));
            r.RegisterByKey("LOR-66", new GengarLOR());
            r.RegisterByKey("LOR-67", new TrickySlapMon());
            r.RegisterByKey("LOR-68", new AlluringDanceMon());
            r.RegisterByKey("LOR-69", new RadiantGardevoir());
            r.RegisterByKey("LOR-70", new SableyeLOR());
            r.RegisterByKey("LOR-71", new TemptingTrapMon());
            r.RegisterByKey("LOR-73", new BanetteLOR());
            r.RegisterByKey("LOR-74", new MoonglowReverseMon());
            r.RegisterByKey("LOR-75", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("LOR-76", new ComboMon(new DoomCurseMon(), new CallBackMon()));
            r.RegisterByKey("LOR-77", new TailsNothingMon("Fickle Attack"));
            r.RegisterByKey("LOR-78", new SynchroKinesisMon());
            r.RegisterByKey("LOR-80", new ComboMon(new ConditionAttackMon("Perplex", SpecialCondition.Confused), new WorstGiftMon()));
            r.RegisterByKey("LOR-81", new ComboMon(new NightFootstepsMon(), new LockAfterAttackMon("Phantom Strike", true)));
            r.RegisterByKey("LOR-82", new EnamorusV());
            r.RegisterByKey("LOR-83", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("LOR-84", new VeryVulnerableMon("Very Vulnerable", 150));
            r.RegisterByKey("LOR-85", new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused), new SplashLoopMon()));
            r.RegisterByKey("LOR-88", new MachampLOR());
            r.RegisterByKey("LOR-89", new RecoilMon("Take Down", 20));
            r.RegisterByKey("LOR-90", new RecoilMon("Take Down", 30));
            r.RegisterByKey("LOR-91", new ComboMon(new GeoCannonMon(), new RecoilMon("Rocky Tackle", 30)));
            r.RegisterByKey("LOR-92", new CardsDRI2.CrunchMon("Rock Crush"));
            r.RegisterByKey("LOR-93", new AerodactylVstar());
            r.RegisterByKey("LOR-94", new ComboMon(new CardsBLK.PeckOffMon("Joust"), new NoRetreatMon("Impound")));
            r.RegisterByKey("LOR-95", new CoinsMon("Cyclone Pincers", 4));
            r.RegisterByKey("LOR-96", new HurricaneShockMon());
            r.RegisterByKey("LOR-97", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("LOR-98", new ComboMon(new ParalyzeFlipMon("Shove"), new CardsFLF1.NoResistanceMon("Muscular Slap")));
            r.RegisterByKey("LOR-99", new TailsNothingMon("Focus Fist"));
            r.RegisterByKey("LOR-100", new BattleStepMon());
            r.RegisterByKey("LOR-101", new CardsPAF1.BasicEnergyFromDiscardMon("Into the Deep", 2));
            r.RegisterByKey("LOR-102", new ComboMon(new RecoverMon(), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20)));
            r.RegisterByKey("LOR-104", new CoinsMon("Double Smash", 2));
            r.RegisterByKey("LOR-105", new CardsTWM1.SelfDiscardMon("Smashing Edge", 2));
            r.RegisterByKey("LOR-107", new BarbaracleLOR());
            r.RegisterByKey("LOR-108", new SearchAttackMon("Lucky Find", IsItemCard, 2));
            r.RegisterByKey("LOR-109", new DrawAttackMon("Double Draw", 2));
            r.RegisterByKey("LOR-110", new FightingHeadbuttMon());
            r.RegisterByKey("LOR-111", new ComboMon(new PowerStoneMon(), new LostShotMon()));
            r.RegisterByKey("LOR-112", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("LOR-113", new ComboMon(new CardsOBF1.RetreatCostDamageMon("String Bind", 30, false), new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned)));
            r.RegisterByKey("LOR-115", new NightCycloneMon());
            r.RegisterByKey("LOR-116", new SuckerPunchAndTurnMon());
            r.RegisterByKey("LOR-117", new SpiritombLOR());
            r.RegisterByKey("LOR-119", new DrapionVstar());
            r.RegisterByKey("LOR-120", new ComboMon(new ConditionAttackMon("Nightmare", SpecialCondition.Asleep), new LockAfterAttackMon("Pitch-Black Blade", false)));
            r.RegisterByKey("LOR-121", new CallForFamilyMon());
            r.RegisterByKey("LOR-122", new HandOfDjinnMon());
            r.RegisterByKey("LOR-123", new RadiantHisuianSneasler());
            r.RegisterByKey("LOR-124", new ComboMon(new EnergyStreamMon(), new DestructiveFinishMon()));
            r.RegisterByKey("LOR-126", new CardsOBF1.RetreatCostDamageMon("Gravitational Drop", 40, false));
            r.RegisterByKey("LOR-127", new FieldTrapMon());
            r.RegisterByKey("LOR-128", new WindupBeamMon());
            r.RegisterByKey("LOR-129", new ComboMon(new DrawAttackMon("Feelin' Fine", 3), new TreasureRushMon()));
            r.RegisterByKey("LOR-130", new ComboMon(new AbyssSeekingMon(), new CardsPAF2.IgnoreDefenderEffectsMon("Shred")));
            r.RegisterByKey("LOR-133", new CardsASC.SelfGuardMon("Rigidify", 50));
            r.RegisterByKey("LOR-135", new ComboMon(new CardsASC.PushDownMon("Slip-'n'-Trip"), new CardsASC.SelfGuardMon("Rolling Shell", 30)));
            r.RegisterByKey("LOR-136", new HisuianGoodraVstar());
            r.RegisterByKey("LOR-137", new PidgeotV());
            r.RegisterByKey("LOR-140", new BranchCalculationMon());
            r.RegisterByKey("LOR-141", new GarbageAttackMon());
            r.RegisterByKey("LOR-142", new DowngradingBeamMon());
            r.RegisterByKey("LOR-143", new SnorlaxLOR());
            r.RegisterByKey("LOR-144", new MischievousTailMon());
            r.RegisterByKey("LOR-145", new ComboMon(new AllYouCanGrabMon(), new CardsASC.RandomDiscardMon("Knock Off")));
            r.RegisterByKey("LOR-146", new ComboMon(new SwitchSelfMon("Void Return", true), new ShadowCycloneMon("Shadow Cyclone")));
            r.RegisterByKey("LOR-148", new LostHeadbuttMon());
            r.RegisterByKey("LOR-149", new KomalaLOR());
            r.RegisterByKey("LOR-151", new ComboMon(new DrawAttackMon("Collect", 2), new SpillOutMon()));
            r.RegisterByKey("LOR-172", new ComboMon(new TantrumBlastMon(), new CardsTWM1.SelfDiscardMon("Solar Shot", 99)));
            r.RegisterByKey("LOR-181", new ComboMon(new RisingSwordMon(), new CardsFLF1.NoResistanceMon("Buster Swing")));
        }

        // ================================================================== Shared helpers (also used by CardsLOR2)

        private static bool IsWurmpleFamily(CardDef d)
        {
            if (!d.IsPokemon) return false;
            return d.Name == "Wurmple" || d.Name == "Silcoon" || d.Name == "Beautifly" || d.Name == "Cascoon" || d.Name == "Dustox";
        }

        private static bool IsItemCard(CardDef d) { return d.IsItem; }

        private static bool IsLightningPokemon(CardDef d) { return d.IsPokemon && d.HasType(EnergyType.Lightning); }

        /// <summary>Puts an Energy attached to a Pokémon in its owner's Lost Zone.</summary>
        internal static void EnergyToLostZone(EffectContext ctx, PokemonInPlay mon, CardInstance energy)
        {
            if (mon == null || energy == null || !mon.Energy.Remove(energy)) return;
            CardsCRZ1.PutInLostZone(ctx, energy);
        }

        /// <summary>
        /// "Put this Pokémon in the Lost Zone": the Pokémon (every card in its evolution stack) goes to the Lost Zone; its attached
        /// cards go there too (Tearing Gust) or are discarded (Puppet Offering).
        /// </summary>
        internal static void PokemonToLostZone(EffectContext ctx, PokemonInPlay mon, bool attachedToo)
        {
            if (mon == null || !ctx.InPlay(mon)) return;
            var owner = ctx.State.Players[mon.Owner];
            var stack = mon.Stack.ToList();
            var attached = new List<CardInstance>(mon.Energy);
            attached.AddRange(mon.Tools);
            string name = mon.Name;
            ctx.Engine.RemoveFromPlay(mon);
            owner.LostZone.AddRange(stack);
            if (attachedToo) owner.LostZone.AddRange(attached);
            else owner.Discard.AddRange(attached);
            ctx.Engine.Emit(GameEventType.Info, mon.Owner, name + " was put in the Lost Zone" + (attached.Count == 0 ? "." : attachedToo ? " with all attached cards." : "; its attached cards were discarded."), stack[stack.Count - 1].Id, mon.Uid);
        }

        /// <summary>"This attack does N damage to 2 of your opponent's Benched Pokémon" (fewer if they have fewer).</summary>
        internal static void DamageBenched(EffectContext ctx, int amount, int count, string attack)
        {
            var bench = ctx.OpponentBench();
            int n = Math.Min(count, bench.Count);
            if (n <= 0) return;
            var targets = ctx.ChoosePokemonMany(bench, n, n, attack + ": choose " + n + " of your opponent's Benched Pokémon to damage");
            foreach (var t in targets) ctx.DamageTarget(t, amount);
        }

        private static readonly string[] Throws = { "Rock", "Paper", "Scissors" };

        /// <summary>
        /// Rock-Paper-Scissors until someone wins (Mr. Mime's Tricky Slap, your ruling, v51): the person on screen picks their
        /// throw; a bot throws at random. Ties are thrown again. Returns true if ctx.Player wins.
        /// </summary>
        internal static bool PlayRockPaperScissors(EffectContext ctx)
        {
            for (int round = 1; round <= 100; round++)
            {
                int mine = Throw(ctx, ctx.Player, round);
                int theirs = Throw(ctx, ctx.Opponent, round);
                string line = ctx.Engine.PlayerName(ctx.Player) + " threw " + Throws[mine] + ", " + ctx.Engine.PlayerName(ctx.Opponent) + " threw " + Throws[theirs];
                if (mine == theirs)
                {
                    ctx.Log("Rock-Paper-Scissors: " + line + ". It's a tie, so they throw again.");
                    continue;
                }
                // Paper (1) beats Rock (0), Scissors (2) beat Paper (1), Rock (0) beats Scissors (2).
                bool won = (mine - theirs + 3) % 3 == 1;
                ctx.Log("Rock-Paper-Scissors: " + line + ". " + ctx.Engine.PlayerName(won ? ctx.Player : ctx.Opponent) + " won.");
                return won;
            }
            return false;
        }

        private static int Throw(EffectContext ctx, int player, int round)
        {
            if (!ctx.Engine.IsInteractive(player)) return ctx.State.Rng.Next(3);
            var opts = new List<DecisionOption>();
            for (int i = 0; i < Throws.Length; i++) opts.Add(new DecisionOption { Label = Throws[i], Index = i });
            string prompt = "Rock-Paper-Scissors" + (round > 1 ? " (it was a tie, throw again)" : "") + ": choose your throw";
            int pick = ctx.Engine.AskOne(player, DecisionKind.ChooseOption, prompt, opts, ctx.Source, false);
            return opts[pick].Index;
        }

        // ================================================================== Reusable attacks

        /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks if that damage is N or less."</summary>
        internal sealed class HardenMon : Mon
        {
            private readonly string attack;
            private readonly int threshold;

            public HardenMon(string attack, int threshold)
            {
                this.attack = attack;
                this.threshold = threshold;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.AddEffect(new CardsWHT.SmallDamageShieldEffect(ctx.Pokemon.Name + " takes no damage of " + threshold + " or less from attacks during the opponent's next turn (" + attack + ")",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, threshold));
                return true;
            }
        }

        /// <summary>"Put N damage counters on your opponent's Pokémon in any way you like."</summary>
        internal sealed class CountersAnyWayMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersAnyWayMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(counters, () => ctx.OpponentPokemon());
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Allergy Storm: heads, the opponent can't play Supporter cards during their next turn; tails, no Item cards.</summary>
        private sealed class AllergyStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Allergy Storm") return false;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                ctx.AddEffect(new CardsPAR2.TrainerKindLockEffect("Allergy Storm: no " + (heads ? "Supporter" : "Item") + " cards from the hand", ctx.Player, ctx.Opponent, ctx.Turn + 1, heads));
                return true;
            }
        }

        /// <summary>Lethargy Spores: when played to evolve, you may make both Active Pokémon Asleep and Poisoned. X-Scissor: 50 more on heads.</summary>
        private sealed class ParasectLOR : Mon
        {
            private static readonly Mon scissor = new FlipBonusMon("X-Scissor", 50);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Lethargy Spores", ctx.Me.Active != null && ctx.Opp.Active != null, "Use Lethargy Spores to make both Active Pokémon Asleep and Poisoned?")) return;
                foreach (var active in new[] { ctx.Me.Active, ctx.Opp.Active })
                {
                    if (active == null) continue;
                    ctx.ApplyCondition(active, SpecialCondition.Asleep);
                    ctx.ApplyCondition(active, SpecialCondition.Poisoned);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return scissor.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Entangling String: flip; heads, the Defending Pokémon can't attack during your opponent's next turn. Also Sheer Cold (HSP
        /// Suicune, v135: the attack name is passed in).
        /// </summary>
        internal sealed class EntanglingStringMon : Mon
        {
            private readonly string attack;

            public EntanglingStringMon() : this("Entangling String") { }

            public EntanglingStringMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Stoked Straw: once during your turn, draw until you have 6 cards in your hand. Mega Drain: heal 30.</summary>
        private sealed class BeautiflyLOR : Mon
        {
            private static readonly Mon drain = new CardsTWM1.HealSelfMon("Mega Drain", 30);

            protected override string Activated { get { return "Stoked Straw"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(6); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Nadir Powder: Confused and Poisoned; that Poison puts 8 damage counters in Pokémon Checkup.</summary>
        private sealed class NadirPowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nadir Powder") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 8;
                return true;
            }
        }

        /// <summary>Tearing Gust: 210, then this Pokémon and all attached cards go to the Lost Zone.</summary>
        private sealed class TearingGustMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tearing Gust") return false;
                ctx.DamageDefending(atk.Damage);
                PokemonToLostZone(ctx, ctx.Pokemon, true);
                return true;
            }
        }

        /// <summary>Assassin's Rose: if the opponent's Active Pokémon has a Special Condition, also 60 to 1 of their Benched Pokémon.</summary>
        private sealed class AssassinsRoseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assassin's Rose") return false;
                bool affected = ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage);
                if (affected) ctx.DamageOneBenched(60);
                return true;
            }
        }

        /// <summary>Elder Tree Barrier: no Prize cards for it when Knocked Out by attack damage from the opponent's Pokémon V. Giga Impact: can't attack next turn.</summary>
        private sealed class TrevenantLOR : Mon
        {
            private static readonly Mon impact = new LockAfterAttackMon("Giga Impact", false);

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut != src.Pokemon || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                return knockedOut.HitByAttackerDef != null && knockedOut.HitByAttackerDef.IsPokemonV ? 0 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return impact.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Jamming Attachment: when played to evolve, you may attach up to 3 Energy cards from the opponent's discard pile to their
        /// Pokémon in any way you like. Mysterious Wave: 50 more for each Energy on their Active Pokémon.
        /// </summary>
        private sealed class OrbeetleLOR : Mon
        {
            private static readonly Mon wave = new CardsMEG.DefendingEnergyBonusMon("Mysterious Wave", 50);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Opp.Discard.Any(c => c.Def.IsEnergy) && ctx.Opp.HasPokemonInPlay;
                if (!TriggerOnce(ctx, "Jamming Attachment", ready, "Use Jamming Attachment to attach up to 3 Energy cards from your opponent's discard pile to their Pokémon?")) return;
                for (int i = 0; i < 3; i++)
                {
                    var energy = ctx.Opp.Discard.Where(card => card.Def.IsEnergy).ToList();
                    var targets = ctx.OpponentPokemon(mon => ctx.CanAffect(mon));
                    if (energy.Count == 0 || targets.Count == 0) break;
                    var picks = ctx.ChooseCards(energy, 0, 1, "Jamming Attachment: choose an Energy card from your opponent's discard pile (" + (i + 1) + " of up to 3; choose none to stop)");
                    if (picks.Count == 0) break;
                    var target = ctx.ChoosePokemon(targets, "Jamming Attachment: attach " + picks[0].Def.Name + " to which of your opponent's Pokémon?");
                    if (target == null || !ctx.Opp.Discard.Remove(picks[0])) break;
                    ctx.Engine.AttachEnergyCard(picks[0], target);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return wave.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Lost Volcano: 220, then every Energy attached to this Pokémon goes to the Lost Zone.</summary>
        private sealed class LostVolcanoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Volcano") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    foreach (var e in me.Energy.ToList()) EnergyToLostZone(ctx, me, e);
                return true;
            }
        }

        /// <summary>Mountain Roasting: when played to evolve, you may discard the top 3 cards of the opponent's deck.</summary>
        private sealed class ChandelureLOR : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Mountain Roasting", ctx.Opp.Deck.Count > 0, "Use Mountain Roasting to discard the top 3 cards of your opponent's deck?"))
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
            }
        }

        /// <summary>Magical Fire: 120, put 2 Energy from this Pokémon in the Lost Zone (even with no Bench to hit), and 120 to 1 Benched Pokémon.</summary>
        private sealed class MagicalFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Fire") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0)
                {
                    var lost = me.Energy.Count <= 2 ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), 2, 2, "Magical Fire: put 2 Energy attached to this Pokémon in the Lost Zone");
                    foreach (var e in lost) EnergyToLostZone(ctx, me, e);
                }
                ctx.DamageOneBenched(120);
                return true;
            }
        }

        /// <summary>Scorching Aura: the opponent's Burned Pokémon get 4 damage counters in Pokémon Checkup instead of 2 (doesn't stack). Fire Fang: heads, Burned.</summary>
        private sealed class PyroarLOR : Mon
        {
            private static readonly Mon fang = new CardsPAR2.FlipConditionMon("Fire Fang", SpecialCondition.Burned);

            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                if (src.Pokemon == null || mon.Owner == src.Player || !CardsCRZ1.IsLeadSource(src, "Scorching Aura")) return counters;
                if (src.Engine.EffectsPrevented(mon, EffectSourceKind.Ability, src.Player, true)) return counters;
                return counters + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return fang.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Lordly Songleader: 60 more with Poliwag on the Bench, 90 more with Poliwhirl, 120 more with Poliwrath (they add up).</summary>
        private sealed class LordlySongleaderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lordly Songleader") return false;
                int damage = atk.Damage;
                // Only the card on top counts: a Poliwrath isn't also a Poliwag (Compendium, 2022-09-08).
                if (ctx.Me.Bench.Any(m => m.Name == "Poliwag")) damage += 60;
                if (ctx.Me.Bench.Any(m => m.Name == "Poliwhirl")) damage += 90;
                if (ctx.Me.Bench.Any(m => m.Name == "Poliwrath")) damage += 120;
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Floe Return: shuffle any amount of Water Energy from your Pokémon into your deck; 40 damage for each.</summary>
        private sealed class FloeReturnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Floe Return") return false;
                var holderOf = new Dictionary<int, PokemonInPlay>();
                var water = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var energy in CardsCRZ1.EnergyOfType(ctx, mon, EnergyType.Water))
                    {
                        water.Add(energy);
                        holderOf[energy.Id] = mon;
                    }
                int n = 0;
                if (water.Count > 0)
                {
                    var picks = ctx.ChooseCards(water, 0, water.Count, "Floe Return: shuffle any amount of Water Energy from your Pokémon into your deck (40 damage for each)");
                    foreach (var pick in picks) n += ctx.Engine.ShuffleEnergyIntoDeck(holderOf[pick.Id], new List<CardInstance> { pick });
                }
                if (n > 0) ctx.DamageDefending(40 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Floe Return did no damage.");
                return true;
            }
        }

        /// <summary>Hydro Jet: 20 damage to 1 of the opponent's Pokémon for each Water Energy attached to this Pokémon.</summary>
        private sealed class HydroJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Jet") return false;
                int n = CardsCRZ1.EnergyOfType(ctx, ctx.Pokemon, EnergyType.Water).Count;
                if (n > 0) ctx.DamageAnyOpponent(20 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Hydro Jet did no damage.");
                return true;
            }
        }

        /// <summary>Seething Currents: once during your turn, either player shuffles their hand, puts it on the bottom of their deck and draws 4.</summary>
        private sealed class KingdraLOR : Mon
        {
            protected override string Activated { get { return "Seething Currents"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 || ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                int pick = ctx.ChooseOption("Seething Currents: which player shuffles their hand, puts it on the bottom of their deck and draws 4 cards?",
                    "Seething Currents: you", "Seething Currents: your opponent");
                int player = pick == 0 ? ctx.Player : ctx.Opponent;
                var p = ctx.State.Players[player];
                var hand = p.Hand.ToList();
                if (hand.Count == 0)
                {
                    ctx.Log(ctx.Engine.PlayerName(player) + " had no cards in " + ctx.Engine.Their(player) + " hand.");
                    return;
                }
                p.Hand.Clear();
                ctx.State.Rng.Shuffle(hand);
                p.Deck.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(player) + " put " + ctx.Engine.Their(player) + " hand (" + hand.Count + (hand.Count == 1 ? " card" : " cards") + ") on the bottom of " + ctx.Engine.Their(player) + " deck.");
                ctx.Engine.DrawCards(player, 4);
            }
        }

        /// <summary>Matching Look: each player reveals the top 2 cards of their deck and draws them.</summary>
        private sealed class MatchingLookMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Matching Look") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var deck = ctx.State.Players[player].Deck;
                    var top = deck.Take(Math.Min(2, deck.Count)).ToList();
                    if (top.Count == 0) continue;
                    ctx.Engine.Reveal(player, top, "revealed the top of " + ctx.Engine.Their(player) + " deck");
                    ctx.Engine.DrawCards(player, top.Count);
                }
                return true;
            }
        }

        /// <summary>Oceanic Accompaniment: as often as you like, attach a Water Energy card from your hand to 1 of your Pokémon that has the Swim Freely attack.</summary>
        private sealed class FinneonLOR : Mon
        {
            private static List<PokemonInPlay> Swimmers(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Def.Attacks.Any(a => a.Name == "Swim Freely"));
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (abilityIndex < 0 || abilityIndex >= ctx.Pokemon.Def.Abilities.Count || ctx.Pokemon.Def.Abilities[abilityIndex].Name != "Oceanic Accompaniment") return false;
                if (!ctx.IsMyTurn || !ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water))) return false;
                return Swimmers(ctx).Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.AttachFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Water), () => Swimmers(ctx), "Oceanic Accompaniment", "a Water Energy card");
            }
        }

        /// <summary>Upstream Spirits: 20 for each basic Energy card in your discard pile, then shuffle those cards into your deck.</summary>
        private sealed class UpstreamSpiritsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upstream Spirits") return false;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0)
                {
                    ctx.Log(ctx.Pokemon.Name + "'s Upstream Spirits did no damage.");
                    return true;
                }
                ctx.DamageDefending(20 * energy.Count);
                ctx.DiscardToDeck(energy);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Rapid Freeze: attach any number of Water Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class RapidFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Freeze") return false;
                ctx.DamageDefending(atk.Damage);
                CardsCRZ1.AttachEnergyFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 60, m => true, "Rapid Freeze");
                return true;
            }
        }

        /// <summary>
        /// Glaciated World: once during your turn, discard the top card of your deck; if it's a Water Energy card, attach it to 1 of
        /// your Pokémon. Max Frost: discard any amount of Water Energy from this Pokémon, 50 more for each.
        /// </summary>
        private sealed class KyuremVmax : Mon
        {
            protected override string Activated { get { return "Glaciated World"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                if (discarded.Count == 0) return;
                var card = discarded[0];
                if (!CardsSSP1.IsBasicOf(card.Def, EnergyType.Water) || !ctx.Me.Discard.Contains(card)) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Glaciated World: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Frost") return false;
                var water = CardsCRZ1.EnergyOfType(ctx, ctx.Pokemon, EnergyType.Water);
                int n = 0;
                if (water.Count > 0)
                {
                    var picks = ctx.ChooseCards(water, 0, water.Count, "Max Frost: discard any amount of Water Energy from this Pokémon (50 more damage for each)");
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(ctx.Pokemon, e)) n++;
                }
                ctx.DamageDefending(atk.Damage + 50 * n);
                return true;
            }
        }

        /// <summary>Lost Provisions: with 4 or more cards in your Lost Zone, its attacks cost no Energy. Spit Innocently: not affected by Weakness.</summary>
        private sealed class CramorantLOR : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon != null && mon == src.Pokemon && src.Me.LostZone.Count >= 4) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spit Innocently") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, true, false);
                return true;
            }
        }

        /// <summary>Freeze Down: if the Defending Pokémon is a Basic Pokémon, it can't attack during your opponent's next turn.</summary>
        private sealed class FreezeDownMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Freeze Down") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.IsBasic)
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Freeze Down)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Ace Spark: 120 more if you have used your VSTAR Power.</summary>
        private sealed class AceSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ace Spark") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.VstarUsed ? 120 : 0));
                return true;
            }
        }

        /// <summary>Assault Laser: 80 more if the opponent's Active Pokémon has a Pokémon Tool attached.</summary>
        private sealed class AssaultLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assault Laser") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Tools.Count > 0 ? 80 : 0));
                return true;
            }
        }

        /// <summary>Splitting Beam: 90, and 30 to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class SplittingBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splitting Beam") return false;
                ctx.DamageDefending(atk.Damage);
                DamageBenched(ctx, 30, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Electro Star (VSTAR Power): 90 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class ElectroStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Star") return false;
                DamageBenched(ctx, 90, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Coil: during your next turn, this Pokémon's attacks do 120 more damage to the opponent's Active Pokémon.</summary>
        private sealed class CoilMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coil") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                var boost = new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do 120 more damage during its next turn (Coil)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, 120);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Moon-Watching Party: once during your turn, if this Pokémon is Active, search for a Psychic Energy card for each of your
        /// Benched Clefairy and attach it to that Clefairy. Wonder Storm: 20 for each Psychic Energy on all your Pokémon.
        /// </summary>
        private sealed class ClefairyLOR : Mon
        {
            protected override string Activated { get { return "Moon-Watching Party"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0 && ctx.Me.Bench.Any(m => m.Name == "Clefairy");
            }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var clefairy in ctx.Me.Bench.Where(m => m.Name == "Clefairy").ToList())
                {
                    if (ctx.Me.Deck.Count == 0) break;
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 1, "Moon-Watching Party: choose a Psychic Energy card to attach to " + ctx.Engine.Describe(clefairy));
                    if (found.Count > 0) ctx.Engine.AttachEnergyCard(found[0], clefairy);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wonder Storm") return false;
                int n = ctx.MyPokemon().Sum(m => CardsCRZ1.EnergyOfType(ctx, m, EnergyType.Psychic).Count);
                if (n > 0) ctx.DamageDefending(20 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Wonder Storm did no damage.");
                return true;
            }
        }

        /// <summary>Spirit Charm: your Pokémon take 30 less damage from attacks by the opponent's Dragon Pokémon (only 1 Spirit Charm applies).</summary>
        private sealed class ClefableLOR : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                if (!src.Engine.IsType(info.Attacker, EnergyType.Dragon)) return damage;
                return info.ApplyOnce("Spirit Charm") ? damage - 30 : damage;
            }
        }

        /// <summary>Furtive Drop: 1 damage counter on the opponent's Active Pokémon. The numbers are passed in.</summary>
        internal sealed class FurtiveDropMon : Mon
        {
            private readonly int amount;

            public FurtiveDropMon(int amount)
            {
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Furtive Drop") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, amount);
                return true;
            }
        }

        /// <summary>
        /// Netherworld Gate: once during your turn, from your discard pile, put this Pokémon onto your Bench with 3 damage counters
        /// on it. Screaming Circle: 2 damage counters on the opponent's Active Pokémon for each of their Benched Pokémon.
        /// </summary>
        private sealed class GengarLOR : Mon
        {
            private const string Gate = "Netherworld Gate";

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.BenchSpace > 0
                       && !ctx.NamedLimitUsedThisTurn(Gate + "#" + ctx.Card.Id) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                ctx.MarkNamedLimit(Gate + "#" + ctx.Card.Id);
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Gengar used its Ability Netherworld Gate.", ctx.Card.Id, mon.Uid);
                ctx.Engine.PlaceDamageCounters(mon, 3, Gate);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Screaming Circle") return false;
                int n = ctx.Opp.Bench.Count;
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 2 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Screaming Circle did nothing.");
                return true;
            }
        }

        /// <summary>Tricky Slap: 90, then Rock-Paper-Scissors until someone wins; if you win, prevent all damage and effects of attacks on this Pokémon next turn.</summary>
        private sealed class TrickySlapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tricky Slap") return false;
                ctx.DamageDefending(atk.Damage);
                if (PlayRockPaperScissors(ctx) && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>Alluring Dance: switch in 1 of the opponent's Benched Pokémon; the new Active Pokémon is Confused.</summary>
        private sealed class AlluringDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alluring Dance") return false;
                ctx.DamageDefending(atk.Damage);
                var target = ctx.GustOpponent("Alluring Dance: choose 1 of your opponent's Benched Pokémon to switch in (it becomes Confused)");
                if (target != null) ctx.ApplyCondition(target, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Loving Veil: your Pokémon take 20 less damage from attacks by the opponent's Pokémon V. Psychic: 20 more per Energy on their Active.</summary>
        private sealed class RadiantGardevoir : Mon
        {
            private static readonly Mon psychic = new CardsMEG.DefendingEnergyBonusMon("Psychic", 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target.Owner != src.Player || !info.AgainstOpponent || !info.Attacker.Def.IsPokemonV) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>Lost Mine: only with 10 or more cards in your Lost Zone; 12 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class SableyeLOR : Mon
        {
            private static readonly Mon mine = new CountersAnyWayMon("Lost Mine", 12);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Lost Mine" || ctx.Me.LostZone.Count >= 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return mine.HandleAttack(ctx, atk); }
        }

        /// <summary>Tempting Trap: the Defending Pokémon can't retreat next turn, and during your next turn it takes 90 more damage from attacks.</summary>
        private sealed class TemptingTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tempting Trap") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.DefendingCantRetreat(defending);
                var trap = new DamageTakenEffect(defending.Name + " takes 90 more damage from attacks during " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Tempting Trap)",
                    ctx.Player, defending.Uid, ctx.Turn + 2, 90);
                trap.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(trap);
                return true;
            }
        }

        /// <summary>Puppet Offering: once during your turn, put a Supporter from your discard pile into your hand; if you do, this Pokémon goes to the Lost Zone (attached cards are discarded).</summary>
        private sealed class BanetteLOR : Mon
        {
            protected override string Activated { get { return "Puppet Offering"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => c.Def.IsSupporter) && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            protected override void Ability(EffectContext ctx)
            {
                var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return;
                var pick = ctx.ChooseCards(supporters, 1, 1, "Puppet Offering: put a Supporter card from your discard pile into your hand")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
                if (!ctx.Me.Hand.Contains(pick)) return;
                PokemonToLostZone(ctx, ctx.Pokemon, false);
            }
        }

        /// <summary>Moonglow Reverse: move 2 damage counters from each of your Pokémon to 1 of the opponent's Pokémon.</summary>
        private sealed class MoonglowReverseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moonglow Reverse") return false;
                ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var targets = ctx.OpponentPokemon();
                if (hurt.Count == 0 || targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Moonglow Reverse: move the damage counters to which of your opponent's Pokémon?");
                foreach (var mon in hurt) ctx.Engine.MoveDamageCounters(mon, target, 2, "Moonglow Reverse");
                return true;
            }
        }

        /// <summary>Doom Curse: at the end of the opponent's next turn, the Defending Pokémon is Knocked Out.</summary>
        private sealed class DoomCurseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom Curse") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.AddEffect(new CardsTWM1.DelayedDefendingEffect(defending.Name + " will be Knocked Out at the end of your opponent's next turn (Doom Curse)",
                    ctx.Player, defending.Uid, ctx.Turn, "Doom Curse", 0));
                return true;
            }
        }

        /// <summary>Call Back: put a card from your discard pile into your hand.</summary>
        private sealed class CallBackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call Back") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Discard.Count == 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Call Back: put a card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>
        /// Synchro Kinesis: each player reveals their hand; 90 more if a card in the opponent's hand has the same name as one in
        /// yours. Basic Energy of the same type match too (Compendium, 2022-09-08).
        /// </summary>
        private sealed class SynchroKinesisMon : Mon
        {
            private static bool Match(CardDef a, CardDef b)
            {
                return a.Name == b.Name || (a.IsBasicEnergy && b.IsBasicEnergy && a.EnergyProvides == b.EnergyProvides);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchro Kinesis") return false;
                if (ctx.Me.Hand.Count > 0) ctx.Engine.Reveal(ctx.Player, ctx.Me.Hand.ToList(), "revealed " + ctx.Engine.Their(ctx.Player) + " hand");
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.Reveal(ctx.Opponent, ctx.Opp.Hand.ToList(), "revealed " + ctx.Engine.Their(ctx.Opponent) + " hand");
                bool match = ctx.Opp.Hand.Any(theirs => ctx.Me.Hand.Any(mine => Match(mine.Def, theirs.Def)));
                ctx.DamageDefending(atk.Damage + (match ? 90 : 0));
                return true;
            }
        }

        /// <summary>Worst Gift: 10 for each damage counter on all of the opponent's Pokémon.</summary>
        private sealed class WorstGiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Worst Gift") return false;
                int n = ctx.OpponentPokemon().Sum(m => m.DamageCounters);
                if (n > 0) ctx.DamageDefending(10 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Worst Gift did no damage.");
                return true;
            }
        }

        /// <summary>Night Footsteps: choose 2 of the opponent's Pokémon and put 2 damage counters on each.</summary>
        private sealed class NightFootstepsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Footsteps") return false;
                var all = ctx.OpponentPokemon();
                int n = Math.Min(2, all.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(all, n, n, "Night Footsteps: choose " + n + " of your opponent's Pokémon (2 damage counters each)"))
                    ctx.PlaceCounters(target, 2);
                return true;
            }
        }

        /// <summary>
        /// Guardian of Love: prevent all effects of the opponent's Abilities on each of your Pokémon with Psychic Energy attached,
        /// except Enamorus V. Blossom Tail: attach up to 2 basic Energy from your discard pile to your Benched Pokémon.
        /// </summary>
        private sealed class EnamorusV : Mon
        {
            /// <summary>No passive sources: the Energy check mustn't ask every Ability again while Abilities are being worked out.</summary>
            private static readonly List<PassiveSource> NoSources = new List<PassiveSource>();

            public override bool ShieldsFromOpponentsAbilities { get { return true; } }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (kind != EffectSourceKind.Ability || src.Pokemon == null || target.Owner != src.Player || target.Def.Name == "Enamorus V") return false;
                foreach (var e in target.Energy)
                    if (src.Engine.EnergyUnitsOf(target, e, NoSources).Any(u => EnergyMask.Provides(u, EnergyType.Psychic))) return true;
                return false;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blossom Tail") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 2, (card, mon) => !ctx.IsActive(mon), "Blossom Tail: attach a basic Energy card from your discard pile to your Benched Pokémon");
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Very Vulnerable: 150 more if you have no cards in your hand. Used for Very Vulnerable, All Out (the attack name and the numbers are passed in).</summary>
        internal sealed class VeryVulnerableMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public VeryVulnerableMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count == 0 ? amount : 0));
                return true;
            }
        }

        /// <summary>Splash Loop: 160, then put 2 Energy attached to this Pokémon into your hand.</summary>
        private sealed class SplashLoopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splash Loop") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                var picks = me.Energy.Count <= 2 ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), 2, 2, "Splash Loop: put 2 Energy attached to this Pokémon into your hand");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                return true;
            }
        }

        /// <summary>Crisis Muscles: +150 HP while the opponent has 3 or fewer Prize cards left. Strong-Arm Lariat: you may do 100 more and not attack next turn.</summary>
        private sealed class MachampLOR : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon && !src.State.InSetup && src.Opp.Prizes.Count <= 3 ? hp + 150 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strong-Arm Lariat") return false;
                bool more = ctx.AskYesNo("Strong-Arm Lariat: do 100 more damage? (If you do, this Pokémon can't attack during your next turn.)");
                ctx.DamageDefending(atk.Damage + (more ? 100 : 0));
                if (more) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Geo Cannon: discard the top card of your deck; if it's an Energy card, 100 more and attach it to this Pokémon.</summary>
        private sealed class GeoCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Geo Cannon") return false;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                bool energy = top.Count > 0 && top[0].Def.IsEnergy;
                ctx.DamageDefending(atk.Damage + (energy ? 100 : 0));
                if (energy && ctx.InPlay(ctx.Pokemon) && ctx.Me.Discard.Remove(top[0])) ctx.Engine.AttachEnergyCard(top[0], ctx.Pokemon);
                return true;
            }
        }

        /// <summary>
        /// Ancient Star's gained Ability, until that Aerodactyl VSTAR leaves play: "Your opponent's Pokémon V in play, except any
        /// Aerodactyl VSTAR, have no Abilities." It works from the Bench too and Path to the Peak turns it off (Compendium). Your
        /// ruling (v51): whichever came first wins. Pokémon whose Ability shields them from the opponent's Abilities (Guardian of
        /// Love, Lustrous Body) that were already in play when Ancient Star was used keep their Abilities, and so do the Pokémon
        /// they protect; Pokémon V shielded then by their own Tool or Special Energy keep theirs while that card still shields
        /// them. Anything that comes into play (or evolves) afterwards loses its Abilities.
        /// </summary>
        public sealed class AncientStarEffect : ContinuousEffect
        {
            public const string AbilityName = "Ancient Star";

            private readonly HashSet<int> protectors = new HashSet<int>();
            private readonly HashSet<int> shielded = new HashSet<int>();

            public AncientStarEffect(GameEngine engine, int owner, int aerodactylUid)
                : base("Ancient Star: your opponent's Pokémon V have no Abilities", owner, aerodactylUid, int.MaxValue, false)
            {
                foreach (var mon in engine.State.Players[1 - owner].InPlay())
                {
                    if (mon.Def.Abilities.Count > 0 && engine.Scripts.For(mon.Def).ShieldsFromOpponentsAbilities && engine.HasWorkingAbilityOrPower(mon))
                        protectors.Add(mon.Uid);
                    if (CardShielded(engine, mon)) shielded.Add(mon.Uid);
                }
            }

            /// <summary>
            /// A Tool or Special Energy on the Pokémon prevents effects of the opponent's Abilities on it (Fusion Strike Energy).
            /// The attached cards are asked directly: SourcesOn would check this Pokémon's Abilities, which asks this effect again.
            /// </summary>
            private static bool CardShielded(GameEngine engine, PokemonInPlay mon)
            {
                var attached = new List<CardInstance>();
                if (!engine.ToolsDisabled()) attached.AddRange(mon.Tools);
                foreach (var e in mon.Energy) if (e.Def.IsAttachedSpecialEnergy) attached.Add(e);
                foreach (var card in attached)
                {
                    var ctx = new EffectContext(engine, mon.Owner, card, mon);
                    if (engine.Scripts.For(card.Def).PreventsEffects(ctx, mon, EffectSourceKind.Ability, false)) return true;
                }
                return false;
            }

            public override string GainedAbility(GameEngine engine, PokemonInPlay mon)
            {
                return mon.Uid == TargetUid ? AbilityName : null;
            }

            public override bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth)
            {
                if (mon.Owner == Owner || !mon.Def.IsPokemonV || mon.Def.Name == "Aerodactyl VSTAR") return false;
                // Only the Pokémon's own Abilities (a Seal Stone's VSTAR Power isn't one, Compendium).
                bool printed = false;
                foreach (var a in mon.Def.Abilities) if (a.Name == abilityName) { printed = true; break; }
                if (!printed) return false;
                var aerodactyl = engine.State.FindPokemon(TargetUid);
                if (aerodactyl == null || aerodactyl.Owner != Owner) return false;
                if (engine.AbilityBlockedAtDepth(aerodactyl, AbilityName, depth + 1)) return false;
                if (protectors.Contains(mon.Uid)) return false;
                if (shielded.Contains(mon.Uid) && CardShielded(engine, mon)) return false;
                foreach (int uid in protectors)
                {
                    var protector = engine.State.FindPokemon(uid);
                    if (protector == null || protector.Owner != mon.Owner || protector == mon || protector.Def.Abilities.Count == 0) continue;
                    var script = engine.Scripts.For(protector.Def);
                    if (!script.ShieldsFromOpponentsAbilities || engine.AbilityBlockedAtDepth(protector, protector.Def.Abilities[0].Name, depth + 1)) continue;
                    if (script.PreventsEffects(new EffectContext(engine, protector.Owner, protector.Top, protector), mon, EffectSourceKind.Ability, false)) return false;
                }
                return true;
            }
        }

        /// <summary>Lost Dive: 240, then the top 3 cards of your deck go to the Lost Zone. Ancient Star (VSTAR Power): see AncientStarEffect.</summary>
        private sealed class AerodactylVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lost Dive")
                {
                    ctx.DamageDefending(atk.Damage);
                    var top = ctx.Me.Deck.Take(Math.Min(3, ctx.Me.Deck.Count)).ToList();
                    foreach (var card in top)
                    {
                        ctx.Me.Deck.Remove(card);
                        CardsCRZ1.PutInLostZone(ctx, card);
                    }
                    return true;
                }
                if (atk.Name == "Ancient Star")
                {
                    if (ctx.InPlay(ctx.Pokemon)) ctx.AddEffect(new AncientStarEffect(ctx.Engine, ctx.Player, ctx.Pokemon.Uid));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hurricane Shock: 4 coins, 50 for each heads; with 2 or more heads the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class HurricaneShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hurricane Shock") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(4);
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(50 * heads);
                if (heads >= 2 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Battle Step: 50, then search for up to 2 Fighting Energy cards and attach them to your Benched Pokémon in any way you like.</summary>
        private sealed class BattleStepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Battle Step") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fighting), 2, "Battle Step: choose up to 2 Fighting Energy cards to attach to your Benched Pokémon");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, mon) => !ctx.IsActive(mon), "Battle Step");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Recover: discard an Energy from this Pokémon and heal all damage from it.</summary>
        internal sealed class RecoverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Recover") return false;
                var me = ctx.Pokemon;
                if (me.Energy.Count == 0) return true;
                var pick = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Recover: discard an Energy from this Pokémon")[0];
                if (ctx.Engine.DiscardEnergy(me, pick)) ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Lost Block: the opponent puts any Prize cards they take in their Lost Zone instead of their hand.</summary>
        private sealed class BarbaracleLOR : Mon
        {
            public override bool SendsPrizesToLostZone(EffectContext src, int taker)
            {
                return src.Pokemon != null && taker != src.Player;
            }
        }

        /// <summary>Fighting Headbutt: not affected by Weakness; 50 more against a Pokémon V.</summary>
        private sealed class FightingHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fighting Headbutt") return false;
                var defending = ctx.Defending;
                int damage = atk.Damage + (defending != null && defending.Def.IsPokemonV ? 50 : 0);
                ctx.DamageWith(defending, damage, false, true, false);
                return true;
            }
        }

        /// <summary>Power Stone: attach up to 2 Fighting Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class PowerStoneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Stone") return false;
                ctx.DamageDefending(atk.Damage);
                CardsCRZ1.AttachEnergyFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Fighting), 2, m => true, "Power Stone");
                return true;
            }
        }

        /// <summary>Lost Shot: 120, then the top card of the opponent's deck goes to the Lost Zone.</summary>
        private sealed class LostShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Shot") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                var card = ctx.Opp.Deck[0];
                ctx.Opp.Deck.RemoveAt(0);
                CardsCRZ1.PutInLostZone(ctx, card);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Night Cyclone: 160, then move all Energy from this Pokémon to your Benched Pokémon in any way you like.</summary>
        private sealed class NightCycloneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                foreach (var e in me.Energy.ToList())
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Night Cyclone: move " + e.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(me, target, e);
                }
                return true;
            }
        }

        /// <summary>Sucker Punch and Turn: 60, then switch this Pokémon with 1 of your Benched Darkness Pokémon.</summary>
        private sealed class SuckerPunchAndTurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sucker Punch and Turn") return false;
                ctx.DamageDefending(atk.Damage);
                var dark = ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Darkness)).ToList();
                if (!ctx.PokemonIsActive || dark.Count == 0) return true;
                var target = ctx.ChoosePokemon(dark, "Sucker Punch and Turn: switch this Pokémon with which of your Benched Darkness Pokémon?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Player, target);
                return true;
            }
        }

        /// <summary>Cursed Message: Knocked Out by the opponent's attack damage, search your deck for a card. Chain of Spirits: 60 more per Spiritomb in your discard pile.</summary>
        private sealed class SpiritombLOR : Mon
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Me.Deck.Count == 0) return;
                Announce(src, "Cursed Message");
                src.SearchToHand(d => true, 1, "Cursed Message: put any card from your deck into your hand", false, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chain of Spirits") return false;
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Name == "Spiritomb");
                ctx.DamageDefending(atk.Damage + 60 * n);
                return true;
            }
        }

        /// <summary>Hazard Star (VSTAR Power): the opponent's Active Pokémon is Paralyzed and Poisoned (3 counters a Checkup). Big Bang Arm: 10 less per damage counter on it.</summary>
        private sealed class DrapionVstar : Mon
        {
            protected override string Activated { get { return "Hazard Star"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                if (ctx.ApplyCondition(target, SpecialCondition.Poisoned)) target.PoisonCounterBase = 3;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Bang Arm") return false;
                int damage = Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log(ctx.Pokemon.Name + "'s Big Bang Arm did no damage.");
                return true;
            }
        }

        /// <summary>Hand of Djinn: search your deck for a Darkness Energy card and attach it to 1 of your Pokémon.</summary>
        private sealed class HandOfDjinnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hand of Djinn") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Darkness), 1, "Hand of Djinn: choose a Darkness Energy card");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Hand of Djinn: attach " + found[0].Def.Name + " to which of your Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(found[0], target);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Poison Peak: 2 more damage counters on the opponent's Poisoned Pokémon in Pokémon Checkup. Poison Jab: Poisoned.</summary>
        private sealed class RadiantHisuianSneasler : Mon
        {
            private static readonly Mon jab = new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned);

            public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                if (src.Pokemon == null || mon.Owner == src.Player) return counters;
                // Fusion Strike Energy blocks it: it's an effect of an Ability on that Pokémon (Compendium, 2022-09-29).
                if (src.Engine.EffectsPrevented(mon, EffectSourceKind.Ability, src.Player, true)) return counters;
                return counters + 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return jab.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Energy Stream: 20, then attach up to 2 Metal Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class EnergyStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Stream") return false;
                ctx.DamageDefending(atk.Damage);
                var metal = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
                if (metal.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                var picks = ctx.ChooseCards(metal, 0, Math.Min(2, metal.Count), "Energy Stream: attach up to 2 Metal Energy cards from your discard pile to this Pokémon");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Destructive Finish: discard cards from the top of your deck until 1 is left; 30 more for each Energy card discarded.</summary>
        private sealed class DestructiveFinishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Destructive Finish") return false;
                int n = Math.Max(0, ctx.Me.Deck.Count - 1);
                var gone = n > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, n) : new List<CardInstance>();
                int energy = gone.Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 30 * energy);
                return true;
            }
        }

        /// <summary>Field Trap: if the opponent has a Stadium in play, discard it; if you did, discard 2 Energy from their Active Pokémon.</summary>
        private sealed class FieldTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Field Trap") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium == null || ctx.State.StadiumOwner != ctx.Opponent) return true;
                ctx.Engine.DiscardStadium();
                var active = ctx.Opp.Active;
                if (active != null && ctx.CanAffect(active)) ctx.DiscardEnergyFrom(active, 2, "Field Trap: discard 2 Energy from your opponent's Active Pokémon");
                return true;
            }
        }

        /// <summary>Windup Beam: with a Pokémon Tool attached, 60 more and the opponent's Active Pokémon is Confused.</summary>
        private sealed class WindupBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Windup Beam") return false;
                bool tool = ctx.Pokemon.Tools.Count > 0;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (tool ? 60 : 0));
                if (tool && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Treasure Rush: 20 for each card in your hand.</summary>
        private sealed class TreasureRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Treasure Rush") return false;
                if (ctx.Me.Hand.Count > 0) ctx.DamageDefending(20 * ctx.Me.Hand.Count);
                else ctx.Log(ctx.Pokemon.Name + "'s Treasure Rush did no damage.");
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Abyss Seeking: look at the top 4 cards, put 2 into your hand and the rest in the Lost Zone.</summary>
        private sealed class AbyssSeekingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Abyss Seeking") return false;
                ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(4);
                if (top.Count == 0) return true;
                int keep = Math.Min(2, top.Count);
                var picks = ctx.ChooseCards(top, keep, keep, "Abyss Seeking: put 2 of these cards into your hand (the others go to the Lost Zone)");
                ctx.DeckToHand(picks);
                foreach (var card in top)
                {
                    if (picks.Contains(card) || !ctx.Me.Deck.Remove(card)) continue;
                    CardsCRZ1.PutInLostZone(ctx, card);
                }
                return true;
            }
        }

        /// <summary>Moisture Star (VSTAR Power): during your turn, heal all damage from this Pokémon. Rolling Iron: takes 80 less next turn.</summary>
        private sealed class HisuianGoodraVstar : Mon
        {
            private static readonly Mon iron = new CardsASC.SelfGuardMon("Rolling Iron", 80);

            protected override string Activated { get { return "Moisture Star"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Pokemon.DamageCounters > 0 && !ctx.Engine.HealingBlocked(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Pokemon, ctx.Pokemon.DamageCounters * 10); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return iron.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Vanishing Wings: once during your turn, from the Bench, shuffle this Pokémon and all attached cards into your deck. Flight Surf: 80 more with your Stadium in play.</summary>
        private sealed class PidgeotV : Mon
        {
            private static readonly Mon surf = new CardsPAL1.StadiumBonusMon("Flight Surf", 80);

            protected override string Activated { get { return "Vanishing Wings"; } }
            protected override bool AbilityReady(EffectContext ctx) { return !ctx.PokemonIsActive; }
            protected override void Ability(EffectContext ctx) { ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return surf.HandleAttack(ctx, atk); }
        }

        /// <summary>Branch Calculation: look at the top 4 cards of either player's deck and put them back in any order.</summary>
        private sealed class BranchCalculationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Branch Calculation") return false;
                ctx.DamageDefending(atk.Damage);
                int pick = ctx.ChooseOption("Branch Calculation: look at the top 4 cards of which deck?", "Your deck", "Your opponent's deck");
                int player = pick == 0 ? ctx.Player : ctx.Opponent;
                var deck = ctx.State.Players[player].Deck;
                var top = deck.Take(Math.Min(4, deck.Count)).ToList();
                if (top.Count == 0) return true;
                ctx.Engine.NoteLookedAt(top, "Top " + top.Count + " cards of " + (player == ctx.Player ? "your" : "your opponent's") + " deck", ctx.Player);
                deck.RemoveRange(0, top.Count);
                var order = new List<CardInstance>();
                var rest = top.ToList();
                while (rest.Count > 1)
                {
                    var next = ctx.ChooseCards(rest, 1, 1, "Branch Calculation: choose the card to put " + (order.Count == 0 ? "on top" : "next (card " + (order.Count + 1) + " from the top)"))[0];
                    order.Add(next);
                    rest.Remove(next);
                }
                order.AddRange(rest);
                deck.InsertRange(0, order);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put the top " + top.Count + " cards of " + ctx.Engine.Their(player) + " deck back in an order of their choice.");
                return true;
            }
        }

        /// <summary>Garbage Attack: 20 for each Pokémon Tool card in both players' Lost Zones.</summary>
        private sealed class GarbageAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Garbage Attack") return false;
                int n = ctx.Me.LostZone.Count(c => c.Def.IsTool) + ctx.Opp.LostZone.Count(card => card.Def.IsTool);
                if (n > 0) ctx.DamageDefending(20 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Garbage Attack did no damage.");
                return true;
            }
        }

        /// <summary>Downgrading Beam: devolve 1 of the opponent's evolved Pokémon by any number of Evolution cards; they shuffle those into their deck.</summary>
        private sealed class DowngradingBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Downgrading Beam") return false;
                ctx.DamageDefending(atk.Damage);
                var evolved = ctx.OpponentPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution && !m.Def.IsLegend);
                if (evolved.Count == 0) return true;
                var target = ctx.ChoosePokemon(evolved, "Downgrading Beam: devolve which of your opponent's Pokémon?");
                if (target == null) return true;
                int max = target.Stack.Count - 1;
                int count = max <= 1 ? 1 : ctx.AskNumber(1, max, "Downgrading Beam: remove how many Evolution cards from " + target.Name + "?");
                ctx.Engine.Devolve(target, count, true);
                return true;
            }
        }

        /// <summary>Unfazed Fat: prevent all effects of the opponent's attacks on this Pokémon (damage isn't an effect). Thumping Snore: 180, then it's Asleep and flips 2 coins in Checkup.</summary>
        private sealed class SnorlaxLOR : Mon
        {
            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thumping Snore") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                // A newer Asleep replaces this one's 2 coins (Compendium, 2022-09-08): ApplyCondition sets 1 coin for any new Asleep.
                if (ctx.InPlay(me) && ctx.ApplyCondition(me, SpecialCondition.Asleep)) me.SleepCoins = 2;
                return true;
            }
        }

        /// <summary>Mischievous Tail: look at the top card of the opponent's deck; you may have them shuffle their deck.</summary>
        private sealed class MischievousTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mischievous Tail") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count == 0) return true;
                var top = ctx.Opp.Deck[0];
                ctx.Engine.NoteLookedAt(new List<CardInstance> { top }, "Top card of your opponent's deck", ctx.Player);
                int pick = ctx.ChooseOption("Mischievous Tail: the top card of your opponent's deck is " + top.Def.Name + ".", "Leave it on top", "Have your opponent shuffle their deck");
                if (pick == 1) ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>All-You-Can-Grab: flip until tails; search your deck for up to that many cards.</summary>
        private sealed class AllYouCanGrabMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "All-You-Can-Grab") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(d => true, heads, "All-You-Can-Grab: choose up to " + heads + (heads == 1 ? " card" : " cards") + " to put into your hand", false);
                return true;
            }
        }

        /// <summary>Shadow Cyclone: 130, then move an Energy from this Pokémon to 1 of your Benched Pokémon. Used for Shadow Cyclone, Transfer Break (the attack name is passed in).</summary>
        internal sealed class ShadowCycloneMon : Mon
        {
            private readonly string attack;

            public ShadowCycloneMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CardsDRI1.MoveEnergyToBench(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Lost Headbutt: 50, then put an Energy attached to the opponent's Active Pokémon in the Lost Zone.</summary>
        private sealed class LostHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Headbutt") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return true;
                var pick = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Lost Headbutt: put an Energy attached to your opponent's Active Pokémon in the Lost Zone")[0];
                EnergyToLostZone(ctx, defending, pick);
                return true;
            }
        }

        /// <summary>All Just a Dream: no Prize cards for it if it's Asleep when Knocked Out by the opponent's attack damage. Collapse: 60, then it's Asleep.</summary>
        private sealed class KomalaLOR : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut != src.Pokemon || taker == src.Player) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                return knockedOut.Has(SpecialCondition.Asleep) ? 0 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Collapse") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Spill Out: discard your hand; 150 more if you discarded 5 or more cards.</summary>
        private sealed class SpillOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spill Out") return false;
                int n = ctx.Me.Hand.Count;
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.DamageDefending(atk.Damage + (n >= 5 ? 150 : 0));
                return true;
            }
        }

        // ================================================================== Secret rares

        /// <summary>Tantrum Blast: 100 for each Special Condition affecting this Pokémon.</summary>
        private sealed class TantrumBlastMon : Mon
        {
            private static readonly SpecialCondition[] EveryCondition =
            {
                SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned
            };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantrum Blast") return false;
                int n = EveryCondition.Count(x => ctx.Pokemon.Has(x));
                if (n > 0) ctx.DamageDefending(100 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Tantrum Blast did no damage.");
                return true;
            }
        }

        /// <summary>Rising Sword: 50 more for each Prize card you have taken.</summary>
        private sealed class RisingSwordMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rising Sword") return false;
                ctx.DamageDefending(atk.Damage + 50 * ctx.MyPrizesTaken);
                return true;
            }
        }
    }
}
