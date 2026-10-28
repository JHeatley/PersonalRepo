using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Dragon Majesty (DRM, v82), part 1: Fire and Water (DRM 3-31). No regulation mark: Expanded. Charmander, Charmeleon,
    /// Reshiram-GX, Wooper and Quagsire were already in the pool as Hidden Fates Shiny Vault cards. "A [T] Energy card" outside
    /// play is basic Energy only (your ruling, v65).
    /// </summary>
    public static class CardsDRM1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fire
            r.RegisterByKey("DRM-3", new CharizardDRM());
            r.RegisterByKey("DRM-4", new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("DRM-5", new CombuskenDRM());
            r.RegisterByKey("DRM-6", new BlazikenDRM());
            r.RegisterByKey("DRM-7", new InfinityMon());
            r.RegisterByKey("DRM-8", new DamageRushMon());
            r.RegisterByKey("DRM-9", new ComboMon(new HeatAssistMon(), new CardsASR1.PlusMon("Darmani-Hands", 50, c => c.FlipCoins(4))));
            r.RegisterByKey("DRM-10", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned),
                new CardsLOT1.DoesNothingUnlessMon("Charring Breath", c => c.Defending != null && c.Defending.Has(SpecialCondition.Burned))));
            r.RegisterByKey("DRM-12", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("DRM-13", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("DRM-14", new ComboMon(new CardsASR1.RingOfFireMon(), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            // Water
            r.RegisterByKey("DRM-15", new CardsMEG.SnipeMon("Splatter", 20));
            r.RegisterByKey("DRM-16", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10));
            r.RegisterByKey("DRM-17", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20));
            r.RegisterByKey("DRM-18", new ComboMon(new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 50), new SwitchSelfMon("Reverse Thrust", false), new MaelstromMon()));
            r.RegisterByKey("DRM-19", new FlipBonusMon("Water Splash", 10));
            r.RegisterByKey("DRM-20", new GyaradosDRM());
            r.RegisterByKey("DRM-21", new ComboMon(new CardsMEG.SnipeBenchMon("Aqua Bullet", 20), new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10)));
            r.RegisterByKey("DRM-22", new ComboMon(new ParalyzeFlipMon("Leer"), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("DRM-23", new CardsBRS1.MillSelfMon("Sweep Away", 3));
            r.RegisterByKey("DRM-24", new FeraligatrDRM());
            r.RegisterByKey("DRM-27", new BubbleShootMon());
            r.RegisterByKey("DRM-28", new SubmergeMon());
            r.RegisterByKey("DRM-29", new ComboMon(new ConditionAttackMon("Aurora Wave", SpecialCondition.Confused), new AquaSplitMon()));
            r.RegisterByKey("DRM-30", new PhioneDRM());
            r.RegisterByKey("DRM-31", new WishiwashiDRM());
        }

        // ================================================================== Shared helpers

        /// <summary>Submerge, Snowed In, Cosmic Guard: while this Pokémon is on your Bench, prevent all damage done to it by attacks (both players').</summary>
        internal sealed class SubmergeMon : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && !src.IsActive(me);
            }
        }

        /// <summary>"You may discard any Stadium card in play" after the damage (Wild Tail, Knock Over).</summary>
        internal static void MayDiscardStadium(EffectContext ctx, string attack)
        {
            if (ctx.State.Stadium != null && ctx.AskYesNo(attack + ": discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
        }

        // ================================================================== Fire

        /// <summary>Resolute Flame: this Pokémon's attacks do 30 more damage to the opponent's Active Pokémon for each of their Pokémon-GX and Pokémon-EX in play. Fiery Blast: discard an Energy.</summary>
        private sealed class CharizardDRM : Mon
        {
            private static readonly Mon Blast = new CardsTWM1.SelfDiscardMon("Fiery Blast", 1);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Attacker != me || !info.ToOpponentsActive || damage <= 0) return damage;
                int gx = src.Opp.InPlay().Count(m => m.Def.IsGXorEX);
                return damage + 30 * gx;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        /// <summary>Natural Cure: whenever you attach an Energy card from your hand to this Pokémon, remove all Special Conditions from it. Lunge: tails, nothing.</summary>
        private sealed class CombuskenDRM : Mon
        {
            private static readonly Mon Lunge = new TailsNothingMon("Lunge");

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || me.Conditions == SpecialCondition.None) return;
                Announce(src, "Natural Cure");
                src.Engine.RemoveAllConditions(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lunge.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Firestarter: once during your turn, attach a Fire Energy card from your discard pile to 1 of your Benched Pokémon. Fire
        /// Stream: discard a Fire Energy from this Pokémon; 20 damage to each of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class BlazikenDRM : Mon
        {
            protected override string Activated { get { return "Firestarter"; } }

            private static List<CardInstance> Fire(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && Fire(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fire = Fire(ctx);
                if (fire.Count == 0) return;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Firestarter: attach a Fire Energy card from your discard pile to which Benched Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(fire[0])) return;
                ctx.Engine.AttachEnergyCard(fire[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Stream") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 1, "Fire Stream: discard a Fire Energy from " + me.Name, c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Fire));
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Infinity (Victini ◇): 20 damage for each basic Energy card in your discard pile; then shuffle those cards into your deck.</summary>
        private sealed class InfinityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Infinity") return false;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0) ctx.DamageDefending(atk.Damage * energy.Count);
                else ctx.Log("Infinity did no damage.");
                if (energy.Count > 0)
                {
                    ctx.DiscardToDeck(energy);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Damage Rush: flip a coin until you get tails; 30 more damage for each heads.</summary>
        private sealed class DamageRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Rush") return false;
                int heads = ctx.FlipUntilTails();
                ctx.DamageDefending(atk.Damage + 30 * heads);
                return true;
            }
        }

        /// <summary>Heat Assist: attach up to 3 Fire Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class HeatAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heat Assist") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Hand.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count > 0) ctx.AttachCardsAnyWay(fire, 3, (c, m) => true, "Heat Assist: attach Fire Energy cards from your hand");
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Maelstrom-GX: 40 damage to each of the opponent's Pokémon.</summary>
        private sealed class MaelstromMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Maelstrom-GX") return false;
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 40);
                return true;
            }
        }

        /// <summary>
        /// Commotion: if this is your Active Pokémon and it's damaged by an opponent's attack (even if it's Knocked Out), put 2 damage
        /// counters on each of your Benched Pokémon. Wild Tail: you may discard any Stadium card in play.
        /// </summary>
        private sealed class GyaradosDRM : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || ctx.Me.Bench.Count == 0) return;
                Announce(ctx, "Commotion");
                foreach (var mon in ctx.MyBench()) ctx.Engine.PlaceDamageCounters(mon, 2, "Commotion");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wild Tail") return false;
                ctx.DamageDefending(atk.Damage);
                MayDiscardStadium(ctx, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Downpour: as often as you like during your turn, discard a Water Energy card from your hand. Riptide: 20 more damage for
        /// each Water Energy card in your discard pile; then shuffle those cards into your deck.
        /// </summary>
        private sealed class FeraligatrDRM : Mon
        {
            private static List<CardInstance> WaterInHand(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Water)).ToList(); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != "Downpour") return false;
                return ctx.IsMyTurn && WaterInHand(ctx).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                var water = WaterInHand(ctx);
                if (water.Count == 0) return;
                var card = water.Select(c => c.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Downpour: discard a Water Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Riptide") return false;
                var water = ctx.Me.Discard.Where(c => CardsTEU1.BasicOf(c.Def, EnergyType.Water)).ToList();
                ctx.DamageDefending(atk.Damage + 20 * water.Count);
                if (water.Count > 0)
                {
                    ctx.DiscardToDeck(water);
                    ctx.ShuffleMyDeck();
                }
                return true;
            }
        }

        /// <summary>Bubble Shoot: 20 damage times the Water Energy attached to this Pokémon, to 1 of the opponent's Pokémon.</summary>
        private sealed class BubbleShootMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bubble Shoot") return false;
                int water = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                if (water > 0 && ctx.Opp.HasPokemonInPlay) ctx.DamageAnyOpponent(20 * water);
                else ctx.Log("Bubble Shoot did no damage.");
                return true;
            }
        }

        /// <summary>Aqua Split: 40 damage to 2 of the opponent's Benched Pokémon (each takes the full 40).</summary>
        private sealed class AquaSplitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Split") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                CardsLOR1.DamageBenched(ctx, 40, 2, atk.Name);
                return true;
            }
        }

        /// <summary>Murmurs of the Sea: your Water Pokémon can't be Confused, and stop being Confused. Water Pulse: Asleep.</summary>
        private sealed class PhioneDRM : Mon
        {
            private static readonly Mon Pulse = new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep);

            public override bool CuresConditionsItPrevents { get { return true; } }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                if (src.Pokemon == null || target == null || target.Owner != src.Player || condition != SpecialCondition.Confused) return false;
                return src.Engine.IsType(target, EnergyType.Water);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pulse.HandleAttack(ctx, atk); }
        }

        /// <summary>Meet Up: your Wishiwashi-GX in play get +20 HP, and their attacks do 20 more damage to the opponent's Active Pokémon (each Wishiwashi adds its own).</summary>
        private sealed class WishiwashiDRM : Mon
        {
            private static bool IsWishiwashiGX(PokemonInPlay mon) { return mon != null && mon.Name == "Wishiwashi-GX"; }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && IsWishiwashiGX(mon) ? hp + 20 : hp;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return IsWishiwashiGX(attacker) ? damage + 20 : damage;
            }
        }
    }
}
