using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Formats (v123): make, name, edit and delete your own formats. Each has a deck size, a number of Prize cards, its legal sets,
    /// banned cards, deck rules (singleton, Rule Box Pokémon, ACE SPECs) and game rules (attacking and Supporters on the first
    /// turn, hands before the coin flip, Tools as Items, old Trainers as plain Trainers). Saved formats show up in the deck
    /// builder and the menu.
    /// </summary>
    public sealed class FormatEditorScreen
    {
        private readonly CardDatabase db;
        private readonly List<CardDef> pool;

        private sealed class SetInfo
        {
            public string Code;
            public string Name;
            public int Cards;
        }

        private readonly List<SetInfo> sets;

        /// <summary>The saved format being edited (null: a new one).</summary>
        private CustomFormat original;
        /// <summary>The edited copy; Save writes it.</summary>
        private CustomFormat draft;
        private bool dirty;
        private bool confirmDelete;
        private string status = "";
        private bool statusIsError;
        private string setSearch = "", banSearch = "";
        private Vector2 listScroll, settingsScroll, setScroll, banScroll, bannedScroll;
        private GUIStyle hint;
        private int legalCount = -1;

        public bool WantsClose;

        public FormatEditorScreen(CardDatabase db)
        {
            this.db = db;
            pool = db.Cards.OrderBy(d => d.Name).ToList();
            var names = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var cards = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in db.All)
            {
                if (string.IsNullOrEmpty(p.SetCode)) continue;
                HashSet<string> keys;
                if (!cards.TryGetValue(p.SetCode, out keys))
                {
                    keys = new HashSet<string>();
                    cards[p.SetCode] = keys;
                }
                keys.Add(p.BaseKey);
                if (!names.ContainsKey(p.SetCode) && !string.IsNullOrEmpty(p.PrintSetName)) names[p.SetCode] = p.PrintSetName;
            }
            sets = cards.Select(kv => new SetInfo
            {
                Code = kv.Key,
                Name = names.ContainsKey(kv.Key) ? names[kv.Key] : kv.Key,
                Cards = kv.Value.Count
            }).OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ThenBy(s => s.Code).ToList();
            Open(CustomFormats.All.FirstOrDefault());
        }

        /// <summary>Called when the screen is shown again: reloads the saved formats and keeps editing the same one.</summary>
        public void Reload()
        {
            FormatLibrary.LoadAll();
            WantsClose = false;
            var same = original == null ? null : CustomFormats.Find(original.Name);
            if (!dirty) Open(same ?? CustomFormats.All.FirstOrDefault());
        }

        private void Open(CustomFormat f)
        {
            original = f;
            draft = f != null ? f.Clone() : NewFormat();
            dirty = f == null;
            confirmDelete = false;
            legalCount = -1;
            status = f == null ? "A new format: set it up and press Save." : "";
            statusIsError = false;
        }

        private CustomFormat NewFormat()
        {
            var f = new CustomFormat { Name = UniqueName("My Format") };
            return f;
        }

        private static string UniqueName(string name)
        {
            if (CustomFormats.Find(name) == null) return name;
            for (int i = 2; ; i++)
                if (CustomFormats.Find(name + " " + i) == null) return name + " " + i;
        }

        private void Changed()
        {
            dirty = true;
            legalCount = -1;
            draft.Invalidate();
        }

        private void SetStatus(string text, bool error)
        {
            status = text;
            statusIsError = error;
        }

        // ------------------------------------------------------------------ Actions

        private void Save()
        {
            string oldName = original != null ? original.Name : null;
            var error = FormatLibrary.Save(draft, original);
            if (error != null)
            {
                SetStatus(error, true);
                return;
            }
            int renamed = oldName != null ? FormatLibrary.RenameInDecks(oldName, draft.Name) : 0;
            var saved = CustomFormats.Find(draft.Name);
            Open(saved);
            SetStatus("Saved \"" + draft.Name + "\" to " + FormatLibrary.Folder + "." + (renamed > 0 ? " " + renamed + " deck" + (renamed == 1 ? "" : "s") + " now name the new name." : ""), false);
        }

        private void Delete()
        {
            confirmDelete = false;
            if (original == null)
            {
                Open(CustomFormats.All.FirstOrDefault());
                return;
            }
            string name = original.Name;
            FormatLibrary.Delete(original);
            Open(CustomFormats.All.FirstOrDefault());
            SetStatus("Deleted the format \"" + name + "\". Decks built for it stay saved; pick another format for them in the deck builder.", false);
        }

        private void Duplicate()
        {
            var copy = draft.Clone();
            copy.Name = UniqueName(draft.Name + " copy");
            original = null;
            draft = copy;
            dirty = true;
            legalCount = -1;
            SetStatus("A copy: rename it and press Save.", false);
        }

        // ------------------------------------------------------------------ Drawing

        public void Draw()
        {
            var ui = UiStyles.Get();
            var actions = new List<Action>();
            float pad = 10f;
            float leftW = 240f;
            float h = UiScale.Height - pad * 2;
            float rest = UiScale.Width - leftW - pad * 4;
            float settingsW = Mathf.Max(380f, rest * 0.48f);
            float listsW = Mathf.Max(320f, rest - settingsW);

            DrawList(new Rect(pad, pad, leftW, h), ui, actions);
            DrawSettings(new Rect(pad * 2 + leftW, pad, settingsW, h), ui, actions);
            DrawCardLists(new Rect(pad * 3 + leftW + settingsW, pad, listsW, h), ui, actions);

            foreach (var a in actions) a();
        }

        private GUIStyle Hint(UiStyles ui)
        {
            if (hint == null)
            {
                hint = new GUIStyle(ui.Label) { fontSize = 12, wordWrap = true };
                hint.normal.textColor = new Color(0.8f, 0.8f, 0.85f);
            }
            return hint;
        }

        private void DrawList(Rect area, UiStyles ui, List<Action> actions)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            GUILayout.Label("Your formats", ui.Title);
            GUILayout.Label("Standard, Expanded and Unlimited are built in. Formats you make here show up in the deck builder and the menu.", Hint(ui));
            listScroll = GUILayout.BeginScrollView(listScroll);
            foreach (var f in CustomFormats.All)
            {
                var format = f;
                bool selected = original == format;
                if (GUILayout.Toggle(selected, format.Name, ui.Button) && !selected)
                    actions.Add(() =>
                    {
                        Open(format);
                        if (dirty) SetStatus("", false);
                    });
            }
            if (CustomFormats.All.Count == 0) GUILayout.Label("No formats yet.", Hint(ui));
            GUILayout.EndScrollView();
            if (GUILayout.Button("New format")) actions.Add(() => Open(null));
            GUILayout.Space(8);
            if (GUILayout.Button("Back")) actions.Add(() => WantsClose = true);
            if (dirty) GUILayout.Label("Unsaved changes are kept while you're away, until you open another format.", Hint(ui));
            GUILayout.EndArea();
        }

        private bool Toggle(bool value, string label, string note, UiStyles ui)
        {
            bool v = GUILayout.Toggle(value, " " + label);
            if (!string.IsNullOrEmpty(note)) GUILayout.Label("      " + note, Hint(ui));
            return v;
        }

        private int Stepper(string label, int value, int min, int max, int[] quick)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.Width(110));
            if (GUILayout.Button("-10", GUILayout.Width(40))) value -= 10;
            if (GUILayout.Button("-", GUILayout.Width(30))) value -= 1;
            GUILayout.Label(value.ToString(), GUILayout.Width(36));
            if (GUILayout.Button("+", GUILayout.Width(30))) value += 1;
            if (GUILayout.Button("+10", GUILayout.Width(40))) value += 10;
            if (quick != null)
                foreach (var q in quick)
                    if (GUILayout.Button(q.ToString(), GUILayout.Width(36))) value = q;
            GUILayout.EndHorizontal();
            return Mathf.Clamp(value, min, max);
        }

        private void DrawSettings(Rect area, UiStyles ui, List<Action> actions)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            var f = draft;
            GUILayout.Label(original == null ? "New format" : "Edit format", ui.Title);
            settingsScroll = GUILayout.BeginScrollView(settingsScroll);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", GUILayout.Width(110));
            var name = GUILayout.TextField(f.Name ?? "", 40);
            GUILayout.EndHorizontal();
            if (name != f.Name) { f.Name = name; dirty = true; }
            var nameProblem = CustomFormat.NameProblem(f.Name, CustomFormats.All.Where(x => x != original), null);
            if (nameProblem != null) GUILayout.Label(nameProblem, ui.Error);

            GUILayout.Space(6);
            GUILayout.Label("Decks", ui.Title);
            int size = Stepper("Cards per deck", f.DeckSize, CustomFormat.MinDeckSize, CustomFormat.MaxDeckSize, new[] { 40, 60 });
            if (size != f.DeckSize) { f.DeckSize = size; Changed(); }
            int prizes = Stepper("Prize cards", f.PrizeCount, CustomFormat.MinPrizes, CustomFormat.MaxPrizes, new[] { 1, 3, 4, 6 });
            if (prizes != f.PrizeCount) { f.PrizeCount = prizes; Changed(); }
            if (f.PrizeCount >= f.DeckSize - 7) GUILayout.Label("Too many Prize cards for that deck size (a hand of 7 has to fit too).", ui.Error);

            bool singleton = Toggle(f.Singleton, "Singleton", "Every card except basic Energy is limited to 1 copy (instead of 4).", ui);
            if (singleton != f.Singleton) { f.Singleton = singleton; Changed(); }
            bool ruleBox = Toggle(f.AllowRuleBox, "Rule Box Pokémon allowed", "ex, EX, GX, V, VSTAR, VMAX, Radiant, BREAK, LV.X, Prism Star and the rest.", ui);
            if (ruleBox != f.AllowRuleBox) { f.AllowRuleBox = ruleBox; Changed(); }
            bool ace = Toggle(f.AllowAceSpec, "ACE SPEC cards allowed", "Still 1 ACE SPEC per deck.", ui);
            if (ace != f.AllowAceSpec) { f.AllowAceSpec = ace; Changed(); }

            GUILayout.Space(6);
            GUILayout.Label("Game rules", ui.Title);
            var r = f.Rules;
            bool attack = Toggle(r.FirstTurnAttack, "The player going first can attack on their first turn", null, ui);
            if (attack != r.FirstTurnAttack) { r.FirstTurnAttack = attack; Changed(); }
            bool supporter = Toggle(r.FirstTurnSupporter, "The player going first can play a Supporter on their first turn", null, ui);
            if (supporter != r.FirstTurnSupporter) { r.FirstTurnSupporter = supporter; Changed(); }
            bool drawFirst = Toggle(r.DrawBeforeCoinFlip, "Draw opening hands before the coin flip", "Off: the coin flip decides who goes first, then both players draw.", ui);
            if (drawFirst != r.DrawBeforeCoinFlip) { r.DrawBeforeCoinFlip = drawFirst; Changed(); }
            bool tools = Toggle(r.ToolsAreItems, "Pokémon Tools are Items", "On: Item lock and \"Item card\" effects include Tools. Off: Tools are their own kind of Trainer (as in the built-in formats).", ui);
            if (tools != r.ToolsAreItems) { r.ToolsAreItems = tools; Changed(); }
            bool plain = Toggle(r.OldTrainersArePlain, "Old Trainers are plain Trainer cards",
                "On: Trainer cards first printed before Black & White are just \"Trainer\": not Items (Item lock and Item searches skip them), no Supporter limit (Here Comes Team Rocket!); old Gyms still work as Stadiums.", ui);
            if (plain != r.OldTrainersArePlain) { r.OldTrainersArePlain = plain; Changed(); }

            GUILayout.Space(6);
            if (legalCount < 0) legalCount = pool.Count(d => f.Allows(db, d));
            GUILayout.Label(legalCount + " of " + pool.Count + " cards are legal.", Hint(ui));
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUI.enabled = nameProblem == null && f.PrizeCount < f.DeckSize - 7;
            if (GUILayout.Button(original == null ? "Save new format" : "Save", GUILayout.Height(30))) actions.Add(Save);
            GUI.enabled = original != null && dirty;
            if (GUILayout.Button("Undo changes", GUILayout.Height(30))) actions.Add(() => Open(original));
            GUI.enabled = true;
            if (GUILayout.Button("Duplicate", GUILayout.Height(30))) actions.Add(Duplicate);
            if (!confirmDelete)
            {
                if (GUILayout.Button(original == null ? "Discard" : "Delete", GUILayout.Height(30))) actions.Add(() => confirmDelete = true);
            }
            GUILayout.EndHorizontal();
            if (confirmDelete)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(original == null ? "Discard this new format?" : "Delete \"" + original.Name + "\"?", ui.Error);
                if (GUILayout.Button("Yes", GUILayout.Width(60))) actions.Add(Delete);
                if (GUILayout.Button("No", GUILayout.Width(60))) actions.Add(() => confirmDelete = false);
                GUILayout.EndHorizontal();
            }
            if (!string.IsNullOrEmpty(status)) GUILayout.Label(status, statusIsError ? ui.Error : ui.Good);
            GUILayout.EndArea();
        }

        private void DrawCardLists(Rect area, UiStyles ui, List<Action> actions)
        {
            GUI.Box(area, GUIContent.none);
            GUILayout.BeginArea(new Rect(area.x + 8, area.y + 8, area.width - 16, area.height - 16));
            var f = draft;
            float half = (area.height - 16) * 0.5f;

            GUILayout.Label("Legal sets", ui.Title);
            bool all = GUILayout.Toggle(f.AllSets, " Every set (sets added in later updates too)");
            if (all != f.AllSets)
            {
                f.AllSets = all;
                Changed();
            }
            if (!f.AllSets)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Search", GUILayout.Width(50));
                setSearch = GUILayout.TextField(setSearch ?? "");
                if (GUILayout.Button("All", GUILayout.Width(40))) actions.Add(() => { foreach (var s in ShownSets()) f.Sets.Add(s.Code); Changed(); });
                if (GUILayout.Button("None", GUILayout.Width(50))) actions.Add(() => { foreach (var s in ShownSets()) f.Sets.Remove(s.Code); Changed(); });
                GUILayout.EndHorizontal();
                GUILayout.Label(f.Sets.Count + " of " + sets.Count + " sets chosen. A card is legal when any of its printings is from a chosen set; basic Energy always is.", Hint(ui));
                setScroll = GUILayout.BeginScrollView(setScroll, GUILayout.Height(Mathf.Max(120f, half - 110f)));
                foreach (var s in ShownSets())
                {
                    var set = s;
                    bool on = f.Sets.Contains(set.Code);
                    bool now = GUILayout.Toggle(on, " " + set.Name + "  (" + set.Code + ", " + set.Cards + ")");
                    if (now != on)
                        actions.Add(() =>
                        {
                            if (now) f.Sets.Add(set.Code);
                            else f.Sets.Remove(set.Code);
                            Changed();
                        });
                }
                GUILayout.EndScrollView();
            }

            GUILayout.Space(8);
            GUILayout.Label("Banned cards (" + f.Banned.Count + ")", ui.Title);
            GUILayout.BeginHorizontal();
            GUILayout.Label("Ban", GUILayout.Width(50));
            banSearch = GUILayout.TextField(banSearch ?? "");
            GUILayout.EndHorizontal();
            var needle = (banSearch ?? "").Trim();
            if (needle.Length >= 2)
            {
                var matches = pool.Where(x => x.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 && !f.Banned.Contains(x.BaseKey)).Take(30).ToList();
                banScroll = GUILayout.BeginScrollView(banScroll, GUILayout.Height(120f));
                foreach (var d in matches)
                {
                    var card = d;
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Ban", GUILayout.Width(50))) actions.Add(() => { f.Banned.Add(card.BaseKey); Changed(); });
                    GUILayout.Label(card.Name + "  <size=11>" + card.SetCode + " " + card.Number + "</size>", RichHint(ui));
                    GUILayout.EndHorizontal();
                }
                if (matches.Count == 0) GUILayout.Label("No card by that name (or it's banned already).", Hint(ui));
                GUILayout.EndScrollView();
            }
            else GUILayout.Label("Type 2 or more letters of a card's name to ban it. A ban covers every printing of the card.", Hint(ui));
            bannedScroll = GUILayout.BeginScrollView(bannedScroll);
            foreach (var key in f.Banned.OrderBy(x => x).ToList())
            {
                var k = key;
                var card = db.Get(k);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Unban", GUILayout.Width(60))) actions.Add(() => { f.Banned.Remove(k); Changed(); });
                GUILayout.Label(card != null ? card.Name + "  <size=11>" + card.SetCode + " " + card.Number + "</size>" : k, RichHint(ui));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private GUIStyle richHint;

        private GUIStyle RichHint(UiStyles ui)
        {
            if (richHint == null) richHint = new GUIStyle(ui.Label) { richText = true, wordWrap = true };
            return richHint;
        }

        private IEnumerable<SetInfo> ShownSets()
        {
            var needle = (setSearch ?? "").Trim();
            return needle.Length == 0 ? sets
                : sets.Where(s => s.Name.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0 || s.Code.IndexOf(needle, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
