using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Scarlet &amp; Violet Black Star Promos (SVP): the cards that weren't already in the pool. Many are G-mark
    /// (Expanded only). Pokémon whose attacks only do their printed damage need no script: Pineco, Arctibax,
    /// Fidough, Fuecoco (Tackle), Crocalor, Quaxwell, Carvanha, Mareep, Flaaffy (Headbutt), Ampharos, Pikachu ex
    /// (Thunderbolt 120), Darkrai ex, Pawniard, Bisharp, Kingambit (Darkness), Snorlax (Lazy Press), Horsea,
    /// Miraidon (Electric Claws), Bulbasaur, Shroodle's Scratch.
    /// </summary>
    public static class CardsSVP
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SVP-1", new CardsTWM1.HealSelfMon("Mini Drain", 10));
            r.RegisterByKey("SVP-2", new ConditionAttackMon("Super Singe", SpecialCondition.Burned));
            r.RegisterByKey("SVP-3", new FlipBonusMon("Water Splash", 20));
            r.RegisterByKey("SVP-4", new MimikyuExSVP());
            r.RegisterByKey("SVP-5", new QuaquavalSVP());
            r.RegisterByKey("SVP-6", new PawmotSVP());
            r.RegisterByKey("SVP-7", new HawluchaSVP());
            r.RegisterByKey("SVP-8", new RevavroomSVP8());
            r.RegisterByKey("SVP-9", new ParalyzeFlipMon("String Truss"));
            r.RegisterByKey("SVP-10", new CardsTWM1.HealSelfMon("Aurora Gain", 30));
            r.RegisterByKey("SVP-11", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("SVP-12", new CardsPRC1.KnockoutNeedleMon("Dangerous Wave", 100));
            r.RegisterByKey("SVP-13", new Cards30CC.BenchHitMon("Lightning Laser", 30));
            r.RegisterByKey("SVP-14", new CardsTWM1.SelfDiscardMon("Rampaging Fang", 3));
            r.RegisterByKey("SVP-15", new CardsTWM1.SelfDiscardMon("Extreme Current", 1));
            r.RegisterByKey("SVP-16", new AmpharosExSVP());
            r.RegisterByKey("SVP-17", new Cards30CC.BenchHitMon("Aura Sphere", 50));
            r.RegisterByKey("SVP-18", new ComboMon(new PowerRunMon(), new LockAfterAttackMon("Full Throttle", false)));
            r.RegisterByKey("SVP-19", new BaxcaliburSVP());
            r.RegisterByKey("SVP-20", new TinkatonSVP20());
            r.RegisterByKey("SVP-21", new MurkrowSVP());
            r.RegisterByKey("SVP-22", new PelipperSVP());
            r.RegisterByKey("SVP-23", new CardsGEN2.BindWoundMon("Nutrients", 30));
            r.RegisterByKey("SVP-24", new CardsMEG.ChargeSelfMon("Stoke", EnergyType.Fire, 2));
            r.RegisterByKey("SVP-25", new CardsASC.FlipDiscardDefendingEnergyMon("Smithereen Smash"));
            r.RegisterByKey("SVP-26", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("SVP-27", new CardsFCO3.DoTheWaveMon("Adventuring Together", 10));
            r.RegisterByKey("SVP-28", new MiraidonExSVP28());
            r.RegisterByKey("SVP-29", new KoraidonExSVP29());
            r.RegisterByKey("SVP-30", new ChienPaoExSVP());
            r.RegisterByKey("SVP-31", new TinkatonExSVP());
            r.RegisterByKey("SVP-32", new AnnihilapeExSVP());
            r.RegisterByKey("SVP-33", new MeowscaradaExSVP());
            r.RegisterByKey("SVP-34", new SkeledirgeExSVP());
            r.RegisterByKey("SVP-35", new QuaquavalExSVP());
            r.RegisterByKey("SVP-36", new PalafinSVP());
            r.RegisterByKey("SVP-37", new CardsBCR1.DrawUntilAttackMon("Grasping Draw", 7));
            r.RegisterByKey("SVP-38", new TogekissSVP());
            r.RegisterByKey("SVP-39", new MawileSVP());
            r.RegisterByKey("SVP-40", new CardsASC.FlipDiscardDefendingEnergyMon("Static Slap"));
            r.RegisterByKey("SVP-41", new PaldeanWooperSVP());
            r.RegisterByKey("SVP-42", new HoundstoneSVP());
            r.RegisterByKey("SVP-43", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("SVP-44", new RecoilMon("Heat Tackle", 10));
            r.RegisterByKey("SVP-47", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("SVP-48", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("SVP-49", new ZapdosExSVP());
            r.RegisterByKey("SVP-50", new AlakazamExSVP());
            r.RegisterByKey("SVP-51", new SnorlaxSVP51());
            r.RegisterByKey("SVP-52", new MewtwoSVP52());
            r.RegisterByKey("SVP-53", new MewExSVP());
            r.RegisterByKey("SVP-54", new ComboMon(new CardsMEG.SnipeMon("Stealthy Shuriken", 40), new CardsPFL.DamagedBonusMon("Torrential Slash", 120)));
            r.RegisterByKey("SVP-55", new ComboMon(new DrawAttackMon("Triple Draw", 3), new CoinsMon("Incessant Punching", 4)));
            r.RegisterByKey("SVP-56", new CharizardExSVP());
            r.RegisterByKey("SVP-57", new ChiYuSVP());
            r.RegisterByKey("SVP-58", new IronBundleSVP());
            r.RegisterByKey("SVP-59", new XatuSVP());
            r.RegisterByKey("SVP-60", new AegislashSVP());
            r.RegisterByKey("SVP-62", new ParalyzeFlipMon("Cold Tea"));
            r.RegisterByKey("SVP-63", new ComboMon(new AvalancheMon(), new CoinsMon("Rolling Slider", 3)));
            r.RegisterByKey("SVP-67", new RoaringMoonExSVP());
            r.RegisterByKey("SVP-68", new IronValiantExSVP());
            r.RegisterByKey("SVP-70", new GreavardSVP());
            r.RegisterByKey("SVP-71", new FlipBonusMon("Ambush", 20));
            r.RegisterByKey("SVP-72", new GreatTuskExSVP());
            r.RegisterByKey("SVP-73", new IronTreadsExSVP());
            r.RegisterByKey("SVP-75", new MimikyuSVP());
            r.RegisterByKey("SVP-76", new SearchAttackMon("Gather Sunlight", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("SVP-77", new CardsASC.PushDownMon("Magic Whip"));
            r.RegisterByKey("SVP-82", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("SVP-86", new ComboMon(new CardsMEG.WeakenDefendingMon("Daunt", 50), new BenchHurtBonusMon("Proud Fangs", 120)));
            r.RegisterByKey("SVP-88", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("SVP-94", new BelliboltSVP());
            r.RegisterByKey("SVP-95", new SearchAttackMon("Twinkling Hope", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("SVP-96", new ComboMon(new FlatOutDashMon("Flat-Out Dash"), new LockAfterAttackMon("Power Tackle", false)));
            r.RegisterByKey("SVP-99", new ConditionAttackMon("Toxic Teeth", SpecialCondition.Poisoned));
            r.RegisterByKey("SVP-100", new GrafaiaiExSVP());
            r.RegisterByKey("SVP-101", new CardsMEG.FlipUntilTailsBonusMon("Scrappy Spark", 30));
            r.RegisterByKey("SVP-102", new GrassKnotMon("Grass Knot"));
            r.RegisterByKey("SVP-103", new HoundoomExSVP());
            r.RegisterByKey("SVP-104", new ComboMon(new CardsMEG.ChargeSelfMon("Metal-bolize", EnergyType.Metal, 2), new CardsMEG.EnergyBonusMon("Full Metal Knuckle", EnergyType.Metal, 30)));
            r.RegisterByKey("SVP-105", new CardsASC.SelfDiscardTypeMon("Armor Cannon", EnergyType.Fire, 1));
            r.RegisterByKey("SVP-119", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("SVP-120", new PupitarSVP());
            r.RegisterByKey("SVP-121", new RevavroomSVP121());
            r.RegisterByKey("SVP-125", new ArmarougeExSVP125());
            r.RegisterByKey("SVP-130", new KingambitSVP130());
            r.RegisterByKey("SVP-138", new CardsTWM1.SelfDiscardMon("Powered Ball", 1));
            r.RegisterByKey("SVP-139", new FloatMon(EnergyType.Psychic));
            r.RegisterByKey("SVP-140", new ComboMon(new CardsASC.RandomDiscardMon("Knock Off"), new RecoilMon("Wild Press", 60)));
            r.RegisterByKey("SVP-142", new ComboMon(new SwitchSelfMon("Strafe", true), new LockAfterAttackMon("Victory Flame", false)));
            r.RegisterByKey("SVP-143", new MiraidonExSVP143());
            r.RegisterByKey("SVP-155", new WooperSVP());
            r.RegisterByKey("SVP-156", new QuagsireSVP());
            r.RegisterByKey("SVP-157", new ZapdosSVP157());
            r.RegisterByKey("SVP-158", new PachirisuSVP());
            r.RegisterByKey("SVP-160", new SquawkabillyExSVP());
            r.RegisterByKey("SVP-161", new CharizardExSVP161());
            r.RegisterByKey("SVP-162", new CardsMEG.FlipUntilTailsBonusMon("Horrifying Fang", 20));
            r.RegisterByKey("SVP-178", new KyogreExSVP());
            r.RegisterByKey("SVP-179", new CardsASC.ExBonusMon("Rising Horns", 100));
            r.RegisterByKey("SVP-180", new ComboMon(new DrawAttackMon("Time Bellow", 1), new CardsMEG.EnergyBonusMon("Metal Blast", EnergyType.Metal, 20)));
            r.RegisterByKey("SVP-185", new CardsASR1.RevealHandAttackMon("Silent Wing"));
            r.RegisterByKey("SVP-186", new TailsNothingMon("Kick Shot"));
            r.RegisterByKey("SVP-187", new SwitchSelfMon("Gyro Shockwave", false));
            r.RegisterByKey("SVP-188", new ScraftySVP());
            r.RegisterByKey("SVP-197", new BenchHurtBonusMon("Revenge Buster", 120));
            r.RegisterByKey("SVP-198", new ComboMon(new CardsMEG.ChargeSelfMon("Steel Armament", EnergyType.Metal, 1), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("SVP-199", new ComboMon(new SearchAttackMon("Pluck Off", d => CardsSSP1.IsBasicOf(d, EnergyType.Grass), 3), new LockAfterAttackMon("Hammer Whip", false)));
            r.RegisterByKey("SVP-201", new ZebstrikaSVP());
            r.RegisterByKey("SVP-202", new BenchSearchAttackMon("Call for Family", 1));
            // v156.16: Pikachu with Grey Felt Hat (the Van Gogh Museum promo; "a Pikachu" is a card named Pikachu).
            r.RegisterByKey("SVP-85", new PikaPortraitMon());

            r.RegisterByName("Paradise Resort", new ParadiseResort());
            r.RegisterByName("Picnicker", new Picnicker());
            r.RegisterByName("Iono", new Iono());
        }

        /// <summary>Pika-Portrait: search your deck for a Pikachu (a card named exactly Pikachu) and put it onto your Bench.</summary>
        private sealed class PikaPortraitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pika-Portrait") return false;
                ctx.SearchToBench(d => d.Name == "Pikachu", 1, "Pika-Portrait: choose a Pikachu to put onto your Bench");
                return true;
            }
        }

        // ================================================================== Shared helpers

        /// <summary>"If your Benched Pokémon have any damage counters on them, this attack does N more damage."</summary>
        public sealed class BenchHurtBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public BenchHurtBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool hurt = ctx.MyBench().Any(m => m.DamageCounters > 0);
                ctx.DamageDefending(atk.Damage + (hurt ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If this Pokémon has any [T] Energy attached, it has no Retreat Cost" (Voltaic Float, Mist Float).</summary>
        public sealed class FloatMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            public FloatMon(EnergyType type, Mon attacks = null)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && src.Engine.HasEnergyOfType(mon, type) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Prevent all damage done to this Pokémon by attacks from your opponent's Pokémon ex and Pokémon V" (Mysterious Shield, Safeguard).</summary>
        private static bool ExShield(EffectContext src, DamageInfo info)
        {
            return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && TefPokemon.IsExOrV(info.Attacker.Def);
        }

        /// <summary>"During your opponent's next turn, if this Pokémon is damaged by an attack, put damage counters on the Attacking Pokémon equal to the damage."</summary>
        public sealed class MirrorDamageEffect : ContinuousEffect
        {
            public MirrorDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage < 10) return;
                engine.PlaceDamageCounters(info.Attacker, damage / 10, Name);
            }
        }

        /// <summary>The Defending Pokémon can't attack during your opponent's next turn.</summary>
        private static void DefendingCantAttack(EffectContext ctx, PokemonInPlay defending, string attack)
        {
            if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return;
            ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its owner's next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
        }

        /// <summary>Attach up to N Basic Energy cards (matching a filter) from your discard pile to 1 of your Pokémon.</summary>
        private static void AttachFromDiscardToOne(EffectContext ctx, Func<CardDef, bool> filter, int max, List<PokemonInPlay> targets, string source)
        {
            var pile = ctx.BasicEnergyInDiscard(filter);
            if (pile.Count == 0 || targets.Count == 0) return;
            var target = ctx.ChoosePokemon(targets, source + ": attach Energy from your discard pile to which Pokémon?");
            if (target == null) return;
            var picks = ctx.ChooseCards(pile, 0, Math.Min(max, pile.Count), source + ": choose up to " + max + " Basic Energy cards to attach to " + target.Name);
            foreach (var c in picks)
                if (ctx.Me.Discard.Remove(c)) ctx.Engine.AttachEnergyCard(c, target);
        }

        /// <summary>Switch this Pokémon with 1 of your Benched Pokémon; if you do, the opponent switches their Active Pokémon (they choose).</summary>
        private static void DanceSwitch(EffectContext ctx)
        {
            if (ctx.Me.Bench.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return;
            if (ctx.SwitchSelfWithBench()) ctx.OpponentSwitchesActive();
        }

        // ================================================================== Grass

        /// <summary>Bouquet Magic: discard a Basic Grass Energy from your hand to put 3 damage counters on 1 of the opponent's Benched Pokémon. Scratching Nails: 120 more against a damaged Pokémon.</summary>
        private sealed class MeowscaradaExSVP : Mon
        {
            private static readonly Mon Nails = new CardsPFL.DamagedBonusMon("Scratching Nails", 120);

            protected override string Activated { get { return "Bouquet Magic"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Bench.Count > 0 && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass));
            }

            protected override void Ability(EffectContext ctx)
            {
                var grass = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Grass)).ToList();
                var pick = ctx.ChooseCards(grass, 1, 1, "Bouquet Magic: discard a Basic Grass Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                var target = ctx.ChoosePokemon(ctx.OpponentBench(), "Bouquet Magic: put 3 damage counters on which of your opponent's Benched Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Nails.HandleAttack(ctx, atk); }
        }

        /// <summary>Grass Knot: 20 damage for each Colorless in the opponent's Active Pokémon's Retreat Cost. Used for Grass Knot, Shadowy Knot (the attack name is passed in).</summary>
        internal sealed class GrassKnotMon : Mon
        {
            private readonly string attack;

            public GrassKnotMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int retreat = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                ctx.DamageDefending(atk.Damage * retreat);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Infernal Reign: when evolved from hand, attach up to 3 Basic Fire Energy from your deck to your Pokémon. Burning Darkness: 30 more for each Prize card your opponent has taken.</summary>
        private sealed class CharizardExSVP : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.Me.Deck.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Fire));
                if (!TriggerOnce(ctx, "Infernal Reign", ready, "Infernal Reign: search your deck for up to 3 Basic Fire Energy cards and attach them to your Pokémon?")) return;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 3, "Infernal Reign: choose up to 3 Basic Fire Energy cards");
                CardsMEG.AttachFromDeckAnyWay(ctx, found, (c, m) => true, "Infernal Reign");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burning Darkness") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.OpponentPrizesTaken);
                return true;
            }
        }

        /// <summary>Flare Bringer: attach up to 2 Basic Fire Energy from your discard pile to 1 of your Pokémon. Megafire of Envy: 90 more after a Knock Out.</summary>
        private sealed class ChiYuSVP : Mon
        {
            private static readonly Mon Envy = new Cards30C.RetaliateBonusMon("Megafire of Envy", 90);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flare Bringer")
                {
                    AttachFromDiscardToOne(ctx, d => d.EnergyProvides == EnergyType.Fire, 2, ctx.MyPokemon(), "Flare Bringer");
                    return true;
                }
                return Envy.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Vitality Song: heal 30 from each of your Pokémon. Burning Voice: 10 less for each damage counter on this Pokémon.</summary>
        private sealed class SkeledirgeExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Vitality Song")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var m in ctx.MyPokemon()) ctx.Heal(m, 30);
                    return true;
                }
                if (atk.Name == "Burning Voice")
                {
                    ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Crimson Armor: with full HP, takes 80 less damage from the opponent's attacks. Scorching Bazooka: 40 more for each Fire Energy.</summary>
        private sealed class ArmarougeExSVP125 : Mon
        {
            private static readonly Mon Bazooka = new CardsMEG.EnergyBonusMon("Scorching Bazooka", EnergyType.Fire, 40);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || !info.AgainstOpponent || src.Pokemon.DamageCounters > 0) return damage;
                return damage - 80;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bazooka.HandleAttack(ctx, atk); }
        }

        /// <summary>Brave Wing: 100 more if this Pokémon has damage counters. Explosive Vortex: discard 3 Energy.</summary>
        private sealed class CharizardExSVP161 : Mon
        {
            private static readonly Mon Vortex = new CardsTWM1.SelfDiscardMon("Explosive Vortex", 3);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Brave Wing")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Pokemon.DamageCounters > 0 ? 100 : 0));
                    return true;
                }
                return Vortex.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Water

        /// <summary>Energy Carnival: once during your turn, attach a Basic Energy card from your hand to 1 of your Pokémon.</summary>
        private sealed class QuaquavalSVP : Mon
        {
            protected override string Activated { get { return "Energy Carnival"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy); }

            protected override void Ability(EffectContext ctx)
            {
                CardsSSP2.AttachFromHandAnyWay(ctx, d => d.IsBasicEnergy, 1, m => true, "Energy Carnival");
            }
        }

        /// <summary>Super Cold: as often as you like, attach a Basic Water Energy from your hand to 1 of your Pokémon.</summary>
        private sealed class BaxcaliburSVP : Mon
        {
            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Super Cold" && ctx.IsMyTurn
                       && ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water))
                       && ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m)).Count > 0;
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                // The person playing attaches as many as they like, then presses Done; bots attach one per use.
                if (RepeatableAbility.ForPerson(ctx))
                {
                    RepeatableAbility.AttachFromHand(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Water), () => ctx.MyPokemon(), "Super Cold", "a Basic Water Energy");
                    return;
                }
                CardsSSP2.AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 1, m => true, "Super Cold");
            }
        }

        /// <summary>Shivery Chill: once during your turn while Active, search for up to 2 Basic Water Energy. Hail Blade: discard any Water Energy from your Pokémon, 60 each.</summary>
        private sealed class ChienPaoExSVP : Mon
        {
            protected override string Activated { get { return "Shivery Chill"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 2, "Shivery Chill: choose up to 2 Basic Water Energy cards");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail Blade") return false;
                var water = new List<CardInstance>();
                foreach (var m in ctx.MyPokemon())
                    water.AddRange(m.Energy.Where(e => ctx.Engine.EnergyUnitsOf(m, e).Any(u => EnergyMask.Provides(u, EnergyType.Water))));
                var picks = water.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(water, 0, water.Count, "Hail Blade: discard any amount of Water Energy from your Pokémon (" + atk.Damage + " damage each)");
                int n = CardsPFL.DiscardChosenEnergy(ctx, picks);
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Exciting Dance: switch with a Benched Pokémon, then the opponent switches. Spiral Shot: put 2 Energy from this Pokémon into your hand.</summary>
        private sealed class QuaquavalExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Exciting Dance")
                {
                    ctx.DamageDefending(atk.Damage);
                    DanceSwitch(ctx);
                    return true;
                }
                if (atk.Name == "Spiral Shot")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me) || me.Energy.Count == 0) return true;
                    var picks = me.Energy.Count <= 2 ? me.Energy.ToList() : ctx.ChooseCards(me.Energy.ToList(), 2, 2, "Spiral Shot: put 2 Energy into your hand");
                    foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Jet Punch: 30 to 1 Benched Pokémon too. Justice Kick: does nothing unless this Pokémon moved from the Bench to the Active Spot this turn.</summary>
        private sealed class PalafinSVP : Mon
        {
            private static readonly Mon JetPunch = new Cards30CC.BenchHitMon("Jet Punch", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Justice Kick")
                {
                    if (ctx.Pokemon.MovedToActiveOnTurn != ctx.Turn) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return JetPunch.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Hyper Blower: once during your turn on the Bench, switch out the opponent's Active Pokémon (they choose); then discard this Pokémon. Refrigerated Stream: an Evolution Pokémon can't attack next turn.</summary>
        private sealed class IronBundleSVP : Mon
        {
            protected override string Activated { get { return "Hyper Blower"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return !ctx.PokemonIsActive && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.OpponentSwitchesActive() && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardPokemonFromPlay(ctx.Pokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Refrigerated Stream") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && defending.Def.IsEvolution) DefendingCantAttack(ctx, defending, atk.Name);
                return true;
            }
        }

        /// <summary>Avalanche: also 10 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class AvalancheMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Avalanche") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var m in ctx.OpponentBench()) ctx.DamageTarget(m, 10);
                return true;
            }
        }

        /// <summary>Scoop Water: shuffle up to 3 Basic Water Energy from your discard pile into your deck.</summary>
        private sealed class WooperSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scoop Water") return false;
                var water = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (water.Count == 0) return true;
                var picks = ctx.ChooseCards(water, 0, Math.Min(3, water.Count), "Scoop Water: shuffle up to 3 Basic Water Energy cards into your deck");
                foreach (var c in picks)
                    if (ctx.Me.Discard.Remove(c)) ctx.Me.Deck.Add(c);
                if (picks.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Drenched Headbutt: discard the top 3 cards of your deck; 80 damage for each Energy card discarded.</summary>
        private sealed class QuagsireSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drenched Headbutt") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                ctx.DamageDefending(atk.Damage * milled.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        /// <summary>Winding Waves: you may switch out the opponent's Active Pokémon (they choose). Tidal Surge: discard 2 Energy.</summary>
        private sealed class KyogreExSVP : Mon
        {
            private static readonly Mon TidalSurge = new CardsTWM1.SelfDiscardMon("Tidal Surge", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Winding Waves")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Opp.Bench.Count > 0 && ctx.AskYesNo("Winding Waves: switch out your opponent's Active Pokémon?")) ctx.OpponentSwitchesActive();
                    return true;
                }
                return TidalSurge.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Lightning

        /// <summary>Electrogenesis: once during your turn, attach a Basic Lightning Energy from your deck to this Pokémon. Electro Paws: discard all Energy.</summary>
        private sealed class PawmotSVP : Mon
        {
            private static readonly Mon ElectroPaws = new CardsTWM1.SelfDiscardMon("Electro Paws", 99);

            protected override string Activated { get { return "Electrogenesis"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Lightning)); }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Lightning), 1, "Electrogenesis: choose a Basic Lightning Energy card");
                foreach (var c in found) ctx.Engine.AttachEnergyCard(c, ctx.Pokemon);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ElectroPaws.HandleAttack(ctx, atk); }
        }

        /// <summary>Thunderstrike Tail: you may discard 2 Energy from this Pokémon for 100 more damage.</summary>
        private sealed class AmpharosExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderstrike Tail") return false;
                // Compendium: you choose whether to do the extra damage, then discard 2 Energy if you can (1 if that's all
                // there is, or none); the 100 more damage happens either way.
                int bonus = 0;
                if (ctx.AskYesNo("Thunderstrike Tail: discard 2 Energy from this Pokémon for 100 more damage?"))
                {
                    ctx.DiscardEnergyFrom(ctx.Pokemon, Math.Min(2, ctx.Pokemon.Energy.Count), "Thunderstrike Tail: choose 2 Energy to discard");
                    bonus = 100;
                }
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Tandem Unit: once during your turn, put up to 2 Basic Lightning Pokémon from your deck onto your Bench. Photon Blaster: can't attack next turn.</summary>
        private sealed class MiraidonExSVP28 : Mon
        {
            private static readonly Mon PhotonBlaster = new LockAfterAttackMon("Photon Blaster", false);

            protected override string Activated { get { return "Tandem Unit"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToBench(d => d.PokemonType == EnergyType.Lightning, 2, "Tandem Unit: choose up to 2 Basic Lightning Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PhotonBlaster.HandleAttack(ctx, atk); }
        }

        /// <summary>Voltaic Float: no Retreat Cost with Lightning Energy. Multishot Lightning: 90 to 1 damaged Benched Pokémon.</summary>
        private sealed class ZapdosExSVP : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && src.Engine.HasEnergyOfType(mon, EnergyType.Lightning) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Multishot Lightning") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(90, m => m.DamageCounters > 0);
                return true;
            }
        }

        /// <summary>Thunder Wave: flip, Paralyzed. Two-Bump Bolt: discard up to 2 Lightning Energy from this Pokémon, 80 more each.</summary>
        private sealed class BelliboltSVP : Mon
        {
            private static readonly Mon ThunderWave = new ParalyzeFlipMon("Thunder Wave");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Two-Bump Bolt")
                {
                    var me = ctx.Pokemon;
                    var lightning = me.Energy.Where(e => ctx.Engine.EnergyUnitsOf(me, e).Any(u => EnergyMask.Provides(u, EnergyType.Lightning))).ToList();
                    var picks = lightning.Count == 0 ? new List<CardInstance>()
                        : ctx.ChooseCards(lightning, 0, Math.Min(2, lightning.Count), "Two-Bump Bolt: discard up to 2 Lightning Energy (80 more damage each)");
                    int n = 0;
                    foreach (var e in picks) if (ctx.Engine.DiscardEnergy(me, e)) n++;
                    ctx.DamageDefending(atk.Damage + 80 * n);
                    return true;
                }
                return ThunderWave.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Rapid Draw: draw 2. Techno Turbo: attach a Basic Lightning Energy from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class MiraidonExSVP143 : Mon
        {
            private static readonly Mon RapidDraw = new DrawAttackMon("Rapid Draw", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Techno Turbo")
                {
                    ctx.DamageDefending(atk.Damage);
                    AttachFromDiscardToOne(ctx, d => d.EnergyProvides == EnergyType.Lightning, 1, ctx.MyBench(), "Techno Turbo");
                    return true;
                }
                return RapidDraw.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Follow-Up Bolt: 10 more for each damage counter on the opponent's Active Pokémon.</summary>
        private sealed class ZapdosSVP157 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Follow-Up Bolt") return false;
                ctx.DamageDefending(atk.Damage + 10 * (ctx.Defending == null ? 0 : ctx.Defending.DamageCounters));
                return true;
            }
        }

        /// <summary>Crackling Charge: flip 3 coins; attach up to that many Basic Lightning Energy from your discard pile to your Benched Pokémon.</summary>
        private sealed class PachirisuSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crackling Charge") return false;
                int heads = ctx.FlipCoins(3);
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Lightning);
                if (heads > 0 && pile.Count > 0 && ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(pile, heads, (c, m) => !ctx.IsActive(m), "Crackling Charge");
                return true;
            }
        }

        /// <summary>Full Speed: discard your hand and draw 6 cards.</summary>
        private sealed class ZebstrikaSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Full Speed") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(6);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Void Return: you may switch. Energy Burst: 30 for each Energy attached to both Active Pokémon.</summary>
        private sealed class MimikyuExSVP : Mon
        {
            private static readonly Mon VoidReturn = new SwitchSelfMon("Void Return", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Burst")
                {
                    int n = ctx.EnergyCount(ctx.Pokemon) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                return VoidReturn.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Gather Materials: discard a card from your hand to draw 3 (once during your turn). Special Hammer: 90 more with Special Energy.</summary>
        private sealed class TinkatonSVP20 : Mon
        {
            protected override string Activated { get { return "Gather Materials"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Gather Materials: discard a card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Special Hammer") return false;
                bool special = ctx.Pokemon.Energy.Any(e => e.Def.IsAttachedSpecialEnergy);
                ctx.DamageDefending(atk.Damage + (special ? 90 : 0));
                return true;
            }
        }

        /// <summary>Big Hammer: 30 for each card in your hand. Pulverizing Press: ignores effects on the Defending Pokémon.</summary>
        private sealed class TinkatonExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Big Hammer")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Me.Hand.Count);
                    return true;
                }
                if (atk.Name == "Pulverizing Press")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Precious Gift: once at the end of your turn, you may draw until you have 8 cards. Power Cyclone: move an Energy to a Benched Pokémon.</summary>
        private sealed class TogekissSVP : Mon
        {
            public override void OnEndOfTurn(EffectContext src)
            {
                if (src.Pokemon == null || !src.InPlay(src.Pokemon) || src.Me.Hand.Count >= 8 || src.Me.Deck.Count == 0) return;
                if (src.AbilityUsedThisTurn("Precious Gift") || !src.AskYesNo("Precious Gift: draw cards until you have 8 cards in your hand?")) return;
                src.MarkAbilityUsed("Precious Gift");
                Announce(src, "Precious Gift");
                src.DrawUntil(8);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                CardsDRI1.MoveEnergyToBench(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Delve: put up to 2 Item cards from your discard pile into your hand.</summary>
        private sealed class HoundstoneSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Delve") return false;
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var picks = ctx.ChooseCards(items, 0, Math.Min(2, items.Count), "Delve: put up to 2 Item cards into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Mind Jack: 30 more for each of the opponent's Benched Pokémon. Dimensional Hand: plain damage, usable from the Bench too.</summary>
        private sealed class AlakazamExSVP : Mon
        {
            public override bool UsableFromBench(AttackDef attack) { return attack != null && attack.Name == "Dimensional Hand"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mind Jack") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.Opp.Bench.Count);
                return true;
            }
        }

        /// <summary>Reflective Barrier: during the opponent's next turn, damage done to this Pokémon goes back to the Attacking Pokémon as damage counters. Psyslash: discard 2 Energy.</summary>
        private sealed class MewtwoSVP52 : Mon
        {
            private static readonly Mon Psyslash = new CardsTWM1.SelfDiscardMon("Psyslash", 2);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reflective Barrier")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon))
                        ctx.AddEffect(new MirrorDamageEffect(ctx.Pokemon.Name + " reflects attack damage during the opponent's next turn (Reflective Barrier)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                    return true;
                }
                return Psyslash.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Restart: once during your turn, draw until you have 3 cards. Genome Hacking: use 1 of the Defending Pokémon's attacks.</summary>
        private sealed class MewExSVP : Mon
        {
            protected override string Activated { get { return "Restart"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 3 && ctx.Me.Deck.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(3); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Genome Hacking") return false;
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, "Genome Hacking: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index)
                    && copied.Attacks[index].Name != "Genome Hacking")
                    UseCopiedAttack(ctx, copied, index);
                return true;
            }
        }

        /// <summary>Clairvoyant Sense: once during your turn, attach a Basic Psychic Energy from your hand to 1 of your Benched Pokémon; if you do, draw 2.</summary>
        private sealed class XatuSVP : Mon
        {
            protected override string Activated { get { return "Clairvoyant Sense"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)) && ctx.MyBench().Any(m => ctx.Engine.CanAttachFromHand(m));
            }

            protected override void Ability(EffectContext ctx)
            {
                int n = CardsSSP2.AttachFromHandAnyWay(ctx, d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 1, m => !ctx.IsActive(m), "Clairvoyant Sense");
                if (n > 0) ctx.Draw(2);
            }
        }

        /// <summary>Tachyon Bits: when this Pokémon moves from your Bench to the Active Spot, you may put 2 damage counters on 1 of the opponent's Pokémon. Laser Blade: can't attack next turn.</summary>
        private sealed class IronValiantExSVP : Mon
        {
            private static readonly Mon LaserBlade = new LockAfterAttackMon("Laser Blade", false);

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Tachyon Bits", ctx.Opp.HasPokemonInPlay, "Tachyon Bits: put 2 damage counters on 1 of your opponent's Pokémon?")) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Tachyon Bits: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LaserBlade.HandleAttack(ctx, atk); }
        }

        /// <summary>Graveyard Gamboling: 10 damage for each Psychic Pokémon in your discard pile.</summary>
        private sealed class GreavardSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Graveyard Gamboling") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Psychic));
                return true;
            }
        }

        /// <summary>Safeguard: prevent all damage from the opponent's Pokémon ex and Pokémon V. Ghost Eye: 7 damage counters on the opponent's Active Pokémon.</summary>
        private sealed class MimikyuSVP : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage) { return ExShield(src, info); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ghost Eye") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 7);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Flying Entry: when played from your hand onto your Bench, you may put 1 damage counter on each of 2 of the opponent's Benched Pokémon.</summary>
        private sealed class HawluchaSVP : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var bench = ctx.OpponentBench();
                if (!TriggerOnce(ctx, "Flying Entry", bench.Count > 0, "Flying Entry: put 1 damage counter on each of 2 of your opponent's Benched Pokémon?")) return;
                var picks = ctx.ChoosePokemonMany(bench, Math.Min(2, bench.Count), 2, "Flying Entry: choose 2 of your opponent's Benched Pokémon");
                foreach (var m in picks) ctx.PlaceCounters(m, 1);
            }
        }

        /// <summary>Dino Cry: once during your turn, attach up to 2 Basic Fighting Energy from your discard pile to your Basic Fighting Pokémon; your turn ends. Wild Impact: can't attack next turn.</summary>
        private sealed class KoraidonExSVP29 : Mon
        {
            private static readonly Mon WildImpact = new LockAfterAttackMon("Wild Impact", false);

            private static bool Target(EffectContext ctx, PokemonInPlay m) { return m.Def.IsBasicPokemon && ctx.Engine.IsType(m, EnergyType.Fighting); }

            protected override string Activated { get { return "Dino Cry"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting).Count > 0 && ctx.MyPokemon(m => Target(ctx, m)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var pile = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fighting);
                ctx.AttachCardsAnyWay(pile, 2, (c, m) => Target(ctx, m), "Dino Cry");
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WildImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>Angry Grudge: put up to 12 damage counters on this Pokémon; 20 damage for each.</summary>
        private sealed class AnnihilapeExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Angry Grudge") return false;
                int n = ctx.AskNumber(0, 12, "Angry Grudge: put how many damage counters on this Pokémon (" + atk.Damage + " damage each)?");
                int placed = n > 0 ? ctx.Engine.PlaceDamageCounters(ctx.Pokemon, n, "Angry Grudge") : 0;
                ctx.DamageDefending(atk.Damage * placed);
                return true;
            }
        }

        /// <summary>Quaking Demolition: at the end of your turn, if this Pokémon is Active, discard the top 5 cards of your deck. Great Bash: ignores effects on the Defending Pokémon.</summary>
        private sealed class GreatTuskExSVP : Mon
        {
            public override void OnEndOfTurn(EffectContext src)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon) || src.Me.Deck.Count == 0) return;
                Announce(src, "Quaking Demolition");
                src.Engine.DiscardTopOfDeck(src.Player, 5);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Great Bash") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
        }

        /// <summary>Blasting Tackle: also 20 damage to 1 of your own Benched Pokémon.</summary>
        private sealed class PupitarSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blasting Tackle") return false;
                ctx.DamageDefending(atk.Damage);
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Blasting Tackle: do 20 damage to which of your Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 20);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Spin Turn: switch. United Wings: 20 for each Pokémon with United Wings in your discard pile.</summary>
        private sealed class MurkrowSVP : Mon
        {
            private static readonly Mon SpinTurn = new SwitchSelfMon("Spin Turn", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "United Wings")
                {
                    int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => a.Name == "United Wings"));
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                return SpinTurn.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Splattering Poison: both Active Pokémon are now Poisoned.</summary>
        private sealed class PaldeanWooperSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splattering Poison") return false;
                if (ctx.Defending != null && ctx.CanAffect(ctx.Defending)) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Poisoned);
                if (ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Frenzied Gouging: Knock Out the opponent's Active Pokémon; if you do, 200 damage to this Pokémon. Calamity Storm: discard a Stadium for 120 more.</summary>
        private sealed class RoaringMoonExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Frenzied Gouging")
                {
                    var defending = ctx.Defending;
                    if (defending != null && ctx.Engine.KnockOut(defending, "Frenzied Gouging")) ctx.DamageSelf(200);
                    return true;
                }
                if (atk.Name == "Calamity Storm")
                {
                    // Compendium (Neutralization Zone, Lively Stadium): you choose to discard the Stadium, the damage is done
                    // while it's still in play (so it can still prevent or change that damage), then it's discarded.
                    bool bonus = ctx.State.Stadium != null && ctx.AskYesNo("Calamity Storm: discard the Stadium in play for 120 more damage?");
                    ctx.DamageDefending(atk.Damage + (bonus ? 120 : 0));
                    if (bonus) CardsTWM1.DiscardStadiumInPlay(ctx);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Numbing Saliva: flip, Paralyzed. Toxic Throw: Poisoned; can't use Toxic Throw next turn.</summary>
        private sealed class GrafaiaiExSVP : Mon
        {
            private static readonly Mon NumbingSaliva = new ParalyzeFlipMon("Numbing Saliva");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Toxic Throw")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(atk.Name);
                    return true;
                }
                return NumbingSaliva.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Evil Claw: a Basic Defending Pokémon can't attack next turn. Hound's Fang: 30 to itself.</summary>
        private sealed class HoundoomExSVP : Mon
        {
            private static readonly Mon HoundsFang = new RecoilMon("Hound's Fang", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Evil Claw")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (defending != null && defending.Def.Stage == Stage.Basic) DefendingCantAttack(ctx, defending, atk.Name);
                    return true;
                }
                return HoundsFang.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Nab 'n' Dash: search your deck for up to as many cards as you have Benched Pokémon.</summary>
        private sealed class ScraftySVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nab 'n' Dash") return false;
                int n = ctx.Me.Bench.Count;
                if (n > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, n, "Nab 'n' Dash: choose up to " + n + " cards", true, 1);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Rumbling Engine: discard an Energy card from your hand to draw until you have 6 cards. Knock Away: flip for 90 more.</summary>
        private sealed class RevavroomSVP8 : Mon
        {
            private static readonly Mon KnockAway = new FlipBonusMon("Knock Away", 90);

            protected override string Activated { get { return "Rumbling Engine"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c.Def.IsEnergy) && ctx.Me.Hand.Count <= 6 && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var pick = ctx.ChooseCards(energy, 1, 1, "Rumbling Engine: discard an Energy card from your hand");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                ctx.DrawUntil(6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return KnockAway.HandleAttack(ctx, atk); }
        }

        /// <summary>Special Eater: when played from your hand onto your Bench, you may discard a Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class MawileSVP : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                bool ready = d != null && d.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(d);
                if (!TriggerOnce(ctx, "Special Eater", ready, "Special Eater: discard a Special Energy from your opponent's Active Pokémon?")) return;
                ctx.DiscardEnergyFrom(d, 1, "Special Eater: choose a Special Energy to discard", e => e.Def.IsAttachedSpecialEnergy);
            }
        }

        /// <summary>Mysterious Shield: prevent all damage from the opponent's Pokémon ex and Pokémon V. Hard Bashing: ignores effects on the Defending Pokémon.</summary>
        private sealed class AegislashSVP : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage) { return ExShield(src, info); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Bashing") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
        }

        /// <summary>Iron-Clad Roll: after damage, you may discard Future Booster Energy Capsule from it to take 150 less next turn.</summary>
        private sealed class IronTreadsExSVP : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron-Clad Roll") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || !me.HasToolNamed("Future Booster Energy Capsule")) return true;
                if (!ctx.AskYesNo("Iron-Clad Roll: discard Future Booster Energy Capsule to take 150 less damage next turn?")) return true;
                var capsule = me.Tools.First(t => t.Def.Name == "Future Booster Energy Capsule");
                if (ctx.Engine.DiscardTool(me, capsule)) ctx.SelfTakesLessNextTurn(150);
                return true;
            }
        }

        /// <summary>Swagger: flip, discard an Energy from the Defending Pokémon. Huge Tackle: 80 more with more cards in hand than the opponent.</summary>
        private sealed class RevavroomSVP121 : Mon
        {
            private static readonly Mon Swagger = new CardsASC.FlipDiscardDefendingEnergyMon("Swagger");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Huge Tackle")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count > ctx.Opp.Hand.Count ? 80 : 0));
                    return true;
                }
                return Swagger.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Strike Down: if the opponent's Active Pokémon has 4 or more damage counters, it is Knocked Out.</summary>
        private sealed class KingambitSVP130 : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strike Down") return false;
                var d = ctx.Defending;
                if (d == null || d.DamageCounters < 4) return Nothing(ctx);
                ctx.Engine.KnockOut(d, "Strike Down");
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Power Run: search your deck for a Basic Energy and attach it to this Pokémon.</summary>
        private sealed class PowerRunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Run") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Power Run: choose a Basic Energy card to attach to " + ctx.Pokemon.Name);
                foreach (var c in found) ctx.Engine.AttachEnergyCard(c, ctx.Pokemon);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Hearsay: when evolved from hand, take a Supporter from your discard pile or search your deck for one.</summary>
        private sealed class PelipperSVP : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool fromDiscard = ctx.Me.Discard.Any(c => c.Def.IsSupporter);
                bool fromDeck = ctx.Me.Deck.Count > 0;
                if (!TriggerOnce(ctx, "Hearsay", fromDiscard || fromDeck, "Hearsay: get a Supporter card?")) return;
                int choice = fromDiscard && fromDeck
                    ? ctx.ChooseOption("Hearsay: choose 1", "Put a Supporter card from your discard pile into your hand", "Search your deck for a Supporter card")
                    : (fromDiscard ? 0 : 1);
                if (choice == 0)
                {
                    var supporters = ctx.Me.Discard.Where(c => c.Def.IsSupporter).ToList();
                    var pick = ctx.ChooseCards(supporters, 1, 1, "Hearsay: choose a Supporter card from your discard pile");
                    ctx.DiscardToHand(pick);
                }
                else ctx.SearchToHand(d => d.IsSupporter, 1, "Hearsay: choose a Supporter card");
            }
        }

        /// <summary>Voraciousness: once during your turn, put up to 2 Leftovers from your discard pile into your hand. Thudding Press: 30 to itself.</summary>
        private sealed class SnorlaxSVP51 : Mon
        {
            private static readonly Mon ThuddingPress = new RecoilMon("Thudding Press", 30);

            protected override string Activated { get { return "Voraciousness"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Discard.Any(c => c.Def.Name == "Leftovers"); }

            protected override void Ability(EffectContext ctx)
            {
                var leftovers = ctx.Me.Discard.Where(c => c.Def.Name == "Leftovers").ToList();
                var picks = ctx.ChooseCards(leftovers, 0, Math.Min(2, leftovers.Count), "Voraciousness: put up to 2 Leftovers into your hand");
                ctx.DiscardToHand(picks);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ThuddingPress.HandleAttack(ctx, atk); }
        }

        /// <summary>Flat-Out Dash: flip until tails; draw a card for each heads. Used for Flat-Out Dash, Splashy Splash (the attack name is passed in).</summary>
        internal sealed class FlatOutDashMon : Mon
        {
            private readonly string attack;

            public FlatOutDashMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Draw(heads);
                return true;
            }
        }

        /// <summary>Squawk and Seize: once during your first turn, discard your hand and draw 6 (only 1 per turn). Motivate: attach up to 2 Basic Energy from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class SquawkabillyExSVP : Mon
        {
            protected override string Activated { get { return "Squawk and Seize"; } }
            protected override bool OnePerPlayer { get { return true; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsMyFirstTurn && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Motivate") return false;
                ctx.DamageDefending(atk.Damage);
                AttachFromDiscardToOne(ctx, d => true, 2, ctx.MyBench(), "Motivate");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Stadium: each Psyduck's Retreat Cost is 1 less.</summary>
        private sealed class ParadiseResort : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon == null && mon != null && mon.Name == "Psyduck" ? Math.Max(0, cost - 1) : cost;
            }
        }

        /// <summary>Flip a coin: heads draw 4 cards, tails draw 2.</summary>
        private sealed class Picnicker : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(ctx.Flip() ? 4 : 2); }
        }

        /// <summary>Each player shuffles their hand and puts it on the bottom of their deck; then each draws a card for each of their remaining Prize cards.</summary>
        private sealed class Iono : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool any = false;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    if (p.Hand.Count == 0) continue;
                    if (player != ctx.Player && TefPokemon.OpponentShielded(ctx)) continue;
                    var hand = p.Hand.ToList();
                    ctx.State.Rng.Shuffle(hand);
                    p.Hand.Clear();
                    p.Deck.AddRange(hand);
                    any = true;
                    ctx.Engine.Log(player, ctx.Engine.PlayerName(player) + " put " + hand.Count + (hand.Count == 1 ? " card" : " cards") + " on the bottom of their deck (Iono).");
                }
                if (!any) return;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                    ctx.Engine.DrawCards(player, ctx.State.Players[player].Prizes.Count);
            }
        }
    }
}
