using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class WinMeterModel : IModel
    {
        public IReactiveProperty<long> WinAmount { get; } = new ReactiveProperty<long>(0);

        public WinMeterModel(ServiceLocator serviceLocator)
        { }
    }
}
