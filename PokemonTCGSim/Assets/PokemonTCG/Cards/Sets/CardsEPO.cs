using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Emerging Powers (EPO 1-98, 2011; Expanded). Cards reprinted in Legendary Treasures (Thundurus, Gothita 43, Gothorita 45,
    /// Gothitelle 47, Tornadus, Bianca), White Flare (Cheren), Perfect Order (Crushing Hammer, Pokémon Catcher), Paldea Evolved
    /// (Great Ball) and Guardians Rising (Max Potion) are prints of those; 97-98 are the full arts. Your rulings (v131): Recycle is
    /// its own Expanded card (not Fossil's); Telekinesis and Feint Attack do their damage only to the Pokémon you choose.
    /// Pokémon whose attacks only do their printed damage need no script: Sewaddle 4, Darumaka, Cubchoo 29, Joltik, Gothita 44,
    /// Drilbur 55, Sandile, Ferroseed 71, Pidove, Rufflet 86 and the rest of the plain attacks.
    /// </summary>
    public static class CardsEPO
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("EPO-1", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("EPO-2", new EnergyPowerMon("Fire's Power", EnergyType.Fire));
            r.RegisterByKey("EPO-3", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("EPO-5", new CoinsMon("Double Razor Leaf", 2));
            r.RegisterByKey("EPO-6", new CardsTWM1.HealSelfMon("Grass Cocooning", 40));
            r.RegisterByKey("EPO-7", new ComboMon(new NurturingMon(), new FlipBonusMon("X-Scissor", 50)));
            r.RegisterByKey("EPO-8", new CoinsMon("Triple Cutter", 3));
            r.RegisterByKey("EPO-9", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("EPO-10", new CardsASC.SelfGuardMon("Cotton Guard", 10));
            r.RegisterByKey("EPO-11", new ComboMon(new CardsSHF2.HelpingHandMon(), new CardsASC.SelfGuardMon("Cotton Guard", 30)));
            r.RegisterByKey("EPO-12", new ComboMon(new EncoreMon(), new SwitchSelfMon("U-turn", false)));
            r.RegisterByKey("EPO-13", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("EPO-14", new BemusingAromaMon());
            r.RegisterByKey("EPO-15", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("EPO-16", new ComboMon(new CardsDRI1.WhirlwindMon("Push Down"), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("EPO-17", new ComboMon(new CardsDAA1.DrainMon("Giga Drain"), new LockAfterAttackMon("Sacred Sword", true)));
            r.RegisterByKey("EPO-18", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("EPO-19", new ComboMon(new EnergyPowerMon("Water's Power", EnergyType.Water), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("EPO-21", new ComboMon(new FlipBonusMon("Rock Smash", 20), new ConditionAttackMon("Fire Punch", SpecialCondition.Burned)));
            r.RegisterByKey("EPO-22", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("EPO-23", new ComboMon(new EnergyPowerMon("Grass' Power", EnergyType.Grass), new RushingWaterMon()));
            r.RegisterByKey("EPO-24", new ComboMon(new CardsASR1.TimesMon("Flail", c => c.Pokemon == null ? 0 : c.Pokemon.DamageCounters), new FinalGambitMon("Final Gambit", 80)));
            r.RegisterByKey("EPO-25", new CardsMEG.SnipeMon("Splatter", 30));
            r.RegisterByKey("EPO-26", new RoostMon());
            r.RegisterByKey("EPO-27", new ComboMon(new DodgeMon("Wing Dance", false), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("EPO-28", new ComboMon(new CardsPAR2.FlipConditionMon("Powder Snow", SpecialCondition.Asleep), new RestMon()));
            r.RegisterByKey("EPO-30", new ComboMon(new CardsEVS1.DefenderCantAttackMon("Sheer Cold", d => true), new CardsFLF1.NoResistanceMon("Icicle Crash")));
            r.RegisterByKey("EPO-31", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep), new CardsSUM5.OptionalBoostRecoilMon("Superpower", 20, 20)));
            r.RegisterByKey("EPO-32", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CardsWHT.FlipMoreMon("Acrobatics", 2, 20)));
            r.RegisterByKey("EPO-34", new ParalyzeFlipMon("Stun Needle"));
            r.RegisterByKey("EPO-36", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("EPO-37", new PhatSoundMon());
            r.RegisterByKey("EPO-38", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("EPO-39", new ComboMon(new CardsRCL1.ConditionBonusMon("Venoshock", SpecialCondition.Poisoned, 60), new CardsFLF1.NoResistanceMon("Steamroller")));
            r.RegisterByKey("EPO-40", new ComboMon(new CardsCEC1.ToxicCECMon("Toxic Claws"), new CardsWHT.FlipMoreMon("Wild Horn", 2, 20)));
            r.RegisterByKey("EPO-41", new ComboMon(new CardsASC.SelfGuardMon("Reflect", 40), new TelekinesisMon(50)));
            r.RegisterByKey("EPO-42", new ComboMon(new CoinsMon("Quick Turn", 2),
                new CardsASR1.PlusMon("Psychic Assault", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters)));
            r.RegisterByKey("EPO-46", new DeletingGlareMon());
            r.RegisterByKey("EPO-48", new MentalShockMon());
            r.RegisterByKey("EPO-49", new CardsLOR1.HardenMon("Harden", 40));
            r.RegisterByKey("EPO-50", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("EPO-51", new CardsFST2.ConditionBonusMon("Smack Down", 60, c => c.Defending != null && c.Defending.Def.Resistance == EnergyType.Fighting));
            r.RegisterByKey("EPO-52", new ThrashMon("Hard Crash"));
            r.RegisterByKey("EPO-53", new ComboMon(new ShearMon(), new CardsMEG.EnergyBonusMon("Rock Bullet", EnergyType.Fighting, 20)));
            r.RegisterByKey("EPO-54", new PowerUpMon("Hone Claws", 30));
            r.RegisterByKey("EPO-56", new CardsDRI2.CrunchMon("Drill Run"));
            r.RegisterByKey("EPO-57", new ComboMon(new DodgeMon("Dig", false), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("EPO-58", new ScarfHoldMon());
            r.RegisterByKey("EPO-59", new FiveFierceChopsMon());
            r.RegisterByKey("EPO-61", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("EPO-62", new KrookodileBlackEyes());
            r.RegisterByKey("EPO-63", new LockAfterAttackMon("Sacred Sword", true));
            r.RegisterByKey("EPO-64", new FlipKnockOffMon());
            r.RegisterByKey("EPO-65", new ComboMon(new CardsCES1.FeintAttackMon("Feint Attack", 30),
                new CardsFST2.ConditionBonusMon("Claw Rend", 30, c => c.Defending != null && c.Defending.DamageCounters > 0)));
            r.RegisterByKey("EPO-66", new FlipBonusMon("Rising Lunge", 10));
            r.RegisterByKey("EPO-67", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("EPO-68", new CardsASC.SelfGuardMon("Rear Guard", 30));
            r.RegisterByKey("EPO-69", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Bone Rush"),
                new CardsASR1.PlusMon("Dark Pulse", 10, c => c.MyPokemon().Sum(m => c.Engine.EnergyCountOfType(m, EnergyType.Darkness)))));
            r.RegisterByKey("EPO-70", new CoinsMon("Pin Missile", 4));
            r.RegisterByKey("EPO-72", new ComboMon(new CoinsMon("Steel Feelers", 3), new GyroBallMon()));
            r.RegisterByKey("EPO-73", new ComboMon(new DodgeMon("Iron Defense", false), new ThunderburstStormMon("Power Whip", 10)));
            r.RegisterByKey("EPO-74", new CoinsMon("Gear Grind", 2));
            r.RegisterByKey("EPO-75", new ComboMon(new ConditionAttackMon("Metal Sound", SpecialCondition.Confused), new CardsASC.SelfGuardMon("Guard Press", 20)));
            r.RegisterByKey("EPO-76", new ComboMon(new CardsGRI2.AttachDiscardEnergyToSelfMon("Charge Beam"), new ZapCannonMon()));
            r.RegisterByKey("EPO-77", new ComboMon(new CardsDRI1.WhirlwindMon("Metal Horns"), new LockAfterAttackMon("Sacred Sword", true)));
            r.RegisterByKey("EPO-78", new TailsNothingMon("Hyper Fang"));
            r.RegisterByKey("EPO-79", new WatchogMon());
            r.RegisterByKey("EPO-81", new TailsNothingMon("Claw"));
            r.RegisterByKey("EPO-82", new ComboMon(new CardsCEC1.TailwindMon(), new FeatherStrikeMon()));
            r.RegisterByKey("EPO-83", new ComboMon(new CardsASC.FlipPerEnergyMon("Powerful Slap"), new CardsPOR.SweetScentMon("Heal Pulse", 50)));
            r.RegisterByKey("EPO-84", new TailsNothingMon("Last Resort"));
            r.RegisterByKey("EPO-85", new ComboMon(new CardsDAA1.GustAttackMon("Captivate", false), new ConditionAttackMon("Fluffy Tail", SpecialCondition.Asleep)));
            r.RegisterByKey("EPO-87", new TailsNothingMon("Claw"));
            r.RegisterByKey("EPO-88", new RecoilMon("Brave Bird", 30));
            r.RegisterByKey("EPO-96", new Recycle());
        }

        /// <summary>Fire's / Water's / Grass' Power: if this Pokémon has any [type] Energy attached, Burned / Asleep / heal 20 from itself.</summary>
        private sealed class EnergyPowerMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public EnergyPowerMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool powered = ctx.Engine.EnergyCountOfType(me, type) > 0;
                ctx.DamageDefending(atk.Damage);
                if (!powered) return true;
                if (type == EnergyType.Grass)
                {
                    if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20);
                    return true;
                }
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, type == EnergyType.Fire ? SpecialCondition.Burned : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Nurturing: choose 1 of your Pokémon and evolve it with a card from your deck; shuffle your deck afterward.</summary>
        private sealed class NurturingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nurturing") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var targets = ctx.MyPokemon(m => ctx.Me.Deck.Any(c => c.Def.IsPokemon && c.Def.EvolvesFrom == m.Name));
                if (targets.Count > 0)
                {
                    var target = ctx.ChoosePokemon(targets, "Nurturing: evolve which of your Pokémon?");
                    if (target != null) CardsSSP1.EvolveFromDeck(ctx, target, "Nurturing: choose a card that evolves from " + target.Name);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Encore: choose 1 of the Defending Pokémon's attacks; during the opponent's next turn it can use only that attack.</summary>
        private sealed class EncoreMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Encore") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                int index = ChooseAttackOf(ctx, defending.Def, "Encore: choose the only attack " + defending.Name + " can use during its next turn");
                if (index < 0) return true;
                string name = defending.Def.Attacks[index].Name;
                ctx.AddEffect(new OnlyAttackEffect(defending.Name + " can use only " + name + " during its next turn (Encore)", ctx.Player, defending.Uid, ctx.Turn + 1, name));
                return true;
            }
        }

        /// <summary>The Pokémon can use only the named attack.</summary>
        private sealed class OnlyAttackEffect : ContinuousEffect
        {
            private readonly string attackName;

            public OnlyAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn, string attackName)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.attackName = attackName;
            }

            public override bool PreventsAttackNamed(GameEngine engine, PokemonInPlay mon, string name)
            {
                return mon.Uid == TargetUid && name != attackName;
            }
        }

        /// <summary>Bemusing Aroma: flip; heads Paralyzed and Poisoned, tails Confused.</summary>
        internal sealed class BemusingAromaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bemusing Aroma") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads)
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                else ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Rushing Water: move an Energy from the Defending Pokémon to 1 of the opponent's Benched Pokémon (you choose).</summary>
        internal sealed class RushingWaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rushing Water") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var energy = defending.Energy.Count == 1 ? defending.Energy[0]
                    : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Rushing Water: move which Energy from " + defending.Name + "?")[0];
                var bench = ctx.OpponentBench();
                var dest = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Rushing Water: move it to which of your opponent's Benched Pokémon?");
                if (dest != null) ctx.Engine.MoveEnergy(defending, dest, energy);
                return true;
            }
        }

        /// <summary>Final Gambit: flip 2 coins; if both are tails, this Pokémon does 80 damage to itself. Used for Final Gambit, Reckless Abandon (the attack name and the numbers are passed in).</summary>
        internal sealed class FinalGambitMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public FinalGambitMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) == 0) ctx.DamageSelf(amount);
                return true;
            }
        }

        /// <summary>Roost: heal 40 from this Pokémon; it can't retreat during your next turn.</summary>
        private sealed class RoostMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Roost") return false;
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 40);
                var effect = new CantRetreatEffect(me.Name + " can't retreat during its next turn (Roost)", ctx.Player, me.Uid, ctx.Turn + 2);
                effect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>Rest: heal 60 from this Pokémon, then it is Asleep.</summary>
        private sealed class RestMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 60);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Phat Sound: flip 3 coins; 10 damage per heads to each of the opponent's Pokémon.</summary>
        private sealed class PhatSoundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Phat Sound") return false;
                int dmg = 10 * ctx.FlipCoins(3);
                if (dmg == 0) return Nothing(ctx);
                ctx.DamageDefending(dmg);
                foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, dmg);
                return true;
            }
        }

        /// <summary>Telekinesis: 50 to 1 of the opponent's Pokémon, no Weakness or Resistance (your ruling: only that Pokémon is hit). The numbers are passed in.</summary>
        internal sealed class TelekinesisMon : Mon
        {
            private readonly int amount;

            public TelekinesisMon(int amount)
            {
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Telekinesis") return false;
                ctx.DamageAnyOpponent(amount, true);
                return true;
            }
        }

        /// <summary>Deleting Glare: flip; heads, discard an Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class DeletingGlareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deleting Glare") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var targets = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Deleting Glare: discard an Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Deleting Glare: discard an Energy from " + target.Name);
                return true;
            }
        }

        /// <summary>Mental Shock: flip; heads Confused, tails discard an Energy from the Defending Pokémon.</summary>
        private sealed class MentalShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mental Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool heads = ctx.Flip();
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (heads) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                else if (defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Mental Shock: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Hard Crash / Thrash: flip; heads 20 more damage, tails this Pokémon does 20 damage to itself.</summary>
        internal sealed class ThrashMon : Mon
        {
            private readonly string attack;

            public ThrashMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (!heads) ctx.DamageSelf(20);
                return true;
            }
        }

        /// <summary>Shear: discard the top 5 cards of your deck and attach the Fighting Energy cards among them to this Pokémon.</summary>
        private sealed class ShearMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shear") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var me = ctx.Pokemon;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                if (!ctx.InPlay(me)) return true;
                foreach (var card in milled.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting)).ToList())
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Hone Claws / Feather Dance / Bulk Up: during your next turn, this Pokémon's attacks do N more damage (before Weakness and Resistance).</summary>
        internal sealed class PowerUpMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public PowerUpMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                var boost = new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do " + bonus + " more damage during its next turn (" + attack + ")",
                    ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, bonus);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Scarf Hold: flip; heads, the Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class ScarfHoldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scarf Hold") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Scarf Hold)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Five Fierce Chops: flip 5 coins, 20 per heads; this Pokémon can't attack during your next turn.</summary>
        private sealed class FiveFierceChopsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Five Fierce Chops") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(5));
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Black Eyes: once during your turn (before your attack), if this Pokémon is your Active Pokémon, you may flip a coin; heads,
        /// discard an Energy from the opponent's Active Pokémon. Thrash: heads 20 more, tails 20 to itself.
        /// </summary>
        private sealed class KrookodileBlackEyes : Mon
        {
            private static readonly Mon Thrash = new ThrashMon("Thrash");

            protected override string Activated { get { return "Black Eyes"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.IsActive(ctx.Pokemon) && ctx.Opp.Active != null && ctx.Opp.Active.Energy.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!ctx.Flip() || target == null || target.Energy.Count == 0) return;
                ctx.DiscardEnergyFrom(target, 1, "Black Eyes: discard an Energy from " + target.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Thrash.HandleAttack(ctx, atk); }
        }

        /// <summary>Knock Off: flip; heads, discard a random card from the opponent's hand.</summary>
        private sealed class FlipKnockOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Off") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Opp.Hand.Count > 0) TefPokemon.DiscardRandomFromOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Gyro Ball: switch this Pokémon with 1 of your Benched Pokémon; then the opponent switches their Active Pokémon.</summary>
        private sealed class GyroBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gyro Ball") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Gyro Ball: switch " + ctx.Pokemon.Name + " with which Benched Pokémon?");
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Zap Cannon: flip; tails, this Pokémon can't use Zap Cannon during your next turn.</summary>
        private sealed class ZapCannonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zap Cannon") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Watcheck: look at the top 5 cards of the opponent's deck and put them back in any order. Quick Tail Smash: before doing
        /// damage you may flip a coin; heads 60 more damage, tails the attack does nothing.
        /// </summary>
        private sealed class WatchogMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Watcheck")
                {
                    if (ctx.Opp.Deck.Count > 0) CardsDRI2.ReorderOpponentTop(ctx, 5, "Watcheck");
                    return true;
                }
                if (atk.Name == "Quick Tail Smash")
                {
                    if (!ctx.AskYesNo("Quick Tail Smash: flip a coin (heads: 60 more damage, tails: the attack does nothing)?"))
                    {
                        ctx.DamageDefending(atk.Damage);
                        return true;
                    }
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage + 60);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Feather Strike: flip; heads 40 more damage, tails discard an Energy from the Defending Pokémon.</summary>
        private sealed class FeatherStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Feather Strike") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 40 : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Feather Strike: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Recycle (EPO): flip a coin; heads, put a card from your discard pile on top of your deck (your opponent sees which).</summary>
        private sealed class Recycle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Discard.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var discard = ctx.Me.Discard.Where(c => c != ctx.Card).ToList();
                if (discard.Count == 0) return;
                var pick = discard.Count == 1 ? discard[0] : ctx.ChooseCards(discard, 1, 1, "Recycle: put which card from your discard pile on top of your deck?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                ctx.Me.Deck.Insert(0, pick);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "card put on top of the deck (Recycle)");
            }
        }
    }
}
