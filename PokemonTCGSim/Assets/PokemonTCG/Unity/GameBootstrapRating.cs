using System;
using System.Linq;
using PokemonTCG.AI;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// v157: your Elo rating (AI/PlayerRating.cs). Every game you play against the AI (normal games and your Regional Sim
    /// games) is rated when it ends: by its result as soon as the Areas of Improvement review finishes (with your play
    /// quality), or by its result alone if the review is stopped. The menu shows it; "Your rating..." opens the screen with
    /// where it thinks you are, what to work on and your recent games.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private PlayerRating rating;
        private string ratingLoadedFrom;
        /// <summary>The game waiting for its review before it's rated.</summary>
        private PendingRated pendingRated;
        /// <summary>The last game rated (shown at the end of the game and in the review).</summary>
        private RatedGame lastRated;
        private bool pendingRating, pendingRatingBack, pendingRatingReset, ratingResetArmed;
        private Vector2 ratingScroll;
        private string ratingError;

        private sealed class PendingRated
        {
            public double Result;
            public string Opponent, YourDeck, TheirDeck;
            public int Turns;
            public DateTime When;
        }

        /// <summary>Your rating, loaded from the AI folder (again if the folder changed).</summary>
        private PlayerRating Rating
        {
            get
            {
                string path = SavePaths.RatingPath;
                if (rating == null || ratingLoadedFrom != path)
                {
                    rating = PlayerRating.Load(path);
                    ratingLoadedFrom = path;
                }
                return rating;
            }
        }

        /// <summary>Called when a game you played ends (after StartReview).</summary>
        private void BeginRatedGame(GameState state, int seat, string yourDeck, string theirDeck)
        {
            if (state == null || !state.IsOver || seat < 0) return;
            if (OnlineGame) return; // a game against a friend isn't rated against the AI
            bool learning = (regionalGame || opponentLearning) && brain != null;
            pendingRated = new PendingRated
            {
                Result = state.Winner == seat ? 1.0 : state.Winner < 0 ? 0.5 : 0.0,
                Opponent = learning ? PlayerRating.LearningAi : PlayerRating.ClassicBot,
                YourDeck = yourDeck,
                TheirDeck = theirDeck,
                Turns = state.TurnNumber,
                When = DateTime.Now
            };
            lastRated = null;
        }

        /// <summary>Rates the waiting game (review null: by its result only) and saves.</summary>
        private void FinishRatedGame(GameReview finishedReview)
        {
            var p = pendingRated;
            if (p == null) return;
            pendingRated = null;
            try
            {
                var r = Rating;
                lastRated = r.Record(p.Result, p.Opponent, p.YourDeck, p.TheirDeck, p.Turns, finishedReview, p.When);
                r.Save(SavePaths.RatingPath);
                ratingError = null;
            }
            catch (Exception e)
            {
                ratingError = "Couldn't save your rating: " + e.Message;
            }
        }

        /// <summary>Called from Update.</summary>
        private void UpdateRating()
        {
            if (pendingRating)
            {
                pendingRating = false;
                ratingResetArmed = false;
                ratingScroll = Vector2.zero;
                mode = Mode.Rating;
            }
            if (pendingRatingBack)
            {
                pendingRatingBack = false;
                mode = Mode.Menu;
            }
            if (pendingRatingReset)
            {
                pendingRatingReset = false;
                ratingResetArmed = false;
                try
                {
                    rating = new PlayerRating();
                    ratingLoadedFrom = SavePaths.RatingPath;
                    rating.Save(SavePaths.RatingPath);
                    lastRated = null;
                    ratingError = null;
                }
                catch (Exception e)
                {
                    ratingError = "Couldn't reset your rating: " + e.Message;
                }
            }
        }

        private string RatingMenuLine()
        {
            var r = Rating;
            if (r.Games == 0) return "1500 to start. Play a game against the AI and it starts measuring you.";
            string line = Math.Round(r.Rating) + "  (" + PlayerRating.Level(r.Rating) + ")  " + r.Wins + "-" + r.Losses + (r.Draws > 0 ? "-" + r.Draws : "") + " in " + r.Games + " game" + (r.Games == 1 ? "" : "s");
            if (r.Provisional) line += ", placing: " + (PlayerRating.PlacementGames - r.Games) + " to go";
            return line;
        }

        private string RatingEndLine()
        {
            if (pendingRated != null) return "Your Elo updates when the Areas of Improvement review finishes.";
            if (lastRated != null) return "Your Elo: " + lastRated.ChangeText;
            return "";
        }

        private void DrawRating(UiStyles ui)
        {
            var r = Rating;
            float w = Mathf.Min(900f, UiScale.Width - 40f);
            var box = new Rect(UiScale.Width / 2f - w / 2f, 30f, w, UiScale.Height - 60f);
            GUI.Box(box, GUIContent.none);
            GUILayout.BeginArea(new Rect(box.x + 16, box.y + 16, box.width - 32, box.height - 32));
            ratingScroll = GUILayout.BeginScrollView(ratingScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            GUILayout.Label("Your rating", ui.Header);

            GUILayout.Label("Elo " + Math.Round(r.Rating) + "  —  " + PlayerRating.Level(r.Rating), ui.Title);
            if (r.Games == 0) GUILayout.Label("No games rated yet. Every game you play against the AI counts (normal games and your Regional Sim games).", ui.Label);
            else
            {
                GUILayout.Label(r.Wins + " wins, " + r.Losses + " losses" + (r.Draws > 0 ? ", " + r.Draws + " draws" : "") + " in " + r.Games + " games. Peak " + Math.Round(r.Peak) + "."
                                + (r.Provisional ? " Placement: your first " + PlayerRating.PlacementGames + " games move it faster (" + (PlayerRating.PlacementGames - r.Games) + " to go)." : ""), ui.Label);
                double q = r.AverageQuality(10);
                if (q >= 0) GUILayout.Label("Play quality, last 10 reviewed games: " + Math.Round(q) + "/100 (60 is par). " + r.Trend(), ui.Label);
            }

            GUILayout.Space(8);
            GUILayout.Label("What to work on", ui.Title);
            foreach (var line in r.Improvements()) GUILayout.Label("• " + line, ui.Label);

            if (r.Opponents.Count > 0)
            {
                GUILayout.Space(8);
                GUILayout.Label("Your opponents", ui.Title);
                foreach (var kv in r.Opponents.OrderBy(x => x.Key))
                {
                    var games = r.History.Where(g => g.Opponent == kv.Key).ToList();
                    int wins = games.Count(g => g.Result >= 1);
                    GUILayout.Label(kv.Key + ": rated " + Math.Round(kv.Value) + ". You're " + wins + "-" + games.Count(g => g.Result <= 0) + " against it; you'd be expected to win "
                                    + Math.Round(PlayerRating.Expected(r.Rating, kv.Value) * 100) + "% now.", ui.Label);
                }
            }

            var recent = r.Recent(15);
            if (recent.Count > 0)
            {
                GUILayout.Space(8);
                GUILayout.Label("Recent games", ui.Title);
                for (int i = recent.Count - 1; i >= 0; i--)
                {
                    var game = recent[i];
                    GUILayout.Label(game.When.ToString("MMM d HH:mm") + "   " + game.ResultText + "   " + game.YourDeck + " vs " + game.TheirDeck + " (" + game.Opponent + ")   "
                                    + (game.Quality >= 0 ? "play " + Math.Round(game.Quality) + "/100   " : "") + game.ChangeText, Hint(ui));
                }
            }

            GUILayout.Space(8);
            GUILayout.Label("How it's worked out", ui.Title);
            GUILayout.Label("Each game moves your rating two ways. The result: standard Elo against your opponent's rating (the classic bot and the Neural AI each have "
                            + "their own, and it moves too, so beating a stronger AI is worth more). The play: a 0-100 score from the Areas of Improvement review "
                            + "(how often you made the AI's choice, the win chance its simulations say your key mistakes cost, and the habits it flagged). Above 60 "
                            + "adds a little, below takes a little, up to " + PlayerRating.QualityNudge + " a game (" + PlayerRating.PlacementQualityNudge + " while placing), "
                            + "win or lose. The levels are measured against this game's AI, not real tournaments. Saved in " + SavePaths.RatingPath + ".", Hint(ui));
            if (ratingError != null) GUILayout.Label(ratingError, ui.Error);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Back to menu", GUILayout.Height(32), GUILayout.Width(160))) pendingRatingBack = true;
            GUILayout.FlexibleSpace();
            if (!ratingResetArmed)
            {
                if (r.Games > 0 && GUILayout.Button("Reset rating...", GUILayout.Height(32), GUILayout.Width(160))) ratingResetArmed = true;
            }
            else
            {
                GUILayout.Label("Start again from 1500 and forget every rated game?", ui.Error);
                if (GUILayout.Button("Yes, reset", GUILayout.Height(32), GUILayout.Width(110))) pendingRatingReset = true;
                if (GUILayout.Button("Cancel", GUILayout.Height(32), GUILayout.Width(90))) ratingResetArmed = false;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }
    }
}
