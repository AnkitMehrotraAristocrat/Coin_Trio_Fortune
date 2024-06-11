using System.Collections.Generic;

namespace GameBackend.Features.Blackout.Data
{
    public class TrackingData
    {
        public Dictionary<string, List<SharedDataPrizeInfo>> Prizes { get; set; } = new();
    }
}
