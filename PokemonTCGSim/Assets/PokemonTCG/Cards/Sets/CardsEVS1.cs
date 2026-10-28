using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Evolving Skies (EVS, 2021), Pokémon 1-90: Grass, Fire, Water, Lightning, Psychic and Fighting. Darkness through Colorless,
    /// the full-art-only Pokémon (Flareon V, Vaporeon V, Jolteon V, Dragonite V), the gold Froslass, Inteleon and Cresselia, the
    /// Trainers and Treasure Energy are in CardsEVS2. Regulation mark E: Expanded only.
    /// Already in the pool through Crown Zenith, Brilliant Stars, Silver Tempest and Fusion Strike: Suicune V, Mareep, Flaaffy,
    /// Emolga, Espeon VMAX, Sylveon V, Sylveon VMAX, Umbreon V, Umbreon VMAX, Altaria, Rayquaza V, Rayquaza VMAX, Duraludon V,
    /// Duraludon VMAX, Eevee, Swablu, Gordie, Raihan and Rescue Carrier.
    /// Pokémon whose attacks only do their printed damage need no script (Petilil, Applin, Litleo, Psyduck, Tentacool, Lombre,
    /// Carvanha, Tympole, Bergmite, Chinchou, Drowzee, Cutiefly, Hippopotas, Roggenrola, Palpitoad, Zorua, Nickit, Bagon,
    /// Zweilous, Teddiursa, Ursaring).
    /// Your rulings (v58): Dream Ball puts any Pokémon onto the Bench (Evolution Pokémon as they are) and can be played whenever
    /// you take it as a face-down Prize card, on either player's turn; Bubble Pod takes any Water Pokémon; the Pokémon Zoroark's
    /// Phantom Transformation puts in its place counts as being in play since Zoroark was; Skiploom's Solar Evolution ignores
    /// the usual evolution timing.
    /// </summary>
    public static class CardsEVS1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("EVS-1", new PinsirEVS());
            r.RegisterByKey("EVS-2", new CardsPFL.FlipUntilTailsTimesMon("Continuous Spin"));
            r.RegisterByKey("EVS-3", new SkiploomEVS());
            r.RegisterByKey("EVS-4", new JumpluffEVS());
            r.RegisterByKey("EVS-5", new AstonishMon("Astonish"));
            r.RegisterByKey("EVS-6", new Cards30C.RetaliateBonusMon("Rally Back", 90));
            r.RegisterByKey("EVS-7", new LeafeonV());
            r.RegisterByKey("EVS-8", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Grass Knot", 60, true), new CardsTWM1.HealSelfMon("Max Leaf", 30)));
            r.RegisterByKey("EVS-10", new DizzyingFlowerMon());
            r.RegisterByKey("EVS-11", new FlipBonusMon("Sharp Claws", 30));
            r.RegisterByKey("EVS-12", new FlipBonusMon("X-Scissor", 60));
            r.RegisterByKey("EVS-13", new ComboMon(new CardsTWM1.HealSelfMon("Absorb Life", 30), new CardsASC.RandomDiscardMon("Shadow Claw")));
            r.RegisterByKey("EVS-14", new CardsASR1.TimesMon("Missing in the Forest", SupportersInOpponentsDiscard));
            r.RegisterByKey("EVS-15", new CardsTWM1.HealSelfMon("Blot", 10));
            r.RegisterByKey("EVS-16", new EldegossEVS());
            // Fire
            r.RegisterByKey("EVS-18", new MaxDetonateMon());
            r.RegisterByKey("EVS-19", new ComboMon(new CardsASR1.TimesMon("Angry Fang", SingleStrikeBenchCounters), new RecoilMon("Heat Tackle", 30)));
            r.RegisterByKey("EVS-20", new SearchAttackMon("Victory Dive", AnyCard, 2));
            r.RegisterByKey("EVS-21", new ComboMon(new SurgingFlamesMon(), new CardsTWM1.SelfDiscardMon("Fire Blast", 1)));
            r.RegisterByKey("EVS-23", new CardsDRI2.CrunchMon("Rip Claw"));
            // Water
            r.RegisterByKey("EVS-25", new ConditionAttackMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("EVS-27", new CardsPOR.PoisonRingMon("Poisonous Prison"));
            r.RegisterByKey("EVS-28", new CardsASR1.TimesMon("Get Angry", OwnDamageCounters));
            r.RegisterByKey("EVS-29", new CardsDRI2.CrunchMon("Hyper Beam"));
            r.RegisterByKey("EVS-30", new ComboMon(new BubblePodMon(), new CardsPFL.DamagedBonusMon("Max Torrent", 100)));
            r.RegisterByKey("EVS-32", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("EVS-34", new LudicoloEVS());
            r.RegisterByKey("EVS-36", new ComboMon(new CardsPOR.FollowMeMon("Taunt"), new LockAfterAttackMon("Jet Bite", false)));
            r.RegisterByKey("EVS-37", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("EVS-38", new MiloticEVS());
            r.RegisterByKey("EVS-39", new SynchrodrawMon("Synchrodraw"));
            r.RegisterByKey("EVS-40", new ComboMon(new EvolveFromDeckAttackMon("Frozen Awakening"), new CardsPAF1.DiscardStadiumMon("Heavy Snow")));
            r.RegisterByKey("EVS-41", new GlaceonVmax());
            r.RegisterByKey("EVS-43", new ElementChainMon());
            r.RegisterByKey("EVS-45", new ShieldAttackMon("Frost Barrier", 30));
            r.RegisterByKey("EVS-46", new WishiwashiEVS());
            r.RegisterByKey("EVS-47", new CardsCRI.BasicShieldAttackMon("Blockface"));
            r.RegisterByKey("EVS-48", new ComboMon(new DefenderCantAttackMon("Ancient Freeze", IsVOrGx), new LockAfterAttackMon("Giga Impact", false)));
            // Lightning
            r.RegisterByKey("EVS-49", new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning));
            r.RegisterByKey("EVS-50", new ComboMon(new BigSparkingMon(), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
            r.RegisterByKey("EVS-51", new MaxThunderRumbleMon());
            r.RegisterByKey("EVS-53", new CardsMEG.SandAttackMon("Blinding Beam"));
            r.RegisterByKey("EVS-56", new ComboMon(new ParalyzeFlipMon("Thunder Shock"), new ElectronCrushMon()));
            r.RegisterByKey("EVS-58", new ComboMon(new PrimevalBeakMon("Primeval Beak"), new CardsBRS1.MillSelfMon("Mountain Swing", 3)));
            r.RegisterByKey("EVS-59", new SparkTrapMon());
            r.RegisterByKey("EVS-60", new TerasparkMon());
            // Psychic
            r.RegisterByKey("EVS-62", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new WakeUpSlapMon()));
            r.RegisterByKey("EVS-63", new ChargeFromHandMon("Cruel Charge", EnergyType.Psychic, new PsylaserMon()));
            r.RegisterByKey("EVS-64", new ZenShotMon());
            r.RegisterByKey("EVS-66", new MirrorPainMon());
            r.RegisterByKey("EVS-67", new ComboMon(new SearchAttackMon("Go and Collect", IsTrainerCard, 1), new NoRetreatMon("Corner")));
            r.RegisterByKey("EVS-68", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("EVS-69", new CardsFST2.ConditionBonusMon("Synchro Woofer", 80, SameHandSize));
            r.RegisterByKey("EVS-70", new CardsBCR2.DevolutionPunchMon("Rewind Beam"));
            r.RegisterByKey("EVS-71", new ConditionAttackMon("Hypnotic Gaze", SpecialCondition.Asleep));
            r.RegisterByKey("EVS-72", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("EVS-73", new FlorgesEVS());
            r.RegisterByKey("EVS-76", new PumpkabooEVS());
            r.RegisterByKey("EVS-77", new PandemoniumMon());
            r.RegisterByKey("EVS-79", new TrickyStepsMon());
            r.RegisterByKey("EVS-80", new ComboMon(new SearchAttackMon("Rapid Hunt", IsRapidStrikeCard, 2), new ShadowFlickerMon()));
            // Fighting
            r.RegisterByKey("EVS-81", new ComboMon(new CardsPAL1.EvolutionBonusMon("Clean Hit", 50), new CardsFLF1.NoResistanceMon("Bullet Straight Punch")));
            r.RegisterByKey("EVS-82", new ChargeFromHandMon("Strong Legs Charge", EnergyType.Fighting, new ZapperKickMon()));
            r.RegisterByKey("EVS-83", new MedichamV());
            r.RegisterByKey("EVS-85", new CardsTWM1.SelfDiscardMon("Sand Press", 2));
            r.RegisterByKey("EVS-87", new DodgeMon("Protect", false));
            r.RegisterByKey("EVS-88", new ComboMon(new ShieldAttackMon("Guard Press", 50), new PressureShotMon()));
            r.RegisterByKey("EVS-90", new ShakyWaveMon());
        }

        // ================================================================== Filters and counters

        internal static bool AnyCard(CardDef d) { return true; }
        internal static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        internal static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }
        /// <summary>"Rapid Strike cards": Pokémon and Trainer cards with Rapid Strike on them.</summary>
        internal static bool IsRapidStrikeCard(CardDef d) { return d.IsRapidStrike || d.Name.IndexOf("Rapid Strike", StringComparison.Ordinal) >= 0; }
        /// <summary>
        /// "A [T] Energy card" outside play (deck, discard pile, hand): only basic Energy has a type there. Typed Special Energy
        /// (Heat Fire, Wash Water...) provides its type only while attached (Compendium, Star Portal; your ruling, v65).
        /// </summary>
        internal static bool IsEnergyOf(CardDef d, EnergyType type) { return d != null && d.IsBasicEnergy && d.EnergyProvides == type; }
        internal static bool IsVOrGx(CardDef d) { return d.IsPokemonV || d.IsGX; }
        internal static bool IsPokemonVCard(CardDef d) { return d.IsPokemonV; }
        internal static bool SameHandSize(EffectContext ctx) { return ctx.Me.Hand.Count == ctx.Opp.Hand.Count; }
        internal static bool WentSecondFirstTurn(EffectContext ctx) { return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn; }

        private static int SupportersInOpponentsDiscard(EffectContext ctx) { return ctx.Opp.Discard.Count(c => c.Def.IsSupporter); }
        private static int OwnDamageCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters; }

        private static int SingleStrikeBenchCounters(EffectContext ctx)
        {
            return ctx.Me.Bench.Where(m => m.Def.IsSingleStrike).Sum(m => m.DamageCounters);
        }

        // ================================================================== Evolving from the deck

        /// <summary>This card can be put onto the Pokémon to evolve it (its name matches, or it's a Baby's Basic Pokémon).</summary>
        internal static bool EvolvesOnto(EffectContext ctx, PokemonInPlay mon, CardDef def)
        {
            if (mon == null || def == null || !def.IsPokemon || def.IsLevelUp || def.IsBreak || def.IsLegend) return false;
            if (ctx.Engine.Scripts.For(def).PutIntoPlayOnlyByEffect) return false;
            return mon.Name == def.EvolvesFrom || mon.Def.BabyEvolvesInto(def) || ctx.Engine.Scripts.For(mon.Def).EvolvesIntoByRule(mon.Def, def);
        }

        /// <summary>
        /// Frozen Awakening, Solar Evolution and Boost Shake ignore the first-turn and same-turn timing, but effects that say a
        /// Pokémon (or a player's Pokémon) can't evolve still stop them.
        /// </summary>
        internal static bool EvolveBlocked(EffectContext ctx, PokemonInPlay mon)
        {
            return mon.CantEvolveOnTurn == ctx.Turn || ctx.Engine.EvolveLocked(mon.Owner);
        }

        /// <summary>"Search your deck for a card that evolves from this Pokémon and put it onto this Pokémon to evolve it. Then, shuffle your deck."</summary>
        internal static bool EvolveFromDeck(EffectContext ctx, PokemonInPlay mon, string what)
        {
            if (mon == null || !ctx.InPlay(mon) || ctx.Me.Deck.Count == 0) return false;
            if (EvolveBlocked(ctx, mon))
            {
                ctx.Log(mon.Name + " can't evolve right now (" + what + ").");
                return false;
            }
            var found = ctx.SearchMyDeck(cd => EvolvesOnto(ctx, mon, cd), 1, what + ": choose a card that evolves from " + mon.Name, false);
            if (found.Count > 0) ctx.Engine.EvolvePokemon(mon, found[0]);
            ctx.ShuffleMyDeck();
            return found.Count > 0;
        }

        // ================================================================== Shared helpers

        /// <summary>
        /// Cruel Charge, Strong Legs Charge, Malevolent Charge (the Galarian birds): "When you play this Pokémon from your hand onto
        /// your Bench during your turn, you may attach up to 2 [T] Energy cards from your hand to this Pokémon."
        /// </summary>
        internal sealed class ChargeFromHandMon : Mon
        {
            private readonly string ability;
            private readonly EnergyType type;
            private readonly Mon attacks;

            public ChargeFromHandMon(string ability, EnergyType type, Mon attacks)
            {
                this.ability = ability;
                this.type = type;
                this.attacks = attacks;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (mon == null || !ctx.IsMyTurn) return;
                var energy = ctx.Me.Hand.Where(c => IsEnergyOf(c.Def, type)).ToList();
                if (energy.Count == 0) return;
                if (!TriggerOnce(ctx, ability, true, "Use " + ability + "? Attach up to 2 " + type + " Energy cards from your hand to " + mon.Name + ".")) return;
                ctx.AttachCardsAnyWay(energy, 2, (card, holder) => holder == mon, ability, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"If the Defending Pokémon is a [kind], it can't attack during your opponent's next turn" (Ancient Freeze, Chrono Wind).</summary>
        internal sealed class DefenderCantAttackMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> kind;

            public DefenderCantAttackMon(string attack, Func<CardDef, bool> kind)
            {
                this.attack = attack;
                this.kind = kind;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool matches = defending != null && kind(defending.Def);
                ctx.DamageDefending(atk.Damage);
                if (!matches || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"Search your deck for a card that evolves from this Pokémon and put it onto this Pokémon to evolve it" as an attack (Frozen Awakening).</summary>
        internal sealed class EvolveFromDeckAttackMon : Mon
        {
            private readonly string attack;

            public EvolveFromDeckAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                EvolveFromDeck(ctx, ctx.Pokemon, attack);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Vise Coach: damage from your Single Strike Pokémon's attacks isn't affected by your opponent's Active Pokémon's Resistance.</summary>
        private sealed class PinsirEVS : Mon
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                var attacker = info.Attacker;
                return src.Pokemon != null && attacker != null && attacker.Owner == src.Player && info.AgainstOpponent && attacker.Def.IsSingleStrike;
            }
        }

        /// <summary>
        /// Solar Evolution: when you attach an Energy card from your hand to this Pokémon during your turn, you may evolve it from
        /// the deck. Your ruling (v58): it ignores the usual timing (first turn, came into play or evolved this turn).
        /// </summary>
        private sealed class SkiploomEVS : Mon
        {
            public override bool EvolvesOnOwnerAttachFromHand { get { return true; } }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var mon = src.Pokemon;
                if (mon == null || target != mon || !src.IsMyTurn || !src.InPlay(mon) || src.Me.Deck.Count == 0 || EvolveBlocked(src, mon)) return;
                if (!src.AskYesNo("Solar Evolution: search your deck for a card that evolves from " + mon.Name + " and evolve it?")) return;
                Announce(src, "Solar Evolution");
                EvolveFromDeck(src, mon, "Solar Evolution");
            }
        }

        /// <summary>Fluffy Barrage: this Pokémon may attack twice each turn (after a Knock Out, once the opponent has a new Active Pokémon).</summary>
        private sealed class JumpluffEVS : Mon
        {
            public override bool AttacksTwice(EffectContext src) { return src.Pokemon != null; }
        }

        /// <summary>Astonish: a random card from the opponent's hand is revealed and shuffled into their deck. Used for Astonish, Petrifying Flame (the attack name is passed in).</summary>
        internal sealed class AstonishMon : Mon
        {
            private readonly string attack;

            public AstonishMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>Greening Cells: once during your turn, search your deck for a Grass Energy card and attach it to 1 of your Pokémon; then your turn ends.</summary>
        private sealed class LeafeonV : Mon
        {
            private static readonly Mon LeafBlade = new FlipBonusMon("Leaf Blade", 60);

            protected override string Activated { get { return "Greening Cells"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(cd => IsEnergyOf(cd, EnergyType.Grass), 1, "Greening Cells: choose a Grass Energy card to attach");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Greening Cells: attach " + found[0].Def.Name + " to which of your Pokémon?");
                    ctx.Engine.AttachEnergyCard(found[0], target);
                }
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LeafBlade.HandleAttack(ctx, atk); }
        }

        /// <summary>Dizzying Flower: flip a coin; heads Asleep, tails Confused.</summary>
        private sealed class DizzyingFlowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dizzying Flower") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, ctx.Flip() ? SpecialCondition.Asleep : SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Cotton Lift: once during your turn, search your deck for up to 2 basic Energy cards. Cotton Guard: 30 less damage next turn.</summary>
        private sealed class EldegossEVS : Mon
        {
            private static readonly Mon CottonGuard = new ShieldAttackMon("Cotton Guard", 30);

            protected override string Activated { get { return "Cotton Lift"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(cd => cd.IsBasicEnergy, 2, "Cotton Lift: choose up to 2 basic Energy cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CottonGuard.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Max Detonate: discard the top 5 cards of your deck; 100 damage for each Energy card discarded.</summary>
        private sealed class MaxDetonateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Detonate") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                int energy = milled.Count(c => c.Def.IsEnergy);
                ctx.Log("Max Detonate: " + energy + (energy == 1 ? " Energy card" : " Energy cards") + " discarded.");
                if (energy > 0) ctx.DamageDefending(atk.Damage * energy);
                return true;
            }
        }

        /// <summary>Surging Flames: 20 more damage for each basic Energy card in your discard pile; then shuffle those cards into your deck.</summary>
        private sealed class SurgingFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surging Flames") return false;
                var basics = ctx.BasicEnergyInDiscard();
                ctx.DamageDefending(atk.Damage + 20 * basics.Count);
                if (basics.Count == 0) return true;
                ctx.DiscardToDeck(basics);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Bubble Pod: put a Water Pokémon from your discard pile onto your Bench; if you do, attach up to 3 Water Energy cards from
        /// your discard pile to it. Your ruling (v58): any Water Pokémon, Evolution Pokémon included (it goes onto the Bench as it is).
        /// </summary>
        private sealed class BubblePodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Pod") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace <= 0) return true;
                var choices = ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Water) && !c.Def.IsLevelUp && !c.Def.IsBreak
                                                         && !c.Def.IsLegend && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                if (choices.Count == 0)
                {
                    ctx.Log("Bubble Pod: there are no Water Pokémon in " + ctx.Engine.Their(ctx.Player) + " discard pile.");
                    return true;
                }
                var pick = ctx.ChooseCards(choices, 1, 1, "Bubble Pod: put which Water Pokémon from your discard pile onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return true;
                var mon = ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                if (mon == null || !ctx.Me.Bench.Contains(mon)) return true;
                var water = ctx.Me.Discard.Where(c => IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count > 0) ctx.AttachCardsAnyWay(water, 3, (card, holder) => holder == mon, "Bubble Pod", true);
                return true;
            }
        }

        /// <summary>
        /// Enthusiastic Dance: when you play this Pokémon from your hand to evolve, you may make your Basic Pokémon's attacks do
        /// 100 more damage to the opponent's Active Pokémon this turn (before Weakness and Resistance).
        /// </summary>
        private sealed class LudicoloEVS : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Enthusiastic Dance", true, "Use Enthusiastic Dance? During this turn, your Basic Pokémon's attacks do 100 more damage to your opponent's Active Pokémon.")) return;
                ctx.AddEffect(new ConditionalDamageBonusEffect("Enthusiastic Dance: " + ctx.Engine.Possessive(ctx.Player) + " Basic Pokémon's attacks do 100 more damage this turn",
                    ctx.Player, ctx.Turn, 100, BasicAttacker));
            }

            private static bool BasicAttacker(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target) { return attacker.Def.IsBasicPokemon; }
        }

        /// <summary>Dew Guard: whenever your opponent plays a Supporter card from their hand, prevent all effects of that card done to you or your hand.</summary>
        private sealed class MiloticEVS : Mon
        {
            private static readonly Mon DoubleSmash = new CoinsMon("Double Smash", 2);

            public override bool ShieldsPlayerFromSupporters(EffectContext src) { return src.Pokemon != null; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DoubleSmash.HandleAttack(ctx, atk); }
        }

        /// <summary>Synchrodraw: shuffle your hand into your deck, then draw a card for each card in your opponent's hand. Used for Synchrodraw, Tuning (the attack name is passed in).</summary>
        internal sealed class SynchrodrawMon : Mon
        {
            private readonly string attack;

            public SynchrodrawMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Opp.Hand.Count);
                return true;
            }
        }

        /// <summary>Crystal Veil: prevent all damage done to this Pokémon by attacks from the opponent's Pokémon VMAX, except any Glaceon VMAX.</summary>
        private sealed class GlaceonVmax : Mon
        {
            private static readonly Mon MaxIcicle = new Cards30CC.BenchHitMon("Max Icicle", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent) return damage;
                var attacker = info.Attacker;
                return attacker.Def.IsVmax && attacker.Name != "Glaceon VMAX" ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MaxIcicle.HandleAttack(ctx, atk); }
        }

        /// <summary>Element Chain: look at the top 6 cards, attach any number of basic Energy there to your Pokémon in any way, shuffle the rest back.</summary>
        private sealed class ElementChainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Element Chain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var basics = ctx.LookAtTop(6).Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0) ctx.AttachCardsAnyWay(basics, basics.Count, (card, holder) => true, "Element Chain", true);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Group Power: +150 HP with 3 or more Water Energy attached. Schooling Shot: 30 more for each basic Energy attached.</summary>
        private sealed class WishiwashiEVS : Mon
        {
            private static readonly Mon SchoolingShot = new CardsASR1.PlusMon("Schooling Shot", 30, BasicEnergyOnSelf);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return hp;
                return src.Engine.EnergyCountOfType(mon, EnergyType.Water) >= 3 ? hp + 150 : hp;
            }

            private static int BasicEnergyOnSelf(EffectContext ctx) { return ctx.Pokemon.Energy.Count(e => e.Def.IsBasicEnergy); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SchoolingShot.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Big Sparking: 50 damage to each Pokémon V and Pokémon-GX, both players' (no Weakness or Resistance on the Bench).</summary>
        private sealed class BigSparkingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Sparking") return false;
                var targets = ctx.OpponentPokemon().Concat(ctx.MyPokemon()).Where(m => IsVOrGx(m.Def)).ToList();
                if (targets.Count == 0) ctx.Log("Big Sparking: there are no Pokémon V or Pokémon-GX in play.");
                foreach (var mon in targets)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 50);
                return true;
            }
        }

        /// <summary>Max Thunder Rumble: also 100 damage to 1 of the opponent's Benched Pokémon that has damage counters on it.</summary>
        private sealed class MaxThunderRumbleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Thunder Rumble") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(100, m => m.DamageCounters > 0);
                return true;
            }
        }

        /// <summary>Electron Crush: you may discard 3 Lightning Energy from this Pokémon for 120 more damage.</summary>
        private sealed class ElectronCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electron Crush") return false;
                var me = ctx.Pokemon;
                bool boost = false;
                if (ctx.Engine.EnergyCountOfType(me, EnergyType.Lightning) >= 3 && ctx.AskYesNo("Electron Crush: discard 3 Lightning Energy from " + me.Name + " for 120 more damage?"))
                    boost = ctx.DiscardEnergyFrom(me, 3, "Electron Crush: discard 3 Lightning Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)) >= 3;
                ctx.DamageDefending(atk.Damage + (boost ? 120 : 0));
                return true;
            }
        }

        /// <summary>Primeval Beak: during the opponent's next turn, Energy cards can't be attached from their hand to the Defending Pokémon. Used for Primeval Beak, Cross Slicer (the attack name is passed in).</summary>
        internal sealed class PrimevalBeakMon : Mon
        {
            private readonly string attack;

            public PrimevalBeakMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsCRI.NoEnergyFromHandEffect(defending.Name + ": no Energy cards from the hand during the next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Spark Trap: during the opponent's next turn, if this Pokémon is damaged by an attack (even if Knocked Out), 12 damage counters on the Attacking Pokémon.</summary>
        private sealed class SparkTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spark Trap") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new RetaliateEffect(me.Name + ": 12 damage counters on the Pokémon that damages it during the next turn (Spark Trap)", ctx.Player, me.Uid, ctx.Turn + 1, 12));
                return true;
            }
        }

        /// <summary>Teraspark: discard all Lightning Energy from this Pokémon; also 40 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class TerasparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teraspark") return false;
                var me = ctx.Pokemon;
                int lightning = me.Energy.Count(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning));
                if (lightning > 0) ctx.DiscardEnergyFrom(me, lightning, "Teraspark: discard all Lightning Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning));
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, "Teraspark: choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(target, 40);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Wake-Up Slap: 90 more damage (80 on Sun &amp; Moon's Poliwrath) if the opponent's Active Pokémon has a Special Condition; then it recovers from all of them.</summary>
        internal sealed class WakeUpSlapMon : Mon
        {
            private readonly int bonus;

            public WakeUpSlapMon() : this(90) { }

            public WakeUpSlapMon(int bonus) { this.bonus = bonus; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wake-Up Slap") return false;
                var defending = ctx.Defending;
                bool condition = defending != null && defending.Conditions != SpecialCondition.None;
                ctx.DamageDefending(atk.Damage + (condition ? bonus : 0));
                if (condition && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending)) ctx.Engine.RemoveAllConditions(defending);
                return true;
            }
        }

        /// <summary>Psylaser: discard all Psychic Energy from this Pokémon; 120 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class PsylaserMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psylaser") return false;
                var me = ctx.Pokemon;
                int psychic = me.Energy.Count(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Psychic));
                if (psychic > 0) ctx.DiscardEnergyFrom(me, psychic, "Psylaser: discard all Psychic Energy", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Psychic));
                ctx.DamageAnyOpponent(120);
                return true;
            }
        }

        /// <summary>Zen Shot: 60 damage to 1 of the opponent's Pokémon V.</summary>
        private sealed class ZenShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zen Shot") return false;
                var targets = ctx.OpponentPokemon(m => m.Def.IsPokemonV);
                if (targets.Count == 0)
                {
                    ctx.Log("Zen Shot: " + ctx.Engine.PlayerName(ctx.Opponent) + " has no Pokémon V in play.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Zen Shot: choose 1 of your opponent's Pokémon V");
                ctx.DamageTarget(target, 60);
                return true;
            }
        }

        /// <summary>Mirror Pain: damage counters on the opponent's Active Pokémon equal to those on 1 of your Benched Pokémon.</summary>
        private sealed class MirrorPainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Pain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                var hurt = ctx.Me.Bench.Where(m => m.DamageCounters > 0).ToList();
                if (defending == null || hurt.Count == 0)
                {
                    ctx.Log("Mirror Pain: none of " + ctx.Engine.Possessive(ctx.Player) + " Benched Pokémon have damage counters.");
                    return true;
                }
                var pick = ctx.ChoosePokemon(hurt, "Mirror Pain: copy the damage counters of which Benched Pokémon?");
                ctx.PlaceCounters(defending, pick.DamageCounters);
                return true;
            }
        }

        /// <summary>Rapid Strike Connection: as often as you like during your turn, move an Energy from 1 of your Pokémon to 1 of your Rapid Strike Pokémon.</summary>
        private sealed class FlorgesEVS : Mon
        {
            private const string Connection = "Rapid Strike Connection";
            private static readonly Mon WonderShine = new ConditionAttackMon("Wonder Shine", SpecialCondition.Confused);

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var rapid = ctx.MyPokemon(m => m.Def.IsRapidStrike);
                return ctx.MyPokemon(m => m.Energy.Count > 0 && rapid.Any(x => x != m));
            }

            private static bool CanMove(EffectContext ctx) { return Givers(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == Connection && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), Connection + ": move an Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null || from.Energy.Count == 0) return false;
                var card = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, Connection + ": move which Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from && m.Def.IsRapidStrike), Connection + ": move " + card.Def.Name + " to which Rapid Strike Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WonderShine.HandleAttack(ctx, atk); }
        }

        /// <summary>Pumpkin Pit: when you play this Pokémon from your hand onto your Bench during your turn, you may discard a Stadium in play.</summary>
        private sealed class PumpkabooEVS : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.State.Stadium == null || !ctx.IsMyTurn) return;
                if (!TriggerOnce(ctx, "Pumpkin Pit", true, "Use Pumpkin Pit? Discard " + ctx.State.Stadium.Def.Name + ".")) return;
                CardsTWM1.DiscardStadiumInPlay(ctx);
            }
        }

        /// <summary>Pandemonium: reveal the top 6; 60 damage for each Psychic Pokémon there; shuffle those back and discard the other cards.</summary>
        private sealed class PandemoniumMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pandemonium") return false;
                var top = ctx.Me.Deck.Take(Math.Min(6, ctx.Me.Deck.Count)).ToList();
                if (top.Count == 0) return true;
                ctx.Engine.Reveal(ctx.Player, top, "revealed the top " + top.Count + " cards of " + ctx.Engine.Their(ctx.Player) + " deck");
                var psychic = top.Where(c => c.Def.IsPokemon && c.Def.HasType(EnergyType.Psychic)).ToList();
                if (psychic.Count > 0) ctx.DamageDefending(atk.Damage * psychic.Count);
                else ctx.Log("Pandemonium: no Psychic Pokémon were revealed.");
                int discarded = 0;
                foreach (var card in top)
                {
                    if (psychic.Contains(card) || !ctx.Me.Deck.Remove(card)) continue;
                    ctx.Me.Discard.Add(card);
                    discarded++;
                }
                if (discarded > 0) ctx.Log("Pandemonium: " + discarded + (discarded == 1 ? " card was" : " cards were") + " discarded.");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Tricky Steps: you may move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        internal sealed class TrickyStepsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tricky Steps") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Tricky Steps: move an Energy from " + defending.Name + " to 1 of your opponent's Benched Pokémon?")) return true;
                var energy = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Tricky Steps: move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Tricky Steps: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, energy);
                return true;
            }
        }

        /// <summary>Shadow Flicker: if the Defending Pokémon is Knocked Out during your next turn, take 1 more Prize card.</summary>
        private sealed class ShadowFlickerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Flicker") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || ctx.Engine.IsKnockedOut(defending)) return true;
                var effect = new KnockOutPrizeBonusEffect("If " + defending.Name + " is Knocked Out during " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn, they take 1 more Prize card (Shadow Flicker)",
                    ctx.Player, defending.Uid, ctx.Turn + 2, 1);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Zapper Kick: you may discard all Energy from this Pokémon; if you do, the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class ZapperKickMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zapper Kick") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Zapper Kick: discard all Energy from " + me.Name + " to Paralyze your opponent's Active Pokémon?")) return true;
                ctx.Engine.DiscardAllEnergy(me);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Yoga Loop: 2 damage counters on 1 of the opponent's Pokémon; if that Knocks it Out, take another turn after this one
        /// (skip Pokémon Checkup). It can't be used if 1 of your Pokémon used it during your last turn. Smash Uppercut: no Resistance.
        /// </summary>
        private sealed class MedichamV : Mon
        {
            private const string Loop = "Yoga Loop";
            private static readonly Mon Uppercut = new CardsFLF1.NoResistanceMon("Smash Uppercut");

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != Loop) return true;
                int turn;
                return !(ctx.Me.NamedLimitUsedOnTurn.TryGetValue(Loop, out turn) && turn == ctx.MyLastTurn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != Loop) return Uppercut.HandleAttack(ctx, atk);
                ctx.MarkNamedLimit(Loop);
                var targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Yoga Loop: put 2 damage counters on which of your opponent's Pokémon?");
                ctx.PlaceCounters(target, 2);
                if (ctx.InPlay(target) && ctx.Engine.IsKnockedOut(target) && ctx.State.ExtraTurnFor < 0)
                {
                    ctx.State.ExtraTurnFor = ctx.Player;
                    ctx.State.SkipCheckup = true;
                    ctx.Log("Yoga Loop Knocked Out " + target.Name + ": " + ctx.Engine.PlayerName(ctx.Player) + " takes another turn after this one.");
                }
                return true;
            }
        }

        /// <summary>Pressure Shot: this Pokémon also does 10 damage to itself for each damage counter on it.</summary>
        private sealed class PressureShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pressure Shot") return false;
                var me = ctx.Pokemon;
                int counters = me.DamageCounters;
                ctx.DamageDefending(atk.Damage);
                if (counters > 0 && ctx.InPlay(me)) ctx.DamageSelf(10 * counters);
                return true;
            }
        }

        /// <summary>Shaky Wave: during the opponent's next turn, the Defending Pokémon's attacks cost [C] more and its Retreat Cost is [C] more.</summary>
        private sealed class ShakyWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shaky Wave") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost [C] more during its next turn (Shaky Wave)", ctx.Player, defending.Uid, ctx.Turn + 1, 1));
                ctx.AddEffect(new CardsMEW1.RetreatUpEffect(defending.Name + "'s Retreat Cost is [C] more during its next turn (Shaky Wave)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }
    }
}
