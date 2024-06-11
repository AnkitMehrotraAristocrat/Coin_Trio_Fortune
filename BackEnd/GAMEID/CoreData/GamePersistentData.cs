using System.Collections.Generic;
using GameBackend.Helpers;

namespace GameBackend.Data
{
    public class GamePersistentData : Milan.XSlotEngine.Core.GameData.PersistentData
    {
        public Dictionary<string, object> FeaturePersistentData { get; set; }
        public Dictionary<string, int> FreeSpinsCount { get; set; }
        public List<ComboLinesRecoveryData> LinesComboRecovery { get; set; }
        public List<ComboWaysRecoveryData> WaysComboRecovery { get; set; }
        public List<ComboScatterRecoveryData> ScatterComboRecovery { get; set; }
        public GaffeQueues GaffeQueues { get; set; }
        public TriggeredStateInfo TriggeredStates { get; set; }
        public Dictionary<string, ReelOutcome> ReelOutcomeData { get; set; }
        public LinesModelData LinesModelPayload { get; set; }
        public string RtpFeatureName { get; set; }
        public int BaseBetIndex { get; set; }
        public string PreviousState { get; set; }

        public bool hasDragonLanded { get; set; }
        public bool hasTigerLanded { get; set; }
        public bool hasKoiLanded {  get; set; }

        public GamePersistentData()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            FeaturePersistentData = new Dictionary<string, object>();
            FreeSpinsCount = new Dictionary<string, int>();
            LinesComboRecovery = new List<ComboLinesRecoveryData>();
            ScatterComboRecovery = new List<ComboScatterRecoveryData>();
            WaysComboRecovery = new List<ComboWaysRecoveryData>();
            GaffeQueues = new GaffeQueues();
            TriggeredStates = new TriggeredStateInfo();
            ReelOutcomeData = new Dictionary<string, ReelOutcome>();
            LinesModelPayload = new LinesModelData();
            RtpFeatureName = string.Empty;
            BaseBetIndex = 0;
            PreviousState = GeneralHelper.GetGameStateString(GameStates.BaseSpin);
            CurrentState = PreviousState; //base class
        }

        public T GetFeatureData<T>() where T : new()
        {
            var type = typeof(T).ToString();
            var data = FeaturePersistentData;
            if (!data.ContainsKey(type)) {
                data.Add(type, new T());
            }
            return (T)data[type];
        }
    }
}