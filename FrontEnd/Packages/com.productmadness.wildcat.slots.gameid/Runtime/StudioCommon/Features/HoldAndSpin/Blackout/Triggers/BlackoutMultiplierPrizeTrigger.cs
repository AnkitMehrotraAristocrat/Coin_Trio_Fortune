using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutMultiplierPrizeTrigger : DynamicTrigger<BlackoutMultiplierPrizeTriggerData>
    {
        [FieldRequiresModel] private BlackoutClientModel _blackoutModel;

        public BlackoutMultiplierPrizeTrigger(BlackoutMultiplierPrizeTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _blackoutModel = serviceLocator.Get<BlackoutClientModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _blackoutModel.IsCurrentPrizeTypeMultiplier();
        }
    }
}
