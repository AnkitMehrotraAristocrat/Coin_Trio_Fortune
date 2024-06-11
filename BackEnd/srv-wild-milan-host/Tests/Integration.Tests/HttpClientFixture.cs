using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JackpotCove.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Integration.Tests
{
    public class HttpClientFixture : IDisposable
    {
        private static HttpClient _client;

        public HttpClientFixture()
        {
            var config = IntegrationTestConfigLoader.Configuration;
            var backendUrl = config["backendUrl"];
            if (string.IsNullOrEmpty(backendUrl))
            {
                var builder = WebHost.CreateDefaultBuilder(Array.Empty<string>())
                    .UseContentRoot(Path.GetFullPath("../../../../../Host/Milan.Host"))
                    .UseStartup<Milan.Host.Startup>();

                _client = new TestServer(builder).CreateClient();
            }
            else
            {
                _client = new HttpClient();
                _client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
                _client.BaseAddress = new Uri(backendUrl);
            }
        }

        public async Task<HttpResponseMessage> Invoke(ServiceRequest serviceRequest)
        {
            var content = new StringContent(JsonConvert.SerializeObject(serviceRequest), Encoding.Default,"application/json");
            return await _client.PostAsync("/JackpotCove/Invoke", content);
        }

        public async Task<HttpResponseMessage> Spin(SpinRequest spinRequest)
        {
            var content = new StringContent(JsonConvert.SerializeObject(spinRequest), Encoding.Default,"application/json");
            return await _client.PostAsync("/JackpotCove/Spin", content);
        }

        public async Task<HttpResponseMessage> ActivateBoost(ActivateBoostRequest activateBoostRequest)
        {
            var content = new StringContent(JsonConvert.SerializeObject(activateBoostRequest), Encoding.Default,"application/json");
            return await _client.PostAsync("/JackpotCove/ActivateBoost", content);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}