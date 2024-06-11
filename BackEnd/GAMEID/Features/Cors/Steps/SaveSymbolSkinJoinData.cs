using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using GameBackend.Features.Cors.Data;

namespace GameBackend.Features.Cors.Steps
{
    public class SaveSymbolSkinJoinData : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            string[] states = GeneralHelper.GetGameStatesArray();
            foreach (var state in states) {
                CorsContext.SaveSymbolSkinSpinningData(context, state);
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
