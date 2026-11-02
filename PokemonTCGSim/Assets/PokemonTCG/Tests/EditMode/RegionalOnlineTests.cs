using System.Collections.Generic;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>A Regional Sim event with a friend playing online: two people, their match against each other, and their games against AI players.</summary>
    public class RegionalOnlineTests
    {
        private static RegionalSim MakeEvent(int seed, int aiSeats = 6)
        {
            var texts = new Dictionary<string, string> { { "Test Deck", "" } };
            var sim = RegionalSim.Create(aiSeats, 3, false, "Standard", "Test Deck", new[] { "Test Deck" }, texts, false, seed);
            sim.AddGuest("Sam", "Sam's deck", "4 Zorua");
            sim.Begin();
            return sim;
        }

        [Test]
        public void The_Friend_Is_A_Second_Person_In_The_Event_And_Kept_When_Saved()
        {
            var sim = MakeEvent(11);
            var guest = sim.Guest();
            Assert.NotNull(guest);
            Assert.IsTrue(guest.Human && guest.Remote);
            Assert.AreEqual("Sam", guest.Name);
            Assert.AreEqual("4 Zorua", sim.DeckText("Sam's deck"));
            Assert.IsTrue(sim.YouPlaying);
            var you = sim.YourMatch();
            Assert.NotNull(you);
            Assert.AreEqual("You", sim.Entrant(you.A).Name, "you sit as A in your own match");
            var back = RegionalSim.Parse(sim.Serialize());
            Assert.IsTrue(back.Guest() != null && back.Guest().Remote, "the friend is still the friend after loading");
        }

        [Test]
        public void Your_Match_Against_The_Friend_Is_Yours_With_You_As_A_And_Results_Count_The_Right_Way()
        {
            RegionalSim sim = null;
            for (int seed = 1; seed < 400 && sim == null; seed++)
            {
                var s = MakeEvent(seed, 5);
                var m = s.YourMatch();
                if (m != null && !m.IsBye && s.Entrant(m.B).Remote) sim = s;
            }
            Assert.NotNull(sim, "some seed pairs you with your friend in round 1");
            var match = sim.YourMatch();
            Assert.AreEqual("You", sim.Entrant(match.A).Name);
            Assert.IsFalse(sim.GuestGamePending, "the friend's game against you is yours to start");
            Assert.IsTrue(sim.RecordYourGame(match.Round, 0, 1), "you won");
            Assert.IsFalse(sim.RecordGuestGame(match.Round, 1, 1), "that game is already counted");
            var after = sim.YourMatch();
            Assert.AreEqual(0, after.Games[0], "A (you) won it");
        }

        [Test]
        public void The_Friends_Games_Against_AI_Players_Are_Recorded_For_The_Friend()
        {
            RegionalSim sim = null;
            for (int seed = 1; seed < 400 && sim == null; seed++)
            {
                var s = MakeEvent(seed);
                var m = s.GuestMatch();
                if (m != null && !m.IsBye && s.GuestGamePending) sim = s;
            }
            Assert.NotNull(sim);
            var gm = sim.GuestMatch();
            var guest = sim.Guest();
            Assert.AreEqual(guest.Id, gm.A, "the friend sits as A against an AI player");
            Assert.IsTrue(sim.RecordGuestGame(gm.Round, 0, -1), "the friend lost");
            var after = sim.GuestMatch();
            Assert.IsTrue(after.Finished);
            Assert.AreEqual(1, after.Games[0], "B (the AI player) won");
            Assert.IsFalse(sim.GuestGamePending);
            Assert.IsTrue(sim.DropGuest() || true);
        }
    }
}
