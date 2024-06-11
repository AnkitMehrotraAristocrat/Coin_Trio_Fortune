using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData;
using PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin;
using PixelUnited.NMG.Slots.Milan.GAMEID.Blackout;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps;
using System;
using Malee;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[Serializable]
	public class GameStateReelWindowPair
	{
		public string gameState;
		public string reelWindow;
	}

	[Serializable]
	public class GameStateReelWindowPairs : ReorderableArray<GameStateReelWindowPair> { }

	public class AnimateCORSymbolsPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{		
		[FieldRequiresModel] private SymbolOutcomeModel _symbolOutcomeModel = default;
		[FieldRequiresModel] private ReelsOutcomeClientModel _reelsOutcomeClientModel = default;
		[FieldRequiresModel] private GameStateModel _gameStateModel = default;

		[FieldRequiresGlobal] private ServiceLocator _serviceLocator;

		[FieldRequiresChild] private AnimateSymbolView _view;
		[FieldRequiresChild] private ISymbolView[] _symbolViews;

		[SerializeField] private string _holdAndSpinClientModelTag;
		[SerializeField] private string _blackoutClientModelTag;
		[SerializeField] private string _preAnimTrigger;
		[SerializeField] private string _preAnimTag;
		[SerializeField] private string _animTrigger;
		[SerializeField] private bool _convertToStandardLocation;
		[SerializeField] private string _standardReelWindowName;
		[SerializeField] private int _holdAndSpinReelsHeight;
		[SerializeField] private float _randomAnimationOffsetRange = 0.0f;
		[SerializeField][Reorderable] private GameStateReelWindowPairs _gameStateReelWindowPairs;

		private HoldAndSpinClientModel _holdAndSpinClientModel = default;
		private BlackoutClientModel _blackoutClientModel = default;
		private Dictionary<int, YieldUntilComplete> _symbolAnimYields = new Dictionary<int, YieldUntilComplete>();

		public string Tag => this.GetTag();

		public INotifier Notifier
		{
			get; set;
		}

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			_holdAndSpinClientModel = _serviceLocator.Get<HoldAndSpinClientModel>(_holdAndSpinClientModelTag);
			_blackoutClientModel = _serviceLocator.Get<BlackoutClientModel>(_blackoutClientModelTag);
		}

		public IEnumerator<Yield> Enter()
		{
			if (_holdAndSpinClientModel.Data.FreeSpinsRemaining == 0 && !_blackoutClientModel.Data.Blackout)
			{
				yield break;
			}

			if (_convertToStandardLocation)
			{
				PlayConvertedCORAnimations();
			}
			else
			{
				PlayCORAnimations();
			}
			yield return new WhenAll(_symbolAnimYields.Values.GetEnumerator());
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}

		/// <summary>
		/// Instructs the view to animate symbols using hold and spin coordinates
		/// </summary>
		public void PlayCORAnimations()
		{
			_symbolAnimYields = new Dictionary<int, YieldUntilComplete>();

			foreach (var symbol in _symbolOutcomeModel.GetAllPrizes(_gameStateModel.GameState))
			{
				var symbolView = _symbolViews.FirstOrDefault(view => view.Location.colIndex.Equals(symbol.PositionData.X) && view.Location.rowIndex.Equals(symbol.PositionData.Y));
				//_view.AnimateSymbol(symbolView.Instance, _animTrigger);
				_symbolAnimYields.Add(symbolView.Instance.GetInstanceID(), new YieldUntilComplete());
				Coroutine.Start(PlaySymbolAnimation(symbolView.Instance));
			}
		}

		/// <summary>
		/// Instructs the view to animate symbols using standard coordinates
		/// </summary>
		public void PlayConvertedCORAnimations()
		{
			_symbolAnimYields = new Dictionary<int, YieldUntilComplete>();
       
			var standardVisibleReels = _serviceLocator.Get<ReelsOutcomeServerModel>(_standardReelWindowName).Symbols;
			var positionMapModel = _serviceLocator.Get<PositionMapModel>();

            // Fetch the LockedWysiwygs from the model's CurrentFeature
            // Loop through each LockedWysiwyg and pass it to the view for animation execution (passing in the _animTrigger string)
            foreach (var lockedsymbol in _symbolOutcomeModel.GetAllPrizes(_gameStateModel.GameState))
            {
				var targetReelHeight = standardVisibleReels[lockedsymbol.PositionData.X].Length;

				var sourceWindowPosition = new WindowPosition(lockedsymbol.PositionData.X, lockedsymbol.PositionData.Y);
				var targetWindowPosition = positionMapModel.GetPositionMap(_gameStateModel.GameState.ToString(), _standardReelWindowName, _holdAndSpinReelsHeight, targetReelHeight, sourceWindowPosition);

                var symbolView = _symbolViews.FirstOrDefault(symbol => symbol.Location.colIndex.Equals(targetWindowPosition.ReelIndex) && symbol.Location.rowIndex.Equals(targetWindowPosition.SymbolIndex));

                _symbolAnimYields.Add(symbolView.Instance.GetInstanceID(), new YieldUntilComplete());
                Coroutine.Start(PlaySymbolAnimation(symbolView.Instance));
            }
        }

		private IEnumerator<Yield> PlaySymbolAnimation(SymbolHandle instance)
		{
			if (!string.IsNullOrEmpty(_preAnimTrigger))
			{
				yield return Coroutine.Start(_view.AnimateSymbol(instance, _preAnimTrigger, _preAnimTag));
				_symbolAnimYields[instance.GetInstanceID()].Complete();
			}

			if (_randomAnimationOffsetRange > 0.0f)
			{
				float randomizedOffset = UnityEngine.Random.Range(0f, _randomAnimationOffsetRange);
				yield return new YieldForSeconds(randomizedOffset);
			}
			Coroutine.Start(_view.AnimateSymbol(instance, _animTrigger));
		}
	}
}
