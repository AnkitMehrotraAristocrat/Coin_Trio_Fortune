using System.Threading.Tasks;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using Milan.XSlotEngine.ExecutionSteps.BetManager;

namespace GameBackend.Steps.General
{
    public class MilanUpdateBetLines : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            new UpdateBetLines().ExecuteAsync(context);
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