using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Unbroken Bonds (UNB) part 3: Metal, Fairy and Colorless (UNB 120-163). No regulation mark: Expanded. Celesteela-GX's Force
    /// Canceler looks at the attack being resolved (GameEngine.ResolvingAttack) to spot the opponent's GX attacks.
    /// </summary>
    public static class CardsUNB3
    {
        public static void Register(CardScriptRegistry r)
        {
            r.RegisterByKey("UNB-120", new LucarioMelmetalGX());
            r.RegisterByKey("UNB-122", new AlolanDugtrioUNB());
            r.RegisterByKey("UNB-123", new CardsASC.SelfGuardMon("Rigidify", 30));
            r.RegisterByKey("UNB-124", new CardsASC.SelfGuardMon("Rigidify", 30));
            r.RegisterByKey("UNB-125", new ComboMon(new ExtraTightPressMon(), new LockAfterAttackMon("Giga Impact", false)));
            r.RegisterByKey("UNB-126", new CardsFST2.ConditionBonusMon("Rush Up", 70, c => c.Pokemon != null && c.Pokemon.ToolFromHandOnTurn == c.Turn));
            r.RegisterByKey("UNB-127", new GenesectUNB());
            r.RegisterByKey("UNB-128", new CardsMEW1.DefendingTypeBonusMon("Steel Melt", EnergyType.Metal, 40));
            r.RegisterByKey("UNB-129", new MelmetalUNB());
            r.RegisterByKey("UNB-130", new GardevoirSylveonGX());
            r.RegisterByKey("UNB-131", new CleffaUNB());
            r.RegisterByKey("UNB-132", new CoinsMon("Double Slap", 2));
            r.RegisterByKey("UNB-133", new CardsASR1.PlusMon("Moon-Watching Dance", 30, c => c.MyPokemon(m => c.Engine.EnergyCountOfType(m, EnergyType.Fairy) > 0).Count));
            r.RegisterByKey("UNB-134", new CardsPFL.FlipUntilTailsTimesMon("Ball Roll"));
            r.RegisterByKey("UNB-135", new ComboMon(new OrbPolishMon(), new ConditionAttackMon("Sleepy Ball", SpecialCondition.Asleep)));
            r.RegisterByKey("UNB-136", new CardsMEG.WeakenDefendingMon("Daunt", 30));
            r.RegisterByKey("UNB-137", new EnergyPresentMon());
            r.RegisterByKey("UNB-138", new TogekissUNB());
            r.RegisterByKey("UNB-139", new CardsASC.SelfGuardMon("Expand", 10));
            r.RegisterByKey("UNB-140", new CardsPAF2.DriftingDodgeMon(new WhimsicottGXAttacks()));
            r.RegisterByKey("UNB-141", new CardsTWM1.HealSelfMon("Nap", 20));
            r.RegisterByKey("UNB-142", new ComboMon(new PungentAromaMon(), new MiraculousCologneMon()));
            r.RegisterByKey("UNB-144", new EscapingIncisorsMon());
            r.RegisterByKey("UNB-146", new DrillRunDoubleMon());
            r.RegisterByKey("UNB-147", new CardsUNB1.CaturdayMon(2));
            r.RegisterByKey("UNB-148", new PersianUNB());
            r.RegisterByKey("UNB-149", new PersianGX());
            r.RegisterByKey("UNB-150", new CoinsMon("Double Peck", 2));
            r.RegisterByKey("UNB-151", new ComboMon(new CoinsMon("Tri Attack", 3), new LockAfterAttackMon("Accelerating Stab", true)));
            r.RegisterByKey("UNB-152", new ParalyzeFlipMon("Lick"));
            r.RegisterByKey("UNB-153", new ComboMon(new EatUpMon(), new CoinsMon("Tonguenado", 4)));
            r.RegisterByKey("UNB-154", new DigichargeMon());
            r.RegisterByKey("UNB-155", new DrawAttackMon("Quick Draw", 1));
            r.RegisterByKey("UNB-156", new DrawAttackMon("Double Draw", 2));
            r.RegisterByKey("UNB-157", new PorygonZUNB());
            r.RegisterByKey("UNB-158", new SnorlaxUNB());
            r.RegisterByKey("UNB-159", new ComboMon(new CardsUNB1.CaturdayMon(1), new BoingBoingTailMon()));
            r.RegisterByKey("UNB-160", new StrayCatDashMon());
            r.RegisterByKey("UNB-161", new HappinyUNB());
            r.RegisterByKey("UNB-162", new ComboMon(new MimicMon(), new ConditionAttackMon("Tone-Deaf", SpecialCondition.Confused)));
            r.RegisterByKey("UNB-163", new CelesteelaGX());
        }

        // ================================================================== Metal

        /// <summary>
        /// Steel Fist: search for a Metal Energy card and attach it to this Pokémon. Full Metal Wall-GX: for the rest of the game your
        /// Metal Pokémon take 30 less damage from the opponent's attacks; with 1 extra Energy, discard all Energy from the
        /// opponent's Active Pokémon.
        /// </summary>
        private sealed class LucarioMelmetalGX : Mon
        {
            private static readonly Mon SteelFist = new CardsMEG.ChargeSelfMon("Steel Fist", EnergyType.Metal, 1);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Full Metal Wall-GX") return SteelFist.HandleAttack(ctx, atk);
                bool extra = ExtraEnergy(ctx, atk, 1);
                var defending = ctx.Defending;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                ctx.AddEffect(new MetalWallEffect("Full Metal Wall-GX: " + ctx.Engine.PlayerName(ctx.Player) + "'s Metal Pokémon take 30 less damage for the rest of the game",
                    ctx.Player, int.MaxValue));
                if (extra && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending) && defending.Energy.Count > 0)
                    foreach (var card in defending.Energy.ToList()) ctx.Engine.DiscardEnergy(defending, card);
                return true;
            }
        }

        private sealed class MetalWallEffect : ContinuousEffect
        {
            public MetalWallEffect(string name, int owner, int expiresAfterTurn) : base(name, owner, -1, expiresAfterTurn, false) { }

            public override int ModifyDamageTaken(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Owner != Owner || !info.AgainstOpponent || damage <= 0) return damage;
                return engine.IsType(info.Target, EnergyType.Metal) ? Math.Max(0, damage - 30) : damage;
            }
        }

        /// <summary>Hair Wall: your Metal Pokémon take 10 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class AlolanDugtrioUNB : Mon
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (src.Pokemon == null || info.Target == null || info.Target.Owner != src.Player || !info.AgainstOpponent || damage <= 0) return damage;
                return src.Engine.IsType(info.Target, EnergyType.Metal) ? Math.Max(0, damage - 10) : damage;
            }
        }

        /// <summary>Extra-Tight Press: during the opponent's next turn, if this Pokémon is damaged by an attack (even if Knocked Out), put 8 damage counters on the Attacking Pokémon.</summary>
        private sealed class ExtraTightPressMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Extra-Tight Press") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ctx.InPlay(me)) ctx.AddEffect(new PressBackEffect(me.Name + " strikes back for 8 damage counters (Extra-Tight Press)", ctx.Player, me.Uid, ctx.Turn + 1));
                return true;
            }
        }

        private sealed class PressBackEffect : ContinuousEffect
        {
            public PressBackEffect(string name, int owner, int targetUid, int expiresAfterTurn) : base(name, owner, targetUid, expiresAfterTurn, true) { }

            public override void OnTargetDamaged(GameEngine engine, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Uid != TargetUid || info.Attacker == null || !info.AgainstOpponent || damage <= 0) return;
                engine.PlaceDamageCounters(info.Attacker, 8, "Extra-Tight Press");
            }
        }

        /// <summary>Fast-Flight Configuration: no Retreat Cost while the opponent has a Pokémon-GX or Pokémon-EX in play. Splitting Beam: also 30 to 2 Benched Pokémon.</summary>
        private sealed class GenesectUNB : Mon
        {
            private static readonly Mon Beam = new CardsHIF1.BenchedSpreadMon("Splitting Beam", 2, 30);

            public override int ModifyRetreatCost(EffectContext src, PokemonInPlay mon, int cost)
            {
                return src.Pokemon != null && mon == src.Pokemon && src.Opp.InPlay().Any(m => m.Def.IsGXorEX) ? 0 : cost;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Beam.HandleAttack(ctx, atk); }
        }

        /// <summary>Metal Eater: once during your turn, discard a Metal Pokémon from your hand to heal 100 damage from this Pokémon.</summary>
        private sealed class MelmetalUNB : Mon
        {
            private static bool MetalCard(CardDef d) { return d.IsPokemon && d.PokemonType == EnergyType.Metal; }

            protected override string Activated { get { return "Metal Eater"; } }

            protected override bool AbilityReady(EffectContext ctx) { return ctx.Me.Hand.Any(c => MetalCard(c.Def)); }

            protected override void Ability(EffectContext ctx)
            {
                var metal = ctx.Me.Hand.Where(c => MetalCard(c.Def)).ToList();
                if (metal.Count == 0) return;
                var pick = ctx.ChooseCards(metal, 1, 1, "Metal Eater: discard which Metal Pokémon?")[0];
                ctx.Engine.DiscardFromHand(ctx.Player, pick);
                ctx.Heal(ctx.Pokemon, 100);
            }
        }

        // ================================================================== Fairy

        /// <summary>
        /// Fairy Song: search for up to 2 Fairy Energy cards and attach them to your Benched Pokémon in any way you like. Kaleidostorm:
        /// move any number of Energy between your Pokémon. Magical Miracle-GX: with 3 extra Fairy Energy (6 in all), the opponent
        /// shuffles their hand into their deck.
        /// </summary>
        private sealed class GardevoirSylveonGX : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Fairy Song")
                {
                    if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                    if (ctx.Me.Deck.Count == 0) return true;
                    var found = ctx.SearchMyDeck(d => CardsUNB1.IsBasicEnergyOf(d, EnergyType.Fairy), 2, "Fairy Song: search your deck for up to 2 Fairy Energy cards");
                    CardsUNB1.AttachFoundAnyWay(ctx, found, "Fairy Song", m => !ctx.IsActive(m));
                    ctx.ShuffleMyDeck();
                    return true;
                }
                if (atk.Name == "Kaleidostorm")
                {
                    ctx.DamageDefending(atk.Damage);
                    MoveEnergyAnyWay(ctx, "Kaleidostorm");
                    return true;
                }
                if (atk.Name == "Magical Miracle-GX")
                {
                    bool extra = ctx.Engine.EnergyCountOfType(ctx.Pokemon, EnergyType.Fairy) >= 6;
                    ctx.DamageDefending(atk.Damage);
                    if (extra && ctx.Opp.Hand.Count > 0)
                    {
                        ctx.Log(ctx.Engine.PlayerName(ctx.Opponent) + " shuffles " + ctx.Engine.Their(ctx.Opponent) + " hand into " + ctx.Engine.Their(ctx.Opponent) + " deck (Magical Miracle-GX).");
                        ctx.Engine.ShuffleHandIntoDeck(ctx.Opponent);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// "Move any number of Energy from your Pokémon to your other Pokémon in any way you like" (Kaleidostorm). The person playing
        /// moves one at a time and presses Done; a bot leaves its Energy where it is (moving none is allowed).
        /// </summary>
        internal static void MoveEnergyAnyWay(EffectContext ctx, string what)
        {
            if (!RepeatableAbility.ForPerson(ctx)) return;
            for (int step = 0; step < 60; step++)
            {
                var givers = ctx.MyPokemon(m => m.Energy.Count > 0);
                if (givers.Count == 0 || ctx.MyPokemon().Count < 2) return;
                var from = RepeatableAbility.ChoosePokemonOrDone(ctx, givers, what + ": move an Energy from which Pokémon? Press Done when finished.", true);
                if (from == null) return;
                var energy = from.Energy.Count == 1 ? from.Energy[0] : ctx.ChooseCards(from.Energy.ToList(), 1, 1, what + ": move which Energy?")[0];
                var to = ctx.ChoosePokemon(ctx.MyPokemon(m => m != from), what + ": move " + energy.Def.Name + " to which Pokémon?");
                if (to == null || !ctx.Engine.MoveEnergy(from, to, energy)) return;
            }
        }

        /// <summary>Excitable Draw: once during your turn, flip; on heads shuffle your hand into your deck and draw 6 cards. Your turn ends.</summary>
        private sealed class CleffaUNB : Mon
        {
            protected override string Activated { get { return "Excitable Draw"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                    ctx.Draw(6);
                }
                else ctx.Log("Excitable Draw: tails.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Orb Polish: put 3 Energy cards from your discard pile into your hand.</summary>
        private sealed class OrbPolishMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Orb Polish") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Discard.Where(c => c.Def.IsEnergy).ToList();
                if (energy.Count == 0) return true;
                var picks = energy.Count <= 3 ? energy : ctx.ChooseCards(energy, 3, 3, "Orb Polish: put which 3 Energy cards into your hand?");
                ctx.DiscardToHand(picks);
                return true;
            }
        }

        /// <summary>Energy Present: attach an Energy card from your hand to 1 of your Benched Pokémon.</summary>
        private sealed class EnergyPresentMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Energy Present") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var energy = ctx.Me.Hand.Where(c => c.Def.IsEnergy).ToList();
                var bench = ctx.MyBench().Where(m => ctx.Engine.CanAttachFromHand(m)).ToList();
                if (energy.Count == 0 || bench.Count == 0) return true;
                var card = ctx.ChooseCards(energy, 1, 1, "Energy Present: attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(bench, "Energy Present: attach " + card.Def.Name + " to which Benched Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
                return true;
            }
        }

        /// <summary>Fairy Feast: once during your turn, heal 30 damage from each of your Fairy Pokémon.</summary>
        private sealed class TogekissUNB : Mon
        {
            protected override string Activated { get { return "Fairy Feast"; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.MyPokemon(m => m.DamageCounters > 0 && ctx.Engine.IsType(m, EnergyType.Fairy)).Count > 0;
            }

            protected override void Ability(EffectContext ctx)
            {
                foreach (var mon in ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fairy))) ctx.Heal(mon, 30);
            }
        }

        /// <summary>Energy Blow: 30 more for each Energy attached to this Pokémon. Toy Box-GX: search your deck for up to 5 cards and put them into your hand.</summary>
        private sealed class WhimsicottGXAttacks : Mon
        {
            private static readonly Mon Blow = new CardsASR1.PlusMon("Energy Blow", 30, c => c.EnergyCount(c.Pokemon));

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Toy Box-GX") return Blow.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Me.Deck.Count > 0) ctx.SearchToHand(d => true, 5, "Toy Box-GX: search your deck for up to 5 cards", false);
                return true;
            }
        }

        /// <summary>Pungent Aroma: flip 2 coins; with any heads the opponent reveals their hand, and for each heads you choose a card there for them to shuffle into their deck.</summary>
        private sealed class PungentAromaMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Pungent Aroma") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(2);
                if (heads == 0) return true;
                TefPokemon.RevealOpponentHand(ctx);
                var hand = ctx.Opp.Hand.ToList();
                if (hand.Count == 0) return true;
                var picks = hand.Count <= heads ? hand : ctx.ChooseCards(hand, heads, heads, "Pungent Aroma: choose " + heads + " card" + (heads == 1 ? "" : "s") + " for your opponent to shuffle into their deck");
                foreach (var card in picks)
                    if (ctx.Opp.Hand.Remove(card)) ctx.Opp.Deck.Add(card);
                ctx.Engine.ShuffleDeck(ctx.Opponent);
                return true;
            }
        }

        /// <summary>Miraculous Cologne: flip; on heads, choose a Special Condition for the opponent's Active Pokémon.</summary>
        private sealed class MiraculousCologneMon : Mon
        {
            private static readonly SpecialCondition[] All = { SpecialCondition.Asleep, SpecialCondition.Burned, SpecialCondition.Confused, SpecialCondition.Paralyzed, SpecialCondition.Poisoned };

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Miraculous Cologne") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (!ctx.Flip() || !TefPokemon.StillDefending(ctx, defending)) return true;
                int pick = ctx.ChooseOption("Miraculous Cologne: which Special Condition for your opponent's Active Pokémon?", "Asleep", "Burned", "Confused", "Paralyzed", "Poisoned");
                ctx.ApplyCondition(defending, All[pick]);
                return true;
            }
        }

        // ================================================================== Colorless

        /// <summary>Escaping Incisors: if the opponent's Active Pokémon is an Evolution Pokémon, switch this Pokémon with 1 of your Benched Pokémon.</summary>
        private sealed class EscapingIncisorsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Escaping Incisors") return false;
                bool evolved = ctx.Defending != null && ctx.Defending.Def.IsEvolution;
                ctx.DamageDefending(atk.Damage);
                if (evolved && ctx.InPlay(ctx.Pokemon)) ctx.SwitchSelfWithBench();
                return true;
            }
        }

        /// <summary>Drill Run Double: flip; on heads, discard 2 Energy from the opponent's Active Pokémon.</summary>
        private sealed class DrillRunDoubleMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Drill Run Double") return false;
                var defending = ctx.Defending;
                ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && TefPokemon.StillDefending(ctx, defending) && ctx.CanAffect(defending))
                    ctx.DiscardEnergyFrom(defending, 2, "Drill Run Double: discard 2 Energy from " + defending.Name);
                return true;
            }
        }

        /// <summary>Gathering of Cats: ignore all Energy in the attack costs of your Pokémon in play that have the Caturday attack.</summary>
        private sealed class PersianUNB : Mon
        {
            public override void ModifyAttackCost(EffectContext src, PokemonInPlay mon, AttackDef attack, List<EnergyType> cost)
            {
                if (src.Pokemon == null || mon == null || mon.Owner != src.Player) return;
                if (mon.Def.Attacks.Any(a => a.Name == "Caturday")) cost.Clear();
            }
        }

        /// <summary>
        /// Cat Walk: once during your turn (only 1 Cat Walk each turn), if any of your Pokémon-GX or Pokémon-EX were Knocked Out
        /// during the opponent's last turn, search your deck for up to 2 cards. Vengeance: 20 more for each Pokémon in your discard
        /// pile (at most 180 more). Slash Back-GX: switch this Pokémon with 1 of your Benched Pokémon.
        /// </summary>
        private sealed class PersianGX : Mon
        {
            private static readonly Mon SlashBack = new SwitchSelfMon("Slash Back-GX", false);

            protected override string Activated { get { return "Cat Walk"; } }

            protected override bool OnePerPlayer { get { return true; } }

            protected override bool AbilityReady(EffectContext ctx)
            {
                return ctx.Me.Deck.Count > 0 && ctx.WasKnockedOutLastOpponentTurn(d => d.IsGXorEX, false);
            }

            protected override void Ability(EffectContext ctx)
            {
                ctx.SearchToHand(d => true, 2, "Cat Walk: search your deck for up to 2 cards", false);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name == "Vengeance")
                {
                    int bonus = Math.Min(180, 20 * ctx.Me.Discard.Count(c => c.Def.IsPokemon));
                    ctx.DamageDefending(atk.Damage + bonus);
                    return true;
                }
                return SlashBack.HandleAttack(ctx, atk);
            }
        }

        /// <summary>Eat Up: before doing damage, discard all Tools from the opponent's Active Pokémon; if any were discarded, heal all damage from this Pokémon.</summary>
        private sealed class EatUpMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Eat Up") return false;
                var defending = ctx.Defending;
                bool ate = false;
                if (defending != null && defending.Tools.Count > 0 && ctx.CanAffect(defending))
                    foreach (var tool in defending.Tools.ToList())
                    {
                        ctx.Engine.DiscardTool(defending, tool);
                        ate = true;
                    }
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                if (ate && ctx.InPlay(me) && me.DamageCounters > 0) ctx.Heal(me, me.DamageCounters * 10);
                return true;
            }
        }

        /// <summary>Digicharge: flip 3 coins; for each heads, search your deck for an Energy card and put it into your hand.</summary>
        private sealed class DigichargeMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Digicharge") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(3);
                if (heads > 0 && ctx.Me.Deck.Count > 0)
                    ctx.SearchToHand(d => d.IsEnergy, heads, "Digicharge: search your deck for up to " + heads + " Energy card" + (heads == 1 ? "" : "s"));
                return true;
            }
        }

        /// <summary>Crazy Code: as often as you like during your turn, attach a Special Energy card from your hand to 1 of your Pokémon. Tantrum: this Pokémon is now Confused.</summary>
        private sealed class PorygonZUNB : Mon
        {
            private const string CrazyCode = "Crazy Code";
            private static readonly Mon Tantrum = new CardsRCL2.SelfConditionMon("Tantrum", SpecialCondition.Confused);

            private static List<CardInstance> Specials(EffectContext ctx)
            {
                return ctx.Me.Hand.Where(c => c.Def.IsSpecialEnergy && !ctx.Engine.EnergyCardBlockedBySource(ctx.Player, c)).ToList();
            }

            private static List<PokemonInPlay> Targets(EffectContext ctx, CardInstance card)
            {
                return ctx.MyPokemon(m => ctx.Engine.CanAttachFromHand(m) && ctx.Engine.Scripts.For(card.Def).CanAttachTo(new EffectContext(ctx.Engine, ctx.Player, card, m), m));
            }

            private static bool CanStep(EffectContext ctx) { return Specials(ctx).Any(c => Targets(ctx, c).Count > 0); }

            public override bool CanUseAbility(EffectContext ctx, int abilityIndex)
            {
                if (ctx.Pokemon.Def.Abilities[abilityIndex].Name != CrazyCode || !ctx.IsMyTurn) return false;
                return CanStep(ctx);
            }

            public override void UseAbility(EffectContext ctx, int abilityIndex)
            {
                RepeatableAbility.Repeat(ctx, () => CanStep(ctx), offerDone =>
                {
                    var cards = Specials(ctx).Where(c => Targets(ctx, c).Count > 0).ToList();
                    if (cards.Count == 0) return false;
                    var card = cards.Count == 1 && !offerDone ? cards[0] : ctx.ChooseCards(cards, offerDone ? 0 : 1, 1, "Crazy Code: attach which Special Energy card?" + (offerDone ? " Choose none to finish." : "")).FirstOrDefault();
                    if (card == null) return false;
                    var target = ctx.ChoosePokemon(Targets(ctx, card), "Crazy Code: attach " + card.Def.Name + " to which Pokémon?");
                    if (target == null || !ctx.Me.Hand.Remove(card)) return false;
                    return ctx.Engine.AttachEnergyCard(card, target, true);
                });
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Tantrum.HandleAttack(ctx, atk); }
        }

        /// <summary>Lazy Eating: between turns, heal 10 damage from this Pokémon. Big Counter: 120 more against a TAG TEAM.</summary>
        private sealed class SnorlaxUNB : Mon
        {
            private static readonly Mon Counter = new CardsFST2.ConditionBonusMon("Big Counter", 120, c => c.Defending != null && c.Defending.Def.IsTagTeam);

            public override void OnPokemonCheckup(EffectContext src)
            {
                var me = src.Pokemon;
                if (me != null && me.DamageCounters > 0) src.Heal(me, 10);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Counter.HandleAttack(ctx, atk); }
        }

        /// <summary>Boing Boing Tail: 60 damage to 1 of the opponent's Pokémon-GX or Pokémon-EX.</summary>
        private sealed class BoingBoingTailMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Boing Boing Tail") return false;
                var targets = ctx.OpponentPokemon(m => m.Def.IsGXorEX);
                if (targets.Count == 0) return true;
                var target = ctx.ChoosePokemon(targets, "Boing Boing Tail: 60 damage to which of your opponent's Pokémon-GX or Pokémon-EX?");
                if (target != null) ctx.DamageTarget(target, 60);
                return true;
            }
        }

        /// <summary>Stray Cat Dash: discard a random card from the opponent's hand (2 if this Pokémon evolved from Glameow this turn).</summary>
        private sealed class StrayCatDashMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Stray Cat Dash") return false;
                ctx.DamageDefending(atk.Damage);
                var me = ctx.Pokemon;
                bool fresh = me != null && me.TurnEvolved == ctx.Turn && me.Stack.Count >= 2 && me.Stack[me.Stack.Count - 2].Def.Name == "Glameow";
                int count = fresh ? 2 : 1;
                for (int k = 0; k < count && ctx.Opp.Hand.Count > 0; k++) TefPokemon.DiscardRandomFromOpponentHand(ctx);
                return true;
            }
        }

        /// <summary>Playhouse Heal: once during your turn, flip; on heads heal 60 damage from 1 of your Pokémon. Your turn ends.</summary>
        private sealed class HappinyUNB : Mon
        {
            protected override string Activated { get { return "Playhouse Heal"; } }

            protected override void Ability(EffectContext ctx)
            {
                if (ctx.Flip())
                {
                    var hurt = ctx.MyPokemon(m => m.DamageCounters > 0);
                    if (hurt.Count > 0)
                    {
                        var target = ctx.ChoosePokemon(hurt, "Playhouse Heal: heal 60 damage from which of your Pokémon?");
                        if (target != null) ctx.Heal(target, 60);
                    }
                }
                else ctx.Log("Playhouse Heal: tails.");
                ctx.Engine.RequestEndTurn();
            }
        }

        /// <summary>Mimic: shuffle your hand into your deck, then draw a card for each card in the opponent's hand.</summary>
        internal sealed class MimicMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Mimic") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int count = ctx.Opp.Hand.Count;
                ctx.Engine.ShuffleHandIntoDeck(ctx.Player);
                if (count > 0) ctx.Draw(count);
                return true;
            }
        }

        /// <summary>
        /// Force Canceler: while this Pokémon is your Active Pokémon, prevent all effects of the opponent's GX attacks, damage
        /// included, done to your Pokémon. Power Cyclone: move an Energy to a Benched Pokémon. Discovery-GX: your Prize cards go into
        /// your hand and the same number from the top of your deck become your Prize cards (nothing happens if the deck is too small).
        /// </summary>
        private sealed class CelesteelaGX : Mon
        {
            private static readonly Mon Cyclone = new CardsFST1.MoveEnergyToBenchMon("Power Cyclone");

            private static bool Guarding(EffectContext src, PokemonInPlay target)
            {
                if (src.Pokemon == null || !src.PokemonIsActive || target == null || target.Owner != src.Player) return false;
                var atk = src.Engine.ResolvingAttack;
                return atk != null && GameEngine.IsGxAttack(atk) && src.Engine.CurrentSourceOwner >= 0 && src.Engine.CurrentSourceOwner != src.Player;
            }

            public override bool PreventsEffects(EffectContext src, PokemonInPlay target, EffectSourceKind kind, bool damageCounters)
            {
                return kind == EffectSourceKind.Attack && Guarding(src, target);
            }

            public override bool PreventsFinalDamage(EffectContext src, DamageInfo info, int damage)
            {
                return info.AgainstOpponent && Guarding(src, info.Target);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Discovery-GX") return Cyclone.HandleAttack(ctx, atk);
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                var me = ctx.Me;
                int n = me.Prizes.Count;
                if (n == 0 || me.Deck.Count < n)
                {
                    ctx.Log("Discovery-GX: not enough cards in the deck, so the attack does nothing.");
                    return true;
                }
                var prizes = me.Prizes.ToList();
                me.Prizes.Clear();
                me.FaceUpPrizeIds.Clear();
                me.Hand.AddRange(prizes);
                for (int k = 0; k < n; k++)
                {
                    var card = me.Deck[0];
                    me.Deck.RemoveAt(0);
                    me.Prizes.Add(card);
                }
                ctx.Log(ctx.Engine.PlayerName(ctx.Player) + " put " + n + " Prize card" + (n == 1 ? "" : "s") + " into " + ctx.Engine.Their(ctx.Player) + " hand and took " + n + " new ones from the top of the deck (Discovery-GX).");
                return true;
            }
        }
    }
}
