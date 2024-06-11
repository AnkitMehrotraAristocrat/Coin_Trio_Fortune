using System.Linq;
using GameBackend.Data;

namespace GameBackend.Helpers
{
    public static class BetHelper
    {
        public static ulong GetTotalBet(GameContext context)
        {
            string defaultCurrency = context.GetBetCurrencyType();
            ulong totalBet = context.MappedConfigurations.BetItems.ToList().Find(x => x.CurrencyType == defaultCurrency).TotalBet;
            return totalBet;
        }

        public static int[] GetBetIndices(GameContext context)
        {
            var currency = context.XSlotConfigurations.BetConfiguration.DefaultCurrency;
            var betCurrency = context.MappedConfigurations.BetItems.First(w => w.CurrencyType == currency);
            var betMultipliersU = betCurrency.MultiplierIndexes.Multipliers.ToArray();

            int[] betIndicesL = new int[betMultipliersU.Length];
            for (int i = 0; i < betMultipliersU.Length; ++i) {
                betIndicesL[i] = i;
            }
            return betIndicesL;
        }

        public static ulong GetBetMultiplier(GameContext context)
        {
            var currency = context.XSlotConfigurations.BetConfiguration.DefaultCurrency;
            var betCurrency = context.MappedConfigurations.BetItems.First(w => w.CurrencyType == currency);
            var multiplierIndex = context.BetOperations.MultiplierIndex;
            return betCurrency.MultiplierIndexes.Multipliers[multiplierIndex];
        }

        public static class BetHelpers
        {
            public static int FindCeilingIndex(ulong betAmount, ulong[] betLevels)
            {
                for (var i = 0; i < betLevels.Length; ++i)
                {
                    if (betLevels[i] >= betAmount)
                        return i;
                }

                return 0;
            }
        }
    }
}
