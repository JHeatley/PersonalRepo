using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
using System.Runtime.InteropServices;
using System.Text;
#endif

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Where the learning AI's files (brain.txt, feedback.txt) and your saved decks live on this computer.
    /// Both start in Application.persistentDataPath (AI and Decks); the Save Folders screen can point either one at any
    /// folder, and the choice is kept in PlayerPrefs. When a folder changes, the files in the current folder are copied to
    /// the new one (anything already there is kept) and the originals stay where they were.
    ///
    /// Read the folders on the main thread (Unity's PlayerPrefs and persistentDataPath only work there); background work
    /// should take a copy of the path first.
    /// </summary>
    public static class SavePaths
    {
        private const string PrefBrainFolder = "PokemonTCG.BrainFolder";
        private const string PrefDeckFolder = "PokemonTCG.DeckFolder";

        public const string BrainFileName = "brain.txt";
        public const string FeedbackFileName = "feedback.txt";
        /// <summary>v157: your Elo rating and its game history.</summary>
        public const string RatingFileName = "rating.txt";

        private static string brainFolder, deckFolder;

        public static string DefaultBrainFolder { get { return Path.Combine(Application.persistentDataPath, "AI"); } }
        public static string DefaultDeckFolder { get { return Path.Combine(Application.persistentDataPath, "Decks"); } }

        public static string BrainFolder
        {
            get
            {
                if (brainFolder == null) Load();
                return brainFolder;
            }
        }

        public static string DeckFolder
        {
            get
            {
                if (deckFolder == null) Load();
                return deckFolder;
            }
        }

        public static string BrainPath { get { return Path.Combine(BrainFolder, BrainFileName); } }
        public static string FeedbackPath { get { return Path.Combine(BrainFolder, FeedbackFileName); } }
        public static string RatingPath { get { return Path.Combine(BrainFolder, RatingFileName); } }

        /// <summary>Games played here are saved in "Game logs" in the AI's folder; TCG Live battle logs can go there too (v76).</summary>
        public const string GameLogFolderName = "Game logs";
        public static string GameLogFolder { get { return Path.Combine(BrainFolder, GameLogFolderName); } }

        public static bool BrainFolderIsDefault { get { return SamePath(BrainFolder, DefaultBrainFolder); } }
        public static bool DeckFolderIsDefault { get { return SamePath(DeckFolder, DefaultDeckFolder); } }

        /// <summary>Reads the saved folders (call on the main thread; the getters do it on first use).</summary>
        public static void Load()
        {
            brainFolder = Saved(PrefBrainFolder, DefaultBrainFolder);
            deckFolder = Saved(PrefDeckFolder, DefaultDeckFolder);
        }

        private static string Saved(string key, string fallback)
        {
            string value = PlayerPrefs.GetString(key, "");
            return string.IsNullOrEmpty(value) ? fallback : value;
        }

        // ------------------------------------------------------------------ Changing folders

        /// <summary>
        /// Points the learning AI's files at a new folder. brain.txt and feedback.txt are copied from the current folder
        /// when the new one doesn't have them yet. Save the brain first so the copy is up to date, and reload it after.
        /// </summary>
        public static bool ChangeBrainFolder(string folder, out string message)
        {
            string full;
            if (!Prepare(folder, BrainFolder, "the AI's folder", out full, out message)) return false;
            string old = BrainFolder;
            var copied = new List<string>();
            var kept = new List<string>();
            try
            {
                foreach (var name in new[] { BrainFileName, FeedbackFileName, RatingFileName })
                    CopyIfMissing(Path.Combine(old, name), Path.Combine(full, name), name, copied, kept);
            }
            catch (Exception e)
            {
                message = "Couldn't copy the AI's files to " + full + ": " + e.Message + " Nothing was changed.";
                return false;
            }
            Store(PrefBrainFolder, full, DefaultBrainFolder);
            brainFolder = full;

            message = "The AI now saves to " + full + ".";
            if (copied.Count > 0) message += " Copied " + string.Join(" and ", copied.ToArray()) + " from " + old + " (the originals are still there).";
            if (kept.Contains(BrainFileName)) message += " That folder already had a brain.txt, so the AI now uses that one; the brain from " + old + " wasn't copied over it.";
            else if (copied.Count == 0) message += " There was no brain in " + old + " yet, so the AI starts fresh here.";
            return true;
        }

        /// <summary>Points your saved decks at a new folder, copying every deck file the new folder doesn't have yet.</summary>
        public static bool ChangeDeckFolder(string folder, out string message)
        {
            string full;
            if (!Prepare(folder, DeckFolder, "the decks folder", out full, out message)) return false;
            string old = DeckFolder;
            var copied = new List<string>();
            var kept = new List<string>();
            try
            {
                if (Directory.Exists(old))
                    foreach (var path in Directory.GetFiles(old, "*.txt"))
                    {
                        string name = Path.GetFileName(path);
                        CopyIfMissing(path, Path.Combine(full, name), name, copied, kept);
                    }
            }
            catch (Exception e)
            {
                message = "Couldn't copy your decks to " + full + ": " + e.Message + " Nothing was changed.";
                return false;
            }
            Store(PrefDeckFolder, full, DefaultDeckFolder);
            deckFolder = full;

            message = "Your decks now save to " + full + ".";
            if (copied.Count > 0) message += " Copied " + copied.Count + " deck" + (copied.Count == 1 ? "" : "s") + " from " + old + " (the originals are still there).";
            if (kept.Count > 0) message += " " + kept.Count + " deck file" + (kept.Count == 1 ? " was" : "s were") + " already there with the same name and " + (kept.Count == 1 ? "was" : "were") + " kept as is: " + string.Join(", ", kept.ToArray()) + ".";
            return true;
        }

        /// <summary>Checks the typed or picked folder, creates it and makes sure it can be written to.</summary>
        private static bool Prepare(string folder, string current, string what, out string full, out string message)
        {
            full = null;
            message = null;
            string f = (folder ?? "").Trim().Trim('"').Trim();
            if (f.Length == 0)
            {
                message = "Type or pick a folder first.";
                return false;
            }
            f = Environment.ExpandEnvironmentVariables(f);
            if (f == "~" || f.StartsWith("~/") || f.StartsWith("~\\"))
                f = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), f.Length > 2 ? f.Substring(2) : "");
            if (!Path.IsPathRooted(f))
            {
                message = "Use a full path, for example " + Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "PokemonTCG") + ".";
                return false;
            }
            try
            {
                full = Path.GetFullPath(f).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (full.EndsWith(":")) full += Path.DirectorySeparatorChar; // a drive root such as D:\
            }
            catch (Exception e)
            {
                message = "That isn't a valid folder: " + e.Message;
                return false;
            }
            if (SamePath(full, current))
            {
                message = "That's already " + what + ".";
                return false;
            }
            try
            {
                Directory.CreateDirectory(full);
                string probe = Path.Combine(full, ".pokemontcg-write-test.tmp");
                File.WriteAllText(probe, "ok");
                File.Delete(probe);
            }
            catch (Exception e)
            {
                message = "Can't save to " + full + ": " + e.Message;
                full = null;
                return false;
            }
            return true;
        }

        private static void CopyIfMissing(string from, string to, string name, List<string> copied, List<string> kept)
        {
            if (!File.Exists(from)) return;
            if (File.Exists(to))
            {
                kept.Add(name);
                return;
            }
            File.Copy(from, to);
            copied.Add(name);
        }

        /// <summary>The default folder is stored as "no choice", so it follows persistentDataPath if that ever moves.</summary>
        private static void Store(string key, string folder, string defaultFolder)
        {
            if (SamePath(folder, defaultFolder)) PlayerPrefs.DeleteKey(key);
            else PlayerPrefs.SetString(key, folder);
            PlayerPrefs.Save();
        }

        public static bool SamePath(string a, string b)
        {
            if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return false;
            try
            {
                string x = Path.GetFullPath(a).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                string y = Path.GetFullPath(b).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var comparison = Path.DirectorySeparatorChar == '\\' ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                return string.Equals(x, y, comparison);
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ------------------------------------------------------------------ Folder picker

        /// <summary>A folder picker exists in the Unity editor and in Windows builds; elsewhere the path is typed.</summary>
        public static bool CanBrowse
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
                return true;
#else
                return false;
#endif
            }
        }

        /// <summary>Opens the folder picker (blocks until it closes). Null if cancelled or there's no picker. Call from Update, not OnGUI.</summary>
        public static string BrowseForFolder(string title, string start)
        {
#if UNITY_EDITOR
            string picked = UnityEditor.EditorUtility.OpenFolderPanel(title, Directory.Exists(start) ? start : "", "");
            return string.IsNullOrEmpty(picked) ? null : picked.Replace('/', Path.DirectorySeparatorChar);
#elif UNITY_STANDALONE_WIN
            return WindowsFolderPicker.Pick(title);
#else
            return null;
#endif
        }

        /// <summary>Opens the folder in Explorer / Finder (creating it if needed).</summary>
        public static void OpenInFileBrowser(string folder)
        {
            try
            {
                Directory.CreateDirectory(folder);
                Application.OpenURL("file:///" + folder.Replace('\\', '/').TrimStart('/'));
            }
            catch (Exception)
            {
                // Nothing to open; the path is shown on screen anyway.
            }
        }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        /// <summary>The Windows "Browse For Folder" dialog (shell32), for Windows builds.</summary>
        private static class WindowsFolderPicker
        {
            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            private struct BrowseInfo
            {
                public IntPtr hwndOwner;
                public IntPtr pidlRoot;
                public IntPtr pszDisplayName;
                [MarshalAs(UnmanagedType.LPWStr)] public string lpszTitle;
                public uint ulFlags;
                public IntPtr lpfn;
                public IntPtr lParam;
                public int iImage;
            }

            private const uint ReturnOnlyFileSystemDirs = 0x0001;
            private const uint NewDialogStyle = 0x0040;

            [DllImport("shell32.dll", EntryPoint = "SHBrowseForFolderW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern IntPtr SHBrowseForFolder(ref BrowseInfo info);

            [DllImport("shell32.dll", EntryPoint = "SHGetPathFromIDListW", CharSet = CharSet.Unicode, ExactSpelling = true)]
            private static extern bool SHGetPathFromIDList(IntPtr pidl, StringBuilder path);

            [DllImport("ole32.dll", ExactSpelling = true)]
            private static extern int OleInitialize(IntPtr reserved);

            [DllImport("ole32.dll", ExactSpelling = true)]
            private static extern void OleUninitialize();

            [DllImport("user32.dll", ExactSpelling = true)]
            private static extern IntPtr GetActiveWindow();

            public static string Pick(string title)
            {
                // The resizable dialog needs OLE on this thread; without it, fall back to the classic dialog.
                int hr = OleInitialize(IntPtr.Zero);
                bool ole = hr >= 0;
                IntPtr display = Marshal.AllocHGlobal(1040);
                try
                {
                    var info = new BrowseInfo
                    {
                        hwndOwner = GetActiveWindow(),
                        pszDisplayName = display,
                        lpszTitle = title,
                        ulFlags = ReturnOnlyFileSystemDirs | (ole ? NewDialogStyle : 0u)
                    };
                    IntPtr pidl = SHBrowseForFolder(ref info);
                    if (pidl == IntPtr.Zero) return null;
                    try
                    {
                        var path = new StringBuilder(1040);
                        return SHGetPathFromIDList(pidl, path) && path.Length > 0 ? path.ToString() : null;
                    }
                    finally
                    {
                        Marshal.FreeCoTaskMem(pidl);
                    }
                }
                catch (Exception)
                {
                    return null;
                }
                finally
                {
                    Marshal.FreeHGlobal(display);
                    if (ole) OleUninitialize();
                }
            }
        }
#endif
    }
}
