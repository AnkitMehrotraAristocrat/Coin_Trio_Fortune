using System;
using Microsoft.Extensions.Configuration;

namespace Integration.Tests
{
    public static class IntegrationTestConfigLoader
    {
        private static IConfigurationRoot _configuration;

        public static IConfigurationRoot Configuration
        {
            get
            {
                if (_configuration != null)
                {
                    return _configuration;
                }

                var environment = Environment.GetEnvironmentVariable("INTEGRATION_ENVIRONMENT");
                var builder = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", false, true);

                if (environment != null)
                {
                    builder.AddJsonFile($"appsettings.{environment}.json", true, true);
                }

                _configuration = builder.Build();

                return _configuration;
            }
        }
    }
}