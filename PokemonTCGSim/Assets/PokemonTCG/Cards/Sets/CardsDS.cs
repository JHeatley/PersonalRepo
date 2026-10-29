using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Delta Species Rare Pokémon, Pokémon-ex and Pokémon ★ (DS 1-18, 108-113; v169). Unlimited only. δ Delta Species Pokémon
    /// (CardDef.IsDeltaSpecies) are often 2 types at once, and both count all the time (Compendium: Metal Crusher, Gang Up). Metagross
    /// 11 is the 30th Celebration's (a print). 1-on-1: "1 of the Defending Pokémon" is the Defending Pokémon.
    /// </summary>
    public static class CardsDS
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("DS-1", new BeedrillFinalSting());
            r.RegisterByKey("DS-2", new ComboMon(new RadarJamMon("Radar Jam"), new TargetAttackMon()));
            r.RegisterByKey("DS-3", new DragoniteDeltaCharge());
            r.RegisterByKey("DS-4", new EspeonDeltaHeal());
            r.RegisterByKey("DS-5", new ComboMon(new DeltaSearchMon(), new ReturnEnergyMon("Return Burn", ReturnBurn)));
            r.RegisterByKey("DS-6", new GardevoirEnergyJump());
            r.RegisterByKey("DS-7", new ComboMon(new DeltaSearchMon(), new ReturnEnergyMon("Return Spark", ReturnSpark)));
            r.RegisterByKey("DS-8", new DeltaAuraMon("Extra Crush", "Latios", new ComboMon(new CardsTWM1.HealSelfMon("Spiral Drain", 20),
                new NothingUnlessMon("Extra Crush", c => c.Opp.InPlay().Any(m => m.Def.IsOldEx)))));
            r.RegisterByKey("DS-9", new DeltaAuraMon("Psychic Force", "Latias",
                new NothingUnlessMon("Psychic Force", c => c.Opp.InPlay().Any(m => m.Def.Stage == Stage.Stage2 && m.Stack.Count > 1))));
            r.RegisterByKey("DS-10", new ComboMon(new EnergyBoneMon(), new BaseIfMon("Metal Crusher", 90, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Metal))));
            r.RegisterByKey("DS-12", new MewtwoDeltaSwitch());
            r.RegisterByKey("DS-13", new RayquazaDeltaGuard());
            r.RegisterByKey("DS-14", new ComboMon(new CardsTRR.FireDanceMon(), new CardsSUM1.DiscardTypeFromSelfMon("Delta Blast", EnergyType.Metal, 1)));
            r.RegisterByKey("DS-15", new StarmieMetalNavigation());
            r.RegisterByKey("DS-16", new TyranitarCrushDraw());
            r.RegisterByKey("DS-17", new UmbreonDeltaMoon());
            r.RegisterByKey("DS-18", new ComboMon(new DeltaSearchMon(), new ReturnEnergyMon("Return Wave", ReturnWave)));

            r.RegisterByKey("DS-108", new EvolutionaryMon("Evolutionary Flame", EvolutionaryFlame, c => c.Defending != null,
                new ComboMon(new ShieldAttackMon("Flame Screen", 20), new RecoilMon("Heat Tackle", 10))));
            r.RegisterByKey("DS-109", new EvolutionaryMon("Evolutionary Thunder", EvolutionaryThunder, c => c.Opp.HasPokemonInPlay,
                new ComboMon(new CardsASR1.PlusMon("Second Bite", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters),
                    new CardsSUM1.DiscardTypeFromSelfMon("Thunder Blast", EnergyType.Lightning, 1))));
            r.RegisterByKey("DS-110", new EvolutionaryMon("Evolutionary Swirl", EvolutionarySwirl, c => true, new CardsEM.PiercingMon("Fastwave")));
            r.RegisterByKey("DS-111", new ComboMon(new CriticalCollectionMon(EnergyType.Fighting), new CardsSUM1.DiscardTypeFromSelfMon("Ground Slash", EnergyType.Fighting, 1)));
            r.RegisterByKey("DS-112", new ComboMon(new CriticalCollectionMon(EnergyType.Water), new CardsDAA1.FlipCantAttackMon("Sheer Cold")));
            r.RegisterByKey("DS-113", new ComboMon(new CriticalCollectionMon(EnergyType.Metal), new CardsASC.FlipDiscardDefendingEnergyMon("Hyper Beam")));
        }

        // ================================================================== Shared (δ and Holon)

        /// <summary>"Pokémon that has δ on its card".</summary>
        internal static bool Delta(PokemonInPlay mon) { return mon != null && mon.Def.IsDeltaSpecies; }

        /// <summary>A Holon Energy card (FF, GL, WP...). A Holon's Pokémon attached as Energy isn't one (Compendium, 2005-11-03).</summary>
        internal static bool IsHolonEnergy(CardDef d) { return d != null && d.IsSpecialEnergy && d.Name.StartsWith("Holon Energy", StringComparison.Ordinal); }

        internal static int HolonEnergyCount(PokemonInPlay mon) { return mon == null ? 0 : mon.Energy.Count(e => IsHolonEnergy(e.Def)); }

        /// <summary>"A [type] Energy card": the basic Energy card of that type.</summary>
        internal static bool BasicOf(CardDef d, EnergyType type) { return CardsRS.BasicOf(d, type); }

        /// <summary>"Once during your turn (before your attack) ... This power can't be used if this Pokémon is affected by a Special Condition."</summary>
        internal static bool PowerReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon); }

        /// <summary>This Pokémon's own Poké-Body or Poké-Power works right now (it isn't blocked).</summary>
        internal static bool Works(EffectContext ctx, PokemonInPlay mon, string ability) { return mon != null && !ctx.Engine.AbilityBlocked(mon, ability); }

        /// <summary>
        /// A Holon's Pokémon: besides being a Pokémon, it may be attached from the hand as the Energy attachment for the turn (a
        /// Special Energy card while attached: CardDef.AttachesAsEnergy). Holon's Electrode and Magneton go only onto a Pokémon that
        /// already has an Energy card attached, return one of those to the hand, and provide every type but 2 Energy at a time;
        /// Holon's Magnemite and Voltorb provide [C].
        /// </summary>
        internal sealed class HolonMon : Mon
        {
            private readonly Mon attacks;
            private readonly bool twoOfEvery;

            public HolonMon(bool twoOfEvery, Mon attacks)
            {
                this.twoOfEvery = twoOfEvery;
                this.attacks = attacks;
            }

            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return !twoOfEvery || (target != null && target.Energy.Count > 0); }

            public override void OnAttachedFromHand(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (!twoOfEvery || mon == null) return;
                var others = mon.Energy.Where(e => e != ctx.Card).ToList();
                if (others.Count == 0) return;
                var pick = ctx.ChooseCards(others, 1, 1, ctx.Card.Def.Name + ": return which Energy card attached to " + mon.Name + " to your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(mon, pick);
            }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return twoOfEvery ? new List<int> { EnergyMask.AnyType, EnergyMask.AnyType } : new List<int> { EnergyMask.Colorless };
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Radar Jam / Disconnect: the opponent can't play Trainer cards other than Supporters during their next turn.</summary>
        internal sealed class RadarJamMon : Mon
        {
            private readonly string attack;

            public RadarJamMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new NonSupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards other than Supporters during their next turn (" + attack + ")",
                    ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class NonSupporterLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public NonSupporterLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card != null && card.Def.IsTrainer && !card.Def.IsSupporter;
            }
        }

        /// <summary>"If [condition] isn't met, this attack does nothing" (Extra Crush, Psychic Force).</summary>
        internal sealed class NothingUnlessMon : Mon
        {
            private readonly string attack;
            private readonly Func<EffectContext, bool> condition;

            public NothingUnlessMon(string attack, Func<EffectContext, bool> condition)
            {
                this.attack = attack;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (!condition(ctx)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>"If [condition], this attack's base damage is N" (Metal Crusher).</summary>
        internal sealed class BaseIfMon : Mon
        {
            private readonly string attack;
            private readonly int damage;
            private readonly Func<EffectContext, bool> condition;

            public BaseIfMon(string attack, int damage, Func<EffectContext, bool> condition)
            {
                this.attack = attack;
                this.damage = damage;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(condition(ctx) ? damage : atk.Damage);
                return true;
            }
        }

        /// <summary>Delta Search: a Holon Energy card from your deck onto this Pokémon.</summary>
        internal sealed class DeltaSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delta Search") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsHolonEnergy, 1, "Delta Search: choose a Holon Energy card");
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Holon Search: a Holon Energy card from your deck onto 1 of your Pokémon.</summary>
        internal sealed class HolonSearchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Holon Search") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(IsHolonEnergy, 1, "Holon Search: choose a Holon Energy card");
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Holon Search: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Return Burn / Spark / Wave: "You may return an Energy card attached to this Pokémon to your hand. If you do, ..." (after the
        /// attack's damage).
        /// </summary>
        internal sealed class ReturnEnergyMon : Mon
        {
            private readonly string attack;
            private readonly Action<EffectContext, PokemonInPlay> then;

            public ReturnEnergyMon(string attack, Action<EffectContext, PokemonInPlay> then)
            {
                this.attack = attack;
                this.then = then;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me) || me.Energy.Count == 0) return true;
                if (!ctx.AskYesNo(attack + ": return an Energy card attached to " + me.Name + " to your hand?")) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, attack + ": return which Energy card?")[0];
                if (ctx.Engine.ReturnEnergyToHand(me, card)) then(ctx, defending);
                return true;
            }
        }

        private static void ReturnBurn(EffectContext ctx, PokemonInPlay defending)
        {
            if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
        }

        private static void ReturnSpark(EffectContext ctx, PokemonInPlay defending) { ctx.DamageOneBenched(20); }

        private static void ReturnWave(EffectContext ctx, PokemonInPlay defending)
        {
            if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
            var card = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Return Wave: return which Energy card attached to " + defending.Name + " to your opponent's hand?")[0];
            ctx.Engine.ReturnEnergyToHand(defending, card);
        }

        /// <summary>
        /// Evolutionary Flame / Thunder / Swirl (Poké-Power): once during your turn, when you play this Pokémon from your hand to
        /// evolve 1 of your Pokémon, you may do the effect.
        /// </summary>
        internal sealed class EvolutionaryMon : Mon
        {
            private readonly string power;
            private readonly Action<EffectContext> effect;
            private readonly Func<EffectContext, bool> ready;
            private readonly Mon attacks;

            public EvolutionaryMon(string power, Action<EffectContext> effect, Func<EffectContext, bool> ready, Mon attacks)
            {
                this.power = power;
                this.effect = effect;
                this.ready = ready;
                this.attacks = attacks;
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, power, ready(ctx), power + ": use it?")) return;
                effect(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        private static void EvolutionaryFlame(EffectContext ctx)
        {
            var target = ctx.Defending;
            if (target == null || !ctx.CanAffect(target)) return;
            ctx.ApplyCondition(target, SpecialCondition.Burned);
            if (ctx.InPlay(target)) ctx.ApplyCondition(target, SpecialCondition.Confused);
        }

        private static void EvolutionaryThunder(EffectContext ctx)
        {
            foreach (var mon in ctx.OpponentPokemon())
                if (ctx.InPlay(mon) && ctx.CanAffect(mon)) ctx.PlaceCounters(mon, 1);
        }

        /// <summary>Evolutionary Swirl: the opponent shuffles their hand into their deck, then draws up to 4 cards (they choose how many).</summary>
        private static void EvolutionarySwirl(EffectContext ctx)
        {
            var opp = ctx.Opp;
            var hand = opp.Hand.ToList();
            opp.Hand.Clear();
            opp.Deck.AddRange(hand);
            ctx.Engine.ShuffleDeck(ctx.Opponent);
            ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + hand.Count + " cards from their hand into their deck (Evolutionary Swirl).");
            int max = Math.Min(4, opp.Deck.Count);
            if (max <= 0) return;
            int n = ctx.Engine.AskNumber(ctx.Opponent, 0, max, "Evolutionary Swirl: draw how many cards (up to 4)?", ctx.Source);
            if (n > 0) ctx.Engine.DrawCards(ctx.Opponent, n);
        }

        /// <summary>Critical Collection: up to [Prize cards the opponent has taken] [type] Energy cards from your discard pile onto this Pokémon.</summary>
        internal sealed class CriticalCollectionMon : Mon
        {
            private readonly EnergyType type;

            public CriticalCollectionMon(EnergyType type) { this.type = type; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Critical Collection") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                int n = ctx.OpponentPrizesTaken;
                var cards = ctx.Me.Discard.Where(c => BasicOf(c.Def, type)).ToList();
                if (n <= 0 || cards.Count == 0 || !ctx.InPlay(me)) return true;
                foreach (var card in ctx.ChooseCards(cards, 0, Math.Min(n, cards.Count), "Critical Collection: attach up to " + n + " " + type + " Energy cards to " + me.Name))
                    if (ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Discard.Add(card);
                return true;
            }
        }

        // ================================================================== Rares

        /// <summary>
        /// Final Sting (Poké-Power): once during your turn, you may Knock Out Beedrill; then the Defending Pokémon is Paralyzed and
        /// Poisoned with 2 damage counters between turns. Not with a Special Condition. Super Slash: 30 more to an Evolved Pokémon.
        /// </summary>
        private sealed class BeedrillFinalSting : Mon
        {
            private const string Power = "Final Sting";
            private static readonly Mon Slash = CardsSS.ConditionBonus("Super Slash", 30, CardsE2.IsEvolved);

            protected override string Activated { get { return Power; } }

            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == Power; }

            protected override bool AbilityReady(EffectContext ctx) { return PowerReady(ctx); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Engine.KnockOut(ctx.Pokemon, Power)) return;
                var target = ctx.Opp.Active;
                if (target == null || !ctx.CanAffect(target)) return;
                ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                if (ctx.InPlay(target) && ctx.ApplyCondition(target, SpecialCondition.Poisoned)) target.PoisonCounterBase = 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Slash.HandleAttack(ctx, atk); }
        }

        /// <summary>Target Attack: 40 to 1 of the opponent's Pokémon, 60 if it already has damage counters (1 is enough: Compendium).</summary>
        private sealed class TargetAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Target Attack") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Target Attack: choose 1 of your opponent's Pokémon (40, or 60 if it has damage counters)");
                if (target != null) ctx.DamageTarget(target, target.DamageCounters > 0 ? 60 : 40);
                return true;
            }
        }

        /// <summary>Delta Charge (Poké-Power): once during your turn, a Lightning Energy card from your discard pile onto 1 of your Benched Pokémon. Agility.</summary>
        private sealed class DragoniteDeltaCharge : Mon
        {
            private const string Power = "Delta Charge";
            private static readonly Mon Agility = new DodgeMon("Agility", false);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return PowerReady(ctx) && ctx.Me.Bench.Count > 0 && ctx.Me.Discard.Any(c => BasicOf(c.Def, EnergyType.Lightning));
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = ctx.Me.Discard.Where(c => BasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (cards.Count == 0 || ctx.Me.Bench.Count == 0) return;
                var card = ctx.ChooseCards(cards, 1, 1, Power + ": choose a Lightning Energy card")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), Power + ": attach it to which Benched Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card) && !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Discard.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Agility.HandleAttack(ctx, atk); }
        }

        /// <summary>Delta Heal (Poké-Power): once during your turn, 1 damage counter off each of your δ Pokémon; only 1 Delta Heal each turn. Psyshock.</summary>
        private sealed class EspeonDeltaHeal : Mon
        {
            private const string Power = "Delta Heal";
            private static readonly Mon Psyshock = new ParalyzeFlipMon("Psyshock");

            protected override string Activated { get { return Power; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerReady(ctx) && ctx.MyPokemon(m => Delta(m) && m.DamageCounters > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => Delta(m) && m.DamageCounters > 0)) ctx.Heal(mon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Psyshock.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Energy Jump (Poké-Power): once during your turn, move an Energy card attached to 1 of your Pokémon to another of your
        /// Pokémon. Psychic Rage: 10 for each damage counter on Gardevoir, to 1 of the opponent's Pokémon. Black Magic: 20 more for
        /// each of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class GardevoirEnergyJump : Mon
        {
            private const string Power = "Energy Jump";
            private static readonly Mon Magic = new CardsASR1.PlusMon("Black Magic", 20, c => c.Opp.Bench.Count);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerReady(ctx) && ctx.MyPokemon().Count >= 2 && ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Energy.Count > 0), Power + ": move an Energy card from which Pokémon?");
                if (from == null) return;
                var card = ctx.ChooseCards(from.Energy.ToList(), 1, 1, Power + ": move which Energy card?")[0];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), Power + ": move " + card.Def.Name + " to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Rage") return Magic.HandleAttack(ctx, atk);
                ctx.DamageAnyOpponent(10 * ctx.Pokemon.DamageCounters);
                return true;
            }
        }

        /// <summary>
        /// Delta Aura (Poké-Body): with the partner (Latios or Latios ex for Latias, Latias or Latias ex for Latios) in play, this
        /// Pokémon's [attack] costs [L][M][C]. Only for this Pokémon: an attack copying it costs the printed amount (Compendium).
        /// </summary>
        internal sealed class DeltaAuraMon : Mon
        {
            private readonly string attack;
            private readonly string partner;
            private readonly Mon attacks;

            public DeltaAuraMon(string attack, string partner, Mon attacks)
            {
                this.attack = attack;
                this.partner = partner;
                this.attacks = attacks;
            }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef a, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon || a == null || a.Name != attack) return;
                if (!src.Me.InPlay().Any(m => m.Name == partner || m.Name == partner + " ex")) return;
                cost.Clear();
                cost.Add(EnergyType.Lightning);
                cost.Add(EnergyType.Metal);
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Bone: 20 to each of up to [Energy on Marowak] of the opponent's Pokémon (no W/R for Benched).</summary>
        private sealed class EnergyBoneMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Bone") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                var all = ctx.OpponentPokemon();
                if (n <= 0 || all.Count == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(all, 1, Math.Min(n, all.Count), "Energy Bone: choose up to " + n + " of your opponent's Pokémon (20 damage each)"))
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, 20);
                return true;
            }
        }

        /// <summary>
        /// Delta Switch (Poké-Power): once during your turn, when you put Mewtwo from your hand onto your Bench, you may move any
        /// number of basic Energy cards attached to your Pokémon to your other Pokémon (not Mewtwo). Energy Burst.
        /// </summary>
        private sealed class MewtwoDeltaSwitch : Mon
        {
            private const string Power = "Delta Switch";
            private static readonly Mon Burst = new CardsCRZ1.BothActiveEnergyMon("Energy Burst", 10);

            private static List<PokemonInPlay> Donors(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy) && ctx.MyPokemon(o => o != m && o != ctx.Pokemon).Count > 0);
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Pokemon == null || !TriggerOnce(ctx, Power, Donors(ctx).Count > 0, Power + ": move basic Energy cards between your other Pokémon?")) return;
                RepeatableAbility.Repeat(ctx, () => Donors(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Donors(ctx), Power + ": move a basic Energy card from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var card = ctx.ChooseCards(from.Energy.Where(e => e.Def.IsBasicEnergy).ToList(), 1, 1, Power + ": move which Energy card?")[0];
                    var to = ctx.ChoosePokemon(ctx.MyPokemon(o => o != from && o != ctx.Pokemon), Power + ": move " + card.Def.Name + " to which Pokémon?");
                    return to != null && ctx.Engine.MoveEnergy(from, to, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Burst.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Delta Guard (Poké-Body): with a Holon Energy card attached, the effect of Lightning Storm (7 damage counters on Rayquaza) is
        /// ignored; the damage is still done (Compendium). Power Blow: 10 for each Energy on Rayquaza.
        /// </summary>
        private sealed class RayquazaDeltaGuard : Mon
        {
            private static readonly Mon Blow = new CardsASR1.TimesMon("Power Blow", c => c.EnergyCount(c.Pokemon));

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Storm") return Blow.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(me)) return true;
                if (HolonEnergyCount(me) > 0 && Works(ctx, me, "Delta Guard")) ctx.Log("Delta Guard: Lightning Storm's damage counters aren't put on " + me.Name + ".");
                else ctx.PlaceCounters(me, 7);
                return true;
            }
        }

        /// <summary>Metal Navigation (Poké-Power): once during your turn, a Metal Energy card (only one named Metal Energy: Compendium) from your deck onto Starmie. Collect. Energy Loop.</summary>
        private sealed class StarmieMetalNavigation : Mon
        {
            private const string Power = "Metal Navigation";
            private static readonly Mon Attacks = new ComboMon(new DrawAttackMon("Collect", 3), new EnergyLoopMon("Energy Loop"));

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsEnergy && d.Name == "Metal Energy", 1, Power + ": choose a Metal Energy card");
                foreach (var card in found) ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Loop: return an Energy card attached to this Pokémon to your hand (after the damage).</summary>
        internal sealed class EnergyLoopMon : Mon
        {
            private readonly string attack;

            public EnergyLoopMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (CardsBS1.CopyingWithMetronome || !ctx.InPlay(me) || me.Energy.Count == 0) return true;
                var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, attack + ": return which Energy card to your hand?")[0];
                ctx.Engine.ReturnEnergyToHand(me, card);
                return true;
            }
        }

        /// <summary>
        /// Crush Draw (Poké-Power): once during your turn, reveal the top card of your deck; a basic Energy card is attached to 1 of
        /// your Pokémon, anything else goes back on top. Not with a Special Condition. Delta Crush: discard an Energy for 20 more.
        /// </summary>
        private sealed class TyranitarCrushDraw : Mon
        {
            private const string Power = "Crush Draw";
            private static readonly Mon Crush = new CardsDR.DiscardForBonusMon("Delta Crush", 20);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerReady(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.Me.Deck[0];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { top }, "revealed the top card of their deck (Crush Draw)");
                if (!top.Def.IsBasicEnergy)
                {
                    ctx.Log(top.Def.Name + " isn't a basic Energy card; it goes back on top of the deck.");
                    return;
                }
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), Power + ": attach " + top.Def.Name + " to which Pokémon?");
                if (target != null && ctx.Me.Deck.Remove(top) && !ctx.Engine.AttachEnergyCard(top, target)) ctx.Me.Deck.Insert(0, top);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crush.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Delta Moon (Poké-Body): when the opponent attaches a Special Energy card (a Holon's Pokémon attached as Energy too) from
        /// their hand to 1 of their Pokémon, 1 damage counter on that Pokémon. Only 1 Delta Moon counts each turn: several Umbreon
        /// don't add up (Compendium, 2006-05-18). Feint Attack: 30 to 1 of the opponent's Pokémon, ignoring W/R and all effects.
        /// </summary>
        private sealed class UmbreonDeltaMoon : Mon
        {
            private const string Body = "Delta Moon";
            private static readonly Mon Feint = new CardsCES1.FeintAttackMon("Feint Attack", 30);

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (energy == null || target == null || !(energy.Def.IsSpecialEnergy || energy.Def.AttachesAsEnergy) || src.NamedLimitUsedThisTurn(Body)) return;
                if (!src.InPlay(target)) return;
                src.MarkNamedLimit(Body);
                Announce(src, Body);
                src.PlaceCounters(target, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Feint.HandleAttack(ctx, atk); }
        }
    }
}
