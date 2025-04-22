using System.Collections.Concurrent;
using analyser.console.cli.models;

namespace analyser.console.cli.infrastructure.memoryStore
{
    internal sealed class EventAnalyseQueue
    {
        private static EventAnalyseQueue? _instance;
        private ConcurrentQueue<EventQuery> _eventQueue = new();


        public static EventAnalyseQueue Instance {

            get
            {
                if (_instance == null)
                {
                    _instance = new EventAnalyseQueue();
                }
                return _instance;
            }

        }

        public void AddEventQuery(EventQuery eventQuery)
        {
            _eventQueue.Enqueue(eventQuery);
        }

        public EventQuery? GetEventQuery()
        {
            if (_eventQueue.TryDequeue(out var eventQuery))
            {
                return eventQuery;
            }
            return null;
        }

        public int GetEventQueueCount()
        {
            return _eventQueue.Count;
        }

        public void ClearEventQueue()
        {
            while (_eventQueue.TryDequeue(out _)) { }
        }
    }
}
