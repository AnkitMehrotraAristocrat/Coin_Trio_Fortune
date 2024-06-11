using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using System.Linq;
using GameBackend.Helpers;
using GameBackend.Features.ReelSets.Configuration;
using GameBackend.Features.ReelSets.Data;

namespace GameBackend.Features.ReelSets.Steps
{
    public class SaveReelsOutcomeData : BaseStep<GameContext>
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
            var rsPersistentData = context.FeaturePersistentData<ReelSetsPersistentData>();
            var reelWindows = context.GetCurrentStateReelWindowNames();
            var currentState = context.GetCurrentState();
            var reelStripData = rsPersistentData.ReelStripsPerBetIndex.WindowData[currentState][reelWindows[0]];
            var reelStrip = reelStripData.NextReelStripsData[betLevel];

            var outcomeData = context.PersistentData.ReelOutcomeData;
            var reels = context.ReelSetOperations.CurrentReelSet.Reels;
            var rowOffset = Constants.RowOffset;
            var currentStops = (from i in reels select (
                i.CurrentIndex - rowOffset >= 0
                ? i.CurrentIndex - rowOffset
                : i.MaxPosition - (i.CurrentIndex - rowOffset)
            )).ToArray();

            outcomeData[currentState].ReelWindowId = reelWindows[0];
            outcomeData[currentState].IndexedOffsets = currentStops.ToList();
            outcomeData[currentState].IndexedReelStrips = reelStrip.ToList();
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData.ReelOutcomeData, nameof(context.PersistentData.ReelOutcomeData));
            GeneralHelper.StepExceptionOnNull(this, context.ReelSetOperations.CurrentReelSet.Reels, nameof(context.ReelSetOperations.CurrentReelSet.Reels));
            return true;
        }
    }
}
