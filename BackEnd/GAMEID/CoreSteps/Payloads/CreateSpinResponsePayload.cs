using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateSpinResponsePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.Payloads.AddPayload(GameConstants.SpinResponsePayloadName, true);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}