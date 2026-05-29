using AppConfigurationConsole.Structures;
using Azure.Data.AppConfiguration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppConfigurationConsole
{
    internal static class ConfigurationRefresher
    {
        internal static async Task RefreshSettings(IConfigurationRoot config, ConfigurationClient configurationClient)
        {
            string[] tracked_keys = ["Pipeline:BatchSize"];
            try
            {
                Dictionary<string, string> before = [];
                foreach (string key in tracked_keys)
                {
                    try
                    {
                        before[key] = config[key]!;
                    }
                    catch(Exception)
                    {
                        before[key] = "—";
                    }
                }

                Random random = new();
                string randomNumber = random.Next(100, 999).ToString();
                ConfigurationSetting setting = new("Pipeline:BatchSize", randomNumber, "Production") { ContentType = "text/plain" };
                await configurationClient.SetConfigurationSettingAsync(setting);

                string newSentinel = DateTime.Now.ToFileTimeUtc().ToString();
                setting = new("Sentinel", newSentinel);
                await configurationClient.SetConfigurationSettingAsync(setting);

                await Task.Delay(2000);
                config.Reload();

                Dictionary<string, string> after = [];
                foreach (string key in tracked_keys)
                {
                    try
                    {
                        after[key] = config[key]!;
                    }
                    catch (Exception)
                    {
                        after[key] = "—";
                    }
                }

                List<TrackedSetting> trackedSettings = [];
                foreach (string key in tracked_keys)
                {
                    trackedSettings.Add(new TrackedSetting { Key = key, Before = before[key], After = after[key], Changed = before[key] != after[key] });
                }

                bool batchSizeUpdated = after["Pipeline:BatchSize"] == randomNumber;
                if (batchSizeUpdated)
                    Console.WriteLine($"Configuration refreshed successfully.");
                else
                    Console.WriteLine($"Refresh completed but changes may not have propagated.");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error  listing settings: {ex}");
            }
        }
    }
}
