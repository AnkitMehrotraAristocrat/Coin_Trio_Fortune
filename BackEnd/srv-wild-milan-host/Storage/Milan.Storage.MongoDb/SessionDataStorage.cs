using JetBrains.Annotations;
using Microsoft.Extensions.Configuration;
using Milan.Common.Implementations.DTOs;
using Milan.Common.Implementations.Enums;
using Milan.Common.Implementations.Metadata;
using Milan.Common.Interfaces.DTOs;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Serializer;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using NewRelic.Api.Agent;
using Newtonsoft.Json;
using System;
using System.Composition;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Wildcat.Milan.Shared.Dtos.Host;
using Wildcat.Milan.Shared.Dtos.Session;

namespace Wildcat.Milan.Storage.MongoDb
{
    [UsedImplicitly]
    [Export(typeof(IStorage))]
    public class SessionDataStorage : MongoStorageAdapter<SessionDataEnvelope>, IStorage, ISessionDataStorage
    {
        private static JsonSerializerSettings AutoAndReplaceSetting = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public SessionDataStorage(Microsoft.Extensions.Configuration.IConfiguration appConfiguration)
            : base(appConfiguration, appConfiguration.GetValue<string>("connectionStrings:milanDatabase:collectionName"))
        {
            Metadata = new StorageMetadata
            {
                Name = "MongoSessionStorage",
                Type = StorageType.Document,
                Version = "1.0",
                Provider = ""
            };
        }

        public async Task<StatusResponse> Status()
        {
            var response = new StatusResponse(StatusType.Storage);
            try
            {
                using var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));
                await Client.ListDatabaseNamesAsync(timeoutCancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                response.Message = ex.Message;
                response.HasErrored = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.HasErrored = true;
                return response;
            }

            if (Client.Cluster.Description.State == ClusterState.Disconnected)
            {
                response.Message = "Cluster in disconnected state";
                response.HasErrored = true;
            }

            // return true if the state of the last operation is connected
            return response;
        }

        private static SessionDataPayload CreateResultPayload(SessionDataEnvelope dataEnvelope)
        {
            dynamic sessionData = NewtonsoftSerializer.Deserialize<SessionData<IPersistentData, IRoundData>>(dataEnvelope.SessionData, AutoAndReplaceSetting);

            var SessionDataPayload = new SessionDataPayload
            {
                SessionData = sessionData,
                BackendMetadata = BsonSerializer.Deserialize<BackendMetadata>(dataEnvelope.BackendMetadata),
                GameMetaData = BsonSerializer.Deserialize<GameMetaDataPayload>(dataEnvelope.GameMetaData)
            };

            return SessionDataPayload;
        }

        public async Task<StorageResult<T>> GetData<T>(string key) where T : class, new()
        {
            try
            {
                var result = new StorageResult<T>
                {
                    Success = true
                };

                var data = await GetRawSessionData(key);
                if (data == null)
                {
                    return result;
                }

                var sessionDataPayload = CreateResultPayload(data);

                result.Value = sessionDataPayload as T;
                if(result.Value == null)
                {
                    throw new Exception($"Unable to fetch session data of type {typeof(T)}");
                }

                return result;
            }
            catch (Exception e)
            {
                return new StorageResult<T>
                {
                    ErrorMessage = e.Message,
                };
            }
        }

        private async Task<SessionDataEnvelope> GetRawSessionData(string key)
        {
            return await Collection.Find(x => x.Id == key).FirstOrDefaultAsync();
        }

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<StorageResult> GetData(string key)
        {
            try
            {
                var result = new StorageResult
                {
                    Success = true
                };

                var data = await GetRawSessionData(key);

                if (data == null)
                {
                    return result;
                }

                result.Value = CreateResultPayload(data);
                return result;

            }
            catch (Exception e)
            {
                return new StorageResult
                {
                    ErrorMessage = e.Message,
                };
            }
        }



        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<StorageResult<T>> SetData<T>(string key, T data) where T : class, new()
        {
            try
            {
                var sessionKey = SessionKey.Deserialize(key);
                if (data is not SessionDataPayload sessionDataPayload)
                {
                    throw new ArgumentException("data not specified or not a SessionDataPayload", nameof(data));
                }

                var currentDateTime = DateTime.UtcNow;
                var stringSessionData = NewtonsoftSerializer.Serialize(sessionDataPayload.SessionData, AutoAndReplaceSetting);

                var envelope = new SessionDataEnvelope
                {
                    Id = key,
                    PlayerId = sessionKey.PlayerId,
                    CreatedOn = currentDateTime,
                    UpdatedOn = currentDateTime,
                    BackendMetadata = sessionDataPayload.BackendMetadata.ToBsonDocument(),
                    GameMetaData = sessionDataPayload.GameMetaData.ToBsonDocument(),
                    SessionData = stringSessionData
                };

                await Collection.InsertOneAsync(envelope, null, CancellationToken.None);

                var result = new StorageResult<T>
                {
                    Success = true,
                    Value = data
                };
                return result;
            }
            catch (Exception ex)
            {
                return new StorageResult<T>
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<StorageResult> DeleteData(string key)
        {
            try
            {
                await Collection.DeleteOneAsync(x => x.Id == key);
            }
            catch (Exception ex)
            {
                return new StorageResult
                {
                    ErrorMessage = ex.Message
                };
            }

            var result = new StorageResult
            {
                Success = true
            };

            return result;
        }

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<StorageResult> UpdateData<T>(string key, T data) where T : class, new()
        {
            try
            {
                if (data is not SessionDataPayload sessionDataPayload)
                {
                    throw new ArgumentException("data not specified or not a SessionDataPayload", nameof(data));
                }

                var stringSessionData = NewtonsoftSerializer.Serialize(sessionDataPayload.SessionData, AutoAndReplaceSetting);

                var filterDefinition = Builders<SessionDataEnvelope>.Filter
                    .Eq(x => x.Id, key);

                // these are the only three fields that should change so update rather than replace the entire document
                var updateDefinition = Builders<SessionDataEnvelope>.Update
                    .Set(x => x.UpdatedOn, DateTime.UtcNow)
                    .Set(x => x.SessionData, stringSessionData)
                    .Set(x => x.GameMetaData, sessionDataPayload.GameMetaData.ToBsonDocument());

                await Collection.UpdateOneAsync(filterDefinition, updateDefinition);

                var result = new StorageResult
                {
                    Success = true,
                    Value = data
                };

                return result;
            }
            catch (Exception ex)
            {
                return new StorageResult
                {
                    ErrorMessage = ex.Message
                };
            }
        }

        public Task<StorageResult> IncrementCounter(string key)
        {
            throw new NotImplementedException();
        }

        public Task<StorageMetadata> GetStorageType()
        {
            throw new NotImplementedException();
        }

        public Task<StorageResult> ExecuteTransaction(TransactionSteps steps)
        {
            var result = steps();
            return Task.FromResult(result);
        }

        public StorageMetadata Metadata { get; set; }
    }
}
