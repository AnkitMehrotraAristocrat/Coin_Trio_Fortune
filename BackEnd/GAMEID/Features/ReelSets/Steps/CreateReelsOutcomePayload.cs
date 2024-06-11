using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Features.ReelSets.Steps
{
    public class CreateReelsOutcomePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var outcomeData = context.PersistentData.ReelOutcomeData;
            if (!outcomeData.ContainsKey(context.Transition.FromState)) {
                return Task.CompletedTask;
            }
            var payload = new ReelOutcomePayload() {
                Id = outcomeData[context.Transition.FromState].Id,
                ReelWindowId = outcomeData[context.Transition.FromState].ReelWindowId,
            };

            // Reel strips and offsets are to be ordered either by cell index (using client preference) or by column index
            payload.ReelStrips = outcomeData[context.Transition.FromState].IndexedReelStrips;
            payload.Offsets = outcomeData[context.Transition.FromState].IndexedOffsets;
            if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(context.Transition.FromState)]) {
                var currWinHeight = context.GetCurrentReelWindowCurrentHeight();
                var currWinWidth = context.GetCurrentReelWindowCurrentWidth();
                payload.ReelStrips = GeneralHelper.GetWorldIndexedListInClientFormation(payload.ReelStrips, currWinHeight, currWinWidth);
                payload.Offsets = GeneralHelper.GetWorldIndexedListInClientFormation(payload.Offsets, currWinHeight, currWinWidth);
            }

            context.Payloads.AddPayload(GameConstants.ReelsOutcomeModelPayloadName, payload);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData.ReelOutcomeData, nameof(context.PersistentData.ReelOutcomeData));
            return true;
        }
    }
}
