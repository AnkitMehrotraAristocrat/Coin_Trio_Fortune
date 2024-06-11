using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using Milan.FrontEnd.Slots.v5_1_1.Meta;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class BonusSpinEndPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler 
    {
		[SerializeField] private string _bonusSpinFrameTag = "";

		public string Tag => this.GetTag();

		public INotifier Notifier 
        {
			private get; set;
		}

		private IBonusSpinFrame _bonusSpinFrame;

		public void OnServicesLoaded() 
        {
            Coroutine.Start(GetBonusSpinFrame());
		}

		public IEnumerator<Yield> Enter() 
        {
			DisableBonusSpinFrame();
			yield break;
		}

		public IEnumerator<Yield> Exit() 
        {
			yield break;
		}

		public void DisableBonusSpinFrame() 
        {
			_bonusSpinFrame.IsActive.Value = false;
		}

        private IEnumerator<Yield> GetBonusSpinFrame()
        {
            yield return new YieldForSeconds(0.1f);

			var serviceLocator = GetComponentInParent<ServiceLocator>();
            _bonusSpinFrame = serviceLocator.Get<IBonusSpinFrame>(_bonusSpinFrameTag);
		}
	}
}
