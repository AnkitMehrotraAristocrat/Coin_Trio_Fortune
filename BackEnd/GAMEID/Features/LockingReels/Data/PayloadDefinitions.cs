using System.Collections.Generic;

namespace GameBackend.Features.LockingReels.Data
{
    public class PayloadData
    {
        public string Id { get; set; }
        public List<int> Reels { get; set; } = new();
    }
}
