using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Wildcat.Milan.Host.Core.Utilities.Configuration;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Utilities
{
    public static class ConfigurationProviderExtensions
    {
        /// <summary>
        /// Returns a list of variations available in the provided game configuration.
        /// NOTE: this assumes that the ConfigurationProvider is using file-based configurations.
        /// </summary>
        /// <param name="configurationProvider">Holds all configuration providers</param>
        /// <returns>List of variations</returns>
        public static List<GameVariationModel> GetGameVariations(this FileSystemConfigurationProvider configurationProvider)
        {
            ArgumentNullException.ThrowIfNull(configurationProvider);

            var variations = Directory.GetDirectories(configurationProvider.Path)
                .Select(fullDirectoryPath => new GameVariationModel()
                {
                    Id = Path.GetFileName(fullDirectoryPath.TrimEnd(Path.DirectorySeparatorChar))
                })
                .ToList();

            return variations;
        }
    }
}
