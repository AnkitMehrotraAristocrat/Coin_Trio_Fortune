using Microsoft.Extensions.Configuration;
using System;

namespace Wildcat.Milan.Host.Core
{
    public static class ConfigurationExtentions
    {
        private const string SERVICE_ADAPTER_STORAGE_PLUGIN = "milan:serviceAdapter:plugin";
        public const string JACKPOT_ENGINE_CONFIG_KEY = "jackpotEngine";

        public static string GetStoragePluginName(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var storagePlugin = configuration.GetValue<string>(SERVICE_ADAPTER_STORAGE_PLUGIN);

            if (string.IsNullOrEmpty(storagePlugin)) throw new InvalidOperationException($"Missing mandatory configuration property '{SERVICE_ADAPTER_STORAGE_PLUGIN}'");

            return storagePlugin;
        }
    }
}
