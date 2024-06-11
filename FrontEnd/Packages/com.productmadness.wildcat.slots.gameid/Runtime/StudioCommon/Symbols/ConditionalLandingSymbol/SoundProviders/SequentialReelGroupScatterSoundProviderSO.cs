using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
    [CreateAssetMenu(fileName = "SequentialReelGroupScatterSoundProviderSO",
        menuName = "NMG/Conditional Landing Symbols/Sound Provider/Sequential Reel Group Scatter")]
    public class SequentialReelGroupScatterSoundProviderSO : BaseConditionalSymbolSoundProviderSO
    {
        [Reorderable] [SerializeField] private SymbolAudioEvents _audioEvents;

        public override BaseConditionalSymbolSoundProvider GetSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers)
		{
            return new SequentialReelGroupScatterSoundProvider(condition, serviceLocator, symbolLocator, _audioEvents.ToArray(), false, eligibilityModifiers);
        }
    }
}
