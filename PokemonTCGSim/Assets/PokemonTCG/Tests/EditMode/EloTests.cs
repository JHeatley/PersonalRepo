using System;
using System.Linq;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>v157: your Elo rating (AI/PlayerRating.cs): the result, the play-quality nudge, placement, saving and what to work on.</summary>
    public class EloTests
    {
        private static readonly DateTime When = new DateTime(2026, 10, 21, 12, 0, 0);

        private static GameReview Review(int compared, int differences, double[] gains, params string[] habits)
        {
            var r = new GameReview { DecisionsCompared = compared, Differences = differences };
            foreach (var g in gains)
                r.Items.Add(new ReviewItem { Section = ReviewItem.BetterPlays, Title = "Better play", YourChance = 0.4, SuggestedChance = 0.4 + g });
            foreach (var h in habits) r.Items.Add(new ReviewItem { Title = h });
            return r;
        }

        [Test]
        public void A_First_Win_Against_An_Equal_Opponent_Moves_20_From_The_Result()
        {
            var rating = new PlayerRating();
            Assert.AreEqual(1500, rating.Rating);
            var g = rating.Record(1, PlayerRating.LearningAi, "Mine", "Theirs", 9, null, When);
            Assert.AreEqual(20, g.FromResult, 1e-9, "K 40 while placing, expected 0.5");
            Assert.AreEqual(0, g.FromPlay, "no review: result only");
            Assert.AreEqual(1520, rating.Rating, 1e-9);
            Assert.AreEqual(1492, rating.OpponentRating(PlayerRating.LearningAi), 1e-9, "the AI loses 16 x 0.5");
            Assert.AreEqual(1, rating.Wins);
            Assert.IsTrue(rating.Provisional);
        }

        [Test]
        public void Play_Quality_Nudges_The_Rating_Win_Or_Lose()
        {
            var perfect = Review(20, 0, new double[0]);
            var poor = Review(20, 10, new[] { 0.3, 0.3 }, "Missed Energy attachments", "A thin Bench");
            Assert.AreEqual(100, PlayerRating.QualityOf(perfect), 1e-9);
            Assert.AreEqual(20, PlayerRating.QualityOf(poor), 1e-9, "60 x 0.5 + 0 - 10");
            Assert.AreEqual(-1, PlayerRating.QualityOf(null));

            var a = new PlayerRating();
            var lossPlayedWell = a.Record(0, PlayerRating.ClassicBot, "D", "E", 8, perfect, When);
            Assert.AreEqual(12, lossPlayedWell.FromPlay, 1e-9, "quality 100 while placing: +12");
            var b = new PlayerRating();
            var winPlayedBadly = b.Record(1, PlayerRating.ClassicBot, "D", "E", 8, poor, When);
            Assert.AreEqual(-12, winPlayedBadly.FromPlay, 1e-9);
            Assert.IsTrue(winPlayedBadly.Flags.Contains("A thin Bench"));
        }

        [Test]
        public void After_Placement_K_Drops_To_20()
        {
            var r = new PlayerRating();
            for (int i = 0; i < PlayerRating.PlacementGames; i++) r.Record(i % 2, PlayerRating.ClassicBot, "D", "E", 8, null, When);
            Assert.IsFalse(r.Provisional);
            double before = r.Rating;
            double expected = PlayerRating.Expected(before, r.OpponentRating(PlayerRating.ClassicBot));
            r.Record(1, PlayerRating.ClassicBot, "D", "E", 8, null, When);
            Assert.AreEqual(before + 20 * (1 - expected), r.Rating, 1e-9);
        }

        [Test]
        public void The_Rating_Saves_And_Loads()
        {
            var r = new PlayerRating();
            r.Record(1, PlayerRating.LearningAi, "Dragapult ex", "Gardevoir ex", 11, Review(10, 2, new[] { 0.1 }, "Turns without a Supporter"), When);
            r.Record(0.5, PlayerRating.ClassicBot, "Dragapult ex", "Charizard ex", 30, null, When.AddHours(1));
            var back = PlayerRating.Parse(r.Serialize());
            Assert.AreEqual(r.Rating, back.Rating, 1e-6);
            Assert.AreEqual(2, back.Games);
            Assert.AreEqual(1, back.Draws);
            Assert.AreEqual(2, back.History.Count);
            Assert.AreEqual("Gardevoir ex", back.History[0].TheirDeck);
            Assert.IsTrue(back.History[0].Flags.Contains("Turns without a Supporter"));
            Assert.AreEqual(r.History[0].Quality, back.History[0].Quality, 1e-3);
            Assert.AreEqual(-1, back.History[1].Quality);
            Assert.AreEqual(r.OpponentRating(PlayerRating.LearningAi), back.OpponentRating(PlayerRating.LearningAi), 1e-6);
        }

        [Test]
        public void What_To_Work_On_Comes_From_The_Habits_Flagged_Most_Often()
        {
            var r = new PlayerRating();
            Assert.IsTrue(r.Improvements()[0].Contains("Play a game"));
            for (int i = 0; i < 6; i++)
                r.Record(0, PlayerRating.ClassicBot, "D", "E", 8, Review(20, 3, new double[0], "Missed Energy attachments"), When);
            var list = r.Improvements();
            Assert.IsTrue(list[0].StartsWith("Missed Energy attachments: in 6 of your last 6 games"), list[0]);
            Assert.IsTrue(list.Any(l => l.StartsWith("Deck: D")), "a deck that keeps losing is mentioned");
            Assert.AreEqual("Learning the basics", PlayerRating.Level(1100));
            Assert.AreEqual("League regular", PlayerRating.Level(1500));
        }
    }
}
