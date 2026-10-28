using System;
using System.Collections.Generic;
using System.Threading;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>Progress and results of a DeckTrial, copied for the UI.</summary>
    public sealed class DeckTrialStatus
    {
        public int Done;
        public int Total;
        public int GamesBefore, WinsBefore;
        public int GamesAfter, WinsAfter;
        public int Errors;
        public string LastError;
        public bool Running;
        public bool Finished;
        public bool Cancelled;
        /// <summary>One line per opponent: "vs Dragapult ex: 9/20 before, 12/20 after".</summary>
        public readonly List<string> Lines = new List<string>();

        public double RateBefore { get { return GamesBefore == 0 ? 0 : (double)WinsBefore / GamesBefore; } }
        public double RateAfter { get { return GamesAfter == 0 ? 0 : (double)WinsAfter / GamesAfter; } }
    }

    /// <summary>
    /// Checks a deck change by playing (v75): the deck before and the deck after each play the same games against each
    /// opponent (same seeds, same seats, half going first), so the difference in wins comes from the change rather than luck
    /// of the draw as much as possible. Both sides are played by the classic bot unless an agent factory is given.
    /// Runs on its own thread; Cancel() stops it after the current game.
    /// </summary>
    public sealed class DeckTrial
    {
        private readonly CardScriptRegistry scripts;
        private readonly List<CardDef> before, after;
        private readonly List<AdvisorDeck> opponents;
        private readonly int games;
        private readonly int seedBase;
        private readonly object gate = new object();
        private readonly DeckTrialStatus status = new DeckTrialStatus();
        private volatile bool cancel;
        private Thread thread;

        /// <summary>Makes the agent for a seat: (seat, seed) -> agent. Null: the classic bot.</summary>
        public Func<int, int, IPlayerAgent> AgentFactory;

        public DeckTrial(CardScriptRegistry scripts, List<CardDef> before, List<CardDef> after, IList<AdvisorDeck> opponents, int gamesPerOpponent, int seed)
        {
            this.scripts = scripts;
            this.before = new List<CardDef>(before);
            this.after = new List<CardDef>(after);
            this.opponents = new List<AdvisorDeck>();
            foreach (var o in opponents) if (o != null && o.Cards != null && o.Cards.Count > 0) this.opponents.Add(o);
            games = Math.Max(1, gamesPerOpponent);
            seedBase = seed & 0x0FFFFFFF;
            status.Total = games * this.opponents.Count * 2;
        }

        public DeckTrialStatus Status()
        {
            lock (gate)
            {
                var s = new DeckTrialStatus
                {
                    Done = status.Done, Total = status.Total, GamesBefore = status.GamesBefore, WinsBefore = status.WinsBefore,
                    GamesAfter = status.GamesAfter, WinsAfter = status.WinsAfter, Errors = status.Errors, LastError = status.LastError,
                    Running = status.Running, Finished = status.Finished, Cancelled = status.Cancelled
                };
                s.Lines.AddRange(status.Lines);
                return s;
            }
        }

        public void Start()
        {
            lock (gate) status.Running = true;
            thread = new Thread(() => Run()) { IsBackground = true, Name = "Deck trial" };
            thread.Start();
        }

        /// <summary>Runs on the calling thread (tests).</summary>
        public void RunBlocking()
        {
            lock (gate) status.Running = true;
            Run();
        }

        public void Cancel()
        {
            cancel = true;
        }

        private void Run()
        {
            try
            {
                for (int o = 0; o < opponents.Count && !cancel; o++)
                {
                    var opp = opponents[o];
                    int winsBefore = 0, winsAfter = 0, played = 0;
                    for (int g = 0; g < games && !cancel; g++)
                    {
                        int seed = seedBase + o * 1009 + g;
                        int seat = g % 2;
                        var pairStart = DateTime.UtcNow;
                        int b = Play(before, opp.Cards, seat, seed);
                        int a = Play(after, opp.Cards, seat, seed);
                        lock (gate)
                        {
                            status.Done += 2;
                            if (b >= 0)
                            {
                                status.GamesBefore++;
                                status.WinsBefore += b;
                            }
                            if (a >= 0)
                            {
                                status.GamesAfter++;
                                status.WinsAfter += a;
                            }
                        }
                        if (b > 0) winsBefore++;
                        if (a > 0) winsAfter++;
                        played++;
                        WorkThrottle.Rest(DateTime.UtcNow - pairStart, () => cancel); // v120: the battery saver
                    }
                    lock (gate)
                        status.Lines.Add("vs " + opp.Name + ": " + winsBefore + "/" + played + " before, " + winsAfter + "/" + played + " after");
                }
            }
            finally
            {
                lock (gate)
                {
                    status.Running = false;
                    status.Finished = true;
                    status.Cancelled = cancel;
                }
            }
        }

        /// <summary>One game with the deck in the given seat: 1 win, 0 loss or draw, -1 error.</summary>
        private int Play(List<CardDef> deck, List<CardDef> opponent, int seat, int seed)
        {
            try
            {
                var a0 = MakeAgent(0, seed * 2 + 1);
                var a1 = MakeAgent(1, seed * 2 + 2);
                var engine = new GameEngine(scripts, a0, a1);
                engine.CardsPerPlayer = deck.Count == opponent.Count ? deck.Count : 0;
                var result = seat == 0 ? engine.Run(deck, opponent, seed) : engine.Run(opponent, deck, seed);
                return result.Winner == seat ? 1 : 0;
            }
            catch (Exception e)
            {
                lock (gate)
                {
                    status.Errors++;
                    status.LastError = "seed " + seed + ": " + e.GetType().Name + ": " + e.Message;
                }
                return -1;
            }
        }

        private IPlayerAgent MakeAgent(int seat, int seed)
        {
            var factory = AgentFactory;
            return factory != null ? factory(seat, seed) : new HeuristicAgent(seed);
        }
    }
}
