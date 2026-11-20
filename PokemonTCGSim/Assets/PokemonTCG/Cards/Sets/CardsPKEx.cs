using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>Power Keepers Trainers (72-83), Pokémon-ex (92-99) and Pokémon ★ (100-102).</summary>
    public static partial class CardsPK
    {
        private static void RegisterExAndTrainers(CardScriptRegistry r)
        {
            r.RegisterByKey("PK-72", new DrakesStadium());
            r.RegisterByKey("PK-76", new GlaciasStadium());
            r.RegisterByKey("PK-79", new PhoebesStadium());
            r.RegisterByKey("PK-82", new SidneysStadium());
            r.RegisterByKey("PK-83", new StevensAdvicePK());

            r.RegisterByKey("PK-92", new AbsolExCursedEyes());
            r.RegisterByKey("PK-93", new TypeShiftMon(EnergyType.Fighting, new ComboMon(
                new CardsASR1.PlusMon("Psychic Boom", 10, c => c.Defending == null ? 0 : c.EnergyCount(c.Defending)), new ShadowCrushMon())));
            r.RegisterByKey("PK-94", new FlygonExPsychicProtector());
            r.RegisterByKey("PK-95", new MetagrossExMagneticRedraw());
            r.RegisterByKey("PK-96", new ComboMon(new CardsBRS1.MillSelfMon("Volcanic Flame", 5), new HydroWaveMon()));
            r.RegisterByKey("PK-97", new ComboMon(new SkillHackMon(), new SameNameBenchMon("Dirge")));
            r.RegisterByKey("PK-98", new SkarmoryExMetalGravity());
            r.RegisterByKey("PK-99", new WalreinExChillingBreath());
            r.RegisterByKey("PK-100", new FlareonStarCrimsonRay());
            r.RegisterByKey("PK-101", new JolteonStarYellowRay());
            r.RegisterByKey("PK-102", new VaporeonStarBlueRay());
        }

        // ================================================================== Trainers

        /// <summary>Drake's Stadium: attack damage done to a Colorless Active Pokémon (either player's) by an opponent's attack is 10 less (after W/R).</summary>
        private sealed class DrakesStadium : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                var a = info.Attacker;
                if (t == null || a == null || a.Owner == t.Owner || !info.TargetIsActive || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return src.Engine.IsType(t, EnergyType.Colorless) ? damage - 10 : damage;
            }
        }

        /// <summary>Glacia's Stadium: each player's Water Pokémon (not Pokémon-ex) has no Weakness.</summary>
        private sealed class GlaciasStadium : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return mon != null && !mon.Def.IsOldEx && src.Engine.IsType(mon, EnergyType.Water);
            }
        }

        /// <summary>Phoebe's Stadium: each player pays 2 Colorless less to retreat their Psychic Pokémon (it lowers the Retreat Cost: Compendium).</summary>
        private sealed class PhoebesStadium : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && src.Engine.IsType(mon, EnergyType.Psychic) ? Math.Max(0, cost - 2) : cost;
            }
        }

        /// <summary>Sidney's Stadium: each player's Darkness Pokémon can't be Asleep, Confused or Paralyzed (ones it already has go away: Compendium).</summary>
        private sealed class SidneysStadium : CardScript
        {
            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                if (target == null || !src.Engine.IsType(target, EnergyType.Darkness)) return false;
                return condition == SpecialCondition.Asleep || condition == SpecialCondition.Confused || condition == SpecialCondition.Paralyzed;
            }

            public override bool CuresConditionsItPrevents { get { return true; } }
        }

        /// <summary>
        /// Steven's Advice (Supporter, Power Keepers): draw up to as many cards as your opponent has Pokémon in play. You can't play it
        /// with 7 or more cards in your hand, this one included (Hidden Legends' says more than 7).
        /// </summary>
        private sealed class StevensAdvicePK : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count < 7 && ctx.Me.Deck.Count > 0 && ctx.OpponentPokemon().Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int n = Math.Min(ctx.OpponentPokemon().Count, ctx.Me.Deck.Count);
                if (n > 0) ctx.Draw(n == 1 ? 1 : ctx.AskNumber(0, n, "Steven's Advice: draw how many cards (up to " + n + ")?"));
            }
        }

        // ================================================================== Pokémon-ex

        /// <summary>
        /// Cursed Eyes (Poké-Power): when you put Absol ex onto your Bench from your hand, you may move 3 damage counters (all of them
        /// if it has fewer: Compendium) from 1 of your opponent's Pokémon to another of theirs. Psychic Pulse.
        /// </summary>
        private sealed class AbsolExCursedEyes : Mon
        {
            private const string Power = "Cursed Eyes";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                bool ready = ctx.OpponentPokemon(m => m.DamageCounters > 0).Count > 0 && ctx.OpponentPokemon().Count > 1;
                if (!TriggerOnce(ctx, Power, ready, "Use " + Power + "? Move 3 damage counters from 1 of your opponent's Pokémon to another.")) return;
                var from = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m.DamageCounters > 0), Power + ": move damage counters from which of your opponent's Pokémon?");
                if (from == null) return;
                var to = ctx.ChoosePokemon(ctx.OpponentPokemon(m => m != from), Power + ": move them to which of your opponent's Pokémon?");
                if (to != null) ctx.Engine.MoveDamageCounters(from, to, Math.Min(3, from.DamageCounters), Power);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Pulse") return false;
                var damaged = ctx.Opp.Bench.Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in damaged)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Type Shift (Poké-Power): once during your turn, this Pokémon is [type] until the end of your turn.</summary>
        internal sealed class TypeShiftMon : Mon
        {
            private const string Marker = "Type Shift";
            private readonly EnergyType type;
            private readonly Mon attacks;

            public TypeShiftMon(EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            protected override string Activated { get { return Marker; } }

            protected override void Ability(EffectContext ctx) { ctx.Pokemon.Markers[Marker] = ctx.Turn; }

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                int turn;
                if (src.Pokemon == null || mon != src.Pokemon || !mon.Markers.TryGetValue(Marker, out turn) || turn != src.Turn) return;
                types.Clear();
                types.Add(type);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Shadow Crush: you may discard a Psychic Energy card from this Pokémon; if you do, discard an Energy card from the Defending Pokémon.</summary>
        private sealed class ShadowCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Crush") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var psychic = me.Energy.Where(e => CardsRS.BasicOf(e.Def, EnergyType.Psychic)).ToList();
                if (!ctx.InPlay(me) || psychic.Count == 0 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.AskYesNo("Shadow Crush: discard a Psychic Energy card from " + me.Name + " to discard an Energy card from " + defending.Name + "?")) return true;
                if (!ctx.Engine.DiscardEnergy(me, psychic[0])) return true;
                if (ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Shadow Crush: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Psychic Protector (Poké-Body): when Flygon ex is damaged by an opponent's attack, you may discard up to 4 cards from your hand
        /// for 10 less damage each (after W/R; more cards than needed is fine: Compendium). Slashing Strike: tails, not next turn.
        /// </summary>
        private sealed class FlygonExPsychicProtector : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == src.Player || damage <= 0 || src.Me.Hand.Count == 0) return damage;
                if (src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                var picks = src.Engine.ChooseCards(src.Player, src.Me.Hand.ToList(), 0, Math.Min(4, src.Me.Hand.Count),
                    "Psychic Protector: discard up to 4 cards from your hand (" + damage + " damage coming; 10 less for each card)", src.Card.Def);
                if (picks.Count == 0) return damage;
                Announce(src, "Psychic Protector");
                foreach (var card in picks) src.Engine.DiscardFromHand(src.Player, card);
                return damage - 10 * picks.Count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slashing Strike") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Magnetic Redraw (Poké-Power): once during your turn, if Metagross ex is your Active Pokémon, each player shuffles their hand
        /// into their deck and draws 4 cards. Not with a Special Condition. Scanblast: 70 to each of their Benched Pokémon with the
        /// Defending Pokémon's name.
        /// </summary>
        private sealed class MetagrossExMagneticRedraw : Mon
        {
            private static readonly Mon Scanblast = new SameNameBenchMon("Scanblast");

            protected override string Activated { get { return "Magnetic Redraw"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsDS.PowerReady(ctx) && ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Draw(4);
                ctx.Engine.DrawCards(ctx.Opponent, 4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Scanblast.HandleAttack(ctx, atk); }
        }

        /// <summary>"Does N damage to each of your opponent's Benched Pokémon that has the same name as the Defending Pokémon" (Scanblast, Dirge).</summary>
        internal sealed class SameNameBenchMon : Mon
        {
            private readonly string attack;

            public SameNameBenchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                string name = defending == null ? null : defending.Name;
                ctx.DamageDefending(atk.Damage);
                if (name == null) return true;
                foreach (var mon in ctx.Opp.Bench.Where(m => m.Name == name).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, atk.Damage);
                return true;
            }
        }

        /// <summary>Hydro Wave: discard all Water Energy from Salamence ex; 30 to each of the opponent's Benched Pokémon (no W/R).</summary>
        private sealed class HydroWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Wave") return false;
                var me = ctx.Pokemon;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me))
                    foreach (var card in me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Water)).ToList()) ctx.Engine.DiscardEnergy(me, card);
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>
        /// Skill Hack: look at your opponent's hand and copy an attack of a Pokémon card there (you must if any has one: Compendium),
        /// without its Energy cost; Shiftry ex stays a Darkness Pokémon.
        /// </summary>
        private sealed class SkillHackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Skill Hack") return false;
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", "Skill Hack: your opponent's hand", ctx.Source);
                var cards = hand.Where(c => c.Def.IsPokemon && c.Def.Attacks.Count > 0).ToList();
                if (cards.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Skill Hack: copy an attack of which Pokémon card?")[0];
                int index = card.Def.Attacks.Count == 1 ? 0 : ctx.ChooseOption("Skill Hack: copy which attack of " + card.Def.Name + "?", card.Def.Attacks.Select(a => a.Name).ToArray());
                UseCopiedAttack(ctx, card.Def, index);
                return true;
            }
        }

        /// <summary>
        /// Metal Gravity (Poké-Body): when your opponent's Active Pokémon retreats with 40 or more HP left, 3 damage counters on it.
        /// Only 1 Metal Gravity each turn. Whirlwind.
        /// </summary>
        private sealed class SkarmoryExMetalGravity : Mon
        {
            private static readonly Mon Whirlwind = new CardsDRI1.WhirlwindMon("Whirlwind");

            public override void OnPokemonRetreated(EffectContext src, PokemonInPlay retreated)
            {
                if (src.Pokemon == null || retreated == null || retreated.Owner == src.Player || !src.InPlay(retreated)) return;
                if (src.RemainingHp(retreated) < 40) return;
                var opp = src.State.Players[1 - src.Player];
                int turn;
                if (opp.NamedLimitUsedOnTurn.TryGetValue("Metal Gravity (opponent's)", out turn) && turn == src.Turn) return;
                opp.NamedLimitUsedOnTurn["Metal Gravity (opponent's)"] = src.Turn;
                Announce(src, "Metal Gravity");
                src.Engine.PlaceDamageCounters(retreated, 3, "Metal Gravity");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Whirlwind.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Chilling Breath (Poké-Power): when you play Walrein ex from your hand to evolve, your opponent can't play Trainer cards from
        /// their hand during their next turn (the lock stays on the player: Compendium). Wreck: 20 more with a Stadium in play, which
        /// is then discarded.
        /// </summary>
        private sealed class WalreinExChillingBreath : Mon
        {
            private const string Power = "Chilling Breath";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Your opponent can't play Trainer cards during their next turn.")) return;
                ctx.AddEffect(new TrainerLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards during their next turn (Chilling Breath)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreck") return false;
                bool stadium = ctx.State.Stadium != null;
                ctx.DamageDefending(atk.Damage + (stadium ? 20 : 0));
                if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        private sealed class TrainerLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public TrainerLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected && card != null && card.Def.IsTrainer; }
        }

        // ================================================================== Pokémon ★

        /// <summary>Base for the Ray Poké-Powers of the Eeveelution ★s: when put onto your Bench from your hand, something happens to each Active Pokémon.</summary>
        private abstract class RayStar : Mon
        {
            protected abstract string Power { get; }
            protected abstract string Question { get; }
            protected abstract void Ray(EffectContext ctx, PokemonInPlay active);
            protected abstract Mon Attacks { get; }

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Engine.AbilityBlocked(me, Power)) return;
                if (!TriggerOnce(ctx, Power, ctx.Me.Active != null || ctx.Opp.Active != null, "Use " + Power + "? " + Question)) return;
                foreach (var active in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (active != null && ctx.InPlay(active)) Ray(ctx, active);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Crimson Ray (Poké-Power): each Active Pokémon (both players') is now Burned. Flamethrower.</summary>
        private sealed class FlareonStarCrimsonRay : RayStar
        {
            private static readonly Mon Flame = new CardsASC.SelfDiscardTypeMon("Flamethrower", EnergyType.Fire, 1);

            protected override string Power { get { return "Crimson Ray"; } }
            protected override string Question { get { return "Each Active Pokémon (both yours and your opponent's) is now Burned."; } }
            protected override void Ray(EffectContext ctx, PokemonInPlay active) { ctx.ApplyCondition(active, SpecialCondition.Burned); }
            protected override Mon Attacks { get { return Flame; } }
        }

        /// <summary>Yellow Ray (Poké-Power): 1 damage counter on each Active Pokémon (both players'). Agility.</summary>
        private sealed class JolteonStarYellowRay : RayStar
        {
            private static readonly Mon Agility = new DodgeMon("Agility", false);

            protected override string Power { get { return "Yellow Ray"; } }
            protected override string Question { get { return "Put 1 damage counter on each Active Pokémon (both yours and your opponent's)."; } }
            protected override void Ray(EffectContext ctx, PokemonInPlay active) { ctx.PlaceCounters(active, 1); }
            protected override Mon Attacks { get { return Agility; } }
        }

        /// <summary>Blue Ray (Poké-Power): remove all Special Conditions and 3 damage counters from each Active Pokémon (both players'). Whirlpool.</summary>
        private sealed class VaporeonStarBlueRay : RayStar
        {
            private static readonly Mon Whirlpool = new CardsASC.FlipDiscardDefendingEnergyMon("Whirlpool");

            protected override string Power { get { return "Blue Ray"; } }
            protected override string Question { get { return "Remove all Special Conditions and 3 damage counters from each Active Pokémon (both yours and your opponent's)."; } }

            protected override void Ray(EffectContext ctx, PokemonInPlay active)
            {
                ctx.Engine.RemoveAllConditions(active);
                if (active.DamageCounters > 0) ctx.Heal(active, 30);
            }

            protected override Mon Attacks { get { return Whirlpool; } }
        }
    }
}
