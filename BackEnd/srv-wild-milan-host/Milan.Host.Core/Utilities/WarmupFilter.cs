using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Interfaces.Utilities;
using Milan.Infrastructure.Components;
using Steeltoe.Management.Info;
using System;
using System.Net.Http;
using Wildcat.Milan.Host.Utilities;

namespace Wildcat.Milan.Host.Core.Utilities
{
    public class WarmupFilter : IStartupFilter
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<WarmupFilter> _logger;

        public WarmupFilter(
            IServiceProvider serviceProvider,
            ILogger<WarmupFilter> logger
        )
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            _logger.LogInformation("Starting Warmup...");
            using (var scope = _serviceProvider.CreateScope())
            {
                scope.ServiceProvider.GetService<IInfoContributor>();
                scope.ServiceProvider.GetService<IConfigurationManager>();
                scope.ServiceProvider.GetService<IComponentManager>();
                scope.ServiceProvider.GetService<IHttpClientFactory>();
                scope.ServiceProvider.GetService<IBackend>();
                scope.ServiceProvider.GetService<IStorage>();
                scope.ServiceProvider.GetService<IHostVersionHelper>();
                scope.ServiceProvider.GetService<IFileManager>();
            }
            _logger.LogInformation("Warmup completed.");
            return builder =>
            {
                next(builder);
            };
        }
    }
}
