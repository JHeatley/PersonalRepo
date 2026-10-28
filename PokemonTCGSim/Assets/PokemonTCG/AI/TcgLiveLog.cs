using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PokemonTCG.AI
{
    /// <summary>One line of a battle log, with the "- ..." lines under it and its "• a, b, c" card list.</summary>
    public sealed class LogLine
    {
        public string Text = "";
        /// <summary>Cards listed under the line ("   • Bulbasaur, Basic Grass Energy").</summary>
        public readonly List<string> Cards = new List<string>();
        /// <summary>The "- ..." lines that belong to this line (what a card's effect did).</summary>
        public readonly List<LogLine> Subs = new List<LogLine>();
        /// <summary>Line number in the text (1 = first line).</summary>
        public int Number;
    }

    /// <summary>One turn of a battle log ("TFR89's Turn"), or the setup before the first turn (Number 0).</summary>
    public sealed class LogTurn
    {
        public string Player = "";
        /// <summary>Turn number in its game (1 = the first player's first turn); 0 for a setup.</summary>
        public int Number;
        /// <summary>0 for the game, 1 and up for each Sudden Death game after it.</summary>
        public int Game;
        /// <summary>The setup of a Sudden Death game ("Entering Sudden Death."): everything starts again with 1 Prize card each.</summary>
        public bool SuddenDeath;
        public readonly List<LogLine> Lines = new List<LogLine>();
    }

    /// <summary>
    /// A Pokémon TCG Live battle log (the text TCG Live exports and Training Court shows), split into turns and lines (v76):
    ///
    ///   Setup
    ///   TFR89 drew 7 cards for the opening hand.
    ///   - 7 drawn cards.
    ///      • Lillie's Determination, Basic Grass Energy, Crustle, ...
    ///   TFR89's Turn
    ///   TFR89 played Ultra Ball.
    ///   - TFR89 discarded 2 cards.
    ///      • Bulbasaur, Basic Grass Energy
    ///   - TFR89 drew Teal Mask Ogerpon ex.
    ///
    /// The players are found from the setup lines and the turn headers; the winner from the last "... wins." line.
    /// </summary>
    public sealed class TcgLiveLog
    {
        public readonly List<string> Players = new List<string>();
        public string FirstPlayer;
        /// <summary>The winner's name, or null.</summary>
        public string Winner;
        /// <summary>The line the game ended with ("Opponent conceded. TFR89 wins.").</summary>
        public string Ending = "";
        public readonly List<LogTurn> Turns = new List<LogTurn>();
        public int LineCount;
        /// <summary>
        /// Written by the video reader (v77): a "# ... from a video" line at the top. Its turns are numbered ("X's Turn 9"),
        /// and Energy isn't shown, so the replay attaches an attack's cost when it has to.
        /// </summary>
        public bool FromVideo;

        /// <summary>Turns played, Sudden Death included.</summary>
        public int TurnCount
        {
            get
            {
                int n = 0;
                foreach (var t in Turns) if (t.Number > 0) n++;
                return n;
            }
        }

        private static readonly Regex TurnHeader = new Regex(@"^(.+)'s Turn(?: (\d+))?$");
        private static readonly Regex CoinChoice = new Regex(@"^(.+?) chose (heads|tails) for the opening coin flip\.?$");
        private static readonly Regex CoinWon = new Regex(@"^(.+?) won the coin toss\.?$");
        private static readonly Regex GoesFirst = new Regex(@"^(.+?) decided to go (first|second)\.?$");
        private static readonly Regex OpeningHand = new Regex(@"^(.+?) drew 7 cards for the opening hand\.?$");
        private static readonly Regex Wins = new Regex(@"^(?:.*\.\s+)?(.+?) wins\.?$");

        /// <summary>True when the text looks like a TCG Live battle log (turn headers or the opening hand lines).</summary>
        public static bool LooksLikeBattleLog(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Contains("'s Turn") || text.Contains("for the opening hand");
        }

        public static TcgLiveLog Parse(string text)
        {
            var log = new TcgLiveLog();
            var setup = new LogTurn { Number = 0 };
            log.Turns.Add(setup);
            var turn = setup;
            LogLine top = null, last = null;
            string goesSecond = null;
            int game = 0, gameTurns = 0;
            var lines = (text ?? "").Replace("\r", "").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string t = lines[i].Trim();
                if (t.Length == 0 || t == "Setup") continue;
                if (t.StartsWith("#", StringComparison.Ordinal))
                {
                    // A comment (the video reader's header).
                    if (t.IndexOf("video", StringComparison.OrdinalIgnoreCase) >= 0) log.FromVideo = true;
                    continue;
                }
                log.LineCount++;
                if (t.StartsWith("•", StringComparison.Ordinal))
                {
                    if (last != null)
                        foreach (var part in t.Substring(1).Split(','))
                        {
                            string card = part.Trim();
                            if (card.Length > 0) last.Cards.Add(card);
                        }
                    continue;
                }
                if (t.StartsWith("- ", StringComparison.Ordinal) || t == "-")
                {
                    var sub = new LogLine { Text = t.Length > 2 ? t.Substring(2).Trim() : "", Number = i + 1 };
                    if (top != null) top.Subs.Add(sub);
                    else turn.Lines.Add(sub);
                    last = sub;
                    continue;
                }
                if (t.StartsWith("Entering Sudden Death", StringComparison.Ordinal))
                {
                    game++;
                    gameTurns = 0;
                    turn = new LogTurn { Number = 0, Game = game, SuddenDeath = true };
                    log.Turns.Add(turn);
                    top = null;
                    last = null;
                    continue;
                }
                var m = TurnHeader.Match(t);
                if (m.Success)
                {
                    // TCG Live doesn't number its turns; the video reader does ("X's Turn 9": a video can start mid-game).
                    int numbered;
                    if (m.Groups[2].Success && int.TryParse(m.Groups[2].Value, out numbered) && numbered > 0) gameTurns = numbered;
                    else ++gameTurns;
                    turn = new LogTurn { Player = m.Groups[1].Value.Trim(), Number = gameTurns, Game = game };
                    log.Turns.Add(turn);
                    log.AddPlayer(turn.Player);
                    top = null;
                    last = null;
                    continue;
                }
                top = new LogLine { Text = t, Number = i + 1 };
                turn.Lines.Add(top);
                last = top;

                if ((m = CoinChoice.Match(t)).Success || (m = CoinWon.Match(t)).Success || (m = OpeningHand.Match(t)).Success)
                    log.AddPlayer(m.Groups[1].Value.Trim());
                else if ((m = GoesFirst.Match(t)).Success)
                {
                    string who = m.Groups[1].Value.Trim();
                    log.AddPlayer(who);
                    // The first game's order (a Sudden Death game decides again).
                    if (game == 0)
                    {
                        if (m.Groups[2].Value == "first") log.FirstPlayer = who;
                        else goesSecond = who;
                    }
                }
                else if ((m = Wins.Match(t)).Success)
                {
                    log.Winner = m.Groups[1].Value.Trim();
                    log.Ending = t;
                }
            }
            if (log.FirstPlayer == null && goesSecond != null)
                foreach (var p in log.Players) if (p != goesSecond) log.FirstPlayer = p;
            if (log.FirstPlayer == null)
                foreach (var tn in log.Turns)
                    if (tn.Number > 0)
                    {
                        log.FirstPlayer = tn.Player;
                        break;
                    }
            // Keep the players in turn order: the first player is player 0.
            if (log.FirstPlayer != null && log.Players.Count == 2 && log.Players[0] != log.FirstPlayer)
            {
                log.Players.Reverse();
            }
            if (log.Winner != null && !log.Players.Contains(log.Winner)) log.Winner = null;
            return log;
        }

        private void AddPlayer(string name)
        {
            if (string.IsNullOrEmpty(name) || Players.Contains(name)) return;
            Players.Add(name);
        }

        /// <summary>0 or 1 for a player's name, -1 if it isn't one of them.</summary>
        public int IndexOf(string name)
        {
            return name == null ? -1 : Players.IndexOf(name);
        }
    }
}
