using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Data;
using GameBackend.Helpers;
using GameBackend.Features.ReelGrowth.Configuration;
using GameBackend.Features.ReelGrowth.Data;
using System.Linq;

namespace GameBackend.Features.ReelGrowth.Steps
{
    public class CheckForGrowth : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var reelWindow = context.GetCurrentReelWindow();
            var reelGrowthRoundData = context.FeatureRoundData<ReelGrowthRoundData>();

            foreach (string sym in Constants.GrowthSymbols) {
                var stops = reelWindow.StopsContent.ToList().FindAll(x => x.StopSymbol.Name == sym);
                foreach (var stop in stops) {
                    if (!reelGrowthRoundData.PreviousGrowthStops.Contains(stop.WorldIndex)) {
                        if (reelGrowthRoundData.GrowthStep < Constants.MaxGrowthStep) {
                            reelGrowthRoundData.GrowthStep++;
                        }
                        reelGrowthRoundData.PreviousGrowthStops.Add(stop.WorldIndex);
                    }
                }
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return true;
        }
    }
}
