using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Data;
using GameBackend.Helpers;
using GameBackend.Features.ReelGrowth.Configuration;
using GameBackend.Features.ReelGrowth.Data;

namespace GameBackend.Features.ReelGrowth.Steps
{
    public class SetHiddenCells : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            int step = context.FeatureRoundData<ReelGrowthRoundData>().GrowthStep;
            int cellsCount = GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight;

            for (int i = 0, j = cellsCount; i < j; i++) {
                context.HiddenWindowCells[i] = Constants.GrowthScenariosHiddenCells[step, i];
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
