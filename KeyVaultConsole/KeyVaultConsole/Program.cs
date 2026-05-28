using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using dotenv.net;
using KeyVaultConsole;
using System;

DotEnv.Load();
var envVars = DotEnv.Read();
string ENDPOINT = envVars["ENDPOINT"];

DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

SecretClient client = new(new Uri(ENDPOINT), new DefaultAzureCredential(options));

string? input = "";
while (input != "4")
{
    Console.Clear();
    Console.WriteLine("=== Menu ===");
    Console.WriteLine("1. Retrive Secrets");
    Console.WriteLine("2. Retrive Secret Properties");
    Console.WriteLine("3. Create New Secret Version");
    Console.WriteLine("4. Quit");
    Console.Write("Enter your choice: ");

    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await SecretsRetriever.Retrieve(client);
            break;
        case "2":
            await SecretPropertiesLister.ListProperties(client);
            break;
        case "3":
            await NewSecretVersionCreator.CreateNewVersion(client, "openai-api-key", Guid.NewGuid().ToString());
            break;
        case "4":
            Console.WriteLine("Exiting application...");
            break;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

    if (input != "4")
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
