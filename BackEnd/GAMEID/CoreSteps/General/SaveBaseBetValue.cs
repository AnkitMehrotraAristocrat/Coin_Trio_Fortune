using GameBackend.Data;
using System.Threading.Tasks;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class SaveBaseBetValue : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            context.PersistentData.BaseBetIndex = context.BetOperations.MultiplierIndex;
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
