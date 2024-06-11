using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.ReelSets.Data;
using GameBackend.Features.ReelSets.Configuration;

namespace GameBackend.Features.ReelSets.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.Payloads.RemovePayload(Constants.ReelSetsPayloadName);

            // Fetch all reel window name configured by state
            string[] reelWindows = context.GetCurrentStateReelWindowNames();
            var rsPersistentData = context.FeaturePersistentData<ReelSetsPersistentData>();
            foreach (string windowName in reelWindows) {
                var payload = new PayloadData(
                    rsPersistentData.ReelStripsPerBetIndex.WindowData[context.GetCurrentState()][windowName],
                    FeatureAccess.GetCurrentBetLevel(context)
                );
                context.Payloads.AddPayload(Constants.ReelSetsPayloadName, payload);
            }

            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}
