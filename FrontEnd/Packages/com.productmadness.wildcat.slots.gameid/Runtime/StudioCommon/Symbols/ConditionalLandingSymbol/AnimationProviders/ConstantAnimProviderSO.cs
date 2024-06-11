using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[CreateAssetMenu(fileName = "ConstantAnimProviderSO",
		menuName = "NMG/Conditional Landing Symbols/Anim Provider/Constant")]
	public class ConstantAnimProviderSO : BaseLandingSymbolAnimProviderSO
	{
		public override BaseLandingSymbolAnimProvider GetAnimProvider(SymbolCondition condition, ServiceLocator serviceLocator, SymbolLocator symbolLocator, BaseEligibilityModifier[] eligibilityModifiers)
		{
			return new ConstantAnimProvider(
				serviceLocator,
				_landingAnimTrigger,
				eligibilityModifiers
				);
		}
	}
}
