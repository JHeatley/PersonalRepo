using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Obsidian Flames (OBF) 1-122: the Pokémon that weren't already in the pool (all G-mark, Expanded only). Reprints already
    /// in the pool (Charmander, Victini ex, Carvanha, Miraidon ex, Togekiss, Sinistea, Pupitar...) are alternate prints.
    /// Pokémon whose attacks only do their printed damage need no script: Phantump, Rowlet, Charcadet, Palpitoad, Wiglett,
    /// Finizen, Magnemite, Magneton, Tynamo, Spoink, Greavard (99), Larvitar, Crabrawler, Rockruff, Toedscool.
    /// </summary>
    public static class CardsOBF1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("OBF-1", new DrawAttackMon("Feelin' Fine", 1));
            r.RegisterByKey("OBF-2", new CardsPOR.FollowMeMon("Inviting Scent"));
            r.RegisterByKey("OBF-3", new ComboMon(new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep), new CardsASC.FlipPerEnergyMon("Powerful Dance")));
            r.RegisterByKey("OBF-5", new ComboMon(new BenchSearchAttackMon("Berry Scent", 1), new CardsASC.SelfGuardMon("Guard Press", 30)));
            r.RegisterByKey("OBF-6", new TailsNothingMon("Lunge"));
            r.RegisterByKey("OBF-7", new MasquerainOBF());
            r.RegisterByKey("OBF-8", new HealBenchedMon("Share", 20));
            r.RegisterByKey("OBF-9", new FoongusOBF());
            r.RegisterByKey("OBF-10", new CardsPGO.PoisonStringUpMon("Dangerous Spores"));
            r.RegisterByKey("OBF-12", new TrevenantOBF());
            r.RegisterByKey("OBF-14", new Cards30CC.BenchHitMon("Shoot Through", 20));
            r.RegisterByKey("OBF-15", new DecidueyeExOBF());
            r.RegisterByKey("OBF-16", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("OBF-17", new CardsDET.HealingMelodyMon("Aromatherapy", 30));
            r.RegisterByKey("OBF-18", new ComboMon(new QueenlyHeelMon("Queenly Heel"), new RecoilMon("Spinning Kick", 20)));
            r.RegisterByKey("OBF-19", new CardsTWM1.HealSelfMon("Absorb", 10));
            r.RegisterByKey("OBF-20", new CardsTWM1.HealSelfMon("Sunny Wind", 30));
            r.RegisterByKey("OBF-21", new HealBenchedMon("Healing Fruit", 9990));
            r.RegisterByKey("OBF-23", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("OBF-24", new CoinsMon("Double Headbutt", 2));
            r.RegisterByKey("OBF-27", new RecoilMon("Heat Tackle", 20));
            r.RegisterByKey("OBF-28", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("OBF-29", new NinetalesOBF());
            r.RegisterByKey("OBF-31", new ConditionAttackMon("Hot Magma", SpecialCondition.Burned));
            r.RegisterByKey("OBF-32", new CameruptOBF());
            r.RegisterByKey("OBF-34", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("OBF-35", new CardsASC.SelfDamagedBonusMon("Counterpunch", 60));
            r.RegisterByKey("OBF-36", new FlipBonusMon("Firebreathing", 10));
            r.RegisterByKey("OBF-37", new CardsWHT.DragOffMon("Alluring Fireball", 30));
            r.RegisterByKey("OBF-38", new CardsMEG.DefendingEnergyBonusMon("Combustion Chain", 50));
            r.RegisterByKey("OBF-39", new CardsMEG.DefendingEnergyBonusMon("Energy Burner", 30));
            r.RegisterByKey("OBF-40", new RecoilMon("Take Down", 10));
            r.RegisterByKey("OBF-41", new AttachFromDiscardToSelfMon("Flame Cloak", EnergyType.Fire));
            r.RegisterByKey("OBF-42", new EiscueExOBF());
            r.RegisterByKey("OBF-44", new CardsCRI.CounterConditionMon("Scorching Armor", SpecialCondition.Burned, null));
            r.RegisterByKey("OBF-45", new CardsCRE1.RainfallMon("Hail", 10, 10));
            r.RegisterByKey("OBF-47", new RetreatCostDamageMon("Aqua Impact", 30, false));
            r.RegisterByKey("OBF-48", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("OBF-49", new FloatzelOBF());
            r.RegisterByKey("OBF-50", new CardsASC.FlipDiscardDefendingEnergyMon("Screw Tail"));
            r.RegisterByKey("OBF-52", new SeismitoadOBF());
            r.RegisterByKey("OBF-53", new FlipBonusMon("Trip Over", 20));
            r.RegisterByKey("OBF-54", new BearticOBF());
            r.RegisterByKey("OBF-55", new CryogonalOBF());
            r.RegisterByKey("OBF-56", new TailsNothingMon("Try Bouncing"));
            r.RegisterByKey("OBF-57", new SwitchSelfMon("Strafe", true));
            r.RegisterByKey("OBF-59", new NoRetreatMon("Entwining Entrapment"));
            r.RegisterByKey("OBF-65", new ComboMon(new MagneticRepulsionMon(), new RecoilMon("Thunder", 30)));
            r.RegisterByKey("OBF-66", new ComboMon(new MountainHurlMon("Mountain Hurl", 2), new CardsSVP.BenchHurtBonusMon("Lightning Rampage", 100)));
            r.RegisterByKey("OBF-68", new ParalyzeFlipMon("Volt Wave"));
            r.RegisterByKey("OBF-69", new EelektrossOBF());
            r.RegisterByKey("OBF-73", new PawmotExOBF());
            r.RegisterByKey("OBF-74", new AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning));
            r.RegisterByKey("OBF-75", new RecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("OBF-76", new Cards30CC.BenchHitMon("Shake and Discharge", 10));
            r.RegisterByKey("OBF-77", new Cards30CC.BenchHitMon("Electrobullet", 30));
            r.RegisterByKey("OBF-78", new BelliboltOBF());
            r.RegisterByKey("OBF-81", new CoinsMon("Slap Slap", 2));
            r.RegisterByKey("OBF-82", new ClefableExOBF());
            r.RegisterByKey("OBF-83", new TogepiOBF());
            r.RegisterByKey("OBF-84", new TogeticOBF());
            r.RegisterByKey("OBF-87", new RecoilMon("Double-Edge", 10));
            r.RegisterByKey("OBF-88", new RecoilMon("Wild Tackle", 30));
            r.RegisterByKey("OBF-89", new MawileOBF());
            r.RegisterByKey("OBF-91", new Cards30C.EnergyGiftMon("Powerful Steps"));
            r.RegisterByKey("OBF-92", new LunatoneOBF());
            r.RegisterByKey("OBF-93", new BenchSearchAttackMon("Call for Family", 2));
            r.RegisterByKey("OBF-94", new CardsFST2.RapidSpinMon("Rapid Spin"));
            r.RegisterByKey("OBF-95", new ComboMon(new KaboomDollMon(), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("OBF-96", new VespiquenExOBF());
            r.RegisterByKey("OBF-98", new PolteageistOBF());
            r.RegisterByKey("OBF-100", new FlipBonusMon("Play Rough", 30));
            r.RegisterByKey("OBF-101", new HoundstoneOBF());
            r.RegisterByKey("OBF-102", new HoundstoneExOBF());
            r.RegisterByKey("OBF-103", new MountainHurlMon("Hole-Diggin' Noggin", 1));
            r.RegisterByKey("OBF-104", new DodgeMon("Dig", false));
            r.RegisterByKey("OBF-107", new NosepassOBF());
            r.RegisterByKey("OBF-109", new WhiscashOBF());
            r.RegisterByKey("OBF-110", new ConditionAttackMon("Blubbering", SpecialCondition.Confused));
            r.RegisterByKey("OBF-111", new FlipBonusMon("Rising Lunge", 10));
            r.RegisterByKey("OBF-112", new StunfiskOBF());
            r.RegisterByKey("OBF-113", new CardsSSP2.StompOffMon("Knocking Hammer", 1));
            r.RegisterByKey("OBF-115", new LockAfterAttackMon("Knuckle Impact", false));
            r.RegisterByKey("OBF-117", new LycanrocOBF());
            r.RegisterByKey("OBF-119", new CoinsMon("Double Whip", 2));
            r.RegisterByKey("OBF-120", new KlawfExOBF());
            r.RegisterByKey("OBF-121", new CardsMEW1.WithdrawMon("Iron Defense"));
            r.RegisterByKey("OBF-122", new ConditionAttackMon("Poison Shard", SpecialCondition.Poisoned));
        }

        // ================================================================== Shared helpers (also used by CardsOBF2)

        /// <summary>"Heal N damage from 1 of your Benched Pokémon" (Share). A huge amount means "heal all damage" (Healing Fruit).</summary>
        public sealed class HealBenchedMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealBenchedMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.Me.Bench.Where(m => m.DamageCounters > 0 && !ctx.Engine.HealingBlocked(m)).ToList();
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, attack + ": heal which of your Benched Pokémon?");
                if (target != null) ctx.Heal(target, Math.Min(amount, target.DamageCounters * 10));
                return true;
            }
        }

        /// <summary>"Attach a Basic [T] Energy card from your discard pile to this Pokémon" after the damage (Flame Cloak, Energize).</summary>
        public sealed class AttachFromDiscardToSelfMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;

            public AttachFromDiscardToSelfMon(string attack, EnergyType type)
            {
                this.attack = attack;
                this.type = type;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var energy = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == type);
                if (energy.Count == 0) return true;
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, attack + ": attach which Basic " + type + " Energy to " + me.Name + "?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, me);
                return true;
            }
        }

        /// <summary>
        /// "This attack does N (more) damage for each [C] in your opponent's Active Pokémon's Retreat Cost" (Aqua Impact, Chomp Chomp
        /// Panic). It uses the Retreat Cost as changed by effects (Compendium, Grass Knot).
        /// </summary>
        public sealed class RetreatCostDamageMon : Mon
        {
            private readonly string attack;
            private readonly int per;
            private readonly bool multiply;

            public RetreatCostDamageMon(string attack, int per, bool multiply)
            {
                this.attack = attack;
                this.per = per;
                this.multiply = multiply;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : ctx.Engine.RetreatCostOf(defending);
                int damage = multiply ? per * cost : atk.Damage + per * cost;
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log(ctx.Pokemon.Name + "'s " + attack + " did no damage.");
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Panic-Prompting Pattern: flip until tails; discard a random card from the opponent's hand for each heads.</summary>
        private sealed class MasquerainOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Panic-Prompting Pattern") return false;
                int heads = ctx.FlipUntilTails();
                if (heads > 0) CardsASC.DiscardRandomFromOpponentHand(ctx, heads);
                return true;
            }
        }

        /// <summary>Enticing Pattern: put a Basic Grass Pokémon from your deck onto your Bench.</summary>
        private sealed class FoongusOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enticing Pattern") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.PokemonType == EnergyType.Grass, 1, "Enticing Pattern: put a Basic Grass Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>Forest Miasma: in every Pokémon Checkup, if Trevenant is Active, 1 damage counter on the opponent's Active Pokémon. Lock Up: can't retreat.</summary>
        private sealed class TrevenantOBF : Mon
        {
            private static readonly Mon LockUp = new NoRetreatMon("Lock Up");

            public override int CheckupAffects(EffectContext src) { return src.PokemonIsActive ? 1 - src.Player : -2; }

            public override void OnPokemonCheckup(EffectContext src)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || src.Opp.Active == null) return;
                src.Engine.PlaceDamageCounters(src.Opp.Active, 1, "Forest Miasma (" + src.Pokemon.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LockUp.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Total Freedom: once during your turn, if it's Benched, switch it with your Active Pokémon; if it's Active, switch it with
        /// 1 of your Benched Pokémon. Hunting Arrow: 30 to 1 of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class DecidueyeExOBF : Mon
        {
            private static readonly Mon HuntingArrow = new Cards30CC.BenchHitMon("Hunting Arrow", 30);

            protected override string Activated { get { return "Total Freedom"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.PokemonIsActive) ctx.SwitchSelfWithBench();
                else if (ctx.Me.Bench.Contains(me)) ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HuntingArrow.HandleAttack(ctx, atk); }
        }

        /// <summary>Queenly Heel: during the opponent's next turn, no Pokémon can be played from their hand to evolve the Defending Pokémon (Rare Candy included). It ends if that Pokémon moves to the Bench (Compendium, 2023-08-10). Used for Queenly Heel, B Cancel (the attack name is passed in).</summary>
        public sealed class QueenlyHeelMon : Mon
        {
            private readonly string attack;

            public QueenlyHeelMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new NoEvolveFromHandEffect(defending.Name + " can't be evolved from the hand during its next turn (" + attack + ")", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        public sealed class NoEvolveFromHandEffect : ContinuousEffect
        {
            public NoEvolveFromHandEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksEvolutionFromHand(GameEngine engine, PokemonInPlay target) { return target.Uid == TargetUid; }
        }

        // ================================================================== Fire

        /// <summary>Nine-Tailed Dance: put 9 damage counters on 1 of the opponent's Pokémon; Ninetales can't attack during your next turn.</summary>
        private sealed class NinetalesOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Nine-Tailed Dance") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count > 0)
                {
                    var target = ctx.ChoosePokemon(all, "Nine-Tailed Dance: put 9 damage counters on which of your opponent's Pokémon?");
                    ctx.PlaceCounters(target, 9);
                }
                ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        /// <summary>Eruption: discard the top card of each player's deck; 100 more damage for each Energy card discarded this way.</summary>
        private sealed class CameruptOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption") return false;
                var discarded = new List<CardInstance>();
                discarded.AddRange(ctx.Engine.DiscardTopOfDeck(ctx.Player, 1));
                discarded.AddRange(ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1));
                int energy = discarded.Count(c => c.Def.IsEnergy);
                ctx.DamageDefending(atk.Damage + 100 * energy);
                return true;
            }
        }

        /// <summary>Scalding Block (Tera): discard an Energy from this Pokémon; during the opponent's next turn the Defending Pokémon can't attack.</summary>
        private sealed class EiscueExOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scalding Block") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Scalding Block: discard which Energy from " + ctx.Pokemon.Name + "?");
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack during its next turn (Scalding Block)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Swirling Tail: flip a coin; on heads put the opponent's Active Pokémon and all attached cards into their hand.</summary>
        private sealed class FloatzelOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Tail") return false;
                var defending = ctx.Defending;
                if (defending == null || !ctx.Flip()) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Swirling Tail on " + defending.Name + " was prevented.");
                    return true;
                }
                if (!ctx.Engine.CanPutIntoHand(defending))
                {
                    ctx.Log(defending.Name + " can't be put into the hand.");
                    return true;
                }
                ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Quaking Zone: while Seismitoad is Active, the opponent's Active Pokémon's attacks cost [C] more. Echoed Voice: +100 next turn (doesn't stack).</summary>
        private sealed class SeismitoadOBF : Mon
        {
            private static readonly Mon EchoedVoice = new CardsASC.NextTurnBoostMon("Echoed Voice", 100);

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || mon.Owner == src.Player || !src.State.IsActive(mon)) return;
                cost.Add(EnergyType.Colorless);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return EchoedVoice.HandleAttack(ctx, atk); }
        }

        /// <summary>Frost Purge: flip a coin; on tails discard all Energy from this Pokémon.</summary>
        private sealed class BearticOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frost Purge") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>First Freeze: if you went second and it's your first turn, the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class CryogonalOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "First Freeze") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                bool secondsFirstTurn = ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
                if (secondsFirstTurn && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Magnetic Repulsion: after the damage, you may switch out the opponent's Active Pokémon (they choose the new one).</summary>
        private sealed class MagneticRepulsionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Repulsion") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && ctx.AskYesNo("Magnetic Repulsion: switch out your opponent's Active Pokémon?"))
                    ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Mountain Hurl: discard the top 2 cards of your deck after the damage. Used for Mountain Hurl, Wild Splash, Hole-Diggin' Noggin (the attack name and the numbers are passed in).</summary>
        internal sealed class MountainHurlMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public MountainHurlMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Player, amount);
                return true;
            }
        }

        /// <summary>
        /// Suction Shock: switch in 1 of the opponent's Benched Pokémon, 60 damage to it, then flip: heads, it's Paralyzed. With no
        /// switch there's no new Active Pokémon, so nothing happens (Compendium, 2023-08-10).
        /// </summary>
        private sealed class EelektrossOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Suction Shock") return false;
                var target = ctx.GustOpponent("Suction Shock: choose 1 of your opponent's Benched Pokémon to switch in");
                if (target == null) return true;
                ctx.DamageDefending(60);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, target)) ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Levin Strike: discard 2 [L] Energy from this Pokémon, then 220 damage to 1 of the opponent's Pokémon.</summary>
        private sealed class PawmotExOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Levin Strike") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Levin Strike: discard 2 Lightning Energy", e => ProvidesType(ctx, me, e, EnergyType.Lightning));
                ctx.DamageAnyOpponent(220);
                return true;
            }
        }

        /// <summary>True when this attached Energy card provides that type right now.</summary>
        public static bool ProvidesType(EffectContext ctx, PokemonInPlay holder, CardInstance energy, EnergyType type)
        {
            foreach (int unit in ctx.Engine.EnergyUnitsOf(holder, energy)) if (EnergyMask.Provides(unit, type)) return true;
            return false;
        }

        /// <summary>Insulator: prevent all damage done to it by attacks from the opponent's [L] Pokémon. Thunderous Edge: ignores effects on the Defending Pokémon.</summary>
        private sealed class BelliboltOBF : Mon
        {
            private static readonly Mon ThunderousEdge = new CardsPAF2.IgnoreDefenderEffectsMon("Thunderous Edge");

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && src.Engine.IsType(info.Attacker, EnergyType.Lightning);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return ThunderousEdge.HandleAttack(ctx, atk); }
        }

        // ================================================================== Psychic

        /// <summary>Lunar Zone: your Pokémon with any [P] Energy attached have no Retreat Cost. Wondrous Moon: you may move any amount of [P] Energy between your Pokémon.</summary>
        private sealed class ClefableExOBF : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon.Owner != src.Player || mon.Energy.Count == 0) return cost;
                return src.Engine.HasEnergyOfType(mon, EnergyType.Psychic) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wondrous Moon") return false;
                ctx.DamageDefending(atk.Damage);
                MovePsychicEnergy(ctx);
                return true;
            }

            private static void MovePsychicEnergy(EffectContext ctx)
            {
                for (int guard = 0; guard < 60; guard++)
                {
                    var mine = ctx.MyPokemon();
                    if (mine.Count < 2) return;
                    var movable = new List<CardInstance>();
                    foreach (var holder in mine)
                        foreach (var e in holder.Energy)
                            if (ProvidesType(ctx, holder, e, EnergyType.Psychic)) movable.Add(e);
                    if (movable.Count == 0) return;
                    var picks = ctx.ChooseCards(movable, 0, 1, "Wondrous Moon: choose a Psychic Energy to move (or none to stop)");
                    if (picks.Count == 0) return;
                    var card = picks[0];
                    var from = mine.FirstOrDefault(m => m.Energy.Contains(card));
                    if (from == null) return;
                    var to = ctx.ChoosePokemon(mine.Where(m => m != from).ToList(), "Wondrous Moon: move " + card.Def.Name + " to which Pokémon?");
                    if (to == null) return;
                    ctx.Engine.MoveEnergy(from, to, card);
                }
            }
        }

        /// <summary>Whiny Voice: a random card from the opponent's hand is revealed and shuffled into their deck.</summary>
        private sealed class TogepiOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Whiny Voice") return false;
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.Reveal(ctx.Opponent, new List<CardInstance> { card }, "shuffled this card from " + ctx.Engine.Their(ctx.Opponent) + " hand into " + ctx.Engine.Their(ctx.Opponent) + " deck (Whiny Voice)");
                hand.Remove(card);
                ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Shared Peace: each player draws 3 cards.</summary>
        private sealed class TogeticOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shared Peace") return false;
                ctx.Draw(3);
                ctx.Engine.DrawCards(ctx.Opponent, 3);
                return true;
            }
        }

        /// <summary>Mischievous Crunch: 30 damage for each [P] Energy attached to this Pokémon.</summary>
        private sealed class MawileOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mischievous Crunch") return false;
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Psychic);
                if (n > 0) ctx.DamageDefending(30 * n);
                else ctx.Log(ctx.Pokemon.Name + "'s Mischievous Crunch did no damage.");
                return true;
            }
        }

        /// <summary>
        /// New Moon: if you have Solrock in play, prevent all effects of any Stadium done to your Pokémon in play. Every Stadium
        /// effect on them is off, good or bad (your ruling); the engine does this (GameEngine.StadiumShielded).
        /// </summary>
        private sealed class LunatoneOBF : Mon
        {
            public override bool ShieldsOwnPokemonFromStadium(EffectContext ctx)
            {
                return ctx.MyPokemon().Any(m => m.Def.Name == "Solrock");
            }
        }

        /// <summary>Kaboom Doll: damage counters on the opponent's Active Pokémon until its remaining HP is 10; if any were placed, 120 damage to this Pokémon.</summary>
        private sealed class KaboomDollMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Kaboom Doll") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int counters = (ctx.RemainingHp(defending) - 10) / 10;
                int placed = counters > 0 ? ctx.Engine.PlaceDamageCounters(defending, counters, "Kaboom Doll") : 0;
                if (placed > 0) ctx.DamageSelf(120);
                return true;
            }
        }

        /// <summary>Healing Pheromone (Tera): heal 60 from 1 of your Pokémon. Phantom Queen: 3 damage counters on each of the opponent's Benched Pokémon that has damage counters.</summary>
        private sealed class VespiquenExOBF : Mon
        {
            private static readonly Mon Pheromone = new CardsPOR.SweetScentMon("Healing Pheromone", 60);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Phantom Queen") return Pheromone.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench())
                    if (mon.DamageCounters > 0) ctx.PlaceCounters(mon, 3);
                return true;
            }
        }

        /// <summary>Antique Collecting: up to 2 Item and Pokémon Tool cards from your discard pile into your hand. Pour Tea: 5 damage counters on the opponent's Active Pokémon.</summary>
        private sealed class PolteageistOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Antique Collecting")
                {
                    var cards = ctx.Me.Discard.Where(c => c.Def.IsItem || c.Def.IsTool).ToList();
                    if (cards.Count == 0) return true;
                    var picks = ctx.ChooseCards(cards, 0, Math.Min(2, cards.Count), "Antique Collecting: put up to 2 Item and Pokémon Tool cards into your hand");
                    if (picks.Count > 0) ctx.DiscardToHand(picks);
                    return true;
                }
                if (atk.Name == "Pour Tea")
                {
                    if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 5);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Two Four-ocious: 120 more damage if the opponent has exactly 2 or 4 Prize cards left.</summary>
        private sealed class HoundstoneOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Two Four-ocious") return false;
                int left = ctx.Opp.Prizes.Count;
                ctx.DamageDefending(atk.Damage + (left == 2 || left == 4 ? 120 : 0));
                return true;
            }
        }

        /// <summary>Big Bite: can't retreat. Last Respects: 10 more for each Psychic Pokémon card in your discard pile (each V-UNION piece counts).</summary>
        private sealed class HoundstoneExOBF : Mon
        {
            private static readonly Mon BigBite = new NoRetreatMon("Big Bite");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Last Respects") return BigBite.HandleAttack(ctx, atk);
                int n = ctx.Me.Discard.Count(c => c.Def.IsPokemon && c.Def.PokemonType == EnergyType.Psychic);
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Iron Collecting: put up to 2 Basic [M] Energy cards from your discard pile into your hand.</summary>
        private sealed class NosepassOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Iron Collecting") return false;
                var metal = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
                if (metal.Count == 0) return true;
                var picks = ctx.ChooseCards(metal, 0, Math.Min(2, metal.Count), "Iron Collecting: put up to 2 Basic Metal Energy into your hand");
                if (picks.Count > 0) ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Raging and Rocking: discard the top card of the opponent's deck for each [F] Energy attached to this Pokémon.</summary>
        private sealed class WhiscashOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Raging and Rocking") return false;
                int n = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fighting);
                if (n > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Custom Trap: if it's Active with a Pokémon Tool attached and damaged by the opponent's attack, 5 damage counters on the Attacking Pokémon. Rumble: can't retreat.</summary>
        private sealed class StunfiskOBF : Mon
        {
            private static readonly Mon Rumble = new NoRetreatMon("Rumble");

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var me = ctx.Pokemon;
                if (me == null || !holderWasActive || me.Tools.Count == 0) return;
                if (attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                Announce(ctx, "Custom Trap");
                ctx.Engine.PlaceDamageCounters(attacker, 5, "Custom Trap");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rumble.HandleAttack(ctx, atk); }
        }

        /// <summary>Finishing Fang: does nothing if the opponent's Active Pokémon has no damage counters before the damage.</summary>
        private sealed class LycanrocOBF : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Finishing Fang") return false;
                var defending = ctx.Defending;
                if (defending == null || defending.DamageCounters == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Counterattacking Pincer: if it's Active and damaged by the opponent's attack (even if Knocked Out), discard an Energy from
        /// the Attacking Pokémon. Falling Press: flip, heads +80.
        /// </summary>
        private sealed class KlawfExOBF : Mon
        {
            private static readonly Mon FallingPress = new FlipBonusMon("Falling Press", 80);

            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (attacker.Energy.Count == 0 || !ctx.CanAffect(attacker)) return;
                Announce(ctx, "Counterattacking Pincer");
                ctx.DiscardEnergyFrom(attacker, 1, "Counterattacking Pincer: discard which Energy from " + attacker.Name + "?");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FallingPress.HandleAttack(ctx, atk); }
        }
    }
}
