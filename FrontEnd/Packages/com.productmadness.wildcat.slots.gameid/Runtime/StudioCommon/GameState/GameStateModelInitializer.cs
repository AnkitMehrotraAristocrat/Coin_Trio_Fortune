using Milan.FrontEnd.Core.v5_1_1;
using UniRx;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.GameState
{
	/// <summary>
	/// Handles initialization of the GameStateModel on join.
	/// </summary>
	public class GameStateModelInitializer : MonoBehaviour, ServiceLocator.IHandler
	{
		[FieldRequiresModel] private GameStateModel _gameStateModel = default;
		[FieldRequiresModel] private JoinStateModel _joinStateModel = default;

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
			SubscribeToJoinStateModel();
		}

		private void SubscribeToJoinStateModel()
		{
			_joinStateModel.Value
				.Where(gameState => !string.IsNullOrEmpty(gameState))
				.Subscribe(gameState => _gameStateModel.SetGameState(gameState)).AddTo(this);
		}
	}
}
