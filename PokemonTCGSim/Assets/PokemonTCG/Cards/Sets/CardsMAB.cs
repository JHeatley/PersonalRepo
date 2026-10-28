using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Team Magma vs Team Aqua Uncommon and Common Pokémon (MA 24-68; v160). Unlimited only. Meowth 42 and Pikachu 43 are Nintendo
    /// Black Star Promos 13 and 12 (prints).
    /// </summary>
    public static class CardsMAB
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MA-24", new ComboMon(new ParalyzeFlipMon("Paralyzing Gaze"), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("MA-25", new DarkLiftMon(new CardsMA.DelayedConditionMon("Slow-Acting Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("MA-26", new ComboMon(new CoinsMon("Double Pinchers", 2), new DarkPoisonMon()));
            r.RegisterByKey("MA-27", new ComboMon(new CardsSSB.AnyOpponentTextMon("Random Spark", 10), new CardsE1.PlasmaMon()));
            r.RegisterByKey("MA-28", new LanturnAuxiliaryLight());
            r.RegisterByKey("MA-29", new ComboMon(new CardsMA.TeamConditionMon("Aqua Beam", CardsMA.Magma, SpecialCondition.Confused), new ChaosCrushMon()));
            r.RegisterByKey("MA-30", new SearchAttackMon("Aqua Call", d => d.IsPokemon && CardsMA.Aqua(d.Name), 3));
            r.RegisterByKey("MA-31", new KnockOverMon());
            r.RegisterByKey("MA-32", new ComboMon(new PsymistMon(), new PainAmplifierMon()));
            r.RegisterByKey("MA-33", new ComboMon(new ClayCurseMon(), new ClayPulseMon()));
            r.RegisterByKey("MA-34", new ComboMon(new CardsMA.TeamConditionMon("Target Scorch", CardsMA.Aqua, SpecialCondition.Burned),
                CardsSS.ConditionBonus("Damage Burn", 20, d => d.DamageCounters > 0)));
            r.RegisterByKey("MA-35", new ComboMon(new CardsPAR2.FlipConditionMon("Singe", SpecialCondition.Burned), new ParalyzeFlipMon("Dark Shock")));
            r.RegisterByKey("MA-36", new CardsMA.DamagedConditionMon("Land Sonic", SpecialCondition.Confused));
            r.RegisterByKey("MA-37", new MightyenaCallForHelp());
            r.RegisterByKey("MA-38", new ComboMon(new CardsRS.TextDamageMon("Double Damage", 10), new TailsNothingMon("Horn Rush")));
            r.RegisterByKey("MA-39", new AttachToSelfFromHandMon("Growth", d => CardsRS.BasicOf(d, EnergyType.Grass)));
            r.RegisterByKey("MA-40", new TailsNothingMon("Bone Attack"));
            r.RegisterByKey("MA-41", new ComboMon(new CoinsMon("Doubleslap", 2), new RestThreeMon()));
            r.RegisterByKey("MA-44", new ComboMon(new ParalyzeFlipMon("Bubble"), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("MA-45", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new FlipBonusMon("Tail Strike", 10)));
            r.RegisterByKey("MA-46", new ShellRetreatMon());
            r.RegisterByKey("MA-47", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new ConditionAttackMon("Poison Fang", SpecialCondition.Poisoned)));
            r.RegisterByKey("MA-48", new PlainMon());
            r.RegisterByKey("MA-49", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("MA-50", new ConditionAttackMon("Toxic Grip", SpecialCondition.Poisoned));
            r.RegisterByKey("MA-51", new CardsMA.DelayedConditionMon("Slow-Acting Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("MA-52", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("MA-53", new AttachToSelfFromHandMon("Self Charge", d => d.IsEnergy));
            r.RegisterByKey("MA-54", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("MA-55", new ComboMon(new CardsE3B.OpponentSwitchesMon("Roar"), new AquaCrushMon()));
            r.RegisterByKey("MA-56", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("MA-57", new CardsMA.DelayedConditionMon("Aqua Trance", SpecialCondition.Asleep));
            r.RegisterByKey("MA-58", new PlainMon());
            r.RegisterByKey("MA-59", new CardsE2.AnyOpponentMon("Dig Under", 10));
            r.RegisterByKey("MA-60", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("MA-61", new CardsDRB.NightAttackMon());
            r.RegisterByKey("MA-62", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("MA-63", new CardsE3B.OpponentSwitchesMon("Kick Away"));
            r.RegisterByKey("MA-64", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("MA-65", new SnarlMon());
            r.RegisterByKey("MA-66", new CardsE3B.OpponentSwitchesMon("Darkness Call"));
            r.RegisterByKey("MA-67", new ComboMon(new CoinsMon("Double Stab", 2), CardsSS.ConditionBonus("Second Strike", 20, d => d.DamageCounters >= 2)));
            r.RegisterByKey("MA-68", new KnockOverMon());
        }

        /// <summary>Dark Lift (Poké-Body): with any Darkness Energy attached, this Pokémon's Retreat Cost is 0.</summary>
        internal sealed class DarkLiftMon : Mon
        {
            private readonly Mon attacks;

            public DarkLiftMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || mon != me) return cost;
                return me.Energy.Any(e => CardsHIF1.Provides(src, me, e, EnergyType.Darkness)) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Poison: discard a basic Energy card from Corphish or the attack does nothing; the Defending Pokémon is now Poisoned.</summary>
        private sealed class DarkPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Poison") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                    if (basics.Count == 0) return Nothing(ctx);
                    var card = ctx.ChooseCards(basics, 1, 1, "Dark Poison: discard which basic Energy card?")[0];
                    if (!ctx.Engine.DiscardEnergy(me, card)) return Nothing(ctx);
                }
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Auxiliary Light (Poké-Power): once during your turn, a basic Energy card from your hand onto Lanturn, then 2 damage counters on it; not with a Special Condition.</summary>
        private sealed class LanturnAuxiliaryLight : Mon
        {
            private const string Power = "Auxiliary Light";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return;
                var card = basics.Select(c => c.Def.Key).Distinct().Count() == 1 ? basics[0] : ctx.ChooseCards(basics, 1, 1, Power + ": attach which basic Energy card?")[0];
                if (ctx.Me.Hand.Remove(card) && ctx.Engine.AttachEnergyCard(card, me, true)) ctx.PlaceCounters(me, 2);
            }
        }

        /// <summary>Chaos Crush: if the Defending Pokémon is Confused, discard an Energy card attached to it.</summary>
        private sealed class ChaosCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chaos Crush") return false;
                var defending = ctx.Defending;
                bool confused = defending != null && defending.Has(SpecialCondition.Confused);
                ctx.DamageDefending(atk.Damage);
                if (confused && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Chaos Crush: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Knock Over: you may discard any Stadium card in play (Aurora Beam on Sealeo, Ram on Rhyhorn are plain).</summary>
        internal sealed class KnockOverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knock Over") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium != null && ctx.AskYesNo("Knock Over: discard " + ctx.State.Stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                return true;
            }
        }

        /// <summary>Psymist: 2 coins; for each heads, 1 damage counter on 1 of the opponent's Pokémon.</summary>
        private sealed class PsymistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psymist") return false;
                int heads = ctx.FlipCoins(2);
                if (heads > 0) ctx.PlaceCountersAnyWay(heads, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Pain Amplifier: 1 damage counter on each of the opponent's Pokémon that already has damage counters.</summary>
        private sealed class PainAmplifierMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pain Amplifier") return false;
                foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0)) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Clay Curse: 2 damage counters on the opponent's Pokémon in any way you like.</summary>
        private sealed class ClayCurseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clay Curse") return false;
                ctx.PlaceCountersAnyWay(2, () => ctx.OpponentPokemon());
                return true;
            }
        }

        /// <summary>Clay Pulse: the damage, and 10 to each of the opponent's Benched Pokémon with damage counters.</summary>
        private sealed class ClayPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clay Pulse") return false;
                var hurt = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in hurt) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Call for Help (Poké-Power): once during your turn, while this Pokémon is Active, a Pokémon with Team Magma in its name from
        /// your deck into your hand; not with a Special Condition. Lunge: tails, nothing.
        /// </summary>
        private sealed class MightyenaCallForHelp : Mon
        {
            private const string Power = "Call for Help";
            private static readonly Mon Lunge = new TailsNothingMon("Lunge");

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && CardsMA.Magma(d.Name), 1, Power + ": choose a Pokémon with Team Magma in its name");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lunge.HandleAttack(ctx, atk); }
        }

        /// <summary>"Attach a [kind of] Energy card from your hand to this Pokémon" (Growth, Self Charge).</summary>
        internal sealed class AttachToSelfFromHandMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;

            public AttachToSelfFromHandMon(string attack, Func<CardDef, bool> filter)
            {
                this.attack = attack;
                this.filter = filter;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var cards = ctx.Me.Hand.Where(c => filter(c.Def)).ToList();
                if (cards.Count == 0 || !ctx.InPlay(me)) return true;
                var card = cards.Select(c => c.Def.Key).Distinct().Count() == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, attack + ": attach which Energy card?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Rest: remove all Special Conditions and 3 damage counters (all if fewer) from Jigglypuff; it is now Asleep.</summary>
        private sealed class RestThreeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rest") return false;
                var me = ctx.Pokemon;
                ctx.Engine.RemoveAllConditions(me);
                if (me.DamageCounters > 0) ctx.Heal(me, 30);
                if (ctx.InPlay(me)) ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Shell Retreat (Poké-Body): with any Energy cards attached, the opponent's attacks do 10 less damage to Squirtle (after Weakness and Resistance).</summary>
        private sealed class ShellRetreatMon : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == src.Player || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                return me.Energy.Count > 0 ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Aqua Crush: flip a coin; heads, discard an Energy card attached to the Defending Pokémon.</summary>
        private sealed class AquaCrushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Crush") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Aqua Crush: discard an Energy card from " + defending.Name);
                return true;
            }
        }

        /// <summary>Snarl: heads, 10 damage; tails, the Defending Pokémon is Paralyzed.</summary>
        internal sealed class SnarlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Snarl") return false;
                var defending = ctx.Defending;
                if (ctx.Flip()) ctx.DamageDefending(Math.Max(atk.Damage, 10));
                else if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }
    }
}
