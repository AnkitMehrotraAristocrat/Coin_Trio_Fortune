using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	public abstract class BaseTensionSoundProviderSO : ScriptableObject
	{
		public abstract BaseTensionSoundProvider GetSoundProvider(TensionType tensionType, ServiceLocator serviceLocator, SymbolLocator symbolLocator);
	}
}
