using Microsoft.Extensions.Logging;
using Milan.Common.Implementations.Metadata;
using System;
using Wildcat.Milan.Shared.Dtos.Backend;
using Wildcat.Milan.Shared.Dtos.Host;
using Wildcat.Milan.Shared.Dtos.Session;

namespace Wildcat.Milan.Host.Core
{
    public static class ClassUsingOptimisedLogging
    {
        private static readonly Action<ILogger, object, string, string, string, string, Exception> ErrorLoggerSlotBackend
            = LoggerMessage.Define<object, string, string, string, string>(
                LogLevel.Error,
                Events.SlotError,
                "Slot backend error: '{ResponseError}', " +
                "BackendId: {BackendId}, " +
                "ServiceId: {ServiceId}, " +
                "PlayerId: {PlayerId}," +
                "TableId: {TableId}");

        private static readonly Action<ILogger, string, string, string, Exception> LoggerStateLoadFailure
            = LoggerMessage.Define<string, string, string>(
                LogLevel.Error,
                Events.StateLoadFailure,
                "Failed to retrieve the player's session data from storage: '{ErrorMessage}', " +
                "BackendId: {BackendId}, " +
                "PlayerId: {PlayerId}");


        private static readonly Action<ILogger, string, string, string, string, Exception> LoggerRTPVariationSwitch
            = LoggerMessage.Define<string, string, string, string>(
                LogLevel.Information,
                Events.RTPVariantSwitch,
                "Change RTP variation: " +
                "OriginalVariation: {OriginalVariation}, " +
                "NewVariation: {NewVariation}, " +
                "BackendId: {BackendId}, " +
                "PlayerId: {PlayerId}");

        public static void LogSlotBackendError(
            this ILogger logger,
            ServiceRequest request,
            WildcatBackendServiceResponse response)
            => ErrorLoggerSlotBackend(logger, response.Value, request.BackendId, request.ServiceId,
                request.PlayerId, null, null);

        public static void LogStateLoadFailure(
            this ILogger logger,
        string errorMessage,
            BackendMetadata backendMetadata,
            ISessionKey sessionKey)
            => LoggerStateLoadFailure(logger, errorMessage, backendMetadata.BackendId, sessionKey.PlayerId, null);

        public static void LogRTPVariationSwitch(
            this ILogger logger,
            string originalVariation,
            string newVariation,
            BackendMetadata backendMetadata,
            string playerId)
        {
            // The SetVariation needs to be called each time we use a variant, but only log when we switch
            if (originalVariation != newVariation)
            {
                LoggerRTPVariationSwitch(logger, originalVariation, newVariation, backendMetadata.BackendId, playerId, null);
            }
        }

        private static class Events
        {
            public static readonly EventId SlotError = new EventId(100, "SlotError");
            public static readonly EventId StateLoadFailure = new EventId(101, "StateLoadFailure");
            public static readonly EventId RTPVariantSwitch = new EventId(102, "RTPVariantSwitch");
            public static readonly EventId MaxBetLevelValidationFailure = new EventId(103, "MaxBetLevelValidationFailure");
        }
    }
}