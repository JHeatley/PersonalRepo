using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PokemonTCG.Core
{
    /// <summary>The building blocks card scripts use. Each one emits events and applies the relevant rules checks.</summary>
    public sealed partial class GameEngine
    {
        /// <summary>Boomerang Energy discarded by its holder's own attack, to re-attach after the attack.</summary>
        private readonly List<KeyValuePair<PokemonInPlay, CardInstance>> returnAfterAttack = new List<KeyValuePair<PokemonInPlay, CardInstance>>();

        /// <summary>
        /// "If this Pokémon is damaged by an attack" triggers on the Defending side (Handheld Fan, Counterattack Grouping,
        /// Strong Bash...) wait until the attack's own effects are done (Compendium: Aqua Return vs Handheld Fan). They're
        /// queued here while an attack resolves.
        /// </summary>
        private readonly List<KeyValuePair<DamageInfo, int>> pendingDamageTriggers = new List<KeyValuePair<DamageInfo, int>>();
        private int attacksResolving;
        /// <summary>Tails the attacking player has flipped for the damage or effects of attacks (Blunder Policy, v54).</summary>
        private int attackTailsFlipped;

        // ------------------------------------------------------------------ Deck, hand, coins

        public int DrawCards(int player, int count)
        {
            var p = State.Players[player];
            // Milotic's Dew Guard (v58): the other player's Supporter can't make this player draw.
            if (count > 0 && SupporterEffectsPreventedOn(player))
            {
                Emit(GameEventType.Info, player, PlayerName(player) + " is protected from the effects of " + ResolvingTrainer.Name + " and doesn't draw.");
                return 0;
            }
            int drawn = 0;
            for (int k = 0; k < count && p.Deck.Count > 0; k++)
            {
                var c = p.Deck[0];
                p.Deck.RemoveAt(0);
                p.Hand.Add(c);
                drawn++;
            }
            if (drawn > 0)
                Emit(GameEventType.Draw, player, PlayerName(player) + " drew " + drawn + (drawn == 1 ? " card." : " cards."), -1, -1, drawn);
            return drawn;
        }

        public bool FlipCoin(int player, string reason)
        {
            willUsedForFlip = false;
            bool heads = FlipOne(player, reason);
            if (OfferAttackReflip(player, heads ? 1 : 0, 1, reason))
            {
                WillAgainForReflip(player);
                heads = FlipOne(player, reason + " (flipped again)");
            }
            return heads;
        }

        /// <summary>Flips a number of coins and returns how many came up heads.</summary>
        public int FlipCoins(int player, int count, string reason)
        {
            willUsedForFlip = false;
            int heads = FlipMany(player, count, reason);
            if (count > 0 && OfferAttackReflip(player, heads, count, reason))
            {
                WillAgainForReflip(player);
                heads = FlipMany(player, count, reason + " (flipped again)");
            }
            return heads;
        }

        /// <summary>"Flip a coin until you get tails." Returns the number of heads.</summary>
        public int FlipUntilTails(int player, string reason)
        {
            willUsedForFlip = false;
            int heads = FlipUntilTailsOnce(player, reason);
            if (OfferAttackReflip(player, heads, -1, reason))
            {
                WillAgainForReflip(player);
                heads = FlipUntilTailsOnce(player, reason + " (flipped again)");
            }
            return heads;
        }

        /// <summary>Will (v68): the flip being made used the player's Will choice.</summary>
        private bool willUsedForFlip;

        /// <summary>
        /// Will + Glimwood Tangle (Compendium, 2020-09-24): when a flip that used Will is flipped again, the player calls the first
        /// coin of the new flip too.
        /// </summary>
        private void WillAgainForReflip(int player)
        {
            if (!willUsedForFlip) return;
            willUsedForFlip = false;
            State.Players[player].WillCoinChoice = true;
        }

        private bool FlipOne(int player, string reason)
        {
            bool heads;
            if (WillApplies(player, reason))
            {
                // Will (v68): the player calls this coin. A coin that's forced to tails is still tails (Compendium: Contrary).
                State.Players[player].WillCoinChoice = false;
                willUsedForFlip = true;
                var opts = new List<DecisionOption> { new DecisionOption { Label = "Heads", Index = 1 }, new DecisionOption { Label = "Tails", Index = 0 } };
                heads = AskOne(player, DecisionKind.ChooseOption, "Will: choose heads or tails for this coin (" + reason + ")", opts, null) == 0;
                Emit(GameEventType.Info, player, PlayerName(player) + " chose " + (heads ? "heads" : "tails") + " (Will).");
            }
            else heads = State.Rng.CoinFlip();
            if (heads && FlipsForcedTails(player))
            {
                Emit(GameEventType.CoinFlip, player, reason + ": tails (every coin flip counts as tails).", -1, -1, 0);
                NoteAttackTails(player);
                return false;
            }
            Emit(GameEventType.CoinFlip, player, reason + ": " + (heads ? "heads" : "tails") + ".", -1, -1, heads ? 1 : 0);
            if (!heads) NoteAttackTails(player);
            return heads;
        }

        /// <summary>
        /// Blunder Policy: counts tails the attacking player flips while their attack's effects resolve. Flips before that (Sand
        /// Attack, Confusion) and the opponent's flips don't count.
        /// </summary>
        private void NoteAttackTails(int player)
        {
            if (attacksResolving > 0 && player == State.CurrentPlayer && CurrentSourceKind == EffectSourceKind.Attack) attackTailsFlipped++;
        }

        /// <summary>
        /// Will (v68): the next coin flip this turn for the effect of an attack, Ability or Trainer card. Not Special Condition flips
        /// (Pokémon Checkup, Confusion), which aren't card effects (Compendium, 2019-10-31).
        /// </summary>
        private bool WillApplies(int player, string reason)
        {
            if (player < 0 || State.CurrentPlayer != player || State.InCheckup || !State.Players[player].WillCoinChoice) return false;
            return reason == null || !reason.EndsWith(" is Confused");
        }

        /// <summary>Psyduck's Overthink: during that player's turn, their coin flips are all tails.</summary>
        private bool FlipsForcedTails(int player)
        {
            if (player < 0 || State.CurrentPlayer != player || State.InCheckup) return false;
            foreach (var e in ActiveEffects())
                if (e.ForcesTails(this, player) && !AttackEffectOnPlayerIgnored(e, player)) return true;
            // Malamar's Contrary (v107): an Ability that makes the opponent's coin flips during their turn tails.
            foreach (var src in Sources())
                if (src.Script.ForcesTailsFor(src.Ctx, player)) return true;
            return false;
        }

        private int FlipMany(int player, int count, string reason)
        {
            int heads = 0;
            for (int i = 0; i < count; i++) if (FlipOne(player, reason + " (" + (i + 1) + "/" + count + ")")) heads++;
            return heads;
        }

        private int FlipUntilTailsOnce(int player, string reason)
        {
            int heads = 0;
            while (heads < 100 && FlipOne(player, reason)) heads++;
            return heads;
        }

        /// <summary>
        /// Backtrack Badge: "Once during your turn, after you flip any coins for an attack of the Pokémon this card is
        /// attached to, you may ignore all results of those coin flips and begin flipping those coins again."
        /// coins is -1 for "flip until you get tails". Returns true when the player flips again.
        /// </summary>
        private bool OfferAttackReflip(int player, int heads, int coins, string reason)
        {
            if (State.CurrentPlayer != player || CurrentSourceKind != EffectSourceKind.Attack || CurrentSourceOwner != player) return false;
            var mon = CurrentSourcePokemon;
            if (mon == null || State.FindPokemon(mon.Uid) != mon) return false;
            foreach (var src in SourcesOn(mon))
            {
                if (!src.Script.OffersAttackReflip(src.Ctx)) continue;
                string limit = "Reflip|" + src.Ctx.Card.Def.Name;
                if (src.Ctx.AbilityUsedThisTurn(limit)) continue;
                string result = coins < 0 ? heads + " heads before tails" : heads + " of " + coins + " heads";
                if (!AskYesNo(player, src.Ctx.Card.Def.Name + ": " + reason + " came up " + result + ". Ignore the results and flip again?", src.Ctx.Card.Def)) return false;
                src.Ctx.MarkAbilityUsed(limit);
                Emit(GameEventType.Info, player, OwnedName(mon) + "'s " + src.Ctx.Card.Def.Name + ": the coins are flipped again.", src.Ctx.Card.Id, mon.Uid);
                return true;
            }
            // Sabrina's ESP (v122): a timed effect on the attacking Pokémon; once per turn.
            foreach (var fx in ActiveEffects())
            {
                if (fx.Owner != player || !fx.OffersAttackReflip(this, mon)) continue;
                string espLimit = "Reflip|Effect|" + fx.Name;
                int usedTurn;
                var me = State.Players[player];
                if (me.NamedLimitUsedOnTurn.TryGetValue(espLimit, out usedTurn) && usedTurn == State.TurnNumber) continue;
                string espResult = coins < 0 ? heads + " heads before tails" : heads + " of " + coins + " heads";
                if (!AskYesNo(player, fx.Name + ": " + reason + " came up " + espResult + ". Ignore the results and flip again?")) return false;
                me.NamedLimitUsedOnTurn[espLimit] = State.TurnNumber;
                Emit(GameEventType.Info, player, OwnedName(mon) + ": the coins are flipped again (" + fx.Name + ").", -1, mon.Uid);
                return true;
            }
            // Victini's Victory Star (v89): an Ability anywhere on the player's side; 1 Victory Star per turn, and only 1 re-flip effect
            // for the same flips (Compendium, Glimwood Tangle, 2020-09-24).
            foreach (var src in Sources())
            {
                if (src.Kind != EffectSourceKind.Ability || src.Ctx.Player != player) continue;
                string ability = src.Script.OwnAttacksReflipAbility(src.Ctx);
                if (ability == null || src.Ctx.NamedLimitUsedThisTurn("Reflip|" + ability)) continue;
                string flips = coins < 0 ? heads + " heads before tails" : heads + " of " + coins + " heads";
                if (!AskYesNo(player, ability + ": " + reason + " came up " + flips + ". Ignore the results and flip again?", src.Ctx.Card.Def)) return false;
                src.Ctx.MarkNamedLimit("Reflip|" + ability);
                Emit(GameEventType.Info, player, OwnedName(src.Ctx.Pokemon) + "'s " + ability + ": the coins are flipped again.", src.Ctx.Card.Id, src.Ctx.Pokemon == null ? -1 : src.Ctx.Pokemon.Uid);
                return true;
            }
            // Glimwood Tangle (v64): once during each player's turn, from the Stadium in play.
            if (State.Stadium != null && !StadiumSwitchedOff() && !StadiumShielded(mon))
            {
                var stadium = Scripts.For(State.Stadium.Def);
                var pctx = new EffectContext(this, player, State.Stadium, mon);
                string limit = "Reflip|Stadium|" + State.Stadium.Def.Name;
                if (stadium.StadiumOffersAttackReflip(pctx, mon) && !pctx.NamedLimitUsedThisTurn(limit))
                {
                    string result = coins < 0 ? heads + " heads before tails" : heads + " of " + coins + " heads";
                    if (!AskYesNo(player, State.Stadium.Def.Name + ": " + reason + " came up " + result + ". Ignore the results and flip again?", State.Stadium.Def)) return false;
                    pctx.MarkNamedLimit(limit);
                    Emit(GameEventType.Info, player, State.Stadium.Def.Name + ": the coins are flipped again.", State.Stadium.Id, mon.Uid);
                    return true;
                }
            }
            return false;
        }

        public void ShuffleDeck(int player)
        {
            ShowLookedAtIfUnseen(); // v128: before the cards it looked at go back in
            State.Rng.Shuffle(State.Players[player].Deck);
            ForgetLookedAt();
            Emit(GameEventType.Shuffle, player, PlayerName(player) + " shuffled " + Their(player) + " deck.");
        }

        public void ShuffleHandIntoDeck(int player)
        {
            var p = State.Players[player];
            if (SupporterEffectsPreventedOn(player))
            {
                Emit(GameEventType.Info, player, PlayerName(player) + " is protected from the effects of " + ResolvingTrainer.Name + " and keeps " + Their(player) + " hand.");
                return;
            }
            int n = p.Hand.Count;
            p.Deck.AddRange(p.Hand);
            p.Hand.Clear();
            State.Rng.Shuffle(p.Deck);
            Emit(GameEventType.Shuffle, player, PlayerName(player) + " shuffled " + n + (n == 1 ? " card" : " cards") + " from " + Their(player) + " hand into " + Their(player) + " deck.");
        }

        public void DiscardHand(int player)
        {
            var p = State.Players[player];
            int n = p.Hand.Count;
            var cards = p.Hand.ToList();
            p.Discard.AddRange(cards); // a Prism Star card goes to the Lost Zone instead (v80)
            p.Hand.Clear();
            if (n > 0) EmitCards(GameEventType.Discard, player, PlayerName(player) + " discarded " + Their(player) + " hand (" + n + (n == 1 ? " card)." : " cards)."), cards);
        }

        /// <summary>Discards the top cards of a player's deck. Returns the cards discarded.</summary>
        public List<CardInstance> DiscardTopOfDeck(int player, int count)
        {
            var p = State.Players[player];
            var list = new List<CardInstance>();
            if (count > 0 && p.Deck.Count > 0 && DeckDiscardBlocked(player))
            {
                Emit(GameEventType.Info, player, "Cards in " + Their(player) + " deck can't be discarded (Patrol Cap).");
                return list;
            }
            for (int k = 0; k < count && p.Deck.Count > 0; k++)
            {
                var c = p.Deck[0];
                p.Deck.RemoveAt(0);
                p.Discard.Add(c);
                list.Add(c);
            }
            if (list.Count > 0)
                EmitCards(GameEventType.Discard, player, PlayerName(player) + " discarded " + string.Join(", ", list.Select(c => c.Def.Name).ToArray()) + " from the top of " + Their(player) + " deck.", list);
            // "If this Pokémon is discarded from your deck by your opponent's effects during their turn" (Startling Drop).
            bool byOpponent = State.CurrentPlayer != player && CurrentSourceOwner == 1 - player
                              && (CurrentSourceKind == EffectSourceKind.Attack || CurrentSourceKind == EffectSourceKind.Ability
                                  || CurrentSourceKind == EffectSourceKind.PokemonPower || CurrentSourceKind == EffectSourceKind.Trainer);
            if (byOpponent)
                foreach (var c in list)
                    Scripts.For(c.Def).OnDiscardedFromDeckByOpponent(new EffectContext(this, player, c, null));
            return list;
        }

        /// <summary>Lets the player pick up to max matching cards from their deck. Removes them from the deck; the caller places them.</summary>
        /// <param name="min">At least this many must be taken if they're there: for "search your deck for a card" with no
        /// condition, a card can't fail to be found (Compendium, Shinobi Blade). Searches with a condition can find nothing.</param>
        public List<CardInstance> SearchDeck(int player, Func<CardDef, bool> filter, int max, string prompt, CardDef source, bool reveal, int min = 0)
        {
            var p = State.Players[player];
            Emit(GameEventType.Search, player, PlayerName(player) + " searched " + Their(player) + " deck" + (source != null ? " (" + source.Name + ")." : "."));
            var matches = p.Deck.Where(c => filter(c.Def)).ToList();
            if (matches.Count == 0 || max <= 0)
            {
                if (max > 0 && p.Deck.Count > 0 && IsInteractive(player))
                {
                    // Nothing can be taken, but a person still gets to look through the deck (press Done to go on).
                    seenForNextAsk = p.Deck.ToList();
                    seenTitleForNextAsk = "Your deck";
                    try { Ask(player, DecisionKind.ChooseCards, prompt + " (no cards in your deck can be chosen)", new List<DecisionOption>(), 0, 0, source); }
                    finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
                }
                if (max > 0) Emit(GameEventType.Info, player, PlayerName(player) + " found nothing.");
                return new List<CardInstance>();
            }
            var opts = matches.Select(c => new DecisionOption { Label = c.Def.Name, CardId = c.Id, Card = c.Def }).ToList();
            List<int> picks;
            seenForNextAsk = p.Deck.ToList(); // "All" tab: the whole deck (Prize cards aren't in it)
            seenTitleForNextAsk = "Your deck";
            try { picks = AskMany(player, DecisionKind.ChooseCards, prompt, opts, Math.Min(min, Math.Min(max, matches.Count)), max, source); }
            finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
            var chosen = picks.Select(i => matches[i]).ToList();
            foreach (var c in chosen) p.Deck.Remove(c);
            if (reveal && chosen.Count > 0) Reveal(player, chosen, "revealed");
            else if (chosen.Count > 0) Emit(GameEventType.Info, player, PlayerName(player) + " took " + chosen.Count + (chosen.Count == 1 ? " card" : " cards") + " from " + Their(player) + " deck.");
            else Emit(GameEventType.Info, player, PlayerName(player) + " didn't take any cards.");
            return chosen;
        }

        public List<CardInstance> ChooseCards(int player, IList<CardInstance> from, int min, int max, string prompt, CardDef source)
        {
            if (from.Count == 0 && max > 0) ShowLookedAtWithNothingToTake(player, prompt, source);
            if (from.Count == 0 || max <= 0) return new List<CardInstance>();
            var opts = from.Select(c => new DecisionOption { Label = c.Def.Name, CardId = c.Id, Card = c.Def }).ToList();
            List<int> picks;
            string title;
            seenForNextAsk = SeenFor(player, from, out title);
            seenTitleForNextAsk = title;
            // Taking cards out of your own discard pile (into the hand or deck, or onto a Pokémon): a person always picks them.
            bool fromMyDiscard = IsInteractive(player) && from.All(c => State.Players[player].Discard.Contains(c));
            try { picks = AskMany(player, DecisionKind.ChooseCards, prompt, opts, min, max, source, fromMyDiscard); }
            finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
            return picks.Select(i => from[i]).ToList();
        }

        /// <summary>
        /// An effect looked at cards (the top 7 for Bug Catching Set, Pokégear 3.0...) and none of them can be taken: the
        /// person on screen still sees them (press Done to go on), so it's clear the card worked and what it found, like a
        /// search that finds nothing (v45). Bots aren't asked.
        /// </summary>
        /// <summary>
        /// Shows a person the cards an effect turned over for them (PokéStop's discards, v69) and waits for Done. Nothing for a
        /// bot: they see the same cards in the game log.
        /// </summary>
        public void ShowCards(int player, IList<CardInstance> cards, string title, string prompt, CardDef source)
        {
            if (!IsInteractive(player) || cards == null || cards.Count == 0) return;
            seenForNextAsk = cards.ToList();
            seenTitleForNextAsk = title;
            try { Ask(player, DecisionKind.ChooseCards, prompt, new List<DecisionOption>(), 0, 0, source); }
            finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
        }

        private void ShowLookedAtWithNothingToTake(int player, string prompt, CardDef source)
        {
            if (!IsInteractive(player) || lookedAt == null || lookedAtPlayer != player || lookedAt.Count == 0) return;
            seenForNextAsk = lookedAt.ToList();
            seenTitleForNextAsk = lookedAtTitle;
            try { Ask(player, DecisionKind.ChooseCards, prompt + " (none of these cards can be chosen)", new List<DecisionOption>(), 0, 0, source); }
            finally { seenForNextAsk = null; seenTitleForNextAsk = null; }
        }

        public PokemonInPlay ChoosePokemon(int chooser, IList<PokemonInPlay> candidates, string prompt, CardDef source, DecisionKind kind = DecisionKind.ChoosePokemon)
        {
            if (candidates.Count == 0) return null;
            var opts = candidates.Select(m => new DecisionOption
            {
                Label = DescribeFor(chooser, m),
                PokemonUid = m.Uid,
                CardId = m.Top.Id,
                Card = m.Def
            }).ToList();
            return candidates[AskOne(chooser, kind, prompt, opts, source)];
        }

        public void DiscardFromHand(int player, CardInstance card)
        {
            var p = State.Players[player];
            if (SupporterEffectsPreventedOn(player) && p.Hand.Contains(card))
            {
                Emit(GameEventType.Info, player, PlayerName(player) + " is protected from the effects of " + ResolvingTrainer.Name + "; " + card.Def.Name + " stays in " + Their(player) + " hand.");
                return;
            }
            p.Hand.Remove(card);
            p.Discard.Add(card);
            Emit(GameEventType.Discard, player, PlayerName(player) + " discarded " + card.Def.Name + ".", card.Id);
        }

        // ------------------------------------------------------------------ Putting Pokémon into play

        /// <summary>True while a Stadium's OnPokemonPutOnBench runs for a Pokémon played from the hand (Gapejaw Bog, v50).</summary>
        public bool BenchingFromHand { get; private set; }

        /// <summary>
        /// Set while an Ability or Pokémon Power switches in 1 of the opponent's Benched Pokémon that its user chose (Bloodthirsty
        /// Eyes): only that Benched Pokémon is targeted, so an effect protecting the Active Pokémon doesn't stop it (Stealthy Hood:
        /// Compendium, 2019-05-30).
        /// </summary>
        public bool PowerGustInProgress;

        /// <summary>
        /// Puts a card straight into the empty Active Spot (Clefairy's Doll Swap plays Lillie's Poké Doll "as your new Active Pokémon",
        /// v68). Returns null when the Active Spot isn't empty.
        /// </summary>
        public PokemonInPlay PutOntoActive(int player, CardInstance card)
        {
            var p = State.Players[player];
            if (p.Active != null) return null;
            var mon = new PokemonInPlay(State.NextUid++, player, card, State.TurnNumber);
            p.Active = mon;
            mon.MovedToActiveOnTurn = State.TurnNumber;
            if (!State.InSetup) mon.PlaySeq = mon.SpotSeq = NextLockSeq();
            Emit(GameEventType.PutOnBench, player, PlayerName(player) + " put " + card.Def.Name + " into the Active Spot.", card.Id, mon.Uid);
            return mon;
        }

        /// <summary>
        /// Puts a Pokémon card onto the player's Bench. fromHand: the player plays it from their hand (its own "when you play this
        /// Pokémon from your hand" effects happen). fromHandByEffect: another effect puts it there from its owner's hand (Erika's
        /// Invitation, Captivating Wink): Stadiums that react to a Pokémon put onto the Bench from the hand still see it (Gapejaw
        /// Bog: Compendium, 2023-09), but it isn't "played".
        /// </summary>
        /// <summary>The next step in the order of events that can turn an Ability lock on (PokemonInPlay.PlaySeq and friends).</summary>
        public int NextLockSeq() { return ++State.LockSeq; }

        public PokemonInPlay PutOntoBench(int player, CardInstance card, bool fromHand, bool fromHandByEffect = false)
        {
            var p = State.Players[player];
            int max = State.InSetup ? PlayerState.DefaultMaxBench : MaxBench(player);
            if (p.Bench.Count >= max) throw new InvalidOperationException("Bench is full.");
            var mon = new PokemonInPlay(State.NextUid++, player, card, State.TurnNumber);
            p.Bench.Add(mon);
            if (!State.InSetup) mon.PlaySeq = mon.SpotSeq = NextLockSeq();
            if (State.InSetup)
            {
                Emit(GameEventType.PutOnBench, player, PlayerName(player) + " placed a Benched Pokémon face down.", -1, mon.Uid);
                return mon;
            }
            Emit(GameEventType.PutOnBench, player, PlayerName(player) + " put " + card.Def.Name + " onto the Bench.", card.Id, mon.Uid);
            // A Stadium's "when a Pokémon is put onto the Bench" effect (Gapejaw Bog) and the Pokémon's own "when you play this
            // Pokémon from your hand onto your Bench" effect (Luminous Sign) happen at the same time: the player chooses the order
            // (Compendium, 2023-07-13). Without both, the Stadium goes first as before.
            bool stadiumTrigger = State.Stadium != null && !StadiumSwitchedOff();
            // The cheap checks first: almost no Pokémon or Stadium has these effects.
            bool ownFirst = fromHand && stadiumTrigger
                && Overrides(Scripts.For(card.Def), "OnPlayedFromHandToBench") && Overrides(Scripts.For(State.Stadium.Def), "OnPokemonPutOnBench")
                && !AbilitiesBlocked(mon) && AskOwnBenchEffectFirst(player, card.Def, State.Stadium.Def);
            if (ownFirst) PlayedToBenchEffect(player, card, mon);
            if (stadiumTrigger && p.Bench.Contains(mon) && State.Stadium != null && !StadiumSwitchedOff())
            {
                PushSource(EffectSourceKind.Stadium, State.StadiumOwner, null);
                BenchingFromHand = fromHand || fromHandByEffect;
                try { Scripts.For(State.Stadium.Def).OnPokemonPutOnBench(StadiumContext(), mon); }
                finally { PopSource(); BenchingFromHand = false; }
            }
            if (fromHand && !ownFirst) PlayedToBenchEffect(player, card, mon);
            // "Whenever you play a Pokémon ... from your hand" on another of the player's Pokémon (Bursting Spores, v72).
            if (fromHand && !State.IsOver && p.Bench.Contains(mon))
            {
                foreach (var src in Sources())
                {
                    if (State.IsOver) break;
                    var holder = src.Ctx.Pokemon;
                    if (src.Ctx.Player != player || holder == null || holder == mon || State.FindPokemon(holder.Uid) != holder || holder.Top != src.Ctx.Card) continue;
                    PushSource(src.Kind, player, holder);
                    try { src.Script.OnOwnPokemonPlayedFromHandToBench(src.Ctx, mon); }
                    finally { PopSource(); }
                }
            }
            // v142: "whenever a player puts a Baby Pokémon or Basic Pokémon onto their Bench from their hand" (Hot Plate): played,
            // or put there from the hand by an effect (Provoke, Eerie Howl; your ruling).
            if ((fromHand || fromHandByEffect) && !State.IsOver && p.Bench.Contains(mon))
            {
                foreach (var src in Sources())
                {
                    if (State.IsOver || !p.Bench.Contains(mon)) break;
                    if (!StillInPlay(src)) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnPokemonBenchedFromHand(src.Ctx, mon); }
                    finally { PopSource(); }
                }
            }
            return mon;
        }

        /// <summary>"When you play this Pokémon from your hand onto your Bench" (Abilities such as Luminous Sign), unless its Abilities are off.</summary>
        private void PlayedToBenchEffect(int player, CardInstance card, PokemonInPlay mon)
        {
            if (!State.Players[player].Bench.Contains(mon) || AbilitiesBlocked(mon)) return;
            PushSource(card.Def.AbilitySourceKind, player, mon);
            var benchShield = HasOldPokemonPower(card.Def) ? BeginDiscardShield() : null; // Allergic Pollen (v141)
            try { Scripts.For(card.Def).OnPlayedFromHandToBench(new EffectContext(this, player, card, mon)); }
            finally { EndDiscardShield(benchShield); PopSource(); }
        }

        /// <summary>Asks the player whether the Pokémon's own effect goes before the Stadium's (the first option, a bot's default).</summary>
        private bool AskOwnBenchEffectFirst(int player, CardDef pokemon, CardDef stadium)
        {
            var opts = new List<DecisionOption>
            {
                new DecisionOption { Label = pokemon.Name + "'s effect first", Card = pokemon, Index = 0 },
                new DecisionOption { Label = stadium.Name + " first", Card = stadium, Index = 1 }
            };
            int pick = AskOne(player, DecisionKind.ChooseOption, "These happen at the same time: which goes first?", opts, pokemon);
            return pick != 1;
        }

        /// <summary>True when the script (not the CardScript default) implements the named hook.</summary>
        /// <summary>Whether a script's class overrides a CardScript method. Asked often (every Pokémon Checkup), so the answer is cached per class.</summary>
        private static bool Overrides(CardScript script, string method)
        {
            if (script == null) return false;
            var key = (script.GetType(), method);
            bool result;
            if (OverridesCache.TryGetValue(key, out result)) return result;
            var m = key.Item1.GetMethod(method);
            result = m != null && m.DeclaringType != typeof(CardScript);
            OverridesCache[key] = result;
            return result;
        }

        /// <summary>Shared by every engine, including the ones the AI tables run on other threads.</summary>
        private static readonly ConcurrentDictionary<(Type, string), bool> OverridesCache = new ConcurrentDictionary<(Type, string), bool>();

        /// <summary>Puts an evolution card on a Pokémon. fromHand = played from the hand (or with Rare Candy), which triggers "when you play this Pokémon to evolve" Abilities.</summary>
        public void EvolvePokemon(PokemonInPlay target, CardInstance evolution, bool fromHand = false)
        {
            // Aerodactyl's Prehistoric Power (v116, your ruling): no Evolution card goes onto a Pokémon by any means.
            if (PrehistoricPowerActive())
            {
                ReturnBlockedEvolution(target, evolution, fromHand);
                return;
            }
            // Brock's Ninetales's Shapeshift (v126): it can't evolve while it's treated as another Pokémon.
            if (ShapeshiftCard(target) != null)
            {
                var owner = State.Players[evolution.Owner];
                if (fromHand) owner.Hand.Add(evolution);
                else
                {
                    owner.Deck.Add(evolution);
                    State.Rng.Shuffle(owner.Deck);
                }
                Emit(GameEventType.Info, target.Owner, target.Name + " can't evolve (Shapeshift).", evolution.Id, target.Uid);
                return;
            }
            string from = target.Name;
            target.Stack.Add(evolution);
            target.TurnEvolved = State.TurnNumber;
            target.PlaySeq = NextLockSeq();
            target.Conditions = ConditionsAfterEvolving(target);
            // A BREAK Pokémon keeps the Abilities it had, so a once-per-turn one already used this turn stays used (Greninja
            // BREAK's Water Shuriken: Compendium, 2016-02-11; v92).
            if (!evolution.Def.IsBreak) target.AbilityUsedOnTurn.Clear();
            target.ClearMarkersOnEvolve(); // Lightning Rod markers and Coloring counters go away when it evolves (your ruling, v48); a Char counter stays (v138)
            ClearEffectsOn(target);
            // "If this Pokémon moved from your Bench to the Active Spot this turn": the Evolution card didn't move, so it
            // doesn't count once evolved (Compendium: Wimpod then Golisopod GX's First Impression).
            target.MovedToActiveOnTurn = -100;
            Emit(GameEventType.Evolve, target.Owner, PlayerName(target.Owner) + " evolved " + from + " into " + evolution.Def.Name + ".", evolution.Id, target.Uid);
            // θ Max (Ancient Trait, v98): "When 1 of your Pokémon becomes this Pokémon, heal all damage from it." Not an Ability.
            if (evolution.Def.AncientTrait == ThetaMax && target.DamageCounters > 0)
            {
                int healed = target.DamageCounters;
                target.DamageCounters = 0;
                Emit(GameEventType.Info, target.Owner, evolution.Def.Name + " (θ Max): healed all " + healed * 10 + " damage.", evolution.Id, target.Uid);
            }
            // XY-era Mega Evolution Pokémon-EX: "When 1 of your Pokémon becomes a Mega Evolution Pokémon, your turn ends." Only
            // the XY-era Megas: today's Mega Evolution Pokémon ex don't end the turn (your rules update, v92).
            if (evolution.Def.IsXYMegaEvolution && !State.InSetup && target.Owner == State.CurrentPlayer)
            {
                // Spirit Link Tools (Evolutions, v92): "Your turn doesn't end if the Pokémon this card is attached to becomes M ...-EX."
                var link = SpiritLinkFor(target, evolution.Def);
                if (link != null) Emit(GameEventType.Info, target.Owner, link.Def.Name + ": " + PlayerName(target.Owner) + "'s turn doesn't end.", link.Id, target.Uid);
                else RequestEndTurn();
            }
            if (fromHand && !AbilitiesBlocked(target))
            {
                PushSource(evolution.Def.AbilitySourceKind, target.Owner, target);
                var evolveShield = HasOldPokemonPower(evolution.Def) ? BeginDiscardShield() : null; // Allergic Pollen (v141)
                try { Scripts.For(evolution.Def).OnEvolvedFromHand(new EffectContext(this, target.Owner, evolution, target)); }
                finally { EndDiscardShield(evolveShield); PopSource(); }
            }
            // "Whenever you play a Pokémon from your hand to evolve the Pokémon V this card is attached to" (Regenerative
            // Energy, v50): the Tools and Special Energy on the evolved Pokémon, told what it was before.
            if (fromHand && !State.InSetup && State.FindPokemon(target.Uid) == target && target.Stack.Count >= 2)
            {
                var before = target.Stack[target.Stack.Count - 2].Def;
                foreach (var src in SourcesOn(target))
                {
                    if (src.Kind != EffectSourceKind.Energy && src.Kind != EffectSourceKind.Trainer) continue;
                    PushSource(src.Kind, target.Owner, target);
                    try { src.Script.OnHolderEvolvedFromHand(src.Ctx, before); }
                    finally { PopSource(); }
                }
            }
            // "Whenever your opponent evolves 1 of their Pokémon by playing a Pokémon from their hand" (Darkest Impulse).
            if (fromHand && !State.InSetup)
            {
                foreach (var src in Sources())
                {
                    if (src.Ctx.Player == target.Owner || State.FindPokemon(target.Uid) != target) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnOpponentEvolvedFromHand(src.Ctx, target); }
                    finally { PopSource(); }
                }
            }
            // "When you play a Pokémon from your hand to evolve 1 of your other Eevee" (Resonant Evolution, v53): the owner's
            // sources, each still in play as the card that was asked. An Eevee evolved by one of them isn't from the hand.
            if (fromHand && !State.InSetup && !State.IsOver && target.Stack.Count >= 2)
            {
                var before = target.Stack[target.Stack.Count - 2].Def;
                foreach (var src in Sources())
                {
                    if (State.IsOver) break;
                    var holder = src.Ctx.Pokemon;
                    if (src.Ctx.Player != target.Owner || holder == null || State.FindPokemon(holder.Uid) != holder || holder.Top != src.Ctx.Card) continue;
                    PushSource(src.Kind, src.Ctx.Player, holder);
                    try { src.Script.OnOwnPokemonEvolvedFromHand(src.Ctx, target, before); }
                    finally { PopSource(); }
                }
            }
            // "Whenever either player plays a Pokémon VMAX from their hand to evolve 1 of their Pokémon V" (Wyndon Stadium, v62).
            if (fromHand && !State.InSetup && !State.IsOver && State.Stadium != null && !StadiumSwitchedOff() && State.FindPokemon(target.Uid) == target
                && target.Stack.Count >= 2 && !StadiumShielded(target))
            {
                var before = target.Stack[target.Stack.Count - 2].Def;
                PushSource(EffectSourceKind.Stadium, State.StadiumOwner, null);
                try { Scripts.For(State.Stadium.Def).OnPokemonEvolvedFromHandUnderStadium(StadiumContext(), target, before); }
                finally { PopSource(); }
            }
            if (!State.InSetup && !State.IsOver) NotifyPokemonEvolved(target);
        }

        /// <summary>
        /// Eevee's Chain Reaction: "This power can only be used when a Pokémon evolves" - any Pokémon, either player's, evolved
        /// in any way (your ruling, v48). Every source still in play is asked; an Eevee evolved this way can set off the others.
        /// </summary>
        private void NotifyPokemonEvolved(PokemonInPlay evolved)
        {
            foreach (var src in Sources())
            {
                if (State.IsOver) return;
                var holder = src.Ctx.Pokemon;
                if (holder == null || holder == evolved || State.FindPokemon(holder.Uid) != holder || holder.Top != src.Ctx.Card) continue;
                PushSource(src.Kind, src.Ctx.Player, holder);
                try { src.Script.OnAnyPokemonEvolved(src.Ctx, evolved); }
                finally { PopSource(); }
            }
            // v126: the Stadium in play too (Viridian City Gym), unless it doesn't reach that Pokémon.
            if (!State.IsOver && State.Stadium != null && !StadiumSwitchedOff() && State.FindPokemon(evolved.Uid) == evolved && !StadiumShielded(evolved))
            {
                PushSource(EffectSourceKind.Stadium, State.StadiumOwner, null);
                try { Scripts.For(State.Stadium.Def).OnAnyPokemonEvolved(StadiumContext(), evolved); }
                finally { PopSource(); }
            }
        }

        /// <summary>Evolving or devolving removes Special Conditions, except Confusion while Dizzying Valley is in play.</summary>
        private SpecialCondition ConditionsAfterEvolving(PokemonInPlay mon)
        {
            // Sea of Nothingness (v86): nothing is removed.
            if (mon.Conditions != SpecialCondition.None)
                foreach (var src in SourcesFor(mon))
                    if (src.Script.KeepsConditionsOnEvolve(src.Ctx, mon)) return mon.Conditions;
            bool confused = (mon.Conditions & SpecialCondition.Confused) != 0;
            bool poisoned = (mon.Conditions & SpecialCondition.Poisoned) != 0;
            bool burned = (mon.Conditions & SpecialCondition.Burned) != 0;
            if (!confused && !poisoned && !burned) return SpecialCondition.None;
            var kept = SpecialCondition.None;
            var sources = SourcesFor(mon);
            // Heatran LV.X's Heat Metal (v136): the opponent's Burned Pokémon stay Burned.
            if (burned)
                foreach (var src in sources)
                    if (src.Script.KeepsBurnOnEvolve(src.Ctx, mon)) { kept |= SpecialCondition.Burned; break; }
            if (confused)
                foreach (var src in sources)
                    if (src.Script.KeepsConfusionOnEvolve(src.Ctx)) { kept |= SpecialCondition.Confused; break; }
            // Muk's Poison Sacs: the Poison stays as it was, Toxic Ripper's 6 counters included (your ruling, v41).
            if (poisoned)
                foreach (var src in sources)
                    if (src.Script.KeepsPoisonOnEvolve(src.Ctx, mon)) { kept |= SpecialCondition.Poisoned; break; }
            return kept;
        }

        /// <summary>Removes up to count Evolution cards from the top of the Pokémon and puts them into its owner's hand.</summary>
        public List<CardInstance> Devolve(PokemonInPlay mon, int count)
        {
            return Devolve(mon, count, false);
        }

        /// <summary>
        /// Devolve, with the removed Evolution cards shuffled into their owner's deck instead of going to the hand when toDeck is
        /// set (Porygon-Z's Downgrading Beam, v51).
        /// </summary>
        public List<CardInstance> Devolve(PokemonInPlay mon, int count, bool toDeck)
        {
            return DevolveTo(mon, count, toDeck ? DevolveToDeck : DevolveToHand);
        }

        /// <summary>v146: devolve, discarding the removed Evolution cards (Aquapolis's Undersea Ruins).</summary>
        public List<CardInstance> DevolveIntoDiscard(PokemonInPlay mon, int count)
        {
            return DevolveTo(mon, count, DevolveToDiscard);
        }

        /// <summary>v147: devolve, putting the removed Evolution cards on the bottom of their owner's deck (Mystery Plate γ's Retro Cave).</summary>
        public List<CardInstance> DevolveToBottomOfDeck(PokemonInPlay mon, int count)
        {
            return DevolveTo(mon, count, DevolveToDeckBottom);
        }

        private const int DevolveToHand = 0, DevolveToDeck = 1, DevolveToDiscard = 2, DevolveToDeckBottom = 3;

        private List<CardInstance> DevolveTo(PokemonInPlay mon, int count, int destination)
        {
            bool toDeck = destination == DevolveToDeck;
            var removed = new List<CardInstance>();
            if (mon == null || mon.Stack.Count < 2 || count <= 0) return removed;
            // Brock's Ninetales's Shapeshift (v126): while it's treated as another Pokémon it can't devolve.
            if (ShapeshiftCard(mon) != null)
            {
                Emit(GameEventType.Info, mon.Owner, mon.Name + " can't devolve (Shapeshift).", mon.Top.Id, mon.Uid);
                return removed;
            }
            if (CurrentEffectPrevented(mon, false))
            {
                Emit(GameEventType.DamagePrevented, mon.Owner, "The effect on " + mon.Name + " was prevented.", mon.Top.Id, mon.Uid);
                return removed;
            }
            if (destination == DevolveToHand && !CanPutIntoHand(mon))
            {
                Emit(GameEventType.Info, mon.Owner, "Cards from " + mon.Name + " can't be put into " + Their(mon.Owner) + " hand.", mon.Top.Id, mon.Uid);
                return removed;
            }
            string before = mon.Name;
            mon.PlaySeq = NextLockSeq();
            for (int k = 0; k < count && mon.Stack.Count > 1 && !mon.Def.IsMultiCard; k++)
            {
                var top = mon.Top;
                mon.Stack.RemoveAt(mon.Stack.Count - 1);
                removed.Add(top);
            }
            if (toDeck)
            {
                State.Players[mon.Owner].Deck.AddRange(removed);
                State.Rng.Shuffle(State.Players[mon.Owner].Deck);
            }
            else if (destination == DevolveToDiscard) State.Players[mon.Owner].Discard.AddRange(removed);
            else if (destination == DevolveToDeckBottom) State.Players[mon.Owner].Deck.AddRange(removed);
            else State.Players[mon.Owner].Hand.AddRange(removed);
            if (removed.Count > 0 && mon.HitByAttack) mon.DevolvedAfterHit = true;
            mon.Conditions = ConditionsAfterEvolving(mon);
            if (removed.Count > 0) mon.ClearMarkersOnEvolve(); // devolving works like evolving here (Devolution Beam: "as if you had evolved it")
            ClearEffectsOn(mon);
            Emit(GameEventType.Info, mon.Owner, before + " devolved into " + mon.Name + "; " + string.Join(", ", removed.Select(c => c.Def.Name).ToArray()) + " went to " + (PlayerName(mon.Owner) == "You" ? "your" : PlayerName(mon.Owner) + "'s") + (toDeck ? " deck, which was shuffled." : destination == DevolveToDiscard ? " discard pile." : destination == DevolveToDeckBottom ? " deck (on the bottom)." : " hand."), mon.Top.Id, mon.Uid);
            return removed;
        }

        // ------------------------------------------------------------------ Energy and Tools

        /// <summary>
        /// Attaches an Energy card. fromHand = attached from the hand (Telepathic Psychic Energy, Enriching Energy react to that).
        /// Returns false when the card wasn't attached: Corrosive Shards keeps it in the hand, Goodra's Slimy Room (tails)
        /// discards it, Aurora Energy with no other card in the hand stays there.
        /// </summary>
        public bool AttachEnergyCard(CardInstance energy, PokemonInPlay target, bool fromHand = false)
        {
            // Corrosive Shards: no Energy card can be attached from the hand to that Pokémon, by any means.
            if (fromHand && (!CanAttachFromHand(target) || EnergyFromHandBlockedTo(energy, target)))
            {
                State.Players[energy.Owner].Hand.Add(energy);
                Emit(GameEventType.Info, target.Owner, energy.Def.Name + " can't be attached from the hand to " + target.Name + "; it stays in the hand.", energy.Id, target.Uid);
                return false;
            }
            // Honchkrow-GX's Ruler of the Night stops any attaching of a Special Energy card from the hand (your ruling, v73).
            if (fromHand && EnergyCardBlockedBySource(energy.Owner, energy))
            {
                var blocked = State.Players[energy.Owner];
                if (!blocked.Hand.Contains(energy)) blocked.Hand.Add(energy);
                Emit(GameEventType.Info, energy.Owner, energy.Def.Name + " can't be attached from the hand right now; it stays in the hand.", energy.Id, target.Uid);
                return false;
            }
            // Aurora Energy (v66): attached from the hand only by discarding another card from the hand, however it's attached
            // from there (Compendium, 2020-02-06). Moving it between Pokémon isn't attaching from the hand (2022-02-24). With no
            // other card in the hand it can't even be tried, so it stays there.
            bool discardCost = fromHand && Scripts.For(energy.Def).DiscardToAttachFromHand(new EffectContext(this, energy.Owner, energy, target));
            if (discardCost && !State.Players[energy.Owner].Hand.Any(c => c != energy))
            {
                var owner = State.Players[energy.Owner];
                if (!owner.Hand.Contains(energy)) owner.Hand.Add(energy);
                Emit(GameEventType.Info, energy.Owner, energy.Def.Name + " needs another card from the hand to be discarded; it stays in the hand.", energy.Id, target.Uid);
                return false;
            }
            // Slimy Room flips when the player tries to attach (your ruling, v66): on tails the card is discarded instead, before
            // Aurora Energy's discard is paid; on heads the discard is paid and the card is attached.
            if (fromHand && SlimyRoomDiscards(energy, target)) return false;
            if (discardCost && !DiscardToAttach(energy, target)) return false;
            target.Energy.Add(energy);
            Emit(GameEventType.AttachEnergy, target.Owner, PlayerName(target.Owner) + " attached " + energy.Def.Name + " to " + target.Name + ".", energy.Id, target.Uid);
            if (fromHand)
            {
                var topAtAttach = target.Top;
                // Cards that react to it: your own (Magearna's Auto Heal) and the other player's (Gengar ex's Gnawing Curse).
                // The attaching player orders them: Solar Evolution first, and once it has evolved the Pokémon, the other
                // player's reactions don't apply to it (Compendium 2022-04-28, v63).
                var reacting = Sources();
                var ordered = new List<PassiveSource>();
                // v110: mandatory reactions that must come before an evolution (Frozen City's counters, Compendium 2014-08-28).
                foreach (var src in reacting)
                    if (src.Script.ReactsFirstToAttachFromHand) ordered.Add(src);
                foreach (var src in reacting)
                    if (src.Ctx.Player == target.Owner && src.Script.EvolvesOnOwnerAttachFromHand && !src.Script.ReactsFirstToAttachFromHand) ordered.Add(src);
                // The Energy card's own "when you attach this card from your hand" effect comes after an evolution the owner chose
                // first, and doesn't apply to the evolved Pokémon: Solar Evolution before Rainbow Energy's damage counter
                // (Compendium, 2022-04-28). Declining Solar Evolution leaves the counter as usual.
                int energyEffectAt = ordered.Count;
                // v86: then the opponent's reactions before the owner's other ones. The owner chooses the order of their own and
                // the opponent's triggers and is never worse off healing last (Compendium: Miltank's Moomoo Malt + Gengar's Gnawing
                // Curse, 2017-11-02: Gnawing Curse first, then Moomoo Malt heals before the Knock Out check).
                foreach (var src in reacting)
                    if (src.Ctx.Player != target.Owner && !src.Script.ReactsFirstToAttachFromHand) ordered.Add(src);
                foreach (var src in reacting)
                    if (src.Ctx.Player == target.Owner && !src.Script.EvolvesOnOwnerAttachFromHand && !src.Script.ReactsFirstToAttachFromHand) ordered.Add(src);
                for (int k = 0; k <= ordered.Count; k++)
                {
                    if (State.FindPokemon(target.Uid) != target) break;
                    if (k == energyEffectAt)
                    {
                        // Temple of Sinnoh: a Special Energy attached while it's in play has no effect, its "when attached" one included.
                        if (target.Top == topAtAttach && target.Energy.Contains(energy) && (!energy.Def.IsSpecialEnergy || !SpecialEnergyOff(target)))
                        {
                            PushSource(EffectSourceKind.Energy, target.Owner, target);
                            try { Scripts.For(energy.Def).OnAttachedFromHand(new EffectContext(this, target.Owner, energy, target)); }
                            finally { PopSource(); }
                        }
                    }
                    if (k == ordered.Count) break;
                    var src = ordered[k];
                    if (src.Ctx.Player != target.Owner && target.Top != topAtAttach) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try
                    {
                        if (src.Ctx.Player == target.Owner) src.Script.OnOwnerAttachedEnergyFromHand(src.Ctx, energy, target);
                        else src.Script.OnOpponentAttachedEnergyFromHand(src.Ctx, energy, target);
                    }
                    finally { PopSource(); }
                }
                foreach (var e in ActiveEffects().ToList())
                {
                    if (!State.Effects.Contains(e)) continue;
                    e.OnEnergyAttachedFromHand(this, energy, target);
                }
            }
            // v139: "Whenever you attach an Energy card to this Pokémon" (Eevee's Energy Evolution), from the hand or by any effect
            // of the Pokémon's owner during their turn.
            if (!State.InSetup && !State.IsOver && State.CurrentPlayer == target.Owner && State.FindPokemon(target.Uid) == target && target.Energy.Contains(energy))
                foreach (var src in Sources())
                {
                    if (src.Ctx.Player != target.Owner || State.IsOver || State.FindPokemon(target.Uid) != target || !target.Energy.Contains(energy)) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnOwnerAttachedEnergyCard(src.Ctx, energy, target); }
                    finally { PopSource(); }
                }
            // v141: timed effects that react to any Energy attachment (Crobat's Triggered Poison).
            if (!State.InSetup && !State.IsOver)
                foreach (var e in ActiveEffects().ToList())
                {
                    if (!State.Effects.Contains(e) || State.IsOver || State.FindPokemon(target.Uid) != target) continue;
                    PushSource(EffectSourceKind.Attack, e.Owner, null);
                    try { e.OnEnergyAttached(this, energy, target); }
                    finally { PopSource(); }
                }
            return true;
        }

        /// <summary>
        /// Aurora Energy's cost (v66): the attaching player chooses another card from their hand and discards it. With no other
        /// card there, the Energy goes back to the hand and isn't attached. Returns true when the cost was paid.
        /// </summary>
        private bool DiscardToAttach(CardInstance energy, PokemonInPlay target)
        {
            var p = State.Players[energy.Owner];
            var others = p.Hand.Where(c => c != energy).ToList();
            if (others.Count == 0)
            {
                if (!p.Hand.Contains(energy)) p.Hand.Add(energy);
                Emit(GameEventType.Info, energy.Owner, energy.Def.Name + " needs another card from the hand to be discarded; it stays in the hand.", energy.Id, target.Uid);
                return false;
            }
            var paid = ChooseCards(energy.Owner, others, 1, 1, energy.Def.Name + ": discard another card from your hand to attach it to " + target.Name, energy.Def);
            if (paid.Count == 0) paid.Add(others[0]);
            var card = paid[0];
            p.Hand.Remove(card);
            p.Discard.Add(card);
            Emit(GameEventType.Discard, energy.Owner, PlayerName(energy.Owner) + " discarded " + card.Def.Name + " to attach " + energy.Def.Name + ".", card.Id);
            return true;
        }

        /// <summary>
        /// Goodra's Slimy Room (v55): while the other player's Goodra is Active, the player attaching an Energy card from their
        /// hand flips a coin; on tails the card goes to their discard pile instead. One flip per card, however it's attached
        /// (your ruling). Returns true when the card was discarded.
        /// </summary>
        private bool SlimyRoomDiscards(CardInstance energy, PokemonInPlay target)
        {
            int player = target.Owner;
            foreach (var src in Sources())
            {
                if (src.Ctx.Player == player || !src.Script.FlipsForEnergyFromHand(src.Ctx, player)) continue;
                string reason = src.Ctx.Pokemon != null && src.Ctx.Pokemon.Def.Abilities.Count > 0 ? src.Ctx.Pokemon.Def.Abilities[0].Name : "Slimy Room";
                if (FlipCoin(player, reason + " (" + energy.Def.Name + ")")) return false;
                State.Players[energy.Owner].Discard.Add(energy);
                Emit(GameEventType.Discard, player, energy.Def.Name + " was discarded instead of being attached to " + target.Name + " (" + reason + ").", energy.Id, target.Uid);
                return true;
            }
            return false;
        }

        public bool DiscardEnergy(PokemonInPlay from, CardInstance energy)
        {
            if (from == null || !from.Energy.Contains(energy)) return false;
            if (!FlipIfAttackAffects(from)) return false; // Lt. Surge's Secret Plan (v126): its Energy went with it
            if (CurrentEffectPrevented(from, false))
            {
                Emit(GameEventType.DamagePrevented, from.Owner, "Discarding Energy from " + from.Name + " was prevented.", from.Top.Id, from.Uid);
                return false;
            }
            if (EnergyDiscardProtected(from, energy))
            {
                Emit(GameEventType.DamagePrevented, from.Owner, energy.Def.Name + " on " + from.Name + " can't be discarded by that card.", energy.Id, from.Uid);
                return false;
            }
            bool specialWasOn = energy.Def.IsSpecialEnergy && !SpecialEnergyOff(from);
            from.Energy.Remove(energy);
            Emit(GameEventType.Discard, from.Owner, energy.Def.Name + " was discarded from " + from.Name + ".", energy.Id, from.Uid);
            DiscardFromPlay(energy);
            EcogymReturns(from, energy);
            // v136: basic Energy a player's Active Pokémon discards from itself with its own attack (Heatran LV.X's Heat Wave).
            if (CurrentSourceKind == EffectSourceKind.Attack && CurrentSourcePokemon == from && energy.Def.IsBasicEnergy && State.IsActive(from))
            {
                var owner = State.Players[from.Owner];
                if (owner.OwnAttackDiscardedTurn != State.TurnNumber || owner.OwnAttackDiscardedFromUid != from.Uid)
                {
                    owner.OwnAttackDiscardedEnergy.Clear();
                    owner.OwnAttackDiscardedTurn = State.TurnNumber;
                    owner.OwnAttackDiscardedFromUid = from.Uid;
                }
                owner.OwnAttackDiscardedEnergy.Add(energy);
            }
            if (CurrentSourceKind == EffectSourceKind.Attack && CurrentSourcePokemon == from && specialWasOn
                && Scripts.For(energy.Def).ReturnsAfterAttackDiscard(new EffectContext(this, from.Owner, energy, from)))
                returnAfterAttack.Add(new KeyValuePair<PokemonInPlay, CardInstance>(from, energy));
            return true;
        }

        /// <summary>
        /// Ecogym (v138): an Energy card another player's attack, Pokémon Power (or Ability) or Trainer card discarded from a
        /// Pokémon goes back to its owner's hand (only basic Energy: your ruling). Knock Outs don't discard Energy this way.
        /// </summary>
        private void EcogymReturns(PokemonInPlay from, CardInstance energy)
        {
            if (State.Stadium == null || StadiumSwitchedOff() || CurrentSourceOwner < 0 || CurrentSourceOwner == energy.Owner) return;
            var kind = CurrentSourceKind;
            if (kind != EffectSourceKind.Attack && kind != EffectSourceKind.Ability && kind != EffectSourceKind.PokemonPower
                && kind != EffectSourceKind.Trainer && kind != EffectSourceKind.Stadium) return;
            var owner = State.Players[energy.Owner];
            if (!owner.Discard.Contains(energy) || !Scripts.For(State.Stadium.Def).ReturnsDiscardedEnergy(StadiumContext(), from, energy)) return;
            owner.Discard.Remove(energy);
            owner.Hand.Add(energy);
            Emit(GameEventType.Info, energy.Owner, energy.Def.Name + " went back to " + Their(energy.Owner) + " hand (" + State.Stadium.Def.Name + ").", energy.Id);
        }

        /// <summary>Discards every Energy attached to the Pokémon. Returns how many cards were discarded.</summary>
        public int DiscardAllEnergy(PokemonInPlay mon)
        {
            int n = 0;
            foreach (var e in mon.Energy.ToList()) if (DiscardEnergy(mon, e)) n++;
            return n;
        }

        public bool MoveEnergy(PokemonInPlay from, PokemonInPlay to, CardInstance energy)
        {
            if (from == null || to == null || from == to || !from.Energy.Contains(energy)) return false;
            if (!FlipIfAttackAffects(from) || !FlipIfAttackAffects(to)) return false; // Lt. Surge's Secret Plan (v126)
            if (CurrentEffectPrevented(from, false) || CurrentEffectPrevented(to, false))
            {
                Emit(GameEventType.DamagePrevented, to.Owner, "Moving Energy was prevented.", to.Top.Id, to.Uid);
                return false;
            }
            from.Energy.Remove(energy);
            to.Energy.Add(energy);
            Emit(GameEventType.AttachEnergy, to.Owner, energy.Def.Name + " moved from " + from.Name + " to " + to.Name + ".", energy.Id, to.Uid);
            return true;
        }

        /// <summary>Puts an attached Energy into its owner's hand.</summary>
        public bool ReturnEnergyToHand(PokemonInPlay from, CardInstance energy)
        {
            if (from == null || !from.Energy.Contains(energy)) return false;
            if (!FlipIfAttackAffects(from)) return false; // Lt. Surge's Secret Plan (v126)
            if (CurrentEffectPrevented(from, false) || !CanPutIntoHand(from)) return false;
            if (EnergyRemovalProtected(from, energy))
            {
                Emit(GameEventType.DamagePrevented, from.Owner, energy.Def.Name + " on " + from.Name + " can't be put into the hand by that card.", energy.Id, from.Uid);
                return false;
            }
            from.Energy.Remove(energy);
            State.Players[energy.Owner].Hand.Add(energy);
            Emit(GameEventType.Info, from.Owner, energy.Def.Name + " was put into " + PlayerName(energy.Owner) + "'s hand from " + from.Name + ".", energy.Id, from.Uid);
            return true;
        }

        /// <summary>Shuffles attached Energy into its owner's deck.</summary>
        public int ShuffleEnergyIntoDeck(PokemonInPlay from, IList<CardInstance> energy)
        {
            int n = 0;
            foreach (var e in energy.ToList())
            {
                if (!from.Energy.Contains(e)) continue;
                if (EnergyRemovalProtected(from, e))
                {
                    Emit(GameEventType.DamagePrevented, from.Owner, e.Def.Name + " on " + from.Name + " can't be put into the deck by that card.", e.Id, from.Uid);
                    continue;
                }
                from.Energy.Remove(e);
                State.Players[e.Owner].Deck.Add(e);
                n++;
            }
            if (n > 0)
            {
                State.Rng.Shuffle(State.Players[from.Owner].Deck);
                Emit(GameEventType.Shuffle, from.Owner, n + " Energy from " + from.Name + " was shuffled into " + PlayerName(from.Owner) + "'s deck.", -1, from.Uid);
            }
            return n;
        }

        /// <summary>
        /// An attached Tool or Energy card leaving play for the discard pile: into its owner's hand instead when its card says so
        /// (U-Turn Board, Recycle Energy; v72).
        /// </summary>
        public void DiscardFromPlay(CardInstance card)
        {
            if (card == null) return;
            var owner = State.Players[card.Owner];
            if (Scripts.For(card.Def).GoesToHandWhenDiscardedFromPlay)
            {
                owner.Hand.Add(card);
                Emit(GameEventType.Info, card.Owner, card.Def.Name + " went into " + PlayerName(card.Owner) + "'s hand instead of the discard pile.", card.Id);
            }
            else owner.Discard.Add(card);
        }

        public void DiscardFromPlay(IEnumerable<CardInstance> cards)
        {
            foreach (var card in cards.ToList()) DiscardFromPlay(card);
        }

        public void AttachTool(CardInstance tool, PokemonInPlay holder)
        {
            if (holder.Tools.Count == 0) holder.ToolSeq = NextLockSeq();
            holder.Tools.Add(tool);
            Emit(GameEventType.AttachTool, holder.Owner, PlayerName(holder.Owner) + " attached " + tool.Def.Name + " to " + holder.Name + ".", tool.Id, holder.Uid);
        }

        /// <summary>
        /// Discards a Pokémon Tool from this Pokémon. When it has more than one (Rotom ex's Multi Adapter), the player
        /// whose effect this is picks which; for a rule, the Pokémon's owner picks.
        /// </summary>
        public bool DiscardTool(PokemonInPlay mon)
        {
            if (mon == null || mon.Tools.Count == 0) return false;
            var tool = mon.Tools[0];
            if (mon.Tools.Count > 1)
            {
                int chooser = CurrentSourceOwner >= 0 ? CurrentSourceOwner : mon.Owner;
                var picks = ChooseCards(chooser, mon.Tools.ToList(), 1, 1, "Choose a Pokémon Tool to discard from " + mon.Name, null);
                if (picks.Count > 0) tool = picks[0];
            }
            return DiscardTool(mon, tool);
        }

        /// <summary>Discards every Pokémon Tool attached to this Pokémon. Returns how many were discarded.</summary>
        public int DiscardAllTools(PokemonInPlay mon)
        {
            if (mon == null) return 0;
            int n = 0;
            foreach (var tool in mon.Tools.ToList())
                if (DiscardTool(mon, tool)) n++;
            return n;
        }

        /// <summary>The Pokémon in play this Tool card is attached to, or null.</summary>
        public PokemonInPlay ToolHolder(CardInstance tool)
        {
            for (int i = 0; i < 2; i++)
                foreach (var mon in State.Players[i].InPlay())
                    if (mon.HasTool(tool)) return mon;
            return null;
        }

        /// <summary>Discards this particular Tool card, if it's attached to this Pokémon.</summary>
        public bool DiscardTool(PokemonInPlay mon, CardInstance tool)
        {
            if (mon == null || tool == null || !mon.Tools.Remove(tool)) return false;
            Emit(GameEventType.Discard, mon.Owner, tool.Def.Name + " was discarded from " + mon.Name + ".", tool.Id, mon.Uid);
            DiscardFromPlay(tool);
            return true;
        }

        // ------------------------------------------------------------------ Switching

        /// <summary>
        /// Swaps the player's Active Pokémon with one of their Benched Pokémon. The old Active recovers from
        /// Special Conditions. Returns false if an effect prevented it (an opponent's attack or Ability against Mist Energy, Hide 'n' Sneak...).
        /// </summary>
        /// <summary>
        /// v138: a timed effect (Jaw Clamp, Sticky Nectar) stops the effect of an attack, Pokémon Power or Trainer card from
        /// changing this player's Active Pokémon. Retreating and replacing a Knocked Out Pokémon aren't such effects.
        /// </summary>
        public bool ActiveSpotLocked(int player)
        {
            var kind = CurrentSourceKind;
            if (kind == EffectSourceKind.None || kind == EffectSourceKind.Rule) return false;
            foreach (var e in ActiveEffects())
                if (e.LocksActiveSpot(this, player)) return true;
            return false;
        }

        public bool SwitchActive(int player, PokemonInPlay benched)
        {
            var p = State.Players[player];
            // Lt. Surge's Secret Plan (v126): an attack that moves a face-down card flips it over first.
            if (IsFaceDown(benched) && p.Bench.Contains(benched) && !FlipIfAttackAffects(benched)) return false;
            if (IsFaceDown(p.Active)) FlipIfAttackAffects(p.Active); // discarded: the benched Pokémon moves into the empty spot
            int idx = p.Bench.IndexOf(benched);
            if (idx < 0) throw new InvalidOperationException(benched.Name + " is not on the Bench.");
            // Jaw Clamp, Sticky Nectar (v138): an effect can't change this player's Active Pokémon (retreating isn't an effect).
            if (p.Active != null && ActiveSpotLocked(player))
            {
                Emit(GameEventType.DamagePrevented, player, OwnedName(p.Active) + " can't be switched out.", p.Active.Top.Id, p.Active.Uid);
                return false;
            }
            // Ω Barrier on the Active Pokémon doesn't stop the opponent's Lysandre or Escape Rope: only a Pokémon the card targets
            // is protected (2015).
            if (CurrentEffectPrevented(benched, false) || (p.Active != null && !PowerGustInProgress && CurrentEffectPrevented(p.Active, false) && !OnlyOmegaBarrierStops(p.Active)))
            {
                Emit(GameEventType.DamagePrevented, player, "The switch was prevented.", benched.Top.Id, benched.Uid);
                return false;
            }
            var old = p.Active;
            // Dust Island (v73): a Poisoned Active Pokémon switched out by its owner's Trainer card passes its Poison on, a stronger
            // Poison included (Compendium, 2019-05-02 and 2020-09-24: Severe Poison's and Poison Cultivation's extra counters follow).
            bool poisonFollows = old != null && old.Has(SpecialCondition.Poisoned) && CurrentSourceKind == EffectSourceKind.Trainer && CurrentSourceOwner == player
                                 && State.Stadium != null && !StadiumSwitchedOff() && Scripts.For(State.Stadium.Def).PoisonFollowsTrainerSwitch(StadiumContext());
            int followingPoison = old != null ? old.PoisonCounterBase : 1;
            if (old == null)
            {
                p.Bench.RemoveAt(idx);
            }
            else
            {
                p.Bench[idx] = old;
                old.Conditions = SpecialCondition.None;
                ClearEffectsOn(old);
            }
            p.Active = benched;
            benched.MovedToActiveOnTurn = State.TurnNumber;
            benched.SpotSeq = NextLockSeq();
            if (old != null) old.SpotSeq = benched.SpotSeq;
            Emit(GameEventType.Switch, player, OwnedName(benched) + " is now Active.", benched.Top.Id, benched.Uid);
            if (poisonFollows)
            {
                Emit(GameEventType.Info, player, State.Stadium.Def.Name + ": the new Active Pokémon is Poisoned.", benched.Top.Id, benched.Uid);
                PushSource(EffectSourceKind.Stadium, State.StadiumOwner, null);
                try
                {
                    if (ApplyCondition(benched, SpecialCondition.Poisoned)) benched.PoisonCounterBase = Math.Max(benched.PoisonCounterBase, followingPoison);
                }
                finally { PopSource(); }
            }
            // v141: "If this Pokémon is your Active Pokémon and moves to the Bench" at any time (Magcargo's Magma Pool).
            if (old != null && !State.InSetup && !State.IsOver && p.Bench.Contains(old) && p.Active == benched && HasWorkingAbilityOrPower(old))
            {
                PushSource(old.Def.AbilitySourceKind, player, old);
                try { Scripts.For(old.Def).OnLeftActiveSpot(new EffectContext(this, player, old.Top, old), benched); }
                finally { PopSource(); }
            }
            // "When this Pokémon moves from the Active Spot to the Bench" during its owner's turn (Palafin's Zero to Hero).
            if (old != null && !State.InSetup && State.CurrentPlayer == player && HasWorkingAbilityOrPower(old))
            {
                PushSource(old.Def.AbilitySourceKind, player, old);
                try { Scripts.For(old.Def).OnMovedToBenchFromActive(new EffectContext(this, player, old.Top, old)); }
                finally { PopSource(); }
            }
            // "Whenever your opponent's Active Pokémon moves to the Bench during their turn" (Magcargo's Lava Zone).
            if (old != null && !State.InSetup && State.CurrentPlayer == player)
            {
                foreach (var src in Sources())
                {
                    if (src.Ctx.Player == player || p.Active != benched) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnOpponentActiveMovedToBench(src.Ctx, player, benched); }
                    finally { PopSource(); }
                }
                foreach (var src in Sources())
                {
                    if (src.Ctx.Player == player || !p.Bench.Contains(old)) continue;
                    PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                    try { src.Script.OnOpponentPokemonMovedToBench(src.Ctx, player, old); }
                    finally { PopSource(); }
                }
            }
            // Spikemuth (v64): "Whenever a player's Active Pokémon moves to the Bench during their turn".
            if (old != null && !State.InSetup && !State.IsOver && State.CurrentPlayer == player && State.Stadium != null && !StadiumSwitchedOff()
                && p.Bench.Contains(old) && !StadiumShielded(old))
            {
                PushSource(EffectSourceKind.Stadium, State.StadiumOwner, null);
                try { Scripts.For(State.Stadium.Def).OnActiveMovedToBenchUnderStadium(StadiumContext(), player, old); }
                finally { PopSource(); }
            }
            MovedToActiveFromBench(player, benched);
            return true;
        }

        /// <summary>"When this Pokémon moves from your Bench to the Active Spot" during its owner's turn (Yanmega ex's Buzzing Boost).</summary>
        private void MovedToActiveFromBench(int player, PokemonInPlay mon)
        {
            if (State.InSetup || State.CurrentPlayer != player || State.Players[player].Active != mon) return;
            if (HasWorkingAbilityOrPower(mon))
            {
                PushSource(mon.Def.AbilitySourceKind, player, mon);
                try { Scripts.For(mon.Def).OnMovedToActiveFromBench(new EffectContext(this, player, mon.Top, mon)); }
                finally { PopSource(); }
            }
            // "When your X moves from your Bench to the Active Spot" on another of the player's cards (Latios's Lustrous Assist).
            foreach (var src in Sources())
            {
                if (src.Ctx.Player != player || src.Ctx.Pokemon == mon || State.Players[player].Active != mon) continue;
                PushSource(src.Kind, player, src.Ctx.Pokemon);
                try { src.Script.OnOwnPokemonMovedToActive(src.Ctx, mon); }
                finally { PopSource(); }
            }
            // v139: "During your opponent's turn, whenever 1 of your opponent's Benched Pokémon becomes their Active Pokémon"
            // (Forretress's Spikes): switches and promotions alike.
            foreach (var src in Sources())
            {
                if (src.Ctx.Player == player || State.IsOver || State.Players[player].Active != mon) continue;
                PushSource(src.Kind, src.Ctx.Player, src.Ctx.Pokemon);
                try { src.Script.OnOpponentBenchedBecameActive(src.Ctx, player, mon); }
                finally { PopSource(); }
            }
        }

        // ------------------------------------------------------------------ Special Conditions and healing

        /// <summary>Special Conditions only affect the Active Pokémon. Asleep, Confused and Paralyzed replace each other.</summary>
        public bool ApplyCondition(PokemonInPlay target, SpecialCondition condition)
        {
            if (target == null || !State.IsActive(target)) return false;
            if (IsFaceDown(target) && !FlipFaceDown(target)) return false; // Lt. Surge's Secret Plan (v126)
            if ((target.Def.IsFossil && !target.Def.FossilPlain) || SpecialConditionsPrevented(target) || ConditionPrevented(target, condition) || CurrentEffectPrevented(target, false))
            {
                Emit(GameEventType.DamagePrevented, target.Owner, target.Name + " can't be " + condition + ".", target.Top.Id, target.Uid);
                return false;
            }
            const SpecialCondition rotation = SpecialCondition.Asleep | SpecialCondition.Confused | SpecialCondition.Paralyzed;
            if ((condition & rotation) != 0) target.Conditions &= ~rotation;
            target.Conditions |= condition;
            if ((condition & SpecialCondition.Paralyzed) != 0) target.ParalyzedOnTurn = State.TurnNumber;
            if ((condition & SpecialCondition.Confused) != 0) target.ConfusionCounters = 3; // a new Confusion is the normal kind
            if ((condition & SpecialCondition.Poisoned) != 0) target.PoisonCounterBase = 1; // and so is a new Poison
            if ((condition & SpecialCondition.Asleep) != 0)
            {
                target.SleepCoins = 1; // and a new Asleep
                target.SleepKnocksOut = false;
            }
            // Mirror Move (v113): remember the Special Conditions the opponent's attack gave this Pokémon this turn.
            if (CurrentSourceKind == EffectSourceKind.Attack && CurrentSourceOwner >= 0 && CurrentSourceOwner != target.Owner)
            {
                int turn;
                if (!target.Markers.TryGetValue(PokemonInPlay.AttackConditionsTurnMarker, out turn) || turn != State.TurnNumber)
                    target.Markers[PokemonInPlay.AttackConditionsMarker] = 0;
                target.Markers[PokemonInPlay.AttackConditionsTurnMarker] = State.TurnNumber;
                target.Markers[PokemonInPlay.AttackConditionsMarker] |= (int)condition;
            }
            Emit(GameEventType.ConditionApplied, target.Owner, target.Name + " is now " + condition + ".", target.Top.Id, target.Uid);
            // v147: the Pokémon's own reaction (Skyridge Wobbuffet's Mirror Coat).
            if (target.Def.Abilities.Count > 0 && Overrides(Scripts.For(target.Def), "OnHolderConditionApplied") && HasWorkingAbilityOrPower(target))
            {
                var kind = CurrentSourceKind;
                int owner = CurrentSourceOwner;
                var sourceMon = CurrentSourcePokemon;
                PushSource(target.Def.AbilitySourceKind, target.Owner, target);
                try { Scripts.For(target.Def).OnHolderConditionApplied(new EffectContext(this, target.Owner, target.Top, target), condition, kind, owner, sourceMon); }
                finally { PopSource(); }
            }
            return true;
        }

        public void RemoveCondition(PokemonInPlay target, SpecialCondition condition)
        {
            // Trapping Thread (v81): the opponent's Item or Supporter card can't cure their Defending Pokémon.
            if (State.IsActive(target) && TrainerEffectStoppedOn(target)) return;
            target.Conditions &= ~condition;
            Emit(GameEventType.ConditionRemoved, target.Owner, target.Name + " is no longer " + condition + ".", target.Top.Id, target.Uid);
        }

        public void RemoveAllConditions(PokemonInPlay target)
        {
            if (target.Conditions == SpecialCondition.None) return;
            if (State.IsActive(target) && TrainerEffectStoppedOn(target)) return;
            target.Conditions = SpecialCondition.None;
            Emit(GameEventType.ConditionRemoved, target.Owner, target.Name + " recovered from all Special Conditions.", target.Top.Id, target.Uid);
        }

        /// <summary>Heals damage (in multiples of 10). Returns the damage healed.</summary>
        public int Heal(PokemonInPlay target, int amount)
        {
            if (target == null || amount <= 0 || target.DamageCounters == 0) return 0;
            if (HealingBlocked(target) || (CurrentSourceKind == EffectSourceKind.Stadium && StadiumShielded(target)) || TrainerEffectStoppedOn(target))
            {
                Emit(GameEventType.Info, target.Owner, target.Name + " can't be healed.", target.Top.Id, target.Uid);
                return 0;
            }
            // α Recovery (v101): the Pokémon is healed twice the amount.
            if (HasAncientTrait(target, AlphaRecovery)) amount *= 2;
            // Legendary Abyss (v152.6): each effect that changes the amount applies in turn (with α Recovery: ×4, your ruling).
            foreach (var src in Sources()) amount = src.Script.ModifyHealAmount(src.Ctx, target, amount);
            int counters = Math.Min(target.DamageCounters, amount / 10);
            target.DamageCounters -= counters;
            if (counters > 0) target.HealedOnTurn = State.TurnNumber;
            Emit(GameEventType.Info, target.Owner, "Healed " + counters * 10 + " damage from " + target.Name + ".", target.Top.Id, target.Uid, counters * 10);
            return counters * 10;
        }

        // ------------------------------------------------------------------ Leaving play

        public void RemoveFromPlay(PokemonInPlay mon)
        {
            // v126: a face-down card (Lt. Surge's Secret Plan) leaves play as the real card, and Evolution cards attached by
            // Shapeshift go wherever its other attached cards go.
            RestoreFaceDown(mon);
            if (mon.Attached.Count > 0)
            {
                mon.Tools.AddRange(mon.Attached);
                mon.Attached.Clear();
            }
            ClearEffectsOn(mon);
            var p = State.Players[mon.Owner];
            if (p.Active == mon) p.Active = null;
            else p.Bench.Remove(mon);
        }

        /// <summary>Discards a Pokémon and everything attached (not a Knock Out: no Prizes).</summary>
        public void DiscardPokemonFromPlay(PokemonInPlay mon)
        {
            RemoveFromPlay(mon);
            State.Players[mon.Owner].Discard.AddRange(mon.Stack);
            Emit(GameEventType.Discard, mon.Owner, mon.Name + " and all attached cards were discarded.", mon.Top.Id, mon.Uid);
            DiscardFromPlay(mon.Energy);
            DiscardFromPlay(mon.Tools);
        }

        /// <summary>
        /// "Discard all cards attached to this Pokémon and put it on the bottom of your deck" (Lillie's Poké Doll, Phione's
        /// Whirlpool Suction; v68): its Energy and Tools are discarded, its Pokémon cards go to the bottom of the deck.
        /// </summary>
        public void PutPokemonOnDeckBottom(PokemonInPlay mon)
        {
            RemoveFromPlay(mon);
            var p = State.Players[mon.Owner];
            DiscardFromPlay(mon.Energy);
            DiscardFromPlay(mon.Tools);
            p.Deck.AddRange(mon.Stack);
            Emit(GameEventType.Info, mon.Owner, mon.Name + " was put on the bottom of " + Their(mon.Owner) + " deck" + (mon.Energy.Count + mon.Tools.Count > 0 ? " and its attached cards were discarded." : "."), mon.Top.Id, mon.Uid);
        }

        public void ReturnPokemonToHand(PokemonInPlay mon)
        {
            if (TrainerEffectStoppedOn(mon))
            {
                Emit(GameEventType.DamagePrevented, mon.Owner, "The effect on " + mon.Name + " was prevented.", mon.Top.Id, mon.Uid);
                return;
            }
            if (!CanPutIntoHand(mon))
            {
                Emit(GameEventType.Info, mon.Owner, mon.Name + " can't be put into " + Their(mon.Owner) + " hand.", mon.Top.Id, mon.Uid);
                return;
            }
            RemoveFromPlay(mon);
            State.Players[mon.Owner].Hand.AddRange(mon.AllCards());
            Emit(GameEventType.Info, mon.Owner, mon.Name + " and all attached cards were put into " + PlayerName(mon.Owner) + "'s hand.", mon.Top.Id, mon.Uid);
        }

        public void ShufflePokemonIntoDeck(PokemonInPlay mon)
        {
            RemoveFromPlay(mon);
            var p = State.Players[mon.Owner];
            p.Deck.AddRange(mon.AllCards());
            State.Rng.Shuffle(p.Deck);
            Emit(GameEventType.Shuffle, mon.Owner, mon.Name + " and all attached cards were shuffled into " + PlayerName(mon.Owner) + "'s deck.", mon.Top.Id, mon.Uid);
        }

        // ------------------------------------------------------------------ Stadiums

        public void PutStadiumIntoPlay(int player, CardInstance card)
        {
            if (State.Stadium != null && !StadiumSwitchedOff() && Scripts.For(State.Stadium.Def).DiscardsNewStadiums(StadiumContext()))
            {
                string swell = State.Stadium.Def.Name;
                DiscardStadium();
                State.Players[card.Owner].Discard.Add(card);
                Emit(GameEventType.Discard, player, card.Def.Name + " was discarded right after " + swell + " (it has no effect).", card.Id);
                return;
            }
            // The Stadiums swap at once (Sky Field / Area Zero Underdepths: 2024-09-26): the old one's "when this card leaves
            // play" effect runs after the new one is in play, so a Bench the new Stadium allows isn't cut.
            CardInstance replaced = null;
            int replacedOwner = -1;
            if (State.Stadium != null)
            {
                deferStadiumLeave = true;
                try { DiscardStadium(); }
                finally { deferStadiumLeave = false; }
                replaced = deferredLeaver;
                replacedOwner = deferredLeaverOwner;
                deferredLeaver = null;
            }
            State.Stadium = card;
            State.StadiumPartner = null; // a Legendary Stadium's other half is added by the caller (PlayTrainer)
            State.StadiumOwner = player;
            Emit(GameEventType.StadiumPlayed, player, card.Def.Name + " is now in play.", card.Id);
            PushSource(EffectSourceKind.Trainer, player, null);
            try { Scripts.For(card.Def).OnStadiumPlayed(new EffectContext(this, player, card, null)); }
            finally { PopSource(); }
            if (replaced != null) Scripts.For(replaced.Def).OnLeavePlay(new EffectContext(this, replacedOwner, replaced, null));
        }

        /// <summary>v152: takes the other half of the Legendary Stadium in play out of it (null for a normal Stadium); the caller puts it where the Stadium goes.</summary>
        public CardInstance TakeStadiumPartner()
        {
            var partner = State.StadiumPartner;
            State.StadiumPartner = null;
            return partner;
        }

        private bool deferStadiumLeave;
        private CardInstance deferredLeaver;
        private int deferredLeaverOwner;

        /// <summary>Discards the Stadium in play (to its owner's discard pile) and runs its "when this card leaves play" effect.</summary>
        public void DiscardStadium()
        {
            var old = State.Stadium;
            if (old == null) return;
            if (StadiumIgnoresCurrentTrainer())
            {
                Emit(GameEventType.Info, State.StadiumOwner, old.Def.Name + " isn't affected by " + ResolvingTrainer.Name + ".", old.Id);
                return;
            }
            int owner = State.StadiumOwner;
            var partner = TakeStadiumPartner();
            State.Stadium = null;
            State.StadiumOwner = -1;
            State.Players[owner].Discard.Add(old);
            if (partner != null) State.Players[owner].Discard.Add(partner); // both halves of a Legendary Stadium (v152)
            Emit(GameEventType.Discard, owner, old.Def.Name + " was discarded.", old.Id);
            if (deferStadiumLeave)
            {
                deferredLeaver = old;
                deferredLeaverOwner = owner;
                return;
            }
            Scripts.For(old.Def).OnLeavePlay(new EffectContext(this, owner, old, null));
        }

        // ------------------------------------------------------------------ Timed effects

        /// <summary>Adds a timed effect. Effects on an opponent's Pokémon can be prevented (Mist Energy, Hide 'n' Sneak...).</summary>
        public bool AddEffect(ContinuousEffect effect)
        {
            if (effect.TargetUid >= 0)
            {
                var target = State.FindPokemon(effect.TargetUid);
                // Lt. Surge's Secret Plan (v126): an attack's effect on a face-down card flips it over first.
                if (target != null && target.Owner != effect.Owner && !FlipIfAttackAffects(target)) return false;
                if (target != null && CurrentEffectPrevented(target, false))
                {
                    Emit(GameEventType.DamagePrevented, target.Owner, "The effect on " + target.Name + " was prevented.", target.Top.Id, target.Uid);
                    return false;
                }
            }
            if (CurrentSourceKind == EffectSourceKind.Attack) effect.FromAttack = true;
            State.Effects.Add(effect);
            Emit(GameEventType.Info, effect.Owner, effect.Name + ".", -1, effect.TargetUid);
            return true;
        }

        /// <summary>Ends attack effects on a Pokémon that moved to the Bench, evolved or left play.</summary>
        public void ClearEffectsOn(PokemonInPlay mon)
        {
            State.Effects.RemoveAll(e => e.EndsWhenTargetMoves && e.TargetUid == mon.Uid);
        }

        // ------------------------------------------------------------------ Names

        public string Describe(PokemonInPlay mon)
        {
            var p = State.Players[mon.Owner];
            if (p.Active == mon) return mon.Name + " (Active)";
            int idx = p.Bench.IndexOf(mon);
            return mon.Name + " (Bench " + (idx + 1) + ")";
        }

        public string DescribeFor(int viewer, PokemonInPlay mon)
        {
            return mon.Owner == viewer ? Describe(mon) : "Opponent's " + Describe(mon);
        }
    }
}
