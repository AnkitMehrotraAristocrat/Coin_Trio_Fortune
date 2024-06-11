using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.Jackpots;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    public abstract class WinMeterValuePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
    {       
        [FieldRequiresModel] protected WinMeterModel _winMeterModel = default;  

        public string Tag => this.GetTag();

        public INotifier Notifier {
            get; set;
        }

        public void OnServicesLoaded()
        {
            this.InitializeDependencies();
        }

        public IEnumerator<Yield> Enter()
        {
            yield break;
        }

        public IEnumerator<Yield> Exit()
        {
            UpdateWinMeterValue();
            yield break;
        }

        public abstract void UpdateWinMeterValue();
    }
}
