using Milan.FrontEnd.Core.v5_1_1;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	/// <summary>
	/// Model than maintains current state of scatter symbols that land on the reels for a given spin.
	/// </summary>
	public class ConstantAnimProvider : BaseLandingSymbolAnimProvider
	{
		public ConstantAnimProvider(ServiceLocator serviceLocator, string landingAnimTrigger, BaseEligibilityModifier[] eligibilityModifiers)
			: base(serviceLocator, landingAnimTrigger, eligibilityModifiers)
		{
		}

		public override bool ShouldAnimate(int reelIndex, int symbolIndex)
		{
			return true;
		}

		public override void OnEnable() { }

		public override void OnDisable() { }
	}
}
