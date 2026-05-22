using Azure.Identity;
using Azure.Messaging.EventGrid.Namespaces;
using dotenv.net;
using System;

namespace EventGridConsole
{
    internal static class EventGridReceiverHelper
    {
        internal static EventGridReceiverClient GetClient(string topic, string subscription)
        {
            DefaultAzureCredentialOptions options = new()
            {
                ExcludeEnvironmentCredential = true,
                ExcludeManagedIdentityCredential = true
            };
            return new(new Uri(DotEnv.Read()["ENDPOINT"]), topic, subscription, new DefaultAzureCredential(options));
        }
    }
}
