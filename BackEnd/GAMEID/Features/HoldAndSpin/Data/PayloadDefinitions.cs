
namespace GameBackend.Features.HoldAndSpin.Data
{
    public class PayloadData
    {
        public string id { get; set; }
        public HoldAndSpinPayloadData HoldAndSpinData { get; set; }
    }

    public class HoldAndSpinPayloadData
    {
        public int FreeSpinsRemaining { get; set; }
        public bool TriggeringSpin { get; set; }
        public string TriggeringState { get; set; }
    }
}