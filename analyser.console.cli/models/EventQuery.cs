namespace analyser.console.cli.models
{
    internal class EventQuery
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public EventQuery(int id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
