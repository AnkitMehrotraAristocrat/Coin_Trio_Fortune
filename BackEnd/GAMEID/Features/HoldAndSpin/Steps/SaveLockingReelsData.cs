using LockingReelsFeatureAccess = GameBackend.Features.LockingReels.Configuration.FeatureAccess;
using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using System.Linq;
using GameBackend.Features.HoldAndSpin.Data;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class SaveLockingReelsData : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var data = System.Array.Empty<int>(); 
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();

            // If HnS feature is not triggered we don't have to set locking reel information
            string hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            if (hnsContext.Triggered || BlackoutFeatureAccess.HasBlackout(context, hnsState) || context.GetCurrentState() == hnsState) {
                data = CorsFeatureAccess.GetPrizePositionsWorldIndex(context).ToArray();
            }

            LockingReelsFeatureAccess.AddLockingReelsData(context, hnsState, data);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
