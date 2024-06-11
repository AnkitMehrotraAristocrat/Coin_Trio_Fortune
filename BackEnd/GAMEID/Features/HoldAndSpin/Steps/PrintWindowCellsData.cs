using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Threading.Tasks;
using GameBackend.Helpers;
using System.Linq;
using System.Text;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class PrintWindowCellsData : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);

            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/
            
            int betLevel = ReelSetsFeatureAccess.GetCurrentBetLevel(context);
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            var reelWindows = context.GetCurrentStateReelWindowNames();

            var outcomeData = context.PersistentData.ReelOutcomeData;
            var hiddenCells = context.HiddenWindowCells;

            // Data to print 
            var reelsForNextSpin = ReelSetsFeatureAccess.GetReelStripsData(context, context.GetCurrentState(), reelWindows[0], betLevel).ToList();
            var reelsForCurrentSpin = outcomeData[hnsState].IndexedReelStrips;
            var offsets = outcomeData[hnsState].IndexedOffsets;
            var lockedPrizes = CorsFeatureAccess.GetPrizePositionsWorldIndex(context);

            var space = "\t\t";
            var hidden = "Hidden";
            var locked = "Locked";
            var active = "Active";

            // Write in batch so console output is not interrupted 
            var batch = new StringBuilder();
            batch.AppendLine();
            batch.AppendLine();
            batch.Append($"Cell{space}State{space}Offset{space}Current{space}{space}Next");
            batch.AppendLine();
            for (int cell = 0; cell < reelsForNextSpin.Count; cell++) {
                var state = hiddenCells[cell] ? hidden : (lockedPrizes.Contains(cell) ? locked : active);
                batch.Append($"{cell}{space}");
                batch.Append($"{state}{space}");
                batch.Append($"{offsets[cell]}{space}");
                batch.Append($"{reelsForCurrentSpin[cell]}{space}");
                batch.Append($"{reelsForNextSpin[cell]}{space}");
                batch.AppendLine();
            }
            batch.AppendLine();

            DebugHelper.LogText(batch.ToString());
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

