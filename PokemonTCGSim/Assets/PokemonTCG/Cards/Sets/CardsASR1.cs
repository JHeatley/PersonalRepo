using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Astral Radiance (ASR, 2022), Pokémon 1-134 plus Virizion V (164), Jirachi V (170) and Kleavor VSTAR (196). The Trainer
    /// Gallery, Trainers, Stadiums and Double Turbo Energy are in CardsASR2. Many ASR cards came into the pool earlier through
    /// Crown Zenith's Galarian Gallery (Hisuian Voltorb, Kricketune, Origin Forme Palkia VSTAR and Dialga VSTAR, Darkrai VSTAR,
    /// Hisuian Samurott V/VSTAR, Magnezone, Miltank...), Lost Origin, Silver Tempest and Paldea Evolved. Regulation mark F:
    /// Expanded only. Pokémon whose attacks only do their printed damage need no script (Scyther 4, Yanma, Dartrix, Ponyta,
    /// Quilava, Barboach, Oshawott, Bergmite, Misdreavus, Hippopotas, Hisuian Qwilfish 88, Hisuian Sneasel, Nickit).
    /// Your rulings (v53): Jirachi V's Wish Connector also works when Jirachi V itself is Knocked Out; Eevee's Resonant
    /// Evolution evolves an Eevee even if it came into play this turn.
    /// </summary>
    public static class CardsASR1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("ASR-1", new ComboMon(new CoinsMon("Twineedle", 2), new SwarmingStingMon()));
            r.RegisterByKey("ASR-3", new DrawAttackMon("Triple Draw", 3));
            r.RegisterByKey("ASR-5", new FlipBonusMon("Quick Blow", 30));
            r.RegisterByKey("ASR-7", new WideWingMon("Wide Wing"));
            r.RegisterByKey("ASR-8", new BenchRecoilMon("Overhead Throw", 30));
            r.RegisterByKey("ASR-11", new SearchAttackMon("Honey Courier", IsItemCard, 1));
            r.RegisterByKey("ASR-12", new HoneyRushMon());
            r.RegisterByKey("ASR-13", new ComboMon(new EnergyGardenMon("Energy Garden"), new LockAfterAttackMon("Leafy Cyclone", false)));
            r.RegisterByKey("ASR-14", new EncouragingGiftMon());
            r.RegisterByKey("ASR-15", new SwitchSelfMon("Spin Turn", false));
            r.RegisterByKey("ASR-16", new TwisterLutzMon());
            r.RegisterByKey("ASR-17", new CardsBCR1.DrawUntilAttackMon("Dance Gracefully", 6));
            r.RegisterByKey("ASR-18", new HisuianLilligantVstar());
            r.RegisterByKey("ASR-19", new FlipBonusMon("Jump On", 10));
            // Fire
            r.RegisterByKey("ASR-22", new RingOfFireMon());
            r.RegisterByKey("ASR-23", new SearchAttackMon("Charge Energy", IsBasicEnergyCard, 2));
            r.RegisterByKey("ASR-25", new ComboMon(new ConditionAttackMon("Heat Burn", SpecialCondition.Burned), new CardsPAL1.StadiumBonusMon("Magma Fall", 90)));
            r.RegisterByKey("ASR-26", new HeatranVmax());
            r.RegisterByKey("ASR-27", new TimesMon("Raging Blast", SelfCounters));
            // Water
            r.RegisterByKey("ASR-28", new CardsPAL1.FlipHealSelfMon("Spacing Out", 10));
            r.RegisterByKey("ASR-29", new EntangledDiveMon());
            r.RegisterByKey("ASR-30", new ComboMon(new CardsCRZ1.PiercingMon("Swift"), new TimesMon("Energy Spiral", OpponentEnergyInPlay)));
            r.RegisterByKey("ASR-31", new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep));
            r.RegisterByKey("ASR-32", new CardsMEW1.SpreadBenchMon("Blizzard", 10));
            r.RegisterByKey("ASR-33", new ComboMon(new CardsMEW1.SpreadBenchMon("Blizzard", 10), new IcebergPressMon("Iceberg Press")));
            r.RegisterByKey("ASR-34", new BorneAshoreMon());
            r.RegisterByKey("ASR-36", new ThrashMon(60, 60));
            r.RegisterByKey("ASR-37", new ComboMon(new BenchSearchAttackMon("Regi Gate", 1), new BlizzardBindMon()));
            r.RegisterByKey("ASR-38", new FrostWallMon("Frost Wall", 9999));
            r.RegisterByKey("ASR-39", new ComboMon(new SearchAttackMon("Rule the Region", IsStadiumCard, 1), new LockAfterAttackMon("Hydro Break", false)));
            r.RegisterByKey("ASR-42", new FlipBonusMon("Razor Shell", 20));
            r.RegisterByKey("ASR-43", new BenchSearchAttackMon("Gather the Crew", 2));
            r.RegisterByKey("ASR-44", new GrudgeDiveMon());
            r.RegisterByKey("ASR-46", new RadiantGreninja());
            r.RegisterByKey("ASR-48", new HisuianAvalugg());
            r.RegisterByKey("ASR-49", new ComboMon(new SearchAttackMon("Surprising Hand", IsItemCard, 3), new CardsASC.ToolBonusMon("Customized Cane", 90)));
            // Lightning
            r.RegisterByKey("ASR-50", new ComboMon(new FangSnipeMon(), new RadiatingPulseMon()));
            r.RegisterByKey("ASR-51", new ComboMon(new Dedenne(), new TargetedBoltMon()));
            // Psychic
            r.RegisterByKey("ASR-52", new HisuianTyphlosion());
            r.RegisterByKey("ASR-53", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new CardsEVS1.AstonishMon("Petrifying Flame")));
            r.RegisterByKey("ASR-54", new ComboMon(new HollowFlameMon(), new ShimmeringStarMon()));
            r.RegisterByKey("ASR-55", new HappinessMon("Touch of Happiness", 10, false));
            r.RegisterByKey("ASR-56", new HappinessMon("Voice of Happiness", 30, true));
            r.RegisterByKey("ASR-57", new HappinessMon("Shine of Happiness", 90, true));
            r.RegisterByKey("ASR-59", new ComboMon(new OminousProseMon(), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("ASR-60", new SwitchSelfMon("Teleportation Burst", false));
            r.RegisterByKey("ASR-61", new SwitchSelfMon("Teleportation Burst", false));
            r.RegisterByKey("ASR-62", new GalladeASR());
            r.RegisterByKey("ASR-63", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("ASR-64", new SpookyBalloonMon());
            r.RegisterByKey("ASR-65", new SearchAttackMon("Wise Guidance", AnyCard, 1));
            r.RegisterByKey("ASR-66", new MesepritASR());
            r.RegisterByKey("ASR-67", new ConditionAttackMon("Mind Bend", SpecialCondition.Confused));
            r.RegisterByKey("ASR-69", new WyrdeerASR());
            // Fighting
            r.RegisterByKey("ASR-70", new CardsMEW1.WithdrawMon("Defensive Posture"));
            r.RegisterByKey("ASR-71", new ScorchingHornMon());
            r.RegisterByKey("ASR-72", new CardsSVP.BenchHurtBonusMon("Revenge Buster", 50));
            r.RegisterByKey("ASR-73", new ComboMon(new CardsSVP.BenchHurtBonusMon("Revenge Buster", 140), new LockAfterAttackMon("G-Max Chi Strike", true)));
            r.RegisterByKey("ASR-74", new SudowoodoASR());
            r.RegisterByKey("ASR-75", new ComboMon(new BenchSearchAttackMon("Regi Gate", 1), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("ASR-76", new FlipBonusMon("Stone Edge", 40));
            r.RegisterByKey("ASR-77", new JurassicHammerMon("Jurassic Hammer", 3));
            r.RegisterByKey("ASR-78", new DiscardDefendingSpecialMon("Crushing Punch", false));
            r.RegisterByKey("ASR-80", new CardsTWM1.SelfDiscardMon("Sand Breath", 2));
            r.RegisterByKey("ASR-81", new RadiantHawlucha());
            r.RegisterByKey("ASR-82", new ComboMon(new CardsASC.CounterMultiplierMon("Piercing Claw"), new CardsMEG.SnipeMon("Direct Arrow", 80)));
            r.RegisterByKey("ASR-83", new ComboMon(new SearchAttackMon("Mountain Hunt", AnyCard, 2), new CardsPAF2.IgnoreDefenderEffectsMon("Close-Quarters Shooting")));
            r.RegisterByKey("ASR-84", new HisuianDecidueyeVstar());
            r.RegisterByKey("ASR-85", new ComboMon(new PlusMon("Rout", 30, OpponentBenched), new RecoilMon("Rocky Tackle", 50)));
            r.RegisterByKey("ASR-86", new ComboMon(new TimberCleaveMon(), new RecoilMon("Berserker Tackle", 30)));
            r.RegisterByKey("ASR-87", new AxeSlashMon());
            // Darkness
            r.RegisterByKey("ASR-89", new CardsPFL.FlipUntilTailsTimesMon("Spiny Rush"));
            r.RegisterByKey("ASR-90", new ComboMon(new CardsDAA2.TriplePoisonMon("Tormenting Poison", 5), new NoRetreatMon("Pinning")));
            r.RegisterByKey("ASR-91", new CardsMEG.EnergyInPlayBonusMon("Dirty Press", EnergyType.Darkness, 3, 90));
            r.RegisterByKey("ASR-93", new HisuianSneasler());
            r.RegisterByKey("ASR-94", new ComboMon(new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned), new TimesMon("Dire Claw", DefendingConditions)));
            r.RegisterByKey("ASR-96", new MightyenaASR());
            r.RegisterByKey("ASR-98", new ConditionAttackMon("Dark Void", SpecialCondition.Asleep));
            r.RegisterByKey("ASR-100", new HisuianSamurott());
            // Metal
            r.RegisterByKey("ASR-105", new MagneticCatchMon());
            r.RegisterByKey("ASR-106", new CardsASC.PushDownMon("Bounce Back"));
            r.RegisterByKey("ASR-108", new ComboMon(new BenchSearchAttackMon("Regi Gate", 1), new HeavySlamMon()));
            r.RegisterByKey("ASR-109", new DodgeMon("Hard Headbutt", false));
            r.RegisterByKey("ASR-110", new BastiodonASR());
            r.RegisterByKey("ASR-111", new ParalyzeFlipMon("Metal Press"));
            r.RegisterByKey("ASR-112", new BronzongASR());
            r.RegisterByKey("ASR-113", new MetalCoatingMon());
            r.RegisterByKey("ASR-116", new PlusMon("Vengeful Cut", 30, BenchedPawniardCounters));
            // Dragon
            r.RegisterByKey("ASR-117", new SonicStrikeMon());
            r.RegisterByKey("ASR-118", new RegidragoASR());
            // Colorless
            r.RegisterByKey("ASR-119", new EeveeASR());
            r.RegisterByKey("ASR-120", new HoothootASR());
            r.RegisterByKey("ASR-121", new ComboMon(new RevealHandAttackMon("Silent Wing"), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("ASR-122", new GatherFoodMon());
            r.RegisterByKey("ASR-123", new CardsPFL.FlipUntilTailsTimesMon("Continuous Slap"));
            r.RegisterByKey("ASR-124", new ComboMon(new PeatHuntMon(), new CardsTWM1.SelfDiscardMon("Bulky Bump", 2)));
            r.RegisterByKey("ASR-125", new ShieldAttackMon("Guard Press", 30));
            r.RegisterByKey("ASR-127", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("ASR-128", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("ASR-129", new ComboMon(new DrawAttackMon("Collect", 1), new NoRetreatMon("Jabber On")));
            r.RegisterByKey("ASR-130", new RegigigasASR());
            r.RegisterByKey("ASR-131", new TailsNothingMon("Claw"));
            r.RegisterByKey("ASR-132", new ComboMon(new TimesMon("Psywave", DefendingEnergy), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("ASR-133", new OranguruV());
            r.RegisterByKey("ASR-134", new WyrdeerV());
            // Full-art only Pokémon
            r.RegisterByKey("ASR-164", new VirizionV());
            r.RegisterByKey("ASR-170", new JirachiV());
            r.RegisterByKey("ASR-196", new ComboMon(new AxeBreakMon("Axe Break"), new TimesMon("Rampaging Star", PokemonInDiscard)));
        }

        // ================================================================== Filters and counters

        private static bool IsItemCard(CardDef d) { return d.IsItem; }
        private static bool IsStadiumCard(CardDef d) { return d.IsStadiumCard; }
        private static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }
        private static bool AnyCard(CardDef d) { return true; }

        private static int SelfCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters; }
        private static int OpponentBenched(EffectContext ctx) { return ctx.Opp.Bench.Count; }
        private static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }
        private static int PokemonInDiscard(EffectContext ctx) { return ctx.Me.Discard.Count(c => c.Def.IsPokemon); }

        private static int OpponentEnergyInPlay(EffectContext ctx)
        {
            return ctx.OpponentPokemon().Sum(m => ctx.EnergyCount(m));
        }

        private static int BenchedPawniardCounters(EffectContext ctx)
        {
            return ctx.Me.Bench.Where(m => m.Name == "Pawniard").Sum(m => m.DamageCounters);
        }

        /// <summary>Special Conditions affecting the opponent's Active Pokémon (Dire Claw).</summary>
        internal static int DefendingConditions(EffectContext ctx)
        {
            var d = ctx.Defending;
            if (d == null) return 0;
            int n = 0;
            foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                if (d.Has(c)) n++;
            return n;
        }

        /// <summary>
        /// "A [T] Energy card" outside play (hand, deck, discard pile): basic Energy of that type only. Typed Special Energy has a
        /// type only while attached (Compendium, Star Portal; your ruling, v65). For attached cards use ProvidesTypeAttached.
        /// </summary>
        internal static bool IsEnergyCardOf(CardDef d, EnergyType type)
        {
            return d.IsBasicEnergy && d.EnergyProvides == type;
        }

        /// <summary>An attached Energy card of a type: basic Energy, or a typed Special Energy (it provides its type while attached).</summary>
        internal static bool ProvidesTypeAttached(CardDef d, EnergyType type)
        {
            return d.IsEnergy && d.EnergyProvides == type;
        }

        // ================================================================== Reusable attacks

        /// <summary>"This attack does N damage for each [something]" (the printed damage times the count).</summary>
        internal sealed class TimesMon : Mon
        {
            private readonly string attack;
            private readonly Func<EffectContext, int> count;

            public TimesMon(string attack, Func<EffectContext, int> count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * Math.Max(0, count(ctx)));
                return true;
            }
        }

        /// <summary>"This attack does N more damage for each [something]."</summary>
        internal sealed class PlusMon : Mon
        {
            private readonly string attack;
            private readonly int per;
            private readonly Func<EffectContext, int> count;

            public PlusMon(string attack, int per, Func<EffectContext, int> count)
            {
                this.attack = attack;
                this.per = per;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + per * Math.Max(0, count(ctx)));
                return true;
            }
        }

        /// <summary>"This attack also does N damage to 1 of your Benched Pokémon" (Overhead Throw).</summary>
        internal sealed class BenchRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchRecoilMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, attack + ": choose 1 of your Benched Pokémon to take " + amount + " damage");
                ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>
        /// "Discard a Special Energy from your opponent's Active Pokémon" after the damage (Crushing Punch, Assault Tackle), or
        /// before doing damage (Thunderous Kick).
        /// </summary>
        internal sealed class DiscardDefendingSpecialMon : Mon
        {
            private readonly string attack;
            private readonly bool beforeDamage;

            public DiscardDefendingSpecialMon(string attack, bool beforeDamage)
            {
                this.attack = attack;
                this.beforeDamage = beforeDamage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (beforeDamage) DiscardSpecial(ctx, defending);
                ctx.DamageDefending(atk.Damage);
                if (!beforeDamage && TefPokemon.StillDefending(ctx, defending)) DiscardSpecial(ctx, defending);
                return true;
            }

            private void DiscardSpecial(EffectContext ctx, PokemonInPlay defending)
            {
                if (defending == null) return;
                ctx.DiscardEnergyFrom(defending, 1, attack + ": discard a Special Energy from " + defending.Name, e => e.Def.IsAttachedSpecialEnergy);
            }
        }

        // ================================================================== Grass

        /// <summary>Swarming Sting: 50 damage to 1 of your opponent's Pokémon for each of your Beedrill V in play.</summary>
        private sealed class SwarmingStingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swarming Sting") return false;
                int n = ctx.MyPokemon(m => m.Name == "Beedrill V").Count;
                ctx.DamageAnyOpponent(50 * n);
                return true;
            }
        }

        /// <summary>Wide Wing: 80 more damage if you have more cards in your hand than your opponent. Used for Wide Wing, Hand Press (the attack name is passed in).</summary>
        internal sealed class WideWingMon : Mon
        {
            private readonly string attack;

            public WideWingMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count > ctx.Opp.Hand.Count ? 80 : 0));
                return true;
            }
        }

        /// <summary>Honey Rush: reveal any number of Sweet Honey cards from your hand (they stay there); 60 damage for each.</summary>
        private sealed class HoneyRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Honey Rush") return false;
                var honey = ctx.Me.Hand.Where(c => c.Def.Name == "Sweet Honey").ToList();
                var shown = honey.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(honey, 0, honey.Count, "Honey Rush: reveal any number of Sweet Honey cards from your hand (60 damage each)");
                if (shown.Count > 0) ctx.Engine.Reveal(ctx.Player, shown, "revealed for Honey Rush");
                ctx.DamageDefending(atk.Damage * shown.Count);
                return true;
            }
        }

        /// <summary>Energy Garden: search for up to 3 basic Energy cards of different types; attach them to your Pokémon in any way you like. Used for Energy Garden, Breath of Life (the attack name is passed in).</summary>
        internal sealed class EnergyGardenMon : Mon
        {
            private readonly string attack;

            public EnergyGardenMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = new List<CardInstance>();
                for (int i = 0; i < 3 && ctx.Me.Deck.Count > 0; i++)
                {
                    var taken = new HashSet<EnergyType>(found.Select(c => c.Def.EnergyProvides));
                    var pick = ctx.SearchMyDeck(d => d.IsBasicEnergy && !taken.Contains(d.EnergyProvides), 1,
                        attack + ": choose a basic Energy card of a type you haven't chosen (" + (i + 1) + " of up to 3)");
                    if (pick.Count == 0) break;
                    found.AddRange(pick);
                }
                if (found.Count > 0) CardsMEG.AttachFromDeckAnyWay(ctx, found, (card, mon) => true, attack);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Encouraging Gift: only if you go second, during your first turn: search your deck for up to 3 cards.</summary>
        private sealed class EncouragingGiftMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Encouraging Gift") return true;
                return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Encouraging Gift") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 3, "Encouraging Gift: choose up to 3 cards to put into your hand", false);
                return true;
            }
        }

        /// <summary>Twister Lutz: 20 damage to each of your opponent's Pokémon, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class TwisterLutzMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister Lutz") return false;
                CardsCRZ1.DamageEachOpponent(ctx, 20);
                ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Star Perfume (VSTAR Power): search for up to 5 Grass Pokémon and Grass Energy cards. Parallel Spin: return an Energy to the hand for 100 more.</summary>
        private sealed class HisuianLilligantVstar : Mon
        {
            protected override string Activated { get { return "Star Perfume"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => (d.IsPokemon && d.HasType(EnergyType.Grass)) || IsEnergyCardOf(d, EnergyType.Grass), 5,
                    "Star Perfume: choose up to 5 Grass Pokémon and Grass Energy cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parallel Spin") return false;
                var me = ctx.Pokemon;
                int bonus = 0;
                if (me.Energy.Count > 0 && ctx.AskYesNo("Parallel Spin: put an Energy attached to this Pokémon into your hand for 100 more damage?"))
                {
                    var pick = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Parallel Spin: put which Energy into your hand?")[0];
                    if (ctx.Engine.ReturnEnergyToHand(me, pick)) bonus = 100;
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Ring of Fire: Burned, and it can't retreat during your opponent's next turn.</summary>
        internal sealed class RingOfFireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ring of Fire") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Magma Gain: once during your turn, if you have a Stadium in play, heal 50 damage from this Pokémon. Max Heat Burst: Burned.</summary>
        private sealed class HeatranVmax : Mon
        {
            private static readonly Mon burst = new ConditionAttackMon("Max Heat Burst", SpecialCondition.Burned);

            protected override string Activated { get { return "Magma Gain"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsPAL1.HaveStadium(ctx) && ctx.Pokemon.DamageCounters > 0 && !ctx.Engine.HealingBlocked(ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Pokemon, 50); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return burst.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>
        /// Entangled Dive: discard each player's Active Pokémon and all attached cards; you choose your new Active Pokémon first.
        /// Not a Knock Out, so no Prize cards (Compendium, 2022-05-26).
        /// </summary>
        private sealed class EntangledDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Entangled Dive") return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                if (defending != null && ctx.CanAffect(defending)) ctx.Engine.DiscardPokemonFromPlay(defending);
                if (ctx.InPlay(me) && ctx.PokemonIsActive) ctx.Engine.DiscardPokemonFromPlay(me);
                return true;
            }
        }

        /// <summary>Iceberg Press: discard an Energy from this Pokémon; the Defending Pokémon can't attack during your opponent's next turn. Used for Iceberg Press, Cryofreeze (the attack name is passed in).</summary>
        internal sealed class IcebergPressMon : Mon
        {
            private readonly string attack;

            public IcebergPressMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Borne Ashore: put a Basic Pokémon from either player's discard pile onto that player's Bench.</summary>
        internal sealed class BorneAshoreMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Borne Ashore") return false;
                var choices = new List<CardInstance>();
                for (int i = 0; i < 2; i++)
                {
                    int owner = i;
                    if (ctx.Engine.BenchSpace(owner) <= 0) continue;
                    choices.AddRange(ctx.State.Players[owner].Discard.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(owner, c.Def)));
                }
                if (choices.Count == 0) return true;
                var pick = ctx.ChooseCards(choices, 1, 1, "Borne Ashore: put a Basic Pokémon from either discard pile onto its owner's Bench")[0];
                var pile = ctx.State.Players[pick.Owner].Discard;
                if (!pile.Remove(pick)) return true;
                ctx.Engine.PutOntoBench(pick.Owner, pick, false);
                return true;
            }
        }

        /// <summary>Thrash: heads, 60 more damage; tails, this Pokémon also does 60 damage to itself. The numbers are passed in.</summary>
        internal sealed class ThrashMon : Mon
        {
            private readonly int amount;
            private readonly int amount2;

            public ThrashMon(int amount, int amount2)
            {
                this.amount = amount;
                this.amount2 = amount2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thrash") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? amount : 0));
                if (!heads) ctx.DamageSelf(amount2);
                return true;
            }
        }

        /// <summary>Blizzard Bind: if the Defending Pokémon is a Pokémon V, it can't attack during your opponent's next turn.</summary>
        private sealed class BlizzardBindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blizzard Bind") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsPokemonV && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Blizzard Bind)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Frost Wall: during your opponent's next turn, prevent all damage done to this Pokémon by attacks from Evolution Pokémon. Used for Frost Wall, Deflecting Splash, Crystal Ray, Crag Bash (the attack name and the numbers are passed in).</summary>
        internal sealed class FrostWallMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FrostWallMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CardsPBL.ShieldSelfFrom(ctx, atk.Name, amount, m => m.Def.IsEvolution, "Evolution Pokémon");
                return true;
            }
        }

        /// <summary>Grudge Dive: if any of your Pokémon were Knocked Out by damage from an attack during your opponent's last turn, 90 more damage and Confused.</summary>
        private sealed class GrudgeDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grudge Dive") return false;
                bool grudge = ctx.WasKnockedOutByAttackLastOpponentTurn();
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (grudge ? 90 : 0));
                if (grudge && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>
        /// Concealed Cards: once during your turn, discard an Energy card from your hand (a cost) to draw 2 cards. Moonlight
        /// Shuriken: discard 2 Energy from this Pokémon; 90 damage to 2 of your opponent's Pokémon.
        /// </summary>
        private sealed class RadiantGreninja : Mon
        {
            protected override string Activated { get { return "Concealed Cards"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => c.Def.IsEnergy) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var pick = ctx.ChooseCards(energy, 1, 1, "Concealed Cards: discard an Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moonlight Shuriken") return false;
                ctx.DiscardEnergyFrom(ctx.Pokemon, 2, "Moonlight Shuriken: discard 2 Energy from this Pokémon");
                CardsDRI1.DamageTwoOpponents(ctx, 90, atk.Name);
                return true;
            }
        }

        /// <summary>Massive Ice: this Pokémon takes 30 less damage from attacks. Mountain Gale: with a Stadium in play, 120 more, then discard it.</summary>
        private sealed class HisuianAvalugg : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Gale") return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? 120 : 0));
                if (stadium) CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Fang Snipe: your opponent reveals their hand; discard a Trainer card you find there.</summary>
        internal sealed class FangSnipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fang Snipe") return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                var trainers = ctx.Opp.Hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Fang Snipe: discard a Trainer card from your opponent's hand");
                if (pick.Count > 0) ctx.Engine.DiscardFromHand(ctx.Opponent, pick[0]);
                return true;
            }
        }

        /// <summary>Radiating Pulse: discard 2 Energy from this Pokémon; the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class RadiatingPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Radiating Pulse") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Targeted Bolt: discard 2 Lightning Energy from this Pokémon; 120 damage to 1 of your opponent's Benched Pokémon.</summary>
        private sealed class TargetedBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Targeted Bolt") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Targeted Bolt: discard 2 Lightning Energy from this Pokémon", e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning));
                ctx.DamageOneBenched(120);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Supernatural Orb: discard a Psychic Energy card from your hand (a cost) to make the opponent's Active Pokémon Burned and
        /// Confused; usable unless it's already both (Compendium, 2022-05-26). Shadow Bind: it can't retreat.
        /// </summary>
        private sealed class HisuianTyphlosion : Mon
        {
            private static readonly Mon bind = new NoRetreatMon("Shadow Bind");

            protected override string Activated { get { return "Supernatural Orb"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                return d != null && !(d.Has(SpecialCondition.Burned) && d.Has(SpecialCondition.Confused))
                    && ctx.Me.Hand.Any(c => IsEnergyCardOf(c.Def, EnergyType.Psychic));
            }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => IsEnergyCardOf(c.Def, EnergyType.Psychic)).ToList();
                var pick = ctx.ChooseCards(energy, 1, 1, "Supernatural Orb: discard a Psychic Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                var target = ctx.Opp.Active;
                if (target == null || !ctx.CanAffect(target)) return;
                ctx.ApplyCondition(target, SpecialCondition.Burned);
                ctx.ApplyCondition(target, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return bind.HandleAttack(ctx, atk); }
        }

        /// <summary>Hollow Flame: put 3 damage counters on your opponent's Benched Pokémon in any way you like.</summary>
        private sealed class HollowFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hollow Flame") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.PlaceCountersAnyWay(3, () => ctx.OpponentBench());
                return true;
            }
        }

        /// <summary>Shimmering Star (VSTAR Power): if the opponent's Active Pokémon has exactly 4 damage counters, it is Knocked Out.</summary>
        private sealed class ShimmeringStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shimmering Star") return false;
                var d = ctx.Defending;
                if (d == null || d.DamageCounters != 4 || !ctx.CanAffect(d))
                {
                    ctx.Log("Shimmering Star: the opponent's Active Pokémon doesn't have exactly 4 damage counters.");
                    return true;
                }
                ctx.Engine.KnockOut(d, "Shimmering Star");
                return true;
            }
        }

        /// <summary>
        /// Touch / Voice / Shine of Happiness: when you play this Pokémon from your hand onto your Bench (Togepi) or to evolve
        /// (Togetic, Togekiss) during your turn, you may heal N damage from your Active Pokémon.
        /// </summary>
        private sealed class HappinessMon : Mon
        {
            private readonly string ability;
            private readonly int amount;
            private readonly bool onEvolve;

            public HappinessMon(string ability, int amount, bool onEvolve)
            {
                this.ability = ability;
                this.amount = amount;
                this.onEvolve = onEvolve;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!onEvolve) Trigger(ctx);
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (onEvolve) Trigger(ctx);
            }

            private void Trigger(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                bool ready = active != null && active.DamageCounters > 0 && !ctx.Engine.HealingBlocked(active);
                if (!TriggerOnce(ctx, ability, ready, ability + ": heal " + amount + " damage from your Active Pokémon?")) return;
                ctx.Heal(active, amount);
            }
        }

        /// <summary>Ominous Prose: the opponent reveals their hand; with 4 or more cards, you choose all but 3 and they shuffle those into their deck.</summary>
        private sealed class OminousProseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ominous Prose") return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand;
                if (hand.Count < 4) return true;
                int n = hand.Count - 3;
                var picks = ctx.ChooseCards(hand.ToList(), n, n, "Ominous Prose: choose " + n + " cards from your opponent's hand to shuffle into their deck (they keep 3)");
                foreach (var c in picks)
                {
                    if (!hand.Remove(c)) continue;
                    ctx.Opp.Deck.Add(c);
                }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Buddy Catch: once during your turn, search your deck for a Supporter. Swirling Slice: move an Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class GalladeASR : Mon
        {
            protected override string Activated { get { return "Buddy Catch"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter, 1, "Buddy Catch: choose a Supporter card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Slice") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Swirling Slice: move which Energy to your Bench?")[0];
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Swirling Slice: move " + energy.Def.Name + " to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, energy);
                return true;
            }
        }

        /// <summary>Spooky Balloon: put 2 damage counters on 1 of your opponent's Benched Pokémon.</summary>
        private sealed class SpookyBalloonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spooky Balloon") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Spooky Balloon: put 2 damage counters on which Benched Pokémon?");
                ctx.PlaceCounters(target, 2);
                return true;
            }
        }

        /// <summary>Mental Shroud: if you have Uxie and Azelf in play, your Pokémon have no Weakness.</summary>
        private sealed class MesepritASR : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                if (mon == null || mon.Owner != src.Player) return false;
                return src.Me.InPlay().Any(m => m.Name == "Uxie") && src.Me.InPlay().Any(m => m.Name == "Azelf");
            }
        }

        /// <summary>Hurried Gait: once during your turn, draw a card. Extrasensory: 80 more if both players have the same number of cards in hand.</summary>
        private sealed class WyrdeerASR : Mon
        {
            protected override string Activated { get { return "Hurried Gait"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extrasensory") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count == ctx.Opp.Hand.Count ? 80 : 0));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Scorching Horn: if this Pokémon has any Fire Energy attached, 80 more damage and the opponent's Active Pokémon is Burned.</summary>
        private sealed class ScorchingHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Horn") return false;
                var me = ctx.Pokemon;
                bool fire = me.Energy.Any(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire));
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (fire ? 80 : 0));
                if (fire && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Vamoose: with any damage counters on it, this Pokémon has no Retreat Cost. Double-Edge: 30 to itself.</summary>
        private sealed class SudowoodoASR : Mon
        {
            private static readonly Mon edge = new RecoilMon("Double-Edge", 30);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon == src.Pokemon && mon.DamageCounters > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return edge.HandleAttack(ctx, atk); }
        }

        /// <summary>Jurassic Hammer: if your opponent has 3 or fewer cards in their hand, this attack does nothing. Used for Jurassic Hammer, Wind Pressure (the attack name and the numbers are passed in).</summary>
        internal sealed class JurassicHammerMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public JurassicHammerMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Opp.Hand.Count <= amount) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Big Match: while this Pokémon is on your Bench, your Pokémon's attacks do 30 more damage to the opponent's Active Pokémon VMAX.</summary>
        private sealed class RadiantHawlucha : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || src.IsActive(me) || info.Attacker == null || info.Attacker.Owner != src.Player) return damage;
                return info.ToOpponentsActive && info.Target.Def.IsVmax ? damage + 30 : damage;
            }
        }

        /// <summary>Star of Fortune (VSTAR Power): draw until you have 8 cards. Somersault Feathers: discard up to 3 Energy cards from your hand, 30 more for each.</summary>
        private sealed class HisuianDecidueyeVstar : Mon
        {
            protected override string Activated { get { return "Star of Fortune"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 8 && ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(8); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Somersault Feathers") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                int n = 0;
                if (energy.Count > 0)
                {
                    var picks = ctx.ChooseCards(energy, 0, Math.Min(3, energy.Count), "Somersault Feathers: discard up to 3 Energy cards from your hand (30 more damage each)");
                    foreach (var card in picks)
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, card);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage + 30 * n);
                return true;
            }
        }

        /// <summary>Timber Cleave: flip 2 coins; if both are heads, the opponent's Active Pokémon is Knocked Out.</summary>
        private sealed class TimberCleaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Timber Cleave") return false;
                var d = ctx.Defending;
                if (ctx.FlipCoins(2) == 2 && d != null && ctx.CanAffect(d)) ctx.Engine.KnockOut(d, "Timber Cleave");
                return true;
            }
        }

        /// <summary>Axe Slash: discard an Energy from this Pokémon; if you do, discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class AxeSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Axe Slash") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Axe Slash: discard an Energy from this Pokémon") == 0) return true;
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Axe Slash: discard an Energy from " + defending.Name);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Carry and Climb: while this Pokémon is on your Bench, your Active Pokémon's Retreat Cost is 2 less (several stack).</summary>
        private sealed class HisuianSneasler : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || src.IsActive(me) || mon == null || mon.Owner != src.Player || !src.IsActive(mon)) return cost;
                return cost - 2;
            }
        }

        /// <summary>Hustle Bark: if your opponent has any Pokémon VMAX in play, this Pokémon's attacks cost 3 Colorless less. Wild Tackle: 50 to itself.</summary>
        private sealed class MightyenaASR : Mon
        {
            private static readonly Mon tackle = new RecoilMon("Wild Tackle", 50);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != src.Pokemon || !src.Opp.InPlay().Any(m => m.Def.IsVmax)) return;
                for (int i = 0; i < 3; i++) if (!cost.Remove(EnergyType.Colorless)) break;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wily Stance: once during your turn, discard a card from your hand (a cost) to draw 3 cards; not usable with an empty
        /// deck (Compendium, 2022-05-26). Dark Mastery: 20 more for each Energy attached to all of your Pokémon.
        /// </summary>
        private sealed class HisuianSamurott : Mon
        {
            private static readonly Mon mastery = new PlusMon("Dark Mastery", 20, MyEnergyInPlay);

            protected override string Activated { get { return "Wily Stance"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Wily Stance: discard a card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return mastery.HandleAttack(ctx, atk); }

            private static int MyEnergyInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)); }
        }

        // ================================================================== Metal

        /// <summary>Magnetic Catch: shuffle up to 3 Metal Energy cards from your discard pile into your deck.</summary>
        private sealed class MagneticCatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Catch") return false;
                var metal = ctx.Me.Discard.Where(c => IsEnergyCardOf(c.Def, EnergyType.Metal)).ToList();
                if (metal.Count == 0) return true;
                var picks = ctx.ChooseCards(metal, 0, Math.Min(3, metal.Count), "Magnetic Catch: shuffle up to 3 Metal Energy cards into your deck");
                if (picks.Count == 0) return true;
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Heavy Slam: 220 damage minus 50 for each Colorless in the opponent's Active Pokémon's Retreat Cost (its current cost).</summary>
        private sealed class HeavySlamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Slam") return false;
                var d = ctx.Defending;
                int cost = d == null ? 0 : ctx.Engine.RetreatCostOf(d);
                int dmg = Math.Max(0, atk.Damage - 50 * cost);
                if (dmg > 0) ctx.DamageDefending(dmg);
                else ctx.Log("Heavy Slam did no damage.");
                return true;
            }
        }

        /// <summary>Primal Fortress: your Pokémon take 30 less damage from attacks from the opponent's Pokémon V (several Bastiodon stack). Iron Tackle: 30 to itself.</summary>
        private sealed class BastiodonASR : Mon
        {
            private static readonly Mon tackle = new RecoilMon("Iron Tackle", 30);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Owner != src.Player || info.Attacker == null || !info.AgainstOpponent) return damage;
                return info.Attacker.Def.IsPokemonV ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return tackle.HandleAttack(ctx, atk); }
        }

        /// <summary>Heatproof: prevent all damage done to this Pokémon by attacks from your opponent's Fire Pokémon.</summary>
        private sealed class BronzongASR : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target != src.Pokemon || info.Attacker == null || !info.AgainstOpponent) return damage;
                return src.Engine.IsType(info.Attacker, EnergyType.Fire) ? 0 : damage;
            }
        }

        /// <summary>Metal Coating: attach up to 2 Metal Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class MetalCoatingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Coating") return false;
                var me = ctx.Pokemon;
                var metal = ctx.Me.Discard.Where(c => IsEnergyCardOf(c.Def, EnergyType.Metal)).ToList();
                if (metal.Count == 0 || !ctx.InPlay(me)) return true;
                var picks = ctx.ChooseCards(metal, 0, Math.Min(2, metal.Count), "Metal Coating: attach up to 2 Metal Energy cards to this Pokémon");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Sonic Strike: discard 3 Energy from this Pokémon; 220 damage to 1 of your opponent's Pokémon.</summary>
        private sealed class SonicStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Strike") return false;
                ctx.DiscardEnergyFrom(ctx.Pokemon, 3, "Sonic Strike: discard 3 Energy from this Pokémon");
                ctx.DamageAnyOpponent(220);
                return true;
            }
        }

        /// <summary>Dragon's Hoard: once during your turn, if this Pokémon is Active, draw until you have 4 cards (only 1 Dragon's Hoard each turn).</summary>
        private sealed class RegidragoASR : Mon
        {
            protected override string Activated { get { return "Dragon's Hoard"; } }
            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Me.Hand.Count < 4 && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(4); }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Resonant Evolution: once during your turn, when you play a Pokémon from your hand to evolve 1 of your other Eevee,
        /// you may search your deck for a card that evolves from this Pokémon and evolve it. Every Eevee with the Ability
        /// triggers (Compendium, 2022-05-26); it works even on an Eevee that came into play this turn (your ruling, v53).
        /// </summary>
        private sealed class EeveeASR : Mon
        {
            public override void OnOwnPokemonEvolvedFromHand(EffectContext src, PokemonInPlay evolved, CardDef before)
            {
                var me = src.Pokemon;
                if (me == null || evolved == me || before == null || before.Name != "Eevee" || me.Name != "Eevee") return;
                if (!TriggerOnce(src, "Resonant Evolution", src.Me.Deck.Count > 0, "Resonant Evolution: search your deck for a card that evolves from this Eevee and evolve it?")) return;
                CardsSSP1.EvolveFromDeck(src, me, "Resonant Evolution: choose a card that evolves from Eevee");
                src.ShuffleMyDeck();
            }
        }

        /// <summary>Stand Sentry: Basic Energy on your Benched Pokémon can't be discarded by the effect of your opponent's Item or Supporter cards.</summary>
        private sealed class HoothootASR : Mon
        {
            public override bool ProtectsEnergyFromTrainerDiscard(EffectContext src, PokemonInPlay holder, CardInstance energy)
            {
                return holder != null && holder.Owner == src.Player && !src.IsActive(holder) && energy.Def.IsBasicEnergy;
            }
        }

        /// <summary>Silent Wing: your opponent reveals their hand. Used for Silent Wing, Fearsome Shadow (the attack name is passed in).</summary>
        internal sealed class RevealHandAttackMon : Mon
        {
            private readonly string attack;

            public RevealHandAttackMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Gather Food: flip a coin; heads, put an Item card from your discard pile into your hand.</summary>
        private sealed class GatherFoodMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Food") return false;
                if (!ctx.Flip()) return true;
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(items, 1, 1, "Gather Food: put an Item card into your hand"));
                return true;
            }
        }

        /// <summary>Peat Hunt: put up to 2 cards from your discard pile into your hand.</summary>
        private sealed class PeatHuntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peat Hunt") return false;
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(pile, 0, Math.Min(2, pile.Count), "Peat Hunt: put up to 2 cards into your hand"));
                return true;
            }
        }

        /// <summary>
        /// Ancient Wisdom: once during your turn, with Regirock, Regice, Registeel, Regieleki and Regidrago in play, attach up to 3
        /// Energy cards from your discard pile to 1 of your Pokémon. Gigaton Break: 150 more against a Pokémon VMAX.
        /// </summary>
        private sealed class RegigigasASR : Mon
        {
            private static readonly string[] Regis = { "Regirock", "Regice", "Registeel", "Regieleki", "Regidrago" };
            private static readonly Mon gigaton = new CardsCRZ1.VmaxBonusMon("Gigaton Break", 150);

            protected override string Activated { get { return "Ancient Wisdom"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var names = new HashSet<string>(ctx.Me.InPlay().Select(m => m.Name));
                return Regis.All(n => names.Contains(n)) && ctx.Me.Discard.Any(c => c.Def.IsEnergy);
            }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Ancient Wisdom: attach Energy cards from your discard pile to which Pokémon?");
                if (target == null) return;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                var picks = ctx.ChooseCards(energy, 1, Math.Min(3, energy.Count), "Ancient Wisdom: choose up to 3 Energy cards to attach to " + target.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return gigaton.HandleAttack(ctx, atk); }
        }

        /// <summary>Back Order: once during your turn, if this Pokémon is Active, search for up to 2 Pokémon Tools. Psychic: 50 more for each Energy on the Defending Pokémon.</summary>
        private sealed class OranguruV : Mon
        {
            private static readonly Mon psychic = new CardsMEG.DefendingEnergyBonusMon("Psychic", 50);

            protected override string Activated { get { return "Back Order"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsTool, 2, "Back Order: choose up to 2 Pokémon Tool cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return psychic.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Frontier Road: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may move any
        /// amount of Energy from your other Pokémon to it (not when it's promoted after a Knock Out, like Iron Moth). Psyshield
        /// Bash: 40 for each Energy attached to this Pokémon.
        /// </summary>
        private sealed class WyrdeerV : Mon
        {
            private static readonly Mon bash = new TimesMon("Psyshield Bash", OwnEnergy);

            private static int OwnEnergy(EffectContext ctx) { return ctx.EnergyCount(ctx.Pokemon); }

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Count > 0);
            }

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                if (ctx.Engine.Promoting || ctx.State.InCheckup) return;
                if (!TriggerOnce(ctx, "Frontier Road", Givers(ctx).Count > 0, "Frontier Road: move Energy from your other Pokémon to Wyrdeer V?")) return;
                var me = ctx.Pokemon;
                for (int step = 0; step < 60; step++)
                {
                    var holders = Givers(ctx);
                    if (holders.Count == 0 || !ctx.InPlay(me)) break;
                    var source = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, "Frontier Road: move an Energy from which Pokémon to " + me.Name + "? Press Done when finished.", true);
                    if (source == null) break;
                    var card = ctx.ChooseCards(source.Energy.ToList(), 1, 1, "Frontier Road: move which Energy from " + source.Name + "?")[0];
                    if (!ctx.Engine.MoveEnergy(source, me, card)) break;
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return bash.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Verdant Wind: each of your Pokémon with any Grass Energy attached recovers from all Special Conditions and can't be
        /// affected by them. Emerald Blade: can't attack next turn.
        /// </summary>
        private sealed class VirizionV : Mon
        {
            private static readonly Mon blade = new LockAfterAttackMon("Emerald Blade", false);

            public override bool PreventsSpecialConditions(EffectContext src, PokemonInPlay target)
            {
                return target != null && target.Owner == src.Player && target.Energy.Any(e => CardsDRI1.ProvidesType(src, target, e, EnergyType.Grass));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return blade.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wish Connector: when 1 of your Basic Pokémon V is Knocked Out by damage from an opponent's attack, you may move a
        /// basic Energy card from it to another of your Pokémon. Jirachi V itself counts (your ruling, v53). Hypnostrike: both
        /// Active Pokémon are now Asleep.
        /// </summary>
        private sealed class JirachiV : Mon
        {
            public override void OnOwnPokemonKnockedOut(EffectContext src, PokemonInPlay mon, bool byOpponentsAttack)
            {
                if (!byOpponentsAttack || mon == null || mon.Owner != src.Player || !mon.Def.IsPokemonV || !mon.Def.IsBasicPokemon) return;
                var basics = mon.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var targets = src.MyPokemon(m => m != mon && !src.Engine.IsKnockedOut(m));
                if (basics.Count == 0 || targets.Count == 0) return;
                if (!src.AskYesNo("Wish Connector: " + mon.Name + " was Knocked Out. Move a basic Energy card from it to another of your Pokémon?")) return;
                Announce(src, "Wish Connector");
                var card = src.ChooseCards(basics, 1, 1, "Wish Connector: move which basic Energy card?")[0];
                var to = src.ChoosePokemon(targets, "Wish Connector: move " + card.Def.Name + " to which Pokémon?");
                if (to != null) src.Engine.MoveEnergy(mon, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnostrike") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Axe Break: also 60 damage to 1 of your opponent's Benched Pokémon V. Used for Axe Break, Psychic Javelin (the attack name is passed in).</summary>
        internal sealed class AxeBreakMon : Mon
        {
            private readonly string attack;

            public AxeBreakMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(60, m => m.Def.IsPokemonV);
                return true;
            }
        }
    }
}
