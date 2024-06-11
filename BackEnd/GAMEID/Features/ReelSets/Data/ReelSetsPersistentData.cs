
namespace GameBackend.Features.ReelSets.Data
{
    /// <summary>
    /// A custom data class for the data pertaining to each bet level the player is able to play at
    /// </summary>
    public class ReelSetsPersistentData
    {
        public NextReelStripsWindowsData ReelStripsPerBetIndex { get; set; } = new();
    }
}
