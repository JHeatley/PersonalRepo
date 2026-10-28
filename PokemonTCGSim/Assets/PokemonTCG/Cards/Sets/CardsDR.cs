using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Rare Pokémon and Pokémon-ex (DR 1-22, 89-100; v159). Unlimited only. A game here is 1-on-1, so "each Defending
    /// Pokémon" is the Defending Pokémon and Chain of Events (Minun, Plusle), which needs a second Active Pokémon, does nothing.
    /// More shared EX-era helpers live here.
    /// </summary>
    public static class CardsDR
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DR-1", new ComboMon(new BadNewsMon(), new CardsFST2.ConditionBonusMon("Prize Count", 20, c => c.Me.Prizes.Count > c.Opp.Prizes.Count)));
            r.RegisterByKey("DR-2", new AltariaDragonDance());
            r.RegisterByKey("DR-3", new CrawdauntPowerPinchers());
            r.RegisterByKey("DR-4", new ComboMon(new AttachFromHandMon("Energy Shower", d => d.IsBasicEnergy, 99),
                new CardsASR1.PlusMon("Rainbow Burn", 10, c => c.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count())));
            r.RegisterByKey("DR-5", new ComboMon(new CardsASR1.PlusMon("Rock Vengeance", 10, c => c.Me.Active == null ? 0 : c.Me.Active.DamageCounters),
                new CardsFST1.TwoBenchedMon("Rock Slide", 20)));
            r.RegisterByKey("DR-6", new ComboMon(new CardsASR1.PlusMon("Psychic Boom", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending)), new MindTripMon()));
            r.RegisterByKey("DR-7", new ComboMon(new CheerOnMon(), new CircuitMon("Special Circuit", (c, m) => m.Def.Abilities.Any(a => a.IsPokePowerOrBody))));
            r.RegisterByKey("DR-8", new ComboMon(new CheerOnMon(), new CircuitMon("Extra Circuit", (c, m) => m.Def.IsOldEx)));
            r.RegisterByKey("DR-9", new RoseliaThickSkin());
            r.RegisterByKey("DR-10", new SalamenceDragonWind());
            r.RegisterByKey("DR-11", new ShedinjaWonderGuard());
            r.RegisterByKey("DR-12", new ComboMon(new EnergyFromDiscardToHandMon("Power Generation", 2), new ScorchingSmokeMon()));
            r.RegisterByKey("DR-13", new ComboMon(new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Rend", 30, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("DR-14", new ComboMon(new CardsPAR2.FlipConditionMon("Dazzle Blast", SpecialCondition.Confused), new HealActiveMon("Healing Wave", 10)));
            r.RegisterByKey("DR-15", new SandGuardMon(20, new FlipDiscardSelfMon("Air Slash", null)));
            r.RegisterByKey("DR-16", new ComboMon(new SynchronizedSearchMon(), new BreakingImpactMon()));
            r.RegisterByKey("DR-17", new MagnetonMagneticField());
            r.RegisterByKey("DR-18", new NinjaskLooseShell());
            r.RegisterByKey("DR-19", new IntimidatingFangMon(new DiscardForBonusMon("Dragon Flame", 20)));
            r.RegisterByKey("DR-20", new CardsMEW1.WithdrawMon("Scrunch"));
            r.RegisterByKey("DR-21", new ComboMon(new PickOnMon(),
                new CardsFST2.ConditionBonusMon("Power Count", 30, c => c.MyPokemon().Sum(m => c.EnergyCount(m)) < c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("DR-22", new ComboMon(new DifferentEnergyMon("Quick Charge"), new FlipEachOpponentMon("Dragon Spark", 20)));

            r.RegisterByKey("DR-89", new AmpharosExConductivity());
            r.RegisterByKey("DR-90", new DragoniteExCallForPower());
            r.RegisterByKey("DR-91", new ComboMon(new CardsRSB.MagnitudeMon(), new RecoilMon("Double-edge", 50)));
            r.RegisterByKey("DR-92", new ComboMon(new CardsN3.GeneticMemoryMon(), new CardsSS.WaterExtraMon("Hydrocannon", 20)));
            r.RegisterByKey("DR-93", new ComboMon(new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep),
                new DiscardTypesMon("Mist Ball", EnergyType.Fire, EnergyType.Water)));
            r.RegisterByKey("DR-94", new ComboMon(new EnergyStreamMon(), new CardsTWM1.SelfDiscardMon("Luster Purge", 3)));
            r.RegisterByKey("DR-95", new ComboMon(new MeltingMountainMon(), new LavaFlowMon()));
            r.RegisterByKey("DR-96", new MukExToxicGas());
            r.RegisterByKey("DR-97", new ComboMon(new SpiralGrowthMon(), new DragonBurstMon()));
            r.RegisterByKey("DR-98", new Cards30C.SelfCounterBonusMon("Rage", 10));
            r.RegisterByKey("DR-99", new ComboMon(new CardsUPR3.SmokescreenMon("Smokescreen", 1), new FlipDiscardSelfMon("Fireworks", EnergyType.Fire)));
            r.RegisterByKey("DR-100", new ComboMon(new CollectTypeMon("Collect Fire", EnergyType.Fire, 2), new FlamePillarMon()));
        }

        // ================================================================== Shared (EX era)

        /// <summary>
        /// Intimidating Fang (Poké-Body): while this Pokémon is your Active Pokémon, damage the opponent's attacks do to your Pokémon
        /// is 10 less, before Weakness and Resistance.
        /// </summary>
        internal sealed class IntimidatingFangMon : Mon
        {
            private readonly Mon attacks;

            public IntimidatingFangMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target == null || info.Target.Owner != src.Player) return damage;
                if (info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (info.IgnoreDefenderEffects && info.TargetIsActive) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>The printed damage, and N to 1 of the opponent's Benched Pokémon (no Weakness or Resistance): Mud Splash, Dark Mind.</summary>
        internal sealed class AlsoOneBenchedMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public AlsoOneBenchedMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>"Attach up to N [kind of] Energy cards from your hand to your Pokémon in any way you like" (Energy Shower, Speed Growth).</summary>
        internal sealed class AttachFromHandMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly int max;

            public AttachFromHandMon(string attack, Func<CardDef, bool> filter, int max)
            {
                this.attack = attack;
                this.filter = filter;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var cards = ctx.Me.Hand.Where(c => filter(c.Def)).ToList();
                if (cards.Count > 0) ctx.AttachCardsAnyWay(cards, Math.Min(max, cards.Count), (c, m) => true, attack + ": attach Energy from your hand");
                return true;
            }
        }

        /// <summary>"Search your discard pile for up to N basic Energy cards, show them and put them into your hand" (Power Generation).</summary>
        internal sealed class EnergyFromDiscardToHandMon : Mon
        {
            private readonly string attack;
            private readonly int max;

            public EnergyFromDiscardToHandMon(string attack, int max)
            {
                this.attack = attack;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0) return true;
                int n = Math.Min(max, basics.Count);
                ctx.DiscardToHand(ctx.ChooseCards(basics, 1, n, attack + ": choose up to " + n + " basic Energy cards"));
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, discard a [type] Energy card (any Energy card if no type) attached to this Pokémon" (Air Slash, Fireworks).</summary>
        internal sealed class FlipDiscardSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType? type;

            public FlipDiscardSelfMon(string attack, EnergyType? type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads || !ctx.InPlay(me)) return true;
                if (type.HasValue) ctx.DiscardEnergyFrom(me, 1, attack + ": discard a " + type.Value + " Energy card", e => CardsHIF1.Provides(ctx, me, e, type.Value));
                else ctx.DiscardEnergyFrom(me, 1, attack + ": discard an Energy card from " + me.Name);
                return true;
            }
        }

        /// <summary>"Remove N damage from your Active Pokémon" after the damage (Healing Wave: 1-on-1, the attacker).</summary>
        internal sealed class HealActiveMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealActiveMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var active = ctx.Me.Active;
                if (active != null && active.DamageCounters > 0) ctx.Heal(active, amount);
                return true;
            }
        }

        /// <summary>"You may discard an Energy card attached to this Pokémon. If you do, N more damage" (Dragon Flame).</summary>
        internal sealed class DiscardForBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public DiscardForBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int damage = atk.Damage;
                if (!CardsBS1.CopyingWithMetronome && me.Energy.Count > 0 && ctx.AskYesNo(attack + ": discard an Energy card from " + me.Name + " for " + bonus + " more damage?")
                    && ctx.DiscardEnergyFrom(me, 1, attack + ": discard which Energy card?") > 0)
                    damage += bonus;
                ctx.DamageDefending(damage);
                return true;
            }
        }

        /// <summary>Up to 3 (or max) basic Energy cards of different types from your deck into your hand (Quick Charge).</summary>
        internal sealed class DifferentEnergyMon : Mon
        {
            private readonly string attack;
            private readonly int max;

            public DifferentEnergyMon(string attack) : this(attack, 3) { }

            public DifferentEnergyMon(string attack, int upTo)
            {
                this.attack = attack;
                max = upTo;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var taken = new List<CardInstance>();
                for (int i = 0; i < max; i++)
                {
                    var types = new HashSet<EnergyType>(taken.Select(c => c.Def.EnergyProvides));
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                        attack + ": choose a basic Energy card of a type you haven't taken (" + (i + 1) + " of up to " + max + ")");
                    if (found.Count == 0) break;
                    taken.AddRange(found);
                }
                ctx.Me.Hand.AddRange(taken);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, N damage to each of your opponent's Pokémon" (Dragon Spark); tails, nothing.</summary>
        internal sealed class FlipEachOpponentMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FlipEachOpponentMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(amount);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Discard 1 Energy for each of these types from this Pokémon after the damage (Mist Ball: Fire and Water; Dragon Wave: Water and Lightning).</summary>
        internal sealed class DiscardTypesMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType[] types;

            public DiscardTypesMon(string attack, params EnergyType[] types)
            {
                this.attack = attack;
                this.types = types;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                foreach (var type in types)
                {
                    var wanted = type;
                    ctx.DiscardEnergyFrom(me, 1, attack + ": discard a " + wanted + " Energy from " + me.Name, e => CardsHIF1.Provides(ctx, me, e, wanted));
                }
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, attach up to N [type] Energy cards from your discard pile to this Pokémon" (Collect Fire).</summary>
        internal sealed class CollectTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int max;

            public CollectTypeMon(string attack, EnergyType type, int max)
            {
                this.attack = attack;
                this.type = type;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (cards.Count == 0 || !ctx.InPlay(me) || !ctx.Flip()) return true;
                int n = Math.Min(max, cards.Count);
                var picks = cards.Count == n ? cards : ctx.ChooseCards(cards, n, n, attack + ": attach which " + type + " Energy cards?");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Pokémon: 20 damage, or 40 if it [condition]" (Special Circuit, Extra Circuit).</summary>
        internal sealed class CircuitMon : Mon
        {
            private readonly string attack;
            private readonly Func<EffectContext, PokemonInPlay, bool> more;

            public CircuitMon(string attack, Func<EffectContext, PokemonInPlay, bool> more)
            {
                this.attack = attack;
                this.more = more;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, attack + ": choose 1 of your opponent's Pokémon to damage");
                if (target != null) ctx.DamageTarget(target, more(ctx, target) ? 40 : atk.Damage);
                return true;
            }
        }

        /// <summary>Cheer On: remove 1 damage counter from each of your Pokémon.</summary>
        internal sealed class CheerOnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cheer On") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Bad News: with 6 or more cards in the opponent's hand, you pick cards there without looking until they have 5; those are discarded.</summary>
        private sealed class BadNewsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bad News") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                while (ctx.Opp.Hand.Count > 5)
                    if (!TefPokemon.DiscardRandomFromOpponentHand(ctx)) break;
                return true;
            }
        }

        /// <summary>
        /// Dragon Dance: during your next turn, this Pokémon's attacks do 40 more damage to the Defending Pokémon (before Weakness and
        /// Resistance; 1-on-1: "any of your current Active Pokémon" is this one). Dragon Song: Asleep.
        /// </summary>
        private sealed class AltariaDragonDance : Mon
        {
            private const string Dance = "Dragon Dance";
            private static readonly Mon Song = new ConditionAttackMon("Dragon Song", SpecialCondition.Asleep);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                int turn;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack || !me.Markers.TryGetValue(Dance, out turn) || turn != src.MyLastTurn || !src.IsMyTurn) return damage;
                return damage + 40;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == Dance)
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[Dance] = ctx.Turn;
                    return true;
                }
                return Song.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Power Pinchers (Poké-Body): while Crawdaunt is your Active Pokémon, its attacks do 10 more damage to the Defending Pokémon (before Weakness and Resistance).</summary>
        private sealed class CrawdauntPowerPinchers : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive) return damage;
                return src.Engine.CurrentSourceKind == EffectSourceKind.Attack && damage > 0 ? damage + 10 : damage;
            }
        }

        /// <summary>Mind Trip: if Grumpig and the Defending Pokémon have the same amount of Energy attached, the Defending Pokémon is Confused.</summary>
        private sealed class MindTripMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Trip") return false;
                var defending = ctx.Defending;
                bool same = defending != null && ctx.EnergyCount(ctx.Pokemon) == ctx.EnergyCount(defending);
                ctx.DamageDefending(atk.Damage);
                if (same && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Thick Skin (Poké-Body): Roselia can't be affected by any Special Conditions. Speed Growth: up to 2 Grass Energy cards from your hand. Sleep Powder.</summary>
        private sealed class RoseliaThickSkin : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new AttachFromHandMon("Speed Growth", d => CardsRS.BasicOf(d, EnergyType.Grass), 2),
                new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target != null && target == src.Pokemon;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Dragon Wind (Poké-Power): once during your turn, while Salamence is your Active Pokémon, switch 1 of the opponent's Benched
        /// Pokémon with the Defending Pokémon; not with a Special Condition. Agility. Dragon Claw.
        /// </summary>
        private sealed class SalamenceDragonWind : Mon
        {
            private const string Power = "Dragon Wind";
            private static readonly Mon Agility = new DodgeMon("Agility", false);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.PokemonIsActive && ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.GustOpponent(Power + ": choose the Benched Pokémon to switch in"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Agility.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wonder Guard (Poké-Body): prevent all effects of attacks, damage included, done to Shedinja by the opponent's Evolved
        /// Pokémon and Pokémon-ex. Damage Curse: 1 damage counter, plus 1 for each on Shedinja, on the Defending Pokémon.
        /// </summary>
        private sealed class ShedinjaWonderGuard : Mon
        {
            private static bool Guarded(EffectContext src, PokemonInPlay attacker)
            {
                return attacker != null && attacker.Owner != src.Player && (CardsE2.IsEvolved(attacker) || attacker.Def.IsOldEx);
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return Guarded(src, info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Attack && Guarded(src, src.Engine.CurrentSourcePokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Curse") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 1 + ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>Scorching Smoke: the Defending Pokémon is Burned; flip a coin, tails, discard a Fire Energy card attached to Torkoal.</summary>
        private sealed class ScorchingSmokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Smoke") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Scorching Smoke: discard a Fire Energy card", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Sand Guard (Poké-Body): when the opponent's attack would damage Flygon (after Weakness and Resistance), flip a coin; heads, 20 less.</summary>
        private sealed class SandGuardMon : Mon
        {
            private readonly int amount;
            private readonly Mon attacks;

            public SandGuardMon(int amount, Mon attacks)
            {
                this.amount = amount;
                this.attacks = attacks;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || damage <= 0) return damage;
                if (src.State.CurrentPlayer == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                Announce(src, "Sand Guard");
                return src.Flip() ? Math.Max(0, damage - amount) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Synchronized Search: with the same amount of Energy as the Defending Pokémon, any 1 card from your deck into your hand.</summary>
        private sealed class SynchronizedSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Synchronized Search") return false;
                var defending = ctx.Defending;
                if (defending == null || ctx.EnergyCount(ctx.Pokemon) != ctx.EnergyCount(defending) || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => true, 1, "Synchronized Search: choose any 1 card", false);
                return true;
            }
        }

        /// <summary>Breaking Impact: 10 damage for each Colorless in the chosen opponent's Pokémon's Retreat Cost (after effects on it).</summary>
        private sealed class BreakingImpactMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breaking Impact") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Breaking Impact: choose 1 of your opponent's Pokémon");
                if (target != null) ctx.DamageTarget(target, 10 * ctx.Engine.RetreatCostOf(target));
                return true;
            }
        }

        /// <summary>
        /// Magnetic Field (Poké-Power): once during your turn, with basic Energy cards in your discard pile, discard a card from your
        /// hand, then up to 2 basic Energy cards (not that one) from your discard pile into your hand; not with a Special Condition.
        /// Magnetic Force: 10 times the Energy on all your Pokémon.
        /// </summary>
        private sealed class MagnetonMagneticField : Mon
        {
            private const string Power = "Magnetic Field";
            private static readonly Mon Force = new CardsASR1.TimesMon("Magnetic Force", c => c.MyPokemon().Sum(m => c.EnergyCount(m)));

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Count > 0 && ctx.BasicEnergyInDiscard().Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return;
                var gone = ctx.ChooseCards(hand, 1, 1, Power + ": discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, gone);
                var basics = ctx.BasicEnergyInDiscard().Where(c => c != gone).ToList();
                if (basics.Count == 0) return;
                int n = Math.Min(2, basics.Count);
                ctx.DiscardToHand(ctx.ChooseCards(basics, 1, n, Power + ": put up to " + n + " basic Energy cards into your hand"));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Force.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Loose Shell (Poké-Power): when you play Ninjask from your hand to evolve, you may put Shedinja from your deck onto your Bench
        /// (as a Basic Pokémon). Quick Touch: you may switch Ninjask with a Benched Pokémon and move any Grass Energy cards to it.
        /// </summary>
        private sealed class NinjaskLooseShell : Mon
        {
            private const string Power = "Loose Shell";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.BenchSpace > 0 && ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.Name == "Shedinja");
                if (!TriggerOnce(ctx, Power, ready, Power + ": put Shedinja from your deck onto your Bench?")) return;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Shedinja" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, Power + ": choose Shedinja");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Touch") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0 || !ctx.AskYesNo("Quick Touch: switch " + me.Name + " with 1 of your Benched Pokémon?")) return true;
                if (!ctx.SwitchSelfWithBench()) return true;
                var active = ctx.Me.Active;
                var grass = me.Energy.Where(e => CardsRS.BasicOf(e.Def, EnergyType.Grass)).ToList();
                if (active == null || active == me || grass.Count == 0) return true;
                foreach (var card in ctx.ChooseCards(grass, 0, grass.Count, "Quick Touch: move any number of Grass Energy cards to " + active.Name))
                    ctx.Engine.MoveEnergy(me, active, card);
                return true;
            }
        }

        /// <summary>Pick On: with 6 or more cards in the opponent's hand, look at it and choose cards until they have 5; those go into their deck.</summary>
        private sealed class PickOnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pick On") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int extra = ctx.Opp.Hand.Count - 5;
                if (extra <= 0) return true;
                var hand = ctx.Opp.Hand.ToList();
                var picks = ctx.ChooseCards(hand, extra, extra, "Pick On: choose " + extra + " cards for your opponent to shuffle into their deck");
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log("Pick On: " + picks.Count + " cards were shuffled into " + ctx.Engine.Possessive(ctx.Opponent) + " deck.");
                return true;
            }
        }

        /// <summary>
        /// Conductivity (Poké-Body): whenever the opponent attaches an Energy card from their hand to 1 of their Pokémon, 1 damage
        /// counter on that Pokémon (only 1, even with more than 1 Ampharos ex). Gigavolt: heads 30 more, tails Paralyzed.
        /// </summary>
        private sealed class AmpharosExConductivity : Mon
        {
            private const string Body = "Conductivity";

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target == null || src.State.FindPokemon(target.Uid) != target) return;
                var first = src.MyPokemon(m => m.Name == "Ampharos ex" && !src.Engine.AbilityBlocked(m, Body)).FirstOrDefault();
                if (first != me) return;
                Announce(src, Body);
                src.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gigavolt") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Call for Power (Poké-Power): as often as you like during your turn, move an Energy card from 1 of your other Pokémon to
        /// Dragonite ex; not with a Special Condition. Dragon Wave: discard a Water and a Lightning Energy. Giant Tail: tails, nothing.
        /// </summary>
        private sealed class DragoniteExCallForPower : Mon
        {
            private const string Power = "Call for Power";
            private static readonly Mon Attacks = new ComboMon(new DiscardTypesMon("Dragon Wave", EnergyType.Water, EnergyType.Lightning), new TailsNothingMon("Giant Tail"));

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.Energy.Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && Donors(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move an Energy card from which Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var card = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, Power + ": move which Energy card?")[0];
                    return ctx.Engine.MoveEnergy(from, me, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Stream: flip a coin; heads, a basic Energy card from your discard pile onto Latios ex.</summary>
        private sealed class EnergyStreamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Stream") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.BasicEnergyInDiscard();
                if (basics.Count == 0 || !ctx.InPlay(me) || !ctx.Flip()) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Energy Stream: attach which basic Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Melting Mountain: discard the top card of your deck; a basic Energy card goes onto Magcargo ex instead.</summary>
        private sealed class MeltingMountainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Melting Mountain") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                if (gone.Count == 1 && gone[0].Def.IsBasicEnergy && ctx.InPlay(me) && ctx.Me.Discard.Remove(gone[0])) ctx.Engine.AttachEnergyCard(gone[0], me);
                return true;
            }
        }

        /// <summary>Lava Flow: you may discard any number of basic Energy cards attached to Magcargo ex; 20 more for each.</summary>
        private sealed class LavaFlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Flow") return false;
                var me = ctx.Pokemon;
                int n = 0;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                if (!CardsBS1.CopyingWithMetronome && basics.Count > 0)
                    foreach (var card in ctx.ChooseCards(basics, 0, basics.Count, "Lava Flow: discard any number of basic Energy cards (20 more damage each)"))
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(atk.Damage + 20 * n);
                return true;
            }
        }

        /// <summary>
        /// Toxic Gas (Poké-Body): while Muk ex is your Active Pokémon, every Poké-Power and Poké-Body other than Toxic Gas is ignored
        /// (both players'; Abilities and old Pokémon Powers aren't Poké-Powers). Poison Breath. Slimy Water: 10 more per Colorless in
        /// the Defending Pokémon's Retreat Cost.
        /// </summary>
        private sealed class MukExToxicGas : Mon
        {
            private const string Gas = "Toxic Gas";
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Poison Breath", SpecialCondition.Poisoned),
                new CardsASR1.PlusMon("Slimy Water", 10, c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending)));

            /// <summary>Toxic Gas works: Muk ex is Active and nothing that stops Poké-Bodies (other than another Toxic Gas, which leaves Toxic Gas alone) stops it.</summary>
            private static bool Working(EffectContext src)
            {
                var me = src.Pokemon;
                return me != null && src.IsActive(me) && !GameEngine.Imprisoned(me) && !src.Engine.PokeBodyStoppedByEffect(me, Gas);
            }

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon) { return mon != src.Pokemon && Working(src); }

            public override bool StopsPokeBodiesOf(EffectContext src, PokemonInPlay mon, string bodyName) { return bodyName != Gas && mon != src.Pokemon && Working(src); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral Growth: flip until tails; a basic Energy card from your discard pile onto Rayquaza ex for each heads.</summary>
        private sealed class SpiralGrowthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Growth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                int heads = ctx.FlipUntilTails();
                for (int i = 0; i < heads && ctx.InPlay(me); i++)
                {
                    var basics = ctx.BasicEnergyInDiscard();
                    if (basics.Count == 0) break;
                    var card = ctx.ChooseCards(basics, 1, 1, "Spiral Growth: attach which basic Energy card? (" + (i + 1) + " of " + heads + ")")[0];
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                }
                return true;
            }
        }

        /// <summary>Dragon Burst: discard all Fire Energy or all Lightning Energy attached to Rayquaza ex; 40 times the Energy discarded.</summary>
        private sealed class DragonBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Burst") return false;
                var me = ctx.Pokemon;
                var fire = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)).ToList();
                var lightning = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList();
                List<CardInstance> chosen;
                if (fire.Count == 0) chosen = lightning;
                else if (lightning.Count == 0) chosen = fire;
                else chosen = ctx.ChooseOption("Dragon Burst: discard which Energy?", "All Fire Energy (" + fire.Count + ")", "All Lightning Energy (" + lightning.Count + ")") == 0 ? fire : lightning;
                int n = 0;
                foreach (var card in chosen) if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Flame Pillar: you may discard a Fire Energy card attached to Charizard; if you do, 30 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class FlamePillarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Pillar") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Opp.Bench.Count == 0) return true;
                var fire = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire)).ToList();
                if (fire.Count == 0 || !ctx.AskYesNo("Flame Pillar: discard a Fire Energy card for 30 damage to 1 of your opponent's Benched Pokémon?")) return true;
                var card = ctx.ChooseCards(fire, 1, 1, "Flame Pillar: discard which Fire Energy card?")[0];
                if (ctx.Engine.DiscardEnergy(me, card)) ctx.DamageOneBenched(30);
                return true;
            }
        }
    }
}
