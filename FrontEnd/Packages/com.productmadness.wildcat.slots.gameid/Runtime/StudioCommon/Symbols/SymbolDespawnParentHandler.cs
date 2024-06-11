using Milan.FrontEnd.Slots.v5_1_1.SymbolCore;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	public class SymbolDespawnParentHandler : BaseSymbolSpawnHandler
	{
		[SerializeField] private GameObject _targetParent;

		protected override void OnSymbolDespawn(SpawnedSymbolData symbolData)
		{
			symbolData.SymbolHandle.transform.SetParent(_targetParent.transform, worldPositionStays: false);
		}

		protected override void OnSymbolSpawn(SpawnedSymbolData symbolData)
		{
			// does nothing
		}
	}
}
