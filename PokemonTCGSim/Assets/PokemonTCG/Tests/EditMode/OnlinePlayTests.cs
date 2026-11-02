using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using PokemonTCG.Presentation;

namespace PokemonTCG.Tests
{
    /// <summary>
    /// Playing a friend online: a host's game where one seat is played over a real (local) TCP connection, the joining side
    /// only ever seeing its own hand, and the messages carrying the game's own objects intact.
    /// </summary>
    public class OnlinePlayTests : ScenarioTestBase
    {
        [Test]
        public void Addresses_Are_Read_With_Or_Without_A_Port()
        {
            string host;
            int port;
            Assert.IsTrue(OnlinePlay.ParseAddress("100.64.0.5", out host, out port));
            Assert.AreEqual("100.64.0.5", host);
            Assert.AreEqual(OnlinePlay.DefaultPort, port);
            Assert.IsTrue(OnlinePlay.ParseAddress(" myfriend.example:8123 ", out host, out port));
            Assert.AreEqual("myfriend.example", host);
            Assert.AreEqual(8123, port);
            Assert.IsTrue(OnlinePlay.ParseAddress("[fd7a:115c::1]:9000", out host, out port));
            Assert.AreEqual("fd7a:115c::1", host);
            Assert.AreEqual(9000, port);
            Assert.IsFalse(OnlinePlay.ParseAddress("", out host, out port));
            Assert.IsFalse(OnlinePlay.ParseAddress("1.2.3.4:notaport", out host, out port));
        }

        [Test]
        public void A_Whole_Game_Is_Played_With_One_Seat_Over_The_Network_Without_Seeing_The_Other_Hand()
        {
            var deck0 = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var deck1 = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            using (var host = new OnlineHost(0, "Host"))
            {
                string hostName;
                var guest = OnlineHost.Join("127.0.0.1:" + host.Port, "Friend", "Zoroark", "4 Zorua", out hostName);
                Assert.AreEqual("Host", hostName);
                for (int i = 0; i < 100 && host.Guest == null; i++) Thread.Sleep(20);
                Assert.NotNull(host.Guest, "the host sees the friend");
                Assert.AreEqual("Friend", host.GuestName);
                Assert.AreEqual("4 Zorua", host.GuestDeckText);

                // The friend's copy: answers each decision with a bot, from what it was sent only.
                var codec = new NetCodec(db.Get);
                int decisions = 0, events = 0, peeks = 0;
                var friend = new Thread(() =>
                {
                    var bot = new HeuristicAgent(5);
                    while (true)
                    {
                        var m = guest.Receive(20000);
                        if (m == null || m.Type == NetMessageType.End || m.Type == NetMessageType.Bye) return;
                        if (m.Type == NetMessageType.Event)
                        {
                            var r = new BinaryReader(new MemoryStream(m.Body));
                            codec.Read(r, typeof(GameEvent));
                            var view = (PlayerView)codec.Read(r, typeof(PlayerView));
                            events++;
                            if (view.Viewer != 1 || (view.Sides[0].Hand.Count > 0 && !view.Sides[0].HandFaceUp)) peeks++;
                        }
                        else if (m.Type == NetMessageType.Decide)
                        {
                            var req = codec.FromBytes<DecisionRequest>(m.Body);
                            decisions++;
                            if (req.Player != 1 || (req.View != null && req.View.Sides[0].Hand.Count > 0 && !req.View.Sides[0].HandFaceUp)) peeks++;
                            guest.Send(NetMessageType.Answer, codec.ToBytes(bot.Decide(req)));
                        }
                    }
                }) { IsBackground = true };
                friend.Start();

                var remote = new RemoteSeat(host.Guest, new NetCodec(db.Get), 1, new HeuristicAgent(9));
                var engine = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(3), remote);
                engine.AddObserver(remote);
                var result = engine.Run(deck0, deck1, 4242);
                host.Guest.Send(NetMessageType.End, (byte[])null);
                Assert.IsTrue(friend.Join(20000), "the friend's side finished");

                Assert.IsNull(remote.Lost, "the friend played the whole game");
                Assert.Greater(decisions, 10);
                Assert.Greater(events, 20);
                Assert.AreEqual(0, peeks, "the friend never saw the host's hand");
                Assert.IsTrue(result.Winner == 0 || result.Winner == 1 || result.Winner < 0);
                guest.Close("done");
            }
        }

        [Test]
        public void If_The_Friend_Leaves_The_Fallback_Finishes_Their_Game()
        {
            var deck0 = UnityCardData.LoadDeck(db, UnityCardData.DefaultDeck);
            var deck1 = UnityCardData.LoadDeck(db, UnityCardData.ZoroarkDeck);
            using (var host = new OnlineHost(0, "Host"))
            {
                string hostName;
                var guest = OnlineHost.Join("127.0.0.1:" + host.Port, "Friend", "", "", out hostName);
                for (int i = 0; i < 100 && host.Guest == null; i++) Thread.Sleep(20);
                guest.Leave("Had to go.");
                var remote = new RemoteSeat(host.Guest, new NetCodec(db.Get), 1, new HeuristicAgent(9));
                var engine = new GameEngine(StandardCards.CreateRegistry(), new HeuristicAgent(3), remote);
                engine.AddObserver(remote);
                engine.Run(deck0, deck1, 77);
                Assert.NotNull(remote.Lost, "noticed the friend left");
            }
        }

        [Test]
        public void A_Different_Version_Is_Turned_Away_With_The_Reason()
        {
            using (var host = new OnlineHost(0, "Host"))
            {
                var c = NetConnection.Connect("127.0.0.1", host.Port, 5000);
                c.Send(NetMessageType.Hello, w =>
                {
                    w.Write(OnlinePlay.Protocol + 1);
                    w.Write(TrainerVersion.Number);
                    w.Write("Old");
                    w.Write("");
                    w.Write("");
                });
                var m = c.Receive(5000);
                Assert.NotNull(m);
                Assert.AreEqual(NetMessageType.Reject, m.Type);
                StringAssert.Contains("same update", new BinaryReader(new MemoryStream(m.Body)).ReadString());
            }
        }
    }
}
