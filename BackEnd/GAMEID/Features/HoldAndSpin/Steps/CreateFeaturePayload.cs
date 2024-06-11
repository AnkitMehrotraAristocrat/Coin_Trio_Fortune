using BlackoutFeatureAccess = GameBackend.Features.Blackout.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Data;
using GameBackend.Features.HoldAndSpin.Configuration;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();
            var hnsRoundData = context.FeatureRoundData<HoldAndSpinRoundData>();
            if (context.Transition.FromState != hnsState && context.Transition.ToState != hnsState && !BlackoutFeatureAccess.HasBlackout(context, hnsState)) {
                hnsRoundData.LastFeaturePayload = null;
                context.Payloads.AddPayload(Constants.PayloadNameFeature, HoldAndSpinContext.GetDefaultStatePayload(hnsState));
                return Task.CompletedTask;
            }

            var payload = new PayloadData() {
                id = hnsState,
                HoldAndSpinData = new HoldAndSpinPayloadData {
                    FreeSpinsRemaining = context.GetRemainingFreeSpins(hnsState),
                    TriggeringSpin = hnsContext.Triggered,
                    TriggeringState = hnsContext.Triggered
                        ? context.Transition.FromState
                        : hnsRoundData.LastFeaturePayload?.HoldAndSpinData?.TriggeringState ?? hnsState
                }
            };
            context.Payloads.AddPayload(Constants.PayloadNameFeature, payload);
            hnsRoundData.LastFeaturePayload = payload;
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
