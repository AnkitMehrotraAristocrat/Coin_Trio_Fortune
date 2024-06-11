using Milan.FrontEnd.Slots.v5_1_1.SpinCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinClientModelStatePresenter : BaseClientModelStateOptionalPresenter<HoldAndSpinServerModel, HoldAndSpinClientModel>
    {
        protected override void SetResult()
        {
            _clientModel.Data = _serverModel.Data;
        }
    }
}
