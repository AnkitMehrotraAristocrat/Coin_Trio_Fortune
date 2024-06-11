using System.Collections.Generic;

namespace GameBackend.Features.SymbolSkins.Data
{
    /* ---------------------------------------------------------------------------------------------
    |           This logic currently only support height for single reel window                     |
    |           Dev need to revisit this to support Multiple reel window                            |
    |                                                                                               |
    |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
    |                                                                                               |
     ----------------------------------------------------------------------------------------------*/

    public class OutcomeTracking
    {
        public string Id { get; set; }
        public List<OutcomeTrackingData> SymbolOutcomeData { get; set; } = new();
    }

    public class OutcomeTrackingData
    {
        public bool CanAward { get; set; }
        public int Tier { get; set; }
        public SkinData SymbolData { get; set; }
        public int WorldIndex { get; set; }
    }

    public class SpinningTrackingData
    {
        public string Id { get; set; }
        public List<SpinningIdData> Data { get; set; } = new();
    }
}
