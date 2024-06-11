using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class ResetWinFramePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private WinClientModel _winClientModel = null;
        [FieldRequiresGlobal] private WinStartEventBroadcaster _eventBroadcaster = null;

        [SerializeField] private string _currencyType = "chips";
        [SerializeField] private WinStartData _winStartConfig;

        public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> Enter()
		{
            ResetWinClientModel();
            yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}

        public void ResetWinClientModel()
        {
            _eventBroadcaster.Broadcast(_winStartConfig);
            _winClientModel.Amounts[_currencyType] = 0;
        }

        public string Tag => this.GetTag();
		public INotifier Notifier
		{
			get; set;
		}
	}
}
