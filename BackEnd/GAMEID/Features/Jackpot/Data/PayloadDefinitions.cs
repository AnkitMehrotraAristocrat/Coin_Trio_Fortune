
namespace GameBackend.Features.Jackpot.Data
{
    public class JackpotBetLevelMultiplierPayload
    {
        public JackpotBetLevelMultiplierPayload(string jID, int tID, JackpotBetLevelBracketPayload? bracketData)
        {
            jackpotId = jID;
            tierId = tID;
            bracket = bracketData;
        }

        public string jackpotId { get; set; }
        public int tierId { get; set; }
        public JackpotBetLevelBracketPayload? bracket { get; set; }
    }

    public class JackpotBetLevelBracketPayload
    {
        public JackpotBetLevelBracketPayload(int id)
        {
            bracketId = id;
        }

        public int bracketId { get; set; }
    }

    public class JackpotBetLevelMultiplier
    {
        public JackpotBetLevelMultiplier(int mFilter, JackpotBetLevelMultiplierPayload jPL, ulong[] multipliers)
        {
            matchFilter = mFilter;
            jackpotPayload = jPL;
            multiplierAtBetLevel = multipliers;
        }

        public int matchFilter { get; set; }
        public JackpotBetLevelMultiplierPayload jackpotPayload { get; set; }
        public ulong[] multiplierAtBetLevel { get; set; }
    }
}