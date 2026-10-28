using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crimson Invasion (CIN, v86), part 1: Grass, Fire and Water (CIN 1-29). No regulation mark: Expanded. "A [T] Energy card"
    /// outside play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsCIN1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("CIN-1", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("CIN-2", new CardsUNM1.NamedBenchSearchMon("Multiply", "Kakuna", 3));
            r.RegisterByKey("CIN-3", new SuddenStingMon());
            r.RegisterByKey("CIN-4", new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep));
            r.RegisterByKey("CIN-5", new CardsFST2.ConditionBonusMon("Sucker Punch", 30, c => c.Pokemon.Energy.Any(e => CardsDRI1.ProvidesType(c, c.Pokemon, e, EnergyType.Darkness))));
            r.RegisterByKey("CIN-6", new ComboMon(new CardsPFL.DamagedBonusMon("Spike Rend", 60), new CardsWHT.DragOffMon("Hunt", 40)));
            r.RegisterByKey("CIN-7", new KarrablastCIN());
            r.RegisterByKey("CIN-8", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("CIN-9", new CardsLOR1.RecoverMon());
            r.RegisterByKey("CIN-11", new GogoatCIN());
            // Fire
            r.RegisterByKey("CIN-12", new AlolanMarowakCIN());
            r.RegisterByKey("CIN-13", new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"));
            r.RegisterByKey("CIN-14", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"), new ConditionAttackMon("Searing Flame", SpecialCondition.Burned)));
            // Water
            r.RegisterByKey("CIN-16", new StarmieCIN());
            r.RegisterByKey("CIN-17", new MagikarpCIN());
            r.RegisterByKey("CIN-18", new GyaradosGX());
            r.RegisterByKey("CIN-20", new RestMon());
            r.RegisterByKey("CIN-21", new ComboMon(new CardsWHT.FlipMoreMon("Double Stomp", 2, 30), new ForcefulTackleMon()));
            r.RegisterByKey("CIN-23", new ComboMon(new CardsUPR3.SmokescreenMon("Ink Spit", 1), new SpecialArtilleryMon()));
            r.RegisterByKey("CIN-25", new DoubleClawsMon());
            r.RegisterByKey("CIN-26", new DodgeMon("Splashing Dodge", false));
            r.RegisterByKey("CIN-27", new ComboMon(new TlcMon(), new CardsMEW1.SpreadBenchMon("Ocean Cyclone", 10)));
            r.RegisterByKey("CIN-28", new RegiceCIN());
            r.RegisterByKey("CIN-29", new CardsTWM1.HealSelfMon("Regeneration", 30));
        }

        // ================================================================== Grass

        /// <summary>Sudden Sting: if this Pokémon evolved from Kakuna during this turn, the opponent's Active Pokémon is now Paralyzed and Poisoned.</summary>
        private sealed class SuddenStingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Sting") return false;
                var me = ctx.Pokemon;
                bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Kakuna";
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!fresh || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Shell On: once during your turn, you may discard a Shelmet from your hand; if you do, evolve this Pokémon from your deck.</summary>
        private sealed class KarrablastCIN : Mon
        {
            protected override string Activated { get { return "Shell On"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c.Def.Name == "Shelmet") && ctx.Me.Deck.Count > 0 && !CardsEVS1.EvolveBlocked(ctx, ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                var shelmet = ctx.Me.Hand.FirstOrDefault(c => c.Def.Name == "Shelmet");
                if (shelmet == null) return;
                ctx.Engine.DiscardFromHand(ctx.Player, shelmet);
                CardsEVS1.EvolveFromDeck(ctx, ctx.Pokemon, "Shell On");
            }
        }

        /// <summary>Sap Sipper: this Pokémon's attacks do 80 more damage to the opponent's Grass Pokémon (before Weakness and Resistance). Horn Leech: heal 20.</summary>
        private sealed class GogoatCIN : Mon
        {
            private static readonly Mon Leech = new CardsTWM1.HealSelfMon("Horn Leech", 20);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || info.Target == null || info.Target.Owner == src.Player || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Grass) ? damage + 80 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Leech.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>
        /// Dance of Flames: for each Energy attached to the opponent's Pokémon, attach a Fire Energy card from your discard pile to
        /// your Pokémon in any way you like. Burning Bonemerang: flip 2 coins, 70 for each heads; any heads, Burned.
        /// </summary>
        private sealed class AlolanMarowakCIN : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Dance of Flames")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    int count = ctx.OpponentPokemon().Sum(m => m.Energy.Count);
                    var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                    if (count > 0 && fire.Count > 0)
                        ctx.AttachCardsAnyWay(fire, Math.Min(count, fire.Count), (card, mon) => true, "Dance of Flames: attach Fire Energy from your discard pile", false);
                    return true;
                }
                if (atk.Name != "Burning Bonemerang") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Escape: once during your turn, you may shuffle this Pokémon and all cards attached to it into your deck.</summary>
        private sealed class StarmieCIN : Mon
        {
            protected override string Activated { get { return "Escape"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon().Count > 1 || !ctx.PokemonIsActive; }

            protected override void Ability(EffectContext ctx)
            {
                bool wasActive = ctx.PokemonIsActive;
                ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                if (wasActive && ctx.Me.Active == null && ctx.Me.Bench.Count > 0) ctx.Engine.PromoteActive(ctx.Player);
            }
        }

        /// <summary>Submerge: as long as this Pokémon is on your Bench, prevent all damage done to it by attacks (both players'). Waterfall Evolution: evolve it from your deck.</summary>
        private sealed class MagikarpCIN : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.State.IsActive(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Waterfall Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsEVS1.EvolveFromDeck(ctx, ctx.Pokemon, "Waterfall Evolution");
                return true;
            }
        }

        /// <summary>
        /// Draconic Disaster: 100 more if any Stadium card is in play; then discard that Stadium. Dread Storm-GX: discard an Energy from
        /// each of the opponent's Pokémon.
        /// </summary>
        private sealed class GyaradosGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Draconic Disaster")
                {
                    bool stadium = ctx.State.Stadium != null;
                    ctx.DamageDefending(atk.Damage + (stadium ? 100 : 0));
                    if (stadium && ctx.State.Stadium != null) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name != "Dread Storm-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m)))
                {
                    var pick = ctx.ChooseCards(mon.Energy.ToList(), 1, 1, "Dread Storm-GX: discard which Energy from " + mon.Name + "?");
                    if (pick.Count > 0) ctx.Engine.DiscardEnergy(mon, pick[0]);
                }
                return true;
            }
        }

        /// <summary>Rest: this Pokémon is now Asleep; heal 90 damage from it.</summary>
        internal sealed class RestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                ctx.Heal(me, 90);
                return true;
            }
        }

        /// <summary>Forceful Tackle: you may put up to 9 damage counters on this Pokémon; 10 more damage for each.</summary>
        private sealed class ForcefulTackleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Forceful Tackle") return false;
                var me = ctx.Pokemon;
                int n = ctx.AskNumber(0, 9, "Forceful Tackle: put how many damage counters on " + me.Name + " (10 more damage each)?");
                int placed = 0;
                if (n > 0)
                {
                    int before = me.DamageCounters;
                    ctx.Engine.PlaceDamageCounters(me, n, "Forceful Tackle");
                    placed = Math.Max(0, me.DamageCounters - before);
                }
                ctx.DamageDefending(atk.Damage + 10 * placed);
                return true;
            }
        }

        /// <summary>Special Artillery: you may discard a Special Energy from this Pokémon for 80 more damage.</summary>
        private sealed class SpecialArtilleryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Artillery") return false;
                var me = ctx.Pokemon;
                var special = me.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                int bonus = 0;
                if (special.Count > 0 && ctx.AskYesNo("Special Artillery: discard a Special Energy from " + me.Name + " for 80 more damage?"))
                {
                    var pick = ctx.ChooseCards(special, 1, 1, "Special Artillery: discard which Special Energy?");
                    if (pick.Count > 0 && ctx.Engine.DiscardEnergy(me, pick[0])) bonus = 80;
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Double Claws: discard 2 Energy from the opponent's Active Pokémon.</summary>
        private sealed class DoubleClawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Claws") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending) || defending.Energy.Count == 0) return true;
                int n = Math.Min(2, defending.Energy.Count);
                var picks = defending.Energy.Count <= n ? defending.Energy.ToList() : ctx.ChooseCards(defending.Energy.ToList(), n, n, "Double Claws: discard which 2 Energy?");
                foreach (var e in picks) ctx.Engine.DiscardEnergy(defending, e);
                return true;
            }
        }

        /// <summary>TLC: shuffle 1 of the opponent's Benched Pokémon that has any damage counters, and all attached cards, into their deck.</summary>
        private sealed class TlcMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "TLC") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentBench().Where(m => m.DamageCounters > 0 && ctx.CanAffect(m)).ToList();
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "TLC: shuffle which of your opponent's damaged Benched Pokémon into their deck?");
                if (target != null && ctx.InPlay(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Iceberg Shield: with Regirock in play, prevent all effects of attacks, including damage, done to this Pokémon by the opponent's Stage 2 Pokémon.</summary>
        private sealed class RegiceCIN : Mon
        {
            private static bool Shielded(EffectContext src, PokemonInPlay target, PokemonInPlay attacker)
            {
                var me = src.Pokemon;
                if (me == null || target != me || attacker == null || attacker.Owner == src.Player) return false;
                return attacker.Def.Stage == Stage.Stage2 && src.Me.InPlay().Any(m => m.Def.Name == "Regirock");
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage) { return Shielded(src, info.Target, info.Attacker); }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Shielded(src, target, src.Engine.CurrentSourcePokemon);
            }
        }
    }
}
