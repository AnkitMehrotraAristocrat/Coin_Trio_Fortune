using Milan.FrontEnd.Core.v5_1_1;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutSetNextPrizePresenter : BaseStatePresenter
    {
        [FieldRequiresModel] private BlackoutClientModel _blackoutModel = default;

        protected override void Execute()
        {
            _blackoutModel.SetNextPrize();
        }
    }
}
