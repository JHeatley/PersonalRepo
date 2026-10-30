using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace PokemonTCG.AI
{
    /// <summary>
    /// v157: your Elo rating from the games you play against the AI (normal games and your Regional Sim games, your choice).
    /// Each game moves it two ways:
    /// 1. The result: standard Elo against the opponent's own rating (the classic bot and the learning AI each have one, and it
    ///    moves too, so a learning AI that gets stronger is worth more to beat). K is 40 for your first 10 games (placement),
    ///    then 20.
    /// 2. Your play: a quality score (0-100) from the Areas of Improvement review: how often your decisions matched the AI's,
    ///    the win chance its simulations say you gave up at key moments, and the habits it flagged. 60 is par; each point above
    ///    or below moves the rating a little (at most 8 a game, 12 while placing), whatever the result.
    /// Start 1500 (your choice). Saved as rating.txt in the AI folder. Touches no Unity API.
    /// </summary>
    public sealed class PlayerRating
    {
        public const double StartRating = 1500;
        public const int PlacementGames = 10;
        public const double PlacementK = 40, NormalK = 20, OpponentK = 16;
        public const double QualityPar = 60, QualitySpread = 40, QualityNudge = 8, PlacementQualityNudge = 12;
        public const string ClassicBot = "Classic bot", LearningAi = "Neural AI";
        /// <summary>The learning opponent's name in rating files from before the Learning and Hybrid AI were removed (read as LearningAi).</summary>
        private const string OldLearningAi = "Learning AI";

        private static string Renamed(string opponent)
        {
            return opponent == OldLearningAi ? LearningAi : opponent;
        }
        public const string FileName = "rating.txt";

        public double Rating = StartRating;
        public double Peak = StartRating;
        public int Games, Wins, Losses, Draws;
        /// <summary>The opponents' ratings (by kind: classic bot, learning AI).</summary>
        public readonly Dictionary<string, double> Opponents = new Dictionary<string, double>();
        /// <summary>Every rated game, oldest first.</summary>
        public readonly List<RatedGame> History = new List<RatedGame>();

        public bool Provisional { get { return Games < PlacementGames; } }

        public double OpponentRating(string opponent)
        {
            double r;
            return Opponents.TryGetValue(opponent ?? ClassicBot, out r) ? r : StartRating;
        }

        /// <summary>The chance the expected scorer with rating a beats rating b (standard Elo).</summary>
        public static double Expected(double a, double b)
        {
            return 1.0 / (1.0 + Math.Pow(10.0, (b - a) / 400.0));
        }

        /// <summary>
        /// The play-quality score (0-100) of a reviewed game, or -1 when there was no review (stopped early). 60% agreement with
        /// the AI's decisions, 40% from the win chance lost at the key moments (half a game's worth or more counts as all of it),
        /// minus 5 per habit flagged.
        /// </summary>
        public static double QualityOf(GameReview review)
        {
            if (review == null || review.DecisionsCompared <= 0) return -1;
            double agreement = 1.0 - (double)review.Differences / review.DecisionsCompared;
            double lost = review.Items.Where(i => i.Section == ReviewItem.BetterPlays).Sum(i => Math.Max(0, i.Gain));
            int habits = review.Items.Count(i => i.Section != ReviewItem.BetterPlays);
            double q = 60.0 * agreement + 40.0 * Math.Max(0, 1.0 - lost / 0.5) - 5.0 * habits;
            return Math.Max(0, Math.Min(100, q));
        }

        /// <summary>Rates one finished game. result: 1 win, 0.5 draw, 0 loss. review may be null (results only).</summary>
        public RatedGame Record(double result, string opponent, string yourDeck, string theirDeck, int turns, GameReview review, DateTime when)
        {
            opponent = string.IsNullOrEmpty(opponent) ? ClassicBot : opponent;
            double opp = OpponentRating(opponent);
            double expected = Expected(Rating, opp);
            double k = Provisional ? PlacementK : NormalK;
            double fromResult = k * (result - expected);
            double quality = QualityOf(review);
            double fromPlay = 0;
            if (quality >= 0)
            {
                double cap = Provisional ? PlacementQualityNudge : QualityNudge;
                fromPlay = Math.Max(-cap, Math.Min(cap, (quality - QualityPar) / QualitySpread * cap));
            }
            var g = new RatedGame
            {
                When = when,
                YourDeck = yourDeck ?? "",
                TheirDeck = theirDeck ?? "",
                Opponent = opponent,
                OpponentRating = opp,
                Result = result,
                Turns = turns,
                Quality = quality,
                Before = Rating,
                FromResult = fromResult,
                FromPlay = fromPlay
            };
            if (review != null)
            {
                g.Agreement = review.DecisionsCompared > 0 ? 1.0 - (double)review.Differences / review.DecisionsCompared : -1;
                g.ChanceLost = review.Items.Where(i => i.Section == ReviewItem.BetterPlays).Sum(i => Math.Max(0, i.Gain));
                g.KeyMistakes = review.Items.Count(i => i.Section == ReviewItem.BetterPlays);
                foreach (var item in review.Items.Where(i => i.Section != ReviewItem.BetterPlays)) g.Flags.Add(item.Title);
                foreach (var line in review.Summary)
                {
                    if (line.StartsWith("You were ahead in the Prize race")) g.Flags.Add(FlagLostLead);
                    if (line.StartsWith("You ran out of cards")) g.Flags.Add(FlagDeckedOut);
                    if (line.StartsWith("You ran out of Pokémon")) g.Flags.Add(FlagNoPokemon);
                }
            }
            Rating += fromResult + fromPlay;
            g.After = Rating;
            Peak = Math.Max(Peak, Rating);
            Opponents[opponent] = opp + OpponentK * ((1.0 - result) - (1.0 - expected));
            Games++;
            if (result >= 1) Wins++;
            else if (result <= 0) Losses++;
            else Draws++;
            History.Add(g);
            return g;
        }

        // ------------------------------------------------------------------ What it thinks

        public const string FlagLostLead = "Lost a Prize lead", FlagDeckedOut = "Ran out of cards", FlagNoPokemon = "Ran out of Pokémon in play";

        /// <summary>A rough description of the rating (it's measured against this game's AI, not real tournaments).</summary>
        public static string Level(double rating)
        {
            if (rating < 1200) return "Learning the basics";
            if (rating < 1400) return "Casual player";
            if (rating < 1550) return "League regular";
            if (rating < 1700) return "Strong local player";
            if (rating < 1850) return "Regional contender";
            if (rating < 2000) return "Top regional player";
            return "World-class against this AI";
        }

        /// <summary>The last n rated games (newest last).</summary>
        public List<RatedGame> Recent(int n)
        {
            return History.Skip(Math.Max(0, History.Count - n)).ToList();
        }

        /// <summary>Average play quality over the last n reviewed games, or -1.</summary>
        public double AverageQuality(int n)
        {
            var q = History.Where(g => g.Quality >= 0).Reverse().Take(n).Select(g => g.Quality).ToList();
            return q.Count == 0 ? -1 : q.Average();
        }

        /// <summary>
        /// What to work on, most important first: from your last 20 games, the habits flagged most often, how much win chance
        /// your key mistakes cost on average, how often you played differently from the AI, lost Prize leads and decks. Each line
        /// says how often and what to do about it.
        /// </summary>
        public List<string> Improvements(int maxItems = 4)
        {
            var recent = Recent(20);
            var reviewed = recent.Where(g => g.Quality >= 0).ToList();
            var items = new List<KeyValuePair<double, string>>();
            if (reviewed.Count == 0)
            {
                if (recent.Count == 0) return new List<string> { "Play a game against the AI: your rating and what to work on start from your first game." };
                return new List<string> { "Let the Areas of Improvement review finish after a game (it takes up to half a minute): what to work on comes from it." };
            }
            int n = reviewed.Count;
            Action<string, string> habit = (flag, advice) =>
            {
                int times = reviewed.Count(g => g.Flags.Contains(flag));
                if (times == 0) return;
                double share = (double)times / n;
                items.Add(new KeyValuePair<double, string>(share, flag + ": in " + times + " of your last " + n + " games. " + advice));
            };
            const string energy = "Attach an Energy every turn you can, even to a Benched Pokémon, so your next attacker is ready on time.";
            const string supporter = "Play a Supporter every turn unless you're saving it on purpose; draw and search keep your turns going.";
            const string bench = "Keep at least two Benched Pokémon: a Knock Out with nothing to promote loses the game, and a deeper Bench gives you options.";
            const string attack = "Attack whenever it doesn't cost you anything: chip damage sets up Knock Outs later.";
            // The review's habits (older games used the first four names).
            var known = new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "Missed Energy attachments", energy }, { "Turns without a Supporter", supporter }, { "A thin Bench", bench }, { "Turns without an attack", attack },
                { "Ending your turn without attaching an Energy", energy }, { "Attacking before you attach an Energy", energy },
                { "Ending your turn without playing a Supporter", supporter }, { "Attacking before you play your Supporter", supporter },
                { "Ending your turn without attacking", attack }
            };
            var summaryFlags = new HashSet<string> { FlagLostLead, FlagDeckedOut, FlagNoPokemon };
            foreach (var flag in reviewed.SelectMany(g => g.Flags).Where(f => !summaryFlags.Contains(f)).Distinct())
            {
                string advice;
                habit(flag, known.TryGetValue(flag, out advice) ? advice : "The review's notes after each game say when it came up and what the AI would do instead.");
            }
            habit(FlagLostLead, "You were ahead in the Prize race and lost it: plan the last Prizes, keep a backup attacker and don't give up easy Knock Outs.");
            habit(FlagDeckedOut, "Watch how many cards your draw and search use up late in the game.");
            habit(FlagNoPokemon, "Keep a Benched Pokémon ready to replace your Active Pokémon.");

            double lost = reviewed.Average(g => g.ChanceLost);
            if (lost >= 0.05)
                items.Add(new KeyValuePair<double, string>(Math.Min(1.0, lost * 4),
                    "Key moments: the AI's simulations say your biggest mistakes cost about " + Math.Round(lost * 100) + " points of win chance a game. Check \"Plays that would have given you a better chance\" after each game and use Show me."));
            var agreeing = reviewed.Where(g => g.Agreement >= 0).ToList();
            if (agreeing.Count > 0)
            {
                double agree = agreeing.Average(g => g.Agreement);
                if (agree < 0.7)
                    items.Add(new KeyValuePair<double, string>(0.7 - agree + 0.2,
                        "Decision-making: you made the AI's choice in " + Math.Round(agree * 100) + "% of your decisions. The AI isn't always right, but where it differs, look at the order of your turn: draw and search first, attach, then attack."));
            }
            var decks = recent.GroupBy(g => g.YourDeck).Where(x => x.Count() >= 3).Select(x => new { Deck = x.Key, Games = x.Count(), Score = x.Average(g => g.Result) })
                              .Where(x => x.Score < 0.4).OrderBy(x => x.Score).ToList();
            foreach (var d in decks.Take(1))
                items.Add(new KeyValuePair<double, string>(0.4 - d.Score + 0.1,
                    "Deck: " + d.Deck + " has won " + Math.Round(d.Score * 100) + "% of your last " + d.Games + " games with it. Try another list or practise its matchups."));

            var result = items.OrderByDescending(x => x.Key).Take(maxItems).Select(x => x.Value).ToList();
            if (result.Count == 0) result.Add("Nothing stands out in your last " + n + " games: keep it up, and look at the close calls in each review.");
            return result;
        }

        /// <summary>A trend line: average quality of your last 5 reviewed games against the 5 before them.</summary>
        public string Trend()
        {
            var q = History.Where(g => g.Quality >= 0).Select(g => g.Quality).ToList();
            if (q.Count < 6) return "";
            double last = q.Skip(q.Count - 5).Average();
            double before = q.Skip(Math.Max(0, q.Count - 10)).Take(Math.Min(5, q.Count - 5)).Average();
            double diff = last - before;
            if (Math.Abs(diff) < 3) return "Your play quality has been steady.";
            return diff > 0 ? "Your play quality is improving (+" + Math.Round(diff) + " over your last 5 games)." : "Your play quality dipped (" + Math.Round(diff) + " over your last 5 games).";
        }

        // ------------------------------------------------------------------ Saving

        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        public string Serialize()
        {
            var sb = new StringBuilder();
            sb.Append("# Your Elo rating (v157). One game per G line.\n");
            sb.Append("rating=").Append(Rating.ToString("R", Inv)).Append('\n');
            sb.Append("peak=").Append(Peak.ToString("R", Inv)).Append('\n');
            sb.Append("games=").Append(Games).Append(" wins=").Append(Wins).Append(" losses=").Append(Losses).Append(" draws=").Append(Draws).Append('\n');
            foreach (var kv in Opponents) sb.Append("opponent=").Append(Clean(kv.Key)).Append('\t').Append(kv.Value.ToString("R", Inv)).Append('\n');
            foreach (var g in History)
            {
                sb.Append("G\t").Append(g.When.ToString("yyyy-MM-dd HH:mm", Inv))
                  .Append('\t').Append(Clean(g.YourDeck)).Append('\t').Append(Clean(g.TheirDeck)).Append('\t').Append(Clean(g.Opponent))
                  .Append('\t').Append(N(g.OpponentRating)).Append('\t').Append(N(g.Result)).Append('\t').Append(g.Turns)
                  .Append('\t').Append(N(g.Quality)).Append('\t').Append(N(g.Agreement)).Append('\t').Append(N(g.ChanceLost)).Append('\t').Append(g.KeyMistakes)
                  .Append('\t').Append(N(g.Before)).Append('\t').Append(N(g.FromResult)).Append('\t').Append(N(g.FromPlay)).Append('\t').Append(N(g.After))
                  .Append('\t').Append(string.Join("|", g.Flags.Select(Clean).ToArray())).Append('\n');
            }
            return sb.ToString();
        }

        private static string N(double v) { return v.ToString("0.####", Inv); }

        private static string Clean(string s) { return (s ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ').Replace('|', '/'); }

        private static double D(string s, double fallback)
        {
            double v;
            return double.TryParse(s, NumberStyles.Float, Inv, out v) ? v : fallback;
        }

        public static PlayerRating Parse(string text)
        {
            var r = new PlayerRating();
            if (string.IsNullOrEmpty(text)) return r;
            foreach (var raw in text.Split('\n'))
            {
                string line = raw.TrimEnd('\r');
                if (line.Length == 0 || line.StartsWith("#")) continue;
                if (line.StartsWith("rating=")) r.Rating = D(line.Substring(7), StartRating);
                else if (line.StartsWith("peak=")) r.Peak = D(line.Substring(5), StartRating);
                else if (line.StartsWith("games="))
                {
                    foreach (var part in line.Split(' '))
                    {
                        int eq = part.IndexOf('=');
                        if (eq < 0) continue;
                        int v;
                        if (!int.TryParse(part.Substring(eq + 1), NumberStyles.Integer, Inv, out v)) continue;
                        switch (part.Substring(0, eq))
                        {
                            case "games": r.Games = v; break;
                            case "wins": r.Wins = v; break;
                            case "losses": r.Losses = v; break;
                            case "draws": r.Draws = v; break;
                        }
                    }
                }
                else if (line.StartsWith("opponent="))
                {
                    var p = line.Substring(9).Split('\t');
                    if (p.Length >= 2) r.Opponents[Renamed(p[0])] = D(p[1], StartRating);
                }
                else if (line.StartsWith("G\t"))
                {
                    var p = line.Split('\t');
                    if (p.Length < 17) continue;
                    DateTime when;
                    DateTime.TryParseExact(p[1], "yyyy-MM-dd HH:mm", Inv, DateTimeStyles.None, out when);
                    int turns, mistakes;
                    int.TryParse(p[7], NumberStyles.Integer, Inv, out turns);
                    int.TryParse(p[11], NumberStyles.Integer, Inv, out mistakes);
                    var g = new RatedGame
                    {
                        When = when,
                        YourDeck = p[2],
                        TheirDeck = p[3],
                        Opponent = Renamed(p[4]),
                        OpponentRating = D(p[5], StartRating),
                        Result = D(p[6], 0),
                        Turns = turns,
                        Quality = D(p[8], -1),
                        Agreement = D(p[9], -1),
                        ChanceLost = D(p[10], 0),
                        KeyMistakes = mistakes,
                        Before = D(p[12], StartRating),
                        FromResult = D(p[13], 0),
                        FromPlay = D(p[14], 0),
                        After = D(p[15], StartRating)
                    };
                    if (p[16].Length > 0) g.Flags.AddRange(p[16].Split('|'));
                    r.History.Add(g);
                }
            }
            return r;
        }

        public static PlayerRating Load(string path)
        {
            SafeFile.Recover(path);
            try { return File.Exists(path) ? Parse(File.ReadAllText(path, Encoding.UTF8)) : new PlayerRating(); }
            catch (Exception) { return new PlayerRating(); }
        }

        public void Save(string path)
        {
            SafeFile.WriteText(path, Serialize());
        }
    }

    /// <summary>One rated game.</summary>
    public sealed class RatedGame
    {
        public DateTime When;
        public string YourDeck = "", TheirDeck = "", Opponent = "";
        public double OpponentRating;
        /// <summary>1 win, 0.5 draw, 0 loss.</summary>
        public double Result;
        public int Turns;
        /// <summary>Play quality 0-100, or -1 without a review.</summary>
        public double Quality = -1;
        /// <summary>Share of your decisions the AI would also have made, or -1.</summary>
        public double Agreement = -1;
        /// <summary>Win chance the review's simulations say your key mistakes cost (summed).</summary>
        public double ChanceLost;
        public int KeyMistakes;
        public double Before, FromResult, FromPlay, After;
        /// <summary>The habits and problems the review flagged.</summary>
        public readonly List<string> Flags = new List<string>();

        public double Change { get { return After - Before; } }

        public string ResultText { get { return Result >= 1 ? "Win" : Result <= 0 ? "Loss" : "Draw"; } }

        /// <summary>"1500 → 1528 (+28: result +24, play +4)".</summary>
        public string ChangeText
        {
            get
            {
                string s = Math.Round(Before) + " → " + Math.Round(After) + " (" + Signed(Change) + ": result " + Signed(FromResult);
                s += Quality >= 0 ? ", play " + Signed(FromPlay) + ")" : ", play not reviewed)";
                return s;
            }
        }

        public static string Signed(double v)
        {
            long r = (long)Math.Round(v);
            return r > 0 ? "+" + r : r.ToString(CultureInfo.InvariantCulture);
        }
    }
}
