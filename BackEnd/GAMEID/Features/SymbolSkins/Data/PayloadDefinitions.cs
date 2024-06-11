using Milan.Common.SlotEngine.Models;
using System.Collections.Generic;

namespace GameBackend.Features.SymbolSkins.Data
{
    public class OutcomePayload
    {
        public string Id { get; set; }
        public List<OutcomePayloadData> SymbolOutcomeData { get; set; } = new();
    }

    public class OutcomePayloadData
    {
        public bool CanAward { get; set; }
        public int Tier { get; set; }
        public SkinData SymbolData { get; set; }
        public PositionData PositionData { get; set; }
    }

    public class SpinningPayloadData
    {
        public string Id { get; set; }
        public List<SpinningIdData> Data { get; set; } = new();
    }
}
