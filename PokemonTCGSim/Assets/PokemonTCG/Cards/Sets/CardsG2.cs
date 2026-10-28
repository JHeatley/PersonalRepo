using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Gym Challenge (G2 1-132, 2000; Unlimited only). 17-20 are the holo Blaine, Giovanni, Koga and Sabrina (prints of 100, 104,
    /// 106 and 110); Rocket's Zapdos (15), Erika's Jigglypuff (69) and Sabrina's Abra (93) are prints of Celebrations, 30th
    /// Celebration and Wizards promo cards; the basic Energy (127-132) are Mega Evolution Energy's. Your rulings (v126):
    /// Rocket's Minefield Gym puts 2 damage counters; Lt. Surge's Secret Plan follows its printed text; Blaine's Quiz #3's guess
    /// is from every Pokémon name; Shapeshift makes Brock's Ninetales the attached Pokémon (name, type, HP, Weakness, Resistance,
    /// Retreat Cost, attacks and Pokémon Powers); Sabrina's Psychic Control can use Items and Supporters (a Supporter doesn't count toward the 1 per
    /// turn); Blaine stays in force for the turn.
    /// </summary>
    public static class CardsG2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("G2-1", new CardsBS1.DiscardCostMon("Firestorm", EnergyType.Fire, 3, null, new RecoilMon("Heat Tackle", 10)));
            r.RegisterByKey("G2-2", new ComboMon(new RoaringFlamesMon(), new FlameJetMon()));
            r.RegisterByKey("G2-3", new NinetalesShapeshift());
            r.RegisterByKey("G2-4", new ComboMon(new GrowthMon(), new WideSolarbeamMon()));
            r.RegisterByKey("G2-5", new ComboMon(new SummonStormMon(), new DragonTornadoMon()));
            r.RegisterByKey("G2-6", new MachampFortitude());
            r.RegisterByKey("G2-7", new ComboMon(new IntimidateMon(), new CardsWHT.FlipMoreMon("Tumbling Attack", 1, 30)));
            r.RegisterByKey("G2-8", new PersianCallTheBoss());
            r.RegisterByKey("G2-9", new CardsG1.CheckedCombo(new BothConditionsFlipMon("Nerve Poison", SpecialCondition.Paralyzed, SpecialCondition.Poisoned, 1), new OncePerStayMon("Hyper Needle")));
            r.RegisterByKey("G2-10", new KogasDitto());
            r.RegisterByKey("G2-11", new ComboMon(new KerzapMon(), new ThundertackleMon()));
            r.RegisterByKey("G2-12", new ComboMon(new ElectroBeamMon("Electro Beam"), new SuperRemovalMon()));
            r.RegisterByKey("G2-13", new GyaradosRebellion());
            r.RegisterByKey("G2-14", new ComboMon(new JuxtaposeMon(), new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("G2-16", new AlakazamPsylink());
            r.RegisterByKey("G2-21", new NinetalesHealingFire());
            r.RegisterByKey("G2-22", new CardsG1.CheckedCombo(new LieLowMon(), new EarthdrillMon()));
            r.RegisterByKey("G2-23", new LoveLariatMon());
            r.RegisterByKey("G2-24", new ComboMon(new CardsWHT.FlipMoreMon("Snapping Pincers", 1, 10), new OverheadTossMon()));
            r.RegisterByKey("G2-25", new ComboMon(new CardsRCL2.SelfConditionMon("Poison Buildup", SpecialCondition.Poisoned), new PoisonPowerMon()));
            r.RegisterByKey("G2-26", new MukEnergyDrain());
            r.RegisterByKey("G2-27", new ComboMon(new CoinsMon("Quick Turn", 2), new AerialManeuversMon()));
            r.RegisterByKey("G2-28", new ComboMon(new HighVoltageMon(), new ThunderFlareMon()));
            r.RegisterByKey("G2-29", new ComboMon(new DarkWaveMon(), new NoRetreatMon("Shadow Bind")));
            r.RegisterByKey("G2-30", new ComboMon(new DamageShiftMon(), new CardsWHT.FlipMoreMon("Water Spray", 1, 20)));
            r.RegisterByKey("G2-31", new BonfireMon());
            r.RegisterByKey("G2-32", new TailsNothingMon("Mega Peck"));
            r.RegisterByKey("G2-33", new StampMon());
            r.RegisterByKey("G2-34", new DetonateMon());
            r.RegisterByKey("G2-35", new PrimeapeScram());
            r.RegisterByKey("G2-36", new ComboMon(new CardsCRZ1.PiercingMon("Swift"), new CardsPAR2.FlipConditionMon("Needle Ball", SpecialCondition.Poisoned)));
            r.RegisterByKey("G2-37", new ComboMon(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep), new Cards30CC.BenchHitMon("Fire Ring", 10)));
            r.RegisterByKey("G2-38", new BellsproutSoakUp());
            r.RegisterByKey("G2-39", new ComboMon(new ConditionAttackMon("Sleep Seed", SpecialCondition.Asleep), new FlipSearchMon("Errand-Running", d => d.IsTrainer, "a Trainer card")));
            r.RegisterByKey("G2-40", new LunarPowerMon());
            r.RegisterByKey("G2-41", new IvysaurRelaxingScent());
            r.RegisterByKey("G2-42", new ComboMon(new CardsG1.EggBombMon("Risky Attack", 100), new HeadlockMon()));
            r.RegisterByKey("G2-43", new ComboMon(new FalseCharityMon(), new CoinsMon("Double Scratch", 2)));
            r.RegisterByKey("G2-44", new ComboMon(new PoisonStingTackleMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("G2-45", new RendMon());
            r.RegisterByKey("G2-46", new CardsPAR2.FlipConditionMon("Sonic Scream", SpecialCondition.Confused));
            r.RegisterByKey("G2-47", new KakunaEmerge());
            r.RegisterByKey("G2-48", new ComboMon(new CardsMEG.SandAttackMon("Smokescreen"), new ObscuringGasMon()));
            r.RegisterByKey("G2-49", new MessengerMon());
            r.RegisterByKey("G2-50", new ComboMon(new FlipBonusRecoilMon("Spontaneous Explosion", 30, 30), new ToxicFlipMon("Toxic Cloud")));
            r.RegisterByKey("G2-51", new SurpriseMon());
            r.RegisterByKey("G2-52", new ElectrodeShockBlast());
            r.RegisterByKey("G2-53", new FocusEnergyMon("Double-edge", 20));
            r.RegisterByKey("G2-54", new ComboMon(new IceThrowMon(), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("G2-55", new ComboMon(new ConditionAttackMon("Nightmare", SpecialCondition.Asleep), new FlipBenchHitMon("Shadow Attack", 30)));
            r.RegisterByKey("G2-56", new ComboMon(new InvigorateMon(), new PendulumCurseMon()));
            r.RegisterByKey("G2-57", new ComboMon(new HelpingHandMon(), new FlipNoRetreatMon("Hug")));
            r.RegisterByKey("G2-58", new LifeDrainMon());
            r.RegisterByKey("G2-59", new MagicDartsMon());
            r.RegisterByKey("G2-60", new FireTailSlapMon());
            r.RegisterByKey("G2-61", new ComboMon(new TailsNothingMon("Wild Kick"), new RetaliateMon("Retaliate")));
            r.RegisterByKey("G2-62", new ComboMon(new StokeMon(), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("G2-63", new ComboMon(new PranksMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("G2-64", new HindKickMon());
            r.RegisterByKey("G2-65", new ComboMon(new TailsNothingMon("Horn Charge"), new FlipBenchHitMon("Overrun", 20)));
            r.RegisterByKey("G2-66", new CallWillOTheWispMon());
            r.RegisterByKey("G2-67", new ComboMon(new TailsNothingMon("Surprise Attack"), new TremorMon()));
            r.RegisterByKey("G2-68", new CoinsMon("Rock Toss", 3));
            r.RegisterByKey("G2-70", new StrangePowderMon());
            r.RegisterByKey("G2-71", new PoisonSporeMon());
            r.RegisterByKey("G2-72", new FuryPunchMon());
            r.RegisterByKey("G2-73", new CardsG1.CheckedCombo(new OncePerStayMon("Ancestral Memory"), new CoinsMon("Flail Around", 3)));
            r.RegisterByKey("G2-74", new CardsPAR2.FlipConditionMon("Cat Fleas", SpecialCondition.Confused));
            r.RegisterByKey("G2-75", new ComboMon(new TailsNothingMon("Horn Thrust"), new RecoilMon("Double-edge", 20)));
            r.RegisterByKey("G2-76", new CardsG1.CheckedCombo(new CoinsMon("Double Kick", 2), new RetaliationMon()));
            r.RegisterByKey("G2-77", new BothConditionsFlipMon("Fast-Acting Poison", SpecialCondition.Confused, SpecialCondition.Poisoned, 2));
            r.RegisterByKey("G2-78", new SludgeGripMon());
            r.RegisterByKey("G2-79", new SmellyGasMon());
            r.RegisterByKey("G2-80", new CardsMEG.SandAttackMon("Sand-attack"));
            r.RegisterByKey("G2-81", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new GraspingVineMon("Grasping Vine", 2)));
            r.RegisterByKey("G2-82", new SharpStingerMon());
            r.RegisterByKey("G2-83", new GroupAttackMon());
            r.RegisterByKey("G2-84", new CardsWHT.FlipMoreMon("Quick Attack", 1, 20));
            r.RegisterByKey("G2-85", new FocusEnergyMon("Quick Attack", 0));
            r.RegisterByKey("G2-86", new CardsFST1.FlipRecoilMon("Bouncing Ball", 10));
            r.RegisterByKey("G2-87", new InkSpurtMon());
            r.RegisterByKey("G2-88", new DodgeMon("Play Dead", false));
            r.RegisterByKey("G2-89", new ComboMon(new BubblesMon(), new AmnesiaMon()));
            r.RegisterByKey("G2-90", new EspMon());
            r.RegisterByKey("G2-91", new ComboMon(new NoRetreatMon("Frostbite"), new CardsMEG.SandAttackMon("Mirage")));
            r.RegisterByKey("G2-92", new StarBoomerangMon());
            r.RegisterByKey("G2-94", new ComboMon(new PsyscanMon(), new CardsWHT.FlipMoreMon("Quick Attack", 1, 20)));
            r.RegisterByKey("G2-95", new ComboMon(new SearchAttackMon("Energy Support", d => d.IsEnergy && d.IsBasicEnergy && d.EnergyProvides == EnergyType.Psychic, 1), new CardsFCO2.NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("G2-96", new ComboMon(new ParalyzeFlipMon("Lick"), new FadeOutMon()));
            r.RegisterByKey("G2-97", new GastlyGaseousForm());
            r.RegisterByKey("G2-98", new BarrierAttackMon());
            r.RegisterByKey("G2-99", new RandomEspMon());

            r.RegisterByKey("G2-100", new BlaineTrainer());
            r.RegisterByKey("G2-101", new BrocksProtection());
            r.RegisterByKey("G2-102", new ChaosGym());
            r.RegisterByKey("G2-103", new ErikasKindness());
            r.RegisterByKey("G2-104", new GiovanniTrainer());
            r.RegisterByKey("G2-105", new GiovannisLastResort());
            r.RegisterByKey("G2-106", new KogaTrainer());
            r.RegisterByKey("G2-107", new LtSurgesSecretPlan());
            r.RegisterByKey("G2-108", new MistysWish());
            r.RegisterByKey("G2-109", new ResistanceGym());
            r.RegisterByKey("G2-110", new SabrinaTrainer());
            r.RegisterByKey("G2-111", new BlainesQuiz2());
            r.RegisterByKey("G2-112", new BlainesQuiz3());
            r.RegisterByKey("G2-113", new CinnabarCityGym());
            r.RegisterByKey("G2-114", new FuchsiaCityGym());
            r.RegisterByKey("G2-115", new KogasNinjaTrick());
            r.RegisterByKey("G2-116", new MasterBallG2());
            r.RegisterByKey("G2-117", new MaxReviveG2());
            r.RegisterByKey("G2-118", new MistysTears());
            r.RegisterByKey("G2-119", new RocketsMinefieldGym());
            r.RegisterByKey("G2-120", new RocketsSecretExperiment());
            r.RegisterByKey("G2-121", new SabrinasPsychicControl());
            r.RegisterByKey("G2-122", new SaffronCityGym());
            r.RegisterByKey("G2-123", new ViridianCityGym());
            r.RegisterByKey("G2-124", new Fervor());
            r.RegisterByKey("G2-125", new TransparentWalls());
            r.RegisterByKey("G2-126", new WarpPoint());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        private static bool Owned(CardDef def, string owner) { return def != null && def.IsOwnersPokemon(owner); }

        /// <summary>A Pokémon in play with the owner's name (by its name now: a Shapeshifted Brock's Ninetales has the name of the Pokémon it's treated as).</summary>
        private static bool Owned(PokemonInPlay mon, string owner) { return mon != null && mon.Name.StartsWith(owner + "'s "); }

        private static bool IsFireCard(EffectContext ctx, PokemonInPlay mon, CardInstance card)
        {
            return card.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, mon, card, EnergyType.Fire);
        }

        private static bool IsFireEnergyCard(CardDef def) { return def.IsEnergy && def.EnergyProvides == EnergyType.Fire; }

        private static List<CardInstance> OtherCards(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card).ToList(); }

        /// <summary>The Pokémon is the opponent's Defending Pokémon still (after the damage).</summary>
        private static bool Still(EffectContext ctx, PokemonInPlay defending) { return TefPokemon.StillDefending(ctx, defending); }

        /// <summary>The printed Pokémon Power name at that index.</summary>
        private static bool IsPower(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        // ================================================================== Pokémon Powers

        /// <summary>
        /// Shapeshift (your ruling): once during your turn (before your attack), attach an Evolution card from your hand to this
        /// Pokémon (not evolving); it's treated as that Pokémon (GameEngine.ShapeshiftCard): its name, type, HP, Weakness,
        /// Resistance, Retreat Cost, attacks and Pokémon Powers. It can't evolve or devolve. During your turn you may discard the
        /// attached Evolution card. Not while Asleep, Confused or Paralyzed, and if it becomes one of those, every Evolution card
        /// attached to it is discarded.
        /// </summary>
        private sealed class NinetalesShapeshift : Mon
        {
            private const string Power = "Shapeshift";
            private const string AttachedThisTurn = "Shapeshift attach";

            private static List<CardInstance> Evolutions(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsEvolution).ToList(); }

            private static bool CanAttach(EffectContext ctx)
            {
                return PowerWorks(ctx.Pokemon) && !ctx.AbilityUsedThisTurn(AttachedThisTurn) && Evolutions(ctx).Count > 0;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (!IsPower(ctx, abilityIndex, Power) || !ctx.IsMyTurn || ctx.Pokemon == null) return false;
                return CanAttach(ctx) || ctx.Pokemon.Attached.Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                var labels = new List<string>();
                var attach = CanAttach(ctx) ? Evolutions(ctx).GroupBy(c => c.Def.BaseKey).Select(g => g.First()).ToList() : new List<CardInstance>();
                foreach (var card in attach) labels.Add("Attach " + card.Def.Name + " (treat " + me.Name + " as it)");
                var held = me.Attached.ToList();
                foreach (var card in held) labels.Add("Discard the attached " + card.Def.Name);
                if (labels.Count == 0) return;
                int pick = labels.Count == 1 ? 0 : ctx.ChooseOption(Power + ": choose 1", labels.ToArray());
                if (pick < attach.Count)
                {
                    var card = attach[pick];
                    if (!ctx.Me.Hand.Remove(card)) return;
                    ctx.MarkAbilityUsed(AttachedThisTurn);
                    me.Attached.Add(card);
                    ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.OwnedName(me) + " is now treated as " + card.Def.Name + " (Shapeshift).", card.Id, me.Uid);
                    return;
                }
                var drop = held[pick - attach.Count];
                ctx.Engine.DiscardAttachedCards(me, new List<CardInstance> { drop }, Power);
            }

            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || me.Attached.Count == 0 || PowerWorks(me)) return;
                src.Engine.DiscardAttachedCards(me, me.Attached.Where(c => c.Def.IsEvolution).ToList(), "Shapeshift: " + me.Name + " has a Special Condition");
            }
        }

        /// <summary>Fortitude: if an opponent's attack would Knock Out this Pokémon, flip a coin; heads, it has 10 HP left instead. Not if it was already Asleep, Confused or Paralyzed.</summary>
        private sealed class MachampFortitude : Mon
        {
            private static readonly Mon Punch = new CoinsMon("Hurricane Punch", 4);

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || src.Engine.CurrentSourceKind != EffectSourceKind.Attack || src.Engine.CurrentSourceOwner == me.Owner) return false;
                if (!CardsBS1.WorkedBeforeAttack(src, me)) return false;
                Announce(src, "Fortitude");
                return src.Engine.FlipCoin(me.Owner, "Fortitude");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Punch.HandleAttack(ctx, atk); }
        }

        /// <summary>Call the Boss: when you play this Pokémon from your hand to evolve, you may search your deck for Giovanni and put it into your hand.</summary>
        private sealed class PersianCallTheBoss : Mon
        {
            private static readonly Mon Ambush = new CardsWHT.FlipMoreMon("Ambush", 1, 20);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!PowerWorks(ctx.Pokemon) || ctx.Me.Deck.Count == 0) return;
                if (!TriggerOnce(ctx, "Call the Boss", true, "Call the Boss: search your deck for Giovanni?")) return;
                ctx.SearchToHand(d => d.IsTrainer && d.Name == "Giovanni", 1, "Call the Boss: choose Giovanni");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ambush.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rebellion: whenever this Pokémon attacks, flip 2 coins; both tails, the attack does nothing and this Pokémon and its cards
        /// are shuffled into your deck. Works even if it's Confused.
        /// </summary>
        private sealed class GyaradosRebellion : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, "Rebellion") || me.Name != "Misty's Gyarados") return false;
                if (ctx.FlipCoins(2) > 0) return false;
                Announce(ctx, "Rebellion");
                Nothing(ctx);
                if (ctx.InPlay(me)) ctx.Engine.ShufflePokemonIntoDeck(me);
                return true;
            }
        }

        /// <summary>Psylink: this Pokémon has a copy of every attack your Psychic Pokémon in play have (costs included). Not while Asleep, Confused or Paralyzed. Mega Burn: can't be used next turn.</summary>
        private sealed class AlakazamPsylink : Mon
        {
            private static readonly Mon Burn = new LockAfterAttackMon("Mega Burn", true);

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !PowerWorks(me)) yield break;
                foreach (var mon in src.MyPokemon(m => m != me))
                {
                    if (!src.Engine.IsType(mon, EnergyType.Psychic)) continue;
                    for (int i = 0; i < mon.Def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(mon.Def, i);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burn.HandleAttack(ctx, atk); }
        }

        /// <summary>Healing Fire: whenever you attach a Fire Energy card from your hand to this Pokémon, remove 1 damage counter from it. Burn Up: tails, discard all Fire Energy cards.</summary>
        private sealed class NinetalesHealingFire : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !PowerWorks(me) || me.DamageCounters == 0 || !IsFireCard(src, me, energy)) return;
                Announce(src, "Healing Fire");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Up") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!heads && ctx.InPlay(me))
                    foreach (var card in me.Energy.Where(c => IsFireCard(ctx, me, c)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Energy Drain: if an opponent's attack damages this Pokémon (even if it's Knocked Out), flip a coin; heads, discard an Energy card from the Attacking Pokémon. Not if it was already Asleep, Confused or Paralyzed.</summary>
        private sealed class MukEnergyDrain : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (!CardsBS1.WorkedBeforeAttack(ctx, me)) return;
                Announce(ctx, "Energy Drain");
                if (!ctx.Flip() || attacker.Energy.Count == 0) return;
                var cards = attacker.Energy.ToList();
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Energy Drain: discard which Energy card from " + attacker.Name + "?")[0];
                ctx.Engine.DiscardEnergy(attacker, card);
            }
        }

        /// <summary>Scram: if this Pokémon ever has exactly 10 HP left, shuffle it and its cards into your deck. Not while Asleep, Confused or Paralyzed. Mega Thrash: 20 to itself, discard the Stadium.</summary>
        private sealed class PrimeapeScram : Mon
        {
            public override void OnStateCheck(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !PowerWorks(me) || src.Engine.AbilityBlocked(me, "Scram") || src.Engine.RemainingHp(me) != 10) return;
                Announce(src, "Scram");
                src.Engine.ShufflePokemonIntoDeck(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mega Thrash") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(20);
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Soak Up: once during your turn, move up to 2 Grass Energy cards from your other Pokémon to this Pokémon. Stretch Vine: 10 to 1 Benched Pokémon.</summary>
        private sealed class BellsproutSoakUp : Mon
        {
            private const string Soak = "Soak Up";
            private static readonly Mon Vine = new Cards30CC.BenchHitMon("Stretch Vine", 10);

            private static List<CardInstance> GrassCards(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var list = new List<CardInstance>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    list.AddRange(mon.Energy.Where(c => c.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, mon, c, EnergyType.Grass)));
                return list;
            }

            protected override string Activated { get { return Soak; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && GrassCards(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var cards = GrassCards(ctx);
                if (cards.Count == 0) return;
                var picks = ctx.ChooseCards(cards, 1, Math.Min(2, cards.Count), Soak + ": move up to 2 Grass Energy cards to " + ctx.Pokemon.Name);
                foreach (var card in picks)
                {
                    var from = ctx.MyPokemon(m => m.Energy.Contains(card)).FirstOrDefault();
                    if (from != null) ctx.Engine.MoveEnergy(from, ctx.Pokemon, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Vine.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Relaxing Scent: while this Pokémon is your Active Pokémon, damage an attack (even yours) does to any Pokémon after
        /// Weakness and Resistance is halved, rounded up to the nearest 10. Not while Asleep, Confused or Paralyzed.
        /// </summary>
        private sealed class IvysaurRelaxingScent : Mon
        {
            private static readonly Mon Leaf = new CoinsMon("Double Razor Leaf", 2);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.State.IsActive(me) || !PowerWorks(me) || info.Attacker == null || damage <= 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return (damage / 10 + 1) / 2 * 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Leaf.HandleAttack(ctx, atk); }
        }

        /// <summary>Emerge: once during your turn, flip a coin; heads, search your deck for Koga's Beedrill and evolve this Pokémon. Toxic Secretion: heads, Poisoned (2 counters).</summary>
        private sealed class KakunaEmerge : Mon
        {
            private static readonly Mon Secretion = new ToxicFlipMon("Toxic Secretion");

            protected override string Activated { get { return "Emerge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Me.Deck.Count > 0 && ctx.Engine.ShapeshiftCard(ctx.Pokemon) == null; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                CardsGEN2.EvolveOneFromDeck(ctx, new List<PokemonInPlay> { ctx.Pokemon }, d => d.Name == "Koga's Beedrill", "Emerge");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Secretion.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Shock Blast: if this Pokémon is your Active Pokémon and an attack damages it (even if it's Knocked Out), flip a coin;
        /// tails, 20 damage (2 damage counters) to each Active Pokémon. Works even while Asleep, Confused or Paralyzed.
        /// </summary>
        private sealed class ElectrodeShockBlast : Mon
        {
            private static readonly Mon Ball = new CardsWHT.FlipMoreMon("Power Ball", 3, 10);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage <= 0) return;
                Announce(ctx, "Shock Blast");
                if (ctx.Flip()) return;
                foreach (var mon in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (mon != null && ctx.InPlay(mon)) ctx.Engine.PlaceDamageCounters(mon, 2, "Shock Blast");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ball.HandleAttack(ctx, atk); }
        }

        /// <summary>Gaseous Form: +10 HP for each Psychic Energy card attached (works while Asleep, Confused or Paralyzed).</summary>
        private sealed class GastlyGaseousForm : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (mon == null || mon != src.Pokemon) return hp;
                return hp + 10 * mon.Energy.Count(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Psychic);
            }
        }

        // ================================================================== Attacks

        /// <summary>Roaring Flames: discard all Fire Energy cards attached; 20 more for each Fire Energy discarded.</summary>
        private sealed class RoaringFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roaring Flames") return false;
                var me = ctx.Pokemon;
                int fire = 0;
                foreach (var card in me.Energy.Where(c => IsFireCard(ctx, me, c)).ToList())
                {
                    int units = ctx.Engine.EnergyUnitsOf(me, card).Count(u => EnergyMask.Provides(u, EnergyType.Fire));
                    if (ctx.Engine.DiscardEnergy(me, card)) fire += Math.Max(1, units);
                }
                ctx.DamageDefending(atk.Damage + 20 * fire);
                return true;
            }
        }

        /// <summary>Flame Jet: heads, 40 to 1 of the opponent's Pokémon, no Weakness or Resistance.</summary>
        private sealed class FlameJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Jet") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageAnyOpponent(40, true);
                return true;
            }
        }

        /// <summary>Growth: heads, attach up to 2 Grass Energy cards from your hand to this Pokémon.</summary>
        internal sealed class GrowthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Growth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                var cards = ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Grass).ToList();
                if (cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Growth: attach up to 2 Grass Energy cards from your hand to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, me, true);
                return true;
            }
        }

        /// <summary>Wide Solarbeam: the damage, and 20 to each of 2 of the opponent's Benched Pokémon (or 1).</summary>
        private sealed class WideSolarbeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wide Solarbeam") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count == 0) return true;
                var picks = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Wide Solarbeam: choose 2 of your opponent's Benched Pokémon");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Summon Storm: 2 coins; both heads, 20 to each other Pokémon (yours too), no Weakness or Resistance.</summary>
        private sealed class SummonStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Summon Storm") return false;
                if (ctx.FlipCoins(2) < 2) return Nothing(ctx);
                var me = ctx.Pokemon;
                foreach (var mon in ctx.OpponentPokemon().Concat(ctx.MyPokemon(m => m != me)).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Dragon Tornado: unless it Knocks Out the Defending Pokémon, switch 1 of the opponent's Benched Pokémon (your choice) with it.</summary>
        internal sealed class DragonTornadoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Tornado") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !Still(ctx, defending) || ctx.Engine.IsKnockedOut(defending) || ctx.Opp.Bench.Count == 0) return true;
                ctx.GustOpponent("Dragon Tornado: choose 1 of your opponent's Benched Pokémon to switch with the Defending Pokémon");
                return true;
            }
        }

        /// <summary>Intimidate: a Defending Pokémon with 50 or less maximum HP can't attack this Pokémon during the opponent's next turn.</summary>
        private sealed class IntimidateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Intimidate") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!Still(ctx, defending) || !ctx.InPlay(me) || ctx.Engine.MaxHp(defending) > 50) return true;
                ctx.AddEffect(new IntimidateEffect(defending.Name + " can't attack " + me.Name + " during its next turn (Intimidate)", ctx.Player, defending.Uid, ctx.Turn + 1, me.Uid, me.Top.Id));
                return true;
            }
        }

        private sealed class IntimidateEffect : ContinuousEffect
        {
            private readonly int scarerUid;
            private readonly int scarerTopId;

            public IntimidateEffect(string name, int owner, int targetUid, int expiresAfterTurn, int scarerUid, int scarerTopId)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.scarerUid = scarerUid;
                this.scarerTopId = scarerTopId;
            }

            /// <summary>Only while that Pokémon is the Defending Pokémon and hasn't evolved (benching or evolving either ends it).</summary>
            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                if (mon == null || mon.Uid != TargetUid) return false;
                var scarer = engine.State.Players[Owner].Active;
                return scarer != null && scarer.Uid == scarerUid && scarer.Top.Id == scarerTopId;
            }
        }

        /// <summary>"Flip N coins; all heads, the Defending Pokémon is now [a] and [b]" (Nerve Poison: 1 coin; Fast-Acting Poison: 2).</summary>
        private sealed class BothConditionsFlipMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition first;
            private readonly SpecialCondition second;
            private readonly int coins;

            public BothConditionsFlipMon(string attack, SpecialCondition first, SpecialCondition second, int coins)
            {
                this.attack = attack;
                this.first = first;
                this.second = second;
                this.coins = coins;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(coins) < coins || !Still(ctx, defending)) return true;
                ctx.ApplyCondition(defending, first);
                ctx.ApplyCondition(defending, second);
                return true;
            }
        }

        /// <summary>
        /// "Flip a coin. If tails, this attack does nothing. Either way, you can't use this attack again as long as this Pokémon stays
        /// in play (even putting it on the Bench won't let you use it again)" (Hyper Needle, Ancestral Memory).
        /// </summary>
        private sealed class OncePerStayMon : Mon
        {
            private readonly string attack;

            public OncePerStayMon(string attack) { this.attack = attack; }

            private string Marker { get { return attack + " used"; } }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != attack || ctx.Pokemon == null || !ctx.Pokemon.Markers.ContainsKey(Marker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Pokemon != null) ctx.Pokemon.Markers[Marker] = ctx.Turn;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Koga's Ditto: Giant Growth (heads: 80 HP and Pound does 30, until it's Benched); Pound.</summary>
        private sealed class KogasDitto : Mon
        {
            private static bool Grown(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me != null && ctx.State.Effects.Any(e => e is GiantGrowthEffect && e.TargetUid == me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Giant Growth")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Flip() && ctx.InPlay(me) && !Grown(ctx))
                        ctx.AddEffect(new GiantGrowthEffect(me.Name + " has 80 HP and its Pound does 30 until it's Benched (Giant Growth)", ctx.Player, me.Uid));
                    return true;
                }
                if (atk.Name != "Pound") return false;
                ctx.DamageDefending(Grown(ctx) ? 30 : atk.Damage);
                return true;
            }
        }

        private sealed class GiantGrowthEffect : ContinuousEffect
        {
            public GiantGrowthEffect(string name, int owner, int targetUid) : base(name, owner, targetUid, int.MaxValue - 10, true) { }

            public override int ModifyMaxHp(GameEngine engine, PokemonInPlay mon, int hp) { return mon != null && mon.Uid == TargetUid ? 80 : hp; }
        }

        /// <summary>Kerzap: heads, 30 more damage and discard all Lightning Energy cards attached to this Pokémon.</summary>
        private sealed class KerzapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kerzap") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                var me = ctx.Pokemon;
                if (heads && ctx.InPlay(me))
                    foreach (var card in me.Energy.Where(c => c.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, me, c, EnergyType.Lightning)).ToList())
                        ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Thundertackle: heads, Paralyzed; tails, 20 damage to itself.</summary>
        private sealed class ThundertackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thundertackle") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                else if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                return true;
            }
        }

        /// <summary>Electro Beam: tails, discard all Energy cards attached to this Pokémon. Used for Electro Beam, Dynamic Bolt, Luster Purge (the attack name is passed in).</summary>
        internal sealed class ElectroBeamMon : Mon
        {
            private readonly string attack;

            public ElectroBeamMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Super Removal: heads, discard 1 Energy card (your choice) from each of the opponent's Pokémon that has any.</summary>
        private sealed class SuperRemovalMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Removal") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0))
                {
                    if (!ctx.InPlay(mon) || mon.Energy.Count == 0 || !ctx.CanAffect(mon)) continue;
                    var cards = mon.Energy.ToList();
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Super Removal: discard which Energy card from " + mon.Name + "?")[0];
                    ctx.Engine.DiscardEnergy(mon, card);
                }
                return true;
            }
        }

        /// <summary>Juxtapose: heads, switch the number of damage counters on this Pokémon and the Defending Pokémon (even if it Knocks Out either).</summary>
        private sealed class JuxtaposeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Juxtapose") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (ctx.Flip() && defending != null && ctx.InPlay(ctx.Pokemon)) ctx.Engine.SwapDamageCounters(ctx.Pokemon, defending, "Juxtapose");
                return true;
            }
        }

        /// <summary>Lie Low: damage to this Pokémon during the opponent's next turn is 20 less (after W/R).</summary>
        private sealed class LieLowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lie Low") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(20);
                return true;
            }
        }

        /// <summary>Earthdrill: only if this Pokémon used Lie Low during your last turn.</summary>
        private sealed class EarthdrillMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Earthdrill") return true;
                var me = ctx.Pokemon;
                return me != null && me.LastAttackName == "Lie Low" && me.LastAttackTurn == ctx.MyLastTurn;
            }
        }

        /// <summary>Mega Kick: plain. Love Lariat: heads, 50 more if you have Giovanni's Nidoking on your Bench; tails, nothing.</summary>
        private sealed class LoveLariatMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Love Lariat") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                bool king = ctx.Me.Bench.Any(m => m.Name == "Giovanni's Nidoking");
                ctx.DamageDefending(atk.Damage + (king ? 50 : 0));
                return true;
            }
        }

        /// <summary>Overhead Toss: if you have Benched Pokémon, flip a coin; tails, 20 to 1 of them (your choice, no W/R).</summary>
        private sealed class OverheadTossMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overhead Toss") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Flip()) return true;
                var bench = ctx.Me.Bench.ToList();
                var mon = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Overhead Toss: do 20 damage to which of your Benched Pokémon?");
                if (mon != null) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Poison Power: if this Pokémon is Poisoned, 40 instead of 20 and the Defending Pokémon is Poisoned.</summary>
        private sealed class PoisonPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Power") return false;
                var defending = ctx.Defending;
                bool poisoned = ctx.Pokemon.Has(SpecialCondition.Poisoned);
                ctx.DamageDefending(poisoned ? 40 : atk.Damage);
                if (poisoned && Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Aerial Maneuvers: heads, 30 more damage and prevent all effects of attacks (damage too) on this Pokémon during the opponent's next turn.</summary>
        private sealed class AerialManeuversMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aerial Maneuvers") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (heads) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>High Voltage: heads, the opponent can't play Trainer cards during their next turn.</summary>
        private sealed class HighVoltageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Voltage") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new NoTrainersEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during " + ctx.Engine.Their(ctx.Opponent) + " next turn (High Voltage)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>The player can't play Trainer cards while this lasts (High Voltage, Rocket's Secret Experiment's tails).</summary>
        private sealed class NoTrainersEffect : ContinuousEffect
        {
            private readonly int affected;

            public NoTrainersEffect(string name, int owner, int affected, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.affected = affected;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected; }
        }

        /// <summary>Thunder Flare: 10 more for each damage counter on this Pokémon, then flip a coin; tails, 30 to itself.</summary>
        private sealed class ThunderFlareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Flare") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Dark Wave: all Pokémon Powers stop working until the end of the opponent's next turn.</summary>
        private sealed class DarkWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Wave") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new DarkWaveEffect("All Pokémon Powers stop working until the end of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s next turn (Dark Wave)", ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class DarkWaveEffect : ContinuousEffect
        {
            public DarkWaveEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool StopsPokemonPower(GameEngine engine, PokemonInPlay mon, string powerName) { return true; }
        }

        /// <summary>"The Defending Pokémon can't retreat during your opponent's next turn" (Shadow Bind, Frostbite).</summary>
        private sealed class NoRetreatMon : Mon
        {
            private readonly string attack;

            public NoRetreatMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (Still(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Hug: heads, the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class FlipNoRetreatMon : Mon
        {
            private readonly string attack;

            public FlipNoRetreatMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && Still(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Damage Shift: move 1 damage counter from each of your Pokémon that has any to the Defending Pokémon (no W/R).</summary>
        private sealed class DamageShiftMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Shift") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0))
                {
                    var defending = ctx.Defending;
                    if (defending == null) break;
                    ctx.Engine.MoveDamageCounters(mon, defending, 1, "Damage Shift");
                }
                return true;
            }
        }

        /// <summary>Bonfire: 3 coins; discard a Fire Energy card for each heads (none to discard: nothing); 10 for each heads to each of the opponent's Pokémon, no W/R.</summary>
        private sealed class BonfireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bonfire") return false;
                var me = ctx.Pokemon;
                int heads = ctx.FlipCoins(3);
                if (heads == 0) return Nothing(ctx);
                var fire = me.Energy.Where(c => IsFireCard(ctx, me, c)).ToList();
                if (fire.Count == 0 && !CardsBS1.CopyingWithMetronome) return Nothing(ctx);
                if (!CardsBS1.CopyingWithMetronome)
                {
                    int n = Math.Min(heads, fire.Count);
                    var picks = n == fire.Count ? fire : ctx.ChooseCards(fire, n, n, "Bonfire: discard " + n + " Fire Energy card" + (n == 1 ? "" : "s"));
                    foreach (var card in picks) ctx.Engine.DiscardEnergy(me, card);
                }
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 10 * heads, false, false, false);
                return true;
            }
        }

        /// <summary>Stamp: heads, 10 more damage and 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class StampMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stamp") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (heads)
                    foreach (var mon in ctx.Opp.Bench.ToList())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Detonate: 10 to each Benched Pokémon (both players'), 50 to itself, and the Stadium in play is discarded.</summary>
        private sealed class DetonateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Detonate") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.Concat(ctx.Me.Bench).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(50);
                if (ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, search your deck for [a card] and put it into your hand" (Errand-Running).</summary>
        private sealed class FlipSearchMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public FlipSearchMon(string attack, Func<CardDef, bool> filter, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(filter, 1, attack + ": choose " + what);
                return true;
            }
        }

        /// <summary>Lunar Power: heads, search your deck for a card that evolves from 1 of your Benched Pokémon and evolve it. Moon Kick: plain.</summary>
        private sealed class LunarPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lunar Power") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Bench.Count == 0) return true;
                CardsGEN2.EvolveOneFromDeck(ctx, ctx.Me.Bench.ToList(), null, "Lunar Power");
                return true;
            }
        }

        /// <summary>Headlock: heads, 20 more damage and Paralyzed.</summary>
        private sealed class HeadlockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Headlock") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>False Charity: heads, look at the top card of the opponent's deck: a Trainer card is discarded, any other card goes into their hand.</summary>
        private sealed class FalseCharityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "False Charity") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Opp.Deck.Count == 0) return true;
                var top = ctx.Opp.Deck[0];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { top }, "Top card of your opponent's deck", "False Charity: the top card of your opponent's deck", ctx.Source);
                if (top.Def.IsTrainer) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                else
                {
                    ctx.Opp.Deck.RemoveAt(0);
                    ctx.Opp.Hand.Add(top);
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put the top card of " + ctx.Engine.Their(ctx.Opponent) + " deck into " + ctx.Engine.Their(ctx.Opponent) + " hand (False Charity).");
                }
                return true;
            }
        }

        /// <summary>Poison Sting Tackle: 20 to itself, then heads, Poisoned.</summary>
        private sealed class PoisonStingTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Sting Tackle") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                if (ctx.Flip() && Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Rend: 20 more if the Defending Pokémon has any damage counters.</summary>
        private sealed class RendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rend") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.DamageCounters > 0 ? 20 : 0));
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, the Defending Pokémon is now Poisoned; it takes 20 Poison damage instead of 10" (Toxic Secretion, Toxic Cloud).</summary>
        private sealed class ToxicFlipMon : Mon
        {
            private readonly string attack;

            public ToxicFlipMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !Still(ctx, defending)) return true;
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 2;
                return true;
            }
        }

        /// <summary>Obscuring Gas: heads, after the damage, shuffle this Pokémon and its cards into your deck.</summary>
        private sealed class ObscuringGasMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Obscuring Gas") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>
        /// Messenger: put this Pokémon and its cards on top of your deck, then search your deck for a Basic Pokémon or Evolution card
        /// not named Koga's Pidgey, show it and put it into your hand; shuffle. Wing Attack: plain.
        /// </summary>
        private sealed class MessengerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Messenger") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                {
                    ctx.Engine.RemoveFromPlay(me);
                    ctx.Me.Deck.InsertRange(0, me.AllCards());
                    ctx.Log(me.Name + " and its cards were put on top of " + ctx.Engine.Their(ctx.Player) + " deck (Messenger).");
                }
                ctx.SearchToHand(d => (d.IsBasicPokemon || d.IsEvolution) && d.Name != "Koga's Pidgey", 1, "Messenger: choose a Basic Pokémon or Evolution card not named Koga's Pidgey");
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, this attack does N more damage and this Pokémon does R damage to itself" (Spontaneous Explosion).</summary>
        private sealed class FlipBonusRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly int recoil;

            public FlipBonusRecoilMon(string attack, int bonus, int recoil)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? bonus : 0));
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(recoil);
                return true;
            }
        }

        /// <summary>Surprise: a random card from the opponent's hand is shuffled into their deck.</summary>
        private sealed class SurpriseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { card }, "A random card from your opponent's hand", "Surprise: your opponent shuffles this card into their deck", ctx.Source);
                ctx.Opp.Hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled a random card from " + ctx.Engine.Their(ctx.Opponent) + " hand into " + ctx.Engine.Their(ctx.Opponent) + " deck (Surprise).");
                return true;
            }
        }

        /// <summary>
        /// Focus Energy: during your next turn, the named attack's damage is doubled (Raticate's Double-edge: its base damage and the
        /// damage to itself; Rattata's Quick Attack: its base damage).
        /// </summary>
        private sealed class FocusEnergyMon : Mon
        {
            private const string Focus = "Focus Energy turn";
            private readonly string attack;
            private readonly int recoil;

            public FocusEnergyMon(string attack, int recoil)
            {
                this.attack = attack;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Focus] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != attack) return false;
                int turn;
                int x = me != null && me.Markers.TryGetValue(Focus, out turn) && turn == ctx.Turn ? 2 : 1;
                int bonus = recoil == 0 && ctx.Flip() ? 20 : 0;
                ctx.DamageDefending(atk.Damage * x + bonus);
                if (recoil > 0 && ctx.InPlay(me)) ctx.DamageSelf(recoil * x);
                return true;
            }
        }

        /// <summary>Ice Throw: base damage doubled against a Fighting Pokémon.</summary>
        private sealed class IceThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Throw") return false;
                var defending = ctx.Defending;
                bool fighting = defending != null && ctx.Engine.IsType(defending, EnergyType.Fighting);
                ctx.DamageDefending(fighting ? atk.Damage * 2 : atk.Damage);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, choose 1 of your opponent's Benched Pokémon; N damage to it" (Shadow Attack, Overrun).</summary>
        private sealed class FlipBenchHitMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FlipBenchHitMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count == 0) return true;
                if (ctx.Flip()) ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>Invigorate: put a Basic Pokémon from either player's discard pile onto its owner's Bench with damage counters equal to half its HP (rounded down to 10).</summary>
        private sealed class InvigorateMon : Mon
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                var list = new List<CardInstance>();
                foreach (var p in ctx.State.Players)
                {
                    if (!ctx.Engine.BenchHasRoom(p.Index)) continue;
                    list.AddRange(p.Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(p.Index, c.Def)));
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Invigorate") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var candidates = Candidates(ctx);
                if (candidates.Count == 0) return true;
                var card = ctx.ChooseCards(candidates, 1, 1, "Invigorate: put which Basic Pokémon from a discard pile onto its owner's Bench?")[0];
                var owner = ctx.State.Players[card.Owner];
                if (!owner.Discard.Remove(card)) return true;
                var mon = ctx.Engine.PutOntoBench(card.Owner, card, false);
                int counters = card.Def.Hp / 2 / 10;
                if (mon != null && counters > 0) ctx.Engine.PlaceDamageCounters(mon, counters, "Invigorate");
                return true;
            }
        }

        /// <summary>Pendulum Curse: a coin for each damage counter on the Defending Pokémon; 20 for each heads.</summary>
        private sealed class PendulumCurseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pendulum Curse") return false;
                var defending = ctx.Defending;
                int n = defending == null ? 0 : defending.DamageCounters;
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Helping Hand: remove any number of damage counters from 1 of the opponent's Pokémon, then draw that many cards.</summary>
        private sealed class HelpingHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Helping Hand") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var mon = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Helping Hand: remove damage counters from which of your opponent's Pokémon?");
                if (mon == null) return true;
                int n = ctx.AskNumber(0, mon.DamageCounters, "Helping Hand: remove how many damage counters from " + mon.Name + "? (Draw that many cards.)");
                if (n <= 0) return true;
                int healed = ctx.Heal(mon, 10 * n) / 10;
                if (healed > 0) ctx.Draw(healed);
                return true;
            }
        }

        /// <summary>Life Drain: heads, damage counters on the Defending Pokémon until it has 10 HP left.</summary>
        private sealed class LifeDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Life Drain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (!ctx.Flip() || defending == null) return true;
                int counters = (ctx.Engine.RemainingHp(defending) - 10) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Magic Darts: choose 1 of the opponent's Pokémon and flip 3 coins; 10 for each heads to it, no W/R.</summary>
        private sealed class MagicDartsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magic Darts") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = all.Count == 1 ? all[0] : ctx.ChoosePokemon(all, "Magic Darts: choose 1 of your opponent's Pokémon");
                int heads = ctx.FlipCoins(3);
                if (heads == 0) return Nothing(ctx);
                if (target != null && ctx.InPlay(target)) ctx.DamageWith(target, 10 * heads, false, false, false);
                return true;
            }
        }

        /// <summary>Fire Tail Slap: only with a Fire Energy card attached; tails, discard 1 of them after the damage.</summary>
        private sealed class FireTailSlapMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                return ctx.Attack(attackIndex).Name != "Fire Tail Slap" || (me != null && me.Energy.Any(c => IsFireCard(ctx, me, c)));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Tail Slap") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads || !ctx.InPlay(me)) return true;
                var fire = me.Energy.Where(c => IsFireCard(ctx, me, c)).ToList();
                if (fire.Count == 0) return true;
                var card = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Fire Tail Slap: discard which Fire Energy card?")[0];
                ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>Retaliate: 10 for each damage counter on this Pokémon.</summary>
        private sealed class RetaliateMon : Mon
        {
            private readonly string attack;

            public RetaliateMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int n = ctx.Pokemon.DamageCounters;
                if (n == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Stoke: search your deck for a Fire Energy card and attach it to this Pokémon.</summary>
        private sealed class StokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsFireEnergyCard, 1, "Stoke: choose a Fire Energy card to attach to " + me.Name);
                if (found.Count > 0 && ctx.InPlay(me)) ctx.Engine.AttachEnergyCard(found[0], me);
                else ctx.Me.Deck.AddRange(found);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Pranks: heads, put a card from the opponent's discard pile on top of their deck.</summary>
        private sealed class PranksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pranks") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Opp.Discard.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Opp.Discard.ToList(), 1, 1, "Pranks: put which card from your opponent's discard pile on top of their deck?")[0];
                if (!ctx.Opp.Discard.Remove(card)) return true;
                ctx.Opp.Deck.Insert(0, card);
                ctx.Log(card.Def.Name + " was put on top of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck (Pranks).");
                return true;
            }
        }

        /// <summary>Hind Kick: if you have Benched Pokémon, flip a coin; heads, switch this Pokémon with 1 of them.</summary>
        private sealed class HindKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hind Kick") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon) && ctx.Flip()) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Bite: plain. Call Will-o'-the-wisp: 3 coins; a Fire Energy card from your discard pile into your hand for each heads.</summary>
        private sealed class CallWillOTheWispMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call Will-o'-the-wisp") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                var fire = ctx.Me.Discard.Where(c => IsFireEnergyCard(c.Def)).ToList();
                int n = Math.Min(heads, fire.Count);
                if (n == 0) return true;
                var picks = n == fire.Count ? fire : ctx.ChooseCards(fire, n, n, "Call Will-o'-the-wisp: put " + n + " Fire Energy card" + (n == 1 ? "" : "s") + " into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Tremor: 10 to each of your own Benched Pokémon.</summary>
        private sealed class TremorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tremor") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Me.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Strange Powder: heads, Confused; tails, Asleep.</summary>
        internal sealed class StrangePowderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strange Powder") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (Still(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Confused : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Poison Spore: heads, Poisoned and 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class PoisonSporeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Spore") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                if (Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Fury Punch: heads, 20 for each damage counter on this Pokémon.</summary>
        private sealed class FuryPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Punch") return false;
                int n = ctx.Pokemon.DamageCounters;
                if (!ctx.Flip() || n == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Retaliation: only with 2 or more damage counters on this Pokémon.</summary>
        private sealed class RetaliationMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Retaliation" || (ctx.Pokemon != null && ctx.Pokemon.DamageCounters >= 2);
            }
        }

        /// <summary>Sludge Grip: if the opponent has Benched Pokémon, flip a coin; heads, switch 1 of them (your choice) with the Defending Pokémon, and the new Defending Pokémon is Poisoned.</summary>
        private sealed class SludgeGripMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sludge Grip") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count == 0 || !ctx.Flip()) return true;
                var pulled = ctx.GustOpponent("Sludge Grip: choose 1 of your opponent's Benched Pokémon to switch with the Defending Pokémon");
                if (pulled != null && Still(ctx, pulled)) ctx.ApplyCondition(pulled, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Smelly Gas: heads, 10 to each Benched Pokémon (yours too).</summary>
        private sealed class SmellyGasMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smelly Gas") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                    foreach (var mon in ctx.Opp.Bench.Concat(ctx.Me.Bench).ToList())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Grasping Vine: heads, draw 2 cards. Used for Grasping Vine, Quick Draw (the attack name and the numbers are passed in).</summary>
        internal sealed class GraspingVineMon : Mon
        {
            private readonly string attack;
            private readonly int cards;

            public GraspingVineMon(string attack, int cards)
            {
                this.attack = attack;
                this.cards = cards;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) ctx.Draw(cards);
                return true;
            }
        }

        /// <summary>Sharp Stinger: heads, Poisoned; tails, Paralyzed.</summary>
        private sealed class SharpStingerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sharp Stinger") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (Still(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Poisoned : SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Group Attack: first you may put any number of Koga's Zubat from your deck onto your Bench; then 10 for each Koga's Zubat you have in play.</summary>
        private sealed class GroupAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Group Attack") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Any(c => c.Def.Name == "Koga's Zubat") && ctx.AskYesNo("Group Attack: put Koga's Zubat from your deck onto your Bench first?"))
                    ctx.SearchToBench(d => d.Name == "Koga's Zubat", ctx.BenchSpace, "Group Attack: choose any number of Koga's Zubat");
                int n = ctx.MyPokemon(m => m.Name == "Koga's Zubat").Count;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Ink Spurt: heads, the Defending Pokémon must flip a coin to attack (tails: no attack) until it's Benched or evolves.</summary>
        private sealed class InkSpurtMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ink Spurt") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && Still(ctx, defending))
                    ctx.AddEffect(new CardsMEG.SandAttackEffect(defending.Name + " must flip a coin to attack until it's Benched or evolves (Ink Spurt)", ctx.Player, defending.Uid, int.MaxValue - 10));
                return true;
            }
        }

        /// <summary>Bubbles: tails, you can't use this attack during your next turn.</summary>
        private sealed class BubblesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubbles") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads) ctx.SelfCantUseAttackNextTurn("Bubbles");
                return true;
            }
        }

        /// <summary>Amnesia: choose 1 of the Defending Pokémon's attacks; it can't use it during the opponent's next turn.</summary>
        private sealed class AmnesiaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Amnesia") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!Still(ctx, defending)) return true;
                var names = ctx.Engine.AttacksOf(defending).Select(a => a.Attack.Name).Distinct().ToArray();
                if (names.Length == 0) return true;
                string name = names.Length == 1 ? names[0] : names[ctx.ChooseOption("Amnesia: which attack can't " + defending.Name + " use during its next turn?", names)];
                ctx.AddEffect(new CantUseAttackEffect(defending.Name + " can't use " + name + " during its next turn (Amnesia)", ctx.Player, defending.Uid, ctx.Turn + 1, name));
                return true;
            }
        }

        /// <summary>ESP: 3 coins; exactly 1 heads, draw a card; 2 heads, 20 damage; 3 heads, copy 1 of the Defending Pokémon's attacks (not its Energy cost).</summary>
        private sealed class EspMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "ESP") return false;
                int heads = ctx.FlipCoins(3);
                if (heads == 1) ctx.Draw(1);
                else if (heads == 2) ctx.DamageDefending(20);
                else if (heads == 3)
                {
                    CardDef source;
                    int index;
                    if (ChooseCopiedAttack(ctx, ctx.Defending, "ESP: copy which of the Defending Pokémon's attacks?", out source, out index))
                        UseCopiedAttack(ctx, source, index);
                }
                else return Nothing(ctx);
                return true;
            }
        }

        /// <summary>Star Boomerang: heads, after the damage, return this Pokémon and its cards to your hand.</summary>
        private sealed class StarBoomerangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Boomerang") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Psyscan: look at your opponent's hand.</summary>
        private sealed class PsyscanMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyscan") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Your opponent's hand", "Psyscan: your opponent's hand", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + ctx.Engine.PlayerName(ctx.Opponent) + "'s hand (Psyscan).");
                return true;
            }
        }

        /// <summary>Fade Out: return this Pokémon and its Energy cards to your hand; its other attached cards are discarded.</summary>
        private sealed class FadeOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fade Out") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.Engine.DiscardAllTools(me);
                ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Barrier Attack: damage to this Pokémon during the opponent's next turn is 10 less (after W/R).</summary>
        private sealed class BarrierAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Barrier Attack") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfTakesLessNextTurn(10);
                return true;
            }
        }

        /// <summary>Random ESP: heads, the damage and the Defending Pokémon is Confused; tails, no damage and this Pokémon is Confused.</summary>
        private sealed class RandomEspMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Random ESP") return false;
                var defending = ctx.Defending;
                if (ctx.Flip())
                {
                    ctx.DamageDefending(atk.Damage);
                    if (Still(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>
        /// Blaine (your ruling): for the rest of the turn, when your Energy attachment for the turn is a Fire Energy card on a Pokémon
        /// with Blaine in its name, you may attach a second Fire Energy card from your hand to it. More Blaine cards add nothing.
        /// </summary>
        private sealed class BlaineTrainer : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.State.Effects.Any(e => e is BlaineEffect && e.Owner == ctx.Player && e.ExpiresAfterTurn == ctx.Turn))
                {
                    ctx.Log("Blaine is already in force this turn.");
                    return;
                }
                ctx.AddEffect(new BlaineEffect("Blaine: this turn you may attach 2 Fire Energy cards to 1 of your Blaine's Pokémon", ctx.Player, ctx.Turn));
            }
        }

        private sealed class BlaineEffect : ContinuousEffect
        {
            public BlaineEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override Func<CardDef, bool> SecondEnergyAttachment(GameEngine engine, int player, PokemonInPlay target, CardInstance first)
            {
                if (player != Owner || target == null || !Owned(target, "Blaine") || first == null || !IsFireEnergyCard(first.Def)) return null;
                return IsFireEnergyCard;
            }
        }

        /// <summary>Brock's Protection: on 1 of your Brock's Pokémon, the opponent's attacks and Trainer cards can't remove its Energy.</summary>
        private sealed class BrocksProtection : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Brock")); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Brock's Protection: attach it to which of your Pokémon?");
                if (mon != null) ctx.AddEffect(new BrocksProtectionEffect("Brock's Protection: the opponent's attacks and Trainer cards can't remove " + mon.Name + "'s Energy", ctx.Player, mon.Uid));
            }
        }

        private sealed class BrocksProtectionEffect : ContinuousEffect
        {
            public BrocksProtectionEffect(string name, int owner, int targetUid) : base(name, owner, targetUid, int.MaxValue - 10, false) { }

            public override bool ProtectsEnergy(GameEngine engine, PokemonInPlay holder, CardInstance energy) { return holder != null && holder.Uid == TargetUid; }
        }

        /// <summary>Chaos Gym: every Trainer card but a Stadium is flipped for (GameEngine.ChaosGymStops).</summary>
        private sealed class ChaosGym : CardScript
        {
            public override bool FlipsForTrainers(EffectContext src, int player, CardInstance card) { return !card.Def.IsStadium; }
        }

        /// <summary>Erika's Kindness: remove 2 damage counters (or 1) from each Pokémon in play that has any, both players'.</summary>
        private sealed class ErikasKindness : CardScript
        {
            private static List<PokemonInPlay> Hurt(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Concat(ctx.OpponentPokemon(m => m.DamageCounters > 0)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Hurt(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in Hurt(ctx)) ctx.Heal(mon, 10 * Math.Min(2, mon.DamageCounters));
            }
        }

        /// <summary>Giovanni: 1 of your Giovanni's Pokémon may evolve this turn even if it just came into play or evolved, or on your first turn.</summary>
        private sealed class GiovanniTrainer : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Giovanni")); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override bool PlayableOnFirstTurn(EffectContext ctx) { return true; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Giovanni: which of your Pokémon may evolve this turn?");
                if (mon != null) ctx.AddEffect(new GiovanniEffect("Giovanni: " + mon.Name + " may evolve this turn", ctx.Player, mon.Uid, ctx.Turn));
            }
        }

        private sealed class GiovanniEffect : ContinuousEffect
        {
            public GiovanniEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool AllowsSameTurnEvolution(GameEngine engine, PokemonInPlay target, CardDef evolution) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>Giovanni's Last Resort: remove all damage counters from 1 of your Giovanni's Pokémon, then discard your hand.</summary>
        private sealed class GiovannisLastResort : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Giovanni")); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Giovanni's Last Resort: remove all damage counters from which Pokémon?");
                if (mon != null && mon.DamageCounters > 0) ctx.Heal(mon, 10 * mon.DamageCounters);
                ctx.Engine.DiscardHand(ctx.Player);
            }
        }

        /// <summary>Koga: this turn, your Koga's Pokémon's attacks that damage the Defending Pokémon also Poison it (after the damage).</summary>
        private sealed class KogaTrainer : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new KogaEffect("Koga: this turn, attacks by your Koga's Pokémon that damage the Defending Pokémon Poison it", ctx.Player, ctx.Turn));
            }
        }

        private sealed class KogaEffect : ContinuousEffect
        {
            public KogaEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (damage <= 0 || info.Attacker == null || info.Attacker.Owner != Owner || target == null || target.Owner == Owner) return;
                if (!info.TargetIsActive || !Owned(info.Attacker, "Koga")) return;
                if (engine.State.FindPokemon(target.Uid) != target || !engine.State.IsActive(target)) return;
                engine.ApplyCondition(target, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Lt. Surge's Secret Plan (your ruling: the printed text): put 1 card from your hand face down onto your Bench (GameEngine.PutFaceDownOntoBench).</summary>
        private sealed class LtSurgesSecretPlan : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && OtherCards(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = OtherCards(ctx);
                if (others.Count == 0 || ctx.BenchSpace <= 0) return;
                var card = ctx.ChooseCards(others, 1, 1, "Lt. Surge's Secret Plan: put which card face down onto your Bench?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.PutFaceDownOntoBench(ctx.Player, card);
            }
        }

        /// <summary>
        /// Misty's Wish: look at 1 of your Prize cards and ask your opponent if you may switch it with a card from your hand. Yes
        /// (with other cards in your hand): switch them. No: the Prize card goes back face down and you draw a card.
        /// </summary>
        private sealed class MistysWish : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Prizes.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                if (me.Prizes.Count == 0) return;
                var slots = me.PrizeSlotList();
                var order = Enumerable.Range(0, me.Prizes.Count).OrderBy(i => slots[i]).ToList();
                int at = order[0];
                if (order.Count > 1)
                {
                    var opts = order.Select(i => new DecisionOption { Label = "Prize card " + (slots[i] + 1), Index = i }).ToList();
                    int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Misty's Wish: look at which Prize card?", opts, ctx.Source, false);
                    at = opts[pick].Index;
                }
                var prize = me.Prizes[at];
                ctx.Engine.ShowCards(ctx.Player, new List<CardInstance> { prize }, "Your Prize card", "Misty's Wish: your Prize card", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at a Prize card (Misty's Wish).");
                bool yes = ctx.Engine.AskYesNo(ctx.Opponent, "Misty's Wish: may " + ctx.Engine.PlayerName(ctx.Player) + " switch a Prize card with a card from their hand?", ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + (yes ? " said yes." : " said no."));
                if (!yes)
                {
                    ctx.Draw(1);
                    return;
                }
                if (me.Hand.Count == 0) return;
                var card = ctx.ChooseCards(me.Hand.ToList(), 1, 1, "Misty's Wish: switch the Prize card with which card from your hand?")[0];
                if (!me.Hand.Remove(card)) return;
                int slot;
                bool hadSlot = me.PrizeSlotByCard.TryGetValue(prize.Id, out slot);
                me.Prizes[at] = card;
                me.FaceUpPrizeIds.Remove(prize.Id);
                me.PrizeSlotByCard.Remove(prize.Id);
                if (hadSlot) me.PrizeSlotByCard[card.Id] = slot;
                me.Hand.Add(prize);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched a Prize card with a card from " + ctx.Engine.Their(ctx.Player) + " hand (Misty's Wish).");
            }
        }

        /// <summary>Resistance Gym: each Pokémon's Resistance is 20 less.</summary>
        private sealed class ResistanceGym : CardScript
        {
            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount) { return amount - 20; }
        }

        /// <summary>Sabrina: move all Energy cards from 1 of your Sabrina's Pokémon to another of your Sabrina's Pokémon.</summary>
        private sealed class SabrinaTrainer : CardScript
        {
            private static List<PokemonInPlay> Sabrinas(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Sabrina")); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var all = Sabrinas(ctx);
                return all.Count >= 2 && all.Any(m => m.Energy.Count > 0);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var all = Sabrinas(ctx);
                var givers = all.Where(m => m.Energy.Count > 0).ToList();
                if (all.Count < 2 || givers.Count == 0) return;
                var from = givers.Count == 1 ? givers[0] : ctx.ChoosePokemon(givers, "Sabrina: move all Energy cards from which Pokémon?");
                var takers = all.Where(m => m != from).ToList();
                var to = takers.Count == 1 ? takers[0] : ctx.ChoosePokemon(takers, "Sabrina: move them to which Pokémon?");
                if (from == null || to == null) return;
                foreach (var card in from.Energy.ToList()) ctx.Engine.MoveEnergy(from, to, card);
            }
        }

        /// <summary>Blaine's Quiz #2: a face-down card from your hand; the opponent guesses Energy, Trainer or Pokémon. Right: they draw 2; wrong: you draw 2.</summary>
        private sealed class BlainesQuiz2 : CardScript
        {
            private static readonly string[] Kinds = { "Energy card", "Trainer card", "Pokémon card" };

            private static int KindOf(CardDef def) { return def.IsEnergy ? 0 : def.IsTrainer ? 1 : 2; }

            public override bool CanPlayTrainer(EffectContext ctx) { return OtherCards(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = OtherCards(ctx);
                if (others.Count == 0) return;
                var hidden = ctx.ChooseCards(others, 1, 1, "Blaine's Quiz #2: put which card face down? (Your opponent guesses its kind.)")[0];
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a card face down (Blaine's Quiz #2).");
                int guess;
                if (!ctx.Engine.IsInteractive(ctx.Opponent)) guess = ctx.State.Rng.Next(Kinds.Length);
                else
                {
                    var opts = Kinds.Select((k, i) => new DecisionOption { Label = k, Index = i }).ToList();
                    guess = opts[ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Blaine's Quiz #2: is the face-down card an Energy, Trainer or Pokémon card?", opts, ctx.Source, false)].Index;
                }
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { hidden }, "revealed the face-down card (Blaine's Quiz #2)");
                bool right = guess == KindOf(hidden.Def);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + Kinds[guess] + ": " + (right ? "right." : "wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 2);
                else ctx.Draw(2);
            }
        }

        /// <summary>
        /// Blaine's Quiz #3 (your ruling): put a Basic Pokémon or Evolution card from your hand face down and name 1 of its attacks;
        /// the opponent guesses its name from every Pokémon name (a bot guesses at random among the Pokémon with that attack).
        /// Right: they draw 3; wrong: you draw 3. The card stays in your hand.
        /// </summary>
        private sealed class BlainesQuiz3 : CardScript
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && (c.Def.IsBasicPokemon || c.Def.IsEvolution) && c.Def.Attacks.Count > 0).ToList();
            }

            /// <summary>Every Pokémon card the game knows: the card database when it's loaded, otherwise the cards in this game.</summary>
            private static List<CardDef> AllPokemon(EffectContext ctx)
            {
                if (CustomFormats.Database != null) return CustomFormats.Database.Cards.Where(d => d.IsPokemon).ToList();
                var list = new List<CardDef>();
                foreach (var p in ctx.State.Players)
                {
                    list.AddRange(p.Deck.Select(c => c.Def));
                    list.AddRange(p.Hand.Select(c => c.Def));
                    list.AddRange(p.Discard.Select(c => c.Def));
                    list.AddRange(p.Prizes.Select(c => c.Def));
                    foreach (var mon in p.InPlay()) list.AddRange(mon.Stack.Select(c => c.Def));
                }
                return list.Where(d => d.IsPokemon).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Candidates(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var candidates = Candidates(ctx);
                if (candidates.Count == 0) return;
                var hidden = candidates.Count == 1 ? candidates[0] : ctx.ChooseCards(candidates, 1, 1, "Blaine's Quiz #3: put which Pokémon face down?")[0];
                var attacks = hidden.Def.Attacks.Select(a => a.Name).Distinct().ToArray();
                string attack = attacks.Length == 1 ? attacks[0] : attacks[ctx.ChooseOption("Blaine's Quiz #3: tell your opponent the name of which attack?", attacks)];
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put a Pokémon face down: it has the attack " + attack + " (Blaine's Quiz #3).");
                var pool = AllPokemon(ctx);
                string guessed;
                if (!ctx.Engine.IsInteractive(ctx.Opponent))
                {
                    var likely = pool.Where(d => d.Attacks.Any(a => a.Name == attack)).Select(d => d.Name).Distinct().ToList();
                    if (!likely.Contains(hidden.Def.Name)) likely.Add(hidden.Def.Name);
                    guessed = likely[ctx.State.Rng.Next(likely.Count)];
                }
                else
                {
                    var names = pool.Select(d => d.Name).Distinct().OrderBy(n => n).ToList();
                    if (!names.Contains(hidden.Def.Name)) names.Add(hidden.Def.Name);
                    var opts = names.Select((label, i) => new DecisionOption { Label = label, Index = i }).ToList();
                    guessed = names[opts[ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Blaine's Quiz #3: guess the face-down Pokémon's name (it has the attack " + attack + ")", opts, ctx.Source, false)].Index];
                }
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { hidden }, "revealed the face-down Pokémon (Blaine's Quiz #3)");
                bool right = guessed == hidden.Def.Name;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + guessed + ": " + (right ? "right." : "wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 3);
                else ctx.Draw(3);
            }
        }

        /// <summary>Cinnabar City Gym: a Water Pokémon's attack damage to a Blaine's Pokémon ignores Weakness.</summary>
        private sealed class CinnabarCityGym : CardScript
        {
            public override bool IgnoresWeakness(EffectContext src, DamageInfo info)
            {
                return info.Attacker != null && info.Target != null && Owned(info.Target, "Blaine") && src.Engine.IsType(info.Attacker, EnergyType.Water);
            }
        }

        /// <summary>Fuchsia City Gym: once during each player's turn, flip a coin; heads, they may shuffle 1 of their Koga's Pokémon and its cards into their deck.</summary>
        private sealed class FuchsiaCityGym : CardScript
        {
            private const string Limit = "Fuchsia City Gym";

            private static List<PokemonInPlay> Kogas(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Koga")); }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && Kogas(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                if (!ctx.Flip()) return false;
                var kogas = Kogas(ctx);
                if (kogas.Count == 0 || !ctx.AskYesNo("Fuchsia City Gym: shuffle 1 of your Koga's Pokémon and its cards into your deck?")) return false;
                var mon = kogas.Count == 1 ? kogas[0] : ctx.ChoosePokemon(kogas, "Fuchsia City Gym: shuffle which Pokémon into your deck?");
                if (mon == null) return false;
                ctx.Engine.ShufflePokemonIntoDeck(mon);
                if (ctx.Me.Active == null && ctx.Me.Bench.Count > 0) ctx.Engine.PromoteActive(ctx.Player);
                else if (!ctx.Me.HasPokemonInPlay) ctx.Engine.CheckWinConditions();
                return false;
            }
        }

        /// <summary>
        /// Koga's Ninja Trick: on your Active Koga's Pokémon; when the opponent attacks, you may switch it with 1 of your Benched
        /// Pokémon before the attack's damage and effects. It's discarded once that Pokémon moves to the Bench.
        /// </summary>
        private sealed class KogasNinjaTrick : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && Owned(ctx.Me.Active, "Koga"); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                ctx.AddEffect(new NinjaTrickEffect("Koga's Ninja Trick on " + active.Name, ctx.Player, active.Uid, active.MovedToActiveOnTurn));
            }
        }

        private sealed class NinjaTrickEffect : ContinuousEffect
        {
            private readonly int activeSince;

            public NinjaTrickEffect(string name, int owner, int targetUid, int activeSince) : base(name, owner, targetUid, int.MaxValue - 10, false)
            {
                this.activeSince = activeSince;
            }

            public override void OnAttackStarting(GameEngine engine, PokemonInPlay attacker, AttackDef attack)
            {
                var me = engine.State.FindPokemon(TargetUid);
                // Moved to the Bench since it was attached (even if it came back): the card was discarded then.
                if (me == null || me.MovedToActiveOnTurn != activeSince || !engine.State.IsActive(me))
                {
                    engine.State.Effects.Remove(this);
                    return;
                }
                if (attacker == null || attacker.Owner == Owner) return;
                var bench = engine.State.Players[Owner].Bench.ToList();
                if (bench.Count == 0 || !engine.AskYesNo(Owner, "Koga's Ninja Trick: switch " + me.Name + " with 1 of your Benched Pokémon before the attack?", null)) return;
                var to = engine.ChoosePokemon(Owner, bench, "Koga's Ninja Trick: switch in which Pokémon?", null);
                if (to == null || !engine.SwitchActive(Owner, to)) return;
                engine.State.Effects.Remove(this);
                engine.Emit(GameEventType.Info, Owner, "Koga's Ninja Trick was discarded.", -1, me.Uid);
            }
        }

        /// <summary>Master Ball: look at the top 7 cards of your deck; you may put a Basic Pokémon or Evolution card from among them into your hand. Shuffle.</summary>
        private sealed class MasterBallG2 : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var pokemon = top.Where(c => c.Def.IsBasicPokemon || c.Def.IsEvolution).ToList();
                var picks = ctx.ChooseCards(pokemon, 0, 1, "Master Ball: put a Basic Pokémon or Evolution card into your hand?");
                if (picks.Count > 0)
                {
                    ctx.Engine.Reveal(ctx.Player, picks, "revealed (Master Ball)");
                    ctx.DeckToHand(picks);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Max Revive: put a Pokémon card from your discard pile on top of your deck.</summary>
        private sealed class MaxReviveG2 : CardScript
        {
            private static List<CardInstance> Pokemon(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pokemon(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cards = Pokemon(ctx);
                if (cards.Count == 0) return;
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Max Revive: put which Pokémon on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Me.Deck.Insert(0, card);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "put on top of the deck (Max Revive)");
            }
        }

        /// <summary>Misty's Tears: discard 1 other card from your hand to search your deck for up to 2 Water Energy cards.</summary>
        private sealed class MistysTears : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OtherCards(ctx).Count > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = OtherCards(ctx);
                if (others.Count == 0) return;
                var pay = others.Count == 1 ? others[0] : ctx.ChooseCards(others, 1, 1, "Misty's Tears: discard which card from your hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pay);
                ctx.SearchToHand(d => d.IsEnergy && d.IsBasicEnergy && d.EnergyProvides == EnergyType.Water, 2, "Misty's Tears: choose up to 2 Water Energy cards");
            }
        }

        /// <summary>Rocket's Minefield Gym (your ruling): a Basic Pokémon put onto the Bench from the hand: its owner flips a coin; tails, 2 damage counters on it.</summary>
        private sealed class RocketsMinefieldGym : CardScript
        {
            public override void OnPokemonPutOnBench(EffectContext ctx, PokemonInPlay benched)
            {
                if (benched == null || !ctx.Engine.BenchingFromHand || !benched.Def.IsBasicPokemon) return;
                if (ctx.Engine.FlipCoin(benched.Owner, "Rocket's Minefield Gym")) return;
                ctx.Engine.PlaceDamageCounters(benched, 2, "Rocket's Minefield Gym");
            }
        }

        /// <summary>Rocket's Secret Experiment: heads, search your deck for any card; tails, you can't play Trainer cards until the end of your next turn.</summary>
        private sealed class RocketsSecretExperiment : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Rocket's Secret Experiment: choose any card", false, 1);
                    return;
                }
                ctx.AddEffect(new NoTrainersEffect(ctx.Engine.PlayerName(ctx.Player) + " can't play Trainer cards until the end of " + ctx.Engine.Their(ctx.Player) + " next turn (Rocket's Secret Experiment)", ctx.Player, ctx.Player, ctx.Turn + 2));
            }
        }

        /// <summary>
        /// Sabrina's Psychic Control (your ruling): heads, use an Item or Supporter card from the opponent's discard pile as if you
        /// played it from your hand (a Supporter doesn't count toward your 1 per turn); it stays in their discard pile.
        /// </summary>
        private sealed class SabrinasPsychicControl : CardScript
        {
            private static List<CardInstance> Usable(EffectContext ctx)
            {
                return ctx.Opp.Discard.Where(c => c.Def.IsTrainer && !c.Def.IsStadium && !c.Def.IsTool && !c.Def.IsFossil && c.Def.Name != ctx.Card.Def.Name
                                                  && ctx.Engine.Scripts.For(c.Def).CanPlayTrainer(new EffectContext(ctx.Engine, ctx.Player, c, null))).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Usable(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var usable = Usable(ctx);
                if (usable.Count == 0) return;
                var card = usable.Count == 1 ? usable[0] : ctx.ChooseCards(usable, 1, 1, "Sabrina's Psychic Control: use which card from your opponent's discard pile?")[0];
                ctx.Engine.UseTrainerFor(ctx.Player, card);
            }
        }

        /// <summary>Saffron City Gym: as often as they like during their turn, a player may return a basic Energy card from 1 of their Sabrina's Pokémon to their hand.</summary>
        private sealed class SaffronCityGym : CardScript
        {
            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => Owned(m, "Sabrina") && m.Energy.Any(c => c.Def.IsBasicEnergy)); }

            public override bool CanUseStadium(EffectContext ctx) { return Holders(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                var holders = Holders(ctx);
                if (holders.Count == 0) return false;
                var mon = holders.Count == 1 ? holders[0] : ctx.ChoosePokemon(holders, "Saffron City Gym: return a basic Energy card from which Pokémon?");
                if (mon == null) return false;
                var cards = mon.Energy.Where(c => c.Def.IsBasicEnergy).ToList();
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Saffron City Gym: return which Energy card to your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(mon, card);
                return false;
            }
        }

        /// <summary>Viridian City Gym: whenever a player evolves 1 of their Giovanni's Pokémon, remove 2 damage counters (or 1) from it.</summary>
        private sealed class ViridianCityGym : CardScript
        {
            public override void OnAnyPokemonEvolved(EffectContext src, PokemonInPlay evolved)
            {
                if (evolved == null || evolved.DamageCounters == 0 || !Owned(evolved, "Giovanni")) return;
                src.Engine.Heal(evolved, 10 * Math.Min(2, evolved.DamageCounters));
            }
        }

        /// <summary>Fervor: show the top 3 cards of your deck; Fire Energy cards go into your hand, the rest are discarded.</summary>
        private sealed class Fervor : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Me.Deck.Take(3).ToList();
                if (top.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, top, "showed the top " + top.Count + " cards of the deck (Fervor)");
                var fire = top.Where(c => IsFireEnergyCard(c.Def)).ToList();
                ctx.DeckToHand(fire);
                var rest = top.Where(c => !fire.Contains(c)).ToList();
                foreach (var card in rest)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + rest.Count + " card" + (rest.Count == 1 ? "" : "s") + " (Fervor).");
            }
        }

        /// <summary>Transparent Walls: until the end of the opponent's next turn, prevent all damage from attacks to your Benched Pokémon.</summary>
        private sealed class TransparentWalls : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new TransparentWallsEffect("Transparent Walls: no damage from attacks to " + ctx.Engine.PlayerName(ctx.Player) + "'s Benched Pokémon until the end of the opponent's next turn", ctx.Player, ctx.Turn + 1));
            }
        }

        private sealed class TransparentWallsEffect : ContinuousEffect
        {
            public TransparentWallsEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (target == null || target.Owner != Owner || info.TargetIsActive || info.Attacker == null || info.Attacker.Owner == Owner) return damage;
                return engine.CurrentSourceKind == EffectSourceKind.Attack ? 0 : damage;
            }
        }

        /// <summary>Warp Point: the opponent switches their Active Pokémon with 1 of their Benched Pokémon (their choice), then you switch yours.</summary>
        private sealed class WarpPoint : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null) ctx.OpponentSwitchesActive();
                if (ctx.Me.Bench.Count > 0 && ctx.Me.Active != null) ctx.SwitchMyActive("Warp Point: choose your new Active Pokémon");
            }
        }
    }
}
