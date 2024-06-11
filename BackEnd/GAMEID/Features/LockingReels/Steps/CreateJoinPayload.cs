using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.LockingReels.Configuration;
using GameBackend.Features.LockingReels.Data;

namespace GameBackend.Features.LockingReels.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            var lrRoundData = context.FeatureRoundData<LockingReelsRoundData>();
            if (lrRoundData.LastFeaturePayload != null) {
                foreach (var data in lrRoundData.LastFeaturePayload) {
                    context.JoinPayload.AddPayload(Constants.PayloadNameLockingReels, data);
                }
            }
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
