using Azure.Identity;
using CosmosDbNoSqlClient;
using dotenv.net;
using Microsoft.Azure.Cosmos;

Console.WriteLine("Hello, World!");

string databaseName = "ragstore";
string containerName = "chunks";

DotEnv.Load();
var envVars = DotEnv.Read();
string cosmosDbAccountUrl = envVars["DOCUMENT_ENDPOINT"];

DefaultAzureCredential defaultAzureCredential = new();
CosmosClient client = new(
    accountEndpoint: cosmosDbAccountUrl,
    tokenCredential: defaultAzureCredential
);

var database = client.GetDatabase(databaseName);
var container = database.GetContainer(containerName);

Chunk[] chunks = 
[
    new Chunk 
    {
        documentId = "test-doc-001",
        id =  "test-doc-001-chunk-0",
        Content = "Azure Cosmos DB is a fully managed NoSQL database service.",
        uniqueId = "{D8A05CAB-6FA7-45D1-9A2E-CE0ACBC5BAFE}",
        Metadata = new Metadata {
            Source = "azure-docs",
            Category =  "databases",
            Tags = ["nosql", "cosmosdb"],
            ChunkIndex =  0
        }
    },
    new Chunk 
    {
        documentId =  "test-doc-001",
        id =  "test-doc-001-chunk-1",
        Content = "Cosmos DB offers multiple APIs including NoSQL, MongoDB, and Cassandra.",
        uniqueId = "{F9D70CF1-0775-429F-813D-3CFAE7C6768E}",
        Metadata = new Metadata
        {
            Source = "azure-docs",
            Category =  "databases",
            Tags = ["nosql", "api"],
            ChunkIndex =  1
        }
    },
    new Chunk 
    {
        documentId =  "test-doc-002",
        id =  "test-doc-002-chunk-0",
        Content = "Azure Functions is a serverless compute service.",
        uniqueId = "{A08F687A-2891-4B26-9A48-EE08AD05F16B}",
        Metadata = new Metadata
        {
            Source = "azure-docs",
            Category =  "compute",
            Tags = ["serverless", "functions"],
            ChunkIndex =  0
        }
    }
];

foreach (var chunk in chunks)
{
    await container.UpsertItemAsync(chunk, partitionKey: new PartitionKey(chunk.documentId));
    Console.WriteLine($"Updated {chunk.Content}");
}