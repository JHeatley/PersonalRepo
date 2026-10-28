using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// XY (XY, v105), part 2: the Fighting, Darkness, Metal, Fairy and Colorless Pokémon (58-114). Diglett, Dugtrio and Tauros are
    /// Generations' cards, Yveltal and Xerneas Steam Siege's and Xerneas-EX the Celebrations Classic Collection one.
    /// </summary>
    public static class CardsXY2
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("XY-60", new CardsAOR2.DigOutMon());
            r.RegisterByKey("XY-61", new MadMountainMon());
            r.RegisterByKey("XY-62", new ComboMon(new RockBlastMon(), new RockWreckerMon()));
            r.RegisterByKey("XY-63", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsMEG.WeakenDefendingMon("Moonblast", 20)));
            r.RegisterByKey("XY-64", new CardsFST2.ConditionBonusMon("Cosmic Spin", 30, c => c.Me.Bench.Any(m => m.Name == "Lunatone")));
            r.RegisterByKey("XY-65", new FlipBonusMon("Pummel", 20));
            r.RegisterByKey("XY-66", new ComboMon(new FlipBonusMon("Pummel", 20), new CardsSSP2.StompOffMon("Hammer Arm", 1)));
            r.RegisterByKey("XY-67", new ComboMon(new CardsEVS1.WakeUpSlapMon(60), new CardsPRC1.DynamicPunchMon(40)));
            r.RegisterByKey("XY-68", new ComboMon(new DrawAttackMon("Filch", 1), new CardsASC.FlipDiscardDefendingEnergyMon("Rip Claw")));
            r.RegisterByKey("XY-70", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("XY-71", new ComboMon(new BotherMon(), new CardsASC.PushDownMon("Knock Back")));
            r.RegisterByKey("XY-72", new SearchAttackMon("Nasty Plot", d => true, 1));
            r.RegisterByKey("XY-73", new ComboMon(new NoRetreatMon("Corner"), new NightClawMon()));
            r.RegisterByKey("XY-74", new InkayUpsideDown());
            r.RegisterByKey("XY-75", new CardsFLF1.NoResistanceMon("Puncture"));
            r.RegisterByKey("XY-76", new ComboMon(new MentalTrashMon(), new CardsLTR3.BrilliantVoiceMon("Distortion Beam")));
            r.RegisterByKey("XY-77", new ComboMon(new CardsMEG.SandAttackMon("Mental Panic"), new CardsFLF1.NoResistanceMon("Puncture")));
            r.RegisterByKey("XY-79", new ComboMon(new EvilBallMon(), new CardsFST1.MoveEnergyToBenchMon("Y Cyclone")));
            r.RegisterByKey("XY-80", new ComboMon(new CardsBLK.PeckOffMon("Joust"), new CardsPFL.DamagedBonusMon("Tailspin Piledriver", 40)));
            r.RegisterByKey("XY-81", new CardsASC.FlipDiscardDefendingEnergyMon("Cut Down"));
            r.RegisterByKey("XY-82", new ComboMon(new ConditionAttackMon("Metal Sound", SpecialCondition.Confused), new CardsASC.NextTurnBoostMon("Metal Wallop", 40)));
            r.RegisterByKey("XY-84", new CoinsMon("Dual Blades", 2));
            r.RegisterByKey("XY-85", new CardsBST2.AegislashBST(new CardsFLF1.NoResistanceMon("Buster Swing")));
            r.RegisterByKey("XY-86", new CardsBST2.AegislashBST(new KingsShieldMon()));
            r.RegisterByKey("XY-87", new HeartfeltSongMon());
            r.RegisterByKey("XY-88", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("XY-89", new ComboMon(new GatherEnergyMon(), new CardsBLK.NoAttackNextTurnMon("Hocus Pinkus")));
            r.RegisterByKey("XY-90", new ComboMon(new CardsASR1.TimesMon("Balloon Barrage", c => c.EnergyCount(c.Pokemon)), new RecoilMon("Double-Edge", 10)));
            r.RegisterByKey("XY-91", new ComboMon(new MassageMon(), new CardsWHT.FlipMoreMon("Slap Down", 2, 20)));
            r.RegisterByKey("XY-92", new CardsPOR.SweetScentMon("Sweet Scent", 20));
            r.RegisterByKey("XY-93", new AromatisseTransfer());
            r.RegisterByKey("XY-95", new SlurpuffVeil());
            r.RegisterByKey("XY-98", new CoinsMon("Double Hit", 2));
            r.RegisterByKey("XY-99", new ComboMon(new Cards30C.SelfCounterBonusMon("Rage", 10), new CardsWHT.FlipMoreMon("Endeavor", 2, 20)));
            r.RegisterByKey("XY-101", new ComboMon(new ParalyzeFlipMon("Glare"), new CardsMEW1.CounterBonusMon("Second Bite", 10)));
            r.RegisterByKey("XY-102", new FlipBonusMon("Aerial Ace", 30));
            r.RegisterByKey("XY-103", new SwellowDriveOff());
            r.RegisterByKey("XY-104", new CardsSHF1.HealEachMon("Heal Bell", 10, false));
            r.RegisterByKey("XY-105", new ComboMon(new EnergySalonMon(), new ParalyzeFlipMon("Fake Out")));
            r.RegisterByKey("XY-106", new TailsNothingMon("Hyper Fang"));
            r.RegisterByKey("XY-107", new ComboMon(new CoinsMon("Double Headbutt", 2), new HypnoHeadbuttMon()));
            r.RegisterByKey("XY-109", new FlipBonusMon("Jump On", 20));
            r.RegisterByKey("XY-110", new ComboMon(new CardsFST2.ConditionBonusMon("Bite Off", 60, c => c.Defending != null && c.Defending.Def.IsPokemonEX),
                new Cards30CC.BenchHitMon("Wild Barking", 20)));
            r.RegisterByKey("XY-111", new DodgeMon("Dig", false));
            r.RegisterByKey("XY-112", new ComboMon(new PickupMon(), new DodgeMon("Dig", false)));
            r.RegisterByKey("XY-113", new DrawAttackMon("Me First", 1));
            r.RegisterByKey("XY-114", new FurfrouCoat());
        }

        // ================================================================== Fighting

        /// <summary>Mad Mountain: flip 2 coins; both heads, discard the top card of the opponent's deck for each damage counter on this Pokémon.</summary>
        private sealed class MadMountainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mad Mountain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.FlipCoins(2) < 2) return true;
                int n = ctx.Pokemon.DamageCounters;
                if (n > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }

        /// <summary>Rock Blast: flip a coin for each Fighting Energy attached to this Pokémon; 50 for each heads.</summary>
        private sealed class RockBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Blast") return false;
                int coins = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fighting);
                int heads = coins > 0 ? ctx.FlipCoins(coins) : 0;
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Rock Wrecker: not affected by Weakness or Resistance; this Pokémon can't attack during your next turn.</summary>
        private sealed class RockWreckerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rock Wrecker") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SelfCantAttackNextTurn();
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Bother: flip a coin; heads, the opponent can't play Supporter cards from their hand during their next turn.</summary>
        private sealed class BotherMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bother") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip()) return true;
                var lockFx = new CardsTWM2.SupporterLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Supporter cards during their next turn (Bother)", ctx.Player, ctx.Opponent, ctx.Turn + 1);
                lockFx.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(lockFx);
                return true;
            }
        }

        /// <summary>Night Claw: flip a coin; tails, discard 2 Energy attached to this Pokémon.</summary>
        private sealed class NightClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Night Claw") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Upside-Down Evolution: once during your turn, if this Pokémon is Confused, search your deck for a card that evolves from it and
        /// evolve it (on any turn, the first one and the one it came into play included: your ruling). Confusion Wave: both Active
        /// Pokémon are Confused.
        /// </summary>
        private sealed class InkayUpsideDown : Mon
        {
            private static readonly Mon Wave = new CardsCEC1.ConfusionWaveMon("Confusion Wave");

            protected override string Activated { get { return "Upside-Down Evolution"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return me != null && me.Has(SpecialCondition.Confused) && ctx.Me.Deck.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                CardsGEN2.EvolveOneFromDeck(ctx, new List<PokemonInPlay> { ctx.Pokemon }, null, "Upside-Down Evolution");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wave.HandleAttack(ctx, atk); }
        }

        /// <summary>Mental Trash: the opponent flips 4 coins and discards a card from their hand for each tails.</summary>
        private sealed class MentalTrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mental Trash") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.Engine.FlipCoins(ctx.Opponent, 4, "Mental Trash");
                int n = Math.Min(4 - heads, ctx.Opp.Hand.Count);
                if (n <= 0) return true;
                var hand = ctx.Opp.Hand.ToList();
                var picks = n == hand.Count ? hand : ctx.Engine.ChooseCards(ctx.Opponent, hand, n, n, "Mental Trash: discard " + n + (n == 1 ? " card" : " cards") + " from your hand", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        /// <summary>Evil Ball: 20 more for each Energy attached to both Active Pokémon.</summary>
        private sealed class EvilBallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evil Ball") return false;
                int n = ctx.EnergyCount(ctx.Pokemon) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                ctx.DamageDefending(atk.Damage + 20 * n);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>King's Shield: prevent all damage done to this Pokémon by attacks next turn; it can't use King's Shield during your next turn.</summary>
        private sealed class KingsShieldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "King's Shield") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.InPlay(ctx.Pokemon)) return true;
                CardsSP1.FlipNoDamageMon.ProtectFromDamage(ctx, "King's Shield");
                ctx.SelfCantUseAttackNextTurn("King's Shield");
                return true;
            }
        }

        // ================================================================== Fairy

        /// <summary>Heartfelt Song: discard a Darkness Energy attached to the opponent's Active Pokémon.</summary>
        private sealed class HeartfeltSongMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Heartfelt Song") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var defending = ctx.Defending;
                if (defending == null || !ctx.CanAffect(defending)) return true;
                if (defending.Energy.Any(en => CardsDRI1.ProvidesType(ctx, defending, en, EnergyType.Darkness)))
                    ctx.DiscardEnergyFrom(defending, 1, "Heartfelt Song: discard a Darkness Energy from " + defending.Name, en => CardsDRI1.ProvidesType(ctx, defending, en, EnergyType.Darkness));
                return true;
            }
        }

        /// <summary>Gather Energy: search your deck for a basic Energy card and attach it to 1 of your Pokémon.</summary>
        internal sealed class GatherEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Energy") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Gather Energy: choose a basic Energy card");
                foreach (var en in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Gather Energy: attach " + en.Def.Name + " to which Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(en, target);
                    else ctx.Me.Deck.Add(en);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Massage: heal 60 damage from 1 of your Benched Pokémon.</summary>
        private sealed class MassageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Massage") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Massage: heal 60 damage from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, 60);
                return true;
            }
        }

        /// <summary>Fairy Transfer: as often as you like during your turn, move a Fairy Energy from 1 of your Pokémon to another. Fairy Wind.</summary>
        private sealed class AromatisseTransfer : Mon
        {
            private const string Transfer = "Fairy Transfer";

            private static List<PokemonInPlay> Holders(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.Energy.Any(e => CardsDRI1.ProvidesType(ctx, m, e, EnergyType.Fairy)));
            }

            private static bool CanMove(EffectContext ctx) { return ctx.MyPokemon().Count > 1 && Holders(ctx).Count > 0; }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                var printed = ctx.Card != null ? ctx.Card.Def : ctx.Pokemon.Def;
                return abilityIndex >= 0 && abilityIndex < printed.Abilities.Count && printed.Abilities[abilityIndex].Name == Transfer && ctx.IsMyTurn && CanMove(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanMove(ctx), offerDone => MoveOne(ctx, offerDone));
            }

            private static bool MoveOne(EffectContext ctx, bool offerDone)
            {
                var giver = RepeatableAbility.ChoosePokemonOrDone(ctx, Holders(ctx), Transfer + ": move a Fairy Energy from which Pokémon?" + (offerDone ? " Press Done when finished." : ""), offerDone);
                if (giver == null) return false;
                var fairy = giver.Energy.Where(e => CardsDRI1.ProvidesType(ctx, giver, e, EnergyType.Fairy)).ToList();
                if (fairy.Count == 0) return false;
                var card = fairy.Select(e => e.Def.Name).Distinct().Count() == 1 ? fairy[0] : ctx.ChooseCards(fairy, 1, 1, Transfer + ": move which Energy?")[0];
                var to = RepeatableAbility.ChoosePokemonOrDone(ctx, ctx.MyPokemon(m => m != giver), Transfer + ": move it from " + giver.Name + " to which Pokémon?", offerDone);
                if (to == null) return false;
                return ctx.Engine.MoveEnergy(giver, to, card);
            }
        }

        /// <summary>Sweet Veil: your Pokémon with any Fairy Energy attached can't be affected by Special Conditions (and lose any they have). Draining Kiss.</summary>
        private sealed class SlurpuffVeil : Mon
        {
            private static readonly Mon Kiss = new CardsTWM1.HealSelfMon("Draining Kiss", 30);

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player
                    && target.Energy.Any(en => CardsDRI1.ProvidesType(src, target, en, EnergyType.Fairy));
            }

            public override bool CuresConditionsItPrevents { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Kiss.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Drive Off: once during your turn, if this Pokémon is Active, the opponent switches their Active Pokémon with a Benched one. Wing Attack.</summary>
        private sealed class SwellowDriveOff : Mon
        {
            protected override string Activated { get { return "Drive Off"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.IsActive(ctx.Pokemon) && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
        }

        /// <summary>Energy Salon: search your deck for up to 3 basic Energy cards of different types and put them into your hand.</summary>
        private sealed class EnergySalonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Salon") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var types = new HashSet<EnergyType>();
                for (int i = 0; i < 3; i++)
                {
                    var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && !types.Contains(d.EnergyProvides), 1,
                        "Energy Salon: choose a basic Energy card of a different type (" + (i + 1) + " of up to 3)");
                    if (found.Count == 0) break;
                    types.Add(found[0].Def.EnergyProvides);
                    ctx.Me.Hand.AddRange(found);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Hypno Headbutt: you may do 30 more damage; if you do, this Pokémon is now Asleep.</summary>
        private sealed class HypnoHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hypno Headbutt") return false;
                bool boost = ctx.AskYesNo("Hypno Headbutt: do 30 more damage (this Pokémon is then Asleep)?");
                ctx.DamageDefending(atk.Damage + (boost ? 30 : 0));
                if (boost && ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Pickup: put 2 Item cards from your discard pile into your hand.</summary>
        private sealed class PickupMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pickup") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var items = ctx.Me.Discard.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                int n = Math.Min(2, items.Count);
                ctx.DiscardToHand(n == items.Count ? items : ctx.ChooseCards(items, n, n, "Pickup: put 2 Item cards from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Fur Coat: 20 less damage from attacks (after Weakness and Resistance). Energy Cutoff.</summary>
        private sealed class FurfrouCoat : Mon
        {
            private static readonly Mon Cutoff = new CardsDRI2.CrunchMon("Energy Cutoff");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.Attacker != null ? damage - 20 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Cutoff.HandleAttack(ctx, atk); }
        }
    }
}
