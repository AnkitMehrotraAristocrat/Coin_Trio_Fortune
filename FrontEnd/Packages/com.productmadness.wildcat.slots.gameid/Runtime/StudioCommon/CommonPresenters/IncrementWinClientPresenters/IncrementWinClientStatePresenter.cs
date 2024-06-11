using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class IncrementWinClientStatePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		public enum EnterOrExit
		{
			Enter,
			Exit
		}

		[FieldRequiresModel] private WinClientModel _winClientModel = default;
        [FieldRequiresGlobal] private WinStartEventBroadcaster _eventBroadcaster = null;

        [SerializeField] private string _winClientModelCurrencyType = "chips";
		[SerializeField] private EnterOrExit _enterOrExit;
		[SerializeField] private RoundWinAmountProvider _roundWinAmountProvider;
        [SerializeField] private WinStartData _winStartConfig;

        public string Tag => this.GetTag();

		public INotifier Notifier
		{
			private get; set;
		}

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> Enter()
		{
			if (_enterOrExit.Equals(EnterOrExit.Enter))
			{
				UpdateWinClientModel();
			}
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			if (_enterOrExit.Equals(EnterOrExit.Exit))
			{
				UpdateWinClientModel();
			}
			yield break;
		}

		public void UpdateWinClientModel()
		{
            _eventBroadcaster.Broadcast(_winStartConfig);
            _winClientModel.Amounts[_winClientModelCurrencyType] += _roundWinAmountProvider.WinAmount;
		}
	}
}
