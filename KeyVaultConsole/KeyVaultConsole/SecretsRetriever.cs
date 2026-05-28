using Azure.Security.KeyVault.Secrets;
using KeyVaultConsole.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KeyVaultConsole
{
    internal static class SecretsRetriever
    {
        internal static async Task Retrieve(SecretClient secretClient)
        {
            List<SecretResult> results = [];
            string[] secret_names = ["openai-api-key", "cosmosdb-connection-string"];
            foreach (string name in secret_names)
            {
                try
                {
                    var secret = (await secretClient.GetSecretAsync(name)).Value;
                    results.Add(new SecretResult
                    {
                        Name = name,
                        Value = secret.Value.Length > 20 ? secret.Value[..20] : secret.Value,
                        Version = secret.Properties.Version,
                        ContentType = secret.Properties.ContentType,
                        CreatedOn = secret.Properties.CreatedOn.HasValue ? secret.Properties.CreatedOn.Value.UtcDateTime.ToLongDateString() : "",
                        Tags = secret.Properties.Tags,
                        Status = "retrieved"
                    });
                }
                catch (Exception ex)
                {
                    results.Add(new SecretResult { Name = name, Status = ex.Message });
                }
            }

            int retrivedStatusCount = results.Count(r => r.Status == "retrieved");

            Console.WriteLine($"Retrieved {retrivedStatusCount} of {results.Count} secret(s).");
        }
    }
}
