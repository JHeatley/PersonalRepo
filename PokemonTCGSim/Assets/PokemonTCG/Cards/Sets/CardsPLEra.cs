using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// v173: shared helpers for the Platinum and HeartGold &amp; SoulSilver era sets (PL, RR, SV, AR, HS, UL, UD, TM, CL and
    /// POP Series 9): attacks written as small functions, a Poké-Power wrapper, and the lasting effects these sets use a lot.
    /// </summary>
    public static class CardsPLEra
    {
        /// <summary>
        /// Attacks as functions. Each returns true when it handled the attack (false: the printed damage is done to the
        /// Defending Pokémon). <see cref="After"/> does the printed damage first and then the effect.
        /// </summary>
        internal sealed class Fx : Mon
        {
            private readonly Dictionary<string, Func<EffectContext, AttackDef, bool>> map = new Dictionary<string, Func<EffectContext, AttackDef, bool>>();

            public Fx On(string attack, Func<EffectContext, AttackDef, bool> resolve) { map[attack] = resolve; return this; }

            /// <summary>The printed damage to the Defending Pokémon, then the effect.</summary>
            public Fx After(string attack, Action<EffectContext, AttackDef> effect)
            {
                map[attack] = (c, a) => { c.DamageDefending(a.Damage); effect(c, a); return true; };
                return this;
            }

            /// <summary>The effect, then the printed damage (for effects that happen "before doing damage").</summary>
            public Fx Before(string attack, Action<EffectContext, AttackDef> effect)
            {
                map[attack] = (c, a) => { effect(c, a); c.DamageDefending(a.Damage); return true; };
                return this;
            }

            /// <summary>Damage set by a function (base damage plus or minus, or times).</summary>
            public Fx Damage(string attack, Func<EffectContext, AttackDef, int> damage)
            {
                map[attack] = (c, a) => { c.DamageDefending(Math.Max(0, damage(c, a))); return true; };
                return this;
            }

            /// <summary>An attack with only an effect and no damage.</summary>
            public Fx Effect(string attack, Action<EffectContext, AttackDef> effect)
            {
                map[attack] = (c, a) => { effect(c, a); return true; };
                return this;
            }

            /// <summary>Uses another script for these attacks.</summary>
            public Fx With(Mon other)
            {
                others.Add(other);
                return this;
            }

            private readonly List<Mon> others = new List<Mon>();

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                Func<EffectContext, AttackDef, bool> f;
                if (map.TryGetValue(atk.Name, out f)) return f(ctx, atk);
                foreach (var o in others) if (o.HandleAttack(ctx, atk)) return true;
                return false;
            }

            public override bool ReactsToOwnKnockOutWithoutAbility { get { return others.Any(p => p.ReactsToOwnKnockOutWithoutAbility); } }

            public override bool ReactsToOwnDamageWithoutAbility { get { return others.Any(p => p.ReactsToOwnDamageWithoutAbility); } }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                foreach (var part in others) if (part.ReactsToOwnKnockOutWithoutAbility) part.OnHolderKnockedOut(src, byOpponentsAttack, wasActive);
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                foreach (var part in others) if (part.ReactsToOwnDamageWithoutAbility) part.OnHolderDamagedByAttack(ctx, attacker, holderWasActive, damage);
            }
        }

        /// <summary>
        /// A Poké-Power used once during your turn (before your attack). By default it can't be used while the Pokémon is
        /// affected by a Special Condition; <paramref name="anyCondition"/> lifts that.
        /// </summary>
        internal class PowerFx : Mon
        {
            private readonly string power;
            private readonly Func<EffectContext, bool> ready;
            private readonly Action<EffectContext> effect;
            private readonly Mon attacks;
            private readonly bool anyCondition;
            private readonly bool onePerPlayer;

            public PowerFx(string power, Func<EffectContext, bool> ready, Action<EffectContext> effect, Mon attacks, bool anyCondition = false, bool onePerPlayer = false)
            {
                this.power = power;
                this.ready = ready;
                this.effect = effect;
                this.attacks = attacks;
                this.anyCondition = anyCondition;
                this.onePerPlayer = onePerPlayer;
            }

            protected override string Activated { get { return power; } }

            protected override bool OnePerPlayer { get { return onePerPlayer; } }

            protected override bool AbilityReady(EffectContext ctx) { return (anyCondition || CardsDS.PowerReady(ctx)) && (ready == null || ready(ctx)); }

            protected override void Ability(EffectContext ctx) { effect(ctx); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }

            public override bool ReactsToOwnKnockOutWithoutAbility { get { return attacks != null && attacks.ReactsToOwnKnockOutWithoutAbility; } }

            public override bool ReactsToOwnDamageWithoutAbility { get { return attacks != null && attacks.ReactsToOwnDamageWithoutAbility; } }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (attacks != null && attacks.ReactsToOwnKnockOutWithoutAbility) attacks.OnHolderKnockedOut(src, byOpponentsAttack, wasActive);
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (attacks != null && attacks.ReactsToOwnDamageWithoutAbility) attacks.OnHolderDamagedByAttack(ctx, attacker, holderWasActive, damage);
            }
        }

        /// <summary>
        /// "As often as you like during your turn (before your attack)": each step returns false when the player stops.
        /// </summary>
        internal sealed class RepeatPowerFx : Mon
        {
            private readonly string power;
            private readonly Func<EffectContext, bool> canStep;
            private readonly Func<EffectContext, bool, bool> step;
            private readonly Mon attacks;
            private readonly bool anyCondition;

            public RepeatPowerFx(string power, Func<EffectContext, bool> canStep, Func<EffectContext, bool, bool> step, Mon attacks, bool anyCondition = false)
            {
                this.power = power;
                this.canStep = canStep;
                this.step = step;
                this.attacks = attacks;
                this.anyCondition = anyCondition;
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != power) return false;
                return ctx.IsMyTurn && (anyCondition || CardsE1.NoCondition(ctx.Pokemon)) && canStep(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => canStep(ctx), offerDone => step(ctx, offerDone));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Once during your turn, when you put this Pokémon from your hand onto your Bench, you may use this power."</summary>
        internal sealed class BenchTriggerFx : Mon
        {
            private readonly string power;
            private readonly Func<EffectContext, bool> ready;
            private readonly Action<EffectContext> effect;
            private readonly Mon attacks;

            public BenchTriggerFx(string power, Func<EffectContext, bool> ready, Action<EffectContext> effect, Mon attacks)
            {
                this.power = power;
                this.ready = ready;
                this.effect = effect;
                this.attacks = attacks;
            }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, power, ready == null || ready(ctx), power + ": use it?")) return;
                effect(ctx);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Base for a Pokémon with a Poké-Body (hooks in the subclass) and attacks from another script.</summary>
        internal abstract class BodyMon : Mon
        {
            private readonly Mon attacks;

            protected BodyMon(Mon attacks) { this.attacks = attacks; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }

            public override bool ReactsToOwnKnockOutWithoutAbility { get { return attacks != null && attacks.ReactsToOwnKnockOutWithoutAbility; } }

            public override bool ReactsToOwnDamageWithoutAbility { get { return attacks != null && attacks.ReactsToOwnDamageWithoutAbility; } }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (attacks != null && attacks.ReactsToOwnKnockOutWithoutAbility) attacks.OnHolderKnockedOut(src, byOpponentsAttack, wasActive);
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (attacks != null && attacks.ReactsToOwnDamageWithoutAbility) attacks.OnHolderDamagedByAttack(ctx, attacker, holderWasActive, damage);
            }
        }

        /// <summary>Sweet Sleeping Face (Poké-Body, the HGSS Baby Pokémon): while Asleep, prevent all damage done to it by attacks.</summary>
        internal sealed class SweetSleepingFaceMon : BodyMon
        {
            public SweetSleepingFaceMon(Mon attacks) : base(attacks) { }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Pokemon.Has(SpecialCondition.Asleep) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, "Sweet Sleeping Face");
            }
        }

        /// <summary>"As long as this Pokémon is on your Bench, prevent all damage done to it by attacks" (Shell Barricade, Submerge).</summary>
        internal sealed class BenchGuardMon : BodyMon
        {
            private readonly string body;

            public BenchGuardMon(string body, Mon attacks) : base(attacks) { this.body = body; }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && src.Me.Bench.Contains(src.Pokemon) && src.Engine.CurrentSourceKind == EffectSourceKind.Attack && Works(src, body);
            }
        }

        /// <summary>Look at the top N cards of either player's deck and put them back in any order (Second Sight).</summary>
        internal static void ReorderTop(EffectContext c, int n, string what, bool either)
        {
            bool mine = !either || c.Opp.Deck.Count == 0 || (c.Me.Deck.Count > 0 && c.ChooseOption(what + ": look at whose deck?", "Your deck", "Your opponent's deck") == 0);
            var deck = mine ? c.Me.Deck : c.Opp.Deck;
            var top = deck.Take(n).ToList();
            foreach (var x in top) deck.Remove(x);
            var order = new List<CardInstance>();
            var left = top.ToList();
            while (left.Count > 1)
            {
                var pick = c.ChooseCards(left, 1, 1, what + ": which card goes next from the top?")[0];
                order.Add(pick);
                left.Remove(pick);
            }
            order.AddRange(left);
            deck.InsertRange(0, order);
        }

        // ------------------------------------------------------------------ Small shared pieces

        /// <summary>"This attack does nothing": logs it and returns true (the attack was handled).</summary>
        internal static bool DidNothing(EffectContext ctx)
        {
            ctx.Log(ctx.Pokemon.Name + "'s attack did nothing.");
            return true;
        }

        internal static bool SP(PokemonInPlay mon) { return mon != null && mon.Def.IsPokemonSP; }

        internal static bool Affected(PokemonInPlay mon) { return mon != null && mon.Conditions != SpecialCondition.None; }

        internal static bool HasPowerOrBody(PokemonInPlay mon)
        {
            return mon != null && mon.Def.Abilities.Any(a => a.Kind == "Poké-Power" || a.Kind == "Poké-Body");
        }

        internal static bool HasPower(PokemonInPlay mon) { return mon != null && mon.Def.Abilities.Any(a => a.Kind == "Poké-Power"); }

        /// <summary>This Pokémon's own Poké-Body works (it isn't blocked).</summary>
        internal static bool Works(EffectContext src, string body) { return src.Pokemon != null && src.InPlay(src.Pokemon) && !src.Engine.AbilityBlocked(src.Pokemon, body); }

        /// <summary>"Move an Energy card attached to this Pokémon to 1 of your Benched Pokémon."</summary>
        internal static void EnergyToBench(EffectContext ctx, string attack, Func<CardInstance, bool> filter = null)
        {
            if (!ctx.InPlay(ctx.Pokemon) || ctx.MyBench().Count == 0) return;
            var cards = ctx.Pokemon.Energy.Where(e => filter == null || filter(e)).ToList();
            if (cards.Count == 0) return;
            var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, attack + ": move which Energy card?")[0];
            var to = ctx.ChoosePokemon(ctx.MyBench(), attack + ": move it to which Benched Pokémon?");
            if (to != null) ctx.Engine.MoveEnergy(ctx.Pokemon, to, card);
        }

        /// <summary>Discards up to n Energy cards of a type attached to this Pokémon (all when n is large).</summary>
        internal static int DiscardTypeFromSelf(EffectContext ctx, EnergyType type, int n)
        {
            int done = 0;
            while (done < n && ctx.InPlay(ctx.Pokemon))
            {
                var e = ctx.Pokemon.Energy.FirstOrDefault(x => CardsDRI1.ProvidesType(ctx, ctx.Pokemon, x, type));
                if (e == null || !ctx.Engine.DiscardEnergy(ctx.Pokemon, e)) break;
                done++;
            }
            return done;
        }

        /// <summary>"Put the Defending Pokémon in the Lost Zone instead of the discard pile if it's Knocked Out by this attack."</summary>
        internal static int DamageLostOnKnockOut(EffectContext ctx, int amount)
        {
            var d = ctx.Defending;
            if (d == null) return 0;
            d.LostZoneKnockOutTurn = ctx.Turn;
            int done = ctx.DamageDefending(amount);
            if (ctx.InPlay(d) && !ctx.Engine.IsKnockedOut(d)) d.LostZoneKnockOutTurn = -100;
            return done;
        }

        /// <summary>A coin flip for each heads... "Flip a coin until you get tails" count.</summary>
        internal static int Heads(EffectContext ctx, int coins) { return ctx.FlipCoins(coins); }

        /// <summary>"Remove N damage counters from each of your Pokémon" (heal in HP).</summary>
        internal static void HealEach(EffectContext ctx, int hp, bool benchOnly = false)
        {
            foreach (var m in benchOnly ? ctx.MyBench() : ctx.MyPokemon()) if (m.DamageCounters > 0) ctx.Heal(m, hp);
        }

        /// <summary>Choose 1 of your opponent's Pokémon and do damage to it (no Weakness or Resistance for the Bench).</summary>
        internal static PokemonInPlay HitAny(EffectContext ctx, int amount, string attack, Func<PokemonInPlay, bool> filter = null)
        {
            var pool = ctx.OpponentPokemon(m => filter == null || filter(m));
            if (pool.Count == 0) return null;
            var t = ctx.ChoosePokemon(pool, attack + ": choose 1 of your opponent's Pokémon");
            if (t != null) ctx.DamageTarget(t, amount);
            return t;
        }

        /// <summary>Choose up to n of the opponent's Benched Pokémon and do damage to each.</summary>
        internal static void HitBenched(EffectContext ctx, int n, int amount, string attack, Func<PokemonInPlay, bool> filter = null)
        {
            var pool = ctx.OpponentBench().Where(m => filter == null || filter(m)).ToList();
            if (pool.Count == 0) return;
            int k = Math.Min(n, pool.Count);
            foreach (var t in ctx.ChoosePokemonMany(pool, k, k, attack + ": choose " + k + " of your opponent's Benched Pokémon")) ctx.DamageTarget(t, amount);
        }

        /// <summary>Damage to each of the opponent's Benched Pokémon (optionally a filter).</summary>
        internal static void HitEachBenched(EffectContext ctx, int amount, Func<PokemonInPlay, bool> filter = null)
        {
            foreach (var t in ctx.OpponentBench().Where(m => filter == null || filter(m)).ToList()) ctx.DamageTarget(t, amount);
        }

        /// <summary>The player chooses 1 card from the opponent's hand without looking (a random card).</summary>
        internal static CardInstance RandomFromHand(EffectContext ctx, PlayerState p)
        {
            if (p.Hand.Count == 0) return null;
            return p.Hand[ctx.State.Rng.Next(p.Hand.Count)];
        }

        /// <summary>Shows the opponent's hand to the player (logs it).</summary>
        internal static void LookAtHand(EffectContext ctx, PlayerState p)
        {
            ctx.Engine.ShowCards(ctx.Player, p.Hand.ToList(), "Your opponent's hand", "Your opponent's hand", ctx.Source);
        }

        /// <summary>Evolves a Pokémon with a card taken out of the deck (search effects), shuffling afterward.</summary>
        internal static bool EvolveFromDeck(EffectContext ctx, PokemonInPlay target, Func<CardDef, bool> filter, string prompt)
        {
            if (target == null) return false;
            // An attack's own evolution overrides the timing rules (HS:Unleashed FAQ: Speed Evolution on the first turn).
            var picks = ctx.SearchMyDeck(d => d.IsPokemon && filter(d) && (d.EvolvesFrom == target.Name || ctx.Engine.CanEvolveInto(target, d, false)), 1, prompt);
            ctx.ShuffleMyDeck();
            if (picks.Count == 0) return false;
            ctx.Engine.EvolvePokemon(target, picks[0]);
            return true;
        }

        /// <summary>Search your deck for up to max cards and attach them to your Pokémon (in any way); shuffle afterward.</summary>
        internal static int SearchAttach(EffectContext ctx, Func<CardDef, bool> filter, string attack, int max = 1, Func<PokemonInPlay, bool> target = null)
        {
            var picks = ctx.SearchMyDeck(filter, max, attack + ": search your deck for Energy to attach" + (max > 1 ? " (up to " + max + ")" : ""));
            int n = 0;
            if (picks.Count > 0) n = ctx.AttachCardsAnyWay(picks, picks.Count, (c, m) => target == null || target(m), attack, false);
            foreach (var c in picks) if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c)) && !ctx.Me.Discard.Contains(c)) ctx.Me.Deck.Add(c);
            ctx.ShuffleMyDeck();
            return n;
        }

        /// <summary>Puts an Energy card attached to a Pokémon in its owner's Lost Zone.</summary>
        internal static bool EnergyToLostZone(EffectContext ctx, PokemonInPlay mon, CardInstance energy)
        {
            if (mon == null || energy == null || !mon.Energy.Remove(energy)) return false;
            ctx.State.Players[energy.Owner].LostZone.Add(energy);
            ctx.Engine.Emit(GameEventType.Info, energy.Owner, energy.Def.Name + " attached to " + mon.Name + " was put in the Lost Zone.", energy.Id, mon.Uid);
            return true;
        }

        /// <summary>A player (maybe the opponent) chooses between min and max of these Pokémon.</summary>
        internal static List<PokemonInPlay> ChooseMonsFor(EffectContext ctx, int player, IList<PokemonInPlay> cands, int min, int max, string prompt)
        {
            if (cands.Count == 0 || max <= 0) return new List<PokemonInPlay>();
            var opts = cands.Select(m => new DecisionOption { Label = ctx.Engine.DescribeFor(player, m), PokemonUid = m.Uid, CardId = m.Top.Id, Card = m.Def }).ToList();
            return ctx.Engine.AskMany(player, DecisionKind.ChoosePokemon, prompt, opts, min, max, ctx.Source).Select(i => cands[i]).ToList();
        }

        /// <summary>Shuffle the player's hand into their deck, then draw n cards.</summary>
        internal static void ShuffleHandDraw(EffectContext ctx, int player, int n)
        {
            var p = ctx.State.Players[player];
            foreach (var c in p.Hand.ToList()) { p.Hand.Remove(c); p.Deck.Add(c); }
            ctx.Engine.ShuffleDeck(player);
            ctx.Engine.DrawCards(player, n);
        }

        // ------------------------------------------------------------------ Lasting effects

        /// <summary>The Pokémon's attacks do N more damage to the Defending Pokémon (before W/R) during the given turn.</summary>
        internal sealed class AttackBonusEffect : ContinuousEffect
        {
            private readonly int amount;
            private readonly int onTurn;
            private readonly string attackName;

            // It stays on the Pokémon if it moves to the Bench and back (Cheerful Voice, Bulk Up rulings).
            public AttackBonusEffect(string name, int owner, int targetUid, int onTurn, int amount, string attackName = null) : base(name, owner, targetUid, onTurn, false)
            {
                this.amount = amount;
                this.onTurn = onTurn;
                this.attackName = attackName;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || engine.State.TurnNumber != onTurn || target == null || target.Owner == Owner) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack || engine.State.Players[target.Owner].Active != target || damage <= 0) return damage;
                if (attackName != null && (engine.ResolvingAttack == null || engine.ResolvingAttack.Name != attackName)) return damage;
                return damage + amount;
            }
        }

        /// <summary>Damage done to the Pokémon by attacks is changed by N (after W/R) until the effect ends.</summary>
        internal sealed class TakenChangeEffect : ContinuousEffect
        {
            private readonly int delta;
            private readonly bool opponentsOnly;

            public TakenChangeEffect(string name, int owner, int targetUid, int expiresAfterTurn, int delta, bool opponentsOnly = false) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.delta = delta;
                this.opponentsOnly = opponentsOnly;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || damage <= 0 || engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (opponentsOnly && (info.Attacker == null || info.Attacker.Owner == info.Target.Owner)) return damage;
                return Math.Max(0, damage + delta);
            }
        }

        /// <summary>If the Pokémon tries to attack, its owner flips a coin; tails, the attack does nothing (Smokescreen).</summary>
        internal sealed class AttackFlipEffect : ContinuousEffect
        {
            public AttackFlipEffect(string name, int owner, int targetUid, int expiresAfterTurn, bool endsWhenMoves = true) : base(name, owner, targetUid, expiresAfterTurn, endsWhenMoves) { }

            public override bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return attacker != null && attacker.Uid == TargetUid; }
        }

        /// <summary>The Pokémon has no Weakness.</summary>
        internal sealed class NoWeaknessEffect : ContinuousEffect
        {
            public NoWeaknessEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override EnergyType? ModifyWeakness(GameEngine engine, PokemonInPlay mon, EnergyType? weakness) { return mon != null && mon.Uid == TargetUid ? (EnergyType?)null : weakness; }
        }

        /// <summary>The Pokémon can't use Poké-Powers (and, optionally, can't retreat).</summary>
        internal sealed class NoPowersEffect : ContinuousEffect
        {
            private readonly bool noRetreat;

            public NoPowersEffect(string name, int owner, int targetUid, int expiresAfterTurn, bool noRetreat) : base(name, owner, targetUid, expiresAfterTurn, true) { this.noRetreat = noRetreat; }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return noRetreat && mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Prevent all effects of attacks, including damage, done to the Pokémon by attackers that match.</summary>
        internal sealed class ShieldFromEffect : ContinuousEffect
        {
            private readonly Func<GameEngine, PokemonInPlay, bool> attackerMatches;

            public ShieldFromEffect(string name, int owner, int targetUid, int expiresAfterTurn, Func<GameEngine, PokemonInPlay, bool> attackerMatches) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.attackerMatches = attackerMatches;
            }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Owner != Owner && engine.CurrentSourceKind == EffectSourceKind.Attack && attackerMatches(engine, info.Attacker);
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target == null || target.Uid != TargetUid || kind != EffectSourceKind.Attack) return false;
                var a = engine.State.Players[1 - Owner].Active;
                return a != null && attackerMatches(engine, a);
            }
        }

        /// <summary>Prevent all damage done to the Pokémon by attacks (optionally only "damage of N or less").</summary>
        internal sealed class NoDamageEffect : ContinuousEffect
        {
            private readonly int upTo;

            public NoDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn, int upTo = int.MaxValue) : base(name, owner, targetUid, expiresAfterTurn, true) { this.upTo = upTo; }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && engine.CurrentSourceKind == EffectSourceKind.Attack && damage <= upTo;
            }
        }

        /// <summary>"If this Pokémon would be Knocked Out by damage from an attack, it is not Knocked Out and its remaining HP becomes 10" (Endure).</summary>
        internal sealed class EndureEffect : ContinuousEffect
        {
            public EndureEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool SurvivesKnockOutFromAttack(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>The Pokémon can't retreat (during its owner's next turn: Roost).</summary>
        internal sealed class SelfNoRetreatEffect : ContinuousEffect
        {
            public SelfNoRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>The Pokémon can't use any Poké-Powers or Poké-Bodies (Seal Off).</summary>
        internal sealed class SealOffEffect : ContinuousEffect
        {
            public SealOffEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool StopsPokePower(GameEngine engine, PokemonInPlay mon, string powerName) { return mon != null && mon.Uid == TargetUid; }

            public override bool StopsPokeBody(GameEngine engine, PokemonInPlay mon, string bodyName) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>The opponent can't play Pokémon from their hand to evolve or level up this Pokémon (Primal Breath).</summary>
        internal sealed class NoEvolveTargetEffect : ContinuousEffect
        {
            public NoEvolveTargetEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>The Pokémon can't attack or retreat (Scary Face).</summary>
        internal sealed class NoAttackNoRetreatEffect : ContinuousEffect
        {
            public NoAttackNoRetreatEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>"Each player shuffles their hand into their deck and draws up to N cards (you draw first)."</summary>
        internal static void EachShufflesAndDraws(EffectContext ctx, int n, string what)
        {
            foreach (int p in new[] { ctx.Player, ctx.Opponent })
            {
                var ps = ctx.State.Players[p];
                foreach (var c in ps.Hand.ToList()) { ps.Hand.Remove(c); ps.Deck.Add(c); }
                ctx.Engine.ShuffleDeck(p);
                int k = ctx.Engine.AskNumber(p, Math.Min(1, ps.Deck.Count), Math.Min(n, ps.Deck.Count), what + ": draw how many cards (up to " + n + ")?", ctx.Source);
                ctx.Engine.DrawCards(p, k);
            }
        }

        /// <summary>"Undevelop": devolve this Pokémon, putting its top card into your hand.</summary>
        internal static PowerFx Undevelop(Mon attacks)
        {
            return new PowerFx("Undevelop", c => c.Pokemon.Stack.Count >= 2, c => c.Engine.Devolve(c.Pokemon, 1), attacks);
        }

        /// <summary>"[Type] Shift": this Pokémon's type is [type] until the end of your turn (Rotom).</summary>
        internal static PowerFx Shift(string power, EnergyType type, Mon attacks)
        {
            return new PowerFx(power, null, c => c.AddEffect(new CardsGE.TypeIsEffect(c.Pokemon.Name + " is " + type + " this turn (" + power + ")", c.Player, c.Pokemon.Uid, c.Turn, type)), attacks, true);
        }

        /// <summary>"If this Pokémon was damaged by an attack during your opponent's last turn, this attack does the same amount of damage."</summary>
        internal static bool ReturnBlow(EffectContext c)
        {
            var me = c.Pokemon;
            if (me.AttackDamageTurn != c.OpponentsLastTurn || me.AttackDamageThatTurn <= 0) return DidNothing(c);
            c.DamageDefending(me.AttackDamageThatTurn);
            return true;
        }

        /// <summary>"During your next turn, this Pokémon's [attack]'s base damage is N" (Focus Energy).</summary>
        internal static Fx FocusEnergy(string attack, int damage, bool tailsNothing)
        {
            return new Fx()
                .Effect("Focus Energy", (c, a) => c.Pokemon.Markers["Focus Energy"] = c.Turn + 2)
                .On(attack, (c, a) =>
                {
                    int t;
                    int dmg = c.Pokemon.Markers.TryGetValue("Focus Energy", out t) && t == c.Turn ? damage : a.Damage;
                    if (tailsNothing && !c.Flip()) return DidNothing(c);
                    c.DamageDefending(dmg);
                    return true;
                });
        }

        /// <summary>The Pokémon is Knocked Out at the end of the given turn (Doom News).</summary>
        internal sealed class DelayedKnockOutEffect : ContinuousEffect
        {
            private readonly int turn;
            private readonly string reason;

            // An effect on the Defending Pokémon: it ends if that Pokémon leaves the Active Spot (Supreme Victors FAQ, 2009-08-27).
            public DelayedKnockOutEffect(string name, int owner, int targetUid, int turn, string reason) : base(name, owner, targetUid, turn, true)
            {
                this.turn = turn;
                this.reason = reason;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != turn) return;
                var target = engine.State.FindPokemon(TargetUid);
                if (target != null) engine.KnockOut(target, reason);
            }
        }

        /// <summary>The Pokémon can use only this attack (Encore).</summary>
        internal sealed class OnlyAttackEffect : ContinuousEffect
        {
            private readonly string allowed;

            public OnlyAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn, string allowed) : base(name, owner, targetUid, expiresAfterTurn, true) { this.allowed = allowed; }

            public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string attackName) { return mon != null && mon.Uid == TargetUid && attackName != allowed; }
        }

        /// <summary>The Pokémon's Retreat Cost is changed (set to 0 when delta is null).</summary>
        internal sealed class RetreatChangeEffect : ContinuousEffect
        {
            private readonly int? delta;

            public RetreatChangeEffect(string name, int owner, int targetUid, int expiresAfterTurn, int? delta) : base(name, owner, targetUid, expiresAfterTurn, true) { this.delta = delta; }

            public override int ModifyRetreatCost(GameEngine engine, PokemonInPlay mon, int cost)
            {
                if (mon == null || mon.Uid != TargetUid) return cost;
                return delta == null ? 0 : Math.Max(0, cost + delta.Value);
            }
        }

        /// <summary>During the player's next turn, each Basic Pokémon they put onto their Bench from their hand gets 2 damage counters (Silver Feather).</summary>
        internal sealed class BenchTrapEffect : ContinuousEffect
        {
            private readonly int affected;
            private readonly string reason;

            public BenchTrapEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn, string reason) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
                this.reason = reason;
            }

            public override void OnPokemonBenchedFromHand(GameEngine engine, PokemonInPlay benched)
            {
                if (benched == null || benched.Owner != affected || !benched.Def.IsBasicPokemon || engine.State.CurrentPlayer != affected) return;
                engine.PlaceDamageCounters(benched, 2, reason);
            }
        }

        /// <summary>The player can't play Pokémon from their hand to evolve their Pokémon (or only the Active one).</summary>
        internal sealed class NoEvolveEffect : ContinuousEffect
        {
            private readonly int affected;

            public NoEvolveEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { affected = affectedPlayer; }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Owner == affected; }
        }
    }
}
