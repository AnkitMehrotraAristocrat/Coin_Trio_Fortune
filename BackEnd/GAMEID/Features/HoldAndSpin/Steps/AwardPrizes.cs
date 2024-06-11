using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class AwardPrizes : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            if (BlackoutFeatureAccess.HasBlackout(context, hnsState) || context.GetRemainingFreeSpins() != 0) {
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
