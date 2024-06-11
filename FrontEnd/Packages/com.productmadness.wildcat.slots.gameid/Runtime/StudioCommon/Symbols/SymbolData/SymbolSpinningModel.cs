using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    public class SymbolSpinningModel : IModel
    {
		private Dictionary<int, Dictionary<GameStateEnum, List<SymbolData>>> _dummySymbolData = new Dictionary<int, Dictionary<GameStateEnum, List<SymbolData>>>(); // symbol id -> game state -> data

		public SymbolSpinningModel(ServiceLocator _)
		{
			// does nothing
		}

		public void SetDummySymbolData(int symbolId, GameStateEnum gameState, List<SymbolData> symbolData)
		{
			if (!_dummySymbolData.ContainsKey(symbolId))
			{
				_dummySymbolData.Add(symbolId, new Dictionary<GameStateEnum, List<SymbolData>>());
			}
			_dummySymbolData[symbolId][gameState] = symbolData;
		}

		public SymbolOutcomeData GetDummySymbol(int symbolId, GameStateEnum gameState)
		{
			int randomIndex = Random.Range(0, _dummySymbolData[symbolId][gameState].Count);
			var dummyOutcomeData = _dummySymbolData[symbolId][gameState][randomIndex];

			var symbolOutcomeData = new SymbolOutcomeData();
			symbolOutcomeData.SymbolData = dummyOutcomeData;

			return symbolOutcomeData;
		}
	}
}
