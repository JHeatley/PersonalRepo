using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>BREAKpoint (BKP, v96), part 2: the Psychic, Fighting, Darkness, Metal, Fairy, Dragon and Colorless Pokémon (50-95).</summary>
    public static class CardsBKP2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BKP-50", new FlipBonusMon("Focused Wish", 20));
            r.RegisterByKey("BKP-51", new HypnoBKP());
            r.RegisterByKey("BKP-52", new ComboMon(new Espeon(), new CardsPAF2.IgnoreDefenderEffectsMon("Psyshock")));
            r.RegisterByKey("BKP-53", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("BKP-54", new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned));
            r.RegisterByKey("BKP-55", new ComboMon(new ReflectiveShieldMon(), new Cards30C.PeerAtMon("Psy Report")));
            r.RegisterByKey("BKP-56", new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray"));
            r.RegisterByKey("BKP-57", new GarbodorBKP());
            r.RegisterByKey("BKP-59", new EnergyPresentMon());
            r.RegisterByKey("BKP-60", new CountersOnActiveMon("Stab", 1));
            r.RegisterByKey("BKP-61", new CountersOnActiveMon("Stab Deeply", 3));
            r.RegisterByKey("BKP-62", new ComboMon(new PainfulSwordMon(), new MegatonSlashMon()));
            r.RegisterByKey("BKP-63", new DodgeMon("Hide", false));
            r.RegisterByKey("BKP-64", new CardsSIT1.AscensionMon());
            r.RegisterByKey("BKP-65", new TrevenantBKP());
            r.RegisterByKey("BKP-66", new SilentFearMon());
            r.RegisterByKey("BKP-67", new WatchAndLearnMon());
            r.RegisterByKey("BKP-68", new NeverEnoughMon());
            r.RegisterByKey("BKP-69", new NoRetreatMon("Sand Tomb"));
            r.RegisterByKey("BKP-70", new ComboMon(new TurboAssaultMon(), new CardsFST2.ConditionBonusMon("Bite Off", 80, c => c.Defending != null && c.Defending.Def.IsPokemonEX)));
            r.RegisterByKey("BKP-72", new TailsNothingMon("Razor Wind"));
            r.RegisterByKey("BKP-73", new ComboMon(new RollUpMon(), new OtherworldlyReturnMon()));
            r.RegisterByKey("BKP-74", new ComboMon(new CardsASR1.PlusMon("Dark Pulse", 20, c => c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Darkness))),
                new CardsFST2.ConditionBonusMon("Dark Head", 80, c => c.Defending != null && c.Defending.Has(SpecialCondition.Asleep))));
            r.RegisterByKey("BKP-75", new ComboMon(new PartingShotMon(), new CardsFLF1.NoResistanceMon("Buster Swing")));
            r.RegisterByKey("BKP-76", new ComboMon(new CardsASC.SelfGuardMon("Steel Wing", 20), new GaleThrustMon("Gale Thrust", 60)));
            r.RegisterByKey("BKP-77", new IronCrusherMon());
            r.RegisterByKey("BKP-78", new ParalyzeFlipMon("Tight Jaw"));
            r.RegisterByKey("BKP-79", new CardsLOR1.HardenMon("Harden", 60));
            r.RegisterByKey("BKP-80", new SpikeLashMon());
            r.RegisterByKey("BKP-81", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("BKP-82", new ComboMon(new MoonBarrierMon(), new FlipBonusMon("Tumbling Attack", 30)));
            r.RegisterByKey("BKP-83", new ComboMon(new MightyWindMon(), new CoinsMon("Hurricane Wing", 4)));
            r.RegisterByKey("BKP-85", new CardsFST2.ConditionBonusMon("Dizzying Cologne", 40, c => c.Defending != null && c.Defending.Def.IsPokemonEX));
            r.RegisterByKey("BKP-86", new ComboMon(new SeverePoisonBKP(), new CardsBRS1.MillSelfMon("Dragon Pulse", 1)));
            r.RegisterByKey("BKP-87", new DangerousSuspicionMon());
            r.RegisterByKey("BKP-88", new RaticateBKP());
            r.RegisterByKey("BKP-89", new CardsPGO.SuperFangMon());
            r.RegisterByKey("BKP-90", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("BKP-91", new ComboMon(new CardsCES1.FlipConditionsMon("Mystifying Horns", SpecialCondition.Confused),
                new CardsFST2.ConditionBonusMon("Big Charge", 50, c => c.MyBench().Any(m => m.Def.IsXYMegaEvolution))));
            r.RegisterByKey("BKP-92", new HoOhEX());
            r.RegisterByKey("BKP-93", new ActCuteMon());
            r.RegisterByKey("BKP-94", new CardsROS1.HeadlockMon("Nyan Press", 40));
            r.RegisterByKey("BKP-95", new ComboMon(new DrawAttackMon("Collect", 1), new ParalyzeFlipMon("Tight Jaw")));
        }

        // ================================================================== Shared helpers

        /// <summary>"Put N damage counters on your opponent's Active Pokémon" (Stab, Stab Deeply).</summary>
        internal sealed class CountersOnActiveMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public CountersOnActiveMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, counters);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Goodnight, Babies: once during your turn, you may leave both Active Pokémon Asleep.</summary>
        private sealed class HypnoBKP : Mon
        {
            protected override string Activated { get { return "Goodnight, Babies"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Active != null && ctx.CanAffect(ctx.Opp.Active)) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep);
                if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Asleep);
            }
        }

        /// <summary>Reflective Shield: during the opponent's next turn, if this Pokémon is damaged by an attack (even if Knocked Out), 5 damage counters on the Attacking Pokémon.</summary>
        private sealed class ReflectiveShieldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reflective Shield") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new RetaliateEffect(ctx.Pokemon.Name + ": 5 damage counters on the Pokémon that damages it during the next turn (Reflective Shield)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 5));
                return true;
            }
        }

        /// <summary>
        /// Garbotoxin: while this Pokémon has a Pokémon Tool card attached, each Pokémon in play, in each player's hand and in each player's
        /// discard pile has no Abilities, except Garbotoxin. Offensive Bomb: Confused and Poisoned.
        /// </summary>
        internal sealed class GarbodorBKP : Mon
        {
            private static readonly Mon Bomb = new CardsCES1.ConditionsMon("Offensive Bomb", SpecialCondition.Confused, SpecialCondition.Poisoned);

            private static bool On(EffectContext src) { return src.Pokemon != null && src.Pokemon.Tools.Count > 0; }

            private static bool Blocked(CardDef card)
            {
                return card != null && card.IsPokemon && card.Abilities.Any(a => a.IsAbility && a.Name != "Garbotoxin");
            }

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }
            public override bool LockDependsOnTool { get { return true; } }

            /// <summary>Only the Pokémon's own Abilities (its cards), not one a Tool gives it (Forest Seal Stone: Compendium, 2024-04-18).</summary>
            internal static bool OwnAbility(PokemonInPlay mon, string abilityName)
            {
                return mon != null && mon.Stack.Any(c => c.Def.IsPokemon && c.Def.Abilities.Any(a => a.Name == abilityName));
            }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName) { return On(src) && abilityName != "Garbotoxin" && OwnAbility(mon, abilityName); }

            public override bool BlocksOpponentsAbilitiesOutOfPlay(EffectContext src, CardDef card) { return On(src) && Blocked(card); }

            public override bool BlocksOwnAbilitiesOutOfPlay(EffectContext src, CardDef card) { return On(src) && Blocked(card); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bomb.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Present: attach up to 2 Energy cards from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class EnergyPresentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Present") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var bench = ctx.MyBench().Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
                if (energy.Count == 0 || bench.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Energy Present: attach up to 2 Energy cards from your hand to 1 of your Benched Pokémon");
                if (picks.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Energy Present: attach them to which Benched Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
                return true;
            }
        }

        /// <summary>Painful Sword: double the number of damage counters on each of the opponent's Pokémon.</summary>
        private sealed class PainfulSwordMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Painful Sword") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0))
                    if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, mon.DamageCounters);
                return true;
            }
        }

        /// <summary>Megaton Slash: also 10 damage to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class MegatonSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megaton Slash") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                var targets = n == bench.Count ? bench : ctx.ChoosePokemonMany(bench, n, n, "Megaton Slash: choose 2 of your opponent's Benched Pokémon");
                foreach (var target in targets)
                    if (ctx.InPlay(target)) ctx.DamageTarget(target, 10);
                return true;
            }
        }

        /// <summary>Nervous Seed: while this Pokémon is your Active Pokémon, the attacks of the opponent's Basic Pokémon cost [C] more. Energy Press: 10 more per Energy on the Defending Pokémon.</summary>
        private sealed class TrevenantBKP : Mon
        {
            private static readonly Mon Press = new CardsASR1.PlusMon("Energy Press", 10, CardsLOT2.DefendingEnergy);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !src.IsActive(src.Pokemon) || !mon.Def.IsBasicPokemon) return;
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Press.HandleAttack(ctx, atk); }
        }

        /// <summary>Silent Fear: 3 damage counters on each of the opponent's Pokémon.</summary>
        private sealed class SilentFearMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silent Fear") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>
        /// Watch and Learn: if the opponent's Pokémon used an attack during their last turn, use it as this attack. A GX attack counts as
        /// your GX attack (Compendium, 2018-05-24); it still works if that Pokémon has left play (2016-02-11); copying Watch and Learn
        /// does nothing (2017-12-07).
        /// </summary>
        private sealed class WatchAndLearnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Watch and Learn") return false;
                var opp = ctx.Opp;
                var source = opp.LastAttackSourceDef;
                int index = opp.LastAttackIndex;
                if (opp.LastAttackTurn != ctx.OpponentsLastTurn || source == null || index < 0 || index >= source.Attacks.Count)
                {
                    ctx.Log("Watch and Learn: your opponent's Pokémon didn't attack during their last turn.");
                    return true;
                }
                if (source.Attacks[index].Name == "Watch and Learn")
                {
                    ctx.Log("Watch and Learn: copying Watch and Learn does nothing.");
                    return true;
                }
                UseCopiedAttack(ctx, source, index);
                return true;
            }
        }

        /// <summary>Never Enough: discard a card from your hand; if you do, draw 2 cards.</summary>
        private sealed class NeverEnoughMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Never Enough") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Never Enough: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
                return true;
            }
        }

        /// <summary>Turbo Assault: attach an Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class TurboAssaultMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Assault") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                var mine = ctx.MyPokemon();
                if (pile.Count == 0 || mine.Count == 0) return true;
                var card = ctx.ChooseCards(pile, 1, 1, "Turbo Assault: attach which Energy card from your discard pile?")[0];
                var target = ctx.ChoosePokemon(mine, "Turbo Assault: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target != null && ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Roll Up: flip 3 coins; if any are heads, the opponent reveals their hand, then discard a card from it for each heads.</summary>
        private sealed class RollUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roll Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0 || ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand.ToList();
                int n = Math.Min(heads, hand.Count);
                var picks = ctx.ChooseCards(hand, n, n, "Roll Up: discard " + n + (n == 1 ? " card" : " cards") + " from your opponent's hand");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Otherworldly Return: put a Trainer card from your discard pile into your hand.</summary>
        private sealed class OtherworldlyReturnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Otherworldly Return") return false;
                ctx.DamageDefending(atk.Damage);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var picks = ctx.ChooseCards(trainers, 1, 1, "Otherworldly Return: put which Trainer card into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Parting Shot: switch this Pokémon with 1 of your Benched Pokémon; the Defending Pokémon's attacks do 60 less next turn (before W/R).</summary>
        private sealed class PartingShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parting Shot") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                if (defending != null && ctx.InPlay(defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do 60 less damage during its next turn (Parting Shot)", ctx.Player, defending.Uid, ctx.Turn + 1, 60));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Gale Thrust: 60 more if this Pokémon moved from your Bench to the Active Spot this turn. Used for Gale Thrust, First Impression, Breakthrough Assault (the attack name and the numbers are passed in).</summary>
        internal sealed class GaleThrustMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public GaleThrustMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn ? amount : 0));
                return true;
            }
        }

        /// <summary>Iron Crusher: you may discard a Special Energy attached to the opponent's Active Pokémon or a Stadium card in play.</summary>
        private sealed class IronCrusherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Crusher") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool special = defending != null && ctx.InPlay(defending) && defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(defending);
                bool stadium = ctx.State.Stadium != null;
                if (!special && !stadium) return true;
                var labels = new List<string>();
                if (special) labels.Add("Discard a Special Energy from " + defending.Name);
                if (stadium) labels.Add("Discard " + ctx.State.Stadium.Def.Name);
                labels.Add("Neither");
                string pick = labels[ctx.ChooseOption("Iron Crusher: discard something?", labels.ToArray())];
                if (pick == "Neither") return true;
                if (special && pick.StartsWith("Discard a Special"))
                    ctx.DiscardEnergyFrom(defending, 1, "Iron Crusher: discard a Special Energy from " + defending.Name, e => e.Def.IsAttachedSpecialEnergy);
                else ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Spike Lash: 10 damage to each of the opponent's Pokémon for each [C] in its Retreat Cost (W/R for the Active only).</summary>
        private sealed class SpikeLashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Lash") return false;
                var plan = ctx.OpponentPokemon().Select(m => new KeyValuePair<PokemonInPlay, int>(m, ctx.Engine.RetreatCostOf(m))).ToList();
                foreach (var kv in plan)
                    if (kv.Value > 0 && ctx.InPlay(kv.Key)) ctx.DamageTarget(kv.Key, 10 * kv.Value);
                return true;
            }
        }

        // ================================================================== Fairy / Dragon

        /// <summary>"Prevent all effects of attacks, including damage, done to this Pokémon by [type] Pokémon during your opponent's next turn" (Moon Barrier).</summary>
        internal sealed class TypeBarrierEffect : ContinuousEffect
        {
            private readonly EnergyType type;

            public TypeBarrierEffect(string name, int owner, int targetUid, int expiresAfterTurn, EnergyType type)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.type = type;
            }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.Attacker != null && info.AgainstOpponent && engine.IsType(info.Attacker, type);
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                var attacker = engine.CurrentSourcePokemon;
                return target.Uid == TargetUid && kind == EffectSourceKind.Attack && attacker != null && attacker.Owner != target.Owner && engine.IsType(attacker, type);
            }
        }

        /// <summary>Moon Barrier: during the opponent's next turn, prevent all effects of attacks, including damage, done to this Pokémon by Dragon Pokémon.</summary>
        private sealed class MoonBarrierMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Moon Barrier") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new TypeBarrierEffect(ctx.Pokemon.Name + " is protected from Dragon Pokémon during the opponent's next turn (Moon Barrier)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, EnergyType.Dragon));
                return true;
            }
        }

        /// <summary>Mighty Wind: you may attach an Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class MightyWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mighty Wind") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var bench = ctx.MyBench().Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
                if (energy.Count == 0 || bench.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, 0, 1, "Mighty Wind: you may attach an Energy card from your hand to 1 of your Benched Pokémon");
                if (picks.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Mighty Wind: attach " + picks[0].Def.Name + " to which Benched Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(picks[0])) ctx.Engine.AttachEnergyCard(picks[0], target, true);
                return true;
            }
        }

        /// <summary>Severe Poison: Poisoned, with 4 damage counters in Pokémon Checkup instead of 1.</summary>
        private sealed class SeverePoisonBKP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Severe Poison") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsUNB2.HeavyPoison(ctx, defending, 4);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Dangerous Suspicion: draw a card, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class DangerousSuspicionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Suspicion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(1);
                if (ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Antibodies: this Pokémon can't be affected by any Special Conditions (and loses any it has). Dirty Shock: Poisoned, and its Tools are discarded.</summary>
        private sealed class RaticateBKP : Mon
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target == src.Pokemon;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Shock") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (defending.Tools.Count > 0 && ctx.CanAffect(defending)) ctx.Engine.DiscardAllTools(defending);
                return true;
            }
        }

        /// <summary>Purifying Fire: once during your turn, if this Pokémon has any basic Fire Energy attached, heal 50 damage from it. Elemental Feather: 30 to 1 Benched.</summary>
        private sealed class HoOhEX : Mon
        {
            private static readonly Mon Feather = new CardsMEG.SnipeBenchMon("Elemental Feather", 30);

            protected override string Activated { get { return "Purifying Fire"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me.DamageCounters > 0 && !ctx.Engine.HealingBlocked(me) && me.Energy.Any(e => CardsSSP1.IsBasicOf(e.Def, EnergyType.Fire));
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Pokemon, 50); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Feather.HandleAttack(ctx, atk); }
        }

        /// <summary>Act Cute: the opponent puts a card from their hand on the bottom of their deck (they choose).</summary>
        private sealed class ActCuteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Act Cute") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.Engine.ChooseCards(ctx.Opponent, hand, 1, 1, "Act Cute: put a card from your hand on the bottom of your deck", ctx.Source);
                if (picks.Count == 0 || !ctx.Opp.Hand.Remove(picks[0])) return true;
                ctx.Opp.Deck.Add(picks[0]);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " put a card from their hand on the bottom of their deck (Act Cute).");
                return true;
            }
        }

    }
}
