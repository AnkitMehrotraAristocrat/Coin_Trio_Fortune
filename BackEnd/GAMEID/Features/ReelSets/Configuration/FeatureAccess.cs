using System;
using GameBackend.Data;
using GameBackend.Features.ReelSets.Data;
using GameBackend.Features.ReelSets.Steps;
using Milan.XSlotEngine.Core.Extensions;
using System.Collections.Generic;
using GameBackend.Helpers;

namespace GameBackend.Features.ReelSets.Configuration
{
    /// <summary>
    /// Used to provide access to other features
    /// </summary>
    public static class FeatureAccess
    {
        public static Dictionary<string, IList<string>> SetNextReelSetAndAddReelSetsPayload(GameContext gameContext)
        {
            new DetermineReelStripsForNextSpin().ExecuteAsync(gameContext);

            var payloads = new Dictionary<string, IList<string>>();
            var rsPersistentData = gameContext.FeaturePersistentData<ReelSetsPersistentData>();
            var currentState = gameContext.GetCurrentState();

            // Fetch all reel window name configured by state
            string[] reelWindows = gameContext.GetCurrentStateReelWindowNames();
            foreach (string windowName in reelWindows) {
                NextReelStripsWindowData reelStripData = rsPersistentData.ReelStripsPerBetIndex.WindowData[currentState][windowName];
                PayloadExtensions.AddPayload(payloads, Constants.ReelSetsPayloadName, reelStripData);
            }
            return payloads;
        }

        public static void UpdateReelStripsData(GameContext context, string stateName, string windowId, int currentBet, string[] nextReelStrips)
        {
            var rsPersistentData = context.FeaturePersistentData<ReelSetsPersistentData>().ReelStripsPerBetIndex.WindowData;
            if (!rsPersistentData.ContainsKey(stateName)) {
                rsPersistentData.Add(
                    stateName,
                    new Dictionary<string, NextReelStripsWindowData>()
                );
            }

            // Note :- Update data against single bet, game will have either bet level persistence or not. In both case only one bet will get updated.
            if (rsPersistentData[stateName].ContainsKey(windowId) && rsPersistentData[stateName][windowId].NextReelStripsData.ContainsKey(currentBet)) {
                rsPersistentData[stateName][windowId].NextReelStripsData[currentBet] = nextReelStrips;
            }
            else {
                var perBetIndexEnabledPreferenceValue = GeneralHelper.GetPreferenceBool(Constants.PerBetIndexEnabledPreferenceKey);
                var nextReelStripDataPerBetIndex = new Dictionary<int, string[]> {
                    { currentBet, nextReelStrips }
                };
                rsPersistentData[stateName].Add(
                    windowId,
                    new NextReelStripsWindowData(windowId, perBetIndexEnabledPreferenceValue, nextReelStripDataPerBetIndex)
                );
            }
        }

        public static string[] GetReelStripsData(GameContext context, string stateName, string windowId, int currentBet)
        {
            var rsPersistentData = context.FeaturePersistentData<ReelSetsPersistentData>().ReelStripsPerBetIndex.WindowData;
            if (rsPersistentData.ContainsKey(stateName) && rsPersistentData[stateName].ContainsKey(windowId)) {
                if (rsPersistentData[stateName][windowId].NextReelStripsData.ContainsKey(currentBet)) {
                    return rsPersistentData[stateName][windowId].NextReelStripsData[currentBet];
                }
            }
            return Array.Empty<string>();
        }

        public static int GetCurrentBetLevel(GameContext context)
        {
            var betLevel = GameConstants.DefaultBetIndex;
            var perBetIndexEnabledPreferenceValue = GeneralHelper.GetPreferenceBool(Constants.PerBetIndexEnabledPreferenceKey);
            if (perBetIndexEnabledPreferenceValue) {
                betLevel = context.BetOperations.MultiplierIndex;
            }
            return betLevel;
        }
    }
}
