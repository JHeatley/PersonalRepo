using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>Settings for turning a video's snapshots into events.</summary>
    public sealed class VideoTimelineOptions
    {
        /// <summary>A new value counts once it's been read this many times in a row (one misread doesn't change the board).</summary>
        public int Confirm = 2;
        /// <summary>Changes less than this many seconds apart are read as one (an attack, its Knock Out and the Prize cards).</summary>
        public double Window = 3.0;
        /// <summary>The player who went first, if known (-1: worked out from the turn counters, if the layout has them).</summary>
        public int FirstPlayer = -1;
        /// <summary>The first turn's number when the video doesn't show turn numbers (0: 3, so no first-turn limits get in the way).</summary>
        public int StartTurn;
        /// <summary>How long after both Active Pokémon are read to wait for the rest of the board before the setup is written.</summary>
        public double SetupWait = 8.0;
    }

    /// <summary>
    /// Turns the snapshots read from a video into events (v77). Each value (a name, HP, Prize cards left...) changes only
    /// once it's been read the same way twice in a row. The first full board is the setup (Pokémon in play, damage, Prize
    /// cards already taken). After that, changes close together are compared as one: Pokémon that appear (played to the
    /// Bench), change into an evolution, swap places (switching, retreating, promoting), lose HP (an attack by the Active
    /// Pokémon of the player whose turn it is, the attack worked out from the damage), get HP back, leave play (Knocked
    /// Out), Prize cards taken, the Stadium. Turns come from the turn counters, the clocks (the one running) or the turn
    /// banner; with none of those, each attack starts its player's turn.
    /// </summary>
    public sealed class VideoTimeline
    {
        private sealed class Place
        {
            /// <summary>null: never read; "": empty.</summary>
            public string Name;
            public int Hp = -1, Max = -1;
        }

        private sealed class State
        {
            public readonly Place[] Active = { new Place(), new Place() };
            public readonly Place[][] Bench = { new Place[0], new Place[0] };
            public readonly int[] Prizes = { -1, -1 };
            public string Stadium;
        }

        private sealed class Field
        {
            public string Candidate;
            public int Count;
            public string Stable;
        }

        private sealed class Mon
        {
            public string Name;
            public int Hp = -1, Max = -1;
            public bool Active;
            public int Slot;
            public Mon Partner;
            public bool Evolved;
        }

        private sealed class Hit
        {
            public int Owner;
            public string Name;
            public int Counters;
            public bool WasActive;
            public bool KnockedOut;
            public int HpLeft = -1;
        }

        private readonly CardDatabase db;
        private readonly VideoTimelineOptions options;
        private readonly Dictionary<string, Field> fields = new Dictionary<string, Field>(StringComparer.Ordinal);
        private readonly Dictionary<string, List<CardDef>> defsByName = new Dictionary<string, List<CardDef>>(StringComparer.Ordinal);
        private readonly List<VideoEvent> events = new List<VideoEvent>();
        private readonly List<KeyValuePair<double, int>> turns = new List<KeyValuePair<double, int>>();
        private readonly int[] benchSlots = new int[2];
        private int first = -1;
        private int lastTurnNumber;
        private int lastTurnPlayer = -1;
        private bool turnSignals;
        private readonly int[] lastClock = { -1, -1 };
        private int clockCandidate = -1, clockCount, clockPlayer = -1;
        private readonly bool[] won = new bool[2];

        private VideoTimeline(CardDatabase db, VideoTimelineOptions options)
        {
            this.db = db;
            this.options = options ?? new VideoTimelineOptions();
            first = this.options.FirstPlayer;
            if (db != null)
                foreach (var d in db.Cards)
                {
                    List<CardDef> list;
                    if (!defsByName.TryGetValue(d.Name, out list))
                    {
                        list = new List<CardDef>();
                        defsByName[d.Name] = list;
                    }
                    list.Add(d);
                }
        }

        /// <summary>The events of a video, in time order (setup events first). firstPlayer: who went first, if it could be told (-1 otherwise).</summary>
        public static List<VideoEvent> Build(IList<VideoSnapshot> snapshots, CardDatabase db, VideoTimelineOptions options, out int firstPlayer)
        {
            var t = new VideoTimeline(db, options);
            t.Run(snapshots ?? new List<VideoSnapshot>());
            firstPlayer = t.first;
            var ordered = t.events.Where(e => e.Setup).ToList();
            ordered.AddRange(t.events.Where(e => !e.Setup).Select((e, i) => new KeyValuePair<int, VideoEvent>(i, e))
                .OrderBy(kv => kv.Value.Time).ThenBy(kv => kv.Key).Select(kv => kv.Value));
            return ordered;
        }

        // ------------------------------------------------------------------ Reading the snapshots

        private void Run(IList<VideoSnapshot> snapshots)
        {
            foreach (var s in snapshots)
                for (int p = 0; p < 2; p++) benchSlots[p] = Math.Max(benchSlots[p], s.Sides[p].Bench.Length);
            State committed = null, previous = null;
            double setupFrom = -1, windowStart = -1, lastChange = -1;
            foreach (var snap in snapshots)
            {
                Feed(snap);
                var state = Current();
                double time = snap.Time;
                TurnSignals(snap, time);
                if (committed == null)
                {
                    bool activesKnown = Known(state.Active[0]) && Known(state.Active[1]) && state.Active[0].Name.Length > 0 && state.Active[1].Name.Length > 0;
                    if (!activesKnown) continue;
                    if (setupFrom < 0) setupFrom = time;
                    if (!AllKnown(state) && time - setupFrom < options.SetupWait) continue;
                    committed = state;
                    previous = state;
                    WriteSetup(state, time);
                    continue;
                }
                if (!Same(state, previous))
                {
                    if (windowStart < 0) windowStart = time;
                    lastChange = time;
                }
                if (windowStart >= 0 && time - lastChange >= options.Window)
                {
                    Diff(committed, state, windowStart);
                    committed = state;
                    windowStart = -1;
                }
                previous = state;
            }
            if (committed != null && windowStart >= 0) Diff(committed, previous, windowStart);
        }

        private static bool Known(Place p)
        {
            return p != null && p.Name != null;
        }

        private bool AllKnown(State s)
        {
            for (int p = 0; p < 2; p++)
                foreach (var b in s.Bench[p])
                    if (!Known(b)) return false;
            return true;
        }

        private void Feed(VideoSnapshot snap)
        {
            for (int p = 0; p < 2; p++)
            {
                var side = snap.Sides[p];
                FeedMon("a." + p, side.Active);
                for (int s = 0; s < side.Bench.Length; s++) FeedMon("b." + p + "." + s, side.Bench[s]);
                if (side.Prizes >= 0) FeedValue("prizes." + p, side.Prizes.ToString(CultureInfo.InvariantCulture));
                if (side.Turns >= 0) FeedValue("turns." + p, side.Turns.ToString(CultureInfo.InvariantCulture));
            }
            if (snap.Stadium != null) FeedValue("stadium", snap.Stadium);
        }

        private void FeedMon(string key, MonReading m)
        {
            if (m == null) return;
            if (m.Empty)
            {
                FeedValue(key, "");
                FeedValue(key + ".hp", "");
                return;
            }
            if (m.Name == null) return;
            FeedValue(key, m.Name);
            if (m.Hp >= 0) FeedValue(key + ".hp", m.Hp.ToString(CultureInfo.InvariantCulture) + "/" + m.MaxHp.ToString(CultureInfo.InvariantCulture));
        }

        private void FeedValue(string key, string value)
        {
            Field f;
            if (!fields.TryGetValue(key, out f))
            {
                f = new Field();
                fields[key] = f;
            }
            if (value == f.Candidate) f.Count++;
            else
            {
                f.Candidate = value;
                f.Count = 1;
            }
            if (f.Count >= Math.Max(1, options.Confirm)) f.Stable = value;
        }

        private string StableOf(string key)
        {
            Field f;
            return fields.TryGetValue(key, out f) ? f.Stable : null;
        }

        private State Current()
        {
            var s = new State();
            for (int p = 0; p < 2; p++)
            {
                s.Active[p] = PlaceOf("a." + p);
                s.Bench[p] = new Place[benchSlots[p]];
                for (int b = 0; b < benchSlots[p]; b++) s.Bench[p][b] = PlaceOf("b." + p + "." + b);
                s.Prizes[p] = IntOf("prizes." + p);
            }
            s.Stadium = StableOf("stadium");
            return s;
        }

        private Place PlaceOf(string key)
        {
            var place = new Place { Name = StableOf(key) };
            string hp = StableOf(key + ".hp");
            if (!string.IsNullOrEmpty(hp) && !string.IsNullOrEmpty(place.Name))
            {
                var parts = hp.Split('/');
                int a, b;
                if (parts.Length == 2 && int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out a)
                    && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out b))
                {
                    place.Hp = a;
                    place.Max = b;
                }
            }
            return place;
        }

        private int IntOf(string key)
        {
            string v = StableOf(key);
            int n;
            return v != null && int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : -1;
        }

        private static bool Same(State a, State b)
        {
            if (a == null || b == null) return a == b;
            for (int p = 0; p < 2; p++)
            {
                if (!SamePlace(a.Active[p], b.Active[p]) || a.Prizes[p] != b.Prizes[p] || a.Bench[p].Length != b.Bench[p].Length) return false;
                for (int s = 0; s < a.Bench[p].Length; s++) if (!SamePlace(a.Bench[p][s], b.Bench[p][s])) return false;
            }
            return a.Stadium == b.Stadium;
        }

        private static bool SamePlace(Place a, Place b)
        {
            return a.Name == b.Name && a.Hp == b.Hp && a.Max == b.Max;
        }

        // ------------------------------------------------------------------ Turns

        private int CurrentAt(double time)
        {
            int player = -1;
            foreach (var kv in turns)
            {
                if (kv.Key > time) break;
                player = kv.Value;
            }
            return player;
        }

        private void StartTurn(int player, int number, double time)
        {
            if (player < 0) return;
            if (player == lastTurnPlayer && (number <= 0 || number == lastTurnNumber)) return;
            if (number <= 0) number = lastTurnNumber > 0 ? lastTurnNumber + 1 : (options.StartTurn > 0 ? options.StartTurn : 3);
            // Turn numbers alternate: keep the parity right for who's playing (odd turns are the first player's).
            if (first >= 0 && (number % 2 == 1) != (player == first)) number++;
            lastTurnPlayer = player;
            lastTurnNumber = number;
            turns.Add(new KeyValuePair<double, int>(time, player));
            events.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Turn, Player = player, Amount = number });
        }

        private void TurnSignals(VideoSnapshot snap, double time)
        {
            // Turn counters (a player's count goes up when their turn starts).
            int c0 = IntOf("turns.0"), c1 = IntOf("turns.1");
            if (c0 > 0 && c1 > 0)
            {
                turnSignals = true;
                if (first < 0 && c0 != c1) first = c0 > c1 ? 0 : 1;
                if (first >= 0)
                {
                    int second = 1 - first;
                    int cf = first == 0 ? c0 : c1, cs = first == 0 ? c1 : c0;
                    int player = cf > cs ? first : second;
                    int number = player == first ? 2 * cf - 1 : 2 * cs;
                    if (player != lastTurnPlayer || number != lastTurnNumber) StartTurn(player, number, time);
                }
            }
            // Clocks: the one running is the player whose turn it is.
            int running = -1;
            bool[] down = new bool[2];
            bool anyClock = false;
            for (int p = 0; p < 2; p++)
            {
                int c = snap.Sides[p].Clock;
                if (c >= 0)
                {
                    anyClock = true;
                    if (lastClock[p] >= 0 && c < lastClock[p] && lastClock[p] - c < 120) down[p] = true;
                    lastClock[p] = c;
                }
            }
            if (anyClock)
            {
                if (down[0] && !down[1]) running = 0;
                else if (down[1] && !down[0]) running = 1;
                if (running >= 0)
                {
                    if (running == clockCandidate) clockCount++;
                    else
                    {
                        clockCandidate = running;
                        clockCount = 1;
                    }
                    if (clockCount >= 2 && clockPlayer != running)
                    {
                        clockPlayer = running;
                        turnSignals = true;
                        if (lastTurnPlayer != running) StartTurn(running, 0, time);
                    }
                }
            }
            // The turn banner.
            if (snap.Banner >= 0)
            {
                turnSignals = true;
                if (lastTurnPlayer != snap.Banner) StartTurn(snap.Banner, 0, time);
            }
        }

        // ------------------------------------------------------------------ Setup

        private void WriteSetup(State s, double time)
        {
            for (int p = 0; p < 2; p++)
            {
                AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Bench, Player = p, Card = s.Active[p].Name, Spot = "Active Spot" });
                foreach (var b in s.Bench[p])
                    if (!string.IsNullOrEmpty(b.Name)) AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Bench, Player = p, Card = b.Name, Spot = "Bench" });
            }
            for (int p = 0; p < 2; p++)
            {
                int c = Counters(s.Active[p]);
                if (c > 0) AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Counters, Player = p, Target = p, Card2 = s.Active[p].Name, Amount = c });
                foreach (var b in s.Bench[p])
                {
                    int cb = Counters(b);
                    if (cb > 0) AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Counters, Player = p, Target = p, Card2 = b.Name, Amount = cb });
                }
                if (s.Prizes[p] >= 0 && s.Prizes[p] < 6)
                    AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Prizes, Player = p, Amount = 6 - s.Prizes[p] });
            }
            if (!string.IsNullOrEmpty(s.Stadium))
                AddSetup(new VideoEvent { Time = time, Kind = VideoEventKind.Stadium, Player = 0, Card = s.Stadium, Note = "in play when the video starts" });
        }

        private void AddSetup(VideoEvent e)
        {
            e.Setup = true;
            events.Add(e);
        }

        /// <summary>Damage counters on a Pokémon: from its HP and max HP, or the card's HP when only the HP shows. -1 when unknown.</summary>
        private int Counters(Place p)
        {
            if (p == null || string.IsNullOrEmpty(p.Name) || p.Hp < 0) return -1;
            return Counters(p.Name, p.Hp, p.Max);
        }

        private int Counters(string name, int hp, int max)
        {
            if (hp < 0) return -1;
            if (max <= 0) max = CardHp(name);
            if (max <= 0) return -1;
            return Math.Max(0, (max - hp) / 10);
        }

        private int CardHp(string name)
        {
            List<CardDef> list;
            if (name == null || !defsByName.TryGetValue(name, out list)) return -1;
            int hp = -1;
            foreach (var d in list) if (d.Hp > 0 && (hp < 0 || d.Hp == hp)) hp = d.Hp;
            foreach (var d in list) if (d.Hp > 0 && d.Hp != hp) return -1; // printings differ: can't tell
            return hp;
        }

        private List<CardDef> Defs(string name)
        {
            List<CardDef> list;
            return name != null && defsByName.TryGetValue(name, out list) ? list : new List<CardDef>();
        }

        /// <summary>True when a card of name "to" evolves from "from" (or from what "from" evolves into, for Rare Candy).</summary>
        private bool EvolvesInto(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to) || from == to) return false;
            foreach (var d in Defs(to))
            {
                if (!d.IsPokemon) continue;
                if (d.EvolvesFrom == from) return true;
                foreach (var mid in Defs(d.EvolvesFrom))
                    if (mid.IsPokemon && mid.EvolvesFrom == from) return true;
                // VMAX / VSTAR evolve from the Pokémon V of the same species.
                if (d.EvolvesFrom.Length > 0 && from.StartsWith(d.EvolvesFrom, StringComparison.Ordinal) && from.EndsWith(" V", StringComparison.Ordinal)) return true;
            }
            return false;
        }

        // ------------------------------------------------------------------ Comparing two boards

        private void Diff(State a, State b, double time)
        {
            int current = CurrentAt(time);
            var pre = new List<VideoEvent>();
            var heals = new List<VideoEvent>();
            var hits = new List<Hit>();
            var knockOuts = new List<VideoEvent>();
            var post = new List<VideoEvent>();
            var prizes = new List<VideoEvent>();
            for (int q = 0; q < 2; q++) DiffSide(q, a, b, time, pre, heals, hits, knockOuts, post);

            for (int q = 0; q < 2; q++)
                if (a.Prizes[q] >= 0 && b.Prizes[q] >= 0 && b.Prizes[q] < a.Prizes[q])
                    prizes.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Prizes, Player = q, Amount = a.Prizes[q] - b.Prizes[q] });
            // A Pokémon that left play while the other player took Prize cards was Knocked Out; otherwise it's a guess.
            foreach (var ko in knockOuts)
            {
                bool prizeTaken = prizes.Any(pz => pz.Player == 1 - ko.Player);
                if (!prizeTaken)
                {
                    ko.Sure = false;
                    ko.Note = "left play: a Knock Out, or picked up";
                }
            }

            if (b.Stadium != null && b.Stadium.Length > 0 && b.Stadium != (a.Stadium ?? ""))
                pre.Insert(0, new VideoEvent { Time = time, Kind = VideoEventKind.Stadium, Player = current >= 0 ? current : 0, Card = b.Stadium, Sure = current >= 0 });

            // Damage: an attack by the Active Pokémon of the player whose turn it is.
            var attackEvents = new List<VideoEvent>();
            if (hits.Count > 0)
            {
                int attacker = current;
                var sides = hits.Select(hit => hit.Owner).Distinct().ToList();
                if (!turnSignals && sides.Count == 1 && sides[0] != 1 - current)
                {
                    // No turns shown: the damage says whose turn it is.
                    attacker = 1 - sides[0];
                    StartTurn(attacker, 0, time - 0.001);
                }
                if (attacker >= 0) attackEvents.AddRange(Attack(attacker, a, b, hits, time));
                else
                    foreach (var h in hits)
                        if (h.Counters > 0) attackEvents.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Counters, Player = 1 - h.Owner, Target = h.Owner, Card2 = h.Name, Amount = h.Counters, Sure = false, Note = "whose turn it was isn't known" });
            }

            foreach (var list in new[] { pre, heals, attackEvents, knockOuts, prizes, post })
                foreach (var e in list) events.Add(e);
            for (int q = 0; q < 2; q++)
                if (!won[q] && b.Prizes[q] == 0 && a.Prizes[q] != 0)
                {
                    won[q] = true;
                    events.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Win, Player = q });
                }
        }

        private void DiffSide(int q, State a, State b, double time, List<VideoEvent> pre, List<VideoEvent> heals, List<Hit> hits, List<VideoEvent> knockOuts, List<VideoEvent> post)
        {
            var before = Mons(a, q);
            var after = Mons(b, q);
            Mon pa = before.FirstOrDefault(x => x.Active), na = after.FirstOrDefault(x => x.Active);
            // 1. Same Pokémon: the Active one, the same Bench place, then the same name anywhere.
            if (pa != null && na != null && pa.Name == na.Name) Pair(pa, na, false);
            foreach (var m in before.Where(x => !x.Active && x.Partner == null))
            {
                var n = after.FirstOrDefault(x => !x.Active && x.Partner == null && x.Slot == m.Slot && x.Name == m.Name);
                if (n != null) Pair(m, n, false);
            }
            foreach (var m in before.Where(x => !x.Active && x.Partner == null))
            {
                var n = after.FirstOrDefault(x => !x.Active && x.Partner == null && x.Name == m.Name);
                if (n != null) Pair(m, n, false);
            }
            // 2. Evolutions: the Active Pokémon (unless what it turned into came from the Bench), then the Bench.
            if (pa != null && na != null && pa.Partner == null && na.Partner == null && EvolvesInto(pa.Name, na.Name)
                && !before.Any(x => !x.Active && x.Partner == null && x.Name == na.Name))
                Pair(pa, na, true);
            foreach (var m in before.Where(x => !x.Active && x.Partner == null))
            {
                var n = after.FirstOrDefault(x => !x.Active && x.Partner == null && x.Slot == m.Slot && EvolvesInto(m.Name, x.Name))
                        ?? after.FirstOrDefault(x => !x.Active && x.Partner == null && EvolvesInto(m.Name, x.Name));
                if (n != null) Pair(m, n, true);
            }
            // 3. Swaps: the new Active Pokémon came from the Bench; the old one went to the Bench.
            Mon promoted = null;
            if (na != null && na.Partner == null)
            {
                promoted = before.FirstOrDefault(x => !x.Active && x.Partner == null && x.Name == na.Name);
                if (promoted != null) Pair(promoted, na, false);
            }
            if (pa != null && pa.Partner == null)
            {
                var benched = after.FirstOrDefault(x => !x.Active && x.Partner == null && x.Name == pa.Name);
                if (benched != null) Pair(pa, benched, false);
            }
            bool activeLeft = pa != null && pa.Partner == null;

            foreach (var m in before)
            {
                if (m.Partner == null) continue;
                var n = m.Partner;
                if (n.Evolved)
                    pre.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Evolve, Player = q, Card = m.Name, Card2 = n.Name, Spot = m.Active ? "Active Spot" : "Bench" });
                int c0 = Counters(m.Name, m.Hp, m.Max), c1 = Counters(n.Name, n.Hp, n.Max);
                if (c0 >= 0 && c1 >= 0 && c1 != c0)
                {
                    if (c1 > c0) hits.Add(new Hit { Owner = q, Name = n.Name, Counters = c1 - c0, WasActive = m.Active });
                    else heals.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Heal, Player = q, Card = n.Name, Amount = (c0 - c1) * 10 });
                }
            }
            // Pokémon that left play.
            foreach (var m in before.Where(x => x.Partner == null))
            {
                knockOuts.Add(new VideoEvent { Time = time, Kind = VideoEventKind.KnockOut, Player = q, Card = m.Name });
                hits.Add(new Hit { Owner = q, Name = m.Name, KnockedOut = true, WasActive = m.Active, HpLeft = m.Hp >= 0 ? m.Hp : CardHp(m.Name) - Math.Max(0, Counters(m.Name, m.Hp, m.Max)) * 10 });
            }
            // Pokémon put into play.
            foreach (var n in after.Where(x => x.Partner == null))
            {
                pre.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Bench, Player = q, Card = n.Name, Spot = "Bench" });
                if (n.Active)
                {
                    var e = new VideoEvent { Time = time, Kind = VideoEventKind.Active, Player = q, Card = n.Name, Sure = false, Note = "it wasn't seen on the Bench" };
                    if (activeLeft) post.Add(e);
                    else pre.Add(e);
                }
            }
            if (promoted != null)
            {
                var e = new VideoEvent { Time = time, Kind = VideoEventKind.Active, Player = q, Card = na.Name };
                if (activeLeft) post.Add(e);
                else pre.Add(e);
            }
        }

        private static void Pair(Mon before, Mon after, bool evolved)
        {
            before.Partner = after;
            after.Partner = before;
            after.Evolved = evolved;
        }

        private static List<Mon> Mons(State s, int q)
        {
            var list = new List<Mon>();
            var a = s.Active[q];
            if (!string.IsNullOrEmpty(a.Name)) list.Add(new Mon { Name = a.Name, Hp = a.Hp, Max = a.Max, Active = true, Slot = 0 });
            for (int i = 0; i < s.Bench[q].Length; i++)
            {
                var b = s.Bench[q][i];
                if (!string.IsNullOrEmpty(b.Name)) list.Add(new Mon { Name = b.Name, Hp = b.Hp, Max = b.Max, Slot = i + 1 });
            }
            return list;
        }

        // ------------------------------------------------------------------ Attacks

        private List<VideoEvent> Attack(int attacker, State a, State b, List<Hit> hits, double time)
        {
            var result = new List<VideoEvent>();
            int defender = 1 - attacker;
            var onThem = hits.Where(x => x.Owner == defender).ToList();
            string user = !string.IsNullOrEmpty(b.Active[attacker].Name) ? b.Active[attacker].Name : a.Active[attacker].Name;
            Hit main = onThem.FirstOrDefault(x => x.WasActive) ?? onThem.FirstOrDefault();
            if (main != null && !string.IsNullOrEmpty(user))
            {
                int damage = main.KnockedOut ? Math.Max(0, main.HpLeft) : main.Counters * 10;
                var e = new VideoEvent { Time = time, Kind = VideoEventKind.Attack, Player = attacker, Target = defender, Card = user, Card2 = main.Name, Amount = damage };
                GuessAttack(Defs(user), Defs(main.Name), damage, main.KnockedOut, e);
                if (main.KnockedOut) e.Note = "Knocked Out: at least " + damage.ToString(CultureInfo.InvariantCulture) + " damage";
                result.Add(e);
            }
            foreach (var h in hits)
            {
                if (h == main || h.Counters <= 0) continue;
                result.Add(new VideoEvent { Time = time, Kind = VideoEventKind.Counters, Player = attacker, Target = h.Owner, Card2 = h.Name, Amount = h.Counters });
            }
            return result;
        }

        /// <summary>
        /// Which of the attacker's attacks did this damage (Weakness and Resistance counted). Sure when exactly one attack fits;
        /// otherwise the likeliest is kept as a suggestion and the event asks which it was.
        /// </summary>
        public static void GuessAttack(List<CardDef> attackers, List<CardDef> defenders, int damage, bool knockedOut, VideoEvent e)
        {
            var options = new List<AttackDef>();
            var seen = new HashSet<string>(StringComparer.Ordinal);
            CardDef attackerDef = null;
            foreach (var d in attackers)
            {
                if (attackerDef == null && d.IsPokemon) attackerDef = d;
                foreach (var atk in d.Attacks)
                    if (Damaging(atk) && seen.Add(atk.Name)) options.Add(atk);
            }
            CardDef defenderDef = defenders.FirstOrDefault(x => x.IsPokemon);
            e.Choices = string.Join(", ", options.Select(x => x.Name).ToArray());
            AttackDef best = null;
            int bestScore = 0, fitting = 0;
            foreach (var atk in options)
            {
                int score = Score(atk, attackerDef, defenderDef, damage, knockedOut);
                if (score >= 2) fitting++;
                if (score > bestScore)
                {
                    bestScore = score;
                    best = atk;
                }
            }
            if (best != null && bestScore >= 2)
            {
                e.Attack = best.Name;
                e.Sure = fitting == 1;
            }
            else if (options.Count == 1)
            {
                e.Attack = options[0].Name;
                e.Sure = false;
            }
            else
            {
                e.Attack = "";
                e.Sure = false;
            }
        }

        private static bool Damaging(AttackDef a)
        {
            return a.Damage > 0 || !string.IsNullOrEmpty(a.DamageSuffix) || (a.Text ?? "").IndexOf("damage", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static int Score(AttackDef a, CardDef attacker, CardDef defender, int damage, bool knockedOut)
        {
            int printed = a.Damage;
            bool more = a.DamageSuffix == "+" || a.DamageSuffix == "x" || a.DamageSuffix == "×";
            if (printed <= 0) return more ? 2 : 1;
            int expected = printed;
            if (attacker != null && defender != null)
            {
                if (defender.Weakness.HasValue && defender.Weakness.Value == attacker.PokemonType)
                    expected = defender.WeaknessAmount > 0 ? expected + defender.WeaknessAmount : expected * 2;
                if (defender.Resistance.HasValue && defender.Resistance.Value == attacker.PokemonType)
                    expected = Math.Max(0, expected - defender.ResistanceAmount);
            }
            if (a.DamageSuffix == "-") return damage <= expected && damage > 0 ? 2 : 0;
            if (knockedOut) return expected >= damage || more ? 2 : 0;
            if (expected == damage) return 3;
            if (more && damage >= expected) return 2;
            return 0;
        }
    }
}
