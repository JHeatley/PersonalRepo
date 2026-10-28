using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>What studying one game log did.</summary>
    public sealed class LogStudyReport
    {
        public string Source = "";
        /// <summary>"TCG Live battle log" or "game from this app".</summary>
        public string Kind = "";
        public string[] Players = { "", "" };
        /// <summary>The winner's name, or "" if the log doesn't say.</summary>
        public string Winner = "";
        public int Turns;
        public int Lines;
        /// <summary>Lines the reader didn't understand (they're skipped).</summary>
        public int UnreadLines;
        public readonly List<string> Unread = new List<string>();
        public readonly List<string> UnknownCards = new List<string>();
        /// <summary>Plays the reader tried to line up with what the game allowed at that moment, and how many it could.</summary>
        public int Plays;
        public int Matched;
        /// <summary>Decisions each player's plays gave the AI to learn from.</summary>
        public readonly int[] Decisions = new int[2];
        public readonly int[] Studied = new int[2];
        /// <summary>A played-back game from this app ran to its end the way it went.</summary>
        public bool Complete;
        public string Note = "";
        public string Error;

        public int TotalStudied { get { return Studied[0] + Studied[1]; } }

        public string Summary()
        {
            if (!string.IsNullOrEmpty(Error)) return Source + ": " + Error;
            var sb = new StringBuilder();
            sb.Append(Players[0]).Append(" vs ").Append(Players[1]).Append(" (").Append(Kind).Append(")");
            if (!string.IsNullOrEmpty(Winner)) sb.Append(", ").Append(Winner).Append(" won");
            if (Turns > 0) sb.Append(", ").Append(Turns).Append(Turns == 1 ? " turn" : " turns");
            sb.Append(": ").Append(TotalStudied).Append(" decisions studied");
            if (Plays > 0) sb.Append(" (").Append(Matched).Append(" of ").Append(Plays).Append(" plays lined up)");
            if (UnreadLines > 0) sb.Append("; ").Append(UnreadLines).Append(UnreadLines == 1 ? " line" : " lines").Append(" not understood");
            if (UnknownCards.Count > 0) sb.Append("; cards not in the pool: ").Append(string.Join(", ", UnknownCards.Take(5).ToArray()));
            if (!string.IsNullOrEmpty(Note)) sb.Append(". ").Append(Note);
            return sb.ToString();
        }
    }

    /// <summary>
    /// The AI studies a game log (v76): a Pokémon TCG Live battle log or a game played in this app (GameRecord). It rebuilds
    /// every decision a player made with the options they had, and learns from them the way it learns from your games
    /// (LearningAgent.LearnFromPlayer): the feature weights move toward the plays it wouldn't have made, the neural network
    /// studies every choice. The winner's plays count fully, the loser's half.
    ///
    /// A game from this app is played back exactly (same decks, seed and answers), so every decision of both players comes
    /// back. A TCG Live log is replayed on a rebuilt board (TcgLiveReplay): the log owner's hand is known from their draws,
    /// so all their plays are studied; the opponent's hand is hidden, so only their board plays (attacks, Abilities,
    /// retreating, ending the turn) are.
    /// </summary>
    public static class LogStudy
    {
        /// <summary>Studies one log (either kind). brain null: only reads it (for checking).</summary>
        public static LogStudyReport Study(string text, string source, CardDatabase db, CardScriptRegistry scripts, LearningBrain brain)
        {
            if (GameRecord.IsRecord(text))
            {
                var problems = new List<string>();
                var record = GameRecord.Parse(text, db, problems);
                var report = StudyRecord(record, scripts, brain);
                report.Source = source ?? "";
                if (problems.Count > 0 && string.IsNullOrEmpty(report.Error)) report.Note = (report.Note.Length > 0 ? report.Note + " " : "") + problems[0];
                return report;
            }
            if (!TcgLiveLog.LooksLikeBattleLog(text))
                return new LogStudyReport { Source = source ?? "", Error = "This isn't a TCG Live battle log or a game saved by this app." };
            var log = TcgLiveLog.Parse(text);
            var replay = new TcgLiveReplay(db, scripts, log);
            replay.FillAttackCosts = log.FromVideo;
            var r = replay.Run();
            if (log.FromVideo) r.Kind = "video";
            r.Source = source ?? "";
            bool conceded = (log.Ending ?? "").IndexOf("conceded", StringComparison.OrdinalIgnoreCase) >= 0;
            if (conceded)
                r.Note = "It ended with a concession (in TCG Live usually the turn timer running out), so the loser's plays count as much as a draw's.";
            if (string.IsNullOrEmpty(r.Error)) LearnFrom(brain, replay, r, log.IndexOf(log.Winner), conceded);
            return r;
        }

        /// <summary>Plays a game from this app back and learns from both players' decisions.</summary>
        public static LogStudyReport StudyRecord(GameRecord record, CardScriptRegistry scripts, LearningBrain brain)
        {
            var report = new LogStudyReport { Kind = "game from this app", Players = new[] { record.Names[0], record.Names[1] } };
            if (record.Decks[0].Count == 0 || record.Decks[1].Count == 0)
            {
                report.Error = "The record has no deck.";
                return report;
            }
            var logs = new[] { new DecisionLog(), new DecisionLog() };
            var agents = new IPlayerAgent[2];
            for (int s = 0; s < 2; s++)
            {
                if (record.Interactive[s])
                {
                    var replay = new InteractiveReplayAgent(record.Answers[s]);
                    agents[s] = new InteractiveRecordingAgent(replay, logs[s]);
                }
                else agents[s] = new RecordingAgent(new ReplayAgent(record.Answers[s]), logs[s]);
            }
            var engine = new GameEngine(scripts, agents[0], agents[1]);
            engine.PrizeCount = record.PrizeCount;
            engine.Rules = record.Rules ?? GameRules.Default;
            engine.Birthdays = new[] { record.Birthdays[0], record.Birthdays[1] };
            engine.Today = record.Today;
            engine.OrderChooser = record.OrderChooser;
            engine.OrderChooserIsHigherSeed = record.OrderChooserIsHigherSeed;
            engine.TieBreaker = record.TieBreaker;
            engine.AskPersonEveryChoice = record.AskedEveryChoice;
            engine.PlayerNames = new[] { record.Names[0], record.Names[1] };
            engine.CardsPerPlayer = record.Decks[0].Count == record.Decks[1].Count ? record.Decks[0].Count : 0;
            logs[0].Engine = engine;
            logs[1].Engine = engine;
            int winner = record.Winner;
            try
            {
                var result = engine.Run(record.Decks[0], record.Decks[1], record.Seed);
                report.Complete = true;
                report.Turns = result.Turns;
                if (record.Winner >= -1 && result.Winner != record.Winner)
                    report.Note = "It played out differently from the record (cards may have changed since), so it was studied as it played out now.";
                winner = result.Winner;
            }
            catch (ReplayEndedException e)
            {
                report.Note = e.Message + " The decisions up to there were studied.";
                if (engine.State != null) report.Turns = engine.State.TurnNumber;
            }
            catch (Exception e)
            {
                report.Note = "It couldn't be played back to the end (" + e.GetType().Name + ": " + e.Message + "); the decisions up to there were studied.";
                if (engine.State != null) report.Turns = engine.State.TurnNumber;
            }
            report.Winner = winner >= 0 && winner < 2 ? record.Names[winner] : "";
            var decisions = new List<RecordedDecision>[] { logs[0].Decisions(), logs[1].Decisions() };
            for (int s = 0; s < 2; s++) report.Decisions[s] = decisions[s].Count;
            if (brain != null)
            {
                for (int s = 0; s < 2; s++)
                    report.Studied[s] = LearningAgent.LearnFromPlayer(brain, decisions[s], record.DeckNames[s], record.DeckNames[1 - s], Weight(s, winner));
                brain.NoteLogStudied(report.TotalStudied);
            }
            return report;
        }

        /// <summary>
        /// How much a player's plays count: the winner's fully, the loser's half, either side of a draw 0.75. A game that ended in a
        /// concession (TCG Live concedes for a player whose turn timer runs out) wasn't decided by the plays, so its loser's count 0.75.
        /// </summary>
        public static double Weight(int seat, int winner, bool conceded = false)
        {
            if (winner < 0 || winner > 1) return 0.75;
            if (winner == seat) return 1.0;
            return conceded ? 0.75 : 0.5;
        }

        /// <summary>Studies a TCG Live log and learns from it (the replay and the learning in one go).</summary>
        internal static void LearnFrom(LearningBrain brain, TcgLiveReplay replay, LogStudyReport report, int winner, bool conceded)
        {
            if (brain == null) return;
            for (int s = 0; s < 2; s++)
                report.Studied[s] = LearningAgent.LearnFromPlayer(brain, replay.Decisions[s], "", "", Weight(s, winner, conceded));
            brain.NoteLogStudied(report.TotalStudied);
        }
    }

    /// <summary>
    /// Replays a TCG Live battle log on a board rebuilt with the engine's scenario tools (v76). Before each play of the player
    /// whose turn it is, it asks the engine what that player could do (GetMainActions) and finds the play in the list: that's
    /// one decision, with every option, the AI can learn from. Then the play and everything the log says it did (cards drawn,
    /// discarded, searched, attached, damage, Knock Outs, Prize cards) are put on the board by hand.
    ///
    /// Cards no one has seen are placeholders: the opponent's hand, both decks and the Prize cards. A Pokémon name with
    /// several cards in the pool is read as the one whose attacks and Abilities the log uses (Standard-legal ones first).
    /// </summary>
    internal sealed class TcgLiveReplay
    {
        private readonly TcgLiveLog log;
        private readonly LogStudyReport report = new LogStudyReport { Kind = "TCG Live battle log" };
        private readonly GameEngine engine;
        private readonly CardDef hidden, hiddenBasic;
        private readonly bool[] handKnown = new bool[2];
        private readonly Dictionary<string, List<CardDef>> byName = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<CardDef>> byLoose = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        private readonly Dictionary<string, CardDef> resolved = new Dictionary<string, CardDef>(StringComparer.Ordinal);
        private readonly Dictionary<string, HashSet<string>> movesUsed = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        public readonly List<RecordedDecision>[] Decisions = { new List<RecordedDecision>(), new List<RecordedDecision>() };
        /// <summary>
        /// A log from a video (v77): Energy isn't shown, so before an attack that can't be lined up because its Pokémon lacks
        /// Energy, the attack's cost is attached (basic Energy) and it's tried again.
        /// </summary>
        public bool FillAttackCosts;
        private int turn;
        private int current = -1;
        /// <summary>The Pokémon whose attack or Ability the lines being read are part of (Teal Dance attaches to itself).</summary>
        private PokemonInPlay effectUser;
        /// <summary>A Stadium just replaced by a new one: the log's "discarded [it]" line that follows is about it, not the hand.</summary>
        private CardInstance replacedStadium;

        private const RegexOptions Opt = RegexOptions.CultureInvariant;
        private const string Poss = "(?:’|')s ";
        private static readonly Regex BasicEnergyName = new Regex(@"^Basic (\w+) Energy$", Opt);
        private static readonly Regex OpeningHand = new Regex(@"^drew (\d+) cards? for the opening hand\.?$", Opt);
        private static readonly Regex DrewMore = new Regex(@"^drew (\d+) more cards? because\b", Opt);
        private static readonly Regex DrewToBench = new Regex(@"^drew (\d+) cards? and played (?:them|it) to the Bench\.?$", Opt);
        private static readonly Regex DrewOne = new Regex(@"^drew a card\.?$", Opt);
        private static readonly Regex DrewCount = new Regex(@"^drew (\d+) cards?\.?$", Opt);
        private static readonly Regex DrewNamed = new Regex(@"^drew (.+?)\.?$", Opt);
        private static readonly Regex PlayedTo = new Regex(@"^played (.+) to the (Active Spot|Bench|Stadium spot)\.?$", Opt);
        private static readonly Regex Played = new Regex(@"^played (.+?)\.?$", Opt);
        private static readonly Regex Attached = new Regex(@"^attached (.+?) to (.+) (in the Active Spot|on the Bench)\.?$", Opt);
        private static readonly Regex Evolved = new Regex(@"^evolved (.+?) to (.+) (in the Active Spot|on the Bench)\.?$", Opt);
        private static readonly Regex Retreated = new Regex(@"^retreated (.+?)(?: to the Bench)?\.?$", Opt);
        private static readonly Regex TookPrize = new Regex(@"^took (a|\d+) Prize cards?\.?$", Opt);
        private static readonly Regex DiscardedCount = new Regex(@"^discarded (\d+) cards?\.?$", Opt);
        private static readonly Regex DiscardedNamed = new Regex(@"^discarded (.+?)\.?$", Opt);
        private static readonly Regex ShuffledIn = new Regex(@"^shuffled (\d+) cards? into their deck\.?$", Opt);
        private static readonly Regex MovedToHand = new Regex(@"^moved (.+?)" + Poss + @"(.+) to their hand\.?$", Opt);
        private static readonly Regex Used = new Regex(@"^'s (.+?) used (.+?)(?: on (.+?)" + Poss + @"(.+?) for (\d+) damage)?\.(?:\s.*)?$", Opt);
        private static readonly Regex KnockedOut = new Regex(@"^'s (.+) was Knocked Out!?$", Opt);
        private static readonly Regex NowActive = new Regex(@"^'s (.+) is now in the Active Spot\.?$", Opt);
        private static readonly Regex SwitchedWith = new Regex(@"^'s (.+?) was switched with (.+?)" + Poss + @"(.+?) to become the Active Pokémon\.?$", Opt);
        private static readonly Regex RetreatedPossessive = new Regex(@"^'s (.+) retreated\.?$", Opt);
        private static readonly Regex AddedToHand = new Regex(@"^(.+) was added to (.+?)" + Poss + @"hand\.?$", Opt);
        private static readonly Regex DiscardedFrom = new Regex(@"^\d+ cards? (?:was|were) discarded from (.+?)" + Poss + @"(.+?)\.?$", Opt);
        private static readonly Regex DrewNamedToBench = new Regex(@"^drew (.+?) and played (?:it|them) to the Bench\.?$", Opt);
        private static readonly Regex PutCounters = new Regex(@"^put (\d+) damage counters? on (.+?)" + Poss + @"(.+?)\.?$", Opt);
        private static readonly Regex MovedCounters = new Regex(@"^moved (\d+) damage counters? from (.+?)" + Poss + @"(.+?) to (.+?)" + Poss + @"(.+?)\.?$", Opt);
        private static readonly Regex BottomOfDeck = new Regex(@"^put (\d+) cards? on the bottom of their deck\.?$", Opt);
        private static readonly Regex ShuffledNamed = new Regex(@"^shuffled (.+?) into their deck\.?$", Opt);
        private static readonly Regex Healed = new Regex(@"^'s (.+?) healed (\d+) damage\.?$", Opt);
        private static readonly Regex Condition = new Regex(@"^'s (.+?) is now (Poisoned|Burned|Asleep|Paralyzed|Confused)\.?$", Opt);
        private static readonly Regex CountersPlaced = new Regex(@"^(\d+) damage counters? (?:was|were) placed on (.+?)" + Poss + @"(.+?) for the Special Condition \w+\.?$", Opt);
        private static readonly Regex EnergyOff = new Regex(@"^(.+) was discarded from (.+?)" + Poss + @"(.+?)\.?$", Opt);
        private static readonly Regex CountOfCards = new Regex(@"^(\d+) cards?$", Opt);
        private static readonly Regex Ignored = new Regex(@"^(Pokémon Checkup|.+ was activated\.?$|Entering Sudden Death|flipped a coin|.+ flipped a coin|\d+ drawn cards|Damage breakdown|Cards revealed from Mulligan|Damage to .+ was prevented|Opponent conceded|.+ wins\.?$|.+ chose (heads|tails)|.+ won the coin toss|.+ decided to go)", Opt);

        public TcgLiveReplay(CardDatabase db, CardScriptRegistry scripts, TcgLiveLog log)
        {
            this.log = log;
            foreach (var d in db.Cards)
            {
                AddTo(byName, d.Name, d);
                AddTo(byLoose, CardDatabase.LooseName(d.Name), d);
            }
            hidden = new CardDef { Key = "LOG-HIDDEN", Name = "(unknown card)", Kind = CardKind.Pokemon, Stage = Stage.Stage2, EvolvesFrom = "(unknown)", Hp = 10 };
            hiddenBasic = new CardDef { Key = "LOG-HIDDEN-BASIC", Name = "(unknown Basic Pokémon)", Kind = CardKind.Pokemon, Stage = Stage.Basic, Hp = 10 };
            engine = new GameEngine(scripts, new HeuristicAgent(1), new HeuristicAgent(2));
            engine.BeginScenario(76);
        }

        private static void AddTo(Dictionary<string, List<CardDef>> map, string key, CardDef d)
        {
            List<CardDef> list;
            if (!map.TryGetValue(key, out list))
            {
                list = new List<CardDef>();
                map[key] = list;
            }
            list.Add(d);
        }

        private PlayerState P(int i)
        {
            return engine.State.Players[i];
        }

        public LogStudyReport Run()
        {
            if (log.Players.Count != 2)
            {
                report.Error = log.Players.Count < 2 ? "Couldn't find the two players in this log." : "This log seems to have more than two players.";
                return report;
            }
            report.Players = new[] { log.Players[0], log.Players[1] };
            report.Winner = log.Winner ?? "";
            Prescan();
            for (int p = 0; p < 2; p++)
            {
                for (int i = 0; i < 60; i++) P(p).Deck.Add(engine.NewCard(p, i % 3 == 0 ? hiddenBasic : hidden));
                for (int i = 0; i < 6; i++)
                {
                    TakeTopOfDeck(p);
                    P(p).Prizes.Add(engine.NewCard(p, hidden));
                }
            }
            foreach (var t in log.Turns)
            {
                if (t.SuddenDeath) StartSuddenDeath();
                if (t.Number > 0)
                {
                    int who = log.IndexOf(t.Player);
                    if (who < 0) continue;
                    turn = t.Number;
                    current = who;
                    engine.ScenarioSetTurn(who, turn);
                    report.Turns++;
                }
                foreach (var line in t.Lines) Apply(line, true, t.Number == 0, "");
            }
            for (int s = 0; s < 2; s++) report.Decisions[s] = Decisions[s].Count;
            return report;
        }

        /// <summary>Sudden Death: a new game, everything back in the decks, 1 Prize card each.</summary>
        private void StartSuddenDeath()
        {
            for (int p = 0; p < 2; p++)
            {
                var ps = P(p);
                ps.Hand.Clear();
                ps.Discard.Clear();
                ps.Prizes.Clear();
                ps.LostZone.Clear();
                ps.Bench.Clear();
                ps.Active = null;
                ps.Deck.Clear();
                for (int i = 0; i < 59; i++) ps.Deck.Add(engine.NewCard(p, i % 3 == 0 ? hiddenBasic : hidden));
                ps.Prizes.Add(engine.NewCard(p, hidden));
                handKnown[p] = false;
            }
            engine.State.Stadium = null;
            engine.State.StadiumOwner = -1;
            turn = 0;
            current = -1;
        }

        // ------------------------------------------------------------------ Names and cards

        private static string CleanName(string name)
        {
            name = (name ?? "").Trim();
            var m = BasicEnergyName.Match(name);
            return m.Success ? m.Groups[1].Value + " Energy" : name;
        }

        private static bool SameName(string a, string b)
        {
            if (a == null || b == null) return false;
            return a == b || CardDatabase.LooseName(CleanName(a)) == CardDatabase.LooseName(CleanName(b));
        }

        private static bool NameIs(CardDef d, string name)
        {
            return d != null && SameName(d.Name, name);
        }

        /// <summary>Reads which attacks and Abilities each player's Pokémon use, to tell apart cards with the same name.</summary>
        private void Prescan()
        {
            foreach (var t in log.Turns)
                foreach (var line in t.Lines) PrescanLine(line);
        }

        private void PrescanLine(LogLine line)
        {
            string rest;
            int who = ActorOf(line.Text, out rest);
            if (who >= 0)
            {
                var m = Used.Match(rest);
                if (m.Success)
                {
                    string key = who + "|" + CleanName(m.Groups[1].Value);
                    HashSet<string> moves;
                    if (!movesUsed.TryGetValue(key, out moves))
                    {
                        moves = new HashSet<string>(StringComparer.Ordinal);
                        movesUsed[key] = moves;
                    }
                    moves.Add(m.Groups[2].Value.Trim());
                }
            }
            foreach (var sub in line.Subs) PrescanLine(sub);
        }

        /// <summary>The card a name means for this player.</summary>
        private CardDef Def(int player, string rawName)
        {
            string name = CleanName(rawName);
            string key = player + "|" + name;
            CardDef found;
            if (resolved.TryGetValue(key, out found)) return found;
            List<CardDef> list;
            if (!byName.TryGetValue(name, out list)) byLoose.TryGetValue(CardDatabase.LooseName(name), out list);
            if (list == null || list.Count == 0)
            {
                if (!report.UnknownCards.Contains(name)) report.UnknownCards.Add(name);
                found = new CardDef { Key = "LOG-" + name, Name = name, Kind = CardKind.Pokemon, Stage = Stage.Stage2, EvolvesFrom = "(unknown)", Hp = 10 };
            }
            else
            {
                HashSet<string> moves;
                movesUsed.TryGetValue(key, out moves);
                double best = double.MinValue;
                foreach (var d in list)
                {
                    double score = 0;
                    if (moves != null)
                    {
                        foreach (var a in d.Attacks) if (moves.Contains(a.Name)) score += 100;
                        foreach (var ab in d.Abilities) if (moves.Contains(ab.Name)) score += 100;
                    }
                    if (Formats.IsStandardCard(d)) score += 10;
                    if (d.IsEnergy && d.IsBasicEnergy) score += 5;
                    if (score > best)
                    {
                        best = score;
                        found = d;
                    }
                }
            }
            resolved[key] = found;
            return found;
        }

        /// <summary>Which player a line starts with ("TFR89 played..." or "TFR89's Crustle..."), and the rest of the line.</summary>
        private int ActorOf(string text, out string rest)
        {
            rest = text ?? "";
            int best = -1, bestLength = -1;
            for (int i = 0; i < log.Players.Count && i < 2; i++)
            {
                string name = log.Players[i];
                if (name.Length <= bestLength) continue;
                if (rest.StartsWith(name + " ", StringComparison.Ordinal) || rest.StartsWith(name + "'s ", StringComparison.Ordinal) || rest.StartsWith(name + "’s ", StringComparison.Ordinal))
                {
                    best = i;
                    bestLength = name.Length;
                }
            }
            if (best < 0) return -1;
            rest = rest.Substring(bestLength);
            if (rest.StartsWith("’s ", StringComparison.Ordinal)) rest = "'s " + rest.Substring(3);
            else if (rest.StartsWith(" ", StringComparison.Ordinal)) rest = rest.Substring(1);
            return best;
        }

        // ------------------------------------------------------------------ Cards moving

        private void TakeTopOfDeck(int p)
        {
            var deck = P(p).Deck;
            if (deck.Count > 0) deck.RemoveAt(deck.Count - 1);
        }

        private CardInstance FromDeck(int p, CardDef d)
        {
            TakeTopOfDeck(p);
            return engine.NewCard(p, d);
        }

        private void DrawKnown(int p, string name)
        {
            P(p).Hand.Add(FromDeck(p, Def(p, name)));
        }

        private void DrawHidden(int p, int n)
        {
            for (int i = 0; i < n; i++) P(p).Hand.Add(FromDeck(p, hidden));
        }

        private void HandBackToDeck(int p)
        {
            var hand = P(p).Hand;
            for (int i = 0; i < hand.Count; i++) P(p).Deck.Add(engine.NewCard(p, hidden));
            hand.Clear();
        }

        /// <summary>Takes a card out of the hand: the card itself if it's known there, a placeholder otherwise.</summary>
        private CardInstance TakeFromHand(int p, string name)
        {
            var def = Def(p, name);
            var hand = P(p).Hand;
            int at = -1;
            for (int i = 0; i < hand.Count && at < 0; i++) if (NameIs(hand[i].Def, def.Name)) at = i;
            if (at >= 0)
            {
                var card = hand[at];
                hand.RemoveAt(at);
                return card;
            }
            for (int i = 0; i < hand.Count && at < 0; i++) if (hand[i].Def == hidden) at = i;
            if (at >= 0) hand.RemoveAt(at);
            return engine.NewCard(p, def);
        }

        private CardInstance TakeFromDiscard(int p, string name)
        {
            var discard = P(p).Discard;
            for (int i = discard.Count - 1; i >= 0; i--)
            {
                if (!NameIs(discard[i].Def, name)) continue;
                var card = discard[i];
                discard.RemoveAt(i);
                return card;
            }
            return null;
        }

        /// <summary>Takes an Energy of that name off another of the player's Pokémon (an effect that moves Energy).</summary>
        private CardInstance TakeFromPlay(int p, string name, PokemonInPlay notFrom)
        {
            foreach (var mon in P(p).InPlay())
            {
                if (mon == notFrom) continue;
                for (int i = 0; i < mon.Energy.Count; i++)
                {
                    if (!NameIs(mon.Energy[i].Def, name)) continue;
                    var card = mon.Energy[i];
                    mon.Energy.RemoveAt(i);
                    return card;
                }
            }
            return null;
        }

        private static bool SameMon(PokemonInPlay mon, string name)
        {
            return mon != null && SameName(mon.Name, name);
        }

        /// <summary>The player's Pokémon with that name, in that spot when the log says ("in the Active Spot", "on the Bench").</summary>
        private PokemonInPlay FindMon(int p, string name, string spot, PokemonInPlay prefer)
        {
            var ps = P(p);
            bool bench = spot != null && spot.StartsWith("on the Bench", StringComparison.Ordinal);
            bool active = spot != null && spot.StartsWith("in the Active", StringComparison.Ordinal);
            if (prefer != null && SameMon(prefer, name) && (!bench || ps.Bench.Contains(prefer)) && (!active || ps.Active == prefer)) return prefer;
            if (!bench && SameMon(ps.Active, name)) return ps.Active;
            foreach (var mon in ps.Bench) if (SameMon(mon, name)) return mon;
            if (SameMon(ps.Active, name)) return ps.Active;
            return null;
        }

        /// <summary>A Pokémon by its owner's name and its name; the log sometimes names the wrong owner, so the other side is tried too.</summary>
        private PokemonInPlay AnyMon(string owner, string name)
        {
            int p = log.IndexOf((owner ?? "").Trim());
            if (p >= 0)
            {
                var mon = FindMon(p, name, null, null);
                if (mon != null) return mon;
            }
            for (int i = 0; i < 2; i++)
            {
                if (i == p) continue;
                var mon = FindMon(i, name, null, null);
                if (mon != null) return mon;
            }
            return null;
        }

        private void PutIntoPlay(int p, CardInstance card, bool asActive)
        {
            var mon = new PokemonInPlay(engine.State.NextUid++, p, card, turn);
            if (asActive && P(p).Active == null) P(p).Active = mon;
            else P(p).Bench.Add(mon);
        }

        private void MakeActive(int p, PokemonInPlay mon)
        {
            var ps = P(p);
            if (mon == null || ps.Active == mon) return;
            int at = ps.Bench.IndexOf(mon);
            if (at < 0) return;
            if (ps.Active != null) ps.Bench[at] = ps.Active;
            else ps.Bench.RemoveAt(at);
            ps.Active = mon;
            mon.MovedToActiveOnTurn = turn;
        }

        private void KnockOut(int p, PokemonInPlay mon)
        {
            if (mon == null) return;
            var ps = P(p);
            foreach (var c in mon.Stack) ps.Discard.Add(c);
            foreach (var c in mon.Energy) ps.Discard.Add(c);
            foreach (var c in mon.Tools) ps.Discard.Add(c);
            if (ps.Active == mon) ps.Active = null;
            else ps.Bench.Remove(mon);
        }

        // ------------------------------------------------------------------ Decisions

        /// <summary>
        /// One play of the player whose turn it is: what the engine says they could do right now, and which of those the log
        /// says they did. Plays from the hand are only studied when that player's hand is known.
        /// </summary>
        private DecisionOption Decide(int player, bool fromHand, Func<DecisionOption, bool> isThePlay, Func<DecisionOption, bool> orElse = null)
        {
            if (fromHand && !handKnown[player]) return null;
            report.Plays++;
            List<DecisionOption> options;
            try
            {
                options = engine.GetMainActions(player);
            }
            catch (Exception)
            {
                return null;
            }
            int index = -1;
            for (int i = 0; i < options.Count && index < 0; i++) if (isThePlay(options[i])) index = i;
            for (int i = 0; i < options.Count && index < 0 && orElse != null; i++) if (orElse(options[i])) index = i;
            if (index < 0) return null;
            report.Matched++;
            if (options.Count < 2) return options[index];
            var request = new DecisionRequest
            {
                Player = player,
                Kind = DecisionKind.MainAction,
                Prompt = "Choose your next action.",
                Options = options,
                View = PlayerView.Create(engine, player)
            };
            Decisions[player].Add(new RecordedDecision { Request = request, Answer = new List<int> { index }, Turn = turn, Index = Decisions[player].Count });
            return options[index];
        }

        /// <summary>The player's Pokémon with this id, or null.</summary>
        private PokemonInPlay MonByUid(int p, int uid)
        {
            foreach (var mon in P(p).InPlay()) if (mon.Uid == uid) return mon;
            return null;
        }

        private bool MonNamed(int p, int uid, string name)
        {
            return SameMon(MonByUid(p, uid), name);
        }

        /// <summary>Removes a card nobody has seen from the hand (any card if none is unknown).</summary>
        private void TakeHiddenFromHand(int p)
        {
            var hand = P(p).Hand;
            for (int i = hand.Count - 1; i >= 0; i--)
            {
                if (hand[i].Def != hidden) continue;
                hand.RemoveAt(i);
                return;
            }
            if (hand.Count > 0) hand.RemoveAt(hand.Count - 1);
        }

        // ------------------------------------------------------------------ Reading lines

        private void Unread(LogLine line)
        {
            report.UnreadLines++;
            if (report.Unread.Count < 20) report.Unread.Add(line.Number + ": " + line.Text);
        }

        /// <summary>
        /// Applies one line (and the lines under it). top: a line of its own (a play), not a "- ..." line under one.
        /// effect: the text of the card or move whose effect this line is part of (says where attached Energy came from).
        /// </summary>
        private void Apply(LogLine line, bool top, bool setup, string effect)
        {
            report.Lines++;
            string rest;
            int who = ActorOf(line.Text, out rest);
            if (who < 0)
            {
                ApplyNoActor(line);
                ApplySubs(line, setup, effect);
                return;
            }
            bool play = top && !setup && who == current;
            Match m;

            if (rest.StartsWith("ended their turn", StringComparison.Ordinal))
            {
                if (play) Decide(who, false, o => o.Action == ActionType.EndTurn);
                return;
            }
            if (rest == "took a mulligan." || rest == "took a mulligan")
            {
                HandBackToDeck(who);
                DrawHidden(who, 7);
                handKnown[who] = false;
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = OpeningHand.Match(rest)).Success)
            {
                HandBackToDeck(who);
                var cards = CardsUnder(line);
                if (cards.Count > 0)
                {
                    handKnown[who] = true;
                    foreach (var c in cards) DrawKnown(who, c);
                }
                else DrawHidden(who, Int(m.Groups[1].Value));
                return;
            }
            if ((m = DrewMore.Match(rest)).Success)
            {
                if (line.Subs.Count > 0) ApplySubs(line, setup, effect);
                else DrawHidden(who, Int(m.Groups[1].Value));
                return;
            }
            if ((m = DrewToBench.Match(rest)).Success)
            {
                foreach (var c in line.Cards) PutIntoPlay(who, FromDeck(who, Def(who, c)), false);
                ApplySubs(line, setup, effect);
                return;
            }
            if (DrewOne.IsMatch(rest))
            {
                DrawHidden(who, 1);
                return;
            }
            if ((m = DrewCount.Match(rest)).Success)
            {
                int n = Int(m.Groups[1].Value);
                if (line.Cards.Count > 0) foreach (var c in line.Cards) DrawKnown(who, c);
                else DrawHidden(who, n);
                return;
            }
            if ((m = DrewNamedToBench.Match(rest)).Success)
            {
                PutIntoPlay(who, FromDeck(who, Def(who, m.Groups[1].Value)), false);
                return;
            }
            if ((m = DrewNamed.Match(rest)).Success)
            {
                DrawKnown(who, m.Groups[1].Value);
                return;
            }
            if ((m = PutCounters.Match(rest)).Success)
            {
                var mon = AnyMon(m.Groups[2].Value, m.Groups[3].Value);
                if (mon != null) mon.DamageCounters += Int(m.Groups[1].Value);
                return;
            }
            if ((m = MovedCounters.Match(rest)).Success)
            {
                int n = Int(m.Groups[1].Value);
                var source = AnyMon(m.Groups[2].Value, m.Groups[3].Value);
                var dest = AnyMon(m.Groups[4].Value, m.Groups[5].Value);
                if (source != null) source.DamageCounters = Math.Max(0, source.DamageCounters - n);
                if (dest != null) dest.DamageCounters += n;
                return;
            }
            if ((m = BottomOfDeck.Match(rest)).Success)
            {
                int n = Int(m.Groups[1].Value);
                var cards = CardsUnder(line);
                if (cards.Count > 0) foreach (var c in cards) TakeFromHand(who, c);
                else for (int i = 0; i < n; i++) TakeHiddenFromHand(who);
                for (int i = 0; i < n; i++) P(who).Deck.Insert(0, engine.NewCard(who, hidden));
                return;
            }
            if ((m = PlayedTo.Match(rest)).Success)
            {
                string name = m.Groups[1].Value;
                string where = m.Groups[2].Value;
                if (where == "Stadium spot")
                {
                    if (play) Decide(who, true, o => o.Action == ActionType.PlayTrainer && NameIs(o.Card, name));
                    var card = TakeFromHand(who, name);
                    replacedStadium = engine.State.Stadium;
                    if (engine.State.Stadium != null && engine.State.StadiumOwner >= 0) P(engine.State.StadiumOwner).Discard.Add(engine.State.Stadium);
                    engine.State.Stadium = card;
                    engine.State.StadiumOwner = who;
                    P(who).StadiumPlayedThisTurn = true;
                    ApplySubs(line, setup, card.Def.Text);
                    return;
                }
                if (play && where == "Bench") Decide(who, true, o => o.Action == ActionType.BenchBasic && NameIs(o.Card, name));
                // A Pokémon put into play by an effect comes from the deck unless the effect says the hand.
                bool fromHand = top || (effect ?? "").Contains("from your hand");
                var put = fromHand ? TakeFromHand(who, name) : FromDeck(who, Def(who, name));
                PutIntoPlay(who, put, where == "Active Spot");
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = Attached.Match(rest)).Success)
            {
                string name = m.Groups[1].Value;
                var def = Def(who, name);
                var target = FindMon(who, m.Groups[2].Value, m.Groups[3].Value, null);
                if (def.IsTool)
                {
                    if (play) Decide(who, true, o => o.Action == ActionType.PlayTrainer && NameIs(o.Card, name));
                    var tool = TakeFromHand(who, name);
                    if (target != null)
                    {
                        target.Tools.Add(tool);
                        target.ToolFromHandOnTurn = turn;
                    }
                    else P(who).Discard.Add(tool);
                    ApplySubs(line, setup, tool.Def.Text);
                    return;
                }
                if (!top && effectUser != null && SameMon(effectUser, m.Groups[2].Value)) target = FindMon(who, m.Groups[2].Value, m.Groups[3].Value, effectUser);
                if (play)
                {
                    var t = target;
                    string targetName = m.Groups[2].Value;
                    var chosen = Decide(who, true, o => o.Action == ActionType.AttachEnergy && NameIs(o.Card, name) && (t == null || o.PokemonUid == t.Uid),
                        o => o.Action == ActionType.AttachEnergy && NameIs(o.Card, name) && MonNamed(who, o.PokemonUid, targetName));
                    if (chosen != null && chosen.PokemonUid >= 0 && MonByUid(who, chosen.PokemonUid) != null) target = MonByUid(who, chosen.PokemonUid);
                }
                CardInstance energy;
                if (top) energy = TakeFromHand(who, name);
                else energy = EnergyFromEffect(who, name, effect, target);
                if (target != null) target.Energy.Add(energy);
                else P(who).Discard.Add(energy);
                if (top && !setup && who == current) P(who).EnergyAttachedThisTurn = true;
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = Evolved.Match(rest)).Success)
            {
                string fromName = m.Groups[1].Value, toName = m.Groups[2].Value;
                var target = FindMon(who, fromName, m.Groups[3].Value, null);
                if (play)
                {
                    var t = target;
                    var chosen = Decide(who, true, o => o.Action == ActionType.Evolve && NameIs(o.Card, toName) && (t == null || o.PokemonUid == t.Uid),
                        o => o.Action == ActionType.Evolve && NameIs(o.Card, toName) && MonNamed(who, o.PokemonUid, fromName));
                    if (chosen != null && MonByUid(who, chosen.PokemonUid) != null) target = MonByUid(who, chosen.PokemonUid);
                }
                bool fromHand = top || (effect ?? "").Contains("from your hand");
                var card = fromHand ? TakeFromHand(who, toName) : FromDeck(who, Def(who, toName));
                if (target != null)
                {
                    target.Stack.Add(card);
                    target.TurnEvolved = turn;
                    target.AbilityUsedOnTurn.Clear();
                    target.DamageCounters = Math.Max(0, target.DamageCounters);
                }
                else PutIntoPlay(who, card, false);
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = Played.Match(rest)).Success && !rest.Contains(" to the "))
            {
                string name = m.Groups[1].Value;
                // "played Grand Tree." with Grand Tree in play: using the Stadium.
                var stadium = engine.State.Stadium;
                if (stadium != null && NameIs(stadium.Def, name))
                {
                    if (play) Decide(who, false, o => o.Action == ActionType.UseStadium);
                    ApplySubs(line, setup, stadium.Def.Text);
                    return;
                }
                // "played Dudunsparce." with Dudunsparce in play: TCG Live's line for one of its Abilities (or attacks).
                if (Def(who, name).IsPokemon && FindMon(who, name, null, null) != null)
                {
                    var used = FindMon(who, name, null, null);
                    if (play)
                    {
                        var chosen = Decide(who, false, o => o.Action == ActionType.UseAbility && MonNamed(who, o.PokemonUid, name),
                            o => o.Action == ActionType.Attack && MonNamed(who, o.PokemonUid, name));
                        if (chosen != null && MonByUid(who, chosen.PokemonUid) != null) used = MonByUid(who, chosen.PokemonUid);
                        if (chosen != null && chosen.Action == ActionType.UseAbility && chosen.Card != null && chosen.Index >= 0 && chosen.Index < chosen.Card.Abilities.Count)
                            used.AbilityUsedOnTurn[chosen.Card.Abilities[chosen.Index].Name] = turn;
                    }
                    var texts = new StringBuilder();
                    foreach (var ab in used.Def.Abilities) texts.Append(ab.Text).Append(' ');
                    foreach (var a in used.Def.Attacks) texts.Append(a.Text).Append(' ');
                    var outerUser = effectUser;
                    effectUser = used;
                    ApplySubs(line, setup, texts.ToString());
                    effectUser = outerUser;
                    return;
                }
                if (play) Decide(who, true, o => o.Action == ActionType.PlayTrainer && NameIs(o.Card, name));
                var card = TakeFromHand(who, name);
                var def = card.Def;
                if (def.IsSupporter && !setup)
                {
                    var ps = P(who);
                    ps.SupporterPlayedThisTurn = true;
                    ps.SupporterNameThisTurn = def.Name;
                    ps.SupporterThisTurn = def;
                    ps.SupportersPlayedCount++;
                    ps.SupportersThisTurn.Add(def.Name);
                }
                P(who).Discard.Add(card);
                ApplySubs(line, setup, def.Text);
                return;
            }
            if ((m = Retreated.Match(rest)).Success || (m = RetreatedPossessive.Match(rest)).Success)
            {
                if (play) Decide(who, false, o => o.Action == ActionType.Retreat);
                P(who).RetreatedThisTurn = true;
                if (P(who).Active != null) P(who).Active.RetreatedOnTurn = turn;
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = Used.Match(rest)).Success)
            {
                ApplyUsed(line, who, m, play, setup);
                return;
            }
            if ((m = Healed.Match(rest)).Success)
            {
                var mon = FindMon(who, m.Groups[1].Value, null, effectUser);
                if (mon != null) mon.DamageCounters = Math.Max(0, mon.DamageCounters - Int(m.Groups[2].Value) / 10);
                return;
            }
            if ((m = Condition.Match(rest)).Success)
            {
                var mon = FindMon(who, m.Groups[1].Value, "in the Active Spot", null);
                SpecialCondition condition;
                if (mon != null && Enum.TryParse(m.Groups[2].Value, out condition)) mon.Conditions |= condition;
                return;
            }
            if ((m = KnockedOut.Match(rest)).Success)
            {
                var mon = FindMon(who, m.Groups[1].Value, null, null);
                if (mon != null && P(who).Active != null && SameMon(P(who).Active, m.Groups[1].Value)) mon = P(who).Active;
                KnockOut(who, mon);
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = NowActive.Match(rest)).Success || (m = SwitchedWith.Match(rest)).Success)
            {
                string name = m.Groups[1].Value;
                if (!SameMon(P(who).Active, name)) MakeActive(who, FindMon(who, name, "on the Bench", null));
                return;
            }
            if ((m = TookPrize.Match(rest)).Success)
            {
                int n = m.Groups[1].Value == "a" ? 1 : Int(m.Groups[1].Value);
                for (int i = 0; i < n; i++)
                {
                    var prizes = P(who).Prizes;
                    if (prizes.Count > 0) prizes.RemoveAt(prizes.Count - 1);
                    P(who).Hand.Add(engine.NewCard(who, hidden));
                }
                ApplySubs(line, setup, effect);
                return;
            }
            if ((m = DiscardedCount.Match(rest)).Success)
            {
                var cards = CardsUnder(line);
                if (cards.Count > 0) foreach (var c in cards) Discard(who, c, effect);
                else
                {
                    int n = Int(m.Groups[1].Value);
                    for (int i = 0; i < n; i++) TakeHiddenFromHand(who);
                }
                return;
            }
            if ((m = DiscardedNamed.Match(rest)).Success)
            {
                Discard(who, m.Groups[1].Value, effect);
                return;
            }
            if ((m = ShuffledIn.Match(rest)).Success)
            {
                var cards = CardsUnder(line);
                int n = cards.Count > 0 ? cards.Count : Int(m.Groups[1].Value);
                if (cards.Count > 0) foreach (var c in cards) TakeFromHand(who, c);
                else for (int i = 0; i < n; i++) TakeHiddenFromHand(who);
                for (int i = 0; i < n; i++) P(who).Deck.Add(engine.NewCard(who, hidden));
                return;
            }
            if (rest.StartsWith("shuffled their", StringComparison.Ordinal)) return;
            if ((m = ShuffledNamed.Match(rest)).Success)
            {
                TakeFromHand(who, m.Groups[1].Value);
                P(who).Deck.Add(engine.NewCard(who, hidden));
                return;
            }
            if ((m = MovedToHand.Match(rest)).Success)
            {
                int owner = log.IndexOf(m.Groups[1].Value.Trim());
                if (owner < 0) owner = who;
                string name = m.Groups[2].Value;
                var names = CountOfCards.IsMatch(name.Trim()) ? CardsUnder(line) : new List<string> { name };
                foreach (var each in names)
                {
                    var card = TakeFromDiscard(owner, each) ?? FromDeck(owner, Def(owner, each));
                    P(owner).Hand.Add(card);
                }
                return;
            }
            if (Ignored.IsMatch(rest) || Ignored.IsMatch(line.Text))
            {
                ApplySubs(line, setup, effect);
                return;
            }
            Unread(line);
            ApplySubs(line, setup, effect);
        }

        private void ApplyNoActor(LogLine line)
        {
            Match m;
            if ((m = AddedToHand.Match(line.Text)).Success)
            {
                int p = log.IndexOf(m.Groups[2].Value.Trim());
                if (p < 0) return;
                if (m.Groups[1].Value.Trim() == "A card") return; // a Prize card nobody else saw: the placeholder stays
                var hand = P(p).Hand;
                for (int i = hand.Count - 1; i >= 0; i--)
                {
                    if (hand[i].Def != hidden) continue;
                    hand.RemoveAt(i);
                    break;
                }
                hand.Add(engine.NewCard(p, Def(p, m.Groups[1].Value)));
                return;
            }
            if ((m = DiscardedFrom.Match(line.Text)).Success)
            {
                int p = log.IndexOf(m.Groups[1].Value.Trim());
                if (p >= 0)
                {
                    var mon = FindMon(p, m.Groups[2].Value, null, null);
                    if (mon != null) KnockOut(p, mon);
                }
                return;
            }
            if ((m = CountersPlaced.Match(line.Text)).Success)
            {
                var mon = AnyMon(m.Groups[2].Value, m.Groups[3].Value);
                if (mon != null) mon.DamageCounters += Int(m.Groups[1].Value);
                return;
            }
            if ((m = EnergyOff.Match(line.Text)).Success)
            {
                int p = log.IndexOf(m.Groups[2].Value.Trim());
                if (p < 0) return;
                var mon = FindMon(p, m.Groups[3].Value, null, null);
                if (mon == null) return;
                string name = m.Groups[1].Value;
                for (int i = 0; i < mon.Energy.Count; i++)
                {
                    if (!NameIs(mon.Energy[i].Def, name)) continue;
                    P(p).Discard.Add(mon.Energy[i]);
                    mon.Energy.RemoveAt(i);
                    return;
                }
                for (int i = 0; i < mon.Tools.Count; i++)
                {
                    if (!NameIs(mon.Tools[i].Def, name)) continue;
                    P(p).Discard.Add(mon.Tools[i]);
                    mon.Tools.RemoveAt(i);
                    return;
                }
                return;
            }
            if (Ignored.IsMatch(line.Text)) return;
            Unread(line);
        }

        private void ApplySubs(LogLine line, bool setup, string effect)
        {
            foreach (var sub in line.Subs) Apply(sub, false, setup, effect);
        }

        /// <summary>The cards listed under a line, or under its first "- ..." line ("- 7 drawn cards." holds the opening hand).</summary>
        private static List<string> CardsUnder(LogLine line)
        {
            if (line.Cards.Count > 0) return line.Cards;
            foreach (var sub in line.Subs) if (sub.Cards.Count > 0) return sub.Cards;
            return new List<string>();
        }

        private void Discard(int p, string name, string effect)
        {
            if (replacedStadium != null && NameIs(replacedStadium.Def, name))
            {
                replacedStadium = null; // already in its owner's discard pile
                return;
            }
            string e = effect ?? "";
            CardInstance card;
            if (e.Contains("top") && e.Contains("deck") && !e.Contains("from your hand")) card = FromDeck(p, Def(p, name));
            else card = TakeFromHand(p, name);
            P(p).Discard.Add(card);
        }

        /// <summary>Where Energy an effect attaches comes from, by the effect's text: the hand, the discard pile, another Pokémon, or the deck.</summary>
        private CardInstance EnergyFromEffect(int p, string name, string effect, PokemonInPlay target)
        {
            string e = effect ?? "";
            CardInstance card = null;
            if (e.Contains("from your hand")) card = TakeFromHand(p, name);
            else if (e.Contains("discard pile")) card = TakeFromDiscard(p, name);
            else if (e.IndexOf("move", StringComparison.OrdinalIgnoreCase) >= 0) card = TakeFromPlay(p, name, target);
            else if (e.Contains("deck")) card = FromDeck(p, Def(p, name));
            if (card == null)
            {
                // Not said: the hand if the card is known to be there, else the deck.
                bool inHand = P(p).Hand.Any(c => NameIs(c.Def, name));
                card = inHand ? TakeFromHand(p, name) : FromDeck(p, Def(p, name));
            }
            return card;
        }

        /// <summary>"X's Pokémon used Move [on Y's Pokémon for N damage]": an attack or an Ability.</summary>
        private void ApplyUsed(LogLine line, int who, Match m, bool play, bool setup)
        {
            string monName = m.Groups[1].Value;
            string move = m.Groups[2].Value.Trim();
            // The log names the Pokémon but not which one when there are two; a line under it often says where it is.
            string spot = null;
            foreach (var sub in line.Subs)
            {
                if (!sub.Text.Contains(monName)) continue;
                if (sub.Text.EndsWith("on the Bench.", StringComparison.Ordinal)) spot = "on the Bench";
                else if (sub.Text.EndsWith("in the Active Spot.", StringComparison.Ordinal)) spot = "in the Active Spot";
                if (spot != null) break;
            }
            var mon = FindUser(who, monName, move, spot);
            var def = mon != null ? mon.Def : Def(who, monName);
            int attackIndex = -1, abilityIndex = -1;
            for (int i = 0; i < def.Attacks.Count; i++) if (def.Attacks[i].Name == move) attackIndex = i;
            for (int i = 0; i < def.Abilities.Count; i++) if (def.Abilities[i].Name == move) abilityIndex = i;
            bool attack = attackIndex >= 0 && (abilityIndex < 0 || m.Groups[5].Success);
            string effect = "";
            if (attack)
            {
                effect = def.Attacks[attackIndex].Text ?? "";
                if (play)
                {
                    Func<DecisionOption, bool> isAttack = o => o.Action == ActionType.Attack && o.Card != null && o.Index >= 0 && o.Index < o.Card.Attacks.Count && o.Card.Attacks[o.Index].Name == move;
                    var chosen = Decide(who, false, isAttack);
                    if (chosen == null && FillAttackCosts && mon != null && P(who).Active == mon)
                    {
                        // The same play, tried again with its Energy attached.
                        report.Plays--;
                        if (PayFor(who, mon, def.Attacks[attackIndex])) Decide(who, false, isAttack);
                        else report.Plays++;
                    }
                }
                P(who).LastAttackTurn = turn;
                P(who).LastAttackName = move;
                if (mon != null)
                {
                    mon.LastAttackName = move;
                    mon.LastAttackTurn = turn;
                }
                if (m.Groups[5].Success)
                {
                    int target = log.IndexOf(m.Groups[3].Value.Trim());
                    int damage = Int(m.Groups[5].Value);
                    if (target >= 0)
                    {
                        var hit = FindMon(target, m.Groups[4].Value, null, P(target).Active);
                        if (hit != null) hit.DamageCounters += damage / 10;
                    }
                }
            }
            else
            {
                if (abilityIndex >= 0) effect = def.Abilities[abilityIndex].Text ?? "";
                if (play)
                {
                    var user = mon;
                    var chosen = Decide(who, false, o => IsAbility(o, move) && (user == null || o.PokemonUid == user.Uid), o => IsAbility(o, move));
                    if (chosen != null && MonByUid(who, chosen.PokemonUid) != null) mon = MonByUid(who, chosen.PokemonUid);
                }
                if (mon != null) mon.AbilityUsedOnTurn[move] = turn;
            }
            var outer = effectUser;
            effectUser = mon;
            ApplySubs(line, setup, effect);
            effectUser = outer;
        }

        /// <summary>Attaches an attack's cost to the Pokémon as basic Energy (Colorless paid with its own type). False if nothing could be attached.</summary>
        private bool PayFor(int p, PokemonInPlay mon, AttackDef attack)
        {
            if (mon == null || attack == null) return false;
            bool added = false;
            foreach (var type in attack.Cost)
            {
                var energyType = type;
                if (energyType == EnergyType.Colorless) energyType = mon.Def.PokemonType;
                if (energyType == EnergyType.Colorless || energyType == EnergyType.Dragon) energyType = EnergyType.Grass;
                var energy = Def(p, energyType.ToString() + " Energy");
                if (energy == null || !energy.IsEnergy) continue;
                mon.Energy.Add(engine.NewCard(p, energy));
                added = true;
            }
            return added;
        }

        private static bool IsAbility(DecisionOption o, string move)
        {
            return o.Action == ActionType.UseAbility && o.Card != null && o.Index >= 0 && o.Index < o.Card.Abilities.Count && o.Card.Abilities[o.Index].Name == move;
        }

        /// <summary>The Pokémon that used a move: the Active one for an attack, otherwise the one in the hinted spot that hasn't used it yet.</summary>
        private PokemonInPlay FindUser(int who, string name, string move, string spot)
        {
            var ps = P(who);
            var candidates = new List<PokemonInPlay>();
            foreach (var mon in ps.InPlay()) if (SameMon(mon, name)) candidates.Add(mon);
            if (candidates.Count == 0) return null;
            PokemonInPlay best = null;
            int bestScore = int.MinValue;
            foreach (var mon in candidates)
            {
                int score = 0;
                int used;
                if (mon.AbilityUsedOnTurn.TryGetValue(move, out used) && used == turn) score -= 10;
                bool isActive = ps.Active == mon;
                if (spot == "on the Bench" && !isActive) score += 5;
                if (spot == "in the Active Spot" && isActive) score += 5;
                foreach (var a in mon.Def.Attacks) if (a.Name == move && isActive) score += 3;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = mon;
                }
            }
            return best;
        }

        private static int Int(string s)
        {
            int n;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : 0;
        }
    }
}
