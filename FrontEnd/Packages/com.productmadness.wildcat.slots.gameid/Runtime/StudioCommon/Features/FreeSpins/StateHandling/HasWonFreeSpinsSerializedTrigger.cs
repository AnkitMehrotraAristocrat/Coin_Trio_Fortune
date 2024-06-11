using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.FreespinCore;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class HasWonFreeSpinsSerializedTrigger : DynamicTrigger<HasWonFreeSpinsSerializedTriggerData>
    {
        private FreeSpinServerModel _model;

        public HasWonFreeSpinsSerializedTrigger(HasWonFreeSpinsSerializedTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _model = serviceLocator.GetOrCreate<FreeSpinServerModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _model.SpinsWon.Value > 0;
        }
    }
}
