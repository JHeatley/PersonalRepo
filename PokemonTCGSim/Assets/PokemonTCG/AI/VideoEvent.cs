using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PokemonTCG.AI
{
    public enum VideoEventKind
    {
        /// <summary>A player's turn starts (Amount: the turn number, 0 if unknown).</summary>
        Turn,
        /// <summary>A Pokémon put into play (Spot "Bench", or "Active Spot" in the setup).</summary>
        Bench,
        Evolve,
        /// <summary>A Pokémon is now the Active Pokémon (promoted, switched, retreated into).</summary>
        Active,
        Stadium,
        Heal,
        /// <summary>An attack (Attack: its name; Amount: damage to Card2). Unidentified, it's written as damage counters.</summary>
        Attack,
        /// <summary>Damage counters put on a Pokémon (Amount: how many).</summary>
        Counters,
        KnockOut,
        /// <summary>Prize cards taken (Amount).</summary>
        Prizes,
        Win,
        Retreat,
        Ability,
        Trainer,
        Energy,
        EndTurn,
        /// <summary>A battle log line typed by hand (Text).</summary>
        Line
    }

    /// <summary>
    /// Something that happened in a video (v77), found by the reader or tagged by hand. Each event is written as one line
    /// of a TCG Live battle log, so the video is studied the same way a TCG Live log is.
    /// </summary>
    public sealed class VideoEvent
    {
        public double Time;
        public VideoEventKind Kind;
        /// <summary>Who did it (whose Pokémon, who took the Prize cards).</summary>
        public int Player = -1;
        public string Card = "";
        /// <summary>The Pokémon it evolved into, the attack's target, the Pokémon the Energy went to.</summary>
        public string Card2 = "";
        /// <summary>Card2's owner.</summary>
        public int Target = -1;
        /// <summary>"Bench" or "Active Spot".</summary>
        public string Spot = "";
        public int Amount;
        /// <summary>The attack's or Ability's name ("" when the reader couldn't tell which attack it was).</summary>
        public string Attack = "";
        /// <summary>The attacks it could have been, for picking one.</summary>
        public string Choices = "";
        /// <summary>False when the reader guessed (an attack it couldn't identify, a Pokémon that left play without a Prize card taken).</summary>
        public bool Sure = true;
        public bool Enabled = true;
        /// <summary>Tagged by hand (kept when the video is read again).</summary>
        public bool Manual;
        /// <summary>Part of the board when the reading starts (written in the log's setup).</summary>
        public bool Setup;
        public string Text = "";
        public string Note = "";

        public bool IsUnidentifiedAttack { get { return Kind == VideoEventKind.Attack && (Attack.Length == 0 || !Sure); } }

        private static string N(string[] names, int p)
        {
            if (names != null && p >= 0 && p < names.Length && !string.IsNullOrEmpty(names[p])) return names[p];
            return "Player " + (p + 1);
        }

        private static string Where(string spot)
        {
            return spot == "Active Spot" ? " in the Active Spot." : " on the Bench.";
        }

        /// <summary>The battle log line(s) for this event (two for a retreat: the retreat and the new Active Pokémon).</summary>
        public List<string> Lines(string[] names)
        {
            var lines = new List<string>();
            string p = N(names, Player), o = N(names, Target >= 0 ? Target : 1 - Math.Max(0, Player));
            switch (Kind)
            {
                case VideoEventKind.Turn:
                    lines.Add(p + "'s Turn" + (Amount > 0 ? " " + Amount.ToString(CultureInfo.InvariantCulture) : ""));
                    break;
                case VideoEventKind.Bench:
                    lines.Add(p + " played " + Card + " to the " + (Spot == "Active Spot" ? "Active Spot" : "Bench") + ".");
                    break;
                case VideoEventKind.Evolve:
                    lines.Add(p + " evolved " + Card + " to " + Card2 + Where(Spot));
                    break;
                case VideoEventKind.Active:
                    lines.Add(p + "'s " + Card + " is now in the Active Spot.");
                    break;
                case VideoEventKind.Stadium:
                    lines.Add(p + " played " + Card + " to the Stadium spot.");
                    break;
                case VideoEventKind.Heal:
                    lines.Add(p + "'s " + Card + " healed " + Amount.ToString(CultureInfo.InvariantCulture) + " damage.");
                    break;
                case VideoEventKind.Attack:
                    if (Attack.Length > 0 && Sure)
                    {
                        if (Amount > 0 && Card2.Length > 0)
                            lines.Add(p + "'s " + Card + " used " + Attack + " on " + o + "'s " + Card2 + " for " + Amount.ToString(CultureInfo.InvariantCulture) + " damage.");
                        else lines.Add(p + "'s " + Card + " used " + Attack + ".");
                    }
                    else if (Amount >= 10 && Card2.Length > 0)
                        lines.Add(p + " put " + Counters(Amount / 10) + " on " + o + "'s " + Card2 + ".");
                    break;
                case VideoEventKind.Counters:
                    if (Amount > 0) lines.Add(p + " put " + Counters(Amount) + " on " + o + "'s " + Card2 + ".");
                    break;
                case VideoEventKind.KnockOut:
                    lines.Add(p + "'s " + Card + " was Knocked Out!");
                    break;
                case VideoEventKind.Prizes:
                    if (Amount > 0) lines.Add(p + " took " + (Amount == 1 ? "a Prize card." : Amount.ToString(CultureInfo.InvariantCulture) + " Prize cards."));
                    break;
                case VideoEventKind.Win:
                    lines.Add(p + " wins.");
                    break;
                case VideoEventKind.Retreat:
                    lines.Add(p + " retreated " + Card + " to the Bench.");
                    if (Card2.Length > 0) lines.Add(p + "'s " + Card2 + " is now in the Active Spot.");
                    break;
                case VideoEventKind.Ability:
                    lines.Add(p + "'s " + Card + " used " + Attack + ".");
                    break;
                case VideoEventKind.Trainer:
                    lines.Add(p + " played " + Card + ".");
                    break;
                case VideoEventKind.Energy:
                    lines.Add(p + " attached " + Card + " to " + Card2 + Where(Spot));
                    break;
                case VideoEventKind.EndTurn:
                    lines.Add(p + " ended their turn.");
                    break;
                case VideoEventKind.Line:
                    if (Text.Trim().Length > 0) lines.Add(Text.Trim());
                    break;
            }
            return lines;
        }

        private static string Counters(int n)
        {
            return n.ToString(CultureInfo.InvariantCulture) + (n == 1 ? " damage counter" : " damage counters");
        }

        /// <summary>The event for the list on screen: its time, its line, and a note when the reader guessed.</summary>
        public string Describe(string[] names)
        {
            var sb = new StringBuilder();
            sb.Append(Clock(Time)).Append("  ");
            if (Kind == VideoEventKind.Attack && (Attack.Length == 0 || !Sure))
            {
                sb.Append(N(names, Player)).Append("'s ").Append(Card.Length > 0 ? Card : "Active Pokémon").Append(" attacked ")
                  .Append(N(names, Target >= 0 ? Target : 1 - Math.Max(0, Player))).Append("'s ").Append(Card2).Append(" for ")
                  .Append(Amount.ToString(CultureInfo.InvariantCulture)).Append(" damage: which attack? ")
                  .Append(Attack.Length > 0 ? "(maybe " + Attack + ")" : "").Append(Choices.Length > 0 ? " Options: " + Choices : "");
            }
            else sb.Append(string.Join(" / ", Lines(names).ToArray()));
            if (Note.Length > 0) sb.Append("  (").Append(Note).Append(')');
            return sb.ToString();
        }

        public static string Clock(double seconds)
        {
            if (seconds < 0) seconds = 0;
            int s = (int)Math.Floor(seconds);
            int h = s / 3600, m = s / 60 % 60, sec = s % 60;
            return h > 0
                ? h.ToString(CultureInfo.InvariantCulture) + ":" + m.ToString("00", CultureInfo.InvariantCulture) + ":" + sec.ToString("00", CultureInfo.InvariantCulture)
                : m.ToString(CultureInfo.InvariantCulture) + ":" + sec.ToString("00", CultureInfo.InvariantCulture);
        }

        // ------------------------------------------------------------------ Saving

        public void AppendTo(StringBuilder sb)
        {
            sb.Append("e\t").Append(Time.ToString("0.###", CultureInfo.InvariantCulture))
              .Append('\t').Append(Kind.ToString())
              .Append('\t').Append(Player.ToString(CultureInfo.InvariantCulture))
              .Append('\t').Append(Clean(Card))
              .Append('\t').Append(Clean(Card2))
              .Append('\t').Append(Target.ToString(CultureInfo.InvariantCulture))
              .Append('\t').Append(Clean(Spot))
              .Append('\t').Append(Amount.ToString(CultureInfo.InvariantCulture))
              .Append('\t').Append(Clean(Attack))
              .Append('\t').Append(Clean(Choices))
              .Append('\t').Append(Sure ? "1" : "0")
              .Append('\t').Append(Enabled ? "1" : "0")
              .Append('\t').Append(Manual ? "1" : "0")
              .Append('\t').Append(Setup ? "1" : "0")
              .Append('\t').Append(Clean(Text))
              .Append('\t').Append(Clean(Note))
              .Append('\n');
        }

        private static string Clean(string s)
        {
            return (s ?? "").Replace('\t', ' ').Replace('\n', ' ').Replace('\r', ' ');
        }

        /// <summary>Reads an "e" line's fields (f[0] is "e"); null if it can't be read.</summary>
        public static VideoEvent Parse(string[] f)
        {
            if (f == null || f.Length < 15) return null;
            var e = new VideoEvent();
            VideoEventKind kind;
            if (!Enum.TryParse(f[2], out kind)) return null;
            e.Kind = kind;
            double t;
            double.TryParse(f[1], NumberStyles.Float, CultureInfo.InvariantCulture, out t);
            e.Time = t;
            e.Player = Int(f[3], -1);
            e.Card = f[4];
            e.Card2 = f[5];
            e.Target = Int(f[6], -1);
            e.Spot = f[7];
            e.Amount = Int(f[8], 0);
            e.Attack = f[9];
            e.Choices = f[10];
            e.Sure = f[11] == "1";
            e.Enabled = f[12] == "1";
            e.Manual = f[13] == "1";
            e.Setup = f[14] == "1";
            e.Text = f.Length > 15 ? f[15] : "";
            e.Note = f.Length > 16 ? f[16] : "";
            return e;
        }

        private static int Int(string s, int fallback)
        {
            int n;
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) ? n : fallback;
        }
    }
}
