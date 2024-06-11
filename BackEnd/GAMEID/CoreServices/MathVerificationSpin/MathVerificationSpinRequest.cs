#if !JACKPOTS_OFF
using Wildcat.Milan.Game.Core.JackpotEngine.Contracts;
#endif

namespace GameBackend
{
    /// <summary>
    /// Defines the structure of a request to the Spin service
    /// </summary>
    public struct MathVerificationSpinRequest
    {
        public int BetIndex { get; set; }
        public int LineIndex { get; set; }
        public int UserId { get; set; }
        #if !JACKPOTS_OFF
        public JackpotConfiguration JackpotEngine { get; set; }
        #endif
    }
}
