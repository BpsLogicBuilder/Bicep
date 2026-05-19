namespace CosmosDbNoSqlClient;

internal class Metadata
{
    public string Source { get; set; } = "";
    public string Category { get; set; } = "";
    public string[] Tags { get; set; } = [];
    public int ChunkIndex { get; set; }
}