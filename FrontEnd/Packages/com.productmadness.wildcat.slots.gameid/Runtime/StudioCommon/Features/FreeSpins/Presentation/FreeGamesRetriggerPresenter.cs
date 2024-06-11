using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Feature.v5_1_1.Utility;
using Milan.FrontEnd.Slots.v5_1_1.FreespinCore;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class FreeGamesRetriggerPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
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
		[FieldRequiresModel] private FreeSpinServerModel _freeSpinServerModel;

		[SerializeField] private float _freeGamesRetriggerWait = 3.0F;
		[SerializeField] private TextMeshPro _freeSpinCountText;
		[SerializeField] private AnimParams _animationParams;
		[SerializeField] private string _playButtonTag;
		[SerializeField] private string _screenButtonTag;
		[SerializeField] private string _spinPressComponentTag = "SpinShortPress";

		private int _spinsWon;
		private List<Coroutine> _retriggerYieldables;
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

			_freeSpinServerModel.SpinsWon.Subscribe(val =>
			{
				_spinsWon = val;
			}).AddTo(this);

			_spinPresses.AddRange(transform.root.GetComponentsInChildren<IConsumable>(_spinPressComponentTag, true));
		}

		public IEnumerator<Yield> Enter()
		{
			_animator.gameObject.SetActive(true);
			_animator.SetTrigger(_animationParams.IntroTrigger);
			_freeSpinCountText.text = '+' + _spinsWon.ToString();

			yield return _animator.WhenStateEnter(_animationParams.LoopTag);

			_exitButtonPressed = false;

			_retriggerYieldables = new List<Coroutine>();

			// TODO: Check if we have these buttons
			// Start the StartWaitForBannerButtonPress coroutine and add it to our yieldables array
			// Start the StartWaitForScreenPress coroutine and add it to our yieldables array
			_retriggerYieldables.Add(Coroutine.Start(StartSpinButtonPressYields()));
			if (_freeGamesRetriggerWait > 0)
			{
				_retriggerYieldables.Add(Coroutine.Start(StartRetriggerTimeout()));
			}

			yield return new OnlyFirst(_retriggerYieldables.ToArray());

			if (_exitButtonPressed)
			{
				_animator.SetTrigger(_animationParams.ButtonPressTrigger);
				yield return _animator.WhenStateEnter(_animationParams.ButtonIdleTag);
			}
		}

		private IEnumerator<Yield> StartWaitForBannerButtonPress()
		{
			// TODO: Implement if buttons are required
			// set a local variable "button" equal to the button entry in _buttons that has a matching _playButtonTag tag
			// Start a coroutine that waits for the banner button to be pressed and yield on it
			// Set the _playButtonPressed flag to true
			yield break;
		}

		private IEnumerator<Yield> StartSpinButtonPressYields()
		{
			_spinPresses.ForEach(shortPress => shortPress.Reset());
			IEnumerable<IEnumerator<Yield>> whenAnyEventYields = _spinPresses.ToYields(shortPress => shortPress.Consume());
			WhenAny whenAnyYield = new WhenAny(whenAnyEventYields);
			yield return whenAnyYield;
		}

		private IEnumerator<Yield> StartWaitForScreenPress()
		{
			// TODO: Implement if buttons are required
			// set a local variable "button" equal to the button entry in _buttons that has a matching _screenButtonTag tag
			// Start a coroutine that waits for the screen (big button) to be pressed and yield on it
			yield break;
		}

		private IEnumerator<Yield> StartRetriggerTimeout()
		{
			yield return new YieldForSeconds(_freeGamesRetriggerWait);
		}

		public IEnumerator<Yield> Exit()
		{
			_animator.SetTrigger(_animationParams.OutroTrigger);
			yield return _animator.WhenStateEnter(_animationParams.IdleTag);
			_animator.gameObject.SetActive(false);
		}
	}
}
