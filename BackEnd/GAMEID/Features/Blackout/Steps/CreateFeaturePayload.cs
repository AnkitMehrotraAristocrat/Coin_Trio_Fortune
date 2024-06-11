using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Blackout.Configuration;
using GameBackend.Features.Blackout.Data;
using System.Collections.Generic;

namespace GameBackend.Features.Blackout.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var blContext = context.FeatureContext<BlackoutContext>();

            List<PayloadData> data = new();
            foreach (var kvp in blContext.OccupiedCells) {
                // Push to payload
                PayloadData payload = new() {
                    Id = kvp.Key,
                    Blackout = FeatureAccess.HasBlackout(context, kvp.Key),
                    Prizes = blContext.BlackoutData.Prizes[kvp.Key]
                };
                context.Payloads.AddPayload(Constants.PayloadNameBlackout, payload);
                data.Add(payload);
            }
            
            context.FeatureRoundData<BlackoutRoundData>().LastFeaturePayload = data;
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
