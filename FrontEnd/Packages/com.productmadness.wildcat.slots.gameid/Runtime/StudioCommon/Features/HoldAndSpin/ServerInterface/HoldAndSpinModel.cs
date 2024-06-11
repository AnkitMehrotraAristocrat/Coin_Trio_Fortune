#region Using
using Milan.FrontEnd.Core.v5_1_1.Data;
#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    /// <summary>
    /// Model to hold server data.
    /// </summary>
    public abstract class HoldAndSpinModel : IModel
    {
        /// <summary>
        /// Payload data.
        /// </summary>
        public HoldAndSpinData Data;
    }
}
