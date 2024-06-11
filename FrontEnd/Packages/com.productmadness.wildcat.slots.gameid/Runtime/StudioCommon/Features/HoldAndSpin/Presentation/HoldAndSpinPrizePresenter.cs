using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Feature.v5_1_1.Audio;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.ArtUtil;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
	public class HoldAndSpinPrizePresenter : BaseStatePresenter, IStatePresenter, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel = default;
		[FieldRequiresModel] private WinMeterModel _winMeterModel = default;

		[FieldRequiresParent] private AudioEventBindings _audioEventBindings;

		[FieldRequiresChild] private ISymbolView[] _symbolViews;
		[FieldRequiresChild] private AnimateSymbolView _symbolAnimationView;

		[SerializeField] private string _winAnimTrigger;
		[SerializeField] private GameObject _doober; // doober has a modified FlyUpMovement script
		[SerializeField] private Transform _dooberTarget;

		[SerializeField] private string _dooberStartSfx;
		[SerializeField] private string _dooberHitSfx;

		public override IEnumerator<Yield> Enter()
		{
			if (_action.Equals(Action.OnEnter))
			{
				yield return Coroutine.Start(RackCurrentPrize());
			}
			yield break;
		}

		public override IEnumerator<Yield> Exit()
		{
			if (_action.Equals(Action.OnExit))
			{
				yield return Coroutine.Start(RackCurrentPrize());
			}
				yield break;
		}

        private IEnumerator<Yield> RackCurrentPrize()
		{
			// Fetch the currentPrize from the _featureModel
			var currentPrize = _symbolOutcomeModel.CurrentPrize;

			// Using the currentPrize info, fetch the instance of the this symbol from _symbolViews using the Location
			SymbolHandle symbol = _symbolViews.FirstOrDefault(view => view.Location.colIndex.Equals(currentPrize.PositionData.X) && view.Location.rowIndex.Equals(currentPrize.PositionData.Y))?.Instance;

			// Position the _doober on top of the symbol instance
			AutomatedFlyUp dooberFlyUp = _doober.GetComponent<AutomatedFlyUp>();

			// Pass the symbol instance and _winAnimTrigger to the AnimateCORSymbolView
			Coroutine.Start(_symbolAnimationView.AnimateSymbol(symbol, _winAnimTrigger));

			// Yield on the _doober's StartFlyUp() method (to make it play)
			dooberFlyUp.SetTargetTransform(_dooberTarget);
			dooberFlyUp.PositionPrefab(symbol.transform.position);

			// yield for doober anim and despawn
			PlayAudio(_dooberStartSfx);
			yield return Coroutine.Start(dooberFlyUp.MoveOnAnimationCurve());
			PlayAudio(_dooberHitSfx);

			// Update the _winMeterModel's current value (thus invoking it's view's hit animation) <-- should occur right as the particles hit
			_winMeterModel.WinAmount.Value += Convert.ToInt64(currentPrize.SymbolData.TextValue);
		}

		private void PlayAudio(string eventName)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				_audioEventBindings.Play(eventName);
			}
		}
	}
}
