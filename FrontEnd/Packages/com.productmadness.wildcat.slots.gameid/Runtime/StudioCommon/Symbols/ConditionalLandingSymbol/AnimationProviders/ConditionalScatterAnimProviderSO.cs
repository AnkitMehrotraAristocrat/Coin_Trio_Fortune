using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[CreateAssetMenu(fileName = "ConditionalScatterAnimProviderSO",
		menuName = "NMG/Conditional Landing Symbols/Anim Provider/Conditional Scatter")]
	public class ConditionalScatterAnimProviderSO : BaseLandingSymbolAnimProviderSO
	{
		[SerializeField] private int[] _maxPerReel;

		public override BaseLandingSymbolAnimProvider GetAnimProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers)
		{
			return new ConditionalScatterAnimProvider(
				serviceLocator,
				_landingAnimTrigger,
				condition.FeatureTriggerThreshold,
				_maxPerReel,
                GetSymbols(condition.SymbolList),
                eligibilityModifiers
				);
		}
	}
}
