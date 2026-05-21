namespace ServiceBusConsole.Structures
{
    internal class QueueMessageResult
    {
        public string correlation_id { get; set; } = "";
        public string type { get; set; } = "";
        public string status { get; set; } = "";
        public string model { get; set; } = "";
        public string prompt { get; set; } = "";
        public string document_id { get; set; } = "";
    }
}
