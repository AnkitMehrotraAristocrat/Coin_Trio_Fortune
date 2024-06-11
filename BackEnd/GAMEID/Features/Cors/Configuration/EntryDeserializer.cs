using GameBackend.Features.Cors.Data;

namespace GameBackend.Features.Cors.Configuration
{
    /// <summary>
    /// Used to interpret weight table entries 
    /// </summary>
    public static class EntryDeserializer
    {
        public static CorPrizeInfo Deserialize(string randomEntryString)
        {
            CorPrizeInfo randomEntry;
            if (double.TryParse(randomEntryString, out double multiplier)) { 
                // multiplier
                randomEntry = new CorPrizeInfo(multiplier, GameConstants.MultiplierPrizeType, string.Empty);
            }
            else { 
                // jackpot
                randomEntry = new CorPrizeInfo(0, GameConstants.JackpotPrizeType, randomEntryString, GameConstants.JackpotTiers[randomEntryString]);
            }
            return randomEntry;
        }
    }
}
