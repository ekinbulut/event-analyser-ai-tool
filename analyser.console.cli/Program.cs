using System.Diagnostics.Eventing.Reader;
using System.Text;
using analyser.console.cli.features.CollectEvents;
using analyser.console.cli.features.PrepareEvents;
using analyser.console.cli.infrastructure.ai;
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
            var eventCollector = new EventCollector(new EventCollectorOptions());
            eventCollector.CollectEvents();


            var prepareEventService = new PrepareEventService();
            var json = prepareEventService.PrepareEventJson();
  
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

            var userPrompt = "You are a system administrator. You have to analyse the following events and give a summary of the events. The events are in JSON format. Please give me a summary of the events. And suggest possible fixes.";
            // append json to userPrompt
            var prompt = PromptBuilder.BuildPrompt(userPrompt, json);



            var aiEngine = new OllamaAIEngine("http://localhost:11434/", "gemma3:1b");

            var response = await aiEngine.ChatWithStreamingAsync(prompt, CancellationToken.None);

            foreach (var item in response)
            {
                if (item.Role == ChatRole.Assistant)
                {
                    Console.WriteLine(item.Text);
                }
            }

            while (true)
            {
                // Get user prompt and add to chat history
                Console.WriteLine("Your prompt:");
                response = await aiEngine.ChatAsync(Console.ReadLine(), CancellationToken.None);
                foreach (var item in response)
                {
                    if (item.Role == ChatRole.Assistant)
                    {
                        Console.WriteLine(item.Text);
                    }
                }
            }
        }
    }
}
