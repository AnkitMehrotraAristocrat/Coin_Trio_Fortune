using System.Collections.Generic;
using Milan.Shared.DTO.FrontEnd;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutPayload : IHasId
    {
        public string Id { get; set; }
        public bool Blackout { get; set; }
        public List<PrizeInfo> Prizes { get; set; } 
    }

    public class PrizeInfo
    {
        public string Type { get; set; }
        public double Value { get; set; }
        public int Tier { get; set; }
    }
}
