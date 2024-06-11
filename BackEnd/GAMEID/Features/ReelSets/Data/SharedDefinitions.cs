using System.Collections.Generic;

namespace GameBackend.Features.ReelSets.Data
{
    public class NextReelStripsWindowData
    {
        public string Id { get; set; }
        public bool PerBetIndexEnabled { get; set; }
        public Dictionary<int, string[]> NextReelStripsData { get; set; }

        public NextReelStripsWindowData() { }
        public NextReelStripsWindowData(string id, bool perBetIndexEnabled, Dictionary<int, string[]> data)
        {
            Id = id;
            PerBetIndexEnabled = perBetIndexEnabled;
            NextReelStripsData = data;
        }
    }
}
