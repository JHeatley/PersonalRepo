using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One player at a Regional Sim event: you, or one of the learning AI's seats.</summary>
    public sealed class RegionalEntrant
    {
        public int Id;
        public string Name = "";
        public bool Human;
        /// <summary>A person playing from another computer (a friend who joined the host's online game: AddGuest).</summary>
        public bool Remote;
        public string Deck = "";

        public RegionalEntrant Copy()
        {
            return new RegionalEntrant { Id = Id, Name = Name, Human = Human, Remote = Remote, Deck = Deck };
        }
    }

    /// <summary>
    /// One match of one round. Game results are from A's side: 0 = A won, 1 = B won, -1 = a draw (the turn limit, or an
    /// engine error). When you play, you are always A. B is -1 for a bye.
    /// </summary>
    public sealed class RegionalMatch
    {
        public int Round;
        public int Table;
        public int A;
        public int B = -1;
        public List<int> Games = new List<int>();
        public bool Finished;
        /// <summary>An AI table being played right now (not saved).</summary>
        public bool InProgress;
        /// <summary>A top cut match (v70): A's and B's Swiss seeds (1 = first after Swiss). 0 for a Swiss match.</summary>
        public int SeedA, SeedB;
        /// <summary>v127: the player who dropped during this match (it's a win for the other one), or -1.</summary>
        public int ConcededBy = -1;
        /// <summary>
        /// v127: a play-in match of an asymmetrical top cut (players on the bubble's match points play for the last seats). It's
        /// played like a top cut match (SeedA and SeedB are Swiss seeds) and doesn't change the Swiss standings.
        /// </summary>
        public bool PlayIn;

        public bool IsBye { get { return B < 0; } }

        /// <summary>A single-elimination match of the top cut (v70).</summary>
        public bool IsCut { get { return SeedA > 0; } }

        public int WinsA { get { return Games.Count(g => g == 0); } }

        public int WinsB { get { return Games.Count(g => g == 1); } }

        /// <summary>
        /// 0 = A won the match, 1 = B won it, -1 = a tie. A bye is a win for A. A top cut match has no ties: one still level
        /// when its games run out goes to the higher seed.
        /// </summary>
        public int Result
        {
            get
            {
                if (IsBye) return 0;
                if (ConcededBy >= 0) return ConcededBy == A ? 1 : 0;
                int a = WinsA, b = WinsB;
                if (a == b && IsCut && Finished) return SeedA < SeedB ? 0 : 1;
                return a > b ? 0 : b > a ? 1 : -1;
            }
        }

        /// <summary>"2-1" from A's side (draws in brackets).</summary>
        public string Score
        {
            get
            {
                int draws = Games.Count(g => g < 0);
                return WinsA + "-" + WinsB + (draws > 0 ? " (" + draws + " drawn)" : "") + (ConcededBy >= 0 ? " (a player dropped)" : "");
            }
        }

        public RegionalMatch Copy()
        {
            return new RegionalMatch { Round = Round, Table = Table, A = A, B = B, Games = new List<int>(Games), Finished = Finished, InProgress = InProgress, SeedA = SeedA, SeedB = SeedB, ConcededBy = ConcededBy, PlayIn = PlayIn };
        }
    }

    /// <summary>One line of the standings.</summary>
    public sealed class RegionalStanding
    {
        public int Rank;
        public int Id;
        public string Name = "";
        public string Deck = "";
        public bool Human;
        public int Wins, Losses, Ties, Byes;
        public int GameWins, GameLosses;
        /// <summary>Match win percentage (match points / 3 per match played, byes included), at least 25%.</summary>
        public double WinPct;
        /// <summary>Opponents' win percentage (byes aren't opponents): the first tiebreaker.</summary>
        public double OppWinPct;
        /// <summary>Opponents' opponents' win percentage: the second tiebreaker.</summary>
        public double OppOppWinPct;

        /// <summary>v127: dropped from the event (still ranked by their record).</summary>
        public bool Dropped;
        /// <summary>v127, an event with a Day 2: made it (true once Day 1 is over), or missed it (MissedDay2).</summary>
        public bool Day2, MissedDay2;

        /// <summary>Win 3, tie 1, loss 0.</summary>
        public int Points { get { return Wins * 3 + Ties; } }

        public string Record { get { return Wins + "-" + Losses + "-" + Ties; } }
    }

    /// <summary>How one deck did across the event.</summary>
    public sealed class RegionalDeckStats
    {
        public string Deck = "";
        public int Players;
        public int Wins, Losses, Ties;
        public int BestRank = int.MaxValue;

        public double WinRate
        {
            get
            {
                int played = Wins + Losses + Ties;
                return played == 0 ? 0 : (Wins + 0.5 * Ties) / played;
            }
        }
    }

    /// <summary>A consistent copy of the event for the screen (taken under the lock, then read freely).</summary>
    public sealed class RegionalView
    {
        public int Version;
        /// <summary>Round: the round being played (Swiss rounds first, then the top cut's). Rounds: the Swiss rounds.</summary>
        public int Round, Rounds, Players;
        public bool BestOfThree;
        /// <summary>The top cut (v70): 0 (none), 2, 4 or 8 players, its matches best of 1 or 3, and how many rounds it has.</summary>
        public int TopCut, CutRounds;
        public bool CutBestOfThree;
        /// <summary>The top cut's players by seed (Swiss finish), empty until Swiss is over.</summary>
        public List<int> CutSeeds = new List<int>();
        /// <summary>Each player's place by Id: the top cut decides the first places, Swiss standings the rest.</summary>
        public int[] Placement = new int[0];
        /// <summary>The winner (the top cut's, or first after Swiss), or -1 while the event is on.</summary>
        public int Champion = -1;

        public bool InTopCut { get { return Round > Rounds; } }
        /// <summary>v127, Day 2: the rounds of Day 1 (0: a one-day event), the match points needed, and whether Day 1 is over.</summary>
        public int Day1Rounds, Day2Points;
        public bool Day2Decided;
        /// <summary>How many players made Day 2 (once it's decided).</summary>
        public int Day2Players;
        /// <summary>v127: you dropped (and after which round), and you're still playing (not dropped, out of Day 2 or out of the top cut).</summary>
        public bool YouDropped, YouStillIn;
        public int YouDroppedAfterRound;
        public bool IsDay2Round { get { return Day1Rounds > 0 && Round > Day1Rounds && Round <= Rounds; } }
        /// <summary>
        /// v127, asymmetrical top cut: on (a setting), the play-in rounds played so far (they come between Swiss and the top cut),
        /// and whether the play-in is being played now. PlayInPool: the players on the bubble's match points, by Swiss seed;
        /// PlayInAbove: the players above them (in the top cut already); PlayInSeats: the seats the pool plays for.
        /// </summary>
        public bool AsymmetricCut;
        public int PlayInRounds, PlayInSeats, PlayInPoints;
        public bool InPlayIn;
        public List<int> PlayInPool = new List<int>();
        public List<int> PlayInAbove = new List<int>();
        /// <summary>The first round of the top cut proper (after Swiss and the play-in).</summary>
        public int CutStart { get { return Rounds + PlayInRounds; } }
        public bool YouPlaying;
        public bool AiExplore;
        /// <summary>How the AI players play: the Neural AI.</summary>
        public AiStyle PlayStyle = AiStyle.Neural;
        public string Format = "";
        public bool Finished;
        public List<RegionalStanding> Standings = new List<RegionalStanding>();
        public List<RegionalMatch> Matches = new List<RegionalMatch>();
        public List<RegionalDeckStats> Decks = new List<RegionalDeckStats>();
        /// <summary>Your match this round (finished or not, a bye included), or null.</summary>
        public RegionalMatch YourMatch;
        /// <summary>AI only: the table of this round picked to watch on the board (finished or not), or null.</summary>
        public RegionalMatch Featured;
        public bool Watching;
        public RegionalStanding You;
        public int RoundMatches, RoundFinished, RoundPlaying;
        public int AiGames, AiErrors;
        public string LastError;
        /// <summary>Names by entrant Id.</summary>
        public string[] Names = new string[0];
        public string[] DeckNames = new string[0];

        public RegionalStanding StandingOf(int id)
        {
            foreach (var s in Standings) if (s.Id == id) return s;
            return null;
        }
    }

    /// <summary>
    /// Regional Sim (v57): a Swiss event of learning AIs, with you in it or not (AI only). Each AI seat plays a deck picked
    /// at random from the legal decks of the event's format.
    ///
    ///  - Matches are best of 1 or best of 3. In a best of 3 the loser of the previous game chooses to go first or second.
    ///  - Round 1 is paired at random; later rounds pair players with the same record (or the closest one), without rematches
    ///    when that's possible. With an odd number of players the lowest-ranked player without a bye yet gets one (a win).
    ///  - Standings: match points (win 3, tie 1), then opponents' win percentage, then opponents' opponents' win percentage
    ///    (each player's win percentage is at least 25%).
    ///  - The AI tables are played on background threads while you play yours; the next round is paired only when every
    ///    match of the current round is over. Every AI game teaches the brain, like the Train screen does.
    ///
    /// Thread-safe: the workers, your games and the screen all go through one lock.
    /// </summary>
    public sealed class RegionalSim
    {
        public const string FileHeader = "PokemonTCG Regional Sim 1";
        /// <summary>The smallest event that fires: 6 players in all, you included (your rule, v57.1).</summary>
        public const int MinPlayers = 6;
        /// <summary>The largest: 10,000 players in all (v69; 4,000 before).</summary>
        public const int MaxPlayers = 10000;
        /// <summary>
        /// The most AI tables played at once in the background (v71: your setting, 1 to 64; v69 played 15). Past the processor's
        /// threads more tables don't finish a round sooner, and hundreds of threads would only cost memory and switching.
        /// </summary>
        public const int MaxTablesAtOnce = 64;
        private const int MaxPairingSteps = 200000;

        private readonly object gate = new object();
        private readonly List<RegionalEntrant> entrants = new List<RegionalEntrant>();
        private readonly List<RegionalMatch> matches = new List<RegionalMatch>();
        private readonly Dictionary<string, string> deckTexts = new Dictionary<string, string>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<CardDef>> deckCards = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        /// <summary>The threads playing the AI tables. A thread taken out of the list stops after the game it's playing.</summary>
        private readonly List<Thread> workers = new List<Thread>();
        private volatile bool stopping;
        private int tablesAtOnce = 1;
        private int workersStarted;
        private int version;
        private int aiGames;
        private int aiErrors;
        private string lastError;

        private int seed;
        private int rounds;
        private int round;
        private bool bestOfThree;
        /// <summary>Top cut (v70): 0, 2, 4 or 8 players after the Swiss rounds, single elimination.</summary>
        private int topCut;
        private bool cutBestOfThree;
        /// <summary>The top cut's players by seed, set when Swiss is over.</summary>
        private readonly List<int> cutSeeds = new List<int>();
        /// <summary>A top cut match still level after this many games (a drawn game is followed by a Tie-Breaker game) goes to the higher seed.</summary>
        public const int MaxCutGames = 9;
        private string format = "Standard";
        private bool aiExplore = true;
        /// <summary>How every AI player of this event plays (v117; saved with the event): the Neural AI, for events saved with another style too.</summary>
        private AiStyle aiStyle = AiStyle.Neural;
        private bool finished;
        /// <summary>AI only: each round one table is played on the board for you to watch instead of in the background.</summary>
        private bool watchOne;
        /// <summary>The table of the current round picked to watch, or -1.</summary>
        private int featuredTable = -1;
        /// <summary>v127, Day 2: Day 1's rounds (0: a one-day event; rounds is Day 1 and Day 2 together) and the match points to make Day 2.</summary>
        private int day1Rounds;
        private int day2Points;
        private bool day2Decided;
        private readonly HashSet<int> day2Players = new HashSet<int>();
        /// <summary>v127: players who dropped, and the last round they were in.</summary>
        private readonly Dictionary<int, int> dropped = new Dictionary<int, int>();
        /// <summary>
        /// v127, asymmetrical top cut (your setting): when Swiss is over, everyone on the same match points as the last top cut
        /// seat plays single elimination for the seats those points hold (byes to the better seeds), before the top cut.
        /// </summary>
        private bool asymmetricCut;
        private readonly List<int> playInPool = new List<int>();
        private readonly List<int> playInAbove = new List<int>();
        private int playInSeats;
        private int playInRounds;
        /// <summary>Index in matches of the current round's first match (matches are stored round by round; v57.1).</summary>
        private int roundStart;
        /// <summary>Matches of the current round still being played (byes don't count).</summary>
        private int roundOpen;

        /// <summary>The brain every AI seat plays with and teaches (null: they play like the classic bot and learn nothing).</summary>
        public LearningBrain Brain;
        /// <summary>Makes the card scripts for a worker thread (each worker gets its own).</summary>
        public Func<CardScriptRegistry> ScriptsFactory;

        public int Seed { get { lock (gate) return seed; } }
        /// <summary>The Swiss rounds (the top cut's rounds come after them).</summary>
        public int Rounds { get { lock (gate) return rounds; } }
        public int TopCut { get { lock (gate) return topCut; } }
        public bool CutBestOfThree { get { lock (gate) return cutBestOfThree; } }

        /// <summary>Rounds of single elimination for a top cut of this size: 3 for a top 8, 2 for a top 4, 1 for a top 2.</summary>
        public static int CutRoundsFor(int cut)
        {
            return cut >= 8 ? 3 : cut >= 4 ? 2 : cut >= 2 ? 1 : 0;
        }

        /// <summary>"Round 3", or the top cut's rounds: "Top 8", "Top 4", "Final".</summary>
        public static string RoundName(int roundNumber, int swissRounds, int cut)
        {
            return RoundName(roundNumber, swissRounds, cut, 0);
        }

        /// <summary>v127: with an asymmetrical top cut's play-in rounds between Swiss and the top cut ("Play-in 1").</summary>
        public static string RoundName(int roundNumber, int swissRounds, int cut, int playIn)
        {
            if (roundNumber <= swissRounds || cut < 2) return "Round " + roundNumber;
            if (roundNumber <= swissRounds + playIn) return "Play-in " + (roundNumber - swissRounds);
            int left = cut >> (roundNumber - swissRounds - playIn - 1);
            return left <= 2 ? "Final" : "Top " + left;
        }

        /// <summary>
        /// Adds a top cut (v70): after the Swiss rounds the top 2, 4 or 8 play single elimination, seeded 1 v 8, 4 v 5, 2 v 7,
        /// 3 v 6. Call after Create, before Begin. size 0 means none.
        /// </summary>
        public void SetTopCut(int size, bool bestOf3)
        {
            lock (gate)
            {
                if (round > 0) throw new InvalidOperationException("The top cut is set before the event starts.");
                if (size != 0 && size != 2 && size != 4 && size != 8) throw new ArgumentException("A top cut is 2, 4 or 8 players.");
                if (size > entrants.Count) throw new ArgumentException("A top " + size + " needs at least " + size + " players.");
                topCut = size;
                cutBestOfThree = bestOf3;
            }
        }
        /// <summary>
        /// v127: splits the event into 2 days. Day 1 (every player) has day1RoundsCount rounds; players with at least points match
        /// points play Day 2's day2RoundsCount more rounds (their records carry over), and the top cut comes after Day 2. Players
        /// who miss Day 2 place below everyone who made it, by their Day 1 record. Call after Create, before Begin.
        /// </summary>
        public void SetDayTwo(int day1RoundsCount, int day2RoundsCount, int points)
        {
            lock (gate)
            {
                if (round > 0) throw new InvalidOperationException("Day 2 is set before the event starts.");
                if (day1RoundsCount < 1 || day2RoundsCount < 1) throw new ArgumentException("Each day needs at least 1 round.");
                day1Rounds = day1RoundsCount;
                rounds = day1RoundsCount + day2RoundsCount;
                day2Points = Math.Max(0, points);
            }
        }

        /// <summary>
        /// v127: an asymmetrical top cut. When Swiss is over, every player on the same match points as the last top cut seat
        /// (inside the cut by tiebreakers or not) plays single elimination for the seats those points hold; the better Swiss
        /// seeds get the byes. Matches are like the top cut's (its best of 1 or 3). The winners take the seats by Swiss seed.
        /// Call after Create, before Begin.
        /// </summary>
        public void SetAsymmetricCut(bool on)
        {
            lock (gate)
            {
                if (round > 0) throw new InvalidOperationException("The top cut is set before the event starts.");
                asymmetricCut = on;
            }
        }

        public bool AsymmetricCut { get { lock (gate) return asymmetricCut; } }

        /// <summary>The play-in rounds played so far (between Swiss and the top cut).</summary>
        public int PlayInRounds { get { lock (gate) return playInRounds; } }

        /// <summary>Day 1's rounds (0: a one-day event).</summary>
        public int Day1Rounds { get { lock (gate) return day1Rounds; } }
        public int Day2Points { get { lock (gate) return day2Points; } }

        /// <summary>A player who still plays Swiss rounds: not dropped, and in Day 2 once Day 1 is over.</summary>
        private bool ActiveLocked(int id)
        {
            if (dropped.ContainsKey(id)) return false;
            return !day2Decided || day2Players.Contains(id);
        }

        /// <summary>
        /// v127: you drop from the event. A match of yours still being played is a win for your opponent; you aren't paired again,
        /// and the rest of the event plays out without you. You keep your place by your record. Returns true when you dropped.
        /// </summary>
        public bool DropYou()
        {
            lock (gate)
            {
                var you = YouLocked();
                if (you == null || finished || dropped.ContainsKey(you.Id)) return false;
                dropped[you.Id] = round;
                version++;
                var m = YourMatchLocked();
                if (m != null && !m.Finished && !m.IsBye)
                {
                    m.ConcededBy = you.Id;
                    CompleteMatchLocked(m);
                }
                Monitor.PulseAll(gate);
                return true;
            }
        }

        /// <summary>You dropped from the event.</summary>
        public bool YouDropped
        {
            get
            {
                lock (gate)
                {
                    var you = YouLocked();
                    return you != null && dropped.ContainsKey(you.Id);
                }
            }
        }

        public int Round { get { lock (gate) return round; } }
        public bool BestOfThree { get { lock (gate) return bestOfThree; } }
        public string Format { get { lock (gate) return format; } }
        public bool Finished { get { lock (gate) return finished; } }
        public int Version { get { lock (gate) return version; } }
        public int AiGamesPlayed { get { lock (gate) return aiGames; } }
        public int PlayerCount { get { lock (gate) return entrants.Count; } }

        /// <summary>You have a seat at this event (false: AI only).</summary>
        public bool YouPlaying
        {
            get { lock (gate) return entrants.Any(e => e.Human && !e.Remote); }
        }

        /// <summary>AI games explore (try other plays now and then, like training) so the brain learns faster.</summary>
        public bool AiExplore
        {
            get { lock (gate) return aiExplore; }
        }

        /// <summary>
        /// How the AI players play (v117): the learning AI's feature weights, its neural network, or both. It can change during the
        /// event; games already being played finish the way they started. Every game still teaches both halves of the brain.
        /// </summary>
        public AiStyle PlayStyle
        {
            get { lock (gate) return aiStyle; }
            set
            {
                lock (gate)
                {
                    if (aiStyle == value) return;
                    aiStyle = value;
                    version++;
                }
            }
        }

        // ------------------------------------------------------------------ Creating an event

        /// <summary>
        /// A new event. youDeck: the deck you play, or null for an AI-only event. aiSeats: how many AI players. aiDecks: the
        /// decks the AI seats pick from at random. deckTextsByName: the list of every deck used (saved with the event).
        /// </summary>
        public static RegionalSim Create(int aiSeats, int rounds, bool bestOfThree, string format, string youDeck, IList<string> aiDecks,
                                         IDictionary<string, string> deckTextsByName, bool aiExplore, int seed, bool watchOneMatch = false)
        {
            return Create(aiSeats, rounds, bestOfThree, format, youDeck, aiDecks, null, true, deckTextsByName, aiExplore, seed, watchOneMatch);
        }

        /// <summary>
        /// A new event with the field you chose (v69): chosenDecks says how many AI players play each of those decks. The
        /// other AI seats pick at random from aiDecks when fillRandom is on; with it off, the chosen numbers must add up to
        /// aiSeats. Which AI player gets which deck is shuffled.
        /// </summary>
        public static RegionalSim Create(int aiSeats, int rounds, bool bestOfThree, string format, string youDeck, IList<string> aiDecks,
                                         IList<KeyValuePair<string, int>> chosenDecks, bool fillRandom,
                                         IDictionary<string, string> deckTextsByName, bool aiExplore, int seed, bool watchOneMatch = false)
        {
            int chosen = 0;
            if (chosenDecks != null)
                foreach (var kv in chosenDecks)
                {
                    if (kv.Value < 0) throw new ArgumentException("A deck can't be played by fewer than 0 AI players.");
                    if (kv.Value > 0 && deckTextsByName != null && !deckTextsByName.ContainsKey(kv.Key))
                        throw new ArgumentException("The deck \"" + kv.Key + "\" isn't one of the event's decks.");
                    chosen += kv.Value;
                }
            if (chosen > aiSeats)
                throw new ArgumentException("The decks you chose are for " + chosen.ToString("N0", CultureInfo.InvariantCulture) + " AI players, but there are only "
                                            + aiSeats.ToString("N0", CultureInfo.InvariantCulture) + ".");
            int randomSeats = aiSeats - chosen;
            if (randomSeats > 0 && !fillRandom)
                throw new ArgumentException("The decks you chose cover " + chosen.ToString("N0", CultureInfo.InvariantCulture) + " of the "
                                            + aiSeats.ToString("N0", CultureInfo.InvariantCulture) + " AI players. Choose decks for all of them, or let the rest be picked at random.");
            if (randomSeats > 0 && (aiDecks == null || aiDecks.Count == 0)) throw new ArgumentException("The AI players need at least one deck to pick from.");
            int total = aiSeats + (youDeck != null ? 1 : 0);
            if (total < MinPlayers) throw new ArgumentException("An event needs at least " + MinPlayers + " players.");
            if (total > MaxPlayers) throw new ArgumentException("An event can have at most " + MaxPlayers.ToString("N0", CultureInfo.InvariantCulture) + " players.");
            var sim = new RegionalSim();
            sim.seed = seed & 0x0FFFFFFF;
            sim.rounds = Math.Max(1, rounds);
            sim.bestOfThree = bestOfThree;
            sim.format = string.IsNullOrEmpty(format) ? "Standard" : format;
            sim.aiExplore = aiExplore;
            sim.watchOne = watchOneMatch && youDeck == null;
            if (deckTextsByName != null)
                foreach (var kv in deckTextsByName) sim.deckTexts[kv.Key] = kv.Value ?? "";
            var pick = new Rng(sim.seed);
            if (youDeck != null) sim.entrants.Add(new RegionalEntrant { Id = 0, Name = "You", Human = true, Deck = youDeck });
            // The chosen decks, then a random pick for each other seat (null), in a shuffled order. With nothing chosen the
            // picks are made in seat order, as before, so an event's seed still gives the same field.
            var seatDecks = new List<string>();
            if (chosenDecks != null)
                foreach (var kv in chosenDecks)
                    for (int k = 0; k < kv.Value; k++) seatDecks.Add(kv.Key);
            for (int k = 0; k < randomSeats; k++) seatDecks.Add(null);
            if (chosen > 0) pick.Shuffle(seatDecks);
            for (int i = 1; i <= aiSeats; i++)
                sim.entrants.Add(new RegionalEntrant { Id = sim.entrants.Count, Name = "AI " + i, Deck = seatDecks[i - 1] ?? aiDecks[pick.Next(aiDecks.Count)] });
            return sim;
        }

        /// <summary>
        /// AI players per deck from meta shares (v70): percent of the AI players for each deck, rounded so the numbers add up to
        /// the shares' total (the largest remainders get the seats left over). Decks with no seat are left out.
        /// </summary>
        public static List<KeyValuePair<string, int>> SeatsFromShares(IList<KeyValuePair<string, double>> percents, int aiSeats)
        {
            var result = new List<KeyValuePair<string, int>>();
            if (percents == null || percents.Count == 0 || aiSeats <= 0) return result;
            double total = 0;
            foreach (var share in percents) total += Math.Max(0, share.Value);
            int target = Math.Min(aiSeats, (int)Math.Round(Math.Min(100.0, total) * aiSeats / 100.0, MidpointRounding.AwayFromZero));
            var exact = percents.Select(p => Math.Max(0, p.Value) * aiSeats / 100.0).ToList();
            var seats = exact.Select(x => (int)Math.Floor(x)).ToList();
            int left = target - seats.Sum();
            var byRemainder = Enumerable.Range(0, exact.Count).OrderByDescending(j => exact[j] - seats[j]).ThenBy(j => j).ToList();
            for (int k = 0; left > 0 && k < byRemainder.Count; k++, left--) seats[byRemainder[k]]++;
            for (int d = 0; d < percents.Count; d++)
                if (seats[d] > 0) result.Add(new KeyValuePair<string, int>(percents[d].Key, seats[d]));
            return result;
        }

        /// <summary>Pairs round 1 (call once, after Create).</summary>
        public void Begin()
        {
            lock (gate)
            {
                if (round > 0) return;
                PairNextRoundLocked();
                Monitor.PulseAll(gate);
            }
        }

        /// <summary>Swiss rounds for this many players (the usual table, up to the 9 rounds of a big Day 1).</summary>
        public static int SuggestedRounds(int players)
        {
            if (players <= 2) return 1;
            if (players <= 4) return 2;
            if (players <= 8) return 3;
            if (players <= 12) return 4;
            if (players <= 32) return 5;
            if (players <= 64) return 6;
            if (players <= 128) return 7;
            if (players <= 226) return 8;
            return 9;
        }

        // ------------------------------------------------------------------ Decks

        /// <summary>The deck lists the event uses (name -> text), to build the cards from.</summary>
        public List<KeyValuePair<string, string>> DeckTexts()
        {
            lock (gate) return deckTexts.ToList();
        }

        public string DeckText(string name)
        {
            lock (gate)
            {
                string text;
                return name != null && deckTexts.TryGetValue(name, out text) ? text : null;
            }
        }

        /// <summary>The cards of a deck, built by the host from DeckTexts (the AI tables need them).</summary>
        public void SetDeckCards(string name, List<CardDef> cards)
        {
            if (name == null || cards == null) return;
            lock (gate) deckCards[name] = cards;
        }

        public RegionalEntrant Entrant(int id)
        {
            lock (gate) return id >= 0 && id < entrants.Count ? entrants[id].Copy() : null;
        }

        // ------------------------------------------------------------------ Your matches

        private RegionalEntrant YouLocked()
        {
            foreach (var e in entrants) if (e.Human && !e.Remote) return e;
            return null;
        }

        /// <summary>
        /// The match's player who sits as A: a person rather than an AI player (their games are played on their screen), and
        /// the host rather than an online friend (the host's copy plays their match against each other). True: b should be A.
        /// </summary>
        private bool PutFirst(int a, int b)
        {
            var ea = entrants[a];
            var eb = entrants[b];
            return eb.Human && (!ea.Human || (ea.Remote && !eb.Remote));
        }

        // ------------------------------------------------------------------ An online friend in the event

        /// <summary>
        /// Adds a friend who joined the host's online game as a player (call after Create, before Begin): their decklist is
        /// kept under deckName. Their games against AI players are played on their screen through the host's copy; their
        /// match against the host is the host's own match.
        /// </summary>
        public void AddGuest(string name, string deckName, string deckText)
        {
            lock (gate)
            {
                if (round > 0) throw new InvalidOperationException("The event has already begun.");
                if (entrants.Count >= MaxPlayers) throw new ArgumentException("An event can have at most " + MaxPlayers + " players.");
                deckTexts[deckName] = deckText ?? "";
                entrants.Add(new RegionalEntrant { Id = entrants.Count, Name = string.IsNullOrEmpty(name) ? "Friend" : name, Human = true, Remote = true, Deck = deckName });
                version++;
            }
        }

        private RegionalEntrant GuestLocked()
        {
            foreach (var e in entrants) if (e.Human && e.Remote) return e;
            return null;
        }

        private RegionalMatch MatchOfLocked(RegionalEntrant e)
        {
            if (e == null) return null;
            for (int k = roundStart; k < matches.Count; k++)
            {
                var m = matches[k];
                if (m.Round == round && (m.A == e.Id || m.B == e.Id)) return m;
            }
            return null;
        }

        /// <summary>The online friend in this event, or null.</summary>
        public RegionalEntrant Guest()
        {
            lock (gate)
            {
                var g = GuestLocked();
                return g == null ? null : g.Copy();
            }
        }

        /// <summary>The online friend's match this round (a copy), or null.</summary>
        public RegionalMatch GuestMatch()
        {
            lock (gate)
            {
                var m = MatchOfLocked(GuestLocked());
                return m == null ? null : m.Copy();
            }
        }

        /// <summary>The online friend has a game to play now against an AI player (their match against the host is the host's to start).</summary>
        public bool GuestGamePending
        {
            get
            {
                lock (gate)
                {
                    var g = GuestLocked();
                    var m = MatchOfLocked(g);
                    if (m == null || m.Finished || m.IsBye || finished || dropped.ContainsKey(g.Id)) return false;
                    return !entrants[m.A == g.Id ? m.B : m.A].Human;
                }
            }
        }

        /// <summary>
        /// Records one of the online friend's games against an AI player: result 1 = the friend won, -1 = lost, 0 = a draw (for
        /// the round and game it was started for, so never twice). Returns true when it was recorded.
        /// </summary>
        public bool RecordGuestGame(int roundNumber, int gameIndex, int result)
        {
            lock (gate)
            {
                var g = GuestLocked();
                var m = MatchOfLocked(g);
                if (m == null || m.Finished || m.IsBye || m.Round != roundNumber || m.Games.Count != gameIndex) return false;
                RecordGameLocked(m, Winner(m, g, result));
                Monitor.PulseAll(gate);
                return true;
            }
        }

        /// <summary>Who wins the game for RecordGameLocked (0 = A, 1 = B, -1 = a draw) when player e's result is result.</summary>
        private static int Winner(RegionalMatch m, RegionalEntrant e, int result)
        {
            if (result == 0) return -1;
            bool eIsA = m.A == e.Id;
            return result > 0 ? (eIsA ? 0 : 1) : (eIsA ? 1 : 0);
        }

        /// <summary>The online friend leaves the event (their connection was lost): their match is conceded and they play no more.</summary>
        public bool DropGuest()
        {
            lock (gate)
            {
                var g = GuestLocked();
                if (g == null || finished || dropped.ContainsKey(g.Id)) return false;
                dropped[g.Id] = round;
                version++;
                var m = MatchOfLocked(g);
                if (m != null && !m.Finished && !m.IsBye)
                {
                    m.ConcededBy = g.Id;
                    CompleteMatchLocked(m);
                }
                Monitor.PulseAll(gate);
                return true;
            }
        }

        private RegionalMatch YourMatchLocked()
        {
            var you = YouLocked();
            if (you == null) return null;
            for (int k = roundStart; k < matches.Count; k++)
            {
                var m = matches[k];
                if (m.Round == round && (m.A == you.Id || m.B == you.Id)) return m;
            }
            return null;
        }

        /// <summary>Your match this round (a copy), or null (AI only, or the event is over).</summary>
        public RegionalMatch YourMatch()
        {
            lock (gate)
            {
                var m = YourMatchLocked();
                return m == null ? null : m.Copy();
            }
        }

        /// <summary>You have a game to play right now.</summary>
        public bool YourGamePending
        {
            get
            {
                lock (gate)
                {
                    var m = YourMatchLocked();
                    var you = YouLocked();
                    return m != null && !m.Finished && !m.IsBye && !finished && (you == null || !dropped.ContainsKey(you.Id));
                }
            }
        }

        /// <summary>
        /// Who chooses to go first in a match's next game, from A's side: -1 = the opening coin flip (game 1, or after a drawn
        /// game), 0 = A, 1 = B. The loser of the last game chooses; in game 1 of a top cut match the higher seed does (your
        /// call, v70). After a drawn top cut game the same player chooses for the Tie-Breaker game (v76).
        /// </summary>
        public static int OrderChooserFor(RegionalMatch m)
        {
            if (m == null) return -1;
            if (m.IsCut)
            {
                for (int k = m.Games.Count - 1; k >= 0; k--)
                {
                    if (m.Games[k] == 0) return 1;
                    if (m.Games[k] == 1) return 0;
                }
                return m.SeedA < m.SeedB ? 0 : 1;
            }
            if (m.Games.Count == 0) return -1;
            int last = m.Games[m.Games.Count - 1];
            return last == 0 ? 1 : last == 1 ? 0 : -1;
        }

        /// <summary>
        /// The next game of this match is a Tie-Breaker game (v76, your ruling): the last game of a top cut match was drawn. It's
        /// played with 6 Prize cards and ends as soon as one player is ahead on Prize cards (GameEngine.TieBreaker).
        /// </summary>
        public static bool TieBreakerNext(RegionalMatch m)
        {
            return m != null && m.IsCut && m.Games.Count > 0 && m.Games[m.Games.Count - 1] < 0;
        }

        /// <summary>The higher seed chooses for this top cut game: nobody has won a game of the match yet (game 1, or its replays).</summary>
        public static bool HigherSeedChoosesFor(RegionalMatch m)
        {
            return m != null && m.IsCut && m.Games.All(g => g < 0);
        }

        /// <summary>
        /// Who chooses to go first in your next game: -1 = the opening coin flip (game 1, or after a drawn game), 0 = you (you
        /// lost the last game, or you're the higher seed in game 1 of a top cut match), 1 = your opponent.
        /// </summary>
        public int YourOrderChooser()
        {
            lock (gate) return OrderChooserFor(YourMatchLocked());
        }

        /// <summary>
        /// Records one of your games: result 1 = you won, -1 = you lost, 0 = a draw. roundNumber and gameIndex are the ones the
        /// game was started for, so a game is never counted twice. Returns true when it was recorded.
        /// </summary>
        public bool RecordYourGame(int roundNumber, int gameIndex, int result)
        {
            lock (gate)
            {
                var m = YourMatchLocked();
                if (m == null || m.Finished || m.IsBye || m.Round != roundNumber || m.Games.Count != gameIndex) return false;
                RecordGameLocked(m, Winner(m, YouLocked(), result));
                Monitor.PulseAll(gate);
                return true;
            }
        }

        // ------------------------------------------------------------------ The table you watch (AI only)

        private RegionalMatch FeaturedLocked()
        {
            if (featuredTable < 0) return null;
            for (int k = roundStart; k < matches.Count; k++)
            {
                var m = matches[k];
                if (m.Round == round && m.Table == featuredTable && !m.IsBye) return m;
            }
            return null;
        }

        /// <summary>AI only: one table each round is played on the board for you to watch.</summary>
        public bool Watching
        {
            get { lock (gate) return watchOne; }
        }

        /// <summary>
        /// Turns watching on or off. Off: this round's table goes back to the background. On in the middle of a round: a table
        /// that hasn't started yet is picked, if there is one.
        /// </summary>
        public void SetWatching(bool on)
        {
            lock (gate)
            {
                if (YouLocked() != null) on = false;
                watchOne = on;
                if (!on) featuredTable = -1;
                else if (FeaturedLocked() == null && !finished)
                {
                    var open = matches.Where(x => x.Round == round && !x.IsBye && !x.Finished && !x.InProgress && x.Games.Count == 0).ToList();
                    if (open.Count > 0) featuredTable = open[new Rng(seed ^ (round * 31 + version)).Next(open.Count)].Table;
                }
                version++;
                Monitor.PulseAll(gate);
            }
        }

        /// <summary>The table to watch this round, if it still has games to play (a copy), or null.</summary>
        public RegionalMatch FeaturedMatch()
        {
            lock (gate)
            {
                var m = FeaturedLocked();
                return m == null || m.Finished || finished ? null : m.Copy();
            }
        }

        /// <summary>Lets this round's table to watch be played in the background instead (watching stays on for later rounds).</summary>
        public void ReleaseFeatured()
        {
            lock (gate)
            {
                featuredTable = -1;
                version++;
                Monitor.PulseAll(gate);
            }
        }

        /// <summary>Who chooses to go first in the watched table's next game: -1 = a coin flip, 0 = A (seat 0), 1 = B.</summary>
        public int FeaturedOrderChooser()
        {
            lock (gate) return OrderChooserFor(FeaturedLocked());
        }

        /// <summary>
        /// Records a game of the watched table: resultA = 0 (A won), 1 (B won) or -1 (a draw), for the round, table and game it was
        /// started for. Returns true when it was recorded.
        /// </summary>
        public bool RecordFeaturedGame(int roundNumber, int table, int gameIndex, int resultA)
        {
            lock (gate)
            {
                var m = FeaturedLocked();
                if (m == null || m.Finished || m.Round != roundNumber || m.Table != table || m.Games.Count != gameIndex) return false;
                aiGames++;
                RecordGameLocked(m, resultA == 0 ? 0 : resultA == 1 ? 1 : -1);
                Monitor.PulseAll(gate);
                return true;
            }
        }

        /// <summary>
        /// Records a game of an AI table by hand (tests and tools; the board uses RecordFeaturedGame): resultA = 0 (A won), 1 (B
        /// won) or -1 (a draw), for a table of the current round that isn't being played right now. Returns true when it was recorded.
        /// </summary>
        public bool RecordTableGame(int roundNumber, int table, int gameIndex, int resultA)
        {
            lock (gate)
            {
                RegionalMatch m = null;
                foreach (var x in matches)
                    if (x.Round == roundNumber && x.Table == table) m = x;
                if (m == null || m.Finished || m.IsBye || m.InProgress || m.Round != round || m.Games.Count != gameIndex) return false;
                if (entrants[m.A].Human || entrants[m.B].Human) return false;
                RecordGameLocked(m, resultA == 0 ? 0 : resultA == 1 ? 1 : -1);
                Monitor.PulseAll(gate);
                return true;
            }
        }

        // ------------------------------------------------------------------ Results

        public static bool Decided(RegionalMatch m, bool bestOf3)
        {
            if (m.IsBye || m.ConcededBy >= 0) return true;
            if (m.IsCut) return DecidedCut(m, bestOf3);
            if (!bestOf3) return m.Games.Count >= 1;
            return m.WinsA >= 2 || m.WinsB >= 2 || m.Games.Count >= 3;
        }

        /// <summary>
        /// A top cut match needs a winner (your call, v70): a drawn game is followed by a Tie-Breaker game (v76) until someone wins
        /// 1 game (best of 1) or 2 (best of 3). After MaxCutGames the higher seed goes through.
        /// </summary>
        public static bool DecidedCut(RegionalMatch m, bool bestOf3)
        {
            int need = bestOf3 ? 2 : 1;
            if (m.ConcededBy >= 0) return true;
            return m.WinsA >= need || m.WinsB >= need || m.Games.Count >= MaxCutGames;
        }

        private bool DecidedLocked(RegionalMatch m)
        {
            return m.IsCut ? DecidedCut(m, cutBestOfThree) : Decided(m, bestOfThree);
        }

        private void RecordGameLocked(RegionalMatch m, int result)
        {
            if (m.Finished) return;
            m.Games.Add(result);
            version++;
            if (DecidedLocked(m)) CompleteMatchLocked(m);
        }

        private void CompleteMatchLocked(RegionalMatch m)
        {
            m.Finished = true;
            m.InProgress = false;
            version++;
            if (m.Round == round) roundOpen--;
            if (roundOpen > 0) return;
            // Every match of the round is over: the next round is paired (or the event ends).
            AdvanceLocked();
            Monitor.PulseAll(gate);
        }

        /// <summary>The round is over: the next Swiss round, the top cut, its next round, or the end of the event.</summary>
        private void AdvanceLocked()
        {
            // v127: Day 1 is over: the players with enough match points make Day 2.
            if (day1Rounds > 0 && !day2Decided && round >= day1Rounds && round <= rounds) DecideDay2Locked();
            if (round < rounds)
            {
                // v127: Swiss goes on while 2 or more players are still in it (drops, or too few made Day 2, can end it early).
                if (entrants.Count(e => ActiveLocked(e.Id)) >= 2)
                {
                    PairNextRoundLocked();
                    return;
                }
                round = rounds;
            }
            if (round == rounds)
            {
                if (topCut >= 2) StartTopCutLocked();
                else finished = true;
                return;
            }
            if (InPlayInLocked())
            {
                AdvancePlayInLocked();
                return;
            }
            int cutStart = rounds + playInRounds;
            if (round > cutStart && round < cutStart + CutRoundsFor(topCut)) PairNextCutRoundLocked();
            else finished = true;
        }

        /// <summary>v127: every player still in (not dropped) with at least day2Points match points makes Day 2.</summary>
        private void DecideDay2Locked()
        {
            day2Decided = true;
            day2Players.Clear();
            foreach (var s in StandingsLocked())
                if (!dropped.ContainsKey(s.Id) && s.Points >= day2Points) day2Players.Add(s.Id);
            version++;
        }

        // ------------------------------------------------------------------ Top cut (v70)

        /// <summary>The first top cut round by seed, in bracket order: the winners of tables 1 and 2 meet, then 3 and 4.</summary>
        private static int[][] FirstCutPairs(int cut)
        {
            if (cut == 2) return new[] { new[] { 1, 2 } };
            if (cut == 4) return new[] { new[] { 1, 4 }, new[] { 2, 3 } };
            return new[] { new[] { 1, 8 }, new[] { 4, 5 }, new[] { 2, 7 }, new[] { 3, 6 } };
        }

        /// <summary>Swiss is over: the top players are seeded by their standing and the first top cut round is paired.</summary>
        private void StartTopCutLocked()
        {
            // v127: players who dropped aren't in it; with fewer players left than the cut, the cut gets smaller.
            var standings = StandingsLocked().Where(x => !dropped.ContainsKey(x.Id)).ToList();
            while (topCut >= 2 && topCut > standings.Count) topCut /= 2;
            if (topCut < 2)
            {
                topCut = 0;
                finished = true;
                version++;
                return;
            }
            if (asymmetricCut)
            {
                // v127: everyone on the last seat's match points plays for the seats those points hold (your ruling).
                var bubble = standings[topCut - 1];
                int first = standings.FindIndex(x => x.Points == bubble.Points && x.MissedDay2 == bubble.MissedDay2);
                var pool = standings.Skip(first).TakeWhile(x => x.Points == bubble.Points && x.MissedDay2 == bubble.MissedDay2).Select(x => x.Id).ToList();
                int seats = topCut - first;
                if (pool.Count > seats)
                {
                    playInAbove.Clear();
                    playInAbove.AddRange(standings.Take(first).Select(x => x.Id));
                    playInPool.Clear();
                    playInPool.AddRange(pool);
                    playInSeats = seats;
                    PairPlayInRoundLocked(pool);
                    return;
                }
            }
            BeginCutLocked(standings.Take(topCut).Select(x => x.Id).ToList());
        }

        /// <summary>The top cut's seeds are set: its first round is paired.</summary>
        private void BeginCutLocked(List<int> seeds)
        {
            while (topCut >= 2 && seeds.Count < topCut) topCut /= 2;
            if (topCut < 2)
            {
                topCut = 0;
                finished = true;
                version++;
                return;
            }
            cutSeeds.Clear();
            for (int k = 0; k < topCut; k++) cutSeeds.Add(seeds[k]);
            round++;
            roundStart = matches.Count;
            int table = 1;
            foreach (var pair in FirstCutPairs(topCut)) AddCutMatchLocked(table++, pair[0], pair[1]);
            roundOpen = matches.Count - roundStart;
            PickFeaturedLocked(new Rng(seed ^ (round * 7919 + 17)));
            version++;
        }

        // ------------------------------------------------------------------ Asymmetrical top cut's play-in (v127)

        /// <summary>The play-in is being played: its pool is set and the top cut isn't seeded yet.</summary>
        private bool InPlayInLocked()
        {
            return playInPool.Count > 0 && cutSeeds.Count == 0 && round > rounds && !finished;
        }

        /// <summary>A play-in player's Swiss seed (1 = first after Swiss, players who dropped before the cut left out).</summary>
        private int PlayInSeedLocked(int id)
        {
            int k = playInPool.IndexOf(id);
            return k < 0 ? 0 : playInAbove.Count + k + 1;
        }

        /// <summary>The play-in players who haven't lost a play-in match or dropped, by Swiss seed.</summary>
        private List<int> PlayInAliveLocked()
        {
            var lost = new HashSet<int>();
            foreach (var m in matches)
                if (m.PlayIn && m.Finished) lost.Add(m.Result == 0 ? m.B : m.A);
            return playInPool.Where(id => !lost.Contains(id) && !dropped.ContainsKey(id)).ToList();
        }

        /// <summary>
        /// One play-in round: as many matches as still need to be decided (at most half the players), played by the lowest seeds
        /// (the highest of them against the lowest); the better seeds have a bye. Each loss is out.
        /// </summary>
        private void PairPlayInRoundLocked(List<int> alive)
        {
            round++;
            playInRounds++;
            roundStart = matches.Count;
            int count = Math.Min(alive.Count - playInSeats, alive.Count / 2);
            var bottom = alive.Skip(alive.Count - 2 * count).ToList();
            for (int k = 0; k < count; k++)
            {
                int high = bottom[k], low = bottom[2 * count - 1 - k];
                int seedHigh = PlayInSeedLocked(high), seedLow = PlayInSeedLocked(low);
                if (PutFirst(high, low))
                    matches.Add(new RegionalMatch { Round = round, Table = k + 1, A = low, B = high, SeedA = seedLow, SeedB = seedHigh, PlayIn = true });
                else
                    matches.Add(new RegionalMatch { Round = round, Table = k + 1, A = high, B = low, SeedA = seedHigh, SeedB = seedLow, PlayIn = true });
            }
            roundOpen = matches.Count - roundStart;
            PickFeaturedLocked(new Rng(seed ^ (round * 7919 + 17)));
            version++;
        }

        /// <summary>
        /// A play-in round is over: the next one, or the top cut. Its seeds are the players above the bubble, then the play-in's
        /// winners, by Swiss seed. A seat left empty by a drop goes to the best-seeded play-in player who lost.
        /// </summary>
        private void AdvancePlayInLocked()
        {
            var alive = PlayInAliveLocked();
            if (alive.Count > playInSeats && alive.Count >= 2)
            {
                PairPlayInRoundLocked(alive);
                return;
            }
            var seeds = playInAbove.Where(id => !dropped.ContainsKey(id)).ToList();
            seeds.AddRange(alive.Take(playInSeats));
            foreach (var loser in playInPool)
            {
                if (seeds.Count >= topCut) break;
                if (!seeds.Contains(loser) && !dropped.ContainsKey(loser)) seeds.Add(loser);
            }
            seeds = seeds.OrderBy(id => playInAbove.Contains(id) ? playInAbove.IndexOf(id) : playInAbove.Count + playInPool.IndexOf(id)).ToList();
            BeginCutLocked(seeds);
        }

        /// <summary>The winners of tables 1 and 2 of the last round meet, then those of 3 and 4.</summary>
        private void PairNextCutRoundLocked()
        {
            var last = new List<RegionalMatch>();
            for (int k = roundStart; k < matches.Count; k++)
                if (matches[k].Round == round) last.Add(matches[k]);
            last = last.OrderBy(x => x.Table).ToList();
            round++;
            roundStart = matches.Count;
            int table = 1;
            for (int k = 0; k + 1 < last.Count; k += 2)
            {
                int first = WinnerSeed(last[k]), second = WinnerSeed(last[k + 1]);
                AddCutMatchLocked(table++, Math.Min(first, second), Math.Max(first, second));
            }
            roundOpen = matches.Count - roundStart;
            PickFeaturedLocked(new Rng(seed ^ (round * 7919 + 17)));
            version++;
        }

        private static int WinnerSeed(RegionalMatch m)
        {
            return m.Result == 0 ? m.SeedA : m.SeedB;
        }

        /// <summary>A top cut match: the higher seed is A, unless you're the other player (you're always A).</summary>
        private void AddCutMatchLocked(int table, int higherSeed, int lowerSeed)
        {
            int high = cutSeeds[higherSeed - 1], low = cutSeeds[lowerSeed - 1];
            if (PutFirst(high, low))
                matches.Add(new RegionalMatch { Round = round, Table = table, A = low, B = high, SeedA = lowerSeed, SeedB = higherSeed });
            else
                matches.Add(new RegionalMatch { Round = round, Table = table, A = high, B = low, SeedA = higherSeed, SeedB = lowerSeed });
        }

        /// <summary>
        /// Places by player Id: the top cut's players by how far they got (the winner, the other finalist, then each round's
        /// losers by seed), everyone else by their Swiss standing.
        /// </summary>
        private int[] PlacementsLocked(List<RegionalStanding> standings)
        {
            var place = new int[entrants.Count];
            foreach (var s in standings) place[s.Id] = s.Rank;
            if (cutSeeds.Count == 0) return place;
            // v127: everyone outside the top cut places after it, in Swiss order (players who dropped, or lost a play-in, included).
            int next = cutSeeds.Count + 1;
            foreach (var s in standings.OrderBy(x => x.Rank))
                if (!cutSeeds.Contains(s.Id)) place[s.Id] = next++;
            var outIn = new Dictionary<int, int>();
            foreach (var m in matches)
            {
                if (!m.IsCut || !m.Finished || m.PlayIn) continue;
                outIn[m.Result == 0 ? m.B : m.A] = m.Round;
            }
            var order = cutSeeds.Select((player, index) => new { Id = player, Seed = index + 1, Out = outIn.ContainsKey(player) ? outIn[player] : int.MaxValue })
                                .OrderByDescending(x => x.Out).ThenBy(x => x.Seed).ToList();
            for (int k = 0; k < order.Count; k++) place[order[k].Id] = k + 1;
            return place;
        }

        // ------------------------------------------------------------------ Standings

        private List<RegionalStanding> StandingsLocked()
        {
            var st = entrants.Select(e => new RegionalStanding { Id = e.Id, Name = e.Name, Deck = e.Deck, Human = e.Human }).ToList();
            var opponents = new List<int>[entrants.Count];
            for (int i = 0; i < opponents.Length; i++) opponents[i] = new List<int>();
            foreach (var m in matches)
            {
                // The top cut doesn't change the Swiss standings (v70).
                if (!m.Finished || m.IsCut) continue;
                var a = st[m.A];
                if (m.IsBye)
                {
                    a.Wins++;
                    a.Byes++;
                    continue;
                }
                var b = st[m.B];
                opponents[m.A].Add(m.B);
                opponents[m.B].Add(m.A);
                a.GameWins += m.WinsA;
                a.GameLosses += m.WinsB;
                b.GameWins += m.WinsB;
                b.GameLosses += m.WinsA;
                int res = m.Result;
                if (res == 0)
                {
                    a.Wins++;
                    b.Losses++;
                }
                else if (res == 1)
                {
                    b.Wins++;
                    a.Losses++;
                }
                else
                {
                    a.Ties++;
                    b.Ties++;
                }
            }
            foreach (var s in st)
            {
                int played = s.Wins + s.Losses + s.Ties;
                s.WinPct = played == 0 ? 0.25 : Math.Max(0.25, s.Points / (3.0 * played));
            }
            foreach (var s in st)
            {
                var opp = opponents[s.Id];
                s.OppWinPct = opp.Count == 0 ? 0 : opp.Average(o => st[o].WinPct);
            }
            foreach (var s in st)
            {
                var opp = opponents[s.Id];
                s.OppOppWinPct = opp.Count == 0 ? 0 : opp.Average(o => st[o].OppWinPct);
            }
            foreach (var s in st)
            {
                s.Dropped = dropped.ContainsKey(s.Id);
                s.Day2 = day2Decided && day2Players.Contains(s.Id);
                s.MissedDay2 = day2Decided && !s.Day2;
            }
            // v127: once Day 1 is over, everyone who made Day 2 ranks above everyone who didn't (your ruling).
            var ordered = st.OrderBy(x => x.MissedDay2 ? 1 : 0).ThenByDescending(x => x.Points).ThenByDescending(x => x.OppWinPct).ThenByDescending(x => x.OppOppWinPct).ThenBy(x => x.Id).ToList();
            for (int k = 0; k < ordered.Count; k++) ordered[k].Rank = k + 1;
            return ordered;
        }

        public List<RegionalStanding> Standings()
        {
            lock (gate) return StandingsLocked();
        }

        private static List<RegionalDeckStats> DeckStats(List<RegionalStanding> standings, int[] placement, IEnumerable<RegionalMatch> cutMatches, IList<RegionalEntrant> players)
        {
            var byDeck = new Dictionary<string, RegionalDeckStats>(StringComparer.Ordinal);
            foreach (var s in standings)
            {
                RegionalDeckStats d;
                if (!byDeck.TryGetValue(s.Deck, out d))
                {
                    d = new RegionalDeckStats { Deck = s.Deck };
                    byDeck[s.Deck] = d;
                }
                d.Players++;
                d.Wins += s.Wins - s.Byes;
                d.Losses += s.Losses;
                d.Ties += s.Ties;
                int place = placement != null && s.Id < placement.Length && placement[s.Id] > 0 ? placement[s.Id] : s.Rank;
                d.BestRank = Math.Min(d.BestRank, place);
            }
            // Top cut matches count for the decks too (v70).
            foreach (var m in cutMatches)
            {
                if (!m.Finished) continue;
                RegionalDeckStats da, db;
                if (!byDeck.TryGetValue(players[m.A].Deck, out da) || !byDeck.TryGetValue(players[m.B].Deck, out db)) continue;
                if (m.Result == 0)
                {
                    da.Wins++;
                    db.Losses++;
                }
                else
                {
                    db.Wins++;
                    da.Losses++;
                }
            }
            return byDeck.Values.OrderByDescending(x => x.Players).ThenBy(x => x.BestRank).ToList();
        }

        /// <summary>Everything the screen shows, copied in one go.</summary>
        public RegionalView View()
        {
            lock (gate)
            {
                var v = new RegionalView
                {
                    Version = version,
                    Round = round,
                    Rounds = rounds,
                    Players = entrants.Count,
                    BestOfThree = bestOfThree,
                    AiExplore = aiExplore,
                    PlayStyle = aiStyle,
                    Format = format,
                    Finished = finished,
                    AiGames = aiGames,
                    AiErrors = aiErrors,
                    LastError = lastError
                };
                v.Standings = StandingsLocked();
                v.Day1Rounds = day1Rounds;
                v.Day2Points = day2Points;
                v.Day2Decided = day2Decided;
                v.Day2Players = day2Players.Count;
                var youEntrant = YouLocked();
                if (youEntrant != null)
                {
                    int droppedAfter;
                    v.YouDropped = dropped.TryGetValue(youEntrant.Id, out droppedAfter);
                    v.YouDroppedAfterRound = v.YouDropped ? droppedAfter : 0;
                    bool outOfCut = matches.Any(x => x.IsCut && x.Finished && (x.Result == 0 ? x.B : x.A) == youEntrant.Id);
                    bool inCut = cutSeeds.Contains(youEntrant.Id);
                    bool playInNow = InPlayInLocked();
                    v.YouStillIn = !finished && !v.YouDropped
                                   && (round <= rounds ? ActiveLocked(youEntrant.Id)
                                       : playInNow ? playInAbove.Contains(youEntrant.Id) || PlayInAliveLocked().Contains(youEntrant.Id)
                                       : inCut && !outOfCut);
                }
                v.TopCut = topCut;
                v.CutRounds = CutRoundsFor(topCut);
                v.CutBestOfThree = cutBestOfThree;
                v.CutSeeds = new List<int>(cutSeeds);
                v.AsymmetricCut = asymmetricCut;
                v.PlayInRounds = playInRounds;
                v.PlayInSeats = playInSeats;
                v.InPlayIn = InPlayInLocked();
                v.PlayInPool = new List<int>(playInPool);
                v.PlayInAbove = new List<int>(playInAbove);
                v.PlayInPoints = 0;
                foreach (var s in v.Standings)
                    if (playInPool.Count > 0 && s.Id == playInPool[0]) v.PlayInPoints = s.Points;
                v.Placement = PlacementsLocked(v.Standings);
                v.Decks = DeckStats(v.Standings, v.Placement, matches.Where(x => x.IsCut), entrants);
                if (finished)
                    for (int id = 0; id < v.Placement.Length; id++)
                        if (v.Placement[id] == 1) v.Champion = id;
                v.Matches = matches.Select(x => x.Copy()).ToList();
                var mine = YourMatchLocked();
                v.YourMatch = mine == null ? null : mine.Copy();
                var featured = FeaturedLocked();
                v.Featured = featured == null ? null : featured.Copy();
                v.Watching = watchOne;
                v.YouPlaying = YouLocked() != null;
                foreach (var s in v.Standings)
                    if (s.Human && !entrants[s.Id].Remote) v.You = s;
                foreach (var m in matches)
                {
                    if (m.Round != round) continue;
                    v.RoundMatches++;
                    if (m.Finished) v.RoundFinished++;
                    else if (m.InProgress) v.RoundPlaying++;
                }
                v.Names = new string[entrants.Count];
                v.DeckNames = new string[entrants.Count];
                foreach (var e in entrants)
                {
                    v.Names[e.Id] = e.Name;
                    v.DeckNames[e.Id] = e.Deck;
                }
                return v;
            }
        }

        // ------------------------------------------------------------------ Pairing

        /// <summary>A key for "these two have played each other" (either order).</summary>
        public static long PairKey(int a, int b)
        {
            int lo = Math.Min(a, b), hi = Math.Max(a, b);
            return ((long)lo << 32) | (uint)hi;
        }

        /// <summary>Pairs the next round. Call inside the lock.</summary>
        private void PairNextRoundLocked()
        {
            round++;
            var standings = StandingsLocked();
            var pr = new Rng(seed ^ (round * 7919 + 17));

            // The bye: the lowest-ranked player who hasn't had one (chosen at random among players tied with them).
            // v127: only the players still in (not dropped, and in Day 2 once Day 1 is over).
            standings = standings.Where(x => ActiveLocked(x.Id)).ToList();
            int byeId = -1;
            if (standings.Count % 2 == 1)
            {
                var hadBye = new HashSet<int>();
                foreach (var m in matches)
                    if (m.IsBye) hadBye.Add(m.A);
                var eligible = standings.Where(x => !hadBye.Contains(x.Id)).ToList();
                if (eligible.Count == 0) eligible = standings.ToList();
                var lowest = eligible[eligible.Count - 1];
                var tied = eligible.Where(x => x.Points == lowest.Points && x.OppWinPct == lowest.OppWinPct && x.OppOppWinPct == lowest.OppOppWinPct).ToList();
                byeId = tied[pr.Next(tied.Count)].Id;
            }

            // Everyone else by match points, in random order among equal records.
            var order = standings.Select(x => x.Id).Where(id => id != byeId).ToList();
            pr.Shuffle(order);
            var points = new Dictionary<int, int>();
            foreach (var s in standings) points[s.Id] = s.Points;
            order = order.OrderByDescending(id => points[id]).ToList();

            var played = new HashSet<long>();
            foreach (var m in matches)
                if (!m.IsBye) played.Add(PairKey(m.A, m.B));

            var pairs = SwissPairs(order, played);
            roundStart = matches.Count;
            roundOpen = pairs.Count;
            int table = 1;
            foreach (var pair in pairs)
            {
                int a = pair.Key, b = pair.Value;
                if (PutFirst(a, b))
                {
                    int t = a;
                    a = b;
                    b = t;
                }
                matches.Add(new RegionalMatch { Round = round, Table = table++, A = a, B = b });
            }
            if (byeId >= 0) matches.Add(new RegionalMatch { Round = round, Table = table, A = byeId, B = -1, Finished = true });
            PickFeaturedLocked(pr);
            version++;
            // A round can be all byes only with 1 player; never with 2 or more, so it never completes here.
        }

        /// <summary>AI only, watching: one table of the round, at random, is played on the board.</summary>
        private void PickFeaturedLocked(Rng pr)
        {
            featuredTable = -1;
            if (!watchOne || YouLocked() != null) return;
            var candidates = matches.Where(x => x.Round == round && !x.IsBye).ToList();
            if (candidates.Count > 0) featuredTable = candidates[pr.Next(candidates.Count)].Table;
        }

        /// <summary>
        /// Pairs players in order (best record first): each one with the closest player below them they haven't played yet,
        /// backing up when that leaves someone without an opponent. If rematches can't be avoided (a small event with many
        /// rounds), each player gets the closest one they haven't played if possible, otherwise the closest one.
        /// </summary>
        public static List<KeyValuePair<int, int>> SwissPairs(List<int> order, HashSet<long> played)
        {
            int n = order.Count;
            var partner = new int[n];
            for (int i = 0; i < n; i++) partner[i] = -1;
            int steps = 0;
            var result = new List<KeyValuePair<int, int>>();
            if (Backtrack(order, played, partner, ref steps))
            {
                for (int i = 0; i < n; i++)
                    if (partner[i] > i) result.Add(new KeyValuePair<int, int>(order[i], order[partner[i]]));
                return result;
            }
            var used = new bool[n];
            for (int i = 0; i < n; i++)
            {
                if (used[i]) continue;
                int best = -1;
                for (int j = i + 1; j < n; j++)
                {
                    if (used[j]) continue;
                    if (best < 0) best = j;
                    if (!played.Contains(PairKey(order[i], order[j])))
                    {
                        best = j;
                        break;
                    }
                }
                if (best < 0) break;
                used[i] = true;
                used[best] = true;
                result.Add(new KeyValuePair<int, int>(order[i], order[best]));
            }
            return result;
        }

        /// <summary>
        /// The pairing search, without recursion (an event of 4,000 players would otherwise go 2,000 calls deep; v57.1): the
        /// first unpaired player takes the next candidate they haven't played; when someone has nobody left, the last pairing
        /// is undone and that player tries their next candidate.
        /// </summary>
        private static bool Backtrack(List<int> order, HashSet<long> played, int[] partner, ref int steps)
        {
            int n = partner.Length;
            var pairedI = new List<int>();
            var pairedJ = new List<int>();
            int i = NextUnpaired(partner, 0);
            int j = i + 1;
            while (true)
            {
                if (i < 0) return true;
                int found = -1;
                for (; j < n; j++)
                {
                    if (partner[j] >= 0) continue;
                    if (++steps > MaxPairingSteps) return false;
                    if (played.Contains(PairKey(order[i], order[j]))) continue;
                    found = j;
                    break;
                }
                if (found >= 0)
                {
                    partner[i] = found;
                    partner[found] = i;
                    pairedI.Add(i);
                    pairedJ.Add(found);
                    i = NextUnpaired(partner, i + 1);
                    j = i + 1;
                    continue;
                }
                if (pairedI.Count == 0) return false;
                int last = pairedI.Count - 1;
                int backI = pairedI[last], backJ = pairedJ[last];
                pairedI.RemoveAt(last);
                pairedJ.RemoveAt(last);
                partner[backI] = -1;
                partner[backJ] = -1;
                i = backI;
                j = backJ + 1;
            }
        }

        private static int NextUnpaired(int[] partner, int from)
        {
            for (int k = Math.Max(0, from); k < partner.Length; k++)
                if (partner[k] < 0) return k;
            return -1;
        }

        // ------------------------------------------------------------------ The AI tables

        /// <summary>
        /// The tables to play at once for a computer with this many processor threads: 2 fewer (one for the screen, one for
        /// your game), from 1 to MaxTablesAtOnce (v71).
        /// </summary>
        public static int SuggestedTablesAtOnce(int processorThreads)
        {
            return Math.Max(1, Math.Min(MaxTablesAtOnce, processorThreads - 2));
        }

        /// <summary>How many AI tables play at once while the workers run (the last count given to StartWorkers).</summary>
        public int TablesAtOnce
        {
            get { lock (gate) return tablesAtOnce; }
        }

        /// <summary>
        /// Starts the background threads that play the AI tables, count at a time (1 to MaxTablesAtOnce). While they run it
        /// changes how many play at once (v71): more start at once, and the ones over the new count stop after their game.
        /// </summary>
        public void StartWorkers(int count)
        {
            lock (gate)
            {
                stopping = false;
                tablesAtOnce = Math.Max(1, Math.Min(MaxTablesAtOnce, count));
                if (workers.Count > tablesAtOnce) workers.RemoveRange(tablesAtOnce, workers.Count - tablesAtOnce);
                while (workers.Count < tablesAtOnce)
                {
                    var t = new Thread(WorkerLoop);
                    t.IsBackground = true;
                    t.Priority = ThreadPriority.BelowNormal;
                    t.Name = "Regional Sim " + (++workersStarted);
                    workers.Add(t);
                    t.Start();
                }
                Monitor.PulseAll(gate);
            }
        }

        /// <summary>Background threads playing right now (the ones told to stop and still finishing a game aren't counted).</summary>
        public int WorkerCount
        {
            get { lock (gate) return workers.Count; }
        }

        /// <summary>This thread is one of the workers that should keep playing.</summary>
        private bool KeepWorkingLocked()
        {
            return !stopping && !finished && workers.Contains(Thread.CurrentThread);
        }

        /// <summary>Stops the workers after the game each is playing (waits up to waitMs). Unfinished matches are played later.</summary>
        public void StopWorkers(int waitMs)
        {
            List<Thread> list;
            lock (gate)
            {
                stopping = true;
                Monitor.PulseAll(gate);
                list = workers.ToList();
                workers.Clear();
            }
            var until = DateTime.UtcNow.AddMilliseconds(Math.Max(0, waitMs));
            foreach (var t in list)
            {
                int left = (int)Math.Max(0, (until - DateTime.UtcNow).TotalMilliseconds);
                if (t.IsAlive) t.Join(left);
            }
        }

        public bool WorkersRunning
        {
            get { lock (gate) return workers.Count > 0 && !stopping; }
        }

        /// <summary>Plays every AI table of the current rounds on this thread until only your match is left (tests).</summary>
        public void PlayAiTablesNow()
        {
            var scripts = ScriptsFactory != null ? ScriptsFactory() : null;
            while (true)
            {
                RegionalMatch m;
                lock (gate)
                {
                    if (finished) return;
                    m = NextAiMatchLocked();
                    if (m == null) return;
                    m.InProgress = true;
                }
                try { PlayAiMatch(m, scripts, false); }
                finally
                {
                    lock (gate) m.InProgress = false;
                }
            }
        }

        private RegionalMatch NextAiMatchLocked()
        {
            for (int k = roundStart; k < matches.Count; k++)
            {
                var m = matches[k];
                if (m.Round != round || m.Finished || m.InProgress || m.IsBye) continue;
                if (entrants[m.A].Human || entrants[m.B].Human) continue;
                if (m.Table == featuredTable) continue; // played on the board
                return m;
            }
            return null;
        }

        private void WorkerLoop()
        {
            CardScriptRegistry scripts;
            try
            {
                scripts = ScriptsFactory != null ? ScriptsFactory() : null;
            }
            catch (Exception e)
            {
                lock (gate) lastError = "The AI tables couldn't start: " + e.Message;
                return;
            }
            while (true)
            {
                RegionalMatch m = null;
                lock (gate)
                {
                    while (true)
                    {
                        if (!KeepWorkingLocked()) return;
                        m = NextAiMatchLocked();
                        if (m != null)
                        {
                            m.InProgress = true;
                            version++;
                            break;
                        }
                        Monitor.Wait(gate, 500);
                    }
                }
                try
                {
                    PlayAiMatch(m, scripts, true);
                }
                catch (Exception ex)
                {
                    lock (gate) lastError = "Table " + m.Table + ": " + ex.GetType().Name + ": " + ex.Message + ErrorSite(ex);
                }
                finally
                {
                    lock (gate)
                    {
                        m.InProgress = false;
                        version++;
                    }
                }
            }
        }

        private static int GameSeed(int eventSeed, int roundNumber, int table, int game)
        {
            unchecked
            {
                int s = eventSeed * 31 + roundNumber * 100003 + table * 1009 + game * 7 + 1;
                return (s & 0x0FFFFFFF) | 1;
            }
        }

        /// <summary>Plays a match's games until it's decided. background: a worker thread, which stops after a game once it's told to.</summary>
        private void PlayAiMatch(RegionalMatch m, CardScriptRegistry scripts, bool background)
        {
            RegionalEntrant a, b;
            List<CardDef> cardsA, cardsB;
            int eventSeed;
            bool explore;
            AiStyle style;
            lock (gate)
            {
                a = entrants[m.A];
                b = entrants[m.B];
                deckCards.TryGetValue(a.Deck, out cardsA);
                deckCards.TryGetValue(b.Deck, out cardsB);
                eventSeed = seed;
                explore = aiExplore;
                style = aiStyle;
            }
            while (!stopping)
            {
                int game;
                int chooser;
                bool tieBreaker;
                lock (gate)
                {
                    if (m.Finished) return;
                    if (background && !KeepWorkingLocked()) return;
                    game = m.Games.Count;
                    // The loser of the previous game (seat 0 is A), or the higher seed in game 1 of a top cut match, chooses.
                    chooser = OrderChooserFor(m);
                    tieBreaker = TieBreakerNext(m);
                }
                int result;
                var gameStart = DateTime.UtcNow;
                if (cardsA == null || cardsB == null || scripts == null)
                {
                    lock (gate)
                    {
                        aiErrors++;
                        lastError = "Table " + m.Table + ": no cards for " + (cardsA == null ? a.Deck : b.Deck) + "; the game counts as a draw.";
                    }
                    result = -1;
                }
                else
                {
                    result = PlayOneGame(scripts, a, b, cardsA, cardsB, GameSeed(eventSeed, m.Round, m.Table, game), chooser, explore, style, tieBreaker);
                }
                lock (gate) RecordGameLocked(m, result);
                // v120: the battery saver (background tables only; never while holding the lock).
                if (background) WorkThrottle.Rest(DateTime.UtcNow - gameStart, () => { lock (gate) return stopping || !KeepWorkingLocked(); });
            }
        }

        /// <summary>One AI game. Both seats learn from it. Returns 0 (A won), 1 (B won) or -1 (a draw or an engine error).</summary>
        private int PlayOneGame(CardScriptRegistry scripts, RegionalEntrant a, RegionalEntrant b, List<CardDef> cardsA, List<CardDef> cardsB,
                                int gameSeed, int chooser, bool explore, AiStyle style, bool tieBreaker)
        {
            var brain = Brain;
            var agentA = new LearningAgent(brain, gameSeed * 2 + 1, a.Deck, b.Deck) { Training = explore, Record = brain != null, Style = style };
            var agentB = new LearningAgent(brain, gameSeed * 2 + 2, b.Deck, a.Deck) { Training = explore, Record = brain != null, Style = style };
            try
            {
                var engine = new GameEngine(scripts, agentA, agentB);
                engine.CheckInvariants = false;
                engine.CardsPerPlayer = cardsA.Count == cardsB.Count ? cardsA.Count : 0;
                engine.PlayerNames = new[] { a.Name, b.Name };
                engine.OrderChooser = chooser;
                engine.TieBreaker = tieBreaker;
                var r = engine.Run(cardsA, cardsB, gameSeed);
                if (brain != null)
                {
                    agentA.LearnFromGame(engine);
                    agentB.LearnFromGame(engine);
                    brain.IncrementCounters(1, 0, 0);
                }
                lock (gate) aiGames++;
                return r.Winner < 0 ? -1 : r.Winner;
            }
            catch (Exception e)
            {
                lock (gate)
                {
                    aiErrors++;
                    lastError = a.Name + " vs " + b.Name + " (seed " + gameSeed + "): " + e.GetType().Name + ": " + e.Message + ErrorSite(e) + " (counted as a draw)";
                }
                return -1;
            }
        }

        /// <summary>
        /// v134.1: where an engine error came from: the first frame of the stack trace that's in the game's own code (not the
        /// .NET list or dictionary that threw), so the message names the method to fix.
        /// </summary>
        private static string ErrorSite(Exception e)
        {
            if (e == null || string.IsNullOrEmpty(e.StackTrace)) return "";
            var lines = e.StackTrace.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
            var own = lines.FirstOrDefault(l => l.Contains("PokemonTCG.")) ?? lines.FirstOrDefault();
            if (own == null) return "";
            if (own.StartsWith("at ", StringComparison.Ordinal)) own = own.Substring(3);
            return " [in " + own + "]";
        }

        // ------------------------------------------------------------------ Saving

        private static string Escape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length + 16);
            foreach (char c in s)
            {
                if (c == '\\') sb.Append("\\\\");
                else if (c == '\t') sb.Append("\\t");
                else if (c == '\n') sb.Append("\\n");
                else if (c != '\r') sb.Append(c);
            }
            return sb.ToString();
        }

        private static string Unescape(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c == '\\' && i + 1 < s.Length)
                {
                    char next = s[++i];
                    sb.Append(next == 'n' ? '\n' : next == 't' ? '\t' : next);
                }
                else sb.Append(c);
            }
            return sb.ToString();
        }

        private static string I(int x)
        {
            return x.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>The whole event as text (decks included, so it doesn't depend on the deck folder).</summary>
        public string Serialize()
        {
            lock (gate)
            {
                var sb = new StringBuilder();
                sb.Append(FileHeader).Append('\n');
                sb.Append("Seed\t").Append(I(seed)).Append('\n');
                sb.Append("Rounds\t").Append(I(rounds)).Append('\n');
                sb.Append("Round\t").Append(I(round)).Append('\n');
                sb.Append("BestOf\t").Append(bestOfThree ? "3" : "1").Append('\n');
                sb.Append("TopCut\t").Append(I(topCut)).Append('\n');
                sb.Append("CutBestOf\t").Append(cutBestOfThree ? "3" : "1").Append('\n');
                if (day1Rounds > 0) sb.Append("Day2\t").Append(I(day1Rounds)).Append('\t').Append(I(day2Points)).Append('\t').Append(day2Decided ? "1" : "0").Append('\n');
                if (day2Players.Count > 0) sb.Append("Day2Players\t").Append(string.Join(",", day2Players.OrderBy(x => x).Select(I).ToArray())).Append('\n');
                foreach (var kv in dropped.OrderBy(x => x.Key)) sb.Append("Dropped\t").Append(I(kv.Key)).Append('\t').Append(I(kv.Value)).Append('\n');
                if (asymmetricCut) sb.Append("Asymmetric\t1\n");
                if (playInPool.Count > 0)
                    sb.Append("PlayIn\t").Append(I(playInSeats)).Append('\t').Append(I(playInRounds)).Append('\t').Append(string.Join(",", playInPool.Select(I).ToArray()))
                      .Append('\t').Append(playInAbove.Count == 0 ? "-" : string.Join(",", playInAbove.Select(I).ToArray())).Append('\n');
                if (cutSeeds.Count > 0) sb.Append("CutSeeds\t").Append(string.Join(",", cutSeeds.Select(I).ToArray())).Append('\n');
                sb.Append("Format\t").Append(Escape(format)).Append('\n');
                sb.Append("Explore\t").Append(aiExplore ? "1" : "0").Append('\n');
                sb.Append("Style\t").Append(I((int)aiStyle)).Append('\n');
                sb.Append("Finished\t").Append(finished ? "1" : "0").Append('\n');
                sb.Append("Watch\t").Append(watchOne ? "1" : "0").Append('\n');
                sb.Append("Featured\t").Append(I(featuredTable)).Append('\n');
                foreach (var kv in deckTexts) sb.Append("Deck\t").Append(Escape(kv.Key)).Append('\t').Append(Escape(kv.Value)).Append('\n');
                foreach (var e in entrants)
                    sb.Append("Player\t").Append(I(e.Id)).Append('\t').Append(e.Human ? "1" : "0").Append('\t').Append(Escape(e.Name)).Append('\t').Append(Escape(e.Deck))
                      .Append('\t').Append(e.Remote ? "1" : "0").Append('\n');
                foreach (var m in matches)
                {
                    sb.Append("Match\t").Append(I(m.Round)).Append('\t').Append(I(m.Table)).Append('\t').Append(I(m.A)).Append('\t').Append(I(m.B)).Append('\t')
                      .Append(m.Finished ? "1" : "0").Append('\t').Append(m.Games.Count == 0 ? "-" : string.Join(",", m.Games.Select(I).ToArray()));
                    if (m.IsCut) sb.Append('\t').Append(I(m.SeedA)).Append('\t').Append(I(m.SeedB)).Append(m.PlayIn ? "\tP" : "");
                    sb.Append('\n');
                }
                foreach (var m in matches)
                    if (m.ConcededBy >= 0) sb.Append("Conceded\t").Append(I(m.Round)).Append('\t').Append(I(m.Table)).Append('\t').Append(I(m.ConcededBy)).Append('\n');
                return sb.ToString();
            }
        }

        /// <summary>Reads an event saved with Serialize. Throws FormatException if the text isn't one.</summary>
        public static RegionalSim Parse(string text)
        {
            var lines = (text ?? "").Replace("\r", "").Split('\n');
            if (lines.Length == 0 || lines[0].Trim() != FileHeader) throw new FormatException("Not a Regional Sim file.");
            var sim = new RegionalSim();
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.Length == 0) continue;
                var f = line.Split('\t');
                switch (f[0])
                {
                    case "Seed": sim.seed = ParseInt(f, 1); break;
                    case "Rounds": sim.rounds = Math.Max(1, ParseInt(f, 1)); break;
                    case "Round": sim.round = Math.Max(0, ParseInt(f, 1)); break;
                    case "BestOf": sim.bestOfThree = ParseInt(f, 1) == 3; break;
                    case "TopCut": sim.topCut = ParseInt(f, 1); break;
                    case "CutBestOf": sim.cutBestOfThree = ParseInt(f, 1) == 3; break;
                    case "CutSeeds":
                        if (f.Length > 1 && f[1].Length > 0)
                            foreach (var id in f[1].Split(',')) sim.cutSeeds.Add(int.Parse(id, NumberStyles.Integer, CultureInfo.InvariantCulture));
                        break;
                    case "Day2":
                        sim.day1Rounds = Math.Max(0, ParseInt(f, 1));
                        sim.day2Points = Math.Max(0, ParseInt(f, 2));
                        sim.day2Decided = f.Length > 3 && f[3] == "1";
                        break;
                    case "Day2Players":
                        if (f.Length > 1 && f[1].Length > 0)
                            foreach (var id in f[1].Split(',')) sim.day2Players.Add(int.Parse(id, NumberStyles.Integer, CultureInfo.InvariantCulture));
                        break;
                    case "Dropped": sim.dropped[ParseInt(f, 1)] = ParseInt(f, 2); break;
                    case "Asymmetric": sim.asymmetricCut = ParseInt(f, 1) != 0; break;
                    case "PlayIn":
                        if (f.Length < 5) throw new FormatException("Bad play-in line " + (i + 1) + ".");
                        sim.playInSeats = ParseInt(f, 1);
                        sim.playInRounds = ParseInt(f, 2);
                        foreach (var id in f[3].Split(',')) sim.playInPool.Add(int.Parse(id, NumberStyles.Integer, CultureInfo.InvariantCulture));
                        if (f[4] != "-")
                            foreach (var id in f[4].Split(',')) sim.playInAbove.Add(int.Parse(id, NumberStyles.Integer, CultureInfo.InvariantCulture));
                        break;
                    case "Conceded":
                    {
                        int cr = ParseInt(f, 1), ct = ParseInt(f, 2);
                        foreach (var cm in sim.matches)
                            if (cm.Round == cr && cm.Table == ct) cm.ConcededBy = ParseInt(f, 3);
                        break;
                    }
                    case "Format": sim.format = f.Length > 1 ? Unescape(f[1]) : "Standard"; break;
                    case "Explore": sim.aiExplore = ParseInt(f, 1) != 0; break;
                    case "Style": sim.aiStyle = AiStyle.Neural; break; // the Learning and Hybrid AI were removed
                    case "Finished": sim.finished = ParseInt(f, 1) != 0; break;
                    case "Watch": sim.watchOne = ParseInt(f, 1) != 0; break;
                    case "Featured": sim.featuredTable = ParseInt(f, 1); break;
                    case "Deck":
                        if (f.Length >= 3) sim.deckTexts[Unescape(f[1])] = Unescape(f[2]);
                        break;
                    case "Player":
                        if (f.Length < 5) throw new FormatException("Bad player line " + (i + 1) + ".");
                        sim.entrants.Add(new RegionalEntrant { Id = ParseInt(f, 1), Human = f[2] == "1", Name = Unescape(f[3]), Deck = Unescape(f[4]), Remote = f.Length > 5 && f[5] == "1" });
                        break;
                    case "Match":
                    {
                        if (f.Length < 7) throw new FormatException("Bad match line " + (i + 1) + ".");
                        var m = new RegionalMatch { Round = ParseInt(f, 1), Table = ParseInt(f, 2), A = ParseInt(f, 3), B = ParseInt(f, 4), Finished = f[5] == "1" };
                        if (f[6] != "-")
                            foreach (var g in f[6].Split(',')) m.Games.Add(int.Parse(g, NumberStyles.Integer, CultureInfo.InvariantCulture));
                        if (f.Length >= 9)
                        {
                            m.SeedA = ParseInt(f, 7);
                            m.SeedB = ParseInt(f, 8);
                            m.PlayIn = f.Length >= 10 && f[9] == "P";
                        }
                        sim.matches.Add(m);
                        break;
                    }
                }
            }
            for (int k = 0; k < sim.entrants.Count; k++)
                if (sim.entrants[k].Id != k) throw new FormatException("Players are out of order.");
            foreach (var m in sim.matches)
                if (m.A < 0 || m.A >= sim.entrants.Count || m.B >= sim.entrants.Count) throw new FormatException("A match names a player who isn't there.");
            if (sim.entrants.Count < 2) throw new FormatException("Fewer than 2 players.");
            sim.roundStart = sim.matches.Count;
            for (int k = 0; k < sim.matches.Count; k++)
            {
                if (sim.matches[k].Round != sim.round) continue;
                if (k < sim.roundStart) sim.roundStart = k;
                if (!sim.matches[k].Finished && !sim.matches[k].IsBye) sim.roundOpen++;
            }
            if (sim.topCut != 0 && sim.topCut != 2 && sim.topCut != 4 && sim.topCut != 8) throw new FormatException("Bad top cut size.");
            foreach (var id in sim.cutSeeds)
                if (id < 0 || id >= sim.entrants.Count) throw new FormatException("A top cut seed names a player who isn't there.");
            foreach (var id in sim.playInPool.Concat(sim.playInAbove))
                if (id < 0 || id >= sim.entrants.Count) throw new FormatException("A play-in player isn't there.");
            // A round saved just as it ended (it shouldn't happen) moves on now.
            if (sim.round > 0 && sim.roundOpen == 0 && !sim.finished) sim.AdvanceLocked();
            return sim;
        }

        private static int ParseInt(string[] f, int index)
        {
            if (index >= f.Length) throw new FormatException("Missing value.");
            return int.Parse(f[index], NumberStyles.Integer, CultureInfo.InvariantCulture);
        }
    }
}
