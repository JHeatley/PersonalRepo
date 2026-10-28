using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Fusion Strike (FST, 2021), Pokémon 131-223 (Fighting, Darkness, Metal, Dragon, Colorless), the full-art-only Pokémon
    /// (Celebi V 245, Sandaconda V 252, Espeon VMAX 270), the Trainers and Fusion Strike Energy. Graveler, Baltoy, Latias,
    /// Snorlax, Dunsparce, Greedent V, Hoopa V, Adventurer's Discovery, Cook, Elesa's Sparkle and Judge came into the pool
    /// earlier. The Pokémon Tools are Tools, not Items.
    /// Pokémon whose attacks only do their printed damage need no script (Mankey, Geodude, Lucario V, Drilbur, Stufful,
    /// Clobbopus, Grapploct, Falinks 155, Makuhita, Tyranitar V, Galarian Linoone, Carvanha, Sharpedo, Croagunk, Darkrai,
    /// Zorua, Yveltal, Goomy, Lopunny, Skitty's evolution line aside).
    /// </summary>
    public static class CardsFST2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("FST-131", new ComboMon(new DigItUpMon(), new CardsFST1.LetsAllRolloutMon()));
            r.RegisterByKey("FST-132", new CardsDEX.DigUppercutMon());
            r.RegisterByKey("FST-134", new CardsCRZ1.VBonusMon("Gut Punch", 60));
            r.RegisterByKey("FST-137", new GolemFST());
            r.RegisterByKey("FST-138", new CardsASC.SelfGuardMon("Guard Press", 30));
            r.RegisterByKey("FST-139", new ComboMon(new CardsASR1.TimesMon("Powerful Rage", OwnDamageCounters), new CardsBUS1.OwnBenchQuakeMon("Earthquake", 30)));
            r.RegisterByKey("FST-140", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("FST-141", new ComboMon(new CardsDRI2.CrunchMon("Cut Down"), new ConditionAttackMon("Venomous Hit", SpecialCondition.Poisoned)));
            r.RegisterByKey("FST-143", new HariyamaFST());
            r.RegisterByKey("FST-145", new ComboMon(new RapidSpinMon("Rapid Spin"), new AncientImprintMon()));
            r.RegisterByKey("FST-148", new ComboMon(new SwitchSelfMon("Strafe", true), new EarthenBoomMon()));
            r.RegisterByKey("FST-149", new CardsMEG.ChargeSelfMon("Raised Bad", EnergyType.Darkness, 2));
            r.RegisterByKey("FST-151", new ConditionAttackMon("Split Spiral Punch", SpecialCondition.Confused));
            r.RegisterByKey("FST-154", new ConditionBonusMon("Cliff Edge Formation", 160, OpponentHasOnePrize));
            // Darkness
            r.RegisterByKey("FST-156", new ComboMon(new ConditionAttackMon("Dark Slumber", SpecialCondition.Asleep), new SelfCountersMon("Pain Explosion", 3)));
            r.RegisterByKey("FST-157", new ComboMon(new CardsASR1.TimesMon("Fear and Panic", OpponentVAndGx), new LockAfterAttackMon("G-Max Swallow Up", false)));
            r.RegisterByKey("FST-159", new ParalyzeFlipMon("Lick"));
            r.RegisterByKey("FST-161", new ComboMon(new CardsPOR.MemoryLockMon("Silence"), new CardsPFL.DamagedBonusMon("Merciless Strike", 90)));
            r.RegisterByKey("FST-164", new CardsWHT.DragOffMon("Drag Off", 30));
            r.RegisterByKey("FST-166", new CardsDAA2.TriplePoisonMon("Severe Poison", 4));
            r.RegisterByKey("FST-168", new ComboMon(new BenchSearchAttackMon("Call for Family", 1), new ConditionAttackMon("Super Poison Breath", SpecialCondition.Poisoned)));
            r.RegisterByKey("FST-169", new ConditionAttackMon("Poison Gas", SpecialCondition.Poisoned));
            r.RegisterByKey("FST-171", new CoinsMon("Double Claw", 2));
            r.RegisterByKey("FST-172", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("FST-173", new BoneBlockMon());
            r.RegisterByKey("FST-174", new ComboMon(new CardsSSP2.StompOffMon("Knocking Hammer", 1), new CardsASC.RandomDiscardMon("Shakedown")));
            r.RegisterByKey("FST-176", new FlipBonusMon("Play Rough", 30));
            r.RegisterByKey("FST-177", new CardsASC.FlipDiscardDefendingEnergyMon("Crushing Blow"));
            r.RegisterByKey("FST-178", new ConditionBonusMon("Rear Attack", 140, SmallBench));
            r.RegisterByKey("FST-179", new CardsASR1.TimesMon("Explosive Discontent", OwnDamageCounters));
            // Metal
            r.RegisterByKey("FST-180", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("FST-181", new CallToMusterMon());
            r.RegisterByKey("FST-182", new CardsASC.SelfGuardMon("Steel Wing", 30));
            r.RegisterByKey("FST-183", new NoRetreatMon("Rock Tomb"));
            r.RegisterByKey("FST-184", new AdversityJawsMon());
            r.RegisterByKey("FST-185", new GenesectV());
            r.RegisterByKey("FST-186", new DrawAttackMon("Unlock", 2));
            r.RegisterByKey("FST-187", new CardsFST1.LetsAllRolloutMon());
            r.RegisterByKey("FST-188", new CardsTWM1.HealSelfMon("Iron Intake", 30));
            r.RegisterByKey("FST-189", new ComboMon(new IngotSwingMon(), new CardsTWM1.SelfDiscardMon("Blasting Hammer", 1)));
            r.RegisterByKey("FST-190", new ComboMon(new CardsASC.SelfGuardMon("Steel Wing", 30), new CardsFST1.MoveEnergyToBenchMon("Power Cyclone")));
            r.RegisterByKey("FST-191", new RecoilMon("High Horsepower", 20));
            r.RegisterByKey("FST-192", new RecoilMon("High Horsepower", 30));
            // Dragon
            r.RegisterByKey("FST-194", new LatiosFST());
            r.RegisterByKey("FST-196", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("FST-197", new GoodraFST());
            r.RegisterByKey("FST-198", new ShellTrapMon());
            // Colorless
            r.RegisterByKey("FST-199", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("FST-200", new DrawAttackMon("Pay Day", 1));
            r.RegisterByKey("FST-201", new ComboMon(new CardsFST1.NextTurnBonusMon("No Reprieve", 80), new RecoilMon("Rampage Drill", 30)));
            r.RegisterByKey("FST-202", new CardsTWM1.HealSelfMon("Drain Slap", 30));
            r.RegisterByKey("FST-203", new BlisseyFST());
            r.RegisterByKey("FST-204", new FlipBonusMon("Coordinated One-Two Punch", 100));
            r.RegisterByKey("FST-205", new CardsPFL.FlipUntilTailsTimesMon("Continuous Steps"));
            r.RegisterByKey("FST-208", new CardsASR1.TimesMon("Wild Dive", DefendingEnergy));
            r.RegisterByKey("FST-209", new SketchingTrashMon());
            r.RegisterByKey("FST-210", new TailsNothingMon("Whimsy Tackle"));
            r.RegisterByKey("FST-211", new ComboMon(new WillfulBusybodyMon(), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("FST-212", new CoinsMon("Double Kick", 2));
            r.RegisterByKey("FST-214", new ComboMon(new SearchAttackMon("Find a Friend", IsPokemonCard, 1), new RecoilMon("Take Down", 10)));
            r.RegisterByKey("FST-215", new RecoilMon("Take Down", 30));
            r.RegisterByKey("FST-216", new CardsASR1.DiscardDefendingSpecialMon("Flying Stomp", false));
            r.RegisterByKey("FST-218", new ComboMon(new TurnAProfitMon(), new DrawAttackMon("Max Gimme Gimme", 3)));
            r.RegisterByKey("FST-219", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("FST-220", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("FST-221", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Derail", false), new CardsFST1.LetsAllRolloutMon()));
            r.RegisterByKey("FST-222", new FlipBonusMon("Knock Away", 40));
            r.RegisterByKey("FST-223", new SwitchSelfMon("Bounce", true));
            // Full-art-only Pokémon
            r.RegisterByKey("FST-245", new ComboMon(new LeafletDanceMon(), new SwitchSelfMon("Slash Back", false)));
            r.RegisterByKey("FST-252", new CardsFST1.TakeLessMon(30, null));
            r.RegisterByKey("FST-270", new EspeonVmax());

            // Trainers
            r.RegisterByName("Battle VIP Pass", new BattleVipPass());
            r.RegisterByName("Bug Catcher", new BugCatcher());
            r.RegisterByName("Chili & Cilan & Cress", new ChiliCilanCress());
            r.RegisterByName("Cram-o-matic", new CramOMatic());
            r.RegisterByName("Cross Switcher", new CrossSwitcher());
            r.RegisterByName("Crossceiver", new Crossceiver());
            r.RegisterByName("Dancer", new Dancer());
            r.RegisterByName("Farewell Bell", new FarewellBell());
            r.RegisterByName("Power Tablet", new PowerTablet());
            r.RegisterByName("Quick Ball", new QuickBall());
            r.RegisterByName("Schoolboy", new PrizeDrawSupporter(1, 3, 5));
            r.RegisterByName("Schoolgirl", new PrizeDrawSupporter(2, 4, 6));
            r.RegisterByName("Shauna", new Shauna());
            r.RegisterByName("Sidney", new Sidney());
            r.RegisterByName("Skaters' Park", new SkatersPark());
            r.RegisterByName("Spongy Gloves", new CardsBRS2.TypeGloves(EnergyType.Water));
            r.RegisterByName("Training Court", new TrainingCourt());
            // Energy
            r.RegisterByName("Fusion Strike Energy", new FusionStrikeEnergy());
        }

        // ================================================================== Filters and counters

        private static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        private static int OwnDamageCounters(EffectContext ctx) { return ctx.Pokemon.DamageCounters; }
        private static int DefendingEnergy(EffectContext ctx) { return ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending); }
        private static int OpponentVAndGx(EffectContext ctx) { return ctx.OpponentPokemon(mon => mon.Def.IsPokemonV || mon.Def.IsGX).Count; }
        private static int OpponentEnergyInPlay(EffectContext ctx) { return ctx.OpponentPokemon().Sum(mon => ctx.EnergyCount(mon)); }
        private static bool OpponentHasOnePrize(EffectContext ctx) { return ctx.Opp.Prizes.Count == 1; }
        private static bool SmallBench(EffectContext ctx) { return ctx.Me.Bench.Count <= 2; }

        // ================================================================== Shared helpers

        /// <summary>"If [condition], this attack does N more damage" (Cliff Edge Formation, Rear Attack).</summary>
        internal sealed class ConditionBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;
            private readonly Func<EffectContext, bool> condition;

            public ConditionBonusMon(string attack, int bonus, Func<EffectContext, bool> condition)
            {
                this.attack = attack;
                this.bonus = bonus;
                this.condition = condition;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (condition(ctx) ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Put N damage counters on this Pokémon" after the damage (Pain Explosion).</summary>
        internal sealed class SelfCountersMon : Mon
        {
            private readonly string attack;
            private readonly int counters;

            public SelfCountersMon(string attack, int counters)
            {
                this.attack = attack;
                this.counters = counters;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, counters);
                return true;
            }
        }

        /// <summary>The cards played 2 at once (Cross Switcher, Crossceiver): another copy in the hand.</summary>
        private static CardInstance Partner(EffectContext ctx)
        {
            return ctx.Me.Hand.FirstOrDefault(card => card != ctx.Card && card.Def.Name == ctx.Card.Def.Name);
        }

        /// <summary>
        /// "You must play 2 [this card] at once. (This effect works one time for 2 cards.)" The second copy is played with the
        /// first and both are discarded. It counts as a second Item card played (your ruling), so an effect that makes you flip
        /// before playing a Trainer flips for it too; on tails it's discarded and the pair's effect doesn't happen.
        /// </summary>
        private static bool PlayPartner(EffectContext ctx)
        {
            var partner = Partner(ctx);
            string name = ctx.Card.Def.Name;
            if (partner == null)
            {
                ctx.Log(name + " has to be played 2 at once; there's no second " + name + " in the hand, so it does nothing.");
                return false;
            }
            ctx.Me.Hand.Remove(partner);
            ctx.Engine.Emit(GameEventType.PlayTrainer, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " played a second " + name + " with it (2 at once).", partner.Id);
            ctx.Me.Discard.Add(partner);
            if (ctx.Engine.ForcesTrainerFlip(ctx.Player) && !ctx.Engine.FlipCoin(ctx.Player, "Quaking Fist (" + name + ")"))
            {
                ctx.Log("The second " + name + " was discarded instead of being used, so the pair does nothing.");
                return false;
            }
            return true;
        }

        // ================================================================== Fighting

        /// <summary>Dig It Up: look at the top card of your deck; you may discard it.</summary>
        private sealed class DigItUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dig It Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.LookAtTop(1);
                if (top.Count == 0) return true;
                if (ctx.AskYesNo("Dig It Up: the top card of your deck is " + top[0].Def.Name + ". Discard it?")) ctx.Engine.DiscardTopOfDeck(ctx.Player, 1);
                return true;
            }
        }

        /// <summary>
        /// Desperate Blast: if this Pokémon is in the Active Spot and is Knocked Out by damage from an attack from the opponent's
        /// Pokémon, put 10 damage counters on the Attacking Pokémon. Double-Edge: 30 damage to itself.
        /// </summary>
        private sealed class GolemFST : Mon
        {
            private static readonly Mon DoubleEdge = new RecoilMon("Double-Edge", 30);

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || !wasActive) return;
                var attacker = src.Engine.AttackingPokemonOf(src.Pokemon);
                if (attacker == null || attacker.Owner == src.Player) return;
                Announce(src, "Desperate Blast");
                src.Engine.PlaceDamageCounters(attacker, 10, "Desperate Blast");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DoubleEdge.HandleAttack(ctx, atk); }
        }

        /// <summary>Guts: if this Pokémon would be Knocked Out by damage from an attack, flip a coin; on heads it isn't, and its remaining HP becomes 10.</summary>
        private sealed class HariyamaFST : Mon
        {
            public override bool SurvivesKnockOut(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return false;
                Announce(src, "Guts");
                return src.Flip();
            }
        }

        /// <summary>Rapid Spin: switch this Pokémon with 1 of your Benched Pokémon; if you do, your opponent switches their Active Pokémon with 1 of their Benched Pokémon. Used for Rapid Spin, Gyro Ball, Interjet (the attack name is passed in).</summary>
        internal sealed class RapidSpinMon : Mon
        {
            private readonly string attack;

            public RapidSpinMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.SwitchSelfWithBench()) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Ancient Imprint: put damage counters on the opponent's Active Pokémon until its remaining HP is 60.</summary>
        private sealed class AncientImprintMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ancient Imprint") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                int counters = (ctx.RemainingHp(defending) - 60) / 10;
                if (counters > 0) ctx.PlaceCounters(defending, counters);
                return true;
            }
        }

        /// <summary>Earthen Boom: move all Energy from this Pokémon to your Benched Pokémon in any way you like.</summary>
        private sealed class EarthenBoomMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earthen Boom") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                foreach (var energy in me.Energy.ToList())
                {
                    if (ctx.Me.Bench.Count == 0 || !me.Energy.Contains(energy)) break;
                    var target = ctx.ChoosePokemon(ctx.MyBench(), "Earthen Boom: move " + energy.Def.Name + " to which Benched Pokémon?");
                    if (target != null) ctx.Engine.MoveEnergy(me, target, energy);
                }
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Bone Block: during the opponent's next turn, Pokémon can't be played from their hand to evolve the Defending Pokémon.</summary>
        private sealed class BoneBlockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bone Block") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new CardsOBF1.NoEvolveFromHandEffect(defending.Name + " can't be evolved from the hand during its next turn (Bone Block)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Call to Muster: search your deck for up to 2 basic Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class CallToMusterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Call to Muster") return false;
                CardsBRS1.SearchAttachAnyWay(ctx, CardsFST1.IsBasicEnergyCard, 2, AnyPokemon, "Call to Muster", "basic Energy cards");
                return true;
            }

            private static bool AnyPokemon(PokemonInPlay mon) { return true; }
        }

        /// <summary>Adversity Jaws: if the opponent's Active Pokémon is a Fire Pokémon, it is now Paralyzed.</summary>
        private sealed class AdversityJawsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Adversity Jaws") return false;
                var defending = ctx.Defending;
                bool fire = defending != null && ctx.Engine.IsType(defending, EnergyType.Fire);
                ctx.DamageDefending(atk.Damage);
                if (fire && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>
        /// Fusion Strike System: once during your turn, draw cards until you have as many cards in your hand as you have Fusion
        /// Strike Pokémon in play. Techno Blast: this Pokémon can't attack during your next turn.
        /// </summary>
        private sealed class GenesectV : Mon
        {
            private static readonly Mon TechnoBlast = new LockAfterAttackMon("Techno Blast", false);

            protected override string Activated { get { return "Fusion Strike System"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Count < CardsFST1.FusionStrikePokemonInPlay(ctx);
            }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(CardsFST1.FusionStrikePokemonInPlay(ctx)); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return TechnoBlast.HandleAttack(ctx, atk); }
        }

        /// <summary>Ingot Swing: during the opponent's next turn, prevent all damage done to this Pokémon by attacks from Pokémon that have an Ability.</summary>
        private sealed class IngotSwingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ingot Swing") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new CardsCRI.AbilityShieldEffect(ctx.Pokemon.Name + " takes no damage from attacks by Pokémon with an Ability during the opponent's next turn (Ingot Swing)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>Blue Assist: once during your turn, attach a Psychic Energy card from your hand to 1 of your Latias. Luster Purge: discard 2 Energy.</summary>
        private sealed class LatiosFST : Mon
        {
            private static readonly Mon LusterPurge = new CardsTWM1.SelfDiscardMon("Luster Purge", 2);

            protected override string Activated { get { return "Blue Assist"; } }

            private static bool IsPsychicEnergyCard(CardDef d) { return d.IsBasicEnergy && d.EnergyProvides == EnergyType.Psychic; }
            private static bool IsLatias(PokemonInPlay mon) { return mon.Def.Name == "Latias"; }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon(IsLatias).Count > 0 && ctx.Me.Hand.Any(card => IsPsychicEnergyCard(card.Def));
            }

            protected override void Ability(EffectContext ctx)
            {
                var options = ctx.Me.Hand.Where(card => IsPsychicEnergyCard(card.Def)).ToList();
                var energy = options.Count == 1 ? options[0] : ctx.ChooseCards(options, 1, 1, "Blue Assist: attach which Psychic Energy card?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(IsLatias), "Blue Assist: attach " + energy.Def.Name + " to which Latias?");
                ctx.Me.Hand.Remove(energy);
                ctx.Engine.AttachEnergyCard(energy, target, true);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return LusterPurge.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Slimy Room: while this Pokémon is Active, whenever the opponent tries to attach an Energy card from their hand to a
        /// Pokémon they flip a coin; on tails the card is discarded instead, and it doesn't use up their attachment for the turn.
        /// Every attachment from the hand counts, the normal one or by an effect (your ruling); the engine does the flip.
        /// </summary>
        private sealed class GoodraFST : Mon
        {
            public override bool FlipsForEnergyFromHand(EffectContext src, int player)
            {
                return src.Pokemon != null && player != src.Player && src.PokemonIsActive;
            }
        }

        /// <summary>Shell Trap: during the opponent's next turn, if this Pokémon is damaged by an attack (even if Knocked Out), 8 damage counters on the Attacking Pokémon.</summary>
        internal sealed class ShellTrapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shell Trap") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new RetaliateEffect("Shell Trap", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1, 8));
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Expert in Roundness: prevent all damage done to each of your Pokémon that has the Let's All Rollout attack by attacks from the opponent's Pokémon VMAX.</summary>
        private sealed class BlisseyFST : Mon
        {
            private static readonly Mon Rollout = new CardsFST1.LetsAllRolloutMon();

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target == null || info.Attacker == null) return damage;
                if (info.Target.Owner != src.Player || info.Attacker.Owner == src.Player || !info.Attacker.Def.IsVmax) return damage;
                return CardsFST1.HasRollout(info.Target) ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rollout.HandleAttack(ctx, atk); }
        }

        /// <summary>Sketching Trash: put up to 2 Fusion Strike Trainer cards from your discard pile into your hand.</summary>
        private sealed class SketchingTrashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sketching Trash") return false;
                var options = ctx.Me.Discard.Where(card => card.Def.IsTrainer && card.Def.IsFusionStrike).ToList();
                if (options.Count > 0)
                    ctx.DiscardToHand(ctx.ChooseCards(options, 0, Math.Min(2, options.Count), "Sketching Trash: choose up to 2 Fusion Strike Trainer cards"));
                return true;
            }
        }

        /// <summary>Willful Busybody: the opponent reveals their hand; choose a card there and put it on the bottom of their deck.</summary>
        private sealed class WillfulBusybodyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Willful Busybody") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                TefPokemon.RevealOpponentHand(ctx);
                if (ctx.Opp.Hand.Count == 0) return true;
                var pick = ctx.ChooseCards(ctx.Opp.Hand.ToList(), 1, 1, "Willful Busybody: put which card on the bottom of your opponent's deck?")[0];
                ctx.Opp.Hand.Remove(pick);
                ctx.Opp.Deck.Add(pick);
                ctx.Log(pick.Def.Name + " was put on the bottom of " + ctx.Engine.Their(ctx.Opponent) + " deck.");
                return true;
            }
        }

        /// <summary>Turn a Profit: if the opponent's Basic Pokémon is Knocked Out by damage from this attack, take 2 more Prize cards.</summary>
        private sealed class TurnAProfitMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Turn a Profit") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && ctx.InPlay(defending) && defending.Def.IsBasicPokemon && ctx.Engine.IsKnockedOut(defending))
                    ctx.AddEffect(new KnockOutPrizeBonusEffect("Turn a Profit", ctx.Player, defending.Uid, ctx.Turn, 2));
                return true;
            }
        }

        /// <summary>Leaflet Dance: attach any number of Grass Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class LeafletDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaflet Dance") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var grass = ctx.Me.Hand.Where(card => card.Def.IsBasicEnergy && card.Def.EnergyProvides == EnergyType.Grass).ToList();
                if (grass.Count > 0)
                    ctx.AttachCardsAnyWay(grass, grass.Count, (energy, target) => true, "Leaflet Dance: attach Grass Energy cards from your hand");
                return true;
            }
        }

        /// <summary>
        /// Solar Revelation: prevent all effects of attacks from the opponent's Pokémon done to your Pokémon that have Energy
        /// attached (damage isn't an effect; effects already there stay). Max Mindstorm: 60 for each Energy on the opponent's Pokémon.
        /// </summary>
        private sealed class EspeonVmax : Mon
        {
            private static readonly Mon Mindstorm = new CardsASR1.TimesMon("Max Mindstorm", OpponentEnergyInPlay);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target != null && target.Owner == src.Player && kind == EffectSourceKind.Attack && target.Energy.Count > 0;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Mindstorm.HandleAttack(ctx, atk); }
        }

        // ================================================================== Trainers

        /// <summary>Battle VIP Pass: only during your first turn; put up to 2 Basic Pokémon from your deck onto your Bench.</summary>
        private sealed class BattleVipPass : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.IsMyFirstTurn && ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToBench(CardsFST1.AnyCard, 2, "Battle VIP Pass: put up to 2 Basic Pokémon onto your Bench");
            }
        }

        /// <summary>Bug Catcher: draw 2 cards; flip a coin, on heads draw 2 more.</summary>
        private sealed class BugCatcher : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.Flip()) ctx.Draw(2);
            }
        }

        /// <summary>Chili &amp; Cilan &amp; Cress: search your deck for up to 3 Fusion Strike Pokémon and put them into your hand.</summary>
        private sealed class ChiliCilanCress : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.SearchToHand(IsFusionStrikePokemon, 3, "Chili & Cilan & Cress: choose up to 3 Fusion Strike Pokémon");
            }

            private static bool IsFusionStrikePokemon(CardDef d) { return d.IsPokemon && d.IsFusionStrike; }
        }

        /// <summary>Cram-o-matic: discard another Item card from your hand to play it; flip a coin, on heads search your deck for a card.</summary>
        private sealed class CramOMatic : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(card => card != ctx.Card && card.Def.IsItem);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var items = ctx.Me.Hand.Where(card => card.Def.IsItem).ToList();
                if (items.Count == 0) return;
                var cost = items.Count == 1 ? items[0] : ctx.ChooseCards(items, 1, 1, "Cram-o-matic: discard another Item card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);
                if (ctx.Flip()) ctx.SearchToHand(CardsFST1.AnyCard, 1, "Cram-o-matic: choose a card to put into your hand", false);
            }
        }

        /// <summary>
        /// Cross Switcher: played 2 at once. Switch 1 of the opponent's Benched Pokémon with their Active Pokémon; if you do,
        /// switch your Active Pokémon with 1 of your Benched Pokémon.
        /// </summary>
        private sealed class CrossSwitcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Partner(ctx) != null && ctx.Opp.Bench.Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!PlayPartner(ctx)) return;
                var switched = ctx.GustOpponent("Cross Switcher: choose 1 of your opponent's Benched Pokémon to switch in");
                if (switched != null && ctx.Me.Bench.Count > 0) ctx.SwitchMyActive("Cross Switcher: choose your new Active Pokémon");
            }
        }

        /// <summary>Crossceiver: played 2 at once. Put a Pokémon or a Supporter card from your discard pile into your hand.</summary>
        private sealed class Crossceiver : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsPokemon || d.IsSupporter; }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return Partner(ctx) != null && ctx.Me.Discard.Any(card => Fits(card.Def));
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (!PlayPartner(ctx)) return;
                var options = ctx.Me.Discard.Where(card => Fits(card.Def)).ToList();
                if (options.Count > 0) ctx.DiscardToHand(ctx.ChooseCards(options, 1, 1, "Crossceiver: put a Pokémon or a Supporter card into your hand"));
            }
        }

        /// <summary>Dancer: draw 2 cards; if you went second and it's your first turn, draw 3 more.</summary>
        private sealed class Dancer : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (ctx.State.FirstPlayer != ctx.Player && ctx.IsMyFirstTurn) ctx.Draw(3);
            }
        }

        /// <summary>Farewell Bell: if the Pokémon VMAX holding it is Knocked Out by damage from an opponent's attack, search your deck for a card.</summary>
        private sealed class FarewellBell : CardScript
        {
            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Pokemon == null || !src.Pokemon.Def.IsVmax || src.Me.Deck.Count == 0) return;
                src.Log("Farewell Bell: " + src.Engine.PlayerName(src.Player) + " may search for a card.");
                src.SearchToHand(CardsFST1.AnyCard, 1, "Farewell Bell: choose a card to put into your hand", false);
            }
        }

        /// <summary>Power Tablet: during this turn, your Fusion Strike Pokémon's attacks do 30 more damage to the opponent's Active Pokémon.</summary>
        private sealed class PowerTablet : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.AddEffect(new ConditionalDamageBonusEffect("Power Tablet: your Fusion Strike Pokémon's attacks do 30 more damage this turn", ctx.Player, ctx.Turn, 30, FusionStrikeAttacker));
            }

            private static bool FusionStrikeAttacker(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target)
            {
                return attacker != null && attacker.Def.IsFusionStrike;
            }
        }

        /// <summary>Quick Ball: discard another card from your hand to play it; search your deck for a Basic Pokémon.</summary>
        private sealed class QuickBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.Me.Hand.Any(card => card != ctx.Card);
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var cost = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Quick Ball: discard another card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, cost);
                ctx.SearchToHand(IsBasicPokemonCard, 1, "Quick Ball: choose a Basic Pokémon to put into your hand");
            }

            private static bool IsBasicPokemonCard(CardDef d) { return d.IsBasicPokemon; }
        }

        /// <summary>Schoolboy / Schoolgirl: draw 2 cards; if the opponent has exactly one of these numbers of Prize cards left, draw 2 more.</summary>
        private sealed class PrizeDrawSupporter : CardScript
        {
            private readonly int[] counts;

            public PrizeDrawSupporter(params int[] counts) { this.counts = counts; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Draw(2);
                if (counts.Contains(ctx.Opp.Prizes.Count)) ctx.Draw(2);
            }
        }

        /// <summary>Shauna: shuffle your hand into your deck, then draw 5 cards.</summary>
        private sealed class Shauna : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
            }
        }

        /// <summary>Sidney: the opponent reveals their hand; discard up to 2 in any combination of Pokémon Tool, Special Energy and Stadium cards from it.</summary>
        private sealed class Sidney : CardScript
        {
            private static bool Fits(CardDef d) { return d.IsTool || d.IsSpecialEnergy || d.IsStadiumCard; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (TefPokemon.OpponentShielded(ctx)) return;
                TefPokemon.RevealOpponentHand(ctx);
                var options = ctx.Opp.Hand.Where(card => Fits(card.Def)).ToList();
                if (options.Count == 0) return;
                var picks = ctx.ChooseCards(options, 0, Math.Min(2, options.Count), "Sidney: discard up to 2 Pokémon Tool, Special Energy and Stadium cards from your opponent's hand");
                foreach (var pick in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
            }
        }

        /// <summary>Skaters' Park: whenever either player's Active Pokémon retreats, basic Energy that would be discarded goes to their hand instead.</summary>
        private sealed class SkatersPark : CardScript
        {
            public override bool RetreatEnergyToHand(EffectContext src, int player) { return true; }
        }

        /// <summary>Training Court: once during each player's turn, that player may put a basic Energy card from their discard pile into their hand.</summary>
        private sealed class TrainingCourt : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.BasicEnergyInDiscard().Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var pile = ctx.BasicEnergyInDiscard();
                var pick = pile.Count == 1 ? pile : ctx.ChooseCards(pile, 1, 1, "Training Court: put a basic Energy card into your hand");
                ctx.DiscardToHand(pick);
                return false;
            }
        }

        /// <summary>
        /// Fusion Strike Energy: only attached to a Fusion Strike Pokémon (discarded if it's on anything else); provides every
        /// type, 1 Energy at a time; prevents all effects of the opponent's Pokémon's Abilities done to its Pokémon.
        /// </summary>
        private sealed class FusionStrikeEnergy : CardScript
        {
            public override List<int> ProvidedEnergy(EffectContext ctx) { return RecifeEnergy.Units(EnergyMask.AnyType, 1); }
            public override bool CanAttachTo(EffectContext ctx, PokemonInPlay target) { return target != null && target.Def.IsFusionStrike; }
            public override bool MustDiscard(EffectContext ctx) { return ctx.Pokemon != null && !ctx.Pokemon.Def.IsFusionStrike; }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return src.Pokemon != null && target == src.Pokemon && kind == EffectSourceKind.Ability;
            }
        }
    }
}
