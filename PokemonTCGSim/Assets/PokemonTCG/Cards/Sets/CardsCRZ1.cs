using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Crown Zenith (CRZ 1-122 and 160, 2023): the Pokémon that weren't already in the pool. Regulation marks D, E and F:
    /// Expanded only. CRZ 102 is a second print of 101 (Rayquaza VMAX). Pokémon whose attacks only do their printed damage
    /// need no script: Tangela, Sunkern, Cherubi, Larvesta, Seel, Corphish, Snorunt, Shinx (39), Helioptile, Graveler,
    /// Baltoy, Riolu, Pancham, Skrelp, Pawniard (92), Snorlax, Yungoos.
    ///
    /// Sword &amp; Shield mechanics (v49, engine): Pokémon V / VMAX / VSTAR (VMAX and VSTAR are stored as Stage 1 cards that
    /// evolve from their V), VSTAR Powers (only 1 per game, attack or Ability: GameEngine.UsableAttacks and the main
    /// actions), Radiant Pokémon (1 per deck), Single / Rapid / Fusion Strike (CardDef.Strike), the Lost Zone, and Origin
    /// Forme Dialga VSTAR's extra turn (GameEngine.StartTurn).
    /// </summary>
    public static class CardsCRZ1
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("CRZ-1", new CoinsMon("Leaf Boomerang", 2));
            r.RegisterByKey("CRZ-2", new CardsCES1.ConditionsMon("Offensive Scent", SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("CRZ-3", new BellossomCRZ());
            r.RegisterByKey("CRZ-5", new CardsTWM1.HealSelfMon("Suctioning Vines", 30));
            r.RegisterByKey("CRZ-6", new MachCutMon());
            r.RegisterByKey("CRZ-8", new SpreadAllMon("Swoop Across"));
            r.RegisterByKey("CRZ-9", new ComboMon(new Cards30CC.BenchHitMon("Shoot Through", 20), new LockAfterAttackMon("Jet Wing", false)));
            r.RegisterByKey("CRZ-10", new FlipBonusMon("Trip Over", 20));
            r.RegisterByKey("CRZ-12", new ComboMon(new CardsCES1.ConditionsMon("Festering Saliva", SpecialCondition.Burned, SpecialCondition.Poisoned), new NoRetreatMon("Bind Down")));
            r.RegisterByKey("CRZ-13", new ComboMon(new CardsASC.SelfGuardMon("Leaf Guard", 30), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("CRZ-14", new LeafeonVstar());
            r.RegisterByKey("CRZ-15", new ComboMon(new CardsOBF1.AttachFromDiscardToSelfMon("Energize", EnergyType.Lightning), new TailsNothingMon("Surprise Attack")));
            r.RegisterByKey("CRZ-16", new ComboMon(new CardsWHT.DragOffMon("Drag Off", 20), new CoinsMon("Triple Whip", 3)));
            r.RegisterByKey("CRZ-17", new CalyrexCRZ());
            r.RegisterByKey("CRZ-18", new CardsBLK.PeckOffMon("Incinerate"));
            r.RegisterByKey("CRZ-19", new ComboMon(new CardsASC.SelfDamagedBonusMon("Explosive Fire", 100), new CardsTWM1.SelfDiscardMon("Star Blaze", 2)));
            r.RegisterByKey("CRZ-20", new RadiantCharizard());
            r.RegisterByKey("CRZ-21", new CardsSVP.FloatMon(EnergyType.Fire));
            r.RegisterByKey("CRZ-22", new ComboMon(new BurstingPowerMon(), new CardsMEG.DefendingEnergyBonusMon("Flare Juggling", 30)));
            r.RegisterByKey("CRZ-23", new ComboMon(new FireballFeverMon(), new EmberStarMon()));
            r.RegisterByKey("CRZ-25", new LockAfterAttackMon("Volcanic Heat", false));
            r.RegisterByKey("CRZ-26", new Cards30C.TypeEnergyBonusMon("Hydro Burn", EnergyType.Water, 80));
            r.RegisterByKey("CRZ-27", new BenchSearchAttackMon("Call for Family", 1));
            r.RegisterByKey("CRZ-28", new ComboMon(new ConditionAttackMon("Tail Trickery", SpecialCondition.Confused), new ConditionAttackMon("Super Singe", SpecialCondition.Burned)));
            r.RegisterByKey("CRZ-30", new SearchAttackMon("Find It", d => d.IsItem, 1));
            r.RegisterByKey("CRZ-31", new CardsTWM1.HealSelfMon("Nap", 30));
            r.RegisterByKey("CRZ-32", new CardsTWM1.HealSelfMon("Bubble Drain", 30));
            r.RegisterByKey("CRZ-35", new EmotionalDrawMon());
            r.RegisterByKey("CRZ-36", new ComboMon(new CardsMEG.ChargeSelfMon("Wave Summoning", EnergyType.Water, 1), new DynamicWaveMon()));
            r.RegisterByKey("CRZ-37", new ComboMon(new DualSplashMon(), new LockAfterAttackMon("Aqua Typhoon", true)));
            r.RegisterByKey("CRZ-38", new CardsMEG.ChargeSelfMon("Frost Charge", EnergyType.Water, 1));
            r.RegisterByKey("CRZ-40", new RecoilMon("Thunder Jolt", 10));
            r.RegisterByKey("CRZ-41", new CardsMEG.SnipeMon("Jumping Kick", 30));
            r.RegisterByKey("CRZ-42", new ShortingSparkMon());
            r.RegisterByKey("CRZ-43", new ComboMon(new ElectrostepMon(), new CardsPFL.DamagedBonusMon("Scar Strikes", 100)));
            r.RegisterByKey("CRZ-44", new ExplosivenessLuxray());
            r.RegisterByKey("CRZ-45", new RotomV());
            r.RegisterByKey("CRZ-46", new RotomVstar());
            r.RegisterByKey("CRZ-47", new ParalyzeFlipMon("Thunder Shock"));
            r.RegisterByKey("CRZ-48", new EelektrikCRZ());
            r.RegisterByKey("CRZ-50", new RecoilMon("Wild Charge", 50));
            r.RegisterByKey("CRZ-51", new RadiantCharjabug());
            r.RegisterByKey("CRZ-52", new RecoilMon("Wild Charge", 20));
            r.RegisterByKey("CRZ-53", new LockAfterAttackMon("Thunderous Bolt", false));
            r.RegisterByKey("CRZ-54", new ComboMon(new ReactivePulseMon(), new CardsTWM1.SelfDiscardMon("Max Fist", 2)));
            r.RegisterByKey("CRZ-55", new ZeraoraVstar());
            r.RegisterByKey("CRZ-56", new ComboMon(new BenchSearchAttackMon("Call for Family", 2), new CardsMEG.FlipUntilTailsBonusMon("Continuous Tumble", 30)));
            r.RegisterByKey("CRZ-57", new ParalyzeFlipMon("Psy Bolt"));
            r.RegisterByKey("CRZ-58", new PowerfulStormMon());
            r.RegisterByKey("CRZ-59", new ComboMon(new CardsPAR1.BringerMon("Psypump", EnergyType.Psychic, 2), new LimitBreakMon("Limit Break", 3, 90)));
            r.RegisterByKey("CRZ-60", new MewV());
            r.RegisterByKey("CRZ-61", new ComboMon(new DrawAttackMon("Double Draw", 2), new ConditionAttackMon("Psybeam", SpecialCondition.Confused)));
            r.RegisterByKey("CRZ-62", new ComboMon(new CycleDrawMon("Cycle Draw", 3), new CardsMEG.EnergyBonusMon("Moon Kinesis", EnergyType.Psychic, 30)));
            r.RegisterByKey("CRZ-63", new ConditionAttackMon("Fade to Black", SpecialCondition.Confused));
            r.RegisterByKey("CRZ-64", new ComboMon(new BothActiveEnergyMon("Energy Burst", 20), new CardsTWM1.HealSelfMon("Spiral Drain", 30)));
            r.RegisterByKey("CRZ-65", new ComboMon(new HoroscopeMon(), new SwitchSelfMon("Teleportation Burst", false)));
            r.RegisterByKey("CRZ-66", new HattereneVmax());
            r.RegisterByKey("CRZ-67", new ComboMon(new CardsTWM1.HealSelfMon("Draining Kiss", 20), new CardsSTS1.ExtrasensoryMon("Loving Sympathy", 70)));
            r.RegisterByKey("CRZ-69", new SolrockCRZ());
            r.RegisterByKey("CRZ-73", new InviteOutMon());
            r.RegisterByKey("CRZ-74", new MidnightFangMon("Midnight Fang", 80));
            r.RegisterByKey("CRZ-75", new ConditionAttackMon("Smog", SpecialCondition.Poisoned));
            r.RegisterByKey("CRZ-76", new LostClawMon());
            r.RegisterByKey("CRZ-77", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("CRZ-78", new CoinsMon("Fury Swipes", 3));
            r.RegisterByKey("CRZ-79", new CardsSSP2.StompOffMon("Dredge Up", 3));
            r.RegisterByKey("CRZ-80", new RecoilMon("Wild Tackle", 30));
            r.RegisterByKey("CRZ-82", new RocketPoisonMon());
            r.RegisterByKey("CRZ-83", new AssaultGateMon());
            r.RegisterByKey("CRZ-84", new FlipBonusMon("Fasten Claws", 20));
            r.RegisterByKey("CRZ-85", new FlipBonusMon("Sharp Claws", 60));
            r.RegisterByKey("CRZ-86", new ComboMon(new FlipBonusMon("X-Scissor", 30), new BasicTargetBonusMon("Dangerous Claws", 80)));
            r.RegisterByKey("CRZ-87", new RecoilMon("Slight Intrusion", 10));
            r.RegisterByKey("CRZ-88", new WreakHavocMon());
            r.RegisterByKey("CRZ-89", new CounterPressMon());
            r.RegisterByKey("CRZ-90", new CardsWHT.FlipMoreMon("Bullet Punch", 2, 30));
            r.RegisterByKey("CRZ-91", new RecoilMon("Reckless Charge", 10));
            r.RegisterByKey("CRZ-93", new DrawAttackMon("Spike Draw", 2));
            r.RegisterByKey("CRZ-94", new BattleLegionMon());
            r.RegisterByKey("CRZ-95", new ComboMon(new PiercingMon("Piercing Strike"), new VmaxBonusMon("Behemoth Blade", 160)));
            r.RegisterByKey("CRZ-96", new ComboMon(new PiercingMon("Break Edge"), new RecoilMon("Sword Star", 30)));
            r.RegisterByKey("CRZ-97", new ZamazentaCRZ());
            r.RegisterByKey("CRZ-98", new ZamazentaV());
            r.RegisterByKey("CRZ-99", new ZamazentaVstar());
            r.RegisterByKey("CRZ-100", new ComboMon(new DragonPulseMon(), new BurstMon("Spiral Burst", 2)));
            r.RegisterByKey("CRZ-101", new RayquazaVmax());
            r.RegisterByKey("CRZ-103", new CardsMEG.WeakenDefendingMon("Breaking Swipe", 30));
            r.RegisterByKey("CRZ-104", new DuraludonVmax());
            r.RegisterByKey("CRZ-105", new RadiantEternatus());
            r.RegisterByKey("CRZ-106", new AdrenaTackleMon("Adrena-Tackle"));
            r.RegisterByKey("CRZ-107", new DittoCRZ());
            r.RegisterByKey("CRZ-108", new VBonusMon("Vee Brave", 80));
            r.RegisterByKey("CRZ-110", new TailsNothingMon("Claw"));
            r.RegisterByKey("CRZ-111", new BidoofCRZ());
            r.RegisterByKey("CRZ-112", new CycleDrawMon("Cycle Draw", 2));
            r.RegisterByKey("CRZ-113", new AngryWhackMon());
            r.RegisterByKey("CRZ-114", new RegigigasVstar());
            r.RegisterByKey("CRZ-115", new GatherFlowersMon());
            r.RegisterByKey("CRZ-116", new ComboMon(new DoubleDipFangsMon(), new RecoilMon("Wild Tackle", 30)));
            r.RegisterByKey("CRZ-118", new StakeoutHeadbuttMon());
            r.RegisterByKey("CRZ-119", new PrimateActingMon("Primate Acting"));
            r.RegisterByKey("CRZ-120", new ComboMon(new ParalyzeFlipMon("Body Slam"), new DrawAttackMon("Nom-Nom-Nom Incisors", 3)));
            r.RegisterByKey("CRZ-121", new CardsPFL.FlipUntilTailsTimesMon("Rolling Rollout"));
            r.RegisterByKey("CRZ-122", new ComboMon(new OverheadThrowMon(), new CardsMEG.FlipUntilTailsBonusMon("Rolling Dash", 30)));
            r.RegisterByKey("CRZ-160", new RecoilMon("Wild Charge", 30));
        }

        // ================================================================== Shared helpers (CRZ and later Sword & Shield sets)

        /// <summary>"This attack does N damage to each of your opponent's Pokémon" (Weakness and Resistance only for the Active one).</summary>
        internal static void DamageEachOpponent(EffectContext ctx, int amount)
        {
            foreach (var mon in ctx.OpponentPokemon()) ctx.DamageTarget(mon, amount);
        }

        /// <summary>Moves a card from its owner's discard pile to their Lost Zone. Returns false if it wasn't in the discard pile.</summary>
        internal static bool DiscardToLostZone(EffectContext ctx, CardInstance card)
        {
            var owner = ctx.State.Players[card.Owner];
            if (!owner.Discard.Remove(card)) return false;
            owner.LostZone.Add(card);
            ctx.Engine.Emit(GameEventType.Info, card.Owner, card.Def.Name + " was put in the Lost Zone.", card.Id);
            return true;
        }

        /// <summary>Puts a card that's no longer anywhere (just taken out of a deck, hand or play) in its owner's Lost Zone.</summary>
        internal static void PutInLostZone(EffectContext ctx, CardInstance card)
        {
            ctx.State.Players[card.Owner].LostZone.Add(card);
            ctx.Engine.Emit(GameEventType.Info, card.Owner, card.Def.Name + " was put in the Lost Zone.", card.Id);
        }

        /// <summary>
        /// "You can't apply more than 1 [Ability] at a time": only one of a player's Pokémon with that working Ability counts
        /// (the one that has been in play longest).
        /// </summary>
        internal static bool IsLeadSource(EffectContext src, string ability)
        {
            PokemonInPlay lead = null;
            foreach (var mon in src.Me.InPlay())
                if (src.Engine.HasAbilityNamed(mon, ability) && (lead == null || mon.Uid < lead.Uid)) lead = mon;
            return lead != null && lead == src.Pokemon;
        }

        /// <summary>Attaches an Energy card from the player's hand by a card effect (it counts as attaching from the hand).</summary>
        internal static void AttachFromHand(EffectContext ctx, CardInstance energy, PokemonInPlay target)
        {
            if (energy == null || target == null || !ctx.Me.Hand.Remove(energy)) return;
            ctx.Engine.AttachEnergyCard(energy, target, true);
        }

        /// <summary>Attach up to max Energy cards from the hand, one at a time, to Pokémon the player picks (it counts as attaching from the hand).</summary>
        internal static int AttachEnergyFromHand(EffectContext ctx, Func<CardDef, bool> energyFilter, int max, Func<PokemonInPlay, bool> targetFilter, string prompt)
        {
            int attached = 0;
            for (int i = 0; i < max; i++)
            {
                var cards = ctx.Me.Hand.Where(c => energyFilter(c.Def)).ToList();
                var targets = ctx.MyPokemon(targetFilter);
                if (cards.Count == 0 || targets.Count == 0) break;
                var picks = ctx.ChooseCards(cards, 0, 1, prompt + " (" + (i + 1) + " of up to " + max + "; choose none to stop)");
                if (picks.Count == 0) break;
                var target = targets.Count == 1 ? targets[0] : ctx.ChoosePokemon(targets, prompt + ": attach " + picks[0].Def.Name + " to which Pokémon?");
                AttachFromHand(ctx, picks[0], target);
                attached++;
            }
            return attached;
        }

        /// <summary>Puts cards (already out of the deck) back on top of it in an order the player picks.</summary>
        internal static void PutOnTopInOrder(EffectContext ctx, List<CardInstance> cards, string prompt)
        {
            var rest = cards.ToList();
            var order = new List<CardInstance>();
            while (rest.Count > 1)
            {
                var next = ctx.ChooseCards(rest, 1, 1, prompt + ": choose the card to put " + (order.Count == 0 ? "on top" : "next (card " + (order.Count + 1) + " from the top)"))[0];
                order.Add(next);
                rest.Remove(next);
            }
            order.AddRange(rest);
            ctx.Me.Deck.InsertRange(0, order);
        }

        /// <summary>Every Benched Pokémon in play (both players').</summary>
        internal static int AllBenched(EffectContext ctx)
        {
            return ctx.Me.Bench.Count + ctx.Opp.Bench.Count;
        }

        /// <summary>Energy attached to the Pokémon that provides this type.</summary>
        internal static List<CardInstance> EnergyOfType(EffectContext ctx, PokemonInPlay mon, EnergyType type)
        {
            return mon.Energy.Where(e => CardsDRI1.ProvidesType(ctx, mon, e, type)).ToList();
        }

        /// <summary>Puts "Put [Tools] from your discard pile in the Lost Zone ... 40 more damage for each" (Scrap Short, Scrap Pulse).</summary>
        internal static void ScrapAttack(EffectContext ctx, AttackDef atk)
        {
            var tools = ctx.Me.Discard.Where(c => c.Def.IsTool).ToList();
            int moved = 0;
            if (tools.Count > 0)
            {
                var picks = ctx.ChooseCards(tools, 0, tools.Count, atk.Name + ": put any number of Pokémon Tool cards from your discard pile in the Lost Zone (40 more damage each)");
                foreach (var t in picks) if (DiscardToLostZone(ctx, t)) moved++;
            }
            ctx.DamageDefending(atk.Damage + 40 * moved);
        }

        /// <summary>
        /// Spiral Burst / Max Burst: discard up to max basic Fire Energy or up to max basic Lightning Energy (not a mix) from
        /// this Pokémon. Returns how many were discarded.
        /// </summary>
        internal static int BurstDiscard(EffectContext ctx, int max, string attack)
        {
            var me = ctx.Pokemon;
            var fire = me.Energy.Where(e => CardsSSP1.IsBasicOf(e.Def, EnergyType.Fire)).ToList();
            var lightning = me.Energy.Where(e => CardsSSP1.IsBasicOf(e.Def, EnergyType.Lightning)).ToList();
            var labels = new List<string>();
            var piles = new List<List<CardInstance>>();
            if (fire.Count > 0) { labels.Add("Discard basic Fire Energy"); piles.Add(fire); }
            if (lightning.Count > 0) { labels.Add("Discard basic Lightning Energy"); piles.Add(lightning); }
            if (piles.Count == 0) return 0;
            labels.Add("Don't discard any Energy");
            int pick = ctx.ChooseOption(attack + ": discard basic Fire or basic Lightning Energy from this Pokémon (80 more damage for each)?", labels.ToArray());
            if (pick < 0 || pick >= piles.Count) return 0;
            var pile = piles[pick];
            var chosen = ctx.ChooseCards(pile, 1, Math.Min(max, pile.Count), attack + ": discard which? (80 more damage for each)");
            int n = 0;
            foreach (var card in chosen) if (ctx.Engine.DiscardEnergy(me, card)) n++;
            return n;
        }

        /// <summary>"Discard a card from your hand. If you do, draw N cards." (Cycle Draw)</summary>
        internal static void CycleDraw(EffectContext ctx, string attack, int draw)
        {
            if (ctx.Me.Hand.Count == 0) return;
            var pick = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, attack + ": discard a card from your hand, then draw " + draw + " cards");
            if (pick.Count == 0) return;
            ctx.Engine.DiscardFromHand(ctx.Player, pick[0]);
            ctx.Draw(draw);
        }

        // ================================================================== Reusable attacks

        /// <summary>"This attack does N damage to each of your opponent's Pokémon" (the printed damage).</summary>
        internal sealed class SpreadAllMon : Mon
        {
            private readonly string attack;

            public SpreadAllMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                DamageEachOpponent(ctx, atk.Damage);
                return true;
            }
        }

        /// <summary>"This attack does N damage for each Energy attached to both Active Pokémon." (Energy Burst, Star Force)</summary>
        internal sealed class BothActiveEnergyMon : Mon
        {
            private readonly string attack;
            private readonly int per;

            public BothActiveEnergyMon(string attack, int per)
            {
                this.attack = attack;
                this.per = per;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int n = (ctx.Me.Active == null ? 0 : ctx.EnergyCount(ctx.Me.Active)) + (ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending));
                ctx.DamageDefending(per * n);
                return true;
            }
        }

        /// <summary>"Discard a card from your hand. If you do, draw N cards."</summary>
        internal sealed class CycleDrawMon : Mon
        {
            private readonly string attack;
            private readonly int draw;

            public CycleDrawMon(string attack, int draw)
            {
                this.attack = attack;
                this.draw = draw;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                CycleDraw(ctx, attack, draw);
                return true;
            }
        }

        /// <summary>"This attack's damage isn't affected by Weakness or Resistance, or by any effects on your opponent's Active Pokémon."</summary>
        internal sealed class PiercingMon : Mon
        {
            private readonly string attack;

            public PiercingMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, false, true);
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon is a Pokémon VMAX, this attack does N more damage."</summary>
        internal sealed class VmaxBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public VmaxBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && ctx.Defending.Def.IsVmax ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon is a Pokémon V, this attack does N more damage."</summary>
        internal sealed class VBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public VBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && ctx.Defending.Def.IsPokemonV ? bonus : 0));
                return true;
            }
        }

        /// <summary>"If your opponent's Active Pokémon is a Basic Pokémon, this attack does N more damage."</summary>
        internal sealed class BasicTargetBonusMon : Mon
        {
            private readonly string attack;
            private readonly int bonus;

            public BasicTargetBonusMon(string attack, int bonus)
            {
                this.attack = attack;
                this.bonus = bonus;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Defending != null && ctx.Defending.IsBasic ? bonus : 0));
                return true;
            }
        }

        /// <summary>Spiral Burst / Max Burst: 80 more damage for each basic Fire or basic Lightning Energy discarded from this Pokémon.</summary>
        internal sealed class BurstMon : Mon
        {
            private readonly string attack;
            private readonly int max;

            public BurstMon(string attack, int max)
            {
                this.attack = attack;
                this.max = max;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int n = BurstDiscard(ctx, max, attack);
                ctx.DamageDefending(atk.Damage + 80 * n);
                return true;
            }
        }

        // ================================================================== Grass

        /// <summary>Flower Spin: 80 for each heads of 3 coins, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class BellossomCRZ : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Flower Spin") return false;
                ctx.DamageDefending(atk.Damage * ctx.FlipCoins(3));
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Mach Cut: discard a Special Energy from the opponent's Active Pokémon.</summary>
        private sealed class MachCutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mach Cut") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!TefPokemon.StillDefending(ctx, defending)) return true;
                ctx.DiscardEnergyFrom(defending, 1, "Mach Cut: discard a Special Energy from " + defending.Name, e => e.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Ivy Star (VSTAR Power): switch 1 of the opponent's Benched Pokémon with their Active Pokémon. Leaf Guard: 30 less damage next turn.</summary>
        private sealed class LeafeonVstar : Mon
        {
            private static readonly Mon leafGuard = new CardsASC.SelfGuardMon("Leaf Guard", 30);

            protected override string Activated { get { return "Ivy Star"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Active != null && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.GustOpponent("Ivy Star: choose 1 of your opponent's Benched Pokémon to switch with their Active Pokémon");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return leafGuard.HandleAttack(ctx, atk); }
        }

        /// <summary>King's Instructions: search your deck for up to 2 cards. Bloomshine: heal 20 from each of your Pokémon.</summary>
        private sealed class CalyrexCRZ : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "King's Instructions")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 2, "King's Instructions: choose up to 2 cards to put into your hand", false);
                    return true;
                }
                if (atk.Name == "Bloomshine")
                {
                    ctx.DamageDefending(atk.Damage);
                    foreach (var mon in ctx.MyPokemon(m => m.DamageCounters > 0)) ctx.Heal(mon, 20);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Fire

        /// <summary>Excited Heart: its attacks cost [C] less for each Prize card the opponent has taken. Combustion Blast: can't use it next turn.</summary>
        private sealed class RadiantCharizard : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (mon == null || mon != src.Pokemon) return;
                for (int i = src.OpponentPrizesTaken; i > 0; i--) if (!cost.Remove(EnergyType.Colorless)) break;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Combustion Blast") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.SelfCantUseAttackNextTurn(atk.Name);
                return true;
            }
        }

        /// <summary>Bursting Power: attach up to 2 basic Energy cards from your hand to your Pokémon in any way you like.</summary>
        private sealed class BurstingPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bursting Power") return false;
                ctx.DamageDefending(atk.Damage);
                AttachEnergyFromHand(ctx, d => d.IsBasicEnergy, 2, m => true, "Bursting Power: attach a basic Energy card from your hand");
                return true;
            }
        }

        /// <summary>Fireball Fever: discard up to 5 cards from the top of your deck, 40 more damage for each.</summary>
        private sealed class FireballFeverMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Fireball Fever") return false;
                int max = Math.Min(5, ctx.Me.Deck.Count);
                int n = max > 0 ? ctx.AskNumber(0, max, "Fireball Fever: discard how many cards from the top of your deck (40 more damage for each)?") : 0;
                int discarded = n > 0 ? ctx.Engine.DiscardTopOfDeck(ctx.Player, n).Count : 0;
                ctx.DamageDefending(atk.Damage + 40 * discarded);
                return true;
            }
        }

        /// <summary>Ember Star (VSTAR Power): 30 damage for each Energy card in your discard pile.</summary>
        private sealed class EmberStarMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ember Star") return false;
                ctx.DamageDefending(atk.Damage * ctx.Me.Discard.Count(c => c.Def.IsEnergy));
                return true;
            }
        }

        // ================================================================== Water

        /// <summary>Emotional Draw: shuffle your hand into your deck, then draw 5 cards.</summary>
        private sealed class EmotionalDrawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Emotional Draw") return false;
                ctx.DamageDefending(atk.Damage);
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                ctx.Draw(5);
                return true;
            }
        }

        /// <summary>Dynamic Wave: put 3 Energy attached to this Pokémon into your hand, then 180 damage to 1 of your opponent's Pokémon.</summary>
        private sealed class DynamicWaveMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dynamic Wave") return false;
                var me = ctx.Pokemon;
                var energy = me.Energy.ToList();
                var picks = energy.Count <= 3 ? energy : ctx.ChooseCards(energy, 3, 3, "Dynamic Wave: put which 3 Energy attached to this Pokémon into your hand?");
                foreach (var e in picks) ctx.Engine.ReturnEnergyToHand(me, e);
                ctx.DamageAnyOpponent(180);
                return true;
            }
        }

        /// <summary>Dual Splash: 50 damage to 2 of your opponent's Pokémon.</summary>
        private sealed class DualSplashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dual Splash") return false;
                var all = ctx.OpponentPokemon();
                var targets = all.Count <= 2 ? all : ctx.ChoosePokemonMany(all, 2, 2, "Dual Splash: choose 2 of your opponent's Pokémon to do 50 damage to");
                foreach (var t in targets) ctx.DamageTarget(t, 50);
                return true;
            }
        }

        // ================================================================== Lightning

        /// <summary>Shorting Spark: 90 damage to each of your opponent's Pokémon that has a Pokémon Tool attached.</summary>
        private sealed class ShortingSparkMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shorting Spark") return false;
                foreach (var t in ctx.OpponentPokemon(m => m.Tools.Count > 0)) ctx.DamageTarget(t, 90);
                return true;
            }
        }

        /// <summary>Electrostep: 40 damage to 1 of your opponent's Pokémon, then switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class ElectrostepMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Electrostep") return false;
                ctx.DamageAnyOpponent(40);
                if (ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Explosiveness: may start face down as the Active Pokémon. Seeking Fang: search for up to 2 Trainer cards.</summary>
        private sealed class ExplosivenessLuxray : Mon
        {
            private static readonly Mon seekingFang = new SearchAttackMon("Seeking Fang", d => d.IsTrainer, 2);

            public override bool CanStartAsActive { get { return true; } }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return seekingFang.HandleAttack(ctx, atk); }
        }

        /// <summary>Instant Charge: draw 3 cards, then your turn ends. Scrap Short: Tools from your discard pile to the Lost Zone, 40 more each.</summary>
        private sealed class RotomV : Mon
        {
            protected override string Activated { get { return "Instant Charge"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Draw(3);
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrap Short") return false;
                ScrapAttack(ctx, atk);
                return true;
            }
        }

        /// <summary>
        /// Conversion Star (VSTAR Power): discard any number of cards from your hand, then draw that many; it needs at least 1 card in
        /// your hand and 1 in your deck (Compendium, 2023-01-19). Scrap Pulse.
        /// </summary>
        private sealed class RotomVstar : Mon
        {
            protected override string Activated { get { return "Conversion Star"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count > 0 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx)
            {
                var picks = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, ctx.Me.Hand.Count, "Conversion Star: discard any number of cards from your hand, then draw that many");
                foreach (var c in picks) ctx.Engine.DiscardFromHand(ctx.Player, c);
                if (picks.Count > 0) ctx.Draw(picks.Count);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scrap Pulse") return false;
                ScrapAttack(ctx, atk);
                return true;
            }
        }

        /// <summary>Ad Hoc Shock: when played from the hand to evolve, flip a coin; on heads the opponent's Active Pokémon is Paralyzed.</summary>
        private sealed class EelektrikCRZ : Mon
        {
            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                if (ctx.Opp.Active == null) return;
                if (!TriggerOnce(ctx, "Ad Hoc Shock", true, "Use Ad Hoc Shock? Flip a coin: on heads, your opponent's Active Pokémon becomes Paralyzed.")) return;
                var target = ctx.Opp.Active;
                if (ctx.Flip() && target != null) ctx.ApplyCondition(target, SpecialCondition.Paralyzed);
            }
        }

        /// <summary>Shocking Block: whenever any player attaches an Energy card from their hand to 1 of their Pokémon V, 2 damage counters on it.</summary>
        private sealed class RadiantCharjabug : Mon
        {
            private static readonly Mon linear = new CardsMEG.SnipeMon("Linear Attack", 30);

            public override void OnOwnerAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Shock(src, target); }

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target) { Shock(src, target); }

            private static void Shock(EffectContext src, PokemonInPlay target)
            {
                if (target == null || !target.Def.IsPokemonV || !src.InPlay(target)) return;
                src.Engine.PlaceDamageCounters(target, 2, "Shocking Block");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return linear.HandleAttack(ctx, atk); }
        }

        /// <summary>Reactive Pulse: 60 damage for each of your opponent's Pokémon in play that has an Ability.</summary>
        private sealed class ReactivePulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Reactive Pulse") return false;
                ctx.DamageDefending(atk.Damage * ctx.OpponentPokemon(m => ctx.Engine.HasAbility(m)).Count);
                return true;
            }
        }

        /// <summary>
        /// Crushing Beat: you may discard a Stadium in play. Lightning Storm Star (VSTAR Power): choose 1 of the opponent's
        /// Pokémon 4 times; each Pokémon takes 60 for each time it was chosen, as one hit (your ruling), with no Weakness or Resistance.
        /// </summary>
        private sealed class ZeraoraVstar : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Crushing Beat")
                {
                    ctx.DamageDefending(atk.Damage);
                    var stadium = ctx.State.Stadium;
                    if (stadium != null && ctx.AskYesNo("Crushing Beat: discard " + stadium.Def.Name + "?")) ctx.Engine.DiscardStadium();
                    return true;
                }
                if (atk.Name == "Lightning Storm Star")
                {
                    var times = new Dictionary<int, int>();
                    var order = new List<PokemonInPlay>();
                    for (int i = 0; i < 4; i++)
                    {
                        var all = ctx.OpponentPokemon();
                        if (all.Count == 0) break;
                        var chosen = ctx.ChoosePokemon(all, "Lightning Storm Star: choose 1 of your opponent's Pokémon (" + (i + 1) + " of 4; you can choose the same one again)");
                        if (chosen == null) break;
                        if (!times.ContainsKey(chosen.Uid))
                        {
                            times[chosen.Uid] = 0;
                            order.Add(chosen);
                        }
                        times[chosen.Uid]++;
                    }
                    foreach (var mon in order) ctx.DamageWith(mon, 60 * times[mon.Uid], false, false, false);
                    return true;
                }
                return false;
            }
        }

        // ================================================================== Psychic

        /// <summary>Powerful Storm: 20 damage for each Energy attached to all of your Pokémon.</summary>
        internal sealed class PowerfulStormMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Powerful Storm") return false;
                int n = ctx.MyPokemon().Sum(m => ctx.EnergyCount(m));
                ctx.DamageDefending(atk.Damage * n);
                return true;
            }
        }

        /// <summary>Limit Break: 90 more damage if your opponent has 3 or fewer Prize cards remaining. Used for Limit Break, Blazing Charge (the attack name and the numbers are passed in).</summary>
        internal sealed class LimitBreakMon : Mon
        {
            private readonly string attack;
            private readonly int amount;
            private readonly int amount2;

            public LimitBreakMon(string attack, int amount, int amount2)
            {
                this.attack = attack;
                this.amount = amount;
                this.amount2 = amount2;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage + (ctx.Opp.Prizes.Count <= amount ? amount2 : 0));
                return true;
            }
        }

        /// <summary>Energy Mix: search for an Energy card and attach it to 1 of your Fusion Strike Pokémon. Psychic Leap: may shuffle this Pokémon into the deck.</summary>
        private sealed class MewV : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Energy Mix")
                {
                    if (ctx.Me.Deck.Count == 0) return true;
                    if (ctx.MyPokemon(m => m.Def.IsFusionStrike).Count == 0)
                    {
                        // No Fusion Strike Pokémon to attach to: the deck is only shuffled.
                        ctx.ShuffleMyDeck();
                        return true;
                    }
                    var found = ctx.SearchMyDeck(d => d.IsEnergy, 1, "Energy Mix: search your deck for an Energy card");
                    foreach (var card in found)
                    {
                        var target = ctx.ChoosePokemon(ctx.MyPokemon(m => m.Def.IsFusionStrike), "Energy Mix: attach " + card.Def.Name + " to which Fusion Strike Pokémon?");
                        ctx.Engine.AttachEnergyCard(card, target);
                    }
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Psychic Leap")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.InPlay(ctx.Pokemon) && ctx.AskYesNo("Psychic Leap: shuffle this Pokémon and all attached cards into your deck?"))
                        ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                    return true;
                }
                return false;
            }
        }

        /// <summary>Horoscope: look at the top 3 cards, attach any Energy cards there to this Pokémon, put the rest back in any order.</summary>
        private sealed class HoroscopeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Horoscope") return false;
                var me = ctx.Pokemon;
                var top = ctx.LookAtTop(3);
                if (top.Count == 0) return true;
                var energy = top.Where(c => c.Def.IsEnergy).ToList();
                var picks = energy.Count == 0 ? new List<CardInstance>()
                    : ctx.ChooseCards(energy, 0, energy.Count, "Horoscope: attach any number of these Energy cards to " + me.Name);
                foreach (var card in top) ctx.Me.Deck.Remove(card);
                foreach (var card in picks) ctx.Engine.AttachEnergyCard(card, me);
                var rest = top.Where(c => !picks.Contains(c)).ToList();
                PutOnTopInOrder(ctx, rest, "Horoscope: put the other cards back");
                return true;
            }
        }

        /// <summary>Witch's Domain: once during your turn, move up to 2 damage counters from your Pokémon to the opponent's Active. G-Max Smite: Confused.</summary>
        private sealed class HattereneVmax : Mon
        {
            private static readonly Mon smite = new ConditionAttackMon("G-Max Smite", SpecialCondition.Confused);

            protected override string Activated { get { return "Witch's Domain"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Active != null && !ctx.Engine.CounterMovementBlocked() && ctx.MyPokemon(m => m.DamageCounters > 0).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                for (int i = 0; i < 2; i++)
                {
                    var damaged = ctx.MyPokemon(m => m.DamageCounters > 0);
                    var target = ctx.Opp.Active;
                    if (damaged.Count == 0 || target == null) break;
                    if (i > 0 && !ctx.AskYesNo("Witch's Domain: move another damage counter?")) break;
                    var from = ctx.ChoosePokemon(damaged, "Witch's Domain: move a damage counter from which of your Pokémon to " + target.Name + "?");
                    if (from == null) break;
                    ctx.Engine.MoveDamageCounters(from, target, 1, "Witch's Domain");
                }
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return smite.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fighting

        /// <summary>Sun Energy: once during your turn, attach a Psychic Energy card from your discard pile to 1 of your Lunatone.</summary>
        private sealed class SolrockCRZ : Mon
        {
            protected override string Activated { get { return "Sun Energy"; } }

            private static List<CardInstance> Energy(EffectContext ctx)
            {
                return ctx.Me.Discard.Where(c => CardsSSP1.IsBasicOf(c.Def, EnergyType.Psychic)).ToList();
            }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return Energy(ctx).Count > 0 && ctx.MyPokemon(m => m.Name == "Lunatone").Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var pile = Energy(ctx);
                var card = pile.Count == 1 ? pile[0] : ctx.ChooseCards(pile, 1, 1, "Sun Energy: attach which Psychic Energy from your discard pile?")[0];
                var lunatones = ctx.MyPokemon(m => m.Name == "Lunatone");
                var target = lunatones.Count == 1 ? lunatones[0] : ctx.ChoosePokemon(lunatones, "Sun Energy: attach it to which Lunatone?");
                if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
            }
        }

        /// <summary>Invite Out: flip a coin; on heads, switch 1 of your opponent's Benched Pokémon with their Active Pokémon.</summary>
        internal sealed class InviteOutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Invite Out") return false;
                if (ctx.Flip()) ctx.GustOpponent("Invite Out: choose 1 of your opponent's Benched Pokémon to switch in");
                return true;
            }
        }

        /// <summary>Midnight Fang: 240, minus 80 for each Energy attached to your opponent's Active Pokémon. Used for Midnight Fang, Windup Swing (the attack name and the numbers are passed in).</summary>
        internal sealed class MidnightFangMon : Mon
        {
            private readonly string attack;
            private readonly int each;

            public MidnightFangMon(string attack, int each)
            {
                this.attack = attack;
                this.each = each;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                int energy = ctx.Defending == null ? 0 : ctx.EnergyCount(ctx.Defending);
                ctx.DamageDefending(Math.Max(0, atk.Damage - each * energy));
                return true;
            }
        }

        // ================================================================== Darkness

        /// <summary>Lost Claw: put a random card from your opponent's hand in the Lost Zone.</summary>
        private sealed class LostClawMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lost Claw") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                var card = ctx.Opp.Hand[ctx.State.Rng.Next(ctx.Opp.Hand.Count)];
                ctx.Opp.Hand.Remove(card);
                PutInLostZone(ctx, card);
                return true;
            }
        }

        /// <summary>Rocket Poison: Poisoned; if this Pokémon evolved from Skrelp this turn, 8 damage counters instead of 1 in Pokémon Checkup.</summary>
        private sealed class RocketPoisonMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Rocket Poison") return false;
                var defending = ctx.Defending;
                if (defending == null) return true;
                bool justEvolved = ctx.Pokemon.TurnEvolved == ctx.Turn && ctx.Pokemon.Stack.Count >= 2 && ctx.Pokemon.Stack[ctx.Pokemon.Stack.Count - 2].Def.Name == "Skrelp";
                if (ctx.ApplyCondition(defending, SpecialCondition.Poisoned) && justEvolved) defending.PoisonCounterBase = 8;
                return true;
            }
        }

        /// <summary>Assault Gate: does nothing unless this Pokémon moved from the Bench to the Active Spot this turn; no Weakness.</summary>
        private sealed class AssaultGateMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Assault Gate") return false;
                if (ctx.Pokemon.MovedToActiveOnTurn != ctx.Turn) return Nothing(ctx);
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage, false, true, false);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Wreak Havoc: flip a coin until you get tails; discard the top card of your opponent's deck for each heads.</summary>
        private sealed class WreakHavocMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Wreak Havoc") return false;
                ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipUntilTails();
                if (heads > 0) ctx.Engine.DiscardTopOfDeck(ctx.Opponent, heads);
                return true;
            }
        }

        /// <summary>Counter Press: during the opponent's next turn, damage to this Pokémon from an attack is returned as damage counters on the attacker.</summary>
        private sealed class CounterPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Counter Press") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.InPlay(ctx.Pokemon))
                    ctx.AddEffect(new CardsSVP.MirrorDamageEffect(ctx.Pokemon.Name + " returns damage from attacks as damage counters (Counter Press)", ctx.Player, ctx.Pokemon.Uid, ctx.Turn + 1));
                return true;
            }
        }

        /// <summary>Battle Legion: 10 more for each of your Benched Pokémon; not affected by Weakness or by effects on the Defending Pokémon.</summary>
        private sealed class BattleLegionMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Battle Legion") return false;
                if (ctx.Defending != null) ctx.DamageWith(ctx.Defending, atk.Damage + 10 * ctx.Me.Bench.Count, false, true, true);
                return true;
            }
        }

        /// <summary>Metal Shield: with any Energy attached, it takes 30 less damage from attacks. Retaliate: 120 more after a Knock Out last turn.</summary>
        private sealed class ZamazentaCRZ : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && src.Pokemon.Energy.Count > 0 ? damage - 30 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Retaliate") return false;
                ctx.DamageDefending(atk.Damage + (ctx.WasKnockedOutLastOpponentTurn() ? 120 : 0));
                return true;
            }
        }

        /// <summary>Regal Stance: discard your hand, draw 5 cards, your turn ends. Revenge Blast: 30 more for each Prize card the opponent has taken.</summary>
        private sealed class ZamazentaV : Mon
        {
            protected override string Activated { get { return "Regal Stance"; } }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(5);
                ctx.Engine.RequestEndTurn();
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Revenge Blast") return false;
                ctx.DamageDefending(atk.Damage + 30 * ctx.OpponentPrizesTaken);
                return true;
            }
        }

        /// <summary>Shield Star (VSTAR Power): during the opponent's next turn, all your Pokémon take 100 less damage from their attacks. Giga Impact.</summary>
        private sealed class ZamazentaVstar : Mon
        {
            private static readonly Mon gigaImpact = new LockAfterAttackMon("Giga Impact", false);

            protected override string Activated { get { return "Shield Star"; } }

            protected override void Ability(EffectContext ctx)
            {
                ctx.AddEffect(new TeamShieldEffect("Shield Star: your Pokémon take 100 less damage from the opponent's attacks during their next turn", ctx.Player, ctx.Turn + 1, 100, m => true));
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return gigaImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// "All of your Pokémon take N less damage from attacks from your opponent's Pokémon" for a while, including Pokémon that
        /// come into play later (Shield Star). attackerFilter limits which attackers it applies to.
        /// </summary>
        internal sealed class TeamShieldEffect : ContinuousEffect
        {
            private readonly int amount;
            private readonly Func<PokemonInPlay, bool> attackerFilter;

            public override bool IsOnPlayer(int player) { return player == Owner; }

            public TeamShieldEffect(string name, int owner, int expiresAfterTurn, int amount, Func<PokemonInPlay, bool> attackerFilter)
                : base(name, owner, -1, expiresAfterTurn, false)
            {
                this.amount = amount;
                this.attackerFilter = attackerFilter;
            }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target.Owner != Owner || info.Attacker == null || info.Attacker.Owner == Owner || !attackerFilter(info.Attacker)) return damage;
                return damage - amount;
            }
        }

        // ================================================================== Dragon

        /// <summary>Dragon Pulse: discard the top 2 cards of your deck.</summary>
        internal sealed class DragonPulseMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dragon Pulse") return false;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.Engine.DiscardTopOfDeck(ctx.Player, 2);
                return true;
            }
        }

        /// <summary>Azure Pulse: once during your turn, discard your hand and draw 3 cards. Max Burst: any amount of basic Fire or Lightning Energy.</summary>
        private sealed class RayquazaVmax : Mon
        {
            private static readonly Mon maxBurst = new BurstMon("Max Burst", 99);

            protected override string Activated { get { return "Azure Pulse"; } }

            protected override void Ability(EffectContext ctx)
            {
                ctx.Engine.DiscardHand(ctx.Player);
                ctx.Draw(3);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return maxBurst.HandleAttack(ctx, atk); }
        }

        /// <summary>Skyscraper: prevent all damage done to it by attacks from the opponent's Pokémon that have Special Energy attached. G-Max Pulverization.</summary>
        private sealed class DuraludonVmax : Mon
        {
            private static readonly Mon pulverization = new CardsPAF2.IgnoreDefenderEffectsMon("G-Max Pulverization");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var a = info.Attacker;
                if (info.Target != src.Pokemon || a == null || a.Owner == src.Player) return damage;
                return a.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return pulverization.HandleAttack(ctx, atk); }
        }

        /// <summary>Climactic Gate: when played from your hand to your Bench, you may put up to 2 Pokémon VMAX from your deck onto your Bench; your turn ends.</summary>
        private sealed class RadiantEternatus : Mon
        {
            public override void OnPlayedFromHandToBench(EffectContext ctx)
            {
                if (ctx.Me.Deck.Count == 0 || ctx.BenchSpace == 0) return;
                if (!TriggerOnce(ctx, "Climactic Gate", true, "Use Climactic Gate? Put up to 2 Pokémon VMAX from your deck onto your Bench. Your turn ends.")) return;
                int n = Math.Min(2, ctx.BenchSpace);
                var found = ctx.SearchMyDeck(d => d.IsPokemon && d.IsVmax, n, "Climactic Gate: choose up to " + n + " Pokémon VMAX to put onto your Bench");
                foreach (var c in found)
                {
                    if (ctx.BenchSpace == 0) { ctx.Me.Deck.Add(c); continue; }
                    ctx.Engine.PutOntoBench(ctx.Player, c, false);
                }
                ctx.ShuffleMyDeck();
                ctx.Engine.RequestEndTurn();
            }
        }

        // ================================================================== Colorless

        /// <summary>Adrena-Tackle: does nothing if this Pokémon has no damage counters on it. Used for Adrena-Tackle, Hit Back (the attack name is passed in).</summary>
        internal sealed class AdrenaTackleMon : Mon
        {
            private readonly string attack;

            public AdrenaTackleMon(string attack)
            {
                this.attack = attack;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (ctx.Pokemon.DamageCounters == 0) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>
        /// Sudden Transformation: it can use the attacks of any Basic Pokémon in your discard pile that doesn't have a Rule Box
        /// (it still needs the Energy for them).
        /// </summary>
        private sealed class DittoCRZ : Mon
        {
            public override IEnumerable<KeyValuePair<CardDef, int>> ExtraAttacks(EffectContext src)
            {
                var seen = new HashSet<string>();
                foreach (var c in src.Me.Discard)
                {
                    var d = c.Def;
                    if (!d.IsBasicPokemon || d.HasRuleBox || !seen.Add(d.BaseKey)) continue;
                    for (int i = 0; i < d.Attacks.Count; i++) yield return new KeyValuePair<CardDef, int>(d, i);
                }
            }
        }

        /// <summary>Carefree Countenance: while on your Bench, prevent all damage done to it by attacks (anyone's). Hyper Fang: tails, nothing.</summary>
        private sealed class BidoofCRZ : Mon
        {
            private static readonly Mon hyperFang = new TailsNothingMon("Hyper Fang");

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                return info.Target == src.Pokemon && info.Attacker != null && !src.IsActive(src.Pokemon) ? 0 : damage;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return hyperFang.HandleAttack(ctx, atk); }
        }

        /// <summary>Angry Whack: 10 more for each damage counter on this Pokémon, then this Pokémon is Confused.</summary>
        private sealed class AngryWhackMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Angry Whack") return false;
                var me = ctx.Pokemon;
                ctx.DamageDefending(atk.Damage + 10 * me.DamageCounters);
                if (ctx.InPlay(me) && ctx.IsActive(me)) ctx.ApplyCondition(me, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Star Guardian (VSTAR Power): if your opponent has exactly 1 Prize card left, they discard 1 of their Benched Pokémon (your choice).</summary>
        private sealed class RegigigasVstar : Mon
        {
            private static readonly Mon gigaImpact = new LockAfterAttackMon("Giga Impact", false);

            protected override string Activated { get { return "Star Guardian"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Opp.Prizes.Count == 1 && ctx.Opp.Bench.Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                var targets = ctx.OpponentBench().Where(m => ctx.CanAffect(m)).ToList();
                if (targets.Count == 0) return;
                var target = ctx.ChoosePokemon(targets, "Star Guardian: choose 1 of your opponent's Benched Pokémon for them to discard");
                if (target != null) ctx.Engine.DiscardPokemonFromPlay(target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return gigaImpact.HandleAttack(ctx, atk); }
        }

        /// <summary>Gather Flowers: shuffle up to 2 Energy cards from your discard pile into your deck.</summary>
        private sealed class GatherFlowersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Gather Flowers") return false;
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var picks = ctx.ChooseCards(energy, 0, Math.Min(2, energy.Count), "Gather Flowers: shuffle up to 2 Energy cards from your discard pile into your deck");
                if (picks.Count == 0) return true;
                ctx.DiscardToDeck(picks);
                ctx.ShuffleMyDeck();
                return true;
            }
        }

        /// <summary>Double Dip Fangs: take 1 more Prize card if this attack's damage Knocks Out your opponent's Basic Pokémon.</summary>
        private sealed class DoubleDipFangsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Double Dip Fangs") return false;
                int owner = ctx.Player;
                int attackerUid = ctx.Pokemon.Uid;
                ctx.AddEffect(new ExtraPrizeEffect("Double Dip Fangs: 1 more Prize card for a Basic Pokémon Knocked Out by this attack", owner, ctx.Turn,
                    (e, ko) => ko.HitByAttack && ko.HitByPlayer == owner && ko.HitByAttackerUid == attackerUid && !ko.ForcedKnockOut && ko.IsBasic));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Stakeout Headbutt: 120 more if the opponent's Active Pokémon moved from the Bench to the Active Spot during their last turn.</summary>
        private sealed class StakeoutHeadbuttMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stakeout Headbutt") return false;
                bool moved = ctx.Defending != null && ctx.Defending.MovedToActiveOnTurn == ctx.OpponentsLastTurn;
                ctx.DamageDefending(atk.Damage + (moved ? 120 : 0));
                return true;
            }
        }

        /// <summary>
        /// Primate Acting: choose a Supporter card in your opponent's discard pile and use its effect as this attack's effect.
        /// Only Supporters whose effect could be used right now are offered.
        /// </summary>
        internal sealed class PrimateActingMon : Mon
        {
            private readonly string attack;

            /// <summary>Also Sableye's Bewitching Eyes (Ancient Origins, v98).</summary>
            public PrimateActingMon(string attack) { this.attack = attack; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var supporter in ctx.Opp.Discard)
                {
                    if (!supporter.Def.IsSupporter || !seen.Add(supporter.Def.Name)) continue;
                    if (ctx.Engine.Scripts.For(supporter.Def).CanPlayTrainer(new EffectContext(ctx.Engine, ctx.Player, supporter, null))) usable.Add(supporter);
                }
                if (usable.Count == 0)
                {
                    ctx.Log(attack + ": there's no Supporter in your opponent's discard pile whose effect can be used.");
                    return true;
                }
                var labels = usable.Select(c => "Use " + c.Def.Name + "'s effect").ToArray();
                int pick = ctx.ChooseOption(attack + ": choose a Supporter in your opponent's discard pile and use its effect", labels);
                if (pick < 0 || pick >= usable.Count) return true;
                var card = usable[pick];
                ctx.Log(ctx.Pokemon.Name + " uses the effect of " + card.Def.Name + ".");
                ctx.Engine.Scripts.For(card.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null));
                return true;
            }
        }

        /// <summary>Overhead Throw: also 10 damage to 1 of your Benched Pokémon.</summary>
        private sealed class OverheadThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Overhead Throw") return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.MyBench();
                if (bench.Count == 0) return true;
                var target = bench.Count == 1 ? bench[0] : ctx.ChoosePokemon(bench, "Overhead Throw: do 10 damage to which of your Benched Pokémon?");
                ctx.DamageTarget(target, 10);
                return true;
            }
        }
    }
}
