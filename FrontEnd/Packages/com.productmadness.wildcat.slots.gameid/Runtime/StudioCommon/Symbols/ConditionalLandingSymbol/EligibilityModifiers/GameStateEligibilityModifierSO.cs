using Milan.FrontEnd.Core.v5_1_1;
using PixelUnited.NMG.Slots.Milan.GAMEID.GameState;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.ConditionalLandingSymbol
{
	[CreateAssetMenu(fileName = "GameStateEligibilityModifierSO",
		menuName = "NMG/Conditional Landing Symbols/Eligibility Modifiers/Game State")]
	public class GameStateEligibilityModifierSO : BaseEligibilityModifier
	{
		[SerializeField] private string[] _eligibleGameStates;

		private GameStateModel _gameStateModel;
		private List<string> _eligibleGameStatesList;

		public override void Initialize(ServiceLocator serviceLocator)
		{
			_gameStateModel = serviceLocator.GetOrCreate<GameStateModel>();
			_eligibleGameStatesList = _eligibleGameStates.Select(state => state.Trim()).Where(state => !string.IsNullOrEmpty(state)).ToList();
		}

		public override bool IsEligible()
		{
			return _eligibleGameStatesList.Count == 0 || _eligibleGameStatesList.Contains(_gameStateModel.GameState.ToString());
		}
	}
}
