using RoundModelRoundData = Wildcat.Milan.Shared.Dtos.Backend.Round.RoundModelRoundData;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wildcat.Milan.Shared.Dtos.Backend;
using Milan.Common.Implementations.DTOs;
using GameBackend.Helpers;
using System.Runtime.CompilerServices;
using NewRelic.Api.Agent;

namespace GameBackend
{
    /// <summary>
    /// Defines the Spin service for the SlotGameBackend.
    /// </summary>
    public partial class SlotGameBackend
    {
        /// <summary>
        /// Generates new spin result payloads based on the current state of the backend and arguments provided by the
        /// client.
        /// </summary>
        /// <param name="request">
        /// A SpinRequest that provides relevant information related to a basic spin operation.
        /// </param>
        /// <returns>
        /// A SpinResponse object containing the collection of payload information required for processing by the game's
        /// frontend.
        /// </returns>
        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<BackendServiceResponse> Spin(MilanRequest request)
        {
            var response = new WildcatBackendSpinServiceResponse { 
                IsSuccess = false, 
                Value = null 
            };

            var gameContext = await CreateGameContext(request);
            var spinRequest = request.GetArguments<SpinRequest>(request.Payload.ToString());
            gameContext.BetOperations.MultiplierIndex = spinRequest.BetIndex;
            gameContext.BetOperations.BetLineIndex = spinRequest.LineIndex;

            // This game's spin request provides the ability to specify a queue of values to use before generating
            // random numbers when requested. 
            if (spinRequest.RandomNumberQueue != null) {
                gameContext.PersistentData.RandomNumberQueue.Clear();
                foreach (var val in spinRequest.RandomNumberQueue) {
                    gameContext.PersistentData.RandomNumberQueue.Enqueue(val);
                }
            }

            // prepare the game's state machine and evaluate to generate new result payloads
            var stateMachine = await CreateStateMachine(gameContext);
            await stateMachine.EvaluateAsync();

            // The BackendServiceResponse provides a property for reporting any bets or bets that were made back
            // to the client. This information may or may not be required by the caller since it may be relevant for
            // wallet adjustments.
            //
            // Note that the bet may be of any currency type. This backend doesn't expect to change the currency
            // from the default, but if you would like to you should retrieve that Value from
            // gameContext.BetOperations.CurrencyType if it has been set there.
            var currency = gameContext.GetBetCurrencyType();
            var bet = new Currency() {
                Name = currency,
                Value = gameContext.XSlotConfigurationProvider
                    .MappedConfigurations
                    .BetItems
                    .First(w => w.CurrencyType == currency)
                    .TotalBet
            };
            response.Bets = new List<Currency>() { bet };

            // The BackendServiceResponse provides a property for reporting any rewards that were won
            // to the client. This information may or may not be required by the caller since it may be relevant for
            // wallet adjustments.
            response.Rewards = new List<Currency>();
            foreach (var reward in gameContext.SpinData.Results.WonRewards) {
                response.Rewards.Add(new Currency() {
                    Name = reward.CurrencyType,
                    Value = reward.TotalWon
                });
            }

            response.Payout.Add(new PayoutResponse {
                Receipt = gameContext.SpinGuid,
                //TotalPayout = gameContext.RoundData.RoundComplete ? gameContext.RoundData.TotalWin : 0,
                TotalPayout = gameContext.RoundData.RoundComplete ? (gameContext.RoundData.TotalRewardsWon.LastOrDefault()?.TotalWon ?? 0) : 0,
                CurrencyType = GameConstants.PayoutCreditType
            });

            // Update the response and attach the response payloads required by the game's frontend for processing
            response.IsSuccess = true;
            response.RoundComplete = gameContext.RoundData.RoundComplete;
            response.Value = gameContext.Payloads;

            response.RoundModel = gameContext.FeatureRoundData<RoundModelRoundData>();
            var bsState = GeneralHelper.GetGameStateString(GameStates.BaseSpin);
            response.IsFreeSpin = !(gameContext.Transition.FromState == bsState && gameContext.Transition.ToState == bsState);
            response.SlotEventId = gameContext.SpinGuid;
            response.GaffeId = gameContext.PersistentData?.GaffeData?.Name;
            return response;
        }
    }
}
