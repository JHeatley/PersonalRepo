using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Black &amp; White (BLW 1-115, 2011; Expanded). Cards reprinted in Legendary Treasures (Snivy 2, Servine 4, Serperior 6, Tepig 16,
    /// Pignite 18, Emboar 20, Oshawott 28, Dewott 30, Samurott 32, Bouffalant 91, Minccino 88), Celebrations' Classic Collection
    /// (Reshiram, Zekrom), Dragons Exalted (Reuniclus), Next Destinies (Zoroark) and Emerging Powers (Patrat 78), and the Trainers
    /// and basic Energy already in the pool, are prints of those. Your rulings (v132): Pike also hits the Defending Pokémon (like
    /// Flame Burst); PlusPower is Base Set's PlusPower, now legal in Expanded as well as Unlimited.
    /// Pokémon whose attacks only do their printed damage need no script: Snivy 1, Pansage, Tepig 15, Pansear, Oshawott 27, Panpour,
    /// Ducklett, Alomomola 38, Blitzle 40, Woobat, Venipede, Timburr, Sawk, Purrloin, Scraggy, Patrat 77 and the rest of the plain attacks.
    /// </summary>
    public static class CardsBLW
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BLW-3", new ParalyzeFlipMon("Wrap"));
            r.RegisterByKey("BLW-5", new CardsPHF1.HealTypeEachMon("Leaf Storm", EnergyType.Grass, 20));
            r.RegisterByKey("BLW-8", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("BLW-9", new MagicalLeafMon());
            r.RegisterByKey("BLW-10", new ComboMon(new PetalDanceMon("Petal Dance"), new CardsPHF1.HealTypeEachMon("Leaf Storm", EnergyType.Grass, 20)));
            r.RegisterByKey("BLW-11", new ComboMon(new CardsTWM1.HealSelfMon("Mega Drain", 20), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("BLW-12", new ComboMon(new ConstantRattleMon(), new CardsDAA1.DrainMon("Giga Drain")));
            r.RegisterByKey("BLW-13", new ComboMon(new CoinsMon("Double Kick", 2), new CardsTWM1.HealSelfMon("Leech Seed", 10)));
            r.RegisterByKey("BLW-14", new ComboMon(
                new CardsASR1.PlusMon("Nature Power", 10, c => c.MyPokemon().Concat(c.OpponentPokemon()).Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Grass))),
                new CardsTWM1.HealSelfMon("Horn Leech", 20)));
            r.RegisterByKey("BLW-17", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("BLW-19", new CardsSUM1.DiscardTypeFromSelfMon("Flare Blitz", EnergyType.Fire, 99));
            r.RegisterByKey("BLW-22", new ComboMon(new CardsFST1.TwoBenchedMon("Flame Burst", 20), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("BLW-23", new FlipBonusMon("Firebreathing", 10));
            r.RegisterByKey("BLW-24", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("BLW-25", new ComboMon(new ConditionAttackMon("Fire Fang", SpecialCondition.Burned), new CardsEPO.ThrashMon("Thrash")));
            r.RegisterByKey("BLW-29", new FlipBonusMon("Razor Shell", 20));
            r.RegisterByKey("BLW-31", new Cards30CC.BenchHitMon("Pike", 30));
            r.RegisterByKey("BLW-34", new ComboMon(new ConditionAttackMon("Scald", SpecialCondition.Burned), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("BLW-35", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("BLW-37", new ComboMon(new CardsEPO.PowerUpMon("Feather Dance", 40), new SwitchSelfMon("Aqua Ring", false)));
            r.RegisterByKey("BLW-39", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep), new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10)));
            r.RegisterByKey("BLW-41", new FlipBonusMon("Stomp", 10));
            r.RegisterByKey("BLW-42", new ComboMon(new FlipBonusMon("Stomp", 20), new RecoilMon("Wild Charge", 10)));
            r.RegisterByKey("BLW-43", new ElectrisparkMon("Electrispark", 10));
            r.RegisterByKey("BLW-44", new ParalyzeFlipMon("Stun Needle"));
            r.RegisterByKey("BLW-45", new CardsDAA1.DrainMon("Leech Life"));
            r.RegisterByKey("BLW-46", new ComboMon(new NoRetreatMon("Electroweb"), new CardsDAA1.DrainMon("Leech Life")));
            r.RegisterByKey("BLW-48", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new DreamEaterMon()));
            r.RegisterByKey("BLW-49", new ComboMon(new ConditionAttackMon("Hypnotic Ray", SpecialCondition.Asleep), new DreamEaterMon()));
            r.RegisterByKey("BLW-51", new CardsMEG.SandAttackMon("Attract"));
            r.RegisterByKey("BLW-53", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("BLW-54", new ComboMon(new CardsFLF1.NoResistanceMon("Steamroller"), new ConditionAttackMon("Poison Claws", SpecialCondition.Poisoned)));
            r.RegisterByKey("BLW-55", new CellCultureMon());
            r.RegisterByKey("BLW-56", new CardsLOR1.RecoverMon());
            r.RegisterByKey("BLW-60", new CardsEPO.PowerUpMon("Bulk Up", 20));
            r.RegisterByKey("BLW-61", new ComboMon(new CardsDRI1.WhirlwindMon("Circle Throw"), new CardsFLF1.NoResistanceMon("Storm Throw")));
            r.RegisterByKey("BLW-63", new NoRetreatMon("Sand Tomb"));
            r.RegisterByKey("BLW-64", new CardsPOR.MemoryLockMon("Torment"));
            r.RegisterByKey("BLW-65", new ComboMon(new CardsPOR.MemoryLockMon("Torment"),
                new CardsFST2.ConditionBonusMon("Krookoroll", 40, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("BLW-67", new ComboMon(new CardsDAA1.GustAttackMon("Taunt", false), new Cards30C.TypeEnergyBonusMon("Sucker Punch", EnergyType.Darkness, 30)));
            r.RegisterByKey("BLW-69", new SpitAcidMon());
            r.RegisterByKey("BLW-70", new TailsNothingMon("Lunge"));
            r.RegisterByKey("BLW-72", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("BLW-73", new ComboMon(new BlindsideMon(),
                new CardsFST2.ConditionBonusMon("Punishment", 60, c => c.Defending != null && c.Defending.Def.Stage == Stage.Stage2)));
            r.RegisterByKey("BLW-74", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("BLW-75", new ComboMon(new ParalyzeFlipMon("Bind"), new CoinsMon("Gear Grind", 2)));
            r.RegisterByKey("BLW-76", new KlinklangShiftGear());
            r.RegisterByKey("BLW-79", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new TailsNothingMon("Hyper Fang")));
            r.RegisterByKey("BLW-80", new PickupMon());
            r.RegisterByKey("BLW-81", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("BLW-82", new DrawAttackMon("Collect", 3));
            r.RegisterByKey("BLW-83", new ComboMon(new OdorSleuthMon(3), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("BLW-84", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("BLW-85", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("BLW-86", new DodgeMon("Fly", true));
            r.RegisterByKey("BLW-87", new CoinsMon("Doubleslap", 2));
            r.RegisterByKey("BLW-89", new ComboMon(new CoinsMon("Tail Slap", 2), new CardsASR1.TimesMon("Do the Wave", c => c.Me.Bench.Count)));
            r.RegisterByKey("BLW-90", new CardsWHT.FlipMoreMon("Double Stomp", 2, 20));
            r.RegisterByKey("BLW-115", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning), new CardsTWM1.SelfDiscardMon("Thunderbolt", 99)));
        }

        /// <summary>Magical Leaf: flip; heads, 10 more damage and heal 10 from this Pokémon.</summary>
        private sealed class MagicalLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Leaf") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                var me = ctx.Pokemon;
                if (heads && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Petal Dance: flip 3 coins, 30 per heads; this Pokémon is now Confused. Used for Petal Dance, Raging Pecks (the attack name is passed in).</summary>
        internal sealed class PetalDanceMon : Mon
        {
            private readonly string attack;

            public PetalDanceMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Constant Rattle: flip 3 coins; 1 heads 10 damage, 2 heads 30, 3 heads 60.</summary>
        private sealed class ConstantRattleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Constant Rattle") return false;
                int heads = ctx.FlipCoins(3);
                int dmg = heads == 1 ? 10 : heads == 2 ? 30 : heads == 3 ? 60 : 0;
                if (dmg == 0) return Nothing(ctx);
                ctx.DamageDefending(dmg);
                return true;
            }
        }

        /// <summary>Electrispark: also 10 to each of the opponent's Benched Pokémon. Used for Electrispark, Liquid Lashing (the attack name and the numbers are passed in).</summary>
        internal sealed class ElectrisparkMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public ElectrisparkMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Dream Eater: if the Defending Pokémon isn't Asleep, this attack does nothing.</summary>
        internal sealed class DreamEaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dream Eater") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Asleep)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Cell Culture: put a Solosis from your deck onto your Bench.</summary>
        private sealed class CellCultureMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cell Culture") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.Name == "Solosis", 1, "Cell Culture: put a Solosis from your deck onto your Bench");
                return true;
            }
        }

        /// <summary>Spit Acid: the Defending Pokémon is now Burned; flip, heads it's also Paralyzed.</summary>
        private sealed class SpitAcidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spit Acid") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool still = TefPokemon.StillDefending(ctx, defending);
                if (still) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                if (ctx.Flip() && still) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Blindside: 50 to 1 of the opponent's Pokémon that has any damage counters on it.</summary>
        private sealed class BlindsideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blindside") return false;
                var targets = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (targets.Count == 0) return Nothing(ctx);
                var target = ctx.ChoosePokemon(targets, "Blindside: do 50 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, 50);
                return true;
            }
        }

        /// <summary>Shift Gear: as often as you like during your turn, move a Metal Energy from 1 of your Pokémon to another of your Pokémon. Gear Grind: 2 coins.</summary>
        private sealed class KlinklangShiftGear : Mon
        {
            private const string Shift = "Shift Gear";
            private static readonly Mon GearGrind = new CoinsMon("Gear Grind", 2);

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Metal)));
            }

            private static bool CanMove(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == Shift && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Shift + ": move a Metal Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var metal = from.Energy.Where(e => CardsDRI1.ProvidesType(ctx, from, e, EnergyType.Metal)).ToList();
                if (metal.Count == 0) return false;
                var card = metal.Select(e => e.Def.Name).Distinct().Count() == 1 ? metal[0] : ctx.ChooseCards(metal, 1, 1, Shift + ": move which Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), Shift + ": move it from " + from.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(from, to, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return GearGrind.HandleAttack(ctx, atk); }
        }

        /// <summary>Pickup: put an Item card from your discard pile into your hand.</summary>
        private sealed class PickupMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pickup") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                ctx.DiscardToHand(items.Count == 1 ? items : ctx.ChooseCards(items, 1, 1, "Pickup: put which Item card from your discard pile into your hand?"));
                return true;
            }
        }

        /// <summary>Odor Sleuth: flip 3 coins; for each heads, put a card from your discard pile into your hand. The numbers are passed in.</summary>
        internal sealed class OdorSleuthMon : Mon
        {
            private readonly int amount;

            public OdorSleuthMon(int amount)
            {
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Odor Sleuth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(amount);
                int n = Math.Min(heads, ctx.Me.Discard.Count);
                if (n <= 0) return true;
                ctx.DiscardToHand(ctx.ChooseCards(ctx.Me.Discard.ToList(), n, n, "Odor Sleuth: put " + n + (n == 1 ? " card" : " cards") + " from your discard pile into your hand"));
                return true;
            }
        }
    }
}
