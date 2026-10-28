using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Furious Fists (FFI, v103), part 2: the Fighting, Darkness, Fairy, Dragon and Colorless Pokémon (44-87). Machop, Machoke,
    /// Machamp, Hitmonlee, Hitmonchan and the two Clefairy line cards are Generations' cards. Tyrunt is a Restored Pokémon.
    /// </summary>
    public static class CardsFFI2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FFI-49", new ComboMon(new DrawAttackMon("Quick Draw", 1), new CardsPRC2.SwitchAfterMon("Helicoptero")));
            r.RegisterByKey("FFI-50", new ComboMon(new CardsDAA1.DrainMon("Giga Drain"), new CardsFLF1.NoResistanceMon("Sky Uppercut")));
            r.RegisterByKey("FFI-52", new HariyamaFat());
            r.RegisterByKey("FFI-53", new CardsSSP2.StompOffMon("Mountain Munch", 1));
            r.RegisterByKey("FFI-54", new ComboMon(new CardsFLF1.NoResistanceMon("Missile Jab"), new CorkscrewSmashMon()));
            r.RegisterByKey("FFI-55", new CardsDRI2.CrunchMon("Rising Fist"));
            r.RegisterByKey("FFI-56", new TailsNothingMon("Lunge"));
            r.RegisterByKey("FFI-57", new CardsPAF1.ReturnSelfMon("Aero Turn"));
            r.RegisterByKey("FFI-58", new CardsSIT1.ShoutOfPowerMon());
            r.RegisterByKey("FFI-59", new BadInfluenceEvolutionMon());
            r.RegisterByKey("FFI-60", new CoinsMon("Comet Punch", 4));
            r.RegisterByKey("FFI-61", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("FFI-62", new ComboMon(new CardsFST2.ConditionBonusMon("Chew Up", 90, c => c.Defending != null && c.Defending.Energy.Any(en => en.Def.IsSpecialEnergy)),
                new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("FFI-63", new HawluchaSpirit());
            r.RegisterByKey("FFI-64", new CardsASC.CounterattackMon("Counterattack", 2, new CardsROS2.AnyStadiumBonusMon("Moonsault Stomp", 40)));
            r.RegisterByKey("FFI-65", new ComboMon(new CardsBLK.VenoshockMon(40), new ConditionAttackMon("Venomous Fang", SpecialCondition.Poisoned)));
            r.RegisterByKey("FFI-67", new MachineGunHeadbuttMon("Machine Gun Headbutt"));
            r.RegisterByKey("FFI-68", new ComboMon(new ClobberMon(), new CardsSSP2.StompOffMon("Hammer Arm", 1)));
            r.RegisterByKey("FFI-70", new CardsMEG.WeakenDefendingMon("Moonblast", 10));
            r.RegisterByKey("FFI-72", new ComboMon(new CurlyRibbonMon(), new CardsASC.NextTurnBoostMon("Echoed Voice", 50)));
            r.RegisterByKey("FFI-73", new KlefkiKey());
            r.RegisterByKey("FFI-74", new DragoniteEX());
            r.RegisterByKey("FFI-75", new SearchAttackMon("Charge Energy", d => d.IsBasicEnergy, 2));
            r.RegisterByKey("FFI-76", new ComboMon(new RainbowShowerMon(), new SandSweepMon("Sand Sweep", 30)));
            r.RegisterByKey("FFI-77", new NoivernEcho());
            r.RegisterByKey("FFI-78", new TailsNothingMon("Lunge"));
            r.RegisterByKey("FFI-79", new ComboMon(new CardsASC.RandomDiscardMon("Knock Off"), new CardsMEG.FlipUntilTailsBonusMon("Lickichop", 30)));
            r.RegisterByKey("FFI-80", new EeveeEvolution());
            r.RegisterByKey("FFI-81", new CardsDEX.BigYawnMon());
            r.RegisterByKey("FFI-82", new CardsFST1.FlipRecoilMon("Reckless Charge", 20));
            r.RegisterByKey("FFI-83", new ComboMon(new CardsTEU1.AmnesiaMon("Amnesia"), new CardsTWM1.SelfDiscardMon("Knuckle Sandwich", 1)));
            r.RegisterByKey("FFI-84", new SafetyCheckMon());
            r.RegisterByKey("FFI-85", new ComboMon(new HeldItemInspectionMon(), new ThoroughCrunchMon()));
            r.RegisterByKey("FFI-86", new ComboMon(new CardsASC.PushDownMon("Push Down"),
                new CardsFST2.ConditionBonusMon("Twister Throw", 60, c => c.Me.Hand.Count == c.Opp.Hand.Count)));
        }

        // ================================================================== Fighting

        /// <summary>Thick Fat: 30 less damage from the opponent's Fire and Water Pokémon's attacks (after Weakness and Resistance).</summary>
        private sealed class HariyamaFat : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || info.Target != me || attacker == null || attacker.Owner == me.Owner) return damage;
                bool hot = src.Engine.IsType(attacker, EnergyType.Fire) || src.Engine.IsType(attacker, EnergyType.Water);
                return hot ? damage - 30 : damage;
            }
        }

        /// <summary>Corkscrew Smash: you may draw cards until you have 6 cards in your hand.</summary>
        private sealed class CorkscrewSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Corkscrew Smash") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count < 6 && ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Corkscrew Smash: draw cards until you have 6 cards in your hand?")) ctx.DrawUntil(6);
                return true;
            }
        }

        /// <summary>Bad-Influence Evolution: with a Darkness Pokémon on your Bench, evolve this Pokémon with a card from your deck.</summary>
        private sealed class BadInfluenceEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bad-Influence Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || ctx.Me.Deck.Count == 0 || !ctx.Me.Bench.Any(m => ctx.Engine.IsType(m, EnergyType.Darkness))) return true;
                CardsSSP1.EvolveFromDeck(ctx, me, "Bad-Influence Evolution: choose a card that evolves from " + me.Name);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Shining Spirit: this Pokémon's attack damage isn't affected by Weakness or Resistance. Flying Press: nothing unless the opponent's Active is a Pokémon-EX.</summary>
        private sealed class HawluchaSpirit : Mon
        {
            public override bool OwnAttacksIgnoreWeaknessAndResistance(EffectContext src, DamageInfo info) { return src.Pokemon != null && info.Attacker == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flying Press") return false;
                if (ctx.Defending == null || !ctx.Defending.Def.IsPokemonEX) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Machine Gun Headbutt: flip 3 coins, 50 damage for each heads; this Pokémon is now Confused. Used for Machine Gun Headbutt, Petal Dance (the attack name is passed in).</summary>
        internal sealed class MachineGunHeadbuttMon : Mon
        {
            private readonly string attack;

            public MachineGunHeadbuttMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Clobber: you may discard an Item card from your hand for 40 more damage.</summary>
        private sealed class ClobberMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Clobber") return false;
                var items = ctx.Me.Hand.Where(c => c.Def.IsItem).ToList();
                bool boost = false;
                if (items.Count > 0)
                {
                    var picks = ctx.ChooseCards(items, 0, 1, "Clobber: you may discard an Item card from your hand for 40 more damage");
                    if (picks.Count > 0)
                    {
                        ctx.Engine.DiscardFromHand(ctx.Player, picks[0]);
                        boost = true;
                    }
                }
                ctx.DamageDefending(atk.Damage + (boost ? 40 : 0));
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Curly Ribbon: move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class CurlyRibbonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Curly Ribbon") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Curly Ribbon: move which Energy from " + defending.Name + "?")[0];
                var bench = ctx.Opp.Bench.ToList();
                var target = ctx.ChoosePokemon(bench, "Curly Ribbon: move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(defending, target, card);
                return true;
            }
        }

        /// <summary>Secret Key: each of your Fairy Pokémon's Resistance is -40. Fairy Lock: the Defending Pokémon can't retreat.</summary>
        private sealed class KlefkiKey : Mon
        {
            private static readonly Mon Lock = new NoRetreatMon("Fairy Lock");

            public override int ModifyResistanceAmount(EffectContext src, PokemonInPlay mon, int amount)
            {
                return mon != null && mon.Owner == src.Player && src.Engine.IsType(mon, EnergyType.Fairy) ? 40 : amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lock.HandleAttack(ctx, atk); }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Bust In: benched from the hand, you may move any number of basic Energy from your other Pokémon to it; if you do, switch it
        /// with your Active Pokémon. Jet Sonic: you may discard an Energy from it for 40 more damage.
        /// </summary>
        private sealed class DragoniteEX : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                bool ready = ctx.MyPokemon().Any(m => m != me && m.Energy.Any(en => en.Def.IsBasicEnergy));
                if (!TriggerOnce(ctx, "Bust In", ready, "Bust In: move basic Energy from your other Pokémon to Dragonite-EX and switch it into the Active Spot?")) return;
                int moved = 0;
                for (int guard = 0; guard < 60; guard++)
                {
                    var givers = ctx.MyPokemon().Where(m => m != me && m.Energy.Any(en => en.Def.IsBasicEnergy)).ToList();
                    if (givers.Count == 0) break;
                    var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, givers, "Bust In: move a basic Energy from which Pokémon? Press Done when finished.", moved > 0);
                    if (giver == null) break;
                    var basics = giver.Energy.Where(en => en.Def.IsBasicEnergy).ToList();
                    var card = ctx.ChooseCards(basics, 1, 1, "Bust In: move which Energy from " + giver.Name + "?")[0];
                    if (ctx.Engine.MoveEnergy(giver, me, card)) moved++;
                    if (!RepeatableAbility.ForPerson(ctx)) break;
                }
                if (moved > 0 && ctx.Me.Bench.Contains(me)) ctx.Engine.SwitchActive(ctx.Player, me);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Jet Sonic") return false;
                var me = ctx.Pokemon;
                bool boost = me.Energy.Count > 0 && ctx.AskYesNo("Jet Sonic: discard an Energy from " + me.Name + " for 40 more damage?");
                if (boost) boost = ctx.DiscardEnergyFrom(me, 1, "Jet Sonic: discard an Energy from " + me.Name) > 0;
                ctx.DamageDefending(atk.Damage + (boost ? 40 : 0));
                return true;
            }
        }

        /// <summary>Rainbow Shower: attach as many basic Energy cards from your hand to your Pokémon as you like, in any way.</summary>
        private sealed class RainbowShowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rainbow Shower") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0) ctx.AttachCardsAnyWay(basics, basics.Count, (card, mon) => true, "Rainbow Shower");
                return true;
            }
        }

        /// <summary>Sand Sweep: heal 30 damage from each of your Pokémon that has any Energy attached. Used for Sand Sweep, Energy Bloom (the attack name and the numbers are passed in).</summary>
        internal sealed class SandSweepMon : Mon
        {
            private readonly string attack;
            private readonly int heal;

            public SandSweepMon(string attack, int heal)
            {
                this.attack = attack;
                this.heal = heal;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => m.Energy.Count > 0 && m.DamageCounters > 0)) ctx.Heal(mon, heal);
                return true;
            }
        }

        /// <summary>Echolocation: whenever attack damage would be done to this Pokémon, flip a coin; heads, prevent it. Boomburst: 30 to each.</summary>
        private sealed class NoivernEcho : Mon
        {
            private static readonly Mon Boom = new CardsCEC3.SpreadAmountMon("Boomburst", 30);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || damage <= 0) return false;
                Announce(src, "Echolocation");
                return src.Engine.FlipCoin(src.Player, "Echolocation");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Boom.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Energy Evolution: when you attach a basic Energy card from your hand to it during your turn, you may search your deck for a
        /// card of that Energy's type that evolves from it and evolve it.
        /// </summary>
        private sealed class EeveeEvolution : Mon
        {
            public override bool EvolvesOnOwnerAttachFromHand { get { return true; } }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var mon = src.Pokemon;
                if (mon == null || target != mon || energy == null || !energy.Def.IsBasicEnergy || !src.IsMyTurn || !src.InPlay(mon)) return;
                var type = energy.Def.EnergyProvides;
                if (src.Me.Deck.Count == 0 || CardsEVS1.EvolveBlocked(src, mon)) return;
                if (!src.AskYesNo("Energy Evolution: search your deck for a " + type + " card that evolves from " + mon.Name + " and evolve it?")) return;
                Announce(src, "Energy Evolution");
                var found = src.SearchMyDeck(d => d.HasType(type) && CardsEVS1.EvolvesOnto(src, mon, d), 1, "Energy Evolution: choose a " + type + " card that evolves from " + mon.Name, false);
                if (found.Count > 0) src.Engine.EvolvePokemon(mon, found[0]);
                src.ShuffleMyDeck();
            }
        }

        /// <summary>Safety Check: look at 1 of your face-down Prize cards.</summary>
        private sealed class SafetyCheckMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Safety Check") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var p = ctx.Me;
                var faceDown = p.Prizes.Where(c => !p.FaceUpPrizeIds.Contains(c.Id)).ToList();
                if (faceDown.Count == 0) return true;
                var labels = Enumerable.Range(1, faceDown.Count).Select(i => "Prize card " + i).ToArray();
                int pick = faceDown.Count == 1 ? 0 : ctx.ChooseOption("Safety Check: look at which of your face-down Prize cards?", labels);
                if (pick < 0 || pick >= faceDown.Count) return true;
                ctx.ChooseCards(new List<CardInstance> { faceDown[pick] }, 0, 1, "Safety Check: this is the Prize card you looked at");
                return true;
            }
        }

        /// <summary>Held-Item Inspection: the opponent reveals their hand; choose an Item card there and they shuffle it into their deck.</summary>
        private sealed class HeldItemInspectionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Held-Item Inspection") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                CardsTWM1.RevealOpponentHand(ctx);
                var items = ctx.Opp.Hand.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var pick = ctx.ChooseCards(items, 1, 1, "Held-Item Inspection: your opponent shuffles which Item card into their deck?")[0];
                if (ctx.Opp.Hand.Remove(pick)) ctx.Opp.Deck.Add(pick);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Thorough Crunch: flip 2 coins; discard an Energy from the opponent's Active Pokémon for each heads.</summary>
        private sealed class ThoroughCrunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thorough Crunch") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage);
                for (int i = 0; i < heads; i++)
                {
                    if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) break;
                    ctx.DiscardEnergyFrom(defending, 1, "Thorough Crunch: discard an Energy from " + defending.Name);
                }
                return true;
            }
        }
    }
}
