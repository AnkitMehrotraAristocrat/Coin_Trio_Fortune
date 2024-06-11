using PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using UnityEngine;
using PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// State machine presenter that will initialize the reel window with symbols starting at symbol
	/// index 0 for each reel.
	/// Use case is anytime you need the reel window to be reset to the starting symbols (i.e. - star 
	/// surge enter/exit).
	/// </summary>
	public class InitializeReelWindowPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		#region Inspector

        [Tooltip("Top row is considered symbol index 0")]
        [SerializeField] private int[] _defaultReelOffsets;

		#endregion

		#region Fields

		private const string SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY = "reelWindowInitialization";

		[FieldRequiresGlobal] protected ServiceLocator _serviceLocator = null;
		[FieldRequiresModel] protected SubSymbolEligibilityModel _subSymbolEligibilityModel = default;

		[FieldRequiresChild] private SymbolCyclingReelView[] _cyclingReelViews;
		[FieldRequiresChild] private RootReelView[] _rootReelViews;
		[FieldRequiresModel] private DynamicSymbolClientModel _dynamicSymbolClientModel = default;
		[FieldRequiresModel] private NextReelStripsClientModel _nextReelStripsClientModel = default;
		[FieldRequiresModel] private GameStateModel _gameStateModel = default;

        private List<int> _initialReelOffsets = new List<int>();

		#endregion

		public virtual void OnServicesLoaded()
		{
			this.InitializeDependencies();

			_initialReelOffsets = _defaultReelOffsets.ToList();
			if (_initialReelOffsets.Count < _cyclingReelViews.Length)
			{
				int paddingCount = _cyclingReelViews.Length - _initialReelOffsets.Count;
				_initialReelOffsets.AddRange(Enumerable.Repeat(0, paddingCount));
			}

			for (int reelIndex = 0; reelIndex < _cyclingReelViews.Length; ++reelIndex)
			{
				GameObject gameObject = _cyclingReelViews[reelIndex].gameObject;
				ISymbolView[] symbolViews = gameObject.GetComponentsInChildren<ISymbolView>();
				int bottomSymbolOffset = symbolViews.Where(view => view.Location.rowIndex > 0).Count();
				_initialReelOffsets[reelIndex] += bottomSymbolOffset;
			}
		}

		public string Tag => this.GetTag();

		public INotifier Notifier {
			get; set;
		}

		public IEnumerator<Yield> Enter()
        {
			SetSymbols();
			yield break;
		}

		public virtual void SetSymbols()
		{
            foreach (var cyclingReelView in _cyclingReelViews)
			{
                ReelsOutcomeModel outcomeModel = _serviceLocator.Get<ReelsOutcomeServerModel>(_gameStateModel.GameState.ToString());
				var symbolProvider = new CustomSymbolProvider(_serviceLocator, cyclingReelView.ReelIndex, _dynamicSymbolClientModel, _nextReelStripsClientModel, _initialReelOffsets[cyclingReelView.ReelIndex]);
                _nextReelStripsClientModel.ActiveReelStrips.Value = outcomeModel.ReelStripIds.Value;

				for (int symbolIndex = cyclingReelView.SymbolCount; --symbolIndex >= 0;)
				{
					SymbolId targetSymbol = symbolProvider.Consume();

					int reelId = _rootReelViews.FirstOrDefault(rootReelView => rootReelView.ReelIndex.Equals(cyclingReelView.ReelIndex)).GetInstanceID();
					_subSymbolEligibilityModel.SetPositionEligibility(reelId, symbolIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, false);

					cyclingReelView.SetSymbol(symbolIndex, targetSymbol);

					_subSymbolEligibilityModel.SetPositionEligibility(reelId, symbolIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, true);
				}

				symbolProvider.Dispose();
			}
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}
	}
}
