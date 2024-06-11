using Malee;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using System;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID
{
	[Serializable]
	public class SymbolAnimPair
	{
		public int symbolId;
		public string animTrigger;
	}

	[Serializable]
	public class SymbolAnimPairs : ReorderableArray<SymbolAnimPair> { }

	[CreateAssetMenu(fileName = "SymbolAnimationPairSO", menuName = "NMG/Symbol Animation Pair")]
	public class SymbolAnimationPairSO : ScriptableObject
	{
		[SerializeField][Reorderable] private SymbolAnimPairs _symbolAnimPairs;

		public SymbolAnimPairs SymbolAnimationPairs => _symbolAnimPairs;
	}
}
