using Milan.Common.Implementations.DTOs;
using Milan.Common.Implementations.Enums;
using Milan.Common.Implementations.Metadata;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Serializer;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Composition;
using System.IO;
using System.Threading.Tasks;
using static Milan.Storage.FileSystem.LfsStorageConstants;

namespace Milan.Storage.FileSystem
{
    [Export(typeof(IStorage))]
    public class LfsStorage : IStorage
    {
        private ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();
        public StorageMetadata Metadata { get; set; }

        public LfsStorage()
        {
            Metadata = new StorageMetadata
            {
                Name = StorageName,
                Type = StorageType.Document,
                Version = StorageVersion,
                Provider = StorageProvider
            };

            Init();
        }

        public Task<StorageResult> DeleteData(string key)
        {
            try
            {
                // Failed deletions by missing entries ignored
                _data.TryRemove(key, out _);

                return Task.FromResult(SuccessfulResult());
            }
            catch (Exception ex)
            {
                return Task.FromResult(UnsuccessfulResult(ex.Message));
            }
        }

        public async Task<StorageResult<T>> GetData<T>(string key) where T : class, new()
        {
            try
            {
                await Load();

                if (!_data.ContainsKey(key))
                    return SuccessfulResult<T>(default);

                var data = _data[key];

                T value;
                switch (data)
                {
                    case JToken token:
                        value = NewtonsoftSerializer.DeserializeAutoAndReplace<T>(token.ToString());
                        break;
                    case T typedData:
                        value = typedData;
                        break;
                    default:
                        value = default;
                        break;
                }

                return SuccessfulResult(value);

            }
            catch (Exception ex)
            {
                return UnsuccessfulResult<T>(ex.Message);
            }
        }

        public async Task<StorageResult> GetData(string key)
        {
            await Load();

            try
            {
                return SuccessfulResult(_data[key]);
            }
            catch (Exception ex)
            {
                return UnsuccessfulResult(ex.Message);
            }
        }

        public Task<StorageMetadata> GetStorageType() => Task.FromResult(Metadata);

        public Task<StorageResult> IncrementCounter(string key)
        {
            try
            {
                _data[key] = Convert.ToInt64(_data[key]) + 1;
                return Task.FromResult(SuccessfulResult(_data[key]));
            }
            catch (Exception ex)
            {
                return Task.FromResult(UnsuccessfulResult(ex.Message));
            }
        }

        public async Task<StorageResult<T>> SetData<T>(string key, T data) where T : class, new()
        {
            try
            {
                _data.TryAdd(key, data);
                await Persist();

                return SuccessfulResult((T)_data[key]);
            }
            catch (Exception ex)
            {
                return UnsuccessfulResult<T>(ex.Message);
            }
        }

        public async Task<StorageResult> UpdateData<T>(string key, T data) where T : class, new()
        {
            try
            {
                _data[key] = data;
                await Persist();

                return SuccessfulResult(_data[key]);
            }
            catch (Exception ex)
            {
                return UnsuccessfulResult(ex.Message);
            }
        }

        public Task<StorageResult> ExecuteTransaction(TransactionSteps steps)
        {
            var result = steps();
            return Task.FromResult(result);
        }

        private async ValueTask Load()
        {
            try
            {
                using var reader = new StreamReader(DefaultStorageFilename);
                var storageData = await reader.ReadToEndAsync();
                if (storageData.Length == 0)
                    return;

                _data = NewtonsoftSerializer
                    .DeserializeAutoAndReplace<ConcurrentDictionary<string, object>>(storageData);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while loading session storage file: '{DefaultStorageFilename}'", ex);
            }
        }

        private async Task Persist()
        {
            await using var writer = new StreamWriter(DefaultStorageFilename);
            await writer.WriteAsync(NewtonsoftSerializer.SerializeAuto(_data));
        }

        private static StorageResult<T> SuccessfulResult<T>(T value) => new StorageResult<T>
        {
            Success = true,
            ErrorMessage = null,
            Value = value
        };

        private static StorageResult SuccessfulResult(object value = null) => new StorageResult
        {
            Success = true,
            ErrorMessage = null,
            Value = value
        };

        private static StorageResult UnsuccessfulResult(string messageError) => new StorageResult
        {
            ErrorMessage = messageError,
            Value = default
        };

        private static StorageResult<T> UnsuccessfulResult<T>(string messageError) => new StorageResult<T>
        {
            ErrorMessage = messageError,
            Value = default
        };

        private static void Init()
        {
            if (!File.Exists(DefaultStorageFilename))
                File.Create(DefaultStorageFilename).Close();
        }
    }
}