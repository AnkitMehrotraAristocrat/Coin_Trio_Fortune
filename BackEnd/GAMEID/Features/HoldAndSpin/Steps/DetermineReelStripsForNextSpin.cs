using CorsFeatureAccess = GameBackend.Features.Cors.Configuration.FeatureAccess;
using ReelSetsFeatureAccess = GameBackend.Features.ReelSets.Configuration.FeatureAccess;
using GameBackend.Data;
using System.Threading.Tasks;
using Milan.StateMachine.PipelineHandler;
using GameBackend.Helpers;
using GameBackend.Features.HoldAndSpin.Data;
using GameBackend.Features.HoldAndSpin.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.HoldAndSpin.Steps
{
    public class DetermineReelStripsForNextSpin : BaseStep<GameContext>
    {
        public override Task ExecuteAsync(GameContext context)
        {
            DebugHelper.LogStep(this);
            string HnsStateString = GeneralHelper.GetGameStateString(GameStates.HoldAndSpin);
            var hnsContext = context.FeatureContext<HoldAndSpinContext>();
            if (!hnsContext.Triggered && context.GetCurrentState() != HnsStateString) {
                return Task.CompletedTask;
            }

            int betLevel = ReelSetsFeatureAccess.GetCurrentBetLevel(context);

            string[] reelWindows = GameConstants.StateReelWindows[GameStates.HoldAndSpin];
            foreach (var reelWindowName in reelWindows) {
                List<string> reelsForNextSpin = new(); // When feature trigger current state will be other than HnS hence empty list.
                if (context.GetCurrentState() == HnsStateString) {
                    reelsForNextSpin = ReelSetsFeatureAccess.GetReelStripsData(context, HnsStateString, reelWindowName, betLevel).ToList();
                }

                // Reelstrip name based on number of locked cors
                var lockCount = CorsFeatureAccess.GetPrizesCollectedCount(context);
                var usableLockCount = lockCount > Constants.ReelStripNameIndexMax
                    ? Constants.ReelStripNameIndexMax
                    : (lockCount < Constants.ReelStripNameIndexMin ? Constants.ReelStripNameIndexMin : lockCount);

                // Make sure each cell has a default Reelstrip
                var cellsCount = GameConstants.WindowMaxHeight * GameConstants.WindowMaxWidth;
                while (reelsForNextSpin.Count < cellsCount) {
                    reelsForNextSpin.Add(Constants.DefaultReelStripName);
                }

                // Update reelsets for cells that are not locked or hidden
                var reelstrip = string.Format(Constants.ReelStripNameFormat, usableLockCount);
                var lockedPrizes = CorsFeatureAccess.GetPrizePositionsWorldIndex(context);
                for (int cell = 0; cell < cellsCount; cell++) {
                    if (!lockedPrizes.Contains(cell) && !context.HiddenWindowCells[cell]) {
                        reelsForNextSpin[cell] = reelstrip;
                    }
                }

                ReelSetsFeatureAccess.UpdateReelStripsData(context, HnsStateString, reelWindowName, betLevel, reelsForNextSpin.ToArray());
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
