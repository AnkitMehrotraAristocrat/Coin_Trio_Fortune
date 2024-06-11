#region Using

using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    /// <summary>
    /// Presenter to update the spin count client model.
    /// </summary>
    public class HoldAndSpinSpinCountClientModelStatePresenter : BaseStatePresenter, IStatePresenter, ServiceLocator.IHandler
    {
        #region Models

        private HoldAndSpinClientModel _holdAndSpinModel = default;
        private HoldAndSpinSpinCountClientModel _spinCountClientModel = default;

        [SerializeField] private string _spinCountClientModelTag;
        [SerializeField] private string _holdAndSpinClientModelTag;

        #endregion

        public override void OnServicesLoaded()
        {
            base.OnServicesLoaded();

            ServiceLocator serviceLocator = GlobalObjectExtensions.GetGlobalComponent<ServiceLocator>();

            _spinCountClientModel = serviceLocator.GetOrCreate<HoldAndSpinSpinCountClientModel>(_spinCountClientModelTag);
            _holdAndSpinModel = serviceLocator.GetOrCreate<HoldAndSpinClientModel>(_holdAndSpinClientModelTag);
        }

        #region HnS Spin Count State Handling

        protected override void Execute()
        {
            UpdateClientSpinCount();
        }

        // This function can also be invoked via the animation event forwarder
        // Also invoked from the CorLandingSymbolView
        public void UpdateClientSpinCount()
        {
            _spinCountClientModel.Count.Value = _holdAndSpinModel.Data.FreeSpinsRemaining;
        }

        #endregion
    }
}
