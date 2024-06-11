using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Slots.v5_1_1.ReelsOutcome;
using UnityEngine.Scripting;
using UniRx;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    [Preserve]
    [Serializable]
    public class SymbolAudioEvent
    {
        public string AudioEventName;
        public int Priority;
    }
    
    [Preserve]
    [Serializable]
    public class SymbolAudioEvents : ReorderableArray<SymbolAudioEvent> { };
    
    public abstract class BaseConditionalSymbolSoundProvider
    {
        protected ServiceLocator _serviceLocator;
        protected SymbolLocator _symbolLocator;
        protected ReelsOutcomeClientModel _reelsOutcomeClientModel;
        protected List<SymbolId> _symbolIds;
        protected BaseEligibilityModifier[] _eligibilityModifiers;
        protected bool _initialized = false;

        protected virtual SymbolAudioEvent[] _audioEvents { get; set; }
        protected bool _considerFeatureTriggerAudio = false;

        protected int _featureTriggerThreshold;
        
        protected IDisposable _visibleReelsSubscription;

		protected List<bool> _shouldPlayTriggerSound = new List<bool>();

        public abstract SymbolAudioEvent GetAudioEvent(Location location, int groupIndex);

        public abstract void Reset();

        public BaseConditionalSymbolSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, SymbolAudioEvent[] audioEvents, bool considerTriggerAudio, BaseEligibilityModifier[] eligibilityModifiers) 
		{
            _audioEvents = audioEvents;
            _considerFeatureTriggerAudio = considerTriggerAudio;
            _featureTriggerThreshold = condition.FeatureTriggerThreshold;
            _serviceLocator = serviceLocator;
            _symbolIds = GetSymbols(condition.SymbolList);
            _symbolLocator = symbolLocator;
            _reelsOutcomeClientModel = _serviceLocator.GetOrCreate<ReelsOutcomeClientModel>();
            _eligibilityModifiers = eligibilityModifiers;

            SetVisibleReelsSubscription();
        }

        public virtual void OnEnable()
		{
            SetVisibleReelsSubscription();
		}

        public virtual void OnDisable()
        {
            DisposeSpinModelSubscriptions();
        }

        private void SetVisibleReelsSubscription()
		{
            _visibleReelsSubscription = _reelsOutcomeClientModel.VisibleReels
                .SkipWhile(visibleReels => visibleReels == null)
				.Skip(1) // skip the first one as this is from the prior spin or join
                .Subscribe(result => VisibleReelsUpdated(result));
        }

        private void DisposeSpinModelSubscriptions()
        {
            _visibleReelsSubscription?.Dispose();
        }

        private List<SymbolId> GetSymbols(List<int> symbolIds)
        {
            var symbols = new List<SymbolId>();

            foreach (var id in symbolIds)
            {
                symbols.Add(new SymbolId(id));
            }

            return symbols;
        }

        protected virtual void VisibleReelsUpdated(SymbolId[][] result)
		{
            if (!_initialized)
			{
                return;
			}
            InitializePlayTriggerSound(result);
        }

		protected virtual void InitializePlayTriggerSound(SymbolId[][] result)
		{
            if (!_considerFeatureTriggerAudio)
			{
                return;
			}

            _shouldPlayTriggerSound.Clear();
            int totalCount = 0;

            foreach (var reel in result)
            {
                foreach (var symbolId in _symbolIds)
                {
                    totalCount += reel.Count(symbol => symbol.Equals(symbolId));
                }

                if (totalCount >= _featureTriggerThreshold && !_shouldPlayTriggerSound.Contains(true))
                {
                    _shouldPlayTriggerSound.Add(true);
                }
				else
                {
                    _shouldPlayTriggerSound.Add(false);
                }
            }
        }

        protected virtual bool ShouldPlayTriggerSound(int reelIndex)
        {
            return _considerFeatureTriggerAudio && _shouldPlayTriggerSound[reelIndex];
        }

        public virtual void Initialize()
		{
            _initialized = true;
		}
    }
}
