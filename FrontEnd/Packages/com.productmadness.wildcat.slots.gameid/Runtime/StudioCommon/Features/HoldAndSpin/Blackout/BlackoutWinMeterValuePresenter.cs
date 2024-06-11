using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Jackpots;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using System;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutWinMeterValuePresenter : WinMeterValuePresenter
    {
        [FieldRequiresModel] private BlackoutClientModel _blackoutModel = default;

        public override void UpdateWinMeterValue()
        {
            if(_blackoutModel.CurrentPrize != null)
            {
                _winMeterModel.WinAmount.Value += Convert.ToInt64(_blackoutModel.CurrentPrize.Value);
            } 
        }
    }
}
