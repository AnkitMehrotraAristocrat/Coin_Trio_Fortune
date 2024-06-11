using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using System.Threading.Tasks;
using Milan.Common.Implementations.DTOs;
using Newtonsoft.Json.Linq;
using GameBackend.Helpers;
using Wildcat.Milan.Shared.Dtos.Backend;

namespace GameBackend
{
    public partial class SlotGameBackend
    {
        /// <summary>
        /// Stages a queue of random numbers to pull from before generating random values with the RngHelper
        /// </summary>
        public async Task<BackendServiceResponse> CustomRandomNumbers(MilanRequest request)
        {
            BackendServiceResponse response = new WildcatBackendServiceResponse {
                IsSuccess = false,
                Value = null
            };

            var customRequest = request.GetArguments<CustomRandomNumbersRequest>(request.Payload.ToString());
            var gameContext = await CreateGameContext(request);
            gameContext.PersistentData.RandomNumberQueue.Clear();

            var isThereReelSetRequest = false;
            foreach (var jToken in customRequest.RandomNumberQueue) {
                if (jToken.Type is JTokenType.Integer) {
                    gameContext.PersistentData.RandomNumberQueue.Enqueue(jToken.Value<ulong>());
                }
                else {
                    // For CategoryQueues
                    string gaffeCat = jToken.Value<string>();
                    if (gaffeCat == GaffeCategories.SelectReelSet.ToString()) {
                        isThereReelSetRequest = true;
                    }
                    gameContext.PersistentData.GaffeQueues.InterpretGaffeString(gaffeCat);
                    //
                    gameContext.PersistentData.RandomNumberQueue.Enqueue(null);
                }
            }
            // For CategoryQueues
            gameContext.PersistentData.GaffeQueues.SetCategoryQueues(gameContext.PersistentData.RandomNumberQueue);

            // grab the first number of non-categorized gaffes and consume it for selecting the reel set in case of non-categorized gaffes
            if (GeneralHelper.GetPreferenceBool(GameConstants.SelectReelSetPreferenceKey) && !gameContext.PersistentData.GaffeQueues.HasCategoryQueues()) {
                isThereReelSetRequest = true;
            }

            // If gaffe has data for Selection of reel set then update same on the fly and return response to the client.
            if (isThereReelSetRequest) {
                response.Value = ReelSetsFeatureAccess.SetNextReelSetAndAddReelSetsPayload(gameContext);
            }

            response.IsSuccess = true;
            return response;
        }
    }
}
