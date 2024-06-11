using Microsoft.Extensions.Options;
using Milan.Common.Implementations.Entities;
using Milan.Common.Interfaces;
using Milan.Common.Interfaces.Utilities;
using Milan.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Wildcat.Milan.Host.Core.Utilities.Configuration
{
    /// <summary>
    /// Wildcat specific configuration manager: we now support only 1 game per host, the
    /// configuration management can therefore be simplified.
    /// </summary>
    public class WildcatConfigurationManager : IConfigurationManager
    {
        private readonly IOptionsMonitor<ConfigurationOptions> _configurationOptions;
        public IDictionary<string, IConfigurationProvider> ConfigurationProviders { get; set; }
        public string GameId { get; }

        public WildcatConfigurationManager(string gameId, IOptionsMonitor<ConfigurationOptions> configurationOptions, IServiceProvider serviceProvider = null)
        {
            GameId = gameId;
            _configurationOptions = configurationOptions;
            LoadConfigurationProviders();
        }

        private void LoadConfigurationProviders()
        {
            ConfigurationProviders = new Dictionary<string, IConfigurationProvider>();
            IConfigurationProvider configurationProvider = new FileSystemConfigurationProvider(_configurationOptions);
            var configDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), _configurationOptions.CurrentValue.Directories.ConfigurationDirectory);
            configurationProvider.SetPath(configDirectory);
            ConfigurationProviders.Add(GameId, configurationProvider);
        }
    }
}
