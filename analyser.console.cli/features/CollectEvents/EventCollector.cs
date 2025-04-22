using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using analyser.console.cli.infrastructure.memoryStore;
using analyser.console.cli.models;

namespace analyser.console.cli.features.CollectEvents;

public class EventCollector
{
    private readonly EventLogQuery _eventLogQuery;
    
    public EventCollector(EventCollectorOptions eventCollectorOptions)
    {
        _eventLogQuery = new EventLogQuery(eventCollectorOptions.Path, eventCollectorOptions.PathType, eventCollectorOptions.Query);
    }
    
    public void CollectEvents()
    {
        var eventIds = new List<int>();
        using var reader = new EventLogReader(_eventLogQuery);
        EventRecord evt;
        while ((evt = reader.ReadEvent()) != null)
        {
            if (eventIds.Contains(evt.Id))
            {
                continue;
            }
            eventIds.Add(evt.Id);
            EventAnalyseQueue.Instance.AddEventQuery(new EventQuery(evt.Id, evt.FormatDescription()));
        }
        eventIds.Clear();
    }
}