using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Skyridge Rare Pokémon (E3 1-35) and the Crystal Pokémon (E3 145-150; v147). Unlimited only. H1-H32 are holo prints of 32 of
    /// 2-35. Engine work: Ancient Wind stops every Poké-Body (ContinuousEffect.StopsPokeBody), Dark Gaze stops the Benched Pokémon's
    /// Poké-Powers (CardScript.StopsPokePowersOf), Mirror Coat reacts to Special Conditions (OnHolderConditionApplied), Synchronicity
    /// takes any Technical Machine (AllowsAnyTechnicalMachine). Your rulings: Lonely Fang and Magnetic Wave never go below their
    /// base damage; Streaming Mantle shuffles in as many basic Energy cards as there are, up to 3; Empathic Healing heals both
    /// players' Pokémon of Celebi's type.
    /// </summary>
    public static class CardsE3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("E3-1", new AerodactylE3());
            r.RegisterByKey("E3-2", new AlakazamE3());
            r.RegisterByKey("E3-3", new ArcanineE3());
            r.RegisterByKey("E3-4", new ArticunoE3());
            r.RegisterByKey("E3-5", new BeedrillE3());
            r.RegisterByKey("E3-6", new CrobatE3());
            r.RegisterByKey("E3-7", new ComboMon(new FreezeLockMon(), new CrushingIceMon()));
            r.RegisterByKey("E3-8", new SelfHealingMon(EnergyType.Fire, new ComboMon(new ConditionAttackMon("Singe", SpecialCondition.Burned), new BurnBoosterMon())));
            r.RegisterByKey("E3-9", new ComboMon(new SpinningBlowMon(), new ScatterbombMon()));
            r.RegisterByKey("E3-10", new GengarE3());
            r.RegisterByKey("E3-11", new GyaradosE3());
            r.RegisterByKey("E3-12", new ComboMon(new FlipBonusMon("Ambush", 20), new BenchDifferenceMon("Lonely Fang", 20, false)));
            r.RegisterByKey("E3-13", new SelfHealingMon(EnergyType.Lightning, new ComboMon(new ParalyzeFlipMon("Thundershock"), new ThundersparkMon())));
            r.RegisterByKey("E3-14", new KabutopsE3());
            r.RegisterByKey("E3-15", new ComboMon(new PollenShieldMon(), new CardsN4.SwiftMon()));
            r.RegisterByKey("E3-16", new MachampE3());
            r.RegisterByKey("E3-17", new CardsBS1.DiscardCostMon("Fire Stream", EnergyType.Fire, 1, FireStream, new EruptionMon()));
            r.RegisterByKey("E3-18", new MagcargoE3());
            r.RegisterByKey("E3-19", new MagnetonLightningE3());
            r.RegisterByKey("E3-20", new ComboMon(new BounceOffMon(), new BenchDifferenceMon("Magnetic Wave", 10, true)));
            r.RegisterByKey("E3-21", new MoltresE3());
            r.RegisterByKey("E3-22", new NidoqueenE3());
            r.RegisterByKey("E3-23", new OmastarE3());
            r.RegisterByKey("E3-24", new ComboMon(new CardsSI.HeadsTailsConditionMon("Freezing Breath", SpecialCondition.Paralyzed, SpecialCondition.Asleep), new TrampleMon()));
            r.RegisterByKey("E3-25", new ComboMon(new SuddenGrowthMon(), new FlipBonusMon("Frog Hop", 20), new EnergySplashMon()));
            r.RegisterByKey("E3-26", new PoliwrathE3());
            r.RegisterByKey("E3-27", new ComboMon(new ZzzapMon(), new LightningStormMon()));
            r.RegisterByKey("E3-28", new RaikouE3());
            r.RegisterByKey("E3-29", new ComboMon(new FlipBonusMon("Stomp", 20), new TailsNothingMon("Giant Tail")));
            r.RegisterByKey("E3-30", new ComboMon(new EnergyBurstMon(), new StarBackMon()));
            r.RegisterByKey("E3-31", new SteelixE3());
            r.RegisterByKey("E3-32", new UmbreonE3());
            r.RegisterByKey("E3-33", new SelfHealingMon(EnergyType.Water, new ComboMon(new ConditionAttackMon("Hypnosplash", SpecialCondition.Asleep), new MoveDefendingEnergyMon("Aqua Trick", false))));
            r.RegisterByKey("E3-34", new WigglytuffE3());
            r.RegisterByKey("E3-35", new XatuE3());
            r.RegisterByKey("E3-145", new CardsE2.CrystalTypeMon(new[] { EnergyType.Grass, EnergyType.Water, EnergyType.Psychic },
                new ComboMon(new EmpathicHealingMon(), new CardsPAR2.FlipConditionMon("Mind Bend", SpecialCondition.Confused))));
            r.RegisterByKey("E3-146", new CardsE2.CrystalTypeMon(new[] { EnergyType.Fire, EnergyType.Lightning, EnergyType.Fighting },
                new ComboMon(new DiscardOwnEnergyMon("Fireblast", false), new CoinsMon("Dragon Tail", 2))));
            r.RegisterByKey("E3-147", new CardsE2.CrystalTypeMon(new[] { EnergyType.Grass, EnergyType.Fire, EnergyType.Psychic },
                new ComboMon(new PoisonFlameMon(), new CoinsMon("Squared Attack", 4))));
            r.RegisterByKey("E3-148", new CardsE2.CrystalTypeMon(new[] { EnergyType.Grass, EnergyType.Fire, EnergyType.Fighting }, new EarthBombMon()));
            r.RegisterByKey("E3-149", new CardsE2.CrystalTypeMon(new[] { EnergyType.Fire, EnergyType.Water, EnergyType.Lightning }, new DiscardOwnEnergyMon("Scalding Steam", true)));
            r.RegisterByKey("E3-150", new CardsE2.CrystalTypeMon(new[] { EnergyType.Water, EnergyType.Lightning, EnergyType.Fighting },
                new ComboMon(new CardsE3B.HealSelfMon("Draining Cut", 10), new CoinsMon("Triple Cutter", 3))));
        }

        // ================================================================== Shared

        internal static bool HasBasicOf(PokemonInPlay mon, EnergyType type)
        {
            return mon != null && mon.Energy.Any(e => CardsE2.IsBasicEnergyOf(e.Def, type));
        }

        /// <summary>"When you play this Pokémon from your hand to evolve your Active Pokémon": the evolved Pokémon is the Active Pokémon now.</summary>
        private static bool EvolvedActive(EffectContext ctx) { return ctx.Pokemon != null && ctx.PokemonIsActive && ctx.IsMyTurn; }

        /// <summary>Fire Stream: after discarding a Fire Energy card for it, 10 damage to each of the opponent's Benched Pokémon.</summary>
        private static void FireStream(EffectContext ctx, AttackDef atk)
        {
            ctx.DamageDefending(atk.Damage);
            foreach (var mon in ctx.OpponentBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
        }

        /// <summary>Self-healing (Poké-Body): attaching a basic Energy card of its type from your hand removes all of its Special Conditions.</summary>
        private sealed class SelfHealingMon : Mon
        {
            private readonly EnergyType type;
            private readonly Mon attacks;

            public SelfHealingMon(EnergyType type, Mon attacks)
            {
                this.type = type;
                this.attacks = attacks;
            }

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target == null || target != src.Pokemon || energy == null || !CardsE2.IsBasicEnergyOf(energy.Def, type)) return;
                if (target.Conditions == SpecialCondition.None) return;
                src.Log("Self-healing: " + target.Name + " recovers from all Special Conditions.");
                foreach (SpecialCondition c in new[] { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned })
                    if (target.Has(c)) src.Engine.RemoveCondition(target, c);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>"[Type] Immunity" (Poké-Body): basic Energy cards of that type can't be attached to it from the hand.</summary>
        private static bool ImmunityBlocks(EffectContext src, CardInstance energy, PokemonInPlay target, EnergyType type)
        {
            return target != null && target == src.Pokemon && energy != null && CardsE2.IsBasicEnergyOf(energy.Def, type);
        }

        /// <summary>Lonely Fang / Magnetic Wave: N more for each Benched Pokémon the one side has over the other (your ruling: never less than the base).</summary>
        private sealed class BenchDifferenceMon : Mon
        {
            private readonly string attack;
            private readonly int per;
            private readonly bool mineMinusTheirs;

            public BenchDifferenceMon(string attack, int per, bool mineMinusTheirs)
            {
                this.attack = attack;
                this.per = per;
                this.mineMinusTheirs = mineMinusTheirs;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int diff = mineMinusTheirs ? ctx.Me.Bench.Count - ctx.Opp.Bench.Count : ctx.Opp.Bench.Count - ctx.Me.Bench.Count;
                ctx.DamageDefending(atk.Damage + per * Math.Max(0, diff));
                return true;
            }
        }

        /// <summary>Moves 1 Energy card (basic only, or any) from the Defending Pokémon to 1 of the opponent's Benched Pokémon on heads (Aqua Trick, Magnetic Lines).</summary>
        internal sealed class MoveDefendingEnergyMon : Mon
        {
            private readonly string attack;
            private readonly bool basicOnly;

            public MoveDefendingEnergyMon(string attack, bool basicOnly)
            {
                this.attack = attack;
                this.basicOnly = basicOnly;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                var cards = defending.Energy.Where(e => !basicOnly || e.Def.IsBasicEnergy).ToList();
                if (cards.Count == 0 || !ctx.Flip() || ctx.Opp.Bench.Count == 0 || !ctx.CanAffect(defending)) return true;
                var card = ctx.ChooseCards(cards, 1, 1, attack + ": move which Energy card from " + defending.Name + "?")[0];
                var bench = ctx.OpponentBench();
                var target = ctx.ChoosePokemon(bench, attack + ": move " + card.Def.Name + " to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(defending, target, card);
                return true;
            }
        }

        /// <summary>Discard an Energy card attached to this Pokémon after the damage (Fireblast); optionally then flip for Burned (Scalding Steam).</summary>
        private sealed class DiscardOwnEnergyMon : Mon
        {
            private readonly string attack;
            private readonly bool flipBurn;

            public DiscardOwnEnergyMon(string attack, bool flipBurn)
            {
                this.attack = attack;
                this.flipBurn = flipBurn;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && me.Energy.Count > 0 && !CardsBS1.CopyingWithMetronome)
                    ctx.DiscardEnergyFrom(me, 1, attack + ": discard an Energy card attached to " + me.Name);
                if (flipBurn && ctx.Flip() && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Venom Spray (Poké-Power): when you play Beedrill from your hand to evolve your Active Pokémon, the Defending Pokémon is now Paralyzed and Poisoned. Needle Rush: 4 coins; 1/2/3/4 heads add 10/20/50/90.</summary>
        private sealed class BeedrillE3 : Mon
        {
            private static readonly int[] Bonus = { 0, 10, 20, 50, 90 };

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (!EvolvedActive(ctx) || defending == null || !ctx.CanAffect(defending)) return;
                Announce(ctx, "Venom Spray");
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Needle Rush") return false;
                ctx.DamageDefending(atk.Damage + Bonus[ctx.FlipCoins(4)]);
                return true;
            }
        }

        /// <summary>
        /// Carry Off (Poké-Power): flip; heads, look at the opponent's hand and choose a Pokémon card there; they shuffle it into their
        /// deck. Double Cross: 2 coins, 40 per heads; 2 tails, Confused and Poisoned.
        /// </summary>
        private sealed class CrobatE3 : Mon
        {
            private const string Power = "Carry Off";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return CardsE1.NoCondition(ctx.Pokemon) && ctx.Opp.Hand.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip()) CardsE3B.HandCardToDeck(ctx, d => d.IsPokemon, Power, "a Pokémon card");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Cross") return false;
                var defending = ctx.Defending;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads == 0 && defending != null && ctx.InPlay(defending) && ctx.Opp.Active == defending)
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                    ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                }
                return true;
            }
        }

        /// <summary>Primal Aura (Poké-Body): while Kabutops is your Active Pokémon, no player can play cards from their hand to evolve Benched Pokémon. Dual Cut: 2 coins.</summary>
        private sealed class KabutopsE3 : Mon
        {
            private static readonly Mon DualCut = new CoinsMon("Dual Cut", 2);

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && target != null && src.State.Players[target.Owner].Bench.Contains(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return DualCut.HandleAttack(ctx, atk); }
        }

        /// <summary>Pollen Shield: during the opponent's next turn, Ledian can't become affected by a Special Condition.</summary>
        private sealed class PollenShieldMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pollen Shield") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new NoConditionEffect(me.Name + " can't be affected by Special Conditions (Pollen Shield)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class NoConditionEffect : ContinuousEffect
        {
            public NoConditionEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsCondition(GameEngine engine, PokemonInPlay mon, SpecialCondition condition) { return mon != null && mon.Uid == TargetUid; }
        }

        /// <summary>Evolution Helper (Poké-Power): while Nidoqueen is Benched, evolve your Active Pokémon with a card from your deck. Double Claw: 2 coins, 20 more per heads.</summary>
        private sealed class NidoqueenE3 : Mon
        {
            private const string Power = "Evolution Helper";
            private static readonly Mon Claw = new CardsE1.DoubleClawMon();

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                var active = ctx.Me.Active;
                return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon) && active != null && !CardsEVS1.EvolveBlocked(ctx, active)
                       && ctx.Me.Deck.Any(c => CardsEVS1.EvolvesOnto(ctx, active, c.Def));
            }

            protected override void Ability(EffectContext ctx) { CardsEVS1.EvolveFromDeck(ctx, ctx.Me.Active, Power); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claw.HandleAttack(ctx, atk); }
        }

        /// <summary>Empathic Healing (your ruling): 2 damage counters off Celebi and every Pokémon in play (both players') of Celebi's type.</summary>
        private sealed class EmpathicHealingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Empathic Healing") return false;
                var me = ctx.Pokemon;
                if (me == null) return true;
                var types = ctx.Engine.TypesOf(me);
                var targets = ctx.MyPokemon().Concat(ctx.OpponentPokemon()).Where(m => m == me || ctx.Engine.TypesOf(m).Any(types.Contains)).ToList();
                foreach (var mon in targets)
                    if (ctx.InPlay(mon) && mon.DamageCounters > 0) ctx.Heal(mon, 20);
                return true;
            }
        }

        /// <summary>Poison Flame: heads, Burned and Poisoned.</summary>
        private sealed class PoisonFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Poison Flame") return false;
                var defending = ctx.Defending;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Burned);
                ctx.ApplyCondition(defending, SpecialCondition.Poisoned);
                return true;
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Energy Recharge (Poké-Power): when you play Arcanine from your hand to evolve your Active Pokémon, you may flip 3 coins and
        /// attach a basic Energy card from your discard pile to it for each heads. Shake: the opponent switches. White Flames: discard
        /// all Fire Energy cards attached.
        /// </summary>
        private sealed class ArcanineE3 : Mon
        {
            private const string Power = "Energy Recharge";
            private static readonly Mon Shake = new CardsE3B.OpponentSwitchesMon("Shake");

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!EvolvedActive(ctx) || ctx.BasicEnergyInDiscard().Count == 0) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Flip 3 coins and attach a basic Energy card from your discard pile to " + me.Name + " for each heads.")) return;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.AttachCardsAnyWay(ctx.BasicEnergyInDiscard(), heads, (c, m) => m == me, Power, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "White Flames")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me) && !CardsBS1.CopyingWithMetronome) CardsE2.DiscardCards(ctx, me, CardsE2.EnergyCardsOf(ctx, me, EnergyType.Fire));
                    return true;
                }
                return Shake.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Burn Booster: discard an Energy card attached to this Pokémon to use it; 10 more if it was a Fire Energy card.</summary>
        private sealed class BurnBoosterMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Attack(attackIndex).Name != "Burn Booster" || (ctx.Pokemon != null && ctx.Pokemon.Energy.Count > 0);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Burn Booster") return false;
                var me = ctx.Pokemon;
                bool fire = false;
                if (!CardsBS1.CopyingWithMetronome && me != null)
                {
                    if (me.Energy.Count == 0) return Nothing(ctx);
                    var card = ctx.ChooseCards(me.Energy.ToList(), 1, 1, "Burn Booster: discard which Energy card? (a Fire Energy card: 10 more damage)")[0];
                    fire = CardsDRI1.ProvidesType(ctx, me, card, EnergyType.Fire);
                    ctx.Engine.DiscardEnergy(me, card);
                }
                ctx.DamageDefending(atk.Damage + (fire ? 10 : 0));
                return true;
            }
        }

        /// <summary>Eruption: each player discards the top card of their deck; 20 more for each Fire Energy card discarded that way.</summary>
        private sealed class EruptionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eruption") return false;
                var discarded = ctx.Engine.DiscardTopOfDeck(ctx.Player, 1).Concat(ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1)).ToList();
                int fire = discarded.Count(c => CardsE2.IsBasicEnergyOf(c.Def, EnergyType.Fire));
                ctx.DamageDefending(atk.Damage + 20 * fire);
                return true;
            }
        }

        /// <summary>
        /// Streaming Mantle (Poké-Power): when you play this Magcargo from your hand to evolve your Active Pokémon, you may discard the top
        /// 3 cards of your deck and shuffle 3 basic Energy cards from your discard pile into your deck (your ruling: as many as there
        /// are); then the opponent does the same. Crushing Lava: you may discard a basic Fire (Burned) or Fighting (20 more) Energy card.
        /// </summary>
        private sealed class MagcargoE3 : Mon
        {
            private const string Power = "Streaming Mantle";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!EvolvedActive(ctx)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Each player discards the top 3 cards of their deck and shuffles up to 3 basic Energy cards from their discard pile into their deck.")) return;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    ctx.Engine.DiscardTopOfDeck(player, 3);
                    var p = ctx.State.Players[player];
                    var pile = p.Discard.Where(c => c.Def.IsBasicEnergy).ToList();
                    int n = Math.Min(3, pile.Count);
                    if (n > 0)
                    {
                        var picks = pile.Count == n ? pile
                            : ctx.Engine.ChooseCards(player, pile, n, n, Power + ": shuffle " + n + " basic Energy cards from your discard pile into your deck", ctx.Source);
                        foreach (var card in picks)
                            if (p.Discard.Remove(card)) p.Deck.Add(card);
                    }
                    ctx.Engine.ShuffleDeck(player);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Lava") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                var cards = me == null ? new List<CardInstance>() : me.Energy.Where(e => CardsE2.IsBasicEnergyOf(e.Def, EnergyType.Fire) || CardsE2.IsBasicEnergyOf(e.Def, EnergyType.Fighting)).ToList();
                CardInstance discarded = null;
                if (cards.Count > 0 && !CardsBS1.CopyingWithMetronome && ctx.AskYesNo("Crushing Lava: discard a basic Fire (Burned) or Fighting (20 more damage) Energy card?"))
                {
                    var card = ctx.ChooseCards(cards, 1, 1, "Crushing Lava: discard which Energy card?")[0];
                    if (ctx.Engine.DiscardEnergy(me, card)) discarded = card;
                }
                bool fighting = discarded != null && discarded.Def.EnergyProvides == EnergyType.Fighting;
                ctx.DamageDefending(atk.Damage + (fighting ? 20 : 0));
                if (discarded != null && !fighting && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Burned);
                return true;
            }
        }

        /// <summary>Fire Immunity (Poké-Body): no Fire Energy cards from the hand. Collect Fire. Burning Tail: tails, discard a Fire Energy card.</summary>
        private sealed class MoltresE3 : Mon
        {
            private static readonly Mon Collect = new CardsE2B.CollectEnergyMon("Collect Fire", EnergyType.Fire);

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target) { return ImmunityBlocks(src, energy, target, EnergyType.Fire); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Burning Tail")
                {
                    bool heads = ctx.Flip();
                    ctx.DamageDefending(atk.Damage);
                    if (!heads && !CardsBS1.CopyingWithMetronome) CardsE2.DiscardOneOf(ctx, ctx.Pokemon, EnergyType.Fire, "Burning Tail");
                    return true;
                }
                return Collect.HandleAttack(ctx, atk);
            }
        }

        // ================================================================== Water

        /// <summary>Water Immunity (Poké-Body): no Water Energy cards from the hand. Freeze Solid. Ice Cyclone: heads, 10 to each of the opponent's Benched Pokémon; tails, 10 to each of your Pokémon.</summary>
        private sealed class ArticunoE3 : Mon
        {
            private static readonly Mon FreezeSolid = new CardsE2B.CollectEnergyMon("Freeze Solid", EnergyType.Water);

            public override bool BlocksEnergyFromHandTo(EffectContext src, CardInstance energy, PokemonInPlay target) { return ImmunityBlocks(src, energy, target, EnergyType.Water); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Cyclone") return FreezeSolid.HandleAttack(ctx, atk);
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (heads)
                {
                    foreach (var mon in ctx.OpponentBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                    return true;
                }
                foreach (var mon in ctx.MyPokemon())
                {
                    if (!ctx.InPlay(mon)) continue;
                    if (mon == ctx.Pokemon) ctx.DamageSelf(10);
                    else ctx.DamageTarget(mon, 10);
                }
                return true;
            }
        }

        /// <summary>Freeze Lock: heads, the opponent can't attach Energy cards from their hand to their Active Pokémon during their next turn.</summary>
        private sealed class FreezeLockMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Freeze Lock") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip())
                    ctx.AddEffect(new FreezeLockEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't attach Energy cards from their hand to their Active Pokémon (Freeze Lock)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class FreezeLockEffect : ContinuousEffect
        {
            private readonly int affected;

            public FreezeLockEffect(string name, int owner, int affected, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { this.affected = affected; }

            public override bool BlocksEnergyFromHand(GameEngine engine, PokemonInPlay target)
            {
                return target != null && target.Owner == affected && engine.State.Players[affected].Active == target;
            }
        }

        /// <summary>Crushing Ice: 10 more for each [C] in the Defending Pokémon's Retreat Cost (after effects).</summary>
        private sealed class CrushingIceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crushing Ice") return false;
                var defending = ctx.Defending;
                int cost = defending == null ? 0 : Math.Max(0, ctx.Engine.RetreatCostOf(defending));
                ctx.DamageDefending(atk.Damage + 10 * cost);
                return true;
            }
        }

        /// <summary>
        /// Flame Vapor (Poké-Power): when you play Gyarados from your hand to evolve your Active Pokémon, you may flip 2 coins and
        /// discard an Energy card from the Defending Pokémon for each heads. Dragon's Vengeance: base 100 with 7 or more damage counters.
        /// </summary>
        private sealed class GyaradosE3 : Mon
        {
            private const string Power = "Flame Vapor";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Defending;
                if (!EvolvedActive(ctx) || defending == null || defending.Energy.Count == 0 || !ctx.CanAffect(defending)) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Flip 2 coins and discard an Energy card from " + defending.Name + " for each heads.")) return;
                int heads = ctx.FlipCoins(2);
                if (heads > 0 && defending.Energy.Count > 0)
                    ctx.DiscardEnergyFrom(defending, Math.Min(heads, defending.Energy.Count), Power + ": discard Energy cards from " + defending.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon's Vengeance") return false;
                ctx.DamageDefending(ctx.Pokemon != null && ctx.Pokemon.DamageCounters >= 7 ? 100 : atk.Damage);
                return true;
            }
        }

        /// <summary>Primal Stare (Poké-Body): while Omastar is your Active Pokémon, the opponent can't play cards from their hand to evolve their Active Pokémon. Spook: no retreat.</summary>
        private sealed class OmastarE3 : Mon
        {
            private static readonly Mon Spook = new NoRetreatMon("Spook");

            public override bool BlocksEvolutionFromHand(EffectContext src, PokemonInPlay target)
            {
                return src.Pokemon != null && src.IsActive(src.Pokemon) && target != null && target.Owner != src.Player && src.Opp.Active == target;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Spook.HandleAttack(ctx, atk); }
        }

        /// <summary>Trample: a coin for each Benched Pokémon (both sides); heads, 20 damage to it.</summary>
        private sealed class TrampleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Trample") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(mon) && ctx.Flip()) ctx.DamageTarget(mon, 20);
                return true;
            }
        }

        /// <summary>Sudden Growth: attach any number of basic Energy cards from your hand to this Pokémon.</summary>
        private sealed class SuddenGrowthMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sudden Growth") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var basics = ctx.Me.Hand.Where(c => c.Def.IsBasicEnergy).ToList();
                if (basics.Count > 0 && ctx.InPlay(me)) ctx.AttachCardsAnyWay(basics, basics.Count, (c, m) => m == me, "Sudden Growth");
                return true;
            }
        }

        /// <summary>Energy Splash: move 2 Water Energy cards from this Pokémon to 1 or 2 of your Benched Pokémon.</summary>
        private sealed class EnergySplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Splash") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (CardsBS1.CopyingWithMetronome) return true;
                for (int i = 0; i < 2; i++)
                {
                    var water = CardsE2.EnergyCardsOf(ctx, me, EnergyType.Water);
                    var bench = ctx.MyBench();
                    if (!ctx.InPlay(me) || water.Count == 0 || bench.Count == 0) break;
                    var card = ctx.ChooseCards(water, 1, 1, "Energy Splash: move which Water Energy card (" + (i + 1) + " of 2)?")[0];
                    var target = ctx.ChoosePokemon(bench, "Energy Splash: move " + card.Def.Name + " to which Benched Pokémon?");
                    if (target == null) break;
                    ctx.Engine.MoveEnergy(me, target, card);
                }
                return true;
            }
        }

        /// <summary>Rare Metal (Poké-Body): its basic Energy cards provide Metal Energy. Squeeze: heads, 10 more and Paralyzed. Metal Tail: you may flip first; heads 80, tails nothing.</summary>
        private sealed class SteelixE3 : Mon
        {
            public override void ModifyProvidedEnergy(EffectContext src, CardInstance energy, PokemonInPlay holder, List<int> units)
            {
                if (holder == null || holder != src.Pokemon || energy == null || !energy.Def.IsBasicEnergy) return;
                for (int i = 0; i < units.Count; i++) if (units[i] != 0) units[i] = EnergyMask.Of(EnergyType.Metal);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                var defending = ctx.Defending;
                if (atk.Name == "Squeeze")
                {
                    bool heads = ctx.Flip();
                    ctx.DamageDefending(atk.Damage + (heads ? 10 : 0));
                    if (heads && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name != "Metal Tail") return false;
                if (ctx.AskYesNo("Metal Tail: flip a coin before doing damage? (heads: 80 damage; tails: nothing)"))
                {
                    if (!ctx.Flip()) return Nothing(ctx);
                    ctx.DamageDefending(80);
                    return true;
                }
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Thunderspark: 10 damage to each Benched Pokémon (both sides) with an Energy card attached.</summary>
        private sealed class ThundersparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Thunderspark") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(mon) && mon.Energy.Count > 0) ctx.DamageTarget(mon, 10);
                return true;
            }
        }

        /// <summary>
        /// Attract Energy (Poké-Power): when you play this Magneton from your hand to evolve 1 of your Pokémon, move any number of basic
        /// Energy cards from your other Pokémon to it. Electric Blast: you may discard all Lightning Energy cards; that many damage
        /// counters on the opponent's Benched Pokémon in any way you like.
        /// </summary>
        private sealed class MagnetonLightningE3 : Mon
        {
            private const string Power = "Attract Energy";

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (me == null || !ctx.IsMyTurn) return;
                var others = ctx.MyPokemon(m => m != me && m.Energy.Any(e => e.Def.IsBasicEnergy));
                if (others.Count == 0) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Move any number of basic Energy cards from your other Pokémon to " + me.Name + ".")) return;
                var pool = others.SelectMany(m => m.Energy.Where(e => e.Def.IsBasicEnergy)).ToList();
                var picks = ctx.ChooseCards(pool, 0, pool.Count, Power + ": move which basic Energy cards to " + me.Name + "?");
                foreach (var card in picks)
                {
                    var from = ctx.MyPokemon().FirstOrDefault(m => m != me && m.Energy.Contains(card));
                    if (from != null) ctx.Engine.MoveEnergy(from, me, card);
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electric Blast") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                var lightning = CardsE2.EnergyCardsOf(ctx, me, EnergyType.Lightning);
                if (lightning.Count == 0 || !ctx.InPlay(me) || CardsBS1.CopyingWithMetronome || ctx.Opp.Bench.Count == 0) return true;
                if (!ctx.AskYesNo("Electric Blast: discard all Lightning Energy cards from " + me.Name + " to put that many damage counters on your opponent's Benched Pokémon?")) return true;
                int n = CardsE2.DiscardCards(ctx, me, lightning);
                if (n > 0) ctx.PlaceCountersAnyWay(n, () => ctx.OpponentBench());
                return true;
            }
        }

        /// <summary>Bounce Off: if the Active Pokémon have different numbers of Energy cards, the player whose Active Pokémon has fewer switches it with 1 of their Benched Pokémon.</summary>
        private sealed class BounceOffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bounce Off") return false;
                var me = ctx.Pokemon;
                var defending = ctx.Defending;
                int mine = me == null ? 0 : me.Energy.Count;
                int theirs = defending == null ? 0 : defending.Energy.Count;
                ctx.DamageDefending(atk.Damage);
                if (mine == theirs) return true;
                if (mine < theirs) { if (ctx.InPlay(me) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench(); }
                else if (ctx.Opp.Bench.Count > 0 && ctx.Opp.Active != null && !ctx.Engine.IsKnockedOut(ctx.Opp.Active)) ctx.OpponentSwitchesActive();
                return true;
            }
        }

        /// <summary>Zzzap: 20 damage to each Pokémon with a Poké-Body or Poké-Power (both sides; no Weakness or Resistance).</summary>
        private sealed class ZzzapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Zzzap") return false;
                foreach (var mon in ctx.MyPokemon().Concat(ctx.OpponentPokemon()).ToList())
                    if (ctx.InPlay(mon) && mon.Def.Abilities.Any(a => a.IsPokePowerOrBody)) ctx.DamageWith(mon, 20, false, false, false);
                return true;
            }
        }

        /// <summary>Lightning Storm: tails, 2 damage counters on this Pokémon.</summary>
        internal sealed class LightningStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Storm") return false;
                bool heads = ctx.Flip();
                ctx.DamageDefending(atk.Damage);
                if (!heads && ctx.InPlay(ctx.Pokemon)) ctx.PlaceCounters(ctx.Pokemon, 2);
                return true;
            }
        }

        /// <summary>Pure Body (Poké-Body): like Suicune's, with Lightning. Lightning Sphere: you may flip; heads, discard all Lightning Energy cards, 20 more per card.</summary>
        private sealed class RaikouE3 : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (target != src.Pokemon || energy == null || !CardsE2.IsBasicEnergyOf(energy.Def, EnergyType.Lightning)) return;
                if (!src.InPlay(target) || target.Energy.Count == 0) return;
                src.Log("Pure Body: an Energy card attached to " + target.Name + " has to be discarded.");
                src.DiscardEnergyFrom(target, 1, "Pure Body: discard an Energy card attached to " + target.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lightning Sphere") return false;
                var me = ctx.Pokemon;
                int n = 0;
                if (!CardsBS1.CopyingWithMetronome && ctx.AskYesNo("Lightning Sphere: flip a coin? (heads: discard all Lightning Energy cards, 20 more damage for each)") && ctx.Flip())
                    n = CardsE2.DiscardCards(ctx, me, CardsE2.EnergyCardsOf(ctx, me, EnergyType.Lightning));
                ctx.DamageDefending(atk.Damage + 20 * n);
                return true;
            }
        }

        // ================================================================== Psychic

        /// <summary>Energy Jump (Poké-Power): move an Energy card from 1 of your Pokémon to another. Psychic: 10 more per Energy card on the Defending Pokémon.</summary>
        private sealed class AlakazamE3 : Mon
        {
            private const string Power = "Energy Jump";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return CardsE1.NoCondition(ctx.Pokemon) && ctx.MyPokemon().Count >= 2 && ctx.MyPokemon(m => m.Energy.Count > 0).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var donors = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (donors.Count == 0) return;
                var from = ctx.ChoosePokemon(donors, Power + ": move an Energy card from which Pokémon?");
                if (from == null) return;
                var card = ctx.ChooseCards(from.Energy.ToList(), 1, 1, Power + ": move which Energy card?")[0];
                var to = ctx.MyPokemon(m => m != from);
                var target = ctx.ChoosePokemon(to, Power + ": move " + card.Def.Name + " to which Pokémon?");
                if (target != null) ctx.Engine.MoveEnergy(from, target, card);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Psychic") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + 10 * (defending == null ? 0 : defending.Energy.Count));
                return true;
            }
        }

        /// <summary>
        /// Manipulate (Poké-Power): when you play Gengar from your hand to evolve your Active Pokémon, you may put a Basic Pokémon (not
        /// a Baby Pokémon) from your discard pile onto your Bench, then flip 3 coins and attach a basic Energy card from your discard
        /// pile to it for each heads. Hydrokinesis: heads Burned, tails Confused.
        /// </summary>
        private sealed class GengarE3 : Mon
        {
            private const string Power = "Manipulate";
            private static readonly Mon Hydrokinesis = new CardsSI.HeadsTailsConditionMon("Hydrokinesis", SpecialCondition.Burned, SpecialCondition.Confused);

            private static List<CardInstance> Basics(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => ctx.Engine.IsPlayableBasic(c.Def) && !c.Def.IsBaby && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!EvolvedActive(ctx) || ctx.BenchSpace <= 0 || Basics(ctx).Count == 0) return;
                if (!TriggerOnce(ctx, Power, true, "Use " + Power + "? Put a Basic Pokémon from your discard pile onto your Bench, then flip 3 coins to attach basic Energy to it.")) return;
                var basics = Basics(ctx);
                var pick = ctx.ChooseCards(basics, 1, 1, Power + ": put which Basic Pokémon onto your Bench?")[0];
                if (!ctx.Me.Discard.Remove(pick)) return;
                var benched = ctx.Engine.PutOntoBench(ctx.Player, pick, false);
                int heads = ctx.FlipCoins(3);
                var energy = ctx.BasicEnergyInDiscard();
                if (heads > 0 && energy.Count > 0) ctx.AttachCardsAnyWay(energy, heads, (c, m) => m == benched, Power, false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Hydrokinesis.HandleAttack(ctx, atk); }
        }

        /// <summary>Energy Burst: heads, 10 damage times the Energy attached to this Pokémon and the Defending Pokémon.</summary>
        private sealed class EnergyBurstMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Burst") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                var defending = ctx.Defending;
                int n = ctx.EnergyCount(ctx.Pokemon) + (defending == null ? 0 : ctx.EnergyCount(defending));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Star Back: attach a basic Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class StarBackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Back") return false;
                ctx.DamageDefending(atk.Damage);
                var pile = ctx.BasicEnergyInDiscard();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, 1, (c, m) => true, "Star Back", false);
                return true;
            }
        }

        /// <summary>Synchronicity (Poké-Body): any Technical Machine can be attached to Xatu. Warp Hole: heads, a card from your discard pile on top of your deck.</summary>
        private sealed class XatuE3 : Mon
        {
            public override bool AllowsAnyTechnicalMachine(EffectContext src, PokemonInPlay mon) { return mon != null && mon == src.Pokemon; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Warp Hole") return false;
                ctx.DamageDefending(atk.Damage);
                var discard = ctx.Me.Discard.ToList();
                if (discard.Count == 0 || !ctx.Flip()) return true;
                var pick = ctx.ChooseCards(discard, 1, 1, "Warp Hole: put which card from your discard pile on top of your deck?")[0];
                if (ctx.Me.Discard.Remove(pick)) ctx.Me.Deck.Insert(0, pick);
                ctx.Log("Warp Hole: " + pick.Def.Name + " went on top of " + ctx.Engine.Possessive(ctx.Player) + " deck.");
                return true;
            }
        }

        // ================================================================== Fighting

        /// <summary>Ancient Wind (Poké-Power): while Aerodactyl is Active, ignore all Poké-Bodies until the end of your turn. Rising Lunge: heads, 20 more.</summary>
        private sealed class AerodactylE3 : Mon
        {
            private const string Power = "Ancient Wind";
            private static readonly Mon Lunge = new FlipBonusMon("Rising Lunge", 20);

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AddEffect(new NoPokeBodyEffect("Every Poké-Body is ignored this turn (Ancient Wind)", ctx.Player, ctx.Turn));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lunge.HandleAttack(ctx, atk); }
        }

        private sealed class NoPokeBodyEffect : ContinuousEffect
        {
            public NoPokeBodyEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override bool StopsPokeBody(GameEngine engine, PokemonInPlay mon, string bodyName) { return true; }
        }

        /// <summary>Immunity (Poké-Body): prevent all effects of the opponent's attacks done to Machamp (not damage). Drag Off: first switch in 1 of the opponent's Benched Pokémon. Hurricane Punch: 4 coins.</summary>
        private sealed class MachampE3 : Mon
        {
            private static readonly Mon Hurricane = new CoinsMon("Hurricane Punch", 4);

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return target != null && target == src.Pokemon && kind == EffectSourceKind.Attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drag Off") return Hurricane.HandleAttack(ctx, atk);
                if (ctx.GustTargets().Count > 0) ctx.GustOpponent("Drag Off: choose 1 of your opponent's Benched Pokémon to switch in");
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Strange Spiral (Poké-Power): while Active, discard a basic Energy card from Poliwrath; the Defending Pokémon is now Confused. Spiral Punch: flip until tails, 20 more per heads.</summary>
        private sealed class PoliwrathE3 : Mon
        {
            private const string Power = "Strange Spiral";

            protected override string Activated { get { return Power; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.PokemonIsActive && CardsE1.NoCondition(ctx.Pokemon) && ctx.Pokemon.Energy.Any(e => e.Def.IsBasicEnergy) && ctx.Defending != null;
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                var basics = me.Energy.Where(e => e.Def.IsBasicEnergy).ToList();
                var card = ctx.ChooseCards(basics, 1, 1, Power + ": discard which basic Energy card?")[0];
                if (!ctx.Engine.DiscardEnergy(me, card)) return;
                var defending = ctx.Defending;
                if (defending != null && ctx.CanAffect(defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spiral Punch") return false;
                ctx.DamageDefending(atk.Damage + 20 * ctx.FlipUntilTails());
                return true;
            }
        }

        /// <summary>Earth Bomb: 20 damage to itself and 10 to each Benched Pokémon (both sides). Rock Throw: plain.</summary>
        private sealed class EarthBombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Earth Bomb") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.MyBench().Concat(ctx.OpponentBench()).ToList())
                    if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10);
                if (ctx.InPlay(ctx.Pokemon)) ctx.DamageSelf(20);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Spinning Blow: 2 coins, 20 per heads; 2 heads, switch with 1 of your Benched Pokémon.</summary>
        private sealed class SpinningBlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Spinning Blow") return false;
                int heads = ctx.FlipCoins(2);
                ctx.DamageDefending(atk.Damage * heads);
                if (heads == 2 && ctx.InPlay(ctx.Pokemon) && ctx.Me.Bench.Count > 0) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Scatterbomb: 2 coins; 10 to each of the opponent's Benched Pokémon per heads and to each of yours per tails.</summary>
        private sealed class ScatterbombMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scatterbomb") return false;
                int heads = ctx.FlipCoins(2);
                int tails = 2 - heads;
                ctx.DamageDefending(atk.Damage);
                if (heads > 0) foreach (var mon in ctx.OpponentBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10 * heads);
                if (tails > 0) foreach (var mon in ctx.MyBench()) if (ctx.InPlay(mon)) ctx.DamageTarget(mon, 10 * tails);
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Dark Gaze (Poké-Body): while Umbreon is your Active Pokémon, Benched Pokémon (both sides) can't use Poké-Powers. Sharp Claws: heads, 30 more.</summary>
        private sealed class UmbreonE3 : Mon
        {
            private static readonly Mon Claws = new FlipBonusMon("Sharp Claws", 30);

            public override bool StopsPokePowersOf(EffectContext src, PokemonInPlay mon)
            {
                var me = src.Pokemon;
                if (me == null || mon == null || mon == me || !src.IsActive(me)) return false;
                if (!src.State.Players[mon.Owner].Bench.Contains(mon)) return false;
                return !src.Engine.AbilityBlocked(me, "Dark Gaze");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Claws.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>
        /// Good Neighbor (Poké-Power): while Wigglytuff is Benched, flip; heads, each player removes up to 2 damage counters from their
        /// Active Pokémon (1 Wigglytuff's Good Neighbor a turn). Do the Wave: heads, 10 for each of your Pokémon in play.
        /// </summary>
        private sealed class WigglytuffE3 : Mon
        {
            protected override string Activated { get { return "Good Neighbor"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Pokemon != null && ctx.Me.Bench.Contains(ctx.Pokemon); }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip()) return;
                foreach (var active in new[] { ctx.Me.Active, ctx.Opp.Active })
                    if (active != null && active.DamageCounters > 0) ctx.Heal(active, 20);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Do the Wave") return false;
                if (!ctx.Flip()) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage * ctx.MyPokemon().Count);
                return true;
            }
        }
    }
}
