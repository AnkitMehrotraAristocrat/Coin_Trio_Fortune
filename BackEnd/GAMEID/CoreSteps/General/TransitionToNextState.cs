using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class TransitionToNextState : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            string nextState = context.GetNextState();
            context.Workflow.SetNextState(nextState);

            context.Transition = new TransitionData(context.GetCurrentState(), nextState);
            context.MetricAddOrUpdate($"Transition to {nextState} Count", 1);

            context.PersistentData.PreviousState = context.GetCurrentState();
            context.SetCurrentState(nextState);
            
            int toHeight = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(context.Transition.ToState)][1];
            int toWidth = GameConstants.VisualWindowWidthHeight[GeneralHelper.GetGameStateEnum(context.Transition.ToState)][0];
            int fromHeight = context.GetCurrentReelWindowCurrentHeight();
            int fromWidth = context.GetCurrentReelWindowCurrentWidth();

            context.UpdateWindowCellSwaps(context.Transition.FromState, fromHeight, fromWidth, context.Transition.ToState, toHeight, toWidth);
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
