using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.ExecutionSteps.PatternGenerator;

namespace GameBackend.Steps.General
{
    public class MilanGetPatternPayLines : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            new GetPatternPayLines().ExecuteAsync(context);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            #if LINES_GAME
            return true;
            #else
            return false;
            #endif
        }
    }
}