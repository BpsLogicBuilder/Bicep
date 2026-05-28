using Azure.Security.KeyVault.Secrets;
using System;
using System.Threading.Tasks;

namespace KeyVaultConsole
{
    internal static class NewSecretVersionCreator
    {
        internal static async Task CreateNewVersion(SecretClient secretClient, string secretName, string newValue)
        {
			try
			{
				var secret = (await secretClient.GetSecretAsync(secretName)).Value;
                Console.WriteLine($"Old value was: {secret.Value}");
            }
			catch (Exception ex)
			{
                Console.WriteLine($"Error getting existing secret: {ex}");
            }

            var newSecret = new KeyVaultSecret(secretName, newValue);
            newSecret.Properties.ContentType = "text/plain";
            newSecret.Properties.Tags.Add("environment", "development");
            newSecret.Properties.Tags.Add("rotated", "true");
            await secretClient.SetSecretAsync(newSecret);

            Console.WriteLine($"Successfully created new version of the secret.");
        }
    }
}
