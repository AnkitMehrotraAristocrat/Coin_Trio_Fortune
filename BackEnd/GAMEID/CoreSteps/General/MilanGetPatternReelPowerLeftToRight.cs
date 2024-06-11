using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.ExecutionSteps.PatternGenerator;

namespace GameBackend.Steps.General
{
    public class MilanGetPatternReelPowerLeftToRight : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            new GetPatternReelPowerLeftToRight().ExecuteAsync(context);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            #if WAYS_GAME
            return true;
            #else //LINES_GAME
            return false;
            #endif
        }
    }
}