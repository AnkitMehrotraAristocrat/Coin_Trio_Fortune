using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Milan.Common.Implementations.DTOs;
using Milan.Common.Implementations.Entities;
using Milan.Common.Implementations.Metadata;
using Milan.Common.Interfaces.DTOs;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Interfaces.Utilities;
using Milan.Common.Serializer;
using Milan.Infrastructure.Components;
using Milan.Shared.DTO.Jackpot;
using NewRelic.Api.Agent;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProductMadness.Phoenix.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Wildcat.Milan.Game.Core.JackpotEngine.Contracts;
using Wildcat.Milan.Game.Core.JackpotEngine.Facade;
using Wildcat.Milan.Game.Core.Utilities;
using Wildcat.Milan.Host.Core.Models;
using Wildcat.Milan.Host.Core.Utilities;
using Wildcat.Milan.Host.Core.Validation;
using Wildcat.Milan.Host.Utilities;
using Wildcat.Milan.Shared.Dtos.Backend;
using Wildcat.Milan.Shared.Dtos.Host;
using Wildcat.Milan.Shared.Dtos.Session;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Wildcat.Milan.Host.Core.Controllers
{
    /// <summary>
    /// A simple development mode Service Adapter for use when developing or testing backends with the Milan Host.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public partial class ServiceCore :
        ControllerBase
    {


        #region Fields

        private readonly IComponentManager _components;
        private readonly IConfigurationManager _configurationManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ServiceCore> _logger;
        private readonly IHostVersionHelper _hostVersionHelper;

        private static AdapterConfiguration ADAPTER_CONFIGURATION;
        private static string JackpotEngineAllUrl { get; set; }
        private const string JoinBackendArgumentVariationKey = "variation";
        private const string JoinBackendArgumentJackpotTemplateIdKey = "jackpot_template_id";
        private const string JoinBackendArgumentSlotName = "slot_name";
        private const string JoinBackendArgumentSlotId = "slot_id";

#nullable enable
        private static string? JACKPOT_ENGINE_URL;
        private readonly BackendContext _context;
        private readonly IBackend _backend;
        private readonly IStorage _storage;
        private readonly GameVersionModel _gameVersionDetails;

        // K_SERVICE is GCR environment variable that contains the service name that the container is running under.
        private static readonly string CloudRunServiceName =
            Environment.GetEnvironmentVariable("K_SERVICE") ?? string.Empty;
#nullable disable

        #endregion

        #region Properties

        /// <summary>
        /// Contains the adapter's metadata for reference by the host or any other plugin that might interact with the
        /// adapter.
        /// </summary>
        private static AdapterMetadata Metadata { get; set; }
        public Microsoft.Extensions.Configuration.IConfiguration AppConfiguration { get; }



        #endregion

        /// <summary>
        /// Creates and initializes a new instance of the Adapter controller.
        /// </summary>
        /// <param name="components">
        /// The components provided by the Host that are available to the service adapter.
        /// </param>
        /// <param name="configurationManager">
        /// The configuration manager that provides access to all configuration files
        /// related to the backends that the Host is currently hosting.
        /// </param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger">A logger for use by the service adapter.</param>
        [ImportingConstructor]
        public ServiceCore(
            Microsoft.Extensions.Configuration.IConfiguration appConfiguration,
            IComponentManager components,
            IConfigurationManager configurationManager,
            IHttpClientFactory httpClientFactory,
            ILogger<ServiceCore> logger,
            IBackend backend,
            IStorage storage,
            IHostVersionHelper hostVersionHelper
        )
        {
            ArgumentNullException.ThrowIfNull(backend);

            AppConfiguration = appConfiguration;
            _components = components;
            _configurationManager = configurationManager;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

            // Load the environment config for the service adapter
            LoadAdapterConfiguration();

            _backend = backend;

            // Making sure the version comes from the Milan game backend assembly
            _backend.Metadata.Version = hostVersionHelper.GameVersion;

            _storage = storage;
            _context = new BackendContext();
            _context.SetComponents(new List<IBackend>() { _backend }, _components.GetInstances<IStorage>());
            _gameVersionDetails = _backend.GetGameVersionDetails(_configurationManager, hostVersionHelper);
            _hostVersionHelper = hostVersionHelper;
        }

        private void LoadAdapterConfiguration()
        {
            if (ADAPTER_CONFIGURATION != null)
            {
                return;
            }

            var infoVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            Metadata = new AdapterMetadata
            {
                Name = "Wildcat.ServiceAdapter",
                Provider = "ProducMadness",
                AdapterId = "Wildcat.ServiceAdapter",
                Version = infoVersion
            };

            //var adapterPath = $"{Directory.GetCurrentDirectory()}/plugins/adapters/JackpotCoveDevServiceAdapter";

            //var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            //var builder = new ConfigurationBuilder()
            //    .SetBasePath(adapterPath)
            //    .AddJsonFile("config.json", true, true)
            //    .AddJsonFile($"config.{environment}.json", true, true)
            //    .AddEnvironmentVariables();

            //var config = builder.Build();

            //ADAPTER_CONFIGURATION = config.Get<AdapterConfiguration>();
            ADAPTER_CONFIGURATION = this.AppConfiguration.Get<AdapterConfiguration>();

            // TODO SD: moving global configurations to application configuraiton
            ADAPTER_CONFIGURATION.StoragePluginName = this.AppConfiguration.GetStoragePluginName();

            //JACKPOT_ENGINE_URL = ADAPTER_CONFIGURATION.jackpotEngine.url;
            var jackpotConfig = new JackpotConfiguration();
            this.AppConfiguration.Bind(ConfigurationExtentions.JACKPOT_ENGINE_CONFIG_KEY, jackpotConfig);
            ADAPTER_CONFIGURATION.jackpotEngine = jackpotConfig;
            JACKPOT_ENGINE_URL = jackpotConfig.url;
            //JackpotEngineAllUrl = JACKPOT_ENGINE_URL + "/api/jackpots/all";
            JackpotEngineAllUrl = JACKPOT_ENGINE_URL + "/api/v2/jackpots/all";

            _logger.LogInformation("Startup - DefaultRTP = {DefaultRtp}", ADAPTER_CONFIGURATION.DefaultRTP);
        }

        private void SetConfiguration(ServiceRequest request, SessionDataPayload session)
        {
            var originalVariation = session.GameMetaData.SlotGameVariation;
            if (request.ServiceId.Equals("Join", StringComparison.OrdinalIgnoreCase))
            {
                var json = JsonConvert.SerializeObject(request.BackendServiceArguments);
                var serviceArgs = NewtonsoftSerializer.DeserializeAutoAndReplace<Dictionary<string, string>>(json);

                string variation = null;
                string jackpotTemplateId = null;

                serviceArgs?.TryGetValue(JoinBackendArgumentVariationKey, out variation);
                serviceArgs?.TryGetValue(JoinBackendArgumentJackpotTemplateIdKey, out jackpotTemplateId);

                session.GameMetaData.SlotGameVariation = variation ?? ADAPTER_CONFIGURATION.DefaultRTP;
                session.GameMetaData.SlotName = serviceArgs?.GetValueOrDefault(JoinBackendArgumentSlotName);
                session.GameMetaData.SlotId = serviceArgs?.GetValueOrDefault(JoinBackendArgumentSlotId);
                session.GameMetaData.JackpotTemplateId = jackpotTemplateId != null ? Int32.Parse(jackpotTemplateId) : ADAPTER_CONFIGURATION.jackpotEngine.defaultTemplateId ?? -1;

                if (session.GameMetaData.SlotGameVariation.IsNullOrEmpty()) throw new InvalidOperationException("Unable to resolve variation for given game.");
                if (session.GameMetaData.JackpotTemplateId <= 0) throw new InvalidOperationException("Unable to resolve Jackpot Template Id for given game.");
            }

            // Provide only the specified backend's configuration data to the service being invoked.
            var configProvider = _configurationManager.ConfigurationProviders[request.BackendId];
            if (!string.IsNullOrEmpty(session.GameMetaData.SlotGameVariation))
            {
                _logger.LogRTPVariationSwitch(originalVariation, session.GameMetaData.SlotGameVariation, _backend.Metadata, request.PlayerId);
                configProvider.SetVariation(session.GameMetaData.SlotGameVariation);
            }

            _context.SetConfigurationProvider(configProvider);
        }

        /// <summary>
        /// A simple health check to verify that the adapter is responding to requests.
        /// </summary>
        /// <returns>Responds with 200 OK if the adapter is responding.</returns>
        //[HttpGet]
        private object HealthCheck()
        {
            return Ok();
        }

        //[HttpGet("Status")]
        //private async Task<ActionResult<ApiStatusResponse>> Status(CancellationToken ctx)
        //{
        //    var response = new ApiStatusResponse();

        //    if (!_context.Backend.IsAny())
        //    {
        //        _context.SetComponents(_components.GetInstances<IBackend>(), _components.GetInstances<IStorage>());
        //    }

        //    //var storage = _context.Storage.First(s => s.Metadata.Name == ADAPTER_CONFIGURATION.StoragePluginName);
        //    // IStorage doesn't support Status as it's only on SessionDataStorage
        //    if (_storage is ISessionDataStorage dataStorage)
        //    {
        //        var statusResponse = await dataStorage.Status();
        //        response.Statuses.Add(statusResponse);
        //    }

        //    using var client = _httpClientFactory.CreateClient("JackpotServiceClient");

        //    // We only need one template id so return the first one. Doesn't really matter which one it is
        //    var templateIds = ADAPTER_CONFIGURATION.jackpotEngine.gameJackpotTemplateMappings.SelectMany(x =>
        //        x.jackpotTargets.Values).Select(jt => jt.templateId).First();

        //    var requestJson = $"{{" +
        //                      $"\"application_id\":{ADAPTER_CONFIGURATION.jackpotEngine.applicationId}," +
        //                      $"\"user_id\":{ADAPTER_CONFIGURATION.userID}," +
        //                      $"\"template_ids\":[{templateIds}]" +
        //                      $"}}";

        //    using var httpResponse = await client.PostAsync(
        //            JackpotEngineAllUrl,
        //            new StringContent(requestJson, Encoding.ASCII, "application/json"), ctx);

        //    var jackpotServiceResponse = new StatusResponse(StatusType.JackpotService);

        //    if (!httpResponse.IsSuccessStatusCode)
        //    {
        //        jackpotServiceResponse.Message = await httpResponse.Content.ReadAsStringAsync(ctx);
        //        jackpotServiceResponse.HasErrored = true;
        //    }

        //    response.Statuses.Add(jackpotServiceResponse);

        //    if (response.Statuses.Any(x => x.HasErrored))
        //    {
        //        return Problem(JsonSerializer.Serialize(response), statusCode: 500);
        //    }

        //    return Ok();
        //}

        //[HttpGet("Version")]
        private VersionInfoResponse Version()
        {
            var (version, sha) = GetVersionInfo(Metadata.Version);

            return new VersionInfoResponse
            {
                Backend = new BackendVersionInfoResponse
                {
                    Name = Metadata.Name,
                    Version = version
                    //SHA = sha,
                },
                // WILD: this is new
                Game = _gameVersionDetails,
                ServiceAdapter = new ServiceAdapterResponse
                {
                    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                    StorageName = ADAPTER_CONFIGURATION.StoragePluginName,
                    GaffingEnabled = ADAPTER_CONFIGURATION.GaffingEnabled
                }
            };
        }

        private static (string, string) GetVersionInfo(string version)
        {
            var parts = version.Split("+");
            return parts.Length > 1 ? (parts[0], parts[1]) : (parts[0], "No SHA available");
        }

        private async Task<ActionResult<SpinResponse>> SpinInternal([FromBody] SpinRequest request)
        {
            // Perform generic ServiceRequest validation
            {
                var validationRequest = new ServiceRequestValidationRequest(request, _gameVersionDetails);
                if (await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None) is var validationResult && !validationResult.IsValid)
                {
                    return Problem(validationResult, request: request);
                }
            }
            // TODO: review usage of Serilog
            using var logEnricher = EnrichLogs.WithGlobalData(request, CloudRunServiceName);
            // EnrichNewRelicTransaction(request);

            // ---- Player Session ---- //

            var sessionKey = SessionKey.Create(_backend.Metadata, request.PlayerId);
            var sessionResult = await GetSession(sessionKey);
            if (!sessionResult.Success)
            {
                return Problem(sessionResult.ErrorMessage);
            }

            var session = sessionResult.Value;

            // ---- PreSpin ---- //

            request.ServiceId = "PreSpin";
            request.BackendServiceArguments = JToken.FromObject(new WildcatBackendPreSpinRequest
            {
                BetIndex = request.BetIndex,
                CurrentChipAmount = request.CurrentChipAmount
            });

            if (!_backend.BackendServices.TryGetValue(request.ServiceId, out var preSpinService))
            {
                return Problem($"Service {request.ServiceId} not found for backend {request.BackendId}", statusCode: 404, request);
            }

            var backendServiceResponse = await CallBackendRequest(request, preSpinService, sessionKey, session);

            if (backendServiceResponse is not WildcatBackendPreSpinResponse preSpinResponse)
            {
                return Problem($"PreSpin service must return a {nameof(WildcatBackendPreSpinResponse)}, and make sure your game backend project is including the shared JackpotCoveMeta.dll", statusCode: 500, request);
            }

            if (!preSpinResponse.IsSuccess)
            {
                _logger.LogSlotBackendError(request, preSpinResponse);
                return Problem($"Slot backend error '{preSpinResponse.Value}'.", statusCode: 500, request);
            }

            // Take into account that MaxBetForPlayerLevel can be negative 1. This is a temporary work around until a bug is fixed in BrainCloud
            if (preSpinResponse.RemainingFreeSpins == 0 && request.MaxBetForPlayerLevel != -1 && (long)preSpinResponse.BetAmount > request.MaxBetForPlayerLevel)
            {
                return Problem($"Max bet level validation failed. Max Bet Allowed: {request.MaxBetForPlayerLevel}, Bet Amount: {preSpinResponse.BetAmount}", ERROR_CODE_MAX_BET_LEVEL, statusCode: 422, request);
            }

            // Temporary log so that we don't lose this signal until the root cause is fixed.
            if (request.MaxBetForPlayerLevel == -1)
            {
                _logger.LogError("Received MaxBetForPlayerLevel -1 for {PlayerId}", request.PlayerId);
            }

            // Check if the slot should allow a spin
            if (!preSpinResponse.AllowSpin)
            {
                return Problem($"Insufficient funds. Current Balance: {request.CurrentChipAmount}, Bet Amount: {preSpinResponse.BetAmount}", ERROR_CODE_INSUFFICIENT_BALANCE, statusCode: 422, request);
            }

            // ---- Spin ---- //
            request.ServiceId = "Spin";
            request.BackendServiceArguments = JToken.FromObject(new WildcatBackendSpinRequest
            {
                BetIndex = request.BetIndex,
                LineIndex = request.LineIndex
            });

            if (!_backend.BackendServices.TryGetValue(request.ServiceId, out var spinService))
            {
                return Problem($"Service {request.ServiceId} not found for backend {request.BackendId}", statusCode: 404, request);
            }

            backendServiceResponse = await CallBackendRequest(request, spinService, sessionKey, session);

            if (backendServiceResponse is not WildcatBackendSpinServiceResponse spinResponse)
            {
                return Problem(
                    $"Backend service must return a {nameof(WildcatBackendSpinServiceResponse)}, and make sure your game backend project is including the shared JackpotCoveMeta.dll",
                    statusCode: 500);
            }

            if (!spinResponse.IsSuccess)
            {
                _logger.LogSlotBackendError(request, spinResponse);
                return Problem($"Slot backend error '{spinResponse.Value}'.", statusCode: 500, request);
            }

            // Update the session data for subsequent requests to use.
            var storageResult = await PersistData(sessionKey, session);

            if (!storageResult.Success)
            {
                return Problem($"Error when saving session data {storageResult.ErrorMessage} ", statusCode: 500, request);
            }

            var response = new SpinResponse()
            {
                Value = spinResponse.Value,
                Payout = spinResponse.Payout.ToArray(),
                IsSuccess = true,
                IsFreeSpin = spinResponse.IsFreeSpin,
                BetAmount = preSpinResponse.BetAmount,
                // WILD: now returning the round data for telemetry
                RoundComplete = spinResponse.RoundComplete,
                // WILD: now returning the round data for telemetry
                RoundModel = spinResponse.RoundModel,
                TelemetryData = new TelemetryDataContract()
                {
                    SlotSessionId = sessionKey.ToString(),
                    SlotSingleSessionId = sessionResult.Value.GameMetaData.SlotSingleSessionId,
                    SlotEventId = spinResponse.SlotEventId.ToString(),
                    SlotGameCode = _gameVersionDetails.GameId,
                    SlotId = sessionResult.Value.GameMetaData.SlotId,
                    SlotName = sessionResult.Value.GameMetaData.SlotName,
                    SlotGameVariation = sessionResult.Value.GameMetaData.SlotGameVariation,
                    SlotGameVersion = _gameVersionDetails.Version,
                    JackpotTemplateId = sessionResult.Value.GameMetaData.JackpotTemplateId,
                    GaffeId = spinResponse.GaffeId,
                    PlayerId = request.PlayerId
                }
            };

            if (spinResponse.Value is not Dictionary<string, IList<string>> valuesPayload)
            {
                return response;
            }

            if (valuesPayload.TryGetValue("jackpotWins", out var jackpotWinModel))
            {
                response.JackpotWins = jackpotWinModel.Select(win =>
                {
                    return JsonSerializer.Deserialize<JackpotWinData>(win);
                }
                ).ToList();
            }
            return response;
        }

        private async Task<ActionResult<GetJackpotsResponseModel>> GetJackpotValues([FromBody] GetJackpotsRequestModel request, CancellationToken cancellationToken)
        {
            var sessionKey = SessionKey.Create(_backend.Metadata, request.PlayerId);
            var sessionResult = await GetSession(sessionKey);
            if (!sessionResult.Success)
            {
                return Problem(sessionResult.ErrorMessage);
            }

            var jackpotRequest = new JackpotRequest()
            {
                ApplicationId = (ulong)ADAPTER_CONFIGURATION.jackpotEngine.applicationId,
                UserId = request.PlayerId,
                TemplateId = sessionResult.Value.GameMetaData.JackpotTemplateId
            };

            var jackpotEngineFacade = ServiceManagerExtension.GetService<IJackpotEngineFacade>();
            var jackpotResultExtra = await jackpotEngineFacade.GetJackpotAsync(jackpotRequest, CancellationToken.None);

            GetJackpotsResponseModel responseModel = new GetJackpotsResponseModel();
            responseModel.Jackpots = new Dictionary<string, JackpotInitData[]> {
                {
                    _backend.Metadata.BackendId, jackpotResultExtra.JackpotData.ToArray()
                }
            };

            return responseModel;
        }

        private static void EnrichNewRelicTransaction(ServiceRequest request)
        {
            var agent = NewRelic.Api.Agent.NewRelic.GetAgent();
            var transaction = agent.CurrentTransaction;
            transaction.AddCustomAttribute("GameId", request.BackendId);
            transaction.AddCustomAttribute("ServiceId", request.ServiceId);
            if (!string.IsNullOrWhiteSpace(request.PlayerId))
            {
                transaction.AddCustomAttribute("PlayerId", request.PlayerId);
            }
        }

        private async Task<ActionResult<ServiceResponse>> ProcessRequest(ServiceRequest request)
        {
            bool isJoinAction = request.ServiceId.Equals("join", StringComparison.OrdinalIgnoreCase);

            // Perform generic ServiceRequest validation
            {
                var validationRequest = new ServiceRequestValidationRequest(request, _gameVersionDetails);
                if (await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None) is var validationResult && !validationResult.IsValid)
                {
                    return Problem(validationResult, request: request);
                }
            }

            if (!_backend.BackendServices.TryGetValue(request.ServiceId, out var service))
            {
                return Problem($"Service '{request.ServiceId}' not found for backend '{request.BackendId}'", statusCode: StatusCodes.Status422UnprocessableEntity, request);
            }

            // Perform specific Join request validation
            if (isJoinAction)
            {
                var joinRequest = new JoinServiceRequest(request);
                var joinValidationRequest = new JoinSlotServiceValidationRequest(joinRequest, _gameVersionDetails);

                if (await new JoinServiceRequestValidator(this.AppConfiguration).ValidateAsync(joinValidationRequest, CancellationToken.None) is var joinValidationResult && !joinValidationResult.IsValid)
                {
                    return Problem(joinValidationResult, request: request);
                }
            }

            var sessionKey = SessionKey.Create(_backend.Metadata, request.PlayerId);
            var sessionResult = await GetOrCreateSession(sessionKey);
            if (!sessionResult.Success)
            {
                return Problem(sessionResult.ErrorMessage);
            }

            var session = sessionResult.Value;

            if (isJoinAction)
            {
                // Generate a single slot session id upon new JOIN
                session.GameMetaData.SlotSingleSessionId = Guid.NewGuid().ToString();
            }

            var serviceResponse = await CallBackendRequest(request, service, sessionKey, session);
            WildcatBackendServiceResponse backendServiceResponse = (WildcatBackendServiceResponse)serviceResponse;

            var response = new ServiceResponse()
            {
                Value = backendServiceResponse.Value,
                Payout = backendServiceResponse.Payout.ToArray(),
                IsSuccess = true,
                TelemetryData = new TelemetryDataContract()
                {
                    SlotSessionId = sessionKey.ToString(),
                    SlotSingleSessionId = sessionResult.Value.GameMetaData.SlotSingleSessionId,
                    SlotEventId = backendServiceResponse.SlotEventId.ToString(),
                    SlotGameCode = _gameVersionDetails.GameId,
                    SlotId = sessionResult.Value.GameMetaData.SlotId,
                    SlotName = sessionResult.Value.GameMetaData.SlotName,
                    SlotGameVariation = sessionResult.Value.GameMetaData.SlotGameVariation,
                    SlotGameVersion = _gameVersionDetails.Version,
                    // WILD: Jackpot Template Id is now store in the Milan session
                    JackpotTemplateId = sessionResult.Value.GameMetaData.JackpotTemplateId,
                    PlayerId = request.PlayerId
                }
            };

            // If this is anything other than Dictionary<string, list<string>> the client will blow up.

            if (!backendServiceResponse.IsSuccess)
            {
                _logger.LogSlotBackendError(request, backendServiceResponse);
                return Problem($"Slot backend error '{backendServiceResponse.Value}'.", statusCode: 500, request);
            }

            // Update the session data for subsequent requests to use.
            session.GameMetaData.SlotGameVariation = sessionResult.Value.GameMetaData.SlotGameVariation;
            // Save session
            var storageResult = await PersistData(sessionKey, session);

            if (!storageResult.Success)
            {
                return Problem($"Error when saving session data {storageResult.ErrorMessage} ", statusCode: 500, request);
            }

            return response;
        }

        private async Task<BackendServiceResponse> CallBackendRequest(
            ServiceRequest request,
            BackendService backendService,
            ISessionKey sessionKey,
            SessionDataPayload session)
        {
            SetConfiguration(request, session);

            var servicePayloads = new Dictionary<string, object>
            {
                { "userId", sessionKey.PlayerId },
                { "jackpotApplicationId", ADAPTER_CONFIGURATION.jackpotEngine.applicationId },
                { "jackpotEngineUrl", JACKPOT_ENGINE_URL },
                { "jackpotTemplateId", session.GameMetaData.JackpotTemplateId },
                { "gaffingEnabled", ADAPTER_CONFIGURATION.GaffingEnabled },
                { "tableId", (ulong?)null },
                { "variation", session.GameMetaData.SlotGameVariation }
            };

            // Invoke the game backend while providing the request arguments, the backend's context, and any existing
            // session data.
            var backendServiceRequest = new MilanRequest(
                JsonConvert.SerializeObject(request.BackendServiceArguments),
                _context,
                session.SessionData,
                servicePayloads,
                null);

            try
            {
                var executionResult = await ExecuteMilanRequestAsync(backendService, backendServiceRequest);
                return executionResult;
            }
            catch (Exception e)
            {
                // Tell NewRelic about slot unhandled exceptions to guarantee they get to NewRelic.
                NewRelic.Api.Agent.NewRelic.NoticeError(e);
                throw;
            }
        }

        // Trace attribute is required to get more detailed New Relic information from the backend service
        [Trace]
        private static async Task<BackendServiceResponse> ExecuteMilanRequestAsync(
            BackendService backendService,
            MilanRequest backendServiceRequest)
        {
            return await backendService.RequestAsync(backendServiceRequest);
        }

        private async Task<StorageResult> PersistData(ISessionKey sessionKey, SessionDataPayload session)
        {
            if (session.IsNew)
            {
                session.IsNew = false;
                var setDataResult = await _storage.SetData(sessionKey.ToString(), session);
                if (!setDataResult.Success)
                {
                    _logger.LogError("Failed to create the player's session data in storage");
                    return new StorageResult
                    {
                        ErrorMessage = setDataResult.ErrorMessage,
                        Success = setDataResult.Success
                    };
                }
            }
            else
            {
                var updateDataResult = await _storage.UpdateData(sessionKey.ToString(), session);
                if (!updateDataResult.Success)
                {
                    _logger.LogError("Failed to update the player's session data in storage");
                    return new StorageResult
                    {
                        ErrorMessage = updateDataResult.ErrorMessage,
                        Success = updateDataResult.Success
                    };
                }
            }

            return new StorageResult
            {
                Success = true,
            };
        }

        private async Task<StorageResult<SessionDataPayload>> GetSession(ISessionKey sessionKey)
        {
            if (_backend == null)
            {
                throw new Exception("_backend is null. Please initialize before calling this method");
            }

            var getDataResult = await _storage.GetData<SessionDataPayload>(sessionKey.ToString());

            if (!getDataResult.Success)
            {
                _logger.LogStateLoadFailure(getDataResult.ErrorMessage, _backend.Metadata, sessionKey);
                return new StorageResult<SessionDataPayload>
                {
                    ErrorMessage = getDataResult.ErrorMessage
                };
            }

            if (getDataResult.Value == null)
            {
                return new StorageResult<SessionDataPayload>
                {
                    ErrorMessage = $"No session found for with key '{sessionKey}'"
                };
            }

            return new StorageResult<SessionDataPayload>
            {
                Value = getDataResult.Value,
                Success = true
            };
        }

        private async Task<StorageResult<SessionDataPayload>> GetOrCreateSession(ISessionKey sessionKey)
        {
            if (_backend == null)
            {
                throw new Exception("_backend is null. Please initialize before calling this method");
            }

            var getDataResult = await _storage.GetData<SessionDataPayload>(sessionKey.ToString());

            if (!getDataResult.Success)
            {
                _logger.LogStateLoadFailure(getDataResult.ErrorMessage, _backend.Metadata, sessionKey);
                return new StorageResult<SessionDataPayload>
                {
                    ErrorMessage = getDataResult.ErrorMessage
                };
            }

            if (getDataResult.Value == null)
            {
                return new StorageResult<SessionDataPayload>
                {
                    Value = new SessionDataPayload
                    {
                        SessionData = new SessionData<IPersistentData, IRoundData>(),
                        BackendMetadata = _backend.Metadata,
                        IsNew = true
                    },
                    Success = true
                };
            }

            return new StorageResult<SessionDataPayload>
            {
                Value = getDataResult.Value,
                Success = true
            };
        }
    }
}
