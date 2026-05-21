using Azure.Identity;
using Azure.Messaging.ServiceBus;
using ServiceBusConsole;
using System;

Console.WriteLine("Hello, World!");
string svcbusNameSpace = "bps-service-bus-uouxshlqhqmbs.servicebus.windows.net";
DefaultAzureCredentialOptions options = new()
{
    ExcludeEnvironmentCredential = true,
    ExcludeManagedIdentityCredential = true
};

ServiceBusClient client = new(svcbusNameSpace, new DefaultAzureCredential(options));

string? input = "";
while (input != "5")
{
    Console.Clear();
    Console.WriteLine("=== Menu ===");
    Console.WriteLine("1. Send Messages to Queue");
    Console.WriteLine("2. Process Queue Messages");
    Console.WriteLine("3. Inspect Dead Letter Queue");
    Console.WriteLine("4. Send and Receive Topic Messages");
    Console.WriteLine("5. Quit");
    Console.Write("Enter your choice: ");

    input = Console.ReadLine();

    switch (input)
    {
        case "1":
            await QueueMessageSender.SendMessages(client);
            break;
        case "2":
            await QueueMessageReceiver.ReceiveMessages(client);
            break;
        case "3":
            await DeadLetterQueueInspector.Inspect(client);
            break;
        case "4":
            await TopicMessageHandler.SendAndReceiveMessages(client);
            break;
        case "5":
            Console.WriteLine("Exiting application...");
            break;
        default:
            Console.WriteLine("Invalid choice. Please try again.");
            break;
    }

    if (input != "5")
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
