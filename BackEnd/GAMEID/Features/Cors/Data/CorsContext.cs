using SymbolSkinsFeatureAccess = GameBackend.Features.SymbolSkins.Configuration.FeatureAccess;
using GameBackend.Data;
using GameBackend.Helpers;
using System.Collections.Generic;
using System.Linq;
using GameBackend.Features.Cors.Configuration;

namespace GameBackend.Features.Cors.Data
{
    public class CorsContext
    {
        public List<FlexibleStop> LandedCorSymbols { get; set; } = new();

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public static HashSet<int> GetPrizePositionsWorldIndex(GameContext context)
        {
            var collection = context.FeatureRoundData<CorsRoundData>().PrizesCollected;
            var positions = new HashSet<int>();
            foreach (CorPrizeInfo prize in collection.Prizes) {
                positions.Add(prize.Stop.WorldIndex);
            }
            return positions;
        }

        public static void SaveSymbolSkinOutcomeData(GameContext context, List<CorPrizeInfo> corPrizes, string state)
        {
            var shouldMultiply = false;
            string symbolSkin;

            foreach (var prize in corPrizes) 
            {
                var corSymbolId = prize.Stop.StopSymbol.Id;
                var tier = prize.Tier;
                var canAward = prize.Awarded;
                var symbolId = corSymbolId;
                var skin = prize.Type == GameConstants.JackpotPrizeType ? prize.Name : GameConstants.MultiplierPrizeType;
                var textValue = prize.Value.ToString();
                SymbolSkinsFeatureAccess.AddOutcomeData(context, state, shouldMultiply, skin, symbolId, textValue, canAward, tier, prize.Stop.WorldIndex); 
            
                if(context.GetCurrentState() == "BaseSpin")
                {
                    symbolSkin = GeneralHelper.GetRandomEntryFromTable<string>(context, Constants.ScatSelection);

                    if(symbolSkin == "SCAT_RED")
                        context.hasDragonLanded = true;
                    if(symbolSkin == "SCAT_GREEN")
                        context.hasTigerLanded = true;
                    if(symbolSkin == "SCAT_BLUE")
                        context.hasKoiLanded = true;
                }
            }
        }

        public static void SaveSymbolSkinSpinningData(GameContext context, string state)
        {
            var windows = GameConstants.StateReelWindows[GeneralHelper.GetGameStateEnum(state)];   
            var totalBet = BetHelper.GetTotalBet(context).ToString();
            var shouldMultiply = false;
            foreach (var reelWindowId in windows) {
                if (ShouldExcludeWindow(reelWindowId)) {
                    continue;
                }
                foreach (var corSymbol in Constants.CorSymbols) {
                    if (ShouldExcludeSymbol(reelWindowId, corSymbol)) {
                        continue;
                    }
                    string[] states = GetStates(corSymbol);
                    int symbolId = context.MappedConfigurations.Symbols.First(s => s.Name == corSymbol).Id;

                    string textValue;
                    string skin = GameConstants.MultiplierPrizeType;
                    if (!ShouldExcludeSkin(reelWindowId, corSymbol, skin)) {
                        textValue = totalBet;
                        SymbolSkinsFeatureAccess.AddSpinningData(context, reelWindowId, states, shouldMultiply, skin, symbolId, textValue);
                    }

                    textValue = string.Empty;
                    foreach (var jackpot in GameConstants.JackpotTiers) {
                        skin = jackpot.Key;
                        if (!ShouldExcludeSkin(reelWindowId, corSymbol, skin)) {
                            SymbolSkinsFeatureAccess.AddSpinningData(context, reelWindowId, states, shouldMultiply, skin, symbolId, textValue);
                        }
                    }
                }
            }
        }

        private static string[] GetStates(string symbol)
        {
            var allStates = GeneralHelper.GetGameStatesArray();
            var states = new List<string>();
            foreach (var state in allStates) {
                if (!ShouldExcludeState(symbol, state)) {
                    states.Add(state);
                }
            }
            return states.ToArray();
        }

        private static bool ShouldExcludeWindow(string window)
        {
            return Constants.SkinExclusions.Any(item => item.Window == window && item.Symbol == null && item.Skin == null);
        }

        private static bool ShouldExcludeSymbol(string window, string symbol)
        {
            return Constants.SkinExclusions.Any(item => item.Window == window && item.Symbol == symbol && item.Skin == null)
                || Constants.SkinExclusions.Any(item => item.Window == null && item.Symbol == symbol && item.Skin == null);
        }

        private static bool ShouldExcludeSkin(string window, string symbol, string skin)
        {
            return Constants.SkinExclusions.Any(item => item.Window == window && item.Symbol == symbol && item.Skin == skin)
                || Constants.SkinExclusions.Any(item => item.Window == null && item.Symbol == symbol && item.Skin == skin)
                || Constants.SkinExclusions.Any(item => item.Window == null && item.Symbol == null && item.Skin == skin);
        }

        private static bool ShouldExcludeState(string symbol, string state)
        {
            return Constants.StateExclusions.Any(item => item.Symbol == symbol && item.State == state)
                || Constants.StateExclusions.Any(item => item.Symbol == null && item.State == state);
        }
    }
}
