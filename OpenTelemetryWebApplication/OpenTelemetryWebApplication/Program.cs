using Azure.Monitor.OpenTelemetry.AspNetCore;
using dotenv.net;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using OpenTelemetryWebApplication;

DotEnv.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry().UseAzureMonitor().WithTracing(tracing => tracing
        .AddSource("OpenTelemetryWebApplication")
        .AddAspNetCoreInstrumentation()); // Automatic spans for incoming requests

builder.Services.AddTransient<IDocumentProcessor, DocumentProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API v1");
    });
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
