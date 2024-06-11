using Milan.Common.SlotEngine.Models;
using System.Collections.Generic;
using Milan.Common.SlotEngine.Models.BetManager;

namespace GameBackend.Data
{
    public class ComboLinesRecoveryData
    {
        public string Id { get; set; }
        public List<ComboLinesData> Lines { get; set; }

        public ComboLinesRecoveryData()
        {
            Id = GameConstants.IdUnkown;
            Lines = new List<ComboLinesData>();
        }
    }

    public class ComboScatterRecoveryData
    {
        public string Id { get; set; }
        public List<ComboScatterData> Combinations { get; set; }

        public ComboScatterRecoveryData()
        {
            Id = GameConstants.IdUnkown;
            Combinations = new List<ComboScatterData>();
        }
    }

    public class ComboScatterData
    {
        public ComboScatterData() { }

        public ComboScatterData(string name, IList<StopData> stops, int paylineIndex)
        {
            var stopsList = new List<ComboStops>();
            foreach (var stop in stops) { 
                stopsList.Add(new ComboStops(stop)); 
            }
            Name = name;
            Stops = stopsList;
            PaylineIndex = paylineIndex;
        }

        public string Name { get; set; }
        public IList<ComboStops> Stops { get; set; }
        public int PaylineIndex { get; set; }
    }

    public class ComboLinesData
    {
        public ComboLinesData() { }

        public ComboLinesData(string name, IList<StopData> stops, int paylineIndex, ComboPattern pattern, ComboReward reward)
        {
            var stopsList = new List<ComboStops>();
            foreach (var stop in stops) { 
                stopsList.Add(new ComboStops(stop)); 
            }
            Name = name;
            Stops = stopsList;
            PaylineIndex = paylineIndex;
            Pattern = pattern;
            Reward = reward;
        }

        public string Name { get; set; }
        public List<ComboStops> Stops { get; set; }
        public ComboPattern Pattern { get; set; }
        public ComboReward Reward { get; set; }
        public int PaylineIndex { get; set; }
    }

    public class ComboStops
    {
        public ComboStops() { }

        public ComboStops(StopData other)
        {
            Position = other.Coordinate.Position;
            SymbolId = other.Symbol.Id;
        }

        public PositionData Position { get; set; }
        public int SymbolId { get; set; }
    }

    public class ComboStop
    {
        public ComboCoordinate Coordinate { get; set; }
        public SymbolData Symbol { get; set; }

        public ComboStop() { }
    }

    public class ComboCoordinate
    {
        public PositionData Position { get; set; }
        public int X => Position.X;
        public int Y => Position.Y;

        public ComboCoordinate() {}
    }

    public class ComboPattern
    {
        public ComboPattern() { }

        public int Id { get; set; }
        public List<ComboStop> Stops { get; set; }
        public EnumData.PatternType Type { get; set; }
        public int PayLineNumber { get; set; }
    }

    public class ComboReward
    {
        public ComboReward() { }
        public List<RewardItemData> RewardItems { get; set; }
    }

    public class ComboWaysRecoveryData
    {
        public string Id { get; set; }
        public ComboWaysData HitResult { get; set; }

        public ComboWaysRecoveryData()
        {
            Id = GameConstants.IdUnkown;
            HitResult = new ComboWaysData();
        }
    }

    public class ComboWaysData
    {
        public List<ComboWaysWinningData> Hits { get; set; }

        public ComboWaysData()
        {
            Hits = new List<ComboWaysWinningData>();
        }

        public ComboWaysData(List<ComboWaysWinningData> hits)
        {
            Hits = hits;
        }
    }

    public class ComboWaysWinningData
    {
        public List<ComboCoordinate> Coordinates { get; set; }
        public ComboReward TotalReward { get; set; }

        public ComboWaysWinningData() { }
    }
}
