using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unified Minds (UNM) part 1: Grass, Fire, Water and Lightning (UNM 1-62). No regulation mark: Expanded. Carracosta's
    /// Ancient Custom turns off the opponent's Tools (GameEngine.ToolWorks); Amoonguss's Bursting Spores reacts to a Pokémon with
    /// Spore played from the hand onto the Bench (OnOwnPokemonPlayedFromHandToBench) or to evolve.
    /// </summary>
    public static class CardsUNM1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNM-1", new RowletExeggutorGX());
            r.RegisterByKey("UNM-3", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("UNM-4", new ComboMon(new TimeSpiralMon(), new ConditionAttackMon("Mind Bend", SpecialCondition.Confused)));
            r.RegisterByKey("UNM-5", new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned));
            r.RegisterByKey("UNM-6", new NamedBenchSearchMon("Multiply", "Sewaddle", 2));
            r.RegisterByKey("UNM-7", new CardsFST1.TakeLessMon(10, new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("UNM-8", new CardsFST1.TakeLessMon(30, null));
            r.RegisterByKey("UNM-9", new LeavannyUNM());
            r.RegisterByKey("UNM-11", new CardsFST1.TakeLessMon(30, new FuryCutterMon()));
            r.RegisterByKey("UNM-12", new RecoilMon("Take Down", 10));
            r.RegisterByKey("UNM-13", new ConditionAttackMon("Spore", SpecialCondition.Asleep));
            r.RegisterByKey("UNM-14", new AmoongussUNM());
            r.RegisterByKey("UNM-15", new CardsPOR.SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("UNM-16", new ComboMon(new CardsCEC3.SpreadAmountMon("Petal Blizzard", 10), new Cards30C.TypeEnergyBonusMon("Sol Slice", EnergyType.Fire, 50)));
            r.RegisterByKey("UNM-18", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("UNM-19", new TsareenaUNM());
            r.RegisterByKey("UNM-20", new BenchSearchAttackMon("Sea Creeper Net", 1));
            r.RegisterByKey("UNM-21", new CardsMEG.SandAttackMon("Smoke Bomb"));
            r.RegisterByKey("UNM-22", new ComboMon(new CardsMEG.SandAttackMon("Smoke Bomb"), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("UNM-23", new CardsTWM1.SelfDiscardMon("Flamethrower", 1));
            r.RegisterByKey("UNM-24", new CardsTWM1.SelfDiscardMon("Strong Flare", 2));
            r.RegisterByKey("UNM-25", new HeatranGX());
            r.RegisterByKey("UNM-26", new VictorySignMon());
            r.RegisterByKey("UNM-27", new SearchAttackMon("Find a Friend", IsFirePokemonCard, 1));
            r.RegisterByKey("UNM-29", new CardsTWM1.SelfDiscardMon("Fireworks", 1));
            r.RegisterByKey("UNM-30", new SpiritBurnerMon());
            r.RegisterByKey("UNM-31", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("UNM-32", new ComboMon(new ConditionAttackMon("Heat Wave", SpecialCondition.Burned), new FlareRaidMon()));
            r.RegisterByKey("UNM-35", new SlowpokePsyduckGX());
            r.RegisterByKey("UNM-36", new LaprasUNM());
            r.RegisterByKey("UNM-38", new ComboMon(new SpitefulSighMon(), new ConditionAttackMon("Icy Wind", SpecialCondition.Asleep)));
            r.RegisterByKey("UNM-40", new NeonTrickeryMon());
            r.RegisterByKey("UNM-41", new ConditionAttackMon("Powder Snow", SpecialCondition.Asleep));
            r.RegisterByKey("UNM-42", new ComboMon(new QuickFreezeMon(), new RecoilMon("Wild Tackle", 20)));
            r.RegisterByKey("UNM-43", new SwarmingBitesMon());
            r.RegisterByKey("UNM-45", new CarracostaUNM());
            r.RegisterByKey("UNM-46", new CardsDAA1.ItemLockAttackMon("Frozen Lock"));
            r.RegisterByKey("UNM-47", new KeldeoGX());
            r.RegisterByKey("UNM-49", new ComboMon(new CardsDAA1.GustAttackMon("Tangle Drag", false), new StickyWebMon()));
            r.RegisterByKey("UNM-50", new CardsMEG.SandAttackMon("Sand Attack"));
            r.RegisterByKey("UNM-51", new GolisopodUNM());
            r.RegisterByKey("UNM-52", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("UNM-53", new TapuFiniUNM());
            r.RegisterByKey("UNM-54", new RaichuAlolanRaichuGX());
            r.RegisterByKey("UNM-55", new CardsSP1.FlipUntilTailsDrawMon("Meal Time"));
            r.RegisterByKey("UNM-57", new ElectroRainMon());
            r.RegisterByKey("UNM-58", new ConditionAttackMon("Supersonic", SpecialCondition.Confused));
            r.RegisterByKey("UNM-59", new CoinsMon("Tri Attack", 3));
            r.RegisterByKey("UNM-60", new MagnezoneUNM());
            r.RegisterByKey("UNM-61", new ParalyzeFlipMon("Jolt"));
            r.RegisterByKey("UNM-62", new LiveWireMon());
        }

        // ================================================================== Shared helpers

        internal static bool IsFirePokemonCard(CardDef d) { return d.IsPokemon && d.PokemonType == EnergyType.Fire; }

        internal static bool IsUltraBeastInPlay(EffectContext ctx, int player)
        {
            foreach (var mon in ctx.State.Players[player].InPlay()) if (mon.Def.IsUltraBeast) return true;
            return false;
        }

        /// <summary>"If this Pokémon moved from your Bench to the Active Spot this turn" (First Impression, Tandem Shock).</summary>
        internal static bool CameInThisTurn(EffectContext ctx) { return ctx.Pokemon != null && ctx.Pokemon.MovedToActiveOnTurn == ctx.Turn; }

        /// <summary>"Search your deck for up to N [name] and put them onto your Bench" (Multiply).</summary>
        internal sealed class NamedBenchSearchMon : Mon
        {
            private readonly string attack;
            private readonly string name;
            private readonly int count;

            public NamedBenchSearchMon(string attack, string name, int count)
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
                if (n == 0 || ctx.Me.Deck.Count == 0) return true;
                string wanted = name;
                ctx.SearchToBench(d => d.IsBasicPokemon && d.Name == wanted, n, attack + ": put up to " + n + " " + name + " onto your Bench");
                return true;
            }
        }

        /// <summary>Time Spiral: devolve 1 of the opponent's evolved Pokémon; the top Evolution card is shuffled into their deck.</summary>
        internal sealed class TimeSpiralMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Time Spiral") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Stack.Count > 1 && m.Def.IsEvolution && !m.Def.IsMultiCard);
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Time Spiral: devolve which of your opponent's evolved Pokémon?");
                if (target != null) ctx.Engine.Devolve(target, 1, true);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Super Growth: evolve 1 of your Grass Pokémon with a card from your deck; if it's now a Stage 1, you may evolve it again
        /// into a Stage 2 from your deck. Calming Hurricane: heal 30 from this Pokémon. Tropical Hour-GX: with 3 extra Energy, the
        /// opponent shuffles all Energy on their Pokémon into their deck.
        /// </summary>
        private sealed class RowletExeggutorGX : Mon
        {
            private static readonly Mon Calming = new CardsTWM1.HealSelfMon("Calming Hurricane", 30);

            private static List<PokemonInPlay> GrowTargets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Grass) && !CardsEVS1.EvolveBlocked(ctx, m)
                                          && ctx.Me.Deck.Any(c => CardsEVS1.EvolvesOnto(ctx, m, c.Def)));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Super Growth")
                {
                    var targets = GrowTargets(ctx);
                    if (targets.Count == 0)
                    {
                        ctx.Log("Super Growth: none of your Grass Pokémon can evolve from your deck.");
                        if (ctx.Me.Deck.Count > 0) ctx.ShuffleMyDeck();
                        return true;
                    }
                    var mon = ctx.ChoosePokemon(targets, "Super Growth: evolve which of your Grass Pokémon?");
                    if (mon == null) return true;
                    if (CardsEVS1.EvolveFromDeck(ctx, mon, "Super Growth") && ctx.InPlay(mon) && mon.Def.Stage == Stage.Stage1
                        && ctx.Me.Deck.Any(c => CardsEVS1.EvolvesOnto(ctx, mon, c.Def))
                        && ctx.AskYesNo("Super Growth: also evolve " + mon.Name + " into a Stage 2 from your deck?"))
                        CardsEVS1.EvolveFromDeck(ctx, mon, "Super Growth");
                    return true;
                }
                if (atk.Name == "Tropical Hour-GX")
                {
                    bool extra = ExtraEnergy(ctx, atk, 3);
                    ctx.DamageDefending(atk.Damage);
                    if (!extra) return true;
                    foreach (var mon in ctx.OpponentPokemon())
                        if (mon.Energy.Count > 0 && ctx.CanAffect(mon)) ctx.Engine.ShuffleEnergyIntoDeck(mon, mon.Energy.ToList());
                    return true;
                }
                return Calming.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Blanket Weaver: your Grass Pokémon take 40 less damage from the opponent's attacks; only 1 Blanket Weaver applies.</summary>
        private sealed class LeavannyUNM : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target == null || info.Target.Owner != src.Player || !info.AgainstOpponent || damage <= 0) return damage;
                if (!src.Engine.IsType(info.Target, EnergyType.Grass) || !info.ApplyOnce("Blanket Weaver")) return damage;
                return damage - 40;
            }
        }

        /// <summary>Fury Cutter: flip 3 coins; 1 heads 40 more, 2 heads 80 more, 3 heads 150 more.</summary>
        private sealed class FuryCutterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fury Cutter") return false;
                int heads = ctx.FlipCoins(3);
                ctx.DamageDefending(atk.Damage + (heads >= 3 ? 150 : heads * 40));
                return true;
            }
        }

        /// <summary>
        /// Bursting Spores: whenever you play a Pokémon that has the Spore attack from your hand during your turn (onto the Bench or
        /// to evolve), you may make the opponent's Active Pokémon Asleep and Poisoned. Venoshock: 70 more if it's Poisoned.
        /// </summary>
        private sealed class AmoongussUNM : Mon
        {
            private static readonly Mon Venoshock = new CardsBLK.VenoshockMon(70);

            private static void Spores(EffectContext src, PokemonInPlay played)
            {
                if (!src.IsMyTurn || played == null || src.Pokemon == null || !played.Def.Attacks.Any(a => a.Name == "Spore")) return;
                var target = src.Opp.Active;
                if (target == null || !src.AskYesNo("Bursting Spores (" + src.Pokemon.Name + "): make your opponent's Active Pokémon Asleep and Poisoned?")) return;
                Announce(src, "Bursting Spores");
                src.ApplyCondition(target, SpecialCondition.Asleep);
                src.ApplyCondition(target, SpecialCondition.Poisoned);
            }

            public override void OnOwnPokemonPlayedFromHandToBench(EffectContext src, PokemonInPlay benched) { Spores(src, benched); }

            public override void OnOwnPokemonEvolvedFromHand(EffectContext src, PokemonInPlay evolved, CardDef before) { Spores(src, evolved); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Venoshock.HandleAttack(ctx, atk); }
        }

        /// <summary>Queenly Reward: once during your turn, attach a basic Grass Energy card from your discard pile to your Active Pokémon.</summary>
        private sealed class TsareenaUNM : Mon
        {
            private static List<CardInstance> Grass(EffectContext ctx) { return ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Grass); }

            protected override string Activated { get { return "Queenly Reward"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null && Grass(ctx).Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var grass = Grass(ctx);
                var active = ctx.Me.Active;
                if (grass.Count == 0 || active == null) return;
                var card = ctx.ChooseCards(grass, 1, 1, "Queenly Reward: attach which Grass Energy card?")[0];
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, active);
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Burning Road: once during your turn, when this Pokémon moves from your Bench to the Active Spot, you may move any number of
        /// Fire Energy from your other Pokémon to it. Hot Burn-GX: 50 damage for each Fire Energy attached to it.
        /// </summary>
        private sealed class HeatranGX : Mon
        {
            private static readonly Mon HotBurn = new CardsASR1.TimesMon("Hot Burn-GX", c => c.Engine.EnergyCountOfType(c.Pokemon, EnergyType.Fire));

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                return ctx.MyPokemon(m => m != me && m.Energy.Any(x => CardsHIF1.Provides(ctx, m, x, EnergyType.Fire)));
            }

            public override void OnMovedToActiveFromBench(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !ctx.IsMyTurn || ctx.AbilityUsedThisTurn("Burning Road") || Givers(ctx).Count == 0) return;
                if (!ctx.AskYesNo("Burning Road: move Fire Energy from your other Pokémon to " + me.Name + "?")) return;
                ctx.MarkAbilityUsed("Burning Road");
                Announce(ctx, "Burning Road");
                RepeatableAbility.Repeat(ctx, () => Givers(ctx).Count > 0 && ctx.InPlay(me), offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Burning Road: move a Fire Energy from which Pokémon to " + me.Name + "?"
                        + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var fire = from.Energy.Where(x => CardsHIF1.Provides(ctx, from, x, EnergyType.Fire)).ToList();
                    var energy = fire.Select(x => x.Def.Name).Distinct().Count() == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Burning Road: move which Energy?")[0];
                    return ctx.Engine.MoveEnergy(from, me, energy);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return HotBurn.HandleAttack(ctx, atk); }
        }

        /// <summary>Victory Sign: search your deck for up to 2 basic Energy cards of different types and attach them to your Pokémon in any way you like.</summary>
        private sealed class VictorySignMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Victory Sign") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 1, "Victory Sign: search your deck for a basic Energy card (up to 2 of different types)");
                if (found.Count > 0 && ctx.Me.Deck.Count > 0)
                {
                    var first = found[0].Def.EnergyProvides;
                    found.AddRange(ctx.SearchMyDeck(d => d.IsBasicEnergy && d.EnergyProvides != first, 1, "Victory Sign: search your deck for a basic Energy card of another type"));
                }
                foreach (var card in found)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Victory Sign: attach " + card.Def.Name + " to which of your Pokémon?");
                    if (target == null || !ctx.Engine.AttachEnergyCard(card, target)) ctx.Me.Deck.Add(card);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>
        /// Spirit Burner: discard the top 5 cards of your deck; 60 more damage for each Pokémon discarded. Then you may put any of the
        /// Fire Pokémon discarded this way onto your Bench: Stage 1 and Stage 2 Pokémon too, but not Prism Star cards (Compendium,
        /// 2019-08-01).
        /// </summary>
        private sealed class SpiritBurnerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spirit Burner") return false;
                var milled = ctx.Engine.DiscardTopOfDeck(ctx.Player, 5);
                ctx.DamageDefending(atk.Damage + 60 * milled.Count(c => c.Def.IsPokemon));
                var fire = milled.Where(c => c.Def.IsPokemon && (c.Def.PokemonType == EnergyType.Fire || c.Def.SecondType == EnergyType.Fire)
                    && !c.Def.IsPrismStar && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def) && ctx.Me.Discard.Contains(c)).ToList();
                int room = ctx.BenchSpace;
                if (fire.Count == 0 || room == 0) return true;
                var picks = ctx.ChooseCards(fire, 0, Math.Min(room, fire.Count), "Spirit Burner: put any of these Fire Pokémon onto your Bench");
                foreach (var card in picks)
                {
                    if (ctx.BenchSpace == 0 || !ctx.Me.Discard.Remove(card)) continue;
                    ctx.Engine.PutOntoBench(ctx.Player, card, false);
                }
                return true;
            }
        }

        /// <summary>Flare Raid: discard an Energy from this Pokémon; also 50 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class FlareRaidMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Raid") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.Pokemon.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(ctx.Pokemon, 1, "Flare Raid: discard an Energy from " + ctx.Pokemon.Name);
                ctx.DamageOneBenched(50);
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Ditch and Splash: discard any number of Supporter cards from your hand, 40 damage for each. Thrilling Times-GX: flip a
        /// coin for 100 more; with 6 extra Water Energy, flip 10 coins instead, 100 more for each heads.
        /// </summary>
        private sealed class SlowpokePsyduckGX : Mon
        {
            private static readonly Mon Ditch = new CardsFST1.DiscardFromHandTimesMon("Ditch and Splash", d => d.IsSupporter, 99, "Supporter cards");

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thrilling Times-GX") return Ditch.HandleAttack(ctx, atk);
                var w = EnergyType.Water;
                bool big = CardsCEC1.ExtraTyped(ctx, atk, w, w, w, w, w, w);
                int bonus = big ? 100 * ctx.FlipCoins(10) : (ctx.Flip() ? 100 : 0);
                ctx.DamageDefending(atk.Damage + bonus);
                return true;
            }
        }

        /// <summary>Mermaid's Call: once during your turn, put a Misty's Favor from your discard pile into your hand.</summary>
        private sealed class LaprasUNM : Mon
        {
            private static List<CardInstance> Favors(EffectContext ctx) { return ctx.Me.Discard.Where(x => x.Def.Name == "Misty's Favor").ToList(); }

            protected override string Activated { get { return "Mermaid's Call"; } }

            protected override bool AbilityReady(EffectContext ctx) { return Favors(ctx).Count > 0 && !ctx.Engine.DiscardToHandBlocked(ctx.Player); }

            protected override void Ability(EffectContext ctx)
            {
                var favors = Favors(ctx);
                if (favors.Count > 0) ctx.DiscardToHand(new[] { favors[0] });
            }
        }

        /// <summary>Spiteful Sigh: put up to 7 damage counters on this Pokémon; 20 damage for each one placed.</summary>
        private sealed class SpitefulSighMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiteful Sigh") return false;
                int n = ctx.AskNumber(0, 7, "Spiteful Sigh: put how many damage counters (up to 7) on " + ctx.Pokemon.Name + "?");
                if (n <= 0) return true;
                int placed = ctx.Engine.PlaceDamageCounters(ctx.Pokemon, n, "Spiteful Sigh");
                if (placed > 0) ctx.DamageDefending(atk.Damage * placed);
                return true;
            }
        }

        /// <summary>Neon Trickery: you may move an Energy from the opponent's Active Pokémon to 1 of their Benched Pokémon.</summary>
        private sealed class NeonTrickeryMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Neon Trickery") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (defending == null || !ctx.InPlay(defending) || defending.Energy.Count == 0 || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                if (!ctx.AskYesNo("Neon Trickery: move an Energy from " + defending.Name + " to 1 of your opponent's Benched Pokémon?")) return true;
                var energy = defending.Energy.Count == 1 ? defending.Energy[0] : ctx.ChooseCards(defending.Energy.ToList(), 1, 1, "Neon Trickery: move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.OpponentBench(), "Neon Trickery: move " + energy.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (to != null) ctx.Engine.MoveEnergy(defending, to, energy);
                return true;
            }
        }

        /// <summary>Quick Freeze: if the opponent's Active Pokémon has any Water Energy attached, it's now Paralyzed.</summary>
        private sealed class QuickFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Quick Freeze") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.Engine.HasEnergyOfType(defending, EnergyType.Water))
                    ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Swarming Bites: 20 damage for each Basculin you have in play to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class SwarmingBitesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swarming Bites") return false;
                int n = ctx.MyPokemon(m => m.Name == "Basculin").Count;
                if (n > 0) ctx.DamageOneBenched(20 * n);
                return true;
            }
        }

        /// <summary>Ancient Custom: Pokémon Tools attached to the opponent's Pokémon have no effect. Aqua Impact: 20 more for each [C] in the Defending Pokémon's Retreat Cost.</summary>
        private sealed class CarracostaUNM : Mon
        {
            private static readonly Mon AquaImpact = new CardsOBF1.RetreatCostDamageMon("Aqua Impact", 20, false);

            public override bool DisablesOpponentsTools { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return AquaImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Pure Heart: prevent all effects of attacks, including damage, done to this Pokémon by the opponent's Pokémon-GX and
        /// Pokémon-EX. Sonic Edge: not affected by effects on the Defending Pokémon. Resolute Blade-GX: 50 for each Benched Pokémon
        /// of the opponent.
        /// </summary>
        private sealed class KeldeoGX : Mon
        {
            private static readonly Mon Attacks = new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Sonic Edge"),
                new CardsASR1.TimesMon("Resolute Blade-GX", c => c.Opp.Bench.Count));

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                if (src.Pokemon == null || target != src.Pokemon || kind != EffectSourceKind.Attack) return false;
                var attacker = src.Engine.CurrentSourcePokemon;
                return attacker != null && attacker.Owner != src.Player && attacker.Def.IsGXorEX;
            }

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return src.Pokemon != null && info.Target == src.Pokemon && info.AgainstOpponent && info.Attacker.Def.IsGXorEX ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Sticky Web: during the opponent's next turn the Defending Pokémon's attacks cost [C] more and its Retreat Cost is [C] more.</summary>
        private sealed class StickyWebMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sticky Web") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.AddEffect(new CardsPAR1.AttackCostUpEffect(defending.Name + "'s attacks cost [C] more (Sticky Web)", ctx.Player, defending.Uid, ctx.Turn + 1, 1));
                ctx.AddEffect(new CardsMEW1.RetreatUpEffect(defending.Name + "'s Retreat Cost is [C] more (Sticky Web)", ctx.Player, defending.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Emergency Exit: with 2 or fewer Energy attached, no Retreat Cost. First Impression: 60 more if it moved from the Bench to the Active Spot this turn.</summary>
        private sealed class GolisopodUNM : Mon
        {
            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Engine.EnergyCount(mon) <= 2 ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "First Impression") return false;
                ctx.DamageDefending(atk.Damage + (CameInThisTurn(ctx) ? 60 : 0));
                return true;
            }
        }

        /// <summary>Nature Wave: costs only [C] while the opponent has an Ultra Beast in play.</summary>
        private sealed class TapuFiniUNM : Mon
        {
            public override void ModifyOwnAttackCost(EffectContext ctx, AttackDef attack, List<EnergyType> cost)
            {
                if (attack.Name != "Nature Wave" || ctx.Pokemon == null || !IsUltraBeastInPlay(ctx, 1 - ctx.Pokemon.Owner)) return;
                cost.Clear();
                cost.Add(EnergyType.Colorless);
            }
        }

        // ================================================================== Lightning

        /// <summary>
        /// Tandem Shock: if this Pokémon moved from the Bench to the Active Spot this turn, 80 more damage and the opponent's Active
        /// Pokémon is Paralyzed. Lightning Ride-GX: with 2 extra Lightning Energy, 100 more; then switch this Pokémon with 1 of your
        /// Benched Pokémon.
        /// </summary>
        private sealed class RaichuAlolanRaichuGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Tandem Shock")
                {
                    var defending = ctx.Defending;
                    bool moved = CameInThisTurn(ctx);
                    ctx.DamageDefending(atk.Damage + (moved ? 80 : 0));
                    if (moved && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name == "Lightning Ride-GX")
                {
                    var l = EnergyType.Lightning;
                    bool extra = CardsCEC1.ExtraTyped(ctx, atk, l, l);
                    ctx.DamageDefending(atk.Damage + (extra ? 100 : 0));
                    if (ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Electro Rain: discard any amount of Lightning Energy from this Pokémon; for each one, 30 damage to 1 of the opponent's
        /// Pokémon (the same one can be picked again), not affected by Weakness or Resistance. All the targets are chosen first and
        /// each Pokémon takes its total as one hit, so Telescopic Sight adds 30 once to a Pokémon chosen twice (Compendium, 2020-12-10).
        /// </summary>
        private sealed class ElectroRainMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electro Rain") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(x => CardsHIF1.Provides(ctx, me, x, EnergyType.Lightning)).ToList();
                if (lightning.Count == 0) return true;
                var picks = ctx.ChooseCards(lightning, 0, lightning.Count, "Electro Rain: discard any amount of Lightning Energy from " + me.Name + " (30 damage for each)");
                int n = 0;
                foreach (var card in picks) if (ctx.Engine.DiscardEnergy(me, card)) n++;
                var counts = new Dictionary<int, int>();
                var order = new List<PokemonInPlay>();
                for (int i = 0; i < n; i++)
                {
                    var targets = ctx.OpponentPokemon();
                    if (targets.Count == 0) break;
                    var target = ctx.ChoosePokemon(targets, "Electro Rain: 30 damage to which of your opponent's Pokémon? (" + (i + 1) + " of " + n + ")");
                    if (target == null) continue;
                    int times;
                    counts.TryGetValue(target.Uid, out times);
                    if (times == 0) order.Add(target);
                    counts[target.Uid] = times + 1;
                }
                foreach (var target in order)
                    if (ctx.InPlay(target)) ctx.DamageWith(target, 30 * counts[target.Uid], false, false, false);
                return true;
            }
        }

        /// <summary>Double Type: this Pokémon is Lightning and Metal. Magnetic Bolt: put a Trainer card from your discard pile into your hand.</summary>
        private sealed class MagnezoneUNM : Mon
        {
            public override void ModifyTypes(EffectContext src, PokemonInPlay mon, List<EnergyType> types)
            {
                if (src.Pokemon == null || mon != src.Pokemon) return;
                if (!types.Contains(EnergyType.Lightning)) types.Add(EnergyType.Lightning);
                if (!types.Contains(EnergyType.Metal)) types.Add(EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Magnetic Bolt") return false;
                ctx.DamageDefending(atk.Damage);
                var trainers = ctx.Me.Discard.Where(x => x.Def.IsTrainer).ToList();
                if (trainers.Count == 0 || ctx.Engine.DiscardToHandBlocked(ctx.Player)) return true;
                ctx.DiscardToHand(ctx.ChooseCards(trainers, 1, 1, "Magnetic Bolt: put a Trainer card from your discard pile into your hand"));
                return true;
            }
        }

        /// <summary>Live Wire: 50 damage to 1 of the opponent's Pokémon; Weakness and Resistance apply to a Benched one too.</summary>
        private sealed class LiveWireMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Live Wire") return false;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Live Wire: 50 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageWith(target, 50, true, true, false);
                return true;
            }
        }
    }
}
