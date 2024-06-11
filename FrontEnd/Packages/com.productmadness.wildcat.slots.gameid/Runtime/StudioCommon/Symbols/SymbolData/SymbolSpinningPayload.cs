using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
	public class SymbolSpinningData
	{
		public string Id;
		public SymbolSpinningIdData[] Data;
	}

	public class SymbolSpinningIdData
	{
		public int SymbolId;
		public SymbolSpinningGameStateData[] Data;
	}

	public class SymbolSpinningGameStateData
	{
		public string[] GameStates;
		public SymbolData[] Data;
	}
}
