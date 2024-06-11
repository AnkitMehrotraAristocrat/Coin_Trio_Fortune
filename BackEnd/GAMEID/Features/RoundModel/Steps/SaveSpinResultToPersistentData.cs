using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using Wildcat.Milan.Shared.Dtos.Backend.Round;

namespace GameBackend.Features.RoundModel.Steps
{
    public class SaveSpinResultToPersistentData : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var rmRoundData = context.FeatureRoundData<RoundModelRoundData>();

            ulong creditsWon = 0;
            foreach (var reward in context.SpinData.Results.WonRewards) {
                if (reward.CurrencyType == GameConstants.CreditType) {
                    creditsWon += reward.TotalWon;
                }
            }

            rmRoundData.Wins.Add(new RoundWin {
                SlotEventId = context.SpinGuid,
                Amount = new Currency { Type = GameConstants.CreditType, Value = creditsWon },
                Ids = rmRoundData.Wins.NewId(context.GetCurrentState())
            });
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
