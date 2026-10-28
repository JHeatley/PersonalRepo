using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Mega Evolution (MEG): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Ivysaur, Nuzleaf, Nincada, Vulpix, Litleo, Sizzlipede, Snover, Clauncher, Magnemite,
    /// Gimmighoul, Sandshrew, Croagunk, Nacli, Nickit, Tinkatink, Stufful.
    /// </summary>
    public static class CardsMEG
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("MEG-1", new NoRetreatMon("Bind Down"));
            r.RegisterByKey("MEG-3", new MegaVenusaurExMEG());
            r.RegisterByKey("MEG-4", new ChargeSelfMon("Jam-Packed", EnergyType.Grass, 1));
            r.RegisterByKey("MEG-5", new ExeggutorMEG());
            r.RegisterByKey("MEG-6", new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned));
            r.RegisterByKey("MEG-7", new TangrowthMEG());
            r.RegisterByKey("MEG-11", new ShuckleMEG());
            r.RegisterByKey("MEG-13", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("MEG-15", new ShiftryMEG());
            r.RegisterByKey("MEG-17", new NinjaskMEG());
            r.RegisterByKey("MEG-18", new DhelmiseMEG());
            r.RegisterByKey("MEG-20", new NinetalesMEG());
            r.RegisterByKey("MEG-21", new CallForFamilyMon());
            r.RegisterByKey("MEG-22", new MegaCameruptExMEG());
            r.RegisterByKey("MEG-24", new PyroarMEG());
            r.RegisterByKey("MEG-25", new VolcanionMEG());
            r.RegisterByKey("MEG-26", new TailsNothingMon("Wild Kick"));
            r.RegisterByKey("MEG-27", new SnipeMon("Jumping Kick", 40));
            r.RegisterByKey("MEG-28", new CinderaceMEG());
            r.RegisterByKey("MEG-30", new CentiskorchMEG());
            r.RegisterByKey("MEG-31", new ChiYuMEG());
            r.RegisterByKey("MEG-32", new CallForFamilyMon());
            r.RegisterByKey("MEG-33", new RecoilMon("Take Down", 10));
            r.RegisterByKey("MEG-34", new KyogreMEG());
            r.RegisterByKey("MEG-36", new MegaAbomasnowExMEG());
            r.RegisterByKey("MEG-38", new ClawitzerMEG());
            r.RegisterByKey("MEG-39", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("MEG-40", new CoinsMon("Double Stab", 2));
            r.RegisterByKey("MEG-41", new InteleonMEG());
            r.RegisterByKey("MEG-42", new DodgeMon("Hide", false));
            r.RegisterByKey("MEG-43", new FrosmothMEG());
            r.RegisterByKey("MEG-44", new ParalyzeFlipMon("Freezing Headbutt"));
            r.RegisterByKey("MEG-46", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("MEG-47", new MagnezoneMEG());
            r.RegisterByKey("MEG-48", new EnergyInPlayBonusMon("Electro Fall", EnergyType.Lightning, 4, 90));
            r.RegisterByKey("MEG-49", new RecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("MEG-50", new MegaManectricExMEG());
            r.RegisterByKey("MEG-51", new PachirisuMEG());
            r.RegisterByKey("MEG-52", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("MEG-53", new ConditionAttackMon("Dazzle Blast", SpecialCondition.Confused));
            r.RegisterByKey("MEG-57", new DefendingEnergyBonusMon("Psychic", 30));
            r.RegisterByKey("MEG-58", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("MEG-59", new SearchAttackMon("Call Sign", d => d.IsPokemon, 3));
            r.RegisterByKey("MEG-60", new MegaGardevoirExMEG());
            r.RegisterByKey("MEG-61", new ShedinjaMEG());
            r.RegisterByKey("MEG-62", new CoinsMon("Triple Spin", 3));
            r.RegisterByKey("MEG-63", new GrumpigMEG());
            r.RegisterByKey("MEG-64", new XerneasMEG());
            r.RegisterByKey("MEG-65", new RecoilMon("Take Down", 10));
            r.RegisterByKey("MEG-66", new HoundstoneMEG());
            r.RegisterByKey("MEG-69", new SandAttackMon("Sand Attack"));
            r.RegisterByKey("MEG-70", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("MEG-71", new FlipUntilTailsBonusMon("Pow-Pow Punching", 30));
            r.RegisterByKey("MEG-79", new RecoilMon("Reckless Charge", 20));
            r.RegisterByKey("MEG-80", new CardsLOT1.ButterflyEdgeMon("Shadowy Side Kick"));
            r.RegisterByKey("MEG-81", new StonjournerMEG());
            r.RegisterByKey("MEG-83", new CardsFLF1.NoResistanceMon("Rock Hurl"));
            r.RegisterByKey("MEG-84", new GarganaclMEG());
            r.RegisterByKey("MEG-85", new CrawdauntMEG());
            r.RegisterByKey("MEG-86", new MegaAbsolExMEG());
            r.RegisterByKey("MEG-87", new SpiritombMEG());
            r.RegisterByKey("MEG-90", new DrawUntilSixMon("Greedy Hunt"));
            r.RegisterByKey("MEG-91", new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned));
            r.RegisterByKey("MEG-92", new CardsSVI1.MiraclePowderMon("Miraculous Paint"));
            r.RegisterByKey("MEG-93", new SteelixMEG());
            r.RegisterByKey("MEG-94", new MegaMawileExMEG());
            r.RegisterByKey("MEG-95", new DialgaMEG());
            r.RegisterByKey("MEG-97", new TinkatuffMEG());
            r.RegisterByKey("MEG-98", new CardsCRZ1.MidnightFangMon("Windup Swing", 60));
            r.RegisterByKey("MEG-99", new GholdengoMEG());
            r.RegisterByKey("MEG-100", new MegaLatiasExMEG());
            r.RegisterByKey("MEG-101", new LatiosMEG());
            r.RegisterByKey("MEG-102", new CardsBLK.PeckOffMon("Pluck"));
            r.RegisterByKey("MEG-103", new CoinsMon("Repeating Drill", 5));
            r.RegisterByKey("MEG-105", new DelibirdMEG());
            r.RegisterByKey("MEG-106", new MiltankMEG());
            r.RegisterByKey("MEG-107", new WeakenDefendingMon("Charm", 20));
            r.RegisterByKey("MEG-108", new SnipeBenchMon("Dashing Kick", 50));
            r.RegisterByKey("MEG-109", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("MEG-110", new GumshoosMEG());
            r.RegisterByKey("MEG-112", new CardsPRC1.KnockoutNeedleMon("Hyper Lariat", 100));

            r.RegisterByName("Acerola's Mischief", new AcerolasMischief());
            r.RegisterByName("Iron Defender", new IronDefender());
            r.RegisterByName("Lt. Surge's Bargain", new LtSurgesBargain());
            r.RegisterByName("Mystery Garden", new MysteryGarden());
            r.RegisterByName("Repel", new Repel());
            r.RegisterByName("Surfing Beach", new SurfingBeach());
        }

        // ================================================================== Shared helpers (also used by PFL / MEP)

        /// <summary>"Search your deck for up to N Basic [T] Energy cards and attach them to this Pokémon."</summary>
        public sealed class ChargeSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;

            public ChargeSelfMon(string attack, EnergyType type, int count)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ChargeSelf(ctx, type, count, attack);
                return true;
            }

            public static void ChargeSelf(EffectContext ctx, EnergyType type, int count, string attack)
            {
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(ctx.Pokemon)) return;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, type), count, attack + ": search your deck for Basic " + type + " Energy");
                foreach (var e in found) ctx.Engine.AttachEnergyCard(e, ctx.Pokemon);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>"This attack does N damage to 1 of your opponent's Pokémon."</summary>
        public sealed class SnipeMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SnipeMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 1 of your opponent's Benched Pokémon."</summary>
        public sealed class SnipeBenchMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SnipeBenchMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.DamageOneBenched(amount);
                return true;
            }
        }

        /// <summary>"This attack does N more damage for each [T] Energy attached to this Pokémon."</summary>
        public sealed class EnergyBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int bonus;

            public EnergyBonusMon(string attack, EnergyType type, int bonus)
            {
                this.attack = attack;
                this.type = type;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + bonus * ctx.Engine.EnergyCountOfType(ctx.Pokemon, type));
                return true;
            }
        }

        /// <summary>"If you have at least N [T] Energy in play, this attack does X more damage."</summary>
        public sealed class EnergyInPlayBonusMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int needed;
            private readonly int bonus;

            public EnergyInPlayBonusMon(string attack, EnergyType type, int needed, int bonus)
            {
                this.attack = attack;
                this.type = type;
                this.needed = needed;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int inPlay = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, type));
                ctx.DamageDefending(atk.Damage + (inPlay >= needed ? bonus : 0));
                return true;
            }
        }

        /// <summary>"This attack does N more damage for each Energy attached to your opponent's Active Pokémon."</summary>
        public sealed class DefendingEnergyBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public DefendingEnergyBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending == null ? 0 : bonus * ctx.EnergyCount(ctx.Defending)));
                return true;
            }
        }

        /// <summary>"Flip a coin until you get tails. This attack does N more damage for each heads."</summary>
        public sealed class FlipUntilTailsBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public FlipUntilTailsBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + bonus * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>"You may draw cards until you have 6 cards in your hand."</summary>
        public sealed class DrawUntilSixMon : Mon
        {
            private readonly string attack;

            public DrawUntilSixMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                DrawUntilSix(ctx, attack);
                return true;
            }

            public static void DrawUntilSix(EffectContext ctx, string attack)
            {
                if (ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo(attack + ": draw cards until you have 6 cards in your hand?"))
                    ctx.DrawUntil(6);
            }
        }

        /// <summary>"During your opponent's next turn, attacks used by the Defending Pokémon do N less damage (before Weakness and Resistance)."</summary>
        public sealed class WeakenDefendingMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public WeakenDefendingMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                WeakenDefending(ctx, atk, amount);
                return true;
            }

            public static void WeakenDefending(EffectContext ctx, AttackDef atk, int amount)
            {
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return;
                ctx.AddEffect(new WeakenAttackerEffect(defending.Name + "'s attacks do " + amount + " less damage during its next turn (" + atk.Name + ")",
                    ctx.Player, defending.Uid, ctx.Turn + 1, amount));
            }
        }

        /// <summary>"During your opponent's next turn, if the Defending Pokémon tries to attack, your opponent flips a coin. If tails, that attack doesn't happen."</summary>
        public sealed class SandAttackEffect : ContinuousEffect
        {
            public SandAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return attacker.Uid == TargetUid; }
        }

        public sealed class SandAttackMon : Mon
        {
            private readonly string attack;

            public SandAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new SandAttackEffect(defending.Name + " must flip a coin to attack during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks from Basic Pokémon."</summary>
        public sealed class BasicShieldEffect : ContinuousEffect
        {
            public BasicShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || !info.AgainstOpponent) return damage;
                return info.Attacker.Def.IsBasicPokemon ? 0 : damage;
            }
        }

        public static void ShieldFromBasics(EffectContext ctx, string attack)
        {
            if (!ctx.InPlay(ctx.Pokemon)) return;
            ctx.AddEffect(new BasicShieldEffect(ctx.Pokemon.Name + " takes no damage from Basic Pokémon's attacks during the opponent's next turn (" + attack + ")",
                ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
        }

        /// <summary>The player can't play Stadium cards from their hand (Chi-Yu's Scorching Earth).</summary>
        public sealed class StadiumLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public StadiumLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card.Def.IsStadium;
            }
        }

        /// <summary>Whenever the opponent attaches an Energy card from their hand to the Defending Pokémon, put N damage counters on it.</summary>
        public sealed class EnergyTrapEffect : ContinuousEffect
        {
            private readonly int counters;

            public EnergyTrapEffect(string name, int owner, int targetUid, int expiresAfterTurn, int counters)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.counters = counters;
            }

            public override void OnEnergyAttachedFromHand(GameEngine engine, CardInstance energy, PokemonInPlay target)
            {
                if (target.Uid != TargetUid || engine.State.CurrentPlayer != target.Owner) return;
                engine.PlaceDamageCounters(target, counters, Name);
            }
        }

        /// <summary>Moves Basic Energy of a type between your own Pokémon (Solar Transfer).</summary>
        public static bool CanMoveBasicEnergy(EffectContext ctx, EnergyType type)
        {
            return ctx.Me.InPlay().Count() > 1 && ctx.MyPokemon().Any(m => m.Energy.Any(e => CardsSSP1.IsBasicOf(e.Def, type)));
        }

        /// <summary>The person playing moves as many as they like, then presses Done (RepeatableAbility); bots move one per use.</summary>
        public static void MoveBasicEnergyRepeatedly(EffectContext ctx, EnergyType type, string ability)
        {
            if (!RepeatableAbility.ForPerson(ctx))
            {
                MoveBasicEnergy(ctx, type, ability);
                return;
            }
            RepeatableAbility.Repeat(ctx, () => CanMoveBasicEnergy(ctx, type), offerDone =>
            {
                var holders = ctx.MyPokemon(m => m.Energy.Any(e => CardsSSP1.IsBasicOf(e.Def, type)));
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, ability + ": move a Basic " + type + " Energy from which Pokémon? Press Done when finished.", offerDone);
                if (from == null) return false;
                var energy = from.Energy.First(e => CardsSSP1.IsBasicOf(e.Def, type));
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != from), ability + ": move it from " + from.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                ctx.Engine.MoveEnergy(from, to, energy);
                return true;
            });
        }

        public static void MoveBasicEnergy(EffectContext ctx, EnergyType type, string ability)
        {
            var holders = ctx.MyPokemon(m => m.Energy.Any(e => CardsSSP1.IsBasicOf(e.Def, type)));
            var from = ctx.ChoosePokemon(holders, ability + ": move a Basic " + type + " Energy from which Pokémon?");
            if (from == null) return;
            var energy = from.Energy.First(e => CardsSSP1.IsBasicOf(e.Def, type));
            var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), ability + ": move it to which Pokémon?");
            if (to != null) ctx.Engine.MoveEnergy(from, to, energy);
        }

        /// <summary>Heals N damage from 1 of your damaged Pokémon (the player chooses). Returns false when nothing could be healed.</summary>
        public static bool HealOneOfMine(EffectContext ctx, int amount, string source, Func<PokemonInPlay, bool> filter = null)
        {
            var hurt = ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m) && (filter == null || filter(m)));
            if (hurt.Count == 0) return false;
            var target = ctx.ChoosePokemon(hurt, source + ": heal which Pokémon?");
            if (target == null) return false;
            ctx.Heal(target, amount);
            return true;
        }

        public static bool AnyHurt(EffectContext ctx, Func<PokemonInPlay, bool> filter = null)
        {
            return ctx.MyPokemon(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m) && (filter == null || filter(m))).Count > 0;
        }

        /// <summary>Attach any number of cards from a list (taken out of the deck) to your Pokémon; the rest stay where they were.</summary>
        public static int AttachFromDeckAnyWay(EffectContext ctx, List<CardInstance> found, Func<CardInstance, PokemonInPlay, bool> filter, string prompt)
        {
            if (found.Count == 0) return 0;
            int n = ctx.AttachCardsAnyWay(found, found.Count, filter, prompt, false);
            foreach (var c in found)
                if (!ctx.MyPokemon().Any(m => m.Energy.Contains(c)) && !ctx.Me.Deck.Contains(c)) ctx.Me.Deck.Add(c);
            return n;
        }

        // ================================================================== Grass

        /// <summary>Solar Transfer: any number of times, move a Basic Grass Energy between your Pokémon. Jungle Dump: heal 30 from it.</summary>
        private sealed class MegaVenusaurExMEG : Mon
        {
            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Solar Transfer" && ctx.IsMyTurn && CanMoveBasicEnergy(ctx, EnergyType.Grass);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex) { MoveBasicEnergyRepeatedly(ctx, EnergyType.Grass, "Solar Transfer"); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jungle Dump") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        /// <summary>Guard Press: takes 30 less next turn. Stomping Wood: +30 per Grass Energy on it.</summary>
        private sealed class ExeggutorMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Guard Press")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(30);
                    return true;
                }
                if (atk.Name == "Stomping Wood")
                {
                    ctx.DamageDefending(atk.Damage + 30 * ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Grass));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Absorb: heal 30 from it. Pumped-Up Whip: +140 with 2 extra Energy beyond the cost.</summary>
        private sealed class TangrowthMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Absorb")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                    return true;
                }
                if (atk.Name == "Pumped-Up Whip")
                {
                    ctx.DamageDefending(atk.Damage + (ExtraEnergy(ctx, atk, 2) ? 140 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fermented Juice: once during your turn, with Grass Energy attached, heal 30 from 1 of your Pokémon.</summary>
        private sealed class ShuckleMEG : Mon
        {
            protected override string Activated { get { return "Fermented Juice"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Engine.HasEnergyOfType(ctx.Pokemon, EnergyType.Grass) && AnyHurt(ctx);
            }

            protected override void Ability(EffectContext ctx) { HealOneOfMine(ctx, 30, "Fermented Juice"); }
        }

        /// <summary>Reversing Gust: flip; heads shuffle 1 of the opponent's Pokémon and its cards into their deck. Perplex: Confused.</summary>
        private sealed class ShiftryMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reversing Gust")
                {
                    if (!ctx.Flip() || !ctx.Opp.HasPokemonInPlay) return true;
                    var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Reversing Gust: shuffle which of your opponent's Pokémon into their deck?");
                    if (target != null && ctx.CanAffect(target)) ctx.Engine.ShufflePokemonIntoDeck(target);
                    return true;
                }
                if (atk.Name == "Perplex")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Cast-Off Shell: when it evolves from the hand, bench a Shedinja from your deck. U-turn: switch it with a Benched Pokémon.</summary>
        private sealed class NinjaskMEG : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0;
                if (!TriggerOnce(ctx, "Cast-Off Shell", ready, "Cast-Off Shell: search your deck for a Shedinja and put it onto your Bench?")) return;
                var found = ctx.SearchMyDeck(d => d.Name == "Shedinja", 1, "Cast-Off Shell: choose a Shedinja");
                foreach (var c in found) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "U-turn") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Earthen Power: +50 if you have a Stadium in play.</summary>
        private sealed class DhelmiseMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earthen Power") return false;
                bool mine = ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player;
                ctx.DamageDefending(atk.Damage + (mine ? 50 : 0));
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Supernatural Shapeshifter: discard the top card of your deck; if it's a Supporter, use its effect.</summary>
        private sealed class NinetalesMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supernatural Shapeshifter") return false;
                var top = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                if (top.Count == 0 || !top[0].Def.IsSupporter) return true;
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + top[0].Def.Name + ".");
                ctx.Engine.Scripts.For(top[0].Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, top[0], null));
                return true;
            }
        }

        /// <summary>Roasting Heat: +160 against a Burned Pokémon. Volcanic Meteor: discard 2 Energy from it.</summary>
        private sealed class MegaCameruptExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Roasting Heat")
                {
                    bool burned = ctx.Defending != null && ctx.Defending.Has(SpecialCondition.Burned);
                    ctx.DamageDefending(atk.Damage + (burned ? 160 : 0));
                    return true;
                }
                if (atk.Name == "Volcanic Meteor")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Intimidating Fang: while Active, the opponent's Active Pokémon's attacks do 30 less damage. Searing Flame: Burned.</summary>
        private sealed class PyroarMEG : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return src.State.IsActive(info.Attacker) ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Searing Flame") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Singe: Burned. Backfire: put 2 Fire Energy attached to it into your hand.</summary>
        private sealed class VolcanionMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Singe")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Burned);
                    return true;
                }
                if (atk.Name == "Backfire")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me)) return true;
                    var fire = me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Fire)).ToList();
                    var picks = fire.Count <= 2 ? fire : ctx.ChooseCards(fire, 2, 2, "Backfire: put which 2 Fire Energy into your hand?");
                    foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Explosiveness: may start face down in the Active Spot. Turbo Flare: attach up to 3 Basic Energy from your deck to your Benched Pokémon.</summary>
        private sealed class CinderaceMEG : Mon
        {
            public override bool CanStartAsActive { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turbo Flare") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Turbo Flare: search your deck for up to 3 Basic Energy cards");
                AttachFromDeckAnyWay(ctx, found, (c, m) => ctx.Me.Bench.Contains(m), "Turbo Flare: attach to your Benched Pokémon");
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Coiling Crush: flip 2 coins; discard an Energy from the opponent's Active Pokémon for each heads.</summary>
        private sealed class CentiskorchMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coiling Crush") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, heads, "Coiling Crush: discard " + heads + " Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Scorching Earth: discard the opponent's Stadium; if you do, they can't play Stadiums next turn.</summary>
        private sealed class ChiYuMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scorching Earth") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium == null || ctx.State.StadiumOwner != ctx.Opponent) return true;
                ctx.Engine.DiscardStadium();
                ctx.AddEffect(new StadiumLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Stadium cards during their next turn (Scorching Earth)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Riptide: 20 for each Basic Water Energy in your discard pile, then shuffle them into your deck. Swirling Waves: discard 2 Energy.</summary>
        private sealed class KyogreMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Riptide")
                {
                    var water = ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                    ctx.DamageDefending(atk.Damage * water.Count);
                    if (water.Count == 0) return true;
                    foreach (var c in water)
                    {
                        ctx.Me.Discard.Remove(c);
                        ctx.Me.Deck.Add(c);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Swirling Waves")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Hammer-lanche: discard the top 6 cards of your deck, 100 for each Basic Water Energy. Frost Barrier: takes 30 less next turn.</summary>
        private sealed class MegaAbomasnowExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hammer-lanche")
                {
                    var gone = ctx.Engine.DiscardTopOfDeck(ctx.Player, 6);
                    ctx.DamageDefending(atk.Damage * gone.Count(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)));
                    return true;
                }
                if (atk.Name == "Frost Barrier")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfTakesLessNextTurn(30);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fall Back to Reload: when it moves from the Active Spot to the Bench, attach up to 2 Basic Water Energy from your hand to it. Aqua Launcher: discard all Energy.</summary>
        private sealed class ClawitzerMEG : Mon
        {
            public override void OnMovedToBenchFromActive(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var water = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList();
                bool ready = water.Count > 0 && ctx.Engine.CanAttachFromHand(me);
                if (!TriggerOnce(ctx, "Fall Back to Reload", ready, "Fall Back to Reload: attach up to 2 Basic Water Energy from your hand to " + me.Name + "?")) return;
                var picks = ctx.ChooseCards(water, 1, Math.Min(2, water.Count), "Fall Back to Reload: choose up to 2 Basic Water Energy");
                foreach (var c in picks)
                    if (ctx.Me.Hand.Remove(c)) ctx.Engine.AttachEnergyCard(c, me, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Launcher") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                return true;
            }
        }

        /// <summary>Bring Down: the Pokémon in play (either side) with the least HP left, other than this one, is Knocked Out. Water Shot: discard an Energy.</summary>
        private sealed class InteleonMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bring Down")
                {
                    var all = ctx.MyPokemon(m => m != ctx.Pokemon);
                    all.AddRange(ctx.OpponentPokemon());
                    if (all.Count == 0) return true;
                    int least = all.Min(m => ctx.Engine.RemainingHp(m));
                    var lowest = all.Where(m => ctx.Engine.RemainingHp(m) == least).ToList();
                    var target = ctx.ChoosePokemon(lowest, "Bring Down: Knock Out which Pokémon (least HP left)?");
                    if (target != null) ctx.Engine.KnockOut(target, "Bring Down");
                    return true;
                }
                if (atk.Name == "Water Shot")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Chilling Wings: 20 to each of the opponent's Pokémon; their Active Pokémon falls Asleep.</summary>
        private sealed class FrosmothMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chilling Wings") return false;
                var defending = ctx.Defending;
                foreach (var m in ctx.OpponentPokemon()) ctx.DamageTarget(m, 20);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Upper Spark: +120 if it evolved from Magneton this turn. Flashing Bolt: can't use Flashing Bolt next turn.</summary>
        private sealed class MagnezoneMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Upper Spark")
                {
                    bool fresh = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Magneton";
                    ctx.DamageDefending(atk.Damage + (fresh ? 120 : 0));
                    return true;
                }
                if (atk.Name == "Flashing Bolt")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.SelfCantUseAttackNextTurn("Flashing Bolt");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Flash Ray: no damage from Basic Pokémon next turn. Riotous Blasting: you may discard all its Energy for 130 more.</summary>
        private sealed class MegaManectricExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flash Ray")
                {
                    ctx.DamageDefending(atk.Damage);
                    ShieldFromBasics(ctx, atk.Name);
                    return true;
                }
                if (atk.Name == "Riotous Blasting")
                {
                    bool all = ctx.Pokemon.Energy.Count > 0 && ctx.AskYesNo("Riotous Blasting: discard all Energy from " + ctx.Pokemon.Name + " for 130 more damage?");
                    if (all) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    ctx.DamageDefending(atk.Damage + (all ? 130 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Electrified Incisors: next turn, each Energy card the opponent attaches from their hand to the Defending Pokémon puts 8 damage counters on it.</summary>
        private sealed class PachirisuMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electrified Incisors") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new EnergyTrapEffect(defending.Name + " gets 8 damage counters for each Energy attached to it from the hand next turn (Electrified Incisors)",
                        ctx.Player, defending.Uid, ctx.Turn + 1, 8));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Overflowing Wishes: a Basic Psychic Energy from your deck onto each of your Benched Pokémon. Mega Symphonia: 50 per Psychic Energy on all your Pokémon.</summary>
        private sealed class MegaGardevoirExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Overflowing Wishes")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    foreach (var m in ctx.MyBench())
                    {
                        if (!ctx.Me.Deck.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic))) break;
                        var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Psychic), 1, "Overflowing Wishes: a Basic Psychic Energy for " + m.Name);
                        foreach (var e in found) ctx.Engine.AttachEnergyCard(e, m);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Mega Symphonia")
                {
                    int psychic = ctx.MyPokemon().Sum(m => ctx.Engine.EnergyCountOfType(m, EnergyType.Psychic));
                    ctx.DamageDefending(atk.Damage * psychic);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Fragile Husk: no Prize cards when a Pokémon ex's attack Knocks it Out. Damage Beat: 20 per damage counter on the Defending Pokémon.</summary>
        private sealed class ShedinjaMEG : Mon
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                if (src.Pokemon == null || knockedOut != src.Pokemon || taker == src.Player) return prizes;
                bool byEx = knockedOut.HitByAttack && !knockedOut.ForcedKnockOut && knockedOut.HitByAttackerDef != null
                            && (knockedOut.HitByAttackerDef.IsEx || knockedOut.HitByAttackerDef.IsMegaEx);
                return byEx ? 0 : prizes;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damage Beat") return false;
                ctx.DamageDefending(atk.Damage * (ctx.Defending == null ? 0 : ctx.Defending.DamageCounters));
                return true;
            }
        }

        /// <summary>Energized Steps: when it evolves from the hand, attach Basic Energy from the top 4 cards of your deck to your Pokémon.</summary>
        private sealed class GrumpigMEG : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Energized Steps", ctx.Me.Deck.Count > 0, "Energized Steps: look at the top 4 cards of your deck and attach Basic Energy from them?")) return;
                var top = ctx.LookAtTop(4);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, energy.Count, (c, m) => true, "Energized Steps: attach Basic Energy", true);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Geo Gate: up to 3 Basic Psychic Pokémon onto your Bench. Bright Horns: can't use it next turn.</summary>
        private sealed class XerneasMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Geo Gate")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => d.PokemonType == EnergyType.Psychic, 3, "Geo Gate: choose up to 3 Basic Psychic Pokémon");
                    return true;
                }
                if (atk.Name == "Bright Horns")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantUseAttackNextTurn("Bright Horns");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Horrifying Bite: flip until tails; a random card from the opponent's hand goes into their deck for each heads.</summary>
        private sealed class HoundstoneMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horrifying Bite") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) CardsTWM1.ShuffleRandomFromOpponentHand(ctx, heads);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Stony Kick: also 20 to 1 Benched Pokémon. Boundless Power: can't attack next turn.</summary>
        private sealed class StonjournerMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Stony Kick")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageOneBenched(20);
                    return true;
                }
                if (atk.Name == "Boundless Power")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Powerful a-Salt: your Fighting Pokémon's attacks do 30 more to the opponent's Active Pokémon.</summary>
        private sealed class GarganaclMEG : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker == null || info.Attacker.Owner != src.Player || !info.ToOpponentsActive) return damage;
                return src.Engine.IsType(info.Attacker, EnergyType.Fighting) ? damage + 30 : damage;
            }
        }

        /// <summary>Cutting Riposte costs only [D] while this Pokémon has damage counters.</summary>
        private sealed class CrawdauntMEG : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Cutting Riposte" || ctx.Pokemon == null || ctx.Pokemon.DamageCounters == 0) return;
                cost.Clear();
                cost.Add(EnergyType.Darkness);
            }
        }

        // ================================================================== Darkness

        /// <summary>Terminal Period: Knocks Out an Active Pokémon with exactly 6 damage counters. Claw of Darkness: discard a card from the opponent's hand.</summary>
        private sealed class MegaAbsolExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Terminal Period")
                {
                    var d = ctx.Defending;
                    if (d != null && d.DamageCounters == 6) ctx.Engine.KnockOut(d, atk.Name);
                    else Nothing(ctx);
                    return true;
                }
                if (atk.Name == "Claw of Darkness")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.RevealOpponentHand(ctx);
                    var hand = ctx.Opp.Hand.ToList();
                    if (hand.Count == 0) return true;
                    var pick = ctx.ChooseCards(hand, 1, 1, "Claw of Darkness: discard a card from your opponent's hand")[0];
                    ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Spiteful Swirl: when your Active Darkness Pokémon is damaged by an opponent's attack, 1 damage counter on the Attacking Pokémon. Mountain Breaker: mill 1.</summary>
        private sealed class SpiritombMEG : Mon
        {
            private static void Swirl(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool wasActive)
            {
                if (!wasActive || attacker == null || attacker.Owner == src.Player || !src.InPlay(attacker)) return;
                if (!src.Engine.IsType(target, EnergyType.Darkness)) return;
                Announce(src, "Spiteful Swirl");
                src.Engine.PlaceDamageCounters(attacker, 1, "Spiteful Swirl");
            }

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                Swirl(ctx, ctx.Pokemon, attacker, holderWasActive);
            }

            public override void OnOwnPokemonDamagedByAttack(EffectContext src, PokemonInPlay target, PokemonInPlay attacker, bool targetWasActive, int damage)
            {
                Swirl(src, target, attacker, targetWasActive);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mountain Breaker") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Welcoming Tail: +200 with exactly 6 Prize cards left.</summary>
        private sealed class SteelixMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Welcoming Tail") return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Prizes.Count == 6 ? 200 : 0));
                return true;
            }
        }

        /// <summary>Gobble Down: 80 for each Prize card you've taken. Huge Bite: base 30 if the Defending Pokémon already has damage counters.</summary>
        private sealed class MegaMawileExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Gobble Down")
                {
                    ctx.DamageDefending(atk.Damage * ctx.Engine.PrizesTaken(ctx.Player));
                    return true;
                }
                if (atk.Name == "Huge Bite")
                {
                    bool hurt = ctx.Defending != null && ctx.Defending.DamageCounters > 0;
                    ctx.DamageDefending(hurt ? 30 : atk.Damage);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Chrono Burst: you may shuffle all its Energy into your deck for 80 more damage.</summary>
        private sealed class DialgaMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chrono Burst") return false;
                var me = ctx.Pokemon;
                bool shuffle = me.Energy.Count > 0 && ctx.AskYesNo("Chrono Burst: shuffle all Energy from " + me.Name + " into your deck for 80 more damage?");
                ctx.DamageDefending(atk.Damage + (shuffle ? 80 : 0));
                if (shuffle && ctx.InPlay(me)) ctx.Engine.ShuffleEnergyIntoDeck(me, me.Energy.ToList());
                return true;
            }
        }

        /// <summary>Haphazard Hammer: when it evolves from the hand, flip; heads discards an Energy from the opponent's Active Pokémon.</summary>
        private sealed class TinkatuffMEG : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var d = ctx.Defending;
                if (!TriggerOnce(ctx, "Haphazard Hammer", d != null && d.Energy.Count > 0, "Haphazard Hammer: flip a coin to discard an Energy from " + (d == null ? "" : d.Name) + "?")) return;
                if (ctx.Flip() && ctx.CanAffect(d)) ctx.DiscardEnergyFrom(d, 1, "Haphazard Hammer: discard an Energy from " + d.Name);
            }
        }

        /// <summary>All-You-Can-Grab: flip until tails; search your deck for up to that many cards.</summary>
        private sealed class GholdengoMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "All-You-Can-Grab") return false;
                int heads = ctx.FlipUntilTails();
                if (heads > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(d => true, heads, "All-You-Can-Grab: search your deck for up to " + heads + (heads == 1 ? " card" : " cards"), false, 1);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Strafe: you may switch it with a Benched Pokémon. Illusory Impulse: discard all its Energy.</summary>
        private sealed class MegaLatiasExMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Strafe")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0 && ctx.AskYesNo("Strafe: switch " + ctx.Pokemon.Name + " with 1 of your Benched Pokémon?"))
                        ctx.SwitchSelfWithBench();
                    return true;
                }
                if (atk.Name == "Illusory Impulse")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 99, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Lustrous Assist: when your Mega Latias ex moves from the Bench to the Active Spot, move any Energy from your Benched Pokémon to it.</summary>
        private sealed class LatiosMEG : Mon
        {
            public override void OnOwnPokemonMovedToActive(EffectContext src, PokemonInPlay mon)
            {
                if (mon.Name != "Mega Latias ex") return;
                var benchEnergy = src.Me.Bench.SelectMany(m => m.Energy).ToList();
                if (!TriggerOnce(src, "Lustrous Assist", benchEnergy.Count > 0, "Lustrous Assist: move Energy from your Benched Pokémon to " + mon.Name + "?")) return;
                var picks = src.ChooseCards(benchEnergy, 0, benchEnergy.Count, "Lustrous Assist: move which Energy to " + mon.Name + "?");
                foreach (var e in picks)
                {
                    var from = src.Me.Bench.FirstOrDefault(m => m.Energy.Contains(e));
                    if (from != null) src.Engine.MoveEnergy(from, mon, e);
                }
            }
        }

        // ================================================================== Colorless

        /// <summary>Quick Gift: usable on the first turn if you go first; search your deck for a card.</summary>
        private sealed class DelibirdMEG : Mon
        {
            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex)
            {
                return ctx.Pokemon != null && ctx.Pokemon.Def.Attacks[attackIndex].Name == "Quick Gift";
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Gift") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 1, "Quick Gift: search your deck for a card", false, 1);
                return true;
            }
        }

        /// <summary>Bellyful of Milk: flip 2 coins; both heads heals all damage from 1 of your Pokémon.</summary>
        private sealed class MiltankMEG : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bellyful of Milk") return false;
                if (ctx.FlipCoins(2) < 2) return true;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                var target = ctx.ChoosePokemon(hurt, "Bellyful of Milk: heal all damage from which Pokémon?");
                if (target != null) ctx.Heal(target, target.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Evidence Gathering: once during your turn, switch a card from your hand with the top card of your deck.</summary>
        private sealed class GumshoosMEG : Mon
        {
            protected override string Activated { get { return "Evidence Gathering"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Evidence Gathering: switch which card from your hand with the top card of your deck?")[0];
                var top = ctx.Me.Deck[0];
                ctx.Me.Deck.RemoveAt(0);
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck.Insert(0, card);
                ctx.Me.Hand.Add(top);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched a card from their hand with the top card of their deck.");
            }
        }

        // ================================================================== Trainers

        /// <summary>Only with the opponent at 2 or fewer Prize cards: next turn, 1 of your Pokémon is safe from the opponent's Pokémon ex attacks.</summary>
        private sealed class AcerolasMischief : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Prizes.Count <= 2 && ctx.Me.HasPokemonInPlay; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Acerola's Mischief: protect which of your Pokémon from Pokémon ex?");
                if (target == null) return;
                ctx.AddEffect(new ExShieldEffect(target.Name + " is protected from attacks of the opponent's Pokémon ex during their next turn (Acerola's Mischief)",
                    ctx.Player, target.Uid, ctx.Turn + 1));
            }
        }

        private sealed class ExShieldEffect : ContinuousEffect
        {
            public ExShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, false) { }

            private static bool IsEx(PokemonInPlay mon) { return mon != null && (mon.Def.IsEx || mon.Def.IsMegaEx); }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target.Uid == TargetUid && info.AgainstOpponent && IsEx(info.Attacker) ? 0 : damage;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target.Uid != TargetUid || kind != EffectSourceKind.Attack) return false;
                var attacker = engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != target.Owner && IsEx(attacker);
            }
        }

        /// <summary>Next turn, your Metal Pokémon (including new ones) take 30 less damage from the opponent's attacks.</summary>
        private sealed class IronDefender : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new TypeGuardEffect("Your Metal Pokémon take 30 less damage from attacks during the opponent's next turn (Iron Defender)",
                    ctx.Player, ctx.Turn + 1, EnergyType.Metal, 30));
            }
        }

        public sealed class TypeGuardEffect : ContinuousEffect
        {
            private readonly EnergyType type;
            private readonly int amount;

            public TypeGuardEffect(string name, int owner, int expiresAfterTurn, EnergyType type, int amount)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.type = type;
                this.amount = amount;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Owner != Owner || !info.AgainstOpponent || !engine.IsType(info.Target, type)) return damage;
                return damage - amount;
            }
        }

        /// <summary>The opponent chooses: each player takes a Prize card, or you draw 4 cards.</summary>
        private sealed class LtSurgesBargain : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                bool yes = ctx.Opp.Prizes.Count > 0 && ctx.Me.Prizes.Count > 0
                           && ctx.Engine.AskYesNo(ctx.Opponent, "Lt. Surge's Bargain: may each player take a Prize card? (If not, your opponent draws 4 cards.)", ctx.Source);
                if (yes)
                {
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " agreed: each player takes a Prize card.");
                    ctx.Engine.TakePrizeCards(ctx.Player, 1);
                    ctx.Engine.TakePrizeCards(ctx.Opponent, 1);
                }
                else ctx.Draw(4);
            }
        }

        /// <summary>Once during each player's turn: discard an Energy card from your hand to draw until your hand matches your number of Psychic Pokémon in play.</summary>
        private sealed class MysteryGarden : StadiumAction
        {
            private static int Target(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Psychic)).Count; }

            protected override bool Ready(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(c => c.Def.IsEnergy) && ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count - 1 < Target(ctx);
            }

            protected override bool Use(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var pick = ctx.ChooseCards(energy, 1, 1, "Mystery Garden: discard an Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.DrawUntil(Target(ctx));
                return false;
            }
        }

        /// <summary>The opponent switches their Active Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class Repel : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }
            public override void ResolveTrainer(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        /// <summary>Once during each player's turn: switch your Active Water Pokémon with 1 of your Benched Water Pokémon.</summary>
        private sealed class SurfingBeach : StadiumAction
        {
            protected override bool Ready(EffectContext ctx)
            {
                // Lunatone's New Moon: the switch is a Stadium effect on both Pokémon, so neither can be shielded.
                var a = ctx.Me.Active;
                return a != null && ctx.Engine.IsType(a, EnergyType.Water) && !ctx.Engine.StadiumShielded(a)
                    && ctx.Me.Bench.Any(m => ctx.Engine.IsType(m, EnergyType.Water) && !ctx.Engine.StadiumShielded(m));
            }

            protected override bool Use(EffectContext ctx)
            {
                var water = ctx.Me.Bench.Where(m => ctx.Engine.IsType(m, EnergyType.Water) && !ctx.Engine.StadiumShielded(m)).ToList();
                var target = ctx.ChoosePokemon(water, "Surfing Beach: switch in which Benched Water Pokémon?");
                if (target != null) ctx.Engine.SwitchActive(ctx.Player, target);
                return false;
            }
        }
    }
}
