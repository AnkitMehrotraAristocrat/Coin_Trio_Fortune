using GameBackend.Data;
using GameBackend.Features.Blackout.Data;
using System.Linq;

namespace GameBackend.Features.Blackout.Configuration
{
    public static class FeatureAccess
    {
        public static bool HasBlackout(GameContext gameContext, string stateName)
        {
            var boContext = gameContext.FeatureContext<BlackoutContext>();
            var reelWindow = gameContext.GetCurrentReelWindow();
            var prizeCount = boContext.OccupiedCells[stateName].Count;
            var hiddenCount = gameContext.HiddenWindowCells.Count(hidden => hidden);
            var cellCount = reelWindow.WindowSize.Width * reelWindow.WindowSize.Height - hiddenCount;
            return prizeCount == cellCount;
        }

        public static void AddOccupiedCells(GameContext gameContext, int[] cells, string stateName)
        {
            var boContext = gameContext.FeatureContext<BlackoutContext>();
            if (!boContext.OccupiedCells.ContainsKey(stateName)) {
                boContext.OccupiedCells[stateName] = new();
            }
            foreach (var cell in cells) {
                if (!boContext.OccupiedCells[stateName].Contains(cell)) {
                    boContext.OccupiedCells[stateName].Add(cell);
                }
            }
        }
    }
}
