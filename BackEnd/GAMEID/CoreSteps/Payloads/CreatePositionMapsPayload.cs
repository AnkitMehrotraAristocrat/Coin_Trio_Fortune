using System.Collections.Generic;
using System.Threading.Tasks;
using GameBackend.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;

namespace GameBackend.Steps.Payloads
{
    public class CreatePositionMapsPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.ConfigPayload.AddPayload(GameConstants.PositionMapsPayloadName, context.CustomConfigurations.ClientPositionMaps);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.MappedConfigurations, nameof(context.MappedConfigurations));
            return true;
        }
    }
}
