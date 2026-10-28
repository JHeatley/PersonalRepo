using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Deck builder set filter (v115): the card pool can show only the cards of one set. A card counts as part of a set when
    /// any of its printings is from that set, so reprints show up too (Base Set lists Blastoise even though the pool's main
    /// printing is the Celebrations one). While a set is chosen, a card your deck doesn't use yet shows that set's printing,
    /// and adding it puts that printing in the deck.
    /// </summary>
    public sealed partial class DeckBuilderScreen
    {
        private sealed class SetEntry
        {
            public string Code;
            public string Name;
            public int Cards;
            /// <summary>The widest pool tab (0 Standard, 1 Expanded, 2 Unlimited) any of its cards needs.</summary>
            public int Tab;
        }

        /// <summary>The set the pool is limited to, or null for every set.</summary>
        private string setFilter;
        private bool setMenuOpen;
        private string setSearch = "";
        private Vector2 setScroll;
        private List<SetEntry> sets;
        /// <summary>Set code -> (card's main key -> that card's printing from the set).</summary>
        private Dictionary<string, Dictionary<string, CardDef>> setPrintings;
        private GUIStyle setItem;

        private void EnsureSets()
        {
            if (sets != null) return;
            setPrintings = new Dictionary<string, Dictionary<string, CardDef>>();
            var names = new Dictionary<string, string>();
            var inPool = new HashSet<string>(pool.Select(d => d.Key));
            foreach (var printing in db.All.OrderBy(d => d.IsAlternatePrint ? 1 : 0))
            {
                string code = printing.SetCode;
                if (string.IsNullOrEmpty(code) || !inPool.Contains(printing.BaseKey)) continue;
                Dictionary<string, CardDef> cards;
                if (!setPrintings.TryGetValue(code, out cards))
                {
                    cards = new Dictionary<string, CardDef>();
                    setPrintings[code] = cards;
                }
                // A card printed twice in one set (Jungle's holo and non-holo) shows its lowest-numbered printing.
                CardDef have;
                if (!cards.TryGetValue(printing.BaseKey, out have) || NumberOrder(printing) < NumberOrder(have)) cards[printing.BaseKey] = printing;
                if (!names.ContainsKey(code) && !string.IsNullOrEmpty(printing.PrintSetName)) names[code] = printing.PrintSetName;
            }
            sets = new List<SetEntry>();
            foreach (var kv in setPrintings)
            {
                string name;
                if (!names.TryGetValue(kv.Key, out name)) name = kv.Key;
                int tab = 0;
                foreach (var key in kv.Value.Keys)
                {
                    var main = db.Get(key);
                    if (main != null) tab = Mathf.Max(tab, (int)Formats.CategoryOf(main));
                }
                sets.Add(new SetEntry { Code = kv.Key, Name = name, Cards = kv.Value.Count, Tab = tab });
            }
            sets = sets.OrderBy(s => s.Name, System.StringComparer.OrdinalIgnoreCase).ThenBy(s => s.Code).ToList();
        }

        /// <summary>Sorts printing numbers like 2 &lt; 10 &lt; TG3 (numbers first, then the rest by text).</summary>
        private static long NumberOrder(CardDef d)
        {
            int n;
            return int.TryParse(d.Number, out n) ? n : 1000000L + (d.Number ?? "").GetHashCode() % 1000;
        }

        /// <summary>Whether the card has a printing in the chosen set (always true with no set chosen).</summary>
        private bool InSetFilter(CardDef card)
        {
            if (setFilter == null) return true;
            EnsureSets();
            Dictionary<string, CardDef> cards;
            return setPrintings.TryGetValue(setFilter, out cards) && cards.ContainsKey(card.BaseKey);
        }

        /// <summary>The chosen set's printing of the card, or null.</summary>
        private CardDef SetPrintingOf(CardDef card)
        {
            if (setFilter == null) return null;
            EnsureSets();
            Dictionary<string, CardDef> cards;
            CardDef p;
            return setPrintings.TryGetValue(setFilter, out cards) && cards.TryGetValue(card.BaseKey, out p) ? p : null;
        }

        private SetEntry ChosenSet()
        {
            if (setFilter == null) return null;
            EnsureSets();
            return sets.FirstOrDefault(s => s.Code == setFilter);
        }

        private string SetButtonLabel()
        {
            var s = ChosenSet();
            return s == null ? "All sets   ▼" : s.Name + " (" + s.Code + ")  ·  " + s.Cards + " card" + (s.Cards == 1 ? "" : "s") + "   ▼";
        }

        private void OpenSetMenu()
        {
            EnsureSets();
            setMenuOpen = true;
            setSearch = "";
            setScroll = Vector2.zero;
            GUIUtility.keyboardControl = 0;
        }

        /// <summary>Limits the pool to one set (null: every set) and widens the pool tab so all of its cards show.</summary>
        private void ChooseSet(string code)
        {
            setFilter = code;
            setMenuOpen = false;
            poolScroll = Vector2.zero;
            var s = ChosenSet();
            if (s != null && category < 3 && s.Tab > category) category = s.Tab;
        }

        /// <summary>The "Set" row of the card pool: the set picker and a button back to every set.</summary>
        private void DrawSetRow(UiStyles ui, List<System.Action> actions)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Set", GUILayout.Width(50));
            if (GUILayout.Button(SetButtonLabel(), SetItem(ui))) actions.Add(OpenSetMenu);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = wasEnabled && setFilter != null;
            if (GUILayout.Button("All sets", GUILayout.Width(80))) actions.Add(() => ChooseSet(null));
            GUI.enabled = wasEnabled;
            GUILayout.EndHorizontal();
            var chosen = ChosenSet();
            if (chosen != null)
                GUILayout.Label("Showing the cards printed in " + chosen.Name + ", reprints included. A card your deck doesn't use yet shows (and adds) its " + chosen.Name + " printing.", Hint(ui));
        }

        /// <summary>The set list: every set with a card in the pool, a box to narrow it down, and All sets.</summary>
        private void DrawSetMenu(UiStyles ui, List<System.Action> actions)
        {
            var ev = Event.current;
            var prevColor = GUI.color;
            GUI.color = new Color(0f, 0f, 0f, 0.6f);
            GUI.DrawTexture(new Rect(0f, 0f, UiScale.Width, UiScale.Height), CardVisuals.Solid(Color.white));
            GUI.color = prevColor;

            float w = Mathf.Min(560f, UiScale.Width - 40f);
            float h = Mathf.Min(640f, UiScale.Height - 40f);
            var box = new Rect((UiScale.Width - w) / 2f, (UiScale.Height - h) / 2f, w, h);
            GUI.DrawTexture(box, CardVisuals.Solid(new Color(0.12f, 0.12f, 0.15f, 1f)));
            DrawFrame(box, CardVisuals.Solid(new Color(0.5f, 0.5f, 0.55f, 1f)), 1f);

            GUILayout.BeginArea(new Rect(box.x + 12f, box.y + 12f, box.width - 24f, box.height - 24f));
            GUILayout.Label("Show the cards of one set", ui.Title);
            GUILayout.Label("A card is in a set when any of its printings is from that set, so reprints are included.", Hint(ui));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Find", GUILayout.Width(40));
            setSearch = GUILayout.TextField(setSearch ?? "");
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);
            if (GUILayout.Button((setFilter == null ? "● " : "") + "All sets  ·  " + pool.Count + " cards", SetItem(ui))) actions.Add(() => ChooseSet(null));
            setScroll = GUILayout.BeginScrollView(setScroll);
            string find = (setSearch ?? "").Trim();
            int shown = 0;
            foreach (var s in sets)
            {
                if (find.Length > 0 && s.Name.IndexOf(find, System.StringComparison.OrdinalIgnoreCase) < 0
                    && s.Code.IndexOf(find, System.StringComparison.OrdinalIgnoreCase) < 0) continue;
                shown++;
                string code = s.Code;
                string label = (code == setFilter ? "● " : "") + s.Name + "  (" + code + ")  ·  " + s.Cards + " card" + (s.Cards == 1 ? "" : "s")
                    + (s.Tab == 2 ? "  ·  Unlimited" : s.Tab == 1 ? "  ·  Expanded" : "");
                if (GUILayout.Button(label, SetItem(ui))) actions.Add(() => ChooseSet(code));
            }
            if (shown == 0) GUILayout.Label("No set matches \"" + find + "\".", Hint(ui));
            GUILayout.EndScrollView();
            if (GUILayout.Button("Close", GUILayout.Height(28f))) actions.Add(() => setMenuOpen = false);
            GUILayout.EndArea();

            if (ev.type == EventType.KeyDown && ev.keyCode == KeyCode.Escape)
            {
                actions.Add(() => setMenuOpen = false);
                ev.Use();
            }
            else if (ev.type == EventType.MouseDown && !box.Contains(ev.mousePosition))
            {
                actions.Add(() => setMenuOpen = false);
                ev.Use();
            }
        }

        private GUIStyle SetItem(UiStyles ui)
        {
            if (setItem == null) setItem = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleLeft, wordWrap = false };
            return setItem;
        }
    }
}
