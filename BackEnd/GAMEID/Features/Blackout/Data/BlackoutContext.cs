using System.Collections.Generic;

namespace GameBackend.Features.Blackout.Data
{
    // Blackout is a sub-feature 
    // Saves data to the context since multiple features can add to it (via FeatureAccess)
    // It is the responsibility of parent features to add appropriate data per spin
    public class BlackoutContext
    {
        public Dictionary<string, HashSet<int>> OccupiedCells { get; set; } = new();
        public TrackingData BlackoutData { get; set; } = new();
    }
}
