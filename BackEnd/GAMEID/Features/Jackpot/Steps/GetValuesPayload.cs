using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using GameBackend.Helpers;
using System.Threading.Tasks;
using GameBackend.Features.Jackpot.Configuration;

namespace GameBackend.Features.Jackpot.Steps
{
    public class GetValuesPayload : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            // WILD: removing this step from the SPINS. The jackpot values will be returned by the spin report action.

            //DebugHelper.LogStep(this);
            var jpVals = await JackpotHelper.GetInitialJackpotValuesAsync(context, GameConstants.JackpotID);
            foreach (var jpVal in jpVals)
            {
                context.Payloads.AddPayload(Constants.ValuesPayloadName, jpVal);
            }
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}