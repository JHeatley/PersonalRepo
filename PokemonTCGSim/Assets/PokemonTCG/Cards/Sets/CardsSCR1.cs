using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Stellar Crown (SCR) 1-69, Pokémon. Registered by printing key. Pokémon whose attacks only do their printed
    /// damage (Ledyba, Nymble, Lokix, Toedscool, Ponyta, Reshiram, Raboot, Charcadet, Lapras, Marill, Finneon,
    /// Froakie, Frogadier, Chewtle, Mewtwo, Milcery, Flittle, Espathra) need no script.
    /// Lileep and Tirtouga evolve from the Antique Fossils (handled by the engine).
    /// </summary>
    public static class CardsSCR1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("SCR-1", new VenusaurExSCR());
            r.RegisterByKey("SCR-3", new LedianSCR());
            r.RegisterByKey("SCR-4", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SCR-5", new NoRetreatMon("Bind Down"));
            r.RegisterByKey("SCR-6", new CradilySCR());
            r.RegisterByKey("SCR-7", new CarnivineSCR());
            r.RegisterByKey("SCR-8", new MowRotomSCR());
            r.RegisterByKey("SCR-9", new FlipBonusMon("Ambush", 30));
            r.RegisterByKey("SCR-10", new CoinsMon("Double Spin", 2));
            r.RegisterByKey("SCR-11", new EldegossSCR());
            r.RegisterByKey("SCR-13", new DipplinSCR());
            r.RegisterByKey("SCR-18", new CardsTWM1.HealSelfMon("Mega Drain", 30));
            r.RegisterByKey("SCR-20", new FlipBonusMon("Burning Run", 60));
            r.RegisterByKey("SCR-21", new PansearSCR());
            r.RegisterByKey("SCR-23", new SalanditSCR());
            r.RegisterByKey("SCR-24", new SalazzleSCR());
            r.RegisterByKey("SCR-25", new TurtonatorSCR());
            r.RegisterByKey("SCR-26", new FlipBonusMon("Quick Attack", 10));
            r.RegisterByKey("SCR-28", new CinderaceExSCR());
            r.RegisterByKey("SCR-30", new BlastoiseExSCR());
            r.RegisterByKey("SCR-32", new LaprasExSCR());
            r.RegisterByKey("SCR-34", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("SCR-36", new CardsBLK.ReturnMon());
            r.RegisterByKey("SCR-37", new SwitchSelfMon("Splashing Turn", false));
            r.RegisterByKey("SCR-38", new CarracostaSCR());
            r.RegisterByKey("SCR-41", new FlipBonusMon("Shifting Shuriken", 100));
            r.RegisterByKey("SCR-42", new CrabominableSCR());
            r.RegisterByKey("SCR-44", new DrednawSCR());
            r.RegisterByKey("SCR-45", new VeluzaSCR());
            r.RegisterByKey("SCR-46", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("SCR-47", new ElectivireSCR());
            r.RegisterByKey("SCR-48", new CoinsMon("Double Voltage", 2));
            r.RegisterByKey("SCR-49", new LanturnSCR());
            r.RegisterByKey("SCR-50", new JoltikSCR());
            r.RegisterByKey("SCR-51", new GalvantulaExSCR());
            r.RegisterByKey("SCR-52", new CharjabugSCR());
            r.RegisterByKey("SCR-53", new VikavoltSCR());
            r.RegisterByKey("SCR-54", new TogedemaruSCR());
            r.RegisterByKey("SCR-55", new CardsSVI2.MaddeningScentMon("Combat Thunder", 20));
            r.RegisterByKey("SCR-56", new CardsMEG.SnipeMon("Targeted Spark", 20));
            r.RegisterByKey("SCR-60", new ShieldAttackMon("Expand", 10));
            r.RegisterByKey("SCR-61", new DrifblimSCR());
            r.RegisterByKey("SCR-62", new YamaskSCR());
            r.RegisterByKey("SCR-63", new ComfeySCR());
            r.RegisterByKey("SCR-65", new AlcremieSCR());
            r.RegisterByKey("SCR-66", new BenchSearchAttackMon("Pleasant Aroma", 1));
            r.RegisterByKey("SCR-67", new DachsbunExSCR());
        }

        // ================================================================== Shared helpers

        /// <summary>Damage to the Defending Pokémon, then Special Conditions on it if it's still the Active Pokémon.</summary>
        private static void HitThen(EffectContext ctx, int damage, params SpecialCondition[] conditions)
        {
            var defending = ctx.Defending;
            ctx.DamageDefending(damage);
            if (!TefPokemon.StillDefending(ctx, defending)) return;
            foreach (var condition in conditions) ctx.ApplyCondition(defending, condition);
        }

        /// <summary>"Discard N Energy from this Pokémon" after the damage.</summary>
        private static void DiscardOwnEnergy(EffectContext ctx, int count, string attackName)
        {
            if (!ctx.InPlay(ctx.Pokemon)) return;
            ctx.DiscardEnergyFrom(ctx.Pokemon, count, attackName + ": discard " + (count == 1 ? "an Energy" : count + " Energy") + " from this Pokémon");
        }

        /// <summary>Number of different Special Conditions affecting the Pokémon.</summary>
        private static int CountConditions(PokemonInPlay mon)
        {
            if (mon == null) return 0;
            var all = new[] { SpecialCondition.Asleep, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned, SpecialCondition.Burned };
            int n = 0;
            foreach (var condition in all) if (mon.Has(condition)) n++;
            return n;
        }

        /// <summary>"During your next turn, your Pokémon can't attack. (This includes new Pokémon that come into play.)"</summary>
        private sealed class PlayerCantAttackEffect : ContinuousEffect
        {
            public PlayerCantAttackEffect(string name, int owner, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool PreventsAttacks(GameEngine engine, PokemonInPlay mon)
            {
                return mon != null && mon.Owner == Owner;
            }
        }

        /// <summary>"During your opponent's next turn, prevent all damage done to this Pokémon by attacks from Basic Pokémon."</summary>
        private sealed class BasicAttackerShieldEffect : ContinuousEffect
        {
            public BasicAttackerShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || !info.AgainstOpponent) return damage;
                return info.Attacker.Def.IsBasicPokemon ? 0 : damage;
            }
        }

        /// <summary>Food Prep: attacks used by this Pokémon cost [C] less for each Kofu card in your discard pile.</summary>
        private abstract class FoodPrepMon : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon != src.Pokemon) return;
                int kofu = src.Me.Discard.Count(card => card.Def.Name == "Kofu");
                for (int i = 0; i < kofu; i++) if (!cost.Remove(EnergyType.Colorless)) break;
            }
        }

        // ================================================================== Grass

        /// <summary>Tranquil Flower: once during your turn, while Active, heal 60 damage from 1 of your Pokémon.</summary>
        private sealed class VenusaurExSCR : Mon
        {
            protected override string Activated { get { return "Tranquil Flower"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return;
                var target = ctx.ChoosePokemon(hurt, "Tranquil Flower: heal 60 damage from which of your Pokémon?");
                ctx.Heal(target, 60);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dangerous Toxwhip") return false;
                HitThen(ctx, atk.Damage, SpecialCondition.Confused, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Glittering Star Pattern (when played to evolve): gust a Benched Pokémon with 90 HP or less remaining. Swift ignores everything.</summary>
        private sealed class LedianSCR : Mon
        {
            private static List<PokemonInPlay> StarTargets(EffectContext ctx)
            {
                return ctx.GustTargets().Where(m => ctx.RemainingHp(m) <= 90).ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var targets = StarTargets(ctx);
                if (!TriggerOnce(ctx, "Glittering Star Pattern", targets.Count > 0,
                    "Use Glittering Star Pattern to switch in 1 of your opponent's Benched Pokémon that has 90 HP or less remaining?")) return;
                var target = ctx.ChoosePokemon(targets, "Glittering Star Pattern: choose 1 of your opponent's Benched Pokémon (90 HP or less remaining) to switch in");
                if (target != null) ctx.Engine.SwitchActive(ctx.Opponent, target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swift") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>Selective Slime: flip; if heads, Burned, Confused or Poisoned (your choice). Miasma Wind: 100x Special Conditions.</summary>
        private sealed class CradilySCR : Mon
        {
            protected override string Activated { get { return "Selective Slime"; } }
            // Compendium (2024-09-12): it can't be used if the opponent's Active Pokémon is already Burned, Confused and
            // Poisoned (it would do nothing).
            protected override bool AbilityReady(EffectContext ctx)
            {
                var a = ctx.Opp.Active;
                return a != null && !(a.Has(SpecialCondition.Burned) && a.Has(SpecialCondition.Confused) && a.Has(SpecialCondition.Poisoned));
            }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var target = ctx.Opp.Active;
                if (target == null) return;
                int pick = ctx.ChooseOption("Selective Slime: choose the Special Condition for your opponent's Active Pokémon", "Burned", "Confused", "Poisoned");
                var condition = pick == 0 ? SpecialCondition.Burned : (pick == 1 ? SpecialCondition.Confused : SpecialCondition.Poisoned);
                ctx.ApplyCondition(target, condition);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miasma Wind") return false;
                ctx.DamageDefending(atk.Damage * CountConditions(ctx.Defending));
                return true;
            }
        }

        private sealed class CarnivineSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Nosh")
                {
                    ctx.Heal(ctx.Pokemon, 40);
                    return true;
                }
                if (atk.Name == "Loom Over")
                {
                    ctx.DamageDefending(Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Reaping Dash: before damage, discard the Tool and all Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class MowRotomSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reaping Dash") return false;
                var defending = ctx.Defending;
                if (defending != null && (defending.Tool != null || defending.Energy.Any(x => x.Def.IsAttachedSpecialEnergy)))
                {
                    if (!ctx.CanAffect(defending))
                    {
                        ctx.Log("The effect of Reaping Dash on " + defending.Name + " was prevented.");
                    }
                    else
                    {
                        ctx.Engine.DiscardAllTools(defending);
                        foreach (var energy in defending.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList())
                            ctx.Engine.DiscardEnergy(defending, energy);
                    }
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Breezy Gift: shuffle this Pokémon and its cards into the deck, then take up to 3 cards from the deck.</summary>
        private sealed class EldegossSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breezy Gift") return false;
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                ctx.SearchToHand(d => true, 3, "Breezy Gift: put up to 3 cards from your deck into your hand", false, 1);
                return true;
            }
        }

        private sealed class DipplinSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Coated Attack") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new BasicAttackerShieldEffect(ctx.Pokemon.Name + " takes no damage from attacks by Basic Pokémon during the opponent's next turn (Coated Attack)",
                        ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Fire

        private sealed class PansearSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Call for Family")
                {
                    if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                        ctx.SearchToBench(d => true, 1, "Call for Family: put a Basic Pokémon onto your Bench");
                    return true;
                }
                if (atk.Name == "Searing Flame")
                {
                    HitThen(ctx, atk.Damage, SpecialCondition.Burned);
                    return true;
                }
                return false;
            }
        }

        private sealed class SalanditSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ember") return false;
                ctx.DamageDefending(atk.Damage);
                DiscardOwnEnergy(ctx, 1, atk.Name);
                return true;
            }
        }

        /// <summary>Sudden Scorching: the opponent discards 1 card (3 if this Pokémon evolved from Salandit this turn).</summary>
        private sealed class SalazzleSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Sudden Scorching")
                {
                    var self = ctx.Pokemon;
                    bool fromSalandit = self.TurnEvolved == ctx.Turn && self.Stack.Count >= 2 && self.Stack[self.Stack.Count - 2].Def.Name == "Salandit";
                    int n = Math.Min(ctx.Opp.Hand.Count, fromSalandit ? 3 : 1);
                    if (n <= 0) return true;
                    var picks = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), n, n,
                        "Sudden Scorching: discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source);
                    foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                    return true;
                }
                if (atk.Name == "Flamethrower")
                {
                    ctx.DamageDefending(atk.Damage);
                    DiscardOwnEnergy(ctx, 1, atk.Name);
                    return true;
                }
                return false;
            }
        }

        private sealed class TurtonatorSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ring of Fire")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (!TefPokemon.StillDefending(ctx, defending)) return true;
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    ctx.DefendingCantRetreat(defending);
                    return true;
                }
                if (atk.Name == "Inferno Onrush")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.DamageSelf(60);
                    return true;
                }
                return false;
            }
        }

        private sealed class CinderaceExSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flare Strike")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantUseAttackNextTurn(atk.Name);
                    return true;
                }
                if (atk.Name == "Garnet Volley")
                {
                    ctx.DamageAnyOpponent(180);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>Solid Shell: takes 30 less damage from attacks. Twin Cannons: discard up to 2 Basic Water Energy from hand, 140 each.</summary>
        private sealed class BlastoiseExSCR : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Twin Cannons") return false;
                var water = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Water).ToList();
                var picks = ctx.ChooseCards(water, 0, 2, "Twin Cannons: discard up to 2 Basic Water Energy cards from your hand (140 damage each)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Larimar Rain: look at the top 20 cards, attach any number of Energy cards from them to your Pokémon, shuffle.</summary>
        private sealed class LaprasExSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Power Splash")
                {
                    ctx.DamageDefending(atk.Damage * ctx.EnergyCount(ctx.Pokemon));
                    return true;
                }
                if (atk.Name == "Larimar Rain")
                {
                    var top = ctx.LookAtTop(20);
                    if (top.Count == 0) return true;
                    var energy = top.Where(card => card.Def.IsEnergy).ToList();
                    if (energy.Count > 0)
                        ctx.AttachCardsAnyWay(energy, energy.Count,
                            (e, m) => ctx.Engine.Scripts.For(e.Def).CanAttachTo(new EffectContext(ctx.Engine, ctx.Player, e, m), m),
                            "Larimar Rain: attach an Energy card from the top 20 cards of your deck");
                    ctx.ShuffleMyDeck();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Primal Knowledge: your attacks do 30 more damage to the opponent's Active Evolution Pokémon.</summary>
        private sealed class CarracostaSCR : Mon
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (a == null || a.Owner != src.Player || !info.ToOpponentsActive || !info.Target.Def.IsEvolution) return damage;
                return damage + 30;
            }
        }

        private sealed class CrabominableSCR : FoodPrepMon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haymaker") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Impervious Shell: prevent all damage from the opponent's attacks if that damage is 200 or more.</summary>
        private sealed class DrednawSCR : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.AgainstOpponent && damage >= 200;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hard Crunch") return false;
                bool damaged = ctx.Defending != null && ctx.Defending.DamageCounters > 0;
                ctx.DamageDefending(atk.Damage + (damaged ? 80 : 0));
                return true;
            }
        }

        private sealed class VeluzaSCR : FoodPrepMon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sonic Edge") return false;
                ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Unleash Lightning: during your next turn, none of your Pokémon can attack.</summary>
        private sealed class ElectivireSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Unleash Lightning") return false;
                ctx.DamageDefending(atk.Damage);
                var lockEffect = new PlayerCantAttackEffect(ctx.Engine.PlayerName(ctx.Player) + "'s Pokémon can't attack during " + ctx.Engine.Their(ctx.Player) + " next turn (Unleash Lightning)",
                    ctx.Player, ctx.Turn + 2);
                lockEffect.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(lockEffect);
                return true;
            }
        }

        private sealed class LanturnSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Disorienting Flash")
                {
                    var defending = ctx.Defending;
                    if (defending != null && ctx.ApplyCondition(defending, SpecialCondition.Confused))
                        defending.ConfusionCounters = 8; // "Put 8 damage counters instead of 3 on that Pokémon for this Special Condition"
                    return true;
                }
                if (atk.Name == "Thunderous Bolt")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.SelfCantAttackNextTurn();
                    return true;
                }
                return false;
            }
        }

        /// <summary>Jolting Charge: up to 2 Basic Grass and up to 2 Basic Lightning Energy from the deck, attached in any way you like.</summary>
        private sealed class JoltikSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jolting Charge") return false;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Grass, 2, "Jolting Charge: choose up to 2 Basic Grass Energy cards");
                found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Lightning, 2, "Jolting Charge: choose up to 2 Basic Lightning Energy cards"));
                if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, (e, m) => true, "Jolting Charge", false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        private sealed class GalvantulaExSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Charged Web")
                {
                    bool exOrV = ctx.Defending != null && TefPokemon.IsExOrV(ctx.Defending.Def);
                    ctx.DamageDefending(atk.Damage + (exOrV ? 110 : 0));
                    return true;
                }
                if (atk.Name == "Fulgurite")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                    ctx.Opp.ItemLockTurn = ctx.Turn + 1;
                    ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Item cards during their next turn.");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Parallel Placement: put up to 3 Charjabug from the deck onto the Bench.</summary>
        private sealed class CharjabugSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Parallel Placement") return false;
                int room = ctx.BenchSpace;
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Charjabug", Math.Min(3, room), "Parallel Placement: put up to 3 Charjabug onto your Bench");
                foreach (var card in found)
                {
                    if (ctx.BenchSpace <= 0)
                    {
                        ctx.Me.Deck.Add(card);
                        continue;
                    }
                    ctx.Engine.PutOntoBench(ctx.Player, card, false);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        private sealed class VikavoltSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Volt Switch") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon) || !ctx.PokemonIsActive) return true;
                var lightning = ctx.MyBench().Where(m => ctx.Engine.IsType(m, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return true;
                var target = ctx.ChoosePokemon(lightning, "Volt Switch: switch this Pokémon with which Benched Lightning Pokémon?");
                ctx.Engine.SwitchActive(ctx.Player, target);
                return true;
            }
        }

        private sealed class TogedemaruSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electrifying Chance") return false;
                if (ctx.Me.Prizes.Count == 1) HitThen(ctx, atk.Damage, SpecialCondition.Paralyzed);
                else ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Everyone Explode Now: 50x your Drifloon and Drifblim in play, then 30 damage to each of them.</summary>
        private sealed class DrifblimSCR : Mon
        {
            private static bool IsBalloon(PokemonInPlay mon) { return mon.Name == "Drifloon" || mon.Name == "Drifblim"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Everyone Explode Now") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(IsBalloon).Count);
                foreach (var mon in ctx.MyPokemon(IsBalloon))
                    ctx.DamageWith(mon, 30, false, false, false);
                return true;
            }
        }

        private sealed class YamaskSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ominous Eyes") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Ominous Eyes: put 3 damage counters on which of your opponent's Pokémon?");
                ctx.PlaceCounters(target, 3);
                return true;
            }
        }

        private sealed class ComfeySCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flower Shower")
                {
                    ctx.Draw(3);
                    ctx.Engine.DrawCards(ctx.Opponent, 3);
                    return true;
                }
                if (atk.Name == "Play Rough")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Flip() ? 20 : 0));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Colorful Confection: up to 5 Pokémon matching the type of any Basic Energy attached to this Pokémon.</summary>
        private sealed class AlcremieSCR : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Colorful Confection") return false;
                var types = ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().ToList();
                if (types.Count == 0 || ctx.Me.Deck.Count == 0) return true;
                ctx.SearchToHand(d => d.IsPokemon && types.Contains(d.PokemonType), 5,
                    "Colorful Confection: choose up to 5 Pokémon of the same type as a Basic Energy attached to this Pokémon");
                return true;
            }
        }

        /// <summary>Time to Chow Down (when played to evolve): heal all damage from each of your Evolution Pokémon, then discard all Energy from those healed.</summary>
        private sealed class DachsbunExSCR : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var hurt = ctx.MyPokemon(m => m.Def.IsEvolution && m.DamageCounters > 0);
                if (!TriggerOnce(ctx, "Time to Chow Down", hurt.Count > 0,
                    "Use Time to Chow Down to heal all damage from each of your Evolution Pokémon (each Pokémon healed then discards all of its Energy)?")) return;
                foreach (var mon in hurt)
                {
                    if (!ctx.InPlay(mon)) continue;
                    int healed = ctx.Heal(mon, mon.DamageCounters * 10);
                    if (healed > 0) ctx.Engine.DiscardAllEnergy(mon);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wonder Shine") return false;
                HitThen(ctx, atk.Damage, SpecialCondition.Confused);
                return true;
            }
        }
    }
}
