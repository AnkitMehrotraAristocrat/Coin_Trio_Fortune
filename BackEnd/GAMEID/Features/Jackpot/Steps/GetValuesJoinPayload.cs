using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using GameBackend.Helpers;
using System.Threading.Tasks;
using System.Linq;
using GameBackend.Features.Jackpot.Data;
using GameBackend.Features.Jackpot.Configuration;

namespace GameBackend.Features.Jackpot.Steps
{
    public class GetValuesJoinPayload : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var jackpotInits = await JackpotHelper.GetInitialJackpotValuesAsync(context, GameConstants.JackpotID);
            var betMultipliers = context.CustomConfigurations.JackpotConfig.Multipliers;
            foreach (var initModel in jackpotInits) {
                // Push the jackpot bet level multiplier driver data into the payloads
                var jackpotBetLevelMultiplierData = new JackpotBetLevelMultiplier(2, new JackpotBetLevelMultiplierPayload(GameConstants.JackpotID, initModel.TierPosition.Value, null), betMultipliers.ToArray());
                context.JoinPayload.AddPayload(Constants.MultiplierPayloadName, jackpotBetLevelMultiplierData);
                context.JoinPayload.AddPayload(Constants.ValuesPayloadName, initModel);
            }
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}