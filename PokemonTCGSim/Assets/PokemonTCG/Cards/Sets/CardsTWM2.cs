using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Twilight Masquerade (TWM) 76-164. Pokémon whose attacks only do their printed damage need no script:
    /// Clefairy, Aipom.
    /// </summary>
    public static class CardsTWM2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("TWM-76", new KilowattrelTWM());
            r.RegisterByKey("TWM-77", new IronThornsExTWM());
            r.RegisterByKey("TWM-79", new CardsEVO3.MetronomeMon("Metronome"));
            r.RegisterByKey("TWM-81", new KadabraTWM());
            r.RegisterByKey("TWM-83", new GirafarigTWM());
            r.RegisterByKey("TWM-84", new FarigirafTWM());
            r.RegisterByKey("TWM-85", new ChimechoTWM());
            r.RegisterByKey("TWM-86", new FlabebeTWM());
            r.RegisterByKey("TWM-87", new FloetteTWM());
            r.RegisterByKey("TWM-88", new FlorgesTWM());
            r.RegisterByKey("TWM-89", new SwirlixTWM());
            r.RegisterByKey("TWM-90", new SlurpuffTWM());
            r.RegisterByKey("TWM-91", new SandAttackMon());
            r.RegisterByKey("TWM-92", new SandAttackMon());
            r.RegisterByKey("TWM-94", new ScreamTailExTWM());
            r.RegisterByKey("TWM-96", new FezandipitiTWM());
            r.RegisterByKey("TWM-97", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("TWM-98", new CardsSSP1.BillowingHeatWaveMon("Earthquake", 10));
            r.RegisterByKey("TWM-99", new HisuianGrowlitheTWM());
            r.RegisterByKey("TWM-100", new HisuianArcanineTWM());
            r.RegisterByKey("TWM-101", new NosepassTWM());
            r.RegisterByKey("TWM-102", new ProbopassTWM());
            r.RegisterByKey("TWM-103", new TailsNothingMon("Best Punch"));
            r.RegisterByKey("TWM-104", new GurdurrTWM());
            r.RegisterByKey("TWM-105", new ConkeldurrTWM());
            r.RegisterByKey("TWM-106", new GreninjaExTWM());
            r.RegisterByKey("TWM-107", new CardsPAR1.PrizeCountMon(90));
            r.RegisterByKey("TWM-108", new CardsTWM1.SelfDiscardMon("Rock Shot", 1));
            r.RegisterByKey("TWM-109", new GlimmoraTWM());
            r.RegisterByKey("TWM-110", new TingLuTWM());
            r.RegisterByKey("TWM-111", new OkidogiTWM());
            r.RegisterByKey("TWM-112", new CornerstoneMaskOgerponExTWM());
            r.RegisterByKey("TWM-113", new CardsPFL.FlipUntilTailsTimesMon("Continuous Steps"));
            r.RegisterByKey("TWM-114", new MightyenaTWM());
            r.RegisterByKey("TWM-115", new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("TWM-116", new WhirlipedeTWM());
            r.RegisterByKey("TWM-117", new Cards30C.SwirlingResentmentMon("Dastardly Jab", 10));
            r.RegisterByKey("TWM-118", new BruteBonnetTWM());
            r.RegisterByKey("TWM-119", new SkarmoryTWM());
            r.RegisterByKey("TWM-120", new RecoilMon("Double-Edge", 10));
            r.RegisterByKey("TWM-121", new RecoilMon("Steel Tackle", 20));
            r.RegisterByKey("TWM-122", new AggronTWM());
            r.RegisterByKey("TWM-124", new VaroomTWM());
            r.RegisterByKey("TWM-125", new RevavroomTWM());
            r.RegisterByKey("TWM-127", new CardsWHT.DragOffMon("Syrup Catcher", 70));
            r.RegisterByKey("TWM-132", new FarfetchdTWM());
            r.RegisterByKey("TWM-133", new ChanseyTWM());
            r.RegisterByKey("TWM-134", new BlisseyExTWM());
            r.RegisterByKey("TWM-135", new EeveeTWM());
            r.RegisterByKey("TWM-136", new SnorlaxTWM());
            r.RegisterByKey("TWM-138", new AmbipomTWM());
            r.RegisterByKey("TWM-139", new DrawAttackMon("Double Draw", 2));
            r.RegisterByKey("TWM-140", new SwannaTWM());

            r.RegisterByName("Accompanying Flute", new AccompanyingFlute());
            r.RegisterByName("Caretaker", new Caretaker());
            r.RegisterByName("Cook", new Cook());
            r.RegisterByName("Hyper Aroma", new CountSearchItem(d => d.IsPokemon && d.Stage == Stage.Stage1, 3, "Hyper Aroma: choose up to 3 Stage 1 Pokémon"));
            r.RegisterByName("Love Ball", new LoveBall());
            r.RegisterByName("Lucian", new Lucian());
            r.RegisterByName("Ogre's Mask", new OgresMask());
            r.RegisterByName("Perrin", new Perrin());
            r.RegisterByName("Raifort", new Raifort());
            r.RegisterByName("Scoop Up Cyclone", new ScoopUpCyclone());
            r.RegisterByName("Survival Brace", new SurvivalBrace());
        }

        // ================================================================== Lightning

        /// <summary>Wind Power Charge: +120 to this Pokémon's attacks during your next turn. Strong Volt: discard an Energy.</summary>
        private sealed class KilowattrelTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Wind Power Charge")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive)
                        ctx.AddEffect(new SelfAttackBonusEffect(ctx.Pokemon.Name + "'s attacks do 120 more damage during its next turn (Wind Power Charge)",
                            ctx.Player, ctx.Pokemon.Uid, ctx.Turn, 120));
                    return true;
                }
                if (atk.Name == "Strong Volt")
                {
                    ctx.DamageDefending(atk.Damage);
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Initialization: while this Pokémon is Active, Pokémon with a Rule Box (both sides) have no Abilities, except Future Pokémon.</summary>
        private sealed class IronThornsExTWM : Mon
        {
            public override bool IsAbilityBlocker { get { return true; } }

            public override bool BlocksAbility(EffectContext src, PokemonInPlay mon, string abilityName)
            {
                return src.Pokemon != null && src.PokemonIsActive && mon.Def.HasRuleBox && !mon.Def.IsFuture;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volt Cyclone") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var energy = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Volt Cyclone: move which Energy to your Bench?")[0];
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Volt Cyclone: move " + energy.Def.Name + " to which Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                return true;
            }
        }

        // ================================================================== Psychic

        private sealed class KadabraTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 30 * (defending == null ? 0 : ctx.EnergyCount(defending)));
                return true;
            }
        }

        /// <summary>Dual Headbutt: also 10 damage to 1 of your own Benched Pokémon.</summary>
        private sealed class GirafarigTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Headbutt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Dual Headbutt: do 10 damage to which of your Benched Pokémon?");
                    if (target != null) ctx.DamageTarget(target, 10);
                }
                return true;
            }
        }

        /// <summary>One-derful Rumble: 40 per Stage 1 Pokémon you have in play. Eerie Wave: Confused.</summary>
        private sealed class FarigirafTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "One-derful Rumble")
                {
                    ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.Def.Stage == Stage.Stage1).Count);
                    return true;
                }
                if (atk.Name == "Eerie Wave")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Homeward Chime: shuffle 1 of your Benched Pokémon and all attached cards into your deck.</summary>
        private sealed class ChimechoTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Homeward Chime") return false;
                if (ctx.Me.Bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyBench(), "Homeward Chime: shuffle which Benched Pokémon into your deck?");
                if (target != null) ctx.Engine.ShufflePokemonIntoDeck(target);
                return true;
            }
        }

        /// <summary>Bind Wound: heal 30 damage from 1 of your Pokémon.</summary>
        private sealed class FlabebeTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bind Wound") return false;
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Bind Wound: heal 30 damage from which of your Pokémon?");
                if (target != null) ctx.Heal(target, 30);
                return true;
            }
        }

        private sealed class FloetteTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Minor Errand-Running") return false;
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => d.IsBasicEnergy, 3, "Minor Errand-Running: choose up to 3 Basic Energy cards");
                return true;
            }
        }

        /// <summary>Captivating Invitation: once during your turn, flip; if heads, switch in 1 of the opponent's Benched Pokémon and Confuse it.</summary>
        private sealed class FlorgesTWM : Mon
        {
            protected override string Activated { get { return "Captivating Invitation"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.GustTargets().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var target = ctx.GustOpponent("Captivating Invitation: choose 1 of your opponent's Benched Pokémon to switch in");
                if (target != null) ctx.ApplyCondition(target, SpecialCondition.Confused);
            }
        }

        /// <summary>Festival Lead plus Sneaky Placement: 2 damage counters on 1 of the opponent's Pokémon.</summary>
        private sealed class SwirlixTWM : FestivalLeadMon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Placement") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Sneaky Placement: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
                return true;
            }
        }

        /// <summary>Slurp Slurp: 90 per heads of 2 coins; if both are tails, the opponent's Active Pokémon is Confused.</summary>
        private sealed class SlurpuffTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Slurp Slurp") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads == 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>"If the Defending Pokémon tries to use an attack, your opponent flips a coin. If tails, that attack doesn't happen."</summary>
        private sealed class SandAttackEffect : ContinuousEffect
        {
            public SandAttackEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool ForcesAttackFlip(GameEngine engine, PokemonInPlay attacker) { return attacker.Uid == TargetUid; }
        }

        /// <summary>Sandygast and Palossand's Sand Attack.</summary>
        private sealed class SandAttackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Attack") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new SandAttackEffect(defending.Name + " must flip a coin to attack during its next turn (Sand Attack)",
                        ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>The player can't play Supporter cards from their hand (Scream).</summary>
        internal sealed class SupporterLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public SupporterLockEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card)
            {
                return player == affected && card.Def.IsSupporter;
            }
        }

        /// <summary>Scream: only on your first turn if you went second; the opponent can't play Supporters next turn. Crunch: discard an Energy.</summary>
        private sealed class ScreamTailExTWM : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Scream") return true;
                return ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Scream")
                {
                    ctx.AddEffect(new SupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (Scream)",
                        ctx.Player, ctx.Opponent, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Crunch")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                        ctx.DiscardEnergyFrom(defending, 1, "Crunch: discard an Energy from " + defending.Name);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Adrena-Pheromone: with Darkness Energy attached, flip when damaged by an attack; heads prevents that damage. Energy Feather: 30 per Energy.</summary>
        private sealed class FezandipitiTWM : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null) return false;
                if (!src.Engine.HasEnergyOfType(me, EnergyType.Darkness)) return false;
                return src.Engine.FlipCoin(src.Player, "Adrena-Pheromone (" + me.Name + ")");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Feather") return false;
                ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Blazing Destruction: discard a Stadium in play. Take Down: 10 to itself.</summary>
        private sealed class HisuianGrowlitheTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Blazing Destruction")
                {
                    CardsTWM1.DiscardStadiumInPlay(ctx);
                    return true;
                }
                if (atk.Name == "Take Down")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(10);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Proud Fangs: +90 if any of your Benched Pokémon have damage counters. Searing Flame: Burned.</summary>
        private sealed class HisuianArcanineTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Proud Fangs")
                {
                    bool hurt = ctx.Me.Bench.Any(b => b.DamageCounters > 0);
                    ctx.DamageDefending(atk.Damage + (hurt ? 90 : 0));
                    return true;
                }
                if (atk.Name == "Searing Flame")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Power Rush: flip; if tails, this Pokémon can't attack during your next turn.</summary>
        private sealed class NosepassTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Rush") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        private sealed class ProbopassTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assault Laser") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Tool != null ? 80 : 0));
                return true;
            }
        }

        /// <summary>Superpower: you may do 30 more damage; if you do, 30 damage to itself.</summary>
        private sealed class GurdurrTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Superpower") return false;
                bool more = ctx.AskYesNo("Superpower: do 30 more damage (and 30 damage to " + ctx.Pokemon.Name + ")?");
                ctx.DamageDefending(atk.Damage + (more ? 30 : 0));
                if (more) ctx.DamageSelf(30);
                return true;
            }
        }

        /// <summary>Tantrum: this Pokémon is now Confused. Gutsy Swing: costs nothing while this Pokémon has a Special Condition.</summary>
        private sealed class ConkeldurrTWM : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name == "Gutsy Swing" && ctx.Pokemon != null && ctx.Pokemon.Conditions != SpecialCondition.None) cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tantrum") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Shinobi Blade: you may search your deck for any card. Mirage Barrage: discard 2 Energy, then 120 to 2 of the opponent's Pokémon.</summary>
        private sealed class GreninjaExTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shinobi Blade")
                {
                    ctx.DamageDefending(atk.Damage);
                    // Compendium: searching is optional, but if you search you must take a card.
                    if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Shinobi Blade: search your deck for a card?"))
                        ctx.SearchToHand(d => true, 1, "Shinobi Blade: choose a card to put into your hand", false, 1);
                    return true;
                }
                if (atk.Name == "Mirage Barrage")
                {
                    CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                    var all = ctx.OpponentPokemon();
                    if (all.Count == 0) return true;
                    var targets = ctx.ChoosePokemonMany(all, Math.Min(2, all.Count), 2, "Mirage Barrage: choose 2 of your opponent's Pokémon (120 damage each)");
                    foreach (var t in targets) ctx.DamageTarget(t, 120);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Stun Poison: flip; if heads, Paralyzed and Poisoned. Venoshock: +100 if the opponent's Active Pokémon is Poisoned.</summary>
        private sealed class GlimmoraTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Stun Poison")
                {
                    var defending = ctx.Defending;
                    if (defending != null && ctx.Flip())
                    {
                        ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                        ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                    }
                    return true;
                }
                if (atk.Name == "Venoshock")
                {
                    var defending = ctx.Defending;
                    bool poisoned = defending != null && defending.Has(SpecialCondition.Poisoned);
                    ctx.DamageDefending(atk.Damage + (poisoned ? 100 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Ground Crasher: with a Stadium in play, also 30 to each opposing Benched Pokémon, then discard the Stadium.</summary>
        private sealed class TingLuTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ground Crasher") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium == null) return true;
                foreach (var b in ctx.OpponentBench()) ctx.DamageTarget(b, 30);
                CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        /// <summary>Adrena-Power: with Darkness Energy attached, +100 HP and its attacks do 100 more to the opponent's Active Pokémon.</summary>
        private sealed class OkidogiTWM : Mon
        {
            private static bool Powered(EffectContext src)
            {
                return src.Pokemon != null && src.Engine.HasEnergyOfType(src.Pokemon, EnergyType.Darkness);
            }

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                return mon == src.Pokemon && Powered(src) ? hp + 100 : hp;
            }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                return info.Attacker == src.Pokemon && info.ToOpponentsActive && Powered(src) ? damage + 100 : damage;
            }
        }

        /// <summary>Cornerstone Stance: prevent damage from attacks of opposing Pokémon that have an Ability. Demolish ignores Weakness, Resistance and effects.</summary>
        private sealed class CornerstoneMaskOgerponExTWM : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && src.Engine.HasAbility(info.Attacker);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Demolish") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Hunting Pack: +90 with another Mightyena on your Bench. Corner: the Defending Pokémon can't retreat.</summary>
        private sealed class MightyenaTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Hunting Pack")
                {
                    bool pack = ctx.Me.Bench.Any(b => b.Name == "Mightyena");
                    ctx.DamageDefending(atk.Damage + (pack ? 90 : 0));
                    return true;
                }
                if (atk.Name == "Corner")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Poison Ring: Poisoned, and it can't retreat during the opponent's next turn.</summary>
        private sealed class WhirlipedeTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Ring") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        /// <summary>Poison Spray: Poisoned. Relentless Punches: +50 per damage counter on the opponent's Active Pokémon.</summary>
        private sealed class BruteBonnetTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Poison Spray")
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Poisoned);
                    return true;
                }
                if (atk.Name == "Relentless Punches")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage + 50 * (defending == null ? 0 : defending.DamageCounters));
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Metal

        private sealed class SkarmoryTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Big Storm") return false;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.DiscardStadiumInPlay(ctx);
                return true;
            }
        }

        /// <summary>Angry Slam: 50 per Pokémon of yours with damage counters. Guard Claw: takes 50 less next turn.</summary>
        private sealed class AggronTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Angry Slam")
                {
                    ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => m.DamageCounters > 0).Count);
                    return true;
                }
                if (atk.Name == "Guard Claw")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfTakesLessNextTurn(50);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Metal Coating: attach a Basic Metal Energy from your discard pile to this Pokémon.</summary>
        private sealed class VaroomTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Coating") return false;
                var metal = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Metal);
                if (metal.Count == 0) return true;
                var card = ctx.ChooseCards(metal, 1, 1, "Metal Coating: choose a Basic Metal Energy to attach")[0];
                ctx.Me.Discard.Remove(card);
                ctx.Engine.AttachEnergyCard(card, ctx.Pokemon);
                return true;
            }
        }

        /// <summary>Rally Back: +90 after a Knock Out by attack damage last turn. Running Charge: 100 per heads until tails.</summary>
        private sealed class RevavroomTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rally Back")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutByAttackLastOpponentTurn() ? 90 : 0));
                    return true;
                }
                if (atk.Name == "Running Charge")
                {
                    ctx.DamageDefending(atk.Damage * ctx.FlipUntilTails());
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Dragon

        // ================================================================== Colorless

        /// <summary>Impromptu Carrier: played from the hand to the Bench, you may search for a Pokémon Tool and attach it. Mach Cut: discard a Special Energy.</summary>
        private sealed class FarfetchdTWM : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var mon = ctx.Pokemon;
                if (!TriggerOnce(ctx, "Impromptu Carrier", ctx.Me.Deck.Count > 0 && ctx.Engine.CanTakeTool(mon),
                    "Impromptu Carrier: search your deck for a Pokémon Tool and attach it to " + mon.Name + "?")) return;
                var found = ctx.SearchMyDeck(d => d.IsTool, 1, "Impromptu Carrier: choose a Pokémon Tool");
                if (found.Count > 0 && ctx.Engine.CanTakeTool(mon)) ctx.Engine.AttachTool(found[0], mon);
                else if (found.Count > 0) ctx.Me.Deck.AddRange(found);
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Cut") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Any(e => e.Def.IsAttachedSpecialEnergy))
                    ctx.DiscardEnergyFrom(defending, 1, "Mach Cut: discard a Special Energy from " + defending.Name, e => e.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Lucky Attachment: attach a Basic Energy from your hand to 1 of your Pokémon. Boundless Power: can't attack next turn.</summary>
        private sealed class ChanseyTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lucky Attachment")
                {
                    var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                    if (basics.Count == 0) return true;
                    var card = ctx.ChooseCards(basics, 1, 1, "Lucky Attachment: choose a Basic Energy to attach")[0];
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Lucky Attachment: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null) return true;
                    ctx.Me.Hand.Remove(card);
                    ctx.Engine.AttachEnergyCard(card, target, true);
                    return true;
                }
                if (atk.Name == "Boundless Power")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Happy Switch: once during your turn, move a Basic Energy from 1 of your Pokémon to another. Return: you may draw until you have 6.</summary>
        private sealed class BlisseyExTWM : Mon
        {
            protected override string Activated { get { return "Happy Switch"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon().Count > 1 && ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var from = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Energy.Any(e => e.Def.IsBasicEnergy)), "Happy Switch: move a Basic Energy from which Pokémon?");
                if (from == null) return;
                var energy = ctx.ChooseCards(from.Energy.Where(e => e.Def.IsBasicEnergy).ToList(), 1, 1, "Happy Switch: choose the Basic Energy to move")[0];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), "Happy Switch: move " + energy.Def.Name + " to which Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(from, to, energy);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Return: draw cards until you have 6 in your hand?"))
                    ctx.DrawUntil(6);
                return true;
            }
        }

        /// <summary>Ascension: evolve this Pokémon with a card from your deck. Quick Attack: +20 on heads.</summary>
        private sealed class EeveeTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ascension")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    var me = ctx.Pokemon;
                    var found = ctx.SearchMyDeck(d => d.IsPokemon && d.EvolvesFrom == me.Name && !ctx.Engine.Scripts.For(d).PutIntoPlayOnlyByEffect, 1,
                        "Ascension: choose a card that evolves from " + me.Name, false);
                    if (found.Count > 0) ctx.Engine.EvolvePokemon(me, found[0]);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Quick Attack")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 20 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>But First, Food: attach an Energy card from your hand to this Pokémon; if you do, heal 60.</summary>
        private sealed class SnorlaxTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "But First, Food") return false;
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "But First, Food: choose an Energy card to attach to " + ctx.Pokemon.Name)[0];
                var script = ctx.Engine.Scripts.For(card.Def);
                if (!script.CanAttachTo(new EffectContext(ctx.Engine, ctx.Player, card, ctx.Pokemon), ctx.Pokemon)) return true;
                ctx.Me.Hand.Remove(card);
                ctx.Engine.AttachEnergyCard(card, ctx.Pokemon, true);
                ctx.Heal(ctx.Pokemon, 60);
                return true;
            }
        }

        /// <summary>Wicked Tail: on evolving from the hand, you may flip 2 coins; per heads a random card from the opponent's hand is shuffled into their deck.</summary>
        private sealed class AmbipomTWM : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Wicked Tail", ctx.Opp.Hand.Count > 0, "Wicked Tail: flip 2 coins to shuffle random cards from your opponent's hand into their deck?"))
                    return;
                int heads = ctx.FlipCoins(2);
                CardsTWM1.ShuffleRandomFromOpponentHand(ctx, heads);
            }
        }

        private sealed class SwannaTWM : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fighting Wings") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && TefPokemon.IsExOrV(defending.Def) ? 90 : 0));
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Search for up to N cards matching a filter and put them into your hand.</summary>
        private sealed class CountSearchItem : CardScript
        {
            private readonly Func<CardDef, bool> filter;
            private readonly int count;
            private readonly string prompt;

            public CountSearchItem(Func<CardDef, bool> filter, int count, string prompt)
            {
                this.filter = filter;
                this.count = count;
                this.prompt = prompt;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.SearchToHand(filter, count, prompt); }
        }

        /// <summary>Reveal the top 5 cards of the opponent's deck; put any Basic Pokémon among them onto their Bench; they shuffle the rest back.</summary>
        private sealed class AccompanyingFlute : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var opp = ctx.Opp;
                var top = opp.Deck.Take(Math.Min(5, opp.Deck.Count)).ToList();
                ctx.Engine.Reveal(ctx.Opponent, top, "revealed the top " + top.Count + " cards of " + ctx.Engine.Their(ctx.Opponent) + " deck");
                var basics = top.Where(c => c.Def.IsBasicPokemon).ToList();
                int room = ctx.Engine.BenchSpace(ctx.Opponent);
                if (basics.Count > 0 && room > 0)
                {
                    var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Accompanying Flute: put any of these Basic Pokémon onto your opponent's Bench");
                    foreach (var c in picks)
                    {
                        opp.Deck.Remove(c);
                        ctx.Engine.PutOntoBench(ctx.Opponent, c, false);
                    }
                }
                ctx.Engine.ShuffleDeck(ctx.Opponent);
            }
        }

        /// <summary>Draw 2 cards; if you drew any and Community Center is in play, shuffle this card into your deck instead of discarding it.</summary>
        private sealed class Caretaker : CardScript
        {
            // Compendium (Professor Juniper, 2014-11-13): a draw Trainer can't be played with an empty deck (no effect).
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int drawn = ctx.Draw(2);
                if (drawn > 0 && ctx.State.Stadium != null && ctx.State.Stadium.Def.Name == "Community Center") ctx.ReturnToDeck = true;
            }
        }

        /// <summary>Heal 70 damage from your Active Pokémon.</summary>
        private sealed class Cook : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                var a = ctx.Me.Active;
                return a != null && a.DamageCounters > 0 && !ctx.Engine.HealingBlocked(a);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Active != null) ctx.Heal(ctx.Me.Active, 70);
            }
        }

        /// <summary>Search for a Pokémon with the same name as 1 of the opponent's Pokémon in play.</summary>
        private sealed class LoveBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var names = new HashSet<string>(ctx.Opp.InPlay().Select(m => m.Name));
                ctx.SearchToHand(d => d.IsPokemon && names.Contains(d.Name), 1, "Love Ball: choose a Pokémon with the same name as 1 of your opponent's Pokémon");
            }
        }

        /// <summary>Each player puts their hand on the bottom of their deck; then each flips: heads draw 6, tails draw 3.</summary>
        private sealed class Lucian : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                int total = 0;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    if (player != ctx.Player && TefPokemon.OpponentShielded(ctx)) continue;
                    var hand = p.Hand.ToList();
                    p.Hand.Clear();
                    ctx.State.Rng.Shuffle(hand);
                    p.Deck.AddRange(hand);
                    total += hand.Count;
                    ctx.Log(ctx.Engine.PlayerName(player) + " put " + hand.Count + " cards from " + ctx.Engine.Their(player) + " hand on the bottom of " + ctx.Engine.Their(player) + " deck.");
                }
                if (total == 0) return;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    if (player != ctx.Player && ctx.Engine.SupporterEffectsPreventedOn(player)) continue;
                    bool heads = ctx.Engine.FlipCoin(player, "Lucian");
                    ctx.Engine.DrawCards(player, heads ? 6 : 3);
                }
            }
        }

        /// <summary>Switch an Ogerpon Pokémon ex in your discard pile with 1 of your Ogerpon Pokémon ex in play (everything attached stays).</summary>
        private sealed class OgresMask : CardScript
        {
            private static bool IsOgerponEx(CardDef d) { return d.IsPokemon && d.IsEx && d.Name.Contains("Ogerpon"); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Discard.Any(c => IsOgerponEx(c.Def)) && ctx.MyPokemon(m => IsOgerponEx(m.Def)).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var inPlay = ctx.MyPokemon(m => IsOgerponEx(m.Def));
                var inDiscard = ctx.Me.Discard.Where(c => IsOgerponEx(c.Def)).ToList();
                if (inPlay.Count == 0 || inDiscard.Count == 0) return;
                var target = ctx.ChoosePokemon(inPlay, "Ogre's Mask: switch which Ogerpon in play?");
                var card = ctx.ChooseCards(inDiscard, 1, 1, "Ogre's Mask: switch it with which Ogerpon from your discard pile?")[0];
                if (target == null) return;
                ctx.Me.Discard.Remove(card);
                CardsTWM1.ReplaceTopCard(ctx, target, card, ctx.Me.Discard);
            }
        }

        /// <summary>Reveal up to 2 Pokémon from your hand and put them into your deck; then search for up to that many Pokémon.</summary>
        private sealed class Perrin : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Any(c => c != ctx.Card && c.Def.IsPokemon); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mons = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count == 0) return;
                var picks = ctx.ChooseCards(mons, 1, Math.Min(2, mons.Count), "Perrin: reveal up to 2 Pokémon from your hand to put into your deck");
                if (picks.Count == 0) return;
                ctx.Engine.Reveal(ctx.Player, picks, "put these Pokémon into " + ctx.Engine.Their(ctx.Player) + " deck");
                foreach (var c in picks) ctx.Me.Hand.Remove(c);
                ctx.Me.Deck.AddRange(picks);
                ctx.SearchToHand(d => d.IsPokemon, picks.Count, "Perrin: choose up to " + picks.Count + " Pokémon");
            }
        }

        /// <summary>Look at the top 5 cards of your deck, discard any number of them, and put the rest back in any order.</summary>
        private sealed class Raifort : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var me = ctx.Me;
                var top = ctx.LookAtTop(5);
                if (top.Count == 0) return;
                var discard = ctx.ChooseCards(top, 0, top.Count, "Raifort: discard any number of these cards");
                foreach (var c in top) me.Deck.Remove(c);
                if (discard.Count > 0)
                {
                    me.Discard.AddRange(discard);
                    ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded "
                        + string.Join(", ", discard.Select(c => c.Def.Name).ToArray()) + " from the top of " + ctx.Engine.Their(ctx.Player) + " deck.", discard);
                }
                var rest = top.Where(c => !discard.Contains(c)).ToList();
                var order = new List<CardInstance>();
                while (rest.Count > 1)
                {
                    var next = ctx.ChooseCards(rest, 1, 1, "Raifort: choose the card to put " + (order.Count == 0 ? "on top of your deck" : "next (card " + (order.Count + 1) + " from the top)"))[0];
                    order.Add(next);
                    rest.Remove(next);
                }
                order.AddRange(rest);
                me.Deck.InsertRange(0, order);
            }
        }

        /// <summary>Put 1 of your Pokémon and all cards attached to it into your hand.</summary>
        private sealed class ScoopUpCyclone : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                // The Active Pokémon only when there's a Benched Pokémon to take its place.
                return ctx.MyPokemon(m => ctx.Engine.CanPutIntoHand(m) && (ctx.Me.Active != m || ctx.Me.Bench.Count > 0));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Scoop Up Cyclone: put which Pokémon and its attached cards into your hand?");
                if (target != null) ctx.Engine.ReturnPokemonToHand(target);
            }
        }

        /// <summary>If its holder has full HP and would be Knocked Out by damage from an opponent's attack, it survives with 10 HP; then discard this card.</summary>
        private sealed class SurvivalBrace : CardScript
        {
            public override bool SurvivesKnockOutFromFullHp(EffectContext src, PokemonInPlay mon) { return mon == src.Pokemon; }

            public override void OnSavedFromKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (mon.HasTool(src.Card)) src.Engine.DiscardTool(mon, src.Card);
            }
        }
    }
}
