using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	[CreateAssetMenu(fileName = "BasicTensionAnimProvider", menuName = "NMG/Selective Reel Tension/Anim Provider/Basic")]
	public class BasicTensionAnimProviderSO : BaseTensionAnimProviderSO
	{
		[SerializeField][Reorderable] private AnimationDefinitions _animDefinitions;

		public override BaseTensionAnimProvider GetAnimProvider(TensionType tensionType, ServiceLocator serviceLocator)
		{
			return new BasicTensionAnimProvider(tensionType, serviceLocator, _idleAnimTrigger, _animDefinitions.ToArray());
		}
	}
}
