using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Primal Clash (PRC, v101), part 1: the Grass, Fire and Water Pokémon (1-60). Tangela (4) is Generations' card. The Ancient
    /// Traits Ω Barrier, Ω Barrage, α Growth and α Recovery are applied by the engine from the card data.
    /// </summary>
    public static class CardsPRC1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PRC-1", new CardsUNM1.NamedBenchSearchMon("Multiply", "Weedle", 1));
            r.RegisterByKey("PRC-3", new ComboMon(new AllergicShockMon(), new CoinsMon("Twineedle", 2)));
            r.RegisterByKey("PRC-5", new ComboMon(new CardsTWM1.HealSelfMon("Mega Drain", 20), new CardsOBF1.RetreatCostDamageMon("Grass Knot", 10, false)));
            r.RegisterByKey("PRC-6", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("PRC-7", new DodgeMon("Agility", false));
            r.RegisterByKey("PRC-8", new SceptileNurture());
            r.RegisterByKey("PRC-9", new ComboMon(new FlipBonusMon("Leaf Blade", 30), new PowerPoisonMon()));
            r.RegisterByKey("PRC-12", new LudicoloRhythm());
            r.RegisterByKey("PRC-14", new ComboMon(new SpiralGyrationMon(), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("PRC-15", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("PRC-16", new DynamicPunchMon(20));
            r.RegisterByKey("PRC-17", new ComboMon(new CardsWHT.FlipMoreMon("Acrobatics", 2, 20), new CardsFST2.ConditionBonusMon("Pester", 30, HasCondition)));
            r.RegisterByKey("PRC-18", new ComboMon(new CardsSHF2.HelpingHandMon(), new ConditionAttackMon("Twirling Sign", SpecialCondition.Confused)));
            r.RegisterByKey("PRC-19", new ComboMon(new NoRetreatMon("Dark Forest"), new CardsMEG.EnergyBonusMon("Wood Blast", EnergyType.Grass, 20)));
            r.RegisterByKey("PRC-20", new CardsDRI1.WhirlwindMon("Roar"));
            r.RegisterByKey("PRC-21", new NinetalesShrine());
            r.RegisterByKey("PRC-22", new CardsFST1.DiscardDefendingTypeMon("Grass Fire", EnergyType.Grass));
            r.RegisterByKey("PRC-23", new ComboMon(new TwoBenchOnlyMon("Flame Burst", 20), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("PRC-24", new CardsFST2.ConditionBonusMon("Indomitable Blaze", 60, c => c.Defending != null && c.Defending.Def.IsPokemonEX));
            r.RegisterByKey("PRC-25", new CardsGEN1.FlipTailsDiscardTypeMon("Ember", EnergyType.Fire, 1));
            r.RegisterByKey("PRC-26", new ComboMon(new FlareBonusTwoMon(), new TailsNothingMon("Claw")));
            r.RegisterByKey("PRC-27", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("PRC-28", new ComboMon(new CardsBST1.SpreadingFlamesMon(), new CardsTWM1.SelfDiscardMon("Heat Blow", 1)));
            r.RegisterByKey("PRC-29", new ComboMon(new FlipBonusMon("Tumbling Attack", 30), new CardsDRM2.ExplosiveJetMon()));
            r.RegisterByKey("PRC-30", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("PRC-31", new KnockoutNeedleMon("Knockout Needle", 40));
            r.RegisterByKey("PRC-32", new TailsNothingMon("Lunge"));
            r.RegisterByKey("PRC-34", new CardsWHT.FlipMoreMon("Endeavor", 2, 20));
            r.RegisterByKey("PRC-35", new CardsMEG.SnipeMon("Water Arrow", 60));
            r.RegisterByKey("PRC-36", new SwampertDiving());
            r.RegisterByKey("PRC-37", new ComboMon(new CardsEVS1.AstonishMon("Astonish"), new CardsASC.NextTurnBoostMon("Splash Dance", 60)));
            r.RegisterByKey("PRC-38", new WailordEX());
            r.RegisterByKey("PRC-40", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new FlipBonusMon("Rising Lunge", 30)));
            r.RegisterByKey("PRC-41", new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20));
            r.RegisterByKey("PRC-42", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("PRC-43", new CardsFST1.FlipRecoilMon("Float On", 10));
            r.RegisterByKey("PRC-44", new MiloticRipples());
            r.RegisterByKey("PRC-45", new CardsPFL.FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("PRC-47", new ParalyzeFlipMon("Freezing Headbutt"));
            r.RegisterByKey("PRC-48", new ComboMon(new CardsLOT4.KnockOverMon(), new CardsFST2.ConditionBonusMon("Frozen Splash", 70, c => c.Defending != null && c.Engine.IsType(c.Defending, EnergyType.Fighting))));
            r.RegisterByKey("PRC-49", new ShellProtectionMon());
            r.RegisterByKey("PRC-50", new ComboMon(new CardsCRZ1.PowerfulStormMon(), new CardsASC.FlipDiscardDefendingEnergyMon("Crunch")));
            r.RegisterByKey("PRC-51", new ComboMon(new CardsDAA1.GustAttackMon("Captivate", false), new CardsMEG.DefendingEnergyBonusMon("Psychic", 10)));
            r.RegisterByKey("PRC-52", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PRC-53", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Spring Tides"), new CardsMEW1.SpreadBenchMon("Ocean Cyclone", 10)));
            r.RegisterByKey("PRC-54", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep), new GiantWhirlpoolMon()));
            r.RegisterByKey("PRC-55", new CardsAOR2.TidalStormMon());
            r.RegisterByKey("PRC-56", new ComboMon(new CardsBKT1.DeepSeaSwirlMon("Deep Sea Swirl", 6), new LifeSaverMon()));
            r.RegisterByKey("PRC-58", new ComboMon(new DodgeMon("Dive", false), new RecoilMon("Take Down", 20)));
            r.RegisterByKey("PRC-59", new CardsMEG.ChargeSelfMon("Charge", EnergyType.Lightning, 1));
            r.RegisterByKey("PRC-60", new ParalyzeFlipMon("Thunder Fang"));
        }

        /// <summary>The opponent's Active Pokémon has a Special Condition (Pester).</summary>
        internal static bool HasCondition(EffectContext ctx) { return ctx.Defending != null && ctx.Defending.Conditions != SpecialCondition.None; }

        // ================================================================== Grass

        /// <summary>
        /// During your next turn, if the Defending Pokémon is damaged by an attack, it is Knocked Out (Allergic Shock). When the damage
        /// alone Knocks it Out, it's Knocked Out by damage (Revenge and Retaliate count it); otherwise Allergic Shock does it and they
        /// don't (Compendium, 2015-02-26).
        /// </summary>
        internal sealed class AllergicShockEffect : ContinuousEffect
        {
            public AllergicShockEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (damage <= 0 || info.Attacker == null || info.Target == null || info.Target.Uid != TargetUid || engine.State.TurnNumber != ExpiresAfterTurn) return;
                if (engine.RemainingHp(info.Target) <= 0) return;
                engine.KnockOut(info.Target, "Allergic Shock");
            }
        }

        private sealed class AllergicShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Allergic Shock") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var shock = new AllergicShockEffect(defending.Name + " is Knocked Out if it's damaged by an attack during " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Allergic Shock)",
                    ctx.Player, defending.Uid, ctx.Turn + 2);
                shock.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(shock);
                return true;
            }
        }

        /// <summary>Nurture and Heal: once during your turn, attach a Grass Energy card from your hand to 1 of your Pokémon and heal 30 from it.</summary>
        private sealed class SceptileNurture : Mon
        {
            protected override string Activated { get { return "Nurture and Heal"; } }

            private static List<CardInstance> Grass(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return Grass(ctx).Count > 0 && ctx.MyPokemon().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            protected override void Ability(EffectContext ctx)
            {
                var cards = Grass(ctx);
                var card = cards.Count == 1 ? cards[0] : ctx.ChooseCards(cards, 1, 1, "Nurture and Heal: attach which Grass Energy?")[0];
                var targets = ctx.MyPokemon().Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Nurture and Heal: attach " + card.Def.Name + " to which Pokémon?");
                if (target == null || !ctx.Me.Hand.Remove(card)) return;
                if (ctx.Engine.AttachEnergyCard(card, target, true) && ctx.InPlay(target)) ctx.Heal(target, 30);
            }
        }

        /// <summary>Power Poison: discard an Energy from this Pokémon; the opponent's Active Pokémon is now Poisoned.</summary>
        private sealed class PowerPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Poison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Captivating Rhythm: once during your turn, flip a coin; heads, switch 1 of the opponent's Benched Pokémon with their Active.</summary>
        private sealed class LudicoloRhythm : Mon
        {
            private static readonly Mon SolarRay = new CardsFCO1.SolarRayMon();

            protected override string Activated { get { return "Captivating Rhythm"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) ctx.GustOpponent("Captivating Rhythm: switch which of your opponent's Benched Pokémon into the Active Spot?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SolarRay.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral Gyration: the opponent's Active Pokémon is now Confused; switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class SpiralGyrationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Gyration") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Dynamic Punch: flip a coin; heads, 20 more damage and the opponent's Active Pokémon is now Confused. The numbers are passed in.</summary>
        internal sealed class DynamicPunchMon : Mon
        {
            private readonly int amount;

            public DynamicPunchMon(int amount)
            {
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Punch") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? amount : 0));
                if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Barrier Shrine: neither player can play Stadium cards from their hand.</summary>
        private sealed class NinetalesShrine : Mon
        {
            private static readonly Mon Flames = new ConditionAttackMon("Flickering Flames", SpecialCondition.Asleep);

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card) { return card != null && card.Def.IsStadium; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flames.HandleAttack(ctx, atk); }
        }

        /// <summary>"This attack does N damage to 2 of your opponent's Benched Pokémon" and nothing to the Active (Flame Burst).</summary>
        internal sealed class TwoBenchOnlyMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TwoBenchOnlyMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count == 0) return true;
                var picks = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, attack + ": choose 2 of your opponent's Benched Pokémon (" + amount + " damage each)");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Flare Bonus (Torchic): discard a Fire Energy card from your hand; if you do, draw 2 cards.</summary>
        private sealed class FlareBonusTwoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Bonus") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return true;
                var card = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Flare Bonus: discard a Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Knockout Needle: flip 2 coins; if both are heads, 40 more damage. Used for Knockout Needle, Expert Splasher, Hyper Lariat, Dangerous Wave (the attack name and the numbers are passed in).</summary>
        internal sealed class KnockoutNeedleMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public KnockoutNeedleMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.FlipCoins(2) == 2 ? amount : 0));
                return true;
            }
        }

        /// <summary>Diving Search: once during your turn, search your deck for a card, shuffle, and put that card on top.</summary>
        private sealed class SwampertDiving : Mon
        {
            private static readonly Mon Pump = new CardsMEG.EnergyBonusMon("Hydro Pump", EnergyType.Water, 30);

            protected override string Activated { get { return "Diving Search"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => true, 1, "Diving Search: choose a card to put on top of your deck", false, 1);
                ctx.ShuffleMyDeck();
                if (found.Count > 0) ctx.Me.Deck.Insert(0, found[0]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Pump.HandleAttack(ctx, atk); }
        }

        /// <summary>Water Veil: attaching an Energy card from your hand to it removes its Special Conditions. High Breaching: it's now Asleep.</summary>
        private sealed class WailordEX : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || target != src.Pokemon || target.Conditions == SpecialCondition.None) return;
                Announce(src, "Water Veil");
                src.Engine.RemoveAllConditions(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Breaching") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Sparkling Ripples: when it evolves from the hand, you may put a card from your discard pile into your hand. Aqua Swirl: you
        /// may have the opponent switch their Active Pokémon.
        /// </summary>
        private sealed class MiloticRipples : Mon
        {
            private static readonly Mon Swirl = new CardsDRI1.WhirlwindMon("Aqua Swirl", true);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Sparkling Ripples", ctx.Me.Discard.Count > 0, "Sparkling Ripples: put a card from your discard pile into your hand?")) return;
                var pick = ctx.ChooseCards(ctx.Me.Discard.ToList(), 1, 1, "Sparkling Ripples: put which card into your hand?");
                ctx.DiscardToHand(pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Swirl.HandleAttack(ctx, atk); }
        }

        /// <summary>During your opponent's next turn, damage of 50 or less from an attack to this Pokémon is prevented (Shell Protection).</summary>
        internal sealed class SmallDamageShieldEffect : ContinuousEffect
        {
            private readonly int limit;

            public SmallDamageShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn, int limit) : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.limit = limit;
            }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Owner != Owner && damage <= limit;
            }
        }

        private sealed class ShellProtectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shell Protection") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new SmallDamageShieldEffect(me.Name + " takes no attack damage of 50 or less during the opponent's next turn (Shell Protection)", ctx.Player, me.Uid, ctx.Turn + 1, 50));
                return true;
            }
        }

        /// <summary>Giant Whirlpool: put 2 Water Energy attached to this Pokémon into your hand.</summary>
        private sealed class GiantWhirlpoolMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Giant Whirlpool") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var water = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water)).ToList();
                var picks = water.Count <= 2 ? water : ctx.ChooseCards(water, 2, 2, "Giant Whirlpool: put which 2 Water Energy into your hand?");
                foreach (var card in picks) ctx.Engine.ReturnEnergyToHand(me, card);
                return true;
            }
        }

        /// <summary>Life Saver: put 2 Water Pokémon from your discard pile into your hand.</summary>
        private sealed class LifeSaverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Life Saver") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => c.Def.IsPokemon && (c.Def.PokemonType == EnergyType.Water || c.Def.SecondType == EnergyType.Water)).ToList();
                if (pile.Count == 0) return true;
                var picks = pile.Count <= 2 ? pile : ctx.ChooseCards(pile, 2, 2, "Life Saver: put which 2 Water Pokémon into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }
    }
}
