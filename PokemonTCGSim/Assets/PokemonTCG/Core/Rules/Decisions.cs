using System.Collections.Generic;

namespace PokemonTCG.Core
{
    public enum DecisionKind
    {
        /// <summary>Pick the next thing to do this turn (play a card, attack, end turn...).</summary>
        MainAction,
        GoFirstOrSecond,
        SetupActive,
        SetupBench,
        MulliganDraws,
        PromoteActive,
        ChooseCards,
        ChoosePokemon,
        ChooseNumber,
        YesNo,
        ChooseOption,
        /// <summary>Call the opening coin flip (heads or tails). Only the person on screen is asked.</summary>
        CallCoinFlip,
        /// <summary>
        /// Take 1 Prize card: one option per Prize card, by its place on the table (Index = slot, 0-5). Face-down cards carry
        /// no card; a face-up one (Crescent Purge) has CardId and Card. Only the person on screen is asked for face-down
        /// cards; it isn't asked when the Prize cards taken are the last ones (v42).
        /// </summary>
        ChoosePrize
    }

    public enum ActionType
    {
        None,
        BenchBasic,
        Evolve,
        AttachEnergy,
        PlayTrainer,
        UseAbility,
        /// <summary>A Stadium's "once during each player's turn" action (Prism Tower, Lumiose City...).</summary>
        UseStadium,
        Retreat,
        Attack,
        EndTurn,
        /// <summary>Antique Fossils: "At any time during your turn, you may discard this card from play."</summary>
        DiscardFossil,
        /// <summary>An Ability used while the card is in your hand (Klinklang's Emergency Rotation).</summary>
        UseHandAbility,
        /// <summary>A card's effect used while it's in your discard pile (Grant: discard 2 cards to put it into your hand, v49).</summary>
        UseFromDiscard,
        /// <summary>V-UNION (v67): once per game, combine 4 different pieces from your discard pile onto your Bench as 1 Pokémon.</summary>
        CombineVUnion,
        /// <summary>v126, Lt. Surge's Secret Plan: "At any time during your turn, you may flip the card over."</summary>
        FlipFaceDown
    }

    /// <summary>One selectable choice. Agents (AI or the human UI) only ever pick from these.</summary>
    public sealed class DecisionOption
    {
        public string Label;
        public ActionType Action = ActionType.None;
        /// <summary>Card involved (a card in hand, deck, discard...), or -1.</summary>
        public int CardId = -1;
        /// <summary>Pokémon in play involved (a target), or -1.</summary>
        public int PokemonUid = -1;
        /// <summary>Attack / ability index, or a number for ChooseNumber.</summary>
        public int Index = -1;
        /// <summary>The printed card for CardId when known to the deciding player.</summary>
        public CardDef Card;
        /// <summary>
        /// Main actions: identical cards in the hand are listed once. These are the other copies the option also stands
        /// for, so the UI can let you drag any of them (it puts the copy you dragged into CardId before answering).
        /// </summary>
        public List<int> SameCardIds;

        /// <summary>True if this option is about that card (CardId, or another identical copy in the hand).</summary>
        public bool UsesCard(int cardId)
        {
            return CardId == cardId || (SameCardIds != null && SameCardIds.Contains(cardId));
        }

        /// <summary>Makes the option play that copy of the card (one of SameCardIds) instead of the first one listed.</summary>
        public void UseCopy(int cardId)
        {
            if (CardId == cardId || SameCardIds == null || !SameCardIds.Contains(cardId)) return;
            SameCardIds.Remove(cardId);
            SameCardIds.Add(CardId);
            CardId = cardId;
        }

        public override string ToString()
        {
            return Label;
        }
    }

    public sealed class DecisionRequest
    {
        public int Player;
        public DecisionKind Kind;
        public string Prompt;
        public List<DecisionOption> Options = new List<DecisionOption>();
        public int MinChoices = 1;
        public int MaxChoices = 1;
        /// <summary>The card whose effect is asking (null for main actions and rules decisions).</summary>
        public CardDef Source;
        /// <summary>What the deciding player is allowed to see right now.</summary>
        public PlayerView View;
        /// <summary>
        /// Every card the effect lets the player look at, valid or not ("All" tab): the top 7 cards for Bug Catching Set,
        /// the whole deck for a search, the discard pile, the hand. Null when it's just the options.
        /// </summary>
        public List<CardRef> SeenCards;
        /// <summary>What SeenCards is, e.g. "Top 7 cards of your deck".</summary>
        public string SeenTitle;

        public bool IsSingleChoice { get { return MinChoices == 1 && MaxChoices == 1; } }
    }

    /// <summary>Anything that can play a seat: the human UI, a heuristic bot, a search AI.</summary>
    public interface IPlayerAgent
    {
        /// <summary>Return the chosen option indices (between MinChoices and MaxChoices of them, no duplicates).</summary>
        List<int> Decide(DecisionRequest request);
    }

    /// <summary>
    /// A seat a person plays on screen. The engine asks it every card choice, even one with a single possible answer
    /// (it never picks cards for it), and still shows it the deck after a search that finds nothing.
    /// </summary>
    public interface IInteractiveAgent : IPlayerAgent
    {
    }

    public enum GameEventType
    {
        Info,
        GameStart,
        CoinFlip,
        Mulligan,
        SetupDone,
        TurnStart,
        TurnEnd,
        Draw,
        PutOnBench,
        Evolve,
        AttachEnergy,
        AttachTool,
        PlayTrainer,
        UseAbility,
        Retreat,
        Switch,
        Attack,
        Damage,
        DamageCounters,
        DamagePrevented,
        ConditionApplied,
        ConditionRemoved,
        KnockOut,
        TakePrize,
        Promote,
        Discard,
        Search,
        Shuffle,
        StadiumPlayed,
        GameOver,
        /// <summary>A player's mulligan hands, shown once their opening hand is final (see SideView.MulliganHands).</summary>
        MulliganReveal,
        /// <summary>Cards shown to both players (searched cards that are revealed, cards put into the hand from a look at the deck...). See GameEvent.Cards.</summary>
        Reveal
    }

    public sealed class GameEvent
    {
        public GameEventType Type;
        public int Player = -1;
        public string Text;
        public int CardId = -1;
        public int PokemonUid = -1;
        public int Amount;
        /// <summary>Cards revealed by this event, or null.</summary>
        public List<CardRef> Cards;
        /// <summary>The card CardId refers to, for events that show a card to both players (playing a card, attacking...). Null otherwise.</summary>
        public CardDef Card;

        public override string ToString()
        {
            return Text;
        }
    }

    public interface IGameObserver
    {
        /// <summary>Called on the engine's thread, while the engine is paused at this event.</summary>
        void OnEvent(GameEngine engine, GameEvent ev);
    }

    public sealed class GameResult
    {
        public int Winner;
        public string Reason;
        public int Turns;

        public GameResult(int winner, string reason, int turns)
        {
            Winner = winner;
            Reason = reason;
            Turns = turns;
        }
    }
}
