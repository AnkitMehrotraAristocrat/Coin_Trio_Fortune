using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.LockingReels.Configuration;
using GameBackend.Features.LockingReels.Data;
using System.Collections.Generic;

namespace GameBackend.Features.LockingReels.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var lrContext = context.FeatureContext<LockingReelsContext>();
            List<PayloadData> lastFeaturePayload = new();
            var currWinHeight = context.GetCurrentReelWindowCurrentHeight();
            var currWinWidth = context.GetCurrentReelWindowCurrentWidth();
            foreach (var kvp in lrContext.LockingReelsData.Data) {
                PayloadData payload = new() { Id = kvp.Key };
                if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(kvp.Key)]) {
                    payload.Reels = GeneralHelper.GetClientIndexListByWorldIndexList(kvp.Value, currWinHeight, currWinWidth);
                }
                else {
                    payload.Reels = kvp.Value;
                }
                context.Payloads.AddPayload(Constants.PayloadNameLockingReels, payload);
                lastFeaturePayload.Add(payload);
            }

            context.FeatureRoundData<LockingReelsRoundData>().LastFeaturePayload = lastFeaturePayload;
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
