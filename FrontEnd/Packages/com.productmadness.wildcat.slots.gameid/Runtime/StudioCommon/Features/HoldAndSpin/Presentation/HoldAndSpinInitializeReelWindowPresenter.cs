using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.PositionMaps;
using System;
using System.Linq;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.HoldAndSpin
{
    [Serializable]
    public class GameStateReelWindowPair
    {
        public string gameState;
        public string reelWindow;
    }

    [Serializable]
    public class GameStateReelWindowPairs : ReorderableArray<GameStateReelWindowPair> { }

    public class HoldAndSpinInitializeReelWindowPresenter :InitializeReelWindowPresenter
	{
		private const string SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY = "holdAndSpinReelWindowInitialization";
        private const string HOLD_AND_SPIN_GAME_STATE_NAME = "HoldAndSpin";

		private HoldAndSpinClientModel _holdAndSpinModel;
		[FieldRequiresChild] private ISymbolView[] _symbolViews;

        [SerializeField] private string _clientTag = "";
        [SerializeField] private int _holdAndSpinReelsHeight;
        [SerializeField][Reorderable] private GameStateReelWindowPairs _gameStateReelWindowPairs;
        public override void OnServicesLoaded()
		{
			base.OnServicesLoaded();
			_symbolViews = GetComponentsInChildren<PooledSymbolView>();
            _holdAndSpinModel = _serviceLocator.Get<HoldAndSpinClientModel>(_clientTag);
		}

        public override void SetSymbols()
        {
            var sourceReelWindowName = _gameStateReelWindowPairs.FirstOrDefault(pair => pair.gameState.Equals(_holdAndSpinModel.Data.TriggeringState)).reelWindow;
            var targetReelWindowName = _gameStateReelWindowPairs.FirstOrDefault(pair => pair.gameState.Equals(HOLD_AND_SPIN_GAME_STATE_NAME)).reelWindow;

            ReelsOutcomeServerModel triggeringOutcomeModel = _serviceLocator.Get<ReelsOutcomeServerModel>(_holdAndSpinModel.Data.TriggeringState);

            PositionMapModel positionMapModel = _serviceLocator.Get<PositionMapModel>();

            SymbolId[][] visibleReels = triggeringOutcomeModel.Symbols;

            for (int reelIndex = 0; reelIndex < visibleReels.Length; ++reelIndex)
            {
                for (int symbolIndex = 0; symbolIndex < visibleReels[reelIndex].Length; ++symbolIndex)
                {
                    var targetWindowPosition = positionMapModel.GetPositionMap(sourceReelWindowName, targetReelWindowName, visibleReels[reelIndex].Length, _holdAndSpinReelsHeight, new WindowPosition(reelIndex, symbolIndex));

                    ISymbolView symbolToSwap = _symbolViews.FirstOrDefault(view => view.Location.colIndex.Equals(targetWindowPosition.ReelIndex) && view.Location.rowIndex.Equals(targetWindowPosition.SymbolIndex));
                    SymbolId targetSymbol = visibleReels[reelIndex][symbolIndex];

                    var starResponder = symbolToSwap.Instance.GetComponent<StarSubSymbolVisibilityResponder>();
                    int reelId = starResponder.RootReelView.GetInstanceID();
                    int rowIndex = starResponder.Location.rowIndex;

                    _subSymbolEligibilityModel.SetPositionEligibility(reelId, rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, false);
                    starResponder.SymbolCyclingReelView.SetSymbol(starResponder.SymbolCyclingReelViewIndex, targetSymbol);
                    _subSymbolEligibilityModel.SetPositionEligibility(reelId, rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, true);
                }
            }
        }
    }
}
