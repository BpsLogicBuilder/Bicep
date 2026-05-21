namespace ServiceBusConsole.Structures
{
    internal class QueueMessageBody
    {
        public string prompt { get; set; } = "";
        public string model { get; set; } = "";
        public string document_id { get; set; } = "";
    }
}
