using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Storm (PLS, v111): Grass to Psychic Pokémon (1-58). Victini-EX is Legendary Treasures' card; vanilla attacks need no
    /// script. Team Plasma Pokémon are checked in play with GameEngine.IsTeamPlasma.
    /// </summary>
    public static class CardsPLS1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLS-1", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("PLS-2", new FlipBonusMon("Knock Away", 20));
            r.RegisterByKey("PLS-3", new ComboMon(new CardsASC.SelfGuardMon("Guard Press", 20), new CardsMEG.FlipUntilTailsBonusMon("Rumble Stomp", 20)));
            r.RegisterByKey("PLS-4", new StingMissileMon("Sting Missile"));
            r.RegisterByKey("PLS-5", new ComboMon(new GatherOrderMon(), new CardsASR1.TimesMon("Damage Beat", c => c.Defending == null ? 0 : c.Defending.DamageCounters)));
            r.RegisterByKey("PLS-6", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("PLS-7", new CherrimFairWeather());
            r.RegisterByKey("PLS-9", new ComboMon(new CardsASC.SelfGuardMon("Swaddle Guard", 40), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("PLS-10", new ComboMon(new CardsLTR2.CleaveMon(), new FlipBonusMon("Leaf Blade", 20)));
            r.RegisterByKey("PLS-11", new ComboMon(new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused), new FlipBonusMon("Ambush", 30)));
            r.RegisterByKey("PLS-12", new CardsPLF2.AstonishMon());
            r.RegisterByKey("PLS-13", new ComboMon(new CardsPLF2.AstonishMon(), new CardsSVI1.MiraclePowderMon("Miracle Powder")));
            r.RegisterByKey("PLS-14", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Destructive Flame"), new PlasmaBonusMon("Power Flame", 40)));
            r.RegisterByKey("PLS-16", new ComboMon(new CardsASC.PushDownMon("Loud Howl"), new CardsTWM1.SelfDiscardMon("Fire Tail Slap", 1)));
            r.RegisterByKey("PLS-17", new ComboMon(new CardsPOR.MemoryLockMon("Torment"), new CardsTWM1.SelfDiscardMon("Malevolent Fire", 99)));
            r.RegisterByKey("PLS-19", new CoinsMon("Double Fire", 2));
            r.RegisterByKey("PLS-20", new ComboMon(new ConditionAttackMon("Searing Flame", SpecialCondition.Burned), new CardsMEG.EnergyBonusMon("Flame Blast", EnergyType.Fire, 20)));
            r.RegisterByKey("PLS-21", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("PLS-22", new NoEnergyFreeRetreatMon(null));
            r.RegisterByKey("PLS-23", new ComboMon(new LuringFlameMon(), new FieryLicksMon()));
            r.RegisterByKey("PLS-25", new ComboMon(new CardsMEW1.SpreadBenchMon("Blizzard", 10), new PlasmaParalyzeMon("Frost Prison")));
            r.RegisterByKey("PLS-26", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("PLS-27", new ComboMon(new ParalyzeFlipMon("Ice Beam"), new CoinsMon("Quintuple Headbutt", 5)));
            r.RegisterByKey("PLS-28", new ComboMon(new FlipBonusConditionMon("Frost Stone", 20, SpecialCondition.Paralyzed), new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt")));
            r.RegisterByKey("PLS-29", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("PLS-30", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("PLS-31", new LudicoloRainDish());
            r.RegisterByKey("PLS-33", new CardsASC.CounterattackMon("Rough Skin", 2, new FlipBonusMon("Hard Bite", 20)));
            r.RegisterByKey("PLS-34", new ManaphyFinalWish());
            r.RegisterByKey("PLS-35", new CardsFLF1.NoResistanceMon("Snow Squall"));
            r.RegisterByKey("PLS-37", new ComboMon(new EnefountainMon(), new CardsMEW1.SpreadBenchMon("Blizzard", 10)));
            r.RegisterByKey("PLS-38", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("PLS-39", new JellicentSpitefulSpirit());
            r.RegisterByKey("PLS-40", new HailFlipMon());
            r.RegisterByKey("PLS-41", new ComboMon(new CardsASR1.TimesMon("Powerful Rage", c => c.Pokemon.DamageCounters), new GlacierDropMon()));
            r.RegisterByKey("PLS-42", new CardsPAR2.FlipConditionMon("Metal Sound", SpecialCondition.Confused));
            r.RegisterByKey("PLS-44", new ConditionAttackMon("Metal Sound", SpecialCondition.Confused));
            r.RegisterByKey("PLS-45", new FlipBonusMon("Knock Away", 20));
            r.RegisterByKey("PLS-46", new MagnezoneDualBrains());
            r.RegisterByKey("PLS-47", new ComboMon(new DoubleAssistMon(), new FlipBonusMon("Tumbling Attack", 20)));
            r.RegisterByKey("PLS-48", new ComboMon(new DodgeMon("Agility", false), new PlasmaBonusMon("Powervolt", 40)));
            r.RegisterByKey("PLS-49", new ComboMon(new ElectribonusMon(), new CardsFST1.RevealCountMon("Poltergeist", d => d.IsTrainer, "Trainer")));
            r.RegisterByKey("PLS-50", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("PLS-51", new ComboMon(new CardsUNB1.DischargeMon(), new CardsPAR2.FlipConditionMon("Signal Beam", SpecialCondition.Confused)));
            r.RegisterByKey("PLS-52", new CardsTWM1.HealSelfMon("Spiral Drain", 10));
            r.RegisterByKey("PLS-53", new NoEnergyFreeRetreatMon(null));
            r.RegisterByKey("PLS-54", new CardsTWM1.HealSelfMon("Spiral Drain", 20));
            r.RegisterByKey("PLS-55", new CrobatNightSight());
            r.RegisterByKey("PLS-56", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("PLS-57", new CardsUPR3.SmokescreenMon("Smokescreen", 1));
            r.RegisterByKey("PLS-58", new WeezingAftermath());
        }

        // ================================================================== Shared

        /// <summary>"If this Pokémon has any Plasma Energy attached to it, this attack does N more damage" (Power Flame, Powervolt).</summary>
        internal sealed class PlasmaBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public PlasmaBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (CardsPLB1.PlasmaEnergy(ctx.Pokemon) > 0 ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If this Pokémon has any Plasma Energy attached to it, the Defending Pokémon is now Paralyzed" (Frost Prison).</summary>
        internal sealed class PlasmaParalyzeMon : Mon
        {
            private readonly string attack;

            public PlasmaParalyzeMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool plasma = CardsPLB1.PlasmaEnergy(ctx.Pokemon) > 0;
                ctx.DamageDefending(atk.Damage);
                if (plasma && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, this attack does N more damage and the Defending Pokémon is now [condition]" (Frost Stone).</summary>
        internal sealed class FlipBonusConditionMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly SpecialCondition condition;

            public FlipBonusConditionMon(string attack, int bonus, SpecialCondition condition)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? bonus : 0));
                if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>Freefloating / Free Flight: with no Energy attached, this Pokémon has no Retreat Cost. Attacks come from another script.</summary>
        internal sealed class NoEnergyFreeRetreatMon : Mon
        {
            private readonly Mon attacks;

            public NoEnergyFreeRetreatMon(Mon attacks) { this.attacks = attacks; }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                return me != null && mon == me && me.Energy.Count == 0 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>Sting Missile: 30, then shuffle this Pokémon and all cards attached to it into your deck. Used for Sting Missile, Jumping Shot (the attack name is passed in).</summary>
        internal sealed class StingMissileMon : Mon
        {
            private readonly string attack;

            public StingMissileMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Gather Order: search your deck for any number of Combee and put them onto your Bench.</summary>
        private sealed class GatherOrderMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Order") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                if (ctx.BenchSpace > 0) ctx.SearchToBench(d => d.Name == "Combee" && d.IsBasicPokemon, ctx.BenchSpace, "Gather Order: put any number of Combee onto your Bench");
                else ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Fair-Weather Heal: once during your turn, heal 20 damage from 1 of your Pokémon with any Grass Energy attached. Random Peck.</summary>
        private sealed class CherrimFairWeather : Mon
        {
            private static readonly Mon Peck = new CardsWHT.FlipMoreMon("Random Peck", 2, 20);

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.HasEnergyOfType(m, EnergyType.Grass));
            }

            protected override string Activated { get { return "Fair-Weather Heal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Targets(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Fair-Weather Heal: heal 20 damage from which Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Peck.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>Luring Flame: switch 1 of the opponent's Benched Pokémon with the Defending Pokémon; the new Defending Pokémon is now Burned.</summary>
        private sealed class LuringFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Luring Flame") return false;
                if (ctx.Opp.Bench.Count > 0) ctx.GustOpponent("Luring Flame: choose 1 of your opponent's Benched Pokémon to switch in");
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Fiery Licks: discard the top 4 cards of your deck; 50 damage for each Fire Energy card discarded.</summary>
        private sealed class FieryLicksMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fiery Licks") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 4);
                int n = discarded.Count(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Rain Dish: at any time between turns, heal 20 damage from this Pokémon. Groovy Dance: may discard an Energy to Confuse.</summary>
        private sealed class LudicoloRainDish : Mon
        {
            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || !src.InPlay(me) || me.DamageCounters == 0) return;
                Announce(src, "Rain Dish");
                src.Heal(me, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Groovy Dance") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || !TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.AskYesNo("Groovy Dance: discard an Energy from " + me.Name + " to make the Defending Pokémon Confused?")) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Groovy Dance: discard an Energy") > 0 && TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Final Wish: when this Pokémon is Knocked Out by damage from an opponent's attack, search your deck for a card. Seafaring.</summary>
        private sealed class ManaphyFinalWish : Mon
        {
            private static readonly Mon Seafaring = new CardsXY1.SeafaringMon();

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Me.Deck.Count == 0) return;
                Announce(src, "Final Wish");
                src.SearchToHand(d => true, 1, "Final Wish: put a card from your deck into your hand", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Seafaring.HandleAttack(ctx, atk); }
        }

        /// <summary>Enefountain: 30, and attach 2 basic Energy cards from your hand to 1 of your Pokémon.</summary>
        private sealed class EnefountainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enefountain") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count == 0) return true;
                var picks = energy.Count <= 2 ? energy : ctx.ChooseCards(energy, 2, 2, "Enefountain: attach which 2 basic Energy cards?");
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Enefountain: attach them to which of your Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Spiteful Spirit: if this Pokémon is Active and Knocked Out by damage from an opponent's attack, the Attacking Pokémon is Confused and Poisoned. Aqua Bullet.</summary>
        private sealed class JellicentSpitefulSpirit : Mon
        {
            private static readonly Mon Bullet = new Cards30CC.BenchHitMon("Aqua Bullet", 10);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive || src.Pokemon == null) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                Announce(src, "Spiteful Spirit");
                src.ApplyCondition(attacker, SpecialCondition.Confused);
                src.ApplyCondition(attacker, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bullet.HandleAttack(ctx, atk); }
        }

        /// <summary>Hail: flip a coin; heads, 10 damage to each of the opponent's Pokémon (no Weakness or Resistance on the Bench).</summary>
        private sealed class HailFlipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail") return false;
                if (!ctx.Flip()) return true;
                foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Glacier Drop: 90, then discard the top card of the opponent's deck.</summary>
        private sealed class GlacierDropMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Glacier Drop") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Dual Brains: during your turn you may play 2 Supporter cards (2 in all however many Magnezone you have, 2013-02-07). Gyro
        /// Ball: switch this Pokémon with 1 of your Benched Pokémon, then the opponent switches the Defending Pokémon.
        /// </summary>
        private sealed class MagnezoneDualBrains : Mon
        {
            public override int ModifySupporterLimit(EffectContext src, int player, int limit)
            {
                return src.Pokemon != null && player == src.Player ? Math.Max(limit, 2) : limit;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gyro Ball") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Gyro Ball: switch " + ctx.Pokemon.Name + " with which Benched Pokémon?");
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Double Assist: 30, and attach 2 basic Energy cards from your discard pile to 1 of your Pokémon.</summary>
        private sealed class DoubleAssistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Assist") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                var mine = ctx.MyPokemon();
                if (pile.Count == 0 || mine.Count == 0) return true;
                var picks = pile.Count <= 2 ? pile : ctx.ChooseCards(pile, 2, 2, "Double Assist: attach which 2 basic Energy cards from your discard pile?");
                var target = mine.Count == 1 ? mine[0] : ctx.ChoosePokemon(mine, "Double Assist: attach them to which of your Pokémon?");
                if (target == null) return true;
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Electribonus: discard a Lightning Energy card from your hand; if you do, draw 3 cards.</summary>
        private sealed class ElectribonusMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electribonus") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var lightning = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return true;
                var pick = lightning.Count == 1 ? lightning[0] : ctx.ChooseCards(lightning, 1, 1, "Electribonus: discard a Lightning Energy card")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(3);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Night Sight: once during your turn, draw a card. Ultra-Toxic Fang: Poisoned, 4 counters between turns instead of 1.</summary>
        private sealed class CrobatNightSight : Mon
        {
            protected override string Activated { get { return "Night Sight"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra-Toxic Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned))
                    defending.PoisonCounterBase = 4;
                return true;
            }
        }

        /// <summary>Aftermath: if this Pokémon is Knocked Out by damage from an opponent's attack, discard the top 3 cards of their deck. Smogbank: 20 to each.</summary>
        private sealed class WeezingAftermath : Mon
        {
            private static readonly Mon Smogbank = new CardsCEC3.SpreadAmountMon("Smogbank", 20);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || src.Opp.Deck.Count == 0) return;
                Announce(src, "Aftermath");
                src.Engine.DiscardTopOfDeck(src.Opponent, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smogbank.HandleAttack(ctx, atk); }
        }
    }
}
