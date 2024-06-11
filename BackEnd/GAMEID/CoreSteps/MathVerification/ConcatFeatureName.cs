using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.MathVerification
{
    public class ConcatFeatureName : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (context.PersistentData.RtpFeatureName.Contains(context.GetCurrentState())) {
                return Task.CompletedTask;
            }

            context.PersistentData.RtpFeatureName = string.Concat(context.PersistentData.RtpFeatureName, "+", context.GetCurrentState());
            context.MetricAddOrUpdate(context.PersistentData.RtpFeatureName, 1);
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
