#region Using

using System;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using PixelUnited.NMG.Slots.Milan.GAMEID.DynamicSymbols;
using System.Linq;
using UniRx;
using UnityEngine.Scripting;
using System.Collections.Generic;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using PixelUnited.NMG.Slots.Milan.GAMEID.NextReelStrips;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	/// <summary>
	/// Spinoff of the SymbolProvider to support reel mode / dynamic symbols.
	/// </summary>
	[Preserve]
	public class CustomSymbolProvider : BaseUnsubscribable, ISymbolProvider, IDisposable
	{
		#region Private Fields

		private int _index;
		private int _reelIndex;
		private ServiceLocator _serviceLocator;
		private ReelStrip[] _activeReelStrips;
		private DynamicSymbolClientModel _dynamicSymbolClientModel;
		private string[] _activeReelStripNames = new string[] { };

        #endregion

		[Preserve]
		public CustomSymbolProvider(ServiceLocator locator, int reelIndex, DynamicSymbolClientModel dynamicSymbolClientModel, NextReelStripsClientModel nextReelStripsClientModel, int index = 0)
		{
			_serviceLocator = locator;
			_reelIndex = reelIndex;
			_dynamicSymbolClientModel = dynamicSymbolClientModel;
			_index = index;
			SubscribeToNextReelStripsClientModel(nextReelStripsClientModel);
		}

		public void Initialize(ReelsOutcomeClientModel clientModel)
		{
			SubscribeToReelsOutcomeClientModel(clientModel);
		}

        public virtual void Dispose()
        {
			Unsubscribe();
        }

        private void SubscribeToNextReelStripsClientModel(NextReelStripsClientModel clientModel)
		{
			var subscription = clientModel.ActiveReelStrips
				.Where(stripNames => stripNames != null && stripNames.Length > 0)
				.Where(stripNames => !stripNames.SequenceEqual(_activeReelStripNames))
				.Subscribe(stripNames =>
				{
					_activeReelStripNames = stripNames;
					_activeReelStrips = stripNames.Select(stripName => _serviceLocator.Get<ReelStripDataServerModel>(stripName).Strip.Value).ToArray();
				});

			// Add this UniRx subscription to the Subscriptions list so that it can be unsubscribed OnDestroy by UnsubscribeOnDestroy component.
			Subscriptions.Add(subscription);
		}

		private void SubscribeToReelsOutcomeClientModel(ReelsOutcomeClientModel clientModel)
		{
			var subscription = clientModel.ReelStripIds
				.Where(stripNames => stripNames != null && stripNames.Length > 0)
				.Where(stripNames => !stripNames.SequenceEqual(_activeReelStripNames))
				.Subscribe(stripNames =>
				{
					_activeReelStripNames = stripNames;
					_activeReelStrips = stripNames.Select(stripName => _serviceLocator.Get<ReelStripDataServerModel>(stripName).Strip.Value).ToArray();
				});

            // Add this UniRx subscription to the Subscriptions list so that it can be unsubscribed OnDestroy by UnsubscribeOnDestroy component.
            Subscriptions.Add(subscription);
		}

        public void SetIndex(int index)
		{
			_index = index;
		}

		public SymbolId Consume()
		{
			var safeReelIndex = _activeReelStrips.Length > _reelIndex ? _reelIndex : 0;
			var symbolCount = _activeReelStrips[safeReelIndex].Symbols.Count;
			_index = (_index + symbolCount) % symbolCount;
			var symbolId = _activeReelStrips[safeReelIndex].Symbols[_index];

			// Only replace if we have replacements.
            if (_dynamicSymbolClientModel.DynamicSymbolReplacements.Value != null)
            {
                symbolId = ReplaceDynamicSymbol(symbolId);
            }

            --_index;

			return symbolId;
		}

		public bool HasNext()
		{
			return true;
		}

		public void Reset()
		{
		}

		private SymbolId ReplaceDynamicSymbol(SymbolId symbolId)
		{
			if (!_dynamicSymbolClientModel.DynamicSymbolReplacements.Value.ContainsKey(_reelIndex))
			{
				return symbolId;
			}
			if (!_dynamicSymbolClientModel.DynamicSymbolReplacements.Value[_reelIndex].ContainsKey(symbolId))
			{
				return symbolId;
			}
			return _dynamicSymbolClientModel.DynamicSymbolReplacements.Value[_reelIndex][symbolId];
		}
	}
}
