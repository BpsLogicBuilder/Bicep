using Azure.Messaging.ServiceBus;
using ServiceBusConsole.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusConsole
{
    internal static class QueueMessageReceiver
    {
        const string QUEUE_NAME = "inference-requests";
        internal static async Task ReceiveMessages(ServiceBusClient client)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(QUEUE_NAME, new ServiceBusReceiverOptions());
            List<QueueMessageResult> results = [];
            var messages = await receiver.ReceiveMessagesAsync(20, TimeSpan.FromSeconds(5), CancellationToken.None);
            foreach (ServiceBusReceivedMessage message in messages)
            {
                try
                {
                    QueueMessageBody? body = JsonSerializer.Deserialize<QueueMessageBody>(message.Body.ToString());
                    if (body != null)
                    {
                        results.Add
                        (
                            new QueueMessageResult
                            {
                                correlation_id = message.CorrelationId,
                                document_id = body.document_id,
                                model = body.model,
                                prompt = body.prompt,
                                status = "completed"
                            }
                        );
                    }
                    await receiver.CompleteMessageAsync(message);
                }
                catch (JsonException)
                {
                    await receiver.DeadLetterMessageAsync(message, "MalformedPayload", "Message body is not valid JSON", CancellationToken.None);
                    results.Add
                    (
                        new QueueMessageResult
                        {
                            correlation_id = message.CorrelationId,
                            document_id = "",
                            model = "",
                            prompt = message.Body.ToString(),//[..50],
                            status = "dead-lettered"
                        }
                    );
                }
            }

            int completed = results.Count(r => r.status == "completed");
            int deadLettered = results.Count(r => r.status == "dead-lettered");

            if (completed + deadLettered == 0)
            {
                Console.WriteLine($"No messages to process. Send messages first.");
            }
            else
            {
                Console.WriteLine($"Processed {results.Count} message(s): {completed} completed, {deadLettered} dead-lettered.");
            }
        }
    }
}
