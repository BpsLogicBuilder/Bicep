using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using ServiceBusConsole.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusConsole
{
    internal static class TopicMessageHandler
    {
        const string TOPIC_NAME = "inference-results";
        internal static async Task SendAndReceiveMessages(ServiceBusClient client)
        {
            try
            {
                await DoSendAndReceiveMessages(client);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error with topic messaging: {ex}");
            }

        }
        internal static async Task DoSendAndReceiveMessages(ServiceBusClient client)
        {
            List<TopicMessageResult> sent = [];
            List<TopicMessageResult> notifications = [];
            List<TopicMessageResult> high_priority = [];
            string[] priorities = ["standard", "high", "standard", "high", "low"];

            ServiceBusSender sender = client.CreateSender(TOPIC_NAME);

            foreach (var (i, priority) in priorities.Index())
            {
                TopicMessageResult result = new()
                { 
                    document_id = $"doc-{i + 1:D3}",
                    status = "completed",
                    confidence = 0.95
                };

                ServiceBusMessage serviceBusMessage = new
                (
                    JsonSerializer.Serialize(result)
                )
                {
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString(),
                    CorrelationId = "req-doc-003"
                };
                serviceBusMessage.ApplicationProperties.Add("priority", priority);
                await sender.SendMessageAsync(serviceBusMessage);
                sent.Add(new()
                {
                    document_id = $"doc-{i + 1:D3}",
                    priority = priority
                });
            }

            ServiceBusReceiver nototicationReceiver = client.CreateReceiver(TOPIC_NAME, "notifications");
            var nototicationMessages = await nototicationReceiver.ReceiveMessagesAsync(20, TimeSpan.FromSeconds(5), CancellationToken.None);
            foreach (ServiceBusReceivedMessage message in nototicationMessages)
            {
                TopicMessageResult? body = JsonSerializer.Deserialize<TopicMessageResult>(message.Body.ToString());
                IReadOnlyDictionary<string, object> applicationProperties = message.ApplicationProperties;

                applicationProperties.TryGetValue("priority", out object? priorityVal);
                notifications.Add
                (
                    new TopicMessageResult
                    {
                        document_id = body?.document_id ?? "",
                        priority = priorityVal?.ToString() ?? "unknown",
                    }
                );
                await nototicationReceiver.CompleteMessageAsync(message, CancellationToken.None);
            }

            ServiceBusReceiver highPriorityReceiver = client.CreateReceiver(TOPIC_NAME, "high-priority");
            var highPriorityMessages = await highPriorityReceiver.ReceiveMessagesAsync(20, TimeSpan.FromSeconds(5), CancellationToken.None);
            foreach (ServiceBusReceivedMessage message in highPriorityMessages)
            {
                TopicMessageResult? body = JsonSerializer.Deserialize<TopicMessageResult>(message.Body.ToString());
                IReadOnlyDictionary<string, object> applicationProperties = message.ApplicationProperties;

                applicationProperties.TryGetValue("priority", out object? priorityVal);
                high_priority.Add
                (
                    new TopicMessageResult
                    {
                        document_id = body?.document_id ?? "",
                        priority = priorityVal?.ToString() ?? "unknown",
                    }
                );
                await highPriorityReceiver.CompleteMessageAsync(message, CancellationToken.None);
            }

            Console.WriteLine($"Sent {sent.Count} message(s). Notifications received {notifications.Count}. High-priority received {high_priority.Count}");
        }
    }
}
