using Azure.Identity;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

Console.WriteLine("Hello, World!");

using var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.SetMinimumLevel(LogLevel.Trace);
});

ConfigurationOptions configurationOptions = new()
{
    // Use the RESP3 protocol instead of RESP2 so pub/sub messages share the same connection with Redis commands without interruption when tokens expire. 
    Protocol = RedisProtocol.Resp3,

    // Supply this sample app's logger factory so we can get logs from both Microsoft.Azure.StackExchangeRedis and StackExchange.Redis.
    LoggerFactory = loggerFactory,

    // Fail fast for the purposes of this sample. In production code, AbortOnConnectFail should remain false to retry connections on startup.
    AbortOnConnectFail = true,

    // Fail commands immediately when a connection isn't available, rather than backlogging them for execution when connection is restored.
    // This option is useful for exposing any connection drops for the sample, but production code should always use BacklogPolicy.Default for resilience.
    BacklogPolicy = BacklogPolicy.FailFast,

    // Ensure we use the latest TLS 1.3 for security and compatibility with Azure Redis minimum supported version.
    SslProtocols = System.Security.Authentication.SslProtocols.Tls13,
};

configurationOptions.EndPoints.Add("bps-redis-cache-uouxshlqhqmbs.centralus.redis.azure.net:10000");

await configurationOptions.ConfigureForAzureWithTokenCredentialAsync(new DefaultAzureCredential());
ConnectionMultiplexer connection = await ConnectionMultiplexer.ConnectAsync(configurationOptions);

var database = connection.GetDatabase();
await database.StringSetAsync("key", "somevalue");
var value = await database.StringGetAsync("key");

Console.WriteLine($"Hello, World! {value}");
