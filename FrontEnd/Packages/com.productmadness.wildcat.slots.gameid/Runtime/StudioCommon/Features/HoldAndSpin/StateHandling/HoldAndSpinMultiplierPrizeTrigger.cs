using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinMultiplierPrizeTrigger : DynamicTrigger<HoldAndSpinMultiplierPrizeTriggerData>
    {
        [FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel;

        public HoldAndSpinMultiplierPrizeTrigger(HoldAndSpinMultiplierPrizeTriggerData data, ServiceLocator serviceLocator) : base(data)
        {
            _symbolOutcomeModel = serviceLocator.Get<SymbolOutcomeModel>(data.modelTag);
        }

        public override bool IsTriggered()
        {
            return _symbolOutcomeModel.IsCurrentPrizeTypeMultiplier();
        }
    }
}
