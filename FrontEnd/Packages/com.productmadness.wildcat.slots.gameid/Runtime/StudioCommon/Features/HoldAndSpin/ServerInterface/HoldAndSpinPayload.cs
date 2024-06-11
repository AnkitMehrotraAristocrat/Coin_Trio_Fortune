using Milan.Shared.DTO.FrontEnd;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    /// <summary>
    /// Data structure that matches a payload sent from the backend.
    /// </summary>
    public class HoldAndSpinPayload : IHasId
    {
        public string Id { get; set; }
        public HoldAndSpinData HoldAndSpinData { get; set; }
    }

    public class HoldAndSpinData
    {
        public int FreeSpinsRemaining { get; set; }
        public bool TriggeringSpin { get; set; }
        public string TriggeringState { get; set; }
    }
}