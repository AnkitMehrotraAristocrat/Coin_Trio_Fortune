#region Using

using Milan.FrontEnd.Core.v5_1_1;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    /// <summary>
    /// Driver.
    /// </summary>
    public class HoldAndSpinDriver : GenericIdModelDriver<HoldAndSpinServerModel, HoldAndSpinPayload>
    {
        protected override void OnResponse(HoldAndSpinServerModel model, HoldAndSpinPayload data)
        {
            model.Data = data.HoldAndSpinData;
        }
    }
}
