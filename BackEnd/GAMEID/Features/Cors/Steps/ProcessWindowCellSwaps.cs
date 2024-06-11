using GameBackend.Data;
using GameBackend.Features.Cors.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;

namespace GameBackend.Features.Cors.Steps
{
    public class ProcessWindowCellSwaps : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            // If any WindowCellSwaps exist that means we are transitioning to a new state, and the client has a custom PositionMap (see config)
            // This occurs only when a non-standard move is made, e.g. |A|B|C|D| -> |C|B|A|D| 
            // This example means that the client wanted to move 0,0 to 2,0 (swap A with C) 
            // All features holding persistant data tied to cells (positions) must take this into account and do the swaps 
            // Note that with state/window based data you'd only consider the data for the window you are transitioning to 
            var prizeData = context.FeatureRoundData<CorsRoundData>().PrizesCollected;

            foreach (var swap in context.WindowCellSwaps.Swaps) {
                var prizesAtFromWorldIndex = prizeData.Prizes.FindAll(prize => prize.Stop.WorldIndex == swap.FromWorldIndex);
                var prizesAtToWorldIndex = prizeData.Prizes.FindAll(prize => prize.Stop.WorldIndex == swap.ToWorldIndex);
                foreach(var prize in prizesAtFromWorldIndex) {
                    prize.Stop.WorldIndex = swap.ToWorldIndex;
                }
                foreach (var prize in prizesAtToWorldIndex) {
                    prize.Stop.WorldIndex = swap.FromWorldIndex;
                }
            }
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            return true;
        }
    }
}
