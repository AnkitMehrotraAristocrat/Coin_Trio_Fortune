using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Milan.Common.Implementations.DTOs;
using Milan.XSlotEngine.Core.Extensions;
using Wildcat.Milan.Shared.Dtos.Backend;

namespace GameBackend
{
    /// <summary>
    /// Defines the GetGaffe service for the SlotGameBackend
    /// Retrieves the collection of gaffes configured for the backend
    /// </summary>
    public partial class SlotGameBackend
    {

        public async Task<BackendServiceResponse> GetGaffes(MilanRequest request)
        {
            var payloads = new Dictionary<string, IList<string>>();
            BackendServiceResponse response = new WildcatBackendServiceResponse {
                IsSuccess = true,
                Value = null
            };

            var gameContext = await CreateGameContext(request);
            var gaffes = gameContext.MappedConfigurations.Gaffes.Select(g => g.Key).ToList();
            gaffes.ForEach(x => payloads.AddPayload(GameConstants.GaffesPayloadName, x));

            response.IsSuccess = true;
            response.Value = payloads;
            return response;
        }
    }
}
