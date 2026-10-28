using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Skyridge Uncommon and Common Pokémon (E3 36-118; v147). Unlimited only. Cleffa (48) and Igglybuff (67) are Baby Pokémon. Buried
    /// Fossil (47) is a Basic Pokémon that evolves into what evolves from Mysterious Fossil (CardScript.EvolvesIntoByRule) and can't be
    /// affected by Special Conditions. Underground Lake's Omanyte and Kabuto are considered Basic Pokémon (PokemonInPlay.IsBasic).
    /// </summary>
    public static class CardsE3B
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E3-36", new ComboMon(new CardsE1.PlasmaMon(), new SelfdestructMon()));
            r.RegisterByKey("E3-37", new DamageCutMon(10, true, new PlainMon()));
            r.RegisterByKey("E3-38", new MachokeE3());
            r.RegisterByKey("E3-39", new ComboMon(new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep), new GiftOfSpiteMon()));
            r.RegisterByKey("E3-40", new NoctowlE3());
            r.RegisterByKey("E3-41", new ComboMon(new WaterOfEvolutionMon(), new DragOffMon(true)));
            r.RegisterByKey("E3-42", new ComboMon(new HandToDeckMon("Surprise Slash", d => d.IsTrainer, "a Trainer card"), new TailsNothingMon("Lunge")));
            r.RegisterByKey("E3-43", new ContinuousChargeMon());
            r.RegisterByKey("E3-44", new ComboMon(new StarmieWaterGunMon(), new CoreBlastMon()));
            r.RegisterByKey("E3-45", new WobbuffetE3());
            r.RegisterByKey("E3-46", new AbraE3());
            r.RegisterByKey("E3-47", new BuriedFossilE3());
            r.RegisterByKey("E3-48", new EnergyRecycleMon());
            r.RegisterByKey("E3-49", new ComboMon(new SearchAttackMon("Limited Delivery", CardsE2T.IsTechnicalMachineOrTool, 1), new OpponentSwitchesMon("Kick Away")));
            r.RegisterByKey("E3-50", new BurrowMon());
            r.RegisterByKey("E3-51", new DittoE3());
            r.RegisterByKey("E3-52", new ComboMon(new BurrowMon(), new CardsE2.AnyOpponentMon("Dig Under", 30)));
            r.RegisterByKey("E3-53", new DunsparceE3());
            r.RegisterByKey("E3-54", new RecoilMon("Take Down", 10));
            r.RegisterByKey("E3-55", new ComboMon(new OpponentSwitchesMon("Whirlwind"), new CardsE2.FlipUntilTailsMon("Leek Rush")));
            r.RegisterByKey("E3-56", new DamageCutMon(10, false, new ComboMon(new ShellRuptureMon(), new CoinsMon("Double Spin", 2))));
            r.RegisterByKey("E3-57", new HauntMon());
            r.RegisterByKey("E3-58", new ComboMon(new CardsE1.EnergyPatchMon(), new SyncroblastMon()));
            r.RegisterByKey("E3-59", new CardsSI.HeadsTailsConditionMon("Sharp Stinger", SpecialCondition.Poisoned, SpecialCondition.Paralyzed));
            r.RegisterByKey("E3-60", new ComboMon(new CardsSI.HeadsTailsConditionMon("Poison Sound Wave", SpecialCondition.Confused, SpecialCondition.Poisoned), new StrengthInNumbersMon()));
            r.RegisterByKey("E3-61", new OpponentSwitchesMon("Fling"));
            r.RegisterByKey("E3-62", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("E3-63", new ComboMon(new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused), new ShadowHandMon()));
            r.RegisterByKey("E3-64", new AngryHornMon());
            r.RegisterByKey("E3-65", new ComboMon(new ConditionAttackMon("Hypnosis", SpecialCondition.Asleep), new CardsPAR2.FlipConditionMon("Eerie Light", SpecialCondition.Confused)));
            r.RegisterByKey("E3-66", new ComboMon(new CardsASR1.TimesMon("Retaliate", c => c.Pokemon.DamageCounters), new CardsE2.FireworksMon()));
            r.RegisterByKey("E3-67", new EnergyHealMon());
            r.RegisterByKey("E3-68", new ComboMon(new HealSelfMon("Nap", 10), new ConditionAttackMon("Lullaby", SpecialCondition.Asleep)));
            r.RegisterByKey("E3-69", new ComboMon(new ConditionAttackMon("Nightmare", SpecialCondition.Asleep), new NoWeaknessResistanceMon("Mind Shock")));
            r.RegisterByKey("E3-70", new DamageCutMon(10, false, new ConditionAttackMon("Poisonpowder", SpecialCondition.Poisoned)));
            r.RegisterByKey("E3-71", new ComboMon(new SearchAttackMon("Assist", d => d.IsSupporter, 1), new CardsPAR2.FlipConditionMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("E3-72", new ComboMon(new CardsE2B.TearyEyesMon(), new CoinsMon("Quick Turn", 2)));
            r.RegisterByKey("E3-73", new CoinsMon("Slam", 2));
            r.RegisterByKey("E3-74", new ComboMon(new TailsNothingMon("Smash Punch"), new OpponentSwitchesMon("Fling")));
            r.RegisterByKey("E3-75", new SignsOfEvolutionMon());
            r.RegisterByKey("E3-76", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new CardsE3.MoveDefendingEnergyMon("Magnetic Lines", true)));
            r.RegisterByKey("E3-77", new ComboMon(new CoinsMon("Fin Smack", 2), new CardsFLF1.NoResistanceMon("Aqua Sonic")));
            r.RegisterByKey("E3-78", new HandToDeckMon("Sudden Swipe", d => d.IsEnergy, "an Energy card"));
            r.RegisterByKey("E3-79", new ComboMon(new EvilEyeMon(), new BlindsideMon()));
            r.RegisterByKey("E3-80", new ComboMon(new ForesightMon(), new RemovalBeamMon()));
            r.RegisterByKey("E3-81", new NidoranCallMon());
            r.RegisterByKey("E3-82", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("E3-83", new CardsPAR2.FlipConditionMon("Poison Claws", SpecialCondition.Poisoned));
            r.RegisterByKey("E3-84", new MaxVoltageMon());
            r.RegisterByKey("E3-85", new TailsNothingMon("Surprise Attack"));
            r.RegisterByKey("E3-86", new RecoilMon("Double-edge", 10));
            r.RegisterByKey("E3-87", new ComboMon(new ConditionAttackMon("Sleep Bubble", SpecialCondition.Asleep), new CoinsMon("Doubleslap", 2)));
            r.RegisterByKey("E3-88", new ComboMon(new CardsSI.HeadsTailsConditionMon("Twiddle", SpecialCondition.Confused, SpecialCondition.Asleep), new CoinsMon("Double Kick", 2)));
            r.RegisterByKey("E3-89", new ContinuousBiteMon());
            r.RegisterByKey("E3-90", new CallForFriendsMon(new ParalyzeFlipMon("Body Slam")));
            r.RegisterByKey("E3-91", new CardsASR1.TimesMon("Retaliate", c => c.Pokemon.DamageCounters));
            r.RegisterByKey("E3-92", new ComboMon(new CoinsMon("Double Scratch", 2), new CardsE2.AnyOpponentMon("Dig Under", 10)));
            r.RegisterByKey("E3-93", new ComboMon(new SandTrapMon(), new PoisonNeedleRushMon()));
            r.RegisterByKey("E3-94", new CardsPAR2.FlipConditionMon("Cold Breath", SpecialCondition.Asleep));
            r.RegisterByKey("E3-95", new ComboMon(new CoinsMon("Double Headbutt", 2), new ParalyzeFlipMon("Ice Beam")));
            r.RegisterByKey("E3-96", new DamageCutMon(20, false, new ConditionAttackMon("Poisonous Saliva", SpecialCondition.Poisoned)));
            r.RegisterByKey("E3-97", new ComboMon(new CoinsMon("Double Peck", 2), new SpinningHeadMon()));
            r.RegisterByKey("E3-98", new CardsBS1.DiscardCostMon("Scorching Lava", EnergyType.Fire, 1, ScorchingLava, null));
            r.RegisterByKey("E3-99", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("E3-100", new SnorlaxE3());
            r.RegisterByKey("E3-101", new ScaryFaceMon());
            r.RegisterByKey("E3-102", new ComboMon(new HandToDeckMon("Threaten", d => d.IsTrainer, "a Trainer card"), new AntlerSwipeMon()));
            r.RegisterByKey("E3-103", new ComboMon(new EnergyHealingMon(), new CoinsMon("Doubleslap", 2)));
            r.RegisterByKey("E3-104", new CardsPAR2.FlipConditionMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("E3-105", new ComboMon(new EnergyPlantMon(), new FlipBonusMon("Trip Over", 30)));
            r.RegisterByKey("E3-106", new CardsE2B.BlotMon());
            r.RegisterByKey("E3-107", new CardsSI.HeadsTailsConditionMon("Freezing Breath", SpecialCondition.Paralyzed, SpecialCondition.Asleep));
            r.RegisterByKey("E3-108", new CardsE1.OverrunMon());
            r.RegisterByKey("E3-109", new ComboMon(new TailsNothingMon("Surprise Attack"), new CardsE2B.RageMon()));
            r.RegisterByKey("E3-110", new ComboMon(new NoRetreatMon("Bear Hug"), new RendMon()));
            r.RegisterByKey("E3-111", new ComboMon(new HealingDustMon(), new CardsE2.MiracleTailMon("Miracle Powder")));
            r.RegisterByKey("E3-112", new ComboMon(new CoinsMon("Double Spin", 2), new CardsPAR2.FlipConditionMon("Supersonic", SpecialCondition.Confused)));
            r.RegisterByKey("E3-113", new ComboMon(new GustAttackMon("Magnetic Pull"), new RecoilMon("Spin Tackle", 10)));
            r.RegisterByKey("E3-114", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("E3-115", new ComboMon(new ParalyzeFlipMon("Entangling Thread"), new CardsPAR2.FlipConditionMon("Poison Horn", SpecialCondition.Poisoned)));
            r.RegisterByKey("E3-116", new ComboMon(new DodgeMon("Agility", false), new TripleSmashMon()));
            r.RegisterByKey("E3-117", new ComboMon(new CardsPAR2.FlipConditionMon("Sound Waves", SpecialCondition.Confused), new CardsE2.AnyOpponentMon("Flitter", 10)));
            r.RegisterByKey("E3-118", new ConditionAttackMon("Poison Spray", SpecialCondition.Poisoned));
        }

        // ================================================================== Shared

        /// <summary>
        /// "Look at your opponent's hand. If they have any [kind] cards there, choose 1 of them. Your opponent shuffles that card into
        /// their deck." Shows the hand; returns true if a card was shuffled in.
        /// </summary>
        internal static bool HandCardToDeck(EffectContext ctx, Func<CardDef, bool> kind, string source, string what)
        {
            var hand = ctx.Opp.Hand.ToList();
            if (hand.Count == 0) return false;
            ctx.Engine.ShowCards(ctx.Player, hand, "Your opponent's hand", source + ": your opponent's hand", ctx.Source);
            var picks = hand.Where(c => kind(c.Def)).ToList();
            if (picks.Count == 0) return false;
            var card = ctx.ChooseCards(picks, 1, 1, source + ": choose " + what + " for your opponent to shuffle into their deck")[0];
            if (!ctx.Opp.Hand.Remove(card)) return false;
            ctx.Opp.Deck.Add(card);
            ctx.Engine.ShuffleDeck(ctx.Opponent);
            ctx.Log(source + ": " + card.Def.Name + " went from " + ctx.Engine.Possessive(ctx.Opponent) + " hand into their deck.");
            return true;
        }

        /// <summary>Scorching Lava: after discarding a Fire Energy card for it, the Defending Pokémon is now Burned.</summary>
        private static void ScorchingLava(EffectContext ctx, AttackDef atk)
        {
            var defending = ctx.Defending;
            if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
            if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
        }

        /// <summary>"Your opponent switches the Defending Pokémon with 1 of their Benched Pokémon, if any" after the damage (Shake, Fling, Kick Away, Whirlwind).</summary>
        internal sealed class OpponentSwitchesMon : Mon
        {
            private readonly string attack;

            public OpponentSwitchesMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && !ctx.Engine.IsKnockedOut(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>"Remove N damage counters from this Pokémon" (Nap, Draining Cut).</summary>
        internal sealed class HealSelfMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HealSelfMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, amount);
                return true;
            }
        }

        /// <summary>"Choose 1 of your opponent's Benched Pokémon and switch it with the Defending Pokémon" with no damage (Magnetic Pull).</summary>
        private sealed class GustAttackMon : Mon
        {
            private readonly string attack;

            public GustAttackMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.GustTargets().Count > 0) ctx.GustOpponent(attack + ": choose 1 of your opponent's Benched Pokémon to switch in");
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Drag Off: before the damage, (you may) switch 1 of the opponent's Benched Pokémon with the Defending Pokémon.</summary>
        private sealed class DragOffMon : Mon
        {
            private readonly bool optional;

            public DragOffMon(bool optional) { this.optional = optional; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drag Off") return false;
                if (ctx.GustTargets().Count > 0 && (!optional || ctx.AskYesNo("Drag Off: switch 1 of your opponent's Benched Pokémon with the Defending Pokémon first?")))
                    ctx.GustOpponent("Drag Off: choose 1 of your opponent's Benched Pokémon to switch in");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Damage from attacks done to this Pokémon is N less after Weakness and Resistance (Exoskeleton, Vase Body; Thick Shell: only from Evolved Pokémon).</summary>
        private sealed class DamageCutMon : Mon
        {
            private readonly int amount;
            private readonly bool evolvedAttackersOnly;
            private readonly Mon attacks;

            public DamageCutMon(int amount, bool evolvedAttackersOnly, Mon attacks)
            {
                this.amount = amount;
                this.evolvedAttackersOnly = evolvedAttackersOnly;
                this.attacks = attacks;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target != src.Pokemon || info.Attacker == null || src.Engine.CurrentSourceKind != EffectSourceKind.Attack) return damage;
                if (evolvedAttackersOnly && !CardsE2.IsEvolved(info.Attacker)) return damage;
                return Math.Max(0, damage - amount);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"Look at your opponent's hand; heads..." attacks: flip; heads, a card of that kind from the opponent's hand goes into their deck.</summary>
        private sealed class HandToDeckMon : Mon
        {
            private readonly string attack;
            private readonly Func<CardDef, bool> kind;
            private readonly string what;

            public HandToDeckMon(string attack, Func<CardDef, bool> kind, string what)
            {
                this.attack = attack;
                this.kind = kind;
                this.what = what;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip()) HandCardToDeck(ctx, kind, attack, what);
                return true;
            }
        }

        /// <summary>"Don't apply Weakness and Resistance" (Mind Shock).</summary>
        private sealed class NoWeaknessResistanceMon : Mon
        {
            private readonly string attack;

            public NoWeaknessResistanceMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, false);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Shell Rupture: 10 damage to each of the opponent's Benched Pokémon.</summary>
        private sealed class ShellRuptureMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shell Rupture") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Strength in Numbers: 10 more for each Zubat, Golbat and Crobat on your Bench.</summary>
        private sealed class StrengthInNumbersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strength in Numbers") return false;
                int n = ctx.Me.Bench.Count(m => m.Name == "Zubat" || m.Name == "Golbat" || m.Name == "Crobat");
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        /// <summary>Angry Horn: base 50 instead of 30 with 4 or more damage counters on this Pokémon.</summary>
        private sealed class AngryHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Angry Horn") return false;
                ctx.DamageDefending(ctx.Pokemon != null && ctx.Pokemon.DamageCounters >= 4 ? 50 : atk.Damage);
                return true;
            }
        }

        /// <summary>Call for Family: put a Nidoran♀ or Nidoran♂ from your deck onto your Bench (not with a full Bench).</summary>
        private sealed class NidoranCallMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Call for Family" || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Family") return false;
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToBench(d => d.Name == "Nidoran♀" || d.Name == "Nidoran♂", 1, "Call for Family: put a Nidoran♀ or Nidoran♂ onto your Bench");
                return true;
            }
        }

        /// <summary>Healing Dust: 2 coins; for each heads, a damage counter off each of your Pokémon.</summary>
        private sealed class HealingDustMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Dust") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return true;
                foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 10 * heads);
                return true;
            }
        }

        /// <summary>Energy Plant: attach a basic Energy card from your deck to 1 of your Pokémon.</summary>
        private sealed class EnergyPlantMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Plant") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Energy Plant: choose a basic Energy card");
                foreach (var card in found)
                {
                    var mons = ctx.MyPokemon();
                    var target = ctx.ChoosePokemon(mons, "Energy Plant: attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target != null) ctx.Engine.AttachEnergyCard(card, target);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Fire

        // ================================================================== Water

        /// <summary>Water Gun: 20 more for each Water Energy not used for its cost (40 more at most).</summary>
        private sealed class StarmieWaterGunMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water Gun") return false;
                var me = ctx.Pokemon;
                int extra = 0;
                if (me != null)
                {
                    int water = ctx.Engine.EnergyCountOfType(me, EnergyType.Water);
                    int total = ctx.EnergyCount(me);
                    int w = atk.Cost.Count(t => t == EnergyType.Water);
                    int c = atk.Cost.Count(t => t == EnergyType.Colorless);
                    int waterForColorless = Math.Max(0, c - (total - water));
                    extra = Math.Max(0, Math.Min(2, water - w - waterForColorless));
                }
                ctx.DamageDefending(atk.Damage + 20 * extra);
                return true;
            }
        }

        /// <summary>Core Blast: 20 more for each Special Energy card attached to the Defending Pokémon.</summary>
        private sealed class CoreBlastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Core Blast") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 20 * (defending == null ? 0 : defending.Energy.Count(e => e.Def.IsSpecialEnergy)));
                return true;
            }
        }

        /// <summary>Water of Evolution: put an Omastar from your hand onto this Omanyte (this counts as evolving it).</summary>
        private sealed class WaterOfEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Water of Evolution") return false;
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                var omastar = ctx.Me.Hand.Where(c => c.Def.Name == "Omastar" && CardsEVS1.EvolvesOnto(ctx, me, c.Def)).ToList();
                if (omastar.Count == 0) return true;
                if (CardsEVS1.EvolveBlocked(ctx, me))
                {
                    ctx.Log(me.Name + " can't evolve right now (Water of Evolution).");
                    return true;
                }
                var card = ctx.ChooseCards(omastar, 1, 1, "Water of Evolution: put which Omastar onto " + me.Name + "?")[0];
                if (ctx.Me.Hand.Remove(card)) ctx.Engine.EvolvePokemon(me, card);
                return true;
            }
        }

        /// <summary>Energy Healing: 2 damage counters off this Pokémon for each Energy attached to it.</summary>
        private sealed class EnergyHealingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Healing") return false;
                var me = ctx.Pokemon;
                int n = ctx.EnergyCount(me);
                if (n > 0 && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, 20 * n);
                return true;
            }
        }

        /// <summary>Signs of Evolution: heads, a card that evolves from Magikarp from your deck into your hand.</summary>
        private sealed class SignsOfEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Signs of Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (me != null && ctx.Me.Deck.Count > 0 && ctx.Flip())
                    ctx.SearchToHand(d => CardsEVS1.EvolvesOnto(ctx, me, d), 1, "Signs of Evolution: choose a card that evolves from " + me.Name);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Selfdestruct: 20 damage to each Benched Pokémon (both sides) and 100 to itself.</summary>
        private sealed class SelfdestructMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Selfdestruct") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 20);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(100);
                return true;
            }
        }

        /// <summary>Max Voltage: discard all Lightning Energy cards attached; 10 more per card.</summary>
        private sealed class MaxVoltageMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Max Voltage") return false;
                var me = ctx.Pokemon;
                int n = CardsBS1.CopyingWithMetronome ? 0 : CardsE2.DiscardCards(ctx, me, CardsE2.EnergyCardsOf(ctx, me, EnergyType.Lightning));
                ctx.DamageDefending(atk.Damage + 10 * n);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Gift of Spite: a damage counter on the Defending Pokémon for each of your Pokémon with damage counters.</summary>
        private sealed class GiftOfSpiteMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gift of Spite") return false;
                var defending = ctx.Defending;
                int n = ctx.MyPokemon(m => m.DamageCounters > 0).Count;
                if (defending != null && n > 0) ctx.PlaceCounters(defending, n);
                return true;
            }
        }

        /// <summary>Mirror Coat (Poké-Body): if the opponent's attack makes Wobbuffet Poisoned or Burned during their turn, their Active Pokémon gets the same. Return Attack: 2 coins; any heads, 10 per damage counter on Wobbuffet.</summary>
        private sealed class WobbuffetE3 : Mon
        {
            public override void OnHolderConditionApplied(EffectContext src, SpecialCondition condition, EffectSourceKind kind, int sourceOwner, PokemonInPlay sourcePokemon)
            {
                var copy = condition & (SpecialCondition.Poisoned | SpecialCondition.Burned);
                if (copy == SpecialCondition.None || kind != EffectSourceKind.Attack || sourceOwner != src.Opponent || src.IsMyTurn) return;
                var defending = src.Opp.Active;
                if (defending == null) return;
                src.Log("Mirror Coat: " + defending.Name + " gets the same Special Condition.");
                if ((copy & SpecialCondition.Poisoned) != 0) src.ApplyCondition(defending, SpecialCondition.Poisoned);
                if ((copy & SpecialCondition.Burned) != 0) src.ApplyCondition(defending, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Return Attack") return false;
                if (ctx.FlipCoins(2) == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * (ctx.Pokemon == null ? 0 : ctx.Pokemon.DamageCounters));
                return true;
            }
        }

        /// <summary>Psychoflow (Poké-Body): with a Psychic Energy card attached, Abra's Retreat Cost is 0.</summary>
        private sealed class AbraE3 : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && CardsE2.EnergyCardsOf(src, mon, EnergyType.Psychic).Count > 0 ? 0 : cost;
            }
        }

        /// <summary>Haunt: a damage counter on the Defending Pokémon.</summary>
        private sealed class HauntMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Haunt") return false;
                if (ctx.Defending != null) ctx.PlaceCounters(ctx.Defending, 1);
                return true;
            }
        }

        /// <summary>Syncroblast: base 10 instead of 40 unless both Active Pokémon have the same number of Energy cards.</summary>
        private sealed class SyncroblastMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Syncroblast") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                bool same = me != null && defending != null && me.Energy.Count == defending.Energy.Count;
                ctx.DamageDefending(same ? atk.Damage : 10);
                return true;
            }
        }

        /// <summary>Shadow Hand: you may discard up to 2 cards from your hand and draw that many.</summary>
        private sealed class ShadowHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shadow Hand") return false;
                ctx.DamageDefending(atk.Damage);
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, Math.Min(2, hand.Count), "Shadow Hand: discard up to 2 cards from your hand to draw that many");
                int n = 0;
                foreach (var card in picks)
                    if (ctx.Me.Hand.Remove(card)) { ctx.Me.Discard.Add(card); n++; }
                if (n > 0) ctx.Draw(n);
                return true;
            }
        }

        /// <summary>Foresight: look at the top 3 cards of either player's deck and put them back in any order.</summary>
        private sealed class ForesightMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Foresight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int pick = ctx.ChooseOption("Foresight: rearrange the top 3 cards of which deck?", "Your deck", "Your opponent's deck");
                CardsE2.ReorderTop(ctx, pick == 0 ? ctx.Player : ctx.Opponent, 3, "Foresight");
                return true;
            }
        }

        /// <summary>Removal Beam: if the Defending Pokémon has Energy cards, flip; heads, discard 1 of them.</summary>
        private sealed class RemovalBeamMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Removal Beam") return false;
                var defending = ctx.Defending;
                bool had = defending != null && defending.Energy.Count > 0;
                ctx.DamageDefending(atk.Damage);
                if (had && ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, 1, "Removal Beam: discard which Energy card from " + defending.Name + "?");
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Focus Energy: during your next turn, Mega Punch's base damage is 80.</summary>
        private sealed class MachokeE3 : Mon
        {
            private const string Marker = "Focus Energy turn";

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Focus Energy")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) me.Markers[Marker] = ctx.Turn + 2;
                    return true;
                }
                if (atk.Name != "Mega Punch") return false;
                int turn;
                bool focused = me != null && me.Markers.TryGetValue(Marker, out turn) && turn == ctx.Turn;
                ctx.DamageDefending(focused ? 80 : atk.Damage);
                return true;
            }
        }

        /// <summary>Continuous Charge: 4 coins, 20 more per heads, and a damage counter on itself per heads.</summary>
        private sealed class ContinuousChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Continuous Charge") return false;
                int heads = ctx.FlipCoins(4);
                ctx.DamageDefending(atk.Damage + 20 * heads);
                if (heads > 0 && ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, heads);
                return true;
            }
        }

        /// <summary>
        /// Buried Fossil: a Basic Pokémon that evolves into anything that evolves from Mysterious Fossil, and can't be affected by Special
        /// Conditions. Reconstruction (Poké-Power): with a basic Energy card in your hand, put an Omanyte or Kabuto from your deck into your
        /// hand, then put a basic Energy card from your hand into your deck and shuffle.
        /// </summary>
        private sealed class BuriedFossilE3 : Mon
        {
            private const string Power = "Reconstruction";

            public override bool EvolvesIntoByRule(CardDef self, CardDef evolution) { return evolution != null && evolution.EvolvesFrom == "Mysterious Fossil"; }

            public override bool PreventsCondition(EffectContext src, PokemonInPlay target, SpecialCondition condition) { return target != null && target == src.Pokemon; }

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Me.Hand.Any(c => c.Def.IsBasicEnergy) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && (d.Name == "Omanyte" || d.Name == "Kabuto"), 1, Power + ": choose an Omanyte or Kabuto");
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0)
                {
                    var card = ctx.ChooseCards(basics, 1, 1, Power + ": put which basic Energy card from your hand into your deck?")[0];
                    if (ctx.Me.Hand.Remove(card)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Burrow: heads, prevent all damage done by attacks to this Pokémon during the opponent's next turn (effects still happen).</summary>
        private sealed class BurrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burrow") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (heads && ctx.InPlay(me)) ctx.AddEffect(new PreventDamageEffect(me.Name + " takes no damage from attacks (Burrow)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Sand Trap: 10 damage to up to 2 of the opponent's Benched Pokémon.</summary>
        private sealed class SandTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sand Trap") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var picks = bench.Count == 1 ? bench : ctx.ChoosePokemonMany(bench, 0, 2, "Sand Trap: choose up to 2 of your opponent's Benched Pokémon");
                foreach (var mon in picks) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>Poison Needle Rush: 3 coins, 20 per heads; any heads, Poisoned.</summary>
        private sealed class PoisonNeedleRushMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Needle Rush") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads > 0 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Evil Eye: a damage counter on 1 of the opponent's Pokémon.</summary>
        private sealed class EvilEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evil Eye") return false;
                var theirs = ctx.OpponentPokemon();
                if (theirs.Count == 0) return true;
                var target = ctx.ChoosePokemon(theirs, "Evil Eye: put a damage counter on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 1);
                return true;
            }
        }

        /// <summary>Blindside: 20 damage to 1 of the opponent's Pokémon with a damage counter (no Weakness or Resistance).</summary>
        private sealed class BlindsideMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blindside") return false;
                var hurt = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                if (hurt.Count == 0) return true;
                var target = ctx.ChoosePokemon(hurt, "Blindside: choose 1 of your opponent's Pokémon with damage counters");
                if (target != null) ctx.DamageWith(target, atk.Damage, false, false, false);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Spinning Head: heads, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class SpinningHeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Head") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads && ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Energy Recycle: shuffle a basic Energy card from your discard pile into your deck.</summary>
        private sealed class EnergyRecycleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Recycle") return false;
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count == 0) return true;
                ctx.DiscardToDeck(pile.Count == 1 ? pile : ctx.ChooseCards(pile, 1, 1, "Energy Recycle: shuffle which basic Energy card into your deck?"));
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Energy Heal: a damage counter off 1 of your Pokémon with an Energy card attached.</summary>
        private sealed class EnergyHealMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Heal") return false;
                var mons = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (mons.Count == 0) return true;
                var target = ctx.ChoosePokemon(mons, "Energy Heal: remove a damage counter from which Pokémon?");
                if (target != null && target.DamageCounters > 0) ctx.Heal(target, 10);
                return true;
            }
        }

        /// <summary>Prismatic Body (Poké-Body): each basic Energy card attached to Ditto provides every type, 1 at a time. Copy.</summary>
        private sealed class DittoE3 : Mon
        {
            private static readonly Mon Copy = new CardsE2.CopyMon();

            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon || energy == null || !energy.Def.IsBasicEnergy) return;
                for (int i = 0; i < units.Count; i++) if (units[i] != 0) units[i] = EnergyMask.AnyType;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Copy.HandleAttack(ctx, atk); }
        }

        /// <summary>Slippery Skin (Poké-Body): while the Defending Pokémon is an Evolved Pokémon, Dunsparce's Retreat Cost is 0. Tail Shake: Asleep.</summary>
        private sealed class DunsparceE3 : Mon
        {
            private static readonly Mon TailShake = new ConditionAttackMon("Tail Shake", SpecialCondition.Asleep);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return mon != null && mon == src.Pokemon && CardsE2.IsEvolved(src.State.Players[1 - mon.Owner].Active) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TailShake.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Investigate (Poké-Power): look at the top 2 cards of either player's deck, or at up to 2 of either player's Prize cards; they
        /// stay where they are. Triple Smash: 3 coins, 10 more per heads.
        /// </summary>
        private sealed class NoctowlE3 : Mon
        {
            private const string Power = "Investigate";
            private static readonly Mon TripleSmash = new TripleSmashMon();

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                int pick = ctx.ChooseOption(Power + ": look at what?", "The top 2 cards of your deck", "The top 2 cards of your opponent's deck", "Up to 2 of your Prize cards", "Up to 2 of your opponent's Prize cards");
                bool mine = pick == 0 || pick == 2;
                var p = mine ? ctx.Me : ctx.Opp;
                var shown = new List<CardInstance>();
                if (pick < 2) shown.AddRange(p.Deck.Take(2));
                else
                {
                    var left = Enumerable.Range(0, p.Prizes.Count).ToList();
                    for (int i = 0; i < 2 && left.Count > 0; i++)
                    {
                        var labels = left.Select(k => "Prize card " + (k + 1)).Concat(new[] { "Done" }).ToArray();
                        int choice = ctx.ChooseOption(Power + ": look at which Prize card?", labels);
                        if (choice < 0 || choice >= left.Count) break;
                        shown.Add(p.Prizes[left[choice]]);
                        left.RemoveAt(choice);
                    }
                }
                if (shown.Count > 0) ctx.Engine.ShowCards(ctx.Player, shown, Power, Power + ": " + (mine ? "your" : "your opponent's") + (pick < 2 ? " top cards" : " Prize cards"), ctx.Source);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " looked at " + shown.Count + " card" + (shown.Count == 1 ? "" : "s") + " (" + Power + ").");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TripleSmash.HandleAttack(ctx, atk); }
        }

        /// <summary>Triple Smash: 3 coins, 10 more per heads.</summary>
        private sealed class TripleSmashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Smash") return false;
                ctx.DamageDefending(atk.Damage + 10 * ctx.FlipCoins(3));
                return true;
            }
        }

        /// <summary>Continuous Bite: 3 coins; 1/2/3 heads add 10/30/60.</summary>
        private sealed class ContinuousBiteMon : Mon
        {
            private static readonly int[] Bonus = { 0, 10, 30, 60 };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Continuous Bite") return false;
                ctx.DamageDefending(atk.Damage + Bonus[ctx.FlipCoins(3)]);
                return true;
            }
        }

        /// <summary>Call for Friends: put a Baby or Basic Pokémon from your deck onto your Bench (not with a full Bench).</summary>
        private sealed class CallForFriendsMon : Mon
        {
            private readonly Mon other;

            public CallForFriendsMon(Mon other) { this.other = other; }

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Call for Friends" || ctx.BenchSpace > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call for Friends") return other != null && other.HandleAttack(ctx, atk);
                if (ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0) ctx.SearchToBench(d => true, 1, "Call for Friends: put a Baby Pokémon or Basic Pokémon onto your Bench");
                return true;
            }
        }

        /// <summary>Lolling About (Poké-Power): while Snorlax is Active, remove a damage counter from it; Snorlax is now Asleep. Continuous Tumble: flip until tails.</summary>
        private sealed class SnorlaxE3 : Mon
        {
            private static readonly Mon Tumble = new CardsE2.FlipUntilTailsMon("Continuous Tumble");

            protected override string Activated { get { return "Lolling About"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me.DamageCounters > 0) ctx.Heal(me, 10);
                ctx.ApplyCondition(me, SpecialCondition.Asleep);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tumble.HandleAttack(ctx, atk); }
        }

        /// <summary>Scary Face: heads, the Defending Pokémon can't attack or retreat until the end of the opponent's next turn.</summary>
        private sealed class ScaryFaceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scary Face") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CantAttackEffect(defending.Name + " can't attack (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                ctx.AddEffect(new CantRetreatEffect(defending.Name + " can't retreat (Scary Face)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Antler Swipe: heads, 20 to the Defending Pokémon (no Weakness or Resistance); tails, 20 to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class AntlerSwipeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Antler Swipe") return false;
                if (ctx.Flip()) { if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, 20, false, false, false); }
                else if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(20);
                return true;
            }
        }

        /// <summary>Rend: 20 more if the Defending Pokémon already has damage counters.</summary>
        private sealed class RendMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rend") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.DamageCounters > 0 ? 20 : 0));
                return true;
            }
        }
    }
}
