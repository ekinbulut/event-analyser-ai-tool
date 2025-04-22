using System.Diagnostics.Eventing.Reader;

namespace analyser.console.cli.features.CollectEvents;

public class EventCollectorOptions
{
    public string Path { get; set; } = "System";
    public PathType PathType { get; set; } = PathType.LogName;
    public string Query { get; set; } = "*[System/Level=2]";
}