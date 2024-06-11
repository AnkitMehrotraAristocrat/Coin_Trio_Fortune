using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using System.Linq;
using GameBackend.Data;
using Milan.StateMachine.PipelineHandler;
using System.Collections.Generic;
using Milan.XSlotEngine.Core.Models;
using System.Threading.Tasks;
using Milan.Common.SlotEngine.Models;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Data;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class GenerateWindowWithStops : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            context.PersistentData.GaffeQueues.ConsumeCategoryQueue(GaffeCategories.GenerateWindowWithStops, context.PersistentData.RandomNumberQueue);

            var hnsContext = context.FeatureContext<HoldAndSpinContext>();

            /* ---------------------------------------------------------------------------------------------
            |           This logic currently only support evaluation for single reel window i.e. index 0    |
            |           Dev need to revisit this to support Multiple reel window                            |
            |                                                                                               |
            |           Search for :- MISSING-MULTI-REEL-WINDOW-SUPPORT to identify the tasks               |
            |                                                                                               |
             ----------------------------------------------------------------------------------------------*/

            int betLevel = ReelSetsFeatureAccess.GetCurrentBetLevel(context);

            string[] reelWindows = context.GetCurrentStateReelWindowNames();
            var reelsForCurrentSpin = new List<string>(ReelSetsFeatureAccess.GetReelStripsData(context, context.GetCurrentState(), reelWindows[0], betLevel).ToList());

            // Initialize window
            var mainReelWindow = context.MappedConfigurations.ReelWindowDefinitions.First(rw => rw.Key == context.GetCurrentState()).Value;
            var reelWindow = new ReelWindow(mainReelWindow);

            // Spin active cells
            var reelStrips = context.XSlotConfigurations.ReelStripCollectionConfiguration.ReelStripsDefinition;
            var lockedPrizes = CorsFeatureAccess.GetPrizePositionsWorldIndex(context);
            var symbolsMapped = HoldAndSpinContext.GetSymbolsMapped(context);

            var cellsCount = GameConstants.WindowMaxWidth * GameConstants.WindowMaxHeight;
            var offsets = new List<int>(new int[cellsCount]);

            for (int cell = 0; cell < cellsCount; cell++) {
                var reelsForCurrentSpinIndex = cell;
                if (reelsForCurrentSpinIndex >= reelsForCurrentSpin.Count) {
                    reelsForCurrentSpinIndex = reelsForCurrentSpin.Count - 1;
                }
                if (reelsForCurrentSpinIndex < 0) {
                    reelsForCurrentSpinIndex = 0;
                }

                var reelStrip = reelStrips.First(rs => rs.Key == reelsForCurrentSpin[reelsForCurrentSpinIndex]).Value;
                var auxReelStrip = new ReelStrip(reelStrip.Stops, symbolsMapped);
                
                SymbolData sym = null;
                if (context.HiddenWindowCells[cell]) {
                    // Hidden cells should stop on a blank
                    var pos = hnsContext.GetBlankStopIndex(reelStrip);
                    offsets[cell] = pos;
                    sym = auxReelStrip.Stops[pos];
                    GeneralHelper.ConsumeQueuedNumber(context.PersistentData.RandomNumberQueue);
                }
                else if (lockedPrizes.Contains(cell)) {
                    // Locked cells should stop on same symbol (from same reelstrip)
                    var pos = CorsFeatureAccess.GetPositionInReelStrip(context, cell, reelStrip); 
                    offsets[cell] = pos;
                    sym = auxReelStrip.Stops[pos];
                    GeneralHelper.ConsumeQueuedNumber(context.PersistentData.RandomNumberQueue);
                }
                else {
                    // Active cells generate a new stop
                    var pos = GeneralHelper.GetRandomValue(context, auxReelStrip.MaxPosition, "GenerateWindowWithStops");
                    offsets[cell] = pos;
                    sym = auxReelStrip.Stops[pos];
                }
                var coords2D = GeneralHelper.GetPositionByIndex(cell);
                reelWindow.StopsContent.Add(new StopData(new CoordinateData(coords2D.X, coords2D.Y), sym));
            }

            // Save outcomeData and window
            var outcomeData = context.PersistentData.ReelOutcomeData;
            var hnsState = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            outcomeData[hnsState].IndexedOffsets = offsets;
            outcomeData[hnsState].IndexedReelStrips = reelsForCurrentSpin;
            
            context.SetReelWindow(reelWindow);
            return Task.CompletedTask;
        }

        public override bool Validate(GameContext context)
        {
            GeneralHelper.StepExceptionOnNull(this, context, nameof(context));
            GeneralHelper.StepExceptionOnNull(this, context.RoundData, nameof(context.RoundData));
            GeneralHelper.StepExceptionOnNull(this, context.PersistentData, nameof(context.PersistentData));
            return true;
        }
    }
}
