using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>
    /// Describes one option of a decision as a few short feature names ("M:Trainer:Ultra Ball|early",
    /// "M:Attack:Dragapult ex/Phantom Dive|ko1"). The learning brain keeps a weight for each feature, so whatever
    /// it learns about a card or a situation carries over to every game where the same thing comes up.
    ///
    /// Every feature also gets a copy for the deck being played ("D:deck|...") so a deck can learn its own habits,
    /// and the most specific feature gets one for the matchup ("X:deck>opponent|...").
    /// </summary>
    public static class Features
    {
        public static List<string> For(DecisionRequest r, int index, string myDeck, string oppDeck)
        {
            var o = r.Options[index];
            var v = r.View;
            var list = new List<string>(12);
            string core;
            switch (r.Kind)
            {
                case DecisionKind.MainAction: core = MainFeatures(r, o, v, list); break;
                case DecisionKind.GoFirstOrSecond: core = Add(list, "G:" + (o.Label ?? "")); break;
                case DecisionKind.MulliganDraws: core = Add(list, "MD:" + o.Index); break;
                case DecisionKind.SetupActive: core = Add(list, "SA:" + CardName(o.Card)); break;
                case DecisionKind.SetupBench: core = Add(list, "SB:" + CardName(o.Card)); break;
                case DecisionKind.PromoteActive: core = PromoteFeatures(o, v, list); break;
                case DecisionKind.ChoosePokemon: core = PokemonFeatures(r, o, v, list); break;
                case DecisionKind.ChooseCards: core = CardFeatures(r, o, v, list); break;
                case DecisionKind.ChooseNumber: core = Add(list, "N:" + SourceKey(r) + ":" + o.Index); break;
                case DecisionKind.YesNo:
                    // Only "Yes" carries features: the brain learns whether saying yes to this effect pays off.
                    if (o.Label != "Yes") return list;
                    core = Add(list, "Y:" + SourceKey(r));
                    list.Add("Y:" + SourceKey(r) + "|" + Phase(v));
                    break;
                case DecisionKind.ChooseOption: core = Add(list, "O:" + SourceKey(r) + ":" + NoDigits(o.Label)); break;
                default: return list;
            }

            int baseCount = list.Count;
            if (!string.IsNullOrEmpty(myDeck))
                for (int i = 0; i < baseCount; i++) list.Add("D:" + myDeck + "|" + list[i]);
            if (!string.IsNullOrEmpty(myDeck) && !string.IsNullOrEmpty(oppDeck) && core != null)
                list.Add("X:" + myDeck + ">" + oppDeck + "|" + core);
            return list;
        }

        private static string Add(List<string> list, string feature)
        {
            list.Add(feature);
            return feature;
        }

        // ------------------------------------------------------------------ Main actions

        private static string MainFeatures(DecisionRequest r, DecisionOption o, PlayerView v, List<string> list)
        {
            var me = v.Me;
            string phase = Phase(v);
            string card = CardName(o.Card);
            switch (o.Action)
            {
                case ActionType.Evolve:
                {
                    string core = Add(list, "M:Evolve:" + card);
                    list.Add(core + "|" + phase);
                    list.Add(core + "|" + Spot(v, o.PokemonUid));
                    return core;
                }
                case ActionType.BenchBasic:
                {
                    string core = Add(list, "M:Bench:" + card);
                    list.Add(core + "|b" + Math.Min(5, me.Bench.Count));
                    list.Add("M:Bench|b" + Math.Min(5, me.Bench.Count) + "|" + phase);
                    return core;
                }
                case ActionType.AttachEnergy:
                {
                    var target = v.FindPokemon(o.PokemonUid);
                    string tname = target == null ? "?" : target.Top.Def.Name;
                    bool ready = target != null && HeuristicAgent.CanAttackNow(target);
                    string core = Add(list, "M:Attach:" + card + "->" + tname);
                    list.Add("M:Attach:->" + tname + "|" + Spot(v, o.PokemonUid));
                    list.Add("M:Attach:->" + tname + "|ready" + (ready ? 1 : 0));
                    list.Add(core + "|" + phase);
                    return core;
                }
                case ActionType.PlayTrainer:
                {
                    string core = Add(list, "M:Trainer:" + card);
                    list.Add(core + "|" + phase);
                    list.Add(core + "|" + HandBucket(me.HandCount));
                    if (o.Card != null) list.Add("M:Trainer|" + o.Card.TrainerKind + "|" + phase);
                    return core;
                }
                case ActionType.UseAbility:
                {
                    string ability = o.Card != null && o.Index >= 0 && o.Index < o.Card.Abilities.Count ? o.Card.Abilities[o.Index].Name : "?";
                    string core = Add(list, "M:Ability:" + card + "/" + ability);
                    list.Add(core + "|" + phase);
                    list.Add(core + "|" + HandBucket(me.HandCount));
                    return core;
                }
                case ActionType.UseHandAbility:
                {
                    string core = Add(list, "M:HandAbility:" + card);
                    list.Add(core + "|" + phase);
                    return core;
                }
                case ActionType.UseStadium:
                {
                    string core = Add(list, "M:Stadium:" + card);
                    list.Add(core + "|" + phase);
                    return core;
                }
                case ActionType.Retreat:
                {
                    bool canAttack = me.Active != null && HeuristicAgent.CanAttackNow(me.Active);
                    string core = Add(list, "M:Retreat:" + card);
                    list.Add(core + "|atk" + (canAttack ? 1 : 0));
                    list.Add("M:Retreat|atk" + (canAttack ? 1 : 0) + "|" + (me.Active == null ? "?" : HpBucket(me.Active.RemainingHp)));
                    return core;
                }
                case ActionType.Attack:
                {
                    string attack = o.Card != null && o.Index >= 0 && o.Index < o.Card.Attacks.Count ? o.Card.Attacks[o.Index].Name : "?";
                    int ko = KnocksOut(o, v) ? 1 : 0;
                    string core = Add(list, "M:Attack:" + card + "/" + attack);
                    list.Add(core + "|ko" + ko);
                    list.Add(core + "|" + phase);
                    list.Add("M:Attack|ko" + ko);
                    return core;
                }
                case ActionType.EndTurn:
                {
                    int attack = r.Options.Any(x => x.Action == ActionType.Attack) ? 1 : 0;
                    int attach = r.Options.Any(x => x.Action == ActionType.AttachEnergy) ? 1 : 0;
                    string core = Add(list, "M:End|atk" + attack);
                    list.Add("M:End|attach" + attach);
                    list.Add("M:End|atk" + attack + "|" + phase);
                    return core;
                }
                case ActionType.DiscardFossil:
                    return Add(list, "M:DiscardFossil:" + card);
                default:
                    return Add(list, "M:" + o.Action + ":" + card);
            }
        }

        /// <summary>Rough check: the attack's estimated damage is enough for the opponent's Active Pokémon.</summary>
        public static bool KnocksOut(DecisionOption o, PlayerView v)
        {
            var target = v.Opponent.Active;
            if (target == null || o.Card == null || o.Index < 0 || o.Index >= o.Card.Attacks.Count) return false;
            var user = HeuristicAgent.AttackerView(o, v);
            double damage = HeuristicAgent.EstimateDamage(o.Card, o.Index, user, v);
            if (target.Weakness.HasValue && user != null && user.Top.Def.PokemonType == target.Weakness.Value) damage *= 2;
            return damage > 0 && damage >= target.RemainingHp;
        }

        // ------------------------------------------------------------------ Other decisions

        private static string PromoteFeatures(DecisionOption o, PlayerView v, List<string> list)
        {
            var p = v.FindPokemon(o.PokemonUid);
            if (p == null) return Add(list, "P:?");
            int canAttack = HeuristicAgent.CanAttackNow(p) ? 1 : 0;
            string core = Add(list, "P:" + p.Top.Def.Name);
            list.Add(core + "|atk" + canAttack);
            list.Add("P|atk" + canAttack + "|" + HpBucket(p.RemainingHp));
            return core;
        }

        private static string PokemonFeatures(DecisionRequest r, DecisionOption o, PlayerView v, List<string> list)
        {
            string src = SourceKey(r);
            var p = v.FindPokemon(o.PokemonUid);
            if (p == null) return Add(list, "CP:" + src + ":" + NoDigits(o.Label));
            string side = p.Owner == v.Viewer ? "mine" : "theirs";
            bool active = (v.Sides[p.Owner].Active != null && v.Sides[p.Owner].Active.Uid == p.Uid);
            string core = Add(list, "CP:" + src + ":" + side + ":" + p.Top.Def.Name);
            list.Add("CP:" + src + ":" + side + ":" + (active ? "active" : "bench"));
            list.Add("CP:" + src + ":" + side + ":" + HpBucket(p.RemainingHp));
            if (p.Top.Def.HasRuleBox) list.Add("CP:" + src + ":" + side + ":ex");
            return core;
        }

        private static string CardFeatures(DecisionRequest r, DecisionOption o, PlayerView v, List<string> list)
        {
            string prompt = (r.Prompt ?? "").ToLowerInvariant();
            string prefix = prompt.Contains("discard") ? "CD" : "CC";
            string card = o.Card != null ? o.Card.Name : NoDigits(o.Label);
            string core = Add(list, prefix + ":" + SourceKey(r) + ":" + card);
            list.Add(prefix + ":*:" + card);
            list.Add(prefix + ":*:" + card + "|" + Phase(v));
            return core;
        }

        // ------------------------------------------------------------------ Context helpers

        /// <summary>early (first 2 turns each), late (someone has 2 or fewer Prize cards left), otherwise mid.</summary>
        public static string Phase(PlayerView v)
        {
            if (v.TurnNumber <= 4) return "early";
            if (Math.Min(v.Me.PrizeCount, v.Opponent.PrizeCount) <= 2) return "late";
            return "mid";
        }

        private static string HandBucket(int n)
        {
            if (n <= 3) return "h0-3";
            if (n <= 6) return "h4-6";
            return "h7+";
        }

        private static string HpBucket(int hp)
        {
            if (hp <= 60) return "hp60";
            if (hp <= 120) return "hp120";
            if (hp <= 200) return "hp200";
            return "hp200+";
        }

        private static string Spot(PlayerView v, int uid)
        {
            return v.Me.Active != null && v.Me.Active.Uid == uid ? "active" : "bench";
        }

        private static string CardName(CardDef d)
        {
            return d == null ? "?" : d.Name;
        }

        /// <summary>The card asking, or the start of the prompt for rules decisions ("Choose your new Active Pokémon").</summary>
        public static string SourceKey(DecisionRequest r)
        {
            if (r.Source != null) return r.Source.Name + "/" + Head(r.Prompt);
            return Head(r.Prompt);
        }

        private static string Head(string prompt)
        {
            if (string.IsNullOrEmpty(prompt)) return "";
            int end = prompt.Length;
            foreach (char c in new[] { ':', '?', '(', '.' })
            {
                int i = prompt.IndexOf(c);
                if (i > 0 && i < end) end = i;
            }
            string head = prompt.Substring(0, Math.Min(end, 40)).Trim();
            return NoDigits(head);
        }

        /// <summary>Numbers in labels and prompts change from game to game ("Draw 3"); features ignore them.</summary>
        private static string NoDigits(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '\t' || c == '\n' || c == '\r') sb.Append(' ');
                else sb.Append(char.IsDigit(c) ? '#' : c);
            }
            return sb.ToString();
        }

        // ------------------------------------------------------------------ Readable descriptions (for the Train screen)

        /// <summary>Turns a feature name into a short sentence, e.g. "Attack with Dragapult ex's Phantom Dive (when it Knocks Out)".</summary>
        public static string Describe(string feature)
        {
            if (string.IsNullOrEmpty(feature)) return "";
            string scope = "";
            string f = feature;
            if (f.StartsWith("X:"))
            {
                int bar = f.IndexOf('|');
                if (bar > 0)
                {
                    scope = " [" + f.Substring(2, bar - 2).Replace(">", " vs ") + "]";
                    f = f.Substring(bar + 1);
                }
            }
            else if (f.StartsWith("D:"))
            {
                int bar = f.IndexOf('|');
                if (bar > 0)
                {
                    scope = " [" + f.Substring(2, bar - 2) + "]";
                    f = f.Substring(bar + 1);
                }
            }
            string context = "";
            int cut = f.IndexOf('|');
            if (cut > 0)
            {
                context = ContextText(f.Substring(cut + 1));
                f = f.Substring(0, cut);
            }
            string text;
            if (f.StartsWith("M:Attack:")) text = "Attack with " + f.Substring(9).Replace("/", "'s ");
            else if (f == "M:Attack") text = "Attack";
            else if (f.StartsWith("M:Trainer:")) text = "Play " + f.Substring(10);
            else if (f == "M:Trainer") text = "Play a Trainer";
            else if (f.StartsWith("M:Evolve:")) text = "Evolve into " + f.Substring(9);
            else if (f.StartsWith("M:Bench:")) text = "Bench " + f.Substring(8);
            else if (f == "M:Bench") text = "Bench a Pokémon";
            else if (f.StartsWith("M:Attach:->")) text = "Attach Energy to " + f.Substring(11);
            else if (f.StartsWith("M:Attach:")) text = "Attach " + f.Substring(9).Replace("->", " to ");
            else if (f.StartsWith("M:Ability:")) text = "Use " + f.Substring(10).Replace("/", "'s ");
            else if (f.StartsWith("M:HandAbility:")) text = "Use the Ability of " + f.Substring(14) + " in hand";
            else if (f.StartsWith("M:Stadium:")) text = "Use " + f.Substring(10);
            else if (f.StartsWith("M:Retreat:")) text = "Retreat " + f.Substring(10);
            else if (f == "M:Retreat") text = "Retreat";
            else if (f == "M:End") text = "End the turn";
            else if (f.StartsWith("M:DiscardFossil:")) text = "Discard " + f.Substring(16) + " from play";
            else if (f.StartsWith("G:")) text = f.Substring(2);
            else if (f.StartsWith("SA:")) text = "Start with " + f.Substring(3) + " Active";
            else if (f.StartsWith("SB:")) text = "Bench " + f.Substring(3) + " at the start";
            else if (f.StartsWith("P:")) text = "Promote " + f.Substring(2);
            else if (f == "P") text = "Promote a Pokémon";
            else if (f.StartsWith("Y:")) text = "Say yes to " + f.Substring(2);
            else if (f.StartsWith("CP:")) text = "Pick " + LastPart(f.Substring(3)) + " for " + FirstPart(f.Substring(3));
            else if (f.StartsWith("CD:*:")) text = "Discard " + f.Substring(5);
            else if (f.StartsWith("CC:*:")) text = "Take " + f.Substring(5);
            else if (f.StartsWith("CD:")) text = "Discard " + LastPart(f.Substring(3)) + " for " + FirstPart(f.Substring(3));
            else if (f.StartsWith("CC:")) text = "Take " + LastPart(f.Substring(3)) + " with " + FirstPart(f.Substring(3));
            else if (f.StartsWith("O:")) text = "Choose " + LastPart(f.Substring(2)) + " for " + FirstPart(f.Substring(2));
            else text = f;
            return text + context + scope;
        }

        private static string FirstPart(string s)
        {
            int i = s.IndexOf(':');
            string first = i < 0 ? s : s.Substring(0, i);
            int slash = first.IndexOf('/');
            return slash > 0 ? first.Substring(0, slash) : first;
        }

        private static string LastPart(string s)
        {
            int i = s.LastIndexOf(':');
            return i < 0 ? s : s.Substring(i + 1);
        }

        private static string ContextText(string context)
        {
            var parts = new List<string>();
            foreach (var c in context.Split('|'))
            {
                switch (c)
                {
                    case "early": parts.Add("early game"); break;
                    case "mid": parts.Add("mid game"); break;
                    case "late": parts.Add("late game"); break;
                    case "ko1": parts.Add("when it Knocks Out"); break;
                    case "ko0": parts.Add("when it doesn't Knock Out"); break;
                    case "atk1": parts.Add("when an attack is ready"); break;
                    case "atk0": parts.Add("with no attack ready"); break;
                    case "attach1": parts.Add("before attaching Energy"); break;
                    case "attach0": parts.Add("after attaching Energy"); break;
                    case "ready1": parts.Add("when it can already attack"); break;
                    case "ready0": parts.Add("when it can't attack yet"); break;
                    case "active": parts.Add("in the Active Spot"); break;
                    case "bench": parts.Add("on the Bench"); break;
                    case "h0-3": parts.Add("with 0-3 cards in hand"); break;
                    case "h4-6": parts.Add("with 4-6 cards in hand"); break;
                    case "h7+": parts.Add("with 7+ cards in hand"); break;
                    case "hp60": parts.Add("at 60 HP or less"); break;
                    case "hp120": parts.Add("at 70-120 HP"); break;
                    case "hp200": parts.Add("at 130-200 HP"); break;
                    case "hp200+": parts.Add("above 200 HP"); break;
                    case "ex": parts.Add("Pokémon ex"); break;
                    default:
                        if (c.Length == 2 && c[0] == 'b' && char.IsDigit(c[1])) parts.Add("with " + c[1] + " on the Bench");
                        else parts.Add(c);
                        break;
                }
            }
            return parts.Count == 0 ? "" : " (" + string.Join(", ", parts.ToArray()) + ")";
        }
    }
}
