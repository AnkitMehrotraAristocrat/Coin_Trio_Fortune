using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.FreespinCore;
using Milan.FrontEnd.Feature.v5_1_1.Utility;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID {
	/// <summary>
	/// A state machine presenter that handles free spins intro presentation.
	/// </summary>
	public class FreeGamesIntroPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler {
		[Preserve]
		[Serializable]
		private class AnimParams {
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

		[SerializeField] private float _freeGamesIntroWait = 8.0F;
		[SerializeField] private TextMeshPro _freeSpinCountText;

		[SerializeField] private AnimParams _animationParams;
		[SerializeField] private string _spinPressComponentTag = "SpinShortPress";

		private int _spinsWon;
		private List<Coroutine> _introYieldables;
		private bool _playButtonPressed = false;
		private List<IConsumable> _spinPresses = new List<IConsumable>();

		public string Tag => this.GetTag();

		public INotifier Notifier {
			private get; set;
		}

		public void OnServicesLoaded() {
			this.InitializeDependencies();

			_freeSpinServerModel.SpinsWon.Subscribe(val => {
				_spinsWon = val;
			}).AddTo(this);

			_spinPresses.AddRange(transform.root.GetComponentsInChildren<IConsumable>(_spinPressComponentTag, true));
		}

		public IEnumerator<Yield> Enter() {
			_animator.gameObject.SetActive(true);
			_animator.SetTrigger(_animationParams.IntroTrigger);
			_freeSpinCountText.text = _spinsWon.ToString();

			yield return _animator.WhenStateEnter(_animationParams.LoopTag);

			_playButtonPressed = false;
			_introYieldables = new List<Coroutine>();

			_introYieldables.Add(Coroutine.Start(StartSpinButtonPressYields()));
			_introYieldables.Add(Coroutine.Start(StartWaitForButtonPress()));
			_introYieldables.Add(Coroutine.Start(StartIntroTimeout()));

			yield return new OnlyFirst(_introYieldables.ToArray());

			if (_playButtonPressed) {
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

		private IEnumerator<Yield> StartWaitForButtonPress() {
			_button.Reset();
			yield return Coroutine.Start(_button.Consume());
			_playButtonPressed = true;
		}

		private IEnumerator<Yield> StartIntroTimeout() {
			yield return new YieldForSeconds(_freeGamesIntroWait);
		}

		public IEnumerator<Yield> Exit() {
			_animator.SetTrigger(_animationParams.OutroTrigger);
			yield return _animator.WhenStateEnter(_animationParams.IdleTag);
			_animator.gameObject.SetActive(false);
			yield break;
		}
	}
}
