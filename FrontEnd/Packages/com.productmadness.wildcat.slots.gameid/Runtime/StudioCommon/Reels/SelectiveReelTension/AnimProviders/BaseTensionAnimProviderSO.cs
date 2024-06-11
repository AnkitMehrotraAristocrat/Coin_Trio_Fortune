using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	public abstract class BaseTensionAnimProviderSO : ScriptableObject
	{
		[SerializeField] protected string _idleAnimTrigger = "idle";

		public abstract BaseTensionAnimProvider GetAnimProvider(TensionType tensionType, ServiceLocator serviceLocator);
	}
}
