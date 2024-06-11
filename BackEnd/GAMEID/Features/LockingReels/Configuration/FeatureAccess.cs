using GameBackend.Data;
using GameBackend.Features.LockingReels.Data;
using System.Linq;

namespace GameBackend.Features.LockingReels.Configuration
{
    public static class FeatureAccess
    {
        public static void AddLockingReelsData(GameContext gameContext, string state, int[] reels)
        {
            var lrContext = gameContext.FeatureContext<LockingReelsContext>();
            if (lrContext.LockingReelsData.Data.ContainsKey(state)) {
                lrContext.LockingReelsData.Data[state].AddRange(reels);
            }
            else {
                lrContext.LockingReelsData.Data[state] = reels.ToList();
            }
        }
    }
}
