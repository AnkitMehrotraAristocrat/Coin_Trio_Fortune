using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutSerializedTrigger : DynamicTrigger<BlackoutSerializedTriggerData>
    {
        private BlackoutClientModel _model;

        public BlackoutSerializedTrigger(BlackoutSerializedTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _model = serviceLocator.GetOrCreate<BlackoutClientModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _model.Data.Blackout;
        }
    }
}
