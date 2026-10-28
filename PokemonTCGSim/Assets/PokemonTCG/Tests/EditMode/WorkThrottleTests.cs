using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using PokemonTCG.AI;

namespace PokemonTCG.Tests
{
    /// <summary>v120: the battery saver's throttle for the background AI work.</summary>
    public class WorkThrottleTests
    {
        [TearDown]
        public void Reset()
        {
            WorkThrottle.Paused = false;
            WorkThrottle.BusyShare = 1.0;
            WorkThrottle.MaxThreads = 0;
        }

        [Test]
        public void Full_Speed_Never_Rests()
        {
            var watch = Stopwatch.StartNew();
            WorkThrottle.Rest(TimeSpan.FromSeconds(5), null);
            Assert.Less(watch.ElapsedMilliseconds, 50);
        }

        [Test]
        public void Half_Speed_Rests_As_Long_As_The_Game_Took()
        {
            WorkThrottle.BusyShare = 0.5;
            var watch = Stopwatch.StartNew();
            WorkThrottle.Rest(TimeSpan.FromMilliseconds(300), null);
            Assert.GreaterOrEqual(watch.ElapsedMilliseconds, 280);
            Assert.Less(watch.ElapsedMilliseconds, 1500);
        }

        [Test]
        public void A_Stop_Ends_The_Rest_At_Once()
        {
            WorkThrottle.BusyShare = 0.1;
            var watch = Stopwatch.StartNew();
            WorkThrottle.Rest(TimeSpan.FromSeconds(5), () => true);
            Assert.Less(watch.ElapsedMilliseconds, 200);
        }

        [Test]
        public void Paused_Waits_Until_Resumed()
        {
            WorkThrottle.Paused = true;
            var resume = new Thread(() =>
            {
                Thread.Sleep(400);
                WorkThrottle.Paused = false;
            });
            var watch = Stopwatch.StartNew();
            resume.Start();
            WorkThrottle.Rest(TimeSpan.Zero, null);
            resume.Join();
            Assert.GreaterOrEqual(watch.ElapsedMilliseconds, 350);
        }

        [Test]
        public void The_Thread_Cap_Limits_The_Workers()
        {
            Assert.AreEqual(6, WorkThrottle.Threads(6));
            WorkThrottle.MaxThreads = 2;
            Assert.AreEqual(2, WorkThrottle.Threads(6));
            Assert.AreEqual(1, WorkThrottle.Threads(1));
            Assert.AreEqual(1, WorkThrottle.Threads(0));
            WorkThrottle.BusyShare = 0.01;
            Assert.AreEqual(0.1, WorkThrottle.BusyShare, 1e-9, "never below 10%");
        }
    }
}
