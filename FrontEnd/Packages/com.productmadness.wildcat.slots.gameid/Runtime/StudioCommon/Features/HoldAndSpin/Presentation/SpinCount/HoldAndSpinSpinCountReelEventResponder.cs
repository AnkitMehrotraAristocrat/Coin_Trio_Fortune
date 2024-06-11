using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    public class HoldAndSpinSpinCountReelEventResponder : MonoBehaviour, ServiceLocator.IHandler, IReelEventResponder
    {
        [FieldRequiresSelf] private SymbolLocator _symbolLocator;

        [SerializeField, Reorderable]
        private SymbolIdList _validSymbols;
        public SymbolIdList ValidSymbols => _validSymbols;

        private HoldAndSpinSpinCountClientModelStatePresenter _spinCountClientModelStatePresenter;

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
            _spinCountClientModelStatePresenter = transform.root.GetComponentInChildren<HoldAndSpinSpinCountClientModelStatePresenter>();
        }

        public void OnReelStop(int reelIndex)
        {
            var symbols = _symbolLocator.ScreenSymbols.ByColumn(reelIndex);
            foreach (var symbol in symbols)
            {
                OnLanded(symbol.CurrentSymbol.Instance);
            }
        }

        public void OnReelQuickStop(int reelIndex)
        {
            var symbols = _symbolLocator.ScreenSymbols.ByColumn(reelIndex);
            foreach (var symbol in symbols)
            {
                OnLanded(symbol.CurrentSymbol.Instance);
            }
        }

        public void OnLanded(SymbolHandle symbol)
        {
            foreach (var validSymbol in ValidSymbols)
            {
                if (validSymbol != symbol.id)
                {
                    continue;
                }
                _spinCountClientModelStatePresenter.UpdateClientSpinCount();
                break; // breaking since H&S lands only 1 symbol
            }
        }

        public void OnReelSpin(int reelIndex)
        {
            // Does nothing
        }

        public void OnReelLanding(int reelIndex)
        {
            // Does nothing
        }
    }
}
