using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using Milan.FrontEnd.Slots.v5_1_1.Meta;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinSpinCountClientModel : IModel, IBonusSpinFrame
    {
        // Unused but allows use of IBonusSpinFrame which allows the use of DeductBonusSpinPresenter
        public IReactiveProperty<bool> IsActive { get; } = new ReactiveProperty<bool>(false); 
        public IReactiveProperty<int> Count { get; } = new ReactiveProperty<int>(0);

        public HoldAndSpinSpinCountClientModel(ServiceLocator serviceLocator) { }
    }
}
