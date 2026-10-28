using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Noble Victories (NVI 1-102, 2011; Expanded). Cards reprinted in Legendary Treasures (Sewaddle line, Virizion, Victini 14, Tympole
    /// line, Kyurem, Solosis line, Terrakion, Landorus, Cobalion, Druddigon), Next Destinies (Chandelure, Hydreigon), Dark Explorers
    /// (Archeops), Kalos Starter Set (Pawniard 81), Plasma Blast (Cover Fossil, Plume Fossil), Plasma Storm (Eviolite), 30th
    /// Celebration (N), Scarlet &amp; Violet (Rocky Helmet) and Paldea Evolved (Super Rod) are prints of those; 97-101 are the full
    /// arts. Your rulings (v130): Eelektrik's Dynamotor card is its own card (not Black Bolt's); Mysterious Evolution counts a
    /// Shelmet / Karrablast in play on either side.
    /// Pokémon whose attacks only do their printed damage need no script: Foongus, Frillish, Tirtouga, Tynamo (39), Lampent's
    /// Will-O-Wisp, Mienfoo, Deino and the rest of the plain attacks.
    /// </summary>
    public static class CardsNVI
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("NVI-4", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("NVI-5", new ComboMon(new AromaxMon(), new SwitchSelfMon("Windmill", false)));
            r.RegisterByKey("NVI-6", new CardsMEW1.WithdrawMon("Withdraw"));
            r.RegisterByKey("NVI-7", new ComboMon(new FlipBonusMon("X-Scissor", 50), new RecoilMon("Reckless Charge", 10)));
            r.RegisterByKey("NVI-8", new ComboMon(new MysteriousEvolutionMon("Shelmet"), new CoinsMon("Fury Attack", 3)));
            r.RegisterByKey("NVI-10", new ComboMon(new CardsCEC1.ToxicCECMon("Toxic"), new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("NVI-11", new MysteriousEvolutionMon("Karrablast"));
            r.RegisterByKey("NVI-12", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Acid Spray"), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("NVI-15", new VCreateMon("V-create"));
            r.RegisterByKey("NVI-16", new TailsNothingMon("Lunge"));
            r.RegisterByKey("NVI-17", new CoinsMon("Double Fire", 2));
            r.RegisterByKey("NVI-18", new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new CardsBLK.PeckOffMon("Incinerate")));
            r.RegisterByKey("NVI-19", new CardsFLF1.FlipTailsSelfDiscardMon("Ember"));
            r.RegisterByKey("NVI-20", new RecoilMon("Take Down", 10));
            r.RegisterByKey("NVI-21", new ComboMon(new FieryDanceMon(), new ConditionAttackMon("Heat Wave", SpecialCondition.Burned)));
            r.RegisterByKey("NVI-26", new CarracostaSolidRock());
            r.RegisterByKey("NVI-27", new FlipBonusMon("Icicle Barb", 10));
            r.RegisterByKey("NVI-28", new ParalyzeFlipMon("Ice Beam"));
            r.RegisterByKey("NVI-29", new DoubleFreezeMon());
            r.RegisterByKey("NVI-31", new JellicentCursedBody());
            r.RegisterByKey("NVI-32", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep), new CardsFST2.ConditionBonusMon("Ice Shard", 40, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Fighting))));
            r.RegisterByKey("NVI-33", new ComboMon(new CardsDAA1.GustAttackMon("Ice Chain", false), new FrostVanishMon()));
            r.RegisterByKey("NVI-35", new DodgeMon("Agility", false));
            r.RegisterByKey("NVI-36", new ComboMon(new FlipBonusMon("Quick Attack", 20), new FlipShockBoltMon()));
            r.RegisterByKey("NVI-37", new CardsFST2.ConditionBonusMon("Electrichain", 20, c => c.Me.Bench.Any(m => c.Engine.IsType(m, EnergyType.Lightning))));
            r.RegisterByKey("NVI-38", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("NVI-40", new EelektrikDynamotor());
            r.RegisterByKey("NVI-41", new ComboMon(new NoRetreatMon("Acid"), new RecoilMon("Wild Charge", 10)));
            r.RegisterByKey("NVI-42", new CardsFST1.FlipRecoilMon("Thunder", 30));
            r.RegisterByKey("NVI-43", new CardsBWP.BothHeadsOrNothingMon("V-blast"));
            r.RegisterByKey("NVI-44", new CardsPAR2.FlipConditionMon("Perplex", SpecialCondition.Confused));
            r.RegisterByKey("NVI-45", new FlipBonusMon("Ambush", 10));
            r.RegisterByKey("NVI-46", new ComboMon(new DamagriiigusMon(), new ConditionAttackMon("Perplex", SpecialCondition.Confused)));
            r.RegisterByKey("NVI-47", new CofagrigusDurableBody());
            r.RegisterByKey("NVI-48", new GarbageCollectionMon());
            r.RegisterByKey("NVI-49", new ComboMon(new NoRetreatMon("Gentle Wrap"), new ConditionAttackMon("Gunk Shot", SpecialCondition.Poisoned)));
            r.RegisterByKey("NVI-53", new ComboMon(new CoinsMon("Dizzy Punch", 2), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("NVI-54", new CardsTWM1.HealSelfMon("Calm Mind", 30));
            r.RegisterByKey("NVI-55", new BenchSearchAttackMon("First Contact", 2));
            r.RegisterByKey("NVI-56", new CardsBUS1.SynchronoiseMon());
            r.RegisterByKey("NVI-57", new CardsPAR2.FlipConditionMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("NVI-58", new SwitchSelfMon("Teleportation Burst", false));
            r.RegisterByKey("NVI-59", new CardsDAA1.GustAttackMon("Luring Light", false));
            r.RegisterByKey("NVI-61", new CoreCannonMon());
            r.RegisterByKey("NVI-62", new FlipBonusMon("Pummel", 10));
            r.RegisterByKey("NVI-63", new FlipBonusMon("Pummel", 20));
            r.RegisterByKey("NVI-64", new ConkeldurrCraftsmanship());
            r.RegisterByKey("NVI-65", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Chip Away"), new CardsWHT.FlipMoreMon("Swing Around", 2, 30)));
            r.RegisterByKey("NVI-66", new CardsWHT.FlipMoreMon("Acrobatics", 2, 20));
            r.RegisterByKey("NVI-68", new ComboMon(new CoinsMon("Trickle", 2), new ParalyzeFlipMon("Thundershock")));
            r.RegisterByKey("NVI-70", new CardsFLF1.NoResistanceMon("Feint"));
            r.RegisterByKey("NVI-71", new CardsPRC1.DynamicPunchMon(20));
            r.RegisterByKey("NVI-72", new ComboMon(new CardsSSP2.StompOffMon("Hammer Arm", 1), new CoinsMon("Hurricane Punch", 4)));
            r.RegisterByKey("NVI-75", new CardsPFL.FlipUntilTailsTimesMon("Iron Head"));
            r.RegisterByKey("NVI-76", new ComboMon(new CardsFST2.ConditionBonusMon("Finishing Blow", 50, c => c.Defending != null && c.Defending.DamageCounters > 0), new SwitchSelfMon("Night Slash", false)));
            r.RegisterByKey("NVI-78", new CoinsMon("Double Hit", 2));
            r.RegisterByKey("NVI-80", new ComboMon(new CardsASC.SelfGuardMon("Guard Press", 20), new CoinsMon("Twineedle", 2)));
            r.RegisterByKey("NVI-82", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energy Stream", EnergyType.Metal), new CardsMEG.EnergyBonusMon("Metal Scissors", EnergyType.Metal, 20)));
            r.RegisterByKey("NVI-83", new DevourMon());
            r.RegisterByKey("NVI-85", new CardsASR1.TimesMon("Do the Wave", c => c.Me.Bench.Count));
            r.RegisterByKey("NVI-86", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("NVI-87", new CoinsMon("Dual Chop", 2));
            r.RegisterByKey("NVI-88", new ComboMon(new CoinsMon("Dual Chop", 2), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("NVI-96", new Xtransceiver());
            r.RegisterByKey("NVI-102", new ComboMon(new CoinsMon("Fury Swipes", 3), new DrawAttackMon("Pay Day", 1)));
        }

        // ================================================================== Abilities

        /// <summary>Solid Rock: when attacks damage this Pokémon, flip a coin; heads, that damage is 50 less (after Weakness and Resistance). Crunch: discard an Energy from the Defending Pokémon.</summary>
        private sealed class CarracostaSolidRock : Mon
        {
            private static readonly Mon Crunch = new CardsDRI2.CrunchMon("Crunch");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || damage <= 0 || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                Announce(src, "Solid Rock");
                return src.Flip() ? Math.Max(0, damage - 50) : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Cursed Body: damaged by the opponent's attack while Active (even if Knocked Out), the Attacking Pokémon is now Confused. Hydro Pump: 20 more for each Water Energy.</summary>
        private sealed class JellicentCursedBody : Mon
        {
            private static readonly Mon Pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 20);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Confused)) Announce(ctx, "Cursed Body");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Dynamotor: once during your turn, attach a Lightning Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class EelektrikDynamotor : Mon
        {
            protected override string Activated { get { return "Dynamotor"; } }

            private static List<CardInstance> Choices(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Count > 0 && Choices(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var energy = Choices(ctx);
                var pick = ctx.ChooseCards(energy, 1, 1, "Dynamotor: attach which Lightning Energy from your discard pile?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Dynamotor: attach " + pick.Def.Name + " to which Benched Pokémon?");
                if (target == null) return;
                if (ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, target);
            }
        }

        /// <summary>Durable Body: if this Pokémon would be Knocked Out by damage from an attack, flip a coin; heads, it has 10 HP left. Ambush: heads, +20.</summary>
        private sealed class CofagrigusDurableBody : Mon
        {
            private static readonly Mon Ambush = new FlipBonusMon("Ambush", 20);

            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return false;
                Announce(src, "Durable Body");
                return src.Flip();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ambush.HandleAttack(ctx, atk); }
        }

        /// <summary>Craftsmanship: +20 HP for each Fighting Energy attached to this Pokémon. Top Down: flip until tails, discard the opponent's top card for each heads.</summary>
        private sealed class ConkeldurrCraftsmanship : Mon
        {
            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return hp;
                return hp + 20 * src.Engine.EnergyCountOfType(mon, EnergyType.Fighting);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Top Down") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        // ================================================================== Attacks

        /// <summary>Aromax: heal all damage from 1 of your Benched Pokémon.</summary>
        private sealed class AromaxMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aromax") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var damaged = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (damaged.Count == 0) return true;
                var target = ctx.ChoosePokemon(damaged, "Aromax: heal all damage from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Mysterious Evolution: if [the other Pokémon] is in play (either side; your ruling, v130), search your deck for a card that
        /// evolves from this Pokémon and evolve it.
        /// </summary>
        private sealed class MysteriousEvolutionMon : Mon
        {
            private readonly string partner;

            public MysteriousEvolutionMon(string partner) { this.partner = partner; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mysterious Evolution") return false;
                bool partnerInPlay = ctx.MyPokemon().Any(m => m.Name == partner) || ctx.OpponentPokemon().Any(m => m.Name == partner);
                if (!partnerInPlay)
                {
                    ctx.Log("Mysterious Evolution: there's no " + partner + " in play.");
                    return true;
                }
                if (ctx.Me.Deck.Count == 0) return true;
                string name = ctx.Pokemon.Name;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == name, 1, "Mysterious Evolution: choose a card that evolves from " + name, false);
                if (found.Count > 0) ctx.Engine.EvolvePokemon(ctx.Pokemon, found[0]);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>V-create: if you have 4 or fewer Benched Pokémon, this attack does nothing. Used for V-create, V-Force (the attack name is passed in).</summary>
        internal sealed class VCreateMon : Mon
        {
            private readonly string attack;

            public VCreateMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Me.Bench.Count <= 4) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Fiery Dance: attach a basic Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class FieryDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fiery Dance") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => true, "Fiery Dance: attach a basic Energy card to 1 of your Pokémon", false);
                return true;
            }
        }

        /// <summary>Double Freeze: flip 2 coins, 40 for each heads; if either is heads, the Defending Pokémon is now Paralyzed.</summary>
        private sealed class DoubleFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Freeze") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Frost Vanish: you may return this Pokémon and all cards attached to it to your hand.</summary>
        private sealed class FrostVanishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frost Vanish") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !ctx.Engine.CanPutIntoHand(me)) return true;
                if (ctx.AskYesNo("Frost Vanish: return " + me.Name + " and all cards attached to it to your hand?")) ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Shock Bolt: flip a coin; if tails, discard all Lightning Energy attached to this Pokémon.</summary>
        private sealed class FlipShockBoltMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shock Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) CardsTEU2.DiscardAllOfType(ctx, ctx.Pokemon, EnergyType.Lightning);
                return true;
            }
        }

        /// <summary>Damagriiigus: move all damage counters from 1 of your Benched Pokémon to the Defending Pokémon.</summary>
        private sealed class DamagriiigusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damagriiigus") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                var damaged = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (defending == null || damaged.Count == 0) return true;
                if (ctx.Engine.CounterMovementBlocked())
                {
                    ctx.Log("Damagriiigus: damage counters can't be moved.");
                    return true;
                }
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Damagriiigus on " + defending.Name + " was prevented.");
                    return true;
                }
                var from = ctx.ChoosePokemon(damaged, "Damagriiigus: move all damage counters from which Benched Pokémon?");
                if (from == null) return true;
                int counters = from.DamageCounters;
                from.DamageCounters = 0;
                defending.DamageCounters += counters;
                ctx.Log("Damagriiigus: " + counters + " damage counters moved from " + from.Name + " to " + defending.Name + ".");
                return true;
            }
        }

        /// <summary>Garbage Collection: put a card from your discard pile on top of your deck.</summary>
        private sealed class GarbageCollectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Garbage Collection") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.ToList();
                if (pile.Count == 0) return true;
                var pick = ctx.ChooseCards(pile, 1, 1, "Garbage Collection: put which card from your discard pile on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return true;
                ctx.Me.Deck.Insert(0, pick);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "put on top of " + ctx.Engine.Their(ctx.Player) + " deck (Garbage Collection)");
                return true;
            }
        }

        /// <summary>Core Cannon: 20 damage for each Fighting Energy on this Pokémon to 1 of the opponent's Pokémon.</summary>
        private sealed class CoreCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Cannon") return false;
                int amount = 20 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fighting);
                if (amount <= 0) return Nothing(ctx);
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>Devour: for each of your Durant in play, discard the top card of the opponent's deck.</summary>
        private sealed class DevourMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Devour") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int durant = ctx.MyPokemon().Count(m => m.Name == "Durant");
                if (durant > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, durant);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Xtransceiver: flip a coin; if heads, search your deck for a Supporter card and put it into your hand.</summary>
        private sealed class Xtransceiver : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                ctx.SearchToHand(d => d.IsSupporter, 1, "Xtransceiver: choose a Supporter card");
            }
        }
    }
}
