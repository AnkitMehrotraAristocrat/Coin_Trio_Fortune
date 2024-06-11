using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps
{
    public class EnableAllReelStrips : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            // All reel strips from 0 to lastEnabledReelStripIndex are enabled
            var lastEnabledReelStripIndex = context.CurrentReelWindowDefinition.Width - 1;
            context.SpinData.PatternContext.EnabledReelStripIndex = lastEnabledReelStripIndex;

            foreach (var betItem in context.MappedConfigurations.BetItems) {
                betItem.SetCurrentReelStripCostIndex(lastEnabledReelStripIndex);
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            #if WAYS_GAME
            return true;
            #else //LINES_GAME
            return false;
            #endif
        }
    }
}
