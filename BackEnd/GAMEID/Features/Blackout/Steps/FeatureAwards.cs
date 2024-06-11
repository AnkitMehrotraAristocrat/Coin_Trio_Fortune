using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Blackout.Configuration;
using System.Linq;
using GameBackend.Features.Blackout.Data;

namespace GameBackend.Features.Blackout.Steps
{
    public class FeatureAwards : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var blContext = context.FeatureContext<BlackoutContext>();
            foreach (var kvp in blContext.OccupiedCells) {
                if(!blContext.BlackoutData.Prizes.ContainsKey(kvp.Key)) {
                    blContext.BlackoutData.Prizes[kvp.Key] = new();
                }
                if(FeatureAccess.HasBlackout(context, kvp.Key)) {
                    // Blackout awards a special prize of highest jackpot
                    var prize = new SharedDataPrizeInfo {
                        Tier = GameConstants.JackpotTiers.First().Value,
                        Type = GameConstants.JackpotPrizeType
                    };
                    ulong win = await JackpotHelper.AwardJackpotAsync(context, prize.Tier, GameConstants.JackpotID, 1);
                    prize.Value = win;
                    context.AddFeatureWonAward(win, context.GetCurrentState());
                    blContext.BlackoutData.Prizes[kvp.Key].Add(prize);
                }
            }
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
