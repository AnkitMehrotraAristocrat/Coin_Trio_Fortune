using System;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using JackpotCove;
using JackpotCove.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace Integration.Tests
{
    public class VerifySlotsResponsive : IClassFixture<HttpClientFixture>
    {
        private readonly HttpClientFixture _fixture;
        private readonly ITestOutputHelper _testOutputHelper;

        public VerifySlotsResponsive(HttpClientFixture fixture, ITestOutputHelper testOutputHelper)
        {
            _fixture = fixture;
            _testOutputHelper = testOutputHelper;
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        }

        [Theory]
        [ClassData(typeof(SlotTheoryData))]
        public async Task VerifyJoin(string backendId)
        {
            var serviceRequest = new ServiceRequest
            {
                BackendId = backendId,
                ServiceId = "Join",
                PlayerId = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                BackendServiceArguments = new object()
            };

            var responseMessage = await _fixture.Invoke(serviceRequest);
            var responseText = await responseMessage.Content.ReadAsStringAsync();

            responseMessage.IsSuccessStatusCode.ShouldBe(true, responseText);

            var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse>(responseText);

            serviceResponse.ShouldSatisfyAllConditions(sr =>
            {
                sr.ShouldNotBeNull();
                sr.value.ShouldNotBeNull();
                sr.isSuccess.ShouldBe(true);
                sr.BoostMultiplier.ShouldBe<uint>(1);
                sr.Payout.Length.ShouldBe(0);
            });
        }

        [Theory]
        [ClassData(typeof(SlotTheoryData))]
        public async Task VerifyWinningSpin(string backendId)
        {
            var maxRetries = 100;

            var serviceRequest = new SpinRequest
            {
                BackendId = backendId,
                ServiceId = "Join",
                PlayerId = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                BetIndex = 0,
                CurrentChipAmount = 100000,
                BackendServiceArguments = new object()
            };

            await _fixture.Invoke(serviceRequest);

            SpinResponse serviceResponse;

            var count = 0;
            do
            {
                var responseMessage = await _fixture.Spin(serviceRequest);
                var responseText = await responseMessage.Content.ReadAsStringAsync();

                responseMessage.IsSuccessStatusCode.ShouldBe(true, responseText);
                serviceResponse = JsonConvert.DeserializeObject<SpinResponse>(responseText);

                serviceResponse.ShouldSatisfyAllConditions(sr =>
                {
                    sr.ShouldNotBeNull();
                    sr.value.ShouldNotBeNull();
                    sr.value.ShouldNotBe(string.Empty);
                    sr.isSuccess.ShouldBeTrue();
                    sr.BetAmount.ShouldBeGreaterThan(0u);
                });

                count++;

                // Retrying the spin is a hack to test the payout payload which might not return for some slots unless
                // there's a win. This can lead to false a positive if there's no win within the retry period.
                // TODO: explore ways to guarantee a win so that the retries can be removed.
                if (serviceResponse.Payout.Length > 0)
                {
                    serviceResponse.ShouldSatisfyAllConditions(sr =>
                    {
                        sr.IsFreeSpin.ShouldBeFalse();
                        sr.Payout.First().Receipt.ShouldNotBe(Guid.Empty);
                        sr.Payout.First().CurrencyType.ShouldBe("Credit");
                    });
                }
                else
                {
                    if (count >= maxRetries)
                    {
                        throw new Xunit.Sdk.XunitException(
                            $"No payout payload returned after {count} attempts. Aborting");
                    }

                    _testOutputHelper.WriteLine(
                        $"There was no payout from this spin so couldn't verify the payout payload. Retrying {count}");
                }
            } while (serviceResponse.Payout.Length == 0 && count < maxRetries);
        }

        [Theory]
        [ClassData(typeof(SlotTheoryData))]
        public async Task VerifyActivateBoost(string backendId)
        {
            var serviceRequest = new ServiceRequest
            {
                BackendId = backendId,
                ServiceId = "Join",
                PlayerId = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                BackendServiceArguments = new object()
            };

            await _fixture.Invoke(serviceRequest);

            var activateBoostRequest = new ActivateBoostRequest
            {
                BackendId = backendId,
                PlayerId = serviceRequest.PlayerId,
                TableId = serviceRequest.TableId,
                BoostID = 1,
                BoostMultiplier = 3
            };

            var responseMessage = await _fixture.ActivateBoost(activateBoostRequest);
            var responseText = await responseMessage.Content.ReadAsStringAsync();

            responseMessage.IsSuccessStatusCode.ShouldBe(true, responseText);

            var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse>(responseText);
            serviceResponse.ShouldSatisfyAllConditions(sr =>
            {
                sr.ShouldNotBeNull();
                sr.value.ShouldNotBeNull();
                sr.value.ShouldNotBe(string.Empty);
                sr.isSuccess.ShouldBeTrue();
                sr.BoostMultiplier.ShouldBe(activateBoostRequest.BoostMultiplier);
                sr.Payout.Length.ShouldBe(0);
            });
            
            var joinRequest = new ServiceRequest
            {
                BackendId = backendId,
                ServiceId = "Join",
                PlayerId = serviceRequest.PlayerId,
                TableId = serviceRequest.TableId,
                BackendServiceArguments = new object()
            };

            var joinResponseMessage = await _fixture.Invoke(joinRequest);
            var joinResponseText = await joinResponseMessage.Content.ReadAsStringAsync();

            joinResponseMessage.IsSuccessStatusCode.ShouldBe(true, joinResponseText);

            var joinResponse = JsonConvert.DeserializeObject<ServiceResponse>(joinResponseText);
            joinResponse.ShouldSatisfyAllConditions(sr =>
            {
                sr.ShouldNotBeNull();
                sr.value.ShouldNotBeNull();
                sr.value.ShouldNotBe(string.Empty);
                sr.isSuccess.ShouldBeTrue();
                sr.BoostMultiplier.ShouldBe(activateBoostRequest.BoostMultiplier);
                sr.Payout.Length.ShouldBe(0);
            });
            
            var spinRequest = new SpinRequest
            {
                BackendId = backendId,
                PlayerId = serviceRequest.PlayerId,
                TableId = serviceRequest.TableId,
                BetIndex = 0,
                CurrentChipAmount = 121121210,
                BackendServiceArguments = new object()
            };

            var spinResponseMessage = await _fixture.Spin(spinRequest);
            var spinResponseText = await spinResponseMessage.Content.ReadAsStringAsync();

            spinResponseMessage.IsSuccessStatusCode.ShouldBe(true, spinResponseText);
            var spinServiceResponse = JsonConvert.DeserializeObject<SpinResponse>(spinResponseText);

            spinServiceResponse.ShouldSatisfyAllConditions(sr =>
            {
                sr.ShouldNotBeNull();
                sr.value.ShouldNotBeNull();
                sr.value.ShouldNotBe(string.Empty);
                sr.isSuccess.ShouldBeTrue();
                sr.BoostedSpin.ShouldBe(true);
                sr.BoostMultiplier.ShouldBe(activateBoostRequest.BoostMultiplier);
            });
        }

        [Theory]
        [ClassData(typeof(SlotTheoryData))]
        public async Task VerifyInsufficientFunds(string backendId)
        {
            var joinRequest = new ServiceRequest
            {
                BackendId = backendId,
                ServiceId = "Join",
                PlayerId = Guid.NewGuid(),
                TableId = Guid.NewGuid(),
                BackendServiceArguments = new object()
            };

            await _fixture.Invoke(joinRequest);

            var spinRequest = new SpinRequest
            {
                BackendId = backendId,
                PlayerId = joinRequest.PlayerId,
                TableId = joinRequest.TableId,
                BetIndex = 0,
                CurrentChipAmount = 0,
                BackendServiceArguments = new object()
            };

            var responseMessage = await _fixture.Spin(spinRequest);
            var responseText = await responseMessage.Content.ReadAsStringAsync();

            responseMessage.IsSuccessStatusCode.ShouldBe(false, responseText);
            responseMessage.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }
    }
}