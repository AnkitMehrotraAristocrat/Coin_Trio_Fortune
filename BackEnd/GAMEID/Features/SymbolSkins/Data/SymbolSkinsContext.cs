using System.Collections.Generic;

namespace GameBackend.Features.SymbolSkins.Data
{
    // SymbolSkins is a sub-feature 
    // Saves data to the context since multiple features can add to it (via FeatureAccess)
    // It is the responsibility of parent features to add appropriate data per spin
    public class SymbolSkinsContext
    {
        public List<OutcomeTracking> SymbolOutcomeTrackingData { get; set; } = new();
        public List<SpinningTrackingData> SymbolSpinningTrackingData { get; set; } = new();
    }
}
