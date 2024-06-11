using System.Threading.Tasks;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateLinesModelRecoveryPayloads : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.JoinPayload.AddPayload(GameConstants.LinesModelPayloadName, context.PersistentData.LinesModelPayload);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            #if LINES_GAME
            return true;
            #else
            return false;
            #endif
        }
    }
}