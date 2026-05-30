using OpenTelemetryWebApplication.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OpenTelemetryWebApplication
{
    public class DocumentProcessor : OpenTelemetryWebApplication.IDocumentProcessor
    {
        private static readonly ActivitySource MyActivitySource = new("OpenTelemetryWebApplication");

        public async Task<List<ProcessingResults>> ProcessorDocuments(int documentCount)
        {
            using var activity = MyActivitySource.StartActivity(nameof(ProcessorDocuments));
            activity?.SetTag("document.count", documentCount);
            activity?.SetTag("pipeline.name", "document-processing");

            List<ProcessingResults> results = [];
            for (int i = 1; i <= documentCount; i++)
            {
                string doc_id = $"DOC-{i:D4}";
                var validateResult = await ValidateDocument(doc_id);
                var enrichResult = await EnrichDocument(doc_id);
                var storeResult = await StoreDocument(doc_id);

                results.Add(new ProcessingResults { DocId = doc_id, Enrich = enrichResult, Store = storeResult, Validate = validateResult });
            }

            // Set status (optional)
            activity?.SetStatus(ActivityStatusCode.Ok);

            return results;
        }

        private static async Task<Dictionary<string, object>> ValidateDocument(string docId)
        {
            using var activity = MyActivitySource.StartActivity(nameof(ValidateDocument));
            activity?.SetTag("document.id", docId);
            activity?.SetTag("document.stage", "validate");

            Random random = new();
            int min = 50;
            int max = 150;
            int result = random.Next(min, max);

            await Task.Delay(result);

            activity?.SetTag("document.valid", true);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return new Dictionary<string, object> { ["status"] = "valid", ["duration_ms"] = result };
        }

        private static async Task<Dictionary<string, object>> EnrichDocument(string doc_id)
        {
            using var activity = MyActivitySource.StartActivity(nameof(EnrichDocument));

            activity?.SetTag("document.id", doc_id);
            activity?.SetTag("document.stage", "enrich");
            HashSet<string> docs = ["DOC-0003", "DOC-0005"];
            Random random = new();
            int min;
            int max;
            int result;
            if (docs.Contains(doc_id))
            {
                min = 1500;
                max = 3000;
                result = random.Next(min, max);
                activity?.SetTag("enrichment.slow", true);
            }
            else
            {
                min = 50;
                max = 200;
                result = random.Next(min, max);
                activity?.SetTag("enrichment.slow", false);
            }

            await Task.Delay(result);
            activity?.SetTag("enrichment.duration_ms", result);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return new Dictionary<string, object> { ["status"] = "enriched", ["duration_ms"] = result, ["slow"] = docs.Contains(doc_id) };
        }

        private static async Task<Dictionary<string, object>> StoreDocument(string doc_id)
        {
            using var activity = MyActivitySource.StartActivity(nameof(StoreDocument));
            activity?.SetTag("document.id", doc_id);
            activity?.SetTag("document.stage", "store");
            activity?.SetTag("storage.type", "blob");

            Random random = new();
            int min = 50;
            int max = 200;
            int result = random.Next(min, max);

            await Task.Delay(result);

            return new Dictionary<string, object> { ["status"] = "stored", ["duration_ms"] = result };
        }
    }
}
