using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Wildcat.Milan.Storage.MongoDb
{
    /// <summary>
    /// Base class for all mongodb storage adapters
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MongoStorageAdapter<T>
        where T : class
    {
        protected MongoClient Client { get; set; }
        protected StorageConfiguration Configuration { get; }
        protected IMongoCollection<T> Collection { get; }

        protected MongoStorageAdapter(IConfiguration appConfiguration, string collectionName)
        {
            ArgumentNullException.ThrowIfNull(appConfiguration);
            ArgumentNullException.ThrowIfNullOrEmpty(collectionName);

            Configuration = appConfiguration.GetStorageConfiguration();

            var client = new MongoClient(Configuration.ConnectionString);
            var database = client.GetDatabase(Configuration.DatabaseName);
            Collection = database.GetCollection<T>(collectionName);
        }

        //private void LoadConfiguration(Microsoft.Extensions.Configuration.IConfiguration appConfiguration)
        //{
        //    var adapterPath = $"{Directory.GetCurrentDirectory()}/plugins/storage/JackpotCove.Storage.MongoDb";

        //    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        //    var builder = new ConfigurationBuilder()
        //        .SetBasePath(adapterPath)
        //        .AddJsonFile("config.json", true, true)
        //        .AddJsonFile($"config.{environment}.json", true, true)
        //        .AddEnvironmentVariables();

        //    var config = builder.Build();
        //    Configuration = config.Get<StorageConfiguration>();
        //}
    }
}