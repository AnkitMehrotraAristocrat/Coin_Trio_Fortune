using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Data;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System;
using System.Linq;
using System.Collections.Generic;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.SymbolData
{
    public class SymbolOutcomeModel : IModel
    {
		private GameStateModel _gameStateModel;
		private Dictionary<GameStateEnum, Dictionary<Location, SymbolOutcomeData>> _symbolData = new Dictionary<GameStateEnum, Dictionary<Location, SymbolOutcomeData>>();
		private IEnumerator<SymbolOutcomeData> _allPrizes = default;

		public Dictionary<GameStateEnum, Dictionary<Location, SymbolOutcomeData>> SymbolData => _symbolData;
		
		public SymbolOutcomeData CurrentPrize { get; private set; } = null;

		public SymbolOutcomeModel(ServiceLocator serviceLocator)
		{
			_gameStateModel = serviceLocator.GetOrCreate<GameStateModel>();
		}

		public void SetSymbolData(GameStateEnum gameState, Dictionary<Location, SymbolOutcomeData> symbolData)
		{
			_symbolData[gameState] = symbolData;
		}

		public bool GetSymbolDataWithLocations(GameStateEnum gameState, out Dictionary<Location, SymbolOutcomeData> symbolDataWithLocations)
		{
			if (!_symbolData.TryGetValue(gameState, out Dictionary<Location, SymbolOutcomeData> gameStateCollection))
			{
				symbolDataWithLocations = null;
				return false;
			}
			else
			{
				symbolDataWithLocations = gameStateCollection;
				return true;
			}
		}

		public bool GetSymbolData(GameStateEnum gameState, Location location, out SymbolOutcomeData symbolData)
		{
			if (!_symbolData.TryGetValue(gameState, out Dictionary<Location, SymbolOutcomeData> gameStateCollection))
			{
				symbolData = null;
				return false;
			}

			if (gameStateCollection.TryGetValue(location, out symbolData))
			{
				return true;
			}

			return false;
		}

		public void SetNextPrize(GameStateEnum gameState)
		{
			if (_allPrizes == null)
			{
				_allPrizes = GetAllPrizes(gameState).GetEnumerator();
			}

			if (_allPrizes.MoveNext())
			{
				SetCurrentPrize(_allPrizes.Current);
			}
			else
			{
				CurrentPrize = null;
				_allPrizes.Dispose();
				_allPrizes = null;
			}
		}
		public List<SymbolOutcomeData> GetAllPrizes(GameStateEnum gameState)
		{
			return _symbolData[gameState].Values.OrderBy(cor => cor.PositionData.X).ToList();
		}

		private void SetCurrentPrize(SymbolOutcomeData prize)
		{
			CurrentPrize = prize;
		}

		public bool IsCurrentPrizeTypeJackpot()
		{
			switch (CurrentPrize?.SymbolData.Skin)
            {
				default:
					return false;
				case CustomPrizeTypes.GrandPrizeType:
					return true;
				case CustomPrizeTypes.MajorPrizeType:
					return true;
				case CustomPrizeTypes.MinorPrizeType:
					return true;
				case CustomPrizeTypes.MiniPrizeType:
					return true;
			}
		}

		public bool IsCurrentPrizeTypeMultiplier()
		{
			return CurrentPrize?.SymbolData.Skin.Equals(CustomPrizeTypes.CreditCorType, StringComparison.OrdinalIgnoreCase) ?? false;
		}
	}
}
