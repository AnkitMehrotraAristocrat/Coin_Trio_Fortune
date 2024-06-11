using Microsoft.Extensions.Configuration;
using Xunit;

namespace Integration.Tests
{
    public class SlotTheoryData : TheoryData<string>
    {
        public SlotTheoryData()
        {
            var config = IntegrationTestConfigLoader.Configuration;
            var gameIds = config.GetSection("gameIds").Get<string[]>();
            foreach (var gameId in gameIds)
            {
                Add(gameId);
            }
        }
    }
}