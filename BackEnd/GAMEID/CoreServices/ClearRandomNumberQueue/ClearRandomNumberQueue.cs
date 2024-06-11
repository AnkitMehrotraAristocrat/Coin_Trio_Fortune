using System.Threading.Tasks;
using Milan.Common.Implementations.DTOs;
using Wildcat.Milan.Shared.Dtos.Backend;

namespace GameBackend
{
    /// <summary>
    /// Defines the ClearRandomNumberQueue service for the SlotGameBackend
    /// Clears any values that may be present in the random number queue
    /// This service does nothing when built for release
    /// </summary>
    public partial class SlotGameBackend
    {
        public async Task<BackendServiceResponse> ClearRandomNumberQueue(MilanRequest request)
        {
            BackendServiceResponse response = new WildcatBackendServiceResponse {
                IsSuccess = false,
                Value = null
            };

            var gameContext = await CreateGameContext(request);
            gameContext.PersistentData.RandomNumberQueue.Clear();
            gameContext.PersistentData.GaffeQueues.Clear();

            // WILD: this is only sample code to show how to clear the GaffeId from the spins.
            gameContext.PersistentData.GaffeData.Name = null;

            response.IsSuccess = true;

            return response;
        }
    }
}
