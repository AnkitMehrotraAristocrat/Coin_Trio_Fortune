using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Slots.v5_1_1.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID {
	public interface IReelWindowModifier {
		void UpdateSymbols(ref Dictionary<Location, SymbolId> symbols, string gameState);
	}
}
