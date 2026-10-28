using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Phantom Forces (PHF, v102), part 2: the Fighting, Darkness, Metal, Fairy, Dragon and Colorless Pokémon (46-90). Swirlix and
    /// Diancie are the Generations Radiant Collection cards.
    /// </summary>
    public static class CardsPHF2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PHF-46", new CardsPFL.FlipUntilTailsTimesMon("Rock Cannon"));
            r.RegisterByKey("PHF-47", new ComboMon(new SubmissionHoldMon(), new CardsPAR2.FlipConditionMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("PHF-49", new CoreHealMon());
            r.RegisterByKey("PHF-50", new GigalithArmor());
            r.RegisterByKey("PHF-51", new FlipBonusMon("Ambush", 10));
            r.RegisterByKey("PHF-52", new ComboMon(new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep),
                new CardsFST2.ConditionBonusMon("Nightmare Mambo", 60, c => c.Defending != null && c.Defending.Has(SpecialCondition.Asleep))));
            r.RegisterByKey("PHF-53", new TailsNothingMon("Lunge"));
            r.RegisterByKey("PHF-55", new ComboMon(new CardsOBF1.QueenlyHeelMon("B Cancel"), new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused)));
            r.RegisterByKey("PHF-56", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("PHF-57", new ComboMon(new CardsMEG.WeakenDefendingMon("Gentle Bite", 60), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("PHF-58", new MalamarEX());
            r.RegisterByKey("PHF-59", new CardsASC.SelfDiscardTypeMon("Iron Wing", EnergyType.Metal, 1));
            r.RegisterByKey("PHF-61", new BronzongLinks());
            r.RegisterByKey("PHF-62", new ComboMon(new CardsEVS1.DefenderCantAttackMon("Chrono Wind", d => d.IsPokemonEX), new CardsASC.SelfDiscardTypeMon("Full Metal Impact", EnergyType.Metal, 2)));
            r.RegisterByKey("PHF-63", new ComboMon(new CardsROS2.AnyStadiumBonusMon("Steel Drop", 40), new CardsTWM1.SelfDiscardMon("Steam Blast", 1)));
            r.RegisterByKey("PHF-64", new ComboMon(new PokeThroughMon(), new CardsMEG.FlipUntilTailsBonusMon("Spiral Rush", 30)));
            r.RegisterByKey("PHF-65", new AegislashEX());
            r.RegisterByKey("PHF-66", new ComboMon(new CallForFamilyMon(), new ConditionAttackMon("Dull Light", SpecialCondition.Confused)));
            r.RegisterByKey("PHF-67", new ComboMon(new SearchAttackMon("Lead", d => d.IsSupporter, 1),
                new CardsASR1.TimesMon("Bright Garden", c => c.MyPokemon().Count(m => c.Engine.IsType(m, EnergyType.Grass) || c.Engine.IsType(m, EnergyType.Fairy)))));
            r.RegisterByKey("PHF-69", new SlurpuffTasting());
            r.RegisterByKey("PHF-70", new ComboMon(new ParalyzeFlipMon("Nuzzle"), new CardsTWM1.HealSelfMon("Spiral Drain", 20)));
            r.RegisterByKey("PHF-72", new FlipBonusMon("Gnaw Off", 10));
            r.RegisterByKey("PHF-73", new CoinsMon("Slam", 2));
            r.RegisterByKey("PHF-74", new HydreigonImpulse());
            r.RegisterByKey("PHF-75", new CardsTWM1.HealSelfMon("Gooey", 10));
            r.RegisterByKey("PHF-76", new CardsTWM1.HealSelfMon("Gooey", 30));
            r.RegisterByKey("PHF-77", new GoodraSlipTrip());
            r.RegisterByKey("PHF-78", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("PHF-79", new DodgeMon("Fly", true));
            r.RegisterByKey("PHF-80", new ComboMon(new CardsTWM1.HealSelfMon("Nap", 30), new LuckyPunchMon("Lucky Punch", 7)));
            r.RegisterByKey("PHF-81", new ComboMon(new TenderVengeanceMon(), new CardsSUM5.OptionalBoostRecoilMon("Charge Dash", 20, 20)));
            r.RegisterByKey("PHF-82", new CardsFST2.ConditionBonusMon("Psybite", 30, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Psychic) > 0));
            r.RegisterByKey("PHF-83", new ScreamingFitMon());
            r.RegisterByKey("PHF-84", new CardsMEG.DefendingEnergyBonusMon("Energy Press", 30));
            r.RegisterByKey("PHF-85", new CardsCEC3.SpreadAmountMon("Boomburst", 20));
            r.RegisterByKey("PHF-86", new CardsMEG.WeakenDefendingMon("Daunt", 40));
            r.RegisterByKey("PHF-88", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Smithereen Smash"), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 10)));
            r.RegisterByKey("PHF-89", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("PHF-90", new ParalyzeFlipMon("Tight Jaw"));
        }

        // ================================================================== Fighting

        /// <summary>During the opponent's next turn, they can't attach Energy cards from their hand to the Defending Pokémon (Submission Hold).</summary>
        internal sealed class NoEnergyFromHandEffect : ContinuousEffect
        {
            public NoEnergyFromHandEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        private sealed class SubmissionHoldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Submission Hold") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new NoEnergyFromHandEffect("No Energy can be attached from the hand to " + defending.Name + " during its next turn (Submission Hold)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Core Heal: discard a Fighting Energy from this Pokémon and heal 50 damage from it.</summary>
        private sealed class CoreHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Heal") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                if (ctx.DiscardEnergyFrom(me, 1, "Core Heal: discard a Fighting Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Fighting)) > 0)
                    ctx.Heal(me, 50);
                return true;
            }
        }

        /// <summary>High Density Armor: with full HP, it takes 50 less from the opponent's attacks (after Weakness and Resistance). Overdrive Smash: +40 next turn.</summary>
        private sealed class GigalithArmor : Mon
        {
            private static readonly Mon Smash = new CardsBKT2.SelfBoostMon("Overdrive Smash", 40);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || info.Attacker.Owner == me.Owner || me.DamageCounters > 0) return damage;
                return damage - 50;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Smash.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>During the opponent's next turn, they can't play Pokémon from their hand to evolve the Defending Pokémon (B Cancel).</summary>
        internal sealed class NoEvolveFromHandEffect : ContinuousEffect
        {
            public NoEvolveFromHandEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>Hyper Hypnosis: whenever you attach an Energy card from your hand to it, you may make the opponent's Active Pokémon Asleep. MAXamar.</summary>
        private sealed class MalamarEX : Mon
        {
            private static readonly Mon Maxamar = new CardsASC.FlipPerEnergyMon("MAXamar");

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var d = src.Opp.Active;
                if (src.Pokemon == null || target != src.Pokemon || d == null || d.Has(SpecialCondition.Asleep)) return;
                if (!src.AskYesNo("Hyper Hypnosis: make your opponent's Active Pokémon Asleep?")) return;
                Announce(src, "Hyper Hypnosis");
                src.ApplyCondition(d, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Maxamar.HandleAttack(ctx, atk); }
        }

        // ================================================================== Metal

        /// <summary>Metal Links: once during your turn, attach a Metal Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class BronzongLinks : Mon
        {
            protected override string Activated { get { return "Metal Links"; } }

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal); }

            protected override bool AbilityReady(EffectContext ctx) { return Pile(ctx).Count > 0 && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Me;
                ctx.AttachCardsAnyWay(Pile(ctx), 1, (card, mon) => me.Bench.Contains(mon), "Metal Links: attach a Metal Energy card from your discard pile to 1 of your Benched Pokémon", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>Poke Through: discard a random card from the opponent's hand.</summary>
        private sealed class PokeThroughMon : Mon
        {
            private static readonly Mon Random = new CardsASC.RandomDiscardMon("Poke Through");

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Random.HandleAttack(ctx, atk); }
        }

        /// <summary>Mighty Shield: no damage from the attacks of the opponent's Pokémon with any Special Energy attached. Slash Blast: +20 per Metal Energy.</summary>
        private sealed class AegislashEX : Mon
        {
            private static readonly Mon Blast = new CardsMEG.EnergyBonusMon("Slash Blast", EnergyType.Metal, 20);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != me.Owner && info.Attacker.Energy.Any(en => en.Def.IsSpecialEnergy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fairy

        /// <summary>Tasting: once during your turn, draw a card; 1 more if this Pokémon is your Active Pokémon.</summary>
        private sealed class SlurpuffTasting : Mon
        {
            private static readonly Mon Pulse = new LightPulseMon();

            protected override string Activated { get { return "Tasting"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(ctx.Me.Active == ctx.Pokemon ? 2 : 1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pulse.HandleAttack(ctx, atk); }
        }

        /// <summary>Light Pulse: during the opponent's next turn, prevent all effects of attacks, except damage, done to this Pokémon.</summary>
        internal sealed class LightPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Light Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new CardsPAR1.AttackEffectShieldEffect(me.Name + " is protected from the effects of attacks during the opponent's next turn (Light Pulse)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Dark Impulse: once during your turn, attach a Darkness Energy card from your discard pile to your Active Pokémon. Crazy Headbutt.</summary>
        private sealed class HydreigonImpulse : Mon
        {
            private static readonly Mon Headbutt = new CardsTWM1.SelfDiscardMon("Crazy Headbutt", 1);

            protected override string Activated { get { return "Dark Impulse"; } }

            private static List<CardInstance> Pile(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Darkness); }

            protected override bool AbilityReady(EffectContext ctx) { return Pile(ctx).Count > 0 && ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                ctx.AttachCardsAnyWay(Pile(ctx), 1, (card, mon) => mon == active, "Dark Impulse: attach a Darkness Energy card from your discard pile to " + active.Name, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Headbutt.HandleAttack(ctx, atk); }
        }

        /// <summary>Slip Trip: neither player can attach Pokémon Tool cards from their hand. Dragon Pulse: discard the top card of your deck.</summary>
        private sealed class GoodraSlipTrip : Mon
        {
            private static readonly Mon Pulse = new CardsBRS1.MillSelfMon("Dragon Pulse", 1);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card) { return card != null && card.Def.IsTool; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pulse.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Lucky Punch: does nothing unless you have exactly 7 cards in your hand. Used for Lucky Punch, Trio-Cheehoo (the attack name and the numbers are passed in).</summary>
        internal sealed class LuckyPunchMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public LuckyPunchMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Me.Hand.Count != amount) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Tender Vengeance: flip a coin; heads, 10 damage for each damage counter on all of your Benched Pokémon.</summary>
        private sealed class TenderVengeanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tender Vengeance") return false;
                int counters = ctx.Me.Bench.Sum(m => m.DamageCounters);
                if (!ctx.Flip() || counters == 0)
                {
                    ctx.Log("Tender Vengeance did no damage.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage * counters);
                return true;
            }
        }

        /// <summary>Screaming Fit: both Active Pokémon are now Confused.</summary>
        private sealed class ScreamingFitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Screaming Fit") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
