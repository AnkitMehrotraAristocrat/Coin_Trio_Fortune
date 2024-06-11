using MilanGenerateSetOfRandomValues = Milan.XSlotEngine.ExecutionSteps.Custom.GenerateSetOfRandomValues;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;
using System.Threading.Tasks;

namespace GameBackend.Steps.General
{
    public class GenerateSetOfRandomValues : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.PersistentData.GaffeQueues.ConsumeCategoryQueue(GaffeCategories.GenerateSetOfRandomValues, context.PersistentData.RandomNumberQueue);
            new MilanGenerateSetOfRandomValues().ExecuteAsync(context);
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
