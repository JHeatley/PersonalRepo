using System;
using System.Collections.Generic;
using System.Linq;
using PokemonTCG.AI;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v175: "The AI builds its own deck" on the menu. The bot builds a deck for the game in a format you pick (Standard, Expanded or
    /// Unlimited), around a card you name or a random one, with the deck advisor (the library's decks and what the AI has learned
    /// about cards). You play against it as usual, and the Neural AI learns from the game the way it does from any game against you.
    /// The built deck can be saved to your decks.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string PrefAiBuilds = "PokemonTCG.AiBuildsDeck";
        private const string PrefAiFormat = "PokemonTCG.AiBuildFormat";
        private const string PrefAiNewEachGame = "PokemonTCG.AiBuildNewEachGame";
        private const string PrefAiAround = "PokemonTCG.AiBuildAround";
        private static readonly string[] AiFormatNames = { "Standard", "Expanded", "Unlimited" };
        private const int AiSearchResults = 6;

        private bool aiPrefsLoaded;
        private bool aiBuilds;
        private DeckFormat aiFormat = DeckFormat.Standard;
        private bool aiNewEachGame = true;
        /// <summary>The card to build around; null for a random one.</summary>
        private CardDef aiAround;
        private string aiAroundText = "";
        private string aiMatchesFor;
        private List<CardDef> aiMatches = new List<CardDef>();
        private Vector2 aiScroll;

        /// <summary>The deck built last (shown on the menu), and whether a game has used it yet.</summary>
        private BuiltDeck aiBuilt;
        private DeckRecord aiBuiltRecord;
        private bool aiBuiltPlayed;
        private string aiStatus;
        private bool aiStatusIsError;

        private volatile bool aiBuildBusy;
        private bool aiStartWhenBuilt;
        private BuiltDeck aiPending;
        private readonly object aiGate = new object();
        private int aiBuildCount;

        // ------------------------------------------------------------------ v177: Japanese card images

        private JapaneseImages jaImages;
        private string jaImagesNote;

        private void ToggleJapaneseImages()
        {
            if (jaImages != null && jaImages.Running)
            {
                jaImages.Stop();
                return;
            }
            jaImages = JapaneseImages.Missing(db);
            if (jaImages.Total == 0)
            {
                jaImagesNote = "Every Japanese card image the game knows of is already here.";
                return;
            }
            jaImagesNote = null;
            jaImages.Start();
        }

        private string JapaneseImagesLine()
        {
            if (jaImages == null) return jaImagesNote ?? "Japanese printings for Japanese mode, saved in each set's \"ja\" folder (about 1 a second).";
            if (jaImages.Running) return "Downloading: " + jaImages.Saved + " saved, " + jaImages.Failed + " failed.";
            if (jaImagesNote != null) return jaImagesNote;
            return "Done: " + jaImages.Saved + " saved, " + jaImages.Failed + " failed" + (jaImages.LastError != null && jaImages.Failed > 0 ? " (last: " + jaImages.LastError + ")" : "") + ".";
        }

        // ------------------------------------------------------------------ v175.1: a random deck from the list

        private const string PrefMyRandomDeck = "PokemonTCG.MyRandomDeck";
        private const string PrefOppRandomDeck = "PokemonTCG.OppRandomDeck";
        private bool randomPrefsLoaded, myRandomDeck, oppRandomDeck;
        /// <summary>Your random deck was picked for a start that's waiting for the AI to build its deck.</summary>
        private bool randomMinePicked;
        /// <summary>The opponent's deck was picked at random: its name stays hidden during the game (you find out by playing it).</summary>
        private bool hideTheirsNext, theirsHidden;
        private readonly System.Random listPicker = new System.Random();

        private void LoadRandomDeckPrefs()
        {
            if (randomPrefsLoaded) return;
            randomPrefsLoaded = true;
            myRandomDeck = PlayerPrefs.GetInt(PrefMyRandomDeck, 0) == 1;
            oppRandomDeck = PlayerPrefs.GetInt(PrefOppRandomDeck, 0) == 1;
        }

        private void SaveRandomDeckPrefs()
        {
            PlayerPrefs.SetInt(PrefMyRandomDeck, myRandomDeck ? 1 : 0);
            PlayerPrefs.SetInt(PrefOppRandomDeck, oppRandomDeck ? 1 : 0);
        }

        /// <summary>The decks a random pick chooses from: the menu's list (its format), ready to play.</summary>
        private List<int> ListDecksReady()
        {
            if (decks == null || deckProblems == null) return new List<int>();
            return Enumerable.Range(0, decks.Count).Where(i => InMenuFormat(i) && i < deckProblems.Count && deckProblems[i].Count == 0).ToList();
        }

        /// <summary>A random deck from the menu's list, or -1 if there's none.</summary>
        private int RandomListDeck()
        {
            var ok = ListDecksReady();
            return ok.Count == 0 ? -1 : ok[listPicker.Next(ok.Count)];
        }

        private void LoadAiDeckPrefs()
        {
            if (aiPrefsLoaded) return;
            aiPrefsLoaded = true;
            aiBuilds = PlayerPrefs.GetInt(PrefAiBuilds, 0) == 1;
            aiFormat = (DeckFormat)Mathf.Clamp(PlayerPrefs.GetInt(PrefAiFormat, (int)DefaultAiFormat()), 0, AiFormatNames.Length - 1);
            aiNewEachGame = PlayerPrefs.GetInt(PrefAiNewEachGame, 1) == 1;
            string around = PlayerPrefs.GetString(PrefAiAround, "");
            aiAround = around.Length > 0 ? db.Get(around) : null;
            if (aiAround != null && !Formats.Allows(aiFormat, aiAround)) aiAround = null;
        }

        /// <summary>The menu's format when it's Standard, Expanded or Unlimited; otherwise Standard.</summary>
        private DeckFormat DefaultAiFormat()
        {
            var chosen = menuFormat.Length > 0 ? GameFormat.Named(menuFormat) : null;
            return chosen != null && !chosen.IsCustom ? chosen.BuiltIn : DeckFormat.Standard;
        }

        private void SaveAiDeckPrefs()
        {
            PlayerPrefs.SetInt(PrefAiBuilds, aiBuilds ? 1 : 0);
            PlayerPrefs.SetInt(PrefAiFormat, (int)aiFormat);
            PlayerPrefs.SetInt(PrefAiNewEachGame, aiNewEachGame ? 1 : 0);
            PlayerPrefs.SetString(PrefAiAround, aiAround != null ? aiAround.Key : "");
        }

        /// <summary>The menu is set to let the AI build its own deck.</summary>
        private bool AiBuildsDeck
        {
            get
            {
                LoadAiDeckPrefs();
                return aiBuilds;
            }
        }

        /// <summary>A new deck is needed before the next game: none yet, the last one failed, or a new one every game.</summary>
        private bool AiDeckNeeded
        {
            get { return aiBuilt == null || !aiBuilt.Ok || (aiNewEachGame && aiBuiltPlayed); }
        }

        /// <summary>The built deck as a deck record for StartGame (marked as played), or null if there's none ready.</summary>
        private DeckRecord TakeAiDeck()
        {
            if (aiBuilt == null || !aiBuilt.Ok) return null;
            if (aiBuiltRecord == null) aiBuiltRecord = new DeckRecord { Name = aiBuilt.Name, Text = aiBuilt.Text, BuiltIn = false };
            aiBuiltPlayed = true;
            return aiBuiltRecord;
        }

        /// <summary>Builds a deck on a background thread; thenStart starts the game when it's done.</summary>
        private void StartAiBuild(bool thenStart)
        {
            if (aiBuildBusy) return;
            aiBuildBusy = true;
            aiStartWhenBuilt = thenStart;
            aiStatus = null;
            var fmt = aiFormat;
            var around = aiAround;
            var records = decks != null ? decks.ToList() : new List<DeckRecord>();
            var b = brain;
            var cards = db;
            int seed = unchecked(Environment.TickCount * 31 + ++aiBuildCount);
            var worker = new System.Threading.Thread(() =>
            {
                BuiltDeck result;
                try
                {
                    var library = new List<AdvisorDeck>();
                    foreach (var record in records)
                    {
                        var problems = new List<string>();
                        var list = DeckLibrary.Build(cards, record, problems);
                        if (list.Count == 60 && problems.Count == 0) library.Add(new AdvisorDeck(record.Name, list));
                    }
                    result = RandomDeckBuilder.Build(cards, fmt, library, b, around, new System.Random(seed));
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    result = new BuiltDeck { Format = fmt, Error = "The deck builder ran into a problem: " + ex.Message };
                }
                lock (aiGate) aiPending = result;
                aiBuildBusy = false;
            });
            worker.IsBackground = true;
            worker.Name = "AI deck";
            worker.Start();
        }

        /// <summary>Picks up a finished build (from Update). A build for a game starts it.</summary>
        private void UpdateAiBuild()
        {
            BuiltDeck done;
            lock (aiGate)
            {
                done = aiPending;
                aiPending = null;
            }
            if (done == null) return;
            if (!done.Ok)
            {
                aiStatus = done.Error ?? "The deck couldn't be built.";
                aiStatusIsError = true;
                // A game that was waiting for the deck (Play again) goes back to the menu, where the reason is shown.
                if (aiStartWhenBuilt && mode == Mode.Game) pendingMenu = true;
                aiStartWhenBuilt = false;
                if (aiBuilt != null && !aiBuilt.Ok) aiBuilt = null;
                return;
            }
            aiBuilt = done;
            aiBuiltRecord = null;
            aiBuiltPlayed = false;
            aiStatus = null;
            if (aiStartWhenBuilt)
            {
                aiStartWhenBuilt = false;
                pendingStart = true;
            }
        }

        /// <summary>The opponent's column when the AI builds its deck: format, the card to build around, and the deck it built.</summary>
        private void DrawAiDeckPanel(UiStyles ui, float listHeight, List<Action> actions)
        {
            LoadAiDeckPrefs();
            GUILayout.Label(Loc.T("Format for the AI's deck"), Hint(ui));
            int fmtPick = GUILayout.Toolbar((int)aiFormat, Loc.T(AiFormatNames));
            if (fmtPick != (int)aiFormat)
            {
                var picked = (DeckFormat)fmtPick;
                actions.Add(() =>
                {
                    aiFormat = picked;
                    if (aiAround != null && !Formats.Allows(aiFormat, aiAround)) aiAround = null;
                    aiMatchesFor = null;
                    SaveAiDeckPrefs();
                });
            }

            GUILayout.Space(4);
            GUILayout.Label(Loc.T(aiAround != null ? "Built around: " + aiAround.Name + " (" + aiAround.Key + ")" : "Built around: a random Pokémon"), ui.Label);
            GUILayout.BeginHorizontal();
            GUILayout.Label(Loc.T("Find a card"), GUILayout.Width(90));
            string text = GUILayout.TextField(aiAroundText ?? "");
            if (aiAround != null && GUILayout.Button(Loc.T("Random"), GUILayout.Width(80))) actions.Add(() => { aiAround = null; SaveAiDeckPrefs(); });
            GUILayout.EndHorizontal();
            if (text != aiAroundText) actions.Add(() => aiAroundText = text);
            if (aiMatchesFor != aiAroundText)
            {
                aiMatchesFor = aiAroundText;
                aiMatches = RandomDeckBuilder.Search(db, aiFormat, aiAroundText, AiSearchResults);
            }
            if (!string.IsNullOrWhiteSpace(aiAroundText))
            {
                if (aiMatches.Count == 0) GUILayout.Label(Loc.T("No card with that name in " + Formats.Label(aiFormat) + "."), Hint(ui));
                foreach (var match in aiMatches)
                {
                    var m = match;
                    if (GUILayout.Button(Loc.T(m.Name + "  (" + m.Key + ")"), ui.Button))
                        actions.Add(() =>
                        {
                            aiAround = m;
                            aiAroundText = "";
                            SaveAiDeckPrefs();
                        });
                }
            }

            bool newEach = GUILayout.Toggle(aiNewEachGame, Loc.T(" A new deck every game"));
            if (newEach != aiNewEachGame) actions.Add(() => { aiNewEachGame = newEach; SaveAiDeckPrefs(); });

            GUILayout.BeginHorizontal();
            GUI.enabled = !aiBuildBusy;
            if (GUILayout.Button(Loc.T(aiBuildBusy ? "Building..." : aiBuilt != null && aiBuilt.Ok ? "Build another" : "Build it now"), GUILayout.Height(26)))
                actions.Add(() => StartAiBuild(false));
            GUI.enabled = !aiBuildBusy && aiBuilt != null && aiBuilt.Ok;
            if (GUILayout.Button(Loc.T("Save to my decks"), GUILayout.Height(26))) actions.Add(SaveAiDeck);
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (aiStatus != null) GUILayout.Label(Loc.T(aiStatus), aiStatusIsError ? ui.Error : ui.Good);

            float height = Mathf.Max(90f, listHeight - 200f);
            aiScroll = GUILayout.BeginScrollView(aiScroll, GUILayout.Height(height));
            if (aiBuilt != null && aiBuilt.Ok)
            {
                GUILayout.Label(Loc.T(aiBuilt.Name + " (" + Formats.Label(aiBuilt.Format) + ")" + (aiBuiltPlayed && aiNewEachGame ? ": played; the next game gets a new one" : "")), ui.Title);
                foreach (var entry in Decklist.Group(aiBuilt.Cards).OrderBy(e => e.Card.IsPokemon ? 0 : e.Card.IsTrainer ? 1 : 2))
                    GUILayout.Label(Loc.T(entry.Count + "  " + entry.Card.Name + (entry.Card.IsPokemon ? "  " + entry.Card.Key : "")), ui.Label);
                foreach (var note in aiBuilt.Notes.Take(3)) GUILayout.Label(Loc.T(note), Hint(ui));
            }
            else
            {
                GUILayout.Label(Loc.T(aiBuildBusy ? "The AI is building its deck..."
                    : "The AI builds its deck when the game starts (or press Build it now to see it first). Leave the card blank for a random one."), Hint(ui));
            }
            GUILayout.EndScrollView();
        }

        private void SaveAiDeck()
        {
            if (aiBuilt == null || !aiBuilt.Ok) return;
            try
            {
                string name = aiBuilt.Name;
                var taken = new HashSet<string>(decks.Select(d => d.Name), StringComparer.OrdinalIgnoreCase);
                for (int i = 2; taken.Contains(name); i++) name = aiBuilt.Name + " " + i;
                string text = Decklist.Format(name, Decklist.Group(aiBuilt.Cards), aiBuilt.Format);
                DeckLibrary.Save(name, text);
                RefreshDecks();
                aiStatus = "Saved as \"" + name + "\" in your decks.";
                aiStatusIsError = false;
            }
            catch (Exception e)
            {
                aiStatus = "Couldn't save the deck: " + e.Message;
                aiStatusIsError = true;
            }
        }
    }
}
