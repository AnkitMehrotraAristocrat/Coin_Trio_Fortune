using System.Collections.Generic;

namespace GameBackend.Data
{
    public class GameRoundData : Milan.XSlotEngine.Core.GameData.RoundData
    {
        public Dictionary<string, object> FeatureRoundData { get; set; }
        public ulong TotalWin { get; set; }
        public int BetIndex { get; set; }
        public int LineIndex { get; set; }

        public bool isDragonFeatureTriggered { get; set; } // Red
        public bool isTigerFeatureTriggered { get; set; } // Green
        public bool isKoiFeatureTriggered { get; set; } // Blue

        public GameRoundData() : base()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            FeatureRoundData = new Dictionary<string, object>();
            TotalWin = 0;
            BetIndex = 0;
            LineIndex = 0;
        }

        public T GetFeatureData<T>() where T : new()
        {
            var type = typeof(T).ToString();
            var data = FeatureRoundData;
            if (!data.ContainsKey(type)) {
                data.Add(type, new T());
            }
            return (T)data[type];
        }
    }
}