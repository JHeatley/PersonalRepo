using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Pitch Black (PBL): the cards that weren't already in the pool. Pokémon whose attacks only do their printed
    /// damage need no script: Goldeen, Wailmer, Popplio, Brionne, Charjabug, Litwick, Mankey, Primeape, Vullaby,
    /// Nickit, Maschiff, Type: Null.
    /// </summary>
    public static class CardsPBL
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PBL-1", new TropiusPBL());
            r.RegisterByKey("PBL-2", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("PBL-3", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PBL-4", new LurantisExPBL());
            r.RegisterByKey("PBL-7", new HeatranPBL());
            r.RegisterByKey("PBL-8", new MegaDelphoxExPBL());
            r.RegisterByKey("PBL-9", new ComboMon(new ControlledBurnMon(1), new BugOutMon()));
            r.RegisterByKey("PBL-10", new ComboMon(new ControlledBurnMon(2), new RecoilMon("Heat Tackle", 30)));
            r.RegisterByKey("PBL-11", new TailsNothingMon("Best Punch"));
            r.RegisterByKey("PBL-14", new SeakingPBL());
            r.RegisterByKey("PBL-16", new CardsEVS2.SmackNSlackMon("Falling Down"));
            r.RegisterByKey("PBL-17", new RelicanthPBL());
            r.RegisterByKey("PBL-20", new PrimarinaPBL());
            r.RegisterByKey("PBL-21", new CardsTWM1.HealSelfMon("Draining Fin", 20));
            r.RegisterByKey("PBL-22", new CardsSVI2.PaybackMon("Knuckle Justice", 200));
            r.RegisterByKey("PBL-23", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("PBL-24", new ManectricPBL());
            r.RegisterByKey("PBL-26", new VikavoltPBL());
            r.RegisterByKey("PBL-29", new SlowpokePBL());
            r.RegisterByKey("PBL-31", new MegaSlowbroExPBL());
            r.RegisterByKey("PBL-32", new JynxPBL());
            r.RegisterByKey("PBL-35", new SpiritombPBL());
            r.RegisterByKey("PBL-37", new LampentPBL());
            r.RegisterByKey("PBL-38", new MegaChandelureExPBL());
            r.RegisterByKey("PBL-40", new CardsSVP.GrassKnotMon("Shadowy Knot"));
            r.RegisterByKey("PBL-44", new CardsASC.PushDownMon("Push Down"));
            r.RegisterByKey("PBL-45", new RampardosExPBL());
            r.RegisterByKey("PBL-47", new KoraidonPBL());
            r.RegisterByKey("PBL-50", new MandibuzzPBL());
            r.RegisterByKey("PBL-51", new SearchAttackMon("Procurement", d => d.IsItem, 1));
            r.RegisterByKey("PBL-52", new MalamarPBL());
            r.RegisterByKey("PBL-54", new ThievulPBL());
            r.RegisterByKey("PBL-55", new MorpekoExPBL());
            r.RegisterByKey("PBL-58", new MabosstiffPBL());
            r.RegisterByKey("PBL-60", new SkarmoryPBL());
            r.RegisterByKey("PBL-61", new CardsDRI2.CrunchMon("Smithereen Smash"));
            r.RegisterByKey("PBL-64", new BronzongPBL());
            r.RegisterByKey("PBL-70", new SilvallyPBL());
            r.RegisterByKey("PBL-71", new BombirdierPBL());

            r.RegisterByName("Antique Armor Fossil", new AntiqueArmorFossil());
            r.RegisterByName("Antique Skull Fossil", new CardsASC.CounterattackMon("Spiny Skull", 3, null));
            r.RegisterByName("Backtrack Badge", new BacktrackBadge());
            r.RegisterByName("Dark Bell", new DarkBell());
            r.RegisterByName("Fossil Quarry", new FossilQuarry());
            r.RegisterByName("Jett", new Jett());
            r.RegisterByName("Misty's Vitality", new MistysVitality());
            r.RegisterByName("Rust Syndicate Grunt", new RustSyndicateGrunt());
            r.RegisterByName("Tremendous Bomb", new TremendousBomb());
            r.RegisterByName("Shadowy Darkness Energy", new ShadowyDarknessEnergy());
            r.RegisterByName("Voltaic Lightning Energy", new VoltaicLightningEnergy());
        }

        // ================================================================== Shared helpers

        /// <summary>"During your opponent's next turn, this Pokémon takes N less damage from attacks by Pokémon that match" (N >= 9999: prevent all of it).</summary>
        public sealed class AttackerShieldEffect : ContinuousEffect
        {
            private readonly int amount;
            private readonly Func<PokemonInPlay, bool> attackerFilter;

            public AttackerShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn, int amount, Func<PokemonInPlay, bool> attackerFilter)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                this.amount = amount;
                this.attackerFilter = attackerFilter;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || !attackerFilter(info.Attacker)) return damage;
                return damage - amount;
            }
        }

        /// <summary>Adds an AttackerShieldEffect on the attacking Pokémon for the opponent's next turn (after the damage).</summary>
        public static void ShieldSelfFrom(EffectContext ctx, string attack, int amount, Func<PokemonInPlay, bool> attackerFilter, string from)
        {
            if (!ctx.InPlay(ctx.Pokemon)) return;
            string what = amount >= 9999 ? " takes no damage from attacks by " : " takes " + amount + " less damage from attacks by ";
            ctx.AddEffect(new AttackerShieldEffect(ctx.Pokemon.Name + what + from + " during the opponent's next turn (" + attack + ")", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, amount, attackerFilter));
        }

        /// <summary>"At the end of your opponent's next turn, discard the Defending Pokémon and all attached cards."</summary>
        public sealed class DiscardDefendingLaterEffect : ContinuousEffect
        {
            private readonly string attack;

            public DiscardDefendingLaterEffect(string name, int owner, int targetUid, int currentTurn, string attack)
                : base(name, owner, targetUid, currentTurn + 1, true)
            {
                this.attack = attack;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != ExpiresAfterTurn) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon == null || engine.CurrentEffectPrevented(mon, false)) return;
                engine.Log(Owner, attack + ": " + mon.Name + " is discarded.");
                engine.DiscardPokemonFromPlay(mon);
            }
        }

        /// <summary>Controlled Burn: discard the top N cards of your opponent's deck.</summary>
        public sealed class ControlledBurnMon : Mon
        {
            private readonly int count;

            public ControlledBurnMon(int count) { this.count = count; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Controlled Burn") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Engine.DiscardTopOfDeck(ctx.Opponent, count);
                return true;
            }
        }

        /// <summary>Bug Out: reveal the bottom 7 cards of your deck; N damage for each Pokémon there with the Bug Out attack. Revealed Pokémon are shuffled back, the other cards are discarded.</summary>
        public sealed class BugOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bug Out") return false;
                var deck = ctx.Me.Deck;
                var bottom = deck.Skip(Math.Max(0, deck.Count - 7)).ToList();
                int bugs = 0;
                if (bottom.Count > 0)
                {
                    ctx.Engine.Reveal(ctx.Player, bottom, "revealed the bottom " + bottom.Count + " cards of " + ctx.Engine.Their(ctx.Player) + " deck");
                    bugs = bottom.Count(c => c.Def.IsPokemon && c.Def.Attacks.Any(a => a.Name == "Bug Out"));
                    var others = bottom.Where(c => !c.Def.IsPokemon).ToList();
                    foreach (var c in others)
                    {
                        deck.Remove(c);
                        ctx.Me.Discard.Add(c);
                    }
                    if (others.Count > 0) ctx.Engine.EmitCards(GameEventType.Discard, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " discarded " + others.Count + (others.Count == 1 ? " card" : " cards") + " (Bug Out).", others);
                    ctx.ShuffleMyDeck();
                }
                ctx.DamageDefending(atk.Damage * bugs);
                return true;
            }
        }

        private static bool IsAntiqueItem(CardDef d) { return d.IsItem && d.Name.Contains("Antique"); }

        // ================================================================== Grass

        /// <summary>Fruity Aroma: look at the top 6 cards, put any number of Pokémon there into your hand, shuffle the rest back.</summary>
        private sealed class TropiusPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fruity Aroma") return false;
                var top = ctx.LookAtTop(6);
                var mons = top.Where(c => c.Def.IsPokemon).ToList();
                if (mons.Count > 0)
                {
                    var picks = ctx.ChooseCards(mons, 0, mons.Count, "Fruity Aroma: put any number of these Pokémon into your hand");
                    RecifeTrainers.TakeIntoHand(ctx, picks);
                }
                if (top.Count > 0) ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Lively Cutter: 200 more if this Pokémon was healed this turn. Leaf Guard: takes 50 less next turn.</summary>
        private sealed class LurantisExPBL : Mon
        {
            private static readonly Mon LeafGuard = new CardsASC.SelfGuardMon("Leaf Guard", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lively Cutter")
                {
                    bool healed = ctx.Pokemon.HealedOnTurn == ctx.Turn;
                    ctx.DamageDefending(atk.Damage + (healed ? 200 : 0));
                    return true;
                }
                return LeafGuard.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Fire

        /// <summary>Singe: Burned. Lava Wall: during the opponent's next turn, prevent all damage from attacks by Burned Pokémon.</summary>
        private sealed class HeatranPBL : Mon
        {
            private static readonly Mon Singe = new ConditionAttackMon("Singe", SpecialCondition.Burned);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Lava Wall")
                {
                    ctx.DamageDefending(atk.Damage);
                    ShieldSelfFrom(ctx, atk.Name, 9999, m => m.Has(SpecialCondition.Burned), "Burned Pokémon");
                    return true;
                }
                return Singe.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Trick Portal: look at the top 9 cards and put any number of Basic Pokémon there onto your Bench. Eerie Glow: Burned and Confused.</summary>
        private sealed class MegaDelphoxExPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Trick Portal")
                {
                    var top = ctx.LookAtTop(9);
                    var basics = top.Where(c => c.Def.IsBasicPokemon).ToList();
                    int room = ctx.BenchSpace;
                    if (basics.Count > 0 && room > 0)
                    {
                        var picks = ctx.ChooseCards(basics, 0, Math.Min(room, basics.Count), "Trick Portal: put any number of these Basic Pokémon onto your Bench");
                        foreach (var c in picks)
                        {
                            if (ctx.BenchSpace <= 0) break;
                            ctx.Me.Deck.Remove(c);
                            ctx.Engine.PutOntoBench(ctx.Player, c, false);
                        }
                    }
                    if (top.Count > 0) ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Eerie Glow")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending))
                    {
                        ctx.ApplyCondition(defending, SpecialCondition.Burned);
                        ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    }
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Water

        /// <summary>Hydro Jet: 30 damage to 1 of the opponent's Pokémon for each Water Energy attached to this Pokémon.</summary>
        private sealed class SeakingPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hydro Jet") return false;
                int water = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Water);
                if (water <= 0) return Nothing(ctx);
                ctx.DamageAnyOpponent(30 * water);
                return true;
            }
        }

        /// <summary>Fossil Beatdown: 30 more for each of your Benched Pokémon with "Antique" in its name.</summary>
        private sealed class RelicanthPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fossil Beatdown") return false;
                int fossils = ctx.MyBench().Count(m => m.Name.Contains("Antique"));
                ctx.DamageDefending(atk.Damage + 30 * fossils);
                return true;
            }
        }

        /// <summary>Enriching Melody: when evolved from hand, heal all damage from 1 of your Pokémon. Aqua Return: shuffle this Pokémon and its cards into your deck.</summary>
        private sealed class PrimarinaPBL : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Enriching Melody", CardsMEG.AnyHurt(ctx), "Enriching Melody: heal all damage from 1 of your Pokémon?")) return;
                CardsMEG.HealOneOfMine(ctx, 10000, "Enriching Melody");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Aqua Return") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Flashing Barrier: prevent damage from Evolution Pokémon next turn. Sonic Edge: ignores effects on the Defending Pokémon.</summary>
        private sealed class ManectricPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Flashing Barrier")
                {
                    ctx.DamageDefending(atk.Damage);
                    ShieldSelfFrom(ctx, atk.Name, 9999, m => m.Def.IsEvolution, "Evolution Pokémon");
                    return true;
                }
                if (atk.Name == "Sonic Edge")
                {
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, true, true, true);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Quick Dive: 50 to 1 of the opponent's Pokémon. Giga Railgun: does nothing without Voltaic Lightning Energy attached.</summary>
        private sealed class VikavoltPBL : Mon
        {
            private static readonly Mon QuickDive = new CardsMEG.SnipeMon("Quick Dive", 50);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Giga Railgun")
                {
                    if (!ctx.Pokemon.Energy.Any(e => e.Def.Name == "Voltaic Lightning Energy")) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return QuickDive.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Psychic

        /// <summary>All-You-Can-Yeet: discard any number of cards from your hand.</summary>
        private sealed class SlowpokePBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "All-You-Can-Yeet") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, hand.Count, "All-You-Can-Yeet: discard any number of cards from your hand");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                return true;
            }
        }

        /// <summary>Shellnado Spin: during the opponent's next turn, if this Pokémon is damaged by an attack, 12 damage counters on the Attacking Pokémon.</summary>
        private sealed class MegaSlowbroExPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shellnado Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new RetaliateEffect(ctx.Pokemon.Name + " hits back with 12 damage counters during the opponent's next turn (Shellnado Spin)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 12));
                return true;
            }
        }

        /// <summary>Intense Kiss: at the end of the opponent's next turn, discard the Defending Pokémon and its cards. Psy Bolt: flip, Paralyzed.</summary>
        private sealed class JynxPBL : Mon
        {
            private static readonly Mon PsyBolt = new ParalyzeFlipMon("Psy Bolt");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Intense Kiss")
                {
                    var defending = ctx.Defending;
                    if (defending == null || !ctx.CanAffect(defending)) return true;
                    ctx.AddEffect(new DiscardDefendingLaterEffect(defending.Name + " will be discarded at the end of its owner's next turn (Intense Kiss)", ctx.Player, defending.Uid, ctx.Turn, atk.Name));
                    return true;
                }
                return PsyBolt.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Spiritual End: with 13 or more Hide 'n' Sneak Pokémon in your discard pile, quadruple the damage counters on 2 of the opponent's Pokémon.</summary>
        private sealed class SpiritombPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiritual End") return false;
                if (HideNSneakInDiscard(ctx) < 13) return Nothing(ctx);
                var candidates = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (candidates.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(candidates, Math.Min(2, candidates.Count), 2, "Spiritual End: choose 2 of your opponent's Pokémon to quadruple their damage counters");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.PlaceCounters(mon, mon.DamageCounters * 3);
                return true;
            }
        }

        /// <summary>Spreading Light: search your deck for up to 3 Lampent and put them onto your Bench.</summary>
        private sealed class LampentPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spreading Light") return false;
                int room = Math.Min(3, ctx.BenchSpace);
                if (room <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.Name == "Lampent", room, "Spreading Light: choose up to " + room + " Lampent to put onto your Bench");
                foreach (var c in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Binding Flame: the opponent's Active Pokémon's Retreat Cost is 1 more. Phantom Maze: 50 more for each Colorless in its Retreat Cost.</summary>
        private sealed class MegaChandelureExPBL : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner == src.Player || !src.State.IsActive(mon)) return cost;
                return cost + 1;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Phantom Maze") return false;
                int retreat = ctx.Defending == null ? 0 : ctx.Engine.RetreatCostOf(ctx.Defending);
                ctx.DamageDefending(atk.Damage + 50 * retreat);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Destructive Headbutting: once during your turn while Active, flip; heads discard an Energy from the opponent's Active. Rowdy Hammer: 150 more next turn.</summary>
        private sealed class RampardosExPBL : Mon
        {
            protected override string Activated { get { return "Destructive Headbutting"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && ctx.Defending != null && ctx.Defending.Energy.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (!ctx.Flip() || defending == null || !ctx.CanAffect(defending)) return;
                ctx.DiscardEnergyFrom(defending, 1, "Destructive Headbutting: discard an Energy from " + defending.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rowdy Hammer") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                {
                    var boost = new CardsCRI.NextTurnPowerEffect(ctx.Pokemon.Name + "'s attacks do 150 more damage during its next turn (Rowdy Hammer)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 2, 150);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                }
                return true;
            }
        }

        /// <summary>Battle Claw: 30 more against an Evolution Pokémon. Gaia Impact: discard all Energy from this Pokémon.</summary>
        private sealed class KoraidonPBL : Mon
        {
            private static readonly Mon GaiaImpact = new CardsTWM1.SelfDiscardMon("Gaia Impact", 99);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Battle Claw")
                {
                    bool evolved = ctx.Defending != null && ctx.Defending.Def.IsEvolution;
                    ctx.DamageDefending(atk.Damage + (evolved ? 30 : 0));
                    return true;
                }
                return GaiaImpact.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Darkness

        /// <summary>Bone Sniper: 70 damage to 1 of the opponent's Pokémon that has any Special Energy attached.</summary>
        private sealed class MandibuzzPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bone Sniper") return false;
                var targets = ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
                var target = ctx.ChoosePokemon(targets, "Bone Sniper: do 70 damage to which of your opponent's Pokémon?");
                if (target == null) return Nothing(ctx);
                ctx.DamageTarget(target, 70);
                return true;
            }
        }

        /// <summary>Perplex: Confused. Brain Crush: does nothing unless the opponent's Active Pokémon is Confused.</summary>
        private sealed class MalamarPBL : Mon
        {
            private static readonly Mon Perplex = new ConditionAttackMon("Perplex", SpecialCondition.Confused);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Brain Crush")
                {
                    if (ctx.Defending == null || !ctx.Defending.Has(SpecialCondition.Confused)) return Nothing(ctx);
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return Perplex.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Skill Thief: with no cards in your hand, use an attack of 1 of the opponent's Pokémon in play.</summary>
        private sealed class ThievulPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Skill Thief") return false;
                if (ctx.Me.Hand.Count > 0) return Nothing(ctx);
                var withAttacks = ctx.OpponentPokemon(m => m.Def.Attacks.Any(a => a.Name != "Skill Thief") || (m.Def.IsLevelUp && m.Stack.Count >= 2));
                var from = ctx.ChoosePokemon(withAttacks, "Skill Thief: use an attack of which of your opponent's Pokémon?");
                if (from == null) return Nothing(ctx);
                CardDef def;
                int index;
                if (!ChooseCopiedAttack(ctx, from, "Skill Thief: choose 1 of " + from.Name + "'s attacks to use", out def, out index)) return Nothing(ctx);
                if (def.Attacks[index].Name == "Skill Thief") return Nothing(ctx);
                UseCopiedAttack(ctx, def, index);
                return true;
            }
        }

        /// <summary>Wheely Draw: shuffle your hand into your deck, draw 6. Hangry Blaster: 40 more for each damage counter on this Pokémon.</summary>
        private sealed class MorpekoExPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Wheely Draw")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(6);
                    return true;
                }
                if (atk.Name == "Hangry Blaster")
                {
                    ctx.DamageDefending(atk.Damage + 40 * ctx.Pokemon.DamageCounters);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Plunging Headbutt: during the opponent's next turn, this Pokémon takes 100 more damage from attacks.</summary>
        private sealed class MabosstiffPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Plunging Headbutt") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new DamageTakenEffect(ctx.Pokemon.Name + " takes 100 more damage during the opponent's next turn (Plunging Headbutt)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 100));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Steel Cutter: discard up to 2 Basic Metal Energy cards from your hand, 40 damage for each.</summary>
        private sealed class SkarmoryPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Steel Cutter") return false;
                var metal = ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Metal)).ToList();
                var picks = metal.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(metal, 0, Math.Min(2, metal.Count), "Steel Cutter: discard up to 2 Basic Metal Energy cards from your hand (" + atk.Damage + " damage each)");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                ctx.DamageDefending(atk.Damage * picks.Count);
                return true;
            }
        }

        /// <summary>Metal Block: during the opponent's next turn, this Pokémon takes 100 less damage from attacks by Evolution Pokémon.</summary>
        private sealed class BronzongPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Metal Block") return false;
                ctx.DamageDefending(atk.Damage);
                ShieldSelfFrom(ctx, atk.Name, 100, m => m.Def.IsEvolution, "Evolution Pokémon");
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Call a Buddy: once during your turn with no cards in your hand, search your deck for a Supporter. Air Slash: discard an Energy.</summary>
        private sealed class SilvallyPBL : Mon
        {
            private static readonly Mon AirSlash = new CardsTWM1.SelfDiscardMon("Air Slash", 1);

            protected override string Activated { get { return "Call a Buddy"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count == 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsSupporter, 1, "Call a Buddy: choose a Supporter card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return AirSlash.HandleAttack(ctx, atk); }
        }

        /// <summary>Challenging Delivery: flip 2 coins; if both are heads, search your deck for a Basic Pokémon and put it onto your Bench.</summary>
        private sealed class BombirdierPBL : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Challenging Delivery") return false;
                if (ctx.FlipCoins(2) < 2) return true;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => true, 1, "Challenging Delivery: choose a Basic Pokémon to put onto your Bench");
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Protective Armor: while this fossil is Active, your Pokémon take 10 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class AntiqueArmorFossil : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || !src.IsActive(me) || info.Target.Owner != src.Player || !info.AgainstOpponent) return damage;
                return damage - 10;
            }
        }

        /// <summary>Tool: once during your turn, the Colorless holder's attack coin flips may be ignored and flipped again (the engine asks).</summary>
        private sealed class BacktrackBadge : CardScript
        {
            public override bool OffersAttackReflip(EffectContext src)
            {
                return src.Pokemon != null && src.Engine.IsType(src.Pokemon, EnergyType.Colorless);
            }
        }

        /// <summary>Both Active Pokémon that aren't Darkness Pokémon are now Confused.</summary>
        private sealed class DarkBell : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                var list = new List<PokemonInPlay>();
                foreach (var m in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (m != null && !ctx.Engine.IsType(m, EnergyType.Darkness)) list.Add(m);
                return list;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                foreach (var m in Targets(ctx)) ctx.ApplyCondition(m, SpecialCondition.Confused);
            }
        }

        /// <summary>Stadium: once during each player's turn, that player may search their deck for up to 2 Antique Item cards and put them onto their Bench.</summary>
        private sealed class FossilQuarry : StadiumAction
        {
            protected override bool Ready(EffectContext ctx)
            {
                return ctx.BenchSpace > 0 && ctx.Me.Deck.Any(c => IsAntiqueItem(c.Def));
            }

            protected override bool Use(EffectContext ctx)
            {
                int room = Math.Min(2, ctx.BenchSpace);
                var found = ctx.SearchMyDeck(IsAntiqueItem, room, "Fossil Quarry: choose up to " + room + " Antique Item cards to put onto your Bench");
                foreach (var c in found)
                {
                    if (ctx.BenchSpace > 0) ctx.Engine.PutOntoBench(ctx.Player, c, false);
                    else ctx.Me.Deck.Add(c);
                }
                ctx.ShuffleMyDeck();
                return false;
            }
        }

        /// <summary>Draw a card for each of your opponent's Mega Evolution Pokémon ex in play.</summary>
        private sealed class Jett : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.OpponentPokemon(m => m.Def.IsMegaEx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(ctx.OpponentPokemon(m => m.Def.IsMegaEx).Count);
            }
        }

        /// <summary>Search your deck for up to 4 Basic Water Energy and attach them to 1 of your Pokémon. Your turn ends.</summary>
        private sealed class MistysVitality : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Any(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Water), 4, "Misty's Vitality: choose up to 4 Basic Water Energy cards (your turn then ends)");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Misty's Vitality: attach the Energy to which of your Pokémon?");
                    if (target == null) ctx.Me.Deck.AddRange(found);
                    else foreach (var c in found) ctx.Engine.AttachEnergyCard(c, target);
                }
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Only if any of your Pokémon were Knocked Out during your opponent's last turn: discard an Energy from 1 of your opponent's Pokémon.</summary>
        private sealed class RustSyndicateGrunt : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.WasKnockedOutLastOpponentTurn() && ctx.OpponentPokemon(m => m.Energy.Count > 0).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var withEnergy = ctx.OpponentPokemon(m => m.Energy.Count > 0);
                var target = ctx.ChoosePokemon(withEnergy, "Rust Syndicate Grunt: discard an Energy from which of your opponent's Pokémon?");
                if (target == null) return;
                ctx.DiscardEnergyFrom(target, 1, "Rust Syndicate Grunt: choose an Energy to discard");
            }
        }

        /// <summary>Tool: if the non-Mega holder in the Active Spot takes 240+ damage from an opponent's Mega Evolution Pokémon ex's attack, 12 damage counters on the Attacking Pokémon; then discard this card.</summary>
        private sealed class TremendousBomb : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                var holder = ctx.Pokemon;
                if (holder == null || holder.Def.IsMegaEx || !holderWasActive || damage < 240) return;
                if (attacker == null || attacker.Owner == ctx.Player || !attacker.Def.IsMegaEx || !ctx.InPlay(attacker)) return;
                int placed = ctx.Engine.PlaceDamageCounters(attacker, 12, "Tremendous Bomb");
                if (placed > 0 && holder.HasTool(ctx.Card)) ctx.Engine.DiscardTool(holder, ctx.Card);
            }
        }

        /// <summary>Provides Darkness. While its Darkness Pokémon is on the Bench, prevent all damage done to it by the opponent's attacks.</summary>
        private sealed class ShadowyDarknessEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Darkness), 1); }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && !info.TargetIsActive && info.AgainstOpponent
                    && src.Engine.IsType(src.Pokemon, EnergyType.Darkness);
            }
        }

        /// <summary>Provides Lightning. Attacks of its Lightning Pokémon do 20 more damage to the opponent's Active Pokémon (before Weakness and Resistance).</summary>
        private sealed class VoltaicLightningEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.Of(EnergyType.Lightning), 1); }

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return src.Engine.IsType(src.Pokemon, EnergyType.Lightning) ? damage + 20 : damage;
            }
        }
    }
}
