using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Cosmic Eclipse (CEC) part 3: Darkness, Metal, Fairy, Dragon and Colorless (CEC 128-184). Rulings used: Altered Creation-GX's
    /// extra Prize applies to every opponent's Active Pokémon your attacks Knock Out for the rest of the game (Compendium, 2019-10-31);
    /// Red Banquet's extra Prize is a game effect (2021-04-08); Flower Picking does as much as it can (2019-10-31); Violent Appetite
    /// works on an undamaged Pokémon (2020-02-27).
    /// </summary>
    public static class CardsCEC3
    {
        public static void Register(CardScriptRegistry r)
        {
            // Darkness
            r.RegisterByKey("CEC-128", new CardsASC.FlipDiscardDefendingEnergyMon("Swagger"));
            r.RegisterByKey("CEC-129", new AlolanPersianGX());
            r.RegisterByKey("CEC-130", new ParalyzeFlipMon("Nasty Goo"));
            r.RegisterByKey("CEC-131", new CardsCES1.ConditionsMon("Panic Poison", SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Poisoned));
            r.RegisterByKey("CEC-132", new DodgeMon("Agility", false));
            r.RegisterByKey("CEC-133", new ComboMon(new OminousNewsMon(), new DirtyThrowMon()));
            r.RegisterByKey("CEC-134", new BagSlashMon());
            r.RegisterByKey("CEC-135", new ComboMon(new NoRetreatMon("Corner"), new LockAfterAttackMon("Slashing Strike", true)));
            r.RegisterByKey("CEC-136", new ComboMon(new CardsSSP2.StompOffMon("Mountain Munch", 1), new RedBanquetMon()));
            // Metal
            r.RegisterByKey("CEC-137", new SwitchSelfMon("Run Around", false));
            r.RegisterByKey("CEC-138", new ComboMon(new CardsSP1.FlipNoDamageMon("Curve Strike"), new CardsASC.ToolBonusMon("Reinforced Needle", 60)));
            r.RegisterByKey("CEC-139", new ComboMon(new CardsFST1.DiscardFromHandTimesMon("Thumping Fall", RetreatFour, 60, "Pokémon with a Retreat Cost of exactly 4"), new CardsPFL.FlipUntilTailsTimesMon("Iron Tail")));
            r.RegisterByKey("CEC-140", new ComboMon(new SearchAttackMon("Dual Calling", IsTagTeamCard, 2), new NoRetreatMon("Dark Clamp")));
            r.RegisterByKey("CEC-141", new ComboMon(new ParalyzeFlipMon("Hard Press"), new CardsWHT.FlipMoreMon("Triple Nose", 3, 40)));
            r.RegisterByKey("CEC-142", new SolgaleoCEC());
            // Fairy
            r.RegisterByKey("CEC-143", new ComboMon(new CardsMEG.FlipUntilTailsBonusMon("Rolling Panic", 30), new SupremePuffMon()));
            r.RegisterByKey("CEC-144", new DollSwapMon());
            r.RegisterByKey("CEC-145", new CardsASR1.TimesMon("Rubbish Blizzard", ToolsInDiscard));
            r.RegisterByKey("CEC-146", new AzurillCEC());
            r.RegisterByKey("CEC-147", new CardsASR1.TimesMon("Lost March", LostMarchCount));
            r.RegisterByKey("CEC-148", new ComboMon(new SneakyPocketMon(), new CardsASR1.TimesMon("Lost March", LostMarchCount)));
            r.RegisterByKey("CEC-149", new SearchAttackMon("Floral Invitation", IsFairyPokemon, 2));
            r.RegisterByKey("CEC-150", new SpreadAmountMon("Petal Blizzard", 10));
            r.RegisterByKey("CEC-151", new FlowerPickingMon(1, null));
            r.RegisterByKey("CEC-152", new FlowerPickingMon(2, new PetalDanceMon()));
            r.RegisterByKey("CEC-153", new ShieldAttackMon("Cotton Guard", 10));
            r.RegisterByKey("CEC-154", new ComboMon(new ConditionAttackMon("Olfactory Enchantment", SpecialCondition.Confused), new SweetPanicMon()));
            r.RegisterByKey("CEC-155", new ComboMon(new CardsMEG.WeakenDefendingMon("Moonblast", 30), new CardsFST2.ConditionBonusMon("Beloved Pulse", 80, TagTeamSupporterPlayed)));
            // Dragon
            r.RegisterByKey("CEC-156", new ArceusDialgaPalkiaGX());
            r.RegisterByKey("CEC-157", new ReshiramZekromGX());
            r.RegisterByKey("CEC-158", new NaganadelGuzzlordGX());
            r.RegisterByKey("CEC-159", new CardsFST2.ConditionBonusMon("Dragon Arcana", 70, TwoBasicEnergyTypes));
            r.RegisterByKey("CEC-160", new Cards30C.SelfCounterBonusMon("Raging Claws", 10));
            r.RegisterByKey("CEC-162", new HakamooCEC());
            r.RegisterByKey("CEC-163", new ComboMon(new ShoutOfPowerMon(), new ScalyUppercutMon()));
            r.RegisterByKey("CEC-164", new UltraNecrozmaCEC());
            // Colorless
            r.RegisterByKey("CEC-165", new ComboMon(new CardsASR1.PlusMon("Jumping Balloon", 60, OpponentGXorEXCount), new PuffySmashersMon()));
            r.RegisterByKey("CEC-166", new SearchAttackMon("Lead", IsSupporterCard, 1));
            r.RegisterByKey("CEC-167", new FollowMyTailMon());
            r.RegisterByKey("CEC-168", new IgglybuffCEC());
            r.RegisterByKey("CEC-169", new AipomCEC());
            r.RegisterByKey("CEC-170", new ComboMon(new DrawAttackMon("Nice-Nice Catch", 2), new CardsFST1.DiscardFromHandTimesMon("Bye-Bye Throw", CardsCEC1.AnyCard, 2, "cards")));
            r.RegisterByKey("CEC-172", new CardsBLK.NoAttackNextTurnMon("Heavy Hold"));
            r.RegisterByKey("CEC-173", new CardsMEW1.CounterBonusMon("Brutal Edge", 10));
            r.RegisterByKey("CEC-174", new NoRetreatMon("Baby-Doll Eyes"));
            r.RegisterByKey("CEC-175", new CardsFST1.NextTurnBonusMon("Work Up", 60));
            r.RegisterByKey("CEC-176", new StoutlandCEC());
            r.RegisterByKey("CEC-177", new CoinsMon("Fury Attack", 3));
            r.RegisterByKey("CEC-178", new ComboMon(new NoRetreatMon("Clutch"), new CardsTWM1.SelfDiscardMon("Aero Fall", 2)));
            r.RegisterByKey("CEC-179", new CardsDAA1.FlipCantAttackMon("Tail Whip"));
            r.RegisterByKey("CEC-180", new CardsFST2.ConditionBonusMon("Parabolic Counter", 90, OpponentHasLightning));
            r.RegisterByKey("CEC-181", new CardsMEG.FlipUntilTailsBonusMon("Continuous Tumble", 30));
            r.RegisterByKey("CEC-182", new BewearCEC());
            r.RegisterByKey("CEC-183", new CardsTWM1.SelfDiscardMon("Air Slash", 1));
            r.RegisterByKey("CEC-184", new SilvallyGX());
        }

        private static bool RetreatFour(CardDef d) { return d.IsPokemon && d.RetreatCost == 4; }
        internal static bool IsTagTeamCard(CardDef d) { return d.IsTagTeam; }
        private static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }
        private static bool IsFairyPokemon(CardDef d) { return d.IsPokemon && d.HasType(EnergyType.Fairy); }
        private static int ToolsInDiscard(EffectContext ctx) { return ctx.Me.Discard.Count(c => c.Def.IsTool); }
        /// <summary>Your Pokémon in the Lost Zone, except Prism Star (◇) Pokémon.</summary>
        private static int LostMarchCount(EffectContext ctx) { return ctx.Me.LostZone.Count(c => c.Def.IsPokemon && !c.Def.Name.EndsWith("◇")); }
        private static bool TagTeamSupporterPlayed(EffectContext ctx) { return ctx.Me.SupporterThisTurn != null && ctx.Me.SupporterThisTurn.IsTagTeam; }
        private static int OpponentGXorEXCount(EffectContext ctx) { return ctx.OpponentPokemon().Count(m => m.Def.IsGXorEX); }
        private static bool OpponentHasLightning(EffectContext ctx) { return ctx.OpponentPokemon().Any(m => ctx.Engine.IsType(m, EnergyType.Lightning)); }

        private static bool TwoBasicEnergyTypes(EffectContext ctx)
        {
            return ctx.Pokemon.Energy.Where(e => e.Def.IsBasicEnergy).Select(e => e.Def.EnergyProvides).Distinct().Count() >= 2;
        }

        /// <summary>Altered Creation-GX: the player's attacks do 30 more damage to the opponent's Active Pokémon for the rest of the game.</summary>
        internal sealed class AlteredCreationEffect : ContinuousEffect
        {
            public AlteredCreationEffect(string name, int owner) : base(name, owner, -1, int.MaxValue, false) { }

            public override bool IsOnPlayer(int player) { return player == Owner; }

            public override int ModifyDamageBeforeWeakness(GameEngine engine, PokemonInPlay attacker, PokemonInPlay target, int damage)
            {
                if (attacker == null || target == null || attacker.Owner != Owner || target.Owner == Owner || !engine.State.IsActive(target)) return damage;
                return damage + 30;
            }
        }

        /// <summary>The opponent's Active Pokémon was Knocked Out by damage from the other player's attack this turn.</summary>
        private static bool ActiveKnockedOutByAttack(GameEngine engine, PokemonInPlay mon)
        {
            return engine.State.IsActive(mon) && mon.HitByAttack && mon.HitByPlayer != mon.Owner && mon.AttackDamageTurn == engine.State.TurnNumber && engine.State.CurrentPlayer != mon.Owner;
        }

        // ================================================================== Darkness

        /// <summary>
        /// Smug Face: prevent all effects of attacks, damage included, done to it by the opponent's TAG TEAM Pokémon, Ultra Beasts and
        /// Pokémon with Special Energy attached. Stalking Claws-GX: 120 to 1 of the opponent's Pokémon, ignoring Weakness, Resistance and
        /// effects on it.
        /// </summary>
        private sealed class AlolanPersianGX : Mon
        {
            private static bool Feared(PokemonInPlay attacker)
            {
                return attacker != null && (attacker.Def.IsTagTeam || attacker.Def.IsUltraBeast || attacker.Energy.Any(e => e.Def.IsAttachedSpecialEnergy));
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                var me = src.Pokemon;
                return me != null && info.Target == me && info.AgainstOpponent && Feared(info.Attacker);
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                var me = src.Pokemon;
                return me != null && target == me && kind == EffectSourceKind.Attack && Feared(src.Engine.CurrentSourcePokemon);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stalking Claws-GX") return false;
                var all = ctx.OpponentPokemon();
                if (all.Count == 0) return true;
                var target = ctx.ChoosePokemon(all, "Stalking Claws-GX: 120 damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageWith(target, 120, false, false, true);
                return true;
            }
        }

        /// <summary>Ominous News: discard a Special Energy from 1 of the opponent's Pokémon.</summary>
        private sealed class OminousNewsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Ominous News") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var targets = ctx.OpponentPokemon(m => m.Energy.Any(e => e.Def.IsAttachedSpecialEnergy) && ctx.CanAffect(m));
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Ominous News: discard a Special Energy from which of your opponent's Pokémon?");
                if (target != null) ctx.DiscardEnergyFrom(target, 1, "Ominous News: discard which Special Energy?", x => x.Def.IsAttachedSpecialEnergy);
                return true;
            }
        }

        /// <summary>Dirty Throw: discard a card from your hand; if you can't, this attack does nothing.</summary>
        private sealed class DirtyThrowMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Dirty Throw") return false;
                if (ctx.Me.Hand.Count == 0) return Nothing(ctx);
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Dirty Throw: discard a card from your hand")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        /// <summary>Bag Slash: your opponent reveals their hand; discard an Item card you find there.</summary>
        private sealed class BagSlashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Bag Slash") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Opp.Hand.Count == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var items = ctx.Opp.Hand.Where(c => c.Def.IsItem).ToList();
                if (items.Count == 0) return true;
                var pick = ctx.ChooseCards(items, 1, 1, "Bag Slash: discard which Item card from your opponent's hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Opponent, pick);
                return true;
            }
        }

        /// <summary>Red Banquet: if the opponent's Pokémon is Knocked Out by damage from this attack, take 1 more Prize card.</summary>
        private sealed class RedBanquetMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Red Banquet") return false;
                var defending = ctx.Defending;
                if (defending != null)
                    ctx.AddEffect(new KnockOutPrizeBonusEffect("Take 1 more Prize card if " + defending.Name + " is Knocked Out by Red Banquet", ctx.Player, defending.Uid, ctx.Turn, 1));
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Metal

        /// <summary>Armor of the Sunne: if you have Lunala in play, your Solgaleo and Lunala take 50 less damage from the opponent's attacks (only 1 applies). Sol Fangs: discard 2 Energy.</summary>
        private sealed class SolgaleoCEC : Mon
        {
            private static readonly Mon SolFangs = new CardsTWM1.SelfDiscardMon("Sol Fangs", 2);

            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                var t = info.Target;
                if (src.Pokemon == null || t.Owner != src.Player || !info.AgainstOpponent) return damage;
                if (t.Name != "Solgaleo" && t.Name != "Lunala") return damage;
                if (!src.MyPokemon().Any(m => m.Name == "Lunala") || !CardsCRZ1.IsLeadSource(src, "Armor of the Sunne")) return damage;
                return damage - 50;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return SolFangs.HandleAttack(ctx, atk); }
        }

        // ================================================================== Fairy

        /// <summary>
        /// Supreme Puff-GX: take another turn after this one, skipping Pokémon Checkup; with 14 extra Fairy Energy, the opponent shuffles
        /// all of their Benched Pokémon and the cards attached to them into their deck.
        /// </summary>
        private sealed class SupremePuffMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Supreme Puff-GX") return false;
                bool huge = CardsCEC1.ExtraTyped(ctx, atk, Enumerable.Repeat(EnergyType.Fairy, 14).ToArray());
                ctx.State.ExtraTurnFor = ctx.Player;
                ctx.State.SkipCheckup = true;
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " will take another turn after this one (Supreme Puff-GX).");
                if (!huge) return true;
                foreach (var mon in ctx.OpponentBench())
                    if (ctx.CanAffect(mon)) ctx.Engine.ShufflePokemonIntoDeck(mon);
                return true;
            }
        }

        /// <summary>Doll Swap: put it and its cards into your hand; if you do, you may play Lillie's Poké Doll from your hand as your new Active Pokémon.</summary>
        private sealed class DollSwapMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Doll Swap") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (!ctx.InPlay(me)) return true;
                bool wasActive = ctx.IsActive(me);
                ctx.Engine.ReturnPokemonToHand(me);
                if (ctx.InPlay(me) || !wasActive || ctx.Me.Active != null) return true;
                var doll = ctx.Me.Hand.FirstOrDefault(c => c.Def.IsPokeDoll);
                if (doll == null || !ctx.AskYesNo("Doll Swap: play Lillie's Poké Doll from your hand as your new Active Pokémon?")) return true;
                if (ctx.Me.Hand.Remove(doll)) ctx.Engine.PutOntoActive(ctx.Player, doll);
                return true;
            }
        }

        /// <summary>Growing Up: once during your turn, flip a coin; on heads, attach a basic Energy from your discard pile to your Active Pokémon. Your turn ends.</summary>
        private sealed class AzurillCEC : Mon
        {
            protected override string Activated { get { return "Growing Up"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    var energy = ctx.BasicEnergyInDiscard();
                    var active = ctx.Me.Active;
                    if (energy.Count > 0 && active != null)
                    {
                        var pick = ctx.ChooseCards(energy, 1, 1, "Growing Up: attach which basic Energy card to " + active.Name + "?")[0];
                        if (ctx.Me.Discard.Remove(pick)) ctx.Engine.AttachEnergyCard(pick, active);
                    }
                }
                ctx.Log("Growing Up: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Sneaky Pocket: put a card from your hand in the Lost Zone; if you do, draw 3 cards.</summary>
        private sealed class SneakyPocketMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sneaky Pocket") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Hand.Count == 0) return true;
                var card = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Sneaky Pocket: put which card from your hand in the Lost Zone?")[0];
                if (!ctx.Me.Hand.Remove(card)) return true;
                ctx.Me.LostZone.Add(card);
                ctx.Log("Sneaky Pocket: " + card.Def.Name + " was put in the Lost Zone.");
                ctx.Draw(3);
                return true;
            }
        }

        /// <summary>Petal Blizzard: 10 damage to each of the opponent's Pokémon. Used for Petal Blizzard, Aqua Shower (the attack name and the numbers are passed in).</summary>
        internal sealed class SpreadAmountMon : Mon
        {
            private readonly string attack;
            private readonly int amount;

            public SpreadAmountMon(string attack, int amount)
            {
                this.attack = attack;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                CardsCRZ1.DamageEachOpponent(ctx, amount);
                return true;
            }
        }

        /// <summary>
        /// Flower Picking (Floette 1, Florges 2): when you evolve into it from your hand, you may shuffle random cards from the opponent's
        /// hand into their deck (as many as possible; Compendium, 2019-10-31).
        /// </summary>
        private sealed class FlowerPickingMon : Mon
        {
            private readonly int count;
            private readonly Mon attacks;

            public FlowerPickingMon(int count, Mon attacks)
            {
                this.count = count;
                this.attacks = attacks;
            }

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                string what = count == 1 ? "a random card" : count + " random cards";
                if (!TriggerOnce(ctx, "Flower Picking", ctx.Opp.Hand.Count > 0, "Flower Picking: have your opponent shuffle " + what + " from their hand into their deck?")) return;
                CardsTWM1.ShuffleRandomFromOpponentHand(ctx, count);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return attacks != null && attacks.HandleAttack(ctx, atk); }
        }

        /// <summary>Petal Dance: 3 coins, 60 for each heads; this Pokémon is now Confused.</summary>
        internal sealed class PetalDanceMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Petal Dance") return false;
                int heads = ctx.FlipCoins(3);
                if (heads > 0) ctx.DamageDefending(atk.Damage * heads);
                if (ctx.InPlay(ctx.Pokemon) && ctx.IsActive(ctx.Pokemon)) ctx.ApplyCondition(ctx.Pokemon, SpecialCondition.Confused);
                return true;
            }
        }

        /// <summary>Sweet Panic: does nothing unless the opponent's Active Pokémon is Confused.</summary>
        private sealed class SweetPanicMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sweet Panic") return false;
                var defending = ctx.Defending;
                if (defending == null || !defending.Has(SpecialCondition.Confused)) return Nothing(ctx);
                ctx.DamageDefending(atk.Damage);
                return true;
            }
        }

        // ================================================================== Dragon

        /// <summary>
        /// Ultimate Ray: search for up to 3 basic Energy cards and attach them to your Pokémon in any way you like. Altered Creation-GX: your
        /// attacks do 30 more to the opponent's Active Pokémon for the rest of the game; with 1 extra Water Energy, you take 1 more Prize
        /// card whenever your attacks Knock Out their Active Pokémon.
        /// </summary>
        private sealed class ArceusDialgaPalkiaGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Ultimate Ray")
                {
                    ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(CardsCEC1.IsBasicEnergyCard, 3, "Ultimate Ray: search your deck for up to 3 basic Energy cards");
                    if (found.Count > 0) ctx.AttachCardsAnyWay(found, found.Count, CardsCEC1.AnyOfMine, "Ultimate Ray: attach a basic Energy card to 1 of your Pokémon", false);
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name != "Altered Creation-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Water);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new AlteredCreationEffect("Altered Creation-GX: " + ctx.Engine.PlayerName(ctx.Player) + "'s attacks do 30 more damage to the opponent's Active Pokémon", ctx.Player));
                if (extra)
                    ctx.AddEffect(new ExtraPrizeEffect("Altered Creation-GX: " + ctx.Engine.PlayerName(ctx.Player) + " takes 1 more Prize card for Knocking Out the opponent's Active Pokémon with an attack", ctx.Player, int.MaxValue, ActiveKnockedOutByAttack));
                return true;
            }
        }

        /// <summary>
        /// Fabled Flarebolts: discard up to 3 basic Fire and Lightning Energy cards from your Benched Pokémon, 90 for each. Cross Break-GX:
        /// 170 to 1 of the opponent's Benched Pokémon; 170 to another one too if you played N's Resolve from your hand this turn.
        /// </summary>
        private sealed class ReshiramZekromGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fabled Flarebolts")
                {
                    var cards = new List<CardInstance>();
                    foreach (var mon in ctx.MyBench())
                        foreach (var e in mon.Energy)
                            if (e.Def.IsBasicEnergy && (e.Def.EnergyProvides == EnergyType.Fire || e.Def.EnergyProvides == EnergyType.Lightning)) cards.Add(e);
                    int n = 0;
                    if (cards.Count > 0)
                    {
                        var picks = ctx.ChooseCards(cards, 0, Math.Min(3, cards.Count), "Fabled Flarebolts: discard up to 3 basic Fire and Lightning Energy cards from your Benched Pokémon (90 damage each)");
                        foreach (var pick in picks)
                        {
                            var holder = ctx.MyBench().FirstOrDefault(m => m.Energy.Contains(pick));
                            if (holder != null && ctx.Engine.DiscardEnergy(holder, pick)) n++;
                        }
                    }
                    if (n > 0) ctx.DamageDefending(atk.Damage * n);
                    else ctx.Log("Fabled Flarebolts did no damage: no Energy was discarded.");
                    return true;
                }
                if (atk.Name != "Cross Break-GX") return false;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                bool resolve = ctx.Me.SupporterThisTurn != null && ctx.Me.SupporterThisTurn.Name == "N's Resolve";
                int count = resolve ? Math.Min(2, bench.Count) : 1;
                var targets = bench.Count <= count ? bench : ctx.ChoosePokemonMany(bench, count, count, "Cross Break-GX: 170 damage to " + (count == 1 ? "which of your opponent's Benched Pokémon?" : "which 2 of your opponent's Benched Pokémon?"));
                foreach (var t in targets) ctx.DamageTarget(t, 170);
                return true;
            }
        }

        /// <summary>
        /// Violent Appetite: once during your turn, discard a Pokémon from your hand to heal 60 from it (usable undamaged). Chaotic Order-GX:
        /// turn all of your Prize cards face up for the rest of the game; with 1 extra Psychic and 1 extra Darkness Energy, take 2 Prize cards.
        /// </summary>
        private sealed class NaganadelGuzzlordGX : Mon
        {
            protected override string Activated { get { return "Violent Appetite"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => c.Def.IsPokemon); }

            protected override void Ability(EffectContext ctx)
            {
                var pokemon = ctx.Me.Hand.Where(c => c.Def.IsPokemon).ToList();
                var card = ctx.ChooseCards(pokemon, 1, 1, "Violent Appetite: discard which Pokémon from your hand?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, card);
                if (ctx.Me.Discard.Contains(card)) ctx.Heal(ctx.Pokemon, 60);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Chaotic Order-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Psychic, EnergyType.Darkness);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                foreach (var prize in ctx.Me.Prizes)
                    if (!ctx.Me.FaceUpPrizeIds.Contains(prize.Id)) ctx.Me.FaceUpPrizeIds.Add(prize.Id);
                ctx.Log("Chaotic Order-GX: " + ctx.Engine.PlayerName(ctx.Player) + "'s Prize cards are face up for the rest of the game.");
                if (extra) ctx.Engine.TakePrizeCards(ctx.Player, 2);
                return true;
            }
        }

        /// <summary>Fighter's Roar: if the opponent's Active Pokémon is a Pokémon-GX or Pokémon-EX, it can evolve during the turn you play it.</summary>
        private sealed class HakamooCEC : Mon
        {
            public override bool AllowsSameTurnEvolution(EffectContext src, PokemonInPlay target, CardDef evolution)
            {
                var opp = src.Opp.Active;
                return src.Pokemon != null && target == src.Pokemon && opp != null && opp.Def.IsGXorEX;
            }
        }

        /// <summary>Shout of Power: attach a basic Energy card from your discard pile to 1 of your Benched Pokémon.</summary>
        private sealed class ShoutOfPowerMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Shout of Power") return false;
                ctx.DamageDefending(atk.Damage);
                var energy = ctx.BasicEnergyInDiscard();
                if (energy.Count == 0 || ctx.Me.Bench.Count == 0) return true;
                ctx.AttachCardsAnyWay(energy, 1, (card, mon) => !ctx.IsActive(mon), "Shout of Power: attach a basic Energy card to 1 of your Benched Pokémon", false);
                return true;
            }
        }

        /// <summary>Scaly Uppercut: you may discard a Pokémon Tool from it; if you do, 90 more damage.</summary>
        private sealed class ScalyUppercutMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Scaly Uppercut") return false;
                var me = ctx.Pokemon;
                bool bonus = false;
                if (me.Tools.Count > 0 && ctx.AskYesNo("Scaly Uppercut: discard a Pokémon Tool from " + me.Name + " for 90 more damage?"))
                {
                    var tool = ctx.ChooseCards(me.Tools.ToList(), 1, 1, "Scaly Uppercut: discard which Pokémon Tool?")[0];
                    bonus = ctx.Engine.DiscardTool(me, tool);
                }
                ctx.DamageDefending(atk.Damage + (bonus ? 90 : 0));
                return true;
            }
        }

        /// <summary>Ultra Burst: it can't attack unless the opponent has 2 or fewer Prize cards left. Luster of Downfall: discard an Energy from the opponent's Active Pokémon.</summary>
        private sealed class UltraNecrozmaCEC : Mon
        {
            private static readonly Mon Luster = new CardsSSH1.DiscardDefendingEnergyMon("Luster of Downfall", 1);

            public override bool CanUseAttack(EffectContext ctx, int attackIndex)
            {
                return ctx.Opp.Prizes.Count <= 2 || !ctx.Engine.HasAbilityNamed(ctx.Pokemon, "Ultra Burst");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Luster.HandleAttack(ctx, atk); }
        }

        // ================================================================== Colorless

        /// <summary>Puffy Smashers-GX: Asleep; with 4 extra Energy, also 200 damage to 1 of the opponent's Benched Pokémon.</summary>
        private sealed class PuffySmashersMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Puffy Smashers-GX") return false;
                bool extra = ExtraEnergy(ctx, atk, 4);
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (TefPokemon.StillDefending(ctx, defending)) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                if (!extra) return true;
                var bench = ctx.OpponentBench();
                if (bench.Count == 0) return true;
                var target = ctx.ChoosePokemon(bench, "Puffy Smashers-GX: 200 damage to which of your opponent's Benched Pokémon?");
                if (target != null) ctx.DamageTarget(target, 200);
                return true;
            }
        }

        /// <summary>Follow My Tail: search your deck for any number of Eevee and Eevee-GX and put them onto your Bench.</summary>
        private sealed class FollowMyTailMon : Mon
        {
            private static bool EeveeCard(CardDef d) { return d.Name == "Eevee" || d.Name == "Eevee-GX"; }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Follow My Tail") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0 && ctx.BenchSpace > 0)
                    ctx.SearchToBench(EeveeCard, ctx.BenchSpace, "Follow My Tail: put any number of Eevee and Eevee-GX onto your Bench");
                return true;
            }
        }

        /// <summary>Sleepy Voice: once during your turn, flip a coin; on heads, the opponent's Active Pokémon is now Asleep. Your turn ends.</summary>
        private sealed class IgglybuffCEC : Mon
        {
            protected override string Activated { get { return "Sleepy Voice"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Active != null; }

            protected override void Ability(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (ctx.Flip() && defending != null) ctx.ApplyCondition(defending, SpecialCondition.Asleep);
                ctx.Log("Sleepy Voice: " + ctx.Engine.PlayerName(ctx.Player) + "'s turn ends.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Scampering Tail: once during your turn, put the top card of the opponent's deck on the bottom without looking at it.</summary>
        private sealed class AipomCEC : Mon
        {
            protected override string Activated { get { return "Scampering Tail"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Opp.Deck.Count > 1; }

            protected override void Ability(EffectContext ctx)
            {
                var top = ctx.Opp.Deck[0];
                ctx.Opp.Deck.RemoveAt(0);
                ctx.Opp.Deck.Add(top);
                ctx.Log("Scampering Tail: the top card of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck was put on the bottom.");
            }
        }

        /// <summary>
        /// Arf Arf Bark: when it evolves from your hand, you may discard an Energy from the opponent's Active Pokémon; also if it's Knocked
        /// Out as your Active Pokémon by damage from an opponent's attack. Overrun: also 30 to 1 Benched Pokémon.
        /// </summary>
        private sealed class StoutlandCEC : Mon
        {
            private static readonly Mon Overrun = new Cards30CC.BenchHitMon("Overrun", 30);

            public override void OnEvolvedFromHand(EffectContext ctx)
            {
                var defending = ctx.Opp.Active;
                if (!TriggerOnce(ctx, "Arf Arf Bark", defending != null && defending.Energy.Count > 0, "Arf Arf Bark: discard an Energy from your opponent's Active Pokémon?")) return;
                ctx.DiscardEnergyFrom(defending, 1, "Arf Arf Bark: discard an Energy from " + defending.Name);
            }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                var defending = src.Opp.Active;
                if (!byOpponentsAttack || !wasActive || defending == null || defending.Energy.Count == 0) return;
                if (!src.AskYesNo("Arf Arf Bark: discard an Energy from your opponent's Active Pokémon?")) return;
                src.DiscardEnergyFrom(defending, 1, "Arf Arf Bark: discard an Energy from " + defending.Name);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Overrun.HandleAttack(ctx, atk); }
        }

        /// <summary>Carry and Run: while it's on your Bench, your Active Pokémon's Retreat Cost is [C][C] less. Lariat: nothing on tails.</summary>
        private sealed class BewearCEC : Mon
        {
            private static readonly Mon Lariat = new TailsNothingMon("Lariat");

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                var me = src.Pokemon;
                if (me == null || src.IsActive(me) || mon == null || mon.Owner != src.Player || !src.IsActive(mon)) return cost;
                return Math.Max(0, cost - 2);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Lariat.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Disk Reload: once during your turn, draw until you have 5 cards. Brave Buddies: 70 more if you played a Supporter from your hand
        /// this turn. Silver Knight-GX: if the opponent's Active Pokémon is an Ultra Beast, it's Knocked Out.
        /// </summary>
        private sealed class SilvallyGX : Mon
        {
            private static readonly Mon BraveBuddies = new CardsASR1.PlusMon("Brave Buddies", 70, SupporterPlayed);

            private static int SupporterPlayed(EffectContext ctx) { return ctx.Me.SupporterPlayedThisTurn ? 1 : 0; }

            protected override string Activated { get { return "Disk Reload"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Count < 5 && ctx.Me.Deck.Count > 0; }

            protected override void Ability(EffectContext ctx) { ctx.DrawUntil(5); }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Silver Knight-GX") return BraveBuddies.HandleAttack(ctx, atk);
                var defending = ctx.Defending;
                if (defending != null && defending.Def.IsUltraBeast) ctx.Engine.KnockOut(defending, "Silver Knight-GX");
                else ctx.Log("Silver Knight-GX did nothing: the opponent's Active Pokémon isn't an Ultra Beast.");
                return true;
            }
        }
    }
}
