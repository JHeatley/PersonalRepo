using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// A game played in this app, saved so it can be played back exactly (v76): both decks in their exact order, the seed,
    /// the engine settings that change play, and every answer each seat gave. Played back with ReplayAgent, the engine asks
    /// the same questions in the same order, so every decision comes back with all its options (what LogStudy learns from).
    /// The battle log lines are saved too, for reading.
    ///
    /// Text format, one record per line, fields separated by tabs:
    ///   # Pokemon TCG game record v1
    ///   when / today / names / decknames / seed / prizes / birthdays / order / interactive / tiebreaker / result
    ///   deck0 / deck1   the card keys in order, separated by spaces
    ///   a0 / a1         one answer of that seat (option indices, comma separated)
    ///   log             a battle log line
    /// </summary>
    public sealed class GameRecord
    {
        public const string Header = "# Pokemon TCG game record v1";

        public string When = "";
        /// <summary>The engine's date for the game (birthdays depend on it).</summary>
        public DateTime Today = DateTime.Today;
        public string[] Names = { "Player 1", "Player 2" };
        public string[] DeckNames = { "", "" };
        public readonly List<CardDef>[] Decks = { new List<CardDef>(), new List<CardDef>() };
        public int Seed;
        public int PrizeCount = 6;
        /// <summary>The format's rules (custom formats, v123); written only when they aren't the normal ones.</summary>
        public GameRules Rules = GameRules.Default;
        public int[] Birthdays = { 0, 0 };
        public int OrderChooser = -1;
        public bool OrderChooserIsHigherSeed;
        /// <summary>A Tie-Breaker game (GameEngine.TieBreaker).</summary>
        public bool TieBreaker;
        /// <summary>Seats played by a person (the engine asks a person a few more questions than a bot).</summary>
        public bool[] Interactive = { false, false };
        /// <summary>The person was asked every question, even ones with a single possible answer (games recorded since then).</summary>
        public bool AskedEveryChoice = true;
        /// <summary>-1 a draw, 0 or 1 the winner, -2 not finished.</summary>
        public int Winner = -2;
        public string Reason = "";
        public int Turns;
        public readonly List<List<int>>[] Answers = { new List<List<int>>(), new List<List<int>>() };
        public readonly List<string> LogLines = new List<string>();

        private readonly object gate = new object();

        public void AddAnswer(int seat, IList<int> answer)
        {
            if (seat < 0 || seat > 1 || answer == null) return;
            lock (gate) Answers[seat].Add(new List<int>(answer));
        }

        public int AnswerCount(int seat)
        {
            lock (gate) return Answers[seat].Count;
        }

        public static bool IsRecord(string text)
        {
            return text != null && text.TrimStart().StartsWith(Header, StringComparison.Ordinal);
        }

        /// <summary>Wraps a seat's agent so its answers go into the record (a person's seat stays interactive).</summary>
        public static IPlayerAgent Wrap(IPlayerAgent inner, GameRecord record, int seat)
        {
            var person = inner as IInteractiveAgent;
            if (person != null) return new InteractiveAnswerRecorder(person, record, seat);
            return new AnswerRecorder(inner, record, seat);
        }

        // ------------------------------------------------------------------ Text

        public string Serialize()
        {
            var sb = new StringBuilder();
            lock (gate)
            {
                sb.Append(Header).Append('\n');
                Line(sb, "when", When);
                Line(sb, "today", Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                Line(sb, "names", Names[0], Names[1]);
                Line(sb, "decknames", DeckNames[0], DeckNames[1]);
                Line(sb, "seed", Seed.ToString(CultureInfo.InvariantCulture));
                Line(sb, "prizes", PrizeCount.ToString(CultureInfo.InvariantCulture));
                Line(sb, "birthdays", Birthdays[0].ToString(CultureInfo.InvariantCulture), Birthdays[1].ToString(CultureInfo.InvariantCulture));
                Line(sb, "order", OrderChooser.ToString(CultureInfo.InvariantCulture), OrderChooserIsHigherSeed ? "1" : "0");
                Line(sb, "interactive", Interactive[0] ? "1" : "0", Interactive[1] ? "1" : "0");
                if (TieBreaker) Line(sb, "tiebreaker", "1");
                if (AskedEveryChoice) Line(sb, "asks", "1");
                if (Rules != null && !Rules.IsDefault) Line(sb, "rules", Rules.ToString());
                Line(sb, "result", Winner.ToString(CultureInfo.InvariantCulture), Turns.ToString(CultureInfo.InvariantCulture), Reason);
                for (int s = 0; s < 2; s++)
                    Line(sb, "deck" + s, string.Join(" ", Decks[s].Select(d => d.Key).ToArray()));
                // Answers in the order they were given, seat by seat (each seat is played back in its own order).
                for (int s = 0; s < 2; s++)
                    foreach (var a in Answers[s])
                        Line(sb, "a" + s, string.Join(",", a.Select(i => i.ToString(CultureInfo.InvariantCulture)).ToArray()));
                foreach (var l in LogLines) Line(sb, "log", l);
            }
            return sb.ToString();
        }

        private static void Line(StringBuilder sb, string tag, params string[] fields)
        {
            sb.Append(tag);
            foreach (var f in fields) sb.Append('\t').Append(Clean(f));
            sb.Append('\n');
        }

        private static string Clean(string s)
        {
            return string.IsNullOrEmpty(s) ? "" : s.Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }

        /// <summary>Reads a record. Cards that aren't in the pool are reported in problems (the record can't be played back then).</summary>
        public static GameRecord Parse(string text, CardDatabase db, List<string> problems)
        {
            var r = new GameRecord { AskedEveryChoice = false }; // older records don't say: their single-answer questions weren't asked
            foreach (var raw in (text ?? "").Replace("\r", "").Split('\n'))
            {
                if (raw.Length == 0 || raw[0] == '#') continue;
                var p = raw.Split('\t');
                try
                {
                    switch (p[0])
                    {
                        case "when": r.When = Field(p, 1); break;
                        case "today":
                            DateTime day;
                            if (DateTime.TryParseExact(Field(p, 1), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out day)) r.Today = day;
                            break;
                        case "names": r.Names = new[] { Field(p, 1), Field(p, 2) }; break;
                        case "decknames": r.DeckNames = new[] { Field(p, 1), Field(p, 2) }; break;
                        case "seed": r.Seed = Int(Field(p, 1)); break;
                        case "prizes": r.PrizeCount = Int(Field(p, 1)); break;
                        case "birthdays": r.Birthdays = new[] { Int(Field(p, 1)), Int(Field(p, 2)) }; break;
                        case "order":
                            r.OrderChooser = Int(Field(p, 1));
                            r.OrderChooserIsHigherSeed = Field(p, 2) == "1";
                            break;
                        case "interactive": r.Interactive = new[] { Field(p, 1) == "1", Field(p, 2) == "1" }; break;
                        case "tiebreaker": r.TieBreaker = Field(p, 1) == "1"; break;
                        case "asks": r.AskedEveryChoice = Field(p, 1) == "1"; break;
                        case "rules": r.Rules = GameRules.Parse(Field(p, 1)); break;
                        case "result":
                            r.Winner = Int(Field(p, 1));
                            r.Turns = Int(Field(p, 2));
                            r.Reason = Field(p, 3);
                            break;
                        case "deck0":
                        case "deck1":
                        {
                            int seat = p[0] == "deck0" ? 0 : 1;
                            foreach (var key in Field(p, 1).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var d = db.Get(key);
                                if (d == null)
                                {
                                    if (problems != null) problems.Add("Card " + key + " isn't in the card pool.");
                                    continue;
                                }
                                r.Decks[seat].Add(d);
                            }
                            break;
                        }
                        case "a0":
                        case "a1":
                        {
                            int seat = p[0] == "a0" ? 0 : 1;
                            var answer = new List<int>();
                            foreach (var part in Field(p, 1).Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)) answer.Add(Int(part));
                            r.Answers[seat].Add(answer);
                            break;
                        }
                        case "log": r.LogLines.Add(Field(p, 1)); break;
                    }
                }
                catch (FormatException)
                {
                    if (problems != null) problems.Add("Unreadable line: " + raw);
                }
                catch (OverflowException)
                {
                    if (problems != null) problems.Add("Unreadable line: " + raw);
                }
            }
            return r;
        }

        private static string Field(string[] p, int i)
        {
            return i < p.Length ? p[i] : "";
        }

        private static int Int(string s)
        {
            return int.Parse(s.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
    }

    /// <summary>Passes a bot's answers through and writes them into a GameRecord.</summary>
    public sealed class AnswerRecorder : IPlayerAgent
    {
        private readonly IPlayerAgent inner;
        private readonly GameRecord record;
        private readonly int seat;

        public AnswerRecorder(IPlayerAgent inner, GameRecord record, int seat)
        {
            this.inner = inner;
            this.record = record;
            this.seat = seat;
        }

        public IPlayerAgent Inner { get { return inner; } }

        public List<int> Decide(DecisionRequest request)
        {
            var answer = inner.Decide(request);
            if (answer != null) record.AddAnswer(seat, answer);
            return answer;
        }
    }

    /// <summary>The same for a person's seat (it stays interactive for the engine).</summary>
    public sealed class InteractiveAnswerRecorder : IInteractiveAgent
    {
        private readonly IInteractiveAgent inner;
        private readonly GameRecord record;
        private readonly int seat;

        public InteractiveAnswerRecorder(IInteractiveAgent inner, GameRecord record, int seat)
        {
            this.inner = inner;
            this.record = record;
            this.seat = seat;
        }

        public List<int> Decide(DecisionRequest request)
        {
            var answer = inner.Decide(request);
            if (answer != null) record.AddAnswer(seat, answer);
            return answer;
        }
    }

    /// <summary>Thrown when a played-back game runs out of answers or an answer no longer fits the question.</summary>
    public sealed class ReplayEndedException : Exception
    {
        public ReplayEndedException(string message) : base(message)
        {
        }
    }

    /// <summary>Plays one seat's recorded answers back, in order.</summary>
    public class ReplayAgent : IPlayerAgent
    {
        private readonly List<List<int>> answers;
        private int next;

        public ReplayAgent(List<List<int>> answers)
        {
            this.answers = answers ?? new List<List<int>>();
        }

        public int Used { get { return next; } }

        public List<int> Decide(DecisionRequest request)
        {
            // v128: cards shown with nothing to take (Done) need no answer; a game recorded before v128 didn't have this question
            // when an effect looked at cards and found nothing it could take, so it's answered without using up a recorded one.
            if (request.MaxChoices == 0 && request.Options.Count == 0 && (next >= answers.Count || answers[next].Count > 0)) return new List<int>();
            if (next >= answers.Count) throw new ReplayEndedException("The record ran out of answers after " + next + ".");
            var answer = answers[next];
            int count = request.Options.Count;
            int min = Math.Min(request.MinChoices, count);
            int max = Math.Min(request.MaxChoices, count);
            if (answer.Count < min || answer.Count > max || answer.Any(i => i < 0 || i >= count) || answer.Distinct().Count() != answer.Count)
                throw new ReplayEndedException("Answer " + (next + 1) + " doesn't fit the question \"" + request.Prompt + "\" any more (the cards may have changed since the game was played).");
            next++;
            return new List<int>(answer);
        }
    }

    /// <summary>The same for a seat a person played.</summary>
    public sealed class InteractiveReplayAgent : ReplayAgent, IInteractiveAgent
    {
        public InteractiveReplayAgent(List<List<int>> answers) : base(answers)
        {
        }
    }
}
