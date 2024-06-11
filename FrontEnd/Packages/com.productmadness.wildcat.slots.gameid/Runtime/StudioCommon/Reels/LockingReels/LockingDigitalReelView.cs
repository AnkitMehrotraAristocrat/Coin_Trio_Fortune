using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.LockingReels
{
	public class LockingDigitalReelView : BaseModalReelView, IModalReelView, ServiceLocator.IHandler, IRecoveryHandler, ISpinResponder
	{
		[FieldRequiresChild] BaseLockingReelsProvider[] _lockingReelProviders;
		[FieldRequiresChild] private Animator _animator = null;

		[SerializeField][Reorderable] private DigitalReelModeSOList _starts = null;
		[SerializeField][Reorderable] private DigitalReelModeSOList _stops = null;
		[SerializeField][Reorderable] private DigitalReelModeSOList _quickStops = null;

		[SerializeField] private float _loopTime = 1f;
		[SerializeField] private string _idleStateTag = "idle";
		[SerializeField] private string _loopStateTag = "spinLoop";

		private int _lockingReelIndex;

		private bool _shouldReelSpin = true;
		private bool _initialized = false;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			_lockingReelIndex = Array.Find(transform.GetComponentsInChildren<PooledSymbolView>(), p => p.Location.rowIndex == 0).Location.colIndex;
		}

		public void OnServerConfigsReady(ServiceLocator _) { }

		public void OnInitialStateReady(ServiceLocator locator)
		{
			SetShouldReelSpin();
			_initialized = true;
		}

		private void OnEnable()
		{
			if (!_initialized)
			{
				return;
			}

			SetShouldReelSpin();
		}

		public override IEnumerator<Yield> Spin(string reelMode)
		{
			_animator.ResetTriggers();

			if (!_shouldReelSpin)
			{
				yield break;
			}
			else
			{
				NotifyOnReelSpin();

				var spin = _starts.First(s => s.reelMode == reelMode);

				_animator.SetTrigger(spin.trigger);

				yield return _animator.WhenStateEnter(_loopStateTag);

				IsLooping = IsSpinning = true;

				yield return new YieldForSeconds(_loopTime);

				IsLooping = false;
			}
		}

		public override IEnumerator<Yield> Stop(string reelMode, int offset)
		{
			var stop = _stops.First(s => s.reelMode == reelMode);
			return StopImplementation(reelMode, offset, stop);
		}

		public override IEnumerator<Yield> QuickStop(string reelMode, int offset)
		{
			var quickStop = _quickStops.First(s => s.reelMode == reelMode);
			return StopImplementation(reelMode, offset, quickStop);
		}

		private IEnumerator<Yield> StopImplementation(string reelMode, int offset, DigitalReelModeSO mode)
		{
			if (!_shouldReelSpin)
			{
				yield break;
			}
			else
			{
				if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) //TODO: Update this to N5022's approach with the complex digital reel view?
				{
					yield break;
				}

				_animator.SetTrigger(mode.trigger);

				yield return _animator.WhenStateEnter(_idleStateTag);

				IsSpinning = false;

				NotifyOnReelStop();
			}
		}

		public void SetShouldReelSpin()
		{
			_shouldReelSpin = true;

			foreach (var lockingReelProvider in _lockingReelProviders)
			{
				if (!lockingReelProvider.ShouldReelSpin(_lockingReelIndex))
				{
					_shouldReelSpin = false;
					break;
				}
			}
		}

		public IEnumerator<Yield> SpinStarted()
		{
			// does nothing
			yield break;
		}

		public IEnumerator<Yield> SpinComplete()
		{
			SetShouldReelSpin();
			yield break;
		}
	}

	[Serializable] public class DigitalReelModeSOList : ReorderableArray<DigitalReelModeSO> { }
}
