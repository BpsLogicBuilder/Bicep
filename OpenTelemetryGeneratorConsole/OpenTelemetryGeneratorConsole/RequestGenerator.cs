using OpenTelemetryGeneratorConsole.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTelemetryGeneratorConsole
{
    internal static class RequestGenerator
    {
        private static readonly ActivitySource ActivitySource = new("OpenTelemetryGeneratorConsole");
        internal static async Task Generate()
        {
            List<RequestGeneratorResult> results = [];
            string[] services = ["api-gateway", "doc-processor", "auth-service"];
            KeyValuePair<string, string>[] endpoints = [
                    new ("/api/documents", "POST"),
                    new ("/api/documents/{id}", "GET"),
                    new ("/api/status", "GET"),
                    new ("/api/process", "POST"),
                    new ("/api/auth/token", "POST"),
                ];

            for (int i=0; i<15; i++)
            {
                string service = services[i % services.Length];
                KeyValuePair<string, string> endpoint = endpoints[i % endpoints.Length];
                HashSet<int> failures = [3, 7, 11, 13];
                bool shouldFail = failures.Contains(i);
                string status_code = shouldFail && (i == 3 || i == 11) ? "500" : "429";
                string path = endpoint.Key;
                string method = endpoint.Value;
                using var activity = ActivitySource.StartActivity($"{method} {path}", ActivityKind.Server);
                activity?.SetTag("http.method", method);
                activity?.SetTag("http.url", $"https://{service}.example.com{path}");
                activity?.SetTag("http.status_code", int.Parse(status_code));
                activity?.SetTag("http.route", path);
                activity?.SetTag("cloud.role.name", service);

                Random random = new();
                int duration = random.Next(1500, 3000);
                await Task.Delay(duration);

                if (shouldFail)
                {
                    activity?.SetStatus(ActivityStatusCode.Ok, $"HTTP {status_code}");
                }
                else
                {
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }

                results.Add
                (
                    new RequestGeneratorResult 
                    { 
                        Service = service, 
                        Endpoint = $"{method} {path}",
                        Status = status_code,
                        Failed = shouldFail
                    }
                );
            }

            int successCount = results.Count(r => !r.Failed);
            int failCount = results.Count(r => r.Failed);
            Console.WriteLine($"Created {results.Count} request spans ({successCount} succeeded, {failCount} failed)");
        }
    }
}
