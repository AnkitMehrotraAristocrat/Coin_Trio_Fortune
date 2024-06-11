using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System.Linq;
using PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    [CreateAssetMenu(fileName = "HoldAndSpinScatterSoundProviderSO",
        menuName = "NMG/Conditional Landing Symbols/Sound Provider/Hold And Spin Scatter")]
    public class HoldAndSpinScatterSoundProviderSO : BaseConditionalSymbolSoundProviderSO
    {
        [SerializeField] private bool _holdAndSpinReels;
        [SerializeField] [Reorderable] private HoldAndSpinSymbolAudioEvents _audioEvents;

        public override BaseConditionalSymbolSoundProvider GetSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers)
        {
            bool considerTriggerAudio = _audioEvents.Any(audioEvent => audioEvent.TriggerSound);
            return new HoldAndSpinScatterSoundProvider(condition, serviceLocator, symbolLocator, _audioEvents.ToArray(), considerTriggerAudio, eligibilityModifiers);
        }
    }
}
