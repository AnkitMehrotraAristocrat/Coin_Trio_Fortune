using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Cors.Data;

namespace GameBackend.Features.Cors.Steps
{
    public class SaveSymbolSkinSpinDataToState : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            // We do this as a separate step (from SaveSymbolSkinSpinDataFromState) because WindowCellSwaps may exist, 
            // in which case we want to save the swapped data for ToState and the original data for FromState.
            // Hence, execution order matters.
            // See StateMachine TransitionToNextState > SaveSymbolSkinSpinDataFromState > ProcessWindowCellSwaps > SaveSymbolSkinSpinDataToState.
            var crRoundData = context.FeatureRoundData<CorsRoundData>();
            var accumulated = crRoundData.PrizesCollected;

            if (context.Transition.ToState != context.Transition.FromState) {
                CorsContext.SaveSymbolSkinOutcomeData(context, accumulated.Prizes, context.Transition.ToState);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
    }
}
