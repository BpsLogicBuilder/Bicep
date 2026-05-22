namespace EventGridConsole.Structures
{
    internal class ReceivedResultDetail
    {
        public string content_id { get; set; } = "";
        public string category { get; set; } = "";
        public string severity { get; set; } = "";
        public double confidence { get; set; }
    }
}
