using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Milan.Common.Implementations.Entities;
using Milan.Common.Interfaces.Entities;
using Milan.Common.Interfaces.Utilities;
using Milan.Common.Logging;
using Milan.Common.Utilities;
using Milan.Infrastructure.Components;
using Milan.Infrastructure.Extensions;
using Newtonsoft.Json;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using Steeltoe.Management.Info;
using Steeltoe.Management.Endpoint;
using System.Threading.Tasks;
using Milan.Storage.FileSystem;
using System;
using ProductMadness.Phoenix.Api.Extensions;
using ProductMadness.Phoenix.Api.Filters;
using System.Linq;
using Refit;
using Wildcat.Milan.Game.Core.JackpotEngine.Endpoints;
using Wildcat.Milan.Game.Core.JackpotEngine.Facade;
using Wildcat.Milan.Game.Core.Utilities;
using Wildcat.Milan.Host.Utilities;
using Milan.Host;
using Wildcat.Milan.Storage.MongoDb;
using Wildcat.Milan.Host.Core.Utilities.Configuration;
using Newtonsoft.Json.Serialization;
using Wildcat.Milan.Host.Core.Utilities;

namespace Wildcat.Milan.Host
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private string _currentDirectory;
        private IComponentManager _componentManager;

        public List<Assembly> ServiceAdapterAssemblies;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            //Get current directory
            _currentDirectory = _webHostEnvironment.ContentRootPath;

            //Setup logging
            ApplicationLogger.SetLogger();

            // File manager for uploading files
            services.AddSingleton<IFileManager, ZipFileManager>();

            //Set configuration options from appSettings and get
            services.AddTransient<ConfigurationOptions, ConfigurationOptions>();
            services.AddTransient<Directories, Directories>();
            services.Configure<ConfigurationOptions>(Configuration.GetSection("ConfigurationOptions"));
            var configurationOptions = GetConfigurationOptions(services);

#if SlotIncluded
            var type = typeof(IWildBackend);
#endif

            var backends = BackendHelper.GetBackendImplementations();
            if (backends == null || !backends.Any())
            {
                throw new Exception("Host needs an implementation of IBackend.");
            }
            else if (backends.Count() > 1)
            {
                throw new Exception("Host doesn't support having multiple implementation of IBackend.");
            }
            services.AddTransient(typeof(IBackend), backends.Single());

            // Resolve game Id
            var gameId = BackendHelper.GetBackendGameId();

            // Initialize the configuration manager for backend
            services.AddSingleton<IConfigurationManager>(serviceProvider =>
                new WildcatConfigurationManager(gameId, serviceProvider.GetService<IOptionsMonitor<ConfigurationOptions>>(), serviceProvider));

            var storagePlugin = Configuration.GetValue<string>("milan:serviceAdapter:plugin");
            if (storagePlugin == "MongoSessionStorage")
            {
                services.AddSingleton<IStorage, SessionDataStorage>();
            }
            else if (storagePlugin == "LFSStorage")
            {
                services.AddTransient<IStorage, LfsStorage>();
            }
            else
            {
                throw new InvalidOperationException($"Unknown session storage: '{storagePlugin}'");
            }

            RegisterDefaultComponentServices(services, configurationOptions);

            RegisterCustomServices(services);

            // REFIT snake_case settings
            var snakeCaseContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
            var snakeCaseRefitSettings = new RefitSettings
            {
                ContentSerializer = new NewtonsoftJsonContentSerializer(
                    new JsonSerializerSettings
                    {
                        ContractResolver = snakeCaseContractResolver
                    }
                )
            };

            // Register Product Madness' Jackpot Engine service (JPE)
            var httpClientTimeoutMs = Configuration.GetValue<double>("http-client-timeout-ms");
            var jackpotEngineUri = new Uri(Configuration.GetValue<string>("jackpotEngine:url"));
            services
                .AddRefitClient<IJackpotEngine>(snakeCaseRefitSettings)
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = jackpotEngineUri;
                    c.Timeout = TimeSpan.FromMilliseconds(httpClientTimeoutMs);
                });

            // Register Facade over Product Madness' Jackpot Engine service
            services.AddTransient<IJackpotEngineFacade, JackpotEngineFacade>();

            // Build service adapters
            var builder = services.AddControllers(options =>
                {
                    options.Filters.Add<ExceptionFilter>();
                })
                .AddNewtonsoftJson(options => options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore)
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.ConfigureInvalidModelStateResponseFactory(Configuration);
                });

            foreach (var adapter in ServiceAdapterAssemblies)
                builder.AddApplicationPart(adapter);
            services.AddHostedService(ServiceAdapterAssemblies);

            services.AddSingleton<IHostVersionHelper, HostVersionHelper>();

            //Generate swagger
            services.AddSwaggerGen(c => c.SwaggerDoc(
                name: "v1",
                new OpenApiInfo { Title = "Milan Backend Platform Web Api", Version = "v1" }));
            // Add all actuators (includes integration with Spring Boot Admin)
            builder.Services.AddAllActuators(Configuration);
            builder.Services.ActivateActuatorEndpoints();
            builder.Services.AddSingleton<IInfoContributor, ProductVersionInfoContributor>();

            builder.Services.AddSingleton<IStartupFilter, WarmupFilter>();
        }

        private void RegisterCustomServices(IServiceCollection services) => services.AddServices(ServiceAdapterAssemblies);

        private void RegisterDefaultComponentServices(IServiceCollection services, IOptionsMonitor<ConfigurationOptions> configurationOptions)
        {
            _componentManager = new GlobalComponentManager();
            services.AddSingleton(_componentManager);
            _componentManager.Load(configurationOptions.CurrentValue, services);
            ServiceAdapterAssemblies = _componentManager.GetAssemblies<IAdapter>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Replacing Milan's service provider
            //ServiceManager.SetServiceProvider(app.ApplicationServices);
            ServiceManagerExtension.SetServiceProvider(app.ApplicationServices);

            app.UseDeveloperExceptionPage();
            switch (env.EnvironmentName)
            {
                case "Development":
                case "QA":
                case "Staging":
                    break;
                case "Production":
                    break;
            }

            // Swagger: using swagger endpoint & UI
            app.UseSwagger(config =>
            {
                config.RouteTemplate = "swagger-ui/{documentname}/swagger.json";
            });
            app.UseSwaggerUI(config =>
            {
                config.RoutePrefix = "swagger-ui";
            });

            /* This has to be disabled as the container has to use http
            app.UseHttpsRedirection();
            */

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", context =>
                {
                    context.Response.Redirect("./swagger-ui/index.html", permanent: false);
                    return Task.FromResult(0);
                });
            });
        }


        #region private methods
        /// <summary>
        /// Gets configuration defined in appSettings ConfigurationOptions section.
        /// Also, set the path where the plugins are located and adds them to configured options.
        /// </summary>
        private IOptionsMonitor<ConfigurationOptions> GetConfigurationOptions(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetService<IOptionsMonitor<ConfigurationOptions>>();

            //Load all path for plugins and configurations
            var pluginsPath = Path.Combine(_currentDirectory, options.CurrentValue.Directories.PluginsDirectory);
            options.CurrentValue.PluginsPath = pluginsPath;
            options.CurrentValue.AdaptersPath = Path.Combine(pluginsPath, options.CurrentValue.Directories.AdapterDirectory);
            options.CurrentValue.BackendPath = Path.Combine(pluginsPath, options.CurrentValue.Directories.BackendDirectory);
            options.CurrentValue.StoragePath = Path.Combine(pluginsPath, options.CurrentValue.Directories.StorageDirectory);
            services.AddSingleton(options);

            return options;
        }
        #endregion
    }
}