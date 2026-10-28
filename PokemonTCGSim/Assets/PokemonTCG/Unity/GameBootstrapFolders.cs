using System;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// The Save Folders screen: where the learning AI's brain and your saved decks are kept on this computer
    /// (SavePaths.cs). Folder changes, the folder picker and "Open folder" all run from Update, outside the GUI passes.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const int FolderBrain = 1, FolderDecks = 2;

        private string brainFolderText, deckFolderText;
        private string foldersMessage;
        private bool foldersMessageIsError;
        private bool pendingFolders, pendingFoldersBack;
        private int pendingBrowse;
        private int pendingApplyWhich;
        private string pendingApplyFolder;
        private string pendingOpenFolder;
        /// <summary>True while the AI learns from a finished game on a background thread.</summary>
        private volatile bool learningFromGame;

        private void OpenFoldersScreen()
        {
            SavePaths.Load();
            brainFolderText = SavePaths.BrainFolder;
            deckFolderText = SavePaths.DeckFolder;
            foldersMessage = null;
            foldersMessageIsError = false;
            mode = Mode.Folders;
        }

        /// <summary>Why the AI's folder can't change right now, or null.</summary>
        private string BrainFolderBlocked()
        {
            if (trainView != null && trainView.Running) return "Training is running. Stop it or let it finish before moving the AI's files.";
            if (learningFromGame) return "The AI is still learning from your last game. Try again in a moment.";
            return null;
        }

        /// <summary>Called from Update.</summary>
        private void UpdateFolders()
        {
            if (pendingFolders)
            {
                pendingFolders = false;
                OpenFoldersScreen();
            }
            if (pendingFoldersBack)
            {
                pendingFoldersBack = false;
                RefreshDecks();
                mode = Mode.Menu;
            }
            if (pendingBrowse != 0)
            {
                int which = pendingBrowse;
                pendingBrowse = 0;
                string start = which == FolderBrain ? SavePaths.BrainFolder : SavePaths.DeckFolder;
                string title = which == FolderBrain ? "Folder for the AI's brain" : "Folder for your saved decks";
                string picked = SavePaths.BrowseForFolder(title, start);
                if (picked != null) ApplyFolder(which, picked);
            }
            if (pendingApplyWhich != 0)
            {
                int which = pendingApplyWhich;
                string folder = pendingApplyFolder;
                pendingApplyWhich = 0;
                pendingApplyFolder = null;
                ApplyFolder(which, folder);
            }
            if (pendingOpenFolder != null)
            {
                string folder = pendingOpenFolder;
                pendingOpenFolder = null;
                SavePaths.OpenInFileBrowser(folder);
            }
        }

        private void ApplyFolder(int which, string folder)
        {
            string message;
            bool ok;
            if (which == FolderBrain)
            {
                string blocked = BrainFolderBlocked();
                if (blocked != null)
                {
                    foldersMessage = blocked;
                    foldersMessageIsError = true;
                    return;
                }
                // Write the brain out first so the copy in the new folder is up to date.
                if (brain != null)
                {
                    try { brain.Save(); }
                    catch (Exception e)
                    {
                        foldersMessage = "Couldn't save the AI before moving it: " + e.Message + " Nothing was changed.";
                        foldersMessageIsError = true;
                        return;
                    }
                }
                ok = SavePaths.ChangeBrainFolder(folder, out message);
                if (ok)
                {
                    brainError = null;
                    LoadBrain();
                }
                brainFolderText = SavePaths.BrainFolder;
            }
            else
            {
                ok = SavePaths.ChangeDeckFolder(folder, out message);
                if (ok) RefreshDecks();
                deckFolderText = SavePaths.DeckFolder;
            }
            foldersMessage = message;
            foldersMessageIsError = !ok;
        }

        private void DrawFolders(UiStyles ui)
        {
            float w = Mathf.Min(900f, UiScale.Width - 40f);
            var r = new Rect(UiScale.Width / 2f - w / 2f, 30f, w, UiScale.Height - 60f);
            GUI.Box(r, GUIContent.none);
            GUILayout.BeginArea(new Rect(r.x + 16, r.y + 16, r.width - 32, r.height - 32));
            GUILayout.Label("Save folders", ui.Header);
            GUILayout.Label("Where the learning AI's brain and your saved decks are kept on this computer. When you choose a new folder, "
                            + "the files in the current one are copied there (files already in the new folder are kept), and the old files "
                            + "stay where they were as a backup.", ui.Label);
            GUILayout.Space(10);

            DrawFolderRow(ui, FolderBrain, "Learning AI (brain.txt and feedback.txt)", ref brainFolderText,
                SavePaths.BrainFolder, SavePaths.DefaultBrainFolder, SavePaths.BrainFolderIsDefault, BrainFolderBlocked());
            GUILayout.Space(14);
            DrawFolderRow(ui, FolderDecks, "Saved decks", ref deckFolderText,
                SavePaths.DeckFolder, SavePaths.DefaultDeckFolder, SavePaths.DeckFolderIsDefault, null);

            GUILayout.Space(10);
            if (!string.IsNullOrEmpty(foldersMessage)) GUILayout.Label(foldersMessage, foldersMessageIsError ? ui.Error : ui.Good);
            if (!SavePaths.CanBrowse) GUILayout.Label("Type or paste a full folder path, then press Use this folder.", Hint(ui));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Back to menu", GUILayout.Height(32), GUILayout.Width(160))) pendingFoldersBack = true;
            GUILayout.EndArea();
        }

        private void DrawFolderRow(UiStyles ui, int which, string title, ref string text, string current, string defaultFolder, bool isDefault, string blocked)
        {
            GUILayout.Label(title, ui.Title);
            GUILayout.Label("Now: " + current + (isDefault ? "   (default)" : ""), Hint(ui));
            GUILayout.BeginHorizontal();
            text = GUILayout.TextField(text ?? "", GUILayout.MinWidth(300f));
            GUI.enabled = blocked == null;
            if (SavePaths.CanBrowse && GUILayout.Button("Browse...", GUILayout.Width(110))) pendingBrowse = which;
            GUI.enabled = true;
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = blocked == null;
            if (GUILayout.Button("Use this folder", GUILayout.Width(160)))
            {
                pendingApplyWhich = which;
                pendingApplyFolder = text;
            }
            GUI.enabled = blocked == null && !isDefault;
            if (GUILayout.Button("Back to default", GUILayout.Width(160)))
            {
                pendingApplyWhich = which;
                pendingApplyFolder = defaultFolder;
            }
            GUI.enabled = true;
            if (GUILayout.Button("Open folder", GUILayout.Width(130))) pendingOpenFolder = current;
            GUILayout.EndHorizontal();
            if (blocked != null) GUILayout.Label(blocked, Hint(ui));
        }
    }
}
