using Milan.Common.Implementations.DTOs;
using Milan.XSlotEngine.Core.Extensions;
using System.Threading.Tasks;
using GameBackend.Data;
using Milan.Common.Logging;
using GameBackend.Helpers;
using GameBackend.UnitTests;
using Wildcat.Milan.Shared.Dtos.Backend;
using Wildcat.Milan.Shared.Dtos.Host;
using static GameBackend.Helpers.BetHelper;
using NewRelic.Api.Agent;
using System.Runtime.CompilerServices;

namespace GameBackend
{
    /// <summary>
    /// Defines the Join service for the SlotGameBackend
    /// </summary>
    public partial class SlotGameBackend
    {
        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        public async Task<BackendServiceResponse> Join(MilanRequest request)
        {
            var response = new WildcatBackendServiceResponse {
                IsSuccess = false,
                Value = null
            };

            var gameContext = await CreateGameContext(request);
            gameContext.PersistentData.RandomNumberQueue.Clear();

            ProcessDynamicBet(request, gameContext);

            var joinResponse = new JoinResponse();
            var stateMachine = await CreateStateMachine(gameContext, GameConstants.JoinStateName);
            await stateMachine.EvaluateAsync();

            joinResponse.configs = gameContext.ConfigPayload;
            joinResponse.payloads = gameContext.JoinPayload;
            joinResponse.currentState = gameContext.GetCurrentState();
            gameContext.Payloads.AddPayload(GameConstants.JoinPayloadName, joinResponse);

            // Update the response and attach the response payloads required by the game's frontend for processing
            response.IsSuccess = true;
            response.RoundComplete = gameContext.RoundData.RoundComplete;
            response.Value = gameContext.Payloads;
            // WILD: now required when returning any WildcatBackendServiceResponse for telemetry
            response.SlotEventId = gameContext.SpinGuid;

            TestPositionInterpreters.RunAllTests();
            return response;
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        private static void ProcessDynamicBet(MilanRequest request, GameContext context)
        {
            string requestPayload = request.Payload.ToString();
            if (!requestPayload.Contains(GameConstants.DynamicBetAmountFieldName) || !requestPayload.Contains(GameConstants.UsedDynamicBetFormulaFieldName)) {
                return;
            }

            JoinRequest joinRequest = request.GetArguments<JoinRequest>(requestPayload);
            if (context.GetCurrentState().Equals(GeneralHelper.GetGameStateString(GameStates.BaseSpin))) {
                // if we are in the BaseSpin state, update the BaseBetIndex to the supplied DynamicBetAmount
                ulong[] betLevels = context.GetBetAmounts();
                int betIndex = BetHelpers.FindCeilingIndex(joinRequest.DynamicBetAmount, betLevels);
                context.PersistentData.BaseBetIndex = betIndex;
            }
            else if (joinRequest.UsedDynamicBetFormula) {
                // otherwise, if braincloud UsedDynamicBetFormula we need to notify them we didn't use it as we are not in a base state
                ApplicationLogger.LogInfo<JoinLog>(string.Format(GameConstants.ErrorDynamicBetLevelFormat, joinRequest.DynamicBetAmount));
            }
        }

        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////

        public class JoinLog
        {
        }

        public class JoinRequest
        {
            public ulong DynamicBetAmount { get; set; }
            public bool UsedDynamicBetFormula { get; set; }
        }
    }
}
