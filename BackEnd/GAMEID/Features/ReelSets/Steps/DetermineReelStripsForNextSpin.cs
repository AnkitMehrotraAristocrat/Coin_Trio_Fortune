using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.ReelSets.Data;
using GameBackend.Features.ReelSets.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.ReelSets.Steps
{
    /// <summary>
    /// Execution step which determines next reel set for current bet index
    /// </summary>
    public class DetermineReelStripsForNextSpin : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            string nextState = context.GetNextState();

            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/

            // Fetch all reek window name configured by state
            string[] reelWindows = GameConstants.StateReelWindows[GeneralHelper.GetGameStateEnum(nextState)];

            string nextReelSet;
            // If weight table defined for game state then consume same else fetch Value from GameConstants.DefaultReelSetsPerWindowId
            if (Constants.ReelSetsWeightTable.ContainsKey(nextState)) {
                nextReelSet = SelectReelSet(context);
            }
            else {
                nextReelSet = GameConstants.DefaultReelSetsPerWindowId[reelWindows[0]];
            }

            int betLevel = FeatureAccess.GetCurrentBetLevel(context);
            // save reel strip data against each reel window
            for (int index = 0; index < reelWindows.Length; index++) {
                string[] reelStrips = CreateReelStripData(context, nextReelSet);
                // Create instance ReelStripsPerBetIndex by state
                var windowData = context.FeaturePersistentData<ReelSetsPersistentData>().ReelStripsPerBetIndex.WindowData;
                if (!windowData.ContainsKey(nextState)) {
                    windowData.Add(
                        nextState,
                        new Dictionary<string, NextReelStripsWindowData>()
                    );
                }
                windowData[nextState][reelWindows[index]].NextReelStripsData[betLevel] = reelStrips;
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            GeneralHelper.StepExceptionOnNull(this, context.SpinData.Results.WinnerCombinations, nameof(context.SpinData.Results.WinnerCombinations));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        protected static string[] CreateReelStripData(GameContext context, string nextReelSet)
        {
            var reelSetConfig = context.XSlotConfigurations.ReelSetsCollectionConfiguration.ReelSets.FirstOrDefault(reelSetEntry => reelSetEntry.Name.Equals(nextReelSet));
            return reelSetConfig.Reels;
        }

        protected static string SelectReelSet(GameContext context)
        {
            context.PersistentData.GaffeQueues.ConsumeCategoryQueue(GaffeCategories.SelectReelSet, context.PersistentData.RandomNumberQueue);
            string nextState = context.GetNextState();
            return GeneralHelper.GetRandomEntryFromTable<string>(context, Constants.ReelSetsWeightTable[nextState]);
        }
    }
}