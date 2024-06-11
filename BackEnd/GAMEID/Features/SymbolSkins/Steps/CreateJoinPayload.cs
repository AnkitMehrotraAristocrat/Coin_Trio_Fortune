using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.SymbolSkins.Configuration;
using GameBackend.Features.SymbolSkins.Data;

namespace GameBackend.Features.SymbolSkins.Steps
{
    public class CreateJoinPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            CreateSpinningDataPayload(context);
            CreateSymbolOutcomePayloads(context);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static void CreateSymbolOutcomePayloads(GameContext context)
        {
            var ssRoundData = context.FeatureRoundData<SymbolSkinsRoundData>();
            foreach (var record in ssRoundData.LastFeaturePayloads) {
                context.JoinPayload.AddPayload(Constants.PayloadSymbolOutcome, record);
            }
        }

        private static void CreateSpinningDataPayload(GameContext context)
        {
            var ssContext = context.FeatureContext<SymbolSkinsContext>();
            foreach (var item in ssContext.SymbolSpinningTrackingData) {
                SpinningPayloadData record = new() {
                    Id = item.Id,
                    Data = item.Data
                };
                context.JoinPayload.AddPayload(Constants.PayloadSymbolSpinning, record);
            }
        }
    }
}
