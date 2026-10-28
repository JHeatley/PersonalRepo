using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Evolving Skies (EVS, 2021), part 2: Pokémon 91-140 (Fighting to Colorless), the full-art-only Flareon V, Vaporeon V,
    /// Jolteon V and Dragonite V, the gold Froslass, Inteleon and Cresselia, the Trainers (Turffield Stadium is only a gold
    /// card here) and Treasure Energy. See CardsEVS1 for the rest of the set and your rulings.
    /// </summary>
    public static class CardsEVS2
    {
        public static void Register(CardScriptRegistry r)
        {
            // Fighting
            r.RegisterByKey("EVS-91", new LockAfterAttackMon("Crashing Fangs", false));
            r.RegisterByKey("EVS-92", new ComboMon(new HuntingClawMon(), new Cards30CC.BenchHitMon("Max Edge", 30)));
            // Darkness
            r.RegisterByKey("EVS-93", new CardsEVS1.ChargeFromHandMon("Malevolent Charge", EnergyType.Darkness, new CardsASR1.PlusMon("Fiery Wrath", 50, OpponentPrizesTaken)));
            r.RegisterByKey("EVS-96", new ParalyzeFlipMon("Fake Out"));
            r.RegisterByKey("EVS-97", new ComboMon(new ShiftadieuMon(), new CardsASC.RandomDiscardMon("Nipping Cyclone")));
            r.RegisterByKey("EVS-98", new ShieldAttackMon("Hard Head", 10));
            r.RegisterByKey("EVS-99", new CardsASC.RandomDiscardMon("Shakedown"));
            r.RegisterByKey("EVS-100", new CardsPOR.PoisonRingMon("Trash Stench"));
            r.RegisterByKey("EVS-101", new GarbodorVmax());
            r.RegisterByKey("EVS-103", new ZoroarkEVS());
            r.RegisterByKey("EVS-105", new ThievulEVS());
            // Dragon
            r.RegisterByKey("EVS-108", new DodgeMon("Hard Roll", false));
            r.RegisterByKey("EVS-109", new SalamenceEVS());
            r.RegisterByKey("EVS-112", new CardsEVS1.DefenderCantAttackMon("Chrono Wind", CardsEVS1.IsPokemonVCard));
            r.RegisterByKey("EVS-113", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("EVS-115", new CardsASR1.PlusMon("Dragon Counter", 100, PrizesOpponentTookLastTurn));
            r.RegisterByKey("EVS-116", new ExtremeFreezeMon());
            r.RegisterByKey("EVS-117", new ComboMon(new CardsCEC3.SpreadAmountMon("Boomburst", 20), new CardsFST2.ConditionBonusMon("Synchro Loud", 120, CardsEVS1.SameHandSize)));
            r.RegisterByKey("EVS-118", new JudgmentSurgeMon());
            r.RegisterByKey("EVS-119", new CardsSVP.BenchHurtBonusMon("Berserk", 90));
            r.RegisterByKey("EVS-120", new ComboMon(new CardsASR1.TimesMon("Acidic Mucus", OpponentPokemonWithAbility), new CardsCRZ1.VBonusMon("Fighting Tackle", 80)));
            r.RegisterByKey("EVS-121", new ComboMon(new CardsASR1.TimesMon("Thick Mucus", OpponentSpecialEnergy), new CardsCRZ1.VBonusMon("Fighting Tackle", 80)));
            r.RegisterByKey("EVS-124", new DragonEnergyMon());
            // Colorless
            r.RegisterByKey("EVS-128", new LivePaintingMon());
            r.RegisterByKey("EVS-129", new SmackNSlackMon("Smack 'n' Slack"));
            r.RegisterByKey("EVS-130", new CardsPAF1.DiscardStadiumMon("Shatter"));
            r.RegisterByKey("EVS-131", new SlakingEVS());
            r.RegisterByKey("EVS-133", new SearchAttackMon("Lead", CardsEVS1.IsSupporterCard, 1));
            r.RegisterByKey("EVS-134", new RecoilMon("Take Down", 20));
            r.RegisterByKey("EVS-135", new StoutlandEVS());
            r.RegisterByKey("EVS-136", new CardsDRI1.WhirlwindMon("Whirlwind"));
            r.RegisterByKey("EVS-137", new ComboMon(new CardsPAL1.EvolutionBonusMon("Valiant Talons", 60), new RecoilMon("Brave Bird", 50)));
            r.RegisterByKey("EVS-138", new ComboMon(new TailwindDrawMon(), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("EVS-139", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("EVS-140", new ComboMon(new NoRetreatMon("Clutch"), new Cards30C.TypeEnergyBonusMon("Nitro Dive", EnergyType.Fire, 80)));
            // Full-art-only Pokémon V
            r.RegisterByKey("EVS-169", new ComboMon(new CardsMEG.ChargeSelfMon("Flaming Breath", EnergyType.Fire, 1), new ConditionAttackMon("Scorching Column", SpecialCondition.Burned)));
            r.RegisterByKey("EVS-172", new ComboMon(new DrawAttackMon("Triple Draw", 3), new SwitchSelfMon("Splash Jump", false)));
            r.RegisterByKey("EVS-177", new ComboMon(new CardsMEG.SnipeMon("Thunder Spear", 20), new CoinsMon("Pin Missile", 4)));
            r.RegisterByKey("EVS-191", new ComboMon(new CardsPAF2.IgnoreDefenderEffectsMon("Shred"), new CardsBUS1.OwnBenchQuakeMon("Dragon Gale", 20)));
            // Gold secret rares that are Chilling Reign Pokémon
            r.RegisterByKey("EVS-226", new FroslassEVS());
            r.RegisterByKey("EVS-227", new InteleonEVS());
            r.RegisterByKey("EVS-228", new ComboMon(new CrescentGlowMon(), new CardsFST2.ConditionBonusMon("Photon Laser", 90, FiveEnergyInPlay)));

            // Trainers
            r.RegisterByName("Aroma Lady", new AromaLady());
            r.RegisterByName("Boost Shake", new BoostShake());
            r.RegisterByName("Copycat", new Copycat());
            r.RegisterByName("Crystal Cave", new CrystalCave());
            r.RegisterByName("Digging Gloves", new CardsBRS2.TypeGloves(EnergyType.Fighting));
            r.RegisterByName("Dream Ball", new DreamBall());
            r.RegisterByName("Elemental Badge", new ElementalBadge());
            r.RegisterByName("Full Face Guard", new FullFaceGuard());
            r.RegisterByName("Lucky Ice Pop", new LuckyIcePop());
            r.RegisterByName("Moon & Sun Badge", new MoonAndSunBadge());
            r.RegisterByName("Rapid Strike Scroll of the Flying Dragon", new StrikeScroll(false));
            r.RegisterByName("Ribbon Badge", new RibbonBadge());
            r.RegisterByName("Rubber Gloves", new CardsBRS2.TypeGloves(EnergyType.Lightning));
            r.RegisterByName("Shopping Center", new ShoppingCenter());
            r.RegisterByName("Single Strike Scroll of the Fanged Dragon", new StrikeScroll(true));
            r.RegisterByName("Snow Leaf Badge", new SnowLeafBadge());
            r.RegisterByName("Spirit Mask", new SpiritMask());
            r.RegisterByName("Stormy Mountains", new StormyMountains());
            r.RegisterByName("Switching Cups", new SwitchingCups());
            r.RegisterByName("Toy Catcher", new ToyCatcher());
            r.RegisterByName("Turffield Stadium", new TurffieldStadium());
            r.RegisterByName("Zinnia's Resolve", new ZinniasResolve());
            // Energy
            r.RegisterByName("Treasure Energy", new TreasureEnergy());
        }

        // ================================================================== Counters

        private static int OpponentPrizesTaken(EffectContext ctx) { return ctx.OpponentPrizesTaken; }

        /// <summary>Prize cards the opponent took during their last turn (Dragon Counter).</summary>
        private static int PrizesOpponentTookLastTurn(EffectContext ctx)
        {
            return ctx.Opp.PrizesTakenTurn == ctx.OpponentsLastTurn ? ctx.Opp.PrizesTakenThatTurn : 0;
        }

        private static int OpponentPokemonWithAbility(EffectContext ctx) { return ctx.OpponentPokemon(m => ctx.Engine.HasAbility(m)).Count; }

        private static int OpponentSpecialEnergy(EffectContext ctx)
        {
            return ctx.OpponentPokemon().Sum(m => m.Energy.Count(e => e.Def.IsAttachedSpecialEnergy));
        }

        private static bool FiveEnergyInPlay(EffectContext ctx) { return ctx.MyPokemon().Sum(m => ctx.EnergyCount(m)) >= 5; }

        /// <summary>A plain Stage 1 Pokémon card (not a Pokémon VMAX or VSTAR, LV.X, BREAK or LEGEND).</summary>
        internal static bool IsPlainStage1(CardDef d)
        {
            return d.IsPokemon && d.Stage == Stage.Stage1 && !d.IsVmax && !d.IsVstar && !d.IsLevelUp && !d.IsBreak && !d.IsLegend;
        }

        // ================================================================== Fighting

        /// <summary>Hunting Claw: Knock Out 1 of the opponent's Pokémon that has 60 HP or less remaining.</summary>
        private sealed class HuntingClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hunting Claw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => ctx.RemainingHp(m) <= 60 && ctx.CanAffect(m));
                if (targets.Count == 0)
                {
                    ctx.Log("Hunting Claw: none of " + ctx.Engine.Possessive(ctx.Opponent) + " Pokémon have 60 HP or less remaining.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Hunting Claw: Knock Out which of your opponent's Pokémon (60 HP or less remaining)?");
                ctx.Engine.KnockOut(target, "Hunting Claw");
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Shiftadieu: if the opponent's Active Pokémon has damage counters, it and all attached cards go to their hand.</summary>
        private sealed class ShiftadieuMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shiftadieu") return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || defending.DamageCounters == 0)
                {
                    ctx.Log("Shiftadieu: the Defending Pokémon has no damage counters on it.");
                    return true;
                }
                if (!ctx.CanAffect(defending))
                {
                    ctx.Log("The effect of Shiftadieu on " + defending.Name + " was prevented.");
                    return true;
                }
                if (!ctx.Engine.CanPutIntoHand(defending))
                {
                    ctx.Log(defending.Name + " can't be put into the hand.");
                    return true;
                }
                ctx.Engine.ReturnPokemonToHand(defending);
                return true;
            }
        }

        /// <summary>Rubbish Collecting: this Pokémon can have up to 2 Pokémon Tools (the engine discards down to 1 if the Ability stops working).</summary>
        private sealed class GarbodorVmax : Mon
        {
            private static readonly Mon Malodor = new CardsPOR.PoisonRingMon("G-Max Malodor");

            public override int ModifyMaxTools(EffectContext src, PokemonInPlay mon, int max)
            {
                return src.Pokemon != null && mon == src.Pokemon ? Math.Max(max, 2) : max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Malodor.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Phantom Transformation: once during your turn, choose a Stage 1 Pokémon (not Zoroark) in your discard pile; discard
        /// this Pokémon and all attached cards and put the chosen Pokémon in its place. Your ruling (v58): the new Pokémon counts
        /// as being in play since Zoroark was (it can evolve this turn if Zoroark could have).
        /// </summary>
        private sealed class ZoroarkEVS : Mon
        {
            private const string Transform = "Phantom Transformation";

            protected override string Activated { get { return Transform; } }
            protected override bool AbilityReady(EffectContext ctx) { return Choices(ctx).Count > 0; }

            private static List<CardInstance> Choices(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => IsPlainStage1(c.Def) && c.Def.Name != "Zoroark" && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
            }

            protected override void Ability(EffectContext ctx)
            {
                var old = ctx.Pokemon;
                var choices = Choices(ctx);
                if (choices.Count == 0 || !ctx.InPlay(old)) return;
                var pick = choices.Count == 1 ? choices[0] : ctx.ChooseCards(choices, 1, 1, Transform + ": choose a Stage 1 Pokémon from your discard pile")[0];
                bool wasActive = ctx.IsActive(old);
                int slot = ctx.Me.Bench.IndexOf(old);
                if (!ctx.Me.Discard.Remove(pick)) return;
                ctx.Engine.DiscardPokemonFromPlay(old);
                var mon = new PokemonInPlay(ctx.State.NextUid++, ctx.Player, pick, old.TurnEnteredPlay);
                mon.TurnEvolved = old.TurnEvolved;
                mon.PlaySeq = mon.SpotSeq = ctx.Engine.NextLockSeq();
                if (wasActive) ctx.Me.Active = mon;
                else ctx.Me.Bench.Insert(Math.Max(0, Math.Min(slot, ctx.Me.Bench.Count)), mon);
                ctx.Engine.Emit(GameEventType.Info, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " put " + pick.Def.Name + " into play in Zoroark's place (" + Transform + ").", pick.Id, mon.Uid);
            }
        }

        /// <summary>
        /// Fumbling Hands: when you play this Pokémon from your hand to evolve, you may have each player shuffle their hand and put
        /// it on the bottom of their deck; if either player did, each player draws 4 cards.
        /// </summary>
        private sealed class ThievulEVS : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0 && ctx.Opp.Hand.Count == 0) return;
                if (!TriggerOnce(ctx, "Fumbling Hands", true, "Use Fumbling Hands? Each player puts their hand on the bottom of their deck, then each player draws 4 cards.")) return;
                bool any = false;
                foreach (int player in new[] { ctx.Player, ctx.Opponent })
                {
                    var p = ctx.State.Players[player];
                    if (p.Hand.Count == 0) continue;
                    var hand = p.Hand.ToList();
                    ctx.State.Rng.Shuffle(hand);
                    p.Hand.Clear();
                    p.Deck.AddRange(hand);
                    any = true;
                    ctx.Engine.Log(player, ctx.Engine.PlayerName(player) + " put " + hand.Count + (hand.Count == 1 ? " card" : " cards") + " on the bottom of their deck (Fumbling Hands).");
                }
                if (!any) return;
                foreach (int player in new[] { ctx.Player, ctx.Opponent }) ctx.Engine.DrawCards(player, 4);
            }
        }

        // ================================================================== Dragon

        /// <summary>Intimidating Roar: once during your turn, your opponent switches their Active Pokémon with 1 of their Benched Pokémon.</summary>
        private sealed class SalamenceEVS : Mon
        {
            private static readonly Mon FierceDragon = new CardsPFL.DamagedBonusMon("Fierce Dragon", 120);

            protected override string Activated { get { return "Intimidating Roar"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }
            protected override void Ability(EffectContext ctx) { ctx.OpponentSwitchesActive(); }
            protected override bool Attack(EffectContext ctx, AttackDef atk) { return FierceDragon.HandleAttack(ctx, atk); }
        }

        /// <summary>Extreme Freeze: discard any amount of Water Energy from your Pokémon; 60 damage for each card discarded.</summary>
        private sealed class ExtremeFreezeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extreme Freeze") return false;
                var cards = new List<CardInstance>();
                var holders = new Dictionary<int, PokemonInPlay>();
                foreach (var mon in ctx.MyPokemon())
                    foreach (var energy in mon.Energy)
                    {
                        if (!CardsDRI1.ProvidesType(ctx, mon, energy, EnergyType.Water)) continue;
                        cards.Add(energy);
                        holders[energy.Id] = mon;
                    }
                int discarded = 0;
                if (cards.Count > 0)
                {
                    var picks = ctx.ChooseCards(cards, 0, cards.Count, "Extreme Freeze: discard any number of Water Energy from your Pokémon (60 damage each)");
                    foreach (var pick in picks)
                        if (ctx.Engine.DiscardEnergy(holders[pick.Id], pick)) discarded++;
                }
                if (discarded > 0) ctx.DamageDefending(atk.Damage * discarded);
                else ctx.Log("Extreme Freeze: no Water Energy was discarded, so it did no damage.");
                return true;
            }
        }

        /// <summary>Judgment Surge: 40 damage to 1 of the opponent's Pokémon for each Prize card they have taken.</summary>
        private sealed class JudgmentSurgeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Judgment Surge") return false;
                int amount = 40 * ctx.OpponentPrizesTaken;
                if (amount <= 0)
                {
                    ctx.Log("Judgment Surge: " + ctx.Engine.PlayerName(ctx.Opponent) + " hasn't taken any Prize cards, so it does no damage.");
                    return true;
                }
                ctx.DamageAnyOpponent(amount);
                return true;
            }
        }

        /// <summary>Dragon Energy: 240 damage, 20 less for each damage counter on this Pokémon.</summary>
        private sealed class DragonEnergyMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Energy") return false;
                int damage = Math.Max(0, atk.Damage - 20 * ctx.Pokemon.DamageCounters);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log("Dragon Energy did no damage.");
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Live Painting: reveal basic Energy cards from your hand; 30 more damage for each type revealed (one of each type is shown).</summary>
        private sealed class LivePaintingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Live Painting") return false;
                var shown = new List<CardInstance>();
                foreach (var card in ctx.Me.Hand)
                    if (card.Def.IsBasicEnergy && !shown.Any(s => s.Def.EnergyProvides == card.Def.EnergyProvides)) shown.Add(card);
                if (shown.Count > 0) ctx.Engine.Reveal(ctx.Player, shown, "revealed basic Energy cards from " + ctx.Engine.Their(ctx.Player) + " hand (Live Painting)");
                ctx.DamageDefending(atk.Damage + 30 * shown.Count);
                return true;
            }
        }

        /// <summary>Smack 'n' Slack: this Pokémon is now Asleep. Used for Smack 'n' Slack, Falling Down (the attack name is passed in).</summary>
        internal sealed class SmackNSlackMon : Mon
        {
            private readonly string attack;

            public SmackNSlackMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon) && ctx.PokemonIsActive) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>Act Freely: if a Stadium is in play, this Pokémon can't attack. Rout: 30 more for each of the opponent's Benched Pokémon.</summary>
        private sealed class SlakingEVS : Mon
        {
            private static readonly Mon Rout = new CardsASR1.PlusMon("Rout", 30, OpponentBenchCount);

            private static int OpponentBenchCount(EffectContext ctx) { return ctx.Opp.Bench.Count; }

            public override bool PreventsAttacksOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.State.Stadium != null;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Rout.HandleAttack(ctx, atk); }
        }

        /// <summary>Intimidating Fang: while this Pokémon is Active, attacks used by the opponent's Active Pokémon do 30 less damage (before Weakness and Resistance).</summary>
        private sealed class StoutlandEVS : Mon
        {
            private static readonly Mon KnockAway = new FlipBonusMon("Knock Away", 100);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                var attacker = info.Attacker;
                if (me == null || !src.IsActive(me) || attacker == null || attacker.Owner == src.Player || !info.AgainstOpponent) return damage;
                return src.Opp.Active == attacker ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return KnockAway.HandleAttack(ctx, atk); }
        }

        /// <summary>Tailwind Draw: draw a card; if you went second and it's your first turn, draw 3 more.</summary>
        private sealed class TailwindDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tailwind Draw") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.Draw(1);
                if (CardsEVS1.WentSecondFirstTurn(ctx)) ctx.Draw(3);
                return true;
            }
        }

        // ================================================================== Gold secret rares

        /// <summary>Frost Over: when you play this Pokémon from your hand to evolve, you may attach a Water Energy card from your discard pile to 1 of your Pokémon.</summary>
        private sealed class FroslassEVS : Mon
        {
            private static readonly Mon CrystalBreath = new LockAfterAttackMon("Crystal Breath", false);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var water = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return;
                if (!TriggerOnce(ctx, "Frost Over", true, "Use Frost Over? Attach a Water Energy card from your discard pile to 1 of your Pokémon.")) return;
                ctx.AttachCardsAnyWay(water, 1, (card, holder) => true, "Frost Over", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return CrystalBreath.HandleAttack(ctx, atk); }
        }

        /// <summary>Quick Shooting: once during your turn, put 2 damage counters on 1 of your opponent's Pokémon.</summary>
        private sealed class InteleonEVS : Mon
        {
            protected override string Activated { get { return "Quick Shooting"; } }
            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.HasPokemonInPlay; }

            protected override void Ability(EffectContext ctx)
            {
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Quick Shooting: put 2 damage counters on which of your opponent's Pokémon?");
                ctx.PlaceCounters(target, 2);
            }
        }

        /// <summary>Crescent Glow: attach a Psychic Energy card from your deck to 1 of your Pokémon (up to 3 to 1 Pokémon if you went second, on your first turn).</summary>
        private sealed class CrescentGlowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Crescent Glow") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                int max = CardsEVS1.WentSecondFirstTurn(ctx) ? 3 : 1;
                var found = ctx.SearchMyDeck(cd => CardsEVS1.IsEnergyOf(cd, EnergyType.Psychic), max, "Crescent Glow: choose up to " + max + (max == 1 ? " Psychic Energy card" : " Psychic Energy cards"));
                if (found.Count > 0)
                {
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Crescent Glow: attach the Psychic Energy to which of your Pokémon?");
                    foreach (var card in found) ctx.Engine.AttachEnergyCard(card, target);
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        // ================================================================== Trainers

        /// <summary>Aroma Lady: draw 2 cards; if you do, your Active Pokémon recovers from all Special Conditions.</summary>
        private sealed class AromaLady : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Draw(2) > 0 && ctx.Me.Active != null) ctx.Engine.RemoveAllConditions(ctx.Me.Active);
            }
        }

        /// <summary>
        /// Boost Shake: evolve 1 of your Pokémon from the deck, even on your first turn or a Pokémon put into play this turn, or a
        /// Stage 1 that evolved this turn (Compendium, 2021-09-09). Your turn ends.
        /// </summary>
        private sealed class BoostShake : CardScript
        {
            private static bool CanBoost(EffectContext ctx, PokemonInPlay mon) { return !CardsEVS1.EvolveBlocked(ctx, mon); }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.MyPokemon(m => CanBoost(ctx, m)).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var mine = ctx.MyPokemon(m => CanBoost(ctx, m));
                var found = ctx.SearchMyDeck(cd => mine.Any(m => CardsEVS1.EvolvesOnto(ctx, m, cd)), 1, "Boost Shake: choose a card that evolves from 1 of your Pokémon", false);
                if (found.Count > 0)
                {
                    var card = found[0];
                    var targets = mine.Where(m => CardsEVS1.EvolvesOnto(ctx, m, card.Def)).ToList();
                    var target = ctx.ChoosePokemon(targets, "Boost Shake: evolve which Pokémon into " + card.Def.Name + "?");
                    ctx.Engine.EvolvePokemon(target, card);
                }
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Copycat: shuffle your hand into your deck, then draw a card for each card in your opponent's hand.</summary>
        private sealed class Copycat : CardScript
        {
            public override void ResolveTrainer(EffectContext ctx)
            {
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(ctx.Opp.Hand.Count);
            }
        }

        /// <summary>Crystal Cave: once during each player's turn, that player may heal 30 damage from each of their Metal and Dragon Pokémon.</summary>
        private sealed class CrystalCave : StadiumAction
        {
            private static bool Fits(EffectContext ctx, PokemonInPlay mon)
            {
                return ctx.Engine.IsType(mon, EnergyType.Metal) || ctx.Engine.IsType(mon, EnergyType.Dragon);
            }

            protected override bool Ready(EffectContext ctx) { return ctx.MyPokemon(m => m.DamageCounters > 0 && Fits(ctx, m)).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => Fits(ctx, m))) ctx.Heal(mon, 30);
                return false;
            }
        }

        /// <summary>
        /// Dream Ball: you can play it only when you take it as a face-down Prize card, before it goes into your hand. Search your
        /// deck for a Pokémon and put it onto your Bench. Your rulings (v58): any Pokémon (an Evolution Pokémon goes onto the Bench
        /// as it is), on either player's turn. Effects that stop you from playing Item cards still stop it.
        /// </summary>
        private sealed class DreamBall : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return false; }
            public override bool UsableWhenTakenAsPrize { get { return true; } }

            public override int OnTakenAsPrize(EffectContext ctx)
            {
                if (!ctx.Engine.CanPlayItemFromOutsideHand(ctx.Player, ctx.Card)) return -1;
                if (ctx.Me.Deck.Count == 0 || ctx.BenchSpace <= 0) return -1;
                if (!ctx.AskYesNo("Dream Ball: you took it as a Prize card. Play it now to put a Pokémon from your deck onto your Bench?")) return -1;
                ctx.Engine.PlayItemFromOutsideHand(ctx.Player, ctx.Card, "from " + ctx.Engine.Their(ctx.Player) + " Prize cards");
                return 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.BenchSpace <= 0 || ctx.Me.Deck.Count == 0) return;
                var found = ctx.SearchMyDeck(cd => cd.IsPokemon && !cd.IsLevelUp && !cd.IsBreak && !cd.IsLegend && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, cd), 1,
                    "Dream Ball: choose a Pokémon to put onto your Bench");
                foreach (var card in found) ctx.Engine.PutOntoBench(ctx.Player, card, false);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Elemental Badge: the attacks of the Pokémon V with Vaporeon, Jolteon or Flareon in its name holding it cost [C] less.</summary>
        private sealed class ElementalBadge : CardScript
        {
            private static bool Fits(PokemonInPlay mon)
            {
                return mon.Def.IsPokemonV && (mon.Name.Contains("Vaporeon") || mon.Name.Contains("Jolteon") || mon.Name.Contains("Flareon"));
            }

            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon != src.Pokemon || !Fits(mon)) return;
                cost.Remove(EnergyType.Colorless);
            }
        }

        /// <summary>Full Face Guard: if the Pokémon holding it has no Abilities, it takes 20 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class FullFaceGuard : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var holder = src.Pokemon;
                if (holder == null || info.Target != holder || !info.AgainstOpponent || src.Engine.HasAbility(holder)) return damage;
                return damage - 20;
            }
        }

        /// <summary>Lucky Ice Pop: heal 20 damage from your Active Pokémon; if you healed any, flip a coin; on heads this card goes back to your hand.</summary>
        private sealed class LuckyIcePop : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Active != null && ctx.Me.Active.DamageCounters > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                int healed = ctx.Heal(ctx.Me.Active, 20);
                if (healed <= 0 || !ctx.Flip()) return;
                ctx.CardPlacedElsewhere = true;
                ctx.Me.Hand.Add(ctx.Card);
                ctx.Log("Lucky Ice Pop went back into " + ctx.Engine.Their(ctx.Player) + " hand.");
            }
        }

        /// <summary>Moon &amp; Sun Badge: the Pokémon V with Espeon or Umbreon in its name holding it is unaffected by the opponent's Supporter cards.</summary>
        private sealed class MoonAndSunBadge : CardScript
        {
            public override bool PreventsTrainerEffects(EffectContext src, PokemonInPlay target, CardDef trainer)
            {
                var holder = src.Pokemon;
                if (holder == null || target != holder || trainer == null || !trainer.IsSupporter || !holder.Def.IsPokemonV) return false;
                return holder.Name.Contains("Espeon") || holder.Name.Contains("Umbreon");
            }
        }

        /// <summary>
        /// The Strike scrolls: a Single Strike (Rapid Strike) Pokémon holding it can use the attack on it. Superstrong Slash: discard
        /// all Energy from this Pokémon, 300 damage. Meteor: discard 2 Energy from this Pokémon, 90 damage to 1 of the opponent's Pokémon.
        /// </summary>
        private sealed class StrikeScroll : CardScript
        {
            private readonly bool single;

            public StrikeScroll(bool single) { this.single = single; }

            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacksFor(EffectContext src, PokemonInPlay mon)
            {
                if (src.Pokemon == null || mon != src.Pokemon || src.Card.Def.Attacks.Count == 0) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                bool fits = single ? mon.Def.IsSingleStrike : mon.Def.IsRapidStrike;
                if (!fits) return Enumerable.Empty<KeyValuePair<CardDef, int>>();
                return new[] { new KeyValuePair<CardDef, int>(src.Card.Def, 0) };
            }

            public override void ResolveAttack(EffectContext ctx, int attackIndex)
            {
                var me = ctx.Pokemon;
                if (me == null) return;
                var atk = ctx.Attack(attackIndex);
                if (single)
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(me)) ctx.Engine.DiscardAllEnergy(me);
                    return;
                }
                if (me.Energy.Count > 0) ctx.DiscardEnergyFrom(me, Math.Min(2, me.Energy.Count), atk.Name + ": discard 2 Energy from " + me.Name);
                ctx.DamageAnyOpponent(90);
            }
        }

        /// <summary>Ribbon Badge: if the Pokémon V with Sylveon in its name holding it is Knocked Out by damage from an opponent's attack, they take 1 fewer Prize card.</summary>
        private sealed class RibbonBadge : CardScript
        {
            public override int ModifyPrizes(EffectContext src, PokemonInPlay knockedOut, int taker, int prizes)
            {
                var holder = src.Pokemon;
                if (holder == null || knockedOut != holder || taker == src.Player || prizes <= 0) return prizes;
                if (!holder.Def.IsPokemonV || !holder.Name.Contains("Sylveon")) return prizes;
                if (!knockedOut.HitByAttack || knockedOut.ForcedKnockOut || knockedOut.HitByPlayer != taker) return prizes;
                src.Log("Ribbon Badge: " + src.Engine.PlayerName(taker) + " takes 1 fewer Prize card.");
                return prizes - 1;
            }
        }

        /// <summary>Shopping Center: once during each player's turn, that player may put a Pokémon Tool attached to 1 of their Pokémon into their hand.</summary>
        private sealed class ShoppingCenter : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.MyPokemon(m => m.Tools.Count > 0).Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                var holder = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Tools.Count > 0), "Shopping Center: put a Pokémon Tool from which of your Pokémon into your hand?");
                if (holder == null || holder.Tools.Count == 0) return false;
                var tool = holder.Tools.Count == 1 ? holder.Tools[0] : ctx.ChooseCards(holder.Tools.ToList(), 1, 1, "Shopping Center: which Pokémon Tool?")[0];
                if (!holder.Tools.Remove(tool)) return false;
                ctx.Me.Hand.Add(tool);
                ctx.Log(tool.Def.Name + " went from " + holder.Name + " into " + ctx.Engine.Their(ctx.Player) + " hand (Shopping Center).");
                return false;
            }
        }

        /// <summary>Snow Leaf Badge: the Pokémon V with Leafeon or Glaceon in its name holding it has no Retreat Cost and no Weakness.</summary>
        private sealed class SnowLeafBadge : CardScript
        {
            private static bool Fits(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && mon.Def.IsPokemonV && (mon.Name.Contains("Leafeon") || mon.Name.Contains("Glaceon"));
            }

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost) { return Fits(src, mon) ? 0 : cost; }
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon) { return Fits(src, mon); }
        }

        /// <summary>Spirit Mask: if the Pokémon holding it is Active and damaged by an opponent's attack (even if Knocked Out), the opponent discards a card from their hand.</summary>
        private sealed class SpiritMask : CardScript
        {
            public override void OnHolderDamagedByAttack(EffectContext ctx, PokemonInPlay attacker, bool holderWasActive, int damage)
            {
                if (!holderWasActive || attacker == null || attacker.Owner == ctx.Player || ctx.Opp.Hand.Count == 0) return;
                var picks = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Spirit Mask: discard a card from your hand", ctx.Source);
                foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Opponent, card);
            }
        }

        /// <summary>Stormy Mountains: once during each player's turn, that player may put a Basic Lightning or Basic Dragon Pokémon from their deck onto their Bench.</summary>
        private sealed class StormyMountains : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.BenchSpace > 0 && ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToBench(cd => cd.HasType(EnergyType.Lightning) || cd.HasType(EnergyType.Dragon), 1, "Stormy Mountains: choose a Basic Lightning or Basic Dragon Pokémon");
                return false;
            }
        }

        /// <summary>Switching Cups: switch a card from your hand with the top card of your deck.</summary>
        private sealed class SwitchingCups : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count >= 2 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0 || ctx.Me.Deck.Count == 0) return;
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Switching Cups: switch which card from your hand with the top card of your deck?")[0];
                var top = ctx.Me.Deck[0];
                ctx.Me.Hand.Remove(card);
                ctx.Me.Deck[0] = card;
                ctx.Me.Hand.Add(top);
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " switched a card from " + ctx.Engine.Their(ctx.Player) + " hand with the top card of " + ctx.Engine.Their(ctx.Player) + " deck.");
            }
        }

        /// <summary>Toy Catcher: switch 1 of the opponent's Benched Pokémon with 50 HP or less remaining with their Active Pokémon.</summary>
        private sealed class ToyCatcher : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Bench.Any(m => ctx.RemainingHp(m) <= 50); }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = ctx.GustTargets().Where(m => ctx.RemainingHp(m) <= 50).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Toy Catcher: switch in which of your opponent's Benched Pokémon (50 HP or less remaining)?");
                ctx.Engine.SwitchActive(ctx.Opponent, target);
            }
        }

        /// <summary>Turffield Stadium: once during each player's turn, that player may search their deck for an Evolution Grass Pokémon.</summary>
        private sealed class TurffieldStadium : StadiumAction
        {
            protected override bool Ready(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override bool Use(EffectContext ctx)
            {
                ctx.SearchToHand(cd => cd.IsEvolution && cd.HasType(EnergyType.Grass), 1, "Turffield Stadium: choose an Evolution Grass Pokémon");
                return false;
            }
        }

        /// <summary>Zinnia's Resolve: discard 2 other cards from your hand; draw a card for each of your opponent's Pokémon in play.</summary>
        private sealed class ZinniasResolve : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Hand.Count >= 3; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var hand = ctx.Me.Hand.ToList();
                if (hand.Count < 2) return;
                foreach (var card in ctx.ChooseCards(hand, 2, 2, "Zinnia's Resolve: discard 2 cards from your hand"))
                    ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(ctx.OpponentPokemon().Count);
            }
        }

        // ================================================================== Energy

        /// <summary>Treasure Energy: provides [C]. If you take it as a face-down Prize card during your turn, you may attach it to 1 of your Pokémon.</summary>
        private sealed class TreasureEnergy : CardScript
        {
            public override bool UsableWhenTakenAsPrize { get { return true; } }

            public override int OnTakenAsPrize(EffectContext ctx)
            {
                if (!ctx.IsMyTurn || ctx.MyPokemon().Count == 0) return -1;
                if (!ctx.AskYesNo("Treasure Energy: you took it as a Prize card. Attach it to 1 of your Pokémon?")) return -1;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Treasure Energy: attach it to which of your Pokémon?");
                ctx.Engine.AttachEnergyCard(ctx.Card, target);
                return 0;
            }
        }
    }
}
