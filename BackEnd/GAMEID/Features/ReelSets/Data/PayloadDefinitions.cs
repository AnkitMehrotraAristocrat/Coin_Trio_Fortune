using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.ReelSets.Data
{
    public class PayloadData
    {
        public string Id { get; set; }
        public bool PerBetIndexEnabled { get; set; }
        public Dictionary<int, string[]> NextReelStripsData { get; set; }

        public PayloadData(NextReelStripsWindowData data, int betIndex = -1)
        {
            Id = data.Id;
            PerBetIndexEnabled = data.PerBetIndexEnabled;
            if (betIndex == -1) {
                NextReelStripsData = data.NextReelStripsData;
            }
            else {
                NextReelStripsData = data.NextReelStripsData.Where(x => x.Key == betIndex).ToDictionary(x => x.Key, x => x.Value);
            }
        }
    }
}
