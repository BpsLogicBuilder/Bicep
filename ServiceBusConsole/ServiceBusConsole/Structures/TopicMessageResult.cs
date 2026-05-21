namespace ServiceBusConsole.Structures
{
    internal class TopicMessageResult
    {
        public string status { get; set; } = "";
        public double confidence { get; set; }
        public string priority { get; set; } = "";
        public string document_id { get; set; } = "";
    }
}
