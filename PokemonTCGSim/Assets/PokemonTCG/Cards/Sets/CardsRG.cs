using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// FireRed &amp; LeafGreen Rare Pokémon and Pokémon-ex (RG 1-30, 104-116; v162). Unlimited only. Charmander 113 is Dragon's (a
    /// print). 1-on-1: "each Defending Pokémon" is the Defending Pokémon; Acid Sampler works with no other Active Pokémon.
    /// </summary>
    public static class CardsRG
    {
        private static readonly string[] NidoFamily = { "Nidoran ♀", "Nidorina", "Nidoqueen", "Nidoran ♂", "Nidorino" };
        private static readonly string[] BondsFamily = { "Nidoran ♀", "Nidorina", "Nidoran ♂", "Nidorino", "Nidoking" };

        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("RG-1", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned),
                new CardsASR1.PlusMon("Link Needle", 30, c => c.MyPokemon(m => m != c.Pokemon && m.Name == "Beedrill").Count)));
            r.RegisterByKey("RG-2", new ButterfreeSoothDust());
            r.RegisterByKey("RG-3", new CardsSS.SafeguardExMon(new ConditionAttackMon("Cold Breath", SpecialCondition.Asleep)));
            r.RegisterByKey("RG-4", new DittoFormVariation());
            r.RegisterByKey("RG-5", new ComboMon(new PsychicExchangeMon(), new CardsSS.EnergyCoinsMon("Big Eggsplosion", 99)));
            r.RegisterByKey("RG-6", new ComboMon(new DrawAttackMon("Fetch", 1), new FlipBonusMon("One-Two Punch", 20)));
            r.RegisterByKey("RG-7", new ComboMon(new CardsSSB.AnyOpponentTextMon("Linear Attack", 30),
                new CardsASR1.PlusMon("Vengeance", 10, c => Math.Min(6, c.Me.Discard.Count(x => x.Def.IsPokemon)))));
            r.RegisterByKey("RG-8", new NidokingPowerGene());
            r.RegisterByKey("RG-9", new NidoqueenFamilyBonds());
            r.RegisterByKey("RG-10", new PidgeotQuickSearch());
            r.RegisterByKey("RG-11", new PoliwrathSpiral());
            r.RegisterByKey("RG-12", new ComboMon(new TypeFromDeckToSelfMon("Recharge", EnergyType.Lightning, 2), new ThunderReflectionMon()));
            r.RegisterByKey("RG-13", new RapidashFieryAura());
            r.RegisterByKey("RG-14", new SlowbroStrangeBehavior());
            r.RegisterByKey("RG-15", new SnorlaxRestUp());
            r.RegisterByKey("RG-16", new ComboMon(new CardsMAB.KnockOverMon(), new RampageMon()));
            r.RegisterByKey("RG-17", new VictreebelAcidSampler());
            r.RegisterByKey("RG-18", new RecoilMon("Heat Tackle", 10));
            r.RegisterByKey("RG-19", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new EggSurpriseMon()));
            r.RegisterByKey("RG-20", new CardsFST1.TakeLessMon(20, new CardsSSB.DoubleBubbleMon()));
            r.RegisterByKey("RG-21", new DodrioRetreatAid());
            r.RegisterByKey("RG-22", new ComboMon(new CardsHL.NoWeaknessResistanceMon("Sonicboom"), new NoRetreatMon("Rumble")));
            r.RegisterByKey("RG-23", new ComboMon(new HoardMon(), CardsSS.ConditionBonus("Cross-Cut", 20, CardsE2.IsEvolved)));
            r.RegisterByKey("RG-24", new FearowFreeFlight());
            r.RegisterByKey("RG-25", new HypnoEerieAura());
            r.RegisterByKey("RG-26", new ComboMon(new TypeFromDeckToSelfMon("Salt Water", EnergyType.Water, 2), new CardsMA.ExtraBasicMon("Hyper Pump", 20, 2)));
            r.RegisterByKey("RG-27", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CardsCES1.FeintAttackMon("Speed Shot", 40)));
            r.RegisterByKey("RG-28", new TossMon());
            r.RegisterByKey("RG-29", new ScytherLeafRide());
            r.RegisterByKey("RG-30", new ComboMon(new VineTeaseMon(), new CardsSI.HeadsTailsConditionMon("Wiggle", SpecialCondition.Confused, SpecialCondition.Poisoned)));

            r.RegisterByKey("RG-104", new BlastoiseExEnergyRain());
            r.RegisterByKey("RG-105", new CharizardExEnergyFlame());
            r.RegisterByKey("RG-106", new CardsTRR.CopyAttackMon("Metronome", false, c => c.Defending == null ? new List<PokemonInPlay>() : new List<PokemonInPlay> { c.Defending }, "the Defending Pokémon"));
            r.RegisterByKey("RG-107", new ElectrodeExEnergyBomb());
            r.RegisterByKey("RG-108", new ComboMon(new PoltergeistMon(), new CardsFST2.ConditionBonusMon("Prize Count", 40, c => c.Me.Prizes.Count > c.Opp.Prizes.Count)));
            r.RegisterByKey("RG-109", new TwisterMon());
            r.RegisterByKey("RG-110", new MagicMon(true));
            r.RegisterByKey("RG-111", new MagicMon(false));
            r.RegisterByKey("RG-112", new VenusaurExEnergyTrans());
            r.RegisterByKey("RG-114", new LegendaryAscentMon("Articuno ex", EnergyType.Water, new ColdCrushMon("Cold Crush")));
            r.RegisterByKey("RG-115", new LegendaryAscentMon("Moltres ex", EnergyType.Fire, new DiscardForConditionAnyMon("Crushing Flames", SpecialCondition.Confused)));
            r.RegisterByKey("RG-116", new LegendaryAscentMon("Zapdos ex", EnergyType.Lightning, new CardsDR.DiscardForBonusMon("Electron Crush", 20)));
        }

        // ================================================================== Shared (EX era)

        /// <summary>"Search your deck for up to N [type] Energy cards and attach them to this Pokémon" (Recharge, Salt Water).</summary>
        internal sealed class TypeFromDeckToSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int max;

            public TypeFromDeckToSelfMon(string attack, EnergyType type, int max)
            {
                this.attack = attack;
                this.type = type;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var wanted = type;
                foreach (var card in ctx.SearchMyDeck(d => CardsRS.BasicOf(d, wanted), max, attack + ": choose up to " + max + " " + type + " Energy cards"))
                    if (!ctx.InPlay(me) || !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Protective Dust (Poké-Body): no effects of attacks other than damage reach this Pokémon.</summary>
        internal sealed class ProtectiveDustMon : Mon
        {
            private readonly Mon attacks;

            public ProtectiveDustMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Thick Skin (Poké-Body): this Pokémon can't be affected by any Special Conditions.</summary>
        internal sealed class ThickSkinMon : Mon
        {
            private readonly Mon attacks;

            public ThickSkinMon(Mon attacks) { this.attacks = attacks; }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition) { return target != null && target == src.Pokemon; }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Retreat Cost is 0 [while it has Energy / while it has none]" (Leaf Ride, Floating Electrons, Free Flight).</summary>
        internal sealed class EnergyRetreatMon : Mon
        {
            private readonly bool withEnergy;
            private readonly Mon attacks;

            public EnergyRetreatMon(bool withEnergy, Mon attacks)
            {
                this.withEnergy = withEnergy;
                this.attacks = attacks;
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return cost;
                return (mon.Energy.Count > 0) == withEnergy ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"You may discard an Energy card attached to this Pokémon. If you do, the Defending Pokémon is now [condition]" (Crushing Flames).</summary>
        internal sealed class DiscardForConditionAnyMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition condition;

            public DiscardForConditionAnyMon(string attack, SpecialCondition condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.AskYesNo(attack + ": discard an Energy card from " + me.Name + " to make the Defending Pokémon " + condition + "?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, attack + ": discard which Energy card?") > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>Cold Crush: you may discard an Energy card from this Pokémon; if you do, the opponent discards an Energy card from the Defending Pokémon.</summary>
        internal sealed class ColdCrushMon : Mon
        {
            private readonly string attack;

            public ColdCrushMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo(attack + ": discard an Energy card from " + me.Name + " so your opponent discards one from " + defending.Name + "?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, attack + ": discard which Energy card?") == 0 || !ctx.CanAffect(defending)) return true;
                var pick = ctx.Engine.ChooseCards(ctx.Opponent, defending.Energy.ToList(), 1, 1, attack + ": discard an Energy card from " + defending.Name, ctx.Source);
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(defending, pick[0]);
                return true;
            }
        }

        /// <summary>
        /// Legendary Ascent (Poké-Power): when you put this Pokémon from your hand onto your Bench, you may switch it with your Active
        /// Pokémon, and then move any number of basic [type] Energy cards from your other Pokémon to it.
        /// </summary>
        internal sealed class LegendaryAscentMon : Mon
        {
            private const string Power = "Legendary Ascent";
            private readonly string self;
            private readonly EnergyType type;
            private readonly Mon attacks;

            public LegendaryAscentMon(string self, EnergyType type, Mon attacks)
            {
                this.self = self;
                this.type = type;
                this.attacks = attacks;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !TriggerOnce(ctx, Power, ctx.Me.Active != null && ctx.Me.Bench.Contains(me), Power + ": switch " + self + " with your Active Pokémon?")) return;
                if (!ctx.Engine.SwitchActive(ctx.Player, me)) return;
                var wanted = type;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon(m => m != me))
                    foreach (var e in mon.Energy.Where(c => CardsRS.BasicOf(c.Def, wanted))) holders[e] = mon;
                if (holders.Count == 0) return;
                foreach (var card in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, Power + ": move any number of basic " + type + " Energy cards to " + self))
                    ctx.Engine.MoveEnergy(holders[card], me, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Pokémon

        /// <summary>Sooth Dust (Poké-Body): while Butterfree is your Active Pokémon, 1 damage counter comes off each of your Pokémon between turns. Whirlwind. Gust.</summary>
        private sealed class ButterfreeSoothDust : Mon
        {
            private static readonly Mon Wind = new CardsE3B.OpponentSwitchesMon("Whirlwind");

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                var hurt = src.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                Announce(src, "Sooth Dust");
                foreach (var mon in hurt) src.Heal(mon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wind.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Form Variation (Poké-Power): once during your turn, a Basic Pokémon (not a Pokémon-ex or Ditto) from your discard pile takes
        /// Ditto's place with everything on it; Ditto goes to the discard pile. Energy Ball: 10 more per extra Energy (up to 20).
        /// </summary>
        private sealed class DittoFormVariation : Mon
        {
            private const string Power = "Form Variation";
            private static readonly Mon Ball = new CardsRS.ExtraEnergyMon("Energy Ball", 2);

            private static bool Fits(CardDef d) { return d.IsBasicPokemon && !d.IsOldEx && d.Name != "Ditto"; }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Discard.Any(c => Fits(c.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var options = ctx.Me.Discard.Where(c => Fits(c.Def)).ToList();
                if (options.Count == 0 || me == null) return;
                var pick = ctx.ChooseCards(options, 1, 1, Power + ": switch Ditto with which Basic Pokémon from your discard pile?")[0];
                var old = me.Top;
                if (!ctx.Me.Discard.Remove(pick)) return;
                me.Stack[me.Stack.Count - 1] = pick;
                ctx.Me.Discard.Add(old);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " switched Ditto with " + pick.Def.Name + " from their discard pile (Form Variation).", pick.Id, me.Uid);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ball.HandleAttack(ctx, atk); }
        }

        /// <summary>Psychic Exchange: shuffle your hand into your deck and draw up to 8 cards.</summary>
        private sealed class PsychicExchangeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Exchange") return false;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int max = Math.Min(8, ctx.Me.Deck.Count);
                if (max > 0) ctx.Draw(max == 1 ? 1 : ctx.AskNumber(0, max, "Psychic Exchange: draw how many cards (up to " + max + ")?"));
                return true;
            }
        }

        /// <summary>
        /// Power Gene (Poké-Body): while Nidoking is in play, the attacks of your Nidoran ♀, Nidorina, Nidoqueen, Nidoran ♂ and Nidorino do
        /// 10 more to the Defending Pokémon. Earth Poison: Poisoned if it already had damage counters. Bound Crush: 60 to 1 of the
        /// opponent's Pokémon, not next turn.
        /// </summary>
        private sealed class NidokingPowerGene : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !NidoFamily.Contains(a.Name)) return damage;
                if (info.Target == null || info.Target.Owner == src.Player || !info.TargetIsActive || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return damage + 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Earth Poison")
                {
                    var defending = ctx.Defending;
                    bool damaged = defending != null && defending.DamageCounters > 0;
                    ctx.DamageDefending(atk.Damage);
                    if (damaged && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    return true;
                }
                if (atk.Name != "Bound Crush") return false;
                ctx.DamageAnyOpponent(Math.Max(atk.Damage, 60));
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Family Bonds (Poké-Body): while Nidoqueen is in play, your Nidoran ♀, Nidorina, Nidoran ♂, Nidorino and Nidoking have no Retreat Cost. Toxic. Power Lariat.</summary>
        private sealed class NidoqueenFamilyBonds : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsBS1.ToxicMon("Toxic", 2),
                new CardsASR1.PlusMon("Power Lariat", 10, c => c.MyPokemon(CardsE2.IsEvolved).Count));

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && BondsFamily.Contains(mon.Name) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Quick Search (Poké-Power): once during your turn, any 1 card from your deck into your hand; only 1 Quick Search each turn; not with a Special Condition. Clutch.</summary>
        private sealed class PidgeotQuickSearch : Mon
        {
            private const string Power = "Quick Search";
            private static readonly Mon Clutch = new NoRetreatMon("Clutch");

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.SearchToHand(d => true, 1, Power + ": choose any 1 card", false); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Clutch.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral (Poké-Body): while Poliwrath is your Active Pokémon, the opponent's Confused Pokémon can't retreat. Split Spiral Punch: Confused. Mega Throw: 30 more against a Pokémon-ex.</summary>
        private sealed class PoliwrathSpiral : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused),
                CardsSS.ConditionBonus("Mega Throw", 30, d => d.Def.IsOldEx));

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player && mon.Has(SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Thunder Reflection: you may move any number of Lightning Energy cards from Raichu to another of your Pokémon.</summary>
        private sealed class ThunderReflectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunder Reflection") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var others = ctx.MyPokemon(m => m != me);
                var cards = me.Energy.Where(e => CardsHIF1.Provides(ctx, me, e, EnergyType.Lightning)).ToList();
                if (!ctx.InPlay(me) || others.Count == 0 || cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, 0, cards.Count, "Thunder Reflection: move any number of Lightning Energy cards from " + me.Name);
                if (picks.Count == 0) return true;
                var to = ctx.ChoosePokemon(others, "Thunder Reflection: move them to which Pokémon?");
                if (to != null) foreach (var card in picks) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Fiery Aura (Poké-Body): while Rapidash is your Active Pokémon, Burned Pokémon take 4 damage counters between turns instead of 2. Searing Flame: Burned. Rear Kick.</summary>
        private sealed class RapidashFieryAura : Mon
        {
            private static readonly Mon Flame = new ConditionAttackMon("Searing Flame", SpecialCondition.Burned);

            public override int ModifyBurnCounters(EffectContext src, PokemonInPlay mon, int counters)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) ? Math.Max(counters, 4) : counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flame.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Strange Behavior (Poké-Power): as often as you like during your turn, move 1 damage counter from another of your Pokémon to
        /// Slowbro if that doesn't Knock it Out; not with a Special Condition. Psyshock.
        /// </summary>
        private sealed class SlowbroStrangeBehavior : Mon
        {
            private const string Power = "Strange Behavior";
            private static readonly Mon Shock = new ParalyzeFlipMon("Psyshock");

            private static List<PokemonInPlay> Donors(EffectContext ctx) { return ctx.MyPokemon(m => m != ctx.Pokemon && m.DamageCounters > 0); }

            private static bool CanMove(EffectContext ctx) { return ctx.InPlay(ctx.Pokemon) && ctx.RemainingHp(ctx.Pokemon) > 10 && Donors(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var me = ctx.Pokemon;
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a damage counter from which Pokémon to " + me.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    return from != null && ctx.Engine.MoveDamageCounters(from, me, 1, Power) > 0;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shock.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Rest Up (Poké-Body): if Snorlax is still Asleep between turns, 2 damage counters come off it. Collapse: Snorlax is now
        /// Asleep. Toss and Turn: 30 more if Snorlax is Asleep, and it can be used while Snorlax is Asleep.
        /// </summary>
        private sealed class SnorlaxRestUp : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !me.Has(SpecialCondition.Asleep) || me.DamageCounters == 0) return;
                Announce(src, "Rest Up");
                src.Heal(me, 20);
            }

            public override bool AllowsAttackWhileAsleepOrParalyzed(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Paralyzed);
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                if (me == null || !me.Has(SpecialCondition.Asleep)) return true;
                return ctx.Attack(attackIndex).Name == "Toss and Turn";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Collapse")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                    return true;
                }
                if (atk.Name != "Toss and Turn") return false;
                ctx.DamageDefending(atk.Damage + (me.Has(SpecialCondition.Asleep) ? 30 : 0));
                return true;
            }
        }

        /// <summary>Rampage: 10 more per damage counter on Tauros; then flip a coin, tails, Tauros is Confused.</summary>
        private sealed class RampageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rampage") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Acid Sampler (Poké-Body): while Victreebel is your Active Pokémon, 1 damage counter on the Defending Pokémon between turns. Acid: can't retreat.</summary>
        private sealed class VictreebelAcidSampler : Mon
        {
            private static readonly Mon Acid = new NoRetreatMon("Acid");

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                var theirs = src.Opp.Active;
                if (me == null || !src.IsActive(me) || theirs == null) return;
                Announce(src, "Acid Sampler");
                src.Engine.PlaceDamageCounters(theirs, 1, "Acid Sampler");
            }

            public override int CheckupAffects(EffectContext src) { return 1 - src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Acid.HandleAttack(ctx, atk); }
        }

        /// <summary>Egg Surprise: heads, 50 damage to the Defending Pokémon; tails, 5 damage counters come off Chansey.</summary>
        private sealed class EggSurpriseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Egg Surprise") return false;
                var me = ctx.Pokemon;
                if (ctx.Flip()) ctx.DamageDefending(50);
                else if (me.DamageCounters > 0) ctx.Heal(me, 50);
                return true;
            }
        }

        /// <summary>Retreat Aid (Poké-Body): while Dodrio is on your Bench, your Active Pokémon (not a Pokémon-ex or Baby Pokémon) retreats for 2 less. Tri Attack.</summary>
        private sealed class DodrioRetreatAid : Mon
        {
            private static readonly Mon Tri = new CoinsMon("Tri Attack", 3);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon.Owner != src.Player || src.IsActive(me) || !src.IsActive(mon)) return cost;
                if (mon.Def.IsOldEx || mon.Def.IsBaby) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tri.HandleAttack(ctx, atk); }
        }

        /// <summary>Hoard: up to 2 Pokémon Tool cards from your deck onto your Pokémon that have none.</summary>
        private sealed class HoardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hoard") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsTool, 2, "Hoard: choose up to 2 Pokémon Tool cards");
                foreach (var tool in found)
                {
                    var spots = ctx.MyPokemon(m => m.Tools.Count == 0 && ctx.Engine.CanAttachToolTo(tool, m));
                    var target = spots.Count == 0 ? null : ctx.ChoosePokemon(spots, "Hoard: attach " + tool.Def.Name + " to which Pokémon?");
                    if (target != null) ctx.Engine.AttachTool(tool, target);
                    else ctx.Me.Deck.Add(tool);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Free Flight (Poké-Body): with no Energy attached, Fearow's Retreat Cost is 0. Shot Air: 20 to 1 of the opponent's Benched Pokémon. Drill Peck.</summary>
        private sealed class FearowFreeFlight : Mon
        {
            private static readonly Mon Shot = new CardsDR.AlsoOneBenchedMon("Shot Air", 20);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shot.HandleAttack(ctx, atk); }
        }

        /// <summary>Eerie Aura (Poké-Body): while Hypno is your Active Pokémon, each Pokémon still Asleep between turns gets 2 damage counters (both players'). Hypnotic Ray.</summary>
        private sealed class HypnoEerieAura : Mon
        {
            private static readonly Mon Ray = new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me)) return;
                for (int side = 0; side < 2; side++)
                    foreach (var mon in src.State.Players[side].InPlay().ToList())
                        if (mon.Has(SpecialCondition.Asleep) && src.InPlay(mon)) src.Engine.PlaceDamageCounters(mon, 2, "Eerie Aura");
            }

            public override int CheckupAffects(EffectContext src) { return -1; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>Toss: discard any number of Technical Machine and Pokémon Tool cards from your hand; 30 for each.</summary>
        private sealed class TossMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toss") return false;
                int n = 0;
                var tools = ctx.Me.Hand.Where(c => c.Def.IsTool).ToList();
                if (!CardsBS1.CopyingWithMetronome && tools.Count > 0)
                    foreach (var card in ctx.ChooseCards(tools, 0, tools.Count, "Toss: discard any number of Technical Machine and Pokémon Tool cards (30 damage each)"))
                        if (ctx.Me.Hand.Contains(card))
                        {
                            ctx.Engine.DiscardFromHand(ctx.Player, card);
                            n++;
                        }
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Leaf Ride (Poké-Body): with any Energy attached, Scyther's Retreat Cost is 0. Fury Cutter: 3 coins, 10 / 20 / 40 more.</summary>
        private sealed class ScytherLeafRide : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Energy.Count > 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage + (heads == 3 ? 40 : 10 * heads));
                return true;
            }
        }

        /// <summary>Vine Tease: look at your Prize cards and switch 1 of them with the top card of your deck; nothing with an empty deck.</summary>
        private sealed class VineTeaseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vine Tease") return false;
                if (ctx.Me.Deck.Count == 0 || ctx.Me.Prizes.Count == 0) return Nothing(ctx);
                var prizes = ctx.Me.Prizes.ToList();
                ctx.Engine.NoteLookedAt(prizes, "Your Prize cards", ctx.Player);
                var pick = ctx.ChooseCards(prizes, 1, 1, "Vine Tease: switch which Prize card with the top card of your deck?")[0];
                int at = ctx.Me.Prizes.IndexOf(pick);
                if (at < 0) return true;
                var top = ctx.Me.Deck[0];
                ctx.Me.Prizes[at] = top;
                ctx.Me.Deck[0] = pick;
                ctx.Log("Vine Tease: a Prize card was switched with the top card of " + ctx.Engine.Possessive(ctx.Player) + " deck.");
                return true;
            }
        }

        /// <summary>
        /// Energy Rain (Poké-Power): as often as you like during your turn, attach a Water Energy card from your hand to 1 of your
        /// Pokémon and put 1 damage counter on it; not with a Special Condition. Hyper Whirlpool: flip until tails; for each heads the
        /// opponent discards an Energy card from the Defending Pokémon.
        /// </summary>
        private sealed class BlastoiseExEnergyRain : Mon
        {
            private const string Power = "Energy Rain";

            private static bool Water(CardDef d) { return CardsRS.BasicOf(d, EnergyType.Water); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Power) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Any(c => Water(c.Def));
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => ctx.Me.Hand.Any(c => Water(c.Def)), offerDone =>
                {
                    var card = ctx.Me.Hand.First(c => Water(c.Def));
                    var target = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(), Power + ": attach a Water Energy card to which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (target == null || !ctx.Me.Hand.Remove(card)) return false;
                    if (!ctx.Engine.AttachEnergyCard(card, target, true)) return false;
                    if (ctx.InPlay(target)) ctx.PlaceCounters(target, 1);
                    return true;
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hyper Whirlpool") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipUntilTails();
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    var pick = ctx.Engine.ChooseCards(ctx.Opponent, defending.Energy.ToList(), 1, 1, "Hyper Whirlpool: discard an Energy card from " + defending.Name, ctx.Source);
                    if (pick.Count == 0 || !ctx.Engine.DiscardEnergy(defending, pick[0])) break;
                }
                return true;
            }
        }

        /// <summary>
        /// Energy Flame (Poké-Body): every Energy on Charizard ex is Fire. Slash. Burn Down: discard 5 Fire Energy; the damage isn't
        /// affected by Weakness, Resistance or effects on the Defending Pokémon.
        /// </summary>
        private sealed class CharizardExEnergyFlame : Mon
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Fire);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Down") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                    ctx.DiscardEnergyFrom(me, 5, "Burn Down: discard 5 Fire Energy", e => CardsHIF1.Provides(ctx, me, e, EnergyType.Fire));
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>
        /// Extra Energy Bomb (Poké-Power): once during your turn, discard Electrode ex and everything on it (a Knock Out: the opponent
        /// takes Prize cards); then attach up to 5 Energy cards from your discard pile to your Pokémon (not Pokémon-ex) in any way you
        /// like. Not with a Special Condition. Crush and Burn: discard any Energy from your Pokémon, 20 more each.
        /// </summary>
        private sealed class ElectrodeExEnergyBomb : Mon
        {
            private const string Power = "Extra Energy Bomb";
            private const string Marker = "Extra Energy Bomb used";

            protected override string Activated { get { return Power; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == Power; }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Pokemon.Markers[Marker] = ctx.Turn;
                if (!ctx.Engine.KnockOut(ctx.Pokemon, Power)) ctx.Pokemon.Markers.Remove(Marker);
            }

            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                if (me == null || !me.Markers.ContainsKey(Marker)) return;
                me.Markers.Remove(Marker);
                var energy = src.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0 || src.MyPokemon(m => !m.Def.IsOldEx).Count == 0) return;
                src.AttachCardsAnyWay(energy, Math.Min(5, energy.Count), (c, m) => !m.Def.IsOldEx, Power + ": attach Energy cards from your discard pile");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crush and Burn") return false;
                int n = 0;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var e in mon.Energy) holders[e] = mon;
                    if (holders.Count > 0)
                        foreach (var card in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Crush and Burn: discard any number of Energy from your Pokémon (20 more damage each)"))
                            if (ctx.Engine.DiscardEnergy(holders[card], card)) n++;
                }
                ctx.DamageDefending(atk.Damage + 20 * n);
                return true;
            }
        }

        /// <summary>Poltergeist: look at the opponent's hand; 10 more for each Trainer card there.</summary>
        private sealed class PoltergeistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poltergeist") return false;
                var hand = ctx.Opp.Hand.ToList();
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Poltergeist: your opponent's hand", ctx.Source);
                ctx.DamageDefending(atk.Damage + 10 * hand.Count(c => c.Def.IsTrainer));
                return true;
            }
        }

        /// <summary>Twister: 2 coins; nothing on 2 tails; otherwise the damage, and an Energy card discarded from the Defending Pokémon for each heads. Dragon Rage.</summary>
        private sealed class TwisterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twister") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    if (ctx.DiscardEnergyFrom(defending, 1, "Twister: discard an Energy card from " + defending.Name) == 0) break;
                }
                return true;
            }
        }

        /// <summary>Magic Odds / Magic Evens (Poké-Body): attack damage to Mr. Mime ex of 10, 30, 50... (or 20, 40, 60...) up to 170 (180) is prevented. Breakdown.</summary>
        private sealed class MagicMon : Mon
        {
            private readonly bool odds;

            public MagicMon(bool odds) { this.odds = odds; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return false;
                if (damage <= 0 || damage % 10 != 0) return false;
                int tens = damage / 10;
                return odds ? tens % 2 == 1 && damage <= 170 : tens % 2 == 0 && damage <= 180;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breakdown") return false;
                int n = ctx.Opp.Hand.Count;
                if (n > 0 && ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, n);
                return true;
            }
        }

        /// <summary>Energy Trans (Poké-Power): as often as you like, move a Grass Energy card from 1 of your Pokémon to another. Pollen Hazard: Poisoned, Burned and Confused. Solarbeam.</summary>
        private sealed class VenusaurExEnergyTrans : Mon
        {
            private const string Trans = "Energy Trans";

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Trans) return false;
                return ctx.IsMyTurn && CardsE1.NoCondition(ctx.Pokemon) && CardsCEL.EnergyTransReady(ctx, EnergyType.Grass);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { CardsCEL.UseEnergyTrans(ctx, Trans, EnergyType.Grass); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pollen Hazard") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
