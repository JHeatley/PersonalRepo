using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Furious Fists (FFI, v103), part 1: the Grass, Fire, Water, Lightning and Psychic Pokémon (1-43). Magmar and Jynx are
    /// Generations' cards. Amaura is a Restored Pokémon (Sail Fossil, Fossil Researcher).
    /// </summary>
    public static class CardsFFI1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("FFI-1", new CardsASC.RandomDiscardMon("Knock Off"));
            r.RegisterByKey("FFI-2", new ConditionAttackMon("Spit Poison", SpecialCondition.Poisoned));
            r.RegisterByKey("FFI-3", new VictreebelScent());
            r.RegisterByKey("FFI-4", new ComboMon(new CardsASC.SelfGuardMon("Guard Press", 20), new CardsSUM5.OptionalBoostRecoilMon("Giga Power", 40, 20)));
            r.RegisterByKey("FFI-5", new CardsUPR1.LessPerSelfCounterMon("Big Bang Horn", 10));
            r.RegisterByKey("FFI-7", new ComboMon(new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep), new MagicalLeafThirtyMon()));
            r.RegisterByKey("FFI-8", new FlipBonusMon("Jump On", 10));
            r.RegisterByKey("FFI-9", new ComboMon(new EvolvedFromThisTurnBonusMon("Raid", "Shelmet", 40), new CardsUNB1.AfterimageStrikeMon()));
            r.RegisterByKey("FFI-11", new ComboMon(new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1),
                new CardsFST2.ConditionBonusMon("Twin Bursts", 80, c => c.Me.Bench.Any(m => m.Name == "Electivire"))));
            r.RegisterByKey("FFI-12", new FlipBonusMon("Quick Attack", 30));
            r.RegisterByKey("FFI-13", new FlipBonusMon("Midair Strike", 20));
            r.RegisterByKey("FFI-14", new ComboMon(new NoRetreatMon("Clutch"), new DiscardThenSnipeMon("Burning Shot", 2, 150)));
            r.RegisterByKey("FFI-15", new CardsBST1.PanicVineMon("Spiral Current"));
            r.RegisterByKey("FFI-16", new CardsPFL.DamagedBonusMon("Finishing Blow", 50));
            r.RegisterByKey("FFI-17", new ComboMon(new Cards30CC.BenchHitMon("Steamroll", 30), new RecoilMon("Submission", 30)));
            r.RegisterByKey("FFI-18", new PolitoedSong());
            r.RegisterByKey("FFI-19", new ComboMon(new CardsMEW1.SpreadBenchMon("Blizzard", 10), new FlipBonusMon("Ice Edge", 30)));
            r.RegisterByKey("FFI-20", new ComboMon(new CardsDAA1.ItemLockAttackMon("Quaking Punch"), new GrenadeHammerMon()));
            r.RegisterByKey("FFI-21", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("FFI-22", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Igloo Hold", 20, false), new CardsROS2.AnyStadiumBonusMon("Mountain Drop", 40)));
            r.RegisterByKey("FFI-23", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("FFI-24", new ComboMon(new SwitchSelfMon("Reverse Thrust", false), new CardsMEG.EnergyBonusMon("Splash Cannon", EnergyType.Water, 20)));
            r.RegisterByKey("FFI-26", new AurorusShield());
            r.RegisterByKey("FFI-27", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("FFI-28", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("FFI-29", new FlipBonusMon("Ambush", 20));
            r.RegisterByKey("FFI-30", new ComboMon(new TagTeamSparkMon(), new CardsROS1.HeadlockMon("Gigavolt", 30)));
            r.RegisterByKey("FFI-31", new ComboMon(new PositiveHandMon(), new ParalyzeFlipMon("Electric Tail")));
            r.RegisterByKey("FFI-32", new CardsPAF1.BasicEnergyFromDiscardMon("Negative Discard", 2));
            r.RegisterByKey("FFI-33", new ComboMon(new CardsFST2.ConditionBonusMon("Raging Thunder Punch", 30, c => c.Defending != null && c.Defending.Tools.Count > 0),
                new LockAfterAttackMon("Lightning Slam", true)));
            r.RegisterByKey("FFI-34", new ComboMon(new BenchSearchAttackMon("Entrainment", 2), new CardsASR1.TimesMon("Energy Short", c => c.Defending == null ? 0 : c.EnergyCount(c.Defending))));
            r.RegisterByKey("FFI-35", new SinisterSuggestionMon());
            r.RegisterByKey("FFI-36", new ComboMon(new HandControlMon(), new ConditionAttackMon("Hypnoblast", SpecialCondition.Asleep)));
            r.RegisterByKey("FFI-38", new CardsPAR2.FlipConditionMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("FFI-39", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("FFI-40", new CardsDAA1.FortunateEyeMon());
            r.RegisterByKey("FFI-41", new GothitelleRoom());
            r.RegisterByKey("FFI-42", new FlipBonusMon("Rock Smash", 30));
            r.RegisterByKey("FFI-43", new CardsAOR2.StadiumBlowMon("Wreck", 60));
        }

        // ================================================================== Shared helpers

        /// <summary>"If this Pokémon evolved from [name] during this turn, this attack does N more damage" (Raid).</summary>
        internal sealed class EvolvedFromThisTurnBonusMon : Mon
        {
            private readonly string attack;
            private readonly string from;
            private readonly int bonus;

            public EvolvedFromThisTurnBonusMon(string attack, string from, int bonus)
            {
                this.attack = attack;
                this.from = from;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                bool evolved = me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == from;
                ctx.DamageDefending(atk.Damage + (evolved ? bonus : 0));
                return true;
            }
        }

        /// <summary>"Discard N Energy attached to this Pokémon. This attack does X damage to 1 of your opponent's Pokémon." (Burning Shot).</summary>
        internal sealed class DiscardThenSnipeMon : Mon
        {
            private readonly string attack;
            private readonly int discard;
            private readonly int amount;

            public DiscardThenSnipeMon(string attack, int discard, int amount)
            {
                this.attack = attack;
                this.discard = discard;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsTWM1.SelfDiscardMon.DiscardOwn(ctx, discard, atk.Name);
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Wafting Scent: once during your turn, discard a Grass Energy from this Pokémon; the opponent's Active is Confused and Poisoned. Spiral Drain.</summary>
        private sealed class VictreebelScent : Mon
        {
            private static readonly Mon Drain = new CardsTWM1.HealSelfMon("Spiral Drain", 30);

            protected override string Activated { get { return "Wafting Scent"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.Opp.Active != null && me.Energy.Any(en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Grass));
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (ctx.DiscardEnergyFrom(me, 1, "Wafting Scent: discard a Grass Energy from " + me.Name, en => CardsDRI1.ProvidesType(ctx, me, en, EnergyType.Grass)) == 0) return;
                var d = ctx.Opp.Active;
                if (d == null) return;
                ctx.ApplyCondition(d, SpecialCondition.Confused);
                ctx.ApplyCondition(d, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Drain.HandleAttack(ctx, atk); }
        }

        /// <summary>Magical Leaf (Leafeon): flip a coin; heads, 30 more damage and heal 30 from this Pokémon.</summary>
        internal sealed class MagicalLeafThirtyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magical Leaf") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage + (heads ? 30 : 0));
                if (heads && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 30);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>King's Song: ignore all Colorless Energy in the attack costs of your Poliwag, Poliwhirl and Poliwrath.</summary>
        private sealed class PolitoedSong : Mon
        {
            private static readonly HashSet<string> Family = new HashSet<string> { "Poliwag", "Poliwhirl", "Poliwrath" };

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon.Owner != src.Player || !Family.Contains(mon.Name)) return;
                cost.RemoveAll(t => t == EnergyType.Colorless);
            }
        }

        /// <summary>Grenade Hammer: also 30 damage to 2 of your own Benched Pokémon.</summary>
        private sealed class GrenadeHammerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grenade Hammer") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                var picks = bench.Count <= 2 ? bench : ctx.ChoosePokemonMany(bench, 2, 2, "Grenade Hammer: choose 2 of your Benched Pokémon (30 damage each)");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Ice Shield: your Water Pokémon with any Water Energy attached take 20 less from the opponent's attacks. Icy Wind: Asleep.</summary>
        private sealed class AurorusShield : Mon
        {
            private static readonly Mon Wind = new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var target = info.Target;
                if (target == null || target.Owner != src.Player || info.Attacker == null || info.Attacker.Owner == src.Player) return damage;
                if (!src.Engine.IsType(target, EnergyType.Water) || src.Engine.EnergyCountOfType(target, EnergyType.Water) == 0) return damage;
                return damage - 20;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Wind.HandleAttack(ctx, atk); }
        }

        // ================================================================== Lightning

        /// <summary>Tag Team Spark: 20 more damage for each Energy attached to your Magmortar (all of them added up: Compendium, 2014-08-28).</summary>
        private sealed class TagTeamSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tag Team Spark") return false;
                var magmortars = ctx.MyPokemon(m => m.Name == "Magmortar");
                int energy = magmortars.Sum(m => ctx.EnergyCount(m));
                ctx.DamageDefending(atk.Damage + 20 * energy);
                return true;
            }
        }

        /// <summary>Positive Hand: discard a basic Energy card from your hand; if you do, draw 2 cards.</summary>
        private sealed class PositiveHandMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Positive Hand") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count == 0) return true;
                var card = ctx.ChooseCards(basics, 1, 1, "Positive Hand: discard a basic Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(2);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>During the opponent's next turn, every coin flip they make is tails (Sinister Suggestion).</summary>
        internal sealed class AllTailsEffect : ContinuousEffect
        {
            public AllTailsEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool ForcesTails(GameEngine engine, int player) { return player != Owner; }
        }

        private sealed class SinisterSuggestionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sinister Suggestion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var effect = new AllTailsEffect("Every coin flip " + ctx.Engine.PlayerName(ctx.Opponent) + " makes during their next turn is tails (Sinister Suggestion)", ctx.Player, ctx.Turn + 1);
                effect.ActiveFromTurn = ctx.Turn + 1;
                ctx.AddEffect(effect);
                return true;
            }
        }

        /// <summary>
        /// Hand Control: the opponent reveals their hand; you choose a Supporter there, the opponent uses its effect as the effect of
        /// this attack with you making all its decisions, then the opponent discards it.
        /// </summary>
        private sealed class HandControlMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hand Control") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                CardsTWM1.RevealOpponentHand(ctx);
                var supporters = ctx.Opp.Hand.Where(c => c.Def.IsSupporter).ToList();
                if (supporters.Count == 0) return true;
                var card = ctx.ChooseCards(supporters, 1, 1, "Hand Control: choose a Supporter card in your opponent's hand")[0];
                ctx.Opp.Hand.Remove(card);
                ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " uses the effect of " + card.Def.Name + " (Hand Control).");
                ctx.Engine.RedirectDecisions(ctx.Opponent, ctx.Player);
                try { ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Opponent, card, null)); }
                finally { ctx.Engine.EndDecisionRedirect(); }
                ctx.Opp.Discard.Add(card);
                return true;
            }
        }

        /// <summary>Teleport Room: once during your turn, discard the Stadium in play to put a Stadium with a different name from your discard pile into play.</summary>
        private sealed class GothitelleRoom : Mon
        {
            private static readonly Mon Report = new Cards30C.PeerAtMon("Psy Report");

            protected override string Activated { get { return "Teleport Room"; } }

            private static List<CardInstance> Stadiums(EffectContext ctx)
            {
                var current = ctx.State.Stadium;
                return current == null ? new List<CardInstance>() : ctx.Me.Discard.Where(c => c.Def.IsStadiumAlone && c.Def.Name != current.Def.Name).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return Stadiums(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var pile = Stadiums(ctx);
                if (pile.Count == 0) return;
                var pick = ctx.ChooseCards(pile, 1, 1, "Teleport Room: put which Stadium into play?")[0];
                ctx.Engine.DiscardStadium();
                if (ctx.State.Stadium != null || !ctx.Me.Discard.Remove(pick)) return;
                ctx.Engine.PutStadiumIntoPlay(ctx.Player, pick);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Report.HandleAttack(ctx, atk); }
        }
    }
}
