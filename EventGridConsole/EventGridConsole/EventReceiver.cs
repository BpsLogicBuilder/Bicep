using Azure;
using Azure.Messaging.EventGrid.Namespaces;
using EventGridConsole.Structures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EventGridConsole
{
    internal static class EventReceiver
    {
        const string TOPIC = "moderation-events";
        const string SUB_FLAGGED = "sub-flagged";
        const string SUB_APPROVED = "sub-approved";
        const string SUB_ALL = "sub-all-events";
        internal static async Task ReceiveEvents()
        {
            try
            {
                List<ReceivedResultDetail> flagged = [];
                List<ReceivedResultDetail> approved = [];
                List<ReceivedResultDetail> all_events = [];

                EventGridReceiverClient client = EventGridReceiverHelper.GetClient(TOPIC, SUB_FLAGGED);
                Response<ReceiveResult> receivedResult = await client.ReceiveAsync(10, TimeSpan.FromSeconds(10));
                List<string> tokens = [];
                foreach (ReceiveDetails receiveDetails in receivedResult.Value.Details)
                {
                    ModerationEventData? data = receiveDetails.Event.Data?.ToObjectFromJson<ModerationEventData>();
                    flagged.Add
                    (
                        new ReceivedResultDetail
                        {
                            content_id = data?.contentId ?? "",
                            category = data?.category ?? "",
                            severity = data?.severity ?? "",
                            confidence = data?.confidence ?? 0,
                        }
                    );
                    tokens.Add(receiveDetails.BrokerProperties.LockToken);
                }

                if (tokens.Count > 0)
                    await client.AcknowledgeAsync(tokens);

                client = EventGridReceiverHelper.GetClient(TOPIC, SUB_APPROVED);
                receivedResult = await client.ReceiveAsync(10, TimeSpan.FromSeconds(10));
                tokens = [];
                foreach (ReceiveDetails receiveDetails in receivedResult.Value.Details)
                {
                    ModerationEventData? data = receiveDetails.Event.Data?.ToObjectFromJson<ModerationEventData>();
                    approved.Add
                    (
                        new ReceivedResultDetail
                        {
                            content_id = data?.contentId ?? "",
                            category = data?.category ?? "",
                            severity = data?.severity ?? "",
                            confidence = data?.confidence ?? 0,
                        }
                    );
                    tokens.Add(receiveDetails.BrokerProperties.LockToken);
                }

                if (tokens.Count > 0)
                    await client.AcknowledgeAsync(tokens);

                client = EventGridReceiverHelper.GetClient(TOPIC, SUB_ALL);
                receivedResult = await client.ReceiveAsync(10, TimeSpan.FromSeconds(10));
                tokens = [];
                foreach (ReceiveDetails receiveDetails in receivedResult.Value.Details)
                {
                    ModerationEventData? data = receiveDetails.Event.Data?.ToObjectFromJson<ModerationEventData>();
                    all_events.Add
                    (
                        new ReceivedResultDetail
                        {
                            content_id = data?.contentId ?? "",
                            category = data?.category ?? "",
                            severity = data?.severity ?? "",
                            confidence = data?.confidence ?? 0,
                        }
                    );
                    tokens.Add(receiveDetails.BrokerProperties.LockToken);
                }

                if (tokens.Count > 0)
                    await client.AcknowledgeAsync(tokens);

                int total = flagged.Count + approved.Count + all_events.Count;
                if (total > 0)
                {
                    Console.WriteLine($"Received and acknowledged — Flagged: {flagged.Count}, Approved: {approved.Count}, All events: {all_events.Count}.");
                }
                else
                {
                    Console.WriteLine($"No events available in subscriptions. Publish events first.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing events: {ex}");
            }
        }
    }
}
