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
    /// Play online: one player hosts and friends join by typing the host's address (OnlinePlay). Free: a direct connection,
    /// with the host's port open on their router or both players on a free virtual network (Tailscale, ZeroTier, Radmin VPN).
    /// The host's copy runs the game; the friend's copy shows it and sends their choices.
    /// </summary>
    public sealed partial class GameBootstrap
    {
        private const string PrefOnlineName = "PokemonTCG.OnlineName";
        private const string PrefOnlinePort = "PokemonTCG.OnlinePort";
        private const string PrefOnlineAddress = "PokemonTCG.OnlineAddress";

        /// <summary>An online game the host starts: the friend's connection and both names.</summary>
        private sealed class OnlineMatch
        {
            public NetConnection Connection;
            public string HostName, GuestName, GuestDeckName;
        }

        private bool pendingOnline, pendingOnlineLobby, pendingOnlineStart;
        private string onlineName = "", onlinePortText = "", onlineAddress = "";
        private string onlineStatus, onlineError;
        private int onlineDeck = -1;
        private Vector2 onlineScroll;

        // Hosting
        private OnlineHost onlineHost;
        // Joining
        private volatile NetConnection onlineGuest;
        private volatile bool onlineJoining;
        private string onlineHostName;
        // The game in progress
        private OnlineMatch onlineMatch;
        private RemoteSeat onlineSeat;
        private CancellationTokenSource onlineCancel;
        private bool onlineEndSent;
        /// <summary>The game on the board is online (hosted here, or joined): no Elo rating against the AI. Set as each game starts.</summary>
        private bool onlineGameOnBoard;
        private bool OnlineGame { get { return onlineGameOnBoard; } }

        private void OpenOnline()
        {
            RefreshDecks();
            onlineName = PlayerPrefs.GetString(PrefOnlineName, Environment.UserName ?? "Player");
            onlinePortText = PlayerPrefs.GetString(PrefOnlinePort, OnlinePlay.DefaultPort.ToString());
            onlineAddress = PlayerPrefs.GetString(PrefOnlineAddress, "");
            if (onlineDeck < 0 || onlineDeck >= decks.Count) onlineDeck = myDeck;
            onlineError = null;
            mode = Mode.Online;
        }

        private int OnlinePort
        {
            get
            {
                int p;
                return int.TryParse((onlinePortText ?? "").Trim(), out p) && p > 0 && p < 65536 ? p : OnlinePlay.DefaultPort;
            }
        }

        private bool OnlineDeckReady { get { return onlineDeck >= 0 && onlineDeck < decks.Count && deckProblems[onlineDeck].Count == 0; } }

        private void SaveOnlinePrefs()
        {
            PlayerPrefs.SetString(PrefOnlineName, onlineName ?? "");
            PlayerPrefs.SetString(PrefOnlinePort, onlinePortText ?? "");
            PlayerPrefs.SetString(PrefOnlineAddress, onlineAddress ?? "");
            PlayerPrefs.Save();
        }

        private void StartHosting()
        {
            StopOnline();
            SaveOnlinePrefs();
            try
            {
                onlineHost = new OnlineHost(OnlinePort, OnlineDisplayName);
                onlineStatus = "Waiting for a friend to join on port " + onlineHost.Port + ".";
                onlineError = null;
            }
            catch (Exception e)
            {
                onlineError = "Couldn't host on port " + OnlinePort + " (" + e.Message + "). Another program may be using it: try another port.";
            }
        }

        private void JoinGame()
        {
            StopOnline();
            SaveOnlinePrefs();
            if (!OnlineDeckReady)
            {
                onlineError = "Pick a legal deck first.";
                return;
            }
            var deck = decks[onlineDeck];
            string address = onlineAddress, name = OnlineDisplayName;
            onlineJoining = true;
            onlineError = null;
            onlineStatus = "Connecting to " + address + "...";
            ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    string hostName;
                    var c = OnlineHost.Join(address, name, deck.Name, deck.Text ?? "", out hostName);
                    onlineHostName = hostName;
                    onlineGuest = c;
                    onlineStatus = "Joined " + hostName + "'s game. Waiting for them to start it.";
                }
                catch (Exception e)
                {
                    onlineError = "Couldn't join: " + e.Message;
                    onlineStatus = null;
                }
                finally
                {
                    onlineJoining = false;
                }
            });
        }

        private string OnlineDisplayName
        {
            get
            {
                string n = (onlineName ?? "").Trim();
                if (n.Length == 0 || n == "You") n = "Player";
                return n.Length > 24 ? n.Substring(0, 24) : n;
            }
        }

        /// <summary>Leaves any online game: stops hosting or disconnects from the host.</summary>
        private void StopOnline()
        {
            if (onlineCancel != null) onlineCancel.Cancel();
            if (friendCancel != null) friendCancel.Cancel();
            if (onlineHost != null) onlineHost.Dispose();
            onlineHost = null;
            if (onlineGuest != null) onlineGuest.Leave("They left the game.");
            onlineGuest = null;
            onlineMatch = null;
            onlineSeat = null;
            onlineStatus = null;
        }

        /// <summary>The host starts a game with the friend who joined (their deck from their Hello, yours from the online screen).</summary>
        private void StartOnlineHostGame()
        {
            var h = onlineHost;
            if (h == null || h.Guest == null || h.Guest.Closed)
            {
                onlineError = "Nobody has joined yet.";
                return;
            }
            if (!OnlineDeckReady)
            {
                onlineError = "Pick a legal deck first.";
                return;
            }
            var theirs = new DeckRecord { Name = string.IsNullOrEmpty(h.GuestDeckName) ? h.GuestName + "'s deck" : h.GuestDeckName, Text = h.GuestDeckText ?? "" };
            var problems = new List<string>();
            DeckLibrary.Build(db, theirs, problems);
            if (problems.Count > 0)
            {
                onlineError = h.GuestName + "'s deck isn't legal here: " + string.Join(" ", problems.ToArray());
                try
                {
                    h.Guest.Send(NetMessageType.Note, w => w.Write("Your deck isn't legal for the host: " + string.Join(" ", problems.ToArray())));
                }
                catch (IOException)
                {
                }
                return;
            }
            var match = new OnlineMatch { Connection = h.Guest, HostName = OnlineDisplayName, GuestName = h.GuestName, GuestDeckName = theirs.Name };
            StartGame(decks[onlineDeck], theirs, null, match);
        }

        /// <summary>The friend's side: the host started a game (the Start message): show it, played through RemoteGameHost.</summary>
        private void StartRemoteGame(NetMessage start)
        {
            var r = new BinaryReader(new MemoryStream(start.Body));
            int seat = r.ReadInt32();
            var names = new[] { r.ReadString(), r.ReadString() };
            var decksShown = new[] { r.ReadString(), r.ReadString() };
            string label = r.ReadString();

            StopGame();
            menuError = null;
            regionalGame = false;
            regionalWatch = false;
            errorText = null;
            log.Clear();
            current = null;
            shownRequest = null;
            multiSelection.Clear();
            filterCardId = int.MinValue;
            ResetScreens();
            human = new HumanAgent();
            learningBot = null;
            humanLog = null;
            eventLog = null;
            gameEndHandled = false;
            finalView = null;
            learnStatus = null;
            deckNames[0] = decksShown[0];
            deckNames[1] = decksShown[1];
            viewerSeat = seat;
            board.BottomSeat = seat;
            board.Clear();
            onlineGameOnBoard = true;
            var remote = new RemoteGameHost(onlineGuest, new NetCodec(db.Get), human, names);
            host = remote;
            remote.Start();
            log.Add("Online game: " + label);
            mode = Mode.Game;
        }

        /// <summary>Every frame: friends joining or leaving, the host starting a game, the friend's deck changes.</summary>
        private void UpdateOnline()
        {
            if (pendingOnline)
            {
                pendingOnline = false;
                OpenOnline();
            }
            if (pendingOnlineStart)
            {
                pendingOnlineStart = false;
                StartOnlineHostGame();
            }
            if (pendingOnlineLobby)
            {
                pendingOnlineLobby = false;
                StopGame();
                board.Clear();
                onlineMatch = null;
                onlineSeat = null;
                mode = Mode.Online;
            }
            bool betweenGames = mode != Mode.Game || (host != null && host.Finished);
            // Host: the friend left, or changed their deck.
            var h = onlineHost;
            if (h != null && h.Guest != null && onlineMatch == null && !friendGameRunning) // no game is using their connection
            {
                if (h.Guest.Closed)
                {
                    onlineStatus = h.GuestName + " left (" + h.Guest.Reason + "). Waiting for a friend to join on port " + h.Port + ".";
                    h.Forget();
                }
                else
                {
                    NetMessage m;
                    while ((m = h.Guest.Receive(0)) != null)
                    {
                        var r = new BinaryReader(new MemoryStream(m.Body));
                        if (m.Type == NetMessageType.Deck)
                        {
                            h.GuestDeckName = r.ReadString();
                            h.GuestDeckText = r.ReadString();
                        }
                        else if (m.Type == NetMessageType.Ready) friendReady = true;
                        else if (m.Type == NetMessageType.Bye) h.Guest.Close(r.ReadString());
                    }
                }
            }
            if (h != null && h.Problem != null)
            {
                onlineError = h.Problem;
                h.Problem = null;
            }
            // Friend: the host starts a game, says something, or leaves.
            var g = onlineGuest;
            if (g != null && betweenGames && !(host is RemoteGameHost && !host.Finished))
            {
                NetMessage m;
                while ((m = g.Receive(0)) != null)
                {
                    var r = new BinaryReader(new MemoryStream(m.Body));
                    if (m.Type == NetMessageType.Start)
                    {
                        StartRemoteGame(m);
                        break;
                    }
                    if (m.Type == NetMessageType.Note) onlineStatus = r.ReadString();
                    else if (m.Type == NetMessageType.Bye) g.Close(r.ReadString());
                }
                if (g.Closed && mode == Mode.Online)
                {
                    onlineError = "Disconnected from the host: " + g.Reason;
                    onlineGuest = null;
                    onlineStatus = null;
                }
            }
            UpdateRegionalFriend();
            // Host: tell the friend once this game is over.
            if (onlineMatch != null && !onlineEndSent && host != null && host.Finished && host.QueuedCount == 0)
            {
                onlineEndSent = true;
                try
                {
                    onlineMatch.Connection.Send(NetMessageType.End, (byte[])null);
                }
                catch (IOException)
                {
                }
                if (onlineSeat != null && onlineSeat.Lost != null) log.Add(onlineMatch.GuestName + " left during the game (" + onlineSeat.Lost + "); the classic bot finished their game.");
                onlineMatch = null;
            }
        }

        /// <summary>The host's side of StartGame: the friend's seat is played over the network (call after the engine is made).</summary>
        private IPlayerAgent OnlineSeatFor(OnlineMatch match, int seat, int seed)
        {
            onlineCancel = new CancellationTokenSource();
            onlineSeat = new RemoteSeat(match.Connection, new NetCodec(db.Get), seat, new HeuristicAgent(seed)) { Cancel = onlineCancel.Token };
            onlineMatch = match;
            onlineEndSent = false;
            return onlineSeat;
        }

        /// <summary>Tells the friend's copy a game starts: their seat, both names and decks.</summary>
        private void SendOnlineStart(OnlineMatch match, int guestSeat, string[] names, string[] deckNamesBySeat)
        {
            try
            {
                match.Connection.Send(NetMessageType.Start, w =>
                {
                    w.Write(guestSeat);
                    w.Write(names[0]);
                    w.Write(names[1]);
                    w.Write(deckNamesBySeat[0] ?? "");
                    w.Write(deckNamesBySeat[1] ?? "");
                    w.Write(match.HostName + " vs " + match.GuestName);
                });
            }
            catch (IOException)
            {
                // the friend's seat notices and the fallback plays it
            }
        }

        /// <summary>One line for the menu while you're hosting or connected (null otherwise).</summary>
        private string OnlineMenuLine()
        {
            if (onlineHost != null)
                return onlineHost.Guest != null && !onlineHost.Guest.Closed
                    ? "Online: " + onlineHost.GuestName + " is connected to you (Play online to start a game, or start a Regional Sim event they play in)."
                    : "Online: hosting on port " + onlineHost.Port + ", waiting for a friend to join.";
            if (onlineGuest != null && !onlineGuest.Closed) return "Online: connected to " + onlineHostName + "'s game. Your games start when they start them.";
            return null;
        }

        private void DrawOnline(UiStyles ui)
        {
            var actions = new List<Action>();
            float w = Mathf.Min(900f, UiScale.Width - 40f);
            var box = new Rect(UiScale.Width / 2f - w / 2f, 30f, w, UiScale.Height - 60f);
            GUI.Box(box, GUIContent.none);
            GUILayout.BeginArea(new Rect(box.x + 16, box.y + 16, box.width - 32, box.height - 32));
            onlineScroll = GUILayout.BeginScrollView(onlineScroll, false, false, GUIStyle.none, GUI.skin.verticalScrollbar);
            GUILayout.Label("Play online", ui.Header);
            GUILayout.Label("Play a friend over the internet for free. One of you hosts and the other types the host's address. The host's copy runs "
                            + "the game; nothing hidden (your hand, your deck) is ever sent to the other player. Both of you need the same version of the game.", Hint(ui));

            GUILayout.BeginHorizontal();
            GUILayout.Label("Your name", GUILayout.Width(110));
            onlineName = GUILayout.TextField(onlineName ?? "", 24, GUILayout.Width(220));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Your deck", GUILayout.Width(110));
            string deckLabel = onlineDeck >= 0 && onlineDeck < decks.Count ? decks[onlineDeck].Name : "(none)";
            if (GUILayout.Button("◀", GUILayout.Width(30)) && decks.Count > 0) actions.Add(() => onlineDeck = (onlineDeck - 1 + decks.Count) % decks.Count);
            GUILayout.Label(deckLabel, ui.Label, GUILayout.Width(420));
            if (GUILayout.Button("▶", GUILayout.Width(30)) && decks.Count > 0) actions.Add(() => onlineDeck = (onlineDeck + 1) % decks.Count);
            GUILayout.EndHorizontal();
            if (onlineDeck >= 0 && onlineDeck < decks.Count)
                foreach (var p in deckProblems[onlineDeck]) GUILayout.Label(p, ui.Error);
            var g = onlineGuest;
            if (g != null && !g.Closed && OnlineDeckReady && GUILayout.Button("Use this deck for the next game", GUILayout.Width(300)))
            {
                var d = decks[onlineDeck];
                actions.Add(() =>
                {
                    try
                    {
                        g.Send(NetMessageType.Deck, wr =>
                        {
                            wr.Write(d.Name);
                            wr.Write(d.Text ?? "");
                        });
                        onlineStatus = "Your deck for the next game: " + d.Name + ".";
                    }
                    catch (IOException)
                    {
                    }
                });
            }

            GUILayout.Space(10);
            GUILayout.Label("Host a game", ui.Title);
            var h = onlineHost;
            if (h == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Port", GUILayout.Width(110));
                onlinePortText = GUILayout.TextField(onlinePortText ?? "", 5, GUILayout.Width(80));
                GUI.enabled = onlineGuest == null && !onlineJoining;
                if (GUILayout.Button("Start hosting", GUILayout.Width(160))) actions.Add(StartHosting);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            else
            {
                var addresses = OnlinePlay.LocalAddresses();
                GUILayout.Label("Your friend types one of these (with the port):", ui.Label);
                foreach (var a in addresses) GUILayout.Label("   " + a + ":" + h.Port, ui.Good);
                if (addresses.Count == 0) GUILayout.Label("   (no network address found)", ui.Error);
                GUILayout.Label("Same home network: the address starting 192.168 or 10. works. Over the internet, either install a free virtual network "
                                + "(Tailscale, ZeroTier or Radmin VPN) on both computers and use the address it gives you (Tailscale's start with 100.), "
                                + "or open port " + h.Port + " (TCP) on your router to this computer and give your friend your public IP. Windows may ask "
                                + "to allow the game through its firewall the first time: allow it.", Hint(ui));
                if (h.Guest != null && !h.Guest.Closed)
                {
                    GUILayout.Label(h.GuestName + " has joined with " + (string.IsNullOrEmpty(h.GuestDeckName) ? "a deck" : h.GuestDeckName) + ".", ui.Good);
                    GUILayout.BeginHorizontal();
                    GUI.enabled = OnlineDeckReady;
                    if (GUILayout.Button("Start the game", GUILayout.Height(34), GUILayout.Width(200))) pendingOnlineStart = true;
                    GUI.enabled = true;
                    GUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Stop hosting", GUILayout.Width(160))) actions.Add(StopOnline);
            }

            GUILayout.Space(10);
            GUILayout.Label("Join a friend's game", ui.Title);
            if (g == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Address", GUILayout.Width(110));
                onlineAddress = GUILayout.TextField(onlineAddress ?? "", 80, GUILayout.Width(300));
                GUI.enabled = onlineHost == null && !onlineJoining && OnlineDeckReady && (onlineAddress ?? "").Trim().Length > 0;
                if (GUILayout.Button(onlineJoining ? "Connecting..." : "Join", GUILayout.Width(160))) actions.Add(JoinGame);
                GUI.enabled = true;
                GUILayout.EndHorizontal();
                GUILayout.Label("Type what your friend's screen shows, like 100.64.0.5:7777 (without a port, " + OnlinePlay.DefaultPort + " is used).", Hint(ui));
            }
            else
            {
                GUILayout.Label("Connected to " + onlineHostName + ". They start each game; you'll go to the board when they do.", ui.Good);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Ready for my next Regional game", GUILayout.Width(280))) actions.Add(SendOnlineReady);
                if (GUILayout.Button("Leave", GUILayout.Width(160))) actions.Add(StopOnline);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);
            if (!string.IsNullOrEmpty(onlineStatus)) GUILayout.Label(onlineStatus, ui.Label);
            if (!string.IsNullOrEmpty(onlineError)) GUILayout.Label(onlineError, ui.Error);
            GUILayout.Space(10);
            GUILayout.Label("You stay connected on the other screens (hosting, you can start a Regional Sim event your friend plays in). "
                            + "Stop hosting or Leave ends it.", Hint(ui));
            if (GUILayout.Button("Back to menu", GUILayout.Width(160)))
                actions.Add(() =>
                {
                    RefreshDecks();
                    mode = Mode.Menu;
                });
            GUILayout.EndScrollView();
            GUILayout.EndArea();
            foreach (var a in actions) a();
        }
    }
}
