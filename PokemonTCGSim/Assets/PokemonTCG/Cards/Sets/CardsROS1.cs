using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Roaring Skies (ROS, v99), part 1: the Grass, Fire, Water, Lightning, Psychic, Fighting, Darkness and Metal Pokémon (1-42).
    /// Articuno (16) and Zapdos (23) are the same cards as Generations'. The Ancient Traits Δ Plus, Δ Evolution and Δ Wild are
    /// applied by the engine from the card data.
    /// </summary>
    public static class CardsROS1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("ROS-1", new LoatheMon());
            r.RegisterByKey("ROS-2", new CardsASR1.TimesMon("Shake It Off", c => c.OpponentPokemon(m => c.Engine.IsType(m, EnergyType.Colorless)).Count));
            r.RegisterByKey("ROS-3", new BenchFamilyMon("Flock", "Wurmple", 1));
            r.RegisterByKey("ROS-4", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("ROS-5", new BeautiflyROS());
            r.RegisterByKey("ROS-6", new ComboMon(new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned), new CardsSP1.FlipNoDamageMon("Iron Defense")));
            r.RegisterByKey("ROS-7", new ComboMon(new ConditionAttackMon("Psybeam", SpecialCondition.Confused),
                new ConditionsAttackMon("Twilight Poison", SpecialCondition.Asleep, SpecialCondition.Poisoned)));
            r.RegisterByKey("ROS-8", new BenchOnlyMon("Wind Shard", 50));
            r.RegisterByKey("ROS-9", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("ROS-10", new NinjaskROS());
            r.RegisterByKey("ROS-11", new ComboMon(new CursedRainMon(), new CardsASR1.TimesMon("Hopeless Scream", c => c.Pokemon.DamageCounters)));
            r.RegisterByKey("ROS-12", new FlipBonusMon("Stomp", 20));
            r.RegisterByKey("ROS-13", new ComboMon(new DrawAttackMon("Me First", 1), new ParalyzeFlipMon("Psy Bolt")));
            r.RegisterByKey("ROS-14", new PeckOffAllMon());
            r.RegisterByKey("ROS-15", new ComboMon(new GrandLoopMon(), new RecoilMon("Brave Bird", 20)));
            r.RegisterByKey("ROS-17", new ComboMon(new ConditionAttackMon("Chilling Sigh", SpecialCondition.Asleep), new CardsWHT.FlipMoreMon("Tri Edge", 3, 40)));
            r.RegisterByKey("ROS-19", new ComboMon(new CardsDAA1.DrainMon("Swallow"), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("ROS-20", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("ROS-21", new ComboMon(new ParalyzeFlipMon("Thunder Wave"), new RecoilMon("Big Explosion", 60)));
            r.RegisterByKey("ROS-22", new ComboMon(new CardsPFL.FlipUntilTailsTimesMon("Continuous Tumble"), new EnergyBombMon()));
            r.RegisterByKey("ROS-25", new CardsMEG.SnipeMon("Random Spark", 30));
            r.RegisterByKey("ROS-26", new ComboMon(new HeadlockMon("Headlock", 30), new RecoilMon("Voltage Rush", 50)));
            r.RegisterByKey("ROS-28", new CardsASR1.TimesMon("Psywave", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending)));
            r.RegisterByKey("ROS-29", new ComboMon(new FutureSightFiveMon(), new StressfulEyeMon()));
            r.RegisterByKey("ROS-30", new BlehMon());
            r.RegisterByKey("ROS-31", new BanetteROS());
            r.RegisterByKey("ROS-32", new ComboMon(new EvolutionJammerMon(), new CardsBKP2.CountersOnActiveMon("Curse Deeply", 5)));
            r.RegisterByKey("ROS-33", new FirstTurnMon("Close Encounter", new ComboMon(new DrawAttackMon("Close Encounter", 2), new CardsBKT2.SelfBoostMon("Overdrive Smash", 60))));
            r.RegisterByKey("ROS-34", new ComboMon(new CardsASC.PushDownMon("Swift Lunge"), new CardsASR1.PlusMon("Piercing Prizes", 20, c => c.Me.Prizes.Count)));
            r.RegisterByKey("ROS-35", new UnwaveringBladeMon());
            r.RegisterByKey("ROS-36", new CardsCES1.FlipConditionsMon("Stun Poison", SpecialCondition.Paralyzed, SpecialCondition.Poisoned));
            r.RegisterByKey("ROS-37", new CardsFST1.TwoBenchedMon("Rock Slide", 20));
            r.RegisterByKey("ROS-38", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("ROS-39", new FlipBonusMon("Midair Strike", 40));
            r.RegisterByKey("ROS-40", new AbsolROS());
            r.RegisterByKey("ROS-41", new ComboMon(new CardsDAA1.RandomHandToDeckMon("Rip Off", 1), new CardsPAR2.FlipConditionMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("ROS-42", new ComboMon(new DiminutiveDesireMon(), new DoomDesireMon()));
        }

        // ================================================================== Shared helpers

        /// <summary>"If you go first, you can use this attack during your first turn" for one attack of the card.</summary>
        internal sealed class FirstTurnMon : Mon
        {
            private readonly string attack;
            private readonly Mon inner;

            public FirstTurnMon(string attack, Mon inner)
            {
                this.attack = attack;
                this.inner = inner;
            }

            public override bool UsableOnFirstTurn(EffectContext ctx, int attackIndex) { return ctx.Attack(attackIndex).Name == attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return inner.HandleAttack(ctx, atk); }
        }

        /// <summary>"Search your deck for up to N [name] and put them onto your Bench" (Flock, Afterimage Assault).</summary>
        internal sealed class BenchFamilyMon : Mon
        {
            private readonly string attack;
            private readonly string name;
            private readonly int count;

            public BenchFamilyMon(string attack, string name, int count)
            {
                this.attack = attack;
                this.name = name;
                this.count = count;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(count, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                // Ninjask is a Stage 1: it goes straight onto the Bench (the card says so), not only Basic Pokémon.
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == name && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    attack + ": put up to " + n + " " + name + " onto your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>"Your opponent's Active Pokémon is now [A] and [B]."</summary>
        internal sealed class ConditionsAttackMon : Mon
        {
            private readonly string attack;
            private readonly SpecialCondition[] conditions;

            public ConditionsAttackMon(string attack, params SpecialCondition[] conditions)
            {
                this.attack = attack;
                this.conditions = conditions;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                foreach (var condition in conditions) ctx.ApplyCondition(defending, condition);
                return true;
            }
        }

        /// <summary>"This attack does N damage to 1 of your opponent's Benched Pokémon" and nothing to the Active (Wind Shard: the printed number is the Bench damage).</summary>
        internal sealed class BenchOnlyMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public BenchOnlyMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageOneBenched(amount);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Loathe: flip a coin; heads, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class LoatheMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Loathe") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Bench.Count > 0 && ctx.Flip()) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Miraculous Scales: no damage from the opponent's Pokémon-EX's attacks. Whirlwind: the opponent switches.</summary>
        private sealed class BeautiflyROS : Mon
        {
            private static readonly Mon Whirlwind = new CardsDRI1.WhirlwindMon("Whirlwind");

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.Attacker != null && info.Attacker.Owner != me.Owner && info.Attacker.Def.IsPokemonEX;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Whirlwind.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Wing Buzz: once during your turn, if this Pokémon is Active, you may discard a card from your hand to discard the top card
        /// of the opponent's deck. Afterimage Assault: bench up to 2 Ninjask from your deck.
        /// </summary>
        private sealed class NinjaskROS : Mon
        {
            private static readonly Mon Assault = new BenchFamilyMon("Afterimage Assault", "Ninjask", 2);

            protected override string Activated { get { return "Wing Buzz"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active == ctx.Pokemon && ctx.Me.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Wing Buzz: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                if (ctx.Opp.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Assault.HandleAttack(ctx, atk); }
        }

        /// <summary>Cursed Rain: 1 damage counter on each of the opponent's Pokémon, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class CursedRainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cursed Rain") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon()) ctx.PlaceCounters(mon, 1);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Peck Off: before doing damage, discard all Pokémon Tools from the opponent's Active Pokémon.</summary>
        private sealed class PeckOffAllMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Peck Off") return false;
                var d = ctx.Defending;
                if (d != null && d.Tools.Count > 0 && ctx.CanAffect(d))
                    foreach (var tool in d.Tools.ToList()) ctx.Engine.DiscardTool(d, tool);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Grand Loop: draw 3 cards; you may switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class GrandLoopMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Loop") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(3);
                if (ctx.Me.Bench.Count > 0 && ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Grand Loop: switch this Pokémon with 1 of your Benched Pokémon?"))
                    ctx.SwitchSelfWithBench();
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Energy Bomb: you may move all Energy from this Pokémon to your Benched Pokémon in any way.</summary>
        private sealed class EnergyBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Bomb") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me) || me.Energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Energy Bomb: move all Energy from " + me.Name + " to your Benched Pokémon?")) return true;
                foreach (var card in me.Energy.ToList())
                {
                    var bench = ctx.MyBench();
                    if (bench.Count == 0) break;
                    var target = ctx.ChoosePokemon(bench, "Energy Bomb: move " + card.Def.Name + " to which Benched Pokémon?");
                    ctx.Engine.MoveEnergy(me, target, card);
                }
                return true;
            }
        }

        /// <summary>Headlock: flip a coin; heads, 30 more damage; tails, the opponent's Active Pokémon is Paralyzed. Used for Headlock, Nyan Press, Gigavolt, Seething Tentacles (the attack name and the numbers are passed in).</summary>
        internal sealed class HeadlockMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public HeadlockMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? amount : 0));
                if (!heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Future Sight (Xatu): look at the top 5 cards of either player's deck and put them back in any order.</summary>
        private sealed class FutureSightFiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Future Sight") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                bool mine = ctx.Me.Deck.Count > 0;
                bool theirs = ctx.Opp.Deck.Count > 0;
                if (!mine && !theirs) return true;
                bool own = mine && (!theirs || ctx.ChooseOption("Future Sight: look at the top 5 cards of which deck?", "Your deck", "Your opponent's deck") == 0);
                if (own) CardsOBF2.ReorderOwnTop(ctx, 5, "Future Sight");
                else CardsDRI2.ReorderOpponentTop(ctx, 5, "Future Sight");
                return true;
            }
        }

        /// <summary>Stressful Eye: the opponent reveals their hand; discard a Trainer card from it.</summary>
        private sealed class StressfulEyeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stressful Eye") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                CardsTWM1.RevealOpponentHand(ctx);
                var trainers = ctx.Opp.Hand.Where(c => c.Def.IsTrainer).ToList();
                if (trainers.Count == 0) return true;
                var pick = ctx.ChooseCards(trainers, 1, 1, "Stressful Eye: discard which Trainer card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                return true;
            }
        }

        /// <summary>Bleh: discard a Special Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class BlehMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bleh") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Any(en => en.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Bleh: discard a Special Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Bleh: discard a Special Energy from " + target.Name, en => en.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Tool Concealment: every Pokémon Tool in play has no effect (engine; a θ Stop Pokémon's own Tools keep working against it).</summary>
        private sealed class BanetteROS : Mon
        {
            public override bool DisablesAllTools { get { return true; } }
        }

        /// <summary>Evolution Jammer: the opponent can't play Pokémon from their hand to evolve during their next turn (Δ Evolution doesn't get around it: 2015-05-07).</summary>
        private sealed class EvolutionJammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Evolution Jammer") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Opp.EvolveLockTurn = ctx.Turn + 1;
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " can't evolve Pokémon from " + ctx.Engine.Their(ctx.Opponent) + " hand during " + ctx.Engine.Their(ctx.Opponent) + " next turn (Evolution Jammer).");
                return true;
            }
        }

        /// <summary>Unwavering Blade: also 30 damage to each of the opponent's Benched Pokémon that has any damage counters on it.</summary>
        private sealed class UnwaveringBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Unwavering Blade") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.Opp.Bench.Where(m => m.DamageCounters > 0).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        // ================================================================== Darkness / Metal

        /// <summary>
        /// Cursed Eyes: played from your hand onto your Bench, you may move 3 damage counters from 1 of the opponent's Pokémon to another
        /// of theirs (all of them if it has fewer). Mach Claw: not affected by Resistance.
        /// </summary>
        private sealed class AbsolROS : Mon
        {
            private static readonly Mon Claw = new CardsFLF1.NoResistanceMon("Mach Claw");

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var donors = ctx.OpponentPokemon(m => m.DamageCounters > 0);
                bool ready = donors.Count > 0 && ctx.Opp.InPlay().Count() >= 2;
                if (!TriggerOnce(ctx, "Cursed Eyes", ready, "Cursed Eyes: move 3 damage counters from 1 of your opponent's Pokémon to another of their Pokémon?")) return;
                var donor = ctx.ChoosePokemon(donors, "Cursed Eyes: move damage counters from which of your opponent's Pokémon?");
                var others = ctx.OpponentPokemon(m => m != donor);
                if (donor == null || others.Count == 0) return;
                var receiver = ctx.ChoosePokemon(others, "Cursed Eyes: move the damage counters to which of your opponent's Pokémon?");
                ctx.Engine.MoveDamageCounters(donor, receiver, Math.Min(3, donor.DamageCounters), "Cursed Eyes");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claw.HandleAttack(ctx, atk); }
        }

        /// <summary>Diminutive Desire: look at the top 7 cards of your deck, put 1 into your hand, shuffle the rest back.</summary>
        private sealed class DiminutiveDesireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Diminutive Desire") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.LookAtTop(7);
                var pick = ctx.ChooseCards(top, 1, 1, "Diminutive Desire: put a card into your hand")[0];
                if (ctx.Me.Deck.Remove(pick)) ctx.Me.Hand.Add(pick);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>At the end of the opponent's next turn, the Defending Pokémon is Knocked Out (Doom Desire). A game already over isn't changed (2015-05-07).</summary>
        internal sealed class DoomDesireEffect : ContinuousEffect
        {
            private readonly int turn;

            public DoomDesireEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true)
            {
                turn = expiresAfterTurn;
            }

            public override void OnEndOfTurn(GameEngine engine)
            {
                if (engine.State.TurnNumber != turn || engine.State.IsOver) return;
                var mon = engine.State.FindPokemon(TargetUid);
                if (mon != null) engine.KnockOut(mon, "Doom Desire");
            }
        }

        /// <summary>Doom Desire: discard all Energy from this Pokémon; the Defending Pokémon is Knocked Out at the end of the opponent's next turn.</summary>
        private sealed class DoomDesireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doom Desire") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.Engine.DiscardAllEnergy(me);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.AddEffect(new DoomDesireEffect(defending.Name + " is Knocked Out at the end of its next turn (Doom Desire)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }
    }
}
