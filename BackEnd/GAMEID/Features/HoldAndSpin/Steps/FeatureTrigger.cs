using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Data;
using GameBackend.Features.HoldAndSpin.Configuration;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class FeatureTrigger : BaseStep<GameContext>
    {        
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            if (CorsFeatureAccess.GetLandedCorsCount(context) >= Constants.CountNeededToTrigger && !BlackoutFeatureAccess.HasBlackout(context, hnsState)) {
                var priority = context.CustomConfigurations.StatesExcecutionPriority.StatePriority[hnsState];
                context.PersistentData.TriggeredStates.Queue.Enqueue(priority, hnsState);

                hnsContext.Triggered = true;
                context.SetRemainingFreeSpins(Constants.FreeSpinsOnTrigger, hnsState);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}