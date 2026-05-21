using Azure.Messaging.ServiceBus;
using ServiceBusConsole.Structures;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceBusConsole
{
    internal static class QueueMessageSender
    {
        const string QUEUE_NAME = "inference-requests";
        internal static async Task SendMessages(ServiceBusClient client)
        {
            List<QueueMessageResult> results = new();
            ServiceBusSender sender = client.CreateSender(QUEUE_NAME);

            //Valid message 1
            ServiceBusMessage serviceBusMessage = new
            (
                JsonSerializer.Serialize
                (
                    new QueueMessageBody 
                    {
                        prompt = "Extract parties and effective date.",
                        model = "gpt-4o",
                        document_id = "doc-001"
                    }
                )
            )
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = "req-doc-003"
            };
            serviceBusMessage.ApplicationProperties.Add("priority", "standard");
            serviceBusMessage.ApplicationProperties.Add("document_type", "contract");
            await sender.SendMessageAsync(serviceBusMessage);
            results.Add(new QueueMessageResult { correlation_id = serviceBusMessage.CorrelationId, type = "valid", status = "sent" });

            //Valid message 2
            serviceBusMessage = new
            (
                JsonSerializer.Serialize
                (
                    new QueueMessageBody
                    {
                        prompt = "Summarize the key terms.",
                        model = "gpt-4o",
                        document_id = "doc-002"
                    }
                )
            )
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = "req-doc-003"
            };
            serviceBusMessage.ApplicationProperties.Add("priority", "high");
            serviceBusMessage.ApplicationProperties.Add("document_type", "contract");
            await sender.SendMessageAsync(serviceBusMessage);
            results.Add(new QueueMessageResult { correlation_id = serviceBusMessage.CorrelationId, type = "valid", status = "sent" });

            //Invalid message (malformed body)
            serviceBusMessage = new
            (
                "not valid json: [broken"
            )
            {
                ContentType = "application/json",
                MessageId = Guid.NewGuid().ToString(),
                CorrelationId = "req-doc-003"
            };
            serviceBusMessage.ApplicationProperties.Add("priority", "standard");
            await sender.SendMessageAsync(serviceBusMessage);
            results.Add(new QueueMessageResult { correlation_id = serviceBusMessage.CorrelationId, type = "malformed", status = "sent" });

            Console.WriteLine($"Successfully sent {results.Count} message(s) to the queue.");
        }
    }
}
