using GameBackend.Data;
using GameBackend.Features.Cors.Data;
using GameBackend.Helpers;
using Milan.XSlotEngine.Core.Configurations.XSlotConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameBackend.Features.Cors.Configuration
{
    /// <summary>
    /// Used to provide access to other features
    /// </summary>
    public static class FeatureAccess
    {
        public static int GetPrizesCollectedCount(GameContext gameContext)
        {
            return gameContext.FeatureRoundData<CorsRoundData>().PrizesCollected.Prizes.Count;
        }

        public static int GetLandedCorsCount(GameContext gameContext)
        {
            return gameContext.FeatureContext<CorsContext>().LandedCorSymbols.Count;
        }

        public static int GetPositionInReelStrip(GameContext gameContext, int cell, ReelStripConfiguration reelStrip)
        {
            var prize = gameContext.FeatureRoundData<CorsRoundData>().PrizesCollected.Prizes.Find(p => p.Stop.WorldIndex == cell);
            var pos = reelStrip.Stops.IndexOf(reelStrip.Stops.FirstOrDefault(x => x.Symbol == prize.Stop.StopSymbol.Name));
            return pos;
        }

        public static HashSet<int> GetPrizePositionsWorldIndex(GameContext gameContext)
        {
            return CorsContext.GetPrizePositionsWorldIndex(gameContext);
        }

        public static ulong AwardMultipliers(GameContext context)
        {
            ulong total = 0;
            CorPrizes data = context.FeatureRoundData<CorsRoundData>().PrizesCollected;
            foreach (CorPrizeInfo prize in data.Prizes) {
                if (prize.Type == GameConstants.MultiplierPrizeType) {
                    total += Convert.ToUInt64(prize.Value);
                    prize.Awarded = true;
                }
            }
            return total;
        }

        public static async Task<ulong> AwardJackpotsAsync(GameContext context)
        {
            ulong total = 0;
            CorPrizes data = context.FeatureRoundData<CorsRoundData>().PrizesCollected;
            foreach (CorPrizeInfo prize in data.Prizes) {
                if (prize.Type != GameConstants.JackpotPrizeType) {
                    continue;
                }
                ulong win = await JackpotHelper.AwardJackpotAsync(context, prize.Tier, GameConstants.JackpotID, prize.JackpotBaseMultiplier);
                prize.Value = win;
                total += win;
                prize.Awarded = true;
            }
            return total;
        }
    }
}
