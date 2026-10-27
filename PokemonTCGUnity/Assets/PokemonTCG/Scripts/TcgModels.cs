using System;

// Mirrors the JSON sent by PokemonTCGBot/server.py. Optional values arrive as
// arrays of zero or one element because JsonUtility can't represent null objects.
namespace PokemonTCG
{
    [Serializable]
    public class CardData
    {
        public int id;
        public string name;
        public string kind;
        public string image;
        public string image_large;
        public string text;
    }

    [Serializable]
    public class MonData
    {
        public int uid;
        public CardData card;
        public int hp;
        public int damage;
        public string[] energy;
        public CardData[] tool;
        public string[] conditions;
    }

    [Serializable]
    public class SideData
    {
        public int prizes;
        public int deck;
        public int discard;
        public int hand_count;
        public CardData[] discard_top;
        public CardData[] hand;
        public MonData[] active;
        public MonData[] bench;
    }

    [Serializable]
    public class OptionData
    {
        public int index;
        public string kind;
        public string label;
        public CardData[] card;
        public string target;    // "you:0" (Active), "opp:2" (Bench 2)... or ""
        public string card_ref;  // "hand" when the option plays card[0] from your hand
    }

    [Serializable]
    public class DecisionData
    {
        public string player;
        public string kind;
        public string prompt;
        public CardData[] source;
        public OptionData[] options;
    }

    [Serializable]
    public class StateData
    {
        public string game;
        public int turn;
        public bool your_turn;
        public bool in_setup;
        public bool done;
        public bool watch;
        public string result;
        public SideData you;
        public SideData opp;
        public CardData[] stadium;
        public string stadium_owner;
        public string your_deck;
        public string bot_deck;
        public string bot;
        public string waiting_on;  // "you", "bot" or "" when the game is over
        public string[] log;
        public DecisionData[] decision;
        public string error;
    }

    [Serializable]
    public class InfoData
    {
        public string[] decks;
        public string[] bots;
    }

    [Serializable]
    public class NewGameRequest
    {
        public string deck;
        public string bot_deck;
        public string bot;
        public bool watch;
        public int seed = -1;
    }

    [Serializable]
    public class GameRequest
    {
        public string game;
        public int option;
    }
}
