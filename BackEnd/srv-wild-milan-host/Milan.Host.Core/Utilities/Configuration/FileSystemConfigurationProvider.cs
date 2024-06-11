using Microsoft.Extensions.Options;
using Milan.Common.Implementations.Entities;
using Milan.Common.Implementations.Exceptions;
using Milan.Common.Interfaces;
using Milan.Common.Interfaces.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Wildcat.Milan.Host.Core.Utilities.Configuration
{
    public class FileSystemConfigurationProvider : IConfigurationProvider
    {
        private ImmutableHashSet<FileConfigurationByKey> _cachedConfigurations = ImmutableHashSet<FileConfigurationByKey>.Empty;

        private readonly IOptionsMonitor<ConfigurationOptions> _configurationOptions;

        /// <summary>
        /// Full path where all configuration files are located.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Variation folder name under configuration folder
        /// </summary>
        public string Variation { get; set; } = String.Empty;

        /// <summary>
        /// Configuration provider initializes with ConfigurationOptions,
        /// which contains configured values that are common for all components,
        /// such as base paths, and jackpot engine url
        /// for jackpot service calls.
        /// </summary>
        /// <param name="configurationOptions"></param>
        public FileSystemConfigurationProvider(IOptionsMonitor<ConfigurationOptions> configurationOptions)
        {
            _configurationOptions = configurationOptions;
        }

        /// <summary>
        /// Sets the configuration path for a particular
        /// instance of this configuration provider.
        /// All configuration files will be located under
        /// this path.
        /// </summary>
        /// <param name="configurationsPath"></param>
        public void SetPath(string configurationsPath) => Path = configurationsPath;

        /// <summary>
        /// Sets the configuration path for a particular
        /// instance of this configuration provider.
        /// All configuration files will be located under
        /// this path.
        /// </summary>
        /// <param name="configurationsPath"></param>
        /// <param name="cleanConfiguration"> if true, clean previous configuration</param>
        public void SetPath(string configurationsPath, bool cleanConfiguration)
        {
            SetPath(configurationsPath);
            if (cleanConfiguration)
                ClearConfigurations();
        }

        /// <inheritdoc />
        public void ClearConfigurations() => _cachedConfigurations = _cachedConfigurations.Clear();

        /// <summary>
        /// Sets the variation folder name for a particular
        /// instance of this configuration provider.
        /// All configuration files located under
        /// this variation will be used when GetConfiguration will be called for that file, 
        /// if not present at the variation folder, will load configuration from default location
        /// </summary>
        /// <param name="variationFolder">Configuration folder</param>
        public void SetVariation(string variationFolder)
        {
            Variation = variationFolder;
        }

        /// <inheritdoc />
        public bool CloneConfigurations { get; set; } = true;

        /// <summary>
        /// Retrieves configured options that are
        /// common for all components, such
        /// as base paths, and jackpot engine url
        /// for jackpot service calls.
        /// </summary>
        /// <returns></returns>
        public ConfigurationOptions GetConfigurationOptions() => _configurationOptions?.CurrentValue;

        /// <summary>
        /// Gets configuration that was previously loaded
        /// when getting configuration by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfiguration<T>() where T : IConfiguration => (T)GetCachedConfiguration(GetConfigurationKey<T>());

        /// <summary>
        /// Gets a configuration json file by name, and
        /// binds it with its provided class.
        /// </summary>
        /// <typeparam name="T">Class that must match the json file.
        /// This class needs to inherit from IConfiguration.</typeparam>
        /// <param name="configurationName">Name of the json file to find, with .json
        /// extension included.</param>
        public async Task<T> GetConfiguration<T>(string configurationName) where T : IConfiguration, new()
        {
            var configurationKey = GetConfigurationKey<T>();
            var cachedConfiguration = GetCachedConfiguration(configurationKey);

            if (cachedConfiguration != null)
                return (T)cachedConfiguration;

            T configurationBinding;

            var targetFilePath = System.IO.Path.Combine(GetRootDirectory(), configurationName);

            if (File.Exists(targetFilePath))
            {
                // Loading configuration under variation folder
                configurationBinding = await new FileBinding(targetFilePath).GenerateBinding<T>();
            }
            else
            {
                // Loading configuration from base folder
                configurationBinding = await new FileBinding(System.IO.Path.Combine(Path, configurationName)).GenerateBinding<T>();
            }

            _cachedConfigurations = _cachedConfigurations.Add(new(configurationKey, configurationBinding));

            if (configurationBinding == null)
                throw new MilanException(nameof(FileSystemConfigurationProvider), $"File not found {configurationName}");

            return configurationBinding;
        }

        /// <summary>
        /// Gets the full path to the root directory with variation (if any) containing configurations
        /// </summary>
        /// <returns></returns>
        private string GetRootDirectory() => System.IO.Path.Combine(Path, Variation);

        /// <summary>
        /// Creates a configuration key using the current Path and type
        /// </summary>
        /// <typeparam name="T">Configuration type</typeparam>
        /// <returns>Key</returns>
        /// <exception cref="NotImplementedException"></exception>
        private string GetConfigurationKey<T>() where T : IConfiguration => $"{GetRootDirectory()}{typeof(T)}";

        public FileSystemConfigurationProvider Clone()
        {
            return new FileSystemConfigurationProvider(_configurationOptions)
            {
                _cachedConfigurations = _cachedConfigurations.Select(c => new FileConfigurationByKey(c.Key, c.Configuration.Duplicate())).ToImmutableHashSet(),
                Path = Path,
                Variation = Variation
            };
        }

        private IConfiguration GetCachedConfiguration(string configurationKey)
        {
            _cachedConfigurations.TryGetValue(new FileConfigurationByKey(configurationKey), out var configuration);

            return CloneConfigurations ? configuration.Configuration?.Duplicate() : configuration.Configuration;
        }
        private class FileBinding
        {
            private readonly JsonSerializer _serializer = new();

            public string Path { get; }

            public FileBinding(string path) => Path = path;

            public async ValueTask<T> GenerateBinding<T>() where T : new()
            {
                try
                {
                    await using var file = new FileStream(Path, FileMode.Open, FileAccess.Read);
                    using var stream = new StreamReader(file);
                    using var reader = new JsonTextReader(stream);

                    return _serializer.Deserialize<T>(reader);
                }
                catch (FileNotFoundException)
                {
                    throw;
                }
                catch (DirectoryNotFoundException)
                {
                    throw;
                }
                catch (IOException)
                {
                    await Task.Delay(1);
                    return await GenerateBinding<T>();
                }
                catch (Exception exception)
                {
                    throw new MilanException(nameof(FileSystemConfigurationProvider), $"Deserialization failure {Path}. Error: {exception.Message} - {exception.InnerException}");
                }
            }
        }

        private record FileConfigurationByKey(string Key, IConfiguration Configuration)
        {
            public FileConfigurationByKey(string key) : this(key, null)
            {
            }

            public override int GetHashCode() => Key.GetHashCode();

            public virtual bool Equals(FileConfigurationByKey other) => Key == other.Key;
        }
    }
}
