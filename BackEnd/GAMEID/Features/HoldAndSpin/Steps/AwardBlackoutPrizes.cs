using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Data;
using GameBackend.Helpers;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class AwardBlackoutPrizes : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            if (!BlackoutFeatureAccess.HasBlackout(context, GeneralHelper.GetGameStateString(GameStates.HoldAndSpin))) {
                return;
            }
            ulong totalWin = 0;
            totalWin += CorsFeatureAccess.AwardMultipliers(context);
            totalWin += await CorsFeatureAccess.AwardJackpotsAsync(context);
            context.AddFeatureWonAward(totalWin, context.GetCurrentState());
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
    }
}
