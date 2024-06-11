using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Wildcat.Milan.Host.Core.Models
{
    public class MilanConfiguration
    {
        public class MilanConfig
        {
            [ConfigurationKeyName("actions")]
            [JsonProperty("actions")]
            public MilanActionsConfig Actions { get; set; } = new MilanActionsConfig();
        }

        public class MilanActionsConfig
        {
            [ConfigurationKeyName("join")]
            [JsonProperty("join")]
            public MilanJoinConfig Join { get; set; } = new MilanJoinConfig();
        }

        public class MilanJoinConfig
        {
            [ConfigurationKeyName("version")]
            [JsonProperty("version")]
            public MilanParameterConfig Version { get; set; } = new MilanParameterConfig();

            [ConfigurationKeyName("variation")]
            [JsonProperty("variation")]
            public MilanParameterConfig Variation { get; set; } = new MilanParameterConfig();

            [ConfigurationKeyName("jackpot_template_id")]
            [JsonProperty("jackpot_template_id")]
            public MilanParameterConfig JackpotTemplateId { get; set; } = new MilanParameterConfig();
        }

        public class MilanParameterConfig
        {
            [ConfigurationKeyName("mandatory")]
            [JsonProperty("mandatory")]
            public bool IsMandatory { get; set; } = true;
        }

        [ConfigurationKeyName("milan")]
        [JsonProperty("milan")]
        public MilanConfig Milan { get; set; }
    }
}
