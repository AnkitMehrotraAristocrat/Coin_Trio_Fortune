using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1.Data;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// State machine presenter that will block state machine flow for the prescribed amount of time
	/// if autospin is enabled.
	/// </summary>
	public class DelayAutospinPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		[FieldRequiresModel(tag = "SpinFrame")] private Frame _spinFrame = null;

		[SerializeField] float autospinWinDelay = 1.5f;

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
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			if (_spinFrame.Get<bool>("autospin"))
			{
				yield return new YieldForSeconds(autospinWinDelay);
			}
			yield break;
		}
	}
}
