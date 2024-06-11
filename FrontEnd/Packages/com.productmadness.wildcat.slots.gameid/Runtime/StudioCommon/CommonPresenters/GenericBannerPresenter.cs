using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Feature.v5_1_1.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class GenericBannerPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{ // or could inherit from the BaseStatePresenter
		[Preserve]
		[Serializable]
		private class AnimParams
		{
			public string IntroTrigger;
			public string OutroTrigger;
			public string IdleTag;
			public string LoopTag;
			public string ButtonPressTrigger;
			public string ButtonIdleTag;
		}

		[FieldRequiresChild(includeInactive = true)] private Animator _animator;
		[FieldRequiresChild(includeInactive = true)] private ColliderButton _button;

		[SerializeField] private float _timeOut = 8.0F;
		[SerializeField] private AnimParams _animationParams;
		[SerializeField] private string _spinPressComponentTag = "SpinShortPress";

		private List<Coroutine> _outroYieldables;
		private bool _exitButtonPressed = false;
		private List<IConsumable> _spinPresses = new List<IConsumable>();


		public string Tag => this.GetTag();

		public INotifier Notifier
		{
			private get; set;
		}

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			_spinPresses.AddRange(transform.root.GetComponentsInChildren<IConsumable>(_spinPressComponentTag, true));
		}

		public IEnumerator<Yield> Enter()
		{
			_animator.gameObject.SetActive(true);
			_animator.SetTrigger(_animationParams.IntroTrigger);

			yield return _animator.WhenStateEnter(_animationParams.LoopTag);

			_exitButtonPressed = false;
			_outroYieldables = new List<Coroutine>();
			_outroYieldables.Add(Coroutine.Start(StartWaitForButtonPress()));
			_outroYieldables.Add(Coroutine.Start(StartSpinButtonPressYields()));
			if (_timeOut > 0)
			{
				_outroYieldables.Add(Coroutine.Start(StartOutroTimeout()));
			}

			yield return new OnlyFirst(_outroYieldables.ToArray());

			if (_exitButtonPressed)
			{
				_animator.SetTrigger(_animationParams.ButtonPressTrigger);
				yield return _animator.WhenStateEnter(_animationParams.ButtonIdleTag);
			}
			yield break;
		}

		private IEnumerator<Yield> StartSpinButtonPressYields()
		{
			_spinPresses.ForEach(shortPress => shortPress.Reset());
			IEnumerable<IEnumerator<Yield>> whenAnyEventYields = _spinPresses.ToYields(shortPress => shortPress.Consume());
			WhenAny whenAnyYield = new WhenAny(whenAnyEventYields);
			yield return whenAnyYield;
		}

		private IEnumerator<Yield> StartWaitForButtonPress()
		{
			_button.Reset();
			yield return Coroutine.Start(_button.Consume());
			_exitButtonPressed = true;
		}

		private IEnumerator<Yield> StartOutroTimeout()
		{
			yield return new YieldForSeconds(_timeOut);
		}

		public IEnumerator<Yield> Exit()
		{
			_animator.SetTrigger(_animationParams.OutroTrigger);
			yield return _animator.WhenStateEnter(_animationParams.IdleTag);
			_animator.gameObject.SetActive(false);
			yield break;
		}
	}
}
