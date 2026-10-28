using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Areas of Improvement: pops up after each game you play. It shows what went wrong (the Prize race, Knock Outs, the
    /// turns that decided it), habits to work on, and the plays where the AI's simulations say another play gave you a
    /// better chance, each with "Show me" previews on the board (the position before, after your play, after the
    /// suggested play and after the rest of that turn) and "Makes sense" / "I disagree" buttons. Disagreeing teaches the
    /// AI your play for spots like that one; both answers (and your notes) go to feedback.txt in the AI folder (SavePaths).
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private static readonly string[] PreviewNames = { "Before", "Your play", "Suggested play", "Suggested full turn" };

        private GameAnalyzer analyzer;
        private string reviewYourDeck = "", reviewTheirDeck = "";
        private bool reviewOpen;
        private GameReview review;
        private string reviewPhase;
        private int reviewDone, reviewTotal;
        private bool reviewFinished;
        private string reviewError;
        private Vector2 reviewScroll;
        private Rect reviewRect, previewBarRect;
        private GUIStyle reviewText, reviewSmall, reviewBold;
        private string learnStatusShown;

        // Board previews
        private ReviewItem previewItem;
        private int previewStage = -1;
        /// <summary>The board when the game ended (restored after a preview).</summary>
        private PlayerView finalView;

        private void StartReview(GameAnalyzer a, string yourDeck, string theirDeck)
        {
            CancelReview();
            analyzer = a;
            reviewYourDeck = yourDeck;
            reviewTheirDeck = theirDeck;
            reviewOpen = true;
            reviewPhase = "Waiting for the AI to learn from the game";
            reviewDone = reviewTotal = 0;
        }

        private void CancelReview()
        {
            FinishRatedGame(null); // v157: rated by its result if the review didn't finish
            if (analyzer != null) analyzer.Cancel();
            analyzer = null;
            review = null;
            reviewFinished = false;
            reviewError = null;
            reviewOpen = false;
            previewItem = null;
            previewStage = -1;
            reviewScroll = Vector2.zero;
        }

        /// <summary>Called from Update while a game screen is up: copies the analysis progress (one consistent copy per frame).</summary>
        private void UpdateReview()
        {
            learnStatusShown = learnStatus; // written by a background thread; copied once per frame for IMGUI
            if (analyzer == null) return;
            string phase, error;
            int done, total;
            bool finished;
            analyzer.Progress(out phase, out done, out total, out finished, out error);
            reviewPhase = phase;
            reviewDone = done;
            reviewTotal = total;
            reviewError = error;
            if (finished && !reviewFinished)
            {
                reviewFinished = true;
                review = analyzer.Review;
                FinishRatedGame(review); // v157: your Elo moves with the result and your play
            }
        }

        private void ShowPreview(ReviewItem item, int stage)
        {
            PlayerView v = null;
            switch (stage)
            {
                case 0: v = item.Before; break;
                case 1: v = item.AfterYours; break;
                case 2: v = item.AfterSuggested; break;
                case 3: v = item.SuggestedTurnEnd; break;
            }
            if (v == null || board == null) return;
            if (finalView == null) finalView = current;
            previewItem = item;
            previewStage = stage;
            current = v;
            board.Apply(v);
        }

        private void EndPreview()
        {
            previewItem = null;
            previewStage = -1;
            if (finalView != null && board != null)
            {
                current = finalView;
                board.Apply(finalView);
            }
        }

        private void GiveFeedback(ReviewItem item, bool disagree)
        {
            if (item == null || !item.CanGiveFeedback || brain == null) return;
            try
            {
                GameAnalyzer.ApplyFeedback(brain, item, reviewYourDeck, reviewTheirDeck, disagree);
            }
            catch (Exception e)
            {
                learnStatus = "Couldn't apply that: " + e.Message;
                return;
            }
            item.Feedback = disagree ? -1 : 1;
            SaveBrainInBackground();
            AppendFeedback(item, disagree ? "disagree" : "agree", "");
        }

        private void AppendFeedback(ReviewItem item, string kind, string note)
        {
            string line = string.Join("\t", new[]
            {
                DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture), kind, reviewYourDeck, reviewTheirDeck,
                "turn " + item.Turn, item.YourPlay ?? "", item.SuggestedPlay ?? "",
                item.YourChance >= 0 ? Math.Round(item.YourChance * 100) + "% vs " + Math.Round(item.SuggestedChance * 100) + "%" : "",
                (note ?? "").Replace('\t', ' ').Replace('\n', ' ')
            }) + "\n";
            // The paths are read here, on the main thread: Unity's PlayerPrefs and persistentDataPath don't work on others.
            string folder = AiFolder, feedbackPath = FeedbackPath;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    Directory.CreateDirectory(folder);
                    lock (FeedbackLock) File.AppendAllText(feedbackPath, line, new UTF8Encoding(false));
                }
                catch (Exception)
                {
                    // The note is a convenience; the AI already learned from the answer.
                }
            });
        }

        private static readonly object FeedbackLock = new object();

        private void EnsureReviewStyles(UiStyles ui)
        {
            if (reviewText != null) return;
            reviewText = new GUIStyle(ui.Label) { fontSize = 13, wordWrap = true, richText = true };
            reviewSmall = new GUIStyle(ui.Label) { fontSize = 12, wordWrap = true, richText = true };
            reviewSmall.normal.textColor = new Color(0.78f, 0.8f, 0.86f);
            reviewBold = new GUIStyle(ui.Label) { fontSize = 14, wordWrap = true, richText = true, fontStyle = FontStyle.Bold };
        }

        /// <summary>The Areas of Improvement window (right side) and the preview banner (top).</summary>
        private void DrawReview(UiStyles ui)
        {
            reviewRect = new Rect();
            previewBarRect = new Rect();
            if (analyzer == null) return;
            EnsureReviewStyles(ui);
            var actions = new List<Action>();
            if (previewItem != null) DrawPreviewBar(ui, actions);
            if (!reviewOpen)
            {
                foreach (var a in actions) a();
                return;
            }

            float w = Mathf.Min(470f, UiScale.Width * 0.42f);
            reviewRect = new Rect(UiScale.Width - w - 10f, 10f, w, UiScale.Height - 20f);
            GUI.Box(reviewRect, GUIContent.none);
            GUI.Box(reviewRect, GUIContent.none);
            GUILayout.BeginArea(new Rect(reviewRect.x + 10, reviewRect.y + 8, reviewRect.width - 20, reviewRect.height - 16));
            GUILayout.BeginHorizontal();
            GUILayout.Label("Areas of Improvement", ui.Title);
            if (GUILayout.Button("Close", GUILayout.Width(70))) actions.Add(() => { EndPreview(); reviewOpen = false; });
            GUILayout.EndHorizontal();
            var status = learnStatusShown;
            if (!string.IsNullOrEmpty(status)) GUILayout.Label(status, reviewSmall);

            if (!reviewFinished)
            {
                GUILayout.Label(reviewPhase ?? "Working...", reviewText);
                float frac = reviewTotal <= 0 ? 0f : Mathf.Clamp01((float)reviewDone / reviewTotal);
                var bar = GUILayoutUtility.GetRect(reviewRect.width - 30f, 16f);
                GUI.Box(bar, GUIContent.none);
                GUI.DrawTexture(new Rect(bar.x + 2, bar.y + 2, (bar.width - 4) * frac, bar.height - 4), ui.Selected);
                GUILayout.Label("The AI is replaying the key moments of your game many times to compare your plays with other options. This takes a little while; you can close this window and reopen it from the end-of-game box.", reviewSmall);
            }
            else if (review != null)
            {
                reviewScroll = GUILayout.BeginScrollView(reviewScroll);
                DrawReviewBody(ui, actions);
                GUILayout.EndScrollView();
            }
            if (reviewError != null) GUILayout.Label("Part of the review failed: " + reviewError, ui.Error);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Play again")) actions.Add(() => pendingRestart = true);
            if (GUILayout.Button("Back to menu")) actions.Add(() => pendingMenu = true);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
            foreach (var a in actions) a();
        }

        private void DrawReviewBody(UiStyles ui, List<Action> actions)
        {
            var r = review;
            GUILayout.Label(r.Headline, reviewBold);
            if (lastRated != null) GUILayout.Label("Your Elo: " + lastRated.ChangeText + ". Play quality " + (lastRated.Quality >= 0 ? Math.Round(lastRated.Quality) + "/100" : "not measured") + ".", reviewSmall);
            bool won = r.Headline.StartsWith("You won");
            GUILayout.Label(won ? "How the game went" : "What went wrong", ui.Title);
            foreach (var line in r.Summary) GUILayout.Label("• " + line, reviewText);

            var better = r.Items.Where(i => i.Section == ReviewItem.BetterPlays).ToList();
            var habits = r.Items.Where(i => i.Section != ReviewItem.BetterPlays).ToList();

            GUILayout.Space(6);
            GUILayout.Label(ReviewItem.BetterPlays, ui.Title);
            if (better.Count == 0)
                GUILayout.Label("None found: where the AI would have played differently, playing it out didn't change your chances by much.", reviewSmall);
            foreach (var item in better) DrawReviewItem(ui, item, actions);

            GUILayout.Space(6);
            GUILayout.Label(ReviewItem.Habits, ui.Title);
            if (habits.Count == 0) GUILayout.Label("Nothing stood out: the " + r.Judge + " agreed with your plays, or rated them about as well as its own.", reviewSmall);
            foreach (var item in habits) DrawReviewItem(ui, item, actions);

            GUILayout.Space(8);
            GUILayout.Label("How this was worked out: the " + r.Judge + " looked at " + r.DecisionsCompared + " of your decisions and would have done something else in " + r.Differences
                            + (r.Judge == "Neural AI" ? " (it only counts a play when it clearly prefers another; it gets sharper as you train it)" : "")
                            + ". It replayed the " + r.Simulated + " most important ones many times each, from the exact position, with the cards you couldn't see (your deck and Prize cards, "
                            + "your opponent's hand and deck) shuffled each time, and the same luck for both plays. The percentages are estimates (about ±10 points). "
                            + "\"" + ReviewItem.Habits + "\" sorts every play it would have made differently into habits and keeps your history of each across your "
                            + r.HabitReviews + " reviewed game" + (r.HabitReviews == 1 ? "" : "s") + ", so it can tell you what keeps coming up and what you're improving at. "
                            + (r.Items.Any(i => i.CanGiveFeedback)
                                ? "If you think a suggestion is wrong, press \"I disagree\" under it: the AI learns your play for spots like that."
                                : "The \"Makes sense\" and \"I disagree\" buttons come with the plays in the first section, and there were none this game. "
                                  + "The notes under \"" + ReviewItem.Habits + "\" are about your patterns, so they don't have them."), reviewSmall);
        }

        private void DrawReviewItem(UiStyles ui, ReviewItem item, List<Action> actions)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label(item.Title, reviewBold);
            GUILayout.Label(item.Detail, reviewText);
            if (item.YourChance >= 0 && item.SuggestedChance >= 0)
            {
                GUILayout.Label("Your play: " + item.YourPlay + "  →  about " + Math.Round(item.YourChance * 100) + "% to win", reviewSmall);
                GUILayout.Label("Better: " + item.SuggestedPlay + "  →  about " + Math.Round(item.SuggestedChance * 100) + "% to win", reviewSmall);
            }
            if (item.YourTurn.Count > 0)
                GUILayout.Label("What you did that turn: " + string.Join(" → ", item.YourTurn.ToArray()), reviewSmall);

            if (item.CanPreview)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Show me:", GUILayout.Width(70));
                for (int s = 0; s < PreviewNames.Length; s++)
                {
                    int stage = s;
                    bool available = PreviewAvailable(item, stage);
                    GUI.enabled = available;
                    bool on = previewItem == item && previewStage == stage;
                    if (GUILayout.Toggle(on, PreviewNames[s], GUI.skin.button) && !on) actions.Add(() => ShowPreview(item, stage));
                    GUI.enabled = true;
                }
                GUILayout.EndHorizontal();
                if (previewItem == item && previewStage == 3 && item.SuggestedTurn.Count > 0)
                {
                    GUILayout.Label("What the AI would do from here (the preview uses the real order of your deck):", reviewSmall);
                    foreach (var line in item.SuggestedTurn.Take(14)) GUILayout.Label("   " + line, reviewSmall);
                }
            }

            if (item.CanGiveFeedback)
            {
                if (item.Feedback == 0)
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Makes sense")) actions.Add(() => GiveFeedback(item, false));
                    if (GUILayout.Button("I disagree")) actions.Add(() => GiveFeedback(item, true));
                    GUILayout.EndHorizontal();
                }
                else
                {
                    GUILayout.Label(item.Feedback < 0
                        ? "Noted: the AI will lean toward your play in spots like this."
                        : "Noted: the AI will lean a little more toward the suggested play.", ui.Good);
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Note (optional)", GUILayout.Width(100));
                    item.Note = GUILayout.TextField(item.Note ?? "");
                    if (GUILayout.Button("Save", GUILayout.Width(50)) && !string.IsNullOrEmpty(item.Note))
                    {
                        string note = item.Note;
                        actions.Add(() => AppendFeedback(item, "note", note));
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(4);
        }

        private static bool PreviewAvailable(ReviewItem item, int stage)
        {
            switch (stage)
            {
                case 0: return item.Before != null;
                case 1: return item.AfterYours != null;
                case 2: return item.AfterSuggested != null;
                default: return item.SuggestedTurnEnd != null;
            }
        }

        private void DrawPreviewBar(UiStyles ui, List<Action> actions)
        {
            var item = previewItem;
            float w = Mathf.Min(620f, UiScale.Width - 520f);
            if (w < 300f) w = 300f;
            previewBarRect = new Rect(UiScale.Width / 2f - w / 2f - 120f, 10f, w, 64f);
            GUI.Box(previewBarRect, GUIContent.none);
            GUI.Box(previewBarRect, GUIContent.none);
            GUILayout.BeginArea(new Rect(previewBarRect.x + 8, previewBarRect.y + 6, previewBarRect.width - 16, previewBarRect.height - 12));
            string what = previewStage == 1 ? item.YourPlay : previewStage >= 2 ? item.SuggestedPlay : "";
            GUILayout.Label("<b>Preview, turn " + item.Turn + ": " + PreviewNames[Mathf.Clamp(previewStage, 0, 3)] + "</b>" + (string.IsNullOrEmpty(what) ? "" : "  (" + what + ")"), reviewText);
            GUILayout.BeginHorizontal();
            for (int s = 0; s < PreviewNames.Length; s++)
            {
                int stage = s;
                GUI.enabled = PreviewAvailable(item, stage) && previewStage != stage;
                if (GUILayout.Button(PreviewNames[s])) actions.Add(() => ShowPreview(item, stage));
            }
            GUI.enabled = true;
            if (GUILayout.Button("Back to the final board")) actions.Add(EndPreview);
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
