using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Base Set (BS, v113): the Pokémon (1-69). Blastoise, Charizard, Venusaur and Pikachu are the Classic Collection cards;
    /// Caterpie, Metapod and Weedle are Evolutions'. Pokémon Powers can't be used while the Pokémon is Asleep, Confused or
    /// Paralyzed. Your rulings (v113): Conversion 1 / 2 last until that Pokémon evolves or leaves play; Mirror Move copies the
    /// damage and the Special Conditions the last attack gave Pidgeotto.
    /// </summary>
    public static class CardsBS1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BS-1", new AlakazamDamageSwap());
            r.RegisterByKey("BS-3", new ComboMon(new CardsMEW1.WithdrawMon("Scrunch"), new RecoilMon("Double-edge", 80)));
            r.RegisterByKey("BS-5", new ComboMon(new CardsPAR2.FlipConditionMon("Sing", SpecialCondition.Asleep), new MetronomeMon()));
            r.RegisterByKey("BS-6", new ParalyzeFlipMon("Bubblebeam"));
            r.RegisterByKey("BS-8", new MachampStrikesBack());
            r.RegisterByKey("BS-9", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new SelfdestructMon("Selfdestruct", 20, 80)));
            r.RegisterByKey("BS-10", new DiscardCostMon("Barrier", EnergyType.Psychic, 1, BarrierEffect, new EnergyCardBonusMon()));
            r.RegisterByKey("BS-11", new ComboMon(new ThrashMon("Thrash"), new ToxicMon("Toxic", 2)));
            r.RegisterByKey("BS-12", new DiscardCostMon("Fire Blast", EnergyType.Fire, 1, null, new LureMon("Lure")));
            r.RegisterByKey("BS-13", new ComboMon(new ExtraWaterMon("Water Gun"), new CardsDRI2.CrunchMon("Whirlpool")));
            r.RegisterByKey("BS-14", new ComboMon(new DodgeMon("Agility", false), new CardsFST1.FlipRecoilMon("Thunder", 30)));
            r.RegisterByKey("BS-16", new DiscardCostMon("Thunderbolt", null, -1, null, new CardsFST1.FlipRecoilMon("Thunder", 30)));
            r.RegisterByKey("BS-17", new ComboMon(new CoinsMon("Twineedle", 2), new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned)));
            r.RegisterByKey("BS-18", new ComboMon(new CoinsMon("Slam", 2), new CardsDRI2.CrunchMon("Hyper Beam")));
            r.RegisterByKey("BS-19", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10));
            r.RegisterByKey("BS-20", new ComboMon(new ParalyzeFlipMon("Thundershock"), new ThrashMon("Thunderpunch")));
            r.RegisterByKey("BS-21", new ElectrodeBuzzap());
            r.RegisterByKey("BS-22", new ComboMon(new CardsASC.PushDownMon("Whirlwind"), new MirrorMoveMon()));
            r.RegisterByKey("BS-23", new DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, new RecoilMon("Take Down", 30)));
            r.RegisterByKey("BS-24", new DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("BS-25", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("BS-27", new LeekSlapMon());
            r.RegisterByKey("BS-29", new HaunterDreamEater());
            r.RegisterByKey("BS-30", new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned));
            r.RegisterByKey("BS-31", new ComboMon(new CoinsMon("Doubleslap", 2), new CardsMEW1.CounterBonusMon("Meditate", 10)));
            r.RegisterByKey("BS-32", new DiscardCostMon("Recover", EnergyType.Psychic, 1, RecoverEffect, null));
            r.RegisterByKey("BS-33", new ComboMon(new CardsMEW1.WithdrawMon("Stiffen"), new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("BS-34", new ComboMon(new KarateChopMon(), new RecoilMon("Submission", 20)));
            r.RegisterByKey("BS-35", new CardsASR1.TimesMon("Flail", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("BS-36", new DiscardCostMon("Flamethrower", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("BS-37", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("BS-38", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CoinsMon("Doubleslap", 2)));
            r.RegisterByKey("BS-39", new PorygonConversion());
            r.RegisterByKey("BS-40", new SuperFangHalfMon());
            r.RegisterByKey("BS-42", new CardsMEW1.WithdrawMon("Withdraw"));
            r.RegisterByKey("BS-43", new ParalyzeFlipMon("Psyshock"));
            r.RegisterByKey("BS-44", new LeechSeedMon());
            r.RegisterByKey("BS-46", new DiscardCostMon("Ember", EnergyType.Fire, 1, null, null));
            r.RegisterByKey("BS-48", new CoinsMon("Fury Attack", 2));
            r.RegisterByKey("BS-49", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("BS-50", new GastlyDestinyBond());
            r.RegisterByKey("BS-51", new FoulGasMon("Foul Gas"));
            r.RegisterByKey("BS-53", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new SelfdestructMon("Selfdestruct", 10, 40)));
            r.RegisterByKey("BS-55", new TailsNothingMon("Horn Hazard"));
            r.RegisterByKey("BS-56", new CardsLOR1.HardenMon("Harden", 30));
            r.RegisterByKey("BS-57", new CardsASC.PushDownMon("Whirlwind"));
            r.RegisterByKey("BS-59", new ExtraWaterMon("Water Gun"));
            r.RegisterByKey("BS-62", new CardsMEG.SandAttackMon("Sand-attack"));
            r.RegisterByKey("BS-63", new ComboMon(new ParalyzeFlipMon("Bubble"), new CardsMEW1.WithdrawMon("Withdraw")));
            r.RegisterByKey("BS-64", new DiscardCostMon("Recover", EnergyType.Water, 1, RecoverEffect, new ParalyzeFlipMon("Star Freeze")));
            r.RegisterByKey("BS-66", new ComboMon(new ParalyzeFlipMon("Bind"), new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("BS-68", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
        }

        // ================================================================== Shared

        /// <summary>Base Set Pokémon Powers: "This power can't be used if this Pokémon is Asleep, Confused, or Paralyzed."</summary>
        internal static bool PowerWorks(PokemonInPlay mon)
        {
            return mon != null && !mon.Has(SpecialCondition.Asleep) && !mon.Has(SpecialCondition.Confused) && !mon.Has(SpecialCondition.Paralyzed);
        }

        private static bool IsAbility(EffectContext ctx, int abilityIndex, string name)
        {
            var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
            return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == name;
        }

        /// <summary>Set while Metronome copies an attack: "except for its Energy cost and anything else required in order to use it" (no discarding).</summary>
        [ThreadStatic] internal static bool CopyingWithMetronome;

        /// <summary>
        /// "Discard N [type] Energy cards attached to this Pokémon in order to use this attack" (count -1: all of them, any type).
        /// The attack can't be used without them; the discard is a cost paid before the damage. Another attack comes from other.
        /// </summary>
        internal sealed class DiscardCostMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType? type;
            private readonly int count;
            private readonly Action<EffectContext, AttackDef> effect;
            private readonly Mon other;

            public DiscardCostMon(string attack, EnergyType? type, int count, Action<EffectContext, AttackDef> effect, Mon other)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
                this.effect = effect;
                this.other = other;
            }

            private List<CardInstance> Payable(EffectContext ctx, PokemonInPlay mon)
            {
                if (mon == null) return new List<CardInstance>();
                return mon.Energy.Where(c => !type.HasValue || CardsDRI1.ProvidesType(ctx, mon, c, type.Value)).ToList();
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var atk = ctx.Attack(attackIndex);
                if (atk.Name != attack || count < 0) return other == null || other.CanUseAttack(ctx, attackIndex);
                return Payable(ctx, ctx.Pokemon).Count >= count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return other != null && other.HandleAttack(ctx, atk);
                var me = ctx.Pokemon;
                if (!CopyingWithMetronome && me != null)
                {
                    if (count < 0) ctx.Engine.DiscardAllEnergy(me);
                    else
                    {
                        var pay = Payable(ctx, me);
                        if (pay.Count < count) return Nothing(ctx);
                        var picks = pay.Count == count ? pay : ctx.ChooseCards(pay, count, count, atk.Name + ": discard " + count + (type.HasValue ? " " + type.Value : "") + " Energy card" + (count == 1 ? "" : "s"));
                        foreach (var card in picks) ctx.Engine.DiscardEnergy(me, card);
                    }
                }
                if (effect != null) effect(ctx, atk);
                else ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Barrier: prevent all effects of attacks, including damage, done to this Pokémon during the opponent's next turn.</summary>
        private static void BarrierEffect(EffectContext ctx, AttackDef atk)
        {
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
        }

        /// <summary>Recover: remove all damage counters from this Pokémon.</summary>
        private static void RecoverEffect(EffectContext ctx, AttackDef atk)
        {
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            var me = ctx.Pokemon;
            if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
        }

        internal const string DestinyBondMarker = "Destiny Bond";

        /// <summary>Destiny Bond: if a Pokémon Knocks Out this Pokémon during the opponent's next turn, that Pokémon is Knocked Out.</summary>
        private static void DestinyBondEffect(EffectContext ctx, AttackDef atk)
        {
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[DestinyBondMarker] = ctx.Turn + 1;
        }

        /// <summary>Thrash / Thunderpunch: flip a coin; heads, 10 more damage; tails, this Pokémon does 10 damage to itself.</summary>
        internal sealed class ThrashMon : Mon
        {
            private readonly string attack;

            public ThrashMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(10);
                return true;
            }
        }

        /// <summary>Hydro Pump / Water Gun: 10 more for each Water Energy not used to pay the attack's cost (up to 2 extra).</summary>
        internal sealed class ExtraWaterMon : Mon
        {
            private readonly string attack;

            public ExtraWaterMon(string attack) { this.attack = attack; }

            internal static int Extra(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                int water = ctx.Engine.EnergyCountOfType(me, EnergyType.Water);
                int total = ctx.EnergyCount(me);
                int w = atk.Cost.Count(t => t == EnergyType.Water);
                int c = atk.Cost.Count(t => t == EnergyType.Colorless);
                int waterForColorless = Math.Max(0, c - (total - water));
                return Math.Max(0, Math.Min(2, water - w - waterForColorless));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + 10 * Extra(ctx, atk));
                return true;
            }
        }

        /// <summary>Selfdestruct: N to each Benched Pokémon (both players', no W/R), and this Pokémon does M damage to itself.</summary>
        internal sealed class SelfdestructMon : Mon
        {
            private readonly string attack;
            private readonly int bench;
            private readonly int self;

            public SelfdestructMon(string attack, int bench, int self)
            {
                this.attack = attack;
                this.bench = bench;
                this.self = self;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.ToList()) ctx.DamageTarget(mon, bench);
                foreach (var mon in ctx.Me.Bench.ToList()) ctx.DamageTarget(mon, bench);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(self);
                return true;
            }
        }

        // ================================================================== Pokémon Powers

        /// <summary>
        /// Damage Swap (Pokémon Power): as often as you like during your turn, move 1 damage counter from 1 of your Pokémon to
        /// another, as long as that doesn't Knock it Out. Confuse Ray: heads, Confused.
        /// </summary>
        private sealed class AlakazamDamageSwap : Mon
        {
            private const string Swap = "Damage Swap";
            private static readonly Mon Ray = new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused);

            private static List<PokemonInPlay> From(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0); }

            private static List<PokemonInPlay> To(EffectContext ctx, PokemonInPlay from) { return ctx.MyPokemon(m => m != from && ctx.RemainingHp(m) > 10); }

            private static bool CanMove(EffectContext ctx) { return From(ctx).Any(f => To(ctx, f).Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return IsAbility(ctx, abilityIndex, Swap) && ctx.IsMyTurn && PowerWorks(ctx.Pokemon) && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
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

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ray.HandleAttack(ctx, atk); }
        }

        /// <summary>Strikes Back (Pokémon Power): whenever the opponent's attack damages this Pokémon (even if Knocked Out), 10 damage to the Attacking Pokémon (no W/R).</summary>
        private sealed class MachampStrikesBack : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker) || !WorkedBeforeAttack(ctx, ctx.Pokemon)) return;
                Announce(ctx, "Strikes Back");
                ctx.Engine.PlaceDamageCounters(attacker, 1, "Strikes Back");
            }
        }

        /// <summary>"Not if this Pokémon was already Asleep, Confused or Paralyzed when your opponent attacked": conditions that attack gave it don't count.</summary>
        internal static bool WorkedBeforeAttack(EffectContext ctx, PokemonInPlay mon)
        {
            if (mon == null) return false;
            int turn, fresh = 0;
            if (mon.Markers.TryGetValue(PokemonInPlay.AttackConditionsTurnMarker, out turn) && turn == ctx.Turn)
                mon.Markers.TryGetValue(PokemonInPlay.AttackConditionsMarker, out fresh);
            foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed })
                if (mon.Has(c) && (fresh & (int)c) == 0) return false;
            return true;
        }

        /// <summary>
        /// Buzzap (Pokémon Power): Knock Out this Pokémon (the opponent takes a Prize card) and attach it to 1 of your other Pokémon
        /// as an Energy card that provides 2 Energy of the type you choose. It counts as Energy of that type (2000-03-30).
        /// </summary>
        private sealed class ElectrodeBuzzap : Mon
        {
            private const string Buzzap = "Buzzap";
            private const string TargetMarker = "Buzzap target";
            private static readonly EnergyType[] Types = { EnergyType.Grass, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning, EnergyType.Psychic, EnergyType.Fighting };
            private static readonly Mon Shock = new CardsFST1.FlipRecoilMon("Electric Shock", 10);

            private static string TypeKey(CardInstance card) { return "Buzzap type#" + card.Id; }

            protected override string Activated { get { return Buzzap; } }
            public override bool AbilityKnocksOutSelf(string abilityName) { return abilityName == Buzzap; }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me);
            }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Buzzap: attach " + ctx.Pokemon.Name + " as Energy to which of your Pokémon?");
                if (target == null) return;
                int pick = ctx.ChooseOption("Buzzap: it provides 2 Energy of which type?", Types.Select(t => t.ToString()).ToArray());
                if (pick < 0 || pick >= Types.Length) pick = 3;
                ctx.Me.NamedLimitUsedOnTurn[TypeKey(ctx.Card)] = (int)Types[pick];
                ctx.Pokemon.Markers[TargetMarker] = target.Uid;
                if (!ctx.Engine.KnockOut(ctx.Pokemon, Buzzap)) ctx.Pokemon.Markers.Remove(TargetMarker);
            }

            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                int uid;
                if (me == null || !me.Markers.TryGetValue(TargetMarker, out uid)) return;
                me.Markers.Remove(TargetMarker);
                var target = src.State.FindPokemon(uid);
                var card = src.Card;
                if (target == null || target.Owner != src.Player || card == null || !src.Me.Discard.Remove(card)) return;
                src.Engine.AttachEnergyCard(card, target);
                src.Log(card.Def.Name + " was attached to " + target.Name + " as an Energy card that provides 2 " + TypeOf(src.State, card) + " Energy (Buzzap).");
            }

            private static EnergyType TypeOf(GameState state, CardInstance card)
            {
                int t;
                return state.Players[card.Owner].NamedLimitUsedOnTurn.TryGetValue(TypeKey(card), out t) ? (EnergyType)t : EnergyType.Lightning;
            }

            /// <summary>Attached as Energy: 2 of the chosen type.</summary>
            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                var type = ctx.Card == null ? EnergyType.Lightning : TypeOf(ctx.State, ctx.Card);
                return new List<int> { EnergyMask.Of(type), EnergyMask.Of(type) };
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shock.HandleAttack(ctx, atk); }
        }

        // ================================================================== Attacks

        /// <summary>
        /// Metronome: choose 1 of the Defending Pokémon's attacks and copy it, except for its Energy cost and anything else required to
        /// use it (no discarding Energy for it); this Pokémon stays Colorless.
        /// </summary>
        internal sealed class MetronomeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metronome") return false;
                var defending = ctx.Defending;
                CardDef source;
                int index;
                if (defending == null || !ChooseCopiedAttack(ctx, defending, "Metronome: choose 1 of the Defending Pokémon's attacks to copy", out source, out index)) return Nothing(ctx);
                CopyingWithMetronome = true;
                try { UseCopiedAttack(ctx, source, index); }
                finally { CopyingWithMetronome = false; }
                return true;
            }
        }

        /// <summary>Toxic: the Defending Pokémon is now Poisoned and takes 2 damage counters from Poison instead of 1. Used for Toxic, Max Toxify, Poison Fang, Tainted Horn (the attack name and the numbers are passed in).</summary>
        internal sealed class ToxicMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public ToxicMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = counters;
                return true;
            }
        }

        /// <summary>Lure: if the opponent has any Benched Pokémon, switch 1 of them (your choice) with their Active Pokémon. Used for Lure, Taunt (the attack name is passed in).</summary>
        internal sealed class LureMon : Mon
        {
            private readonly string attack;

            public LureMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0) ctx.GustOpponent(attack + ": choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>
        /// Mirror Move: if this Pokémon was attacked last turn, do the final result of that attack on it to the Defending Pokémon. Your
        /// ruling (v113): the damage it took (no Weakness or Resistance again) and the Special Conditions that attack gave it.
        /// </summary>
        internal sealed class MirrorMoveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mirror Move") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int last = ctx.OpponentsLastTurn;
                int damage = me.AttackDamageTurn == last ? me.AttackDamageThatTurn : 0;
                int turn, flags = 0;
                if (me.Markers.TryGetValue(PokemonInPlay.AttackConditionsTurnMarker, out turn) && turn == last)
                    me.Markers.TryGetValue(PokemonInPlay.AttackConditionsMarker, out flags);
                if (defending == null || (damage <= 0 && flags == 0))
                {
                    ctx.Log("Mirror Move: " + me.Name + " wasn't attacked last turn, so nothing happens.");
                    return true;
                }
                if (damage > 0) ctx.DamageWith(defending, damage, false, false, false);
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned, SpecialCondition.Burned })
                    if ((flags & (int)c) != 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, c);
                return true;
            }
        }

        /// <summary>Leek Slap: flip a coin, tails nothing; either way it can't be used again while this Pokémon stays in play.</summary>
        private sealed class LeekSlapMon : Mon
        {
            private const string Used = "Leek Slap used";

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Leek Slap" || ctx.Pokemon == null || !ctx.Pokemon.Markers.ContainsKey(Used);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leek Slap") return false;
                if (ctx.InPlay(ctx.Pokemon)) ctx.Pokemon.Markers[Used] = 1;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Hypnosis: Asleep. Dream Eater: can't be used unless the Defending Pokémon is Asleep.</summary>
        private sealed class HaunterDreamEater : Mon
        {
            private static readonly Mon Hypnosis = new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Dream Eater") return true;
                return ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hypnosis.HandleAttack(ctx, atk); }
        }

        /// <summary>Karate Chop: the printed damage minus 10 for each damage counter on this Pokémon.</summary>
        internal sealed class KarateChopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Karate Chop") return false;
                int dmg = Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters);
                if (dmg > 0) ctx.DamageDefending(dmg);
                return true;
            }
        }

        /// <summary>
        /// Conversion 1: if the Defending Pokémon has a Weakness, you may change it to a type other than Colorless. Conversion 2: change
        /// this Pokémon's Resistance to a type other than Colorless. Your ruling (v113): until that Pokémon evolves or leaves play.
        /// </summary>
        internal sealed class PorygonConversion : Mon
        {
            private static readonly EnergyType[] Types = Enum.GetValues(typeof(EnergyType)).Cast<EnergyType>().Where(t => t != EnergyType.Colorless).ToArray();

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Conversion 1")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var defending = ctx.Defending;
                    if (defending == null || !ctx.Engine.WeaknessOf(defending).HasValue || !ctx.CanAffect(defending)) return true;
                    if (!ctx.AskYesNo("Conversion 1: change " + defending.Name + "'s Weakness?")) return true;
                    int pick = ctx.ChooseOption("Conversion 1: change its Weakness to which type?", Types.Select(t => t.ToString()).ToArray());
                    if (pick < 0 || pick >= Types.Length) return true;
                    defending.Markers[PokemonInPlay.ConversionWeaknessMarker] = (int)Types[pick];
                    ctx.Log(defending.Name + "'s Weakness is now " + Types[pick] + " (Conversion 1).");
                    return true;
                }
                if (atk.Name == "Conversion 2")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me)) return true;
                    int pick = ctx.ChooseOption("Conversion 2: change " + me.Name + "'s Resistance to which type?", Types.Select(t => t.ToString()).ToArray());
                    if (pick < 0 || pick >= Types.Length) return true;
                    me.Markers[PokemonInPlay.ConversionResistanceMarker] = (int)Types[pick];
                    ctx.Log(me.Name + "'s Resistance is now " + Types[pick] + " (Conversion 2).");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Super Fang: damage equal to half the Defending Pokémon's remaining HP, rounded up to the nearest 10 (Weakness and Resistance apply).</summary>
        internal sealed class SuperFangHalfMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Super Fang") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int remaining = Math.Max(0, ctx.RemainingHp(defending));
                int half = (remaining + 1) / 2;
                int dmg = (half + 9) / 10 * 10;
                if (dmg > 0) ctx.DamageDefending(dmg);
                return true;
            }
        }

        /// <summary>Leech Seed: unless all of its damage is prevented, remove 1 damage counter from this Pokémon.</summary>
        internal sealed class LeechSeedMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leech Seed") return false;
                int done = ctx.DamageDefending(atk.Damage);
                if (done > 0 && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 10);
                return true;
            }
        }

        /// <summary>Psychic: 10 more damage for each Energy card attached to the Defending Pokémon.</summary>
        internal sealed class EnergyCardBonusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : 10 * ctx.Defending.Energy.Count));
                return true;
            }
        }

        /// <summary>
        /// Sleeping Gas: heads, Asleep. Destiny Bond (discard 1 Psychic Energy card to use it): if a Pokémon Knocks Out this Pokémon
        /// during the opponent's next turn, Knock Out that Pokémon.
        /// </summary>
        private sealed class GastlyDestinyBond : Mon
        {
            private static readonly DiscardCostMon Bond = new DiscardCostMon("Destiny Bond", EnergyType.Psychic, 1, DestinyBondEffect,
                new CardsPAR2.FlipConditionMon("Sleeping Gas", SpecialCondition.Asleep));

            public override bool CanUseAttack(EffectContext ctx, int attackIndex) { return Bond.CanUseAttack(ctx, attackIndex); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bond.HandleAttack(ctx, atk); }

            /// <summary>v121 fix: Gastly has no Pokémon Power, so the engine has to be asked to tell it about its Knock Out.</summary>
            public override bool ReactsToOwnKnockOutWithoutAbility { get { return true; } }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                int turn;
                if (me == null || !me.Markers.TryGetValue(DestinyBondMarker, out turn) || turn != src.Turn) return;
                var attacker = src.Engine.AttackingPokemonOf(me);
                if (!byOpponentsAttack || attacker == null || attacker.Owner == src.Player || src.Engine.IsKnockedOut(attacker)) return;
                Announce(src, "Destiny Bond");
                src.Engine.KnockOut(attacker, "Destiny Bond");
            }
        }

        /// <summary>Foul Gas: flip a coin; heads, Poisoned; tails, Confused. Used for Foul Gas, Jellyfish Poison (the attack name is passed in).</summary>
        internal sealed class FoulGasMon : Mon
        {
            private readonly string attack;

            public FoulGasMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Poisoned : SpecialCondition.Confused);
                return true;
            }
        }
    }
}
