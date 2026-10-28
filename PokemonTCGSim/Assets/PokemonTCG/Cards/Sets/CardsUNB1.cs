using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unbroken Bonds (UNB) part 1: Grass, Fire, Water and Lightning (UNB 1-60). No regulation mark: Expanded. Caterpie's Pupate
    /// and Metapod's Emerge evolve from the deck with no first-turn or same-turn limit (your ruling, v73). Charjabug's Battery puts
    /// the Charjabug card from the hand onto a Vikavolt as an Energy that provides 2 Lightning Energy.
    /// </summary>
    public static class CardsUNB1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNB-1", new PheromosaBuzzwoleGX());
            r.RegisterByKey("UNB-2", new FlipEvolveMon("Pupate", null));
            r.RegisterByKey("UNB-3", new FlipEvolveMon("Emerge", null));
            r.RegisterByKey("UNB-4", new TripleChargeMon());
            r.RegisterByKey("UNB-5", new ParalyzeFlipMon("Stun Spore"));
            r.RegisterByKey("UNB-7", new GloomUNB());
            r.RegisterByKey("UNB-8", new VileplumeUNB());
            r.RegisterByKey("UNB-9", new CardsSMP3.BrilliantDeductionMon("Radar Eyes", 7));
            r.RegisterByKey("UNB-10", new ConditionAttackMon("Psybeam", SpecialCondition.Confused));
            r.RegisterByKey("UNB-11", new VenomothUNB());
            r.RegisterByKey("UNB-12", new VenomothGX());
            r.RegisterByKey("UNB-14", new CardsCES1.ConditionsMon("Burning Venom", SpecialCondition.Burned, SpecialCondition.Poisoned));
            r.RegisterByKey("UNB-15", new VictreebelUNB());
            r.RegisterByKey("UNB-16", new CardsDAA1.GustAttackMon("Tangle Drag", false));
            r.RegisterByKey("UNB-17", new ComboMon(new NoRetreatMon("Bind Down"), new ChlorowhipMon()));
            r.RegisterByKey("UNB-18", new SearchAttackMon("Electrical Signal", d => d.IsPokemon && d.PokemonType == EnergyType.Lightning, 2));
            r.RegisterByKey("UNB-19", new KartanaUNB());
            r.RegisterByKey("UNB-20", new ReshiramCharizardGX());
            r.RegisterByKey("UNB-22", new ComboMon(new GrandFlameMon(), new RecoilMon("Heat Tackle", 50)));
            r.RegisterByKey("UNB-23", new CardsMEG.ChargeSelfMon("Flame Charge", EnergyType.Fire, 1));
            r.RegisterByKey("UNB-24", new ComboMon(new SearchAttackMon("Find Wildfire", IsFireEnergyCard, 3), new CardsASC.SelfDiscardTypeMon("Flare Blitz", EnergyType.Fire, 99)));
            r.RegisterByKey("UNB-25", new ComboMon(new FlareStarterMon(), new CardsMEG.EnergyInPlayBonusMon("High-Heat Blast", EnergyType.Fire, 4, 60)));
            r.RegisterByKey("UNB-26", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("UNB-27", new ComboMon(new CaturdayMon(1), new NoRetreatMon("Big Bite")));
            r.RegisterByKey("UNB-28", new ConditionAttackMon("Fire Fang", SpecialCondition.Burned));
            r.RegisterByKey("UNB-29", new IncineroarUNB());
            r.RegisterByKey("UNB-30", new CardsFST1.DiscardDefendingTypeMon("Grass Fire", EnergyType.Grass));
            r.RegisterByKey("UNB-31", new SalazzleUNB());
            r.RegisterByKey("UNB-32", new ComboMon(new BlazerMon(), new CardsFST1.DiscardFromHandTimesMon("Fireball Circus", IsFireEnergyCard, 99, "Fire Energy cards")));
            r.RegisterByKey("UNB-33", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("UNB-34", new CardsFST1.TakeLessMon(20, new LockAfterAttackMon("Aqua Slash", false)));
            r.RegisterByKey("UNB-35", new CardsFST1.TakeLessMon(30, new BlastoiseGXAttacks()));
            r.RegisterByKey("UNB-36", new PoliwagUNB());
            r.RegisterByKey("UNB-37", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("UNB-38", new ParalyzeFlipMon("Bubble"));
            r.RegisterByKey("UNB-39", new CardsFST2.ConditionBonusMon("Swirly Rush", 90, PoliFamilyOnBench));
            r.RegisterByKey("UNB-40", new CardsBRS1.SupporterBonusMon("Bubble Jutsu", "Janine", 50));
            r.RegisterByKey("UNB-41", new ComboMon(new WickedTentaclesMon(), new ParalyzeFlipMon("Wrap")));
            r.RegisterByKey("UNB-42", new CardsMEG.WeakenDefendingMon("Growl", 20));
            r.RegisterByKey("UNB-43", new ComboMon(new ConditionAttackMon("Yawn", SpecialCondition.Asleep), new ThreeStrikesMon()));
            r.RegisterByKey("UNB-45", new DualBlizzardMon());
            r.RegisterByKey("UNB-47", new ParalyzeFlipMon("Bubble Beam"));
            r.RegisterByKey("UNB-48", new DodgeMon("Elegant Swim", false));
            r.RegisterByKey("UNB-49", new EnhancedHornMon());
            r.RegisterByKey("UNB-50", new ComboMon(new CardsMEG.ChargeSelfMon("Call Forth Cold", EnergyType.Water, 1), new HailPrisonMon()));
            r.RegisterByKey("UNB-51", new FlipBonusMon("Quick Attack", 20));
            r.RegisterByKey("UNB-52", new AfterimageStrikeMon());
            r.RegisterByKey("UNB-53", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new SurpriseFistMon()));
            r.RegisterByKey("UNB-55", new NeverGiveUpMon());
            r.RegisterByKey("UNB-56", new ComboMon(new CardsASR1.BenchRecoilMon("Raging Thunder", 10),
                new CardsASR1.TimesMon("Electric Trap", c => c.MyPokemon(m => m.DamageCounters > 0).Count)));
            r.RegisterByKey("UNB-57", new DedenneGX());
            r.RegisterByKey("UNB-58", new CharjabugUNB());
            r.RegisterByKey("UNB-59", new VikavoltUNB());
            r.RegisterByKey("UNB-60", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Crushing Claw", false), new DischargeMon()));
        }

        // ================================================================== Shared helpers

        /// <summary>A basic Fire Energy card ("Fire Energy card" in Sun &amp; Moon wording).</summary>
        internal static bool IsFireEnergyCard(CardDef d) { return d.IsBasicEnergy && d.EnergyProvides == EnergyType.Fire; }

        internal static bool IsBasicEnergyOf(CardDef d, EnergyType type) { return d.IsBasicEnergy && d.EnergyProvides == type; }

        private static bool PoliFamilyOnBench(EffectContext ctx)
        {
            return ctx.Me.Bench.Any(m => m.Name == "Poliwag") && ctx.Me.Bench.Any(m => m.Name == "Poliwhirl");
        }

        /// <summary>"If you go second and it's your first turn" (Flare Starter, Energy Spinner).</summary>
        internal static bool SecondPlayersFirstTurn(EffectContext ctx) { return ctx.IsMyFirstTurn && ctx.Player != ctx.State.FirstPlayer; }

        /// <summary>
        /// Attaches cards just found in the deck to your Pokémon in any way you like (each to a Pokémon the filter allows). Any card
        /// that couldn't be attached goes back into the deck; the caller shuffles.
        /// </summary>
        internal static void AttachFoundAnyWay(EffectContext ctx, List<CardInstance> found, string what, Func<PokemonInPlay, bool> allowed)
        {
            if (found.Count == 0) return;
            var pile = new List<CardInstance>(found);
            ctx.AttachCardsAnyWay(pile, pile.Count, (c, p) => allowed(p), what, false);
            foreach (var card in found)
                if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card))) ctx.Me.Deck.Add(card);
        }

        /// <summary>"Take N more Prize cards" for a Knock Out that meets a condition (Beast Game-GX, Beast Bringer).</summary>
        internal sealed class MorePrizesEffect : ContinuousEffect
        {
            private readonly int amount;
            private readonly Func<GameEngine, PokemonInPlay, bool> condition;

            public MorePrizesEffect(string name, int owner, int expiresAfterTurn, int amount, Func<GameEngine, PokemonInPlay, bool> condition)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.amount = amount;
                this.condition = condition;
            }

            public override int ModifyPrizes(GameEngine engine, PokemonInPlay knockedOut, int taker, int prizes)
            {
                return taker == Owner && knockedOut.Owner != Owner && condition(engine, knockedOut) ? prizes + amount : prizes;
            }
        }

        /// <summary>The Pokémon has no Abilities while this lasts (Gastro Acid). It ends if the Pokémon leaves the Active Spot.</summary>
        internal sealed class NoAbilityEffect : ContinuousEffect
        {
            public NoAbilityEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool BlocksAbility(GameEngine engine, PokemonInPlay mon, string abilityName, int depth)
            {
                return mon != null && mon.Uid == TargetUid;
            }
        }

        /// <summary>Caturday: draw N cards; if you do, this Pokémon is now Asleep.</summary>
        internal sealed class CaturdayMon : Mon
        {
            private readonly int draw;

            public CaturdayMon(int draw) { this.draw = draw; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Caturday") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Draw(draw) > 0 && ctx.InPlay(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Asleep);
                return true;
            }
        }

        /// <summary>
        /// Pupate / Emerge: once during your turn, flip a coin; on heads, search your deck for a card that evolves from this Pokémon
        /// and evolve it. Your ruling (v73): no first-turn or same-turn limit, like the other evolve-from-deck effects.
        /// </summary>
        internal sealed class FlipEvolveMon : Mon
        {
            private readonly string ability;
            private readonly Mon attacks;

            public FlipEvolveMon(string ability, Mon attacks)
            {
                this.ability = ability;
                this.attacks = attacks;
            }

            protected override string Activated { get { return ability; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Pokemon != null && ctx.Me.Deck.Count > 0 && !CardsEVS1.EvolveBlocked(ctx, ctx.Pokemon);
            }

            protected override void Ability(EffectContext ctx)
            {
                var me = ctx.Pokemon;
                if (!ctx.Flip())
                {
                    ctx.Log(ability + ": tails, so " + me.Name + " doesn't evolve.");
                    return;
                }
                CardsEVS1.EvolveFromDeck(ctx, me, ability);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        // ================================================================== Grass

        /// <summary>
        /// Jet Punch: also 30 to 1 Benched Pokémon. Elegant Sole: 190, but its base damage is 60 during your next turn. Beast Game-GX:
        /// take 1 more Prize card for a Knock Out by its damage (3 more with 7 extra Energy attached).
        /// </summary>
        private sealed class PheromosaBuzzwoleGX : Mon
        {
            private static readonly Mon JetPunch = new Cards30CC.BenchHitMon("Jet Punch", 30);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Elegant Sole")
                {
                    var me = ctx.Pokemon;
                    bool tired = me.LastAttackName == "Elegant Sole" && me.LastAttackTurn == ctx.MyLastTurn && me.MovedToActiveOnTurn <= me.LastAttackTurn;
                    ctx.DamageDefending(tired ? 60 : atk.Damage);
                    return true;
                }
                if (atk.Name == "Beast Game-GX")
                {
                    int owner = ctx.Player;
                    int more = ExtraEnergy(ctx, atk, 7) ? 3 : 1;
                    ctx.AddEffect(new MorePrizesEffect("Beast Game-GX: take " + more + " more Prize card" + (more == 1 ? "" : "s") + " for a Knock Out by this attack's damage",
                        owner, ctx.Turn, more, (eng, ko) => ko.HitByAttack && ko.HitByPlayer == owner && !ko.ForcedKnockOut));
                    ctx.DamageDefending(atk.Damage);
                    return true;
                }
                return JetPunch.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Triple Charge: search your deck for up to 3 basic Energy cards and attach them to your Pokémon in any way you like.</summary>
        private sealed class TripleChargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Triple Charge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var found = ctx.SearchMyDeck(d => d.IsBasicEnergy, 3, "Triple Charge: search your deck for up to 3 basic Energy cards");
                AttachFoundAnyWay(ctx, found, "Triple Charge", m => true);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Irresistible Aroma: if the opponent's Bench isn't full, flip; on heads they reveal their hand and you put a Basic Pokémon from it onto their Bench.</summary>
        private sealed class GloomUNB : Mon
        {
            protected override string Activated { get { return "Irresistible Aroma"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Engine.BenchSpace(ctx.Opponent) > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip())
                {
                    ctx.Log("Irresistible Aroma: tails.");
                    return;
                }
                TefPokemon.RevealOpponentHand(ctx);
                var basics = ctx.Opp.Hand.Where(c => c.Def.IsBasicPokemon && !ctx.Engine.PutIntoPlayBlocked(ctx.Opponent, c.Def)).ToList();
                if (basics.Count == 0 || ctx.Engine.BenchSpace(ctx.Opponent) <= 0) return;
                var pick = ctx.ChooseCards(basics, 1, 1, "Irresistible Aroma: put which Basic Pokémon onto your opponent's Bench?")[0];
                if (ctx.Opp.Hand.Remove(pick)) ctx.Engine.PutOntoBench(ctx.Opponent, pick, false);
            }
        }

        /// <summary>Varied Pollen: flip; on heads the opponent's Active Pokémon is Asleep, Burned, Confused or Poisoned (you choose). Giant Bloom: heal 30.</summary>
        private sealed class VileplumeUNB : Mon
        {
            private static readonly Mon Bloom = new CardsTWM1.HealSelfMon("Giant Bloom", 30);
            private static readonly SpecialCondition[] Pollen = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned };

            protected override string Activated { get { return "Varied Pollen"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (!ctx.Flip())
                {
                    ctx.Log("Varied Pollen: tails.");
                    return;
                }
                var target = ctx.Opp.Active;
                if (target == null) return;
                int pick = ctx.ChooseOption("Varied Pollen: which Special Condition for your opponent's Active Pokémon?", "Asleep", "Burned", "Confused", "Poisoned");
                ctx.ApplyCondition(target, Pollen[pick]);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bloom.HandleAttack(ctx, atk); }
        }

        /// <summary>Assassin Flight: only if the opponent's Active Pokémon has a Special Condition; 90 to 1 Benched Pokémon. Poison Powder: Poisoned.</summary>
        private sealed class VenomothUNB : Mon
        {
            private static readonly Mon Powder = new ConditionAttackMon("Poison Powder", SpecialCondition.Poisoned);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Assassin Flight") return true;
                var target = ctx.Opp.Active;
                return target != null && target.Conditions != SpecialCondition.None;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Assassin Flight")
                {
                    if (ctx.Opp.Bench.Count > 0) ctx.DamageOneBenched(90);
                    return true;
                }
                return Powder.HandleAttack(ctx, atk);
            }
        }

        /// <summary>
        /// Shinobi Mastery: 90 more if you played Koga's Trap from your hand this turn; if you played Janine, prevent all damage done to
        /// this Pokémon by attacks from Basic Pokémon during the opponent's next turn. Ten-Card Return-GX: shuffle your hand into your
        /// deck, then draw 10 cards.
        /// </summary>
        private sealed class VenomothGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Shinobi Mastery")
                {
                    bool koga = ctx.Me.PlayedSupporterThisTurn("Koga's Trap");
                    bool janine = ctx.Me.PlayedSupporterThisTurn("Janine");
                    ctx.DamageDefending(atk.Damage + (koga ? 90 : 0));
                    var me = ctx.Pokemon;
                    if (janine && ctx.InPlay(me))
                        ctx.AddEffect(new CardsMEG.BasicShieldEffect(me.Name + " takes no damage from Basic Pokémon's attacks (Shinobi Mastery)", ctx.Player, me.Uid, ctx.Turn + 1));
                    return true;
                }
                if (atk.Name == "Ten-Card Return-GX")
                {
                    ctx.DamageDefending(atk.Damage);
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(10);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Reactive Poison: 60 more for each Special Condition on the opponent's Active Pokémon. Gastro Acid: it has no Abilities until the end of your next turn.</summary>
        private sealed class VictreebelUNB : Mon
        {
            private static readonly SpecialCondition[] All = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Reactive Poison")
                {
                    var target = ctx.Defending;
                    int n = target == null ? 0 : All.Count(sc => target.Has(sc));
                    ctx.DamageDefending(atk.Damage + 60 * n);
                    return true;
                }
                if (atk.Name == "Gastro Acid")
                {
                    var defending = ctx.Defending;
                    ctx.DamageDefending(atk.Damage);
                    if (TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                        ctx.AddEffect(new NoAbilityEffect(defending.Name + " has no Abilities until the end of " + ctx.Engine.PlayerName(ctx.Player) + "'s next turn (Gastro Acid)",
                            ctx.Player, defending.Uid, ctx.Turn + 2));
                    return true;
                }
                return false;
            }
        }

        /// <summary>Chlorowhip: if this Pokémon has at least 2 Grass Energy attached, heal 60 damage from it.</summary>
        private sealed class ChlorowhipMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chlorowhip") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me) && ctx.Engine.EnergyCountOfType(me, EnergyType.Grass) >= 2) ctx.Heal(me, 60);
                return true;
            }
        }

        /// <summary>Big Cut: 120 more with exactly 4 Prize cards left. False Swipe: flip; on heads, damage counters until the Defending Pokémon has 10 HP left.</summary>
        private sealed class KartanaUNB : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Big Cut")
                {
                    ctx.DamageDefending(atk.Damage + (ctx.Me.Prizes.Count == 4 ? 120 : 0));
                    return true;
                }
                if (atk.Name == "False Swipe")
                {
                    var defending = ctx.Defending;
                    if (defending == null) return true;
                    if (!ctx.Flip()) return Nothing(ctx);
                    int counters = (ctx.RemainingHp(defending) - 10) / 10;
                    if (counters > 0) ctx.PlaceCounters(defending, counters);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>
        /// Outrage: 10 more for each damage counter on it. Flare Strike: it can't use Flare Strike next turn. Double Blaze-GX: with 3
        /// extra Fire Energy (6 Fire in all), 100 more damage that isn't affected by any effects on the opponent's Active Pokémon.
        /// </summary>
        private sealed class ReshiramCharizardGX : Mon
        {
            private static readonly Mon Outrage = new Cards30C.SelfCounterBonusMon("Outrage", 10);
            private static readonly Mon Strike = new LockAfterAttackMon("Flare Strike", true);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Double Blaze-GX")
                {
                    bool extra = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fire) >= 6;
                    if (!extra)
                    {
                        ctx.DamageDefending(atk.Damage);
                        return true;
                    }
                    if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage + 100, true, true, true);
                    return true;
                }
                return Outrage.HandleAttack(ctx, atk) || Strike.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Grand Flame: attach 2 Fire Energy cards from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class GrandFlameMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Grand Flame") return false;
                ctx.DamageDefending(atk.Damage);
                var fire = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Fire);
                var bench = ctx.MyBench();
                if (fire.Count == 0 || bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Grand Flame: attach Fire Energy cards from your discard pile to which Benched Pokémon?");
                if (target == null) return true;
                var picks = fire.Count <= 2 ? fire : ctx.ChooseCards(fire, 2, 2, "Grand Flame: attach which 2 Fire Energy cards?");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>Flare Starter: search for a Fire Energy card and attach it to 1 of your Pokémon; going second, on your first turn, up to 3 in any way you like.</summary>
        private sealed class FlareStarterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flare Starter") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                bool boosted = SecondPlayersFirstTurn(ctx);
                var found = ctx.SearchMyDeck(IsFireEnergyCard, boosted ? 3 : 1,
                    "Flare Starter: search your deck for " + (boosted ? "up to 3 Fire Energy cards" : "a Fire Energy card"));
                AttachFoundAnyWay(ctx, found, "Flare Starter", m => true);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Strong Cheer: your Pokémon's attacks do 30 more damage to the opponent's Active Pokémon; only 1 Strong Cheer applies. Flamethrower: discard an Energy.</summary>
        private sealed class IncineroarUNB : Mon
        {
            private static readonly Mon Flamethrower = new CardsTWM1.SelfDiscardMon("Flamethrower", 1);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var attacker = info.Attacker;
                if (src.Pokemon == null || attacker == null || attacker.Owner != src.Player || !info.ToOpponentsActive || damage <= 0) return damage;
                return info.ApplyOnce("Strong Cheer") ? damage + 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flamethrower.HandleAttack(ctx, atk); }
        }

        /// <summary>Roast Reveal: once during your turn, discard a Fire Energy card from your hand to draw 3 cards.</summary>
        private sealed class SalazzleUNB : Mon
        {
            protected override string Activated { get { return "Roast Reveal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => IsFireEnergyCard(c.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(c => IsFireEnergyCard(c.Def)).ToList();
                if (fire.Count == 0) return;
                var pick = ctx.ChooseCards(fire, 1, 1, "Roast Reveal: discard which Fire Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Draw(3);
            }
        }

        /// <summary>Blazer: turn 1 of your face-down Prize cards face up (it stays face up); 50 more damage if it's a Fire Energy card.</summary>
        private sealed class BlazerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Blazer") return false;
                var me = ctx.Me;
                bool fire = false;
                if (me.Prizes.Any(c => !me.FaceUpPrizeIds.Contains(c.Id)))
                {
                    var slots = me.PrizeSlotList();
                    var opts = new List<DecisionOption>();
                    for (int k = 0; k < me.Prizes.Count; k++)
                    {
                        if (me.FaceUpPrizeIds.Contains(me.Prizes[k].Id)) continue;
                        opts.Add(new DecisionOption { Label = "Prize card " + (slots[k] + 1), Index = k });
                    }
                    int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Blazer: turn which of your face-down Prize cards face up?", opts, ctx.Source);
                    var card = me.Prizes[opts[pick].Index];
                    me.FaceUpPrizeIds.Add(card.Id);
                    ctx.Engine.Reveal(ctx.Player, new List<CardInstance> { card }, "turned a Prize card face up (Blazer)");
                    fire = IsFireEnergyCard(card.Def);
                }
                ctx.DamageDefending(atk.Damage + (fire ? 50 : 0));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Rocket Splash: shuffle any amount of Water Energy from your Pokémon into your deck; 60 for each card shuffled. Giant
        /// Geyser-GX: attach any number of Water Energy cards from your hand to your Pokémon in any way you like.
        /// </summary>
        private sealed class BlastoiseGXAttacks : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Rocket Splash")
                {
                    var holderOf = new Dictionary<CardInstance, PokemonInPlay>();
                    foreach (var mon in ctx.MyPokemon())
                        foreach (var energy in mon.Energy)
                            if (CardsHIF1.Provides(ctx, mon, energy, EnergyType.Water)) holderOf[energy] = mon;
                    int n = 0;
                    if (holderOf.Count > 0)
                    {
                        var picks = ctx.ChooseCards(holderOf.Keys.ToList(), 0, holderOf.Count, "Rocket Splash: shuffle any amount of Water Energy from your Pokémon into your deck (60 damage each)");
                        foreach (var card in picks)
                            n += ctx.Engine.ShuffleEnergyIntoDeck(holderOf[card], new List<CardInstance> { card });
                    }
                    ctx.DamageDefending(atk.Damage * n);
                    return true;
                }
                if (atk.Name == "Giant Geyser-GX")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    var water = ctx.Me.Hand.Where(c => IsBasicEnergyOf(c.Def, EnergyType.Water)).ToList();
                    if (water.Count > 0) ctx.AttachCardsAnyWay(water, water.Count, (c, p) => true, "Giant Geyser-GX");
                    return true;
                }
                return false;
            }
        }

        /// <summary>Round 'n' Round: if you went second, once during your first turn, make the opponent's Active Pokémon Confused.</summary>
        private sealed class PoliwagUNB : Mon
        {
            protected override string Activated { get { return "Round 'n' Round"; } }

            protected override bool AbilityReady(EffectContext ctx) { return SecondPlayersFirstTurn(ctx) && ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Opp.Active != null) ctx.ApplyCondition(ctx.Opp.Active, SpecialCondition.Confused);
            }
        }

        /// <summary>Wicked Tentacles: move an Energy from 1 of the opponent's Pokémon to another; put 3 damage counters on the one that got it.</summary>
        private sealed class WickedTentaclesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wicked Tentacles") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.OpponentPokemon().Count < 2) return true;
                var givers = ctx.OpponentPokemon(m => m.Energy.Count > 0 && ctx.CanAffect(m));
                if (givers.Count == 0) return true;
                var from = ctx.ChoosePokemon(givers, "Wicked Tentacles: move an Energy from which of your opponent's Pokémon?");
                if (from == null) return true;
                var energy = ctx.ChooseCards(from.Energy.ToList(), 1, 1, "Wicked Tentacles: move which Energy?")[0];
                var receivers = ctx.OpponentPokemon(m => m != from && ctx.CanAffect(m));
                if (receivers.Count == 0) return true;
                var to = ctx.ChoosePokemon(receivers, "Wicked Tentacles: move " + energy.Def.Name + " to which of your opponent's Pokémon?");
                if (to != null && ctx.Engine.MoveEnergy(from, to, energy)) ctx.PlaceCounters(to, 3);
                return true;
            }
        }

        /// <summary>Three Strikes: flip 3 coins, 100 for each heads; if all 3 are tails, you lose the game.</summary>
        private sealed class ThreeStrikesMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Three Strikes") return false;
                int heads = ctx.FlipCoins(3);
                if (heads == 0)
                {
                    ctx.Engine.EndGame(ctx.Opponent, ctx.Engine.PlayerName(ctx.Player) + " flipped 3 tails with Three Strikes and lost the game.");
                    return true;
                }
                ctx.DamageDefending(atk.Damage * heads);
                return true;
            }
        }

        /// <summary>Dual Blizzard: discard 2 Energy from this Pokémon; 60 damage to 2 of the opponent's Pokémon.</summary>
        private sealed class DualBlizzardMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Blizzard") return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, 2, "Dual Blizzard: discard 2 Energy from " + me.Name);
                var targets = ctx.OpponentPokemon();
                if (targets.Count == 0) return true;
                var picks = targets.Count <= 2 ? targets : ctx.ChoosePokemonMany(targets, 2, 2, "Dual Blizzard: choose 2 of your opponent's Pokémon (60 damage each)");
                foreach (var target in picks) ctx.DamageTarget(target, 60);
                return true;
            }
        }

        /// <summary>Enhanced Horn: flip 2 coins (6 if this Pokémon has a Tool attached); 30 for each heads.</summary>
        private sealed class EnhancedHornMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Enhanced Horn") return false;
                int coins = ctx.Pokemon.Tools.Count > 0 ? 6 : 2;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(coins));
                return true;
            }
        }

        /// <summary>Hail Prison: discard 2 Water Energy from this Pokémon; the opponent's Active Pokémon is now Paralyzed.</summary>
        private sealed class HailPrisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hail Prison") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.DiscardEnergyFrom(me, 2, "Hail Prison: discard 2 Water Energy from " + me.Name, e => CardsHIF1.Provides(ctx, me, e, EnergyType.Water));
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                return true;
            }
        }

        /// <summary>Afterimage Strike: during the opponent's next turn, if this Pokémon would be damaged by an attack, flip; on heads, prevent that damage.</summary>
        internal sealed class AfterimageStrikeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Afterimage Strike") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new AfterimageEffect(me.Name + " may dodge attack damage (Afterimage Strike)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class AfterimageEffect : ContinuousEffect
        {
            public AfterimageEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override bool PreventsFinalDamage(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return false;
                return engine.FlipCoin(Owner, "Afterimage Strike");
            }
        }

        /// <summary>Surprise Fist: you and the opponent play Rock-Paper-Scissors; 60 more damage if you win.</summary>
        private sealed class SurpriseFistMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Surprise Fist") return false;
                bool won = CardsLOR1.PlayRockPaperScissors(ctx);
                ctx.DamageDefending(atk.Damage + (won ? 60 : 0));
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Never Give Up: only with at least 3 more Prize cards left than the opponent; attach all Lightning Energy cards from your discard pile to your Pokémon in any way you like.</summary>
        private sealed class NeverGiveUpMon : Mon
        {
            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Never Give Up") return true;
                return ctx.Me.Prizes.Count >= ctx.Opp.Prizes.Count + 3;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Never Give Up") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var pile = ctx.Me.Discard.Where(c => IsBasicEnergyOf(c.Def, EnergyType.Lightning)).ToList();
                if (pile.Count > 0) ctx.AttachCardsAnyWay(pile, pile.Count, (card, mon) => true, "Never Give Up", false);
                return true;
            }
        }

        /// <summary>
        /// Dedechange: when you play it from your hand onto your Bench, you may discard your hand and draw 6 cards (only 1 Dedechange
        /// each turn). Tingly Return-GX: the opponent's Active Pokémon is Paralyzed; put this Pokémon and all its cards into your hand.
        /// </summary>
        private sealed class DedenneGX : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (!ctx.IsMyTurn || ctx.NamedLimitUsedThisTurn("Dedechange")) return;
                if (!ctx.AskYesNo("Dedechange: discard your hand and draw 6 cards?")) return;
                ctx.MarkNamedLimit("Dedechange");
                Announce(ctx, "Dedechange");
                foreach (var card in ctx.Me.Hand.ToList()) ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.Draw(6);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tingly Return-GX") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ReturnPokemonToHand(ctx.Pokemon);
                return true;
            }
        }

        /// <summary>
        /// Battery: during your turn, attach this Charjabug from your hand to 1 of your Vikavolt or Vikavolt-GX as a Special Energy
        /// card; it provides 2 Lightning Energy while attached. Once attached it is discarded like any Energy and goes back to being a
        /// Pokémon card. Effects that pick out Special Energy cards by their card type don't see it.
        /// </summary>
        private sealed class CharjabugUNB : Mon
        {
            private static List<PokemonInPlay> Vikavolts(EffectContext ctx)
            {
                return ctx.MyPokemon(m => (m.Name == "Vikavolt" || m.Name == "Vikavolt-GX") && ctx.Engine.CanAttachFromHand(m));
            }

            public override bool CanUseFromHand(EffectContext ctx)
            {
                return ctx.IsMyTurn && ctx.Card != null && ctx.Me.Hand.Contains(ctx.Card) && Vikavolts(ctx).Count > 0
                       && !ctx.Engine.EnergyCardBlockedBySource(ctx.Player, ctx.Card);
            }

            public override void UseFromHand(EffectContext ctx)
            {
                if (!ctx.Me.Hand.Contains(ctx.Card)) return;
                var targets = Vikavolts(ctx);
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Battery: attach this Charjabug to which Vikavolt as a Special Energy card?");
                if (target == null || !ctx.Me.Hand.Remove(ctx.Card)) return;
                ctx.Engine.Emit(GameEventType.UseAbility, ctx.Player, ctx.Engine.PlayerName(ctx.Player) + " used Battery: Charjabug from the hand becomes a Special Energy card.", ctx.Card.Id, target.Uid);
                ctx.Engine.AttachEnergyCard(ctx.Card, target, true);
            }

            public override List<int> ProvidedEnergy(EffectContext ctx)
            {
                return new List<int> { EnergyMask.Of(EnergyType.Lightning), EnergyMask.Of(EnergyType.Lightning) };
            }
        }

        /// <summary>Stealthy Body: no Weakness while any Stadium is in play. Electricannon: you may discard all Lightning Energy from it for 100 more damage.</summary>
        private sealed class VikavoltUNB : Mon
        {
            public override bool RemovesWeakness(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.State.Stadium != null;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electricannon") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(x => CardsHIF1.Provides(ctx, me, x, EnergyType.Lightning)).ToList();
                bool boost = false;
                if (lightning.Count > 0 && ctx.AskYesNo("Electricannon: discard all Lightning Energy from " + me.Name + " for 100 more damage?"))
                {
                    foreach (var card in lightning) ctx.Engine.DiscardEnergy(me, card);
                    boost = true;
                }
                ctx.DamageDefending(atk.Damage + (boost ? 100 : 0));
                return true;
            }
        }

        /// <summary>Discharge: discard all Lightning Energy from this Pokémon; 50 for each card discarded.</summary>
        internal sealed class DischargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Discharge") return false;
                var me = ctx.Pokemon;
                var lightning = me.Energy.Where(x => CardsHIF1.Provides(ctx, me, x, EnergyType.Lightning)).ToList();
                int n = 0;
                foreach (var card in lightning) if (ctx.Engine.DiscardEnergy(me, card)) n++;
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }
    }
}
