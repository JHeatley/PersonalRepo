using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Fusion Strike (FST, 2021), Pokémon 1-130: Grass, Fire, Water, Lightning and Psychic. Fighting through Colorless, the
    /// full-art-only Pokémon (Celebi V, Sandaconda V, Espeon VMAX), the Trainers and Fusion Strike Energy are in CardsFST2.
    /// Grubbin, Oricorio, Zeraora, Toxtricity (108), Mew V and Mew VMAX came into the pool earlier through Crown Zenith and
    /// 30th Celebration. Regulation mark E: Expanded only.
    /// Pokémon whose attacks only do their printed damage need no script (Shroomish, Pansage, Phantump, Trevenant, Dewpider,
    /// Gossifleur, both Vulpix, Ninetales 30, Slugma, Sizzlipede 46, Centiskorch 49, Shellder, Totodile, Croconaw, Mudkip,
    /// Clamperl, Clauncher, Chewtle, Arrokuda, Barraskewda, Pikachu V, Shinx, Luxio, Luxray, Helioptile, Charjabug, Toxel
    /// 105, Snubbull, Galarian Corsola, Munna, Sandygast).
    /// Your rulings (v55): Goodra's Slimy Room flips for every Energy card attached from the hand, by any means; a pair of
    /// Cross Switchers or Crossceivers counts as 2 Item cards played; Fusion Strike Energy keeps its Pokémon's Abilities
    /// against every opponent's Ability that takes Abilities away (GameEngine.AbilityBlockedAt).
    /// </summary>
    public static class CardsFST1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("FST-1", new FlockFST());
            r.RegisterByKey("FST-2", new TakeLessMon(20, null));
            r.RegisterByKey("FST-3", new ButterfreeFST());
            r.RegisterByKey("FST-5", new LockAfterAttackMon("Impact Blow", true));
            r.RegisterByKey("FST-6", new CounterMon("Counter"));
            r.RegisterByKey("FST-8", new CardsMEG.DrawUntilSixMon("Return"));
            r.RegisterByKey("FST-9", new DiscardDefendingTypeMon("Grass Munch", EnergyType.Grass));
            r.RegisterByKey("FST-10", new FlipBonusMon("Trip Over", 20));
            r.RegisterByKey("FST-11", new HealingCircleMon());
            r.RegisterByKey("FST-12", new DiscardFromHandTimesMon("Ditch and Shake", IsToolCard, 99, "Pokémon Tool cards"));
            r.RegisterByKey("FST-14", new AccelgorFST());
            r.RegisterByKey("FST-15", new BailOutMon());
            r.RegisterByKey("FST-20", new MoveEnergyToBenchMon("Bubble Launch"));
            r.RegisterByKey("FST-21", new QueensOrdersMon());
            r.RegisterByKey("FST-22", new CardsTWM1.HealSelfMon("Drain Punch", 30));
            r.RegisterByKey("FST-23", new TwoBenchedMon("G-Max Drum Solo", 40));
            r.RegisterByKey("FST-25", new CardsTWM1.HealSelfMon("Sunny Wind", 20));
            r.RegisterByKey("FST-26", new CardsTWM1.HealSelfMon("Sweet Impact", 30));
            r.RegisterByKey("FST-27", new FlipBonusMon("Wild Whip", 30));
            // Fire
            r.RegisterByKey("FST-31", new NinetalesFST());
            r.RegisterByKey("FST-32", new WarmUpMon());
            r.RegisterByKey("FST-33", new RecoilMon("Heat Tackle", 30));
            r.RegisterByKey("FST-35", new BodySplashMon());
            r.RegisterByKey("FST-36", new FieryCheeringMon());
            r.RegisterByKey("FST-37", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("FST-38", new DiscardFromHandTimesMon("Fling Fire", IsBasicEnergyCard, 2, "basic Energy cards"));
            r.RegisterByKey("FST-39", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new RevealCountMon("Poltergeist", IsTrainerCard, "Trainer")));
            r.RegisterByKey("FST-40", new ChandelureVmax());
            r.RegisterByKey("FST-41", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire), new ExcitingFlameMon()));
            r.RegisterByKey("FST-43", new CardsASC.SelfDiscardTypeMon("Blaze Kick", EnergyType.Fire, 2));
            r.RegisterByKey("FST-44", new LockAfterAttackMon("All-Out Shot", false));
            r.RegisterByKey("FST-45", new GMaxFireballMon());
            r.RegisterByKey("FST-47", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("FST-48", new CoilFST());
            // Water
            r.RegisterByKey("FST-51", new TakeLessMon(30, new TwoBenchedMon("Aqua Split", 30)));
            r.RegisterByKey("FST-52", new SoakInWaterMon());
            r.RegisterByKey("FST-53", new MultishotStarMon());
            r.RegisterByKey("FST-54", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep), new SplashArchMon()));
            r.RegisterByKey("FST-57", new FeraligatrFST());
            r.RegisterByKey("FST-58", new ComboMon(new CardsMEG.SnipeBenchMon("Aqua Liner", 20), new LetsAllRolloutMon()));
            r.RegisterByKey("FST-59", new DiveAndRescueMon());
            r.RegisterByKey("FST-60", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("FST-61", new SwitchSelfMon("Bounce", true));
            r.RegisterByKey("FST-63", new CardsDRI1.EnergyLoopMon("Energy Loop"));
            r.RegisterByKey("FST-64", new SwampertFST());
            r.RegisterByKey("FST-66", new HuntailFST());
            r.RegisterByKey("FST-67", new GorebyssFST());
            r.RegisterByKey("FST-68", new CardsASR1.RevealHandAttackMon("Pry"));
            r.RegisterByKey("FST-69", new ComboMon(new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep), new RevealCountMon("Circus Soaking", IsSupporterCard, "Supporter")));
            r.RegisterByKey("FST-70", new CardsMEW1.CounterBonusMon("Swarm the Wound", 10));
            r.RegisterByKey("FST-71", new FlipRecoilMon("Reckless Charge", 10));
            r.RegisterByKey("FST-72", new GalarianDarmanitanFST());
            r.RegisterByKey("FST-73", new CoinsMon("Dancing Shuriken", 3));
            r.RegisterByKey("FST-75", new CardsMEG.SnipeBenchMon("Snipe Shot", 50));
            r.RegisterByKey("FST-76", new ComboMon(new CardsSSP2.StompOffMon("Trigger Avalanche", 2), new CardsMEW1.CounterBonusMon("Destroyer Punch", 60)));
            r.RegisterByKey("FST-77", new PyukumukuFST());
            r.RegisterByKey("FST-78", new Cards30CC.BenchHitMon("Aqua Bullet", 20));
            r.RegisterByKey("FST-79", new InteleonVmax());
            r.RegisterByKey("FST-81", new FlipBonusMon("Hard Bite", 50));
            r.RegisterByKey("FST-84", new SearchAttackMon("Find Ice", IsWaterEnergyCard, 2));
            r.RegisterByKey("FST-85", new ComboMon(new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep), new AllEnergyToHandMon("Blizzard Loop")));
            // Lightning
            r.RegisterByKey("FST-87", new TailsNothingMon("Single Shot Blast"));
            r.RegisterByKey("FST-88", new ComboMon(new NoWeaknessResistanceMon("Sonic Boom"), new RecoilMon("Explosion", 90)));
            r.RegisterByKey("FST-89", new SparkDuoMon());
            r.RegisterByKey("FST-90", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("FST-94", new SurpriseShortMon());
            r.RegisterByKey("FST-95", new ParalyzeFlipMon("Thunder Wave"));
            r.RegisterByKey("FST-96", new RecoilMon("Thunder", 20));
            r.RegisterByKey("FST-97", new ComboMon(new UpperShockMon(), new RecoilMon("Wild Charge", 30)));
            r.RegisterByKey("FST-99", new Cards30CC.BenchHitMon("Electrobullet", 20));
            r.RegisterByKey("FST-101", new ElectroBlasterMon());
            r.RegisterByKey("FST-103", new ComboMon(new SwitchSelfMon("Smash Turn", true), new Cards30CC.BenchHitMon("Electrobullet", 30)));
            r.RegisterByKey("FST-104", new ComboMon(new CardsASR1.PlusMon("Bolt Storm", 30, LightningEnergyInPlay), new LockAfterAttackMon("Max Bolt", true)));
            r.RegisterByKey("FST-106", new CardsMEG.WeakenDefendingMon("Growl", 30));
            r.RegisterByKey("FST-107", new PunkShockMon());
            r.RegisterByKey("FST-109", new CardsMEG.SnipeMon("Targeted Spark", 30));
            // Psychic
            r.RegisterByKey("FST-110", new LetsAllRolloutMon());
            r.RegisterByKey("FST-111", new SearchAttackMon("Find Treasure", AnyCard, 2));
            r.RegisterByKey("FST-112", new ComboMon(new DrawAttackMon("Double Draw", 2), new ConditionAttackMon("Dazzle Dance", SpecialCondition.Confused)));
            r.RegisterByKey("FST-116", new GranbullFST());
            r.RegisterByKey("FST-118", new ForceRegenerationMon());
            r.RegisterByKey("FST-119", new ChompChompHoldMon());
            r.RegisterByKey("FST-122", new ComboMon(new SleepInducerMon(), new CardsMEG.DefendingEnergyBonusMon("Psychic", 30)));
            r.RegisterByKey("FST-123", new ComboMon(new CardsBLK.PeckOffMon("Joust"), new MoveEnergyToBenchMon("Reflect Energy")));
            r.RegisterByKey("FST-124", new CardsASR1.TimesMon("Melodious Echo", FusionStrikeEnergyInPlay));
            r.RegisterByKey("FST-126", new OppressingSandstormMon("Oppressing Sandstorm"));
            r.RegisterByKey("FST-127", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("FST-128", new NoRetreatMon("Infestation"));
            r.RegisterByKey("FST-129", new SwitchSelfMon("U-turn", false));
            r.RegisterByKey("FST-130", new CardsASR1.TimesMon("Fusion Strike Assault", FusionStrikePokemonInPlay));
        }

        // ================================================================== Filters and counters

        internal static bool AnyCard(CardDef d) { return true; }
        internal static bool IsToolCard(CardDef d) { return d.IsTool; }
        internal static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }
        internal static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        internal static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }
        internal static bool IsWaterEnergyCard(CardDef d) { return d.IsBasicEnergy && d.EnergyProvides == EnergyType.Water; }

        private static int LightningEnergyInPlay(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(mon => ctx.Engine.EnergyCountOfType(mon, EnergyType.Lightning));
        }

        internal static int FusionStrikeEnergyInPlay(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(mon => mon.Energy.Count(card => card.Def.Name == "Fusion Strike Energy"));
        }

        internal static int FusionStrikePokemonInPlay(EffectContext ctx)
        {
            return ctx.MyPokemon(mon => mon.Def.IsFusionStrike).Count;
        }

        /// <summary>The Pokémon has the Let's All Rollout attack printed on it.</summary>
        internal static bool HasRollout(PokemonInPlay mon)
        {
            return mon != null && mon.Def.Attacks.Any(a => a.Name == "Let's All Rollout");
        }

        // ================================================================== Shared helpers

        /// <summary>"This Pokémon takes N less damage from attacks (after applying Weakness and Resistance)" (Exoskeleton, Shell Armor, Wall of Sand).</summary>
        internal sealed class TakeLessMon : Mon
        {
            private readonly int amount;
            private readonly Mon attacks;

            public TakeLessMon(int amount, Mon attacks)
            {
                this.amount = amount;
                this.attacks = attacks;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - amount : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Let's All Rollout: N damage for each of your Benched Pokémon that has the Let's All Rollout attack (this Pokémon doesn't count).</summary>
        internal sealed class LetsAllRolloutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Let's All Rollout") return false;
                int count = ctx.Me.Bench.Count(HasRollout);
                ctx.DamageDefending(atk.Damage * count);
                return true;
            }
        }

        /// <summary>"Flip a coin. If tails, this Pokémon also does N damage to itself" (Reckless Charge).</summary>
        internal sealed class FlipRecoilMon : Mon
        {
            private readonly string attack;
            private readonly int recoil;

            public FlipRecoilMon(string attack, int recoil)
            {
                this.attack = attack;
                this.recoil = recoil;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(recoil);
                return true;
            }
        }

        /// <summary>"This attack also does N damage to 2 of your opponent's Benched Pokémon" (G-Max Drum Solo, Aqua Split). With 1 Benched Pokémon, just that one.</summary>
        internal sealed class TwoBenchedMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public TwoBenchedMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, attack + ": choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>"Your opponent reveals their hand. This attack does N damage for each [kind of] card you find there" (Poltergeist, Circus Soaking).</summary>
        internal sealed class RevealCountMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly string what;

            public RevealCountMon(string attack, Func<CardDef, bool> filter, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                TefPokemon.RevealOpponentHand(ctx);
                int count = ctx.Opp.Hand.Count(card => filter(card.Def));
                ctx.Log(attack + ": " + count + " " + what + (count == 1 ? " card" : " cards") + " in " + ctx.Engine.Their(ctx.Opponent) + " hand.");
                ctx.DamageDefending(atk.Damage * count);
                return true;
            }
        }

        /// <summary>"Discard any number of / up to N [cards] from your hand. This attack does X damage for each card you discarded" (Ditch and Shake, Fling Fire).</summary>
        internal sealed class DiscardFromHandTimesMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> filter;
            private readonly int max;
            private readonly string what;

            public DiscardFromHandTimesMon(string attack, Func<CardDef, bool> filter, int max, string what)
            {
                this.attack = attack;
                this.filter = filter;
                this.max = max;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var options = ctx.Me.Hand.Where(card => filter(card.Def)).ToList();
                int n = 0;
                if (options.Count > 0)
                {
                    int most = Math.Min(max, options.Count);
                    var picks = ctx.ChooseCards(options, 0, most, attack + ": discard up to " + most + " " + what + " from your hand (" + atk.Damage + " damage each)");
                    foreach (var pick in picks)
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, pick);
                        n++;
                    }
                }
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>"Move an Energy from this Pokémon to 1 of your Benched Pokémon" after the damage (Bubble Launch, Reflect Energy, Power Cyclone).</summary>
        internal sealed class MoveEnergyToBenchMon : Mon
        {
            private readonly string attack;

            public MoveEnergyToBenchMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, attack + ": move which Energy?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), attack + ": move " + energy.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                return true;
            }
        }

        /// <summary>"Put all Energy attached to this Pokémon into your hand" after the damage (Blizzard Loop).</summary>
        internal sealed class AllEnergyToHandMon : Mon
        {
            private readonly string attack;

            public AllEnergyToHandMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ReturnAllEnergy(ctx);
                return true;
            }

            internal static void ReturnAllEnergy(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return;
                foreach (var energy in me.Energy.ToList()) ctx.Engine.ReturnEnergyToHand(me, energy);
            }
        }

        /// <summary>"This attack's damage isn't affected by Weakness or Resistance" (Sonic Boom).</summary>
        internal sealed class NoWeaknessResistanceMon : Mon
        {
            private readonly string attack;

            public NoWeaknessResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        /// <summary>"Discard a [T] Energy from your opponent's Active Pokémon" after the damage (Grass Munch).</summary>
        internal sealed class DiscardDefendingTypeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public DiscardDefendingTypeMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.DiscardEnergyFrom(defending, 1, attack + ": discard a " + type + " Energy from " + defending.Name, card => CardsDRI1.ProvidesType(ctx, defending, card, type));
                return true;
            }
        }

        /// <summary>
        /// Counter: "If this Pokémon was damaged by an attack during your opponent's last turn, this attack does that much more
        /// damage."
        /// </summary>
        internal sealed class CounterMon : Mon
        {
            private readonly string attack;

            public CounterMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                int taken = me.AttackDamageTurn == ctx.OpponentsLastTurn ? me.AttackDamageThatTurn : 0;
                ctx.DamageDefending(atk.Damage + taken);
                return true;
            }
        }

        /// <summary>"During your next turn, this Pokémon's attacks do N more damage to your opponent's Active Pokémon" (Coil, No Reprieve).</summary>
        internal sealed class NextTurnBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public NextTurnBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                var boost = new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do " + bonus + " more damage during its next turn (" + attack + ")", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, bonus);
                boost.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(boost);
                return true;
            }
        }

        /// <summary>Coil: during your next turn, this Pokémon's attacks do 90 more damage to the opponent's Active Pokémon.</summary>
        private sealed class CoilFST : Mon
        {
            private static readonly Mon Coil = new NextTurnBonusMon("Coil", 90);

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Coil.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>Flock: search your deck for a Caterpie and put it onto your Bench.</summary>
        private sealed class FlockFST : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flock") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToBench(IsCaterpie, 1, "Flock: choose a Caterpie to put onto your Bench");
                return true;
            }

            private static bool IsCaterpie(CardDef d) { return d.Name == "Caterpie"; }
        }

        /// <summary>Tricolored Scales: when played from your hand to evolve, you may make the opponent's Active Pokémon Burned, Confused and Poisoned.</summary>
        private sealed class ButterfreeFST : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Tricolored Scales", target != null, "Use Tricolored Scales to make your opponent's Active Pokémon Burned, Confused and Poisoned?")) return;
                ctx.ApplyCondition(target, SpecialCondition.Burned);
                ctx.ApplyCondition(target, SpecialCondition.Confused);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }
        }

        /// <summary>Healing Circle: heal all damage from each of your Benched Pokémon; if any was healed, shuffle this Pokémon and all attached cards into your deck.</summary>
        private sealed class HealingCircleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Circle") return false;
                int healed = 0;
                foreach (var benched in ctx.MyBench())
                    if (benched.DamageCounters > 0) healed += ctx.Heal(benched, benched.DamageCounters * 10);
                if (healed > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Ninja Tornado: if this Pokémon moved from your Bench to the Active Spot this turn, it can be used for [G].</summary>
        private sealed class AccelgorFST : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Ninja Tornado" || ctx.Pokemon == null || !ctx.IsMyTurn || ctx.Pokemon.MovedToActiveOnTurn != ctx.Turn) return;
                cost.Clear();
                cost.Add(EnergyType.Grass);
            }
        }

        /// <summary>Bail Out: put up to 2 Pokémon from your discard pile into your hand (at least 1 when there are any: Compendium, 2021-11-11).</summary>
        private sealed class BailOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bail Out") return false;
                var mons = ctx.Me.Discard.Where(card => card.Def.IsPokemon).ToList();
                if (mons.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(mons, 1, Math.Min(2, mons.Count), "Bail Out: choose up to 2 Pokémon to put into your hand"));
                return true;
            }
        }

        /// <summary>Queen's Orders: you may discard any number of your Benched Pokémon; 40 more damage for each.</summary>
        private sealed class QueensOrdersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Queen's Orders") return false;
                var bench = ctx.MyBench();
                var picks = bench.Count == 0 ? new List<PokemonInPlay>()
                    : ctx.ChoosePokemonMany(bench, 0, bench.Count, "Queen's Orders: discard any number of your Benched Pokémon (40 more damage for each)");
                foreach (var benched in picks) ctx.Engine.DiscardPokemonFromPlay(benched);
                ctx.DamageDefending(atk.Damage + 40 * picks.Count);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Byway of the Nine-Tailed Fox: the Retreat Cost of each of your Pokémon with any Fire Energy attached is [C][C] less.</summary>
        private sealed class NinetalesFST : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !src.Engine.HasEnergyOfType(mon, EnergyType.Fire)) return cost;
                return Math.Max(0, cost - 2);
            }
        }

        /// <summary>Warm Up: search your deck for a Fire Energy card and attach it to 1 of your Pokémon.</summary>
        private sealed class WarmUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Warm Up") return false;
                CardsBRS1.SearchAttachAnyWay(ctx, IsFireEnergyCard, 1, AnyPokemon, "Warm Up", "Fire Energy card");
                return true;
            }

            private static bool IsFireEnergyCard(CardDef d) { return d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fire; }
            private static bool AnyPokemon(PokemonInPlay mon) { return true; }
        }

        /// <summary>Body Splash: flip 3 coins; discard an Energy from this Pokémon for each tails.</summary>
        private sealed class BodySplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Body Splash") return false;
                ctx.DamageDefending(atk.Damage);
                int tails = 3 - ctx.FlipCoins(3);
                var me = ctx.Pokemon;
                if (tails > 0 && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, tails, "Body Splash: discard " + tails + " Energy from " + me.Name);
                return true;
            }
        }

        /// <summary>Fiery Cheering: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class FieryCheeringMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fiery Cheering") return false;
                if (ctx.Me.Bench.Count > 0)
                    ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), 1, (card, target) => ctx.Me.Bench.Contains(target), "Fiery Cheering: attach a basic Energy card to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>Cursed Shimmer: while Active, the opponent can't play Pokémon Tool cards. Max Poltergeist: 70 for each Trainer card in their hand.</summary>
        private sealed class ChandelureVmax : Mon
        {
            private static readonly Mon MaxPoltergeist = new RevealCountMon("Max Poltergeist", IsTrainerCard, "Trainer");

            public override bool BlocksTrainer(EffectContext src, int player, CardInstance card)
            {
                return src.Pokemon != null && player != src.Player && src.PokemonIsActive && card.Def.IsTool;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return MaxPoltergeist.HandleAttack(ctx, atk); }
        }

        /// <summary>Exciting Flame: with at least 3 extra Energy attached, it also does 180 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class ExcitingFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Exciting Flame") return false;
                bool extra = ExtraEnergy(ctx, atk, 3);
                ctx.DamageDefending(atk.Damage);
                if (extra) ctx.DamageOneBenched(180);
                return true;
            }
        }

        /// <summary>G-Max Fireball: the opponent's Active Pokémon is now Burned; this Pokémon can't attack during your next turn.</summary>
        private sealed class GMaxFireballMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Fireball") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Soak in Water: attach a Water Energy card from your hand to this Pokémon.</summary>
        private sealed class SoakInWaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Soak in Water") return false;
                var water = ctx.Me.Hand.Where(card => IsWaterEnergyCard(card.Def)).ToList();
                if (water.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return true;
                var pick = water.Count == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Soak in Water: attach which Water Energy card to this Pokémon?")[0];
                ctx.Me.Hand.Remove(pick);
                ctx.Engine.AttachEnergyCard(pick, ctx.Pokemon, true);
                return true;
            }
        }

        /// <summary>
        /// Multishot Star: discard any amount of Water Energy from this Pokémon, then for each card discarded do 30 damage to 1
        /// of the opponent's Pokémon (the same one may be chosen again). Not affected by Weakness or Resistance.
        /// </summary>
        private sealed class MultishotStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Multishot Star") return false;
                var me = ctx.Pokemon;
                var water = me.Energy.Where(card => CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Water)).ToList();
                int shots = 0;
                if (water.Count > 0)
                {
                    var picks = ctx.ChooseCards(water, 0, water.Count, "Multishot Star: discard any amount of Water Energy (30 damage for each)");
                    foreach (var pick in picks)
                        if (ctx.Engine.DiscardEnergy(me, pick)) shots++;
                }
                for (int i = 0; i < shots; i++)
                {
                    var all = ctx.OpponentPokemon();
                    if (all.Count == 0) break;
                    var target = ctx.ChoosePokemon(all, "Multishot Star: 30 damage to which of your opponent's Pokémon? (" + (i + 1) + " of " + shots + ")");
                    ctx.DamageWith(target, 30, false, false, false);
                }
                return true;
            }
        }

        /// <summary>Splash Arch: put all Energy attached to this Pokémon into your hand, then 100 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SplashArchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Splash Arch") return false;
                AllEnergyToHandMon.ReturnAllEnergy(ctx);
                ctx.DamageOneBenched(100);
                return true;
            }
        }

        /// <summary>
        /// Rowdy: when you play this Pokémon from your hand to evolve during your turn, you must flip a coin: heads discards the
        /// top 5 cards of the opponent's deck, tails the top 5 of yours. Crunch: discard an Energy from the opponent's Active.
        /// </summary>
        private sealed class FeraligatrFST : Mon
        {
            private static readonly Mon Crunch = new CardsDRI2.CrunchMon("Crunch");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!ctx.IsMyTurn) return;
                Announce(ctx, "Rowdy");
                if (ctx.Flip()) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 5);
                else ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Crunch.HandleAttack(ctx, atk); }
        }

        /// <summary>Dive and Rescue: put up to 3 in any combination of Pokémon and Supporter cards from your discard pile into your hand.</summary>
        private sealed class DiveAndRescueMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dive and Rescue") return false;
                var options = ctx.Me.Discard.Where(card => card.Def.IsPokemon || card.Def.IsSupporter).ToList();
                if (options.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(options, 0, Math.Min(3, options.Count), "Dive and Rescue: choose up to 3 Pokémon and Supporter cards"));
                return true;
            }
        }

        /// <summary>Muddy Maker: once during your turn, attach a Water or Fighting Energy card from your hand to 1 of your Pokémon. Earthquake: 20 to each of your Benched Pokémon.</summary>
        private sealed class SwampertFST : Mon
        {
            private static readonly Mon Earthquake = new CardsBUS1.OwnBenchQuakeMon("Earthquake", 20);

            protected override string Activated { get { return "Muddy Maker"; } }

            private static bool Usable(CardDef d) { return d.IsBasicEnergy && (d.EnergyProvides == EnergyType.Water || d.EnergyProvides == EnergyType.Fighting); }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(card => Usable(card.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var options = ctx.Me.Hand.Where(card => Usable(card.Def)).ToList();
                var energy = options.Count == 1 ? options[0] : ctx.ChooseCards(options, 1, 1, "Muddy Maker: attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Muddy Maker: attach " + energy.Def.Name + " to which of your Pokémon?");
                ctx.Me.Hand.Remove(energy);
                ctx.Engine.AttachEnergyCard(energy, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Earthquake.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Single Strike Jammer: the opponent's Single Strike Pokémon's attacks cost [C] more (each Huntail adds [C]). A Pokémon
        /// protected from the effects of the opponent's Abilities (Fusion Strike Energy) isn't affected.
        /// </summary>
        private sealed class HuntailFST : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !mon.Def.IsSingleStrike) return;
                if (src.Engine.ShieldedFromOpponentsAbilities(mon)) return;
                cost.Add(EnergyType.Colorless);
            }
        }

        /// <summary>
        /// Rapid Strike Canceler: the opponent's Rapid Strike Pokémon in play have no Abilities. The engine keeps them for a
        /// Pokémon whose Tool or Special Energy prevents the effects of the opponent's Abilities (Fusion Strike Energy; your
        /// ruling). Draining Kiss: heal 30 from this Pokémon.
        /// </summary>
        private sealed class GorebyssFST : Mon
        {
            private static readonly Mon DrainingKiss = new CardsTWM1.HealSelfMon("Draining Kiss", 30);

            public override bool IsAbilityBlocker { get { return true; } }
            public override bool LockDependsOnSpot { get { return false; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && mon.Def.IsRapidStrike;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DrainingKiss.HandleAttack(ctx, atk); }
        }

        /// <summary>Daruma Headbutt: if this Pokémon has any damage counters on it, it can be used for [W]. Powder Snow: Asleep.</summary>
        private sealed class GalarianDarmanitanFST : Mon
        {
            private static readonly Mon PowderSnow = new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep);

            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Daruma Headbutt" || ctx.Pokemon == null || ctx.Pokemon.DamageCounters == 0) return;
                cost.Clear();
                cost.Add(EnergyType.Water);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PowderSnow.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Pitch a Pyukumuku: once during your turn, if this Pokémon is in your hand, reveal it and put it on the bottom of your
        /// deck, then draw a card. Only 1 Pitch a Pyukumuku each turn.
        /// </summary>
        private sealed class PyukumukuFST : Mon
        {
            private const string Pitch = "Pitch a Pyukumuku";

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Me.Hand.Contains(ctx.Card) && !ctx.NamedLimitUsedThisTurn(Pitch);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                ctx.MarkNamedLimit(Pitch);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { ctx.Card }, "revealed with " + Pitch);
                ctx.Me.Hand.Remove(ctx.Card);
                ctx.Me.Deck.Add(ctx.Card);
                ctx.Log(ctx.Card.Def.Name + " was put on the bottom of " + ctx.Engine.Their(ctx.Player) + " deck.");
                ctx.Draw(1);
            }
        }

        /// <summary>
        /// Double Gunner: you must discard a Water Energy card from your hand; once during your turn, 2 damage counters on each of
        /// 2 of the opponent's Benched Pokémon (on the only one if they have 1). G-Max Spiral: you may put an Energy attached to
        /// this Pokémon into your hand for 70 more damage.
        /// </summary>
        private sealed class InteleonVmax : Mon
        {
            protected override string Activated { get { return "Double Gunner"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Bench.Count > 0 && ctx.Me.Hand.Any(card => IsWaterEnergyCard(card.Def));
            }

            protected override void Ability(EffectContext ctx)
            {
                var water = ctx.Me.Hand.Where(card => IsWaterEnergyCard(card.Def)).ToList();
                var cost = water.Count == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Double Gunner: discard a Water Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);
                var bench = ctx.OpponentBench();
                int n = Math.Min(2, bench.Count);
                if (n == 0) return;
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, "Double Gunner: choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "G-Max Spiral") return false;
                // The Energy is chosen first but goes to the hand after the damage (Compendium: G-Max Spiral vs Skyscraper,
                // 2023-06-01), so it still counts while the damage is done.
                var me = ctx.Pokemon;
                CardInstance pick = null;
                if (me.Energy.Count > 0 && ctx.AskYesNo("G-Max Spiral: put an Energy attached to " + me.Name + " into your hand for 70 more damage?"))
                    pick = me.Energy.Count == 1 ? me.Energy[0] : ctx.ChooseCards(me.Energy.ToList(), 1, 1, "G-Max Spiral: put which Energy into your hand?")[0];
                ctx.DamageDefending(atk.Damage + (pick != null ? 70 : 0));
                if (pick != null && ctx.InPlay(me) && me.Energy.Contains(pick)) ctx.Engine.ReturnEnergyToHand(me, pick);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Spark Duo: 100 more damage if 1 of your Minun used an attack during your last turn.</summary>
        private sealed class SparkDuoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spark Duo") return false;
                var me = ctx.Me;
                bool minun = me.LastAttackTurn == ctx.MyLastTurn && me.LastAttackerDef != null && me.LastAttackerDef.Name == "Minun";
                ctx.DamageDefending(atk.Damage + (minun ? 100 : 0));
                return true;
            }
        }

        /// <summary>Surprise Short: discard all Pokémon Tools from all of the opponent's Pokémon.</summary>
        private sealed class SurpriseShortMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise Short") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon())
                    if (mon.Tools.Count > 0 && ctx.CanAffect(mon)) ctx.Engine.DiscardAllTools(mon);
                return true;
            }
        }

        /// <summary>Upper Shock: if this Pokémon evolved from Eelektrik during this turn, the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class UpperShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Upper Shock") return false;
                var me = ctx.Pokemon;
                bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Eelektrik";
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (fresh && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Electro Blaster: discard 2 Lightning Energy from this Pokémon, then 200 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class ElectroBlasterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Blaster") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Electro Blaster: discard 2 Lightning Energy from " + me.Name, card => CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Lightning));
                ctx.DamageAnyOpponent(200);
                return true;
            }
        }

        /// <summary>Punk Shock: you may make the opponent's Active Pokémon Paralyzed; if you do, this Pokémon also does 70 damage to itself.</summary>
        private sealed class PunkShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Punk Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                if (!ctx.AskYesNo("Punk Shock: make " + defending.Name + " Paralyzed? (" + ctx.Pokemon.Name + " also does 70 damage to itself)")) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(70);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Dig Up: when played from your hand to evolve, you may put up to 2 Pokémon Tool cards from your discard pile into your hand.</summary>
        private sealed class GranbullFST : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var tools = ctx.Me.Discard.Where(card => card.Def.IsTool).ToList();
                if (!TriggerOnce(ctx, "Dig Up", tools.Count > 0, "Use Dig Up to put up to 2 Pokémon Tool cards from your discard pile into your hand?")) return;
                ctx.DiscardToHand(ctx.ChooseCards(tools, 1, Math.Min(2, tools.Count), "Dig Up: choose up to 2 Pokémon Tool cards"));
            }
        }

        /// <summary>
        /// Force Regeneration: put a Basic Pokémon V from the opponent's discard pile onto their Bench; if you do, put damage
        /// counters on it until its remaining HP is 30.
        /// </summary>
        private sealed class ForceRegenerationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Force Regeneration") return false;
                if (ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return true;
                var options = ctx.Opp.Discard.Where(card => card.Def.IsBasicPokemon && card.Def.IsPokemonV && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, card.Def)).ToList();
                if (options.Count == 0) return true;
                var pick = options.Count == 1 ? options[0] : ctx.ChooseCards(options, 1, 1, "Force Regeneration: put which Basic Pokémon V onto your opponent's Bench?")[0];
                ctx.Opp.Discard.Remove(pick);
                var placed = ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
                if (placed == null) return true;
                int counters = (ctx.RemainingHp(placed) - 30) / 10;
                if (counters > 0) ctx.PlaceCounters(placed, counters);
                return true;
            }
        }

        /// <summary>Chomp Chomp Hold: during the opponent's next turn, the Defending Pokémon's attacks cost [C] more and its Retreat Cost is [C] more.</summary>
        private sealed class ChompChompHoldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chomp Chomp Hold") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost [C] more (Chomp Chomp Hold)", ctx.Player, defending.Uid, ctx.Turn + 1, 1));
                ctx.AddEffect(new CardsMEW1.RetreatUpEffect(defending.Name + "'s Retreat Cost is [C] more (Chomp Chomp Hold)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Sleep Inducer: switch in 1 of the opponent's Benched Pokémon; the new Active Pokémon is now Asleep.</summary>
        private sealed class SleepInducerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Inducer") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var switched = ctx.GustOpponent("Sleep Inducer: choose 1 of your opponent's Benched Pokémon to switch in");
                if (switched != null) ctx.ApplyCondition(switched, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Oppressing Sandstorm: if the opponent's Active Pokémon is a Basic Pokémon, it is Knocked Out. Used for Oppressing Sandstorm, Wild Crash (the attack name is passed in).</summary>
        internal sealed class OppressingSandstormMon : Mon
        {
            private readonly string attack;

            public OppressingSandstormMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Def.IsBasicPokemon && ctx.CanAffect(defending))
                    ctx.Engine.KnockOut(defending, attack);
                return true;
            }
        }
    }
}
