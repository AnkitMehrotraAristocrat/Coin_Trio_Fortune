using FluentValidation;
using ProductMadness.Phoenix.Api.Contracts;
using ProductMadness.Phoenix.Core.Extensions;
using ProductMadness.Phoenix.Core.FluentValidation;
using System;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core.Validation
{
    /// <summary>
    /// Represents all the data necessary to validate the generic incoming ServiceRequest.
    /// </summary>
    public class ServiceRequestValidationRequest
    {
        public ServiceRequestValidationRequest(ServiceRequest request, GameVersionModel gameInfo)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(gameInfo);

            Request = request;
            GameInfo = gameInfo;
        }

        /// <summary>
        /// Incoming generic ServiceRequest.
        /// </summary>
        public ServiceRequest Request { get; }

        /// <summary>
        /// Context about hosted game backend.
        /// </summary>
        public GameVersionModel GameInfo { get; }
    }

    /// <summary>
    /// Actual implementation of the generic ServiceRequest validation.
    /// </summary>
    public class ServiceRequestValidator : CustomAbstractValidator<ServiceRequestValidationRequest>
    {
        public ServiceRequestValidator()
        {
            RuleFor(data => data.Request.PlayerId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

            RuleFor(data => data.Request.BackendId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                .DependentRules(() =>
                {
                    RuleFor(data => data.Request.BackendId)
                        .Equal(data => data.GameInfo.GameId)
                        .WithMessage(data => $"Backend '{data.Request.BackendId}' not supported. Hosted backend Id is '{data.GameInfo.GameId}'");
                });

            RuleFor(data => data.Request.ServiceId)
                .NotEmpty()
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                .DependentRules(() =>
                {
                    RuleFor(data => data)
                        .Must(data => data.GameInfo.Services.Contains(data.Request.ServiceId))
                        .OverridePropertyName(nameof(ServiceRequest.ServiceId).ToSnakeCase())
                        .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                        .WithMessage(data => $"Service '{data.Request.ServiceId}' not supported in hosted backend '{data.GameInfo.GameId}'");
                });
        }
    }
}
