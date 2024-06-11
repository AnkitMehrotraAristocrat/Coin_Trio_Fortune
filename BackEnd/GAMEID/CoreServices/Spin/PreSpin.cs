using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using GameBackend.Data;
using Milan.Common.Implementations.DTOs;
using Milan.XSlotEngine.Core.Extensions;
using Milan.XSlotEngine.Interfaces.Core.BetManager;
using NewRelic.Api.Agent;
using Wildcat.Milan.Shared.Dtos.Backend;

namespace GameBackend
{
    public partial class SlotGameBackend
    {
        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<BackendServiceResponse> PreSpin(MilanRequest request)
        {
            var response = new WildcatBackendPreSpinResponse {
                IsSuccess = false,
                Value = null
            };
            
            var gameContext = await CreateGameContext(request);
            var prespinRequest = request.GetArguments<WildcatBackendPreSpinRequest>(request.Payload.ToString());

            // Get the base cost for a spin and the bet Multipliers for this game
            gameContext.BetOperations.CurrencyType = gameContext.GetBetCurrencyType();
            gameContext.BetOperations.BetLineIndex = gameContext.RoundData.LineIndex = GameConstants.DefaultBetLineIndex;

            var betItems = gameContext.MappedConfigurations.BetItems;
            var betCurrency = betItems.First(w => w.CurrencyType == gameContext.BetOperations.CurrencyType);
            var baseCost = GetBaseCost(gameContext, betCurrency);
            var betMultipliers = betCurrency.MultiplierIndexes.Multipliers;

            response.RemainingFreeSpins = gameContext.GetRemainingFreeSpins();
            response.BetAmount = baseCost * betMultipliers[prespinRequest.BetIndex];
            response.AllowSpin = prespinRequest.CurrentChipAmount >= response.BetAmount || response.RemainingFreeSpins > 0;

            gameContext.Payloads.Clear();
            gameContext.Payloads.AddPayload(GameConstants.PrespinPayloadName, response);

            response.IsSuccess = true;
            response.Value = new Dictionary<string, IList<string>>(gameContext.Payloads);
            return response;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        
        public static ulong GetBaseCost(GameContext gameContext, IBetItem betCurrency)
        {
            #if WAYS_GAME
            return betCurrency.ReelStripCostIndexes.GetTotalReelStripCost();
            #else //LINES_GAME
            return betCurrency.BetLineIndexes.BetLines[gameContext.BetOperations.BetLineIndex].TotalLineCost;
            #endif
        }
    }
}
