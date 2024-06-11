using System.Collections.Generic;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.Payloads
{
    public class CreateFixedReelsConfigPayload : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var reelData = GetConfigsForGameContext(context);
            foreach (ReelStripData data in reelData.ReelStripData) {
                context.ConfigPayload.AddPayload(GameConstants.ReelStripModelPayloadName, data);
            }
            foreach (FixedReelSetData data in reelData.FixedReelSetData) {
                context.ConfigPayload.AddPayload(GameConstants.FixedReelSetModelPayloadName, data);
            }
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

        private static ReelData GetConfigsForGameContext(GameContext gameContext)
        {
            var reelData = new ReelData();
            var ReelSetsConfig = gameContext.CustomConfigurations.ReelSetStrips;
            foreach (var reelSet in gameContext.MappedConfigurations.ReelSets) {
                var reelSetData = new FixedReelSetData();
                reelData.FixedReelSetData.Add(reelSetData);
                reelSetData.Id = reelSet.Key;

                int i = 0;
                foreach (var reelStrip in reelSet.Value.Reels) {
                    string stripId = ReelSetsConfig[reelSet.Key][i];
                    reelSetData.Strips.Add(stripId);

                    var reelStripData = new ReelStripData();
                    reelData.ReelStripData.Add(reelStripData);
                    reelStripData.Id = stripId;

                    foreach (var symbol in reelStrip.Stops) {
                        reelStripData.Strip.Add(new SymbolIdData() { value = symbol.Id });
                    }
                    i++;
                }
            }
            return reelData;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private class ReelData
        {
            public List<FixedReelSetData> FixedReelSetData { get; set; } = new List<FixedReelSetData>();
            public List<ReelStripData> ReelStripData { get; set; } = new List<ReelStripData>();
        }

        private class FixedReelSetData
        {
            public string Id { get; set; }
            public List<string> Strips { get; set; } = new List<string>();
        }

        private class ReelStripData
        {
            public string Id { get; set; }
            public List<SymbolIdData> Strip { get; set; } = new List<SymbolIdData>();
        }

        private class SymbolIdData
        {
            public int value { get; set; }
        }
    }
}