using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.RoundModel.Configuration;
using Wildcat.Milan.Shared.Dtos.Backend.Round;

namespace GameBackend.Features.RoundModel.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var rmRoundData = context.FeatureRoundData<RoundModelRoundData>();
            context.Payloads.AddPayload(Constants.PayloadName, rmRoundData);
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
