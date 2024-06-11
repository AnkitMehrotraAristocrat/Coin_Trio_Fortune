using Milan.Common.SlotEngine.Models;
using Milan.Common.SlotEngine.Models.BetManager;
using Milan.XSlotEngine.Core.Contexts;
using Milan.XSlotEngine.Interfaces.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using GameBackend.Helpers;
using Milan.XSlotEngine.Interfaces.Core.BetManager;
using Milan.XSlotEngine.Core.Models.WeightTables;

namespace GameBackend.Data
{
    /// <summary>
    /// Get feature specific context via context FeatureContext<type>()
    /// Get feature specific round data via context FeatureRoundData<type>()
    /// Get feature specific persistent data via context FeaturePersistentData<type>()
    /// For state specific spin count use context GetRemainingFreeSpins(), SetRemainingFreeSpins(), HasFreeSpinsAnyState()
    /// </summary>
    public class GameContext : GameBaseContext
    {
        public string UserId { get; set; }
        private Dictionary<string, object> FeatureContexts { get; set; }
        private CustomReelWindow CurrentReelWindow { get; set; }
        public bool[] HiddenWindowCells { get; set; }
        private SymbolData FillerSymbol { get; set; }
        public TransitionData Transition { get; set; }
        public Dictionary<string, IList<string>> ConfigPayload { get; set; }
        public Dictionary<string, IList<string>> JoinPayload { get; set; }
        public List<WonAwardViaFeature> WonAwardsViaFeatures { get; set; }
        public Configurations CustomConfigurations { get; set; }
        public Guid SpinGuid { get; set; } //set on each spin when the context is created
        public CellSwaps WindowCellSwaps { get; set; }
        public bool hasDragonLanded { get; set; }
        public bool hasTigerLanded { get; set; }
        public bool hasKoiLanded { get; set; }
        public WeightTableDefinition WeightTableDefinition { get; set; }
        public GameContext(string currentStateName)
        {
            FeatureContexts = new Dictionary<string, object>();
            HiddenWindowCells = new bool[GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight];
            Transition = new TransitionData(currentStateName, currentStateName);
            ConfigPayload = new Dictionary<string, IList<string>>();
            JoinPayload = new Dictionary<string, IList<string>>();
            WonAwardsViaFeatures = new List<WonAwardViaFeature>();
            CustomConfigurations = null;
            WindowCellSwaps = new();
        }

        public new GamePersistentData PersistentData
        {
            get => base.PersistentData as GamePersistentData;
            set => base.PersistentData = value;
        }

        public new GameRoundData RoundData
        {
            get => base.RoundData as GameRoundData;
            set => base.RoundData = value;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Data getters for features

        public T FeatureContext<T>() where T : new()
        {
            var type = typeof(T).ToString();
            if (!FeatureContexts.ContainsKey(type)) {
                FeatureContexts.Add(type, new T());
            }
            return (T)FeatureContexts[type];
        }

        public T FeatureRoundData<T>() where T : new()
        {
            return RoundData.GetFeatureData<T>();
        }

        public T FeaturePersistentData<T>() where T : new()
        {
            return PersistentData.GetFeatureData<T>();
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Free spin counter methods 

        public void SetRemainingFreeSpins(int count, string state = null, bool updateRoundData = true)
        {
            // state null means current state
            var counts = PersistentData.FreeSpinsCount;
            state ??= GetCurrentState();
            if (!counts.ContainsKey(state)) {
                counts.Add(state, count);
            }
            if (updateRoundData) {
                RoundData.RemainingFreeSpins = count;
            }
            counts[state] = count;
        }

        public int GetRemainingFreeSpins(string state = null, bool updateRoundData = true)
        {
            // state null means current state
            var counts = PersistentData.FreeSpinsCount;
            state ??= GetCurrentState();
            if (!counts.ContainsKey(state)) {
                counts.Add(state, 0);
            }
            if (updateRoundData) {
                RoundData.RemainingFreeSpins = counts[state];
            }
            return counts[state];
        }

        public bool HasFreeSpinsAnyState()
        {
            var counts = PersistentData.FreeSpinsCount;
            foreach (var count in counts) {
                if (count.Value > 0) {
                    return true;
                }
            }
            return false;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// General context-dependant helper method

        public string GetBetCurrencyType()
        {
            return XSlotConfigurations.BetConfiguration.DefaultCurrency;
        }

        public void MetricAddOrUpdate(string label, long value)
        {
            MetricOperations.CustomMetrics.AddOrUpdate(label, value, (key, oldValue) => oldValue + value);
        }

        public ulong[] GetBetAmounts()
        {
            string currency = XSlotConfigurations.BetConfiguration.DefaultCurrency;
            IBetItem wagerCurrency = MappedConfigurations.BetItems.First(w => w.CurrencyType == currency);
            ulong[] wagerMultipliers = wagerCurrency.MultiplierIndexes.Multipliers.ToArray();

            #if WAYS_GAME 
            var baseCost = wagerCurrency.ReelStripCostIndexes.GetTotalReelStripCost();
            #else //LINES_GAME
            ulong baseCost = wagerCurrency.BetLineIndexes.BetLines[this.RoundData.LineIndex].TotalLineCost;
            #endif

            List<ulong> wagerAmounts = new();
            foreach (ulong wagerMultiplier in wagerMultipliers) {
                wagerAmounts.Add(wagerMultiplier * baseCost);
            }
            return wagerAmounts.ToArray();
        }

        public void AddFeatureWonAward(ulong winAmount, string winName)
        {
            WonAwardsViaFeatures.Add(new WonAwardViaFeature {
                Name = winName,
                Value = winAmount
            });

            var currency = GetBetCurrencyType();
            var existingRoundReward = SpinData.Results.WonRewards.FirstOrDefault(r => string.Equals(r.CurrencyType, currency, StringComparison.OrdinalIgnoreCase));
            if (existingRoundReward != null) {
                existingRoundReward.TotalWon += winAmount;
                existingRoundReward.Quantity += 1;
            }
            else {
                SpinData.Results.WonRewards.Add(new RewardItemData {
                    TotalWon = winAmount,
                    CurrencyType = currency,
                    Quantity = 1
                });
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Window methods (shared world window)

        public void InitilizeHiddenCells()
        {
            int[] widthHeight = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(GetCurrentState())];
            SetReelWindowMaxPadded(widthHeight[0], widthHeight[1]);
        }

        public void SetReelWindow(IReelWindow reelWindow)
        {
            int[] widthHeight = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(GetCurrentState())];
            InitilizeHiddenCells();
            // Add existing stops
            int index = CurrentReelWindow.StopsContent.Count;
            for (int y = 0; y < widthHeight[1]; y++) {
                for (int x = 0; x < widthHeight[0]; x++) {
                    var sym = reelWindow.GetSymbolByPosition(y, x);
                    CurrentReelWindow.StopsContent.Add(new FlexibleStop() {
                        StopSymbol = sym,
                        WorldIndex = index
                    });
                    index++;
                }
            }
        }

        private void SetReelWindowMaxPadded(int regWindowWidth, int regWindowHeight)
        {
            // Backend will maintain a window based on max size
            var maxWindow = new CustomReelWindow {
                WindowSize = new Size() {
                    Width = GameConstants.WindowMaxWidth,
                    Height = GameConstants.WindowMaxHeight
                }
            };
            // Add missing stops
            int count = (GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight) - (regWindowWidth * regWindowHeight);
            if (count > 0) {
                // Initialize symbol to be used for hidden cells
                FillerSymbol ??= new SymbolData() {
                    Id = MappedConfigurations.Symbols.First(s => s.Name == GameConstants.FillerSymbol).Id,
                    Name = GameConstants.FillerSymbol,
                    IsScatter = false
                };

                for (int i = 0; i < count; i++) {
                    HiddenWindowCells[i] = true;
                    maxWindow.StopsContent.Add(new FlexibleStop() {
                        StopSymbol = FillerSymbol,
                        WorldIndex = i
                    });
                }
            }
            CurrentReelWindow = maxWindow;
        }

        public string[] GetCurrentStateReelWindowNames()
        {
            return GameConstants.StateReelWindows[GeneralHelper.GetGameStateEnum(GetCurrentState())];
        }

        public CustomReelWindow GetCurrentReelWindow()
        {
            return CurrentReelWindow;
        }

        public int GetCurrentReelWindowCurrentHeight()
        {
            int windowCurrentHeight = 0;
            for (int c = 0; c < GameConstants.WindowMaxWidth; c++) {
                windowCurrentHeight = Math.Max(windowCurrentHeight, GetCurrentWindowCurrentColumnHeight(c));
            }
            return windowCurrentHeight;
        }

        public int GetCurrentWindowCurrentColumnHeight(int column)
        {
            int height = GameConstants.WindowMaxHeight;
            for (int r = 0; r < GameConstants.WindowMaxHeight; r++) {
                if (HiddenWindowCells[GeneralHelper.GetWorldIndexByPosition(column, r)]) {
                    height--;
                }
            }
            return height;
        }

        public int GetCurrentReelWindowCurrentWidth()
        {
            int windowCurrentWidth = 0;
            for (int r = 0; r < GameConstants.WindowMaxHeight; r++) {
                windowCurrentWidth = Math.Max(windowCurrentWidth, GetCurrentWindowCurrentRowWidth(r));
            }
            return windowCurrentWidth;
        }

        public int GetCurrentWindowCurrentRowWidth(int row)
        {
            int width = GameConstants.WindowMaxWidth;
            for (int c = 0; c < GameConstants.WindowMaxWidth; c++) {
                if (HiddenWindowCells[GeneralHelper.GetWorldIndexByPosition(c, row)]) {
                    width--;
                }
            }
            return width;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// State methods

        public string GetNextState()
        {
            // Case when the game is loaded for the very first time
            // Queue is created in step CreateStatePayloads
            if (PersistentData.TriggeredStates.Queue.Count() == 0) {
                return GeneralHelper.GetGameStateString(GameStates.BaseSpin);
            }
            return PersistentData.TriggeredStates.Queue.Peek().State;
        }

        public string GetCurrentState()
        {
            return PersistentData.CurrentState;
        }

        public void SetCurrentState(string state)
        {
            PersistentData.CurrentState = state;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        /// Position interpretation methods

        public void UpdateWindowCellSwaps(string fromState, int fromHeight, int fromWidth, string toState, int toHeight, int toWidth)
        {
            WindowCellSwaps.Swaps.Clear();
            if(fromState == toState) {
                return;
            }
            if (!GeneralHelper.ConfigExist(GameConstants.ConfigPositionMap)) {
                return;
            }

            var fromReelWindow = GameConstants.StateReelWindows[GeneralHelper.GetGameStateEnum(fromState)][0];
            var toReelWindow = GameConstants.StateReelWindows[GeneralHelper.GetGameStateEnum(toState)][0];
            var data = CustomConfigurations.ClientPositionMaps.Find(rec => rec.FromHeight == fromHeight && rec.ToHeight == toHeight && rec.ReelWindows[0] == fromReelWindow && rec.ReelWindows[1] == toReelWindow);
            if (data == null) {
                return;
            }

            foreach (var rec in data.PositionMaps) {
                int toWorldIndex;
                if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(toState)]) {
                    toWorldIndex = GeneralHelper.GetWorldIndexByClientIndex(rec[toReelWindow].ReelIndex, toHeight, toWidth);
                }
                else {
                    toWorldIndex = GeneralHelper.GetWorldIndexByClientPosition(rec[toReelWindow].ReelIndex, rec[toReelWindow].SymbolIndex, toHeight);
                }

                int fromWorldIndex;
                if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(fromState)]) {
                    fromWorldIndex = GeneralHelper.GetWorldIndexByClientIndex(rec[fromReelWindow].ReelIndex, fromHeight, fromWidth);
                }
                else {
                    fromWorldIndex = GeneralHelper.GetWorldIndexByClientPosition(rec[fromReelWindow].ReelIndex, rec[fromReelWindow].SymbolIndex, fromHeight);
                }
                
                // swaps are only for non-standard moves (meaning the visual location of a cell's contents will change)
                if (fromWorldIndex != toWorldIndex) {
                    // Doesn't add duplictes (e.g. in the map a from/to may also exist in reverse)
                    WindowCellSwaps.AddSwap(fromWorldIndex, toWorldIndex);
                }
            }
        }
    }
}
