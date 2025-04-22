using System.Text;
using analyser.console.cli.infrastructure.memoryStore;

namespace analyser.console.cli.features.PrepareEvents;

public class PrepareEventService
{
    public string PrepareEventJson()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("{");
        sb.AppendLine("\"events\": [");
        while (EventAnalyseQueue.Instance.GetEventQueueCount() > 0)
        {
            var eventQuery = EventAnalyseQueue.Instance.GetEventQuery();
            sb.AppendLine("{");
            sb.AppendLine($"\"Id\": {eventQuery.Id},");
            sb.AppendLine($"\"Description\": \"{eventQuery.Description}\"");
            sb.AppendLine("},");

        }
        sb.Remove(sb.Length - 3, 1); // remove last comma
        sb.AppendLine("]");
        sb.Append("}");

        return sb.ToString();
    }
}