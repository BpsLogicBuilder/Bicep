using Azure;
using Azure.Messaging;
using Azure.Messaging.EventGrid.Namespaces;
using EventGridConsole.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGridConsole
{
    internal static class FlaggedEventHandler
    {
        const string TOPIC = "moderation-events";
        const string SUB_FLAGGED = "sub-flagged";

        internal static async Task InspectEvents(EventGridSenderClient senderClient)
        {
            try
            {
                ModerationEventData data = new()
                {
                    contentId = "test-inspect",
                    contentType = "text",
                    modelName = "text-moderator-v2",
                    modelVersion = "2.4.0",
                    confidence = 0.76,
                    category = "misinformation",
                    severity = "medium",
                    reviewRequired = true,
                    timestamp = DateTime.UtcNow.ToString("O")
                };

                CloudEvent cloudEvent = new("/services/content-moderation", "com.contoso.ai.ContentFlagged", data, typeof(ModerationEventData))
                {
                    Subject = "/content/text/test-inspect",
                    Id = Guid.NewGuid().ToString()
                };

                await senderClient.SendAsync(cloudEvent);

                EventGridReceiverClient receiverClient = EventGridReceiverHelper.GetClient(TOPIC, SUB_FLAGGED);
                Response<ReceiveResult> receivedResult = await receiverClient.ReceiveAsync(1, TimeSpan.FromSeconds(10));

                ReceiveDetails receiveDetails = receivedResult.Value.Details[0];
                CloudEvent receivedEvent = receiveDetails.Event;
                string lockToken = receiveDetails.BrokerProperties.LockToken;
                int deliveryCount = receiveDetails.BrokerProperties.DeliveryCount;

                Dictionary<string, object> result = new()
                {
                    ["specversion"] = "1.0",
                    ["type"] = receivedEvent.Type,
                    ["source"] = receivedEvent.Source,
                    ["subject"] = receivedEvent.Subject ?? "",
                    ["id"] = receivedEvent.Id,
                    ["time"] = receivedEvent.Time.HasValue ? receivedEvent.Time.Value.UtcDateTime.ToLongDateString() : "",
                    ["data"] = receivedEvent.Data!,
                    ["delivery_count"] = deliveryCount,
                    ["action"] = "rejected"
                };

                await receiverClient.RejectAsync([lockToken]);

                if ((int)result["delivery_count"] > 0)
                    Console.WriteLine($"Published a test event, received it, inspected the envelope, and rejected it.");
                else
                    Console.WriteLine($"No events received from the subscription. Check that the namespace is deployed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inspecting event: {ex}");
            }
        }
    }
}
