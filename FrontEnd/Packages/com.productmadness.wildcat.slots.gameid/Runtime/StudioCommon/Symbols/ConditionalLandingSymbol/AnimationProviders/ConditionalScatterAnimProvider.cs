using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	/// <summary>
	/// Model than maintains current state of scatter symbols that land on the reels for a given spin.
	/// </summary>
	public class ConditionalScatterAnimProvider : BaseLandingSymbolAnimProvider
	{
		private List<SymbolId> _symbolIds;
		private ReelsOutcomeClientModel _reelsOutcomeClientModel;
		private ReelWindowDataServerModel _activeReelWindowServerModel;

		private int[] _potentialPerReel;
		private int _thresholdCount = 3; // default to 3, set upon InitializeParams
		private int _maxCount;

		private List<int> _maxPotentialThroughReelIndex = new List<int>();
		private List<int> _countThroughReelIndex = new List<int>();
		private List<bool> _scatterVisibleOnReelIndex = new List<bool>(); // Supports growing reels

		private IDisposable _visibleReelsSubscription;
		private IDisposable _reelWindowSubscription;

		public ConditionalScatterAnimProvider(ServiceLocator serviceLocator, string landingAnimTrigger, int threshold, int[] potentialPerReel, List<SymbolId> symbols, BaseEligibilityModifier[] eligibilityModifiers)
			: base(serviceLocator, landingAnimTrigger, eligibilityModifiers)
		{
			_reelsOutcomeClientModel = serviceLocator.GetOrCreate<ReelsOutcomeClientModel>();
			_thresholdCount = threshold;
			_potentialPerReel = potentialPerReel;
			_symbolIds = symbols;

			_maxCount = _potentialPerReel.Sum();
			InitializeMaxPotentialThroughReelIndex();

			//SetReelWindowModelSubscription();
			SetVisibleReelsSubscriptions();
		}

		private void InitializeMaxPotentialThroughReelIndex()
		{
			int totalCount = 0;
			foreach (var symbolCount in _potentialPerReel)
			{
				totalCount += symbolCount;
				_maxPotentialThroughReelIndex.Add(totalCount);
			}
		}

		public override void OnEnable()
		{
			//SetReelWindowModelSubscription();
			SetVisibleReelsSubscriptions();
		}

		public override void OnDisable()
		{
			DisposeSubscriptions();
		}

		private void SetVisibleReelsSubscriptions()
		{
			_visibleReelsSubscription = _reelsOutcomeClientModel.VisibleReels
				.SkipWhile(visibleReels => visibleReels == null)
				.Skip(1) // skip the first eligible as this is from the prior spin or join
				.Subscribe(result => InitializeCountThroughReelIndex(result));
		}

		// ROB: Commenting this out as we have Milan updates that don't force notify causing null ref errors down the line
		//	due to the _reelWindowServerModel not getting assigned
		//private void SetReelWindowModelSubscription()
		//{
		//	_reelWindowSubscription = _reelsOutcomeClientModel.ReelWindowId
		//		.SkipWhile(result => string.IsNullOrEmpty(result))
		//		.Skip(1) // skip the first eligible as this is from the prior spin or join
		//		.Subscribe(result => _activeReelWindowServerModel = _serviceLocator.Get<ReelWindowDataServerModel>(result));
		//}

		private void DisposeSubscriptions()
		{
			_visibleReelsSubscription?.Dispose();
			_reelWindowSubscription?.Dispose();
		}

		private void InitializeCountThroughReelIndex(SymbolId[][] result)
		{
			if (!_initialized)
			{
				return;
			}

			_countThroughReelIndex.Clear();
			_scatterVisibleOnReelIndex.Clear();

			string reelWindowId = _reelsOutcomeClientModel.ReelWindowId.Value;
			if (string.IsNullOrEmpty(reelWindowId))
			{
				return;
			}
			_activeReelWindowServerModel = _serviceLocator.Get<ReelWindowDataServerModel>(reelWindowId);

			int totalCount = 0;

			foreach (var reel in result)
			{
				int countOnReel = reel.Count(symbol => symbol.Equals(_symbolIds));
				_scatterVisibleOnReelIndex.Add(countOnReel > 0);

				totalCount += countOnReel;
				_countThroughReelIndex.Add(totalCount);
			}
		}

		public override bool ShouldAnimate(int reelIndex, int symbolIndex)
		{
			int width = _activeReelWindowServerModel.Width.Value;
			if (_potentialPerReel.Length != width)
			{
				GameIdLogger.Logger.Error("ConditionalScatterAnimModel.ShouldAnimate - Max Per Reel length does not match visible reels count");
				return false;
			}

			bool isEligible = _maxCount - _maxPotentialThroughReelIndex[reelIndex] + _countThroughReelIndex[reelIndex] >= _thresholdCount;
			bool isVisible = _scatterVisibleOnReelIndex[reelIndex];

			return isEligible && isVisible;
		}
	}
}
