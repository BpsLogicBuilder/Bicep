using Azure.Identity;
using Azure.Messaging.EventGrid.Namespaces;
using dotenv.net;
using EventGridConsole;
using System;

DotEnv.Load();
var envVars = DotEnv.Read();
string ENDPOINT = envVars["ENDPOINT"];
const string TOPIC = "moderation-events";

DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};
EventGridSenderClient senderClient = new
(
    new Uri(ENDPOINT),
    TOPIC,
    new DefaultAzureCredential(options)
);

string? input = "";
while (input != "4")
{
    Console.Clear();
    Console.WriteLine("=== Menu ===");
    Console.WriteLine("1. Publish Moderation Events");
    Console.WriteLine("2. Receive & Acknowledge Events");
    Console.WriteLine("3. Inspect & Reject Event");
    Console.WriteLine("4. Quit");
    Console.Write("Enter your choice: ");

    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await EventPublisher.PublishEvents(senderClient);
            break;
        case "2":
            await EventReceiver.ReceiveEvents();
            break;
        case "3":
            await FlaggedEventHandler.InspectEvents(senderClient);
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
