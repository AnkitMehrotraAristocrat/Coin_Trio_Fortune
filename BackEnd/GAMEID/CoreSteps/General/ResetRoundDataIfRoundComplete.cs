using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class ResetRoundDataIfRoundComplete : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (!context.RoundData.RoundComplete) {
                return Task.CompletedTask;
            }
            // Resetting the round data must be done via Reset.
            // Instantiating a new object will break the reference with the objects managed by the service.
            // adapter and result in stale data being retained in storage.
            context.RoundData.Reset();
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
    }
}