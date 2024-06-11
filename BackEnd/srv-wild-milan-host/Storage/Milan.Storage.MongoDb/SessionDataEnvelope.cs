using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Wildcat.Milan.Storage.MongoDb
{
    public class SessionDataEnvelope
    {
        [BsonId]
        public required string Id { get; set; }

        [BsonElement("player_id", Order = 2)]
        public required string PlayerId { get; set; }

        [BsonElement("created_on", Order = 3)]
        public required DateTime CreatedOn { get; set; }

        [BsonElement("updated_on", Order = 4)]
        public required DateTime UpdatedOn { get; set; }

        [BsonElement("backend_metadata", Order = 5)]
        public required BsonDocument BackendMetadata { get; set; }

        [BsonElement("game_metadata", Order = 6)]
        public required BsonDocument GameMetaData { get; set; }

        [BsonElement("session_data", Order = 7)]
        public required string SessionData { get; set; }
    }
}