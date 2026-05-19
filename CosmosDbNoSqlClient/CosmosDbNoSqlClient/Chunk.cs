namespace CosmosDbNoSqlClient;
internal class Chunk
{
    public string documentId { get; set; } = "";
    public string id { get; set; } = "";
    public string Content { get; set; } = "";
    public Metadata? Metadata { get; set; }
    public string uniqueId { get; set; } = "";
}