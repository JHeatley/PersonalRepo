using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>Conceding: the game ends with the other player winning, whether the one conceding is being asked or not, locally and online.</summary>
    public class ConcedeTests : ScenarioTestBase
    {
        /// <summary>Plays like the classic bot; at its nth decision it concedes for a seat (its own: while being asked).</summary>
        private sealed class Conceder : IPlayerAgent
        {
            private readonly HeuristicAgent bot = new HeuristicAgent(4);
            public GameEngine Engine;
            public int At, Seat, For;
            private int asked;

            public List<int> Decide(DecisionRequest r)
            {
                if (++asked == At)
                {
                    Engine.Concede(For);
                    if (For == Seat) throw new ConcedeException();
                }
                return bot.Decide(r);
            }
        }

        private IList<CardDef> Deck0 { get { return UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck); } }
        private IList<CardDef> Deck1 { get { return UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck); } }

        [Test]
        public void Conceding_While_Being_Asked_Ends_The_Game_As_A_Loss()
        {
            var me = new Conceder { At = 8, Seat = 1, For = 1 };
            var engine = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(3), me);
            me.Engine = engine;
            var result = engine.Run(Deck0, Deck1, 11);
            Assert.AreEqual(0, result.Winner, "the other player wins");
            StringAssert.Contains("conceded", result.Reason);
        }

        [Test]
        public void Conceding_While_The_Opponent_Thinks_Ends_The_Game_At_Their_Next_Decision()
        {
            var them = new Conceder { At = 10, Seat = 0, For = 1 }; // seat 1 concedes while seat 0 is choosing
            var engine = new GameEngine(StandardCards.CreateRegistry(), them, new HeuristicAgent(5));
            them.Engine = engine;
            var result = engine.Run(Deck0, Deck1, 12);
            Assert.AreEqual(0, result.Winner);
            StringAssert.Contains("conceded", result.Reason);
        }

        [Test]
        public void The_Concede_Button_Stops_Your_Waiting_Decision_And_You_Lose()
        {
            var human = new HumanAgent();
            var engine = new GameEngine(StandardCards.CreateRegistry(), human, new HeuristicAgent(6));
            engine.PlayerNames = new[] { "You", "Opponent" };
            var host = new EngineHost(engine, human, 0);
            host.Start(Deck0, Deck1, 13);
            var bot = new HeuristicAgent(7);
            int answered = 0;
            for (int t = 0; t < 2000 && !host.Finished; t++)
            {
                var req = human.Pending;
                if (req != null)
                {
                    if (answered == 6)
                    {
                        host.Concede(0); // what the button does
                        break;
                    }
                    human.Submit(bot.Decide(req));
                    answered++;
                }
                Thread.Sleep(5);
            }
            for (int t = 0; t < 400 && !host.Finished; t++) Thread.Sleep(5);
            Assert.IsTrue(host.Finished);
            Assert.AreEqual(1, engine.State.Winner);
            Assert.AreEqual("You conceded.", engine.State.EndReason);
        }

        [Test]
        public void An_Online_Friend_Who_Concedes_Loses_On_The_Hosts_Copy()
        {
            using (var hosting = new OnlineHost(0, "Alex"))
            {
                string hostName;
                var guest = OnlineHost.Join("127.0.0.1:" + hosting.Port, "Sam", "", "", out hostName);
                for (int i = 0; i < 100 && hosting.Guest == null; i++) Thread.Sleep(20);
                var human = new HumanAgent();
                var screen = new RemoteGameHost(guest, new NetCodec(db.Get), human, new[] { "Alex", "Sam" });
                screen.Start();
                var bot = new HeuristicAgent(5);
                int answered = 0;
                var friend = new Thread(() =>
                {
                    while (!screen.Finished)
                    {
                        HostMessage m;
                        while (screen.TryDequeue(out m)) { }
                        var req = human.Pending;
                        if (req != null)
                        {
                            if (answered == 5) screen.Concede(1);
                            else
                            {
                                human.Submit(bot.Decide(req));
                                answered++;
                            }
                        }
                        Thread.Sleep(2);
                    }
                }) { IsBackground = true };
                friend.Start();

                var remote = new RemoteSeat(hosting.Guest, new NetCodec(db.Get), 1, new HeuristicAgent(9));
                var engine = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(3), remote);
                engine.PlayerNames = new[] { "Alex", "Sam" };
                engine.AddObserver(remote);
                var result = engine.Run(Deck0, Deck1, 14);
                hosting.Guest.Send(NetMessageType.End, (byte[])null);
                Assert.IsTrue(friend.Join(20000));
                Assert.IsTrue(remote.Conceded);
                Assert.AreEqual(0, result.Winner, "the host wins");
                Assert.AreEqual("Sam conceded.", result.Reason);
                guest.Close("done");
            }
        }
    }
}
