using Microsoft.Extensions.Configuration;
using Milan.Common.Implementations.DTOs;
using Milan.Common.Implementations.Metadata;
using Milan.Common.Interfaces.DTOs;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Serializer;
using System.Diagnostics;
using Wildcat.Milan.Shared.Dtos.Session;
using Wildcat.Milan.Storage.MongoDb;

Microsoft.Extensions.Configuration.IConfiguration configuration = new ConfigurationBuilder()
   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
   .Build();

// Create initial data
var sessionDataString = File.ReadAllText(@"data\gameid_session_data.json");
var sessionDataEnvelopeString = File.ReadAllText(@"data\gameid_session_envelope.json");
dynamic sessionData = NewtonsoftSerializer.DeserializeAutoAndReplace<SessionData<IPersistentData, IRoundData>>(sessionDataString);
dynamic sessionDataEnvelope = NewtonsoftSerializer.DeserializeAutoAndReplace<SessionDataEnvelope>(sessionDataEnvelopeString);

var backendMetaData = new BackendMetadata()
{
    BackendId = "SAMPLE_GAME",
    Name = "TEST GAME",
    Provider = "milan",
    Version = "1.0.0"
};

var gameMetaData = new GameMetaDataPayload()
{
    JackpotTemplateId = 13,
    SlotGameVariation = "variation_83",
    SlotId = "GAMEID_nmg_template",
    SlotName = "NMG Game Template"
};

var sessionDataPayload = new SessionDataPayload
{
    SessionData = sessionData,
    BackendMetadata = backendMetaData,
    GameMetaData = gameMetaData
};

// Loop x number of times pulling and storing data (simulating spin)
int iterations = 1;

var sessionDataStorage = new SessionDataStorage(configuration);
//var wildcatSessionDataStorageV1 = new WildcatSessionDataStorageV1<GamePersistentData, GameRoundData>(configuration);
//var wildcatSessionDataStorageV2 = new WildcatSessionDataStorageV2<GamePersistentData, GameRoundData>(configuration);

await RunTest(sessionDataStorage);
//await RunTest(wildcatSessionDataStorageV1);
//await RunTest(wildcatSessionDataStorageV2);
async Task RunTest(IStorage storage)
{
    Console.WriteLine();
    Console.WriteLine($">>> Starting mongoDB test ({iterations} iterations) - {storage.GetType()}");
    //Console.WriteLine($"Session key: {sessionKey.ToString()}");
    var stopWatch = Stopwatch.StartNew();

    for (int i = 0; i < iterations; i++)
    {
        var sessionKey = SessionKey.Create("SAMPLE_GAME", "1.0.0", Guid.NewGuid().ToString());
        var setDataResult = await storage.SetData(sessionKey.ToString(), sessionDataPayload);
        if (!setDataResult.Success)
        {
            throw new Exception("Failed to created initial document");
        }

        var getDataResult = await storage.GetData<SessionDataPayload>(sessionKey.ToString());
        if (!getDataResult.Success)
        {
            throw new Exception("Failed to READ document");
        }
        var updateDataResult = await storage.UpdateData(sessionKey.ToString(), sessionDataPayload);
        if (!updateDataResult.Success)
        {
            throw new Exception("Failed to UPDATE document");
        }
    }

    long elapsedMs = stopWatch.ElapsedMilliseconds;

    Console.WriteLine($">>> Took {elapsedMs}ms to perform {iterations} READ and WRITE or {(float)elapsedMs / (float)iterations / 2}ms/operation.");
}
