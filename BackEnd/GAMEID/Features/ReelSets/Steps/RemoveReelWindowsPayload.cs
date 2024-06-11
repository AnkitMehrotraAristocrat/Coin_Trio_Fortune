using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.ReelSets.Configuration;

namespace GameBackend.Features.ReelSets.Steps
{
    public class RemoveReelWindowsPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            // Remove payload, when not needed by frontend
            context.Payloads.RemovePayload(Constants.ReelWindowsPayloadName);
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
