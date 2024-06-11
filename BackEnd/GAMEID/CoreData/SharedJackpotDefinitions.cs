using Milan.Common.Interfaces.Entities;
using System.Linq;

namespace GameBackend.Data
{
    public class JackpotPayloadData
    {
        public JackpotPayloadData(string jId, int? tId, ulong? val)
        {
            JackpotId = jId;
            TierId = tId;
            Value = val;
        }

        public string JackpotId { get; set; }
        public int? TierId { get; set; }
        public ulong? Value { get; set; }
    }

    public class JackpotConfigData : IConfiguration
    {
        public JackpotDefinition[] Jackpots { get; set; }
        public ulong[] Multipliers { get; set; }

        public object Clone() => Duplicate();

        public IConfiguration Duplicate()
        {
            return new JackpotConfigData() {
                Jackpots = Jackpots.Select(x => x.Clone() as JackpotDefinition).ToArray(),
                Multipliers = Multipliers
            };
        }
    }

    public class JackpotDefinition : IConfiguration
    {
        public string Id { get; set; }
        public ulong Tier { get; set; }

        public object Clone() => Duplicate();

        public IConfiguration Duplicate()
        {
            return new JackpotDefinition() {
                Id = Id.Clone() as string,
                Tier = Tier
            };
        }
    }
}