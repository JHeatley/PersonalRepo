using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One past version of the network in the league: a champion it replaced, a snapshot, or an exploiter.</summary>
    public sealed class LeagueMember
    {
        public int Id;
        /// <summary>"champion", "snapshot" or "exploiter".</summary>
        public string Kind = "";
        public string Name = "";
        /// <summary>The brain's training games when it joined.</summary>
        public int AddedAt;
        public PolicyNet Net;
        /// <summary>
        /// The network's recent results against it: games and points (a win 1, a draw half). They fade (League.Fade per game), so
        /// they follow the network as it changes.
        /// </summary>
        public double Games, Points;

        /// <summary>The network's share of the points against it lately (0.5 before any games).</summary>
        public double WinRate { get { return (Points + 1) / (Games + 2); } }

        /// <summary>
        /// How often it's picked as an opponent: the ones the network does worst against most (prioritized fictitious self-play),
        /// never none at all.
        /// </summary>
        public double Weight
        {
            get
            {
                double lose = 1 - WinRate;
                return lose * lose + 0.02;
            }
        }

        public LeagueMember Copy()
        {
            return (LeagueMember)MemberwiseClone();
        }
    }

    /// <summary>
    /// The league (expert roadmap step 6, training against more than itself): past versions of the network kept with the AI,
    /// that training plays against as well as itself. Playing only itself, a network gets good at beating itself and can forget
    /// how to beat other styles; past versions remind it, and exploiters (copies trained only to beat the current network, kept
    /// when they find a weakness) show it where it's weak. The opponents it does worst against are picked most. At most Capacity
    /// members; when there's no room, the one it beats most easily leaves. The networks are kept next to the AI's file
    /// (LearningBrain), their results in the brain's file. Thread-safe.
    /// </summary>
    public sealed class League
    {
        public const int Capacity = 8;
        public const double Fade = 0.998;
        /// <summary>Games a member needs before it can be the one to leave because it's easy.</summary>
        public const int MinGamesToDrop = 50;

        private readonly object sync = new object();
        private readonly List<LeagueMember> members = new List<LeagueMember>();
        /// <summary>Every member's network seen while the app runs, so "Undo this session" can bring back one that left.</summary>
        private readonly Dictionary<int, PolicyNet> nets = new Dictionary<int, PolicyNet>();
        private readonly List<string> events = new List<string>();
        private int nextId = 1;

        /// <summary>The brain's training games at the last exploiter, and exploiters trained so far.</summary>
        public int LastExploiterAt, ExploitersTried;

        public int Count { get { lock (sync) return members.Count; } }

        /// <summary>The members (copies, for showing), oldest first.</summary>
        public List<LeagueMember> Members()
        {
            lock (sync)
            {
                var list = new List<LeagueMember>();
                foreach (var m in members) list.Add(m.Copy());
                return list;
            }
        }

        /// <summary>What happened in the league, newest last (members joining and leaving, exploiters).</summary>
        public List<string> Events()
        {
            lock (sync) return new List<string>(events);
        }

        /// <summary>The training games when the newest member joined (0 with none).</summary>
        public int NewestAddedAt
        {
            get
            {
                lock (sync)
                {
                    int at = 0;
                    foreach (var m in members) at = Math.Max(at, m.AddedAt);
                    return at;
                }
            }
        }

        /// <summary>A share of the picks for each member (for showing), in Members() order.</summary>
        public static double[] PickShares(List<LeagueMember> list)
        {
            double total = 0;
            foreach (var m in list) if (m.Net != null) total += m.Weight;
            var shares = new double[list.Count];
            for (int i = 0; i < list.Count; i++) shares[i] = total > 0 && list[i].Net != null ? list[i].Weight / total : 0;
            return shares;
        }

        /// <summary>An opponent for a training game, the ones it does worst against most often; null when there are none.</summary>
        public LeagueMember Pick(Rng rng)
        {
            lock (sync)
            {
                double total = 0;
                foreach (var m in members) if (m.Net != null) total += m.Weight;
                if (total <= 0) return null;
                double u = rng.Next(1000000) / 1000000.0 * total;
                LeagueMember last = null;
                foreach (var m in members)
                {
                    if (m.Net == null) continue;
                    last = m;
                    u -= m.Weight;
                    if (u < 0) return m;
                }
                return last;
            }
        }

        /// <summary>A training game's result against member id: points for the network (1 win, 0.5 draw, 0 loss).</summary>
        public void Record(int id, double points)
        {
            lock (sync)
            {
                foreach (var m in members)
                {
                    if (m.Id != id) continue;
                    m.Games = m.Games * Fade + 1;
                    m.Points = m.Points * Fade + points;
                    return;
                }
            }
        }

        /// <summary>
        /// A new member (net is kept as it is: pass a copy). When the league is full, the member the network beats most easily
        /// (once it has played it MinGamesToDrop games; otherwise the oldest) leaves.
        /// </summary>
        public LeagueMember Add(PolicyNet net, string kind, string name, int addedAt, string when)
        {
            lock (sync)
            {
                var m = new LeagueMember { Id = nextId++, Kind = kind ?? "", Name = name ?? "", AddedAt = addedAt, Net = net };
                members.Add(m);
                nets[m.Id] = net;
                Event(when, "At " + N(addedAt) + " games: " + m.Name + " joined the league.");
                while (members.Count > Capacity)
                {
                    LeagueMember leave = null;
                    foreach (var o in members)
                        if (o != m && o.Games >= MinGamesToDrop && (leave == null || o.WinRate > leave.WinRate)) leave = o;
                    if (leave == null) leave = members[0] != m ? members[0] : members[1];
                    members.Remove(leave);
                    Event(when, "At " + N(addedAt) + " games: " + leave.Name + " left (it scored " + Math.Round(leave.WinRate * 100)
                                + "% against it lately, the easiest of the league).");
                }
                return m;
            }
        }

        /// <summary>A line for the league's history (an exploiter's result, for one).</summary>
        public void Note(string when, string text)
        {
            lock (sync) Event(when, text);
        }

        private void Event(string when, string text)
        {
            events.Add((string.IsNullOrEmpty(when) ? "" : when + "  ") + text);
            while (events.Count > 100) events.RemoveAt(0);
        }

        private static string N(int n)
        {
            return n.ToString("N0", CultureInfo.InvariantCulture);
        }

        public void Clear()
        {
            lock (sync)
            {
                members.Clear();
                events.Clear();
                LastExploiterAt = 0;
                ExploitersTried = 0;
                // nextId keeps counting, so a new member never takes an old one's file.
            }
        }

        // ------------------------------------------------------------------ Saving (LearningBrain)

        /// <summary>
        /// The brain's text lines: lgn nextId lastExploiterAt exploitersTried; lg id kind addedAt games points name (a member;
        /// its network is in its own file); lgh event.
        /// </summary>
        internal void Write(StringBuilder sb)
        {
            lock (sync)
            {
                if (members.Count == 0 && events.Count == 0 && ExploitersTried == 0 && nextId == 1) return;
                sb.Append("lgn\t").Append(nextId).Append('\t').Append(LastExploiterAt).Append('\t').Append(ExploitersTried).Append('\n');
                foreach (var m in members)
                    sb.Append("lg\t").Append(m.Id).Append('\t').Append(Clean(m.Kind)).Append('\t').Append(m.AddedAt).Append('\t')
                      .Append(m.Games.ToString("R", CultureInfo.InvariantCulture)).Append('\t').Append(m.Points.ToString("R", CultureInfo.InvariantCulture))
                      .Append('\t').Append(Clean(m.Name)).Append('\n');
                foreach (var e in events) sb.Append("lgh\t").Append(Clean(e)).Append('\n');
            }
        }

        private static string Clean(string s)
        {
            return (s ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }

        /// <summary>Before reading the brain's text: no members (the networks seen stay remembered, for Undo).</summary>
        internal void BeginLoad()
        {
            lock (sync)
            {
                members.Clear();
                events.Clear();
                LastExploiterAt = 0;
                ExploitersTried = 0;
            }
        }

        /// <summary>One of Write's lines (p: split on tabs). False when it isn't a league line.</summary>
        internal bool LoadLine(string[] p)
        {
            lock (sync)
            {
                switch (p[0])
                {
                    case "lgn":
                        if (p.Length >= 4)
                        {
                            nextId = Math.Max(nextId, int.Parse(p[1], CultureInfo.InvariantCulture));
                            LastExploiterAt = int.Parse(p[2], CultureInfo.InvariantCulture);
                            ExploitersTried = int.Parse(p[3], CultureInfo.InvariantCulture);
                        }
                        return true;
                    case "lg":
                        if (p.Length >= 7)
                        {
                            var m = new LeagueMember
                            {
                                Id = int.Parse(p[1], CultureInfo.InvariantCulture), Kind = p[2], AddedAt = int.Parse(p[3], CultureInfo.InvariantCulture),
                                Games = double.Parse(p[4], CultureInfo.InvariantCulture), Points = double.Parse(p[5], CultureInfo.InvariantCulture), Name = p[6]
                            };
                            PolicyNet net;
                            if (nets.TryGetValue(m.Id, out net)) m.Net = net;
                            members.Add(m);
                            nextId = Math.Max(nextId, m.Id + 1);
                        }
                        return true;
                    case "lgh":
                        if (p.Length >= 2) events.Add(p[1]);
                        return true;
                }
                return false;
            }
        }

        /// <summary>Members still without a network get it from load (its file); the ones it can't load leave.</summary>
        internal void LoadNets(Func<int, PolicyNet> load)
        {
            lock (sync)
            {
                foreach (var m in members)
                {
                    if (m.Net != null) continue;
                    m.Net = load(m.Id);
                    if (m.Net != null) nets[m.Id] = m.Net;
                }
                members.RemoveAll(m => m.Net == null);
            }
        }

        /// <summary>The members' ids and networks (for saving them to their files; a network not loaded yet is null).</summary>
        internal List<KeyValuePair<int, PolicyNet>> Networks()
        {
            lock (sync)
            {
                var list = new List<KeyValuePair<int, PolicyNet>>();
                foreach (var m in members) list.Add(new KeyValuePair<int, PolicyNet>(m.Id, m.Net));
                return list;
            }
        }
    }
}
