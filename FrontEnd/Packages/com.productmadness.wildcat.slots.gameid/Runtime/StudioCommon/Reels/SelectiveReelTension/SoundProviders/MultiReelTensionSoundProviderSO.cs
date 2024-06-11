using Malee;
using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.SpinCore;
using UnityEngine;
using UnityEngine.Scripting;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SelectiveReelTension
{
	[Preserve]
	[CreateAssetMenu(fileName = "MultiReelTensionSoundProvider", menuName = "NMG/Selective Reel Tension/Sound Provider/Multiple Reels")]
	public class MultiReelTensionSoundProviderSO : BaseTensionSoundProviderSO
	{
		[SerializeField][Reorderable] private MultiReelAudioDefinitions _audioEventDefinitions;

		public override BaseTensionSoundProvider GetSoundProvider(TensionType tensionType, ServiceLocator serviceLocator, SymbolLocator symbolLocator)
		{
			return new MultiReelTensionSoundProvider(tensionType, serviceLocator, symbolLocator, _audioEventDefinitions.ToArray());
		}
	}
}
