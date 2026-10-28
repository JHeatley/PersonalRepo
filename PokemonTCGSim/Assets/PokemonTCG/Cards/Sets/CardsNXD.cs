using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Next Destinies (NXD 1-103, 2012; Expanded). Cards reprinted in Legendary Treasures (Shaymin-EX, Moltres, Reshiram-EX, Articuno,
    /// Kyurem-EX, Pikachu, Zapdos, Zekrom-EX, Riolu, Lucario, Minccino, Cinccino, Emboar), Celebrations (Reshiram, Zekrom,
    /// Mewtwo-EX), Dark Explorers (Gardevoir), Scarlet &amp; Violet (Exp. Share), BREAKthrough (Heavy Ball), Battle Styles (Level
    /// Ball), Shining Legends (Double Colorless Energy) and Ascended Heroes (Prism Energy) are prints of those; 94-99 are the full
    /// arts. Your ruling (v129): Chandelure's Flame Burst also does its printed 30 to the Defending Pokémon.
    /// Pokémon whose attacks only do their printed damage need no script: Pansage, Growlithe (11), Pansear, Darumaka, Litwick,
    /// Panpour, Vanillite, Ralts, Munna and Pidove's Gust.
    /// </summary>
    public static class CardsNXD
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("NXD-1", new ComboMon(new PowerPinchMon(), new NoRetreatMon("Grip and Squeeze")));
            r.RegisterByKey("NXD-2", new FlipBonusMon("Trip Over", 10));
            r.RegisterByKey("NXD-3", new CardsWHT.FlipMoreMon("Double Headbutt", 2, 10));
            r.RegisterByKey("NXD-4", new ComboMon(new ConditionAttackMon("White Noise", SpecialCondition.Asleep), new DrainingCutMon()));
            r.RegisterByKey("NXD-7", new ComboMon(new DrawAttackMon("Collect", 3), new StadiumBonusMon("Stadium Drain", StadiumBonusMon.Extra.HealSelf)));
            r.RegisterByKey("NXD-8", new ComboMon(new FindAFriendMon(), new FlipBonusMon("Rising Lunge", 20)));
            r.RegisterByKey("NXD-9", new AmoongussSporprise());
            r.RegisterByKey("NXD-10", new ComboMon(new StokeMon(), new FlipBonusMon("Firebreathing", 20)));
            r.RegisterByKey("NXD-12", new ArcanineBlazingMane());
            r.RegisterByKey("NXD-13", new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"));
            r.RegisterByKey("NXD-16", new ComboMon(new DrawAttackMon("Collect", 3), new StadiumBonusMon("Stadium Burn", StadiumBonusMon.Extra.Burn)));
            r.RegisterByKey("NXD-19", new CardsFLF1.FlipTailsSelfDiscardMon("Ember"));
            r.RegisterByKey("NXD-20", new ComboMon(new CardsFST1.TwoBenchedMon("Flame Burst", 30), new InfernoMon()));
            r.RegisterByKey("NXD-23", new CardsLOR1.RecoverMon());
            r.RegisterByKey("NXD-24", new ComboMon(new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused), new CardsCRZ1.PiercingMon("Swift")));
            r.RegisterByKey("NXD-25", new ComboMon(new CallForFamilyMon(), new RecoilMon("Reckless Charge", 20)));
            r.RegisterByKey("NXD-26", new CardsMEG.SnipeMon("Water Arrow", 20));
            r.RegisterByKey("NXD-29", new ComboMon(new DrawAttackMon("Collect", 3), new StadiumBonusMon("Stadium Wave", StadiumBonusMon.Extra.Sleep)));
            r.RegisterByKey("NXD-30", new BaredFangsMon("Bared Fangs"));
            r.RegisterByKey("NXD-32", new CardsPAR2.FlipConditionMon("Icy Wind", SpecialCondition.Asleep));
            r.RegisterByKey("NXD-33", new VanilluxeSlipperySoles());
            r.RegisterByKey("NXD-34", new ComboMon(new ParalyzeFlipMon("Bubble"), new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned)));
            r.RegisterByKey("NXD-35", new ComboMon(new CardsEVO3.MirrorMoveMon("Vengeful Wish"), new CardsTWM1.HealSelfMon("Absorb Life", 30)));
            r.RegisterByKey("NXD-36", new CardsRCL1.SetupBoostMon("Sniffle", "Belt", 20));
            r.RegisterByKey("NXD-37", new ComboMon(new CardsMEG.WeakenDefendingMon("Daunt", 20), new FlipBonusMon("Ambush", 30)));
            r.RegisterByKey("NXD-40", new ComboMon(new ParalyzeFlipMon("Thundershock"), new CoinsMon("Slam", 2)));
            r.RegisterByKey("NXD-42", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("NXD-43", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("NXD-44", new ComboMon(new FlipBonusMon("Jump On", 30), new RecoilMon("Wild Charge", 10)));
            r.RegisterByKey("NXD-45", new CoinsMon("Quick Turn", 2));
            r.RegisterByKey("NXD-46", new ComboMon(new CardsDRX.FlashImpactMon(20), new CardsASC.FlipDiscardDefendingEnergyMon("Crunch")));
            r.RegisterByKey("NXD-47", new CardsFST1.FlipRecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("NXD-48", new ComboMon(new CardsUPR2.DisconnectMon(), new CardsDEX.VoltBoltMon("Lightning Crash", 80)));
            r.RegisterByKey("NXD-49", new SwitchSelfMon("Bounce", false));
            r.RegisterByKey("NXD-52", new ParalyzeFlipMon("Nasty Goo"));
            r.RegisterByKey("NXD-53", new ComboMon(new NoRetreatMon("Gentle Wrap"), new CardsCEC1.ToxicCECMon("Toxic Secretion")));
            r.RegisterByKey("NXD-56", new CardsMEG.DefendingEnergyBonusMon("Psychic", 10));
            r.RegisterByKey("NXD-59", new MusharnaForewarn());
            r.RegisterByKey("NXD-60", new ComboMon(new CardsEVS1.SynchrodrawMon("Synchrodraw"), new CardsASC.FlipPerEnergyMon("DarMAXitan")));
            r.RegisterByKey("NXD-61", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("NXD-62", new ComboMon(new BrainControlMon(), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("NXD-65", new Cards30CC.BenchHitMon("Sand Jet", 20));
            r.RegisterByKey("NXD-66", new ComboMon(new SandBazookaMon(), new CardsFLF1.NoResistanceMon("Rock Tumble")));
            r.RegisterByKey("NXD-67", new CoinsMon("Triple Smash", 3));
            r.RegisterByKey("NXD-68", new ComboMon(new SearchAttackMon("Haul In", d => d.IsTool, 2), new CardsASR1.PlusMon("Meditate", 10, c => c.Defending == null ? 0 : c.Defending.DamageCounters)));
            r.RegisterByKey("NXD-69", new NoRetreatMon("Corner"));
            r.RegisterByKey("NXD-70", new ComboMon(new DarkPenaltyMon(), new CoinsMon("Fury Swipes", 3)));
            r.RegisterByKey("NXD-71", new SurprisePunchMon());
            r.RegisterByKey("NXD-72", new ShiftryGiantFan());
            r.RegisterByKey("NXD-73", new ComboMon(new CardsTWM1.HealSelfMon("Shed Skin", 40), new TailsNothingMon("Lunge")));
            r.RegisterByKey("NXD-74", new ComboMon(new CardsASC.SelfGuardMon("Rock Head", 20), new CardsFST2.ConditionBonusMon("Hammer Kick", 30, c => c.Defending != null && c.RemainingHp(c.Pokemon) < c.RemainingHp(c.Defending))));
            r.RegisterByKey("NXD-75", new FlipBonusMon("Knock Away", 10));
            r.RegisterByKey("NXD-76", new BronzongHealBlock());
            r.RegisterByKey("NXD-77", new RecoilMon("Self Destruct", 60));
            r.RegisterByKey("NXD-78", new ComboMon(new ConditionAttackMon("Sing", SpecialCondition.Asleep), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("NXD-79", new ComboMon(new CardsBLK.RoundMon(), new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("NXD-80", new CoinsMon("Double Scratch", 2));
            r.RegisterByKey("NXD-81", new ComboMon(new SearchAttackMon("Nasty Plot", d => true, 1), new FlipRandomDiscardMon("Shadow Claw")));
            r.RegisterByKey("NXD-82", new ComboMon(new CardsSUM5.OptionalBoostRecoilMon("Giga Power", 20, 20), new CardsASR1.PlusMon("Raging Hammer", 10, c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("NXD-83", new Cards30C.PeerAtMon("Scout"));
            r.RegisterByKey("NXD-86", new Cilan());
            r.RegisterByKey("NXD-90", new PokemonCenterStadium());
            r.RegisterByKey("NXD-91", new SkyarrowBridge());
            r.RegisterByKey("NXD-101", new ChandelureCursedShadow());
            r.RegisterByKey("NXD-102", new ZoroarkFoulPlay());
            r.RegisterByKey("NXD-103", new HydreigonDarkAura());
        }

        // ================================================================== Abilities

        /// <summary>Sporprise: when you evolve into this Pokémon from your hand, you may make the opponent's Active Pokémon Confused and Poisoned. Rising Lunge: heads, +30.</summary>
        private sealed class AmoongussSporprise : Mon
        {
            private static readonly Mon Lunge = new FlipBonusMon("Rising Lunge", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Sporprise", target != null, "Sporprise: make your opponent's Active Pokémon Confused and Poisoned?")) return;
                if (target == null || !ctx.InPlay(target)) return;
                ctx.ApplyCondition(target, SpecialCondition.Confused);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lunge.HandleAttack(ctx, atk); }
        }

        /// <summary>Blazing Mane: damaged by the opponent's attack while Active (even if Knocked Out), the Attacking Pokémon is now Burned. Fire Spin: tails, discard 2 Energy.</summary>
        private sealed class ArcanineBlazingMane : Mon
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || !ctx.InPlay(attacker)) return;
                if (ctx.ApplyCondition(attacker, SpecialCondition.Burned)) Announce(ctx, "Blazing Mane");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Spin") return false;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() && ctx.InPlay(ctx.Pokemon)) CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, 2, atk.Name);
                return true;
            }
        }

        /// <summary>
        /// Slippery Soles: once during your turn, you may switch your Active Pokémon with 1 of your Benched Pokémon; if you do, the
        /// opponent switches their Active Pokémon with 1 of their Benched Pokémon. Crushing Ice: 10 more for each Colorless in the
        /// Defending Pokémon's Retreat Cost.
        /// </summary>
        private sealed class VanilluxeSlipperySoles : Mon
        {
            private const string Soles = "Slippery Soles";
            private static readonly Mon Ice = new CardsASR1.PlusMon("Crushing Ice", 10, c => c.Defending == null ? 0 : c.Engine.RetreatCostOf(c.Defending));

            protected override string Activated { get { return Soles; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.SwitchMyActive(Soles + ": switch your Active Pokémon with which Benched Pokémon?")) return;
                if (ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Ice.HandleAttack(ctx, atk); }
        }

        /// <summary>Forewarn: once during your turn, look at the top 2 cards of your deck; put 1 into your hand and the other back on top. Fluffy Dream: this Pokémon is now Asleep.</summary>
        private sealed class MusharnaForewarn : Mon
        {
            private const string Forewarn = "Forewarn";
            private static readonly Mon Dream = new CardsRCL2.SelfConditionMon("Fluffy Dream", SpecialCondition.Asleep);

            protected override string Activated { get { return Forewarn; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.LookAtTop(2);
                if (top.Count == 0) return;
                var pick = ctx.ChooseCards(top, 1, 1, Forewarn + ": put which card into your hand? (the other goes back on top of your deck)")[0];
                if (ctx.Me.Deck.Remove(pick)) ctx.Me.Hand.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put 1 of the top 2 cards of their deck into their hand (Forewarn).");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Dream.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Giant Fan: when you evolve into this Pokémon from your hand, you may flip a coin; heads, choose 1 of the opponent's
        /// Pokémon, which the opponent shuffles with its cards into their deck. Whirlwind: the opponent switches their Active Pokémon.
        /// </summary>
        private sealed class ShiftryGiantFan : Mon
        {
            private static readonly Mon Wind = new CardsDRI1.WhirlwindMon("Whirlwind");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Giant Fan", ctx.Opp.InPlay().Any(), "Giant Fan: flip a coin to shuffle 1 of your opponent's Pokémon into their deck?")) return;
                if (!ctx.Flip()) return;
                var targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Giant Fan: choose 1 of your opponent's Pokémon to shuffle into their deck");
                if (target == null || !ctx.InPlay(target)) return;
                if (!ctx.CanAffect(target))
                {
                    ctx.Log("The effect of Giant Fan on " + target.Name + " was prevented.");
                    return;
                }
                ctx.Engine.ShufflePokemonIntoDeck(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wind.HandleAttack(ctx, atk); }
        }

        /// <summary>Heal Block: no Pokémon (yours or the opponent's) can be healed. Oracle Inflict: 10 more for each card in the opponent's hand.</summary>
        private sealed class BronzongHealBlock : Mon
        {
            private static readonly Mon Oracle = new CardsASR1.PlusMon("Oracle Inflict", 10, c => c.Opp.Hand.Count);

            public override bool BlocksHealing(EffectContext src, PokemonInPlay mon) { return src.Pokemon != null; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Oracle.HandleAttack(ctx, atk); }
        }

        /// <summary>Cursed Shadow: once during your turn, if this Pokémon is your Active Pokémon, put 3 damage counters on the opponent's Pokémon in any way you like. Eerie Glow: Burned and Confused.</summary>
        private sealed class ChandelureCursedShadow : Mon
        {
            private const string Shadow = "Cursed Shadow";
            private static readonly Mon Glow = new Cards30C.BurnAndConfuseMon("Eerie Glow");

            protected override string Activated { get { return Shadow; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && ctx.Opp.InPlay().Any(); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.PlaceCountersAnyWay(3, () => ctx.OpponentPokemon());
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Glow.HandleAttack(ctx, atk); }
        }

        /// <summary>Dark Aura: every Energy attached to this Pokémon provides Darkness instead of its usual type (a Double Colorless Energy gives 2). Berserker Blade: 40 to 2 Benched.</summary>
        private sealed class HydreigonDarkAura : Mon
        {
            private static readonly Mon Blade = new CardsFST1.TwoBenchedMon("Berserker Blade", 40);

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon) return;
                for (int i = 0; i < units.Count; i++) units[i] = EnergyMask.Of(EnergyType.Darkness);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blade.HandleAttack(ctx, atk); }
        }

        /// <summary>Nasty Plot: search your deck for a card and put it into your hand. Foul Play: use 1 of the Defending Pokémon's attacks as this attack.</summary>
        private sealed class ZoroarkFoulPlay : Mon
        {
            private static readonly Mon Plot = new SearchAttackMon("Nasty Plot", d => true, 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foul Play") return Plot.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                CardDef copied;
                int index;
                if (ChooseCopiedAttack(ctx, defending, "Foul Play: choose 1 of " + (defending == null ? "" : defending.Name) + "'s attacks to use", out copied, out index))
                    UseCopiedAttack(ctx, copied, index);
                return true;
            }
        }

        // ================================================================== Attacks

        /// <summary>Power Pinch: flip 2 coins; for each heads, discard an Energy attached to the Defending Pokémon.</summary>
        private sealed class PowerPinchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Pinch") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads == 0 || !TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Power Pinch on " + defending.Name + " was prevented.");
                    return true;
                }
                ctx.DiscardEnergyFrom(defending, heads, "Power Pinch: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Draining Cut: flip 2 coins, 40 for each heads; heal from this Pokémon the damage done.</summary>
        private sealed class DrainingCutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Draining Cut") return false;
                int heads = ctx.FlipCoins(2);
                int dealt = heads > 0 ? ctx.DamageDefending(atk.Damage * heads) : 0;
                var me = ctx.Pokemon;
                if (dealt > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, dealt);
                return true;
            }
        }

        /// <summary>"If there is any Stadium card in play, this attack does 30 more damage, and [heal 30 / Burned / Asleep]" (Stadium Drain, Burn, Wave).</summary>
        private sealed class StadiumBonusMon : Mon
        {
            internal enum Extra { HealSelf, Burn, Sleep }

            private readonly string attack;
            private readonly Extra extra;

            public StadiumBonusMon(string attack, Extra extra)
            {
                this.attack = attack;
                this.extra = extra;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                bool stadium = ctx.State.Stadium != null;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (stadium ? 30 : 0));
                if (!stadium) return true;
                if (extra == Extra.HealSelf)
                {
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 30);
                }
                else if (TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, extra == Extra.Burn ? SpecialCondition.Burned : SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Find a Friend: flip a coin; if heads, search your deck for a Pokémon and put it into your hand.</summary>
        private sealed class FindAFriendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Find a Friend") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0 || !ctx.Flip()) return true;
                ctx.SearchToHand(d => d.IsPokemon, 1, "Find a Friend: choose a Pokémon");
                return true;
            }
        }

        /// <summary>Stoke: flip a coin; if heads, search your deck for a Fire Energy card and attach it to this Pokémon.</summary>
        private sealed class StokeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stoke") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0 || !ctx.InPlay(me) || !ctx.Flip()) return true;
                var found = ctx.SearchMyDeck(d => CardsSSP1.IsBasicOf(d, EnergyType.Fire), 1, "Stoke: choose a Fire Energy card to attach to " + me.Name);
                foreach (var en in found) ctx.Engine.AttachEnergyCard(en, me);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Inferno: discard all Energy attached to this Pokémon; the Defending Pokémon is now Burned.</summary>
        private sealed class InfernoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Inferno") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.DiscardAllEnergy(ctx.Pokemon);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Bared Fangs: if the Defending Pokémon has no damage counters on it before the damage, this attack does nothing. Used for Bared Fangs, Cauterize, Finishing Stinger, Discerning Spear (the attack name is passed in).</summary>
        internal sealed class BaredFangsMon : Mon
        {
            private readonly string attack;

            public BaredFangsMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending == null || ctx.Defending.DamageCounters == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Brain Control: the opponent reveals their hand; choose a card from it and put it on the bottom of their deck.</summary>
        private sealed class BrainControlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Brain Control") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var pick = ctx.ChooseCards(ctx.Opp.Hand.ToList(), 1, 1, "Brain Control: put which card on the bottom of your opponent's deck?")[0];
                if (!ctx.Opp.Hand.Remove(pick)) return true;
                ctx.Opp.Deck.Add(pick);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Def.Name + " on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck (Brain Control).");
                return true;
            }
        }

        /// <summary>Sand Bazooka: you may move an Energy from this Pokémon to 1 of your Benched Pokémon.</summary>
        private sealed class SandBazookaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Bazooka") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                var picks = ctx.ChooseCards(me.Energy.ToList(), 0, 1, "Sand Bazooka: you may move an Energy from " + me.Name + " to 1 of your Benched Pokémon");
                if (picks.Count == 0) return true;
                var to = ctx.ChoosePokemon(ctx.MyBench(), "Sand Bazooka: move " + picks[0].Def.Name + " to which Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(me, to, picks[0]);
                return true;
            }
        }

        /// <summary>Dark Penalty: if the Defending Pokémon has no Pokémon Tool attached, this attack does nothing.</summary>
        private sealed class DarkPenaltyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dark Penalty") return false;
                if (ctx.Defending == null || ctx.Defending.Tools.Count == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Surprise Punch: move an Energy attached to the Defending Pokémon to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SurprisePunchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise Punch") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0) return true;
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Surprise Punch on " + defending.Name + " was prevented.");
                    return true;
                }
                var energy = defending.Energy.ToList();
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Surprise Punch: move which Energy from " + defending.Name + "?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Surprise Punch: move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, card);
                return true;
            }
        }

        /// <summary>"Flip a coin. If heads, discard a random card from your opponent's hand" (Shadow Claw).</summary>
        private sealed class FlipRandomDiscardMon : Mon
        {
            private readonly string attack;

            public FlipRandomDiscardMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Opp.Hand;
                if (hand.Count == 0 || !ctx.Flip() || hand.Count == 0) return true;
                var card = hand[ctx.State.Rng.Next(hand.Count)];
                ctx.Engine.DiscardFromHand(ctx.Opponent, card);
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Cilan: search your deck for up to 3 basic Energy cards, reveal them, and put them into your hand.</summary>
        private sealed class Cilan : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsBasicEnergy, 3, "Cilan: choose up to 3 basic Energy cards");
            }
        }

        /// <summary>Pokémon Center: once during each player's turn, that player may heal 20 damage from 1 of their Benched Pokémon.</summary>
        private sealed class PokemonCenterStadium : StadiumAction
        {
            private static List<PokemonInPlay> Damaged(EffectContext ctx) { return ctx.MyBench().Where(m => m.DamageCounters > 0).ToList(); }

            protected override bool Ready(EffectContext ctx) { return Damaged(ctx).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var targets = Damaged(ctx);
                if (targets.Count == 0) return false;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, "Pokémon Center: heal 20 damage from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, 20);
                return false;
            }
        }

        /// <summary>Skyarrow Bridge: the Retreat Cost of each Basic Pokémon in play (both players') is 1 Colorless less.</summary>
        private sealed class SkyarrowBridge : CardScript
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon.IsBasic ? Math.Max(0, cost - 1) : cost;
            }
        }
    }
}
