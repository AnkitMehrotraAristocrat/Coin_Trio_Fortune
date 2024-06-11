using AutoFixture;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using Wildcat.Milan.Host.Core;
using Wildcat.Milan.Host.Core.Models;
using Wildcat.Milan.Host.Core.Validation;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.MilanAdapter.Common.Tests.Features.Slots.Commands;

[TestClass]
public class JoinServiceRequestValidatorTests
{
    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_passes_with_valid_request()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: true);
        var validationRequest = BuildValidValidationRequest();

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.IsValid);
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_missing_variation_when_mandatory()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: true);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.Variation = string.Empty;

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "variation"));
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_invalid_variation()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: false);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.Variation = "unknown_variation";

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "variation"));
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_missing_version_when_mandatory()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: true);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.Version = string.Empty;

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "version"));
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_invalid_version()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: false);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.Version = "9.9.9";

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "version"));
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_missing_jackpot_template_id_when_mandatory()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: true);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.JackpotTemplateId = null;

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "jackpot_template_id"));
    }

    [TestMethod]
    public async Task Ensure_JoinServiceRequestValidator_detects_missing_default_jackpot_template_id_when_NOT_mandatory()
    {
        // Arrange
        IConfigurationRoot configuration = BuildConfiguration(shouldCheckForMandatoryFields: false);
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.BackendServiceArguments.JackpotTemplateId = null;

        var test = configuration.Get<AdapterConfiguration>();

        // Act
        var validationResult = await new JoinServiceRequestValidator(configuration).ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "jackpot_template_id"));
    }

    private static JoinSlotServiceValidationRequest BuildValidValidationRequest()
    {
        var fixture = new Fixture();
        var gameInfo = fixture.Create<GameVersionModel>();
        var joinRequest = new JoinServiceRequest()
        {
            BackendId = gameInfo.GameId,
            ServiceId = gameInfo.Services.First(),
            PlayerId = fixture.Create<string>(),
            BackendServiceArguments = new JoinBackendArguments()
            {
                JackpotTemplateId = fixture.Create<int>(),
                Variation = gameInfo.Variations.First().Id,
                Version = gameInfo.Version
            }
        };
        return new JoinSlotServiceValidationRequest(joinRequest, gameInfo);
    }

    private static IConfigurationRoot BuildConfiguration(bool shouldCheckForMandatoryFields)
    {
        var configObject = new MilanConfiguration()
        {
            Milan = new MilanConfiguration.MilanConfig()
            {
                Actions = new MilanConfiguration.MilanActionsConfig()
                {
                    Join = new MilanConfiguration.MilanJoinConfig()
                    {
                        JackpotTemplateId = new MilanConfiguration.MilanParameterConfig() { IsMandatory = shouldCheckForMandatoryFields },
                        Variation = new MilanConfiguration.MilanParameterConfig() { IsMandatory = shouldCheckForMandatoryFields },
                        Version = new MilanConfiguration.MilanParameterConfig() { IsMandatory = shouldCheckForMandatoryFields }
                    }
                }
            }
        };
        var json = JsonConvert.SerializeObject(configObject);
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)))
            .Build();
        return configuration;
    }

}