using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
    /// <summary>
    /// Presenter to handle round win recovery.
    /// </summary>
    public class RoundWinRecoveryPresenter : MonoBehaviour, IRecoveryHandler
    {
        [FieldRequiresModel] private WinClientModel _winClientModel = default;
        [FieldRequiresModel] private RoundServerModel _roundServerModel = default;

        [FieldRequiresGlobal] private WinStartEventBroadcaster _eventBroadcaster = null;

        [SerializeField] private string _winClientModelCurrencyType = "chips";
        [SerializeField] private string _roundServerModelCurrencyType = "credit";
        [SerializeField] private WinStartData _winStartConfig;

        public void OnServerConfigsReady(ServiceLocator locator)
        { }

        public void OnInitialStateReady(ServiceLocator locator)
        {
            this.InitializeDependencies();

            _eventBroadcaster.Broadcast(_winStartConfig);
            if (_roundServerModel.Wins != null && _roundServerModel.Wins.Value != null)
            {
                _winClientModel.Amounts[_winClientModelCurrencyType] = (long) _roundServerModel.Wins.Value.Sum(_roundServerModelCurrencyType);
            }
        }
    }
}
