using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Chaos Rising (CRI): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Chespin, Litleo, Remoraid, Bergmite, Wimpod, Pumpkaboo, Phanpy, Stunky, Skrelp,
    /// Ferroseed, Sliggoo.
    /// </summary>
    public static class CardsCRI
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("CRI-1", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("CRI-2", new SolidShellMon());
            r.RegisterByKey("CRI-3", new BeedrillExCRI());
            r.RegisterByKey("CRI-4", new CarnivineCRI());
            r.RegisterByKey("CRI-6", new CardsMEG.ChargeSelfMon("Leafy Charge", EnergyType.Grass, 1));
            r.RegisterByKey("CRI-7", new ChesnaughtCRI());
            r.RegisterByKey("CRI-8", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("CRI-9", new NinetalesCRI());
            r.RegisterByKey("CRI-10", new HoOhCRI());
            r.RegisterByKey("CRI-12", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("CRI-15", new MegaPyroarExCRI());
            r.RegisterByKey("CRI-17", new OctilleryCRI());
            r.RegisterByKey("CRI-18", new DelibirdCRI());
            r.RegisterByKey("CRI-19", new KeldeoCRI());
            r.RegisterByKey("CRI-20", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("CRI-21", new SearchAttackMon("Summoning Jutsu", d => d.IsPokemon, 3));
            r.RegisterByKey("CRI-24", new AvaluggCRI());
            r.RegisterByKey("CRI-26", new GolisopodCRI());
            r.RegisterByKey("CRI-27", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("CRI-28", new CardsUPR2.DisconnectMon());
            r.RegisterByKey("CRI-30", new EmolgaCRI());
            r.RegisterByKey("CRI-31", new ComboMon(new CardsMEG.ChargeSelfMon("Genome Charge", EnergyType.Psychic, 2), new CardsMEG.DefendingEnergyBonusMon("Psychic", 20)));
            r.RegisterByKey("CRI-33", new DeoxysProtectionCRI());
            r.RegisterByKey("CRI-34", new DeoxysSpeedCRI());
            r.RegisterByKey("CRI-35", new MegaFloetteExCRI());
            r.RegisterByKey("CRI-36", new EspurrCRI());
            r.RegisterByKey("CRI-37", new MeowsticCRI());
            r.RegisterByKey("CRI-38", new PhantumpCRI());
            r.RegisterByKey("CRI-39", new TrevenantCRI());
            r.RegisterByKey("CRI-41", new GourgeistExCRI());
            r.RegisterByKey("CRI-42", new XerneasCRI());
            r.RegisterByKey("CRI-43", new ComboMon(new SearchAttackMon("Trials and Trip-ulations", d => d.Name == "Transformation Tome", 2), new CardsFLF1.NoResistanceMon("Rock Hurl")));
            r.RegisterByKey("CRI-45", new DonphanCRI());
            r.RegisterByKey("CRI-46", new CardsPFL.FlipUntilTailsTimesMon("Continuous Spin"));
            r.RegisterByKey("CRI-47", new CardsBCR2.DevolutionPunchMon("Devolution Ray"));
            r.RegisterByKey("CRI-48", new MegaGalladeExCRI());
            r.RegisterByKey("CRI-49", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("CRI-50", new BasicShieldAttackMon("Covert Flight"));
            r.RegisterByKey("CRI-52", new CounterConditionMon("Poison Point", SpecialCondition.Poisoned, new CardsBLK.VenoshockMon(50)));
            r.RegisterByKey("CRI-54", new SwitchSelfMon("Smash Turn", false));
            r.RegisterByKey("CRI-55", new ComboMon(new NoRetreatMon("Corner"), new CardsASC.ToolBonusMon("Strong Bite", 140)));
            r.RegisterByKey("CRI-56", new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray"));
            r.RegisterByKey("CRI-57", new GarbodorCRI());
            r.RegisterByKey("CRI-60", new CardsASC.SelfGuardMon("Guard Press", 30));
            r.RegisterByKey("CRI-63", new FerrothornCRI());
            r.RegisterByKey("CRI-64", new CobalionExCRI());
            r.RegisterByKey("CRI-65", new MegaDragalgeExCRI());
            r.RegisterByKey("CRI-66", new CardsTWM1.HealSelfMon("Absorb", 30));
            r.RegisterByKey("CRI-69", new TaurosCRI());
            r.RegisterByKey("CRI-71", new WatchogCRI());

            r.RegisterByName("Adversity Policy", new AdversityPolicy());
            r.RegisterByName("Ange Floette", new AngeFloette());
            r.RegisterByName("Emma", new Emma());
            r.RegisterByName("Great Haul Net", new GreatHaulNet());
            r.RegisterByName("Roxie's Performance", new RoxiesPerformance());
            r.RegisterByName("Transformation Tome", new TransformationTome());
            r.RegisterByName("Bubbly Water Energy", new BubblyWaterEnergy());
            r.RegisterByName("Magnetic Metal Energy", new MagneticMetalEnergy());
            r.RegisterByName("Nitro Fire Energy", new NitroFireEnergy());
        }

        // ================================================================== Shared helpers

        /// <summary>"If this Pokémon is in the Active Spot and is damaged by an attack from your opponent's Pokémon (even if it is Knocked Out), the Attacking Pokémon is now [condition]." Attacks come from another script.</summary>
        public sealed class CounterConditionMon : Mon
        {
            private readonly string ability;
            private readonly SpecialCondition condition;
            private readonly Mon attacks;

            public CounterConditionMon(string ability, SpecialCondition condition, Mon attacks)
            {
                this.ability = ability;
                this.condition = condition;
                this.attacks = attacks;
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, condition)) Announce(ctx, ability);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks from Basic Pokémon."</summary>
        public sealed class BasicShieldAttackMon : Mon
        {
            private readonly string attack;

            public BasicShieldAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CardsMEG.ShieldFromBasics(ctx, atk.Name);
                return true;
            }
        }

        /// <summary>"During your next turn, attacks used by this Pokémon do N more damage to your opponent's Active Pokémon."</summary>
        public sealed class NextTurnPowerEffect : ContinuousEffect
        {
            private readonly int bonus;

            public NextTurnPowerEffect(string name, int owner, int targetUid, int expiresAfterTurn, int bonus)
                : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                this.bonus = bonus;
            }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || attacker.Uid != TargetUid || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                return damage + bonus;
            }
        }

        /// <summary>"During your opponent's next turn, if the Defending Pokémon tries to use an attack, your opponent flips N coins. If any of them is tails, that attack doesn't happen."</summary>
        public sealed class AttackCoinsEffect : ContinuousEffect
        {
            private readonly int coins;

            public AttackCoinsEffect(string name, int owner, int targetUid, int expiresAfterTurn, int coins)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.coins = coins;
            }

            public override int AttackFlipCoins(GameEngine engine, PokemonInPlay attacker) { return attacker.Uid == TargetUid ? coins : 0; }
        }

        /// <summary>"During your opponent's next turn, Energy can't be attached from your opponent's hand to the Defending Pokémon."</summary>
        public sealed class NoEnergyFromHandEffect : ContinuousEffect
        {
            public NoEnergyFromHandEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target) { return target.Uid == TargetUid; }
        }

        /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks from Pokémon that have an Ability."</summary>
        public sealed class AbilityShieldEffect : ContinuousEffect
        {
            public AbilityShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && engine.HasAbility(info.Attacker) ? 0 : damage;
            }
        }

        // ================================================================== Grass

        /// <summary>Rumbling Bees: 110 for each of your Beedrill and Beedrill ex in play.</summary>
        private sealed class BeedrillExCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rumbling Bees") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.Name == "Beedrill" || m.Name == "Beedrill ex").Count);
                return true;
            }
        }

        /// <summary>Chomp Whole: +80 if the opponent's Active Pokémon has no Retreat Cost.</summary>
        private sealed class CarnivineCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chomp Whole") return false;
                bool free = ctx.Defending != null && ctx.Engine.RetreatCostOf(ctx.Defending) == 0;
                ctx.DamageDefending(atk.Damage + (free ? 80 : 0));
                return true;
            }
        }

        /// <summary>Needly Armor: when damaged in the Active Spot by the opponent's attack, 3 damage counters on the attacker per Grass Energy on it. Impound: can't retreat.</summary>
        private sealed class ChesnaughtCRI : Mon
        {
            private static readonly Mon Impound = new NoRetreatMon("Impound");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                int grass = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass);
                if (grass == 0) return;
                Announce(ctx, "Needly Armor");
                ctx.Engine.PlaceDamageCounters(attacker, 3 * grass, "Needly Armor");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Impound.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Nine-Tailed Transfer: move all damage counters from 1 of your Benched Pokémon to the opponent's Active Pokémon.</summary>
        private sealed class NinetalesCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nine-Tailed Transfer") return false;
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                var d = ctx.Defending;
                if (hurt.Count == 0 || d == null || !ctx.CanAffect(d)) return true;
                var from = ctx.ChoosePokemon(hurt, "Nine-Tailed Transfer: move all damage counters from which Benched Pokémon?");
                if (from != null) ctx.Engine.MoveDamageCounters(from, d, from.DamageCounters, atk.Name);
                return true;
            }
        }

        /// <summary>Flames of Revival: up to 3 Basic Pokémon from your discard pile onto your Bench. Bright Wing: discard a Fire Energy.</summary>
        private sealed class HoOhCRI : Mon
        {
            private static readonly Mon BrightWing = new CardsASC.SelfDiscardTypeMon("Bright Wing", EnergyType.Fire, 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flames of Revival")
                {
                    var basics = ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon).ToList();
                    int max = Math.Min(3, ctx.BenchSpace);
                    if (basics.Count == 0 || max <= 0) return true;
                    var picks = ctx.ChooseCards(basics, 0, Math.Min(max, basics.Count), "Flames of Revival: put up to " + max + " Basic Pokémon onto your Bench");
                    foreach (var c in picks)
                        if (ctx.BenchSpace > 0 && ctx.Me.Discard.Remove(c)) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    return true;
                }
                return BrightWing.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Ferocious Bellow: the Defending Pokémon's attacks do 50 less next turn. Fiery Big Bang: 10 less per damage counter on it.</summary>
        private sealed class MegaPyroarExCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ferocious Bellow")
                {
                    CardsMEG.WeakenDefendingMon.WeakenDefending(ctx, atk, 50);
                    return true;
                }
                if (atk.Name == "Fiery Big Bang")
                {
                    ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>Jet of Ink: next turn the Defending Pokémon must flip 2 heads to attack. Tantrum: it becomes Confused.</summary>
        private sealed class OctilleryCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Jet of Ink")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.AddEffect(new AttackCoinsEffect(defending.Name + " must flip 2 heads to attack during its next turn (Jet of Ink)", ctx.Player, defending.Uid, ctx.Turn + 1, 2));
                    return true;
                }
                if (atk.Name == "Tantrum")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Pleasing Present: each player (the opponent first) may attach up to 3 Basic Energy from their hand to their Pokémon.</summary>
        private sealed class DelibirdCRI : Mon
        {
            private static void Present(EffectContext ctx, int player)
            {
                var engine = ctx.Engine;
                var p = ctx.State.Players[player];
                for (int i = 0; i < 3; i++)
                {
                    var energy = p.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                    var targets = p.InPlay().Where(m => engine.CanAttachFromHand(m)).ToList();
                    if (energy.Count == 0 || targets.Count == 0) return;
                    var pick = engine.ChooseCards(player, energy, 0, 1, "Pleasing Present: attach a Basic Energy card from your hand (" + (i + 1) + " of up to 3)", ctx.Source);
                    if (pick.Count == 0) return;
                    var target = engine.ChoosePokemon(player, targets, "Pleasing Present: attach " + pick[0].Def.Name + " to which Pokémon?", ctx.Source);
                    if (target == null || !p.Hand.Remove(pick[0])) return;
                    engine.AttachEnergyCard(pick[0], target, true);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pleasing Present") return false;
                Present(ctx, ctx.Opponent);
                Present(ctx, ctx.Player);
                return true;
            }
        }

        /// <summary>Shoot Through: also 20 to 1 Benched Pokémon. Reflect Energy: move an Energy from it to 1 of your Benched Pokémon.</summary>
        private sealed class KeldeoCRI : Mon
        {
            private static readonly Mon ShootThrough = new CardsMEG.SnipeBenchMon("Shoot Through", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reflect Energy")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsDRI1.MoveEnergyToBench(ctx, 1, atk.Name);
                    return true;
                }
                return ShootThrough.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Iceberg Breaker: discard your top 6 cards, 60 per Basic Water Energy among them.</summary>
        private sealed class AvaluggCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iceberg Breaker") return false;
                var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 6);
                ctx.DamageDefending(atk.Damage * gone.Count(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)));
                return true;
            }
        }

        /// <summary>Critical Slash: if it Knocks Out the Defending Pokémon, it's protected next turn. Boundless Power: can't attack next turn.</summary>
        private sealed class GolisopodCRI : Mon
        {
            private static readonly Mon BoundlessPower = new LockAfterAttackMon("Boundless Power", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Critical Slash")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (defending != null && ctx.InPlay(defending) && ctx.Engine.IsKnockedOut(defending) && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                    return true;
                }
                return BoundlessPower.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Lightning

        /// <summary>Minor Errand-Running: search for up to 2 Basic Energy. Sky Return: put this Pokémon and everything attached into your hand.</summary>
        private sealed class EmolgaCRI : Mon
        {
            private static readonly Mon Errand = new SearchAttackMon("Minor Errand-Running", d => d.IsBasicEnergy, 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sky Return")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                    return true;
                }
                return Errand.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Psychic

        /// <summary>Psy Protection: next turn, no damage from attacks of Pokémon that have an Ability.</summary>
        private sealed class DeoxysProtectionCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psy Protection") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new AbilityShieldEffect(ctx.Pokemon.Name + " takes no damage from Pokémon with Abilities during the opponent's next turn (Psy Protection)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Psyspeed: you may draw until you have 5 cards in your hand.</summary>
        private sealed class DeoxysSpeedCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psyspeed") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count < 5 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Psyspeed: draw cards until you have 5 cards in your hand?")) ctx.DrawUntil(5);
                return true;
            }
        }

        /// <summary>Gentle Light: heal 30 from every Pokémon. Eternity Bloom: up to 4 Basic Psychic Energy from your deck to your Benched Pokémon.</summary>
        private sealed class MegaFloetteExCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gentle Light")
                {
                    foreach (var m in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                        if (m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)) ctx.Heal(m, 30);
                    return true;
                }
                if (atk.Name == "Eternity Bloom")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 4, "Eternity Bloom: search your deck for up to 4 Basic Psychic Energy cards");
                    CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => ctx.Me.Bench.Contains(m), "Eternity Bloom: attach to your Benched Pokémon");
                    ctx.ShuffleMyDeck();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Buddy Attack: +60 if you played Emma this turn.</summary>
        private sealed class EspurrCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Buddy Attack") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.SupporterNameThisTurn == "Emma" ? 60 : 0));
                return true;
            }
        }

        /// <summary>Tricky Steps: you may move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class MeowsticCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tricky Steps") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Tricky Steps: move an Energy from " + defending.Name + " to 1 of your opponent's Benched Pokémon?")) return true;
                var e = ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Tricky Steps: move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.Opp.Bench.ToList(), "Tricky Steps: move it to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, e);
                return true;
            }
        }

        /// <summary>Spiteful Evolution: once during your turn (not your first), evolve it from your hand; 2 damage counters on the evolved Pokémon.</summary>
        private sealed class PhantumpCRI : Mon
        {
            private static List<CardInstance> Evolutions(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsPokemon && c.Def.EvolvesFrom == ctx.Pokemon.Name).ToList();
            }

            protected override string Activated { get { return "Spiteful Evolution"; } }
            protected override bool AbilityReady(EffectContext ctx) { return !ctx.IsMyFirstTurn && Evolutions(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var options = Evolutions(ctx);
                var card = options.Select(c => c.Def.Name).Distinct().Count() == 1 ? options[0]
                    : ctx.ChooseCards(options, 1, 1, "Spiteful Evolution: evolve into which card?")[0];
                var me = ctx.Pokemon;
                if (!ctx.Me.Hand.Remove(card)) return;
                ctx.Engine.EvolvePokemon(me, card, true);
                if (ctx.InPlay(me)) ctx.Engine.PlaceDamageCounters(me, 2, "Spiteful Evolution");
            }
        }

        /// <summary>Cursed Roots: no Energy from the opponent's hand onto the Defending Pokémon next turn. Overwhelming Pain: +10 per damage counter on the opponent's Pokémon.</summary>
        private sealed class TrevenantCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Cursed Roots")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.AddEffect(new NoEnergyFromHandEffect("No Energy can be attached from the hand to " + defending.Name + " during its next turn (Cursed Roots)", ctx.Player, defending.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Overwhelming Pain")
                {
                    int counters = ctx.OpponentPokemon().Sum(m => m.DamageCounters);
                    ctx.DamageDefending(atk.Damage + 10 * counters);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Horrifying Rondo: +50 per Benched Pokémon of yours with damage counters. Ghostly Touch: discard a random card from the opponent's hand.</summary>
        private sealed class GourgeistExCRI : Mon
        {
            private static readonly Mon GhostlyTouch = new CardsASC.RandomDiscardMon("Ghostly Touch");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Horrifying Rondo")
                {
                    ctx.DamageDefending(atk.Damage + 50 * ctx.Me.Bench.Count(m => m.DamageCounters > 0));
                    return true;
                }
                return GhostlyTouch.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Geo Storm: 30 for each Psychic Energy on all your Pokémon.</summary>
        private sealed class XerneasCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Geo Storm") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Psychic)));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>No Reprieve: next turn its attacks do 120 more to the opponent's Active Pokémon. Smashing Headbutt: discard 2 Energy.</summary>
        private sealed class DonphanCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "No Reprieve")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                    {
                        var boost = new NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do 120 more damage during its next turn (No Reprieve)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, 120);
                        boost.ActiveFromTurn = ctx.Turn + 2;
                        ctx.AddEffect(boost);
                    }
                    return true;
                }
                if (atk.Name == "Smashing Headbutt")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Gale Slash: +150 if it has no damage counters.</summary>
        private sealed class MegaGalladeExCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gale Slash") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.DamageCounters == 0 ? 150 : 0));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Gloomy Garbage: the opponent's Active Pokémon with a Tool attached does 20 less damage with its attacks.</summary>
        private sealed class GarbodorCRI : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner == src.Player || a.Tool == null || !src.State.IsActive(a)) return damage;
                return damage - 20;
            }
        }

        // ================================================================== Metal

        /// <summary>Startling Drop: discarded from your deck by the opponent's effects during their turn, discard their top 8 cards. Special Whip: +70 with Special Energy attached.</summary>
        private sealed class FerrothornCRI : Mon
        {
            public override void OnDiscardedFromDeckByOpponent(EffectContext ctx)
            {
                if (ctx.Opp.Deck.Count == 0) return;
                ctx.Log("Startling Drop (" + ctx.Card.Def.Name + "): " + ctx.Engine.PlayerName(ctx.Opponent) + " discards the top 8 cards of their deck.");
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 8);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Whip") return false;
                bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? 70 : 0));
                return true;
            }
        }

        /// <summary>Metal Road: once during your turn, when it moves from your Bench to the Active Spot, move any Metal Energy from your other Pokémon to it. Power Tackle: can't attack next turn.</summary>
        private sealed class CobalionExCRI : Mon
        {
            private static readonly Mon PowerTackle = new LockAfterAttackMon("Power Tackle", false);

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var metal = new List<CardInstance>();
                foreach (var m in ctx.MyPokemon(x => x != me))
                    foreach (var e in m.Energy)
                        if (CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Metal)) metal.Add(e);
                if (!TriggerOnce(ctx, "Metal Road", metal.Count > 0, "Metal Road: move Metal Energy from your other Pokémon to " + me.Name + "?")) return;
                var picks = ctx.ChooseCards(metal, 0, metal.Count, "Metal Road: move which Metal Energy to " + me.Name + "?");
                foreach (var e in picks)
                {
                    var from = ctx.MyPokemon(m => m.Energy.Contains(e)).FirstOrDefault();
                    if (from != null) ctx.Engine.MoveEnergy(from, me, e);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PowerTackle.HandleAttack(ctx, atk); }
        }

        // ================================================================== Dragon

        /// <summary>Corrosive Liquid: discard every Tool and Special Energy from the opponent's Pokémon. Pernicious Poison: Poisoned with 16 damage counters per Checkup.</summary>
        private sealed class MegaDragalgeExCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Corrosive Liquid")
                {
                    foreach (var m in ctx.OpponentPokemon())
                    {
                        if (!ctx.CanAffect(m)) continue;
                        ctx.Engine.DiscardAllTools(m);
                        foreach (var e in m.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(m, e);
                    }
                    return true;
                }
                if (atk.Name == "Pernicious Poison")
                {
                    var d = ctx.Defending;
                    if (d != null && ctx.ApplyCondition(d, SpecialCondition.Poisoned)) d.PoisonCounterBase = 16;
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Colorless

        /// <summary>Target Together: choose 1 of the opponent's Pokémon; flip a coin per Tauros you have in play, 50 per heads.</summary>
        private sealed class TaurosCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Target Together") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Target Together: choose 1 of your opponent's Pokémon");
                int heads = ctx.FlipCoins(ctx.MyPokemon(m => m.Name.Contains("Tauros")).Count);
                if (target != null && heads > 0) ctx.DamageTarget(target, 50 * heads);
                return true;
            }
        }

        /// <summary>Snap Inspection: flip 3; for each heads, shuffle a card you choose from the opponent's hand into their deck.</summary>
        private sealed class WatchogCRI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snap Inspection") return false;
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Opp.Hand.Count == 0) return true;
                CardsTWM1.RevealOpponentHand(ctx);
                var picks = ctx.ChooseCards(ctx.Opp.Hand.ToList(), Math.Min(heads, ctx.Opp.Hand.Count), Math.Min(heads, ctx.Opp.Hand.Count), "Snap Inspection: shuffle which cards into your opponent's deck?");
                foreach (var c in picks)
                    if (ctx.Opp.Hand.Remove(c)) ctx.Opp.Deck.Add(c);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Tool: if its holder is Weak to the opponent's Active Pokémon's type and is damaged in the Active Spot by their attack, draw 3.</summary>
        private sealed class AdversityPolicy : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || !holderWasActive || attacker == null || attacker.Owner == ctx.Player) return;
                var weakness = ctx.Engine.WeaknessOf(holder);
                if (!weakness.HasValue || !ctx.Engine.IsType(attacker, weakness.Value)) return;
                ctx.Log("Adversity Policy: " + ctx.Engine.PlayerName(ctx.Player) + " draws 3 cards.");
                ctx.Draw(3);
            }
        }

        /// <summary>Stadium: only by discarding a Prism Tower in play (even the turn it was played). Each Mega Floette ex gets +150 HP.</summary>
        private sealed class AngeFloette : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == "Prism Tower"; }
            public override bool IgnoresStadiumLimit(EffectContext ctx) { return CanPlayTrainer(ctx); }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon.Name == "Mega Floette ex" ? hp + 150 : hp;
            }
        }

        /// <summary>The opponent reveals their hand; draw a card for each Pokémon in it.</summary>
        private sealed class Emma : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                CardsTWM1.RevealOpponentHand(ctx);
                int n = ctx.Opp.Hand.Count(c => c.Def.IsPokemon);
                if (n > 0) ctx.Draw(n);
            }
        }

        /// <summary>Shuffle up to 3 Water Pokémon and/or up to 3 Basic Water Energy from your discard pile into your deck.</summary>
        private sealed class GreatHaulNet : CardScript
        {
            private static List<CardInstance> Pokemon(EffectContext ctx) { return ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Water).ToList(); }
            private static List<CardInstance> Energy(EffectContext ctx) { return ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pokemon(ctx).Count > 0 || Energy(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var picks = new List<CardInstance>();
                var mons = Pokemon(ctx);
                if (mons.Count > 0) picks.AddRange(ctx.ChooseCards(mons, 0, Math.Min(3, mons.Count), "Great Haul Net: shuffle up to 3 Water Pokémon into your deck"));
                var energy = Energy(ctx);
                if (energy.Count > 0) picks.AddRange(ctx.ChooseCards(energy, 0, Math.Min(3, energy.Count), "Great Haul Net: shuffle up to 3 Basic Water Energy cards into your deck"));
                foreach (var c in picks)
                    if (ctx.Me.Discard.Remove(c)) ctx.Me.Deck.Add(c);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>During the opponent's next turn, their Poisoned Pokémon can't retreat.</summary>
        private sealed class RoxiesPerformance : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new PoisonedCantRetreatEffect(ctx.Engine.PlayerName(ctx.Opponent) + "'s Poisoned Pokémon can't retreat during their next turn (Roxie's Performance)", ctx.Player, ctx.Turn + 1));
            }
        }

        private sealed class PoisonedCantRetreatEffect : ContinuousEffect
        {
            public PoisonedCantRetreatEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsRetreat(GameEngine engine, PokemonInPlay mon) { return mon.Owner != Owner && mon.Has(SpecialCondition.Poisoned); }
        }

        /// <summary>Play 2 at once: swap a Basic Pokémon in your discard pile with 1 of your Basic Pokémon in play; everything on it stays.</summary>
        private sealed class TransformationTome : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => m.Stack.Count == 1 && m.Def.IsBasicPokemon); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.Name == "Transformation Tome")
                       && ctx.Me.Discard.Any(c => c.Def.IsBasicPokemon) && Targets(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var second = ctx.Me.Hand.FirstOrDefault(c => c != ctx.Card && c.Def.Name == "Transformation Tome");
                if (second == null) return;
                ctx.Engine.DiscardFromHand(ctx.Player, second);
                var basics = ctx.Me.Discard.Where(c => c.Def.IsBasicPokemon).ToList();
                var targets = Targets(ctx);
                if (basics.Count == 0 || targets.Count == 0) return;
                var incoming = ctx.ChooseCards(basics, 1, 1, "Transformation Tome: choose a Basic Pokémon in your discard pile")[0];
                var target = ctx.ChoosePokemon(targets, "Transformation Tome: switch it with which of your Basic Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(incoming)) return;
                var old = target.Stack[0];
                target.Stack[0] = incoming;
                ctx.Me.Discard.Add(old);
                ctx.Log(old.Def.Name + " was switched with " + incoming.Def.Name + " from the discard pile (Transformation Tome).");
            }
        }

        /// <summary>Provides Water; a Water Pokémon with it recovers from and can't be affected by Special Conditions.</summary>
        private sealed class BubblyWaterEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Water), 1); }

            private static void Recover(EffectContext src)
            {
                var m = src.Pokemon;
                if (m == null || m.Conditions == SpecialCondition.None || !src.Engine.IsType(m, EnergyType.Water)) return;
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (m.Has(c)) src.Engine.RemoveCondition(m, c);
            }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return target == src.Pokemon && src.Engine.IsType(target, EnergyType.Water);
            }

            public override void OnAttachedFromHand(EffectContext ctx) { Recover(ctx); }
            public override void OnPokemonCheckup(EffectContext src) { Recover(src); }
            public override void OnEndOfTurn(EffectContext src) { Recover(src); }
        }

        /// <summary>Provides Metal; a Metal Pokémon with it has no Retreat Cost.</summary>
        private sealed class MagneticMetalEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Metal), 1); }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon == src.Pokemon && src.Engine.IsType(mon, EnergyType.Metal) ? 0 : cost;
            }
        }

        /// <summary>Provides Fire; discarded by its Fire Pokémon's own attack, it goes back to your hand afterwards.</summary>
        private sealed class NitroFireEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Fire), 1); }

            public override bool ReturnsAfterAttackDiscard(EffectContext ctx)
            {
                return ctx.Pokemon != null && ctx.Engine.IsType(ctx.Pokemon, EnergyType.Fire);
            }
        }
    }
}
