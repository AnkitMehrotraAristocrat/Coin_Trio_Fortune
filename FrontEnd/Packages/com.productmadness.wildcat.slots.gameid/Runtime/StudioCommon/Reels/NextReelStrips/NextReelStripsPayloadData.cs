using Milan.Shared.DTO.FrontEnd;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips
{
    public class NextReelStripsPayloadData : IHasId
    {
        public string Id { get; set; }
        public bool PerBetIndexEnabled { get; set; }
        public Dictionary<int, string[]> NextReelStripsData { get; set; }
    }
}
