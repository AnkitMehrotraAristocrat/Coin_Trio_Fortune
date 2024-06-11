using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Configuration;
using GameBackend.Features.HoldAndSpin.Data;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var hnsRoundData = context.FeatureRoundData<HoldAndSpinRoundData>();
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            if (hnsRoundData.LastFeaturePayload != null && context.GetCurrentState() == hnsState) {
                context.JoinPayload.AddPayload(Constants.PayloadNameFeature, hnsRoundData.LastFeaturePayload);
            }
            else {
                var state = hnsRoundData.LastFeaturePayload?.id ?? hnsState;
                context.JoinPayload.AddPayload(Constants.PayloadNameFeature, HoldAndSpinContext.GetDefaultStatePayload(state));
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
