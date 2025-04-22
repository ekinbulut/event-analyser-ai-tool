namespace analyser.console.cli.features.PrepareEvents;

public class PromptBuilder
{
    public static string BuildPrompt(string instructions, string events)
    {
        return $"{instructions}\n\n{events}";
    }
}