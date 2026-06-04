using OpenTelemetryGeneratorConsole.Structures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace OpenTelemetryGeneratorConsole
{
    internal static class ExceptionsGenerator
    {
        private static readonly ActivitySource ActivitySource = new("OpenTelemetryGeneratorConsole");
        internal static async Task Generate()
        {
            List<ExceptionsGeneratorResult> results = [];
            List<ExceptionScenario> exception_scenarios = [
                    new ("doc-processor", "/api/process", "ValueError", "Invalid document format: missing required field 'title'"),
                    new ("api-gateway", "/api/documents", "TimeoutError", "Upstream service did not respond within 30s"),
                    new ("auth-service", "/api/auth/token", "PermissionError", "Token refresh failed: invalid grant"),
                    new ("doc-processor", "/api/documents/{id}", "FileNotFoundError", "Document DOC-9999 not found in storage"),
                    new ("api-gateway", "/api/process", "ConnectionError", "Failed to connect to downstream service")
                ];

            foreach (var scenario in exception_scenarios)
            {
                string service = scenario.Service, path = scenario.Path, exc_type = scenario.ExceptionType, exc_message = scenario.ExceptionMessage;
                using var activity = ActivitySource.StartActivity($"POST {path}", ActivityKind.Server);

                activity?.SetTag("http.method", "POST");
                activity?.SetTag("http.url", $"https://{service}.example.com{path}");
                activity?.SetTag("http.status_code", 500);
                activity?.SetTag("cloud.role.name", service);

                Random random = new();
                int duration = random.Next(50, 200);
                await Task.Delay(duration);

                Exception exception = GetException(exc_type, exc_message);
                activity?.AddException(exception);
                activity?.SetStatus(ActivityStatusCode.Error, exc_message);

                results.Add
                (
                    new ExceptionsGeneratorResult { Service = service, ExceptionType = exc_type, Message = exc_message }
                );
            }

            Console.WriteLine($"Created {results.Count} exception spans.");
        }

        static Exception GetException(string exceptionType, string message)
        {
            return exceptionType switch
            {
                "ValueError" => new InvalidDataException(message),
                "TimeoutError" => new TimeoutException(message),
                "PermissionError" => new SecurityException(message),
                "ConnectionError" => new HttpRequestException(),
                "FileNotFoundError" => new FileNotFoundException(),
                _ => new Exception(message),
            };
        }
    }
}
