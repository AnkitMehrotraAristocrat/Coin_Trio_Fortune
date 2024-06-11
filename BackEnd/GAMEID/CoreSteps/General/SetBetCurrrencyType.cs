using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class SetBetCurrrencyType : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.BetOperations.CurrencyType = context.XSlotConfigurations.BetConfiguration.DefaultCurrency;
            context.BetOperations.BetLineIndex = context.RoundData.LineIndex = 0;
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            GeneralHelper.StepExceptionOnNull(this, context.BetOperations, nameof(context.BetOperations));
            GeneralHelper.StepExceptionOnNull(this, context.XSlotConfigurations, nameof(context.XSlotConfigurations));
            return true;
        }
    }
}
