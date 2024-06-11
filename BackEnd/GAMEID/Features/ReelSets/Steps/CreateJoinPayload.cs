using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using GameBackend.Features.ReelSets.Data;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.ReelSets.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.ReelSets.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var rsPersistentData = context.FeaturePersistentData<ReelSetsPersistentData>();

            string nextState = context.GetNextState();
            if (rsPersistentData.ReelStripsPerBetIndex.GetCount() == 0) {
                SetNextReelStripOnFirstJoin(context);
            }

            // Fetch all reel window name configured by state
            string[] reelWindows = context.GetCurrentStateReelWindowNames();
            foreach (string windowName in reelWindows) {
                var response = new PayloadData(
                    rsPersistentData.ReelStripsPerBetIndex.WindowData[nextState][windowName]
                );
                context.JoinPayload.AddPayload(Constants.ReelSetsPayloadName, response);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.MappedConfigurations, nameof(context.MappedConfigurations));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static void SetNextReelStripOnFirstJoin(GameContext context)
        {
            var windowData = context.FeaturePersistentData<ReelSetsPersistentData>().ReelStripsPerBetIndex.WindowData;
            var perBetIndexEnabledPreferenceValue = GeneralHelper.GetPreferenceBool(Constants.PerBetIndexEnabledPreferenceKey);
            foreach (var kvp in GameConstants.StateReelWindows) {
                string state = GeneralHelper.GetGameStateString(kvp.Key);
                windowData.Add(
                    state,
                    new Dictionary<string, NextReelStripsWindowData>()
                );
                foreach (string reelWindowId in kvp.Value) {
                    string reelSet = GameConstants.DefaultReelSetsPerWindowId[reelWindowId];
                    var reelSetConfig = context.XSlotConfigurations.ReelSetsCollectionConfiguration.ReelSets.FirstOrDefault(reelSetEntry => reelSetEntry.Name.Equals(reelSet));
                    var nextReelStripDataPerBetIndex = new Dictionary<int, string[]>();
                    if (perBetIndexEnabledPreferenceValue) {
                        string defaultCurrency = context.XSlotConfigurations.BetConfiguration.DefaultCurrency;
                        int multipliersCount = context.XSlotConfigurations.BetConfiguration.CurrencyBets.ToList().Find(x => x.CurrencyType == defaultCurrency).Multipliers.Count;
                        for (int betIndex = 0; betIndex < multipliersCount; betIndex++) {
                            // Make sure the SingleCellReels have reelstrips mapped to each position (add extra padding)
                            // If HnS feature exists, this data is updated in GameBackend.Features.HoldAndSpin.Steps.DetermineReelStripsForNextSpin
                            if (GameConstants.SingleCellReels.ContainsKey(GeneralHelper.GetGameStateEnum(state)) && GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(state)]) {
                                int[] windowDimensions = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(state)];
                                var cellsCount = windowDimensions[0] * windowDimensions[1];
                                List<string> reelSets = reelSetConfig.Reels.ToList();
                                while (reelSets.Count < cellsCount) {
                                    reelSets.Add(reelSetConfig.Reels[0]);
                                }
                                nextReelStripDataPerBetIndex.Add(betIndex, reelSets.ToArray());
                            }
                            else {
                                nextReelStripDataPerBetIndex.Add(betIndex, reelSetConfig.Reels);
                            }
                        }
                    }
                    else {
                        // Make sure the SingleCellReels have reelstrips mapped to each position (add extra padding)
                        // this data is updated in GameBackend.Features.HoldAndSpin.Steps.DetermineReelStripsForNextSpin
                        if (GameConstants.SingleCellReels.ContainsKey(GeneralHelper.GetGameStateEnum(state)) && GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(state)]) {
                            int[] windowDimentions = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(state)];
                            var cellsCount = windowDimentions[0] * windowDimentions[1];
                            List<string> reelSets = reelSetConfig.Reels.ToList();
                            while (reelSets.Count < cellsCount) {
                                reelSets.Add(reelSetConfig.Reels[0]);
                            }
                            nextReelStripDataPerBetIndex.Add(GameConstants.DefaultBetIndex, reelSets.ToArray());
                        }
                        else {
                            nextReelStripDataPerBetIndex.Add(GameConstants.DefaultBetIndex, reelSetConfig.Reels);
                        }
                    }
                    windowData[state][reelWindowId] = new NextReelStripsWindowData(reelWindowId, perBetIndexEnabledPreferenceValue, nextReelStripDataPerBetIndex);
                }
            }
        }
    }
}
