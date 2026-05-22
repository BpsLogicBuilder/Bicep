namespace EventGridConsole.Structures
{
    internal class PublishResult
    {
        public string content_id { get; set; } = "";
        public string event_type { get; set; } = "";
        public string category { get; set; } = "";
        public double confidence { get; set; }
        public string status { get; set; } = "";
    }
}
