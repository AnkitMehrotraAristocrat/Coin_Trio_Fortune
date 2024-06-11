using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class DecrementFreeSpinsCount : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (context.GetRemainingFreeSpins() > 0) {
                context.SetRemainingFreeSpins(context.GetRemainingFreeSpins() - 1);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
