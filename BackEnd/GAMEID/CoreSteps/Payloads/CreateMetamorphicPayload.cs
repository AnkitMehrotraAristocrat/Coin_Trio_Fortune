using System.Collections.Generic;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateMetamorphicPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            MetamorphicFeature metamorphicFeature = new MetamorphicFeature();
            metamorphicFeature.IsDragonFeatureActivated = context.RoundData.isDragonFeatureTriggered;
            metamorphicFeature.IsTigerFeatureActivated = context.RoundData.isTigerFeatureTriggered;
            metamorphicFeature.IsKoiFeatureActivated = context.RoundData.isKoiFeatureTriggered;

            context.Payloads.AddPayload(GameConstants.MetamorphicFeaturePayloadName, metamorphicFeature);

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