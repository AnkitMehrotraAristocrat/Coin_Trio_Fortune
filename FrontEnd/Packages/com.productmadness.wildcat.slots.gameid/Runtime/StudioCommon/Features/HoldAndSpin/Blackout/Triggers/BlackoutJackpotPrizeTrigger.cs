using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.Blackout
{
    public class BlackoutJackpotPrizeTrigger : DynamicTrigger<BlackoutJackpotPrizeTriggerData>
    {
        [FieldRequiresModel] private BlackoutClientModel _blackoutModel;

        public BlackoutJackpotPrizeTrigger(BlackoutJackpotPrizeTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _blackoutModel = serviceLocator.Get<BlackoutClientModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _blackoutModel.IsCurrentPrizeTypeJackpot();
        }
    }
}
