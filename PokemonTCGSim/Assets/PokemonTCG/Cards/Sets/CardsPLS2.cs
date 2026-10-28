using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Plasma Storm (PLS, v111): Psychic to Colorless Pokémon (59-116) and the Trainers. Lugia-EX is Legendary Treasures' card;
    /// Escape Rope, Scramble Switch, Random Receiver, Charizard, Blastoise and Plasma Energy were already in the pool. Your rulings
    /// (v111): Moon Guidance ignores evolution timing; Silent Claw may discard any Supporter, using its effect only if it can be.
    /// </summary>
    public static class CardsPLS2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("PLS-59", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PLS-60", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("PLS-61", new ComboMon(new CardsCRZ1.PowerfulStormMon(), new CardsASC.PushDownMon("Swift Lunge")));
            r.RegisterByKey("PLS-62", new ComboMon(new CardsFST2.ConditionBonusMon("Hex", 50, c => c.Defending != null && c.Defending.Conditions != SpecialCondition.None),
                new CardsASC.RandomDiscardMon("Shadow Claw")));
            r.RegisterByKey("PLS-63", new PileUpMon());
            r.RegisterByKey("PLS-65", new CardsASR1.TimesMon("Tool Drop", ToolsInPlay));
            r.RegisterByKey("PLS-66", new BiosmogMon());
            r.RegisterByKey("PLS-67", new ComboMon(new CardsASR1.TimesMon("Ensnarl", c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending)), new CoinsMon("Double Ducts", 2)));
            r.RegisterByKey("PLS-69", new ComboMon(new CardsASC.SelfGuardMon("Reflect", 20), new TailsNothingMon("Smash Punch")));
            r.RegisterByKey("PLS-70", new ComboMon(new NoRetreatMon("Lock Up"), new DamakinesisMon()));
            r.RegisterByKey("PLS-72", new ComboMon(new SpinningTurnMon(), new CardsAOR2.StadiumBlowMon("Wreck", 60)));
            r.RegisterByKey("PLS-73", new LunatonePremonition());
            r.RegisterByKey("PLS-74", new ComboMon(new ConditionAttackMon("Heat Burn", SpecialCondition.Burned), new RecoilMon("Explosion", 90)));
            r.RegisterByKey("PLS-76", new CardsFLF1.NoResistanceMon("Feint"));
            r.RegisterByKey("PLS-78", new LucarioDualArmor());
            r.RegisterByKey("PLS-79", new CardsASC.PushDownMon("Knock Back"));
            r.RegisterByKey("PLS-80", new CardsPRC1.DynamicPunchMon(20));
            r.RegisterByKey("PLS-81", new ComboMon(new CardsFST2.ConditionBonusMon("Facade", 60, c => c.Pokemon.Has(SpecialCondition.Burned) || c.Pokemon.Has(SpecialCondition.Poisoned)),
                new CardsTWM1.HealSelfMon("Drain Punch", 20)));
            r.RegisterByKey("PLS-82", new ComboMon(new CardsPAR2.FlipConditionMon("Tail Trickery", SpecialCondition.Confused), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("PLS-83", new ComboMon(new Cards30C.PeerAtMon("Scout"), new FlipBonusMon("Fasten Claws", 10)));
            r.RegisterByKey("PLS-84", new ComboMon(new SilentClawMon(), new ParalyzeFlipMon("Fake Out")));
            r.RegisterByKey("PLS-86", new ComboMon(new CardsASC.PushDownMon("Kick Away"), new CardsASC.ToolBonusMon("Reinforced Headbutt", 50)));
            r.RegisterByKey("PLS-87", new ComboMon(new CargoJetMon(), new CardsASC.SelfGuardMon("Steel Wing", 20)));
            r.RegisterByKey("PLS-88", new CardsASC.SelfGuardMon("Rigidify", 20));
            r.RegisterByKey("PLS-89", new CardsWHT.FlipMoreMon("Gear Smash", 2, 20));
            r.RegisterByKey("PLS-90", new KlinklangPlasmaSteel());
            r.RegisterByKey("PLS-91", new CardsPFL.DamagedBonusMon("Hard Crunch", 30));
            r.RegisterByKey("PLS-92", new ComboMon(new SearchAttackMon("Call for Backup", d => d.IsPokemon, 1), new NoRetreatMon("Metal Jail")));
            r.RegisterByKey("PLS-93", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Righteous Edge", false), new CardsCRZ1.PiercingMon("Steel Bullet")));
            r.RegisterByKey("PLS-94", new CardsASC.SelfGuardMon("Guard Press", 20));
            r.RegisterByKey("PLS-95", new CardsTWM1.SelfDiscardMon("Black Ballista", 3));
            r.RegisterByKey("PLS-96", new CardsASR1.PlusMon("White Inferno", 10, c => c.Pokemon.DamageCounters));
            r.RegisterByKey("PLS-97", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("PLS-98", new ClefableMoonGuidance());
            r.RegisterByKey("PLS-99", new CoinsMon("Gatling Peck", 5));
            r.RegisterByKey("PLS-100", new CardsBLW.PetalDanceMon("Raging Pecks"));
            r.RegisterByKey("PLS-101", new SnorlaxBlock());
            r.RegisterByKey("PLS-102", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("PLS-103", new SweetKissMon());
            r.RegisterByKey("PLS-104", new TogekissBrightVeil());
            r.RegisterByKey("PLS-105", new ShoutMon());
            r.RegisterByKey("PLS-106", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("PLS-107", new ComboMon(new CardsFLI3.DestructiveSoundMon(), new CardsBLK.RoundMon()));
            r.RegisterByKey("PLS-109", new CoinsMon("Triple Slap", 3));
            r.RegisterByKey("PLS-110", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("PLS-111", new ComboMon(new DrawAttackMon("Collect", 1), new CoinsMon("Slam", 2)));
            r.RegisterByKey("PLS-112", new ComboMon(new CardsASC.RandomDiscardMon("Fast Swipe"), new FlipBonusMon("Biting Fang", 20)));
            r.RegisterByKey("PLS-113", new CardsMEW1.CounterBonusMon("Second Bite", 10));
            r.RegisterByKey("PLS-114", new ToolBreakerMon());
            r.RegisterByKey("PLS-115", new CardsMEG.FlipUntilTailsBonusMon("Incessant Peck", 20));
            r.RegisterByKey("PLS-116", new BraviaryBigWing());

            r.RegisterByName("Bicycle", new Bicycle());
            r.RegisterByName("Colress", new Colress());
            r.RegisterByName("Colress Machine", new ColressMachine());
            r.RegisterByName("Ether", new Ether());
            r.RegisterByName("Eviolite", new Eviolite());
            r.RegisterByName("Hypnotoxic Laser", new HypnotoxicLaser());
            r.RegisterByName("Plasma Frigate", new PlasmaFrigate());
            r.RegisterByName("Team Plasma Grunt", new TeamPlasmaGrunt());
            r.RegisterByName("Virbank City Gym", new VirbankCityGym());
            r.RegisterByName("Dowsing Machine", new DowsingMachine());
            r.RegisterByName("Victory Piece", new VictoryPiece());
        }

        private static int ToolsInPlay(EffectContext ctx)
        {
            return ctx.MyPokemon().Sum(m => m.Tools.Count) + ctx.OpponentPokemon().Sum(m => m.Tools.Count);
        }

        /// <summary>Bright Veil is on: the Active Togekiss with an unblocked Bright Veil (for Escape Rope: it stops only that player's switch, 2013-02-07).</summary>
        internal static bool BrightVeilOn(GameEngine engine, int player)
        {
            var active = engine.State.Players[player].Active;
            return active != null && active.Def.Abilities.Any(a => a.Name == "Bright Veil") && !engine.AbilityBlocked(active, "Bright Veil");
        }

        // ================================================================== Psychic

        /// <summary>Pile Up: flip a coin; heads, search your deck for a Pokémon Tool card.</summary>
        private sealed class PileUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pile Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0 && ctx.Flip()) ctx.SearchToHand(d => d.IsTool, 1, "Pile Up: choose a Pokémon Tool card");
                return true;
            }
        }

        /// <summary>Biosmog: 20, the Defending Pokémon is now Poisoned; flip a coin, heads: discard an Energy from it.</summary>
        private sealed class BiosmogMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Biosmog") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Biosmog: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Damakinesis: move 6 damage counters from your Pokémon to the Defending Pokémon (6 if there are 6, from any of them: 2013-02-07).</summary>
        private sealed class DamakinesisMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Damakinesis") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                int total = Math.Min(6, ctx.MyPokemon().Sum(m => m.DamageCounters));
                for (int i = 0; i < total; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending)) break;
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count == 0) break;
                    var from = ctx.ChoosePokemon(hurt, "Damakinesis: move damage counter " + (i + 1) + " of " + total + " from which of your Pokémon?");
                    if (from == null) break;
                    ctx.Engine.MoveDamageCounters(from, defending, 1, atk.Name);
                }
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Spinning Turn: 40, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class SpinningTurnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Turn") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Spinning Turn: switch with which Benched Pokémon?");
                return true;
            }
        }

        /// <summary>Premonition: once during your turn, look at the top 2 cards of your deck and put them back in any order.</summary>
        private sealed class LunatonePremonition : Mon
        {
            protected override string Activated { get { return "Premonition"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { CardsOBF2.ReorderOwnTop(ctx, 2, "Premonition"); }
        }

        /// <summary>
        /// Dual Armor: with any Metal Energy attached (a card printed as providing Metal), this Pokémon is both Fighting and Metal.
        /// Hurricane Kick: 30 more for each Prize card the opponent has taken.
        /// </summary>
        private sealed class LucarioDualArmor : Mon
        {
            private static readonly Mon Kick = new CardsASR1.PlusMon("Hurricane Kick", 30, c => c.OpponentPrizesTaken);

            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                var me = src.Pokemon;
                if (me == null || mon != me || types.Contains(EnergyType.Metal)) return;
                // The printed Energy type, so the check can't loop through effects that change types or Energy (Dark Aura).
                if (me.Energy.Any(c => c.Def.EnergyProvides == EnergyType.Metal)) types.Add(EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kick.HandleAttack(ctx, atk); }
        }

        // ================================================================== Darkness

        /// <summary>Silent Claw: the opponent reveals their hand; discard a Supporter there and use its effect (your ruling, v111: any Supporter; its effect only if it can be used).</summary>
        private sealed class SilentClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silent Claw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var supporters = ctx.Opp.Hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                var pick = ctx.ChooseCards(supporters, 1, 1, "Silent Claw: discard which Supporter? (You use its effect.)")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                var script = ctx.Engine.Scripts.For(pick.Def);
                if (!script.CanPlayTrainer(new EffectContext(ctx.Engine, ctx.Player, pick, null)))
                {
                    ctx.Log("Silent Claw: " + pick.Def.Name + "'s effect can't be used right now.");
                    return true;
                }
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + pick.Def.Name + ".");
                script.ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, pick, null));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Cargo Jet: discard a Team Plasma card from your hand; if you do, draw 3 cards.</summary>
        private sealed class CargoJetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cargo Jet") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var plasma = ctx.Me.Hand.Where(c => c.Def.IsTeamPlasma).ToList();
                if (plasma.Count == 0) return true;
                var pick = ctx.ChooseCards(plasma, 1, 1, "Cargo Jet: discard a Team Plasma card")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Plasma Steel: prevent all damage done to your Metal Pokémon by attacks from the opponent's Pokémon-EX. Heavy Bullet: flip for 20 to a Benched Pokémon.</summary>
        private sealed class KlinklangPlasmaSteel : Mon
        {
            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target == null || info.Target.Owner != src.Player) return false;
                var attacker = info.Attacker;
                if (attacker == null || attacker.Owner == src.Player || !attacker.Def.IsPokemonEX) return false;
                return src.Engine.IsType(info.Target, EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heavy Bullet") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Flip()) ctx.DamageOneBenched(20);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Moon Guidance: once during your turn, flip a coin; heads, search your deck for a card that evolves from 1 of your Pokémon
        /// and evolve it. Your ruling (v111): no timing limits (first turn, played or evolved this turn); "can't evolve" effects still apply.
        /// </summary>
        private sealed class ClefableMoonGuidance : Mon
        {
            private static List<PokemonInPlay> Evolvable(EffectContext ctx, CardDef d)
            {
                return ctx.MyPokemon(m => !CardsEVS1.EvolveBlocked(ctx, m) && CardsEVS1.EvolvesOnto(ctx, m, d));
            }

            protected override string Activated { get { return "Moon Guidance"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                var found = ctx.SearchMyDeck(d => Evolvable(ctx, d).Count > 0, 1, "Moon Guidance: choose a card that evolves from 1 of your Pokémon", false);
                if (found.Count > 0)
                {
                    var card = found[0];
                    var targets = Evolvable(ctx, card.Def);
                    var target = targets.Count == 0 ? null : ctx.ChoosePokemon(targets, "Moon Guidance: evolve which Pokémon into " + card.Def.Name + "?");
                    if (target != null) ctx.Engine.EvolvePokemon(target, card);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return false; }
        }

        /// <summary>Block: while this Pokémon is Active, the opponent's Active Pokémon can't retreat. Teampact: 30 per Team Plasma Pokémon you have in play.</summary>
        private sealed class SnorlaxBlock : Mon
        {
            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && mon != null && mon.Owner != src.Player && src.IsActive(mon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Teampact") return false;
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon(m => ctx.Engine.IsTeamPlasma(m)).Count);
                return true;
            }
        }

        /// <summary>Sweet Kiss: the damage, then your opponent draws a card.</summary>
        internal sealed class SweetKissMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Kiss") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.DrawCards(ctx.Opponent, 1);
                return true;
            }
        }

        /// <summary>
        /// Bright Veil: while this Pokémon is Active, Item cards the opponent plays from their hand have no effect on your Pokémon
        /// (Tools already attached keep working, 2014-11-20). Return: draw until you have 6 cards.
        /// </summary>
        private sealed class TogekissBrightVeil : Mon
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && target != null && target.Owner == src.Player && trainer != null && trainer.IsItem;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.DrawUntil(6);
                return true;
            }
        }

        /// <summary>Shout: flip a coin; heads, discard a random card from the opponent's hand.</summary>
        private sealed class ShoutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shout") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count > 0 && ctx.Flip()) CardsASC.DiscardRandomFromOpponentHand(ctx, 1);
                return true;
            }
        }

        /// <summary>Tool Breaker: discard a Pokémon Tool card attached to the Defending Pokémon.</summary>
        private sealed class ToolBreakerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tool Breaker") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Tools.Count == 0 || !ctx.CanAffect(defending)) return true;
                var tool = defending.Tools.Count == 1 ? defending.Tools[0] : ctx.ChooseCards(defending.Tools.ToList(), 1, 1, "Tool Breaker: discard which Pokémon Tool?")[0];
                ctx.Engine.DiscardTool(defending, tool);
                return true;
            }
        }

        /// <summary>Big Wing: once during your turn, if this Pokémon is Active, the opponent switches their Active Pokémon (they choose: 2013-02-07). Wild Edge.</summary>
        private sealed class BraviaryBigWing : Mon
        {
            private static readonly Mon WildEdge = new CardsSUM5.OptionalBoostRecoilMon("Wild Edge", 20, 20);

            protected override string Activated { get { return "Big Wing"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsActive(ctx.Pokemon) && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return WildEdge.HandleAttack(ctx, atk); }
        }

        // ================================================================== Trainers

        /// <summary>Bicycle: draw cards until you have 4 cards in your hand.</summary>
        private sealed class Bicycle : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count(c => c != ctx.Card) < 4; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.DrawUntil(4); }
        }

        /// <summary>Colress: shuffle your hand into your deck, then draw a card for each Benched Pokémon (both players'). Playable with none (2013-03-07).</summary>
        private sealed class Colress : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                int n = ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
                if (n > 0) ctx.Draw(n);
            }
        }

        /// <summary>Colress Machine: search your deck for a Plasma Energy card and attach it to 1 of your Team Plasma Pokémon.</summary>
        private sealed class ColressMachine : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsTeamPlasma(m)); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                var found = targets.Count == 0 ? new List<CardInstance>() : ctx.SearchMyDeck(d => d.Name == "Plasma Energy", 1, "Colress Machine: choose a Plasma Energy card");
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(targets, "Colress Machine: attach Plasma Energy to which Team Plasma Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(found[0], target);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Ether: reveal the top card of your deck; a basic Energy card is attached to 1 of your Pokémon, anything else goes back on top.</summary>
        private sealed class Ether : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0) return;
                var top = ctx.Me.Deck[0];
                ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { top }, "revealed the top card of the deck (Ether)");
                var mine = ctx.MyPokemon();
                if (!top.Def.IsBasicEnergy || mine.Count == 0) return;
                var target = ctx.ChoosePokemon(mine, "Ether: attach " + top.Def.Name + " to which of your Pokémon?");
                if (target == null) return;
                ctx.Me.Deck.RemoveAt(0);
                ctx.Engine.AttachEnergyCard(top, target);
            }
        }

        /// <summary>Eviolite: a Basic Pokémon holder takes 20 less damage from attacks (its own attack's self-damage too: 2011-11-17).</summary>
        private sealed class Eviolite : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !holder.Def.IsBasicPokemon || info.Attacker == null) return damage;
                return Math.Max(0, damage - 20);
            }
        }

        /// <summary>Hypnotoxic Laser: the opponent's Active Pokémon is now Poisoned (a new 1-counter Poison: 2013-02-21); flip, heads: also Asleep.</summary>
        private sealed class HypnotoxicLaser : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
                if (ctx.Flip() && ctx.Opp.Active == target) ctx.ApplyCondition(target, SpecialCondition.Asleep);
            }
        }

        /// <summary>Plasma Frigate: each Pokémon with any Plasma Energy attached (both players') has no Weakness.</summary>
        private sealed class PlasmaFrigate : CardScript
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return mon != null && CardsPLB1.PlasmaEnergy(mon) > 0; }
        }

        /// <summary>Team Plasma Grunt: discard another Team Plasma card from your hand (required), then draw 4 cards.</summary>
        private sealed class TeamPlasmaGrunt : CardScript
        {
            private static List<CardInstance> Cost(EffectContext ctx) { return ctx.Me.Hand.Where(c => c != ctx.Card && c.Def.IsTeamPlasma).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0 && Cost(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cost = Cost(ctx);
                if (cost.Count == 0) return;
                var pick = ctx.ChooseCards(cost, 1, 1, "Team Plasma Grunt: discard a Team Plasma card")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(4);
            }
        }

        /// <summary>Virbank City Gym: 2 more damage counters on Poisoned Pokémon (both players') between turns (on top of any other count: 2013-02-28).</summary>
        private sealed class VirbankCityGym : CardScript
        {
            public override int ModifyPoisonCounters(EffectContext src, PokemonInPlay mon, int counters) { return counters + 2; }
        }

        /// <summary>Dowsing Machine (ACE SPEC): discard 2 cards from your hand, then put a Trainer card from your discard pile into your hand (not one just discarded: 2013-02-07).</summary>
        private sealed class DowsingMachine : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Count(c => c != ctx.Card) >= 2 && ctx.Me.Discard.Any(x => x.Def.IsTrainer) && !ctx.Engine.DiscardToHandBlocked(ctx.Player);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count < 2) return;
                var discarded = ctx.ChooseCards(ctx.Me.Hand.ToList(), 2, 2, "Dowsing Machine: discard 2 cards from your hand");
                foreach (var card in discarded) ctx.Engine.DiscardFromHand(ctx.Player, card);
                var trainers = ctx.Me.Discard.Where(c => c.Def.IsTrainer && !discarded.Contains(c) && c != ctx.Card).ToList();
                if (trainers.Count == 0) return;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Dowsing Machine: put which Trainer card into your hand?")[0];
                ctx.DiscardToHand(new List<CardInstance> { pick });
            }
        }

        /// <summary>Victory Piece (ACE SPEC): a Victini-EX holder can use its attacks whatever Energy is attached.</summary>
        private sealed class VictoryPiece : CardScript
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                var holder = src.Pokemon;
                if (holder != null && mon == holder && holder.Name == "Victini-EX") cost.Clear();
            }
        }
    }
}
