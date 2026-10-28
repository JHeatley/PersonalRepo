using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Lost Thunder (LOT, v81), part 1: Grass (LOT 1-37), plus helpers the other parts share. No regulation mark: Expanded.
    /// Your rulings used: Skiploom's Floral Path to the Sky does nothing if no Jumpluff is found (v81).
    /// </summary>
    public static class CardsLOT1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("LOT-1", new ParalyzeFlipMon("Bind"));
            r.RegisterByKey("LOT-2", new CardsFST2.ConditionBonusMon("Hefty Whip", 130, c => c.Pokemon != null && c.Pokemon.HealedOnTurn == c.Turn));
            r.RegisterByKey("LOT-3", new CardsDEX.FuryCutterMon(70, 50, 20));
            r.RegisterByKey("LOT-4", new NoRetreatMon("Grip and Squeeze"));
            r.RegisterByKey("LOT-5", new CardsVIV1.SynthesisMon());
            r.RegisterByKey("LOT-6", new CardsTWM1.HealSelfMon("Mini Drain", 10));
            r.RegisterByKey("LOT-7", new ConditionAttackMon("Soothing Scent", SpecialCondition.Asleep));
            r.RegisterByKey("LOT-8", new MeganiumLOT());
            r.RegisterByKey("LOT-9", new SpinarakLOT());
            r.RegisterByKey("LOT-10", new ComboMon(new TrappingThreadMon(), new ConditionAttackMon("Poison Jab", SpecialCondition.Poisoned)));
            r.RegisterByKey("LOT-11", new CardsUNM1.NamedBenchSearchMon("Multiply", "Hoppip", 1));
            r.RegisterByKey("LOT-13", new SkiploomLOT());
            r.RegisterByKey("LOT-14", new CardsASR1.TimesMon("Lost March", LostMarchCount));
            r.RegisterByKey("LOT-15", new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"));
            r.RegisterByKey("LOT-16", new ShuckleLOT());
            r.RegisterByKey("LOT-17", new ShuckleGX());
            r.RegisterByKey("LOT-18", new CardsFST2.ConditionBonusMon("Powerful Friends", 90, c => c.Me.Bench.Any(m => m.Def.Stage == Stage.Stage2)));
            r.RegisterByKey("LOT-19", new CelebiPrismStar());
            r.RegisterByKey("LOT-20", new SearchAttackMon("Find a Friend", d => d.IsPokemon && d.PokemonType == EnergyType.Grass, 1));
            r.RegisterByKey("LOT-21", new GrovyleLOT());
            r.RegisterByKey("LOT-22", new SceptileGX());
            r.RegisterByKey("LOT-24", new ConditionAttackMon("Poison Sting", SpecialCondition.Poisoned));
            r.RegisterByKey("LOT-25", new CocoonCollectorMon());
            r.RegisterByKey("LOT-26", new ComboMon(new CardsMEG.SnipeMon("Skill Dive", 50), new ButterflyEdgeMon("Butterfly Edge")));
            r.RegisterByKey("LOT-27", new CocoonCollectorMon());
            r.RegisterByKey("LOT-28", new DustoxLOT());
            r.RegisterByKey("LOT-29", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("LOT-30", new NinjaskLOT());
            r.RegisterByKey("LOT-31", new CardsUNM1.NamedBenchSearchMon("Bee March", "Combee", 3));
            r.RegisterByKey("LOT-32", new CommandingQueenMon());
            r.RegisterByKey("LOT-33", new ShayminLOT());
            r.RegisterByKey("LOT-34", new ComboMon(new DrawAttackMon("Double Draw", 2), new CardsFST2.ConditionBonusMon("Sensitive Blade", 80, SupporterPlayed), new BreezeAwayMon()));
            r.RegisterByKey("LOT-35", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("LOT-36", new CardsASC.NextTurnBoostMon("Leaf Wallop", 50));
            r.RegisterByKey("LOT-37", new ComboMon(new CardsASR1.TimesMon("Heavy Punch", c => c.Opp.Bench.Count), new CardsFST1.FlipRecoilMon("Wild Tackle", 30)));
        }

        // ================================================================== Shared helpers

        /// <summary>Lost March: the player's Pokémon in the Lost Zone, not counting Prism Star (◇) Pokémon.</summary>
        internal static int LostMarchCount(EffectContext ctx)
        {
            return ctx.Me.LostZone.Count(c => c.Def.IsPokemon && !c.Def.IsPrismStar);
        }

        /// <summary>"If you played a Supporter card from your hand during this turn" (Sensitive Blade, Sensitive Ray).</summary>
        internal static bool SupporterPlayed(EffectContext ctx) { return ctx.Me.SupporterPlayedThisTurn; }

        /// <summary>Poisoned, with N damage counters between turns instead of 1 (Triple Poison, Hazardous Evolution).</summary>
        internal static void PoisonWith(EffectContext ctx, PokemonInPlay target, int counters)
        {
            if (target == null) return;
            if (ctx.ApplyCondition(target, SpecialCondition.Poisoned)) target.PoisonCounterBase = counters;
        }

        /// <summary>The player has a Pokémon Tool with "Fairy Charm" in its name attached to this Pokémon.</summary>
        internal static bool HasFairyCharm(PokemonInPlay mon)
        {
            return mon != null && mon.Tools.Any(t => t.Def.Name.IndexOf("Fairy Charm", StringComparison.Ordinal) >= 0);
        }

        /// <summary>Damage to the opponent's Active Pokémon with Weakness and/or Resistance left out (Mind Shock, Sonic Wing...).</summary>
        internal sealed class PlainDamageMon : Mon
        {
            private readonly string attack;
            private readonly bool weakness;

            /// <summary>weakness: Weakness still applies and only Resistance is left out.</summary>
            public PlainDamageMon(string attack, bool weakness)
            {
                this.attack = attack;
                this.weakness = weakness;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (defending != null) ctx.DamageWith(defending, atk.Damage, weakness, false, false);
                return true;
            }
        }

        /// <summary>"If this Pokémon evolved / [condition], this attack does nothing" checked before the damage.</summary>
        internal sealed class DoesNothingUnlessMon : Mon
        {
            private readonly string attack;
            private readonly Func<EffectContext, bool> works;

            public DoesNothingUnlessMon(string attack, Func<EffectContext, bool> works)
            {
                this.attack = attack;
                this.works = works;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (!works(ctx)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Quick-Ripening Herb: once during your turn, choose 1 of your Basic Pokémon in play; if you have a Stage 2 card in your hand
        /// that evolves from it, put that card on it to evolve it. Works on your first turn and on a Pokémon put into play this turn.
        /// </summary>
        private sealed class MeganiumLOT : Mon
        {
            protected override string Activated { get { return "Quick-Ripening Herb"; } }

            private static bool Fits(EffectContext ctx, CardDef stage2, PokemonInPlay basic)
            {
                string stage1 = stage2.EvolvesFrom;
                string basicName;
                return stage2.IsPokemon && stage2.Stage == Stage.Stage2 && !string.IsNullOrEmpty(stage1)
                    && ctx.Engine.PreEvolutionOf.TryGetValue(stage1, out basicName) && basicName == basic.Name
                    && !ctx.Engine.PokemonPlayBlocked(ctx.Player, stage2);
            }

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.IsBasic && m.CantEvolveOnTurn != ctx.Turn && !ctx.Engine.EvolutionFromHandBlocked(m)
                                          && ctx.Me.Hand.Any(c => Fits(ctx, c.Def, m)));
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.EvolveLockTurn != ctx.Turn && Targets(ctx).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var basic = ctx.ChoosePokemon(Targets(ctx), "Quick-Ripening Herb: choose a Basic Pokémon to evolve");
                var cards = ctx.Me.Hand.Where(c => Fits(ctx, c.Def, basic)).ToList();
                if (cards.Count == 0) return;
                var stage2 = ctx.ChooseCards(cards, 1, 1, "Quick-Ripening Herb: choose the Stage 2 card")[0];
                ctx.Me.Hand.Remove(stage2);
                ctx.Engine.EvolvePokemon(basic, stage2, true);
            }
        }

        /// <summary>Spider Scram: the opponent's Active Pokémon is Paralyzed and Poisoned; this Pokémon and its cards go to the Lost Zone.</summary>
        private sealed class SpinarakLOT : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spider Scram") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                if (ctx.InPlay(ctx.Pokemon)) CardsLOR1.PokemonToLostZone(ctx, ctx.Pokemon, true);
                return true;
            }
        }

        /// <summary>
        /// Trapping Thread: during the opponent's next turn, whenever they play an Item or Supporter card from their hand, its effects
        /// on the Defending Pokémon are prevented (Switch, Guzma's switch, Potion, Super Scoop Up... don't reach it).
        /// </summary>
        private sealed class TrappingThreadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trapping Thread") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending))
                    ctx.AddEffect(new TrappingThreadEffect(defending.Name + " isn't affected by the Item and Supporter cards its owner plays during their next turn (Trapping Thread)",
                        ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class TrappingThreadEffect : ContinuousEffect
        {
            public TrappingThreadEffect(string name, int owner, int targetUid, int expiresAfterTurn)
                : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsTrainerEffects(GameEngine engine, PokemonInPlay target, CardDef trainer)
            {
                if (target == null || trainer == null || target.Uid != TargetUid || target.Owner == Owner) return false;
                return engine.State.CurrentPlayer == target.Owner && (trainer.IsItem || trainer.IsSupporter);
            }
        }

        /// <summary>
        /// Floral Path to the Sky: once during your turn, search your deck for Jumpluff; this Pokémon and all its cards go to the
        /// Lost Zone and Jumpluff is put in its place (a new Pokémon: no damage, no attached cards). No Jumpluff: nothing happens
        /// (your ruling, v81).
        /// </summary>
        private sealed class SkiploomLOT : Mon
        {
            protected override string Activated { get { return "Floral Path to the Sky"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Jumpluff" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), 1, "Floral Path to the Sky: choose Jumpluff");
                if (found.Count == 0 || !ctx.InPlay(me))
                {
                    if (found.Count > 0) ctx.Me.Deck.AddRange(found);
                    ctx.ShuffleMyDeck();
                    return;
                }
                var player = ctx.Me;
                bool wasActive = player.Active == me;
                int index = player.Bench.IndexOf(me);
                CardsLOR1.PokemonToLostZone(ctx, me, true);
                var jumpluff = found[0];
                if (wasActive) ctx.Engine.PutOntoActive(ctx.Player, jumpluff);
                else
                {
                    var mon = ctx.Engine.PutOntoBench(ctx.Player, jumpluff, false);
                    if (mon != null && index >= 0 && player.Bench.Remove(mon)) player.Bench.Insert(Math.Min(index, player.Bench.Count), mon);
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Fresh Squeezed: when played from your hand onto your Bench during your turn, you may search your deck for up to 3 basic
        /// Energy cards and discard them. Energy Drink: attach 2 basic Energy cards from your discard pile to your Pokémon.
        /// </summary>
        private sealed class ShuckleLOT : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Fresh Squeezed", ctx.Me.Deck.Count > 0, "Fresh Squeezed: search your deck for up to 3 basic Energy cards and discard them?")) return;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Fresh Squeezed: choose up to 3 basic Energy cards to discard");
                foreach (var card in found) ctx.Me.Discard.Add(card);
                if (found.Count > 0) ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " discarded " + found.Count + " basic Energy " + (found.Count == 1 ? "card" : "cards") + " from their deck (Fresh Squeezed).");
                ctx.ShuffleMyDeck();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Drink") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0) ctx.AttachCardsAnyWay(energy, 2, (c, m) => true, "Energy Drink: attach basic Energy from your discard pile");
                return true;
            }
        }

        /// <summary>
        /// Protective Shell: prevent all damage done to this Pokémon by attacks from the opponent's Pokémon with 2 or fewer Energy
        /// attached. Triple Poison: Poisoned, 3 damage counters between turns. Wrap-GX: Paralyzed.
        /// </summary>
        private sealed class ShuckleGX : Mon
        {
            private static readonly Mon Wrap = new ConditionAttackMon("Wrap-GX", SpecialCondition.Paralyzed);

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                if (me == null || info.Target != me || info.Attacker == null || !info.AgainstOpponent) return false;
                return src.Engine.EnergyCount(info.Attacker) <= 2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Wrap.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Triple Poison") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (defending != null && TefPokemon.StillDefending(ctx, defending)) PoisonWith(ctx, defending, 3);
                return true;
            }
        }

        /// <summary>
        /// Time Distortion: devolve any number of your Benched Pokémon as many times as you like; the Evolution cards removed go to
        /// your hand. Leech Seed: heal 20 damage from this Pokémon.
        /// </summary>
        private sealed class CelebiPrismStar : Mon
        {
            private static readonly Mon Seed = new CardsTWM1.HealSelfMon("Leech Seed", 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Seed.HandleAttack(ctx, atk)) return true;
                if (atk.Name != "Time Distortion") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var evolved = ctx.MyBench().Where(m => m.Stack.Count > 1 && !m.Def.IsMultiCard).ToList();
                if (evolved.Count == 0)
                {
                    ctx.Log("Time Distortion: there's no evolved Benched Pokémon to devolve.");
                    return true;
                }
                var picks = ctx.ChoosePokemonMany(evolved, 0, evolved.Count, "Time Distortion: choose any of your Benched Pokémon to devolve");
                foreach (var mon in picks)
                {
                    if (!ctx.InPlay(mon)) continue;
                    int levels = mon.Stack.Count - 1;
                    if (levels <= 0) continue;
                    int times = levels == 1 ? 1 : ctx.AskNumber(1, levels, "Time Distortion: devolve " + mon.Name + " how many times?");
                    ctx.Engine.Devolve(mon, times);
                }
                return true;
            }
        }

        /// <summary>Sunshine Grace: once during your turn, search your deck for a Grass Pokémon, reveal it and put it into your hand.</summary>
        private sealed class GrovyleLOT : Mon
        {
            protected override string Activated { get { return "Sunshine Grace"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => d.IsPokemon && d.PokemonType == EnergyType.Grass, 1, "Sunshine Grace: choose a Grass Pokémon");
            }
        }

        /// <summary>
        /// Mach Cut: discard a Special Energy from the opponent's Active Pokémon. Leaf Cyclone: move a Grass Energy from this Pokémon
        /// to 1 of your Benched Pokémon. Jungle Heal-GX: heal all damage from each of your Pokémon with any Grass Energy attached.
        /// </summary>
        private sealed class SceptileGX : Mon
        {
            private static readonly Mon Mach = new CardsASR1.DiscardDefendingSpecialMon("Mach Cut", false);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Mach.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Leaf Cyclone")
                {
                    var me = ctx.Pokemon;
                    ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me) || ctx.Me.Bench.Count == 0) return true;
                    var grass = me.Energy.Where(c => CardsTEU1.ProvidesType(ctx, me, c, EnergyType.Grass)).ToList();
                    if (grass.Count == 0) return true;
                    var card = ctx.ChooseCards(grass, 1, 1, "Leaf Cyclone: choose a Grass Energy to move")[0];
                    var dest = ctx.ChoosePokemon(ctx.MyBench(), "Leaf Cyclone: move " + card.Def.Name + " to which Benched Pokémon?");
                    ctx.Engine.MoveEnergy(me, dest, card);
                    return true;
                }
                if (atk.Name == "Jungle Heal-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.MyPokemon())
                        if (ctx.Engine.HasEnergyOfType(mon, EnergyType.Grass)) CardsTEU1.HealAll(ctx, mon);
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Cocoon Collector (Silcoon, Cascoon): search your deck for up to 4 in any combination of Silcoon and Cascoon and put them
        /// onto your Bench.
        /// </summary>
        private sealed class CocoonCollectorMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cocoon Collector") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int n = Math.Min(4, ctx.BenchSpace);
                if (n <= 0 || ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && (d.Name == "Silcoon" || d.Name == "Cascoon") && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, d), n,
                    "Cocoon Collector: put up to " + n + " Silcoon and Cascoon onto your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Butterfly Edge: if this Knocks Out the opponent's Pokémon, prevent all effects of attacks, damage included, done to this Pokémon next turn. Used for Butterfly Edge, Shadowy Side Kick (the attack name is passed in).</summary>
        internal sealed class ButterflyEdgeMon : Mon
        {
            private readonly string attack;

            public ButterflyEdgeMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending != null && ctx.InPlay(defending) && ctx.Engine.IsKnockedOut(defending) && ctx.InPlay(ctx.Pokemon)) ctx.ProtectSelfNextTurn();
                return true;
            }
        }

        /// <summary>
        /// Hazardous Evolution: when played from your hand to evolve during your turn, you may make the opponent's Active Pokémon
        /// Paralyzed and Poisoned (3 damage counters between turns).
        /// </summary>
        private sealed class DustoxLOT : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Hazardous Evolution", ctx.Opp.Active != null, "Hazardous Evolution: make your opponent's Active Pokémon Paralyzed and Poisoned?")) return;
                var target = ctx.Opp.Active;
                ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
                PoisonWith(ctx, target, 3);
            }
        }

        /// <summary>Molting: when played from your hand to evolve during your turn, you may put a Shedinja from your discard pile onto your Bench. X-Scissor: flip, 40 more.</summary>
        private sealed class NinjaskLOT : Mon
        {
            private static readonly Mon Scissor = new FlipBonusMon("X-Scissor", 40);

            private static List<CardInstance> Shedinja(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => c.Def.IsPokemon && c.Def.Name == "Shedinja" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                bool ready = ctx.BenchSpace > 0 && Shedinja(ctx).Count > 0;
                if (!TriggerOnce(ctx, "Molting", ready, "Molting: put a Shedinja from your discard pile onto your Bench?")) return;
                var cards = Shedinja(ctx);
                var card = ctx.ChooseCards(cards, 1, 1, "Molting: choose a Shedinja")[0];
                if (!ctx.Me.Discard.Remove(card)) return;
                ctx.Engine.PutOntoBench(ctx.Player, card, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Scissor.HandleAttack(ctx, atk); }
        }

        /// <summary>Commanding Queen: if you have 4 or fewer Grass Pokémon on your Bench, this attack does nothing.</summary>
        private sealed class CommandingQueenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Commanding Queen") return false;
                int grass = ctx.Me.Bench.Count(m => ctx.Engine.IsType(m, EnergyType.Grass));
                if (grass <= 4) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Floral Heal: once during your turn, heal 20 damage from your Active Grass Pokémon.</summary>
        private sealed class ShayminLOT : Mon
        {
            protected override string Activated { get { return "Floral Heal"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return active != null && active.DamageCounters > 0 && ctx.Engine.IsType(active, EnergyType.Grass);
            }

            protected override void Ability(EffectContext ctx) { ctx.Heal(ctx.Me.Active, 20); }
        }

        /// <summary>Breeze Away-GX: return any number of your Pokémon in play, with all attached cards, to your hand.</summary>
        private sealed class BreezeAwayMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Breeze Away-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var mine = ctx.MyPokemon();
                if (mine.Count == 0) return true;
                var picks = ctx.ChoosePokemonMany(mine, 0, mine.Count, "Breeze Away-GX: choose any of your Pokémon to return to your hand with their attached cards");
                foreach (var mon in picks)
                    if (ctx.InPlay(mon)) ctx.Engine.ReturnPokemonToHand(mon);
                return true;
            }
        }
    }
}
