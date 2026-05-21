using Azure.Messaging.ServiceBus;
using ServiceBusConsole.Structures;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusConsole
{
    internal static class DeadLetterQueueInspector
    {
        const string QUEUE_NAME = "inference-requests";
        internal static async Task Inspect(ServiceBusClient client)
        {
            ServiceBusReceiver receiver = client.CreateReceiver(QUEUE_NAME, new ServiceBusReceiverOptions() { SubQueue = SubQueue.DeadLetter });
            List<DeadLetterMessageResult> results = [];
            var messages = await receiver.ReceiveMessagesAsync(20, TimeSpan.FromSeconds(5), CancellationToken.None);
            foreach (ServiceBusReceivedMessage message in messages)
            {
                results.Add
                (
                    new DeadLetterMessageResult
                    {
                        message_id = message.MessageId,
                        correlation_id = message.CorrelationId,
                        dead_letter_reason = message.DeadLetterReason,
                        error_description = message.DeadLetterErrorDescription,
                        delivery_count = message.DeliveryCount,
                        body = message.Body.ToString()
                    }
                );

                await receiver.CompleteMessageAsync(message);
            }

            if (results.Count == 0)
            {
                Console.WriteLine($"No messages in the dead-letter queue.");
            }
            else
            {
                Console.WriteLine($"Found {results.Count} message(s) in the dead-letter queue.");
            }
        }
    }
}
