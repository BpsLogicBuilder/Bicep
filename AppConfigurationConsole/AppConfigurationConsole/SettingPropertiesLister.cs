using Azure.Data.AppConfiguration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AppConfigurationConsole
{
    internal static class SettingPropertiesLister
    {
        internal static async Task ListSettingProperties(ConfigurationClient configurationClient)
        {
            try
            {
                List<ConfigurationSetting> configurationSettings = [];
                await foreach(ConfigurationSetting setting in configurationClient.GetConfigurationSettingsAsync(new SettingSelector() { KeyFilter = "*", LabelFilter = null }))
                {
                    configurationSettings.Add(setting);
                }

                await foreach (ConfigurationSetting setting in configurationClient.GetConfigurationSettingsAsync(new SettingSelector() { KeyFilter = "*", LabelFilter = "Production" }))
                {
                    configurationSettings.Add(setting);
                }

                Console.WriteLine($"Found {configurationSettings.Count} setting(s).");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error  listing settings: {ex}");
            }
        }
    }
}
