using Microsoft.Extensions.Configuration;
using MongoDB.Driver.Core.Configuration;
using System;

namespace Wildcat.Milan.Storage.MongoDb
{
    public static class ConfigurationExtensions
    {
        private const string MONGODB_CONNECTION_STRING = "connectionStrings:milanDatabase:uri";

        public static string GetConnectionString(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var rawConnectionString = configuration.GetValue<string>(MONGODB_CONNECTION_STRING);

            if (string.IsNullOrWhiteSpace(rawConnectionString))
            {
                throw new Exception($"Connection string missing for {MONGODB_CONNECTION_STRING}");
            }

            return rawConnectionString;
        }

        public static string GetDatabaseName(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            return configuration.GetStorageConfiguration().DatabaseName;
        }

        public static StorageConfiguration GetStorageConfiguration(this IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

            var rawConnectionString = configuration.GetConnectionString();

            // To extract the database name
            var connectionString = new ConnectionString(rawConnectionString);

            var config = new StorageConfiguration()
            {
                ConnectionString = rawConnectionString,
                DatabaseName = connectionString.DatabaseName,
            };

            if (string.IsNullOrEmpty(config.ConnectionString)) throw new InvalidOperationException($"Missing mandatory configuration property '{MONGODB_CONNECTION_STRING}'");
            if (string.IsNullOrEmpty(config.DatabaseName)) throw new InvalidOperationException($"Connection string is missing mandatory database name. See mandatory configuration property ( '{MONGODB_CONNECTION_STRING}'");

            return config;
        }
    }
}
