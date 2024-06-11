using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milan.Common.Implementations.DTOs;
using GameBackend.Helpers;
using Wildcat.Milan.Shared.Dtos.Backend;

namespace GameBackend
{
    public partial class SlotGameBackend
    {
        /// <summary>
        /// Stages a queue of random numbers specified in the backend's gaffes configuration. Random values are pulled
        /// from the queue of provided values before generating random values with the RngHelper.
        /// </summary>
        public async Task<BackendServiceResponse> Gaffe(MilanRequest request)
        {
            BackendServiceResponse response = new WildcatBackendServiceResponse {
                IsSuccess = false,
                Value = null
            };

            var gameContext = await CreateGameContext(request);
            var gaffes = gameContext.XSlotConfigurationProvider.GetGaffesMapped();
            var gaffeRequest = request.GetArguments<GaffeRequest>(request.Payload.ToString());
            var gaffeValues = gaffeRequest.Index != null
                ? GetGaffeValuesByIndex((int)gaffeRequest.Index, gaffes)
                : GetGaffeValuesByName(gaffeRequest.Name, gaffes);

            gameContext.PersistentData.RandomNumberQueue.Clear();
            foreach (var number in gaffeValues) {
                gameContext.PersistentData.RandomNumberQueue.Enqueue(number);
            }

            var isThereReelSetRequest = false;
            // For CategoryQueues
            var prog = gameContext.XSlotConfigurationProvider.XSlotConfigurations.GaffesConfiguration.Programs.Find(prog => prog.Name == gaffeRequest.Name);
            foreach (object value in prog.Values) {
                if (Type.GetTypeCode(value.GetType()) == TypeCode.String) {
                    string gaffeCat = (string)value;
                    if (gaffeCat == GaffeCategories.SelectReelSet.ToString()) {
                        isThereReelSetRequest = true;
                    }
                    gameContext.PersistentData.GaffeQueues.InterpretGaffeString(gaffeCat);
                }
            }
            gameContext.PersistentData.GaffeQueues.SetCategoryQueues(gameContext.PersistentData.RandomNumberQueue);
            //

            // grab the first number of non-categorized gaffes and consume it for selecting the reel set in case of non-categorized gaffes
            if (GeneralHelper.GetPreferenceBool(GameConstants.SelectReelSetPreferenceKey) && !gameContext.PersistentData.GaffeQueues.HasCategoryQueues()) {
                isThereReelSetRequest = true;
            }

            // If gaffe has data for Selection of reel set then update same on the fly and return response to the client.
            if (isThereReelSetRequest) {
                response.Value = ReelSetsFeatureAccess.SetNextReelSetAndAddReelSetsPayload(gameContext);
            }

            // WILD: this is only sample code to preserve the GaffeId
            gameContext.PersistentData.GaffeData.Name = gaffeRequest.Name;

            response.IsSuccess = true;
            return response;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static IList<ulong?> GetGaffeValuesByIndex(int index, IDictionary<string, IList<ulong?>> gaffes)
        {
            if (index > gaffes.Count) {
                throw new IndexOutOfRangeException(GameConstants.ErrorGaffeIndex);
            }
            return gaffes[gaffes.Keys.ToList()[index]];
        }

        private static IList<ulong?> GetGaffeValuesByName(string name, IDictionary<string, IList<ulong?>> gaffes)
        {
            if (!gaffes.ContainsKey(name)) {
                throw new KeyNotFoundException(string.Format(GameConstants.ErrorGaffeNameFormat, name));
            }
            return gaffes[name];
        }
    }
}
