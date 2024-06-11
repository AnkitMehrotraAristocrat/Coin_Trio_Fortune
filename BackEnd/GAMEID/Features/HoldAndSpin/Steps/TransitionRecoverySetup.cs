using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using GameBackend.Data;
using GameBackend.Features.HoldAndSpin.Data;
using GameBackend.Helpers;
using Milan.StateMachine.PipelineHandler;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class TransitionRecoverySetup : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            if (context.Transition.ToState != hnsState) {
                return Task.CompletedTask;
            }
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();
            int betLevel = ReelSetsFeatureAccess.GetCurrentBetLevel(context);

            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/

            string[] reelWindows = context.GetCurrentStateReelWindowNames();
            var reelsForCurrentSpin = new List<string>(ReelSetsFeatureAccess.GetReelStripsData(context, context.GetCurrentState(), reelWindows[0], betLevel).ToList());

            // Set recovery offsets
            var reelStrips = context.XSlotConfigurations.ReelStripCollectionConfiguration.ReelStripsDefinition;
            var lockedPrizes = CorsFeatureAccess.GetPrizePositionsWorldIndex(context);

            var cellsCount = GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight;
            var offsets = new List<int>(new int[cellsCount]);

            for (int cell = 0; cell < cellsCount; cell++) {
                var reelStrip = reelStrips.First(rs => rs.Key == reelsForCurrentSpin[cell]).Value;

                if (context.HiddenWindowCells[cell]) {
                    // Hidden cells should be on a blank
                    var pos = hnsContext.GetBlankStopIndex(reelStrip);
                    offsets[cell] = pos;
                }
                else if (lockedPrizes.Contains(cell)) {
                    // Locked cells should be on same symbol (from same reelstrip)
                    var pos = CorsFeatureAccess.GetPositionInReelStrip(context, cell, reelStrip);
                    offsets[cell] = pos;
                }
                else {
                    // Try and find the current symbol, otherwise use blank
                    var currSymbol = context.GetCurrentReelWindow().StopsContent[cell].StopSymbol.Name;
                    var pos = reelStrip.Stops.IndexOf(reelStrip.Stops.FirstOrDefault(x => x.Symbol == currSymbol));
                    if (pos == -1) {
                        pos = hnsContext.GetBlankStopIndex(reelStrip);
                    }
                    offsets[cell] = pos;
                }
            }

            // Save offsets and reelstrips 
            var outcomeData = context.PersistentData.ReelOutcomeData;
            outcomeData[hnsState].IndexedOffsets = offsets;
            outcomeData[hnsState].IndexedReelStrips = reelsForCurrentSpin;
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}
