using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace LogicBuilder.Function;

public class Greeter
{
    private ILogger<Greeter> _logger;

    public Greeter(ILogger<Greeter> logger)
    {
        _logger = logger;
    }

    [Function(nameof(Greeter))]
    public string Hello(
        [McpToolTrigger(nameof(Greeter), "Responds to the user with a hello message.")] ToolInvocationContext context,
        [McpToolProperty(nameof(name), "The name of the person to greet.")] string? name
    )
    {
        _logger.LogInformation("C# MCP tool trigger function processed a request.");
        return $"Hello, {name ?? "world"}! From the MCP Tool!";
    }

    [Function("SayFarewell")]
    public string Farewell(
        [McpToolTrigger("SayFarewell", "Responds to the user with a farewell message.")] ToolInvocationContext context,
        [McpToolProperty(nameof(name), "The name of the person to bid farewell.")] string? name
    )
    {
        _logger.LogInformation("C# MCP tool trigger function processed a request.");
        return $"Goodbye, {name ?? "world"}! From the MCP Tool!";
    }
}
