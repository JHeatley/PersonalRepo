using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using PokemonTCG.AI;
using PokemonTCG.Core;

namespace PokemonTCG.Presentation
{
    /// <summary>One engine event plus the board as the viewer saw it right after that event.</summary>
    public sealed class HostMessage
    {
        public GameEvent Event;
        public PlayerView View;
        public string Error;
    }

    /// <summary>
    /// The human seat. The engine thread calls Decide() and blocks; the Unity UI shows Pending
    /// and calls Submit() when the player clicks. Cancel() unblocks the engine when the scene closes.
    /// </summary>
    public sealed class HumanAgent : IInteractiveAgent
    {
        private readonly object gate = new object();
        private DecisionRequest pending;
        private List<int> answer;
        private bool cancelled;

        public DecisionRequest Pending
        {
            get { lock (gate) return pending; }
        }

        public List<int> Decide(DecisionRequest request)
        {
            lock (gate)
            {
                pending = request;
                answer = null;
                while (answer == null && !cancelled) Monitor.Wait(gate);
                pending = null;
                if (cancelled) throw new OperationCanceledException("Game closed.");
                var result = answer;
                answer = null;
                return result;
            }
        }

        public void Submit(List<int> choice)
        {
            lock (gate)
            {
                if (pending == null) return;
                answer = choice;
                Monitor.PulseAll(gate);
            }
        }

        public void Cancel()
        {
            lock (gate)
            {
                cancelled = true;
                Monitor.PulseAll(gate);
            }
        }
    }

    /// <summary>
    /// Where the game screen gets its game from: this computer's engine (EngineHost), or a friend's over the network
    /// (RemoteGameHost, when you joined their online game). The screen only reads the events and views it hands out.
    /// </summary>
    public interface IGameHost
    {
        /// <summary>The engine, or null when the game runs on another computer.</summary>
        GameEngine Engine { get; }
        HumanAgent Human { get; }
        bool Finished { get; }
        int QueuedCount { get; }
        bool TryDequeue(out HostMessage message);
        void Stop();
        string PlayerName(int seat);
        /// <summary>"Your" for a player named "You", otherwise "Name's" (for the battle log).</summary>
        string Possessive(int seat);
    }

    /// <summary>
    /// Runs a GameEngine on a background thread so the rules (and later the AI search) never block rendering.
    /// The main thread drains the message queue at its own pace to animate the game.
    /// </summary>
    public sealed class EngineHost : IGameObserver, IGameHost
    {
        private readonly ConcurrentQueue<HostMessage> queue = new ConcurrentQueue<HostMessage>();
        private readonly int viewer;
        private Thread thread;
        private volatile bool finished;

        public GameEngine Engine { get; private set; }
        public HumanAgent Human { get; private set; }
        public bool Finished { get { return finished; } }
        public int QueuedCount { get { return queue.Count; } }

        public EngineHost(GameEngine engine, HumanAgent human, int viewerSeat)
        {
            Engine = engine;
            Human = human;
            viewer = viewerSeat;
            engine.AddObserver(this);
        }

        public void Start(IList<CardDef> deck0, IList<CardDef> deck1, int seed)
        {
            thread = new Thread(() =>
            {
                try
                {
                    Engine.Run(deck0, deck1, seed);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    queue.Enqueue(new HostMessage { Error = "Engine error (seed " + seed + "): " + e });
                }
                finally
                {
                    finished = true;
                }
            });
            thread.IsBackground = true;
            thread.Name = "Pokemon TCG engine";
            thread.Start();
        }

        public void OnEvent(GameEngine engine, GameEvent ev)
        {
            queue.Enqueue(new HostMessage { Event = ev, View = PlayerView.Create(engine, viewer) });
        }

        public bool TryDequeue(out HostMessage message)
        {
            return queue.TryDequeue(out message);
        }

        public void Stop()
        {
            if (Human != null) Human.Cancel();
            if (thread != null && thread.IsAlive) thread.Join(1000);
        }

        public string PlayerName(int seat)
        {
            return Engine.PlayerName(seat);
        }

        public string Possessive(int seat)
        {
            return Engine.Possessive(seat);
        }
    }

    /// <summary>
    /// The game screen's game when you joined a friend's online game: their copy runs it (OnlinePlay) and sends the events
    /// with the board as you see it, and your decisions; you answer through Human like in any game. Ends when the host says
    /// the game is over (Ended) or the connection drops (an error message with the reason).
    /// </summary>
    public sealed class RemoteGameHost : IGameHost
    {
        private readonly ConcurrentQueue<HostMessage> queue = new ConcurrentQueue<HostMessage>();
        private readonly NetConnection connection;
        private readonly NetCodec codec;
        private readonly string[] names;
        private Thread thread;
        private volatile bool finished, stopping;

        public GameEngine Engine { get { return null; } }
        public HumanAgent Human { get; private set; }
        public bool Finished { get { return finished; } }
        public int QueuedCount { get { return queue.Count; } }
        /// <summary>The host said the game is over (rather than the connection dropping).</summary>
        public bool Ended { get; private set; }

        public RemoteGameHost(NetConnection connection, NetCodec codec, HumanAgent human, string[] names)
        {
            this.connection = connection;
            this.codec = codec;
            Human = human;
            this.names = names;
        }

        public void Start()
        {
            thread = new Thread(Run) { IsBackground = true, Name = "Pokemon TCG online game" };
            thread.Start();
        }

        private void Run()
        {
            try
            {
                while (!stopping)
                {
                    var m = connection.Receive(500);
                    if (m == null)
                    {
                        if (connection.Closed)
                        {
                            queue.Enqueue(new HostMessage { Error = "The connection to the host was lost: " + connection.Reason });
                            return;
                        }
                        continue;
                    }
                    switch (m.Type)
                    {
                        case NetMessageType.Event:
                            var r = new BinaryReader(new MemoryStream(m.Body));
                            var ev = (GameEvent)codec.Read(r, typeof(GameEvent));
                            var view = (PlayerView)codec.Read(r, typeof(PlayerView));
                            queue.Enqueue(new HostMessage { Event = ev, View = view });
                            break;
                        case NetMessageType.Decide:
                            var request = codec.FromBytes<DecisionRequest>(m.Body);
                            var picks = Human.Decide(request); // waits for you
                            connection.Send(NetMessageType.Answer, codec.ToBytes(picks));
                            break;
                        case NetMessageType.End:
                            Ended = true;
                            return;
                        case NetMessageType.Bye:
                            string why = new BinaryReader(new MemoryStream(m.Body)).ReadString();
                            queue.Enqueue(new HostMessage { Error = "The host left: " + why });
                            return;
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception e)
            {
                queue.Enqueue(new HostMessage { Error = "The online game stopped: " + e.Message });
            }
            finally
            {
                finished = true;
            }
        }

        public bool TryDequeue(out HostMessage message)
        {
            return queue.TryDequeue(out message);
        }

        public void Stop()
        {
            stopping = true;
            if (Human != null) Human.Cancel();
            if (thread != null && thread.IsAlive) thread.Join(1000);
        }

        public string PlayerName(int seat)
        {
            return names != null && seat >= 0 && seat < names.Length ? names[seat] : "Nobody";
        }

        public string Possessive(int seat)
        {
            string name = PlayerName(seat);
            return name == "You" ? "Your" : name + "'s";
        }
    }
}
