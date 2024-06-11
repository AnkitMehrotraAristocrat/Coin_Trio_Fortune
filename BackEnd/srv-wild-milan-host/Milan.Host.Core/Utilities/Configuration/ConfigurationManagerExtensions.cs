using Milan.Common.Interfaces.Utilities;
using System;
using System.Collections.Generic;
using Wildcat.Milan.Host.Core.Utilities.Configuration;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Utilities
{
    public static class ConfigurationManagerExtensions
    {
        /// <summary>
        /// Returns a list of variations available in the provided game configuration.
        /// NOTE: this assumes that the ConfigurationProvider is using file-based configurations.
        /// </summary>
        /// <param name="configurationManager">Holds all configuration providers</param>
        /// <param name="gameId">Target game Id</param>
        /// <returns>List of variations</returns>
        public static List<GameVariationModel> GetGameVariations(this IConfigurationManager configurationManager, string gameId)
        {
            ArgumentNullException.ThrowIfNull(configurationManager);
            if (string.IsNullOrWhiteSpace(gameId)) throw new ArgumentNullException(nameof(gameId));

            configurationManager.ConfigurationProviders.TryGetValue(gameId, out var configurationProvider);

            if (configurationProvider == null) throw new InvalidOperationException($"Unable to find configuration provider for game id: '{gameId}'");

            var milanConfigurationProvider = configurationProvider as FileSystemConfigurationProvider;
            if (milanConfigurationProvider == null) throw new InvalidOperationException($"Unable to extract configuration path from configuration provider for game id: '{gameId}'");

            return milanConfigurationProvider.GetGameVariations();
        }
    }
}
