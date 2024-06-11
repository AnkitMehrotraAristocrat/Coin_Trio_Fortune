using System.Collections.Generic;
using System.Linq;
using Milan.Common.Implementations.DTOs;
using GameBackend.Data;
using System.Threading.Tasks;
using Milan.StateMachine.Core;
using Milan.XSlotEngine.Core.Helpers;

namespace GameBackend
{
    /// <summary>
    /// Defines the MathVerificationSpin service for the SlotGameBackend
    /// </summary>
    public partial class SlotGameBackend
    {
        public async Task<BackendServiceResponse> MathVerificationSpin(MilanRequest request)
        {
            BackendServiceResponse response = new() {
                IsSuccess = false,
                Value = null
            };

            var spinRequest = request.GetArguments<MathVerificationSpinRequest>(request.Payload.ToString());
            request.ServicePayloads.Add(GameConstants.UserIdPayloadName, spinRequest.UserId);
            request.ServicePayloads.Add(GameConstants.TableIdPayloadName, 0);

            #if !JACKPOTS_OFF
            request.ServicePayloads.Add(GameConstants.JackpotApplicationIdPayloadName, spinRequest.JackpotEngine.applicationId);
            request.ServicePayloads.Add(GameConstants.JackpotEngineUrlPayloadName, spinRequest.JackpotEngine.url);
            #endif

            var gameContext = await CreateGameContext(request);
            gameContext.BetOperations.MultiplierIndex = spinRequest.BetIndex;

            // If no lineIndex is provided the configured CurrentBetLineIndex will be used by default
            gameContext.BetOperations.BetLineIndex = spinRequest.LineIndex;

            StateMachine<GameContext> stateMachine = await CreateStateMachine(gameContext);
            if (gameContext.PersistentData.TriggeredStates.Queue.Count() == 0) {
                stateMachine.Workflow.SetNextState(GameConstants.JoinStateName);
                await stateMachine.EvaluateAsync();
            }

            // Prepare the game's state machine and evaluate to generate new result payloads
            SetInitialState(gameContext, stateMachine, gameContext.GetNextState());
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
            response.ServicePayloads.Add(GameConstants.MetricsPayloadName, DTOHelper.CreatePayload(gameContext, gameContext.PersistentData.RtpFeatureName));

            // The BackendServiceResponse provides a property for reporting any rewards that were won to the client
            // This information may or may not be required by the caller since it may be relevant for wallet adjustments
            response.Rewards = new List<Currency>();
            foreach (var reward in gameContext.SpinData.Results.WonRewards) {
                response.Rewards.Add(new Currency() {
                    Name = reward.CurrencyType,
                    Value = reward.TotalWon
                });
            }

            response.IsSuccess = true;
            response.RoundComplete = gameContext.RoundData.RoundComplete;
            response.Value = null; //payloads (gameContext.Payloads) not required for math verification
            return response;
        }
    }
}
