using Milan.Common.Interfaces.Entities;
using Milan.Common.Interfaces.Utilities;
using System;
using System.Linq;
using Wildcat.Milan.Host.Utilities;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Utilities
{
    public static class IBackendExtensions
    {
        /// <summary>
        /// Returns the details of the hosted games, with supported services and variations.
        /// </summary>
        public static GameVersionModel GetGameVersionDetails(this IBackend backend, IConfigurationManager configurationManager, IHostVersionHelper hostVersionHelper)
        {
            ArgumentNullException.ThrowIfNull(backend);
            ArgumentNullException.ThrowIfNull(hostVersionHelper);

            return new GameVersionModel
            {
                GameId = backend.Metadata.Name,
                Version = hostVersionHelper.GameVersion,
                Services = backend.BackendServices.Keys.ToList(),
                Variations = configurationManager.GetGameVariations(backend.Metadata.Name),
                HostVersion = hostVersionHelper.HostVersion
            };
        }
    }
}
