namespace EventGridConsole.Structures
{
    internal class ModerationEvent
    {
        public string type { get; set; } = "";
        public string source { get; set; } = "";
        public string subject { get; set; } = "";
        public ModerationEventData? data { get; set; }
    }
}
