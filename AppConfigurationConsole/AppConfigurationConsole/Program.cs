using AppConfigurationConsole;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using dotenv.net;
using Microsoft.Extensions.Configuration;
using System;

Console.WriteLine("Hello, World!");

DotEnv.Load();
var envVars = DotEnv.Read();
string ENDPOINT = envVars["ENDPOINT"];

DefaultAzureCredentialOptions credentialOptions = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

var builder = new ConfigurationBuilder();
DefaultAzureCredential defaultAzureCredential = new(credentialOptions);
builder.AddAzureAppConfiguration(options =>
{
    options.Connect(new Uri(ENDPOINT), defaultAzureCredential).ConfigureKeyVault(kv => kv.SetCredential(defaultAzureCredential));
});

var config = builder.Build();

ConfigurationClient configurationClient = new(new Uri(ENDPOINT), defaultAzureCredential, new ConfigurationClientOptions());

string? input = "";
while (input != "4")
{
    Console.Clear();
    Console.WriteLine("=== Menu ===");
    Console.WriteLine("1. Load Settings");
    Console.WriteLine("2. List Setting Properties");
    Console.WriteLine("3. Refresh Settings");
    Console.WriteLine("4. Quit");
    Console.Write("Enter your choice: ");

    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            SettingsLoader.Load(config);
            break;
        case "2":
            await SettingPropertiesLister.ListSettingProperties(configurationClient);
            break;
        case "3":
            await ConfigurationRefresher.RefreshSettings(config, configurationClient);
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
