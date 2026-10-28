using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Boundaries Crossed (BCR, v112): Grass to Psychic Pokémon (1-77). Charmander, Charmeleon, Charizard, Blastoise, Keldeo-EX,
    /// Dusknoir, Croagunk (65), Toxicroak and Meloetta were already in the pool and are linked as prints; vanilla attacks need no
    /// script.
    /// </summary>
    public static class CardsBCR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("BCR-1", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("BCR-2", new ComboMon(new FoulOdorMon(), new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned)));
            r.RegisterByKey("BCR-3", new VileplumeAllergyPanic());
            r.RegisterByKey("BCR-4", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Grass Knot", 20, false), new CardsCEC3.PetalDanceMon()));
            r.RegisterByKey("BCR-5", new CardsTWM1.HealSelfMon("Nap", 30));
            r.RegisterByKey("BCR-6", new ComboMon(new HundredFuriousLashesMon(), new CardsTWM1.HealSelfMon("Mega Drain", 30)));
            r.RegisterByKey("BCR-8", new GigaHornMon("Giga Horn"));
            r.RegisterByKey("BCR-9", new CelebiTimeRecall());
            r.RegisterByKey("BCR-10", new ComboMon(new CallForFamilyMon(), new FlipHealSelfMon("Leaf Drain", 30)));
            r.RegisterByKey("BCR-12", new CoinsMon("Double Slash", 2));
            r.RegisterByKey("BCR-13", new ComboMon(new CoinsMon("Double Slash", 2), new CardsTWM1.HealSelfMon("Mega Drain", 30)));
            r.RegisterByKey("BCR-15", new ComboMon(new FluffyTagMon(), new ParalyzeFlipMon("Stun Spore")));
            r.RegisterByKey("BCR-16", new CardsPAR2.FlipConditionMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("BCR-17", new ComboMon(new DrawUntilAttackMon("Return", 6), new CardsFFI1.MagicalLeafThirtyMon()));
            r.RegisterByKey("BCR-21", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("BCR-22", new ComboMon(new FireShardMon(), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("BCR-23", new ComboMon(new DrawAttackMon("Collect", 1), new CardsPFL.FlipUntilTailsTimesMon("Relentless Flames")));
            r.RegisterByKey("BCR-25", new FlipBonusMon("Firebreathing", 20));
            r.RegisterByKey("BCR-26", new ComboMon(new FlipBonusMon("Firebreathing", 30), new CardsTWM1.SelfDiscardMon("Fire Blast", 1)));
            r.RegisterByKey("BCR-27", new CardsFST1.FlipRecoilMon("Reckless Charge", 10));
            r.RegisterByKey("BCR-28", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"),
                new CardsFST2.ConditionBonusMon("Damage Counterpunch", 60, c => c.Pokemon.DamageCounters > 0)));
            r.RegisterByKey("BCR-29", new SquirtleShellShield());
            r.RegisterByKey("BCR-30", new CardsMEW1.WithdrawMon("Withdraw"));
            r.RegisterByKey("BCR-32", new CardsPAR2.FlipConditionMon("Dazzle Dance", SpecialCondition.Confused));
            r.RegisterByKey("BCR-33", new FirefightingMon());
            r.RegisterByKey("BCR-34", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("BCR-35", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CardsTWM1.SelfDiscardMon("Aquafall", 99)));
            r.RegisterByKey("BCR-37", new ComboMon(new DeepDiveMon(), new CardsFLF1.NoResistanceMon("Aqua Sonic")));
            r.RegisterByKey("BCR-38", new ComboMon(new PresentMon(), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("BCR-41", new CardsDRI2.CrunchMon("Destructive Whirlpool"));
            r.RegisterByKey("BCR-43", new ComboMon(new FlipBonusMon("Aerial Ace", 20), new DefogMon()));
            r.RegisterByKey("BCR-45", new JellicentStickiness());
            r.RegisterByKey("BCR-46", new FlipBonusMon("Ice Edge", 20));
            r.RegisterByKey("BCR-47", new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 10));
            r.RegisterByKey("BCR-48", new FlipBonusMon("Rising Lunge", 20));
            r.RegisterByKey("BCR-50", new CoinsMon("Double Voltage", 2));
            r.RegisterByKey("BCR-54", new ComboMon(new CardsMEW1.SpreadBenchMon("Electriwave", 30), new CardsFLF1.NoResistanceMon("Shock Wave")));
            r.RegisterByKey("BCR-55", new CardsMEG.SnipeMon("Electripult", 20));
            r.RegisterByKey("BCR-57", new ComboMon(new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1), new CardsFST1.FlipRecoilMon("Thunder", 30)));
            r.RegisterByKey("BCR-59", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("BCR-60", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused), new CardsSTS1.ExtrasensoryMon("Extrasensory", 60)));
            r.RegisterByKey("BCR-61", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("BCR-62", new CardsEVS1.AstonishMon("Astonish"));
            r.RegisterByKey("BCR-64", new ParalyzeFlipMon("Paralyzing Jab"));
            r.RegisterByKey("BCR-67", new CresseliaSparklingParticles());
            r.RegisterByKey("BCR-68", new MunnaHypnosis());
            r.RegisterByKey("BCR-69", new MusharnaDreamWaltz());
            r.RegisterByKey("BCR-70", new Cards30C.PeerAtMon("Scout"));
            r.RegisterByKey("BCR-71", new ComboMon(new JetWooferMon(), new CardsWHT.FlipMoreMon("Acrobatics", 2, 20)));
            r.RegisterByKey("BCR-72", new CardsCRI.CounterConditionMon("Poison Point", SpecialCondition.Poisoned, null));
            r.RegisterByKey("BCR-73", new CardsCRI.CounterConditionMon("Poison Point", SpecialCondition.Poisoned, null));
            r.RegisterByKey("BCR-74", new CardsCRI.CounterConditionMon("Poison Point", SpecialCondition.Poisoned, new CardsBLK.VenoshockMon(40)));
            r.RegisterByKey("BCR-76", new CardsASC.FlipDiscardDefendingEnergyMon("Destructive Beam"));
        }

        // ================================================================== Shared

        /// <summary>"Flip a coin. If heads, heal N damage from this Pokémon" (Leaf Drain).</summary>
        internal sealed class FlipHealSelfMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FlipHealSelfMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, amount);
                return true;
            }
        }

        /// <summary>"Draw cards until you have N cards in your hand" (Return; not optional).</summary>
        internal sealed class DrawUntilAttackMon : Mon
        {
            private readonly string attack;
            private readonly int size;

            public DrawUntilAttackMon(string attack, int size)
            {
                this.attack = attack;
                this.size = size;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.DrawUntil(size);
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, this Pokémon has no Weakness" after the damage (Psychic Protection, Ghost Hammer).</summary>
        internal sealed class NoWeaknessNextTurnMon : Mon
        {
            private readonly string attack;

            public NoWeaknessNextTurnMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new CardsPLF2.NoWeaknessEffect(me.Name + " has no Weakness during your opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Foul Odor: both this Pokémon and the Defending Pokémon are now Confused.</summary>
        private sealed class FoulOdorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foul Odor") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Allergy Panic: Weakness is ×4 for every Pokémon (both players'; no Weakness means nothing to multiply, 2012-11-08). Pollen Spray: Asleep and Poisoned.</summary>
        private sealed class VileplumeAllergyPanic : Mon
        {
            public override int WeaknessMultiplier(EffectContext src, DamageInfo info) { return src.Pokemon != null ? 4 : 2; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pollen Spray") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Hundred Furious Lashes: 30 for each Grass Energy attached to this Pokémon; it can't use this attack next turn.</summary>
        private sealed class HundredFuriousLashesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hundred Furious Lashes") return false;
                ctx.DamageDefending(atk.Damage * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Giga Horn: flip 2 coins; if both are tails, this attack does nothing. Used for Giga Horn, Giga Claw (the attack name is passed in).</summary>
        internal sealed class GigaHornMon : Mon
        {
            private readonly string attack;

            public GigaHornMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.FlipCoins(2) == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Time Recall: each of your evolved Pokémon can use the attacks of its previous Evolutions (Energy still needed). Wind Whisk: switch this Pokémon.</summary>
        private sealed class CelebiTimeRecall : Mon
        {
            private static readonly Mon Recall = new RelicanthTEF();
            private static readonly Mon Whisk = new SwitchSelfMon("Wind Whisk", false);

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon) { return Recall.ExtraAttacksFor(src, mon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Whisk.HandleAttack(ctx, atk); }
        }

        /// <summary>Fluffy Tag: switch this Pokémon with 1 of your Benched Pokémon; during your next turn, the new Active's attacks do 40 more to the Active Pokémon.</summary>
        private sealed class FluffyTagMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fluffy Tag") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || !ctx.IsActive(ctx.Pokemon) || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.SwitchSelfWithBench()) return true;
                var tagged = ctx.Me.Active;
                if (tagged == null || tagged == ctx.Pokemon) return true;
                var boost = new CardsCRI.NextTurnPowerEffect(tagged.Name + "'s attacks do 40 more damage to the Active Pokémon during your next turn (Fluffy Tag)",
                    ctx.Player, tagged.Uid, ctx.Turn + 2, 40);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Fire Shard: the Defending Pokémon is now Burned; flip a coin, heads: also Paralyzed.</summary>
        private sealed class FireShardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Shard") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Shell Shield: while on your Bench, prevent all damage done to this Pokémon by attacks (both players'). Water Splash.</summary>
        private sealed class SquirtleShellShield : Mon
        {
            private static readonly Mon Splash = new FlipBonusMon("Water Splash", 20);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && src.Me.Bench.Contains(me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Splash.HandleAttack(ctx, atk); }
        }

        /// <summary>Firefighting: discard a Fire Energy attached to the Defending Pokémon.</summary>
        private sealed class FirefightingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Firefighting") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.DiscardEnergyFrom(defending, 1, "Firefighting: discard a Fire Energy from " + defending.Name, c => CardsTEU1.ProvidesType(ctx, defending, c, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Deep Dive: flip 2 coins; heal 40 damage from this Pokémon for each heads.</summary>
        private sealed class DeepDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deep Dive") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 40 * heads);
                return true;
            }
        }

        /// <summary>Present: flip a coin; heads, search your deck for a card (you must take one) and put it into your hand.</summary>
        private sealed class PresentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Present") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0 && ctx.Flip()) ctx.SearchToHand(d => true, 1, "Present: put a card from your deck into your hand", false, 1);
                return true;
            }
        }

        /// <summary>Defog: you may discard the Stadium in play; if you do, 40 more damage.</summary>
        private sealed class DefogMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Defog") return false;
                bool more = false;
                if (ctx.State.Stadium != null && ctx.AskYesNo("Defog: discard " + ctx.State.Stadium.Def.Name + " for 40 more damage?"))
                {
                    ctx.Engine.DiscardStadium();
                    more = ctx.State.Stadium == null;
                }
                ctx.DamageDefending(atk.Damage + (more ? 40 : 0));
                return true;
            }
        }

        /// <summary>Stickiness: the opponent's Pokémon in play have a Retreat Cost [C] more (not one an effect set to 0, 2012-11-08). Eerie Light.</summary>
        private sealed class JellicentStickiness : Mon
        {
            private static readonly Mon Light = new CardsPAR2.FlipConditionMon("Eerie Light", SpecialCondition.Confused);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player ? cost + 1 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Light.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        // ================================================================== Psychic

        /// <summary>Sparkling Particles: heal 10 damage from this Pokémon between turns. Psychic Protection: no Weakness next turn.</summary>
        private sealed class CresseliaSparklingParticles : Mon
        {
            private static readonly Mon Protection = new NoWeaknessNextTurnMon("Psychic Protection");

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.InPlay(me) || me.DamageCounters == 0) return;
                Announce(src, "Sparkling Particles");
                src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Protection.HandleAttack(ctx, atk); }
        }

        /// <summary>Long-Distance Hypnosis: once during your turn, flip a coin; heads, the opponent's Active is Asleep; tails, your Active is.</summary>
        private sealed class MunnaHypnosis : Mon
        {
            protected override string Activated { get { return "Long-Distance Hypnosis"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Asleep);
                }
                else if (ctx.Me.Active != null) ctx.ApplyCondition(ctx.Me.Active, SpecialCondition.Asleep);
            }
        }

        /// <summary>Telekinesis: 30 to 1 of the opponent's Pokémon, no Weakness or Resistance. Dream Waltz: usable while Asleep; 30 more if Asleep.</summary>
        private sealed class MusharnaDreamWaltz : Mon
        {
            public override bool UsableWhileAsleep(AttackDef attack) { return attack != null && attack.Name == "Dream Waltz"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Telekinesis")
                {
                    ctx.DamageAnyOpponent(30, true);
                    return true;
                }
                if (atk.Name == "Dream Waltz")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.Has(SpecialCondition.Asleep) ? 30 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Jet Woofer: discard the top card of the opponent's deck for each Psychic Energy attached to this Pokémon.</summary>
        private sealed class JetWooferMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Woofer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Psychic);
                if (n > 0 && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }
    }
}
