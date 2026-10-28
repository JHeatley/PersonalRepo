using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
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
    /// Runs a GameEngine on a background thread so the rules (and later the AI search) never block rendering.
    /// The main thread drains the message queue at its own pace to animate the game.
    /// </summary>
    public sealed class EngineHost : IGameObserver
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
    }
}
