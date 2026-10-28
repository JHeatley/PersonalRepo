using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Forbidden Light (FLI, v84), part 1: Grass, Fire and Water (FLI 1-33). No regulation mark: Expanded. Pheromosa, Froakie,
    /// Frogadier and Greninja-GX were already in the pool (Hidden Fates Shiny Vault) and keep their scripts. "A [T] Energy card"
    /// outside play is basic Energy only (your ruling, v65). Printed damage plus "does N damage to 1 of your opponent's Benched
    /// Pokémon" hits the Active Pokémon too (your ruling, v83).
    /// </summary>
    public static class CardsFLI1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("FLI-1", new CardsUNM1.NamedBenchSearchMon("Multiply", "Exeggcute", 1));
            r.RegisterByKey("FLI-2", new CardsASR1.PlusMon("Tropical Shake", 20, CardsTEU3.BasicEnergyTypesInDiscard));
            r.RegisterByKey("FLI-3", new CardsMEW1.DefendingTypeBonusMon("Ice Shard", EnergyType.Fighting, 40));
            r.RegisterByKey("FLI-4", new AbomasnowFLI());
            r.RegisterByKey("FLI-5", new AbnormalOutbreakMon());
            r.RegisterByKey("FLI-6", new UltraEvolutionMon());
            r.RegisterByKey("FLI-7", new ParalyzeFlipMon("String Shot"));
            r.RegisterByKey("FLI-8", new CardsCES1.ConditionsMon("Vivid Powder", SpecialCondition.Asleep, SpecialCondition.Poisoned));
            r.RegisterByKey("FLI-9", new CardsCES1.AttachFromHandToSelfMon("Growth", EnergyType.Grass, 1));
            r.RegisterByKey("FLI-10", new ComboMon(new MilkDrinkMon(), new RecoilMon("Double-Edge", 30)));
            // Fire
            r.RegisterByKey("FLI-12", new CardsTEU2.AlolanMarowakTEU());
            r.RegisterByKey("FLI-13", new ComboMon(new ShieldAttackMon("Guard Press", 30), new CardsTWM1.SelfDiscardMon("Boiling Impact", 2)));
            r.RegisterByKey("FLI-14", new CardsTWM1.SelfDiscardMon("Ember", 1));
            r.RegisterByKey("FLI-16", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("FLI-17", new DelphoxFLI());
            r.RegisterByKey("FLI-19", new PyroarFLI());
            // Water
            r.RegisterByKey("FLI-20", new PalkiaGX());
            r.RegisterByKey("FLI-26", new ClawitzerFLI());
            r.RegisterByKey("FLI-27", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("FLI-28", new ComboMon(new CardsASR1.FrostWallMon("Frost Wall", 9999), new LockAfterAttackMon("Blizzard Burn", false)));
            r.RegisterByKey("FLI-29", new BreakOpenMon());
            r.RegisterByKey("FLI-30", new FrozenGroundMon());
            r.RegisterByKey("FLI-31", new VolcanionPrismStar());
            r.RegisterByKey("FLI-32", new NoRetreatMon("Spider Web"));
            r.RegisterByKey("FLI-33", new ComboMon(new ParalyzeFlipMon("Bubble"), new CardsFST2.ConditionBonusMon("Bubble Trap", 80, UsedBubbleLastTurn)));
        }

        /// <summary>"If 1 of your Pokémon used Bubble during your last turn": the player's record, so it counts even if that Pokémon is gone (Compendium, 2018-05-03).</summary>
        internal static bool UsedBubbleLastTurn(EffectContext ctx)
        {
            return ctx.Me.LastAttackTurn == ctx.MyLastTurn && ctx.Me.LastAttackName == "Bubble";
        }

        // ================================================================== Grass

        /// <summary>Blessings of the Frost: when you evolve into this Pokémon from your hand, you may attach a Grass Energy card from your discard pile to 1 of your Pokémon. Hypno Hammer: Asleep.</summary>
        private sealed class AbomasnowFLI : Mon
        {
            private static readonly Mon Hammer = new ConditionAttackMon("Hypno Hammer", SpecialCondition.Asleep);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var grass = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Grass);
                if (!TriggerOnce(ctx, "Blessings of the Frost", grass.Count > 0, "Use Blessings of the Frost? Attach a Grass Energy card from your discard pile to 1 of your Pokémon.")) return;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Blessings of the Frost: attach the Grass Energy to which Pokémon?");
                if (target == null || !ctx.Me.Discard.Remove(grass[0])) return;
                ctx.Engine.AttachEnergyCard(grass[0], target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hammer.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Abnormal Outbreak: only if you went second, during your first turn: search your deck for a Spewpa and a Vivillon. Once per
        /// Scatterbug (Compendium, 2018-05-03).
        /// </summary>
        private sealed class AbnormalOutbreakMon : Mon
        {
            protected override string Activated { get { return "Abnormal Outbreak"; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsUNB1.SecondPlayersFirstTurn(ctx) && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Spewpa", 1, "Abnormal Outbreak: search your deck for a Spewpa");
                found.AddRange(ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Vivillon", 1, "Abnormal Outbreak: search your deck for a Vivillon"));
                ctx.Me.Hand.AddRange(found);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Ultra Evolution: flip a coin; on heads, search your deck for a Vivillon and evolve this Scatterbug into it.</summary>
        private sealed class UltraEvolutionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultra Evolution") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || ctx.Me.Deck.Count == 0) return true;
                var me = ctx.Pokemon;
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.Name == "Vivillon", 1, "Ultra Evolution: search your deck for a Vivillon", false);
                if (found.Count > 0)
                {
                    if (ctx.InPlay(me) && me.Name == "Scatterbug") ctx.Engine.EvolvePokemon(me, found[0]);
                    else ctx.Me.Deck.Add(found[0]);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Milk Drink: flip 2 coins; heal 40 damage from this Pokémon for each heads.</summary>
        private sealed class MilkDrinkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Milk Drink") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && ctx.InPlay(ctx.Pokemon)) ctx.Heal(ctx.Pokemon, 40 * heads);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Mystical Torch: once during your turn, you may make the opponent's Active Pokémon Burned. Fire Spin: discard 2 Energy.</summary>
        private sealed class DelphoxFLI : Mon
        {
            private static readonly Mon Spin = new CardsTWM1.SelfDiscardMon("Fire Spin", 2);

            protected override string Activated { get { return "Mystical Torch"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var d = ctx.Opp.Active;
                return d != null && !d.Has(SpecialCondition.Burned) && ctx.CanAffect(d);
            }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Burned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spin.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Unnerve: the effects of the opponent's Item and Supporter cards played from the hand don't affect this Pokémon (Eneporter
        /// is still playable: Compendium, 2018-05-03; Pokémon Tools aren't Items: your ruling, v90). Dominating Fangs: 60 more while Lysandre Labs is in play.
        /// </summary>
        private sealed class PyroarFLI : Mon
        {
            private static readonly Mon Fangs = new CardsFST2.ConditionBonusMon("Dominating Fangs", 60, c => c.State.Stadium != null && c.State.Stadium.Def.Name == "Lysandre Labs");

            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                return src.Pokemon != null && target == src.Pokemon && trainer != null && (trainer.IsItem || trainer.IsSupporter);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Fangs.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>
        /// Spatial Control: move any number of Energy from your Benched Pokémon to this Pokémon. Hydro Pressure: 20 more for each Water
        /// Energy on it. Zero Vanish-GX: shuffle all Energy from each of the opponent's Pokémon into their deck.
        /// </summary>
        private sealed class PalkiaGX : Mon
        {
            private static readonly Mon Pressure = new CardsMEG.EnergyBonusMon("Hydro Pressure", EnergyType.Water, 20);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (Pressure.HandleAttack(ctx, atk)) return true;
                if (atk.Name == "Spatial Control")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (!ctx.InPlay(me)) return true;
                    var holders = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var mon in ctx.MyBench())
                        foreach (var e in mon.Energy) holders[e] = mon;
                    if (holders.Count == 0) return true;
                    foreach (var e in ctx.ChooseCards(holders.Keys.ToList(), 0, holders.Count, "Spatial Control: move any number of Energy from your Benched Pokémon to " + me.Name))
                        ctx.Engine.MoveEnergy(holders[e], me, e);
                    return true;
                }
                if (atk.Name == "Zero Vanish-GX")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m)))
                        ctx.Engine.ShuffleEnergyIntoDeck(mon, mon.Energy.ToList());
                    return true;
                }
                return false;
            }
        }

        /// <summary>Standing By: during your next turn, this Pokémon's Sharpshooting does 120 damage instead of 40. Sharpshooting: 40 damage to 1 of the opponent's Pokémon (no printed damage).</summary>
        private sealed class ClawitzerFLI : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var me = ctx.Pokemon;
                if (atk.Name == "Standing By")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (!ctx.InPlay(me)) return true;
                    var boost = new AttackBoostEffect(me.Name + "'s Sharpshooting does 120 damage during its next turn (Standing By)", ctx.Player, me.Uid, ctx.Turn + 2, "Sharpshooting", 80);
                    boost.ActiveFromTurn = ctx.Turn + 2;
                    ctx.AddEffect(boost);
                    return true;
                }
                if (atk.Name == "Sharpshooting")
                {
                    int uid = me.Uid;
                    int extra = ctx.Engine.ActiveEffects().OfType<AttackBoostEffect>().Where(e => e.TargetUid == uid && e.AttackName == "Sharpshooting").Sum(e => e.Bonus);
                    ctx.DamageAnyOpponent(40 + extra);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Break Open: if your opponent has a Stadium card in play, discard it; if you do, their Active Pokémon is now Paralyzed.</summary>
        private sealed class BreakOpenMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Break Open") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.State.Stadium == null || ctx.State.StadiumOwner != ctx.Opponent) return true;
                ctx.Engine.DiscardStadium();
                if (ctx.State.Stadium == null && ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Frozen Ground: the opponent can't play Stadium cards from their hand during their next turn.</summary>
        private sealed class FrozenGroundMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Frozen Ground") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new CardsMEG.StadiumLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Stadium cards during their next turn (Frozen Ground)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>
        /// Jet Geyser: once during your turn, you may discard a Water Energy card from your hand; if you do, the opponent switches their
        /// Active Pokémon with 1 of their Benched Pokémon (usable with no opposing Bench: Compendium, 2018-05-03). Sauna Blast: 20
        /// damage to each of the opponent's Benched Pokémon too.
        /// </summary>
        private sealed class VolcanionPrismStar : Mon
        {
            private static readonly Mon Blast = new CardsMEW1.SpreadBenchMon("Sauna Blast", 20);

            protected override string Activated { get { return "Jet Geyser"; } }

            private static List<CardInstance> Water(EffectContext ctx) { return ctx.Me.Hand.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Water)).ToList(); }

            protected override bool AbilityReady(EffectContext ctx) { return Water(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var water = Water(ctx);
                if (water.Count == 0) return;
                var pick = ctx.ChooseCards(water, 1, 1, "Jet Geyser: discard which Water Energy card?");
                if (pick.Count == 0) return;
                ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
                if (ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0) ctx.OpponentSwitchesActive();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Blast.HandleAttack(ctx, atk); }
        }
    }
}
