using System.Collections.Generic;

namespace GameBackend.Features.LockingReels.Data
{
    public class TrackingData
    {
        // For SingleCellReels these are ordered as WorldIndex
        // For Matrix they are ordered in column index
        public Dictionary<string, List<int>> Data { get; set; } = new();
    }
}
