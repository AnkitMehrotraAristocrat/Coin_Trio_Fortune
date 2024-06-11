using GameBackend.Data;
using System.Collections.Generic;

namespace GameBackend.Features.Cors.Data
{
    public class CorPrizes
    {
        public List<CorPrizeInfo> Prizes { get; set; } = new();
    }

    public class CorPrizeInfo
    {
        public FlexibleStop Stop { get; set; }
        public string Type { get; set; }
        public double Value { get; set; }
        public string Name { get; set; }
        public int Tier { get; set; }
        public int JackpotBaseMultiplier { get; set; }
        public bool Awarded { get; set; }

        public CorPrizeInfo(double value, string type, string name, int tier = -1, int jackpotBaseMultiplier = 1)
        {
            Value = value;
            Type = type;
            Name = name;
            Tier = tier;
            JackpotBaseMultiplier = jackpotBaseMultiplier;
            Awarded = false;
        }
    }
}