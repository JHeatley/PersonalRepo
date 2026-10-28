using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>v71: Regional Sim "Tables at once": how many AI tables play in the background, changed while the event runs.</summary>
    public class RegionalTablesAtOnceTests
    {
        [Test]
        public void The_Suggested_Tables_Are_The_Processor_Threads_Less_2_From_1_To_64()
        {
            Assert.AreEqual(14, RegionalSim.SuggestedTablesAtOnce(16));
            Assert.AreEqual(10, RegionalSim.SuggestedTablesAtOnce(12));
            Assert.AreEqual(30, RegionalSim.SuggestedTablesAtOnce(32));
            Assert.AreEqual(2, RegionalSim.SuggestedTablesAtOnce(4));
            Assert.AreEqual(1, RegionalSim.SuggestedTablesAtOnce(2));
            Assert.AreEqual(1, RegionalSim.SuggestedTablesAtOnce(1));
            Assert.AreEqual(64, RegionalSim.SuggestedTablesAtOnce(128));
            Assert.AreEqual(64, RegionalSim.MaxTablesAtOnce);
        }

        [Test]
        public void The_Tables_At_Once_Change_While_The_Event_Runs()
        {
            // No cards: each AI game counts as a draw at once, and the round then waits for your match with the workers idle.
            var decks = new[] { "Test Deck" };
            var sim = RegionalSim.Create(5, 2, false, "Standard", "Test Deck", decks, decks.ToDictionary(n => n, n => ""), false, 7);
            sim.Begin();
            try
            {
                sim.StartWorkers(4);
                Assert.AreEqual(4, sim.TablesAtOnce);
                Assert.AreEqual(4, sim.WorkerCount);
                Assert.IsTrue(sim.WorkersRunning);
                for (int wait = 0; wait < 100 && sim.View().RoundFinished < 2; wait++) Thread.Sleep(50);
                Assert.AreEqual(2, sim.View().RoundFinished, "both AI tables were played in the background");
                Assert.AreEqual(1, sim.Round, "the round waits for your match");

                sim.StartWorkers(2);
                Assert.AreEqual(2, sim.WorkerCount, "two of them stop");
                sim.StartWorkers(6);
                Assert.AreEqual(6, sim.WorkerCount, "four more start");
                sim.StartWorkers(0);
                Assert.AreEqual(1, sim.TablesAtOnce, "at least 1");
                sim.StartWorkers(1000);
                Assert.AreEqual(RegionalSim.MaxTablesAtOnce, sim.TablesAtOnce, "at most 64");
                Assert.AreEqual(RegionalSim.MaxTablesAtOnce, sim.WorkerCount);

                sim.StopWorkers(3000);
                Assert.AreEqual(0, sim.WorkerCount);
                Assert.IsFalse(sim.WorkersRunning);
                sim.StartWorkers(3);
                Assert.AreEqual(3, sim.WorkerCount, "paused and resumed with a new number");
                Assert.IsTrue(sim.WorkersRunning);

                Assert.IsTrue(sim.RecordYourGame(1, 0, 1));
                for (int wait = 0; wait < 100 && sim.Round < 2; wait++) Thread.Sleep(50);
                Assert.AreEqual(2, sim.Round, "round 2 is paired and its AI tables play on");
                for (int wait = 0; wait < 100 && sim.View().RoundFinished < 2; wait++) Thread.Sleep(50);
                Assert.AreEqual(2, sim.View().RoundFinished);
            }
            finally
            {
                sim.StopWorkers(3000);
            }
        }
    }
}
