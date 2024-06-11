using System.Collections.Generic;

namespace GameBackend.Features.ReelGrowth.Data
{
    public class ReelGrowthRoundData
    {
        public int GrowthStep { get; set; }
        public List<int> PreviousGrowthStops { get; set; }
        public PayloadData LastFeaturePayload { get; set; }

        public ReelGrowthRoundData()
        {
            PreviousGrowthStops = new List<int>();
        }
    }
}
