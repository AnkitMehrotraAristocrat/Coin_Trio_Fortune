using System.Threading.Tasks;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;
using GameBackend.Features.ReelGrowth.Configuration;
using GameBackend.Features.ReelGrowth.Data;

namespace GameBackend.Features.ReelGrowth.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (context.FeatureRoundData<ReelGrowthRoundData>().LastFeaturePayload != null) {
                PayloadData payload = new() {
                    GrowthStep = context.FeatureRoundData<ReelGrowthRoundData>().LastFeaturePayload.GrowthStep,
                    CellsHidden = context.FeatureRoundData<ReelGrowthRoundData>().LastFeaturePayload.CellsHidden
                };
                context.JoinPayload.AddPayload(Constants.PayloadName, payload);
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