using Milan.Common.SlotEngine.Models;
using Milan.Common.SlotEngine.Models.BetManager;

namespace GameBackend.Data
{
    public class LinesModelData
    {
        public string Id { get; set; }
        public LinesModelWinningComboData[] Lines { get; set; }
    }

    public class LinesModelWinningComboData
    {
        public string Name { get; set; }
        public LinesModelPatternData Pattern { get; set; }
        public LinesModelRewardData Reward { get; set; }
        public LinesModelStopData[] Stops { get; set; }
    }

    public class LinesModelPatternData
    {
        public LinesModelStopData[] Stops { get; set; }
        public int Type { get; set; }
        public int PayLineNumber { get; set; }
    }

    public class LinesModelStopData
    {
        public LinesModelCoordinateData Coordinate { get; set; }
        public SymbolData Symbol { get; set; }
    }

    public class LinesModelCoordinateData
    {
        public PositionData Position { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class LinesModelRewardData
    {
        public RewardItemData[] RewardItems { get; set; }
    }
}
