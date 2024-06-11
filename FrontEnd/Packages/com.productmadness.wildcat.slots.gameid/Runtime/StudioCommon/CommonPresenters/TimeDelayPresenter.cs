using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.WinCore;
using System.Collections.Generic;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// State machine presenter that will block state machine flow for the prescribed amount of time
	/// based on the configured serialized fields.
	/// </summary>
	public class TimeDelayPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		private enum DelayWhen
		{
			Enter,
			Exit
		}

		[SerializeField] private RoundWinAmountProvider _winAmountProvider;

		[Tooltip("Amount to delay in seconds (float)")]
		[SerializeField] private float _delay = 0.5f;

		[SerializeField] private DelayWhen _delayWhen = DelayWhen.Enter;
		[SerializeField] private bool _winsOnly = false;

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
			if (_delayWhen == DelayWhen.Enter)
			{
				yield return Coroutine.Start(StartDelay());
			}
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			if (_delayWhen == DelayWhen.Exit)
			{
				yield return Coroutine.Start(StartDelay());
			}
			yield break;
		}

		private IEnumerator<Yield> StartDelay()
		{
			if (_winsOnly && _winAmountProvider.WinAmount > 0)
			{
				yield return new YieldForSeconds(_delay);
			}
			else if (!_winsOnly)
			{
				yield return new YieldForSeconds(_delay);
			}
			yield break;
		}
	}
}
