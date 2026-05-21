namespace ServiceBusConsole.Structures
{
    internal class DeadLetterMessageResult
    {
        public string body { get; set; } = "";
        public string correlation_id { get; set; } = "";
        public string dead_letter_reason { get; set; } = "";
        public string error_description { get; set; } = "";
        public int delivery_count { get; set; }
        public string message_id { get; set; } = "";
    }
}
