using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public class CorLandingSymbolView : LandingSymbolView, ServiceLocator.IHandler
    {
        [FieldRequiresGlobal] HoldAndSpinSpinCountClientModelStatePresenter _spinCountClientModelStatePresenter;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public override void OnLanded(Location location, SymbolHandle symbol)
        {
            foreach (var validSymbol in ValidSymbols)
            {
                if (validSymbol != symbol.id)
                {
                    continue;
                }
                TriggerHandle(symbol, LandedTrigger);
                _spinCountClientModelStatePresenter.UpdateClientSpinCount();
                break; // breaking since H&S lands only 1 symbol
            }
        }
    }
}
