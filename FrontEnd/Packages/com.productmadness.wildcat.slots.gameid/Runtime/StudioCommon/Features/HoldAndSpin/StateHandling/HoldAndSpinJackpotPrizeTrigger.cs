using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinJackpotPrizeTrigger : DynamicTrigger<HoldAndSpinJackpotPrizeTriggerData>
    {
		[FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel;

		public HoldAndSpinJackpotPrizeTrigger(HoldAndSpinJackpotPrizeTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _symbolOutcomeModel = serviceLocator.Get<SymbolOutcomeModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _symbolOutcomeModel.IsCurrentPrizeTypeJackpot();
        }
    }
}
