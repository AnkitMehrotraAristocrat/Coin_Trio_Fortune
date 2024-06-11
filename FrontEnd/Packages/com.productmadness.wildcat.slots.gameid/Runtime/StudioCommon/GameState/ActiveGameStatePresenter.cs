using Milan.FrontEnd.Core.v5_1_1;
using Milan.FrontEnd.Core.v5_1_1.Async;
using System.Collections.Generic;
using UnityEngine;

namespace PixelUnited.NMG.Slots.Milan.GAMEID.GameState
{
	/// <summary>
	/// Presenter meant to be placed onto states in which the game is transitioning to another macro state,
	/// such as the transition from the BaseSpin to HoldAndSpin.
	/// Can be placed on recovery states, but GameStateModelInitializer should handle that.
	/// </summary>
	public class ActiveGameStatePresenter : MonoBehaviour, IStatePresenter, ServiceLocator.IHandler
	{
        /// <summary>
        /// The actual game state we will be setting the game state model to.
        /// </summary>
        public GameStateEnum GameState;

		/// <summary>
		/// Handle to the game state model.
		/// </summary>
		[FieldRequiresModel] private GameStateModel _gameStateModel = default;

        public string Tag => this.GetTag();

		public INotifier Notifier {
			get; set;
		}

		public void OnServicesLoaded()
		{
			this.InitializeDependencies();
		}

		public IEnumerator<Yield> Enter()
		{
            UpdateGameState();
			yield break;
		}

		public IEnumerator<Yield> Exit()
		{
			yield break;
		}

		public void UpdateGameState()
		{
			_gameStateModel.SetGameState(GameState);
        }
	}
}
