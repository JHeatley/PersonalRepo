using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Sword &amp; Shield (SSH, 2020), Pokémon 1-91: Grass, Fire, Water, Lightning and Psychic. Fighting through Colorless, the
    /// Trainers and Aurora Energy are in CardsSSH2. Regulation mark D: Expanded only.
    /// Already in the pool (most reprinted in Shining Fates): Dhelmise V, Grookey (10), Rillaboom (14), Dottler, Orbeetle,
    /// Gossifleur, Eldegoss, Scorbunny (30), Raboot (32), Cinderace (34), Sizzlipede (37), Centiskorch, Lapras V / VMAX, Sobble
    /// (54), Drizzile (56), Inteleon (58), Chewtle, Drednaw, Frosmoth, Yamper (73), Boltund (75), Pincurchin, Morpeko V / VMAX,
    /// Galarian Ponyta, Galarian Rapidash and Indeedee V.
    /// Pokémon whose attacks only do their printed damage need no script (Cottonee, Grookey 11, Blipbug 16, Vulpix, Scorbunny
    /// 31, Raboot 33, Goldeen 45, Sobble 55, Drizzile 57, Snom, Raichu, Chinchou 67, Joltik, Yamper 74).
    /// Rule used here (your ruling, v65): "a [T] Energy card" outside play is basic Energy only (CardsEVS1.IsEnergyOf).
    /// </summary>
    public static class CardsSSH1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("SSH-1", new ComboMon(new SearchAttackMon("Find a Friend", IsPokemonCard, 2), new CardsASR1.PlusMon("Line Force", 20, MyBenchCount)));
            r.RegisterByKey("SSH-2", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("SSH-3", new CardsPOR.SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("SSH-4", new ComboMon(new ParalyzePoisonMon(), new CardsTWM1.HealSelfMon("Mega Drain", 30)));
            r.RegisterByKey("SSH-6", new CottonRideMon());
            r.RegisterByKey("SSH-7", new CardsASC.FlipPerEnergyMon("Powerful Needles"));
            r.RegisterByKey("SSH-8", new CardsASR1.PlusMon("Bite Together", 60, DurantOnBench));
            r.RegisterByKey("SSH-12", new ComboMon(new CardsDAA1.GustAttackMon("Taunt", false), new CoinsMon("Double Hit", 2)));
            r.RegisterByKey("SSH-13", new FlipBonusMon("Ambush", 30));
            r.RegisterByKey("SSH-15", new ComboMon(new CardsMEW1.SpreadBenchMon("Drum Roll", 10), new LockAfterAttackMon("Drum Beating", true)));
            r.RegisterByKey("SSH-17", new BenchSearchAttackMon("Call for Family", 1));
            // Fire
            r.RegisterByKey("SSH-23", new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire));
            r.RegisterByKey("SSH-24", new ComboMon(new CombustionPillarMon(), new DiscardDefendingEnergyMon("Steam Crush", 2)));
            r.RegisterByKey("SSH-25", new ComboMon(new CardsBST1.SpreadingFlamesMon(), new CardsCRZ1.BothActiveEnergyMon("Energy Burst", 30)));
            r.RegisterByKey("SSH-26", new ConditionAttackMon("Searing Flame", SpecialCondition.Burned));
            r.RegisterByKey("SSH-27", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("SSH-28", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new CardsTWM1.SelfDiscardMon("Bright Flame", 2)));
            r.RegisterByKey("SSH-29", new CardsTWM1.SelfDiscardMon("Fire Spin", 2));
            r.RegisterByKey("SSH-35", new ComboMon(new ConditionAttackMon("Pyro Ball", SpecialCondition.Burned), new CardsTWM1.SelfDiscardMon("Burning Kick", 99)));
            r.RegisterByKey("SSH-36", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire), new TailsSelfDiscardMon("Bright Flame", 2)));
            r.RegisterByKey("SSH-38", new CardsPFL.FlipUntilTailsTimesMon("Relentless Flames"));
            // Water
            r.RegisterByKey("SSH-40", new FlipBonusMon("Water Splash", 20));
            r.RegisterByKey("SSH-41", new ParalyzeFlipMon("Shell Grab"));
            r.RegisterByKey("SSH-42", new CardsCEC3.SpreadAmountMon("Aqua Shower", 10));
            r.RegisterByKey("SSH-43", new CardsPLB2.BigSwingMon("Super Slice"));
            r.RegisterByKey("SSH-44", new ComboMon(new CardsSSP2.StompOffMon("Heavy Pincers", 1), new CardsPFL.DamagedBonusMon("Claw Rend", 60)));
            r.RegisterByKey("SSH-46", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SSH-47", new RippingHornMon());
            r.RegisterByKey("SSH-48", new CardsVIV1.AquaWashMon(2));
            r.RegisterByKey("SSH-51", new QwilfishSSH());
            r.RegisterByKey("SSH-52", new SearchAttackMon("Water Reserve", IsBasicWater, 3));
            r.RegisterByKey("SSH-53", new CardsMEG.EnergyBonusMon("Secret Sword", EnergyType.Water, 30));
            r.RegisterByKey("SSH-59", new ComboMon(new CardsASC.RandomDiscardMon("Silent Shot"), new CardsRCL1.MayReturnDefendingEnergyMon("Hydro Snipe")));
            r.RegisterByKey("SSH-62", new CardsMEG.SnipeMon("Water Arrow", 20));
            // Lightning
            r.RegisterByKey("SSH-65", new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1));
            r.RegisterByKey("SSH-68", new CardsDAA1.GustAttackMon("Luring Glow", false));
            r.RegisterByKey("SSH-69", new StrobeShockMon());
            r.RegisterByKey("SSH-71", new ParalyzeFlipMon("Volt Wave"));
            r.RegisterByKey("SSH-72", new ComboMon(new DrawAttackMon("Spike Draw", 2), new LockAfterAttackMon("Thunderous Bolt", false)));
            r.RegisterByKey("SSH-76", new LockAfterAttackMon("Electrodash", false));
            r.RegisterByKey("SSH-78", new CardsPFL.DamagedBonusMon("Attack the Wound", 50));
            // Psychic
            r.RegisterByKey("SSH-83", new CardsPAF1.ReturnSelfMon("Fade Out"));
            r.RegisterByKey("SSH-84", new ConditionAttackMon("Nightmare", SpecialCondition.Asleep));
            r.RegisterByKey("SSH-85", new GengarSSH());
            r.RegisterByKey("SSH-86", new ComboMon(new GrittyComebackMon(), new NoRetreatMon("Shadow Bind")));
            r.RegisterByKey("SSH-87", new CardsTWM1.HealSelfMon("Moonlight", 30));
            r.RegisterByKey("SSH-88", new ComboMon(new SleepyPulseMon(), new CardsRCL1.ConditionBonusMon("Super Hypnoblast", SpecialCondition.Asleep, 120)));
            r.RegisterByKey("SSH-89", new TeatimeMon("Teatime", 2, 2));
            r.RegisterByKey("SSH-90", new ComboMon(new TeatimeMon("Teatime", 2, 2), new PoltergeistMon()));
        }

        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static bool IsBasicWater(CardDef d) { return CardsEVS1.IsEnergyOf(d, EnergyType.Water); }
        private static int MyBenchCount(EffectContext ctx) { return ctx.Me.Bench.Count; }
        private static int DurantOnBench(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.Name == "Durant") ? 1 : 0; }

        // ================================================================== Shared (also used by CardsSSH2)

        /// <summary>"Discard N Energy from your opponent's Active Pokémon" after the damage (Steam Crush).</summary>
        internal sealed class DiscardDefendingEnergyMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public DiscardDefendingEnergyMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, count, attack + ": discard " + count + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Teatime: each player draws 2 cards (you first). Used for Teatime, Happy Mime, Dual Draw (the attack name and the numbers are passed in).</summary>
        internal sealed class TeatimeMon : Mon
        {
            private readonly string attack;
            private readonly int cards;
            private readonly int amount;

            public TeatimeMon(string attack, int cards, int amount)
            {
                this.attack = attack;
                this.cards = cards;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(cards);
                ctx.Engine.DrawCards(ctx.Opponent, amount);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Paralyze Poison: the opponent's Active Pokémon is now Poisoned; flip a coin, and if heads it's also Paralyzed.</summary>
        private sealed class ParalyzePoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Paralyze Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Cotton Ride: flip a coin; if heads, the opponent shuffles their Active Pokémon and all attached cards into their deck.</summary>
        private sealed class CottonRideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cotton Ride") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (!ctx.Flip()) return true;
                if (defending != null && ctx.InPlay(defending) && ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Combustion Pillar: discard the top card of your deck; 90 more damage if it's a basic Fire Energy card.</summary>
        private sealed class CombustionPillarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Combustion Pillar") return false;
                bool fire = false;
                if (ctx.Me.Deck.Count > 0)
                    fire = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1).Any(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage + (fire ? 90 : 0));
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, discard N Energy from this Pokémon" after the damage (Bright Flame).</summary>
        private sealed class TailsSelfDiscardMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public TailsSelfDiscardMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, count, attack + ": discard " + count + " Energy from " + me.Name);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Ripping Horn: flip 3 coins; for each heads, discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class RippingHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ripping Horn") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, heads, "Ripping Horn: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Poison Point: if this Pokémon is Active and damaged by an opponent's attack (even if Knocked Out), the Attacking Pokémon is
        /// now Poisoned. Venoshock: 60 more damage if the opponent's Active Pokémon is Poisoned.
        /// </summary>
        private sealed class QwilfishSSH : Mon
        {
            private static readonly PoisonPointMon Point = new PoisonPointMon(null);
            private static readonly Mon Venoshock = new CardsRCL1.ConditionBonusMon("Venoshock", SpecialCondition.Poisoned, 60);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                Point.OnHolderDamagedByAttack(ctx, attacker, holderWasActive, damage);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Venoshock.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Strobe Shock: the opponent reveals their hand; if there's any Energy card there, their Active Pokémon is now Paralyzed.</summary>
        private sealed class StrobeShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strobe Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                if (ctx.Opp.Hand.Any(c => c.Def.IsEnergy) && TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Life Shaker: as often as you like during your turn, move 1 damage counter from 1 of your Psychic Pokémon to another of
        /// your Psychic Pokémon. Hypnoblast: the opponent's Active Pokémon is now Asleep.
        /// </summary>
        private sealed class GengarSSH : Mon
        {
            private const string Shaker = "Life Shaker";
            private static readonly Mon Hypnoblast = new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep);

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != Shaker || !ctx.IsMyTurn) return false;
                return CanShake(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing moves as many as they like, then presses Done; bots move one per use.
                RepeatableAbility.Repeat(ctx,
                    () => CanShake(ctx),
                    offerDone =>
                    {
                        var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => IsPsychic(ctx, m) && m.DamageCounters > 0),
                            "Life Shaker: move a damage counter from which of your Psychic Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                        if (giver == null) return false;
                        var taker = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(p => p != giver && IsPsychic(ctx, p)), "Life Shaker: move it to which of your Psychic Pokémon?", offerDone);
                        if (taker == null) return false;
                        ctx.Engine.MoveDamageCounters(giver, taker, 1, Shaker);
                        return true;
                    });
            }

            private static bool CanShake(EffectContext ctx)
            {
                if (ctx.Engine.CounterMovementBlocked()) return false;
                var psychic = ctx.MyPokemon(m => IsPsychic(ctx, m));
                return psychic.Count >= 2 && psychic.Any(m => m.DamageCounters > 0);
            }

            private static bool IsPsychic(EffectContext ctx, PokemonInPlay mon) { return ctx.Engine.IsType(mon, EnergyType.Psychic); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hypnoblast.HandleAttack(ctx, atk); }
        }

        /// <summary>Gritty Comeback: switch all damage counters on this Pokémon with those on the opponent's Active Pokémon.</summary>
        private sealed class GrittyComebackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gritty Comeback") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (defending != null && ctx.InPlay(me)) ctx.Engine.SwapDamageCounters(me, defending, "Gritty Comeback");
                return true;
            }
        }

        /// <summary>
        /// Sleepy Pulse: the opponent's Active Pokémon is now Asleep, and in Pokémon Checkup its owner flips 2 coins instead of 1:
        /// it wakes up only if both are heads (as Altaria's Soothing Lullaby: for as long as that Asleep lasts).
        /// </summary>
        private sealed class SleepyPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleepy Pulse") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Asleep))
                    defending.SleepCoins = 2;
                return true;
            }
        }

        /// <summary>Poltergeist: the opponent reveals their hand; 50 damage for each Trainer card there.</summary>
        private sealed class PoltergeistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poltergeist") return false;
                TefPokemon.RevealOpponentHand(ctx);
                int trainers = ctx.Opp.Hand.Count(c => c.Def.IsTrainer);
                ctx.DamageDefending(atk.Damage * trainers);
                return true;
            }
        }
    }
}
