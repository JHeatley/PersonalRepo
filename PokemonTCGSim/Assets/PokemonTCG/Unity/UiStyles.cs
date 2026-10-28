using System.Text;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>Shared IMGUI styles. Create only from inside OnGUI (GUI.skin isn't available elsewhere).</summary>
    public sealed class UiStyles
    {
        private static UiStyles instance;

        public readonly GUIStyle Label, Small, Badge, Tooltip, Title, Header, Button, Error, Good;
        public readonly Texture2D Highlight, Selected, BadgeBackground;

        public static UiStyles Get()
        {
            if (instance == null) instance = new UiStyles();
            return instance;
        }

        private UiStyles()
        {
            Label = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            Small = new GUIStyle(GUI.skin.label) { fontSize = 11, wordWrap = true, alignment = TextAnchor.MiddleCenter };
            Small.normal.textColor = Color.black;
            Badge = new GUIStyle(GUI.skin.label) { fontSize = 12, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
            Badge.normal.textColor = Color.white;
            Tooltip = new GUIStyle(GUI.skin.box) { fontSize = 13, wordWrap = true, alignment = TextAnchor.UpperLeft, richText = true };
            Tooltip.normal.textColor = Color.white;
            Title = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, wordWrap = true };
            Header = new GUIStyle(GUI.skin.label) { fontSize = 20, fontStyle = FontStyle.Bold, wordWrap = true };
            Button = new GUIStyle(GUI.skin.button) { fontSize = 13, alignment = TextAnchor.MiddleLeft, wordWrap = true };
            Error = new GUIStyle(Label);
            Error.normal.textColor = new Color(1f, 0.45f, 0.4f);
            Good = new GUIStyle(Label);
            Good.normal.textColor = new Color(0.45f, 0.95f, 0.55f);
            Highlight = CardVisuals.Solid(new Color(1f, 0.85f, 0.2f, 1f));
            Selected = CardVisuals.Solid(new Color(0.3f, 1f, 0.5f, 1f));
            BadgeBackground = CardVisuals.Solid(new Color(0.75f, 0.1f, 0.1f, 0.9f));
        }

        /// <summary>Card-detail popup near the mouse.</summary>
        public void DrawTooltip(string text, Vector2 mouse)
        {
            float w = 300f;
            float h = Tooltip.CalcHeight(new GUIContent(text), w);
            float x = Mathf.Min(mouse.x + 18, UiScale.Width - w - 10);
            float y = Mathf.Min(mouse.y + 18, UiScale.Height - h - 10);
            GUI.Box(new Rect(x, y, w, h), text, Tooltip);
        }
    }

    public static class CardText
    {
        /// <summary>Rich-text summary of a card for tooltips.</summary>
        public static string Describe(CardDef d)
        {
            var sb = new StringBuilder();
            sb.Append("<b>" + d.Name + "</b>  <size=11>" + d.SetCode + " " + d.Number + "</size>\n");
            if (d.IsPokemon)
            {
                sb.Append(StageText(d) + " " + TypeText(d) + "  HP " + d.Hp + (d.IsEx ? "  (ex: 2 Prizes)" : "") + "\n");
                string rule = MechanicRule(d);
                if (rule != null) sb.Append("<i>" + rule + "</i>\n");
                if (!string.IsNullOrEmpty(d.Strike)) sb.Append("<i>" + d.Strike + " Pokémon</i>\n");
                if (!string.IsNullOrEmpty(d.EvolvesFrom)) sb.Append("Evolves from " + d.EvolvesFrom + "\n");
                foreach (var a in d.Abilities) sb.Append("<b>" + (a.VstarPower ? "VSTAR Power - " : "") + (a.IsAbility ? "Ability" : a.Kind) + ": " + a.Name + "</b>\n" + a.Text + "\n");
                foreach (var a in d.Attacks) AppendAttack(sb, a);
                sb.Append("Weakness " + (d.Weakness.HasValue ? d.Weakness.Value + (d.SecondWeakness.HasValue ? "/" + d.SecondWeakness.Value : "") + (d.WeaknessAmount > 0 ? " +" + d.WeaknessAmount : "") : "-") +
                          "   Resistance " + (d.Resistance.HasValue ? d.Resistance.Value + (d.SecondResistance.HasValue ? "/" + d.SecondResistance.Value : "") + " -" + d.ResistanceAmount : "-") +
                          "   Retreat " + d.RetreatCost + "\n");
            }
            else if (d.IsTrainer)
            {
                sb.Append(d.TrainerLabel + (d.IsAceSpec ? " (ACE SPEC)" : "") + "\n");
            }
            else
            {
                sb.Append(d.IsBasicEnergy ? "Basic Energy\n" : "Special Energy\n");
            }
            // Strike-style Trainers and Energy (Fusion Strike Energy, Cross Switcher, the Style Mustards), v55.
            if (!d.IsPokemon && !string.IsNullOrEmpty(d.Strike)) sb.Append("<i>" + d.Strike + " card</i>\n");
            if (!string.IsNullOrEmpty(d.Text)) sb.Append(d.Text);
            // v168.1: a Trainer or Energy with its own attack or Ability (Mega Rayquaza Cap's Delta Gift, Technical Machines,
            // Rocket's Tricky Gym, Miracle Spheres) shows it, unless the card's text already spells it out.
            if (!d.IsPokemon)
            {
                bool first = true;
                foreach (var a in d.Abilities)
                {
                    if (!string.IsNullOrEmpty(d.Text) && d.Text.IndexOf(a.Name, System.StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (first) { sb.Append("\n"); first = false; }
                    sb.Append("\n<b>" + (a.VstarPower ? "VSTAR Power - " : "") + (a.IsAbility ? "Ability" : a.Kind) + ": " + a.Name + "</b>\n" + a.Text);
                }
                foreach (var a in d.Attacks)
                {
                    if (!string.IsNullOrEmpty(d.Text) && d.Text.IndexOf(a.Name, System.StringComparison.OrdinalIgnoreCase) >= 0) continue;
                    if (first) { sb.Append("\n"); first = false; }
                    sb.Append("\n");
                    AppendAttack(sb, a);
                }
            }
            if (Formats.IsExpandedBanned(d)) sb.Append("\n<i>Banned in Expanded: Unlimited format only.</i>");
            else if (d.UnlimitedOnly) sb.Append("\n<i>Unlimited format only (older than Black & White, or not legal for tournaments).</i>");
            return sb.ToString().TrimEnd();
        }

        private static void AppendAttack(StringBuilder sb, AttackDef a)
        {
            string cost = a.Cost.Length == 0 ? "free" : a.CostString;
            sb.Append("<b>[" + cost + "] " + (a.VstarPower ? "VSTAR Power - " : "") + a.Name + "</b> " + (a.Damage > 0 ? a.Damage + a.DamageSuffix : "") + "\n");
            if (!string.IsNullOrEmpty(a.Text)) sb.Append(a.Text + "\n");
        }

        private static string StageText(CardDef d)
        {
            if (d.IsLegend) return "LEGEND";
            if (d.IsVUnion) return "V-UNION";
            if (d.IsLevelUp) return "Level-Up";
            if (d.IsBreak) return "BREAK";
            if (d.Mechanic == "VMAX" || d.Mechanic == "VSTAR" || d.IsOldMega) return d.Mechanic == "M EX" ? "Mega Evolution" : d.Mechanic;
            return d.Stage.ToString();
        }

        private static string TypeText(CardDef d)
        {
            return d.SecondType.HasValue ? d.PokemonType + "/" + d.SecondType.Value : d.PokemonType.ToString();
        }

        /// <summary>The rule box of older card kinds (Classic Collection), or null.</summary>
        private static string MechanicRule(CardDef d)
        {
            switch (d.Mechanic)
            {
                case "Pokémon-ex": return "Pokémon-ex: when it's Knocked Out, your opponent takes 2 Prize cards.";
                case "EX": return "Pokémon-EX: when it's Knocked Out, your opponent takes 2 Prize cards.";
                case "M EX": return "Mega Evolution: when it Mega Evolves, your turn ends. 2 Prize cards when Knocked Out.";
                case "GX": return "Pokémon-GX: 2 Prize cards when Knocked Out. Only 1 GX attack per game.";
                case "TAG TEAM": return "TAG TEAM: 3 Prize cards when Knocked Out. Only 1 GX attack per game.";
                case "V": return "Pokémon V: 2 Prize cards when Knocked Out.";
                case "VMAX": return "Pokémon VMAX: 3 Prize cards when Knocked Out.";
                case "VSTAR": return "Pokémon VSTAR: 2 Prize cards when Knocked Out. Only 1 VSTAR Power per game.";
                case "Radiant": return "Radiant Pokémon: it has a Rule Box. Only 1 Radiant Pokémon in a deck.";
                case "LEGEND": return "LEGEND: played onto your Bench together with its other half (not during setup). 2 Prize cards when Knocked Out.";
                case "V-UNION": return "V-UNION: once per game for each V-UNION, combine 4 different pieces from your discard pile onto your Bench. 3 Prize cards when Knocked Out.";
                case "LV.X": return "Level-Up: put it onto your Active " + d.EvolvesFrom + ". It can still use the attacks of the Pokémon it's on.";
                case "BREAK": return "BREAK Evolution: it can still use the attacks of the Pokémon it's on.";
                default: return null;
            }
        }

        /// <summary>One-line summary for lists.</summary>
        public static string Short(CardDef d)
        {
            if (d.IsPokemon) return StageText(d) + " " + TypeText(d) + ", " + d.Hp + " HP" + (d.IsEx ? ", ex" : "") + (string.IsNullOrEmpty(d.Mechanic) || d.IsLegend || d.IsVUnion || d.IsLevelUp || d.IsBreak ? "" : ", " + d.Mechanic);
            if (d.IsTrainer) return (d.TrainerKind == TrainerKind.Tool && !d.ToolIsItem ? "Tool" : d.TrainerLabel) + (d.IsAceSpec ? ", ACE SPEC" : "");
            return d.IsBasicEnergy ? "Basic Energy" : "Special Energy";
        }
    }
}
