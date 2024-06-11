using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using Milan.FrontEnd.Slots.v5_1_1.Meta;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// State machine presenter that will deduct the spin count on the bonus spin frame.
	/// Use case, this is placed on a state machine node so it can be processed before
	/// the spin response is received such that the count is instantly deducted when a spin
	/// is started (no server latency involved).
	/// </summary>
	public class DeductBonusSpinPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler 
    {
		[SerializeField] private string _frameTag = "";
		private IBonusSpinFrame _bonusSpinFrame = null;

		public string Tag => this.GetTag();

		public INotifier Notifier 
        {
			private get; set;
		}

		public void OnServicesLoaded() 
        {
			this.InitializeDependencies();

            Coroutine.Start(GetBonusSpinFrame());
        }

		public IEnumerator<Yield> Enter() 
        {
			_bonusSpinFrame.Count.Value--;
			yield break;
		}

		public IEnumerator<Yield> Exit() 
        {
			yield break;
		}

        private IEnumerator<Yield> GetBonusSpinFrame()
        {
            yield return new YieldForSeconds(0.1f);

            _bonusSpinFrame = GetComponentInParent<ServiceLocator>().Get<IBonusSpinFrame>(_frameTag);
		}
    }
}
