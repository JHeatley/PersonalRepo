using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Primal Clash (PRC, v101), part 2: the Lightning, Psychic, Fighting, Darkness, Metal, Fairy, Dragon and Colorless Pokémon
    /// (61-121). Rhyhorn (74) is Generations' card; Gardevoir-EX and M Gardevoir-EX were already in the pool.
    /// </summary>
    public static class CardsPRC2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PRC-61", new ComboMon(new SwitchAfterMon("Lightning Turn"), new ElectricShockMon()));
            r.RegisterByKey("PRC-62", new FlipBonusMon("Water Splash", 10));
            r.RegisterByKey("PRC-63", new CoinsMon("Buzz Flip", 4));
            r.RegisterByKey("PRC-64", new CardsGRI2.FlipBonusOrRecoilMon("Thrash", 20, 20));
            r.RegisterByKey("PRC-65", new EelektrossConnect());
            r.RegisterByKey("PRC-68", new ComboMon(new CoinsMon("Double Kick", 2), new CardsBS1.ToxicMon("Poison Fang", 2)));
            r.RegisterByKey("PRC-69", new ComboMon(new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned), new RecoilMon("Dynamite Punch", 20)));
            r.RegisterByKey("PRC-70", new CardsPAF1.ReturnSelfMon("Lost in the Waves"));
            r.RegisterByKey("PRC-72", new ComboMon(new CardsROS1.ConditionsAttackMon("Dancing Tentacles", SpecialCondition.Confused, SpecialCondition.Poisoned), new StickAndAbsorbMon()));
            r.RegisterByKey("PRC-73", new CardsFST2.ConditionBonusMon("Synchro Star", 60, c => c.Defending != null && c.EnergyCount(c.Pokemon) == c.EnergyCount(c.Defending)));
            r.RegisterByKey("PRC-75", new RecoilMon("Take Down", 10));
            r.RegisterByKey("PRC-76", new ComboMon(new RockShowerMon(), new FlipBonusMon("Stone Edge", 40)));
            r.RegisterByKey("PRC-77", new RhyperiorWall());
            r.RegisterByKey("PRC-78", new ShieldAttackMon("Stiffen", 40));
            r.RegisterByKey("PRC-80", new ComboMon(new CardsLOR1.CountersAnyWayMon("Pure Power", 4), new ConditionAttackMon("Windmill Kick", SpecialCondition.Confused)));
            r.RegisterByKey("PRC-81", new ComboMon(new CardsTWM1.HealSelfMon("Calm Mind", 30), new CardsFST1.NoWeaknessResistanceMon("Yoga Kick")));
            r.RegisterByKey("PRC-83", new ComboMon(new SearchAttackMon("Solar Generator", d => d.IsSpecialEnergy, 2), new FlipBonusMon("Knock Away", 20)));
            r.RegisterByKey("PRC-84", new ComboMon(new FlipBonusMon("Rock Smash", 20), new CardsBUS1.OwnBenchQuakeMon("Break Ground", 10)));
            r.RegisterByKey("PRC-85", new CardsASC.FlipDiscardDefendingEnergyMon("Rip Claw"));
            r.RegisterByKey("PRC-86", new CardsAOR2.StadiumBlowMon("Gaia Volcano", 100));
            r.RegisterByKey("PRC-88", new ComboMon(new ResistanceDesertMon(), new RecoilMon("Double-Edge", 20)));
            r.RegisterByKey("PRC-89", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("PRC-90", new EarDigMon());
            r.RegisterByKey("PRC-91", new ComboMon(new CardsWHT.DragOffMon("Hunt", 30), new JaggedFangMon()));
            r.RegisterByKey("PRC-92", new CrawdauntClaw());
            r.RegisterByKey("PRC-93", new ComboMon(new FlipBonusMon("Steel Headbutt", 30), new Cards30C.SelfCounterBonusMon("Raging Hammer", 10)));
            r.RegisterByKey("PRC-94", new MegatonSlamMon());
            r.RegisterByKey("PRC-95", new ComboMon(new CoinsMon("Triple Smash", 3), new CardsASC.ToolBonusMon("Reinforced Nose", 50)));
            r.RegisterByKey("PRC-96", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Drill Run"), new StraightClawMon()));
            r.RegisterByKey("PRC-97", new ComboMon(new DredgeMon(), new CardsFLF1.NoResistanceMon("Mach Claw")));
            r.RegisterByKey("PRC-98", new ConditionAttackMon("Metal Sound", SpecialCondition.Confused));
            r.RegisterByKey("PRC-99", new FalseSwipeMon());
            r.RegisterByKey("PRC-100", new AegislashGuard());
            r.RegisterByKey("PRC-101", new ComboMon(new TrickMon(), new NoRetreatMon("Lock Up")));
            r.RegisterByKey("PRC-103", new ComboMon(new ParalyzeFlipMon("Bubble Beam"), new CardsSUM5.OptionalBoostRecoilMon("Superpower", 30, 30)));
            r.RegisterByKey("PRC-104", new ComboMon(new CoinsMon("Tail Rap", 2), new CardsUPR1.LessPerSelfCounterMon("Dwindling Wave", 10)));
            r.RegisterByKey("PRC-107", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsUNM3.TwisterMon()));
            r.RegisterByKey("PRC-108", new ComboMon(new SearchAttackMon("Gather Strength", d => d.IsBasicEnergy, 4), new DragonBlastMon()));
            r.RegisterByKey("PRC-109", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("PRC-110", new FlygonFlap());
            r.RegisterByKey("PRC-111", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("PRC-112", new ComboMon(new CoinsMon("Fury Swipes", 3), new CardsROS1.BenchOnlyMon("Dash Attack", 30)));
            r.RegisterByKey("PRC-113", new CardsMEG.WeakenDefendingMon("Charm", 20));
            r.RegisterByKey("PRC-114", new ComboMon(new ReplaceMon(), new FlipBonusMon("Play Rough", 30)));
            r.RegisterByKey("PRC-115", new ComboMon(new StaggeringStepsMon(), new CardsMEW1.SpreadBenchMon("Uproar", 10)));
            r.RegisterByKey("PRC-116", new CardsSP1.FlipNoDamageMon("Scrunch"));
            r.RegisterByKey("PRC-117", new CardsFST2.ConditionBonusMon("Drench", 20, c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Water) > 0));
            r.RegisterByKey("PRC-118", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt")));
            r.RegisterByKey("PRC-119", new BouffalantSipper());
            r.RegisterByKey("PRC-120", new FlipBonusMon("Trip Over", 30));
            r.RegisterByKey("PRC-121", new ComboMon(new CardsSMP4.MillOpponentMon("Burrow", 1), new CardsCEC2.RototillerMon()));
        }

        // ================================================================== Lightning

        /// <summary>"Switch this Pokémon with 1 of your Benched Pokémon" after the damage (Lightning Turn).</summary>
        internal sealed class SwitchAfterMon : Mon
        {
            private readonly string attack;

            public SwitchAfterMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Electric Shock: discard all Lightning Energy from this Pokémon; the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class ElectricShockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Shock") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    foreach (var en in me.Energy.Where(e => CardsDRI1.ProvidesType(ctx, me, e, EnergyType.Lightning)).ToList()) ctx.Engine.DiscardEnergy(me, en);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Energy Connect: as often as you like during your turn, move a basic Energy from 1 of your Benched Pokémon to your Active
        /// Pokémon. Electricannon: you may discard all Lightning Energy from this Pokémon for 50 more damage.
        /// </summary>
        private sealed class EelektrossConnect : Mon
        {
            private static bool Ready(EffectContext ctx)
            {
                return ctx.Me.Active != null && ctx.Me.Bench.Any(m => m.Energy.Any(en => en.Def.IsBasicEnergy));
            }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                return ctx.Pokemon.Def.Abilities[abilityIndex].Name == "Energy Connect" && ctx.IsMyTurn && Ready(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => Ready(ctx), offerDone =>
                {
                    var active = ctx.Me.Active;
                    var holders = ctx.Me.Bench.Where(m => m.Energy.Any(en => en.Def.IsBasicEnergy)).ToList();
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, holders, "Energy Connect: move a basic Energy from which Benched Pokémon to "
                                                                     + active.Name + "?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var energy = giver.Energy.Where(en => en.Def.IsBasicEnergy).ToList();
                    var card = energy.Select(en => en.Def.Name).Distinct().Count() == 1 ? energy[0]
                        : ctx.ChooseCards(energy, 1, 1, "Energy Connect: move which Energy from " + giver.Name + "?")[0];
                    return ctx.Engine.MoveEnergy(giver, active, card);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electricannon") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning)).ToList();
                bool boost = lightning.Count > 0 && ctx.AskYesNo("Electricannon: discard all Lightning Energy from " + me.Name + " for 50 more damage?");
                if (boost)
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                ctx.DamageDefending(atk.Damage + (boost ? 50 : 0));
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Stick and Absorb: heal 30 from this Pokémon; the Defending Pokémon can't retreat during the opponent's next turn.</summary>
        private sealed class StickAndAbsorbMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stick and Absorb") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                ctx.DefendingCantRetreat(defending);
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Rock Shower: flip 3 coins; 20 damage for each heads to each of the opponent's Pokémon.</summary>
        private sealed class RockShowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Shower") return false;
                int amount = 20 * ctx.FlipCoins(3);
                if (amount == 0)
                {
                    ctx.Log("Rock Shower did no damage.");
                    return true;
                }
                var bench = ctx.Opp.Bench.ToList();
                ctx.DamageDefending(amount);
                foreach (var mon in bench)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, amount);
                return true;
            }
        }

        /// <summary>Rock Wall: damage to your Pokémon from the opponent's attacks is reduced by 10 (after Weakness and Resistance). Hammer Arm: mill 1.</summary>
        private sealed class RhyperiorWall : Mon
        {
            private static readonly Mon Arm = new CardsSSP2.StompOffMon("Hammer Arm", 1);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                return damage - 10;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Arm.HandleAttack(ctx, atk); }
        }

        /// <summary>During your opponent's next turn, no effects of attacks, damage included, from Pokémon-EX reach this Pokémon (Resistance Desert).</summary>
        internal sealed class ExShieldEffect : ContinuousEffect
        {
            public ExShieldEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                return info.Target != null && info.Target.Uid == TargetUid && info.Attacker != null && info.Attacker.Owner != Owner && info.Attacker.Def.IsPokemonEX;
            }

            public override bool PreventsEffects(GameEngine engine, PokemonInPlay target, EffectSourceKind kind)
            {
                if (target == null || target.Uid != TargetUid || kind != EffectSourceKind.Attack) return false;
                var attacker = engine.State.Players[1 - Owner].Active;
                return attacker != null && attacker.Def.IsPokemonEX;
            }
        }

        private sealed class ResistanceDesertMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Resistance Desert") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me))
                    ctx.AddEffect(new ExShieldEffect(me.Name + " is protected from the attacks of Pokémon-EX during the opponent's next turn (Resistance Desert)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Ear Dig: flip a coin until you get tails; discard the top card of the opponent's deck for each heads.</summary>
        private sealed class EarDigMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ear Dig") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = 0;
                while (ctx.Flip()) heads++;
                if (heads > 0 && ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Jagged Fang: discard an Energy from this Pokémon and an Energy from the opponent's Active Pokémon.</summary>
        private sealed class JaggedFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jagged Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 1, atk.Name);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0 && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 1, "Jagged Fang: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Unruly Claw: when it evolves from the hand, you may discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class CrawdauntClaw : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                bool ready = d != null && d.Energy.Count > 0 && ctx.CanAffect(d);
                if (!TriggerOnce(ctx, "Unruly Claw", ready, "Unruly Claw: discard an Energy attached to your opponent's Active Pokémon?")) return;
                ctx.DiscardEnergyFrom(d, 1, "Unruly Claw: discard an Energy from " + d.Name);
            }
        }

        // ================================================================== Metal

        /// <summary>Megaton Slam: you may flip a coin; heads, 120 more damage; tails, 20 damage to each of your Benched Pokémon.</summary>
        private sealed class MegatonSlamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Megaton Slam") return false;
                if (!ctx.AskYesNo("Megaton Slam: flip a coin? (Heads: 120 more damage. Tails: 20 damage to each of your Benched Pokémon.)"))
                {
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 120 : 0));
                if (!heads)
                    foreach (var mon in ctx.Me.Bench.ToList())
                        if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Straight Claw: you may discard an Energy from this Pokémon for 30 more damage.</summary>
        private sealed class StraightClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Straight Claw") return false;
                var me = ctx.Pokemon;
                bool boost = me.Energy.Count > 0 && ctx.AskYesNo("Straight Claw: discard an Energy from " + me.Name + " for 30 more damage?");
                if (boost) boost = ctx.DiscardEnergyFrom(me, 1, "Straight Claw: discard an Energy from " + me.Name) > 0;
                ctx.DamageDefending(atk.Damage + (boost ? 30 : 0));
                return true;
            }
        }

        /// <summary>Dredge: search your deck for 2 Energy cards and attach them to this Pokémon.</summary>
        private sealed class DredgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dredge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(me)) return true;
                var found = ctx.SearchMyDeck(d => d.IsEnergy, 2, "Dredge: choose 2 Energy cards to attach to " + me.Name);
                foreach (var card in found)
                    if (!ctx.Engine.AttachEnergyCard(card, me)) ctx.Me.Discard.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>False Swipe: flip a coin; heads, put damage counters on the opponent's Active Pokémon until it has 10 HP left.</summary>
        private sealed class FalseSwipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "False Swipe") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null || !ctx.Flip()) return true;
                int counters = ctx.RemainingHp(defending) / 10 - 1;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Miracle Guard: each of your Pokémon has no Weakness. Protect Charge: takes 20 less next turn.</summary>
        private sealed class AegislashGuard : Mon
        {
            private static readonly Mon Charge = new CardsASC.SelfGuardMon("Protect Charge", 20);

            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && mon.Owner == src.Player; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Charge.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fairy

        /// <summary>
        /// Trick: move a Pokémon Tool from 1 of either player's Pokémon to another of that player's Pokémon without a Tool (nothing
        /// happens with no such Pokémon). A Tool that can't be attached to its new Pokémon (a Team Flare Tool moved onto a Pokémon
        /// that isn't a Pokémon-EX) is discarded (Compendium, 2015-02-05).
        /// </summary>
        private sealed class TrickMon : Mon
        {
            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                var list = new List<PokemonInPlay>();
                for (int side = 0; side < 2; side++)
                {
                    var all = ctx.State.Players[side].InPlay().ToList();
                    list.AddRange(all.Where(m => m.Tools.Count > 0 && all.Any(o => o != m && o.Tools.Count == 0)));
                }
                return list;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trick") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var holders = Holders(ctx);
                if (holders.Count == 0) return true;
                var giver = ctx.ChoosePokemon(holders, "Trick: move a Pokémon Tool from which Pokémon?");
                if (giver == null || (giver.Owner != ctx.Player && !ctx.CanAffect(giver))) return true;
                var tool = giver.Tools.Count == 1 ? giver.Tools[0] : ctx.ChooseCards(giver.Tools.ToList(), 1, 1, "Trick: move which Pokémon Tool?")[0];
                var receivers = ctx.State.Players[giver.Owner].InPlay().Where(m => m != giver && m.Tools.Count == 0).ToList();
                var receiver = ctx.ChoosePokemon(receivers, "Trick: move " + tool.Def.Name + " to which Pokémon?");
                if (receiver == null || (receiver.Owner != ctx.Player && !ctx.CanAffect(receiver))) return true;
                if (!ctx.Engine.CanAttachToolTo(tool, receiver))
                {
                    ctx.Engine.DiscardTool(giver, tool);
                    return true;
                }
                giver.Tools.Remove(tool);
                ctx.Engine.AttachTool(tool, receiver);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Dragon Blast: discard a Water Energy and a Lightning Energy from this Pokémon.</summary>
        private sealed class DragonBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Blast") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                ctx.DiscardEnergyFrom(me, 1, "Dragon Blast: discard a Water Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Water));
                ctx.DiscardEnergyFrom(me, 1, "Dragon Blast: discard a Lightning Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Lightning));
                return true;
            }
        }

        /// <summary>Sand Flap: once during your turn, choose a player; they shuffle their hand into their deck and draw 4 cards.</summary>
        private sealed class FlygonFlap : Mon
        {
            private static readonly Mon Tomb = new NoRetreatMon("Sand Tomb");

            protected override string Activated { get { return "Sand Flap"; } }

            protected override void Ability(EffectContext ctx)
            {
                int pick = ctx.ChooseOption("Sand Flap: which player shuffles their hand into their deck and draws 4 cards?", "You", "Your opponent");
                int player = pick == 0 ? ctx.Player : ctx.Opponent;
                ctx.Engine.ShuffleHandIntoDeck(player);
                ctx.Engine.DrawCards(player, 4);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tomb.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Replace: move as many Energy attached to your Pokémon to your other Pokémon as you like, in any way.</summary>
        internal sealed class ReplaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Replace") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                RepeatableAbility.Repeat(ctx, () => ctx.MyPokemon().Count >= 2 && ctx.MyPokemon().Any(m => m.Energy.Count > 0), offerDone =>
                {
                    var givers = ctx.MyPokemon().Where(m => m.Energy.Count > 0).ToList();
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, givers, "Replace: move an Energy from which of your Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (giver == null) return false;
                    var card = giver.Energy.Count == 1 ? giver.Energy[0] : ctx.ChooseCards(giver.Energy.ToList(), 1, 1, "Replace: move which Energy from " + giver.Name + "?")[0];
                    var others = ctx.MyPokemon().Where(m => m != giver).ToList();
                    var receiver = ctx.ChoosePokemon(others, "Replace: move " + card.Def.Name + " to which Pokémon?");
                    return receiver != null && ctx.Engine.MoveEnergy(giver, receiver, card);
                });
                return true;
            }
        }

        /// <summary>Staggering Steps: flip a coin; heads, the opponent's Active Pokémon is now Confused; tails, this Pokémon is now Confused.</summary>
        private sealed class StaggeringStepsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Staggering Steps") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                {
                    if (ctx.Defending != null) ctx.ApplyCondition(ctx.Defending, SpecialCondition.Confused);
                }
                else if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Sap Sipper: this Pokémon's attacks do 40 more damage to the opponent's Grass Pokémon (before Weakness and Resistance). Derail.</summary>
        private sealed class BouffalantSipper : Mon
        {
            private static readonly Mon Derail = new CardsASR1.DiscardDefendingSpecialMon("Derail", false);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.AgainstOpponent || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Grass) ? damage + 40 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Derail.HandleAttack(ctx, atk); }
        }
    }
}
