using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Ultra Prism (UPR, v85), part 1: Grass, Fire and Water (UPR 1-42). No regulation mark: Expanded. Leafeon-GX and Glaceon-GX
    /// were already in the pool (Hidden Fates Shiny Vault) and keep their scripts; Snover, Torterra, Infernape and Empoleon have
    /// the same attacks as their Forbidden Light prints and reuse those scripts. "A [T] Energy card" outside play is basic Energy
    /// only (your ruling, v65).
    /// </summary>
    public static class CardsUPR1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("UPR-1", new CardsASR1.TimesMon("Continuous Eggsplosion", c => c.FlipUntilTails()));
            r.RegisterByKey("UPR-2", new DodgeMon("Super Speed", false));
            r.RegisterByKey("UPR-3", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("UPR-4", new CardsFFI2.MachineGunHeadbuttMon("Petal Dance"));
            r.RegisterByKey("UPR-5", new RoseradeUPR());
            r.RegisterByKey("UPR-6", new CardsVIV1.SynthesisMon());
            r.RegisterByKey("UPR-8", new CardsTWM1.HealSelfMon("Mega Drain", 30));
            Same(r, "UPR-9", "FLI-58");
            r.RegisterByKey("UPR-10", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("UPR-11", new CherrimUPR());
            r.RegisterByKey("UPR-12", new ComboMon(new CardsTWM1.HealSelfMon("Chomp Chomp", 20), new LessPerSelfCounterMon("Loom Over", 10)));
            r.RegisterByKey("UPR-14", new RotoMotorMon(new SpecialMowMon()));
            r.RegisterByKey("UPR-15", new ComboMon(new CoaxMon(), new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep)));
            r.RegisterByKey("UPR-16", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("UPR-17", new AraquanidUPR());
            // Fire
            r.RegisterByKey("UPR-18", new ComboMon(new CardsPBL.ControlledBurnMon(1), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("UPR-19", new MagmortarUPR());
            r.RegisterByKey("UPR-21", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("UPR-22", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            Same(r, "UPR-23", "FLI-59");
            r.RegisterByKey("UPR-24", new RotoMotorMon());
            r.RegisterByKey("UPR-26", new ComboMon(new CardsCES1.ConditionsMon("Panic Poison", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned),
                new CardsFST2.ConditionBonusMon("Hunter's Nails", 60, c => c.Defending != null && c.Defending.Conditions != SpecialCondition.None)));
            r.RegisterByKey("UPR-27", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            // Water
            r.RegisterByKey("UPR-28", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("UPR-29", new SpikeArmorMon());
            r.RegisterByKey("UPR-30", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("UPR-31", new CardsMEG.SnipeMon("Splatter", 20));
            r.RegisterByKey("UPR-33", new ParalyzeFlipMon("Bubble Beam"));
            Same(r, "UPR-34", "FLI-81");
            r.RegisterByKey("UPR-36", new ComboMon(new DodgeMon("Agility", false), new CardsASC.SelfDiscardTypeMon("Aqua Blast", EnergyType.Water, 1)));
            Same(r, "UPR-37", "FLI-3");
            r.RegisterByKey("UPR-38", new BlessingsOfTheFrostMon(EnergyType.Water));
            r.RegisterByKey("UPR-40", new RotoMotorMon(new CardsMEG.SnipeBenchMon("Wash Arrow", 50)));
            r.RegisterByKey("UPR-41", new RotoMotorMon(new CardsASR1.PlusMon("Frost Crush", 20, c => c.OpponentPokemon().Sum(m => c.EnergyCount(m)))));
            r.RegisterByKey("UPR-42", new ComboMon(new DeepCurrentsMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
        }

        // ================================================================== Shared helpers

        /// <summary>Registers a card under the script another set's print of the same Pokémon already has (same attacks and Abilities).</summary>
        internal static void Same(CardScriptRegistry r, string key, string existingKey)
        {
            var script = r.ForKey(existingKey);
            if (script == null) throw new InvalidOperationException(key + ": no script registered for " + existingKey);
            r.RegisterByKey(key, script);
        }

        /// <summary>
        /// Roto Motor: with 9 or more Pokémon Tool cards in your discard pile, ignore all Energy in this Pokémon's attack costs (a set
        /// cost, so Sticky Membrane can't add to it: Compendium, 2018-05-03). The attacks are handled by the given parts.
        /// </summary>
        internal sealed class RotoMotorMon : Mon
        {
            private readonly Mon[] parts;

            public RotoMotorMon(params Mon[] parts) { this.parts = parts; }

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                var me = ctx.Pokemon;
                if (me == null || ctx.Me.Discard.Count(c => c.Def.IsTool) < 9) return;
                if (ctx.Engine.AbilityBlocked(me, "Roto Motor")) return;
                cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                foreach (var part in parts)
                    if (part.HandleAttack(ctx, atk)) return true;
                return false;
            }
        }

        /// <summary>"This attack does N less damage for each damage counter on this Pokémon" (Loom Over).</summary>
        internal sealed class LessPerSelfCounterMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public LessPerSelfCounterMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int damage = Math.Max(0, atk.Damage - per * ctx.Pokemon.DamageCounters);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log(ctx.Pokemon.Name + "'s " + attack + " did no damage.");
                return true;
            }
        }

        /// <summary>Blessings of the Frost: when you evolve into this Pokémon from your hand, you may attach a [T] Energy card from your discard pile to 1 of your Pokémon. Hypno Hammer: Asleep.</summary>
        internal sealed class BlessingsOfTheFrostMon : Mon
        {
            private static readonly Mon Hammer = new ConditionAttackMon("Hypno Hammer", SpecialCondition.Asleep);
            private readonly EnergyType type;

            public BlessingsOfTheFrostMon(EnergyType type) { this.type = type; }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (!TriggerOnce(ctx, "Blessings of the Frost", energy.Count > 0, "Use Blessings of the Frost? Attach a " + type + " Energy card from your discard pile to 1 of your Pokémon.")) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Blessings of the Frost: attach the " + type + " Energy to which Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(energy[0])) return;
                ctx.Engine.AttachEnergyCard(energy[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hammer.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>
        /// Inviting Poison: switch 1 of the opponent's Benched Pokémon with their Active Pokémon; the new Active Pokémon is Poisoned.
        /// Flower Tornado: move any number of Grass Energy from your Pokémon to your other Pokémon in any way you like.
        /// </summary>
        private sealed class RoseradeUPR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Inviting Poison")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var target = ctx.GustOpponent("Inviting Poison: switch which of your opponent's Benched Pokémon into the Active Spot?");
                    if (target != null && ctx.IsActive(target)) ctx.ApplyCondition(target, SpecialCondition.Poisoned);
                    return true;
                }
                if (atk.Name != "Flower Tornado") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.MyPokemon().Count < 2) return true;
                var holders = new Dictionary<CardInstance, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var e in mon.Energy)
                        if (CardsDRI1.ProvidesType(ctx, mon, e, EnergyType.Grass)) holders[e] = mon;
                if (holders.Count == 0) return true;
                var picks = ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Flower Tornado: choose any number of Grass Energy to move");
                foreach (var e in picks)
                {
                    var origin = holders[e];
                    if (!ctx.InPlay(origin) || !origin.Energy.Contains(e)) continue;
                    var dest = ctx.ChoosePokemon(ctx.MyPokemon(m => m != origin), "Flower Tornado: move " + e.Def.Name + " from " + origin.Name + " to which Pokémon?");
                    if (dest != null) ctx.Engine.MoveEnergy(origin, dest, e);
                }
                return true;
            }
        }

        /// <summary>Weather Guard: your Grass Pokémon have no Weakness.</summary>
        private sealed class CherrimUPR : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner == src.Player && src.Engine.IsType(mon, EnergyType.Grass);
            }
        }

        /// <summary>Special Mow: discard a Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class SpecialMowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Mow") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var special = defending.Energy.Where(e => e.Def.IsAttachedSpecialEnergy).ToList();
                if (special.Count == 0) return true;
                var pick = ctx.ChooseCards(special, 1, 1, "Special Mow: discard which Special Energy?");
                if (pick.Count > 0) ctx.Engine.DiscardEnergy(defending, pick[0]);
                return true;
            }
        }

        /// <summary>Coax: search your deck for up to 3 Basic Pokémon of different types and put them onto your Bench.</summary>
        internal sealed class CoaxMon : Mon
        {
            private static bool SharesType(CardDef a, CardDef b)
            {
                return a.HasType(b.PokemonType) || (b.SecondType.HasValue && a.HasType(b.SecondType.Value));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coax") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int max = Math.Min(3, ctx.BenchSpace);
                if (max <= 0 || ctx.Me.Deck.Count == 0) return true;
                var chosen = new List<CardDef>();
                for (int i = 0; i < max && ctx.BenchSpace > 0; i++)
                {
                    var sofar = chosen.ToList();
                    var found = ctx.SearchMyDeck(d => d.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d) && !sofar.Any(o => SharesType(o, d)), 1,
                        "Coax: choose a Basic Pokémon of a different type (" + (i + 1) + " of up to " + max + ")");
                    if (found.Count == 0) break;
                    chosen.Add(found[0].Def);
                    ctx.Engine.PutOntoBench(ctx.Player, found[0], false);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Water Bubble: prevent all damage done to this Pokémon by attacks from the opponent's Fire Pokémon.</summary>
        private sealed class AraquanidUPR : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                return me != null && info.Target == me && attacker != null && attacker.Owner != me.Owner && src.Engine.IsType(attacker, EnergyType.Fire);
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Incandescent Body: if this Pokémon is your Active Pokémon and is damaged by an opponent's attack (even if it's Knocked Out),
        /// the Attacking Pokémon is now Burned. Fire Blaster: you may discard 2 Energy from this Pokémon for 80 more damage.
        /// </summary>
        private sealed class MagmortarUPR : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (holderWasActive && attacker != null && attacker.Owner != ctx.Player && ctx.IsActive(attacker)) ctx.ApplyCondition(attacker, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Blaster") return false;
                var me = ctx.Pokemon;
                int bonus = 0;
                if (me.Energy.Count >= 2 && ctx.AskYesNo("Fire Blaster: discard 2 Energy from " + me.Name + " for 80 more damage?"))
                    bonus = ctx.DiscardEnergyFrom(me, 2, "Fire Blaster: discard 2 Energy") >= 2 ? 80 : 0;
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Spike Armor: during the opponent's next turn, if this Pokémon is damaged by an attack (even if it's Knocked Out), put 6 damage counters on the Attacking Pokémon.</summary>
        private sealed class SpikeArmorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spike Armor") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new RetaliateEffect(me.Name + " puts 6 damage counters on a Pokémon that damages it during the opponent's next turn (Spike Armor)", ctx.Player, me.Uid, ctx.Turn + 1, 6));
                return true;
            }
        }

        /// <summary>Deep Currents: shuffle 5 Water Energy cards from your discard pile into your deck (as many as there are, up to 5).</summary>
        private sealed class DeepCurrentsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Currents") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var water = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (water.Count == 0) return true;
                int n = Math.Min(5, water.Count);
                var picks = water.Count <= n ? water : ctx.ChooseCards(water, n, n, "Deep Currents: shuffle which " + n + " Water Energy cards into your deck?");
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }
    }
}
