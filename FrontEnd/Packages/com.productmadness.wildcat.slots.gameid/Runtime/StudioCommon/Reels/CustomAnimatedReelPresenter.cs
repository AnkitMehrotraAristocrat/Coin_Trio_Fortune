using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips;
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using Milan.FrontEnd.Core.v5_1_1.Meta;
using Milan.FrontEnd.MetaEvents;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Provides hooks for animators to trigger symbol recycles.
	/// Spinoff of the AnimatedReelPresenter to support reel mode / dynamic symbols.
	/// </summary>
	public class CustomAnimatedReelPresenter : MonoBehaviour, ISymbolSpawnNotifier, ServiceLocator.IHandler, IRecoveryHandler, IReelSymbolsPresenter
	{
		private const string SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY = "nextReelStripsInitialStateReady";

		[Preserve]
		[Serializable]
		public class EligibleGameStates : ReorderableArray<GameStateEnum> { }

		[FieldRequiresModel] protected GameStateModel _gameStateModel = null;
		[FieldRequiresModel] protected ReelsOutcomeClientModel _reelsOutcomeClientModel = null;
		[FieldRequiresModel] protected DynamicSymbolClientModel _dynamicSymbolClientModel = null;
		[FieldRequiresModel] protected NextReelStripsClientModel _nextReelStripsClientModel = null;
		[FieldRequiresModel] protected SubSymbolEligibilityModel _subSymbolEligibilityModel = null;

		[FieldRequiresParent] protected RootReelView _rootReelView = null;
		[FieldRequiresParent] protected ISymbolCyclingReelView _cycleReelView = null;
		[FieldRequiresParent(optional = true)] protected IActiveReelsProvider _activeReelsProvider = null;

		[FieldRequiresChild] protected ISymbolView[] _symbolViews;

		[SerializeField, Reorderable] protected EligibleGameStates _gameStates;
		[SerializeField] protected int _defaultOffset = 0;

        protected MetaEventManager _metaEvents;
        protected ServiceLocator _serviceLocator;
        protected CustomSymbolProvider _symbolProvider;
        protected int _bottomSymbolOffset = 0;

		public ReactiveProperty<SymbolId> SpawnedSymbol { get; protected set; } = new ReactiveProperty<SymbolId>();
		public ReactiveProperty<SymbolId> DespawnedSymbol { get; protected set; } = new ReactiveProperty<SymbolId>();

		public ReactiveProperty<SpawnedSymbolData> SpawnedSymbolData { get; } = new ReactiveProperty<SpawnedSymbolData>();
		public ReactiveProperty<SpawnedSymbolData> DeSpawnedSymbolData { get; } = new ReactiveProperty<SpawnedSymbolData>();

        public int ReelIndex => _rootReelView.ReelIndex;

		public virtual void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public virtual void OnServerConfigsReady(ServiceLocator locator) { }

		public virtual void OnInitialStateReady(ServiceLocator serviceLocator)
		{
			_serviceLocator = serviceLocator;
			_serviceLocator.TryGet(out _metaEvents);

			var targetGameState = _gameStates.Contains(_gameStateModel.GameState) ? _gameStateModel.GameState : _gameStates[0];
			ReelsOutcomeModel outcomeModel = _serviceLocator.Get<ReelsOutcomeServerModel>(targetGameState.ToString());

			string[] reelStripsIds = outcomeModel.ReelStripIds.Value;
			var safeReelIndex = reelStripsIds.Length > _rootReelView.ReelIndex ? _rootReelView.ReelIndex : 0;
            ReelStripDataServerModel reelStrip = _serviceLocator.Get<ReelStripDataServerModel>(reelStripsIds[safeReelIndex]);

			var offset = outcomeModel.Offsets.Value[safeReelIndex];
			_bottomSymbolOffset = _symbolViews.Where(view => view.Location.rowIndex > 0).Count();
			var bottomSymbolIndex = (offset + _bottomSymbolOffset) % reelStrip.Strip.Value.Symbols.Count;

			_symbolProvider = new CustomSymbolProvider(_serviceLocator, _rootReelView.ReelIndex, _dynamicSymbolClientModel, _nextReelStripsClientModel, bottomSymbolIndex);
			_nextReelStripsClientModel.ActiveReelStrips.Value = outcomeModel.ReelStripIds.Value;

			if (_serviceLocator.TryGet(out DynamicSymbolServerModel _dynamicSymbolServerModel, targetGameState.ToString()))
			{
				_dynamicSymbolClientModel.SetDynamicSymbolReplacements(_dynamicSymbolServerModel.DynamicSymbolReplacements.Value);
			}

			_cycleReelView.InitializeSymbolViews();
			SetSymbols();
			_symbolProvider.Initialize(_reelsOutcomeClientModel);
		}

		public void SetSymbols()
		{
			int reelId = _rootReelView.GetInstanceID();
			for (var i = _cycleReelView.SymbolCount; --i >= 0;)
			{
				// HACK: Setting it first to get the ISymbolView from the SymbolCyclingReelView is hacky but we don't have a getter on that class
				// TODO: Request a getter from MFF
				ISymbolView symbolView = _cycleReelView.SetSymbol(i, new SymbolId(1));
				int rowIndex = symbolView.Location.rowIndex;

				_subSymbolEligibilityModel.SetPositionEligibility(reelId, rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, false);
				_cycleReelView.SetSymbol(i, _symbolProvider.Consume());
				_subSymbolEligibilityModel.SetPositionEligibility(reelId, rowIndex, SUB_SYMBOL_ELIGIBILITY_CRITERIA_KEY, true);
            }
		}

		public virtual void SetTopSymbol(int symbolIndex)
		{
			var symbolId = _symbolProvider.Consume();

			var oldSymbol = SpawnedSymbol.Value;
			var oldSymbolData = SpawnedSymbolData.Value;

			var symbolView = _cycleReelView.SetSymbol(symbolIndex, symbolId);

			SpawnedSymbol.Value = symbolId;
			SpawnedSymbolData.Value = new SpawnedSymbolData()
			{
				SymbolHandle = symbolView.Instance,
				SymbolId = symbolId,
				Location = symbolView.Location
			};

			DespawnedSymbol.Value = oldSymbol;
			DeSpawnedSymbolData.Value = new SpawnedSymbolData()
			{
				SymbolHandle = oldSymbolData.SymbolHandle,
				SymbolId = oldSymbolData.SymbolId,
				Location = oldSymbolData.Location
			};
		}

		public virtual void UseResults()
		{
			int index = _reelsOutcomeClientModel.Offsets.Value[_rootReelView.ReelIndex] + _bottomSymbolOffset;
			_symbolProvider.SetIndex(index);
		}

		public virtual void BroadcastReelLandingMetaEvent()
		{
			if (_activeReelsProvider != null)
			{
				_metaEvents?.Broadcast(new ReelLanding
				{
					reelIndex = _rootReelView.ReelIndex,
					reelId = _activeReelsProvider.GetReelId(_rootReelView),
				});
			}
		}
	}
}
