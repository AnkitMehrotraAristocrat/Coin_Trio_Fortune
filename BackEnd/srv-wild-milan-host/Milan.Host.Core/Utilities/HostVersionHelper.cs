using Microsoft.AspNetCore.Hosting;
using Milan.Common.Interfaces.Entities;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace Wildcat.Milan.Host.Utilities
{
    public interface IHostVersionHelper
    {
        public string GameVersion { get; }

        public string HostVersion { get; }
    }

    public class HostVersionHelper : IHostVersionHelper
    {
        private IWebHostEnvironment _hostingEnvironment;

        public string HostVersion { get; }
        public string GameVersion { get; }


        public HostVersionHelper(IWebHostEnvironment hostingEnvironment, IBackend backend)
        {
            ArgumentNullException.ThrowIfNull(hostingEnvironment);
            ArgumentNullException.ThrowIfNull(backend);

            _hostingEnvironment = hostingEnvironment;
            var rootPath = _hostingEnvironment.ContentRootPath;
            // Debug path
            var path = Path.Combine(rootPath, "..", "version.json");
            if (!File.Exists(path))
            {
                // Deployed path
                path = Path.Combine(rootPath, "version.json");
            }
            var versionJson = File.ReadAllText(path);
            var hostInfo = JsonConvert.DeserializeObject<HostVersionRecord>(versionJson);
            HostVersion = hostInfo.Version;
            GameVersion = backend.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
        }

        public record HostVersionRecord
        {
            public string Version { get; set; }
        }
    }
}
