using System.Collections.Generic;

namespace GameBackend.Features.ReelSets.Configuration
{
    public static class Constants
    {
        public const int RowOffset = 0;
        public const string ReelSetsPayloadName = "NextReelStrips";
        public const string ReelWindowsPayloadName = "reelWindows";
        public const string PerBetIndexEnabledPreferenceKey = "ReelStripsPerBetIndexEnabled";

        // If weight table for specific GameStates is not defined then it will fetch information from GameConstants.DefaultReelSetsPerWindowId
        public static readonly Dictionary<string, string> ReelSetsWeightTable = new () {
            { "BaseSpin" , "BaseReelSets" }
        };
    }
}
