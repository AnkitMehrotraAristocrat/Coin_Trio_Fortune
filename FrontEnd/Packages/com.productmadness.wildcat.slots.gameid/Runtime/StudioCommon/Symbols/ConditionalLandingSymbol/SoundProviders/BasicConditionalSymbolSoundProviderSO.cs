using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[CreateAssetMenu(fileName = "BasicSoundProviderSO",
		menuName = "NMG/Conditional Landing Symbols/Sound Provider/Basic")]
	public class BasicConditionalSymbolSoundProviderSO : BaseConditionalSymbolSoundProviderSO
	{
		[Reorderable] [SerializeField] private SymbolAudioEvents _audioEvents;

		public override BaseConditionalSymbolSoundProvider GetSoundProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers)
		{
			return new BasicConditionalSymbolSoundProvider(condition, serviceLocator, symbolLocator, _audioEvents.ToArray(), false, eligibilityModifiers);
		}
	}
}
