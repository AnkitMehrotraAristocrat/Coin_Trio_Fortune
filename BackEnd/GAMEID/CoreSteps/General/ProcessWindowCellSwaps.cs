using Milan.StateMachine.PipelineHandler;
using GameBackend.Data;
using System.Threading.Tasks;
using GameBackend.Helpers;

namespace GameBackend.Steps.General
{
    public class ProcessWindowCellSwaps : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            // IndexedReelStrips and IndexedOffsets are only indexed by cell in SingleCellReels type windows
            // In Matrix windows IndexedReelStrips and IndexedOffsets are indexed by column  
            if (!GameConstants.SingleCellReels[GeneralHelper.GetGameStateEnum(context.Transition.ToState)]) {
                return Task.CompletedTask;
            }

            // If any WindowCellSwaps exist that means we are transitioning to a new state, and the client has a custom PositionMap (see config)
            // This occurs only when a non-standard move is made, e.g. |A|B|C|D| -> |C|B|A|D| 
            // This example means that the client wanted to move 0,0 to 2,0 (swap A with C) 
            // All features holding persistant data tied to cells (positions) must take this into account and do the swaps 
            // Note that with state/window based data you'd only consider the data for the window you are transitioning to 
            var outcomeData = context.PersistentData.ReelOutcomeData;
            var transitionStateData = outcomeData[context.Transition.ToState];

            foreach (var swap in context.WindowCellSwaps.Swaps) {
                var tempReelStrip = transitionStateData.IndexedReelStrips[swap.FromWorldIndex];
                transitionStateData.IndexedReelStrips[swap.FromWorldIndex] = transitionStateData.IndexedReelStrips[swap.ToWorldIndex];
                transitionStateData.IndexedReelStrips[swap.ToWorldIndex] = tempReelStrip;

                var tempOffset = transitionStateData.IndexedOffsets[swap.FromWorldIndex];
                transitionStateData.IndexedOffsets[swap.FromWorldIndex] = transitionStateData.IndexedOffsets[swap.ToWorldIndex];
                transitionStateData.IndexedOffsets[swap.ToWorldIndex] = tempOffset;
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            return (context.PersistentData.ReelOutcomeData.Count > 0);
        }
    }
}