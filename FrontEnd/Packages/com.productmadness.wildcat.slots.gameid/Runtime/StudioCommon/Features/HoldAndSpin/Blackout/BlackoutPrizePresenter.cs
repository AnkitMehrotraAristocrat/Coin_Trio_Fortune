using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using Milan.FrontEnd.Bridge.Logging;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutPrizePresenter : BaseBlackoutPrizePresenter
    {
        protected override IEnumerator<Yield> PresentCurrentPrize(PrizeInfo currentPrize)
        {
            GameIdLogger.Logger.Debug("Add custom blackout presentation here");
            yield break;
        }
    }
}
