using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.SymbolSkins.Configuration;
using GameBackend.Features.SymbolSkins.Data;

namespace GameBackend.Features.SymbolSkins.Steps
{
    public class CreateFeaturePayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
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
            var ssContext = context.FeatureContext<SymbolSkinsContext>();
            var ssRoundData = context.FeatureRoundData<SymbolSkinsRoundData>();
            var outcomePayloads = ssRoundData.LastFeaturePayloads;

            // Cache current spin payloads
            var currWinHeight = context.GetCurrentReelWindowCurrentHeight();
            var currWinWidth = context.GetCurrentReelWindowCurrentWidth();
            foreach (var item in ssContext.SymbolOutcomeTrackingData) {
                OutcomePayload outcomePayload = outcomePayloads.Find(payload => payload.Id == item.Id);
                if (outcomePayload == null) {
                    outcomePayload = new() { Id = item.Id };
                    outcomePayloads.Add(outcomePayload);
                }

                outcomePayload.SymbolOutcomeData.Clear();
                foreach (var outcomeData in item.SymbolOutcomeData) {
                    OutcomePayloadData outcomePayloadData = new() {
                        CanAward = outcomeData.CanAward,
                        Tier = outcomeData.Tier,
                        SymbolData = outcomeData.SymbolData
                    };
                    if (GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(item.Id)]) {
                        int clientReelIndex = GeneralHelper.GetClientIndexByWorldIndex(outcomeData.WorldIndex, currWinHeight, currWinWidth);
                        outcomePayloadData.PositionData = new(clientReelIndex, 0);
                    }
                    else {
                        outcomePayloadData.PositionData = GeneralHelper.GetClientPositionByWorldIndex(outcomeData.WorldIndex, currWinHeight);
                    }
                    outcomePayload.SymbolOutcomeData.Add(outcomePayloadData);
                }
            }

            // Send cached payloads
            foreach (var payload in outcomePayloads) {
                context.Payloads.AddPayload(Constants.PayloadSymbolOutcome, payload);
            }
        }
    }
}
