using Azure.Messaging;
using Azure.Messaging.EventGrid;
using Azure.Messaging.EventGrid.Namespaces;
using EventGridConsole.Structures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventGridConsole
{
    internal static class EventPublisher
    {
        internal static async Task PublishEvents(EventGridSenderClient client)
        {
            try
            {
                string jsonString = await File.ReadAllTextAsync("moderation_events.json");
                List<ModerationEvent> event_definitions = JsonSerializer.Deserialize<List<ModerationEvent>>(jsonString) ?? [];
                event_definitions.ForEach(e => e.data?.timestamp = DateTime.UtcNow.ToString("O"));
                CloudEvent[] cloudEvents =
                [
                    .. event_definitions.Select
                (
                    def => new CloudEvent(def.source, def.type, def.data, typeof(ModerationEventData))
                    {
                        Subject = def.subject,
                        Id = Guid.NewGuid().ToString()
                    }
                )
                ];

                await client.SendAsync(cloudEvents);

                PublishResult[] publishResults =
                [
                    .. cloudEvents.Select
                (
                    e =>
                    {
                        ModerationEventData? data = e.Data?.ToObjectFromJson<ModerationEventData>();
                        string[] typeNameParts = e.Type.Split('.', StringSplitOptions.RemoveEmptyEntries);
                        return new PublishResult
                        {
                            content_id = data?.contentId ?? "",
                            event_type = typeNameParts[^1],
                            category = data?.category ?? "",
                            confidence = data?.confidence ?? 0,
                            status = "published"
                        };
                    }
                )
                ];

                Console.WriteLine($"Successfully published {publishResults.Length} event(s) to the Event Grid namespace topic.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing events: {ex}");
            }
        }
    }
}
