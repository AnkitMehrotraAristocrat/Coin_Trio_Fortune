using System.Threading.Tasks;
using GameBackend.Data;
using Milan.XSlotEngine.Core.Extensions;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;
using GameBackend.Features.ReelGrowth.Configuration;
using GameBackend.Features.ReelGrowth.Data;
using System.Linq;

namespace GameBackend.Features.ReelGrowth.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            // Push to payload
            var rgRoundData = context.FeatureRoundData<ReelGrowthRoundData>();
            var hiddenCells = GeneralHelper.GetWorldIndexedListInClientFormation(context.HiddenWindowCells.ToList(), GameConstants.WindowMaxHeight, GameConstants.WindowMaxWidth);
            PayloadData payload = new() {
                GrowthStep = rgRoundData.GrowthStep,
                CellsHidden = hiddenCells.ToArray()
            };
            context.Payloads.AddPayload(Constants.PayloadName, payload);
            rgRoundData.LastFeaturePayload = payload;
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
