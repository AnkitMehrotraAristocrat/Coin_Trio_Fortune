using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using System.Linq;
using GameBackend.Helpers;
using GameBackend.Features.Cors.Configuration;
using GameBackend.Features.Cors.Data;

namespace GameBackend.Features.Cors.Steps
{
    public class GetLandedCorSymbols : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var reelWindow = context.GetCurrentReelWindow();
            var collected = CorsContext.GetPrizePositionsWorldIndex(context);

            foreach (string sym in Constants.CorSymbols) {
                var stops = reelWindow.StopsContent.ToList().FindAll(x => x.StopSymbol.Name == sym);
                foreach (var stop in stops) {
                    // Add only Cor symbols that have no prizes yet, all others are locked for the round
                    if (!collected.Contains(stop.WorldIndex)) {
                        context.FeatureContext<CorsContext>().LandedCorSymbols.Add(stop);
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