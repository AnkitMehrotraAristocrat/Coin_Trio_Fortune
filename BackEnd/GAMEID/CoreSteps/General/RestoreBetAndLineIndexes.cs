using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using System.Threading.Tasks;
using GameBackend.Helpers;
using System.Linq;

namespace GameBackend.Steps.General
{
    /// <summary>
    /// Restores bet and line indexes from round data and ignores the values specified on the request since we can't
    /// trust them when in a feature.
    /// </summary>
    public class RestoreBetAndLineIndexes : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (context.GetCurrentState() == GeneralHelper.GetGameStateString(GameStates.BaseSpin)) {
                context.RoundData.BetIndex = context.BetOperations.MultiplierIndex;
                context.RoundData.LineIndex = context.BetOperations.BetLineIndex;
            }
            else {
                context.BetOperations.MultiplierIndex = context.RoundData.BetIndex;
                context.BetOperations.BetLineIndex = context.RoundData.LineIndex;
            }
            string currency = context.GetBetCurrencyType();
            var bet = context.MappedConfigurations.BetItems.ToList().Find(x => x.CurrencyType == currency);
            bet.SetCurrentLineIndex(context.RoundData.LineIndex);
            bet.SetCurrentMultiplierIndex(context.RoundData.BetIndex);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            GeneralHelper.StepExceptionOnNull(this, context.BetOperations, nameof(context.BetOperations));
            #if LINES_GAME
            return true;
            #else
            return false;
            #endif
        }
    }
}
