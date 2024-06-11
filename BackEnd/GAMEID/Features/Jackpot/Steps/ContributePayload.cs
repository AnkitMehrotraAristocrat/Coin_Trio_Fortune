using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using GameBackend.Helpers;
using System.Threading.Tasks;

namespace GameBackend.Features.Jackpot.Steps
{
    public class ContributePayload : BaseStep<GameContext>
    {
        public override async Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            var jpVals = await JackpotHelper.ContributeToJackpotAsync(context);

            // WILD: consume the jackpot spin payload
            foreach (var jpVal in jpVals)
            {
                context.Payloads.AddPayload(GameBackend.Features.Jackpot.Configuration.Constants.ValuesPayloadName, jpVal);
            }
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}