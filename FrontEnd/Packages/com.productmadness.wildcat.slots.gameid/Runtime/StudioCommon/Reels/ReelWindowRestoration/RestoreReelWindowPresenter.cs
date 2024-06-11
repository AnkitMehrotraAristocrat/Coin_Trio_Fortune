#region Using

using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;
using Coroutine = Milan.FrontEnd.Core.v5_1_1.Async.Coroutine;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.MetaEvents;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// A state machine presenter that restores a reel window to a stored ReelsOutcomeModel entry with a GameState value
	/// that matches the Tag applied. Choreography is configurable:
	/// - Swap all positions at once
	/// - Swap positions by row or column in a top to bottom or left to right direction (and reverse)
	/// </summary>
	[AddComponentMenu("Restore Reel Window Presenter")]
	public class RestoreReelWindowPresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
		#region Enum

		private enum SwapByIndexType
		{
			None,
			ByRowTopDown,
			ByRowBottomUp,
			ByReelLeftToRight,
			ByReelRightToLeft
		};

		#endregion

		#region Inspector

        [Tooltip("If enabled, the symbol restoration occurs row by row or column by column via AnimationEventForwarder " +
                 "method invocations using the PermitSymbolUpdate method. The animation clip(s) must have enough " +
                 "PermitSymbolUpdate invocations for the desired direction (i.e. - a 5 column game has 5 invocations if ByColumn " +
                 "is selected or a 4 row game has 4 invocations if ByRow is selected. If disabled, all symbols are restored in " +
                 "a single pass.")]
        [SerializeField] private bool _swapByIndex = false;

        [Tooltip("The type indicator to control the direction the symbol restoration executes in.")]
        [SerializeField] private SwapByIndexType _swapByIndexType = SwapByIndexType.None;

		#endregion

		#region Private Fields

		private const string SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY = "reelWindowRestoration";

		[FieldRequiresModel] private DynamicSymbolClientModel _dynamicSymbolClientModel = null;
		[FieldRequiresModel] private ReelsOutcomeClientModel _reelsOutcomeClientModel = null;
		[FieldRequiresModel] private SubSymbolEligibilityModel _subSymbolEligibilityModel = null;

		[FieldRequiresChild] private ISymbolView[] _symbolViews;
		[FieldRequiresChild] private RootReelView[] _rootReelViews;

		[FieldRequiresSelf(optional = true)] private IReelWindowModifier _reelWindowModifier;

        // The symbols in all positions
		private Dictionary<Location, SymbolId> _symbols = new Dictionary<Location, SymbolId>();

		// Rows or columns, dependent on if we are swapping by column/row indices or all at once
		private int[] _indices;

		// List of YieldUntilComplete(s) that will block the presenters flow until presenter's
		// PermitSymbolUpdate() method is invoked which will complete the yield. This yield approach
		// is how we can time when symbols of a given row / column index are swapped with an animation
		// clip having an animation event invoking an entry on the animation event forwarder. See N5009
		// SlotMachine->Transitions->FGTransition as an example (FGExitTransition).
		private List<YieldUntilComplete> _setSymbolYields = new List<YieldUntilComplete>();

		// Tracks which yieldComplete entry is next
		private int _yieldCompleteIndex;

        [FieldRequiresGlobal] private ServiceLocator _serviceLocator;
		private ReplacementsDict _dynamicSymbolReplacements = new ReplacementsDict();

		private MetaEventManager _metaEvents;

		#endregion

		#region Interface Implementations

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			_serviceLocator.TryGet(out _metaEvents);
			CheckTagPresent();
		}

		public string Tag => this.GetTag();

		public INotifier Notifier
		{
			get; set;
		}

		#endregion

		#region State Handling

		public IEnumerator<Yield> Enter()
		{
			yield return Coroutine.Start(ExecuteSymbolSwap());
		}

        public IEnumerator<Yield> Exit()
        {
            yield break;
        }

		#endregion

		#region Reel Window Restoration

		/// <summary>
		/// To be used in the AnimationEventForwarder.
		/// </summary>
		public void StartExecuteSymbolSwapCoroutine()
		{
			Coroutine.Start(ExecuteSymbolSwap());
		}

		private IEnumerator<Yield> ExecuteSymbolSwap()
		{
			SetSymbols();

			switch (_swapByIndexType)
			{
				case SwapByIndexType.ByReelLeftToRight:
					ResetModel(true, location => location.colIndex);
					break;
				case SwapByIndexType.ByReelRightToLeft:
					ResetModel(false, location => location.colIndex);
					break;
				case SwapByIndexType.ByRowTopDown:
					ResetModel(true, location => location.rowIndex);
					break;
				case SwapByIndexType.ByRowBottomUp:
					ResetModel(false, location => location.rowIndex);
					break;
				default:
					Debug.Assert(!_swapByIndex, "RestoreReelWindowPresenter." + Tag + " has Swap By Index enabled but the Swap By Index Type is None!");
					ResetModel(true, location => location.rowIndex);
					break;
			}

			yield return Coroutine.Start(SwapSymbols());
		}

		public void SetSymbols()
        {
            _symbols = new Dictionary<Location, SymbolId>();
            _dynamicSymbolReplacements = _dynamicSymbolClientModel.DynamicSymbolReplacements.Value;

            ReelWindowDataServerModel windowModel = _serviceLocator.Get<ReelWindowDataServerModel>(_reelsOutcomeClientModel.ReelWindowId.Value);
            int[] offsets = _reelsOutcomeClientModel.Offsets.Value;
            ReelStrip[] reelStrips = GetStrips(_reelsOutcomeClientModel.ReelStripIds.Value);

            for (int reelIndex = 0; reelIndex < windowModel.Width.Value; reelIndex++)
            {
                int offset = offsets[reelIndex];
                int stripSymbolCount = reelStrips[reelIndex].Symbols.Count;
                var symbolViews = _symbolViews.Where(view => view.Location.colIndex.Equals(reelIndex)).ToList();
                for (int symbolIndex = 0; symbolIndex < symbolViews.Count; symbolIndex++)
                {
                    int targetColumnIndex = symbolViews[symbolIndex].Location.colIndex;
                    int targetRowIndex = symbolViews[symbolIndex].Location.rowIndex;
                    int targetIndex = (offset + targetRowIndex + stripSymbolCount) % stripSymbolCount;

                    SymbolId symbol = reelStrips[reelIndex].Symbols[targetIndex];
                    ReplaceDynamicSymbol(ref symbol, targetColumnIndex);

                    Location location = new Location() { colIndex = targetColumnIndex, rowIndex = targetRowIndex };

                    _symbols.Add(location, symbol);
                }
            }

            if (_reelWindowModifier != null)
            {
                _reelWindowModifier.UpdateSymbols(ref _symbols, Tag);
            }
        }

		private void CheckTagPresent()
		{
			Debug.Assert(!string.IsNullOrEmpty(Tag), "RestoreReelWindowPresenter (" + gameObject.name + ") must be tagged! Must align name with target game state.");
		}

		private void ReplaceDynamicSymbol(ref SymbolId symbolId, int reelIndex)
		{
            if (_dynamicSymbolReplacements == null)
            {
                return;
            }
            if (!_dynamicSymbolReplacements.ContainsKey(reelIndex))
			{
				return;
			}
			if (!_dynamicSymbolReplacements[reelIndex].ContainsKey(symbolId))
			{
				return;
			}
			symbolId = _dynamicSymbolReplacements[reelIndex][symbolId];
		}

		private IEnumerator<Yield> SwapSymbols()
		{
			if (_symbols.Count == 0)
			{
				yield break;
			}

			if (_swapByIndex)
			{
				yield return Coroutine.Start(SwapByIndex());
			}
			else
			{
				SwapTargetSymbols(_symbols.ToArray());
				yield break;
			}
		}

		private IEnumerator<Yield> SwapByIndex()
		{
			switch (_swapByIndexType)
			{
				case SwapByIndexType.ByReelLeftToRight:
					yield return Coroutine.Start(StartSwapByIndexCoroutines(location => location.colIndex));
					break;
				case SwapByIndexType.ByReelRightToLeft:
					yield return Coroutine.Start(StartSwapByIndexCoroutines(location => location.colIndex));
					break;
				case SwapByIndexType.ByRowTopDown:
					yield return Coroutine.Start(StartSwapByIndexCoroutines(location => location.rowIndex));
					break;
				case SwapByIndexType.ByRowBottomUp:
					yield return Coroutine.Start(StartSwapByIndexCoroutines(location => location.rowIndex));
					break;
				default:
					yield break;
			}
		}

		private IEnumerator<Yield> StartSwapByIndexCoroutines(Func<Location, int> groupBy)
		{
			List<Yield> setSymbolYields = new List<Yield>();
			int yieldIndex = 0;
			foreach (int index in _indices)
			{
				var targetSymbols = _symbols.Where(symbol => groupBy(symbol.Key) == index).ToArray();
				setSymbolYields.Add(Coroutine.Start(SwapTargetSymbolsByIndex(yieldIndex, targetSymbols)));
				yieldIndex++;
			}
			yield return new WhenAll(setSymbolYields.ToArray());
		}

		private IEnumerator<Yield> SwapTargetSymbolsByIndex(int yieldIndex, KeyValuePair<Location, SymbolId>[] symbols)
		{
			yield return _setSymbolYields[yieldIndex];
			SwapTargetSymbols(symbols);
		}

		private void SwapTargetSymbols(KeyValuePair<Location, SymbolId>[] symbols)
		{
			foreach (var symbol in symbols)
			{
				SwapSymbol(symbol);
			}
		}

		private void SwapSymbol(KeyValuePair<Location, SymbolId> symbol)
		{
			Location location = symbol.Key;
			ISymbolView symbolToSwap = _symbolViews.Where(symbolView => symbolView.Location.Equals(location)).FirstOrDefault();
			SymbolId targetSymbol = symbol.Value;

			int reelId = _rootReelViews.FirstOrDefault(rootReelView => rootReelView.ReelIndex.Equals(location.colIndex)).GetInstanceID();
			_subSymbolEligibilityModel.SetPositionEligibility(reelId, location.rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, false);

			symbolToSwap.Set(targetSymbol, location.colIndex);

			BroadcastSymbolSpawnEvent(symbolToSwap.Instance.transform, location.colIndex, location.rowIndex, reelId);
			_subSymbolEligibilityModel.SetPositionEligibility(reelId, location.rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, true);
		}

        public void PermitSymbolUpdate()
		{
			if (!_swapByIndex)
			{
				GameIdLogger.Logger.Warning("RestoreReelWindowPresenter." + Tag + " :: No need to invoke PermitSymbolUpdate when the Swap By Index " +
				                                           "is set to disabled.");
				return;
			}

			if (_swapByIndexType == SwapByIndexType.ByReelLeftToRight || _swapByIndexType == SwapByIndexType.ByRowTopDown)
			{
				_setSymbolYields[_yieldCompleteIndex].Complete();
				UpdateYieldCompleteIndex(true);
			}
			else if (_swapByIndexType == SwapByIndexType.ByReelRightToLeft || _swapByIndexType == SwapByIndexType.ByRowBottomUp)
			{
				UpdateYieldCompleteIndex(false);
				_setSymbolYields[_yieldCompleteIndex].Complete();
			}
		}

		private void ResetModel(bool doesYieldIndexIncrease, Func<Location, int> indexBy)
		{
			InitIndices(indexBy);
			InitSymbolYields(doesYieldIndexIncrease);
		}

		private void InitIndices(Func<Location, int> indexBy)
		{
			_indices = _symbols.GroupBy(symbol => indexBy(symbol.Key))
					.ToDictionary(symbol => symbol.Key, symbol => symbol.ToList())
					.Keys.ToArray();
		}

		private void InitSymbolYields(bool doesYieldIndexIncrease)
		{
			_setSymbolYields = new List<YieldUntilComplete>();
			for (int symbolIndex = 0; symbolIndex < _indices.Length; symbolIndex++)
			{
				_setSymbolYields.Add(new YieldUntilComplete());
			}
			_yieldCompleteIndex = doesYieldIndexIncrease ? 0 : _indices.Length;
		}

		private void UpdateYieldCompleteIndex(bool shouldIncreaseIndex)
		{
			if (shouldIncreaseIndex)
			{
				IncreaseYieldCompleteIndex();
			}
			else
			{
				DecreaseYieldCompleteIndex();
			}
		}

		private void DecreaseYieldCompleteIndex()
		{
			if (_yieldCompleteIndex > 0)
			{
				_yieldCompleteIndex--;
			}
		}

		private void IncreaseYieldCompleteIndex()
		{
			int targetIndex = _yieldCompleteIndex + 1;
			if (targetIndex < _indices.Length)
			{
				_yieldCompleteIndex++;
			}
		}

		private void BroadcastSymbolSpawnEvent(Transform symbolTransform, int reelIndex, int rowIndex, int reelId)
		{
			_metaEvents?.Broadcast(new SymbolSpawn
			{
				symbolTransform = symbolTransform,
				reelIndex = reelIndex,
				rowIndex = rowIndex,
				reelId = reelId
			});
		}

        private ReelStrip[] GetStrips(string[] stripIds)
        {
            return stripIds
                .Select(stripId =>
                {
                    return _serviceLocator.Get<ReelStripDataServerModel>(stripId).Strip.Value;
                })
                .ToArray();
        }


        #endregion
    }
}
