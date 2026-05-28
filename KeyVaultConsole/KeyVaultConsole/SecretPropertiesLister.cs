using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyVaultConsole
{
    internal static class SecretPropertiesLister
    {
        internal static async Task ListProperties(SecretClient secretClient)
        {
            List<SecretProperties> properties = [];
            try
            {
                await foreach (SecretProperties secretProperties in secretClient.GetPropertiesOfSecretsAsync())
                {
                    properties.Add(secretProperties);
                }
                Console.WriteLine($"Found {properties.Count} secret(s) in the vault.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listing secrets: {ex}");
            }
        }
    }
}
