using System.Collections.Generic;

namespace GameBackend.Features.Blackout.Data
{
    public class PayloadData
    {
        public string Id { get; set; }
        public bool Blackout { get; set; }
        public List<SharedDataPrizeInfo> Prizes { get; set; } = new();
    }
}
