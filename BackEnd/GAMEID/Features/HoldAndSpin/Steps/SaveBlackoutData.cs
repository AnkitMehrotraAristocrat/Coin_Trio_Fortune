using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using System.Linq;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class SaveBlackoutData : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var data = CorsFeatureAccess.GetPrizePositionsWorldIndex(context).ToArray();
            BlackoutFeatureAccess.AddOccupiedCells(context, data, GeneralHelper.GetGameStateString(GameStates.HoldAndSpin));
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
