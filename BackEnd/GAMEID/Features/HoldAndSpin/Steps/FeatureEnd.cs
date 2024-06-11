using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class FeatureEnd : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (context.GetRemainingFreeSpins() == 0 || BlackoutFeatureAccess.HasBlackout(context, GeneralHelper.GetGameStateString(GameStates.HoldAndSpin))) {
                context.PersistentData.TriggeredStates.Queue.Dequeue();
            }
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

