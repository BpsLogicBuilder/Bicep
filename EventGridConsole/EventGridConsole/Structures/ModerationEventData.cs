namespace EventGridConsole.Structures
{
    internal class ModerationEventData
    {
        public string contentId { get; set; } = "";
        public string contentType { get; set; } = "";
        public string modelName { get; set; } = "";
        public string modelVersion { get; set; } = "";
        public double confidence { get; set; }
        public string category { get; set; } = "";
        public string severity { get; set; } = "";
        public bool reviewRequired { get; set; }
        public string timestamp { get; set; } = "";
    }
}
