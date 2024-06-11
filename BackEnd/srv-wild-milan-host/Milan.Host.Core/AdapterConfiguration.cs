#nullable enable

using Wildcat.Milan.Game.Core.JackpotEngine.Contracts;

namespace Wildcat.Milan.Host.Core
{
    public class AdapterConfiguration
    {
        public ulong userID { get; set; }
        public ulong tableID { get; set; }

        public string StoragePluginName { get; set; }
        public bool GaffingEnabled { get; set; }

        public string? DefaultRTP { get; set; }
        public JackpotConfiguration jackpotEngine { get; set; }
    }
}