using OpenTelemetryGeneratorConsole.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetryGeneratorConsole
{
    internal static class DependencyGenerator
    {
        private static readonly ActivitySource ActivitySource = new("OpenTelemetryGeneratorConsole");

        internal static async Task Generate()
        {
            List<DependencyGeneratorResult> results = [];
            KeyValuePair<string, string>[] targets = [
                    new ("blob-storage.blob.core.windows.net", "Azure blob"),
                    new ("cosmos-db.documents.azure.com", "Azure DocumentDB"),
                    new ("redis-cache.redis.cache.windows.net", "InProc")
                ];
            int[] slowIndexes = [2, 5, 8];
            
            for (int i = 0; i < 12; i++)
            {
                KeyValuePair<string, string> target = targets[i % targets.Length];
                string target_host = target.Key;
                string dep_type = target.Value;
                bool is_slow = slowIndexes.Contains(i);

                using var activity = ActivitySource.StartActivity($"call {target_host}", ActivityKind.Client);
                activity?.SetTag("peer.service", target_host);
                activity?.SetTag("db.system", dep_type);
                activity?.SetTag("cloud.role.name", "doc-processor");

                Random random = new();
                int duration;
                if (is_slow)
                {
                    duration = random.Next(1500, 3000);
                }
                else
                {
                    duration = random.Next(20, 200);
                }

                await Task.Delay(duration);
                activity?.SetStatus(ActivityStatusCode.Ok);

                results.Add
                (
                    new DependencyGeneratorResult 
                    {
                        TargetHost = target_host,
                        DepType = dep_type,
                        IsSlow = is_slow
                    }
                );


            }

            int slow_count = results.Count(r => r.IsSlow);

            Console.WriteLine($"Created {results.Count} dependency spans ({slow_count} with high latency)");
        }
    }
}
