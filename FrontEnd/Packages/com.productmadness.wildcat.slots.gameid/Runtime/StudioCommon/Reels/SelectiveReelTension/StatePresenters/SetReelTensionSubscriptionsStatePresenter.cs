using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	public class SetReelTensionSubscriptionsStatePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		[FieldRequiresChild] private ReelTensionView _reelTensionView;

		public string Tag => this.GetTag();

		public INotifier Notifier
		{
			get; set;
		}

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> Enter()
		{
			_reelTensionView.SetSpinSubscriptions();
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}
	}
}
