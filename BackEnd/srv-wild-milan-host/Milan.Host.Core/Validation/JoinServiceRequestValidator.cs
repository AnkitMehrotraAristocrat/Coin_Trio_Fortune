using FluentValidation;
using Microsoft.Extensions.Configuration;
using ProductMadness.Phoenix.Api.Contracts;
using ProductMadness.Phoenix.Core.Extensions;
using ProductMadness.Phoenix.Core.FluentValidation;
using System;
using System.Linq;
using Wildcat.Milan.Host.Core.Configuration;
using Wildcat.Milan.Host.Core.Models;
using Wildcat.Milan.Shared.Dtos.Host;
using static Wildcat.Milan.Host.Core.Models.MilanConfiguration;

namespace Wildcat.Milan.Host.Core.Validation
{
    /// <summary>
    /// Represents all the data necessary to validate the incoming 'Join' service request.
    /// </summary>
    public class JoinSlotServiceValidationRequest
    {
        public JoinSlotServiceValidationRequest(JoinServiceRequest request, GameVersionModel gameInfo)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(gameInfo);

            Request = request;
            GameInfo = gameInfo;
        }

        public JoinServiceRequest Request { get; }
        public GameVersionModel GameInfo { get; }
    }

    /// <summary>
    /// Actual implementation of the 'Join' service request validation.
    /// </summary>
    public class JoinServiceRequestValidator : CustomAbstractValidator<JoinSlotServiceValidationRequest>
    {
        public JoinServiceRequestValidator(IConfiguration configuration)
        {
            MilanJoinConfig joinConfiguration = configuration.GetMilanJoinActionConfiguration();

            RuleFor(data => data.Request.BackendServiceArguments.Variation)
                .NotEmpty()
                .When(data => joinConfiguration.Variation.IsMandatory)
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

            RuleFor(data => data)
                .NotEmpty()
                .Must(data => data.GameInfo.Variations.Any(variation => variation.Id == data.Request.BackendServiceArguments.Variation))
                .When(data => data.Request.BackendServiceArguments.Variation.IsNotNullOrEmpty())
                .OverridePropertyName(nameof(JoinServiceRequest.BackendServiceArguments.Variation).ToSnakeCase())
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                .WithMessage(data => $"Variation '{data.Request.BackendServiceArguments.Variation}' not supported");

            RuleFor(data => data.Request.BackendServiceArguments.Version)
                .NotEmpty()
                .When(data => joinConfiguration.Version.IsMandatory)
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

            RuleFor(data => data)
                .Must(data => data.Request.BackendServiceArguments.Version.Equals(data.GameInfo.Version, StringComparison.InvariantCultureIgnoreCase))
                .When(data => data.Request.BackendServiceArguments.Version.IsNotNullOrEmpty())
                .OverridePropertyName(nameof(JoinServiceRequest.BackendServiceArguments.Version).ToSnakeCase())
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                .WithMessage(data => $"Version '{data.Request.BackendServiceArguments.Version}' is invalid. Current hosted version of '{data.GameInfo.GameId}' is '{data.GameInfo.Version}'");

            RuleFor(data => data.Request.BackendServiceArguments.JackpotTemplateId)
                .NotEmpty()
                .When(data => joinConfiguration.JackpotTemplateId.IsMandatory)
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR);

            RuleFor(data => data.Request.BackendServiceArguments.JackpotTemplateId)
                .Must(data => configuration.Get<AdapterConfiguration>().jackpotEngine.defaultTemplateId.HasValue)
                .When(data => !data.Request.BackendServiceArguments.JackpotTemplateId.HasValue)
                .When(data => !joinConfiguration.JackpotTemplateId.IsMandatory)
                .WithErrorCode(ErrorCodes.VALIDATION_ERROR)
                .WithMessage(data => $"Default jackpot Id must be provided in settings when no 'jackpot_template_id' is not provided in backend service arguments.");
            ;
        }
    }
}
