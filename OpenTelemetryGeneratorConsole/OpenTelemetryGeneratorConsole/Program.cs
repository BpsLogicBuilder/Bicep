using Azure.Monitor.OpenTelemetry.Exporter;
using dotenv.net;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetryGeneratorConsole;
using System;

Console.WriteLine("Hello, World!");
DotEnv.Load();
var envVars = DotEnv.Read();
string connectionString = envVars["APPLICATIONINSIGHTS_CONNECTION_STRING"];

const string serviceName = "OpenTelemetryGeneratorConsole";

var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: serviceName, serviceVersion: "1.0.0");

using var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(resourceBuilder)
            .AddSource(serviceName) // Must match the ActivitySource name
            .AddAzureMonitorTraceExporter(o => o.ConnectionString = connectionString)
            .AddConsoleExporter()
            .Build();

await RequestGenerator.Generate();
await DependencyGenerator.Generate();
await ExceptionsGenerator.Generate();
Console.WriteLine("Goodbye");

// Dispose tracer provider before the application ends.
// This will flush the remaining spans and shutdown the tracing pipeline.
tracerProvider.Dispose();
