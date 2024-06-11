using System.Collections.Generic;

namespace GameBackend
{
    public static class GameConstants
    {
        public const string GameId = "GAMEID";
        public const string Provider = "Aristocrat";
        public const string CreditType = "credit";
        public const string PayoutCreditType = "Credit";
        public const int DefaultBetIndex = 0;
        public const int DefaultBetLineIndex = 0;

        // LRTB is the default formation, can be altered in Configuration/Preferences
        // Backend uses LRTB only, and translates for payloads if client prefers TBLR
        public const string ClientPositionFormationPreferenceKey = "ClientPositionFormationValue";
        public const string ClientPositionFormationTBLR = "TBLR";

        // ReelGrowth support, backend maintains world-coordinates in a full size window
        // Specify max size, and symbol to be used for hidden cells
        // VisualWindowWidthHeight widths and heights must be accurate to full visual window (since ReelWindow.json may not be)
        // FillerSymbol must exist in Symbols.json
        public const string FillerSymbol = "BLANK";
        public const int WindowMaxWidth = 5;
        public const int WindowMaxHeight = 3; //Height 3, Change to 6 to run CoreUnitTests/TestPositionInterpreters 

        // Sizes in ReelWindow.json may not match what is shown on the client (HoldAndSpin for example)
        // Should not exceed [WindowMaxWidth, WindowMaxHeight]
        public static readonly Dictionary<GameStates, int[]> VisualWindowWidthHeight = new() {
            { GameStates.BaseSpin, new[] { 5, 3 } },
            { GameStates.HoldAndSpin, new[] { 5, 3 } }
        };

        // Can be used to determine states that have a reel per cell
        public static readonly Dictionary<GameStates, bool> SingleCellReels = new () {
            { GameStates.BaseSpin, false },
            { GameStates.HoldAndSpin, true }
        };

        // All windows associated with each state 
        public static readonly Dictionary<GameStates, string[]> StateReelWindows = new() {
            { GameStates.BaseSpin, new[] { "Main" } },
            { GameStates.HoldAndSpin, new[] { "HoldAndSpin" } }
        };

        // Each windowId's default reelset configuration
        public static readonly Dictionary<string, string> DefaultReelSetsPerWindowId = new() {
            { StateReelWindows[GameStates.BaseSpin][0], "BaseReels_PIC1" },
            { StateReelWindows[GameStates.HoldAndSpin][0], "HoldAndSpin" }
        };

        // Jackpot constants 
        public const string JackpotID = "main";
        public static readonly ulong[] JackpotBaseValues = { 200, 20, 10, 5 };
        public static readonly Dictionary<string, int> JackpotTiers = new() {
            { "Grand", 0 }, { "Major", 1 }, { "Minor", 2 }, { "Mini", 3 }
        };

        // Prize types, shared data 
        public const string MultiplierPrizeType = "Multiplier";
        public const string JackpotPrizeType = "Jackpot";

        // Files
        public const string ConfigPositionMap = "PositionMap.json";
        public const string ConfigStatePriority = "StatePriority.json";
        public const string ConfigJackpots = "Jackpots.json";
        public const string ConfigReelSets = "ReelSet.json";
        public const string ConfigPreferences = "Preferences.json";

        // Names and labels
        public const string IdUnkown = "UNKNOWN";
        public const string JoinStateName = "Join";
        public const string VariationFieldName = "variation";
        public const string DynamicBetAmountFieldName = "DynamicBetAmount";
        public const string UsedDynamicBetFormulaFieldName = "UsedDynamicBetFormula";

        public const string BetStatePayloadName = "betState";
        public const string GaffesPayloadName = "gaffes";
        public const string JoinPayloadName = "join";
        public const string MetricsPayloadName = "metricsPayload";
        public const string UserIdPayloadName = "userId";
        public const string TableIdPayloadName = "tableId";
        public const string JackpotApplicationIdPayloadName = "jackpotApplicationId";
        public const string JackpotEngineUrlPayloadName = "jackpotEngineUrl";
        public const string JackpotMappingsPayloadName = "jackpotMappings";
        public const string PrespinPayloadName = "prespin";
        public const string JackpotWinsPayloadName = "jackpotWins";
        public const string JackpotResetsPayloadName = "jackpotResets";
        public const string ReelStripModelPayloadName = "ReelStripData";
        public const string FixedReelSetModelPayloadName = "FixedReelSetData";
        public const string WinningCombinationsPayloadName = "winningCombinations";
        public const string WaysModelPayloadName = "WaysModel";
        public const string LinesModelPayloadName = "LinesModel";
        public const string PayLinesPayloadName = "PayLines";
        public const string ReelWindowModelPayloadName = "ReelWindowData";
        public const string ReelsOutcomeModelPayloadName = "ReelsOutcomeModel";
        public const string SpinResponsePayloadName = "spinResponse";
        public const string MilanBetLevelsPayloadName = "milanBetLevels";
        public const string ScatterModelPayloadName = "ScatterModel";
        public const string PositionMapsPayloadName = "PositionMaps";
        public const string SpinGaffePayloadName = "SpinGaffe";
		public const string JackpotInitValuePayloadName = "JackpotInitValue";
		public const string SelectReelSetPreferenceKey = "ProcSelectReelSetOnGaffeReqForNonCat";

        public const string Red_SCAT_Trigger_Table = "Red_SCAT_Trigger_Table";
        public const string Green_SCAT_Trigger_Table = "Green_SCAT_Trigger_Table";
        public const string Blue_SCAT_Trigger_Table = "Blue_SCAT_Trigger_Table";

        public const string Red_Green_SCAT_Trigger_Table = "Red_Green_SCAT_Trigger_Table";
        public const string Red_Blue_SCAT_Trigger_Table = "Red_Blue_SCAT_Trigger_Table";
        public const string Green_Blue_SCAT_Trigger_Table = "Green_Blue_SCAT_Trigger_Table";
        public const string Red_Green_Blue_SCAT_Trigger_Table = "Red_Green_Blue_SCAT_Trigger_Table";

        // Metamorphic Payload
        public const string MetamorphicFeaturePayloadName = "MetamorphicFeaturePayload";

        // Messages and errors 
        public const string ErrorCreateStateMachineFormat = "An error occured while setting up the state machine: {0}";
        public const string ErrorCreateGameContextFormat = "An error occurred while creating the game context: {0}";
        public const string ErrorGaffeIndex = "Specified gaffe index is out of range";
        public const string ErrorGaffeNameFormat = "Specified gaffe {0} is unknown";
        public const string ErrorStateQueuePeek = "Queue empty";
        public const string ErrorStateQueueDequeue = "Can't Dequeue. Must have one valid state.";
        public const string ErrorDynamicBetLevelFormat = "[DynamicBetLevel] A player rejoined a machine mid-feature. Ignoring DynamicBetAmount of {0}.";
    }
}
