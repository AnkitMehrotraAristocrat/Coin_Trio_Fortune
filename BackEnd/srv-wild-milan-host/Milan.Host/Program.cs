using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Milan.Host;
using Serilog;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Extensions.Configuration.Placeholder;
using System.IO;

namespace Wildcat.Milan.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    var root = Path.Combine(env.ContentRootPath, "..", "..");

                    var assemblyName = string.Empty;

#if SlotIncluded
                    assemblyName = typeof(IWildBackend).Assembly.GetName().Name;
#endif

                    config
                    .AddJsonFile($"appsettings.game.json", optional: true)
                    .AddJsonFile($"appsettings.game.{env.EnvironmentName}.json", optional: true)
                    .AddJsonFile(Path.Combine(root, assemblyName, "appsettings.game.json"), optional: true)
                    .AddJsonFile(Path.Combine(root, assemblyName, $"appsettings.game.{env.EnvironmentName}.json"), optional: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.ListenAnyIP(8080);
                    });

                    // Use Config Server for configuration data
                    webBuilder.AddConfigServer();
                    // Steeltoe Placeholder resolver to for complex setting resolution like connection strings
                    webBuilder.AddPlaceholderResolver();

                    webBuilder.UseStartup<Startup>();
                });
    }
}
