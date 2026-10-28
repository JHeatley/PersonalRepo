using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Cosmic Eclipse (CEC 1-271), part 1: Grass, Fire and Water (CEC 1-65), plus the Sun &amp; Moon helpers the other parts share
    /// (TAG TEAM Supporter bonuses, typed extra Energy, the Eevee "Cheer" Abilities). No regulation mark: Expanded.
    /// Rulings used: Shining Vine's erratum (not once per turn; Compendium, 2024-11-07); "a [T] Energy card" outside play is basic
    /// Energy only (your ruling, v65); Crimson Flame Pillar-GX checks for the extra Energy before it attaches anything (your ruling,
    /// v68); Brilliant Flare's search is optional, but a search takes at least 1 card (Compendium, 2019-10-31).
    /// </summary>
    public static class CardsCEC1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Grass
            r.RegisterByKey("CEC-1", new VenusaurSnivyGX());
            r.RegisterByKey("CEC-2", new CardsPOR.SweetScentMon("Sweet Scent", 30));
            r.RegisterByKey("CEC-3", new ConditionAttackMon("Sleep Powder", SpecialCondition.Asleep));
            r.RegisterByKey("CEC-4", new VileplumeGX());
            r.RegisterByKey("CEC-5", new ToxicCECMon("Toxic"));
            r.RegisterByKey("CEC-6", new ComboMon(new CardsOBF1.RetreatCostDamageMon("Grass Knot", 30, false), new CoinsMon("Slam", 2)));
            r.RegisterByKey("CEC-7", new CardsTWM1.HealSelfMon("Leech Seed", 10));
            r.RegisterByKey("CEC-8", new SunfloraCEC());
            r.RegisterByKey("CEC-9", new CardsFST2.ConditionBonusMon("Riled Horn", 70, OpponentHasTagTeam));
            r.RegisterByKey("CEC-10", new ConditionAttackMon("Confuse Ray", SpecialCondition.Confused));
            r.RegisterByKey("CEC-11", new CradilyCEC());
            r.RegisterByKey("CEC-12", new ComboMon(new CardsVIV1.SynthesisMon(), new CardsTWM1.HealSelfMon("Leaf Drain", 30)));
            r.RegisterByKey("CEC-13", new DrawAttackMon("Collect", 1));
            r.RegisterByKey("CEC-14", new KricketuneCEC());
            r.RegisterByKey("CEC-16", new SawsbuckCEC());
            r.RegisterByKey("CEC-17", new DodgeMon("Hide and Seek", false));
            r.RegisterByKey("CEC-18", new CardsMEG.SnipeMon("Skill Dive", 10));
            r.RegisterByKey("CEC-19", new CardsDRI1.WhirlwindMon("Wing Flick"));
            r.RegisterByKey("CEC-20", new ComboMon(new CardsMEG.SnipeMon("Skill Dive", 40), new TrackingShotMon()));
            r.RegisterByKey("CEC-21", new BuzzwoleCEC());
            // Fire
            r.RegisterByKey("CEC-22", new CharizardBraixenGX());
            r.RegisterByKey("CEC-23", new SearchAttackMon("Minor Errand-Running", IsBasicEnergyCard, 2));
            r.RegisterByKey("CEC-24", new Cards30CC.BenchHitMon("Overrun", 30));
            r.RegisterByKey("CEC-25", new FlareonCEC());
            r.RegisterByKey("CEC-26", new ConditionAttackMon("Singe", SpecialCondition.Burned));
            r.RegisterByKey("CEC-27", new ComboMon(new CardsSSP2.StompOffMon("Stomp Off", 2), new CardsTWM1.SelfDiscardMon("Flamethrower", 1)));
            r.RegisterByKey("CEC-28", new CardsFST2.ConditionBonusMon("Rally Back", 90, KnockedOutLastTurn));
            r.RegisterByKey("CEC-29", new ComboMon(new FireFlingMon(), new CardsSP1.SelfThenDefendingDiscardMon("Kindle")));
            r.RegisterByKey("CEC-30", new VictiniCEC());
            r.RegisterByKey("CEC-33", new EmboarCEC());
            r.RegisterByKey("CEC-35", new VolcaronaGX());
            r.RegisterByKey("CEC-37", new SwirlingInfernoMon());
            // Water
            r.RegisterByKey("CEC-38", new BlastoisePiplupGX());
            r.RegisterByKey("CEC-39", new CardsDRM1.SubmergeMon());
            r.RegisterByKey("CEC-40", new ConfusionWaveMon("Confusion Wave"));
            r.RegisterByKey("CEC-41", new CardsDRI1.EnergyLoopMon("Energy Loop"));
            r.RegisterByKey("CEC-42", new VaporeonCEC());
            r.RegisterByKey("CEC-43", new DodgeMon("Agility", false));
            r.RegisterByKey("CEC-44", new SearchAttackMon("Nasty Plot", AnyCard, 2));
            r.RegisterByKey("CEC-47", new CardsPFL.FlipUntilTailsTimesMon("Continuous Headbutt"));
            r.RegisterByKey("CEC-48", new ComboMon(new IceFangMon(), new LockAfterAttackMon("Frosty Typhoon", true)));
            r.RegisterByKey("CEC-50", new ParalyzeFlipMon("Body Slam"));
            r.RegisterByKey("CEC-52", new WalreinCEC());
            r.RegisterByKey("CEC-53", new ComboMon(new HighWaterMon(), new CardsTWM1.SelfDiscardMon("Swirling Waves", 1)));
            r.RegisterByKey("CEC-54", new CardsEVS1.DefenderCantAttackMon("Bubble Hold", IsBasicPokemonCard));
            r.RegisterByKey("CEC-55", new DirectDiveMon());
            r.RegisterByKey("CEC-56", new EmpoleonCEC());
            r.RegisterByKey("CEC-57", new PhioneCEC());
            r.RegisterByKey("CEC-58", new CoinsMon("Flail Around", 3));
            r.RegisterByKey("CEC-59", new FlipBonusMon("Aerial Ace", 30));
            r.RegisterByKey("CEC-60", new ComboMon(new TailwindMon(), new CardsTWM1.SelfDiscardMon("Air Slash", 1)));
            r.RegisterByKey("CEC-61", new ComboMon(new CardsASR1.DiscardDefendingSpecialMon("Frozen Wings", false), new CardsFST2.ConditionBonusMon("Dazzling Blizzard", 100, HaveStadiumInPlay)));
            r.RegisterByKey("CEC-62", new WishiwashiCEC());
            r.RegisterByKey("CEC-63", new ComboMon(new CardsASR1.TimesMon("School Storm", Wishiwashis), new MassiveCatchMon()));
            r.RegisterByKey("CEC-65", new LiquidationMon());
        }

        // ================================================================== Shared helpers (Cosmic Eclipse)

        internal static bool AnyCard(CardDef d) { return true; }
        internal static bool IsBasicEnergyCard(CardDef d) { return d.IsBasicEnergy; }
        internal static bool IsBasicPokemonCard(CardDef d) { return d.IsBasicPokemon; }
        internal static bool AnyOfMine(CardInstance card, PokemonInPlay mon) { return true; }
        internal static bool OpponentHasTagTeam(EffectContext ctx) { return ctx.OpponentPokemon().Any(m => m.Def.IsTagTeam); }
        private static bool KnockedOutLastTurn(EffectContext ctx) { return ctx.Me.LastKnockedOutByAttackTurn == ctx.OpponentsLastTurn; }
        private static bool HaveStadiumInPlay(EffectContext ctx) { return ctx.State.Stadium != null && ctx.State.StadiumOwner == ctx.Player; }
        private static int Wishiwashis(EffectContext ctx) { return ctx.MyPokemon(m => m.Name == "Wishiwashi" || m.Name == "Wishiwashi-GX").Count; }

        /// <summary>A Pokémon-GX of the player's that evolves from Eevee (for the Cheer Abilities).</summary>
        internal static bool EeveeGX(PokemonInPlay mon) { return mon != null && mon.Def.IsGX && mon.Def.EvolvesFrom == "Eevee"; }

        /// <summary>
        /// "If this Pokémon has at least these extra Energy attached (in addition to this attack's cost)", for typed extras
        /// ("3 extra [W] Energy", "1 extra [P] Energy and 1 extra [D] Energy").
        /// </summary>
        internal static bool ExtraTyped(EffectContext ctx, AttackDef atk, params EnergyType[] extra)
        {
            var me = ctx.Pokemon;
            if (me == null) return false;
            var cost = ctx.Engine.AttackCost(me, atk);
            cost.AddRange(extra);
            return ctx.Engine.CanPayCost(me, cost);
        }

        /// <summary>
        /// TAG TEAM Supporters: "When you play this card, you may discard N other cards from your hand. If you do, ..." Asks, discards
        /// the cards the player picks and returns true when they did.
        /// </summary>
        internal static bool TagTeamBonus(EffectContext ctx, int count, string prompt)
        {
            if (ctx.Me.Hand.Count < count || !ctx.AskYesNo(prompt)) return false;
            var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), count, count, ctx.SourceName + ": discard " + count + (count == 1 ? " card" : " cards") + " from your hand");
            foreach (var card in picks) ctx.Engine.DiscardFromHand(ctx.Player, card);
            return true;
        }

        /// <summary>"Your opponent's Active Pokémon is now Poisoned. Put 2 damage counters instead of 1 on that Pokémon between turns."</summary>
        internal sealed class ToxicCECMon : Mon
        {
            private readonly string attack;

            public ToxicCECMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending) && ctx.ApplyCondition(defending, SpecialCondition.Poisoned)) defending.PoisonCounterBase = 2;
                return true;
            }
        }

        /// <summary>"Your opponent can't play any Trainer cards from their hand during their next turn" (Cold Snap).</summary>
        internal sealed class TrainerLockCECEffect : ContinuousEffect
        {
            private readonly int affected;

            public TrainerLockCECEffect(string name, int owner, int affectedPlayer, int expiresAfterTurn)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                affected = affectedPlayer;
            }

            public override bool BlocksTrainer(GameEngine engine, int player, CardInstance card) { return player == affected && card.Def.IsTrainer; }
        }

        /// <summary>Solar Power: during the player's next turn, attacks of their Grass and Fire Pokémon cost no Energy.</summary>
        internal sealed class SolarPowerEffect : ContinuousEffect
        {
            public SolarPowerEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override void ModifyAttackCost(GameEngine engine, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon.Owner != Owner) return;
                if (engine.IsType(mon, EnergyType.Grass) || engine.IsType(mon, EnergyType.Fire)) cost.Clear();
            }
        }

        // ================================================================== Grass

        /// <summary>
        /// Shining Vine (erratum, 2024-11-07): during your turn, whenever you attach a basic Grass Energy card from your hand to it while
        /// it's your Active Pokémon, you may switch 1 of the opponent's Benched Pokémon with their Active. Solar Plant-GX: 50 to each
        /// of the opponent's Pokémon; with 2 extra Energy, heal all damage from all of your Pokémon.
        /// </summary>
        private sealed class VenusaurSnivyGX : Mon
        {
            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                var me = src.Pokemon;
                if (me == null || target != me || !src.IsActive(me) || !src.IsMyTurn || energy == null) return;
                if (!CardsEVS1.IsEnergyOf(energy.Def, EnergyType.Grass) || src.GustTargets().Count == 0) return;
                if (!src.AskYesNo("Shining Vine: switch 1 of your opponent's Benched Pokémon with their Active Pokémon?")) return;
                Announce(src, "Shining Vine");
                src.GustOpponent("Shining Vine: choose 1 of your opponent's Benched Pokémon to switch in");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Plant-GX") return false;
                bool heal = ExtraEnergy(ctx, atk, 2);
                CardsCRZ1.DamageEachOpponent(ctx, 50);
                if (!heal) return true;
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, mon.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>
        /// Fragrant Flower Garden: once during your turn, heal 30 from each of your Pokémon. Massive Bloom: 10 less for each damage
        /// counter on it. Allergic Explosion-GX: Burned, Paralyzed and Poisoned.
        /// </summary>
        private sealed class VileplumeGX : Mon
        {
            private static readonly Mon Allergic = new CardsCES1.ConditionsMon("Allergic Explosion-GX", SpecialCondition.Burned, SpecialCondition.Paralyzed, SpecialCondition.Poisoned);

            protected override string Activated { get { return "Fragrant Flower Garden"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.MyPokemon().Any(m => m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon())
                    if (mon.DamageCounters > 0) ctx.Heal(mon, 30);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Massive Bloom") return Allergic.HandleAttack(ctx, atk);
                int damage = Math.Max(0, atk.Damage - 10 * ctx.Pokemon.DamageCounters);
                if (damage > 0) ctx.DamageDefending(damage);
                else ctx.Log("Massive Bloom did no damage.");
                return true;
            }
        }

        /// <summary>Solar Power: during your next turn, attacks of your Grass and Fire Pokémon cost no Energy (including ones put into play then).</summary>
        private sealed class SunfloraCEC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Solar Power") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fx = new SolarPowerEffect("Attacks of " + ctx.Engine.PlayerName(ctx.Player) + "'s Grass and Fire Pokémon cost no Energy during their next turn (Solar Power)", ctx.Player, ctx.Turn + 2);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }

        /// <summary>Swaying Strangle: the opponent's Pokémon with a Special Condition can't retreat. Poison Tentacles: Poisoned.</summary>
        private sealed class CradilyCEC : Mon
        {
            private static readonly Mon PoisonTentacles = new ConditionAttackMon("Poison Tentacles", SpecialCondition.Poisoned);

            public override bool PreventsRetreatOf(EffectContext src, PokemonInPlay mon)
            {
                return src.Pokemon != null && mon != null && mon.Owner != src.Player && mon.Conditions != SpecialCondition.None;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return PoisonTentacles.HandleAttack(ctx, atk); }
        }

        /// <summary>Improvisational Performance: with exactly 1 card in hand +100; exactly 3, Confused; exactly 6, 30 to each Benched opponent.</summary>
        private sealed class KricketuneCEC : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Improvisational Performance") return false;
                int hand = ctx.Me.Hand.Count;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage + (hand == 1 ? 100 : 0));
                if (hand == 3 && TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (hand == 6)
                    foreach (var mon in ctx.OpponentBench()) ctx.DamageTarget(mon, 30);
                return true;
            }
        }

        /// <summary>Seasonal Blessings: once during your turn, draw a card. Bounce: you may switch it with a Benched Pokémon.</summary>
        private sealed class SawsbuckCEC : Mon
        {
            private static readonly Mon Bounce = new SwitchSelfMon("Bounce", true);

            protected override string Activated { get { return "Seasonal Blessings"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.Draw(1); }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Bounce.HandleAttack(ctx, atk); }
        }

        /// <summary>Tracking Shot: 80 damage to 1 of the opponent's Benched Pokémon that has damage counters on it.</summary>
        private sealed class TrackingShotMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tracking Shot") return false;
                var targets = ctx.OpponentBench().Where(m => m.DamageCounters > 0).ToList();
                if (targets.Count == 0)
                {
                    ctx.Log("Tracking Shot did nothing: none of your opponent's Benched Pokémon has damage counters on it.");
                    return true;
                }
                var target = ctx.ChoosePokemon(targets, "Tracking Shot: 80 damage to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 80);
                return true;
            }
        }

        /// <summary>Beast Boost (Ultra Beast): its attacks do 20 more to the opponent's Active Pokémon for each Prize card you've taken. Touchdown: heal 30.</summary>
        private sealed class BuzzwoleCEC : Mon
        {
            private static readonly Mon Touchdown = new CardsTWM1.HealSelfMon("Touchdown", 30);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Attacker != src.Pokemon || !info.ToOpponentsActive) return damage;
                return damage + 20 * src.Engine.PrizesTaken(src.Player);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Touchdown.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fire

        /// <summary>
        /// Brilliant Flare: you may search your deck for up to 3 cards (at least 1 if you search). Crimson Flame Pillar-GX: attach 5 basic
        /// Energy cards from your discard pile to your Pokémon; with 1 extra Energy (counted before attaching: your ruling, v68), Burned
        /// and Confused.
        /// </summary>
        private sealed class CharizardBraixenGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Brilliant Flare")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count > 0 && ctx.AskYesNo("Brilliant Flare: search your deck for up to 3 cards?"))
                        ctx.SearchToHand(AnyCard, 3, "Brilliant Flare: choose up to 3 cards to put into your hand", false, 1);
                    return true;
                }
                if (atk.Name != "Crimson Flame Pillar-GX") return false;
                var defending = ctx.Defending;
                bool extra = ExtraEnergy(ctx, atk, 1);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, Math.Min(5, energy.Count), AnyOfMine, "Crimson Flame Pillar-GX: attach a basic Energy card to 1 of your Pokémon", false);
                if (extra && TefPokemon.StillDefending(ctx, defending))
                {
                    ctx.ApplyCondition(defending, SpecialCondition.Burned);
                    ctx.ApplyCondition(defending, SpecialCondition.Confused);
                }
                return true;
            }
        }

        /// <summary>
        /// Power Cheer: your Pokémon-GX that evolve from Eevee do 30 more to the opponent's Active Pokémon (only 1 Power Cheer applies).
        /// Flamethrower: discard an Energy from it.
        /// </summary>
        private sealed class FlareonCEC : Mon
        {
            private static readonly Mon Flamethrower = new CardsTWM1.SelfDiscardMon("Flamethrower", 1);

            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (src.Pokemon == null || a == null || a.Owner != src.Player || !info.ToOpponentsActive || !EeveeGX(a)) return damage;
                return CardsCRZ1.IsLeadSource(src, "Power Cheer") ? damage + 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Flamethrower.HandleAttack(ctx, atk); }
        }

        /// <summary>Fire Fling: put 4 basic Fire Energy cards from your discard pile into your hand (as many as there are).</summary>
        private sealed class FireFlingMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fire Fling") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var fire = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire)).ToList();
                if (fire.Count == 0) return true;
                var picks = fire.Count <= 4 ? fire : ctx.ChooseCards(fire, 4, 4, "Fire Fling: put 4 Fire Energy cards into your hand");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Victory Heal: once during your turn, heal 20 damage from 1 of your Benched Pokémon.</summary>
        private sealed class VictiniCEC : Mon
        {
            protected override string Activated { get { return "Victory Heal"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Bench.Any(m => m.DamageCounters > 0); }

            protected override void Ability(EffectContext ctx)
            {
                var hurt = ctx.MyBench().Where(m => m.DamageCounters > 0).ToList();
                var target = hurt.Count == 1 ? hurt[0] : ctx.ChoosePokemon(hurt, "Victory Heal: heal 20 damage from which Benched Pokémon?");
                if (target != null) ctx.Heal(target, 20);
            }
        }

        /// <summary>Explosive Fire Dance: when it evolves from the hand, look at the top 8 cards and attach any basic Energy there to your Pokémon.</summary>
        private sealed class EmboarCEC : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (!TriggerOnce(ctx, "Explosive Fire Dance", ctx.Me.Deck.Count > 0, "Explosive Fire Dance: look at the top 8 cards of your deck and attach basic Energy cards you find there to your Pokémon?")) return;
                var top = ctx.LookAtTop(8);
                var energy = top.Where(c => c.Def.IsBasicEnergy).ToList();
                if (energy.Count > 0)
                    ctx.AttachCardsAnyWay(energy, energy.Count, AnyOfMine, "Explosive Fire Dance: attach a basic Energy card to 1 of your Pokémon", true);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Flaming Shot: once during your turn, discard a basic Fire Energy card from your hand to put 2 damage counters on 1 of the
        /// opponent's Pokémon. Backfire: 2 Fire Energy attached to it go to your hand. Massive Heat Wave-GX: discard an Energy from each
        /// of the opponent's Pokémon.
        /// </summary>
        private sealed class VolcaronaGX : Mon
        {
            private static readonly Mon Backfire = new CardsSHF1.EnergyToHandMon("Backfire", EnergyType.Fire, 2);

            private static bool FireCard(CardInstance c) { return CardsEVS1.IsEnergyOf(c.Def, EnergyType.Fire); }

            protected override string Activated { get { return "Flaming Shot"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(FireCard) && ctx.OpponentPokemon().Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var fire = ctx.Me.Hand.Where(FireCard).ToList();
                var card = fire.Count == 1 ? fire[0] : ctx.ChooseCards(fire, 1, 1, "Flaming Shot: discard which Fire Energy card?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (!ctx.Me.Discard.Contains(card)) return;
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), "Flaming Shot: put 2 damage counters on which of your opponent's Pokémon?");
                if (target != null) ctx.PlaceCounters(target, 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Massive Heat Wave-GX") return Backfire.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon())
                    if (mon.Energy.Count > 0 && ctx.CanAffect(mon)) ctx.DiscardEnergyFrom(mon, 1, "Massive Heat Wave-GX: discard an Energy from " + mon.Name);
                return true;
            }
        }

        /// <summary>Swirling Inferno: discard all Pokémon Tools and Special Energy from each of the opponent's Pokémon.</summary>
        private sealed class SwirlingInfernoMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Swirling Inferno") return false;
                ctx.DamageDefending(atk.Damage);
                foreach (var mon in ctx.OpponentPokemon())
                {
                    if (!ctx.CanAffect(mon)) continue;
                    foreach (var tool in mon.Tools.ToList()) ctx.Engine.DiscardTool(mon, tool);
                    foreach (var special in mon.Energy.Where(x => x.Def.IsAttachedSpecialEnergy).ToList()) ctx.Engine.DiscardEnergy(mon, special);
                }
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>
        /// Splash Maker: you may attach up to 3 basic Water Energy cards from your hand to your Pokémon; heal 50 from each of those
        /// Pokémon for each card attached to it. Bubble Launcher-GX: Paralyzed; 150 more with 3 extra Water Energy.
        /// </summary>
        private sealed class BlastoisePiplupGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Bubble Launcher-GX")
                {
                    var defending = ctx.Defending;
                    bool extra = ExtraTyped(ctx, atk, EnergyType.Water, EnergyType.Water, EnergyType.Water);
                    ctx.DamageDefending(atk.Damage + (extra ? 150 : 0));
                    if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                    return true;
                }
                if (atk.Name != "Splash Maker") return false;
                ctx.DamageDefending(atk.Damage);
                var attachedTo = new Dictionary<int, int>();
                for (int i = 0; i < 3; i++)
                {
                    var water = ctx.Me.Hand.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                    if (water.Count == 0) break;
                    var picks = ctx.ChooseCards(water, 0, 1, "Splash Maker: attach a Water Energy card from your hand (" + (i + 1) + " of up to 3)");
                    if (picks.Count == 0) break;
                    var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Splash Maker: attach " + picks[0].Def.Name + " to which of your Pokémon?");
                    if (target == null || !ctx.Me.Hand.Remove(picks[0])) break;
                    if (!ctx.Engine.AttachEnergyCard(picks[0], target, true)) continue;
                    int already;
                    attachedTo.TryGetValue(target.Uid, out already);
                    attachedTo[target.Uid] = already + 1;
                }
                foreach (var kv in attachedTo)
                {
                    var mon = ctx.State.FindPokemon(kv.Key);
                    if (mon != null && mon.DamageCounters > 0) ctx.Heal(mon, 50 * kv.Value);
                }
                return true;
            }
        }

        /// <summary>Confusion Wave: both Active Pokémon are now Confused. Used for Confusion Wave, Foul Odor (the attack name is passed in).</summary>
        internal sealed class ConfusionWaveMon : Mon
        {
            private readonly string attack;

            public ConfusionWaveMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Confused);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Vitality Cheer: your Pokémon-GX that evolve from Eevee get +60 HP (only 1 applies). Refreshing Rain: heal 30 from each of your Pokémon.</summary>
        private sealed class VaporeonCEC : Mon
        {
            private static readonly Mon RefreshingRain = new CardsPLF1.HealAllMineMon("Refreshing Rain", 30);

            public override int ModifyMaxHp(EffectContext src, PokemonInPlay mon, int hp)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player || !EeveeGX(mon)) return hp;
                return CardsCRZ1.IsLeadSource(src, "Vitality Cheer") ? hp + 60 : hp;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return RefreshingRain.HandleAttack(ctx, atk); }
        }

        /// <summary>Ice Fang: on heads, Paralyzed and discard an Energy from it.</summary>
        private sealed class IceFangMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ice Fang") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.ApplyCondition(defending, SpecialCondition.Paralyzed);
                if (defending.Energy.Count > 0 && ctx.CanAffect(defending)) ctx.DiscardEnergyFrom(defending, 1, "Ice Fang: discard an Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>
        /// Cold Snap: the opponent can't play Trainer cards from their hand during their next turn; it can't be used if 1 of your Pokémon
        /// used Cold Snap during your last turn. Blizzard: 10 to each of the opponent's Benched Pokémon.
        /// </summary>
        private sealed class WalreinCEC : Mon
        {
            private static readonly Mon Blizzard = new CardsMEW1.SpreadBenchMon("Blizzard", 10);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                if (ctx.Attack(attackIndex).Name != "Cold Snap") return true;
                return !(ctx.Me.LastAttackName == "Cold Snap" && ctx.Me.LastAttackTurn == ctx.MyLastTurn);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Cold Snap") return Blizzard.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new TrainerLockCECEffect(ctx.Engine.PlayerName(ctx.Opponent) + " can't play Trainer cards from their hand during their next turn (Cold Snap)", ctx.Player, ctx.Opponent, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>High Water: attach 2 basic Water Energy cards from your discard pile to 1 of your Pokémon.</summary>
        private sealed class HighWaterMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "High Water") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var water = ctx.Me.Discard.Where(c => CardsEVS1.IsEnergyOf(c.Def, EnergyType.Water)).ToList();
                if (water.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "High Water: attach Water Energy from your discard pile to which of your Pokémon?");
                if (target == null) return true;
                var picks = water.Count <= 2 ? water : ctx.ChooseCards(water, 2, 2, "High Water: attach 2 Water Energy cards to " + target.Name);
                foreach (var e in picks)
                    if (ctx.Me.Discard.Remove(e)) ctx.Engine.AttachEnergyCard(e, target);
                return true;
            }
        }

        /// <summary>Direct Dive: discard all Energy from it; 100 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class DirectDiveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Direct Dive") return false;
                var me = ctx.Pokemon;
                foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Direct Dive: 100 damage to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 100);
                return true;
            }
        }

        /// <summary>
        /// Recall: use an attack of 1 of its previous Evolutions (only Recall's cost is needed; Compendium, 2019-10-31). Aquafall: discard
        /// all Energy from it.
        /// </summary>
        private sealed class EmpoleonCEC : Mon
        {
            private static bool ChoosePrevious(EffectContext ctx, out CardDef source, out int index)
            {
                source = null;
                index = -1;
                var me = ctx.Pokemon;
                var opts = new List<DecisionOption>();
                for (int k = 0; k < me.Stack.Count - 1; k++)
                {
                    var d = me.Stack[k].Def;
                    if (!d.IsPokemon) continue;
                    for (int i = 0; i < d.Attacks.Count; i++)
                    {
                        var a = d.Attacks[i];
                        if (GameEngine.IsGxAttack(a) && ctx.Me.GxUsed) continue;
                        opts.Add(new DecisionOption { Label = d.Name + ": " + a.Name + (a.Damage > 0 ? " (" + a.Damage + a.DamageSuffix + ")" : ""), Card = d, Index = i });
                    }
                }
                if (opts.Count == 0) return false;
                int pick = ctx.Engine.AskOne(ctx.Player, DecisionKind.ChooseOption, "Recall: choose an attack of " + me.Name + "'s previous Evolutions", opts, ctx.Source);
                source = opts[pick].Card;
                index = opts[pick].Index;
                return true;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Aquafall")
                {
                    ctx.DamageDefending(atk.Damage);
                    var me = ctx.Pokemon;
                    if (ctx.InPlay(me))
                        foreach (var e in me.Energy.ToList()) ctx.Engine.DiscardEnergy(me, e);
                    return true;
                }
                if (atk.Name != "Recall") return false;
                CardDef source;
                int index;
                if (ChoosePrevious(ctx, out source, out index)) UseCopiedAttack(ctx, source, index);
                else ctx.Log("Recall did nothing: " + ctx.Pokemon.Name + " has no previous Evolutions with attacks.");
                return true;
            }
        }

        /// <summary>
        /// Whirlpool Suction: once during your turn, from the Bench, your opponent switches their Active Pokémon with a Benched one; if
        /// they do, discard all cards attached to Phione and put it on the bottom of your deck.
        /// </summary>
        private sealed class PhioneCEC : Mon
        {
            protected override string Activated { get { return "Whirlpool Suction"; } }

            protected override bool AbilityReady(EffectContext ctx) { return !ctx.IsActive(ctx.Pokemon) && ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.OpponentSwitchesActive()) ctx.Engine.PutPokemonOnDeckBottom(ctx.Pokemon);
            }
        }

        /// <summary>Tailwind: attach an Energy card from your hand to 1 of your Pokémon.</summary>
        internal sealed class TailwindMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Tailwind") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var card = energy.Count == 1 ? energy[0] : ctx.ChooseCards(energy, 1, 1, "Tailwind: attach which Energy card from your hand?")[0];
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Tailwind: attach " + card.Def.Name + " to which of your Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
                return true;
            }
        }

        /// <summary>
        /// Scatter: at the end of the opponent's turn, after Knock Outs and before Pokémon Checkup (Compendium, 2019-10-31), if it has
        /// damage counters, flip a coin; on tails, shuffle it and its cards into your deck.
        /// </summary>
        private sealed class WishiwashiCEC : Mon
        {
            public override void OnEndOfTurnAfterKnockOuts(EffectContext src)
            {
                var me = src.Pokemon;
                if (me == null || src.IsMyTurn || me.DamageCounters == 0 || !src.InPlay(me)) return;
                src.Log("Scatter: " + me.Name + " has damage counters on it, so " + src.Engine.PlayerName(src.Player) + " flips a coin.");
                if (src.Flip()) return;
                src.Engine.ShufflePokemonIntoDeck(me);
            }
        }

        /// <summary>Massive Catch-GX: look at the top 12 cards and put any number of Basic Pokémon there onto your Bench; shuffle the rest back.</summary>
        private sealed class MassiveCatchMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Massive Catch-GX") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count == 0) return true;
                var top = ctx.LookAtTop(12);
                var basics = top.Where(c => c.Def.IsBasicPokemon && ctx.Engine.IsPlayableBasic(c.Def) && !ctx.Engine.PutIntoPlayBlocked(ctx.Player, c.Def)).ToList();
                int space = ctx.BenchSpace;
                if (basics.Count > 0 && space > 0)
                {
                    var picks = ctx.ChooseCards(basics, 0, Math.Min(space, basics.Count), "Massive Catch-GX: put any number of Basic Pokémon onto your Bench");
                    foreach (var card in picks)
                    {
                        if (ctx.BenchSpace <= 0 || !ctx.Me.Deck.Remove(card)) continue;
                        ctx.Engine.PutOntoBench(ctx.Player, card, false);
                    }
                }
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Liquidation: during your next turn, the Defending Pokémon takes 60 more damage from attacks (after Weakness and Resistance).</summary>
        private sealed class LiquidationMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Liquidation") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending) || !ctx.CanAffect(defending)) return true;
                var fx = new DamageTakenEffect(defending.Name + " takes 60 more damage from attacks during your next turn (Liquidation)", ctx.Player, defending.Uid, ctx.Turn + 2, 60);
                fx.ActiveFromTurn = ctx.Turn + 2;
                ctx.AddEffect(fx);
                return true;
            }
        }
    }
}
