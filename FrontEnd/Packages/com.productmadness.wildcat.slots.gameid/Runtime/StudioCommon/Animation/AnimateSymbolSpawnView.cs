using Milan.FrontEnd.Feature.v5_1_1.Extensions;
using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class AnimateSymbolSpawnView : BaseSymbolSpawnHandler
	{
		[SerializeField] SymbolAnimationPairSO _symbolAnimPairs;

		protected override void OnSymbolSpawn(SpawnedSymbolData symbolData)
		{
			SymbolAnimPair symbolAnimPair = _symbolAnimPairs.SymbolAnimationPairs.FirstOrDefault(pair => pair.symbolId.Equals(symbolData.SymbolId.Value));
			if (symbolAnimPair == null)
			{
				return;
			}

			Animator animator = symbolData.SymbolHandle.GetComponentInChildren<Animator>();
			animator.ResetTriggers();
			animator.Play(symbolAnimPair.animTrigger);
			animator.Update(0.1f); // TODO: If this works, change the magic number
		}

		protected override void OnSymbolDespawn(SpawnedSymbolData symbolData)
		{
			// does nothing
		}
	}
}
