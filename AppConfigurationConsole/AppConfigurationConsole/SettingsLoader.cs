using AppConfigurationConsole.Structures;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppConfigurationConsole
{
    internal static class SettingsLoader
    {
        internal static void Load(IConfigurationRoot config)
        {
            List<ConfigItem> configItems = [];
            string[] known_keys = [
                "OpenAI:Endpoint",
                "OpenAI:DeploymentName",
                "OpenAI:ApiKey",
                "Pipeline:BatchSize",
                "Pipeline:RetryCount",
                "Sentinel"
            ];

            try
            {
                foreach (string key in known_keys)
                {
                    string value = config[key]!;
                    try
                    {
                        configItems.Add(new ConfigItem
                        {
                            Key = key,
                            Value = value,
                            Type = key == "OpenAI:ApiKey" ? "Key Vault reference" : "configuration",
                            Status = "loaded"
                        });
                    }
                    catch (Exception)
                    {
                        configItems.Add(new ConfigItem
                        {
                            Key = key,
                            Value = "",
                            Type = "unknown",
                            Status = "not found"
                        });
                    }
                }

                int loaded = configItems.Count(i => i.Status == "loaded");
                Console.WriteLine($"Loaded {loaded} of {configItems.Count} setting(s).");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading settings: {ex}");
            }

        }
    }
}
