using System.Collections.Generic;
using PokemonTCG.Core;

namespace PokemonTCG.AI
{
    /// <summary>One decision a player made: what they were asked, what they saw, what they picked.</summary>
    public sealed class RecordedDecision
    {
        public DecisionRequest Request;
        public List<int> Answer;
        /// <summary>The whole game right before a main-action choice (null for other decisions). Used to replay "what if".</summary>
        public GameState Snapshot;
        public int Turn;
        /// <summary>Position in the player's list of decisions.</summary>
        public int Index;
    }

    /// <summary>
    /// Every decision one seat makes in a game, with a snapshot of the game at each main action. Feeds the learning
    /// AI (it studies your plays) and the Areas of Improvement review.
    /// </summary>
    public sealed class DecisionLog
    {
        private readonly List<RecordedDecision> decisions = new List<RecordedDecision>();
        /// <summary>Set after the engine is created; needed for the snapshots.</summary>
        public GameEngine Engine;

        public List<int> Record(IPlayerAgent inner, DecisionRequest r)
        {
            GameState snapshot = null;
            if (r.Kind == DecisionKind.MainAction && Engine != null && Engine.State != null) snapshot = Engine.State.Clone();
            var answer = inner.Decide(r);
            if (answer != null)
            {
                lock (decisions)
                {
                    decisions.Add(new RecordedDecision
                    {
                        Request = r,
                        Answer = new List<int>(answer),
                        Snapshot = snapshot,
                        Turn = r.View != null ? r.View.TurnNumber : 0,
                        Index = decisions.Count
                    });
                }
            }
            return answer;
        }

        /// <summary>A copy of the decisions so far (safe to use from another thread).</summary>
        public List<RecordedDecision> Decisions()
        {
            lock (decisions) return new List<RecordedDecision>(decisions);
        }

        public int Count
        {
            get { lock (decisions) return decisions.Count; }
        }
    }

    /// <summary>Records a bot's decisions (tests, reviews of bot games).</summary>
    public sealed class RecordingAgent : IPlayerAgent
    {
        private readonly IPlayerAgent inner;
        public readonly DecisionLog Log;

        public RecordingAgent(IPlayerAgent inner, DecisionLog log)
        {
            this.inner = inner;
            Log = log;
        }

        public IPlayerAgent Inner { get { return inner; } }

        public List<int> Decide(DecisionRequest request)
        {
            return Log.Record(inner, request);
        }
    }

    /// <summary>Records the decisions of a person playing on screen (keeps the seat interactive for the engine).</summary>
    public sealed class InteractiveRecordingAgent : IInteractiveAgent
    {
        private readonly IPlayerAgent inner;
        public readonly DecisionLog Log;

        public InteractiveRecordingAgent(IInteractiveAgent inner, DecisionLog log)
        {
            this.inner = inner;
            Log = log;
        }

        public List<int> Decide(DecisionRequest request)
        {
            return Log.Record(inner, request);
        }
    }

    /// <summary>Keeps every game event with the turn it happened on (for the post-game review).</summary>
    public sealed class EventCollector : IGameObserver
    {
        public sealed class Entry
        {
            public int Turn;
            public GameEvent Event;
        }

        private readonly List<Entry> entries = new List<Entry>();

        public void OnEvent(GameEngine engine, GameEvent ev)
        {
            lock (entries) entries.Add(new Entry { Turn = engine.State != null ? engine.State.TurnNumber : 0, Event = ev });
        }

        public List<Entry> Entries()
        {
            lock (entries) return new List<Entry>(entries);
        }
    }
}
