using System.Collections.Generic;
using System.IO;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>Loads card data (every JSON file in Resources/CardData) and deck lists.</summary>
    public static class UnityCardData
    {
        public const string CardDataFolder = "CardData";
        public const string DefaultDeck = "Decklists/dragapult";
        public const string ZoroarkDeck = "Decklists/n_zoroark";

        public const string PrintingsResource = "CardPrintings/printings";

        public static CardDatabase LoadAllCards()
        {
            var db = new CardDatabase();
            var assets = Resources.LoadAll<TextAsset>(CardDataFolder);
            if (assets.Length == 0) Debug.LogError("[PokemonTCG] No card data found in Resources/" + CardDataFolder);
            foreach (var asset in assets.OrderBy(a => a.name))
            {
                var file = JsonUtility.FromJson<CardDataFile>(asset.text);
                db.AddAll(file);
            }
            // Alternate printings (other arts and reprints) for the deck builder's art picker.
            var printings = Resources.Load<TextAsset>(PrintingsResource);
            if (printings != null) db.AddPrintings(JsonUtility.FromJson<PrintingsFile>(printings.text));
            // Image folders named after a set give it to files named with just the card number.
            CardVisuals.UseSetsFrom(db);
            // Custom formats (v123) check cards against this pool; load them now so decks can name them.
            CustomFormats.Database = db;
            try { FormatLibrary.LoadAll(); }
            catch (System.Exception e) { Debug.LogWarning("[PokemonTCG] Couldn't load the custom formats: " + e.Message); }
            return db;
        }

        public static List<CardDef> LoadDeck(CardDatabase db, string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);
            if (asset == null)
            {
                Debug.LogError("[PokemonTCG] Deck list not found in Resources: " + resourcePath);
                return new List<CardDef>();
            }
            return ParseDeck(db, asset.text, resourcePath);
        }

        public static List<CardDef> ParseDeck(CardDatabase db, string text, string label)
        {
            var errors = new List<string>();
            var entries = Decklist.Parse(text, db, errors);
            foreach (var e in errors) Debug.LogWarning("[PokemonTCG] " + label + ": " + e);
            var deck = Decklist.Expand(entries);
            var format = GameFormat.Named(Decklist.ReadFormatName(text)) ?? GameFormat.Standard;
            foreach (var p in format.Validate(deck)) Debug.LogWarning("[PokemonTCG] " + label + ": " + p);
            return deck;
        }
    }

    /// <summary>A deck the player can pick: built in (Resources/Decklists) or saved by the player.</summary>
    public sealed class DeckRecord
    {
        public string Name;
        public string Text;
        public bool BuiltIn;
        /// <summary>File path for saved decks; null for built-in ones.</summary>
        public string FilePath;

        /// <summary>The built-in format: Standard unless the list has a "# Format: Expanded" or "# Format: Unlimited" line. A deck for a
        /// custom format (IsCustomFormat) isn't in any of them.</summary>
        public DeckFormat Format { get { return Decklist.ReadFormat(Text); } }

        /// <summary>The name on the deck's "# Format:" line ("Standard" without one), a custom format's included (v123).</summary>
        public string FormatName { get { return Decklist.ReadFormatName(Text); } }

        /// <summary>The deck's format, or null when it names a custom format that doesn't exist (any more).</summary>
        public GameFormat ResolvedFormat { get { return GameFormat.Named(FormatName); } }

        /// <summary>Built for a format that isn't Standard, Expanded or Unlimited (a custom format, v123).</summary>
        public bool IsCustomFormat
        {
            get
            {
                var f = ResolvedFormat;
                return f == null || f.IsCustom;
            }
        }

        public string Label
        {
            get
            {
                var f = FormatName;
                bool standard = f.Equals("Standard", System.StringComparison.OrdinalIgnoreCase);
                return (BuiltIn ? Name + "  (built-in)" : Name) + (standard ? "" : "  [" + (ResolvedFormat != null ? ResolvedFormat.Name : f + "?") + "]");
            }
        }
    }

    /// <summary>
    /// Built-in decks plus the player's own decks, saved as plain text in the decks folder (SavePaths:
    /// Application.persistentDataPath/Decks unless you chose another one) so they survive updates and can be edited or shared by hand.
    /// </summary>
    public static class DeckLibrary
    {
        public static string UserFolder
        {
            get { return SavePaths.DeckFolder; }
        }

        public static List<DeckRecord> LoadAll()
        {
            var list = new List<DeckRecord>();
            var hidden = HiddenBuiltIns();
            foreach (var asset in Resources.LoadAll<TextAsset>("Decklists"))
            {
                // A built-in deck you deleted in the deck builder stays hidden (v123); Restore built-in decks brings it back.
                if (hidden.Contains(Decklist.ReadName(asset.text) ?? asset.name)) continue;
                list.Add(new DeckRecord
                {
                    Name = Decklist.ReadName(asset.text) ?? asset.name,
                    Text = asset.text,
                    BuiltIn = true
                });
            }
            list = list.OrderBy(d => d.Name).ToList();

            var user = new List<DeckRecord>();
            if (Directory.Exists(UserFolder))
            {
                foreach (var path in Directory.GetFiles(UserFolder, "*.txt"))
                {
                    string text;
                    try { text = File.ReadAllText(path); }
                    catch (IOException) { continue; }
                    user.Add(new DeckRecord
                    {
                        Name = Decklist.ReadName(text) ?? Path.GetFileNameWithoutExtension(path),
                        Text = text,
                        BuiltIn = false,
                        FilePath = path
                    });
                }
            }
            list.AddRange(user.OrderBy(d => d.Name));
            return list;
        }

        /// <summary>Saves (or overwrites) a player deck. Returns the saved record.</summary>
        public static DeckRecord Save(string name, string text)
        {
            Directory.CreateDirectory(UserFolder);
            var path = Path.Combine(UserFolder, SafeFileName(name) + ".txt");
            File.WriteAllText(path, text);
            return new DeckRecord { Name = name, Text = text, BuiltIn = false, FilePath = path };
        }

        public static void Delete(DeckRecord deck)
        {
            if (deck == null || deck.BuiltIn || string.IsNullOrEmpty(deck.FilePath)) return;
            if (File.Exists(deck.FilePath)) File.Delete(deck.FilePath);
        }

        // Built-in decks can't be deleted from the game's files, so deleting one hides it (v123), in a file next to your decks.
        private const string HiddenFileName = "hidden built-in decks.cfg";

        private static string HiddenPath { get { return Path.Combine(UserFolder, HiddenFileName); } }

        public static HashSet<string> HiddenBuiltIns()
        {
            var set = new HashSet<string>();
            try
            {
                if (File.Exists(HiddenPath))
                    foreach (var line in File.ReadAllLines(HiddenPath))
                        if (line.Trim().Length > 0) set.Add(line.Trim());
            }
            catch (IOException) { }
            return set;
        }

        private static void WriteHidden(HashSet<string> set)
        {
            try
            {
                Directory.CreateDirectory(UserFolder);
                if (set.Count == 0) { if (File.Exists(HiddenPath)) File.Delete(HiddenPath); }
                else File.WriteAllLines(HiddenPath, set.OrderBy(n => n).ToArray());
            }
            catch (IOException e) { Debug.LogWarning("[PokemonTCG] Couldn't save the hidden decks: " + e.Message); }
        }

        /// <summary>Deletes a saved deck's file, or hides a built-in deck (v123).</summary>
        public static void Remove(DeckRecord deck)
        {
            if (deck == null) return;
            if (!deck.BuiltIn)
            {
                Delete(deck);
                return;
            }
            var set = HiddenBuiltIns();
            set.Add(deck.Name);
            WriteHidden(set);
        }

        /// <summary>Shows every built-in deck again. Returns how many were hidden.</summary>
        public static int RestoreBuiltIns()
        {
            int n = HiddenBuiltIns().Count;
            WriteHidden(new HashSet<string>());
            return n;
        }

        public static string SafeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var chars = (name ?? "").Trim().Select(c => invalid.Contains(c) ? '_' : c).ToArray();
            var result = new string(chars).Trim();
            return result.Length == 0 ? "My Deck" : result;
        }

        /// <summary>Parses a deck record against the card pool. Problems are returned, not logged.</summary>
        public static List<CardDef> Build(CardDatabase db, DeckRecord deck, List<string> problems)
        {
            var entries = Decklist.Parse(deck.Text, db, problems);
            var cards = Decklist.Expand(entries);
            var format = deck.ResolvedFormat;
            if (format == null) problems.Add("The deck's format \"" + deck.FormatName + "\" doesn't exist. Make it under Formats, or pick another format in the deck builder.");
            else problems.AddRange(format.Validate(cards));
            return cards;
        }
    }
}
