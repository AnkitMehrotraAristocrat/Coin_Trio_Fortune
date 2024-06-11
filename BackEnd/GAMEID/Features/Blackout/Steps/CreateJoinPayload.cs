using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Blackout.Configuration;
using GameBackend.Features.Blackout.Data;

namespace GameBackend.Features.Blackout.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var lrRoundData = context.FeatureRoundData<BlackoutRoundData>();
            if (lrRoundData.LastFeaturePayload != null && lrRoundData.LastFeaturePayload.Count > 0) {
                foreach (var data in lrRoundData.LastFeaturePayload) {
                    context.JoinPayload.AddPayload(Constants.PayloadNameBlackout, data);
                }
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
