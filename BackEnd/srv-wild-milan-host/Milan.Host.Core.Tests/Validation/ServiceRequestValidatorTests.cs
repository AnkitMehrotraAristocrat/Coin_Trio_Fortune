using AutoFixture;
using Newtonsoft.Json.Linq;
using Wildcat.Milan.Host.Core;
using Wildcat.Milan.Host.Core.Validation;
using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.MilanAdapter.Common.Tests.Features.Slots.Commands;

[TestClass]
public class ServiceRequestValidatorTests
{
    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_passes_with_valid_request()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.IsValid);
    }

    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_detects_missing_player_id()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();
        validationRequest.Request.PlayerId = string.Empty;

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "player_id"));
    }

    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_detects_missing_backend_id()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();
        validationRequest.Request.BackendId = string.Empty;

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "backend_id"));
    }

    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_detects_invalid_backend_id()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();
        validationRequest.Request.BackendId = "wrong_backend_id";

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "backend_id"));
    }

    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_detects_missing_service_id()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.ServiceId = string.Empty;

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "service_id"));
    }

    [TestMethod]
    public async Task Ensure_ServiceRequestValidator_detects_invalid_service_id()
    {
        // Arrange
        var validationRequest = BuildValidValidationRequest();

        validationRequest.Request.ServiceId = "unknown_service_id";

        // Act
        var validationResult = await new ServiceRequestValidator().ValidateAsync(validationRequest, CancellationToken.None);

        // Assert
        Assert.IsTrue(validationResult.Errors.Any(error => error.PropertyName == "service_id"));
    }

    private static ServiceRequestValidationRequest BuildValidValidationRequest()
    {
        var fixture = new Fixture();
        var gameInfo = fixture.Create<GameVersionModel>();
        var serviceRequest = new ServiceRequest()
        {
            BackendId = gameInfo.GameId,
            ServiceId = gameInfo.Services.First(),
            PlayerId = fixture.Create<string>(),
            BackendServiceArguments = JToken.FromObject(new { })
        };
        return new ServiceRequestValidationRequest(serviceRequest, gameInfo);
    }
}