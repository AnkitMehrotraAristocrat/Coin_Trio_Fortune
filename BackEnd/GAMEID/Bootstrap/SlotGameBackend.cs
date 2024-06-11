using System;
using System.Collections.Generic;
using System.Composition;
using System.Reflection;
using GameBackend.Data;
using GameBackend.Helpers;
using Milan.Common.Implementations.Metadata;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Utilities;
using Milan.StateMachine.Core;
using Milan.Common.Implementations.DTOs;
using Milan.Common.Implementations.Entities;
using System.Threading.Tasks;
using NewRelic.Api.Agent;

using System.Runtime.CompilerServices;


#if !JACKPOTS_OFF
using Milan.Shared.DTO.Jackpot;
#endif

namespace GameBackend
{
    /// <summary>
    /// Milan compatible backend implementation
    /// </summary>
    [Export(typeof(IBackend))]
    public partial class SlotGameBackend : ExportableBackendServices, IBackend
    {
        // Deprecated
        public BackendContext BackendContext { get; set; }

        // Collection of services exposed by the game backend
        public Dictionary<string, BackendService> BackendServices { get; set; }

        // Backend metadata that may be queried by the host or another plugin managed by the Host
        public BackendMetadata Metadata { get; set; }

        // Creates and initializes a new instance of the backend
        public SlotGameBackend()
        {
            Metadata = new BackendMetadata
            {
                BackendId = GameConstants.GameId,
                Name = GameConstants.GameId,
                Provider = GameConstants.Provider,
                Version = Assembly.GetExecutingAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            };
            ExportBackendServices();
        }

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async Task<StateMachine<GameContext>> CreateStateMachine(GameContext gameContext, string gameState = null)
        {
            try
            {
                gameState = !string.IsNullOrEmpty(gameState) ? gameState : gameContext.GetCurrentState();
                await gameContext.XSlotConfigurationProvider.InitializeStateMachineConfigurations();

                var states = gameContext.XSlotConfigurationProvider.StateMachineConfiguration;
                var stateMachine = new StateMachine<GameContext>(states, Assembly.GetExecutingAssembly().GetName(), gameContext);
                stateMachine.Workflow.SetNextState(gameState);
                stateMachine.Context = gameContext;
                return stateMachine;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(string.Format(GameConstants.ErrorCreateStateMachineFormat, ex.Message));
                throw;
            }
        }

        [Trace]
        [MethodImpl(MethodImplOptions.NoInlining)]
        private async Task<GameContext> CreateGameContext(MilanRequest request)
        {
            try
            {
                var persistentData = request.GetPersistentData<GamePersistentData>();
                var gameContext = new GameContext(persistentData.CurrentState);
                await gameContext.InitializeXSlotConfigurations(request.Context.ConfigurationProvider);

                var roundData = request.GetRoundData<GameRoundData>();

                gameContext.CustomConfigurations = new Configurations(request.Context.ConfigurationProvider);
                await gameContext.CustomConfigurations.InitializeConfigurations();
                gameContext.RoundData = roundData;
                gameContext.PersistentData = persistentData;
                gameContext.SpinGuid = Guid.NewGuid();

                // WILD: userid is now string
                string userId = request.GetServicePayload<string>(GameConstants.UserIdPayloadName);

#if !JACKPOTS_OFF
                var jackpotApplicationId = Convert.ToUInt64(request.ServicePayloads["jackpotApplicationId"]);
                var jackpotTemplateId = Convert.ToInt32(request.ServicePayloads["jackpotTemplateId"]);

                gameContext.ConfiguredOptions.JackpotEngineUrl = request.ServicePayloads["jackpotEngineUrl"].ToString();
                gameContext.JackpotOperations.Templates = new List<TemplateData>() {
                    new TemplateData()
                    {
                        TemplateId = jackpotTemplateId,
                        QualifyToWin = false
                    }
                };

                gameContext.JackpotOperations.ApplicationId = jackpotApplicationId;
                // WILD: let's use this TemplateId property instead
                gameContext.JackpotOperations.TemplateId = jackpotTemplateId;
                gameContext.JackpotOperations.UserId = userId;
#endif

                // WILD: user Id has been added to context
                gameContext.UserId = userId;

                return gameContext;
            }
            catch (Exception ex)
            {
                DebugHelper.LogError(string.Format(GameConstants.ErrorCreateGameContextFormat, ex.Message));
                throw;
            }
        }

        private static void SetInitialState(GameContext gameContext, StateMachine<GameContext> stateMachine, string state)
        {
            stateMachine.Workflow.SetNextState(state);
            gameContext.PersistentData.CurrentState = state;
        }
    }
}