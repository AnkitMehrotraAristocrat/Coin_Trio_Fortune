using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Configuration;
using GameBackend.Features.HoldAndSpin.Data;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class FeatureRetrigger : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();
            if (CorsFeatureAccess.GetLandedCorsCount(context) >= Constants.CountNeededToRetrigger) {
                hnsContext.Triggered = true;
                context.SetRemainingFreeSpins(Constants.FreeSpinsOnRetrigger);
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