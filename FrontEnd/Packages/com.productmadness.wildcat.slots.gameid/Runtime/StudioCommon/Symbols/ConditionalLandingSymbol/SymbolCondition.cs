#region Using

using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	/// <summary>
	/// Scriptable object to be loaded into the ConditionalLandingSymbolView that contains info on what will happen when one or more symbols land.
	/// </summary>
	[CreateAssetMenu(fileName = "SymbolCondition", menuName = "NMG/Conditional Landing Symbols/Symbol Condition")]
	public class SymbolCondition : ScriptableObject
	{
		#region Inspector

        public List<int> SymbolList;
		[Range(0, 128)] public int FeatureTriggerThreshold;
		public BaseLandingSymbolAnimProviderSO AnimProviderSO;
		public BaseLandingSymbolAnimProvider AnimProvider;
		public BaseConditionalSymbolSoundProviderSO SoundProviderSO;
		public BaseConditionalSymbolSoundProvider SoundProvider;
		[SerializeField, SerializeReference] public BaseEligibilityModifier[] EligibilityModifiers;

		#endregion

		public void Initialize(ServiceLocator serviceLocator, SymbolLocator symbolLocator)
		{
			foreach (BaseEligibilityModifier eligibilityModifier in EligibilityModifiers)
			{
				eligibilityModifier.Initialize(serviceLocator);
			}

			if (AnimProviderSO == null)
			{
				throw new NullReferenceException("SymbolCondition is missing the AnimDataSO!");
			}
			AnimProvider = AnimProviderSO.GetAnimProvider(this, serviceLocator, symbolLocator, EligibilityModifiers);

			if (SoundProviderSO != null)
			{
				SoundProvider = SoundProviderSO.GetSoundProvider(this, serviceLocator, symbolLocator, EligibilityModifiers);
			}

			
		}
	}
}
