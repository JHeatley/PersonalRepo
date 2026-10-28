using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragons Exalted (DRX 1-128, 2012; Expanded). Cards reprinted in Legendary Treasures, Boundaries Crossed, the XY promos (Mew-EX),
    /// 30th Celebration's Classic Collection (Rayquaza-EX), Evolutions (Devolution Spray) and Ascended Heroes (Tool Scrapper) are prints
    /// of those, and 119-124 are the full arts. Your ruling (v125): Reuniclus's Damage Swap may Knock Out your own Pokémon.
    /// </summary>
    public static class CardsDRX
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DRX-1", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("DRX-2", new CoinsMon("Bullet Seed", 4));
            r.RegisterByKey("DRX-3", new JumpluffLeaveIt());
            r.RegisterByKey("DRX-4", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("DRX-5", new DodgeMon("Agility", false));
            r.RegisterByKey("DRX-6", new FlipTwoConditionsMon("Sleep Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned));
            r.RegisterByKey("DRX-7", new CardsLOR1.HardenMon("Harden", 60));
            r.RegisterByKey("DRX-8", new ComboMon(new TripleEnergyMon(), new DrainpourMon()));
            r.RegisterByKey("DRX-9", new ComboMon(new CardsDAA1.GustAttackMon("Tangle Drag", false), new CardsTWM1.HealSelfMon("Spiral Drain", 20)));
            r.RegisterByKey("DRX-10", new DodgeMon("Dig", false));
            r.RegisterByKey("DRX-11", new NinjaskCastOffShell());
            r.RegisterByKey("DRX-12", new ComboMon(new CoinsMon("Double Whip", 2), new RelaxingFragranceMon()));
            r.RegisterByKey("DRX-13", new FlipBonusMon("Needling Sting", 20));
            r.RegisterByKey("DRX-14", new ComboMon(new CoinsMon("Crosswise Whip", 4), new ConditionAttackMon("Poison Point", SpecialCondition.Poisoned)));
            r.RegisterByKey("DRX-15", new RoseradeLeParfum());
            r.RegisterByKey("DRX-16", new ComboMon(new ParalyzeFlipMon("Stun Needle"), new CardsASC.ToolBonusMon("Reinforced Needle", 40)));
            r.RegisterByKey("DRX-17", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("DRX-18", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("DRX-19", new NinetalesBrightLook());
            r.RegisterByKey("DRX-21", new ComboMon(new CardsASC.SelfGuardMon("Flame Screen", 20), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("DRX-22", new HoOhRebirth());
            r.RegisterByKey("DRX-23", new CardsPFL.FlipUntilTailsTimesMon("Soggy Rush"));
            r.RegisterByKey("DRX-24", new CardsWHT.FlipMoreMon("Swing Around", 2, 30));
            r.RegisterByKey("DRX-25", new CardsPAR2.FlipConditionMon("Lullaby", SpecialCondition.Asleep));
            r.RegisterByKey("DRX-26", new ComboMon(new WaterCannonMon(), new LockAfterAttackMon("Giant Wave", true)));
            r.RegisterByKey("DRX-27", new AddADashMon());
            r.RegisterByKey("DRX-28", new ComboMon(new SearchAttackMon("Clear Search", d => true, 3), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("DRX-29", new BothHeadsBonusMon("Unstoppable Roll", 30));
            r.RegisterByKey("DRX-31", new IceEntombMon());
            r.RegisterByKey("DRX-34", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("DRX-35", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("DRX-36", new ComboMon(new CardsASC.NextTurnBoostMon("Echoed Voice", 50), new CardsTWM1.HealSelfMon("Drain Punch", 20)));
            r.RegisterByKey("DRX-37", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Mysterious Beam"), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("DRX-38", new ComboMon(new CardsASC.SelfGuardMon("Cotton Guard", 20), new CardsFST1.FlipRecoilMon("Thunder Jolt", 10)));
            r.RegisterByKey("DRX-39", new CardsASC.SelfGuardMon("Cotton Guard", 20));
            r.RegisterByKey("DRX-40", new AmpharosWall());
            r.RegisterByKey("DRX-41", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("DRX-42", new CoinsMon("Quick Turn", 2));
            r.RegisterByKey("DRX-43", new ComboMon(new CardsASR1.TimesMon("Energy Crush", c => c.OpponentPokemon().Sum(m => c.EnergyCount(m))), new FlashImpactMon(20)));
            r.RegisterByKey("DRX-44", new ComboMon(new EnergyAssistTwoMon(), new FlipBonusMon("Quick Attack", 20)));
            r.RegisterByKey("DRX-47", new ComboMon(new HazardousScalesMon(), new FlipBonusMon("Aerial Ace", 30)));
            r.RegisterByKey("DRX-48", new ShedinjaEmptyShell());
            r.RegisterByKey("DRX-49", new CardsRCL1.CountersOnOneMon("Sneaky Placement", 1));
            r.RegisterByKey("DRX-51", new ComboMon(new CardsASR1.TimesMon("Shadow Steal", c => c.Opp.Discard.Count(x => x.Def.IsSpecialEnergy)), new CardsRCL1.CountersOnOneMon("Plentiful Placement", 4)));
            r.RegisterByKey("DRX-55", new CardsLTR2.ReorderTopMon("Future Sight", 5));
            r.RegisterByKey("DRX-56", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep), new CardsFST1.NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("DRX-57", new ComboMon(new DoomDecreeMon(), new CardsASR1.PlusMon("Black Magic", 20, c => c.Opp.Bench.Count)));
            r.RegisterByKey("DRX-58", new CardsTWM1.HealSelfMon("Nap", 40));
            r.RegisterByKey("DRX-61", new ComboMon(new NoRetreatMon("Bone Lock"), new CardsFST2.ConditionBonusMon("Vortex Chop", 30, HasResistance)));
            r.RegisterByKey("DRX-63", new ReverseSpinMon("Reverse Spin"));
            r.RegisterByKey("DRX-64", new ComboMon(new RapidSpinMon(), new FlipBonusMon("Rock Smash", 30)));
            r.RegisterByKey("DRX-65", new FlipBonusMon("Stone Edge", 20));
            r.RegisterByKey("DRX-66", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Rock Cannon"), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("DRX-67", new ComboMon(new CardsASR1.PlusMon("Revenge Cannon", 10, c => c.Me.Bench.Sum(m => m.DamageCounters)), new RecoilMon("Reckless Charge", 40)));
            r.RegisterByKey("DRX-68", new ComboMon(new SqueezeMon("Squeeze"), new CardsSUM5.OptionalBoostRecoilMon("Superpower", 20, 20)));
            r.RegisterByKey("DRX-69", new ComboMon(new DefensiveStanceMon(), new CardsBS1.KarateChopMon()));
            r.RegisterByKey("DRX-71", new ComboMon(new CardsFLF1.NoResistanceMon("Rock Tumble"), new PumpUpSmashMon()));
            r.RegisterByKey("DRX-73", new ComboMon(new CardsDRI1.WhirlwindMon("Whirlwind"), new CardsASC.RandomDiscardMon("Diving Swipe")));
            r.RegisterByKey("DRX-75", new ConditionAttackMon("Fire Fang", SpecialCondition.Burned));
            r.RegisterByKey("DRX-76", new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray"));
            r.RegisterByKey("DRX-77", new SmogscreenMon());
            r.RegisterByKey("DRX-78", new RecoilMon("Take Down", 10));
            r.RegisterByKey("DRX-79", new WreakHavocMon());
            r.RegisterByKey("DRX-80", new AggronTopplingWind());
            r.RegisterByKey("DRX-81", new ComboMon(new TripleLaserMon(), new CardsASC.SelfGuardMon("Protect Charge", 20)));
            r.RegisterByKey("DRX-82", new ComboMon(new MagneticLinesMon(), new CardsPFL.DamagedBonusMon("Heavy Nose", 30)));
            r.RegisterByKey("DRX-83", new ComboMon(new PullOutMon(), new CardsPFL.FlipUntilTailsTimesMon("Iron Head")));
            r.RegisterByKey("DRX-87", new ComboMon(new CardsMEG.SandAttackMon("Sand-Attack"), new FlipBonusMon("Knock Away", 20)));
            r.RegisterByKey("DRX-89", new GabiteDragonCall());
            r.RegisterByKey("DRX-91", new NoRetreatMon("Sand Tomb"));
            r.RegisterByKey("DRX-92", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsBRS1.MillSelfMon("Dragon Pulse", 3)));
            r.RegisterByKey("DRX-94", new CardsASC.SelfGuardMon("Guard Press", 10));
            r.RegisterByKey("DRX-96", new CardsSP1.AttachUpToFromDiscardToSelfMon("Draw In", EnergyType.Darkness, 2));
            r.RegisterByKey("DRX-98", new ComboMon(new ConsumeMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Destructor Beam")));
            r.RegisterByKey("DRX-99", new CoinsMon("Double Hit", 2));
            r.RegisterByKey("DRX-100", new ComboMon(new CoinsMon("Double Hit", 2), CardsBKT1.HandFling()));
            r.RegisterByKey("DRX-101", new CardsRCL2.SelfConditionMon("Smack 'n' Slack", SpecialCondition.Asleep));
            r.RegisterByKey("DRX-102", new FlipBonusMon("Ambush", 40));
            r.RegisterByKey("DRX-103", new SlakingUnobservant());
            r.RegisterByKey("DRX-104", new ConditionAttackMon("Sing", SpecialCondition.Asleep));
            r.RegisterByKey("DRX-106", new BothConditionMon("Bang Heads", SpecialCondition.Confused));
            r.RegisterByKey("DRX-107", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new BothConditionMon("Tumbling Tackle", SpecialCondition.Asleep)));
            r.RegisterByKey("DRX-108", new ComboMon(new WakeUpBeamMon(), new CardsTWM1.HealSelfMon("Drain Slap", 30)));
            r.RegisterByKey("DRX-109", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("DRX-110", new BouffalantBouffer());
            r.RegisterByKey("DRX-111", new ComboMon(new SearchAttackMon("Chirp", d => d.IsPokemon && d.Resistance == EnergyType.Fighting, 2), new FlipBonusMon("Sharp Beak", 20)));
            r.RegisterByKey("DRX-112", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("DRX-114", new GiantCape());
            r.RegisterByKey("DRX-115", new RescueScarf());
            r.RegisterByKey("DRX-117", new CardsFLI4.UnitEnergy(EnergyType.Grass, EnergyType.Fire, EnergyType.Psychic, EnergyType.Darkness));
            r.RegisterByKey("DRX-118", new CardsFLI4.UnitEnergy(EnergyType.Water, EnergyType.Lightning, EnergyType.Fighting, EnergyType.Metal));
            r.RegisterByKey("DRX-126", new ReuniclusDamageSwap());
            r.RegisterByKey("DRX-127", new ComboMon(new NoRetreatMon("Dark Clamp"), new CardsASR1.TimesMon("Bombast", c => c.MyPrizesTaken)));
        }

        // ================================================================== Shared

        private static bool HasResistance(EffectContext ctx)
        {
            var defending = ctx.Defending;
            return defending != null && ctx.Engine.ResistanceOf(defending).HasValue;
        }

        private static readonly SpecialCondition[] AllConditions =
            { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

        internal static int ConditionCount(PokemonInPlay mon)
        {
            return mon == null ? 0 : AllConditions.Count(mon.Has);
        }

        // ================================================================== Abilities

        /// <summary>Leave It to the Wind: once during your turn, return this Pokémon and its cards to your hand (not your last Pokémon in play). Acrobatics: 2 coins, +30 each.</summary>
        private sealed class JumpluffLeaveIt : Mon
        {
            private const string Leave = "Leave It to the Wind";
            private static readonly Mon Acrobatics = new CardsWHT.FlipMoreMon("Acrobatics", 2, 30);

            protected override string Activated { get { return Leave; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !ctx.IsActive(ctx.Pokemon) || ctx.Me.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Acrobatics.HandleAttack(ctx, atk); }
        }

        /// <summary>Cast-off Shell: evolving from the hand, you may search your deck for Shedinja and put it onto your Bench. Night Slash: you may switch.</summary>
        private sealed class NinjaskCastOffShell : Mon
        {
            private static readonly Mon Slash = new SwitchSelfMon("Night Slash", true);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Cast-off Shell", ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0, "Cast-off Shell: search your deck for Shedinja and put it onto your Bench?")) return;
                var found = ctx.SearchMyDeck(d => d.Name == "Shedinja" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, "Cast-off Shell: choose Shedinja");
                foreach (var card in found)
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Hand.Add(card);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slash.HandleAttack(ctx, atk); }
        }

        /// <summary>Le Parfum: evolving from the hand, you may search your deck for any card. Squeeze: heads, +20 and Paralyzed.</summary>
        private sealed class RoseradeLeParfum : Mon
        {
            private static readonly Mon Squeeze = new SqueezeMon("Squeeze");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Le Parfum", ctx.Me.Deck.Count > 0, "Le Parfum: search your deck for any card and put it into your hand?")) return;
                ctx.SearchToHand(d => true, 1, "Le Parfum: choose any card", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Squeeze.HandleAttack(ctx, atk); }
        }

        /// <summary>Bright Look: evolving from the hand, you may switch 1 of the opponent's Benched Pokémon with their Active Pokémon. Hexed Flame: +50 for each Special Condition on the Defending Pokémon.</summary>
        private sealed class NinetalesBrightLook : Mon
        {
            private static readonly Mon Hexed = new CardsASR1.PlusMon("Hexed Flame", 50, c => ConditionCount(c.Defending));

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (TriggerOnce(ctx, "Bright Look", ctx.GustTargets().Count > 0, "Bright Look: switch 1 of your opponent's Benched Pokémon with their Active Pokémon?"))
                    ctx.GustOpponent("Bright Look: choose the Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hexed.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rebirth: once during your turn, if this card is in your discard pile, flip a coin; heads, put it onto your Bench and attach
        /// basic Energy cards of 3 different types from your discard pile to it (as many different types as there are, up to 3).
        /// Rainbow Burn: +20 for each type of basic Energy attached.
        /// </summary>
        private sealed class HoOhRebirth : Mon
        {
            private const string Rebirth = "Rebirth";
            private static readonly Mon Burn = new CardsASR1.PlusMon("Rainbow Burn", 20,
                c => c.Pokemon == null ? 0 : c.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count());

            private static string LimitKey(EffectContext ctx) { return Rebirth + "#" + ctx.Card.Id; }

            public override bool CanUseFromDiscard(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Discard.Contains(ctx.Card) && ctx.BenchSpace > 0 && !ctx.NamedLimitUsedThisTurn(LimitKey(ctx))
                       && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, ctx.Card.Def);
            }

            public override void UseFromDiscard(EffectContext ctx)
            {
                ctx.MarkNamedLimit(LimitKey(ctx));
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + "'s Ho-Oh-EX used its Ability Rebirth.", ctx.Card.Id);
                if (!ctx.Flip()) return;
                if (!ctx.Me.Discard.Remove(ctx.Card)) return;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, ctx.Card, false);
                if (mon == null) return;
                var types = new List<EnergyType>();
                for (int i = 0; i < 3; i++)
                {
                    var taken = types.ToList();
                    var options = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && !taken.Contains(c.Def.EnergyProvides)).ToList();
                    if (options.Count == 0) break;
                    var pick = options.Count == 1 ? options[0]
                        : ctx.ChooseCards(options, 1, 1, "Rebirth: attach a basic Energy card of a different type (" + (i + 1) + " of 3)")[0];
                    types.Add(pick.Def.EnergyProvides);
                    if (ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, mon);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burn.HandleAttack(ctx, atk); }
        }

        /// <summary>Electromagnetic Wall: while this Pokémon is Active, whenever the opponent attaches an Energy card from their hand to 1 of their Pokémon, 3 damage counters on it. Electrobullet: 20 to 1 Benched.</summary>
        private sealed class AmpharosWall : Mon
        {
            private static readonly Mon Bullet = new Cards30CC.BenchHitMon("Electrobullet", 20);

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon) || target == null || !src.InPlay(target) || target.Owner == src.Player) return;
                Announce(src, "Electromagnetic Wall");
                src.Engine.PlaceDamageCounters(target, 3, "Electromagnetic Wall");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bullet.HandleAttack(ctx, atk); }
        }

        /// <summary>Empty Shell: if this Pokémon is Knocked Out, the opponent takes no Prize cards for it. Cursed Drop: 3 counters any way.</summary>
        private sealed class ShedinjaEmptyShell : Mon
        {
            private static readonly Mon Drop = new CardsLOR1.CountersAnyWayMon("Cursed Drop", 3);

            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (knockedOut == null || knockedOut != src.Pokemon || taker == src.Player || prizes <= 0) return prizes;
                src.Log("Empty Shell: " + src.Engine.PlayerName(taker) + " takes no Prize cards for " + knockedOut.Name + ".");
                return 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drop.HandleAttack(ctx, atk); }
        }

        /// <summary>Dragon Call: once during your turn, search your deck for a Dragon Pokémon and put it into your hand.</summary>
        private sealed class GabiteDragonCall : Mon
        {
            private const string Call = "Dragon Call";

            protected override string Activated { get { return Call; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.PokemonType == EnergyType.Dragon, 1, Call + ": choose a Dragon Pokémon");
            }
        }

        /// <summary>Unobservant: this Pokémon can't attack while the opponent's Active Pokémon is a Basic Pokémon. Crushing Blow: discard an Energy from the Defending Pokémon.</summary>
        private sealed class SlakingUnobservant : Mon
        {
            private static readonly Mon Blow = new CardsDRI2.CrunchMon("Crushing Blow");

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var opp = ctx.Opp.Active;
                return opp == null || !opp.Def.IsBasicPokemon || ctx.Pokemon == null || ctx.Engine.AbilityBlocked(ctx.Pokemon, "Unobservant");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blow.HandleAttack(ctx, atk); }
        }

        /// <summary>Bouffer: damage done to this Pokémon by attacks is reduced by 20 (after W/R). Gold Breaker: +60 against a Pokémon-EX.</summary>
        private sealed class BouffalantBouffer : Mon
        {
            private static readonly Mon Breaker = new CardsFST2.ConditionBonusMon("Gold Breaker", 60, c => c.Defending != null && c.Defending.Def.IsPokemonEX);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target != src.Pokemon || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Math.Max(0, damage - 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Breaker.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Damage Swap (Ability): as often as you like during your turn, move 1 damage counter from 1 of your Pokémon to another (your
        /// ruling, v125: even if that Knocks it Out). Psywave: +10 for each Energy attached to the Defending Pokémon.
        /// </summary>
        private sealed class ReuniclusDamageSwap : Mon
        {
            private const string Swap = "Damage Swap";
            private static readonly Mon Psywave = new CardsMEG.DefendingEnergyBonusMon("Psywave", 10);

            private static List<PokemonInPlay> From(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.RemainingHp(m) > 0); }

            private static List<PokemonInPlay> To(EffectContext ctx, PokemonInPlay from) { return ctx.MyPokemon(m => m != from && ctx.RemainingHp(m) > 0); }

            private static bool CanMove(EffectContext ctx) { return From(ctx).Any(f => To(ctx, f).Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Swap) return false;
                return ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                Announce(ctx, Swap);
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var sources = From(ctx).Where(f => To(ctx, f).Count > 0).ToList();
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, sources, Swap + ": move a damage counter from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, To(ctx, from), Swap + ": move it from " + from.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveDamageCounters(from, to, 1, Swap) > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psywave.HandleAttack(ctx, atk); }
        }

        // ================================================================== Attacks

        /// <summary>"Flip a coin. If heads, the Defending Pokémon is now [A] and [B]" (Sleep Poison).</summary>
        private sealed class FlipTwoConditionsMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition first, second;

            public FlipTwoConditionsMon(string attack, SpecialCondition first, SpecialCondition second)
            {
                this.attack = attack;
                this.first = first;
                this.second = second;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!heads || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, first);
                ctx.ApplyCondition(defending, second);
                return true;
            }
        }

        /// <summary>Hazardous Scales: the Defending Pokémon is now Asleep, Burned and Poisoned.</summary>
        private sealed class HazardousScalesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hazardous Scales") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Triple Energy: search your deck for basic Energy cards of 3 different types and attach them to your Pokémon in any way you like.</summary>
        private sealed class TripleEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Energy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = new List<CardInstance>();
                for (int i = 0; i < 3; i++)
                {
                    var types = found.Select(c => c.Def.EnergyProvides).ToList();
                    var one = ctx.SearchMyDeck(d => d.IsEnergy && d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                        "Triple Energy: choose a basic Energy card of a different type (" + (i + 1) + " of 3)");
                    if (one.Count == 0) break;
                    found.AddRange(one);
                }
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (e, m) => true, "Triple Energy: attach the Energy to your Pokémon", false);
                foreach (var card in found)
                    if (!ctx.Me.Deck.Contains(card) && !ctx.MyPokemon().Any(m => m.Energy.Contains(card))) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Drainpour: heal 40 damage from each of your Benched Pokémon.</summary>
        private sealed class DrainpourMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drainpour") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Me.Bench.Where(m => m.DamageCounters > 0).ToList()) ctx.Heal(mon, 40);
                return true;
            }
        }

        /// <summary>Relaxing Fragrance: heal 30 damage and remove all Special Conditions from this Pokémon.</summary>
        private sealed class RelaxingFragranceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Relaxing Fragrance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 30);
                foreach (var c in AllConditions)
                    if (me.Has(c)) ctx.Engine.RemoveCondition(me, c);
                return true;
            }
        }

        /// <summary>Squeeze: flip a coin; heads, 20 more damage and the Defending Pokémon is now Paralyzed. Used for Squeeze, Sticky Hands (the attack name is passed in).</summary>
        internal sealed class SqueezeMon : Mon
        {
            private readonly string attack;

            public SqueezeMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Water Cannon: flip a coin; heads, 30 damage for each Water Energy attached to this Pokémon (tails, nothing).</summary>
        private sealed class WaterCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Cannon") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                int water = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                if (water == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * water);
                return true;
            }
        }

        /// <summary>Add-a-Dash: flip 2 coins; draw a card for each heads.</summary>
        private sealed class AddADashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Add-a-Dash") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.Draw(heads);
                return true;
            }
        }

        /// <summary>"Flip 2 coins. If both of them are heads, this attack does N more damage" (Unstoppable Roll).</summary>
        private sealed class BothHeadsBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public BothHeadsBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.FlipCoins(2) == 2 ? bonus : 0));
                return true;
            }
        }

        /// <summary>Ice Entomb: the Defending Pokémon is now Paralyzed; this Pokémon can't use Ice Entomb during your next turn.</summary>
        private sealed class IceEntombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Entomb") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn("Ice Entomb");
                return true;
            }
        }

        /// <summary>Flash Impact: the damage, then N to 1 of your own Pokémon (no W/R for Benched Pokémon).</summary>
        internal sealed class FlashImpactMon : Mon
        {
            private readonly int amount;

            public FlashImpactMon(int amount) { this.amount = amount; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flash Impact") return false;
                ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = ctx.ChoosePokemon(mine, "Flash Impact: do " + amount + " damage to which of your Pokémon?");
                if (target != null && ctx.InPlay(target)) ctx.DamageWith(target, amount, false, false, false);
                return true;
            }
        }

        /// <summary>Energy Assist: attach 2 basic Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EnergyAssistTwoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Assist") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var bench = ctx.Me.Bench.ToList();
                var energy = ctx.BasicEnergyInDiscard();
                if (bench.Count == 0 || energy.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Energy Assist: attach 2 basic Energy cards to which of your Benched Pokémon?");
                if (target == null) return true;
                var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Energy Assist: choose 2 basic Energy cards");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Doom Decree: flip 2 coins; both heads, the Defending Pokémon is Knocked Out.</summary>
        private sealed class DoomDecreeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom Decree") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 2 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.KnockOut(defending, "Doom Decree");
                return true;
            }
        }

        /// <summary>Reverse Spin: the opponent shuffles their hand into their deck and draws 4 cards. Used for Reverse Spin, Nitpick (the attack name is passed in).</summary>
        internal sealed class ReverseSpinMon : Mon
        {
            private readonly string attack;

            public ReverseSpinMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
                return true;
            }
        }

        /// <summary>Rapid Spin: switch this Pokémon with 1 of your Benched Pokémon; then the opponent switches the Defending Pokémon with 1 of theirs.</summary>
        private sealed class RapidSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rapid Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Defensive Stance: heal 30 damage from this Pokémon, then switch it with 1 of your Benched Pokémon.</summary>
        private sealed class DefensiveStanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Defensive Stance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 30);
                if (ctx.IsActive(me) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Pump-up Smash: attach 2 basic Energy cards from your hand to your Benched Pokémon in any way you like.</summary>
        private sealed class PumpUpSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pump-up Smash") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 2, (card, mon) => !ctx.IsActive(mon), "Pump-up Smash: attach a basic Energy card from your hand to 1 of your Benched Pokémon");
                return true;
            }
        }

        /// <summary>Smogscreen: the Defending Pokémon is now Poisoned, and if it tries to attack during the opponent's next turn, they flip a coin; tails, that attack does nothing.</summary>
        private sealed class SmogscreenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Smogscreen") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.AddEffect(new CardsMEG.SandAttackEffect(defending.Name + " must flip a coin to attack during its next turn (Smogscreen)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Wreak Havoc: flip a coin until you get tails; discard the top card of the opponent's deck for each heads.</summary>
        private sealed class WreakHavocMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreak Havoc") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = 0;
                while (heads < 100 && ctx.Flip()) heads++;
                if (heads > 0 && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, Math.Min(heads, ctx.Opp.Deck.Count));
                return true;
            }
        }

        /// <summary>Toppling Wind: evolving from the hand, you may discard the top 3 cards of the opponent's deck. Giga Horn: 2 coins; both tails, nothing.</summary>
        private sealed class AggronTopplingWind : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Toppling Wind", ctx.Opp.Deck.Count > 0, "Use Toppling Wind? Discard the top 3 cards of your opponent's deck.")) return;
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, Math.Min(3, ctx.Opp.Deck.Count));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giga Horn") return false;
                if (ctx.FlipCoins(2) == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Triple Laser: 30 damage to each of 3 of the opponent's Pokémon (all of them if fewer; no W/R for Benched Pokémon).</summary>
        private sealed class TripleLaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Laser") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var picks = all.Count <= 3 ? all : ctx.ChoosePokemonMany(all, 3, 3, "Triple Laser: choose 3 of your opponent's Pokémon (30 damage to each)");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Magnetic Lines: you may move an Energy attached to the Defending Pokémon to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class MagneticLinesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Lines") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Magnetic Lines: move an Energy from the Defending Pokémon to 1 of your opponent's Benched Pokémon?")) return true;
                var cards = defending.Energy.ToList();
                var card = ctx.ChooseCards(cards, 1, 1, "Magnetic Lines: move which Energy?")[0];
                var bench = ctx.Opp.Bench.ToList();
                var to = ctx.ChoosePokemon(bench, "Magnetic Lines: move it to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>Pull Out: put a card from your discard pile on top of your deck.</summary>
        private sealed class PullOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pull Out") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var discard = ctx.Me.Discard.ToList();
                if (discard.Count == 0) return true;
                var card = ctx.ChooseCards(discard, 1, 1, "Pull Out: put which card from your discard pile on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(card)) return true;
                ctx.Me.Deck.Insert(0, card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + card.Def.Name + " from " + ctx.Engine.Their(ctx.Player) + " discard pile on top of " + ctx.Engine.Their(ctx.Player) + " deck (Pull Out).");
                return true;
            }
        }

        /// <summary>Consume: heal from this Pokémon as much damage as the attack did to the Defending Pokémon.</summary>
        private sealed class ConsumeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Consume") return false;
                int dealt = ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (dealt > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, dealt);
                return true;
            }
        }

        /// <summary>"Both this Pokémon and the Defending Pokémon are now [condition]" (Bang Heads, Tumbling Tackle).</summary>
        internal sealed class BothConditionMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public BothConditionMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, condition);
                return true;
            }
        }

        /// <summary>Wake-up Beam: remove all Special Conditions from the Defending Pokémon.</summary>
        private sealed class WakeUpBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wake-up Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var c in AllConditions)
                    if (defending.Has(c)) ctx.Engine.RemoveCondition(defending, c);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Giant Cape: the Pokémon this card is attached to gets +20 HP.</summary>
        private sealed class GiantCape : CardScript
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon == src.Pokemon ? hp + 20 : hp;
            }
        }

        /// <summary>Rescue Scarf: Knocked Out by damage from an opponent's attack, the Pokémon goes to your hand (its attached cards are discarded).</summary>
        private sealed class RescueScarf : CardScript
        {
            public override bool GoesToHandWhenKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                return byOpponentsAttack && mon != null && mon == src.Pokemon;
            }
        }
    }
}
