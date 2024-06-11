using Wildcat.Milan.Shared.Dtos.Host;

namespace Wildcat.Milan.Host.Core
{
    public class VersionInfoResponse
    {
        public BackendVersionInfoResponse Backend { get; set; }
        public GameVersionModel Game { get; set; }

        public ServiceAdapterResponse ServiceAdapter { get; set; }

    }

    public class BackendVersionInfoResponse
    {
        public string Name { get; set; }
        public string Version { get; set; }
        //public string SHA { get; set; }
    }

    public class ServiceAdapterResponse
    {
        public string StorageName { get; set; }
        public string EnvironmentName { get; internal set; }
        public bool GaffingEnabled { get; internal set; }
    }
}