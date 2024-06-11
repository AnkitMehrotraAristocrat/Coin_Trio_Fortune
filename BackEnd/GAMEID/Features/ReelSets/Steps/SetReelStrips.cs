using GameBackend.Data;
using GameBackend.Features.ReelSets.Configuration;
using GameBackend.Features.ReelSets.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Models;
using System.Threading.Tasks;

namespace GameBackend.Features.ReelSets.Steps
{
    public class SetReelStrips : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            int betLevel = FeatureAccess.GetCurrentBetLevel(context);

            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/

            // Fetch all reel window name configured by state
            string[] reelWindows = context.GetCurrentStateReelWindowNames();
            string nextState = context.GetNextState();
            NextReelStripsWindowData reelStripData = context.FeaturePersistentData<ReelSetsPersistentData>().ReelStripsPerBetIndex.WindowData[nextState][reelWindows[0]];
            string[] reelStrip = reelStripData.NextReelStripsData[betLevel];

            ReelSet reelSetData = new();
            for (int index = 0; index < reelStrip.Length; index++) {
                ReelStrip strip = new(
                    context.XSlotConfigurations.ReelStripCollectionConfiguration.ReelStripsDefinition[reelStrip[index]].Stops,
                    context.MappedConfigurations.Symbols
                );
                reelSetData.AddReel(strip);
            }

            if (!context.ReelSetOperations.ReelSets.ContainsKey(reelWindows[0])) {
                context.ReelSetOperations.ReelSets.Add(reelWindows[0], reelSetData);
            }
            else {
                context.ReelSetOperations.ReelSets[reelWindows[0]] = reelSetData;
            }

            context.ReelSetOperations.CurrentReelSetName = reelWindows[0];
            context.CurrentReelWindowDefinitionName = context.GetCurrentStateReelWindowNames()[0];
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}
