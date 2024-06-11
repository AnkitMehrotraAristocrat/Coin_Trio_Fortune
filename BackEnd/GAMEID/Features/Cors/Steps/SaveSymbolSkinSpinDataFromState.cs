using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Cors.Data;

namespace GameBackend.Features.Cors.Steps
{
    public class SaveSymbolSkinSpinDataFromState : BaseStep<GameContext>
	{
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            var crRoundData = context.FeatureRoundData<CorsRoundData>();
            var accumulated = crRoundData.PrizesCollected;

            CorsContext.SaveSymbolSkinOutcomeData(context, accumulated.Prizes, context.Transition.FromState);
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
