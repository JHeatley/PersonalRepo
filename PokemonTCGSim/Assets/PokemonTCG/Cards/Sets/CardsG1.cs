using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Gym Heroes (G1, v122): the owners' Pokémon (Brock's, Erika's, Lt. Surge's, Misty's, Sabrina's, Blaine's, Rocket's), the Gym
    /// Stadiums and the Trainers. Owner's Pokémon evolve only from the same owner's Pokémon (their names say so). 15-18 are the
    /// holo prints of Brock, Erika, Lt. Surge and Misty (Misty is the 30th Celebration Classic Collection card); the basic Energy
    /// are Mega Evolution Energy's. Your rulings (v122): Misty's Duel is real Rock-Paper-Scissors; Blaine's Quiz #1 works like
    /// Cedric Juniper; Crosscounter's damage takes the attacker's Weakness and Resistance; Restless Sleep's 20 doesn't.
    /// </summary>
    public static class CardsG1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("G1-1", new PhoenixFlameMon());
            r.RegisterByKey("G1-2", new RhydonBenchGuard());
            r.RegisterByKey("G1-3", new FairyPowerMon());
            r.RegisterByKey("G1-4", new ComboMon(new CardsFO.BlizzardMon(), new TakeAwayMon()));
            r.RegisterByKey("G1-5", new VileplumePollenDefense());
            r.RegisterByKey("G1-6", new CheckedCombo(new ChargeFromDiscardMon("Charge", 2), new DischargeMon()));
            r.RegisterByKey("G1-7", new ComboMon(new CoinsMon("Repeating Drill", 5), new NoRetreatMon("Clutch")));
            r.RegisterByKey("G1-8", new MagnetonEnergyCharge());
            r.RegisterByKey("G1-9", new KnockoutNeedleMon());
            r.RegisterByKey("G1-10", new TentacruelFlee());
            r.RegisterByKey("G1-11", new CounterMon("Crosscounter", 2, true, true));
            r.RegisterByKey("G1-12", new MoltresRebirth());
            r.RegisterByKey("G1-13", new ShadowImagesMon());
            r.RegisterByKey("G1-14", new ComboMon(new PainAmplifierMon(), new CallOfTheNightMon()));

            r.RegisterByKey("G1-20", new ChooseBenchedMon("Rock Slide", 3, 10, false));
            r.RegisterByKey("G1-21", new ComboMon(new ParalyzeFlipMon("Bind"), new TunnelingMon()));
            r.RegisterByKey("G1-22", new ComboMon(new CardsASC.PushDownMon("Horn Toss"), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("G1-23", new ComboMon(new NeedlesMon(), new CardsMEG.SandAttackMon("Sandstorm")));
            r.RegisterByKey("G1-24", new AlertMon());
            r.RegisterByKey("G1-25", new ComboMon(new SearchAttackMon("Moonwatching", d => d.IsEnergy && d.IsBasicEnergy, 1), new CoinsMon("Comet Slap", 3)));
            r.RegisterByKey("G1-26", new VictreebelFragranceTrap());
            r.RegisterByKey("G1-27", new ComboMon(new ChargeFromDiscardMon("Charge", 2), new ElectricCurrentMon()));
            r.RegisterByKey("G1-28", new CardsBS1.DiscardCostMon("Thunderbolt", null, -1, null, null));
            r.RegisterByKey("G1-29", new CloysterShellArmor());
            r.RegisterByKey("G1-30", new TailsNothingMon("Horn Hazard"));
            r.RegisterByKey("G1-31", new WaterRingMon());
            r.RegisterByKey("G1-32", new ComboMon(new CardsPAR2.FlipConditionMon("Mysterious Light", SpecialCondition.Asleep), new JellyfishPodMon()));
            r.RegisterByKey("G1-33", new SnorlaxRestlessSleep());
            r.RegisterByKey("G1-34", new ComboMon(new HealingPollenEachMon(), new SonicDistortionMon()));
            r.RegisterByKey("G1-35", new ComboMon(new CardsASC.PushDownMon("Shake"), new RecoilMon("Fire Tackle", 10)));
            r.RegisterByKey("G1-36", new ComboMon(new TailsNothingMon("Child's Punch"), new CardsWHT.FlipMoreMon("One-Two Punch", 1, 10)));
            r.RegisterByKey("G1-37", new ComboMon(new CardsWHT.FlipMoreMon("Firebreathing", 1, 10), new LavaBurstMon()));
            r.RegisterByKey("G1-38", new LuckyShotMon());
            r.RegisterByKey("G1-39", new SpiralDiveMon());
            r.RegisterByKey("G1-40", new CoinsMon("Rock Toss", 3));
            r.RegisterByKey("G1-41", new CoinsMon("Slam", 2));
            r.RegisterByKey("G1-42", new DratiniStrangeBarrier());
            r.RegisterByKey("G1-43", new ComboMon(new DeflectorMon(), new EggBombMon("Egg Bomb", 20)));
            r.RegisterByKey("G1-44", new ComboMon(new CardsBKT1.DeepSeaSwirlMon("Psychic Exchange", 5), new CardsWHT.FlipMoreMon("Stomp", 1, 10)));
            r.RegisterByKey("G1-45", new ComboMon(new HealSelfFlipMon("Healing Pollen", 4), new MagicPollenMon()));
            r.RegisterByKey("G1-46", new ComboMon(new CardsDRX.BothConditionMon("Dream Dance", SpecialCondition.Asleep), new CardsDRX.BothConditionMon("Vile Smell", SpecialCondition.Confused)));
            r.RegisterByKey("G1-47", new OddishPhotosynthesis());
            r.RegisterByKey("G1-48", new FlytrapMon());
            r.RegisterByKey("G1-49", new SleepPoisonMon());
            r.RegisterByKey("G1-50", new ComboMon(new FlipDiscardDefendingEnergyMon("Removal Pulse"), new CardsPAR2.FlipConditionMon("Confusion Pulse", SpecialCondition.Confused)));
            r.RegisterByKey("G1-51", new CardsBS1.SuperFangHalfMon());
            r.RegisterByKey("G1-53", new ComboMon(new FlipDiscardDefendingEnergyMon("Rapids"), new WaterPunchMon()));
            r.RegisterByKey("G1-54", new CallForFriendMon("Misty"));
            r.RegisterByKey("G1-55", new ChooseBenchedMon("Mud Splash", 1, 10, true));
            r.RegisterByKey("G1-56", new ComboMon(new CardsBS1.ExtraWaterMon("Water Gun"), new ParalyzeFlipMon("Bubblebeam")));
            r.RegisterByKey("G1-57", new CrystalBeamMon());
            r.RegisterByKey("G1-58", new NightSpiritsMon());
            r.RegisterByKey("G1-59", new ComboMon(new ConditionAttackMon("Good Night", SpecialCondition.Asleep), new GoodMorningMon()));
            r.RegisterByKey("G1-60", new ComboMon(new NaptimeMon(), new CantUseNextTurnMon("Screaming Headbutt")));
            r.RegisterByKey("G1-61", new CardsBS1.DiscardCostMon("Kindle", null, 1, KindleEffect, null));
            r.RegisterByKey("G1-62", new BlazeMon());
            r.RegisterByKey("G1-63", new DodgeMon("Agility", false));
            r.RegisterByKey("G1-64", new ComboMon(new CoinsMon("3-Pronged Tail", 3), new FullSpeedChargeMon()));
            r.RegisterByKey("G1-65", new VulpixNaturalHealing());
            r.RegisterByKey("G1-66", new CheckedCombo(new CallForFriendMon("Brock"), new CardsFLF1.NoResistanceMon("Hook Shot")));
            r.RegisterByKey("G1-67", new CardsBS1.LureMon("Taunt"));
            r.RegisterByKey("G1-68", new ComboMon(new ShuffleOwnDeckMon("Fidget"), new CardsBS1.KarateChopMon()));
            r.RegisterByKey("G1-69", new ParalyzeFlipMon("Bellow"));
            r.RegisterByKey("G1-70", new CardsBWP.BothHeadsOrNothingMon("Drill Tackle"));
            r.RegisterByKey("G1-71", new CardsMEW1.WithdrawMon("Defense Curl"));
            r.RegisterByKey("G1-72", new NoRetreatMon("Sand Pit"));
            r.RegisterByKey("G1-73", new CardsWHT.FlipMoreMon("Quick Attack", 1, 20));
            r.RegisterByKey("G1-74", new CardsPAR2.FlipConditionMon("Poison Fang", SpecialCondition.Poisoned));
            r.RegisterByKey("G1-75", new CardsPAR2.FlipConditionMon("Poison Vine", SpecialCondition.Poisoned));
            r.RegisterByKey("G1-76", new RecoilMon("Careless Tackle", 10));
            r.RegisterByKey("G1-77", new ComboMon(new EggsplosionMon(), new CardsBS1.EnergyCardBonusMon()));
            r.RegisterByKey("G1-78", new ComboMon(new BlotMon(), new SporadicSpongingMon()));
            r.RegisterByKey("G1-79", new Cards30CC.BenchHitMon("Stretch Vine", 20));
            r.RegisterByKey("G1-80", new ParalyzeFlipMon("Thundershock"));
            r.RegisterByKey("G1-81", new ComboMon(new ChargeFromDiscardMon("Charge", 1), new ParalyzeFlipMon("Lightning Tail")));
            r.RegisterByKey("G1-82", new FocusEnergyRattata());
            r.RegisterByKey("G1-83", new ComboMon(new CardsASC.PushDownMon("Whirlwind"), new TailsNothingMon("Razor Wind")));
            r.RegisterByKey("G1-84", new ComboMon(new CoinsMon("Spin Ball", 1), new CoinsMon("Double Spin", 2)));
            r.RegisterByKey("G1-85", new ComboMon(new CoinsMon("Fury Attack", 2), new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused)));
            r.RegisterByKey("G1-86", new CardsMEG.SandAttackMon("Smokescreen"));
            r.RegisterByKey("G1-87", new ComboMon(new HypnoticStareMon(), new CoinsMon("Tail Rap", 2)));
            r.RegisterByKey("G1-89", new CardsFO.ClampMon());
            r.RegisterByKey("G1-90", new CardsCRZ1.PiercingMon("Swift"));
            r.RegisterByKey("G1-91", new EnergyLoopMon());
            r.RegisterByKey("G1-92", new SuggestionMon());
            r.RegisterByKey("G1-93", new NoRetreatMon("Spook"));
            r.RegisterByKey("G1-94", new SleightOfHandMon());
            r.RegisterByKey("G1-95", new CardsRCL2.SelfConditionMon("Lazy Attack", SpecialCondition.Asleep));
            r.RegisterByKey("G1-96", new ComboMon(new ConditionAttackMon("Poison Antennae", SpecialCondition.Poisoned), new FlipDiscardDefendingEnergyMon("Removal Beam")));

            r.RegisterByKey("G1-98", new BrockTrainer());
            r.RegisterByKey("G1-19", new RocketsTrap());
            r.RegisterByKey("G1-97", new BlainesQuiz());
            r.RegisterByKey("G1-99", new Charity());
            r.RegisterByKey("G1-100", new ErikaTrainer());
            r.RegisterByKey("G1-101", new LtSurgeTrainer());
            r.RegisterByKey("G1-103", new CardScript()); // No Removal Gym: the Energy Removal cards check it (CardsBS2)
            r.RegisterByKey("G1-104", new RocketsTrainingGym());
            r.RegisterByKey("G1-105", new BlainesLastResort());
            r.RegisterByKey("G1-106", new OwnerSearch("Brock's Training Method", "Brock", 1, 0));
            r.RegisterByKey("G1-107", new CeladonCityGym());
            r.RegisterByKey("G1-108", new CeruleanCityGym());
            r.RegisterByKey("G1-109", new OwnerSearch("Erika's Maids", "Erika", 2, 2));
            r.RegisterByKey("G1-110", new ErikasPerfume());
            r.RegisterByKey("G1-111", new GoodManners());
            r.RegisterByKey("G1-112", new LtSurgesTreaty());
            r.RegisterByKey("G1-113", new MinionOfTeamRocket());
            r.RegisterByKey("G1-114", new MistysWrath());
            r.RegisterByKey("G1-115", new PewterCityGym());
            r.RegisterByKey("G1-116", new Recall());
            r.RegisterByKey("G1-117", new SabrinasEsp());
            r.RegisterByKey("G1-118", new SecretMission());
            r.RegisterByKey("G1-119", new TicklingMachine());
            r.RegisterByKey("G1-120", new VermilionCityGym());
            r.RegisterByKey("G1-121", new BlainesGamble());
            r.RegisterByKey("G1-122", new EnergyFlow());
            r.RegisterByKey("G1-123", new MistysDuel());
            r.RegisterByKey("G1-124", new NarrowGym());
            r.RegisterByKey("G1-125", new SabrinasGaze());
            r.RegisterByKey("G1-126", new TrashExchange());
        }

        // ================================================================== Shared

        private static bool PowerWorks(PokemonInPlay mon) { return CardsBS1.PowerWorks(mon); }

        private static bool HasOwner(CardDef def, string owner) { return def != null && def.Name.Contains(owner); }

        private static bool IsLightningCard(EffectContext ctx, PokemonInPlay mon, CardInstance card)
        {
            return card.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, mon, card, EnergyType.Lightning);
        }

        /// <summary>No Removal Gym (v122): with it in play, Energy Removal and Super Energy Removal need 2 other cards discarded from the hand.</summary>
        internal static bool NoRemovalGymInPlay(EffectContext ctx)
        {
            var stadium = ctx.State.Stadium;
            return stadium != null && stadium.Def.Name == "No Removal Gym" && !ctx.Engine.StadiumSwitchedOff();
        }

        internal static bool NoRemovalGymCostPayable(EffectContext ctx)
        {
            return !NoRemovalGymInPlay(ctx) || ctx.Me.Hand.Count(c => c != ctx.Card) >= 2;
        }

        /// <summary>Pays No Removal Gym's 2 cards (if it's in play). False if the hand doesn't have them.</summary>
        internal static bool PayNoRemovalGymCost(EffectContext ctx)
        {
            if (!NoRemovalGymInPlay(ctx)) return true;
            return DiscardOthers(ctx, 2, "No Removal Gym: discard 2 cards from your hand to play " + ctx.Card.Def.Name);
        }

        private static List<CardInstance> OtherCards(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card).ToList(); }

        private static bool DiscardOthers(EffectContext ctx, int count, string prompt)
        {
            var others = OtherCards(ctx);
            if (others.Count < count) return false;
            var picks = others.Count == count ? others : ctx.ChooseCards(others, count, count, prompt);
            foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
            return true;
        }

        /// <summary>A coin-flip-free counter-attack's damage with the receiver's Weakness and Resistance (Fire Wall, Crosscounter).</summary>
        private static int WithWR(GameEngine engine, PokemonInPlay source, PokemonInPlay target, int damage)
        {
            return CardsTR.WithWeaknessAndResistance(engine, source, target, damage);
        }

        /// <summary>ComboMon that also asks each part whether its attack can be used (Discharge, Call for Friend).</summary>
        internal sealed class CheckedCombo : Mon
        {
            private readonly Mon[] parts;

            public CheckedCombo(params Mon[] parts) { this.parts = parts; }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                foreach (var part in parts)
                    if (!part.CanUseAttack(ctx, attackIndex)) return false;
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                foreach (var part in parts)
                    if (part.HandleAttack(ctx, atk)) return true;
                return false;
            }
        }

        // ================================================================== Pokémon Powers

        /// <summary>
        /// Bench Guard: while Brock's Rhydon is on the Bench, whenever 1 of your other Benched Pokémon would be damaged, you may have
        /// 10 of that damage done to Rhydon instead.
        /// </summary>
        private sealed class RhydonBenchGuard : Mon
        {
            private static readonly Mon Lariat = new TailsNothingMon("Lariat");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var target = info.Target;
                if (me == null || target == null || target == me || damage < 10 || target.Owner != src.Player) return damage;
                if (!src.Me.Bench.Contains(me) || !src.Me.Bench.Contains(target)) return damage;
                if (!src.Engine.AskYesNo(src.Player, "Bench Guard: have 10 of the " + damage + " damage to " + target.Name + " done to " + me.Name + " instead?", src.Card.Def)) return damage;
                src.Engine.DealDamage(new DamageInfo { Attacker = info.Attacker, Target = me, ApplyWeakness = false, ApplyResistance = false }, 10);
                return damage - 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lariat.HandleAttack(ctx, atk); }
        }

        /// <summary>Pollen Defense: if an opponent's attack damages Erika's Vileplume while it's Active (even if Knocked Out), flip a coin; heads, the opponent's Active Pokémon is Confused. Works while Asleep, Confused or Paralyzed.</summary>
        private sealed class VileplumePollenDefense : Mon
        {
            private static readonly Mon Drain = new CardsJU.HalfDrainMon("Mega Drain");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || damage <= 0 || attacker == null || attacker.Owner == ctx.Player) return;
                Announce(ctx, "Pollen Defense");
                if (!ctx.Flip()) return;
                var opp = ctx.Opp.Active;
                if (opp != null) ctx.ApplyCondition(opp, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Charge: as often as you like during your turn, while Lt. Surge's Magneton is Active, move a Lightning Energy card from 1 of your other Pokémon to it. Mega Shock: tails, 20 to itself.</summary>
        private sealed class MagnetonEnergyCharge : Mon
        {
            private const string Charge = "Energy Charge";
            private static readonly Mon Shock = new CardsFST1.FlipRecoilMon("Mega Shock", 20);

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Any(c => IsLightningCard(ctx, m, c)));
            }

            private static bool CanMove(EffectContext ctx) { return ctx.State.IsActive(ctx.Pokemon) && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                if (abilityIndex < 0 || abilityIndex >= printed.Abilities.Count || printed.Abilities[abilityIndex].Name != Charge) return false;
                return ctx.IsMyTurn && PowerWorks(ctx.Pokemon) && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Charge + ": move a Lightning Energy card from which of your Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (from == null) return false;
                var cards = from.Energy.Where(c => IsLightningCard(ctx, from, c)).ToList();
                if (cards.Count == 0) return false;
                var card = ctx.ChooseCards(cards, 1, 1, Charge + ": move which Lightning Energy card?")[0];
                return ctx.Engine.MoveEnergy(from, ctx.Pokemon, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shock.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Flee: when an opponent's attack damages Misty's Tentacruel while it's Active, you may switch it with 1 of your Benched
        /// Pokémon (that prevents the attack's other effects on it: moving to the Bench ends them). Not if it was already Asleep,
        /// Confused or Paralyzed.
        /// </summary>
        private sealed class TentacruelFlee : Mon
        {
            private static readonly Mon Poison = new CardsBS1.FoulGasMon("Jellyfish Poison");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (!holderWasActive || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || me == null) return;
                if (!ctx.State.IsActive(me) || ctx.Me.Bench.Count == 0 || ctx.Engine.IsKnockedOut(me)) return;
                if (!CardsBS1.WorkedBeforeAttack(ctx, me)) return;
                if (!ctx.AskYesNo("Flee: switch " + me.Name + " with 1 of your Benched Pokémon?")) return;
                Announce(ctx, "Flee");
                var bench = ctx.Me.Bench.ToList();
                var to = ctx.ChoosePokemon(bench, "Flee: switch in which of your Benched Pokémon?");
                if (to != null) ctx.Engine.SwitchActive(ctx.Player, to);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Poison.HandleAttack(ctx, atk); }
        }

        /// <summary>Rebirth: when Rocket's Moltres is Knocked Out (not while Asleep, Confused or Paralyzed), you may put it into your hand after it's discarded. Fire Wall: counter-attack for 10.</summary>
        private sealed class MoltresRebirth : Mon
        {
            private static readonly Mon Wall = new CounterMon("Fire Wall", 0, false, true);

            public override void OnHolderKnockedOutRemoved(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var me = src.Pokemon;
                var card = src.Card;
                if (me == null || card == null || !PowerWorks(me) || !src.Me.Discard.Contains(card)) return;
                if (!src.AskYesNo("Rebirth: put " + card.Def.Name + " into your hand?")) return;
                Announce(src, "Rebirth");
                src.Me.Discard.Remove(card);
                src.Me.Hand.Add(card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wall.HandleAttack(ctx, atk); }
        }

        /// <summary>Shell Armor: damage from attacks to Misty's Cloyster is reduced by 10 (after W/R), unless it's Asleep, Confused or Paralyzed. Triple Cannon: 3 coins x20.</summary>
        private sealed class CloysterShellArmor : Mon
        {
            private static readonly Mon Cannon = new CoinsMon("Triple Cannon", 3);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target != src.Pokemon || !PowerWorks(src.Pokemon)) return damage;
                return Math.Max(0, damage - 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cannon.HandleAttack(ctx, atk); }
        }

        /// <summary>Restless Sleep: if Rocket's Snorlax is Asleep and an opponent's attack damages it (even if Knocked Out), 20 damage to the attacker (no W/R, your ruling). Collapse: Snorlax is now Asleep.</summary>
        private sealed class SnorlaxRestlessSleep : Mon
        {
            private static readonly Mon Collapse = new CardsRCL2.SelfConditionMon("Collapse", SpecialCondition.Asleep);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || damage <= 0 || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (!me.Has(SpecialCondition.Asleep)) return;
                Announce(ctx, "Restless Sleep");
                ctx.Engine.PlaceDamageCounters(attacker, 2, "Restless Sleep");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Collapse.HandleAttack(ctx, atk); }
        }

        /// <summary>Fragrance Trap: once during your turn, flip a coin; heads, switch 1 of the opponent's Benched Pokémon with their Active Pokémon.</summary>
        private sealed class VictreebelFragranceTrap : Mon
        {
            private const string Trap = "Fragrance Trap";

            protected override string Activated { get { return Trap; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip() && ctx.Opp.Bench.Count > 0) ctx.GustOpponent(Trap + ": choose 1 of your opponent's Benched Pokémon to switch in");
            }
        }

        /// <summary>Strange Barrier: an attack by a Basic Pokémon (even yours) that would do 20 or more damage to Erika's Dratini (after W/R) does 10 instead. Tail Strike: heads, 20 more.</summary>
        private sealed class DratiniStrangeBarrier : Mon
        {
            private static readonly Mon Strike = new CardsWHT.FlipMoreMon("Tail Strike", 1, 20);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Target != src.Pokemon || !PowerWorks(src.Pokemon)) return damage;
                if (!info.Attacker.Def.IsBasicPokemon || damage < 20) return damage;
                return 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Strike.HandleAttack(ctx, atk); }
        }

        /// <summary>Photosynthesis: all Energy attached to Erika's Oddish provides Grass Energy instead (works while Asleep, Confused or Paralyzed). Poisonpowder: heads, Poisoned.</summary>
        private sealed class OddishPhotosynthesis : Mon
        {
            private static readonly Mon Powder = new CardsPAR2.FlipConditionMon("Poisonpowder", SpecialCondition.Poisoned);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++)
                    if (units[i] != 0) units[i] = EnergyMask.Of(EnergyType.Grass);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Powder.HandleAttack(ctx, atk); }
        }

        /// <summary>Natural Healing: once during your turn, remove 1 damage counter from Blaine's Vulpix. Tail Fan: heads, Confused.</summary>
        private sealed class VulpixNaturalHealing : Mon
        {
            private const string Healing = "Natural Healing";
            private static readonly Mon Fan = new CardsPAR2.FlipConditionMon("Tail Fan", SpecialCondition.Confused);

            protected override string Activated { get { return Healing; } }

            protected override bool AbilityReady(EffectContext ctx) { return PowerWorks(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fan.HandleAttack(ctx, atk); }
        }

        // ================================================================== Attacks: effects during the opponent's next turn

        /// <summary>
        /// "If an attack does damage to this Pokémon during your opponent's next turn (even if it's Knocked Out), this Pokémon
        /// attacks the Defending Pokémon for ..." Crosscounter: heads, double the damage (multiplier 2, flip); Fire Wall: 10
        /// (multiplier 0 = a flat 10). Weakness and Resistance apply (your ruling for Crosscounter, printed on Fire Wall).
        /// </summary>
        internal sealed class CounterMon : Mon
        {
            private readonly string attack;
            private readonly int multiplier;
            private readonly bool flip;
            private readonly bool weakness;

            public CounterMon(string attack, int multiplier, bool flip, bool weakness)
            {
                this.attack = attack;
                this.multiplier = multiplier;
                this.flip = flip;
                this.weakness = weakness;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new CounterEffect(me.Name + " strikes back at attacks during the opponent's next turn (" + attack + ")", ctx.Player, me.Uid, ctx.Turn + 1, attack, multiplier, flip, weakness) { ActiveFromTurn = ctx.Turn + 1 });
                return true;
            }
        }

        private sealed class CounterEffect : ContinuousEffect
        {
            private readonly string attack;
            private readonly int multiplier;
            private readonly bool flip;
            private readonly bool weakness;

            public CounterEffect(string name, int owner, int targetUid, int expiresAfterTurn, string attack, int multiplier, bool flip, bool weakness)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.attack = attack;
                this.multiplier = multiplier;
                this.flip = flip;
                this.weakness = weakness;
            }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                var me = info.Target;
                if (me == null || me.Uid != TargetUid || damage <= 0 || info.Attacker == null || info.Attacker.Owner == Owner) return;
                var target = engine.State.Players[1 - Owner].Active;
                if (target == null) return;
                if (flip && !engine.FlipCoin(Owner, attack)) return;
                int back = multiplier > 0 ? damage * multiplier : 10;
                if (weakness) back = WithWR(engine, me, target, back);
                engine.Emit(GameEventType.Info, Owner, attack + ": " + me.Name + " does " + back + " damage to " + target.Name + ".", me.Top.Id, me.Uid);
                if (back >= 10) engine.PlaceDamageCounters(target, back / 10, attack);
            }
        }

        /// <summary>
        /// Shadow Images: whenever the opponent attacks Rocket's Scyther, they flip a coin; tails, that attack does no damage to it.
        /// Until Scyther takes damage, moves to the Bench or evolves.
        /// </summary>
        private sealed class ShadowImagesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Images") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new ShadowImagesEffect(me.Name + ": the opponent flips a coin when attacking it; tails, no damage (Shadow Images)", ctx.Player, me.Uid));
                return true;
            }
        }

        private sealed class ShadowImagesEffect : ContinuousEffect
        {
            public ShadowImagesEffect(string name, int owner, int targetUid) : base(name, owner, targetUid, int.MaxValue - 10, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                var me = info.Target;
                if (me == null || me.Uid != TargetUid || info.Attacker == null || info.Attacker.Owner == Owner || damage <= 0) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (engine.FlipCoin(info.Attacker.Owner, "Shadow Images")) return damage;
                engine.Emit(GameEventType.DamagePrevented, Owner, "Shadow Images: the attack does no damage to " + me.Name + ".", me.Top.Id, me.Uid);
                return 0;
            }

            /// <summary>"Until it takes damage": the first time it does, the effect ends.</summary>
            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target != null && info.Target.Uid == TargetUid && damage > 0) engine.State.Effects.Remove(this);
            }
        }

        /// <summary>Deflector: during the opponent's next turn, damage done to this Pokémon is halved (rounded down to the nearest 10).</summary>
        internal sealed class DeflectorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Deflector") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new HalveDamageEffect(me.Name + " takes half damage during the opponent's next turn (Deflector)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class HalveDamageEffect : ContinuousEffect
        {
            public HalveDamageEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || engine.State.TurnNumber != ExpiresAfterTurn) return damage;
                return damage / 2 / 10 * 10;
            }
        }

        /// <summary>Crystal Beam: heads, the opponent can't attach Energy cards to the Defending Pokémon during their next turn.</summary>
        private sealed class CrystalBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crystal Beam") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.Flip())
                    ctx.AddEffect(new NoEnergyAttachEffect(defending.Name + ": no Energy can be attached to it during its next turn (Crystal Beam)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoEnergyAttachEffect : ContinuousEffect
        {
            public NoEnergyAttachEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target) { return target != null && target.Uid == TargetUid; }
        }

        /// <summary>Suggestion: heads, the Defending Pokémon can't attack during the opponent's next turn.</summary>
        private sealed class SuggestionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Suggestion") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.Flip())
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Suggestion)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"The Defending Pokémon can't retreat during your opponent's next turn" (Clutch, Sand Pit, Spook).</summary>
        private sealed class NoRetreatMon : Mon
        {
            private readonly string attack;

            public NoRetreatMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>"This Pokémon can't use [attack] during your next turn" (Screaming Headbutt).</summary>
        private sealed class CantUseNextTurnMon : Mon
        {
            private readonly string attack;

            public CantUseNextTurnMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn(attack);
                return true;
            }
        }

        /// <summary>Focus Energy: during your next turn, Gnaw's base damage is doubled.</summary>
        private sealed class FocusEnergyRattata : Mon
        {
            private const string Focus = "Focus Energy turn";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Focus] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Gnaw") return false;
                int turn;
                bool doubled = me != null && me.Markers.TryGetValue(Focus, out turn) && turn == ctx.Turn;
                ctx.DamageDefending(doubled ? atk.Damage * 2 : atk.Damage);
                return true;
            }
        }

        // ================================================================== Attacks

        /// <summary>Phoenix Flame: tails, shuffle Blaine's Moltres and all cards attached to it into your deck after the damage.</summary>
        private sealed class PhoenixFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Phoenix Flame") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Fairy Power: heads, return any number of your Pokémon in play (with their cards) to your hand. Moon Impact: plain.</summary>
        private sealed class FairyPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fairy Power") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(mine, 0, mine.Count, "Fairy Power: return any number of your Pokémon (and their cards) to your hand");
                foreach (var mon in picks.OrderBy(m => ctx.State.IsActive(m) ? 1 : 0))
                    if (ctx.InPlay(mon)) ctx.Engine.ReturnPokemonToHand(mon);
                return true;
            }
        }

        /// <summary>Take Away: shuffle this Pokémon and its cards into your deck; then the opponent shuffles their Active Pokémon and its cards into theirs.</summary>
        private sealed class TakeAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Take Away") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                if (defending != null && ctx.InPlay(defending) && ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        /// <summary>Charge: attach up to N Lightning Energy cards from your discard pile to this Pokémon.</summary>
        private sealed class ChargeFromDiscardMon : Mon
        {
            private readonly string attack;
            private readonly int count;

            public ChargeFromDiscardMon(string attack, int count)
            {
                this.attack = attack;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var cards = ctx.Me.Discard.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Lightning).ToList();
                if (cards.Count == 0) return true;
                var picks = ctx.ChooseCards(cards, count == 1 ? 1 : 0, Math.Min(count, cards.Count), attack + ": attach " + (count == 1 ? "a" : "up to " + count) + " Lightning Energy card" + (count == 1 ? "" : "s") + " from your discard pile");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>Discharge: discard all Lightning Energy cards attached to this Pokémon (needs at least 1); a coin for each, 30 for each heads.</summary>
        internal sealed class DischargeMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                return ctx.Attack(attackIndex).Name != "Discharge" || (me != null && me.Energy.Any(c => IsLightningCard(ctx, me, c)));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Discharge") return false;
                var me = ctx.Pokemon;
                int n = 0;
                if (CardsBS1.CopyingWithMetronome) n = 1;
                else
                    foreach (var card in me.Energy.Where(c => IsLightningCard(ctx, me, c)).ToList())
                        if (ctx.Engine.DiscardEnergy(me, card)) n++;
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Knockout Needle: 2 coins; both heads, 60 more damage.</summary>
        private sealed class KnockoutNeedleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Knockout Needle") return false;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage + (heads == 2 ? 60 : 0));
                return true;
            }
        }

        /// <summary>Pain Amplifier: a damage counter on each of the opponent's Pokémon that already has any.</summary>
        private sealed class PainAmplifierMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pain Amplifier") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon(m => m.DamageCounters > 0))
                    if (ctx.InPlay(mon)) ctx.PlaceCounters(mon, 1);
                return true;
            }
        }

        /// <summary>Call of the Night: unless it Knocks Out the Defending Pokémon, 2 coins; both heads, the opponent shuffles it and its cards into their deck.</summary>
        private sealed class CallOfTheNightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call of the Night") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !TefPokemon.StillDefending(ctx, defending) || ctx.Engine.IsKnockedOut(defending)) return true;
                if (ctx.FlipCoins(2) == 2 && ctx.CanAffect(defending)) ctx.Engine.ShufflePokemonIntoDeck(defending);
                return true;
            }
        }

        /// <summary>"Choose up to N of your opponent's Benched Pokémon" and do X to each (Rock Slide); flip: 1 of them, heads only (Mud Splash).</summary>
        internal sealed class ChooseBenchedMon : Mon
        {
            private readonly string attack;
            private readonly int max;
            private readonly int amount;
            private readonly bool flip;

            public ChooseBenchedMon(string attack, int max, int amount, bool flip)
            {
                this.attack = attack;
                this.max = max;
                this.amount = amount;
                this.flip = flip;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count == 0) return true;
                int n = Math.Min(max, bench.Count);
                var picks = max == 1 ? new List<PokemonInPlay> { ctx.ChoosePokemon(bench, attack + ": choose 1 of your opponent's Benched Pokémon") }
                                     : ctx.ChoosePokemonMany(bench, 1, n, attack + ": choose up to " + n + " of your opponent's Benched Pokémon");
                if (flip && !ctx.Flip()) return true;
                foreach (var mon in picks)
                    if (mon != null && ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Tunneling: 20 to each of up to 2 of the opponent's Benched Pokémon; this Pokémon can't attack during your next turn.</summary>
        private sealed class TunnelingMon : Mon
        {
            private static readonly Mon Hit = new ChooseBenchedMon("Tunneling", 2, 20, false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (!Hit.HandleAttack(ctx, atk)) return false;
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Needles: heads, the Defending Pokémon is Paralyzed and Poisoned.</summary>
        private sealed class NeedlesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Needles") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Alert: draw a card, then switch this Pokémon with 1 of your Benched Pokémon (not without a Bench).</summary>
        private sealed class AlertMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Alert" || ctx.Me.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Alert") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(1);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Electric Current: move a Lightning Energy card from this Pokémon to 1 of your Benched Pokémon; with no Bench, discard it.</summary>
        internal sealed class ElectricCurrentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Current") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var cards = me.Energy.Where(c => IsLightningCard(ctx, me, c)).ToList();
                if (cards.Count == 0) return true;
                var card = ctx.ChooseCards(cards, 1, 1, "Electric Current: move which Lightning Energy card?")[0];
                var bench = ctx.Me.Bench.ToList();
                if (bench.Count == 0)
                {
                    ctx.Engine.DiscardEnergy(me, card);
                    return true;
                }
                var to = ctx.ChoosePokemon(bench, "Electric Current: attach it to which of your Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, card);
                return true;
            }
        }

        /// <summary>Water Ring: 10 to each Benched Pokémon (both players') that isn't a Water Pokémon.</summary>
        private sealed class WaterRingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Ring") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.Concat(ctx.Me.Bench).ToList())
                    if (ctx.InPlay(mon) && !ctx.Engine.IsType(mon, EnergyType.Water)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Jellyfish Pod: search your deck for any number of Tentacool, Tentacruel, Misty's Tentacool and Misty's Tentacruel and put them into your hand.</summary>
        private sealed class JellyfishPodMon : Mon
        {
            private static bool Wanted(CardDef d) { return d.IsPokemon && (d.Name == "Tentacool" || d.Name == "Tentacruel" || d.Name == "Misty's Tentacool" || d.Name == "Misty's Tentacruel"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jellyfish Pod") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(Wanted, 60, "Jellyfish Pod: choose any number of Tentacool and Tentacruel (Misty's included)");
                return true;
            }
        }

        /// <summary>Healing Pollen (Sabrina's Venomoth): 3 coins; for each heads, remove 1 damage counter from each of your Pokémon.</summary>
        private sealed class HealingPollenEachMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Pollen") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads == 0) return true;
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0))
                    ctx.Heal(mon, 10 * Math.Min(heads, mon.DamageCounters));
                return true;
            }
        }

        /// <summary>Sonic Distortion: 2 coins; if either is heads, the Defending Pokémon is Confused.</summary>
        private sealed class SonicDistortionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Distortion") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Lava Burst: discard the top 5 cards of your deck; 20 for each Fire Energy card discarded this way.</summary>
        private sealed class LavaBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lava Burst") return false;
                var top = ctx.Me.Deck.Take(5).ToList();
                int fire = top.Count(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire);
                if (top.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, top.Count);
                if (fire == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * fire);
                return true;
            }
        }

        /// <summary>Lucky Shot: choose 1 of the opponent's Benched Pokémon and flip a coin; heads, 30 to it. Not without an opposing Bench.</summary>
        private sealed class LuckyShotMon : Mon
        {
            private static readonly Mon Shot = new ChooseBenchedMon("Lucky Shot", 1, 30, true);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Lucky Shot" || ctx.Opp.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Shot.HandleAttack(ctx, atk); }
        }

        /// <summary>Spiral Dive: 10 to each of the opponent's Pokémon, no W/R.</summary>
        private sealed class SpiralDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Dive") return false;
                foreach (var mon in ctx.OpponentPokemon())
                    if (ctx.InPlay(mon)) ctx.DamageWith(mon, 10, false, false, false);
                return true;
            }
        }

        /// <summary>Egg Bomb: tails, no damage to the Defending Pokémon and this Pokémon does 20 damage to itself. Used for Egg Bomb, Risky Attack (the attack name and the numbers are passed in).</summary>
        internal sealed class EggBombMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public EggBombMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Flip()) ctx.DamageDefending(atk.Damage);
                else if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(amount);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, remove N damage counters from this Pokémon" (Healing Pollen, Erika's Gloom).</summary>
        private sealed class HealSelfFlipMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public HealSelfFlipMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Flip() && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 10 * Math.Min(counters, me.DamageCounters));
                return true;
            }
        }

        /// <summary>Magic Pollen: heads, the Defending Pokémon is Asleep, Confused, Paralyzed or Poisoned (your choice).</summary>
        private sealed class MagicPollenMon : Mon
        {
            private static readonly SpecialCondition[] Conditions = { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magic Pollen") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads || !TefPokemon.StillDefending(ctx, defending)) return true;
                int pick = ctx.ChooseOption("Magic Pollen: the Defending Pokémon is now...", Conditions.Select(c => c.ToString()).ToArray());
                if (pick < 0 || pick >= Conditions.Length) pick = 0;
                ctx.ApplyCondition(defending, Conditions[pick]);
                return true;
            }
        }

        /// <summary>Flytrap: before the damage, switch 1 of the opponent's Benched Pokémon (your choice) with their Active Pokémon; not without an opposing Bench. Drool: plain.</summary>
        private sealed class FlytrapMon : Mon
        {
            private static readonly Mon Trap = new CardsWHT.DragOffMon("Flytrap", 20);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Flytrap" || ctx.Opp.Bench.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Trap.HandleAttack(ctx, atk); }
        }

        /// <summary>Sleep Poison: the Defending Pokémon is Asleep and Poisoned. Vine Whip: plain.</summary>
        private sealed class SleepPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleep Poison") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon has any Energy cards attached, flip a coin. If heads, choose 1 of them and discard it" (Removal Pulse, Rapids, Removal Beam).</summary>
        private sealed class FlipDiscardDefendingEnergyMon : Mon
        {
            private readonly string attack;

            public FlipDiscardDefendingEnergyMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.Flip() || !ctx.CanAffect(defending)) return true;
                var cards = defending.Energy.ToList();
                var card = ctx.ChooseCards(cards, 1, 1, attack + ": discard which Energy card?")[0];
                ctx.Engine.DiscardEnergy(defending, card);
                return true;
            }
        }

        /// <summary>Water Punch: a coin for each Water Energy attached to this Pokémon; 10 more for each heads.</summary>
        private sealed class WaterPunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Punch") return false;
                int water = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                int heads = water > 0 ? ctx.FlipCoins(water) : 0;
                ctx.DamageDefending(atk.Damage + 10 * heads);
                return true;
            }
        }

        /// <summary>Call for Friend: heads, search your deck for a Basic Pokémon with the owner's name and put it onto your Bench. Not with a full Bench.</summary>
        private sealed class CallForFriendMon : Mon
        {
            private readonly string owner;

            public CallForFriendMon(string owner) { this.owner = owner; }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Call for Friend" || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Friend") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                ctx.SearchToBench(d => HasOwner(d, owner), 1, "Call for Friend: choose a Basic Pokémon with " + owner + " in its name");
                return true;
            }
        }

        /// <summary>Night Spirits: a coin for each Sabrina's Gastly, Haunter and Gengar you have in play; 30 for each heads.</summary>
        private sealed class NightSpiritsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Spirits") return false;
                int n = ctx.MyPokemon(m => m.Name == "Sabrina's Gastly" || m.Name == "Sabrina's Haunter" || m.Name == "Sabrina's Gengar").Count;
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Good Morning: if the Defending Pokémon is Asleep, it wakes up.</summary>
        private sealed class GoodMorningMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Good Morning") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Has(SpecialCondition.Asleep)) ctx.Engine.RemoveCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Naptime: heads, remove 3 damage counters from this Pokémon and it's now Asleep.</summary>
        private sealed class NaptimeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Naptime") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.Flip() || !ctx.InPlay(me)) return true;
                if (me.DamageCounters > 0) ctx.Heal(me, 10 * Math.Min(3, me.DamageCounters));
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Kindle (discard an Energy card from this Pokémon to use it): discard an Energy card from the Defending Pokémon.</summary>
        private static void KindleEffect(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(atk.Damage);
            if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
            var cards = defending.Energy.ToList();
            var card = ctx.ChooseCards(cards, 1, 1, "Kindle: discard which Energy card from the Defending Pokémon?")[0];
            ctx.Engine.DiscardEnergy(defending, card);
        }

        /// <summary>Blaze: 10 to each of the opponent's Benched Grass Pokémon.</summary>
        private sealed class BlazeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blaze") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.ToList())
                    if (ctx.InPlay(mon) && ctx.Engine.IsType(mon, EnergyType.Grass)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Full Speed Charge: 4 coins; 20 for each heads, and 20 to itself for each tails.</summary>
        private sealed class FullSpeedChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Full Speed Charge") return false;
                int heads = ctx.FlipCoins(4);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (heads < 4 && ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20 * (4 - heads));
                return true;
            }
        }

        /// <summary>Fidget: shuffle your deck.</summary>
        private sealed class ShuffleOwnDeckMon : Mon
        {
            private readonly string attack;

            public ShuffleOwnDeckMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Eggsplosion: a coin for each Energy attached to this Pokémon; 10 for each heads.</summary>
        private sealed class EggsplosionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eggsplosion") return false;
                int n = ctx.EnergyCount(ctx.Pokemon);
                int heads = n > 0 ? ctx.FlipCoins(n) : 0;
                if (heads == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Blot: after the damage, remove 1 damage counter from this Pokémon.</summary>
        private sealed class BlotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blot") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.DamageCounters > 0) ctx.Heal(ctx.Pokemon, 10);
                return true;
            }
        }

        /// <summary>Sporadic Sponging: if this Pokémon has damage counters, flip a coin; heads, remove 1.</summary>
        private sealed class SporadicSpongingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sporadic Sponging") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0 && ctx.Flip()) ctx.Heal(me, 10);
                return true;
            }
        }

        /// <summary>Hypnotic Stare: heads, Paralyzed; tails, Asleep.</summary>
        private sealed class HypnoticStareMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypnotic Stare") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, heads ? SpecialCondition.Paralyzed : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Energy Loop: return a Psychic Energy card attached to this Pokémon to your hand to use it.</summary>
        private sealed class EnergyLoopMon : Mon
        {
            private static List<CardInstance> Psychic(EffectContext ctx, PokemonInPlay mon)
            {
                return mon == null ? new List<CardInstance>() : mon.Energy.Where(c => c.Def.IsEnergy && CardsDRI1.ProvidesType(ctx, mon, c, EnergyType.Psychic)).ToList();
            }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Energy Loop" || Psychic(ctx, ctx.Pokemon).Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Loop") return false;
                var me = ctx.Pokemon;
                if (!CardsBS1.CopyingWithMetronome)
                {
                    var cards = Psychic(ctx, me);
                    if (cards.Count == 0) return Nothing(ctx);
                    var card = ctx.ChooseCards(cards, 1, 1, "Energy Loop: return which Psychic Energy card to your hand?")[0];
                    ctx.Engine.ReturnEnergyToHand(me, card);
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Sleight of Hand: put up to 3 cards from your hand on top of your deck, then search it for that many basic Energy cards and put them into your hand.</summary>
        private sealed class SleightOfHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sleight of Hand") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, Math.Min(3, hand.Count), "Sleight of Hand: put up to 3 cards from your hand on top of your deck");
                if (picks.Count == 0) return true;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Insert(0, card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + picks.Count + " card" + (picks.Count == 1 ? "" : "s") + " on top of " + ctx.Engine.Their(ctx.Player) + " deck (Sleight of Hand).");
                ctx.SearchToHand(d => d.IsEnergy && d.IsBasicEnergy, picks.Count, "Sleight of Hand: choose up to " + picks.Count + " basic Energy cards");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Brock: remove 1 damage counter from each of your Pokémon that has any.</summary>
        private sealed class BrockTrainer : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10);
            }
        }

        /// <summary>Erika: you may draw up to 3 cards, then your opponent may draw up to 3 cards.</summary>
        private sealed class ErikaTrainer : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    int max = Math.Min(3, ctx.State.Players[player].Deck.Count);
                    if (max <= 0) continue;
                    int n = ctx.Engine.AskNumber(player, 0, max, "Erika: draw how many cards (up to " + max + ")?", ctx.Card.Def);
                    if (n > 0) ctx.Engine.DrawCards(player, n);
                }
            }
        }

        /// <summary>Lt. Surge: put a Basic Pokémon from your hand into play as your Active Pokémon; your old Active Pokémon goes to the Bench. Not with a full Bench.</summary>
        private sealed class LtSurgeTrainer : CardScript
        {
            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def) && !ctx.Engine.PokemonPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Active != null && Basics(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var basics = Basics(ctx);
                if (basics.Count == 0 || ctx.BenchSpace <= 0) return;
                var card = ctx.ChooseCards(basics, 1, 1, "Lt. Surge: put which Basic Pokémon into play as your Active Pokémon?")[0];
                if (!ctx.Me.Hand.Remove(card)) return;
                var benched = ctx.Engine.PutOntoBench(ctx.Player, card, false);
                if (benched != null && ctx.Me.Bench.Contains(benched)) ctx.Engine.SwitchActive(ctx.Player, benched);
            }
        }

        /// <summary>The Rocket's Trap: heads, choose up to 3 cards at random from the opponent's hand; they shuffle them into their deck.</summary>
        private sealed class RocketsTrap : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip() || ctx.Opp.Hand.Count == 0) return;
                var hand = ctx.Opp.Hand.ToList();
                var picks = new List<CardInstance>();
                for (int i = 0; i < 3 && hand.Count > 0; i++)
                {
                    int at = ctx.State.Rng.Next(hand.Count);
                    picks.Add(hand[at]);
                    hand.RemoveAt(at);
                }
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffled " + picks.Count + " card" + (picks.Count == 1 ? "" : "s") + " from " + ctx.Engine.Their(ctx.Opponent) + " hand into " + ctx.Engine.Their(ctx.Opponent) + " deck (The Rocket's Trap).");
            }
        }

        /// <summary>
        /// Blaine's Quiz #1 (your ruling, v122: like Cedric Juniper): name a Basic Pokémon or Evolution card from your hand; the
        /// opponent guesses its exact length (Pokédex height) from the full list; right, they draw 2; wrong, you draw 2. The card
        /// stays in your hand.
        /// </summary>
        private sealed class BlainesQuiz : CardScript
        {
            private static List<CardInstance> Candidates(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c != ctx.Card && (c.Def.IsBasicPokemon || c.Def.IsEvolution) && PokedexHeights.HeightOf(c.Def) > 0).ToList();
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Candidates(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var candidates = Candidates(ctx);
                if (candidates.Count == 0) return;
                var pick = ctx.ChooseCards(candidates, 1, 1, "Blaine's Quiz #1: put which Pokémon face down? (Your opponent guesses its length.)")[0];
                int length = PokedexHeights.HeightOf(pick.Def);
                ctx.Log("Blaine's Quiz #1: " + ctx.Engine.PlayerName(ctx.Player) + " put a Pokémon face down and named it " + pick.Def.Name + ".");
                int guess = CardsLTR3.CedricJuniper.Guess(ctx, pick.Def.Name);
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { pick }, "revealed the Pokémon (Blaine's Quiz #1)");
                bool right = guess == length;
                ctx.Log("Blaine's Quiz #1: " + ctx.Engine.PlayerName(ctx.Opponent) + " guessed " + PokedexHeights.Label(guess) + "; " + pick.Def.Name + " is "
                        + PokedexHeights.Label(length) + (right ? ". Right!" : ". Wrong."));
                if (right) ctx.Engine.DrawCards(ctx.Opponent, 2);
                else ctx.Draw(2);
            }
        }

        /// <summary>
        /// Charity: attached to your Active Pokémon this turn; if it attacks and damages the Defending Pokémon, you may reduce that
        /// damage by any amount (multiples of 10). At the end of your turn, unless that Pokémon was Knocked Out, Charity goes back
        /// to your hand.
        /// </summary>
        private sealed class Charity : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                if (active == null) return;
                ctx.AddEffect(new CharityEffect("Charity: " + active.Name + " may do less damage this turn", ctx.Player, active.Uid, ctx.Turn, ctx.Card.Id));
            }
        }

        private sealed class CharityEffect : ContinuousEffect
        {
            private readonly int cardId;

            public CharityEffect(string name, int owner, int targetUid, int expiresAfterTurn, int cardId) : base(name, owner, targetUid, expiresAfterTurn, false)
            {
                this.cardId = cardId;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Attacker.Uid != TargetUid || !info.ToOpponentsActive || damage < 10) return damage;
                if (engine.CurrentSourceKind != EffectSourceKind.Attack || !engine.IsInteractive(Owner)) return damage;
                int less = engine.AskNumber(Owner, 0, damage / 10, "Charity: reduce the " + damage + " damage by how many 10s?");
                return Math.Max(0, damage - 10 * less);
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn || engine.State.FindPokemon(TargetUid) == null) return;
                var p = engine.State.Players[Owner];
                var card = p.Discard.FirstOrDefault(c => c.Id == cardId);
                if (card == null) return;
                p.Discard.Remove(card);
                p.Hand.Add(card);
                engine.Emit(GameEventType.Info, Owner, "Charity went back to " + engine.Their(Owner) + " hand.", card.Id);
            }
        }

        /// <summary>The Rocket's Training Gym: each player pays [C] more to retreat their Active Pokémon.</summary>
        private sealed class RocketsTrainingGym : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return cost + 1; }
        }

        /// <summary>Cerulean City Gym: Pokémon with Misty in their names cost [C] less to retreat.</summary>
        private sealed class CeruleanCityGym : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && HasOwner(mon.Def, "Misty") ? Math.Max(0, cost - 1) : cost;
            }
        }

        /// <summary>Pewter City Gym: Resistance doesn't apply to attacks by Pokémon with Brock in their names.</summary>
        private sealed class PewterCityGym : CardScript
        {
            public override bool IgnoresResistance(EffectContext src, DamageInfo info)
            {
                return info.Attacker != null && HasOwner(info.Attacker.Def, "Brock") && src.Engine.CurrentSourceKind == EffectSourceKind.Attack;
            }
        }

        /// <summary>Celadon City Gym: once during each player's turn, they may discard an Energy card from 1 of their Pokémon with Erika in its name; it recovers from Asleep, Confused, Paralyzed and Poisoned.</summary>
        private sealed class CeladonCityGym : CardScript
        {
            private const string Limit = "Celadon City Gym";

            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => HasOwner(m.Def, "Erika") && m.Energy.Count > 0); }

            public override bool CanUseStadium(EffectContext ctx) { return !ctx.NamedLimitUsedThisTurn(Limit) && Targets(ctx).Count > 0; }

            public override bool UseStadium(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return false;
                ctx.MarkNamedLimit(Limit);
                var mon = ctx.ChoosePokemon(targets, "Celadon City Gym: discard an Energy card from which of your Pokémon?");
                if (mon == null) return false;
                var cards = mon.Energy.ToList();
                var card = ctx.ChooseCards(cards, 1, 1, "Celadon City Gym: discard which Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(mon, card)) return false;
                foreach (var c in new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (mon.Has(c)) ctx.Engine.RemoveCondition(mon, c);
                return false; // v126 fix: true would end the turn (UseStadium's return value)
            }
        }

        /// <summary>
        /// Vermilion City Gym: whenever a player attacks with a Pokémon with Lt. Surge in its name, they may flip a coin. Heads: if the
        /// attack damages the Defending Pokémon (after W/R), 10 more. Tails: the attacker also does 10 damage to itself.
        /// </summary>
        private sealed class VermilionCityGym : CardScript
        {
            private const string TailsKey = "Vermilion City Gym tails";

            public override void OnAttackStarting(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (attacker == null || !HasOwner(attacker.Def, "Lt. Surge")) return;
                int player = attacker.Owner;
                if (!src.Engine.AskYesNo(player, "Vermilion City Gym: flip a coin? (Heads: 10 more damage. Tails: " + attacker.Name + " does 10 damage to itself.)", src.Card.Def)) return;
                if (src.Engine.FlipCoin(player, "Vermilion City Gym"))
                    src.Engine.AddEffect(new VermilionBonusEffect("Vermilion City Gym: 10 more damage", player, attacker.Uid, src.Turn));
                else src.State.Players[player].NamedLimitUsedOnTurn[TailsKey] = attacker.Uid;
            }

            public override void OnAttackFinished(EffectContext src, PokemonInPlay attacker, AttackDef attack)
            {
                if (attacker == null) return;
                var p = src.State.Players[attacker.Owner];
                int uid;
                if (!p.NamedLimitUsedOnTurn.TryGetValue(TailsKey, out uid)) return;
                p.NamedLimitUsedOnTurn.Remove(TailsKey);
                if (uid != attacker.Uid || src.State.FindPokemon(uid) != attacker) return;
                src.Log("Vermilion City Gym: " + attacker.Name + " does 10 damage to itself.");
                src.Engine.DealDamage(new DamageInfo { Attacker = attacker, Target = attacker, ApplyWeakness = false, ApplyResistance = false }, 10);
            }
        }

        private sealed class VermilionBonusEffect : ContinuousEffect
        {
            public VermilionBonusEffect(string name, int owner, int attackerUid, int expiresAfterTurn) : base(name, owner, attackerUid, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Attacker.Uid != TargetUid || !info.ToOpponentsActive || damage <= 0) return damage;
                return engine.CurrentSourceKind == EffectSourceKind.Attack ? damage + 10 : damage;
            }
        }

        /// <summary>
        /// Narrow Gym: no player can have more than 4 Benched Pokémon. When it's played, a player with 5 returns 1 of them and its
        /// cards to their hand (the opponent first).
        /// </summary>
        private sealed class NarrowGym : CardScript
        {
            public override int CapMaxBench(EffectContext src, int player) { return 4; }

            public override void OnStadiumPlayed(EffectContext ctx)
            {
                foreach (int player in new[] { ctx.Opponent, ctx.Player })
                {
                    var pctx = new EffectContext(ctx.Engine, player, ctx.Card, null);
                    for (int guard = 0; guard < 10 && pctx.Me.Bench.Count > 4; guard++)
                    {
                        var bench = pctx.Me.Bench.ToList();
                        var mon = pctx.ChoosePokemon(bench, "Narrow Gym: return which of your Benched Pokémon (and its cards) to your hand?");
                        if (mon == null) break;
                        ctx.Engine.ReturnPokemonToHand(mon);
                        if (pctx.Me.Bench.Contains(mon)) break;
                    }
                }
            }
        }

        /// <summary>Blaine's Last Resort: only with no other cards in your hand; draw 5 cards.</summary>
        private sealed class BlainesLastResort : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OtherCards(ctx).Count == 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(5); }
        }

        /// <summary>
        /// Brock's Training Method / Erika's Maids: search your deck for up to N Basic Pokémon or Evolution cards with the owner's name
        /// and put them into your hand (Erika's Maids: discard 2 other cards first).
        /// </summary>
        private sealed class OwnerSearch : CardScript
        {
            private readonly string card;
            private readonly string owner;
            private readonly int count;
            private readonly int discard;

            public OwnerSearch(string card, string owner, int count, int discard)
            {
                this.card = card;
                this.owner = owner;
                this.count = count;
                this.discard = discard;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && OtherCards(ctx).Count >= discard; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (discard > 0 && !DiscardOthers(ctx, discard, card + ": discard " + discard + " cards from your hand")) return;
                ctx.SearchToHand(d => (d.IsBasicPokemon || d.IsEvolution) && HasOwner(d, owner), count, card + ": choose " + (count == 1 ? "a" : "up to " + count) + " Basic Pokémon or Evolution card" + (count == 1 ? "" : "s") + " with " + owner + " in the name");
            }
        }

        /// <summary>Erika's Perfume: look at the opponent's hand; you may put any number of their Basic Pokémon onto their Bench.</summary>
        private sealed class ErikasPerfume : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return;
                ctx.Engine.Reveal(ctx.Opponent, hand, "hand (Erika's Perfume)");
                var opp = new EffectContext(ctx.Engine, ctx.Opponent, ctx.Card, null);
                var basics = hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                int room = opp.BenchSpace;
                if (basics.Count == 0 || room <= 0) return;
                var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Erika's Perfume: put any number of your opponent's Basic Pokémon onto their Bench");
                foreach (var card in picks)
                    if (opp.BenchSpace > 0 && ctx.Opp.Hand.Remove(card)) ctx.Engine.PutOntoBench(ctx.Opponent, card, false);
            }
        }

        /// <summary>Good Manners: only with no Basic Pokémon in your hand; show your hand, then search your deck for a Basic Pokémon.</summary>
        private sealed class GoodManners : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && !OtherCards(ctx).Any(c => c.Def.IsBasicPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = OtherCards(ctx);
                if (hand.Count > 0) ctx.Engine.Reveal(ctx.Player, hand, "hand (Good Manners)");
                ctx.SearchToHand(d => d.IsBasicPokemon, 1, "Good Manners: choose a Basic Pokémon");
            }
        }

        /// <summary>Lt. Surge's Treaty: the opponent chooses: each player takes 1 of their own Prize cards into their hand, or you draw a card.</summary>
        private sealed class LtSurgesTreaty : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var options = new List<DecisionOption>
                {
                    new DecisionOption { Label = "Each player takes 1 of their Prize cards", Index = 0 },
                    new DecisionOption { Label = ctx.Engine.PlayerName(ctx.Player) + " draws a card", Index = 1 }
                };
                int pick = ctx.Engine.AskOne(ctx.Opponent, DecisionKind.ChooseOption, "Lt. Surge's Treaty: choose 1", options, ctx.Card.Def);
                if (options[pick].Index == 1)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " chose: " + ctx.Engine.PlayerName(ctx.Player) + " draws a card (Lt. Surge's Treaty).");
                    ctx.Draw(1);
                    return;
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " chose: each player takes 1 of their Prize cards (Lt. Surge's Treaty).");
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                    if (!ctx.State.IsOver && ctx.State.Players[player].Prizes.Count > 0) ctx.Engine.TakePrizeCards(player, 1);
            }
        }

        /// <summary>Minion of Team Rocket: 2 coins; both heads, return 1 of the opponent's Benched Pokémon (your choice) and its cards to their hand; otherwise your turn ends.</summary>
        private sealed class MinionOfTeamRocket : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.FlipCoins(2) < 2)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (Minion of Team Rocket).");
                    ctx.Engine.RequestEndTurn();
                    return;
                }
                var bench = ctx.Opp.Bench.ToList();
                if (bench.Count == 0) return;
                var mon = ctx.ChoosePokemon(bench, "Minion of Team Rocket: return which of your opponent's Benched Pokémon to their hand?");
                if (mon != null) ctx.Engine.ReturnPokemonToHand(mon);
            }
        }

        /// <summary>Misty's Wrath: look at the top 7 cards of your deck; put 2 of them into your hand and discard the rest.</summary>
        private sealed class MistysWrath : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                if (top.Count == 0) return;
                var keep = top.Count <= 2 ? top : ctx.ChooseCards(top, 2, 2, "Misty's Wrath: put 2 of these cards into your hand (the rest are discarded)");
                ctx.DeckToHand(keep);
                var rest = top.Where(c => !keep.Contains(c)).ToList();
                foreach (var card in rest)
                    if (ctx.Me.Deck.Remove(card)) ctx.Me.Discard.Add(card);
                if (rest.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + rest.Count + " card" + (rest.Count == 1 ? "" : "s") + " (Misty's Wrath).");
            }
        }

        /// <summary>Recall: for your attack this turn, your Active Pokémon can also use the attacks of its Basic Pokémon card and the Evolution cards under it.</summary>
        private sealed class Recall : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new RecallEffect("Recall: your Active Pokémon can use the attacks of the cards under it this turn", ctx.Player, ctx.Turn));
            }
        }

        private sealed class RecallEffect : ContinuousEffect
        {
            public RecallEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(GameEngine engine, PokemonInPlay mon)
            {
                if (mon == null || mon.Owner != Owner || !engine.State.IsActive(mon) || mon.Stack.Count < 2) yield break;
                for (int k = 0; k < mon.Stack.Count - 1; k++)
                {
                    var def = mon.Stack[k].Def;
                    if (!def.IsPokemon) continue;
                    for (int i = 0; i < def.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(def, i);
                }
            }
        }

        /// <summary>Sabrina's ESP: on 1 of your Pokémon with Sabrina in its name this turn; if it attacks and flips coins, you may flip them all again once.</summary>
        private sealed class SabrinasEsp : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => HasOwner(m.Def, "Sabrina")); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var mon = ctx.ChoosePokemon(targets, "Sabrina's ESP: attach it to which of your Pokémon?");
                if (mon != null) ctx.AddEffect(new EspEffect("Sabrina's ESP", ctx.Player, mon.Uid, ctx.Turn));
            }
        }

        private sealed class EspEffect : ContinuousEffect
        {
            public EspEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, false) { }

            public override bool OffersAttackReflip(GameEngine engine, PokemonInPlay attacker) { return attacker != null && attacker.Uid == TargetUid; }
        }

        /// <summary>Secret Mission: look at the opponent's hand; then discard any number of the other cards in your hand and draw that many.</summary>
        private sealed class SecretMission : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Opp.Hand.Count > 0) ctx.Engine.Reveal(ctx.Opponent, ctx.Opp.Hand.ToList(), "hand (Secret Mission)");
                var others = OtherCards(ctx);
                if (others.Count == 0) return;
                var picks = ctx.ChooseCards(others, 0, others.Count, "Secret Mission: discard any number of cards from your hand to draw that many");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (picks.Count > 0) ctx.Draw(picks.Count);
            }
        }

        /// <summary>Tickling Machine: heads, the opponent sets their hand aside face down until the end of their next turn; tails, your turn ends.</summary>
        private sealed class TicklingMachine : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!ctx.Flip())
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Player) + "'s turn ends (Tickling Machine).");
                    ctx.Engine.RequestEndTurn();
                    return;
                }
                var hand = ctx.Opp.Hand.ToList();
                ctx.Opp.Hand.Clear();
                ctx.Opp.SetAsideHand.AddRange(hand);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " set " + ctx.Engine.Their(ctx.Opponent) + " hand of " + hand.Count + " card" + (hand.Count == 1 ? "" : "s") + " aside face down until the end of " + ctx.Engine.Their(ctx.Opponent) + " next turn (Tickling Machine).");
                ctx.AddEffect(new TicklingEffect("Tickling Machine: the hand set aside comes back at the end of the turn", ctx.Player, ctx.Opponent, ctx.Turn + 1));
            }
        }

        private sealed class TicklingEffect : ContinuousEffect
        {
            private readonly int victim;

            public TicklingEffect(string name, int owner, int victim, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.victim = victim;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var p = engine.State.Players[victim];
                if (p.SetAsideHand.Count == 0) return;
                int n = p.SetAsideHand.Count;
                p.Hand.AddRange(p.SetAsideHand);
                p.SetAsideHand.Clear();
                engine.Emit(GameEventType.Info, victim, engine.PlayerName(victim) + " put the " + n + " set-aside card" + (n == 1 ? "" : "s") + " back into " + engine.Their(victim) + " hand (Tickling Machine).");
            }
        }

        /// <summary>Blaine's Gamble: discard any number of the other cards in your hand, then flip a coin; heads, draw twice that many.</summary>
        private sealed class BlainesGamble : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var others = OtherCards(ctx);
                var picks = others.Count == 0 ? new List<CardInstance>() : ctx.ChooseCards(others, 0, others.Count, "Blaine's Gamble: discard any number of cards (heads: draw twice that many)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (ctx.Flip() && picks.Count > 0) ctx.Draw(2 * picks.Count);
            }
        }

        /// <summary>Energy Flow: return any number of Energy cards attached to your Pokémon to your hand.</summary>
        private sealed class EnergyFlow : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => m.Energy.Count > 0))
                {
                    var cards = mon.Energy.ToList();
                    var picks = ctx.ChooseCards(cards, 0, cards.Count, "Energy Flow: return any number of Energy cards from " + mon.Name + " to your hand");
                    foreach (var card in picks) ctx.Engine.ReturnEnergyToHand(mon, card);
                }
            }
        }

        /// <summary>
        /// Misty's Duel (your ruling, v122): real Rock-Paper-Scissors; each player picks secretly (a bot at random), a tie is played
        /// again; the winner shuffles their hand into their deck and draws 5 cards.
        /// </summary>
        private sealed class MistysDuel : CardScript
        {
            private static readonly string[] Signs = { "Rock", "Paper", "Scissors" };

            private static int Pick(EffectContext ctx, int player)
            {
                if (!ctx.Engine.IsInteractive(player)) return ctx.State.Rng.Next(3);
                var options = Signs.Select((s, i) => new DecisionOption { Label = s, Index = i }).ToList();
                return options[ctx.Engine.AskOne(player, DecisionKind.ChooseOption, "Misty's Duel: Rock, Paper or Scissors?", options, ctx.Card.Def, false)].Index;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int winner = -1;
                for (int round = 0; round < 50 && winner < 0; round++)
                {
                    int mine = Pick(ctx, ctx.Player);
                    int theirs = Pick(ctx, ctx.Opponent);
                    ctx.Log("Misty's Duel: " + ctx.Engine.PlayerName(ctx.Player) + " shows " + Signs[mine] + ", " + ctx.Engine.PlayerName(ctx.Opponent) + " shows " + Signs[theirs] + ".");
                    if (mine == theirs) continue;
                    winner = (mine - theirs + 3) % 3 == 1 ? ctx.Player : ctx.Opponent;
                }
                if (winner < 0) return;
                ctx.Log(ctx.Engine.PlayerName(winner) + " wins Misty's Duel: hand shuffled into the deck, 5 new cards.");
                ctx.Engine.ShuffleHandIntoDeck(winner);
                ctx.Engine.DrawCards(winner, 5);
            }
        }

        /// <summary>Sabrina's Gaze: each player shuffles their hand into their deck and draws that many cards.</summary>
        private sealed class SabrinasGaze : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int mine = ctx.Me.Hand.Count(c => c != ctx.Card);
                int theirs = ctx.Opp.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                if (mine > 0) ctx.Draw(mine);
                if (theirs > 0) ctx.Engine.DrawCards(ctx.Opponent, theirs);
            }
        }

        /// <summary>Trash Exchange: shuffle your discard pile into your deck, then discard that many cards from the top of your deck.</summary>
        private sealed class TrashExchange : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                var discard = ctx.Me.Discard.Where(c => c != ctx.Card).ToList();
                if (discard.Count == 0) return;
                var moved = ctx.DiscardToDeck(discard);
                ctx.ShuffleMyDeck();
                if (moved.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, moved.Count);
            }
        }
    }
}
