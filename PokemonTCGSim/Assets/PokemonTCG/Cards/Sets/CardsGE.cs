using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Great Encounters (GE, 2008; v172): the cards not already in the pool (the others are prints). Unlimited only. Buizel 61,
    /// Krabby 75 and Wingull 95 have no card text.
    /// </summary>
    public static partial class CardsGE
    {
        private static bool UnownOnBench(EffectContext ctx, params string[] letters)
        {
            return letters.All(l => ctx.Me.Bench.Any(m => m.Name == "Unown [" + l + "]" || m.Name == "Unown " + l));
        }

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("GE-1", new ComboMon(new FireDanceMon(), new FlameKickMon()));
            r.RegisterByKey("GE-2", new ComboMon(new MoonTwinkleMon(), new LunarDanceMon()));
            r.RegisterByKey("GE-3", new ComboMon(new SearchAttackMon("Reach Over", d => d.IsEnergy, 1), new DarknessPursuitMon()));
            r.RegisterByKey("GE-4", new ComboMon(new DarknessRoarMon(), new SleepHoleMon()));
            r.RegisterByKey("GE-5", new ComboMon(new BenchSearchAttackMon("Call for Family", 3), new SmashShortMon()));
            r.RegisterByKey("GE-6", new PorygonZConversion());
            r.RegisterByKey("GE-7", new ComboMon(new AstonishMon("Astonish"), new CardsPLS1.FlipBonusConditionMon("Telebeam", 30, SpecialCondition.Paralyzed)));
            r.RegisterByKey("GE-8", new SceptileWildGrowth());
            r.RegisterByKey("GE-9", new SwampertWashOut());
            r.RegisterByKey("GE-10", new ComboMon(new PowerWhipMon(), new StickAndAbsorbMon()));
            r.RegisterByKey("GE-11", new CardsDS.EvolutionaryMon("Serene Grace", SereneGrace, c => c.Me.Deck.Count > 0, new AirScrollMon()));
            r.RegisterByKey("GE-12", new ComboMon(new TailsNothingParalyzeMon("Dragonbreath"), new CottonCloudMon()));
            r.RegisterByKey("GE-13", new ComboMon(new CardsASR1.TimesMon("Band Attack", c => c.MyPokemon(m => m.Name == "Beedrill").Count), new CoinsMon("Twineedle", 2)));
            r.RegisterByKey("GE-14", new ComboMon(new ParallelDrainMon(), new CardsSI.HeadsTailsConditionMon("Dozing Scales", SpecialCondition.Asleep, SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-17", new AmbientNoiseMon());
            r.RegisterByKey("GE-18", new ComboMon(new CardsHPB.KnockOffMon("Dark Fang", false), new BlackFireMon()));
            r.RegisterByKey("GE-19", new ComboMon(new DreamCatcherMon(), new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("GE-20", new ScissorBlowMon());
            r.RegisterByKey("GE-21", new CarryInMon());
            r.RegisterByKey("GE-22", new ComboMon(new MiraculousLightMon(), new CardsDR.DiscardTypesMon("Mist Ball", EnergyType.Fire, EnergyType.Water)));
            r.RegisterByKey("GE-23", new ComboMon(new FlipGateMon("Energy Draw", new CardsMT.SearchAttachSelfMon("Energy Draw", d => d.IsBasicEnergy)), new CardsTWM1.SelfDiscardMon("Luster Purge", 3)));
            r.RegisterByKey("GE-24", new ComboMon(new PickOutMon(), new JawBiteMon()));
            r.RegisterByKey("GE-25", new MiloticMarvelScale());
            r.RegisterByKey("GE-27", new PrimeapeAngerPoint());
            r.RegisterByKey("GE-28", new SlowkingTrumpCard());
            r.RegisterByKey("GE-29", new UnownHeal());
            r.RegisterByKey("GE-30", new WailordSleepingPulse());
            r.RegisterByKey("GE-31", new WeezingToxicVirus());
            r.RegisterByKey("GE-32", new WigglytuffGoodNightMelody());
            r.RegisterByKey("GE-33", new ComboMon(new DeadlyPoisonMon(), new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("GE-34", new ComboMon(new CoinsMon("Pin Missile", 3), new CardsFST2.ConditionBonusMon("Needle Arm", 20, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("GE-35", new ComboMon(new BulkUpMon(), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("GE-36", new PresentMon("Present"));
            r.RegisterByKey("GE-37", new ComboMon(new DodgeMon("Agility", false), new FlipGateMon("Aqua Jet", new Cards30CC.BenchHitMon("Aqua Jet", 10))));
            r.RegisterByKey("GE-38", new ComboMon(new SweetTemptationMon(), new PsychicSnapMon()));
            r.RegisterByKey("GE-39", new ComboMon(new CardsMEG.WeakenDefendingMon("Daunt", 20), new Cards30C.SelfCounterBonusMon("Rage", 10)));
            r.RegisterByKey("GE-40", new ComboMon(new SearchAttackMon("Greenness", d => CardsRS.BasicOf(d, EnergyType.Grass), 2), new ConditionAttackMon("Poison Breath", SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-41", new ComboMon(new FlipBonusMon("Palm Strike", 40), new SpiritedThrowMon()));
            r.RegisterByKey("GE-42", new ComboMon(new CardsEM.ScaryFaceMon(), new ThreatenAndDropMon()));
            r.RegisterByKey("GE-43", new ComboMon(new SwitcherooMon(), new Cards30CC.BenchHitMon("Overrun", 30)));
            r.RegisterByKey("GE-44", new ComboMon(new CardsSSB.AnyOpponentTextMon("Bass Control", 20), new FlipBonusMon("Stomp", 20)));
            r.RegisterByKey("GE-45", new CardsDX.ConditionImmuneMon(SpecialCondition.Asleep | SpecialCondition.Paralyzed, new LavaPlumeMon()));
            r.RegisterByKey("GE-46", new MuddyBombMon());
            r.RegisterByKey("GE-47", new EmergeMon("Emerge"));
            r.RegisterByKey("GE-48", new ComboMon(new GuzzleMon(), new JetReturnMon()));
            r.RegisterByKey("GE-49", new Porygon2Download());
            r.RegisterByKey("GE-50", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("GE-51", new ComboMon(new SearchAttackMon("Primal Calling", d => d.IsTrainer && d.Name.Contains("Fossil"), 3), new CardsDRX.BothConditionMon("Sleep Swirl", SpecialCondition.Asleep)));
            r.RegisterByKey("GE-52", new ComboMon(new TailBladeMon(), new CardsPLS1.FlipBonusConditionMon("Poison Tail", 30, SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-53", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Air Crash"), new CardsASC.SelfGuardMon("Steel Wing", 20)));
            r.RegisterByKey("GE-54", new CardsTEU1.AmnesiaMon("Amnesia"));
            r.RegisterByKey("GE-55", new SweetKissMon());
            r.RegisterByKey("GE-56", new UnownFake());
            r.RegisterByKey("GE-57", new UnownGuard());
            r.RegisterByKey("GE-58", new FlipGateMon("Tsunami", new CardsSS.AlsoEachBenchMon("Tsunami", 10)));
            r.RegisterByKey("GE-59", new CardsFST2.ConditionBonusMon("Cutting Claw", 50, c => c.OpponentPokemon().Any(m => m.Name == "Seviper")));
            r.RegisterByKey("GE-60", new ComboMon(new CardsSW.PsychicBalanceMon(), new SwitchSelfMon("Spin Turn", false)));
            r.RegisterByKey("GE-61", new PlainMon());
            r.RegisterByKey("GE-62", new ComboMon(new CardsMEG.SandAttackMon("Sand Attack"), new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-63", new EmergeMon("Pupate"));
            r.RegisterByKey("GE-64", new TailsNothingParalyzeMon("Clamp"));
            r.RegisterByKey("GE-65", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("GE-66", new ComboMon(new CardsE3B.HealSelfMon("Shed Skin", 20), new CardsPAR2.FlipConditionMon("Poison Fang", SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-67", new SearchAttackMon("Fast Evolution", d => d.IsEvolution, 1));
            r.RegisterByKey("GE-68", new CardsDAA1.GustAttackMon("Captivate", false));
            r.RegisterByKey("GE-69", new CardsDR.FlipDiscardSelfMon("Ember", EnergyType.Fire));
            r.RegisterByKey("GE-70", new CardsSS.BabyEvolutionMon("Jigglypuff", new InquireMon()));
            r.RegisterByKey("GE-71", new IllumiseScentConduct());
            r.RegisterByKey("GE-72", new ComboMon(new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep), new CardsASC.SelfGuardMon("Expand", 20)));
            r.RegisterByKey("GE-73", new ComboMon(new CardsASC.SelfGuardMon("Stiffen", 30), new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("GE-74", new SmogscreenMon());
            r.RegisterByKey("GE-75", new PlainMon());
            r.RegisterByKey("GE-76", new LunatoneGravityChange());
            r.RegisterByKey("GE-77", new ComboMon(new RendezvousMon(), new CardsFST2.ConditionBonusMon("Synchro Dance", 20, c => c.Defending != null && c.EnergyCount(c.Pokemon) == c.EnergyCount(c.Defending))));
            r.RegisterByKey("GE-78", new CardsSW.BotheringWaveMon("Arm Thrust"));
            r.RegisterByKey("GE-79", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("GE-80", new CardsRSB.CoinsPlusMon("Endeavor", 2, 20));
            r.RegisterByKey("GE-81", new CardsLTR2.ReorderTopMon("Calculate", 3));
            r.RegisterByKey("GE-82", new ComboMon(new CardsDP.RestMon("Rest", 30), new LockAfterAttackMon("Tumble Over", false)));
            r.RegisterByKey("GE-83", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("GE-84", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("GE-85", new SolrockSunlight());
            r.RegisterByKey("GE-86", new ComboMon(new DodgeMon("Super Speed", false), new Cards30CC.BenchHitMon("Shoot Through", 10)));
            r.RegisterByKey("GE-87", new ComboMon(new CardsDAA1.GustAttackMon("Vine Invite", false), new CardsOBF1.RetreatCostDamageMon("Grass Knot", 10, false)));
            r.RegisterByKey("GE-88", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new FlipGateMon("Find a Friend", new SearchAttackMon("Find a Friend", d => d.IsPokemon, 1))));
            r.RegisterByKey("GE-89", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("GE-90", new ComboMon(new CardsRS.ExtraEnergyMon("Energy Ball", 2), new CardsTWM1.HealSelfMon("Absorb", 10)));
            r.RegisterByKey("GE-91", new UnownLink());
            r.RegisterByKey("GE-92", new VolbeatLightConduct());
            r.RegisterByKey("GE-93", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new ParalyzeFlipMon("String Shot")));
            r.RegisterByKey("GE-94", new CardsDR.FlipEachOpponentMon("Uproar", 10));
            r.RegisterByKey("GE-95", new PlainMon());
            r.RegisterByKey("GE-96", new ComboMon(new PullOutMon(), new CoinsMon("Double Stab", 2)));

            r.RegisterByKey("GE-97", new AmuletCoin());
            r.RegisterByKey("GE-98", new FelicitysDrawing());
            r.RegisterByKey("GE-100", new MoonlightStadium());
            r.RegisterByKey("GE-101", new PremierBall());
            r.RegisterByKey("GE-103", new CresseliaLvXFullMoonDance());
        }

        // ------------------------------------------------------------------ Rares

        /// <summary>Fire Dance: attach a Fire Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class FireDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Dance") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.EnergyProvides == EnergyType.Fire).ToList();
                if (pile.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Fire Dance: attach a Fire Energy card to which Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(pile[0]) && !ctx.Engine.AttachEnergyCard(pile[0], target) && !ctx.Me.Discard.Contains(pile[0])) ctx.Me.Discard.Add(pile[0]);
                return true;
            }
        }

        /// <summary>Flame Kick: discard 2 Fire Energy; 80 to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class FlameKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Kick") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Flame Kick: discard 2 Fire Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                ctx.DamageAnyOpponent(80);
                return true;
            }
        }

        /// <summary>Moon Twinkle: with any Stadium in play, remove 2 damage counters from this Pokémon.</summary>
        private sealed class MoonTwinkleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moon Twinkle") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 20);
                return true;
            }
        }

        /// <summary>Lunar Dance: you may discard 2 Energy from this Pokémon to remove all damage counters from 1 of your Benched Pokémon.</summary>
        private sealed class LunarDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lunar Dance") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (!ctx.InPlay(me) || me.Energy.Count < 2 || hurt.Count == 0 || !ctx.AskYesNo("Lunar Dance: discard 2 Energy to heal 1 of your Benched Pokémon completely?")) return true;
                ctx.DiscardEnergyFrom(me, 2, "Lunar Dance: discard 2 Energy");
                var target = ctx.ChoosePokemon(hurt, "Lunar Dance: remove all damage counters from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Darkness Pursuit: base 100 against Cresselia.</summary>
        private sealed class DarknessPursuitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Pursuit") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.Defending.Name.StartsWith("Cresselia", StringComparison.Ordinal) ? 100 : atk.Damage);
                return true;
            }
        }

        /// <summary>Darkness Roar: you may have the opponent switch the Defending Pokémon with a Benched Pokémon.</summary>
        private sealed class DarknessRoarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Darkness Roar") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.AskYesNo("Darkness Roar: have your opponent switch their Active Pokémon?")) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Sleep Hole: 40 more if the Defending Pokémon is Asleep; if it isn't, it's now Asleep.</summary>
        private sealed class SleepHoleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Hole") return false;
                var defending = ctx.Defending;
                bool asleep = defending != null && (defending.Conditions & SpecialCondition.Asleep) != 0;
                ctx.DamageDefending(atk.Damage + (asleep ? 40 : 0));
                if (!asleep && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Smash Short: with a Pokémon Tool on the Defending Pokémon, 30 more; discard it, look at the opponent's hand and discard the Tools of the same name there.</summary>
        private sealed class SmashShortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smash Short") return false;
                var defending = ctx.Defending;
                var tool = defending == null ? null : defending.Tool;
                ctx.DamageDefending(atk.Damage + (tool != null ? 30 : 0));
                if (tool == null || !TefPokemon.StillDefending(ctx, defending) || !defending.HasTool(tool) || !ctx.CanAffect(defending)) return true;
                string name = tool.Def.Name;
                if (!ctx.Engine.DiscardTool(defending, tool)) return true;
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.ShowCards(ctx.Player, ctx.Opp.Hand.ToList(), "Smash Short", "Smash Short: your opponent's hand", ctx.Source);
                foreach (var card in ctx.Opp.Hand.Where(c => c.Def.Name == name).ToList()) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Conversion (Poké-Power): once during your turn, discard a basic Energy card from your hand; Porygon-Z is that type until the end of your turn. Not with a Special Condition. Tri Attack.</summary>
        private sealed class PorygonZConversion : Mon
        {
            private static readonly Mon Tri = new CoinsMon("Tri Attack", 3);

            protected override string Activated { get { return "Conversion"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList(), 1, 1, "Conversion: discard which basic Energy card?")[0];
                var type = card.Def.EnergyProvides;
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.AddEffect(new TypeIsEffect(ctx.Pokemon.Name + " is " + type + " this turn (Conversion)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn, type));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tri.HandleAttack(ctx, atk); }
        }

        internal sealed class TypeIsEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public TypeIsEffect(string name, int owner, int targetUid, int expiresAfterTurn, EnergyType type) : base(name, owner, targetUid, expiresAfterTurn, false) { this.type = type; }

            public override void ModifyTypes(GameEngine engine, PokemonInPlay mon, List<EnergyType> types)
            {
                if (mon == null || mon.Uid != TargetUid) return;
                types.Clear();
                types.Add(type);
            }
        }

        /// <summary>Wild Growth (Poké-Body): each basic Grass Energy card on your Grass Pokémon provides 2 Grass Energy (only 1 Wild Growth counts). Leaf Blade.</summary>
        private sealed class SceptileWildGrowth : Mon
        {
            private static readonly Mon Blade = new FlipBonusMon("Leaf Blade", 20);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (src.Pokemon == null || energy == null || holder == null || holder.Owner != src.Player || !energy.Def.IsBasicEnergy || energy.Def.EnergyProvides != EnergyType.Grass) return;
                if (units.Count != 1 || !src.Engine.IsType(holder, EnergyType.Grass)) return;
                var first = src.MyPokemon(m => m.Name == "Sceptile" && src.Engine.HasAbilityNamed(m, "Wild Growth")).OrderBy(m => m.Uid).FirstOrDefault();
                if (first != src.Pokemon) return;
                units.Add(EnergyMask.Of(EnergyType.Grass));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blade.HandleAttack(ctx, atk); }
        }

        /// <summary>Wash Out (Poké-Power): as often as you like during your turn, move a Water or Fighting Energy from 1 of your Benched Pokémon to your Active Pokémon. Not with a Special Condition. Hammer Arm.</summary>
        private sealed class SwampertWashOut : Mon
        {
            private const string Power = "Wash Out";

            private static bool Fits(EffectContext ctx, PokemonInPlay m, CardInstance e)
            {
                return CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Water) || CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fighting);
            }

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyBench().Where(m => m.Energy.Any(e => Fits(ctx, m, e))).ToList(); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Active != null && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => ctx.Me.Active != null && Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a Water or Fighting Energy to your Active Pokémon from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var cards = from.Energy.Where(e => Fits(ctx, from, e)).ToList();
                    var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, Power + ": move which Energy card?")[0];
                    return ctx.Engine.MoveEnergy(from, ctx.Me.Active, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hammer Arm") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>Power Whip: 10 damage for each Energy from basic Energy cards on this Pokémon (errata) to 1 of the opponent's Pokémon (no W/R on the Bench).</summary>
        private sealed class PowerWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Whip") return false;
                var me = ctx.Pokemon;
                int units = me.Energy.Where(e => e.Def.IsBasicEnergy).Sum(e => ctx.Engine.EnergyUnitsOf(me, e).Count);
                if (units == 0) return true;
                ctx.DamageAnyOpponent(10 * units);
                return true;
            }
        }

        /// <summary>Stick and Absorb: remove 3 damage counters from this Pokémon; the Defending Pokémon can't retreat next turn.</summary>
        private sealed class StickAndAbsorbMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stick and Absorb") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Serene Grace: look at the top 10 cards; attach any basic Energy cards there to your Pokémon in any way; the rest go back; shuffle.</summary>
        private static void SereneGrace(EffectContext ctx)
        {
            var top = ctx.LookAtTop(10);
            var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
            if (energy.Count > 0)
            {
                var picks = ctx.ChooseCards(energy, 0, energy.Count, "Serene Grace: attach which basic Energy cards?");
                foreach (var card in picks) ctx.Me.Deck.Remove(card);
                if (picks.Count > 0) ctx.AttachCardsAnyWay(picks, picks.Count, (c, m) => true, "Serene Grace: attach this Energy card to which Pokémon?", false);
                foreach (var card in picks) if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card)) && !ctx.Me.Discard.Contains(card) && !ctx.Me.Hand.Contains(card)) ctx.Me.Deck.Add(card);
            }
            ctx.ShuffleMyDeck();
        }

        /// <summary>Air Scroll: heads, 30 more; tails, remove 3 damage counters from this Pokémon.</summary>
        private sealed class AirScrollMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Air Scroll") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (!heads && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, this attack does nothing. If heads, the Defending Pokémon is now Paralyzed" (Dragonbreath, Clamp).</summary>
        internal sealed class TailsNothingParalyzeMon : Mon
        {
            private readonly string attack;

            public TailsNothingParalyzeMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Cotton Cloud: attack damage to this Pokémon from the opponent's Evolved Pokémon is 30 less next turn (after W/R).</summary>
        private sealed class CottonCloudMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cotton Cloud") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new EvolvedGuardEffect(ctx.Pokemon.Name + " takes 30 less damage from Evolved Pokémon next turn (Cotton Cloud)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 30));
                return true;
            }
        }

        private sealed class EvolvedGuardEffect : ContinuousEffect
        {
            private readonly int amount;

            public EvolvedGuardEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount) : base(name, owner, targetUid, expiresAfterTurn, true) { this.amount = amount; }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target == null || info.Target.Uid != TargetUid || a == null || a.Owner == Owner || !CardsE2.IsEvolved(a) || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage - amount;
            }
        }

        /// <summary>Parallel Drain: remove from 1 of your Pokémon as many damage counters as the damage done to the Defending Pokémon.</summary>
        private sealed class ParallelDrainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parallel Drain") return false;
                int done = ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (done <= 0 || hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Parallel Drain: remove damage counters from which of your Pokémon?");
                if (target != null) ctx.Heal(target, done);
                return true;
            }
        }

        /// <summary>Ambient Noise: 2 coins; 1st heads 50 to the Defending Pokémon, tails 20 to each Benched Pokémon; 2nd heads, Confused.</summary>
        private sealed class AmbientNoiseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ambient Noise") return false;
                var defending = ctx.Defending;
                bool first = ctx.Flip();
                bool second = ctx.Flip();
                if (first) ctx.DamageDefending(50);
                else foreach (var m in ctx.OpponentBench().ToList()) ctx.DamageTarget(m, 20);
                if (second && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Black Fire: you may discard a Fire or Darkness Energy: Fire, Burned; Darkness, 30 more (a rainbow card does both: GE FAQ).</summary>
        private sealed class BlackFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Black Fire") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                var cards = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire) || CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Darkness)).ToList();
                bool fire = false, dark = false;
                if (cards.Count > 0 && ctx.AskYesNo("Black Fire: discard a Fire or Darkness Energy?"))
                {
                    var card = ctx.ChooseCards(cards, 1, 1, "Black Fire: discard which Energy?")[0];
                    fire = CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Fire);
                    dark = CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Darkness);
                    ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage + (dark ? 30 : 0));
                if (fire && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Dream Catcher: if the Defending Pokémon is Asleep, move all damage counters from this Pokémon to it.</summary>
        private sealed class DreamCatcherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Catcher") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (defending == null || (defending.Conditions & SpecialCondition.Asleep) == 0 || me.DamageCounters == 0 || !ctx.CanAffect(defending)) return true;
                ctx.Engine.MoveDamageCounters(me, defending, me.DamageCounters, "Dream Catcher");
                return true;
            }
        }

        /// <summary>Scissor Blow: if this attack Knocks Out the Defending Pokémon, 30 to this Pokémon.</summary>
        private sealed class ScissorBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scissor Blow") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && (!ctx.InPlay(defending) || ctx.Engine.IsKnockedOut(defending))) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Carry In: a Pokémon Tool, a Supporter and a basic Energy card from your deck into your hand.</summary>
        private sealed class CarryInMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Carry In") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => d.IsTool, 1, "Carry In: choose a Pokémon Tool card");
                ctx.SearchToHand(d => d.IsSupporter, 1, "Carry In: choose a Supporter card");
                ctx.SearchToHand(d => d.IsBasicEnergy, 1, "Carry In: choose a basic Energy card");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Miraculous Light: remove 2 damage counters and all Special Conditions from this Pokémon.</summary>
        private sealed class MiraculousLightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Light") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 20);
                ctx.Engine.RemoveAllConditions(me);
                return true;
            }
        }

        /// <summary>Pick Out: turn 1 face-down Prize card (either player's) face up; if it's a Supporter, use its effect (it stays face up).</summary>
        private sealed class PickOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pick Out") return false;
                var mine = ctx.Me.Prizes.Where(c => !ctx.Me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                var theirs = ctx.Opp.Prizes.Where(c => !ctx.Opp.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (mine.Count + theirs.Count == 0) return Nothing(ctx);
                bool own = theirs.Count == 0 || (mine.Count > 0 && ctx.ChooseOption("Pick Out: turn up whose Prize card?", "Yours", "Your opponent's") == 0);
                var pool = own ? mine : theirs;
                var owner = own ? ctx.Me : ctx.Opp;
                var card = pool[ctx.Engine.State.Rng.Next(pool.Count)];
                owner.FaceUpPrizeIds.Add(card.Id);
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { card }, "Pick Out", "Pick Out: the Prize card turned face up", ctx.Source);
                ctx.Log(card.Def.Name + " was turned face up (Pick Out).");
                if (!card.Def.IsSupporter || !MrMimeTEF.WouldDoSomething(ctx, card)) return true;
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                ctx.Me.Hand.Remove(card);
                ctx.Me.Discard.Remove(card);
                ctx.Me.Deck.Remove(card);
                if (!owner.Prizes.Contains(card)) owner.Prizes.Add(card);
                return true;
            }
        }

        /// <summary>Jaw Bite: during your next turn, attacks that damage the Defending Pokémon do 20 more to it (after W/R).</summary>
        private sealed class JawBiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jaw Bite") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsSW.SilverWindEffect(defending.Name + " takes 20 more damage from attacks during the next turn (Jaw Bite)", ctx.Player, defending.Uid, ctx.Turn + 2, 20));
                return true;
            }
        }

        /// <summary>Marvel Scale (Poké-Body): prevent all effects of attacks, damage included, from the opponent's Pokémon LV.X. Twister.</summary>
        private sealed class MiloticMarvelScale : Mon
        {
            private static readonly Mon Twister = new CardsNP.TwisterMon();

            private static bool LvX(PokemonInPlay a) { return a != null && a.Def.IsLevelUp; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != src.Player && LvX(info.Attacker) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && !src.IsMyTurn && LvX(src.Opp.Active);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Twister.HandleAttack(ctx, atk); }
        }

        /// <summary>Anger Point (Poké-Body): with damage counters on it, Primeape's attacks do 40 more to the Active Pokémon (before W/R). Whump.</summary>
        private sealed class PrimeapeAngerPoint : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || me.DamageCounters == 0 || !info.ToOpponentsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whump") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Whump)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Trump Card (Poké-Power): once during your turn, if any of your Pokémon was Knocked Out during the opponent's last turn, any 1 card from your deck into your hand. Not with a Special Condition. Psych Up.</summary>
        private sealed class SlowkingTrumpCard : Mon
        {
            private static readonly Mon Psych = new CardsBKT2.SelfBoostMon("Psych Up", 30);

            protected override string Activated { get { return "Trump Card"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.WasKnockedOutLastOpponentTurn() && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.DeckToHand(ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, 1, "Trump Card: put which card into your hand?"));
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psych.HandleAttack(ctx, atk); }
        }

        /// <summary>HEAL (Poké-Power): once during your turn, with Unown H, E, A and L on your Bench, remove all Special Conditions from your Active Pokémon. Hidden Power: discard a card from your hand (can't: nothing).</summary>
        private sealed class UnownHeal : Mon
        {
            protected override string Activated { get { return "HEAL"; } }

            protected override bool AbilityReady(EffectContext ctx) { return UnownOnBench(ctx, "H", "E", "A", "L") && ctx.Me.Active != null && ctx.Me.Active.Conditions != SpecialCondition.None; }

            protected override void Ability(EffectContext ctx) { ctx.Engine.RemoveAllConditions(ctx.Me.Active); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (ctx.Me.Hand.Count == 0) return Nothing(ctx);
                ctx.Engine.DiscardFromHand(ctx.Player, ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Hidden Power: discard a card from your hand")[0]);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Sleeping Pulse (Poké-Body): while Wailord stays Asleep between turns, remove 1 damage counter from it. Sink Deep: with damage counters on it, it's now Asleep.</summary>
        private sealed class WailordSleepingPulse : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || (me.Conditions & SpecialCondition.Asleep) == 0 || me.DamageCounters == 0) return;
                Announce(src, "Sleeping Pulse");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sink Deep") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Toxic Virus (Poké-Body): each Poisoned Pokémon (both players') gets 1 more damage counter between turns (each Toxic Virus adds 1). Offensive Gas.</summary>
        private sealed class WeezingToxicVirus : Mon
        {
            private static readonly Mon Gas = new CardsCES1.FlipConditionsMon("Offensive Gas", SpecialCondition.Confused, SpecialCondition.Poisoned);

            public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters) { return src.Pokemon != null && mon != null ? counters + 1 : counters; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gas.HandleAttack(ctx, atk); }
        }

        /// <summary>Good Night Melody (Poké-Power): once during your turn, each Active Pokémon (both players') is now Asleep. Not with a Special Condition. Ballon Attack.</summary>
        private sealed class WigglytuffGoodNightMelody : Mon
        {
            private static readonly Mon Balloon = new CardsFST2.ConditionBonusMon("Ballon Attack", 20, c => EvolvedFrom(c, "Igglybuff"));

            protected override string Activated { get { return "Good Night Melody"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Asleep);
                if (ctx.Opp.Active != null && ctx.CanAffect(ctx.Opp.Active)) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Balloon.HandleAttack(ctx, atk); }
        }

        // ------------------------------------------------------------------ Uncommons

        /// <summary>Deadly Poison: 40 more if the Defending Pokémon is Poisoned; if not, it's now Poisoned.</summary>
        private sealed class DeadlyPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deadly Poison") return false;
                var defending = ctx.Defending;
                bool poisoned = defending != null && (defending.Conditions & SpecialCondition.Poisoned) != 0;
                ctx.DamageDefending(atk.Damage + (poisoned ? 40 : 0));
                if (!poisoned && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Bulk Up: during your next turn, this Pokémon's attacks do 30 more to the Defending Pokémon (before W/R; the effect stays on this Pokémon: May 2008).</summary>
        private sealed class BulkUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bulk Up") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && defending != null)
                    ctx.AddEffect(new CardsSW.DrillEffect(ctx.Pokemon.Name + "'s attacks do 30 more to " + defending.Name + " next turn (Bulk Up)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, defending.Uid, 30));
                return true;
            }
        }

        /// <summary>Present: heads, any 1 card from your deck into your hand.</summary>
        internal sealed class PresentMon : Mon
        {
            private readonly string attack;

            public PresentMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                ctx.DeckToHand(ctx.ChooseCards(ctx.Me.Deck.ToList(), 1, 1, attack + ": put which card into your hand?"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Sweet Temptation: switch in 1 of the opponent's Benched Pokémon; 10 to the new Defending Pokémon.</summary>
        private sealed class SweetTemptationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Temptation") return false;
                if (ctx.GustTargets().Count > 0) ctx.GustOpponent("Sweet Temptation: switch in which Benched Pokémon?");
                ctx.DamageDefending(10);
                return true;
            }
        }

        /// <summary>Psychic Snap: with Psychic Energy attached, 20 more and the Defending Pokémon is Confused.</summary>
        private sealed class PsychicSnapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Snap") return false;
                var defending = ctx.Defending;
                bool psychic = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Psychic) > 0;
                ctx.DamageDefending(atk.Damage + (psychic ? 20 : 0));
                if (psychic && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Spirited Throw: base 80 if this Pokémon has fewer HP left than the Defending Pokémon.</summary>
        private sealed class SpiritedThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirited Throw") return false;
                ctx.DamageDefending(ctx.Defending != null && ctx.RemainingHp(ctx.Pokemon) < ctx.RemainingHp(ctx.Defending) ? 80 : atk.Damage);
                return true;
            }
        }

        /// <summary>Threaten and Drop: with Darkness Energy attached, 20 more and discard a Special Energy card from the Defending Pokémon.</summary>
        private sealed class ThreatenAndDropMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Threaten and Drop") return false;
                var defending = ctx.Defending;
                bool dark = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Darkness) > 0;
                ctx.DamageDefending(atk.Damage + (dark ? 20 : 0));
                if (dark && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Any(e => e.Def.IsSpecialEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Threaten and Drop: discard a Special Energy card", e => e.Def.IsSpecialEnergy);
                return true;
            }
        }

        /// <summary>Switcheroo: move a Pokémon Tool from 1 of the opponent's Pokémon to another of theirs without a Tool.</summary>
        private sealed class SwitcherooMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Switcheroo") return false;
                var holders = ctx.OpponentPokemon(m => m.Tools.Count > 0 && ctx.CanAffect(m));
                var free = ctx.OpponentPokemon(m => m.Tools.Count == 0 && ctx.CanAffect(m));
                if (holders.Count == 0 || free.Count == 0) return Nothing(ctx);
                var from = ctx.ChoosePokemon(holders, "Switcheroo: move a Pokémon Tool from which Pokémon?");
                var to = ctx.ChoosePokemon(free, "Switcheroo: move it to which Pokémon?");
                if (from == null || to == null) return true;
                var tool = from.Tools[0];
                from.Tools.Remove(tool);
                ctx.Engine.AttachTool(tool, to);
                return true;
            }
        }

        /// <summary>Lava Plume: you may discard the top card of your deck; if you do, the Defending Pokémon is Burned.</summary>
        private sealed class LavaPlumeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Plume") return false;
                var defending = ctx.Defending;
                bool burn = ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Lava Plume: discard the top card of your deck to Burn the Defending Pokémon?");
                if (burn) ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                ctx.DamageDefending(atk.Damage);
                if (burn && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Muddy Bomb: 30 to 1 of the opponent's Pokémon and 10 to each of their others (no W/R on the Bench).</summary>
        private sealed class MuddyBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Muddy Bomb") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Muddy Bomb: do 30 damage to which Pokémon?");
                if (target == null) return true;
                var others = ctx.OpponentPokemon(m => m != target);
                ctx.DamageTarget(target, 30);
                foreach (var m in others) if (ctx.InPlay(m)) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        /// <summary>Emerge / Pupate (Poké-Power): once during your turn, if this Pokémon is Active, heads evolves it from your deck. Not with a Special Condition.</summary>
        private sealed class EmergeMon : Mon
        {
            private readonly string power;

            public EmergeMon(string power) { this.power = power; }

            protected override string Activated { get { return power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) CardsEVS1.EvolveFromDeck(ctx, ctx.Pokemon, power);
            }
        }

        /// <summary>Guzzle: attach up to 2 basic Water Energy cards from your hand to this Pokémon; remove 2 damage counters for each.</summary>
        private sealed class GuzzleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Guzzle") return false;
                var me = ctx.Pokemon;
                var water = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList();
                if (water.Count == 0) return true;
                int n = 0;
                foreach (var card in ctx.ChooseCards(water, 0, Math.Min(2, water.Count), "Guzzle: attach up to 2 basic Water Energy cards from your hand"))
                {
                    ctx.Me.Hand.Remove(card);
                    if (ctx.Engine.AttachEnergyCard(card, me)) n++;
                    else if (!ctx.Me.Hand.Contains(card) && !ctx.Me.Discard.Contains(card)) ctx.Me.Hand.Add(card);
                }
                if (n > 0 && me.DamageCounters > 0) ctx.Heal(me, 20 * n);
                return true;
            }
        }

        /// <summary>Jet Return: 30 times the Energy cards on this Pokémon; then they all go into your deck, shuffled.</summary>
        private sealed class JetReturnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Return") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage * me.Energy.Count);
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                foreach (var card in me.Energy.ToList())
                {
                    me.Energy.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Download (Poké-Power): once during your turn, discard a Supporter card from your hand and use its effect. Not with a Special Condition.</summary>
        private sealed class Porygon2Download : Mon
        {
            protected override string Activated { get { return "Download"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Any(c => c.Def.IsSupporter && MrMimeTEF.WouldDoSomething(ctx, c)); }

            protected override void Ability(EffectContext ctx)
            {
                var usable = ctx.Me.Hand.Where(c => c.Def.IsSupporter && MrMimeTEF.WouldDoSomething(ctx, c)).ToList();
                if (usable.Count == 0) return;
                var card = ctx.ChooseCards(usable, 1, 1, "Download: discard which Supporter card and use its effect?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + " (Download).");
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Remove(card);
                if (!ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
            }
        }

        /// <summary>Tail Blade: heads, the opponent can't play Trainer cards during their next turn.</summary>
        private sealed class TailBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tail Blade") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new DpTrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Tail Blade)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Sweet Kiss: the opponent may draw a card.</summary>
        private sealed class SweetKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Kiss") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "Sweet Kiss: draw a card?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>FAKE (Poké-Power): once during your turn, if Unown F is on your Bench, place a coin face; a wrong guess by the opponent draws you a card. Hidden Power: you may flip; tails nothing, heads base 30.</summary>
        private sealed class UnownFake : Mon
        {
            protected override string Activated { get { return "FAKE"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                bool heads = ctx.AskYesNo("FAKE: put the coin heads up? (No = tails)");
                bool guess = ctx.Engine.AskYesNo(ctx.Opponent, "FAKE: guess the coin is heads? (No = tails)", ctx.Source);
                if (guess != heads) ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                if (!ctx.AskYesNo("Hidden Power: flip a coin? (Heads: 30 damage; tails: nothing)"))
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(30);
                return true;
            }
        }

        /// <summary>
        /// GUARD (Poké-Power): once during your turn, if Unown G is on your Bench, discard its attached cards and attach it to 1 of your
        /// Pokémon as a Pokémon Tool; while attached, prevent all effects of the opponent's attacks, except damage, done to that
        /// Pokémon (errata). Hidden Power: base 10 if Unown G has damage counters.
        /// </summary>
        private sealed class UnownGuard : Mon
        {
            private static List<PokemonInPlay> Holders(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && ctx.Engine.CanAttachToolTo(ctx.Card, m)); }

            protected override string Activated { get { return "GUARD"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && ctx.Pokemon.Stack.Count == 1 && Holders(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var target = ctx.ChoosePokemon(Holders(ctx), "GUARD: attach Unown G to which of your Pokémon as a Pokémon Tool?");
                if (target == null) return;
                var card = me.Stack[0];
                ctx.Engine.RemoveFromPlay(me);
                ctx.Me.Discard.Remove(card);
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachTool(card, target);
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var holder = src.Pokemon;
                return holder != null && target == holder && holder.HasTool(src.Card) && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                ctx.DamageDefending(ctx.Pokemon.DamageCounters > 0 ? 10 : atk.Damage);
                return true;
            }
        }

        // ------------------------------------------------------------------ Commons

        /// <summary>Inquire: draw a card; 2 more if you played no Supporter from your hand this turn.</summary>
        private sealed class InquireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inquire") return false;
                ctx.Draw(ctx.Me.SupporterPlayedThisTurn ? 1 : 3);
                return true;
            }
        }

        /// <summary>Scent Conduct (Poké-Power): once during your turn, heads puts a Grass Basic Pokémon from your deck onto your Bench. Not with a Special Condition. Firefly Scent: Asleep if the Defending Pokémon has a Poké-Body.</summary>
        private sealed class IllumiseScentConduct : Mon
        {
            protected override string Activated { get { return "Scent Conduct"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.SearchToBench(d => d.IsBasicPokemon && !d.IsLevelUp && d.HasType(EnergyType.Grass), 1, "Scent Conduct: choose a Grass Basic Pokémon");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Firefly Scent") return false;
                var defending = ctx.Defending;
                bool body = defending != null && defending.Def.Abilities.Any(a => a.Kind == AbilityDef.PokeBody);
                ctx.DamageDefending(atk.Damage);
                if (body && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Smogscreen: the Defending Pokémon is Poisoned and must flip to attack next turn (tails, the attack does nothing).</summary>
        private sealed class SmogscreenMon : Mon
        {
            private static readonly Mon Sand = new CardsMEG.SandAttackMon("Smogscreen");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smogscreen") return false;
                var defending = ctx.Defending;
                Sand.HandleAttack(ctx, atk);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Gravity Change (Poké-Power): once during your turn, discard a card from your hand; then, with Solrock in play, draw a card. Not with a Special Condition. Knock Over.</summary>
        private sealed class LunatoneGravityChange : Mon
        {
            private static readonly Mon Knock = new CardsSIT1.MayDiscardStadiumMon("Knock Over");

            protected override string Activated { get { return "Gravity Change"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardFromHand(ctx.Player, ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Gravity Change: discard a card from your hand")[0]);
                if (ctx.MyPokemon(m => m.Name == "Solrock").Count > 0) ctx.Draw(1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Knock.HandleAttack(ctx, atk); }
        }

        /// <summary>Rendezvous: reveal the top card and put it into your hand; a Pokémon draws 2 more.</summary>
        private sealed class RendezvousMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rendezvous") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.Me.Deck[0];
                ctx.Engine.ShowCards(ctx.Opponent, new List<CardInstance> { top }, "Rendezvous", "Rendezvous: the top card of your opponent's deck", ctx.Source);
                ctx.DeckToHand(new[] { top });
                if (top.Def.IsPokemon) ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Sunlight (Poké-Body): with Lunatone in play, damage from your Psychic and Fighting Pokémon to the opponent's Pokémon ignores Resistance. Sol Charge.</summary>
        private sealed class SolrockSunlight : Mon
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || info.Target == null || info.Target.Owner == src.Player) return false;
                if (src.MyPokemon(m => m.Name == "Lunatone").Count == 0) return false;
                return src.Engine.IsType(a, EnergyType.Psychic) || src.Engine.IsType(a, EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sol Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                if (pile.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Sol Charge: attach which basic Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Sol Charge: attach it to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target) && !ctx.Me.Discard.Contains(card)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        /// <summary>LINK (Poké-Body): Unown L can use the attacks of any Unown in play (both players'), paying for them. Hidden Power: up to 2 cards from either discard pile on top of that player's deck, in any order.</summary>
        private sealed class UnownLink : Mon
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null) yield break;
                var seen = new HashSet<string>();
                foreach (var m in src.MyPokemon().Concat(src.OpponentPokemon()))
                {
                    if (m == me || !m.Name.StartsWith("Unown", StringComparison.Ordinal) || !seen.Add(m.Def.Key)) continue;
                    for (int i = 0; i < m.Def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(m.Def, i);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hidden Power") return false;
                bool mine = ctx.Opp.Discard.Count == 0 || (ctx.Me.Discard.Count > 0 && ctx.ChooseOption("Hidden Power: use whose discard pile?", "Yours", "Your opponent's") == 0);
                var pile = mine ? ctx.Me.Discard : ctx.Opp.Discard;
                var deck = mine ? ctx.Me.Deck : ctx.Opp.Deck;
                if (pile.Count == 0) return true;
                var picks = ctx.ChooseCards(pile.ToList(), 1, Math.Min(2, pile.Count), "Hidden Power: put which cards on top of that deck? (the first chosen ends up on top)");
                for (int i = picks.Count - 1; i >= 0; i--)
                    if (pile.Remove(picks[i])) deck.Insert(0, picks[i]);
                return true;
            }
        }

        /// <summary>Light Conduct (Poké-Power): once during your turn, with Illumise in play, put a Supporter card from your discard pile on top of your deck. Not with a Special Condition. Firefly Light.</summary>
        private sealed class VolbeatLightConduct : Mon
        {
            private static readonly Mon Light = new CardsPAR2.FlipConditionMon("Firefly Light", SpecialCondition.Confused);

            protected override string Activated { get { return "Light Conduct"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.MyPokemon(m => m.Name == "Illumise").Count > 0 && ctx.Me.Discard.Any(c => c.Def.IsSupporter); }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList(), 1, 1, "Light Conduct: put which Supporter card on top of your deck?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Insert(0, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Light.HandleAttack(ctx, atk); }
        }

        /// <summary>Pull Out: put any 1 card from your discard pile on top of your deck.</summary>
        private sealed class PullOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull Out") return false;
                if (ctx.Me.Discard.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Pull Out: put which card on top of your deck?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Me.Deck.Insert(0, card);
                return true;
            }
        }

        // ------------------------------------------------------------------ Trainers and LV.X

        /// <summary>Amulet Coin (Pokémon Tool): if the holder is your Active Pokémon at the end of your turn, draw a card.</summary>
        private sealed class AmuletCoin : CardScript
        {
            public override void OnEndOfTurn(EffectContext src)
            {
                var holder = src.Pokemon;
                if (holder == null || !src.IsMyTurn || !src.IsActive(holder) || src.Me.Deck.Count == 0) return;
                src.Log("Amulet Coin: " + src.Engine.PlayerName(src.Player) + " draws a card.");
                src.Draw(1);
            }
        }

        /// <summary>Felicity's Drawing (Supporter): discard 1 card from your hand to draw 3, or 2 to draw 4 (not 0: GE FAQ).</summary>
        private sealed class FelicitysDrawing : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var picks = ctx.ChooseCards(others, 1, Math.Min(2, others.Count), "Felicity's Drawing: discard 1 card (draw 3) or 2 cards (draw 4)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(picks.Count == 2 ? 4 : 3);
            }
        }

        /// <summary>Moonlight Stadium (Stadium): every Psychic and Darkness Pokémon's Retreat Cost is 0.</summary>
        private sealed class MoonlightStadium : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (mon == null) return cost;
                return src.Engine.IsType(mon, EnergyType.Psychic) || src.Engine.IsType(mon, EnergyType.Darkness) ? 0 : cost;
            }
        }

        /// <summary>Premier Ball: a Pokémon LV.X from your deck or discard pile into your hand.</summary>
        private sealed class PremierBall : CardScript
        {
            private static bool Wanted(CardDef d) { return d.IsPokemon && d.IsLevelUp; }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 || (ctx.Me.Discard.Any(c => Wanted(c.Def)) && !ctx.Engine.DiscardToHandBlocked(ctx.Player)); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                bool discard = ctx.Me.Discard.Any(c => Wanted(c.Def)) && !ctx.Engine.DiscardToHandBlocked(ctx.Player)
                    && (ctx.Me.Deck.Count == 0 || ctx.ChooseOption("Premier Ball: search where?", "Deck", "Discard pile") == 1);
                if (discard)
                {
                    ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.Where(c => Wanted(c.Def)).ToList(), 1, 1, "Premier Ball: put which Pokémon LV.X into your hand?"));
                    return;
                }
                ctx.SearchToHand(Wanted, 1, "Premier Ball: choose a Pokémon LV.X");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Full Moon Dance (Poké-Power): once during your turn, move 1 damage counter from any Pokémon to another (either player's). Not with a Special Condition. Moon Skip: 1 more Prize card if it Knocks Out the Defending Pokémon.</summary>
        private sealed class CresseliaLvXFullMoonDance : Mon
        {
            protected override string Activated { get { return "Full Moon Dance"; } }

            private static List<PokemonInPlay> All(EffectContext ctx) { return ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && All(ctx).Any(m => m.DamageCounters > 0) && All(ctx).Count > 1; }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(All(ctx).Where(m => m.DamageCounters > 0).ToList(), "Full Moon Dance: move a damage counter from which Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(All(ctx).Where(m => m != from).ToList(), "Full Moon Dance: move it to which Pokémon?");
                if (to == null) return;
                if ((from.Owner == ctx.Player || ctx.CanAffect(from)) && (to.Owner == ctx.Player || ctx.CanAffect(to))) ctx.Engine.MoveDamageCounters(from, to, 1, "Full Moon Dance");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moon Skip") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.AddEffect(new KnockOutPrizeBonusEffect("Moon Skip: 1 more Prize card", ctx.Player, defending.Uid, ctx.Turn, 1));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }
    }
}
