using Milan.Common.Interfaces.Entities;
using System.Collections.Generic;

namespace GameBackend.Data
{
    public class PositionMapsData : IConfiguration
    {
        public List<PositionMapsPayload> Data { get; set; } = new();

        public object Clone() => Duplicate();
        public IConfiguration Duplicate()
        {
            return new PositionMapsData() {
                Data = Data
            };
        }
    }

    public class PositionMapsPayload
    {
        public int FromHeight { get; set; }
        public int ToHeight { get; set; }
        public string[] ReelWindows { get; set; }
        public List<Dictionary<string, PositionMapCellData>> PositionMaps { get; set; }
        public List<Dictionary<string, PositionMapCellData>> ExcludedPositions { get; set; }
    }

    public class PositionMapCellData
    {
        public int ReelIndex { get; set; }
        public int SymbolIndex { get; set; }

        public PositionMapCellData(int reelIndex, int symbolIndex)
        {
            ReelIndex = reelIndex;
            SymbolIndex = symbolIndex;
        }
    }
}
