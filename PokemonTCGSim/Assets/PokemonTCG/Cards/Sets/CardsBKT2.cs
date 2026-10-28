using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// BREAKthrough (BKT, v97), part 2: the Psychic, Fighting, Darkness, Metal and Fairy Pokémon (61-107). Gastly, Haunter, Gengar
    /// (58-60) and Mr. Mime (97) are the same cards as Generations' and keep those scripts.
    /// </summary>
    public static class CardsBKT2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Psychic
            r.RegisterByKey("BKT-61", new CardsMEG.WeakenDefendingMon("Photon Wave", 30));
            r.RegisterByKey("BKT-62", new ComboMon(new CardsASR1.TimesMon("Shatter Shot", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Psychic)), new DamageChangeMon()));
            r.RegisterByKey("BKT-63", new VanishingStrikeMon());
            r.RegisterByKey("BKT-64", new PsychicInfinityMon());
            r.RegisterByKey("BKT-65", new CardsBKP2.CountersOnActiveMon("Haunt", 2));
            r.RegisterByKey("BKT-66", new MismagiusBKT());
            r.RegisterByKey("BKT-67", new CardsMEW1.WithdrawMon("Mirror Barrier"));
            r.RegisterByKey("BKT-68", new CardsTWM1.HealSelfMon("Nap", 30));
            r.RegisterByKey("BKT-69", new ComboMon(new CardsLOT4.BeckonMon(), new CoinsMon("Quick Turn", 2)));
            r.RegisterByKey("BKT-70", new CresseliaBKT());
            r.RegisterByKey("BKT-71", new CardsRCL1.FlipDiscardToHandMon("Odor Sleuth"));
            r.RegisterByKey("BKT-72", new SwoobatBKT());
            r.RegisterByKey("BKT-73", new SelfBoostMon("Psych Up", 20));
            r.RegisterByKey("BKT-74", new ComboMon(new MindBulletMon(), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            // Fighting
            r.RegisterByKey("BKT-75", new ComboMon(new CoinsMon("Double Scratch", 2), new NoRetreatMon("Sand Tomb")));
            r.RegisterByKey("BKT-76", new CardsCRZ1.PiercingMon("Swift"));
            r.RegisterByKey("BKT-77", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("BKT-78", new ComboMon(new CardsMEG.SnipeMon("Sharpshooting", 30), new BoneWindmillMon()));
            r.RegisterByKey("BKT-79", new CardsASR1.PlusMon("Bone Revenge", 40, c => c.Engine.PrizesTaken(c.Opponent)));
            r.RegisterByKey("BKT-80", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("BKT-81", new ComboMon(new CardsASC.PushDownMon("Push Down"), new GatheringFootstepsMon()));
            r.RegisterByKey("BKT-82", new MamoswineBKT());
            r.RegisterByKey("BKT-83", new CardsUPR3.SmokescreenMon("Nose Jet", 1));
            r.RegisterByKey("BKT-84", new GalladeBKT());
            r.RegisterByKey("BKT-85", new ComboMon(new AcceleratingSpinMon(),
                new CardsFST2.ConditionBonusMon("Prima Rondo", 50, c => c.Engine.HasEnergyOfType(c.Pokemon, EnergyType.Psychic))));
            r.RegisterByKey("BKT-86", new PompousPunchMon());
            r.RegisterByKey("BKT-87", new CardsFLF1.NoResistanceMon("Skyward Kick"));
            // Darkness
            r.RegisterByKey("BKT-88", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Derail", false), new CoinsMon("Pin Missile", 3)));
            r.RegisterByKey("BKT-89", new ComboMon(new ConditionAttackMon("Moonless Madness", SpecialCondition.Confused), new CardsTWM1.SelfDiscardMon("Dark Edge", 1)));
            r.RegisterByKey("BKT-90", new ComboMon(new CardsDAA1.RandomHandToDeckMon("Whiny Voice", 1), new CardsTWM1.SelfDiscardMon("Dark Edge", 1)));
            r.RegisterByKey("BKT-91", new ZoroarkBKT());
            r.RegisterByKey("BKT-92", new CardsEVO3.MetronomeMon("Foul Play"));
            r.RegisterByKey("BKT-93", new CoinsMon("Disorderly Flip", 4));
            r.RegisterByKey("BKT-94", new YveltalBKT());
            // Metal
            r.RegisterByKey("BKT-95", new CardsFST2.ConditionBonusMon("Payback", 60, c => c.Opp.Prizes.Count == 1));
            r.RegisterByKey("BKT-96", new ComboMon(new PainAmplifierMon("Pain Amplifier", 3), new FlipBonusMon("Knock Away", 20)));
            // Fairy
            r.RegisterByKey("BKT-98", new CardsASC.PushDownMon("Roar"));
            r.RegisterByKey("BKT-99", new ComboMon(new StartlingBarkMon(), new TantrumMon()));
            r.RegisterByKey("BKT-101", new FlipWishMon());
            r.RegisterByKey("BKT-102", new ComboMon(new CardsSHF1.HealEachMon("Aromatherapy", 30, false), new MagicalLeafMon()));
            r.RegisterByKey("BKT-103", new FlorgesBKT());
            r.RegisterByKey("BKT-104", new FlorgesBreakBKT());
            r.RegisterByKey("BKT-105", new CardsASR1.TimesMon("Flail", c => c.Pokemon == null ? 0 : c.Pokemon.DamageCounters));
            r.RegisterByKey("BKT-106", new ComboMon(new HeavyPerfumeMon(), new NoRetreatMon("Hug")));
            r.RegisterByKey("BKT-107", new ComboMon(new RainbowForceMon(), new CardsBKT1.HealedBonusMon("Power Creation", 80)));
        }

        // ================================================================== Shared helpers

        /// <summary>"During your next turn, this Pokémon's [attack] does N more damage (before W/R)": the attack boosts itself (Psych Up).</summary>
        internal sealed class SelfBoostMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public SelfBoostMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int uid = me.Uid;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == attack).Sum(b => b.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (!ctx.InPlay(me)) return true;
                var boost = new AttackBoostEffect(me.Name + "'s " + attack + " does " + bonus + " more damage during its next turn (" + attack + ")", ctx.Player, uid, ctx.Turn + 2, attack, bonus);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>
        /// Damage Change: switch all damage counters on this Pokémon with those on the opponent's Active Pokémon. If that Pokémon is
        /// protected from attack effects, the counters that would move onto it are removed and its own stay (Compendium, 2015-10-29).
        /// </summary>
        private sealed class DamageChangeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Change") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                if (!ctx.InPlay(me) || defending == null) return true;
                if (!ctx.Engine.CounterMovementBlocked() && ctx.Engine.CurrentEffectPrevented(defending, true))
                {
                    if (me.DamageCounters > 0)
                    {
                        ctx.Log("Damage Change: " + defending.Name + " is protected, so the " + me.DamageCounters + " damage counters on " + me.Name + " are removed.");
                        me.DamageCounters = 0;
                    }
                    return true;
                }
                ctx.Engine.SwapDamageCounters(me, defending, "Damage Change");
                return true;
            }
        }

        /// <summary>Vanishing Strike: with any Stadium in play, 50 more damage that isn't affected by Resistance or effects on the Defending Pokémon.</summary>
        private sealed class VanishingStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Vanishing Strike") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                if (ctx.State.Stadium != null) ctx.DamageWith(defending, atk.Damage + 50, true, false, true);
                else ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Psychic Infinity: 10 plus 30 for each Energy attached to both Active Pokémon; not affected by Weakness.</summary>
        private sealed class PsychicInfinityMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic Infinity") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int energy = ctx.EnergyCount(ctx.Pokemon) + ctx.EnergyCount(defending);
                ctx.DamageWith(defending, atk.Damage + 30 * energy, false, true, false);
                return true;
            }
        }

        /// <summary>
        /// Twisted Incantation: when you play it from your hand to evolve, you may have the opponent shuffle their hand into their deck
        /// and draw a card for each of their remaining Prize cards. Curse Deeply: Confused.
        /// </summary>
        private sealed class MismagiusBKT : Mon
        {
            private static readonly Mon Curse = new ConditionAttackMon("Curse Deeply", SpecialCondition.Confused);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Twisted Incantation", true,
                    "Twisted Incantation: have your opponent shuffle their hand into their deck and draw a card for each of their remaining Prize cards?")) return;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                ctx.Engine.DrawCards(ctx.Opponent, ctx.Opp.Prizes.Count);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Curse.HandleAttack(ctx, atk); }
        }

        /// <summary>My Way: no Retreat Cost while any Stadium is in play. Moonlight Gain: heal 20 from this Pokémon.</summary>
        private sealed class CresseliaBKT : Mon
        {
            private static readonly Mon Gain = new CardsTWM1.HealSelfMon("Moonlight Gain", 20);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.State.Stadium != null ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Gain.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wave Amplification: during your next turn, this Pokémon's Returning Echo does 60 more damage. Returning Echo: flip a coin;
        /// tails, put this Pokémon and everything attached into your hand (you must flip, even if it's your only Pokémon: 2015-11-19).
        /// </summary>
        private sealed class SwoobatBKT : Mon
        {
            private const string Echo = "Returning Echo";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                int uid = me.Uid;
                if (atk.Name == "Wave Amplification")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s Returning Echo does 60 more damage during its next turn (Wave Amplification)", ctx.Player, uid, ctx.Turn + 2, Echo, 60);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name != Echo) return false;
                int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(b => b.TargetUid == uid && b.AttackName == Echo).Sum(b => b.Bonus);
                ctx.DamageDefending(atk.Damage + extra);
                if (!ctx.Flip() && ctx.InPlay(me)) ctx.Engine.ReturnPokemonToHand(me);
                return true;
            }
        }

        /// <summary>Mind Bullet: choose 1 of the opponent's Pokémon; 20 damage for each Energy attached to it (W/R only for the Active).</summary>
        private sealed class MindBulletMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Bullet") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = all.Count == 1 ? all[0] : ctx.ChoosePokemon(all, "Mind Bullet: choose 1 of your opponent's Pokémon (20 damage for each Energy attached to it)");
                if (target == null) return true;
                ctx.DamageTarget(target, 20 * ctx.EnergyCount(target));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Bone Windmill: if the opponent's Active Pokémon is a Pokémon-EX, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class BoneWindmillMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bone Windmill") return false;
                var defending = ctx.Defending;
                bool ex = defending != null && defending.Def.IsPokemonEX;
                ctx.DamageDefending(atk.Damage);
                if (ex && ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>
        /// Gathering Footsteps: 10 more damage for each [C] in the Retreat Cost of your Swinub, Piloswine and Mamoswine in play, as their
        /// Retreat Costs are now (0 with Float Stone: Compendium, 2015-11-19).
        /// </summary>
        private sealed class GatheringFootstepsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gathering Footsteps") return false;
                int colorless = ctx.MyPokemon(m => m.Name == "Swinub" || m.Name == "Piloswine" || m.Name == "Mamoswine").Sum(m => ctx.Engine.RetreatCostOf(m));
                ctx.DamageDefending(atk.Damage + 10 * colorless);
                return true;
            }
        }

        /// <summary>
        /// Thick Fat: 30 less damage from the opponent's Fire and Water Pokémon's attacks (after W/R). Primordial Boom: 40 more damage if
        /// you have a Stadium in play; heal 40 from this Pokémon if your opponent has one in play.
        /// </summary>
        private sealed class MamoswineBKT : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var a = info.Attacker;
                if (me == null || info.Target != me || a == null || a.Owner == me.Owner || damage <= 0) return damage;
                return src.Engine.IsType(a, EnergyType.Fire) || src.Engine.IsType(a, EnergyType.Water) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Primordial Boom") return false;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                bool theirs = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Opponent;
                ctx.DamageDefending(atk.Damage + (mine ? 40 : 0));
                if (theirs && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 40);
                return true;
            }
        }

        /// <summary>Premonition: once during your turn, look at the top 5 cards of your deck and put them back in any order. Sensitive Blade: 70 more after playing a Supporter this turn.</summary>
        private sealed class GalladeBKT : Mon
        {
            private static readonly Mon Blade = new CardsFST2.ConditionBonusMon("Sensitive Blade", 70, c => c.Me.SupporterPlayedThisTurn);

            protected override string Activated { get { return "Premonition"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { CardsOBF2.ReorderOwnTop(ctx, 5, "Premonition"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blade.HandleAttack(ctx, atk); }
        }

        /// <summary>Accelerating Spin: attach 2 Fighting Energy cards from your discard pile to this Pokémon, then switch it with 1 of your Benched Pokémon.</summary>
        private sealed class AcceleratingSpinMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Accelerating Spin") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var pile = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fighting)).ToList();
                var picks = pile.Count <= 2 ? pile : ctx.ChooseCards(pile, 2, 2, "Accelerating Spin: attach 2 Fighting Energy cards to " + me.Name);
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Pompous Punch: discard a random card from the opponent's hand.</summary>
        private sealed class PompousPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pompous Punch") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Stand In: once during your turn, if this Pokémon is on your Bench, switch it with your Active Pokémon. Mind Jack: 30 more for each of the opponent's Benched Pokémon.</summary>
        private sealed class ZoroarkBKT : Mon
        {
            private static readonly Mon Jack = new CardsASR1.PlusMon("Mind Jack", 30, c => c.Opp.Bench.Count);

            protected override string Activated { get { return "Stand In"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx) { ctx.Engine.SwitchActive(ctx.Player, ctx.Pokemon); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Jack.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Fright Night: while this Pokémon is your Active Pokémon, every Pokémon Tool in play has no effect (engine). Pitch-Black Spear:
        /// also 60 damage to 1 of the opponent's Benched Pokémon-EX.
        /// </summary>
        private sealed class YveltalBKT : Mon
        {
            public override bool DisablesAllToolsWhileActive { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pitch-Black Spear") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(60, m => m.Def.IsPokemonEX);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Pain Amplifier: put 3 damage counters on each of the opponent's Pokémon that has any damage counters on it. Used for Pain Amplifier, Shining Eyes (the attack name and the numbers are passed in).</summary>
        internal sealed class PainAmplifierMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public PainAmplifierMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0))
                    ctx.PlaceCounters(mon, counters);
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Startling Bark: flip a coin; heads, 3 random cards from the opponent's hand are shuffled into their deck.</summary>
        private sealed class StartlingBarkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Startling Bark") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Opp.Hand.Count > 0) CardsTWM1.ShuffleRandomFromOpponentHand(ctx, 3);
                return true;
            }
        }

        /// <summary>Tantrum: this Pokémon is now Confused.</summary>
        private sealed class TantrumMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantrum") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Wish (Flabébé): flip a coin; heads, search your deck for any card and put it into your hand.</summary>
        private sealed class FlipWishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Wish: choose a card to put into your hand", false);
                return true;
            }
        }

        /// <summary>Magical Leaf: flip a coin; heads, 20 more damage and heal 20 from this Pokémon.</summary>
        private sealed class MagicalLeafMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Leaf") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 20 : 0));
                if (heads && ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 20);
                return true;
            }
        }

        /// <summary>Calming Aroma: each of your Pokémon's attacks costs [Y] less (each Florges takes off its own). Wonder Shine: Confused.</summary>
        private sealed class FlorgesBKT : Mon
        {
            private static readonly Mon Shine = new ConditionAttackMon("Wonder Shine", SpecialCondition.Confused);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return;
                cost.Remove(EnergyType.Fairy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shine.HandleAttack(ctx, atk); }
        }

        /// <summary>Floral Breeze: once during your turn, heal 30 damage from your Active Pokémon and remove a Special Condition from it.</summary>
        private sealed class FlorgesBreakBKT : Mon
        {
            private static readonly SpecialCondition[] All =
            {
                SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned
            };

            protected override string Activated { get { return "Floral Breeze"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && (active.DamageCounters > 0 || active.Conditions != SpecialCondition.None);
            }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                if (active.DamageCounters > 0) ctx.Heal(active, 30);
                var present = All.Where(c => active.Has(c)).ToList();
                if (present.Count == 0) return;
                var pick = present.Count == 1 ? present[0] : present[ctx.ChooseOption("Floral Breeze: remove which Special Condition from " + active.Name + "?", present.Select(c => c.ToString()).ToArray())];
                ctx.Engine.RemoveCondition(active, pick);
                ctx.Log(active.Name + " is no longer " + pick + ".");
            }
        }

        /// <summary>Heavy Perfume: Confused, with 6 damage counters instead of 3 for this Confusion.</summary>
        private sealed class HeavyPerfumeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Perfume") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Confused))
                    defending.ConfusionCounters = 6;
                return true;
            }
        }

        /// <summary>Rainbow Force: 30 more for each type of Pokémon on your Bench (Colorless counts; a 2-type Pokémon counts twice: 2016-01-07).</summary>
        private sealed class RainbowForceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Force") return false;
                var types = new HashSet<EnergyType>();
                foreach (var mon in ctx.Me.Bench)
                    foreach (var t in ctx.Engine.TypesOf(mon)) types.Add(t);
                ctx.DamageDefending(atk.Damage + 30 * types.Count);
                return true;
            }
        }
    }
}
