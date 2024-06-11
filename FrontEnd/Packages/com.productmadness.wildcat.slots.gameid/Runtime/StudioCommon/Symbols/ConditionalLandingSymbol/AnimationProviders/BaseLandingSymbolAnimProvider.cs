using Milan.FrontEnd.Core.v5_1_1;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[Preserve]
	public abstract class BaseLandingSymbolAnimProvider
	{
		protected ServiceLocator _serviceLocator;
		protected string _landingAnimTrigger;
		protected BaseEligibilityModifier[] _eligibilityModifiers;
		protected bool _initialized = false;

		public BaseLandingSymbolAnimProvider(ServiceLocator serviceLocator, string landingAnimTrigger, BaseEligibilityModifier[] eligibilityModifiers)
		{
			_serviceLocator = serviceLocator;
			_landingAnimTrigger = landingAnimTrigger;
			_eligibilityModifiers = eligibilityModifiers;
		}

		public abstract bool ShouldAnimate(int reelIndex, int symbolIndex);
		public abstract void OnEnable();
		public abstract void OnDisable();

		public virtual string GetLandingAnimTrigger()
		{
			return _landingAnimTrigger;
		}

		public virtual void Initialize()
		{
			_initialized = true;
		}
	}
}
