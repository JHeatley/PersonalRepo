using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Cards;
using PokemonTCG.Core;
using UnityEngine;

namespace PokemonTCG.Presentation
{
    /// <summary>
    /// Your online friend in your Regional Sim event (the host's copy runs it): they join the event with their deck; their
    /// games against AI players are played here in the background and shown on their screen through the network; your match
    /// against them is your own match, played between you; they're kept told how the event stands, and dropped if their
    /// connection is lost.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private bool regWithFriend = true;
        private volatile bool friendGameRunning, friendReady, friendDropped;
        private int friendGameRound, friendGameIndex;
        private volatile int friendGameResult = int.MinValue;
        private volatile string friendGameLost;
        private int friendNoteVersion = -1;
        private float friendNoteTime;
        private CancellationTokenSource friendCancel;

        private bool FriendConnected { get { return onlineHost != null && onlineHost.Guest != null && !onlineHost.Guest.Closed; } }

        /// <summary>The Regional Sim setup row: your online friend plays in the event too (when someone has joined you).</summary>
        private void DrawRegionalFriendRow(List<Action> actions)
        {
            if (regAiOnly || !FriendConnected) return;
            var h = onlineHost;
            bool with = GUILayout.Toggle(regWithFriend, " " + h.GuestName + " (online) plays in this event too, with "
                                                  + (string.IsNullOrEmpty(h.GuestDeckName) ? "their deck" : h.GuestDeckName)
                                                  + ": their games against AI players are played on their screen, and your match against them is played between you");
            if (with != regWithFriend) actions.Add(() => regWithFriend = with);
        }

        /// <summary>Adds the online friend to a new event (false, with regionalError, when their deck isn't legal).</summary>
        private bool AddOnlineFriend(RegionalSim sim)
        {
            if (!FriendConnected) return true; // nobody there: the event goes ahead without them
            var h = onlineHost;
            string deckName = h.GuestName + "'s " + (string.IsNullOrEmpty(h.GuestDeckName) ? "deck" : h.GuestDeckName);
            var problems = new List<string>();
            DeckLibrary.Build(db, new DeckRecord { Name = deckName, Text = h.GuestDeckText ?? "" }, problems);
            if (problems.Count > 0)
            {
                regionalError = h.GuestName + "'s deck isn't legal: " + string.Join(" ", problems.ToArray());
                return false;
            }
            try
            {
                sim.AddGuest(h.GuestName, deckName, h.GuestDeckText);
            }
            catch (Exception e)
            {
                regionalError = e.Message;
                return false;
            }
            friendReady = true;
            friendDropped = false;
            friendNoteVersion = -1;
            return true;
        }

        /// <summary>Your Regional match against your online friend, played over the network; null when they're not connected.</summary>
        private OnlineMatch RegionalFriendMatch(RegionalEntrant friend)
        {
            if (!FriendConnected || friendGameRunning) return null;
            return new OnlineMatch { Connection = onlineHost.Guest, HostName = OnlineDisplayName, GuestName = friend.Name, GuestDeckName = friend.Deck };
        }

        /// <summary>
        /// Every frame, hosting an event your friend plays in: records their finished game, starts their next game against an AI
        /// player once they're ready, keeps them told how the event stands, and drops them if their connection is lost.
        /// </summary>
        private void UpdateRegionalFriend()
        {
            var sim = regional;
            var h = onlineHost;
            if (sim == null || h == null) return;
            var guest = sim.Guest();
            if (guest == null) return;
            if (!friendGameRunning && friendGameResult != int.MinValue)
            {
                int result = friendGameResult;
                friendGameResult = int.MinValue;
                if (sim.RecordGuestGame(friendGameRound, friendGameIndex, result)) SaveRegional();
                try
                {
                    if (FriendConnected) h.Guest.Send(NetMessageType.End, (byte[])null);
                }
                catch (IOException)
                {
                }
                if (friendGameLost != null) learnStatus = guest.Name + " lost their connection during a game (" + friendGameLost + "); the classic bot finished it.";
            }
            if (!friendGameRunning && !friendDropped && !FriendConnected && onlineMatch == null)
            {
                friendDropped = true;
                if (sim.DropGuest()) SaveRegional();
                learnStatus = guest.Name + " left, so they're dropped from the event.";
                return;
            }
            if (!friendGameRunning && friendReady && FriendConnected && onlineMatch == null && sim.GuestGamePending) StartFriendGame(sim, guest, h);
            if (FriendConnected && sim.Version != friendNoteVersion && Time.time >= friendNoteTime)
            {
                friendNoteVersion = sim.Version;
                friendNoteTime = Time.time + 1f;
                string note = RegionalNoteFor(sim, guest);
                try
                {
                    h.Guest.Send(NetMessageType.Note, w => w.Write(note));
                }
                catch (IOException)
                {
                }
            }
        }

        /// <summary>Starts your friend's next game against an AI player: played here in the background, shown on their screen.</summary>
        private void StartFriendGame(RegionalSim sim, RegionalEntrant guest, OnlineHost h)
        {
            var m = sim.GuestMatch();
            if (m == null) return;
            var ai = sim.Entrant(m.A == guest.Id ? m.B : m.A);
            var p1 = new List<string>();
            var p2 = new List<string>();
            var guestDeck = DeckLibrary.Build(db, new DeckRecord { Name = guest.Deck, Text = sim.DeckText(guest.Deck) ?? "" }, p1);
            var aiDeck = DeckLibrary.Build(db, new DeckRecord { Name = ai.Deck, Text = sim.DeckText(ai.Deck) ?? "" }, p2);
            if (p1.Count > 0 || p2.Count > 0)
            {
                regionalError = "Couldn't start " + guest.Name + "'s game: " + string.Join(" ", p1.Concat(p2).ToArray());
                friendReady = false;
                return;
            }
            int guestSeat = m.A == guest.Id ? 0 : 1;
            int seed = (Environment.TickCount & 0x0FFFFFFF) | 1;
            friendCancel = new CancellationTokenSource();
            var remote = new RemoteSeat(h.Guest, new NetCodec(db.Get), guestSeat, new HeuristicAgent(seed)) { Cancel = friendCancel.Token };
            IPlayerAgent aiAgent = brain != null
                ? (IPlayerAgent)new LearningAgent(brain, seed + 1, ai.Deck, guest.Deck) { Training = false, Record = false, Style = AiStyle.Neural }
                : new HeuristicAgent(seed + 1);
            var engine = new GameEngine(StandardCards.CreateRegistry(), guestSeat == 0 ? remote : aiAgent, guestSeat == 0 ? aiAgent : remote);
            engine.AddObserver(remote);
            engine.CardsPerPlayer = guestDeck.Count == aiDeck.Count ? guestDeck.Count : 0;
            engine.PlayerNames = guestSeat == 0 ? new[] { guest.Name, ai.Name } : new[] { ai.Name, guest.Name };
            int chooser = RegionalSim.OrderChooserFor(m); // from A's side
            int seatOfA = m.A == guest.Id ? guestSeat : 1 - guestSeat;
            engine.OrderChooser = chooser < 0 ? -1 : chooser == 0 ? seatOfA : 1 - seatOfA;
            engine.OrderChooserIsHigherSeed = RegionalSim.HigherSeedChoosesFor(m) && chooser >= 0;
            engine.TieBreaker = RegionalSim.TieBreakerNext(m);
            string label = "Regional Sim, " + RegionalSim.RoundName(m.Round, sim.Rounds, sim.TopCut, sim.PlayInRounds) + ", table " + m.Table
                           + ", game " + (m.Games.Count + 1) + " against " + ai.Name;
            try
            {
                h.Guest.Send(NetMessageType.Start, w =>
                {
                    w.Write(guestSeat);
                    w.Write(engine.PlayerNames[0]);
                    w.Write(engine.PlayerNames[1]);
                    // In your own Regional match you find out your opponent's deck by playing it (v134.7).
                    w.Write(guestSeat == 0 ? guest.Deck : "your opponent's deck");
                    w.Write(guestSeat == 0 ? "your opponent's deck" : guest.Deck);
                    w.Write(label);
                });
            }
            catch (IOException)
            {
                return;
            }
            friendGameRound = m.Round;
            friendGameIndex = m.Games.Count;
            friendGameLost = null;
            friendReady = false;
            friendGameRunning = true;
            var d0 = guestSeat == 0 ? guestDeck : aiDeck;
            var d1 = guestSeat == 0 ? aiDeck : guestDeck;
            var thread = new Thread(() =>
            {
                int result = int.MinValue;
                try
                {
                    var r = engine.Run(d0, d1, seed);
                    result = r.Winner < 0 ? 0 : r.Winner == guestSeat ? 1 : -1;
                    friendGameLost = remote.Lost;
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    friendGameLost = "the game stopped: " + e.Message;
                    result = 0;
                }
                finally
                {
                    friendGameResult = result;
                    friendGameRunning = false;
                }
            }) { IsBackground = true, Name = "Pokemon TCG friend's Regional game" };
            thread.Start();
        }

        /// <summary>How the event stands, for your friend's screen: the round, their record and their match.</summary>
        private string RegionalNoteFor(RegionalSim sim, RegionalEntrant guest)
        {
            var v = sim.View();
            var st = v.StandingOf(guest.Id);
            var lines = new List<string>();
            lines.Add("Regional Sim hosted by " + OnlineDisplayName + ": " + (v.Finished ? "finished" : RegionalSim.RoundName(v.Round, v.Rounds, v.TopCut, v.PlayInRounds))
                      + " · " + v.Players + " players");
            if (st != null) lines.Add("Your record: " + st.Record + (st.Rank > 0 ? " (place " + st.Rank + ")" : ""));
            if (friendDropped) lines.Add("You were dropped from the event.");
            var m = sim.GuestMatch();
            if (!v.Finished && m != null)
            {
                if (m.IsBye) lines.Add("You have a bye this round.");
                else
                {
                    var opp = sim.Entrant(m.A == guest.Id ? m.B : m.A);
                    string score = m.Games.Count == 0 ? "" : " (games played: " + m.Games.Count + ")";
                    if (m.Finished) lines.Add("Your match against " + opp.Name + " is done" + score + ". Waiting for the other tables.");
                    else if (opp.Human) lines.Add("This round you play " + opp.Name + " at table " + m.Table + score + ": they start each game.");
                    else lines.Add("This round: table " + m.Table + " against " + opp.Name + score + ". Press Ready when you want your next game.");
                }
            }
            if (v.Finished && guest.Id < v.Placement.Length) lines.Add("You finished in place " + v.Placement[guest.Id] + ".");
            return string.Join("\n", lines.ToArray());
        }

        /// <summary>The friend's side: ready for your next Regional game (the host's event starts it).</summary>
        private void SendOnlineReady()
        {
            var g = onlineGuest;
            if (g == null || g.Closed) return;
            try
            {
                g.Send(NetMessageType.Ready, (byte[])null);
                onlineStatus = "Ready: your next game starts as soon as the host's event has one for you.";
            }
            catch (IOException)
            {
            }
        }
    }
}
