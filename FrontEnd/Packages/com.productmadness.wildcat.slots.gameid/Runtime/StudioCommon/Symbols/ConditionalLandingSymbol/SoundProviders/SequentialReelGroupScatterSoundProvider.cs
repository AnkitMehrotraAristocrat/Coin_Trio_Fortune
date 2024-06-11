using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System.Collections.Generic;
using System.Linq;
using Milan.FrontEnd.Bridge.Logging;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    public class SequentialReelGroupScatterSoundProvider : BaseConditionalSymbolSoundProvider
    {
        private IEnumerator<SymbolAudioEvent> _audioEventEnumerator;
        private int? _latestGroupIndex = null;

        public SequentialReelGroupScatterSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, SymbolAudioEvent[] audioEvents, bool considerTriggerAudio, BaseEligibilityModifier[] eligibilityModifiers)
            : base(condition, serviceLocator, symbolLocator, audioEvents, considerTriggerAudio, eligibilityModifiers)
        {
            _audioEventEnumerator = audioEvents.ToList().GetEnumerator();
        }

        public override SymbolAudioEvent GetAudioEvent(Location location, int groupIndex)
		{
            if (groupIndex.Equals(_latestGroupIndex))
			{
                return _audioEventEnumerator.Current;
            }

            _latestGroupIndex = groupIndex;

            if (_audioEventEnumerator.MoveNext())
			{
                return _audioEventEnumerator.Current;
            }
            else
			{
                GameIdLogger.Logger.Error(GetType() + " :: The SequentialReelGroupScatterSoundProvider (ConditionalLandingSymbolView) does not have enough audio events!");
                return null;
            }

		}

        public override void Reset()
		{
            _audioEventEnumerator.Reset();
            _latestGroupIndex = null;
        }
    }
}
