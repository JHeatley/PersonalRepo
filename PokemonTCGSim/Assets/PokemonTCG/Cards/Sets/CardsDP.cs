using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using static PokemonTCG.Cards.CardsDPEra;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Diamond &amp; Pearl (DP, 2007; v172): the cards not already in the pool (the others are prints; the LV.X cards are the DP
    /// Promos'). Unlimited only. Compendium rulings used: "Trainer" means Trainer cards that aren't Supporters or Stadiums; Revenge
    /// counts a Knock Out whatever Machamp was then; Giga Drain heals the full damage done; Gravity Wave hits Pokémon whose Retreat
    /// Cost is 0 for any reason; Harden checks the damage after W/R; Life Drain places counters (no damage); Metronome copies an
    /// attack as Clefable; Flame Dance works with no Fire Energy in the discard pile; Unown C's CALL finds any Unown; Intense Voltage
    /// moves any Lightning Energy; Professor Rowan always shuffles; Fake Tears does nothing on tails.
    /// </summary>
    public static partial class CardsDP
    {
        public static void Register(CardScriptRegistry r)
        {
            // Rares (2-17)
            r.RegisterByKey("DP-2", new DusknoirDarkPalm());
            r.RegisterByKey("DP-3", new ElectivireIntenseVoltage());
            r.RegisterByKey("DP-4", new ComboMon(new CardsSSB.AnyOpponentTextMon("Ice Blade", 40), new FlipGateMon("Aqua Jet", new Cards30CC.BenchHitMon("Aqua Jet", 20))));
            r.RegisterByKey("DP-5", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Meteor Punch"), new DiscardAllTypeMon("Flare Blitz", EnergyType.Fire)));
            r.RegisterByKey("DP-6", new ComboMon(new CardsFLF1.NoResistanceMon("Feint"), new Cards30CC.BenchHitMon("Aura Sphere", 20)));
            r.RegisterByKey("DP-7", new LuxrayGleamEyes());
            r.RegisterByKey("DP-8", new MagnezoneMagnetize());
            r.RegisterByKey("DP-9", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new SwitchSelfMon("Aqua Ring", false)));
            r.RegisterByKey("DP-10", new ComboMon(new AvengeMon(), new CardsASR1.PlusMon("Psywave", 20, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("DP-12", new RhyperiorEarthFissure());
            r.RegisterByKey("DP-13", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new SpikeWhipMon()));
            r.RegisterByKey("DP-14", new ShiftryDarknessFan());
            r.RegisterByKey("DP-15", new ComboMon(new ChooseOpponentMon("Frustration", 30, m => m.DamageCounters == 0),
                                                  new FlipGateMon("Toxic Cloud", new CardsPK.PoisonCountersMon("Toxic Cloud", 2))));
            r.RegisterByKey("DP-16", new ComboMon(new AcceleratingDiveMon(), new CardsFST1.FlipRecoilMon("Brave Heart", 100)));
            r.RegisterByKey("DP-17", new ComboMon(new ParalyzeFlipMon("Body Slam"), new HealTypeMon("Leaf Storm", EnergyType.Grass, 20)));

            // Uncommons (18-64)
            r.RegisterByKey("DP-18", new ComboMon(new CardsPK.DefenseCurlMon(), new BubblePumpMon()));
            r.RegisterByKey("DP-19", new ComboMon(new CardsDRI1.WhirlwindMon("Whirlwind"), new DrainMon("Giga Drain", 0)));
            r.RegisterByKey("DP-20", new ComboMon(new RestMon("Rest", 40), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("DP-21", new ComboMon(new SwallowUpMon(), new WringOutMon()));
            r.RegisterByKey("DP-22", new ComboMon(new CoinsMon("Double Slap", 2), new MetronomeMon()));
            r.RegisterByKey("DP-23", new ComboMon(new FlipBonusMon("Stomp", 20), new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned)));
            r.RegisterByKey("DP-25", new ComboMon(new ParalyzeFlipMon("Stun Spore"), new CardsPK.PoisonCountersMon("Toxic Dust", 2)));
            r.RegisterByKey("DP-26", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Screw Tail"), new CardsP3.ExtraWater20Mon("Water Gun")));
            r.RegisterByKey("DP-27", new ComboMon(new CardsCEC2.LifeDrainMon(), new ShadowDanceMon()));
            r.RegisterByKey("DP-28", new ComboMon(new CardsDRI1.WhirlwindMon("Pitch"), new FlipBonusMon("Horn Slash", 30)));
            r.RegisterByKey("DP-29", new ComboMon(new NoRetreatMon("Vacuum Up"), new SandEjectMon()));
            r.RegisterByKey("DP-30", new ComboMon(new HealingWishMon(), new FlopMon()));
            r.RegisterByKey("DP-31", new ComboMon(new CardsFST2.ConditionBonusMon("Revenge", 50, c => c.WasKnockedOutByAttackLastOpponentTurn()), new DynamicPunchMon()));
            r.RegisterByKey("DP-32", new ComboMon(new ForcePalmMon(), new RecoilMon("Spinning Kick", 20)));
            r.RegisterByKey("DP-33", new CardsSS.BabyEvolutionMon("Snorlax", new HeaveMon()));
            r.RegisterByKey("DP-34", new ComboMon(new SeeBeyondMon(), new CardsFST2.ConditionBonusMon("Extrasensory", 50, c => c.Me.Hand.Count == c.Opp.Hand.Count)));
            r.RegisterByKey("DP-36", new ComboMon(new CardsHPB.KnockOffMon("High Hat", false), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("DP-37", new ComboMon(new NoRetreatMon("Block"), new EaseUpMon()));
            r.RegisterByKey("DP-38", new ComboMon(new CoinsMon("Slam", 2), new LockAfterAttackMon("Metal Slash", false)));
            r.RegisterByKey("DP-39", new ComboMon(new LeafHoneyMon(), new CardsASR1.TimesMon("Attack Order", c => c.MyPokemon().Concat(c.OpponentPokemon()).Count(m => c.Engine.IsType(m, EnergyType.Grass)))));
            r.RegisterByKey("DP-40", new ComboMon(new SearchAttackMon("Nasty Plot", d => true, 1),
                                                  new CardsASR1.TimesMon("Dark Pulse", c => c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Darkness)))));
            r.RegisterByKey("DP-41", new CounterchargeMon());
            r.RegisterByKey("DP-42", new CardsSS.BabyEvolutionMon("Wobbuffet", new AstonishMon("Astonish")));
            r.RegisterByKey("DP-43", new CardsSS.BabyEvolutionMon("Roselia", new SearchAttackMon("Errand-Running", DpTrainer, 1)));
            r.RegisterByKey("DP-44", new ComboMon(new HardenMon("Harden", 30), new NoRetreatMon("Gooey Thread")));
            r.RegisterByKey("DP-45", new ComboMon(new CardsPAR2.FlipConditionMon("Worry Seed", SpecialCondition.Confused), new MagicalLeafMon()));
            r.RegisterByKey("DP-46", new ComboMon(new BlowingWindMon(), new ConditionNoRetreatMon("Ominous Wind", SpecialCondition.Confused, true)));
            r.RegisterByKey("DP-47", new ComboMon(new RoamMon(), new GravityWaveMon()));
            r.RegisterByKey("DP-48", new CardsSS.BabyEvolutionMon("Electabuzz", new SearchAttackMon("Outlet", d => CardsRS.BasicOf(d, EnergyType.Lightning), 1)));
            r.RegisterByKey("DP-49", new SearchAttachMon("Synthesis", d => CardsRS.BasicOf(d, EnergyType.Grass)));
            r.RegisterByKey("DP-50", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new CardsPK.DreamEaterMon()));
            r.RegisterByKey("DP-51", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new CardsMEG.SandAttackMon("Sand Attack")));
            r.RegisterByKey("DP-52", new ComboMon(new FlipBonusMon("Fasten Claws", 30), new ParalyzeFlipMon("Thunder Fang")));
            r.RegisterByKey("DP-53", new CardsBS1.KarateChopMon());
            r.RegisterByKey("DP-54", new ComboMon(new CoinsMon("Tri Attack", 3), new MagneticRayMon()));
            r.RegisterByKey("DP-55", new CardsSS.BabyEvolutionMon("Mantine", new SearchAttackMon("Call for Friends", d => d.IsBasicPokemon && d.HasType(EnergyType.Water), 1)));
            r.RegisterByKey("DP-56", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Flame Dance", EnergyType.Fire), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("DP-57", new ComboMon(new DirtyTrickMon(), new TailsNothingMon("Razor Wind")));
            r.RegisterByKey("DP-58", new ComboMon(new CardsLTR1.SpreadMon("Aqua Shower", 10), new ChooseOpponentMon("Brine", 40, m => m.DamageCounters > 0)));
            r.RegisterByKey("DP-59", new ComboMon(new Cards30CC.BenchHitMon("Overrun", 10), new BlazeUpMon()));
            r.RegisterByKey("DP-60", new ComboMon(new StadiumBonusDiscardMon("Storm Up", 20), new CrashImpactMon()));
            r.RegisterByKey("DP-61", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("DP-62", new CardsASR1.TimesMon("Flail", CardsCG.SelfCounters));
            r.RegisterByKey("DP-63", new ComboMon(new HardenMon("Harden", 30), new CardsLOR1.EntanglingStringMon("Entangling String")));
            r.RegisterByKey("DP-64", new ComboMon(new CardsDRI1.WhirlwindMon("Whirlwind"), new NoRetreatMon("Clutch")));
            r.RegisterByKey("DP-65", new UnownAnalyze());
            r.RegisterByKey("DP-66", new UnownBounce());
            r.RegisterByKey("DP-67", new UnownCall());
            r.RegisterByKey("DP-68", new UnownDraw());

            // Commons (69-104)
            r.RegisterByKey("DP-69", new CardsSS.BabyEvolutionMon("Marill", new CardsCG.DiscardCardToHandMon("Delivery")));
            r.RegisterByKey("DP-70", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new TrainerFromDiscardMon("Scavenge")));
            r.RegisterByKey("DP-71", new CardsSS.BabyEvolutionMon("Sudowoodo", new FakeTearsMon()));
            r.RegisterByKey("DP-75", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new DrainMon("Leech Seed", 10)));
            r.RegisterByKey("DP-77", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new CardsFST2.ConditionBonusMon("Moon Impact", 20, c => EvolvedFrom(c, "Cleffa"))));
            r.RegisterByKey("DP-78", new CardsSS.BabyEvolutionMon("Clefairy", new DrawAttackMon("Eeek", 1)));
            r.RegisterByKey("DP-79", new SolicitingNectarMon());
            r.RegisterByKey("DP-80", new ComboMon(new FlipGateMon("Disable", new CardsTEU1.AmnesiaMon("Disable")), new Cards30CC.BenchHitMon("Night Shade", 10)));
            r.RegisterByKey("DP-81", new ComboMon(new ThunderpunchMon(), new ShockWaveMon()));
            r.RegisterByKey("DP-82", new PainPaybackMon());
            r.RegisterByKey("DP-84", new RecoilMon("Take Down", 10));
            r.RegisterByKey("DP-85", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new TailsNothingMon("Kick Shot")));
            r.RegisterByKey("DP-86", new PlainMon());
            r.RegisterByKey("DP-87", new ComboMon(new CardsPAR2.FlipConditionMon("Metal Sound", SpecialCondition.Confused),
                                                  new CardsFST2.ConditionBonusMon("Resonance", 20, c => c.Defending != null && c.Defending.Has(SpecialCondition.Confused))));
            r.RegisterByKey("DP-88", new ComboMon(new CardsFST2.ConditionBonusMon("Rollout", 20, c => EvolvedFrom(c, "Azurill")), new SwitchSelfMon("Splashing Turn", false)));
            r.RegisterByKey("DP-89", new ComboMon(new DodgeMon("Detect", false), new CardsASR1.PlusMon("Meditate", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters)));
            r.RegisterByKey("DP-91", new ComboMon(new AstonishMon("Astonish"), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("DP-92", new CardsRGB.TunnelingMon());
            r.RegisterByKey("DP-94", new PlainMon());
            r.RegisterByKey("DP-95", new RecoilMon("Take Down", 10));
            r.RegisterByKey("DP-96", new ComboMon(new ToxicSpikesMon(), new SweetSpikeMon()));
            r.RegisterByKey("DP-97", new BideMon("Bide"));
            r.RegisterByKey("DP-99", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("DP-100", new ComboMon(new CoinsMon("Double Slap", 2), new CardsCES1.FeintAttackMon("Feint Attack", 20)));
            r.RegisterByKey("DP-101", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("DP-102", new ConditionAttackMon("Severe Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("DP-104", new ComboMon(new FlipBonusMon("Needling Sting", 10), new ParalyzeFlipMon("String Shot")));

            // Trainers
            r.RegisterByKey("DP-108", new NightPokemonCenter());
            r.RegisterByKey("DP-109", new PlusPowerDP());
            r.RegisterByKey("DP-111", new PokedexHandy());
            r.RegisterByKey("DP-112", new ProfessorRowan());
            r.RegisterByKey("DP-113", new Rival());
            r.RegisterByKey("DP-114", new SpeedStadium());
        }

        // ================================================================== Shared (used by the later DP sets too)

        /// <summary>"Discard all [T] Energy attached to this Pokémon" after the damage (Flare Blitz).</summary>
        internal sealed class DiscardAllTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public DiscardAllTypeMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    foreach (var card in me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, type)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                return true;
            }
        }

        /// <summary>"Remove N damage counters from each of your [type] Pokémon" (Leaf Storm).</summary>
        internal sealed class HealTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int amount;

            public HealTypeMon(string attack, EnergyType type, int amount)
            {
                this.attack = attack;
                this.type = type;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, type))) ctx.Heal(mon, amount);
                return true;
            }
        }

        /// <summary>Rest: remove all Special Conditions and N damage from this Pokémon; it's now Asleep.</summary>
        internal sealed class RestMon : Mon
        {
            private readonly string attack;
            private readonly int heal;

            public RestMon(string attack, int heal)
            {
                this.attack = attack;
                this.heal = heal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, heal);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Metronome: copy 1 of the Defending Pokémon's attacks without its Energy cost (Compendium: as this Pokémon).</summary>
        internal sealed class MetronomeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metronome") return false;
                CardDef source;
                int index;
                if (ctx.Defending == null || !ChooseCopiedAttack(ctx, ctx.Defending, "Metronome: copy which of the Defending Pokémon's attacks?", out source, out index)) return true;
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        // ================================================================== Rares

        /// <summary>
        /// Dark Palm (Poké-Power): once during your turn, if your opponent has 4 or more Benched Pokémon, shuffle 1 of them and its
        /// cards into their deck. Not with a Special Condition. Hard Feelings: 5 damage counters on the Defending Pokémon, plus 1 for
        /// each Prize card your opponent has taken.
        /// </summary>
        private sealed class DusknoirDarkPalm : Mon
        {
            protected override string Activated { get { return "Dark Palm"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.Opp.Bench.Count >= 4; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = ctx.Opp.Bench.Where(ctx.CanAffect).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Dark Palm: shuffle which of your opponent's Benched Pokémon into their deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Feelings") return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.PlaceCounters(defending, 5 + ctx.Engine.PrizesTaken(ctx.Opponent));
                return true;
            }
        }

        /// <summary>
        /// Intense Voltage (Poké-Power): as often as you like during your turn, if Elekid is under Electivire, move a Lightning Energy
        /// from 1 of your other Pokémon to it (any Lightning Energy: Compendium). Not with a Special Condition. Giga Impact: you may
        /// discard all Lightning Energy from Electivire for a base damage of 120.
        /// </summary>
        private sealed class ElectivireIntenseVoltage : Mon
        {
            private const string Power = "Intense Voltage";

            private static List<PokemonInPlay> Donors(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Lightning)));
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && EvolvedFrom(ctx, "Elekid") && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => ctx.InPlay(me) && Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a Lightning Energy to " + me.Name + " from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var card = from.Energy.First(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Lightning));
                    return ctx.Engine.MoveEnergy(from, me, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giga Impact") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                bool big = lightning.Count > 0 && ctx.AskYesNo("Giga Impact: discard all Lightning Energy from " + me.Name + " for 120 damage instead of " + atk.Damage + "?");
                if (big) foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(big ? 120 : atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Gleam Eyes (Poké-Power): when you play Luxray from your hand to evolve, you may look at your opponent's hand; if their Bench
        /// isn't full, put a Basic Pokémon from it onto their Bench and switch it with the Defending Pokémon. Lightning Star: move all
        /// Lightning Energy from Luxray to 1 of your Benched Pokémon.
        /// </summary>
        private sealed class LuxrayGleamEyes : Mon
        {
            private const string Power = "Gleam Eyes";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Opp.Hand.Count == 0 || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Look at your opponent's hand; a Basic Pokémon there goes onto their Bench and becomes their Active Pokémon.")) return;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", Power + ": your opponent's hand", ctx.Source);
                var basics = hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return;
                var pick = ctx.ChooseCards(basics, 1, 1, Power + ": put which Basic Pokémon onto your opponent's Bench?")[0];
                if (!ctx.Opp.Hand.Remove(pick)) return;
                var benched = ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
                if (benched != null && ctx.Opp.Active != null) ctx.Engine.SwitchActive(ctx.Opponent, benched);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Star") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                var lightning = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return true;
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Lightning Star: move all Lightning Energy to which of your Benched Pokémon?");
                if (to != null) foreach (var card in lightning) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Magnetize (Poké-Body): your Active Pokémon with any Metal Energy attached has no Retreat Cost. Metal Blast: 10 more for each Metal Energy on Magnezone.</summary>
        private sealed class MagnezoneMagnetize : Mon
        {
            private static readonly Mon Blast = new CardsASR1.PlusMon("Metal Blast", 10, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Metal));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.IsActive(mon)) return cost;
                return src.Engine.EnergyCountOfType(mon, EnergyType.Metal) > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Avenge: as many damage counters as you have damaged Pokémon, on the Defending Pokémon.</summary>
        private sealed class AvengeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Avenge") return false;
                int n = ctx.MyPokemon(m => m.DamageCounters > 0).Count;
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Earth Fissure (Poké-Power): when you play Rhyperior from your hand to evolve, you may discard the top 3 cards of your opponent's deck. Rock Wrecker: no W/R; can't attack next turn.</summary>
        private sealed class RhyperiorEarthFissure : Mon
        {
            private const string Power = "Earth Fissure";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Opp.Deck.Count == 0 || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Discard the top 3 cards of your opponent's deck.")) return;
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Wrecker") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Spike Whip: if Budew is under Roserade, 30 to 1 of the opponent's Benched Pokémon too.</summary>
        private sealed class SpikeWhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Whip") return false;
                bool budew = EvolvedFrom(ctx, "Budew");
                ctx.DamageDefending(atk.Damage);
                if (budew) ctx.DamageOneBenched(30);
                return true;
            }
        }

        /// <summary>
        /// Darkness Fan (Poké-Power): once during your turn, if Shiftry is your Active Pokémon, flip a coin; heads, the highest Stage
        /// card of 1 of your opponent's Benched Evolved Pokémon goes back to their hand. Not with a Special Condition. Spirit Dance.
        /// </summary>
        private sealed class ShiftryDarknessFan : Mon
        {
            private static readonly Mon Dance = new SpiritDanceMon();

            protected override string Activated { get { return "Darkness Fan"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && CardsDS.PowerReady(ctx) && ctx.Opp.Bench.Any(CardsE2.IsEvolved);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var targets = ctx.Opp.Bench.Where(m => CardsE2.IsEvolved(m) && ctx.CanAffect(m)).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Darkness Fan: return the top Evolution card of which Benched Pokémon to your opponent's hand?");
                if (target != null) ctx.Engine.Devolve(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dance.HandleAttack(ctx, atk); }
        }

        private sealed class SpiritDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Dance") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>Accelerative Dive: flip; tails, nothing; heads, damage and no damage to Staraptor from attacks (anyone's) until the end of your next turn.</summary>
        private sealed class AcceleratingDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Accelerative Dive") return false;
                var me = ctx.Pokemon;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks until the end of " + ctx.Engine.Possessive(ctx.Player) + " next turn (Accelerative Dive)", ctx.Player, me.Uid, ctx.Turn + 2));
                return true;
            }
        }

        // ================================================================== Uncommons

        /// <summary>Bubble Pump: 20 more with 3 or more Energy; if Azurill is under Azumarill, flip a coin, heads Paralyzed.</summary>
        private sealed class BubblePumpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Pump") return false;
                var defending = ctx.Defending;
                bool azurill = EvolvedFrom(ctx, "Azurill");
                ctx.DamageDefending(atk.Damage + (ctx.EnergyCount(ctx.Pokemon) >= 3 ? 20 : 0));
                if (azurill && ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Swallow Up: if the Defending Pokémon has less HP left than Carnivine, this attack does 60 instead.</summary>
        private sealed class SwallowUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swallow Up") return false;
                var d = ctx.Defending;
                bool weaker = d != null && ctx.RemainingHp(d) < ctx.RemainingHp(ctx.Pokemon);
                ctx.DamageDefending(weaker ? 60 : atk.Damage);
                return true;
            }
        }

        /// <summary>Wring Out: flip a coin; heads, the Defending Pokémon is Paralyzed and loses an Energy card.</summary>
        private sealed class WringOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wring Out") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Wring Out: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Shadow Dance: 4 damage counters on the opponent's Pokémon in any way you like, then switch Gengar with 1 of your Benched Pokémon.</summary>
        private sealed class ShadowDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Dance") return false;
                ctx.PlaceCountersAnyWay(4, () => ctx.OpponentPokemon());
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Sand Eject: during your next turn, an attack that damages the Defending Pokémon (after W/R) does 40 more.</summary>
        private sealed class SandEjectMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Eject") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new SandEjectEffect(defending.Name + " takes 40 more damage from attacks during " + ctx.Engine.Possessive(ctx.Player) + " next turn (Sand Eject)", ctx.Player, defending.Uid, ctx.Turn));
                return true;
            }
        }

        private sealed class SandEjectEffect : ContinuousEffect
        {
            public SandEjectEffect(string name, int owner, int targetUid, int turn) : base(name, owner, targetUid, turn + 2, true) { ActiveFromTurn = turn + 2; }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || info.Attacker.Owner != Owner || damage <= 0) return damage;
                return engine.CurrentSourceKind == EffectSourceKind.Attack ? damage + 40 : damage;
            }
        }

        /// <summary>Healing Wish: flip until tails; remove that many damage counters from 1 of your Pokémon.</summary>
        private sealed class HealingWishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Wish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                var damaged = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (heads == 0 || damaged.Count == 0) return true;
                var target = ctx.ChoosePokemon(damaged, "Healing Wish: remove " + heads + " damage counters from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 10 * heads);
                return true;
            }
        }

        /// <summary>Flop: 20 to 1 of the opponent's Benched Pokémon; you may switch Lopunny with 1 of your Benched Pokémon.</summary>
        private sealed class FlopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flop") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(20);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Flop: switch " + ctx.Pokemon.Name + " with 1 of your Benched Pokémon?")) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Dynamic Punch: flip a coin; heads, 30 more and the Defending Pokémon is now Confused.</summary>
        private sealed class DynamicPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Punch") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Force Palm: as many damage counters as Medicham has, on 1 of the opponent's Pokémon.</summary>
        private sealed class ForcePalmMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Force Palm") return false;
                int n = ctx.Pokemon.DamageCounters;
                var all = ctx.OpponentPokemon();
                if (n == 0 || all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Force Palm: put " + n + " damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, n);
                return true;
            }
        }

        /// <summary>Heave: discard 2 cards from your hand (or the attack does nothing); 2 coins, 30 for each heads.</summary>
        private sealed class HeaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heave") return false;
                if (ctx.Me.Hand.Count < 2) return Nothing(ctx);
                foreach (var card in ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Heave: discard 2 cards from your hand")) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(2));
                return true;
            }
        }

        /// <summary>
        /// See Beyond: put a card from your hand face up as a Prize card, then put 1 of your face-down Prize cards (random: they can't
        /// be told apart) into your hand. Does nothing if all your Prize cards are face up.
        /// </summary>
        private sealed class SeeBeyondMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "See Beyond") return false;
                var me = ctx.Me;
                var faceDown = me.Prizes.Where(c => !me.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count == 0) return Nothing(ctx);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (me.Hand.Count > 0)
                {
                    var card = ctx.ChooseCards(me.Hand.ToList(), 1, 1, "See Beyond: put which card from your hand face up as a Prize card?")[0];
                    if (me.Hand.Remove(card))
                    {
                        me.Prizes.Add(card);
                        me.FaceUpPrizeIds.Add(card.Id);
                        ctx.Log("See Beyond: " + card.Def.Name + " is now a face-up Prize card.");
                    }
                }
                var taken = faceDown[ctx.State.Rng.Next(faceDown.Count)];
                if (me.Prizes.Remove(taken)) me.Hand.Add(taken);
                ctx.Log("See Beyond: " + ctx.Engine.PlayerName(ctx.Player) + " put a face-down Prize card into " + ctx.Engine.Their(ctx.Player) + " hand.");
                return true;
            }
        }

        /// <summary>Ease Up: Snorlax is now Asleep; 30 more if it evolved from Munchlax.</summary>
        private sealed class EaseUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ease Up") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + (EvolvedFrom(ctx, "Munchlax") ? 30 : 0));
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Leaf Honey: discard a Grass Energy from Vespiquen and remove all damage counters from 1 of your Benched Grass Pokémon.</summary>
        private sealed class LeafHoneyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Honey") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var grass = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Grass)).ToList();
                if (grass.Count == 0 || !ctx.Engine.DiscardEnergy(me, grass[0])) return true;
                var targets = ctx.Me.Bench.Where(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Grass)).ToList();
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Leaf Honey: remove all damage counters from which of your Benched Grass Pokémon?");
                if (target != null) ctx.Heal(target, 10 * target.DamageCounters);
                return true;
            }
        }

        /// <summary>Countercharge: flip a coin; heads, move all damage counters from Wobbuffet to the Defending Pokémon.</summary>
        private sealed class CounterchargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Countercharge") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (me.DamageCounters == 0 || defending == null || !ctx.Flip()) return true;
                ctx.Engine.MoveDamageCounters(me, defending, me.DamageCounters, "Countercharge");
                return true;
            }
        }

        /// <summary>Magical Leaf: flip a coin; heads, 20 more and remove 3 damage counters from Cherrim.</summary>
        private sealed class MagicalLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Leaf") return false;
                var me = ctx.Pokemon;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30);
                return true;
            }
        }

        /// <summary>Blowing Wind: flip a coin; heads, shuffle 1 of your Benched Pokémon and its cards into your deck.</summary>
        private sealed class BlowingWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blowing Wind") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || !ctx.Flip()) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Blowing Wind: shuffle which of your Benched Pokémon into your deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Roam: flip a coin; heads, 2 damage counters on each of the opponent's Pokémon; tails, 2 on 1 of yours.</summary>
        private sealed class RoamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roam") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                {
                    foreach (var mon in ctx.OpponentPokemon()) if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 2);
                    return true;
                }
                var mine = ctx.ChoosePokemon(ctx.MyPokemon(), "Roam: put 2 damage counters on which of your Pokémon?");
                if (mine != null) ctx.PlaceCounters(mine, 2);
                return true;
            }
        }

        /// <summary>Gravity Wave: 10 to each of the opponent's Benched Pokémon with no Retreat Cost (for any reason: Compendium).</summary>
        private sealed class GravityWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gravity Wave") return false;
                var targets = ctx.Opp.Bench.Where(m => ctx.Engine.RetreatCostOf(m) == 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in targets) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Magnetic Ray: before the damage, you may switch in 1 of the opponent's Benched Pokémon that has Energy attached.</summary>
        private sealed class MagneticRayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Ray") return false;
                var targets = ctx.GustTargets().Where(m => m.Energy.Count > 0).ToList();
                if (targets.Count > 0 && ctx.AskYesNo("Magnetic Ray: switch in 1 of your opponent's Benched Pokémon that has Energy attached first?"))
                {
                    var target = ctx.ChoosePokemon(targets, "Magnetic Ray: switch in which Benched Pokémon?");
                    if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Dirty Trick: flip a coin; heads, discard an Energy card from 1 of the opponent's Pokémon.</summary>
        private sealed class DirtyTrickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Trick") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0 || !ctx.Flip()) return true;
                var target = ctx.ChoosePokemon(targets, "Dirty Trick: discard an Energy card from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Dirty Trick: discard an Energy card from " + target.Name);
                return true;
            }
        }

        /// <summary>Blaze Up: flip a coin; tails, discard a Fire Energy from Rapidash and 10 to each of the opponent's Benched Pokémon.</summary>
        private sealed class BlazeUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze Up") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) return true;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "Blaze Up: discard a Fire Energy from " + me.Name, e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                foreach (var mon in ctx.Opp.Bench.ToList()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Crash Impact: 10 damage to Rhydon; your opponent switches the Defending Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class CrashImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crash Impact") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageSelf(10);
                ctx.OpponentSwitchesActive();
                return true;
            }
        }

        private sealed class UnownAnalyze : BenchUnownPower
        {
            protected override string Power { get { return "ANALYZE"; } }
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Use(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                CardsCRZ1.PutOnTopInOrder(ctx, top, "ANALYZE");
            }
        }

        private sealed class UnownBounce : BenchUnownPower
        {
            protected override string Power { get { return "BOUNCE"; } }

            protected override void Use(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var unown = ctx.MyPokemon(m => m.Name.StartsWith("Unown", StringComparison.Ordinal));
                if (unown.Count == 0) return;
                var target = ctx.ChoosePokemon(unown, "BOUNCE: discard the cards attached to which Unown and shuffle it into your deck?");
                if (target == null) return;
                foreach (var card in target.Energy.ToList()) ctx.Engine.DiscardEnergy(target, card);
                ctx.Engine.DiscardAllTools(target);
                ctx.Engine.ShufflePokemonIntoDeck(target);
            }
        }

        private sealed class UnownCall : BenchUnownPower
        {
            protected override string Power { get { return "CALL"; } }
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0; }

            protected override void Use(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.SearchToBench(d => d.IsBasicPokemon && d.Name.StartsWith("Unown", StringComparison.Ordinal), 1, "CALL: put an Unown onto your Bench");
            }
        }

        private sealed class UnownDraw : BenchUnownPower
        {
            protected override string Power { get { return "DRAW"; } }

            protected override void Use(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("DRAW: draw a card?")) ctx.Draw(1);
                if (ctx.Opp.Deck.Count > 0 && ctx.Engine.AskYesNo(ctx.Opponent, "DRAW: draw a card?", ctx.Source)) ctx.Engine.DrawCards(ctx.Opponent, 1);
            }
        }

        // ================================================================== Commons

        /// <summary>Fake Tears: flip a coin; heads, your opponent can't play Trainer cards next turn and attack damage to this Pokémon is 30 less (tails: nothing at all).</summary>
        private sealed class FakeTearsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fake Tears") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new DpTrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Fake Tears)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(30);
                return true;
            }
        }

        /// <summary>Soliciting Nectar: switch 1 of the opponent's Benched Pokémon with the Defending Pokémon; the new Defending Pokémon is now Asleep.</summary>
        private sealed class SolicitingNectarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Soliciting Nectar") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var target = ctx.GustOpponent("Soliciting Nectar: choose 1 of your opponent's Benched Pokémon to switch in");
                if (target != null && ctx.InPlay(target)) ctx.ApplyCondition(target, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Thunderpunch: flip a coin; heads, 10 more; tails, 10 damage to Electabuzz.</summary>
        private sealed class ThunderpunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderpunch") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (!heads) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Shock Wave: flip a coin, heads Paralyzed; if Electabuzz evolved from Elekid, 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class ShockWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shock Wave") return false;
                var defending = ctx.Defending;
                bool elekid = EvolvedFrom(ctx, "Elekid");
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (elekid) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Pain Payback: move 1 damage counter from Gastly to 1 of the opponent's Pokémon.</summary>
        private sealed class PainPaybackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pain Payback") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (me.DamageCounters == 0) return true;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Pain Payback: move a damage counter to which of your opponent's Pokémon?");
                if (target != null) ctx.Engine.MoveDamageCounters(me, target, 1, "Pain Payback");
                return true;
            }
        }

        /// <summary>Toxic Spikes: the Defending Pokémon is now Poisoned; 10 damage if Roselia evolved from Budew.</summary>
        private sealed class ToxicSpikesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toxic Spikes") return false;
                var defending = ctx.Defending;
                if (EvolvedFrom(ctx, "Budew")) ctx.DamageDefending(10);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Sweet Spike: the Defending Pokémon is now Asleep; remove 2 damage counters from 1 of your Pokémon.</summary>
        private sealed class SweetSpikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Spike") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                var damaged = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (damaged.Count == 0) return true;
                var target = ctx.ChoosePokemon(damaged, "Sweet Spike: remove 2 damage counters from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Night Pokémon Center: choose 1 of your Pokémon; 2 heads, remove all its damage counters; 2 tails, discard all its Energy cards.</summary>
        private sealed class NightPokemonCenter : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Night Pokémon Center: choose 1 of your Pokémon");
                if (target == null) return;
                int heads = ctx.FlipCoins(2);
                if (heads == 2 && target.DamageCounters > 0) ctx.Heal(target, 10 * target.DamageCounters);
                else if (heads == 0) foreach (var card in target.Energy.ToList()) ctx.Engine.DiscardEnergy(target, card);
            }
        }

        /// <summary>PlusPower (Diamond &amp; Pearl): this turn, attacks by the Pokémon you choose do 10 more damage to the Active Pokémon (before W/R).</summary>
        private sealed class PlusPowerDP : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "PlusPower: attach it to which of your Pokémon?");
                if (target != null) ctx.AddEffect(new PlusPowerOnEffect(target.Name + "'s attacks do 10 more damage this turn (PlusPower)", ctx.Player, target.Uid, ctx.Turn));
            }
        }

        private sealed class PlusPowerOnEffect : ContinuousEffect
        {
            public PlusPowerOnEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target == null || target.Owner == Owner || !engine.State.IsActive(target) || damage <= 0) return damage;
                return damage + 10;
            }
        }

        /// <summary>Pokédex HANDY910is: look at the top 2 cards of your deck, put 1 into your hand and the other on the bottom of your deck.</summary>
        private sealed class PokedexHandy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var keep = top.Count == 1 ? top[0] : ctx.ChooseCards(top, 1, 1, "Pokédex HANDY910is: put which card into your hand? (The other goes on the bottom of your deck.)")[0];
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                ctx.Me.Hand.Add(keep);
                foreach (var card in top.Where(c => c != keep)) ctx.Me.Deck.Add(card);
            }
        }

        /// <summary>Professor Rowan (Supporter): keep 1 card from your hand, shuffle the rest into your deck (always shuffle: Compendium), draw 4.</summary>
        private sealed class ProfessorRowan : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = ctx.Me.Hand.Where(c => c != ctx.Card).ToList();
                if (others.Count == 0) return;
                var keep = ctx.ChooseCards(others, 1, 1, "Professor Rowan: keep which card? (The rest go into your deck.)")[0];
                foreach (var card in others.Where(c => c != keep))
                {
                    ctx.Me.Hand.Remove(card);
                    ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                ctx.Draw(4);
            }
        }

        /// <summary>Rival (Supporter): reveal the top 5 cards of your deck; your opponent chooses 3 for your hand; the other 2 go back and you shuffle.</summary>
        private sealed class Rival : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.Me.Deck.Take(Math.Min(5, ctx.Me.Deck.Count)).ToList();
                ctx.Engine.ShowCards(ctx.Opponent, top, "Rival", "Rival: the top cards of your opponent's deck", ctx.Source);
                int n = Math.Min(3, top.Count);
                var picks = top.Count <= 3 ? top : ctx.Engine.ChooseCards(ctx.Opponent, top, n, n, "Rival: choose 3 of these cards for your opponent's hand", ctx.Card.Def);
                foreach (var card in picks)
                {
                    ctx.Me.Deck.Remove(card);
                    ctx.Me.Hand.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Speed Stadium: once during each player's turn, they may flip until tails and draw a card for each heads.</summary>
        private sealed class SpeedStadium : CardScript
        {
            private const string Limit = "Speed Stadium";

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && ctx.Me.Deck.Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Limit);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(heads);
                return true;
            }
        }
    }
}
