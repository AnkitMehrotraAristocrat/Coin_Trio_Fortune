using GameBackend.Data;
using GameBackend.Features.SymbolSkins.Data;
using System.Collections.Generic;
using System.Linq;

namespace GameBackend.Features.SymbolSkins.Configuration
{
    /// <summary>
    /// Used to provide access to other features
    /// </summary>
    public static class FeatureAccess
    {
        public static void AddOutcomeData(GameContext gameContext, string id, bool shouldMultiply, string skin, int symbolId, string textValue, bool canAward, int tier, int worldIndex)
        {
            var ssContext = gameContext.FeatureContext<SymbolSkinsContext>();
            var data = ssContext.SymbolOutcomeTrackingData;

            var symbolData = new SkinData() {
                ShouldMultiply = shouldMultiply,
                Skin = skin,
                SymbolId = symbolId,
                TextValue = textValue
            };

            var outcomeData = data.Find(item => item.Id == id);
            if (outcomeData == null) {
                outcomeData = new OutcomeTracking { Id = id };
                data.Add(outcomeData);
            }
            outcomeData.SymbolOutcomeData.Add(new OutcomeTrackingData() {
                CanAward = canAward,
                Tier = tier,
                SymbolData = symbolData,
                WorldIndex = worldIndex
            });
        }

        public static void AddSpinningData(GameContext gameContext, string id, string[] gameStates, bool shouldMultiply, string skin, int symbolId, string textValue)
        { 
            var ssContext = gameContext.FeatureContext<SymbolSkinsContext>();
            var data = ssContext.SymbolSpinningTrackingData;

            var spinningData = data.Find(item => item.Id == id);
            if (spinningData == null) {
                spinningData = new SpinningTrackingData { Id = id };
                data.Add(spinningData);
            }

            var symbolData = new SkinData() {
                ShouldMultiply = shouldMultiply,
                Skin = skin,
                SymbolId = symbolId,
                TextValue = textValue
            };
            var symbolSpinningGameStateData = new SpinningGameStateData() {
                GameStates = gameStates.ToList(),
                Data = new List<SkinData>()
            };
            var symbolSpinningIdData = new SpinningIdData() {
                SymbolId = symbolId,
                Data = new List<SpinningGameStateData>()
            };

            symbolSpinningGameStateData.Data.Add(symbolData);
            symbolSpinningIdData.Data.Add(symbolSpinningGameStateData);
            spinningData.Data.Add(symbolSpinningIdData);
        }
    }
}
