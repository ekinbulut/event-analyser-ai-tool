using System.Diagnostics.Eventing.Reader;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace analyser.console.cli
{
    [SimpleJob(RuntimeMoniker.Net80)]
    internal class Program
    {
        [Benchmark]
        static async Task Main(string[] args)
        {
            var eventIds = new List<int>();
            var query = new EventLogQuery("System", PathType.LogName, "*[System/Level=2]");
            using var reader = new EventLogReader(query);
            EventRecord evt;
            while ((evt = reader.ReadEvent()) != null) {
                if (eventIds.Contains(evt.Id)) 
                { 
                    continue; 
                }
                eventIds.Add(evt.Id);
                EventAnalyseQueue.Instance.AddEventQuery(new EventQuery(evt.Id, evt.FormatDescription()));

            }

            eventIds.Clear();

            Console.WriteLine(EventAnalyseQueue.Instance.GetEventQueueCount());


            // generate a Json format with EventQuery
            /*
             * {
             *   "events": [
             *      {
             *          "Id" : 1,
             *          "Description" : "Event 1"
             *      }]
             * }
             */
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

            var json = sb.ToString();

            var userPrompt = "You are a system administrator. You have to analyse the following events and give a summary of the events. The events are in JSON format. Please give me a summary of the events.";
            // append json to userPrompt
            userPrompt += json;



            var aiEngine = new AIEngine("http://localhost:11434/", "gemma3:1b");

            var response = await aiEngine.QueryAsync(userPrompt, CancellationToken.None);

            Console.WriteLine(response[1].Text);
        }
    }
}
