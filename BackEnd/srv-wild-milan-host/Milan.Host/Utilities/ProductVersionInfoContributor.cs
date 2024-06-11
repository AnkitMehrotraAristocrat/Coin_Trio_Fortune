using Steeltoe.Management.Info;
using System.Reflection;

namespace Wildcat.Milan.Host.Utilities
{
    /// <summary>
    /// Replacing the source of the build version in the /actuator/info endpoint from file version to product version.
    /// </summary>
    public class ProductVersionInfoContributor : IInfoContributor
    {
        private readonly IHostVersionHelper _hostVersionHelper;

        public ProductVersionInfoContributor(IHostVersionHelper hostVersionHelper)
        {
            _hostVersionHelper = hostVersionHelper;
        }

        public void Contribute(IInfoBuilder builder)
        {
            builder.WithInfo("build", new { 
                version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion,
                host_version = _hostVersionHelper.HostVersion
            });
        }
    }
}
