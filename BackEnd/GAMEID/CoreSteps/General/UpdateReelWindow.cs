using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class UpdateReelWindow : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.SetReelWindow(context.SpinData.CurrentReelWindow);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.SpinData, nameof(context.SpinData));
            GeneralHelper.StepExceptionOnNull(this, context.SpinData.CurrentReelWindow, nameof(context.SpinData.CurrentReelWindow));
            return true;
        }
    }
}
