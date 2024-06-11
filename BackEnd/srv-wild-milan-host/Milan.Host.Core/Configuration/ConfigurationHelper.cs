using Microsoft.Extensions.Configuration;
using ProductMadness.Phoenix.Core.Extensions;
using static Wildcat.Milan.Host.Core.Models.MilanConfiguration;

namespace Wildcat.Milan.Host.Core.Configuration
{
    public static class ConfigurationHelper
    {
        public const string HTTP_CLIENT_TIMEOUT_MS = "http-client-timeout-ms";
        private const string MILAN_CONFIG_KEY = "milan:actions:join";

        private static readonly object _staticLock = new object();
        private static MilanJoinConfig _staticMilanJoinConfig;

        public static MilanJoinConfig GetMilanJoinActionConfiguration(this IConfiguration configuration)
        {
            if (_staticMilanJoinConfig == null)
            {
                lock (_staticLock)
                {
                    if (_staticMilanJoinConfig == null)
                    {
                        _staticMilanJoinConfig = configuration.GetSectionOrThrow<MilanJoinConfig>(MILAN_CONFIG_KEY);
                    }
                }
            }
            return _staticMilanJoinConfig;
        }
    }
}