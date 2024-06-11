using Milan.FrontEnd.Slots.v5_1_1.SpinCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutClientModelStatePresenter : BaseClientModelStateOptionalPresenter<BlackoutServerModel, BlackoutClientModel>
    {
        protected override void SetResult()
        {
            _clientModel.Data = _serverModel.Data;
        }
    }
}
