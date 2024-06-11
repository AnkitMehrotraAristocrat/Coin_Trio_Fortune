using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HasHoldAndSpinsRemainingTrigger : DynamicTrigger<HasHoldAndSpinsRemainingTriggerData>
    {
        private HoldAndSpinClientModel _model;

        public HasHoldAndSpinsRemainingTrigger(HasHoldAndSpinsRemainingTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _model = serviceLocator.GetOrCreate<HoldAndSpinClientModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _model.Data.FreeSpinsRemaining > 0;
        }
    }
}
