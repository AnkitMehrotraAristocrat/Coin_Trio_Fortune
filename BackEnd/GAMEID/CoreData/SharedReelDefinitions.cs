using Milan.Common.Interfaces.Entities;
using System.Collections.Generic;

namespace GameBackend.Data
{
    public class ReelSetsData : IConfiguration
    {
        public List<ReelSets> ReelSets { get; set; } = new List<ReelSets>();

        public object Clone() => Duplicate();

        public IConfiguration Duplicate()
        {
            return new ReelSetsData() {
                ReelSets = ReelSets
            };
        }
    }

    public class ReelSets
    {
        public string Name { get; set; }
        public List<string> Reels { get; set; }
    }

    public class ReelOutcome
    {
        public string Id { get; set; }
        public string ReelWindowId { get; set; }

        // For SingleCellReels these are ordered as WorldIndex
        // For Matrix they are ordered in column index
        public List<string> IndexedReelStrips { get; set; }
        public List<int> IndexedOffsets { get; set; }
    }

    public class ReelOutcomePayload
    {
        public string Id { get; set; }
        public string ReelWindowId { get; set; }

        // These are always ordered for client window
        // By index preference (LTBR/TBLR) for SingleCellReels, and column index for Matrix
        public List<string> ReelStrips { get; set; }
        public List<int> Offsets { get; set; }
    }
}