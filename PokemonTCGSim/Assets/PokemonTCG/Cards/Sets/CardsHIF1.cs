using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.Cards
{
    /// <summary>
    /// Hidden Fates (HIF) part 1: the main set (HIF 1-65; 64 Pokémon Center Lady and the full-art and rainbow printings 66-69
    /// are in the pool already). No regulation mark: Expanded; Jessie &amp; James and Lt. Surge's Strategy are on the Expanded ban
    /// list. Your rulings (v69): Misty's Water Command and Brock's Training name exact Pokémon (Gyarados-GX isn't Gyarados, plain
    /// Onix isn't Onix-GX); Sky Legends-GX checks its extra Energy before it shuffles itself away; Brock's Grit shuffles in all of
    /// them when there are fewer than 6. Compendium: Lt. Surge's Strategy must be your first Supporter of the turn (2019-05);
    /// a Supporter copied with Sabrina's Suggestion keeps its own conditions (2019-09-12); Electromagnetic Wall triggers once per
    /// Energy card attached from the hand, also by an Ability (2012); each Weezing discarded by Jessie &amp; James uses its own
    /// Surrender Now, like Roxie's Blow-Away Bomb.
    /// </summary>
    public static class CardsHIF1
    {
        public static void Register(CardScriptRegistry r)
        {
            // Pinsir-GX, Charizard-GX, Gyarados-GX and Raichu-GX: GX attacks with only the reminder text; the engine already
            // enforces 1 GX attack per game.
            r.RegisterByKey("HIF-6", new CardScript());
            r.RegisterByKey("HIF-9", new CardScript());
            r.RegisterByKey("HIF-16", new CardScript());
            r.RegisterByKey("HIF-20", new CardScript());
            r.RegisterByKey("HIF-12", new ConditionAttackMon("Yawn", SpecialCondition.Asleep));
            r.RegisterByKey("HIF-13", new ParalyzeFlipMon("Numbing Water"));
            r.RegisterByKey("HIF-14", new StarmieGX());
            r.RegisterByKey("HIF-18", new CardsTWM1.HealSelfMon("Bubble Drain", 30));
            r.RegisterByKey("HIF-22", new CardsWHT.FlipMoreMon("Electroblast", 2, 30));
            r.RegisterByKey("HIF-23", new JolteonHIF());
            r.RegisterByKey("HIF-24", new HurricaneCallMon());
            r.RegisterByKey("HIF-25", new CardsASC.FlipDiscardDefendingEnergyMon("Wrap"));
            r.RegisterByKey("HIF-27", new ArbokHIF());
            r.RegisterByKey("HIF-29", new PlainMon()); // Surrender Now: Jessie & James uses it
            r.RegisterByKey("HIF-31", new CardsSSH1.DiscardDefendingEnergyMon("Psycrush-GX", 99));
            r.RegisterByKey("HIF-34", new DiscardThenSnipeMon("Mudslide", EnergyType.Fighting, 2, 80));
            r.RegisterByKey("HIF-35", new BenchedSpreadMon("Rock Slide", 3, 20));
            r.RegisterByKey("HIF-36", new ComboMon(new ParalyzeFlipMon("Bind"), new ShieldAttackMon("Rocky Avalanche-GX", 100)));
            r.RegisterByKey("HIF-37", new CardsMEG.SnipeMon("Sharpshooting", 20));
            r.RegisterByKey("HIF-38", new FlipLeadMon());
            r.RegisterByKey("HIF-42", new ComboMon(new CardsMEG.FlipUntilTailsBonusMon("Rolling Rush", 30), new CardsTWM1.HealSelfMon("Lovely Star-GX", 999)));
            r.RegisterByKey("HIF-43", new ComboMon(new CardsSSH1.TeatimeMon("Happy Mime", 1, 1), new CoinsMon("Double Slap", 2)));
            r.RegisterByKey("HIF-44", new SkyLegendsMon());
            r.RegisterByKey("HIF-46", new RecoilMon("Double-Edge", 20));
            r.RegisterByKey("HIF-47", new CardsPFL.FlipUntilTailsTimesMon("Parental Fury"));
            r.RegisterByKey("HIF-48", new ComboMon(new Cards30C.PeerAtMon("Curiosity"), new CardsFST1.FlipRecoilMon("Spin Tackle", 10)));

            r.RegisterByName("Bill's Analysis", new BillsAnalysis());
            r.RegisterByName("Blaine's Last Stand", new BlainesLastStand());
            r.RegisterByName("Brock's Grit", new BrocksGrit());
            r.RegisterByName("Brock's Pewter City Gym", new BrocksPewterCityGym());
            r.RegisterByName("Brock's Training", new BrocksTraining());
            r.RegisterByName("Erika's Hospitality", new ErikasHospitality());
            r.RegisterByName("Giovanni's Exile", new GiovannisExile());
            r.RegisterByName("Jessie & James", new JessieJames());
            r.RegisterByName("Koga's Trap", new KogasTrap());
            r.RegisterByName("Lt. Surge's Strategy", new LtSurgesStrategy());
            r.RegisterByName("Misty's Cerulean City Gym", new MistysCeruleanCityGym());
            r.RegisterByName("Misty's Determination", new MistysDetermination());
            r.RegisterByName("Misty's Water Command", new MistysWaterCommand());
            r.RegisterByName("Sabrina's Suggestion", new SabrinasSuggestion());
        }

        // ================================================================== Shared helpers

        internal static bool IsTrainerCard(CardDef d) { return d.IsTrainer; }
        internal static bool IsSupporterCard(CardDef d) { return d.IsSupporter; }
        internal static bool IsPokemonCard(CardDef d) { return d.IsPokemon; }
        internal static bool AnyCard(CardDef d) { return true; }

        /// <summary>Cards in the hand other than the Trainer being played (it's still in the hand while legality is checked).</summary>
        internal static int OthersInHand(EffectContext ctx)
        {
            int n = 0;
            foreach (var card in ctx.Me.Hand) if (card != ctx.Card) n++;
            return n;
        }

        /// <summary>An Energy attached to a Pokémon that provides this type ("[W] Energy" in play).</summary>
        internal static bool Provides(EffectContext ctx, PokemonInPlay mon, CardInstance energy, EnergyType type)
        {
            return CardsDRI1.ProvidesType(ctx, mon, energy, type);
        }

        /// <summary>
        /// Attach up to n cards from a pile (taken from the deck or the discard pile) to targets the player picks, one at a time,
        /// none of them optional. Cards left over go back where they came from: discard-pile cards stay put, deck cards are put
        /// back into the deck (the caller shuffles).
        /// </summary>
        internal static int AttachEach(EffectContext ctx, List<CardInstance> pile, int n, Func<CardInstance, PokemonInPlay, bool> filter, string prompt)
        {
            return ctx.AttachCardsAnyWay(pile, n, filter, prompt, false);
        }

        /// <summary>
        /// "Discard N [T] Energy from this Pokémon. This attack does X damage to 1 of your opponent's Pokémon" (Mudslide, Hydro
        /// Shot). The Energy is discarded first; with fewer than N it discards what there is.
        /// </summary>
        internal sealed class DiscardThenSnipeMon : Mon
        {
            private readonly string attack;
            private readonly EnergyType type;
            private readonly int count;
            private readonly int amount;

            public DiscardThenSnipeMon(string attack, EnergyType type, int count, int amount)
            {
                this.attack = attack;
                this.type = type;
                this.count = count;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                var me = ctx.Pokemon;
                ctx.DiscardEnergyFrom(me, count, attack + ": discard " + count + " " + type + " Energy from " + me.Name, x => Provides(ctx, me, x, type));
                var target = ctx.ChoosePokemon(ctx.OpponentPokemon(), attack + ": do " + amount + " damage to which of your opponent's Pokémon?");
                if (target != null) ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>"This attack does N damage to [count] of your opponent's Benched Pokémon" after the damage (Rock Slide). With fewer Benched, all of them.</summary>
        internal sealed class BenchedSpreadMon : Mon
        {
            private readonly string attack;
            private readonly int count;
            private readonly int amount;

            public BenchedSpreadMon(string attack, int count, int amount)
            {
                this.attack = attack;
                this.count = count;
                this.amount = amount;
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != attack) return false;
                ctx.DamageDefending(atk.Damage);
                var bench = ctx.OpponentBench();
                int n = Math.Min(count, bench.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(bench, n, n, attack + ": choose " + n + " of your opponent's Benched Pokémon"))
                    ctx.DamageTarget(target, amount);
                return true;
            }
        }

        /// <summary>Lead (Clefairy HIF 38): "Flip a coin. If heads, search your deck for a Supporter card."</summary>
        private sealed class FlipLeadMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Lead") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                if (ctx.Flip() && ctx.Me.Deck.Count > 0) ctx.SearchToHand(IsSupporterCard, 1, "Lead: search your deck for a Supporter card");
                return true;
            }
        }

        // ================================================================== Pokémon

        /// <summary>Star Stream: attach 2 basic Water Energy cards from your discard pile to 1 of your Pokémon. Hydro Pump-GX: 40 more for each Water Energy attached.</summary>
        private sealed class StarmieGX : Mon
        {
            private static readonly Mon HydroPump = new CardsMEG.EnergyBonusMon("Hydro Pump-GX", EnergyType.Water, 40);

            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Star Stream") return HydroPump.HandleAttack(ctx, atk);
                ctx.DamageDefending(atk.Damage);
                var water = ctx.BasicEnergyInDiscard(d => d.EnergyProvides == EnergyType.Water);
                if (water.Count == 0) return true;
                var target = ctx.ChoosePokemon(ctx.MyPokemon(), "Star Stream: attach 2 Water Energy cards from your discard pile to which of your Pokémon?");
                if (target == null) return true;
                var picks = water.Count <= 2 ? water : ctx.ChooseCards(water, 2, 2, "Star Stream: choose 2 Water Energy cards");
                foreach (var card in picks)
                    if (ctx.Me.Discard.Remove(card)) ctx.Engine.AttachEnergyCard(card, target);
                return true;
            }
        }

        /// <summary>
        /// Electromagnetic Wall: as long as this Pokémon is Active, whenever the opponent attaches an Energy card from their hand to
        /// 1 of their Pokémon, 2 damage counters on that Pokémon. Thunderbolt: discard all Energy from this Pokémon.
        /// </summary>
        private sealed class JolteonHIF : Mon
        {
            private static readonly Mon Thunderbolt = new CardsTWM1.SelfDiscardMon("Thunderbolt", 99);

            public override void OnOpponentAttachedEnergyFromHand(EffectContext src, CardInstance energy, PokemonInPlay target)
            {
                if (src.Pokemon == null || !src.IsActive(src.Pokemon) || target == null || !src.InPlay(target)) return;
                Announce(src, "Electromagnetic Wall");
                src.Engine.PlaceDamageCounters(target, 2, "Electromagnetic Wall");
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return Thunderbolt.HandleAttack(ctx, atk); }
        }

        /// <summary>Hurricane Call: flip 4 coins; for each heads, search for a basic Lightning Energy card and attach it to 1 of your Pokémon-GX or Pokémon-EX.</summary>
        private sealed class HurricaneCallMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Hurricane Call") return false;
                if (atk.Damage > 0) ctx.DamageDefending(atk.Damage);
                int heads = ctx.FlipCoins(4);
                if (heads == 0 || ctx.Me.Deck.Count == 0) return true;
                if (!ctx.MyPokemon().Any(m => m.Def.IsGXorEX))
                {
                    ctx.Log("Hurricane Call: no Pokémon-GX or Pokémon-EX to attach Energy to.");
                    return true;
                }
                var found = ctx.SearchMyDeck(d => CardsEVS1.IsEnergyOf(d, EnergyType.Lightning), heads, "Hurricane Call: search your deck for up to " + heads + " Lightning Energy cards");
                AttachEach(ctx, found, found.Count, IsGxOrExTarget, "Hurricane Call");
                foreach (var card in found) if (!ctx.MyPokemon().Any(m => m.Energy.Contains(card))) ctx.Me.Deck.Add(card);
                ctx.ShuffleMyDeck();
                return true;
            }

            private static bool IsGxOrExTarget(CardInstance card, PokemonInPlay mon) { return mon.Def.IsGXorEX; }
        }

        /// <summary>
        /// Last Pattern: if this Pokémon is Knocked Out by damage from an opponent's attack, discard 2 random cards from their hand.
        /// Rocket Tail: 80 more if Jessie &amp; James is in your discard pile.
        /// </summary>
        private sealed class ArbokHIF : Mon
        {
            private static readonly Mon RocketTail = new CardsFST2.ConditionBonusMon("Rocket Tail", 80, JessieJamesInDiscard);

            private static bool JessieJamesInDiscard(EffectContext ctx) { return ctx.Me.Discard.Any(x => x.Def.Name == "Jessie & James"); }

            public override void OnHolderKnockedOut(EffectContext src, bool byOpponentsAttack, bool wasActive)
            {
                if (!byOpponentsAttack || src.Opp.Hand.Count == 0) return;
                src.Log("Last Pattern: " + src.Engine.PlayerName(src.Opponent) + " discards 2 random cards from their hand.");
                for (int i = 0; i < 2; i++) TefPokemon.DiscardRandomFromOpponentHand(src);
            }

            protected override bool Attack(EffectContext ctx, AttackDef atk) { return RocketTail.HandleAttack(ctx, atk); }
        }

        /// <summary>
        /// Sky Legends-GX: shuffle this Pokémon and all attached cards into your deck; if it had at least 1 extra Fire, Water and
        /// Lightning Energy (checked before the shuffle, your ruling), 110 damage to 3 of the opponent's Pokémon (all, with fewer).
        /// </summary>
        private sealed class SkyLegendsMon : Mon
        {
            protected override bool Attack(EffectContext ctx, AttackDef atk)
            {
                if (atk.Name != "Sky Legends-GX") return false;
                bool extra = CardsCEC1.ExtraTyped(ctx, atk, EnergyType.Fire, EnergyType.Water, EnergyType.Lightning);
                if (ctx.InPlay(ctx.Pokemon)) ctx.Engine.ShufflePokemonIntoDeck(ctx.Pokemon);
                if (!extra) return true;
                var targets = ctx.OpponentPokemon();
                int n = Math.Min(3, targets.Count);
                if (n == 0) return true;
                foreach (var target in ctx.ChoosePokemonMany(targets, n, n, "Sky Legends-GX: choose " + n + " of your opponent's Pokémon for 110 damage"))
                    ctx.DamageTarget(target, 110);
                return true;
            }
        }

        // ================================================================== Supporters

        /// <summary>Bill's Analysis: look at the top 7 cards; put up to 2 Trainer cards among them into your hand; shuffle the rest back.</summary>
        private sealed class BillsAnalysis : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var top = ctx.LookAtTop(7);
                var trainers = top.Where(x => x.Def.IsTrainer).ToList();
                var picks = ctx.ChooseCards(trainers, 0, Math.Min(2, trainers.Count), "Bill's Analysis: put up to 2 Trainer cards into your hand");
                if (picks.Count > 0)
                {
                    ctx.DeckToHand(picks);
                    ctx.Engine.Reveal(ctx.Player, picks, "put these Trainer cards into their hand");
                }
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Blaine's Last Stand: only when it's the last card in your hand; draw 2 cards for each Fire Pokémon you have in play.</summary>
        private sealed class BlainesLastStand : CardScript
        {
            private static int FirePokemon(EffectContext ctx) { return ctx.MyPokemon(m => ctx.Engine.IsType(m, EnergyType.Fire)).Count; }

            public override bool CanPlayTrainer(EffectContext ctx) { return OthersInHand(ctx) == 0 && ctx.Me.Deck.Count > 0 && FirePokemon(ctx) > 0; }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(2 * FirePokemon(ctx)); }
        }

        /// <summary>Brock's Grit: shuffle 6 Pokémon and basic Energy cards, in any combination, from your discard pile into your deck (all of them with fewer, your ruling).</summary>
        private sealed class BrocksGrit : CardScript
        {
            private static List<CardInstance> Pool(EffectContext ctx) { return ctx.Me.Discard.Where(x => x.Def.IsPokemon || x.Def.IsBasicEnergy).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Pool(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var pool = Pool(ctx);
                int n = Math.Min(6, pool.Count);
                var picks = ctx.ChooseCards(pool, n, n, "Brock's Grit: choose " + n + " Pokémon and basic Energy cards to shuffle into your deck");
                var moved = ctx.DiscardToDeck(picks);
                ctx.Log("Brock's Grit: " + moved.Count + (moved.Count == 1 ? " card" : " cards") + " shuffled into the deck.");
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>Brock's Training: attach an Energy card from your hand to 1 of your Geodude, Graveler, Golem, Onix-GX, Cubone, Rhyhorn, Rhydon or Sudowoodo (exact names, your ruling).</summary>
        private sealed class BrocksTraining : CardScript
        {
            private static readonly string[] Names = { "Geodude", "Graveler", "Golem", "Onix-GX", "Cubone", "Rhyhorn", "Rhydon", "Sudowoodo" };

            private static List<PokemonInPlay> Targets(EffectContext ctx)
            {
                return ctx.MyPokemon(m => Names.Contains(m.Name) && ctx.Engine.CanAttachFromHand(m));
            }

            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return ctx.Me.Hand.Any(x => x.Def.IsEnergy) && Targets(ctx).Count > 0;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var energy = ctx.Me.Hand.Where(x => x.Def.IsEnergy).ToList();
                var targets = Targets(ctx);
                if (energy.Count == 0 || targets.Count == 0) return;
                var card = ctx.ChooseCards(energy, 1, 1, "Brock's Training: attach which Energy card?")[0];
                var target = ctx.ChoosePokemon(targets, "Brock's Training: attach " + card.Def.Name + " to which Pokémon?");
                if (target != null && ctx.Me.Hand.Remove(card)) ctx.Engine.AttachEnergyCard(card, target, true);
            }
        }

        /// <summary>Erika's Hospitality: only with 4 or fewer other cards in your hand; draw a card for each of your opponent's Pokémon in play.</summary>
        private sealed class ErikasHospitality : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return OthersInHand(ctx) <= 4 && ctx.Me.Deck.Count > 0 && ctx.Opp.HasPokemonInPlay;
            }

            public override void ResolveTrainer(EffectContext ctx) { ctx.Draw(ctx.OpponentPokemon().Count); }
        }

        /// <summary>
        /// Giovanni's Exile: discard up to 2 of your undamaged Benched Pokémon, with all attached cards. Diglett's Underground Work
        /// (UNB, v73): each Diglett discarded this way with that Ability working discards the top card of the opponent's deck.
        /// </summary>
        private sealed class GiovannisExile : CardScript
        {
            private static List<PokemonInPlay> Targets(EffectContext ctx) { return ctx.MyBench().Where(m => m.DamageCounters == 0).ToList(); }

            public override bool CanPlayTrainer(EffectContext ctx) { return Targets(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var targets = Targets(ctx);
                if (targets.Count == 0) return;
                foreach (var mon in ctx.ChoosePokemonMany(targets, 1, Math.Min(2, targets.Count), "Giovanni's Exile: discard up to 2 of your undamaged Benched Pokémon"))
                {
                    if (!ctx.InPlay(mon)) continue;
                    bool underground = mon.Def.Abilities.Any(a => a.Name == "Underground Work") && !ctx.Engine.AbilityBlocked(mon, "Underground Work");
                    ctx.Engine.DiscardPokemonFromPlay(mon);
                    if (!underground || ctx.Opp.Deck.Count == 0 || ctx.Engine.DeckDiscardBlocked(ctx.Opponent)) continue;
                    ctx.Log("Underground Work (" + mon.Name + "): the top card of " + ctx.Engine.PlayerName(ctx.Opponent) + "'s deck is discarded.");
                    ctx.Engine.DiscardTopOfDeck(ctx.Opponent, 1);
                }
            }
        }

        /// <summary>
        /// Jessie &amp; James: each player discards 2 cards from their hand, the opponent first (each chooses their own). Then each
        /// Weezing of yours discarded this way may use Surrender Now: your opponent discards a card from their hand.
        /// </summary>
        private sealed class JessieJames : CardScript
        {
            private static bool HasSurrenderNow(CardDef d)
            {
                foreach (var ab in d.Abilities) if (ab.Name == "Surrender Now") return true;
                return false;
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return OthersInHand(ctx) > 0 || ctx.Opp.Hand.Count > 0; }

            private static List<CardInstance> DiscardTwo(EffectContext ctx, int player)
            {
                var p = ctx.State.Players[player];
                var gone = new List<CardInstance>();
                if (p.Hand.Count == 0) return gone;
                var picks = p.Hand.Count <= 2 ? p.Hand.ToList()
                    : ctx.Engine.ChooseCards(player, p.Hand.ToList(), 2, 2, "Jessie & James: discard 2 cards from your hand", ctx.Source);
                foreach (var card in picks)
                {
                    ctx.Engine.DiscardFromHand(player, card);
                    if (p.Discard.Contains(card)) gone.Add(card);
                }
                return gone;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                DiscardTwo(ctx, ctx.Opponent);
                var mine = DiscardTwo(ctx, ctx.Player);
                foreach (var weezing in mine)
                {
                    if (!HasSurrenderNow(weezing.Def) || ctx.Opp.Hand.Count == 0) continue;
                    if (!ctx.AskYesNo("Surrender Now (" + weezing.Def.Name + "): have your opponent discard a card from their hand?")) continue;
                    ctx.Engine.PushSource(EffectSourceKind.Ability, ctx.Player, null);
                    try
                    {
                        var pick = ctx.Engine.ChooseCards(ctx.Opponent, ctx.Opp.Hand.ToList(), 1, 1, "Surrender Now: discard a card from your hand", weezing.Def);
                        if (pick.Count > 0) ctx.Engine.DiscardFromHand(ctx.Opponent, pick[0]);
                    }
                    finally { ctx.Engine.PopSource(); }
                }
            }
        }

        /// <summary>Koga's Trap: your opponent's Active Pokémon is now Confused and Poisoned.</summary>
        private sealed class KogasTrap : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Active != null; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                var target = ctx.Opp.Active;
                if (target == null) return;
                ctx.ApplyCondition(target, SpecialCondition.Confused);
                ctx.ApplyCondition(target, SpecialCondition.Poisoned);
            }
        }

        /// <summary>
        /// Lt. Surge's Strategy: only if you have more Prize cards left than your opponent, and only as your first Supporter of the
        /// turn (Compendium, 2019-05); this turn you can play 3 Supporter cards, this one included.
        /// </summary>
        private sealed class LtSurgesStrategy : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx)
            {
                return !ctx.Me.SupporterPlayedThisTurn && ctx.Me.Prizes.Count > ctx.Opp.Prizes.Count;
            }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.SupportersPlayedCount > 1) return; // copied after another Supporter: nothing (Compendium)
                ctx.Me.SupporterLimit = 3;
                ctx.Log("Lt. Surge's Strategy: " + ctx.Engine.PlayerName(ctx.Player) + " can play 3 Supporter cards this turn.");
            }
        }

        /// <summary>Misty's Determination: discard a card from your hand; look at the top 8 cards and put 1 into your hand; shuffle the rest back.</summary>
        private sealed class MistysDetermination : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return OthersInHand(ctx) > 0 && ctx.Me.Deck.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                if (ctx.Me.Hand.Count == 0) return;
                var cost = ctx.ChooseCards(ctx.Me.Hand.ToList(), 1, 1, "Misty's Determination: discard a card from your hand");
                ctx.Engine.DiscardFromHand(ctx.Player, cost[0]);
                if (!ctx.Me.Discard.Contains(cost[0]) || ctx.Me.Deck.Count == 0) return;
                var top = ctx.LookAtTop(8);
                var pick = ctx.ChooseCards(top, 1, 1, "Misty's Determination: put 1 of these cards into your hand");
                ctx.DeckToHand(pick);
                ctx.ShuffleMyDeck();
            }
        }

        /// <summary>
        /// Misty's Water Command: move any number of Water Energy from your Pokémon to your Psyduck, Horsea, Staryu, Starmie-GX,
        /// Magikarp, Gyarados or Lapras in any way you like (exact names, your ruling). The person moves as many as they like and
        /// presses Done; bots move one.
        /// </summary>
        private sealed class MistysWaterCommand : CardScript
        {
            private static readonly string[] Names = { "Psyduck", "Horsea", "Staryu", "Starmie-GX", "Magikarp", "Gyarados", "Lapras" };

            private static List<PokemonInPlay> Receivers(EffectContext ctx) { return ctx.MyPokemon(m => Names.Contains(m.Name)); }

            private static List<PokemonInPlay> Givers(EffectContext ctx)
            {
                var receivers = Receivers(ctx);
                return ctx.MyPokemon(m => m.Energy.Any(x => Provides(ctx, m, x, EnergyType.Water)) && receivers.Any(t => t != m));
            }

            public override bool CanPlayTrainer(EffectContext ctx) { return Givers(ctx).Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                RepeatableAbility.Repeat(ctx, () => Givers(ctx).Count > 0, offerDone =>
                {
                    var from = RepeatableAbility.ChoosePokemonOrDone(ctx, Givers(ctx), "Misty's Water Command: move a Water Energy from which Pokémon?"
                        + (offerDone ? " Press Done when finished." : ""), offerDone);
                    if (from == null) return false;
                    var water = from.Energy.Where(x => Provides(ctx, from, x, EnergyType.Water)).ToList();
                    var energy = water.Select(x => x.Def.Name).Distinct().Count() == 1 ? water[0] : ctx.ChooseCards(water, 1, 1, "Misty's Water Command: move which Energy?")[0];
                    var to = ctx.ChoosePokemon(Receivers(ctx).Where(t => t != from).ToList(), "Misty's Water Command: move " + energy.Def.Name + " to which Pokémon?");
                    if (to == null) return false;
                    return ctx.Engine.MoveEnergy(from, to, energy);
                });
            }
        }

        /// <summary>
        /// Sabrina's Suggestion: your opponent reveals their hand; you may use the effect of a Supporter card there as this card's
        /// effect. A Supporter you can't play right now (its own conditions) can't be chosen (Compendium, 2019-09-12).
        /// </summary>
        private sealed class SabrinasSuggestion : CardScript
        {
            public override bool CanPlayTrainer(EffectContext ctx) { return ctx.Opp.Hand.Count > 0; }

            public override void ResolveTrainer(EffectContext ctx)
            {
                TefPokemon.RevealOpponentHand(ctx);
                var usable = new List<CardInstance>();
                var seen = new HashSet<string>();
                foreach (var card in ctx.Opp.Hand)
                {
                    if (!card.Def.IsSupporter || card.Def.Name == "Sabrina's Suggestion" || !seen.Add(card.Def.Name)) continue;
                    if (ctx.Engine.Scripts.For(card.Def).CanPlayTrainer(new EffectContext(ctx.Engine, ctx.Player, card, null))) usable.Add(card);
                }
                if (usable.Count == 0) return;
                var labels = usable.Select(x => "Use " + x.Def.Name + "'s effect").ToList();
                labels.Add("Don't use a Supporter's effect");
                int pick = ctx.ChooseOption("Sabrina's Suggestion: use the effect of a Supporter from your opponent's hand?", labels.ToArray());
                if (pick >= usable.Count) return;
                var chosen = usable[pick];
                ctx.Log("Sabrina's Suggestion: " + ctx.Engine.PlayerName(ctx.Player) + " uses the effect of " + chosen.Def.Name + ".");
                ctx.Engine.Scripts.For(chosen.Def).ResolveTrainer(new EffectContext(ctx.Engine, ctx.Player, chosen, null));
            }
        }

        // ================================================================== Stadiums

        /// <summary>Brock's Pewter City Gym: Onix-GX (both players') take 40 less damage from the opponent's attacks (after Weakness and Resistance).</summary>
        private sealed class BrocksPewterCityGym : CardScript
        {
            public override int ModifyDamageTaken(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Target == null || info.Target.Name != "Onix-GX" || !info.AgainstOpponent || damage <= 0) return damage;
                return Math.Max(0, damage - 40);
            }
        }

        /// <summary>Misty's Cerulean City Gym: attacks of Starmie-GX (both players') do 40 more damage to the opponent's Active Pokémon.</summary>
        private sealed class MistysCeruleanCityGym : CardScript
        {
            public override int ModifyDamageDealt(EffectContext src, DamageInfo info, int damage)
            {
                if (info.Attacker == null || info.Attacker.Name != "Starmie-GX" || !info.ToOpponentsActive || damage <= 0) return damage;
                return damage + 40;
            }
        }
    }
}
