
namespace GameBackend.Features.LockingReels.Data
{
    // LockingReels is a sub-feature 
    // Saves data to the context since multiple features can add to it (via FeatureAccess)
    // It is the responsibility of parent features to add appropriate data per spin
    public class LockingReelsContext
    {
        public TrackingData LockingReelsData { get; set; } = new();
    }
}
