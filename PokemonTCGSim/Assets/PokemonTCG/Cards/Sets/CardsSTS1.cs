using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Steam Siege (STS, v93), part 1: the Grass, Fire and Water Pokémon (1-37). Steam Siege brought dual-type Pokémon (Shiftry,
    /// Volcarona, Volcanion-EX, Galvantula, Bisharp, M Steelix-EX, Azumarill, M Gardevoir-EX: "secondType" in the data).
    /// </summary>
    public static class CardsSTS1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("STS-1", new ComboMon(new CoinsMon("Slam", 2), new CardsTWM1.HealSelfMon("Mega Drain", 20)));
            r.RegisterByKey("STS-2", new ComboMon(new LeafStormMon(), new FlipBonusMon("Flog", 30)));
            r.RegisterByKey("STS-5", new ComboMon(new CardsDAA1.GustAttackMon("Fluffy Transport", false), new CardsASR1.TimesMon("Solar Step", c => c.Me.Prizes.Count)));
            r.RegisterByKey("STS-6", new Cards30C.PeerAtMon("Scout"));
            r.RegisterByKey("STS-7", new YanmegaSTS());
            r.RegisterByKey("STS-8", new CardsCRZ1.PiercingMon("Barrier Break"));
            r.RegisterByKey("STS-9", new CardsCES2.EndureMon("Bide"));
            r.RegisterByKey("STS-10", new CardsLOR1.HardenMon("Harden", 60));
            r.RegisterByKey("STS-11", new ComboMon(new WickedWindMon(), new ExtrasensoryMon("Extrasensory", 60)));
            r.RegisterByKey("STS-12", new FoongusSTS());
            r.RegisterByKey("STS-13", new ComboMon(new CrazySporeMon(), new StrangeReactionMon()));
            r.RegisterByKey("STS-15", new ComboMon(new ShimmeringScalesMon(), new CardsTWM1.SelfDiscardMon("Power Hurricane", 99)));
            r.RegisterByKey("STS-16", new ReturningFlamesMon());
            r.RegisterByKey("STS-17", new CardsSUM1.DiscardTypeFromSelfMon("Fire Blast", EnergyType.Fire, 1));
            r.RegisterByKey("STS-18", new ChimcharEmberMon());
            r.RegisterByKey("STS-20", new ComboMon(new CardsSUM1.DiscardTypeFromSelfMon("Flare Blitz", EnergyType.Fire, 99), new FlareUpMon()));
            r.RegisterByKey("STS-21", new CardsSUM1.DiscardTypeFromSelfMon("Flare Blitz", EnergyType.Fire, 99));
            r.RegisterByKey("STS-22", new TailsNothingMon("Lunge"));
            r.RegisterByKey("STS-23", new ComboMon(new FlameChargeMon(), new CardsSUM4.DiscardDefendingToolsMon("Incinerate")));
            r.RegisterByKey("STS-24", new RecoilMon("Kaiser Tackle", 50));
            r.RegisterByKey("STS-25", new PowerHeaterMon());
            r.RegisterByKey("STS-26", new VolcanionEX());
            r.RegisterByKey("STS-27", new ComboMon(new HealingWaveMon(), new CardsUPR1.LessPerSelfCounterMon("Dwindling Wave", 10)));
            r.RegisterByKey("STS-29", new ComboMon(new StickyShotMon(), new ConditionAttackMon("Water Pulse", SpecialCondition.Asleep)));
            r.RegisterByKey("STS-31", new FlipBonusMon("Razor Shell", 20));
            r.RegisterByKey("STS-32", new ComboMon(new UltimateBladeMon(), new CardsMEG.SnipeBenchMon("Pike", 30)));
            r.RegisterByKey("STS-33", new CoinsMon("Double Pincers", 2));
            r.RegisterByKey("STS-34", new ClawitzerSTS());
            r.RegisterByKey("STS-35", new LockOnMon());
            r.RegisterByKey("STS-36", new CardsTWM1.HealSelfMon("Ice Block", 30));
            r.RegisterByKey("STS-37", new ComboMon(new CardsASC.FlipDiscardDefendingEnergyMon("Crunch"), new MeltingFloeMon()));
        }

        // ================================================================== Grass

        /// <summary>Leaf Storm: heal 40 damage from each of your Grass Pokémon.</summary>
        private sealed class LeafStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Leaf Storm") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass)))
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, 40);
                return true;
            }
        }

        /// <summary>
        /// Sonic Vision: with exactly 4 cards in your hand, ignore all Energy in this Pokémon's attack costs (a passive Ability, so a
        /// Yanmega BREAK on top keeps it). Assault Boom: 70 more if the opponent's Active Pokémon has a Pokémon Tool attached.
        /// </summary>
        private sealed class YanmegaSTS : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon || src.Me.Hand.Count != 4) return;
                cost.Clear();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assault Boom") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (defending != null && defending.Tools.Count > 0 ? 70 : 0));
                return true;
            }
        }

        /// <summary>Wicked Wind: until the end of the opponent's next turn, every Stadium and Pokémon Tool card in play has no effect.</summary>
        private sealed class WickedWindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wicked Wind") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new WickedWindEffect("Stadium and Pokémon Tool cards in play have no effect until the end of the opponent's next turn (Wicked Wind)",
                    ctx.Player, ctx.Turn + 1));
                return true;
            }
        }

        internal sealed class WickedWindEffect : ContinuousEffect
        {
            public WickedWindEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool SwitchesOffStadiumAndTools(GameEngine engine) { return true; }
        }

        /// <summary>Extrasensory: 60 more if you have the same number of cards in your hand as your opponent. Used for Extrasensory, Loving Sympathy (the attack name and the numbers are passed in).</summary>
        internal sealed class ExtrasensoryMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public ExtrasensoryMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Me.Hand.Count == ctx.Opp.Hand.Count ? amount : 0));
                return true;
            }
        }

        /// <summary>Play Ball: when you play this Pokémon from your hand onto your Bench, you may put up to 3 cards named Poké Ball from your discard pile into your hand.</summary>
        private sealed class FoongusSTS : Mon
        {
            internal const string PlayBall = "Play Ball";

            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                var balls = ctx.Me.Discard.Where(c => c.Def.Name == "Poké Ball").ToList();
                if (!TriggerOnce(ctx, PlayBall, balls.Count > 0, "Play Ball: put up to 3 Poké Ball cards from your discard pile into your hand?")) return;
                ctx.DiscardToHand(balls.Take(3).ToList());
            }
        }

        /// <summary>Crazy Spore: the opponent's Active Pokémon is now Confused and Poisoned.</summary>
        private sealed class CrazySporeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crazy Spore") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Confused);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        /// <summary>Strange Reaction: if the opponent's Active Pokémon is Confused, it is now Paralyzed.</summary>
        private sealed class StrangeReactionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Strange Reaction") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && defending.Has(SpecialCondition.Confused))
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Shimmering Scales: flip a coin; heads, the opponent's Active Pokémon is now Confused; tails, Paralyzed.</summary>
        private sealed class ShimmeringScalesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shimmering Scales") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending))
                    ctx.ApplyCondition(defending, heads ? SpecialCondition.Confused : SpecialCondition.Paralyzed);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>Returning Flames: put 2 Fire Energy cards from your discard pile into your hand (1 if that's all there is).</summary>
        private sealed class ReturningFlamesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Returning Flames") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count == 0) return true;
                int n = Math.Min(2, fire.Count);
                ctx.DiscardToHand(n == fire.Count ? fire : ctx.ChooseCards(fire, n, n, "Returning Flames: put 2 Fire Energy cards into your hand"));
                return true;
            }
        }

        /// <summary>Ember (Chimchar): flip a coin; on tails, discard a Fire Energy from this Pokémon.</summary>
        private sealed class ChimcharEmberMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ember") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!heads && ctx.InPlay(me))
                    ctx.DiscardEnergyFrom(me, 1, "Ember: discard a Fire Energy from " + me.Name, en => CardsHIF1.Provides(ctx, me, en, EnergyType.Fire));
                return true;
            }
        }

        /// <summary>Flare Up: with fewer than 10 Fire Energy cards in your discard pile it does nothing; otherwise 200, then shuffle 10 of them into your deck.</summary>
        private sealed class FlareUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Up") return false;
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                if (fire.Count < 10) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                var back = fire.Count == 10 ? fire : ctx.ChooseCards(fire, 10, 10, "Flare Up: shuffle 10 Fire Energy cards from your discard pile into your deck");
                ctx.DiscardToDeck(back);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Flame Charge: search your deck for a Fire Energy card and attach it to this Pokémon; shuffle.</summary>
        private sealed class FlameChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flame Charge") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fire, 1, "Flame Charge: choose a Fire Energy card to attach to " + me.Name);
                foreach (var card in found)
                {
                    if (ctx.InPlay(me)) ctx.Engine.AttachEnergyCard(card, me);
                    else ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Power Heater: choose 2 of your Benched Pokémon; attach a Fire Energy card from your discard pile to each.</summary>
        private sealed class PowerHeaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Power Heater") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                int n = Math.Min(2, bench.Count);
                var chosen = n == bench.Count ? bench : ctx.ChoosePokemonMany(bench, n, n, "Power Heater: choose 2 of your Benched Pokémon");
                foreach (var mon in chosen)
                {
                    var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                    if (fire.Count == 0 || !ctx.InPlay(mon)) continue;
                    if (ctx.Me.Discard.Remove(fire[0])) ctx.Engine.AttachEnergyCard(fire[0], mon);
                }
                return true;
            }
        }

        /// <summary>
        /// Steam Up: once during your turn (before your attack), you may discard a Fire Energy card from your hand; if you do, this
        /// turn your Basic Fire Pokémon's attacks do 30 more damage to the opponent's Active Pokémon (each use adds 30). Volcanic Heat:
        /// it can't attack during your next turn.
        /// </summary>
        private sealed class VolcanionEX : Mon
        {
            private static readonly Mon Heat = new LockAfterAttackMon("Volcanic Heat", false);

            protected override string Activated { get { return "Steam Up"; } }

            private static List<CardInstance> FireInHand(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsEnergy && c.Def.IsBasicEnergy && c.Def.EnergyProvides == EnergyType.Fire).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx) { return FireInHand(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fire = FireInHand(ctx);
                if (fire.Count == 0) return;
                var pick = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Steam Up: discard a Fire Energy card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.AddEffect(new ConditionalDamageBonusEffect("Basic Fire Pokémon do 30 more damage this turn (Steam Up)", ctx.Player, ctx.Turn, 30,
                    (engine, attacker, target) => attacker.Def.IsBasicPokemon && engine.IsType(attacker, EnergyType.Fire)));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Heat.HandleAttack(ctx, atk); }
        }

        // ================================================================== Water

        /// <summary>Healing Wave: discard any number of cards from your hand; heal 10 from this Pokémon for each.</summary>
        private sealed class HealingWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Healing Wave") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count == 0 || me.DamageCounters == 0) return true;
                var picks = ctx.ChooseCards(hand, 0, Math.Min(hand.Count, me.DamageCounters), "Healing Wave: discard cards from your hand (heal 10 for each)");
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (picks.Count > 0 && ctx.InPlay(me)) ctx.Heal(me, 10 * picks.Count);
                return true;
            }
        }

        /// <summary>Sticky Shot: during the opponent's next turn, the Defending Pokémon's attacks cost [C] more and its Retreat Cost is [C] more.</summary>
        private sealed class StickyShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sticky Shot") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost Colorless more during its next turn (Sticky Shot)", ctx.Player, defending.Uid, ctx.Turn + 1, 1));
                ctx.AddEffect(new CardsMEW1.RetreatUpEffect(defending.Name + "'s Retreat Cost is Colorless more during its next turn (Sticky Shot)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Ultimate Blade: if this attack's damage leaves the opponent's Active Pokémon with 60 HP or less, it is Knocked Out.</summary>
        private sealed class UltimateBladeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ultimate Blade") return false;
                var defending = ctx.Defending;
                int dealt = ctx.DamageDefending(atk.Damage);
                if (dealt > 0 && TefPokemon.StillDefending(ctx, defending) && !ctx.Engine.IsKnockedOut(defending) && ctx.RemainingHp(defending) <= 60)
                    ctx.Engine.KnockOut(defending, "Ultimate Blade");
                return true;
            }
        }

        /// <summary>Mega Boost: once during your turn (before your attack), attach a Special Energy card from your hand to 1 of your Mega Evolution Pokémon (XY-era Megas only).</summary>
        private sealed class ClawitzerSTS : Mon
        {
            protected override string Activated { get { return "Mega Boost"; } }

            private static List<CardInstance> Special(EffectContext ctx) { return ctx.Me.Hand.Where(c => c.Def.IsSpecialEnergy).ToList(); }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return Special(ctx).Count > 0 && ctx.MyPokemon(m => m.Def.IsXYMegaEvolution).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AttachCardsAnyWay(Special(ctx), 1, (card, m) => m.Def.IsXYMegaEvolution, "Mega Boost: attach a Special Energy card to 1 of your Mega Evolution Pokémon", false);
            }
        }

        /// <summary>
        /// Lock-On: the Defending Pokémon can't retreat during the opponent's next turn, and during your next turn it takes 120 more
        /// damage from attacks (after Weakness and Resistance). Both are effects on that Pokémon: they end if it leaves the Active Spot.
        /// </summary>
        private sealed class LockOnMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lock-On") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                ctx.DefendingCantRetreat(defending);
                var more = new DamageTakenEffect(defending.Name + " takes 120 more damage from attacks during its opponent's next turn (Lock-On)", ctx.Player, defending.Uid, ctx.Turn + 2, 120);
                more.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(more);
                return true;
            }
        }

        /// <summary>Melting Floe: discard the top 3 cards of your deck; for each Water Energy card among them, discard the top 3 of the opponent's deck.</summary>
        private sealed class MeltingFloeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Melting Floe") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var top = ctx.Me.Deck.Count == 0 ? new List<CardInstance>() : ctx.Engine.DiscardTopOfDeck(ctx.Player, 3);
                int water = top.Count(card => card.Def.IsBasicEnergy && card.Def.EnergyProvides == EnergyType.Water);
                int n = Math.Min(3 * water, ctx.Opp.Deck.Count);
                if (n > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, n);
                return true;
            }
        }
    }
}
