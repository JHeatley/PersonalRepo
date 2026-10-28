using System.Collections.Generic;
using System.IO;
using System.Linq;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Your custom formats (v123), one text file each in the "Formats" folder inside your decks folder, so they move with your
    /// decks and can be shared like them. Loaded into CustomFormats when the card pool loads and after every change.
    /// </summary>
    public static class FormatLibrary
    {
        public const string FolderName = "Formats";

        public static string Folder { get { return Path.Combine(SavePaths.DeckFolder, FolderName); } }

        /// <summary>The file each loaded format came from (renaming a format replaces its file).</summary>
        private static readonly Dictionary<CustomFormat, string> files = new Dictionary<CustomFormat, string>();

        public static void LoadAll()
        {
            var list = new List<CustomFormat>();
            files.Clear();
            try
            {
                if (Directory.Exists(Folder))
                {
                    foreach (var path in Directory.GetFiles(Folder, "*.txt").OrderBy(p => p))
                    {
                        CustomFormat f;
                        try { f = CustomFormat.Parse(File.ReadAllText(path)); }
                        catch (IOException) { continue; }
                        if (f == null || CustomFormat.NameProblem(f.Name, list, null) != null)
                        {
                            Debug.LogWarning("[PokemonTCG] Skipped the format file " + Path.GetFileName(path) + " (no name, a built-in name, or a name used twice).");
                            continue;
                        }
                        list.Add(f);
                        files[f] = path;
                    }
                }
            }
            catch (IOException e)
            {
                Debug.LogWarning("[PokemonTCG] Couldn't read the formats folder: " + e.Message);
            }
            CustomFormats.Set(list);
        }

        /// <summary>Saves a format (new or edited) and reloads the list. Returns an error message, or null.</summary>
        public static string Save(CustomFormat format, CustomFormat replacing)
        {
            var others = CustomFormats.All.Where(f => f != replacing).ToList();
            var problem = CustomFormat.NameProblem(format.Name, others, null);
            if (problem != null) return problem;
            format.Name = format.Name.Trim();
            try
            {
                Directory.CreateDirectory(Folder);
                var path = Path.Combine(Folder, DeckLibrary.SafeFileName(format.Name) + ".txt");
                string old;
                if (replacing != null && files.TryGetValue(replacing, out old) && old != path && File.Exists(old)) File.Delete(old);
                File.WriteAllText(path, format.ToText());
            }
            catch (IOException e)
            {
                return "Couldn't save the format: " + e.Message;
            }
            LoadAll();
            return null;
        }

        public static void Delete(CustomFormat format)
        {
            string path;
            if (format != null && files.TryGetValue(format, out path))
            {
                try { if (File.Exists(path)) File.Delete(path); }
                catch (IOException e) { Debug.LogWarning("[PokemonTCG] Couldn't delete the format: " + e.Message); }
            }
            LoadAll();
        }

        /// <summary>
        /// After a format is renamed, the saved decks built for it get the new name on their "# Format:" line. Returns how many
        /// decks changed.
        /// </summary>
        public static int RenameInDecks(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName) || string.Equals(oldName.Trim(), newName.Trim(), System.StringComparison.Ordinal)) return 0;
            int n = 0;
            foreach (var deck in DeckLibrary.LoadAll())
            {
                if (deck.BuiltIn || string.IsNullOrEmpty(deck.FilePath)) continue;
                if (!deck.FormatName.Equals(oldName.Trim(), System.StringComparison.OrdinalIgnoreCase)) continue;
                var lines = deck.Text.Replace("\r", "").Split('\n')
                    .Select(l => l.Trim().StartsWith("# Format:") ? "# Format: " + newName.Trim() : l);
                try
                {
                    File.WriteAllText(deck.FilePath, string.Join("\n", lines.ToArray()));
                    n++;
                }
                catch (IOException) { }
            }
            return n;
        }
    }
}
